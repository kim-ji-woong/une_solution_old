import React, { Component } from 'react';
import styles from '../css/spatial.module.css';
import { SpaceBody } from '../spaceBody';
import imgPlus from '../image/addSimple-01.png';
import imgMinus from '../image/minusSimple-01.png';
import imgPencil from '../image/pencilSimple-01.png';
import { ZoneInfo } from './zoneInfo';
import { DragDropManager } from './worker/dragDropManager';
import { SpaceController } from '../services/spaceController';
import { SiteInfo } from './siteInfo';

export class BuildingInfo extends Component {
    constructor(props) {
        super(props);

        this.state = {
            editMode: false,
            editText: "",
            modelingFilePath: SiteInfo.defaultModelingFilePathInfo,
            expandNode: true,
            instance: this
        }

        this.refBuildingName = React.createRef();
        this.refZoneList = React.createRef();
        this.refEditBuildingName = React.createRef();

        this.refModelingFilePath = React.createRef();
        this.refInputFile = React.createRef();
        this.refInputFile2 = React.createRef();

        this.dragDropManager = new DragDropManager(this);
        this.updateModelingFilePath = false;
    }

    componentDidMount() {
        this.checkChildVisible();
        this.dragDropManager.initDragEvents(this.refModelingFilePath.current);
    }

    componentDidUpdate(prevProps, prevState) {
        this.checkChildVisible();

        if (this.state.editMode && this.refEditBuildingName.current) {
            this.refEditBuildingName.current.focus();
        }

        if (this.dragDropManager.initDragDrop === false) {
            this.dragDropManager.initDragEvents(this.refModelingFilePath.current);
        }
    }

    componentWillUnmount() {
        this.dragDropManager.resetDragEvents(this.refModelingFilePath.current);
    }

    static getDerivedStateFromProps(props, state) {
        if (props === state.prevProps) {
            return state;
        }

        if (props.updateModelingFilePath) {
            state.instance.updateModelingFilePath = true;

            return {
                editMode: state.editMode,
                editText: state.editText,
                modelingFilePath: BuildingInfo.getModelingFilePathFromProps(props, state),
                expandNode: state.expandNode,
                instance: state.instance
            };
        }

        return state;
    }

    static getModelingFilePathFromProps(props, state) {
        const buildings = props.tempModelFiles?.buildings;

        if (buildings) {
            const buildingID = props.building?.id ? props.building.id : -1;
            const modelingFilePath = buildings[buildingID];

            if (modelingFilePath) {
                return modelingFilePath;
            }
        }

        return state.modelingFilePath;
    }

    async onDropFiles(files) {
        if (files && files.length > 0) {
            const [success, message] = await SpaceController.requestUploadTempModelFile(files[0].object, this.props.loginData, SpaceBody.Type_Building);

            if (success) {
                this.props.onSetModelFile(files[0].object.name, SpaceBody.Type_Building, this.props.building?.id);
                this.setState({ modelingFilePath: files[0].object.name });
            }
            else if (message && message.length > 0) {
                alert(message);
            }
        }
    }

    checkChildVisible() {
        if (this.refBuildingName.current) {
            if (this.state.expandNode) {
                if (this.refBuildingName.current.dataset.show_child !== 'true') {
                    this.refBuildingName.current.dataset.show_child = 'true';
                }

                if (this.refZoneList.current.classList.contains(styles.on) === false) {
                    this.refZoneList.current.classList.add(styles.on);
                }
            }
            else {
                if (this.refBuildingName.current.dataset.show_child !== 'false') {
                    this.refBuildingName.current.dataset.show_child = 'false';
                }

                if (this.refZoneList.current.classList.contains(styles.on)) {
                    this.refZoneList.current.classList.remove(styles.on);
                }
            }
        }
    }

    showChild(e) {
        this.setState({ expandNode: !this.state.expandNode });
    }

