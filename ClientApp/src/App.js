import React from 'react';
import { Route } from 'react-router';
import Layout from './components/Layout';
import Home from './components/Home';
import FetchData from './components/FetchData';
import Stats from './components/Stats';

export default () => (
  <Layout>
    <Route exact path='/' component={Home} />
    <Route path='/fetchdata' component={FetchData} />
    <Route path='/stats' component={Stats} />
  </Layout>
);
