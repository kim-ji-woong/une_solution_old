import React, { Component } from 'react';
import $ from 'jquery';

import dashboard from '../css/dashboardNew.module.css';

import DashboardResource from '../resource/id';
import SDMSResource from '../../SDMS/resource/id';
import ProjectResource from '../../Root/resource/id';

class AlarmComponet extends Component {
    constructor(props) {
        super(props);

        this.state = {
            open: true,
            type: DashboardResource.displayInfoType.FIRE,
        }

        this.props = props;
    }

    componentWillUpdate(nextProps, nextState) {
        if (this.props.type !== nextProps.type && nextProps.type !== DashboardResource.displayInfoType.SAFETY_EYE)
            this.state.type = nextProps.type;
    }

    init = () => {
        const buildingGroupID = this.props.buildingGroupID;
        const buildingID = this.props.buildingID;
        const buildingGroupList = this.props.buildingGroupList;
        const selectSensors = this.props.selectSensors;
        let todayAlarms = [];

        if (this.props.todayAlarms !== null && this.props.todayAlarms !== undefined)
            todayAlarms = this.props.todayAlarms;

        let sensorList = {};
        sensorList.fireSensors = [];
        sensorList.disabledFireSensors = [];
        sensorList.psmSensors = [];
        sensorList.disabledPSMSensors = [];
        sensorList.etcSensors = [];
        sensorList.disabledEtcSensors = [];
        sensorList.cctvs = [];
        sensorList.disabledCCTVs = [];

        let alarms = {};
        alarms.fireAlarm = 0;
        alarms.cctvAlarm = 0;
        alarms.iotAlarm = 0;
        alarms.psmAlarm = 0;
        alarms.etcAlarm = 0;

        let currentAlarm = {};
        let currentFireAlarm = false;
        let currentPSMAlarm = false;
        let currentETCAlarm = false;
        let currentCCTVAlarm = false;

        currentAlarm.fireAlarm = currentFireAlarm;
        currentAlarm.psmAlarm = currentPSMAlarm;
        currentAlarm.etcAlarm = currentETCAlarm;
        currentAlarm.cctvAlarm = currentCCTVAlarm;

        let buildingGroupName = "";
        let displayText = "";

        if (((buildingGroupID === null || buildingGroupID === undefined) && (buildingID === null || buildingID === undefined))||
            buildingGroupList === null || buildingGroupList === undefined ||
            selectSensors === null || selectSensors === undefined)
            return [buildingGroupName, displayText, sensorList, alarms, currentAlarm];


        // 선택된 센서 리스트
        let fireSensors = [];
        let disabledFireSensors = [];
        let psmSensors = [];
        let disabledPSMSensors = [];
        let etcSensors = [];
        let disabledEtcSensors = [];
        let cctvs = [];
        let disabledCCTVs = [];

        let fireAlarm = 0;
        let iotAlarm = 0;
        let psmAlarm = 0;
        let etcAlarm = 0;
        let cctvAlarm = 0;

        for (let i = 0; i < this.props.buildingGroupList.length; i++) {
            let buildingGroup = this.props.buildingGroupList[i];

            if ((buildingID === null && buildingGroupID !== buildingGroup.id) ||
                (buildingGroupID === null && (buildingID === null || buildingID === undefined)))
                continue;

            if (buildingGroupID === buildingGroup.id) {
                buildingGroupName = buildingGroup.groupName;
                displayText = buildingGroup.displayText;

                for (let k = 0; k < todayAlarms.length; k++) {
                    let alarm = todayAlarms[k];

                    if (buildingGroup.id === alarm.buildingGroupID) {

                        if (alarm.facilityType === SDMSResource.facilityType.FIRE) {
                            fireAlarm++;

                            if (alarm.isAlarm === true)
                                currentFireAlarm = true;
                        } else if (SDMSResource.isSVMSSensorType(alarm.facilityType)) {
                            cctvAlarm++;

                            if (alarm.isAlarm === true)
                                currentCCTVAlarm = true;
                        } else if (SDMSResource.isPSMSensorType(alarm.facilityType)) {
                            psmAlarm++;

                            if (alarm.isAlarm === true)
                                currentPSMAlarm = true;
                        }
                        else if (SDMSResource.isETCSensorType(alarm.facilityType)) {
                            etcAlarm++;

                            if (alarm.isAlarm === true)
                                currentETCAlarm = true;
                        }
                    }
                }
            }

            for (let j = 0; j < buildingGroup.buildingDatas.length; j++) {
                let building = buildingGroup.buildingDatas[j];

                if (buildingGroupID !== buildingGroup.id && buildingID !== building.id)
                    continue;


                if (buildingGroupID === null && buildingID === building.id) {
                    buildingGroupName = building.buildingName;
                    displayText = building.displayText;

                    for (let k = 0; k < todayAlarms.length; k++) {
                        let alarm = todayAlarms[k];

                        if (building.id === alarm.buildingID) {

                            if (alarm.facilityType === SDMSResource.facilityType.FIRE) {
                                fireAlarm++;

                                if (alarm.isAlarm === true)
                                    currentFireAlarm = true;
                            } else if (SDMSResource.isSVMSSensorType(alarm.facilityType)) {
                                cctvAlarm++;

                                if (alarm.isAlarm === true)
                                    currentCCTVAlarm = true;
                            } else if (SDMSResource.isPSMSensorType(alarm.facilityType)) {
                                psmAlarm++;

                                if (alarm.isAlarm === true)
                                    currentPSMAlarm = true;
                            }
                            else if (SDMSResource.isETCSensorType(alarm.facilityType)) {
                                etcAlarm++;

                                if (alarm.isAlarm === true)
                                    currentETCAlarm = true;
                            }
                        }
                    }
                }

                for (let y = 0; y < building.zoneDatas.length; y++) {
                    let zone = building.zoneDatas[y];

                    // fire 갯수 파악
                    for (let k = 0; k < selectSensors.fireSensors.length; k++) {
                        let fireSensor = selectSensors.fireSensors[k];

                        if (fireSensor.zoneID === zone.id) {
                            //fireSensorCount++;
                            fireSensors.push(fireSensor);
                        }
                    }

                    for (let k = 0; k < selectSensors.disabledFireSensors.length; k++) {
                        let disabledFireSensor = selectSensors.disabledFireSensors[k];

                        if (disabledFireSensor.zoneID === zone.id) {
                            //disabledFireSensorCount++;
                            disabledFireSensors.push(disabledFireSensor);
                        }
                    }

                    // psm 갯수 파악
                    for (let k = 0; k < selectSensors.psmSensors.length; k++) {
                        let psmSensor = selectSensors.psmSensors[k];

                        if (psmSensor.zoneID === zone.id) {
                            //psmSensorCount++;
                            psmSensors.push(psmSensor);
                        }
                    }

                    for (let k = 0; k < selectSensors.disabledPSMSensors.length; k++) {
                        let disabledPSMSensor = selectSensors.disabledPSMSensors[k];

                        if (disabledPSMSensor.zoneID === zone.id) {
                            //disabledPSMSensorCount++;
                            disabledPSMSensors.push(disabledPSMSensor);
                        }
                    }

                    // etc 갯수 파악
                    for (let k = 0; k < selectSensors.etcSensors.length; k++) {
                        let etcSensor = selectSensors.etcSensors[k];

                        if (etcSensor.zoneID === zone.id) {
                            //etcSensorCount++;
                            etcSensors.push(etcSensor);
                        }
                    }

                    for (let k = 0; k < selectSensors.disabledEtcSensors.length; k++) {
                        let disabledEtcSensor = selectSensors.disabledEtcSensors[k];

                        if (disabledEtcSensor.zoneID === zone.id) {
                            //disabledEtcSensorCount++;
                            disabledEtcSensors.push(disabledEtcSensor);
                        }
                    }

                    // cctv 갯수 파악
                    for (let k = 0; k < selectSensors.cctvs.length; k++) {
                        let cctv = selectSensors.cctvs[k];

                        if (cctv.zoneID === zone.id) {
                            //cctvCount++;
                            cctvs.push(cctv);
                        }
                    }

                    for (let k = 0; k < selectSensors.disabledCCTVs.length; k++) {
                        let disabledCCTV = selectSensors.disabledCCTVs[k];

                        if (disabledCCTV.zoneID === zone.id) {
                            //disabledCCTVCount++;
                            disabledCCTVs.push(disabledCCTV);
                        }
                    }
                }

            }
        }

        sensorList.fireSensors = fireSensors;
        sensorList.disabledFireSensors = disabledFireSensors;
        sensorList.psmSensors = psmSensors;
        sensorList.disabledPSMSensors = disabledPSMSensors;
        sensorList.etcSensors = etcSensors;
        sensorList.disabledEtcSensors = disabledEtcSensors;
        sensorList.cctvs = cctvs;
        sensorList.disabledCCTVs = disabledCCTVs;

        
        alarms.fireAlarm = fireAlarm;
        alarms.cctvAlarm = cctvAlarm;
        alarms.psmAlarm = psmAlarm;
        alarms.etcAlarm = etcAlarm;

        currentAlarm.fireAlarm = currentFireAlarm;
        currentAlarm.psmAlarm = currentPSMAlarm;
        currentAlarm.etcAlarm = currentETCAlarm;
        currentAlarm.cctvAlarm = currentCCTVAlarm;

        return [buildingGroupName, displayText, sensorList, alarms, currentAlarm];
    }

