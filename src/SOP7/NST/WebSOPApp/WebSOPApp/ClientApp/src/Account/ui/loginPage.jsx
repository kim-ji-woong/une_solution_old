import * as React from 'react';
import { withRouter } from 'react-router-dom';
import SessionString from '../../Common/js/sessionString';

import accounts from '../css/account.module.css';
/*import sampleNst from '../image/sample_bg.png'; */
import sampleNst from '../image/refinery.jpg';
import idIcon from '../image/icon/id_icon-02.png';
import pwIcon from '../image/icon/pw_icon-03.png';
import enhancement from '../image/icon/enhancement_CI.png';
import technology from '../image/icon/technology_CI-02.png';
import kist from '../image/icon/KIST CI-03.png';

import $ from 'jquery';

import AccountResource from '../resource/id';
import { AccountController } from '../services/accountController';

//import Menu from '../../Root/menu';
import ConfirmDialog from '../../Common/ui/confirmDialog';
import RootResource from '../../Root/resource/id';

/*interface Props {
	history: any
}

interface State {
	loginError: string | null,
	confirmMessage: {
		visible: boolean,
		title: string,
		messages: Array<string | null>,
		buttons: Array<string> | null,
		onClose: object | null,
		onClickButton: object | null
	}
}*/

class LoginPage extends React.Component/*<Props, State>*/ {
	/*private refID: React.RefObject<HTMLInputElement>;
	private refPW: React.RefObject<HTMLInputElement>;*/

	constructor(props/*: Props*/) {
		super(props);

		this.refID = React.createRef();
		this.refPW = React.createRef();

		this.state = {
			loginError: null,
			confirmMessage: {
				visible: false,
				title: "",
				messages: [""],
				buttons: ["확인"],
				onClose: this.onCloseConfirmDialog,
				onClickButton: null
			}
		}
	}

	componentDidUpdate() {
		//console.log('componentDidUpdate');
	}

	componentWillMount() {
		//console.log('componentWillMount');
	}

	componentDidMount() {
		$('body').css({ 'background': 'rgba(0,0,0,0.9)' });
	}

	onClickLogin() {
		const loginPage = this;
		//const loginPage: LoginPage = this as LoginPage;

		if (loginPage?.refID?.current && loginPage?.refPW?.current && loginPage.doLogin) {
			const id = loginPage.refID.current.value.toString().trim();

			if (id.length === 0) {
				//this.setState({ loginError: AccountResource.ID.textLoginIDError });
				this.showConfirmDialog("로그인", [AccountResource.ID.textLoginIDError], null, null);
				return;
			}

			const pw = loginPage.refPW.current.value.toString().trim();

			if (pw.length === 0) {
				//this.setState({ loginError: AccountResource.ID.textLoginPwdError });
				loginPage.showConfirmDialog("로그인", [AccountResource.ID.textLoginPwdError], null, null);
				return;
			}

			loginPage.doLogin(id, pw);
		}
	}

	async doLogin(id/*: string*/, pw/*: string*/) {
		const result = await AccountController.login(id, pw);

		if (result === null) {
			//this.setState({ loginError: AccountResource.ID.textLoginError });
			this.showConfirmDialog("로그인", [AccountResource.ID.textLoginError], null, null);
			return;
		}

		if (result.success == true) {
			// 로그인 성공

			// SDMS 페이지로 이동
			this.props.history.push(RootResource.path.sdms);
		}
		else {
			//this.setState({ loginError: AccountResource.ID.textLoginError });
			this.showConfirmDialog("로그인", [AccountResource.ID.textLoginError], null, null);
		}
	}

	onKeyPressLogin = (e/*: KeyboardEvent*/) => {
		if (e.key === 'Enter') {
			this.onClickLogin();
		}

		return;
	}

	onClickSetPwd = () => {
		this.props.history.push("/setPassword");
	}

