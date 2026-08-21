import { ui } from 'jquery';
import React, { Component } from 'react';
import styles from '../../css/sdms.module.css';
import StatusInfoZone from './statusInfoZone';
import SDMSMainMenu from '../../data/sdmsMainMenu';

class StatusInfoBuilding extends Component {
    constructor(props) {
        super(props);

        this.refBuildingName = React.createRef();
        this.refZoneList = React.createRef();
        this.moveToX = this.moveToX.bind(this);
        this.prevSelectedSensor = [null, null, null];

        // 사용자가 마우스로 조작하였는가?
        // true : 접혔다.
        // false : 펼쳐졌다.
        this.manualExpand = null;
        this.showChildResult = false;
    }

    componentDidMount() {
        this.checkChildVisible();
    }

    componentDidUpdate(prevProps, prevState) {
        this.checkChildVisible();
    }

    checkChildVisible() {
        if (this.refBuildingName.current) {
            if (this.showChildResult) {
                if (this.refBuildingName.current.dataset.show_child !== 'true') {
                    this.refBuildingName.current.dataset.show_child = 'true';
                }

                if (this.refZoneList.current.classList.contains(styles.on) === false) {
                    this.refZoneList.current.classList.add(styles.on);
                }
            }
            else {
                if (this.refBuildingName.current.dataset.show_child !== 'false') {
                    this.refBuildingName.current.dataset.show_child = 'false';
                }

                if (this.refZoneList.current.classList.contains(styles.on)) {
                    this.refZoneList.current.classList.remove(styles.on);
                }
            }
        }
    }

    moveToX() {
        this.props.moveToX(SDMSMainMenu.Menu_MoveTo_Building, [this.props.building.buildingName]);
    }

    getZoneUI() {
        let ui = [];

        if (this.props.building.zoneDatas) {
            const zoneDatas = this.props.building.zoneDatas;
            if (zoneDatas === undefined || zoneDatas === null || zoneDatas.length === 0)
                return ui;

            this.setZoneUI(zoneDatas, ui);
            /*zoneDatas.sort(function (a, b) { return (a.floorIndex + a.addFloor) - (b.floorIndex + b.addFloor) });
            for (var i = 0; i < zoneDatas.length; i++) {
                const zone = zoneDatas[i];

                if (zone.visible === false && this.props.searchText.length > 0)
                    continue;

                let fireSensors = null, psmSensors = null, etcSensors = null, cctvs = null, facilityInfos = null;

                if (this.props.sensorList) {
                    fireSensors = this.props.sensorList['fireSensors'].filter(x => x.zoneID === zone.id && (x.visible === true || this.props.searchText === ''));
                    psmSensors = this.props.sensorList['psmSensors'];//.filter(x => x.zoneID === zone.id);
                    etcSensors = this.props.sensorList['etcSensors'].filter(x => x.zoneID === zone.id && (x.visible === true || this.props.searchText === ''));
                    cctvs = this.props.sensorList['cctvs'].filter(x => x.zoneID === zone.id && (x.visible === true || this.props.searchText === ''));

                    if (this.props.facilityInfos) {
                        facilityInfos = this.props.facilityInfos.filter(x => x.zoneID === zone.id && (x.visible === true || this.props.searchText === ''));
                    }
                }
                //ui.push(<StatusInfoZone key={'zone_' + zone.id} zone={zone} sensorList={this.props.sensorList} moveToX={this.props.moveToX} sensorAlarms={this.props.sensorAlarms} />);
                ui.push(<StatusInfoZone key={'zone_' + zone.id} zone={zone} sensorList={this.props.sensorList} fireSensors={fireSensors} psmSensors={psmSensors} etcSensors={etcSensors} cctvs={cctvs} facilityInfos={facilityInfos} moveToX={this.props.moveToX} onSelectSensor={this.props.onSelectSensor} selectedSensor={this.props.selectedSensor} sensorAlarms={this.props.sensorAlarms} showChild={this.props.showChild} searchText={this.props.searchText} isEditMode={this.props.isEditMode} />);
            }*/
        }
        else {
            const outdoorZones = this.props.building;
            const zoneDatas = [];

            for (const zoneID in outdoorZones) {
                const zoneData = outdoorZones[zoneID];
                zoneDatas.push(zoneData);
            }

            if (zoneDatas === undefined || zoneDatas === null || zoneDatas.length === 0)
                return ui;

            this.setZoneUI(zoneDatas, ui);
        }

        return ui;
    }

