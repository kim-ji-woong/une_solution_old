import React, { Component } from 'react';

import styles from '../../Common/css/style.module.css';
import bodyStyles from '../css/body.module.css';
import sectionStyles from '../../Common/css/section.module.css';

import SopManagerResource from '../resource/id';
import PanelAreas from './panelAreas';
import SopController from '../services/sopController';
import SectionData from '../../Common/models/sections/sectionData';
import SectionDataAnnotation from '../../Common/models/sections/sectionDataAnnotation';
import SectionDataDecision from '../../Common/models/sections/sectionDataDecision';
import SectionDataEndpoint from '../../Common/models/sections/sectionDataEndpoint';
import SectionDataInternal from '../../Common/models/sections/sectionDataInternal';
import SectionDataProcess from '../../Common/models/sections/sectionDataProcess';
import JsonManager from '../services/jsonManager';
import NewSOPOptions from './body/newSOPOptions';
import SopManagerBodyMain from './body/sopManagerBodyMain';
import SopManager from './sopManager';
import SaveSOPOptions from './popup/saveSOPOptions';

//import $ from 'jquery';

class SopManagerBody extends Component {
	static cssStyles = styles;

	/*static Menu_New_SOP = 0;
	static Menu_Open_SOP = 1;
	static Menu_Save_SOP = 2;
	static Menu_Delete_SOP = 3;
	static Menu_None = 4;*/

	constructor(props) {
		super(props);

		this.props = props;

		/*this.state =
		{
			currentMenu: { dataType: "" },
			sopData: null,
			menuType: SopManagerBody.Menu_None,
			menuDatas: null,
			// sectionData + actionStep
			selectedSectionData: [],
			loading: true
		};*/
	}

	componentDidMount() {
		//this.getSOPName();
		/*const element = document.getElementById("idAAA");

		$(element).resizable({
			handles: 'e',       //  좌우넓이 조절
			minWidth: 300,     // 최소 넓이값 설정
			resize: function (event, ui) {
				$(this).css("height", "50px");    // 기본 높이값 셋팅
				var cellWidth = $(this).outerWidth();    // 넓이값
				$(this).css("width", cellWidth + "px");
				//$('.sectionGrid > .sectionGridColumn[data-index="' + $(this).index() + '"]').css("width", cellWidth + "px");
			}
		});*/

		/*$(".sectionGridFix .sectionGridRow .sectionGridCell").resizable({
			handles: 's',       //  상하높이 조절
			minHeight: 200,     // 최소 높이값 설정
			resize: function (event, ui) {
				$(this).css("width", "50px");    // 기본 넓이값 셋팅
				var cellHeight = $(this).outerHeight();    // 높이값
				console.log("cellHeight ::: " + cellHeight);
				$('.sectionGrid .sectionGridCell[data-index="' + $(this).index() + '"]').css("height", cellHeight + "px");
			}
		});

		$(".sectionGridFix .sectionGridColumn .sectionGridCell").resizable({
			handles: 'e',       //  좌우넓이 조절
			minWidth: 300,     // 최소 넓이값 설정
			resize: function (event, ui) {
				$(this).css("height", "50px");    // 기본 높이값 셋팅
				var cellWidth = $(this).outerWidth();    // 넓이값
				$('.sectionGrid > .sectionGridColumn[data-index="' + $(this).index() + '"]').css("width", cellWidth + "px");
			}
		});*/
	}

	async getSOPName() {
		//const response = await fetch('SOPManager/SOP/Name');
		//const sopName = await response.text();
		//const currentMenu = { ...this.state.currentMenu };

		/*const datas = await response.json();
		if (datas !== null) {
			for (let i = 0; i < datas.length; i++) {
				const disasterCategory = datas[i].disasterCategory;
				const disasterCategoryLog = `DisasterCategory[${i}] => ID : ${disasterCategory.id}, CategoryName : ${disasterCategory.categoryName}, SiteID : ${disasterCategory.siteID}`;
				console.log(disasterCategoryLog);

				for (let j = 0; j < datas[i].subDisasterCategories.length; j++) {
					const subDisasterCategory = datas[i].subDisasterCategories[j];
					const subDisasterCategoryLog = `SubDisasterCategory[${i}, ${j}] => ID : ${subDisasterCategory.id}, SubCategoryName : ${subDisasterCategory.subCategoryName}`;
					console.log(subDisasterCategoryLog);
				}
			}
		}*/

		//this.setState({ currentMenu: currentMenu, sopData: this.state.sopData, menuType: SopManagerBody.Menu_None, menuDatas: null, selectedSectionData: this.state.selectedSectionData, loading: false });
	}

