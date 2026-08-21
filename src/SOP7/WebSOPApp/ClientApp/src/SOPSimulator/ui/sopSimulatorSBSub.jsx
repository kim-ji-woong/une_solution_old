import React, { Component } from 'react';
import '../../Common/css/scroll.css';
import $ from 'jquery';
import { TabContent } from 'reactstrap';

import uis from '../../Common/css/ui.module.css';

import SopSimulatorResource from "../resource/id";
import SopSimulatorController from '../services/sopSimulatorController';
import SopController from '../../SOPManager/services/sopController';

import ProcessListSB from './processListSB';
import SopSimulatorSBChart from './sopSimulatorSBChart';
import MissionListSB from './missionListSB';

import Arrow from '../../Common/sections/components/arrow';
import SectionData from '../../Common/models/sections/sectionData';

class SopSimulatorSBSub extends Component {
    constructor(props) {
        super(props);

        this.state = {
            orderSopData: null,
            arrowDatas: null,
            currentActionStepID: 4,    // 현재 SOP 단계 ID
            currentActionStep: "관심",  // 현재 SOP 단계
            currentSection: null,      // 현재 임무 Data
            ActionStepHistoryID: -1,
            isBegin: false,
            prevProps: null
        }

        this.props = props;

        this.runSection = this.runSection.bind(this);
        this.runSectionFromChart = this.runSectionFromChart.bind(this);
        this.excuteSOP = this.excuteSOP.bind(this);
        this.moveCallPage = this.moveCallPage.bind(this);
        this.onProgressMission = this.onProgressMission.bind(this);
        this.onProgressInternalSpread = this.onProgressInternalSpread.bind(this);
        this.onProgressSendSMS = this.onProgressSendSMS.bind(this);
        this.beginSopData = this.beginSopData.bind(this);
        this.closeSopData = this.closeSopData.bind(this);
    }

    componentDidMount() {

        // Top Menu
        $('.' + uis.tabArea).on('click', 'a', function () {
            $(this).closest('li').addClass(uis.isActive).siblings().removeClass(uis.isActive);
        });

        // 판단문 yes or no combobox
        $('.' + uis.seleteBox).on('click', '.' + uis.seletedTxt, function () {
            $(this).closest('.' + uis.seleteBox).toggleClass(uis.isShow);
        })
            .on('click', '.' + "value", function () {
                var value = $(this).closest('li').data('val');
                $(this).closest('.' + uis.seleteBox).toggleClass(uis.isShow);
                $(this).closest('.' + uis.seleteBox).removeClass('.step01', '.step02', '.step03', '.step04').addClass(value);
                $(this).closest('.' + uis.seleteBox).find('.' + uis.seletedTxt).text($(this).text());
            });

        // 페이지 타이틀 
        $('#pageTitle').text("");
    }

