import React, { Component } from 'react';
import { connect } from 'react-redux';

class Home extends Component {

  constructor(props) {
    super(props);
    this.state = { filecontent: null};
  }

  async componentDidMount() {
    const url = "api/Data/GetFilenames";
    const d = await fetch(url);
    const json = await d.json();
    var fc = [];
    for (var i = 0; i < json.fileNames.length; i++) {
      fc[i] = new LoadClass(json.fileNames[i].title, "Loading...")
    }
    this.setState({filecontent: fc});

    for (i = 0; i < json.fileNames.length; i++) {
      this.doReadFile(json.fileNames[i].outFile, json.fileNames[i].title, i);
    }
  }

  async doReadFile(filename, title, i) {
    const url = "api/Data/ReadOutFile?filename="+filename+"&title="+title;
    const d = await fetch(url);
    const data = await d.json();
    var st = this.state.filecontent;
    st[i] = data;
    this.setState({filecontent: st});
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
