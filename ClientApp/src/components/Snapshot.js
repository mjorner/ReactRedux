import React, { Component } from 'react';
import { connect } from 'react-redux';
import './Snapshot.css';

class Snapshot extends Component {
    constructor(props) {
        super(props);
        document.title = this.props.app_title;
        this.state = { dt: Date.now() };
        this.reload = this.reload.bind(this);
    }

    componentDidMount() {
        this.props.setReloadHandler(this.reload);
        this.reload();
    }

    reload() {
        this.setState({dt: Date.now()});
    }

    render() {
        const file_name = "images/"+this.props.snapshot_file_name+"?"+this.state.dt;
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