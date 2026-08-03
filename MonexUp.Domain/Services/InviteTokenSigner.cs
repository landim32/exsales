using Microsoft.Extensions.Configuration;
using MonexUp.Domain.Interfaces.Services;
using System;
using System.Security.Cryptography;
using System.Text;

namespace MonexUp.Domain.Impl.Services
{
    /// <summary>
    /// HMAC-SHA256 signer for stateless invite links. Token format:
    /// <c>base64url(payload) + "." + base64url(HMAC-SHA256(secret, payload))</c>
    /// where <c>payload = "networkId|inviterUserId|targetUserId|hasAccount"</c>,
    /// optionally followed by <c>"|inviteId"</c> for no-account invites that have
    /// a row in <c>monexup_network_invites</c>. The 5th segment is only emitted
    /// when <c>inviteId &gt; 0</c>, so existing-account tokens stay byte-identical
    /// to the pre-inviteId format and every already-issued link keeps verifying.
    /// Verification uses a constant-time comparison. Mirrors the HMAC pattern
    /// used by BillingService, lifted into a reusable helper.
    /// </summary>
    public class InviteTokenSigner : IInviteTokenSigner
    {
        private readonly string _secret;

        public InviteTokenSigner(IConfiguration configuration)
        {
            _secret = configuration["Invite:Secret"];
            if (string.IsNullOrWhiteSpace(_secret))
            {
                throw new InvalidOperationException("Invite:Secret is not configured.");
            }
        }

        public string Sign(long networkId, long inviterUserId, long targetUserId, bool hasAccount, long inviteId = 0)
        {
            var payload = BuildPayload(networkId, inviterUserId, targetUserId, hasAccount, inviteId);
            var signature = ComputeHmac(payload);
            return $"{Base64UrlEncode(Encoding.UTF8.GetBytes(payload))}.{Base64UrlEncode(signature)}";
        }

        public bool TryVerify(string token, out InviteTokenPayload payload)
        {
            payload = null;
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            var parts = token.Split('.');
            if (parts.Length != 2)
            {
                return false;
            }

            byte[] payloadBytes;
            byte[] providedSignature;
            try
            {
                payloadBytes = Base64UrlDecode(parts[0]);
                providedSignature = Base64UrlDecode(parts[1]);
            }
            catch (FormatException)
            {
                return false;
            }

            var payloadText = Encoding.UTF8.GetString(payloadBytes);
            var expectedSignature = ComputeHmac(payloadText);

            if (!CryptographicOperations.FixedTimeEquals(providedSignature, expectedSignature))
            {
                return false;
            }

            // 4 segments = legacy token (issued before no-account invites were
            // persisted); 5 = current no-account token carrying its invite id.
            var segments = payloadText.Split('|');
            if ((segments.Length != 4 && segments.Length != 5)
                || !long.TryParse(segments[0], out var networkId)
                || !long.TryParse(segments[1], out var inviterUserId)
                || !long.TryParse(segments[2], out var targetUserId)
                || !int.TryParse(segments[3], out var hasAccountFlag))
            {
                return false;
            }

            long inviteId = 0;
            if (segments.Length == 5 && !long.TryParse(segments[4], out inviteId))
            {
                return false;
            }

            payload = new InviteTokenPayload
            {
                NetworkId = networkId,
                InviterUserId = inviterUserId,
                TargetUserId = targetUserId,
                HasAccount = hasAccountFlag == 1,
                InviteId = inviteId
            };
            return true;
        }

        private static string BuildPayload(long networkId, long inviterUserId, long targetUserId, bool hasAccount, long inviteId)
        {
            var payload = $"{networkId}|{inviterUserId}|{targetUserId}|{(hasAccount ? 1 : 0)}";
            // Omit the 5th segment when there is no invite row, so existing-account
            // tokens stay identical to the ones already in the wild.
            return inviteId > 0 ? $"{payload}|{inviteId}" : payload;
        }

        private byte[] ComputeHmac(string payload)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secret));
            return hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        }

        private static string Base64UrlEncode(byte[] bytes)
            => Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

        private static byte[] Base64UrlDecode(string value)
        {
            var padded = value.Replace('-', '+').Replace('_', '/');
            switch (padded.Length % 4)
            {
                case 2: padded += "=="; break;
                case 3: padded += "="; break;
            }
            return Convert.FromBase64String(padded);
        }
    }
}
