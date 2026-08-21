import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';

import Title from '../../Root/title';
import { AccountController } from '../services/accountController';
import Menu from '../../Root/menu';

import styles from '../../Common/css/style.css';

import AccountResource from '../resource/id';
import SessionString from '../../Common/js/sessionString';

class PwdFind extends Component {
    constructor(props) {
        super(props);

        this.state = {
            step: 1,
            userID: null,
        }

        this.props = props;
    }

    showPage = () => {
        let step = this.state.step;

        if (step === 1) {
            return <PwdFindID onClickCancle={this.onClickCancle} onClickID={this.onClickID} />;
        } else if (step === 2) {
            return <PwdFindCode onClickCancle={this.onClickCancle} onClickCode={this.onClickCode} />;
        } else if (step === 3) {
            return <PwdFindRepass onClickCancle={this.onClickCancle} onClickPWDFind={this.onClickPWDFind} />;
        }
    }

    onClickCancle = () => {
        this.props.history.push('/');
    }

    onClickID = (id) => {
        // 아이디 확인 후 다음 페이지 이동
        this.checkID(id);
    }

    async checkID(id) {
        const result = await AccountController.checkUserID(id);

        if (result.success == true) {
            let step = this.state.step;
            step++;

            // 다음 페이지 화면
            this.setState({ step: step, userID: id });
        } else {
            alert(result.message);
        }
    }

    onClickCode = (code) => {
        // 인증코드를 확인 후 다음 페이지 이동
        this.checkCode(code);
    }

    async checkCode(code) {
        const result = await AccountController.checkCode(code);

        if (result.success == true) {
            let step = this.state.step;
            step++;

            // 다음 페이지 화면
            this.setState({ step: step});
        } else {
            alert(result.message);
        }
    }

    onClickPWDFind = (pw) => {
        // 비밀번호 변경 후 로그인 창으로 이동
        this.pwdFind(pw);
    }

    async pwdFind(pw) {
        const userID = this.state.userID;

        if (userID === null || userID === undefined) {
            alert(AccountResource.ID.textEnterError);
            return;
        }
            
        const result = await AccountController.pwdFind(userID, pw);

        if (result.success == true) {
            // 다음 페이지 화면
            alert(result.message);
            this.props.history.push('/');

        } else {
            alert(result.message);
        }
    }

    render() {

        let page = this.showPage();

        return (
            <div className="container_sub2">
                <div className="header_sub">
                    <span><p id="behav_title">비밀번호 찾기</p></span>
                </div>


                {page}


            </div>
        );
    }
}

export default withRouter(PwdFind);



class PwdFindID extends Component {
    constructor(props) {
        super(props);

        this.refID = React.createRef();

        this.state = {

        }

        this.props = props;
    }

    onClickCancle = () => {
        this.props.onClickCancle();
    }

    onClickID = () => {
        const id = this.refID.current.value.toString().trim();

        if (id.length === 0) {
            alert(AccountResource.ID.textLoginIDError);
            return;
        }

        this.props.onClickID(id);
    }

    render() {

        return (
            <div className="contents">
                <div className="circles">
                    <div className="circle1"></div>
                    <div className="circle2"></div>
                    <div className="circle3"></div>
                </div>
                <div id="passwordfind_title">
                    <p>아이디입력</p>
                    <p>비밀번호를 찾고자하는 아이디를 입력해주세요.</p>
                </div>
                <div id="pw_box">
                    <input ref={this.refID} type="text" className="pw_1" placeholder="아이디" />
                </div>
                <div className="pw_cbox">
                    <div className="pw_cancel" onClick={this.onClickCancle}>
                        <p>취소</p>
                    </div>
                    <div className="pw_confirm" onClick={this.onClickID}>
                        <p>확인</p>
                    </div>
                </div>
            </div>
        );
    }
}


class PwdFindCode extends Component {
    constructor(props) {
        super(props);

        this.refCode = React.createRef();

        this.state = {

        }

        this.props = props;
    }

    onClickCancle = () => {
        this.props.onClickCancle();
    }

    onClickCode = () => {
        const code = this.refCode.current.value.toString().trim();

        if (code.length === 0) {
            alert(AccountResource.ID.textFindCodeError);
            return;
        }

        this.props.onClickCode(code);
    }

    render() {

        return (
            <div className="contents">
                <div className="circles">
                    <div className="circle1"></div>
                    <div className="circle2" style={{backgroundColor:"dodgerblue"}}></div>
                    <div className="circle3"></div>
                </div>
                <div id="passwordfind_title">
                    <p>인증코드</p>
                    <p>부여된 인증코드를 입력해 주세요.</p>
                </div>
                <div id="pw_box">
                    <input ref={this.refCode} type="password" className="pw_1" placeholder="인증코드" />
                </div>
                <div className="pw_cbox">
                    <div className="pw_cancel" onClick={this.onClickCancle}>
                        <p>취소</p>
                    </div>
                    <div className="pw_confirm" onClick={this.onClickCode}>
                        <p>확인</p>
                    </div>
                </div>
            </div>
        );
    }
}


class PwdFindRepass extends Component {
    constructor(props) {
        super(props);

        this.refPW = React.createRef();
        this.refPW2 = React.createRef();

        this.state = {

        }

        this.props = props;
    }

    onClickCancle = () => {
        this.props.onClickCancle();
    }

    onClickPWDFind = () => {
        const pw = this.refPW.current.value.toString().trim();

        if (pw === 0) {
            alert(AccountResource.ID.textNewPwdError);
            return;
        }

        const pw2 = this.refPW2.current.value.toString().trim();

        if (pw2 === 0) {
            alert(AccountResource.ID.textNewPwd2Error);
            return;
        }

        if (pw !== pw2) {
            alert(AccountResource.ID.textNewPwd3Error);
            return;
        }

        this.props.onClickPWDFind(pw);
    }

    render() {

        return (
            <div className="contents">
                <div className="circles">
                    <div className="circle1"></div>
                    <div className="circle2" style={{ backgroundColor: "dodgerblue" }}></div>
                    <div className="circle3" style={{ backgroundColor: "dodgerblue" }}></div>
                </div>
                <div id="passwordfind_title">
                    <p>비밀번호 재설정</p>
                    <p>비밀번호를 입력하세요.</p>
                </div>
                <div id="pw_box">
                    <input ref={this.refPW} type="password" className="pw_1" placeholder="새 비밀번호" />
                    <input ref={this.refPW2} type="password" className="pw_1" placeholder="새 비밀번호 확인" />
                </div>
                <div className="pw_cbox">
                    <div className="pw_cancel" onClick={this.onClickCancle}>
                        <p>취소</p>
                    </div>
                    <div className="pw_confirm" id="modal_btn" onClick={this.onClickPWDFind}>
                        <p>확인</p>
                    </div>
                </div>
            </div>
        );
    }
}
