import React, { Component } from 'react';
import { Container } from 'reactstrap';
import $ from 'jquery';

import newStyles from '../../../Common/css/newStyle.module.css';
import newDefaults from '../../../Common/css/newDefault.module.css';
import settings from '../../css/settings.module.css';

import SDMSResource from '../../../SDMS/resource/id';
import { fab } from '@fortawesome/free-brands-svg-icons';

class SelectSOP extends Component {
    constructor(props) {
		super(props);

		this.state = {
			selectBuildingGroup: null,					// 선택된 빌딩그룹
			selectBuilding: null,						// 선택된 빌딩
			selectZone: null,							// 선택된 층
			selectedDisasterCategory: null,				// 선택된 재난분야
			selectedSubDisasterCategory: null,			// 선택된 재난종류
			selectedDisasterData: null,					// 선택 SOP
			selectFacilityType: SDMSResource.facilityType.FIRE,

			reload: true,
        }

		this.props = props;

	}

	componentDidMount() {
		// 화재센서 선택
		$("#facilityType_radio_" + SDMSResource.facilityType.FIRE).prop("checked", true);
	}

	displayBuildingGroup = () => {
		let buildingGroup = [];
		let building = [];
		let zone = [];

		const buildingGroupList = this.props.buildingGroupList;

		if (buildingGroupList === null || buildingGroupList === undefined)
			return [buildingGroup, building, zone];

		for (let i = 0; i < buildingGroupList.length; i++) {
			const buildingGroupData = buildingGroupList[i];

			if (this.state.selectBuildingGroup !== null && this.state.selectBuildingGroup !== undefined && this.state.selectBuildingGroup.id === buildingGroupData.id)
				buildingGroup.push(<li key={"buildingGroup_" + buildingGroupData.id}><input type="radio" name="buildingGroup_radio" id={"buildingGroup_radio_" + buildingGroupData.id} defaultChecked /><label htmlFor={"buildingGroup_radio_" + buildingGroupData.id} onClick={() => this.onClickBuildingGroup(buildingGroupData)} >{buildingGroupData.displayText}</label></li>);
			else
				buildingGroup.push(<li key={"buildingGroup_" + buildingGroupData.id}><input type="radio" name="buildingGroup_radio" id={"buildingGroup_radio_" + buildingGroupData.id} /><label htmlFor={"buildingGroup_radio_" + buildingGroupData.id} onClick={() => this.onClickBuildingGroup(buildingGroupData)} >{buildingGroupData.displayText}</label></li>);
		}

		if (this.state.selectBuildingGroup !== null && this.state.selectBuildingGroup !== undefined) {
			const selectBuildingGroup = this.state.selectBuildingGroup;
			//buildingDatas = Array(23) [Object, Object, Object, …]
			for (let i = 0; i < selectBuildingGroup.buildingDatas.length; i++) {
				const buildingData = selectBuildingGroup.buildingDatas[i];

				if (this.state.selectBuilding !== null && this.state.selectBuilding !== undefined && this.state.selectBuilding.id === buildingData.id)
					building.push(<li key={"building_" + buildingData.id}><input type="radio" name="building_radio" id={"building_radio_" + buildingData.id} defaultChecked /><label fhtmlForor={"building_radio_" + buildingData.id} onClick={() => this.onClickBuilding(buildingData)}>{buildingData.displayText}</label></li>);
				else 
					building.push(<li key={"building_" + buildingData.id}><input type="radio" name="building_radio" id={"building_radio_" + buildingData.id} /><label htmlFor={"building_radio_" + buildingData.id} onClick={() => this.onClickBuilding(buildingData)}>{buildingData.displayText}</label></li>);
            }
		}

		if (this.state.selectBuilding !== null && this.state.selectBuilding !== undefined) {
			const selectBuilding = this.state.selectBuilding;
			//zoneDatas = Array(2) [Object, Object]
			for (let i = 0; i < selectBuilding.zoneDatas.length; i++) {
				const zoneData = selectBuilding.zoneDatas[i];

				if (this.state.selectZone !== null && this.state.selectZone !== undefined && this.state.selectZone.id === zoneData.id)
					zone.push(<li key={"zone_" + zoneData.id}><input type="radio" name="zone_radio" id={"zone_radio_" + zoneData.id} defaultChecked /><label htmlFor={"zone_radio_" + zoneData.id} onClick={() => this.onClickZone(zoneData)}>{zoneData.displayText}</label></li>);
				else
					zone.push(<li key={"zone_" + zoneData.id}><input type="radio" name="zone_radio" id={"zone_radio_" + zoneData.id} /><label htmlFor={"zone_radio_" + zoneData.id} onClick={() => this.onClickZone(zoneData)}>{zoneData.displayText}</label></li>);
            }
		}

		return [buildingGroup, building, zone];
	}

