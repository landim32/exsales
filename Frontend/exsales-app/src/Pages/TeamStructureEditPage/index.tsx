import { useContext, useEffect, useState } from "react";
import Col from "react-bootstrap/esm/Col";
import Container from "react-bootstrap/esm/Container";
import Row from "react-bootstrap/esm/Row";
import Form from 'react-bootstrap/Form';
import AuthContext from "../../Contexts/Auth/AuthContext";
import Button from "react-bootstrap/esm/Button";
import Card from 'react-bootstrap/Card';
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faAddressBook, faArrowRight, faBitcoinSign, faCalendar, faCalendarAlt, faCancel, faClose, faDollar, faEnvelope, faEthernet, faIdCard, faLock, faPercent, faPhone, faSave, faSignInAlt, faTrash, faUser } from '@fortawesome/free-solid-svg-icons';
import Table from "react-bootstrap/esm/Table";
import { Link, useNavigate } from "react-router-dom";
import InputGroup from 'react-bootstrap/InputGroup';
import UserContext from "../../Contexts/User/UserContext";
import MessageToast from "../../Components/MessageToast";
import Moment from 'moment';
import { MessageToastEnum } from "../../DTO/Enum/MessageToastEnum";

export default function TeamStructureEditPage() {

    const authContext = useContext(AuthContext);
    const userContext = useContext(UserContext);

    const [insertMode, setInsertMode] = useState<boolean>(false);

    const [dialog, setDialog] = useState<MessageToastEnum>(MessageToastEnum.Error);
    const [showMessage, setShowMessage] = useState<boolean>(false);
    const [messageText, setMessageText] = useState<string>("");

    let navigate = useNavigate();
    Moment.locale('en');

    const throwError = (message: string) => {
        setDialog(MessageToastEnum.Error)
        setMessageText(message);
        setShowMessage(true);
    };
    const showSuccessMessage = (message: string) => {
        setDialog(MessageToastEnum.Success)
        setMessageText(message);
        setShowMessage(true);
    };

    useEffect(() => {
        if (authContext.sessionInfo) {
            if (authContext.sessionInfo?.userId > 0) {
                userContext.getMe().then((ret) => {
                    if (ret.sucesso) {
                        setInsertMode(false);
                    }
                    else {
                        setInsertMode(true);
                    }
                });
            }
            else {
                setInsertMode(true);
            }
        }
        else {
            setInsertMode(true);
        }
    }, []);

    return (
        <>
            <MessageToast
                dialog={dialog}
                showMessage={showMessage}
                messageText={messageText}
                onClose={() => setShowMessage(false)}
            ></MessageToast>
            <Container>
                <Row>
                    <Col md="12">
                        <Card>
                            <Card.Header>
                                <h3 className="text-center">Team Structure (Profile)</h3>
                            </Card.Header>
                            <Card.Body>
                                <Form>
                                    <div className="text-center mb-3">
                                        Registration is not required to make swaps, but you can do so anyway to access your transaction history.
                                    </div>
                                    <Form.Group as={Row} className="mb-3">
                                        <Form.Label column sm="2">Name:</Form.Label>
                                        <Col sm="5">
                                            <InputGroup>
                                                <InputGroup.Text><FontAwesomeIcon icon={faUser} fixedWidth /></InputGroup.Text>
                                                <Form.Control type="text" size="lg"
                                                    placeholder="Your profile name"
                                                    value={userContext.user?.name}
                                                    onChange={(e) => {
                                                        userContext.setUser({
                                                            ...userContext.user,
                                                            name: e.target.value
                                                        });
                                                    }} />
                                            </InputGroup>
                                        </Col>
                                        <Form.Label column sm="1">Level:</Form.Label>
                                        <Col sm="4">
                                            <InputGroup>
                                                <InputGroup.Text><FontAwesomeIcon icon={faDollar} fixedWidth /></InputGroup.Text>
                                                <Form.Control type="text" size="lg"
                                                    placeholder="Network level number"
                                                    value={userContext.user?.email}
                                                />
                                            </InputGroup>
                                        </Col>
                                    </Form.Group>
                                    <Form.Group as={Row} className="mb-3">
                                        <Form.Label column sm="2">Commission (%):</Form.Label>
                                        <Col sm="5">
                                            <InputGroup>
                                                <InputGroup.Text><FontAwesomeIcon icon={faCalendar} fixedWidth /></InputGroup.Text>
                                                <Form.Control type="text" size="lg"
                                                    placeholder="Commission in percents"
                                                    value={userContext.user?.email}
                                                />
                                            </InputGroup>
                                        </Col>
                                        <Form.Label column sm="1">Status:</Form.Label>
                                        <Col sm="4">
                                            <InputGroup>
                                                <InputGroup.Text><FontAwesomeIcon icon={faPercent} fixedWidth /></InputGroup.Text>
                                                <Form.Select size="lg">
                                                    <option>Active</option>
                                                    <option value="1">Inactive</option>
                                                    <option value="2">Suspense</option>
                                                </Form.Select>
                                            </InputGroup>
                                        </Col>
                                    </Form.Group>
                                    {!insertMode &&
                                        <Form.Group as={Row} className="mb-3">
                                            <Form.Label column sm="2">Create At:</Form.Label>
                                            <Col sm="4">
                                                <InputGroup>
                                                    <InputGroup.Text><FontAwesomeIcon icon={faCalendarAlt} fixedWidth /></InputGroup.Text>
                                                    <Form.Control type="text" size="lg" disabled={true} readOnly={true}
                                                        value={Moment(userContext.user?.createAt).format("MMM DD YYYY")} />
                                                </InputGroup>
                                            </Col>
                                            <Form.Label column sm="2">Update At:</Form.Label>
                                            <Col sm="4">
                                                <InputGroup>
                                                    <InputGroup.Text><FontAwesomeIcon icon={faCalendarAlt} fixedWidth /></InputGroup.Text>
                                                    <Form.Control type="text" size="lg" disabled={true} readOnly={true}
                                                        value={Moment(userContext.user?.updateAt).format("MMM DD YYYY")} />
                                                </InputGroup>
                                            </Col>
                                        </Form.Group>
                                    }
                                    <div className="d-grid gap-2 d-md-flex justify-content-md-end">
                                        <Button variant="danger" size="lg" onClick={() => {
                                            navigate("/minha-rede/team-structure");
                                        }}><FontAwesomeIcon icon={faCancel} fixedWidth /> Cancel</Button>
                                        <Button variant="success" size="lg" onClick={async (e) => {
                                            if (insertMode) {
                                                let ret = await userContext.insert(userContext.user);
                                                if (ret.sucesso) {
                                                    showSuccessMessage(ret.mensagemSucesso);
                                                    //alert(userContext.user?.id);
                                                }
                                                else {
                                                    throwError(ret.mensagemErro);
                                                }
                                            }
                                            else {
                                                let ret = await userContext.update(userContext.user);
                                                if (ret.sucesso) {
                                                    //alert(userContext.user?.id);
                                                    showSuccessMessage(ret.mensagemSucesso);
                                                }
                                                else {
                                                    throwError(ret.mensagemErro);
                                                }
                                            }
                                        }}
                                            disabled={userContext.loadingUpdate}
                                        >
                                            {userContext.loadingUpdate ? "Loading..." :
                                                <>
                                                    <FontAwesomeIcon icon={faSave} fixedWidth />&nbsp;Save
                                                </>}
                                        </Button>
                                    </div>
                                </Form>
                            </Card.Body>
                        </Card>
                    </Col>
                </Row>
            </Container>
        </>
    );
}