import { useContext, useEffect, useState } from "react";
import Col from "react-bootstrap/esm/Col";
import Container from "react-bootstrap/esm/Container";
import Row from "react-bootstrap/esm/Row";
import { Link, useNavigate } from "react-router-dom";
import ReactQuill from "react-quill";
import {
    Editor,
    Frame,
    Element,
    useEditor,
    useNode,
} from "@craftjs/core";
import "react-quill/dist/quill.snow.css";
import { CustomToolbar } from "../../Components/CustomToolbar";
import Nav from 'react-bootstrap/Nav';
import Card from 'react-bootstrap/Card';
import Form from 'react-bootstrap/Form';
import InputGroup from 'react-bootstrap/InputGroup';
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faArrowRight, faCalendar, faCreditCard, faLock, faUser } from "@fortawesome/free-solid-svg-icons";
import Button from "react-bootstrap/esm/Button";

export default function ProductPage() {

    let navigate = useNavigate();



    // Componente editável com Bootstrap
    const HeaderText = () => {
        const {
            connectors: { connect, drag },
            actions: { setProp },
            props,
        } = useNode((node) => ({
            props: node.data.props,
        }));

        const { query } = useEditor();

        const [editorContent, setEditorContent] = useState(props.html || "");

        useEffect(() => {
            setProp((props: any) => (props.html = editorContent));
        }, [editorContent, setProp]);

        const handleSave = () => {
            const json = query.serialize();
        };

        return (
            <div ref={(ref) => connect(drag(ref))} className="p-3 bg-light">
                <CustomToolbar onSave={handleSave} />
                <ReactQuill
                    theme="snow"
                    value={editorContent}
                    onChange={setEditorContent}
                    modules={{
                        toolbar: {
                            container: "#custom-toolbar",
                        },
                    }}
                    formats={[
                        "header",
                        "bold",
                        "italic",
                        "underline",
                        "size",
                        "link",
                        "clean",
                    ]}
                />
            </div>
        );

    };

    HeaderText.craft = {
        props: { html: "<h2>Minha Rede Principal</h2><p>Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum</p>" },
        displayName: "HeaderText",
    };

    return (
        <>
            <Container>
                <Row>
                    <Col md="12">
                        <Editor resolver={{ HeaderText }}>
                            <Frame>
                                <Element is="div" canvas>
                                    <HeaderText />
                                </Element>
                            </Frame>
                        </Editor>
                    </Col>
                </Row>
                <Row>
                    <Col md="12" className="py-4">
                        <Row>
                            <Col md={8}>
                                <h1>Doador Básico / Mensal</h1>
                                <p>
                                    Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum
                                </p>
                                <h2>Forma de Pagamento</h2>
                                <Nav variant="pills" defaultActiveKey="credito" className="py-4">
                                    <Nav.Item>
                                        <Nav.Link eventKey="credito">
                                            Cartão de Crédito
                                        </Nav.Link>
                                    </Nav.Item>
                                    <Nav.Item>
                                        <Nav.Link eventKey="boleto">
                                            Boleto Bancário
                                        </Nav.Link>
                                    </Nav.Item>
                                    <Nav.Item>
                                        <Nav.Link eventKey="pix">
                                            PIX
                                        </Nav.Link>
                                    </Nav.Item>
                                </Nav>
                                <Card>
                                    <Card.Header>
                                        <h3 className="text-center">Credit Card</h3>
                                    </Card.Header>
                                    <Card.Body>
                                        <Form>
                                            <Form.Group className="mb-3">
                                                <Form.Label>Credit Card Number:</Form.Label>
                                                <InputGroup>
                                                    <InputGroup.Text><FontAwesomeIcon icon={faCreditCard} fixedWidth /></InputGroup.Text>
                                                    <Form.Control type="text" size="lg" placeholder="Your Credit Card Number" />
                                                </InputGroup>
                                            </Form.Group>
                                            <Row>
                                                <Col md={3}>
                                                    <Form.Group className="mb-3">
                                                        <Form.Label>Expire Date:</Form.Label>
                                                        <InputGroup>
                                                            <InputGroup.Text><FontAwesomeIcon icon={faCalendar} fixedWidth /></InputGroup.Text>
                                                            <Form.Control type="text" size="lg" placeholder="MM/YYYY" />
                                                        </InputGroup>
                                                    </Form.Group>
                                                </Col>
                                                <Col md={3}>
                                                    <Form.Group className="mb-3">
                                                        <Form.Label>CCV:</Form.Label>
                                                        <InputGroup>
                                                            <InputGroup.Text><FontAwesomeIcon icon={faLock} fixedWidth /></InputGroup.Text>
                                                            <Form.Control type="text" size="lg" placeholder="000" />
                                                        </InputGroup>
                                                    </Form.Group>
                                                </Col>
                                                <Col md={6}>
                                                    <Form.Group className="mb-3">
                                                        <Form.Label>Name on Card:</Form.Label>
                                                        <InputGroup>
                                                            <InputGroup.Text><FontAwesomeIcon icon={faUser} fixedWidth /></InputGroup.Text>
                                                            <Form.Control type="text" size="lg" placeholder="Your Name exact on credit card" />
                                                        </InputGroup>
                                                    </Form.Group>
                                                </Col>
                                            </Row>

                                            <div className="d-grid gap-2 d-md-flex justify-content-md-end">
                                                <Button variant="success" size="lg" onClick={() => {
                                                    navigate("/recovery-password");
                                                }}>Pay <FontAwesomeIcon icon={faArrowRight} fixedWidth /></Button>
                                            </div>
                                        </Form>
                                    </Card.Body>
                                </Card>
                            </Col>
                            <Col md={4}>
                                <div className="card">
                                    <div className="card-header">
                                        <h4 className="my-0">Doação Suprema / Mensal</h4>
                                    </div>
                                    <div className="card-body text-center">
                                        <h5 className="card-title">
                                            <span className="display-4"><b>R$199,90</b></span>
                                            <span className="lead">/mês</span>
                                        </h5>

                                        <div className="card-text my-4 lc-block">
                                            <div>
                                                <ul className="list-unstyled">
                                                    <li>Pagamento Mensal</li>
                                                </ul>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </Col>
                        </Row>

                    </Col>
                </Row>
            </Container>
        </>
    );
}