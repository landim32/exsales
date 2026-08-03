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
    /// Coverage for the manager-only invite listing/cancellation used by
    /// /admin/teams. The authorization tests are the security constraint of the
    /// feature: pending invites carry invitee e-mail addresses and must never be
    /// readable by a non-manager.
    /// </summary>
    public class NetworkServiceInviteListTests
    {
        private const long NetworkId = 1;
        private const long ManagerId = 100;
        private const string Token = "bearer-token";

        private readonly Mock<IUserClient> _userClient = new();
        private readonly Mock<INetworkDomainFactory> _networkFactory = new();
        private readonly Mock<IUserNetworkDomainFactory> _userNetworkFactory = new();
        private readonly Mock<IUserProfileDomainFactory> _userProfileFactory = new();
        private readonly Mock<IProfileService> _profileService = new();
        private readonly Mock<IFileClient> _fileClient = new();
        private readonly Mock<IInviteTokenSigner> _inviteTokenSigner = new();
        private readonly Mock<INetworkInviteDomainFactory> _networkInviteFactory = new();
        private readonly NetworkService _service;

        public NetworkServiceInviteListTests()
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

        private Mock<IUserNetworkModel> SetupUserNetworkBuilder()
        {
            var builder = new Mock<IUserNetworkModel>();
            builder.SetupAllProperties();
            _userNetworkFactory.Setup(f => f.BuildUserNetworkModel()).Returns(builder.Object);
            return builder;
        }

        private void SetupManagerAccess(Mock<IUserNetworkModel> builder, long networkId = NetworkId)
        {
            var manager = new Mock<IUserNetworkModel>();
            manager.SetupGet(m => m.Role).Returns(UserRoleEnum.NetworkManager);
            builder.Setup(m => m.Get(networkId, ManagerId, It.IsAny<IUserNetworkDomainFactory>()))
                .Returns(manager.Object);
        }

        private void SetupNetwork(string slug = "acme")
        {
            var network = new Mock<INetworkModel>();
            network.SetupGet(m => m.NetworkId).Returns(NetworkId);
            network.SetupGet(m => m.Slug).Returns(slug);

            var builder = new Mock<INetworkModel>();
            builder.Setup(m => m.GetById(NetworkId, It.IsAny<INetworkDomainFactory>())).Returns(network.Object);
            _networkFactory.Setup(f => f.BuildNetworkModel()).Returns(builder.Object);
        }

        private Mock<INetworkInviteModel> SetupInviteBuilder()
        {
            var builder = new Mock<INetworkInviteModel>();
            builder.SetupAllProperties();
            _networkInviteFactory.Setup(f => f.BuildNetworkInviteModel()).Returns(builder.Object);
            return builder;
        }

        private static INetworkInviteModel Invite(long inviteId, string email, long inviterUserId)
        {
            var invite = new Mock<INetworkInviteModel>();
            invite.SetupGet(m => m.InviteId).Returns(inviteId);
            invite.SetupGet(m => m.NetworkId).Returns(NetworkId);
            invite.SetupGet(m => m.Email).Returns(email);
            invite.SetupGet(m => m.InviterUserId).Returns(inviterUserId);
            invite.SetupGet(m => m.Status).Returns(NetworkInviteStatusEnum.Pending);
            return invite.Object;
        }

        // ---- ListPendingInvites ---------------------------------------------

        [Fact]
        public async Task ListPendingInvites_NonManagerCaller_ShouldThrow()
        {
            // Arrange — no membership for the caller → ValidateManager rejects.
            var builder = SetupUserNetworkBuilder();
            builder.Setup(m => m.Get(NetworkId, ManagerId, It.IsAny<IUserNetworkDomainFactory>()))
                .Returns((IUserNetworkModel)null!);
            var inviteBuilder = SetupInviteBuilder();

            // Act & Assert — invitee e-mails must not leak to non-managers.
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _service.ListPendingInvites(NetworkId, ManagerId, Token));
            inviteBuilder.Verify(
                m => m.ListPendingByNetwork(It.IsAny<long>(), It.IsAny<INetworkInviteDomainFactory>()),
                Times.Never);
        }

        [Fact]
        public async Task ListPendingInvites_ShouldReturnPendingInvitesWithResignedToken()
        {
            // Arrange
            var builder = SetupUserNetworkBuilder();
            SetupManagerAccess(builder);
            SetupNetwork("acme");
            var inviteBuilder = SetupInviteBuilder();

            inviteBuilder.Setup(m => m.ListPendingByNetwork(NetworkId, It.IsAny<INetworkInviteDomainFactory>()))
                .Returns(new List<INetworkInviteModel> { Invite(77, "new@x.com", ManagerId) });

            _inviteTokenSigner.Setup(s => s.Sign(NetworkId, ManagerId, 0, false, 77)).Returns("signed-77");
            _userClient.Setup(c => c.GetByIdAsync(ManagerId, Token))
                .ReturnsAsync(new NAuth.DTO.User.UserInfo { UserId = ManagerId, Name = "Gestor" });

            // Act
            var result = await _service.ListPendingInvites(NetworkId, ManagerId, Token);

            // Assert
            var invite = Assert.Single(result);
            Assert.Equal(77, invite.InviteId);
            Assert.Equal("new@x.com", invite.Email);
            Assert.Equal("Gestor", invite.InviterName);
            Assert.Equal("signed-77", invite.Token);
            Assert.Equal("acme", invite.NetworkSlug);
            Assert.Equal(NetworkInviteStatusEnum.Pending, invite.Status);
        }

        [Fact]
        public async Task ListPendingInvites_ShouldResolveEachInviterNameOnlyOnce()
        {
            // Arrange — three invites from the same inviter must cost one NAuth call.
            var builder = SetupUserNetworkBuilder();
            SetupManagerAccess(builder);
            SetupNetwork();
            var inviteBuilder = SetupInviteBuilder();

            inviteBuilder.Setup(m => m.ListPendingByNetwork(NetworkId, It.IsAny<INetworkInviteDomainFactory>()))
                .Returns(new List<INetworkInviteModel>
                {
                    Invite(1, "a@x.com", ManagerId),
                    Invite(2, "b@x.com", ManagerId),
                    Invite(3, "c@x.com", ManagerId)
                });
            _userClient.Setup(c => c.GetByIdAsync(ManagerId, Token))
                .ReturnsAsync(new NAuth.DTO.User.UserInfo { UserId = ManagerId, Name = "Gestor" });

            // Act
            var result = await _service.ListPendingInvites(NetworkId, ManagerId, Token);

            // Assert
            Assert.Equal(3, result.Count);
            // One call for ValidateManager is not made (the caller IS a NetworkManager),
            // so every remaining call comes from name resolution — exactly one.
            _userClient.Verify(c => c.GetByIdAsync(ManagerId, Token), Times.Once);
        }

        // ---- CancelInvite ----------------------------------------------------

        [Fact]
        public async Task CancelInvite_NonManagerCaller_ShouldThrow()
        {
            // Arrange
            var builder = SetupUserNetworkBuilder();
            builder.Setup(m => m.Get(NetworkId, ManagerId, It.IsAny<IUserNetworkDomainFactory>()))
                .Returns((IUserNetworkModel)null!);

            var invite = new Mock<INetworkInviteModel>();
            invite.SetupAllProperties();
            invite.Object.InviteId = 77;
            invite.Object.NetworkId = NetworkId;
            invite.Object.Status = NetworkInviteStatusEnum.Pending;
            var inviteBuilder = SetupInviteBuilder();
            inviteBuilder.Setup(m => m.GetById(77, It.IsAny<INetworkInviteDomainFactory>()))
                .Returns(invite.Object);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _service.CancelInvite(77, ManagerId, Token));
            invite.Verify(m => m.Update(It.IsAny<INetworkInviteDomainFactory>()), Times.Never);
        }

        [Fact]
        public async Task CancelInvite_ShouldSetCancelled()
        {
            // Arrange
            var builder = SetupUserNetworkBuilder();
            SetupManagerAccess(builder);

            var invite = new Mock<INetworkInviteModel>();
            invite.SetupAllProperties();
            invite.Object.InviteId = 77;
            invite.Object.NetworkId = NetworkId;
            invite.Object.Status = NetworkInviteStatusEnum.Pending;
            invite.Setup(m => m.Update(It.IsAny<INetworkInviteDomainFactory>())).Returns(invite.Object);
            var inviteBuilder = SetupInviteBuilder();
            inviteBuilder.Setup(m => m.GetById(77, It.IsAny<INetworkInviteDomainFactory>()))
                .Returns(invite.Object);

            // Act
            await _service.CancelInvite(77, ManagerId, Token);

            // Assert
            Assert.Equal(NetworkInviteStatusEnum.Cancelled, invite.Object.Status);
            invite.Verify(m => m.Update(It.IsAny<INetworkInviteDomainFactory>()), Times.Once);
        }

        [Fact]
        public async Task CancelInvite_AlreadyCancelled_ShouldThrowAndNotUpdate()
        {
            // Arrange
            var builder = SetupUserNetworkBuilder();
            SetupManagerAccess(builder);

            var invite = new Mock<INetworkInviteModel>();
            invite.SetupAllProperties();
            invite.Object.InviteId = 77;
            invite.Object.NetworkId = NetworkId;
            invite.Object.Status = NetworkInviteStatusEnum.Cancelled;
            var inviteBuilder = SetupInviteBuilder();
            inviteBuilder.Setup(m => m.GetById(77, It.IsAny<INetworkInviteDomainFactory>()))
                .Returns(invite.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.CancelInvite(77, ManagerId, Token));
            invite.Verify(m => m.Update(It.IsAny<INetworkInviteDomainFactory>()), Times.Never);
        }

        [Fact]
        public async Task CancelInvite_UnknownInvite_ShouldThrowBeforeAuthorizing()
        {
            // Arrange — no invite row at all.
            var inviteBuilder = SetupInviteBuilder();
            inviteBuilder.Setup(m => m.GetById(It.IsAny<long>(), It.IsAny<INetworkInviteDomainFactory>()))
                .Returns((INetworkInviteModel)null!);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.CancelInvite(4242, ManagerId, Token));
        }
    }
}
