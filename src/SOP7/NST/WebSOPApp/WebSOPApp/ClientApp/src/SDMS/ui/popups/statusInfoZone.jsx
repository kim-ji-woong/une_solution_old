import { ui } from 'jquery';
import React, { Component } from 'react';
import styles from '../../css/sdms.module.css';

import imgGrayLightIco from '../../../Common/image/icon/gray_light_ico.png';
import imgRedLightIco from '../../../Common/image/icon/red_light_ico.png';
import { SDMSDataManager } from '../../services/sdmsDataManager';
import SDMSMainMenu from '../../data/sdmsMainMenu';
import ProjectResource from '../../../Root/resource/id';

class StatusInfoZone extends Component {
    constructor(props) {
        super(props);

        this.refZoneName = React.createRef();
        this.refZoneNameList = React.createRef();
        this.refSensors = React.createRef();
        this.refSensorsList = React.createRef();
        this.refFireSensors = React.createRef();
        this.refFireSensorsList = React.createRef();
        this.refCoSensors = React.createRef();
        this.refCoSensorsList = React.createRef();
        this.refO2Sensors = React.createRef();
        this.refO2SensorsList = React.createRef();
        this.refH2Sensors = React.createRef();
        this.refH2SensorsList = React.createRef();
        this.refCh4Sensors = React.createRef();
        this.refCh4SensorsList = React.createRef();
        this.refDetectSensors = React.createRef();
        this.refDetectSensorsList = React.createRef();
        /*this.refPsmSensors = React.createRef();
        this.refPsmSensorsList = React.createRef();
        this.refEtcSensors = React.createRef();
        this.refEtcSensorsList = React.createRef();*/
        this.refCCTVGroups = React.createRef();
        this.refCCTVGroupsList = React.createRef();
        this.refCCTVSubGroups = React.createRef();
        this.refCCTVSubGroupsList = React.createRef();

        // 사용자가 마우스로 조작하였는가?
        // true : 접혔다.
        // false : 펼쳐졌다.
        this.manualZoneNameExpand = null;
        this.showZoneNameResult = false;
        this.manualSensorsExpand = null;
        this.showSensorsResult = false;
        this.manualFireSensorsExpand = null;
        this.showFireSensorsResult = false;
        this.manualCoSensorsExpand = null;
        this.showCoSensorsResult = false;
        this.manualO2SensorsExpand = null;
        this.showO2SensorsResult = false;
        this.manualH2SensorsExpand = null;
        this.showH2SensorsResult = false;
        this.manualCh4SensorsExpand = null;
        this.showCh4SensorsResult = false;
        this.manualDetectSensorsExpand = null;
        this.showDetectSensorsResult = false;
        //this.manualPsmSensorsExpand = null;
        //this.showPsmSensorsResult = false;
        //this.manualEtcSensorsExpand = null;
        //this.showEtcSensorsResult = false;
        this.manualCCTVGroupsExpand = null;
        this.showCCTVGroupsResult = false;
        this.manualCCTVSubGroupsExpand = null;
        this.showCCTVSubGroupsResult = false;

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
        this.checkChildVisibleData(this.refCoSensors.current, this.refCoSensorsList.current, this.showCoSensorsResult);
        this.checkChildVisibleData(this.refO2Sensors.current, this.refO2SensorsList.current, this.showO2SensorsResult);
        this.checkChildVisibleData(this.refH2Sensors.current, this.refH2SensorsList.current, this.showH2SensorsResult);
        this.checkChildVisibleData(this.refCh4Sensors.current, this.refCh4SensorsList.current, this.showCh4SensorsResult);
        this.checkChildVisibleData(this.refDetectSensors.current, this.refDetectSensorsList.current, this.showDetectSensorsResult);
        //this.checkChildVisibleData(this.refPsmSensors.current, this.refPsmSensorsList.current, this.showPsmSensorsResult);
        //this.checkChildVisibleData(this.refEtcSensors.current, this.refEtcSensorsList.current, this.showEtcSensorsResult);
        this.checkChildVisibleData(this.refCCTVGroups.current, this.refCCTVGroupsList.current, this.showCCTVGroupsResult);
        this.checkChildVisibleData(this.refCCTVSubGroups.current, this.refCCTVSubGroupsList.current, this.showCCTVSubGroupsResult);
    }

    checkChildVisibleData(mainElement, listElement, showChild) {
        if (mainElement) {
            if (showChild) {
                if (mainElement.dataset.show_child !== 'true') {
                    mainElement.dataset.show_child = 'true';
                }

                if (listElement?.classList && listElement.classList.contains(styles.on) === false) {
                    listElement.classList.add(styles.on);
                }
            }
            else {
                if (mainElement.dataset.show_child !== 'false') {
                    mainElement.dataset.show_child = 'false';
                }

                if (listElement?.classList && listElement.classList.contains(styles.on)) {
                    listElement.classList.remove(styles.on);
                }
            }
        }
    }

