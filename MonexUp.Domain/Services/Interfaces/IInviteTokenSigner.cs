namespace MonexUp.Domain.Interfaces.Services
{
    /// <summary>
    /// Decoded payload of a stateless invite token.
    /// </summary>
    public class InviteTokenPayload
    {
        public long NetworkId { get; set; }
        public long InviterUserId { get; set; }
        /// <summary>Invited user's id for existing-account invites; 0 for no-account invites.</summary>
        public long TargetUserId { get; set; }
        /// <summary>True when the invite targets an existing account (accept/decline flow).</summary>
        public bool HasAccount { get; set; }
        /// <summary>
        /// Row id in <c>monexup_network_invites</c> for no-account invites; 0 for
        /// existing-account invites and for legacy 4-segment tokens.
        /// </summary>
        public long InviteId { get; set; }
    }

    /// <summary>
    /// Signs and verifies the stateless, tamper-resistant invite link token.
    /// Encodes <c>networkId|inviterUserId|targetUserId|hasAccount</c> — plus a
    /// 5th <c>inviteId</c> segment for no-account invites — with an HMAC-SHA256
    /// signature (secret from <c>IConfiguration:Invite:Secret</c>). No expiry.
    /// </summary>
    public interface IInviteTokenSigner
    {
        string Sign(long networkId, long inviterUserId, long targetUserId, bool hasAccount, long inviteId = 0);
        bool TryVerify(string token, out InviteTokenPayload payload);
    }
}
