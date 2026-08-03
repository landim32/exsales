using System;
using System.Collections.Generic;

namespace DB.Infra.Context;

public partial class UserNetwork
{
    public long UserId { get; set; }

    public long NetworkId { get; set; }

    public long? ProfileId { get; set; }

    public long? ReferrerId { get; set; }

    public int Role { get; set; }

    public int Status { get; set; }

    /// <summary>
    /// Set when the membership was created by an invite; null for self-service
    /// RequestAccess. Drives the "Convidado" badge on /admin/teams.
    /// </summary>
    public DateTime? InvitedAt { get; set; }

    public virtual Network Network { get; set; }

    public virtual UserProfile Profile { get; set; }
}
