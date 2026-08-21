import React, { Component } from 'react';
import styles from '../../../Common/css/style.module.css';
import bodyStyles from '../../css/body.module.css';
import SopController from '../../services/sopController';
import '../../../Common/css/scroll.css';

import fire_icon from '../../image/fire.png';
import etc_icon from '../../image/etc.png';
import explosion_icon from '../../image/explosion.png';
import natureDisaster_icon from '../../image/natureDisaster.png';
import pollution_icon from '../../image/pollution.png';
import security_icon from '../../image/security.png';
import terror_icon from '../../image/terror.png';
import SopManagerResource from '../../resource/id';
import SopManager from '../sopManager';

class NewSOPOptions extends Component {
	static cssStyles = styles;

	constructor(props) {
		super(props);

		this.props = props;

		this.state = {
			isNormal: true,
			normalDisasterCategories: [],
			abnormalDisasterCategories: [],
			disasterCategories: [],
			subDisasterCategories: [],
			disasterDatas: [],
			selectedDisasterCategory: null,
			selectedSubDisasterCategory: null,
			selectedDisaster: null,
			newSubDisasterCategory: null,
			newDisaster: null,
			loading: true,
			loadingMessage: SopManagerResource.ID.messages.loadingData
		}

		this.refNewSDC = React.createRef();
		this.refNewSDCRadio = React.createRef();
		this.refNewDisaster = React.createRef();
		this.refNewDisasterRadio = React.createRef();
	}

	componentDidMount() {
		this.onSelectSopMode(null, this.state.isNormal);
	}

	async getDisasterCategories(isNormal) {
		const [disasterCategories, message] = await SopController.disasterCategories(isNormal);

		if (disasterCategories) {
			if (isNormal) {
				this.setState({ loading: false, isNormal: isNormal, normalDisasterCategories: disasterCategories, disasterCategories: disasterCategories });
			}
			else {
				this.setState({ loading: false, isNormal: isNormal, abnormalDisasterCategories: disasterCategories, disasterCategories: disasterCategories });
            }
		}
		else {
			this.setState({ loadingMessage: message, isNormal: isNormal});
        }
	}

	onSelectSopMode(event, isNormal) {
		if (event) {
			const childCount = event.target.parentNode.children.length;
			let inputCount = 0;

			for (let i = 0; i < childCount; i++) {
				const child = event.target.parentNode.children[i];
				if (child.tagName === "INPUT") {
					inputCount++;

					if (isNormal === false && inputCount === 1) {
						continue;
					}

					child.checked = true;
					break;
				}
			}
		}

		// 항상 DB를 읽어오는 방식으로 바꾼다.
		this.getDisasterCategories(isNormal);
		/*if (isNormal) {
			if (this.state.normalDisasterCategories && this.state.normalDisasterCategories.length > 0) {
				this.setState({ isNormal: isNormal, disasterCategories: this.state.normalDisasterCategories });
			}
			else {
				this.getDisasterCategories(isNormal);
			}
		}
		else {
			if (this.state.abnormalDisasterCategories && this.state.abnormalDisasterCategories.length > 0) {
				this.setState({ isNormal: isNormal, disasterCategories: this.state.abnormalDisasterCategories });
			}
			else {
				this.getDisasterCategories(isNormal);
			}
        }*/
	}

	onSelectDisasterCategory(disasterCategoryData) {
		this.refNewSDC.current.value = '';
		this.refNewDisaster.current.value = '';
		this.refNewSDCRadio.current.checked = false;
		this.refNewDisasterRadio.current.checked = false;

		this.setState(
			{
				selectedDisasterCategory: disasterCategoryData,
				selectedSubDisasterCategory: null,
				selectedDisaster: null,
				subDisasterCategories: disasterCategoryData.subDisasterCategories,
				disasterDatas: [],
				newSubDisasterCategory: null,
				newDisaster: null
			}
		);
	}

	onSelectSubDisasterCategory(event, subDisasterCategoryData) {
		if (event) {
			const childCount = event.target.parentNode.children.length;

			for (let i = 0; i < childCount; i++) {
				const child = event.target.parentNode.children[i];
				if (child.tagName === "INPUT") {
					child.checked = true;
					break;
                }
            }
		}

		this.refNewDisaster.current.value = '';
		this.refNewDisasterRadio.current.checked = false;

		this.setState(
			{
				selectedSubDisasterCategory: subDisasterCategoryData,
				selectedDisaster: null,
				disasterDatas: subDisasterCategoryData.disasterDatas,
				newDisaster: null,
			}
		);
	}

