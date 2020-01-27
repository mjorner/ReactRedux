import React, { Component } from 'react';
import { connect } from 'react-redux';
import './Stats.css';
import { authHeader } from '../helpers';

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
        const requestOptions = {
            headers: authHeader()
          };
        const d = await fetch(url, requestOptions);
        if (!d.ok) {
            this.props.history.push('/login')
            return;
        }
        const json = await d.json();
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