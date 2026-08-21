import React, { Component } from 'react';
import styles from './css/account.module.css';
import closeUpBusinessman from '../Account/image/closeUpBusinessman.jpg';

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

    onClickLogin = (event) => {
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
            this.onClickLogin(null);
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
                <div className={styles.loginSpaceArea}>
                    <img src={closeUpBusinessman} className={styles.loginSpace} style={{ width: "480px" , height: "600px" }} />
                    <div className={styles.loginSpace2}>
                     <div className={styles.loginborder}>
                        <span className={styles.loginTitle}>Sensor Maker</span>
                            <div className={styles.loginBox}>
                              <span className={styles.idText}>아이디</span>
                                <input ref={this.refID} className={styles.textBox} type="text" name="userID" onKeyUp={(event) => this.onKeyUp(event)} />
                                <span className={styles.errorMessage}>{this.state.errors?.id}</span>
                              <span className={styles.passwordText}>비밀번호</span>
                                <input ref={this.refPW} className={styles.textBox} type="password" name="userPW" onKeyUp={(event) => this.onKeyUp(event)} />
                                <span className={styles.errorMessage}>{this.state.errors?.pw}</span>
                              <button className={styles.btnPrimary} disabled={this.state.buttonDisabled} onClick={this.onClickLogin}>로그인</button>
                        </div>
                        <div className={styles.registBox}>
                            <p className={styles.leftAlignText}>비밀번호를 잊어버렸거나 처음 사용자일 경우 아래의 링크를 눌러 주세요.</p>
                            <p>
                                <span className={styles.registLink} onClick={() => this.props.onRegist(null, null) }>사용자 등록 또는 비밀번호 설정</span>
                            </p>
                        </div>
                    </div>
                    </div>
                </div>
            </div> 
        );
    }
}
