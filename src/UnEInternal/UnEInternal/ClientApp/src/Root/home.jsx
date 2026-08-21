import React, { Component } from 'react';
import { Login } from '../Account/login';
import { AccountController } from './services/accountController';
import { Register } from '../Account/register';
import { VacationBody } from '../Vacation/vacationBody';

export class Home extends Component {
    constructor(props) {
        super(props);

        this.state = {
            newRegist: null
        }
    }

    onLogin = (id, pw) => {
        this.doLogin(id, pw);
    }

    async doLogin(id, pw) {
        const result = await AccountController.login(id, pw);

        if (result) {
            this.props.onLogin(result);
        }
        else {
            this.setState({ newRegist: { success: false, message: "로그인에 실패하였습니다." } });
        }
    }

    onRegist = (name, email) => {
        if (name && email) {
            this.doRegist(name, email);
        }
        else {
            this.setState({ newRegist: { success: null, message: null }});
        }
    }

    async doRegist(name, email) {
        const result = await AccountController.regist(name, email);

        if (result) {
            this.setState({ newRegist: { success: result.success, message: result.message } });
        }
        else {
            this.setState({ newRegist: { success: false, message: "비밀번호가 등록되지 않았습니다." } });
        }
    }

    render() {
        let login = null;
        let regist = null;

        const user = this.props.loginUser;

        if (!user) {
            if (this.state.newRegist) {
                regist = <Register onRegist={this.onRegist} response={this.state.newRegist} />;
            }
            else {
                login = <Login onLogin={this.onLogin} onRegist={this.onRegist} />;
            }
        }

        return (
            <div>
                {user && (<VacationBody loginUser={this.props.loginUser} options={this.props.options} onLogout={this.props.onLogout} />)}
                {login}
                {regist}
                {
                    /*<h1>Hello, world!</h1>
                    <p>Welcome to your new single-page application, built with:</p>
                    <ul>
                        <li><a href='https://get.asp.net/'>ASP.NET Core</a> and <a href='https://msdn.microsoft.com/en-us/library/67ef8sbd.aspx'>C#</a> for cross-platform server-side code</li>
                        <li><a href='https://facebook.github.io/react/'>React</a> for client-side code</li>
                        <li><a href='http://getbootstrap.com/'>Bootstrap</a> for layout and styling</li>
                    </ul>
                    <p>To help you get started, we have also set up:</p>
                    <ul>
                        <li><strong>Client-side navigation</strong>. For example, click <em>Counter</em> then <em>Back</em> to return here.</li>
                        <li><strong>Development server integration</strong>. In development mode, the development server from <code>create-react-app</code> runs in the background automatically, so your client-side resources are dynamically built on demand and the page refreshes when you modify any file.</li>
                        <li><strong>Efficient production builds</strong>. In production mode, development-time features are disabled, and your <code>dotnet publish</code> configuration produces minified, efficiently bundled JavaScript files.</li>
                    </ul>
                    <p>The <code>ClientApp</code> subdirectory is a standard React application based on the <code>create-react-app</code> template. If you open a command prompt in that directory, you can run <code>npm</code> commands such as <code>npm test</code> or <code>npm install</code>.</p>*/
                }
            </div>
        );
    }
}
