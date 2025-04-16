import { useContext, useEffect, useState } from "react";
import Col from "react-bootstrap/esm/Col";
import Container from "react-bootstrap/esm/Container";
import Row from "react-bootstrap/esm/Row";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCalendar, faDollar, faSearch, faTrash } from '@fortawesome/free-solid-svg-icons';
import Table from "react-bootstrap/esm/Table";
import { Link, useNavigate } from "react-router-dom";
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import InputGroup from 'react-bootstrap/InputGroup';

export default function NetworkListPage() {


    let navigate = useNavigate();

    return (
        <>
            <Container>
                <Row>
                    <Col md="12">
                        <InputGroup>
                            <Form.Control
                                placeholder="Search for Network"
                                aria-label="Search for Network"
                            />
                            <Button variant="outline-secondary"><FontAwesomeIcon icon={faSearch} fixedWidth /></Button>
                        </InputGroup>
                    </Col>
                </Row>
                <Row className="py-4">
                    <Col md="12">
                        <Table striped bordered hover>
                            <thead>
                                <tr>
                                    <th>Network</th>
                                    <th style={{ textAlign: "right" }}>Commission (%)</th>
                                    <th>Owner</th>
                                    <th style={{ textAlign: "right" }}>Members</th>
                                    <th>Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td><Link to="/minha-rede/dashboard">Minha Rede Principal</Link></td>
                                    <td style={{ textAlign: "right" }}>10%</td>
                                    <td>Rodrigo L.</td>
                                    <td style={{ textAlign: "right" }}>7/100</td>
                                    <td>
                                        <Link to="/minha-rede">
                                            <FontAwesomeIcon icon={faSearch} fixedWidth />
                                        </Link>
                                        <Link to="/minha-rede/dashboard">
                                            <FontAwesomeIcon icon={faTrash} fixedWidth />
                                        </Link>
                                    </td>
                                </tr>
                                <tr>
                                    <td><Link to="/minha-rede/dashboard">Rede Secundária</Link></td>
                                    <td style={{ textAlign: "right" }}>15%</td>
                                    <td>Rodrigo L.</td>
                                    <td style={{ textAlign: "right" }}>0/10</td>
                                    <td>
                                        <Link to="/minha-rede">
                                            <FontAwesomeIcon icon={faSearch} fixedWidth />
                                        </Link>
                                        <Link to="/minha-rede/dashboard">
                                            <FontAwesomeIcon icon={faTrash} fixedWidth />
                                        </Link>
                                    </td>
                                </tr>
                                <tr>
                                    <td><Link to="/minha-rede/dashboard">Última rede</Link></td>
                                    <td style={{ textAlign: "right" }}>5%</td>
                                    <td>Rodrigo L.</td>
                                    <td style={{ textAlign: "right" }}>1/5</td>
                                    <td>
                                        <Link to="/minha-rede">
                                            <FontAwesomeIcon icon={faSearch} fixedWidth />
                                        </Link>
                                        <Link to="/minha-rede/dashboard">
                                            <FontAwesomeIcon icon={faTrash} fixedWidth />
                                        </Link>
                                    </td>
                                </tr>
                            </tbody>
                        </Table>
                    </Col>
                </Row>
            </Container>
        </>
    );
}