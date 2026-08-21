import React, { Component } from 'react';
import { BrowserRouter as Route, Link } from 'react-router-dom';
import MainMenubar from './mainMenubar';
import AlarmWindow from './alarmWindow';
import pc_cctv_sample from '../../Common/img/main/pc_cctv_sample.jpg';
import pc_graph_sample from '../../Common/img/main/pc_graph_sample.jpg';
import store from '../../Root/store';

import { SDMSController } from '../../SDMS/services/sdmsController';

//import defaults from '../../Common/css/default.module.css';
import commons from '../../Common/css/common.module.css';
import styles from '../../Common/css/style.module.css';

import $ from 'jquery';

class Main extends Component {

	constructor(props) {
		super(props);

		this.state = {
			type: "main",
			sensorAlarms: null,
		}

		this.props = props;
	}

	componentDidUpdate() {
		//console.log(' componentDidUpdate');
	}

	componentWillMount() {
		//console.log('componentWillMount');
	}

	componentWillUpdate(nextProps, nextState) {
		//console.log('componentWillUpdate');
	}

	componentDidMount() {
		//console.log('componentDidMount');

		// 서브 페이지에서 html,body에 부여했던 style 값 제거.
		$('html, body').removeAttr("style");

		// 각 페이지 별로 클래스 초기화
		$('#subPage').removeClass('sop');

		// 센서 히스토리 & 날씨정보 감시 타이머 시작
		SDMSController.StartWatchTimer();
	}

	render() {
		return (
			<div id={commons.wrap}>

				<MainMenubar type={this.state.type} />

				<div id={styles.mnTop}>
					<div className={commons.container}>
						<div className={styles.mntTitle}>
							<h3>안전한 <span>LG화학</span><br /><span>내손에서</span> 출발합니다.</h3>
							<p>안전은 권리 입니다.</p>
							<a href="#">자세히 보기</a>
						</div>
						<ul className={styles.mntBtn}>
							<li>
								<a href="#">
									<h5>실시간 근무표</h5>
									<p>현재 근무중인 직원과 비번인 직원들의 정보를 확인하고 편집할 수 있습니다.</p>
								</a>
							</li>
							<li>
								<a href="#">
									<h5>비상조직</h5>
									<p>재난발생시 효과적인 대응을 위하여 구성될 임시조직을 확인하고 편집할 수 있습니다..</p>
								</a>
							</li>
							<li>
								<a href="#">
									<h5>알람현황</h5>
									<p>센서 및 재난타입에 따른 알람들에 대한 조회 및 분석화면들을 보여줍니다.</p>
								</a>
							</li>
							<li>
								<a href="#">
									<h5>SOP</h5>
									<p>SOP 매뉴얼들의 내용을 확인하고 실행할 수 있습니다.</p>
								</a>
							</li>
							<li>
								<a href="#">
									<h5>설정</h5>
									<p>시스템 전반적인 환경설정 정보가 담겨있습니다.</p>
								</a>
							</li>
						</ul>
					</div>
				</div>



				<div id={styles.mnMid}>
					<div className={commons.container}>
						<div className={styles.mnmWrap}>
							<div className={styles.mnTro}>
								<div className={styles.mnmTitle}>
									<h4>CCTV</h4>
									<a href="#">더보기</a>
								</div>
								<div className={styles.mnmCont + " " + styles.mnmCcTv}>
									<img src={pc_cctv_sample} alt="" style={{ display: 'block' }} />
									<p>최근 조회 CCTV - XXX 1층 A 구역</p>
								</div>
							</div>
							<div className={styles.mnTro}>
								<div className={styles.mnmTitle}>
									<h4>작동한 센서 정보 <span>(최근7일)</span></h4>
									<a href="#">더보기</a>
								</div>
								<div className={styles.mnmCont + " " + styles.mnmGrph}>
									<img src={pc_graph_sample} alt="" style={{ display: 'block', maxWidth: '100%' }} />
								</div>
							</div>
							<div className={styles.mnTro}>
								<div className={styles.mnmTitle}>
									<h4>최근 알람</h4>
									<a href="#">더보기</a>
								</div>
								<div className={styles.mnmCont + " " + styles.mnmTb}>
									<table>
										<caption>번호, 발생일시, 유형, 발생위치, 현장으로 구성된 표</caption>
										<colgroup>
											<col style={{ width: '13%' }} />
											<col style={{ width: '25%' }} />
											<col style={{ width: '15%' }} />
											<col style={{ width: '32%' }} />
											<col style={{ width: '15%' }} />
										</colgroup>
										<thead>
											<tr>
												<th>No</th>
												<th>발생일시</th>
												<th>유형</th>
												<th>발생일시</th>
												<th>현장</th>
											</tr>
										</thead>
										<tbody>
											<tr>
												<td>1</td>
												<td>2020-12-05</td>
												<td>화재</td>
												<td>XXX 1층 A 구역</td>
												<td>주의</td>
											</tr>
											<tr>
												<td>2</td>
												<td>2020-12-04</td>
												<td>지진</td>
												<td>XXX 3층 C 구역</td>
												<td>경계</td>
											</tr>
											<tr>
												<td>3</td>
												<td>2020-12-03</td>
												<td>화재</td>
												<td>XXX 1층 A 구역</td>
												<td>경계</td>
											</tr>
											<tr>
												<td>4</td>
												<td>2020-12-03</td>
												<td>화재</td>
												<td>XXX 1층 A 구역</td>
												<td>주의</td>
											</tr>
											<tr>
												<td>5</td>
												<td>2020-12-05</td>
												<td>화재</td>
												<td>XXX 1층 A 구역</td>
												<td>주의</td>
											</tr>
											<tr>
												<td>6</td>
												<td>2020-12-04</td>
												<td>지진</td>
												<td>XXX 3층 C 구역</td>
												<td>경계</td>
											</tr>
											<tr>
												<td>7</td>
												<td>2020-12-03</td>
												<td>화재</td>
												<td>XXX 1층 A 구역</td>
												<td>경계</td>
											</tr>
											<tr>
												<td>8</td>
												<td>2020-12-03</td>
												<td>화재</td>
												<td>XXX 1층 A 구역</td>
												<td>주의</td>
											</tr>
										</tbody>
									</table>
								</div>
							</div>
						</div>
					</div>
				</div>

				<DisplayAlarm />

				<div id={styles.mnFooter} className={/*defaults.pb160*/"pb160"}>
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

class DisplayAlarm extends Component {
	constructor(props) {
		super(props);

		this.state = {
			sensorAlarms: store.getState().sensorAlarm,
		}

		this.props = props;

		// 오류로 인한 Redux 관련 부분 주석처리
		//store.subscribe(function () {
		//	this.setState({ sensorAlarms: store.getState().sensorAlarm });
		//}.bind(this));
	}

	render() {
		let alarmArea = "";

		let sensorAlarms = this.state.sensorAlarms;
		sensorAlarms = sensorAlarms == null ? new Array() : sensorAlarms;

		if (sensorAlarms.length != 0) {
			alarmArea = <AlarmWindow sensorAlarms={this.state.sensorAlarms}/>;
        }

		return (<> {alarmArea} </>);
	}
}

export default Main;