    static getDerivedStateFromProps(nextProps, prevState) {
        if (nextProps === prevState.prevProps) {
            return prevState;
        }

        if (nextProps.sopTabIndex < 0)
            return null;

        const [orderSopData, arrowDatas] = SopSimulatorSBSub.makeOrderSopData(nextProps);

        var currentSection = prevState.currentSection;
        var actionStepHistoryID = prevState.ActionStepHistoryID;
        var currentActionStep = prevState.currentActionStep;
        var currentActionStepID = prevState.currentActionStepID;
        var isBegin = prevState.isBegin;
        if (nextProps.sopDatas.length > 0) {
            const sopData = nextProps.sopDatas[nextProps.sopTabIndex];
            const actionStep = sopData.currentActionStep.actionStep;

            currentActionStep = actionStep.stepName;
            currentActionStepID = actionStep.id;

            const actionStepDatasLength = sopData.actionStepDatas.length;
            for (var i = 0; i < actionStepDatasLength; i++) {
                const actionStepData = sopData.actionStepDatas[i];
                const stepMemberData = actionStepData.stepMemberDatas[0];

                if (currentActionStep === actionStepData.stepName) {
                    if (actionStepData.componentHistories !== undefined) { 
                        // 진행중이었던 SOP인가?(=componentHistories가 있다)
                        const historyLength = actionStepData.componentHistories.length;
                        const lastHistory = actionStepData.componentHistories[historyLength - 1];

                        const sectionLength = stepMemberData.sections.length;
                        for (var j = 0; j < sectionLength; j++) {
                            const sectionData = stepMemberData.sections[j]

                            for (var k = 0; k < historyLength; k++) {
                                const history = actionStepData.componentHistories[k];
                                if (sectionData.componentType === history.componentType && sectionData.id === history.componentID) {
                                    // 해당 section의 실행 기록 인가?
                                    // 맞으면 상태값 넣기
                                    sectionData.status = history.status;

                                    if (actionStepData.componentHistoryDetails !== null && actionStepData.componentHistoryDetails !== undefined) {
                                        // 해당 section의 실행 기록이 있다면 detail 기록도 있을 수 있음
                                        // Detail 기록 넣기
                                        for (var m = 0; m < actionStepData.componentHistoryDetails.length; m++) {
                                            const detail = actionStepData.componentHistoryDetails[m];
                                            if (detail.componentHistoryID === history.id) {
                                                if (sectionData.componentHistoryDetails === undefined) {
                                                    sectionData.componentHistoryDetails = [];
                                                }
                                                sectionData.componentHistoryDetails.push(detail);
                                            }
                                        }
                                    }
                                }
                            }

                            if (lastHistory) {
                                if (sectionData.componentType === lastHistory.componentType && sectionData.id === lastHistory.componentID) {
                                    currentSection = sectionData; // 진행중인 SOP를 열었으므로 현재section에 진행중이었던 section을 넣는다
                                    actionStepHistoryID = lastHistory.actionStepHistoryID; // 진행중인 SOP를 열었으므로 해당 SOP의 ID를 넣는다                                    
                                }
                            }
                        }

                        break;
                    }
                    else {
                        // 새로운 SOP
                        currentSection = stepMemberData.orderSections[0]; // 새로운 SOP를 열었으므로 현재section에 첫번째section을 넣는다
                        actionStepHistoryID = -1; // 새로운 SOP를 열었으므로 -1
                        break;
                    }
                }
            }
        }

        return {
            orderSopData: orderSopData[nextProps.sopTabIndex],
            arrowDatas: arrowDatas[nextProps.sopTabIndex],
            currentActionStepID: currentActionStepID,
            currentActionStep: currentActionStep,
            currentSection: currentSection,
            ActionStepHistoryID: actionStepHistoryID,
            prevProps: nextProps
        };
    }

    // sop 임무 순서대로 정렬
    static makeOrderSopData(props) {
        const sopDataLength = props.sopDatas.length;

        var orderSectionDatas = new Array(sopDataLength); // 실행중인 sop 개수만큼 배열 만듦
        var orderSectionArrowDatas = []; // 실행중인 sop 개수만큼 배열 만듦

        for (var k = 0; k < sopDataLength; k++) {            
            const actionStepDatas = props.sopDatas[k].actionStepDatas;
            const actionStepDatasCount = actionStepDatas.length;
            for (var i = 0; i < actionStepDatasCount; i++) {
                if (props.sopDatas[k].currentActionStep.stepName === actionStepDatas[i].stepName) {
                    const stepMemberData = actionStepDatas[i].stepMemberDatas[0];

                    const sectionDatas = stepMemberData.sections;
                    const arrowDatas = stepMemberData.arrows;
                    orderSectionArrowDatas[k] = arrowDatas; 

                    const sectionDatasCount = sectionDatas.length;
                    orderSectionDatas[k] = new Array(sectionDatasCount); // 총 임무 개수만큼 배열 만듦

                    for (var j = 0; j < sectionDatasCount; j++) {
                        if (sectionDatas[j].componentType === 2) {
                            // 설명은 임무가 아니므로 제외                           
                            orderSectionDatas[k].pop();
                            continue;
                        }
                        var index = sectionDatas[j].sectionNumber -1;
                        if (index === null) { // 시작은 null로 입력되어있으므로 0으로 판단
                            index = 0;
                        }
                        orderSectionDatas[k][index] = sectionDatas[j];
                    }

                    stepMemberData.orderSections = orderSectionDatas[k];
                    break;
                }
            }
        }

        return [orderSectionDatas, orderSectionArrowDatas];
    }

