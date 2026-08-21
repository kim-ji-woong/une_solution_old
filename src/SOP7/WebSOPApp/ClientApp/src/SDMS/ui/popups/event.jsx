import { ui } from 'jquery';
import React, { Component } from 'react';
import content from '../../../Common/css/content.module.css';
import uis from '../../../Common/css/ui.module.css';
import { SDMSController } from '../../services/sdmsController';
import SDMS from '../sdms';
import Contents3D from '../3D/contents3D';
import SessionString from '../../../Common/js/sessionString';
import StringUtil from '../../../Common/util/StringUtil';
import { Scrollbars } from 'react-custom-scrollbars-2';
import { SdmsScrollbar } from './SdmsScrollbar';
import $ from 'jquery';
import ProjectResource from '../../../Root/resource/id';
import SDMSResource from '../../resource/id';
import AccountResource from '../../../Account/resource/id';
import ConfirmDialog from '../../../Common/ui/confirmDialog';

import SettingsStore from '../../../Settings/settingsStore';

import styles from '../../css/sdms.module.css';
import imgCloseBroadcast from "../../img/broadcast/closeBroadcast.png";
import imgOnBroadcast from "../../img/broadcast/onBroadcast.png";
import { SettingController } from '../../../Settings/services/settingController';
import { fab } from '@fortawesome/free-brands-svg-icons';

class Event extends Component {
    static keys = [];

    constructor(props) {
        super(props);

        this.state = {
            popupMinWidth: 360, // 팝업 최소 너비
            popupMinHeight: 300, // 팝업 최소  높이
            loginUser: null,
            commonSettings: SettingsStore.getState().sdmsCommonSettings,
            broadcastInfo: {},
        }

        this.onMalfunction = this.onMalfunction.bind(this);
        this.onSituationNotice = this.onSituationNotice.bind(this);
        this.onSelectedAlarm = this.onSelectedAlarm.bind(this);
        this.onSound = this.onSound.bind(this);
        this.initPopupState = this.initPopupState.bind(this)

        this.refAlarmList = React.createRef();
        this.refTitle = React.createRef();
        this.refHeader = React.createRef();
        this.refScrollbar = React.createRef();
        this.refTable = React.createRef();

        SettingsStore.subscribe(function () {
            let data = SettingsStore.getState();

            if (data.actionType === 'RESET_POPUP') {
                this.repositionPopup(data.popupState);
            } else if (data.actionType === 'SDMS_COMMON_SETTINGS') {
                this.changeSDMSCommonSettings(data.sdmsCommonSettings);
            }

        }.bind(this));

        this.initSiteID();
    }

