import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import $ from 'jquery';

import uis from '../../../Common/css/ui.module.css';
import contents from '../../../Common/css/content.module.css';
import uneCommon from '../../../Common/css/uneCommon.module.css';
import accounts from '../../css/account.module.css';

import { AccountController } from '../../services/accountController';
import { TeamEditController } from '../../../TeamEditor/services/teamEditController';

import AcoountResource from '../../resource/id';
import SessionString from '../../../Common/js/sessionString';

import ProjectResource from '../../../Root/resource/id';
import ConfirmDialog from '../../../Common/ui/confirmDialog';

import { SDMSController } from '../../../SDMS/services/sdmsController';

class accountFindPwd extends Component {
	constructor(props) {
		super(props);

        this.refID = React.createRef();
        this.refEmail = React.createRef();
        this.refPhone = React.createRef();
        this.refEmailMode = React.createRef();
        this.refSMSMode = React.createRef();

		this.state = {
            showMessage: "",
            result: null,
            mode: null,
            titleID: "",        
            placeID: AcoountResource.ID.textPlaceID,
            confirmMessage: {
                visible: false,
                title: "",
                messages: [""],
                buttons: ["확인"],
                onClose: this.onCloseConfirmDialog,
                onClickButton: null
            },
            reload: null,
		}

        this.props = props;
        this.initSiteID();
    }

	componentDidUpdate(prevProps, prevState) {
        //console.log('componentDidUpdate');

        //this.setUI();
	}

	componentWillMount() {
		//console.log('componentWillMount');
	}

	componentWillUpdate(nextProps, nextState) {
		//console.log('componentWillUpdate');
	}

	componentDidMount() {
		//console.log('componentDidMount');

        this.checkParamsCode();
    }

    async initSiteID() {
        const siteID = ProjectResource.SiteID;

        if (siteID === null || siteID === undefined) {
            // 사이트 ID 요청
            const [result, message] = await SDMSController.requestGetSiteID();

            if (result !== null && result !== undefined) {
                ProjectResource.SiteID = result;
            }

            this.setState({ reload: true });
        }
    }

    showConfirmDialog = (title, messages, buttons, onClickButton) => {
        const confirmMessage = { ...this.state.confirmMessage };
        confirmMessage.visible = true;
        confirmMessage.title = title;
        confirmMessage.buttons = buttons;
        confirmMessage.onClickButton = onClickButton;

        if (!messages) {
            confirmMessage.messages = [""];
        }
        else if (Array.isArray(messages)) {
            confirmMessage.messages = messages;
        }
        else {
            confirmMessage.messages = [messages];
        }

        this.setState({ confirmMessage });
    }

    onCloseConfirmDialog = () => {
        const confirmMessage = { ...this.state.confirmMessage };
        confirmMessage.visible = false;

        this.setState({ confirmMessage });
    }

    async checkParamsCode() {
        const search = window.location.search.substring(1);

        if (!search) {
            // 값이 없을 경우
            // 사용자 정보 입력 모드
            this.setState({
                mode: AcoountResource.findMode.email,
                showMessage: AcoountResource.ID.textSetPasswordInfo,
                titleID: AcoountResource.ID.textTitleName,
                placeID: AcoountResource.ID.textPlaceName,
            });

            return;
        }
    }

    onClick = () => {
        if (this.state.mode === AcoountResource.findMode.email) {
            const name = this.refID.current.value.toString().trim();
            const email = this.refEmail.current.value.toString().trim();

            if (name.length === 0) {
                this.showConfirmDialog("에러", ["이름을 입력하세요."], null, null);
                return;
            }
            if (email.length === 0) {
                this.showConfirmDialog("에러", ["이메일을 입력하세요."], null, null);
                return;
            }

            this.changePassword(name, email, AcoountResource.findMode.email);

        } else if (this.state.mode === AcoountResource.findMode.sms) {
            const name = this.refID.current.value.toString().trim();
            const phone = this.refPhone.current.value.toString().trim();

            if (name.length === 0) {
                this.showConfirmDialog("에러", ["이름을 입력하세요."], null, null);
                return;
            }
            if (phone.length === 0) {
                this.showConfirmDialog("에러", ["핸드폰 번호를 입력하세요."], null, null);
                return;
            }

            this.changePassword(name, phone, AcoountResource.findMode.sms);
        }

    }

    async changePassword(name, value, mode) {
        this.setState({ showMessage: "처리 중입니다." });

        const [result, message] = await AccountController.changePassword(name, value, mode);

        if (result === null) {
            this.setState({ showMessage: message });
            this.showConfirmDialog("에러", [message], null, null);
        } else if (result.success === true) {
            this.setState({ showMessage: message });
            this.showConfirmDialog("성공", [message], ["확인"], this.onClickCancle);
        } 
    }

    onClickCancle = () => {
        // 메인 페이지 이동
        this.props.history.push('/');
    }

    setUI = () => {
        // 모드에 따른 UI 변경
        if (this.state.mode === AcoountResource.findMode.email) {
            $('#userInfo').show();
            $('#rowEmail').show();
            $('#rowPhone').hide();
        } else if (this.state.mode === AcoountResource.findMode.sms) {
            $('#userInfo').show();
            $('#rowEmail').hide();
            $('#rowPhone').show();
        } 
    }

