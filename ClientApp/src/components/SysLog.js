import React, { Component } from 'react';
import { connect } from 'react-redux';
import './SysLog.css';
import Dropdown from 'react-dropdown'
import 'react-dropdown/style.css'
import cookie from 'react-cookies'
import { authHeader } from '../helpers';

class SysLog extends Component {
    constructor(props) {
        super(props);
        this.state = { filecontent: null, selected: null };
        this.onSelectType = this.onSelectType.bind(this);
        document.title = this.props.app_title;
        this.reload = this.reload.bind(this);
    }

    componentDidMount() {
        this.props.setReloadHandler(this.reload);
        this.reload();
    }

    reload() {
        var dropdown_values = this.props.log_files.split(";");
        var log_sel = cookie.load("log_sel");
        if (log_sel == null) {
            log_sel = dropdown_values[0];
        }
        this.showData(log_sel, dropdown_values);
    }

    async showData(logfile, dropdown_values) {
        const url = "api/Data/ReadSysLog?filename="+logfile;
        const requestOptions = {
            headers: authHeader()
          };
        const d = await fetch(url, requestOptions);
        if (!d.ok) {
            this.props.history.push('/login')
            return;
        }
        const json = await d.json();
        this.setState({ filecontent: json.text, selected: logfile, dropdown_options: dropdown_values });
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