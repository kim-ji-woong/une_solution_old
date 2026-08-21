import React, { Component } from 'react';
import space from './../css/space.module.css';
import styles from '../css/spatial.module.css';


import $ from 'jquery';
import { BuildingGroupNode } from './buildingGroupTreeView/buildingGroupNode';
import { SpaceController } from '../services/spaceController';
import { SpaceDataManager } from '../services/spaceDataManager';
import { SensorListEdit } from './sensorListEdit';
import { SensorListEdit_gridView } from './sensorListEdit_gridview';
import SensorMakerResource from '../../resource/id';
import { SpaceBody } from '../spaceBody';
/* import styles from '../css/space.module.css'; */
import rootStyles from '../../Root/css/root.module.css';



export class EquipZoneCCTV extends Component {

    static ApplyEquipZone_All = 0;
    static ApplyEquipZone_On = 1;
    static ApplyEquipZone_Off = 2;

    constructor(props) {
        super(props);
        this.state = {
            curApplyEquipZone: SensorListEdit.ApplyEquipZone_All,
            isEditMode: false,    

            searchText: '',
            searchTextGridView: '',
            viewSensors: []
        }

        this.selectedRows = [];

        this.nIndexLastSelectedRow = -1;

        this.nLastSelectedNodeEquipZoneID = -1;
        this.nIndexLastSelectedNode = -1;

        // 파일 업로드 버튼
        this.refFileUpload = React.createRef();
        this.refFileUploadTemp = React.createRef();

        this.onDownloadExcel = this.onDownloadExcel.bind(this);
        this.onUploadExcel = this.onUploadExcel.bind(this);
        this.onClickUploadDelegate = this.onClickUploadDelegate.bind(this);
        //this.onRemoveEquipZoneSensor = this.onRemoveEquipZoneSensor.bind(this);
        this.onKeyDownEventHandler = this.onKeyDownEventHandler.bind(this);
    }

    componentDidMount() {

        $('li:not(:has(ul))').css({ cursor: 'pointer', 'list-style-image': 'none' });
        $('li:has(ul)')
            /* .css({ cursor: 'pointer', 'list-style-image': "url(../../Space/image/treePlus-01.png)" }) */
            .children().hide();

        $('li:has(ul)').click(function (event) {
            if (this == event.target) {
                if ($(this).children().is(':hidden')) {
                    $(this).css('list-style-image', 'url(minus.gif)').children().slideDown();
                }
                else {
                    $(this).css('list-style-image', 'url(plus.gif)').children().slideUp();
                }
            }
            return false;
        });

        const btnFileUpload = this.refFileUploadTemp.current;
        btnFileUpload.addEventListener("click", this.onClickUploadDelegate);

        window.addEventListener('keydown', this.onKeyDownEventHandler, false);
        window.addEventListener('keyup', this.onKeyUpEventHandler, false);
    }

    // <input type='file'의 디자인 변경하려고 다른 버튼에 이벤트 붙임
    onClickUploadDelegate() {
        const galleryFile = this.refFileUpload.current;

        const event = new MouseEvent("click", {
            bubbles: true,
            cancelable: true,
            view: window
        });

        galleryFile.dispatchEvent(event);
    }

    componentWillUnmount() {
        const btnFileUpload = this.refFileUploadTemp.current;
        btnFileUpload.addEventListener("click", this.onClickUploadDelegate);

        window.removeEventListener('keydown', this.onKeyDownEventHandler);
        window.removeEventListener('keyup', this.onKeyUpEventHandler);
    }

    onKeyDownEventHandler(e) {
        if (e.key === 'Control') {
            SpaceBody.keyPress = SpaceBody.keyPressControl;
        }
        else if (e.key === 'Shift') {
            SpaceBody.keyPress = SpaceBody.keyPressShift;
        }
        else if (e.key === 'Delete') {
            // 선택된 센서 노드가 있다면 삭제한다
            this.onRemoveEquipZoneSensor();
        }
    }

    onKeyUpEventHandler(e) {
        if (e.key === 'Control' || e.key === 'Shift') {
            SpaceBody.keyPress = SpaceBody.keyPressNone;
        }
    }

    async onDownloadExcel() {
        let sensors = this.props.sensorList[SpaceDataManager.CCTVType];
        await SpaceController.requestDownloadSensorExcelFile(SpaceDataManager.CCTVType, sensors);
    }

    async onUploadExcel(event) {
        const file = event.target.files[0];
        if (!file) {
            return;
        }

        const result = await SpaceController.requestUploadExcelFile(file, SpaceDataManager.CCTVType);
        if (!result[0]) {
            alert(result[1]);
            return;
        }

        if (result[2]) {
            const sensors = this.initID(result[2]);
            this.props.onChangeSensorList(SpaceDataManager.CCTVType, sensors);
        }
    }

    initID(sensors) {
        const sensorCount = sensors.length;
        for (let i = 0; i < sensorCount; i++) {
            const sensor = sensors[i];
            if (sensor.id === null || sensor.id === -1) {                
                sensor.id = ++SpaceDataManager.cctvID;
            }
        }

        return sensors;
    }

