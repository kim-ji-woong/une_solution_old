import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import SopSimulatorController from "../SOPSimulator/services/sopSimulatorController";
import RootResource from './resource/id';
import ProjectResource from './resource/id';
import newStyles from '../Common/css/newStyle.module.css';

class TitleBar extends Component {
    static pathSDMS = '/sdms';
    static pathSOPSimulator = '/sop-simulator';
    static pathSopManager = '/sop-manager';
    static pathTeamEditor = '/team-editor';
    static pathDashboard = '/dashboard';
    static pathHistory = '/history';

    static keys = [];
    static shortcutKey = null;

    constructor(props) {
        super(props);

        this.state = {
            popupOpen: false,
            settingOnOff: false,

            loading: true,
            reload: null,
        }

        this.props = props;
        this.initSiteID();
    }

    componentDidUpdate() {
        //console.log('componentDidUpdate');
    }

    componentWillMount() {
        //console.log('componentWillMount');
    }

    componentWillUpdate(nextProps, nextState) {
        //console.log('componentWillUpdate');
    }

    componentDidMount() {
        SopSimulatorController.StartWatchTimer();
    }

    async initSiteID() {
        const siteID = ProjectResource.SiteID;

        if (siteID === null || siteID === undefined) {
            // 사이트 ID 요청
            const [result, message, siteID] = await SopSimulatorController.requestSiteID();

            if (result === true) {
                ProjectResource.SiteID = siteID;
            }

            this.setState({ reload: true });
        }
    }

    getTitleNameUI() {
        const path = window.location.pathname;

        /*if (path === RootResource.path.sdms) {
            return (<></>);
        } else */if (path === RootResource.path.sopSimulator) {
            return (
                <div id={newStyles.hsTop}>
                    <h2 className={newStyles.hstTitle}>{RootResource.ID.title.sopSimulator}</h2>
                </div>);
        }/* else if (path === RootResource.path.teamEditor) {
            return (
                <div id={newStyles.hsTop}>
                    <h2 className={newStyles.hstTitle}>{RootResource.ID.title.teamEditor}</h2>
                </div>);
        } else if (path === RootResource.path.sopManager) {
            return (
                <div id={newStyles.hsTop}>
                    <h2 className={newStyles.hstTitle}>{RootResource.ID.title.sopManager}</h2>
                </div>);
        } else if (path === RootResource.path.dashboard) {
            return (
                <div id={newStyles.hsTop} className={dashboard.hsTop}>
                    <h2 className={newStyles.hstTitle}>{RootResource.ID.title.dashboard}</h2>
                </div>);
        } else if (path === RootResource.path.history) {
            return (
                <div id={newStyles.hsTop}>
                    <h2 className={newStyles.hstTitle}>{RootResource.ID.title.history}</h2>
                </div>);
        }*/
    }

    render() {
        return (
            <div>
                { this.getTitleNameUI()}
            </div>
        );
    }
}
export default withRouter(TitleBar);