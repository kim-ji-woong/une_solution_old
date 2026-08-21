import React, { Component } from 'react';
import styles from '../../../Common/css/style.module.css';
import uis from '../../../Common/css/ui.module.css';
/*import resets from '../../../Common/css/reset.module.css';*/
import uneStyles from '../../../Common/css/uneCommon.module.css';
import $ from 'jquery';
import SectionData from '../../../Common/models/sections/sectionData';
import { SettingController } from '../../../Settings/services/settingController';

class Internal extends Component {
    constructor(props) {
        super(props);

        this.state = {
            allChecked: false,
            prevProps: props
        }

        this.refMessage = React.createRef();
        this.props = props;

        this.runSection = this.runSection.bind(this);
        this.onProgressSpread = this.onProgressSpread.bind(this);
    }

    static getDerivedStateFromProps(nextProps, prevState) {
        if (nextProps === prevState.prevProps) {
            return prevState;
        }

        return {
            allChecked: nextProps.sectionData.checked,
            prevProps: nextProps
        };
    }

    runSection() {
        this.props.runSection(this.props.sectionData);
    }

    componentDidMount() {
        $('html, body').css({ 'display': 'block', 'height': '100%', 'overflow': 'hidden', 'color': '#fff' });

        // Sound Button UI
        $('.btnSoundToggle').on('click', function () {
            $(this).closest('.' + uis.soundInfo).toggleClass(uis.isShow);
        });

        // soundInfoList
        $('.' + uis.soundInfoList).on('click', 'button', function () {
            var val = $(this).data('value');
            if (val == 'Y') {
                $(this).closest('.' + uis.soundInfo).addClass(uis.isOn);
            }
            else if (val == 'N') {
                $(this).closest('.' + uis.soundInfo).removeClass(uis.isOn);
            }
            $(this).closest('.' + uis.soundInfo).removeClass(uis.isShow);
        });


        //Mic
        $('.btnMicToggle').on('click', function () {
            $(this).closest('.' + uneStyles.micInfo).toggleClass(uneStyles.isShow);
        });

        $('.' + uneStyles.micInfoList).on('click', 'button', function () {
            var val = $(this).data('value');
            if (val == 'Y') {
                $(this).closest('.' + uneStyles.micInfo).addClass(uneStyles.isOn);
                $('#' + uneStyles.micInfoOn).css('color', '#39A7DE');
                $('#' + uneStyles.micInfoOff).css('color', '#fff');
            }
            else if (val == 'N') {
                $(this).closest('.' + uneStyles.micInfo).removeClass(uneStyles.isOn);
                $('#' + uneStyles.micInfoOn).css('color', '#fff');
                $('#' + uneStyles.micInfoOff).css('color', '#39A7DE');
            }
            $(this).closest('.' + uneStyles.micInfo).removeClass(uneStyles.isShow);
        });



        //Volume
        $('.btnVolumeToggle').on('click', function () {
            $(this).closest('.' + uneStyles.volumeInfo).toggleClass(uneStyles.isShow);
        });

        $('.' + uneStyles.volumeInfoList).on('click', 'button', function () {
            var val = $(this).data('value');
            if (val == 'Y') {
                $(this).closest('.' + uneStyles.volumeInfo).addClass(uneStyles.isOn);
                $('#' + uneStyles.volumeInfoOn).css('color', '#39A7DE');
                $('#' + uneStyles.volumeInfoOff).css('color', '#fff');
            }
            else if (val == 'N') {
                $(this).closest('.' + uneStyles.volumeInfo).removeClass(uneStyles.isOn);
                $('#' + uneStyles.volumeInfoOn).css('color', '#fff');
                $('#' + uneStyles.volumeInfoOff).css('color', '#39A7DE');
            }
            $(this).closest('.' + uneStyles.volumeInfo).removeClass(uneStyles.isShow);
        });

        this.refMessage.current.value = this.props.sectionData.message;
    }

    async onProgressMission(checked) {
        if (this.props.currentSection.componentType === 3 && this.props.currentSection.isBegin) {
            return;
        }

        this.setState({ allChecked: checked });

        await this.props.onProgressMission(checked, this.props.sectionData, 0);       
    }

