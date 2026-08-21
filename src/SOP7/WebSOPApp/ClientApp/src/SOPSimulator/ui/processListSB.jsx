import React, { Component } from 'react';
import uis from '../../Common/css/ui.module.css';

import uneStyles from '../../Common/css/uneCommon.module.css';
import $ from 'jquery';
import { array } from '@amcharts/amcharts4/core';
import SopSimulatorController from '../services/sopSimulatorController';

class ProcessListSB extends Component {
    constructor(props) {
        super(props);

        this.state = {
            sopData: null,
            prevProps: props
        }

        this.props = props;
    }

    componentDidMount() {
        // SOP 진행 내역 버튼
        $('.' + uis.pgProgress).on('click', '.' + uis.btnToggle, function () {
            $(this).closest('.' + uis.pgProgress).toggleClass(uis.isHidden);
        });

        // SOP 진행 내역 목록
        $('.' + uis.progressHistoryWrap + ' ' + '.' + uis.numList).on('click', '.' + uis.btnList, function () {
            $(this).closest('.' + uis.list).toggleClass(uis.isShow);
            $(this).siblings('.' + uis.detailInfo).slideToggle();
        });
    }

    getHMS(time) {
        const now = new Date(time);

        //const year = now.getFullYear();
        //const month = now.getMonth();
        //const day = now.getDate();
        const hour = now.getHours();
        const min = now.getMinutes();
        const sec = now.getSeconds();

        const strHour = hour >= 10 ? hour.toString() : "0" + hour;
        const strMin = min >= 10 ? min.toString() : "0" + min;
        const strSec = sec >= 10 ? sec.toString() : "0" + sec;

        return strHour + ':' + strMin + ':' + strSec;
    }

    makeHistories() {
        let historyUI = [];

        let summaries = new Array();

        const actionStepDatasLength = this.props.sopRunData.sopData.actionStepDatas.length;
        for (let i = 0; i < actionStepDatasLength; i++) {
            const actionStepData = this.props.sopRunData.sopData.actionStepDatas[i];
            if (!actionStepData.actionStep) {
                continue;
            }

            if (actionStepData.actionStep.id !== this.props.sopRunData.sopData.currentActionStep.actionStep.id) {
                continue;
            }

            if (!actionStepData.componentHistoryData) {
                continue;
            }

            const histroyLength = actionStepData.componentHistoryData.length;
            for (let j = 0; j < histroyLength; j++) {
                const history = actionStepData.componentHistoryData[j].componentHistory;
                const historyDetail = actionStepData.componentHistoryData[j]._ComponentHistoryDetails;

                if (historyDetail && historyDetail.datai < 2) {
                    continue;
                }

                const sectionLength = actionStepData.stepMemberDatas[0].sections.length;
                for (let k = 0; k < sectionLength; k++) {
                    const section = actionStepData.stepMemberDatas[0].sections[k];
                    if (section.id === history.componentID && section.componentType === history.componentType) {
                        const key = section.componentType + '_' + section.id;
                        const value = [];
                        value.key = key;
                        value.time = this.getHMS(history.time);
                        value.title = section.text;

                        let detail = null;

                        if (history.status === 3) {
                            value.status = '확인';

                            if (section.componentType === 0) {
                                // 프로세스
                                if (section.checked) {
                                    value.status = "완료";
                                }
                                else {
                                    if (section.missions) {
                                        let checked = false;
                                        for (let q = 0; q < section.missions.length; q++) {
                                            if (section.missions[q].checked) {
                                                checked = true;
                                                break;
                                            }
                                        }
                                        if (checked) {
                                            value.status = "부분 완료";
                                        }
                                    }
                                }
                            }
                            else if (section.componentType === 6) {
                                if (section.checked) {
                                    value.status = "완료";
                                }
                            }
                        }
                        else if (history.status === 2) {
                            value.status = '실행중';
                        }
                        else {
                            value.status = '대기';
                        }

                        if (section.componentType === 0 || section.componentType === 6) {
                            if (historyDetail) {
                                if (!detail) {
                                    detail = [];
                                }

                                // 0: 체크해제, 1: 체크, 10: 문자메시지 전파, 20: 메일전파, 30: 방송전파
                                if (historyDetail.dataIndex === -1) {                                    
                                    // 전체 전파                                    
                                    detail.title = '전체 임무 문자메시지, 메일 전파';
                                }
                                else {
                                    detail.title = (historyDetail.dataIndex + 1) + '번 임무 ';
                                    if (historyDetail.datai === 10) {
                                        detail.title += ' 문자메시지 전파';
                                    }
                                    else if (historyDetail.datai === 20) {
                                        detail.title += ' 메일 전파';
                                    }
                                    else if (historyDetail.datai === 30) {
                                        detail.title += ' 방송 전파';
                                    }
                                }
                            }
                        }

                        let match = false;
                        for (let q = 0; q < summaries.length; q++) {
                            if (key === summaries[q].key) {
                                value.details = [];
                                if (summaries[q].details) {
                                    value.details = summaries[q].details;
                                } 

                                if (detail) {
                                    value.details.push(detail);
                                }

                                summaries[q] = value;
                                
                                match = true;
                                break;
                            }
                        }

                        if (!match) {
                            summaries.push(value);
                        }

                        break;
                    }
                }       
            }
        }

        for (let i = 0; i < summaries.length; i++) {
            const summary = summaries[i];
            
            let detailsUI = [];
            let haveDetailClassName = uis.btnList;
            if (summary.details) {
                haveDetailClassName = uis.btnList;
                for (let j = 0; j < summary.details.length; j++) {
                    const detail = summary.details[j];

                    detailsUI.push(
                        <div key={'summaryDetail_' + j} className={uis.detailInfo + " " + uis.clfix}>
                            <span className={uis.message}>{detail.title}</span>
                            {/*<span className={uis.target}>통합방재실</span>*/}
                        </div>
                    );
                }
            }

            historyUI.push(
                <li key={'summary_' + i} className={uis.list}>
                    <a className={haveDetailClassName}>
                        <span className={uis.text}>{summary.title}</span>
                        <span className={uis.statue}>{summary.status}</span>
                        <span className={uis.time}>{summary.time}</span>
                    </a>
                    {detailsUI}
                </li>
            );
        }

        

        return historyUI;
    }

