import React, { Component } from 'react';
import styles from '../Root/css/root.module.css';
import { POIManager } from './3D/poiManager';
import { CommonController } from './services/commonController';
import { CommonDataManager } from './services/commonDataManager';
import { ModelDataManager } from './services/modelDataManager';
import { SpaceController } from './services/spaceController';
import { SpaceDataManager } from './services/spaceDataManager';
import { ExcelManager } from './spaceBodyWorker/excelManager';
import { SpaceContents } from './spaceContents';
import { SpaceMenus } from './spaceMenus';
import { TempModelManager } from './spatialInfo/worker/tempModelManager';

export class SpaceBody extends Component {
    static SelectedStatusInfoType = {
        none: 0,
        buildingGroup: 1,
        building: 2,
        zone: 3,
        sensorGroups: 4,
        fireSensors: 5,
        psmSensors: 6,
        etcSensors: 7,
        cctvGroups: 8,
        cctvSubGroups: 9,
        facilityGroups: 10,
        facilitySubGroups: 11
    }

    static Type_Site = 0;
    static Type_BuildingGroup = 1;
    static Type_Building = 2;
    static Type_Floor = 3;
    static Type_EquipZone = 4;

    static NewBuildingGroupName = "새 건물그룹";
    static NewBuildingName = "새 건물";

    static keyPressNone = 0;
    static keyPressControl = 1;
    static keyPressShift = 2;

    static keyPress = SpaceBody.keyPressNone;

    constructor(props) {
        super(props);
        this.state =
        {
            selectedMenu: SpaceMenus.EditBasicInfo,           
            loading: false,
            loadingMessage: "공간정보를 얻어오고 있습니다.",
            errors: null,
            _3dOptions: {},
            buildingGroupList: [],
            modelList: ModelDataManager.makeNewModelList(),
            sensorList: {},
            sensorTypes: [],
            selectedInfo: {
                buildingGroup: null,
                building: null,
                zone: null,
                equipZone: null
            },
            selectedSensorNodes: [],
            tempModelFiles: null,
            currentView: {
                buildingID: null,   // null이면 외부영역
                zoneID: null,
                zoneName: ''
            },
            command:
            {
                menu: null,
                menuParameter: null
            },
            // 공간정보 Tree에서 선택된 Node 정보
            selectedStatusInfo: {
                buildingGroup: null,
                building: null,
                zone: null,
                sensorGroups: null,
                fireSensors: null,
                psmSensors: null,
                etcSensors: null,
                cctvGroups: null,
                cctvSubGroups: null,
                facilityGroups: null,
                facilitySubGroups: null,
            }
        };

        this.onOpenXML = this.onOpenXML.bind(this);
        this.onSaveXML = this.onSaveXML.bind(this);

        this.poiManager = new POIManager(null);
    }

    componentDidMount() {
        this.requestOpenTempXML();
    }
        
    async requestOpenTempXML() {
        // temp.xml이 있다면 열기
        const result = await CommonController.requestOpenTempXML(this.props.loginData);
        if (result !== null) {
            if (!result.buildingGroups) {
                result.buildingGroups = [];                
            }

            if (!result.outdoorZones) {
                result.outdoorZones = {};
            }

            this.makeResultData(result);
        }
        else {
            this.initEmptyData();
        }
    }

    async initEmptyData() {
        const sensorList = {};
        await this.set3DOptions(sensorList);
    }

    async set3DOptions(sensorList) {
        let [buildingGroupList, outdoorZones, errorMessage] = await SpaceController.requestBuildingGroupList();

        if (!buildingGroupList && errorMessage && errorMessage.length > 0) {
            alert(errorMessage);
            return {};
        }

        const _3dOptions = await SpaceDataManager.get3DOptions(buildingGroupList, outdoorZones, null, this.props.loginData.options);

        if (buildingGroupList === null) {
            buildingGroupList = [];
        }

        if (outdoorZones === null) {
            outdoorZones = [];
        }

        this.setSensorList(_3dOptions, sensorList);
        this.setState({ loading: false, _3dOptions, buildingGroupList });
    }

