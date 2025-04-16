import { useContext, useEffect, useState } from "react";
import Col from "react-bootstrap/esm/Col";
import Container from "react-bootstrap/esm/Container";
import Row from "react-bootstrap/esm/Row";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCalendar, faDollar, faEdit, faPlus, faSearch, faTrash } from '@fortawesome/free-solid-svg-icons';
import Table from "react-bootstrap/esm/Table";
import { Link, useNavigate } from "react-router-dom";
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import InputGroup from 'react-bootstrap/InputGroup';
import Dropdown from 'react-bootstrap/Dropdown';

export default function TeamStructurePage() {


    let navigate = useNavigate();

    return (
        <>
            <Container>
                <Row>
                    <Col md="8">
                        <h3>
                            <nav aria-label="breadcrumb">
                                <ol className="breadcrumb">
                                    <li className="breadcrumb-item"><Link to="/minha-rede/dashboard">Minha Rede</Link></li>
                                    <li className="breadcrumb-item active" aria-current="page">Network Team Structure</li>
                                </ol>
                            </nav>
                        </h3>
                    </Col>
                    <Col md="4" style={{ textAlign: "right" }}>
                        <InputGroup className="pull-right">
                            <Dropdown>
                                <Dropdown.Toggle variant="danger" id="dropdown-basic">
                                    Filter by: All Status
                                </Dropdown.Toggle>

                                <Dropdown.Menu>
                                    <Dropdown.Item href="#/action-1">Action</Dropdown.Item>
                                    <Dropdown.Item href="#/action-2">Another action</Dropdown.Item>
                                    <Dropdown.Item href="#/action-3">Something else</Dropdown.Item>
                                </Dropdown.Menu>
                            </Dropdown>
                            <Button variant="primary"><FontAwesomeIcon icon={faPlus} fixedWidth /> New</Button>
                        </InputGroup>
                    </Col>
                </Row>
                <Row className="py-4">
                    <Col md="12">
                        <Table striped bordered hover>
                            <thead>
                                <tr>
                                    <th>Profile Name</th>
                                    <th style={{ textAlign: "right" }}>Level</th>
                                    <th style={{ textAlign: "right" }}>Commission (%)</th>
                                    <th style={{ textAlign: "right" }}>Members</th>
                                    <th>Status</th>
                                    <th>Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td><Link to="/minha-rede/team-structure/new">Vendedor Superior</Link></td>
                                    <td style={{ textAlign: "right" }}>1</td>
                                    <td style={{ textAlign: "right" }}>10%</td>
                                    <td style={{ textAlign: "right" }}>2</td>
                                    <td>Active</td>
                                    <td>
                                        <Link to="/minha-rede/team-structure/new">
                                            <FontAwesomeIcon icon={faEdit} fixedWidth />
                                        </Link>
                                        <Link to="/minha-rede/team-structure/new">
                                            <FontAwesomeIcon icon={faTrash} fixedWidth />
                                        </Link>
                                    </td>
                                </tr>
                                <tr>
                                    <td><Link to="/minha-rede/team-structure/new">Vendedor Normal</Link></td>
                                    <td style={{ textAlign: "right" }}>2</td>
                                    <td style={{ textAlign: "right" }}>7%</td>
                                    <td style={{ textAlign: "right" }}>1</td>
                                    <td>Active</td>
                                    <td>
                                        <Link to="/minha-rede/team-structure/new">
                                            <FontAwesomeIcon icon={faEdit} fixedWidth />
                                        </Link>
                                        <Link to="/minha-rede/team-structure/new">
                                            <FontAwesomeIcon icon={faTrash} fixedWidth />
                                        </Link>
                                    </td>
                                </tr>
                                <tr>
                                    <td><Link to="/minha-rede/team-structure/new">Vendedor Boqueta</Link></td>
                                    <td style={{ textAlign: "right" }}>3</td>
                                    <td style={{ textAlign: "right" }}>3%</td>
                                    <td style={{ textAlign: "right" }}>1</td>
                                    <td>Active</td>
                                    <td>
                                        <Link to="/minha-rede/team-structure/new">
                                            <FontAwesomeIcon icon={faEdit} fixedWidth />
                                        </Link>
                                        <Link to="/minha-rede/team-structure/new">
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