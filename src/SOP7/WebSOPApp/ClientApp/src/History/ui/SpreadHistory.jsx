import $ from 'jquery';
import React, { Component } from 'react';
import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';
import newStyles from '../../Common/css/newStyle.module.css';
import newDefault from '../../Common/css/newDefault.module.css';
import uneStyles from "../../Common/css/uneCommon.module.css";
import HistoryResource from "../resource/id";
import HistoryController from '../services/historyController';
import datepicker from 'react-datepicker';


class SpreadHistory extends Component {
	constructor(props) {
		super(props);

		this.state = {
			content: HistoryResource.ID.menu.userHistory,
			dataSource: null,
			maxRowCount: 10, // 한 페이지에 보여줄 data row 수
			maxPageCount: 5, // 한번에 보여줄 페이지 개수
			pageIndex: 1,    // 현재 페이지
			maxPageIndex: 1, // 최대 페이지 Index			
			dateType: 'today',
			beginDate: new Date(),
			endDate: new Date(),
			prevProps: null
		}

		this.props = props;
		this.display = this.display.bind(this);
	}

	componentDidMount() {
		this.display();
	}

	async display() {
		const beginDate = this.getMakeDateTime(this.state.beginDate) + ' 00:00:00';
		const endDate = this.getMakeDateTime(this.state.endDate) + ' 23:59:59';

		if (beginDate > endDate) {
			alert('조회 기간을 다시 선택하세요');
			return;
		}

		//const dataSource = await HistoryController.DisplaySOPHistories(beginDate, endDate);

		//const datacount = dataSource.length;
		//const value1 = parseInt(datacount / this.state.maxRowCount);
		////const value2 = datacount % (this.state.maxRowCount + 1); // 나머지가 있는 경우 페이지 하나를 추가한다.
		//let maxPageIndex = value1;// + ((value2 > 0) ? 1 : 0);

		//this.setState({ dataSource, maxPageIndex });
	}

	getMakeDateTime(dateTime) {
		let year = dateTime.getFullYear();
		let month = 1 + dateTime.getMonth();
		month = month >= 10 ? month : '0' + month;  //month 두자리로 저장
		let day = dateTime.getDate();                   //d
		day = day >= 10 ? day : '0' + day;

		//let hour = dateTime.getHours();
		//hour = hour >= 10 ? hour : '0' + hour;
		//let min = dateTime.getMinutes();
		//min = min >= 10 ? min : '0' + min;
		//let sec = dateTime.getSeconds();
		//sec = sec >= 10 ? sec : '0' + sec;

		let strDate = year + '-' + month + '-' + day; //+ ' ' + hour + ':' + min + ':' + sec;

		return strDate;
	}

	setPageIndex(index) {
		if (this.state.pageIndex === index) {
			return;
		}
		if (this.state.maxPageIndex < index || index < 1) {
			return;
		}

		this.setState({ pageIndex: index });
	}

	onChangeBegin = (date) => {
		this.setState({ beginDate: date });
		$("input:radio[name='stgDate']").prop('checked', false);

		let year = date.getFullYear();
		let month = date.getMonth() + 1;
		let day = date.getDate();

		let korFormat = year + "-" + month + "-" + day;
	}

	onChangeEnd = (date) => {
		this.setState({ endDate: date });
		$("input:radio[name='stgDate']").prop('checked', false);

		let year = date.getFullYear();
		let month = date.getMonth() + 1;
		let day = date.getDate();

		let korFormat = year + "-" + month + "-" + day;
	}

	onClickDateType = (type) => {
		let today = new Date();
		let date = new Date();
		let dateType = '';

		if (type === 'today') {
			dateType = 'today';
		}
		else if (type === 'week') {
			date.setDate(date.getDate() - 7);
			dateType = 'week';
		}
		else if (type === 'month') {
			date.setMonth(date.getMonth() - 1);
			dateType = 'month';
		}
		else if (type === 'year') {
			date.setFullYear(date.getFullYear() - 1);
			dateType = 'year';
		}

		let year = today.getFullYear();
		let month = today.getMonth() + 1;
		let day = today.getDate();

		let korFormat = year + "-" + month + "-" + day;

		this.setState({ beginDate: date, endDate: today, dateType: dateType });
	}

