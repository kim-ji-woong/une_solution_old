import React, { Component } from 'react';
import { Scrollbars } from 'react-custom-scrollbars-2';
import $ from 'jquery';

import TreeView from '../utility/treeview';
import { TeamEditController } from '../../services/teamEditController';
import SessionString from '../../../Common/js/sessionString';
import ColSelectManager from './colSelectManager';

import styles from '../../../Common/css/style.module.css';
import teamEditors from '../../css/teamEditor.module.css';
import uneStyles from '../../../Common/css/uneCommon.module.css';

import TeamEditorResource from '../../resource/id';


class PopupSelectManager extends Component {
	static cssStyles = styles;

	constructor(props) {
		super(props);

		this.state = {
			teamTreeData: null,				// 정규조직 정보
			selectedTeam: null,				// 팝업창에서 선택한 팀
			selectedMember: null,
			gridMembers: null,				// 선택된 정규조직 멤버
			displayMembers: null,			// 팝업창에서 표시된 정규조직 멤버
			//popupMember: null,				// 해당 비상조직 멤버 정보
        }

		this.props = props;

	}

	componentDidMount() {
		$('table').css({ 'width': '100%', 'border-spacing': '0', 'border-collapse': 'collapse', 'table-layout': 'fixed' });

		//this.displayRegular();
		this.initManager();
	}

	onClickClose = () => {
		this.props.close();
	}

	//displayRegular() {

	//	const data = this.props.regularTreeData;
	//	if (data === null || data === undefined)
	//		return;



	//	if (data.length > 0) {
	//		this.setState({ selectedTeam: data[0] }, () => this.displayRegularMember());
	//	}
	//}

	initManager() {
		const popupMember = this.props.popupMember;

		if (popupMember === null || popupMember === undefined)
			return;

		let regular = null;
		let regularMember = null;

		if (popupMember.regular !== null && popupMember.regular !== undefined) {
			regular = new Object();
			regular.ID = popupMember.regular.id;
			regular.TeamName = popupMember.regular.teamName;
			regular.ParentTeamID = popupMember.regular.parentTeamID;
		}

		if (popupMember.regularMember !== null && popupMember.regularMember !== undefined) {
			regularMember = new Object();
			regularMember.ID = popupMember.regularMember.id;
			regularMember.MemberID = popupMember.regularMember.memberID;
			regularMember.MemberName = popupMember.regularMember.memberName;
			regularMember.JobLevelID = popupMember.regularMember.jobLevelID; 
			regularMember.JobPositionID = popupMember.regularMember.jobPositionID; 
			regularMember.Email = popupMember.regularMember.email;
			regularMember.PhoneNumber = popupMember.regularMember.phoneNumber; 
			regularMember.OfficePhoneNumber = popupMember.regularMember.officePhoneNumber; 
			regularMember.RegularID = popupMember.regularMember.regularID;
		}

		let selectedTeam = null;
		if (regular && regular !== null) {
			selectedTeam = regular;
		}
		else if (this.props.regularTreeData && this.props.regularTreeData.length > 0) {
			selectedTeam = this.props.regularTreeData[0];
        }

		this.state.selectedTeam = selectedTeam;
		this.state.selectedMember = regularMember;

		this.displayRegularMember();
    }

	displayRegularMember() {
		const members = this.props.regularMembers;
		if (members === null || members === undefined) {
			return;
		}

		let displayMembers = [];

		if (this.state.selectedTeam !== null && this.state.selectedTeam) {
			for (let i = 0; i < members.length; i++) {
				if (this.state.selectedTeam.ID === members[i].RegularID) {
					let member = members[i];
					member.check = false;

					if (this.state.selectedMember && this.state.selectedMember !== null && this.state.selectedMember.ID === member.ID) {
						member.check = true;
					}

					displayMembers.push(member);
				}
			}
		}

		this.setState({ gridMembers: displayMembers, displayMembers: displayMembers });
	}

	onTreeNodeChanged = (team) => {
		// 선택된 멤버 해제
		this.state.selectedMember = null;

		this.state.selectedTeam = team;
		this.displayRegularMember();
	}

