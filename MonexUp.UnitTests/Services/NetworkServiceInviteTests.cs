using Microsoft.Extensions.Logging;
using Moq;
using MonexUp.Domain.Impl.Services;
using MonexUp.Domain.Interfaces.Factory;
using MonexUp.Domain.Interfaces.Models;
using MonexUp.Domain.Interfaces.Services;
using MonexUp.DTO.Network;
using MonexUp.DTO.User;
using NAuth.ACL.Interfaces;
using zTools.ACL.Interfaces;

namespace MonexUp.UnitTests.Services
{
    /// <summary>
    /// Tests for the referrer-invite flows on NetworkService:
    /// InviteByEmail / JoinFromInvite / AcceptInvite / DeclineInvite, plus the
    /// referrer regression on the self-service RequestAccess path.
    /// </summary>
    public class NetworkServiceInviteTests
    {
        private const long NetworkId = 1;
        private const long InviterId = 100;
        private const long InviteeId = 200;
        private const string Token = "bearer-token";
        private const string InviteToken = "invite-token";

        private readonly Mock<IUserClient> _userClient = new();
        private readonly Mock<INetworkDomainFactory> _networkFactory = new();
        private readonly Mock<IUserNetworkDomainFactory> _userNetworkFactory = new();
        private readonly Mock<IUserProfileDomainFactory> _userProfileFactory = new();
        private readonly Mock<IProfileService> _profileService = new();
        private readonly Mock<IFileClient> _fileClient = new();
        private readonly Mock<IInviteTokenSigner> _inviteTokenSigner = new();
        private readonly Mock<INetworkInviteDomainFactory> _networkInviteFactory = new();
        private readonly NetworkService _service;

        public NetworkServiceInviteTests()
        {
            _service = new NetworkService(
                _userClient.Object,
                _networkFactory.Object,
                _userNetworkFactory.Object,
                _userProfileFactory.Object,
                _profileService.Object,
                _fileClient.Object,
                _inviteTokenSigner.Object,
                _networkInviteFactory.Object,
                new Mock<ILogger<NetworkService>>().Object
            );
        }

        // ---- shared setup helpers -------------------------------------------------

        /// <summary>
        /// Builds the shared UserNetwork "builder" mock returned by the factory.
        /// Its Insert returns itself so CreatePendingMembership's property writes
        /// are observable via VerifySet, mirroring the existing NetworkServiceTests style.
        /// </summary>
        private Mock<IUserNetworkModel> SetupUserNetworkBuilder()
        {
            var builder = new Mock<IUserNetworkModel>();
            builder.SetupAllProperties();
            builder.Setup(m => m.Insert(It.IsAny<IUserNetworkDomainFactory>())).Returns(builder.Object);
            builder.Setup(m => m.Update(It.IsAny<IUserNetworkDomainFactory>())).Returns(builder.Object);
            _userNetworkFactory.Setup(f => f.BuildUserNetworkModel()).Returns(builder.Object);
            return builder;
        }

        /// <summary>Configures the manager-authorization lookup (Get by managerId).</summary>
        private void SetupManagerAccess(Mock<IUserNetworkModel> builder, long managerId,
            UserRoleEnum role = UserRoleEnum.NetworkManager)
        {
            var manager = new Mock<IUserNetworkModel>();
            manager.SetupGet(m => m.Role).Returns(role);
            builder.Setup(m => m.Get(NetworkId, managerId, It.IsAny<IUserNetworkDomainFactory>()))
                .Returns(manager.Object);
        }

        private void SetupNetwork(string slug = "test-net")
        {
            var network = new Mock<INetworkModel>();
            network.SetupGet(m => m.NetworkId).Returns(NetworkId);
            network.SetupGet(m => m.Slug).Returns(slug);

            var builder = new Mock<INetworkModel>();
            builder.Setup(m => m.GetById(NetworkId, It.IsAny<INetworkDomainFactory>())).Returns(network.Object);
            _networkFactory.Setup(f => f.BuildNetworkModel()).Returns(builder.Object);
        }

