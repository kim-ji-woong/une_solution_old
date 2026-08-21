import React, { Component } from 'react';

import SopSimulatorSBcall from './sopSimulatorSBcall';
import SopSimulatorSBSub from './sopSimulatorSBSub';

import SopSimulatorResource from "../resource/id";
import SopController from '../../SOPManager/services/sopController';
import SopSimulatorController from '../services/sopSimulatorController';
import store from '../../Root/store';

import uis from '../../Common/css/ui.module.css';
import $ from 'jquery';

class SopSimulatorSB extends Component {

    static resource = SopSimulatorResource;

    constructor(props) {
        super(props);

        this.state = {
            content: SopSimulatorResource.ID.menu.callSOP,
            sopTabIndex: -1,            
            sopDatas: [], 
            sensorAlarms: store.getState().sensorAlarm,
            prevProps: null
        }

        this.props = props;

        this.openDB = this.openDB.bind(this);
        this.closeSopData = this.closeSopData.bind(this);
        this.onChangeTab = this.onChangeTab.bind(this);
        this.onChangeStep = this.onChangeStep.bind(this);

        store.subscribe(function () {
            this.chgAlarm(store.getState().sensorAlarm);
        }.bind(this));
    }

    componentDidMount() {
        // 타이틀바 클릭 이벤트 핸들러
        this.props.menuEvent.handler = this.onSelectMenu;

        this.loadMonitorData();
    }

    async chgAlarm(alarms) {
        const orgAlarms = this.state.sensorAlarms;
        if (orgAlarms === alarms)
            return;

        // 새로운 알람
        //var newAlarm = this.checkAlarm(orgAlarms, alarms);
        //if (newAlarm.length > 0) {
        //    // eSOP는 경계, 심각단계에서 자동실행되며, 주의 단계의 경우 ‘상황전파’ 버튼을 통한 실행
        //    for (var i = 0; i < newAlarm.length; i++) {
        //        if (newAlarm[i].facilityType < 0)
        //            continue;
        //        const versionID = await SopSimulatorController.loadLinkedSOP(newAlarm[i].facilityType);
        //        if (versionID < 0)
        //            continue;
        //        this.openDB(versionID, newAlarm[i].sensorZoneHistoryID);
        //    }
        //}

        await this.checkSopOfAlarm(alarms);

        // 해지된 알람은 SOP를 닫는다
        var removeAlarm = this.checkAlarm(alarms, orgAlarms);
        if (removeAlarm !== null) {
            if (removeAlarm.length > 0) {
                for (var j = 0; j < removeAlarm.length; j++) {
                    const index = this.checkSensorSOP(removeAlarm[j].sensorZoneHistoryID);

                    if (index >= 0) {
                        await this.closeSopDbData(index);
                        this.closeSopData(index);
                    }
                }
            }
        }

        this.setState({ sensorAlarms: alarms });
    }

    // 기존 알람과 비교해서 새로운 알람, 해지된 알람을 구분한다
    checkAlarm(alarms, targetAlarms) {
        var returnAlarm = [];

        if (alarms === null || alarms.length === 0) {
            returnAlarm = targetAlarms;
            return returnAlarm;
        }

        if (targetAlarms !== null) {
            for (var i = 0; i < targetAlarms.length; i++) {
                var chk = false;
                for (var j = 0; j < alarms.length; j++) {
                    if (targetAlarms[i].sensorZoneHistoryID === alarms[j].sensorZoneHistoryID) {
                        chk = true;
                        break;
                    }
                }

                if (!chk) {
                    returnAlarm.push(targetAlarms[i]);
                }
            }
        }

        return returnAlarm;
    }

    // 신호 발생하여 열린 SOP가 있는지 확인 (sensorZoneHistoryID로 비교)
    checkSensorSOP(sensorZoneHistoryID) {
        const sopDatas = this.state.sopDatas;
        var index = -1;
        for (var i = 0; i < sopDatas.length; i++) {
            const sopData = sopDatas[i];
            if (sopData.sensorZoneHistoryID === sensorZoneHistoryID) {
                index = i;
                break;
            }
        }

        return index;
    }

