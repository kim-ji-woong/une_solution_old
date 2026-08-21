import React, { Component } from 'react';
import { Container } from 'reactstrap';
import $ from 'jquery';
import SelectReceiver from './selectReceiver';
import ConfirmDialog from '../../../Common/ui/confirmDialog';

import newStyles from '../../../Common/css/newStyle.module.css';
import newDefaults from '../../../Common/css/newDefault.module.css';
import settings from '../../css/settings.module.css';

import SettingResource from '../../resource/id';
import { SettingController } from '../../services/settingController';
import SessionString from '../../../Common/js/sessionString';
import SettingsStore from '../../settingsStore';
import { array } from '@amcharts/amcharts4/core';
import SettingsResource from '../../resource/id';
import ProjectResource from '../../../Root/resource/id';
import AccountResource from '../../../Account/resource/id';
import SDMSResource from '../../../SDMS/resource/id';

class Monitoring3D extends Component {
    constructor(props) {
        super(props);

		this.state = {
			tabMenu: SettingResource.ID.monitoring3DMode.normal,
		}

        this.props = props;
	}

	onClickTab = (target, value) => {
		$('.MonitoringTab li a').removeClass(newStyles.on);
		$(target).addClass(newStyles.on);

		this.setState({ tabMenu: value});
	}

	setContentUI = () => {
		let contentUI = [];

		if (this.state.tabMenu === SettingResource.ID.monitoring3DMode.normal) {
			contentUI.push(<NormalTab key="NormalTab" settings={this.props.settings} />);
		} else if (this.state.tabMenu === SettingResource.ID.monitoring3DMode.spread) {
			contentUI.push(<SpreadTab key="SpreadTab" settings={this.props.settings} buildingGroupList={this.props.buildingGroupList} spreadMessages={this.props.spreadMessages} teamTreeDatas={this.props.teamTreeDatas} teams={this.props.teams} members={this.props.members} />);
		} else if (this.state.tabMenu === SettingResource.ID.monitoring3DMode.detection) {
			contentUI.push(<DetectionTab key="DetectionTab" settings={this.props.settings} />);
        }

		return contentUI;
	}

	getAuthorTab() {
		let authorTabUI = [];
		authorTabUI.push(<React.Fragment><li><a onClick={(e) => this.onClickTab(e.target, SettingResource.ID.monitoring3DMode.normal)} className={newStyles.on}>일반</a></li></React.Fragment>);

		let userInfo = ProjectResource.getUserInfo();
		if (userInfo === null || userInfo === undefined)
			return authorTabUI;

		if (userInfo.level === AccountResource.ID.accountLevel.admin) {
			authorTabUI.push(<React.Fragment>
				<li><a onClick={(e) => this.onClickTab(e.target, SettingResource.ID.monitoring3DMode.spread)}>초기상황전파관리</a></li>
				<li><a onClick={(e) => this.onClickTab(e.target, SettingResource.ID.monitoring3DMode.detection)}>센서감지관리</a></li>
			</React.Fragment>);
		}

		return authorTabUI;
	}

	render() {
		const contentUI = this.setContentUI();
		const authorTabUI = this.getAuthorTab();

        return (
            <>
				<ul className={newStyles.stgTab + " MonitoringTab"}>
					{authorTabUI}
				</ul>
				<span className={newStyles.stgScroll}>
					{contentUI}
				</span>
            </>
        );
    }
}

export default Monitoring3D;

class NormalTab extends Component {
	constructor(props) {
		super(props);

		this.refSDMS = React.createRef();
		this.refSOP = React.createRef();
		this.refSOPMgr = React.createRef();
		this.refTeamEdit = React.createRef();
		this.refSettings = React.createRef();
		this.refHistory = React.createRef();
		this.refDashboard = React.createRef();
		this.refHome = React.createRef();
		this.refRotation = React.createRef();

		this.refIdleTime = React.createRef();
		this.refIdleTimeUse = React.createRef();

		this.refBuildingFile = React.createRef();
		this.refGroupFile = React.createRef();
		this.refFacilityFile = React.createRef();

		this.refBuildingFileName = React.createRef();

		this.refUsePoiHighlightOn = React.createRef();
		this.refUsePoiHighlightOff = React.createRef();

		this.refTurnStart01 = React.createRef();
		this.refTurnStart02 = React.createRef();
		this.refUseAlarmTurnOn = React.createRef();
		this.refUseAlarmTurnOff = React.createRef();

		this.state = {
			confirmMessage: {
				visible: false,
				title: "",
				messages: [""],
				buttons: ["확인"],
				onClose: this.onCloseConfirmDialog,
				onClickButton: null
			},
		}

		this.props = props;
	}

