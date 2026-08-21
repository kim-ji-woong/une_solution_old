import React, { Component } from 'react';
import { BrowserRouter as Route, Link } from 'react-router-dom';

import styles from '../../Common/css/style.module.css';

import TeamEditorResource from '../resource/id';

class ScheduleMenu extends Component {
	static cssStyles = styles;
	//static textFixed = "고정 근무표";
	//static textCurrent = "실시간 근무표";

	constructor(props) {
		super(props);

		this.state = {
			fixedClass: ScheduleMenu.cssStyles.current,
			currentClass: null,
		}

		this.props = props;
	}

	onClickList(e) {
		//console.log('onClickList');
		var target = e;

		// 각각의 state 값에 저장 한 후 해당 클래스 네임에 입력하기
		if (target.innerText == TeamEditorResource.ID.textFixed && this.state.fixedClass !== ScheduleMenu.cssStyles.current) {
			this.setState({ fixedClass: ScheduleMenu.cssStyles.current, currentClass: null });
			this.props.onChange(TeamEditorResource.ID.textFixed);
		} else if (target.innerText == TeamEditorResource.ID.textCurrent && this.state.currentClass !== ScheduleMenu.cssStyles.current) {
			this.setState({ fixedClass: null, currentClass: ScheduleMenu.cssStyles.current });
			this.props.onChange(TeamEditorResource.ID.textCurrent);
		}

		return;
	}

	render() {
		return (
			
			<div className={styles.saRht + " " + styles.pt60}>
				<div className={styles.sarSel}>
					<h3>{TeamEditorResource.ID.textSchedule}</h3>
				</div>
				<div>
					<ul className={styles.sarList}>
						<li><a onClick={(e) => this.onClickList(e.target)} className={this.state.fixedClass}>{TeamEditorResource.ID.textFixed}</a></li>
						<li><a onClick={(e) => this.onClickList(e.target)} className={this.state.currentClass}>{TeamEditorResource.ID.textCurrent}</a></li>
					</ul>
				</div>
			</div>

            
        );
    }
}

export default ScheduleMenu;