    getOpenCloseUI = (currentAlarm) => {
        const open = this.state.open;
        let upDownClass = dashboard.downIcon;
        let openCloseClass = dashboard.sensorInfo;

        if (currentAlarm !== null && currentAlarm !== undefined &&
            (currentAlarm.fireAlarm === true || currentAlarm.cctvAlarm === true || currentAlarm.psmAlarm === true || currentAlarm.etcAlarm === true)) {
            openCloseClass = dashboard.sensorAlarm;
        }

        if (open === true) {
            upDownClass = dashboard.upIcon;
            openCloseClass = dashboard.sensorInfoOpen;

            if (currentAlarm !== null && currentAlarm !== undefined &&
                (currentAlarm.fireAlarm === true || currentAlarm.cctvAlarm === true || currentAlarm.psmAlarm === true || currentAlarm.etcAlarm === true)) {
                openCloseClass = dashboard.sensorAlarmOpen;
            }
        }

        return [upDownClass, openCloseClass];
    }

    openCloseComponet = () => {
        let open = this.state.open;

        if (open === false)
            open = true;
        else
            open = false

        this.setState({ open: open});
    }

    typeAlarmSort = (buildingGroupName, sensorList, alarms, siteID) => {
        const type = this.state.type;

        let enabledFireSensors = sensorList.fireSensors.length - sensorList.disabledFireSensors;
        let enabledPSMSensors = sensorList.psmSensors.length - sensorList.disabledPSMSensors.length;
        let enabledEtcSensors = sensorList.etcSensors.length - sensorList.disabledEtcSensors.length;
        let enabledCCTVs = sensorList.cctvs.length - sensorList.disabledCCTVs.length;

        let typeAlarmSort = [];

        // 타입에 따라 데이터 순서

        if (siteID === ProjectResource.Site.GCC) {
            // 녹십자
            if (type === DashboardResource.displayInfoType.FIRE) {
                typeAlarmSort.push({ typeName: "화재", sensorNum: sensorList.fireSensors.length, enabled: enabledFireSensors, alarm: alarms.fireAlarm, type: DashboardResource.displayInfoType.FIRE });
                typeAlarmSort.push({ typeName: "누출", sensorNum: (sensorList.psmSensors.length), enabled: enabledPSMSensors, alarm: alarms.psmAlarm, type: DashboardResource.displayInfoType.PSM });
                typeAlarmSort.push({ typeName: "CCTV", sensorNum: sensorList.cctvs.length, enabled: enabledCCTVs, alarm: alarms.cctvAlarm, type: DashboardResource.displayInfoType.INTELLIGENT });
                
            } else if (type === DashboardResource.displayInfoType.INTELLIGENT) {
                typeAlarmSort.push({ typeName: "CCTV", sensorNum: sensorList.cctvs.length, enabled: enabledCCTVs, alarm: alarms.cctvAlarm, type: DashboardResource.displayInfoType.INTELLIGENT });
                typeAlarmSort.push({ typeName: "화재", sensorNum: sensorList.fireSensors.length, enabled: enabledFireSensors, alarm: alarms.fireAlarm, type: DashboardResource.displayInfoType.FIRE });
                typeAlarmSort.push({ typeName: "누출", sensorNum: (sensorList.psmSensors.length), enabled: enabledPSMSensors, alarm: alarms.psmAlarm, type: DashboardResource.displayInfoType.PSM });

            } else if (type === DashboardResource.displayInfoType.PSM) {
                typeAlarmSort.push({ typeName: "누출", sensorNum: (sensorList.psmSensors.length), enabled: enabledPSMSensors, alarm: alarms.psmAlarm, type: DashboardResource.displayInfoType.PSM });
                typeAlarmSort.push({ typeName: "화재", sensorNum: sensorList.fireSensors.length, enabled: enabledFireSensors, alarm: alarms.fireAlarm, type: DashboardResource.displayInfoType.FIRE });
                typeAlarmSort.push({ typeName: "CCTV", sensorNum: sensorList.cctvs.length, enabled: enabledCCTVs, alarm: alarms.cctvAlarm, type: DashboardResource.displayInfoType.INTELLIGENT });
            }
        } else {
            //솔브레인
            if (type === DashboardResource.displayInfoType.FIRE) {
                typeAlarmSort.push({ typeName: "화재", sensorNum: sensorList.fireSensors.length, enabled: enabledFireSensors, alarm: alarms.fireAlarm, type: DashboardResource.displayInfoType.FIRE });
                typeAlarmSort.push({ typeName: "누출", sensorNum: (sensorList.psmSensors.length), enabled: enabledPSMSensors, alarm: alarms.psmAlarm, type: DashboardResource.displayInfoType.PSM });
                typeAlarmSort.push({ typeName: "ETC", sensorNum: (sensorList.etcSensors.length), enabled: enabledEtcSensors, alarm: alarms.etcAlarm, type: DashboardResource.displayInfoType.ETC });
                typeAlarmSort.push({ typeName: "CCTV", sensorNum: sensorList.cctvs.length, enabled: enabledCCTVs, alarm: alarms.cctvAlarm, type: DashboardResource.displayInfoType.INTELLIGENT });

            } else if (type === DashboardResource.displayInfoType.INTELLIGENT) {
                typeAlarmSort.push({ typeName: "CCTV", sensorNum: sensorList.cctvs.length, enabled: enabledCCTVs, alarm: alarms.cctvAlarm, type: DashboardResource.displayInfoType.INTELLIGENT });
                typeAlarmSort.push({ typeName: "화재", sensorNum: sensorList.fireSensors.length, enabled: enabledFireSensors, alarm: alarms.fireAlarm, type: DashboardResource.displayInfoType.FIRE });
                typeAlarmSort.push({ typeName: "누출", sensorNum: (sensorList.psmSensors.length), enabled: enabledPSMSensors, alarm: alarms.psmAlarm, type: DashboardResource.displayInfoType.PSM });
                typeAlarmSort.push({ typeName: "ETC", sensorNum: (sensorList.etcSensors.length), enabled: enabledEtcSensors, alarm: alarms.etcAlarm, type: DashboardResource.displayInfoType.ETC });

            } else if (type === DashboardResource.displayInfoType.PSM) {
                typeAlarmSort.push({ typeName: "누출", sensorNum: (sensorList.psmSensors.length), enabled: enabledPSMSensors, alarm: alarms.psmAlarm, type: DashboardResource.displayInfoType.PSM });
                typeAlarmSort.push({ typeName: "화재", sensorNum: sensorList.fireSensors.length, enabled: enabledFireSensors, alarm: alarms.fireAlarm, type: DashboardResource.displayInfoType.FIRE });
                typeAlarmSort.push({ typeName: "ETC", sensorNum: (sensorList.etcSensors.length), enabled: enabledEtcSensors, alarm: alarms.etcAlarm, type: DashboardResource.displayInfoType.ETC });
                typeAlarmSort.push({ typeName: "CCTV", sensorNum: sensorList.cctvs.length, enabled: enabledCCTVs, alarm: alarms.cctvAlarm, type: DashboardResource.displayInfoType.INTELLIGENT });

            } else if (type === DashboardResource.displayInfoType.ETC) {
                typeAlarmSort.push({ typeName: "ETC", sensorNum: (sensorList.etcSensors.length), enabled: enabledEtcSensors, alarm: alarms.etcAlarm, type: DashboardResource.displayInfoType.ETC });
                typeAlarmSort.push({ typeName: "화재", sensorNum: sensorList.fireSensors.length, enabled: enabledFireSensors, alarm: alarms.fireAlarm, type: DashboardResource.displayInfoType.FIRE });
                typeAlarmSort.push({ typeName: "누출", sensorNum: (sensorList.psmSensors.length), enabled: enabledPSMSensors, alarm: alarms.psmAlarm, type: DashboardResource.displayInfoType.PSM });
                typeAlarmSort.push({ typeName: "CCTV", sensorNum: sensorList.cctvs.length, enabled: enabledCCTVs, alarm: alarms.cctvAlarm, type: DashboardResource.displayInfoType.INTELLIGENT });
            }
        }

        return typeAlarmSort;
    }

