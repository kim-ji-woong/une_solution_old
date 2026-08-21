import React, { Component } from 'react';
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';
import { withRouter } from 'react-router-dom';
import Layout from './layout';
import TypeMenu from '../FacilityType/ui/typeMenu'; 
import Main from '../FacilityType/ui/main'; 
import Setting from '../Account/ui/setting'; 
import Password from '../Account/ui/password'; 
import PwdFind from '../Account/ui/pwdFind'; 
import AlarmList from '../FacilityType/ui/alarmList'; 
import ManualList from '../FacilityType/ui/manualList'; 

class Menu extends Component {
    //static pathFacilityType = '/facilityType';
    static pathTypeMenu = '/typeMenu';
    static pathMain = '/main';
    static pathSetting = '/setting';
    static pathPassword = '/password';
    static pathPwdFind = '/pwdFind';
    static pathAlarmList = '/alarmList';
    static pathManualList = '/manualList';
    
    render() {
        // 로그인 체크 필요

        return (
            <Layout>
                <Route path={Menu.pathTypeMenu} component={TypeMenu} />
                <Route path={Menu.pathMain} component={Main} />
                <Route path={Menu.pathSetting} component={Setting} />
                <Route path={Menu.pathPassword} component={Password} />
                <Route path={Menu.pathPwdFind} component={PwdFind} />
                <Route path={Menu.pathAlarmList} component={AlarmList} />
                <Route path={Menu.pathManualList} component={ManualList} />
            </Layout>
        );
    }
}

export default withRouter(Menu);
