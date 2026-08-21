import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import SessionString from '../../Common/js/sessionString';
import ConfirmDialog from '../../Common/ui/confirmDialog';

import uis from '../../Common/css/ui.module.css';
import contents from '../../Common/css/content.module.css';
import accounts from '../css/account.module.css';
import newStyles from '../../Common/css/newStyle.module.css';

import $ from 'jquery';

import TeamEditorResource from '../resource/id';
import { AccountController } from '../services/accountController';

import logo from '../../Common/image/common/index_logo.png';
import Glogo from '../../Common/image/common/GCgreencross_white.png';

import { SDMSController } from '../../SDMS/services/sdmsController';
import ProjectResource from '../../Root/resource/id';
import AccountResource from '../resource/id';

class LoginPageSB extends Component {
	static FirstPage = "/sdms";

	constructor(props) {
		super(props);

		this.refID = React.createRef();
		this.refPW = React.createRef();

		this.refHighVer = React.createRef();
		this.refLightVer = React.createRef();

		this.state = {
			loginError: null,
			confirmMessage: {
				visible: false,
				title: "",
				messages: [""],
				buttons: ["확인"],
				onClose: this.onCloseConfirmDialog,
				onClickButton: null
			},
			reload: null,
			isFullVersion: true,			// 고,저용량 선택 인자 (true: 고용량(default), false: 저용량)
			loading: false,
			loadingMessage: "데이터를 불러오고 있습니다.",
			prevBeginCode: null
		}

		this.props = props;
		this.initSiteID();
		this.checkLogin();
	}

	componentDidUpdate() {
		//console.log('componentDidUpdate');
	}

	componentWillMount() {
		//console.log('componentWillMount');
	}

	componentWillUpdate(nextProps, nextState) {
		//console.log('componentWillUpdate');
	}

	componentDidMount() {
		$('body').css({ 'background': 'rgba(0,0,0,0.9)' });
	}

	async initSiteID() {
		let siteID = ProjectResource.SiteID;

		if (siteID === null || siteID === undefined) {
			// 사이트 ID 요청
			//const [result, message] = await SDMSController.requestGetSiteID();

			//if (result !== null && result !== undefined) {
			//	ProjectResource.SiteID = result;
			//}
			siteID = await ProjectResource.loadSiteID();

			this.setState({ reload: true });
		}
	}

	async checkLogin() {
		// 세션 키를 이용해 로그인 체크
		const user = await ProjectResource.initUserInfo();

		if (user !== null && user !== undefined) {

			if (user.sessionKey !== null && user.sessionKey !== undefined) {
				// 로그인 정보가 남아있다면
				const [result, message] = await AccountController.checkLoginSession(user.id, user.sessionKey);

				if (result === AccountResource.loginState.login) {
					// SDMS 페이지로 이동
					this.props.history.push(LoginPageSB.FirstPage);
					//this.props.history.push("/sdms");
					//this.props.history.push("/dashboard");
					//this.props.history.push("/history");
					//this.props.history.push("/sop-simulator");
					//this.props.history.push("/team-editor");
                }
			}
		}
    }

	onClickLogin = () => {
		const id = this.refID.current.value.toString().trim();

		if (id.length === 0) {
			this.setState({ loginError: TeamEditorResource.ID.textLoginIDError });
			//this.showConfirmDialog("에러", [TeamEditorResource.ID.textLoginIDError], null, null);
			return;
		}

		const pw = this.refPW.current.value.toString().trim();

		if (pw.length === 0) {
			this.setState({ loginError: TeamEditorResource.ID.textLoginPwdError });
			//this.showConfirmDialog("에러", [TeamEditorResource.ID.textLoginPwdError], null, null);
			return;
		}

		this.doLogin(id, pw);
    }

	async doLogin(id, pw) {
		//const siteID = ProjectResource.SiteID;
		const isFullVersion = this.state.isFullVersion;
		const result = await AccountController.login(id, pw, isFullVersion);
		
		if (result === null) {
			this.setState({ loginError: TeamEditorResource.ID.textLoginError });
			//this.showConfirmDialog("에러", [TeamEditorResource.ID.textLoginError], null, null);
        }

		if (result.success == true) {
			// 로그인 성공

			// 세션 저장
			ProjectResource.setLoginUser(result.user);

			// SDMS 페이지로 이동
			this.props.history.push(LoginPageSB.FirstPage);
			//this.props.history.push("/sdms");
		}
		else {
			if (result?.message && result.message.length > 0) {
				this.setState({ loginError: result.message });
			}
			else {
				this.setState({ loginError: TeamEditorResource.ID.textLoginError });
				//this.showConfirmDialog("에러", [TeamEditorResource.ID.textLoginError], null, null);
			}
        }
	}

