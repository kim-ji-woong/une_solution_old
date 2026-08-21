import React, { Component } from 'react';
import { BrowserRouter as Route, Link } from 'react-router-dom';

import styles from '../../Common/css/style.module.css';
import TeamEditorResource from '../resource/id';
import uneStyles from '../../Common/css/uneCommon.module.css';
//import $ from 'jquery';

//import ProjectResource from '../../Root/resource/id';
//import AccountResource from '../../Account/resource/id';
//import SessionString from '../../Common/js/sessionString';

class TeamEditorContent extends Component {
	static cssStyles = styles;

	constructor(props) {
		super(props);

		this.state = {
			teamClass: TeamEditorContent.cssStyles.on,
		    scheduleClass: null,
			isEditMode: false,
        }

		this.props = props;
	}

	onClickMenu = (e) => {
		var target = e;

		// 각각의 state 값에 저장 한 후 해당 클래스 네임에 입력하기
		if (target.innerText == TeamEditorResource.ID.textRegular && this.state.teamClass !== TeamEditorContent.cssStyles.on) {
			this.setState({ teamClass: TeamEditorContent.cssStyles.on, scheduleClass: null });
			this.props.changeMenuType(TeamEditorResource.ID.textRegular);
		}
		else if (target.innerText == TeamEditorResource.ID.textSchedule && this.state.scheduleClass !== TeamEditorContent.cssStyles.on) {
			this.setState({ teamClass: null, scheduleClass: TeamEditorContent.cssStyles.on });
			this.props.changeMenuType(TeamEditorResource.ID.textSchedule);
		}
	}

	//편집모드 event
	//onClickEdit = () => {
	//	// 권한 체크
	//	const userAuthor = ProjectResource.getUserAuthor();

	//	if (userAuthor !== AccountResource.ID.accountLevel.admin) {
	//		this.props.onAuthorError();
	//		return;
	//	}

	//	let chk = this.state.isEditMode;

	//	if (chk === false) {
	//		chk = true;
	//	} else {
	//		chk = false;
	//	}

	//	this.setState({ isEditMode: chk });
	//	this.props.isEditMode(chk);
	//	return;
	//}

	//save = () => {
	//	if (!this.state.isEditMode)
	//		return;

	//	this.props.save();
	//}

	//getSaveEnabled() {
	//	let saveClass = "";

	//	if (!this.state.isEditMode)
	//		saveClass = uneStyles.disabled;

	//	return saveClass;
 //   }

	render() {

		return (

			<div className={styles.saLeft}>
				<div className={uneStyles.aslWrap + " " + uneStyles.typeH}>
					<Link to="/team-editor" className={styles.salHome}>홈</Link>
					<div className={styles.salMenu + " " + this.state.teamClass}>
					{/*<div className={styles.salMenu}>*/}
						<a onClick={(e) => this.onClickMenu(e.target)} className={styles.salIco + " " + styles.ico0101}>{TeamEditorResource.ID.textRegular}</a>
						{
							//<dl className={styles.salCont + " " + uneStyles.salCont}>
							//	<dt onClick={this.onClickEdit}><input type="checkbox" id="salChk01" onChange={this.onChangeEdit} checked={this.state.isEditMode} /><label>편집</label></dt>
							//	<dd><a className={this.getSaveEnabled()} onClick={this.save} >저장</a></dd>
							//</dl>
						}
					</div>
					{/*
					<div className={styles.salMenu + " " + this.state.scheduleClass}>
						<a onClick={(e) => this.onClickMenu(e.target)} className={styles.salIco + " " + styles.ico0102}>{TeamEditorResource.ID.textSchedule}</a>
						<dl className={styles.salCont + " " + uneStyles.salCont}>
							<dt onClick={this.onClickEdit}><input type="checkbox" id="salChk01" onChange={this.onChangeEdit} checked={this.state.isEditMode} /><label>편집</label></dt>
							<dd><a href="#">뒤로가기</a></dd>
							<dd><a href="#">되돌리기</a></dd>
							<dd><a href="#">저장</a></dd>
						</dl>
					</div>
					*/}
				</div>
			</div>

        );
    }
}

export default TeamEditorContent;