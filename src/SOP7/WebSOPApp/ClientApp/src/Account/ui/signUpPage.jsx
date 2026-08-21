import React, { Component } from 'react';

import styles from '../../Common/css/style.module.css';

import $ from 'jquery';

class SignUpPage extends Component {

	constructor(props) {
		super(props);
		this.state = {

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



	render() {
		return (
			<div class={styles.lgnForm}>
				<form action="">
					<div class={styles.joinIpt}>
						<h5>아이디</h5>
						<div class={styles.joinID}>
							<input type="text" />
							<a href="#">중복체크</a>
						</div>
						<p>중복된 아이디 입니다.</p>
					</div>
					<div class={styles.joinIpt}>
						<h5>비밀번호</h5>
						<input type="password" />
						<p>10자리 이상 영문 대/소문자, 숫자, 특수문자를 사용하세요.</p>
					</div>
					<div class={styles.joinIpt}>
						<h5>비밀번호 확인</h5>
						<input type="password" />
						<p>비밀번호가 일치하지 않습니다.</p>
					</div>
					<div class={styles.joinIpt}>
						<h5>Nick Name</h5>
						<input type="text" />
					</div>
					<div class={styles.joinIpt}>
						<h5>조직선택(선택사항)</h5>
						<a href="#">조직선택 하기</a>
					</div>
					<p class={styles.joinOgz}>선택한 조직 : 전기팀, 설비팀, EV팀, 운영팀, 총무팀</p>
					<a href="#" class={styles.joinSbmt}>가입하기</a>
				</form>
			</div>
        );
    }
}

export default SignUpPage;