    runSectionFromChart(selectSection) {
        if (this.state.ActionStepHistoryID === -1 && selectSection.componentType !== 3 && !selectSection.isBegin) {
            // SOP가 시작하지 않았는데 시작이 아닌 다른 세션을 눌렀을 때 
            return;
        }

        if (selectSection.componentType === 3 && selectSection.isBegin) {
            // 시작한 경우 다음 세션을 찾아야하므로 runSection 함수에 넘긴다
            this.runSection(selectSection);
        }
        else {
            var preSection = this.state.currentSection;
            this.runSection2(selectSection, preSection, true);
        }
    }

    // decisionResult 는 판단문일때만 있음
    runSection(selectSection, decisionResult) {
        console.log(this.state.arrowDatas);

        var preSection = this.state.currentSection;  // 현재 세션
        var nextSection = null; // 다음 세션
        var isSkip = false; // skip하는가 ? 

        if (preSection !== selectSection && selectSection.componentType === 3 && !selectSection.isBegin) {
            // Skip하고 종료를 누른 경우 다음 임무가 없으므로 그냥 넘긴다
            nextSection = selectSection;
            isSkip = true;
        }
        else {// 현재임무(preSection)를 완료했거나 skip하고 다른 임무를 완료했다면            
            if (preSection !== selectSection) {
                // skip하고 다른 A임무를 완료했다면 A임무를 현재section에 넣고 화살표값 이용해서 다음 section을 찾는다
                // 즉, 현재 임무가된 A임무를 완료처리했으므로 isSkip은 false가 된다.
                preSection = selectSection;                
            }

            const nType = (preSection.componentType << 24);
            const curBeginComponentID = (nType | preSection.id);

            const arrowCount = this.state.arrowDatas.length;
            for (var i = 0; i < arrowCount; i++) {

                const arrowData = this.state.arrowDatas[i];

                var beginComponentID = arrowData.beginComponentID;
                var endComponentID = arrowData.endComponentID;
                var text = arrowData.text;

                if (beginComponentID === undefined || endComponentID === undefined || text === undefined) {
                    const componentIDs = Arrow.getArrowInfo2(arrowData, this.state.orderSopData);

                    if (componentIDs === null) {
                        continue;
                    }  

                    beginComponentID = ((componentIDs[0] << 24) | componentIDs[1]);
                    endComponentID = ((componentIDs[2] << 24) | componentIDs[3]);
                }

                if (curBeginComponentID === beginComponentID) {
                    if (this.state.currentSection.componentType === 1) {
                        // 판단문은 분기에 맞춰 다음 임무로 진행한다
                        const resultText = text.toLowerCase();
                        //var result = false;
                        //if (resultText.toLowerCase() === "yes" || resultText === "예" || resultText === "네") {
                        //    result = true;
                        //}

                        if (!resultText || decisionResult.toLowerCase() !== resultText) {
                            continue;
                        }
                    }

                    const [endSectionType, endSectionID] = SopController.getSectionInfo(endComponentID);

                    if (endSectionType === 2) // 설명으로 이어진 화살표는 패스
                        continue;

                    const dataCount = this.state.orderSopData.length;
                    for (var j = 0; j < dataCount; j++) {
                        if (this.state.orderSopData[j].componentType === endSectionType &&
                            this.state.orderSopData[j].id === endSectionID) {
                            // 다음 임무 추출
                            nextSection = this.state.orderSopData[j];
                            break;
                        }
                    }
                    if (nextSection !== null)
                        break;
                }
            }
        }
        if (nextSection !== null || (nextSection === null && preSection.componentType === 3 && !preSection.isBegin))
            this.runSection2(nextSection, preSection, isSkip);
    }