	onSelectNewSubDisasterCategory() {
		const sdc = {
			subDisasterCategory:
			{
				id: -1,
				disasterCategoryID: this.state.selectedDisasterCategory ? this.state.selectedDisasterCategory.id : -1,
				subCategoryName: ''
			},
			disasterDatas: []
		};

		this.refNewDisaster.current.value = '';
		this.refNewDisasterRadio.current.checked = false;

		this.setState(
			{
				selectedSubDisasterCategory: sdc,
				selectedDisaster: null,
				newSubDisasterCategory: sdc,
				newDisaster: null,
				disasterDatas: sdc.disasterDatas
			}
		);
	}

	onSelectDisaster(event, disasterData) {
		if (event) {
			const childCount = event.target.parentNode.children.length;

			for (let i = 0; i < childCount; i++) {
				const child = event.target.parentNode.children[i];
				if (child.tagName === "INPUT") {
					child.checked = true;
					break;
				}
			}
		}

		this.setState(
			{
				selectedDisaster: disasterData
			}
		);
	}

	onSelectNewDisaster() {
		const newDisaster = {
			disaster:
			{
				id: -1,
				disasterName: "",
				subDisasterCategoryID: this.state.selectedSubDisasterCategory?.subDisasterCategory?.id,
				versionID: -1,
				userLevelIDs: null,
				description: null
			},
			actionSteps: [],
			version:
			{
				id: -1,
				isNormal: this.state.isNormal,
				createTime: null,
				lastAccessTime: null,
				versionName: "",
				ownerID: -1,
				description: -1
            }
		};

		this.setState(
			{
				selectedDisaster: newDisaster,
				newDisaster: newDisaster
			}
		);
	}

	getLastDisaster(disasterDatas) {
		if (disasterDatas) {
			const count = disasterDatas.length;

			for (let i = count-1; i >= 0; i--) {
				const disasterData = disasterDatas[i];
				return disasterData;
            }
		}

		return null;
	}

	getSOPData() {
		if (this.state.selectedSubDisasterCategory === this.state.newSubDisasterCategory) {
			const sdcName = this.refNewSDC.current.value.trim();

			if (sdcName.length === 0) {
				alert(SopManagerResource.ID.messages.inputDisasterType);
				return null;
			}

			this.state.selectedSubDisasterCategory.subDisasterCategory.subCategoryName = sdcName;
		}

		let disasterData = this.state.selectedDisaster;

		if (disasterData === this.state.newDisaster) {
			const disasterName = this.refNewDisaster.current.value.trim();

			if (disasterName.length === 0) {
				alert(SopManagerResource.ID.messages.inputSOPName);
				return null;
			}

			disasterData.disaster.disasterName = disasterName;
			disasterData = {
				disasterName: disasterName,
				disasterDatas: [disasterData]
			};
		}

		const disaster = this.getLastDisaster(disasterData.disasterDatas);

		if (disaster === null) {
			alert(SopManagerResource.ID.messages.unknownSOPName);
			return;
		}

		disaster.version.owner = disaster.owner;

		const sopData = {
			disasterCategory: this.state.selectedDisasterCategory,
			subDisasterCategory: this.state.selectedSubDisasterCategory,
			disaster: disaster.disaster,
			version: disaster.version,
			actionStepDatas: disaster.actionSteps
		};

		return sopData;
	}

	onClickCancel() {
		// 원래 상태 그대로 돌려준다.
		this.props.content(SopManager.menu.editSOP, this.props.sopData);
    }

	onClickApply() {
		if (!this.state.selectedDisasterCategory) {
			alert(SopManagerResource.ID.messages.selectDisasterCategory);
			return;
		}
		else if (!this.state.selectedSubDisasterCategory) {
			alert(SopManagerResource.ID.messages.selectSubDisasterCategory);
			return;
		}
		else if (!this.state.selectedDisaster) {
			alert(SopManagerResource.ID.messages.selectSOPName);
			return;
		}

		const sopData = this.getSOPData();

		if (sopData?.actionStepDatas && sopData.actionStepDatas.length > 0) {
			this._checkStepMembers(sopData);
			//this.props.content(SopManager.menu.editSOP, sopData);
		}
		else {
			this.addActionStepDatas(sopData);
        }
	}

