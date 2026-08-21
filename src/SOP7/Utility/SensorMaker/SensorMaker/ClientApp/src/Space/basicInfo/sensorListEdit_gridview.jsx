import React, { Component } from 'react';
import SensorMakerResource from '../../resource/id';
import { SpaceDataManager } from '../services/spaceDataManager';
import { SpaceBody } from '../spaceBody';
import space from './../css/space.module.css';
import ColComboBox from './columns/colComboBox';
import ColComboBox_SelectBuilding from './columns/colComboBox_SelectBuilding';
import ColText from './columns/colText';
import { SensorListEdit } from './sensorListEdit';
import $ from 'jquery';

export class SensorListEdit_gridView extends Component {
    constructor(props) {
        super(props);
        this.state = {
            isLoading: false
        }

        this.nSumRowCount = 20;
        this.nBeginRowIndex = 0;
        this.nEndRowIndex = 0;
    }

    componentDidMount() {
    }

    componentWillUnmount() {
    }

    onChangeSensor = (chgSensor) => {
        let sensors = this.props.sensors;
        const sensorCount = sensors.length;
        for (let i = 0; i < sensorCount; i++) {
            if (chgSensor.id === sensors[i].id) {
                sensors[i] = chgSensor;
                break;
            }
        }

        this.props.onChangeSensor(sensors);
    }

    onDeleteSensor = (deleteSensor) => {
        let sensors = this.props.sensors;
        let deleteIndex = -1;
        const sensorCount = sensors.length;
        for (let i = 0; i < sensorCount; i++) {
            if (deleteSensor.id === sensors[i].id) {
                deleteIndex = i;
                break;
            }
        }

        sensors.splice(deleteIndex, 1);

        this.props.onChangeSensor(sensors);
    }

    onRowMouseDown = (sensor, index) => {
        console.log(sensor);

    }

    onRowClick = (sensor, index) => {
        const selectedSensors = [...this.props.parentFrm.selectedRows];

        if (SpaceBody.keyPress === SpaceBody.keyPressShift) {
            selectedSensors.length = 0;

            let clickBeginIndex = this.props.parentFrm.nIndexLastSelectedRow < index ? this.props.parentFrm.nIndexLastSelectedRow : index;
            let clickEndIndex = this.props.parentFrm.nIndexLastSelectedRow > index ? this.props.parentFrm.nIndexLastSelectedRow : index;

            const sensorCount = this.props.sensors.length;

            let beginIndex = 0;
            let endIndex = sensorCount;
            if (this.props.pageIndex && this.props.pageIndex > 1) {
                beginIndex = (this.props.pageIndex - 1) * this.props.parentFrm.maxRowCount;
            }
            if (this.props.parentFrm.maxRowCount) {
                endIndex = beginIndex + this.props.parentFrm.maxRowCount;
            }

            let rowIndex = -1;
            for (let i = beginIndex; i < endIndex; i++) {
                if (sensorCount < i + 1) {
                    break;
                }

                const sensor2 = this.props.sensors[i];
                if (this.props.curApplyEquipZone === SensorListEdit.ApplyEquipZone_On) {
                    if (sensor2.equipZoneID === null) {
                        continue;
                    }
                }
                else if (this.props.curApplyEquipZone === SensorListEdit.ApplyEquipZone_Off) {
                    if (sensor2.equipZoneID !== null) {
                        continue;
                    }
                }

                let tempIndex = ++rowIndex;
                if (tempIndex >= clickBeginIndex && tempIndex <= clickEndIndex) {
                    selectedSensors.push(sensor2);
                }
            }
        }
        else if (SpaceBody.keyPress === SpaceBody.keyPressControl) {
            let alreadySelected = false;

            const sensorCount = this.props.sensors.length;

            let beginIndex = 0;
            let endIndex = sensorCount;
            if (this.props.pageIndex && this.props.pageIndex > 1) {
                beginIndex = (this.props.pageIndex - 1) * this.props.parentFrm.maxRowCount;
            }
            if (this.props.parentFrm.maxRowCount) {
                endIndex = beginIndex + this.props.parentFrm.maxRowCount;
            }

            for (let i = beginIndex; i < endIndex; i++) {
                if (sensorCount < i + 1) {
                    break;
                }

                const selectedSensor = selectedSensors[i];
                if (selectedSensor === sensor) {
                    selectedSensors.splice(i, 1);
                    //$('#' + this.props.curSensorType + '_' + sensor.id).removeClass(space.selectedRow);
                    alreadySelected = true;
                    break;
                }
            }

            if (!alreadySelected) {
                selectedSensors.push(sensor);
                //$('#' + this.props.curSensorType + '_' + sensor.id).addClass(space.selectedRow);
            }
        }
        else {
            if (selectedSensors.length === 1 && selectedSensors[0] === sensor) {
                return;
            }
            selectedSensors.length = 0;
            selectedSensors.push(sensor);
            //$('#' + this.props.curSensorType + '_' + sensor.id).addClass(space.selectedRow);
        }

        this.props.parentFrm.nIndexLastSelectedRow = index;

        //this.props.addSelectedRows(selectedSensors);

        for (let i = 0; i < this.props.parentFrm.selectedRows.length; i++) {
            const beforeSensor = this.props.parentFrm.selectedRows[i];
            if (beforeSensor && beforeSensor.id) {
                $('#' + this.props.curSensorType + '_' + beforeSensor.id).removeClass(space.selectedRow);
            }
        }
        
        this.props.parentFrm.selectedRows = selectedSensors;

        for (let i = 0; i < this.props.parentFrm.selectedRows.length; i++) {
            const afterSensor = this.props.parentFrm.selectedRows[i];
            if (afterSensor && afterSensor.id) {
                $('#' + this.props.curSensorType + '_' + afterSensor.id).addClass(space.selectedRow);
            }
        }      
    }

