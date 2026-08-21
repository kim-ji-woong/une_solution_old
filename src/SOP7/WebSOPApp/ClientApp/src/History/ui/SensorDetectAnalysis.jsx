import React, { Component } from 'react';
import $ from 'jquery';
import HistoryResource from "../resource/id";
import HistoryController from '../services/historyController';
import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';
import { ko } from 'date-fns/esm/locale';
import newStyles from '../../Common/css/newStyle.module.css';
import uneStyles from "../../Common/css/uneCommon.module.css";
import { Bar } from 'react-chartjs-2';
import btnCalendarBk from '../../Common/img/sub/dashboard_calendar_bk.png';

import CircularProgress from '@material-ui/core/CircularProgress';

import * as ExcelJS from 'exceljs'; /*excel 만들기*/
import { saveAs } from 'file-saver'; /*excel 다운로드*/

class SensorDetectHistory extends Component {

	constructor(props) {
		super(props);

		this.state = {
			content: HistoryResource.ID.menu.userHistory,

			selectedBuildingGroupID: -1,
			selectedBuildingID: -1,
			selectedZoneID: -1,
			searchZoneName: '-',

			facilityType: -1, // -1:전체, 0:화재, 11:누출, 900:CCTV
			dataSource: null,
			maxRowCount: 5,  // 한 페이지에 보여줄 data row 수
			maxPageCount: 5, // 한번에 보여줄 페이지 개수
			pageIndex: 1,    // 현재 페이지
			maxPageIndex: 1, // 최대 페이지 Index			
			dateType: 'today',
			beginDate: new Date(),
			endDate: new Date(),

			selectedDate: null,
			allDetectCount: 0,
			allMalfunctionRate: 0,
			maxCountSensorName: '',

			chartOptions: {
				responsive: true,
				maintainAspectRatio: false,
				//tooltips 사용시
				//tooltips: {
				//	enabled: true,
				//	mode: "nearest",
				//	position: "average",
				//	intersect: false,
				//},
				scales: {
					xAxes: [
						{
							//   position: "top", //default는 bottom
							display: true,
							//scaleLabel: {
							//	display: true,
							//	labelString: "Step",
							//	fontFamily: "Montserrat",
							//	fontColor: "black",
							//},
							ticks: {
								// beginAtZero: true,
								maxTicksLimit: 10, //x축에 표시할 최대 눈금 수
							},
						},
					],
					yAxes: [
						{
							id: 'A',
							display: true,
							position: 'left',
							//   padding: 10,						
							ticks: {
								beginAtZero: true,
								stepSize: 20
							},
						},
						{
							id: 'B',
							display: true,
							type: 'linear',
							position: 'right',
							//   padding: 10,
							//scaleLabel: {
							//	display: true,
							//	labelString: "Coverage",
							//	fontFamily: "Montserrat",
							//	fontColor: "black",
							//},
							ticks: {
								beginAtZero: true,
								stepSize: 25,
								min: 0,
								max: 100,
								//y축 scale 값에 % 붙이기 위해 사용
								callback: function (value) {
									return value + "%";
								},
							},
						},
					],
				},
			}, // chart 옵션
			chartLegend: {
				display: true,
				position: 'bottom',
				//align: 'start',
				position: 'top'
			},  // chart 옵션

			loadingIndicator: false,

			prevProps: null
		}

		this.refDatepicker01 = React.createRef();
		this.refDatepicker02 = React.createRef();

		this.props = props;
		this.display = this.display.bind(this);
		this.onClickDownload = this.onClickDownload.bind(this);
	}

	componentDidMount() {
		this.display();
	}

