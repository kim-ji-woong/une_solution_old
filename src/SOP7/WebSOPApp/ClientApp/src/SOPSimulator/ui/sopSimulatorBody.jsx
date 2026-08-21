import React, { Component } from 'react';
import '../../Common/css/scroll.css';
import $ from 'jquery';
import { TabContent } from 'reactstrap';

import uis from '../../Common/css/ui.module.css';
import uneStyles from '../../Common/css/uneCommon.module.css';
import uneCommon from '../../Common/css/uneCommon.module.css';

import SopSimulatorResource from "../resource/id";
import SopSimulatorController from '../services/sopSimulatorController';
import SopController from '../../SOPManager/services/sopController';

import ProcessListSB from './processListSB';
import SopSimulatorSBChart from './sopSimulatorSBChart';
import MissionListSB from './missionListSB';

import BeginOption from './popup/beginOption';
import SummarySOP from './popup/endPopup';

import SectionData from '../../Common/models/sections/sectionData';
import SessionString from '../../Common/js/sessionString';
import ProjectResource from '../../Root/resource/id';

import onGoingSopImg from '../../Common/image/icon/onGoingSopRedImg.png';

class SopSimulatorBody extends Component {
    
    constructor(props) {
        super(props);

        this.state = {
            content: '',
            currentActionStep: "관심",  // 현재 SOP 단계
            orderSopData: null,
            arrowDatas: null,
            currentActionStepID: 4,    // 현재 SOP 단계 ID
            ActionStepHistoryID: -1,
            isBegin: false,
            // 위에 삭제하기
                        
            currentSection: null,      // 현재 임무 Data            
            currentActionStepHistoryID:-1,            
            sections: null,
            loginUser: null,            
            endPopup: null,

            sensorNames: '',

            prevProps: null,

            tabMaxCount: 6 // 한 페이지에 표시할 탭 최대 개수
        }

        this.props = props;

        this.runSection = this.runSection.bind(this);
        this.runSectionFromChart = this.runSectionFromChart.bind(this);
        this.excuteSOP = this.excuteSOP.bind(this);
        this.onProgressMission = this.onProgressMission.bind(this);
        this.onProgressSpread = this.onProgressSpread.bind(this);
        this.beginSopData = this.beginSopData.bind(this);
        this.beginSOP = this.beginSOP.bind(this);
        this.closeSOP = this.closeSOP.bind(this);
    }

    componentDidMount() {
        let userInfo = ProjectResource.getUserInfo();
        if (userInfo !== null || userInfo !== undefined) {
            this.setState({ loginUser: userInfo });
        }

        // Top Menu
        $('.' + uis.tabArea).on('click', 'a', function () {
            $(this).closest('li').addClass(uis.isActive).siblings().removeClass(uis.isActive);
        });

        // 판단문 yes or no combobox
        $('.' + uis.seleteBox).on('click', '.' + uis.seletedTxt, function () {
            $(this).closest('.' + uis.seleteBox).toggleClass(uis.isShow);
        })
            .on('click', '.' + "value", function () {
                let value = $(this).closest('li').data('val');
                $(this).closest('.' + uis.seleteBox).toggleClass(uis.isShow);
                $(this).closest('.' + uis.seleteBox).removeClass('.step01', '.step02', '.step03', '.step04').addClass(value);
                $(this).closest('.' + uis.seleteBox).find('.' + uis.seletedTxt).text($(this).text());
            });

        // 페이지 타이틀 
        $('#pageTitle').text("");


/*        $('.' + uneStyles.rightt).click(function () {
            $('.' + uneStyles.tabArea).append(uneStyles.testt);
        });*/

        this.requestSensorName(this.props.sopDatas[this.props.sopTabIndex].sensorZoneHistoryID);
    }

    static getDerivedStateFromProps(nextProps, prevState) {
        if (nextProps === prevState.prevProps) {
            return prevState;
        }

        let sections = prevState.sections;
        let currentSection = prevState.currentSection;
        let currentActionStepHistoryID = -1;

        if (nextProps.sopDatas !== null || nextProps.sopDatas.length - 1 >= nextProps.sopTabIndex) {
            let sopRunData = nextProps.sopDatas[nextProps.sopTabIndex];
            const stepLength = sopRunData.sopData.actionStepDatas.length;
            for (let i = 0; i < stepLength; i++) {
                const actionStepData = sopRunData.sopData.actionStepDatas[i];
                if (!actionStepData.actionStep)
                    continue;

                if (actionStepData.actionStep.id === sopRunData.sopData.currentActionStep.actionStep.id) {
                    sections = actionStepData.stepMemberDatas[0].sections;
                    currentSection = actionStepData.currentSection;
                    if (actionStepData._ActionStepHistory !== null)
                        currentActionStepHistoryID = actionStepData._ActionStepHistory.id;

                    // 임무 상태 (status)값 할당
                    if (actionStepData.componentHistoryData !== null) {
                        for (let j = 0; j < actionStepData.stepMemberDatas[0].sections.length; j++) {
                            const section = actionStepData.stepMemberDatas[0].sections[j];
                            for (let k = 0; k < actionStepData.componentHistoryData.length; k++) {
                                const componentHistory = actionStepData.componentHistoryData[k].componentHistory;
                                if (componentHistory.componentID === section.id && componentHistory.componentType === section.componentType) {
                                    section.status = componentHistory.status;
                                }
                            }
                        }
                    }
                    else {
                        // 진행 이력이 없으므로 첫번째 임무로 지정한다
                        currentSection = actionStepData.stepMemberDatas[0].sections[0];
                    }

                    break;
                }
            }
        }

        return {
            sections: sections,
            currentSection: currentSection,
            currentActionStepHistoryID: currentActionStepHistoryID,
            prevProps: nextProps
        };
    }