    setZoneUI(zoneDatas, ui) {
        const hasIndoorModel = this.hasFloorModel(this.props.building.id);
        for (var i = 0; i < zoneDatas.length; i++) {
            const zone = zoneDatas[i];

            if (zone.floorIndex === null) {
                // floorIndex가 null이면 건물 전체를 의미한다.
                continue;
            }

            if (zone.visible === false && this.props.searchText.length > 0)
                continue;

            let fireSensors = null/*, psmSensors = null, etcSensors = null*/, cctvs = null, facilityInfos = null;
            let coSensors = null, o2Sensors = null, h2Sensors = null, ch4Sensors = null, detectSensors = null;

            if (this.props.sensorList) {
                fireSensors = this.props.sensorList['fireSensors'].filter(x => x.zoneID === zone.id && (x.visible === true || this.props.searchText === ''));
                coSensors = this.props.sensorList[SDMSMainMenu.CO_Sensor + "Sensors"].filter(x => x.zoneID === zone.id && (x.visible === true || this.props.searchText === ''));
                o2Sensors = this.props.sensorList[SDMSMainMenu.O2_Sensor + "Sensors"].filter(x => x.zoneID === zone.id && (x.visible === true || this.props.searchText === ''));
                h2Sensors = this.props.sensorList[SDMSMainMenu.H2_Sensor + "Sensors"].filter(x => x.zoneID === zone.id && (x.visible === true || this.props.searchText === ''));
                ch4Sensors = this.props.sensorList[SDMSMainMenu.CH4_Sensor + "Sensors"].filter(x => x.zoneID === zone.id && (x.visible === true || this.props.searchText === ''));
                detectSensors = this.props.sensorList[SDMSMainMenu.Detect_Sensor + "Sensors"].filter(x => x.zoneID === zone.id && (x.visible === true || this.props.searchText === ''));
                //psmSensors = this.props.sensorList['psmSensors'];//.filter(x => x.zoneID === zone.id);
                //etcSensors = this.props.sensorList['etcSensors'].filter(x => x.zoneID === zone.id && (x.visible === true || this.props.searchText === ''));
                cctvs = this.props.sensorList['cctvs'].filter(x => x.zoneID === zone.id && (x.visible === true || this.props.searchText === ''));

                if (this.props.facilityInfos) {
                    facilityInfos = this.props.facilityInfos.filter(x => x.zoneID === zone.id && (x.visible === true || this.props.searchText === ''));
                }
            }

            //ui.push(<StatusInfoZone key={'zone_' + zone.id} zone={zone} sensorList={this.props.sensorList} moveToX={this.props.moveToX} sensorAlarms={this.props.sensorAlarms} />);
            ui.push(
                <StatusInfoZone
                    key={'zone_' + zone.id} zone={zone}
                    sensorList={this.props.sensorList}
                    fireSensors={fireSensors}
                    coSensors={coSensors}
                    o2Sensors={o2Sensors}
                    h2Sensors={h2Sensors}
                    ch4Sensors={ch4Sensors}
                    detectSensors={detectSensors}
                    //psmSensors={psmSensors}
                    //etcSensors={etcSensors}
                    cctvs={cctvs}
                    facilityInfos={facilityInfos}
                    moveToX={this.props.moveToX}
                    onSelectSensor={this.props.onSelectSensor}
                    selectedSensor={this.props.selectedSensor}
                    selectedInfo={this.props.selectedInfo}
                    sensorAlarms={this.props.sensorAlarms}
                    showChild={this.props.showChild}
                    searchText={this.props.searchText}
                    isEditMode={this.props.isEditMode}
                    hasIndoorModel={hasIndoorModel}
                    onChangeBuildingGroup={this.props.onChangeBuildingGroup}
                />
            );
        }
    }