    setSensorList(_3dOptions, sensorList) {
        if (!sensorList || !_3dOptions) {
            console.log('[error] sensorList가 없음');
        }
        else {
            const fireSensors = sensorList[SpaceDataManager.FireSensorType];
            const psmSensors = sensorList[SpaceDataManager.PSMSensorType];
            const etcSensors = sensorList[SpaceDataManager.EtcSensorType];
            const cctvs = sensorList[SpaceDataManager.CCTVType];

            if (fireSensors) {
                this.setFireSensors(fireSensors, _3dOptions);
            }

            if (psmSensors) {
                this.setPSMSensors(psmSensors, _3dOptions);
            }

            if (etcSensors) {
                this.setEtcSensors(etcSensors, _3dOptions);
            }

            if (cctvs) {
                this.setCCTVs(cctvs, _3dOptions);
            }
        }
    }

    setFireSensors(fireSensors, site3dOptions) {
        const sensorCount = fireSensors.length;

        for (let i = 0; i < sensorCount; i++) {
            const sensor = fireSensors[i];
            const zone = this.getZone(site3dOptions, sensor.zoneID);
            /*let zone = _3dOptions.zones[sensor.zoneID];

            if (!zone) {
                zone = _3dOptions.outdoorZones[sensor.zoneID.toString()];
            }*/

            try {
                if (zone) {
                    if (!zone.sensors.fire) {
                        zone.sensors.fire = [];
                    }

                    zone.sensors.fire.push(sensor);
                }
            } catch (e) {
                console.log('error');
            }
        }
    }

    setPSMSensors(psmSensors, site3dOptions) {
        const sensorCount = psmSensors.length;

        for (let i = 0; i < sensorCount; i++) {
            const sensor = psmSensors[i];
            const zone = this.getZone(site3dOptions, sensor.zoneID);
            /*let zone = _3dOptions.zones[sensor.zoneID];

            if (!zone) {
                zone = _3dOptions.outdoorZones[sensor.zoneID.toString()];
            }*/

            if (zone) {
                if (!zone.sensors.psm) {
                    zone.sensors.psm = [];
                }

                zone.sensors.psm.push(sensor);
            }
        }
    }

    setEtcSensors(etcSensors, site3dOptions) {
        const sensorCount = etcSensors.length;

        for (let i = 0; i < sensorCount; i++) {
            const sensor = etcSensors[i];
            const zone = this.getZone(site3dOptions, sensor.zoneID);
            /*let zone = _3dOptions.zones[sensor.zoneID];

            if (!zone) {
                zone = _3dOptions.outdoorZones[sensor.zoneID.toString()];
            }*/

            if (zone) {
                if (!zone.sensors.etc) {
                    zone.sensors.etc = [];
                }

                zone.sensors.etc.push(sensor);
            }
        }
    }

    setCCTVs(cctvs, site3dOptions) {
        const cctvCount = cctvs.length;

        for (let i = 0; i < cctvCount; i++) {
            const cctv = cctvs[i];
            const zone = this.getZone(site3dOptions, cctv.zoneID);
            /*let zone = _3dOptions.zones[cctv.zoneID];

            if (!zone && cctv.zoneID !== null && cctv.zoneID !== undefined) {
                zone = _3dOptions.outdoorZones[cctv.zoneID.toString()];
            }*/

            if (zone) {
                if (!zone.sensors.cctv) {
                    zone.sensors.cctv = [];
                }

                zone.sensors.cctv.push(cctv);
            }
        }
    }

    getZone(_3dOptions, zoneID) {         
        let zone = _3dOptions.zones[zoneID];

        if (!zone && zoneID !== null && zoneID !== undefined) {
            zone = _3dOptions.outdoorZones[zoneID.toString()];
        }

        return zone;
    }

    onSelectMenu = (menu) => {
        if (this.state.selectedMenu !== menu) {
            this.onSaveXML(true);
            this.setState({ selectedMenu: menu, selectedSensorNodes: [] });
        }
    }

    setCommand(menu, menuParam) {
        this.setState({ command: { menu: menu, menuParameter: menuParam } });
    }

    getNewEquipZoneName(zone) {
        let index = 'a';
        let sameExist = true;
        let equipZoneName = "";

        const equipZoneCount = zone.equipmentZoneDatas.length;

        while (sameExist) {
            sameExist = false;
            equipZoneName = zone.zoneName + " " + index + "구역";

            for (let i = 0; i < equipZoneCount; i++) {
                const equipZone = zone.equipmentZoneDatas[i];

                if (equipZone.zoneName === equipZoneName) {
                    sameExist = true;
                    break;
                }
            }

            index = SpaceBody.getNextCharacter(index, index.length - 1);
        }

        return equipZoneName;
    }

