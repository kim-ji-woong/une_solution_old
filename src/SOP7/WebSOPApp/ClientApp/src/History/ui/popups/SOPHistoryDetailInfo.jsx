import { Button } from '@amcharts/amcharts4/core';
import React, { Component } from 'react';
import newStyles from "../../../Common/css/newStyle.module.css";
import HistoryController from '../../services/historyController';
import $ from 'jquery';

import * as ExcelJS from 'exceljs'; /*excel 만들기*/
import { saveAs } from 'file-saver'; /*excel 다운로드*/

class SOPHistoryDetailInfo extends Component {

	constructor(props) {
		super(props);

		this.state = {
			dataSource: null,
			selectComponentHistoryID: null
		}
		this.props = props;
		this.onClickAllDownload = this.onClickAllDownload.bind(this);
	}

	componentDidMount() {
		this.display();

		//$('.' + newStyles.hsmDtl).hide();
	}

	async display() {
		const dataSource = await HistoryController.DisplaySOPComponentHistories(this.props.selectedData.actionStepHistoryID);

		if (dataSource && dataSource.length > 0) {			
			this.setState({ dataSource: dataSource, selectComponentHistoryID: dataSource[0].componentHistoryID });

		}
		else {
			this.setState({ dataSource: dataSource });
        }
	}

	onClose = () => {
		this.props.changeSubContent(null);
    }

	onClickhsmDtl = (num, componentHistoryID) => {
		//$('.' + newStyles.hsmDtl).hide();
		//$('#' + newStyles.hsmDtl + num).show();

		this.setState({ selectComponentHistoryID: componentHistoryID });
	}