	componentDidMount() {
		this.initData();
		this.initShortcutKey();

		// .TODO: 고도화 내용으로 임시 주석처리
		//this.initUsePoiHighlight();
		//this.initTurnStart();
		//this.initUseAlarmTurn();
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

	onCloseConfirmDialog = () => {
		const confirmMessage = { ...this.state.confirmMessage };
		confirmMessage.visible = false;

		this.setState({ confirmMessage });
	}

	initData() {
		if (this.props.settings === null || this.props.settings === undefined)
			return;

		// 3D 모니터 회전 시간 입력 
		let idleTime = this.props.settings.idleTime;

		if (idleTime === null || idleTime === undefined)
			idleTime = "10;1";

		let arrIdleTime = idleTime.split(";");

		if (arrIdleTime.length !== 2) {
			idleTime = "10;1";
			arrIdleTime = idleTime.split(";");
        }

		this.refIdleTime.current.value = arrIdleTime[0];

		if (arrIdleTime[1] === "1")
			this.refIdleTimeUse.current.checked = true;
		else if (arrIdleTime[1] === "0")
			this.refIdleTimeUse.current.checked = false;
	}

	initShortcutKey() {
		if (this.props.settings === null || this.props.settings === undefined)
			return;

		const shortcutKey = this.props.settings.shortcutKey;

		if (shortcutKey.sdms !== null && shortcutKey.sdms !== undefined && shortcutKey.sdms !== "") {
			// SDMS 단축키 입력
			let keyCode = shortcutKey.sdms;

			if (keyCode !== "") {
				keyCode = String.fromCharCode(keyCode);
            }

			this.refSDMS.current.value = keyCode;
		}
		if (shortcutKey.teamEdit !== null && shortcutKey.teamEdit !== undefined && shortcutKey.teamEdit !== "") {
			// teamEdit 단축키 입력
			let keyCode = shortcutKey.teamEdit;

			if (keyCode !== "") {
				keyCode = String.fromCharCode(keyCode);
			}

			this.refTeamEdit.current.value = keyCode;
		}
		if (shortcutKey.settings !== null && shortcutKey.settings !== undefined && shortcutKey.settings !== "") {
			// settings 단축키 입력
			let keyCode = shortcutKey.settings;

			if (keyCode !== "") {
				keyCode = String.fromCharCode(keyCode);
			}

			this.refSettings.current.value = keyCode;
		}
		if (shortcutKey.home !== null && shortcutKey.home !== undefined && shortcutKey.home !== "") {
			// home 단축키 입력
			let keyCode = shortcutKey.home;

			if (keyCode !== "") {
				keyCode = String.fromCharCode(keyCode);
			}

			this.refHome.current.value = keyCode;
		}
		if (shortcutKey.rotation !== null && shortcutKey.rotation !== undefined && shortcutKey.rotation !== "") {
			// rotation 단축키 입력
			let keyCode = shortcutKey.rotation;

			if (keyCode !== "") {
				keyCode = String.fromCharCode(keyCode);
			}

			this.refRotation.current.value = keyCode;
		}
		if (shortcutKey.sop !== null && shortcutKey.sop !== undefined && shortcutKey.sop !== "") {
			// sop 단축키 입력
			let keyCode = shortcutKey.sop;

			if (keyCode !== "") {
				keyCode = String.fromCharCode(keyCode);
			}

			this.refSOP.current.value = keyCode;
		}
		if (shortcutKey.sopMgr !== null && shortcutKey.sopMgr !== undefined && shortcutKey.sopMgr !== "") {
			// sopMgr 단축키 입력
			let keyCode = shortcutKey.sopMgr;

			if (keyCode !== "") {
				keyCode = String.fromCharCode(keyCode);
			}

			this.refSOPMgr.current.value = keyCode;
		}
		if (shortcutKey.dashboard !== null && shortcutKey.dashboard !== undefined && shortcutKey.dashboard !== "") {
			// dashBoard 단축키 입력
			let keyCode = shortcutKey.dashboard;

			if (keyCode !== "") {
				keyCode = String.fromCharCode(keyCode);
			}

			this.refDashboard.current.value = keyCode;
		}
		if (shortcutKey.history !== null && shortcutKey.history !== undefined && shortcutKey.history !== "") {
			// history 단축키 입력
			let keyCode = shortcutKey.history;

			if (keyCode !== "") {
				keyCode = String.fromCharCode(keyCode);
			}

			this.refHistory.current.value = keyCode;
		}
	}

	handleKeyPress = (e, id) => {
		let key = "";
		let code = "";
		let keyCode = null;

		// 숫자 또는 알파벳 또는 숫자 패드의 숫자 아니면 제외
		if (!(e.keyCode > 47 && e.keyCode < 58) && !(e.keyCode > 64 && e.keyCode < 91) && !(e.keyCode > 95 && e.keyCode < 106)) {
			e.preventDefault();
			return;
		}
			
		if (e.keyCode > 95 && e.keyCode < 106) {
			// 키패드 숫자
			keyCode = e.keyCode - 48;
			key = String.fromCharCode(keyCode);
			code = keyCode.toString();
		} else if (e.keyCode !== 8 && e.keyCode !== 46) {
			// 백스페이스 및 딜리트 키는 공백으로
			keyCode = e.keyCode;
			key = String.fromCharCode(keyCode);
			code = keyCode.toString();
		}

		if (keyCode === null || code === "" || key === "") {
			e.preventDefault();
			return;
        }

		let shortcutKey = this.props.settings.shortcutKey;

		if (id === SettingResource.ID.shortcutKey.sdms) {
			if (code !== "" &&
				(shortcutKey.history === code ||
					shortcutKey.sop === code ||
					shortcutKey.sopMgr === code ||
					shortcutKey.teamEdit === code ||
					shortcutKey.dashboard === code ||
					shortcutKey.settings === code ||
					shortcutKey.home === code ||
					shortcutKey.rotation === code)) {
				//alert("현재 사용 중인 단축키 입니다.");
				this.showConfirmDialog("에러", ["현재 사용 중인 단축키 입니다."], null, null);
				this.refSDMS.current.value = "";
				shortcutKey.sdms = "";

			} else {
				this.refSDMS.current.value = key;
				shortcutKey.sdms = code;
			}

			e.preventDefault();
		} else if (id === SettingResource.ID.shortcutKey.teamEdit) {
			if (code !== "" &&
				(shortcutKey.sdms === code ||
					shortcutKey.history === code ||
					shortcutKey.sop === code ||
					shortcutKey.sopMgr === code ||
					shortcutKey.dashboard === code ||
					shortcutKey.settings === code ||
					shortcutKey.home === code ||
					shortcutKey.rotation === code)) {
				//alert("현재 사용 중인 단축키 입니다.");
				this.showConfirmDialog("에러", ["현재 사용 중인 단축키 입니다."], null, null);
				this.refTeamEdit.current.value = "";
				shortcutKey.teamEdit = "";
			} else {
				this.refTeamEdit.current.value = key;
				shortcutKey.teamEdit = code;
			}

			e.preventDefault();
		} else if (id === SettingResource.ID.shortcutKey.settings) {
			if (code !== "" &&
				(shortcutKey.sdms === code ||
					shortcutKey.history === code ||
					shortcutKey.sop === code ||
					shortcutKey.sopMgr === code ||
					shortcutKey.teamEdit === code ||
					shortcutKey.dashboard === code ||
					shortcutKey.home === code ||
					shortcutKey.rotation === code)) {
				//alert("현재 사용 중인 단축키 입니다.");
				this.showConfirmDialog("에러", ["현재 사용 중인 단축키 입니다."], null, null);
				this.refSettings.current.value = "";
				shortcutKey.settings = "";
			} else {
				this.refSettings.current.value = key;
				shortcutKey.settings = code;
			}

			e.preventDefault();
		} else if (id === SettingResource.ID.shortcutKey.home) {
			if (code !== "" &&
				(shortcutKey.sdms === code ||
					shortcutKey.history === code ||
					shortcutKey.sop === code ||
					shortcutKey.sopMgr === code ||
					shortcutKey.teamEdit === code ||
					shortcutKey.dashboard === code ||
					shortcutKey.settings === code ||
					shortcutKey.rotation === code)) {
				//alert("현재 사용 중인 단축키 입니다.");
				this.showConfirmDialog("에러", ["현재 사용 중인 단축키 입니다."], null, null);
				this.refHome.current.value = "";
				shortcutKey.home = "";
			} else {
				this.refHome.current.value = key;
				shortcutKey.home = code;
			}

			e.preventDefault();
		} else if (id === SettingResource.ID.shortcutKey.rotation) {
			if (code !== "" &&
				(shortcutKey.sdms === code ||
					shortcutKey.history === code ||
					shortcutKey.sop === code ||
					shortcutKey.sopMgr === code ||
					shortcutKey.teamEdit === code ||
					shortcutKey.dashboard === code ||
					shortcutKey.home === code ||
					shortcutKey.settings === code)) {
				this.showConfirmDialog("에러", ["현재 사용 중인 단축키 입니다."], null, null);
				this.refRotation.current.value = "";
				shortcutKey.rotation = "";
			} else {
				this.refRotation.current.value = key;
				shortcutKey.rotation = code;
			}

			e.preventDefault();
		}
		else if (id === SettingResource.ID.shortcutKey.sop) {
			if (code !== "" &&
				(shortcutKey.sdms === code ||
					shortcutKey.history === code ||
					shortcutKey.sopMgr === code ||
					shortcutKey.teamEdit === code ||
					shortcutKey.dashboard === code ||
					shortcutKey.settings === code ||
					shortcutKey.home === code ||
					shortcutKey.rotation === code)) {
				//alert("현재 사용 중인 단축키 입니다.");
				this.showConfirmDialog("에러", ["현재 사용 중인 단축키 입니다."], null, null);
				this.refSOP.current.value = "";
				shortcutKey.sop = "";
			} else {
				this.refSOP.current.value = key;
				shortcutKey.sop = code;
			}

			e.preventDefault();
		} else if (id === SettingResource.ID.shortcutKey.sopMgr) {
			if (code !== "" &&
				(shortcutKey.sdms === code ||
					shortcutKey.history === code ||
					shortcutKey.sop === code ||
					shortcutKey.teamEdit === code ||
					shortcutKey.dashboard === code ||
					shortcutKey.settings === code ||
					shortcutKey.home === code ||
					shortcutKey.rotation === code)) {
				//alert("현재 사용 중인 단축키 입니다.");
				this.showConfirmDialog("에러", ["현재 사용 중인 단축키 입니다."], null, null);
				this.refSOPMgr.current.value = "";
				shortcutKey.sopMgr = "";
			} else {
				this.refSOPMgr.current.value = key;
				shortcutKey.sopMgr = code;
			}

			e.preventDefault();
		} else if (id === SettingResource.ID.shortcutKey.dashBoard) {
			if (code !== "" &&
				(shortcutKey.sdms === code ||
					shortcutKey.history === code ||
					shortcutKey.sop === code ||
					shortcutKey.sopMgr === code ||
					shortcutKey.teamEdit === code ||
					shortcutKey.settings === code ||
					shortcutKey.home === code ||
					shortcutKey.rotation === code)) {
				//alert("현재 사용 중인 단축키 입니다.");
				this.showConfirmDialog("에러", ["현재 사용 중인 단축키 입니다."], null, null);
				this.refDashboard.current.value = "";
				shortcutKey.dashboard = "";
			} else {
				this.refDashboard.current.value = key;
				shortcutKey.dashboard = code;
			}

			e.preventDefault();
		} else if (id === SettingResource.ID.shortcutKey.history) {
			if (code !== "" &&
				(shortcutKey.sdms === code ||
					shortcutKey.sop === code ||
					shortcutKey.sopMgr === code ||
					shortcutKey.teamEdit === code ||
					shortcutKey.dashboard === code ||
					shortcutKey.settings === code ||
					shortcutKey.home === code ||
					shortcutKey.rotation === code)) {
				//alert("현재 사용 중인 단축키 입니다.");
				this.showConfirmDialog("에러", ["현재 사용 중인 단축키 입니다."], null, null);
				this.refHistory.current.value = "";
				shortcutKey.history = "";
			} else {
				this.refHistory.current.value = key;
				shortcutKey.history = code;
			}

			e.preventDefault();
		}
	}

	initUsePoiHighlight() {
		if (this.props.settings === null || this.props.settings === undefined)
			return;

		let usePoiHighlight = this.props.settings.usePoiHighlight;

		if (usePoiHighlight === SettingsResource.usePoiHighlight.on) {
			this.refUsePoiHighlightOn.current.click();
		} else if (usePoiHighlight === SettingsResource.usePoiHighlight.off) {
			this.refUsePoiHighlightOff.current.click();
		}
	}

	initTurnStart() {
		if (this.props.settings === null || this.props.settings === undefined)
			return;

		let turnStart = this.props.settings.turnStart;

		if (turnStart === SettingsResource.turnStart.LastView) {
			this.refTurnStart01.current.click();
		} else if (turnStart === SettingsResource.turnStart.StandardView) {
			this.refTurnStart02.current.click();
		}
	}

	initUseAlarmTurn() {
		if (this.props.settings === null || this.props.settings === undefined)
			return;

		let useAlarmTurn = this.props.settings.useAlarmTurn;

		if (useAlarmTurn === SettingsResource.useAlarmTurn.on) {
			this.refUseAlarmTurnOn.current.click();
		} else if (useAlarmTurn === SettingsResource.useAlarmTurn.off) {
			this.refUseAlarmTurnOff.current.click();
		}
	}

	onClickPopupReset = () => {
		// 시스템 팝업 위치/사이즈 리셋
		this.showConfirmDialog("확인", ["팝업창 위치/사이즈 초기화를 하시겠습니까?"], ["확인", "취소"], this.onClickDialogPopupReset);
	}

	onClickAccoutPopupSet = () => {
		// 사용자 팝업 위치/사이즈 저장
		this.showConfirmDialog("확인", ["현재 팝업창 위치/사이즈를 저장하시겠습니까?"], ["확인", "취소"], this.onClickDialogAccoutPopupSet);
	}

	onClickAccoutPopupReset = () => {
		// 사용자 팝업 위치/사이즈 리셋
		this.showConfirmDialog("확인", ["팝업창 위치/사이즈를 사용자 저장 값으로 초기화를 하시겠습니까?"], ["확인", "취소"], this.onClickDialogAccoutPopupReset);
	}

	onClickDialogPopupReset = (index) => {
		if (index === 0) {
			this.doPopupReset();
			this.onCloseConfirmDialog();
        } else if (index === 1)
			this.onCloseConfirmDialog();
	}

	onClickDialogAccoutPopupSet = (index) => {
		if (index === 0) {
			this.doAccoutPopupSet();
			this.onCloseConfirmDialog();
		} else if (index === 1)
			this.onCloseConfirmDialog();
	}

	onClickDialogAccoutPopupReset = (index) => {
		if (index === 0) {
			this.doAccoutPopupReset();
			this.onCloseConfirmDialog();
		} else if (index === 1)
			this.onCloseConfirmDialog();
	}

	selectUsePoiHighlight = (use) => {
		if (use === null || use === undefined)
			return;

		if (use === SettingsResource.usePoiHighlight.on ||
			use === SettingsResource.usePoiHighlight.off) {
			this.props.settings.usePoiHighlight = use;
		}
	}

	async doPopupReset() {
		// 데이터 팝업 위치 및 사이즈 초기화
		let popupState = [];

		let cctvInfo = SDMSResource.popupResetLocation.cctvInfo;
		let cctvInfo_1 = SDMSResource.popupResetLocation.cctvInfo_1;
		let cctvInfo_2 = SDMSResource.popupResetLocation.cctvInfo_2;
		let cctvInfo_3 = SDMSResource.popupResetLocation.cctvInfo_3;
		let weatherInfo = SDMSResource.popupResetLocation.weatherInfo;
		let statusInfo = SDMSResource.popupResetLocation.statusInfo;
		let buildingInfo = SDMSResource.popupResetLocation.buildingInfo;
		let dashboard = SDMSResource.popupResetLocation.dashboard;
		let miniMap = SDMSResource.popupResetLocation.miniMap;
		let event = SDMSResource.popupResetLocation.event;

		popupState = {
			cctvInfo: cctvInfo,
			cctvInfo_1: cctvInfo_1,
			cctvInfo_2: cctvInfo_2,
			cctvInfo_3: cctvInfo_3,
			weatherInfo: weatherInfo,
			statusInfo: statusInfo,
			buildingInfo: buildingInfo,
			dashboard: dashboard,
			miniMap: miniMap,
			event: event,
		};

		SettingsStore.dispatch({ type: 'RESET_POPUP', popupState: popupState });

		let userInfo = ProjectResource.getUserInfo();
		if (userInfo === null || userInfo === undefined) {
			this.showConfirmDialog("에러", ["유저 정보를 불러오지 못했습니다. 다시 시도해주세요."], null, null);
			return;
        }

		// DB 값 초기화
		const [surcess, message] = await SettingController.requestResetPopup(userInfo.id, popupState);

		if (surcess === null) {
			//alert(message);
			this.showConfirmDialog("에러", [message], null, null);
			return;
		}
	}

	async doAccoutPopupSet() {
		let userInfo = ProjectResource.getUserInfo();
		if (userInfo === null || userInfo === undefined) {
			this.showConfirmDialog("에러", ["유저 정보를 불러오지 못했습니다. 다시 시도해주세요."], null, null);
			return;
		}

		// 현재 팝업 사이즈/위치 설정값 계정 팝업에 저장
		const [surcess, message] = await SettingController.requestSetAccoutPopup(userInfo.id);

		if (surcess === null) {
			this.showConfirmDialog("에러", [message], null, null);
			return;
		}
    }

	async doAccoutPopupReset() {
		let userInfo = ProjectResource.getUserInfo();
		if (userInfo === null || userInfo === undefined) {
			this.showConfirmDialog("에러", ["유저 정보를 불러오지 못했습니다. 다시 시도해주세요."], null, null);
			return;
		}

		// 계정 팝업 사이즈/위치 불러오기
		const [result, message] = await SettingController.requestResetAccoutPopup(userInfo.id);

		if (result === null) {
			this.showConfirmDialog("에러", [message], null, null);
			return;
		}

		// 계정 팝업 위치/사이즈 초기화
		let popupState = [];

		let cctvInfo = null;
		let cctvInfo_1 = null;
		let cctvInfo_2 = null;
		let cctvInfo_3 = null;
		let weatherInfo = null;
		let statusInfo = null;
		let buildingInfo = null;
		let dashboard = null;
		let miniMap = null;
		let event = null;

		for (let i = 0; i < result.length; i++) {
			const data = result[i];

			if (data.category === "accountPopup" && data.subCategory === "cctvInfo") {
				cctvInfo = {
					x: data.propertyValue1, y: data.propertyValue2, height: data.propertyValue3, width: data.propertyValue4
				};
			} else if (data.category === "accountPopup" && data.subCategory === "cctvInfo_1") {
				cctvInfo_1 = {
					x: data.propertyValue1, y: data.propertyValue2, height: data.propertyValue3, width: data.propertyValue4
				};
			} else if (data.category === "accountPopup" && data.subCategory === "cctvInfo_2") {
				cctvInfo_2 = {
					x: data.propertyValue1, y: data.propertyValue2, height: data.propertyValue3, width: data.propertyValue4
				};
			} else if (data.category === "accountPopup" && data.subCategory === "cctvInfo_3") {
				cctvInfo_3 = {
					x: data.propertyValue1, y: data.propertyValue2, height: data.propertyValue3, width: data.propertyValue4
				};
			} else if (data.category === "accountPopup" && data.subCategory === "weatherInfo") {
				weatherInfo = {
					x: data.propertyValue1, y: data.propertyValue2, height: data.propertyValue3, width: data.propertyValue4
				};
			} else if (data.category === "accountPopup" && data.subCategory === "statusInfo") {
				statusInfo = {
					x: data.propertyValue1, y: data.propertyValue2, height: data.propertyValue3, width: data.propertyValue4
				};
			} else if (data.category === "accountPopup" && data.subCategory === "buildingInfo") {
				buildingInfo = {
					x: data.propertyValue1, y: data.propertyValue2, height: data.propertyValue3, width: data.propertyValue4
				};
			} else if (data.category === "accountPopup" && data.subCategory === "dashboard") {
				dashboard = {
					x: data.propertyValue1, y: data.propertyValue2, height: data.propertyValue3, width: data.propertyValue4
				};
			} else if (data.category === "accountPopup" && data.subCategory === "miniMap") {
				miniMap = {
					x: data.propertyValue1, y: data.propertyValue2, height: data.propertyValue3, width: data.propertyValue4
				};
			} else if (data.category === "accountPopup" && data.subCategory === "event") {
				event = {
					x: data.propertyValue1, y: data.propertyValue2, height: data.propertyValue3, width: data.propertyValue4
				};
			}
        }
		
		popupState = {
			cctvInfo: cctvInfo,
			cctvInfo_1: cctvInfo_1,
			cctvInfo_2: cctvInfo_2,
			cctvInfo_3: cctvInfo_3,
			weatherInfo: weatherInfo,
			statusInfo: statusInfo,
			buildingInfo: buildingInfo,
			dashboard: dashboard,
			miniMap: miniMap,
			event: event,
		};

		SettingsStore.dispatch({ type: 'RESET_POPUP', popupState: popupState });
	}

	onChangeCheck = (e) => {
		let value = e.target.value;
		let inputValue = value.replace(/[^0-9\b .]/g, '');

		if (!$.isNumeric(inputValue)) {
			this.refIdleTime.current.value = "";
		} else {
			//let num = parseFloat(inputValue);
			//inputValue = num.toString();
			this.refIdleTime.current.value = inputValue;
        }
			
	}

	onBlurCheck = (e) => {
		let value = e.value;

		if (value !== "") {
			let num = parseFloat(value);
			value = num.toString();
        } 

		if (value === "0") {
			//alert("0 은 입력하실 수 없습니다.");
			this.showConfirmDialog("에러", ["0 은 입력하실 수 없습니다."], null, null);
			value = "";
        }

		this.refIdleTime.current.value = value;

		if (this.props.settings === null || this.props.settings === undefined)
			return;

		let use = this.refIdleTimeUse.current.checked;
		let useValue = "1";

		if (use === false)
			useValue = "0";

		value = value + ";" + useValue;
		this.props.settings.idleTime = value;
	}

	onChangeIdleTimeUse = () => {
		if (this.props.settings === null || this.props.settings === undefined)
			return;

		let value = this.refIdleTime.current.value;
		let use = this.refIdleTimeUse.current.checked;
		let useValue = "1";

		if (use === false)
			useValue = "0";

		value = value + ";" + useValue;
		this.props.settings.idleTime = value;
	}

	onClickUpload = (mode) => {
		if (mode === SettingResource.ID.excelMode.building) {
			this.refBuildingFile.current.click();
		} else if (mode === SettingResource.ID.excelMode.group) {
			this.refGroupFile.current.click();
		} else if (mode === SettingResource.ID.excelMode.facility) {
			this.refFacilityFile.current.click();
		}
	}

	onSelectBuildingFile = (event) => {
		const file = event.target.files[0];
		this.refBuildingFile.current.value = "";

		this.props.settings.buildingFile = file;
	}

	onSelectGroupFile = (event) => {
		const file = event.target.files[0];
		this.refGroupFile.current.value = "";

		this.props.settings.groupFile = file;
	}

	onSelectFacilityFile = (event) => {
		const file = event.target.files[0];
		this.refFacilityFile.current.value = "";

		this.props.settings.facilityFile = file;
	}

	onClickDownload = (mode) => {
		if (mode === SettingResource.ID.excelMode.building) {
			this.downloadBuilding();
		} else if (mode === SettingResource.ID.excelMode.group) {
			this.downloadBuildingGroup();
		} else if (mode === SettingResource.ID.excelMode.facility) {
			this.downloadFacility();
		}
	}

	onChangeBroadcastUse = (use) => {
		if (this.props.settings === null || this.props.settings === undefined)
			return;

		this.refUseBroadcast.current.checked = !this.refUseBroadcast.current.checked;
		this.refNotUseBroadcast.current.checked = !this.refNotUseBroadcast.current.checked;

		/*if (use) {
			this.refUseBroadcast.current.value = true;
			this.refNotUseBroadcast.current.value = false;
		}
		else {
			this.refUseBroadcast.current.value = false;
			this.refNotUseBroadcast.current.value = true;
        }*/

		/*let value = this.refIdleTime.current.value;
		let use = this.refIdleTimeUse.current.checked;
		let useValue = "1";

		if (use === false)
			useValue = "0";

		value = value + ";" + useValue;
		this.props.settings.idleTime = value;*/
	}

	async downloadBuilding() {
		const [surcess, message] = await SettingController.requestDownloadBuilding();

		if (surcess === null) {
			//alert(message);
			this.showConfirmDialog("에러", [message], null, null);
        }
	}

	async downloadBuildingGroup() {
		const [surcess, message] = await SettingController.requestDownloadBuildingGroup();

		if (surcess === null) {
			//alert(message);
			this.showConfirmDialog("에러", [message], null, null);
		}
	}

	async downloadFacility() {
		const [surcess, message] = await SettingController.requestDownloadFacility();

		if (surcess === null) {
			//alert(message);
			this.showConfirmDialog("에러", [message], null, null);
		}
	}

	getAuthorUI() {
		let authorMenuUI = [];

		let userInfo = ProjectResource.getUserInfo();
		if (userInfo === null || userInfo === undefined)
			return authorMenuUI;

		if (userInfo.level === AccountResource.ID.accountLevel.admin) {
			authorMenuUI.push(<React.Fragment>
				<div className={newStyles.stgName}>
					<h5>건물정보 업데이트</h5>
					<span className={newStyles.stgTltp} data-tooltip="건물 정보를 엑셀파일 형식으로 업로드 합니다."></span>
					<a onClick={() => this.onClickUpload(SettingResource.ID.excelMode.building)} className={newStyles.stgnRset + " " + newStyles.upload}>업로드</a>
					<a onClick={() => this.onClickDownload(SettingResource.ID.excelMode.building)} className={newStyles.stgnRset + " " + newDefaults.ml5}>다운로드</a>
					<input ref={this.refBuildingFile} className={settings.hidden} type='file' accept='.xls,.xlsx' onChange={this.onSelectBuildingFile} />
				</div>
				<div className={newStyles.stgName}>
					<h5>건물그룹 정보 업데이트</h5>
					<span className={newStyles.stgTltp} data-tooltip="건물그룹 정보를 엑셀파일 형식으로 업로드 합니다."></span>
					<a onClick={() => this.onClickUpload(SettingResource.ID.excelMode.group)} className={newStyles.stgnRset + " " + newStyles.upload}>업로드</a>
					<a onClick={() => this.onClickDownload(SettingResource.ID.excelMode.group)} className={newStyles.stgnRset + " " + newDefaults.ml5}>다운로드</a>
					<input ref={this.refGroupFile} className={settings.hidden} type='file' accept='.xls,.xlsx' onChange={this.onSelectGroupFile} />
				</div>
				<div className={newStyles.stgName}>
					<h5>설비정보 업데이트</h5>
					<span className={newStyles.stgTltp} data-tooltip="설비 정보를 엑셀파일 형식으로 업로드 합니다."></span>
					<a onClick={() => this.onClickUpload(SettingResource.ID.excelMode.facility)} className={newStyles.stgnRset + " " + newStyles.upload}>업로드</a>
					<a onClick={() => this.onClickDownload(SettingResource.ID.excelMode.facility)} className={newStyles.stgnRset + " " + newDefaults.ml5}>다운로드</a>
					<input ref={this.refFacilityFile} className={settings.hidden} type='file' accept='.xls,.xlsx' onChange={this.onSelectFacilityFile} />
				</div>
			</React.Fragment>);
		}

		return authorMenuUI;
	}

	selectTurnStart = (mode) => {
		if (mode === null || mode === undefined)
			return;

		if (mode === SettingsResource.turnStart.LastView ||
			mode === SettingsResource.turnStart.StandardView) {
			this.props.settings.turnStart = mode;
		}
	}

	selectUseAlarmTurn = (mode) => {
		if (mode === null || mode === undefined)
			return;

		if (mode === SettingsResource.useAlarmTurn.on ||
			mode === SettingsResource.useAlarmTurn.off) {
			this.props.settings.useAlarmTurn = mode;
		}
	}

	render() {
		const authorMenuUI = this.getAuthorUI();

		return (
			<>
				<div className={newStyles.stgList}>
					<div className={newStyles.stgName}>
						<h5>3D 회전 대기시간</h5>
						<span className={newStyles.stgTltp} data-tooltip="3D 회전 대기시간을 설정 합니다."></span>
						<input type="text" ref={this.refIdleTime} className={settings.settingInput} name="" id="" onChange={(e) => this.onChangeCheck(e)} onBlur={(e) => this.onBlurCheck(e.target)} /><span className={settings.white}> 분 &nbsp;&nbsp;</span>
						<span className={settings.white}>&nbsp;&nbsp;자동회전 사용 &nbsp;&nbsp;</span><input ref={this.refIdleTimeUse} type="checkbox" name="" id="" onChange={this.onChangeIdleTimeUse} />
					</div>
					{/*	.TODO: 고도화 내용으로 임시 주석처리 */}
					{/*<div className={newStyles.stgName}>*/}
					{/*	<h5>자동회전 시작점</h5>*/}
					{/*	<span className={newStyles.stgTltp} data-tooltip="자동회전 시작시 기준화면 설정">&nbsp;&nbsp;</span>*/}

					{/*	<input type="radio" ref={this.refTurnStart01} name="turnStart" id="turnStart01" onChange={() => this.selectTurnStart(SettingResource.turnStart.LastView)} />*/}
					{/*	<span className={settings.white}>&nbsp;&nbsp;마지막 위치 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>*/}

					{/*	<input type="radio" ref={this.refTurnStart02} name="turnStart" id="turnStart02" onChange={() => this.selectTurnStart(SettingResource.turnStart.StandardView)} />*/}
					{/*	<span className={settings.white}>&nbsp;&nbsp;기본뷰 &nbsp;&nbsp;</span>*/}
					{/*</div>*/}
					{/*<div className={newStyles.stgName}>*/}
					{/*	<h5>알람시 회전기능</h5>*/}
					{/*	<span className={newStyles.stgTltp} data-tooltip="알람 발생시 회전기능 사용여부">&nbsp;&nbsp;</span>*/}

					{/*	<input type="radio" ref={this.refUseAlarmTurnOn} name="useAlarmTurn" id="useAlarmTurnOn" onChange={() => this.selectUseAlarmTurn(SettingResource.useAlarmTurn.on)} />*/}
					{/*	<span className={settings.white}>&nbsp;&nbsp;사용 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>*/}

					{/*	<input type="radio" ref={this.refUseAlarmTurnOff} name="useAlarmTurn" id="useAlarmTurnOff" onChange={() => this.selectUseAlarmTurn(SettingResource.useAlarmTurn.off)} />*/}
					{/*	<span className={settings.white}>&nbsp;&nbsp;사용안함 &nbsp;&nbsp;</span>*/}
					{/*</div>*/}
					<div className={newStyles.stgName}>
						<h5>팝업창 위치/사이즈 초기화</h5>
						<span className={newStyles.stgTltp} data-tooltip="팝업창위치 및 사이즈를 초기화 합니다."></span>
						<a onClick={this.onClickPopupReset} className={newStyles.stgnRset}>초기화</a>
						{/*	.TODO: 고도화 내용으로 임시 주석처리
						<a onClick={this.onClickAccoutPopupSet} className={newStyles.stgnRset}>사용자 설정</a>
						<a onClick={this.onClickAccoutPopupReset} className={newStyles.stgnRset}>사용자 초기화</a>
						*/}
					</div>

					{authorMenuUI}

					{/*	.TODO: 고도화 내용으로 임시 주석처리
					<div className={newStyles.stgName + " " + settings.settingRadio}>
						<h5>POI 하이라이트</h5>
						<span className={newStyles.stgTltp} data-tooltip="POI 선택시 선택된 POI 및 같은 공간의 POI 확대 여부를 설정 합니다"></span>
						<span className={settings.white}>&nbsp;&nbsp;사용 &nbsp;&nbsp;</span>
						<input type="radio" ref={this.refUsePoiHighlightOn} name="usePoiHighlight" id="usePoiHighlightOn" onChange={() => this.selectUsePoiHighlight(SettingResource.usePoiHighlight.on)}  />

						<span className={settings.white}>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 사용안함 &nbsp;&nbsp;</span>
						<input type="radio" ref={this.refUsePoiHighlightOff} name="usePoiHighlight" id="usePoiHighlightOff" onChange={() => this.selectUsePoiHighlight(SettingResource.usePoiHighlight.off)} />

					</div>
					*/}

					<div className={newStyles.stgName}>
						<h5>단축키 설정</h5>
						<span className={newStyles.stgTltp} data-tooltip="단위 시스템 불러오기 단축키 기능을 설정 합니다."></span>
						<ul className={newStyles.stgnKey}>
							<li><dl><dt>3D 관제시스템</dt><dd><span>Alt +</span><input ref={this.refSDMS} type="text" min="0" name="" id="keySDMS" onKeyDown={(e) => this.handleKeyPress(e, SettingResource.ID.shortcutKey.sdms)} /></dd></dl></li>
							<li><dl><dt>이력</dt><dd><span>Alt +</span><input ref={this.refHistory} type="text" name="" id="keyHistory" onKeyDown={(e) => this.handleKeyPress(e, SettingResource.ID.shortcutKey.history)} /></dd></dl></li>
							<li><dl><dt>대시보드</dt><dd><span>Alt +</span><input ref={this.refDashboard} type="text" name="" id="keyDashboard" onKeyDown={(e) => this.handleKeyPress(e, SettingResource.ID.shortcutKey.dashBoard)} /></dd></dl></li>
							<li><dl><dt>조직관리</dt><dd><span>Alt +</span><input ref={this.refTeamEdit} type="text" name="" id="keyTeamEdit" onKeyDown={(e) => this.handleKeyPress(e, SettingResource.ID.shortcutKey.teamEdit)} /></dd></dl></li>
							<li><dl><dt>SOP 실행</dt><dd><span>Alt +</span><input ref={this.refSOP} type="text" name="" id="keySOP" onKeyDown={(e) => this.handleKeyPress(e, SettingResource.ID.shortcutKey.sop)} /></dd></dl></li>
							<li><dl><dt>설정</dt><dd><span>Alt +</span><input ref={this.refSettings} type="text" name="" id="keySettings" onKeyDown={(e) => this.handleKeyPress(e, SettingResource.ID.shortcutKey.settings)} /></dd></dl></li>
							<li><dl><dt>SOP편집</dt><dd><span>Alt +</span><input ref={this.refSOPMgr} type="text" name="" id="keySOPMgr" onKeyDown={(e) => this.handleKeyPress(e, SettingResource.ID.shortcutKey.sopMgr)} /></dd></dl></li>
							<li><dl><dt>홈 버튼</dt><dd><span>Alt +</span><input ref={this.refHome} type="text" name="" id="keyHome" onKeyDown={(e) => this.handleKeyPress(e, SettingResource.ID.shortcutKey.home)} /></dd></dl></li>
							<li><dl><dt>즉시회전</dt><dd><span>Alt +</span><input ref={this.refRotation} type="text" name="" id="keyRotation" onKeyDown={(e) => this.handleKeyPress(e, SettingResource.ID.shortcutKey.rotation)} /></dd></dl></li>
						</ul>
					</div>

				</div>
				{
					/* alert창 대신 사용 */
					this.state.confirmMessage.visible &&
					<ConfirmDialog title={this.state.confirmMessage.title} messages={this.state.confirmMessage.messages} buttons={this.state.confirmMessage.buttons} onClose={this.state.confirmMessage.onClose} onClickButton={this.state.confirmMessage.onClickButton} />
				}
			</>
		);
	}
}

class SpreadTab extends Component {
	constructor(props) {
		super(props);

		this.state = {
			//buildingGroupList: null,
			buildingList: [],
			messageType: SettingResource.messageType.sms,
			message: "",
			receiver: "",
			selectPopupOnOff: false,
			selectFacilityType: "",			// 전파 대상자지정 팝업창 전달용 인자
			selectBuildingGroup: "",		// 전파 대상자지정 팝업창 전달용 인자
			selectBuilding: "",				// 전파 대상자지정 팝업창 전달용 인자
			selectRegularID: null,			// 전파 대상자지정 팝업창 전달용 인자
			selectRegularMemberID: null,	// 전파 대상자지정 팝업창 전달용 인자

			confirmMessage: {
				visible: false,
				title: "",
				messages: [""],
				buttons: ["확인"],
				onClose: this.onCloseConfirmDialog,
				onClickButton: null
			},
		}

		this.props = props;

		this.refFacilityType = React.createRef();
		this.refBuildingGroup = React.createRef();
		this.refBuilding = React.createRef();
		this.refMessage = React.createRef();
		this.refRegularID = React.createRef();
		this.refRegularMemberID = React.createRef();
		

		//if (this.props.buildingGroupList !== null && this.props.buildingGroupList !== undefined) {
		//	this.state.buildingGroupList = this.props.buildingGroupList;
		//}
	}

