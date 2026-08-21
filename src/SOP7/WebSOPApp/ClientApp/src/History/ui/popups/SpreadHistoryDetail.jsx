import { Button } from '@amcharts/amcharts4/core';
import React, { Component } from 'react';
import newStyles from "../../../Common/css/newStyle.module.css";

class SpreadHistoryDetail extends Component {

	constructor(props) {
		super(props);
		this.props = props;
	}

	render() {
		return (
			<div id={newStyles.hsMmo} className={newStyles.popup}>
				<div>
					<div>
						<div className={newStyles.hsmCont}>
							<div className={newStyles.hsmTitle}>
								<h3>문자 재발송</h3>
								<a href="javascript:popClose();" className={newStyles.hsmCls}>닫기</a>
							</div>
							<h5 className={newStyles.hsmSub}>발송 문구</h5>
							<textarea name="" id="" cols="30" rows="10" className={"scroll-wrapper" + " " + newStyles.hsmTxt + "scroll-bar"}>T1-1 구역 화재발생 비상방송 실시</textarea>
							<h5 className={newStyles.hsmSub}>전파 대상자</h5>
							<div className={"scroll-wrapper" + " " + newStyles.hsmSend + "scroll-bar"}>
								<ul className={newStyles.hsmUsr}>
									<li><input type="checkbox" name="" id="hsmUsr01" /><label for="hsmUsr01">안전관리팀 홍길동</label></li>
									<li><input type="checkbox" name="" id="hsmUsr02" /><label for="hsmUsr02">안전관리팀 홍길동</label></li>
									<li><input type="checkbox" name="" id="hsmUsr03" /><label for="hsmUsr03">안전관리팀 홍길동</label></li>
									<li><input type="checkbox" name="" id="hsmUsr04" /><label for="hsmUsr04">안전관리팀 홍길동</label></li>
									<li><input type="checkbox" name="" id="hsmUsr05" /><label for="hsmUsr05">안전관리팀 홍길동</label></li>
									<li><input type="checkbox" name="" id="hsmUsr06" /><label for="hsmUsr06">안전관리팀 홍길동</label></li>
									<li><input type="checkbox" name="" id="hsmUsr07" /><label for="hsmUsr07">안전관리팀 홍길동</label></li>
									<li><input type="checkbox" name="" id="hsmUsr08" /><label for="hsmUsr08">안전관리팀 홍길동</label></li>
									<li><input type="checkbox" name="" id="hsmUsr09" /><label for="hsmUsr09">안전관리팀 홍길동</label></li>
									<li><input type="checkbox" name="" id="hsmUsr10" /><label for="hsmUsr10">안전관리팀 홍길동</label></li>
								</ul>
							</div>
							<ul className={newStyles.hsmBtn}>
								<li><a href="javascript:popClose();">취소</a></li>
								<li><a href="#">발송</a></li>
							</ul>
						</div>
					</div>
				</div>
			</div>
		);
	}
}

export default SpreadHistoryDetail;