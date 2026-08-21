import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './layout';
import { Home } from './home';
//import { HomeMobile } from './homeMobile';
import { AccountController } from './services/accountController';
import './css/custom.css'
import { NavMenu } from './navMenu';
import { RegisterPassword } from '../Account/registerPassword';

export default class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            loginUser: null,
            loadingUser: true,
            loadingLink: true,
            links: [],
            loadingMessage: "데이터를 불러오고 있습니다.",
            currentMenu: NavMenu.Menu_Basic
        }
    }

    componentDidMount() {
        this.getCurrentUser();
        this.getLinks();
    }

    async getCurrentUser() {
        const result = await AccountController.currentUser();

        if (result?.success) {
            const user = {
                name: result.name,
                userID: result.userID,
                teamName: result.teamName,
                loginKey: result.loginKey
            };

            this.setState({ loginUser: user, loadingUser: false });
        }
        else {
            this.setState({ loginUser: null, loadingUser: false });
        }
    }

    async getLinks() {
        const [success, message, linkDatas] = await AccountController.getLinks();

        if (!success) {
            if (message !== null && message.length > 0) {
                alert(message);
            }

            this.setState({ loadingLink: false });
        }
        else {
            this.setState({ loadingLink: false, links: linkDatas });
        }
    }

    onSelectMenu = (menu) => {
        if (menu !== this.state.currentMenu) {
            this.setState({ currentMenu: menu });
        }
    }

    onLogin = (loginData) => {
        if (loginData?.success) {
            const user = {
                name: loginData.name,
                userID: loginData.userID,
                teamName: loginData.teamName,
                loginKey: loginData.loginKey
            };

            this.setState({ loginUser: user, currentMenu: NavMenu.Menu_TeamTree });
        }
        else {
            this.setState({ loginUser: null });
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
            this.setState({ loginUser: null, currentMenu: NavMenu.Menu_Basic });
        }
    }

    currentLocation() {
        return window.location.pathname;
    }


    render() {
        if (this.state.loadingUser || this.state.loadingLink) {
            return (
                <h2>{this.state.loadingMessage}</h2>
            );
        }

        if (!this.state.loginUser && this.currentLocation() !== '/Account/Regist') {
            return (
                <Layout loginUser={this.state.loginUser} onLogin={this.requestLogin} onLogout={this.requestLogout} currentMenu={this.state.currentMenu} onSelectMenu={this.onSelectMenu}>
                    <Route exact
                        render={() => <Home
                            url={this.state.url}
                            loginUser={this.state.loginUser}
                            onLogin={this.onLogin}
                            currentMenu={this.state.currentMenu}
                            links={this.state.links}
                        />}
                    />
                </Layout>
            )
        } else if (this.state.loginUser) {
            return (
                <Layout loginUser={this.state.loginUser} onLogin={this.requestLogin} onLogout={this.requestLogout} currentMenu={this.state.currentMenu} onSelectMenu={this.onSelectMenu}>
                    <Route exact
                        render={() => <Home
                            url={this.state.url}
                            loginUser={this.state.loginUser}
                            onLogin={this.onLogin}
                            currentMenu={this.state.currentMenu}
                            links={this.state.links}
                        />}
                    />
                </Layout>
            );
        }


        return (
            <Layout loginUser={this.state.loginUser} onLogin={this.requestLogin} onLogout={this.requestLogout} currentMenu={this.state.currentMenu} onSelectMenu={this.onSelectMenu}>
                <Route exact path='/'
                    render={() => <Home
                        url={this.state.url}
                        loginUser={this.state.loginUser}
                        onLogin={this.onLogin}
                        currentMenu={this.state.currentMenu}
                        links={this.state.links}
                    />}
                />
                <Route path='/Account/Regist'
                    render={() => <RegisterPassword url={this.state.url} onSelectMenu={this.onSelectMenu} />}
                />
            </Layout>
        );
    }

}
  
