import React, { Component } from 'react';
import { Container } from 'reactstrap';
import $ from 'jquery';

import newStyles from '../../../Common/css/newStyle.module.css';
import newDefaults from '../../../Common/css/newDefault.module.css';
import settings from '../../css/settings.module.css';

class SelectReceiver extends Component {
    constructor(props) {
		super(props);

		this.state = {
			selectRegularID: null,
			arrRegularID: null,				// 리시버 팀 기존 리스트
			arrRegularMemberID: null,		// 리시버 팀원 기존 리스트
			arrNewRegularID: null,			// 리시버 팀 수정 리스트
			arrNewRegularMemberID: null,	// 리시버 팀원 수정 리스트
			addRegularID: null,				// 선택된 팀
			addRegularMemberID: [],			// 선택된 팀원 리스트
			removeReceiverList:[],			// 삭제할 리스트
			displayReceiverList: [],		// 리시버 리스트 (팀, 팀원 다 포함)
        }

		this.props = props;

		const teamTreeDatas = this.props.teamTreeDatas;
		const teams = this.props.teams;
		const members = this.props.members;
		const facilityType = this.props.facilityType;
		const buildingGroup = this.props.buildingGroup;
		const building = this.props.building;
		const regularID = this.props.regularID;
		const regularMemberID = this.props.regularMemberID;

		this.initData();
	}

	componentDidMount() {
		//this.initData();

		// 트리 열고 닫기
		$('.' + newStyles.dsiTree + ' h5 span').click(function () {
			if ($(this).is('.' + newStyles.on)) {
				$(this).removeClass(newStyles.on);
				$(this).parent().next().hide();
			} else {
				$(this).addClass(newStyles.on);
				$(this).parent().next().show();
			};
		});

		// 트리 선택
		$('.' + newStyles.dsiTree + ' h5 span').click(function () {
			let targets = $('.' + newStyles.dsiTreeCheck);

			if (targets !== null && targets !== undefined && targets.length > 0) {
				for (let i = 0; i < targets.length; i++) {
					let target = targets[i];

					$(target).removeClass(newStyles.dsiTreeCheck);
                }
			}

			$(this).addClass(newStyles.dsiTreeCheck);
		});

		// 트리 선택
		$('.' + newStyles.dsiTree + ' li a').click(function () {
			let targets = $('.' + newStyles.dsiTreeCheck);

			if (targets !== null && targets !== undefined && targets.length > 0) {
				for (let i = 0; i < targets.length; i++) {
					let target = targets[i];

					$(target).removeClass(newStyles.dsiTreeCheck);
				}
			}

			$(this).addClass(newStyles.dsiTreeCheck);
		});
	}

	initData = () => {
		const regularID = this.props.regularID;
		const regularMemberID = this.props.regularMemberID;
		let arrRegularID = null;
		let arrRegularMemberID = null;
		const teams = this.props.teams;
		const members = this.props.members;

		if (regularID !== null && regularID !== undefined) 
			arrRegularID = regularID.split(",");

		if (regularMemberID !== null && regularMemberID !== undefined)
			arrRegularMemberID = regularMemberID.split(",");

		let displayReceiverList = [];
		let idx = 1;

		if (arrRegularID !== null && arrRegularID !== undefined && arrRegularID.length > 0 &&
			teams !== null && teams !== undefined) {

			for (let j = 0; j < arrRegularID.length; j++) {
				const regularID = arrRegularID[j];

				for (let i = 0; i < teams.length; i++) {
					const team = teams[i];

					if (regularID === team.id.toString()) {
						/*
						displayReceiverList.push(
							<tr>
								<td>{idx}</td>
								<td><a>{team.teamName}</a></td>
							</tr>
						);
						*/

						displayReceiverList.push("team_" + team.id.toString());

						idx++;
						break;
					}
				}
			}

		}

		if (arrRegularMemberID !== null && arrRegularMemberID !== undefined && arrRegularMemberID.length > 0 &&
			members !== null && members !== undefined) {

			for (let i = 0; i < arrRegularMemberID.length; i++) {
				const regularMemberID = arrRegularMemberID[i];

				for (let j = 0; j < members.length; j++) {
					const member = members[j];

					if (regularMemberID === member.ID.toString()) {
						/*
						displayReceiverList.push(
							<tr>
								<td>{idx}</td>
								<td><a>{member.MemberName}</a></td>
							</tr>
						);
						*/

						displayReceiverList.push("member_" + member.ID.toString());

						idx++;
						break;
					}
				}
			}
		}

		this.state.arrRegularID = arrRegularID;
		this.state.arrNewRegularID = arrRegularID;
		this.state.arrRegularMemberID = arrRegularMemberID;
		this.state.arrNewRegularMemberID = arrRegularMemberID;
		this.state.displayReceiverList = displayReceiverList;
    }

