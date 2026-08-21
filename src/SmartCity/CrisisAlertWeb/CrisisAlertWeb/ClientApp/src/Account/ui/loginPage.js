import React, { Component } from 'react';
import styles from '../../Common/css/style.css';
import Footer from '../../Root/footer';
import Menu from '../../Root/menu';

import { AccountController } from '../services/accountController';

import SessionString from '../../Common/js/sessionString';
import AccountResource from '../resource/id';


class LoginPage extends Component {
    constructor(props) {
        super(props);

        this.refID = React.createRef();
        this.refPW = React.createRef();
        this.refSave = React.createRef();

        this.props = props;
    }

    onClickLogin = () => {
        const id = this.refID.current.value.toString().trim();

        if (id.length === 0) {
            alert(AccountResource.ID.textLoginIDError);
            return;
        }

        const pw = this.refPW.current.value.toString().trim();

        if (pw.length === 0) {
            alert(AccountResource.ID.textLoginPwdError);
            return;
        }

        const save = this.refSave.current.checked;

        this.doLogin(id, pw, save);
    }

    async doLogin(id, pw, save) {
        const result = await AccountController.login(id, pw, save);

        if (result === null) {
            alert(AccountResource.ID.textLoginError);
        }

        if (result.success == true) {
            // 로그인 성공
            // 재난 유형 선택 페이지로 이동
            //this.props.history.push(Menu.pathFacilityType);
            await this.showFacilityType(result.key);
        }
        else {
            alert(AccountResource.ID.textLoginError);
        }
    }

    async showFacilityType(key) {
        const result = await AccountController.sessionLogin(key);

        if (result === null || result.user === null) {
            //alert(AccountResource.ID.textLoginSessionError);
        } else if (result.user.level.id === AccountResource.ID.accountLevel.general) {
            // 총괄 관리자인 경우
            this.props.history.push(Menu.pathTypeMenu);
        } else if (result.user.level.id === AccountResource.ID.accountLevel.manager) {
            let facilityType = result.user.user.facilityType;

            if (facilityType === null || facilityType === undefined)
                facilityType = 1;
            else {
                let arrFacility = facilityType.split(',');

                if (arrFacility === null || arrFacility.length < 1)
                    facilityType = 1;
                else {
                    facilityType = arrFacility[0];
                    facilityType = parseInt(facilityType);
                }
            }

            // 세션 스토리지에 재난분류 타입 저장
            window.sessionStorage.setItem(SessionString.Key.facilityType, facilityType);

            // 일반 관리자인 경우
            this.props.history.push(Menu.pathMain);
        }

    }

    //protected List<int> StringToIntList(string strData) {
    //    if (strData == null)
    //        return null;

    //    List < int > datas = new List<int>();

    //    if (strData.Length == 0)
    //        return datas;

    //    int data;
    //    string[] tokens = strData.Split(',');

    //    foreach(string strToken in tokens)
    //    {
    //        if (int.TryParse(strToken.Trim(), out data))
    //            datas.Add(data);
    //    }

    //    return datas;
    //}

    async checkSession() {
        let key = null;

        if (window.localStorage.getItem(SessionString.Key.account) !== null) {
            key = JSON.parse(window.localStorage.getItem(SessionString.Key.account));
        } else if (window.sessionStorage.getItem(SessionString.Key.account) !== null) {
            key = JSON.parse(window.sessionStorage.getItem(SessionString.Key.account));
        } else {
            return;
        }

        this.showFacilityType(key);
    }

    onClickPWFind = () => {
        this.props.history.push(Menu.pathPwdFind);
    }

    render() {
        this.checkSession();

        return (
            <div className="area">
                <div className="containers">
                    <div className="header">
                        <embed src="/resource/img/daeguCitySlogan.svg" ></embed>
                        <p className="title"><span>위기경보수준</span>관리 시스템</p>
                        <p className="title_word">로그인 후 이용하실 수 있습니다.</p>
                    </div>
                    <div className="login_form">
                        <p>ID</p>
                        <input ref={this.refID} type="text" name="id" className="text_id" placeholder="아이디" />
                        <p>PW</p>
                        <input ref={this.refPW} type="password" name="password" className="text_pw" placeholder="비밀번호" />
                        <input type="submit" className="submit_btn" value="LOGIN" onClick={this.onClickLogin} />
                        <label id="id_save"><input ref={this.refSave} type="checkbox" />아이디 저장하기</label>
                        <div className="verticalLine"></div>
                        <span className="pwFind" onClick={this.onClickPWFind}>비밀번호 찾기</span>
                    </div>
                </div>
                <Footer />
            </div>
        );
    }
}

export default LoginPage;
