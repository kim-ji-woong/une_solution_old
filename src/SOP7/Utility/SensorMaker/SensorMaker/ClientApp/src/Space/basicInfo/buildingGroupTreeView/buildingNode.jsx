import React, { Component } from 'react';
import space from './../../css/space.module.css';
import styles from '../../css/spatial.module.css';

import $ from 'jquery';
import { ZoneNode } from './zoneNode';

export class BuildingNode extends Component {
    constructor(props) {
        super(props);
        this.state = {
            searchText: ''
        }
    }

    getUI() {
        let ui = [];
        const building = this.props.building;
        if (!building) {
            return null;
        }

        const zoneDataCount = building.zoneDatas.length;
        for (let i = 0; i < zoneDataCount; i++) {
            const zone = building.zoneDatas[i];
            if (zone.visibleTreeView === false && this.props.searchText.length > 0) {
                continue;
            }

            ui.push(
                <ZoneNode
                    key={'node_zone_' + zone.id}
                    zone={zone}
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
        const buildingText = this.props.building?.displayText;
        const ui = this.getUI();

        return (
            <li><span className={styles.poiTreebuildIcon}></span>{buildingText}
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