	onClickConfirm = () => {
		const arrNewRegularID = this.state.arrNewRegularID;
		const arrRegularMemberID = this.state.arrRegularMemberID;
		let propsRegularID = this.props.regularID;
		let propsRegularMemberID = this.props.regularMemberID;

		let regularID = "";
		let regularMemberID = "";

		for (let i = 0; i < arrNewRegularID.length; i++) {
			let newRegularID = arrNewRegularID[i];

			if (regularID === "") 
				regularID = newRegularID.toString();
			else 
				regularID = regularID + "," + newRegularID.toString();
		}

		for (let i = 0; i < arrRegularMemberID.length; i++) {
			let newRegularMemberID = arrRegularMemberID[i];

			if (regularMemberID === "")
				regularMemberID = newRegularMemberID.toString();
			else 
				regularMemberID = regularMemberID + "," + newRegularMemberID.toString();
		}

		this.props.onClickConfirm(regularID, regularMemberID);
	}

	onClickClose = () => {
		this.props.onClickClose();
	}

	getInfo = () => {
		const facilityType = this.props.facilityType;
		const buildingGroup = this.props.buildingGroup;
		const building = this.props.building;

		let location = "";

		if (buildingGroup === "" && building === "")
			location = "전체"
		else
			location = buildingGroup + " " + building;

		return [facilityType, location];
	}

	displayTreeView = () => {

		const teamTreeDatas = this.props.teamTreeDatas;
		let displayTreeViewUI = [];

		if (teamTreeDatas === null || teamTreeDatas === undefined || teamTreeDatas.length < 1)
			return displayTreeViewUI;

		for (let i = 0; i < teamTreeDatas.length; i++) {
			let node = teamTreeDatas[i];

			if (node.Children !== null && node.Children !== undefined && node.Children.length > 0) {
				displayTreeViewUI.push(this.getParentNode(node));
			} else {
				displayTreeViewUI.push(this.getChildNode(node));
            }
        }

		return displayTreeViewUI;
	}

	getParentNode = (node) => {
		let name = node.Name;
		let id = node.ID;

		let childTree = [];

		for (let i = 0; i < node.Children.length; i++) {
			let child = node.Children[i];

			if (child.Children !== null && child.Children !== undefined && child.Children.length > 0) {
				childTree.push(this.getParentNode(child));
			} else {
				childTree.push(this.getChildNode(child));
            }
		}

		return (<li key={"parentNode_" + id}>
			<h5><span id={id} onClick={() => this.selectRegular(id)}> {name} </span></h5>
			<ul>{childTree}</ul>
		</li>);
	}

	getChildNode = (node) => {
		let name = node.Name;
		let id = node.ID;

		return (<li key={"childNode_" + id}><a id={id} onClick={() => this.selectRegular(id)}> {name} </a></li>);
    }

	selectRegular = (id) => {
		console.log("selectRegular ID: " + id);

		// 선택된 팀원 체크 해제
		let targets = $('.' + settings.regularMemberCheck);

		if (targets !== null && targets !== undefined && targets.length > 0) {
			for (let i = 0; i < targets.length; i++) {
				let target = targets[i];

				$(target).removeClass(settings.regularMemberCheck);
			}
		}

		this.setState({ selectRegularID: id, addRegularID: id, addRegularMemberID: [] });
	}