	onClickBuildingGroup = (buildingGroup) => {
		let buildingData = null;

		if (buildingGroup !== null && buildingGroup !== undefined) {
			for (let i = 0; i < buildingGroup.buildingDatas.length; i++) {
				if (i === 0) {
					buildingData = buildingGroup.buildingDatas[i];
					break;
                }
			}
        }

		this.setState({ selectBuildingGroup: buildingGroup, selectBuilding: buildingData, selectZone: null });
	}

	onClickBuilding = (buildingData) => {
		const selectBuilding = this.state.selectBuilding;

		for (let i = 0; i < selectBuilding.zoneDatas.length; i++) {
			const zoneData = selectBuilding.zoneDatas[i];

			$("#zone_radio_" + zoneData.id).prop("checked", false);
		}

		this.setState({ selectBuilding: buildingData, selectZone: null });
	}

	onClickZone = (zoneData) => {
		this.setState({ selectZone: zoneData });
    }

	displayDisasterCategory = () => {
		let disasterCategory = [];
		let subDisasterCategory = [];
		let disaster = [];

		const disasterCategories = this.props.disasterCategories;

		if (disasterCategories === null || disasterCategories === undefined)
			return [disasterCategory, subDisasterCategory, disaster];

		for (let i = 0; i < disasterCategories.length; i++) {
			const disaster = disasterCategories[i];

			if (this.state.selectedDisasterCategory !== null && this.state.selectedDisasterCategory !== undefined && this.state.selectedDisasterCategory.disasterCategory.id === disaster.disasterCategory.id)
				disasterCategory.push(<li key={"disaster_" + disaster.disasterCategory.id}><input type="radio" name="disaster_radio" id={"disaster_radio_" + disaster.disasterCategory.id} defaultChecked /><label htmlFor={"disaster_radio_" + disaster.disasterCategory.id} onClick={() => this.onClickDisasterCategory(disaster)}>{disaster.disasterCategory.categoryName}</label></li>);
			else
				disasterCategory.push(<li key={"disaster_" + disaster.disasterCategory.id}><input type="radio" name="disaster_radio" id={"disaster_radio_" + disaster.disasterCategory.id} /><label htmlFor={"disaster_radio_" + disaster.disasterCategory.id} onClick={() => this.onClickDisasterCategory(disaster)}>{disaster.disasterCategory.categoryName}</label></li>);
		}

		if (this.state.selectedDisasterCategory !== null && this.state.selectedDisasterCategory !== undefined) {
			const selectedDisasterCategory = this.state.selectedDisasterCategory;
			const subDisasterCategories = selectedDisasterCategory.subDisasterCategories;

			if (subDisasterCategories !== null && subDisasterCategories !== undefined) {
				for (let i = 0; i < subDisasterCategories.length; i++) {
					const subDisaster = subDisasterCategories[i];

					if (this.state.selectedSubDisasterCategory !== null && this.state.selectedSubDisasterCategory !== undefined && this.state.selectedSubDisasterCategory.subDisasterCategory.id === subDisaster.subDisasterCategory.id)
						subDisasterCategory.push(<li key={"subDisaster_" + subDisaster.subDisasterCategory.id}><input type="radio" name="subDisaster_radio" id={"subDisaster_radio_" + subDisaster.subDisasterCategory.id} defaultChecked /><label htmlFor={"subDisaster_radio_" + subDisaster.subDisasterCategory.id} onClick={() => this.onClickSubDisasterCategory(subDisaster)}>{subDisaster.subDisasterCategory.subCategoryName}</label></li>);
					else
						subDisasterCategory.push(<li key={"subDisaster_" + subDisaster.subDisasterCategory.id}><input type="radio" name="subDisaster_radio" id={"subDisaster_radio_" + subDisaster.subDisasterCategory.id} /><label htmlFor={"subDisaster_radio_" + subDisaster.subDisasterCategory.id} onClick={() => this.onClickSubDisasterCategory(subDisaster)}>{subDisaster.subDisasterCategory.subCategoryName}</label></li>);
				}
            }
		}

		if (this.state.selectedSubDisasterCategory !== null && this.state.selectedSubDisasterCategory !== undefined) {
			const disasterDatas = this.state.selectedSubDisasterCategory.disasterDatas; //disasterName

			for (let i = 0; i < disasterDatas.length; i++) {
				const disasterData = disasterDatas[i];

				if (this.state.selectedDisasterData !== null && this.state.selectedDisasterData !== undefined && this.state.selectedDisasterData.disasterName === disasterData.disasterName)
					disaster.push(<li key={"disasterData_" + disasterData.disasterName}><input type="radio" name="disasterData_radio" id={"disasterData_radio_" + disasterData.disasterName} defaultChecked /><label htmlFor={"disasterData_radio_" + disasterData.disasterName} onClick={() => this.onClickDisasterData(disasterData)}>{disasterData.disasterName}</label></li>);
				else
					disaster.push(<li key={"disasterData_" + disasterData.disasterName}><input type="radio" name="disasterData_radio" id={"disasterData_radio_" + disasterData.disasterName} /><label htmlFor={"disasterData_radio_" + disasterData.disasterName} onClick={() => this.onClickDisasterData(disasterData)}>{disasterData.disasterName}</label></li>);
            }
        }

		return [disasterCategory, subDisasterCategory, disaster];
	}

