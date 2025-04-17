import env from 'react-dotenv';
import { HttpClient } from '../Infra/Impl/HttpClient';
import IHttpClient from '../Infra/Interface/IHttpClient';
import IUserService from './Interfaces/IUserService';
import UserService from './Impl/UserService';

const httpClientAuth : IHttpClient = HttpClient();
httpClientAuth.init(env.API_BASE_URL);

const userServiceImpl : IUserService = UserService;
userServiceImpl.init(httpClientAuth);

const ServiceFactory = {
  UserService: userServiceImpl,
  setLogoffCallback: (cb : () => void) => {
    httpClientAuth.setLogoff(cb);
  }
};

export default ServiceFactory;