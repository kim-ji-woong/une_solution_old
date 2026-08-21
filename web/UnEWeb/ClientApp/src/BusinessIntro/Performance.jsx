import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';

class Performance extends Component {
    static displayName = Performance.name;

    constructor(props) {
        super(props);
    }

    render() {
        return (
            <div>
                <h4>주요실적 페이지</h4>
            </div>
        );
    }
}

export default Performance;