    static getNextCharacter(str, index) {
        const ch = str.substring(index, index + 1);

        if (ch === 'z') {
            if (index === 0) {
                return 'a' + SpaceBody.getCharacterString('a', str.length);
            }
            else {
                const sub = str.substring(0, index);
                return SpaceBody.getNextCharacter(sub, sub.length - 1) + SpaceBody.getCharacterString('a', str.length - index);
            }
        }

        return str.substring(0, index) + String.fromCharCode(ch.charCodeAt(0) + 1) + SpaceBody.getCharacterString('a', str.length - index - 1);
    }

    static getCharacterString(ch, len) {
        let str = '';

        for (let i = 0; i < len; i++) {
            str += ch;
        }

        return str;
    }

    getNewZoneName(building) {
        let index = 1;
        let sameExist = true;
        let zoneName = "";

        const zoneCount = building.zoneDatas.length;

        while (sameExist) {
            sameExist = false;
            zoneName = building.buildingName + " " + index + "층";

            for (let i = 0; i < zoneCount; i++) {
                const zone = building.zoneDatas[i];

                if (zone.zoneName === zoneName) {
                    sameExist = true;
                    break;
                }
            }

            index++;
        }

        return zoneName;
    }

    getNewBuildingName(_3dOptions) {
        let buildingName = SpaceBody.NewBuildingName;
        let index = 1;
        let sameExist = true;

        while (sameExist) {
            sameExist = false;

            for (const _buildingName in _3dOptions.allBuildings) {
                if (buildingName === _buildingName) {
                    sameExist = true;
                    buildingName = SpaceBody.NewBuildingName + "_" + index++;
                    break;
                }
            }
        }

        return buildingName;
    }

    getNewBuildingGroupName(buildingGroupList) {
        let buildingGroupName = SpaceBody.NewBuildingGroupName;
        let index = 1;
        let sameExist = true;

        while (sameExist) {
            sameExist = false;
            const buildingGroupCount = buildingGroupList.length;

            for (let i = 0; i < buildingGroupCount; i++) {
                const buildingGroup = buildingGroupList[i];

                if (buildingGroup.groupName === buildingGroupName) {
                    sameExist = true;
                    buildingGroupName = SpaceBody.NewBuildingGroupName + "_" + index++;
                    break;
                }
            }
        }

        return buildingGroupName;
    }

    getNewSiteName() {
        return "새로운 사이트";
    }

    onSelectItem = (item, itemType) => {
        const selectedInfo = this.selectItem(item, itemType);
        this.setState({ selectedInfo });
    }

    selectItem(item, itemType) {
        const selectedInfo = { ...this.state.selectedInfo };

        selectedInfo.buildingGroup = null;
        selectedInfo.building = null;
        selectedInfo.zone = null;
        selectedInfo.equipZone = null;

        if (itemType === SpaceBody.Type_BuildingGroup) {            
            selectedInfo.buildingGroup = item;
        }
        else if (itemType === SpaceBody.Type_Building) {
            if (item && item.length >= 2) {
                const building = item[0];
                const buildingGroup = item[1];

                selectedInfo.buildingGroup = buildingGroup;
                selectedInfo.building = building;
            }
        }
        else if (itemType === SpaceBody.Type_Floor) {
            if (item.length === 3) {
                // 실내 Zone
                const zone = item[0];
                const building = item[1];
                const buildingGroup = item[2];

                selectedInfo.buildingGroup = buildingGroup;
                selectedInfo.building = building;
                selectedInfo.zone = zone;
            }
            else if (item.length === 1) {
                // outdoorZone
                const zone = item[0];
                selectedInfo.zone = zone;
            }
        }
        else if (itemType === SpaceBody.Type_EquipZone) {
            selectedInfo.equipZone = item;
        }

        return selectedInfo;
    }

