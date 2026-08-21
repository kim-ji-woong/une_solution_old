import React, { Component } from 'react';
import { BrowserRouter as Route, Link } from 'react-router-dom';
import { withRouter } from 'react-router-dom';
import styles from '../../css/sdms.module.css';


class DetectionTextInfo extends Component {
    constructor(props) {
        super(props);

        this.state = {
            loading: false
        }

        this.props = props;
        this.ignoreSensorZoneHistories = {};
    }

    componentDidMount() {

    }

    onClickClose(sensorZoneHistoryID) {
        this.ignoreSensorZoneHistories[sensorZoneHistoryID] = true;
        this.setState({ loading: false });
    }

    render() {
        if (!this.props.sensorAlarms || this.props.sensorAlarms === null || this.props.sensorAlarms.length === 0) {
            this.ignoreSensorZoneHistories = {};
            return <></>;
        }

        var grid = [];

        const alarms = this.props.sensorAlarms;
        for (let i = 0; i < alarms.length; i++) {
            const alarm = alarms[i];

            if (alarm.sopStatus === 2 || alarm.isAlarm === false) {
                continue;
            }

            if (this.ignoreSensorZoneHistories[alarm.sensorZoneHistoryID]) {
                continue;
            }

            let subClassName = styles.alertAlert;
            const alarmInfo = "[" + alarm.dtTime.toString().replace('T', ' ') + "] " + alarm.alarmType + " " + alarm.alarmInfo;

            if (alarm.alarmDepth === 1) {
                // 관심
                subClassName = styles.alertSuccess;
            }
            else if (alarm.alarmDepth === 3) {
                // 경계
                subClassName = styles.alertWarning;
            }
            else if (alarm.alarmDepth === 4) {
                // 심각
                subClassName = styles.alertDanger;
            }

            grid.push(
                <div key={"alarm_text_" + i} className={styles.alert + " " + subClassName}>
                    <div className={styles.alertBody}>
                        <button type="button" className={styles.btnAlertClose} onClick={() => this.onClickClose(alarm.sensorZoneHistoryID)}>
                        </button>
                        <i className={styles.alertImg}></i>
                        <p>{alarmInfo}</p>
                    </div>
                </div>
            );
        }

        return (
            <aside className={styles.alertWrap}>
                {
                    grid
                }
            </aside>
        );
    }
} export default DetectionTextInfo;