	displayRegularMemberList = () => {
		const RegularID = this.state.selectRegularID;
		const members = this.props.members;

		let displayRegularMemberList = [];

		if (RegularID === null || RegularID === undefined)
			return displayRegularMemberList;

		if (members !== null && members !== undefined) {
			let j = 1;

			for (let i = 0; i < members.length; i++) {
				const member = members[i];

				if (member.RegularID === RegularID) {
					displayRegularMemberList.push(
						<tr>
							<td>{j}</td>
							<td><a onClick={(e) => this.selectRegularMember(e, member.ID)}>{member.MemberName}</a></td>
						</tr>
					);
					j++;
				}
			}
		}

		return displayRegularMemberList;
	}

	selectRegularMember = (e, id) => {
		console.log("selectRegularMember ID: " + id);

		// 선택된 팀 해제
		this.state.addRegularID = null;
		let targets = $('.' + newStyles.dsiTreeCheck);

		if (targets !== null && targets !== undefined && targets.length > 0) {
			for (let i = 0; i < targets.length; i++) {
				let target = targets[i];

				$(target).removeClass(newStyles.dsiTreeCheck);
			}
		}

		let addRegularMemberID = this.state.addRegularMemberID;
		const target = e.target;
		if ($(target).hasClass(settings.regularMemberCheck)) {
			// 선택된 팀원이라면 체크해제
			$(target).removeClass(settings.regularMemberCheck);

			for (let i = 0; i < addRegularMemberID.length; i++) {
				let memberID = addRegularMemberID[i];

				if (memberID === id) {
					addRegularMemberID.splice(i, 1);
					break;
				}
			}
		} else {
			// 선택되지 않은 팀원이라면 체크
			$(target).addClass(settings.regularMemberCheck);
			addRegularMemberID.push(id);
        }

		this.setState({ addRegularMemberID: addRegularMemberID });
	}

	removeReceiverList = (e, stringID) => {
		console.log("removeReceiverList ID: " + stringID);

		const target = e.target;
		let removeReceiverList = this.state.removeReceiverList;

		if ($(target).hasClass(settings.regularMemberCheck)) {
			// 이미 리스트이라면 체크해제
			$(target).removeClass(settings.regularMemberCheck);

			for (let i = 0; i < removeReceiverList.length; i++) {
				let removeReceiver = removeReceiverList[i];

				if (removeReceiver === stringID) {
					removeReceiverList.splice(i, 1);
					break;
				}
			}
		} else {
			// 선택되지 않은 리스트이라면 체크
			$(target).addClass(settings.regularMemberCheck);

			removeReceiverList.push(stringID);
		}

		let bChk = false;

		
    }

	displayReceiverList = () => {
		const arrRegularID = this.state.arrRegularID;
		const arrRegularMemberID = this.state.arrRegularMemberID;
		const teams = this.props.teams;
		const members = this.props.members;

		let displayReceiverList = [];
		let idx = 1;

		if (arrRegularID !== null && arrRegularID !== undefined && arrRegularID.length > 0 &&
			teams !== null && teams !== undefined) {

			for (let j = 0; j < arrRegularID.length; j++) {
				const regularID = arrRegularID[j];

				for (let i = 0; i < teams.length; i++) {
					const team = teams[i];
					//team.id.toString();
					if (regularID === team.id.toString()) {
						displayReceiverList.push(
							<tr>
								<td>{idx}</td>
								<td><a>{team.teamName}</a></td>
							</tr>
						);

						idx++;
						break;
                    }
				}
            }
			
		}

		if (arrRegularMemberID !== null && arrRegularMemberID !== undefined && arrRegularMemberID.length > 0 &&
			members !== null && members !== undefined) {

			for (let i = 0; i < arrRegularMemberID.length; i++) {
				const regularMemberID = arrRegularMemberID[i];

				for (let j = 0; j < members.length; j++) {
					const member = members[j];
					//member.ID.toString();
					
					if (regularMemberID === member.ID.toString()) {
						displayReceiverList.push(
							<tr>
								<td>{idx}</td>
								<td><a>{member.MemberName}</a></td>
							</tr>
						);

						idx++;
						break;
                    }
                }
            }
		}

		return displayReceiverList;
	}

