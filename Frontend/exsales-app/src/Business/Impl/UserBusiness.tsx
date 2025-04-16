import BusinessResult from "../../DTO/Business/BusinessResult";
import AuthSession from "../../DTO/Domain/AuthSession";
import UserInfo from "../../DTO/Domain/UserInfo";
import { ChainEnum } from "../../DTO/Enum/ChainEnum";
import IUserService from "../../Services/Interfaces/IUserService";
import AuthFactory from "../Factory/AuthFactory";
import IUserBusiness from "../Interfaces/IUserBusiness";

let _userService: IUserService;

const UserBusiness: IUserBusiness = {
  init: function (userService: IUserService): void {
    _userService = userService;
  },
  getMe: async () => {
    try {
      let ret: BusinessResult<UserInfo>;
      let session: AuthSession = AuthFactory.AuthBusiness.getSession();
      if (!session) {
        return {
          ...ret,
          sucesso: false,
          mensagem: "Not logged"
        };
      }
      let retServ = await _userService.getMe(session.token);
      if (retServ.sucesso) {
        return {
          ...ret,
          dataResult: retServ.user,
          sucesso: true
        };
      } else {
        return {
          ...ret,
          sucesso: false,
          mensagem: retServ.mensagem
        };
      }
    } catch {
      throw new Error("Failed to get user by address");
    }
  },
  getUserByAddress: async (chain: ChainEnum, address: string) => {
    try {
      let ret: BusinessResult<UserInfo>;
      let retServ = await _userService.getUserByAddress(chain, address);
      if (retServ.sucesso) {
        return {
          ...ret,
          dataResult: retServ.user,
          sucesso: true
        };
      } else {
        return {
          ...ret,
          sucesso: false,
          mensagem: retServ.mensagem
        };
      }
    } catch {
      throw new Error("Failed to get user by address");
    }
  },
  getUserByEmail: async (email: string) => {
    try {
      let ret: BusinessResult<UserInfo>;
      let retServ = await _userService.getUserByEmail(email);
      if (retServ.sucesso) {
        return {
          ...ret,
          dataResult: retServ.user,
          sucesso: true
        };
      } else {
        return {
          ...ret,
          sucesso: false,
          mensagem: retServ.mensagem
        };
      }
    } catch {
      throw new Error("Failed to get user by email");
    }
  },
  getTokenUnauthorized: async (chainId: ChainEnum, address: string) => {
    let ret: BusinessResult<string>;
    let retServ = await _userService.getTokenUnauthorized(chainId, address);
    if (retServ.sucesso) {
      return {
        ...ret,
        dataResult: retServ.token,
        sucesso: true
      };
    } else {
      return {
        ...ret,
        sucesso: false,
        mensagem: retServ.mensagem
      };
    }
  },
  getTokenAuthorized: async (email: string, password: string) => {
    let ret: BusinessResult<string>;
    let retServ = await _userService.getTokenAuthorized(email, password);
    if (retServ.sucesso) {
      return {
        ...ret,
        dataResult: retServ.token,
        sucesso: true
      };
    } else {
      return {
        ...ret,
        sucesso: false,
        mensagem: retServ.mensagem
      };
    }
  },
  insert: async (user: UserInfo) => {
    try {
      let ret: BusinessResult<UserInfo>;
      let retServ = await _userService.insert(user);
      if (retServ.sucesso) {
        return {
          ...ret,
          dataResult: retServ.user,
          sucesso: true
        };
      } else {
        return {
          ...ret,
          sucesso: false,
          mensagem: retServ.mensagem
        };
      }
    } catch {
      throw new Error("Failed to insert");
    }
  },
  update: async (user: UserInfo) => {
    try {
      let ret: BusinessResult<UserInfo>;
      let session: AuthSession = AuthFactory.AuthBusiness.getSession();
      if (!session) {
        return {
          ...ret,
          sucesso: false,
          mensagem: "Not logged"
        };
      }
      let retServ = await _userService.update(user, session.token);
      if (retServ.sucesso) {
        return {
          ...ret,
          dataResult: retServ.user,
          sucesso: true
        };
      } else {
        return {
          ...ret,
          sucesso: false,
          mensagem: retServ.mensagem
        };
      }
    } catch {
      throw new Error("Failed to update");
    }
  },
  loginWithEmail: async (email: string, password: string) => {
    try {
      let ret: BusinessResult<UserInfo>;
      let retServ = await _userService.loginWithEmail(email, password);
      if (retServ.sucesso) {
        return {
          ...ret,
          dataResult: retServ.user,
          sucesso: true
        };
      } else {
        return {
          ...ret,
          sucesso: false,
          mensagem: retServ.mensagem
        };
      }
    } catch {
      throw new Error("Failed to login with email");
    }
  },
  hasPassword: async () => {
    try {
      let ret: BusinessResult<boolean>;
      let session: AuthSession = AuthFactory.AuthBusiness.getSession();
      if (!session) {
        return {
          ...ret,
          sucesso: false,
          mensagem: "Not logged"
        };
      }
      let retServ = await _userService.hasPassword(session.token);
      if (retServ.sucesso) {
        return {
          ...ret,
          dataResult: true,
          sucesso: true
        };
      } else {
        return {
          ...ret,
          sucesso: false,
          mensagem: retServ.mensagem
        };
      }
    } catch {
      throw new Error("Failed to change password");
    }
  },
  changePassword: async (oldPassword: string, newPassword: string) => {
    let ret: BusinessResult<boolean>;
    let session: AuthSession = AuthFactory.AuthBusiness.getSession();
    if (!session) {
      return {
        ...ret,
        sucesso: false,
        mensagem: "Not logged"
      };
    }
    let retServ = await _userService.changePassword(oldPassword, newPassword, session.token);
    if (retServ.sucesso) {
      return {
        ...ret,
        dataResult: true,
        sucesso: true,
        mensagem: retServ.mensagem
      };
    } else {
      return {
        ...ret,
        sucesso: false,
        mensagem: retServ.mensagem
      };
    }
  },
  sendRecoveryEmail: async (email: string) => {
    try {
      let ret: BusinessResult<boolean>;
      let retServ = await _userService.sendRecoveryEmail(email);
      if (retServ.sucesso) {
        return {
          ...ret,
          dataResult: ret.sucesso,
          sucesso: true
        };
      } else {
        return {
          ...ret,
          sucesso: false,
          mensagem: retServ.mensagem
        };
      }
    } catch {
      throw new Error("Failed to send recovery email");
    }
  },
  changePasswordUsingHash: async (recoveryHash: string, newPassword: string) => {
    try {
      let ret: BusinessResult<boolean>;
      let retServ = await _userService.changePasswordUsingHash(recoveryHash, newPassword);
      if (retServ.sucesso) {
        return {
          ...ret,
          dataResult: ret.sucesso,
          sucesso: true
        };
      } else {
        return {
          ...ret,
          sucesso: false,
          mensagem: retServ.mensagem
        };
      }
    } catch {
      throw new Error("Failed to change password using hash");
    }
  }
}

export default UserBusiness;