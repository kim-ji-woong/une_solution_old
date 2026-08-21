import React, { Component } from 'react';
import styles from '../css/spatial.module.css';
import { SpaceBody } from '../spaceBody';
import imgPlus from '../image/addSimple-01.png';
import imgMinus from '../image/minusSimple-01.png';
import imgPencil from '../image/pencilSimple-01.png';
import { BuildingInfo } from './buildingInfo';
import { DragDropManager } from './worker/dragDropManager';
import { SpaceController } from '../services/spaceController';
import { SiteInfo } from './siteInfo';

export class BuildingGroupInfo extends Component {
    constructor(props) {
        super(props);

        this.state = {
            editMode: false,
            editText: "",
            modelingFilePath: SiteInfo.defaultModelingFilePathInfo,
            expandNode: true,
            instance: this
        }

        this.refBuildingGroupName = React.createRef();
        this.refBuildingList = React.createRef();
        this.refEditBuildingGroupName = React.createRef();

        this.refModelingFilePath = React.createRef();
        this.refInputFile = React.createRef();
        this.refInputFile2 = React.createRef();

        this.dragDropManager = new DragDropManager(this);
        this.updateModelingFilePath = false;
    }

    componentDidMount() {
        this.dragDropManager.initDragEvents(this.refModelingFilePath.current);
    }

    componentDidUpdate(prevProps, prevState) {
        if (this.state.editMode && this.refEditBuildingGroupName.current) {
            this.refEditBuildingGroupName.current.focus();
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
                modelingFilePath: BuildingGroupInfo.getModelingFilePathFromProps(props, state),
                expandNode: state.expandNode,
                instance: state.instance
            };
        }

