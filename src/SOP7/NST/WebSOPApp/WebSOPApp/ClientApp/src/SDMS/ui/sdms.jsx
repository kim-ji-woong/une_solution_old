import * as React from 'react';
import styles from '../css/sdms.module.css';
import { SDMSController } from '../services/sdmsController';
import { SDMSDataManager } from '../services/sdmsDataManager';
import SDMSResource from '../resource/id';
import Contents3D from './3D/contents3D';
import SDMSMainMenu from '../data/sdmsMainMenu';
import $ from 'jquery';

import ConfirmDialog from '../../Common/ui/confirmDialog';
import * as Backend from '../data/backend';
import * as Common from '../../Common/data/common';

import StatusInfo from './popups/statusInfo';
import CCTVInfo from './popups/cctvInfo';
import WarningAlarmInfo from './popups/warningAlarmInfo';
import WorkerInfo from './popups/workerInfo';
import SpreadInfo from './popups/spreadInfo';
import DetectionTextInfo from './popups/detectionTextInfo';
import SessionString from '../../Common/js/sessionString';
import * as SizableDragable from './popups/sizableDragable';
import HistoryInfo from './popups/historyPopups/historyInfo';
import POIManager from './3D/poiManager';
import store from '../../Root/store';
import ProjectResource from '../../Root/resource/id';

/*interface Props {
    menuEvent: {
        onClickLogo: () => void,
        handler: (menu: Common.NullableNumber, param: string) => void
    }
}

type PopupLayerType = {
    statusInfoZIndex: number,
    spreadInfoZIndex: number,
    cctvInfoZIndex: number,
    warningAlarmInfoZIndex: number,
    workerInfoZIndex: number
}

interface State {
    loading: boolean,
    _3dOptions: object,
    command:
    {
        menu: Common.NullableNumber,
        menuParameter: any
    },
    currentView: {
        buildingID: Common.NullableNumber,   // null이면 외부영역
        zoneID: Common.NullableNumber,
        zoneName: string
    },
    confirmMessage: {
        visible: boolean,
        title: string,
        messages: Array<string>,
        buttons: Array<string> | null,
        onClose: () => void,
        onClickButton: (index: number) => void
    },
    visiblePopups: {
        statusInfo: boolean,
        cctvInfo: boolean,
        warningAlarmInfo: boolean,
        workerInfo: boolean,
        detectionTextInfo: boolean,
        spreadInfo: boolean
    },
    buildingGroupList: Array<Backend.BuildingGroup>,
    streamServerURL: Common.NullableString,
    popupState: Map<string, SizableDragable.PopupState>,
    popupLayer: PopupLayerType,
    cctvList: Common.NullableString,
    visibleSensorTypes: Map<string, boolean>,
    cctvFullScreenState: {
        isFullScreen: boolean,
        cctvName: string | null,
        url: string | null,
        w: number | null,
        h: number | null
    },
    // sensorType, zoneID, sensorID
    selectedPOI: [string, number, number] | null,
    selectedBuildingGroupInfo: {
        buildingGroup: object | null,
        building: object | null,
        zone: object | null
    }
}*/

class SDMS extends React.Component/*<Props, State>*/ {
    static menu = {
        none: null
    }

    constructor(props/*: Props*/) {
        super(props);

        this.state = {
            loading: true,
            _3dOptions: {},
            sensorList: {},
            sensorAlarms: store.getState().sensorAlarm,
            selectedAlarm: null,
            alarmSound: true,
            command:
            {
                menu: null,
                menuParameter: null
            },
            currentView: {
                buildingID: null,   // null이면 외부영역
                zoneID: null,
                zoneName: ''
            },
            confirmMessage: {
                visible: false,
                title: "",
                messages: [""],
                buttons: ["확인"],
                onClose: this.onCloseConfirmDialog,
                onClickButton: null
            },
            visiblePopups: {
                statusInfo: true,
                cctvInfo: false,
                warningAlarmInfo: true,
                workerInfo: false,
                detectionTextInfo: false,
                historyInfo: false,
                spreadInfo: false
            },
            buildingGroupList: [],
            streamServerURL: null,
            popupState: {},
            popupLayer: {
                statusInfoZIndex: 0,
                spreadInfoZIndex: 0,
                cctvInfoZIndex: -1,
                warningAlarmInfoZIndex: 0,
                workerInfoZIndex: 0,
                historyInfoZIndex: 0
            },
            cctvList: null,
            visibleSensorTypes: this.initVisibleSensorTypes(),
            cctvFullScreenState: {
                isFullScreen: false,
                cctvName: null,
                url: null,
                w: null,
                h: null
            },
            selectedPOI: null,
            selectedBuildingGroupInfo: {
                buildingGroup: null,
                building: null,
                zone: null,
                sensorGroups: null,
                fireSensors: null,
                coSensors: null,
                o2Sensors: null,
                h2Sensors: null,
                ch4Sensors: null,
                detectSensors: null,
                psmSensors: null,
                etcSensors: null,                
                cctvGroups: null,
                cctvSubGroups: null
            },
            workers: {
                zones: {}
            },
            selectWorker: null,
        }

        this.getStreamServerURL();

        this.setPopupState = this.setPopupState.bind(this);
        this.onSelectedAlarm = this.onSelectedAlarm.bind(this);
        this.onClickMalfunction = this.onClickMalfunction.bind(this);
        this.onSound = this.onSound.bind(this);

        store.subscribe(function () {
            const data = store.getState();

            if (data !== null && data !== undefined) {
                if (data.actionType === 'SENSOR_ALARM') {
                    this.changeAlarm(store.getState());
                }

                if (data.actionType === 'MOBILE_USERS') {
                    this.setMobileUsers(store.getState());
                }
            }
        }.bind(this));
    }

    setMobileUsers(storeValue) {
        if (!this.state._3dOptions) {
            return;
        }

        const workers = {
            zones: {}
        };

        if (storeValue.mobileUsers) {
            const userCount = storeValue.mobileUsers.length;

            for (let i = 0; i < userCount; i++) {
                const user = storeValue.mobileUsers[i];

                if (user.zoneID === null || user.zoneID === undefined) {
                    continue;
                }

                let zoneDatas = workers.zones[user.zoneID];

                if (!zoneDatas) {
                    zoneDatas = {};
                    workers.zones[user.zoneID] = zoneDatas;
                }

                zoneDatas[user.id] = user;
            }
        }

        this.setState({ workers });
    }

    componentDidMount() {
        this.props.menuEvent.handler = this.onSelectMenu;
        this.props.menuEvent.onClickLogo = this.onClickLogo;

        // 각 페이지 별로 클래스 초기화
        /*$('#header').addClass(uis.posiHeaderWrap);
        $('#header').removeClass(uis.appHeaderWrap);*/

        this.requestSensorList();

        //팝업 상태값 일괄 획득
        this.getPopupState();
    }

    componentWillUnmount() {
    }

    async changeAlarm(storeValue) {
        const alarms = storeValue.sensorAlarm;

        if (storeValue && storeValue.actionType !== 'SENSOR_ALARM')
            return;

        const orgAlarms = this.state.sensorAlarms;

        var menus = this.state.visiblePopups;

        let selectedAlarm = null;
        if (alarms && alarms.length > 0) {
            selectedAlarm = alarms[0];
        }

        let alarmType = "";
        let alarmCCTV = "";

        if (selectedAlarm) {
            if (selectedAlarm.sensorZoneID < 1000000) {
                menus.warningAlarmInfo = true;

                //alarmType = this.getAlarmTypeFromMessage(selectedAlarm.message);                
                //alarmCCTV = this.showAlarmCCTV(alarmType, selectedAlarm);
            }
        }
        else {
            menus.warningAlarmInfo = false;
            //menus[SDMS.menu.cctv] = false;
        }

        if (selectedAlarm === null || !selectedAlarm.isAlarm) { // 알람 없음
            this.hideAlarm();
            this.onClickLogo();
        }
        else {
            let moveToAlarm = await this.checkAlarm(orgAlarms, alarms, true, alarmCCTV);
            await this.checkAlarm(alarms, orgAlarms, false, alarmCCTV);

            if (moveToAlarm) {
                selectedAlarm = moveToAlarm;
            }
        }

        if (selectedAlarm === null) {
            this.setState({ sensorAlarms: alarms, selectedAlarm: selectedAlarm, visiblePopups: menus, cctvList: null, alarmSound: false });
        }
        else {
            this.setState({ sensorAlarms: alarms, selectedAlarm: selectedAlarm, visiblePopups: menus, alarmSound: selectedAlarm.isAlarm });
        }
    }

