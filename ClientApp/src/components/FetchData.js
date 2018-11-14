import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { actionCreators } from '../store/WeatherForecasts';
import Plot from 'react-plotly.js';


class FetchData extends Component {

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
          return await this.doRenderGraphFromFile(file.csvFile);
        })
        const all = await Promise.all(allGraphs);
        this.setState({filecontent: all});
      });
    }

    async doRenderGraphFromFile(filename) {
      const url = "api/SampleData/WeatherForecasts?filename="+filename;
      const d = await fetch(url);
      const data = await d.json();
      const filecontent = renderGraph(data, filename);
      return filecontent;
    }

  render() {
    return (
      <div>{this.state.filecontent}</div>
    )
  }
}

function renderGraph(forecasts, filename) {
  return (
    <Plot
      data={[
        {type: 'scatter', line: {shape: 'spline'}, x: createDates(forecasts), y: createTemps(forecasts)},
      ]}
      layout={ {width: 500, height: 300, title: filename} }
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
