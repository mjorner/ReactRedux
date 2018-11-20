import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { actionCreators } from '../store/WeatherForecasts';

class Home extends Component {

  constructor(props) {
    super(props);
    this.state = { filecontent: null};
  }

  async componentDidMount() {
    const url = "api/Data/GetFilenames"
    fetch(url)
    .then(results => {return results.json();})
    .then(async data => {
      const allGraphs = data.map(async file => {
        return await this.doReadFile(file.outFile, file.title);
      })
      const all = await Promise.all(allGraphs);
      this.setState({filecontent: all});
    });
  }

  async doReadFile(filename, title) {
    const url = "api/Data/ReadOutFile?filename="+filename+"&title="+title;
    const d = await fetch(url);
    const data = await d.json();
    return data;
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
          <th>Read</th>
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

export default connect(
  state => state.home,
  dispatch => bindActionCreators(actionCreators, dispatch)
)(Home);