	componentDidMount() {
		$('.' + newStyles.stgmTab + " li a").click(function() {
			$('.' + newStyles.stgmTab + " li a").removeClass(newStyles.on);
			$(this).addClass(newStyles.on);
			
		});

		// 해당 메시지 및 전파인원 초기화 
		let type = this.refFacilityType.current.value;
		let buildingGroupID = this.refBuildingGroup.current.value;
		let buildingID = this.refBuilding.current.value;

		const [message, receiver, regularID, regularMemberID] = this.getSpreadInfo(type, buildingGroupID, buildingID);

		this.setState({ receiver: receiver });
		this.refMessage.current.value = message;
		this.refRegularID.current.value = regularID;
		this.refRegularMemberID.current.value = regularMemberID;
	}

	componentDidUpdate(prevProps, prevState) {
		//if (this.props.buildingGroupList !== prevProps.buildingGroupList) {
		//	this.setState({ buildingGroupList: this.props.buildingGroupList });
		//}
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

	onCloseConfirmDialog = () => {
		const confirmMessage = { ...this.state.confirmMessage };
		confirmMessage.visible = false;

		this.setState({ confirmMessage });
	}

	updateSpreadMessage(type, buildingGroupID, buildingID, messageType, message, regularID, regularMemberID) {
		let dataType = parseInt(type);
		let dataBuildingGroupID = null;
		let dataBuildingID = null;
		let dataMessageType = messageType;
		let chk = false;

		if (buildingGroupID !== null && buildingGroupID !== undefined && buildingGroupID !== "")
			dataBuildingGroupID = parseInt(buildingGroupID);

		if (buildingID !== null && buildingID !== undefined && buildingID !== "")
			dataBuildingID = parseInt(buildingID);

		for (let i = 0; i < this.props.spreadMessages.length; i++) {
			let spreadMessage = this.props.spreadMessages[i];

			if (dataType === spreadMessage.facilityType &&
				spreadMessage.buildingGroupID === dataBuildingGroupID &&
				spreadMessage.buildingID === dataBuildingID &&
				spreadMessage.messageType === dataMessageType) {
				spreadMessage.message = message;
				spreadMessage.regularID = regularID;
				spreadMessage.regularMemberID = regularMemberID;
				chk = true;
				break;
			}
		}

		if (chk === false) {
			let spreadMessage = {
				id: -1,
				facilityType: dataType,
				buildingGroupID: dataBuildingGroupID,
				buildingID: dataBuildingID,
				regularID: regularID,
				regularMemberID: regularMemberID,
				messageType: dataMessageType,
				message: message,
			};

			this.props.spreadMessages.push(spreadMessage);
        }

    }

	getSpreadInfo(type, buildingGroupID, buildingID) {
		if (this.props.spreadMessages === null || this.props.spreadMessages === undefined)
			return [null, null, null, null];

		let dataType = parseInt(type);
		let dataBuildingGroupID = null;
		let dataBuildingID = null;

		if (buildingGroupID !== null && buildingGroupID !== undefined && buildingGroupID !== "")
			dataBuildingGroupID = parseInt(buildingGroupID);

		if (buildingID !== null && buildingID !== undefined && buildingID !== "")
			dataBuildingID = parseInt(buildingID);

		let message = "";
		let receiver = "";
		let regularID = "";
		let regularMemberID = "";
		const spreadMessages = this.props.spreadMessages;

		for (let i = 0; i < spreadMessages.length; i++) {
			let spreadMessage = spreadMessages[i];

			if (dataType === spreadMessage.facilityType &&
				spreadMessage.buildingGroupID === dataBuildingGroupID &&
				spreadMessage.buildingID === dataBuildingID &&
				spreadMessage.messageType === this.state.messageType) {
				message = spreadMessage.message;
				regularID = spreadMessage.regularID;
				regularMemberID = spreadMessage.regularMemberID;
				break;
			}
		}

		if (regularID !== null && regularID !== undefined && regularID !== "") {
			let arrRegular = regularID.split(",");

			// 팀명 조회
			if (this.props.teams !== null && this.props.teams !== undefined && arrRegular.length > 0) {
				let teams = this.props.teams;

				for (let i = 0; i < arrRegular.length; i++) {
					let regular = arrRegular[i];

					for (let j = 0; j < teams.length; j++) {
						let team = teams[j];

						if (team.id.toString() === regular) {
							if (receiver === "")
								receiver += team.teamName;
							else
								receiver = receiver + ", " + team.teamName;

							break;
						}
					}
				}
			}
		}

		if (regularMemberID !== null && regularMemberID !== undefined && regularMemberID !== "") {
			let arrMembers = regularMemberID.split(",");

			// 멤버 조회
			if (this.props.members !== null && this.props.members !== undefined && arrMembers.length > 0) {
				let members = this.props.members;

				for (let i = 0; i < arrMembers.length; i++) {
					let arrMember = arrMembers[i];

					for (let j = 0; j < members.length; j++) {
						let member = members[j];

						if (member.ID.toString() === arrMember) {
							if (receiver === "")
								receiver += member.MemberName;
							else
								receiver = receiver + ", " + member.MemberName;

							break;
						}
					}

				}
			}
		}

		return [message, receiver, regularID, regularMemberID];
	}

	onChangeFacilityType = () => {
		let type = this.refFacilityType.current.value;
		this.refBuildingGroup.current.value = "";
		this.refBuilding.current.value = "";

		const [message, receiver, regularID, regularMemberID] = this.getSpreadInfo(type, null, null);

		this.setState({ receiver: receiver });
		this.refMessage.current.value = message;
		this.refRegularID.current.value = regularID;
		this.refRegularMemberID.current.value = regularMemberID;
	}

	onChangeBuilding = () => {
		if (this.props.spreadMessages === null || this.props.spreadMessages === undefined)
			return;

		// 해당 메시지 및 전파인원 
		let type = this.refFacilityType.current.value;
		let buildingGroupID = this.refBuildingGroup.current.value;
		let buildingID = this.refBuilding.current.value;

		const [message, receiver, regularID, regularMemberID] = this.getSpreadInfo(type, buildingGroupID, buildingID);

		this.setState({ receiver: receiver });
		this.refMessage.current.value = message;
		this.refRegularID.current.value = regularID;
		this.refRegularMemberID.current.value = regularMemberID;
    }

	setBuildingGroupUI = () => {
		let buildingGroupUI = [];

		if (this.props.buildingGroupList !== null && this.props.buildingGroupList !== undefined) {
			const buildingGroupList = this.props.buildingGroupList;

			for (let i = 0; i < buildingGroupList.length; i++) {
				let buildingGroup = buildingGroupList[i];

				buildingGroupUI.push(<option key={"buildingGroup_" + buildingGroup.id} value={buildingGroup.id}>{buildingGroup.displayText}</option>);
            }

		} else {
			buildingGroupUI.push(<></>);
        }

		return buildingGroupUI;
	}

	

	onChangeBuildingGroup = () => {
		// 건물 리스트 생성
		let buildingGroupID = this.refBuildingGroup.current.value;
		this.refBuilding.current.value = "";
		let buildingList = [];

		if (buildingGroupID !== null && buildingGroupID !== undefined && buildingGroupID !== "") {
			for (let i = 0; i < this.props.buildingGroupList.length; i++) {
				let buildingGroup = this.props.buildingGroupList[i];

				if (buildingGroupID === buildingGroup.id.toString()) {
					for (let j = 0; j < buildingGroup.buildingDatas.length; j++) {
						let building = buildingGroup.buildingDatas[j];

						buildingList.push(<option value={building.id}>{building.displayText}</option>);
					}

					break;
                }
            }
        }

		this.setState({ buildingList: buildingList });

		// 해당 메시지 및 전파인원 
		let type = this.refFacilityType.current.value;

		const [message, receiver, regularID, regularMemberID] = this.getSpreadInfo(type, buildingGroupID, null);

		this.setState({ receiver: receiver });
		this.refMessage.current.value = message;
		this.refRegularID.current.value = regularID;
		this.refRegularMemberID.current.value = regularMemberID;

	}

	onClickMessageTyep = (mode) => {
		//this.setState({ messageType: mode });
		this.state.messageType = mode;

		if (this.props.spreadMessages === null || this.props.spreadMessages === undefined)
			return;

		// 해당 메시지 및 전파인원 
		let type = this.refFacilityType.current.value;
		let buildingGroupID = this.refBuildingGroup.current.value;
		let buildingID = this.refBuilding.current.value;

		const [message, receiver, regularID, regularMemberID] = this.getSpreadInfo(type, buildingGroupID, buildingID);

		this.setState({ receiver: receiver });
		this.refMessage.current.value = message;
		this.refRegularID.current.value = regularID;
		this.refRegularMemberID.current.value = regularMemberID;
    }

	onClickMsgSave = () => {
		if (this.props.spreadMessages === null || this.props.spreadMessages === undefined)
			return;

		// 해당 메시지 및 전파인원 
		let type = this.refFacilityType.current.value;
		let buildingGroupID = this.refBuildingGroup.current.value;
		let buildingID = this.refBuilding.current.value;
		let message = this.refMessage.current.value;
		let regularID = this.refRegularID.current.value;
		let regularMemberID = this.refRegularMemberID.current.value;

		this.updateSpreadMessage(type, buildingGroupID, buildingID, this.state.messageType, message, regularID, regularMemberID);

		this.showConfirmDialog("확인", ["적용되었습니다."], null, null);
	}

	onClickAllMsgSave = () => {
		if (this.props.spreadMessages === null || this.props.spreadMessages === undefined)
			return;

		// 해당 메시지 및 전파인원 
		//let type = this.refFacilityType.current.value;
		let buildingGroupID = this.refBuildingGroup.current.value;
		let buildingID = this.refBuilding.current.value;
		let regularID = this.refRegularID.current.value;
		let regularMemberID = this.refRegularMemberID.current.value;
		let message = this.refMessage.current.value;

		// 화재 초기화
		let type = SettingsResource.facilityType.Fire;
		this.updateSpreadMessage(type, "", "", this.state.messageType, message, regularID, regularMemberID);

		if (this.props.buildingGroupList !== null && this.props.buildingGroupList !== undefined) {
			const buildingGroupList = this.props.buildingGroupList;

			for (let i = 0; i < buildingGroupList.length; i++) {
				let buildingGroup = buildingGroupList[i];

				this.updateSpreadMessage(type, buildingGroup.id, "", this.state.messageType, message, regularID, regularMemberID);

				for (let j = 0; j < buildingGroup.buildingDatas.length; j++) {
					let building = buildingGroup.buildingDatas[j];

					this.updateSpreadMessage(type, buildingGroup.id, building.id, this.state.messageType, message, regularID, regularMemberID);
				}
			}
		}

		// 누출 초기화
		type = SettingsResource.facilityType.PSM;
		this.updateSpreadMessage(type, "", "", this.state.messageType, message, regularID, regularMemberID);

		if (this.props.buildingGroupList !== null && this.props.buildingGroupList !== undefined) {
			const buildingGroupList = this.props.buildingGroupList;

			for (let i = 0; i < buildingGroupList.length; i++) {
				let buildingGroup = buildingGroupList[i];

				this.updateSpreadMessage(type, buildingGroup.id, "", this.state.messageType, message, regularID, regularMemberID);

				for (let j = 0; j < buildingGroup.buildingDatas.length; j++) {
					let building = buildingGroup.buildingDatas[j];

					this.updateSpreadMessage(type, buildingGroup.id, building.id, this.state.messageType, message, regularID, regularMemberID);
				}
			}
		}

		// ETC 초기화
		type = SettingsResource.facilityType.ETC;
		this.updateSpreadMessage(type, "", "", this.state.messageType, message, regularID, regularMemberID);

		if (this.props.buildingGroupList !== null && this.props.buildingGroupList !== undefined) {
			const buildingGroupList = this.props.buildingGroupList;

			for (let i = 0; i < buildingGroupList.length; i++) {
				let buildingGroup = buildingGroupList[i];

				this.updateSpreadMessage(type, buildingGroup.id, "", this.state.messageType, message, regularID, regularMemberID);

				for (let j = 0; j < buildingGroup.buildingDatas.length; j++) {
					let building = buildingGroup.buildingDatas[j];

					this.updateSpreadMessage(type, buildingGroup.id, building.id, this.state.messageType, message, regularID, regularMemberID);
				}
			}
		}

		// 지능형 영상 초기화
		type = SettingsResource.facilityType.SVMS;
		this.updateSpreadMessage(type, "", "", this.state.messageType, message, regularID, regularMemberID);

		if (this.props.buildingGroupList !== null && this.props.buildingGroupList !== undefined) {
			const buildingGroupList = this.props.buildingGroupList;

			for (let i = 0; i < buildingGroupList.length; i++) {
				let buildingGroup = buildingGroupList[i];

				this.updateSpreadMessage(type, buildingGroup.id, "", this.state.messageType, message, regularID, regularMemberID);

				for (let j = 0; j < buildingGroup.buildingDatas.length; j++) {
					let building = buildingGroup.buildingDatas[j];

					this.updateSpreadMessage(type, buildingGroup.id, building.id, this.state.messageType, message, regularID, regularMemberID);
				}
			}
		}

		this.showConfirmDialog("확인", ["모든 재난유형 동일하게 적용되었습니다. "], null, null);
	}

	onClickSelectReceiver = () => {
		const regularID = this.refRegularID.current.value;
		const regularMemberID = this.refRegularMemberID.current.value;

		const facilityType = this.refFacilityType.current.value;
		let facilityTypeName = "";

		if (facilityType === SettingsResource.facilityType.Fire.toString())
			facilityTypeName = SettingsResource.ID.facilityType.Fire;
		else if (facilityType === SettingsResource.facilityType.PSM.toString())
			facilityTypeName = SettingsResource.ID.facilityType.PSM;
		else if (facilityType === SettingsResource.facilityType.ETC.toString())
			facilityTypeName = SettingsResource.ID.facilityType.ETC;
		else if (facilityType === SettingsResource.facilityType.SVMS.toString())
			facilityTypeName = SettingsResource.ID.facilityType.SVMS;

		const buildingGroup = this.refBuildingGroup.current.value;
		let buildingDatas = null;
		let buildingGroupName = "";

		if (buildingGroup !== null && buildingGroup !== undefined && buildingGroup !== "") {
			const buildingGroupList = this.props.buildingGroupList;

			for (let i = 0; i < buildingGroupList.length; i++) {
				let buildingGroupData = buildingGroupList[i];

				if (buildingGroup === buildingGroupData.id.toString()) {
					buildingGroupName = buildingGroupData.displayText;

					buildingDatas = buildingGroupData.buildingDatas;
					break;
				}
			}
		} 


		const building = this.refBuilding.current.value;
		let buildingName = "";

		if (building !== null && building !== undefined && building !== "") {

			for (let i = 0; i < buildingDatas.length; i++) {
				const buildingData = buildingDatas[i];

				if (building === buildingData.id.toString()) {
					buildingName = buildingData.displayText;
					break;
                }
            }
		} 

		this.setState({ selectPopupOnOff: true, selectFacilityType: facilityTypeName, selectBuildingGroup: buildingGroupName, selectBuilding: buildingName, selectRegularID: regularID, selectRegularMemberID: regularMemberID });
		
	}

	onClickConfirm = (regularID, regularMemberID) => {
		//const regularID = this.refRegularID.current.value;
		//const regularMemberID = this.refRegularMemberID.current.value;
		this.setSpreadReceiver(regularID, regularMemberID);

		this.setState({ selectPopupOnOff: false });
	}

	setSpreadReceiver = (regularID, regularMemberID) => {
		let receiver = "";

		if (regularID !== null && regularID !== undefined && regularID !== "") {
			let arrRegular = regularID.split(",");

			// 팀명 조회
			if (this.props.teams !== null && this.props.teams !== undefined && arrRegular.length > 0) {
				let teams = this.props.teams;

				for (let i = 0; i < arrRegular.length; i++) {
					let regular = arrRegular[i];

					for (let j = 0; j < teams.length; j++) {
						let team = teams[j];

						if (team.id.toString() === regular) {
							if (receiver === "")
								receiver += team.teamName;
							else
								receiver = receiver + ", " + team.teamName;

							break;
						}
					}
				}
			}
		}

		if (regularMemberID !== null && regularMemberID !== undefined && regularMemberID !== "") {
			let arrMembers = regularMemberID.split(",");

			// 멤버 조회
			if (this.props.members !== null && this.props.members !== undefined && arrMembers.length > 0) {
				let members = this.props.members;

				for (let i = 0; i < arrMembers.length; i++) {
					let arrMember = arrMembers[i];

					for (let j = 0; j < members.length; j++) {
						let member = members[j];

						if (member.ID.toString() === arrMember) {
							if (receiver === "")
								receiver += member.MemberName;
							else
								receiver = receiver + ", " + member.MemberName;

							break;
						}
					}

				}
			}
		}

		this.state.receiver = receiver;
		//this.refMessage.current.value = message;
		this.refRegularID.current.value = regularID;
		this.refRegularMemberID.current.value = regularMemberID;
    }

	onClickClose = () => {
		this.setState({ selectPopupOnOff: false });
    }

	displaySelect = () => {
		if (this.state.selectPopupOnOff === true) {
			return (
				<SelectReceiver
					onClickConfirm={this.onClickConfirm}
					onClickClose={this.onClickClose}
					teamTreeDatas={this.props.teamTreeDatas}
					teams={this.props.teams}
					members={this.props.members}
					facilityType={this.state.selectFacilityType}
					buildingGroup={this.state.selectBuildingGroup}
					building={this.state.selectBuilding}
					regularID={this.state.selectRegularID}
					regularMemberID={this.state.selectRegularMemberID}				/>
				);
		} else {
			return (<></>);
        }
	}

	onBlurCheck = (e) => {
		let target = e;

		let type = this.refFacilityType.current.value;
		let buildingGroupID = this.refBuildingGroup.current.value;
		let buildingID = this.refBuilding.current.value;

		let dataType = parseInt(type);
		let dataBuildingGroupID = null;
		let dataBuildingID = null;

		if (buildingGroupID !== null && buildingGroupID !== undefined && buildingGroupID !== "")
			dataBuildingGroupID = parseInt(buildingGroupID);

		if (buildingID !== null && buildingID !== undefined && buildingID !== "")
			dataBuildingID = parseInt(buildingID);

		const spreadMessages = this.props.spreadMessages;
		let chk = false;

		for (let i = 0; i < spreadMessages.length; i++) {
			let spreadMessage = spreadMessages[i];

			if (dataType === spreadMessage.facilityType &&
				spreadMessage.buildingGroupID === dataBuildingGroupID &&
				spreadMessage.buildingID === dataBuildingID &&
				spreadMessage.messageType === this.state.messageType) {

				chk = true;
				break;
			}
		}

		if (chk === false) {
			let spreadData = new Object();

			spreadData = {
				id: -1,
				buildingGroupID: dataBuildingGroupID,
				buildingID: dataBuildingID,
				facilityType: dataType,
				message: target.value,
				messageType: this.state.messageType,
				regularID: "",
				regularMemberID: ""
			}

			spreadMessages.push(spreadData);
		}

	}

	render() {

		return (
			<>
				<div className={newStyles.stgList}>
					<div className={newStyles.stgName}>
						<h5>문자/이메일발송설정</h5>
						<span className={newStyles.stgTltp} data-tooltip="문자/이메일 발송 옵션을 설정 합니다."></span>
					</div>

					<div className={newStyles.stgmWrap}>
						<ul className={newStyles.stgmLft}>
							<li>
								<select ref={this.refFacilityType} name="" id="" className={newStyles.dslSel} onChange={this.onChangeFacilityType}>
									<option value={SettingResource.facilityType.Fire}>화재</option>
									<option value={SettingResource.facilityType.PSM}>누출</option>
									<option value={SettingResource.facilityType.ETC}>기타</option>
									<option value={SettingResource.facilityType.SVMS}>지능형 영상</option>
								</select>
							</li>
							<li>
								<select ref={this.refBuildingGroup} name="" id="" className={newStyles.dslSel} onChange={this.onChangeBuildingGroup}>
									<option value="">전체</option>
									{this.setBuildingGroupUI()}
								</select>
							</li>
							<li>
								<select ref={this.refBuilding} name="" id="" className={newStyles.dslSel} onChange={this.onChangeBuilding}>
									<option value="">전체</option>
									{this.state.buildingList}
								</select>
							</li>
							<li><a onClick={this.onClickSelectReceiver}>전파 대상자 지정</a></li>
						</ul>
						<div className={newStyles.stgmCen}>
							<div className={newStyles.stgmCont}>
								<ul className={newStyles.stgmTab}>
									<li><a id="sms" onClick={() => this.onClickMessageTyep(SettingResource.messageType.sms)} className={newStyles.on}>문자</a></li>
									<li><a id="email" onClick={() => this.onClickMessageTyep(SettingResource.messageType.email)}>이메일</a></li>
								</ul>
								<div className={newStyles.stgmDtl} id="stgmsms" style={{ "display": "block" }}>
									<p className={newStyles.stgmDft}>기본 문구가 표출되는 영역(수정불가) 기본 문구가 표출되는 영역(수정불가) 기본 문구가 표출되는 영역(수정불가)</p>
									<textarea ref={this.refMessage} className={newStyles.stgmTxt + " scrollbar"} ></textarea>
									<div className={newStyles.stgmTo}>
										<span>수신자 : </span>
										<p>{this.state.receiver}</p>
									</div>
								</div>
								{/*
								<div class={newStyles.stgmDtl} id="stgmmail">
									<p class={newStyles.stgmDft}>기본 문구가 표출되는 영역(수정불가) 기본 문구가 표출되는 영역(수정불가) 기본 문구가 표출되는 영역(수정불가)</p>
									<textarea class={newStyles.stgmTxt + " scrollbar"}></textarea>
									<div class={newStyles.stgmTo}>
										<span>수신자 : </span>
										<p>전기팀, 설비팀, 지원팀</p>
									</div>
								</div>
								*/}
								<input ref={this.refRegularID} type="text" className={settings.hidden} />
								<input ref={this.refRegularMemberID} type="text" className={settings.hidden} />
							</div>
						</div>
						<div className={newStyles.stgmRht}>
							<h5>특수문자설명</h5>
							<ul>
								<li><span> &#123; location &#125; : </span>재난발생위치</li>
								<li><span> &#123; date &#125; : </span>재난발생시간</li>
							</ul>
						</div>
						<ul className={newStyles.stgmBtn}>
							<li><a onClick={this.onClickAllMsgSave}>모든 재난유형 동일하게 적용하기</a></li>
							{/*<li><a href="#">임시저장</a></li>*/}
							<li><a onClick={this.onClickMsgSave}>적용</a></li>
						</ul>
					</div>
				</div>

				{
					/* alert창 대신 사용 */
					this.state.confirmMessage.visible &&
					<ConfirmDialog title={this.state.confirmMessage.title} messages={this.state.confirmMessage.messages} buttons={this.state.confirmMessage.buttons} onClose={this.state.confirmMessage.onClose} onClickButton={this.state.confirmMessage.onClickButton} />
				}

				{this.displaySelect()}
			</>
		);
	}
}

class DetectionTab extends Component {
	constructor(props) {
		super(props);

		this.state = {

		}

		this.props = props;

		this.refTimeTrem = React.createRef();

		//this.refStgAlm01 = React.createRef();
		//this.refStgAlm02 = React.createRef();
		//this.refStgAlm03 = React.createRef();

		this.refTimeUnit = React.createRef();

		this.refUseReceiveFire = React.createRef();
		this.refUseReceivePSM = React.createRef();
		this.refUseReceiveETC = React.createRef();
		this.refUseReceiveSVMS = React.createRef();

		//this.eventTermDay = React.createRef();
		//this.eventTermWeek = React.createRef();
		//this.eventTermMonth = React.createRef();

		//this.UseScreenMoveTrue = React.createRef();
		//this.UseScreenMoveFalse = React.createRef();

		//this.refExeCautionSOP = React.createRef();
		//this.refExeAlartSOP = React.createRef();
		//this.refExeSeriousSOP = React.createRef();

		//this.refUseTrainingMode = React.createRef();
		//this.refUseWaterMark = React.createRef();
		//this.refUseHeadMessage = React.createRef();
		//this.refHeadMessage = React.createRef();

		this.refDisAlm01 = React.createRef();
		this.refDisAlm02 = React.createRef();
		this.refDisAlm03 = React.createRef();
		this.refDisAlm04 = React.createRef();

		this.refUseBroadcast = React.createRef();
		this.refNotUseBroadcast = React.createRef();

		this.refUsePoiFocusOn = React.createRef();
		this.refUsePoiFocusOff = React.createRef();
	}