    moveToX() {
        this.props.moveToX(SDMSMainMenu.Menu_MoveTo_Floor, [this.props.zone.buildingID, SDMSDataManager.getZoneFloor(this.props.zone)]);
    }

    moveToSensor(sensorType, sensorID) {
        this.props.moveToX(SDMSMainMenu.Menu_MoveTo_POI, [this.props.zone.id, sensorType, sensorID]);
    }

    onSelectSensor(sensorType, sensorID) {
        this.props.onSelectSensor(sensorType, this.props.zone.id, sensorID);
    }

    isAlarmSensor(facilityType, sensorID) {
        let alarmImgID = styles.lightGrayICO;
        let alarmImgSrc = imgGrayLightIco;
        
        if (this.props.sensorAlarms) {
            for (let j = 0; j < this.props.sensorAlarms.length; j++) {
                const alarm = this.props.sensorAlarms[j];
                if (!alarm.isAlarm) {
                    // 알람 발생한 센서는 상단에 있기 때문에 isAlarm=false가 나온 시점 이후에는 다 false만 있음
                    break;
                }
                if (alarm.facilityType === facilityType && alarm.orgSensorID === sensorID && alarm.isAlarm) {
                    alarmImgID = styles.lightRedICO;
                    alarmImgSrc = imgRedLightIco;
                    break;
                }
            }
        }
        
        return [alarmImgID, alarmImgSrc];
    }

