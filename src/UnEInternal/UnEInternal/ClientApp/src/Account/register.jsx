import React, { Component } from 'react';
import styles from './css/account.module.css';

export class Register extends Component {
    constructor(props) {
        super(props);

        this.refName = React.createRef();
        this.refEmail = React.createRef();

        this.state =
        {
            errors: null,
            buttonDisabled: false,
            loading: false,
            prevProps: props
        }
    }

    static getDerivedStateFromProps(props, state) {
        if (props === state.prevProps) {
            return state;
        }

        return {
            errors: null,
            buttonDisabled: false,
            loading: false,
            prevProps: props
        };
    }

    onClick = (event) => {
        const name = this.refName.current.value.toString().trim();
        let nameError = null, emailError = null;

        if (name.length === 0) {
            nameError = "이름을 입력하세요.";
        }

        const email = this.refEmail.current.value.toString().trim();

        if (email.length === 0) {
            emailError = "전자메일주소를 입력하세요.";
        }

        if (nameError || emailError) {
            this.setErrorMessage(nameError, emailError);
        }
        else {
            this.setState({ buttonDisabled: true, loading: true });
            this.props.onRegist(name, email);
        }
    }

    onKeyUp(event) {
        if (event.key === 'Enter') {
            this.onClick(null);
        }
    }

    setErrorMessage(nameError, emailError) {
        this.setState(
            {
                errors: {
                    name: nameError,
                    email: emailError
                }
            });
    }

    render() {
        if (this.state.loading) {
            return (
                <div className={styles.loginArea}>
                    <div className={styles.titleBox}>
                        <h2>처리중입니다...</h2>
                    </div>
                </div>
            );
        }

        const responseMessageClassName = this.props.response?.success ? styles.responseMessage : styles.responseMessage + " " + styles.fail;

        return (
            <div className={styles.loginArea}>
                <div className={styles.titleBox}>
                    <h2>사용자 등록</h2>
                    <h6>직원 정보를 입력해주세요.</h6>
                    <hr />
                </div>
                <div className={styles.loginBox}>
                    <input ref={this.refName} className={styles.textBox} type="text" name="userName" placeholder="이름" onKeyUp={(event) => this.onKeyUp(event)} />
                    <span className={styles.errorMessage}>{this.state.errors?.name}</span>
                    <input ref={this.refEmail} className={styles.textBox} type="text" name="eMail" placeholder="전자메일주소" onKeyUp={(event) => this.onKeyUp(event)} />
                    <span className={styles.errorMessage}>{this.state.errors?.email}</span>
                    <button className={styles.btnPrimary} disabled={this.state.buttonDisabled} onClick={this.onClick}>등록</button>
                    <span className={responseMessageClassName}>{this.props.response?.message}</span>
                </div>
            </div>
        );
    }
}
