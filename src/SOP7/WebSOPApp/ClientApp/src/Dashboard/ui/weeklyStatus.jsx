import React, { Component } from 'react';
import $ from 'jquery';
import { Line } from 'react-chartjs-2';

import { DashboardController } from '../services/dashboardController';

import dashboard from '../css/dashboardNew.module.css';

import DashboardResource from '../resource/id';
import SDMSResource from '../../SDMS/resource/id';
import ProjectResource from '../../Root/resource/id';


class WeeklyStatus extends Component {
    constructor(props) {
        super(props);

        this.state = {
            labels: null,
            sensorData: null,           

            //sensorZoneHistorys: null,
        }

        this.props = props;

        //this.getSensorZoneHistorys();
        //this.getDate();
    }


    componentDidMount() {
        //this.getDate();
        const chart = document.getElementById('lineChart');
    }

    //async getSensorZoneHistorys() {
    //    const [sensorZoneHistorys, message] = await DashboardController.requestWeeklyStatus();

    //    if (sensorZoneHistorys === null || sensorZoneHistorys === undefined)
    //        return;

    //    this.setState({ sensorZoneHistorys: sensorZoneHistorys });
    //}

    getTodayData() {
        const type = this.props.type;
        let today = 0;

        if (this.props.todayAlarms === null || this.props.todayAlarms === undefined)
            return today;

        let todayAlarms = this.props.todayAlarms;
        

        for (let i = 0; i < todayAlarms.length; i++) {
            let alarm = todayAlarms[i];
            let facilityType = alarm.facilityType;

            if (type === DashboardResource.displayInfoType.FIRE && facilityType === SDMSResource.facilityType.FIRE) {
                today++;
            } else if (type === DashboardResource.displayInfoType.INTELLIGENT && SDMSResource.isSVMSSensorType(facilityType)) {
                today++;
            } else if (type === DashboardResource.displayInfoType.PSM && SDMSResource.isPSMSensorType(facilityType)) {
                today++;
            } else if (type === DashboardResource.displayInfoType.ETC && SDMSResource.isETCSensorType(facilityType)) {
                today++;
            }

            // .TODO: safety 카운팅
        }

        return today;
    }

    getSensorData() {
        const type = this.props.type;
        //const sensorZoneHistorys = this.state.sensorZoneHistorys;
        const weeklyAlarms = this.props.weeklyAlarms;

        let nToday = this.getTodayData();
        let nOne = 0;
        let nTwo = 0;
        let nThree = 0;
        let nFour = 0;
        let nFive = 0;
        let nSix = 0;

        if (weeklyAlarms === null || weeklyAlarms === undefined || weeklyAlarms.length === 0)
            return [nSix, nFive, nFour, nThree, nTwo, nOne, nToday];

        let dtToday = new Date();
        let dtOne = new Date();
        dtOne.setDate(dtToday.getDate() - 1);
        let dtTwo = new Date();
        dtTwo.setDate(dtToday.getDate() - 2);
        let dtThree = new Date();
        dtThree.setDate(dtToday.getDate() - 3);
        let dtFour = new Date();
        dtFour.setDate(dtToday.getDate() - 4);
        let dtFive = new Date();
        dtFive.setDate(dtToday.getDate() - 5);
        let dtSix = new Date();
        dtSix.setDate(dtToday.getDate() - 6);


        for (let i = 0; i < weeklyAlarms.length; i++) {
            let weeklyAlarm = weeklyAlarms[i];

            let sensorType = weeklyAlarm.facilityType;
            let time = weeklyAlarm.time;
            let zoneID = weeklyAlarm.zoneID;      // 추후 공장별 구분

            if ((type === DashboardResource.displayInfoType.FIRE && sensorType === SDMSResource.facilityType.FIRE) ||
                (type === DashboardResource.displayInfoType.INTELLIGENT && SDMSResource.isSVMSSensorType(sensorType)) ||
                (type === DashboardResource.displayInfoType.PSM && SDMSResource.isPSMSensorType(sensorType)) ||
                (type === DashboardResource.displayInfoType.ETC && SDMSResource.isETCSensorType(sensorType)) ) {

                if (time === null || time === undefined)
                    continue;

                let date = new Date(time);
                let temp = dtOne.getDate();

                if (date.getDate() === dtOne.getDate()) {
                    nOne++;
                } else if (date.getDate() === dtTwo.getDate()) {
                    nTwo++;
                } else if (date.getDate() === dtThree.getDate()) {
                    nThree++;
                } else if (date.getDate() === dtFour.getDate()) {
                    nFour++;
                } else if (date.getDate() === dtFive.getDate()) {
                    nFive++;
                } else if (date.getDate() === dtSix.getDate()) {
                    nSix++;
                }
            } else if (type === DashboardResource.displayInfoType.SAFETY_EYE) {
                // .TODO: 세이프티 카운팅 필요
                continue;
            } else
                continue;
        }

        return [nSix, nFive, nFour, nThree, nTwo, nOne, nToday];
    }

