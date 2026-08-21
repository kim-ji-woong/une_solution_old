import React, { Component } from 'react';
import MainMenubar from '../../Main/ui/mainMenubar';
import LoginPage from './loginPage';
import SignUpPage from './signUpPage';
import $ from 'jquery';

import styles from '../../Common/css/style.module.css';
import commons from '../../Common/css/common.module.css';

import TeamEditorResource from '../resource/id';

class AccountLayout extends Component {
	static pathAccount = '/account/:path';
	static pathLogin = 'login';
	static pathSignUp = 'signUp';

	constructor(props) {
		super(props);
		this.state = {
			loginClass: null,
			signUpClass: null,
			content: null,
			type: "account",
			nextPath: "/",			// 로그인 후 이동할 페이지
		}

		this.props = props;

		if (this.props.match.params.path == AccountLayout.pathLogin) {
			this.state.loginClass = styles.current;
			this.state.content = TeamEditorResource.ID.textLogin;
		} else if (this.props.match.params.path == AccountLayout.pathSignUp) {
			this.state.signUpClass = styles.current;
			this.state.content = TeamEditorResource.ID.textSignUp;
		} else {
			// 로그인 없이 다른 페이지로 이동 시
			this.state.loginClass = styles.current;
			this.state.content = TeamEditorResource.ID.textLogin;
			this.state.nextPath = this.props.match.params.path;
        }
			
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
		$('html, body').css({ 'color': '#000', 'font-size': '14px' });
	}

	onClickLogin = () => {
		if (this.state.loginClass == styles.current)
			return;

		this.setState({ loginClass: styles.current, signUpClass: null, content: TeamEditorResource.ID.textLogin});
	}

	onClickSignUp = () => {
		if (this.state.signUpClass == styles.current)
			return;

		this.setState({ loginClass: null, signUpClass: styles.current, content: TeamEditorResource.ID.textSignUp });
    }

	render() {
		return (
			<div id={commons.wrap}>

				<MainMenubar type={this.state.type} />


				<div id={styles.content}>
					<div className={styles.lgnWrap}>

						<ul className={styles.lgnTab}>
							<li className={this.state.loginClass}><a onClick={this.onClickLogin}>{TeamEditorResource.ID.textLogin}</a></li>
							{/*<li className={this.state.signUpClass}><a onClick={this.onClickSignUp}>{TeamEditorResource.ID.textSignUp}</a></li>*/}
						</ul>


						<DisplayContent content={this.state.content} nextPath={this.state.nextPath} />


					</div>
				</div>


				<div id={styles.mnFooter}>
					<div className={commons.container}>
						<p><b>서울사무소</b> 140-710 서울시 용산구 서계동 209 주연빌딩 8층</p>
						<p><b>대구본사</b> 705-701 대구시 달서구 달구벌대로 1053 계명대학교 첨단산업지원센터 108호</p>
						<p><b>T.</b> 02-714-4133</p>
						<p><b>Ｆ.</b> 02-714-4134</p>
						<p><b>E.</b> exe@unes.co.kr</p>
						<span className={styles.mnfCpy}>COPYRIGHT U&E corp. ALL RIGHTS RESERVED.</span>
					</div>
				</div>

			</div>
        );
    }
}

class DisplayContent extends Component {
	componentDidUpdate(prevProps, prevState) {
		if (this.props.type !== prevProps.type) {

			if (this.props.type == "account") {
				this.setState({ layoutClass: styles.bk });
			} else {
				this.setState({ layoutClass: null });
			}
		}
	}

	render() {
		if (this.props.content == TeamEditorResource.ID.textLogin)
			return <LoginPage nextPath={this.props.nextPath} />
		else
			return <SignUpPage />
	}
}

export default AccountLayout;