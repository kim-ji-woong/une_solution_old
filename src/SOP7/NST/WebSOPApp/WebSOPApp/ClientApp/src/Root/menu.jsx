import * as React from 'react';
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';
import { withRouter } from 'react-router-dom';
import SDMS from '../SDMS/ui/sdms';

import Layout from './layout';
import RootResource from './resource/id';

/*interface Props {
    history: any,
    match: {
        isExact: boolean,
        params: object,
        path: string,
        url: string
    }
}

type NullableObject = object | null;

interface State {
    sdmsEvent: NullableObject
}*/

class Menu extends React.Component/*<Props, State>*/ {
    //public static pathSDMS: string = RootResource.path.sdms;
    
    constructor(props/*: Props*/) {
        super(props);

        this.state = {
            sdmsEvent: {}/*,
            sopSimulatorEvent: {},
            teamEditorEvent: {},
            sopManagerEvent: {},
            dashboardEvent: {},
            historyEvent: {}*/
        };
    }

    getTargetEvent()/*: [NullableObject, string]*/ {
        const pathName = window.location.pathname;

        if (pathName.length > 0) {
            const target = pathName.substring(1).toLowerCase();
            return [this.state.sdmsEvent, "sdms"];

             /*if (target === "sdms")
                return [this.state.sdmsEvent, target];*/
            /*else if (target === "sop-simulator")
                return [this.state.sopSimulatorEvent, target];
            else if (target === "team-editor")
                return [this.state.teamEditorEvent, target];
            else if (target === "sop-manager")
                return [this.state.sopManagerEvent, target];
            else if (target === "dashboard")
                return [this.state.dashboardEvent, target];
            else if (target === "history")
                return [this.state.historyEvent, target];*/
        }

        return [null, ""];
    }

    render() {
        const path = this.props.match.path;
        let title = "";

        /*if (path.indexOf(Menu.pathSOPSimulator) === 0) {
            title = Menu.titleSopSimulator;
        } else if (path.indexOf(Menu.pathTeamEditor) === 0) {
            title = Menu.titleTeamEditor;
        } else if (path.indexOf(Menu.pathSopManager) === 0) {
            title = Menu.titleSopManage;
        } else if (path.indexOf(Menu.pathDashboard) === 0) {
            title = Menu.titleDashboard;
        } else if (path.indexOf(Menu.pathHistory) === 0) {
            title = Menu.titleHistory;
        }*/

        const [targetEvent, target] = this.getTargetEvent();

        return (
            <Layout title={title} menuEvent={targetEvent} target={target}>
                <Route path={Menu.pathSDMS} render={() => <SDMS menuEvent={this.state.sdmsEvent} />} />
            </Layout>
        );
    }
}

export default withRouter(Menu);