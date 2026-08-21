import React, { Component } from 'react';
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';
import { withRouter } from 'react-router-dom';
import SopSimulator from '../SOPSimulator/ui/sopSimulator';
import SDMS from '../SDMS/ui/sdms';
import TeamEditor from '../TeamEditor/ui/teamEditor';
import SopManager from '../SOPManager/ui/sopManager';
import Dashboard from '../Dashboard/ui/dashboardNew';
import History from '../History/ui/history';

import SessionString from '../Common/js/sessionString';
import LayoutSB from './layoutSB';

import uneCommon from '../Common/css/uneCommon.module.css';

import RootResource from './resource/id';
import { SDMSController } from '../SDMS/services/sdmsController';

class MenuSB extends Component {

    constructor(props) {
        super(props);

        this.state = {
            sdmsEvent: {},
            sopSimulatorEvent: {},
            teamEditorEvent: {},
            sopManagerEvent: {},
            dashboardEvent: {},
            historyEvent: {}
        };

        this.props = props;
        this.initSiteID();
    }

    async initSiteID() {
        //let siteID = RootResource.SiteID;

        //if (siteID === null || siteID === undefined) {
        //    // 사이트 ID 요청
        //    const [result, message] = await SDMSController.requestGetSiteID();

        //    if (result !== null && result !== undefined) {
        //        RootResource.SiteID = result;
        //    }

        //    siteID = RootResource.SiteID;

        //    if (window.localStorage.getItem(SessionString.Key.account + "_" + siteID.toString()) == null) {
        //        // 로그인 정보가 없으면 로그인 페이지로 이동
        //        this.props.history.push('/');
        //    }
        //}
        const user = await RootResource.initUserInfo();

        if (user === null || user === undefined) {
             // 로그인 정보가 없으면 로그인 페이지로 이동
                this.props.history.push('/');
        }

    }

    getTargetEvent() {
        const path = window.location.pathname;

        if (path.length > 0) {
            const target = path.substring(1).toLowerCase();

            if (path === RootResource.path.sdms)
                return [this.state.sdmsEvent, target];
            else if (path === RootResource.path.sopSimulator)
                return [this.state.sopSimulatorEvent, target];
            else if (path === RootResource.path.teamEditor)
                return [this.state.teamEditorEvent, target];
            else if (path === RootResource.path.sopManager)
                return [this.state.sopManagerEvent, target];
            else if (path === RootResource.path.dashboard)
                return [this.state.dashboardEvent, target];
            else if (path === RootResource.path.history)
                return [this.state.historyEvent, target];
        }

        return [null, ""];
    }

    render() {
        const path = this.props.match.path;

        const [targetEvent, target] = this.getTargetEvent();

        return (
            <LayoutSB menuEvent={targetEvent} target={target}>
                <Route path={RootResource.path.sopSimulator} render={() => <SopSimulator menuEvent={this.state.sopSimulatorEvent} />} />
                <Route path={RootResource.path.sdms} render={() => <SDMS menuEvent={this.state.sdmsEvent} />} />
                <Route path={RootResource.path.teamEditor} render={() => <TeamEditor menuEvent={this.state.teamEditorEvent} />} />
                <Route path={RootResource.path.sopManager} render={() => <SopManager className={uneCommon.paddingTop50} menu={SopManager.menu.editSOP} menuEvent={this.state.sopManagerEvent} />} />
                <Route path={RootResource.path.dashboard} render={() => <Dashboard menuEvent={this.state.teamEditorEvent} />} />
                <Route path={RootResource.path.history} render={() => <History menuEvent={this.state.historyEvent} />} />
            </LayoutSB>
        );
    }
}

export default withRouter(MenuSB);