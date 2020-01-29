import React, { Component } from 'react';
import { connect } from 'react-redux';
import './Stats.css';
import { fetchJson } from '../helpers';

class Stats extends Component {
    constructor(props) {
        super(props);
        this.state = { filecontent: null };
        document.title = this.props.app_title;
        this.reload = this.reload.bind(this);
    }

    componentDidMount() {
        this.props.setReloadHandler(this.reload);
        this.reload();
    }

    async reload() {
        const url = "api/Data/ReadTextFile?filename=s.stat";
        const [ok, json] = await fetchJson(url, this.props.history);
        if (!ok) {
            return;
        }
        this.setState({ filecontent: json.text });
    }

    render() {
        if (this.state.filecontent === null) {
            return (
                <pre>Loading data...</pre>
            )
        } else {
            return (
                <pre>
                    {this.state.filecontent}
                </pre>
            )
        }
    }
}

export default connect(
)(Stats);