        private void SetupProfiles(long profileId = 42, int level = 2)
        {
            var profile = new Mock<IUserProfileModel>();
            profile.SetupGet(m => m.ProfileId).Returns(profileId);
            profile.SetupGet(m => m.Level).Returns(level);

            var builder = new Mock<IUserProfileModel>();
            builder.Setup(m => m.ListByNetwork(It.IsAny<long>(), It.IsAny<IUserProfileDomainFactory>()))
                .Returns(new List<IUserProfileModel> { profile.Object });
            _userProfileFactory.Setup(f => f.BuildUserProfileModel()).Returns(builder.Object);
        }

        private static NAuth.DTO.User.UserInfo Invitee(long userId)
            => new NAuth.DTO.User.UserInfo { UserId = userId, Name = "Invitee", Email = "invitee@x.com" };

        private void SetupVerify(InviteTokenPayload payload, bool result = true)
        {
            _inviteTokenSigner.Setup(s => s.TryVerify(It.IsAny<string>(), out payload)).Returns(result);
        }

        /// <summary>
        /// Builds the shared NetworkInvite "builder" mock returned by the factory,
        /// mirroring SetupUserNetworkBuilder. Insert echoes the model back so the
        /// service can read the generated InviteId off it.
        /// </summary>
        private Mock<INetworkInviteModel> SetupInviteBuilder(long inviteId = 55)
        {
            var builder = new Mock<INetworkInviteModel>();
            builder.SetupAllProperties();
            builder.Setup(m => m.Insert(It.IsAny<INetworkInviteDomainFactory>()))
                .Returns(() =>
                {
                    builder.Object.InviteId = inviteId;
                    return builder.Object;
                });
            builder.Setup(m => m.Update(It.IsAny<INetworkInviteDomainFactory>())).Returns(builder.Object);
            _networkInviteFactory.Setup(f => f.BuildNetworkInviteModel()).Returns(builder.Object);
            return builder;
        }

        // ---- InviteByEmail --------------------------------------------------------

        [Fact]
        public async Task InviteByEmail_NoAccount_ShouldPersistPendingInvite()
        {
            // Arrange
            var builder = SetupUserNetworkBuilder();
            SetupManagerAccess(builder, InviterId);
            SetupNetwork("acme");
            SetupProfiles();
            var inviteBuilder = SetupInviteBuilder(inviteId: 77);

            // NAuth throws when the email has no account.
            _userClient.Setup(c => c.GetByEmailAsync("New@X.com")).ThrowsAsync(new Exception("404"));
            _inviteTokenSigner.Setup(s => s.Sign(NetworkId, InviterId, 0, false, 77)).Returns("no-account-token");

            // Act
            var result = await _service.InviteByEmail(NetworkId, "New@X.com", InviterId, Token);

            // Assert
            Assert.True(result.Sucesso);
            Assert.False(result.HasAccount);
            Assert.False(result.AlreadyMember);
            Assert.Equal("no-account-token", result.Token);
            Assert.Equal("acme", result.NetworkSlug);
            _inviteTokenSigner.Verify(s => s.Sign(NetworkId, InviterId, 0, false, 77), Times.Once);

            // The invite is persisted with a normalized e-mail...
            inviteBuilder.Verify(m => m.Insert(It.IsAny<INetworkInviteDomainFactory>()), Times.Once);
            inviteBuilder.VerifySet(m => m.Email = "new@x.com");
            inviteBuilder.VerifySet(m => m.InviterUserId = InviterId);
            inviteBuilder.VerifySet(m => m.Status = NetworkInviteStatusEnum.Pending);
            // ...and NO membership row is created (there is no user id yet).
            builder.Verify(m => m.Insert(It.IsAny<IUserNetworkDomainFactory>()), Times.Never);
        }

