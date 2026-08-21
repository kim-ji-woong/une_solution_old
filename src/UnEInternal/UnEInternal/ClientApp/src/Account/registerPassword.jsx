import React, { Component } from 'react';
import styles from './css/account.module.css';
import { AccountController } from '../Root/services/accountController';

export class RegisterPassword extends Component {
    constructor(props) {
        super(props);

        this.state = {
            errors: null,
            loading: true,
            loadingMessage: "데이터를 불러오고 있습니다.",
            params: null
        }

        this.refPassword = React.createRef();
        this.refPasswordConfirm = React.createRef();
    }

    componentDidMount() {
        this.checkValidation();
    }

    async checkValidation() {
        const search = window.location.search.substring(1);

        if (!search) {
            return;
        }

        const params = search.split('&');

        for (let i = 0; i < params.length; i++) {
            const param = params[i];
            const tokens = param.split('=');

            if (tokens.length !== 2) {
                continue;
            }

            const name = tokens[0].trim();
            const value = tokens[1].trim();

            if (name === "code") {
                const result = await AccountController.checkRegistParams(value);
                this.setState({ errors: null, loading: false, params: result });
                break;
            }
        }
    }

    onClick = () => {
        if (this.state.params?.success) {
            const pw = this.refPassword.current.value.toString().trim();
            let passwordError = null, confirmError = null;

            if (pw.length === 0) {
                passwordError = "비밀번호를 입력하세요.";
            }

            const confirm = this.refPasswordConfirm.current.value.toString().trim();

            if (confirm.length === 0) {
                confirmError = "비밀번호를 한번더 입력하세요.";
            }

            if (pw.length > 0 && confirm.length > 0 && pw !== confirm) {
                confirmError = "비밀번호가 일치하지 않습니다.";
            }

            if (passwordError || confirmError) {
                this.setErrorMessage(passwordError, confirmError);
            }
            else {
                this.setPassword(this.state.params.userID, pw);
            }
        }
    }

    onKeyUp(event) {
        if (event.key === 'Enter') {
            this.onClick();
        }
    }

    async setPassword(id, pw) {
        this.setState(
            {
                errors: null,
                loading: true,
                loadingMessage: "처리중입니다.",
                params: this.state.params
            }
        );

        const result = await AccountController.setPassword(this.state.params.userID, pw);

        if (result?.success) {
            /*const params = { ...this.state.params };
            params.success = result.success;
            params.message = "비밀번호가 등록되었습니다.\r\n로그인 화면으로 이동합니다.";*/

            this.setState(
                {
                    errors: null,
                    loading: false,
                    loadingMessage: "비밀번호가 등록되었습니다.\r\n로그인 화면으로 이동합니다.",
                    params: null
                }
            );

            const target = window.location.origin;
            window.location = target;
            //setInterval(() => window.location = target, 1000);
        }
        else {
            const params = { ...this.state.params };
            params.success = false;
            params.message = "비밀번호 등록에 실패하였습니다.\r\n시스템 관리자에게 문의하세요.";

            this.setState(
                {
                    errors: null,
                    loading: false,
                    params: params
                }
            );
        }
    }

    setErrorMessage(passwordError, confirmError) {
        this.setState(
            {
                errors: {
                    password: passwordError,
                    passwordConfirm: confirmError
                }
            });
    }

    render() {
        if (this.state.loading || !this.state.params) {
            return (
                <div className={styles.loginArea}>
                    <div className={styles.titleBox}>
                        <h2>{this.state.loadingMessage}</h2>
                    </div>
                </div>
            );
        }

        if (this.state.params.success === false) {
            return (
                <div className={styles.loginArea}>
                    <div className={styles.titleBox}>
                        <h2>비밀번호 등록실패</h2>
                        <span className={styles.responseMessage + " " + styles.fail}>{this.state.params.message}</span>
                        <hr />
                    </div>
                </div>
            );
        }

        const responseMessageClassName = this.state.params.success ? styles.responseMessage : styles.responseMessage + " " + styles.fail;
        const userName = this.state.params.level === "사원" ? this.state.params.name + "님" : this.state.params.name + " " + this.state.params.level + "님";

        return (
            <div className={styles.loginArea}>
                <div className={styles.titleBox}>
                    <h2>비밀번호 등록</h2>
                    <h6>{userName}께서 사용할 비밀번호를 입력해 주세요.</h6>
                    <hr />
                </div>
                <div className={styles.loginBox}>
                    <input className={styles.textBox} type="text" name="userID" value={this.state.params.userID} disabled="true" />
                    <input ref={this.refPassword} className={styles.textBox} type="password" placeholder="비밀번호" onKeyUp={(event) => this.onKeyUp(event)} />
                    <span className={styles.errorMessage}>{this.state.errors?.password}</span>
                    <input ref={this.refPasswordConfirm} className={styles.textBox} type="password" placeholder="비밀번호 확인" onKeyUp={(event) => this.onKeyUp(event)} />
                    <span className={styles.errorMessage}>{this.state.errors?.passwordConfirm}</span>
                    <button className={styles.btnPrimary} disabled={this.state.buttonDisabled} onClick={this.onClick}>등록</button>
                    <span className={responseMessageClassName}>{this.state.params.message}</span>
                </div>
            </div>
        );
    }
}
