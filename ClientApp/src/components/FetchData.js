import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { actionCreators } from '../store/WeatherForecasts';
import Plot from 'react-plotly.js';
import './FetchData.css';
import no_graph from '../../src/no_graph.png'
import Dropdown from 'react-dropdown'
import 'react-dropdown/style.css'

class FetchData extends Component {

  constructor(props) {
    super(props);
    this.state = { all_json: null, filecontent: null, dropdown_options: null, selected: null };
    this.onSelect = this.onSelect.bind(this)
  }

  async componentDidMount() {
    const url = "api/Data/GetFilenames"
    const data = await fetch(url);
    const json = await data.json();
    const set = new Set();
    for (var i = 0; i < json.length; i++) {
      this.setTypes(set, json[i].type)
    }
    var dropdown_values = Array.from(set);
    this.setState({all_json: json, dropdown_options: dropdown_values, selected: dropdown_values[0] });

    this.displayGraphs(json, dropdown_values[0]);
  }

  isCorrectType(json, type) {
    const t = json.type.split(',');
    for (var i = 0; i < t.length; i++) {
      if (t[i] === type) {
        return true;
      }
    }
    return false;
  }

  displayGraphs(all_json, type) {
    const json = all_json.filter(x=> this.isCorrectType(x, type))
    var arr = [];
    for (var i = 0; i < json.length; i++) {
      arr.push(this.no_graph());
    }
    this.setState({ filecontent: arr});
    for (var i = 0; i < json.length; i++) {
      this.doRenderGraphFromFile(json[i].csvFile, json[i].title, i, this.getTypeFileIndex(type));
    }
  }

  getTypeFileIndex(type) {
    if (type === "TEMP") {
      return 1;
    }
    else if (type === "HUM") {
      return 2;
    }
    else if (type === "PRES") {
      return 3;
    }
    return 1;
  }

  setTypes(set, types) {
    const t = types.split(',');
    for (var i = 0; i < t.length; i++) {
      set.add(t[i]);
    }
  }

  no_graph() {
    return (
      <img src={no_graph} alt="Loading..." />
    )
  }

  async doRenderGraphFromFile(filename, title, i, columnIndex) {
    const url = "api/Data/ReadGraphData?filename=" + filename + "&columnIndex=" + columnIndex;
    const data = await fetch(url);
    const json = await data.json();
    var SnappyJS = require('snappyjs');
    const buffer = Uint8Array.from(atob(json.base64Bytes), c => c.charCodeAt(0))
    const output = this.bin2String(SnappyJS.uncompress(buffer));
    const subJson = JSON.parse(output);
    const x = this.createDates(subJson);
    const y = this.createTemps(subJson);
    const avgs = this.calculateMovingAvg(y);
    const filecontent = this.renderGraph(x, y, avgs, title);
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

  renderGraph(xVals, yVals, avgs, graphTitle) {
    return (
      <Plot
        data={[
          { name: 'value', type: 'line', shape: 'spline',  x: xVals, y: yVals, line: { color: 'rgb(55, 128, 191)'} },
          { name: '10p avg', type: 'line', shape: 'spline', x: xVals, y: avgs, line: { color: 'rgb(128, 0, 128)'} }
        ]}
        layout={{ dragmode: 'turntable', height: 300, autosize: false, margin: {
          l: 50,
          r: 0,
          b: 50,
          t: 80,
          pad: 4
        }, title: graphTitle }}
      />
    );
  }

  calculateMovingAvg(y) {
    var temps = [];
    for (var i = 0; i < y.length; i++) {
      if (i > 9) {
        var sum = y[i] + y[i-1] + y[i-2] + y[i-3] + y[i-4] + y[i-5] + y[i-6] + y[i-7] + y[i-8] + y[i-9];
        temps.push(sum/10.0);
      }
      else {
        temps.push(y[i]);
      }
    }
    return temps;
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
      temps.push(data[index].Value);
    }
    return temps;
  }

  onSelect(option) {
    this.displayGraphs(this.state.all_json, option.value);
    this.setState({selected: option.value});
  }

  render() {
    if (this.state.filecontent === null) {
      return (
        <div></div>
      )
    } else {
      return (
        <div>
          <div className="drop_d">
            <Dropdown options={this.state.dropdown_options} onChange={this.onSelect} value={this.state.selected} placeholder="Select an option" />
          </div>
        <table>
          <tbody>
            {this.state.filecontent.map((graph, index) =>
              <tr key={index}>
                <td>{graph}</td>
              </tr>
            )}
          </tbody>
        </table>
        </div>
      )
    }
  }
}

export default connect(
  state => state.weatherForecasts,
  dispatch => bindActionCreators(actionCreators, dispatch)
)(FetchData);
