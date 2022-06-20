﻿import React, { Component } from 'react';
import { Button, Row, Col, Input, InputGroup, InputGroupAddon } from 'reactstrap';
import ModalUser from './ModalUser';
import { CustomPagination } from './Pagination';

export class Users extends Component {
    static displayName = Users.name;

    constructor(props) {
        super(props);
        this.state = {
            users: [],
            searchString: "",
            urlForPagination: "",
            postfixUrlForPagination: "",
            emptyUser: { name: "", password: "", role: "user", login: "" }
        };
        this.getUsers = this.getUsers.bind(this);
        this.onSearchStringChange = this.onSearchStringChange.bind(this);
    }

    componentDidMount() {
        this.getUsers("/api/users/");
        this.setState({ searchString: "", urlForPagination: "api/users/", postfixUrlForPagination: "" });
    }
    onSearchStringChange(e) {
        this.setState({ searchString: e.target.value });
    }
    async getUsers(url) {
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
            this.setState({ users: [] });
            this.setState({ users: data });
        }
        else {
            console.log("Error: ", response.status);
        }
    }
    shouldComponentUpdate(nextProps, nextState) {
        return this.state.users != nextState.users
    }
    render() {
        return (
            <div>
                <h2>Список пользователей:</h2>
                <br />
                <Row>
                    <Col xs="1"><strong>Поиск</strong></Col>
                    <Col xs="5">
                        <InputGroup>
                            <Input value={this.state.searchString} onChange={this.onSearchStringChange} />
                            <InputGroupAddon addonType="append">
                                <Button color="secondary" outline onClick={() => { this.getUsers("api/users/page/1"); this.setState({ searchString: "", urlForPagination: "api/users/", postfixUrlForPagination: "" }) }}>&#215;</Button>
                            </InputGroupAddon>
                        </InputGroup>
                    </Col>
                    <Col xs="2"><Button color="info" className="col-12" onClick={() => {
                        if (this.state.searchString !== "") {
                            this.getUsers("api/users/name/page/1/" + this.state.searchString);
                            this.setState({ urlForPagination: "api/users/name/", postfixUrlForPagination: this.state.searchString });
                        }
                        else {
                            this.getPatients("api/users/page/1");
                            this.setState({ urlForPagination: "api/users/", postfixUrlForPagination: "" })
                        }
                    }}>Найти</Button></Col>
                    <Col xs="1"></Col>
                    <Col xs="auto"><ModalUser user={this.state.emptyUser} isCreate={true} onClose={this.getUsers} /></Col>
                </Row>
                <br />
                <hr />
                <div>
                    {
                        this.state.users.map((user) => {
                            return <User user={user} getUsers={this.getUsers} key={user.id} />
                        })
                    }
                </div>
                <br />
                <CustomPagination controllerUrl={this.state.urlForPagination} postfixUrl={this.state.postfixUrlForPagination} getContent={this.getUsers} className="col-12" />
            </div>
        );
    }
}

class User extends Component {
    static displayName = User.name;

    constructor(props) {
        super(props);
        this.state = {
            user: props.user
        };
    }

    render() {
        return (
            <div>
                <Row>
                    <Col xs="6">{this.state.user.name}</Col>
                    <Col><ModalUser user={this.state.user} isCreate={false} onClose={this.props.getUsers} /></Col>
                    <Col xs="2"></Col>
                </Row>
                <br />
            </div>
        );
    }
}