	// 하단 페이지 index 만들기
	getPageIndexUI() {
		let ui = [];
		if (!this.state.dataSource) {
			return ui;
		}

		const pageArr = new Array();

		let index = this.state.pageIndex;
		// 이전 페이지 넣기
		while (true) {
			index--;
			if (index < 1) {
				break;
			}
			if (this.state.pageIndex - 2 > index) {
				break;
			}

			pageArr.push(index);
		}
		index = this.state.pageIndex;
		pageArr.push(index);

		// 다음 페이지 넣기
		while (true) {
			if (pageArr.length === this.state.maxPageCount) {
				break;
			}

			index++;
			if (index > this.state.maxPageIndex) {
				break;
			}

			pageArr.push(index);
		}

		// 정렬
		pageArr.sort(function (a, b) { if (a > b) return 1; if (a === b) return 0; if (a < b) return -1; });

		for (let i = 0; i < pageArr.length; i++) {
			let pageIndex = pageArr[i];
			if (pageIndex === this.state.pageIndex) {
				ui.push(<li key={'pageIndex_' + (pageIndex)} className={newStyles.on}><a onClick={() => this.setPageIndex(pageIndex)}>{pageIndex}</a></li>);
			}
			else {
				ui.push(<li key={'pageIndex_' + (pageIndex)}><a onClick={() => this.setPageIndex(pageIndex)}>{pageIndex}</a></li>);
			}
		}

		return ui;
	}

	getGridData() {
		let ui = [];
		if (!this.state.dataSource) {
			return ui;
		}

		const dataSource = this.state.dataSource;
		const datacount = dataSource.length;

		// 데이터를 읽을 시작할 배열값
		let beginIndex = 0;
		if (this.state.pageIndex > 1) {
			beginIndex = (this.state.pageIndex * this.state.maxRowCount) - 1;
		}

		for (let i = beginIndex; i < beginIndex + this.state.maxRowCount; i++) {
			if (datacount < i + 1) {
				break;
			}

			ui.push(<tr key={'dataSource_' + (i)}>
				<td><input type="checkbox" /></td>
				<td>{i + 1}</td>
				<td>{dataSource[i].disasterName}</td>
				<td>{dataSource[i].sopName}</td>
				<td>{dataSource[i].actionStepName}</td>
				<td>{dataSource[i].realMode}</td>
				<td>{(!dataSource[i].sensorName || dataSource[i].sensorName.length === 0) ? '-' : dataSource[i].sensorName}</td>
				<td>{dataSource[i].position}</td>
				<td>{dataSource[i].beginTime}</td>
				<td>{dataSource[i].endTime}</td>
				<td>{dataSource[i].userName}</td>
				<td><a href="#">상세정보</a></td>
			</tr>);
		}

		return ui;
	}