	componentDidMount() {
		//this.initReAlarmSet();
		this.initUseReceive();
		//this.initEventInfoDisplayTerm();
		//this.initUseScreenMove();
		this.initExeSOPSet();
		//this.initTrainingData();
		this.initMoveDisplayAlarm();

		// .TODO: 고도화 내용으로 임시 주석처리
		//this.initUsePoiFocus();

		this.setAlarmBroadcast();
	}

	initMoveDisplayAlarm() {
		if (this.props.settings === null || this.props.settings === undefined)
			return;

		let moveDisplayAlarm = this.props.settings.moveDisplayAlarm;

		// 값에 따라서 클릭 
		if (moveDisplayAlarm === SettingsResource.moveDisplayAlarm.currentDisplay) {
			this.refDisAlm01.current.click();
		} else if (moveDisplayAlarm === SettingsResource.moveDisplayAlarm.moveAlarm) {
			//this.refDisAlm02.current.click();
		} else if (moveDisplayAlarm === SettingsResource.moveDisplayAlarm.firstAlarm) {
			this.refDisAlm03.current.click();
		} else if (moveDisplayAlarm === SettingsResource.moveDisplayAlarm.lastAlarm) {
			this.refDisAlm04.current.click();
		}
	}

	initUsePoiFocus() {
		if (this.props.settings === null || this.props.settings === undefined)
			return;

		let usePoiFocus = this.props.settings.usePoiFocus;

		if (usePoiFocus === SettingsResource.usePoiFocus.on) {
			this.refUsePoiFocusOn.current.click();
		} else if (usePoiFocus === SettingsResource.usePoiFocus.off) {
			this.refUsePoiFocusOff.current.click();
        }
    }

