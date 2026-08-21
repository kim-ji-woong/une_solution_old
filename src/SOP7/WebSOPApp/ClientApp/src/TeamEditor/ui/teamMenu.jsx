import React, { Component } from 'react';
import styles from '../../Common/css/style.module.css';
import '../../Common/js/treeview.js';
import $ from 'jquery';
import { TeamEditController } from '../services/teamEditController';
import TeamEditorResource from '../resource/id';
import TreeView from './utility/treeview';
import ProjectResource from '../../Root/resource/id';

class TeamMenu extends Component {
	static cssStyles = styles;

	constructor(props) {
		super(props);

		this.state = {			
			editNodeID: 0 /* 팀 이름 편집하기 위해 필요한거 editNodeID랑 같은 TeamID를 가진 Node가 텍스트박스로 변경된다 */
        }

		this.props = props;
	}

	onClickList(e) {
		var target = e;

		if ($(target).is('.' + TeamMenu.cssStyles.on)) {
			$(target).removeClass(TeamMenu.cssStyles.on);
			$(target).next().slideUp();
		} else {
			$(target).addClass(TeamMenu.cssStyles.on);
			$(target).next().slideDown();
		}

		return;
	}

	onClickTeam(teamType) {		
		this.props.onChangeTeamType(teamType);
		$('#btnTeamMenu').removeClass(TeamMenu.cssStyles.on);
		$('#btnTeamMenu').next().slideUp();
	}

	onTreeNodeChanged = (team, target) => {
		if (this.props.selectedTeam !== team) {
			this.props.onTeamNodeChanged(team);
		}
	}

	addRootTeam = async () => {
		if (!this.props.isEditMode)
			return;

		let name = "";

		if (this.props.teamType === TeamEditorResource.ID.textRegular) {
			name = "새 조직";
		} else if (this.props.teamType === TeamEditorResource.ID.textTemporary) {
			name = "새 비상조직";
		} else if (this.props.teamType === TeamEditorResource.ID.textTemporaryEmergency) {
			name = "새 휴일 비상조직";
		}

		const nodeData = { ID: -1, TeamName: name, ParentTeam: null, ParentTeamID: null, Children:[] };

		if (this.props.teamType === TeamEditorResource.ID.textRegular) {
			const [success, newID, message] = await TeamEditController.UpdateRegularTeam(nodeData);
			if (!success) {
				alert(message);
				return;
			}

			nodeData.ID = newID;
		}
		else if (this.props.teamType === TeamEditorResource.ID.textTemporary || this.props.teamType === TeamEditorResource.ID.textTemporaryEmergency) {
			if (this.props.teamType === TeamEditorResource.ID.textTemporary) {
				nodeData.IsNormal = true;
			}
			else {
				nodeData.IsNormal = false;
            }
			nodeData.SiteID = ProjectResource.SiteID;

			const [success, newID, message] = await TeamEditController.UpdateTemporaryTeam(nodeData);
			if (!success) {
				alert(message);
				return;
			}

			nodeData.ID = newID;
		}
		else {
			return;
        }

		const teamTreeData = this.props.teamTreeData;
		teamTreeData.push(nodeData);

		this.props.onUpdateTeamTreeData(teamTreeData);
    }

	addTeam = async () => {
		if (!this.props.isEditMode)
			return;

		let name = '';
		if (this.props.teamType === TeamEditorResource.ID.textRegular) {
			name = "새 조직";
		}
		else if (this.props.teamType === TeamEditorResource.ID.textTemporary) {
			name = "새 비상조직";
		}
		else if (this.props.teamType === TeamEditorResource.ID.textTemporaryEmergency) {
			name = "새 휴일 비상조직";
		}
		else {
			return;
		}

		const nodeData = { ID: -1, TeamName: name, ParentTeam: null, ParentTeamID: this.props.selectedTeam.ID, Children: [] };

		if (this.props.teamType === TeamEditorResource.ID.textRegular) {
			const [success, newID, message] = await TeamEditController.UpdateRegularTeam(nodeData);
			if (!success) {
				alert(message);
				return;
			}

			nodeData.ID = newID;
		}
		else if (this.props.teamType === TeamEditorResource.ID.textTemporary || this.props.teamType === TeamEditorResource.ID.textTemporaryEmergency) {
			if (this.props.teamType === TeamEditorResource.ID.textTemporary) {
				nodeData.IsNormal = true;
			}
			else {
				nodeData.IsNormal = false;
			}
			nodeData.SiteID = ProjectResource.SiteID;

			const [success, newID, message] = await TeamEditController.UpdateTemporaryTeam(nodeData);
			if (!success) {
				alert(message);
				return;
			}

			nodeData.ID = newID;
		}

		const findNode = TeamEditController.findParent(nodeData.ParentTeamID, this.props.teamTreeData);
		if (!findNode.Children)
			findNode.Children = [];
		findNode.Children.push(nodeData);

		this.props.onUpdateTeamTreeData(this.props.teamTreeData);
	}

	removeTeam = () => {
		if (!this.props.isEditMode)
			return;

		this.props.removeTeam();
	}

	editTeam = () => {
		if (!this.props.isEditMode)
			return;
				
		this.setState({ editNodeID: this.props.selectedTeam.ID });
	}

	editTeamInfo = (team, chgName) => {
		if (team !== null) {
			this.props.editTeam(team, chgName);
		}

		// 팀 이름 수정이 끝났으면 텍스트박스를 label tag로 바꿔주려고
		this.setState({ editNodeID: 0 });
    }

	render() {
		let editArea = null;
		if (this.props.isEditMode) {
			editArea =
				/* <div className={styles.sarEdit}>
					<a className={styles.left} onClick={this.editTeam}>수정</a>
					<a onClick={this.addTeam}>추가</a>
					<a onClick={this.removeTeam}>삭제</a>
				</div> */
				<div className={styles.sarEdit}>
					<a onClick={this.addRootTeam}></a>
				</div>
        }

		return (			
			<div className={styles.saRht}>
				<div className={styles.sarSel}>
					<button id="btnTeamMenu" onClick={(e) => this.onClickList(e.target)}>{this.props.teamType}</button>
					<ul>
						<li onClick={() => this.onClickTeam(TeamEditorResource.ID.textRegular)}><a>{TeamEditorResource.ID.textRegular}</a></li>
						<li onClick={() => this.onClickTeam(TeamEditorResource.ID.textTemporary)}><a>{TeamEditorResource.ID.textTemporary}</a></li>
						<li onClick={() => this.onClickTeam(TeamEditorResource.ID.textTemporaryEmergency)}><a>{TeamEditorResource.ID.textTemporaryEmergency}</a></li>
					</ul>
				</div>
				{editArea} 
				{/* 트리뷰 위치 */}
				<TreeView
					treeViewID="teamTree"
					teamTreeData={this.props.teamTreeData}
					onTreeNodeChanged={this.onTreeNodeChanged}
					isEditMode={this.props.isEditMode}
					editNodeID={this.state.editNodeID}
					editTeamInfo={this.editTeamInfo}
					selectedTeam={this.props.selectedTeam}
					addTeam={this.addTeam}
					editTeam={this.editTeam}
					removeTeam={this.removeTeam}
				/>
			</div>
        );
    }
}

export default TeamMenu;