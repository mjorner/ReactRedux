import React, { Component } from 'react';
import { connect } from 'react-redux';
import './Snapshot.css';
import { fetchJson } from '../helpers';

class Snapshot extends Component {
    constructor(props) {
        super(props);
        document.title = this.props.app_title;
        this.state = { dt: Date.now(), token: null };
        this.reload = this.reload.bind(this);
    }

    componentDidMount() {
        this.props.setReloadHandler(this.reload);
        this.reload();
    }

    async reload() {
        const url = "api/Data/GetSnapshotToken"
        const [ok, json] = await fetchJson(url, this.props.history);
        if (!ok) {
            return;
        }
        this.setState({dt: Date.now(), token: json.text});
    }

    render() {
        if (this.state.token === null) {
            return (
            <div></div>
          )
        }
        const file_name = "api/Data/Image?token="+this.state.token+"&dt="+this.state.dt;;
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