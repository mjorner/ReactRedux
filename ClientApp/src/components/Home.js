import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { actionCreators } from '../store/WeatherForecasts';
import Plot from 'react-plotly.js';


class Home extends Component {

  constructor(props) {
    super(props);
    this.state = { filecontent: [],};
  }

  async componentDidMount() {
    const url = "api/SampleData/GetFilenames"
    fetch(url)
    .then(results => {return results.json();})
    .then(async data => {
      const allGraphs = data.map(async file => {
        return await this.doReadFile(file.outFile);
      })
      const all = await Promise.all(allGraphs);
      this.setState({filecontent: all});
    });
  }

  async doReadFile(filename) {
    const url = "api/SampleData/ReadFile?filename="+filename;
    const d = await fetch(url);
    const data = await d.json();
    return data;
  }

  render() {
    return (
      <div>
        {renderForecastsTable(this.state.filecontent)}
      </div>
    )
  }
}

function renderForecastsTable(props) {
  return (
    <table className='table'>
      <thead>
        <tr>
          <th>File</th>
          <th>Read</th>
        </tr>
      </thead>
      <tbody>
        {props.map(forecast =>
          <tr key={forecast.filename}>
            <td>{forecast.filename}</td>
            <td>{forecast.str}</td>
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
