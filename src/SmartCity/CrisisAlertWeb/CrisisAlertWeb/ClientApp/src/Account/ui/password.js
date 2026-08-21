import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';

import Title from '../../Root/title';
import { AccountController } from '../services/accountController';
import Menu from '../../Root/menu';

import styles from '../../Common/css/style.css';

import AccountResource from '../resource/id';
import SessionString from '../../Common/js/sessionString';

class Password extends Component {
    constructor(props) {
        super(props);

        this.state = {
            userID: null,
        }

        this.refPW = React.createRef();
        this.refNewPW = React.createRef();
        this.refNewPW2 = React.createRef();

        this.props = props;

        // 세션 값으로 유저ID 확인
        this.checkSession();
    }

    async checkSession() {
        let key = null;

        if (window.localStorage.getItem(SessionString.Key.account) === null && window.sessionStorage.getItem(SessionString.Key.account) === null) {
            // 로그인 정보가 없으면 로그인 페이지로 이동
            alert(AccountResource.ID.textLoginSessionError);
            this.props.history.push('/');
        } else if (window.localStorage.getItem(SessionString.Key.account) !== null) {
            key = JSON.parse(window.localStorage.getItem(SessionString.Key.account));
        } else if (window.sessionStorage.getItem(SessionString.Key.account) !== null) {
            key = JSON.parse(window.sessionStorage.getItem(SessionString.Key.account));
        }

        const result = await AccountController.sessionLogin(key);

        if (result === null) {
            alert(AccountResource.ID.textLoginSessionError);
            this.props.history.push('/');
        } else if (result.success === false) {
            alert(result.message);
            this.props.history.push('/');
        } else if (result.user.id !== null && result.user.id !== undefined && result.user.id !== -1) {
            this.state.userID = result.user.id;
        } else {
            alert(AccountResource.ID.textLoginSessionError);
            this.props.history.push('/');
        }

    }

    onClickConfirm = () => {
        const pw = this.refPW.current.value.toString().trim();

        if (pw.length === 0) {
            alert(AccountResource.ID.textLoginPwdError);
            return;
        }

        const newPW = this.refNewPW.current.value.toString().trim();

        if (newPW.length === 0) {
            alert(AccountResource.ID.textNewPwdError);
            return;
        }

        const newPW2 = this.refNewPW2.current.value.toString().trim();

        if (newPW2.length === 0) {
            alert(AccountResource.ID.textNewPwd2Error);
            return;
        }

        if (newPW !== newPW2) {
            alert(AccountResource.ID.textNewPwd3Error);
            return;
        }

        if (pw === newPW) {
            alert(AccountResource.ID.textNewPwd4Error);
            return;
        }

        this.changePW(pw, newPW);
    }

    async changePW(pw, newPW) {
        const result = await AccountController.changePassword(this.state.userID, pw, newPW);

        alert(result.message);

        if (result.success === true) {
            this.props.history.push(Menu.pathSetting);
        }
    }

    onClickCancle = () => {
        this.props.history.push(Menu.pathSetting);
    }

    render() {

        return (
            <div className="container_sub2">

                <div class="header_sub">
                    <span><p id="behav_title">비밀번호 변경</p></span>
                </div>

                <div className="contents">
                    <div id="pw_title">
                        <p><span style={{ fontSize: "17px",  fontWeight: "900", paddingBottom:"5px"}} >비밀번호 변경</span></p><br/>
                            <p>안전한 비밀번호로 개인정보를 보호하세요.</p>
                    </div>
                    <div id="pw_box">
                        <input ref={this.refPW} type="password" className="pw_1" placeholder="현재 비밀번호" />
                        <input ref={this.refNewPW} type="password" className="pw_2" placeholder="새 비밀번호" />
                        <input ref={this.refNewPW2} type="password" className="pw_3" placeholder="새 비밀번호 확인" />
                    </div>
                    <div className="pw_cbox">
                        <div className="pw_cancel" onClick={this.onClickCancle} >
                            <p>취소</p>
                        </div>
                        <div className="pw_confirm" onClick={this.onClickConfirm} >
                            <p>확인</p>
                        </div>
                    </div>
                </div>

            </div>
        );
    }
}

export default withRouter(Password);
