import React, { Component } from 'react';
import { Container } from 'reactstrap';
import $ from 'jquery';

import './css/layout.css';      // LG 화학 관련 추가 CSS
import uneCommon from '../Common/css/uneCommon.module.css';

import TitleBarSB from './titleBarSB';
import uis from '../Common/css/ui.module.css';

import RootResource from './resource/id';

class LayoutSB extends Component {
    constructor(props) {
        super(props);

        this.props = props;
    }

    componentDidMount() {
        const path = window.location.pathname;

        // 경로에 따라 타이틀바 css 변경
        if (path.indexOf(RootResource.path.sdms) === 0) {
            $('#layoutContainer').removeClass(uneCommon.paddingTop50);
        } else if (path.indexOf(RootResource.path.sopSimulator) === 0) {
            $('#layoutContainer').addClass(uneCommon.paddingTop50);
        } else if (path.indexOf(RootResource.path.teamEditor) === 0) {
            $('#layoutContainer').addClass(uneCommon.paddingTop50);
        } else if (path.indexOf(RootResource.path.sopManager) === 0) {
            $('#layoutContainer').addClass(uneCommon.paddingTop50);
        } else if (path.indexOf(RootResource.path.dashboard) === 0) {
            $('#layoutContainer').addClass(uneCommon.paddingTop50);
        } else if (path.indexOf(RootResource.path.history) === 0) {
            $('#layoutContainer').addClass(uneCommon.paddingTop50);
        }
    }

    render() {
        return (
            <main id="mainSB" className={uis.appWrap} >
                <TitleBarSB menuEvent={this.props.menuEvent} target={this.props.target} />

                <Container id="layoutContainer">
                    {this.props.children}
                </Container>

            </main>
        );
    }
}

export default LayoutSB;