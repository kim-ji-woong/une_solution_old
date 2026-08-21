import React, { Component } from 'react';
import styles from '../css/spatial.module.css';
import { SpaceBody } from '../spaceBody';
import imgPlus from '../image/addSimple-01.png';
import imgMinus from '../image/minusSimple-01.png';
import imgPencil from '../image/pencilSimple-01.png';
import { EquipZoneInfo } from './equipZoneInfo';
import { DragDropManager } from './worker/dragDropManager';
import { SpaceController } from '../services/spaceController';
import { SiteInfo } from './siteInfo';
import { SpaceMenus } from '../spaceMenus';

export class ZoneInfo extends Component {
    constructor(props) {
        super(props);

        this.state = {
            editMode: false,
            editText: "",
            modelingFilePath: SiteInfo.defaultModelingFilePathInfo,
            expandNode: true
        }

        this.refZoneName = React.createRef();
        this.refZoneNameList = React.createRef();
        this.refEditZoneName = React.createRef();

        this.refModelingFilePath = React.createRef();
        this.refInputFile = React.createRef();
        this.refInputFile2 = React.createRef();

        this.prevSelectedSensor = [null, null, null];
        this.dragDropManager = new DragDropManager(this);
    }

    componentDidMount() {
        this.checkChildVisible();
        this.dragDropManager.initDragEvents(this.refModelingFilePath.current);
    }

    componentDidUpdate(prevProps, prevState) {
        this.checkChildVisible();

        if (this.state.editMode && this.refEditZoneName.current) {
            this.refEditZoneName.current.focus();
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
            return {
                editMode: state.editMode,
                editText: state.editText,
                modelingFilePath: ZoneInfo.getModelingFilePathFromProps(props, state),
                expandNode: state.expandNode
            };
        }

        return state;
    }

    static getModelingFilePathFromProps(props, state) {
        const zones = props.tempModelFiles?.zones;

        if (zones) {
            const zoneID = props.zone?.id ? props.zone.id : -1;
            const modelingFilePath = zones[zoneID];

            if (modelingFilePath) {
                return modelingFilePath;
            }
        }

        return state.modelingFilePath;
    }

    async onDropFiles(files) {
        if (files && files.length > 0) {
            const [success, message] = await SpaceController.requestUploadTempModelFile(files[0].object, this.props.loginData, SpaceBody.Type_Floor);

            if (success) {
                this.props.onSetModelFile(files[0].object.name, SpaceBody.Type_Floor, this.props.zone?.id);
                this.setState({ modelingFilePath: files[0].object.name });
            }
            else if (message && message.length > 0) {
                alert(message);
            }
        }
    }

    checkChildVisible() {
        this.checkChildVisibleData(this.refZoneName.current, this.refZoneNameList.current, this.state.expandNode);
    }

    checkChildVisibleData(mainElement, listElement, showChild) {
        if (mainElement) {
            if (showChild) {
                if (mainElement.dataset.show_child !== 'true') {
                    mainElement.dataset.show_child = 'true';
                }

                if (listElement) {
                    if (listElement.classList.contains(styles.on) === false) {
                        listElement.classList.add(styles.on);
                    }
                }
            }
            else if (listElement) {
                if (mainElement.dataset.show_child !== 'false') {
                    mainElement.dataset.show_child = 'false';
                }

                if (listElement.classList.contains(styles.on)) {
                    listElement.classList.remove(styles.on);
                }
            }
        }
    }

    showChild(e) {
        this.setState({ expandNode: !this.state.expandNode });
    }

    isSelected() {
        let zoneShowChild = 'false';

        const [sensorType, zoneID, sensorID] = this.props.selectedSensor;
        this.prevSelectedSensor = [sensorType, zoneID, sensorID];

        if (sensorType !== null && zoneID !== null && sensorID !== null) {
            const zoneData = this.props.zone;

            if (zoneData && zoneData.id === zoneID) {
                // 선택된 센서가 있으니 Tree를 펼친다.
                zoneShowChild = 'true';
            }
        }
        else {
            if (this.props.selectedInfo) {
                if (this.props.zone === this.props.selectedInfo.zone) {
                    zoneShowChild = 'true';
                }
            }
            else {
            }
        }

        return zoneShowChild;
    }

