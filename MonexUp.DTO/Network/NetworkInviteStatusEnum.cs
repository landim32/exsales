namespace MonexUp.DTO.Network
{
    /// <summary>
    /// Lifecycle of a no-account invite stored in <c>monexup_network_invites</c>.
    /// Invites to e-mails that already have an account are NOT stored here —
    /// those become a WaitForApproval row in monexup_user_networks right away.
    /// </summary>
    public enum NetworkInviteStatusEnum
    {
        Pending = 1,
        Accepted = 2,
        Cancelled = 3
    }
}
