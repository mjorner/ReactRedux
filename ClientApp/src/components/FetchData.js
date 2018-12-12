import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { actionCreators } from '../store/WeatherForecasts';
import Plot from 'react-plotly.js';
import './FetchData.css';
import no_graph from '../../src/no_graph.png'
import Dropdown from 'react-dropdown'
import 'react-dropdown/style.css'
import cookie from 'react-cookies'

class FetchData extends Component {

  constructor(props) {
    super(props);
    this.state = { all_json: null, filecontent: null, dropdown_options: null, selected: null, timePeriods: null, selected_time: null };
    this.onSelectType = this.onSelectType.bind(this);
    this.onSelectTime = this.onSelectTime.bind(this);
  }

  async componentDidMount() {
    const url = "api/Data/GetFilenames"
    const data = await fetch(url);
    var json = await data.json();
    const timePeriods = json.timePeriods;
    json = json.fileNames;
    const set = new Set();
    for (var i = 0; i < json.length; i++) {
      this.setTypes(set, json[i].type)
    }
    var dropdown_values = Array.from(set);

    var dev_sel = cookie.load("dev_sel");
    if (dev_sel == null) {
      dev_sel = dropdown_values[0];
    }
    
    var time_sel = cookie.load("time_sel");
    if (time_sel == null) {
      time_sel = timePeriods[0];
    }

    this.setState({all_json: json, dropdown_options: dropdown_values, selected: dev_sel, timePeriods: timePeriods, selected_time: time_sel });

    this.displayGraphs(json, dev_sel, time_sel);
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

  displayGraphs(all_json, type, timeSpan) {
    const json = all_json.filter(x=> this.isCorrectType(x, type))
    var arr = [];
    for (var i = 0; i < json.length; i++) {
      arr.push(this.no_graph());
    }
    this.setState({ filecontent: arr});
    for (i = 0; i < json.length; i++) {
      this.doRenderGraphFromFile(json[i].csvFile, json[i].title, i, this.getTypeFileIndex(type), timeSpan);
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

  async doRenderGraphFromFile(filename, title, i, columnIndex, timeSpan) {
    const url = "api/Data/ReadGraphData?filename=" + filename + "&columnIndex=" + columnIndex + "&timeSpan=" + timeSpan;
    const data = await fetch(url);
    const json = await data.json();
    var SnappyJS = require('snappyjs');
    const buffer = Uint8Array.from(atob(json.base64Bytes), c => c.charCodeAt(0))
    const output = this.bin2String(SnappyJS.uncompress(buffer));
    const subJson = JSON.parse(output);
    const x = this.createDates(subJson);
    const y = this.createValues(subJson);
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
    var values = [];
    for (var i = 0; i < y.length; i++) {
      if (i > 9) {
        var sum = y[i] + y[i-1] + y[i-2] + y[i-3] + y[i-4] + y[i-5] + y[i-6] + y[i-7] + y[i-8] + y[i-9];
        values.push(sum/10.0);
      }
      else {
        values.push(y[i]);
      }
    }
    return values;
  }

  createDates(data) {
    var dates = [];
    for (var index = 0; index < data.length; ++index) {
      var d = data[index].DateTime
      dates.push(new Date(d));
    }
    return dates;
  }

  createValues(data) {
    var values = [];
    for (var index = 0; index < data.length; ++index) {
      values.push(data[index].Value);
    }
    return values;
  }

  onSelectType(option) {
    this.displayGraphs(this.state.all_json, option.value, this.state.selected_time);
    cookie.save("dev_sel", option.value, {path: "/"});
    this.setState({selected: option.value});
  }

  onSelectTime(option) {
    this.displayGraphs(this.state.all_json, this.state.selected, option.value);
    cookie.save("time_sel", option.value, {path: "/"});
    this.setState({selected_time: option.value});
  }

  render() {
    if (this.state.filecontent === null) {
      return (
        <div></div>
      )
    } else {
      return (
        <div>
          <div className="data_options">
            <div className="drop_d">
              <Dropdown options={this.state.dropdown_options} onChange={this.onSelectType} value={this.state.selected} placeholder="Select an option" />
            </div>
            <div className="drop_d">
              <Dropdown options={this.state.timePeriods} onChange={this.onSelectTime} value={this.state.selected_time} placeholder="Select an option" />
            </div>
          </div>
        <div className="table_div">
        <table>
          <tbody>
            {this.state.filecontent.map((graph, index) =>
              <tr key={index}>
                <td className="td_row">{graph}</td>
              </tr>
            )}
          </tbody>
        </table>
        </div>
        </div>
      )
    }
  }
}

export default connect(
  state => state.weatherForecasts,
  dispatch => bindActionCreators(actionCreators, dispatch)
)(FetchData);
