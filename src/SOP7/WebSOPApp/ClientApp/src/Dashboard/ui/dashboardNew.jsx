import React, { Component } from 'react';
import $ from 'jquery';

import InfoHeader from './infoHeaderNew';
import { SDMSController } from '../../SDMS/services/sdmsController';
import { DashboardController } from '../services/dashboardController';
import store from '../../Root/store';
import DashboardResource from '../resource/id';
import SDMSResource from '../../SDMS/resource/id';

import dashboard from '../css/dashboardNew.module.css';

import Mainboard from './mainboard';
import Subboard from './subboard';

import DashboardStore from '../dashboardStore';
import { object } from '@amcharts/amcharts4/core';
import SessionString from '../../Common/js/sessionString';

import ProjectResource from '../../Root/resource/id';

class DashboardNew extends Component {
    static arrDayStr = ['일', '월', '화', '수', '목', '금', '토'];

    constructor(props) {
        super(props);

        this.state = {
            buildingGroupList: [],
            buildingGroup: -1,
            building: -1,
            zone: -1,
            useSensorList: null,
            mode: DashboardResource.mode.main,

            todayAllAlarms: store.getState().sensorAllAlarm,
            currentWork: DashboardStore.getState().currentWork,

            selectDay: [],
            weeklyAlarms: [],
            materials: [],
        }

        this.props = props;

        store.subscribe(function () {
            let data = store.getState();

            //if ((data.todayAlarm !== null && data.todayAlarm !== undefined)
            //    && data.actionType === 'TODAY_ALARM') {
            //    this.changeAlarm(data.todayAlarm);
            //}
            if ((data.sensorAllAlarm !== null && data.sensorAllAlarm !== undefined)
                && data.actionType === 'SENSOR_ALARM') {
                this.changeAlarm(data.sensorAllAlarm);
            }
            
        }.bind(this));

        DashboardStore.subscribe(function () {
            let data = DashboardStore.getState();

            if ((data.currentWork !== null && data.currentWork !== undefined)
                && data.actionType === 'CURRENT_WORK') {
                this.changeCurrentWork(data.currentWork);
            }

        }.bind(this));

        this.init();
    }

    changeAlarm(todayAllAlarms) {
        this.setState({ todayAllAlarms: todayAllAlarms});
    }

    changeCurrentWork(currentWork) {
        this.setState({ currentWork: currentWork});
    }

    componentDidMount() {
        this.initCount();
    }

    async initCount() {
        const [result, message] = await DashboardController.requestUseSensor(this.state.buildingGroup, this.state.building, this.state.zone);

        if (result !== null && result !== undefined) {
            this.setState({ useSensorList: result});
        }
    }

    async init() {
        // 선택 날짜 초기화
        this.makeWeek();

        // 유저가 선택한 날짜 불러오기
        this.getSelectDay();

        // 건물 정보 가져오기
        const [buildingGroupListData, outdoorZones, errorMessage] = await SDMSController.requestBuildingGroupList();
        let buildingGroupList = [];
        let weeklyAlarms = [];
        let materials = [];

        if (buildingGroupListData !== null && buildingGroupListData !== undefined)
            buildingGroupList = buildingGroupListData;

        // 1주일 알람 정보 가져오기
        const [weeklyAlarmData, message] = await DashboardController.requestWeeklyStatus();

        if (weeklyAlarmData !== null && weeklyAlarmData !== undefined)
            weeklyAlarms = weeklyAlarmData;

        // material 값 가져오기
        const [materialData, materialMessage] = await SDMSController.requestMaterials();
        if (materialData !== null && materialData !== undefined)
            materials = materialData;

        this.setState({ buildingGroupList: buildingGroupList, weeklyAlarms: weeklyAlarms, materials: materials });
    }

    async getSelectDay() {
        let userInfo = await ProjectResource.initUserInfo();
        let selectDay = this.state.selectDay;

        if (userInfo === null || userInfo === undefined || selectDay === null || selectDay === undefined)
            return;

        const [result, value] = await SDMSController.requestGetOption(userInfo.id, "SelectDay");

        if (result === true && value.length > 0) {
            let selectDayData = value[0].propertyValue1;
            let arrSelectDay = selectDayData.split(",");

            if (arrSelectDay === null || arrSelectDay === undefined || arrSelectDay.length !== 7)
                return;

            for (let i = 0; i < 7; i++) {
                if (selectDay[i] !== null && selectDay[i] !== undefined) {
                    if (arrSelectDay[i] === "true") {
                        selectDay[i].checked = true;
                        $('#hsmUsr0' + i).prop("checked", true);    // 체크박스 선택
                    } else {
                        selectDay[i].checked = false;
                        $('#hsmUsr0' + i).prop("checked", false);   // 체크박스 해제
                    }
                }
            }
        }

        this.setState({ selectDay: selectDay });
    }

    async reloadSiteID() {
        let siteID = ProjectResource.SiteID;

        if (siteID === null || siteID === undefined) {
            // 사이트 ID 요청
            const [result, message] = await SDMSController.requestGetSiteID();

            if (result !== null && result !== undefined) {
                siteID = result;
            }
        }

        return siteID;
    }

