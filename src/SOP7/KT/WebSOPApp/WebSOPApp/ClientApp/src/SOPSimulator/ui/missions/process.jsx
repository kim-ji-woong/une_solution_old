import React, { Component } from 'react';
import styles from '../../../Common/css/style.module.css';
import uis from '../../../Common/css/ui.module.css';
import uneStyles from '../../../Common/css/uneCommon.module.css';
import SectionData from '../../../Common/models/sections/sectionData';
import $ from 'jquery';

class Process extends Component {
    constructor(props) {
        super(props);

        this.state = {
            allChecked: false,
            missions: this.props.sectionData.missions,
            prevProps: props
        }

        this.props = props;

        this.runSection = this.runSection.bind(this);
    }

    static getDerivedStateFromProps(nextProps, prevState) {
        if (nextProps === prevState.prevProps) {
            return prevState;
        }

        let allChecked = true;
        if (nextProps.sectionData.missions !== null && nextProps.sectionData.missions.length > 0) {
            for (let i = 0; i < nextProps.sectionData.missions.length; i++) {
                if (!nextProps.sectionData.missions[i].checked) {
                    allChecked = false;
                    break;
                }
            }
        }

        return {
            missions: nextProps.sectionData.missions,
            allChecked: allChecked,
            prevProps: nextProps
        };
    }

    runSection() {
        this.props.runSection(this.props.sectionData);
    }

    /*    componentDidMount() {
        $('.' + uneStyles.dropBox).hide();
        $('.' + uneStyles.propagatePeople).hover(function () {
            $('.' + uneStyles.dropBox).fadeIn();
        }, function () {
            $('.' + uneStyles.dropBox).fadeOut();
        });
       
    }*/

    onProgressSpread(mission, isSMS) {
        if (this.props.currentSection.componentType === 3 && this.props.currentSection.isBegin) {
            return;
        }

        let strMsg = '문자메시지를 발송할까요?';
        if (!isSMS) {
            strMsg = '메일을 전송할까요?';
        }
        if (window.confirm(strMsg)) {
            const missions = this.props.sectionData.missions;
            const missionsLength = this.props.sectionData.missions.length;

            var missionIndex = -1;
            for (var i = 0; i < missionsLength; i++) {
                if (missions[i] === mission) {
                    // 몇 번째 임무인지 구하기
                    missionIndex = i;
                    break;
                }
            }

            this.props.onProgressSpread(this.props.sectionData, missionIndex, isSMS, !isSMS, false, false);
        }
    }


    onProgressSpreadAll() {
        if (this.props.currentSection.componentType === 3 && this.props.currentSection.isBegin) {
            return;
        }

        if (window.confirm('문자메시지와 메일을 모두 전송할까요?')) {
            this.props.onProgressSpread(this.props.sectionData, -1, true, true, false, false);
        }
    }


    async onProgressMission(checked, mission) {
        if (this.props.currentSection.componentType === 3 && this.props.currentSection.isBegin) {
            return;
        }

        const missions = this.state.missions;
        const missionsLength = this.state.missions.length;

        let allChecked = true;
        let missionIndex = -1;
        for (var i = 0; i < missionsLength; i++) {            
            if (missions[i] === mission) {
                // 몇 번째 임무인지 구하기
                missionIndex = i;
                missions[i].checked = checked;
            }

            if (!missions[i].checked) {
                allChecked = false;
            }
        }

        this.setState({ missions: missions, allChecked: allChecked });

        if (missionIndex >= 0) {
            await this.props.onProgressMission(checked, this.props.sectionData, missionIndex);
        }
    }

    getMissionListUI() {
        var missionList = [];

        if (this.state.missions !== null) {
            for (var i = 0; i < this.state.missions.length; i++) {
                const mission = this.state.missions[i];
                
                missionList.push(
                    <dd className={uneStyles.borderSide} key={i}>
                        <p className={uis.tit}>{mission.missionText}</p>                       
                        <p className={uis.check}>
                            <span className={uis.checkBox}>
                                <input type="checkbox" name="task01"
                                    onChange={(e) => this.onProgressMission(e.target.checked, mission)}
                                    checked={mission.checked} />
                            </span>
                        </p>
                        {
                            //<p className={uis.send}><button type="button" onClick={() => this.onProgressSpread(mission)}><i className={uis.iconSend}></i></button></p>
                            <p><button type="button" onClick={() => this.onProgressSpread(mission, true)}><i className={uneStyles.messagee}></i></button></p>
                        }
                        <p><button type="button" onClick={() => this.onProgressSpread(mission, false)}><i className={uneStyles.emaill}></i></button></p>
                        <p className={uneStyles.completionStatus}>{mission.checked ? '완료' : '미완료'}</p>
                    </dd>
                );
            }
        }


        return [missionList];
    }

    async onCheckedChangeAll(checked) {
        if (this.props.currentSection.componentType === 3 && this.props.currentSection.isBegin) {
            return;
        }

        const missions = this.state.missions;
        const missionsLength = this.state.missions.length;

        
        for (var i = 0; i < missionsLength; i++) {            
            if (missions[i].checked === checked) {
                continue;
            }

            missions[i].checked = checked;

            await this.props.onProgressMission(checked, this.props.sectionData, i);
        }

        this.setState({ missions: missions, allChecked: checked });

    }

