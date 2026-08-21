import React, { Component } from 'react';
import space from './../../css/space.module.css';
import styles from '../../css/spatial.module.css';

import $ from 'jquery';
import { BuildingNode } from './buildingNode';

export class BuildingGroupNode extends Component {
    constructor(props) {
        super(props);
        this.state = {
            searchText: '',
        }
    }

    componentDidMount() {

    }

    getUI() {
        let ui = [];
        const buildingGroup = this.props.buildingGroup;
        if (!buildingGroup) {
            return null;
        }

        const buildingDataCount = buildingGroup.buildingDatas.length;
        for (let i = 0; i < buildingDataCount; i++) {
            const building = buildingGroup.buildingDatas[i];
            if (building.visibleTreeView === false && this.props.searchText.length > 0) {
                continue;
            }

            ui.push(
                <BuildingNode
                    key={'node_building_' + building.id}
                    building={building}
                    sensorList={this.props.sensorList}
                    selectedRows={this.props.selectedRows}
                    onChangeSensor={this.props.onChangeSensor}
                    curSensorType={this.props.curSensorType}
                    selectedNodes={this.props.selectedNodes}
                    addSelectedNodes={this.props.addSelectedNodes}
                    removeSelectedNodes={this.props.removeSelectedNodes}
                    isEditMode={this.props.isEditMode}
                    dashboard={this.props.dashboard}
                    selectedMenu={this.props.selectedMenu}
                    parentFrm={this.props.parentFrm}
                    searchText={this.props.searchText}
                />);
        }

        return ui;
    }

    render() {
        const buildingGroupText = this.props.buildingGroup?.displayText;
        const ui = this.getUI();
        return (
            <li style={{ position: 'relative' }}><span className={styles.poiTreeGroupIcon}></span>{buildingGroupText}
                {
                this.props.dashboard ?
                    <div className={styles.treeIconImageAreaPoi}>
                        <span className={styles.goLink} onClick={this.moveToX} style={{ cursor: 'pointer' }}><a>이동</a></span>
                    </div>
                    : null
                }
                <ul>                    
                    {ui}
                </ul>
            </li>
        );
    }
}