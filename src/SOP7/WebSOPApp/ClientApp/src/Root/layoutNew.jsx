import React, { Component } from 'react';
import { Container } from 'reactstrap';

import TitleBar from './titleBar';

class LayoutNew extends Component {
    constructor(props) {
        super(props);

        this.props = props;
    }

    render() {
        return (
            <div id="subPage">

                <TitleBar title={this.props.title} />

                <Container>
                    {this.props.children}
                </Container>

            </div>
        );
    }
}

export default LayoutNew;