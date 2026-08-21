import React, { Component } from 'react';
import SectionPanel from '../../Common/sections/sectionPanel';

import sectionStyles from '../../Common/css/section.module.css';
import uneStyles from '../../Common/css/uneCommon.module.css';
import uis from '../../Common/css/ui.module.css';
import apps from '../css/app.module.css';
import $ from 'jquery';

import SopSimulatorResource from "../resource/id";
import SessionString from '../../Common/js/sessionString';

class SopSimulatorSBChart extends Component {

    static resource = SopSimulatorResource;

    static cellDefaultWidth = 300;
    static cellDefaultHeight = 200;
    static gridHeaderMargin = 50;
    static scrollCenterWidth = 180;

    constructor(props) {
        super(props);

        this.state = {
            currentMenu: { menuType: "" },
            editDatas:
            {
                command: "",
                sectionCellDatas: null,
            },
            selectedSectionData: [],
            firstUpdateChk: false,      // SOP Chart 불러오기 유무 판단

        }

        this.props = props;
        this.showConfirmDialog = this.showConfirmDialog.bind(this);
        this.onClickEditSOP = this.onClickEditSOP.bind(this);
    }

    /*워터마크*/
/*    watermarkplay() {
        $('#' + apps.watermark).hide(); 
        $('#' + apps.waterbtn).click(function () {
            if ($('#' + apps.watermark).css('display') == 'none') {
                $('#' + apps.watermark).show();
            } else {
               $('#' + apps.watermark).hide();
            }
        });
    }*/

    componentDidMount() {        
        //this.watermarkplay();

        function btnActive() {
            const target = document.getElementById('target_btn');
            target.disabled = false;
        }

    }


    componentDidUpdate(prevProps, prevState) {
        // SOP Chart 첫 업데이트 시 or 탭을 클릭하여 SOP Chart를 변경하였을때
        if (this.state.firstUpdateChk !== true || prevProps.sopTabIndex !== this.props.sopTabIndex)
            this.selectCurrentComponent();

        // 부모에서 현재 컴포넌트를 변경하였을 경우
        if (prevProps.currentSection !== this.props.currentSection) {
            // 현재 Section 조회
            let currentActionStep = this.props.sopData?.currentActionStep;

            let sectionData = null;
            sectionData = this.props.currentSection;

            // 현재 Section Component 체크하기
            this.setState({ selectedSectionData: [sectionData, currentActionStep] });

            // 실행중인 컴포넌트로 자동 화면 이동
            if (this.props.commonSettings.UseAutoMoveSOPScreen === 'true') {
                this.gridCenterForcus(sectionData?.gridRowIndex, sectionData?.gridColumnIndex);
            }
        }
    }

    selectCurrentComponent = () => {
        // 현재 Section 조회
        let currentActionStep = this.props.sopData?.currentActionStep;
        let sectionData = null;

        if (this.props.currentSection !== null || this.props.currentSection !== undefined) {
            sectionData = this.props.currentSection;
        }

        // 현재 Section Component 체크하기
        this.setState({ selectedSectionData: [sectionData, currentActionStep] });
        this.gridCenterForcus(sectionData?.gridRowIndex, sectionData?.gridColumnIndex);
        
        // 첫 업데이트 체크 >> SOP Chart 불러오기 유무 판단
        this.setState({ firstUpdateChk: true });
    }

    onSelectComponent = (sectionData, actionStep) => {
        // 선택된 sectionData 전달하기
        if (this.props.onSelectComponent(sectionData, null)) {
            this.setState({ selectedSectionData: [sectionData, actionStep] });
            this.gridCenterForcus(sectionData?.gridRowIndex, sectionData?.gridColumnIndex);
        }
    }

    toOriginalLocation = () => {
        this.gridCenterForcus(this.props.currentSection.gridRowIndex, this.props.currentSection.gridColumnIndex);
    }

    gridCenterForcus = (rowIndex, columnIndex) => {
        if (rowIndex === null || rowIndex === undefined) {
            rowIndex = 0;
        }
        if (columnIndex === null || columnIndex === undefined) {
            columnIndex = 0;
        }

        // 그리드 계산
        let width = (SopSimulatorSBChart.cellDefaultWidth * columnIndex) - SopSimulatorSBChart.scrollCenterWidth;
        let height = SopSimulatorSBChart.gridHeaderMargin + (SopSimulatorSBChart.cellDefaultHeight * rowIndex);

        // 스크롤 이동
        //$('.' + sectionStyles.sectionPanel).scrollLeft(width);
        //$('.' + sectionStyles.sectionPanel).scrollTop(height);
        // 애니메이션 효과
        $('.' + sectionStyles.sectionPanel).animate({ scrollTop: height, scrollLeft: width }, 500);
    }