    // 다음 임무 진행 이벤트
    async runSection2(section, preSection, isSkip) {
        var actionStepHistoryID = this.state.ActionStepHistoryID;
        if (preSection.componentType === 3) {
            if (preSection.isBegin) {
                // 시작
                const sensorZoneHistoryID = this.props.sopDatas[this.props.sopTabIndex].sensorZoneHistoryID;
                actionStepHistoryID = await this.excuteSOP(this.state.currentActionStepID, '테스트', preSection.id, sensorZoneHistoryID);                
            }
            else {
                // 종료
                const sensorZoneHistoryID = this.props.sopDatas[this.props.sopTabIndex].sensorZoneHistoryID;
                await this.closeSOP(actionStepHistoryID, sensorZoneHistoryID);
            }
        }
        else if (section.componentType === 3 && isSkip) {
            if (!section.isBegin) {
                // 종료
                const sensorZoneHistoryID = this.props.sopDatas[this.props.sopTabIndex].sensorZoneHistoryID;
                await this.closeSOP(actionStepHistoryID, sensorZoneHistoryID);
            }
        }

        var histories = this.getHistories(this.props.sopDatas[this.props.sopTabIndex]);

        if (!isSkip) {
            // 이전 임무 완료 처리
            // 완료 처리가 되었다면
            const history = await this.progressSOP(actionStepHistoryID, preSection.id, preSection.componentType, SectionData.Status_Done, preSection.text);
            histories.push(history);
            preSection.status = SectionData.Status_Done; // 상태값 업데이트
        }

        var isEnd = false;
        if (section !== null) {
            if (section.componentType === 3 && isSkip) {
                // 종료 완료 처리
                const history3 = await this.progressSOP(actionStepHistoryID, section.id, section.componentType, SectionData.Status_Done, section.text);
                histories.push(history3);
                section.status = SectionData.Status_Done; // 상태값 업데이트

                if (!section.isBegin)
                    isEnd = true;
            }
            else {
                // 현재 임무 대기 처리
                const history2 = await this.progressSOP(actionStepHistoryID, section.id, section.componentType, SectionData.Status_Run, section.text);
                histories.push(history2);
                section.status = SectionData.Status_Run; // 상태값 업데이트
            }
        }
        else {
            // 종료
            // 순차적으로 진행하여 종료시 section null임
            isEnd = true;
        }

        if (isEnd) {
            actionStepHistoryID = -1;
            this.props.closeSopData(this.props.sopTabIndex);
            
            this.setState({ isBegin: false, ActionStepHistoryID: actionStepHistoryID });
            return;
        }

        this.setState({ isBegin: true, ActionStepHistoryID: actionStepHistoryID, currentSection: section });

        if (section.autoRun) {
            this.runSection(section);
        }
    }

    getHistories(sopData) {
        var histories = [];
        const actionStepDatasLength = sopData.actionStepDatas.length;
        for (var i = 0; i < actionStepDatasLength; i++) {
            const actionStepData = sopData.actionStepDatas[i];
            if (actionStepData.stepName === this.state.currentActionStep) {
                if (actionStepData.componentHistories === undefined) {
                    actionStepData.componentHistories = [];
                }
                histories = actionStepData.componentHistories;
                break;
            }
        }

        return histories;
    }

    getHistoryDetails(sopData) {
        var details = [];
        const actionStepDatasLength = sopData.actionStepDatas.length;
        for (var i = 0; i < actionStepDatasLength; i++) {
            const actionStepData = sopData.actionStepDatas[i];
            if (actionStepData.stepName === this.state.currentActionStep) {
                if (actionStepData.componentHistoryDetails === undefined) {
                    actionStepData.componentHistoryDetails = [];
                }
                details = actionStepData.componentHistoryDetails;
                break;
            }
        }

        return details;
    }

    async excuteSOP(actionStepID, position, componentID, sensorZoneHistoryID) {
        const userID = 1;
        const actionStepHistoryID = await SopSimulatorController.excuteSOP(actionStepID, position, userID, componentID, sensorZoneHistoryID);
        return actionStepHistoryID;
    }

    async closeSOP(actionStepHistoryID, sensorZoneHistoryID) {
        await SopSimulatorController.closeSOP(actionStepHistoryID, sensorZoneHistoryID);
    }

    async progressSOP(actionStepHistoryID, componentID, componentType, status, text) {
        const userID = 1;
        const history = await SopSimulatorController.progressSOP(actionStepHistoryID, componentID, componentType, userID, status, text);
        return history;
    }

