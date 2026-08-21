import { ui } from 'jquery';
import React, { Component } from 'react';
import content from '../../../Common/css/content.module.css';
import uis from '../../../Common/css/ui.module.css';

import imgGrayLightIco from '../../../Common/image/icon/gray_light_ico.png';
import imgRedLightIco from '../../../Common/image/icon/red_light_ico.png';
import SdmsResource from '../../resource/id';
import { SDMSDataManager } from '../../services/sdmsDataManager';
import SDMS from '../sdms';
import SDMSMainMenu from '../sdmsMainMenu';


class StatusInfoZone extends Component {
    constructor(props) {
        super(props);

        this.refZoneName = React.createRef();
        this.refZoneNameList = React.createRef();
        this.refSensors = React.createRef();
        this.refSensorsList = React.createRef();
        this.refFireSensors = React.createRef();
        this.refFireSensorsList = React.createRef();
        this.refPsmSensors = React.createRef();
        this.refPsmSensorsList = React.createRef();
        this.refEtcSensors = React.createRef();
        this.refEtcSensorsList = React.createRef();
        this.refCCTVGroups = React.createRef();
        this.refCCTVGroupsList = React.createRef();
        this.refCCTVSubGroups = React.createRef();
        this.refCCTVSubGroupsList = React.createRef();
        this.refFacilityGroups = React.createRef();
        this.refFacilityGroupsList = React.createRef();
        this.refFacilitySubGroups = React.createRef();
        this.refFacilitySubGroupsList = React.createRef();

        // 사용자가 마우스로 조작하였는가?
        // true : 접혔다.
        // false : 펼쳐졌다.
        this.manualZoneNameExpand = null;
        this.showZoneNameResult = false;
        this.manualSensorsExpand = null;
        this.showSensorsResult = false;
        this.manualFireSensorsExpand = null;
        this.showFireSensorsResult = false;
        this.manualPsmSensorsExpand = null;
        this.showPsmSensorsResult = false;
        this.manualEtcSensorsExpand = null;
        this.showEtcSensorsResult = false;
        this.manualCCTVGroupsExpand = null;
        this.showCCTVGroupsResult = false;
        this.manualCCTVSubGroupsExpand = null;
        this.showCCTVSubGroupsResult = false;
        this.manualFacilityGroupsExpand = null;
        this.showFacilityGroupsResult = false;
        this.manualFacilitySubGroupsExpand = null;
        this.showFacilitySubGroupsResult = false;

        this.moveToX = this.moveToX.bind(this);
        this.prevSelectedSensor = [null, null, null];
    }

    componentDidMount() {
        this.checkChildVisible();

    }

    componentDidUpdate(prevProps, prevState) {
        this.checkChildVisible();
    }

    checkChildVisible() {
        this.checkChildVisibleData(this.refZoneName.current, this.refZoneNameList.current, this.showZoneNameResult);
        this.checkChildVisibleData(this.refSensors.current, this.refSensorsList.current, this.showSensorsResult);
        this.checkChildVisibleData(this.refFireSensors.current, this.refFireSensorsList.current, this.showFireSensorsResult);
        this.checkChildVisibleData(this.refPsmSensors.current, this.refPsmSensorsList.current, this.showPsmSensorsResult);
        this.checkChildVisibleData(this.refEtcSensors.current, this.refEtcSensorsList.current, this.showEtcSensorsResult);
        this.checkChildVisibleData(this.refCCTVGroups.current, this.refCCTVGroupsList.current, this.showCCTVGroupsResult);
        this.checkChildVisibleData(this.refCCTVSubGroups.current, this.refCCTVSubGroupsList.current, this.showCCTVSubGroupsResult);
        this.checkChildVisibleData(this.refFacilityGroups.current, this.refFacilityGroupsList.current, this.showFacilityGroupsResult);
        this.checkChildVisibleData(this.refFacilitySubGroups.current, this.refFacilitySubGroupsList.current, this.showFacilitySubGroupsResult);
    }

    checkChildVisibleData(mainElement, listElement, showChild) {
        if (mainElement) {
            if (showChild) {
                if (mainElement.dataset.show_child !== 'true') {
                    mainElement.dataset.show_child = 'true';
                }

                if (listElement.classList.contains(content.on) === false) {
                    listElement.classList.add(content.on);
                }
            }
            else {
                if (mainElement.dataset.show_child !== 'false') {
                    mainElement.dataset.show_child = 'false';
                }

                if (listElement.classList.contains(content.on)) {
                    listElement.classList.remove(content.on);
                }
            }
        }
    }

    moveToX() {
        this.props.moveToX(SDMSMainMenu.Menu_MoveTo_Floor, this.props.zone);
    }