	async display() {
		$("body").css("cursor", "wait");

		const beginDate = this.getMakeDateTime(this.state.beginDate) + ' 00:00:00';
		const endDate = this.getMakeDateTime(this.state.endDate) + ' 23:59:59';

		if (beginDate > endDate) {
			$("body").css("cursor", "default");
			alert('조회 기간을 다시 선택하세요');
			return;
		}

		await this.setState({ loadingIndicator: true })

		const buildingGroupID = this.state.selectedBuildingGroupID;
		const buildingID = this.state.selectedBuildingID;
		const zoneID = this.state.selectedZoneID;

		const result = await HistoryController.DisplaySensorDetectAnalysis(beginDate, endDate, this.state.facilityType, buildingGroupID, buildingID, zoneID);
		if (!result) {
			$("body").css("cursor", "default");
			await this.setState({ loadingIndicator: false })			
			return;
        }

		const allDetectCount = result.allDetectCount;
		const allMalfunctionRate = result.allMalfunctionRate
		const maxCountSensorName = result.maxCountSensorName;
		const searchZoneName = result.searchZoneName;

		const dataSource = result.sensorDetectAnalysisDatas;
		const datacount = dataSource.length;
		const value1 = parseInt(datacount / this.state.maxRowCount);
		const value2 = datacount % this.state.maxRowCount; // 나머지가 있는 경우 페이지 하나를 추가한다.
		let maxPageIndex = value1 + ((value2 > 0) ? 1 : 0);

		const selectedDate = this.getMakeDateTime(this.state.beginDate) + ' ~ ' + this.getMakeDateTime(this.state.endDate);

		$("body").css("cursor", "default");

		this.setState({ dataSource, maxPageIndex, selectedDate, allDetectCount, allMalfunctionRate, maxCountSensorName, searchZoneName, pageIndex: 1, loadingIndicator: false });
	}

	getMakeDateTime(dateTime) {
		let year = dateTime.getFullYear();
		let month = 1 + dateTime.getMonth();
		month = month >= 10 ? month : '0' + month;  //month 두자리로 저장
		let day = dateTime.getDate();                   //d
		day = day >= 10 ? day : '0' + day;

		let strDate = year + '-' + month + '-' + day;
		return strDate;
	}
	getMakeTime(dateTime) {
		let hour = dateTime.getHours();
		hour = hour >= 10 ? hour : '0' + hour;
		let min = dateTime.getMinutes();
		min = min >= 10 ? min : '0' + min;
		let sec = dateTime.getSeconds();
		sec = sec >= 10 ? sec : '0' + sec;

		let strDate = hour + ':' + min + ':' + sec;
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

		this.setState({ dateType: 'select' });
	}

	onChangeEnd = (date) => {
		this.setState({ endDate: date });
		$("input:radio[name='stgDate']").prop('checked', false);

		let year = date.getFullYear();
		let month = date.getMonth() + 1;
		let day = date.getDate();

		let korFormat = year + "-" + month + "-" + day;

		this.setState({ dateType: 'select' });
	}

	onChangeBuildingGroup = (target) => {
		this.setState({ selectedBuildingGroupID: Number(target.value), selectedBuildingID: -1, selectedZoneID: -1  });
	}
	onChangeBuilding = (target) => {
		this.setState({ selectedBuildingID: Number(target.value), selectedZoneID: -1 });
	}
	onChangeZone = (target) => {
		this.setState({ selectedZoneID: Number(target.value) });
	}