	displayReceiverUI = () => {
		const displayReceiverList = this.state.displayReceiverList;
		const teams = this.props.teams;
		const members = this.props.members;
		let arrRegularID = [];
		let arrRegularMemberID = [];
		let displayReceiverUI = [];

		let idx = 1;

		for (let i = 0; i < displayReceiverList.length; i++) {
			let stringID = displayReceiverList[i];

			if (stringID.indexOf("team_") !== -1) {
				let arr = stringID.split("_");
				let id = parseInt(arr[1]);

				if (id !== null && id !== undefined && id !== "NaN") {
					arrRegularID.push(id);

					for (let i = 0; i < teams.length; i++) {
						const team = teams[i];

						if (id === team.id) {
							displayReceiverUI.push(
								<tr key={stringID}>
									<td>{idx}</td>
									<td><a onClick={(e) => this.removeReceiverList(e, stringID)}>{team.teamName}</a></td>
								</tr>
							);

							idx++;
							break;
						}
					}
                }
					
			} else if (stringID.indexOf("member_") !== -1) {
				let arr = stringID.split("_");
				let id = parseInt(arr[1]);

				if (id !== null && id !== undefined && id !== "NaN") {
					arrRegularMemberID.push(id);

					for (let j = 0; j < members.length; j++) {
						const member = members[j];

						if (id === member.ID) {
							displayReceiverUI.push(
								<tr key={stringID}>
									<td>{idx}</td>
									<td><a onClick={(e) => this.removeReceiverList(e, stringID)}>{member.MemberName}</a></td>
								</tr>
							);

							idx++;
							break;
						}
					}
                }

            }
        }

		return displayReceiverUI;
	}

	addReceiver = () => {
		const addRegularID = this.state.addRegularID;
		const addRegularMemberID = this.state.addRegularMemberID;
		let arrRegularID = this.state.arrNewRegularID;
		let arrRegularMemberID = this.state.arrNewRegularMemberID;
		let displayReceiverList = this.state.displayReceiverList;
		const teams = this.props.teams;
		const members = this.props.members;

		if (addRegularID !== null && addRegularID !== undefined) {
			let chk = false;

			if (arrRegularID === null || arrRegularID === undefined)
				arrRegularID = [];
			else {
				for (let i = 0; i < arrRegularID.length; i++) {
					const regularID = arrRegularID[i];

					if (regularID === addRegularID.toString()) {
						chk = true;
						break;
                    }
                }
            }

			if (chk === false) {
				arrRegularID.push(addRegularID.toString());
				displayReceiverList.push("team_" + addRegularID.toString());
            }
				
        }
			
		if (addRegularMemberID !== null && addRegularMemberID !== undefined) {
			for (let i = 0; i < addRegularMemberID.length; i++) {
				let regularMemberID = addRegularMemberID[i];
				let chk = false;

				for (let j = 0; j < arrRegularMemberID.length; j++) {
					let arrRegularMemberData = arrRegularMemberID[j];

					if (arrRegularMemberData === regularMemberID.toString()) {
						chk = true;
						break;
                    }
				}

				if (chk === false) {
					arrRegularMemberID.push(regularMemberID.toString());
					displayReceiverList.push("member_" + regularMemberID.toString());
                }
					
            }
		}

		this.setState({ arrNewRegularID: arrRegularID, arrNewRegularMemberID: arrRegularMemberID});
	}

