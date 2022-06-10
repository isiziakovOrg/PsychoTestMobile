﻿import React, { Component } from 'react';
import { Button, Row, Col, Input, InputGroup, InputGroupAddon } from 'reactstrap';


export class Patients extends Component {
    static displayName = Patients.name;

    constructor(props) {
        super(props);
        this.state = {
            patients: [],
            searchString: ""
        };
        this.getPatients = this.getPatients.bind(this);
        this.onSearchStringChange = this.onSearchStringChange.bind(this);
    }

    componentDidMount() {
        this.getPatients("/api/patients/");
        this.setState({ searchString: "" });
    }

    onSearchStringChange(e) {
        this.setState({ searchString: e.target.value });
    }

    async getPatients(url) {
        const token = sessionStorage.getItem('tokenKey');
        var response = await fetch(url, {
            method: "GET",
            headers: {
                "Accept": "application/json",
                "Authorization": "Bearer " + token
            }
        });
        var data = await response.json();
        if (response.ok === true) {
            this.setState({ patients: [] });
            this.setState({ patients: data });
        }
        else {
            console.log("Error: ", response.status);
        }
    }

    render() {
        return (
            <div>
                <h2>Список пациентов:</h2>
                <br />
                <Row>
                    <Col xs="1"><strong>Поиск</strong></Col>
                    <Col xs="5">
                        <InputGroup>
                            <Input value={this.state.searchString} onChange={this.onSearchStringChange} />
                            <InputGroupAddon addonType="append">
                                <Button color="secondary" outline onClick={() => { this.getPatients("api/patients/"); this.setState({ searchString: "" }) }}>&#215;</Button>
                            </InputGroupAddon>
                        </InputGroup>
                    </Col>
                    <Col xs="3"><Button color="info" onClick={() => { this.getPatients("api/patients/name/" + this.state.searchString) }}>Найти</Button></Col>
                    <Col xs="auto"><Button color="info" >Добавить пациента</Button></Col>
                </Row>
                <br />
                <hr />
                <div>
                    {
                        this.state.patients.map((patient) => {
                            return <Patient patient={patient} />
                        })
                    }
                </div>
            </div>
        );
    }
}

class Patient extends Component {
    static displayName = Patient.name;

    constructor(props) {
        super(props);
        this.state = {
            patient: props.patient
        };
    }

    render() {
        return (
            <div>
                <Row>
                    <Col xs="6">{this.state.patient.name}</Col>
                    <Col xs="auto"><Button color="info" outline >Просмотр</Button></Col>
                </Row>
                <br />
            </div>
        );
    }
}