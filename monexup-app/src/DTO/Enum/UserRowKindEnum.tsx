// Discriminates the two kinds of row rendered by /admin/teams: an actual
// network membership, or a pending invite to an e-mail with no account yet
// (which has no userId, no profile and no role).
export enum UserRowKindEnum {
    Member = 0,
    PendingInvite = 1
}
