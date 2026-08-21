//import { ui } from 'jquery';
import React, { Component } from 'react';
import $ from 'jquery';
import { CommonScrollbar } from '../../Root/commonScrollbar';
import rootStyles from '../../Root/css/root.module.css';
import styles from '../css/spatial.module.css';
import spaceStyles from '../css/space.module.css';
import { BuildingGroupInfo } from './buildingGroupInfo';
/* import imgPlus from '../image/addPlus-01-01.png'; */
import imgPlus from '../image/addSimple-01.png';
import imgPlus2 from '../image/addSimple2-01-01.png';
import imgMinus from '../image/minusSimple-01.png';
import imgPencil from '../image/pencilSimple-01.png';
import { SpaceBody } from '../spaceBody';
import { BuildingInfo } from './buildingInfo';
import { SpaceDataManager } from '../services/spaceDataManager';
import { DragDropManager } from './worker/dragDropManager';
import { SpaceController } from '../services/spaceController';
import { TempModelManager } from './worker/tempModelManager';
import { SpaceMenus } from '../spaceMenus';
import { PoiHeight } from './poiHeight';
import { ModelDataManager } from '../services/modelDataManager';

export class ModelSiteInfo extends Component {
    static defaultModelingFilePathInfo = "glb 파일을 끌어다 놓으세요.";

    static Mode_SensorType = 0;
    static Mode_Sensor = 1;

    constructor(props) {
        super(props);
        this.state = {
            searchText: '',
            editMode: false,
            editText: "",
            expandNode: true,
            updateModelingFilePath: false,
            isDragging: false,
            tempModelFiles: TempModelManager.makeTempModelFiles(),
            poiMode: ModelSiteInfo.Mode_SensorType
        }

        this.refLayer = React.createRef();
        this.refScrollArea = React.createRef();
        this.refScrollbar = React.createRef();
        this.refTree = React.createRef();
        this.refEditSiteName = React.createRef();
        this.refModelingFilePath = React.createRef();
        this.refInputFile = React.createRef();
        this.refBtnApply = React.createRef();
        this.refBtnCancel = React.createRef();

        this.dragDropManager = new DragDropManager(this);
        this.tempModelManager = new TempModelManager(this);

        this.sensorTypeHeight = ModelSiteInfo.makeSensorTypeHeight();
    }

    componentDidMount() {
        this.setScrollbar();
        $('.' + rootStyles.scrollbar).scrollTop(0);

        this.dragDropManager.initDragEvents(this.refModelingFilePath.current);

        if (this.props.tempModelFiles) {
            this.updateTempModelFiles(this.props.tempModelFiles, true);
        }


        $(document).ready(function () {
            var button = $('.' + styles.moveMe);
            var bar = $('.' + styles.progressBar);
            var barWidth = bar.outerWidth();
            var clickPosition;
            var percentage = 0;
            var buttonPosition;
            $(window).resize(function () {
                barWidth = bar.outerWidth();
                setButton();
            });
            var setButton = function () {
                buttonPosition = percentage * barWidth - 10;
                button.css("width", buttonPosition + 'px');
            };
            $('.' + styles.progressBar).click(function (e) {
                clickPosition = e.pageX - $(this).offset().left;
                percentage = clickPosition / barWidth;
                setButton();
                $('.' + styles.percentage).text(Math.round(percentage * 100) + "%");
            });
        });

    }

    componentDidUpdate(prevProps, prevState) {
        this.setScrollbar();

        if (this.dragDropManager.initDragDrop === false) {
            this.dragDropManager.initDragEvents(this.refModelingFilePath.current);
        }

        if (this.state.updateModelingFilePath) {
            if (this.props.tempModelFiles) {
                this.updateTempModelFiles(this.props.tempModelFiles, false);
            }
            else {
                this.setState({ updateModelingFilePath: false });
            }
        }

        if (this.props.modeling && this.refBtnApply.current) {
            if (this.tempModelManager.isChanged()) {
                this.refBtnApply.current.disabled = false;
                this.refBtnCancel.current.disabled = false;
            }
            else {
                this.refBtnApply.current.disabled = true;
                this.refBtnCancel.current.disabled = true;
            }
        }
    }