    async checkAlarm(alarms, targetAlarms, isChg, targetCCTVMenu) {
        var returnAlarm = [];

        if (!alarms || alarms.length === 0) {
            for (let i = 0; i < targetAlarms.length; i++) {
                if ((isChg && targetAlarms[i].isAlarm) || (!isChg && !targetAlarms[i].isAlarm)) {
                    returnAlarm.push(targetAlarms[i]);
                }
            }
        }
        else {
            if (targetAlarms) {
                for (let i = 0; i < targetAlarms.length; i++) {
                    if (targetAlarms[i].isAlarm) {
                        let isUpdate = true;
                        for (let j = 0; j < alarms.length; j++) {
                            if (targetAlarms[i].sensorZoneHistoryID === alarms[j].sensorZoneHistoryID) {
                                if (isChg) {
                                    // 알람 발생
                                    // alarms : org alarm
                                    // targetAlarms: new alarm
                                    //if (targetAlarms[i].isAlarm) {
                                    // 같은 Equipzone에 알람이 추가됐나 ?
                                    if (targetAlarms.length - 1 >= j &&
                                        alarms[j].alarmSensorZoneIDs.length < targetAlarms[j].alarmSensorZoneIDs.length) {
                                        isUpdate = true;
                                    }
                                    else {
                                        isUpdate = false;
                                    }
                                }
                                else {
                                    // 알람 해제
                                    // alarms : new alarm
                                    // targetAlarms: org alarm
                                    if (!alarms[j].isAlarm) { // 알람해제 상태인가?
                                        //if (targetAlarms[i].isAlarm) { // 이전에는 알람중 이었나?
                                        isUpdate = true;
                                    }
                                    else {
                                        // 같은 Equipzone에 알람이 해지됐나 ?
                                        if (alarms[j].alarmSensorZoneIDs.length < targetAlarms[i].alarmSensorZoneIDs.length) {
                                            isUpdate = true;
                                        }
                                        else {
                                            // 알람 진행중
                                            isUpdate = false;
                                        }
                                    }
                                }

                                break;
                            }
                        }

                        if (isUpdate) {
                            returnAlarm.push(targetAlarms[i]);
                        }
                    }
                }
            }
        }

        //0 : 현재대로
        //1 : 알람 울릴때마다 화면 이동
        //2 : 첫번째 알람 화면으로 이동
        //3 : 마지막 알람 화면으로 이동        
        const moveToOption = "1";//this.state.moveDisplayAlarm;
        let moveToSensor = new Array();

        for (let k = 0; k < returnAlarm.length; k++) {
            for (let i = 0; i < returnAlarm[k].alarmSensorZoneIDs.length; i++) {
                //const [orgSensorID, isAlarmStatus] = await SDMSController.getOrgSensorID(returnAlarm[k].alarmSensorZoneIDs[i]);

                const sensorZoneID = returnAlarm[k].alarmSensorZoneIDs[i];
                if (sensorZoneID < 1000000) {
                    const sensor = this.getOrgSensor(returnAlarm[k].facilityType, sensorZoneID)
                    if (!sensor) {
                        moveToSensor.push(returnAlarm[k]);
                        continue;
                    }

                    if (isChg) { // 알람 발생
                        if (k == returnAlarm.length - 1) {
                            
                        }

                        this.addAlarm(returnAlarm[k].zoneID, returnAlarm[k].facilityType, sensor.id, returnAlarm[k].alarmDepth, returnAlarm[k].equipZoneID, targetCCTVMenu);
                        
                        if (moveToOption === "1") {
                            moveToSensor.push(returnAlarm[k]);
                        }
                        else if (moveToOption === "2") {
                            if (k === 0) {
                                moveToSensor.push(returnAlarm[k]);
                            }
                        }
                        else if (moveToOption === "3") {
                            if (k == returnAlarm.length - 1) {
                                moveToSensor.push(returnAlarm[k]);
                            }
                        }
                    }
                    else { // 알람 해제
                        this.removeAlarm(returnAlarm[k].facilityType, sensor.id, returnAlarm[k].alarmDepth);
                    }
                }
                else {
                    // 수동 신고
                    if (isChg) {
                        moveToSensor.push(returnAlarm[k]);
                    }
                    else {
                        this.removeAlarm(returnAlarm[k].facilityType, -1, returnAlarm[k].alarmDepth);
                    }
                }
            }
        }

        let selectedAlarm = null;
        if (isChg) {
            // 3D 이동할 알람
            for (let i = 0; i < moveToSensor.length; i++) {
                for (let j = 0; j < moveToSensor[i].alarmSensorZoneIDs.length; j++) {
                    const sensorZoneID = moveToSensor[i].alarmSensorZoneIDs[j];
                    if (sensorZoneID < 1000000) {
                        const sensor = this.getOrgSensor(moveToSensor[i].facilityType, sensorZoneID);
                        if (!sensor) {
                            this.showAlarm(moveToSensor[i], null);
                            continue;
                        }
                        this.moveToSensor(moveToSensor[i].zoneID, moveToSensor[i].facilityType, sensor.id);
                        this.addAlarm(moveToSensor[i].zoneID, moveToSensor[i].facilityType, sensor.id, moveToSensor[i].alarmDepth, moveToSensor[i].equipZoneID, targetCCTVMenu);
                    }
                    else {
                        //const alarmType = this.getAlarmTypeFromMessage(moveToSensor[i].message);
                        //const alarmCCTV = this.showAlarmCCTV(alarmType, moveToSensor[i]);
                        this.showAlarm(moveToSensor[i], null);
                    }
                    selectedAlarm = moveToSensor[i];
                }
            }
        }

        return selectedAlarm;
    }

    getOrgSensor(facilityType, sensorZoneID) {
        if (SDMSResource.isPSMSensorType(facilityType)) {
            if (this.state.sensorList.psmSensors) {
                const sensorLength = this.state.sensorList.psmSensors.length;
                for (let i = 0; i < sensorLength; i++) {
                    const sensor = this.state.sensorList.psmSensors[i];
                    if (sensor.sensorZoneID === sensorZoneID) {
                        return sensor;
                    }
                }
            }
        }
        else if (SDMSResource.isETCSensorType(facilityType)) {
            if (this.state.sensorList.etcSensors) {
                const sensorLength = this.state.sensorList.etcSensors.length;
                for (let i = 0; i < sensorLength; i++) {
                    const sensor = this.state.sensorList.etcSensors[i];
                    if (sensor.sensorZoneID === sensorZoneID) {
                        return sensor;
                    }
                }
            }
        }
        else {
            if (this.state.sensorList.fireSensors) {
                const sensorLength = this.state.sensorList.fireSensors.length;
                for (let i = 0; i < sensorLength; i++) {
                    const sensor = this.state.sensorList.fireSensors[i];
                    if (sensor.sensorZoneID === sensorZoneID) {
                        return sensor;
                    }
                }
            }
        }

        return null;
    }

    getFacilityType(facilityType) {
        let sensorType = SDMSMainMenu.Fire_Sensor;

        if ((facilityType >= SDMSResource.facilityType.FIREWALL && facilityType <= SDMSResource.facilityType.ETC) ||
            (facilityType >= SDMSResource.facilityType.Temp && facilityType <= SDMSResource.facilityType.BLE_Count) ||
            (facilityType >= SDMSResource.facilityType.O2 && facilityType <= SDMSResource.facilityType.Relay)) {
            sensorType = SDMSMainMenu.Etc_Sensor;
        } else if (facilityType === SDMSResource.facilityType.PSM_SENSOR ||
            facilityType === SDMSResource.facilityType.HF ||
            facilityType === SDMSResource.facilityType.CO ||
            (facilityType >= SDMSResource.facilityType.HCL && facilityType <= SDMSResource.facilityType.H2S)) {
            sensorType = SDMSMainMenu.PSM_Sensor;
        }

        return sensorType;
    }