	/*
	initTrainingData() {
		if (this.props.settings === null || this.props.settings === undefined)
			return;

		let useTrainingMode = this.props.settings.useTrainingMode;
		let useWaterMark = this.props.settings.useWaterMark;
		let useHeadMessage = this.props.settings.useHeadMessage;

		if (useTrainingMode === "true") {
			this.refUseTrainingMode.current.click();
		}

		if (useWaterMark === "true") {
			this.refUseWaterMark.current.click();
		}

		if (useHeadMessage !== "false") {
			this.refHeadMessage.current.value = useHeadMessage;
			this.refUseHeadMessage.current.click();
		} 
	}
	*/

	onChangeHeadMessage = (e) => {
		if (this.props.settings === null || this.props.settings === undefined)
			return;

		let value = e.value;
		this.props.settings.useHeadMessage = value;

    }

	onChangeUseTrainingMode = (e) => {
		if (this.props.settings === null || this.props.settings === undefined)
			return;

		let value = e.checked.toString();
		this.props.settings.useTrainingMode = value;
	}

	onChangeUseWaterMark = (e) => {
		if (this.props.settings === null || this.props.settings === undefined)
			return;

		let value = e.checked.toString();
		this.props.settings.useWaterMark = value;
	}

	onChangeUseHeadMessage = (e) => {
		if (this.props.settings === null || this.props.settings === undefined)
			return;

		let value = e.checked.toString();

		if (value === "false") {
			this.props.settings.useHeadMessage = value;
			$('#headMessage').attr("disabled", true);
		} else {
			this.props.settings.useHeadMessage = this.refHeadMessage.current.value;
			$('#headMessage').attr("disabled", false);
        }
			
	}