	onKeyPressLogin = (e) => {
		if (e.key === 'Enter') {
			this.onClickLogin();
		}

		return;
	}

	onClickSetPwd = () => {
		this.props.history.push("/findPassword");
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

	getLogo(){
		if (ProjectResource.SiteID === ProjectResource.Site.Soulbrain) {
			return <div className={contents.indexLoginTitlee}><img className={accounts.logoSize} src={logo} alt="soulbrain" /></div>
		}
		else if (ProjectResource.SiteID === ProjectResource.Site.GCC) {
			return <div className={contents.indexLoginTitle}><img className={accounts.logoSizeGC} src={Glogo} alt="녹십자" /></div>
		}

		return <></>
    }

	onChangeSelectVer = (isFullVersion) => {
		this.setState({ isFullVersion: isFullVersion });
	}

	displayVersionUI = () => {
		// 솔브레인 버전 선택
		if (ProjectResource.SiteID === ProjectResource.Site.Soulbrain) {
			return (
				<>
					<li>
						<input ref={this.refHighVer} type="radio" name="version" id="HighVer" onChange={() => this.onChangeSelectVer(true)} defaultChecked />
						<label for="HighVer">Full ver.</label>
					</li>
					<li>
						<input ref={this.refLightVer} type="radio" name="version" id="LightVer" onChange={() => this.onChangeSelectVer(false)} />
						<label for="LightVer">Light ver.</label>
					</li>
				</>);
		}

		return <></>;
	}

	checkAutoLogin() {
		const parameters = window.location.search;

		if (parameters.length > 0) {
			return this.processBeginCode(parameters);
		}

		return false;
	}

	processBeginCode(parameters) {
		if (!parameters || parameters.length === 0) {
			return false;
		}

		parameters = parameters.substring(1).trim();

		const params = parameters.split('&');
		const paramCount = params.length;

		for (let i = 0; i < paramCount; i++) {
			const datas = params[i].split('=');

			if (datas.length !== 2) {
				continue;
			}

			const paramName = datas[0].trim();
			const paramValue = datas[1].trim();

			if (paramName.toLowerCase() === "bc") {
				const beginCode = paramValue;

				if (beginCode !== null && beginCode !== undefined && beginCode !== this.state.prevBeginCode) {
					this.autoLogin(beginCode);
					return true;
				}
			}
		}

		return false;
	}

	async autoLogin(beginCode) {
		this.setState({ loading: true });

		const result = await AccountController.autoLogin(beginCode);
		this.onAutoLogin(result, beginCode);
		window.history.pushState(null, null, window.location.origin);
	}

	onAutoLogin(loginData, beginCode) {
		if (loginData?.success) {
			ProjectResource.setLoginUser(loginData.user);
			this.setState({ loading: false, prevBeginCode: beginCode });

			// 다른 페이지로 이동
			this.props.history.push(LoginPageSB.FirstPage);
		}
		else {
			this.setState({ loading: false, prevBeginCode: beginCode });
		}
	}

	render() {
		if (this.state.loginError != null) {
			alert(this.state.loginError);
		}

		if (this.state.loading || this.checkAutoLogin()) {
			return (
				<h2>{this.state.loadingMessage}</h2>
			);
		}

		return (
			<>
				<div id={contents.popupConts} className={contents.loginPopup}>
					{
						this.getLogo()
                    }
					<div className={contents.indexLoginBox}>
    					<ul>
							<li><input ref={this.refID} type="text" className={contents.indexLoginId} onKeyPress={(e) => this.onKeyPressLogin(e)} placeholder="ID" /></li>
							<li><input ref={this.refPW} type="password" className={contents.indexLoginPw} onKeyPress={(e) => this.onKeyPressLogin(e)} placeholder="Password" /></li>
						</ul>
	
						<div className={accounts.loginCheck}>
							{
								this.displayVersionUI()
							}
						</div>
				
						<div className={uis.btnArea}>
							<a className={contents.btnLogin} onClick={this.onClickLogin} >로그인</a>
							<a className={accounts.btnSetPwd} onClick={this.onClickSetPwd}>비밀번호 찾기</a>
						</div>

					</div>
				</div>
				<div className={uis.dim}></div>
				{
					/* alert창 대신 사용 */
					this.state.confirmMessage.visible &&
					<ConfirmDialog title={this.state.confirmMessage.title} messages={this.state.confirmMessage.messages} buttons={this.state.confirmMessage.buttons} onClose={this.state.confirmMessage.onClose} onClickButton={this.state.confirmMessage.onClickButton} />
				}
			</>
        );
    }
}

export default withRouter(LoginPageSB);