    componentDidMount() {
        this.initUser();

        // 팝업 마우스 드래그 이벤트 리스너
        this.popupDragMouseMove = (event) => {
            let mousePosition = {
                x: event.clientX,
                y: event.clientY
            }

            //움직여야할 좌표
            let moveX = mousePosition.x + this.state.dragOffsetX;
            let perMoveX = ((moveX / this.state.maxScreenWidth) * 100);

            let moveY = mousePosition.y + this.state.dragOffsetY;
            let perMoveY = ((moveY / this.state.maxScreenHeight) * 100);

            // 팝업 너비
            let width = this.state.popup.clientWidth;
            let left = this.state.popup.offsetLeft;

            // 팝업 높이
            let height = this.state.popup.clientHeight;
            let top = this.state.popup.offsetTop;

            let popupRightPos = width + left;   // 현재 위치에서 오른쪽 끝 절대 좌표
            let popupBottomPos = height + top;  // 현재 위치에서 아래쪽 끝 절대 좌표

            // 팝업이 화면밖으로 안나가도록 처리
            if (moveX > 0 && moveX + width < this.state.maxScreenWidth) {
                this.state.popup.style.left = perMoveX + '%';
            } else if (moveX + width > this.state.maxScreenWidth) {
                // 드래그 도중 이동할 마우스 포지션 지점부터 팝업 끝지점이 우측 화면 밖을 벗어나게 될 때
                if (popupRightPos < this.state.maxScreenWidth) {
                    // 팝업을 우측 변에 고정
                    let lim = ((this.state.maxScreenWidth - width) / this.state.maxScreenWidth) * 100;
                    this.state.popup.style.left = lim + '%';
                } else if (this.state.preMousePosition.x > mousePosition.x) {
                    // 화면 오른쪽으로 팝업이 이미 벗어나 있을 때
                    this.state.popup.style.left = perMoveX + '%';
                }
            } else if (moveX <= 0) {
                // 드래그 도중 팝업 시작점이 좌측 화면 밖을 벗어나게 될 때
                if (left > 0) {
                    this.state.popup.style.left = '0%';
                } else if (this.state.preMousePosition.x < mousePosition.x) {
                    // 화면 왼쪽으로 팝업이 이미 벗어나 있을 때
                    this.state.popup.style.left = perMoveX + '%';
                }
            }

            if (moveY > 60 && moveY + height < this.state.maxScreenHeight) {
                this.state.popup.style.top = perMoveY + '%';
            } else if (moveY + height > this.state.maxScreenHeight) {
                // 드래그 도중 이동할 마우스 포지션 지점부터 팝업 하단 끝지점이 화면 밖을 벗어나게 될 때
                if (popupBottomPos < this.state.maxScreenHeight) {
                    // 팝업을 아랫 변에 고정
                    let lim = ((this.state.maxScreenHeight - height) / this.state.maxScreenHeight) * 100;
                    this.state.popup.style.top = lim + '%';
                } else if (this.state.preMousePosition.y > mousePosition.y) {
                    // 화면 아래쪽으로 팝업이 이미 벗어나 있을 때
                    this.state.popup.style.top = perMoveY + '%';
                }
            } else if (moveY <= 60) {
                // 드래그 도중 상단 끝지점이 화면 밖을 벗어나게 될 때
                if (top > 60) {
                    // 팝업을 윗 변에 고정
                    //상단 툴바는 항상 높이 60 고정이기 때문에 현재 화면 사이즈에서 60px의 비율을 계산한다.
                    let lim = (60 / this.state.maxScreenHeight) * 100;
                    this.state.popup.style.top = lim + '%';
                } else if (this.state.preMousePosition.y < mousePosition.y) {
                    //화면 위쪽으로 팝업이 이미 벗어나 있을 때
                    this.state.popup.style.top = perMoveY + '%';
                }
            }
        }
        //팝업 리사이즈 이벤트 리스너
        this.popupResizeMouseMove = (event) => {
            let sizeX = 0;
            let sizeY = 0;

            switch (this.state.resizeType) {
                // 수평
                case 'h-r': // 오른쪽 수평
                    sizeX = event.pageX - this.state.popup.getBoundingClientRect().left;

                    if (event.pageX > 0 && event.pageX < this.state.maxScreenWidth && sizeX > this.state.popupMinWidth) {
                        this.state.popup.style.width = sizeX + 'px';
                    }
                    break;
                case 'h-l': //왼쪽 수평
                    sizeX = this.state.originalWidth - (event.pageX - this.state.originalMouseX);

                    if (event.pageX > 0 && event.pageX < this.state.maxScreenWidth && sizeX > this.state.popupMinWidth) {
                        this.state.popup.style.width = sizeX + 'px';

                        let pxLeft = (this.state.originalX + (event.pageX - this.state.originalMouseX));
                        this.state.popup.style.left = ((pxLeft / this.state.maxScreenWidth) * 100) + '%';
                    }
                    break;
                // 수직
                case 'v-b': // 바텀 수직
                    sizeY = event.pageY - this.state.popup.getBoundingClientRect().top;

                    if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && sizeY > this.state.popupMinHeight) {
                        this.state.popup.style.height = sizeY + 'px';
                    }
                    break;
                case 'v-t': //탑 수직
                    sizeY = this.state.originalHeight - (event.pageY - this.state.originalMouseY);

                    if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && sizeY > this.state.popupMinHeight) {
                        this.state.popup.style.height = sizeY + 'px';

                        let pxTop = this.state.originalY + (event.pageY - this.state.originalMouseY);
                        this.state.popup.style.top = ((pxTop / this.state.maxScreenHeight) * 100) + '%';
                    }
                    break;
                // 대각
                case 'd-rb': // 오른쪽 하단 대각
                    sizeX = event.pageX - this.state.popup.getBoundingClientRect().left;
                    sizeY = event.pageY - this.state.popup.getBoundingClientRect().top;

                    if (event.pageX > 0 && event.pageX < this.state.maxScreenWidth && sizeX > this.state.popupMinWidth) {
                        this.state.popup.style.width = sizeX + 'px';
                    }

                    if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && sizeY > this.state.popupMinHeight) {
                        this.state.popup.style.height = sizeY + 'px';
                    }
                    break;
                case 'd-rt': //오른쪽 상단 대각
                    sizeX = this.state.originalWidth + (event.pageX - this.state.originalMouseX);
                    sizeY = this.state.originalHeight - (event.pageY - this.state.originalMouseY);

                    if (event.pageX > 0 && event.pageX < this.state.maxScreenWidth && sizeX > this.state.popupMinWidth) {
                        this.state.popup.style.width = sizeX + 'px'
                    }