        [Fact]
        public async Task InviteByEmail_NoAccountTwice_ShouldReusePendingInvite()
        {
            // Arrange
            var builder = SetupUserNetworkBuilder();
            SetupManagerAccess(builder, InviterId);
            SetupNetwork("acme");
            SetupProfiles();
            var inviteBuilder = SetupInviteBuilder();

            var pending = new Mock<INetworkInviteModel>();
            pending.SetupGet(m => m.InviteId).Returns(91);
            inviteBuilder.Setup(m => m.GetPending(NetworkId, "new@x.com", It.IsAny<INetworkInviteDomainFactory>()))
                .Returns(pending.Object);

            _userClient.Setup(c => c.GetByEmailAsync("new@x.com")).ThrowsAsync(new Exception("404"));
            _inviteTokenSigner.Setup(s => s.Sign(NetworkId, InviterId, 0, false, 91)).Returns("reused-token");

            // Act
            var result = await _service.InviteByEmail(NetworkId, "new@x.com", InviterId, Token);

            // Assert — idempotent: the existing pending row is reused, not duplicated.
            Assert.True(result.Sucesso);
            Assert.Equal("reused-token", result.Token);
            inviteBuilder.Verify(m => m.Insert(It.IsAny<INetworkInviteDomainFactory>()), Times.Never);
        }

        [Fact]
        public async Task InviteByEmail_ExistingAccount_ShouldCreatePendingMembershipWithReferrer()
        {
            // Arrange
            var builder = SetupUserNetworkBuilder();
            SetupManagerAccess(builder, InviterId);
            SetupNetwork();
            SetupProfiles();

            _userClient.Setup(c => c.GetByEmailAsync("invitee@x.com")).ReturnsAsync(Invitee(InviteeId));
            // No existing membership for the invitee.
            builder.Setup(m => m.Get(NetworkId, InviteeId, It.IsAny<IUserNetworkDomainFactory>()))
                .Returns((IUserNetworkModel)null!);
            _inviteTokenSigner.Setup(s => s.Sign(NetworkId, InviterId, InviteeId, true, 0)).Returns("existing-token");

            // Act
            var result = await _service.InviteByEmail(NetworkId, "invitee@x.com", InviterId, Token);

            // Assert
            Assert.True(result.Sucesso);
            Assert.True(result.HasAccount);
            Assert.False(result.AlreadyMember);
            Assert.Equal("existing-token", result.Token);
            builder.Verify(m => m.Insert(It.IsAny<IUserNetworkDomainFactory>()), Times.Once);
            builder.VerifySet(m => m.Status = UserNetworkStatusEnum.WaitForApproval);
            builder.VerifySet(m => m.Role = UserRoleEnum.Seller);
            builder.VerifySet(m => m.ReferrerId = InviterId);
            // Provenance marker — this is what renders the "Convidado" badge.
            builder.VerifySet(m => m.InvitedAt = It.IsAny<DateTime?>(), Times.Once);
        }

        [Fact]
        public async Task InviteByEmail_AlreadyActiveMember_ShouldReturnAlreadyMemberWithoutDuplicate()
        {
            // Arrange
            var builder = SetupUserNetworkBuilder();
            SetupManagerAccess(builder, InviterId);
            SetupNetwork();
            SetupProfiles();

            var existing = new Mock<IUserNetworkModel>();
            existing.SetupGet(m => m.Status).Returns(UserNetworkStatusEnum.Active);
            builder.Setup(m => m.Get(NetworkId, InviteeId, It.IsAny<IUserNetworkDomainFactory>()))
                .Returns(existing.Object);

            _userClient.Setup(c => c.GetByEmailAsync("invitee@x.com")).ReturnsAsync(Invitee(InviteeId));
            _inviteTokenSigner.Setup(s => s.Sign(NetworkId, InviterId, InviteeId, true, 0)).Returns("existing-token");

            // Act
            var result = await _service.InviteByEmail(NetworkId, "invitee@x.com", InviterId, Token);

            // Assert
            Assert.True(result.Sucesso);
            Assert.True(result.HasAccount);
            Assert.True(result.AlreadyMember);
            builder.Verify(m => m.Insert(It.IsAny<IUserNetworkDomainFactory>()), Times.Never);
            existing.Verify(m => m.Update(It.IsAny<IUserNetworkDomainFactory>()), Times.Never);
        }

