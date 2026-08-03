import { UserNetworkStatusEnum } from "../Enum/UserNetworkStatusEnum";
import { UserRoleEnum } from "../Enum/UserRoleEnum";
import { UserRowKindEnum } from "../Enum/UserRowKindEnum";

export default interface UserNetworkSearchInfo {
    userId: number;
    networkId: number;
    profileId?: number;
    name: string;
    email: string;
    slug?: string;
    profile: string;
    level: number;
    commission: number;
    role: UserRoleEnum;
    status: UserNetworkStatusEnum;

    // --- /admin/teams row extras (all optional; undefined ⟹ a plain member) ---
    /** undefined is treated as UserRowKindEnum.Member. */
    kind?: UserRowKindEnum;
    /** Membership originated from an invite (drives the "Convidado" badge). */
    invited?: boolean;
    /** Only set on PendingInvite rows. */
    inviteId?: number;
    inviteToken?: string;
    inviteCreatedAt?: string;
    inviterName?: string;
}