    render() {
        const position = this.props.sopRunData.position;
        const sensorName = this.state.sensorName;
        const type = this.props.sopRunData.sopData.disaster.disasterName;
        const stepName = this.props.sopRunData.sopData.currentActionStep.stepName;
        let beginTime = '';
        for (var i = 0; i < this.props.sopRunData.sopData.actionStepDatas.length; i++) {
            const actionStepData = this.props.sopRunData.sopData.actionStepDatas[i];
            if (actionStepData.stepName === stepName && actionStepData._ActionStepHistory !== null) {
                beginTime = actionStepData._ActionStepHistory.beginTime.replace('T', ' ');
                break;
            }
        }

        const histroyUI = this.makeHistories();

        return (
            <>
              <section className={uis.subSection + " " + uis.progressHistoryWrap}>
                <div className={uis.tit + " " + uis.clfix}>
                    <div>SOP 정보</div>
                    <button className={uis.btnToggle}><i className={uis.iconArrowLeft}></i></button>
                </div>
                <div className={uneStyles.subSection + " " + uneStyles.innerSectionn}>
                {/*<div className={uneStyles.innerSectionn + " " + uis.scrollbar}>*/}
                        <ul>
                            <li>위치 : {position}</li>
                            <li>발생시간 : {beginTime}</li>
                            <li>감지센서 : {this.props.sensorNames}</li>
                            <li>유형 : {type}</li>
                            <li>단계 : {stepName}</li>
                    </ul>
                </div>
              {/*<div className={uis.subSection + " " + uis.progressHistoryWrap}>*/}
                <div className={uis.tit + " " + uis.clfix}>
                    <div className={uis.step + " " + uis.step01}>SOP 진행 내역</div>
                    <button className={uis.btnToggle}><i className={uis.iconArrowLeft}></i></button>
                    </div>
                    <div className={uneStyles.innerSectionnn + " " + uis.scrollbar}>
                    <ol className={uis.numList}>
                        {histroyUI}
                    </ol>
                </div>
              </section>
              {/*</div>*/}
           </>
        );
    }
}

export default ProcessListSB;