    async progressMission(componentHistoryID, dataIndex, datai) {
        const userID = 1;
        const history = await SopSimulatorController.progressMission(componentHistoryID, dataIndex, datai);
        return history;
    }

    onChangeTab(index) {
        if (index === this.props.sopTabIndex)
            return;

        this.props.onChangeTab(index);
    }

    moveCallPage() {
        this.props.changeContent(SopSimulatorResource.ID.menu.callSOP);
    }

    // SOP Chart에서 단계 변경
    onChangeActionStep = (stepName, stepID) => {
        this.props.onChangeStep(stepID, stepName);
    }

    setCheckStatus(sopData, checked, section, missionIndex) {
        var histories = [];
        const actionStepDatasLength = sopData.actionStepDatas.length;
        for (var i = 0; i < actionStepDatasLength; i++) {
            const actionStepData = sopData.actionStepDatas[i];
            if (actionStepData.stepName === this.state.currentActionStep) {
                const orgSection = actionStepData.stepMemberDatas[0].sections;
                for (var j = 0; j < orgSection.length; j++) {
                    if (section === orgSection[j]) {
                        if (orgSection[j].componentType === 0) {
                            orgSection[j].missions[missionIndex].checkStatus = checked;
                            break;
                        }
                        else {
                            orgSection[j].checkStatus = checked;
                        }
                    }
                }
            }
        }

        return histories;
    }

    async onProgressMission(checked, section, missionIndex) {
        var nStatus = section.status;
        if (!nStatus)
            nStatus = SectionData.Status_Normal;

        this.setCheckStatus(this.props.sopDatas[this.props.sopTabIndex], checked, section, missionIndex);

        var text = section.text + ' ' + (missionIndex + 1) + '번째';
        if (checked)
            text += '체크';
        else
            text += '체크 해제';

        var histories = this.getHistories(this.props.sopDatas[this.props.sopTabIndex]);

        // section 뒤져서 완료된 임무라면status(3), 실행한적 없는 임무라면 status(1) 현재임무라면 status(2)로 insert
        const history = await this.progressSOP(this.state.ActionStepHistoryID, section.id, section.componentType, nStatus, text);
        histories.push(history);

        var historyDetails = this.getHistoryDetails(this.props.sopDatas[this.props.sopTabIndex]);

        const detail = await this.progressMission(history.id, missionIndex, (checked) ? 1 : 0);
        historyDetails.push(detail);

        if (checked && section.componentType === 6) { 
            // 내부상황전파가 체크되면 다음 임무로 넘어간다
            // 프로세스는 임무 목록이 여러개일 수 있으므로 프로세스쪽에서 체크한다
            this.runSection(section);
        }

        this.setState({ orderSopData: this.state.orderSopData });
    }

    async onProgressInternalSpread(section) {
        var nStatus = section.status;
        if (!nStatus)
            nStatus = SectionData.Status_Normal;

        var text = section.text;
        if (section.isSMS)
            text += '_SMS전파';
        else if (section.isBroadcast)
            text += '_방송전파';
        else if (section.isEmail)
            text += '_Email전파';

        // commandHistory 
        var histories = this.getHistories(this.props.sopDatas[this.props.sopTabIndex]);
        const history = await this.progressSOP(this.state.ActionStepHistoryID, section.id, section.componentType, nStatus, text);
        histories.push(history);

        // commandHistoryDetail
        var historyDetails = this.getHistoryDetails(this.props.sopDatas[this.props.sopTabIndex]);
        const detail = await this.progressMission(history.id, 0, 2);
        historyDetails.push(detail);

        let message = "";
        if (this.props.sopDatas[this.props.sopTabIndex].alarmPosition) {
            message = this.props.sopDatas[this.props.sopTabIndex].alarmPosition + section.message;
        }
        else {
            message = section.message
        }
        // 전파
        await SopSimulatorController.progressInternalSpread(message, section.isSMS, section.isBroadcast, section.isEmail, section.receivers);
    }

