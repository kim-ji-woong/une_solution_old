import React, { Component } from 'react';
import $ from 'jquery';
import space from '../css/space.module.css';
import styles from '../css/spatial.module.css';

import { BuildingGroupNode } from './buildingGroupTreeView/buildingGroupNode';
import { ZoneNode } from './buildingGroupTreeView/zoneNode';
import { SpaceDataManager } from '../services/spaceDataManager';
import { CommonScrollbar } from '../../Root/commonScrollbar';
import rootStyles from '../../Root/css/root.module.css';

export class POIEdit extends Component {
    static Mode_All_POI = 0;
    static Mode_EquipZone_POI = 1;

    static Menu_Info = 0;
    static Menu_Add = 1;
    static Menu_Move = 2;
    static Menu_Delete = 3;

    constructor(props) {
        super(props);
        this.state = {
            poiMode: POIEdit.Mode_All_POI,
            controlMenu: POIEdit.Menu_Info,
            isEditMode: true,
            dashboard: false
        }

        /*this.refLayer = React.createRef();
        this.refScrollArea = React.createRef();
        this.refScrollbar = React.createRef();
        this.refTree = React.createRef();*/
        this.refScrollArea = React.createRef();
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
    }


    setScrollbar() {
        const treeArea = this.refScrollArea.current.getBoundingClientRect();

        let scrollVisible = false;

        if (this.refTree.current) {
            const rectTree = this.refTree.current.getBoundingClientRect();

            if (rectTree.height > treeArea.height) {
                scrollVisible = true;
            }
        }

        CommonScrollbar.setContentStyle(this.refScrollbar.current, treeArea.width, treeArea.height, scrollVisible);

        const treeArea2 = this.refScrollArea.current.getBoundingClientRect();

        if (this.props.selectedInfo && this.props.selectedInfo.buildingGroup) {

        }
    }


    getBuildingGroupTreeViewUI() {
        let ui = [];
        const buildingGroupList = this.props.buildingGroupList;
        const buildingGroupCount = buildingGroupList.length;
        for (let i = 0; i < buildingGroupCount; i++) {
            const buildingGroup = buildingGroupList[i];
            if (buildingGroup.visible) {
                ui.push(
                    <BuildingGroupNode
                        selectedMenu={this.props.selectedMenu}
                        key={'node_buildingGroup_' + buildingGroup.id}
                        buildingGroup={buildingGroup} sensorList={this.props.sensorList}
                        selectedNodes={this.props.selectedNodes}
                        addSelectedNodes={this.props.addSelectedNodes}
                        removeSelectedNodes={this.props.removeSelectedNodes}
                        isEditMode={this.state.isEditMode}
                        dashboard={this.state.dashboard}
                    />
                );
            }
        }

        return ui;
    }

    getCurrentZone() {
        const currentView = this.props.currentView;

        if (currentView) {
            if (currentView.zoneID !== null && currentView.zoneID !== undefined) {
                const zoneData = SpaceDataManager.findZone(currentView.zoneID, this.props._3dOptions);

                if (zoneData) {
                    return [zoneData, currentView.zoneID];
                }
            }
        }

        return [null, null];
    }

    getCurrentZoneName(zoneData) {
        if (zoneData && zoneData.length >= 3) {
            return zoneData[2];
        }

        return "외부영역";
    }

