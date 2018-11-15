import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { actionCreators } from '../store/WeatherForecasts';
import Plot from 'react-plotly.js';

class FetchData extends Component {

  constructor(props) {
    super(props);
    this.state = { filecontent: null,};
  }

  async componentDidMount() {
    const url = "api/SampleData/GetFilenames"
    const data = await fetch(url);
    const json = await data.json();
    var i;
    var arr = [];
    for (i = 0; i < json.length; i++) {
      arr.push("Loading...");
    }
    this.setState({filecontent: arr});

    for (i = 0; i < json.length; i++) {
      this.doRenderGraphFromFile(json[i].csvFile, i);
    }
  }

  async doRenderGraphFromFile(filename, i) {
    const url = "api/SampleData/WeatherForecasts?filename="+filename;
    const d = await fetch(url);
    const data = await d.json();
    const filecontent = renderGraph(data, filename);
    const st = this.state.filecontent;
    st[i] = filecontent;
    this.setState({filecontent: st});
  }

  render() {
    if (this.state.filecontent === null) {
      return (
        <div>Loading data...</div>
      )
    } else {
      return (
        <table className='table'>
        <tbody>
          {this.state.filecontent.map((forecast, index) =>
            <tr key={index}>
              <td>{forecast}</td>
            </tr>
          )}
          </tbody>
        </table>
      )
    }
  }
}

function renderGraph(forecasts, filename) {
  return (
    <Plot
      data={[
        {type: 'scatter', line: {shape: 'spline'}, x: createDates(forecasts), y: createTemps(forecasts)},
      ]}
      layout={ {height: 400, title: filename} }
    />
  );
}

function createDates(forecasts) {
  var index;
  var dates = [];
  for (index = 0; index < forecasts.length; ++index) {
    var d = forecasts[index].dateTime
    dates.push(new Date(d));
  }
  return dates;
}

function createTemps(forecasts) {
  var index;
  var temps = [];
  for (index = 0; index < forecasts.length; ++index) {
    temps.push(forecasts[index].temperatureC);
  }
  return temps;
}

export default connect(
  state => state.weatherForecasts,
  dispatch => bindActionCreators(actionCreators, dispatch)
)(FetchData);
