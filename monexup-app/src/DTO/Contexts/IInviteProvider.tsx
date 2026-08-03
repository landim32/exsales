import { InviteUrlParam } from "../../Business/Interfaces/IInviteBusiness";
import { NetworkInviteInfo } from "../Domain/InviteInfo";
import InviteDetailProviderResult from "./InviteDetailProviderResult";
import InviteListProviderResult from "./InviteListProviderResult";
import InviteProviderResult from "./InviteProviderResult";
import ProviderResult from "./ProviderResult";

interface IInviteProvider {
    loading: boolean;
    loadingDetail: boolean;
    loadingAction: boolean;
    loadingInvites: boolean;
    // Pending no-account invites of the last network passed to listByNetwork.
    invites: NetworkInviteInfo[];

    invite: (networkId: number, email: string) => Promise<InviteProviderResult>;
    listByNetwork: (networkId: number) => Promise<InviteListProviderResult>;
    cancel: (inviteId: number) => Promise<ProviderResult>;
    join: (token: string) => Promise<ProviderResult>;
    getDetail: (token: string) => Promise<InviteDetailProviderResult>;
    accept: (token: string) => Promise<ProviderResult>;
    decline: (token: string) => Promise<ProviderResult>;
    buildInviteUrl: (param: InviteUrlParam) => string;
}

export default IInviteProvider;