    isSelected() {
        if (this.props.selectedInfo) {
            if (this.props.selectedInfo.building === this.props.building) {
                return true;
            }
            else {
                return false;
            }
        }

        const [sensorType, zoneID, sensorID] = this.props.selectedSensor;

        if (this.prevSelectedSensor[0] !== sensorType ||
            this.prevSelectedSensor[1] !== zoneID ||
            this.prevSelectedSensor[2] !== sensorID) {
        }

        this.prevSelectedSensor = [sensorType, zoneID, sensorID];

        if (sensorType !== null && zoneID !== null && sensorID !== null) {
            if (this.props.building.zoneDatas) {
                const buildingData = this.props.building;

                const zoneCount = buildingData.zoneDatas.length;

                for (let i = 0; i < zoneCount; i++) {
                    const zoneData = buildingData.zoneDatas[i];

                    if (zoneData.id === zoneID) {
                        return true;
                    }
                }
            }
            else {
                const outdoorZones = this.props.building;
                const zoneIDString = zoneID.toString();

                for (const outdoorZoneID in outdoorZones) {
                    if (outdoorZoneID === zoneIDString) {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    getZoneUI() {
        let ui = [];

        if (this.props.building.zoneDatas) {
            const zoneDatas = this.props.building.zoneDatas;
            if (zoneDatas === undefined || zoneDatas === null || zoneDatas.length === 0)
                return ui;

            this.setZoneUI(zoneDatas, ui);
        }
        else {
            const outdoorZones = this.props.building;
            const zoneDatas = [];

            for (const zoneID in outdoorZones) {
                const zoneData = outdoorZones[zoneID];
                zoneDatas.push(zoneData);
            }

            if (zoneDatas === undefined || zoneDatas === null || zoneDatas.length === 0)
                return ui;

            this.setZoneUI(zoneDatas, ui);
        }

        return ui;
    }

    setZoneUI(zoneDatas, ui) {
        const hasIndoorModel = this.hasFloorModel(this.props.building.id);

        for (let i = 0; i < zoneDatas.length; i++) {
            const zone = zoneDatas[i];

            if ((zone.visible === false && this.props.searchText.length > 0) || zone.floorIndex === null)
                continue;

            ui.push(
                <ZoneInfo
                    key={'zone_' + zone.id}
                    zone={zone}
                    sensorList={this.props.sensorList}
                    selectedInfo={this.props.selectedInfo}
                    selectedSensor={[null, null, null]}
                    searchText={this.props.searchText}
                    isEditMode={this.props.isEditMode}
                    hasIndoorModel={hasIndoorModel}
                    modeling={this.props.modeling}
                    dashboard={this.props.dashboard}
                    loginData={this.props.loginData}
                    updateModelingFilePath={this.updateModelingFilePath}
                    tempModelFiles={this.props.tempModelFiles}
                    onAddItem={this.props.onAddItem}
                    onRemoveItem={this.props.onRemoveItem}
                    onRenameItem={this.props.onRenameItem}
                    onSelectItem={this.props.onSelectItem}
                    onSetModelFile={this.props.onSetModelFile}
                    onClickClear={this.props.onClickClear}
                    moveToX={this.props.moveToX}
                />
            );
        }

        this.updateModelingFilePath = false;
    }

    hasFloorModel = (buildingID, floorIndex) => {
        if (!buildingID) {
            return false;
        }

        const building = this.props.buildingIDs[buildingID.toString()];

        if (building) {
            const buildingName = building[2];
            const buildingGroupID = this.props.building?.buildingGroupID;
            /*const indoorModels = this.props.indoorModels;

            for (const modelName in indoorModels) {
                const buildingGroup = indoorModels[modelName];

                if (buildingGroup.buildingGroupID !== buildingGroupID) {
                    continue;
                }

                //const buildingGroup = this.getBuildingGroupModel(buildingGroupID, this.props.indoorModels);

                if (buildingGroup) {
                    let buildingData = buildingGroup[buildingName];

                    if (!buildingData) {
                        buildingData = this.getBuildingDataFromDisplayText(buildingName, buildingGroup);
                    }

                    if (buildingData && buildingData.floors) {
                        return true;
                    }
                }
            }*/
        }

        return false;
    }

    onClickAdd(e, type) {
        const building = this.props.building;

        if (!building) {
            return;
        }

        this.props.onAddItem(building, type);
        this.setState({ expandNode: true });
    }

    onClickRemove(e, type) {
        const building = this.props.building;

        if (!building) {
            return;
        }

        this.props.onRemoveItem(building, type);
    }

    onClickEdit(e, type) {
        const building = this.props.building;

        if (!building) {
            return;
        }

        this.setState({ editMode: true, editText: building.buildingName });
    }

    renameBuildingName() {
        this.props.onRenameItem(this.props.building, this.state.editText.trim(), SpaceBody.Type_Building);
        this.setState({ editMode: false });
    }

    onKeyUp(e) {
        if (e.key === "Enter") {
            this.renameBuildingName();
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
        this.renameBuildingName();
    }

    moveToX = (e) => {

    }

    getButtonImages() {
        if (!this.props.modeling && !this.props.dashboard) {
            return (
                <div className={styles.treeIconImageArea}>
                    <img className={styles.treeIconImage} src={imgMinus} alt="icon" onClick={(e) => this.onClickRemove(e, SpaceBody.Type_Building)} />
                    <img className={styles.treeIconImage} src={imgPlus} alt="icon" onClick={(e) => this.onClickAdd(e, SpaceBody.Type_Floor)} />
                    <img className={styles.treeIconImage} src={imgPencil} alt="icon" onClick={(e) => this.onClickEdit(e, SpaceBody.Type_Building)} />
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

    onClickSelectFile = () => {
        this.refInputFile2.current.click();
    }

    selectFile(file) {
        this.onDropFiles([{ object: file }]);
    }

    render() {
        let zoneUI = this.getZoneUI();
        let listClassName = styles.viewList2Depth;
        let showChild = 'false';
        
        if (this.state.expandNode) {
            listClassName += " " + styles.on;
            showChild = 'true';
        }

        const buildingName = this.props.building.displayText ? this.props.building.displayText : this.props.building.buildingName;
        const buildingID = this.props.building?.id ? this.props.building.id : -1;
        const buildingClassName = this.state.editMode ? styles.viewList1Depth + " " + styles.hidden : styles.viewList1Depth;

        return (
            <li>
                <div className={styles.viewListDepthParent}>
                    <span className={styles.testtt}> {/* test */}
                    <div id={this.props.id} ref={this.refBuildingName}
                        className={buildingClassName}
                        data-show_child={showChild}
                        data-target_class='viewList1Depth'
                        onClick={(e) => { this.showChild(e) }}>
                        {buildingName}
                        {
                            this.props.modeling &&
                            <div className={styles.dragDrop}>
                                <div ref={this.refModelingFilePath} className={styles.modelingInput}>{this.state.modelingFilePath}</div>
                                <input ref={this.refInputFile} type="file" className={styles.hidden} onChange={() => { }} />
                                <img className={styles.treeIconImage} src={imgMinus} alt="icon" onClick={() => this.props.onClickClear(this, buildingID, SpaceBody.Type_Building)} />
                                <img className={styles.treeIconImage} src={imgPlus} alt="icon" onClick={() => this.onClickSelectFile()} />
                                <input ref={this.refInputFile2} className={styles.hidden} onChange={(e) => this.selectFile(e.target.files[0])} type="file" />
                            </div>
                        }
                        </div>
                     </span>
                    {
                        this.state.editMode &&
                        <input ref={this.refEditBuildingName} type="text" value={this.state.editText} onKeyUp={(e) => this.onKeyUp(e)} onChange={(e) => this.onChange(e)} onBlur={(e) => this.onFocusout(e)} />
                    }
                    {
                        this.getButtonImages()
                    }
                </div>
                <ul ref={this.refZoneList} id={'buildingArea_' + this.props.building.id} className={listClassName}>
                    {zoneUI}
                </ul>
            </li>
        );
    }
}