    componentWillUnmount() {
        this.dragDropManager.resetDragEvents(this.refModelingFilePath.current);
    }

    updateTempModelFiles(tempModelFiles, update) {
        if (tempModelFiles.site) {
            this.setState({ tempModelFiles, modelingFilePath: tempModelFiles.site, updateModelingFilePath: update });
        }
        else {
            this.setState({ tempModelFiles, updateModelingFilePath: update });
        }
    }

    async onDropFiles(files) {
        if (files && files.length > 0) {
            const [success, message] = await SpaceController.requestUploadTempModelFile(files[0].object, this.props.loginData, SpaceBody.Type_Site);

            if (success) {
                const tempModelFiles = this.onSetModelFile(files[0].object.name, SpaceBody.Type_Site, null, false);
                this.setState({ modelingFilePath: files[0].object.name, tempModelFiles });
            }
            else if (message && message.length > 0) {
                alert(message);
            }
        }
    }

    setScrollbar() {
        const treeArea = this.refScrollArea.current.getBoundingClientRect();

        let scrollVisible = false;

        if (this.refTree.current) {
            const rectTree = this.refTree.current.getBoundingClientRect();

            if (rectTree.height > treeArea.height) {
                scrollVisible = true;
            }
        }

        CommonScrollbar.setContentStyle(this.refScrollbar.current, treeArea.width, treeArea.height, scrollVisible);

        //const treeArea2 = this.refScrollArea.current.getBoundingClientRect();

        if (this.props.selectedInfo && this.props.selectedInfo.buildingGroup) {

        }
    }

    // node가 보이는 범위내에 있는지 체크한다
    checkRange(titleID, areaID, targetRect) {
        const titleEle = document.getElementById(titleID);
        const areaEle = document.getElementById(areaID);
        if (!titleEle || !areaEle) {
            return false;
        }

        const titleRect = titleEle.getBoundingClientRect();
        const areaRect = areaEle.getBoundingClientRect();

        const beginY = targetRect.y;
        const endY = targetRect.y + targetRect.height;
        if (titleRect.top >= beginY && areaRect.bottom <= endY) {
            // 범위내에 있음
            return false;
        }

        return true;
    }

    searchEnterKey = () => {
        if (window.event && window.event.keyCode === 13) {
            this.search();
        }
    }

    search = () => {
        const text = document.getElementById('txtSearch').value;
        this.setState({ searchText: text });
    }

    getBuildingUI(ui, buildingGroup) {
        const buildingDatas = buildingGroup.buildingDatas;
        if (buildingDatas === undefined || buildingDatas === null || buildingDatas.length === 0)
            return;

        for (let i = 0; i < buildingDatas.length; i++) {
            const building = buildingDatas[i];

            ui.push(
                <BuildingInfo
                    key={'building_' + building.id}
                    building={building}
                    buildingIDs={this.props._3dOptions.buildingIDs}
                    selectedInfo={this.props.selectedInfo}
                    onAddItem={this.props.onAddItem}
                    onRemoveItem={this.props.onRemoveItem}
                    onRenameItem={this.props.onRenameItem}
                    onSelectItem={this.props.onSelectItem}
                />
            );
        }
    }

