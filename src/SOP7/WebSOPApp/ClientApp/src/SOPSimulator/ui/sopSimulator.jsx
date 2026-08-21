import React, { Component } from 'react';

import SopSimulatorSBcall from './sopSimulatorSBcall';
import SopSimulatorBody from './sopSimulatorBody';

import SopSimulatorResource from "../resource/id";
import SopController from '../../SOPManager/services/sopController';
import SopSimulatorController from '../services/sopSimulatorController';
import store from '../../Root/store';
import SettingsStore from '../../Settings/settingsStore';

import uis from '../../Common/css/ui.module.css';
import $ from 'jquery';
import { TeamEditController } from '../../TeamEditor/services/teamEditController';
import ConfirmDialog from '../../Common/ui/confirmDialog';

class SopSimulator extends Component {

    static resource = SopSimulatorResource;

    constructor(props) {
        super(props);
        
        this.state = {
            content: SopSimulatorResource.ID.menu.callSOP,
            sopTabIndex: -1,
            currentActionStep: null,
            currentSopTabKey: '',
            sopDatas: [],
            teamDatas: [],
            confirmMessage: {
                visible: false,
                title: "",
                messages: [""],
                buttons: ["확인"],
                onClose: this.onCloseConfirmDialog,
                onClickButton: null
            },
            commonSettings: {},
            prevProps: null
        }

        this.props = props;

        this.openDB = this.openDB.bind(this);
        this.closeSOP = this.closeSOP.bind(this);
        this.onChangeTab = this.onChangeTab.bind(this);
        this.onChangeStep = this.onChangeStep.bind(this);
        this.confirmDialogCloseSOP = this.confirmDialogCloseSOP.bind(this);

        store.subscribe(function () {
            this.loadSOP(store.getState());
            //this.changeCommonSettings(store.getState());
        }.bind(this));

        SettingsStore.subscribe(function () {
            let data = SettingsStore.getState();

            if (data.actionType === 'SOP_COMMON_SETTINGS') {
                this.changeSOPCommonSettings(data.sopCommonSettings);
            }

        }.bind(this));
    }

    componentDidMount() {
        // 타이틀바 클릭 이벤트 핸들러
        this.props.menuEvent.handler = this.onSelectMenu;        
    }

