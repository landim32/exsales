import { useState } from "react";
import IUserProvider from "../../DTO/Contexts/IUserProvider";
import UserContext from "./UserContext";
import UserInfo from "../../DTO/Domain/UserInfo";
import ProviderResult from "../../DTO/Contexts/ProviderResult";
import UserFactory from "../../Business/Factory/UserFactory";

export default function UserProvider(props: any) {

    const [loading, setLoading] = useState<boolean>(false);
    const [loadingPassword, setLoadingPassword] = useState<boolean>(false);
    const [loadingUpdate, setLoadingUpdate] = useState<boolean>(false);

    const [userHasPassword, setUserHasPassword] = useState<boolean>(false);

    const [user, _setUser] = useState<UserInfo>(null);

    const userProviderValue: IUserProvider = {
        loading: loading,
        loadingPassword: loadingPassword,
        loadingUpdate: loadingUpdate,
        userHasPassword: userHasPassword,
        user: user,
        setUser: (user: UserInfo) => {
            _setUser(user);
        },
        getMe: async () => {
            let ret: Promise<ProviderResult>;
            setLoading(true);
            try {
                let brt = await UserFactory.UserBusiness.getMe();
                if (brt.sucesso) {
                    setLoading(false);
                    _setUser(brt.dataResult);
                    return {
                        ...ret,
                        sucesso: true,
                        mensagemSucesso: "User load"
                    };
                }
                else {
                    setLoading(false);
                    return {
                        ...ret,
                        sucesso: false,
                        mensagemErro: brt.mensagem
                    };
                }
            }
            catch (err) {
                setLoading(false);
                return {
                    ...ret,
                    sucesso: false,
                    mensagemErro: JSON.stringify(err)
                };
            }
        },
        getUserByEmail: async (email: string) => {
            let ret: Promise<ProviderResult>;
            setLoading(true);
            try {
                let brt = await UserFactory.UserBusiness.getUserByEmail(email);
                if (brt.sucesso) {
                    setLoading(false);
                    _setUser(brt.dataResult);
                    return {
                        ...ret,
                        sucesso: true,
                        mensagemSucesso: "User load"
                    };
                }
                else {
                    setLoading(false);
                    return {
                        ...ret,
                        sucesso: false,
                        mensagemErro: brt.mensagem
                    };
                }
            }
            catch (err) {
                setLoading(false);
                return {
                    ...ret,
                    sucesso: false,
                    mensagemErro: JSON.stringify(err)
                };
            }
        },
        insert: async (user: UserInfo) => {
            let ret: Promise<ProviderResult>;
            setLoadingUpdate(true);
            try {
                let brt = await UserFactory.UserBusiness.insert(user);
                if (brt.sucesso) {
                    setLoadingUpdate(false);
                    _setUser(brt.dataResult);
                    return {
                        ...ret,
                        sucesso: true,
                        mensagemSucesso: "User inseted"
                    };
                }
                else {
                    setLoadingUpdate(false);
                    return {
                        ...ret,
                        sucesso: false,
                        mensagemErro: brt.mensagem
                    };
                }
            }
            catch (err) {
                setLoadingUpdate(false);
                return {
                    ...ret,
                    sucesso: false,
                    mensagemErro: JSON.stringify(err)
                };
            }
        },
        update: async (user: UserInfo) => {
            let ret: Promise<ProviderResult>;
            setLoadingUpdate(true);
            try {
                let brt = await UserFactory.UserBusiness.update(user);
                if (brt.sucesso) {
                    setLoadingUpdate(false);
                    _setUser(brt.dataResult);
                    return {
                        ...ret,
                        sucesso: true,
                        mensagemSucesso: "User updated"
                    };
                }
                else {
                    setLoadingUpdate(false);
                    return {
                        ...ret,
                        sucesso: false,
                        mensagemErro: brt.mensagem
                    };
                }
            }
            catch (err) {
                setLoadingUpdate(false);
                return {
                    ...ret,
                    sucesso: false,
                    mensagemErro: JSON.stringify(err)
                };
            }
        },
        loginWithEmail: async (email: string, password: string) => {
            let ret: Promise<ProviderResult>;
            setLoading(true);
            try {
                let brt = await UserFactory.UserBusiness.update(user);
                if (brt.sucesso) {
                    setLoading(false);
                    _setUser(brt.dataResult);
                    return {
                        ...ret,
                        sucesso: true,
                        mensagemSucesso: "User updated"
                    };
                }
                else {
                    setLoading(false);
                    return {
                        ...ret,
                        sucesso: false,
                        mensagemErro: brt.mensagem
                    };
                }
            }
            catch (err) {
                setLoading(false);
                return {
                    ...ret,
                    sucesso: false,
                    mensagemErro: JSON.stringify(err)
                };
            }
        },
        hasPassword: async () => {
            let ret: Promise<ProviderResult>;
            setLoadingPassword(true);
            setUserHasPassword(false);
            try {
                let brt = await UserFactory.UserBusiness.hasPassword();
                if (brt.sucesso) {
                    setUserHasPassword(true);
                    setLoadingPassword(false);
                    return {
                        ...ret,
                        sucesso: true,
                        mensagemSucesso: "Password changed"
                    };
                }
                else {
                    setLoadingPassword(false);
                    return {
                        ...ret,
                        sucesso: false,
                        mensagemErro: brt.mensagem
                    };
                }
            }
            catch (err) {
                setLoadingPassword(false);
                return {
                    ...ret,
                    sucesso: false,
                    mensagemErro: JSON.stringify(err)
                };
            }
        },
        changePassword: async (oldPassword: string, newPassword: string) => {
            let ret: Promise<ProviderResult>;
            setLoadingUpdate(true);
            try {
                let brt = await UserFactory.UserBusiness.changePassword(oldPassword, newPassword);
                console.log("changePassword: ", JSON.stringify(brt));
                if (brt.sucesso) {
                    setLoadingUpdate(false);
                    return {
                        ...ret,
                        sucesso: true,
                        mensagemSucesso: brt.mensagem
                    };
                }
                else {
                    setLoadingUpdate(false);
                    return {
                        ...ret,
                        sucesso: false,
                        mensagemErro: brt.mensagem
                    };
                }
            }
            catch (err) {
                setLoadingUpdate(false);
                console.log("Error change password: ", err);
                return {
                    ...ret,
                    sucesso: false,
                    mensagemErro: JSON.stringify(err)
                };
            }
        },
        sendRecoveryEmail: async (email: string) => {
            let ret: Promise<ProviderResult>;
            setLoadingUpdate(true);
            try {
                let brt = await UserFactory.UserBusiness.sendRecoveryEmail(email);
                if (brt.sucesso) {
                    setLoadingUpdate(false);
                    return {
                        ...ret,
                        sucesso: true,
                        mensagemSucesso: "Recovery email sent successfully"
                    };
                }
                else {
                    setLoadingUpdate(false);
                    return {
                        ...ret,
                        sucesso: false,
                        mensagemErro: brt.mensagem
                    };
                }
            }
            catch (err) {
                setLoadingUpdate(false);
                return {
                    ...ret,
                    sucesso: false,
                    mensagemErro: JSON.stringify(err)
                };
            }
        },
        changePasswordUsingHash: async (recoveryHash: string, newPassword: string) => {
            let ret: Promise<ProviderResult>;
            setLoadingUpdate(true);
            try {
                let brt = await UserFactory.UserBusiness.changePasswordUsingHash(recoveryHash, newPassword);
                if (brt.sucesso) {
                    setLoadingUpdate(false);
                    return {
                        ...ret,
                        sucesso: true,
                        mensagemSucesso: "Recovery email sent successfully"
                    };
                }
                else {
                    setLoadingUpdate(false);
                    return {
                        ...ret,
                        sucesso: false,
                        mensagemErro: brt.mensagem
                    };
                }
            }
            catch (err) {
                setLoadingUpdate(false);
                return {
                    ...ret,
                    sucesso: false,
                    mensagemErro: JSON.stringify(err)
                };
            }
        }
    }

    return (
        <UserContext.Provider value={userProviderValue}>
            {props.children}
        </UserContext.Provider>
    );
}