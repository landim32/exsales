import { useContext, useEffect, useState } from "react";
import Col from "react-bootstrap/esm/Col";
import Container from "react-bootstrap/esm/Container";
import Row from "react-bootstrap/esm/Row";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCalendar, faDollar, faEdit, faEnvelope, faPlus, faSearch, faTrash } from '@fortawesome/free-solid-svg-icons';
import Table from "react-bootstrap/esm/Table";
import { Link, useNavigate } from "react-router-dom";
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import InputGroup from 'react-bootstrap/InputGroup';
import Dropdown from 'react-bootstrap/Dropdown';
import Pagination from 'react-bootstrap/Pagination';

export default function TeamPage() {


    let navigate = useNavigate();

    return (
        <>
            <Container>
                <Row>
                    <Col md="6">
                        <h3>
                            <nav aria-label="breadcrumb">
                                <ol className="breadcrumb">
                                    <li className="breadcrumb-item"><Link to="/minha-rede/dashboard">Minha Rede</Link></li>
                                    <li className="breadcrumb-item active" aria-current="page">Network Team (3/10)</li>
                                </ol>
                            </nav>
                        </h3>
                    </Col>
                    <Col md="6" style={{ textAlign: "right" }}>
                        <InputGroup className="pull-right">
                            <Form.Control
                                placeholder="Search for Seller"
                                aria-label="Search for Seller"
                            />
                            <Button variant="outline-secondary"><FontAwesomeIcon icon={faSearch} fixedWidth /></Button>
                            <Dropdown>
                                <Dropdown.Toggle variant="danger" id="dropdown-basic">
                                    Filter by: All Profiles
                                </Dropdown.Toggle>

                                <Dropdown.Menu>
                                    <Dropdown.Item href="#/action-1">Action</Dropdown.Item>
                                    <Dropdown.Item href="#/action-2">Another action</Dropdown.Item>
                                    <Dropdown.Item href="#/action-3">Something else</Dropdown.Item>
                                </Dropdown.Menu>
                            </Dropdown>
                            <Button variant="primary"><FontAwesomeIcon icon={faEnvelope} fixedWidth /> Invite</Button>
                        </InputGroup>
                    </Col>
                </Row>
                <Row className="py-4">
                    <Col md="12">
                        <Table striped bordered hover>
                            <thead>
                                <tr>
                                    <th>Seller</th>
                                    <th>Profile</th>
                                    <th>Role</th>
                                    <th style={{ textAlign: "right" }}>Commission (%)</th>
                                    <th>Status</th>
                                    <th>Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td>Rodrigo Landim Carneiro</td>
                                    <td>Vendedor Supremo</td>
                                    <td>Administrator</td>
                                    <td style={{ textAlign: "right" }}>10%</td>
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
                                    <td>Sandoval Pereira da Silva</td>
                                    <td>Vendedor Superior</td>
                                    <td>Seller</td>
                                    <td style={{ textAlign: "right" }}>5%</td>
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
                                    <td>José Maria da Silva</td>
                                    <td>Vendedor Boqueta</td>
                                    <td>Seller</td>
                                    <td style={{ textAlign: "right" }}>3%</td>
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
                <Row>
                    <Col md={12} className="text-center">
                        <Pagination className="justify-content-center">
                            <Pagination.First />
                            <Pagination.Prev />
                            <Pagination.Item>{1}</Pagination.Item>
                            <Pagination.Ellipsis />

                            <Pagination.Item>{10}</Pagination.Item>
                            <Pagination.Item>{11}</Pagination.Item>
                            <Pagination.Item active>{12}</Pagination.Item>
                            <Pagination.Item>{13}</Pagination.Item>
                            <Pagination.Item disabled>{14}</Pagination.Item>

                            <Pagination.Ellipsis />
                            <Pagination.Item>{20}</Pagination.Item>
                            <Pagination.Next />
                            <Pagination.Last />
                        </Pagination>
                    </Col>
                </Row>
            </Container>
        </>
    );
}