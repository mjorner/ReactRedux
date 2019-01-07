﻿import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import { Glyphicon, Nav, Navbar, NavItem } from 'react-bootstrap';
import { LinkContainer } from 'react-router-bootstrap';
import './NavMenu.css';

export default class NavMenu extends Component {
  
  renderSnapShot(snapshot_file_name) {
    if (snapshot_file_name.length !== 0) {
      return ( 
        <LinkContainer to={'/snapshot'}>
          <NavItem>
            <Glyphicon glyph='th-list' /> Snapshot
          </NavItem>
        </LinkContainer>
      )
    }
    return null;
  }

  render() {
    return (
      <Navbar inverse fixedTop fluid collapseOnSelect>
    <Navbar.Header>
      <Navbar.Brand>
        <Link to={'/'}>{this.props.app_title}</Link>
      </Navbar.Brand>
      <Navbar.Toggle />
    </Navbar.Header>
    <Navbar.Collapse>
      <Nav>
        <LinkContainer to={'/'} exact>
          <NavItem>
            <Glyphicon glyph='home' /> Home
          </NavItem>
        </LinkContainer>
        <LinkContainer to={'/fetchdata'}>
          <NavItem>
            <Glyphicon glyph='th-list' /> Graphs
          </NavItem>
        </LinkContainer>
        <LinkContainer to={'/stats'}>
          <NavItem>
            <Glyphicon glyph='th-list' /> Stats
          </NavItem>
        </LinkContainer>
        {this.renderSnapShot(this.props.snapshot_file_name)}
      </Nav>
    </Navbar.Collapse>
  </Navbar>
    )
  }
}