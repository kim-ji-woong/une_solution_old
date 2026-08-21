import React, { Component } from 'react';
//import styles from '../../../Common/css/style.module.css';
//import uneStyles from '../../../Common/css/uneCommon.module.css';
import space from '../../css/space.module.css';

class ColComboBox_SelectBuilding extends Component {
    constructor(props) {
        super(props);
        this.state =
        {
        };

        this.props = props;

        if (this.state.value == null) {
            this.state.value = "";
        }
    }

    onChangeBuildingCheck = (e) => {
        let buildingID = Number(e.target.value);
        let sensor = this.props.sensor;
        sensor.buildingID = buildingID;
        sensor.zoneID = -1;
        sensor.equipZoneID = null;

        this.props.onChangeSensor(sensor);
    }

    onChangeZoneCheck = (e) => {
        let zoneID = Number(e.target.value);
        let sensor = this.props.sensor;
        sensor.zoneID = zoneID;
        sensor.equipZoneID = null;

        this.props.onChangeSensor(sensor);
    }

    onChangeEquipZoneCheck = (e) => {
        let equipZoneID = Number(e.target.value);
        let sensor = this.props.sensor;
        sensor.equipZoneID = equipZoneID;

        this.props.onChangeSensor(sensor);
    }

    getComboUI() {
        let buildingUI = [];        
        let zoneUI = [];
        let equipZoneUI = [];

        buildingUI.push(<option key={'cbBuilding_' + this.props.sensor.id + '_-1'} value={null} className={space.buildingOpt} >Unknown</option>);
        zoneUI.push(<option key={'cbZone_' + this.props.sensor.id + '_-1'} value={-1}>Unknown</option>);
        equipZoneUI.push(<option key={'cbEquipZone_' + this.props.sensor.id + '_-1'} value={null}>Unknown</option>);

        const buildingGroupList = this.props.buildingGroupList;
        const buildingGroupCount = buildingGroupList.length;
        for (let i = 0; i < buildingGroupCount; i++) {
            const buildingGroup = buildingGroupList[i];
            //if (buildingGroup.visible) {
                const buildingCount = buildingGroup.buildingDatas.length;
                for (let j = 0; j < buildingCount; j++) {
                    const building = buildingGroup.buildingDatas[j];                    
                    buildingUI.push(<option key={'cbBuilding_' + this.props.sensor.id + '_' + building.id} value={building.id}>{building.displayText}</option>);

                    if (building.id === this.props.buildingID) {
                        const zoneDataCount = building.zoneDatas.length;
                        for (let k = 0; k < zoneDataCount; k++) {
                            const zone = building.zoneDatas[k];
                            zoneUI.push(<option key={'cbZone_' + this.props.sensor.id + '_' + zone.id}value={zone.id}>{zone.displayText}</option>);

                            if (zone.id === this.props.zoneID) {
                                const equipZoneCount = zone.equipmentZoneDatas.length;
                                for (let q = 0; q < equipZoneCount; q++) {
                                    const equipZone = zone.equipmentZoneDatas[q];
                                    equipZoneUI.push(<option key={'cbEquipZone_' + this.props.sensor.id + '_' + equipZone.id} value={equipZone.id}>{equipZone.displayText}</option>);
                                }
                            }
                        }
                    }
                }
            //}
        }

        return [buildingUI, zoneUI, equipZoneUI];
    }

    getZoneName() {
        let buildingName = 'Unknown';
        let zoneName = 'Unknown';
        let equipZoneName = 'Unknown';

        if (this.props.buildingID && this.props.buildingID > 0) {           
        const buildingGroupList = this.props.buildingGroupList;
        const buildingGroupCount = buildingGroupList.length;
            for (let i = 0; i < buildingGroupCount; i++) {
                const buildingGroup = buildingGroupList[i];
                const buildingCount = buildingGroup.buildingDatas.length;
                for (let j = 0; j < buildingCount; j++) {
                    const building = buildingGroup.buildingDatas[j];
                    if (building.id === this.props.buildingID) {
                        buildingName = building.displayText;

                        if (!this.props.zoneID || this.props.zoneID <= 0) {
                            break;
                        }

                        const zoneDataCount = building.zoneDatas.length;
                        for (let k = 0; k < zoneDataCount; k++) {
                            const zone = building.zoneDatas[k];                            
                            if (zone.id === this.props.zoneID) {
                                zoneName = zone.displayText;
                                if (!this.props.equipZoneID || this.props.equipZoneID <= 0) {
                                    break;
                                }

                                const equipZoneCount = zone.equipmentZoneDatas.length;
                                for (let q = 0; q < equipZoneCount; q++) {
                                    const equipZone = zone.equipmentZoneDatas[q];
                                    if (equipZone.id === this.props.equipZoneID) {
                                        equipZoneName = equipZone.displayText;
                                        break;
                                    }
                                }

                                break;
                            }
                        }

                        break;
                    }
                }
            }
        }

        return [buildingName, zoneName, equipZoneName];
    }

    render() {

        let [buildingUI, zoneUI, equipZoneUI] = [[], [], []];
        if (this.props.isEditMode) {
            [buildingUI, zoneUI, equipZoneUI] = this.getComboUI();
        }
        const [buildingName, zoneName, equipZoneName] = this.getZoneName();
        let displayPositionName = '';
        if (buildingName === 'Unknown' && zoneName === 'Unknown' && equipZoneName === 'Unknown') {
            displayPositionName = 'Unknown';
        }
        else {
            displayPositionName = buildingName + ' > ' + zoneName + ' > ' + equipZoneName;
        }
        
        return (
            <>
                {
                    (this.props.isEditMode)  
                        ?
                        <>
                            <select className={space.sensorEditSelect} onChange={(e) => this.onChangeBuildingCheck(e)} defaultValue={this.props.buildingID} autoFocus>
                                {
                                    buildingUI
                                }
                            </select>
                            <select className={space.sensorEditSelect2} onChange={(e) => this.onChangeZoneCheck(e)} defaultValue={this.props.zoneID} autoFocus>
                                {
                                    zoneUI
                                }
                            </select>
                            <select className={space.sensorEditSelect3} onChange={(e) => this.onChangeEquipZoneCheck(e)} defaultValue={this.props.equipZoneID} autoFocus>
                                {
                                    equipZoneUI
                                }
                            </select>
                        </>
                        :
                        <>
                            <span className={space.sensorEditMar}>{displayPositionName}</span>
                        </>
                }
            </>
            );
    }
}

export default ColComboBox_SelectBuilding;