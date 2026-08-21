import { ui } from 'jquery';
import React, { Component } from 'react';
import content from '../../../Common/css/content.module.css';
import SdmsResource from '../../resource/id';
import { SDMSController } from '../../services/sdmsController';

class EventDashboard extends Component {
    constructor(props) {
        super(props);

        this.state = {
            sensorList: null,
            sensorData: "",
            prevData: "",
        }
    }

    async requestFacilityTypeUnit(facilityTypeID) {
        const [result, message] = await SDMSController.getFacilityType(facilityTypeID);
        let retUnit = "";

        if (result === null) {
            console.log(message);
        }
        else {
            if (message.id !== 0) {
                if (message.uom !== null && message.uom !== undefined) {
                    retUnit = message.uom;
                }
            } 
        }

        return retUnit;
    }

    async requestSensorList(selectedAlarm) {
        let sensorData = "";

        const [result, message] = await SDMSController.requestSensorList();

        if (result === null) {
            console.log(message);
        }
        else {
            
            if (selectedAlarm.facilityType !== 1) {

                let sensorList = "";

                if (selectedAlarm.facilityType == 215 ||
                    selectedAlarm.facilityType == 216 ||
                    selectedAlarm.facilityType == 222) {
                    sensorList = result.psmSensors;
                } else {
                    sensorList = result.etcSensors;
                }

                for (let i = 0; i < sensorList.length; i++) {
                    let sensor = sensorList[i];

                    if (selectedAlarm.orgSensorID == sensor.id) {
                        sensorData = sensor.currentData;
                        break;
                    }
                }
            }
        }

        return sensorData;
    }

    async getSensorData(selectedAlarm) {
        if (selectedAlarm === null || selectedAlarm === undefined)
            return;

        let unit = "";
        let Data = "";

        // 센서 단위 받아오기
        unit = await this.requestFacilityTypeUnit(selectedAlarm.facilityType);
        // 센서 현재값 갱신
        Data = await this.requestSensorList(this.props.selectedAlarm);

        let sensorData = Data + unit;

        if (this.state.prevData !== sensorData) {
            this.setState({ sensorData: sensorData, prevData: sensorData });
        }
    }

    getSensorName() {
        let sensorNames = '';

        const containsSensorLength = this.props.selectedAlarm.alarmSensorZoneIDs.length;
        let matchSensorLength = 0;

        if (SdmsResource.isPSMSensorType(this.props.selectedAlarm.facilityType)) {
            const sensorLength = this.props.sensorList.psmSensors.length;
            for (let i = 0; i < sensorLength; i++) {
                const sensor = this.props.sensorList.psmSensors[i];
                if (this.props.selectedAlarm.alarmSensorZoneIDs.includes(sensor.sensorZoneID)) {
                    if (sensorNames.length > 0) {
                        sensorNames += ', ' + sensor.name;
                    }
                    else {
                        sensorNames = sensor.name;
                    }

                    matchSensorLength++;
                }

                if (containsSensorLength === matchSensorLength) {
                    sensorNames = '(' + sensorNames + ')';
                    break;
                }
            }
        }
        else if (SdmsResource.isETCSensorType(this.props.selectedAlarm.facilityType)) {
            const sensorLength = this.props.sensorList.etcSensors.length;
            for (let i = 0; i < sensorLength; i++) {
                const sensor = this.props.sensorList.etcSensors[i];
                if (this.props.selectedAlarm.alarmSensorZoneIDs.includes(sensor.sensorZoneID)) {
                    if (sensorNames.length > 0) {
                        sensorNames += ', ' + sensor.name;
                    }
                    else {
                        sensorNames = sensor.name;
                    }

                    matchSensorLength++;
                }

                if (containsSensorLength === matchSensorLength) {
                    sensorNames = '(' + sensorNames + ')';
                    break;
                }
            }
        }
        else {
            const sensorLength = this.props.sensorList.fireSensors.length;
            for (let i = 0; i < sensorLength; i++) {
                const sensor = this.props.sensorList.fireSensors[i];
                if (this.props.selectedAlarm.alarmSensorZoneIDs.includes(sensor.sensorZoneID)) {
                    if (sensorNames.length > 0) {
                        sensorNames += ', ' + sensor.name;
                    }
                    else {
                        sensorNames = sensor.name;
                    }

                    matchSensorLength++;
                }

                if (containsSensorLength === matchSensorLength) {
                    sensorNames = '(' + sensorNames + ')';
                    break;
                }
            }
        }

        return sensorNames;
    }

    render() {
        const dt = new Date(this.props.selectedAlarm.dtTime);
        var mm = dt.getMonth() + 1;
        var dd = dt.getDate();
        var ss = dt.getSeconds();
        const ymd = dt.getFullYear() + '.' + ((mm > 9) ? '' : '0') + mm + '.' + ((dd > 9) ? '' : '0') + dd;
        const hms = dt.getHours() + ':' + dt.getMinutes() + ':' + ((ss > 9) ? '' : '0') + ss;

        // 현재 센서 수치값 및 센서 단위 받아오기
        //this.getSensorData(this.props.selectedAlarm);

        var fontColor = 'yellow'
        if (this.props.selectedAlarm.alarmDepth === 3) {
            fontColor = 'orange';
        }
        else if (this.props.selectedAlarm.alarmDepth === 4) {
            fontColor = 'red';
        }

        const sensorNames = this.getSensorName();
        let message = this.props.selectedAlarm.message;
        if (sensorNames.length > 0) {
            // 센서명 끼워넣기
            const index = this.props.selectedAlarm.message.indexOf(']에서');
            if (index >= 0) {
                message = message.slice(0, index + 1) + sensorNames + message.slice(index + 1, message.length)
            }
        }

        return (
            <div id={this.props.popupType} className={content.viewDashboardBoxD + ' ' + content.viewDashboardCurrent}>
                <div className={content.viewTitleTxt} style={{ color: fontColor }}>
                    {ymd}&nbsp;{hms} {/*[{this.props.selectedAlarm.facilityTypeString}]*/} &nbsp;
                    {/*{this.props.selectedAlarm.positionName} {this.state.sensorData}*/}
                    {message}
                </div>
            </div>
        );
    }
}

export default EventDashboard;