    makeWeek() {
        let week = new Array();
        const arrDayStr = DashboardNew.arrDayStr;

        for (let i = 6; i >= 0; i--) {
            const value = {};

            let today = new Date();
            let date = new Date(today.setDate(today.getDate() - i));

            const year = date.getFullYear();
            let month = date.getMonth() + 1;
            if (month < 10)
                month = "0" + month;

            let day = date.getDate();
            if (day < 10)
                day = "0" + day;

            const dayString = arrDayStr[date.getDay()];

            const displayText = month + '/' + day + '(' + dayString + ')'
            value.displayText = displayText;
            value.value = year + '-' + month + '-' + day;

            const selectDay = this.state.selectDay;

            if (selectDay[i] === null || selectDay[i] === undefined) {
                // 해당 값이 없다면
                value.checked = true;
                selectDay[6 - i] = value;
            }
            else {
                // 해당 값이 이미 존재한다면
                value.checked = selectDay[i].checked;
                selectDay[6 - i] = value;
            }

            week.push(value);
        }
    }

    setSelectSensors() {
        if (this.state.useSensorList === null || this.state.useSensorList === undefined) {
            return null;
        }

        const buildingGroupID = this.state.buildingGroup;
        const buildingID = this.state.building;
        const zoneID = this.state.zone;
        const useSensorList = this.state.useSensorList;

        // 전체 센서 갯수
        if (buildingGroupID === -1) {
            let sensorList = {};
            sensorList.fireSensors = useSensorList.fireSensors;
            sensorList.disabledFireSensors = useSensorList.disabledFireSensors;
            sensorList.psmSensors = useSensorList.psmSensors;
            sensorList.disabledPSMSensors = useSensorList.disabledPSMSensors;
            sensorList.etcSensors = useSensorList.etcSensors;
            sensorList.disabledEtcSensors = useSensorList.disabledEtcSensors;
            sensorList.cctvs = useSensorList.cctvs;
            sensorList.disabledCCTVs = useSensorList.disabledCCTVs;

            return sensorList;
        }


    }

    selectSpatial = (buildingGroup, building, zone) => {
        console.log(buildingGroup + "," + building + "," + zone);

        this.setState({ buildingGroup: buildingGroup, building: building, zone: zone});
    }

    todayAlarms = () => {
        let todayAllAlarms = [];
        let todayAlarms = [];
        const buildingGroupList = this.state.buildingGroupList;

        if (this.state.todayAllAlarms === null || this.state.todayAllAlarms === undefined || this.state.todayAllAlarms.length === 0) {
            return todayAlarms;
        }

        todayAllAlarms = this.state.todayAllAlarms;

        for (let i = 0; i < todayAllAlarms.length; i++) {
            let chk = false;
            let todayAlarm = new Object();
            let todayAlarmData = todayAllAlarms[i];

            // 해당 알람 빌딩그룹 정보 가져오기
            if (buildingGroupList !== null && buildingGroupList !== undefined) {

                for (let j = 0; j < buildingGroupList.length; j++) {
                    const buildingGroup = buildingGroupList[j];

                    for (let z = 0; z < buildingGroup.buildingDatas.length; z++) {
                        const building = buildingGroup.buildingDatas[z];

                        for (let n = 0; n < building.zoneDatas.length; n++) {
                            const zone = building.zoneDatas[n];

                            if (zone.id === todayAlarmData.zoneID) {
                                chk = true;
                                todayAlarm.buildingGroupID = buildingGroup.id;
                                todayAlarm.buildingID = building.id;
                                break;
                            }
                        }

                        if (chk === true)
                            break;
                    }

                    if (chk === true)
                        break;
                }
            }

            if (chk === false) {
                todayAlarm.buildingGroupID = null;
                todayAlarm.buildingID = null;
            }

            todayAlarm.time = todayAlarmData.dtTime;
            todayAlarm.orgSensorID = todayAlarmData.orgSensorID;
            todayAlarm.facilityType = todayAlarmData.facilityType;
            todayAlarm.zoneID = todayAlarmData.zoneID;
            todayAlarm.sensorZoneID = todayAlarmData.sensorZoneID;
            todayAlarm.isAlarm = todayAlarmData.isAlarm;
            todayAlarm.materialType = todayAlarmData.materialType;

            todayAlarms.push(todayAlarm);
        }

        return todayAlarms;
    }

    changeMode = (mode) => {
        let currentMode = this.state.mode;
        let changeMode = this.state.mode;

        if (currentMode === mode) {
            return;
        } else if (mode === DashboardResource.mode.main) {
            changeMode = DashboardResource.mode.main;
        } else if (mode === DashboardResource.mode.sub) {
            changeMode = DashboardResource.mode.sub;
        }

        this.setState({ mode: changeMode});
    }