    moveToSensor(sensorType, sensorID) {
        if (sensorType === SDMSMainMenu.Facility) {
            this.props.moveToX(SDMSMainMenu.Menu_MoveTo_Facility, [this.props.zone.id, sensorID]);
        }
        else {
            this.props.moveToX(SDMSMainMenu.Menu_MoveTo_POI, [this.props.zone.id, sensorType, sensorID]);
        }        
    }

    onSelectSensor(sensorType, sensorID) {
        this.props.onSelectSensor(sensorType, this.props.zone.id, sensorID);
    }

    isAlarmSensor(facilityType, sensorID) {
        let alarmImgID = content.lightGrayICO;
        let alarmImgSrc = imgGrayLightIco;
        
        if (this.props.sensorAlarms) {
            for (let j = 0; j < this.props.sensorAlarms.length; j++) {
                const alarm = this.props.sensorAlarms[j];
                if (!alarm.isAlarm) {
                    // 알람 발생한 센서는 상단에 있기 때문에 isAlarm=false가 나온 시점 이후에는 다 false만 있음
                    break;
                }
                if (facilityType === SdmsResource.facilityType.FIRE ||
                    (facilityType === SdmsResource.facilityType.PSM_SENSOR && SdmsResource.isPSMSensorType(facilityType)) ||
                    (facilityType === SdmsResource.facilityType.ETC && SdmsResource.isETCSensorType(facilityType)) ||
                    (facilityType === SdmsResource.facilityType.Intrusion_S1 && SdmsResource.isSVMSSensorType(facilityType))) {
                    if (/*alarm.facilityType === facilityType && */alarm.orgSensorID === sensorID && alarm.isAlarm) {
                        alarmImgID = content.lightRedICO;
                        alarmImgSrc = imgRedLightIco;
                        break;
                    }
                }
            }
        }
        
        return [alarmImgID, alarmImgSrc];
    }

