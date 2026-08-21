import React, { Component } from 'react';
import dashStyles from '../../../Common/css/dash.module.css';
import swiper from '../../../Common/css/swiper.module.css';
import $ from 'jquery';
import InfoContentSensor from '../../../Dashboard/ui/infoContentSensor';
import InfoContentScene from '../../../Dashboard/ui/infoContentScene';
import DashboardHeader from '../../../Dashboard/ui/dashboardHeader';
import InfoHeader from '../../../Dashboard/ui/infoHeader';
import InfoContent from '../../../Dashboard/ui/infoContent'
import store from '../../../Root/store';
import DashboardResource from '../../../Dashboard/resource/id';


class DashboardDetail extends Component {
    constructor(props) {
        super(props);

        this.state = {
            buildingGroupList: [],
            buildingGroup: -1,
            building: -1,
            zone: -1,
            sensorAlarms: [],
        }

        this.props = props;

        store.subscribe(function () {
            let data = store.getState();

            if ((data.sensorAlarm === null || data.sensorAlarm === undefined)
                && data.actionType !== 'SensorAlarm')
                return;

            this.changeAlarm(data.sensorAlarm);
        }.bind(this));
    }

    componentDidMount() {
        // 센서 알람 초기화
        let sensorAlarms = store.getState().sensorAlarm;

        this.setState({ sensorAlarms: sensorAlarms });
    }

    changeAlarm(sensorAlarms) {
        this.setState({ sensorAlarms: sensorAlarms });
    }

    selectSpatial = (buildingGroup, building, zone) => {
        console.log(buildingGroup + "," + building + "," + zone);

        this.setState({ buildingGroup: buildingGroup, building: building, zone: zone });
    }