    async loadSOP(storeValue) {
        if (storeValue && storeValue.actionType !== 'SOP_HISTORY')
            return;

        if (!storeValue.sopHistory)
            return;
                
        const sopHistories = this.checkAlreadySOP(storeValue.sopHistory.sopRunDatas);
        const lastAccessActionStepHistoryID = storeValue.sopHistory.lastAccessActionStepHistoryID;
        console.log(sopHistories);

        if (sopHistories === null || sopHistories.length === 0) {
            this.setState({ sopDatas: [], content: SopSimulatorResource.ID.menu.callSOP, sopTabIndex: -1, currentSopTabKey: '' });
        }
        else {
            if (sopHistories.length !== this.state.sopDatas.length && sopHistories.length > 0) {
                this.loadTeamDatas();
            }

            let sopTabIndex = 0;
            let currentSopTabKey = this.state.currentSopTabKey;
            let currentActionStep = this.state.currentActionStep;            
            let currentSopData = null;
            
            const optionIndex = 1;
            const historyLength = sopHistories.length;
            for (let i = 0; i < historyLength; i++) {
                const sopHistory = sopHistories[i];
                
                // flow chart 설정
                const currentActionStepID = this.getCurrentActionStepID(sopHistory.key);
                this.setCurrentActionStep(sopHistory.sopData, currentActionStepID);

                this.checkArrows(sopHistory.sopData);
                await this.checkStepMembers(sopHistory.sopData);
            }

            
            for (let i = 0; i < historyLength; i++) {
                const sopHistory = sopHistories[i];
                // 옵션 : 새로운 이력이 있어도 현재 Client가 보고 있던 SOP를 유지한다                    
                if (optionIndex === 1 && this.state.currentSopTabKey.length > 0 && sopHistory.key === this.state.currentSopTabKey) {                    
                    sopTabIndex = i;
                    currentSopTabKey = this.state.currentSopTabKey;
                    currentSopData = sopHistory.sopData;
                    break;
                }
                else {
                    for (let j = 0; j < sopHistory.sopData.actionStepDatas.length; j++) {
                        if (sopHistory.sopData.actionStepDatas[j]._ActionStepHistory === null) {
                            continue;
                        }

                        
                        sopTabIndex = i;
                        currentSopTabKey = sopHistory.key;
                        currentActionStep = sopHistory.sopData.actionStepDatas[j];
                        currentSopData = sopHistory.sopData;

                        if (sopHistory.sopData.actionStepDatas[j]._ActionStepHistory.id === lastAccessActionStepHistoryID) {
                            break;
                        }                        
                    }                    
                }
            }

            let closeSOPHistoryID = null;
            // 확인 후 자동종료할 SOP가 있나 ?
            const closeSOPs = storeValue.sopHistory.confirmTimeoutCloseSOPs ? [...storeValue.sopHistory.confirmTimeoutCloseSOPs] : new Array();
            if (closeSOPs.length > 0) {
                closeSOPHistoryID = closeSOPs[0];
                for (let i = 0; i < historyLength; i++) {
                    const sopHistory = sopHistories[i];
                    for (let j = 0; j < sopHistory.sopData.actionStepDatas.length; j++) {
                        if (sopHistory.sopData.actionStepDatas[j]._ActionStepHistory === null) {
                            continue;
                        }

                        if (sopHistory.sopData.actionStepDatas[j]._ActionStepHistory.id === closeSOPHistoryID) {
                            sopTabIndex = i;
                            currentSopTabKey = sopHistory.key;
                            currentActionStep = sopHistory.sopData.actionStepDatas[j];
                            currentSopData = sopHistory.sopData;
                            this.confirmDialogData = sopHistory.sopData.actionStepDatas[j]._ActionStepHistory;
                            break;
                        }
                    }
                }

                if (this.confirmDialogData) {
                    let message = '대기시간 자동 종료시간이 경과되어 SOP를 종료합니다.';
                    if (this.confirmDialogData.sensorZoneHistoryID && this.confirmDialogData.sensorZoneHistoryID > 0) {
                        message = '신호 복구 후 자동 종료시간이 경과되어 SOP를 종료합니다.'
                    }
                    this.showConfirmDialog('알림', message, ['확인'], this.confirmDialogCloseSOP);
                }
            }

            // flow chart 설정
            const currentActionStepID = this.getCurrentActionStepID(currentSopTabKey);
            if (currentSopData && currentSopData !== null) {
                this.setCurrentActionStep(currentSopData, currentActionStepID, lastAccessActionStepHistoryID);

                this.checkArrows(currentSopData);
                await this.checkStepMembers(currentSopData);
            }

            this.setState({
                content: SopSimulatorResource.ID.menu.execSOP,
                sopDatas: sopHistories,
                sopTabIndex,
                currentSopTabKey,
                currentActionStep
            });
        }
    }

    async loadTeamDatas() {
        const teamDatas = [];
        teamDatas.regular = await TeamEditController.GetRegular();
        teamDatas.regularMember = await TeamEditController.DisplayRegularMember();
        teamDatas.normal = await TeamEditController.DisplayTemporary(true);
        teamDatas.emergency = await TeamEditController.DisplayTemporary(false);

        this.setState({ teamDatas });
    }

    /*
    async changeCommonSettings(storeValue) {
        if (storeValue && storeValue.actionType !== 'SOP_COMMON_SETTINGS')
            return;

        const commonSettings = storeValue.sopCommonSettings ? { ...storeValue.sopCommonSettings } : {};

        const oldCommonSettings = { ...this.state.commonSettings };
        let newCount = 0;

        for (const name in commonSettings) {
            newCount++;

            const oldValue = oldCommonSettings[name];
            const newValue = commonSettings[name];

            if (oldValue !== newValue) {
                this.setState({ commonSettings: commonSettings });
                return;
            }
        }

        let oldCount = 0;

        for (const name in oldCommonSettings) {
            oldCount++;
        }

        if (newCount !== oldCount) {
            this.setState({ commonSettings: commonSettings });
        }
    }
    */
    changeSOPCommonSettings(storeValue) {
        const commonSettings = storeValue ? storeValue : {};

        this.setState({ commonSettings: commonSettings });
    }