	showConfirmDialog = (title/*: string*/, messages/*: Array<string | null>*/, buttons/*: Array<string> | null*/, onClickButton/*: object | null*/) => {
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

	render() {
		if (window?.localStorage) {
			const item = window.localStorage.getItem(SessionString.Key.account);

			if (item) {
				const user = JSON.parse(item);

				if (user !== null && user !== undefined && user.sessionKey !== null && user.sessionKey !== undefined) {
					// 로그인이 되어있다면
					// SDMS 페이지로 이동
					this.props.history.push(RootResource.path.sdms);
				}

				if (this.state.loginError != null) {
					//alert(this.state.loginError);
					//this.showConfirmDialog("로그인", [this.state.loginError], null, null);
				}

			}
		}

		return (
			<>
				<div id={accounts.popupConts} className={accounts.loginPopup}>
					{/*<div className={accounts.indexLoginTitle}><img src="/resource/image/common/index_logo.png" alt="NST" /></div>*/}
					<header className={accounts.loginHeader}>
						<h1><img src={enhancement} alt="" /></h1>
						<h2 className={accounts.title}>산업재해 통합 안전관리시스템</h2>
					</header>
					{/*<div className={accounts.indexLoginBox}>
				<ul>
					<li><input ref={this.refID} type="text" className={accounts.indexLoginId} onKeyPress={(e) => this.onKeyPressLogin(e)} placeholder="ID" /></li>
					<li><input ref={this.refPW} type="password" className={accounts.indexLoginPw} onKeyPress={(e) => this.onKeyPressLogin(e)} placeholder="Password" /></li>
				</ul>
				<div>
					<a onClick={this.onClickLogin} className={accounts.btnLogin}>로그인</a>
					<a className={accounts.btnSetPwd} onClick={this.onClickSetPwd}>비밀번호 설정</a>
				</div>
			</div>*/}
					<form className={accounts.loginForm}>
						<section className={accounts.formGroup}>
							<img src={idIcon} alt="id" className={accounts.idIcon} /><input ref={this.refID} type="email" name="loginEmail" id="loginEmail" placeholder="ID" className={accounts.idText} onKeyPress={(e) => this.onKeyPressLogin(e)} />
						</section>
						<section className={accounts.formGroup}>
							<img src={pwIcon} alt="pw" className={accounts.pwIcon} /><input ref={this.refPW} type="password" name="loginPassword" id="loginPassword" placeholder="Password" className={accounts.passwordText} onKeyPress={(e) => this.onKeyPressLogin(e)} />
						</section>
						{/*<div className={accounts.idSave}>
				<input type="checkbox" /><span>아이디 저장</span>
				</div>*/}
						<section className={accounts.formBtnWrap}>
							<div className={accounts.btnGroup + " " + accounts.btnFull}>
								<button onClick={() => this.onClickLogin()} type="button" id="btnLogin" className={accounts.btn + " " + accounts.btnLogin}>로그인</button>
								{/*<button className={accounts.btn + " " + accounts.btnLogout}>로그아웃</button>*/}
							</div>
						</section>
						{/*<div className={nst.btnGroup + " " + nst.btnFull}>
                <button type="button" className={nst.btn + " " + nst.btnSingUp} data-popup={nst.popupJoin}>회원가입</button>
            </div>*/}
					</form>
					<span className={accounts.tech}><img src={technology} alt="" /></span>
					<span className={accounts.kist}><img src={kist} alt="" /></span>
				</div>
				<div className={accounts.dim}>
					<img src={sampleNst} alt="임시배경" className={accounts.sampleNst} />
				</div>
				{
					/* alert창 대신 사용 */
					this.state.confirmMessage.visible &&
					<ConfirmDialog title={this.state.confirmMessage.title} messages={this.state.confirmMessage.messages} buttons={this.state.confirmMessage.buttons} onClose={this.state.confirmMessage.onClose} onClickButton={this.state.confirmMessage.onClickButton} />
				}
			</>
		);
	}
}

export default withRouter(LoginPage);