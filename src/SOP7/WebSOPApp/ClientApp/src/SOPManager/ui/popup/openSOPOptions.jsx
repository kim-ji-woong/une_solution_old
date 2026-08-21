import React, { Component } from 'react';
import styles from '../../../Common/css/style.module.css';
import bodyStyles from '../../css/body.module.css';
import SopManagerResource from '../../resource/id';
import SopController from '../../services/sopController';
import SopManager from '../sopManager';
import $ from 'jquery';
import '../../../Common/js/treeview.js';
import '../../../TeamEditor/ui/utility/css/style.css'; /* 사용중인것, 지우지마세요 */

class OpenSOPOptions extends Component {
	static cssStyles = styles;

	constructor(props) {
		super(props);
		this.props = props;

		this.state = {
			disasterCategories: [],
			isNormal: true,
			selectedDisaster: null,
			selectedVersion: null,
			loading: true,
			loadingMessage: SopManagerResource.ID.messages.loadingData
		};
	}

	componentDidMount() {
		this.onChangeSopMode(this.state.isNormal);
		$(document).ready(function () {
			$('.treeview').hummingbird();
		})
    }

	onChangeSopMode(isNormal) {
		this.getDisasterCategories(isNormal);
		//this.setState({ isNormal: isNormal });
	}

	async getDisasterCategories(isNormal) {
		const [disasterCategories, message] = await SopController.disasterCategories(isNormal);

		if (disasterCategories) {
			if (isNormal) {
				this.setState({ loading: false, isNormal: isNormal, selectedDisaster: null, disasterCategories: disasterCategories });
			}
			else {
				this.setState({ loading: false, isNormal: isNormal, selectedDisaster: null, disasterCategories: disasterCategories });
			}
		}
		else {
			this.setState({ loading: true, loadingMessage: message, isNormal: isNormal, selectedDisaster: null });
		}
	}

	onClickClose() {
		// 원래 상태 그대로 돌려준다.
		this.props.content(SopManager.menu.editSOP, this.props.sopData);
	}

	onClickTreeNode = (event) => {
		//if (event.target.classList.contains("fa-minus")) {
		//	event.target.classList.remove("fa-minus");
		//	event.target.classList.add("fa-plus");
		//}
		//else if (event.target.classList.contains("fa-plus")) {
		//	event.target.classList.remove("fa-plus");
		//	event.target.classList.add("fa-minus");
		//}

		//this.setState({ loading: false });
	}

	onClickDisaster(disasterData) {
		this.setState({ selectedDisaster: disasterData });
	}

	onClickOpen = (event) => {
		if (this.state.selectedVersion) {
			this.props.content(SopManager.menu.open, this.state.selectedVersion.id);
		}
		else {
			alert(SopManagerResource.ID.messages.selectSOPVersion);
        }
	}

	tbRdo(event, version) {
		const tr = event.target.parentElement;

		for (let i = 0; i < tr.parentElement.children.length; i++)
		{
			const row = tr.parentElement.children[i];

			if (row === tr) {
				continue;
			}
			else {
				row.classList.remove(OpenSOPOptions.cssStyles.on);
            }
        }

		tr.classList.add(OpenSOPOptions.cssStyles.on);
		this.setState({ selectedVersion: version });
	};

	getDisasterVersion(disaster) {
		if (disaster?.version) {
			this.versionCount = this.versionCount + 1;

			return (
				<tr key={"version_" + this.versionCount} onClick={(event) => this.tbRdo(event, disaster.version)}>
					<td>{disaster.version.versionName}</td>
					<td>{disaster.owner}</td>
					<td>{disaster.version.createTime.toString().replace('T', ' ')}</td>
					<td>{disaster.version.lastAccessTime.toString().replace('T', ' ')}</td>
					<td className={OpenSOPOptions.cssStyles.tal}>{disaster.version.description}</td>
				</tr>
				);
		}

		return <></>
    }

	getDisasterVersions(disasterData) {
		this.versionCount = 0;

		if (disasterData) {
			return (
				<table className={OpenSOPOptions.cssStyles.scTb}>
					<caption>버전명, 작성자, 생성일자, 수정일자, 부가설명으로 구성된 표</caption>
					<colgroup>
						<col className={bodyStyles.col10Pro} />
						<col className={bodyStyles.col10Pro} />
						<col className={bodyStyles.col20Pro} />
						<col className={bodyStyles.col20Pro} />
						<col className={bodyStyles.col40Pro} />
					</colgroup>
					<thead>
						<tr>
							<th>버전명</th>
							<th>작성자</th>
							<th>생성일자</th>
							<th>수정일자</th>
							<th>부가설명</th>
						</tr>
					</thead>
					<tbody>
						{
							disasterData.disasterDatas && (
								disasterData.disasterDatas.map(disaster => (this.getDisasterVersion(disaster)))
							)
						}
					</tbody>
				</table>
			);
		}

		return <></>
    }

