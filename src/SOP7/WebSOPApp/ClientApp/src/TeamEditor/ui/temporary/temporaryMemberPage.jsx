import React, { Component } from 'react';
import { BrowserRouter as Route, Link } from 'react-router-dom';
import { Scrollbars } from 'react-custom-scrollbars-2';

import styles from '../../../Common/css/style.module.css';
import uneStyles from '../../../Common/css/uneCommon.module.css';
import teamEditors from '../../css/teamEditor.module.css';

import PageFooter from '../../../Root/pageFooter';
import { TeamEditController } from '../../services/teamEditController';
import ColTemporaryMemberNew from './colTemporaryMemberNew';
import PopupSelectManager from './popupSelectManager';

import $ from 'jquery';

import TeamEditorResource from '../../resource/id';
import ProjectResource from '../../../Root/resource/id';

class TemporaryMemberPage extends Component {
	static cssStyles = styles;

	constructor(props) {
		super(props);
		this.state = {
			selectedTeam: null,			// 선택된 팀
			//teamType: null,				// 평일 혹은 휴일 비상조직 체크
			members: null,				// 선택된 팀원 정보들
			displayMembers: null,		// 화면에 출력할 팀원 정보들 (검색에 활용)
			roles: null,				// 정/부 정보들 (JSON {{value: "value값", name: "name값"}...})
			openPopup: false,			// 팝업창(조직 담당자 설정) 오픈 여부
			popupMember: null,			// 팝업창(조직 담당자 설정) 파라미터 >> 설정된 담당자
			addIndex: -1, /* 새로 추가될 멤버의 ID (addIndex--; 되서 겹치지 않게 한다) */
		};

		this.props = props;
		this.displayMembers = this.props.memberGridData;

		// 정/부 ColComboBox 데이터형 만들기
		this.initRoles();
	}

	initRoles() {
		const arrRoles = new Array();

		// 정/부 ColComboBox 데이터형 만들기 >> JSON {{value: "value값", name: "name값"}...}
		let item = new Object();
		item.value = "";
		item.name = "알수 없음";
		arrRoles.push(item);

		item = new Object();
		item.value = 0;
		item.name = "정";
		arrRoles.push(item);

		item = new Object();
		item.value = 1;
		item.name = "부";
		arrRoles.push(item);

		item = new Object();
		item.value = 2;
		item.name = "일반";
		arrRoles.push(item);

		this.state.roles = arrRoles;
	}

	componentDidMount() {
		$('table').css({ 'width': '100%', 'border-spacing': '0', 'border-collapse': 'collapse', 'table-layout': 'fixed' });
	}

	componentDidUpdate(prevProps, prevState) {
		//if (this.props.selectedTeam !== prevProps.selectedTeam) {
		//	const isNormal = (this.props.teamType == TeamEditorResource.ID.textTemporary ? true : false);
		//	this.getTemporaryMember(this.props.selectedTeam, isNormal);
		//}

		//if (this.props.teamType !== prevProps.teamType) {
		//	//this.state.teamType = this.props.teamType;
		//	const isNormal = (this.props.teamType == TeamEditorResource.ID.textTemporary ? true : false);
		//	this.getTemporaryMember(this.props.selectedTeam, isNormal);
		//}

		if (this.props.memberGridData !== prevProps.memberGridData) {
			this.setState({ displayMembers: this.props.memberGridData });
		}
	}

	async getTemporaryMember(team, isNormal) {
		let members = new Array();

		if (team != null) 
			members = await TeamEditController.displayTemporaryMember(team.ID, isNormal);	// 해당 팀원 불러오기

		if (members !== null) {
			this.setState({ members: members, displayMembers: members });
	  }
	}

	onClickAddMember = () => {		
		const index = this.state.addIndex;
		this.onAddMember(index);
		this.setState({ addIndex: index - 1 });
	}

	onAddMember(addIndex) {
		const index = addIndex;

		const isNormal = (this.props.teamType == TeamEditorResource.ID.textTemporary ? 1 : 0);
		const selectedTeam = this.props.selectedTeam;
		const temporary = this.makeTemporaryData(selectedTeam, isNormal);

		const member = new Object();
		member.id = index;
		member.isNormal = isNormal;
		member.displaySOPName = "새 인원";
		member.regular = null;
		member.regularMember = null;
		member.role = null;
		member.temporary = temporary;

		let members = this.props.memberGridData;
		let temporaryMembers = this.props.temporaryMembers;

		members.push(member);
		temporaryMembers.push(member);

		// 새 인원 css 효과
		$('#temporaryMemberTableBody').addClass(teamEditors.addPointer);
	}

	makeTemporaryData(selectedTeam, isNormal) {
		let temporary = new Object();
		temporary.id = selectedTeam.ID;
		temporary.teamName = selectedTeam.TeamName
		temporary.isNormal = isNormal === 1 ? true : false;
		temporary.parentTeamID = selectedTeam.ParentTeamID;

		return temporary;
    }

	onClickRemoveMember = () => {
		let curMembers = this.state.displayMembers;

		// 삭제할 인원 분류
		const deleteMembers = [];
		for (let i = 0; i < curMembers.length; i++) {
			if (curMembers[i].check) {
				//await this.props.onDeleteMember(curMembers[i]);
				deleteMembers.push(curMembers[i]);
			}
		}

		// 삭제할 인원 데이터 삭제
		this.onDeleteMember(deleteMembers);
	}