    getTypeName = () => {
        const type = this.props.type;
        let typeName = "-";

        if (type === null || type === undefined)
            return typeName;

        typeName = DashboardResource.displayInfoTypeName(type);

        return typeName;
    }

    getDate() {
        let dt = new Date();
        const arrDayStr = ['일', '월', '화', '수', '목', '금', '토'];

        let strToday = (dt.getMonth() + 1) + "/" + dt.getDate() + " (" + arrDayStr[dt.getDay()] + ")";
        dt.setDate(dt.getDate() - 1);
        let strOne = (dt.getMonth() + 1) + "/" + dt.getDate() + " (" + arrDayStr[dt.getDay()] + ")";
        dt.setDate(dt.getDate() - 1);
        let strTwo = (dt.getMonth() + 1) + "/" + dt.getDate() + " (" + arrDayStr[dt.getDay()] + ")";
        dt.setDate(dt.getDate() - 1);
        let strThree = (dt.getMonth() + 1) + "/" + dt.getDate() + " (" + arrDayStr[dt.getDay()] + ")";
        dt.setDate(dt.getDate() - 1);
        let strFour = (dt.getMonth() + 1) + "/" + dt.getDate() + " (" + arrDayStr[dt.getDay()] + ")";
        dt.setDate(dt.getDate() - 1);
        let strFive = (dt.getMonth() + 1) + "/" + dt.getDate() + " (" + arrDayStr[dt.getDay()] + ")";
        dt.setDate(dt.getDate() - 1);
        let strSix = (dt.getMonth() + 1) + "/" + dt.getDate() + " (" + arrDayStr[dt.getDay()] + ")";

        let labels = [strSix, strFive, strFour, strThree, strTwo, strOne, strToday];

        //this.setState({ labels: labels});
        //this.state.labels = labels;
        return labels;
    }

    getMaxSensorCount(sensorData) {
        let maxCount = 10;      // 기본값

        if (sensorData === null || sensorData === undefined)
            return maxCount;

        for (let i = 0; i < sensorData.length; i++) {
            let sensorCount = sensorData[i];

            if (sensorCount > maxCount)
                maxCount = sensorCount;
        }

        let temp = maxCount % 10;

        if (temp > 0 && maxCount > 10)
            maxCount = maxCount + 10;

        temp = maxCount / 10;

        if (temp !== 0)
            temp = Math.floor(temp);

        maxCount = temp * 10;

        return maxCount;
    }

    getLineData() {
        let chartUI = [];
        //let labels = ['-', '-', '-', '-', '-', '-', '-'];
        let typeName = this.getTypeName();                     
        let sensorData = this.getSensorData();
        let maxCount = this.getMaxSensorCount(sensorData);

        //if (this.state.labels !== null && this.state.labels !== undefined)
        //    labels = this.state.labels;
        const labels = this.getDate();

        // 그래프 데이터
        const data = {
            labels: labels,
            datasets: [
                {
                    type: 'line',
                    label: typeName,
                    data: sensorData,
                    lineTension: 0,
                    borderColor: "#1465EF",
                    borderWidth: 3,
                    fill: true,
                },
            ],
        };

        // 그래프 설정 값
        const chartOptions = {
            responsive: true,  // 컨테이너가 수행 할 때 차트 캔버스의 크기를 조정(dafalut : true)
            responsiveAnimationDuration: 1000,  // 크기 조정 이벤트 후 새 크기로 애니메이션하는 데 걸리는 시간(밀리 초) (defalut : 0)
            maintainAspectRatio: false,  // (width / height) 크기를 조정할 떄 원래 캔버스 종횡비를 유지 (defalut : true)
            //aspectRatio: 2,  // 캔버스 종횡비( width / height, 정사각형 캔버스를 나타내는 값) 높이가 속성으로 또는 스타일 통해 명시적으로 정의된 경우이 옵션은 무시
            //tooltips 사용시
            tooltips: {
                enabled: true,
                mode: "nearest",
                position: "average",
                intersect: false,
            },
            hover: {
                mode: 'nearest',
                intersect: true
            },
            scales: {
                xAxes: [
                    {
                        //position: "top", //default는 bottom
                        display: true,
                        scaleLabel: {
                            display: true,
                            fontFamily: "Montserrat",
                            fontColor: "rgb(255,255,255)",
                        },
                        ticks: {
                            //beginAtZero: true,
                            maxTicksLimit: 7,                   // 표시할 최대 눈금 수
                            //color: "rgb(190,190,190)",
                            fontColor: "rgb(213,214,214)",      // 눈금 텍스트 컬러
                            //fontSize: 25,                     // 눈금 텍스트 사이즈
                        },
                        gridLines: {
                            color: "rgb(57,72,81)",             // 눈금 라인 컬러
                            //lineWidth: 3                      // 눈금 라인 두께
                        },
                    },
                ],
                yAxes: [
                    {
                        display: true,
                        //   padding: 10,
                        scaleLabel: {
                            display: true,
                            fontFamily: "Montserrat",
                            fontColor: "rgb(190,190,190)",
                        },
                        ticks: {
                            beginAtZero: true,
                            //stepSize: 100,
                            maxTicksLimit: 6,       
                            min: 0,
                            max: maxCount,
                            fontColor: "rgb(213,214,214)",

                        },
                        gridLines: {
                            color: "rgb(57,72,81)",
                        },
                    },
                ],
            },
        };

        const chartLegend = {
            display: false,
            labels: {
                fontColor: "rgb(255,255,255)",
            },
            position: "top", //label를 넣어주지 않으면 position이 먹히지 않음
        };

        chartUI.push(<Line key={'lineChart'} id='lineChart' data={data} legend={chartLegend} options={chartOptions} />)
        return [chartUI];
    }

