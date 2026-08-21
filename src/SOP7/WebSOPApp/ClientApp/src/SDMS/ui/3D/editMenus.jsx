import React, { Component } from 'react';
import contentStyles from '../../../Common/css/content.module.css';
import uiStyles from '../../../Common/css/ui.module.css';
import SDMS from '../sdms';
import $ from 'jquery';
import Contents3D from './contents3D';
import { FakeWallManager } from './fakeWallManager';
import CCTVInfo from '../popups/cctvInfo';
import { TextPOIManager } from './textPOIManager';

class EditMenus extends Component {
	constructor(props) {
		super(props);

		this.refQuickButton = React.createRef();
	}

	save = () => {
		this.props.saveEditDatas();
	}

	popupBtm = () => {
		const buttons = this.refQuickButton.current;

		if (buttons) {
			if (buttons.classList.contains(uiStyles.off)) {
				buttons.classList.add(uiStyles.on);
				buttons.classList.remove(uiStyles.off);
				$(buttons).slideUp();
			}
			else/* if (buttons.classList.contains('on'))*/ {
				buttons.classList.add(uiStyles.off);
				buttons.classList.remove(uiStyles.on);
				$(buttons).slideDown();
			}
		}
	}

	getQuickButtonClassName(editMode) {
		if (this.props.editMode === editMode) {
			return uiStyles.on;
		}

		return uiStyles.off;
	}

	onClickFakeWall() {
		if (!this.props.editModeParam) {
			this.props.setEditModeItem(Contents3D.Edit_Mode_FakeWall, FakeWallManager.Mode_Add_NoClick);
		}
		else {
			this.props.setEditModeItem(Contents3D.Edit_Mode_FakeWall, this.props.editModeParam);
        }
	}

	onClickCCTVGroup() {
		if (this.props.editMode === Contents3D.Edit_Mode_CCTVGroup) {
			return;
		}

		//if (!this.props.editModeParam) {
			this.props.setEditModeItem(Contents3D.Edit_Mode_CCTVGroup, CCTVInfo.Mode_Select_Sensor);
		/*}
		else {
			this.props.setEditModeItem(Contents3D.Edit_Mode_CCTVGroup, this.props.editModeParam);
		}*/
    }

	render() {
		const cctvVisible = this.props.editModeCCTV ? uiStyles.on : uiStyles.off;

        return (
			<div id={contentStyles.dsh}>
				{
					this.props.isEditMode &&
					<>
						<div id={contentStyles.edTitle}>
							<h2>편집모드</h2>
							<a className={contentStyles.edtClose} onClick={() => this.props.setEditMode(false)}>종료</a>
							<a className={contentStyles.edtSave} onClick={this.save}>저장</a>
						</div>

						<div id={uiStyles.dsBot}>
							<button className={uiStyles.edit} onClick={this.popupBtm}></button>
							<ul ref={this.refQuickButton} className={uiStyles.edit}>
								<li><a className={uiStyles.poi + " " + this.getQuickButtonClassName(Contents3D.Edit_Mode_MovePOI)} onClick={() => this.props.setEditModeItem(Contents3D.Edit_Mode_MovePOI, null)}><span><em>POI</em></span></a></li>
								<li><a className={uiStyles.fakeWall + " " + this.getQuickButtonClassName(Contents3D.Edit_Mode_FakeWall)} onClick={() => this.onClickFakeWall()}><span><em>가벽</em></span></a></li>
								<li><a className={uiStyles.equipZoneName + " " + this.getQuickButtonClassName(Contents3D.Edit_Mode_Text)} onClick={() => this.props.setEditModeItem(Contents3D.Edit_Mode_Text, TextPOIManager.Mode_MoveText)}><span><em>구역명</em></span></a></li>
								<li><a className={uiStyles.cctvGroup + " " + this.getQuickButtonClassName(Contents3D.Edit_Mode_CCTVGroup)} onClick={() => this.onClickCCTVGroup()}><span><em>구역별<br/>CCTV</em></span></a></li>
								<li><a className={uiStyles.cctv + " " + cctvVisible} onClick={() => this.props.setEditModeCCTV(!this.props.editModeCCTV)}><span><em>CCTV<br />보기</em></span></a></li>
							</ul>
						</div>
					</>
				}
				{
					/*<ul id={contentStyles.edtBot}>
						<li><a href="#" className={contentStyles.on}>POI</a></li>
						<li><a href="#">가벽</a></li>
						<li><a href="#">구역명</a></li>
						<li><a href="#">그룹핑</a></li>
					</ul>*/
				}
				<div id={contentStyles.dsMap}>
					<h3 className={contentStyles.dsmTitle}>{this.props.currentZoneName}</h3>
				</div>
            </div>
        );
    }
}

export default EditMenus;