	onClickSearch = () => {

		// 1. 팀원 정보 및 검색 단어 불러오기
		const members = this.state.gridMembers;
		const search = document.getElementById('searchManager').value;
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

	onClickRow = (index) => {
		//console.log(this.state.displayMembers[index].MemberName);
		//let newDisplayMebers = new Array();
		let selectedMember = this.state.selectedMember;

		for (let i = 0; i < this.state.displayMembers.length; i++) {
			let member = this.state.displayMembers[i];
			member.check = false;

			if (i == index) {
				member.check = true;

				selectedMember = member;
				//selectedMember.RegularMemberID = member.ID;
				//selectedMember.RegularMemberName = member.MemberName;
            }
				
			//newDisplayMebers.push(member);
		}

		//this.setState({ displayMembers: newDisplayMebers, selectedMember: selectedMember});
		this.setState({ selectedMember: selectedMember});
	}

	onClickSelect = () => {
		// props 멤버 정보를 수정
		let popupMember = this.props.popupMember;
		let selectedTeam = this.state.selectedTeam;
		let selectedMember = this.state.selectedMember;
		
		popupMember.regular = null;
		popupMember.regularMember = null;

		if (selectedTeam !== null && selectedTeam !== undefined) {
			let regular = this.makeRegularData(selectedTeam);
			popupMember.regular = regular;
		}

		if (selectedMember !== null && selectedMember !== undefined) {
			let regularMember = this.makeRegularMemberData(selectedMember);
			popupMember.regularMember = regularMember;
		}

		this.props.onChangeMember(popupMember, true);
		
		this.onClickClose();
	}

	makeRegularData(selectedTeam) {
		let regular = new Object();
		regular.id = selectedTeam.ID;
		regular.teamName = selectedTeam.TeamName;
		regular.parentTeamID = selectedTeam.ParentTeamID;

		return regular;
	}

	makeRegularMemberData(selectedMember) {
		let regularMember = new Object();
		regularMember.id = selectedMember.ID;
		regularMember.memberID = selectedMember.MemberID;
		regularMember.memberName = selectedMember.MemberName;
		regularMember.jobLevelID = selectedMember.JobLevelID;
		regularMember.jobPositionID = selectedMember.JobPositionID;
		regularMember.email = selectedMember.Email;
		regularMember.phoneNumber = selectedMember.PhoneNumber;
		regularMember.officePhoneNumber = selectedMember.OfficePhoneNumber;
		regularMember.regularID = selectedMember.RegularID;

		return regularMember;
    }

	render() {
		let teamName = "";
		if (this.state.selectedTeam !== null && this.state.selectedTeam !== undefined)
			teamName = this.state.selectedTeam.TeamName;

		// 헤더와 바텀 높이를 제외한 높이를 가져와 스크롤 높이 넣기
		let scpRht = $('.' + styles.scpRht);
		let scprTop = $('.' + styles.scprTop);
		let scprBot = $('.' + styles.scprBot);
		let menuHeight = 0;

		if (scpRht[0] != null && scprTop[0] != null && scprBot[0] != null) {
			scpRht = scpRht[0].clientHeight;
			scprTop = scprTop[0].clientHeight;
			scprBot = scprBot[0].clientHeight;

			menuHeight = scpRht - scprTop - scprBot;
		}

		const rowContent = [];

		if (this.state.displayMembers !== null) {
			this.state.displayMembers.map((member, index) =>
				(
					rowContent.push(
						<tr key={Math.random()} onClick={() => this.onClickRow(index)}>
							<ColSelectManager
								member={member}
								teamName={teamName}
								jobLevels={this.props.jobLevels}
								jobPositions={this.props.jobPositions}
								index={index}
							/>
						</tr>
					)
				))
		} 

		return (
			<div className={styles.scPop}>
				<div>
					<div>
						<div className={styles.scpWrap + " " + styles.w950}>
							<div className={styles.scpTop + " " + uneStyles.scpTop}>
								<h3>{TeamEditorResource.ID.textSetTeamManager}</h3>
								<a onClick={this.onClickClose}>닫기</a>
							</div>
							<div className={styles.scpCont}>
								<div className={styles.scpLft + " popupMenu"}>
									<h4 className={styles.scplTitle}>{TeamEditorResource.ID.textSelectTeam}</h4>
									<div className={styles.scplOgz + " scrollbar-outer"}>

										{/* 트리뷰 위치 */}
										<TreeView treeViewID="temporaryPopupTree" teamTreeData={this.props.regularTreeData} onTreeNodeChanged={this.onTreeNodeChanged} />
	
									</div>
								</div>
								<div className={styles.scpRht}>
									<div className={styles.scprTop + " " + uneStyles.scprTop}>
										<h4>{teamName}</h4>
										<div className={teamEditors.scprTopForm}>
											<input id="searchManager" type="text" placeholder={TeamEditorResource.ID.textFilter} title={TeamEditorResource.ID.textFilter} onKeyPress={(e) => this.onKeyPressSearch(e)} />
											<a onClick={this.onClickSearch}>{TeamEditorResource.ID.textSearch}</a>
										</div>
									</div>
									{/*<div className="scrollbar-outer">*/}
									<div className={teamEditors.scrollbar} style={{ height: menuHeight }} >
										<div className={styles.scprCont}>
											<table className={styles.scprTb}>
												<caption>선택, 번호, 소속팀, 이름, 직위, 직급, 휴대정화번호, 사번으로 구성된 표</caption>
												<colgroup>
													<col style={{ width: "7%" }} />
													<col style={{ width: "7%" }} />
													<col style={{ width: "10%" }} />
													<col style={{ width: "20%" }} />
													<col style={{ width: "10%" }} />
													<col style={{ width: "10%" }} />
													<col style={{ width: "20%" }} />
													<col style={{ width: "16%" }} />
												</colgroup>
												<thead>
													<tr>
														<th>선택</th>
														<th>번호</th>
														<th>소속팀</th>
														<th>이름</th>
														<th>직위</th>
														<th>직급</th>
														<th>휴대전화번호</th>
														<th>사번</th>
													</tr>
												</thead>
												<tbody>

													{rowContent}

												</tbody>
											</table>
										</div>
									</div>
									<div className={styles.scprBot + " " + uneStyles.scprBot}>
										<a onClick={this.onClickSelect} className={styles.red + " " + uneStyles.navy}>{TeamEditorResource.ID.textSelect}</a>
										<a onClick={this.onClickClose}>{TeamEditorResource.ID.textCancle}</a>
									</div>
								</div>
							</div>
						</div>
					</div>
				</div>
			</div>
		);
    }
}

export default PopupSelectManager;