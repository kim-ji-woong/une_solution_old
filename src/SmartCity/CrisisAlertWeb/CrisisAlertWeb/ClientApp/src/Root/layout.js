import React, { Component } from 'react';
import { Container } from 'reactstrap';
import Footer from './footer';


class Layout extends Component {

    render () {
        return (
            <div className="area auto">

                <Container>
                    {this.props.children}
                </Container>

                <Footer />
            </div>
        );
    }
}

export default Layout;
