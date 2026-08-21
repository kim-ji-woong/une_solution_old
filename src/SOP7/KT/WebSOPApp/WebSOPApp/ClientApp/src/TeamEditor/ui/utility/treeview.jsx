import React, { Component } from 'react';
import TreeNode from './treenode';
import '../../../Common/js/treeview.js';
import './css/style.css'; /* 사용중인것, 지우지마세요 */
/*import styles from '../../../Common/css/style.module.css';*/

class TreeView extends Component {
	static EventCheckedChanged = 1;

	constructor(props) {
		super(props);

		this.state = {
			teamTreeData: this.props.teamTreeData,
			useCheckBox: this.getCheckBoxType()
		}
	}

	getCheckBoxType() {
		if (this.props.useCheckBox !== null && this.props.useCheckBox !== undefined) {
			return this.props.useCheckBox;
		}

		return TreeNode.CheckBox_NotUse;
    }

	onTreeNodeChanged = (team, event) => {
		this.props.onTreeNodeChanged(team, event);
	}

	render() {

		if (this.props.teamTreeData && this.props.teamTreeData.length > 0) {
			return (
				<div className="scrollbar-outer">
					<ul className={'sarTree' + ' ' + 'treeview'/*styles.treeview*/} >
						{
							this.props.teamTreeData.map((data, index) => (
								(this.props.teamTreeData[index].Children === null || this.props.teamTreeData[index].Children === undefined || this.props.teamTreeData[index].length === 0)
									?
									<TreeNode key={data.ID}
										teamTreeData={data}
										onTreeNodeChanged={this.onTreeNodeChanged}
										isEditMode={this.props.isEditMode}
										editNodeID={this.props.editNodeID}
										editTeamInfo={this.props.editTeamInfo}
										useCheckBox={this.state.useCheckBox}
									/>
									:
									<li key={data.ID}>
										<TreeNode key={data.ID}
											teamTreeData={data}
											onTreeNodeChanged={this.onTreeNodeChanged}
											isEditMode={this.props.isEditMode}
											editNodeID={this.props.editNodeID}
											editTeamInfo={this.props.editTeamInfo}
											useCheckBox={this.state.useCheckBox}
										/>
									</li>
							))
						}
					</ul>
				</div>
			);
		}

		return <></>;
	}
}

export default TreeView;