    addAlarm(zoneID, facilityType, orgSensorID, alarmDepth, equipZoneID, targetCCTVMenu) {
        var sensorType = this.getFacilityType(facilityType);

        this.onSelectMenu(SDMSMainMenu.Menu_Add_Alarm, [zoneID, sensorType, orgSensorID, alarmDepth]);
        //this.getEquipZoneCCTV(equipZoneID, targetCCTVMenu);
    }

    moveToSensor(zoneID, facilityType, orgSensorID) {
        var sensorType = this.getFacilityType(facilityType);
        this.onSelectMenu(SDMSMainMenu.Menu_MoveTo_POI, [zoneID, sensorType, orgSensorID]);
    }

    showAlarm(alarm, targetCCTVMenu) {
        const [zoneID, sensorType, orgSensorID, alarmDepth, isAlarm] = this.getAlarmInfo(alarm);
        this.onSelectMenu(SDMSMainMenu.Menu_Show_Alarm, [zoneID, sensorType, orgSensorID, alarmDepth, isAlarm]);
        if (alarm.sensorZoneID < 1000000) {

            //this.getEquipZoneCCTV(alarm.equipZoneID, targetCCTVMenu);
        }
    }

    hideAlarm() {
        this.onSelectMenu(SDMSMainMenu.Menu_Hide_Alarm);
    }

    removeAlarm(facilityType, orgSensorID, alarmDepth) {
        var sensorType = this.getFacilityType(facilityType);
        this.onSelectMenu(SDMSMainMenu.Menu_Remove_Alarm, [sensorType, orgSensorID, alarmDepth]);
    }

    getAlarmInfo(alarm) {
        var sensorType = this.getFacilityType(alarm.facilityType);
        return [alarm.zoneID, sensorType, alarm.orgSensorID, alarm.alarmDepth, alarm.isAlarm];
    }

    /*
    async getEquipZoneCCTV(equipZoneID, targetCCTVMenu) {
        let cctvList = "";

        // EquipZoneCCTV LIST 조회
        const [success, result] = await SDMSController.getEquipZoneCCTV(equipZoneID);

        if (success === null || success === undefined || success === false) {
            if (!targetCCTVMenu || targetCCTVMenu === SDMS.menu.cctv) {
                this.state.cctvList = cctvList;
            }

            if (targetCCTVMenu && targetCCTVMenu.length > 0) {
                this.alarmCCTVs[targetCCTVMenu] = cctvList;
            }
            //this.setState({ cctvList: cctvList });
            return;
        }

        if (result.cctV1 !== null && result.cctV1 !== undefined) {
            cctvList = this.addCCTVList(cctvList, result.cctV1);
        }
        if (result.cctV2 !== null && result.cctV2 !== undefined) {
            cctvList = this.addCCTVList(cctvList, result.cctV2);
        }
        if (result.cctV3 !== null && result.cctV3 !== undefined) {
            cctvList = this.addCCTVList(cctvList, result.cctV3);
        }
        if (result.cctV4 !== null && result.cctV4 !== undefined) {
            cctvList = this.addCCTVList(cctvList, result.cctV4);
        }
        if (result.cctV5 !== null && result.cctV5 !== undefined) {
            cctvList = this.addCCTVList(cctvList, result.cctV5);
        }
        if (result.cctV6 !== null && result.cctV6 !== undefined) {
            cctvList = this.addCCTVList(cctvList, result.cctV6);
        }

        if (!targetCCTVMenu || targetCCTVMenu === SDMS.menu.cctv) {
            this.state.cctvList = cctvList;
        }

        if (targetCCTVMenu && targetCCTVMenu.length > 0) {
            this.alarmCCTVs[targetCCTVMenu] = cctvList;
        }
        //this.setState({ cctvList: cctvList });
    }
    */

    getAlarmTypeFromMessage(message) {
        const index = message.indexOf("에서");

        if (index < 0) {
            return "";
        }

        const index2 = message.lastIndexOf("탐지");

        if (index2 < 0) {
            return "";
        }

        let alarmType = message.substring(index + 2, index2).trim();

        if (alarmType.endsWith("이") || alarmType.endsWith("가")) {
            alarmType = alarmType.substring(0, alarmType.length - 1);
        }

        return alarmType;
    }

    initVisibleSensorTypes() {
        const visibleSensorTypes = {};

        visibleSensorTypes[SDMSMainMenu.Detect_Sensor] = true;
        visibleSensorTypes[SDMSMainMenu.O2_Sensor] = true;
        visibleSensorTypes[SDMSMainMenu.H2_Sensor] = true;
        visibleSensorTypes[SDMSMainMenu.CCTV_Type] = true;
        visibleSensorTypes[SDMSMainMenu.CO_Sensor] = true;
        visibleSensorTypes[SDMSMainMenu.CH4_Sensor] = true;
        visibleSensorTypes[SDMSMainMenu.Worker_Type] = true;

        return visibleSensorTypes;
    }

    // SDMS 컴포넌트 마운트 시, 저장된 위치 값 호출
    async getPopupState() {
        if (ProjectResource.isModelViewer) {
            return;
        }

        const key = SessionString.Key;
        const account = SessionString.Key.account;
        // 세션에서 DB의 유저 key값 획득, 전체 팝업 좌표를 호출한다.
        const userInfo = JSON.parse(window.localStorage.getItem(SessionString.Key.account));

        if (!userInfo) {
            return;
        }

        const result = await SDMSController.requestGetOption(userInfo.id, 'popup');

        const success = result[0];
        const datas = result[1];

        /*
         * propertyValue1 - x좌표 (pos)
         * propertyValue2 - y좌표 (pos)
         * propertyValue3 - height (size)
         * propertyValue4 - width (size)
        */
        if (success && datas != null) {
            const popupState = {};
            //const popupState: Map<string, SizableDragable.PopupState> = new Map<string, SizableDragable.PopupState>();

            for (let i = 0; i < datas.length; i++) {
                const data = datas[i]/* as Backend.AccountOption*/;

                if (data.subCategory !== null) {
                    popupState[data.subCategory] = {
                        id: data.id,
                        x: data.propertyValue1,
                        y: data.propertyValue2,
                        height: data.propertyValue3,
                        width: data.propertyValue4
                    };
                    //popupState.set(data.subCategory, {
                    //    id: data.id,
                    //    x: data.propertyValue1/* as string*/,
                    //    y: data.propertyValue2/* as string*/,
                    //    height: data.propertyValue3/* as string*/,
                    //    width: data.propertyValue4/* as string*/
                    //});
                }
            }
            this.setState({ popupState: popupState });
        }
    }

    async getStreamServerURL() {
        const streamServerURL = await SDMSController.getStreamServerURL();

        if (streamServerURL !== null || streamServerURL !== undefined)
            this.setState({ streamServerURL: streamServerURL });
    }

    async requestSensorList() {
        const [result, message] = await SDMSController.requestSensorList();

        if (result === null) {
            console.log(message);
        }
        else {
            var sensorList = {};
            if (result.fireSensors) {
                sensorList['fireSensors'] = result.fireSensors;
            }

            if (result.psmSensors) {
                POIManager.setSensors(sensorList, result.psmSensors);
                //sensorList['psmSensors'] = result.psmSensors;
            }

            if (result.etcSensors) {
                POIManager.setSensors(sensorList, result.etcSensors);
                //sensorList['etcSensors'] = result.etcSensors;
            }

            if (result.cctvs) {
                sensorList['cctvs'] = result.cctvs;
            }

            this.setState({ sensorList: sensorList });
        }

        await this.set3DOptions(sensorList);
    }

