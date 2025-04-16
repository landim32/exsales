import { useContext, useState } from "react";
import AuthContext from "../../Contexts/Auth/AuthContext";
import Container from "react-bootstrap/esm/Container";
import Row from "react-bootstrap/esm/Row";
import Col from "react-bootstrap/esm/Col";
import Card from "react-bootstrap/esm/Card";
import Alert from 'react-bootstrap/Alert';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faWarning, faPlus, faBurn, faFire, faSearch, faDollar, faClock, faBoltLightning, faLock, faFileUpload, faCalendar, faCalendarAlt, faFileWord, faBoxOpen, faSign, faLockOpen, faUserDoctor, faChartLine, faChartPie, faCoins } from '@fortawesome/free-solid-svg-icons';
import Button from "react-bootstrap/esm/Button";
import { Link, useNavigate } from "react-router-dom";
import { faBitcoin, faOpencart } from "@fortawesome/free-brands-svg-icons";
import CardHeader from "react-bootstrap/esm/CardHeader";
import CardTitle from "react-bootstrap/esm/CardTitle";
import CardBody from "react-bootstrap/esm/CardBody";
import CardText from "react-bootstrap/esm/CardText";
import Header from "./Header";
import Footer from "./Footer";
import Features from "./Features";
import Pricing from "./Pricing";



