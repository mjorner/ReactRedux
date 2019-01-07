import React from 'react';
import { Route } from 'react-router';
import Layout from './components/Layout';
import Home from './components/Home';
import FetchData from './components/FetchData';
import Stats from './components/Stats';
import Snapshot from './components/Snapshot';

class App extends React.Component {
  constructor(props) {
    super(props);
    this.state = { app_config: null }
  }

  async componentDidMount() {
    const url = "api/Data/GetAppConfiguration"
    const data = await fetch(url);
    const json = await data.json();
    this.setState({app_config: json});
  }

  renderSnapShot(title, snapshot_file_name) {
    if (snapshot_file_name.length !== 0) {
      return ( 
        <Route path='/snapshot' render={props => <Snapshot {...props} app_title={title} snapshot_file_name={snapshot_file_name} />}/>
      )
    }
  }

  render() {  
    if (this.state.app_config === null) {
      return (
        <div></div>
      )
    }
    else {
      const title = this.state.app_config.appTitle;
      const snapshot_file_name = this.state.app_config.snapShotFile;
      return (
        <Layout app_title={title} snapshot_file_name={snapshot_file_name}>
          <Route exact path='/' render={props => <Home {...props} app_title={title} />}/>
          <Route path='/fetchdata' render={props => <FetchData {...props} app_title={title} />}/>
          <Route path='/stats' render={props => <Stats {...props} app_title={title} />}/>
          {this.renderSnapShot(title, snapshot_file_name)}
        </Layout>
      )
    }
  } 
}
export default App;