        [Fact]
        public async Task InviteByEmail_NonManagerCaller_ShouldThrow()
        {
            // Arrange
            var builder = SetupUserNetworkBuilder();
            // No manager access configured → Get returns null → ValidateManager throws.
            builder.Setup(m => m.Get(NetworkId, InviterId, It.IsAny<IUserNetworkDomainFactory>()))
                .Returns((IUserNetworkModel)null!);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _service.InviteByEmail(NetworkId, "invitee@x.com", InviterId, Token));
            builder.Verify(m => m.Insert(It.IsAny<IUserNetworkDomainFactory>()), Times.Never);
        }

        [Fact]
        public async Task InviteByEmail_InvalidEmail_ShouldReturnFailure()
        {
            // Act
            var result = await _service.InviteByEmail(NetworkId, "not-an-email", InviterId, Token);

            // Assert
            Assert.False(result.Sucesso);
        }

        [Fact]
        public async Task InviteByEmail_SelfInvite_ShouldReturnFailure()
        {
            // Arrange
            var builder = SetupUserNetworkBuilder();
            SetupManagerAccess(builder, InviterId);
            SetupNetwork();
            SetupProfiles();

            // Invitee resolves to the inviter itself.
            _userClient.Setup(c => c.GetByEmailAsync("self@x.com")).ReturnsAsync(Invitee(InviterId));

            // Act
            var result = await _service.InviteByEmail(NetworkId, "self@x.com", InviterId, Token);

            // Assert
            Assert.False(result.Sucesso);
            builder.Verify(m => m.Insert(It.IsAny<IUserNetworkDomainFactory>()), Times.Never);
            _inviteTokenSigner.Verify(s => s.Sign(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<bool>(), It.IsAny<long>()), Times.Never);
        }

        // ---- JoinFromInvite -------------------------------------------------------

        [Fact]
        public async Task JoinFromInvite_ShouldEnrollWaitForApprovalWithReferrer_AndBeIdempotent()
        {
            // Arrange
            long joinerId = 300;
            var builder = SetupUserNetworkBuilder();
            SetupProfiles();
            SetupVerify(new InviteTokenPayload
            {
                NetworkId = NetworkId,
                InviterUserId = InviterId,
                TargetUserId = joinerId,
                HasAccount = true
            });

            // First lookup: no membership → create. Second lookup: pending exists → no-op.
            var pending = new Mock<IUserNetworkModel>();
            pending.SetupGet(m => m.Status).Returns(UserNetworkStatusEnum.WaitForApproval);
            builder.SetupSequence(m => m.Get(NetworkId, joinerId, It.IsAny<IUserNetworkDomainFactory>()))
                .Returns((IUserNetworkModel)null!)
                .Returns(pending.Object);

            // Act
            await _service.JoinFromInvite(joinerId, InviteToken);
            await _service.JoinFromInvite(joinerId, InviteToken);

            // Assert — single insert across both calls (idempotent).
            builder.Verify(m => m.Insert(It.IsAny<IUserNetworkDomainFactory>()), Times.Once);
            builder.VerifySet(m => m.Status = UserNetworkStatusEnum.WaitForApproval);
            builder.VerifySet(m => m.ReferrerId = InviterId);
        }

