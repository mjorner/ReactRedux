import React, { Component } from 'react';
import { connect } from 'react-redux';

class Home extends Component {

  constructor(props) {
    super(props);
    this.state = { filecontent: null};
    document.title = this.props.app_title;
    this.reload = this.reload.bind(this);
  }

  async componentDidMount() {
    this.props.setReloadHandler(this.reload);
    this.reload();
  }

  async reload() {
    const url = "api/Data/GetFilenames";
    const d = await fetch(url);
    const json = await d.json();
    var fileNames = this.removeInvalidFileNames(json.fileNames);
    var fc = [];
    for (var i = 0; i < fileNames.length; i++) {
      fc[i] = new LoadClass(fileNames[i].title, "Loading...")    
    }
    this.setState({filecontent: fc});

    for (i = 0; i < fileNames.length; i++) {
      this.doReadFile(fileNames[i].outFile, fileNames[i].title, fileNames[i].type, i);
    }
  }

  removeInvalidFileNames(fileNames) {
    var pruned = [];
    var prunedIndex = 0;
    for (var i = 0; i < fileNames.length; i++) {
      if (fileNames[i].outFile.length > 0 && fileNames[i].active) {
        pruned[prunedIndex++] = fileNames[i];
      }
    }
    return pruned;
  }

  async doReadFile(filename, title, type, i) {
    const url = "api/Data/ReadOutFile?filename="+filename+"&title="+title;
    const d = await fetch(url);
    const data = await d.json();
    var st = this.state.filecontent;
    const formatted = this.doFormat(data.str, type);
    data.str = formatted;
    st[i] = data;
    this.setState({filecontent: st});
  }

  doFormat(data, type) {
    if (type === 'PIR') {
      return this.getPirLastCaught(data);
    }
    else if (type === 'LAWNM') {
      return this.takeFirstPart(data);
    }
    return data;
  }

  takeFirstPart(data) {
    return data.split(";")[0];
  }

  getPirLastCaught(motion) {
    var time_in_millis = Date.now()/1000;
    var lg = parseInt(motion);
    var diff = Math.floor(time_in_millis-lg);
    var daysSince = this.convertSecondsToDays(diff);
    var at = new Date(lg*1000);
    var month = at.getMonth() + 1;
    month = (month < 10 ? "0" : "") + month;

    var day  = at.getDate();
    day = (day < 10 ? "0" : "") + day;

    return daysSince+" ("+at.getFullYear()+"-"+month+"-"+day+")";
  }

  convertSecondsToDays(seconds) {
    var numdays = Math.floor(seconds / 86400);
    var numhours = Math.floor((seconds % 86400) / 3600);
    var numminutes = Math.floor(((seconds % 86400) % 3600) / 60);
    var numseconds = Math.floor(((seconds % 86400) % 3600) % 60);
    var uptimestr = '';
    if (numdays>0) {
      if (numdays<10) { uptimestr='0'+numdays+'d '; }
      else { uptimestr=numdays+'d ';}
    }
    else {
      uptimestr='00d ';
    }
    if (numhours>0) {
      if (numhours<10) { uptimestr=uptimestr + '0' + numhours +'h '; }
      else { uptimestr=uptimestr + numhours +'h '; }
    }
    else {
      uptimestr=uptimestr +'00h ';
    }
    if (numminutes>0) {
      if (numminutes<10) { uptimestr=uptimestr + '0' + numminutes +'m '; }
      else { uptimestr=uptimestr + numminutes +'m '; }
    }
    else {
      uptimestr=uptimestr +'00m ';
    }
    if (numseconds>0) {
      if (numseconds<10) { uptimestr=uptimestr + '0' + numseconds +'s '; }
      else { uptimestr=uptimestr + numseconds +'s '; }
    }
    else {
      uptimestr=uptimestr +'00s';
    }
    return uptimestr.trim();
  }

  render() {
    if (this.state.filecontent === null) {
      return (
        <div>Loading data...</div>
      )
    } else {
      return (
        <div>
          {renderTable(this.state.filecontent)}
        </div>
      )
    }
  }
}

function renderTable(props) {
  return (
    <table className='table'>
      <thead>
        <tr>
          <th>Name</th>
          <th>Value</th>
        </tr>
      </thead>
      <tbody>
        {props.map(entity =>
          <tr key={entity.title}>
            <td>{entity.title}</td>
            <td>{entity.str}</td>
          </tr>
        )}
      </tbody>
    </table>
  );
}

class LoadClass {
  constructor(title, str) {
    this.title = title;
    this.str = str;
  }
}

export default connect(
)(Home);