	onClickDisasterCategory = (disasterCategory) => {
		this.setState({ selectedDisasterCategory: disasterCategory, selectedSubDisasterCategory: null, selectedDisasterData: null });
	}

	onClickSubDisasterCategory = (subDisasterCategory) => {
		this.setState({ selectedSubDisasterCategory: subDisasterCategory, selectedDisasterData: null });
	}

	onClickDisasterData = (disasterData) => {
		this.setState({ selectedDisasterData: disasterData});
	}

	displayFacilityType() {
		//SDMSResource
		let facilityType = [];

		facilityType.push(<li key={"facilityType_" + SDMSResource.facilityType.FIRE}><input type="radio" name="facilityType" id={"facilityType_radio_" + SDMSResource.facilityType.FIRE} /><label htmlFor={"facilityType_radio_" + SDMSResource.facilityType.FIRE} onClick={() => this.onClickFacilityType(SDMSResource.facilityType.FIRE)}>{SDMSResource.getFacilityTypeString(SDMSResource.facilityType.FIRE)}</label></li>);
		facilityType.push(<li key={"facilityType_" + SDMSResource.facilityType.PSM_SENSOR}><input type="radio" name="facilityType" id={"facilityType_radio_" + SDMSResource.facilityType.PSM_SENSOR} /><label htmlFor={"facilityType_radio_" + SDMSResource.facilityType.PSM_SENSOR} onClick={() => this.onClickFacilityType(SDMSResource.facilityType.PSM_SENSOR)}>{SDMSResource.getFacilityTypeString(SDMSResource.facilityType.PSM_SENSOR)}</label></li>);
		facilityType.push(<li key={"facilityType_" + SDMSResource.facilityType.ETC}><input type="radio" name="facilityType" id={"facilityType_radio_" + SDMSResource.facilityType.ETC} /><label htmlFor={"facilityType_radio_" + SDMSResource.facilityType.ETC} onClick={() => this.onClickFacilityType(SDMSResource.facilityType.ETC)}>{SDMSResource.getFacilityTypeString(SDMSResource.facilityType.ETC)}</label></li>);
		facilityType.push(<li key={"facilityType_" + SDMSResource.facilityType.Intrusion_S1}><input type="radio" name="facilityType" id={"facilityType_radio_" + SDMSResource.facilityType.Intrusion_S1} /><label htmlFor={"facilityType_radio_" + SDMSResource.facilityType.Intrusion_S1} onClick={() => this.onClickFacilityType(SDMSResource.facilityType.Intrusion_S1)}>지능형 영상</label></li>);

		return facilityType;
    }