	initExeSOPSet() {
		if (this.props.settings === null || this.props.settings === undefined)
			return;

		let exeCautionSOP = this.props.settings.exeCautionSOP;
		let exeAlartSOP = this.props.settings.exeAlartSOP;
		let exeSeriousSOP = this.props.settings.exeSeriousSOP;

		//this.refExeCautionSOP.current.value = exeCautionSOP;
		//this.refExeAlartSOP.current.value = exeAlartSOP;
		//this.refExeSeriousSOP.current.value = exeSeriousSOP;
	}
	/*
	onChangeExeCautionSOP = () => {
		if (this.props.settings === null || this.props.settings === undefined)
			return;

		this.props.settings.exeCautionSOP = this.refExeCautionSOP.current.value;
	}

	onChangeExeAlartSOP = () => {
		if (this.props.settings === null || this.props.settings === undefined)
			return;

		this.props.settings.exeAlartSOP = this.refExeAlartSOP.current.value;
	}

	onChangeExeSeriousSOP = () => {
		if (this.props.settings === null || this.props.settings === undefined)
			return;

		this.props.settings.exeSeriousSOP = this.refExeSeriousSOP.current.value;
	}
	*/

	/*
	initUseScreenMove() {
		if (this.props.settings === null || this.props.settings === undefined)
			return;

		let useScreenMove = this.props.settings.useScreenMove;

		if (useScreenMove === null || useScreenMove === undefined || useScreenMove === "")
			return;

		if (useScreenMove === "true") {
			this.UseScreenMoveTrue.current.click();
		} else if (useScreenMove === "false") {
			this.UseScreenMoveFalse.current.click();
		}
	}
	*/