    onAddItem = (parent, itemType) => {
        if (itemType === SpaceBody.Type_Site) {
            if (parent === null) {
                const _3dOptions = { ...this.state._3dOptions };
                _3dOptions.siteName = this.getNewSiteName();

                const modelList = ModelDataManager.setOutdoorModel({ ...this.state.modelList });
                this.setState({ _3dOptions, modelList });
            }
        }
        else if (itemType === SpaceBody.Type_BuildingGroup) {
            if (parent === null) {
                const buildingGroupList = [...this.state.buildingGroupList];
                const buildingGroupName = this.getNewBuildingGroupName(buildingGroupList);
                const buildingGroup = SpaceDataManager.makeBuildingGroup(buildingGroupName, parent);

                if (buildingGroup) {
                    buildingGroupList.push(buildingGroup);

                    const _3dOptions = { ...this.state._3dOptions };
                    SpaceDataManager.addBuildingGroup(buildingGroup, _3dOptions);

                    const modelList = ModelDataManager.setIndoorModels({ ...this.state.modelList }, buildingGroupList);
                    this.setState({ buildingGroupList, _3dOptions, modelList });
                }
            }
        }
        else if (itemType === SpaceBody.Type_Building) {
            const buildingGroupList = [...this.state.buildingGroupList];

            if (parent === null) {
                parent = SpaceDataManager.getHiddenBuildingGroup(buildingGroupList);

                if (parent === null)
                    return;
            }

            const buildingGroup = parent;

            const _3dOptions = { ...this.state._3dOptions };
            const buildingName = this.getNewBuildingName(_3dOptions);
            SpaceDataManager.addBuilding(buildingGroup, buildingName, _3dOptions, buildingGroupList);

            const selectedInfo = this.selectItem(buildingGroup, SpaceBody.Type_BuildingGroup);

            const modelList = ModelDataManager.setIndoorModels({ ...this.state.modelList }, buildingGroupList);
            this.setState({ buildingGroupList, _3dOptions, modelList, selectedInfo });
        }
        else if (itemType === SpaceBody.Type_Floor) {
            if (parent === null) {
                return;
            }

            const building = parent;

            const buildingGroupList = [...this.state.buildingGroupList];
            const _3dOptions = { ...this.state._3dOptions };
            const zoneName = this.getNewZoneName(building);
            SpaceDataManager.addZone(building, zoneName, _3dOptions, buildingGroupList);

            const buildingGroup = SpaceDataManager.getBuildingGroupFromBuilding(building, buildingGroupList);
            const selectedInfo = this.selectItem([building, buildingGroup], SpaceBody.Type_Building);

            const modelList = ModelDataManager.setIndoorModels({ ...this.state.modelList }, buildingGroupList);
            this.setState({ buildingGroupList, _3dOptions, modelList, selectedInfo });
        }
        else if (itemType === SpaceBody.Type_EquipZone) {
            if (parent === null) {
                return;
            }

            const zone = parent;

            const buildingGroupList = [...this.state.buildingGroupList];
            const _3dOptions = { ...this.state._3dOptions };
            const equipZoneName = this.getNewEquipZoneName(zone);
            SpaceDataManager.addEquipZone(zone, equipZoneName, _3dOptions);

            let selectedInfo = null;
            const building = SpaceDataManager.getBuildingFromZone(zone, _3dOptions, buildingGroupList);

            if (building) {
                const buildingGroup = SpaceDataManager.getBuildingGroupFromBuilding(building, buildingGroupList);
                selectedInfo = this.selectItem([zone, building, buildingGroup], SpaceBody.Type_Floor);
            }
            else {
                selectedInfo = this.selectItem([zone], SpaceBody.Type_Floor);
            }

            this.setState({ buildingGroupList, _3dOptions, selectedInfo });
        }
    }

    onRemoveItem = (item, itemType) => {
        if (itemType === SpaceBody.Type_Site) {
            const _3dOptions = { ...this.state._3dOptions };
            const buildingGroupList = [...this.state.buildingGroupList];
            const modelList = ModelDataManager.makeNewModelList();

            _3dOptions.siteName = null;
            SpaceDataManager.clearBuildingGroup(buildingGroupList, _3dOptions);
            this.setState({ buildingGroupList, _3dOptions, modelList });
        }
        else if (itemType === SpaceBody.Type_BuildingGroup) {
            const _3dOptions = { ...this.state._3dOptions };
            const buildingGroupList = [...this.state.buildingGroupList];
            
            SpaceDataManager.removeBuildingGroup(item, buildingGroupList, _3dOptions);
            const modelList = ModelDataManager.setIndoorModels({ ...this.state.modelList }, buildingGroupList);
            this.setState({ buildingGroupList, _3dOptions, modelList });
        }
        else if (itemType === SpaceBody.Type_Building) {
            const _3dOptions = { ...this.state._3dOptions };
            const buildingGroupList = [...this.state.buildingGroupList];

            SpaceDataManager.removeBuilding(item, buildingGroupList, _3dOptions);
            const modelList = ModelDataManager.setIndoorModels({ ...this.state.modelList }, buildingGroupList);
            this.setState({ buildingGroupList, _3dOptions, modelList });
        }
        else if (itemType === SpaceBody.Type_Floor) {
            const _3dOptions = { ...this.state._3dOptions };
            const buildingGroupList = [...this.state.buildingGroupList];

            SpaceDataManager.removeZone(item, buildingGroupList, _3dOptions);
            const modelList = ModelDataManager.setIndoorModels({ ...this.state.modelList }, buildingGroupList);
            this.setState({ buildingGroupList, _3dOptions, modelList });
        }
        else if (itemType === SpaceBody.Type_EquipZone) {
            const _3dOptions = { ...this.state._3dOptions };
            const buildingGroupList = [...this.state.buildingGroupList];

            const equipZone = item[0];
            const zone = item[1];

            SpaceDataManager.removeEquipZone(equipZone, zone, _3dOptions);
            this.setState({ buildingGroupList, _3dOptions });
        }
    }