	onClickFacilityType = (facilityType) => {
		const buildingGroupList = this.props.buildingGroupList;

		if (buildingGroupList !== null && buildingGroupList !== undefined) {
			for (let i = 0; i < buildingGroupList.length; i++) {
				const buildingGroupData = buildingGroupList[i];

				$("#buildingGroup_radio_" + buildingGroupData.id).prop("checked", false);
			}
        }

		this.setState({ selectFacilityType: facilityType, selectBuildingGroup: null, selectBuilding: null, selectZone: null });
	}

	displayLinkedSOP() {
		let linkedSOPUI = [];
		const linkedSOPs = this.props.linkedSOPs;

		if (linkedSOPs === null || linkedSOPs === undefined)
			return linkedSOPUI;

		let num = 1;
		const selectBuildingGroup = this.state.selectBuildingGroup;
		const selectBuilding = this.state.selectBuilding;
		const selectZone = this.state.selectZone;
		const selectFacilityType = this.state.selectFacilityType;

		const disasterCategories = this.props.disasterCategories;
		const buildingGroupList = this.props.buildingGroupList;

		let facilityType = "";
		let buildingGroup = "";
		let building = "";
		let zone = "";

		let disasterCategory = "";
		let subDisasterCategory = "";

		for (let i = 0; i < linkedSOPs.length; i++) {
			let sopData = linkedSOPs[i];

			if (selectFacilityType !== sopData.facilityTypeID)
				continue;
			else if (selectBuilding !== null && sopData.linkedBuildingID !== selectBuilding.id)
				continue;
			else if (selectZone !== null && sopData.linkedZoneID !== selectZone.id)
				continue;

			if (selectFacilityType !== SDMSResource.facilityType.Intrusion_S1)
				facilityType = SDMSResource.getFacilityTypeString(selectFacilityType);
			else
				facilityType = "지능형 영상";

			let buildingGroupData = null;
			let buildingData = null;
			let zoneData = null;
			let chk = false;

			//if (sopData.linkedBuildingID !== -1 && sopData.linkedBuildingID !== 0 &&
			if (sopData.linkedBuildingID !== null &&
				buildingGroupList !== null && buildingGroupList !== undefined) {

				for (let i = 0; i < buildingGroupList.length; i++) {
					const buildingGroup = buildingGroupList[i];

					for (let j = 0; j < buildingGroup.buildingDatas.length; j++) {
						const building = buildingGroup.buildingDatas[j];

						if (building.id === sopData.linkedBuildingID) {
							buildingGroupData = buildingGroup;
							buildingData = building;

							//if (sopData.linkedZoneID !== -1 && sopData.linkedZoneID !== 0) {
							if (sopData.linkedZoneID !== null) {
								for (let z = 0; z < building.zoneDatas.length; z++) {
									const zone = building.zoneDatas[z];

									if (sopData.linkedZoneID === zone.id) {
										zoneData = zone;
										break;
                                    }
                                }
							}

							chk = true;
							break;
						}
					}

					if (chk === true)
						break;
				}
			}

			if (buildingData !== null) {
				buildingGroup = buildingGroupData.displayText;
				building = buildingData.buildingName;
			}

			if (zoneData !== null) {
				building = zoneData.displayText;
            }

			for (let j = 0; j < disasterCategories.length; j++) {
				const data = disasterCategories[j].disasterCategory;
				const subDisasterCategories = disasterCategories[j].subDisasterCategories;

				if (data.id === sopData.disasterCategoryID) {
					disasterCategory = data.categoryName;

					for (let z = 0; z < subDisasterCategories.length; z++) {
						const sub = subDisasterCategories[z].subDisasterCategory;

						if (sub.id === sopData.subDisasterCategoryID) {
							subDisasterCategory = sub.subCategoryName;
							break;
                        }
                    }

					break;
                }
            }

			linkedSOPUI.push(
				<tr key="linkedSOPUI">
					<td>{num}</td>
					<td>{buildingGroup}</td>
					<td>{building}</td>
					<td>{facilityType}</td>
					<td>{disasterCategory}</td>
					<td>{subDisasterCategory}</td>
					<td>{sopData.disasterName}</td>
					<td>
						<a onClick={() => this.onClickModifyLinkedSOP(sopData)}>편집</a>
						<a onClick={() => this.onClickDeleteLinkedSOP(sopData.facilityTypeID, sopData.linkedBuildingID, sopData.linkedZoneID)}>삭제</a></td>
				</tr>
			);

			num++;
		}

		return linkedSOPUI;
	}

