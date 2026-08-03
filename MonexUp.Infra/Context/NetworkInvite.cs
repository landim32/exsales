using System;

namespace DB.Infra.Context;

public partial class NetworkInvite
{
    public long InviteId { get; set; }

    public long NetworkId { get; set; }

    public string Email { get; set; }

    public long InviterUserId { get; set; }

    public int Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ConsumedAt { get; set; }

    public long? ConsumedUserId { get; set; }

    public virtual Network Network { get; set; }
}
