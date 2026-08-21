import React, { Component } from 'react';
import space from './../css/space.module.css';
import styles from '../css/spatial.module.css';
import rootStyles from '../../Root/css/root.module.css';

import $ from 'jquery';
import { BuildingGroupNode } from './buildingGroupTreeView/buildingGroupNode';
import { SpaceController } from '../services/spaceController';
import { SpaceDataManager } from '../services/spaceDataManager';
import { SensorListEdit_gridView } from './sensorListEdit_gridview';
import SensorMakerResource from '../../resource/id';
import { SpaceBody } from '../spaceBody';
import { Input } from '@material-ui/core';

export class SensorListEdit extends Component {

    static ApplyEquipZone_All = 0;
    static ApplyEquipZone_On = 1;
    static ApplyEquipZone_Off = 2;

    constructor(props) {
        super(props);
        this.state = {
            curSensorType: SpaceDataManager.FireSensorType,
            curApplyEquipZone: SensorListEdit.ApplyEquipZone_All,
            isEditMode: false,
            //selectedRows: [],  

            // 페이지 관련
            minPageIndex: 1,
            maxPageIndex: 1,
            pageIndex: 1,

            searchText: '',
            searchTextGridView: '',
            viewSensors: []
        }

        this.selectedRows = [];

        this.nIndexLastSelectedRow = -1;

        this.maxPageCount = 10; // 한번에 보여줄 페이지 개수
        this.maxRowCount = 20;

        // 파일 업로드 버튼
        this.refFileUpload = React.createRef();
        this.refFileUploadTemp = React.createRef();

        this.onDownloadExcel = this.onDownloadExcel.bind(this);
        this.onUploadExcel = this.onUploadExcel.bind(this);
        this.onClickUploadDelegate = this.onClickUploadDelegate.bind(this);
        this.onKeyDownEventHandler = this.onKeyDownEventHandler.bind(this);
    }