    setSelectSensors() {
        if (this.props.dashboardSensors === null || this.props.dashboardSensors === undefined) {
            return null;
        }

        const buildingGroupID = this.state.buildingGroup;
        const buildingID = this.state.building;
        const zoneID = this.state.zone;
        const useSensorList = this.props.dashboardSensors;

        // 전체 센서 갯수
        if (buildingGroupID === -1) {
            let sensorList = {};
            sensorList.fireSensors = useSensorList.fireSensors;
            sensorList.disabledFireSensors = useSensorList.disabledFireSensors;
            sensorList.psmSensors = useSensorList.psmSensors;
            sensorList.disabledPSMSensors = useSensorList.disabledPSMSensors;
            sensorList.etcSensors = useSensorList.etcSensors;
            sensorList.disabledEtcSensors = useSensorList.disabledEtcSensors;
            sensorList.cctvs = useSensorList.cctvs;
            sensorList.disabledCCTVs = useSensorList.disabledCCTVs;

            return sensorList;
        }

        if (this.props.buildingGroupList === null || this.props.buildingGroupList === undefined) {
            return null;
        }

        // 선택된 센서 리스트
        let fireSensors = [];
        let disabledFireSensors = [];
        let psmSensors = [];
        let disabledPSMSensors = [];
        let etcSensors = [];
        let disabledEtcSensors = [];
        let cctvs = [];
        let disabledCCTVs = [];

        // 외곽인지 내부인지 판단
        if (buildingGroupID === DashboardResource.zoneID.outdoor) {
            // fire 갯수 파악
            for (let k = 0; k < useSensorList.fireSensors.length; k++) {
                let fireSensor = useSensorList.fireSensors[k];

                if (fireSensor.zoneID === DashboardResource.zoneID.outdoor) {
                    fireSensors.push(fireSensor);
                }
            }

            for (let k = 0; k < useSensorList.disabledFireSensors.length; k++) {
                let disabledFireSensor = useSensorList.disabledFireSensors[k];

                if (disabledFireSensor.zoneID === DashboardResource.zoneID.outdoor) {
                    disabledFireSensors.push(disabledFireSensor);
                }
            }

            // etc 갯수 파악
            for (let k = 0; k < useSensorList.etcSensors.length; k++) {
                let etcSensor = useSensorList.etcSensors[k];

                if (etcSensor.zoneID === DashboardResource.zoneID.outdoor) {
                    etcSensors.push(etcSensor);
                }
            }

            for (let k = 0; k < useSensorList.disabledEtcSensors.length; k++) {
                let disabledEtcSensor = useSensorList.disabledEtcSensors[k];

                if (disabledEtcSensor.zoneID === DashboardResource.zoneID.outdoor) {
                    disabledEtcSensors.push(disabledEtcSensor);
                }
            }

            // cctv 갯수 파악
            for (let k = 0; k < useSensorList.cctvs.length; k++) {
                let cctv = useSensorList.cctvs[k];

                if (cctv.zoneID === DashboardResource.zoneID.outdoor) {
                    cctvs.push(cctv);
                }
            }

            for (let k = 0; k < useSensorList.disabledCCTVs.length; k++) {
                let disabledCCTV = useSensorList.disabledCCTVs[k];

                if (disabledCCTV.zoneID === DashboardResource.zoneID.outdoor) {
                    disabledCCTVs.push(disabledCCTV);
                }
            }

            // psm 갯수 파악
            for (let k = 0; k < useSensorList.psmSensors.length; k++) {
                let psmSensor = useSensorList.psmSensors[k];

                if (psmSensor.equipZoneID === DashboardResource.zoneID.outdoor) {
                    psmSensors.push(psmSensor);
                }
            }

            for (let k = 0; k < useSensorList.disabledPSMSensors.length; k++) {
                let disabledPSMSensor = useSensorList.disabledPSMSensors[k];

                if (disabledPSMSensor.equipZoneID === DashboardResource.zoneID.outdoor) {
                    disabledPSMSensors.push(disabledPSMSensor);
                }
            }

        } else {
            let buildingGroupList = this.props.buildingGroupList;
            if (buildingGroupList === null || buildingGroupList === undefined)
                buildingGroupList = [];

            for (let i = 0; i < buildingGroupList.length; i++) {
                let buildingGroup = buildingGroupList[i];

                if (buildingGroupID !== -1 && buildingGroupID !== buildingGroup.id)
                    continue;

                for (let j = 0; j < buildingGroup.buildingDatas.length; j++) {
                    let building = buildingGroup.buildingDatas[j];

                    if (buildingID !== -1 && buildingID !== building.id)
                        continue;

                    for (let y = 0; y < building.zoneDatas.length; y++) {
                        let zone = building.zoneDatas[y];

                        if (zoneID !== -1 && zoneID !== zone.id)
                            continue;

                        // fire 갯수 파악
                        for (let k = 0; k < useSensorList.fireSensors.length; k++) {
                            let fireSensor = useSensorList.fireSensors[k];

                            if (fireSensor.zoneID === zone.id) {
                                //fireSensorCount++;
                                fireSensors.push(fireSensor);
                            }
                        }

                        for (let k = 0; k < useSensorList.disabledFireSensors.length; k++) {
                            let disabledFireSensor = useSensorList.disabledFireSensors[k];

                            if (disabledFireSensor.zoneID === zone.id) {
                                //disabledFireSensorCount++;
                                disabledFireSensors.push(disabledFireSensor);
                            }
                        }

                        // etc 갯수 파악
                        for (let k = 0; k < useSensorList.etcSensors.length; k++) {
                            let etcSensor = useSensorList.etcSensors[k];

                            if (etcSensor.zoneID === zone.id) {
                                //etcSensorCount++;
                                etcSensors.push(etcSensor);
                            }
                        }

                        for (let k = 0; k < useSensorList.disabledEtcSensors.length; k++) {
                            let disabledEtcSensor = useSensorList.disabledEtcSensors[k];

                            if (disabledEtcSensor.zoneID === zone.id) {
                                //disabledEtcSensorCount++;
                                disabledEtcSensors.push(disabledEtcSensor);
                            }
                        }

                        // cctv 갯수 파악
                        for (let k = 0; k < useSensorList.cctvs.length; k++) {
                            let cctv = useSensorList.cctvs[k];

                            if (cctv.zoneID === zone.id) {
                                //cctvCount++;
                                cctvs.push(cctv);
                            }
                        }

                        for (let k = 0; k < useSensorList.disabledCCTVs.length; k++) {
                            let disabledCCTV = useSensorList.disabledCCTVs[k];

                            if (disabledCCTV.zoneID === zone.id) {
                                //disabledCCTVCount++;
                                disabledCCTVs.push(disabledCCTV);
                            }
                        }


                        for (let z = 0; z < zone.equipmentZoneDatas.length; z++) {
                            let equipmentZone = zone.equipmentZoneDatas[z];

                            // psm 갯수 파악
                            for (let k = 0; k < useSensorList.psmSensors.length; k++) {
                                let psmSensor = useSensorList.psmSensors[k];

                                if (psmSensor.equipZoneID === equipmentZone.id) {
                                    //psmSensorCount++;
                                    psmSensors.push(psmSensor);
                                }
                            }

                            for (let k = 0; k < useSensorList.disabledPSMSensors.length; k++) {
                                let disabledPSMSensor = useSensorList.disabledPSMSensors[k];

                                if (disabledPSMSensor.equipZoneID === equipmentZone.id) {
                                    //disabledPSMSensorCount++;
                                    disabledPSMSensors.push(disabledPSMSensor);
                                }
                            }
                        }
                    }
                }
            }
        }

        let sensorList = {};
        sensorList.fireSensors = fireSensors;
        sensorList.disabledFireSensors = disabledFireSensors;
        sensorList.psmSensors = psmSensors;
        sensorList.disabledPSMSensors = disabledPSMSensors;
        sensorList.etcSensors = etcSensors;
        sensorList.disabledEtcSensors = disabledEtcSensors;
        sensorList.cctvs = cctvs;
        sensorList.disabledCCTVs = disabledCCTVs;

        return sensorList;
    }