	onClickDateType = (type) => {
		let dateType = '';

		if (type === 'select') {
			dateType = 'select';
			this.setState({ dateType: dateType });
			return;
		}

		let today = new Date();
		let date = new Date();

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

	async onClickDownload(isCheckedDownload) {
		const title = '센서 탐지 분석';

		const workbook = new ExcelJS.Workbook();
		const worksheet = workbook.addWorksheet(title); // sheet 이름

		// title		
		let titleRow = worksheet.getCell('A1');
		titleRow.value = title;

		titleRow.font = { name: '맑은 고딕', family: 4, size: 20, bold: true };
		worksheet.getCell('A1').alignment = { vertical: 'middle', horizontal: 'center' };

		worksheet.mergeCells('A1:H2');
		worksheet.getCell('A1:H2').border = {
			top: { style: 'thin' },
			left: { style: 'thin' },
			bottom: { style: 'thin' },
			right: { style: 'thin' }
		}

		const beginDate = this.getMakeDateTime(this.state.beginDate);
		const endDate = this.getMakeDateTime(this.state.endDate);
		worksheet.addRow(['조회기간 : ' + beginDate + ' ~ ' + endDate]);
		worksheet.addRow(['조회범위 : ' + this.state.searchZoneName]);
		worksheet.addRow([]);
		
		let content = this.state.selectedDate + '동안 ';
		content += this.state.searchZoneName + '의 센서 탐지 횟수는' + this.state.allDetectCount + '회 이며 ';
		content += '오작동률은 ' + this.state.allMalfunctionRate + ' % 입니다.'
		let content2 = '가장 많은 오작동을 일으킨 센서는 ' + ((this.state.maxCountSensorName && this.state.maxCountSensorName.length > 0) ? this.state.maxCountSensorName : '-') + '입니다.'

		worksheet.addRow([content]);
		worksheet.addRow([content2]);

		const chart = document.getElementById('chart_analysis2');
		let img = chart.toDataURL(1.0);
		let img2 = workbook.addImage({ base64: img, extension: 'png' });
		worksheet.addImage(img2, 'A9:H14');

		// 빈칸 10칸 띄우기
        for (let i = 0; i < 10; i++) {
			worksheet.addRow([]);
        }		

		// column
		let columnRow = worksheet.addRow(['No', '유형', '위치', '센서명', '탐지횟수', '오작동', '현장복구', '오작동률(%)']);
		columnRow.eachCell((cell, number) => {
			cell.fill = {
				type: 'pattern',
				pattern: 'solid',
				fgColor: { argb: '#A24B40' }
			};
			cell.style = {
				alignment: { vertical: 'middle', horizontal: 'center' }
			};
			cell.border = {
				top: { style: 'thin' },
				left: { style: 'thin' },
				bottom: { style: 'thin' },
				right: { style: 'thin' }
			}

		});

		// column key 설정 
		worksheet.columns = [
			{ key: "no", width: 5 },
			{ key: "type", width: 15 },
			{ key: "zoneName", width: 20 },
			{ key: "sensorName", width: 25 },
			{ key: "detectCount", width: 13 },
			{ key: "malfunctionCount", width: 13 },
			{ key: "endCount", width: 13 },
			{ key: "malfunctionRate", width: 13 }
		];

		if (this.state.dataSource) {
			let arrDatas = [];
			const dataLength = this.state.dataSource.length;
			for (let i = 0; i < dataLength; i++) {
				const data = [];

				const checked = this.state.dataSource[i].checked;
				if (isCheckedDownload && !checked) {
					continue;
				}

				const no = arrDatas.length + 1;
				const type = this.state.dataSource[i].type;
				const zoneName = this.state.dataSource[i].zoneName;
				const sensorName = this.state.dataSource[i].sensorName;
				const detectCount = this.state.dataSource[i].detectCount;
				const malfunctionCount = this.state.dataSource[i].malfunctionCount;
				const endCount = this.state.dataSource[i].endCount;
				const malfunctionRate = this.state.dataSource[i].malfunctionRate;

				data.no = no;
				data.type = type;
				data.zoneName = zoneName;
				data.sensorName = sensorName;
				data.detectCount = detectCount;
				data.malfunctionCount = malfunctionCount;
				data.endCount = endCount;
				data.malfunctionRate = malfunctionRate;

				arrDatas.push(data);
			}

			arrDatas.forEach(function (item, index) {
				worksheet.addRow({
					no: item.no,
					type: item.type,
					zoneName: item.zoneName,
					sensorName: item.sensorName,
					detectCount: item.detectCount,
					malfunctionCount: item.malfunctionCount,
					endCount: item.endCount,
					malfunctionRate: item.malfunctionRate
				}).alignment = { vertical: 'middle', horizontal: 'center' };
			})
		}

		// 다운로드 
		const mimeType = { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" };
		const buffer = await workbook.xlsx.writeBuffer();
		const blob = new Blob([buffer], mimeType);

		const dtNow = new Date();
		const date = this.getMakeDateTime(dtNow).replace(/-/gi, '');
		const time = this.getMakeTime(dtNow).replace(/:/gi, '');

		saveAs(blob, title + '_' + date + '_' + time + ".xlsx");
	}

	onClickFacilityType = (facilityType) => {
		this.setState({ facilityType });
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

	onCheckedRow(checked, index) {
		const dataSource = this.state.dataSource;
		const datacount = dataSource.length;
		if (index === -1) {
			let beginIndex = 0;
			if (this.state.pageIndex > 1) {
				beginIndex = (this.state.pageIndex - 1) * this.state.maxRowCount;
			}

			for (let i = beginIndex; i < beginIndex + this.state.maxRowCount; i++) {
				if (datacount < i + 1) {
					break;
				}
				dataSource[i].checked = checked;
			}
		}
		else {			
			dataSource[index].checked = checked;
		}
		this.setState({ dataSource });
	}

	getGridData() {
		let gridUI = [];
		let chartUI = [];
		if (!this.state.dataSource) {
			return [gridUI, chartUI, false];
		}

		const dataTemp = {};
		dataTemp.labels = [];
		dataTemp.detectCount = [];
		dataTemp.detectRate = [];

		let allDetectCount = 0;
		let allDetectRate = 0;
		const dataSource = this.state.dataSource;
		const datacount = dataSource.length;

		let rowCount = 0;
		let allChecked = true; // 전체 체크 여부

		// 데이터를 읽을 시작할 배열값
		let beginIndex = 0;
		if (this.state.pageIndex > 1) {
			beginIndex = (this.state.pageIndex - 1) * this.state.maxRowCount;
		}

		for (let i = beginIndex; i < beginIndex + this.state.maxRowCount; i++) {
			if (datacount < i + 1) {
				break;
			}

			gridUI.push(<tr key={'dataSource_' + (i)}>
				<td><input type="checkbox" checked={dataSource[i].checked} onChange={(e) => this.onCheckedRow(e.target.checked, i)} /></td>
				<td>{i + 1}</td>
				<td>{dataSource[i].type}</td>
				<td>{dataSource[i].zoneName}</td>
				<td>{dataSource[i].sensorName}</td>
				<td>{dataSource[i].detectCount}</td>
				<td>{dataSource[i].malfunctionCount}</td>
				<td>{dataSource[i].endCount}</td>
				<td>{dataSource[i].userResetCount}</td>
				<td>{dataSource[i].malfunctionRate}%</td>
			</tr>);

			dataTemp.labels.push(dataSource[i].sensorName);
			dataTemp.detectCount.push(dataSource[i].detectCount);
			dataTemp.detectRate.push(dataSource[i].detectRate);

			//allDetectCount += dataSource[i].detectCount;

			rowCount++;
			if (allChecked && !dataSource[i].checked) {
				allChecked = false;
			}
		}

		//for (let i = 0; i < dataTemp.detectCount.length; i++) {
		//	allDetectRate += (dataTemp.detectCount[i] / this.state.allDetectCount) * 100;
		//	dataTemp.detectRate.push(allDetectRate);
		//}

		if (rowCount === 0 && allChecked) {
			allChecked = false;
		}

		const data = {
			labels: dataTemp.labels,
			datasets: [
				{
					label: '탐지 횟수',
					data: dataTemp.detectCount,
					backgroundColor: 'rgba(219, 0, 0, 0.5)',
					fill: false,
					barThickness: 40,
					pointStyle: 'rectRounded',
					yAxisID: 'A'
				},
				{
					type: 'line',
					label: "누적 탐지율",
					data: dataTemp.detectRate,
					//backgroundColor: 'rgba(247, 169, 43, 0.8)',
					borderColor: 'rgba(247, 169, 43, 0.8)',
					//borderDash: [5, 5],
					//backgroundColor: "#e755ba",
					//pointBackgroundColor: "#55bae7",
					pointBorderColor: 'rgb(247, 169, 43)',
					pointRadius: 10, // 포인트 사이즈
					pointHoverRadius: 10, // 포인트 호버 사이즈
					//pointHoverBackgroundColor: "#55bae7",
					//pointHoverBorderColor: "#55bae7",
					fill: false,
					pointStyle: 'triangle',
					yAxisID: 'B'
				}
			]
		};

		chartUI.push(<Bar key={'chart_analysis2'} id='chart_analysis2' data={data} legend={this.state.chartLegend} options={this.state.chartOptions} />)

		return [gridUI, chartUI, allChecked];
	}

	getSpatailUI() {
		let buildingGroupUI = [];
		let buildingUI = [];
		let zoneUI = [];

		buildingGroupUI.push(<option key={'buildingGroupOption_-1'} value="-1">전체</option>);
		buildingUI.push(<option key={'buildingOption_-1'} value="-1">전체</option>);
		zoneUI.push(<option key={'zoneOption_-1'} value="-1">전체</option>);

		if (!this.props.buildingGroupList) {
			return [buildingGroupUI, buildingUI, zoneUI];
		}

		const buildingGroupLength = this.props.buildingGroupList.length;
		for (let i = 0; i < buildingGroupLength; i++) {
			const buildingGroup = this.props.buildingGroupList[i];

			if (this.state.selectedBuildingGroupID === buildingGroup.id) {
				buildingGroupUI.push(<option key={'buildingGroupOption_' + buildingGroup.id} value={buildingGroup.id} selected>{buildingGroup.displayText}</option>);

				const buildingLength = buildingGroup.buildingDatas.length;
				for (let j = 0; j < buildingLength; j++) {
					const building = buildingGroup.buildingDatas[j];					
					if (this.state.selectedBuildingID === building.id) {
						buildingUI.push(<option key={'buildingOption_' + building.id} value={building.id} selected>{building.displayText}</option>);

						const zoneLength = building.zoneDatas.length;
						for (var k = 0; k < zoneLength; k++) {
							const zone = building.zoneDatas[k];
							if (this.state.selectedZoneID === zone.id) {
								zoneUI.push(<option key={'zoneOption_' + zone.id} value={zone.id} selected>{zone.displayText}</option>);
							}
							else {
								zoneUI.push(<option key={'zoneOption_' + zone.id} value={zone.id}>{zone.displayText}</option>);
                            }
						}
					}
					else {
						buildingUI.push(<option key={'buildingOption_' + building.id} value={building.id}>{building.displayText}</option>);
                    }
				}
			}
			else {
				buildingGroupUI.push(<option key={'buildingGroupOption_' + buildingGroup.id} value={buildingGroup.id}>{buildingGroup.displayText}</option>);
            }
        }

		return [buildingGroupUI, buildingUI, zoneUI];
    }

	onClickDatepicker01 = () => {
		this.refDatepicker01.current.setOpen(true);
	}

	onClickDatepicker02 = () => {
		this.refDatepicker02.current.setOpen(true);
	}

	render() {
		const [buildingGroupUI, buildingUI, zoneUI] = this.getSpatailUI();
		const pageIndexUI = this.getPageIndexUI();
		const [gridUI, chartUI, allChecked] = this.getGridData();
		return (
			<>
				<div id={uneStyles.hsback}>
					<div id={uneStyles.hsty}>

						<div id={newStyles.hsLft}>
							<ul className={newStyles.hslMenu}>								
								<li><a onClick={() => this.props.changeContent(HistoryResource.ID.menu.sensorDetectHistory)}>센서 탐지 이력</a></li>
								<li><a onClick={() => this.props.changeContent(HistoryResource.ID.menu.sensorDetectAnalysis)} className={newStyles.on}>센서 탐지 분석</a></li>
								<li><a onClick={() => this.props.changeContent(HistoryResource.ID.menu.sopHistory)}>SOP 이력</a></li>
								<li><a onClick={() => this.props.changeContent(HistoryResource.ID.menu.userHistory)}>데이터 수정 이력</a></li>
								{/*<li><a onClick={() => this.props.changeContent(HistoryResource.ID.menu.spreadHistory)}>상황전파 이력</a></li>*/}
							</ul>
						</div>

						<div className={newStyles.hsScr + " " + uneStyles.hsScr}>
							<div id={newStyles.hsCont}>
								<form action="">
									<div className={newStyles.hscSch}>
										<dl>
											<dt>재난타입</dt>
											<dd>
												<ul className={newStyles.hscsRdo}>
													<li><input type="radio" name="hscsType" id="hscsType01" onChange={() => this.onClickFacilityType(-1)} checked={this.state.facilityType === -1} /> <label htmlFor="hscsType01">전체</label></li>
													<li><input type="radio" name="hscsType" id="hscsType02" onChange={() => this.onClickFacilityType(0)} checked={this.state.facilityType === 0}/><label htmlFor="hscsType02">화재</label></li>													
													<li><input type="radio" name="hscsType" id="hscsType03" onChange={() => this.onClickFacilityType(900)} checked={this.state.facilityType === 900}/><label htmlFor="hscsType03">지능형 영상감시</label></li>
													<li><input type="radio" name="hscsType" id="hscsType04" onChange={() => this.onClickFacilityType(11)} checked={this.state.facilityType === 11} /><label htmlFor="hscsType04">누출센서</label></li>
													<li><input type="radio" name="hscsType" id="hscsType05" onChange={() => this.onClickFacilityType(21)} checked={this.state.facilityType === 21} /><label htmlFor="hscsType05">IoT센서</label></li>
												</ul>
											</dd>
										</dl>
										<dl>
											<dt>위치</dt>
											<dd>
												<ul className={newStyles.hscsLoc}>
													<li>
														<select name="" id="" onChange={(e) => this.onChangeBuildingGroup(e.target)} className={newStyles.selWh}>
															{buildingGroupUI}
														</select>
													</li>
													<li>
														<select name="" id="" onChange={(e) => this.onChangeBuilding(e.target)} className={newStyles.selWh}>
															{buildingUI}
														</select>
													</li>
													<li>
														<select name="" id="" onChange={(e) => this.onChangeZone(e.target)} className={newStyles.selWh}>
															{zoneUI}
														</select>
													</li>
												</ul>
											</dd>
										</dl>
										<dl>
											<dt>조회기간</dt>
											<dd>
												<ul className={newStyles.hscsDate}>
													<li>
														<div className={newStyles.datepicker}>
															<DatePicker ref={this.refDatepicker01} name="datepicker01" id="datepicker01"
																dateFormat="yyyy-MM-dd"
																locale={ko}
																maxDate={new Date()}
																selected={this.state.beginDate}
																onChange={date => this.onChangeBegin(date)} />
															<img src={btnCalendarBk} alt="" className={newStyles.btnCalendarBk} onClick={this.onClickDatepicker01} />
														</div>
													</li>
													<li>~</li>
													<li>
														<div className={newStyles.datepicker}>
															<DatePicker ref={this.refDatepicker02} name="datepicker02" id="datepicker02"
																dateFormat="yyyy-MM-dd"
																locale={ko}
																maxDate={new Date()}
																selected={this.state.endDate}
																onChange={date => this.onChangeEnd(date)} />
															<img src={btnCalendarBk} alt="" className={newStyles.btnCalendarBk} onClick={this.onClickDatepicker02} />
														</div>
													</li>
												</ul>
												<ul className={newStyles.hscsRdo}>
													<li><input type="radio" name="hscsRdo" id="hscsRdo01" onChange={() => this.onClickDateType('select')} checked={this.state.dateType === 'select'} /><label htmlFor="hscsRdo01">기간선택</label></li>
													<li><input type="radio" name="hscsRdo" id="hscsRdo02" onChange={() => this.onClickDateType('today')} checked={this.state.dateType === 'today'} /><label htmlFor="hscsRdo02">오늘</label></li>
													<li><input type="radio" name="hscsRdo" id="hscsRdo03" onChange={() => this.onClickDateType('week')} checked={this.state.dateType === 'week'} /><label htmlFor="hscsRdo03">1주</label></li>
													<li><input type="radio" name="hscsRdo" id="hscsRdo04" onChange={() => this.onClickDateType('month')} checked={this.state.dateType === 'month'} /><label htmlFor="hscsRdo04">1개월</label></li>
													<li><input type="radio" name="hscsRdo" id="hscsRdo05" onChange={() => this.onClickDateType('year')} checked={this.state.dateType === 'year'} /><label htmlFor="hscsRdo05">1년</label></li>
												</ul>
											</dd>
										</dl>
										{
											this.state.loadingIndicator === true ?
												<a className={newStyles.hscsSbmt} id={newStyles.hscsSbmting}><span><span><CircularProgress className="spinner" /></span></span></a>
												:
												<a onClick={this.display} className={newStyles.hscsSbmt}><span><span>검색</span></span></a>
										}
									</div>
								</form>

								{
									this.state.loadingIndicator === true ?
										<ul className={newStyles.hscExl}>
											<li><a className={newStyles.all} id={newStyles.hscsSbmting}>전체 다운로드</a></li>
											<li><a className={newStyles.exl} id={newStyles.hscsSbmting}>선택 다운로드</a></li>
										</ul>
										:
										<ul className={newStyles.hscExl}>
											<li><a onClick={() => this.onClickDownload(false)} className={newStyles.all}>전체 다운로드</a></li>
											<li><a onClick={() => this.onClickDownload(true)} className={newStyles.exl}>선택 다운로드</a></li>
										</ul>
								}

								<p className={newStyles.hscWng}><span>{this.state.selectedDate}</span> 동안 <span>{this.state.searchZoneName}</span>의 센서 탐지 횟수는 <span>{this.state.allDetectCount}</span>회 이며 오작동률은 <span>{this.state.allMalfunctionRate}%</span> 입니다.
									가장 많은 오작동을 일으킨 센서는 <span>{(this.state.maxCountSensorName && this.state.maxCountSensorName.length > 0) ? this.state.maxCountSensorName : '-'}</span> 입니다.</p>

								<div className={newStyles.hscCht} id='chart_analysis'>
									{chartUI}
								</div>

								<div className={newStyles.hscTb}>
									<div className={newStyles.scrTb}>
										<table>
											<colgroup>
												<col style={{ width: '5%' }} />
												<col style={{ width: '5%' }} />
												<col style={{ width: '10%' }} />
												<col style={{ width: '10%' }} />
												<col style={{ width: '30%' }} />
												<col style={{ width: '8%' }} />
												<col style={{ width: '8%' }} />
												<col style={{ width: '8%' }} />
												<col style={{ width: '8%' }} />
												<col style={{ width: '8%' }} />
											</colgroup>
											<thead>
												<tr>
													<th><input type="checkbox" checked={allChecked} onChange={(e) => this.onCheckedRow(e.target.checked, -1)}/></th>
													<th>No.</th>
													<th>유형</th>
													<th>위치</th>
													<th>센서명</th>
													<th>탐지횟수</th>
													<th>오작동</th>
													<th>현장복구</th>
													<th>사용자복구</th>
													<th>오작동률(%)</th>
												</tr>
											</thead>
											<tbody>												
												{gridUI}
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
}export default SensorDetectHistory;
//npm install --save react-chartjs-2 chart.js