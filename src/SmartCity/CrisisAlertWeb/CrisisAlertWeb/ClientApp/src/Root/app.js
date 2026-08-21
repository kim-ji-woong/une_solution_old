import React, { Component } from 'react';
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';
import LoginPage from '../Account/ui/loginPage';
import Menu from './menu';
import SensorSearch from '../FacilityType/ui/sensorSearch';

export default class App extends Component {

    render () {
        return (
            <Router>
                <Switch>
                    <Route exact path='/' component={LoginPage} />
                    <Route path={SensorSearch.pathSensorSearch} component={SensorSearch} />
                    <Route path={Menu.pathFacilityType} component={Menu} />
                    <Route path={Menu.pathMain} component={Menu} />
                    <Route path={Menu.pathSetting} component={Menu} />
                    <Route path={Menu.pathPassword} component={Menu} />
                    <Route path={Menu.pathPwdFind} component={Menu} />
                    <Route path={Menu.pathAlarmList} component={Menu} />
                    <Route path={Menu.pathManualList} component={Menu} />
                </Switch>
            </Router>
        );
    }
}
