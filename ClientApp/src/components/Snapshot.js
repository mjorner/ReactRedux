import React, { Component } from 'react';
import { connect } from 'react-redux';
import './Snapshot.css';

class Snapshot extends Component {
    constructor(props) {
        super(props);
        document.title = this.props.app_title;
        this.state = { counter: 1 };
        this.reload = this.reload.bind(this);
    }

    componentDidMount() {
        this.props.setReloadHandler(this.reload);
    }

    reload() {
        var counter = this.state.counter;
        counter++;
        if (counter > 10) {
            counter = 1;
        }
        this.setState({counter: counter});
    }

    render() {
        const file_name = "images/"+this.props.snapshot_file_name;
        return (
            <div>
                <div className="pic_div">
                    <img src={file_name} alt="Snapshot" width="100%"></img>
                </div>
            </div>
        )
    }
}

export default connect(
)(Snapshot);