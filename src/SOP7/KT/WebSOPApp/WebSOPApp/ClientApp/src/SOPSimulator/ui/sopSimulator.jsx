import React, { Component } from 'react';

import SopSimulatorSOPList from './sopSimulatorSOPList';
import SopSimulatorBody from './sopSimulatorBody';

import SopSimulatorResource from "../resource/id";
import SopController from '../../SOPManager/services/sopController';
import SopSimulatorController from '../services/sopSimulatorController';
import store from '../../Root/store';

import uis from '../../Common/css/ui.module.css';
import $ from 'jquery';
import { TeamEditController } from '../../TeamEditor/services/teamEditController';

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
            prevProps: null
        }

        this.props = props;

        this.openDB = this.openDB.bind(this);
        this.closeSopData = this.closeSopData.bind(this);
        this.onChangeTab = this.onChangeTab.bind(this);
        this.onChangeStep = this.onChangeStep.bind(this);

        store.subscribe(function () {
            this.loadSOP(store.getState());
        }.bind(this));
    }

    componentDidMount() {
        // 타이틀바 클릭 이벤트 핸들러
        this.props.menuEvent.handler = this.onSelectMenu;        
    }

    async loadSOP(storeValue) {
        const sopHistory = storeValue.sopHistory;
        if (storeValue && storeValue.actionType !== 'SOP_HISTORY')
            return;

        if (!sopHistory)
            return;

        const sopHistories = sopHistory.sopRunDatas;
        const lastAccessActionStepHistoryID = sopHistory.lastAccessActionStepHistoryID;

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
            let findCurrentData = false; // 이게 false면 sop가 종료됨

            const optionIndex = 1;                        
            const historyLength = sopHistories.length;
            for (let i = 0; i < historyLength; i++) {
                const sopHistory = sopHistories[i];
                if (optionIndex === 1 && this.state.currentSopTabKey.length > 0) {
                    // 옵션 : 새로운 이력이 있어도 현재 Client가 보고 있던 SOP를 유지한다                    
                    for (let j = 0; j < sopHistory.sopData.actionStepDatas.length; j++) {
                        const stepData = sopHistory.sopData.actionStepDatas[j];

                        if (stepData._ActionStepHistory === null) {
                            continue;
                        }

                        // flow chart 설정
                        this.setCurrentActionStep(sopHistory.sopData, currentActionStep.actionStep.id);
                        this.checkArrows(sopHistory.sopData);
                        await this.checkStepMembers(sopHistory.sopData);

                        if (sopHistory.key === this.state.currentSopTabKey) {
                            if (stepData.actionStep.id === currentActionStep.actionStep.id) {
                                sopTabIndex = i;
                                findCurrentData = true;
                            }
                        }
                    }

                    //for (let j = sopHistory.sopData.actionStepDatas.length -1; j >= 0; j--) {
                    //    if (sopHistory.sopData.actionStepDatas[j]._ActionStepHistory === null) {
                    //        continue;
                    //    }

                    //    // flow chart 설정
                    //    this.setCurrentActionStep(sopHistory.sopData, sopHistory.sopData.actionStepDatas[j].actionStep.id);
                    //    this.checkArrows(sopHistory.sopData);
                    //    await this.checkStepMembers(sopHistory.sopData);

                    //    sopTabIndex = i;
                    //    currentActionStep = sopHistory.sopData.actionStepDatas[j];
                    //    //return;
                    //}

                    //// flow chart 설정
                    //this.setCurrentActionStep(sopHistory.sopData, this.state.currentActionStep.actionStep.id);
                    //this.checkArrows(sopHistory.sopData);
                    //await this.checkStepMembers(sopHistory.sopData);                        
                }
                else {                    
                    // 옵션 : 새로운 이력이 있으면 새로운 SOP 탭으로 변경한다.
                    const stepLength = sopHistory.sopData.actionStepDatas.length;
                    for (let j = 0; j < stepLength; j++) {
                        const actionStepData = sopHistory.sopData.actionStepDatas[j];
                        const actionStepHistory = actionStepData._ActionStepHistory;
                        if (actionStepHistory) {

                            // flow chart 설정
                            this.setCurrentActionStep(sopHistory.sopData, actionStepData.actionStep.id);
                            this.checkArrows(sopHistory.sopData);
                            await this.checkStepMembers(sopHistory.sopData);                            

                            if (actionStepHistory.id === lastAccessActionStepHistoryID) {
                                sopTabIndex = i;
                                currentSopTabKey = sopHistory.key;
                                currentActionStep = actionStepData;                                
                            }
                        }
                    }
                }
            }

            if (!findCurrentData) {
                for (let i = 0; i < historyLength; i++) {
                    const sopHistory = sopHistories[i];
                    
                    for (let j = sopHistory.sopData.actionStepDatas.length -1; j >= 0; j--) {
                        if (sopHistory.sopData.actionStepDatas[j]._ActionStepHistory === null) {
                            continue;
                        }

                        this.setCurrentActionStep(sopHistory.sopData, sopHistory.sopData.actionStepDatas[j].actionStep.id);
                        //this.checkArrows(sopHistory.sopData);
                        //await this.checkStepMembers(sopHistory.sopData);

                        this.setState({
                            content: SopSimulatorResource.ID.menu.execSOP,
                            sopDatas: sopHistories,
                            sopTabIndex: i,
                            currentSopTabKey: sopHistory.key,
                            currentActionStep: sopHistory.sopData.actionStepDatas[j]
                        });

                        return;
                    }               
                }
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
        
    setCurrentActionStep(sopData, actionStepID) {
        if (sopData.actionStepDatas === null)
            return;

        for (var k = 0; k < sopData.actionStepDatas.length; k++) {
            const actionStepData = sopData.actionStepDatas[k];
            if (actionStepData.actionStep) {
                if (actionStepData.actionStep.id === actionStepID) {
                    sopData.currentActionStep = actionStepData;
                    break;
                }
                else {
                    sopData.currentActionStep = actionStepData;
                }
            }
        }
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

    closeSopData(index) {
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
            this.openDB(versionID, null);
        }
    }

    onChangeTab(index) {
        this.setState({ sopTabIndex: index, currentSopTabKey: this.state.sopDatas[index].key });
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

    render() {
        let ui = <></>;
        if (this.state.content === SopSimulatorResource.ID.menu.callSOP) {
            ui = <>
                <SopSimulatorSOPList openDB={this.openDB}/>
            </>
        } else if (this.state.content === SopSimulatorResource.ID.menu.execSOP) {
            ui = <>
                <SopSimulatorBody
                    changeContent={this.changeContent}
                    sopDatas={this.state.sopDatas}
                    sopTabIndex={this.state.sopTabIndex}
                    onChangeTab={this.onChangeTab}
                    closeSopData={this.closeSopData}
                    onChangeStep={this.onChangeStep}

                    currentSopTabKey={this.state.currentSopTabKey}
                    currentActionStep={this.state.currentActionStep}
                    teamDatas={this.state.teamDatas}
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

export default SopSimulator;