	onChangeUseScreenMove = (mode) => {

		if (this.props.settings === null || this.props.settings === undefined)
			return;

		this.props.settings.useScreenMove = mode;
    }
	/*
	initEventInfoDisplayTerm() {
		if (this.props.settings === null || this.props.settings === undefined) 
			return;

		let eventTerm = this.props.settings.eventInfoDisplayTerm;

		if (eventTerm === null || eventTerm === undefined || eventTerm === "")
			return;

		if (eventTerm === SettingResource.eventInfoDisplayTerm.day) {
			this.eventTermDay.current.click();
		} else if (eventTerm === SettingResource.eventInfoDisplayTerm.week) {
			this.eventTermWeek.current.click();
		} else if (eventTerm === SettingResource.eventInfoDisplayTerm.month) {
			this.eventTermMonth.current.click();
		}
    }
	*/

	initUseReceive() {
		let useReceiveFire = "true";
		let useReceivePSM = "true";
		let useReceiveETC = "true";
		let useReceiveSVMS = "true";

		if (this.props.settings !== null && this.props.settings !== undefined) {
			let fireData = this.props.settings.useReceiveFire;
			let psmData = this.props.settings.useReceivePSM;
			let etcData = this.props.settings.useReceiveETC;
			let svmsData = this.props.settings.useReceiveSVMS;

			if (fireData !== "" && fireData !== null && fireData !== undefined) {
				useReceiveFire = fireData;
			}

			if (psmData !== "" && psmData !== null && psmData !== undefined) {
				useReceivePSM = psmData;
			}

			if (etcData !== "" && etcData !== null && etcData !== undefined) {
				useReceiveETC = etcData;
			}

			if (svmsData !== "" && svmsData !== null && svmsData !== undefined) {
				useReceiveSVMS = svmsData;
			}
		}

		if (useReceiveFire === "true")
			this.refUseReceiveFire.current.click();

		if (useReceivePSM === "true")
			this.refUseReceivePSM.current.click();

		if (useReceiveETC === "true")
			this.refUseReceiveETC.current.click();

		if (useReceiveSVMS === "true")
			this.refUseReceiveSVMS.current.click();
	}

	onChangeReceiveFire = (e) => {
		if (this.props.settings === null || this.props.settings === undefined)
			return;

		let value = e.checked.toString();

		if (this.props.settings.useReceiveFire !== null && this.props.settings.useReceiveFire !== undefined)
			this.props.settings.useReceiveFire = value;
	}

	onChangeReceivePSM = (e) => {
		if (this.props.settings === null || this.props.settings === undefined)
			return;

		let value = e.checked.toString();

		if (this.props.settings.useReceivePSM !== null && this.props.settings.useReceivePSM !== undefined)
			this.props.settings.useReceivePSM = value;
	}

	onChangeReceiveETC = (e) => {
		if (this.props.settings === null || this.props.settings === undefined)
			return;

		let value = e.checked.toString();

		if (this.props.settings.useReceiveETC !== null && this.props.settings.useReceiveETC !== undefined)
			this.props.settings.useReceiveETC = value;
	}

	onChangeReceiveSVMS = (e) => {
		if (this.props.settings === null || this.props.settings === undefined)
			return;

		let value = e.checked.toString();

		if (this.props.settings.useReceiveSVMS !== null && this.props.settings.useReceiveSVMS !== undefined)
			this.props.settings.useReceiveSVMS = value;
	}

	selectReAlarm = (mode) => {
		if (mode === SettingResource.reAlarm.ReAlarm) {
			$('#timeTrem').attr("disabled", true);
			$('#timeUnit').attr("disabled", true);

			if (this.props.settings !== null && this.props.settings !== undefined)
				this.props.settings.reAlarm = mode + ",0,0";
		} else if (mode === SettingResource.reAlarm.NoAlarmTerm) {
			$('#timeTrem').attr("disabled", false);
			$('#timeUnit').attr("disabled", false);

			let timeTrem = $('#timeTrem').val();
			let timeUnit = $('#timeUnit').val();

			if (this.props.settings !== null && this.props.settings !== undefined)
				this.props.settings.reAlarm = mode + "," + timeUnit + "," + timeTrem;
		} else if (mode === SettingResource.reAlarm.NoAlarm) {
			$('#timeTrem').attr("disabled", true);
			$('#timeUnit').attr("disabled", true);

			if (this.props.settings !== null && this.props.settings !== undefined)
				this.props.settings.reAlarm = mode + ",0,0";
		}
    }
	/*
	initReAlarmSet() {
		let reAlarmValue = "0,0,0";

		if (this.props.settings !== null && this.props.settings !== undefined) {
			let data = this.props.settings.reAlarm;

			if (data !== "" && data !== null && data !== undefined) {
				reAlarmValue = data;
            }
		}

		let arrReAlarm = reAlarmValue.split(",");	// 첫번째: 재알람/* 후 재알람/미알람, 두번째: 초/분/시 단위, 세번째: 시간값

		if (arrReAlarm.length !== 3) {
			reAlarmValue = "0,0,0";
			arrReAlarm = reAlarmValue.split(",");
		}

		if (arrReAlarm[0] === SettingResource.reAlarm.ReAlarm) {
			this.refStgAlm01.current.click();
		} else if (arrReAlarm[0] === SettingResource.reAlarm.NoAlarmTerm) {
			this.refStgAlm02.current.click();
		} else if (arrReAlarm[0] === SettingResource.reAlarm.NoAlarm) {
			this.refStgAlm03.current.click();
		}

		this.refTimeUnit.current.value = arrReAlarm[1];
		this.refTimeTrem.current.value = arrReAlarm[2];
	}
	*/

	onChangeNumCheck = (e) => {
		let value = e.target.value;
		let inputValue = value.replace(/[^0-9\b ]/g, '');

		this.refTimeTrem.current.value = inputValue;
	}

	onBlurCheck = (e) => {
		let value = e.value;
		/*
		if (value === "")
			value = "0";

		let reAlarmValue = "0,0,0";

		if (this.props.settings !== null && this.props.settings !== undefined) {
			let data = this.props.settings.reAlarm;

			if (data !== "" && data !== null && data !== undefined) {
				reAlarmValue = data;
			}
		}

		let arrReAlarm = reAlarmValue.split(",");	// 첫번째: 재알람/* 후 재알람/미알람, 두번째: 초/분/시 단위, 세번째: 시간값

		if (arrReAlarm.length !== 3) {
			reAlarmValue = "0,0,0";
			arrReAlarm = reAlarmValue.split(",");
		}

		let reAlarm = arrReAlarm[0] + "," + arrReAlarm[1] + "," + value;
		this.props.settings.reAlarm = reAlarm;
		*/
	}

	onChangeTimeUnit = (e) => {
		let value = e.value;

		let reAlarmValue = "0,0,0";

		if (this.props.settings !== null && this.props.settings !== undefined) {
			let data = this.props.settings.reAlarm;

			if (data !== "" && data !== null && data !== undefined) {
				reAlarmValue = data;
			}
		}

		let arrReAlarm = reAlarmValue.split(",");	// 첫번째: 재알람/* 후 재알람/미알람, 두번째: 초/분/시 단위, 세번째: 시간값

		if (arrReAlarm.length !== 3) {
			reAlarmValue = "0,0,0";
			arrReAlarm = reAlarmValue.split(",");
		}

		let reAlarm = arrReAlarm[0] + "," + value + "," + arrReAlarm[2];
		this.props.settings.reAlarm = reAlarm;
	}

	onChangeEventInfoDisplayTerm = (mode) => {

		if (this.props.settings === null || this.props.settings === undefined)
			return;

		this.props.settings.eventInfoDisplayTerm = mode;
    }

	selectDisplayAlarm = (mode) => {
		if (mode === null || mode === undefined)
			return;

		if (mode === SettingsResource.moveDisplayAlarm.currentDisplay || 
			mode === SettingsResource.moveDisplayAlarm.moveAlarm ||
			mode === SettingsResource.moveDisplayAlarm.firstAlarm ||
			mode === SettingsResource.moveDisplayAlarm.lastAlarm) {
			this.props.settings.moveDisplayAlarm = mode;
		} 
	}

	selectUsePoiFocus = (use) => {
		if (use === null || use === undefined)
			return;

		if (use === SettingsResource.usePoiFocus.on ||
			use === SettingsResource.usePoiFocus.off) {
			this.props.settings.usePoiFocus = use;
        }
    }

	onChangeAlarmBroadcast = (e) => {
		let use;

		if (e.target === this.refUseBroadcast.current) {
			use = this.refUseBroadcast.current.checked;
		}
		else if (e.target === this.refNotUseBroadcast.current) {
			use = !this.refNotUseBroadcast.current.checked;
		}
		else {
			return;
		}

		this.props.settings.useAlarmBroadcast = use.toString();
		this.setAlarmBroadcast();
	}

	setAlarmBroadcast() {
		let useAlarmBroadcast = this.props.settings.useAlarmBroadcast;

		if (useAlarmBroadcast) {
			useAlarmBroadcast = useAlarmBroadcast.toLowerCase();

			if (useAlarmBroadcast === "1" || useAlarmBroadcast === "true") {
				this.refUseBroadcast.current.checked = true;
				this.refNotUseBroadcast.current.checked = false;
			}
			else {
				this.refUseBroadcast.current.checked = false;
				this.refNotUseBroadcast.current.checked = true;
			}
		}
	}

