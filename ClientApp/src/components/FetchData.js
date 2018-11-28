import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { actionCreators } from '../store/WeatherForecasts';
import Plot from 'react-plotly.js';

class FetchData extends Component {

  constructor(props) {
    super(props);
    this.state = { filecontent: null };
  }

  async componentDidMount() {
    const url = "api/Data/GetFilenames"
    const data = await fetch(url);
    const json = await data.json();
    var i;
    var arr = [];
    for (i = 0; i < json.length; i++) {
      arr.push("Loading...");
    }
    this.setState({ filecontent: arr });

    for (i = 0; i < json.length; i++) {
      this.doRenderGraphFromFile(json[i].csvFile, json[i].title, i);
    }
  }

  async doRenderGraphFromFile(filename, title, i) {
    const url = "api/Data/ReadGraphData?filename=" + filename;
    const data = await fetch(url);
    const json = await data.json();
    var SnappyJS = require('snappyjs');
    const buffer = Uint8Array.from(atob(json.base64Bytes), c => c.charCodeAt(0))
    const output = this.bin2String(SnappyJS.uncompress(buffer));
    const filecontent = this.renderGraph(JSON.parse(output), title);
    const st = this.state.filecontent;
    st[i] = filecontent;
    this.setState({ filecontent: st });
  }

  bin2String(array) {
    var result = "";
    for (var i = 0; i < array.length; i++) {
      result += String.fromCharCode(array[i]);
    }
    return result;
  }

  renderGraph(data, filename) {
    return (
      <Plot
        data={[
          { type: 'scatter', line: { shape: 'spline' }, x: this.createDates(data), y: this.createTemps(data) },
        ]}
        layout={{ dragmode: 'turntable', height: 400, title: filename }}
      />
    );
  }

  createDates(data) {
    var index;
    var dates = [];
    for (index = 0; index < data.length; ++index) {
      var d = data[index].DateTime
      dates.push(new Date(d));
    }
    return dates;
  }

  createTemps(data) {
    var index;
    var temps = [];
    for (index = 0; index < data.length; ++index) {
      temps.push(data[index].TemperatureC);
    }
    return temps;
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
            {this.state.filecontent.map((graph, index) =>
              <tr key={index}>
                <td>{graph}</td>
              </tr>
            )}
          </tbody>
        </table>
      )
    }
  }
}

export default connect(
  state => state.weatherForecasts,
  dispatch => bindActionCreators(actionCreators, dispatch)
)(FetchData);
