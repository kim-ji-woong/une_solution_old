import React, { Component } from 'react';
import styles from './css/account.module.css';

export class Login extends Component {
    constructor(props)
    {
        super(props);

        this.refID = React.createRef();
        this.refPW = React.createRef();

        this.state =
        {
            loading: false,
            errors: null,
            buttonDisabled: false,
            prevProps: props
        }
    }

    static getDerivedStateFromProps(props, state) {
        if (props === state.prevProps) {
            return state;
        }

        return {
            loading: false,
            errors: null,
            buttonDisabled: false,
            prevProps: props
        };
    }

    onClick = (event) => {
        const id = this.refID.current.value.toString().trim();
        let idError = null, pwError = null;

        if (id.length === 0) {
            idError = "아이디를 입력하세요.";
        }

        const pw = this.refPW.current.value.toString().trim();

        if (pw.length === 0) {
            pwError = "비밀번호를 입력하세요.";
        }

        if (idError || pwError) {
            this.setErrorMessage(idError, pwError);
        }
        else {
            this.setState({ loading: true, buttonDisabled: true });
            this.props.onLogin(id, pw);
        }
    }

    onKeyUp(event) {
        if (event.key === 'Enter') {
            this.onClick(null);
        }
    }

    setErrorMessage(idError, pwError) {
        this.setState(
            {
                errors: {
                    id: idError,
                    pw: pwError
                }
            });
    }

    render() {
        if (this.state.loading) {
            return (
                <div className={styles.loginArea}>
                    <div className={styles.titleBox}>
                        <h2>데이터 처리중입니다...</h2>
                    </div>
                </div>
            );
        }

        return (
            <div className={styles.loginArea}>
                <div className={styles.titleBox}>
                    <h2>로그인</h2>
                    <h6></h6>
                    <hr />
                </div>
                <div className={styles.titleBox2}></div>
                <div className={styles.loginborder}>
                <div className={styles.loginBox}>
                    <input ref={this.refID} className={styles.textBox} type="text" name="userID" placeholder="아이디" onKeyUp={(event) => this.onKeyUp(event)} />
                    <span className={styles.errorMessage}>{this.state.errors?.id}</span>
                    <input ref={this.refPW} className={styles.textBox} type="password" name="userPW" placeholder="비밀번호" onKeyUp={(event) => this.onKeyUp(event)} />
                    <span className={styles.errorMessage}>{this.state.errors?.pw}</span>
                    {/*
                        <div className={styles.checkBox}>
                            <input data-val="true" data-val-required="사용자 이름 및 암호 저장 필드가 필요합니다." id="RememberMe" name="RememberMe" type="checkbox" value="true" />
                            <input name="RememberMe" type="hidden" value="false" />
                            <label htmlFor="RememberMe"><span className={styles.save}>&nbsp;사용자 이름 및 암호 저장</span></label>
                        </div>
                    */}
                    <button className={styles.btnPrimary} disabled={this.state.buttonDisabled} onClick={this.onClick}>로그인</button>
                </div>
                <div className={styles.registBox}>
                    <p>비밀번호를 잊어버렸거나 처음 사용자일 경우 아래의 링크를 눌러 비밀번호를 설정하여 주세요.</p>
                    <p>
                        <span className={styles.registLink} onClick={() => this.props.onRegist(null, null) }>비밀번호 설정</span>
                    </p>
                    </div>
                </div>
            </div>
        );
    }
}
