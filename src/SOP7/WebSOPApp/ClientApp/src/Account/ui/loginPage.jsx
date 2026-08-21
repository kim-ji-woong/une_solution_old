import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import MainMenubar from '../../Main/ui/mainMenubar';

import styles from '../../Common/css/style.module.css';

import $ from 'jquery';

import TeamEditorResource from '../resource/id';
import { AccountController } from '../services/accountController';

class LoginPage extends Component {

	constructor(props) {
		super(props);

		this.refID = React.createRef();
		this.refPW = React.createRef();

		this.state = {
			loginError: null,
		}

		this.props = props;
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
		//console.log('componentDidMount');
	}

	onClickLogin = () => {
		const id = this.refID.current.value.toString().trim();

		if (id.length === 0) {
			this.setState({ loginError: TeamEditorResource.ID.textLoginIDError });
			return;
		}

		const pw = this.refPW.current.value.toString().trim();

		if (pw.length === 0) {
			this.setState({ loginError: TeamEditorResource.ID.textLoginPwdError });
			return;
		}

		this.doLogin(id, pw);
    }

	async doLogin(id, pw) {
		const result = await AccountController.login(id, pw);

		if (result === null) {
			this.setState({ loginError: TeamEditorResource.ID.textLoginError });
        }

		if (result.success == true) {
			// 로그인 성공

			// 다음 페이지로 이동
			let nextPath = this.props.nextPath;

			if (nextPath.indexOf('/') != 0)
				nextPath = '/' + nextPath;

			this.props.history.push(nextPath);
		}
		else {
			this.setState({ loginError: TeamEditorResource.ID.textLoginError });
        }
	}

	onKeyPressLogin = (e) => {
		if (e.key === 'Enter') {
			this.onClickLogin();
		}

		return;
	}

	render() {
		let loginErrorText = null;
		if (this.state.loginError != null)
			loginErrorText = <p>{this.state.loginError}</p>;

		return (
			<div className={styles.lgnForm}>
				<div>
					<div className={styles.lgnIpt}>
						<input ref={this.refID} type="text" onKeyPress={(e) => this.onKeyPressLogin(e)} placeholder={TeamEditorResource.ID.textIDInput} title={TeamEditorResource.ID.textIDInput} />
						<input ref={this.refPW} type="password" onKeyPress={(e) => this.onKeyPressLogin(e)} placeholder={TeamEditorResource.ID.textPwdInput} title={TeamEditorResource.ID.textPwdInput} />

						{loginErrorText}

						<a onClick={this.onClickLogin}>{TeamEditorResource.ID.textLogin}</a>
					</div>
				</div>
				<ul className={styles.lgnBtn}>
					{/*
					<li><a href="#">{TeamEditorResource.ID.textIDFind}</a></li>
					<li><a href="#">{TeamEditorResource.ID.textPwdFind}</a></li>
					<li><a href="#">{TeamEditorResource.ID.textSignUp}</a></li>
					*/}
				</ul>
				<p className={styles.lgnInfo}>아이디/비밀번호 분실 <b>02-1234-5678</b> 로 문의해 주세요.</p>
				{/*<a href="#" className={styles.lgnJoin}>{TeamEditorResource.ID.textSignUp}</a>*/}
				<ul className={styles.lgnPvcy}>
					<li><a href="#">이용약관</a></li>
					<li><a href="#">개인정보처리방침</a></li>
				</ul>
			</div>
        );
    }
}

export default withRouter(LoginPage);