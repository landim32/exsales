using System;
using System.Text.Json.Serialization;

namespace MonexUp.DTO.Network
{
    /// <summary>
    /// A pending no-account invite, as rendered on /admin/teams. Served only by
    /// the manager-only endpoint <c>GET /Network/invite/list/{networkId}</c> —
    /// never by the public <c>listByNetwork</c>, since it carries the invitee e-mail.
    /// </summary>
    public class NetworkInviteInfo
    {
        [JsonPropertyName("inviteId")]
        public long InviteId { get; set; }
        [JsonPropertyName("networkId")]
        public long NetworkId { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("inviterUserId")]
        public long InviterUserId { get; set; }
        [JsonPropertyName("inviterName")]
        public string InviterName { get; set; }
        [JsonPropertyName("status")]
        public NetworkInviteStatusEnum Status { get; set; }
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
        /// <summary>Re-signed on every read — the token is never persisted.</summary>
        [JsonPropertyName("token")]
        public string Token { get; set; }
        [JsonPropertyName("networkSlug")]
        public string NetworkSlug { get; set; }
    }
}
