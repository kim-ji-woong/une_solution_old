import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './layout';
import { VacationBody } from '../Vacation/vacationBody';
import { Home } from './home';
import { Teams } from './teams';
import { RegisterPassword } from '../Account/registerPassword';
import { AccountController } from './services/accountController';

/* import { VacationMenus } from '../Vacation/vacationMenus'; */   /* 0414 */
/* import { VacationContents } from '../Vacation/vacationContents'; */   /* 0414 */

import './css/custom.css'

export default class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            loginUser: null,
            options: null,
            loading: true,
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
            this.setState({ loginUser: result, loading: false });
        }
        else {
            this.setState({ loginUser: null, loading: false });
        }
    }

    onLogin = (loginData) => {
        if (loginData?.success) {
            this.setState({ loginUser: loginData.user, options: loginData.options });
        }
        else {
            this.setState({ loginUser: null });
        }
    }

    onAutoLogin(loginData, beginCode) {
        if (loginData?.success) {
            this.setState({ loginUser: loginData.user, options: loginData.options, loading: false, prevBeginCode: beginCode });
        }
        else {
            this.setState({ loginUser: null, loading: false, prevBeginCode: beginCode });
        }
    }

    requestLogin = () => {
        this.setState({ loginUser: null });
    }

    requestLogout = (user) => {
        this.doLogout(user);
    }

    async doLogout(user) {
        const result = await AccountController.logout(user.userID);

        if (result?.success) {
            this.setState({ loginUser: null, options: null });
        }
    }

    currentLocation() {
        return window.location.pathname;
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

    render() {
        if (this.state.loading || this.checkAutoLogin()) {
            return (
                <h2>{this.state.loadingMessage}</h2>
            );
        }

        if (!this.state.loginUser && this.currentLocation() !== '/Account/Regist') {
            return (
                <Layout loginUser={this.state.loginUser} options={this.state.options} onLogin={this.requestLogin} onLogout={this.requestLogout}>
                    <Route exact
                        render={() => <Home
                            loginUser={this.state.loginUser}
                            options={this.state.options}
                            onLogin={this.onLogin} />}
                    />
                </Layout>
                );
        }

        return (
            <Layout loginUser={this.state.loginUser} options={this.state.options} onLogin={this.requestLogin} onLogout={this.requestLogout}>
                <Route exact path='/'
                    render={() => <Home
                        loginUser={this.state.loginUser}
                        options={this.state.options} onLogout={this.requestLogout}  />}
                />
                {/* <Route path='/vacation' render={() => <VacationBody
                    loginUser={this.state.loginUser}
                    options={this.state.options} />} /> */}
                <Route path='/vacation' render={() => <VacationBody
                    loginUser={this.state.loginUser}
                    options={this.state.options}
                    onLogin={this.props.onLogin}
                    onLogout={this.requestLogout}  />}
                />
                <Route path='/teams' render={() => <Teams
                    loginUser={this.state.loginUser}
                    onLogin={this.props.onLogin} 
                    onLogout={this.requestLogout}
                />} />
                <Route path='/Account/Regist' component={RegisterPassword} />
             </Layout>


            /* <div className="bodyArea">
                <VacationMenus loginUser={this.props.loginUser} options={this.props.options} managerRequest={this.state.managerRequest} onSelectMenu={this.onSelectMenu} selectedMenu={this.state.selectedMenu} />
                <VacationContents loginUser={this.props.loginUser} options={this.props.options} history={this.state.history} managerRequest={this.state.managerRequest} membersHistory={this.state.membersHistory} onSelectMenu={this.onSelectMenu} selectedMenu={this.state.selectedMenu} addVacationHistory={this.addVacationHistory} removeRequest={this.onRemoveRequest} updateHistory={this.updateHistory} getNextYearHistory={this.getNextYearHistory} getLastYearHistory={this.getLastYearHistory} />
            </div> */
        );
    }
}