    // 수동 시작해서 실행전인 SOP는 DB입력이 되어있지 않기 때문에 DB 변경이 되서 loadSOP 함수가 호출되면 사라진다
    // 그러므로 전 state랑 비교해서 수동 시작해서 실행전인 SOP를 직접 list에 포함시킨다
    checkAlreadySOP(sopRunDatas) {
        const oldSopDatas = this.state.sopDatas;

        if (!oldSopDatas || oldSopDatas.length == 0) {
            return sopRunDatas;        
        }

        let newSopDatas = [];       

        for (let i = 0; i < oldSopDatas.length; i++) {
            // 변경된 DB에 동일한 SOP가 있으면 넣지 않는다 > 어차피 뒤에서 넣을거야 
            // 뒤에서 넣는 이유는 SOP 순서 때문에 (수동 실행한 SOP를 앞에 배치함)
            let match = false;
            for (let j = 0; j < sopRunDatas.length; j++) {
                if (oldSopDatas[i].key === sopRunDatas[j].key) {
                    newSopDatas.push(sopRunDatas[j]);
                    match = true;
                    break;
                }
            }

            if (!match) {
                // position이 없으면 실행 전 SOP이다 (띄워지기만 함, DB에 정보 없음)
                if (!oldSopDatas[i].position) {
                newSopDatas.push(oldSopDatas[i]);
                }
            }            
        }

        for (let i = 0; i < sopRunDatas.length; i++) {

            let match = false;
            for (let j = 0; j < newSopDatas.length; j++) {
                if (sopRunDatas[i].key === newSopDatas[j].key) {
                    newSopDatas[j] = sopRunDatas[i];
                    match = true;
                    break;
                }
            }

            if (!match) {
                newSopDatas.push(sopRunDatas[i]);
            }
        }

        return newSopDatas;
    }

    // sensorZoneHistoryID : 센서 신호를 통해 열리는 sop는 자동 시작한다
    async openDB(versionID, alarm) {        
        const [sopDataResult, message] = await SopController.requestOpenDB(versionID);

        if (sopDataResult && sopDataResult.success) {
            var loadSopDatas = this.state.sopDatas;
            var sopTabIndex = this.checkOpenSOP(sopDataResult.sopData);
            if (sopTabIndex > -1) {
                this.setState({ content: SopSimulatorResource.ID.menu.execSOP, sopDatas: loadSopDatas, sopTabIndex: sopTabIndex });
            }
            else {                                
                this.setCurrentActionStep(sopDataResult.sopData);
                this.checkArrows(sopDataResult.sopData);
                await this.checkStepMembers(sopDataResult.sopData);

                this.loadTeamDatas();

                const newSOP = {};
                newSOP.key = sopDataResult.sopData.disasterCategory.id + '/' + sopDataResult.sopData.subDisasterCategory.id + '/' + sopDataResult.sopData.disaster.id;
                newSOP.position = null;
                newSOP.sensorZoneHistoryID = null;
                newSOP.sopData = sopDataResult.sopData;

                loadSopDatas.push(newSOP); // 열려있는 SOP 중에서 마지막에 추가
                sopTabIndex = loadSopDatas.length - 1; // 열려있는 SOP 중에서 마지막 index

                this.setState({
                    content: SopSimulatorResource.ID.menu.execSOP,
                    sopDatas: loadSopDatas,
                    sopTabIndex: sopTabIndex,
                    currentActionStep: sopDataResult.sopData.currentActionStep,
                    currentSopTabKey: newSOP.key
                });
            }
        }
    }

    // 같은 sop가 이미 열려있는지 체크
    checkOpenSOP(newSopData) {        
        if (this.state.sopDatas === null || this.state.sopDatas.length === 0)
            return -1;

        const sopKey = newSopData.disasterCategory.id + '/' + newSopData.subDisasterCategory.id + '/' + newSopData.disaster.id;

        for (var i = 0; i < this.state.sopDatas.length; i++) {
            if (this.state.sopDatas[i].key === sopKey)
                return i;
        }

        return -1;
    }

    getCurrentActionStepID(sopKey) {
        const oldSopDatas = this.state.sopDatas;
        if (!oldSopDatas) {
            return -1;
        }

        for (let i = 0; i < oldSopDatas.length; i++) {
            if (oldSopDatas[i].key === sopKey) {
                const sopData = oldSopDatas[i].sopData;
                if (sopData.currentActionStep) {
                    return sopData.currentActionStep.actionStep.id;
                }

                return -1;
            }
        }
    }
        