    onKeyUpSearch = (isTreeView, e) => {
        if (e.key === 'Enter') {
            if (isTreeView) {
                this.onSearch(e.target.value);
            }
            else {
                this.onSearchGridView(e.target.value);
            }
        }
    }

    onSearch = (text) => {
        if (this.state.searchText === text) {
            return;
        }

        this.setState({ searchText: text });
    }

    onSearchGridView = (text) => {
        if (this.state.searchTextGridView === text) {
            return;
        }

        this.setState({ searchTextGridView: text });
    }

    onChangeSensor = (sensors) => {
        this.removeSelectedRows(null);
        this.props.onChangeSensorList(SpaceDataManager.CCTVType, sensors);        
    }

    onRemoveEquipZoneSensor = () => {
        let sensorList = this.props.sensorList;

        const selectedNodes = this.props.selectedNodes;
        const selectedNodeCount = selectedNodes.length;
        for (let i = 0; i < selectedNodeCount; i++) {
            const selectedNode = selectedNodes[i];
            if (SensorMakerResource.isPSMSensorType(selectedNode.sensorType)) {
                const sensors = sensorList[SpaceDataManager.PSMSensorType];
                let sensor = sensors.find(p => p.id === selectedNode.id);
                if (sensor) {
                    sensor.buildingID = null;
                    sensor.zoneID = -1;
                    sensor.equipZoneID = null;
                }
            }
            else if (SensorMakerResource.isETCSensorType(selectedNode.sensorType)) {
                const sensors = sensorList[SpaceDataManager.EtcSensorType];
                let sensor = sensors.find(p => p.id === selectedNode.id);
                if (sensor) {
                    sensor.buildingID = null;
                    sensor.zoneID = -1;
                    sensor.equipZoneID = null;
                }
            }
            else if (SensorMakerResource.isSVMSSensorType(selectedNode.sensorType)) {
                const sensors = sensorList[SpaceDataManager.CCTVType];
                let sensor = sensors.find(p => p.id === selectedNode.id);
                if (sensor) {
                    const equipZoneIDCount = sensor.equipZoneIDs.length;
                    for (let j = 0; j < equipZoneIDCount; j++) {
                        if (sensor.equipZoneIDs[j] === this.nLastSelectedNodeEquipZoneID) {
                            sensor.equipZoneIDs.splice(j, 1);
                            break;
                        }
                    }
                }
            }
            else {
                const sensors = sensorList[SpaceDataManager.FireSensorType];
                let sensor = sensors.find(p => p.id === selectedNode.id);
                if (sensor) {
                    sensor.buildingID = null;
                    sensor.zoneID = -1;
                    sensor.equipZoneID = null;
                }
            }
        }

        this.props.onChangeSensorList(null, sensorList);
    }

    onApplyEquipZone(value) {
        let value2 = Number(value);

        if (value2 === this.state.curApplyEquipZone) {
            return;
        }

        this.nIndexLastSelectedRow = -1;
        this.selectedRows = [];
        this.setState({ curApplyEquipZone: value2 });
    }

    addSelectedRows = (sensors) => {
        this.selectedRows = sensors;
        //this.setState({ selectedRows: sensors });
    }

    removeSelectedRows = (sensor) => {
        if (sensor) {
            let rows = [...this.state.selectedRows];

            let removeIndex = -1;
            const rowCount = rows.length;
            for (let i = 0; i < rowCount; i++) {
                if (rows[i] === sensor) {
                    removeIndex = i;
                }
            }
            rows.splice(removeIndex, 1);
            this.selectedRows = rows;
            //this.setState({ selectedRows: rows });
        }
        else {
            this.selectedRows = [];
            //this.setState({ selectedRows: [] });
        }
    }

    getBuildingGroupTreeViewUI() {
        let ui = [];
        const buildingGroupList = this.props.buildingGroupList;
        if (this.state.searchText.length > 0) {
            SpaceDataManager.setVisibleBuildingGroupList(buildingGroupList, this.props.sensorList, this.state.searchText);
        }

        const buildingGroupCount = buildingGroupList.length;
        for (let i = 0; i < buildingGroupCount; i++) {
            const buildingGroup = buildingGroupList[i];
            if (buildingGroup.visible) {
                if (buildingGroup.visibleTreeView === false && this.state.searchText.length > 0) {
                    continue;
                }

                ui.push(
                    <BuildingGroupNode
                        key={'node_buildingGroup_' + buildingGroup.id}
                        buildingGroup={buildingGroup} sensorList={this.props.sensorList}
                        //selectedRows={this.state.selectedRows}
                        onChangeSensor={this.onChangeSensor}
                        curSensorType={SpaceDataManager.CCTVType}
                        selectedNodes={this.props.selectedNodes}
                        addSelectedNodes={this.props.addSelectedNodes}
                        removeSelectedNodes={this.props.removeSelectedNodes}
                        isEditMode={true}
                        selectedMenu={this.props.selectedMenu}
                        parentFrm={this}
                        searchText={this.state.searchText}
                    />
                );
            }
        }

        return ui;
    }