    getSectionData() {
        if (this.state.selectedSectionData && this.state.selectedSectionData.length >= 2) {
            return [this.state.selectedSectionData[0], this.state.selectedSectionData[1]];
        }

        return [null, null];
    }

    onSelectArrow = (arrow, actionStep) => {
        
    }

    getSOPName() {
        let sopName = '';
        if (this.props.sopData?.disaster?.disasterName) {
            sopName = this.props.sopData.disaster.disasterName + ' ';

            if (this.props.sopData.disaster.version) {
                //const mode = this.state.sopData.disaster.version.isNormal ? "(" + SopManagerResource.ID.sopMode.day + ")" : "(" + SopManagerResource.ID.sopMode.night + ")";
                //return sopName + mode;
            }
        }

        if (this.props.sopData?.alarmDateTime && this.props.sopData?.alarmPosition) {
            sopName += this.props.sopData?.alarmDateTime + this.props.sopData?.alarmPosition;
        }

        return sopName;
    }

    // SOP 단계 버튼 클릭시 작동 핸들러 
    onClickStep(e) {
        let target = e;

        // 해당 단계 sop 내용이 없는 버튼이라면 return
        if ($(target).closest('li').hasClass(uis.unActive) !== true)
            return;

        $('.btnActionStep').closest('li.' + uis.isActive).removeClass(uis.isActive).addClass(uis.unActive);
        $(target).closest('li').removeClass(uis.unActive).addClass(uis.isActive);

        /* test */
        $(uis.isActive).show(uis.actCircle);


        // 현재 SOP ID 및 단계 정보 얻어오기
        let stepName = target.innerText;
        let sopID = null;
        let actionStepDatas = this.props.sopData?.actionStepDatas;

        for (let i = 0; i < actionStepDatas.length; i++) {
            let actionStepData = actionStepDatas[i];

            if (stepName === actionStepData.stepName) {
                sopID = actionStepData.actionStep?.id;
            }
        }

        // 해당 SOP ID가 없다면 리턴 
        if (sopID === null)
            return;

        // 첫 업데이트 체크 해제 >> SOP Chart 불러오기 유무 판단
        this.setState({ firstUpdateChk: false });

        // 값 전달하기
        this.props.onChangeActionStep(stepName, sopID);
        return;
    }

    // SOP 단계 버튼영역 생성
    getActionStepArea() {
        let class_1st = "btnActionStep " + uis.class1st;
        let class_2nd = "btnActionStep " + uis.class2nd;
        let class_3rd = "btnActionStep " + uis.class3rd;
        let class_4th = "btnActionStep " + uis.class4th;
        let actionStepArea = "";

        // 현재 단계 및 단계 유무 파악
        const stepDatas = this.props.sopData.actionStepDatas;
        const stepDatasLength = stepDatas.length;

        for (let i = 0; i < stepDatasLength; i++) {
            let stepData = stepDatas[i];

            // 단계별 데이터가 존재한다면
            if (stepData.actionStep != null) {
                if (stepData.stepName === SopSimulatorResource.ID.actionStep._1st) {
                    if (this.props.sopData.currentActionStep.stepName === stepData.stepName) {
                        class_1st += " " + uis.isActive;
                    }
                    else {
                        class_1st += " " + uis.unActive;
                    }

                    if (stepData._ActionStepHistory) {
                        class_1st += " " + uis.actCircle;
                    }                    
                } else if (stepData.stepName === SopSimulatorResource.ID.actionStep._2nd) {
                    if (this.props.sopData.currentActionStep.stepName === stepData.stepName) {
                        class_2nd += " " + uis.isActive;
                    }
                    else {
                        class_2nd += " " + uis.unActive;
                    }

                    if (stepData._ActionStepHistory) {
                        class_2nd += " " + uis.actCircle;
                    }                    
                } else if (stepData.stepName === SopSimulatorResource.ID.actionStep._3rd) {
                    if (this.props.sopData.currentActionStep.stepName === stepData.stepName) {
                        class_3rd += " " + uis.isActive;
                    }
                    else {
                        class_3rd += " " + uis.unActive;
                    }

                    if (stepData._ActionStepHistory) {
                        class_3rd += " " + uis.actCircle;
                    }                    
                } else if (stepData.stepName === SopSimulatorResource.ID.actionStep._4th) {
                    if (this.props.sopData.currentActionStep.stepName === stepData.stepName) {
                        class_4th += " " + uis.isActive;
                    }
                    else {
                        class_4th += " " + uis.unActive;
                    }

                    if (stepData._ActionStepHistory) {
                        class_4th += " " + uis.actCircle;
                    }                    
                }

            }
        }

        // HTML 작성
        actionStepArea = <>
            <li className={class_1st}><button type="button" className={"value"} onClick={(e) => this.onClickStep(e.target)}>{SopSimulatorResource.ID.actionStep._1st}</button></li>
            <li className={class_2nd}><button type="button" className={"value"} onClick={(e) => this.onClickStep(e.target)}>{SopSimulatorResource.ID.actionStep._2nd}</button></li>
            <li className={class_3rd}><button type="button" className={"value"} onClick={(e) => this.onClickStep(e.target)}>{SopSimulatorResource.ID.actionStep._3rd}</button></li>
            <li className={class_4th}><button type="button" className={"value"} onClick={(e) => this.onClickStep(e.target)}>{SopSimulatorResource.ID.actionStep._4th}</button></li>
        </>;

        return actionStepArea;
    }

