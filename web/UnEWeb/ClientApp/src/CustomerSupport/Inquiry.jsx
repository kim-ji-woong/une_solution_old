import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';

class Inquiry extends Component {
    static displayName = Inquiry.name;

    constructor(props) {
        super(props);
    }

    render() {
        return (
            <div>
                <h4>문의 페이지</h4>
            </div>
        );
    }
}

export default Inquiry;