	/*onChangeMenu = (dataType) => {
		this.state.currentMenu.dataType = dataType;
	}

	onSelectMainMenu = (menuType) => {
		this.processMainMenu(menuType);
	}

	async processMainMenu(menuType) {
		if (menuType === SopManagerBody.Menu_Save_SOP) {
			await SopController.saveDB({ ...this.state.sopData });
		}
		else if (menuType === SopManagerBody.Menu_New_SOP) {
			const disasterCategories = await SopController.newSOP();
			this.setState({ currentMenu: this.state.currentMenu, sopData: this.state.sopData, menuType: menuType, menuDatas: disasterCategories, selectedSectionData: this.state.selectedSectionData, loading: false });
		}
		else if (menuType === SopManagerBody.Menu_Open_SOP) {
			const disasterCategories = await SopController.openDB();
			this.setState({ currentMenu: this.state.currentMenu, sopData: this.state.sopData, menuType: menuType, menuDatas: disasterCategories, selectedSectionData: this.state.selectedSectionData, loading: false });
		}
	}

	onAddComponent = (sectionData, actionStep) => {
		if (sectionData === null) {
			return;
		}

		const stepMember = SopManagerBody.getStepMember(actionStep);

		if (stepMember !== null) {
			const _sectionData = SopManagerBody.getSectionData(stepMember, sectionData.gridColumnIndex, sectionData.gridRowIndex);

			if (_sectionData === null) {
				stepMember.sections.push(sectionData);
			}
			else {
				SopManagerBody.copySectionData(sectionData, _sectionData);
			}

			const currentMenu = { ...this.state.currentMenu };
			this.setState({ currentMenu: currentMenu, sopData: this.state.sopData, menuType: this.state.menuType, menuDatas: this.state.menuDatas, selectedSectionData: this.state.selectedSectionData, loading: false });
		}
	}

	static copySectionData(src, trg) {
		SectionData.copyTo(src, trg);

		if (src.componentType === SectionDataAnnotation.getComponentType()) {
			SectionDataAnnotation.copyTo(src, trg);
		}
		else if (src.componentType === SectionDataDecision.getComponentType()) {
			SectionDataDecision.copyTo(src, trg);
		}
		else if (src.componentType === SectionDataEndpoint.getComponentType()) {
			SectionDataEndpoint.copyTo(src, trg);
		}
		else if (src.componentType === SectionDataInternal.getComponentType()) {
			SectionDataInternal.copyTo(src, trg);
		}
		else if (src.componentType === SectionDataProcess.getComponentType()) {
			SectionDataProcess.copyTo(src, trg);
		}
	}

	static getStepMember(actionStep) {
		let stepMember = null;

		if (actionStep.stepMemberDatas && actionStep.stepMemberDatas.length > 0) {
			stepMember = actionStep.stepMemberDatas[0];
		}

		if (stepMember !== null) {
			if (!stepMember.sections) {
				stepMember.sections = [];
			}

			if (!stepMember.arrows) {
				stepMember.arrows = [];
			}
		}

		return stepMember;
	}

	static getSectionData(stepMember, columnIndex, rowIndex) {
		if (stepMember.sections) {
			for (let i = 0; i < stepMember.sections.length; i++) {
				const sectionData = stepMember.sections[i];

				if (sectionData.gridColumnIndex === columnIndex &&
					sectionData.gridRowIndex === rowIndex) {
					return sectionData;
				}
			}
		}

		return null;
	}

	onRemoveComponent = (columnIndex, rowIndex, actionStep) => {
		const stepMember = SopManagerBody.getStepMember(actionStep);

		if (stepMember !== null) {
			if (stepMember.sections) {
				for (let i = 0; i < stepMember.sections.length; i++) {
					const sectionData = stepMember.sections[i];

					if (sectionData.gridColumnIndex === columnIndex && sectionData.gridRowIndex === rowIndex) {
						stepMember.sections.splice(i, 1);
						this.setState({ currentMenu: this.state.currentMenu, sopData: this.state.sopData, menuType: this.state.menuType, menuDatas: this.state.menuDatas, selectedSectionData: this.state.selectedSectionData, loading: false });
						break;
					}
				}
			}
		}
	}

	onSelectComponent = (sectionData, actionStep) => {
		const currentMenu = { ...this.state.currentMenu };
		this.setState({ currentMenu: currentMenu, sopData: this.state.sopData, menuType: this.state.menuType, menuDatas: this.state.menuDatas, selectedSectionData: [sectionData, actionStep], loading: false });
	}

	onApplyComponentProperty = (sectionData, actionStep) => {
		const stepMember = SopManagerBody.getStepMember(actionStep);

		if (stepMember !== null) {
			const _sectionData = SopManagerBody.getSectionData(stepMember, sectionData.gridColumnIndex, sectionData.gridRowIndex);

			if (_sectionData === null) {
				return;
			}
			else {
				SopManagerBody.copySectionData(sectionData, _sectionData);
			}

			const currentMenu = { ...this.state.currentMenu };
			this.setState({ currentMenu: currentMenu, sopData: this.state.sopData, menuType: this.state.menuType, menuDatas: this.state.menuDatas, selectedSectionData: [], loading: false });
		}
	}

	onNewSOP = (disasterCategory, subDisasterCategory, disaster) => {
		const currentMenu = { ...this.state.currentMenu };
		//const sopData = new SOPData(disasterCategory, subDisasterCategory, disaster, disaster.actionSteps);
		const sopData = {
			disasterCategory: disasterCategory,
			subDisasterCategory: subDisasterCategory,
			disaster: disaster,
			actionSteps: disaster.actionSteps,
			currentActionStep: null
		}

		if (disaster.actionSteps) {
			for (let i = 0; i < disaster.actionSteps.length; i++) {
				const actionStepData = disaster.actionSteps[i];

				if (actionStepData) {
					if (!actionStepData.stepMemberDatas) {
						actionStepData.stepMemberDatas = [];
					}

					if (actionStepData.stepMemberDatas.length === 0) {
						const stepMemberData = JsonManager.newStepMemberData();

						actionStepData.stepMemberDatas.push(stepMemberData);
						//actionStepData.stepMemberDatas.push(new StepMemberData(-1, '', [], []));
					}

					if (actionStepData.actionStep && !sopData.currentActionStep)
						sopData.currentActionStep = actionStepData;
				}
			}
		}

		this.setState({ currentMenu: currentMenu, sopData: sopData, menuType: SopManagerBody.Menu_None, menuDatas: null, selectedSectionData: this.state.selectedSectionData, loading: false });
	}

	onOpenSOP = (disasterCategory, subDisasterCategory, disaster) => {
		this.openSOP(disasterCategory, subDisasterCategory, disaster);
	}

	async openSOP(disasterCategory, subDisasterCategory, disaster) {
		if (disaster?.disaster?.id) {
			const actionStepDatas = await SopController.openDB(disaster.disaster.id);

			if (actionStepDatas) {
				disaster.actionSteps = actionStepDatas;

				for (let i = 0; i < actionStepDatas.length; i++) {
					const actionStepData = actionStepDatas[i];

					if (actionStepData?.actionStep) {
						disaster.currentActionStep = actionStepData;
						break;
					}
				}

				const sopData = {
					disasterCategory: disasterCategory,
					subDisasterCategory: subDisasterCategory,
					disaster: disaster,
					actionSteps: disaster.actionSteps,
					currentActionStep: disaster.currentActionStep
				}

				this.setState({ currentMenu: this.state.currentMenu, sopData: sopData, menuType: SopManagerBody.Menu_None, menuDatas: actionStepDatas, selectedSectionData: this.state.selectedSectionData, loading: false });
			}
		}
	}

	makeNewSOPOptions() {
		const newSOPData = this.state.menuType === SopManagerBody.Menu_New_SOP ? this.state.menuDatas : null;

		if (newSOPData) {
			return <NewSOPOptions disasterCategories={newSOPData} onNewSOP={this.onNewSOP} />
		}

		return <></>
	}

	makeOpenSOPOptions() {
		const openSOPData = this.state.menuType === SopManagerBody.Menu_Open_SOP ? this.state.menuDatas : null;

		if (openSOPData) {
			return <OpenSOPOptions disasterCategories={openSOPData} onOpenSOP={this.onOpenSOP} />
		}

		return <></>
	}

	selectActionStep(actionStepName) {
		console.log("selectActionStep : " + actionStepName);
	}

	selectSopMode(isNormal) {
		console.log("selectSopMode : " + isNormal);
	}*/

	getBodyContents() {
		if (this.props.menu === SopManager.menu.editSOP ||
			this.props.menu === SopManager.menu.open) {
			return <SopManagerBodyMain sopData={this.props.menuDatas} loginUser={this.props.loginUser} showCascading={this.props.showCascading} content={this.props.content} changeCascadingMode={this.props.changeCascadingMode} />;
		}
		else if (this.props.menu === SopManager.menu.newSOP) {
			return <NewSOPOptions content={this.props.content} sopData={this.props.sopData} loginUser={this.props.loginUser} />;
		}
		else if (this.props.menu === SopManager.menu.save) {
			return <SaveSOPOptions sopData={this.props.menuDatas} content={this.props.content} loginUser={this.props.loginUser} />;
		}

		return <></>;
    }

	render() {
		return (
			<div className={bodyStyles.sopCont}>
				<h2 id={SopManagerBody.cssStyles.sopTitle}>{SopManagerResource.ID.projectName}</h2>
				{
					this.getBodyContents()
                }
			</div>
		);
	}
}

export default SopManagerBody;