    runSectionFromChart(selectSection) {
        if (this.state.currentActionStepHistoryID === -1 && !selectSection.isBegin) {
            // SOP가 시작하지 않았는데 시작이 아닌 다른 세션을 눌렀을 때 
            return false;
        }

        if (this.state.currentActionStepHistoryID > 0 && selectSection.componentType === 3 && selectSection.isBegin) {
            // SOP 시작했는데 시작section 눌렀을 때
            return false;
        }

        if (selectSection.componentType === 3 && selectSection.isBegin) {
            // 시작한 경우 다음 세션을 찾아야하므로 runSection 함수에 넘긴다
            this.runSection(selectSection);
        }
        else {
            //var preSection = this.state.currentSection;
            //this.runSection2(selectSection, preSection, true);

            this.runSection(selectSection, undefined, true);
        }

        return true;
    }

    async requestSensorName(sensorZoneHistoryID) {
        let sensorName = '수동';
        if (sensorZoneHistoryID && sensorZoneHistoryID > 0) {
            sensorName = await SopSimulatorController.requestSensorName(sensorZoneHistoryID);
        }
        this.setState({ sensorNames: sensorName });
    }

    // decisionResult 는 판단문일때만 있음    
    async runSection(section, decisionValue, fromChart) {        
        const sopKey = this.props.currentSopTabKey;
        const actionStepID = this.props.sopDatas[this.props.sopTabIndex].sopData.currentActionStep.actionStep.id;
        const actionStepHistoryID = this.state.currentActionStepHistoryID;
        const userID = this.state.loginUser.id;

        if (this.props.sopDatas[this.props.sopTabIndex].position === null && this.state.currentActionStepHistoryID === -1) {
            this.changeContent(SopSimulatorResource.ID.menu.beginSOPOption);
        }
        else {
            let isSkip = false;
            if (!fromChart) {
                //const currentSection = this.props.sopDatas[this.props.sopTabIndex].sopData.currentActionStep.currentSection;
                const currentSection = this.state.currentSection;
                if (currentSection.componentType !== section.componentType || currentSection.id !== section.id) {
                    isSkip = true;
                }
            }

            const history = await SopSimulatorController.runSection(sopKey, actionStepID, actionStepHistoryID, section.id, section.componentType, userID, section.text, decisionValue, isSkip);
            return history;
        }

        //if (section.componentType === 3) {
        //    if (section.isBegin /*this.props.sopDatas[this.props.sopTabIndex].position === null && this.state.currentActionStepHistoryID === -1*/) {
        //        this.changeContent(SopSimulatorResource.ID.menu.beginSOPOption);
        //    }
        //    else {
        //        await this.closeSOP();
        //    }
        //}
        //else {
        //    let isSkip = false;
        //    if (!fromChart) {
        //        //const currentSection = this.props.sopDatas[this.props.sopTabIndex].sopData.currentActionStep.currentSection;
        //        const currentSection = this.state.currentSection;
        //        if (currentSection.componentType !== section.componentType || currentSection.id !== section.id) {
        //            isSkip = true;
        //        }
        //    }

        //    const history = await SopSimulatorController.runSection(sopKey, actionStepID, actionStepHistoryID, section.id, section.componentType, userID, section.text, decisionValue, isSkip);
        //    return history;
        //}
    }