	async addActionStepDatas(sopData) {
		const [actionStepDatas, message] = await SopController.requestDefaultActionStepDatas();

		if (actionStepDatas === null) {
			alert(message);
		}
		else {
			sopData.actionStepDatas = actionStepDatas;
			await this.checkStepMembers(sopData);
			this.props.content(SopManager.menu.editSOP, sopData);
        }
	}

	async _checkStepMembers(sopData) {
		await this.checkStepMembers(sopData);
		this.props.content(SopManager.menu.editSOP, sopData);
    }

	async checkStepMembers(sopData) {
		if (sopData) {
			const actionStepCount = sopData.actionStepDatas.length;

			for (let i = 0; i < actionStepCount; i++) {
				const actionStepData = sopData.actionStepDatas[i];

				if (actionStepData.stepMemberDatas.length === 0) {
					const [stepMemberData, message] = await SopController.requestDefaultStepMemberData(actionStepData);

					if (!stepMemberData) {
						alert(message);
						break;
					}
				}
			}
		}
	}

	getDisasterCategoryImage(disasterCategoryData) {
		const dcType = SopManagerResource.getDisasterCategoryType(disasterCategoryData.disasterCategory.categoryName);

		if (dcType === SopManagerResource.disasterCategoryType.fire) {
			return fire_icon;
		}
		else if (dcType === SopManagerResource.disasterCategoryType.natureDisaster) {
			return natureDisaster_icon;
		}
		else if (dcType === SopManagerResource.disasterCategoryType.explosion) {
			return explosion_icon;
		}
		else if (dcType === SopManagerResource.disasterCategoryType.pollution) {
			return pollution_icon;
		}
		else if (dcType === SopManagerResource.disasterCategoryType.security) {
			return security_icon;
		}
		else if (dcType === SopManagerResource.disasterCategoryType.terror) {
			return terror_icon;
		}

		return etc_icon;
	}

	onChangeSubDisasterCategoryText() {
		if (this.refNewSDC.current) {
			const value = this.refNewSDC.current.value;

			if (value && value.length > 0) {
				if (this.refNewSDCRadio.current) {
					this.refNewSDCRadio.current.checked = true;
					this.onSelectNewSubDisasterCategory();
				}
			}
        }
	}

	onChangeDisasterText() {
		if (this.refNewDisaster.current) {
			const value = this.refNewDisaster.current.value;

			if (value && value.length > 0) {
				if (this.refNewDisasterRadio.current) {
					this.refNewDisasterRadio.current.checked = true;
					this.onSelectNewDisaster();
				}
			}
        }
    }