    selectAlarms = () => {
        let selectAlarms = [];

        if (this.state.sensorAlarms === null || this.state.sensorAlarms === undefined || this.state.sensorAlarms.length === 0) {
            return selectAlarms;
        }

        const buildingGroupID = this.state.buildingGroup;
        const buildingID = this.state.building;
        const zoneID = this.state.zone;
        const sensorAlarms = this.state.sensorAlarms;

        //let equipmentZoneList = []; // equipmentZoneID 중복 방지
        console.log("sensorAlarms 갯수: " + sensorAlarms.length.toString());

        let buildingGroupList = this.props.buildingGroupList;
        if (buildingGroupList === null || buildingGroupList === undefined)
            buildingGroupList = [];

        for (let k = 0; k < sensorAlarms.length; k++) {
            let alarm = sensorAlarms[k];
            let chk = false;

            for (let i = 0; i < buildingGroupList.length; i++) {
                let buildingGroup = buildingGroupList[i];

                if (buildingGroupID !== -1 && buildingGroupID !== buildingGroup.id)
                    continue;

                for (let j = 0; j < buildingGroup.buildingDatas.length; j++) {
                    let building = buildingGroup.buildingDatas[j];

                    if (buildingID !== -1 && buildingID !== building.id)
                        continue;

                    for (let y = 0; y < building.zoneDatas.length; y++) {
                        let zone = building.zoneDatas[y];

                        if (zoneID !== -1 && zoneID !== zone.id)
                            continue;

                        for (let z = 0; z < zone.equipmentZoneDatas.length; z++) {
                            let equipmentZone = zone.equipmentZoneDatas[z];

                            //for (let k = 0; k < sensorAlarms.length; k++) {
                            //    let alarm = sensorAlarms[k];

                            // 같은 이킵존id 해당하는 센서 중복 추가 방지 
                            //if (equipmentZoneList[equipmentZone.id] === true)
                            //    continue;

                            if (equipmentZone.id === alarm.equipZoneID) {
                                alarm.equipZoneName = equipmentZone.displayText;
                                selectAlarms.push(alarm);
                                chk = true;
                                break;
                            }
                            //}

                            //equipmentZoneList[equipmentZone.id] = true;
                        }

                        if (chk === true)
                            break;
                    }

                    if (chk === true)
                        break;
                }

                if (chk === true)
                    break;
            }
        }

        if (sensorAlarms.length > 0) {
            console.log("sensorAlarms 갯수: " + selectAlarms.length.toString());
        }

        return selectAlarms;
    }

    render() {
        let selectSensors = this.setSelectSensors();
        let selectAlarms = this.selectAlarms();

        let maxSildes = 3;

        return (
            <aside className={dashStyles.bythemDashboard + " " + dashStyles.subDashboard} style={{ zIndex: "2" }}>
                <div className={dashStyles.dashboardContainer + " dashboardHasTitle"}>

                    <DashboardHeader btbClose={true} onClickBtnClose={() => this.props.onClickBtnClose()} buildingGroupList={this.props.buildingGroupList} selectSpatial={this.selectSpatial} />


                    <div className={dashStyles.dashboardBody}>

                        <InfoHeader selectSensors={selectSensors} />
                        

                        <div className={dashStyles.infoContainer}>

                            <figure className={dashStyles.infoContent}>
                                <InfoContentSensor maxSildes={maxSildes} autoplay={true} selectSensors={selectSensors} selectAlarms={selectAlarms} />
                                <InfoContentScene maxSildes={maxSildes} selectSensors={selectSensors} selectAlarms={selectAlarms} />
                            </figure>

                            <InfoContent selectAlarms={selectAlarms} />

                        </div>
                    </div>
                </div>
            </aside>
        );
    }
}


export default DashboardDetail;