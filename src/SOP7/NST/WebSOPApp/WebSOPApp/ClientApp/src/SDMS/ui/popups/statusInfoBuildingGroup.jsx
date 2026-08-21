import React, { Component } from 'react';
import styles from '../../css/sdms.module.css';
import SDMSMainMenu from '../../data/sdmsMainMenu';
import StatusInfoBuilding from './statusInfoBuilding';

class StatusInfoBuildingGroup extends Component {
    constructor(props) {
        super(props);

        this.prevSelectedSensor = [null, null, null];
        this.moveToX = this.moveToX.bind(this);
        this.refBuildingGroupName = React.createRef();
        this.refBuildingList = React.createRef();
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
        if (this.refBuildingGroupName.current) {
            if (this.showChildResult) {
                if (this.refBuildingGroupName.current.dataset.show_child !== 'true') {
                    this.refBuildingGroupName.current.dataset.show_child = 'true';
                }

                if (this.refBuildingList.current.classList.contains(styles.on) === false) {
                    this.refBuildingList.current.classList.add(styles.on);
                }
            }
            else {
                if (this.refBuildingGroupName.current.dataset.show_child !== 'false') {
                    this.refBuildingGroupName.current.dataset.show_child = 'false';
                }

                if (this.refBuildingList.current.classList.contains(styles.on)) {
                    this.refBuildingList.current.classList.remove(styles.on);
                }
            }
        }
    }

    moveToX() {
        this.props.moveToX(SDMSMainMenu.Menu_MoveTo_BuildingGroup, [this.props.buildingGroup.groupName]);
    }

    getBuildingUI() {
        let ui = [];

        if (!this.props.selectedInfo || this.props.selectedInfo.buildingGroup === null || this.props.selectedInfo.buildingGroup.id !== this.props.buildingGroup.id) {
            return ui;
        }

        if (this.props.buildingGroup.buildingDatas) {
            const buildingDatas = this.props.buildingGroup.buildingDatas;
            if (buildingDatas === undefined || buildingDatas === null || buildingDatas.length === 0)
                return ui;

            for (var i = 0; i < buildingDatas.length; i++) {
                const building = buildingDatas[i];
                if (building.visible === false && this.props.searchText.length > 0)
                    continue;

                ui.push(
                    <StatusInfoBuilding
                        key={'building_' + building.id}
                        building={building}
                        buildingIDs={this.props.buildingIDs}
                        indoorModels={this.props.indoorModels}
                        sensorList={this.props.sensorList}
                        moveToX={this.props.moveToX}
                        onSelectSensor={this.props.onSelectSensor}                        
                        selectedSensor={this.props.selectedSensor}
                        selectedInfo={this.props.selectedInfo}
                        sensorAlarms={this.props.sensorAlarms}
                        showChild={this.showChild}
                        searchText={this.props.searchText}
                        facilityInfos={this.props.facilityInfos}
                        isEditMode={this.props.isEditMode}
                        onChangeBuildingGroup={this.props.onChangeBuildingGroup} />
                );
            }
        }
        else {
            const outdoorZones = this.props.buildingGroup;
            ui.push(
                <StatusInfoBuilding
                    key={'building_outdoor'}
                    building={outdoorZones}
                    buildingIDs={this.props.buildingIDs}
                    indoorModels={this.props.indoorModels}
                    sensorList={this.props.sensorList}
                    moveToX={this.props.moveToX}
                    onSelectSensor={this.props.onSelectSensor}
                    selectedSensor={this.props.selectedSensor}
                    selectedInfo={this.props.selectedInfo}
                    sensorAlarms={this.props.sensorAlarms}
                    showChild={this.showChild}
                    searchText={this.props.searchText}
                    facilityInfos={this.props.facilityInfos}
                    isEditMode={this.props.isEditMode}
                    onChangeBuildingGroup={this.props.onChangeBuildingGroup} />

            );
        }
        
        return ui;
    }

