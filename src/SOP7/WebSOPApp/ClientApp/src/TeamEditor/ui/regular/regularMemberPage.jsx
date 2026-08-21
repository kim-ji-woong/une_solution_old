import React, { Component } from 'react';
import { BrowserRouter as Route, Link } from 'react-router-dom';

import styles from '../../../Common/css/style.module.css';
import uneStyles from '../../../Common/css/uneCommon.module.css';
import teamEditors from '../../css/teamEditor.module.css';

import { Scrollbars } from 'react-custom-scrollbars-2';
import PageFooter from '../../../Root/pageFooter';
import { TeamEditController } from '../../services/teamEditController';
import ColRegularMemberNew from './colRegularMemberNew';
import ConfirmDialog from '../../../Common/ui/confirmDialog';

import $ from 'jquery';
import TeamEditorResource from '../../resource/id';
import ProjectResource from '../../../Root/resource/id';

//import CommandStyle from "../../services/commandStyle";

class RegularMemberPage extends Component {
	static cssStyles = styles;

	constructor(props) {
		super(props);
		this.state = {
			//selectedTeam: null,			// 선택된 팀
			displayMembers: null,		// 화면에 출력할 팀원 정보들 (검색에 활용)
			addIndex: -1, /* 새로 추가될 멤버의 ID (addIndex--; 되서 겹치지 않게 한다) */

			confirmMessage: {
				visible: false,
				title: "",
				messages: [""],
				buttons: ["확인"],
				onClose: this.onCloseConfirmDialog,
				onClickButton: null
			},
		};

		this.props = props;
		//this.displayMembers = this.props.displayMemberGridData;
		this.displayMembers = this.props.memberGridData;

		this.onClickRemoveMember = this.onClickRemoveMember.bind(this);
	}

	componentDidMount() {
		$('table').css({ 'width': '100%', 'border-spacing': '0', 'border-collapse': 'collapse', 'table-layout': 'fixed' });
	}