    getEquipZoneUI() {
        let ui = [];
        const zone = this.props.zone;

        if (zone) {
            const equipZoneDatas = zone.equipmentZoneDatas;
            if (!equipZoneDatas)
                return ui;

            const equipZoneCount = equipZoneDatas.length;

            for (let i = 0; i < equipZoneCount; i++) {
                const equipZone = equipZoneDatas[i];

                ui.push(
                    <EquipZoneInfo
                        key={'equipZone_' + equipZone.id}
                        equipZone={equipZone}
                        zone={this.props.zone}
                        sensorList={this.props.sensorList}
                        selectedInfo={this.props.selectedInfo}
                        searchText={this.props.searchText}
                        isEditMode={this.props.isEditMode}
                        modeling={this.props.modeling}
                        dashboard={this.props.dashboard}
                        loginData={this.props.loginData}
                        onAddItem={this.props.onAddItem}
                        onRemoveItem={this.props.onRemoveItem}
                        onRenameItem={this.props.onRenameItem}
                        onSelectItem={this.props.onSelectItem}
                    />
                );
            }
        }

        return ui;
    }

    onClickAdd(e, type) {
        const zone = this.props.zone;

        if (!zone) {
            return;
        }

        this.props.onAddItem(zone, type);
        this.setState({ expandNode: true });
    }

    onClickRemove(e, type) {
        const zone = this.props.zone;

        if (!zone) {
            return;
        }

        this.props.onRemoveItem(zone, type);
    }

    onClickEdit(e, type) {
        const zone = this.props.zone;

        if (!zone) {
            return;
        }

        this.setState({ editMode: true, editText: zone.zoneName });
    }

    renameZoneName() {
        this.props.onRenameItem(this.props.zone, this.state.editText.trim(), SpaceBody.Type_Floor);
        this.setState({ editMode: false });
    }

    onKeyUp(e) {
        if (e.key === "Enter") {
            this.renameZoneName();
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
        this.renameZoneName();
    }

    moveToX = (e) => {
        this.props.moveToX(SpaceMenus.Menu_MoveTo_Floor, this.props.zone);
    }

    getButtonImages() {
        if (!this.props.modeling && !this.props.dashboard) {
            return (
                <div className={styles.treeIconImageArea}>
                    <img className={styles.treeIconImage} src={imgMinus} alt="icon" onClick={(e) => this.onClickRemove(e, SpaceBody.Type_Floor)} />
                    <img className={styles.treeIconImage} src={imgPlus} alt="icon" onClick={(e) => this.onClickAdd(e, SpaceBody.Type_EquipZone)} />
                    <img className={styles.treeIconImage} src={imgPencil} alt="icon" onClick={(e) => this.onClickEdit(e, SpaceBody.Type_Floor)} />
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
        const equipZoneUI = this.getEquipZoneUI();
        const zoneShowChild = this.state.expandNode ? 'true' : 'false';
        const zoneName = this.props.zone.displayText ? this.props.zone.displayText : this.props.zone.name;
        const zoneID = this.props.zone?.id ? this.props.zone.id : -1;
        const zoneClassName = this.state.editMode ? styles.viewList2DepthSpen + " " + styles.hidden : styles.viewList2DepthSpen;
        const listClassName = this.state.expandNode ? styles.viewList3Depth + " " + styles.on : styles.viewList3Depth;

        return (
            <li>
                <div className={styles.viewListDepthParent}>
                    <span className={styles.testtt}> {/* test */}
                    <div className={styles.viewList2DepthHead} >
                        <>
                            <span ref={this.refZoneName} className={zoneClassName} data-show_child={zoneShowChild} data-target_class='viewList2Depth' onClick={(e) => { this.showChild(e) }}>{zoneName}</span>
                            {
                                this.props.modeling &&
                                <div className={styles.dragDrop}>
                                    <div ref={this.refModelingFilePath} className={styles.modelingInput}>{this.state.modelingFilePath}</div>
                                    <input ref={this.refInputFile} type="file" className={styles.hidden} onChange={() => { }} />
                                    <img className={styles.treeIconImage} src={imgMinus} alt="icon" onClick={() => this.props.onClickClear(this, zoneID, SpaceBody.Type_Floor)} />
                                    <img className={styles.treeIconImage} src={imgPlus} alt="icon" onClick={() => this.onClickSelectFile()} />
                                    <input ref={this.refInputFile2} className={styles.hidden} onChange={(e) => this.selectFile(e.target.files[0])} type="file" />
                                </div>
                            }
                        </>
                        </div>
                    </span>
                    {
                        this.state.editMode &&
                        <input ref={this.refEditZoneName} type="text" value={this.state.editText} onKeyUp={(e) => this.onKeyUp(e)} onChange={(e) => this.onChange(e)} onBlur={(e) => this.onFocusout(e)} />

                    }
                    {
                        this.getButtonImages()
                    }
                </div>
                {
                    <ul ref={this.refZoneNameList} id={'zoneArea_' + this.props.zone.id} className={listClassName}>
                        {equipZoneUI}
                    </ul>
                }
            </li>
        );
    }
}