    //각 트리 단계별 show / hide
    showChild = (e) => {
        // show / hide 여부, dataset은 항상 string으로 적용된다.
        let flag = e.target.dataset.show_child;
        //this.showChildElement(e.target, flag === 'false');
        //닫을 메뉴들
        let hideDepths = [];
        let targetClass = e.target.dataset.target_class;
        let heads = [];

        if (targetClass === 'viewListHead') {
            hideDepths = [styles.viewListConts, styles.viewList1Depth, styles.viewList2Depth, styles.viewList3Depth, styles.viewList4Depth, styles.viewList5Depth];
            heads = [styles.viewListHead, styles.viewList1Depth, styles.viewList2DepthSpen, styles.viewList3DepthHead, styles.viewList4DepthHead, styles.viewList5DepthHead];
        } else if (targetClass === 'viewList1Depth') {
            hideDepths = [styles.viewList1Depth, styles.viewList2Depth, styles.viewList3Depth, styles.viewList4Depth, styles.viewList5Depth];
            heads = [styles.viewList1Depth, styles.viewList2DepthSpen, styles.viewList3DepthHead, styles.viewList4DepthHead, styles.viewList5DepthHead];
        } else if (targetClass === 'viewList2Depth') {
            hideDepths = [styles.viewList3Depth, styles.viewList4Depth, styles.viewList5Depth];
            heads = [styles.viewList2DepthSpen, styles.viewList3DepthHead, styles.viewList4DepthHead, styles.viewList5DepthHead];
        } else if (targetClass === 'viewList3Depth') {
            hideDepths = [styles.viewList4Depth, styles.viewList5Depth];
            heads = [styles.viewList3DepthHead, styles.viewList4DepthHead, styles.viewList5DepthHead];
        } else if (targetClass === 'viewList4Depth') {
            hideDepths = [styles.viewList5Depth];
            heads = [styles.viewList4DepthHead];
        }

        // 다른 트리 비활성화
        for (let depth of hideDepths) {
            let nodes = document.getElementsByClassName(depth);
            for (let node of nodes) {
                node.classList.remove(styles.on);
            }
        }

        //플래그 초기화
        for (let head of heads) {
            let tags = document.getElementsByClassName(head);
            for (let tag of tags) {
                tag.dataset.show_child = 'false';
            }
        }

        let expand = false;

         // child tree show
        if (flag === 'false') {
            e.target.dataset.show_child = 'true';

            if (targetClass === 'viewListHead') {
                e.target.parentElement.nextElementSibling.classList.add(styles.on);
            } else if (targetClass === 'viewList_4Depth') {
                let viewList5Depth = e.target.getElementsByClassName(styles.viewList5Depth);
                for (let depth of viewList5Depth) {
                    depth.classList.add(styles.on);
                }
            } else if (targetClass === 'viewList2Depth') {
                if (e.target.parentElement && e.target.parentElement.nextElementSibling) {
                    e.target.parentElement.nextElementSibling.classList.add(styles.on);
                }
            } else {//1, 3 뎁스 공통
                e.target.nextElementSibling.classList.add(styles.on);
            }

            expand = true;
        } else {
            // child tree hide
            e.target.dataset.show_child = 'false';

            if (targetClass === 'viewListHead') {
                e.target.parentElement.nextElementSibling.classList.remove(styles.on);
            } else if (targetClass === 'viewList4Depth') {
                let viewList5Depth = e.target.getElementsByClassName(styles.viewList5Depth);
                for (let depth of viewList5Depth) {
                    depth.classList.remove(styles.on);
                }
            } else if (targetClass === 'viewList2Depth') {
                if (e.target.parentElement && e.target.parentElement.nextElementSibling) {
                    e.target.parentElement.nextElementSibling.classList.remove(styles.on);
                }
            } else {//1, 3 뎁스 공통
                e.target.nextElementSibling.classList.remove(styles.on);
            }

            expand = false;
        }

        return expand;
    }

    showChildElement = (e) => {
        this.manualExpand = this.showChild(e);

        if (this.props.onChangeBuildingGroup) {
            if (!this.manualExpand) {
                this.props.onChangeBuildingGroup(this.props.buildingGroup, 'all');
            }
            else {
                this.props.onChangeBuildingGroup(this.props.buildingGroup, 'buildingGroup');
            }
        }
    }

    isSelected() {
        if (this.props.selectedInfo) {
            if (this.props.selectedInfo.buildingGroup === this.props.buildingGroup) {
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
            const zoneList = this.props.zoneList;

            if (!zoneList) {
                return false;
            }

            if (this.props.buildingGroup.buildingDatas) {
                const zoneData = zoneList[zoneID];

                if (!zoneData) {
                    return false;
                }

                const buildingID = zoneData[1];

                if (buildingID === undefined || buildingID === null) {
                    return false;
                }

                const buildingGroupData = this.props.buildingGroup;

                if (!buildingGroupData || !buildingGroupData.buildingDatas) {
                    return false;
                }

                const buildingCount = buildingGroupData.buildingDatas.length;

                for (let i = 0; i < buildingCount; i++) {
                    const buildingData = buildingGroupData.buildingDatas[i];

                    if (buildingData.id === buildingID) {
                        return true;
                    }
                }
            }
            else {
                const outdoorZones = this.props.buildingGroup;
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
        let buildingUI = this.getBuildingUI();
        let listClassName = styles.viewListConts;
        let showChild = 'false';
        this.showChildResult = false;

        if (this.isSelected()) {
            listClassName += " " + styles.on;
            showChild = 'true';
            this.showChildResult = true;
        }

        const buildingGroupName = this.props.buildingGroup.displayText ? this.props.buildingGroup.displayText : "외부 영역";

        return (
            <li>
                <div className={styles.viewListHeadWrap}>
                    <span ref={this.refBuildingGroupName} className={styles.viewListHead} data-show_child={showChild} data-target_class='viewListHead' onClick={(e) => { this.showChildElement(e) }}>{buildingGroupName}</span>
                </div>
                <div ref={this.refBuildingList} className={listClassName} data-id={this.props.buildingGroup.id }>
                    <ul>
                        {buildingUI}
                    </ul>
                </div>
            </li>
        );
    }
}

export default StatusInfoBuildingGroup;