    async set3DOptions(sensorList) {
        const [buildingGroupList, outdoorZones, errorMessage] = await SDMSController.requestBuildingGroupList();
        const _3dOptions = await SDMSDataManager.get3DOptions(buildingGroupList, outdoorZones, errorMessage);
        this.setSensorList(_3dOptions, sensorList);

        const selectedBuildingGroupInfo = { ...this.state.selectedBuildingGroupInfo };

        if (buildingGroupList && buildingGroupList.length > 0) {
            selectedBuildingGroupInfo.buildingGroup = buildingGroupList[0];
        }

        //console.log(JSON.stringify(_3dOptions));
        this.setState({ loading: false, _3dOptions: _3dOptions, buildingGroupList: buildingGroupList, selectedBuildingGroupInfo });
    }

    setSensorList(_3dOptions, sensorList) {
        if (!sensorList || !_3dOptions) {
            console.log('[error] sensorList가 없음');
        }
        else {
            const fireSensors = sensorList['fireSensors'];
            //const psmSensors = sensorList['psmSensors'];
            //const etcSensors = sensorList['etcSensors'];
            const cctvs = sensorList['cctvs'];
            
            if (fireSensors) {
                this.setFireSensors(fireSensors, _3dOptions);
            }

            /*if (psmSensors !== null) {
                this.setPSMSensors(psmSensors, _3dOptions);
            }

            if (etcSensors !== null) {
                this.setEtcSensors(etcSensors, _3dOptions);
            }*/

            if (cctvs) {
                this.setCCTVs(cctvs, _3dOptions);
            }

            this.setSensorTypes(sensorList, _3dOptions, SDMSMainMenu.CO_Sensor + "Sensors");
            this.setSensorTypes(sensorList, _3dOptions, SDMSMainMenu.O2_Sensor + "Sensors");
            this.setSensorTypes(sensorList, _3dOptions, SDMSMainMenu.H2_Sensor + "Sensors");
            this.setSensorTypes(sensorList, _3dOptions, SDMSMainMenu.CH4_Sensor + "Sensors");
            this.setSensorTypes(sensorList, _3dOptions, SDMSMainMenu.Detect_Sensor + "Sensors");
        }
    }

    setSensorTypes(sensorList, _3dOptions, sensorTypeName) {
        const sensors = sensorList[sensorTypeName];
        const sensorType = sensorTypeName.replace('Sensors', '');

        if (sensors) {
            const sensorCount = sensors.length;

            for (let i = 0; i < sensorCount; i++) {
                const sensor = sensors[i];
                let zoneID = null;

                if (sensor.zoneID !== undefined && sensor.zoneID !== null) {
                    zoneID = sensor.zoneID;
                }
                else {
                    if (sensor.linkedZones && sensor.linkedZones.length > 0) {
                        zoneID = sensor.linkedZones[0].id;
                    }
                }

                if (zoneID === undefined || zoneID === null) {
                    continue;
                }

                let zone = _3dOptions.zones[zoneID.toString()];

                if (!zone) {
                    zone = _3dOptions.outdoorZones[zoneID.toString()];
                }

                if (zone) {
                    if (!zone.sensors[sensorType]) {
                        zone.sensors[sensorType] = [];
                    }

                    zone.sensors[sensorType].push(sensor);
                }
            }
        }
    }

    setFireSensors(fireSensors, _3dOptions) {
        const sensorCount = fireSensors.length;

        for (let i = 0; i < sensorCount; i++) {
            const sensor = fireSensors[i];
            let zone = _3dOptions.zones[sensor.zoneID];

            if (!zone) {
                zone = _3dOptions.outdoorZones[sensor.zoneID.toString()];
            }

            if (zone) {
                if (!zone.sensors.fire) {
                    zone.sensors.fire = [];
                }

                zone.sensors.fire.push(sensor);
            }
        }
    }

    setPSMSensors(psmSensors, _3dOptions) {
        const sensorCount = psmSensors.length;

        for (let i = 0; i < sensorCount; i++) {
            const sensor = psmSensors[i];

            if (sensor.linkedZones.length > 0) {
                let zone = _3dOptions.zones[sensor.linkedZones[0].id];

                if (!zone) {
                    // PSM 센서는 Zone ID가 없음.
                    //zone = _3dOptions.outdoorZones[sensor.zoneID.toString()];
                    zone = _3dOptions.outdoorZones[sensor.linkedZones[0].id.toString()];
                }

                if (zone) {
                    if (!zone.sensors.psm) {
                        zone.sensors.psm = [];
                    }

                    zone.sensors.psm.push(sensor);
                }
            }
        }
    }

    setEtcSensors(etcSensors, _3dOptions) {
        const sensorCount = etcSensors.length;

        for (let i = 0; i < sensorCount; i++) {
            const sensor = etcSensors[i];
            let zone = _3dOptions.zones[sensor.zoneID];

            if (!zone) {
                zone = _3dOptions.outdoorZones[sensor.zoneID.toString()];
            }

            if (zone) {
                if (!zone.sensors.etc) {
                    zone.sensors.etc = [];
                }

                zone.sensors.etc.push(sensor);
            }
        }
    }

    setCCTVs(cctvs, _3dOptions) {
        const cctvCount = cctvs.length;

        for (let i = 0; i < cctvCount; i++) {
            const cctv = cctvs[i];
            let zone = _3dOptions.zones[cctv.zoneID];

            if (!zone && cctv.zoneID !== null && cctv.zoneID !== undefined) {
                zone = _3dOptions.outdoorZones[cctv.zoneID.toString()];
            }

            if (zone) {
                if (!zone.sensors.cctv) {
                    zone.sensors.cctv = [];
                }

                zone.sensors.cctv.push(cctv);
            }
        }
    }

    // 드래그로 선택된 팝업과 나머지 팝업의 z-index를 조절한다. (선택된 팝업이 앞으로 나오도록)
    setActiveDragPopup = (popupType/*: string*/) => {
        const popupLayer = { ...this.state.popupLayer };
        const layerName = popupType + "ZIndex";

        for (const key in popupLayer) {
            if (key === layerName) {
                popupLayer[key] = 1;
            }
            else {
                popupLayer[key] = 0;
            }
        }

        this.setState({ popupLayer });
    }

    //팝업 크기, 위치값 저장
    async setPopupState(popup/*: string*/, state/*: SizableDragable.PopupState*/) {
        if (ProjectResource.isModelViewer) {
            return;
        }

        // setState
        const popupState = this.state.popupState;
        //popupState.set(popup, state);
        popupState[popup] = state;
        //DB 전달
        const userInfo = JSON.parse(window.localStorage.getItem(SessionString.Key.account));
        const result = await SDMSController.requestSaveOption(
            state.id,
            1/*userInfo.id*/,    // UserID
            'popup',        // Category
            popup,          // SubCategory
            state.x,        // PropertyValue1
            state.y,        // PropertyValue2
            state.height,    // PropertyValue3
            state.width    // PropertyValue4
        );
        if (result[0]) {
            const options = result[1];
            //popupState.get(popup).id = options[0].id;
            popupState[popup].id = result[1][0].id;
            this.setState({ popupState: popupState });
        }
    }

    //cctv 전체화면 설정
    setCctvFullScreenState = (cctvFullScreenState) => {
        this.setState({
            cctvFullScreenState: {
                isFullScreen: cctvFullScreenState.isFullScreen,
                url: cctvFullScreenState.url,
                cctvName: cctvFullScreenState.cctvName,
                w: cctvFullScreenState.w,
                h: cctvFullScreenState.h
            }
        });

    }

    onSelectMenu = (menu/*: Common.NullableNumber*/, param/*: any*/) => {
        this.processMenu(menu, param);
    }