    // 같은 SOP가 열려 있는지 확인 (버전으로 비교)
    checkSensorSOP2(versionID) {
        const sopDatas = this.state.sopDatas;
        var index = -1;
        for (var i = 0; i < sopDatas.length; i++) {
            const sopData = sopDatas[i];
            if (sopData.version.id === versionID) {
                index = i;
                break;
            }
        }

        return index;
    }

    
    async checkSopOfAlarm(alarms) {
        if (alarms === null)
            return;

        for (var i = 0; i < alarms.length; i++) {
            const alarm = alarms[i];
            // SOP 실행 상태 (-1: SOP 시작 하기전, 0: SOP 실행 요청, 1: SOP 실행중, 2: SOP종료)
            if (alarm.sopStatus === -1)
                continue;

            if (alarm.sopStatus === 0 || alarm.sopStatus === 1) {
                // 이미 실행중인지 확인한다
                var tabIndex = this.checkSensorSOP(alarm.sensorZoneHistoryID);
                if (tabIndex >= 0) {
                    continue;
                }

                // 연결된 SOP를 연다
                if (alarm.facilityType < 0)
                    continue;

                const versionID = await SopSimulatorController.loadLinkedSOP(alarm.facilityType);
                if (versionID < 0)
                    continue;

                // 이미 실행중인지 확인한다
                tabIndex = this.checkSensorSOP2(versionID);
                if (tabIndex >= 0) {
                    continue;
                }

                await this.openDB(versionID, alarm);
            }
            else {
                // 해당 알람으로 열린 SOP가 종료되었는지 확인하고 종료한다
                const tabIndex = this.checkSensorSOP(alarm.sensorZoneHistoryID);
                if (tabIndex === -1) {
                    continue;
                }

                await this.closeSopDbData(tabIndex);
                this.closeSopData(tabIndex);
            }
        }
    }

    // 실행중인 sop가 있나 ?
    async loadMonitorData() {
        const data = await SopSimulatorController.monitorComponentHistory();
        if (data === null || data.historyData.length === 0)
            return;

        var loadSopDatas = [...this.state.sopDatas];

        const dataLength = data.historyData.length;
        for (var i = 0; i < dataLength; i++) {
            const [sopDataResult, message] = await SopController.requestOpenDB(data.historyData[i].versionID);
            if (sopDataResult && sopDataResult.success) {
                
                const actionStepLength = sopDataResult.sopData.actionStepDatas.length;
                for (var j = 0; j < actionStepLength; j++) {
                    if (sopDataResult.sopData.actionStepDatas[j].actionStep === null)
                        continue;

                    // 같은 단계인가? ex) 관심
                    if (data.historyData[i].actionStep.id === sopDataResult.sopData.actionStepDatas[j].actionStep.id) {
                        sopDataResult.sopData.currentActionStep = sopDataResult.sopData.actionStepDatas[j];

                        // 단계 안에 ActionStepHistory 기록 넣음
                        sopDataResult.sopData.actionStepDatas[j].actionStepHistory = data.historyData[i].actionStepHistory;

                        // 단계 안에 ComponentHistory 기록 넣음
                        sopDataResult.sopData.actionStepDatas[j].componentHistories = data.historyData[i].componentHistories
                        sopDataResult.sopData.actionStepDatas[j].componentHistoryDetails = data.historyData[i].componentHistoryDetails
                        break;
                    }
                }

                this.checkArrows(sopDataResult.sopData);
                await this.checkStepMembers(sopDataResult.sopData);

                loadSopDatas.push(sopDataResult.sopData);
            }
        }
        if (this.state.sopDatas !== loadSopDatas) {

            const sopTabIndex = loadSopDatas.length - 1;

            this.setState({ sopDatas: loadSopDatas, content: SopSimulatorResource.ID.menu.execSOP, sopTabIndex: sopTabIndex });
        }
    }

