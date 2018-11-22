import React, { Component } from 'react';
import { connect } from 'react-redux';

class Stats extends Component {
    constructor(props) {
        super(props);
        this.state = { filecontent: null };
    }

    componentDidMount() {
        const url = "api/Data/ReadTextFile?filename=s.stat";
        fetch(url)
            .then(results => { return results.json(); })
            .then(data => {
                const lines = data.text.split('\n');
                var i;
                var joined = ""
                for (i = 0; i < lines.length; i++) {
                    if (i === lines.length - 1) {
                        joined += lines[i];
                    }
                    else {
                        joined += lines[i] + '\n';
                    }
                }
                this.setState({ filecontent: joined });
            });
    }

    render() {
        if (this.state.filecontent === null) {
            return (
                <code>Loading data...</code>
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