    displayInputUI = () => {
        let displayModeUI = [];
        let displayInputUI = [];

        if (this.state.mode === AcoountResource.findMode.sms) {
            displayInputUI.push(
                <>
                    <tr key={"userID"}>
                        <td>・ {this.state.titleID}</td>
                        <td><input type="text" ref={this.refID} id="userID" className={contents.DblueInput + " " + contents.w100p} placeholder={this.state.placeID} /></td>
                    </tr>
                    <tr key={"userPhone"} id="rowPhone">
                        <td>・ {AcoountResource.ID.textTitlePhone}</td>
                        <td>
                            <input
                                type="text"
                                ref={this.refPhone}
                                className={contents.DblueInput + " " + contents.w100p}
                                placeholder={AcoountResource.ID.textPlacePhone}
                                onChange={(e) => this.onChangeCheck(e.target)} />
                        </td>
                    </tr>
                </>);
        } else {
            displayInputUI.push(
                <>
                    <tr key={"userID"}>
                        <td>・ {this.state.titleID}</td>
                        <td><input type="text" ref={this.refID} id="userID" className={contents.DblueInput + " " + contents.w100p} placeholder={this.state.placeID} /></td>
                    </tr>
                    <tr key={"userEmail"} id="rowEmail">
                        <td>・ {AcoountResource.ID.textTitleEmail}</td>
                        <td><input type="text" ref={this.refEmail} className={contents.DblueInput + " " + contents.w100p} placeholder={AcoountResource.ID.textPlaceEmail} /></td>
                    </tr>
                </>);
        }

        return displayInputUI;
    }

    onChangeMode = (mode) => {
        const currentMode = this.state.mode;

        if (mode !== currentMode) {
            this.setState({ mode: mode });
        }
    }

    onChangeCheck = (e) => {
        let target = e;

        // 휴대전화일 경우 숫자 및 자릿수 제한
        const regex = /^[0-9\b -]{0,13}$/;

        if (regex.test(target.value)) {
            let value = target.value;
            let inputValue = value.replace(/-/g, '');

            if (inputValue.length === 4) {
                inputValue = inputValue.replace(/(\d{3})(\d{1})/, '$1-$2');
            } else if (inputValue.length === 8) {
                inputValue = inputValue.replace(/(\d{3})(\d{4})(\d{1})/, '$1-$2-$3');
            } else if (inputValue.length === 10) {
                inputValue = inputValue.replace(/(\d{3})(\d{3})(\d{4})/, '$1-$2-$3');
            } else if (inputValue.length === 11) {
                inputValue = inputValue.replace(/(\d{3})(\d{4})(\d{4})/, '$1-$2-$3');
            } else {
                inputValue = value;
            }

            this.refPhone.current.value = inputValue;
        }

        return;
    }

    displaySelectModeUI = () => {
        let displaySelectModeUI = [];

        if (ProjectResource.SiteID === ProjectResource.Site.GCC) {
            // 녹십자
            displaySelectModeUI.push(
                <div className={accounts.findCheck}>
                    <li>
                        <input ref={this.refEmailMode} type="radio" name="mode" id="EmailMode" onChange={() => this.onChangeMode(AcoountResource.findMode.email)} defaultChecked />
                        <label for="EmailMode">카카오웍스로 찾기</label>
                    </li>
                    <li className={accounts.disabledCursor}>
                        <input ref={this.refSMSMode} type="radio" name="mode" id="SMSMode" onChange={() => this.onChangeMode(AcoountResource.findMode.sms)} disabled />
                        <label for="SMSMode">SMS으로 찾기</label>
                    </li>
                </div>
            );
            
        } else {
            displaySelectModeUI.push(
                <div className={accounts.findCheck}>
                    <li>
                        <input ref={this.refEmailMode} type="radio" name="mode" id="EmailMode" onChange={() => this.onChangeMode(AcoountResource.findMode.email)} defaultChecked />
                        <label for="EmailMode">Email으로 찾기</label>
                    </li>
                    <li>
                        <input ref={this.refSMSMode} type="radio" name="mode" id="SMSMode" onChange={() => this.onChangeMode(AcoountResource.findMode.sms)} />
                        <label for="SMSMode">SMS으로 찾기</label>
                    </li>
                </div>
            );
        }

        return displaySelectModeUI;
    }
        

    render() {

        let displayInputUI = this.displayInputUI();
        let displaySelectModeUI = this.displaySelectModeUI();

		return (
			<>
                <div id={contents.popupConts} className={contents.loginPopup}>
                    <div className={contents.passwordConts}>
                        <div className={contents.passwordBoxTitle}>비밀번호 찾기</div>

                        <div className={contents.passwordBox}>
                            <div className={contents.passwordBoxTxt}>{this.state.showMessage}</div>

                            {displaySelectModeUI}

                            <table className={contents.tblNone}>
                                <caption>게시판입니다.</caption>
                                <colgroup>
                                    <col style={{ width: "30%" }} />
                                    <col style={{ width: "*" }} />
                                </colgroup>
                                <tbody id="userInfo" >
                                    {displayInputUI}
                                </tbody>
                            </table>

                            <div className={contents.gap20}></div>

                            <div className={uis.btnArea + " " + uis.alignC}>
                                <a onClick={this.onClick} className={contents.btnBlue}>확인</a>
                                <a onClick={this.onClickCancle} className={contents.btnNavy}>취소</a>
                            </div>
                        </div>

                    </div>
                </div>
                <div className={contents.dim}></div>
                {
                    /* alert창 대신 사용 */
                    this.state.confirmMessage.visible &&
                    <ConfirmDialog
                        title={this.state.confirmMessage.title}
                        messages={this.state.confirmMessage.messages}
                        buttons={this.state.confirmMessage.buttons}
                        onClose={this.state.confirmMessage.onClose}
                        onClickButton={this.state.confirmMessage.onClickButton} />
                }
			</>
        );
    }
}

export default withRouter(accountFindPwd);