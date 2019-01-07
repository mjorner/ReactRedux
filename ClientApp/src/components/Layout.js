import React from 'react';
import { Col, Grid, Row } from 'react-bootstrap';
import NavMenu from './NavMenu';

export default props => (
  <Grid fluid>
    <Row>
      <Col sm={3}>
        <NavMenu app_title={props.app_title} snapshot_file_name={props.snapshot_file_name}/>
      </Col>
      <Col sm={9}>
        {props.children}
      </Col>
    </Row>
  </Grid>
);