    async processMenu(menu/*: Common.NullableNumber*/, param/*: any*/) {
        const cmd = {
            menu: menu,
            menuParameter: param
        };

        if (menu === SDMSMainMenu.Menu_Add_Alarm) {
            this.setState({ command: cmd, alarmSound: true });
            return;
        }

        if (menu >= SDMSMainMenu.Menu_ToggleStatusInfo && menu <= SDMSMainMenu.Menu_ToggleHistory) {
            this.togglePopup(menu);
            return;
        }

        this.setState({ command: cmd });
    }

    togglePopup(menu) {
        const visiblePopups = { ...this.state.visiblePopups };

        if (menu === SDMSMainMenu.Menu_ToggleStatusInfo) {
            visiblePopups.statusInfo = !visiblePopups.statusInfo;
        }
        else if (menu === SDMSMainMenu.Menu_ToggleCCTV) {
            visiblePopups.cctvInfo = !visiblePopups.cctvInfo;
        }
        else if (menu === SDMSMainMenu.Menu_ToggleWorkerMonitor) {
            visiblePopups.workerInfo = !visiblePopups.workerInfo;
        }
        else if (menu === SDMSMainMenu.Menu_ToggleWarning) {
            visiblePopups.warningAlarmInfo = !visiblePopups.warningAlarmInfo;
        }
        else if (menu === SDMSMainMenu.Menu_ToggleSpreadInfo) {
            visiblePopups.spreadInfo = !visiblePopups.spreadInfo;
        }
        else if (menu === SDMSMainMenu.Menu_ToggleHistory) {
            visiblePopups.historyInfo = !visiblePopups.historyInfo;
        }

        this.setState({ visiblePopups });
    }

    isIndoor() {
        const currentBuildingID = this.state.currentView.buildingID;

        if (currentBuildingID !== null && currentBuildingID !== undefined) {
            return true;
        }

        return false;
    }

    onClickLogo = () => {
        const outdoorModel = this.state._3dOptions["outdoorModel"];

        if (outdoorModel) {
            const cmd = {
                menu: SDMSMainMenu.Menu_Show_Outdoor,
                menuParameter: outdoorModel
            };

            this.setState({ command: cmd });
        }
    }

    setCurrentView = (zoneID/*: Common.NullableNumber*/) => {
        if (this.state.currentView.zoneID !== zoneID) {
            let buildingID = null;
            let zoneName = '';

            if (zoneID !== null) {
                const zone = this.state._3dOptions["zones"][zoneID];

                if (zone) {
                    buildingID = zone[1];
                    zoneName = zone[3];
                }
            }

            this.setState({ currentView: { buildingID, zoneID, zoneName } });
        }
    }

    showConfirmDialog = (title, messages, buttons, onClickButton) => {
        const confirmMessage = { ...this.state.confirmMessage };
        confirmMessage.visible = true;
        confirmMessage.title = title;
        confirmMessage.buttons = buttons;
        confirmMessage.onClickButton = onClickButton;

        if (!messages) {
            confirmMessage.messages = [""];
        }
        else if (Array.isArray(messages)) {
            confirmMessage.messages = messages;
        }
        else {
            confirmMessage.messages = [messages];
        }

        this.setState({ confirmMessage });
    }

    onCloseConfirmDialog = () => {
        const confirmMessage = { ...this.state.confirmMessage };
        confirmMessage.visible = false;

        this.setState({ confirmMessage });
    }

    setVisiblePoi = (typeName, visible) => {
        let types = { ...this.state.visibleSensorTypes };
        types[typeName] = visible;
        this.setState({ visibleSensorTypes: types });
    }

    setVisiblePopup = (popup, visible) => {
        const visiblePopups = { ...this.state.visiblePopups };
        visiblePopups[popup] = visible;

        // cctv창이 닫힐 경우 기존 리스트 초기화
        if (popup === "cctvInfo") {
            this.state.cctvList = null;
        }

        this.setState({ visiblePopups });
    }

    getSelectedSensorInfo() {
        const selectedPOI = this.state.selectedPOI;

        if (selectedPOI) {
            return [selectedPOI[0], selectedPOI[1], selectedPOI[2]];
        }

        return [null, null, null];
    }

    onSelectSensor = (sensorType, sensorID, zoneID) => {
        if (!sensorType || !sensorID || !zoneID) {
            this.setState({ selectedPOI: null });
        }
        else {
            this.setState({ selectedPOI: [sensorType, zoneID, sensorID] });
        }
    }

