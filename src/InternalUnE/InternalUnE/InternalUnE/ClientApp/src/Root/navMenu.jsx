import React, { Component } from 'react';
import './css/navMenu.css';

export class NavMenu extends Component {
    static Menu_Basic = 0;
    static Menu_Login = 1;
    static Menu_TeamTree = 2;

    constructor (props) {
        super(props);

        this.toggleNavbar = this.toggleNavbar.bind(this);
        this.state = {
            collapsed: true
        };
    }

    toggleNavbar () {
        this.setState({
            collapsed: !this.state.collapsed
        });
    }

    processLogin = () => {
        if (this.props.loginUser) {
            this.props.onLogout(this.props.loginUser);
        }
        else {
            this.props.onSelectMenu(NavMenu.Menu_Login);
            //this.props.onLogin();
        }
    }

    getItems() {
        let loginMenu = "로그인", menuText = "메인화면";
        let menu = NavMenu.Menu_Basic;

        if (this.props.loginUser) {
            loginMenu = this.props.loginUser.name + "님 로그아웃";

            if (this.props.currentMenu === NavMenu.Menu_Basic) {
                menuText = "조직도 보기";
                menu = NavMenu.Menu_TeamTree;
            }
            else if (this.props.currentMenu === NavMenu.Menu_Login ||
                this.props.currentMenu === NavMenu.Menu_TeamTree) {
                menuText = "메인화면";
                menu = NavMenu.Menu_Basic;
            }
        }

        return [loginMenu, menu, menuText];
    }

    render() {
        const [loginMenu, menu, menuText] = this.getItems();

        return (
            /* <header>
                <div className="topBar box-shadow">
                    <div className="topBarLeft">
                        <div className="topBarArea">
                            <a className="navbar-brand" href="/">유엔이 임직원들을 위한 공간</a>
                        </div>
                    </div>
                    <div className="topBarRight">
                        <div className="topBarArea">
                            <div className="loginBoxArea">
                                <span onClick={() => this.processLogin()}>{loginMenu}</span>
                                <span className="menuButton" onClick={() => this.props.onSelectMenu(menu)}>{menuText}</span>
                            </div>
                        </div>
                    </div>
                </div>
            </header> */

            <header>
                <div className="topBarBox">
                    <div className="topBarLeft">
                        <div>
                            <a className="navbar-brand" href="/">유엔이 임직원들을 위한 공간</a>
                        </div>
                    </div>
                    <div className="topBarRight">
                        <div>
                            <div className="loginBoxArea">
                                <span onClick={() => this.processLogin()}>{loginMenu}</span>
                                <span className="menuButton" onClick={() => this.props.onSelectMenu(menu)}>{menuText}</span>
                            </div>
                        </div>
                    </div>
                </div>
            </header>
        );
    }
}