	onDeleteMember = async (deleteMembers) => {
		const [success, message] = await TeamEditController.RemoveTemporaryMembers(deleteMembers);
		if (!success && message.length > 0) {
			alert(message);
			return;
		}

		let curMembers = this.state.displayMembers;
		let members = this.props.memberGridData;
		let temporaryMembers = this.props.temporaryMembers;

		for (let i = 0; i < deleteMembers.length; i++) {
			for (let j = 0; j < members.length; j++) {
				if (members[j].id === deleteMembers[i].id) {
					members.splice(j, 1);
					break;
				}
			}

			for (let j = 0; j < temporaryMembers.length; j++) {
				if (temporaryMembers[j].id === deleteMembers[i].id) {
					temporaryMembers.splice(j, 1);
					break;
				}
			}

			for (let j = 0; j < curMembers.length; j++) {
				if (curMembers[j].id === deleteMembers[i].id) {
					curMembers.splice(j, 1);
					break;
				}
			}
		}

		this.setState({ displayMembers: curMembers });

		//this.setState({ memberGridData: members, displayMemberGridData: members, regularMembers: regularMembers });
		//this.setState({ memberGridData: members, regularMembers: regularMembers });
	}

	onChangeMember = (index, member) => {
		let members = this.state.members;
		members[index] = member;

		this.setState({ members: members, displayMembers: members });
		return;
	}

	onClickSearch = () => {
		
		return;
	}

	openPopup = (member) => {
		this.setState({ openPopup: true, popupMember: member });

		return;
	}

	closePopup = () => {
		this.setState({ openPopup: false });

		return;
	}

	onChangeSelect = (member) => {
		let members = this.state.members;

		for (let i = 0; i < members.length; i++) {
			let oldMember = members[i];
			
			if (oldMember.TemporaryMemberID === member.TemporaryMemberID) {
				members[i] = member;
            }
		}

		this.props.onChangeMember(member, true);

		this.setState({ /*members: members, */openPopup: false });
	}

	openPopupSelectManager = () => {
		let popupSelectManagerUI = null;

		if (this.state.openPopup === true) {
			popupSelectManagerUI = <>
				<PopupSelectManager
					popupMember={this.state.popupMember}
					close={this.closePopup}
					select={this.onChangeSelect}
					jobLevels={this.props.jobLevels}
					jobPositions={this.props.jobPositions}
					regularTreeData={this.props.regularTreeData}
					regularMembers={this.props.regularMembers}
					onChangeMember={this.props.onChangeMember}
				/>
			</>;
		}

		return popupSelectManagerUI;
    }

	render() {
		let teamName = "";
		if (this.props.selectedTeam !== null && this.props.selectedTeam !== undefined &&
			this.props.selectedTeam.TeamName !== null && this.props.selectedTeam.TeamName !== undefined)
			teamName = this.props.selectedTeam.TeamName;

		// 왼쪽 메뉴 높이 가져와 스크롤 높이 넣기
		const target = $('.pageMenu');
		let menuHeight = 0;

		if (target[0] != null) {
			menuHeight = target[0].clientHeight;
        }

		const rowContent = [];

		if (this.state.displayMembers !== null) {
			this.state.displayMembers.map((member, index) =>
				(
					rowContent.push(
						<tr key={Math.random()}>
							<ColTemporaryMemberNew
								member={member}
								jobPositions={this.props.jobPositions}
								roles={this.state.roles}
								index={index}
								openPopup={this.openPopup}
								onChangeMemberEditMode={this.props.onChangeMemberEditMode}
								onChangeMember={this.props.onChangeMember}
							/>
						</tr>
					)
				))
		} 

		let editArea = null;
		if (this.props.isEditMode) {
			editArea =
				<>
					<a onClick={this.onClickAddMember} className={styles.sctAdd}>추가</a>
					<a onClick={this.onClickRemoveMember} className={styles.sctDel}>삭제</a>
					{
						/*
						<a href="#" className={styles.sctDwn}>엑셀 다운로드</a>
						<a href="#" className={styles.sctUld}>엑셀 업로드</a>
						*/
					}
				</>
		}
		else {
			editArea = null;
		}

		let popupSelectManagerUI = this.openPopupSelectManager();

		return (
			<>
				<div id={styles.subCont} className={uneStyles.subContt}>
					{/*<Scrollbars style={{ height: menuHeight }}>*/}
					<div className={teamEditors.scrollbar} style={{ height: menuHeight }} >
					<div className={styles.scWrap + " " + uneStyles.scWrapp}>
						<div className={styles.scCont}>
							<div className={styles.scTop + " " + uneStyles.scTopp}>
							<h4>{teamName}</h4>
									<div className={styles.sctRht + " " + uneStyles.sctRht}>
								<form action="" className={styles.sctSch + " " + uneStyles.sctSch}>
									<input id="search" type="text" placeholder="검색어 입력" title="검색어 입력" />
									<a onClick={this.onClickSearch} >검색</a>
								</form>
								{editArea}
							</div>
						</div>
					    <table className={styles.scTb + " " + uneStyles.scTbb}>
							<colgroup>
								<col style={{ width: '5%' }} />
								<col style={{ width: '5%' }} />
								<col style={{ width: '10%' }} />
								<col style={{ width: '25%' }} />
								<col style={{ width: '25%' }} />
								<col style={{ width: '10%' }} />
								<col style={{ width: '20%' }} />
							</colgroup>
							<thead>
								<tr>
									<th>삭제</th>
									<th>번호</th>
									<th>정/부</th>
									<th>SOP 표시이름</th>
									<th>부서명</th>
									<th>직위</th>
									<th>성명</th>
								</tr>
							</thead>
							<tbody id="temporaryMemberTableBody">
								
								{rowContent}

							</tbody>
						</table>
					</div>

					<PageFooter />

				</div>
				</div>
			</div>

			{popupSelectManagerUI}

			</>
		);
    }
}

export default TemporaryMemberPage;