        [Fact]
        public async Task JoinFromInvite_WithInviteId_ShouldConsumeInviteAndMarkInvitedAt()
        {
            // Arrange
            long joinerId = 300;
            var builder = SetupUserNetworkBuilder();
            SetupProfiles();
            var inviteBuilder = SetupInviteBuilder();
            SetupVerify(new InviteTokenPayload
            {
                NetworkId = NetworkId,
                InviterUserId = InviterId,
                TargetUserId = 0,
                HasAccount = false,
                InviteId = 77
            });

            builder.Setup(m => m.Get(NetworkId, joinerId, It.IsAny<IUserNetworkDomainFactory>()))
                .Returns((IUserNetworkModel)null!);

            var invite = new Mock<INetworkInviteModel>();
            invite.SetupAllProperties();
            invite.Object.NetworkId = NetworkId;
            invite.Object.Status = NetworkInviteStatusEnum.Pending;
            invite.Setup(m => m.Update(It.IsAny<INetworkInviteDomainFactory>())).Returns(invite.Object);
            inviteBuilder.Setup(m => m.GetById(77, It.IsAny<INetworkInviteDomainFactory>())).Returns(invite.Object);

            // Act
            await _service.JoinFromInvite(joinerId, InviteToken);

            // Assert — membership created as invited, invite row consumed.
            builder.Verify(m => m.Insert(It.IsAny<IUserNetworkDomainFactory>()), Times.Once);
            builder.VerifySet(m => m.InvitedAt = It.IsAny<DateTime?>(), Times.Once);
            invite.VerifySet(m => m.Status = NetworkInviteStatusEnum.Accepted);
            invite.VerifySet(m => m.ConsumedUserId = joinerId);
            invite.Verify(m => m.Update(It.IsAny<INetworkInviteDomainFactory>()), Times.Once);
        }

        [Fact]
        public async Task JoinFromInvite_WithAlreadyAcceptedInvite_ShouldNotUpdateItAgain()
        {
            // Arrange
            long joinerId = 300;
            var builder = SetupUserNetworkBuilder();
            SetupProfiles();
            var inviteBuilder = SetupInviteBuilder();
            SetupVerify(new InviteTokenPayload
            {
                NetworkId = NetworkId,
                InviterUserId = InviterId,
                TargetUserId = 0,
                HasAccount = false,
                InviteId = 77
            });

            builder.Setup(m => m.Get(NetworkId, joinerId, It.IsAny<IUserNetworkDomainFactory>()))
                .Returns((IUserNetworkModel)null!);

            var invite = new Mock<INetworkInviteModel>();
            invite.SetupGet(m => m.NetworkId).Returns(NetworkId);
            invite.SetupGet(m => m.Status).Returns(NetworkInviteStatusEnum.Accepted);
            inviteBuilder.Setup(m => m.GetById(77, It.IsAny<INetworkInviteDomainFactory>())).Returns(invite.Object);

            // Act
            await _service.JoinFromInvite(joinerId, InviteToken);

            // Assert — idempotent, the consumed invite is left alone.
            invite.Verify(m => m.Update(It.IsAny<INetworkInviteDomainFactory>()), Times.Never);
        }

        [Fact]
        public async Task JoinFromInvite_WithLegacyToken_ShouldEnrollWithoutTouchingInvites()
        {
            // Arrange — legacy 4-segment token: InviteId 0, nothing was ever persisted.
            long joinerId = 300;
            var builder = SetupUserNetworkBuilder();
            SetupProfiles();
            var inviteBuilder = SetupInviteBuilder();
            SetupVerify(new InviteTokenPayload
            {
                NetworkId = NetworkId,
                InviterUserId = InviterId,
                TargetUserId = 0,
                HasAccount = false,
                InviteId = 0
            });

            builder.Setup(m => m.Get(NetworkId, joinerId, It.IsAny<IUserNetworkDomainFactory>()))
                .Returns((IUserNetworkModel)null!);

            // Act
            await _service.JoinFromInvite(joinerId, InviteToken);

            // Assert
            builder.Verify(m => m.Insert(It.IsAny<IUserNetworkDomainFactory>()), Times.Once);
            inviteBuilder.Verify(m => m.GetById(It.IsAny<long>(), It.IsAny<INetworkInviteDomainFactory>()), Times.Never);
        }