    getBuildingGroupUI(siteExist) {
        let ui = [];
        let buildingGroupList = this.props.buildingGroupList;
        if (buildingGroupList === undefined || buildingGroupList === null || buildingGroupList.length === 0 || siteExist === false)
            return ui;

        if (this.state.searchText.length > 0) {
            this.setVisibleBuildingGroupList(buildingGroupList);
        }

        for (let i = 0; i < buildingGroupList.length; i++) {
            const buildingGroup = buildingGroupList[i];
            if (this.state.searchText.length > 0)
                continue;

            if (buildingGroup.visible === false) {
                // 건물그룹없이 Site 바로 아래에 위치하는 건물들을 가져온다.
                this.getBuildingUI(ui, buildingGroup);
                continue;
            }

            ui.push(<BuildingGroupInfo
                key={buildingGroup.id}
                _3dOptions={this.props._3dOptions}
                buildingGroup={buildingGroup}
                selectedInfo={this.props.selectedInfo}
                isOutdoor={false}
                modeling={this.props.modeling}
                dashboard={this.props.dashboard}
                loginData={this.props.loginData}
                updateModelingFilePath={this.state.updateModelingFilePath}
                tempModelFiles={this.state.tempModelFiles}
                onAddItem={this.props.onAddItem}
                onRemoveItem={this.props.onRemoveItem}
                onRenameItem={this.props.onRenameItem}
                onSelectItem={this.props.onSelectItem}
                onSetModelFile={this.onSetModelFile}
                onClickClear={this.onClickClear}
                moveToX={this.props.moveToX}
            />);
        }

        if (this.props.outdoorZones) {
            ui.push(<BuildingGroupInfo
                key={"bg_outdoor"}
                _3dOptions={this.props._3dOptions}
                buildingGroup={this.props._3dOptions.outdoorZones}
                selectedInfo={this.props.selectedInfo}
                isOutdoor={true}
                modeling={this.props.modeling}
                dashboard={this.props.dashboard}
                loginData={this.props.loginData}
                updateModelingFilePath={this.state.updateModelingFilePath}
                tempModelFiles={this.state.tempModelFiles}
                onAddItem={this.props.onAddItem}
                onRemoveItem={this.props.onRemoveItem}
                onRenameItem={this.props.onRenameItem}
                onSelectItem={this.props.onSelectItem}
                onSetModelFile={this.onSetModelFile}
                onClickClear={this.onClickClear}
                moveToX={this.props.moveToX}
            />);
        }

        return ui;
    }

    setVisibleBuildingGroupList(buildingGroupList) {
        let count = buildingGroupList.length;
        for (let i = 0; i < count; i++) {
            const buildingGroup = buildingGroupList[i];
            if (buildingGroup.displayText.includes(this.state.searchText)) {
                buildingGroup.visible = true;
            }
            else {
                let visible = false;
                for (let i = 0; i < buildingGroup.buildingDatas.length; i++) {
                    const buildingVisible = this.setVisibleBuildingList(buildingGroup.buildingDatas[i]);
                    if (buildingVisible === true) {
                        visible = true;
                    }
                }

                if (visible) {
                    buildingGroup.visible = true;
                }
                else {
                    buildingGroup.visible = false;
                }
            }
        }
    }

    setVisibleBuildingList(buildingData) {
        let buildingVisible = false;

        let count = buildingData.zoneDatas.length;
        for (let i = 0; i < count; i++) {
            const zone = buildingData.zoneDatas[i];
            if (zone.displayText.includes(this.state.searchText)) {
                zone.visible = true;
                buildingVisible = true;
            }
            else {
                let visibleCount = 0;
                if (this.setVisibleSensors(zone.id, this.props.sensorList.fireSensors)) {
                    visibleCount++;
                }
                if (this.setVisibleSensors(zone.id, this.props.sensorList.etcSensors)) {
                    visibleCount++;
                }
                if (this.setVisibleSensors(zone.id, this.props.sensorList.cctvs)) {
                    visibleCount++;
                }
                if (this.setVisiblePsmSensors(zone.id, this.props.sensorList.psmSensors)) {
                    visibleCount++;
                }

                if (visibleCount > 0) {
                    zone.visible = true;
                }
                else {
                    zone.visible = false;
                }

                if (zone.visible) {
                    buildingVisible = true;
                }
            }
        }

        buildingData.visible = buildingVisible;

        return buildingData.visible;
    }

    setVisibleSensors(zoneID, sensors) {
        let visible2 = false;
        const sensorsCount = sensors.length;
        for (let j = 0; j < sensorsCount; j++) {
            const sensor = sensors[j];
            if (zoneID !== sensor.zoneID)
                continue;

            if (sensor.name.includes(this.state.searchText)) {
                sensor.visible = true;
                visible2 = true;
            }
            else {
                sensor.visible = false;
            }
        }

        return visible2;
    }

    setVisiblePsmSensors(zoneID, sensors) {
        let visible2 = false;
        const sensorsCount = sensors.length;
        for (let i = 0; i < sensorsCount; i++) {
            const sensor = sensors[i];
            if (!sensor.linkedZones)
                continue;

            for (var j = 0; j < sensor.linkedZones.length; j++) {
                if (sensor.linkedZones[j].id !== zoneID)
                    continue;

                if (sensor.name.includes(this.state.searchText)) {
                    sensor.visible = true;
                    visible2 = true;
                }
                else {
                    sensor.visible = false;
                }
                break;
            }
        }
        return visible2;
    }

