import React, { Component } from 'react';
import { connect } from 'react-redux';
import './Stats.css';

class Stats extends Component {
    constructor(props) {
        super(props);
        this.state = { filecontent: null };
        document.title = this.props.app_title;
    }

    componentDidMount() {
        const url = "api/Data/ReadTextFile?filename=s.stat";
        fetch(url)
            .then(results => { return results.json(); })
            .then(data => {
                this.setState({ filecontent: data.text });
            });
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