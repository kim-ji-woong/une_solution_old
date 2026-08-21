import React, { Component } from 'react';
import styles from '../Root/css/style.module.css';
import $ from 'jquery';
import './js/treeview.js';
import TreeViewstyles from './css/style.css';

class TreeNode extends Component {
	constructor(props) {
		super(props);

		this.state = {
			teamTreeData: this.props.teamTreeData,
			nodeEdit: false,
			isEditMode: this.props.isEditMode
		}

		this.props = props;
		this.state.isEditMode = this.props.isEditMode;
		this.onTreeNodeChanged = this.onTreeNodeChanged.bind(this);
	}

	componentDidMount() {
		$(document).ready(function () {
			$('.' + TreeViewstyles.treeview).hummingbird();
		})
	}

	componentDidUpdate(prevProps, prevState) {
		if (this.props.isEditMode !== prevProps.isEditMode) {
			this.setState({ isEditMode: this.props.isEditMode });
		}

		// 편집중이다가 편집모드가 종료되면 
		if (!this.props.isEditMode && this.state.nodeEdit) {
			this.setState({ nodeEdit: false });
        }
	}

	onTreeNodeChanged = (e) => {
		if (e.nativeEvent.which === 1) {
			this.props.onTreeNodeChanged(this.props.teamTreeData);

			// 선택된 노드 구별 클래스(active) 추가
			let actives = $('.active');
			for (let i = 0; i < actives.length; i++) {
				const active = actives[i];
				$(active).removeClass("active");
			}
			let target = e.target;
			$(target).addClass("active");
		}
		else {
			if (this.state.isEditMode) {
				e.preventDefault(); // web 우클릭 메뉴 숨기기
				if (!this.state.nodeEdit) {
					this.setState({ nodeEdit: true });
				}
			}
		}
	}

	onBlur(name) {
		if (name !== this.props.teamTreeData.Name) {
			const chgData = this.props.teamTreeData;
			chgData.Name = name;

			this.setState({ teamTreeData: chgData, nodeEdit: false });
			this.props.teamInfoChange(chgData);
		}
		else {
			this.setState({ nodeEdit: false });
		}
	}

	teamInfoChange = (team) => {
		this.props.teamInfoChange(team);
	}

	handleKeyPress = (e) => {

		if (e.key === "Enter") {
			this.onBlur(e.target.value);
		}
	}

	render() {

		let output = null;
		
		if (this.props.teamTreeData === null || this.props.teamTreeData === undefined) {
			return <></>;
		}

		let haveChildren = null;

		if (this.props.teamTreeData.children === null || this.props.teamTreeData.children === undefined) {
			haveChildren = false;
			output =
			<>
				{
					(this.props.isEditMode && this.state.nodeEdit) ?
						<a className={styles.a}>
							<input type="text"
								defaultValue={this.props.teamTreeData.Name}
								onBlur={(e) => this.onBlur(e.target.value)}
								onKeyPress={this.handleKeyPress}
								autoFocus />
						</a>
						:
						<a onClick={(e) => this.onTreeNodeChanged(e)}
							onContextMenu={(e) => this.onTreeNodeChanged(e)}
							className={styles.a}>
							{this.props.teamTreeData.Name}
						</a>
                }
			</>
		}
		else {
			haveChildren = true;
			output =
				<>
				<i className="fa-minus icon-white" > 더보기</i>
				{
					(this.props.isEditMode && this.state.nodeEdit) ?
						<h5>
							<input type="text"
								defaultValue={this.props.teamTreeData.Name}
								onBlur={(e) => this.onBlur(e.target.value)}
								onKeyPress={this.handleKeyPress}
								autoFocus />
						</h5>
						:
						<h5 onClick={this.onTreeNodeChanged}
							onContextMenu={(e) => this.onTreeNodeChanged(e)}>{this.props.teamTreeData.Name}</h5>
					}
				</>
        }

		return (	
			<>
				{output}				
				{
					(haveChildren) ?
						<ul>
							{
								(this.props.teamTreeData.children === null || this.props.teamTreeData.children === undefined)
									? <></>
									: this.props.teamTreeData.children.map((data, index) => (
										<li key={data.ID}>
											<TreeNode key={data.ID}
												teamTreeData={data}
												onTreeNodeChanged={this.props.onTreeNodeChanged}
												teamInfoChange={this.props.teamInfoChange}
												isEditMode={this.state.isEditMode} />											
										</li>
									))
							}
						</ul>
						: <></>
				}
			</>
		);
	}
}

export default TreeNode;