    getSensorUI() {
        let fireSensorUI = [];
        let psmSensorUI = [];
        let etcSensorUI = [];
        let cctvUI = [];
        let facilityInfosUI = [];

        const [sensorType, zoneID, sensorID] = this.props.selectedSensor;

        //const sensorList = this.props.sensorList;
        //if (sensorList === undefined || sensorList === null)
        //    return ui;
        
        if (this.props.fireSensors) {
            for (let i = 0; i < this.props.fireSensors.length; i++) {
                const sensor = this.props.fireSensors[i];
                const sensorClassName = sensorType === SDMSMainMenu.Fire_Sensor && sensorID === sensor.id ? content.viewList5DepthTxt + " " + content.selected : content.viewList5DepthTxt;

                if (sensor.zoneID === this.props.zone.id) {
                    if (this.props.isEditMode) {
                        fireSensorUI.push(
                            <li key={'fireSensor_' + sensor.id} id={'fireSensor_' + sensor.id}>
                                <span className={sensorClassName} onClick={() => this.moveToSensor(SDMSMainMenu.Fire_Sensor, sensor.id)}>{sensor.name}</span>
                            </li>
                        );
                    }
                    else {
                        const [alarmImgID, alarmImgSrc] = this.isAlarmSensor(SdmsResource.facilityType.FIRE, sensor.id); // 알람이 발생한 센서인가 ?

                        if (this.props.hasIndoorModel || (sensor.x && sensor.y && sensor.z)) {
                            fireSensorUI.push(
                                <li key={'fireSensor_' + sensor.id} id={'fireSensor_' + sensor.id}>
                                    <span className={sensorClassName} onClick={() => this.moveToSensor(SDMSMainMenu.Fire_Sensor, sensor.id)}>{sensor.name}</span>
                                    <div className={uis.floatR + ' ' + content.posiRelative + " " + content.flexBox + " " + content.linkArea}>
                                        <span className={content.goLink} onClick={() => this.moveToSensor(SDMSMainMenu.Fire_Sensor, sensor.id)}><a className={content.goA}>이동</a></span>
                                        <div className={content.iconHorizontal}>
                                            <img className={content.alarmImg} id={alarmImgID} src={alarmImgSrc} />
                                            <span className={content.greenDOTT}></span>
                                        </div>
                                    </div>
                                </li>
                            );
                        }
                        else {
                            fireSensorUI.push(
                                <li key={'fireSensor_' + sensor.id} id={'fireSensor_' + sensor.id}>
                                    <span className={sensorClassName}>{sensor.name}</span>
                                    <div className={uis.floatR + ' ' + content.posiRelative + " " + content.flexBox}>
                                        <span><a/></span>
                                        <div className={content.iconHorizontal}>
                                            <img className={content.alarmImg} id={alarmImgID} src={alarmImgSrc} />
                                            <span className={content.greenDOTT}></span>
                                        </div>
                                    </div>
                                </li>
                            );
                        }
                    }
                }
            }
        }
        
        if (this.props.psmSensors) {
            for (let i = 0; i < this.props.psmSensors.length; i++) {
                const sensor = this.props.psmSensors[i];
                const sensorClassName = sensorType === SDMSMainMenu.PSM_Sensor && sensorID === sensor.id ? content.viewList5DepthTxt + " " + content.selected : content.viewList5DepthTxt;

                if (!sensor.linkedZones)
                    continue;

                if (sensor.zoneID === this.props.zone.id) {
                    if (this.props.isEditMode) {
                        psmSensorUI.push(
                            <li key={'psmSensor_' + sensor.id} id={'psmSensor_' + sensor.id}>
                                <span className={sensorClassName} onClick={() => this.moveToSensor(SDMSMainMenu.PSM_Sensor, sensor.id)}>{sensor.name}</span>
                            </li>
                        );
                    }
                    else {
                        const [alarmImgID, alarmImgSrc] = this.isAlarmSensor(SdmsResource.facilityType.PSM_SENSOR, sensor.id); // 알람이 발생한 센서인가 ?
                        let enableColor = content.greenDOTT;
                        if (!sensor.enabled) {
                            enableColor = content.grayDOTT;
                        }

                        if (this.props.hasIndoorModel || (sensor.x && sensor.y && sensor.z)) {
                            psmSensorUI.push(
                                <li key={'psmSensor_' + sensor.id} id={'psmSensor_' + sensor.id}>
                                    <span className={sensorClassName} onClick={() => this.moveToSensor(SDMSMainMenu.PSM_Sensor, sensor.id)}>{sensor.name}</span>
                                    <div className={uis.floatR + ' ' + content.posiRelative + " " + content.flexBox + " " + content.linkArea}>
                                        <span className={content.goLink} onClick={() => this.moveToSensor(SDMSMainMenu.PSM_Sensor, sensor.id)}><a className={content.goA}>이동</a></span>
                                        <div className={content.iconHorizontal}>
                                            <img className={content.alarmImg} id={alarmImgID} src={alarmImgSrc} />
                                            <span className={enableColor}></span>
                                        </div>
                                    </div>
                                </li>
                            );
                        } else {
                            psmSensorUI.push(
                                <li key={'psmSensor_' + sensor.id} id={'psmSensor_' + sensor.id}>
                                    <span className={sensorClassName}>{sensor.name}</span>
                                    <div className={uis.floatR + ' ' + content.posiRelative + " " + content.flexBox}>
                                        <span><a /></span>
                                        <div className={content.iconHorizontal}>
                                            <img className={content.alarmImg} id={alarmImgID} src={alarmImgSrc} />
                                            <span className={enableColor}></span>
                                        </div>
                                    </div>
                                </li>
                            );
                        }

                    }
                }
            }
        }

        if (this.props.etcSensors) {
            // etc 센서는 복합센서라서 하나의 센서가 여러개의 타입을 가질수 있다.
            // 같은 이름의 센서는 하나만 표시하도록 한다.
            let prevSensorName = "";

            for (let i = 0; i < this.props.etcSensors.length; i++) {
                const sensor = this.props.etcSensors[i];

                if (sensor.name === prevSensorName)
                    continue;
                else
                    prevSensorName = sensor.name;

                const sensorClassName = sensorType === SDMSMainMenu.Etc_Sensor && sensorID === sensor.id ? content.viewList5DepthTxt + " " + content.selected : content.viewList5DepthTxt;

                if (sensor.zoneID === this.props.zone.id) {
                    if (this.props.isEditMode) {
                        etcSensorUI.push(
                            <li key={'etcSensor_' + sensor.id} id={'etcSensor_' + sensor.id}>
                                <span className={sensorClassName} onClick={() => this.moveToSensor(SDMSMainMenu.Etc_Sensor, sensor.id)}>{sensor.name}</span>
                            </li>
                        );
                    }
                    else {
                        const [alarmImgID, alarmImgSrc] = this.isAlarmSensor(SdmsResource.facilityType.ETC, sensor.id); // 알람이 발생한 센서인가 ?
                        let enableColor = content.greenDOTT;
                        if (!sensor.enabled) {
                            enableColor = content.grayDOTT;
                        }

                        if (this.props.hasIndoorModel || (sensor.x && sensor.y && sensor.z)) {
                            etcSensorUI.push(
                                <li key={'etcSensor_' + sensor.id} id={'etcSensor_' + sensor.id}>
                                    <span className={sensorClassName} onClick={() => this.moveToSensor(SDMSMainMenu.Etc_Sensor, sensor.id)}>{sensor.name}</span>
                                    <div className={uis.floatR + ' ' + content.posiRelative + " " + content.flexBox + " " + content.linkArea}>
                                        <span className={content.goLink} onClick={() => this.moveToSensor(SDMSMainMenu.Etc_Sensor, sensor.id)}>
                                            <a className={content.goA}>이동</a>
                                        </span>
                                        <div className={content.iconHorizontal}>
                                            <img className={content.alarmImg} id={alarmImgID} src={alarmImgSrc} />
                                            <span className={enableColor}></span>
                                        </div>
                                    </div>
                                </li>
                            );
                        }
                        else {
                            etcSensorUI.push(
                                <li key={'etcSensor_' + sensor.id} id={'etcSensor_' + sensor.id}>
                                    <span className={sensorClassName}>{sensor.name}</span>
                                    <div className={uis.floatR + ' ' + content.posiRelative + " " + content.flexBox}>
                                        <span><a /></span>
                                        <div className={content.iconHorizontal}>
                                            <img className={content.alarmImg} id={alarmImgID} src={alarmImgSrc} />
                                            <span className={enableColor}></span>
                                        </div>
                                    </div>
                                </li>
                            );
                        }
                    }
                }
            }
        }

        if (this.props.cctvs) {
            for (let i = 0; i < this.props.cctvs.length; i++) {
                const sensor = this.props.cctvs[i];
                const sensorClassName = sensorType === SDMSMainMenu.CCTV_Type && sensorID === sensor.id ? content.viewList5DepthTxt + " " + content.selected : content.viewList5DepthTxt;

                if (sensor.zoneID === this.props.zone.id) {
                    let enableColor = content.grayDOTT;
                    if (sensor.enabled === true || sensor.enabled === null) {
                        enableColor = content.greenDOTT;
                    }

                    const [alarmImgID, alarmImgSrc] = this.isAlarmSensor(SdmsResource.facilityType.Intrusion_S1, sensor.id); // 알람이 발생한 센서인가 ?

                    cctvUI.push(
                        <li key={'cctv_' + sensor.id} id={'cctv_' + sensor.id}>
                            <span className={sensorClassName} /* style={{ width: '147px' }} */ onClick={() => this.moveToSensor(SDMSMainMenu.CCTV_Type, sensor.id)}>{sensor.name}</span>
                            {
                                (this.props.isEditMode === false && (this.props.hasIndoorModel || (sensor.x && sensor.y && sensor.z))) &&
                                <>
                                    <div className={content.linkArea}>
                                        <span className={content.goLink} onClick={() => this.moveToSensor(SDMSMainMenu.CCTV_Type, sensor.id)}>
                                            <a className={content.goA}>이동</a>
                                        </span>
                                        <div className={content.iconHorizontal}>
                                            <img className={content.alarmImg} id={alarmImgID} src={alarmImgSrc} />
                                            <span className={enableColor}></span>
                                        </div>
                                   </div>
                                </>
                            }
                        </li>
                    );
                }
            }
        }

        if (this.props.facilityInfos) {

            const selectedFacility = this.props.selectedFacility;
            //getFacilityID = { this.props.getFacilityID }

            for (let i = 0; i < this.props.facilityInfos.length; i++) {
                const info = this.props.facilityInfos[i];                

                if (info.zoneID === this.props.zone.id) {
                    const sensorClassName = selectedFacility.facilityID === info.id ? content.viewList5DepthTxt + " " + content.selected : content.viewList5DepthTxt;

                    facilityInfosUI.push(
                        <li key={'facilityInfo_' + info.id} id={'facilityInfo_' + info.id}>
                            <span className={sensorClassName} style={{ width: '147px' }} onClick={() => this.moveToSensor(SDMSMainMenu.Facility, info.id)}>{info.facilityName}</span>
                            {
                                (this.props.isEditMode === false) &&
                                <>
                                    <span className={content.goLink} onClick={() => this.moveToSensor(SDMSMainMenu.Facility, info.id)}>
                                        <a className={content.goA}>이동</a>
                                    </span>
                                </>
                            }
                        </li>
                    );
                }
            }
        }

        return [fireSensorUI, psmSensorUI, etcSensorUI, cctvUI, facilityInfosUI];
    }

