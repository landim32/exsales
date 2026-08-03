import { NetworkInviteInfo } from "../Domain/InviteInfo";
import ProviderResult from "./ProviderResult";

export default interface InviteListProviderResult extends ProviderResult {
    invites?: NetworkInviteInfo[];
};
