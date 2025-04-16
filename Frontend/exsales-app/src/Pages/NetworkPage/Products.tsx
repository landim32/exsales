import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { useNavigate } from "react-router-dom";
import Container from "react-bootstrap/esm/Container";
import Row from "react-bootstrap/esm/Row";
import Col from "react-bootstrap/esm/Col";
import { faBoltLightning, faLock, faFileUpload, faCalendarAlt, faFileWord, faBoxOpen, faLockOpen, faUserDoctor } from '@fortawesome/free-solid-svg-icons';
import { faBitcoin } from "@fortawesome/free-brands-svg-icons";
import Card from "react-bootstrap/esm/Card";
import CardHeader from "react-bootstrap/esm/CardHeader";
import CardTitle from "react-bootstrap/esm/CardTitle";
import CardBody from "react-bootstrap/esm/CardBody";
import CardText from "react-bootstrap/esm/CardText";
import Button from "react-bootstrap/esm/Button";

export default function Products() {

    let navigate = useNavigate();

    return (
        <>
            <section id="network" className="py-5">
                <Container className="pb-5">
                    <Row>
                        <Col md={12} className="text-center">
                            <div className="lc-block mb-4">
                                <h2 className="display-2 mb-0"><b>Plans</b></h2>
                                <p> Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nunc et metus id<br /> ligula malesuada placerat sit amet quis enim.</p>
                            </div>
                        </Col>
                    </Row>
                    <Row md={4} className="text-center">
                        <Col lg={4} md={6} className="text-dark my-2">
                            <Card>
                                <CardHeader>
                                    <h4 className="my-0">Doação Básica / Mensal</h4>
                                </CardHeader>
                                <CardBody>
                                    <CardTitle>
                                        <span className="display-4"><b>R$29,90</b></span>
                                        <span className="lead">/mês</span>
                                    </CardTitle>
                                    <CardText className="my-4 lc-block">
                                        <div>
                                            <ul className="list-unstyled">
                                                <li>Pagamento Mensal</li>
                                            </ul>
                                        </div>
                                    </CardText>
                                    <div className="d-grid lc-block">
                                        <Button variant="primary" size="lg" className="btn-outline-primary" onClick={(e) => {
                                            navigate("/minha-rede/p/doacao-basica");
                                        }}>Order Now</Button>
                                    </div>
                                </CardBody>
                            </Card>
                        </Col>
                        <Col lg={4} md={6} className="text-dark my-2">
                            <Card>
                                <CardHeader>
                                    <h4 className="my-0">Doação Avançada / Mensal</h4>
                                </CardHeader>
                                <CardBody>
                                    <CardTitle>
                                        <span className="display-4"><b>R$99,90</b></span>
                                        <span className="lead">/mês</span>
                                    </CardTitle>
                                    <CardText className="my-4 lc-block">
                                        <div>
                                            <ul className="list-unstyled">
                                                <li>Pagamento Mensal</li>
                                            </ul>

                                        </div>
                                    </CardText>
                                    <div className="d-grid lc-block">
                                        <Button variant="primary" size="lg" onClick={(e) => {
                                            navigate("/minha-rede/p/doacao-basica");
                                        }}>Order Now</Button>
                                    </div>
                                </CardBody>
                            </Card>
                        </Col>
                        <div className="col-lg-4 col-md-6 text-dark my-2">
                            <div className="card">
                                <div className="card-header">
                                    <h4 className="my-0">Doação Suprema / Mensal</h4>
                                </div>
                                <div className="card-body">
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
                                    <div className="d-grid lc-block">
                                        <Button variant="primary" size="lg" onClick={(e) => {
                                            navigate("/minha-rede/p/doacao-basica");
                                        }}>Order Now</Button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </Row>
                </Container>
            </section>
        </>
    );
}