    async onProgressSendSMS(section, mission, missionIndex) {
        var nStatus = section.status;
        if (!nStatus)
            nStatus = SectionData.Status_Normal;

        var text = section.text + ' ' + (missionIndex + 1) + '번째_SMS전파';
        
        console.log(mission);
        // commandHistory 
        var histories = this.getHistories(this.props.sopDatas[this.props.sopTabIndex]);
        const history = await this.progressSOP(this.state.ActionStepHistoryID, section.id, section.componentType, nStatus, text);
        histories.push(history);

        // commandHistoryDetail
        var historyDetails = this.getHistoryDetails(this.props.sopDatas[this.props.sopTabIndex]);
        const detail = await this.progressMission(history.id, 0, 2);
        historyDetails.push(detail);

        // 전파
        await SopSimulatorController.progressInternalSpread(mission.missionText, true, false, false, section.receivers);
    }

    beginSopData() {
        if (this.state.currentSection.componentType === 3 && this.state.currentSection.isBegin) {
            this.runSectionFromChart(this.state.currentSection);
        }
    }
    async closeSopData() {
        if (this.state.ActionStepHistoryID > 0) {
            const sensorZoneHistoryID = this.props.sopDatas[this.props.sopTabIndex].sensorZoneHistoryID;
            await this.closeSOP(this.state.ActionStepHistoryID, sensorZoneHistoryID);
        }

        this.props.closeSopData(this.props.sopTabIndex);
    }

    // 열려있는 SOP 탭 만들기
    setSopTabUI() {
        if (this.props.sopDatas === null || this.props.sopDatas.length === 0)
            return null;

        var sopTabUI = [];

        for (var i = 0; i < this.props.sopDatas.length; i++) {

            const disasterName = this.props.sopDatas[i].disaster.disasterName;
            const stepName = this.props.sopDatas[i].currentActionStep.stepName;
            const className = (this.props.sopTabIndex === i) ? uis.isActive : null;
            const index = i;
            sopTabUI.push(<li className={className} key={i}><a onClick={() => this.onChangeTab(index)}>{disasterName}({stepName})</a></li>);
        }

        return sopTabUI;
    }

    render() {
        const tabUI = this.setSopTabUI();

        if (this.props.sopDatas[this.props.sopTabIndex].sensorZoneHistoryID !== undefined) {
            // 알람 발생하여 열린 sop는 sensorZoneHistoryID를 가지고 있는데 자동 시작이 안된경우 sop를 시작한다
            if (this.state.currentSection.componentType === 3 && this.state.currentSection.isBegin && !this.state.isBegin) {
                if (!this.state.isBegin) {
                    this.setState({ isBegin: true });
                }
                this.runSection(this.state.currentSection, null);
            }
        }

        return (
            <>                
                {/* <section className={uis.appContainerWrapSop + " " + uis.clfix}> */}
                    <section className={uis.appContainerWrapSop}>
                    <div className={uis.appContainer + " " + uis.pgProgress}>

                        <div className={uis.tabArea}>
                            {/* <ul className={uis.clfix}> */}
                            <ul>
                                {tabUI}
                            </ul>
                        </div>   

                        <ProcessListSB />
                        <SopSimulatorSBChart
                            sopData={this.props.sopDatas[this.props.sopTabIndex]}
                            onChangeActionStep={this.onChangeActionStep}
                            onSelectComponent={this.runSectionFromChart}
                            currentSection={this.state.currentSection}
                            sopTabIndex={this.props.sopTabIndex}
                            beginSopData={this.beginSopData}
                            closeSopData={this.closeSopData}
                        />
                        <MissionListSB
                            orderSopData={this.state.orderSopData}
                            arrowDatas={this.state.arrowDatas}
                            currentSection={this.state.currentSection}
                            runSection={this.runSection}
                            onProgressInternalSpread={this.onProgressInternalSpread}
                            runSectionFromChart={this.runSectionFromChart}
                            onProgressMission={this.onProgressMission}
                            onProgressSendSMS={this.onProgressSendSMS}
                            sopTabIndex={this.props.sopTabIndex}
                        />
                    </div>
                </section>

                {/*<SopSimulatorSBpopup />*/}
        	</>
		);
	}
}

export default SopSimulatorSBSub;