                    if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && sizeY > this.state.popupMinHeight) {
                        this.state.popup.style.height = sizeY + 'px'

                        let pxTop = this.state.originalY + (event.pageY - this.state.originalMouseY);
                        this.state.popup.style.top = ((pxTop / this.state.maxScreenHeight) * 100) + '%';
                    }
                    break;

                case 'd-lb': //왼쪽 하단 대각
                    sizeY = event.pageY - this.state.popup.getBoundingClientRect().top;
                    sizeX = this.state.originalWidth - (event.pageX - this.state.originalMouseX);

                    if (event.pageX > 0 && event.pageX < this.state.maxScreenWidth && sizeX > this.state.popupMinWidth) {
                        this.state.popup.style.width = sizeX + 'px';

                        let pxLeft = (this.state.originalX + (event.pageX - this.state.originalMouseX));
                        this.state.popup.style.left = ((pxLeft / this.state.maxScreenWidth) * 100) + '%';
                    }

                    if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && sizeY > this.state.popupMinHeight) {
                        this.state.popup.style.height = sizeY + 'px';
                    }
                    break;

                case 'd-lt': //왼쪽 상단 대각
                    sizeX = this.state.originalWidth - (event.pageX - this.state.originalMouseX);
                    sizeY = this.state.originalHeight - (event.pageY - this.state.originalMouseY);

                    if (event.pageX > 0 && event.pageX < this.state.maxScreenWidth && sizeX > this.state.popupMinWidth) {
                        this.state.popup.style.width = sizeX + 'px';

                        let pxLeft = (this.state.originalX + (event.pageX - this.state.originalMouseX));
                        this.state.popup.style.left = ((pxLeft / this.state.maxScreenWidth) * 100) + '%';
                    }
                    if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && sizeY > this.state.popupMinHeight) {
                        this.state.popup.style.height = sizeY + 'px'

                        let pxTop = this.state.originalY + (event.pageY - this.state.originalMouseY);
                        this.state.popup.style.top = ((pxTop / this.state.maxScreenHeight) * 100) + '%';
                    }
                    break;
                default:
            }

        }

        this.initPopupState();
        this.setScrollbar();

        // 단축키 이벤트 리스너
        //document.addEventListener("keydown", (e) => this.keysPressed(e, this), false);
        document.addEventListener("keydown", this.keyFunction, false);
        document.addEventListener("keyup", this.keysReleased, false);

        this.props.setActiveDragPopup(this.props.popupType);
    }

    componentWillUnmount() {
        // 단축키 이벤트 리스너 제거
        document.removeEventListener("keydown", this.keyFunction);
        document.removeEventListener("keyup", this.keysReleased);
    }

    keyFunction = (e) => this.keysPressed(e, this);

    keysPressed(e, target) {
        const commonKey = 16;   // Shift 키

        // store an entry for every key pressed
        Event.keys[e.keyCode] = true;

        if (Event.keys[commonKey] && Event.keys[81]) {
            // 상황전파 퀵버튼
            target.onSituationNotice();

            Event.keys[81] = false;
            // prevent default browser behavior
            e.preventDefault();
        } else if (Event.keys[commonKey] && Event.keys[87]) {
            // 화면전환 퀵버튼
            target.onMoveSelectedAlarm();

            Event.keys[87] = false;
            // prevent default browser behavior
            e.preventDefault();
        } else if (Event.keys[commonKey] && Event.keys[69]) {
            // 소리끄기 퀵버튼
            target.onSound();

            Event.keys[69] = false;
            // prevent default browser behavior
            e.preventDefault();
        } else if (Event.keys[commonKey] && Event.keys[82]) {
            // 종료 퀵버튼
            target.onMalfunction();

            Event.keys[82] = false;
            // prevent default browser behavior
            e.preventDefault();
        }
    }

    keysReleased(e) {
        // mark keys that were released
        Event.keys[e.keyCode] = false;
    }

    repositionPopup(popupState) {
        let data = popupState.event;

        if (data === null || data === undefined)
            return;

        let popup = document.getElementsByClassName(content.viewDashboardBoxD + " " + content.viewDashboardEvent)[0];
        if (popup === null || popup === undefined)
            return;

        popup.style.left = data.x;
        popup.style.top = data.y;
        popup.style.width = data.width;
        popup.style.height = data.height;

        this.setState({ popup: popup });
    }

    async initUser() {
        let user = await ProjectResource.initUserInfo();

        this.setState({ loginUser: user });
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

    setScrollbar() {
        const rectAlarmList = this.refAlarmList.current.getBoundingClientRect();
        const rectTitle = this.refTitle.current.getBoundingClientRect();
        const rectHeader = this.refHeader.current.getBoundingClientRect();

        const width = rectHeader.width;
        const height = rectAlarmList.height - rectTitle.height - rectHeader.height - 5;

        let scrollVisible = false;

        if (this.refTable.current) {
            const rectTable = this.refTable.current.getBoundingClientRect();

            if (rectTable.height > height) {
                scrollVisible = true;
            }
        }

        SdmsScrollbar.setContentStyle(this.refScrollbar.current, width, height, scrollVisible);
    }

    componentDidUpdate(prevProps, prevState) {
        // 팝업이 선택 됐을 때(Drag 될때) 맨 앞에 팝업 위치
        if (this.props.zIndex !== prevProps.zIndex) {
            this.state.popup.style.zIndex = this.props.zIndex
        }

        this.setScrollbar();
    }

    onSelectedAlarm(alarm) {
        this.props.onSelectedAlarm(alarm);
    }
    onMoveSelectedAlarm() {
        this.props.onMoveSelectedAlarm();
    }

    onSound() {
        if (this.props.selectedAlarm.sound === undefined) {
            this.props.selectedAlarm.sound = true;
        }

        this.props.selectedAlarm.sound = !this.props.selectedAlarm.sound;
        this.props.onSound(this.props.selectedAlarm.sound);
    }

    setGridUI() {
        if (this.props.sensorAlarms === null || this.props.sensorAlarms.length === 0)
            return null;

        var grid = [];


        const alarms = this.props.sensorAlarms;
        for (var i = 0; i < alarms.length; i++) {
            const alarm = alarms[i];
            const dt = new Date(alarm.dtTime);
            var mm = dt.getMonth() + 1;
            var dd = dt.getDate();
            var ss = dt.getSeconds();
            const ymd = dt.getFullYear() + '.' + StringUtil.getDoubleString(mm) + '.' + StringUtil.getDoubleString(dd);
            const hms = StringUtil.getDoubleString(dt.getHours()) + ':' + StringUtil.getDoubleString(dt.getMinutes()) + ':' + StringUtil.getDoubleString(ss);

            var sopStatusClassName = content.redTxt;
            var sopStatusText = '미대응';
            let statusClassName = content.red;

            if (alarm.sopStatus === 0 || alarm.sopStatus === 1) {
                sopStatusClassName = content.greenTxt;
                sopStatusText = '대응중';
                statusClassName = content.grn;
            }
            else if (alarm.sopStatus === 2) {
                sopStatusClassName = content.whiteTxt;
                sopStatusText = '상황종료';
                statusClassName = '';
            }

            if (alarm.isAlarm === false) {
                sopStatusClassName = content.whiteTxt;
                sopStatusText = '알람종료';
                statusClassName = '';
            }

            var alarmDepth = '주의'
            if (alarm.alarmDepth === 1) {
                alarmDepth = '관심';
            }
            else if (alarm.alarmDepth === 3) {
                alarmDepth = '경계';
            }
            else if (alarm.alarmDepth === 4) {
                alarmDepth = '심각';
            }

            let rowClassName = '';
            if (alarm === this.props.selectedAlarm && !alarm.isAlarm) { // 알람종료된 row가 선택됐을 때
                rowClassName = content.selectedClosedAlarm;
            }
            else if (alarm === this.props.selectedAlarm) { // 선택된 row
                rowClassName = content.selectedAlarm;
            }
            else if (!alarm.isAlarm) { // 알람 종료된 row
                rowClassName = content.closedAlarm;
            }

            // MaterialType이 있을 경우
            let typeString = alarm.facilityTypeString;
            if (alarm.materialTypeString !== null && alarm.materialTypeString !== undefined && alarm.materialTypeString !== "")
                typeString = alarm.materialTypeString;

            // 이벤트 숫자
            // .TODO: 고도화 내용으로 인한 주석처리 
            //const displayNumUI = this.displayNumUI(alarm);
            const displayNumUI = [];
            const positionName = (!alarm.buildingName || alarm.buildingName.length === 0) ? alarm.zoneName : alarm.buildingName;

            grid.push(
                <React.Fragment key={"eventGrid_" + i}>
                    <tr className={rowClassName} onClick={() => this.onSelectedAlarm(alarm)} onDoubleClick={() => this.onMoveSelectedAlarm()}>
                        <td>{ymd}<br />{hms}{displayNumUI}</td> 
                        <td>{typeString}</td>
                        <td>{alarmDepth}</td>
                        <td>{positionName}</td>
                        <td><span className={statusClassName}>{sopStatusText}</span></td>
                    </tr>
                </React.Fragment>
            );
        }

        return grid;
    }

    displayNumUI = (alarm) => {
        let alarmInfo = this.props.alarmInfo;
        let displayNumUI = [];
        let num = null;

        for (let key in alarmInfo) {
            const data = alarmInfo[key];

            if (data[1].sensorZoneHistoryID === alarm.sensorZoneHistoryID) {
                if (key.indexOf(SDMSResource.ID.menu.alarmCCTV + "_") !== -1) {
                    num = key.replace(SDMSResource.ID.menu.alarmCCTV + "_", "");
                }
            }
        }

        if (num !== null && num !== undefined) {
            displayNumUI.push(
                <React.Fragment key={"eventSpan_" + num}>
                    <span className={content.eventAct}>{num}</span>
                </React.Fragment>
            );
        }

        return displayNumUI;
    }

    async onMalfunction() {
        const alarm = this.props.selectedAlarm;
        const userAuthor = ProjectResource.getUserAuthor();

        if (alarm.isAlarm) {
            // 권한에 따른 상황 종료 여부
            if (userAuthor === AccountResource.ID.accountLevel.admin)
                this.props.onMalfunction(alarm);
            else
                this.props.onAuthorError();
        } 
    }
    
    async onSituationNotice() {
        const alarm = this.props.selectedAlarm;
        if (alarm.sopStatus !== -1) {
            alert('SOP가 진행중이거나 이미 종료되었습니다.');
            return;
        }

        if (!window.confirm('SOP를 실행할까요?'))
            return;

        // sop 새탭으로 띄우기
        window.open(ProjectResource.path.sopSimulator);
                
        await SDMSController.requestSituationNotice(alarm.facilityType, alarm.sensorZoneID);
    }

    initPopupState() {
        var popup = document.getElementsByClassName(content.viewDashboardBoxD + " " + content.viewDashboardEvent)[0];

        //DB에 값이 있을 경우에만
        if (typeof this.props.popupState !== 'undefined') {
            popup.style.left = this.props.popupState.x;
            popup.style.top = this.props.popupState.y;
            popup.style.width = this.props.popupState.width;
            popup.style.height = this.props.popupState.height;
        }

        this.setState({ popup: popup });
    }

    // 팝업 드래그 시작(팝업을 누르고 있을 때)
    popupDragMousePress(event) {
        if (event.button == 0) {
            //마우스 조작중에 브라우저의 크기를 조절할 수 없으므로
            // 이 시점에 도큐먼트 전체 크기를 호출한다.
            this.setState({
                maxScreenHeight: document.getElementsByTagName('body')[0].clientHeight,
                maxScreenWidth: document.getElementsByTagName('body')[0].clientWidth,
                dragOffsetX: this.state.popup.offsetLeft - event.clientX,
                dragOffsetY: this.state.popup.offsetTop - event.clientY,
                preMousePosition: {
                    x: event.clientX,
                    y: event.clientY
                }
            });

            document.addEventListener('mousemove', this.popupDragMouseMove);
            document.addEventListener('mouseup', this.popupDragMouseUp);

            // z-index 조정, 1 이 다른 팝업보다 앞에 배치됨
            this.props.setActiveDragPopup(this.props.popupType);
        }
    }
    // 팝업 드래그 종료(mouse up)
    popupDragMouseUp = () => {
        console.log('popup drag false')
        document.removeEventListener('mousemove', this.popupDragMouseMove);
        document.removeEventListener('mouseup', this.popupDragMouseUp);
        // 팝업 정보 DB 작성
        this.setPopupState();
    }

    // 팝업 리사이징(누르고 있을 때)
    popupResizeMousePress(event, resizeType) {
        /* resizeType
         * h-r      오른쪽 수평
         * h-l      왼쪽 수평
         * v-b      바텀 수직
         * v-t      탑 수직
         * d-rt     우측 상단 대각
         * d-rb     우측 하단 대각
         * d-lt     좌축 상단 대각
         * d-lb     좌측 하단 대각
        */

        console.log('popupResizeMousePress');
        this.setState({
            maxScreenHeight: document.getElementsByTagName('body')[0].clientHeight,
            maxScreenWidth: document.getElementsByTagName('body')[0].clientWidth,
            resizeType: resizeType,
            originalMouseX: event.pageX,
            originalMouseY: event.pageY,
            originalWidth: parseFloat(getComputedStyle(this.state.popup, null).getPropertyValue('width').replace('px', '')),
            originalHeight: parseFloat(getComputedStyle(this.state.popup, null).getPropertyValue('height').replace('px', '')),
            originalX: this.state.popup.getBoundingClientRect().left,
            originalY: this.state.popup.getBoundingClientRect().top
        });

        document.addEventListener('mousemove', this.popupResizeMouseMove);

        document.addEventListener('mouseup', this.popupResizeMouseUp);
        // z-index 조정, 1 이 다른 팝업보다 앞에 배치됨
        this.props.setActiveDragPopup(this.props.popupType);
    }

    popupResizeMouseUp = () => {
        console.log('popup resize false');
        document.removeEventListener('mousemove', this.popupResizeMouseMove);
        document.removeEventListener('mouseup', this.popupResizeMouseUp);
        this.setState({ resizeType: null });
        this.setPopupState();
    }

    setPopupState() {
        // 팝업 정보 DB 작성
        let perX = ((this.state.popup.offsetLeft / this.state.maxScreenWidth) * 100);
        let perY = ((this.state.popup.offsetTop) / this.state.maxScreenHeight * 100);
        let width = this.state.popup.offsetWidth;
        let height = this.state.popup.offsetHeight;

        if (perX > 0 && perY > 0 && width > 0 && height > 0) {
            let popupState = {
                id: typeof this.props.popupState !== 'undefined' ? this.props.popupState.id : -1,
                x: perX + '%',
                y: perY + '%',
                height: height + 'px',
                width: width + 'px'
            }
            this.props.setPopupState(this.props.popupType, popupState);
        }
    }

    changeSDMSCommonSettings(storeValue) {
        const commonSettings = storeValue ? storeValue : {};

        this.setState({ commonSettings: commonSettings });
    }

    broadcastIsRunning() {
        const run = this.state.commonSettings?.RunAlarmBroadcast;

        if (run) {
            const param = parseInt(run);

            if (param && param > 0) {
                return true;
            }
        }

        return false;
    }
     
    broadcastState() {
        let state = SDMSResource.BroadcastState.None;
        let buildingID = null;

        const runAlarmBroadcast = this.state.commonSettings?.RunAlarmBroadcast;
        const closeAlarmBroadcast = this.state.commonSettings?.CloseAlarmBroadcast;
        const selectedAlarm = this.props.selectedAlarm;

        const buildingGroupList = this.props.buildingGroupList;
        if (buildingGroupList !== null && buildingGroupList !== undefined) {
            const buildingDatas = buildingGroupList[0].buildingDatas;
            let alarmBuildingID = null;

            for (var building of buildingDatas) {
                const zoneDatas = building.zoneDatas;

                for (var zone of zoneDatas) {
                    if (zone.id === selectedAlarm.zoneID) {
                        alarmBuildingID = building.id;
                        break;
                    }
                }

                if (alarmBuildingID !== null)
                    break;
            }

            if (runAlarmBroadcast !== null && runAlarmBroadcast !== undefined &&
                closeAlarmBroadcast !== null && closeAlarmBroadcast !== undefined &&
                alarmBuildingID !== null) {
                var arrRunAlarmBroadcast = runAlarmBroadcast.split(",");
                var arrCloseAlarmBroadcast = closeAlarmBroadcast.split(",");
                buildingID = alarmBuildingID;

                if (arrRunAlarmBroadcast.indexOf(alarmBuildingID.toString()) !== -1) {
                    state = SDMSResource.BroadcastState.Run;
                }

                if (arrCloseAlarmBroadcast.indexOf(alarmBuildingID.toString()) !== -1) {
                    state = SDMSResource.BroadcastState.Stop;
                }  
            }
        }

        this.state.broadcastInfo.state = state;
        this.state.broadcastInfo.buildingID = buildingID;
        return state;
    }

    onClickCloseBroadcast = (e) => {
        this.props.showConfirmDialog(SDMSResource.ID.common.confirm,
            SDMSResource.ID.broadcast.closeInfo,
            [SDMSResource.ID.broadcast.closeBroadcast, SDMSResource.ID.common.cancel],
            this.onClickCloseBroadcastOption
        );
    }

    onClickCloseBroadcastOption = (index) => {
        if (index === 0) {
            this.closeBroadcast();
        }
        else {
            console.log("취소");
        }

        this.props.closeConfirmDialog();
    }

    async closeBroadcast() {
        const settings = [
            {
                "name": "CloseAlarmBroadcast",
                "value": "1"
            }
        ];

        const [success, message] = await SettingController.requestUpdateSdmsSettings(settings);

        if (!success && message !== null) {
            alert(message);
        }
    }

    onClickOnOffBroadcast = (onOff) => {
        if (onOff === SDMSResource.BroadcastState.Stop) {
            // 방송 시작
            this.props.showConfirmDialog(SDMSResource.ID.common.confirm,
                SDMSResource.ID.broadcast.onInfo,
                [SDMSResource.ID.broadcast.onBroadcast, SDMSResource.ID.common.cancel],
                this.onClickOnOffBroadcastOption
            );
        } else if (onOff === SDMSResource.BroadcastState.Run) {
            // 방송 중지
            this.props.showConfirmDialog(SDMSResource.ID.common.confirm,
                SDMSResource.ID.broadcast.closeInfo,
                [SDMSResource.ID.broadcast.closeBroadcast, SDMSResource.ID.common.cancel],
                this.onClickOnOffBroadcastOption
            );
        }
    }

    onClickOnOffBroadcastOption = (index) => {
        if (index === 0) {
            this.onOffBroadcast();
        }
        else {
            console.log("취소");
        }

        this.props.closeConfirmDialog();
    }

    async onOffBroadcast() {
        const state = this.state.broadcastInfo.state;
        const buildingID = this.state.broadcastInfo.buildingID;
        let onOff = null;

        if (state === SDMSResource.BroadcastState.Stop) {
            onOff = "true";
        } else if (state === SDMSResource.BroadcastState.Run) {
            onOff = "false";
        }

        const [success, message] = await SettingController.requestOnOffBroadcast(onOff, buildingID.toString());
        
        if (!success && message !== null) {
            alert(message);
        }
    }

    displayBroadcastUI = () => {
        let displayBroadcastUI = [];

        let siteID = ProjectResource.SiteID;

        if (siteID === ProjectResource.Site.Soulbrain) {
            if (this.broadcastIsRunning() === true) {
                displayBroadcastUI.push(<img className={styles.imgBroadcast} src={imgCloseBroadcast} title={SDMSResource.ID.broadcast.close} onClick={this.onClickCloseBroadcast} />);
            }
        } else if (siteID === ProjectResource.Site.GCC) {
            const state = this.broadcastState();

            if (state === SDMSResource.BroadcastState.Run) {
                // 방송 중 경우
                displayBroadcastUI.push(<img className={styles.imgBroadcast} src={imgCloseBroadcast} title={SDMSResource.ID.broadcast.on} onClick={() => this.onClickOnOffBroadcast(state)} />);
            } else if (state === SDMSResource.BroadcastState.Stop) {
                // 방송 중지 경우
                displayBroadcastUI.push(<img className={styles.imgBroadcast} src={imgOnBroadcast} title={SDMSResource.ID.broadcast.close} onClick={() => this.onClickOnOffBroadcast(state)} />);
            }
        }

        return displayBroadcastUI;
    }

    async initSiteID() {
        let siteID = ProjectResource.SiteID;

        if (siteID === null || siteID === undefined) {
            // 사이트 ID 요청
            siteID = await ProjectResource.loadSiteID();

            this.setState({ reload: true });
        }
    }

    getAlarmInfo() {
        const selectedAlarm = this.props.selectedAlarm;

        let evtClassName = content.evtFIRE;

        if (selectedAlarm.facilityType) {
            if (selectedAlarm.facilityType === SDMSResource.facilityType.FIRE) {
                evtClassName = content.evtFIRE;
            } else if (SDMSResource.isPSMSensorType(selectedAlarm.facilityType)) {
                evtClassName = content.evtPSM;
            } else if (SDMSResource.isETCSensorType(selectedAlarm.facilityType)) {
                evtClassName = content.evtETC;
            } else if (SDMSResource.isSVMSSensorType(selectedAlarm.facilityType)) {
                evtClassName = content.evtSVMS;
            }
        }

        return (
            <div className={content.dseInfo}>
                <em className={evtClassName}>이벤트 아이콘</em>
                <p>
                    <span>{this.props.selectedAlarm.positionName}</span>
                    {
                        (this.props.selectedAlarm.reportPerson.length === 0 && this.props.selectedAlarm.memo.length === 0)
                            ? <></>
                            : <>
                                <span>{(this.props.selectedAlarm.reportPerson.length === 0) ? ' - ' : this.props.selectedAlarm.reportPerson}</span>
                                <span>{(this.props.selectedAlarm.memo.length === 0) ? ' - ' : this.props.selectedAlarm.memo}</span>
                            </>
                    }
                </p>
                <p>{this.props.selectedAlarm.strDateTime}</p>
                <p>{this.props.selectedAlarm.message}</p>
            </div>
            );
    }

    render() {
        const gridUI = this.setGridUI();

        return (
            <div id={this.props.popupType} className={content.viewDashboardBoxD + " " + content.viewDashboardEvent}>
                <div className={content.popupSizingAreaRight} onMouseDown={(e) => this.popupResizeMousePress(e, 'h-r')} ></div>
                <div className={content.popupSizingAreaLeft} onMouseDown={(e) => this.popupResizeMousePress(e, 'h-l')}></div>
                <div className={content.popupSizingAreaBottom} onMouseDown={(e) => this.popupResizeMousePress(e, 'v-b')}></div>
                <div className={content.popupSizingAreaTop} onMouseDown={(e) => this.popupResizeMousePress(e, 'v-t')}></div>
                <div className={content.popupSizingAreaRightTopPoint} onMouseDown={(e) => this.popupResizeMousePress(e, 'd-rt')}></div>
                <div className={content.popupSizingAreaRightBottomPoint} onMouseDown={(e) => this.popupResizeMousePress(e, 'd-rb')}></div>
                <div className={content.popupSizingAreaLeftTopPoint} onMouseDown={(e) => this.popupResizeMousePress(e, 'd-lt')}></div>
                <div className={content.popupSizingAreaLeftBottomPoint} onMouseDown={(e) => this.popupResizeMousePress(e, 'd-lb')}></div>

                <div className={content.dslTop + " " + content.dslGrd}>
                    <h5 className={content.dslTitle} onMouseDown={(e) => this.popupDragMousePress(e)}>
                        이벤트 정보
                    </h5>

                    {
                        //this.broadcastIsRunning() &&
                        //<img className={styles.imgBroadcast} src={imgCloseBroadcast} title={SDMSResource.ID.broadcast.close} onClick={this.onClickCloseBroadcast} />
                        this.displayBroadcastUI()
                    }
                    
                    <a className={content.dslX} onClick={() => this.props.setVisiblePopups(SDMS.menu.eventInfo, false)}></a>
                </div>

                <div className={content.dslCont}>
                    <div ref={this.refAlarmList} className={content.alarmList}>
                        <h5 ref={this.refTitle}className={content.dseTitle}>발생현황</h5>
                        <div ref={this.refHeader} className={content.dseTop}>
                            <table>
                                <colgroup>
                                    <col className={content.width_25Pro} />
                                    <col className={content.width_20Pro} />
                                    <col className={content.width_15Pro} />
                                    <col className={content.width_15Pro} />
                                    <col className={content.width_25Pro} />
                                </colgroup>
                                <thead>
                                    <tr>
                                        <th>발생일시</th>
                                        <th>상황유형</th>
                                        <th>단계</th>
                                        <th>위치</th>
                                        <th>대응상태</th>
                                    </tr>
                                </thead>
                            </table>
                        </div>

                        <div className={content.dseTb + " " + content.scrollbar}>
                            <Scrollbars ref={this.refScrollbar}>
                                <table ref={this.refTable} className={content.scrollTable}>
                                    <caption>이벤트 발생일시, 상황유형, 단계, 위치, 대응상태로 구성된 표</caption>
                                    <colgroup>
                                        <col className={content.width_25Pro} />
                                        <col className={content.width_20Pro} />
                                        <col className={content.width_15Pro} />
                                        <col className={content.width_15Pro} />
                                        <col className={content.width_25Pro} />
                                    </colgroup>
                                    <tbody>
                                        {gridUI}
                                    </tbody>
                                </table>
                            </Scrollbars>
                        </div>
                    </div>

                    <div className={content.alarmDetail}>
                        <div className={content.gap10}></div>

                        <h5 className={content.dseTitle}>세부정보</h5>
                        {this.getAlarmInfo()}
                        <ul className={content.dseBtn}>
                            <li onClick={this.onSituationNotice}><span className={"shortcutKey" + " " + content.eventShortCut + " " + uis.hideKey}>Sh+Q</span><a>상황전파</a></li>
                            <li onClick={() => this.onMoveSelectedAlarm()}><span className={"shortcutKey" + " " + content.eventShortCut + " " + uis.hideKey}>Sh+W</span><a>화면전환</a></li>
                            {
                                (this.props.alarmSound)
                                    ? <li onClick={this.onSound}><span className={"shortcutKey" + " " + content.eventShortCut + " " + uis.hideKey}>Sh+E</span><a>소리끄기</a></li>
                                    : <li onClick={this.onSound}><span className={"shortcutKey" + " " + content.eventShortCut + " " + uis.hideKey}>Sh+E</span><a>소리켜기</a></li>
                            }
                            <li onClick={this.onMalfunction}><span className={"shortcutKey" + " " + content.eventShortCut + " " + uis.hideKey}>Sh+R</span><a>종료</a></li>
                        </ul>
                    </div>
                </div>
            </div>
        );
    }
}

export default Event;