    // sensorZoneHistoryID : 센서 신호를 통해 열리는 sop는 자동 시작한다
    async openDB(versionID, alarm) {        
        const [sopDataResult, message] = await SopController.requestOpenDB(versionID);

        if (sopDataResult && sopDataResult.success) {
            var loadSopDatas = this.state.sopDatas;
            var sopTabIndex = this.checkOpenSOP(loadSopDatas, sopDataResult.sopData);
            if (sopTabIndex > -1) {
                this.setState({ content: SopSimulatorResource.ID.menu.execSOP, sopDatas: loadSopDatas, sopTabIndex: sopTabIndex });
            }
            else {
                if (alarm !== null) {
                    sopDataResult.sopData.sensorZoneHistoryID = alarm.sensorZoneHistoryID; // 알람 신호로 열린 SOP는 sensorZoneHistoryID를 가진다.
                    sopDataResult.sopData.alarmDateTime = '[' + alarm.strDateTime + '] ';
                    sopDataResult.sopData.alarmPosition = alarm.positionName;
                    //sopDataResult.sopData.title = 
                    // 신호가 발생한 단계에 맞는 sop를 현재 sop로 설정한다
                    this.setCurrentActionStep(sopDataResult.sopData, alarm.alarmDepth);
                }

                // 현재 sop가 설정되지 않은경우
                // ex : 신호가 발생해서 띄워진 sop에 그 단계가 없는 경우
                if (sopDataResult.sopData.currentActionStep === undefined) {
                    this.setCurrentActionStep2(sopDataResult.sopData);
                }
                
                this.checkArrows(sopDataResult.sopData);
                await this.checkStepMembers(sopDataResult.sopData);

                loadSopDatas.push(sopDataResult.sopData); // 열려있는 SOP 중에서 마지막에 추가
                sopTabIndex = loadSopDatas.length - 1; // 열려있는 SOP 중에서 마지막 index

                this.setState({ content: SopSimulatorResource.ID.menu.execSOP, sopDatas: loadSopDatas, sopTabIndex: sopTabIndex });
            }
        }
        //else {
        //    this.setState({ content: SopSimulator.menu.execSOP, sopDatas: this.state.sopData });
        //    alert(message);
        //}
    }

    // 같은 sop가 이미 열려있는지 체크
    checkOpenSOP(preSopDatas, newSopData) {
        if (preSopDatas === null || preSopDatas.length === 0)
            return -1;

        for (var i = 0; i < preSopDatas.length; i++) {
            if (preSopDatas[i].version.id === newSopData.version.id)
                return i;
        }

        return -1;
    }
        
    setCurrentActionStep(sopData, alarmDepth) {
        if (sopData.actionStepDatas === null)
            return;

        for (var i = 0; i < sopData.actionStepDatas.length; i++) {
            const actionStepData = sopData.actionStepDatas[i];
            if (actionStepData.actionStep) {
                if (alarmDepth) {
                    if (alarmDepth === 1 && actionStepData.actionStep.stepName === '관심') {
                        sopData.currentActionStep = actionStepData;
                        break;
                    }
                    else if (alarmDepth === 2 && actionStepData.actionStep.stepName === '주의') {
                        sopData.currentActionStep = actionStepData;
                        break;
                    }
                    else if (alarmDepth === 3 && actionStepData.actionStep.stepName === '경계') {
                        sopData.currentActionStep = actionStepData;
                        break;
                    }
                    else if (alarmDepth === 4 && actionStepData.actionStep.stepName === '심각') {
                        sopData.currentActionStep = actionStepData;
                        break;
                    }
                }
                else {
                    sopData.currentActionStep = actionStepData;
                }
            }
        }

        //sopData.actionStepDatas.map(actionStepData => {
        //    if (actionStepData.actionStep) {
        //          sopData.currentActionStep = actionStepData;
        //    }
        //});
    }

    setCurrentActionStep2(sopData) {
        if (sopData.actionStepDatas === null)
            return;

        sopData.actionStepDatas.map(actionStepData => {
            if (actionStepData.actionStep) {
                  sopData.currentActionStep = actionStepData;
            }
        });
    }

    checkArrows(sopData) {
        if (sopData) {
            const actionStepCount = sopData.actionStepDatas.length;

            for (let i = 0; i < actionStepCount; i++) {
                const actionStepData = sopData.actionStepDatas[i];
                const stepMemberCount = actionStepData.stepMemberDatas.length;

                for (let j = 0; j < stepMemberCount; j++) {
                    const stepMemberData = actionStepData.stepMemberDatas[j];

                    if (stepMemberData.arrows.length > 0) {
                        stepMemberData.resetArrows = true;
                    }
                }
            }
        }
    }

    async checkStepMembers(sopData) {
        if (sopData) {
            const actionStepCount = sopData.actionStepDatas.length;

            for (let i = 0; i < actionStepCount; i++) {
                const actionStepData = sopData.actionStepDatas[i];

                if (actionStepData.stepMemberDatas.length === 0) {
                    const [stepMemberData, message] = await SopController.requestDefaultStepMemberData(actionStepData);

                    if (!stepMemberData) {
                        alert(message);
                        break;
                    }
                }
            }
        }
    }