    onChangeBuildingGroup = (value, type) => {
        const selectedBuildingGroupInfo = this.state.selectedBuildingGroupInfo;
        if (type === 'buildingGroup') {
            selectedBuildingGroupInfo.buildingGroup = value;
            selectedBuildingGroupInfo.building = null;
            selectedBuildingGroupInfo.zone = null;
            selectedBuildingGroupInfo.sensorGroups = false;
            selectedBuildingGroupInfo.fireSensors = false;
            selectedBuildingGroupInfo.coSensors = false;
            selectedBuildingGroupInfo.o2Sensors = false;
            selectedBuildingGroupInfo.h2Sensors = false;
            selectedBuildingGroupInfo.ch4Sensors = false;
            selectedBuildingGroupInfo.detectSensors = false;
            selectedBuildingGroupInfo.psmSensors = false;
            selectedBuildingGroupInfo.etcSensors = false;
            selectedBuildingGroupInfo.cctvGroups = false;
            selectedBuildingGroupInfo.cctvSubGroups = false;
        }
        else if (type === 'building') {
            selectedBuildingGroupInfo.building = value;
            selectedBuildingGroupInfo.zone = null;
            selectedBuildingGroupInfo.sensorGroups = false;
            selectedBuildingGroupInfo.fireSensors = false;
            selectedBuildingGroupInfo.coSensors = false;
            selectedBuildingGroupInfo.o2Sensors = false;
            selectedBuildingGroupInfo.h2Sensors = false;
            selectedBuildingGroupInfo.ch4Sensors = false;
            selectedBuildingGroupInfo.detectSensors = false;
            selectedBuildingGroupInfo.psmSensors = false;
            selectedBuildingGroupInfo.etcSensors = false;
            selectedBuildingGroupInfo.cctvGroups = false;
            selectedBuildingGroupInfo.cctvSubGroups = false;
        }
        else if (type === 'zone') {
            selectedBuildingGroupInfo.zone = value;
            selectedBuildingGroupInfo.sensorGroups = false;
            selectedBuildingGroupInfo.fireSensors = false;
            selectedBuildingGroupInfo.coSensors = false;
            selectedBuildingGroupInfo.o2Sensors = false;
            selectedBuildingGroupInfo.h2Sensors = false;
            selectedBuildingGroupInfo.ch4Sensors = false;
            selectedBuildingGroupInfo.detectSensors = false;
            selectedBuildingGroupInfo.psmSensors = false;
            selectedBuildingGroupInfo.etcSensors = false;
            selectedBuildingGroupInfo.cctvGroups = false;
            selectedBuildingGroupInfo.cctvSubGroups = false;
        }
        else if (type === 'sensorGroups') {
            selectedBuildingGroupInfo.sensorGroups = true;
            selectedBuildingGroupInfo.fireSensors = false;
            selectedBuildingGroupInfo.coSensors = false;
            selectedBuildingGroupInfo.o2Sensors = false;
            selectedBuildingGroupInfo.h2Sensors = false;
            selectedBuildingGroupInfo.ch4Sensors = false;
            selectedBuildingGroupInfo.detectSensors = false;
            selectedBuildingGroupInfo.psmSensors = false;
            selectedBuildingGroupInfo.etcSensors = false;
            selectedBuildingGroupInfo.cctvGroups = false;
            selectedBuildingGroupInfo.cctvSubGroups = false;
        }
        else if (type === 'fireSensors') {
            selectedBuildingGroupInfo.fireSensors = true;
            selectedBuildingGroupInfo.coSensors = false;
            selectedBuildingGroupInfo.o2Sensors = false;
            selectedBuildingGroupInfo.h2Sensors = false;
            selectedBuildingGroupInfo.ch4Sensors = false;
            selectedBuildingGroupInfo.detectSensors = false;
            selectedBuildingGroupInfo.psmSensors = false;
            selectedBuildingGroupInfo.etcSensors = false;
            selectedBuildingGroupInfo.cctvGroups = false;
            selectedBuildingGroupInfo.cctvSubGroups = false;
        }
        else if (type === 'coSensors') {
            selectedBuildingGroupInfo.fireSensors = false;
            selectedBuildingGroupInfo.coSensors = true;
            selectedBuildingGroupInfo.o2Sensors = false;
            selectedBuildingGroupInfo.h2Sensors = false;
            selectedBuildingGroupInfo.ch4Sensors = false;
            selectedBuildingGroupInfo.detectSensors = false;
            selectedBuildingGroupInfo.psmSensors = false;
            selectedBuildingGroupInfo.etcSensors = false;
            selectedBuildingGroupInfo.cctvGroups = false;
            selectedBuildingGroupInfo.cctvSubGroups = false;
        }
        else if (type === 'o2Sensors') {
            selectedBuildingGroupInfo.fireSensors = false;
            selectedBuildingGroupInfo.coSensors = false;
            selectedBuildingGroupInfo.o2Sensors = true;
            selectedBuildingGroupInfo.h2Sensors = false;
            selectedBuildingGroupInfo.ch4Sensors = false;
            selectedBuildingGroupInfo.detectSensors = false;
            selectedBuildingGroupInfo.psmSensors = false;
            selectedBuildingGroupInfo.etcSensors = false;
            selectedBuildingGroupInfo.cctvGroups = false;
            selectedBuildingGroupInfo.cctvSubGroups = false;
        }
        else if (type === 'h2Sensors') {
            selectedBuildingGroupInfo.fireSensors = false;
            selectedBuildingGroupInfo.coSensors = false;
            selectedBuildingGroupInfo.o2Sensors = false;
            selectedBuildingGroupInfo.h2Sensors = true;
            selectedBuildingGroupInfo.ch4Sensors = false;
            selectedBuildingGroupInfo.detectSensors = false;
            selectedBuildingGroupInfo.psmSensors = false;
            selectedBuildingGroupInfo.etcSensors = false;
            selectedBuildingGroupInfo.cctvGroups = false;
            selectedBuildingGroupInfo.cctvSubGroups = false;
        }
        else if (type === 'ch4Sensors') {
            selectedBuildingGroupInfo.fireSensors = false;
            selectedBuildingGroupInfo.coSensors = false;
            selectedBuildingGroupInfo.o2Sensors = false;
            selectedBuildingGroupInfo.h2Sensors = false;
            selectedBuildingGroupInfo.ch4Sensors = true;
            selectedBuildingGroupInfo.detectSensors = false;
            selectedBuildingGroupInfo.psmSensors = false;
            selectedBuildingGroupInfo.etcSensors = false;
            selectedBuildingGroupInfo.cctvGroups = false;
            selectedBuildingGroupInfo.cctvSubGroups = false;
        }
        else if (type === 'detectSensors') {
            selectedBuildingGroupInfo.fireSensors = false;
            selectedBuildingGroupInfo.coSensors = false;
            selectedBuildingGroupInfo.o2Sensors = false;
            selectedBuildingGroupInfo.h2Sensors = false;
            selectedBuildingGroupInfo.ch4Sensors = false;
            selectedBuildingGroupInfo.detectSensors = true;
            selectedBuildingGroupInfo.psmSensors = false;
            selectedBuildingGroupInfo.etcSensors = false;
            selectedBuildingGroupInfo.cctvGroups = false;
            selectedBuildingGroupInfo.cctvSubGroups = false;
        }
        else if (type === 'psmSensors') {
            selectedBuildingGroupInfo.fireSensors = false;
            selectedBuildingGroupInfo.coSensors = false;
            selectedBuildingGroupInfo.o2Sensors = false;
            selectedBuildingGroupInfo.h2Sensors = false;
            selectedBuildingGroupInfo.ch4Sensors = false;
            selectedBuildingGroupInfo.detectSensors = false;
            selectedBuildingGroupInfo.psmSensors = true;
            selectedBuildingGroupInfo.etcSensors = false;
            selectedBuildingGroupInfo.cctvGroups = false;
            selectedBuildingGroupInfo.cctvSubGroups = false;
        }
        else if (type === 'etcSensors') {
            selectedBuildingGroupInfo.fireSensors = false;
            selectedBuildingGroupInfo.coSensors = false;
            selectedBuildingGroupInfo.o2Sensors = false;
            selectedBuildingGroupInfo.h2Sensors = false;
            selectedBuildingGroupInfo.ch4Sensors = false;
            selectedBuildingGroupInfo.detectSensors = false;
            selectedBuildingGroupInfo.psmSensors = false;
            selectedBuildingGroupInfo.etcSensors = true;
            selectedBuildingGroupInfo.cctvGroups = false;
            selectedBuildingGroupInfo.cctvSubGroups = false;
        }
        else if (type === 'cctvGroups') {
            selectedBuildingGroupInfo.fireSensors = false;
            selectedBuildingGroupInfo.coSensors = false;
            selectedBuildingGroupInfo.o2Sensors = false;
            selectedBuildingGroupInfo.h2Sensors = false;
            selectedBuildingGroupInfo.ch4Sensors = false;
            selectedBuildingGroupInfo.detectSensors = false;
            selectedBuildingGroupInfo.psmSensors = false;
            selectedBuildingGroupInfo.etcSensors = false;
            selectedBuildingGroupInfo.cctvGroups = true;
            selectedBuildingGroupInfo.cctvSubGroups = false;
        }
        else if (type === 'cctvSubGroups') {
            selectedBuildingGroupInfo.fireSensors = false;
            selectedBuildingGroupInfo.coSensors = false;
            selectedBuildingGroupInfo.o2Sensors = false;
            selectedBuildingGroupInfo.h2Sensors = false;
            selectedBuildingGroupInfo.ch4Sensors = false;
            selectedBuildingGroupInfo.detectSensors = false;
            selectedBuildingGroupInfo.psmSensors = false;
            selectedBuildingGroupInfo.etcSensors = false;
            selectedBuildingGroupInfo.cctvGroups = true;
            selectedBuildingGroupInfo.cctvSubGroups = true;
        }
        else if (type === 'all') {
            selectedBuildingGroupInfo.buildingGroup = null;
            selectedBuildingGroupInfo.building = null;
            selectedBuildingGroupInfo.zone = null;
            selectedBuildingGroupInfo.sensorGroups = null;
            selectedBuildingGroupInfo.fireSensors = null;
            selectedBuildingGroupInfo.coSensors = false;
            selectedBuildingGroupInfo.o2Sensors = false;
            selectedBuildingGroupInfo.h2Sensors = false;
            selectedBuildingGroupInfo.ch4Sensors = false;
            selectedBuildingGroupInfo.detectSensors = false;
            selectedBuildingGroupInfo.psmSensors = null;
            selectedBuildingGroupInfo.etcSensors = null;
            selectedBuildingGroupInfo.cctvGroups = null;
            selectedBuildingGroupInfo.cctvSubGroups = null;
        }

        this.setState({ selectedBuildingGroupInfo, selectedPOI: null });
    }

    moveToX = (menu, menuParameter) => {
        this.onSelectMenu(menu, menuParameter);
    }

    onSelectedAlarm(alarm) {
        if (this.state.selectedAlarm === alarm) {
            return;
        }

        //this.getEquipZoneCCTV(alarm.equipZoneID);

        this.setState({ selectedAlarm: alarm });
    }

    // 선택된 알람으로 3D 이동
    onMoveSelectedAlarm = () => {
        const selectedAlarm = this.state.selectedAlarm;

        if (selectedAlarm) {
            if (selectedAlarm.sensorZoneID < 1000000) {
                //const alarmType = this.getAlarmTypeFromMessage(selectedAlarm.message);
                //const alarmCCTV = this.showAlarmCCTV(alarmType, selectedAlarm);
                //this.showAlarm(selectedAlarm, alarmCCTV);
                this.showAlarm(selectedAlarm, null);
            }
            else {
                this.showAlarm(selectedAlarm, null);
            }
        }
    }

