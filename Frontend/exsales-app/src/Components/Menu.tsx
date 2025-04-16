import React, { useContext, useEffect, useState } from 'react';
import Container from 'react-bootstrap/Container';
import Button from 'react-bootstrap/esm/Button';
import Nav from 'react-bootstrap/Nav';
import Navbar from 'react-bootstrap/Navbar';
import { Link, useNavigate } from 'react-router-dom';
import AuthContext from '../Contexts/Auth/AuthContext';
import NavDropdown from 'react-bootstrap/NavDropdown';
import Alert from 'react-bootstrap/Alert';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faWarning } from '@fortawesome/free-solid-svg-icons/faWarning'
import { faBitcoinSign, faBoltLightning, faBox, faBrazilianRealSign, faBuilding, faCancel, faCheck, faCheckCircle, faCircle, faCircleUser, faClose, faCog, faCoins, faDollar, faEthernet, faFileWord, faHome, faLock, faPencil, faSearch, faSignInAlt, faUser, faUserAlt, faUserCircle, faUserCog, faUserFriends, faUserGear, faUserGraduate, faUserGroup, faUserMd } from '@fortawesome/free-solid-svg-icons';
import MessageToast from './MessageToast';
import { ChainEnum } from '../DTO/Enum/ChainEnum';
import { MessageToastEnum } from '../DTO/Enum/MessageToastEnum';
import env from 'react-dotenv';
import { RoleEnum } from '../DTO/Enum/RoleEnum';