    onClickAdd = (e, type) => {
        if (type === SpaceBody.Type_BuildingGroup && this.hasNoParentBuilding()) {
            alert("건물그룹이 없는 건물들이 존재합니다.\r\n일반 건물그룹을 추가하려면 먼저 이 건물들을 삭제해야 합니다.");
        }
        else if (type === SpaceBody.Type_Building && this.hasNormalBuildingGroup()) {
            alert("일반 건물그룹들이 존재합니다.\r\n건물그룹이 없는 건물을 추가하려면 먼저 이 건물그룹들을 삭제해야 합니다.");
        }
        else {
            this.props.onAddItem(null, type);
        }
    }

    onClickRemove = (parent, type) => {
        this.props.onRemoveItem(null, type);
    }

    onClickEdit(e, type) {
        const siteName = this.props._3dOptions.siteName;

        if (!siteName) {
            return;
        }

        this.setState({ editMode: true, editText: siteName.trim() });

        setTimeout(() => {
            if (this.state.editMode && this.refEditSiteName.current) {
                this.refEditSiteName.current.focus();
            }
        }, 300);
    }

    onClickClear = (object, id, type) => {
        this.initModelFilePath(object, id, type);
    }

    /* async initModelFilePath(object, id, type) {
        const [success, message] = await SpaceController.requestRemoveTempFile(this.props.loginData, object.state.modelingFilePath);

        if (success) {
            const tempModelFiles = this.tempModelManager.initData(id, type);

            if (this === object) {
                object.setState({ modelingFilePath: SiteInfo.defaultModelingFilePathInfo, tempModelFiles });
            }
            else {
                object.setState({ modelingFilePath: SiteInfo.defaultModelingFilePathInfo });
                this.setState({ tempModelFiles });
            }
        }
        else if (message !== null && message.length > 0) {
            alert(message);
        }
    } */

    onKeyUp(e) {
        if (e.key === "Enter") {
            this.renameSite();
        }
        else if (e.key === "Escape") {
            this.setState({ editMode: false });
        }
    }

    onChange(e) {
        const editText = e.target.value;
        this.setState({ editText });
    }

    onFocusout(e) {
        this.renameSite();
    }

    renameSite() {
        this.props.onRenameItem(null, this.state.editText.trim(), SpaceBody.Type_Site);
        this.setState({ editMode: false });
    }

    showChildElement(e) {
        this.setState({ expandNode: !this.state.expandNode });
    }

    isExistSite() {
        const siteName = this.props._3dOptions.siteName;

        if (siteName) {
            if (siteName.trim().length > 0) {
                return true;
            }
        }

        return false;
    }

    hasNormalBuildingGroup() {
        const buildingGroupList = [...this.props.buildingGroupList];
        const buildingGroupCount = buildingGroupList.length;

        for (let i = 0; i < buildingGroupCount; i++) {
            const buildingGroup = buildingGroupList[i];

            if (buildingGroup.visible)
                return true;
        }

        return false;
    }

    // 건물그룹을 부모로 두지않는 건물들이 존재하는가?
    hasNoParentBuilding() {
        const buildingGroupList = [...this.props.buildingGroupList];
        const buildingGroup = SpaceDataManager.getHiddenBuildingGroup(buildingGroupList);

        if (buildingGroup) {
            return buildingGroup.buildingDatas.length > 0;
        }

        return false;
    }

    onSetModelFile = (fileName, type, id, update = true) => {
        return this.tempModelManager.onSetModelFile(fileName, type, id, update);
    }

    async onClickApply() {
        const [success, message] = await SpaceController.requestUploadModelFile(this.props.loginData, this.props._3dOptions);

        if (success) {
            const tempModelFiles = { ...this.state.tempModelFiles };
            this.props.onSetModelFile(tempModelFiles);
            alert("적용되었습니다.");
        }
        else {
            if (message && message.length > 0) {
                alert(message);
            }
        }
    }