    showConfirmDialog() {
        this.props.showConfirmDialog('알림',
            '실행중인 SOP는 종료 후 수정 할 수 있습니다. 종료할까요 ?'
            , ['종료 후 열기', '종료하지 않고 열기', '취소'], this.onClickEditSOP);
    }

    onClickEditSOP(index) {        
        if (index === 0) {
            this.props.closeSOP();
        }

        if (index <= 1) {
            window.open("/sop-manager?sop=" + this.props.sopData.version.id, '_blank');
        }

        this.props.onCloseConfirmDialog();
    }

    render() {
        const [sectionData, actionStep] = this.getSectionData();

        let sopName = "";
        sopName = this.getSOPName();

        // SOP 단계 버튼영역 생성
        let btnActionStepArea = this.getActionStepArea();

        return (
          <>
                {/*soulbrainchart*/}
                
                <section className={uis.subSection + " " + uis.progressViewWrap} >
                    <div className={uis.tit + " " + uis.clfix}>
                        <strong>{sopName}</strong>
                        {
                            /*
                            <select id="" value="훈련모드" onChange={() => { }} className={uneStyles.modeSelect}>
                                <option>훈련모드</option>
                                <option>실제모드</option>
                            </select>
                            */
                        }
                        <div className={uis.btnArea}>
                            <button className={uis.btnMod} onClick={this.showConfirmDialog}><i className={uis.iconMod}></i></button>
                            <button className={uis.btnPlay} onClick={this.props.beginSopData}><i className={uis.iconPlay}></i></button>
                            <button className={uis.btnEnd} onClick={this.props.closeSOP}><i className={uis.iconEnd}></i></button>
                        </div>
                    </div>
                    <div className={uis.chartWrap}>
                        <ul className={uis.infoList}>
                            {btnActionStepArea}
                        </ul>

                          <div className={apps.chartArea}>
                            <section className={sectionStyles.panelAreas}>
                                {/*<button id={apps.waterbtn}>훈련모드</button>*/}
                                <button id={uneStyles.refresh} onClick={this.toOriginalLocation}></button>
                                {/* 실행모드일 경우 Cell hover 테두리 제거를 위해서 apps.sectionPanels 클래스 네임 선언 */}
                                    <div className={apps.sectionPanels}>
                                        <SectionPanel
                                            currentMenu={this.state.currentMenu}
                                            selectedSectionData={sectionData}
                                            editDatas={this.state.editDatas}
                                            onProcessEdit={""}
                                            onSelectComponent={this.onSelectComponent}
                                            selectedArrowData={""}
                                            onAddComponent={""}
                                            onRemoveComponent={""}
                                            onSelectArrow={this.onSelectArrow}
                                            sopData={this.props.sopData}
                                            rowCount={30}
                                            columnCount={30}
                                            mode={"exec"}
                                            />
                                    {/*<div id={apps.watermark}>Training mode</div>*/}
                                    {/*<div id={apps.watermark}>훈련모드</div>*/}
                                    </div>
                                </section>
                            </div>

                    </div>
                </section>
                {/*soulbrainchart*/}

       </>
    );
  }
}

export default SopSimulatorSBChart;