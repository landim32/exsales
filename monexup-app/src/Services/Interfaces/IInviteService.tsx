import { InviteDetailInfo, InviteResultInfo, NetworkInviteInfo } from "../../DTO/Domain/InviteInfo";
import ApiResponse from "../../DTO/Services/ApiResponse";
import IHttpClient from "../../Infra/Interface/IHttpClient";

export default interface IInviteService {
    init: (httpClient: IHttpClient) => void;
    invite: (networkId: number, email: string, token: string) => Promise<ApiResponse<InviteResultInfo>>;
    listByNetwork: (networkId: number, authToken: string) => Promise<ApiResponse<NetworkInviteInfo[]>>;
    cancel: (inviteId: number, authToken: string) => Promise<ApiResponse<void>>;
    join: (token: string, authToken: string) => Promise<ApiResponse<void>>;
    getDetail: (token: string, authToken: string) => Promise<ApiResponse<InviteDetailInfo>>;
    accept: (token: string, authToken: string) => Promise<ApiResponse<void>>;
    decline: (token: string, authToken: string) => Promise<ApiResponse<void>>;
}
