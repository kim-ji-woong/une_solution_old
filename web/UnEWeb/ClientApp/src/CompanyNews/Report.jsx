import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';

class Report extends Component {
    static displayName = Report.name;

    constructor(props) {
        super(props);
    }

    render() {
        return (
            <div>
                <h4>언론보도 페이지</h4>
            </div>
        );
    }
}

export default Report;