    onMalfunction = (alarm) => {
        if (alarm.sensorZoneID >= 1000000) {
            this.showConfirmDialog("알람 종료", ["수동 신고한 상황을 종료할까요?"], ["상황 종료", "취소"], this.onClickMalfunction);
        }
        else {
            this.showConfirmDialog("알람 종료", ["탐지된 신호를 종료할까요?"], ["종료", "오작동", "취소"], this.onClickMalfunction);
        }
    }

    async onClickMalfunction(index) {
        const confirmMessage = { ...this.state.confirmMessage };
        confirmMessage.visible = false;

        const alarm = this.state.selectedAlarm;
        const userInfo = JSON.parse(window.localStorage.getItem(SessionString.Key.account));

        if (alarm.sensorZoneID >= 1000000) {
            if (index === 0) {
                await SDMSController.requestClearManualReport(alarm.facilityType, alarm.sensorZoneID, alarm.sensorZoneHistoryID, 1/*userInfo.id*/);
            }
        }
        else if (alarm && index <= 1) { // 오작동, 사용자복구
            if (index === 0) {
                await SDMSController.requestMalfunction(alarm.facilityType, alarm.sensorZoneID, 1/*userInfo.id*/, false);
            }
            else if (index === 1) {
                await SDMSController.requestMalfunction(alarm.facilityType, alarm.sensorZoneID, 1/*userInfo.id*/, true);
            }
        }

        this.setState({ confirmMessage });
    }

    onSelectPOI = (poi, updateDB, contents3D) => {
        if (poi) {
            const [buildingGroup, building, zone, sensorType] = this.onChangeBuildingGroup2(poi);
            if (buildingGroup && building && zone) {
                const selectedBuildingGroupInfo = this.state.selectedBuildingGroupInfo;

                selectedBuildingGroupInfo.buildingGroup = buildingGroup;
                selectedBuildingGroupInfo.building = building;
                selectedBuildingGroupInfo.zone = zone;

                if (sensorType === 'cctv') {
                    selectedBuildingGroupInfo.sensorGroups = false;
                    selectedBuildingGroupInfo.fireSensors = false;
                    selectedBuildingGroupInfo.coSensors = false;
                    selectedBuildingGroupInfo.o2Sensors = false;
                    selectedBuildingGroupInfo.h2Sensors = false;
                    selectedBuildingGroupInfo.ch4Sensors = false;
                    selectedBuildingGroupInfo.detectSensors = false;
                    selectedBuildingGroupInfo.psmSensors = false;
                    selectedBuildingGroupInfo.etcSensors = false;
                    selectedBuildingGroupInfo.cctvGroups = true;
                    selectedBuildingGroupInfo.cctvSubGroups = true;
                }
                else {
                    selectedBuildingGroupInfo.sensorGroups = true;
                    if (sensorType === 'fire') {
                        selectedBuildingGroupInfo.fireSensors = true;
                        selectedBuildingGroupInfo.coSensors = false;
                        selectedBuildingGroupInfo.o2Sensors = false;
                        selectedBuildingGroupInfo.h2Sensors = false;
                        selectedBuildingGroupInfo.ch4Sensors = false;
                        selectedBuildingGroupInfo.detectSensors = false;
                        selectedBuildingGroupInfo.psmSensors = false;
                        selectedBuildingGroupInfo.etcSensors = false;
                    }
                    else if (sensorType === 'psm') {
                        selectedBuildingGroupInfo.fireSensors = false;
                        selectedBuildingGroupInfo.coSensors = false;
                        selectedBuildingGroupInfo.o2Sensors = false;
                        selectedBuildingGroupInfo.h2Sensors = false;
                        selectedBuildingGroupInfo.ch4Sensors = false;
                        selectedBuildingGroupInfo.detectSensors = false;
                        selectedBuildingGroupInfo.psmSensors = true;
                        selectedBuildingGroupInfo.etcSensors = false;
                    }
                    else if (sensorType === 'etc') {
                        selectedBuildingGroupInfo.fireSensors = false;
                        selectedBuildingGroupInfo.coSensors = false;
                        selectedBuildingGroupInfo.o2Sensors = false;
                        selectedBuildingGroupInfo.h2Sensors = false;
                        selectedBuildingGroupInfo.ch4Sensors = false;
                        selectedBuildingGroupInfo.detectSensors = false;
                        selectedBuildingGroupInfo.psmSensors = false;
                        selectedBuildingGroupInfo.etcSensors = true;
                    }
                    selectedBuildingGroupInfo.cctvGroups = false;
                    selectedBuildingGroupInfo.cctvSubGroups = false;
                }

                this.setState({ selectedBuildingGroupInfo });
            }

            this.setState({ selectedPOI: [poi, updateDB] });
        }
        else {
            if (this.state.selectedPOI !== null) {
                this.setState({ selectedPOI: null });
            }
        }
    }

    onChangeBuildingGroup2(poi) {
        const [sensorType, zoneID, sensorID] = SDMS.getSensorInfo(poi);

        if (zoneID) {
            if (zoneID >= 20000) {

                return [this.state._3dOptions.outdoorZones, this.state._3dOptions.outdoorZones, this.state._3dOptions.outdoorZones[zoneID], sensorType];
            }
            else {
                const buildingGroupCount = this.state.buildingGroupList.length;
                for (let i = 0; i < buildingGroupCount; i++) {
                    const buildingGroup = this.state.buildingGroupList[i];
                    const buildingCount = buildingGroup.buildingDatas.length;
                    for (let j = 0; j < buildingCount; j++) {
                        const building = buildingGroup.buildingDatas[j];
                        const zoneCount = building.zoneDatas.length;
                        for (let k = 0; k < zoneCount; k++) {
                            const zone = building.zoneDatas[k];
                            if (!zone)
                                continue;

                            if (zoneID === zone.id) {
                                return [buildingGroup, building, zone, sensorType];
                            }
                        }
                    }
                }
            }
        }
    }

    onSelectCCTV = (cctvID, poi, poiManager) => {
        const isEditMode = false;

        if (!isEditMode) {
            if (this.containCCTV(cctvID) === false) {
                var menus = this.state.visiblePopups;
                menus['cctvInfo'] = true;

                this.setState({ cctvList: this.getCCTVList(cctvID), visiblePopups: menus, selectedPOI: [poi, false] });
                // 하나의 CCTV만 표시하는 방식
                //this.setState({ cctvList: cctvID, visiblePopups: menus, selectedPOI: [poi, false] });
            }
        }
        else if (isEditMode) {
            this.setState({ selectedPOI: [poi, false] });
        }
    }

    containCCTV(cctvID) {
        const cctvList = this.state.cctvList;

        if (!cctvList) {
            return false;
        }

        const ids = cctvList.toString().split(',');
        const count = ids.length;

        const _cctvID = cctvID.toString();

        for (let i = 0; i < count; i++) {
            const id = ids[i].trim();

            if (id === _cctvID) {
                return true;
            }
        }

        return false;
    }

    getCCTVList(cctvID) {
        const cctvList = this.state.cctvList;

        if (!cctvList) {
            return cctvID;
        }

        const ids = cctvList.toString().split(',');
        const count = ids.length;

        if (count === 0) {
            return cctvID;
        } else if (count <= 3) {
            //return cctvList + "," + cctvID;
            return cctvID + ',' + cctvList;
        }

        let strCCTVList = "";

        //for (let i = count - 3; i < count; i++) {
        //    if (i === count - 3) {
        //        strCCTVList = ids[i].trim();
        //    }
        //    else {
        //        strCCTVList += "," + ids[i].trim();
        //    }
        //}
        for (let i = 0; i < 3; i++) {
            if (i === 0) {
                strCCTVList = ids[i].trim();
            } else {
                strCCTVList += "," + ids[i].trim();
            }
        }

        //return strCCTVList + "," + cctvID;
        return cctvID + ',' + strCCTVList;
    }