    hasFloorModel = (buildingID, floorIndex) => {
        if (buildingID === null || buildingID === undefined) {
            return false;
        }

        const building = this.props.buildingIDs[buildingID.toString()];

        if (building) {
            const buildingGroupName = building[1];
            const buildingName = building[2];

            const buildingGroup = this.props.indoorModels[buildingGroupName];

            if (buildingGroup) {
                let buildingData = buildingGroup[buildingName];

                if (!buildingData) {
                    buildingData = this.getBuildingDataFromDisplayText(buildingName, buildingGroup);
                }

                if (buildingData && buildingData.floors) {
                    return true;
                }
            }
        }

        return false;
    }

    getBuildingDataFromDisplayText(displayText, buildingGroup) {
        for (const buildingName in buildingGroup) {
            const buildingData = buildingGroup[buildingName];

            if (buildingData.modelDisplayText && buildingData.modelDisplayText === displayText) {
                return buildingData;
            }
        }

        return null;
    }

    showChild(e) {
        this.manualExpand = this.props.showChild(e);
        if (this.manualExpand && this.props.onChangeBuildingGroup) {
            this.props.onChangeBuildingGroup(this.props.building, 'building');
        }
    }

    isSelected() {
        if (this.props.selectedInfo) {
            if (this.props.selectedInfo.building === this.props.building) {
                return true;
            }
        }

        const [sensorType, zoneID, sensorID] = this.props.selectedSensor;

        if (this.prevSelectedSensor[0] !== sensorType ||
            this.prevSelectedSensor[1] !== zoneID ||
            this.prevSelectedSensor[2] !== sensorID) {
            this.manualExpand = null;
        }

        this.prevSelectedSensor = [sensorType, zoneID, sensorID];

        if (this.manualExpand !== null) {
            return this.manualExpand;
        }

        if (sensorType !== null && zoneID !== null && sensorID !== null) {
            if (this.props.building.zoneDatas) {
                const buildingData = this.props.building;

                /*if (!buildingData || !buildingData.zoneDatas) {
                    return false;
                }*/

                const zoneCount = buildingData.zoneDatas.length;

                for (let i = 0; i < zoneCount; i++) {
                    const zoneData = buildingData.zoneDatas[i];

                    if (zoneData.id === zoneID) {
                        return true;
                    }
                }
            }
            else {
                const outdoorZones = this.props.building;
                const zoneIDString = zoneID.toString();
                
                for (const outdoorZoneID in outdoorZones) {
                    if (outdoorZoneID === zoneIDString) {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    render() {
        let zoneUI = this.getZoneUI();
        let listClassName = styles.viewList2Depth;
        let showChild = 'false';
        this.showChildResult = false;

        if (this.isSelected()) {
            listClassName += " " + styles.on;
            showChild = 'true';
            this.showChildResult = true;
        }

        const buildingName = this.props.building.displayText ? this.props.building.displayText : "외부 영역";

        return (
            <li>
                <div ref={this.refBuildingName}
                    className={styles.viewList1Depth}
                    data-show_child={showChild}
                    data-target_class='viewList1Depth'
                    onClick={(e) => { this.showChild(e) }}>
                    {buildingName}
                    {
                        //<span className={styles.goLink} onClick={this.moveToX}><a>이동</a></span>
                    }
                </div>
                <ul ref={this.refZoneList} className={listClassName}>
                    {zoneUI}
                </ul>
            </li>
        );
    }
}

export default StatusInfoBuilding;