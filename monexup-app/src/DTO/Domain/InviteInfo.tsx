// DTOs for the "referrer invite" feature. Mirrors the MonexUp API
// `/Network/invite*` contract 1:1. Account existence is detected
// server-side — the frontend never probes emails directly.

export interface InviteRequestInfo {
    networkId: number;
    email: string;
}

export interface InviteResultInfo {
    sucesso: boolean;
    hasAccount: boolean;
    alreadyMember: boolean;
    token: string;
    networkSlug: string;
    mensagemErro: string | null;
}

export interface InviteDetailInfo {
    sucesso: boolean;
    networkId: number;
    networkName: string;
    inviterName: string;
    targetUserId: number;
    isForCurrentUser: boolean;
    alreadyActiveMember: boolean;
    mensagemErro: string | null;
}

export interface InviteActionInfo {
    token: string;
}

export enum NetworkInviteStatusEnum {
    Pending = 1,
    Accepted = 2,
    Cancelled = 3
}

// A pending invite sent to an e-mail with no account yet. Served only by the
// manager-only `GET /Network/invite/list/{networkId}` — it carries the invitee
// e-mail, so it is never part of the public listByNetwork payload.
export interface NetworkInviteInfo {
    inviteId: number;
    networkId: number;
    email: string;
    inviterUserId: number;
    inviterName: string | null;
    status: NetworkInviteStatusEnum;
    createdAt: string;
    token: string;
    networkSlug: string;
}