        return state;
    }

    static getModelingFilePathFromProps(props, state) {
        const buildingGroups = props.tempModelFiles?.buildingGroups;

        if (buildingGroups) {
            const buildingGroupID = props.buildingGroup?.id ? props.buildingGroup.id : -1;
            const modelingFilePath = buildingGroups[buildingGroupID];

            if (modelingFilePath) {
                return modelingFilePath;
            }
        }

        return state.modelingFilePath;
    }

    async onDropFiles(files) {
        if (files && files.length > 0) {
            const [success, message] = await SpaceController.requestUploadTempModelFile(files[0].object, this.props.loginData, SpaceBody.Type_BuildingGroup);

            if (success) {
                this.props.onSetModelFile(files[0].object.name, SpaceBody.Type_BuildingGroup, this.props.buildingGroup?.id);
                this.setState({ modelingFilePath: files[0].object.name });
            }
            else if (message && message.length > 0) {
                alert(message);
            }
        }
    }

    getBuildingUI() {
        let ui = [];

        if (!this.props.selectedInfo || this.props.selectedInfo.buildingGroup === null || this.props.selectedInfo.buildingGroup.id !== this.props.buildingGroup.id) {
            if (this.props.isOutdoor) {
                const outdoorZones = this.props.buildingGroup;

                for (const outdoorZoneID in outdoorZones) {

                    if (outdoorZones[outdoorZoneID] && this.props.selectedInfo.buildingGroup &&
                        this.props.selectedInfo.buildingGroup.id === outdoorZones[outdoorZoneID].id) {
                        continue;
                    }
                }
            }
            else {
                return ui;
            }
        }

        if (this.props.buildingGroup.buildingDatas) {
            const buildingDatas = this.props.buildingGroup.buildingDatas;
            if (buildingDatas === undefined || buildingDatas === null || buildingDatas.length === 0)
                return ui;

            for (var i = 0; i < buildingDatas.length; i++) {
                const building = buildingDatas[i];
                /*if (building.visible === false && this.props.searchText.length > 0)
                    continue;*/

                ui.push(
                    <BuildingInfo
                        key={'building_' + building.id}
                        building={building}
                        buildingIDs={this.props._3dOptions.buildingIDs}
                        selectedInfo={this.props.selectedInfo}
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
        else {
            const outdoorZones = this.props.buildingGroup;
            ui.push(
                <BuildingInfo
                    key={'building_outdoor'}
                    building={outdoorZones}
                    buildingIDs={this.props._3dOptions.buildingIDs}
                    selectedInfo={this.props.selectedInfo}
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

            this.updateModelingFilePath = false;
        }

        return ui;
    }

    showChildElement = (e) => {
        //this.setState({ expandNode: !this.state.expandNode });
        if (this.props.selectedInfo.buildingGroup === this.props.buildingGroup) {
            this.props.onSelectItem(null, SpaceBody.Type_BuildingGroup);
        }
        else {
            this.props.onSelectItem(this.props.buildingGroup, SpaceBody.Type_BuildingGroup);
        }        
    }

    isSelected() {
        if (this.props.selectedInfo) {
            if (this.props.selectedInfo.buildingGroup === this.props.buildingGroup) {
                return true;
            }
        }

        return false;
    }

    onClickAdd(e, type) {
        const buildingGroup = this.props.buildingGroup;

        if (!buildingGroup) {
            return;
        }

        this.props.onAddItem(buildingGroup, type);
        this.setState({ expandNode: true });
    }

    onClickRemove(e, type) {
        const buildingGroup = this.props.buildingGroup;//this.getSelectedBuildingGroup(e);

        if (!buildingGroup) {
            return;
        }

        this.props.onRemoveItem(buildingGroup, type);
    }

    onClickEdit(e, type) {
        const buildingGroup = this.props.buildingGroup;//this.getSelectedBuildingGroup(e);

        if (!buildingGroup) {
            return;
        }

        this.setState({ editMode: true, editText: buildingGroup.groupName });
    }

    /*getSelectedBuildingGroup(e) {
        const parent = e.target.parentElement?.parentElement;

        if (!parent) {
            return null;
        }

        const childCount = parent.children.length;

        for (let i = 0; i < childCount; i++) {
            const element = parent.children[i];

            if (element.nodeName === "SPAN") {
                if (element.dataset.buildinggroupid === this.props.buildingGroup.id.toString()) {
                    return this.props.buildingGroup;
                }
            }
        }

        return null;
    }*/

    renameBuildingGroup() {
        this.props.onRenameItem(this.props.buildingGroup, this.state.editText.trim(), SpaceBody.Type_BuildingGroup);
        this.setState({ editMode: false });
    }

    onKeyUp(e) {
        if (e.key === "Enter") {
            this.renameBuildingGroup();
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
        this.renameBuildingGroup();
    }

    moveToX = (e) => {
    }

    getButtonImages() {
        if (!this.props.modeling && !this.props.dashboard) {
            return (
                <div className={styles.treeIconImageArea}>
                    <img className={styles.treeIconImage} src={imgMinus} alt="icon" onClick={(e) => this.onClickRemove(e, SpaceBody.Type_BuildingGroup)} />
                    <img className={styles.treeIconImage} src={imgPlus} alt="icon" onClick={(e) => this.onClickAdd(e, SpaceBody.Type_Building)} />
                    <img className={styles.treeIconImage} src={imgPencil} alt="icon" onClick={(e) => this.onClickEdit(e, SpaceBody.Type_BuildingGroup)} />
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
        const buildingUI = this.getBuildingUI();
        let listClassName = styles.viewListConts;
        let showChild = 'false';

        if (this.state.expandNode) {
            listClassName += " " + styles.on;
            showChild = 'true';
        }

        const buildingGroupName = this.props.buildingGroup.displayText ? this.props.buildingGroup.displayText : "외부 영역";
        const buildingGroupID = this.props.buildingGroup.id ? this.props.buildingGroup.id : -1;

        return (
            <li>
                <div className={styles.viewListHeadWrap}>
                    {
                        !this.state.editMode &&
                        <>
                            <span ref={this.refBuildingGroupName} className={styles.viewListHead} data-show_child={showChild} data-buildinggroupid={buildingGroupID} data-target_class='viewListHead' onClick={(e) => { this.showChildElement(e) }}>{buildingGroupName}</span>
                            {
                                this.props.modeling &&
                                <div className={styles.dragDrop}>
                                    <div ref={this.refModelingFilePath} className={styles.modelingInput}>{this.state.modelingFilePath}</div>
                                    <input ref={this.refInputFile} type="file" className={styles.hidden} onChange={() => { }} />
                                    <img className={styles.treeIconImage} src={imgMinus} alt="icon" onClick={() => this.props.onClickClear(this, buildingGroupID, SpaceBody.Type_BuildingGroup)} />
                                    <img className={styles.treeIconImage} src={imgPlus} alt="icon" onClick={() => this.onClickSelectFile()} />
                                    <input ref={this.refInputFile2} className={styles.hidden} onChange={(e) => this.selectFile(e.target.files[0])} type="file" />
                                </div>
                            }
                        </>
                    }
                    {
                        this.state.editMode &&
                        <input ref={this.refEditBuildingGroupName} type="text" id="txtSearch" value={this.state.editText} onKeyUp={(e) => this.onKeyUp(e)} onChange={(e) => this.onChange(e)} onBlur={(e) => this.onFocusout(e)} />
                    }
                    {
                        this.getButtonImages()
                    }
                </div>
                <div id={'buildingGroupArea_' + this.props.buildingGroup.id} ref={this.refBuildingList} className={listClassName} data-id={this.props.buildingGroup.id}>
                    <ul>
                        {buildingUI}
                    </ul>
                </div>
            </li>
        );
    }
}