    onRenameItem = (item, newName, itemType) => {
        if (itemType === SpaceBody.Type_Site) {
            const _3dOptions = { ...this.state._3dOptions };
            _3dOptions.siteName = newName;
            this.setState({ _3dOptions });
        }
        else if (itemType === SpaceBody.Type_BuildingGroup) {
            const _3dOptions = { ...this.state._3dOptions };
            const buildingGroupList = [...this.state.buildingGroupList];
            SpaceDataManager.renameBuildingGroup(item, newName, buildingGroupList, _3dOptions);

            const modelList = ModelDataManager.setIndoorModels({ ...this.state.modelList }, buildingGroupList);
            this.setState({ buildingGroupList, _3dOptions, modelList });
        }
        else if (itemType === SpaceBody.Type_Building) {
            const _3dOptions = { ...this.state._3dOptions };
            const buildingGroupList = [...this.state.buildingGroupList];
            SpaceDataManager.renameBuilding(item, newName, buildingGroupList, _3dOptions);

            const modelList = ModelDataManager.setIndoorModels({ ...this.state.modelList }, buildingGroupList);
            this.setState({ buildingGroupList, _3dOptions, modelList });
        }
        else if (itemType === SpaceBody.Type_Floor) {
            const _3dOptions = { ...this.state._3dOptions };
            const buildingGroupList = [...this.state.buildingGroupList];
            SpaceDataManager.renameZone(item, newName, buildingGroupList, _3dOptions);

            const modelList = ModelDataManager.setIndoorModels({ ...this.state.modelList }, buildingGroupList);
            this.setState({ buildingGroupList, _3dOptions, modelList });
        }
        else if (itemType === SpaceBody.Type_EquipZone) {
            const _3dOptions = { ...this.state._3dOptions };
            const buildingGroupList = [...this.state.buildingGroupList];

            const equipZone = item[0];
            const zone = item[1];

            SpaceDataManager.renameEquipZone(equipZone, zone, newName, _3dOptions);
            this.setState({ buildingGroupList, _3dOptions });
        }
    }

    setGltfOption(option) {
        const loginOption = this.props.loginData.options;

        if (!loginOption) {
            return;
        }

        if (!option._3DBackgroundImage || option._3DBackgroundImage.length === 0) {
            if (loginOption._3DBackgroundImage && loginOption._3DBackgroundImage.length > 0) {
                option._3DBackgroundImage = loginOption._3DBackgroundImage;
            }
        }

        if (loginOption._3DModelBaseURL && loginOption._3DModelBaseURL.length > 0) {
            option._3DModelBaseURL = loginOption._3DModelBaseURL;
        }

        if (loginOption._3DTextureBaseURL && loginOption._3DTextureBaseURL.length > 0) {
            option._3DTextureBaseURL = loginOption._3DTextureBaseURL;
        }
    }

    async onOpenXML(event) {
        const file = event.target.files[0];
        if (!file) {
            return;
        }

        const result = await CommonController.requestOpenXML(file);
        if (!result || !result.success) {
            alert('XML 불러오기 실패 : ' + result.message);
        }

        this.makeResultData(result);
    }