    getViewSensors() {
        let sensors = this.props.sensorList[SpaceDataManager.CCTVType];
        const searchText = this.state.searchTextGridView;
        if (searchText.length === 0) {
            return sensors;
        }

        let viewSensors = [];

        const sensorCount = sensors.length;
        for (let i = 0; i < sensorCount; i++) {
            const sensor = sensors[i];
            if (sensor.name.includes(searchText) || sensor.uniqueKey.includes(searchText) || sensor.type.toString().includes(searchText) ||
                sensor.userID.includes(searchText) || sensor.password.includes(searchText) || sensor.url.toString().includes(searchText) ||
                sensor.positionName.toString().includes(searchText)) {
                viewSensors.push(sensor);
            }
        }

        return viewSensors;
    }

    render() {
        const buildingGroupTreeViewUI = this.getBuildingGroupTreeViewUI();
        let sensors = this.getViewSensors();
        const scrollAreaStyle = this.props.modeling ? styles.dsiScr + " " + styles.short + " " + rootStyles.scrollbar : styles.dsiScr + " " + rootStyles.scrollbar;

        return (
            <>
                <span className={space.listTitle}>Sensor List Edit</span>
                <div className={space.listArea}>
                    <div className={space.listBox}>
                        <div className={space.listTitleBox}>
                            <span className={space.listTitleText}>구역별 CCTV 편집</span>
                            <span className={space.listSelect}><input type="text" onKeyUp={(e) => this.onKeyUpSearch(true, e)} className={space.inputFind} /></span>
                        </div>


                        <div className={space.listContent} className={scrollAreaStyle}>
                            <ul>
                                {buildingGroupTreeViewUI}
                            </ul>
                        </div>
                    </div>

                    <div className={space.listBox2}>
                        <div className={space.listTitleComboBox}>
                            {
                                //<div className={space.selectBox}>
                                //</div>
                            }
                            <div className={space.selectBox2}>
                                <div className={space.select}>
                                    <select className={space.selectCom} onChange={(e) => this.onApplyEquipZone(e.target.value)}>
                                        <option value={SensorListEdit.ApplyEquipZone_All}>전체</option>
                                        <option value={SensorListEdit.ApplyEquipZone_Off}>미적용</option>
                                        <option value={SensorListEdit.ApplyEquipZone_On}>적용</option>
                                    </select>
                                </div>
                            </div>
                            <span className={space.listSelect2}><input type="text" onKeyUp={(e) => this.onKeyUpSearch(false, e)} className={space.listSelectInput} /></span>
                            {
                            //<span className={space.listIcon2} onClick={() => this.onChangeEditMode()} title="센서편집" ></span>
                            //<span className={space.listIcon2_1} onClick={() => this.onAddSensor()} title="센서추가" ></span>
                            }
                            <span className={space.downIcon} onClick={this.onDownloadExcel}>엑셀파일 다운로드</span>

                            <input type='file' className={space.galleryFile} ref={this.refFileUpload} onChange={(e) => this.onUploadExcel(e)} />
                            <span className={space.uploadIcon} ref={this.refFileUploadTemp}>엑셀파일 업로드</span>

                            {
                                //<span className={space.uploadIcon}>엑셀파일 업로드</span>
                            }
                        </div>
                        <div className={space.listContScrollbar}>
                            <div className={space.listContent2}>
                                <table className={space.listTable2}>
                                    <thead>
                                        <tr>
                                            {
                                                //<th style={{ width: "60px" }}></th>
                                            }
                                            <th style={{ width: "150px" }}>CCTV명</th>
                                            <th style={{ width: "100px" }}>고유키</th>
                                            <th style={{ width: "75px" }}>Type</th>
                                            <th style={{ width: "75px" }}>UserID</th>
                                            <th style={{ width: "90px" }}>Password</th>
                                            <th style={{ width: "250px" }}>URL</th>
                                            <th style={{ width: "250px" }}>설치장소</th>
                                            <th style={{ width: "450px" }}>...</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {
                                            <SensorListEdit_gridView
                                                parentFrm={this}
                                                isEditMode={false}
                                                sensors={sensors}
                                                curSensorType={SpaceDataManager.CCTVType}
                                                curApplyEquipZone={this.state.curApplyEquipZone}
                                                buildingGroupList={this.props.buildingGroupList}
                                                onChangeSensor={this.onChangeSensor}
                                                //selectedRows={this.state.selectedRows}
                                                addSelectedRows={this.addSelectedRows}
                                                removeSelectedRows={this.removeSelectedRows}
                                            />
                                        }
                                    </tbody>
                                </table>
                            </div> {/* listContent2 */}
                        </div>
                    </div>

                </div> {/* listArea */}
            </>
        );
    }
}