	render() {
		//const reAlarmUI = this.reAlarmUI();

		return (
			<>
				<div className={newStyles.stgList}>
					{/*
					<div className={newStyles.stgName}>
						<h5>오작동 처리 센서의 재알람 기준(기본값)</h5>
						<span className={newStyles.stgTltp} data-tooltip="오작동 처리 센서의 재알람 기준을 설정 합니다."></span>
						<ul className={newStyles.stgAlm}>
							<li>
								<input ref={this.refStgAlm01} type="radio" name="stgAlm" id="stgAlm01" onChange={() => this.selectReAlarm(SettingResource.reAlarm.ReAlarm)} />
								<label for="stgAlm01">모두 재알람</label>
							</li>
							<li>
								<input ref={this.refStgAlm02} type="radio" name="stgAlm" id="stgAlm02" onChange={() => this.selectReAlarm(SettingResource.reAlarm.NoAlarmTerm)} />
								<input ref={this.refTimeTrem} type="text" className={settings.settingInput} name="" id="timeTrem" onChange={this.onChangeNumCheck} onBlur={(e) => this.onBlurCheck(e.target)} />
								<select ref={this.refTimeUnit} name="" id="timeUnit" className={newStyles.dslSel} onChange={(e) => this.onChangeTimeUnit(e.target)} >
									<option value={SettingResource.timeUnit.second} >초</option>
									<option value={SettingResource.timeUnit.minute} >분</option>
									<option value={SettingResource.timeUnit.hour} >시</option>
								</select>
								<label for="stgAlm02">동안 미알람</label>
							</li>
							<li>
								<input ref={this.refStgAlm03} type="radio" name="stgAlm" id="stgAlm03" onChange={() => this.selectReAlarm(SettingResource.reAlarm.NoAlarm)} />
								<label for="stgAlm03">계속 미알람</label>
							</li>
						</ul>
					</div>
					*/}

					{/*<div className={newStyles.stgHalf}>*/}
					<div className={newStyles.stgName}>
						<div>
							<div className={newStyles.stgName + " " + newStyles.bdNon + " " + newDefaults.mb0}>
								<h5>유형별 알람 설정</h5>
								<span className={newStyles.stgTltp} data-tooltip="유형별 알람을 설정 합니다."></span>
							</div>
							<ul className={newStyles.stgAlt}>
								<li><p>화재</p><input ref={this.refUseReceiveFire} type="checkbox" name="stgAlt" id="stgAlt01" onChange={(e) => this.onChangeReceiveFire(e.target)} /><label for="stgAlt01">수신</label></li>
								<li><p>누출</p><input ref={this.refUseReceivePSM} type="checkbox" name="stgAlt" id="stgAlt02" onChange={(e) => this.onChangeReceivePSM(e.target)} /><label for="stgAlt02">수신</label></li>
								<li><p>기타</p><input ref={this.refUseReceiveETC} type="checkbox" name="stgAlt" id="stgAlt03" onChange={(e) => this.onChangeReceiveETC(e.target)} /><label for="stgAlt03">수신</label></li>
								<li><p>SVMS</p><input ref={this.refUseReceiveSVMS} type="checkbox" name="stgAlt" id="stgAlt04" onChange={(e) => this.onChangeReceiveSVMS(e.target)} /><label for="stgAlt04">수신</label></li>
							</ul>
						</div>
						{/*
						<div>
							<div className={newStyles.stgName + " " + newStyles.bdNon + " " + newDefaults.mb0}>
								<h5>SOP 실행</h5>
								<span className={newStyles.stgTltp} data-tooltip="SOP 실행을 설정 합니다."></span>
							</div>
							<div className={newStyles.stgmTo + " " + newDefaults.mt0}>
								<span>주의 : </span>
								<select ref={this.refExeCautionSOP} name="" id="" className={newStyles.dslSel + " " + newDefaults.h28} onChange={this.onChangeExeCautionSOP}>
									<option value={SettingResource.ExeSOPMode.false}>센서 감지시 SOP 자동실행 안함</option>
									<option value={SettingResource.ExeSOPMode.exe}>센서 감지시 SOP 자동 열기 및 실행</option>
								</select>
							</div>
							<div className={newStyles.stgmTo + " " + newDefaults.mt5}>
								<span>경계 : </span>
								<select ref={this.refExeAlartSOP} name="" id="" className={newStyles.dslSel + " " + newDefaults.h28} onChange={this.onChangeExeAlartSOP}>
									<option value={SettingResource.ExeSOPMode.false} >센서 감지시 SOP 자동실행 안함</option>
									<option value={SettingResource.ExeSOPMode.exe} >센서 감지시 SOP 자동 열기 및 실행</option>
								</select>
							</div>
							<div className={newStyles.stgmTo + " " + newDefaults.mt5}>
								<span>심각 : </span>
								<select ref={this.refExeSeriousSOP} name="" id="" className={newStyles.dslSel + " " + newDefaults.h28} onChange={this.onChangeExeSeriousSOP}>
									<option value={SettingResource.ExeSOPMode.false} >센서 감지시 SOP 자동실행 안함</option>
									<option value={SettingResource.ExeSOPMode.exe} >센서 감지시 SOP 자동 열기 및 실행</option>
								</select>
							</div>
						</div>
						*/}
					</div>

					<div className={newStyles.stgName}>
						<h5>이벤트 자동 화면 전환</h5>
						<span className={newStyles.stgTltp} data-tooltip="이벤트시 자동 화면 전환 여부를 설정 합니다."></span>
						<ul className={newStyles.stgAlm + " " + settings.settingRadio}>
							<li>
								<input ref={this.refDisAlm01} type="radio" name="disAlm" id="disAlm01" onChange={() => this.selectDisplayAlarm(SettingResource.moveDisplayAlarm.currentDisplay)} />
								<label for="disAlm01">현재화면 유지</label>
							</li>
							{/*
							<li>
								<input ref={this.refDisAlm02} type="radio" name="disAlm" id="disAlm02" onChange={() => this.selectDisplayAlarm(SettingResource.moveDisplayAlarm.moveAlarm)} />
								<label for="disAlm02">알람마다 화면 이동</label>
							</li>
							*/}
							<li>
								<input ref={this.refDisAlm03} type="radio" name="disAlm" id="disAlm03" onChange={() => this.selectDisplayAlarm(SettingResource.moveDisplayAlarm.firstAlarm)} />
								<label for="disAlm03">첫번째 알람 화면으로 이동</label>
							</li>
							<li>
								<input ref={this.refDisAlm04} type="radio" name="disAlm" id="disAlm04" onChange={() => this.selectDisplayAlarm(SettingResource.moveDisplayAlarm.lastAlarm)} />
								<label for="disAlm04">마지막 알람 화면으로 이동</label>
							</li>
						</ul>
					</div>

					<div className={newStyles.stgName}>
						<h5>알람 방송</h5>
						<span className={newStyles.stgTltp} data-tooltip="알람 발생시 자동으로 재난방송을 실시합니다."></span>
						<span className={settings.white}>&nbsp;&nbsp;사용 &nbsp;&nbsp;</span><input ref={this.refUseBroadcast} type="checkbox" name="" id="" onChange={this.onChangeAlarmBroadcast} />
						<span className={settings.white}>&nbsp;&nbsp;사용안함 &nbsp;&nbsp;</span><input ref={this.refNotUseBroadcast} type="checkbox" name="" id="" onChange={this.onChangeAlarmBroadcast} />
					</div>

					{/* .TODO: 고도화 내용으로 임시 주석처리
					<div className={newStyles.stgName + " " + settings.settingRadio}>
						<h5>이벤트 포커싱</h5>
						<span className={newStyles.stgTltp} data-tooltip="이벤트 관련 POI에 카메라 포커싱 여부를 설정 합니다"></span>

						<span className={settings.white}>&nbsp;&nbsp;사용 &nbsp;&nbsp;</span>
						<input type="radio" ref={this.refUsePoiFocusOn} name="usePOIFocus" id="usePoiFocusOn" onChange={() => this.selectUsePoiFocus(SettingResource.usePoiFocus.on)} />
						
						<span className={settings.white}>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 사용안함 &nbsp;&nbsp;</span>
						<input type="radio" ref={this.refUsePoiFocusOff} name="usePOIFocus" id="usePoiFocusOff" onChange={() => this.selectUsePoiFocus(SettingResource.usePoiFocus.off)} />
					</div>
					*/}

					{/*
					<div className={newStyles.stgHalf}>
						<div>
							<div className={newStyles.stgName + " " + newStyles.bdNon + " " + newDefaults.mb0 + " " + newDefaults.pb0}>
								<h5>이벤트 정보창 표출 리스트 기간설정</h5>
								<span className={newStyles.stgTltp} data-tooltip="이벤트 정보창 표출 리스트 기간을 설정 합니다."></span>
							</div>
							<ul className={newStyles.stgAlm + " " + newDefaults.mt0 + " " + newDefaults.mb20}>
								<li><input ref={this.eventTermDay} type="radio" name="stgDate" id="stgDate01" onChange={() => this.onChangeEventInfoDisplayTerm(SettingResource.eventInfoDisplayTerm.day)} /><label for="stgDate01">하루</label></li>
								<li><input ref={this.eventTermWeek} type="radio" name="stgDate" id="stgDate02" onChange={() => this.onChangeEventInfoDisplayTerm(SettingResource.eventInfoDisplayTerm.week)} /><label for="stgDate02">일주일</label></li>
								<li><input ref={this.eventTermMonth} type="radio" name="stgDate" id="stgDate03" onChange={() => this.onChangeEventInfoDisplayTerm(SettingResource.eventInfoDisplayTerm.month)} /><label for="stgDate03">한달</label></li>
							</ul>
							<div className={newStyles.stgName + " " + newStyles.bdNon + " " + newDefaults.mb0 + " " + newDefaults.pb0}>
								<h5>종료/오탐지시 화면 이동</h5>
								<span className={newStyles.stgTltp} data-tooltip="종료/오탐지시 화면 이동을 설정 합니다."></span>
							</div>
							<ul className={newStyles.stgAlm + " " + newDefaults.mt0}>
								<li><input ref={this.UseScreenMoveTrue} type="radio" name="stgDply" id="stgDply01" onChange={() => this.onChangeUseScreenMove("true")} /><label for="stgDply01">초기화면으로 이동</label></li>
								<li><input ref={this.UseScreenMoveFalse} type="radio" name="stgDply" id="stgDply02" onChange={() => this.onChangeUseScreenMove("false")} /><label for="stgDply02">현재화면 유지</label></li>
							</ul>
						</div>
						<div>
								<div className={newStyles.stgName + " " + newStyles.bdNon + " " + newDefaults.mb0 + " " + newDefaults.pb0}>
								<h5>훈련모드</h5>
									<span className={newStyles.stgTltp} data-tooltip="훈련모드를 설정 합니다."></span>
							</div>
								<ul className={newStyles.stgMode}>
									<li><input ref={this.refUseTrainingMode} type="checkbox" name="stgMode" id="stgMode01" onChange={(e) => this.onChangeUseTrainingMode(e.target)} /><label for="stgMode01">모든 센서 신호 수신시 훈련모드로 사용</label></li>
									<li><input ref={this.refUseWaterMark} type="checkbox" name="stgMode" id="stgMode02" onChange={(e) => this.onChangeUseWaterMark(e.target)} /><label for="stgMode02">훈련모드 워터마크 사용</label></li>
									<li><input ref={this.refUseHeadMessage} type="checkbox" name="stgMode" id="stgMode03" onChange={(e) => this.onChangeUseHeadMessage(e.target)} /><label for="stgMode03">전파 메시지 앞머리 문구 지정</label><input ref={this.refHeadMessage} type="text" name="" id="headMessage" className="" onChange={(e) => this.onChangeHeadMessage(e.target)} /></li>
							</ul>
						</div>
					</div>
					*/}

				</div>
			</>
		);
	}

}