    async makeResultData(result) {
        this.setGltfOption(result.gltfOption);

        const _3dOptions = await SpaceDataManager.get3DOptions(result.buildingGroups, result.outdoorZones, result.models, result.gltfOption);
        _3dOptions.siteName = result.siteName;

        let sensorList = {};
        if (result.fireSensors) {
            sensorList[SpaceDataManager.FireSensorType] = result.fireSensors;
        }

        if (result.psmSensors) {
            sensorList[SpaceDataManager.PSMSensorType] = result.psmSensors;
        }

        if (result.etcSensors) {
            sensorList[SpaceDataManager.EtcSensorType] = result.etcSensors;
        }

        if (result.cctvs) {
            sensorList[SpaceDataManager.CCTVType] = result.cctvs;
        }

        this.setSensorList(_3dOptions, sensorList);

        let modelList = ModelDataManager.setOutdoorModel({ ...this.state.modelList });
        modelList = ModelDataManager.setIndoorModels(modelList, result.buildingGroups);

        const tempModelFiles = TempModelManager.makeTempModelFilesFrom3dOptions(_3dOptions);
        ModelDataManager.setModelDatas(tempModelFiles, modelList, result.buildingGroups);

        this.setState({ _3dOptions, sensorList, buildingGroupList: result.buildingGroups, sensorTypes: result.sensorTypes, modelList, tempModelFiles });
    }

    async onSaveXML(bTempSave) {  
        const xmlData = this.makeXMLData();
        if (!xmlData) {
            return;
        }

        xmlData.bTempSave = bTempSave;
        xmlData.loginData = this.props.loginData;

        const [success, message] = await CommonController.requestSaveXML(xmlData);
        if (!xmlData.bTempSave ) {
            if (!success) {
                alert('XML 저장 실패 : ' + message);
            }
        }
    }

    makeXMLData() {
        const sensorList = this.state.sensorList;
        let fireSensors = null;
        if (sensorList[SpaceDataManager.FireSensorType]) {
            fireSensors = sensorList[SpaceDataManager.FireSensorType];
        }

        let psmSensors = null;
        if (sensorList[SpaceDataManager.PSMSensorType]) {
            psmSensors = sensorList[SpaceDataManager.PSMSensorType];
        }

        let etcSensors = null;
        if (sensorList[SpaceDataManager.EtcSensorType]) {
            etcSensors = sensorList[SpaceDataManager.EtcSensorType];
        }

        let cctvSensors = null;
        if (sensorList[SpaceDataManager.CCTVType]) {
            cctvSensors = sensorList[SpaceDataManager.CCTVType];
        }

        const models = CommonDataManager.makeModels(this.state._3dOptions.indoorModels, this.state._3dOptions.outdoorModel, this.state.modelList, this.state.buildingGroupList);        

        
        const [success, makeBuildingGroupList] = CommonDataManager.makeBuildingGroupList(this.state.buildingGroupList);
        if (!success && makeBuildingGroupList && makeBuildingGroupList.length > 0) {            
            alert(makeBuildingGroupList);
            return;
        }

        const [success2, makeOutdoorZones] = CommonDataManager.makeOutdoorZoneList(this.state._3dOptions.outdoorZones);
        if (!success2 && makeOutdoorZones && makeOutdoorZones.length > 0) {
            alert(makeOutdoorZones);
            return;
        }

        const xmlData = {
            siteName: this.state._3dOptions.siteName,
            buildingGroupList: makeBuildingGroupList,
            models: models,
            sensorTypes: this.state.sensorTypes,
            gltfOption:
            {
                _3DModelBaseURL: this.state._3dOptions.modelBaseURL,
                _3DTextureBaseURL: this.state._3dOptions.textureBaseURL,
                _3DBackgroundImage: this.state._3dOptions.backgroundImage,
                indoorModelOnMemory: this.state._3dOptions.indoorModelOnMemory,
            },
            fireSensors: fireSensors,
            psmSensors: psmSensors,
            etcSensors: etcSensors,
            cctvSensors: cctvSensors,

            outdoorZones: makeOutdoorZones,

        }

        return xmlData;
    }

    // zone에 센서를 추가할 땐 zoneID is not null, equipZoneID is null
    // equipZone에 센서를 추가할 땐 zoneID is not null, equipZoneID is not null
    onOpenExcel = (event, sensorType, zoneID, equipZoneID) => {
        ExcelManager.openSensorFile(event, sensorType, zoneID, equipZoneID, this);
    }

    onSetModelFile = (tempModelFiles) => {
        const _3dOptions = { ...this.state._3dOptions };
        const buildingGroupList = [ ...this.state.buildingGroupList ];

        SpaceDataManager.setModelFiles(tempModelFiles, _3dOptions, buildingGroupList);
        const modelList = ModelDataManager.setModelDatas(tempModelFiles, { ...this.state.modelList }, buildingGroupList);

        this.setState({ _3dOptions, buildingGroupList, modelList }, () => this.onSaveXML(true));
    }