    async beginSOP(beginTime, position) {
        const sopRunData = this.props.sopDatas[this.props.sopTabIndex];

        const date = this.getMakeDateTime(beginTime);

        // sop 시작
        await this.excuteSOP(date, sopRunData.sopData.currentActionStep.actionStep.id, position, this.state.loginUser.id, sopRunData.sensorZoneHistoryID);
        
        this.changeContent('');
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

    async excuteSOP(beginTime, actionStepID, position, userID, sensorZoneHistoryID) {
        const actionStepHistoryID = await SopSimulatorController.excuteSOP(beginTime, actionStepID, position, userID, sensorZoneHistoryID);
        return actionStepHistoryID;
    }

    //async progressSOP(actionStepHistoryID, componentID, componentType, status) {
    //    const userID = 1;
    //    const history = await SopSimulatorController.progressSOP(actionStepHistoryID, componentID, componentType, userID, status);
    //    return history;
    //}

    onChangeTab(index) {
        if (index === this.props.sopTabIndex)
            return;

        this.props.onChangeTab(index);

        this.requestSensorName(this.props.sopDatas[index].sensorZoneHistoryID);
    }

    // SOP Chart에서 단계 변경
    onChangeActionStep = (stepName, stepID) => {
        this.props.onChangeStep(stepID, stepName);
    }

    // 임무 체크
    async onProgressMission(checked, section, dataIndex) {
        let nStatus = section.status;
        if (!nStatus)
            nStatus = SectionData.Status_Normal;

        const sopKey = this.props.currentSopTabKey;
        const actionStepHistoryID = this.state.currentActionStepHistoryID;
        const userID = this.state.loginUser.id;

        // section 뒤져서 완료된 임무라면status(3), 실행한적 없는 임무라면 status(1) 현재임무라면 status(2)로 insert
        
        await SopSimulatorController.progressMission(
            sopKey,
            actionStepHistoryID,
            section.componentType, section.id,
            dataIndex, nStatus, userID, checked);
    }

    // 상황 전파
    async onProgressSpread(section, dataIndex, isSMS, isEmail, isBroadcast, isSiren, message) {
        let nStatus = section.status;
        if (!nStatus)
            nStatus = SectionData.Status_Normal;

        const sopKey = this.props.currentSopTabKey;
        const actionStepHistoryID = this.state.currentActionStepHistoryID;
        const userID = this.state.loginUser.id;

        // 전파
        await SopSimulatorController.progressSpread(
            sopKey,
            actionStepHistoryID,
            section.componentType, section.id,
            dataIndex, nStatus, userID,
            isSMS, isEmail, isBroadcast, isSiren,
            message);
    }

    beginSopData() {
        if (this.state.currentSection.componentType === 3 && this.state.currentSection.isBegin) {
            this.runSectionFromChart(this.state.currentSection);
        }
    }

    async closeSOP() {
        const sopRunData = this.props.sopDatas[this.props.sopTabIndex];
        const actionStepHistory = sopRunData.sopData.currentActionStep._ActionStepHistory;
        if (actionStepHistory !== null) {

            const endTime = this.getMakeDateTime(new Date());

            actionStepHistory.endTime = endTime;
            // SOP 결과 요약창 사용?
            if (this.props.commonSettings.UseResultSummary === 'true') {
                this.setState({ content: SopSimulatorResource.ID.menu.summarySOP });
            }
            else {
                await this.props.closeSOP(this.props.sopTabIndex, actionStepHistory.id, actionStepHistory.endTime);
            }
        }
        else {
            // 시작 전 SOP는 탭만 없애준다
            await this.props.closeSOP(this.props.sopTabIndex, null, null, null);
        }
    }

    // 열려있는 SOP 탭 만들기
    getSopTabUI() {
        if (this.props.sopDatas === null || this.props.sopDatas.length === 0)
            return null;

        let sopTabUI = [];

        let beginIndex = 0;
        let endIndex = 0;
        if (this.props.sopDatas.length <= this.state.tabMaxCount) {
            beginIndex = 0;
            endIndex = this.props.sopDatas.length;
        }
        else {            
            let value = parseInt(this.props.sopDatas.length / this.state.tabMaxCount);
            let remainder = this.props.sopDatas.length % this.state.tabMaxCount;
            
            let bundle = parseInt((this.props.sopTabIndex) / this.state.tabMaxCount);
            beginIndex = bundle * this.state.tabMaxCount;
            endIndex = beginIndex + 6;
        }
        
        for (let i = beginIndex; i < endIndex; i++) {

            if (this.props.sopDatas.length - 1 < i) {
                continue;
            }

            const disasterName = this.props.sopDatas[i].sopData.disaster.disasterName;
            const stepName = (this.props.sopDatas[i].sopData.currentActionStep) ? this.props.sopDatas[i].sopData.currentActionStep.stepName : '';
            const className = (this.props.sopTabIndex === i) ? uis.isActive : null;
            const index = i;

            // position 값이 있으면 실행중임
            if (this.props.sopDatas[i].position) {
                sopTabUI.push(<li className={className} key={i}><a onClick={() => this.onChangeTab(index)}>{disasterName}({stepName})</a><img className={uis.onGoingSopImg} src={onGoingSopImg} /></li>);
            }
            else {
                sopTabUI.push(<li className={className} key={i}><a onClick={() => this.onChangeTab(index)}>{disasterName}({stepName})</a></li>);
            }            
        }

        sopTabUI.push(<li className={uneStyles.posiRelative} key={'tabPlus'} onClick={() => this.props.changeContent(SopSimulatorResource.ID.menu.callSOP)}><a className={uneStyles.plus + " " + uneStyles.posiAbsolute}></a></li>);

        return sopTabUI;
    }

    changeContent = (content) => {
        this.setState({ content: content });
    }

    onClickPrevTab = () => {
        const curIndex = this.props.sopTabIndex;
        if (curIndex - 1 < 0 || this.props.sopDatas.length - 1 < curIndex - 1)
            return;

        this.props.onChangeTab(curIndex - 1);
    }

    onClickNextTab = () => {
        const curIndex = this.props.sopTabIndex;
        if (this.props.sopDatas.length - 1 < curIndex + 1)
            return;

        this.props.onChangeTab(curIndex + 1);
    }

    getPopup() {
        if (this.state.content === SopSimulatorResource.ID.menu.beginSOPOption) {
            const sopData = this.props.sopDatas[this.props.sopTabIndex].sopData;
            const title = sopData.disasterCategory.categoryName + ' → ' + sopData.subDisasterCategory.subCategoryName + ' → ' + sopData.disaster.disasterName + ' → ' + sopData.currentActionStep.stepName;
            return <BeginOption changeContent={this.changeContent} beginSOP={this.beginSOP} title={title}/>
        }
        else if (this.state.content === SopSimulatorResource.ID.menu.summarySOP) {
            const sopRunData = this.props.sopDatas[this.props.sopTabIndex];

            return <SummarySOP changeContent={this.changeContent} closeSOP={this.props.closeSOP} sopRunData={sopRunData} loginUser={this.state.loginUser} />
        }

        return <></>;
    }

    render() {
        const tabUI = this.getSopTabUI();

        return (
            <>                
                {/* <section className={uis.appContainerWrapSop + " " + uis.clfix + " " + uneCommon.paddingTop60}> */}
                 <section className={uis.appContainerWrapSop + " " + uneCommon.paddingTop60}>
                    <div className={uis.appContainer + " " + uis.pgProgress}>

                        <div className={uis.tabArea + " " + uneStyles.tabArea}>
                            {/*<div className={uneStyles.tabAreaLeft}>화살표 위치</div>*/}
                            <div className={uneStyles.squaree}>
                                <div className={uneStyles.leftt} onClick={this.onClickPrevTab}></div>
                                <div className={uneStyles.rightt} onClick={this.onClickNextTab}></div>
                            </div>
                            {/* <ul className={uis.clfix}> */}
                            <ul>
                                {tabUI}
                            </ul>
                        </div>   

                        <ProcessListSB sopRunData={this.props.sopDatas[this.props.sopTabIndex]} sensorNames={this.state.sensorNames} />
                        
                        <SopSimulatorSBChart
                            sopData={this.props.sopDatas[this.props.sopTabIndex].sopData}
                            onChangeActionStep={this.onChangeActionStep}
                            onSelectComponent={this.runSectionFromChart}
                            currentSection={this.state.currentSection}
                            sopTabIndex={this.props.sopTabIndex}
                            beginSopData={this.beginSopData}
                            closeSOP={this.closeSOP}
                            sopData={this.props.sopDatas[this.props.sopTabIndex].sopData}
                            currentActionStep={this.props.currentActionStep}
                            showConfirmDialog={this.props.showConfirmDialog}
                            onCloseConfirmDialog={this.props.onCloseConfirmDialog}
                            commonSettings={this.props.commonSettings}
                        />
                        
                        <MissionListSB
                            orderSopData={this.state.orderSopData}
                            arrowDatas={this.state.arrowDatas}
                            currentSection={this.state.currentSection}
                            runSection={this.runSection}
                            onProgressSpread={this.onProgressSpread}
                            runSectionFromChart={this.runSectionFromChart}
                            onProgressMission={this.onProgressMission}
                            sopTabIndex={this.props.sopTabIndex}

                            sopDatasNew={this.props.sopDatas}
                            sections={this.state.sections}
                            displaySopTabKey={this.props.displaySopTabKey}
                            currentActionStep={this.props.currentActionStep}
                            teamDatas={this.props.teamDatas}
                            showConfirmDialog={this.props.showConfirmDialog}
                            onCloseConfirmDialog={this.props.onCloseConfirmDialog}
                            commonSettings={this.props.commonSettings}
                        />
                        
                        
                    </div>
                </section>
                {                    
                    this.getPopup()
                }
        	</>
		);
	}
}

export default SopSimulatorBody;