	render() {
		const pageIndexUI = this.getPageIndexUI();
		const gridUI = this.getGridData();

		return (
			<>
				<div id={uneStyles.hsback}>
					<div id={uneStyles.hsty}>

						<div id={newStyles.hsLft}>
							<ul className={newStyles.hslMenu}>								
								<li><a onClick={() => this.props.changeContent(HistoryResource.ID.menu.sensorDetectHistory)}>센서 감지 이력</a></li>
								<li><a onClick={() => this.props.changeContent(HistoryResource.ID.menu.sensorDetectAnalysis)}>센서 감지 분석</a></li>
								<li><a onClick={() => this.props.changeContent(HistoryResource.ID.menu.sopHistory)}>SOP 이력</a></li>
								<li><a onClick={() => this.props.changeContent(HistoryResource.ID.menu.userHistory)}>데이터 수정 이력</a></li>
								<li><a onClick={() => this.props.changeContent(HistoryResource.ID.menu.spreadHistory)} className={newStyles.on}>상황전파 이력</a></li>
							</ul>
						</div>

						<div className={newStyles.hsScr + " " + uneStyles.hsScr}>
							<div id={newStyles.hsCont}>
								<form action="">
									<div className={newStyles.hscSch}>
										<dl>
											<dt>조회기간</dt>
											<dd>
												<ul className={newStyles.hscsDate}>
													<li>
														<div className={newStyles.datepicker}>
															<datepicker type="text" name="" id="dateStart" />
															<label htmlFor="dateStart">달력</label>
														</div>
													</li>
													<li>~</li>
													<li>
														<div className={newStyles.datepicker}>
															<datepicker type="text" name="" id="dateEnd" />
															<label htmlFor="dateEnd">달력</label>
														</div>
													</li>
												</ul>
												<ul className={newStyles.hscsRdo}>
													<li><input type="radio" name="hscsRdo" id="hscsRdo01" onClick={() => this.onClickDateType('today')} checked={this.state.dateType === 'today'} /><label htmlFor="hscsRdo01">오늘</label></li>
													<li><input type="radio" name="hscsRdo" id="hscsRdo02" onClick={() => this.onClickDateType('week')} checked={this.state.dateType === 'week'} /><label htmlFor="hscsRdo02">1주</label></li>
													<li><input type="radio" name="hscsRdo" id="hscsRdo03" onClick={() => this.onClickDateType('month')} checked={this.state.dateType === 'month'} /><label htmlFor="hscsRdo03">1개월</label></li>
													<li><input type="radio" name="hscsRdo" id="hscsRdo04" onClick={() => this.onClickDateType('year')} checked={this.state.dateType === 'year'} /><label htmlFor="hscsRdo04">1년</label></li>
												</ul>
											</dd>
										</dl>
										<dl>
											<dt>발신자</dt>
											<dd>
												<div className={newStyles.hscsIpt + " " + newDefault.pl0}>
													<input type="text" name="" id="" />
												</div>
											</dd>
										</dl>
										<a href="#" className={newStyles.hscsSbmt}><span><span>검색</span></span></a>
									</div>
								</form>

								<ul className={newStyles.hscExl}>
									<li><a href="#" className={newStyles.all}>전체 다운로드</a></li>
									<li><a href="#" className={newStyles.exl}>엑셀 다운로드</a></li>
								</ul>

								<div className={newStyles.hscTb}>
									<div className={newStyles.scrTb}>
										<table>
											<colgroup>
												<col style={{ width: '5%' }} />
												<col style={{ width: '5%' }} />
												<col style={{ width: '10%' }} />
												<col style={{ width: '10%' }} />
												<col style={{ width: '25%' }} />
												<col style={{ width: '35%' }} />
												<col style={{ width: '10%' }} />
											</colgroup>
											<thead>
												<tr>
													<th><input type="checkbox" /></th>
													<th>No.</th>
													<th>일시</th>
													<th>발신자</th>
													<th>수신 대상자</th>
													<th>내용</th>
													<th>재발송</th>
												</tr>
											</thead>
											<tbody>
												<tr>
													<td><input type="checkbox" /></td>
													<td>1</td>
													<td>210517 03:11:15</td>
													<td>홍길동</td>
													<td>안전관리팀(3), 총괄팀(5)</td>
													<td>T1-1 구역 화재발생, 비상발송을 실시합니다.</td>
													<td><a href="#">재발송</a></td>
												</tr>
												<tr>
													<td><input type="checkbox" /></td>
													<td>2</td>
													<td>210517 03:11:15</td>
													<td>홍길동</td>
													<td>안전관리팀(3), 총괄팀(5)</td>
													<td>T1-1 구역 화재발생, 비상발송을 실시합니다.</td>
													<td><a href="#">재발송</a></td>
												</tr>
												<tr>
													<td><input type="checkbox" /></td>
													<td>3</td>
													<td>210517 03:11:15</td>
													<td>홍길동</td>
													<td>안전관리팀(3), 총괄팀(5)</td>
													<td>T1-1 구역 화재발생, 비상발송을 실시합니다.</td>
													<td><a href="#">재발송</a></td>
												</tr>
												<tr>
													<td><input type="checkbox" /></td>
													<td>4</td>
													<td>210517 03:11:15</td>
													<td>홍길동</td>
													<td>안전관리팀(3), 총괄팀(5)</td>
													<td>T1-1 구역 화재발생, 비상발송을 실시합니다.</td>
													<td><a href="#">재발송</a></td>
												</tr>
												<tr>
													<td><input type="checkbox" /></td>
													<td>5</td>
													<td>210517 03:11:15</td>
													<td>홍길동</td>
													<td>안전관리팀(3), 총괄팀(5)</td>
													<td>T1-1 구역 화재발생, 비상발송을 실시합니다.</td>
													<td><a href="#">재발송</a></td>
												</tr>
												<tr>
													<td><input type="checkbox" /></td>
													<td>6</td>
													<td>210517 03:11:15</td>
													<td>홍길동</td>
													<td>안전관리팀(3), 총괄팀(5)</td>
													<td>T1-1 구역 화재발생, 비상발송을 실시합니다.</td>
													<td><a href="#">재발송</a></td>
												</tr>
												<tr>
													<td><input type="checkbox" /></td>
													<td>7</td>
													<td>210517 03:11:15</td>
													<td>홍길동</td>
													<td>안전관리팀(3), 총괄팀(5)</td>
													<td>T1-1 구역 화재발생, 비상발송을 실시합니다.</td>
													<td><a href="#">재발송</a></td>
												</tr>
												<tr>
													<td><input type="checkbox" /></td>
													<td>8</td>
													<td>210517 03:11:15</td>
													<td>홍길동</td>
													<td>안전관리팀(3), 총괄팀(5)</td>
													<td>T1-1 구역 화재발생, 비상발송을 실시합니다.</td>
													<td><a href="#">재발송</a></td>
												</tr>
												<tr>
													<td><input type="checkbox" /></td>
													<td>9</td>
													<td>210517 03:11:15</td>
													<td>홍길동</td>
													<td>안전관리팀(3), 총괄팀(5)</td>
													<td>T1-1 구역 화재발생, 비상발송을 실시합니다.</td>
													<td><a href="#">재발송</a></td>
												</tr>
												<tr>
													<td><input type="checkbox" /></td>
													<td>10</td>
													<td>210517 03:11:15</td>
													<td>홍길동</td>
													<td>안전관리팀(3), 총괄팀(5)</td>
													<td>T1-1 구역 화재발생, 비상발송을 실시합니다.</td>
													<td><a href="#">재발송</a></td>
												</tr>
											</tbody>
										</table>
									</div>
									{
										(this.state.dataSource && this.state.dataSource.length > 0) ?
											<div className={newStyles.hscNav}>
												<a className={newStyles.first} onClick={() => this.setPageIndex(1)}>맨앞</a>
												<a className={newStyles.prev} onClick={() => this.setPageIndex(this.state.pageIndex - 1)}>이전</a>
												<ul>
													{pageIndexUI}
												</ul>
												<a className={newStyles.next} onClick={() => this.setPageIndex(this.state.pageIndex + 1)}>다음</a>
												<a className={newStyles.last} onClick={() => this.setPageIndex(this.state.maxPageIndex)}>맨뒤</a>
											</div>
											: <> </>
									}
								</div>
							</div>
						</div>
					</div>
				</div>
			</>
		);
	}
} export default SpreadHistory;