    async onClickCancel() {
        const [success, message] = await SpaceController.requestClearTempModelFiles(this.props.loginData);

        if (success) {
            const tempModelFiles = { ...this.state.tempModelFiles };
            const _3dOptions = { ...this.props._3dOptions };
            this.tempModelManager.rollbackTempModelFiles(_3dOptions, tempModelFiles);
        }
    }

    moveToX = () => {
        this.props.moveToX(SpaceMenus.Menu_MoveTo_Site, null);
    }

    getButtonImages() {
        if (!this.props.modeling && !this.props.dashboard) {
            return (
                <div className={styles.treeIconImageArea}>
                    <img className={styles.treeIconImage} src={imgMinus} alt="icon" onClick={(e) => this.onClickRemove(e, SpaceBody.Type_Site)} />
                    <img className={styles.treeIconImage} src={imgPlus} alt="icon" onClick={(e) => this.onClickAdd(e, SpaceBody.Type_Building)} />
                    <img className={styles.treeIconImage} src={imgPlus2} alt="icon" onClick={(e) => this.onClickAdd(e, SpaceBody.Type_BuildingGroup)} />
                    <img className={styles.treeIconImage} src={imgPencil} alt="icon" onClick={(e) => this.onClickEdit(e, SpaceBody.Type_Site)} />
                </div>
            );
        }

        if (this.props.dashboard) {
            return (
                <div className={styles.treeIconImageArea}>
                    <span className={styles.goLink} onClick={this.moveToX} style={{ cursor: 'pointer' }}><a>이동</a></span>
                </div>
            );
        }

        return <></>;
    }

    onClickMode(mode) {
        this.setState({ poiMode: mode });
    }

    getOutdoorZone() {
        const outdoorZones = this.props._3dOptions?.outdoorZones;

        if (outdoorZones) {
            for (const zoneID in outdoorZones) {
                const zone = outdoorZones[zoneID];
                return zone;
            }
        }

        return null;
    }

    getOutdoorModelDefaultPoiHeight() {
        const outdoorZone = this.getOutdoorZone();

        if (outdoorZone?.datas) {
            const poiElevation = outdoorZone.datas.poiElevation;

            if (poiElevation !== null && poiElevation !== undefined) {
                return poiElevation;
            }
        }

        const _3dOptions = this.props._3dOptions;

        if (_3dOptions) {
            const outdoorModel = _3dOptions[ModelDataManager.OutdoorModelName];

            if (outdoorModel?.camera?.targetControl) {
                const elevation = outdoorModel?.camera?.targetControl[1] + 10;

                if (outdoorZone) {
                    if (!outdoorZone.datas) {
                        outdoorZone.datas = {};
                    }

                    outdoorZone.datas.poiElevation = elevation;
                }

                return elevation;
            }
        }

        return null;
    }