    // 시작된 sop는 db종료
    async closeSopDbData(sopTabIndex) {
        const sopData = this.state.sopDatas[sopTabIndex];

        if (sopData.currentActionStep.componentHistories !== undefined &&
            sopData.currentActionStep.componentHistories.length > 0) {

            const actionStepHistoryID = sopData.currentActionStep.componentHistories[0].actionStepHistoryID;
            await SopSimulatorController.closeSOP(actionStepHistoryID);
        }
    }

    closeSopData(index) {
        var sopDatasCopy = [...this.state.sopDatas];
        sopDatasCopy.splice(index, 1);

        var sopTabIndexTemp = -1;
        if (sopDatasCopy.length > 0) {
            sopTabIndexTemp = sopDatasCopy.length - 1;
        }

        if (sopTabIndexTemp < 0)
            this.setState({ sopDatas: sopDatasCopy, sopTabIndex: sopTabIndexTemp, content: SopSimulatorResource.ID.menu.callSOP });
        else
            this.setState({ sopDatas: sopDatasCopy, sopTabIndex: sopTabIndexTemp });
    }

    changeContent = (content, versionID) => {        
        if (content === SopSimulatorResource.ID.menu.callSOP) {
            this.setState({ content: content });
        }
        else if (content === SopSimulatorResource.ID.menu.execSOP) {
            this.openDB(versionID, null);
        }
    }

    onChangeTab(index) {
        this.setState({ sopTabIndex: index });
    }

    // 단계 변경
    async onChangeStep(stepID, stepName) {        
        var sopDatas = this.state.sopDatas;
        
        // 이미 열려 있음
        for (var i = 0; i < sopDatas.length; i++) {
            if (sopDatas[i].currentActionStep.actionStep.id === stepID) {
                this.setState({ sopTabIndex: i });
                return;
            }
        }

        const [sopDataResult, message] = await SopController.requestOpenDB(sopDatas[this.state.sopTabIndex].version.id);

        if (sopDataResult && sopDataResult.success) {
            
            this.setActionStep(sopDataResult.sopData, stepID);
            this.checkArrows(sopDataResult.sopData);
            await this.checkStepMembers(sopDataResult.sopData);

            sopDatas.push(sopDataResult.sopData); 
            const index = sopDatas.length - 1;

            this.setState({ sopDatas: sopDatas, sopTabIndex: index });
        }
    }

    setActionStep(sopData, actionStepID) {
        sopData.actionStepDatas.map(actionStepData => {
            if (actionStepData.actionStep !== null && actionStepData.actionStep.id === actionStepID) {
                sopData.currentActionStep = actionStepData;
            }
        });

        return sopData;
    }

    onSelectMenu = (menu, param) => {
        // SOP 불러오기 버튼을 클릭 시

        // 첫 sop 불러오기 페이지에서 작동 안함 
        if (this.state.content !== SopSimulatorResource.ID.menu.execSOP && $('.btnCallSOP').hasClass(uis.toggleOn) !== true)
            return;

        $('.btnCallSOP').toggleClass(uis.toggleOn);

        if (menu === SopSimulatorSB.resource.ID.menu.callSOP) {
            if ($('.btnCallSOP').hasClass(uis.toggleOn) === true) {
                // SOP 불러오기 페이지로 이동
                console.log("CallSOP on");
    
                this.setState({ content: SopSimulatorResource.ID.menu.callSOP});
            } else {
                // SOP 실행페이지로 이동
                console.log("CallSOP off");

                this.setState({ content: SopSimulatorResource.ID.menu.execSOP });
            }
            
        }
    }

    render() {
        let ui = <></>;
        if (this.state.content === SopSimulatorResource.ID.menu.callSOP) {
            ui = <>
                <SopSimulatorSBcall openDB={this.openDB}/>
            </>
        } else if (this.state.content === SopSimulatorResource.ID.menu.execSOP) {
            ui = <>
                <SopSimulatorSBSub
                    changeContent={this.changeContent}
                    sopDatas={this.state.sopDatas}
                    sopTabIndex={this.state.sopTabIndex}
                    onChangeTab={this.onChangeTab}
                    closeSopData={this.closeSopData}
                    onChangeStep={this.onChangeStep}
                />
            </>
        }

        return (
            <>
                {ui}
            </>
        );
    }
}

export default SopSimulatorSB;