using Microsoft.Extensions.Configuration;
using MonexUp.Domain.Impl.Services;
using MonexUp.Domain.Interfaces.Services;

namespace MonexUp.UnitTests.Services
{
    public class InviteTokenSignerTests
    {
        private const string Secret = "super-secret-invite-key-for-tests";

        private static InviteTokenSigner BuildSigner(string secret = Secret)
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Invite:Secret"] = secret
                })
                .Build();
            return new InviteTokenSigner(configuration);
        }

        [Fact]
        public void Ctor_WithMissingSecret_ShouldThrow()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>())
                .Build();

            Assert.Throws<InvalidOperationException>(() => new InviteTokenSigner(configuration));
        }

        [Fact]
        public void SignThenVerify_ShouldRoundTripSamePayload()
        {
            // Arrange
            var signer = BuildSigner();
            var token = signer.Sign(networkId: 7, inviterUserId: 100, targetUserId: 200, hasAccount: true);

            // Act
            var ok = signer.TryVerify(token, out InviteTokenPayload payload);

            // Assert
            Assert.True(ok);
            Assert.NotNull(payload);
            Assert.Equal(7, payload.NetworkId);
            Assert.Equal(100, payload.InviterUserId);
            Assert.Equal(200, payload.TargetUserId);
            Assert.True(payload.HasAccount);
        }

        [Fact]
        public void SignThenVerify_NoAccountInvite_ShouldPreserveFlagAndZeroTarget()
        {
            // Arrange
            var signer = BuildSigner();
            var token = signer.Sign(networkId: 3, inviterUserId: 55, targetUserId: 0, hasAccount: false);

            // Act
            var ok = signer.TryVerify(token, out InviteTokenPayload payload);

            // Assert
            Assert.True(ok);
            Assert.Equal(3, payload.NetworkId);
            Assert.Equal(55, payload.InviterUserId);
            Assert.Equal(0, payload.TargetUserId);
            Assert.False(payload.HasAccount);
        }

        [Fact]
        public void SignThenVerify_WithInviteId_ShouldRoundTripFifthSegment()
        {
            // Arrange
            var signer = BuildSigner();
            var token = signer.Sign(networkId: 3, inviterUserId: 55, targetUserId: 0, hasAccount: false, inviteId: 77);

            // Act
            var ok = signer.TryVerify(token, out InviteTokenPayload payload);

            // Assert
            Assert.True(ok);
            Assert.Equal(3, payload.NetworkId);
            Assert.Equal(55, payload.InviterUserId);
            Assert.Equal(0, payload.TargetUserId);
            Assert.False(payload.HasAccount);
            Assert.Equal(77, payload.InviteId);
        }

        [Fact]
        public void Sign_WithoutInviteId_ShouldEmitTheLegacyFourSegmentPayload()
        {
            // The 5th segment must be omitted when there is no invite row, so
            // existing-account tokens stay byte-identical to the ones in the wild.
            var signer = BuildSigner();

            var token = signer.Sign(7, 100, 200, true);
            var payloadText = Base64UrlDecode(token.Split('.')[0]);

            Assert.Equal("7|100|200|1", payloadText);
        }

        [Fact]
        public void TryVerify_WithLegacyFourSegmentToken_ShouldSucceedWithZeroInviteId()
        {
            // A token issued before invite persistence existed must keep working.
            var signer = BuildSigner();
            var token = signer.Sign(3, 55, 0, false);

            var ok = signer.TryVerify(token, out InviteTokenPayload payload);

            Assert.True(ok);
            Assert.Equal(0, payload.InviteId);
        }

        [Fact]
        public void TryVerify_WithTamperedInviteIdSegment_ShouldFail()
        {
            // Arrange
            var signer = BuildSigner();
            var token = signer.Sign(3, 55, 0, false, inviteId: 77);
            var parts = token.Split('.');

            // Swap the invite id while keeping the original signature.
            var forgedPayload = Base64UrlEncode("3|55|0|0|999");
            var tampered = $"{forgedPayload}.{parts[1]}";

            // Act
            var ok = signer.TryVerify(tampered, out InviteTokenPayload payload);

            // Assert
            Assert.False(ok);
            Assert.Null(payload);
        }

        [Fact]
        public void TryVerify_WithTamperedPayloadSegment_ShouldFail()
        {
            // Arrange
            var signer = BuildSigner();
            var token = signer.Sign(1, 2, 3, true);
            var parts = token.Split('.');

            // Re-encode a different payload while keeping the original signature.
            var forgedPayload = Base64UrlEncode("999|2|3|1");
            var tampered = $"{forgedPayload}.{parts[1]}";

            // Act
            var ok = signer.TryVerify(tampered, out InviteTokenPayload payload);

            // Assert
            Assert.False(ok);
            Assert.Null(payload);
        }

        [Fact]
        public void TryVerify_WithTamperedSignature_ShouldFail()
        {
            // Arrange
            var signer = BuildSigner();
            var token = signer.Sign(1, 2, 3, true);
            var parts = token.Split('.');

            // Flip the signature to a valid-base64url but wrong value.
            var forgedSignature = Base64UrlEncode("not-the-real-signature-bytes");
            var tampered = $"{parts[0]}.{forgedSignature}";

            // Act
            var ok = signer.TryVerify(tampered, out InviteTokenPayload payload);

            // Assert
            Assert.False(ok);
            Assert.Null(payload);
        }

        [Fact]
        public void TryVerify_WithDifferentSecret_ShouldFail()
        {
            // Arrange
            var producer = BuildSigner("secret-A");
            var consumer = BuildSigner("secret-B");
            var token = producer.Sign(1, 2, 3, true);

            // Act
            var ok = consumer.TryVerify(token, out InviteTokenPayload payload);

            // Assert
            Assert.False(ok);
            Assert.Null(payload);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("no-dot-separator")]
        [InlineData("too.many.dots")]
        [InlineData("@@@notbase64.@@@notbase64")]
        public void TryVerify_WithMalformedToken_ShouldReturnFalse(string token)
        {
            // Arrange
            var signer = BuildSigner();

            // Act
            var ok = signer.TryVerify(token, out InviteTokenPayload payload);

            // Assert
            Assert.False(ok);
            Assert.Null(payload);
        }

        [Fact]
        public void TryVerify_WithNullToken_ShouldReturnFalse()
        {
            // Arrange
            var signer = BuildSigner();

            // Act
            var ok = signer.TryVerify(null!, out InviteTokenPayload payload);

            // Assert
            Assert.False(ok);
            Assert.Null(payload);
        }

        private static string Base64UrlEncode(string text)
            => Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(text))
                .TrimEnd('=').Replace('+', '-').Replace('/', '_');

        private static string Base64UrlDecode(string value)
        {
            var padded = value.Replace('-', '+').Replace('_', '/');
            switch (padded.Length % 4)
            {
                case 2: padded += "=="; break;
                case 3: padded += "="; break;
            }
            return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(padded));
        }
    }
}
