import UserInfo from "../Domain/UserInfo";
import ProviderResult from "./ProviderResult";


interface IUserProvider {
    loading: boolean;
    loadingPassword: boolean;
    loadingUpdate: boolean;
    userHasPassword: boolean;
    user: UserInfo;

    setUser: (user: UserInfo) => void;
    getMe: () => Promise<ProviderResult>;
    getUserByEmail: (email: string) => Promise<ProviderResult>;
    insert: (user: UserInfo) => Promise<ProviderResult>;
    update: (user: UserInfo) => Promise<ProviderResult>;
    loginWithEmail: (email: string, password: string) => Promise<ProviderResult>;

    hasPassword: () => Promise<ProviderResult>;
    changePassword: (oldPassword: string, newPassword: string) => Promise<ProviderResult>;
    sendRecoveryEmail: (email: string) => Promise<ProviderResult>;
    changePasswordUsingHash: (recoveryHash: string, newPassword: string) => Promise<ProviderResult>; 
}

export default IUserProvider;