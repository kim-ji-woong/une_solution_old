import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';

import Title from '../../Root/title';
import { FacilityTypeController } from '../services/facilityTypeController';
import Menu from '../../Root/menu';

import store from '../../Root/store';
import SessionString from '../../Common/js/sessionString';
import FacilityTypeResource from '../resource/id';
import SensorSearch from './sensorSearch';

import styles from '../../Common/css/style.css';

class Main extends Component {
    constructor(props) {
        super(props);

        this.state = {
            facilityType: null,
            sensorID: null,
            typeName: "",
            sensorState: null,
            sensorAddr: null,
        }

        this.props = props;
        this.initType();
        this.initSensor();

        store.subscribe(function () {
            this.showSensorInfo(store.getState().sensorInfo);
        }.bind(this));
    }

    showSensorInfo(sensor) {
        if (sensor === null || sensor === undefined)
            return;

        this.setState({ sensorState: sensor.state });
    }

    // 타입에 따른 화면 변환 Init 필요.
    initType = () => {
        if (window.sessionStorage.getItem(SessionString.Key.facilityType) === null) {
            // 다시 로그인 페이지
            alert(FacilityTypeResource.ID.textEnterError);
            this.props.history.push('/');
        } else {
            let facilityType = parseInt(window.sessionStorage.getItem(SessionString.Key.facilityType));

            this.state.facilityType = facilityType;

            if (facilityType === FacilityTypeResource.ID.facilityType.fire) {
                this.state.typeName = FacilityTypeResource.ID.facilityTypeName.fire;
            } else if (facilityType === FacilityTypeResource.ID.facilityType.flood) {
                this.state.typeName = FacilityTypeResource.ID.facilityTypeName.flood;
            } else if (facilityType === FacilityTypeResource.ID.facilityType.heat) {
                this.state.typeName = FacilityTypeResource.ID.facilityTypeName.heat;
            } else if (facilityType === FacilityTypeResource.ID.facilityType.collapse) {
                this.state.typeName = FacilityTypeResource.ID.facilityTypeName.collapse;
            }
        }
    }

    initSensor = () => {
        let facilityType = parseInt(window.sessionStorage.getItem(SessionString.Key.facilityType));

        // 세션 스토리지 확인 selectSensor
        let sensor;

        if (window.sessionStorage.getItem(SessionString.Key.selectSensor) !== null && window.sessionStorage.getItem(SessionString.Key.selectSensor) !== undefined) {
            let sensor = JSON.parse(window.sessionStorage.getItem(SessionString.Key.selectSensor));

            this.state.sensorID = sensor.id;
            this.state.sensorAddr = sensor.addr;
            this.state.sensorState = sensor.state;

            // 선택된 센서가 있다면 해당 센서 정보 불러오기
            FacilityTypeController.StartWatchTimer(sensor.id, facilityType);
        } else {
            // 선택된 센서가 없으면 첫번째 센서 불러오기
            this.getFirstSensor(facilityType);
        }
    }

    async getFirstSensor(type) {
        const result = await FacilityTypeController.firstSensor(type);

        if (result.success === true && result.sensor !== null) {
            //this.setState({ sensorID: result.sensor.id });
            this.state.sensorID = result.sensor.id;
            this.state.sensorAddr = result.sensor.addr;

            // 세션 스토리지에 선택된 센서 저장
            window.sessionStorage.setItem(SessionString.Key.selectSensor, JSON.stringify(result.sensor));

            // 주기적으로 해당 센서 상태 불러오기 
            FacilityTypeController.StartWatchTimer(result.sensor.id, type);
        }
    }

    showStateImg = () => {
        let retImg = <img src="/resource/img/normal_times.png" ></img>;

        if (this.state.sensorState === FacilityTypeResource.ID.riskLevel.Attention) {
            retImg = <img src="/resource/img/attention.png" ></img>;
        } else if (this.state.sensorState === FacilityTypeResource.ID.riskLevel.Caution) {
            retImg = <img src="/resource/img/precautions.png" ></img >;
        } else if (this.state.sensorState === FacilityTypeResource.ID.riskLevel.Alert) {
            retImg = <img src="/resource/img/boundary.png" ></img >;
        } else if (this.state.sensorState === FacilityTypeResource.ID.riskLevel.Serious) {
            retImg = <img src="/resource/img/serious.png" ></img >;
        }

        return retImg;
    }

    onclickReload = () => {
        let sensorID = this.state.sensorID;
        let facilityType = parseInt(window.sessionStorage.getItem(SessionString.Key.facilityType));

        this.getSensorInfo(sensorID, facilityType);
    }

    async getSensorInfo(id, type) {
        const result = await FacilityTypeController.DisplaySensorInfo(id, type);

        if (result.success === true && result.sensor !== null) {
            this.setState({ sensorState: result.sensor.state });
        }
    }

    onclickSearch = () => {
        this.props.history.push(SensorSearch.pathSensorSearch);
    }

    onclickAlarmList = () => {
        this.props.history.push(Menu.pathAlarmList);
    }

    onclickManualList = () => {
        this.props.history.push(Menu.pathManualList);
    }

    render() {
        let stateImg = this.showStateImg();

        return (
            <div className="container_sub2">

                <Title />

                <div className="contents">
                    <h3>{this.state.typeName} 위기경보</h3>
                    <div className="content_box">
                        <div className="state_box">
                            <div id="location">
                                <embed src="/resource/icon/placeholder.png" style={{ width: "15px", height: "15px" }}></embed>
                                <p>{this.state.sensorAddr}</p>
                            </div>
                            <div id="new" onClick={this.onclickReload} >
                                <img src="/resource/icon/Refresh.png"></img>
                                <img src="/resource/icon/Refresh_b.png" />  
                            </div>
                            {stateImg}
                        </div>
                        <div id="select_box" onClick={this.onclickSearch}>센서 검색</div>
                        <div className="history" onClick={this.onclickAlarmList}>알람 이력</div>
                        <div className="manual" onClick={this.onclickManualList}>행동 메뉴얼</div>
                    </div>
                </div>
            </div >
        );
    }
}

export default Main;
