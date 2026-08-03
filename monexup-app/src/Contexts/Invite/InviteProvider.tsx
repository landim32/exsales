import { useState } from "react";
import ProviderResult from "../../DTO/Contexts/ProviderResult";
import InviteProviderResult from "../../DTO/Contexts/InviteProviderResult";
import InviteDetailProviderResult from "../../DTO/Contexts/InviteDetailProviderResult";
import InviteListProviderResult from "../../DTO/Contexts/InviteListProviderResult";
import IInviteProvider from "../../DTO/Contexts/IInviteProvider";
import { NetworkInviteInfo } from "../../DTO/Domain/InviteInfo";
import { InviteUrlParam } from "../../Business/Interfaces/IInviteBusiness";
import InviteFactory from "../../Business/Factory/InviteFactory";
import InviteContext from "./InviteContext";

export default function InviteProvider(props: any) {

    const [loading, setLoading] = useState<boolean>(false);
    const [loadingDetail, setLoadingDetail] = useState<boolean>(false);
    const [loadingAction, setLoadingAction] = useState<boolean>(false);
    const [loadingInvites, setLoadingInvites] = useState<boolean>(false);
    const [invites, setInvites] = useState<NetworkInviteInfo[]>([]);

    const inviteProviderValue: IInviteProvider = {
        loading: loading,
        loadingDetail: loadingDetail,
        loadingAction: loadingAction,
        loadingInvites: loadingInvites,
        invites: invites,

        invite: async (networkId: number, email: string) => {
            let ret: Promise<InviteProviderResult>;
            setLoading(true);
            try {
                let brt = await InviteFactory.InviteBusiness.invite(networkId, email);
                if (brt.sucesso) {
                    setLoading(false);
                    return {
                        ...ret,
                        result: brt.dataResult,
                        sucesso: true,
                        mensagemSucesso: "Invite created"
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
        listByNetwork: async (networkId: number) => {
            let ret: Promise<InviteListProviderResult>;
            setLoadingInvites(true);
            try {
                let brt = await InviteFactory.InviteBusiness.listByNetwork(networkId);
                setLoadingInvites(false);
                if (brt.sucesso) {
                    const list = brt.dataResult || [];
                    setInvites(list);
                    return {
                        ...ret,
                        invites: list,
                        sucesso: true,
                        mensagemSucesso: "Invites loaded"
                    };
                }
                else {
                    return {
                        ...ret,
                        sucesso: false,
                        mensagemErro: brt.mensagem
                    };
                }
            }
            catch (err) {
                setLoadingInvites(false);
                return {
                    ...ret,
                    sucesso: false,
                    mensagemErro: JSON.stringify(err)
                };
            }
        },
        cancel: async (inviteId: number) => {
            let ret: Promise<ProviderResult>;
            setLoadingAction(true);
            try {
                let brt = await InviteFactory.InviteBusiness.cancel(inviteId);
                setLoadingAction(false);
                if (brt.sucesso) {
                    // Drop it locally so the row disappears before the refetch lands.
                    setInvites((current) => current.filter((x) => x.inviteId !== inviteId));
                    return {
                        ...ret,
                        sucesso: true,
                        mensagemSucesso: "Invite cancelled"
                    };
                }
                else {
                    return {
                        ...ret,
                        sucesso: false,
                        mensagemErro: brt.mensagem
                    };
                }
            }
            catch (err) {
                setLoadingAction(false);
                return {
                    ...ret,
                    sucesso: false,
                    mensagemErro: JSON.stringify(err)
                };
            }
        },
        join: async (token: string) => {
            let ret: Promise<ProviderResult>;
            setLoadingAction(true);
            try {
                let brt = await InviteFactory.InviteBusiness.join(token);
                if (brt.sucesso) {
                    setLoadingAction(false);
                    return {
                        ...ret,
                        sucesso: true,
                        mensagemSucesso: "Invite joined"
                    };
                }
                else {
                    setLoadingAction(false);
                    return {
                        ...ret,
                        sucesso: false,
                        mensagemErro: brt.mensagem
                    };
                }
            }
            catch (err) {
                setLoadingAction(false);
                return {
                    ...ret,
                    sucesso: false,
                    mensagemErro: JSON.stringify(err)
                };
            }
        },
        getDetail: async (token: string) => {
            let ret: Promise<InviteDetailProviderResult>;
            setLoadingDetail(true);
            try {
                let brt = await InviteFactory.InviteBusiness.getDetail(token);
                if (brt.sucesso) {
                    setLoadingDetail(false);
                    return {
                        ...ret,
                        detail: brt.dataResult,
                        sucesso: true,
                        mensagemSucesso: "Invite detail"
                    };
                }
                else {
                    setLoadingDetail(false);
                    return {
                        ...ret,
                        sucesso: false,
                        mensagemErro: brt.mensagem
                    };
                }
            }
            catch (err) {
                setLoadingDetail(false);
                return {
                    ...ret,
                    sucesso: false,
                    mensagemErro: JSON.stringify(err)
                };
            }
        },
        accept: async (token: string) => {
            let ret: Promise<ProviderResult>;
            setLoadingAction(true);
            try {
                let brt = await InviteFactory.InviteBusiness.accept(token);
                if (brt.sucesso) {
                    setLoadingAction(false);
                    return {
                        ...ret,
                        sucesso: true,
                        mensagemSucesso: "Invite accepted"
                    };
                }
                else {
                    setLoadingAction(false);
                    return {
                        ...ret,
                        sucesso: false,
                        mensagemErro: brt.mensagem
                    };
                }
            }
            catch (err) {
                setLoadingAction(false);
                return {
                    ...ret,
                    sucesso: false,
                    mensagemErro: JSON.stringify(err)
                };
            }
        },
        decline: async (token: string) => {
            let ret: Promise<ProviderResult>;
            setLoadingAction(true);
            try {
                let brt = await InviteFactory.InviteBusiness.decline(token);
                if (brt.sucesso) {
                    setLoadingAction(false);
                    return {
                        ...ret,
                        sucesso: true,
                        mensagemSucesso: "Invite declined"
                    };
                }
                else {
                    setLoadingAction(false);
                    return {
                        ...ret,
                        sucesso: false,
                        mensagemErro: brt.mensagem
                    };
                }
            }
            catch (err) {
                setLoadingAction(false);
                return {
                    ...ret,
                    sucesso: false,
                    mensagemErro: JSON.stringify(err)
                };
            }
        },
        buildInviteUrl: (param: InviteUrlParam): string => {
            return InviteFactory.InviteBusiness.buildInviteUrl(param);
        }
    }

    return (
        <InviteContext.Provider value={inviteProviderValue}>
            {props.children}
        </InviteContext.Provider>
    );
}
