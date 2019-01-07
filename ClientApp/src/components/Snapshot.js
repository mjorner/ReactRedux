import React, { Component } from 'react';
import { connect } from 'react-redux';
import './Snapshot.css';

class Snapshot extends Component {
    constructor(props) {
        super(props);
        document.title = this.props.app_title;
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