	onClickModifyLinkedSOP = (sopData) => {
		let buildingGroupData = null;
		let buildingData = null;
		let zoneData = null;
		let disasterCategory = null;
		let subDisasterCategory = null;
		let chk = false;

		const buildingGroupList = this.props.buildingGroupList;
		const selectFacilityType = sopData.facilityTypeID;
		const disasterCategories = this.props.disasterCategories;

		//if (sopData.linkedBuildingID !== -1 && sopData.linkedBuildingID !== 0 &&
		if (sopData.linkedBuildingID !== null &&
			buildingGroupList !== null && buildingGroupList !== undefined) {

			for (let i = 0; i < buildingGroupList.length; i++) {
				const buildingGroup = buildingGroupList[i];

				for (let j = 0; j < buildingGroup.buildingDatas.length; j++) {
					const building = buildingGroup.buildingDatas[j];

					if (building.id === sopData.linkedBuildingID) {
						buildingGroupData = buildingGroup;
						buildingData = building;

						//if (sopData.linkedZoneID !== -1 && sopData.linkedZoneID !== 0) {
						if (sopData.linkedZoneID !== null) {
							for (let z = 0; z < building.zoneDatas.length; z++) {
								const zone = building.zoneDatas[z];

								if (sopData.linkedZoneID === zone.id) {
									zoneData = zone;
									break;
								}
							}
						}

						chk = true;
						break;
					}
				}

				if (chk === true)
					break;
			}
		}

		for (let j = 0; j < disasterCategories.length; j++) {
			const data = disasterCategories[j].disasterCategory;
			const subDisasterCategories = disasterCategories[j].subDisasterCategories;

			if (data.id === sopData.disasterCategoryID) {
				disasterCategory = disasterCategories[j];

				for (let z = 0; z < subDisasterCategories.length; z++) {
					const sub = subDisasterCategories[z].subDisasterCategory;

					if (sub.id === sopData.subDisasterCategoryID) {
						subDisasterCategory = subDisasterCategories[z];
						break;
					}
				}

				break;
			}
		}

		this.setState({ selectFacilityType: selectFacilityType, selectBuildingGroup: buildingGroupData, selectBuilding: buildingData, selectZone: zoneData, selectedDisasterCategory: disasterCategory, selectedSubDisasterCategory: subDisasterCategory, selectedDisasterData: sopData });
    }

	onClickDeleteLinkedSOP = (facilityTypeID, linkedBuildingID, linkedZoneID) => {
		let linkedSOPs = this.props.linkedSOPs;

		if (linkedSOPs === null || linkedSOPs === undefined)
			return;

		for (let i = 0; i < linkedSOPs.length; i++) {
			let sopData = linkedSOPs[i];

			// 기존에 설정된 LinkedSOP 찾기
			if (sopData.facilityTypeID !== facilityTypeID) {
				continue;
				//} else if ((linkedBuildingID === 0 || linkedBuildingID === -1) &&
				//(sopData.linkedBuildingID === 0 || sopData.linkedBuildingID === -1)) {
			} else if ((linkedBuildingID === null) &&
				(sopData.linkedBuildingID === null)) {

				linkedSOPs.splice(i, 1);
				break;

				//} else if ((linkedZoneID === 0 || linkedZoneID === -1) &&
				//(sopData.linkedZoneID === 0 || sopData.linkedZoneID === -1) &&
			} else if ((linkedZoneID === null) &&
				(sopData.linkedZoneID === null) &&
				sopData.linkedBuildingID === linkedBuildingID) {
				
				linkedSOPs.splice(i, 1);
				break;

			//} else if ((linkedZoneID !== -1 && linkedZoneID !== 0) &&
			//	(linkedBuildingID !== 0 && linkedBuildingID !== -1) &&
			} else if ((linkedZoneID !== null) &&
				(linkedBuildingID !== null) &&
				sopData.linkedBuildingID === linkedBuildingID &&
				sopData.linkedZoneID === linkedZoneID) {

				linkedSOPs.splice(i, 1);
				break;

			}
		}

		// 화면 reload
		this.setState({ reload: true });
    }