    getReceiversUI() {
        if (!this.props.sectionData.receivers || this.props.sectionData.receivers.length === 0)
            return <></>;

        let ui = [];
        const receiverCount = this.props.sectionData.receivers.length;

        for (let i = 0; i < receiverCount; i++) {
            const receiver = this.props.sectionData.receivers[i];
            // 0:평일비상조직, 1:휴일비상조직, 2:정규조직
            const teamType = receiver.teamType;
            const teamID = receiver.teamID;

            let content = this.getReceiverContent(teamType, teamID);

            ui.push(<p key={"teamInfo_" + i}>{content}</p>);
        }
        return ui;
    }

    getReceiverContent(teamType, teamID) {
        let content = '';

        let teamDatas = null;
        if (teamType === 2) {
            teamDatas = this.props.teamDatas.regular;
        }

        if (!teamDatas || teamDatas === null) {
            return '';
        }

        for (var i = 0; i < teamDatas.length; i++) {
            if (teamDatas[i].id === teamID) {
                content = teamDatas[i].teamName;
                break;
            }
        }

        return content;
    }

    render() {
        const [missionList] = this.getMissionListUI();

        //현재임무 - sectionCurrent
        //대기(실행중) - sectionRun
        //완료 - sectionDone
        //대기 - 
        let boxClassName = "";    
        let textClassName = " " + uis.textNormal;
        if (this.props.currentSection.id === this.props.sectionData.id && this.props.currentSection.componentType === this.props.sectionData.componentType) {
            boxClassName = " " + uis.sectionCurrent + " " + uneStyles.currentBox;
            textClassName = " " + uis.textCurrent;
        } else if (this.props.sectionData.status === SectionData.Status_Run) {
            boxClassName = " " + uis.sectionRun;
            textClassName = " " + uis.textRun;
        } else if (this.props.sectionData.status === SectionData.Status_Done) {
            boxClassName = " " + uis.sectionDone;
            textClassName = " " + uis.textDone;
        }

        // 다음 버튼 활성화
        let btnNextClassName = uis.btnAllCheck;
        let btnSpreadAllName = uneStyles.btnArea + " " + uneStyles.btnPropagateSelect;
        if (!this.props.sectionData.sectionNumber)
        {
            // sectionNumber가 null일 때
            btnNextClassName = uis.btnDisable;
            btnSpreadAllName = uneStyles.btnAreaDisable + " " + uneStyles.proBtnDisable;
        }
        else if (this.props.currentSection.componentType === 3 && this.props.currentSection.isBegin) {
            // 현재 SOP가 시작되지 않았을 때 Disable
            btnNextClassName = uis.btnDisable;
            btnSpreadAllName = uneStyles.btnAreaDisable + " " + uneStyles.proBtnDisable;
        }
        else if (this.props.sectionData.status === SectionData.Status_Done) {
            // 현재 임무가 완료된 상태라면 Disable
            btnNextClassName = uis.btnDisable;
            btnSpreadAllName = uneStyles.btnAreaDisable + " " + uneStyles.proBtnDisable;
        }

        const receiversUI = this.getReceiversUI();

        return (               
            <div className={uis.sectionBox + boxClassName} id={this.props.id}>
                <div className={uis.tit + " " + uis.clfix + textClassName}>
                    <strong>
                        {this.props.sectionData.sectionNumber}.{this.props.sectionData.text}
                        {
                            (this.props.sectionData.autoRun) 
                                ? <span className={uis.flag + " " + uis.flag01}>자동</span>
                                : <></>
                        }
                    </strong>
                    <div className={uneStyles.tooltip}>
                        <div className={uneStyles.propagatePeople}><button type="button"></button></div>
                        <div className={uneStyles.dropBox}>
                            <p>전파 대상자</p>
                            {receiversUI}
                        </div>
                    </div>
                    <div className={uis.btnArea + " " + uneStyles.btnArea}>
                        <a className={btnNextClassName} onClick={this.runSection}>다음</a>
                    </div>
                    <div className={uneStyles.completionStatuss}>{(this.state.allChecked) ? '완료' : '미완료'}</div>
                </div>
                <dl className={uis.taskDetail}>
                    <dt>행동요령
                        <p className={uis.checkk + " " + uneStyles.checkk}>
                            {
                                (this.props.missions !== null) ?
                                    <span className={uis.checkBox}>
                                        <input type="checkbox" name=" " checked={this.state.allChecked} onChange={(e) => this.onCheckedChangeAll(e.target.checked)} />
                                    </span>
                                    : null
                            }
                        </p>
                        {/*<p className={uneStyles.btnAreaDisable + " " + uneStyles.proBtnDisable}>전체전파</p> */} {/*--> 비활성화 모드*/}
                        <p className={btnSpreadAllName} onClick={() => this.onProgressSpreadAll()}>전체전파</p>  {/*활성화 모드*/}
                     </dt>
                    {missionList}
                </dl>
            </div>            
        );
    }
}

export default Process;