    showChild(e) {
        const expand = this.props.showChild(e);

        // 현황정보 트리를 선택시 기존 선택된 POI는 선택해제 - K.D.R
        this.onSelectSensor(null, null);

        if (e.target === this.refZoneName.current) {
            this.manualZoneNameExpand = expand;
            if (this.manualZoneNameExpand && this.props.onChangeBuildingGroup) {
                this.props.onChangeBuildingGroup(this.props.zone, SDMS.SelectedStatusInfoType.zone);
            } else if (!this.manualZoneNameExpand && this.props.onChangeBuildingGroup) {
                // 층 트리가 열린 상태에서 다시 클릭하면 닫혀야 하는데 props값이 열린 상태로 유지되어서 닫히지 않는 오류 >> 층 트리가 닫힐 경우도 추가 - K.D.R
                this.props.onChangeBuildingGroup("zone", SDMS.SelectedStatusInfoType.closeZone);
            }
        }
        else if (e.target === this.refSensors.current) {
            this.manualSensorsExpand = expand;
            if (this.manualSensorsExpand && this.props.onChangeBuildingGroup) {
                this.props.onChangeBuildingGroup('sensorGroups', SDMS.SelectedStatusInfoType.sensorGroups);
            } else if (!this.manualSensorsExpand && this.props.onChangeBuildingGroup) {
                // 센서 트리가 열린 상태에서 다시 클릭하면 닫혀야 하는데 props값이 열린 상태로 유지되어서 닫히지 않는 오류 >> 센서 트리가 닫힐 경우도 추가 - K.D.R
                this.props.onChangeBuildingGroup(this.props.zone, SDMS.SelectedStatusInfoType.zone);
            }
        }
        else if (e.target === this.refFireSensors.current) {
            this.manualFireSensorsExpand = expand;
            if (this.manualFireSensorsExpand && this.props.onChangeBuildingGroup) {
                this.props.onChangeBuildingGroup('fireSensors', SDMS.SelectedStatusInfoType.fireSensors);
            } else if (!this.manualFireSensorsExpand && this.props.onChangeBuildingGroup) {
                // 화재센서 트리가 열린 상태에서 다시 클릭하면 닫혀야 하는데 props값이 열린 상태로 유지되어서 닫히지 않는 오류 >> 화재센서 트리가 닫힐 경우도 추가 - K.D.R
                this.props.onChangeBuildingGroup('sensorGroups', SDMS.SelectedStatusInfoType.sensorGroups);
            }
        }
        else if (e.target === this.refPsmSensors.current) {
            this.manualPsmSensorsExpand = expand;
            if (this.manualPsmSensorsExpand && this.props.onChangeBuildingGroup) {
                this.props.onChangeBuildingGroup('psmSensors', SDMS.SelectedStatusInfoType.psmSensors);
            } else if (!this.manualPsmSensorsExpand && this.props.onChangeBuildingGroup) {
                // PSM센서 트리가 열린 상태에서 다시 클릭하면 닫혀야 하는데 props값이 열린 상태로 유지되어서 닫히지 않는 오류 >> PSM센서 트리가 닫힐 경우도 추가 - K.D.R
                this.props.onChangeBuildingGroup('sensorGroups', SDMS.SelectedStatusInfoType.sensorGroups);
            }
        }
        else if (e.target === this.refEtcSensors.current) {
            this.manualEtcSensorsExpand = expand;
            if (this.manualEtcSensorsExpand && this.props.onChangeBuildingGroup) {
                this.props.onChangeBuildingGroup('etcSensors', SDMS.SelectedStatusInfoType.etcSensors);
            } else if (!this.manualEtcSensorsExpand && this.props.onChangeBuildingGroup) {
                // ETC센서 트리가 열린 상태에서 다시 클릭하면 닫혀야 하는데 props값이 열린 상태로 유지되어서 닫히지 않는 오류 >> ETC센서 트리가 닫힐 경우도 추가 - K.D.R
                this.props.onChangeBuildingGroup('sensorGroups', SDMS.SelectedStatusInfoType.sensorGroups);
            }
        }
        else if (e.target === this.refCCTVGroups.current) {
            this.manualCCTVGroupsExpand = expand;
            if (this.manualCCTVGroupsExpand && this.props.onChangeBuildingGroup) {
                this.props.onChangeBuildingGroup('cctvGroups', SDMS.SelectedStatusInfoType.cctvGroups);
            } else if (!this.manualCCTVGroupsExpand && this.props.onChangeBuildingGroup) {
                // CCTV 트리가 열린 상태에서 다시 클릭하면 닫혀야 하는데 props값이 열린 상태로 유지되어 닫히지 않는 오류 >> CCTV 트리가 닫힐 경우도 추가 - K.D.R
                this.props.onChangeBuildingGroup(this.props.zone, SDMS.SelectedStatusInfoType.zone);
            }
        }
        else if (e.target === this.refCCTVSubGroups.current) {
            this.manualCCTVSubGroupsExpand = expand;
            if (this.manualCCTVSubGroupsExpand && this.props.onChangeBuildingGroup) {
                this.props.onChangeBuildingGroup('cctvSubGroups', SDMS.SelectedStatusInfoType.cctvSubGroups);
            } else if (!this.manualCCTVSubGroupsExpand && this.props.onChangeBuildingGroup) {
                // CCTV 서브트리가 열린 상태에서 다시 클릭하면 닫혀야 하는데 props값이 열린 상태로 유지되어 닫히지 않는 오류 >> CCTV 서브 트리가 닫힐 경우도 추가 - K.D.R
                this.props.onChangeBuildingGroup('cctvGroups', SDMS.SelectedStatusInfoType.cctvGroups);
            }
        }
        else if (e.target === this.refFacilityGroups.current) {
            this.manualFacilityGroupsExpand = expand;
            if (this.manualFacilityGroupsExpand && this.props.onChangeBuildingGroup) {
                this.props.onChangeBuildingGroup('facilityGroups', SDMS.SelectedStatusInfoType.facilityGroups);
            } else if (!this.manualFacilityGroupsExpand && this.props.onChangeBuildingGroup) {
                // 설비 트리가 열린 상태에서 다시 클릭하면 닫혀야 하는데 props값이 열린 상태로 유지되어서 닫히지 않는 오류 >> 설비 트리가 닫힐 경우도 추가 - K.D.R
                this.props.onChangeBuildingGroup(this.props.zone, SDMS.SelectedStatusInfoType.zone);
            }
        }
        else if (e.target === this.refFacilitySubGroups.current) {
            this.manualFacilitySubGroupsExpand = expand;
            if (this.manualFacilitySubGroupsExpand && this.props.onChangeBuildingGroup) {
                this.props.onChangeBuildingGroup('facilitySubGroups', SDMS.SelectedStatusInfoType.facilitySubGroups);
            } else if (!this.manualFacilitySubGroupsExpand && this.props.onChangeBuildingGroup) {
                // 설비 서브트리가 열린 상태에서 다시 클릭하면 닫혀야 하는데 props값이 열린 상태로 유지되어 닫히지 않는 오류 >> 설비 서브트리가 닫힐 경우도 추가 - K.D.R
                this.props.onChangeBuildingGroup('facilityGroups', SDMS.SelectedStatusInfoType.facilityGroups);
            }
        }        
    }