export default function HomePage() {

    const authContext = useContext(AuthContext);

    let navigate = useNavigate();

    return (
        <>
            <Header />
            <Features />
            <section id="how-it-works" className="bg-light py-5">
                <Container fluid>
                    <Row className="mb-4">
                        <Col md={12} className="text-center">
                            <h4 className="display-2 mb-0">TOP 3 Networks</h4>
                        </Col>
                    </Row>
                    <div className="row row-cols-1 row-cols-lg-3 justify-content-center py-6">
                        <div className="col lc_border_lg w-auto ">
                            <div className="lc-block">
                                <div className="lc-block card border-0 bg-transparent">
                                    <div className="card-body">
                                        <div className="d-flex px-1 px-lg-3 ">
                                            <div className="lc-block">
                                                <FontAwesomeIcon icon={faSearch} size="2x" />
                                            </div>
                                            <div className="ps-2 ps-md-3">
                                                <div className="lc-block">
                                                    <h3 className="rfs-6"><Link to="/minha-rede">Minha Rede Principal</Link></h3>
                                                    <p className="text-muted rfs-4">3 products and 7 affiliate sellers</p>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div className="col lc_border_lg w-auto">
                            <div className="lc-block card border-0 bg-transparent">
                                <div className="card-body">
                                    <div className="d-flex px-1 px-lg-3 ">
                                        <div className="lc-block">
                                            <FontAwesomeIcon icon={faDollar} size="2x" />
                                        </div>
                                        <div className="ps-2 ps-md-3">
                                            <div className="lc-block">

                                                <h3 className="rfs-6"><Link to="/minha-rede">Minha Rede Secundária</Link></h3>
                                                <p className="text-muted rfs-4">5 products and 3 affiliate sellers</p>

                                            </div>

                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div className="col w-auto">

                            <div className="lc-block  card border-0 bg-transparent">
                                <div className="card-body">
                                    <div className="d-flex px-1 px-lg-3 ">
                                        <div className="lc-block">
                                            <FontAwesomeIcon icon={faClock} size="2x" />
                                        </div>
                                        <div className="ps-2 ps-md-3">
                                            <div className="lc-block">

                                                <h3 className="rfs-6"><Link to="/minha-rede">Última Rede</Link></h3>
                                                <p className="text-muted rfs-4">1 products and 2 affiliate sellers</p>

                                            </div>

                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </Container>
            </section>
            <Pricing />
            <section id="contact" className="bg-light py-5">
                <div className="container-fluid py-5">
                    <div className="row text-center">
                        <div className="col-sm-6 col-lg-3 py-5">
                            <div className="lc-block mb-5">
                                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 320 512" width="3em" height="3em" lc-helper="svg-icon" fill="currentColor">
                                    <path d="M279.14 288l14.22-92.66h-88.91v-60.13c0-25.35 12.42-50.06 52.24-50.06h40.42V6.26S260.43 0 225.36 0c-73.22 0-121.08 44.38-121.08 124.72v70.62H22.89V288h81.39v224h100.17V288z"></path>
                                </svg>
                            </div>
                            <div className="lc-block mb-4">
                                <div>
                                    <p><a href="#" className="h3 text-dark text-decoration-none">Facebook</a></p>
                                </div>
                            </div>
                            <div className="lc-block col-2 offset-5 border-bottom border-2 border-dark">
                                <div>
                                </div>
                            </div>
                        </div>
                        <div className="col-sm-6 col-lg-3 py-5">
                            <div className="lc-block mb-5">
                                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512" width="3em" height="3em" lc-helper="svg-icon" fill="currentColor">
                                    <path d="M100.28 448H7.4V148.9h92.88zM53.79 108.1C24.09 108.1 0 83.5 0 53.8a53.79 53.79 0 0 1 107.58 0c0 29.7-24.1 54.3-53.79 54.3zM447.9 448h-92.68V302.4c0-34.7-.7-79.2-48.29-79.2-48.29 0-55.69 37.7-55.69 76.7V448h-92.78V148.9h89.08v40.8h1.3c12.4-23.5 42.69-48.3 87.88-48.3 94 0 111.28 61.9 111.28 142.3V448z"></path>
                                </svg>
                            </div>
                            <div className="lc-block mb-4">
                                <div>
                                    <p><a href="#" className="h3 text-dark text-decoration-none">Linkedin</a></p>
                                </div>
                            </div>
                            <div className="lc-block col-2 offset-5 border-bottom border-2 border-dark">
                                <div>
                                </div>
                            </div>
                        </div>
                        <div className="col-sm-6 col-lg-3 py-5">
                            <div className="lc-block mb-5">
                                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512" width="3em" height="3em" lc-helper="svg-icon" fill="currentColor">
                                    <path d="M459.37 151.716c.325 4.548.325 9.097.325 13.645 0 138.72-105.583 298.558-298.558 298.558-59.452 0-114.68-17.219-161.137-47.106 8.447.974 16.568 1.299 25.34 1.299 49.055 0 94.213-16.568 130.274-44.832-46.132-.975-84.792-31.188-98.112-72.772 6.498.974 12.995 1.624 19.818 1.624 9.421 0 18.843-1.3 27.614-3.573-48.081-9.747-84.143-51.98-84.143-102.985v-1.299c13.969 7.797 30.214 12.67 47.431 13.319-28.264-18.843-46.781-51.005-46.781-87.391 0-19.492 5.197-37.36 14.294-52.954 51.655 63.675 129.3 105.258 216.365 109.807-1.624-7.797-2.599-15.918-2.599-24.04 0-57.828 46.782-104.934 104.934-104.934 30.213 0 57.502 12.67 76.67 33.137 23.715-4.548 46.456-13.32 66.599-25.34-7.798 24.366-24.366 44.833-46.132 57.827 21.117-2.273 41.584-8.122 60.426-16.243-14.292 20.791-32.161 39.308-52.628 54.253z"></path>
                                </svg>
                            </div>
                            <div className="lc-block mb-4">
                                <div>
                                    <p><a href="#" className="h3 text-dark text-decoration-none">Twitter</a></p>
                                </div>
                            </div>
                            <div className="lc-block col-2 offset-5 border-bottom border-2 border-dark">
                                <div>
                                </div>
                            </div>
                        </div>
                        <div className="col-sm-6 col-lg-3 py-5">
                            <div className="lc-block mb-5">
                                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512" width="3em" height="3em" lc-helper="svg-icon" fill="currentColor">
                                    <path d="M256 8C119.252 8 8 119.252 8 256s111.252 248 248 248 248-111.252 248-248S392.748 8 256 8zm163.97 114.366c29.503 36.046 47.369 81.957 47.835 131.955-6.984-1.477-77.018-15.682-147.502-6.818-5.752-14.041-11.181-26.393-18.617-41.614 78.321-31.977 113.818-77.482 118.284-83.523zM396.421 97.87c-3.81 5.427-35.697 48.286-111.021 76.519-34.712-63.776-73.185-116.168-79.04-124.008 67.176-16.193 137.966 1.27 190.061 47.489zm-230.48-33.25c5.585 7.659 43.438 60.116 78.537 122.509-99.087 26.313-186.36 25.934-195.834 25.809C62.38 147.205 106.678 92.573 165.941 64.62zM44.17 256.323c0-2.166.043-4.322.108-6.473 9.268.19 111.92 1.513 217.706-30.146 6.064 11.868 11.857 23.915 17.174 35.949-76.599 21.575-146.194 83.527-180.531 142.306C64.794 360.405 44.17 310.73 44.17 256.323zm81.807 167.113c22.127-45.233 82.178-103.622 167.579-132.756 29.74 77.283 42.039 142.053 45.189 160.638-68.112 29.013-150.015 21.053-212.768-27.882zm248.38 8.489c-2.171-12.886-13.446-74.897-41.152-151.033 66.38-10.626 124.7 6.768 131.947 9.055-9.442 58.941-43.273 109.844-90.795 141.978z"></path>
                                </svg>
                            </div>
                            <div className="lc-block mb-4">
                                <div>
                                    <p><a href="#" className="h3 text-dark text-decoration-none">Dribble</a></p>
                                </div>
                            </div>
                            <div className="lc-block col-2 offset-5 border-bottom border-2 border-dark">
                                <div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
            <Footer />
        </>
    );

}