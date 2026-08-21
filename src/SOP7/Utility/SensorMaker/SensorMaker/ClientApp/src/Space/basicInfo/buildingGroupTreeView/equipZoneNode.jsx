import React, { Component } from 'react';
import space from './../../css/space.module.css';
import styles from '../../css/spatial.module.css';

import $ from 'jquery';
import { SensorNode } from './sensorNode';
import SensorMakerResource from '../../../resource/id';
import { SpaceDataManager } from '../../services/spaceDataManager';
import { DeliveryManager } from '../worker/deliveryManager';
import { SensorListEdit } from '../sensorListEdit';
import { SpaceBody } from '../../spaceBody';
import { SpaceMenus } from '../../spaceMenus';

export class EquipZoneNode extends Component {
    constructor(props) {
        super(props);
        this.state = {
            isSelectedNode: false
        }

        this.deliveryManager = new DeliveryManager(this);
        this.refNode = React.createRef();
    }

    componentDidMount() {
        this.deliveryManager.initDragEvents(this.refNode.current);

    }

    componentDidUpdate(prevProps, prevState) {
        if (this.deliveryManager.initDragDrop === false) {
            this.deliveryManager.initDragEvents(this.refNode.current);
        }
    }

    componentWillUnmount() {        
        this.deliveryManager.resetDragEvents(this.refNode.current);
    }

    onChangeSensorZone = (chgSensors) => {
        if (this.props.isEditMode) {
            const chgSensorCount = chgSensors.length;
            const allSensors = this.props.sensorList[this.props.curSensorType];
            const allSensorCount = allSensors.length;

            for (let i = 0; i < chgSensorCount; i++) {
                for (let j = 0; j < allSensorCount; j++) {
                    
                    if (chgSensors[i].id === allSensors[j].id) {
                        if (this.props.selectedMenu === SpaceMenus.EditSensorList) {
                            // EquipZone 변경
                            allSensors[j].buildingID = this.props.zone.buildingID;
                            allSensors[j].zoneID = this.props.zone.id;
                            allSensors[j].equipZoneID = this.props.equipZone.id;                            
                        }
                        else if (this.props.selectedMenu === SpaceMenus.EditEquipZoneCCTVs) {
                            // EquipZone에 추가
                            if (!allSensors[j].equipZoneIDs) {
                                allSensors[j].equipZoneIDs = [];
                            }

                            if (allSensors[j].equipZoneIDs.indexOf(this.props.equipZone.id) === -1) {
                                allSensors[j].equipZoneIDs.push(this.props.equipZone.id);
                            }
                        }

                        break;
                    }
                }
            }
            this.props.onChangeSensor(allSensors);
        }
    }

    onNodeClick = (sensor, index) => {        
        const selectedSensors = [...this.props.selectedNodes];
        if (selectedSensors.length === 1 && selectedSensors[0] === sensor) {
            // 같은 Node click
            return;
        }

        let selectedSensorType = SpaceDataManager.FireSensorType;
        if (SensorMakerResource.isPSMSensorType(sensor.sensorType)) {
            selectedSensorType = SpaceDataManager.PSMSensorType;
        }
        else if (SensorMakerResource.isETCSensorType(sensor.sensorType)) {
            selectedSensorType = SpaceDataManager.EtcSensorType;
        }
        else if (SensorMakerResource.isSVMSSensorType(sensor.sensorType)) {
            selectedSensorType = SpaceDataManager.CCTVType;
        }

        if (SpaceBody.keyPress === SpaceBody.keyPressShift) {
            // 구벽열 cctv 편집 메뉴에서는 CCTV가 실제 존재하는 EquipZone이 아닌 포함되어 있는 EquipZoneIDs의 내용과 비교한다
            if (this.props.selectedMenu === SpaceMenus.EditEquipZoneCCTVs) {
                if (sensor.equipZoneIDs.indexOf(this.props.parentFrm.nLastSelectedNodeEquipZoneID) >= 0) {
                    let sameEquipZoneSensors = [];
                    const allSensors = this.props.sensorList[selectedSensorType];
                    const allSensorCount = allSensors.length;
                    for (let i = 0; i < allSensorCount; i++) {
                        const allSensor = allSensors[i];
                        if (allSensor.equipZoneIDs.indexOf(this.props.parentFrm.nLastSelectedNodeEquipZoneID) >= 0) {
                            sameEquipZoneSensors.push(allSensor);
                        }
                    }

                    let beginIndex = this.props.parentFrm.nIndexLastSelectedNode < index ? this.props.parentFrm.nIndexLastSelectedNode : index;
                    let endIndex = this.props.parentFrm.nIndexLastSelectedNode > index ? this.props.parentFrm.nIndexLastSelectedNode : index;

                    for (let i = beginIndex; i <= endIndex; i++) {
                        const sensor2 = sameEquipZoneSensors[i];
                        selectedSensors.push(sensor2);
                    }
                }
                else {
                    selectedSensors.length = 0;
                    selectedSensors.push(sensor);
                }
            }
            else {
                if (this.props.parentFrm.nLastSelectedNodeEquipZoneID === sensor.equipZoneID) {
                    let sameEquipZoneSensors = [];
                    const allSensors = this.props.sensorList[selectedSensorType];
                    const allSensorCount = allSensors.length;
                    for (let i = 0; i < allSensorCount; i++) {
                        const allSensor = allSensors[i];
                        if (allSensor.equipZoneID === sensor.equipZoneID) {
                            sameEquipZoneSensors.push(allSensor);
                        }
                    }

                    let beginIndex = this.props.parentFrm.nIndexLastSelectedNode < index ? this.props.parentFrm.nIndexLastSelectedNode : index;
                    let endIndex = this.props.parentFrm.nIndexLastSelectedNode > index ? this.props.parentFrm.nIndexLastSelectedNode : index;

                    for (let i = beginIndex; i <= endIndex; i++) {
                        const sensor2 = sameEquipZoneSensors[i];
                        selectedSensors.push(sensor2);
                    }
                }
                else {
                    selectedSensors.length = 0;
                    selectedSensors.push(sensor);
                }
            }
        }
        else if (SpaceBody.keyPress === SpaceBody.keyPressControl) {
            let alreadySelected = false;
            const selectedSensorCount = selectedSensors.length;
            for (let i = 0; i < selectedSensorCount; i++) {
                const selectedSensor = selectedSensors[i];
                if (selectedSensor === sensor) {
                    selectedSensors.splice(i, 1);
                    alreadySelected = true;
                    break;
                }
            }

            if (!alreadySelected) {
                selectedSensors.push(sensor);
            }
        }
        else {
            selectedSensors.length = 0;
            selectedSensors.push(sensor);
        }

        this.props.parentFrm.nLastSelectedNodeEquipZoneID = this.props.equipZone.id;//sensor.equipZoneID;
        this.props.parentFrm.nIndexLastSelectedNode = index;

        this.props.addSelectedNodes(selectedSensors);
    }