        [Fact]
        public async Task JoinFromInvite_WithTamperedToken_ShouldThrow()
        {
            // Arrange — TryVerify fails.
            SetupVerify(null!, result: false);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.JoinFromInvite(300, InviteToken));
        }

        // ---- DeclineInvite --------------------------------------------------------

        [Fact]
        public async Task DeclineInvite_ShouldSetPendingMembershipInactive()
        {
            // Arrange
            var builder = SetupUserNetworkBuilder();
            SetupVerify(new InviteTokenPayload
            {
                NetworkId = NetworkId,
                InviterUserId = InviterId,
                TargetUserId = InviteeId,
                HasAccount = true
            });

            var existing = new Mock<IUserNetworkModel>();
            existing.SetupGet(m => m.Status).Returns(UserNetworkStatusEnum.WaitForApproval);
            existing.Setup(m => m.Update(It.IsAny<IUserNetworkDomainFactory>())).Returns(existing.Object);
            builder.Setup(m => m.Get(NetworkId, InviteeId, It.IsAny<IUserNetworkDomainFactory>()))
                .Returns(existing.Object);

            // Act — caller is the invite target.
            await _service.DeclineInvite(InviteeId, InviteToken);

            // Assert
            existing.VerifySet(m => m.Status = UserNetworkStatusEnum.Inactive);
            existing.Verify(m => m.Update(It.IsAny<IUserNetworkDomainFactory>()), Times.Once);
        }

        [Fact]
        public async Task DeclineInvite_ByNonTargetUser_ShouldThrowUnauthorized()
        {
            // Arrange — token targets InviteeId, but a different caller invokes it.
            SetupVerify(new InviteTokenPayload
            {
                NetworkId = NetworkId,
                InviterUserId = InviterId,
                TargetUserId = InviteeId,
                HasAccount = true
            });

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _service.DeclineInvite(999, InviteToken));
        }

        // ---- AcceptInvite ---------------------------------------------------------

        [Fact]
        public async Task AcceptInvite_ByNonTargetUser_ShouldThrowUnauthorized()
        {
            // Arrange
            SetupVerify(new InviteTokenPayload
            {
                NetworkId = NetworkId,
                InviterUserId = InviterId,
                TargetUserId = InviteeId,
                HasAccount = true
            });

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _service.AcceptInvite(999, InviteToken));
        }

        [Fact]
        public async Task AcceptInvite_WhenAlreadyPending_ShouldNotChangeStatusOrInsert()
        {
            // Arrange
            var builder = SetupUserNetworkBuilder();
            SetupVerify(new InviteTokenPayload
            {
                NetworkId = NetworkId,
                InviterUserId = InviterId,
                TargetUserId = InviteeId,
                HasAccount = true
            });

            var existing = new Mock<IUserNetworkModel>();
            existing.SetupGet(m => m.Status).Returns(UserNetworkStatusEnum.WaitForApproval);
            builder.Setup(m => m.Get(NetworkId, InviteeId, It.IsAny<IUserNetworkDomainFactory>()))
                .Returns(existing.Object);

            // Act
            await _service.AcceptInvite(InviteeId, InviteToken);

            // Assert — pending row stays as-is; no insert, no update.
            builder.Verify(m => m.Insert(It.IsAny<IUserNetworkDomainFactory>()), Times.Never);
            existing.Verify(m => m.Update(It.IsAny<IUserNetworkDomainFactory>()), Times.Never);
        }

        // ---- RequestAccess referrer regression ------------------------------------

        [Fact]
        public void RequestAccess_WithNullReferrer_ShouldCreateMembershipWithNullReferrer()
        {
            // Arrange
            var builder = SetupUserNetworkBuilder();
            SetupProfiles();

            // Act
            _service.RequestAccess(NetworkId, InviteeId, null);

            // Assert
            builder.Verify(m => m.Insert(It.IsAny<IUserNetworkDomainFactory>()), Times.Once);
            builder.VerifySet(m => m.Status = UserNetworkStatusEnum.WaitForApproval);
            builder.VerifySet(m => m.ReferrerId = null);
            // Gap-B regression guard: a self-service request is NOT an invite, so it
            // must never render the "Convidado" badge.
            builder.VerifySet(m => m.InvitedAt = null);
        }
    }
}