	onClickSetSOP = () => {
		if (this.state.selectedDisasterData === null)
			return;

		const linkedSOPs = this.props.linkedSOPs;
		const selectBuilding = this.state.selectBuilding;
		const selectZone = this.state.selectZone;
		const selectedDisasterCategory = this.state.selectedDisasterCategory;
		const selectedSubDisasterCategory = this.state.selectedSubDisasterCategory;
		const selectedDisasterData = this.state.selectedDisasterData;
		const selectFacilityType = this.state.selectFacilityType;

		let chk = false;	// 해당 sop 유무 판단

		for (let i = 0; i < linkedSOPs.length; i++) {
			let sopData = linkedSOPs[i];

			// 기존에 설정된 LinkedSOP 찾기
			if (sopData.facilityTypeID !== selectFacilityType) {
				continue;
			} else if (selectBuilding === null &&
				//(sopData.linkedBuildingID === 0 || sopData.linkedBuildingID === -1)) {
				(sopData.linkedBuildingID === null)) {
				chk = true;
				sopData.disasterName = this.state.selectedDisasterData.disasterName;
			} else if (selectZone === null && selectBuilding !== null &&
				sopData.linkedBuildingID === selectBuilding.id &&
				//(sopData.linkedZoneID === 0 || sopData.linkedZoneID === -1)) {
				(sopData.linkedZoneID === null)) {
				chk = true;
				sopData.disasterName = this.state.selectedDisasterData.disasterName;
			} else if (selectBuilding !== null && selectZone !== null &&
				sopData.linkedBuildingID === selectBuilding.id &&
				sopData.linkedZoneID === selectZone.id) {
				chk = true;
				sopData.disasterName = this.state.selectedDisasterData.disasterName;
            }
		}

		if (chk === false) {
			// 해당 linkedSOP를 찾을 수 없기에 새로 데이터 생성
			let sopData = new Object;
			sopData.id = -1;
			sopData.facilityTypeID = selectFacilityType;
			sopData.disasterCategoryID = selectedDisasterCategory.disasterCategory.id;
			sopData.subDisasterCategoryID = selectedSubDisasterCategory.subDisasterCategory.id

			if (selectBuilding === null)
				sopData.linkedBuildingID = null;
			else
				sopData.linkedBuildingID = selectBuilding.id;

			if (selectZone === null)
				sopData.linkedZoneID = null;
			else
				sopData.linkedZoneID = selectZone.id;

			sopData.description = null;
			sopData.disasterName = this.state.selectedDisasterData.disasterName;

			this.props.linkedSOPs.push(sopData);
        }

		// 화면 reload
		this.setState({reload: true});
    }