    getSensorUI() {
        let fireSensorUI = [];
        let coSensorUI = [];
        let o2SensorUI = [];
        let h2SensorUI = [];
        let ch4SensorUI = [];
        let detectSensorUI = [];
        //let psmSensorUI = [];
        //let etcSensorUI = [];
        let cctvUI = [];
        let facilityInfosUI = [];

        const [sensorType, zoneID, sensorID] = this.props.selectedSensor;

        //const sensorList = this.props.sensorList;
        //if (sensorList === undefined || sensorList === null)
        //    return ui;
        
        if (this.props.fireSensors) {
            for (let i = 0; i < this.props.fireSensors.length; i++) {
                const sensor = this.props.fireSensors[i];
                const sensorClassName = sensorType === "fire" && sensorID === sensor.id ? styles.viewList5DepthTxt + " " + styles.selected : styles.viewList5DepthTxt;

                if (sensor.zoneID === this.props.zone.id) {
                    if (this.props.isEditMode) {
                        fireSensorUI.push(
                            <li key={'fireSensor_' + sensor.id}>
                                <span className={sensorClassName}>{sensor.name}</span>
                            </li>
                        );
                    }
                    else {
                        const [alarmImgID, alarmImgSrc] = this.isAlarmSensor(0, sensor.id); // 알람이 발생한 센서인가 ?

                        if (this.props.hasIndoorModel) {
                            fireSensorUI.push(
                                <li key={'fireSensor_' + sensor.id}>
                                    <span className={sensorClassName} onClick={() => this.moveToSensor(SDMSMainMenu.Fire_Sensor, sensor.id)}>{sensor.name}</span>
                                    <div className={styles.floatR + ' ' + styles.posiRelative + " " + styles.flexBox}>
                                        <span className={styles.goLink} onClick={() => this.moveToSensor(SDMSMainMenu.Fire_Sensor, sensor.id)}><a>이동</a></span>
                                        <div className={styles.iconHorizontal}>
                                            <img className={styles.alarmImg} id={alarmImgID} src={alarmImgSrc} />
                                            <span className={styles.greenDOTT}></span>
                                        </div>
                                    </div>
                                </li>
                            );
                        }
                        else {
                            fireSensorUI.push(
                                <li key={'fireSensor_' + sensor.id}>
                                    <span className={sensorClassName}>{sensor.name}</span>
                                    <div className={styles.floatR + ' ' + styles.posiRelative + " " + styles.flexBox}>
                                        <span><a/></span>
                                        <div className={styles.iconHorizontal}>
                                            <img className={styles.alarmImg} id={alarmImgID} src={alarmImgSrc} />
                                            <span className={styles.greenDOTT}></span>
                                        </div>
                                    </div>
                                </li>
                            );
                        }
                    }
                }
            }
        }
        
        if (this.props.coSensorUI) {
            for (let i = 0; i < this.props.coSensors.length; i++) {
                const sensor = this.props.coSensors[i];
                const sensorClassName = sensorType === "co" && sensorID === sensor.id ? styles.viewList5DepthTxt + " " + styles.selected : styles.viewList5DepthTxt;

                if (!sensor.linkedZones)
                    continue;

                for (let j = 0; j < sensor.linkedZones.length; j++) {
                    if (sensor.linkedZones[j].id === this.props.zone.id && (sensor.visible === true || this.props.searchText === '')) {
                        if (this.props.isEditMode) {
                            coSensorUI.push(
                                <li key={'coSensor_' + sensor.id}>
                                    <span className={sensorClassName}>{sensor.name}</span>
                                </li>
                            );
                        }
                        else {
                            const [alarmImgID, alarmImgSrc] = this.isAlarmSensor(0, sensor.id); // 알람이 발생한 센서인가 ?

                            if (this.props.hasIndoorModel) {
                                coSensorUI.push(
                                    <li key={'coSensor_' + sensor.id}>
                                        <span className={sensorClassName} onClick={() => this.moveToSensor(SDMSMainMenu.PSM_Sensor, sensor.id)}>{sensor.name}</span>
                                        <div className={styles.floatR + ' ' + styles.posiRelative + " " + styles.flexBox}>
                                            <span className={styles.goLink} onClick={() => this.moveToSensor(SDMSMainMenu.PSM_Sensor, sensor.id)}><a>이동</a></span>
                                            <div className={styles.iconHorizontal}>
                                                <img className={styles.alarmImg} id={alarmImgID} src={alarmImgSrc} />
                                                <span className={styles.greenDOTT}></span>
                                            </div>
                                        </div>
                                    </li>
                                );
                            } else {
                                coSensorUI.push(
                                    <li key={'coSensor_' + sensor.id}>
                                        <span className={sensorClassName}>{sensor.name}</span>
                                        <div className={styles.floatR + ' ' + styles.posiRelative + " " + styles.flexBox}>
                                            <span><a /></span>
                                            <div className={styles.iconHorizontal}>
                                                <img className={styles.alarmImg} id={alarmImgID} src={alarmImgSrc} />
                                                <span className={styles.greenDOTT}></span>
                                            </div>
                                        </div>
                                    </li>
                                );
                            }
                            
                        }
                    }
                }
            }
        }

        if (this.props.o2SensorUI) {
            for (let i = 0; i < this.props.coSensors.length; i++) {
                const sensor = this.props.coSensors[i];
                const sensorClassName = sensorType === "o2" && sensorID === sensor.id ? styles.viewList5DepthTxt + " " + styles.selected : styles.viewList5DepthTxt;

                if (!sensor.linkedZones)
                    continue;

                for (let j = 0; j < sensor.linkedZones.length; j++) {
                    if (sensor.linkedZones[j].id === this.props.zone.id && (sensor.visible === true || this.props.searchText === '')) {
                        if (this.props.isEditMode) {
                            o2SensorUI.push(
                                <li key={'o2Sensor_' + sensor.id}>
                                    <span className={sensorClassName}>{sensor.name}</span>
                                </li>
                            );
                        }
                        else {
                            const [alarmImgID, alarmImgSrc] = this.isAlarmSensor(0, sensor.id); // 알람이 발생한 센서인가 ?

                            if (this.props.hasIndoorModel) {
                                o2SensorUI.push(
                                    <li key={'o2Sensor_' + sensor.id}>
                                        <span className={sensorClassName} onClick={() => this.moveToSensor(SDMSMainMenu.PSM_Sensor, sensor.id)}>{sensor.name}</span>
                                        <div className={styles.floatR + ' ' + styles.posiRelative + " " + styles.flexBox}>
                                            <span className={styles.goLink} onClick={() => this.moveToSensor(SDMSMainMenu.PSM_Sensor, sensor.id)}><a>이동</a></span>
                                            <div className={styles.iconHorizontal}>
                                                <img className={styles.alarmImg} id={alarmImgID} src={alarmImgSrc} />
                                                <span className={styles.greenDOTT}></span>
                                            </div>
                                        </div>
                                    </li>
                                );
                            } else {
                                o2SensorUI.push(
                                    <li key={'o2Sensor_' + sensor.id}>
                                        <span className={sensorClassName}>{sensor.name}</span>
                                        <div className={styles.floatR + ' ' + styles.posiRelative + " " + styles.flexBox}>
                                            <span><a /></span>
                                            <div className={styles.iconHorizontal}>
                                                <img className={styles.alarmImg} id={alarmImgID} src={alarmImgSrc} />
                                                <span className={styles.greenDOTT}></span>
                                            </div>
                                        </div>
                                    </li>
                                );
                            }

                        }
                    }
                }
            }
        }

        if (this.props.h2SensorUI) {
            for (let i = 0; i < this.props.h2Sensors.length; i++) {
                const sensor = this.props.h2Sensors[i];
                const sensorClassName = sensorType === "h2" && sensorID === sensor.id ? styles.viewList5DepthTxt + " " + styles.selected : styles.viewList5DepthTxt;

                if (!sensor.linkedZones)
                    continue;

                for (let j = 0; j < sensor.linkedZones.length; j++) {
                    if (sensor.linkedZones[j].id === this.props.zone.id && (sensor.visible === true || this.props.searchText === '')) {
                        if (this.props.isEditMode) {
                            h2SensorUI.push(
                                <li key={'h2Sensor_' + sensor.id}>
                                    <span className={sensorClassName}>{sensor.name}</span>
                                </li>
                            );
                        }
                        else {
                            const [alarmImgID, alarmImgSrc] = this.isAlarmSensor(0, sensor.id); // 알람이 발생한 센서인가 ?

                            if (this.props.hasIndoorModel) {
                                h2SensorUI.push(
                                    <li key={'coSensor_' + sensor.id}>
                                        <span className={sensorClassName} onClick={() => this.moveToSensor(SDMSMainMenu.PSM_Sensor, sensor.id)}>{sensor.name}</span>
                                        <div className={styles.floatR + ' ' + styles.posiRelative + " " + styles.flexBox}>
                                            <span className={styles.goLink} onClick={() => this.moveToSensor(SDMSMainMenu.PSM_Sensor, sensor.id)}><a>이동</a></span>
                                            <div className={styles.iconHorizontal}>
                                                <img className={styles.alarmImg} id={alarmImgID} src={alarmImgSrc} />
                                                <span className={styles.greenDOTT}></span>
                                            </div>
                                        </div>
                                    </li>
                                );
                            } else {
                                h2SensorUI.push(
                                    <li key={'h2Sensor_' + sensor.id}>
                                        <span className={sensorClassName}>{sensor.name}</span>
                                        <div className={styles.floatR + ' ' + styles.posiRelative + " " + styles.flexBox}>
                                            <span><a /></span>
                                            <div className={styles.iconHorizontal}>
                                                <img className={styles.alarmImg} id={alarmImgID} src={alarmImgSrc} />
                                                <span className={styles.greenDOTT}></span>
                                            </div>
                                        </div>
                                    </li>
                                );
                            }

                        }
                    }
                }
            }
        }

        if (this.props.ch4SensorUI) {
            for (let i = 0; i < this.props.coSensors.length; i++) {
                const sensor = this.props.coSensors[i];
                const sensorClassName = sensorType === "ch4" && sensorID === sensor.id ? styles.viewList5DepthTxt + " " + styles.selected : styles.viewList5DepthTxt;

                if (!sensor.linkedZones)
                    continue;

                for (let j = 0; j < sensor.linkedZones.length; j++) {
                    if (sensor.linkedZones[j].id === this.props.zone.id && (sensor.visible === true || this.props.searchText === '')) {
                        if (this.props.isEditMode) {
                            ch4SensorUI.push(
                                <li key={'ch4Sensor_' + sensor.id}>
                                    <span className={sensorClassName}>{sensor.name}</span>
                                </li>
                            );
                        }
                        else {
                            const [alarmImgID, alarmImgSrc] = this.isAlarmSensor(0, sensor.id); // 알람이 발생한 센서인가 ?

                            if (this.props.hasIndoorModel) {
                                ch4SensorUI.push(
                                    <li key={'ch4Sensor_' + sensor.id}>
                                        <span className={sensorClassName} onClick={() => this.moveToSensor(SDMSMainMenu.PSM_Sensor, sensor.id)}>{sensor.name}</span>
                                        <div className={styles.floatR + ' ' + styles.posiRelative + " " + styles.flexBox}>
                                            <span className={styles.goLink} onClick={() => this.moveToSensor(SDMSMainMenu.PSM_Sensor, sensor.id)}><a>이동</a></span>
                                            <div className={styles.iconHorizontal}>
                                                <img className={styles.alarmImg} id={alarmImgID} src={alarmImgSrc} />
                                                <span className={styles.greenDOTT}></span>
                                            </div>
                                        </div>
                                    </li>
                                );
                            } else {
                                ch4SensorUI.push(
                                    <li key={'ch4Sensor_' + sensor.id}>
                                        <span className={sensorClassName}>{sensor.name}</span>
                                        <div className={styles.floatR + ' ' + styles.posiRelative + " " + styles.flexBox}>
                                            <span><a /></span>
                                            <div className={styles.iconHorizontal}>
                                                <img className={styles.alarmImg} id={alarmImgID} src={alarmImgSrc} />
                                                <span className={styles.greenDOTT}></span>
                                            </div>
                                        </div>
                                    </li>
                                );
                            }

                        }
                    }
                }
            }
        }

        if (this.props.detectSensorUI) {
            for (let i = 0; i < this.props.coSensors.length; i++) {
                const sensor = this.props.coSensors[i];
                const sensorClassName = sensorType === "detect" && sensorID === sensor.id ? styles.viewList5DepthTxt + " " + styles.selected : styles.viewList5DepthTxt;

                if (!sensor.linkedZones)
                    continue;

                for (let j = 0; j < sensor.linkedZones.length; j++) {
                    if (sensor.linkedZones[j].id === this.props.zone.id && (sensor.visible === true || this.props.searchText === '')) {
                        if (this.props.isEditMode) {
                            detectSensorUI.push(
                                <li key={'detectSensor_' + sensor.id}>
                                    <span className={sensorClassName}>{sensor.name}</span>
                                </li>
                            );
                        }
                        else {
                            const [alarmImgID, alarmImgSrc] = this.isAlarmSensor(0, sensor.id); // 알람이 발생한 센서인가 ?

                            if (this.props.hasIndoorModel) {
                                detectSensorUI.push(
                                    <li key={'detectSensor_' + sensor.id}>
                                        <span className={sensorClassName} onClick={() => this.moveToSensor(SDMSMainMenu.PSM_Sensor, sensor.id)}>{sensor.name}</span>
                                        <div className={styles.floatR + ' ' + styles.posiRelative + " " + styles.flexBox}>
                                            <span className={styles.goLink} onClick={() => this.moveToSensor(SDMSMainMenu.PSM_Sensor, sensor.id)}><a>이동</a></span>
                                            <div className={styles.iconHorizontal}>
                                                <img className={styles.alarmImg} id={alarmImgID} src={alarmImgSrc} />
                                                <span className={styles.greenDOTT}></span>
                                            </div>
                                        </div>
                                    </li>
                                );
                            } else {
                                detectSensorUI.push(
                                    <li key={'detectSensor_' + sensor.id}>
                                        <span className={sensorClassName}>{sensor.name}</span>
                                        <div className={styles.floatR + ' ' + styles.posiRelative + " " + styles.flexBox}>
                                            <span><a /></span>
                                            <div className={styles.iconHorizontal}>
                                                <img className={styles.alarmImg} id={alarmImgID} src={alarmImgSrc} />
                                                <span className={styles.greenDOTT}></span>
                                            </div>
                                        </div>
                                    </li>
                                );
                            }

                        }
                    }
                }
            }
        }

        if (this.props.cctvs) {
            for (let i = 0; i < this.props.cctvs.length; i++) {
                const sensor = this.props.cctvs[i];
                const sensorClassName = sensorType === "cctv" && sensorID === sensor.id ? styles.viewList5DepthTxt + " " + styles.selected : styles.viewList5DepthTxt;

                if (sensor.zoneID === this.props.zone.id) {
                    cctvUI.push(
                        <li key={'cctv_' + sensor.id}>
                            <span className={sensorClassName} style={{ width: '147px' }} onClick={() => this.moveToSensor(SDMSMainMenu.CCTV_Type, sensor.id)}>{sensor.name}</span>
                            {
                                (this.props.isEditMode === false || this.props.hasIndoorModel) &&
                                <span className={styles.goLink} onClick={() => this.moveToSensor(SDMSMainMenu.CCTV_Type, sensor.id)}>
                                    <a>이동</a>
                                </span>
                            }
                        </li>
                    );
                }
            }
        }

        if (this.props.facilityInfos) {
            for (let i = 0; i < this.props.facilityInfos.length; i++) {
                const info = this.props.facilityInfos[i];
                if (info.zoneID === this.props.zone.id) {
                    facilityInfosUI.push(
                        <li key={'facilityInfo_' + info.id}>
                            <span className={styles.viewList5DepthTxt} style={{ width: '147px' }}>{info.facilityName}</span>
                            
                        </li>
                    );
                }
            }
        }

        return [fireSensorUI, coSensorUI, o2SensorUI, h2SensorUI, ch4SensorUI, detectSensorUI, cctvUI, facilityInfosUI];
        //return [fireSensorUI, psmSensorUI, etcSensorUI, cctvUI, facilityInfosUI];
    }