    getUI() {
        let fireSensorUI = [];
        let psmSensorUI = [];
        let etcSensorUI = [];
        let cctvUI = [];

        if (this.props.selectedMenu === SpaceMenus.EditSensorList || this.props.selectedMenu === SpaceMenus.EditPois) {
            if (this.props.sensorList[SpaceDataManager.FireSensorType]) {
                const fireSensors = this.props.sensorList[SpaceDataManager.FireSensorType];

                let rowIndex = -1;

                const fireSensorCount = fireSensors.length;
                for (let i = 0; i < fireSensorCount; i++) {
                    const fireSensor = fireSensors[i];
                    if (fireSensor.equipZoneID === this.props.equipZone.id) {
                        if (fireSensor.visibleTreeView === false && this.props.searchText.length > 0) {
                            continue;
                        }

                        const tempIndex = ++rowIndex;
                        fireSensorUI.push(
                            <SensorNode
                                key={'sle_fire_' + fireSensor.id}
                                facilityType={SensorMakerResource.facilityType.FIRE}
                                sensor={fireSensor}
                                selectedNodes={this.props.selectedNodes}
                                addSelectedNodes={this.props.addSelectedNodes}
                                removeSelectedNodes={this.props.removeSelectedNodes}
                                onNodeClick={this.onNodeClick}
                                index={tempIndex}
                                isEditMode={this.props.isEditMode}
                            />);
                    }
                }
            }

            if (this.props.sensorList[SpaceDataManager.PSMSensorType]) {
                const psmSensors = this.props.sensorList[SpaceDataManager.PSMSensorType];

                let rowIndex = -1;

                const psmSensorCount = psmSensors.length;
                for (let i = 0; i < psmSensorCount; i++) {
                    const psmSensor = psmSensors[i];
                    if (psmSensor.equipZoneID === this.props.equipZone.id) {
                        if (psmSensor.visibleTreeView === false && this.props.searchText.length > 0) {
                            continue;
                        }

                        const tempIndex = ++rowIndex;
                        psmSensorUI.push(
                            <SensorNode
                                key={'sle_psm_' + psmSensor.id}
                                facilityType={SensorMakerResource.facilityType.PSM_SENSOR}
                                sensor={psmSensor}
                                selectedNodes={this.props.selectedNodes}
                                addSelectedNodes={this.props.addSelectedNodes}
                                removeSelectedNodes={this.props.removeSelectedNodes}
                                onNodeClick={this.onNodeClick}
                                index={tempIndex}
                                isEditMode={this.props.isEditMode}
                            />);
                    }
                }
            }

            if (this.props.sensorList[SpaceDataManager.EtcSensorType]) {
                const etcSensors = this.props.sensorList[SpaceDataManager.EtcSensorType];

                let rowIndex = -1;

                const etcSensorCount = etcSensors.length;
                for (let i = 0; i < etcSensorCount; i++) {
                    const etcSensor = etcSensors[i];
                    if (etcSensor.visibleTreeView === false && this.props.searchText.length > 0) {
                        continue;
                    }
                    if (etcSensor.equipZoneID === this.props.equipZone.id) {

                        const tempIndex = ++rowIndex;
                        etcSensorUI.push(
                            <SensorNode
                                key={'sle_etc_' + etcSensor.id}
                                facilityType={SensorMakerResource.facilityType.ETC}
                                sensor={etcSensor}
                                selectedNodes={this.props.selectedNodes}
                                addSelectedNodes={this.props.addSelectedNodes}
                                removeSelectedNodes={this.props.removeSelectedNodes}
                                onNodeClick={this.onNodeClick}
                                index={tempIndex}
                                isEditMode={this.props.isEditMode}
                            />);
                    }
                }
            }

            if (this.props.sensorList[SpaceDataManager.CCTVType]) {
                const cctvs = this.props.sensorList[SpaceDataManager.CCTVType];

                let rowIndex = -1;

                const cctvCount = cctvs.length;
                for (let i = 0; i < cctvCount; i++) {
                    const cctv = cctvs[i];
                    if (cctv.visibleTreeView === false && this.props.searchText.length > 0) {
                        continue;
                    }

                    if (cctv.equipZoneID === this.props.equipZone.id) {

                        const tempIndex = ++rowIndex;
                        cctvUI.push(
                            <SensorNode
                                key={'sle_cctv_' + cctv.id}
                                facilityType={SensorMakerResource.facilityType.Security_Sensor}
                                sensor={cctv}
                                selectedNodes={this.props.selectedNodes}
                                addSelectedNodes={this.props.addSelectedNodes}
                                removeSelectedNodes={this.props.removeSelectedNodes}
                                onNodeClick={this.onNodeClick}
                                index={tempIndex}
                                isEditMode={this.props.isEditMode}                                
                            />);
                    }
                }
            }
        }
        else if (this.props.selectedMenu === SpaceMenus.EditEquipZoneCCTVs) {
            if (this.props.sensorList[SpaceDataManager.CCTVType]) {
                const cctvs = this.props.sensorList[SpaceDataManager.CCTVType];

                let rowIndex = -1;

                const cctvCount = cctvs.length;
                for (let i = 0; i < cctvCount; i++) {
                    const cctv = cctvs[i];                    
                    if (cctv.equipZoneIDs.indexOf(this.props.equipZone.id) >= 0) {
                        const tempIndex = ++rowIndex;
                        cctvUI.push(
                            <SensorNode
                                key={'sle_ez_' + this.props.equipZone.id + '_cctv_' + cctv.id}
                                facilityType={SensorMakerResource.facilityType.Security_Sensor}
                                sensor={cctv}
                                selectedNodes={this.props.selectedNodes}
                                addSelectedNodes={this.props.addSelectedNodes}
                                removeSelectedNodes={this.props.removeSelectedNodes}
                                onNodeClick={this.onNodeClick}
                                index={tempIndex}
                                isEditMode={this.props.isEditMode}
                                curSensorType={this.props.curSensorType}
                                selectedMenu={this.props.selectedMenu}
                                equipZoneID={this.props.equipZone.id}                                
                                parentFrm={this.props.parentFrm}
                                searchText={this.props.searchText}
                            />);
                    }
                }
            }
        }

        return [fireSensorUI, psmSensorUI, etcSensorUI, cctvUI];
    }

    render() {
        const equipZoneText = this.props.equipZone?.displayText;
        const [fireSensorUI, psmSensorUI, etcSensorUI, cctvUI] = this.getUI();

        let selectedRowClassName = (this.state.isSelectedNode) ? space.selectedRow : null;

        return (
            <li ref={this.refNode} className={selectedRowClassName}><span className={styles.poiTreeEquipIcon}></span>
                {equipZoneText}
                <ul>
                    {
                        this.props.selectedMenu === SpaceMenus.EditEquipZoneCCTVs ?
                            <li><span className={styles.poiTreeCCTVIcon}></span>CCTV
                                {cctvUI}
                            </li>
                            :
                            <>
                                <li><span className={styles.poiTreeFireIcon}></span>화재
                                    {fireSensorUI}
                                </li>
                                <li><span className={styles.poiTreeLeakIcon}></span>누출
                                    {psmSensorUI}
                                </li>
                                <li><span className={styles.poiTreeETCIcon}></span>ETC
                                    {etcSensorUI}
                                </li>
                                <li><span className={styles.poiTreeCCTVIcon}></span>CCTV
                                    {cctvUI}
                                </li>
                            </>
                    }
                </ul>
            </li>
        );
    }
}