    showConfirmDialog(isSMS, isEmail, isBroadcast, isSiren) {
        if (this.props.currentSection.componentType === 3 && this.props.currentSection.isBegin) {            
            return;
        }

        if (!this.refMessage || this.refMessage.current.value.length === 0) {
            return;
        }

        if (!isBroadcast && this.props.currentSection.receivers === null || this.props.currentSection.receivers.length === 0) {
            this.props.showConfirmDialog('실패', '전파할 수신자가 없습니다.', null, null);
        }
        else {
            let strMessage = '';
            if (isSMS) {
                if (this.props.commonSettings.UseSMS === 'false') {
                    strMessage = '문자 전파가 사용되지 않음으로 설정되어 있습니다. 문자 전파를 사용함으로 설정하고 발송할까요?'
                }
                else {
                    strMessage = '문자메시지를 발송할까요?'
                }

                this.confirmDialogData = [isSMS, isEmail, isBroadcast, isSiren];
            }
            else if (isEmail) {
                if (this.props.commonSettings.UseEmail === 'false') {
                    strMessage = '메일 전파가 사용되지 않음으로 설정되어 있습니다. 메일 전파를 사용함으로 설정하고 발송할까요?'
                }
                else {
                    strMessage = '메일을 전송할까요?';
                }

                this.confirmDialogData = [isSMS, isEmail, isBroadcast, isSiren];
            }
            else if (isBroadcast) {
                if (this.props.commonSettings.UseEmail === 'false') {
                    strMessage = '방송 전파가 사용되지 않음으로 설정되어 있습니다. 방송 전파를 사용함으로 설정하고 전파할까요?'
                }
                else {
                    strMessage = '방송을 전파할까요?';
                }

                this.confirmDialogData = [isSMS, isEmail, isBroadcast, isSiren];
            }

            this.props.showConfirmDialog('알림', strMessage, ['상황 전파', '취소'], this.onProgressSpread);
        }
    }

    async SaveSetting(propertyName, propertyValue) {
        const result = await SettingController.requestSaveSetting(propertyName, propertyValue);
        return result;
    }