    showChild(e) {
        const expand = this.props.showChild(e);

        if (e.target === this.refZoneName.current) {
            this.manualZoneNameExpand = expand;
            if (this.manualZoneNameExpand && this.props.onChangeBuildingGroup) {
                this.props.onChangeBuildingGroup(this.props.zone, 'zone');
            }
        }
        else if (e.target === this.refSensors.current) {
            this.manualSensorsExpand = expand;
            if (this.manualSensorsExpand && this.props.onChangeBuildingGroup) {
                this.props.onChangeBuildingGroup('sensorGroups', 'sensorGroups');
            }
        }
        else if (e.target === this.refFireSensors.current) {
            this.manualFireSensorsExpand = expand;
            if (this.manualFireSensorsExpand && this.props.onChangeBuildingGroup) {
                this.props.onChangeBuildingGroup('fireSensors', 'fireSensors');
            }
        }
        else if (e.target === this.refCoSensors.current) {
            this.manualCoSensorsExpand = expand;
            if (this.manualCoSensorsExpand && this.props.onChangeBuildingGroup) {
                this.props.onChangeBuildingGroup('coSensors', 'coSensors');
            }
        }
        else if (e.target === this.refO2Sensors.current) {
            this.manualO2SensorsExpand = expand;
            if (this.manualO2SensorsExpand && this.props.onChangeBuildingGroup) {
                this.props.onChangeBuildingGroup('o2Sensors', 'o2Sensors');
            }
        }
        else if (e.target === this.refH2Sensors.current) {
            this.manualH2SensorsExpand = expand;
            if (this.manualH2SensorsExpand && this.props.onChangeBuildingGroup) {
                this.props.onChangeBuildingGroup('h2Sensors', 'h2Sensors');
            }
        }
        else if (e.target === this.refCh4Sensors.current) {
            this.manualCh4SensorsExpand = expand;
            if (this.manualCh4SensorsExpand && this.props.onChangeBuildingGroup) {
                this.props.onChangeBuildingGroup('ch4Sensors', 'ch4Sensors');
            }
        }
        else if (e.target === this.refDetectSensors.current) {
            this.manualDetectSensorsExpand = expand;
            if (this.manualDetectSensorsExpand && this.props.onChangeBuildingGroup) {
                this.props.onChangeBuildingGroup('detectSensors', 'detectSensors');
            }
        }
        /*else if (e.target === this.refPsmSensors.current) {
            this.manualPsmSensorsExpand = expand;
            if (this.manualPsmSensorsExpand && this.props.onChangeBuildingGroup) {
                this.props.onChangeBuildingGroup('psmSensors', 'psmSensors');
            }
        }
        else if (e.target === this.refEtcSensors.current) {
            this.manualEtcSensorsExpand = expand;
            if (this.manualEtcSensorsExpand && this.props.onChangeBuildingGroup) {
                this.props.onChangeBuildingGroup('etcSensors', 'etcSensors');
            }
        }*/
        else if (e.target === this.refCCTVGroups.current) {
            this.manualCCTVGroupsExpand = expand;
            if (this.manualCCTVGroupsExpand && this.props.onChangeBuildingGroup) {
                this.props.onChangeBuildingGroup('cctvGroups', 'cctvGroups');
            }
        }
        else if (e.target === this.refCCTVSubGroups.current) {
            this.manualCCTVSubGroupsExpand = expand;
            if (this.manualCCTVSubGroupsExpand && this.props.onChangeBuildingGroup) {
                this.props.onChangeBuildingGroup('cctvSubGroups', 'cctvSubGroups');
            }
        }
    }