    onChangeTempModelFiles = (tempModelFiles) => {
        this.setState({ tempModelFiles: tempModelFiles });
    }

    onChangeSensorList = (sensorType, sensors) => {
        let sensorList = {};
        if (sensorType) {
            sensorList = this.state.sensorList;
            sensorList[sensorType] = sensors;
        }
        else {
            sensorList = sensors;

        }
        this.setState({ sensorList }, () => this.onSaveXML(true));
    }

    isIndoor() {
        const currentBuildingID = this.state.currentView.buildingID;

        if (currentBuildingID !== null && currentBuildingID !== undefined) {
            return true;
        }

        return false;
    }

    setCurrentView = (zoneID) => {
        if (this.state.currentView.zoneID !== zoneID) {
            let buildingID = null;
            let zoneName = '';

            if (zoneID !== null) {
                const zone = this.state._3dOptions.zones[zoneID];

                if (zone) {
                    buildingID = zone[1];
                    zoneName = zone[3];
                }
            }

            this.setState({ currentView: { buildingID, zoneID, zoneName } });
        }
    }

    onClickLogo = () => {
        if (this.state._3dOptions.outdoorModel) {
            const cmd = {};
            cmd.menu = SpaceMenus.Menu_Show_Outdoor;
            cmd.menuParameter = this.state._3dOptions.outdoorModel;
            
            this.setState({ command: cmd });
        }
    }

    moveToX = (menu, menuParameter) => {
        if (menu === SpaceMenus.Menu_MoveTo_Site) {
            this.onClickLogo();
        }
        else if (menu === SpaceMenus.Menu_MoveTo_BuildingGroup) {
            this.onChangeBuildingGroup(menuParameter, SpaceBody.SelectedStatusInfoType.buildingGroup);
            this.setCommand(menu, [menuParameter.groupName]);
        }
        else if (menu === SpaceMenus.Menu_MoveTo_Floor) {
            this.setCommand(menu, [menuParameter.buildingID, SpaceDataManager.getZoneFloor(menuParameter)]);
        }
        else {
            this.setCommand(menu, menuParameter);
        }
    }

    onChangeBuildingGroup = (value, type) => {
        const selectedStatusInfo = this.state.selectedStatusInfo;

        selectedStatusInfo.sensorGroups = false;
        selectedStatusInfo.fireSensors = false;
        selectedStatusInfo.psmSensors = false;
        selectedStatusInfo.etcSensors = false;
        selectedStatusInfo.cctvGroups = false;
        selectedStatusInfo.cctvSubGroups = false;
        selectedStatusInfo.facilityGroups = false;
        selectedStatusInfo.facilitySubGroups = false;

        if (type === SpaceBody.SelectedStatusInfoType.buildingGroup) {
            selectedStatusInfo.buildingGroup = value;
        }
        else if (type === SpaceBody.SelectedStatusInfoType.building) {
            selectedStatusInfo.building = value;
        }
        else if (type === SpaceBody.SelectedStatusInfoType.zone) {
            selectedStatusInfo.zone = value;
        }
        else if (type === SpaceBody.SelectedStatusInfoType.sensorGroups) {
            selectedStatusInfo.sensorGroups = true;
        }
        else if (type === SpaceBody.SelectedStatusInfoType.fireSensors) {
            selectedStatusInfo.sensorGroups = true;
            selectedStatusInfo.fireSensors = true;
        }
        else if (type === SpaceBody.SelectedStatusInfoType.psmSensors) {
            selectedStatusInfo.sensorGroups = true;
            selectedStatusInfo.psmSensors = true;
        }
        else if (type === SpaceBody.SelectedStatusInfoType.etcSensors) {
            selectedStatusInfo.sensorGroups = true;
            selectedStatusInfo.etcSensors = true;
        }
        else if (type === SpaceBody.SelectedStatusInfoType.cctvGroups) {
            selectedStatusInfo.cctvGroups = true;
        }
        else if (type === SpaceBody.SelectedStatusInfoType.cctvSubGroups) {
            selectedStatusInfo.cctvGroups = true;
            selectedStatusInfo.cctvSubGroups = true;
        }
        else if (type === SpaceBody.SelectedStatusInfoType.facilityGroups) {
            selectedStatusInfo.facilityGroups = true;
        }
        else if (type === SpaceBody.SelectedStatusInfoType.facilitySubGroups) {
            selectedStatusInfo.facilityGroups = true;
            selectedStatusInfo.facilitySubGroups = true;
        }
        else if (type === SpaceBody.SelectedStatusInfoType.none) {
            if (selectedStatusInfo.buildingGroup === null)
                return;

            selectedStatusInfo.buildingGroup = null;
            selectedStatusInfo.building = null;
            selectedStatusInfo.zone = null;
        }

        this.setState({ selectedStatusInfo, selectedPOI: null });
    }