    selectWeeklyAlarms(todayAlarms) {
        let weeklyAlarms = [];
        let selectWeeklyAlarms = [];

        // 오늘 날짜 체크 유무 확인
        let today = new Date();
        let chkTodayAlarm = this.checkAlarmDate(today);

        if (chkTodayAlarm === true && todayAlarms !== null && todayAlarms !== undefined) {
            for (let i = 0; i < todayAlarms.length; i++) {
                let todayAlarmData = todayAlarms[i];

                selectWeeklyAlarms.push(todayAlarmData);
            }
        }

        if (this.state.weeklyAlarms !== null && this.state.weeklyAlarms !== undefined && this.state.weeklyAlarms.length !== 0) {
            weeklyAlarms = this.state.weeklyAlarms;

            for (let i = 0; i < weeklyAlarms.length; i++) {
                const weeklyAlarm = weeklyAlarms[i];

                // 해당 날짜 알람 데이터 확인
                let chkDate = this.checkAlarmDate(weeklyAlarm.time);

                if (chkDate === true)
                    selectWeeklyAlarms.push(weeklyAlarm);
            }
        }

        return selectWeeklyAlarms;
    }

    checkAlarmDate(alarmTime) {
        const selectDay = this.state.selectDay;

        if (selectDay === null || selectDay === undefined || selectDay.length === 0) {
            return true;
        }

        let alarmDate = new Date(alarmTime);

        for (let i = 0; i < 7; i++) {
            let date = new Date(selectDay[i].value);

            if (alarmDate.getDate() === date.getDate()) {
                if (selectDay[i].checked === true)
                    return true;
                else
                    return false;
            }
        }

        return false;
    }

    displayBoardContent = () => {
        const mode = this.state.mode;
       
        let selectSensors = this.setSelectSensors();
        let todayAlarms = this.todayAlarms();
        const selectWeeklyAlarms = this.selectWeeklyAlarms(todayAlarms);

        if (mode === DashboardResource.mode.main) {
            return <Mainboard
                todayAlarms={todayAlarms}
                selectSensors={selectSensors}
                buildingGroupList={this.state.buildingGroupList}
                changeMode={this.changeMode}
                currentWork={this.state.currentWork}
                weeklyAlarms={this.state.weeklyAlarms}
                selectWeeklyAlarms={selectWeeklyAlarms}
                materials={this.state.materials}
            />;

        } else if (mode === DashboardResource.mode.sub) {
            return <Subboard
                selectSensors={selectSensors}
                selectDay={this.state.selectDay}
                sensorZoneHistorys={this.state.sensorZoneHistorys}
                selectWeeklyAlarms={selectWeeklyAlarms}
                currentWork={this.state.currentWork}
                materials={this.state.materials}
            />;
        }

        return <></>;
    }

    changeDay = (index) => {
        let selectDay = this.state.selectDay;

        if (selectDay.length === 0)
            return;

        let checked = selectDay[index].checked;

        if (checked === true)
            selectDay[index].checked = false;
        else
            selectDay[index].checked = true;

        this.setState({ selectDay: selectDay });

        // 선택된 날짜 설정 저장하기
        this.setSelectDay();    
    }

    async setSelectDay() {
        let selectDay = this.state.selectDay;
        let userInfo = ProjectResource.getUserInfo();
        let strSelectDay = "";

        if (userInfo === null || userInfo === undefined || selectDay === null || selectDay === undefined)
            return;

        for (let i = 0; i < 7; i++) {
            let chk = selectDay[i].checked;

            if (strSelectDay === "")
                strSelectDay = strSelectDay + selectDay[i].checked;
            else
                strSelectDay = strSelectDay + "," + selectDay[i].checked;
        }

        await SDMSController.requestSaveOption(-1, userInfo.id, "SelectDay", "", strSelectDay, "", "", "");
    }

    changedDate = () => {
        // 날짜가 바뀌는 이벤트
        this.reloadDate();
    }

    async reloadDate() {
        // 선택 날짜 다시 불러오기
        this.makeWeek();

        let sensorZoneHistorys = [];

        const [sensorZoneHistorysData, message] = await DashboardController.requestWeeklyStatus();

        if (sensorZoneHistorysData !== null && sensorZoneHistorysData !== undefined)
            sensorZoneHistorys = sensorZoneHistorysData;

        this.setState({ sensorZoneHistorys: sensorZoneHistorys });
    }

    render() {

        let selectSensors = this.setSelectSensors();
        

        return (
       
            <aside className={dashboard.bythemDashboard + " " + dashboard.stopDragging}  >

                <div className={dashboard.dashboardContainer + " " + dashboard.dashboardHasTitle} >

                    <div className={dashboard.dashboardBody} >
                        
                        <InfoHeader selectSensors={selectSensors} mode={this.state.mode} changeMode={this.changeMode} selectDay={this.state.selectDay} changeDay={this.changeDay}  reloadDate={this.changedDate} />

                        <div className={dashboard.infoContainer}>

                            <figure className={dashboard.infoContent}>

                                {this.displayBoardContent()}

                            </figure>

                        </div>
                        
                    </div>

                </div>

            </aside>
        );
    }
}
export default DashboardNew;