	deleteReceiver = () => {
		let removeReceiverList = this.state.removeReceiverList;
		let displayReceiverList = this.state.displayReceiverList;
		let arrNewRegularID = this.state.arrNewRegularID;
		let arrNewRegularMemberID = this.state.arrNewRegularMemberID;

		for (let i = 0; i < removeReceiverList.length; i++) {
			let removeReceiver = removeReceiverList[i];

			for (let j = 0; j < displayReceiverList.length; j++) {
				let displayReceiver = displayReceiverList[j];

				if (displayReceiver === removeReceiver) {
					displayReceiverList.splice(j, 1);

					let arr = removeReceiver.split("_");
					let id = arr[1];

					if (id !== null && id !== undefined) {
						if (removeReceiver.indexOf("team_") !== -1) {
							for (let z = 0; z < arrNewRegularID.length; z++) {
								let regularID = arrNewRegularID[z];

								if (regularID === id) {
									arrNewRegularID.splice(z, 1);
									break;
								}
							}
						} else if (removeReceiver.indexOf("member_") !== -1) {
							for (let z = 0; z < arrNewRegularMemberID.length; z++) {
								let regularMemberID = arrNewRegularMemberID[z];

								if (regularMemberID === id) {
									arrNewRegularMemberID.splice(z, 1);
									break;
								}
							}
						}
					}

					break;
				}
			}
		}

		this.setState({ displayReceiverList: displayReceiverList, removeReceiverList: [], arrNewRegularID: arrNewRegularID, arrNewRegularMemberID: arrNewRegularMemberID });
	}
    
	render() {

		const [facilityType, location] = this.getInfo();
		const displayTreeViewUI = this.displayTreeView();
		const displayRegularMemberList = this.displayRegularMemberList();

		//const displayReceiverList = this.displayReceiverList();
		//const displayReceiverList = this.state.displayReceiverList;
		const displayReceiverUI = this.displayReceiverUI();

        return (
            <>
				<div id={newStyles.dshPop}>
					<div>
						<div>
							<div className={newStyles.dspCont}>
								<h4 className={newStyles.dspTitle}>수신자편집</h4>
								<div className={newStyles.stgmTo}>
									<span>유형 : </span>
									<p>{facilityType}</p>
								</div>
								<div className={newStyles.stgmTo}>
									<span>위치 : </span>
									<p>{location}</p>
								</div>

								<div className={newStyles.stguWrap}>
									<div>
										<div className={newStyles.stguTree + " " + settings.scrollbar}>
											<ul className={newStyles.dsiTree}>
												
												{displayTreeViewUI }
											</ul>
										</div>
									</div>
									<div>
										<div className={newStyles.stguTeam}>
											<div className={newStyles.stguTh}>
												<table>
													<colgroup>
														<col style={{ "width": "15%" }}/>
													</colgroup>
													<thead>
														<tr>
															<th>No</th>
															<th>팀원</th>
														</tr>
													</thead>
												</table>
											</div>
											<div className={newStyles.stguTd + " " + settings.scrollbar}>
												<table>
													<colgroup>
														<col style={{ "width": "15%" }} />
													</colgroup>
													<tbody className={settings.regularMemberList}>
														{displayRegularMemberList}
													</tbody>
												</table>
											</div>
										</div>
									</div>
									<div>
										<div className={newStyles.stguAdd}>
											<div>
												<div>
													<ul>
														<li><a onClick={this.addReceiver}>추가</a></li>
														<li><a onClick={this.deleteReceiver}>삭제</a></li>
													</ul>
												</div>
											</div>
										</div>
									</div>
									<div>
										<div className={newStyles.stguName}>
											<div className={newStyles.stguTh}>
												<table>
													<colgroup>
														<col style={{ "width": "15%" }} />
													</colgroup>
													<thead>
														<tr>
															<th>No</th>
															<th>이름</th>
														</tr>
													</thead>
												</table>
											</div>
											<div className={newStyles.stguTd + " " + settings.scrollbar}>
												<table>
													<colgroup>
														<col style={{ "width": "15%" }} />
													</colgroup>
													<tbody>
														{displayReceiverUI}
													</tbody>
												</table>
											</div>
										</div>
									</div>
								</div>

								<ul className={newStyles.dspBtnNew}>
									<li><a onClick={this.onClickConfirm}>확인</a></li>
									<li><a onClick={this.onClickClose}>취소</a></li>
								</ul>
							</div>
						</div>
					</div>
				</div>
            </>
        );
    }
}

export default SelectReceiver;