    isSelected() {
        let zoneShowChild = 'false';
        let sensorsShowChild = 'false';
        let fireSensorsShowChild = 'false';
        let coSensorsShowChild = 'false';
        let o2SensorsShowChild = 'false';
        let h2SensorsShowChild = 'false';
        let ch4SensorsShowChild = 'false';
        let detectSensorsShowChild = 'false';
        //let psmSensorsShowChild = 'false';
        //let etcSensorsShowChild = 'false';
        let cctvGroupsShowChild = 'false';
        let cctvSubGroupsShowChild = 'false';

        const [sensorType, zoneID, sensorID] = this.props.selectedSensor;

        if (this.prevSelectedSensor[0] !== sensorType ||
            this.prevSelectedSensor[1] !== zoneID ||
            this.prevSelectedSensor[2] !== sensorID) {
            this.manualZoneNameExpand = null;
            this.manualSensorsExpand = null;
            this.manualFireSensorsExpand = null;
            this.manualCoSensorsExpand = null;
            this.manualO2SensorsExpand = null;
            this.manualH2SensorsExpand = null;
            this.manualCh4SensorsExpand = null;
            this.manualDetectSensorsExpand = null;
            //this.manualPsmSensorsExpand = null;
            //this.manualEtcSensorsExpand = null;
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
                    else if (sensorType === SDMSMainMenu.CO_Sensor) {
                        coSensorsShowChild = 'true';
                    }
                    else if (sensorType === SDMSMainMenu.O2_Sensor) {
                        o2SensorsShowChild = 'true';
                    }
                    else if (sensorType === SDMSMainMenu.H2_Sensor) {
                        h2SensorsShowChild = 'true';
                    }
                    else if (sensorType === SDMSMainMenu.CH4_Sensor) {
                        ch4SensorsShowChild = 'true';
                    }
                    else if (sensorType === SDMSMainMenu.Detect_Sensor) {
                        detectSensorsShowChild = 'true';
                    }
                    //else if (sensorType === "psm") {
                    //    psmSensorsShowChild = 'true';
                    //}
                    //else if (sensorType === "etc") {
                    //    etcSensorsShowChild = 'true';
                    //}
                }
            }
        }
        else {
            if (this.props.selectedInfo) {
                if (this.props.zone === this.props.selectedInfo.zone) {
                    zoneShowChild = 'true';

                    if (this.props.selectedInfo.sensorGroups) {
                        sensorsShowChild = 'true';
                    }
                    if (this.props.selectedInfo.fireSensors) {
                        fireSensorsShowChild = 'true';
                    }
                    if (this.props.selectedInfo.coSensors) {
                        coSensorsShowChild = 'true';
                    }
                    if (this.props.selectedInfo.o2Sensors) {
                        o2SensorsShowChild = 'true';
                    }
                    if (this.props.selectedInfo.h2Sensors) {
                        h2SensorsShowChild = 'true';
                    }
                    if (this.props.selectedInfo.ch4Sensors) {
                        ch4SensorsShowChild = 'true';
                    }
                    if (this.props.selectedInfo.detectSensors) {
                        detectSensorsShowChild = 'true';
                    }
                    //if (this.props.selectedInfo.psmSensors) {
                    //    psmSensorsShowChild = 'true';
                    //}
                    //if (this.props.selectedInfo.etcSensors) {
                    //    etcSensorsShowChild = 'true';
                    //}
                    if (this.props.selectedInfo.cctvGroups) {
                        cctvGroupsShowChild = 'true';
                    }
                    if (this.props.selectedInfo.cctvSubGroups) {
                        cctvSubGroupsShowChild = 'true';
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

                if (this.manualCoSensorsExpand !== null) {
                    coSensorsShowChild = this.manualCoSensorsExpand ? 'true' : 'false';
                }

                if (this.manualO2SensorsExpand !== null) {
                    o2SensorsShowChild = this.manualO2SensorsExpand ? 'true' : 'false';
                }

                if (this.manualH2SensorsExpand !== null) {
                    h2SensorsShowChild = this.manualH2SensorsExpand ? 'true' : 'false';
                }

                if (this.manualCh4SensorsExpand !== null) {
                    ch4SensorsShowChild = this.manualCh4SensorsExpand ? 'true' : 'false';
                }

                if (this.manualDetectSensorsExpand !== null) {
                    detectSensorsShowChild = this.manualDetectSensorsExpand ? 'true' : 'false';
                }

                //if (this.manualPsmSensorsExpand !== null) {
                //    psmSensorsShowChild = this.manualPsmSensorsExpand ? 'true' : 'false';
                //}
        
                //if (this.manualEtcSensorsExpand !== null) {
                //    etcSensorsShowChild = this.manualEtcSensorsExpand ? 'true' : 'false';
                //}

                if (this.manualCCTVGroupsExpand !== null) {
                    cctvGroupsShowChild = this.manualCCTVGroupsExpand ? 'true' : 'false';
                }

                if (this.manualCCTVSubGroupsExpand !== null) {
                    cctvSubGroupsShowChild = this.manualCCTVSubGroupsExpand ? 'true' : 'false';
                }
            }
        }

        return [zoneShowChild, sensorsShowChild, fireSensorsShowChild, coSensorsShowChild, o2SensorsShowChild, h2SensorsShowChild, ch4SensorsShowChild, detectSensorsShowChild, cctvGroupsShowChild, cctvSubGroupsShowChild];
    }

    render() {
        let [fireSensorUI, coSensorUI, o2SensorUI, h2SensorUI, ch4SensorUI, detectSensorUI, cctvUI, facilityInfosUI] = this.getSensorUI();
        //let [fireSensorUI, psmSensorUI, etcSensorUI, cctvUI, facilityInfosUI] = this.getSensorUI();
        const [zoneShowChild, sensorsShowChild, fireSensorsShowChild, coSensorsShowChild, o2SensorsShowChild, h2SensorsShowChild, ch4SensorsShowChild, detectSensorsShowChild, cctvGroupsShowChild, cctvSubGroupsShowChild] = this.isSelected();
        //const [zoneShowChild, sensorsShowChild, fireSensorsShowChild, psmSensorsShowChild, etcSensorsShowChild, cctvGroupsShowChild, cctvSubGroupsShowChild] = this.isSelected();
        this.showZoneNameResult = zoneShowChild === 'true';
        this.showSensorsResult = sensorsShowChild === 'true';
        this.showFireSensorsResult = fireSensorsShowChild === 'true';
        this.showCoSensorsResult = coSensorsShowChild === 'true';
        this.showO2SensorsResult = o2SensorsShowChild === 'true';
        this.showH2SensorsResult = h2SensorsShowChild === 'true';
        this.showCH4SensorsResult = ch4SensorsShowChild === 'true';
        this.showDetectSensorsResult = detectSensorsShowChild === 'true';
        //this.showPsmSensorsResult = psmSensorsShowChild === 'true';
        //this.showEtcSensorsResult = etcSensorsShowChild === 'true';
        this.showCCTVGroupsResult = cctvGroupsShowChild === 'true';
        this.showCCTVSubGroupsResult = cctvSubGroupsShowChild === 'true';

        const zoneName = this.props.zone.displayText ? this.props.zone.displayText : this.props.zone.name;

        return (            
            <li>
                <div className={styles.viewList2DepthHead}>
                    <span ref={this.refZoneName} className={styles.viewList2DepthSpen} data-show_child={zoneShowChild} data-target_class='viewList2Depth' onClick={(e) => { this.showChild(e) }}>{zoneName}</span>
                    {
                        (this.props.hasIndoorModel) ? <span className={styles.goLink2} onClick={this.moveToX}><a>이동</a></span> : <></>
                    }
                </div>
                {
                    this.props.sensorList && !ProjectResource.isModelViewer &&
                    <ul ref={this.refZoneNameList} className={zoneShowChild === 'true' ? styles.viewList3Depth + " " + styles.on : styles.viewList3Depth}>
                        <li>
                            <div ref={this.refSensors} className={styles.viewList3DepthHead} data-show_child={sensorsShowChild} data-target_class='viewList3Depth' onClick={(e) => { this.showChild(e) }}>센서</div>
                            <ul ref={this.refSensorsList} className={sensorsShowChild === 'true' ? styles.viewList4Depth + " " + styles.on : styles.viewList4Depth}>
                                <li>
                                    <span ref={this.refFireSensors} className={styles.viewList4DepthHead} data-show_child={fireSensorsShowChild} data-target_class='viewList4Depth' onClick={(e) => { this.showChild(e) }}>화재센서</span>
                                    <ul ref={this.refFireSensorsList} className={fireSensorsShowChild === 'true' ? styles.viewList5Depth + " " + styles.on : styles.viewList5Depth}>
                                        {fireSensorUI}
                                    </ul>
                                </li>
                                <li>
                                    <span ref={this.refCoSensors} className={styles.viewList4DepthHead} data-show_child={coSensorsShowChild} data-target_class='viewList4Depth' onClick={(e) => { this.showChild(e) }}>Co센서</span>
                                    <ul ref={this.refCoSensorsList} className={coSensorsShowChild === 'true' ? styles.viewList5Depth + " " + styles.on : styles.viewList5Depth}>
                                        {coSensorUI}
                                    </ul>
                                </li>
                                <li>
                                    <span ref={this.refO2Sensors} className={styles.viewList4DepthHead} data-show_child={o2SensorsShowChild} data-target_class='viewList4Depth' onClick={(e) => { this.showChild(e) }}>o2센서</span>
                                    <ul ref={this.refO2SensorsList} className={o2SensorsShowChild === 'true' ? styles.viewList5Depth + " " + styles.on : styles.viewList5Depth}>
                                        {o2SensorUI}
                                    </ul>
                                </li>
                                <li>
                                    <span ref={this.refH2Sensors} className={styles.viewList4DepthHead} data-show_child={h2SensorsShowChild} data-target_class='viewList4Depth' onClick={(e) => { this.showChild(e) }}>h2센서</span>
                                    <ul ref={this.refH2SensorsList} className={h2SensorsShowChild === 'true' ? styles.viewList5Depth + " " + styles.on : styles.viewList5Depth}>
                                        {h2SensorUI}
                                    </ul>
                                </li>
                                <li>
                                    <span ref={this.refCh4Sensors} className={styles.viewList4DepthHead} data-show_child={ch4SensorsShowChild} data-target_class='viewList4Depth' onClick={(e) => { this.showChild(e) }}>ch4센서</span>
                                    <ul ref={this.refCh4SensorsList} className={ch4SensorsShowChild === 'true' ? styles.viewList5Depth + " " + styles.on : styles.viewList5Depth}>
                                        {ch4SensorUI}
                                    </ul>
                                </li>
                                <li>
                                    <span ref={this.refDetectSensors} className={styles.viewList4DepthHead} data-show_child={detectSensorsShowChild} data-target_class='viewList4Depth' onClick={(e) => { this.showChild(e) }}>감지센서</span>
                                    <ul ref={this.refDetectSensorsList} className={detectSensorsShowChild === 'true' ? styles.viewList5Depth + " " + styles.on : styles.viewList5Depth}>
                                        {detectSensorUI}
                                    </ul>
                                </li>
                            </ul>
                        </li>
                        <li>
                            <div ref={this.refCCTVGroups} className={styles.viewList3DepthHead} data-show_child={cctvGroupsShowChild} data-target_class='viewList3Depth' onClick={(e) => { this.showChild(e) }}>CCTV</div>
                            <ul ref={this.refCCTVGroupsList} className={cctvGroupsShowChild === 'true' ? styles.viewList4Depth + " " + styles.on : styles.viewList4Depth}>
                                <li>
                                    <span ref={this.refCCTVSubGroups} className={styles.viewList4DepthHead} data-show_child={cctvSubGroupsShowChild} data-target_class='viewList4Depth' onClick={(e) => { this.showChild(e) }}>CCTV</span>

                                    <ul ref={this.refCCTVSubGroupsList} className={cctvSubGroupsShowChild === 'true' ? styles.viewList5Depth + " " + styles.on : styles.viewList5Depth}>
                                        {cctvUI}
                                    </ul>
                                </li>
                            </ul>
                        </li>
                        <li>
                            <div className={styles.viewList3DepthHead} data-show_child='false' data-target_class='viewList3Depth' onClick={(e) => { this.showChild(e) }}>설비</div>
                            <ul className={styles.viewList4Depth}>
                                <li>
                                    <span className={styles.viewList4DepthHead} data-show_child='false' data-target_class='viewList4Depth' onClick={(e) => { this.showChild(e) }}>설비</span>

                                    <ul className={styles.viewList5Depth}>
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