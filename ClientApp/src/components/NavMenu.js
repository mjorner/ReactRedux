import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import { Glyphicon, Nav, Navbar, NavItem } from 'react-bootstrap';
import { LinkContainer } from 'react-router-bootstrap';
import './NavMenu.css';

export default class NavMenu extends Component {
  
  constructor(props) {
    super(props);
  }

  renderSnapShot(snapshot_file_name) {
    if (snapshot_file_name.length === 0) {
      return null;
    }
    return ( 
      <LinkContainer to={'/snapshot'}>
        <NavItem>
          <Glyphicon glyph='picture' /> Snapshot
        </NavItem>
      </LinkContainer>
    )
  }

  renderLogFiles(log_files) {
    if (log_files.length === 0) {
      return null;
    }
    return ( 
      <LinkContainer to={'/syslog'}>
          <NavItem>
            <Glyphicon glyph='list-alt' /> Logs
          </NavItem>
        </LinkContainer>
    )
  }

  render() {
    return (
      <Navbar inverse fixedTop fluid collapseOnSelect>
    <Navbar.Header>
      <Navbar.Brand>
        <Link to={'/'}>{this.props.app_title}</Link>
        <button type="button" class="button" onClick={this.props.reload_handler}>RELOAD</button>
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
            <Glyphicon glyph='equalizer' /> Graphs
          </NavItem>
        </LinkContainer>
        <LinkContainer to={'/stats'}>
          <NavItem>
            <Glyphicon glyph='stats' /> Stats
          </NavItem>
        </LinkContainer>
        {this.renderLogFiles(this.props.log_files)}
        {this.renderSnapShot(this.props.snapshot_file_name)}
      </Nav>
    </Navbar.Collapse>
  </Navbar>
    )
  }
}