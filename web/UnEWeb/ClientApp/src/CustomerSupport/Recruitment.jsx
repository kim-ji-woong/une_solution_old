import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';

class Recruitment extends Component {
    static displayName = Recruitment.name;

    constructor(props) {
        super(props);
    }

    render() {
        return (
            <div>
                <h4>채용정보 페이지</h4>
            </div>
        );
    }
}

export default Recruitment;