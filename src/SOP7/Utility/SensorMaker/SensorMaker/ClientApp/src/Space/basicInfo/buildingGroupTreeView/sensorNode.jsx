import React, { Component } from 'react';
import space from './../../css/space.module.css';
import { SpaceMenus } from '../../spaceMenus';
import $ from 'jquery';
import { EquipZoneNode } from './equipZoneNode';
import { SensorListEdit } from '../sensorListEdit';
import { SpaceDataManager } from '../../services/spaceDataManager';

export class SensorNode extends Component {
    constructor(props) {
        super(props);
        this.state = {
            
        }

        this.refSensorNode = React.createRef();
    }

    componentDidMount() {

    }

    componentWillUnmount() {
    }

    getSelectedNode() {
        if (this.props.selectedNodes) {
            const selectedSensors = [...this.props.selectedNodes];
            const selectedSensorCount = selectedSensors.length;
            for (let i = 0; i < selectedSensorCount; i++) {
                const selectedSensor = selectedSensors[i];
                if (this.props.selectedMenu === SpaceMenus.EditEquipZoneCCTVs) {
                    if (this.props.parentFrm.nLastSelectedNodeEquipZoneID === this.props.equipZoneID) {
                        if (selectedSensor === this.props.sensor) {
                            return true;
                        }
                    }
                }
                else {
                    if (selectedSensor === this.props.sensor) {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    static hasPosition(sensor) {
        if (sensor.x !== undefined && sensor.x !== null &&
            sensor.y !== undefined && sensor.y !== null &&
            sensor.z !== undefined && sensor.z !== null) {
            return true;
        }

        return false;
    }

    getClassName(isSelected) {
        const sensor = this.props.sensor;

        if (isSelected) {            
            if (sensor && SensorNode.hasPosition(sensor)) {
                return space.selectedRow;
            }
            else {
                return space.selectedRow + " " + space.noPositionSensor;
            }
        }
        else {
            if (sensor && SensorNode.hasPosition(sensor)) {
                return null;
            }
            else {
                return space.noPositionSensor;
            }
        }
    }

    render() {
        const isSelected = this.getSelectedNode();

        return (
            <ul className={this.getClassName(isSelected)}
                ref={this.refSensorNode}
                draggable={true}
                onMouseUp={() => this.props.onNodeClick(this.props.sensor, this.props.index)}>
                {this.props.sensor.name}
            </ul>
        );
    }
}