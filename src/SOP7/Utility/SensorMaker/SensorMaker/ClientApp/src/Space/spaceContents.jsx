import React, { Component } from 'react';
import { BasicBody } from './basicInfo/basicBody';
import styles from './css/space.module.css';
import { SpaceMenus } from './spaceMenus';
import { SiteInfo } from './spatialInfo/siteInfo';
import { ModelSiteInfo } from './spatialInfo/modelsiteInfo';
import { UploadDetailInfo } from './spatialInfo/uploadDetailInfo';
import { DetailInfo } from './spatialInfo/detailInfo';
import { ModelView } from './basicInfo/3dModelView';
import { SensorListEdit } from './basicInfo/sensorListEdit';
import { EquipZoneCCTV } from './basicInfo/equipZoneCCTV';
import { POIEdit } from './basicInfo/poiEdit';
import { Contents3D } from './3D/contents3D';
import { BuildingGroupNode } from './basicInfo/buildingGroupTreeView/buildingGroupNode';


export class SpaceContents extends Component {
    getClassName() {
        return styles.contentsArea;
    }

    getContents() {
        if (this.props.selectedMenu === SpaceMenus.EditBasicInfo) {
            return <BasicBody xmlData={this.props.xmlData} />;
        }
        else if (this.props.selectedMenu === SpaceMenus.EditSpaceInfo) {
            return (
                <>
                    <div className={styles.editSpaceBox}>
                        <SiteInfo loginData={this.props.loginData} _3dOptions={this.props._3dOptions} buildingGroupList={this.props.buildingGroupList} tempModelFiles={this.props.tempModelFiles} selectedInfo={this.props.selectedInfo} modeling={false} onAddItem={this.props.onAddItem} onRemoveItem={this.props.onRemoveItem} onRenameItem={this.props.onRenameItem} onSelectItem={this.props.onSelectItem} onSetModelFile={this.props.onSetModelFile} onSaveXML={this.props.onSaveXML} onChangeTempModelFiles={this.props.onChangeTempModelFiles} />
                        <DetailInfo />
                    </div>
                </>
            );
        }
        else if (this.props.selectedMenu === SpaceMenus.Upload3DModels) {
            return (
                <>
                    <div className={styles.Upload3DBox}>
                        <SiteInfo loginData={this.props.loginData} _3dOptions={this.props._3dOptions} buildingGroupList={this.props.buildingGroupList} tempModelFiles={this.props.tempModelFiles} selectedInfo={this.props.selectedInfo} modeling={true} onAddItem={this.props.onAddItem} onRemoveItem={this.props.onRemoveItem} onRenameItem={this.props.onRenameItem} onSelectItem={this.props.onSelectItem} onSetModelFile={this.props.onSetModelFile} onSaveXML={this.props.onSaveXML} onChangeTempModelFiles={this.props.onChangeTempModelFiles} />
                        <UploadDetailInfo />
                    </div>
                </>
            );
        }
        /*else if (this.props.selectedMenu === SpaceMenus.Upload3DModels) {
            return <WaitResponse loginUser={this.props.loginUser} options={this.props.options} managerRequest={this.props.managerRequest} removeRequest={this.props.removeRequest} />;
        } */
        else if (this.props.selectedMenu === SpaceMenus.Load3DModels) {
            return (
                <>
                    <Contents3D _3dOptions={this.props._3dOptions}
                        editMode={false}
                        setCurrentView={this.props.setCurrentView}
                        currentView={this.props.currentView}
                        initOutdoorViewport={this.props.initOutdoorViewport}
                        getSpatialInfo={this.props.getSpatialInfo}
                        onChangeBuildingGroup={this.props.onChangeBuildingGroup}
                        command={this.props.command}
                        poiManager={this.props.poiManager}
                        selectedNodes={this.props.selectedSensorNodes}
                        onSaveXML={this.props.onSaveXML} />
                    {/* <SiteInfo loginData={this.props.loginData}
                        _3dOptions={this.props._3dOptions}
                        buildingGroupList={this.props.buildingGroupList}
                        tempModelFiles={this.props.tempModelFiles}
                        selectedInfo={this.props.selectedInfo}
                        modeling={false}
                        dashboard={true}
                        onAddItem={this.props.onAddItem}
                        onRemoveItem={this.props.onRemoveItem}
                        onRenameItem={this.props.onRenameItem}
                        onSelectItem={this.props.onSelectItem}
                        onSetModelFile={this.props.onSetModelFile}
                        onSaveXML={this.props.onSaveXML}
                        onChangeTempModelFiles={this.props.onChangeTempModelFiles}
                        moveToX={this.props.moveToX} />; */}

                    <ModelSiteInfo loginData={this.props.loginData}
                        _3dOptions={this.props._3dOptions}
                        buildingGroupList={this.props.buildingGroupList}
                        tempModelFiles={this.props.tempModelFiles}
                        selectedInfo={this.props.selectedInfo}
                        modeling={false}
                        dashboard={true}
                        currentView={this.props.currentView}
                        poiManager={this.props.poiManager}
                        onAddItem={this.props.onAddItem}
                        onRemoveItem={this.props.onRemoveItem}
                        onRenameItem={this.props.onRenameItem}
                        onSelectItem={this.props.onSelectItem}
                        onSetModelFile={this.props.onSetModelFile}
                        onSaveXML={this.props.onSaveXML}
                        onChangeTempModelFiles={this.props.onChangeTempModelFiles}
                        moveToX={this.props.moveToX} />;
                </>
            );
        }
        else if (this.props.selectedMenu === SpaceMenus.EditSensorList) {
            return <SensorListEdit
                selectedMenu={this.props.selectedMenu}
                buildingGroupList={this.props.buildingGroupList}
                sensorList={this.props.sensorList}
                onChangeSensorList={this.props.onChangeSensorList}
                selectedNodes={this.props.selectedSensorNodes}
                addSelectedNodes={this.props.addSelectedSensorNodes}
                removeSelectedNodes={this.props.removeSelectedSensorNodes}
                sensorTypes={this.props.sensorTypes}
            />
        }
        else if (this.props.selectedMenu === SpaceMenus.EditEquipZoneCCTVs) {
            return <EquipZoneCCTV
                selectedMenu={this.props.selectedMenu}
                buildingGroupList={this.props.buildingGroupList}
                sensorList={this.props.sensorList}
                onChangeSensorList={this.props.onChangeSensorList}
                selectedNodes={this.props.selectedSensorNodes}
                addSelectedNodes={this.props.addSelectedSensorNodes}
                removeSelectedNodes={this.props.removeSelectedSensorNodes}
            />
        }
        else if (this.props.selectedMenu === SpaceMenus.EditPois) {
            return (
                <>
                    <Contents3D _3dOptions={this.props._3dOptions}
                        editMode={true}
                        setCurrentView={this.props.setCurrentView}
                        currentView={this.props.currentView}
                        initOutdoorViewport={this.props.initOutdoorViewport}
                        getSpatialInfo={this.props.getSpatialInfo}
                        onChangeBuildingGroup={this.props.onChangeBuildingGroup}
                        command={this.props.command}
                        modeling={false}
                        poiManager={this.props.poiManager}
                        selectedNodes={this.props.selectedSensorNodes}
                        onSaveXML={this.props.onSaveXML} />

                    <POIEdit
                        _3dOptions={this.props._3dOptions}
                        currentView={this.props.currentView}
                        selectedMenu={this.props.selectedMenu}
                        buildingGroupList={this.props.buildingGroupList}
                        sensorList={this.props.sensorList}
                        selectedNodes={this.props.selectedSensorNodes}
                        poiManager={this.props.poiManager}
                        addSelectedNodes={this.props.addSelectedSensorNodes}
                        removeSelectedNodes={this.props.removeSelectedSensorNodes}
                    />
                    {
                    //<SiteInfo loginData={this.props.loginData}
                    //    _3dOptions={this.props._3dOptions}
                    //    buildingGroupList={this.props.buildingGroupList}
                    //    tempModelFiles={this.props.tempModelFiles}
                    //    selectedInfo={this.props.selectedInfo}
                    //    modeling={false}
                    //    onAddItem={this.props.onAddItem}
                    //    onRemoveItem={this.props.onRemoveItem}
                    //    onRenameItem={this.props.onRenameItem}
                    //    onSelectItem={this.props.onSelectItem}
                    //    onSetModelFile={this.props.onSetModelFile}
                    //    onSaveXML={this.props.onSaveXML}
                    //    onChangeTempModelFiles={this.props.onChangeTempModelFiles}
                    //        moveToX={this.props.moveToX} />;
                    }
                </>
            );
            //return <POIEdit />
        }

        return <></>;
    }

    render() {
        const contents = this.getContents();

        return (
            <div className={this.getClassName()}>
                {contents}
            </div>
        );
    }
}