    getSpatialInfo = (zoneID) => {
        if (zoneID > 0) {
            if (zoneID >= 20000) {

                return [this.state._3dOptions.outdoorZones, this.state._3dOptions.outdoorZones, this.state._3dOptions.outdoorZones[zoneID]];
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
                                return [buildingGroup, building, zone];
                            }
                        }
                    }
                }
            }
        }

        return [null, null, null];
    }

    addSelectedSensorNodes = (sensors) => {
        const prevSensors = this.state.selectedSensorNodes;

        if (!prevSensors && !sensors) {
            return;
        }
        else if (prevSensors && sensors) {
            const prevCount = prevSensors.length;
            const currentCount = sensors.length;

            if (prevCount === currentCount) {
                let isSame = true;

                for (let i = 0; i < currentCount; i++) {
                    const currentSensor = sensors[i];
                    let find = false;

                    for (let j = 0; j < prevCount; j++) {
                        const prevSensor = prevSensors[j];

                        if (currentSensor.id === prevSensor.id && currentSensor.sensorType === prevSensor.sensorType) {
                            find = true;
                            break;
                        }
                    }

                    if (find === false) {
                        isSame = false;
                        break;
                    }
                }

                if (isSame) {
                    return;
                }
            }
        }

        this.poiManager.hideTempPOI();
        this.setState({ selectedSensorNodes: sensors });
    }

    removeSelectedSensorNodes = (sensor) => {
        if (sensor) {
            let nodes = [...this.state.selectedSensorNodes];

            let removeIndex = -1;
            const nodeCount = nodes.length;
            for (let i = 0; i < nodeCount; i++) {
                if (nodes[i] === sensor) {
                    removeIndex = i;
                }
            }
            nodes.splice(removeIndex, 1);

            this.setState({ selectedSensorNodes: nodes });
        }
        else {
            this.setState({ selectedSensorNodes: [] });
        }
    }

    render() {
        if (this.state.loading) {
            return (
                <div className={styles.bodyArea}>
                    <h2>{this.state.loadingMessage}</h2>
                </div>
            );
        }
        else if (this.state.errors) {
            return (
                <div className={styles.bodyArea}>
                    <h2>{this.state.errors}</h2>
                </div>
            );
        }

        return (
            <div className={styles.bodyArea}>
                <SpaceMenus onSelectMenu={this.onSelectMenu} selectedMenu={this.state.selectedMenu} onOpenXML={this.onOpenXML} onSaveXML={this.onSaveXML}/>
                <SpaceContents
                    selectedMenu={this.state.selectedMenu}
                    _3dOptions={this.state._3dOptions}
                    buildingGroupList={this.state.buildingGroupList}
                    tempModelFiles={this.state.tempModelFiles}
                    selectedInfo={this.state.selectedInfo}
                    loginData={this.props.loginData}
                    onAddItem={this.onAddItem}
                    onRemoveItem={this.onRemoveItem}
                    onRenameItem={this.onRenameItem}
                    onSelectItem={this.onSelectItem}
                    xmlData={this.state.xmlData}
                    onSetModelFile={this.onSetModelFile}
                    sensorTypes={this.state.sensorTypes}
                    sensorList={this.state.sensorList}
                    onChangeTempModelFiles={this.onChangeTempModelFiles}
                    onChangeSensorList={this.onChangeSensorList}
                    setCurrentView={this.setCurrentView}
                    initOutdoorViewport={this.onClickLogo}
                    currentView={this.state.currentView}
                    command={this.state.command}
                    poiManager={this.poiManager}
                    moveToX={this.moveToX}
                    getSpatialInfo={this.getSpatialInfo}
                    onChangeBuildingGroup={this.onChangeBuildingGroup}
                    selectedSensorNodes={this.state.selectedSensorNodes}
                    addSelectedSensorNodes={this.addSelectedSensorNodes}
                    removeSelectedSensorNodes={this.removeSelectedSensorNodes}
                    onSaveXML={this.onSaveXML}
                />
            </div>
        );
    }
}
