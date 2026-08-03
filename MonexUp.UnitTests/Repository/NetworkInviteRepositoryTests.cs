using DB.Infra.Context;
using DB.Infra.Repository;
using Microsoft.EntityFrameworkCore;
using Moq;
using MonexUp.Domain.Interfaces.Factory;
using MonexUp.Domain.Interfaces.Models;
using MonexUp.DTO.Network;

namespace MonexUp.UnitTests.Repository
{
    /// <summary>
    /// Repository-level coverage for the no-account invite table. Exercised
    /// against an in-memory MonexUpContext so the real LINQ filters are asserted.
    /// </summary>
    public class NetworkInviteRepositoryTests
    {
        private static MonexUpContext NewContext()
        {
            var options = new DbContextOptionsBuilder<MonexUpContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new MonexUpContext(options);
        }

        private static NetworkInvite Row(long inviteId, long networkId, string email,
            NetworkInviteStatusEnum status = NetworkInviteStatusEnum.Pending)
        {
            return new NetworkInvite
            {
                InviteId = inviteId,
                NetworkId = networkId,
                Email = email,
                InviterUserId = 100,
                Status = (int)status,
                CreatedAt = new DateTime(2026, 8, 3, 10, 0, 0, DateTimeKind.Unspecified)
            };
        }

        private static MonexUpContext SeededContext(params NetworkInvite[] rows)
        {
            var ctx = NewContext();
            ctx.NetworkInvites.AddRange(rows);
            ctx.SaveChanges();
            return ctx;
        }

        // A factory whose BuildNetworkInviteModel() returns a fresh property-bag
        // model per call, so DbToModel maps each row into its own instance.
        private static INetworkInviteDomainFactory FactoryStub()
        {
            var factory = new Mock<INetworkInviteDomainFactory>();
            factory.Setup(f => f.BuildNetworkInviteModel()).Returns(() =>
            {
                var m = new Mock<INetworkInviteModel>();
                m.SetupAllProperties();
                return m.Object;
            });
            return factory.Object;
        }

        [Fact]
        public void ListPendingByNetwork_ShouldFilterByNetworkAndPendingStatus()
        {
            const long networkId = 4;
            using var ctx = SeededContext(
                Row(1, networkId, "a@x.com"),
                Row(2, networkId, "b@x.com", NetworkInviteStatusEnum.Accepted),
                Row(3, networkId, "c@x.com", NetworkInviteStatusEnum.Cancelled),
                Row(4, networkId: 999, "d@x.com"));
            var repo = new NetworkInviteRepository(ctx);

            var result = repo.ListPendingByNetwork(networkId, FactoryStub());

            Assert.Single(result);
            Assert.Equal("a@x.com", result[0].Email);
        }

        [Fact]
        public void GetPending_ShouldMatchEmailCaseInsensitively()
        {
            const long networkId = 4;
            using var ctx = SeededContext(Row(1, networkId, "someone@x.com"));
            var repo = new NetworkInviteRepository(ctx);

            var found = repo.GetPending(networkId, "  SomeOne@X.com  ", FactoryStub());

            Assert.NotNull(found);
            Assert.Equal(1, found.InviteId);
        }

        [Fact]
        public void GetPending_ShouldIgnoreConsumedAndCancelledRows()
        {
            const long networkId = 4;
            using var ctx = SeededContext(
                Row(1, networkId, "a@x.com", NetworkInviteStatusEnum.Accepted),
                Row(2, networkId, "b@x.com", NetworkInviteStatusEnum.Cancelled));
            var repo = new NetworkInviteRepository(ctx);

            Assert.Null(repo.GetPending(networkId, "a@x.com", FactoryStub()));
            Assert.Null(repo.GetPending(networkId, "b@x.com", FactoryStub()));
        }

        [Fact]
        public void Insert_ShouldNormalizeEmailAndStampCreatedAt()
        {
            using var ctx = NewContext();
            var repo = new NetworkInviteRepository(ctx);
            var factory = FactoryStub();

            var model = factory.BuildNetworkInviteModel();
            model.NetworkId = 4;
            model.Email = "  MiXeD@Case.COM  ";
            model.InviterUserId = 100;
            model.Status = NetworkInviteStatusEnum.Pending;

            var saved = repo.Insert(model, factory);

            Assert.True(saved.InviteId > 0);
            Assert.NotEqual(default, saved.CreatedAt);
            var stored = ctx.NetworkInvites.Single();
            Assert.Equal("mixed@case.com", stored.Email);
        }

        [Fact]
        public void Update_ShouldPersistConsumption()
        {
            const long networkId = 4;
            using var ctx = SeededContext(Row(1, networkId, "a@x.com"));
            var repo = new NetworkInviteRepository(ctx);
            var factory = FactoryStub();

            var model = repo.GetById(1, factory);
            model.Status = NetworkInviteStatusEnum.Accepted;
            model.ConsumedAt = new DateTime(2026, 8, 3, 11, 0, 0, DateTimeKind.Unspecified);
            model.ConsumedUserId = 300;
            repo.Update(model, factory);

            var reloaded = repo.GetById(1, factory);
            Assert.Equal(NetworkInviteStatusEnum.Accepted, reloaded.Status);
            Assert.Equal(300, reloaded.ConsumedUserId);
            Assert.NotNull(reloaded.ConsumedAt);
        }

        [Fact]
        public void Update_ShouldNormalizeConsumedAtToUnspecifiedKind()
        {
            // consumed_at is `timestamp without time zone`. Npgsql throws on a
            // Kind=Utc DateTime for that type, so the repository must normalize it.
            const long networkId = 4;
            using var ctx = SeededContext(Row(1, networkId, "a@x.com"));
            var repo = new NetworkInviteRepository(ctx);
            var factory = FactoryStub();

            var model = repo.GetById(1, factory);
            model.Status = NetworkInviteStatusEnum.Accepted;
            model.ConsumedAt = DateTime.UtcNow; // Kind = Utc
            repo.Update(model, factory);

            var stored = ctx.NetworkInvites.Single();
            Assert.NotNull(stored.ConsumedAt);
            Assert.Equal(DateTimeKind.Unspecified, stored.ConsumedAt!.Value.Kind);
        }

        [Fact]
        public void Update_WithUnknownInviteId_ShouldReturnNull()
        {
            using var ctx = NewContext();
            var repo = new NetworkInviteRepository(ctx);
            var factory = FactoryStub();

            var model = factory.BuildNetworkInviteModel();
            model.InviteId = 4242;

            Assert.Null(repo.Update(model, factory));
        }
    }
}
