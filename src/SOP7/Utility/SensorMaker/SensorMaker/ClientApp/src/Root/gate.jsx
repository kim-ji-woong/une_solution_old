import React, { Component } from 'react';
import { Login } from '../Account/login';
import { AccountController } from '../Account/services/accountController';
import { Register } from '../Account/register';
import { NavMenu } from './navMenu';
import { SpaceBody } from '../Space/spaceBody';

export class Gate extends Component {
    constructor(props) {
        super(props);

        this.state = {
            /*newRegist: false,*/
            response: null
        }
    }

    onLogin = (id, pw) => {
        this.doLogin(id, pw);
    }

    async doLogin(id, pw) {
        const result = await AccountController.login(id, pw);

        if (result?.success) {
            this.props.onLogin(result);
        }
        else {
            const message = result?.message && result?.message.length > 0 ? result.message : "로그인에 실패하였습니다.";
            this.setState({ response: { success: false, message: message } });

            alert(message);
            this.props.changeMenu(this.props.menu.item, NavMenu.SubMenu_None);
            //this.setState({ newRegist: false, response: { success: false, message: "로그인에 실패하였습니다." } });
        }
    }

    onRegist = (name, email, phoneNumber, password) => {
        if (name && email && phoneNumber) {
            this.doRegist(name, email, phoneNumber, password);
        }
        else {
            this.setState({ response: { success: null, message: null } });
            this.props.changeMenu(NavMenu.Menu_Login, NavMenu.SubMenu_NewRegist);
            //this.setState({ newRegist: true, response: { success: null, message: null }});
        }
    }

    async doRegist(name, email, phoneNumber, password) {
        const result = await AccountController.regist(name, email, phoneNumber, password);

        if (result) {
            if (result.success) {
                alert(result.message);
                this.setState({ response: null });
                this.props.changeMenu(this.props.menu.item, NavMenu.SubMenu_None);
                //this.setState({ newRegist: false, response: null });
            }
            else {
                this.setState({ response: { success: result.success, message: result.message } });
                this.props.changeMenu(NavMenu.Menu_Login, NavMenu.SubMenu_NewRegist);
                //this.setState({ newRegist: true, response: { success: result.success, message: result.message } });
            }
        }
        else {
            this.setState({ response: { success: false, message: "서버에 접속할 수 없습니다." } });
            this.props.changeMenu(this.props.menu.item, NavMenu.SubMenu_None);
            //this.setState({ newRegist: true, response: { success: false, message: result.message } });
        }
    }

    isNewRegist() {
        return this.props.menu.item === NavMenu.Menu_Login && this.props.menu.param === NavMenu.SubMenu_NewRegist;
    }

    render() {
        let login = null;
        let regist = null;

        const user = this.props.loginUser;

        if (!user) {
            if (this.isNewRegist()) {
                regist = <Register onRegist={this.onRegist} response={this.state.response} />;
            }
            else {
                login = <Login onLogin={this.onLogin} onRegist={this.onRegist} />;
            }
        }

        return (
            <div>
                {user && (<SpaceBody loginData={this.props.loginUser} />)}
                {login}
                {regist}
            </div>
        );
    }
}