    setCurrentActionStep(sopData, actionStepID, lastActionStepHistoryID) {
        if (!sopData.actionStepDatas || sopData.actionStepDatas === null)
            return;

        for (var k = 0; k < sopData.actionStepDatas.length; k++) {
            const actionStepData = sopData.actionStepDatas[k];
            if (actionStepData.actionStep) {
                if (actionStepData._ActionStepHistory && lastActionStepHistoryID === actionStepData._ActionStepHistory.id) {
                    sopData.currentActionStep = actionStepData;
                    break;
                }
                else {
                    
                    if (actionStepID) {
                        if (actionStepData.actionStep.id === actionStepID) {
                            sopData.currentActionStep = actionStepData;
                            break;
                        }
                    }
                    else {
                        sopData.currentActionStep = actionStepData;
                    }
                }
            }
        }

        //for (var k = 0; k < sopData.actionStepDatas.length; k++) {
        //    const actionStepData = sopData.actionStepDatas[k];
        //    if (actionStepData.actionStep) {
        //        if (actionStepID) {
        //            if (actionStepData.actionStep.id === actionStepID) {
        //                sopData.currentActionStep = actionStepData;
        //                break;
        //            }
        //        }
        //        else {
        //            if (lastActionStepHistoryID === actionStepData._ActionStepHistory.id) {
        //                sopData.currentActionStep = actionStepData;
        //                break;
        //            }
        //            else {
        //                sopData.currentActionStep = actionStepData;
        //            }
        //        }
        //    }
        //}
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

    getMakeDateTime(dateTime) {
        let year = dateTime.getFullYear();
        let month = 1 + dateTime.getMonth();
        month = month >= 10 ? month : '0' + month;  //month 두자리로 저장
        let day = dateTime.getDate();                   //d
        day = day >= 10 ? day : '0' + day;

        let hour = dateTime.getHours();
        hour = hour >= 10 ? hour : '0' + hour;
        let min = dateTime.getMinutes();
        min = min >= 10 ? min : '0' + min;
        let sec = dateTime.getSeconds();
        sec = sec >= 10 ? sec : '0' + sec;

        let strDate = year + '-' + month + '-' + day + ' ' + hour + ':' + min + ':' + sec;

        return strDate;
    }

    async confirmDialogCloseSOP(index) {        
        const actionStepHistory = this.confirmDialogData
        if (actionStepHistory) {
            const endTime = this.getMakeDateTime(new Date());
            actionStepHistory.endTime = endTime;

            await this.closeSOP(null, actionStepHistory.id, actionStepHistory.endTime, null);
        }
        this.confirmDialogData = undefined;
        this.onCloseConfirmDialog();
    }

    async closeSOP(tabIndex, actionStepHistoryID, endTime, accessedUserID) {
        if (actionStepHistoryID && actionStepHistoryID > 0) {
            await SopSimulatorController.closeSOP(actionStepHistoryID, endTime, accessedUserID);
        }
        else {
            // 시작전
            this.closeSopData(tabIndex);
        }
    }

    closeSopData = (index) => {
        if (!this.state.sopDatas[index].position) {
            let sopTabIndexTemp = -1;
            let sopDatasCopy = [...this.state.sopDatas];

            // SOP 시작전이라면 통으로 없애기
            sopDatasCopy.splice(index, 1);

            if (sopDatasCopy.length > 0) {
                sopTabIndexTemp = sopDatasCopy.length - 1;
            }
            if (sopTabIndexTemp < 0) {
                this.setState({ sopDatas: sopDatasCopy, sopTabIndex: -1, content: SopSimulatorResource.ID.menu.callSOP, currentSopTabKey: '' });
            }
            else {
                this.setState({ sopDatas: sopDatasCopy, sopTabIndex: sopTabIndexTemp, currentSopTabKey: sopDatasCopy[sopTabIndexTemp].key });
            }
        }
    }

    changeContent = (content, versionID) => {        
        if (content === SopSimulatorResource.ID.menu.callSOP) {
            this.setState({ content: content });
        }
        else if (content === SopSimulatorResource.ID.menu.execSOP) {
            if (versionID) {
                this.openDB(versionID, null);
            }
        }
    }

    onChangeTab(index) {
        this.setState({
            sopTabIndex: index,
            currentSopTabKey: this.state.sopDatas[index].key,
            currentActionStep: this.state.sopDatas[index].sopData.currentActionStep
        });
    }

    // 단계 변경
    async onChangeStep(stepID, stepName) {        
        var sopDatas = this.state.sopDatas;

        const sopData = sopDatas[this.state.sopTabIndex].sopData;
        
        for (var i = 0; i < sopData.actionStepDatas.length; i++) {
            const actionStepData = sopData.actionStepDatas[i];
            if (actionStepData.actionStep && actionStepData.actionStep.id === stepID) {
                this.setCurrentActionStep(sopData, stepID);
                this.setState({ currentActionStep: actionStepData });
                break;
            }
        }
    }

    onSelectMenu = (menu, param) => {
        // SOP 불러오기 버튼을 클릭 시

        // 첫 sop 불러오기 페이지에서 작동 안함 
        //if (this.state.content !== SopSimulatorResource.ID.menu.execSOP && $('.btnCallSOP').hasClass(uis.toggleOn) !== true)
        //    return;

        //$('.btnCallSOP').toggleClass(uis.toggleOn);

        if (menu === SopSimulator.resource.ID.menu.callSOP) {
            //if ($('.btnCallSOP').hasClass(uis.toggleOn) === true) {
            //    // SOP 불러오기 페이지로 이동
            //    console.log("CallSOP on");

            //    this.setState({ content: SopSimulatorResource.ID.menu.callSOP});
            //} else {
            //    // SOP 실행페이지로 이동
            //    console.log("CallSOP off");

            //    this.setState({ content: SopSimulatorResource.ID.menu.execSOP });
            //}   
            let content = this.state.content;
            const sopDatas = this.state.sopDatas;

            if (content === SopSimulatorResource.ID.menu.callSOP && sopDatas.length !== 0) {
                this.setState({ content: SopSimulatorResource.ID.menu.execSOP });
            } else {
                this.setState({ content: SopSimulatorResource.ID.menu.callSOP });
            }   
        }
    }

    onCloseConfirmDialog = () => {
        const confirmMessage = { ...this.state.confirmMessage };
        confirmMessage.visible = false;

        this.setState({ confirmMessage });
    }

    showConfirmDialog = (title, messages, buttons, onClickButton) => {
        const confirmMessage = { ...this.state.confirmMessage };
        confirmMessage.visible = true;
        confirmMessage.title = title;
        confirmMessage.buttons = buttons;
        confirmMessage.onClickButton = onClickButton;

        if (!messages) {
            confirmMessage.messages = [""];
        }
        else if (Array.isArray(messages)) {
            confirmMessage.messages = messages;
        }
        else {
            confirmMessage.messages = [messages];
        }

        this.setState({ confirmMessage });
    }

    render() {
        let ui = <></>;
        if (this.state.content === SopSimulatorResource.ID.menu.callSOP) {
            ui = <>
                <SopSimulatorSBcall
                    openDB={this.openDB}
                    onSelectMenu={this.onSelectMenu}
                    commonSettings={this.state.commonSettings}
                />
            </>
        } else if (this.state.content === SopSimulatorResource.ID.menu.execSOP) {
            ui = <>
                <SopSimulatorBody
                    id={'SopSimulatorBody'}
                    changeContent={this.changeContent}
                    sopDatas={this.state.sopDatas}
                    sopTabIndex={this.state.sopTabIndex}
                    onChangeTab={this.onChangeTab}
                    closeSOP={this.closeSOP}
                    onChangeStep={this.onChangeStep}
                    showConfirmDialog={this.showConfirmDialog}
                    onCloseConfirmDialog={this.onCloseConfirmDialog}
                    currentSopTabKey={this.state.currentSopTabKey}
                    currentActionStep={this.state.currentActionStep}
                    teamDatas={this.state.teamDatas}
                    commonSettings={this.state.commonSettings}
                />
            </>
        }

        return (
            <>
                {ui}
                {
                    /* alert창 대신 사용 */
                    this.state.confirmMessage.visible &&
                    <ConfirmDialog title={this.state.confirmMessage.title} messages={this.state.confirmMessage.messages} buttons={this.state.confirmMessage.buttons} onClose={this.state.confirmMessage.onClose} onClickButton={this.state.confirmMessage.onClickButton} />
                }
            </>
        );
    }
}

export default SopSimulator;