    isSelected() {
        let zoneShowChild = 'false';
        let sensorsShowChild = 'false';
        let fireSensorsShowChild = 'false';
        let psmSensorsShowChild = 'false';
        let etcSensorsShowChild = 'false';
        let cctvGroupsShowChild = 'false';
        let cctvSubGroupsShowChild = 'false';
        let facilityGroupsShowChild = 'false';
        let facilitySubGroupsShowChild = 'false';

        const [sensorType, zoneID, sensorID] = this.props.selectedSensor;

        if (this.prevSelectedSensor[0] !== sensorType ||
            this.prevSelectedSensor[1] !== zoneID ||
            this.prevSelectedSensor[2] !== sensorID) {

            this.manualZoneNameExpand = null;
            this.manualSensorsExpand = null;
            this.manualFireSensorsExpand = null;
            this.manualPsmSensorsExpand = null;
            this.manualEtcSensorsExpand = null;
            this.manualCCTVGroupsExpand = null;
            this.manualCCTVSubGroupsExpand = null;
        }

        this.prevSelectedSensor = [sensorType, zoneID, sensorID];

        if (sensorType !== null && zoneID !== null && sensorID !== null) {
            const zoneData = this.props.zone;

            if (zoneData && zoneData.id === zoneID) {
                // 선택된 센서가 있으니 Tree를 펼친다.
                zoneShowChild = 'true';

                if (sensorType === "cctv") {
                    cctvGroupsShowChild = 'true';
                    cctvSubGroupsShowChild = 'true';
                }
                else {
                    sensorsShowChild = 'true';

                    if (sensorType === "fire") {
                        fireSensorsShowChild = 'true';
                    }
                    else if (sensorType === "psm") {
                        psmSensorsShowChild = 'true';
                    }
                    else if (sensorType === "etc") {
                        etcSensorsShowChild = 'true';
                    }
                }
            }
        }
        else {
            if (this.props.selectedInfo) {
                if (this.props.zone === this.props.selectedInfo.zone) {
                    zoneShowChild = 'true';

                    if (this.props.selectedInfo.sensorGroups) {
                        sensorsShowChild = 'true';
                        if (this.props.selectedInfo.fireSensors) {
                            fireSensorsShowChild = 'true';
                        }
                        else if (this.props.selectedInfo.psmSensors) {
                            psmSensorsShowChild = 'true';
                        }
                        else if (this.props.selectedInfo.etcSensors) {
                            etcSensorsShowChild = 'true';
                        }
                    }                    
                    if (this.props.selectedInfo.cctvGroups) {
                        cctvGroupsShowChild = 'true';
                        if (this.props.selectedInfo.cctvSubGroups) {
                            cctvSubGroupsShowChild = 'true';
                        }
                    }                    
                    if (this.props.selectedInfo.facilityGroups) {
                        facilityGroupsShowChild = 'true';
                        if (this.props.selectedInfo.facilitySubGroups) {
                            facilitySubGroupsShowChild = 'true';
                        }
                    }                    
                }
            }
            else {
                if (this.manualZoneNameExpand !== null) {
                    zoneShowChild = this.manualZoneNameExpand ? 'true' : 'false';
                }

                if (this.manualSensorsExpand !== null) {
                    sensorsShowChild = this.manualSensorsExpand ? 'true' : 'false';
                }

                if (this.manualFireSensorsExpand !== null) {
                    fireSensorsShowChild = this.manualFireSensorsExpand ? 'true' : 'false';
                }

                if (this.manualPsmSensorsExpand !== null) {
                    psmSensorsShowChild = this.manualPsmSensorsExpand ? 'true' : 'false';
                }

                if (this.manualEtcSensorsExpand !== null) {
                    etcSensorsShowChild = this.manualEtcSensorsExpand ? 'true' : 'false';
                }

                if (this.manualCCTVGroupsExpand !== null) {
                    cctvGroupsShowChild = this.manualCCTVGroupsExpand ? 'true' : 'false';
                }

                if (this.manualCCTVSubGroupsExpand !== null) {
                    cctvSubGroupsShowChild = this.manualCCTVSubGroupsExpand ? 'true' : 'false';
                }

                if (this.manualFacilityGroupsExpand !== null) {
                    facilityGroupsShowChild = this.manualFacilityGroupsExpand ? 'true' : 'false';
                }

                if (this.manualFacilitySubGroupsExpand !== null) {
                    facilitySubGroupsShowChild = this.manualFacilitySubGroupsExpand ? 'true' : 'false';
                }
            }
        }

        return [zoneShowChild, sensorsShowChild, fireSensorsShowChild, psmSensorsShowChild, etcSensorsShowChild, cctvGroupsShowChild, cctvSubGroupsShowChild, facilityGroupsShowChild, facilitySubGroupsShowChild ];
    }

