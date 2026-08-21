import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import { BrowserRouter as Route, Link } from 'react-router-dom';

import styles from '../../Common/css/style.module.css';
import commons from '../../Common/css/common.module.css';

import SessionString from '../../Common/js/sessionString';

import $ from 'jquery';

class MainMenubar extends Component {
	constructor(props) {
		super(props);

		this.state = {
			layoutClass: null,
		}

		this.props = props;

		if (this.props.type == "account") {
			this.state.layoutClass = styles.bk;
        }
	}

	componentDidUpdate(prevProps, prevState) {
		if (this.props.type !== prevProps.type) {

			if (this.props.type == "account") {
				this.setState({ layoutClass: styles.bk });
			} else {
				this.setState({ layoutClass: null });
            }
		}
	}

	componentWillMount() {
		//console.log('componentWillMount');
	}

	componentWillUpdate(nextProps, nextState) {
		//console.log('componentWillUpdate');
	}

	componentDidMount() {
		$('.' + styles.mngAll + ' > .' + commons.container).html($('.' + styles.mngMenu).clone());
	}

	onMouseOverMenu(e) {
		// this가 제대로 인식을 하지 못함 >> 함수 내부이기 때문에
		var target = e;

		// 이벤트 발생 위치 확인 및 변경
		if (target.localName == "a" && target.parentElement.parentElement.parentElement.localName == "div") {
			target = e.parentElement;
		} else if (target.localName !== "li" || target.parentElement.parentElement.localName !== "div") {
			return;
		}

		//console.log("menu mouse over");
		// 기존 jQuery 소스
		$(target).addClass(styles.on);
		$(target).children('ul').slideDown('fast');

		return;
	}

	onMouseLeaveMenu(e) {
		// this가 제대로 인식을 하지 못하기 때문에 변수로 전달
		var target = e;

		// 이벤트 발생 위치 확인 및 변경
		if (target.localName !== "li") {
			target = target.parentElement;
		} else if (target.parentElement.parentElement.className == styles.on) {
			target = target.parentElement.parentElement;
		}

		//console.log("menu mouse leave:" + target.localName);
		// 기존 jQuery 소스
		$(target).removeClass(styles.on);
		$(target).children('ul').stop().slideUp('fast');

		return;
	}

	onClickAllMenu(e) {
		// this가 제대로 인식을 하지 못하기 때문에 변수로 전달
		var target = e;

		if ($(target).is('.' + styles.on)) {
			$(target).removeClass(styles.on);
			$('.' + styles.mngAll).slideUp('fast');
		} else {
			$(target).addClass(styles.on);
			$('.' + styles.mngAll).slideDown('fast');
		}

		return;
	}

	onClickLogout = () => {
		// 세션 초기화
		window.localStorage.removeItem(SessionString.Key.account);

		// 메인 페이지 이동
		this.props.history.push('/');
    }

	render() {
		let loginArea = null;

		// 로그인 여부 확인한 뒤, 로그인 영역 생성
		if (window.localStorage.getItem(SessionString.Key.account) == null) {
			loginArea = <>
				{/*<li><Link to="/account/signUp">회원가입</Link></li>*/}
				<li><a>{/*회원가입*/}</a></li>
				<li><Link to="/account/login">로그인</Link></li>
			</>
		} else {
			loginArea = <>
				<li><a></a></li>
				<li><a onClick={this.onClickLogout}>로그아웃</a></li>
			</>
        }
		
		return (
			<div id={styles.mnGnb} className={this.state.layoutClass}>
				<h1 className={styles.mngLogo}><Link to="/"><span>LG화학</span> 스마트 재난관리 시스템</Link></h1>
				<ul className={styles.mngMenu}>
					<li onMouseOver={(e) => this.onMouseOverMenu(e.target)} onMouseLeave={(e) => this.onMouseLeaveMenu(e.target)}>
						<Link to="/sdms">재난관리</Link>
						<ul>
							<li><a href="#">3D</a></li>
							<li><a href="#">보고서</a></li>
							<li><a href="#">센서</a></li>
							<li><a href="#">편집</a></li>
							<li><a href="#">설정</a></li>
						</ul>
					</li>
					<li onMouseOver={(e) => this.onMouseOverMenu(e.target)} onMouseLeave={(e) => this.onMouseLeaveMenu(e.target)}>
						<Link to="/sop-simulator">SOP</Link>
						<ul>
							<li><Link to="/sop-simulator" target="_blank">SOP 빠른실행</Link></li>
							<li><Link to="/sop-simulatorcall" target="_blank">SOP 불러오기</Link></li>
							<li><Link to="/sop-simulatorset" target="_blank">설정</Link></li>
							<li><a href="#">SOP 실행</a></li>
						</ul>
					</li>
					<li onMouseOver={(e) => this.onMouseOverMenu(e.target)} onMouseLeave={(e) => this.onMouseLeaveMenu(e.target)}>
						<Link to="/sop-manager">SOP 편집</Link>
					</li>
					<li onMouseOver={(e) => this.onMouseOverMenu(e.target)} onMouseLeave={(e) => this.onMouseLeaveMenu(e.target)}><a href="#">상황판</a></li>
					<li onMouseOver={(e) => this.onMouseOverMenu(e.target)} onMouseLeave={(e) => this.onMouseLeaveMenu(e.target)}>
						<Link to="/team-editor" >조직</Link>
						<ul>
							<li><Link to="/team-editor" target="_blank">조직</Link></li>
							<li><a href="#">근무표</a></li>
						</ul>
					</li>
					<li onMouseOver={(e) => this.onMouseOverMenu(e.target)} onMouseLeave={(e) => this.onMouseLeaveMenu(e.target)}><a href="#">CCTV</a></li>
					<li onMouseOver={(e) => this.onMouseOverMenu(e.target)} onMouseLeave={(e) => this.onMouseLeaveMenu(e.target)}><a href="#">도움말</a></li>
				</ul>
				<ul className={styles.mngUsr}>

					{loginArea}

				</ul>
				<button className={styles.mngBtn} onClick={(e) => this.onClickAllMenu(e.target)}>
					<span>메뉴</span>
				</button>
				<div className={styles.mngAll}>
					<div className={commons.container}></div>
				</div>
			</div>
        );
    }
}

export default withRouter(MainMenubar);