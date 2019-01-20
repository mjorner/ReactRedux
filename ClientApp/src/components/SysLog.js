import React, { Component } from 'react';
import { connect } from 'react-redux';
import './SysLog.css';

class SysLog extends Component {
    constructor(props) {
        super(props);
        this.state = { filecontent: null };
        document.title = this.props.app_title;
    }

    componentDidMount() {
        const url = "api/Data/ReadSysLog";
        fetch(url)
            .then(results => { return results.json(); })
            .then(data => {
                this.setState({ filecontent: data.text });
            });
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
                <pre className="syslog">
                    {this.state.filecontent}
                </pre>
            )
        }
    }
}

export default connect(
)(SysLog);