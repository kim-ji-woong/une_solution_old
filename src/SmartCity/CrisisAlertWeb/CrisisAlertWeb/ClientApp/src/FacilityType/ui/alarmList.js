import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';

import Title from '../../Root/title';
import { FacilityTypeController } from '../services/facilityTypeController';
import Paginate from '../../Root/paginate';

import SessionString from '../../Common/js/sessionString';
import FacilityTypeResource from '../resource/id';

import styles from '../../Common/css/style.css';

class AlarmList extends Component {
    constructor(props) {
        super(props);

        this.state = {
            alarmList: null,
            allRequest: null,                   // 전체 요청 갯수  
            page: 1,                            // 현재 페이지
            ongPage: 10,                        // 한 페이지에 보여줄 요청의 수.
        }

        this.props = props;
        this.initLoad();
    }

    initLoad = () => {
        let facilityType = parseInt(window.sessionStorage.getItem(SessionString.Key.facilityType));
        let sensor = JSON.parse(window.sessionStorage.getItem(SessionString.Key.selectSensor));

        let sensorID = sensor.id;

        this.getAlarmList(facilityType);
    }

    async getAlarmList(type) {
        const result = await FacilityTypeController.getAlarmList(type);

        if (result.success === true && result.alarms !== null) {
            let alarms = result.alarms;
            let allRequest = 0;

            if (alarms !== null && alarms.length > 0)
                allRequest = alarms.length;

            alarms = JSON.stringify(alarms);

            this.setState({ alarmList: alarms, allRequest: allRequest});
        }
    }

    pageChange = (pageNum) => {
        this.setState({ page: pageNum });
        return;
    }

    showAlarmList() {
        let showList = [];
        let alarmList = this.state.alarmList;

        if (alarmList === null || alarmList === undefined)
            return showList;

        let list = JSON.parse(alarmList);

        let min = (this.state.page - 1) * this.state.ongPage;
        let max = min + this.state.ongPage;
        if (max > this.state.allRequest) {
            max = this.state.allRequest;
        }

        for (let i = min; i < max; i++) {
            let alarm = list[i];

            let date = alarm.createTime;
            let dateTime = new Date(date);

            let isCheck = alarm.isCheck;

            if (isCheck === 1) {
                isCheck = "YES";
            } else if (isCheck === 0) {
                isCheck = "NO";
            }

            showList.push(
                <tr>
                    <td>{i + 1}</td>
                    <td>{alarm.riskLevel}</td>
                    <td>{dateTime.toLocaleString()}</td>
                    <td>{alarm.address}</td>
                    <td>{isCheck}</td>
                </tr>
            );
        }

        return showList;
    }

    render() {
        let showList = [];

        showList = this.showAlarmList();


        return (
            <div className="container_sub2">

                <Title />



                <div className="contents">
                    <h3>알람 이력</h3>
                    <div className="content_box">
                        <table id="behav_tb">
                            <colgroup>
                                <col width="1%" />
                                <col width="15%" />
                                <col width="20%" />
                                <col width="20%" />
                                <col width="10%" />
                            </colgroup>
                            <thead>
                                <tr>
                                    <th>NO</th>
                                    <th>위기경보 수준</th>
                                    <th>일시</th>
                                    <th>감지 위치</th>
                                    <th>확인 여부</th>
                                </tr>
                            </thead>
                            <tbody>
                                {showList}
                            </tbody>
                        </table>
                    </div>
                </div>

                <Paginate page={this.state.page} allRequest={this.state.allRequest} onChange={this.pageChange} />
                

            </div >
        );
    }
}

export default AlarmList;
