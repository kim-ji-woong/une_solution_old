import React, { Component } from 'react';

import styles from '../../Common/css/style.module.css';
import commons from '../../Common/css/common.module.css';

class AlarmWindow extends Component {
	constructor(props) {
		super(props);
		this.state = {
			sensorAlarms: null,			// 발생된 모든 알람
			currentNum: 0,				// 현재 표시되고 있는 알람 순번
			prevClass: null,			// 이전 알람 버튼 클래스
			nextClass: null,			// 다음 알람 버튼 클래스
		}

		this.props = props;

		if (this.props.sensorAlarms != null) {
			this.state.sensorAlarms = this.props.sensorAlarms;

			if (this.state.sensorAlarms.length > 1) {
				this.state.nextClass = styles.current;
            }
        }
	}

	componentDidUpdate(prevProps, prevState) {
		// 새로운 알람이 감시될 경우
		if (prevProps.sensorAlarms != this.props.sensorAlarms) {
			let prevClass = null;
			let nextClass = null;
			let sensorAlarms = this.props.sensorAlarms;
			let currentNum = this.state.currentNum;

			// 현재 표시되고 있는 알람 순번보다 새로운 알람 갯수가 적을 경우
			if (sensorAlarms.length < currentNum + 1) {
				currentNum = sensorAlarms.length - 1;
			}

			if (sensorAlarms.length > currentNum + 1) {
				nextClass = styles.current;
			}

			if (0 < currentNum) {
				prevClass = styles.current;
			}

			this.setState({ sensorAlarms: sensorAlarms, currentNum: currentNum, prevClass: prevClass, nextClass: nextClass });
        }
	}

	componentWillMount() {
		//console.log('componentWillMount');
	}

	componentWillUpdate() {
		//console.log('componentWillUpdate');
	}

	componentDidMount() {
		//console.log('componentDidMount');
	}

	onClickPrev = () => {
		// 현재 알람 순번이 처음보다 작거나 같으면 리턴
		if (this.state.currentNum <= 0)
			return;

		let currentNum = this.state.currentNum - 1;
		let prevClass = null;
		let nextClass = null;

		if (this.state.sensorAlarms.length > currentNum + 1) {
			nextClass = styles.current;
		}

		if (0 < currentNum) {
			prevClass = styles.current;
		}

		this.setState({ currentNum: currentNum, prevClass: prevClass, nextClass: nextClass });
	}

	onClickNext = () => {
		// 현재 알람 순번이 마지막 순번보다 크거나 같으면 리턴
		if (this.state.currentNum + 1 >= this.state.sensorAlarms.length)
			return;

		let currentNum = this.state.currentNum + 1;
		let prevClass = null;
		let nextClass = null;

		if (this.state.sensorAlarms.length > currentNum + 1) {
			nextClass = styles.current;
		}

		if (0 < currentNum) {
			prevClass = styles.current;
		}

		this.setState({ currentNum: currentNum, prevClass: prevClass, nextClass: nextClass });
    }

	render() {
		let alarm = null;
		let alarmType = "";		// 알람 타입
		let alarmMeg = "";		// 알람 메시지
		let date = null;		// 알람 발생 시각
		let hours = null;		// 알람 발생 시간
		let minutes = null;		// 알람 발생 분
		let time = "";			// 알람 발생 AM/PM
		
		if (this.state.sensorAlarms != null) {
			if (this.state.sensorAlarms[this.state.currentNum] != null) {
				alarm = this.state.sensorAlarms[this.state.currentNum];
				//alarmType = alarm.facilityTypeName;
				alarmMeg = alarm.message;
				date = new Date(alarm.dtTime);
				hours = date.getHours();
				minutes = date.getMinutes();

				if (minutes < 10) {
					minutes = '0' + minutes;
				}

				time = "AM";
				if (hours > 12) {
					time = "PM";
				}
            }
		}

		return (
			<div id={styles.mnBot}>
				<div className={commons.container}>
					<div className={styles.mnbTime}>
						<h5>{hours}:{minutes}</h5>
						<span>{time}</span>
					</div>
					<div className={styles.mnbFire}>
						<h5>화재</h5>
						<div>
							<p>{alarmMeg}</p>
							<ul>
								<li className={styles.current}><span>관심</span></li>
								<li className={styles.current}><span>주의</span></li>
								<li className={styles.current}><span>경계</span></li>
								<li className={styles.current}><span>심각</span></li>
							</ul>
						</div>
					</div>
					<div className={styles.mnbNav}>
						<a onClick={this.onClickPrev} className={styles.prev + " " + this.state.prevClass}>이전</a>
						<a onClick={this.onClickNext} className={styles.next + " " + this.state.nextClass}>다음</a>
					</div>
				</div>
			</div>
        );
    }
}

export default AlarmWindow;