    async onProgressSpread(index) {
        if (index === 0) {
            if (this.confirmDialogData && this.confirmDialogData.length === 4) {

                const isSMS = this.confirmDialogData[0];
                const isEmail = this.confirmDialogData[1];
                const isBroadcast = this.confirmDialogData[2];
                const isSiren = this.confirmDialogData[3];

                if (isSMS) {
                    let useSMS = this.props.commonSettings.UseSMS === 'true';
                    if (!useSMS) {
                        useSMS = await this.SaveSetting('UseSMS', 'true');
                    }

                    if (!useSMS) {
                        this.props.showConfirmDialog('오류', '문자 전파 설정이 실패했습니다', ['확인'], null);
                        return;
                    }
                }
                else if (isEmail) {
                    let useEmail = this.props.commonSettings.UseEmail === 'true';
                    if (!useEmail) {
                        useEmail = await this.SaveSetting('UseEmail', 'true');
                    }

                    if (!useEmail) {
                        this.props.showConfirmDialog('오류', '메일 전파 설정이 실패했습니다', ['확인'], null);
                        return;
                    }
                }
                else if (isBroadcast) {
                    let useBroadcast = this.props.commonSettings.UseBroadcast === 'true';
                    if (!useBroadcast) {
                        useBroadcast = await this.SaveSetting('UseBroadcast', 'true');
                    }

                    if (!useBroadcast) {
                        this.props.showConfirmDialog('오류', '방송 전파 설정이 실패했습니다', ['확인'], null);
                        return;
                    }
                }

                const dataIndex = 0; // 상황전파는 한 개 임무만 있으므로 첫번째인 0
                this.props.onProgressSpread(this.props.sectionData, dataIndex, isSMS, isEmail, isBroadcast, isSiren, this.refMessage.current.value);

                this.confirmDialogData = undefined;
            }
        }

        this.props.onCloseConfirmDialog();
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
            
            ui.push(<li key={"teamInfo_" + i}>{content}</li>);
        }
        return ui;
    }

    getReceiverContent(teamType, teamID) {
        let content = '';

        let teamDatas = null;
        if (teamType === 2) {
            teamDatas = this.props.teamDatas.regular;
        }

        if (!teamDatas) {
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

    onChangea(e) {
        this.refMessage.current.value = e.target.value;
    }

    render() {
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
        var btnNextClassName = uis.btnAllCheck;
        if (!this.props.sectionData.sectionNumber) {
            // sectionNumber가 null일 때
            btnNextClassName = uis.btnDisable;
        }
        else if (this.props.currentSection.componentType === 3 && this.props.currentSection.isBegin) {
            // 현재 SOP가 시작되지 않았을 때 Disable
            btnNextClassName = uis.btnDisable;
        }
        else if (this.props.sectionData.status === SectionData.Status_Done) {
            // 현재 임무가 완료된 상태라면 Disable
            btnNextClassName = uis.btnDisable;
        }

        let tagUI = [];
        if (this.props.sectionData.autoRun)
            tagUI.push(<span key='auto' className={uis.flag + " " + uis.flag01}>자동</span>);
        if (this.props.sectionData.isBroadcast)
            tagUI.push(<span key='broadcast' className={uis.flag + " " + uis.flag02}>방송</span>);
        if (this.props.sectionData.isSMS)
            tagUI.push(<span key='sms' className={uis.flag + " " + uis.flag03}>문자</span>);
        if (this.props.sectionData.isEmail)
            tagUI.push(<span key='email' className={uis.flag + " " + uis.flag03}>메일</span>);

        let receiverUI = this.getReceiversUI();

        return (
            <div className={uis.sectionBox + boxClassName + " " + uneStyles.sectionBox} id={this.props.id}>
                <div className={uis.tit + " " + uis.clfix + textClassName}>
                    <strong>{this.props.sectionData.sectionNumber}.{this.props.sectionData.text}
                        {tagUI}
                    </strong>
                    <div className={uis.btnArea + " " + uneStyles.btnArea}>
                        <a className={btnNextClassName} onClick={this.runSection}>다음</a>
                    </div>
                    <div className={uneStyles.completionStatuss}>{(this.state.allChecked) ? '완료' : '미완료' }</div>
                </div>
                <div className={uis.sendMessage + " " + uneStyles.sendMessage}>
                    {
                        (this.props.sectionData.isBroadcast)
                            ? <div className={uis.send}>
                                {
                                    /* <div className={uis.soundInfo}>
                                            <button type="button" className={"btnSoundToggle"}><i className={uis.iconSound}></i></button>
                                            <ul className={uis.soundInfoList}>
                                                <li><button type="button" data-value="Y">Sound On</button></li>
                                                <li><button type="button" data-value="N">Sound Off</button></li>
                                            </ul>
                                        </div>*/
                                }
                            </div>
                            : <></>
                    }
                    {/*<div className={uis.message}><button type="button" onClick={() => this.onProgressSpread(true, false, false, false)}><i className={uis.iconMessage}></i></button></div>*/}
                    <div className={uis.check}>
                        <span className={uis.checkBox}>
                            <input type="checkbox" checked={this.state.allChecked} onChange={(e) => this.onProgressMission(e.target.checked)}/>
                        </span>
                    </div>
                    {
                        (this.props.sectionData.isSMS) ?
                            <div><button type="button" onClick={() => this.showConfirmDialog(true, false, false, false)}><i className={uneStyles.message}></i></button></div>
                            : <></>
                    }
                    {
                        (this.props.sectionData.isEmail) ?
                            <div><button type="button" onClick={() => this.showConfirmDialog(false, true, false, false)}><i className={uneStyles.email}></i></button></div>
                            : <></>
                    }
                    {/*<div className={uneStyles.mic}><button type="button"></button></div>*/}
                    {/**********************************************************************/}
                    {
                        (this.props.sectionData.isBroadcast) ?
                            <>
                                <div className={uneStyles.micInfo}>
                                    <button type="button" className={"btnMicToggle"}><i className={uneStyles.mic}></i></button>
                                    <ul className={uneStyles.micInfoList}>
                                        {/**********************************************************/}
                                        <li><button type="button" id={uneStyles.micInfoOn} data-value="Y">Mic On</button></li>
                                        <li><button type="button" id={uneStyles.micInfoOff} data-value="N">Mic Off</button></li>
                                    </ul>
                                </div>
                                <div className={uneStyles.volumeInfo}>
                                    <button type="button" className={"btnVolumeToggle"}><i className={uneStyles.volume}></i></button>
                                    <ul className={uneStyles.volumeInfoList}>
                                        <li><button type="button" id={uneStyles.volumeInfoOn} data-value="Y">Volume On</button></li>
                                        <li><button type="button" id={uneStyles.volumeInfoOff} data-value="N">Volume Off</button></li>
                                    </ul>
                                </div>
                            </>
                            : <></>
                    }
                    {/*<p className={uneStyles.completionStatus}>완료</p>*/}
                </div>
                <dl className={uis.taskDetail + " " + uis.taskSubSection}>
                    <div className={uis.taskSub}>
                        <dt>전파메시지</dt>
                        <textarea ref={this.refMessage} className={uis.taskMessage} onChange={(e) => this.onChangea(e)} />                        
                    </div>
                    <div className={uis.taskSub + " " + uis.taskSubList}>
                        <dt><i className={uneStyles.propagatePeoplee}></i> 전파 대상자</dt>
                        <dd className={uis.scrollbar}>
                            <ul className={uis.clfix}>
                                {receiverUI}
                            </ul>
                        </dd>
                    </div>
                </dl>
            </div>
        );
    }
}

export default Internal;