    componentDidCatch(error, info) {
        console.log('sensorListEdit.jsx error');
        console.error(error, info);
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

        this.initPageIndex(this.state.searchTextGridView, this.state.curSensorType, this.state.curApplyEquipZone);
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

    initPageIndex(searchText, sensorType, applyEquipZone) {
        const viewSensors = this.getViewSensors(searchText, sensorType, applyEquipZone);

        let minPageIndex = 1;
        let maxPageIndex = 1;

        if (viewSensors) {
            let rowCount = viewSensors.length;

            const value1 = parseInt(rowCount / this.maxRowCount);
            const value2 = rowCount % this.maxRowCount; // 나머지가 있는 경우 페이지 하나를 추가한다.
            maxPageIndex = value1 + ((value2 > 0) ? 1 : 0);
        }

        this.setState({
            viewSensors, minPageIndex, maxPageIndex, pageIndex: 1,
            searchTextGridView: searchText,
            curSensorType: sensorType,
            curApplyEquipZone: applyEquipZone
        });
    }

    getViewSensors(searchText, sensorType, applyEquipZone) {
        let viewSensors = [];

        let sensors = this.props.sensorList[sensorType];
        if (sensors) {
            //if (searchText === this.state.searchTextGridView && sensorType === this.state.curSensorType && applyEquipZone === this.state.curApplyEquipZone) {
            //    // 필터 상태가 이전과 같다
            //    return sensors;
            //}

            const sensorCount = sensors.length;
            for (let i = 0; i < sensorCount; i++) {
                let sensor = sensors[i];

                // 센서타입, Equipzone적용 여부 필터
                if (applyEquipZone === SensorListEdit.ApplyEquipZone_On) {
                    if (sensor.equipZoneID === null) {
                        continue;
                    }
                }
                else if (applyEquipZone === SensorListEdit.ApplyEquipZone_Off) {
                    if (sensor.equipZoneID !== null) {
                        continue;
                    }
                }

                // 검색어 필터
                if (searchText.length === 0 ||
                    (searchText.length > 0 && (sensor.name.includes(searchText) || sensor.positionName.includes(searchText) || sensor.tagNo.toString().includes(searchText)))) {
                    viewSensors.push(sensor);
                }
            }
        }

        return viewSensors;
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

        this.initPageIndex(text, this.state.curSensorType, this.state.curApplyEquipZone);
    }

    async onDownloadExcel() {
        let sensors = this.props.sensorList[this.state.curSensorType];
        await SpaceController.requestDownloadSensorExcelFile(this.state.curSensorType, sensors);
    }

    async onUploadExcel(event) {
        const file = event.target.files[0];
        if (!file) {
            return;
        }

        const result = await SpaceController.requestUploadExcelFile(file, this.state.curSensorType);
        if (!result[0]) {
            alert(result[1]);
            return;
        }

        if (result[2]) {
            const sensors = this.initID(result[2]);
            this.props.onChangeSensorList(this.state.curSensorType, sensors);
            this.initPageIndex(this.state.searchTextGridView, this.state.curSensorType, this.state.curApplyEquipZone);
        }
    }

    initID(sensors) {
        const sensorCount = sensors.length;
        for (let i = 0; i < sensorCount; i++) {
            const sensor = sensors[i];
            if (sensor.id === null || sensor.id === -1) {
                if (this.state.curSensorType === SpaceDataManager.FireSensorType) {
                    sensor.id = ++SpaceDataManager.fireSensorID;
                }
                else if (this.state.curSensorType === SpaceDataManager.PSMSensorType) {
                    sensor.id = ++SpaceDataManager.psmSensorID;
                }
                else if (this.state.curSensorType === SpaceDataManager.EtcSensorType) {
                    sensor.id = ++SpaceDataManager.etcSensorID;
                }
                else if (this.state.curSensorType === SpaceDataManager.CCTVType) {
                    sensor.id = ++SpaceDataManager.cctvID;
                }
            }
        }

        return sensors;
    }

    onChangeSensor = (sensors) => {
        this.props.onChangeSensorList(this.state.curSensorType, sensors);
        this.removeSelectedRows(null);
    }

    onRemoveEquipZoneSensor = () => {
        let sensorList = this.props.sensorList;

        const selectedNodes = this.props.selectedNodes;
        const selectedNodeCount = selectedNodes.length;
        for (let i = 0; i < selectedNodeCount; i++) {
            const selectedNode = selectedNodes[i];
            if (selectedNode.sensorType === SensorMakerResource.isPSMSensorType) {
                const sensors = sensorList[SpaceDataManager.PSMSensorType];
                let sensor = sensors.find(p => p.id === selectedNode.id);
                if (sensor) {
                    sensor.buildingID = null;
                    sensor.zoneID = -1;
                    sensor.equipZoneID = null;
                }
            }
            else if (selectedNode.sensorType === SensorMakerResource.isETCSensorType) {
                const sensors = sensorList[SpaceDataManager.EtcSensorType];
                let sensor = sensors.find(p => p.id === selectedNode.id);
                if (sensor) {
                    sensor.buildingID = null;
                    sensor.zoneID = -1;
                    sensor.equipZoneID = null;
                }
            }
            else if (selectedNode.sensorType === SensorMakerResource.isSVMSSensorType) {
                const sensors = sensorList[SpaceDataManager.CCTVType];
                let sensor = sensors.find(p => p.id === selectedNode.id);
                if (sensor) {
                    sensor.buildingID = null;
                    sensor.zoneID = -1;
                    sensor.equipZoneID = null;
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
        this.initPageIndex(this.state.searchTextGridView, this.state.curSensorType, this.state.curApplyEquipZone);
    }

    onChangeSensorType(value) {
        if (value === this.state.curSensorType) {
            return;
        }

        this.nIndexLastSelectedRow = -1;
        this.selectedRows = [];
        this.initPageIndex(this.state.searchTextGridView, value, this.state.curApplyEquipZone);
    }

    onApplyEquipZone(value) {
        let value2 = Number(value);

        if (value2 === this.state.curApplyEquipZone) {
            return;
        }

        this.nIndexLastSelectedRow = -1;
        this.selectedRows = [];
        this.initPageIndex(this.state.searchTextGridView, this.state.curSensorType, value2);
    }

    onChangeEditMode = () => {
        let isEditMode = !this.state.isEditMode;

        this.setState({ isEditMode: isEditMode });
    }

    onAddSensor = () => {
        if (!this.state.isEditMode) {
            return;
        }

        const newSensor = {
            //id: SpaceDataManager.fireSensorID,            
            buildingID: null,
            department: null,
            departmentPhoneNumber: null,
            enabled: null,
            equipZoneID: null,
            isIndoor: true,
            name: '',
            positionName: '',
            sensorTagInfoID: null,
            sensorZoneID: null,
            x: null,
            y: null,
            z: null,            
            zoneID: -1            
        };

        if (this.state.curSensorType === SpaceDataManager.FireSensorType) {
            const id = ++SpaceDataManager.fireSensorID;
            newSensor.id = id;
            newSensor.orgSensorID = id;
            newSensor.sensorType = SensorMakerResource.facilityType.FIRE;
            newSensor.sensorSubType = null;
        }
        else if (this.state.curSensorType === SpaceDataManager.PSMSensorType) {
            const id = ++SpaceDataManager.psmSensorID;
            newSensor.id = id;
            newSensor.orgSensorID = id;
            newSensor.sensorType = SensorMakerResource.facilityType.PSM_SENSOR;
        }
        else if (this.state.curSensorType === SpaceDataManager.EtcSensorType) {
            const id = ++SpaceDataManager.etcSensorID;
            newSensor.id = id;
            newSensor.orgSensorID = id;
            newSensor.sensorType = SensorMakerResource.facilityType.ETC;
        }
        else if (this.state.curSensorType === SpaceDataManager.CCTVType) {
            const id = ++SpaceDataManager.cctvID;
            newSensor.id = id;
            newSensor.orgSensorID = id;
            newSensor.sensorType = SensorMakerResource.facilityType.Security_Sensor;
        }
        else {
            return;
        }

        let sensorList = this.props.sensorList[this.state.curSensorType];
        sensorList.push(newSensor);

        this.props.onChangeSensorList(this.state.curSensorType, sensorList);
        this.setState({ pageIndex: this.state.maxPageIndex });
    }

    addSelectedRows = (sensors) => {        
        this.selectedRows = sensors;
        this.setState({ selectedRows: sensors });
    }

    removeSelectedRows = (sensor) => {
        if (sensor) {
            let rows = [...this.selectedRows];

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

    setPageIndex = (index) => {
        if (this.state.pageIndex === index) {
            return;
        }
        if (this.state.maxPageIndex < index || index < 1) {
            return;
        }

        this.selectedRows = [];
        this.setState({ pageIndex: index });
    }

    getBuildingGroupTreeViewUI() {
        let ui = [];

        const buildingGroupList = [...this.props.buildingGroupList];
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
                        buildingGroup={buildingGroup}
                        sensorList={this.props.sensorList}
                        //selectedRows={this.state.selectedRows}
                        onChangeSensor={this.onChangeSensor}
                        curSensorType={this.state.curSensorType}
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

    // 하단 페이지 index 만들기
    getPageIndexUI() {
        let ui = [];
        if (!this.props.sensorList[this.state.curSensorType]) {
            return ui;
        }

        const pageArr = new Array();

        let index = this.state.pageIndex;
        // 이전 페이지 넣기
        while (true) {
            index--;
            if (index < 1) {
                break;
            }
            if (this.state.pageIndex - 5 > index) {
                break;
            }

            pageArr.push(index);
        }
        index = this.state.pageIndex;
        pageArr.push(index);

        // 다음 페이지 넣기
        while (true) {
            if (pageArr.length === this.maxPageCount) {
                break;
            }

            index++;
            if (index > this.state.maxPageIndex) {
                break;
            }

            pageArr.push(index);
        }

        // 정렬
        pageArr.sort(function (a, b) { if (a > b) return 1; if (a === b) return 0; if (a < b) return -1; });

        for (let i = 0; i < pageArr.length; i++) {
            let pageIndex = pageArr[i];
            if (pageIndex === this.state.pageIndex) {
                ui.push(<li key={'pageIndex_' + (pageIndex)} className={space.on}><a onClick={() => this.setPageIndex(pageIndex)}>{pageIndex}</a></li>);
            }
            else {
                ui.push(<li key={'pageIndex_' + (pageIndex)}><a onClick={() => this.setPageIndex(pageIndex)}>{pageIndex}</a></li>);
            }
        }

        return ui;
    }

    render() {
        const buildingGroupTreeViewUI = this.getBuildingGroupTreeViewUI();
        const scrollAreaStyle = this.props.modeling ? styles.dsiScr + " " + styles.short + " " + rootStyles.scrollbar : styles.dsiScr + " " + rootStyles.scrollbar;

        const pageIndexUI = this.getPageIndexUI();
        
        return (
            <>
                <span className={space.listTitle}>센서목록 편집</span>
                <div className={space.listArea}>
                    <div className={space.listBox}>
                        <div className={space.listTitleBox}>
                            <span className={space.listTitleText}>센서 목록 편집</span>
                            <span className={space.listSelect}><input type="text" onKeyUp={(e) => this.onKeyUpSearch(true, e)} className={space.inputFind}  /></span>
                        </div>
                        <div className={space.listContent} className={scrollAreaStyle}>
                            <ul>
                                {buildingGroupTreeViewUI}
                            </ul>
                        </div>
                    </div>

                    <div className={space.listBox2}>
                        <div className={space.listTitleComboBox}>
                            <div className={space.selectBox}>
                              <div className={space.select}>
                                <select className={space.selectCom} onChange={(e) => this.onChangeSensorType(e.target.value)}>
                                    <option value={SpaceDataManager.FireSensorType}>화재</option>
                                    <option value={SpaceDataManager.PSMSensorType}>누출</option>
                                    <option value={SpaceDataManager.CCTVType}>CCTV</option>
                                    <option value={SpaceDataManager.EtcSensorType}>ETC</option>
                                </select>
                                </div>
                            </div>
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
                            <span className={space.listIcon2} onClick={() => this.onChangeEditMode()} title="센서편집" ></span>
                            <span className={space.listIcon2_1} onClick={() => this.onAddSensor()} title="센서추가" ></span>
                            <span className={space.downIcon} onClick={this.onDownloadExcel}>엑셀파일 다운로드</span>
                            
                                <input type='file' className={space.galleryFile} ref={this.refFileUpload} onChange={(e) => this.onUploadExcel(e)} />
                                <span className={space.uploadIcon} ref={this.refFileUploadTemp}>엑셀파일 업로드</span>
                            
                            {
                                //<span className={space.uploadIcon}>엑셀파일 업로드</span>
                            }
                        </div>
                        <div className={space.listContScrollbar}>
                            <div className={space.listContent2}>
                            <table className={space.listTable}>
                                <thead>
                                    <tr>                                        
                                            
                                            {
                                                // 화재
                                                (this.state.curSensorType === SpaceDataManager.FireSensorType) ?
                                                    <>
                                                        <th className={space.fireTh1}>센서명</th>
                                                        <th className={space.fireTh2}>TagNo</th>
                                                        <th className={space.fireTh3}>감지기 타입</th>
                                                        <th className={space.fireTh4}>설치장소</th>
                                                        <th className={space.fireTh5}>Zone</th>
                                                    </>
                                                    : null
                                            }
                                            {
                                                // 누출
                                                (this.state.curSensorType === SpaceDataManager.PSMSensorType) ?
                                                    <>
                                                        <th className={space.psmTh1}>센서명</th>
                                                        <th className={space.psmTh2}>고유키</th>
                                                        <th className={space.psmTh3}>물질 이름</th>
                                                        <th className={space.psmTh4}>설치장소</th>
                                                        <th className={space.psmTh5}>Zone</th>
                                                    </>
                                                    : null
                                            }
                                            {
                                                // ETC
                                                (this.state.curSensorType === SpaceDataManager.EtcSensorType) ?
                                                    <>
                                                        <th className={space.etcTh1}>센서명</th>
                                                        <th className={space.etcTh2}>고유키</th>
                                                        <th className={space.etcTh3}>물질 이름</th>
                                                        <th className={space.etcTh4}>설치장소</th>
                                                        <th className={space.etcTh5}>Zone</th>
                                                    </>
                                                    : null
                                            }
                                            {
                                                // CCTV
                                                (this.state.curSensorType === SpaceDataManager.CCTVType) ?
                                                    <>
                                                        <th className={space.cctvTh1}>CCTV명</th>
                                                        <th className={space.cctvTh2}>고유키</th>
                                                        <th className={space.cctvTh3}>Type</th>
                                                        <th className={space.cctvTh4}>UserID</th>
                                                        <th className={space.cctvTh5}>Password</th>
                                                        <th className={space.cctvTh6}>URL</th>
                                                        <th className={space.cctvTh7}>설치장소</th>
                                                        <th className={space.cctvTh8}>Zone</th>
                                                    </>
                                                    : null
                                            }
                                            
                                            {
                                                this.state.isEditMode ?
                                                    <th className={space.editModeTh}></th>
                                                    : null
                                            }
                                    </tr>
                                </thead>
                                <tbody>  
                                        {
                                            <SensorListEdit_gridView        
                                                parentFrm={this}
                                                isEditMode={this.state.isEditMode}
                                                sensors={this.state.viewSensors}//{sensors}
                                                curSensorType={this.state.curSensorType}
                                                curApplyEquipZone={this.state.curApplyEquipZone}
                                                buildingGroupList={this.props.buildingGroupList}
                                                onChangeSensor={this.onChangeSensor}
                                                //selectedRows={this.state.selectedRows}
                                                addSelectedRows={this.addSelectedRows}
                                                removeSelectedRows={this.removeSelectedRows}
                                                sensorTypes={this.props.sensorTypes}
                                                pageIndex={this.state.pageIndex}
                                                searchTextGridView={this.state.searchTextGridView}
                                            />
                                        }                                        
                                </tbody>
                            </table>
                            </div> {/* listContent2 */}
                            
                        </div>
                        <div className={space.hscNav}>
                            <a className={space.first} onClick={() => this.setPageIndex(1)}>맨앞</a>
                            <a className={space.prev} onClick={() => this.setPageIndex(this.state.pageIndex - 1)}>이전</a>
                            <ul>
                                {pageIndexUI}
                            </ul>
                            <a className={space.next} onClick={() => this.setPageIndex(this.state.pageIndex + 1)}>다음</a>
                            <a className={space.last} onClick={() => this.setPageIndex(this.state.maxPageIndex)}>맨뒤</a>
                        </div>
                    </div>

                </div> {/* listArea */}
            </>
        );
    }
}