import React, { Component } from 'react';

import styles from '../../Common/css/style.module.css';

class Footer extends Component {

	render() {
		return (
			<div id={styles.sopFt}>
				<div className={styles.scFt}>
					{/*
					<p><b>서울사무소</b> 140-710 서울시 용산구 서계동 209 주연빌딩 8층</p>
					<p><b>대구본사</b> 705-701 대구시 달서구 달구벌대로 1053 계명대학교 첨단산업지원센터 108호</p>
					<p><b>T.</b> 02-714-4133</p>
					<p><b>Ｆ.</b> 02-714-4134</p>
					<p><b>E.</b> exe@unes.co.kr</p>
					<span className={styles.scfCpy}>COPYRIGHT U&E corp. ALL RIGHTS RESERVED.</span>
					*/}
				</div>
			</div>
		);
	}
}

export default Footer;