    getCurrentAlarmUI = (currentAlarm) => {
        let currentAlarmUI = [];

        if (currentAlarm === null || currentAlarm === undefined)
            return currentAlarmUI;

        const buildingGroupID = this.props.buildingGroupID;

        if (currentAlarm.fireAlarm === true)
            currentAlarmUI.push(<span key={"fireAlarm_" + buildingGroupID} className={dashboard.popFireIcon}></span>);
        if (currentAlarm.psmAlarm === true)
            currentAlarmUI.push(<span key={"iotAlarm" + buildingGroupID} className={dashboard.popIoTIcon}></span>);
        if (currentAlarm.etcAlarm === true)
           currentAlarmUI.push(<span key={"etcAlarm" + buildingGroupID} className={dashboard.popEtcIcon}></span>);
        if (currentAlarm.cctvAlarm === true)
            currentAlarmUI.push(<span key={"cctvAlarm" + buildingGroupID} className={dashboard.popCCTVIcon}></span>); 

        return currentAlarmUI;
    }

    displaySiteUI = () => {
        const siteID = ProjectResource.SiteID;
        let displaySiteUI = [];

        const [buildingGroupName, displayText, sensorList, alarms, currentAlarm] = this.init();

        let typeAlarmSort = this.typeAlarmSort(buildingGroupName, sensorList, alarms, siteID);
        let [upDownClass, openCloseClass] = this.getOpenCloseUI(currentAlarm);

        const currentAlarmUI = this.getCurrentAlarmUI(currentAlarm);

        if (siteID === ProjectResource.Site.GCC) {
            /* 녹십자 */
            displaySiteUI.push(
                <div className={openCloseClass}>
                    <div className={dashboard.sensorTitleGC}><div className={dashboard.sersorPopTxt}>{displayText}</div>
                        <div className={dashboard.sensorIconAreaGC}>
                            {currentAlarmUI}
                        </div>
                        <span className={upDownClass} onClick={() => this.openCloseComponet()}></span></div>
                    <div className={dashboard.sensorText}>
                        <span className={dashboard.sensorReport}>{typeAlarmSort[0].typeName}: </span>
                        <span className={dashboard.senserNum}>{typeAlarmSort[0].enabled}</span> /
                        <span className={dashboard.senserNum2}>{typeAlarmSort[0].sensorNum}</span> /
                        <span className={dashboard.senserNum3}>{typeAlarmSort[0].alarm}</span>
                    </div>
                    <div onClick={() => this.props.changeType(typeAlarmSort[1].type)} className={dashboard.sensorText}>
                        <span className={dashboard.sensorReport}>{typeAlarmSort[1].typeName}: </span>
                        <span className={dashboard.senserNum}>{typeAlarmSort[1].enabled}</span> /
                        <span className={dashboard.senserNum2}>{typeAlarmSort[1].sensorNum}</span> /
                        <span className={dashboard.senserNum3}>{typeAlarmSort[1].alarm}</span>
                    </div>
                    <div onClick={() => this.props.changeType(typeAlarmSort[2].type)} className={dashboard.sensorText}>
                        <span className={dashboard.sensorReport}>{typeAlarmSort[2].typeName}: </span>
                        <span className={dashboard.senserNum}>{typeAlarmSort[2].enabled}</span> /
                        <span className={dashboard.senserNum2}>{typeAlarmSort[2].sensorNum}</span> /
                        <span className={dashboard.senserNum3}>{typeAlarmSort[2].alarm}</span>
                    </div>
                </div>
            );
        } else {
            /* 솔브레인 */
            displaySiteUI.push(
                <div className={openCloseClass}>
                    <div className={dashboard.sensorTitle}><div className={dashboard.sersorPopTxt}>{displayText}</div>
                        <div className={dashboard.sensorIconArea}>
                            {currentAlarmUI}
                        </div>
                        <span className={upDownClass} onClick={() => this.openCloseComponet()}></span></div>
                    <div className={dashboard.sensorText}>
                        <span className={dashboard.sensorReport}>{typeAlarmSort[0].typeName}: </span>
                        <span className={dashboard.senserNum}>{typeAlarmSort[0].enabled}</span> /
                        <span className={dashboard.senserNum2}>{typeAlarmSort[0].sensorNum}</span> /
                        <span className={dashboard.senserNum3}>{typeAlarmSort[0].alarm}</span>
                    </div>
                    <div onClick={() => this.props.changeType(typeAlarmSort[1].type)} className={dashboard.sensorText}>
                        <span className={dashboard.sensorReport}>{typeAlarmSort[1].typeName}: </span>
                        <span className={dashboard.senserNum}>{typeAlarmSort[1].enabled}</span> /
                        <span className={dashboard.senserNum2}>{typeAlarmSort[1].sensorNum}</span> /
                        <span className={dashboard.senserNum3}>{typeAlarmSort[1].alarm}</span>
                    </div>
                    <div onClick={() => this.props.changeType(typeAlarmSort[2].type)} className={dashboard.sensorText}>
                        <span className={dashboard.sensorReport}>{typeAlarmSort[2].typeName}: </span>
                        <span className={dashboard.senserNum}>{typeAlarmSort[2].enabled}</span> /
                        <span className={dashboard.senserNum2}>{typeAlarmSort[2].sensorNum}</span> /
                        <span className={dashboard.senserNum3}>{typeAlarmSort[2].alarm}</span>
                    </div>
                    <div onClick={() => this.props.changeType(typeAlarmSort[3].type)} className={dashboard.sensorText}>
                        <span className={dashboard.sensorReport}>{typeAlarmSort[3].typeName}: </span>
                        <span className={dashboard.senserNum}>{typeAlarmSort[3].enabled}</span> /
                        <span className={dashboard.senserNum2}>{typeAlarmSort[3].sensorNum}</span> /
                        <span className={dashboard.senserNum3}>{typeAlarmSort[3].alarm}</span>
                    </div>
                </div>
            );
        }

        return displaySiteUI;
    }

    render() {
        const displaySiteUI = this.displaySiteUI();

        return (
            <>
                {/* 사이트별 UI */
                    displaySiteUI
                }
            </>
        );
    }
}
export default AlarmComponet;