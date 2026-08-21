import React, { Component } from 'react';
import { Container } from 'reactstrap';
import $ from 'jquery';

import Monitoring3D from './monitoring3D';
import DashboardSet from './dashboardSet';
import SopSet from './sopSet';
import TeamEditor from './teamEditor';
import { SettingController } from '../../services/settingController';
import SessionString from '../../../Common/js/sessionString';
import SettingsStore from '../../settingsStore';
import ConfirmDialog from '../../../Common/ui/confirmDialog';

import newStyles from '../../../Common/css/newStyle.module.css';
import newDefaults from '../../../Common/css/newDefault.module.css';
import settings from '../../css/settings.module.css';

import SettingResource from '../../resource/id';
import ProjectResource from '../../../Root/resource/id';
import AccountResource from '../../../Account/resource/id';

import { SDMSController } from '../../../SDMS/services/sdmsController';
import { TeamEditController } from '../../../TeamEditor/services/teamEditController';
import SopController from '../../../SOPManager/services/sopController';

import styles from '../../../Common/css/style.module.css';


class LayoutSetting extends Component {
    constructor(props) {
		super(props);

		this.state = {
			menu: SettingResource.ID.menu.monitoring3D,
			confirmMessage: {
				visible: false,
				title: "",
				messages: [""],
				buttons: ["확인"],
				onClose: this.onCloseConfirmDialog,
				onClickButton: null
			},
			onOffState: true,			// 팝업 OnOff 상태
			isSaving: false,
			isLoading: true,			// 정보 불러오기 체크

			disasterCategories: null,   // 재난 종류
			buildingGroupList: null,
			teamTreeDatas: null,
			teams: null,
			members: null,

			settings: null,
			spreadMessages: null,
			linkedSOPs: null,			// 재난 종류 및 빌딩, 층에 따른 SOP 연결 정보
        }

		this.props = props;

		this.init();
	}

	async init() {
		let userInfo = await ProjectResource.initUserInfo();
		if (userInfo === null || userInfo === undefined)
			return;

		// 설정 불러오기 
		const [result, message] = await SettingController.requestSettings(userInfo.id);
		if (result === null || result === undefined)
			return;

		this.setState({ settings: result, isLoading: false });

		// 단축키 적용, sdms 회전 대기시간 적용
		let shortcutKey = result.shortcutKey;
		let idleTime = result.idleTime;
		let moveDisplayAlarm = result.moveDisplayAlarm;
		let turnStart = result.turnStart;
		let useAlarmTurn = result.useAlarmTurn;
		SettingsStore.dispatch({ type: 'SETTINGS', idleTime, moveDisplayAlarm, turnStart, useAlarmTurn });

		// 건물 정보 가져오기
		const [buildingGroupListData, outdoorZones, errorMessage] = await SDMSController.requestBuildingGroupList();
		let buildingGroupList = [];

		if (buildingGroupListData !== null && buildingGroupListData !== undefined)
			buildingGroupList = buildingGroupListData;

		// 초기 상황전파 메시지 가져오기
		let spreadMessages = [];
		const [spreadResult, spreadMessage] = await SettingController.requestGetSpreadMessage();
		if (spreadResult !== null && spreadResult !== undefined)
			spreadMessages = spreadResult;

		// SOP Link 정보 가져오기
		let linkedSOPs = [];
		const [linkedSOPData, linkedSOPMessage] = await SettingController.requestLinkedSOPs();
		if (linkedSOPData !== null && linkedSOPData !== undefined)
			linkedSOPs = linkedSOPData;

		// 정규조직 팀, 멤버 가져오기
		const teamTreeDatas = await TeamEditController.DisplayRegular();
		const teams = await TeamEditController.GetRegular();
		const members = await TeamEditController.DisplayRegularMember();

		// SOP 재난 정보 가져오기
		const [disasterCategories, disasterCategoriesMessage] = await SopController.disasterCategories(true);

		this.setState({ buildingGroupList, teamTreeDatas, teams, members, disasterCategories, spreadMessages, linkedSOPs });


		// 백업용 데이터 설정 데이터 불러오기 
		//this.backupData();
	}

	componentWillUpdate(nextProps, nextState) {
		//console.log('componentWillUpdate');
	}

	componentDidMount() {
		
	}

	onClickClose = (mode) => {
		this.state.onOffState = false;
		//console.log('onOffState: ' + this.state.onOffState);

		this.props.settingOff(mode);
	}

	onClickSaveClose = () => {
		this.props.settingOff(SettingResource.closeMode.confirm);
	}

	afterReload = () => {
		this.props.settingOff(SettingResource.closeMode.afterReload);
    }