    getRowsUI() {
        let ui = [];
        if (!this.props.sensors) {
            return ui;
        }

        $('.' + space.selectedRow).removeClass(space.selectedRow);

        //let [buildingUI, zoneUI, equipZoneUI] = this.getBuildingGroupList();

        const selectedRowCount = this.props.parentFrm.selectedRows.length;
        const sensorCount = this.props.sensors.length;
        let rowIndex = -1;

        let fireSensorSubTypes = [];
        if (this.props.curSensorType === SpaceDataManager.FireSensorType) {
            for (let i = 0; i < this.props.sensorTypes.length; i++) {
                if (this.props.sensorTypes[i].id === SensorMakerResource.facilityType.FIRE) {
                    fireSensorSubTypes = this.props.sensorTypes[i].subType;
                    break;
                }
            }
        }

        let beginIndex = 0;
        let endIndex = sensorCount;
        if (this.props.pageIndex && this.props.pageIndex > 1) {
            beginIndex = (this.props.pageIndex - 1) * this.props.parentFrm.maxRowCount;
        }
        if (this.props.parentFrm.maxRowCount) {
            endIndex = beginIndex + this.props.parentFrm.maxRowCount;
        }

        for (let i = /*0*/beginIndex; i < endIndex; i++) {
            if (sensorCount < i + 1) {
                break;
            }

            const sensor = this.props.sensors[i];
            if (!sensor) {
                break;
            }
            if (this.props.curApplyEquipZone === SensorListEdit.ApplyEquipZone_On) {
                if (sensor.equipZoneID === null) {
                    continue;
                }
            }
            else if (this.props.curApplyEquipZone === SensorListEdit.ApplyEquipZone_Off) {
                if (sensor.equipZoneID !== null) {
                    continue;
                }
            }

            //let isSelectedRow = false;
            let selectedRowClassName = null; //(this.props.selectedRows.find(sensor)) ? space.selectedRow : null;
            for (let j = 0; j < selectedRowCount; j++) {
                if (this.props.parentFrm.selectedRows[j].id === sensor.id) {
                    selectedRowClassName = space.selectedRow;
                    //isSelectedRow = true;
                    break;                    
                }
            }          
            let tempIndex = ++rowIndex;
            if (this.props.curSensorType === SpaceDataManager.FireSensorType) {
                ui.push(
                    <tr key={this.props.curSensorType + '_' + sensor.id} id={this.props.curSensorType + '_' + sensor.id} ref={this.refRow} draggable={true} className={selectedRowClassName} onClick={() => this.onRowClick(sensor, tempIndex)} onMouseDown={() => this.onRowMouseDown(sensor, tempIndex)}>
                        <td className={space.fireTd1}><ColText sensor={sensor} valueType={'name'} value={sensor.name} isEditMode={this.props.isEditMode} onChangeSensor={this.onChangeSensor} className={space.FireTdName} /></td>
                        <td className={space.fireTd2}><ColText sensor={sensor} valueType={'tagNo'} value={sensor.tagNo} isEditMode={this.props.isEditMode} onChangeSensor={this.onChangeSensor} /></td>
                        {
                            //<td><span>{sensor.sensorSubType}</span></td>
                            <td className={space.fireTd3}><ColComboBox sensor={sensor} valueType={'sensorSubType'} options={fireSensorSubTypes} value={sensor.sensorSubType} isEditMode={this.props.isEditMode} onChangeSensor={this.onChangeSensor} /></td>
                        }
                        <td className={space.fireTd4}><ColText sensor={sensor} valueType={'positionName'} value={sensor.positionName} isEditMode={this.props.isEditMode} onChangeSensor={this.onChangeSensor} /></td>
                        {
                            //<td><span>Zone</span></td>
                            <td className={space.fireTd5}><ColComboBox_SelectBuilding
                                sensor={sensor}
                                buildingGroupList={this.props.buildingGroupList}
                                buildingID={sensor.buildingID} zoneID={sensor.zoneID} equipZoneID={sensor.equipZoneID}
                                isEditMode={this.props.isEditMode}
                                onChangeSensor={this.onChangeSensor} /></td>
                        }
                        {
                            (this.props.isEditMode) ?
                                <td className={space.fireTd6}><span className={space.listIcon3} onClick={() => this.onDeleteSensor(sensor)}></span></td>
                                : null
                        }
                    </tr>
                );
            }
            else if (this.props.curSensorType === SpaceDataManager.PSMSensorType) {
                ui.push(
                    <tr key={this.props.curSensorType + '_' + sensor.id} id={this.props.curSensorType + '_' + sensor.id} ref={this.refRow} draggable={true} className={selectedRowClassName} onClick={() => this.onRowClick(sensor, tempIndex)}>
                        <td className={space.psmTd1}><ColText sensor={sensor} valueType={'name'} value={sensor.name} isEditMode={this.props.isEditMode} onChangeSensor={this.onChangeSensor} /></td>
                        <td className={space.psmTd2}><ColText sensor={sensor} valueType={'uniqueKey'} value={sensor.uniqueKey} isEditMode={this.props.isEditMode} onChangeSensor={this.onChangeSensor} /></td>
                        <td className={space.psmTd3}><ColText sensor={sensor} valueType={'materialType'} value={sensor.materialType} isEditMode={this.props.isEditMode} onChangeSensor={this.onChangeSensor} /></td>
                        <td className={space.psmTd4}><ColText sensor={sensor} valueType={'positionName'} value={sensor.positionName} isEditMode={this.props.isEditMode} onChangeSensor={this.onChangeSensor} /></td>
                        {
                            <td className={space.psmTd5}><ColComboBox_SelectBuilding
                                sensor={sensor}
                                buildingGroupList={this.props.buildingGroupList}
                                buildingID={sensor.buildingID} zoneID={sensor.zoneID} equipZoneID={sensor.equipZoneID}
                                isEditMode={this.props.isEditMode}
                                onChangeSensor={this.onChangeSensor} /></td>
                        }
                        {
                            (this.props.isEditMode) ?
                                <td className={space.psmTd6}><span className={space.listIcon3} onClick={() => this.onDeleteSensor(sensor)}></span></td>
                                : null
                        }
                    </tr>
                );
            }
            else if (this.props.curSensorType === SpaceDataManager.EtcSensorType) {
                ui.push(
                    <tr key={this.props.curSensorType + '_' + sensor.id} id={this.props.curSensorType + '_' + sensor.id} ref={this.refRow} draggable={true} className={selectedRowClassName} onClick={() => this.onRowClick(sensor, tempIndex)}>
                        <td className={space.etcTd1}><ColText sensor={sensor} valueType={'name'} value={sensor.name} isEditMode={this.props.isEditMode} onChangeSensor={this.onChangeSensor} /></td>
                        <td className={space.etcTd2}><ColText sensor={sensor} valueType={'uniqueKey'} value={sensor.uniqueKey} isEditMode={this.props.isEditMode} onChangeSensor={this.onChangeSensor} /></td>
                        <td className={space.etcTd3}><ColText sensor={sensor} valueType={'materialType'} value={sensor.materialType} isEditMode={this.props.isEditMode} onChangeSensor={this.onChangeSensor} /></td>
                        <td className={space.etcTd4}><ColText sensor={sensor} valueType={'positionName'} value={sensor.positionName} isEditMode={this.props.isEditMode} onChangeSensor={this.onChangeSensor} /></td>
                        {
                            <td className={space.etcTd5}><ColComboBox_SelectBuilding
                                sensor={sensor}
                                buildingGroupList={this.props.buildingGroupList}
                                buildingID={sensor.buildingID} zoneID={sensor.zoneID} equipZoneID={sensor.equipZoneID}
                                isEditMode={this.props.isEditMode}
                                onChangeSensor={this.onChangeSensor} /></td>
                        }
                        {
                            (this.props.isEditMode) ?
                                <td className={space.etcTd6}><span className={space.listIcon3} onClick={() => this.onDeleteSensor(sensor)}></span></td>
                                : null
                        }
                    </tr>
                );
            }

            /* cctv type */
            else if (this.props.curSensorType === SpaceDataManager.CCTVType) {
                ui.push(
                    <tr key={this.props.curSensorType + '_' + sensor.id} id={this.props.curSensorType + '_' + sensor.id} ref={this.refRow} draggable={true} className={selectedRowClassName} onClick={() => this.onRowClick(sensor, tempIndex)}>
                        <td className={space.cctvTd1}><ColText sensor={sensor} valueType={'name'} value={sensor.name} isEditMode={this.props.isEditMode} onChangeSensor={this.onChangeSensor} /></td>
                        <td className={space.cctvTd2}><ColText sensor={sensor} valueType={'uniqueKey'} value={sensor.uniqueKey} isEditMode={this.props.isEditMode} onChangeSensor={this.onChangeSensor} /></td>
                        <td className={space.cctvTd3}><ColText sensor={sensor} valueType={'type'} value={sensor.type} isEditMode={this.props.isEditMode} onChangeSensor={this.onChangeSensor} /></td>
                        <td className={space.cctvTd4}><ColText sensor={sensor} valueType={'userID'} value={sensor.userID} isEditMode={this.props.isEditMode} onChangeSensor={this.onChangeSensor} /></td>
                        <td className={space.cctvTd5}><ColText sensor={sensor} valueType={'password'} value={sensor.password} isEditMode={this.props.isEditMode} onChangeSensor={this.onChangeSensor} /></td>
                        <td className={space.cctvTd6}><ColText sensor={sensor} valueType={'url'} value={sensor.url} isEditMode={this.props.isEditMode} onChangeSensor={this.onChangeSensor} /></td>
                        <td className={space.cctvTd7}><ColText sensor={sensor} valueType={'positionName'} value={sensor.positionName} isEditMode={this.props.isEditMode} onChangeSensor={this.onChangeSensor} /></td>

                        {
                            <td className={space.cctvTd8}><ColComboBox_SelectBuilding
                                sensor={sensor}
                                buildingGroupList={this.props.buildingGroupList}
                                buildingID={sensor.buildingID} zoneID={sensor.zoneID} equipZoneID={sensor.equipZoneID}
                                isEditMode={this.props.isEditMode}
                                onChangeSensor={this.onChangeSensor} /></td>
                        }
                        {
                            (this.props.isEditMode) ?
                                <td className={space.cctvTd9}><span className={space.listIcon3} onClick={() => this.onDeleteSensor(sensor)}></span></td>
                                : null
                        }
                    </tr>
                );
            }
        }

        return ui;
    }

    render() {        
        const rowsUI = this.getRowsUI();
        return (
            <>
                {rowsUI}
            </>
        );
    }
}