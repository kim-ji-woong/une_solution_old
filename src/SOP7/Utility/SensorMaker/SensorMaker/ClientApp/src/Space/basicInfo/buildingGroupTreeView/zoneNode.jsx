import React, { Component } from 'react';
import space from './../../css/space.module.css';
import styles from '../../css/spatial.module.css';

import $ from 'jquery';
import { EquipZoneNode } from './equipZoneNode';
import { SpaceDataManager } from '../../services/spaceDataManager';
import SensorMakerResource from '../../../resource/id';
import { SensorNode } from './sensorNode';

export class ZoneNode extends Component {
    constructor(props) {
        super(props);
        this.state = {
            searchText: ''
        }
    }

    componentDidMount() {
    }

    getUI() {
        let ui = [];
        const equipmentZoneDatas = this.props.zone.equipmentZoneDatas;
        const equipmentZoneDataCount = equipmentZoneDatas.length;
        for (let i = 0; i < equipmentZoneDataCount; i++) {
            const equipZone = equipmentZoneDatas[i];
            if (equipZone.visibleTreeView === false && this.props.searchText.length > 0) {
                continue;
            }

            ui.push(
                <EquipZoneNode
                    key={'node_equipZone_' + equipZone.id}
                    zone={this.props.zone}
                    equipZone={equipZone}
                    sensorList={this.props.sensorList}
                    selectedRows={this.props.selectedRows}
                    onChangeSensor={this.props.onChangeSensor}
                    curSensorType={this.props.curSensorType}
                    selectedNodes={this.props.selectedNodes}
                    addSelectedNodes={this.props.addSelectedNodes}
                    removeSelectedNodes={this.props.removeSelectedNodes}  
                    isEditMode={this.props.isEditMode}
                    selectedMenu={this.props.selectedMenu}
                    parentFrm={this.props.parentFrm}
                    searchText={this.props.searchText}
                />);
        }

        return ui;
    }

    onSensorNodeClick = (sensor, index) => {
        if (this.props.isEditMode) {
            this.props.addSelectedNodes([sensor]);
        }
    }

    getSensorUI(sensors, facilityType, tag) {
        const ui = [];
        let rowIndex = -1;

        for (const sensor of sensors) {
            const tempIndex = ++rowIndex;

            ui.push(
                <SensorNode
                    key={tag + sensor.id}
                    facilityType={SensorMakerResource.facilityType.FIRE}
                    sensor={sensor}
                    selectedNodes={this.props.selectedNodes}
                    addSelectedNodes={this.props.addSelectedNodes}
                    removeSelectedNodes={this.props.removeSelectedNodes}
                    onNodeClick={this.onSensorNodeClick}
                    index={tempIndex}
                    isEditMode={this.props.isEditMode}
                />);
        }

        return ui;
    }

    getSensorsUI() {
        const ui = [];
        const sensorList = this.props.sensorList;

        if (sensorList) {
            let fireIndex = 0, psmIndex = 0, etcIndex = 0, cctvIndex = 0;

            for (const sensorType in sensorList) {
                if (sensorType === SpaceDataManager.FireSensorType) {
                    ui.push(
                        <li key={"fire_" + fireIndex++}>화재
                            {
                                this.getSensorUI(sensorList[sensorType], SensorMakerResource.facilityType.FIRE, 'sensor_fire')
                            }
                        </li>
                    );
                }
                else if (sensorType === SpaceDataManager.PSMSensorType) {
                    ui.push(
                        <li key={"psm_" + psmIndex++}>누출
                            {
                                this.getSensorUI(sensorList[sensorType], SensorMakerResource.facilityType.PSM_SENSOR, 'sensor_psm')
                            }
                        </li>
                    );
                }
                else if (sensorType === SpaceDataManager.EtcSensorType) {
                    ui.push(
                        <li key={"etc_" + etcIndex++}>ETC
                            {
                                this.getSensorUI(sensorList[sensorType], SensorMakerResource.facilityType.ETC, 'sensor_etc')
                            }
                        </li>
                    );
                }
                else if (sensorType === SpaceDataManager.CCTVType) {
                    ui.push(
                        <li key={"cctv_" + cctvIndex++}>CCTV
                            {
                                this.getSensorUI(sensorList[sensorType], SensorMakerResource.facilityType.Security_Sensor, 'sensor_cctv')
                            }
                        </li>
                    );
                }
            }
        }

        return ui;
    }

    sensorDisplayMode() {
        if (this.props.showEquipZone) {
            return (
                <ul>
                    {
                        this.getUI()
                    }
                </ul>
            );
        }

        return (
            <ul>
                {
                    this.getSensorsUI()
                }
            </ul>
            );
    }

    render() {
        if (this.props.sensorDisplayMode) {
            return this.sensorDisplayMode();
        }

        const zoneText = this.props.zone?.displayText;
        const ui = this.getUI();

        return (
            <li><span className={styles.poiTreeZoneIcon}></span>{zoneText}
                {
                    this.props.dashboard ?
                     <div className={styles.treeIconImageAreaPoi}>
                       <span className={styles.goLink} onClick={this.moveToX} style={{ cursor: 'pointer' }}><a>이동</a></span>
                     </div>
                     :null
                }
                <ul>
                    {ui}
                </ul>
            </li>
        );
    }
}