	getDisasterContents(disasterData) {
		this.disasterCount = this.disasterCount + 1;
		const className = disasterData === this.state.selectedDisaster ? "treeviewLastItem " + OpenSOPOptions.cssStyles.selectedTreeNode + " " + OpenSOPOptions.cssStyles.clickable : "treeviewLastItem " + OpenSOPOptions.cssStyles.clickable;

		return (
			<li key={"disaster_" + this.disasterCount} className={className} onClick={() => this.onClickDisaster(disasterData)}>{disasterData.disasterName}</li>
		);
	}

	getSubDisasterCategoryContents(subDisasterCategoryData) {
		if (!subDisasterCategoryData.subDisasterCategory) {
			return <></>
		}

		this.sdcCount = this.sdcCount + 1;

		if (subDisasterCategoryData.disasterDatas && subDisasterCategoryData.disasterDatas.length > 0) {
			return (
				<li key={"sdc_" + this.sdcCount}>
					<i className="fa-minus" onClick={this.onClickTreeNode}>더보기</i><h5>{subDisasterCategoryData.subDisasterCategory.subCategoryName}</h5>
					{
						subDisasterCategoryData.disasterDatas && (
							<ul>
								{
									subDisasterCategoryData.disasterDatas.map(disasterData => this.getDisasterContents(disasterData))
								}
							</ul>
						)
					}
				</li>
			);
        }

		return (
			<li key={"sdc_" + this.sdcCount} className="treeviewLastItem">{subDisasterCategoryData.subDisasterCategory.subCategoryName}</li>
		);
	}

	getDisasterCategoryContents(disasterCategoryData) {
		if (!disasterCategoryData.disasterCategory) {
			return <></>
		}

		this.dcCount = this.dcCount + 1;

		return (
			<li key={"dc_" + this.dcCount}>
				<i className="fa-minus" onClick={this.onClickTreeNode}>더보기</i><h5>{disasterCategoryData.disasterCategory.categoryName}</h5>
				{
					disasterCategoryData.subDisasterCategories && (
					<ul>
					{
							disasterCategoryData.subDisasterCategories.map(subDisasterCategoryData => this.getSubDisasterCategoryContents(subDisasterCategoryData))
					}
					</ul>
					)
				}
			</li>
		);
    }

	render() {
		this.dcCount = 0;
		this.sdcCount = 0;
		this.disasterCount = 0;

		return (
			<div id={OpenSOPOptions.cssStyles.sopPop}>
				<div>
					<div>
						<div className={OpenSOPOptions.cssStyles.spPop + " " + OpenSOPOptions.cssStyles.sopOpen}>
							<div className={OpenSOPOptions.cssStyles.sppTop}>
								<h4>SOP열기</h4>
								<a className={OpenSOPOptions.cssStyles.clickable} onClick={() => this.onClickClose()}>닫기</a>
							</div>
							<div className={OpenSOPOptions.cssStyles.sppSel}>
								<h5>전체 SOP</h5>
								<label className={OpenSOPOptions.cssStyles.clickable}>
									<input type="radio" name="sppSel" className={bodyStyles.labelInput} checked={this.state.isNormal} onChange={() => this.onChangeSopMode(true)} />
									{SopManagerResource.ID.sopMode.normal}
								</label>
								<label className={OpenSOPOptions.cssStyles.clickable}>
									<input type="radio" name="sppSel" className={bodyStyles.labelInput} checked={!this.state.isNormal} onChange={() => this.onChangeSopMode(false)} />
									{SopManagerResource.ID.sopMode.abnormal}
								</label>
							</div>
							<div className={OpenSOPOptions.cssStyles.sppCont}>
								<div className={OpenSOPOptions.cssStyles.sppLft}>
									<div className={OpenSOPOptions.cssStyles.scrollbarOuter}>
										<ul className={styles.sarTree + ' treeview'}>
										{
											this.state.disasterCategories && (
												this.state.disasterCategories.map(disasterCategoryData => (this.getDisasterCategoryContents(disasterCategoryData)))
											)
										}
										</ul>
									</div>
								</div>
								<div className={OpenSOPOptions.cssStyles.sppRht}>
									<div className={OpenSOPOptions.cssStyles.scrollbarOuter}>
										<div className={OpenSOPOptions.cssStyles.spprCont}>
										{
											this.getDisasterVersions(this.state.selectedDisaster)
                                        }
										</div>
									</div>
									<div className={OpenSOPOptions.cssStyles.spprBot}>
										<a className={OpenSOPOptions.cssStyles.blu} onClick={this.onClickOpen}>SOP 열기</a>
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

export default OpenSOPOptions;