	onClickMenu = (menu, target) => {
		$('.settingMenu li a').removeClass(newStyles.on);
		$(target).addClass(newStyles.on);

		this.setState({ menu: menu });
    }


	setMenuUI = () => {
		let menuUI = [];
		let shortcutKey = null;

		if (this.state.isLoading === true) {
			menuUI.push(
				<React.Fragment key="Loading"><p className={settings.white}>설정을 불러오고 있는 중입니다...</p></React.Fragment>
			);
        } else if (this.state.menu === SettingResource.ID.menu.monitoring3D) {
			menuUI.push(
				<Monitoring3D key="Monitoring3D" settings={this.state.settings} buildingGroupList={this.state.buildingGroupList} spreadMessages={this.state.spreadMessages}
				teamTreeDatas={this.state.teamTreeDatas} teams={this.state.teams} members={this.state.members} showConfirmDialog={this.showConfirmDialog} />); 
		} else if (this.state.menu === SettingResource.ID.menu.dashboardSet) {
			menuUI.push(
				<DashboardSet key="DashboardSet" settings={this.state.settings} />);
		} else if (this.state.menu === SettingResource.ID.menu.sopSet) {
			menuUI.push(
				<SopSet key="SopSet" settings={this.state.settings} buildingGroupList={this.state.buildingGroupList} disasterCategories={this.state.disasterCategories} linkedSOPs={this.state.linkedSOPs} />);
		} else if (this.state.menu === SettingResource.ID.menu.teamEditor) {
			menuUI.push(
				<TeamEditor key="TeamEditor" settings={this.state.settings} />);
		}
		return menuUI;
	}


	onClickSave = () => {
		let userInfo = ProjectResource.getUserInfo();
		if (userInfo === null || userInfo === undefined)
			return;

		const settings = this.state.settings;

		const spreadMessages = this.state.spreadMessages;
		const linkedSOPs = this.state.linkedSOPs;
		const buildingFile = settings.buildingFile;
		const groupFile = settings.groupFile;
		const facilityFile = settings.facilityFile;
		const regularTeamFile = settings.regularTeamFile;

		let saveData = {
			userID: userInfo.id,
			shortcutKey: settings.shortcutKey,
			idleTime: settings.idleTime,
			reAlarm: settings.reAlarm,
			useReceiveFire: settings.useReceiveFire,
			useReceivePSM: settings.useReceivePSM,
			useReceiveETC: settings.useReceiveETC,
			useReceiveSVMS: settings.useReceiveSVMS,
			eventInfoDisplayTerm: settings.eventInfoDisplayTerm,
			useScreenMove: settings.useScreenMove,
			exeCautionSOP: settings.exeCautionSOP,
			exeAlartSOP: settings.exeAlartSOP,
			exeSeriousSOP: settings.exeSeriousSOP,
			useTrainingMode: settings.useTrainingMode,
			useWaterMark: settings.useWaterMark,
			useHeadMessage: settings.useHeadMessage,
			useAutoMoveSOPScreen: settings.useAutoMoveSOPScreen,
			useBroadcast: settings.useBroadcast,
			useSMS: settings.useSMS,
			useEmail: settings.useEmail,
			useConfirm: settings.useConfirm,
			workingBeginHour: settings.workingBeginHour,
			workingEndHour: settings.workingEndHour,
			useResultSummary: settings.useResultSummary,
			dashboardBegin: settings.dashboardBegin,
			dashboardEnd: settings.dashboardEnd,
			fireSOPWaitEndTime: settings.fireSOPWaitEndTime,
			psmsopWaitEndTime: settings.psmsopWaitEndTime,
			etcsopWaitEndTime: settings.etcsopWaitEndTime,
			fireSOPRecoverEndTime: settings.fireSOPRecoverEndTime,
			psmsopRecoverEndTime: settings.psmsopRecoverEndTime,
			etcsopRecoverEndTime: settings.etcsopRecoverEndTime,
			moveDisplayAlarm: settings.moveDisplayAlarm,
			useAlarmBroadcast: settings.useAlarmBroadcast,
			usePoiFocus: settings.usePoiFocus,
			usePoiHighlight: settings.usePoiHighlight,
			turnStart: settings.turnStart,
			useAlarmTurn: settings.useAlarmTurn,
		};

		this.doSave(saveData, buildingFile, groupFile, facilityFile, regularTeamFile, spreadMessages, linkedSOPs);
	}