	componentDidUpdate(prevProps, prevState) {
		//if (this.props.displayMemberGridData !== prevProps.displayMemberGridData) {
		if (this.props.memberGridData !== prevProps.memberGridData) {
			//this.setState({ displayMembers: this.props.displayMemberGridData });
			this.setState({ displayMembers: this.props.memberGridData });
		}

		//if (this.props.selectedTeam !== prevProps.selectedTeam) {
		//	this.getRegularMember(this.props.selectedTeam);
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

	onClickAddMember = () => {
		const index = this.state.addIndex;
		this.onAddMember(index);
		this.setState({ addIndex: index - 1 });
	}

	async onAddMember(addIndex) {
		const index = addIndex;

		const member = new Object();
		member.ID = index;
		member.RegularID = this.props.selectedTeam.ID;
		member.MemberName = "새 인원";
		member.MemberID = null;
		member.OfficePhoneNumber = null;
		member.PhoneNumber = null;
		member.JobLevelID = 1;
		member.JobPositionID = 1;

		let members = this.props.memberGridData;
		let regularMembers = this.props.regularMembers;

		members.push(member);
		regularMembers.push(member);

		// 새 인원 css 효과
		$('#regularMemberTableBody').addClass(teamEditors.addPointer);
	}

	async onClickRemoveMember() {		
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
		//await this.props.onDeleteMember(members);
		this.onDeleteMember(deleteMembers);
	}

	onDeleteMember = async (deleteMembers) => {
		const [success, message] = await TeamEditController.RemoveRegularMembers(deleteMembers);
		if (!success && message.length > 0) {
			alert(message);
			return;
		}

		//const [members, regularMembers] = await this.makeRemoveRegularMember(deleteMembers, this.state.memberGridData);
		let curMembers = this.state.displayMembers;
		let members = this.props.memberGridData;
		let regularMembers = this.props.regularMembers;

		for (let i = 0; i < deleteMembers.length; i++) {
			for (let j = 0; j < members.length; j++) {
				if (members[j].ID === deleteMembers[i].ID) {
					members.splice(j, 1);
					break;
				}
			}

			for (let j = 0; j < regularMembers.length; j++) {
				if (regularMembers[j].ID === deleteMembers[i].ID) {
					regularMembers.splice(j, 1);
					break;
				}
			}

			for (let j = 0; j < curMembers.length; j++) {
				if (curMembers[j].ID === deleteMembers[i].ID) {
					curMembers.splice(j, 1);
					break;
				}
			}
		}

		this.setState({ displayMembers: curMembers });

		//this.setState({ memberGridData: members, displayMemberGridData: members, regularMembers: regularMembers });
		//this.setState({ memberGridData: members, regularMembers: regularMembers });
	}

	onClickSearch = () => {
		// 검색할 경우 member 정보를 검색어와 비교하여 rowContent에 push하기
		// 1. 팀원 정보 및 검색 단어 불러오기
		// 2. member 정보 조회 및 검색 단어와 비교하기
		// 3. 단어가 포함된다면 포함하기

		// 1. 팀원 정보 및 검색 단어 불러오기
		const members = this.props.memberGridData;
		const search = document.getElementById('search').value;
		let searchMembers = new Array();

		if (members == null || search == null) {
			return;
		}

		// 2. 팀원 정보와 검색 단어 비교하기
		for (let i = 0; i < members.length; i++) {
			const member = members[i];

			// 멤버 이름에서 검색
			if (member.MemberName != null) {
				let memberName = null;
				memberName = member.MemberName;

				if (memberName.indexOf(search) !== -1) {
					searchMembers.push(members[i]);
					continue;
				}
			}

			// 멤버 직급에서 검색
			if (member.JobLevelID != null || this.props.jobLevels[member.JobLevelID] != null) {
				let jobLevel = this.props.jobLevels[member.JobLevelID].name;

				if (jobLevel.indexOf(search) != -1) {
					searchMembers.push(members[i]);
					continue;
				}
			}

			// 멤버 직위에서 검색
			if (member.JobPositionID != null || this.props.jobPositions[member.JobPositionID] != null) {
				let jobPosition = this.props.jobPositions[member.JobPositionID].name;

				if (jobPosition.indexOf(search) != -1) {
					searchMembers.push(members[i]);
					continue;
				}
			}

			// 휴대전화번호에서 검색
			if (member.PhoneNumber != null) {
				let phoneNumber = member.PhoneNumber;

				if (phoneNumber.indexOf(search) != -1) {
					searchMembers.push(members[i]);
					continue;
				}
			}

			// 근무처 전화번호에서 검색
			if (member.OfficePhoneNumber != null) {
				let officePhoneNumber = member.OfficePhoneNumber;

				if (officePhoneNumber.indexOf(search) != -1) {
					searchMembers.push(members[i]);
					continue;
				}
			}
		}

		this.setState({ displayMembers: searchMembers });
		return;
	}

	onKeyPressSearch = (e) => {
		if (e.key === 'Enter') {
			this.onClickSearch();
		}

		return;
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
						<ColRegularMemberNew
							member={member}
							teamName={teamName}
							jobLevels={this.props.jobLevels}
							jobPositions={this.props.jobPositions}
							index={index}
							checkMemberID={this.props.checkMemberID}
							checkPhoneNumber={this.props.checkPhoneNumber}
							//onChange={this.props.onChangeMember}
							showConfirmDialog={this.showConfirmDialog}
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

		return (

			<div id={styles.subCont} className="scrollbar-outer">
				{/*<Scrollbars style={{ height: menuHeight }}>*/}
				<div className={teamEditors.scrollbar} style={{ height: menuHeight }} >
					<div className={styles.scWrap}>
						<div className={styles.scCont}>
							<div className={styles.scTop}>
								<h4>{teamName}</h4>
								<div className={styles.sctRht}>
									<div className={styles.sctSch}>
										<input id="search" type="text" onKeyPress={(e) => this.onKeyPressSearch(e)} placeholder={TeamEditorResource.ID.textFilter} title={TeamEditorResource.ID.textFilter} />
										<a onClick={this.onClickSearch}>{TeamEditorResource.ID.textSearch}</a>
									</div>
									{editArea}
								</div>
							</div>
							<table className={styles.scTb + " " + uneStyles.scTb}>
								<colgroup>
									<col style={{ width: '5%' }} />
									<col style={{ width: '5%' }} />
									<col style={{ width: '12%' }} />
									<col style={{ width: '12%' }} />
									<col style={{ width: '10%' }} />
									<col style={{ width: '10%' }} />
									<col style={{ width: '10%' }} />
									<col style={{ width: '9%' }} />
									<col style={{ width: '12%' }} />
									<col style={{ width: '15%' }} />
								</colgroup>
								<thead>
									<tr>
										<th>삭제</th>
										<th>번호</th>
										<th>소속팀</th>
										<th>이름</th>
										<th>직위</th>
										<th>직급</th>
										<th>휴대전화번호</th>
										<th>사번</th>
										<th>근무처 전화번호</th>
										<th>Email</th>
									</tr>
								</thead>
								<tbody id="regularMemberTableBody">

									{rowContent}

								</tbody>
							</table>
						</div>

						<PageFooter />

					</div>
					{/*</Scrollbars>*/}
				</div>
				{
					/* alert창 대신 사용 */
					this.state.confirmMessage.visible &&
					<ConfirmDialog title={this.state.confirmMessage.title} messages={this.state.confirmMessage.messages} buttons={this.state.confirmMessage.buttons} onClose={this.state.confirmMessage.onClose} onClickButton={this.state.confirmMessage.onClickButton} />
				}
			</div>

		);
	}
}

export default RegularMemberPage;