	render() {
		const [buildingGroup, building, zone] = this.displayBuildingGroup();
		const [disasterCategory, subDisasterCategory, disasterData] = this.displayDisasterCategory();
		const facilityType = this.displayFacilityType();
		const linkedSOPUI = this.displayLinkedSOP();

        return (
			<React.Fragment>
				<div id={newStyles.sopPop}>
					<div>
						<div>
							<div className={newStyles.sppCont}>
								<div className={newStyles.sppTitle}>
									<h3>센서신호별실행SOP 설정</h3>
									<a onClick={this.props.selectSOPOff}>닫기</a>
								</div>
								<div className={newStyles.sppRow}>
									<div>
										<div className={newStyles.sppCol}>
											<div className={newStyles.spcDep1}>
												<h4>위치</h4>
											</div>
											<ul className={newStyles.spcDep2}>
												<li className={newStyles.col1}><h5>공장</h5></li>
												<li className={newStyles.col1}><h5>건물</h5></li>
												<li className={newStyles.col1}><h5>층</h5></li>
											</ul>
											<div className={newStyles.spcDep3 + " " + settings.scrollbar}>
												<div className={newStyles.spcTr}>
													<div className={newStyles.spcTd + " " + newStyles.col1} id="spcTd1" style={{ display: "block" }}>
														<ul className={newStyles.spcChk}>
															{buildingGroup}
														</ul>
													</div>
													<div className={newStyles.spcTd + " " + newStyles.col1} id="spcTd2" style={{ display: "block" }}>
														<ul className={newStyles.spcChk}>
															{building}
														</ul>
													</div>
													<div className={newStyles.spcTd + " " + newStyles.col1} id="spcTd3" style={{ display: "block" }}>
														<ul className={newStyles.spcChk}>
															{zone}
														</ul>
													</div>
												</div>
											</div>
										</div>
									</div>
									<div>
										<div className={newStyles.sppCol}>
											<div className={newStyles.spcDep1}>
												<h4>센서 유형</h4>
											</div>
											<ul className={newStyles.spcDep2}>
											</ul>
											<div className={newStyles.spcDep3 + " " + settings.scrollbar}>
												<div className={newStyles.spcTr}>
													<div className={newStyles.spcTd + " " + newStyles.col1} id="spcTd4" style={{ display: "block" }}>
														<ul className={newStyles.spcChk}>
															{facilityType}
														</ul>
													</div>
												</div>
											</div>
										</div>
									</div>
									<div>
										<div className={newStyles.sppCol}>
											<div className={newStyles.spcDep1}>
												<h4>재난유형 및 SOP 목록</h4>
												<a onClick={() => this.onClickSetSOP()}>적용</a>
											</div>
											<ul className={newStyles.spcDep2}>
												<li className={newStyles.col1}><h5>재난분야</h5></li>
												<li className={newStyles.col1}><h5>재난종류</h5></li>
												<li className={newStyles.col1}><h5>SOP 이름</h5></li>
											</ul>
											<div className={newStyles.spcDep3 + " " + settings.scrollbar}>
												<div className={newStyles.spcTr}>
													<div className={newStyles.spcTd + " " + newStyles.col1} id="spcTd7" style={{display: "block"}}>
														<ul className={newStyles.spcChk}>
															{disasterCategory}
														</ul>
													</div>
													<div className={newStyles.spcTd + " " + newStyles.col1} id="spcTd8" style={{ display: "block" }}>
														<ul className={newStyles.spcChk}>
															{subDisasterCategory}
														</ul>
													</div>
													<div className={newStyles.spcTd + " " + newStyles.col1} id="spcTd9" style={{ display: "block" }}>
														<ul className={newStyles.spcChk}>
															{disasterData}
														</ul>
													</div>
												</div>
											</div>
										</div>
									</div>
								</div>
								<div className={newStyles.sppBot}>

									<div className={newStyles.stguTh}>
										<table>
											<colgroup>
												<col style={{ width: "5%" }} />
												<col style={{ width: "11%" }} />
												<col style={{ width: "10%" }} />
												<col style={{ width: "12%" }} />
												<col style={{ width: "12%" }} />
												<col style={{ width: "12%" }} />
												<col style={{ width: "20%" }} />
												<col style={{ width: "18%" }} />
											</colgroup>
											<thead>
												<tr>
													<th>No</th>
													<th>공장</th>
													<th>건물 및 층</th>
													<th>센서유형</th>
													<th>재난분야</th>
													<th>재난종류</th>
													<th>SOP이름</th>
													<th>편집</th>
												</tr>
											</thead>
										</table>
									</div>
									<div className={newStyles.stguTd + " " + settings.scrollbar}>
										<table>
											<colgroup>
												<col style={{ width: "5%" }} />
												<col style={{ width: "11%" }} />
												<col style={{ width: "10%" }} />
												<col style={{ width: "12%" }} />
												<col style={{ width: "12%" }} />
												<col style={{ width: "12%" }} />
												<col style={{ width: "20%" }} />
												<col style={{ width: "18%" }} />
											</colgroup>
											<tbody>
												{linkedSOPUI}
											</tbody>
										</table>
									</div>


								</div>
							</div>
						</div>
					</div>
				</div>
			</React.Fragment>
        );
    }
}

export default SelectSOP;