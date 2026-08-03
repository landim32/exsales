using System.Text.Json.Serialization;

namespace MonexUp.DTO.Network
{
    public class InviteCancelInfo
    {
        [JsonPropertyName("inviteId")]
        public long InviteId { get; set; }
    }
}
