import * as React from 'react';
import { Container } from 'reactstrap';
import { withRouter } from 'react-router-dom';

import rootStyles from './css/root.module.css';
import TitleBar from './titleBar';


/*interface Props {
    title: string,
    menuEvent: object,
    target: string,
    children: React.ReactNode
}*/

export default class Layout extends React.Component/*<Props>*/ {
    constructor(props/*: Props*/) {
        super(props);
    }

    render() {
        return (

            <main id="main" className={rootStyles.appWrap}>
                <TitleBar menuEvent={this.props.menuEvent} target={this.props.target} />
                
                <div className={rootStyles.container}>
                    {this.props.children}
                </div>
            </main>
        );
    }
/*    render() {
        return (
            <main id="main" className={rootStyles.appWrap} >
                <Container>
                    {this.props.children}
                </Container>
            </main>
        );
    }*/
}