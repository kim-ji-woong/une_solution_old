import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './layout';
import { Gate } from './gate';
import { AccountController } from '../Account/services/accountController';

import './css/custom.css'
import { NavMenu } from './navMenu';
import { SpaceBody } from '../Space/spaceBody';
import { MemberBody } from '../Member/memberBody';

export default class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            loginData: null,
            loading: true,
            menu: {
                item: NavMenu.Menu_Login,
                param: NavMenu.SubMenu_None
            },
            loadingMessage: "데이터를 불러오고 있습니다.",
            prevBeginCode: null
        }
    }

    componentDidMount() {
        this.getCurrentUser();
    }

    async getCurrentUser() {
        const result = await AccountController.currentUser();

        if (result) {
            this.setState({ loginData: result, loading: false });
        }
        else {
            this.setState({ loginData: null, loading: false });
        }
    }

    onLogin = (loginData) => {
        if (loginData?.success) {
            this.setState({ loginData: loginData });
        }
        else {
            this.setState({ loginData: null });
        }
    }

    requestLogin = () => {
        this.setState({ loginData: null, menu: { item: NavMenu.Menu_Login, param: NavMenu.SubMenu_None } });
    }

    requestLogout = (user) => {
        this.doLogout(user);
    }

    async doLogout(user) {
        const result = await AccountController.logout(user.userID);

        if (result?.success) {
            this.setState({ loginData: null });
        }
    }

    checkAutoLogin() {
        const parameters = window.location.search;

        if (parameters.length > 0) {
            return this.processBeginCode(parameters);
        }

        return false;
    }

    processBeginCode(parameters) {
        if (!parameters || parameters.length === 0) {
            return false;
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

            if (paramName.toLowerCase() === "bc") {
                const beginCode = paramValue;

                if (beginCode !== null && beginCode !== undefined && beginCode !== this.state.prevBeginCode) {
                    this.autoLogin(beginCode);
                    return true;
                }
            }
        }

        return false;
    }

    async autoLogin(beginCode) {
        this.setState({ loading: true });

        const result = await AccountController.autoLogin(beginCode);
        this.onAutoLogin(result, beginCode);
        window.history.pushState(null, null, window.location.origin);
    }

    onAutoLogin(loginData, beginCode) {
        if (loginData?.success) {
            this.setState({ loginData: loginData, loading: false, prevBeginCode: beginCode });
        }
        else {
            this.setState({ loginData: null, loading: false, prevBeginCode: beginCode });
        }
    }

    onChangeMenu = (menu, param) => {
        if (this.state.menu.item !== menu || this.state.menu.param !== param) {
            this.setState({ menu: { item: menu, param: param } });
        }
    }

    currentLocation() {
        return window.location.pathname;
    }

    render() {
        if (this.state.loading || this.checkAutoLogin()) {
            return (
                <h2>{this.state.loadingMessage}</h2>
            );
        }

        if (!this.state.loginData && this.currentLocation() !== '/Account/Regist') {
            return (
                <Layout loginData={this.state.loginData} menu={this.state.menu} onLogin={this.requestLogin} onLogout={this.requestLogout}>
                    <Route exact
                        render={() => <Gate
                            loginUser={this.state.loginData}
                            menu={this.state.menu}
                            onLogin={this.onLogin}
                            changeMenu={this.onChangeMenu} />}
                    />
                </Layout>
            );
        }

        return (
            <Layout loginData={this.state.loginData} menu={this.state.menu} onLogin={this.requestLogin} onLogout={this.requestLogout}>
                <Route exact path='/'
                    render={() => <Gate
                        loginUser={this.state.loginData}
                        menu={this.state.menu}
                        onLogin={this.onLogin}
                        changeMenu={this.onChangeMenu} />}
                />
                <Route path='/space' render={() => <SpaceBody loginData={this.state.loginData} />} />
                <Route path='/member' render={() => <MemberBody loginData={this.state.loginData} />} />
            </Layout>
        );
    }
}
