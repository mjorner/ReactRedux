import React from 'react';
import { Route } from 'react-router';
import Layout from './components/Layout';
import Home from './components/Home';
import FetchData from './components/FetchData';
import Stats from './components/Stats';
import Snapshot from './components/Snapshot';
import SysLog from './components/SysLog';
import Login from './components/Login';

class App extends React.Component {
  constructor(props) {
    super(props);
    this.state = { app_config: null }
    this.reload = this.reload.bind(this);
  }

  async componentDidMount() {
    const url = "api/Data/GetAppConfiguration"
    const data = await fetch(url);
    const json = await data.json();
    this.setState({app_config: json});
  }

  reload() {
    this.reloadChild();
  }

  renderSnapShot(title, snapshot_file_name) {
    if (snapshot_file_name.length !== 0) {
      return ( 
        <Route path='/snapshot' render={props => <Snapshot {...props} app_title={title} snapshot_file_name={snapshot_file_name} setReloadHandler={handler => this.reloadChild = handler}/>}/>
      )
    }
  }

  renderLogFiles(title, log_files) {
    if (log_files.length !== 0) {
      return (
        <Route path='/syslog' render={props => <SysLog {...props} app_title={title} log_files={log_files} setReloadHandler={handler => this.reloadChild = handler}/>}/>
      )
    }
  }

  renderLogin(title) {
    return (
      <Route path='/login' render={props => <Login {...props} app_title={title}/>}/>
    )
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
      const log_files = this.state.app_config.logFiles;
      return (
        <Layout app_title={title} snapshot_file_name={snapshot_file_name} log_files={log_files} reload_handler={this.reload}>
          <Route exact path='/' render={props => <Home {...props} app_title={title} setReloadHandler={handler => this.reloadChild = handler}/>}/>
          <Route path='/fetchdata' render={props => <FetchData {...props} app_title={title} setReloadHandler={handler => this.reloadChild = handler}/>}/>
          <Route path='/stats' render={props => <Stats {...props} app_title={title} setReloadHandler={handler => this.reloadChild = handler}/>}/>
          {this.renderLogFiles(title, log_files)}
          {this.renderSnapShot(title, snapshot_file_name)}
          {this.renderLogin(title)}
        </Layout>
      )
    }
  } 
}
export default App;