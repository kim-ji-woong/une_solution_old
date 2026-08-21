import React, { Component } from 'react';
import { Container } from 'reactstrap';
import $ from 'jquery';

import uneCommon from '../Common/css/uneCommon.module.css';

import TitleBar from './titleBar';
import uis from '../Common/css/ui.module.css';

import RootResource from './resource/id';

class Layout extends Component {
    constructor(props) {
        super(props);

        this.props = props;
    }

    componentDidMount() {
        const path = window.location.pathname;

        // 경로에 따라 타이틀바 css 변경
        /*if (path.indexOf(RootResource.path.sdms) === 0) {
            $('#layoutContainer').removeClass(uneCommon.paddingTop50);
        } else */if (path.indexOf(RootResource.path.sopSimulator) === 0) {
            $('#layoutContainer').addClass(uneCommon.paddingTop50);
        }/* else if (path.indexOf(RootResource.path.teamEditor) === 0) {
            $('#layoutContainer').addClass(uneCommon.paddingTop50);
        } else if (path.indexOf(RootResource.path.sopManager) === 0) {
            $('#layoutContainer').addClass(uneCommon.paddingTop50);
        } else if (path.indexOf(RootResource.path.dashboard) === 0) {
            $('#layoutContainer').addClass(uneCommon.paddingTop50);
        } else if (path.indexOf(RootResource.path.history) === 0) {
            $('#layoutContainer').addClass(uneCommon.paddingTop50);
        }*/
    }

    render() {
        return (
            <main id="layoutMain" className={uis.appWrap}>
                <TitleBar menuEvent={this.props.menuEvent} target={this.props.target} />

                <Container id="layoutContainer">
                    {this.props.children}
                </Container>

            </main>
        );
    }
}

export default Layout;