	render() {
		if (this.state.loading) {
			return <h2>{this.state.loadingMessage}</h2>
		}

		const sdcClassName = this.state.selectedDisasterCategory ? NewSOPOptions.cssStyles.on : "";
		const disasterClassName = this.state.selectedSubDisasterCategory ? NewSOPOptions.cssStyles.on : "";
        
        return (
			<div className={NewSOPOptions.cssStyles.speWrap}>

				<div className={NewSOPOptions.cssStyles.speTop}>
					<h3>{SopManagerResource.ID.menu.newSOP}</h3>
					<input type="radio" name="speTop" id={NewSOPOptions.cssStyles.speTop01} checked={this.state.isNormal} onChange={() => this.onSelectSopMode(null, true)} /><label onClick={(event) => this.onSelectSopMode(event, true)}>{SopManagerResource.ID.sopMode.normal}</label>
					<input type="radio" name="speTop" id={NewSOPOptions.cssStyles.speTop02} checked={!this.state.isNormal} onChange={() => this.onSelectSopMode(null, false)} /><label onClick={(event) => this.onSelectSopMode(event, false)}>{SopManagerResource.ID.sopMode.abnormal}</label>
				</div>

				<div className={NewSOPOptions.cssStyles.speRow}>
					<div className={NewSOPOptions.cssStyles.speCont}>
						<div>
							<div>
								<h4 className={NewSOPOptions.cssStyles.on}>{SopManagerResource.ID.category.disasterCategory}</h4>
								<div className={NewSOPOptions.cssStyles.speChk}>
								</div>
								<div className={"scroll-wrapper " + NewSOPOptions.cssStyles.speScr + " scrollbar-outer"}>
									<div className={NewSOPOptions.cssStyles.speScr}>
										<ul className={NewSOPOptions.cssStyles.speGry}>
											{
												this.state.disasterCategories.map(disasterCategoryData => (
													<li key={"dc_" + disasterCategoryData.disasterCategory.categoryName}>
														<label htmlFor={NewSOPOptions.cssStyles.speGry01}>
															<img src={this.getDisasterCategoryImage(disasterCategoryData)} alt="" />
															<input type="radio" name="speGry" id={NewSOPOptions.cssStyles.speGry01} onChange={() => this.onSelectDisasterCategory(disasterCategoryData)} />
															<span>{disasterCategoryData.disasterCategory.categoryName}</span>
														</label>
													</li>
													))
                                            }
										</ul>
									</div>
								</div>
							</div>
						</div>
					</div>

					<div className={NewSOPOptions.cssStyles.speCont}>
						<div>
							<div>
								<h4 className={sdcClassName}>{SopManagerResource.ID.category.subDisasterCategory}</h4>
								<div className="scroll-wrapper scrollbar-outer">
									<div className={NewSOPOptions.cssStyles.speScr}>
										<ul className={NewSOPOptions.cssStyles.speLst}>
											{
												this.state.subDisasterCategories.map(sdc => (
													<li key={"sdc_" + sdc.subDisasterCategory.subCategoryName}>
														<input type="radio" name="speType" id={NewSOPOptions.cssStyles.speType01} disabled={this.state.selectedDisasterCategory === null} onChange={() => this.onSelectSubDisasterCategory(null, sdc)} />
														<label onClick={(event) => this.onSelectSubDisasterCategory(event, sdc)}>{sdc.subDisasterCategory.subCategoryName}</label>
													</li>
                                                ))
											}
											<li>
												<input ref={this.refNewSDCRadio} type="radio" name="speType" id={NewSOPOptions.cssStyles.speType01} disabled={this.state.selectedDisasterCategory === null} onChange={() => this.onSelectNewSubDisasterCategory()} />
												<input ref={this.refNewSDC} type="text" className={bodyStyles.fullText} disabled={this.state.selectedDisasterCategory === null} placeholder={SopManagerResource.ID.placeHolders.newSubDisasterCategory} onChange={() => this.onChangeSubDisasterCategoryText()} />
											</li>
										</ul>
									</div>
								</div>
							</div>
						</div>
					</div>

					<div className={NewSOPOptions.cssStyles.speCont}>
						<div>
							<div>
								<h4 className={disasterClassName}>{SopManagerResource.ID.category.disaster}</h4>
								<div className="scroll-wrapper scrollbar-outer">
									<div className={NewSOPOptions.cssStyles.speScr}>
										<ul className={NewSOPOptions.cssStyles.speIpt}>
											{
												this.state.disasterDatas.map(disasterData => (
													<li key={"disaster_" + disasterData.disasterName}>
														<input type="radio" name="speIpt" id={NewSOPOptions.cssStyles.speIpt01} disabled={this.state.selectedSubDisasterCategory === null} onChange={() => this.onSelectDisaster(null, disasterData)}/>
														<label onClick={(event) => this.onSelectDisaster(event, disasterData)}>{disasterData.disasterName}</label>
													</li>
													))
                                            }
											<li>
												<input ref={this.refNewDisasterRadio} type="radio" name="speIpt" id={NewSOPOptions.cssStyles.speIpt01} disabled={this.state.selectedSubDisasterCategory === null} onChange={() => this.onSelectNewDisaster()} />
												<input ref={this.refNewDisaster} type="text" className={bodyStyles.fullText} disabled={this.state.selectedSubDisasterCategory === null} placeholder={SopManagerResource.ID.placeHolders.newSOP} onChange={() => this.onChangeDisasterText()} />
											</li>
										</ul>
									</div>
								</div>
							</div>
						</div>
					</div>
				</div>


				<div className={NewSOPOptions.cssStyles.speBot}>
					<a className={NewSOPOptions.cssStyles.clickable} onClick={() => this.onClickCancel()}>{SopManagerResource.ID.common.cancel}</a>
					&nbsp;&nbsp;
					<a className={NewSOPOptions.cssStyles.clickable} onClick={() => this.onClickApply()}>{SopManagerResource.ID.common.make}</a>
				</div>

			</div>
        );
    }
}

export default NewSOPOptions;