    static getSensorInfo(poi) {
        const name = poi.object ? poi.object.name : poi.name;

        const index1 = name.indexOf('_');
        const index2 = name.lastIndexOf('_');

        if (index1 < 0 || index2 <= index1) {
            return [null, null, null];
        }

        const sensorType = name.substring(0, index1).trim();
        const strZoneID = name.substring(index1 + 1, index2).trim();
        const strSensorID = name.substring(index2 + 1).trim();

        const zoneID = parseInt(strZoneID);
        const sensorID = parseInt(strSensorID);

        if (sensorType.length > 0 &&
            zoneID !== null && zoneID !== undefined && zoneID !== NaN &&
            sensorID !== null && sensorID !== undefined && sensorID !== NaN) {
            return [sensorType, zoneID, sensorID];
        }

        return [null, null, null];
    }

    onSelectWorker = (worker) => {
        if (worker === null || worker === undefined)
            return;

        let menus = this.state.visiblePopups;
        menus['workerInfo'] = true;

        this.setState({ visiblePopups: menus, selectWorker: worker});
    }

    onSound(sound) {
        if (sound !== this.state.alarmSound) {
            this.setState({ alarmSound: sound });
        }
    }

    popupMessage = (title, message) => {
        if (title === null || title === undefined || title === "" ||
            message === null || message === undefined || message === "")
            return;

        this.showConfirmDialog(title, message, null, null);
    }

    openSpreadInfo = () => {
        let menus = this.state.visiblePopups;
        menus['spreadInfo'] = true;

        this.setState({ visiblePopups: menus });
    }

    render() {
        if (this.state.loading) {
            return (
                <></>
            );
        }

        const [sensorType, zoneID, sensorID] = this.getSelectedSensorInfo();

        return (
            <div className={styles.bodyArea} style={{ MozUserSelect: 'none', WebkitUserSelect: 'none' }}>
                <Contents3D
                    _3dOptions={this.state._3dOptions}
                    command={this.state.command}
                    setCurrentView={this.setCurrentView}
                    initOutdoorViewport={this.onClickLogo}
                    visibleSensorTypes={this.state.visibleSensorTypes}
                    onSelectPOI={this.onSelectPOI}
                    onSelectCCTV={this.onSelectCCTV}
                    workers={this.state.workers}
                    currentView={this.state.currentView}
                    onSelectWorker={this.onSelectWorker}
                    alarmSound={this.state.alarmSound}
                />

                {
                    this.state.visiblePopups.statusInfo &&
                    <StatusInfo
                        popupType="statusInfo"
                        setVisiblePopup={this.setVisiblePopup}
                        setActiveDragPopup={this.setActiveDragPopup}
                        zIndex={this.state.popupLayer.statusInfoZIndex}
                        popupState={this.state.popupState["statusInfo"]}
                        setPopupState={this.setPopupState}
                        visibleSensorTypes={this.state.visibleSensorTypes}
                        setVisiblePoi={this.setVisiblePoi}
                        buildingGroupList={this.state.buildingGroupList}
                        outdoorZones={this.state._3dOptions.outdoorZones}
                        zoneList={this.state._3dOptions.zones}
                        buildingIDs={this.state._3dOptions.buildingIDs}
                        indoorModels={this.state._3dOptions.indoorModels}
                        sensorList={this.state.sensorList}
                        selectedSensor={[sensorType, zoneID, sensorID]}
                        onSelectSensor={this.onSelectSensor}
                        selectedInfo={this.state.selectedBuildingGroupInfo}
                        onChangeBuildingGroup={this.onChangeBuildingGroup}
                        moveToX={this.moveToX}
                        sensorAlarms={this.state.sensorAlarms}
                        initOutdoorViewport={this.onClickLogo}
                    />
                }
                {
                    this.state.visiblePopups.spreadInfo && !ProjectResource.isModelViewer &&
                    <SpreadInfo
                        popupType="spreadInfo"
                        setVisiblePopup={this.setVisiblePopup}
                        setActiveDragPopup={this.setActiveDragPopup}
                        zIndex={this.state.popupLayer.spreadInfoZIndex}
                        popupState={this.state.popupState["spreadInfo"]}
                        setPopupState={this.setPopupState}
                        workers={this.state.workers}
                        buildingGroupList={this.state.buildingGroupList}
                        popupMessage={this.popupMessage}
                    />
                }
                {
                    this.state.visiblePopups.cctvInfo && !ProjectResource.isModelViewer &&
                    <CCTVInfo
                        popupType="cctvInfo"
                        setVisiblePopup={this.setVisiblePopup}
                        streamServerURL={this.state.streamServerURL}
                        cctvs={this.state.sensorList['cctvs']}
                        popupState={this.state.popupState["cctvInfo"]}
                        setActiveDragPopup={this.setActiveDragPopup}
                        zIndex={this.state.popupLayer.cctvInfoZIndex}
                        setPopupState={this.setPopupState}
                        cctvList={this.state.cctvList}
                        cctvFullScreenState={this.state.cctvFullScreenState}
                        setCctvFullScreenState={this.setCctvFullScreenState}
                        setCCTVList={this.setCCTVList}
                    />
                }
                {
                    this.state.visiblePopups.warningAlarmInfo && !ProjectResource.isModelViewer &&
                    <WarningAlarmInfo
                        popupType="warningAlarmInfo"
                        setVisiblePopup={this.setVisiblePopup}
                        setActiveDragPopup={this.setActiveDragPopup}
                        zIndex={this.state.popupLayer.warningAlarmInfoZIndex}
                        popupState={this.state.popupState["warningAlarmInfo"]}
                        setPopupState={this.setPopupState}
                        visibleSensorTypes={this.state.visibleSensorTypes}
                        setVisiblePoi={this.setVisiblePoi}
                        sensorAlarms={this.state.sensorAlarms}
                        selectedAlarm={this.state.selectedAlarm}
                        onSelectedAlarm={this.onSelectedAlarm}
                        onMoveSelectedAlarm={this.onMoveSelectedAlarm}
                        onMalfunction={this.onMalfunction}
                        alarmSound={this.state.alarmSound}
                        onSound={this.onSound}
                    />
                }
                {
                    this.state.visiblePopups.workerInfo && !ProjectResource.isModelViewer &&
                    <WorkerInfo
                        popupType="workerInfo"
                        setVisiblePopup={this.setVisiblePopup}
                        setActiveDragPopup={this.setActiveDragPopup}
                        zIndex={this.state.popupLayer.workerInfoZIndex}
                        popupState={this.state.popupState["workerInfo"]}
                        setPopupState={this.setPopupState}
                        selectWorker={this.state.selectWorker}
                        buildingGroupList={this.state.buildingGroupList}
                        openSpreadInfo={this.openSpreadInfo}
                    />
                }
                {
                    this.state.visiblePopups.historyInfo && !ProjectResource.isModelViewer &&
                    <HistoryInfo
                        popupType="historyInfo"
                        setVisiblePopup={this.setVisiblePopup}
                        buildingGroupList={this.state.buildingGroupList}
                        setActiveDragPopup={this.setActiveDragPopup}
                        zIndex={this.state.popupLayer.workerInfoZIndex}
                        popupState={this.state.popupState["historyInfo"]}
                        setPopupState={this.setPopupState}
                    />
                }
                {
                    this.state.visiblePopups.warningAlarmInfo && !ProjectResource.isModelViewer &&
                    <DetectionTextInfo
                        popupType="detectionTextInfo"
                        setVisiblePopup={this.setVisiblePopup}
                        sensorAlarms={this.state.sensorAlarms}
                    />
                }
                {
                    /* alert창 대신 사용 */
                    this.state.confirmMessage.visible &&
                    <ConfirmDialog title={this.state.confirmMessage.title} messages={this.state.confirmMessage.messages} buttons={this.state.confirmMessage.buttons} onClose={this.state.confirmMessage.onClose} onClickButton={this.state.confirmMessage.onClickButton} />
                }
            </div>
        );
    }
}


export default SDMS;