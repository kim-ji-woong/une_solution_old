import React, { Component } from 'react';
import styles from './css/account.module.css';
import personUsingLaptopLow from '../Account/image/personUsingLaptopLow.jpg';

export class Register extends Component {
    constructor(props) {
        super(props);

        this.refName = React.createRef();
        this.refEmail = React.createRef();
        this.refPhoneNumber = React.createRef();
        this.refRegistUser = React.createRef();
        this.refChangePassword = React.createRef();
        this.refPW = React.createRef();
        this.refPWConfirm = React.createRef();

        this.state =
        {
            errors: null,
            buttonDisabled: false,
            loading: false,
            registUser: true,
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
        let nameError = null, emailError = null, phoneNumberError = null, pwError = null, pwConfirmError = null;

        if (name.length === 0) {
            nameError = "이름을 입력하세요.";
        }

        const email = this.refEmail.current.value.toString().trim();

        if (email.length === 0) {
            emailError = "전자메일주소를 입력하세요.";
        }

        const phoneNumber = this.refPhoneNumber.current.value.toString().trim();

        if (phoneNumber.length === 0) {
            phoneNumberError = "휴대폰 번호를 입력하세요.";
        }

        let password = null;

        if (this.state.registUser) {
            password = this.refPW.current.value.toString().trim();
            const passwordConfirm = this.refPWConfirm.current.value.toString().trim();

            if (password.length === 0) {
                pwError = "비밀번호를 입력하세요.";
            }
            else if (passwordConfirm.length === 0) {
                pwConfirmError = "비밀번호 확인을 입력하세요.";
            }
            else if (password !== passwordConfirm) {
                pwConfirmError = "비밀번호가 일치하지 않습니다.";
            }
        }

        if (nameError || emailError || phoneNumberError || pwError || pwConfirmError) {
            this.setErrorMessage(nameError, emailError, phoneNumberError, pwError, pwConfirmError);
        }
        else {
            this.setState({ buttonDisabled: true, loading: true });
            this.props.onRegist(name, email, phoneNumber, password);
        }
    }

    onKeyUp(event) {
        if (event.key === 'Enter') {
            this.onClick(null);
        }
    }

    setErrorMessage(nameError, emailError, phoneNumberError, pwError, pwConfirmError) {
        this.setState(
            {
                errors: {
                    name: nameError,
                    email: emailError,
                    phoneNumber: phoneNumberError,
                    pw: pwError,
                    pwConfirm: pwConfirmError
                }
            });
    }

    onChangeRegist(registUser) {
        if (this.state.registUser === registUser) {
            return;
        }

        this.setState({ registUser });
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
                <div className={styles.registerSpaceArea}>
                    <div className={styles.registerSpace1}>
                       <img src={personUsingLaptopLow} className={styles.registerSpace} style={{ width: "500px", height: "600px" }} />
                    </div>
                    <div className={styles.registerSpace2}>
                        <div className={styles.registerborder}>
                            <span className={styles.registerTitle}>사용자 등록</span>
                            <div className={styles.loginBox}>
                                <div className={styles.radioBox}>
                                    <label className={styles.radioOption}>
                                        <input type="radio" ref={this.refRegistUser} className={styles.radioControl} checked={this.state.registUser} onChange={() => this.onChangeRegist(true)} />
                                        사용자 등록
                                    </label>
                                    <label className={styles.radioOption}>
                                        <input type="radio" ref={this.refRegistUser} className={styles.radioControl} checked={!this.state.registUser} onChange={() => this.onChangeRegist(false)} />
                                        비밀번호 변경
                                    </label>
                                </div>
                                <span className={styles.nameText}>이름</span>
                                  <input ref={this.refName} className={styles.textBox} type="text" name="userName" onKeyUp={(event) => this.onKeyUp(event)} />
                                  <span className={styles.errorMessage}>{this.state.errors?.name}</span>
                                <span className={styles.mailText}>전자메일주소(아이디)</span>
                                  <input ref={this.refEmail} className={styles.textBox} type="text" name="eMail" onKeyUp={(event) => this.onKeyUp(event)} />
                                  <span className={styles.errorMessage}>{this.state.errors?.email}</span>
                                <span className={styles.phoneText}>휴대폰번호</span>
                                  <input ref={this.refPhoneNumber} className={styles.textBox} type="text" name="phoneNumber" onKeyUp={(event) => this.onKeyUp(event)} />
                                  <span className={styles.errorMessage}>{this.state.errors?.phoneNumber}</span>
                                  {
                                    this.state.registUser &&
                                    <>
                                        <span className={styles.passwordText}>비밀번호</span>
                                         <input ref={this.refPW} className={styles.textBox} type="password" name="userPW" onKeyUp={(event) => this.onKeyUp(event)} />
                                         <span className={styles.errorMessage}>{this.state.errors?.pw}</span>
                                        <span className={styles.passwordTextCheck}>비밀번호 확인</span>
                                         <input ref={this.refPWConfirm} className={styles.textBox} type="password" name="userPW" onKeyUp={(event) => this.onKeyUp(event)} />
                                         <span className={styles.errorMessage}>{this.state.errors?.pwConfirm}</span>
                                    </>
                                  }
                                <button className={styles.btnPrimary} disabled={this.state.buttonDisabled} onClick={this.onClick}>등록</button>
                                <span className={responseMessageClassName}>{this.props.response?.message}</span>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        );
    }
}