export default function Menu() {

  const [showAlert, setShowAlert] = useState<boolean>(true);

  const [showMessage, setShowMessage] = useState<boolean>(false);
  const [messageText, setMessageText] = useState<string>("");

  const throwError = (message: string) => {
    setMessageText(message);
    setShowMessage(true);
  };

  const showRoleText = (role: RoleEnum) => {
    switch (role) {
      case RoleEnum.NoRole:
        return (
          <>
            <FontAwesomeIcon icon={faCancel} fixedWidth />&nbsp;No role
          </>
        );
        break;
      case RoleEnum.User:
        return (
          <>
            <FontAwesomeIcon icon={faUser} fixedWidth />&nbsp;User
          </>
        );
        break;
      case RoleEnum.Seller:
        return (
          <>
            <FontAwesomeIcon icon={faUserMd} fixedWidth />&nbsp;Seller
          </>
        );
        break;
      case RoleEnum.NetworkManager:
        return (
          <>
            <FontAwesomeIcon icon={faUserGroup} fixedWidth />&nbsp;Network Manager
          </>
        );
        break;
      case RoleEnum.Administrator:
        return (
          <>
            <FontAwesomeIcon icon={faUserGear} fixedWidth />&nbsp;Adminstrator
          </>
        );
        break;
    }
  };

  let navigate = useNavigate();

  const authContext = useContext(AuthContext);

  useEffect(() => {
    authContext.loadUserSession();
  }, []);
  return (
    <>
      <MessageToast
        dialog={MessageToastEnum.Error}
        showMessage={showMessage}
        messageText={messageText}
        onClose={() => setShowMessage(false)}
      ></MessageToast>
      <Navbar expand="lg" className="mb-3 border-bottom">
        <Container>
          <Link className='navbar-brand' to="/">{env.PROJECT_NAME}</Link>
          <Navbar.Toggle aria-controls="basic-navbar-nav" />
          <Navbar.Collapse id="basic-navbar-nav">
            <Nav className="me-auto">
              <Link className='nav-link' to="/"><FontAwesomeIcon icon={faHome} fixedWidth /> Home</Link>
              <Link className='nav-link' to="/new-seller"><FontAwesomeIcon icon={faUser} fixedWidth /> Seja um representante</Link>
              <Link className='nav-link' to="/network"><FontAwesomeIcon icon={faBuilding} fixedWidth /> Crie sua rede</Link>
              {authContext.sessionInfo &&
                <>
                  <Link className='nav-link' to="/my-swaps">My Swaps</Link>
                  {authContext.sessionInfo?.isAdmin &&
                    <Link className='nav-link' to="/all-swaps">All Swaps</Link>
                  }
                </>
              }
              <NavDropdown title={
                <>
                  <FontAwesomeIcon icon={faUserGroup} />&nbsp;My Network
                </>
              } id="basic-nav-dropdown">
                <NavDropdown.ItemText className='small text-center'>Network</NavDropdown.ItemText>
                <NavDropdown.Item onClick={() => {
                  navigate("/minha-rede/network");
                }}><FontAwesomeIcon icon={faCog} fixedWidth />&nbsp;Preferences</NavDropdown.Item>
                <NavDropdown.Item onClick={() => {
                  navigate("/minha-rede/team-structure");
                }}><FontAwesomeIcon icon={faUserCog} fixedWidth />&nbsp;Team Structure</NavDropdown.Item>
                <NavDropdown.Item onClick={() => {
                  navigate("/minha-rede/teams");
                }}><FontAwesomeIcon icon={faUserGroup} fixedWidth />&nbsp;Teams</NavDropdown.Item>
                <NavDropdown.Divider />
                <NavDropdown.ItemText className='small text-center'>Finances</NavDropdown.ItemText>
                <NavDropdown.Item onClick={() => {
                  navigate("/minha-rede/orders");
                }}><FontAwesomeIcon icon={faFileWord} fixedWidth />&nbsp;Orders</NavDropdown.Item>
                <NavDropdown.Item onClick={() => {
                  navigate("/minha-rede/invoices");
                }}><FontAwesomeIcon icon={faDollar} fixedWidth />&nbsp;Invoices</NavDropdown.Item>
                <NavDropdown.Item onClick={() => {
                  navigate("/minha-rede/products");
                }}><FontAwesomeIcon icon={faBox} fixedWidth />&nbsp;Products</NavDropdown.Item>
              </NavDropdown>
            </Nav>
          </Navbar.Collapse>
          <Navbar.Collapse>
            <Nav className="ms-auto justify-content-end">
              <NavDropdown title={
                <>
                  <FontAwesomeIcon icon={faUserGroup} />&nbsp;Minha Rede Principal
                </>
              } id="basic-nav-dropdown">
                <NavDropdown.ItemText className='small'>Select network to connect</NavDropdown.ItemText>
                <NavDropdown.Divider />
                <NavDropdown.Item onClick={() => {
                  navigate("/minha-rede/dashboard");
                }}><FontAwesomeIcon icon={faUserGroup} />&nbsp;Minha Rede Principal</NavDropdown.Item>
                <NavDropdown.Item onClick={() => {
                  navigate("/minha-rede/dashboard");
                }}><FontAwesomeIcon icon={faUserGroup} />&nbsp;Rede Secundária</NavDropdown.Item>
                <NavDropdown.Item onClick={() => {
                  navigate("/minha-rede/dashboard");
                }}><FontAwesomeIcon icon={faUserGroup} />&nbsp;Última Rede</NavDropdown.Item>
                <NavDropdown.Divider />
                <NavDropdown.Item onClick={() => {
                  navigate("/network/search");
                }}><FontAwesomeIcon icon={faSearch} />&nbsp;Buscar uma rede</NavDropdown.Item>
              </NavDropdown>
              <NavDropdown title={showRoleText(RoleEnum.User)} id="basic-nav-dropdown">
                <NavDropdown.ItemText className='small'>Select the chain you will connect to</NavDropdown.ItemText>
                <NavDropdown.Divider />
                <NavDropdown.Item>{showRoleText(RoleEnum.User)}</NavDropdown.Item>
                <NavDropdown.Item>{showRoleText(RoleEnum.Seller)}</NavDropdown.Item>
                <NavDropdown.Item>{showRoleText(RoleEnum.NetworkManager)}</NavDropdown.Item>
                <NavDropdown.Item>{showRoleText(RoleEnum.Administrator)}</NavDropdown.Item>
              </NavDropdown>
              <NavDropdown title={
                <>
                  <FontAwesomeIcon icon={faCheckCircle} />&nbsp;Edição Ativada
                </>
              } id="basic-nav-dropdown">
                <NavDropdown.ItemText className='small'>Ative o modo de edição para alterar as páginas da rede</NavDropdown.ItemText>
                <NavDropdown.Divider />
                <NavDropdown.Item><FontAwesomeIcon icon={faCheckCircle} />&nbsp;Ativar Edição</NavDropdown.Item>
                <NavDropdown.Item><FontAwesomeIcon icon={faCircle} />&nbsp;Desativar Edição</NavDropdown.Item>
              </NavDropdown>
              {
                authContext.sessionInfo ?
                  <NavDropdown title={
                    <>
                      <FontAwesomeIcon icon={faCircleUser} />&nbsp;
                      <span>{authContext.sessionInfo.name}</span>
                    </>
                  } id="basic-nav-dropdown">
                    <NavDropdown.Item onClick={async () => {
                      navigate("/edit-account");
                    }}><FontAwesomeIcon icon={faPencil} fixedWidth /> Edit Account</NavDropdown.Item>
                    <NavDropdown.Item onClick={async () => {
                      navigate("/change-password");
                    }}><FontAwesomeIcon icon={faLock} fixedWidth /> Change Password</NavDropdown.Item>
                    <NavDropdown.Divider />
                    <NavDropdown.Item onClick={async () => {
                      let ret = authContext.logout();
                      if (!ret.sucesso) {
                        throwError(ret.mensagemErro);
                      }
                      navigate(0);
                    }}><FontAwesomeIcon icon={faClose} fixedWidth /> Logout</NavDropdown.Item>
                  </NavDropdown>
                  :
                  <>
                    <Nav.Item>
                      <Button variant="danger" onClick={async () => {
                        navigate("/login");
                      }}>
                        <FontAwesomeIcon icon={faSignInAlt} fixedWidth /> Sign In
                      </Button>
                    </Nav.Item>
                  </>
              }
            </Nav>
          </Navbar.Collapse>
        </Container>
      </Navbar>
      {showAlert &&
        <Container>
          <Alert key="danger" variant="danger" onClose={() => setShowAlert(false)} dismissible>
            <FontAwesomeIcon icon={faWarning} /> This is a <strong>trial version</strong>, do not make payments with your real data.
          </Alert>
        </Container>
      }
    </>
  );
}
