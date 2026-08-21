import React, { Component } from 'react';
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';
import { withRouter } from 'react-router-dom';
import SopSimulator from '../SOPSimulator/ui/sopSimulator';
import Layout from './layout';

import uneCommon from '../Common/css/uneCommon.module.css';

import RootResource from './resource/id';
import SopSimulatorController from '../SOPSimulator/services/sopSimulatorController';
import ProjectResource from './resource/id';

class Menu extends Component {

    constructor(props) {
        super(props);

        this.state = {
            sopSimulatorEvent: {}
        };

        this.props = props;
    }

    componentDidMount() {
    }

    getTargetEvent() {
        const path = window.location.pathname;

        if (path.length > 0) {
            const target = path.substring(1).toLowerCase();

            /*if (path === RootResource.path.sdms)
                return [this.state.sdmsEvent, target];
            else */if (path === RootResource.path.sopSimulator) {
                this.processBeginCode(window.location.search);
                return [this.state.sopSimulatorEvent, target];
            }
            /*else if (path === RootResource.path.teamEditor)
                return [this.state.teamEditorEvent, target];
            else if (path === RootResource.path.sopManager)
                return [this.state.sopManagerEvent, target];
            else if (path === RootResource.path.dashboard)
                return [this.state.dashboardEvent, target];
            else if (path === RootResource.path.history)
                return [this.state.historyEvent, target];*/
        }

        return [null, ""];
    }

    processBeginCode(parameters) {
        if (!parameters || parameters.length === 0) {
            return;
        }

        parameters = parameters.substring(1).trim();

        const params = parameters.split('&');
        const paramCount = params.length;

        for (let i = 0; i < paramCount; i++) {
            const datas = params[i].split('=');

            if (datas.length !== 2) {
                continue;
            }

            const paramName = datas[0].trim();
            const paramValue = datas[1].trim();

            if (paramName.toLowerCase() === "begincode") {
                const beginCode = parseInt(paramValue);

                if (beginCode !== null && beginCode !== undefined && beginCode != NaN) {
                    this.runSOP(beginCode);
                    break;
                }
            }
        }
    }

    async runSOP(beginCode) {
        const [success, message, actionStepHistoryID, accessMode, accessToken, serviceType, siteID] = await SopSimulatorController.runSOP(beginCode);

        if (success) {
            ProjectResource.setSOPParams(actionStepHistoryID, accessMode, accessToken, serviceType, siteID);
        }
        else {
            alert(message);
        }
    }

    render() {
        const path = this.props.match.path;
        const [targetEvent, target] = this.getTargetEvent();

        return (
            <Layout menuEvent={targetEvent} target={target}>
                <Route path={RootResource.path.sopSimulator} render={() => <SopSimulator menuEvent={this.state.sopSimulatorEvent} />} />
            </Layout>
        );
    }
}

export default withRouter(Menu);