	async onClickAllDownload() {		
		const title = 'SOP 상세 이력';

		const workbook = new ExcelJS.Workbook();
		const worksheet = workbook.addWorksheet(title); // sheet 이름

		// title		
		let titleRow = worksheet.getCell('A1');
		titleRow.value = title;

		titleRow.font = { name: '맑은 고딕', family: 4, size: 20, bold: true };
		worksheet.getCell('A1').alignment = { vertical: 'middle', horizontal: 'center' };

		worksheet.mergeCells('A1:E2');
		worksheet.getCell('A1:E2').border = {
			top: { style: 'thin' },
			left: { style: 'thin' },
			bottom: { style: 'thin' },
			right: { style: 'thin' }
		}

		worksheet.addRow(['시간 : ' + this.props.selectedData.beginTime]);
		worksheet.addRow(['SOP유형 : ' + this.props.selectedData.disasterName ]);
		worksheet.addRow(['위기경보단계 : ' + this.props.selectedData.actionStepName]);
		worksheet.addRow(['SOP모드 : ' + this.props.selectedData.realMode]);
		worksheet.addRow([]);

		// column
		let columnRow = worksheet.addRow(['No', '프로세스 제목', '전파 대상자/전파 메시지', '시간', '완료 여부']);
		columnRow.eachCell((cell, number) => {
			cell.fill = {
				type: 'pattern',
				pattern: 'solid',
				fgColor: { argb: '#0595D5' }
			};
			cell.font = { name: '맑은 고딕', family: 4, size: 20, bold: true };
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
			{ key: "sectionName", width: 15 },
			{ key: "msg", width: 40 },
			{ key: "time", width: 20 },
			{ key: "status", width: 13 }
		];

		if (this.state.dataSource) {
			let arrDatas = [];
			const dataLength = this.state.dataSource.length;
			for (let i = 0; i < dataLength; i++) {
				const data = [];

				const no = i + 1;
				const sectionName = this.state.dataSource[i].sectionName;
				const msg = this.state.dataSource[i].teamList.join(', ');
				const time = this.state.dataSource[i].time;
				const status = this.state.dataSource[i].strStatus;

				data.no = no;
				data.sectionName = sectionName;
				data.msg = msg;
				data.time = time;
				data.status = status;

				arrDatas.push(data);

				// 세부
				const missionDatas = this.state.dataSource[i].missionDatas;
				const missionCount = missionDatas.length;

				if (missionCount > 0) {
					for (let j = 0; j < missionCount; j++) {
						const data2 = [];

						const noDetail = (i + 1) + '-' + (j + 1);
						const sectionNameDetail = this.state.dataSource[i].missionDatas[j].sectionName;
						const msgDetail = this.state.dataSource[i].missionDatas[j].missionText;
						const timeDetail = this.state.dataSource[i].missionDatas[j].time;
						const statusDetail = this.state.dataSource[i].missionDatas[j].completion;

						data2.no = noDetail;
						data2.sectionName = sectionNameDetail;
						data2.msg = msgDetail;
						data2.time = timeDetail;
						data2.status = statusDetail;

						arrDatas.push(data2);
                    }
                }
			}

			arrDatas.forEach(function (item, index) {
				worksheet.addRow({
					no: item.no,
					sectionName: item.sectionName,
					msg: item.msg,
					time: item.time,
					status: item.status
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

	getGridData() {
		let grid1 = [];
		let grid2 = [];

		if (!this.state.dataSource) {
			return [grid1, grid2];
		}

		const dataSource = this.state.dataSource;
		const datacount = dataSource.length;

		for (let j = 0; j < datacount; j++) {
			grid1.push(
				<tr onClick={() => this.onClickhsmDtl(j + 1, dataSource[j].componentHistoryID)} key={'grid1_' + j}>
					<td>{j + 1}</td>
					<td>{dataSource[j].sectionName}</td>
					<td>{dataSource[j].teamList.join(', ')}</td>
					<td>{dataSource[j].time}</td>
					<td>{dataSource[j].strStatus}</td>
				</tr>
			);

			if (this.state.selectComponentHistoryID === dataSource[j].componentHistoryID) {
				const missionDatas = dataSource[j].missionDatas;
				const missionCount = missionDatas.length;

				for (let i = 0; i < missionCount; i++) {
					grid2.push(
						<tr key={'grid2_' + i}>
							<td>{i + 1}</td>
							<td>{missionDatas[i].sectionName}</td>
							<td>
								<div className={"scroll-wrapper" + " " + newStyles.hsmScr + " " + "scroll-bar"}>
									{missionDatas[i].missionText}
								</div>
							</td>
							<td>{missionDatas[i].time}</td>
							<td>{missionDatas[i].completion}</td>
						</tr>
					);
                }
            }
        }

		return [grid1, grid2];
    }

	render() {
		const [grid1, grid2] = this.getGridData();

		return (
			<div id={newStyles.hsMmo} className={newStyles.popup}>
				<div>
					<div>
						<div className={newStyles.hsmCont + " " + newStyles.sop}>
							<div className={newStyles.hsmTitle}>
								<h3>SOP 상세정보</h3>
								<a onClick={this.onClickAllDownload} className={newStyles.hsmExl}>엑셀 다운로드</a>
								<a onClick={this.onClose} className={newStyles.hsmCls}>닫기</a>
							</div>
							<div className={"scroll-wrapper" + " " + newStyles.hsmPrc + " " + "scroll-bar"}>
								<table className={newStyles.hsmTb}>
									<colgroup>
										<col style={{ width: '5%' }} />
										<col style={{ width: '20%' }} />
										<col style={{ width: '45%' }} />
										<col style={{ width: '15%' }} />
										<col style={{ width: '15%' }} />
									</colgroup>
									<thead>
										<tr>
											<th>No.</th>
											<th>프로세스 제목</th>
											<th>전파 대상자</th>
											<th>시간</th>
											<th>완료여부</th>
										</tr>
									</thead>
									<tbody>
										{grid1}
									</tbody>
								</table>
							</div>
							<div className={"scroll-wrapper" + " " + newStyles.hsmDtl + " " + "scroll-bar"} id={newStyles.hsmDtl + "1"}>
								<table className={newStyles.hsmTb}>
									<colgroup>
										<col style={{ width: '5%' }} />
										<col style={{ width: '20%' }} />
										<col style={{ width: '45%' }} />
										<col style={{ width: '15%' }} />
										<col style={{ width: '15%' }} />
									</colgroup>
									<thead>
										<tr>
											<th>No.</th>
											<th>프로세스 제목</th>
											<th>세부 임무/전파 메시지</th>
											<th>시간</th>
											<th>완료여부</th>
										</tr>
									</thead>
									<tbody>
										{grid2}
									</tbody>
								</table>
							</div>
						</div>
					</div>
				</div>
			</div>
		);
	}
}

export default SOPHistoryDetailInfo;