import React, { Component } from 'react';
import { connect } from 'react-redux';
import './SysLog.css';
import Dropdown from 'react-dropdown'
import 'react-dropdown/style.css'
import cookie from 'react-cookies'

class SysLog extends Component {
    constructor(props) {
        super(props);
        this.state = { filecontent: null, selected: null };
        this.onSelectType = this.onSelectType.bind(this);
        document.title = this.props.app_title;
    }

    componentDidMount() {
        var dropdown_values = this.props.log_files.split(";");
        var log_sel = cookie.load("log_sel");
        if (log_sel == null) {
            log_sel = dropdown_values[0];
        }
        this.showData(log_sel, dropdown_values);
    }

    showData(logfile, dropdown_values) {
        const url = "api/Data/ReadSysLog?filename="+logfile;
        fetch(url)
            .then(results => { return results.json(); })
            .then(data => {
                this.setState({ filecontent: data.text, selected: logfile, dropdown_options: dropdown_values });
            });
    }

    onSelectType(option) {
        this.showData(option.value, this.state.dropdown_options)
        cookie.save("log_sel", option.value, {path: "/"});
        this.setState({selected: option.value});
      }

    render() {
        if (this.state.filecontent === null) {
            return (
                <pre className="syslog">
                    Loading data...
                </pre>
            )
        } else {
            return (
                <div>
                  <div className="data_options">
                    <div className="drop_d">
                      <Dropdown options={this.state.dropdown_options} onChange={this.onSelectType} value={this.state.selected} placeholder="Select an option" />
                    </div>
                  </div>
                    <pre className="syslog">
                        {this.state.filecontent}
                    </pre>
                </div>
              )
        }
    }
}

export default connect(
)(SysLog);