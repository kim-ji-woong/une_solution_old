import React, { Component } from 'react';

import scroll from '../Root/css/scroll.module.css';
import TreeViewstyles from './css/style.css';
import TreeNode from './treenode';

class TreeView extends Component {
	constructor(props) {
		super(props);

		this.state = {
			teamTreeData: this.props.teamTreeData,
			isEditMode: this.props.isEditMode
		}
		this.props = props;
		this.state.isEditMode = this.props.isEditMode;
		this.onTreeNodeChanged = this.onTreeNodeChanged.bind(this);
	}

	componentDidUpdate(prevProps, prevState) {
		if (this.props.isEditMode !== prevProps.isEditMode) {
			this.setState({ isEditMode: this.props.isEditMode });
		}
	}

	onTreeNodeChanged = (team) => {
		this.props.onTreeNodeChanged(team);
	}

	teamInfoChange = (team) => {
		this.props.teamInfoChange(team);
	}

	render() {

		if (this.props.teamTreeData === null || this.props.teamTreeData.length === 0) {
			return <></>;
        }

		return (
			<>
				<div className={scroll.scrollbarOuter}>
					<ul className={TreeViewstyles.sarTree + ' treeview'} id="abc" >
						{
							this.props.teamTreeData.map((data, index) => (
								(this.props.teamTreeData[index].children === null || this.props.teamTreeData[index].children === undefined)
									? <TreeNode key={data.ID}
										teamTreeData={data}
										onTreeNodeChanged={this.onTreeNodeChanged}
										teamInfoChange={this.props.teamInfoChange}
										isEditMode={this.state.isEditMode} />
									: <li key={data.ID}>
										<TreeNode key={data.ID}
											teamTreeData={data}
											onTreeNodeChanged={this.onTreeNodeChanged}
											teamInfoChange={this.props.teamInfoChange}
											isEditMode={this.state.isEditMode} />
									  </li>
							))
						}
					</ul>
				</div>
			</>
		);
	}
}

export default TreeView;