    render() {
        let [fireSensorUI, psmSensorUI, etcSensorUI, cctvUI, facilityInfosUI] = this.getSensorUI();
        const [zoneShowChild, sensorsShowChild, fireSensorsShowChild, psmSensorsShowChild, etcSensorsShowChild, cctvGroupsShowChild, cctvSubGroupsShowChild, facilityGroupsShowChild, facilitySubGroupsShowChild] = this.isSelected();
        this.showZoneNameResult = zoneShowChild === 'true';
        this.showSensorsResult = sensorsShowChild === 'true';
        this.showFireSensorsResult = fireSensorsShowChild === 'true';
        this.showPsmSensorsResult = psmSensorsShowChild === 'true';
        this.showEtcSensorsResult = etcSensorsShowChild === 'true';
        this.showCCTVGroupsResult = cctvGroupsShowChild === 'true';
        this.showCCTVSubGroupsResult = cctvSubGroupsShowChild === 'true';
        this.showFacilityGroupsResult = facilityGroupsShowChild === 'true';
        this.showFacilitySubGroupsResult = facilitySubGroupsShowChild === 'true';

        const zoneName = this.props.zone.displayText ? this.props.zone.displayText : this.props.zone.name;

        const fireSensorCount = (this.props.fireSensors) ? this.props.fireSensors.length : 0;
        const psmSensorCount = (this.props.psmSensors) ? this.props.psmSensors.length : 0;
        const etcSensorCount = (this.props.etcSensors) ? this.props.etcSensors.length : 0;
        const cctvCount = (this.props.cctvs) ? this.props.cctvs.length : 0;
        const facilityCount = (this.props.facilityInfos) ? this.props.facilityInfos.length : 0;

        const allSensorCount = fireSensorCount + psmSensorCount + etcSensorCount;

        return (            
            <li>
                <div id={this.props.id} className={content.viewList2DepthHead}>
                    <span ref={this.refZoneName} className={content.viewList2DepthSpen} data-show_child={zoneShowChild} data-target_class='viewList2Depth' onClick={(e) => { this.showChild(e) }}>{zoneName}</span>
                    {
                        (this.props.hasIndoorModel) ? <span className={content.goLink} onClick={this.moveToX}><a className={content.goA}>이동</a></span> : <></>
                    }
                </div>
                {
                    this.props.sensorList &&
                    <ul ref={this.refZoneNameList} id={'zoneArea_' + this.props.zone.id} className={zoneShowChild === 'true' ? content.viewList3Depth + " " + content.on : content.viewList3Depth}>
                        <li>
                            <div ref={this.refSensors} id={'sensorGroups_' + this.props.zone.id} className={content.viewList3DepthHead} data-show_child={sensorsShowChild} data-target_class='viewList3Depth' onClick={(e) => { this.showChild(e) }}>센서 ({allSensorCount})</div>
                            <ul ref={this.refSensorsList} id={'sensorGroupsArea_' + this.props.zone.id} className={sensorsShowChild === 'true' ? content.viewList4Depth + " " + content.on : content.viewList4Depth}>
                                <li>
                                    <span ref={this.refFireSensors} className={content.viewList4DepthHead} data-show_child={fireSensorsShowChild} data-target_class='viewList4Depth' onClick={(e) => { this.showChild(e) }}>화재센서 ({fireSensorCount})</span>
                                    <ul ref={this.refFireSensorsList} className={fireSensorsShowChild === 'true' ? content.viewList5Depth + " " + content.on : content.viewList5Depth}>
                                        {fireSensorUI}
                                    </ul>
                                </li>
                                <li>
                                    <span ref={this.refPsmSensors} className={content.viewList4DepthHead} data-show_child={psmSensorsShowChild} data-target_class='viewList4Depth' onClick={(e) => { this.showChild(e) }}>누출센서 ({psmSensorCount})</span>
                                    <ul ref={this.refPsmSensorsList} className={psmSensorsShowChild === 'true' ? content.viewList5Depth + " " + content.on : content.viewList5Depth}>
                                        {psmSensorUI}
                                    </ul>
                                </li>
                                <li>
                                    <span ref={this.refEtcSensors} className={content.viewList4DepthHead} data-show_child={etcSensorsShowChild} data-target_class='viewList4Depth' onClick={(e) => { this.showChild(e) }}>ETC센서 ({etcSensorCount})</span>
                                    <ul ref={this.refEtcSensorsList} className={etcSensorsShowChild === 'true' ? content.viewList5Depth + " " + content.on : content.viewList5Depth}>
                                        {etcSensorUI}
                                    </ul>
                                </li>
                            </ul>
                        </li>
                        <li>
                            <div ref={this.refCCTVGroups} id={'cctvGroups_' + this.props.zone.id} className={content.viewList3DepthHead} data-show_child={cctvGroupsShowChild} data-target_class='viewList3Depth' onClick={(e) => { this.showChild(e) }}>CCTV ({cctvCount})</div>
                            <ul ref={this.refCCTVGroupsList} id={'cctvGroupsArea_' + this.props.zone.id} className={cctvGroupsShowChild === 'true' ? content.viewList4Depth + " " + content.on : content.viewList4Depth}>
                                <li>
                                    <span ref={this.refCCTVSubGroups} className={content.viewList4DepthHead} data-show_child={cctvSubGroupsShowChild} data-target_class='viewList4Depth' onClick={(e) => { this.showChild(e) }}>CCTV ({cctvCount})</span>

                                    <ul ref={this.refCCTVSubGroupsList} className={cctvSubGroupsShowChild === 'true' ? content.viewList5Depth + " " + content.on : content.viewList5Depth}>
                                        {cctvUI}
                                    </ul>
                                </li>
                            </ul>
                        </li>
                        <li>
                            <div ref={this.refFacilityGroups} id={'facilityGroups_' + this.props.zone.id} className={content.viewList3DepthHead} data-show_child={facilityGroupsShowChild} data-target_class='viewList3Depth' onClick={(e) => { this.showChild(e) }}>설비 ({facilityCount})</div>
                            <ul ref={this.refFacilityGroupsList} id={'facilityGroupsArea_' + this.props.zone.id} className={facilityGroupsShowChild === 'true' ? content.viewList4Depth + " " + content.on : content.viewList4Depth}>
                                <li>
                                    <span ref={this.refFacilitySubGroups} className={content.viewList4DepthHead} data-show_child={facilitySubGroupsShowChild} data-target_class='viewList4Depth' onClick={(e) => { this.showChild(e) }}>설비 ({facilityCount})</span>

                                    <ul ref={this.refFacilitySubGroupsList} className={facilitySubGroupsShowChild === 'true' ? content.viewList5Depth + " " + content.on : content.viewList5Depth}> 
                                        {facilityInfosUI}
                                    </ul>
                                </li>
                            </ul>
                        </li>
                    </ul>
                }
            </li>
        );
    }
}

export default StatusInfoZone;