    getZoneData(zoneID, buildingID) {
        const buildingData = this.props._3dOptions.buildingIDs[buildingID];

        if (buildingData && buildingData.length >= 2) {
            const buildingGroupName = buildingData[1];
            const buildingGroupList = this.props.buildingGroupList;

            if (buildingGroupList) {
                for (const buildingGroup of buildingGroupList) {
                    if (buildingGroup.visible && buildingGroup.groupName === buildingGroupName) {
                        for (const building of buildingGroup.buildingDatas) {
                            if (building.id === buildingID) {
                                for (const zoneData of building.zoneDatas) {
                                    if (zoneData.id === zoneID) {
                                        return zoneData;
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

        return null;
    }

    getZoneUI(zone, zoneID) {
        if (!zone) {
            return <></>;
        }

        const zoneData = zone.length >= 2 ? this.getZoneData(zoneID, zone[1]) : null;

        if (zoneData === null) {
            return <></>;
        }

        return (
            <ZoneNode
                key={'node_zone_' + zoneID}
                zone={zoneData}
                sensorList={zone.sensors}
                curSensorType={this.props.curSensorType}
                selectedNodes={this.props.selectedNodes}
                addSelectedNodes={this.props.addSelectedNodes}
                removeSelectedNodes={this.props.removeSelectedNodes}
                isEditMode={this.state.isEditMode}
                dashboard={this.state.dashboard}
                selectedMenu={this.props.selectedMenu}
                sensorDisplayMode={true}
                showEquipZone={this.state.poiMode === POIEdit.Mode_EquipZone_POI}
            />);
    }

    onClickMode(mode) {
        if (this.state.poiMode !== mode) {
            this.setState({ poiMode: mode });
        }
    }

    onClickMenu(menu) {
        if (this.state.controlMenu !== menu) {
            this.props.poiManager.selectPOI(null, true, null);
            this.props.poiManager.hideTempPOI();
            this.props.poiManager.CurrentMode = menu;
            this.setState({ controlMenu: menu });
        }
    }

    refresh() {
        this.setState({ poiMode: this.state.poiMode });
    }

    render() {
        const [currentZone, zoneID] = this.getCurrentZone();
        const buildingGroupTreeViewUI = this.getBuildingGroupTreeViewUI();
        const poiMode = this.state.poiMode;
        const ctrlMenu = this.state.controlMenu;
        const scrollAreaStyle = this.props.modeling ? styles.dsiScr + " " + styles.short + " " + rootStyles.scrollbar : styles.dsiScr + " " + rootStyles.scrollbar;
        const scrollAreaStyles = this.props.modeling ? styles.dsiScrr + " " + styles.short + " " + rootStyles.scrollbar : styles.dsiScrr + " " + rootStyles.scrollbar;
        const scrollAreaStyless = this.props.modeling ? styles.dsiScrr + " " + styles.short + " " + rootStyles.scrollbar : styles.dsiScrrr + " " + rootStyles.scrollbar; 
        this.props.poiManager.PoiEdit = this;

        return (
            <div className={space.poiListArea}>
                <div className={space.poiListBox}>
                    <div className={space.poiListTitleBox}>
                        <span className={space.poiListTitleText}>POI 편집</span>
                        <ul className={space.poiRadioBox3}>
                            <li><input type="radio" id="iconInfo" className={space.poiRadioCtrl} onChange={() => this.onClickMenu(POIEdit.Menu_Info)} checked={ctrlMenu === POIEdit.Menu_Info} /> <label htmlFor="iconInfo" className={space.poiRadioText}>정보확인</label></li>
                            <li><input type="radio" id="iconAdd" className={space.poiRadioCtrl} onChange={() => this.onClickMenu(POIEdit.Menu_Add)} checked={ctrlMenu === POIEdit.Menu_Add} /> <label htmlFor="iconAdd" className={space.poiRadioText}>추가</label></li>
                            <li><input type="radio" id="iconMove" className={space.poiRadioCtrl} onChange={() => this.onClickMenu(POIEdit.Menu_Move)} checked={ctrlMenu === POIEdit.Menu_Move} /> <label htmlFor="iconMove" className={space.poiRadioText}>이동</label></li>
                            <li><input type="radio" id="iconDelete" className={space.poiRadioCtrl} onChange={() => this.onClickMenu(POIEdit.Menu_Delete)} checked={ctrlMenu === POIEdit.Menu_Delete} /> <label htmlFor="iconDelete" className={space.poiRadioText}>삭제</label></li>
                        </ul>
                        <div className={space.poiSelectBox}>
                            <span className={space.poiListSelect}></span>
                        </div>
                        <div className={space.poiListContent} className={scrollAreaStyles}>
                        {/* <div className={space.poiListContent}> */}
                            <ul /* className={space.poiListContentUl} */>
                                {buildingGroupTreeViewUI}
                            </ul>
                        </div>
                    </div>
                </div>
                <div className={space.poiSpaceListBox}>
                    <div className={space.poiListTitleBox}>
                        <span className={space.poiListTitleText}>{this.getCurrentZoneName(currentZone)}</span>
                        <ul className={space.poiRadioBox}>
                            <li><input type="radio" id="checkIcon" className={space.poiRadioCtrl} onChange={() => this.onClickMode(POIEdit.Mode_All_POI)} checked={poiMode === POIEdit.Mode_All_POI} /> <label htmlFor="checkIcon" className={space.poiRadioText}>전체보기</label></li>
                            <li><input type="radio" id="moveIcon" className={space.poiRadioCtrl} onChange={() => this.onClickMode(POIEdit.Mode_EquipZone_POI)} checked={poiMode === POIEdit.Mode_EquipZone_POI} /> <label htmlFor="moveIcon" className={space.poiRadioText}>구역별 보기</label></li>
                        </ul>
                        <div className={space.poiListContent2} className={scrollAreaStyless}>
                        {/* <div className={space.poiListContent}> */}
                            {/*  <span className={space.poiListScrollbar}> */}
                                <ul /* className={space.poiListContentUl} */ >
                                    {this.getZoneUI(currentZone, zoneID)}
                                </ul>
                            {/* </span> */}
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}