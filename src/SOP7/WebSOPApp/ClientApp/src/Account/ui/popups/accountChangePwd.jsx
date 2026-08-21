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

class accountChangePwd extends Component {
	constructor(props) {
		super(props);

        this.refPassword = React.createRef();
        this.refRePassword = React.createRef();
        this.refPwd = React.createRef();

		this.state = {
            showMessage: "",
            result: null,
            mode: null,
            disableID: false,

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
		}

        this.props = props;
    }

	componentDidUpdate(prevProps, prevState) {
        //console.log('componentDidUpdate');
	}

	componentWillMount() {
		//console.log('componentWillMount');
	}

	componentWillUpdate(nextProps, nextState) {
		//console.log('componentWillUpdate');
	}

	componentDidMount() {
		//console.log('componentDidMount');
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

    onClick = () => {
        // 비밀번호 설정
        let error = "";

        const pwd = this.refPwd.current.value.toString().trim();
        if (pwd.length === 0) {
            error = "비밀번호를 입력하세요.";
        }

        const newPwd = this.refPassword.current.value.toString().trim();
        if (newPwd.length === 0) {
            error = "새로운 비밀번호를 입력하세요.";
        }

        const newRePwd = this.refRePassword.current.value.toString().trim();
        if (newRePwd.length === 0) {
            error = "새로운 비밀번호를 한번 더 입력하세요.";
        }

        if (newPwd.length > 0 && newRePwd.length > 0 && newPwd !== newRePwd) {
            error = "새로운 비밀번호가 서로 일치하지 않습니다.";
        }

        if (error.length > 0) {
            this.showConfirmDialog("에러", [error], null, null);
            return;
        }

        this.setPassword(pwd, newPwd);
    }

    async setPassword(pwd, newPwd) {
        this.setState({ showMessage: "처리 중입니다." });

        // id 값 불러오기
        let user = ProjectResource.getUserInfo();
        if (user === null || user === undefined) {
            let message = "유저 정보를 불러 올 수 없습니다. 관리자에게 문의바람";
            this.showConfirmDialog("에러", [message], null, null);
            this.setState({ showMessage: message });
        }

        const [result, message] = await AccountController.setPassword(user.id, pwd, newPwd);

        if (result === null) {
            this.setState({ showMessage: message });
            this.showConfirmDialog("에러", [message], null, null);
        } else if (result.success === true) {
            this.setState({ showMessage: "비밀번호 변경 성공" });
            this.showConfirmDialog("성공", ["비밀번호 변경 성공"], ["확인"], this.onClickCancle);
        } else if (result.success === false) {
            this.showConfirmDialog("에러", [message], null, null);
        }
    }

    onClickCancle = () => {
        this.props.onClickCloseChangePwd();
    }

    render() {

		return (
			<>
                <div id={contents.popupConts} className={contents.loginPopup}>
                    <div className={contents.passwordConts}>
                        <div className={contents.passwordBoxTitle}>비밀번호 변경</div>

                        <div className={contents.passwordBox}>
                            <div className={contents.passwordBoxTxt}>{this.state.showMessage}</div>
                            <table className={contents.tblNone}>
                                <caption>게시판입니다.</caption>
                                <colgroup>
                                    <col style={{ width: "30%" }} />
                                    <col style={{ width: "*" }} />
                                </colgroup>
                                <tbody id="userInfo" >
                                    <tr>
                                        <td>・ {AcoountResource.ID.textTitlePwdConfirm}</td>
                                        <td><input type="password" ref={this.refPwd} className={contents.DblueInput + " " + contents.w100p} placeholder={AcoountResource.ID.textPlacePwdConfirm} /></td>
                                    </tr>
                                    <tr id="rowPassword">
                                        <td>・ {AcoountResource.ID.textTitlePwd}</td>
                                        <td><input type="password" ref={this.refPassword} className={contents.DblueInput + " " + contents.w100p} placeholder={AcoountResource.ID.textPlacePwd} /></td>
                                    </tr>
                                    <tr id="rowRepassword">
                                        <td>・{AcoountResource.ID.textTitleRePwd}</td>
                                        <td><input type="password" ref={this.refRePassword} className={contents.DblueInput + " " + contents.w100p} placeholder={AcoountResource.ID.textPlacePwd} /></td>
                                    </tr>
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
                    <ConfirmDialog title={this.state.confirmMessage.title} messages={this.state.confirmMessage.messages} buttons={this.state.confirmMessage.buttons} onClose={this.state.confirmMessage.onClose} onClickButton={this.state.confirmMessage.onClickButton} />
                }
			</>
        );
    }
}

export default withRouter(accountChangePwd);