import React, { Component } from 'react';
import styles from '../../Common/css/style.module.css';
import teamEditors from '../css/teamEditor.module.css';

import TeamEditorContent from './teamEditorContent';
import TeamMenu from './teamMenu';
import ScheduleMenu from './scheduleMenu';
import SchedulePage from './schedulePage';
import RegularMemberPage from './regular/regularMemberPage';
import TemporaryMemberPage from './temporary/temporaryMemberPage';
import ConfirmDialog from '../../Common/ui/confirmDialog';

import $ from 'jquery';

import TeamEditorResource from '../resource/id';
//import Commands from "../services/commands";
//import CommandStyle from "../services/commandStyle";
import SessionString from '../../Common/js/sessionString';
import { TeamEditController } from '../services/teamEditController';

import teamCSS from '../css/teamEditor.module.css';

class TeamEditorNew extends Component {
	static cssStyles = styles;

	constructor(props) {
		super(props);

		this.state = {
			content: TeamEditorResource.ID.textRegular,
			team: TeamEditorResource.ID.textRegular,
			schedule: TeamEditorResource.ID.textFixed,
			isEditMode: false,

			regularTreeData: [],				// 정규조직 팀 데이터
			temporaryTreeData: [],				// 평일 비상조직 팀 데이터
			temporaryEmergencyTreeData: [],		// 주말 비상조직 팀 데이터
			teamTreeData: [],					/* Treeview에 바인딩된 데이터(현재 선택된 팀 데이터) */
			regularMembers: [],					// 정규조직 멤버 데이터
			temporaryMembers: [],
			memberGridData: [],					/* GridView에 바인딩된 데이터(현재 선택된 팀 멤버 데이터 */
			//displayMemberGridData: [],			/* 화면에 출력할 팀원 정보들 (검색에 활용) */

			regularTreeOld: [],					// 정규조직 팀 기존 데이터 (저장 시에 비교 데이터)
			temporaryTreeOld: [],				// 평일 비상조직 팀 기존 데이터 (저장 시에 비교 데이터)
			temporaryEmergencyTreeOld: [],		// 주말 비상조직 팀 기존 데이터 (저장 시에 비교 데이터)
			regularMembersOld: [],				// 정규조직 멤버 기존 데이터 (저장 시에 비교 데이터)
			temporaryMembersOld: [],

			selectedTeam: null,					/* Treeview에서 선택된 팀정보 */
			jobLevels: null,					/* 직급 정보들 (JSON {{value: "value값", name: "name값"}...}) */
			jobPositions: null,					/* 직위 정보들 (JSON {{value: "value값", name: "name값"}...}) */
			isSaveEnable: false,				/* Save 버튼 활성화 여부 */

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
		//this.onChangeMember = this.onChangeMember.bind(this);
		//this.onAddMember = this.onAddMember.bind(this);
		//this.onDeleteMember = this.onDeleteMember.bind(this);
		this.removeTeam = this.removeTeam.bind(this);
		this.editTeam = this.editTeam.bind(this);
		this.save = this.save.bind(this);
	}

	componentDidMount() {
		// LG화학, 솔브레인 CSS 충돌 >> LG화학 폰트 색상 관련 수정
		$('html, body').css({ 'display': 'block', 'height': '100%', 'overflow': 'hidden', 'color': '#000', 'font-size': '14px' });

		// 각 페이지 별로 클래스 초기화
		$('#subPage').removeClass('sop');

		$('#subPage').click(function (e) {
			// 새 인원 css 효과가 있을 경우 제거
			$('#regularMemberTableBody').removeClass(teamEditors.addPointer);
			$('#temporaryMemberTableBody').removeClass(teamEditors.addPointer);
		});

		this.displayRegular();
		this.displayJob();
		this.initTemporary();
	}

	async initTemporary() {
		// 비상조직, 멤버 초기화
		const temporaryTreeData = await TeamEditController.DisplayTemporary(true);
		//const temporaryTreeOld = await TeamEditController.DisplayTemporary(true);
		const temporaryEmergencyTreeData = await TeamEditController.DisplayTemporary(false);
		//const temporaryEmergencyTreeOld = await TeamEditController.DisplayTemporary(false);

		//let temporaryMembers = null;
		//let temporaryMembersOld = null;
		//let message = null;

		const [temporaryMembers, message] = await TeamEditController.requestTemporaryMembers();
		//[temporaryMembersOld, message] = await TeamEditController.requestTemporaryMembers();

		this.setState({
			temporaryTreeData: temporaryTreeData, temporaryTreeOld: temporaryTreeData,
			temporaryEmergencyTreeData: temporaryEmergencyTreeData, temporaryEmergencyTreeOld: temporaryEmergencyTreeData,
			temporaryMembers: temporaryMembers, temporaryMembersOld: temporaryMembers
		});
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

	changeContent = (content) => {
		if (content == TeamEditorResource.ID.textRegular) {
			this.state.team = TeamEditorResource.ID.textRegular;
		}

		this.setState({ content: content });
		return;
	}

	changeTeamType = (type) => {
		//console.log("changeTeamType: " + type);

		this.setState({ team: type });
		if (type === TeamEditorResource.ID.textRegular) {
			// 조직
			this.displayRegular();
		} else if (type === TeamEditorResource.ID.textTemporary) {
			// 평일 비상조직
			this.displayTemporary();
		} else if (type === TeamEditorResource.ID.textTemporaryEmergency) {
			// 휴일 비상 조직
			this.displayTemporaryEmergency();
		}
		return;
	}

	changeScheduleType = (type) => {
		//console.log("changeScheduleType: " + type);

		this.setState({ schedule: type });
		return;
	}

	changeEditMode = (isEditMode) => {
		//console.log("TeamEditorNew: " + isEditMode);

		this.setState({ isEditMode: isEditMode });
		return;
	}

	async displayRegular() {
		let data = [];

		if (this.state.regularTreeData === null || this.state.regularTreeData.length === 0) {
			data = await TeamEditController.DisplayRegular();
			let temp = await TeamEditController.DisplayRegular();
			this.state.regularTreeData = data;
			this.state.regularTreeOld = temp;
        } else
			data = this.state.regularTreeData;

		if (data.length > 0) 
			this.setState({ selectedTeam: data[0], teamTreeData: data }, () => this.displayRegularMember());
		else
			this.setState({ teamTreeData: data });
	}

	async displayTemporary() {
		let data = [];

		if (this.state.temporaryTreeData === null || this.state.temporaryTreeData.length === 0) {
			data = await TeamEditController.DisplayTemporary(true);
			//let temp = await TeamEditController.DisplayTemporary(true);
			this.state.temporaryTreeData = data;
			this.state.temporaryTreeOld = data;
        } else
			data = this.state.temporaryTreeData;

		if (data.length > 0) {
			//this.setState({ selectedTeam: data[0], teamTreeData: data, }, () => this.onTeamNodeChanged(this.state.selectedTeam));
			this.setState({ selectedTeam: data[0], teamTreeData: data, }, () => this.displayTemporaryMember());
        } else
			this.setState({ teamTreeData: data, });
	}

	async displayTemporaryEmergency() {
		let data = [];

		if (this.state.temporaryEmergencyTreeData === null || this.state.temporaryEmergencyTreeData.length === 0) {
			data = await TeamEditController.DisplayTemporary(false);
			//let temp = await TeamEditController.DisplayTemporary(false);
			this.state.temporaryEmergencyTreeData = data;
			this.state.temporaryEmergencyTreeOld = data;
        } else
			data = this.state.temporaryEmergencyTreeData;

		if (data.length > 0)
			this.setState({ selectedTeam: data[0], teamTreeData: data, }, () => this.displayTemporaryMember());
		else
			this.setState({ teamTreeData: data, });
	}

	async displayJob() {
		var arrJobLevels = new Array();
		let jobLevels = await TeamEditController.GetJobLevels(); // 직급 

		// ColComboBox 데이터형 만들기 >> JSON {{value: "value값", name: "name값"}}
		if (jobLevels !== null) {
			for (let i = 0; i < jobLevels.length; i++) {
				const item = { value: jobLevels[i].PropertyID, name: jobLevels[i].PropertyValue }
				arrJobLevels.push(item);
			}
		}

		var arrJobPositions = new Array();
		let jobPositions = await TeamEditController.GetJobPositions(); // 직책

		// ColComboBox 데이터형 만들기 >> JSON {{value: "value값", name: "name값"}}
		if (jobPositions !== null) {
			for (let i = 0; i < jobPositions.length; i++) {
				const item = { value: jobPositions[i].PropertyID, name: jobPositions[i].PropertyValue }
				arrJobPositions.push(item);
			}
		}

		this.setState({ jobLevels: arrJobLevels, jobPositions: arrJobPositions });
    }

	async displayRegularMember() {
		let members = [];

		if (this.state.regularMembers === null || this.state.regularMembers.length === 0) {
			members = await TeamEditController.DisplayRegularMember(); // 해당 팀원 불러오기
			let temp = await TeamEditController.DisplayRegularMember(); // 해당 팀원 불러오기;
			this.state.regularMembers = members;
			this.state.regularMembersOld = temp;
		} else {
			members = this.state.regularMembers;
		}

		let displayMembers = [];

		if (this.state.selectedTeam !== null && this.state.selectedTeam !== undefined) {
			for (var i = 0; i < members.length; i++) {
				if (this.state.selectedTeam.ID === members[i].RegularID)
					displayMembers.push(members[i]);
			}

			// 하위팀에 소속된 직원까지 표출하기
			//let selectedChildTeams = [];
			//TeamEditController.getChildTeams(this.state.selectedTeam, selectedChildTeams);

			//for (let i = 0; i < members.length; i++) {
			//	//if (this.state.selectedTeam.ID === members[i].RegularID)
			//	for (let j = 0; j < selectedChildTeams.length; j++) {
			//		if (selectedChildTeams[j].ID === members[i].RegularID) {
			//			members[i].teamName = selectedChildTeams[j].Name;
			//			displayMembers.push(members[i]);
			//			break;
			//		}
			//	}
			//}
        }

		//await this.setState({ memberGridData: displayMembers, displayMemberGridData: displayMembers });
		this.setState({ memberGridData: displayMembers });
	}

	async displayTemporaryMember() {
		let temporaryMembers = null;
		//let temporaryMembersOld = null;
		let message = null;

		if (this.state.temporaryMembers === null || this.state.temporaryMembers.length === 0) {
			[temporaryMembers, message] = await TeamEditController.requestTemporaryMembers();
			//[temporaryMembersOld, message] = await TeamEditController.requestTemporaryMembers();
			this.state.temporaryMembers = temporaryMembers;
			this.state.temporaryMembersOld = temporaryMembers;
		} else {
			temporaryMembers = this.state.temporaryMembers;
		}

		let displayMembers = [];

		for (var i = 0; i < temporaryMembers.length; i++) {
			if (this.state.selectedTeam.ID === temporaryMembers[i].temporary.id)
				displayMembers.push(temporaryMembers[i]);
		}

		//await this.setState({ memberGridData: displayMembers, displayMemberGridData: displayMembers });
		this.setState({ memberGridData: displayMembers });
    }

	// 조직,비상조직 TreeView 팀을 선택 했을 때 Member 조회를 한다
	onTeamNodeChanged = (team) => {

		if (team === null) {
			this.setState({ selectedTeam: team});
        } else if (this.state.team === TeamEditorResource.ID.textRegular) {
			this.setState({ selectedTeam: team }, () => this.displayRegularMember());
		} else if (this.state.team === TeamEditorResource.ID.textTemporary || this.state.team === TeamEditorResource.ID.textTemporaryEmergency) {
			this.setState({ selectedTeam: team }, () => this.displayTemporaryMember());
		} 
	}

	makeRemoveRegularMember(deleteMembers, data) {
		let memberGridData = data;

		for (let i = 0; i < deleteMembers.length; i++) {
			for (let j = 0; j < memberGridData.length; j++) {
				if (memberGridData[j].ID === deleteMembers[i].ID) {
					memberGridData.splice(j, 1);
					break;
				}
			}
		}

		let members = null;

		if (this.state.team === TeamEditorResource.ID.textRegular) {
			members = this.state.regularMembers;

			for (let i = 0; i < deleteMembers.length; i++) {
				for (let j = 0; j < members.length; j++) {
					if (members[j].ID === deleteMembers[i].ID) {
						members.splice(j, 1);
						break;
					}
				}
			}
		} else if (this.state.team === TeamEditorResource.ID.textTemporary || this.state.team === TeamEditorResource.ID.textTemporaryEmergency) {
			members = this.state.temporaryMembers;

			for (let i = 0; i < deleteMembers.length; i++) {
				for (let j = 0; j < members.length; j++) {
					if (members[j].id === deleteMembers[i].id) {
						members.splice(j, 1);
						break;
					}
				}
			}
		}

		return [memberGridData, members];
    }

	async removeTeam() {
		if (!this.state.isEditMode)
			return;

		const curTeamTreeData = [...this.state.teamTreeData];

		if (this.state.team === TeamEditorResource.ID.textRegular) {
			//const data = await CommandStyle.MakeCommandRemoveRegularTeam(this.state.selectedTeam, curTeamTreeData);
			const data = await this.makeRemoveRegularTeam(this.state.selectedTeam, curTeamTreeData);
			this.setState({ teamTreeData: curTeamTreeData, regularTreeData: curTeamTreeData }, () => this.onTeamNodeChanged(curTeamTreeData[0]));
		} else if (this.state.team === TeamEditorResource.ID.textTemporary) {
			const data = await this.makeRemoveRegularTeam(this.state.selectedTeam, curTeamTreeData);
			this.setState({ teamTreeData: curTeamTreeData, temporaryTreeData: curTeamTreeData }, () => this.onTeamNodeChanged(curTeamTreeData[0]));
		} else if (this.state.team === TeamEditorResource.ID.textTemporaryEmergency) {
			const data = await this.makeRemoveRegularTeam(this.state.selectedTeam, curTeamTreeData);
			this.setState({ teamTreeData: curTeamTreeData, temporaryEmergencyTreeData: curTeamTreeData }, () => this.onTeamNodeChanged(curTeamTreeData[0]));
		}
	}

	// 조직 삭제
    // data : 모든 팀 정보
	async makeRemoveRegularTeam(selectedTeam, data) {
		// 하위 팀
		const deleteTeams = [];
		deleteTeams.push({ ID: selectedTeam.ID, TeamName: selectedTeam.Name, ParentTeamID: selectedTeam.ParentTeamID });
		if (selectedTeam.Children) {
			TeamEditController.findChild(selectedTeam.ID, selectedTeam.Children, deleteTeams);
		}

		let members = [];
		let deleteMembers = [];

		// 속한 직원 (팀 타입에 따라 달리 설정 필요.)
		if (this.state.team === TeamEditorResource.ID.textRegular) {
			members = this.state.regularMembers;

			for (var i = 0; i < members.length; i++) {
				for (var j = 0; j < deleteTeams.length; j++) {
					if (deleteTeams[j].ID === members[i].RegularID) {
						deleteMembers.push(members[i]);
						break;
					}
				}
			}
		} else if (this.state.team === TeamEditorResource.ID.textTemporary || this.state.team === TeamEditorResource.ID.textTemporaryEmergency) {
			members = this.state.temporaryMembers;

			for (var i = 0; i < members.length; i++) {
				for (var j = 0; j < deleteTeams.length; j++) {
					if (deleteTeams[j].ID === members[i].temporary.id) {
						deleteMembers.push(members[i]);
						break;
					}
				}
			}
        }

		if (deleteMembers.length > 0) {
			const [memberGridData, members] = this.makeRemoveRegularMember(deleteMembers, this.state.memberGridData);
			this.state.memberGridData = memberGridData;
			//this.state.displayMemberGridData = newMembers;

			if (this.state.team === TeamEditorResource.ID.textRegular) {
				this.state.regularMembers = members;
			} else if(this.state.team === TeamEditorResource.ID.textTemporary || this.state.team === TeamEditorResource.ID.textTemporaryEmergency) {
				this.state.temporaryMembers = members;
            }
        }

		let findNode = null;

		if (selectedTeam.ParentTeamID === null) {
			// 루트 노드일 경우
			findNode = TeamEditController.findParent(selectedTeam.ID, data);

			const idx = data.findIndex(function (item) { return item.ID === selectedTeam.ID });
			if (idx > -1) {
				data.splice(idx, 1);
				this.setState({ teamTreeData: data });
			}

		} else {
			// 자식 노드일 경우
			findNode = TeamEditController.findParent(selectedTeam.ParentTeamID, data);

			if (findNode !== null && findNode.Children !== null && findNode.Children) {
				const idx = findNode.Children.findIndex(function (item) { return item.ID === selectedTeam.ID })
				if (idx > -1) {
					findNode.Children.splice(idx, 1);
					this.setState({ teamTreeData: data });
				}
			}
        }
    }

	async editTeam(team, chgName) {
		if (!this.state.isEditMode)
			return;

		const curTeamTreeData = [...this.state.teamTreeData]

		if (this.state.team === TeamEditorResource.ID.textRegular) {
			//const data = await CommandStyle.MakeCommandChangeRegularTeamInfo(this.state.selectedTeam, chgName, curTeamTreeData);
			const data = await this.makeChangeRegularTeamInfo(this.state.selectedTeam, chgName, curTeamTreeData);
			this.setState({ teamTreeData: data, regularTreeData: data });
		} else if (this.state.team === TeamEditorResource.ID.textTemporary) {
			const data = await this.makeChangeRegularTeamInfo(this.state.selectedTeam, chgName, curTeamTreeData);
			this.setState({ teamTreeData: data, temporaryTreeData: data });
		} else if (this.state.team === TeamEditorResource.ID.textTemporaryEmergency) {
			const data = await this.makeChangeRegularTeamInfo(this.state.selectedTeam, chgName, curTeamTreeData);
			this.setState({ teamTreeData: data, temporaryEmergencyTreeData: data });
		}
	}

	async makeChangeRegularTeamInfo(selectedTeam, newData, data) {
		//const findNode = await TeamEditController.findNode(data[0], selectedTeam.ID);
		const findNode = await TeamEditController.findNode(data, selectedTeam.ID);
		findNode.Name = newData;

		return data;
    }


	async save() {
		// 기존 데이터와 비교 후 팀 및 멤버의 추가, 수정, 삭제로 분류 후 서버에 전송

		// 팀 데이터 비교
		const regularTreeData = this.state.regularTreeData;
		//const regularTreeOld = this.state.regularTreeOld;
		const regularTreeOld = await TeamEditController.DisplayRegular();
		let arrRegular = this.regularArrData(regularTreeData);
		let arrRegularOld = this.regularArrData(regularTreeOld);

		const temporaryTreeData = this.state.temporaryTreeData;
		//const temporaryTreeOld = this.state.temporaryTreeOld;
		const temporaryTreeOld = await TeamEditController.DisplayTemporary(true);
		let arrTemporary = this.temporaryArrData(temporaryTreeData, true);
		let arrTemporaryOld = this.temporaryArrData(temporaryTreeOld, true);

		const temporaryEmergencyTreeData = this.state.temporaryEmergencyTreeData;
		//const temporaryEmergencyTreeOld = this.state.temporaryEmergencyTreeOld;
		const temporaryEmergencyTreeOld = await TeamEditController.DisplayTemporary(false);
		let arrTemporaryEmergency = this.temporaryArrData(temporaryEmergencyTreeData, false);
		let arrTemporaryEmergencyOld = this.temporaryArrData(temporaryEmergencyTreeOld, false);

		const [addRegular, updateRegular, removeRegular] = this.compareRegularData(arrRegular, arrRegularOld);
		const [addTemporary, updateTemporary, removeTemporary] = this.compareRegularData(arrTemporary, arrTemporaryOld);
		const [addTemporaryEmergency, updateTemporaryEmergency, removeTemporaryEmergency] = this.compareRegularData(arrTemporaryEmergency, arrTemporaryEmergencyOld);


		// 멤버 데이터
		const regularMembers = this.state.regularMembers;
		//const regularMembersOld = this.state.regularMembersOld;
		const regularMembersOld = await TeamEditController.DisplayRegularMember();
		let arrMembers = this.cloneRularMembers(regularMembers);
		let arrMembersOld = this.cloneRularMembers(regularMembersOld);

		let temporaryMembersOld = null;
		let message = null;

		const temporaryMembers = this.state.temporaryMembers;
		//const temporaryMembersOld = this.state.temporaryMembersOld;
		[temporaryMembersOld, message] = await TeamEditController.requestTemporaryMembers();
		let arrTemporaryMembers = this.cloneRularMembers(temporaryMembers);
		let arrTemporaryMembersOld = this.cloneRularMembers(temporaryMembersOld);

		const [addRegularMembers, updateRegularMembers, removeRegularMembers] = this.compareRegularMemberData(arrMembers, arrMembersOld);
		const [addTemporaryMembers, updateTemporaryMembers, removeTemporaryMembers] = this.compareTemporaryMemberData(arrTemporaryMembers, arrTemporaryMembersOld);

		const addTemporaryMembersData = this.changeTemporaryMemberData(addTemporaryMembers);
		const updateTemporaryMembersData = this.changeTemporaryMemberData(updateTemporaryMembers);
		const removeTemporaryMembersData = this.changeTemporaryMemberData(removeTemporaryMembers);

		// TemporaryMember 데이터 형식 변환


		// 업데이트 데이터 만들기
		let updateData = {
			addRegular: addRegular,
			updateRegular: updateRegular,
			removeRegular: removeRegular,
			addRegularMembers: addRegularMembers,
			updateRegularMembers: updateRegularMembers,
			removeRegularMembers: removeRegularMembers,

			addTemporary: addTemporary,
			updateTemporary: updateTemporary,
			removeTemporary: removeTemporary,
			addTemporaryEmergency: addTemporaryEmergency,
			updateTemporaryEmergency: updateTemporaryEmergency,
			removeTemporaryEmergency: removeTemporaryEmergency,
			//addTemporaryMembers: addTemporaryMembers,
			//updateTemporaryMembers: updateTemporaryMembers,
			//removeTemporaryMembers: removeTemporaryMembers,
			addTemporaryMembers: addTemporaryMembersData,
			updateTemporaryMembers: updateTemporaryMembersData,
			removeTemporaryMembers: removeTemporaryMembersData,
		};

		let result = null;

		[result, message] = await TeamEditController.saveUpdateData(updateData);

		if (result === true) {
			this.showConfirmDialog("저장 성공", ["데이터가 저장되었습니다."], null, null);

			// 기존 데이터 새로고침.
			//let regularTreeOld = await TeamEditController.DisplayRegular();
			//let regularMembersOld = await TeamEditController.DisplayRegularMember(); // 해당 팀원 불러오기;

			let regular = await TeamEditController.DisplayRegular();
			let regularMembers = await TeamEditController.DisplayRegularMember();

			let temporary = await TeamEditController.DisplayTemporary(true);			
			let temporaryEmergency = await TeamEditController.DisplayTemporary(false);

			const [temporaryMembers, message] = await TeamEditController.requestTemporaryMembers();

			let teamTreeData = [];
			if (this.state.team === TeamEditorResource.ID.textRegular) {
				teamTreeData = regular;
			}
			else if (this.state.team === TeamEditorResource.ID.textTemporary) {
				teamTreeData = temporary;
			}
			else if (this.state.team === TeamEditorResource.ID.textTemporaryEmergency) {
				teamTreeData = temporaryEmergency;
			}

			this.setState({
				regularTreeOld: regular, regularMembersOld: regularMembers, regularMembers,
				temporaryTreeData: temporary, temporaryTreeOld: temporary,
				temporaryEmergencyTreeData: temporaryEmergency, temporaryEmergencyTreeOld: temporaryEmergency,
				temporaryMembers: temporaryMembers, temporaryMembersOld: temporaryMembers,
				teamTreeData
			});
		} else {
			this.showConfirmDialog("에러", [message], null, null);
        }
	}

	changeTemporaryMemberData(TemporaryMemberInfos) {
		if (TemporaryMemberInfos === null || TemporaryMemberInfos === undefined)
			return [];

		let TemporaryMembers = [];

		for (let i = 0; i < TemporaryMemberInfos.length; i++) {
			let TemporaryMemberInfo = TemporaryMemberInfos[i];

			let TemporaryMember = new Object();
			TemporaryMember.ID = TemporaryMemberInfo.id;
			TemporaryMember.DisplaySOPName = TemporaryMemberInfo.displaySOPName;
			TemporaryMember.TeamID = TemporaryMemberInfo.temporary.id;

			TemporaryMember.RegularID = null;
			if (TemporaryMemberInfo.regular !== null && TemporaryMemberInfo.regular !== undefined)
				TemporaryMember.RegularID = TemporaryMemberInfo.regular.id;

			TemporaryMember.RegularMemberID = null;
			if (TemporaryMemberInfo.regularMember !== null && TemporaryMemberInfo.regularMember !== undefined)
				TemporaryMember.RegularMemberID = TemporaryMemberInfo.regularMember.id;

			if (TemporaryMemberInfo.isNormal === true)
				TemporaryMember.IsNormal = 1;
			else 
				TemporaryMember.IsNormal = 0;

			TemporaryMember.Role = TemporaryMemberInfo.role;

			TemporaryMembers.push(TemporaryMember);
		}

		return TemporaryMembers;
    }

	compareRegularMemberData(arrMembers, arrMembersOld) {
		if (arrMembers.length === 0 && arrMembersOld === 0)
			return [[], [], []];

		let addRegularMembers = [];
		let updateRegularMembers = [];

		for (let i = 0; i < arrMembers.length; i++) {
			let member = arrMembers[i];

			// 추가 멤버 구분
			if (member.ID < 0) {
				addRegularMembers.push(member);
				continue;
			}

			// 업데이트 및 삭제 멤버 구분
			for (let j = 0; j < arrMembersOld.length; j++) {
				let memberOld = arrMembersOld[j];

				if (member.ID === memberOld.ID) {
					if (member.MemberName !== memberOld.MemberName ||
						member.MemberID !== memberOld.MemberID ||
						member.JobLevelID !== memberOld.JobLevelID ||
						member.JobPositionID !== memberOld.JobPositionID ||
						member.OfficePhoneNumber !== memberOld.OfficePhoneNumber ||
						member.PhoneNumber !== memberOld.PhoneNumber ||
						member.RegularID !== memberOld.RegularID ||
						member.Email !== memberOld.Email) {
						updateRegularMembers.push(member);
					} 

					arrMembersOld.splice(j, 1);
					break;
                }
			}
		}

		return [addRegularMembers, updateRegularMembers, arrMembersOld];
    }

	compareTemporaryMemberData(arrMembers, arrMembersOld) {
		if (arrMembers.length === 0 && arrMembersOld === 0)
			return [[], [], []];

		let addTemporaryMembers = [];
		let updateTemporaryMembers = [];

		for (let i = 0; i < arrMembers.length; i++) {
			let member = arrMembers[i];

			// 추가 멤버 구분
			if (member.id < 0) {
				addTemporaryMembers.push(member);
				continue;
			}

			// 업데이트 및 삭제 멤버 구분
			for (let j = 0; j < arrMembersOld.length; j++) {
				let memberOld = arrMembersOld[j];

				if (member.id === memberOld.id) {
					if (member.displaySOPName !== memberOld.displaySOPName ||
						member.role !== memberOld.role ||
						member.temporary.id !== memberOld.temporary.id) {
						updateTemporaryMembers.push(member);
					} else {
						let regularID = -1;
						let regularID_Old = -1;
						let regularMemberID = -1;
						let regularMemberID_Old = -1;

						if (member.regular !== null && member.regular !== undefined)
							regularID = member.regular.id;
						if (memberOld.regular !== null && memberOld.regular !== undefined)
							regularID_Old = memberOld.regular.id;
						if (member.regularMember !== null && member.regularMember !== undefined)
							regularMemberID = member.regularMember.id;
						if (memberOld.regularMember !== null && memberOld.regularMember !== undefined)
							regularMemberID_Old = memberOld.regularMember.id;

						if (regularID !== regularID_Old ||
							regularMemberID !== regularMemberID_Old)
							updateTemporaryMembers.push(member);
                    }

					arrMembersOld.splice(j, 1);
					break;
				}
			}
		}

		return [addTemporaryMembers, updateTemporaryMembers, arrMembersOld];
	}

	cloneRularMembers(regularMembers) {
		let arrMembers = [];

		if (regularMembers === null || regularMembers.length === 0)
			return arrMembers;

		for (let i = 0; i < regularMembers.length; i++) {
			let member = regularMembers[i];

			arrMembers.push(member);
        }

		return arrMembers;
    }

	compareRegularData(arrRegular, arrRegularOld) {
		if (arrRegular.length === 0 && arrRegularOld === 0)
			return [[], [], []];

		let addRegular = [];
		let updateRegular = [];

		for (let i = 0; i < arrRegular.length; i++) {
			let data = arrRegular[i];

			if (data.ID < 0) {
				// 추가 데이터
				addRegular.push(data);
				continue;
			}

			for (let j = 0; j < arrRegularOld.length; j++) {
				let oldData = arrRegularOld[j];

				if (oldData.ID === data.ID) {
					if (oldData.TeamName !== data.TeamName) {
						updateRegular.push(data);
					}
					
					arrRegularOld.splice(j, 1);
					break;
                }
			}
        }

		return [addRegular, updateRegular, arrRegularOld];
    }

	temporaryArrData(regular, isNormal) {
		let arrRegularData = [];

		if (regular === null || regular === undefined)
			return arrRegularData;

		/*
		let regularData = regular[0];
		arrRegularData.push({ ID: regularData.ID, TeamName: regularData.Name, ParentTeamID: regularData.ParentTeamID, IsNormal: isNormal, SiteID: -1 });

		if (regularData.Children) {
			this.temporaryChildrenArrData(regularData.Children, arrRegularData, isNormal);
		}
		*/
		for (let regularData of regular) {
			arrRegularData.push({ ID: regularData.ID, TeamName: regularData.Name, ParentTeamID: regularData.ParentTeamID, IsNormal: isNormal, SiteID: -1 });

			if (regularData.Children) {
				this.temporaryChildrenArrData(regularData.Children, arrRegularData, isNormal);
			}
        }


		return arrRegularData;
	}

	temporaryChildrenArrData(regularChildren, arrRegularData, isNormal) {

		if (regularChildren !== null && regularChildren !== undefined && regularChildren.length > 0) {

			for (let i = 0; i < regularChildren.length; i++) {
				let data = regularChildren[i];
				arrRegularData.push({ ID: data.ID, TeamName: data.Name, ParentTeamID: data.ParentTeamID, IsNormal: isNormal, SiteID: -1 });

				if (data.Children) {
					this.temporaryChildrenArrData(data.Children, arrRegularData, isNormal);
                }
            }
		}
	}

	regularArrData(regular) {
		let arrRegularData = [];

		if (regular === null || regular === undefined)
			return arrRegularData;

		/*
		let regularData = regular[0];
		arrRegularData.push({ ID: regularData.ID, TeamName: regularData.Name, ParentTeamID: regularData.ParentTeamID });

		if (regularData.Children) {
			this.regularChildrenArrData(regularData.Children, arrRegularData);
		}
		*/
		for (let regularData of regular) {
			arrRegularData.push({ ID: regularData.ID, TeamName: regularData.Name, ParentTeamID: regularData.ParentTeamID });

			if (regularData.Children) {
				this.regularChildrenArrData(regularData.Children, arrRegularData);
			}
        }

		return arrRegularData;
	}

	regularChildrenArrData(regularChildren, arrRegularData) {

		if (regularChildren !== null && regularChildren !== undefined && regularChildren.length > 0) {

			for (let i = 0; i < regularChildren.length; i++) {
				let data = regularChildren[i];
				arrRegularData.push({ ID: data.ID, TeamName: data.Name, ParentTeamID: data.ParentTeamID });

				if (data.Children) {
					this.regularChildrenArrData(data.Children, arrRegularData);
				}
			}
		}
	}

	checkMemberID = (id, memberID) => {
		console.log("ID: " + id + ", 사번:" + memberID);

		if (id === null || id === undefined ||
			memberID === null || memberID === undefined)
			return;

		let members = [];
		let chk = false;

		// 속한 직원 (팀 타입에 따라 달리 설정 필요.)
		if (this.state.team === TeamEditorResource.ID.textRegular)
			members = this.state.regularMembers;
		else
			members = this.state.regularMembers;

		for (let i = 0; i < members.length; i++) {
			let member = members[i];

			if (member.ID !== id && member.MemberID === memberID) {
				chk = true;
				break;
            }
        }

		if (chk === true) {
			// 사번이 중복됨.
			//alert(memberID + "사번이 이미 사용 중에 있습니다.");
			this.showConfirmDialog("에러", [memberID + " 사번이 이미 사용 중에 있습니다."], null, null);

			$('#colMemberID_' + id).val("");
        }
	}

	checkPhoneNumber = (phoneNumber, id) => {
		console.log("ID: " + id + ", 휴대폰:" + phoneNumber);

		if (id === null || id === undefined ||
			phoneNumber === null || phoneNumber === undefined)
			return;

		let members = [];
		let chk = false;

		// 속한 직원 (팀 타입에 따라 달리 설정 필요.)
		if (this.state.team === TeamEditorResource.ID.textRegular)
			members = this.state.regularMembers;
		else
			members = this.state.regularMembers;

		for (let i = 0; i < members.length; i++) {
			let member = members[i];

			if (member.ID !== id && member.PhoneNumber === phoneNumber) {
				chk = true;
				break;
			}
		}

		if (chk === true) {
			// 휴대전화번호이 중복됨.
			//alert(phoneNumber + "휴대전화번호가 이미 사용 중에 있습니다.");
			this.showConfirmDialog("에러", [phoneNumber + " 휴대전화번호가 이미 사용 중에 있습니다."], null, null);

			$('#colPhoneNumber_' + id).val("");
		}
	}

	onAuthorError = () => {
		this.showConfirmDialog("권한", ["해당 로그인 사용자는 권한이 없습니다."], null, null);
	}

	render() {
		return (
			<div id="subPage">
				<div id={styles.subAside} className="pageMenu">
					{/* 조직, 근무표 메뉴 버튼 컴포넌트 */}
					<TeamEditorContent
						content={this.changeContent}
						isEditMode={this.changeEditMode}
						save={this.save}
						onAuthorError={this.onAuthorError} />

					{/* 조직, 근무표 선택 시 해당 메뉴 컴포넌트 */}
					<DisplayMenu menu={this.state.content}
						teamType={this.changeTeamType}
						scheduleType={this.changeScheduleType}
						onTeamNodeChanged={this.onTeamNodeChanged}
						isEditMode={this.state.isEditMode}
						teamTreeData={this.state.teamTreeData}
						selectedTeam={this.state.selectedTeam}
						removeTeam={this.removeTeam}
						editTeam={this.editTeam}/>
				</div>

				{/* 멤버 테이블 또는 근무표 테이블 */}
				<DisplayContent menu={this.state.content}
					teamType={this.state.team}
					scheduleType={this.state.schedule}
					selectedTeam={this.state.selectedTeam}
					isEditMode={this.state.isEditMode}
					regularTreeData={this.state.regularTreeData}
					regularMembers={this.state.regularMembers}
					temporaryMembers={this.state.temporaryMembers}
					memberGridData={this.state.memberGridData}
					//displayMemberGridData={this.state.displayMemberGridData}
					jobLevels={this.state.jobLevels}
					jobPositions={this.state.jobPositions}
					//onChangeMember={this.onChangeMember}
					//onAddMember={this.onAddMember}
					//onDeleteMember={this.onDeleteMember}
					checkMemberID={this.checkMemberID}
					checkPhoneNumber={this.checkPhoneNumber}
				/>
				{
					/* alert창 대신 사용 */
					this.state.confirmMessage.visible &&
					<ConfirmDialog title={this.state.confirmMessage.title} messages={this.state.confirmMessage.messages} buttons={this.state.confirmMessage.buttons} onClose={this.state.confirmMessage.onClose} onClickButton={this.state.confirmMessage.onClickButton} />
				}
			</div>
		);
	}
}

class DisplayMenu extends Component {
	constructor(props) {
		super(props);

		this.props = props;
	}

	relayTeamType = (type) => {
		//console.log("relayTeamType: " + type);
		this.props.teamType(type);

		return;
	}

	relayScheduleType = (type) => {
		//console.log("relayScheduleType: " + type);
		this.props.scheduleType(type);

		return;
	}
	
	render() {
		if (this.props.menu === TeamEditorResource.ID.textRegular) {
			return <TeamMenu teamTreeData={this.props.teamTreeData}
				selectedTeam={this.props.selectedTeam}
				teamType={this.relayTeamType}
				onTeamNodeChanged={this.props.onTeamNodeChanged}
				isEditMode={this.props.isEditMode}
				removeTeam={this.props.removeTeam}
				editTeam={this.props.editTeam}
			/>;
		}
		else if (this.props.menu === TeamEditorResource.ID.textSchedule) {
			return <ScheduleMenu onChange={this.relayScheduleType} />;
		}
		else
			return null;
	}
}

class DisplayContent extends Component {
	constructor(props) {
		super(props);

		this.props = props;
	}

	render() {
		if (this.props.menu === TeamEditorResource.ID.textRegular && this.props.teamType === TeamEditorResource.ID.textRegular) {
			return (
				<RegularMemberPage selectedTeam={this.props.selectedTeam}
					isEditMode={this.props.isEditMode}
					regularMembers={this.props.regularMembers}
					memberGridData={this.props.memberGridData}
					//displayMemberGridData={this.props.displayMemberGridData}
					jobLevels={this.props.jobLevels}
					jobPositions={this.props.jobPositions}
					//onChangeMember={this.props.onChangeMember}
					//onAddMember={this.props.onAddMember}
					//onDeleteMember={this.props.onDeleteMember}
					checkMemberID={this.props.checkMemberID}
					checkPhoneNumber={this.props.checkPhoneNumber}
				/>
			);
		} else if (this.props.menu === TeamEditorResource.ID.textRegular && (this.props.teamType === TeamEditorResource.ID.textTemporary || this.props.teamType === TeamEditorResource.ID.textTemporaryEmergency)) {
			return (
				<TemporaryMemberPage
					selectedTeam={this.props.selectedTeam}
					temporaryMembers={this.props.temporaryMembers}
					memberGridData={this.props.memberGridData}
					teamType={this.props.teamType}
					isEditMode={this.props.isEditMode}
					jobLevels={this.props.jobLevels}
					jobPositions={this.props.jobPositions}
					regularTreeData={this.props.regularTreeData}
					regularMembers={this.props.regularMembers}
				/>
			);
		} else if (this.props.menu === TeamEditorResource.ID.textSchedule) {
			return <SchedulePage scheduleType={this.props.scheduleType} isEditMode={this.props.isEditMode} />;
		} else
			return null;
	}
}

export default TeamEditorNew;