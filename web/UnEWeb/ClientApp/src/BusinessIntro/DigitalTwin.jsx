import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';

class DigitalTwin extends Component {
    static displayName = DigitalTwin.name;

    constructor(props) {
        super(props);
    }

    render() {
        return (
            <div>
                <h4>디지털트윈 페이지</h4>
            </div>
        );
    }
}

export default DigitalTwin;