    getZoneDefaultPoiHeight(zone, zoneID) {
        const _3dOptions = this.props._3dOptions;

        if (_3dOptions) {
            const zoneData = SpaceDataManager.findZone(zoneID, _3dOptions);

            if (zoneData?.datas) {
                const poiElevation = zoneData.datas.poiElevation;

                if (poiElevation !== null && poiElevation !== undefined) {
                    return poiElevation;
                }
            }

            const indoorModels = _3dOptions[ModelDataManager.IndoorModelName];

            if (indoorModels) {
                const buildingID = zone[1];

                if (buildingID !== undefined && buildingID !== null) {
                    const buildingData = _3dOptions.buildingIDs[buildingID];

                    if (buildingData.length >= 3) {
                        const buildingGroupName = buildingData[1];
                        const buildingName = buildingData[2];
                        const buildingGroupModel = indoorModels[buildingGroupName];

                        if (buildingGroupModel) {
                            const buildingModel = buildingGroupModel[buildingName];
                            const floors = buildingModel?.floors;

                            if (floors) {
                                for (const zoneModel of floors) {
                                    if (zoneModel.zoneID === zoneID) {
                                        if (zoneModel.camera?.targetControl) {
                                            const elevation = zoneModel.camera.targetControl[1] + 10;

                                            if (zoneData) {
                                                if (!zoneData.datas) {
                                                    zoneData.datas = {};
                                                }

                                                zoneData.datas.poiElevation = elevation;
                                            }

                                            return elevation;
                                        }

                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return null;
    }

    static makeSensorTypeHeight() {
        const sensorTypeHeight = {};

        sensorTypeHeight[SpaceDataManager.FireSensorType] = null;
        sensorTypeHeight[SpaceDataManager.PSMSensorType] = null;
        sensorTypeHeight[SpaceDataManager.EtcSensorType] = null;
        sensorTypeHeight[SpaceDataManager.CCTVType] = null;

        return sensorTypeHeight;
    }

    getSensorTypeHeight() {
        const currentView = this.props.currentView;
        const sensorTypeHeight = { ...this.sensorTypeHeight };

        sensorTypeHeight[SpaceDataManager.FireSensorType] = null;
        sensorTypeHeight[SpaceDataManager.PSMSensorType] = null;
        sensorTypeHeight[SpaceDataManager.EtcSensorType] = null;
        sensorTypeHeight[SpaceDataManager.CCTVType] = null;

        if (currentView) {
            if (currentView.zoneID === null || currentView.zoneID === undefined) {
                const outdoorZone = this.getOutdoorZone();
                this.setSensorTypeHeight(sensorTypeHeight, outdoorZone, this.getOutdoorModelDefaultPoiHeight());
            }
            else {
                const zone = this.props._3dOptions.zones[currentView.zoneID];
                this.setSensorTypeHeight(sensorTypeHeight, zone, this.getZoneDefaultPoiHeight(zone, currentView.zoneID));
            }
        }

        return sensorTypeHeight;
    }

    static hasXYZ(sensor) {
        if (sensor.x !== null && sensor.x !== undefined &&
            sensor.y !== null && sensor.y !== undefined &&
            sensor.z !== null && sensor.z !== undefined) {
            return true;
        }

        return false;
    }

    setSensorTypeHeight(sensorTypeHeight, zone, defaultHeight) {
        if (zone?.sensors) {
            for (const sensorType in zone.sensors) {
                const sensors = zone.sensors[sensorType];

                for (const sensor of sensors) {
                    if (ModelSiteInfo.hasXYZ(sensor)) {
                        sensorTypeHeight[sensorType] = sensor.y;
                        break;
                    }
                }
            }
        }

        for (const sensorType in sensorTypeHeight) {
            const height = sensorTypeHeight[sensorType];

            if (height === null || height === undefined) {
                sensorTypeHeight[sensorType] = defaultHeight.toFixed(2);
            }
        }
    }

    onChangeSensorTypeHeight = (sensorTypeHeight) => {
        let firstElevation = null;

        for (const sensorType in sensorTypeHeight) {
            const height = sensorTypeHeight[sensorType];

            if (height !== null && height !== undefined) {
                const elevation = parseFloat(height.toString());
                this.sensorTypeHeight[sensorType] = elevation;

                if (firstElevation === null) {
                    firstElevation = elevation;
                }
            }
        }

        const poiManager = this.props.poiManager;

        if (poiManager) {
            poiManager.setPoiElevation(this.sensorTypeHeight);
        }

        const currentView = this.props.currentView;

        if (currentView && firstElevation !== null) {
            let zone = null;

            if (currentView.zoneID === null || currentView.zoneID === undefined) {
                zone = this.getOutdoorZone();
            }
            else {
                zone = this.props._3dOptions.zones[currentView.zoneID];
            }

            if (zone) {
                if (!zone.datas) {
                    zone.datas = {};
                }

                zone.datas.poiElevation = firstElevation;
                this.setZoneSensorHeight(zone, this.sensorTypeHeight);
            }
        }
    }

    setZoneSensorHeight(zone, sensorTypeHeight) {
        const sensors = zone?.sensors;

        if (sensors) {
            for (const sensorType in sensors) {
                const elevation = sensorTypeHeight[sensorType];

                if (elevation !== null && elevation !== undefined) {
                    const sensorDatas = sensors[sensorType];

                    for (const sensor of sensorDatas) {
                        if (ModelSiteInfo.hasXYZ(sensor)) {
                            sensor.y = elevation;
                        }
                    }
                }
            }
        }
    }

    render() {
        const siteExist = this.isExistSite();
        const buildingGroupUI = this.getBuildingGroupUI(siteExist);
        const treeNodeStyle = styles.viewListHeadWrap;
        const scrollAreaStyle = this.props.modeling ? styles.dsiScr + " " + styles.short + " " + rootStyles.scrollbar : styles.dsiScr + " " + rootStyles.scrollbar;
        const className = this.props.dashboard ? styles.spaceArea + " " + styles.spaceAreaDashboard : styles.spaceArea;
        const poiMode = this.state.poiMode;
        this.sensorTypeHeight = this.getSensorTypeHeight();

        return (
            <>
                <span className={styles.modellogoAreaTitle}>공간정보 등록</span>
                <div className={className}>
                    <div className={styles.modelInfoContent}>

                        <span className={styles.modelInfoBox}>
                            <span className={styles.modelInfoTitle}>3D 모델 뷰어</span>
                            <div className={styles.areaSearchBox}>
                                <div className={styles.leftChild}>
                                    <input type="text" id="txtSearch" onKeyUp={this.searchEnterKey} className={styles.areaSelect} />
                                </div>
                            </div>
                        </span>

                        <div ref={this.refScrollArea} className={scrollAreaStyle}>
                            {
                                <ul ref={this.refTree} className={styles.dsiTree}>
                                    <li>
                                        {
                                            siteExist &&
                                            <div className={treeNodeStyle} id={styles.treePadding}>
                                                {
                                                    !this.state.editMode &&
                                                    <>
                                                        <span className={styles.viewListSite} onClick={(e) => { this.showChildElement(e) }}>{this.props._3dOptions.siteName}</span>
                                                        {
                                                            !this.props.modeling &&
                                                            <div className={styles.treeIconImageArea}>
                                                              <span className={styles.goLink} onClick={this.moveToX} style={{ cursor: 'pointer' }}><a>이동</a></span>
                                                            </div>
                                                        }
                                                    </>
                                                }
                                                {
                                                    this.state.editMode &&
                                                    <input ref={this.refEditSiteName} type="text" value={this.state.editText} onKeyUp={(e) => this.onKeyUp(e)} onChange={(e) => this.onChange(e)} onBlur={(e) => this.onFocusout(e)} />
                                                }
                                            </div>
                                        }
                                        <div>
                                            {
                                                this.state.expandNode &&
                                                <ul className={styles.buildingGroupUL}>
                                                    {buildingGroupUI}
                                                </ul>
                                            }
                                        </div>
                                    </li>
                                </ul>
                            }
                        </div>
                        {
                            this.props.modeling &&
                            <div className={styles.modelBtnBox}>
                                <button ref={this.refBtnApply} className={styles.btnRight + " " + styles.rightMargin} onClick={() => this.onClickCancel()}>취소</button>
                                <button ref={this.refBtnCancel} className={styles.btnRight + " " + styles.leftMargin} onClick={() => this.onClickApply()}>적용</button>
                            </div>
                        }
                    </div>
                    <div className={styles.modelInfoContentBottom}>
                        <div className={spaceStyles.poiListTitleBox}>
                            <span className={spaceStyles.poiListTitleText}>POI 높이 설정</span>
                            <ul className={spaceStyles.poiRadioBox2}>
                            {/*
                                <li><input type="radio" className={spaceStyles.poiRadioCtrl} onChange={() => this.onClickMode(ModelSiteInfo.Mode_SensorType)} checked={poiMode === ModelSiteInfo.Mode_SensorType} /> <label htmlFor="checkIcon" className={spaceStyles.poiRadioText}>일괄적용</label></li>
                                <li><input type="radio" className={spaceStyles.poiRadioCtrl} onChange={() => this.onClickMode(ModelSiteInfo.Mode_Sensor)} checked={poiMode === ModelSiteInfo.Mode_Sensor} /> <label htmlFor="moveIcon" className={spaceStyles.poiRadioText}>개별적용</label></li>
                            */}
                            </ul>
                            <div className={spaceStyles.poiListContent}>
                                {
                                    /*poiMode === ModelSiteInfo.Mode_SensorType &&*/
                                    <PoiHeight sensorTypeHeight={this.sensorTypeHeight} onChangeSensorTypeHeight={this.onChangeSensorTypeHeight}/>
                                }
                            </div>
                        </div>
                    </div>
                </div>
            </>
        );
    }
}