	async doSave(saveData, buildingFile, groupFile, facilityFile, regularTeamFile, spreadMessages, linkedSOPs) {
		// 설정 값이 이미 저장 중 일 경우
		if (this.state.isSaving === true)
			return;

		// 저장 중 상태 변화
		this.state.isSaving = true;

		let [success, message] = await SettingController.requestSaveSettings(saveData);

		if (success === true) {
			// 설정 적용
			let idleTime = saveData.idleTime;
			let moveDisplayAlarm = saveData.moveDisplayAlarm;
			let turnStart = saveData.turnStart;
			let useAlarmTurn = saveData.useAlarmTurn;
			SettingsStore.dispatch({ type: 'SETTINGS', idleTime, moveDisplayAlarm, turnStart, useAlarmTurn });
        } else {
			//alert(message);
			this.showConfirmDialog("에러", [message], null, null);
		}

		let spreadTemp = spreadMessages;
		// 초기 상황전파 메시지 가져오기
		let spreadOld = [];
		const [spreadResult, spreadMessage] = await SettingController.requestGetSpreadMessage();
		if (spreadResult !== null && spreadResult !== undefined)
			spreadOld = spreadResult;

		// 변경점 비교하기
		let addSpread = [];
		let updateSpread = [];

		if (spreadTemp !== null && spreadTemp !== undefined) {
			for (let i = 0; i < spreadTemp.length; i++) {
				let spread = spreadTemp[i];

				if (spread.id === -1) {
					addSpread.push(spread);
					continue;
				}

				for (let j = 0; j < spreadResult.length; j++) {
					let spreadData = spreadResult[j];

					if (spread.id === spreadData.id) {
						if (spread.regularID !== spreadData.regularID ||
							spread.regularMemberID !== spreadData.regularMemberID ||
							spread.message !== spreadData.message) {
							updateSpread.push(spread);
						}

						spreadResult.splice(j, 1);
						break;
					}
				}
			}
		}

		[success, message] = await SettingController.requestSetSpreadMessage(addSpread, updateSpread, spreadResult);

		if (success === null) {
			// 저장이 끝남 상태변화
			this.state.isSaving = false;

			this.showConfirmDialog("에러", [message], null, null);
			return;
		}

		// 상황별 sop 설정
		let linkedSOPData = linkedSOPs;
		let linkedSOPOld = [];					// 삭제 linkedSOP 리스트

		const [linkedSOPResult, linkedSOPMessage] = await SettingController.requestLinkedSOPs();
		if (linkedSOPResult !== null && linkedSOPResult !== undefined)
			linkedSOPOld = linkedSOPResult;

		// 비교하기
		let addLinkedSOP = [];
		let updateLinkedSOP = [];

		if (linkedSOPData !== null && linkedSOPData !== undefined) {
			for (let i = 0; i < linkedSOPData.length; i++) {
				let linkedSOP = linkedSOPData[i];
				let chk = false;

				for (let j = 0; j < linkedSOPOld.length; j++) {
					const sopOld = linkedSOPOld[j];

					if (linkedSOP.facilityTypeID === sopOld.facilityTypeID &&
						(linkedSOP.linkedBuildingID === sopOld.linkedBuildingID || ((linkedSOP.linkedBuildingID === 0 || linkedSOP.linkedBuildingID === -1) && (sopOld.linkedBuildingID === 0 || sopOld.linkedBuildingID === -1))) &&
						(linkedSOP.linkedZoneID === sopOld.linkedZoneID || ((linkedSOP.linkedZoneID === 0 || linkedSOP.linkedZoneID === -1) && (sopOld.linkedZoneID === 0 || sopOld.linkedZoneID === -1)))) {
						chk = true;

						if (linkedSOP.disasterCategoryID !== sopOld.disasterCategoryID ||
							linkedSOP.subDisasterCategoryID !== sopOld.subDisasterCategoryID ||
							linkedSOP.disasterName !== sopOld.disasterName) {
							updateLinkedSOP.push(linkedSOP);
						}

						linkedSOPOld.splice(j, 1);
						break;
                    }
				}

				if (chk === false)
					addLinkedSOP.push(linkedSOP);
			}
		}

		[success, message] = await SettingController.requestUpdateLinkedSOPs(addLinkedSOP, updateLinkedSOP, linkedSOPOld);

		if (success === null) {
			// 저장이 끝남 상태변화
			this.state.isSaving = false;

			this.showConfirmDialog("에러", [message], null, null);
			return;
		}

		// 건물 정보 업로드
		if (buildingFile !== null && buildingFile !== undefined) {
			[success, message] = await SettingController.requestUploadBuildingFile(buildingFile);
			if (success !== true) {
				// 저장이 끝남 상태변화
				this.state.isSaving = false;

				//alert("건물정보 업로드 실패:" + message);
				this.showConfirmDialog("에러", ["건물정보 업로드 실패 : " + message], null, null);
				return;
			} else if (success === true) {
				this.state.settings.buildingFile = null;
			}
        }
		
		// 건물그룹 정보 업로드
		if (groupFile !== null && groupFile !== undefined) {
			[success, message] = await SettingController.requestUploadBuildingGroupFile(groupFile);
			if (success !== true) {
				// 저장이 끝남 상태변화
				this.state.isSaving = false;

				//alert("건물그룹 정보 업로드 실패:" + message);
				this.showConfirmDialog("에러", ["건물그룹 정보 업로드 실패 : " + message], null, null);
				return;
			} else if (success === true) {
				this.state.settings.groupFile = null;
            }
        }

		// 설비 정보 업로드
		if (facilityFile !== null && facilityFile !== undefined) {
			[success, message] = await SettingController.requestUploadFacilityFile(facilityFile);
			if (success !== true) {
				// 저장이 끝남 상태변화
				this.state.isSaving = false;

				//alert("설비 정보 업로드 실패:" + message);
				this.showConfirmDialog("에러", ["설비 정보 업로드 실패 : " + message], null, null);
				return;
			} else if (success === true) {
				this.state.settings.facilityFile = null;
			}
		}

		// 조직 정보 업로드
		if (regularTeamFile !== null && regularTeamFile !== undefined) {
			[success, message] = await SettingController.requestUploadRegularTeamFile(regularTeamFile);
			if (success !== true) {
				// 저장이 끝남 상태변화
				this.state.isSaving = false;

				//alert("조직 정보 업로드 실패:" + message);
				this.showConfirmDialog("에러", ["조직 정보 업로드 실패 : " + message], null, null);
				return;
			} else if (success === true) {
				this.state.settings.regularTeamFile = null;
			}
        }

		const settingOnOff = this.props.settingOnOff;
		// 창이 닫힌 상태
		if (this.state.onOffState === false) {
			//this.props.reloadSetting();
			this.afterReload();
        }

		// 저장이 끝남 상태변화
		this.state.isSaving = false;

		// 설정 완료 팝업
		//alert("설정이 저장되었습니다.");
		// 닫기
		//this.onClickClose(SettingResource.closeMode.confirm);
		this.showConfirmDialog("확인", ["설정이 저장되었습니다."], ["확인"], this.onClickSaveClose);
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

	getAuthorMenu() {
		let authorMenuUI = [];
		authorMenuUI.push(<React.Fragment key={"memuCommon"}>
			<li><a onClick={(e) => this.onClickMenu(SettingResource.ID.menu.monitoring3D, e.target)} className={newStyles.on}>3D 관제 시스템</a></li>
		</React.Fragment>);

		const userAuthor = ProjectResource.getUserAuthor();
		if (userAuthor === AccountResource.ID.accountLevel.admin) {
			authorMenuUI.push(<React.Fragment key={"memuAdmin"}>
				{/*<li><a onClick={(e) => this.onClickMenu(SettingResource.ID.menu.dashboardSet, e.target)}>대시보드</a></li>*/}
				{/*// .TODO: GS인증 */}
				<li><a onClick={(e) => this.onClickMenu(SettingResource.ID.menu.sopSet, e.target)}>SOP</a></li>
				<li><a onClick={(e) => this.onClickMenu(SettingResource.ID.menu.teamEditor, e.target)}>조직관리</a></li>
			</React.Fragment>);
		} 

		return authorMenuUI;
    }

	render() {
		const menuUI = this.setMenuUI();
		const authorMenuUI = this.getAuthorMenu();
		this.state.onOffState = true;

		
        return (
            <>
				<div id={newStyles.stgPop}>
					<div>
						<div>
							<div className={newStyles.stgCont}>
								<div className={newStyles.stgLft}>
									<h4 className={newStyles.stgTitle}>환경설정</h4>
									<ul className={newStyles.stgMenu + " settingMenu"}>
										{authorMenuUI}
									</ul>
								</div>
								<div className={newStyles.stgRht}>
									<a onClick={() => this.onClickClose(SettingResource.closeMode.cancle)} className={newStyles.stgClose}>닫기</a>
									  {menuUI}
									<ul className={newStyles.dspBtn}>
										<li><a className={settings.pointCursor} onClick={this.onClickSave}>확인</a></li>
										<li><a className={settings.pointCursor} onClick={() => this.onClickClose(SettingResource.closeMode.cancle)}>취소</a></li>
									</ul>
								</div>
							</div>
						</div>
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

export default LayoutSetting;