    getBtnUI = (siteID) => {
        const type = this.props.type;
        let btnUI = [];

        let fireBtnClass = dashboard.iconFire;
        let cctvBtnClass = dashboard.iconCCTV; 
        let psmBtnClass = dashboard.iconIOT;
        let safetyBtnClass = dashboard.iconSafety;
        let etcBtnClass = dashboard.iconETC;

        if (type === DashboardResource.displayInfoType.FIRE) {
            fireBtnClass = dashboard.iconFireAct;
        } else if (type === DashboardResource.displayInfoType.INTELLIGENT) {
            cctvBtnClass = dashboard.iconCCTVAct;
        } else if (type === DashboardResource.displayInfoType.PSM) {
            psmBtnClass = dashboard.iconIOTAct;
        } else if (type === DashboardResource.displayInfoType.SAFETY_EYE) {
            safetyBtnClass = dashboard.iconSafetyAct;
        } else if (type === DashboardResource.displayInfoType.ETC) {
            etcBtnClass = dashboard.iconETCAct;
        }

        if (siteID === ProjectResource.Site.GCC) {
            btnUI.push(
                /* 녹십자 */
               <ul>
                    <li onClick={() => this.props.changeType(DashboardResource.displayInfoType.FIRE)} className={fireBtnClass}><p>화재</p></li>
                    <li onClick={() => this.props.changeType(DashboardResource.displayInfoType.PSM)} className={psmBtnClass}><p>누출</p></li>
                    <li onClick={() => this.props.changeType(DashboardResource.displayInfoType.INTELLIGENT)} className={cctvBtnClass}><p>CCTV</p></li>
               </ul > 
            );
        } else {
            btnUI.push(
                /* 솔브레인 */
                <ul>
                    <li onClick={() => this.props.changeType(DashboardResource.displayInfoType.FIRE)} className={fireBtnClass}><p>화재</p></li>
                    <li onClick={() => this.props.changeType(DashboardResource.displayInfoType.PSM)} className={psmBtnClass}><p>누출</p></li>
                    <li onClick={() => this.props.changeType(DashboardResource.displayInfoType.ETC)} className={etcBtnClass}><p>ETC</p></li>
                    <li onClick={() => this.props.changeType(DashboardResource.displayInfoType.INTELLIGENT)} className={cctvBtnClass}><p>CCTV</p></li>
                    <li onClick={() => this.props.changeType(DashboardResource.displayInfoType.SAFETY_EYE)} className={safetyBtnClass}><p>S.I</p></li>
                </ul>
            );
        }

        return btnUI;
    }

    componentDidMount() {
        const resizableWeeklyStatusCloumn = "." + dashboard.weeklyStatus;
        const resizableWeeklyStatusRow = "." + dashboard.weeklyStatus;
        const weeklyStatus = this;

        $(function () {
            $(resizableWeeklyStatusCloumn).resizable({
               direction: 'left'
            });
        });
    }

    displaySiteUI = () => {
        const siteID = ProjectResource.SiteID;
        let displaySiteUI = [];
        const [chartUI] = this.getLineData();
        const [btnUI] = this.getBtnUI(siteID);

        if (siteID === ProjectResource.Site.GCC) {
            /* 녹십자 */
            displaySiteUI.push(
                <div className={dashboard.weeklyStatusGC}>
                    <div className={dashboard.weeklyTitle}>주간 현황</div>
                    <div className={dashboard.weekBox}>
                        <div className={dashboard.graph} id='chart_analysis'>
                            {chartUI}
                        </div>
                        <div className={dashboard.weekIconBoxGC}>
                            {btnUI}
                        </div>
                    </div>
                </div>
            );

        } else {
            /* 솔브레인 */
            displaySiteUI.push(
                <div className={dashboard.weeklyStatus}>
                    <div className={dashboard.weeklyTitle}>주간 현황</div>
                    <div className={dashboard.weekBox}>
                        <div className={dashboard.graph} id='chart_analysis'>
                            {chartUI}
                        </div>
                        <div className={dashboard.weekIconBox}>
                            {btnUI}
                        </div>
                    </div>
                </div>
            );
        }

        return displaySiteUI;
    }

    render() {
        const displaySiteUI = this.displaySiteUI();
        

        return (
            <>
                {/* 사이트별 UI */
                    displaySiteUI
                }
            </>
        );
    }
}
export default WeeklyStatus;