import { ModelDataManager } from "./modelDataManager";
import { SpaceDataManager } from "./spaceDataManager";

export class CommonDataManager {

    static makeModels(indoorModels, outdoorModel, modelList, buildingGroupList) {
        let gltfModels = [];

        if (modelList) {
            if (outdoorModel && modelList.models['outdoorModel']) {
                let gltfModel = {};
                gltfModel.id = modelList.models['outdoorModel'].id;
                gltfModel.modelName = modelList.models['outdoorModel'].modelName;

                for (const key in outdoorModel) {
                    const value = outdoorModel[key];
                    if (key === 'id') {
                        gltfModel.id = value;
                    }
                    else if (key === 'modelName') {
                        gltfModel.modelName = value;
                    }
                    else if (key === 'parentID') {
                        gltfModel.parentID = value;
                    }
                    else if (key === 'camera') {
                        let modelData = CommonDataManager.getCameraData(value, false);                        
                        modelData.id = modelList.models['outdoorModel'].id;
                        modelData.modelID = modelList.models['outdoorModel'].id;
                        modelData.modelFile = outdoorModel['file'];
                        modelData.modelDisplayText = outdoorModel['modelDisplayText'];

                        if (!gltfModel.modelDatas) {
                            gltfModel.modelDatas = [];
                        }
                        gltfModel.modelDatas.push(modelData);
                    }
                    else if (key === 'cameraOrtho' && value !== null) {
                        let modelOrthoData = CommonDataManager.getCameraData(value, true);
                        modelOrthoData.modelID = outdoorModel['id'];
                        modelOrthoData.modelFile = outdoorModel['file'];

                        if (!gltfModel.modelOrthoDatas) {
                            gltfModel.modelOrthoDatas = [];
                        }
                        gltfModel.modelOrthoDatas.push(modelOrthoData);
                    }
                }

                if (gltfModel.constructor === Object && Object.keys(gltfModel).length > 0) {
                    gltfModels.push(gltfModel);
                }
            }

            if (indoorModels && modelList.models['indoorModels']) {
                let gltfModel = {};
                gltfModel.id = modelList.models['indoorModels'].id;
                gltfModel.modelName = modelList.models['indoorModels'].modelName;
                gltfModel.parentID = null;
                gltfModel.childModels = [];

                for (const key in indoorModels) {
                    const value = indoorModels[key];
                    if (SpaceDataManager.isBuildingGroupModelData(value)) {
                        const childModel = CommonDataManager.makeModel(value, modelList, buildingGroupList);
                        childModel.parentID = gltfModel.id;

                        const buildingGroupID = value.buildingGroupID;
                        const model = ModelDataManager.getBuildingGroupModel(buildingGroupID, modelList, buildingGroupList);
                        if (model) {
                            if (model.id) {
                                childModel.id = model.id;
                            }
                            if (model.modelName) {
                                childModel.modelName = model.modelName;
                            }
                        }

                        const modelData = ModelDataManager.getBuildingGroupModelData(buildingGroupID, modelList);
                        if (modelData) {
                            if (modelData.modelID) {
                                model.modelID = modelData.modelID;
                            }
                            if (modelData.modelFile) {
                                model.modelFile = modelData.modelFile;
                            }
                        }

                        gltfModel.childModels.push(childModel);
                    }
                }

                if (gltfModel.constructor === Object && Object.keys(gltfModel).length > 0) {
                    gltfModels.push(gltfModel);
                }
            }
        }

        return gltfModels;
    }

    static makeModel(indoorModels, modelList, buildingGroupList) {
        const gltfModel = {};                
        gltfModel.childModels = [];
        gltfModel.modelDatas = [];

        for (const key in indoorModels) {
            const value = indoorModels[key];
            if (SpaceDataManager.isBuildingModelData(value)) {
                const childModel = CommonDataManager.makeModel(value, modelList, buildingGroupList);                
                const buildingID = value.buildingID;
                if (buildingID) {
                    const model = ModelDataManager.getBuildingModel(buildingID, modelList, buildingGroupList);                    
                    if (model) {
                        if (model.id) {
                            childModel.id = model.id;
                        }
                        if (model.modelName) {
                            childModel.modelName = model.modelName;
                        }
                        if (model.parentID) {
                            childModel.parentID = model.parentID;
                        }
                    }

                    const modelData = ModelDataManager.getBuildingModelData(buildingID, modelList);
                    if (modelData) {
                        if (modelData.modelID) {
                            childModel.modelID = modelData.modelID;
                        }
                        if (modelData.modelFile) {
                            childModel.modelFile = modelData.modelFile;
                        }
                    }
                }

                gltfModel.childModels.push(childModel);
            }
            else {
                if (key === 'id') {
                    gltfModel.id = value;
                }
                else if (key === 'parentID') {
                    gltfModel.parentID = value;
                }
                else if (key === 'floors') {
                    const floorCount = value.length;                    
                    for (let i = 0; i < floorCount; i++) {
                        if (value[i].camera) {
                            const innerModelData = CommonDataManager.getCameraData(value[i].camera, false);                            
                            innerModelData.modelDisplayText = value[i].modelDisplayText;                            
                            innerModelData.floorIndex = value[i].floorIndex;
                            
                            const zoneID = value[i].zoneID;
                            innerModelData.zoneID = zoneID;

                            if (zoneID) {
                                const model = ModelDataManager.getZoneModel(zoneID, modelList, buildingGroupList);                                
                                if (model) {
                                    if (model.id) {
                                        innerModelData.id = model.id;
                                    }
                                    if (model.modelName) {
                                        innerModelData.modelDisplayText = model.modelName;
                                    }
                                }

                                const modelData = ModelDataManager.getZoneModelData(zoneID, modelList);
                                if (modelData) {
                                    if (modelData.id) {
                                        innerModelData.id = modelData.id;
                                    }
                                    if (modelData.modelID) {
                                        innerModelData.modelID = modelData.modelID;
                                    }
                                    if (modelData.modelFile) {
                                        innerModelData.modelFile = modelData.modelFile;
                                    }
                                }
                            }

                            gltfModel.modelDatas.push(innerModelData);
                        }
                        
                        if (value[i].cameraOrtho) {
                            let modelOrthoData = CommonDataManager.getCameraData(value[i].cameraOrtho, true);
                            //modelOrthoData.id = value[i].cameraOrtho.id;
                            modelOrthoData.modelID = value[i].modelID;
                            modelOrthoData.modelFile = value[i].file;
                            modelOrthoData.zoneID = value[i].zoneID;

                            if (!gltfModel.modelOrthoDatas) {
                                gltfModel.modelOrthoDatas = [];
                            }
                            gltfModel.modelOrthoDatas.push(modelOrthoData);
                        }
                    }
                }
                else if (key === 'camera') {
                    let innerModelData = CommonDataManager.getCameraData(value, false);                    
                    innerModelData.modelFile = indoorModels['file'];//-
                    innerModelData.buildingGroupID = indoorModels['buildingGroupID'];
                    innerModelData.buildingID = indoorModels['buildingID'];
                    innerModelData.modelDisplayText = indoorModels['modelDisplayText'];


                    let buildingGroupID = indoorModels['buildingGroupID'];
                    if (buildingGroupID) {
                        const model = ModelDataManager.getBuildingGroupModel(buildingGroupID, modelList, buildingGroupList);
                        if (model) {
                            if (model.id) {
                                innerModelData.id = model.id;
                            }
                            if (model.modelName) {
                                innerModelData.modelName = model.modelName;
                            }
                        }

                        const modelData = ModelDataManager.getBuildingGroupModelData(buildingGroupID, modelList);
                        if (modelData) {
                            if (modelData.modelID) {
                                innerModelData.modelID = modelData.modelID;
                            }
                            if (modelData.modelFile) {
                                innerModelData.modelFile = modelData.modelFile;
                            }
                        }
                    }
                    else {
                        let buildingID = indoorModels['buildingID'];
                        if (buildingID) {
                            const model = ModelDataManager.getBuildingModel(buildingID, modelList, buildingGroupList);
                            if (model) {
                                if (model.id) {
                                    innerModelData.id = model.id;
                                }
                                if (model.modelName) {
                                    innerModelData.modelName = model.modelName;
                                }
                            }

                            const modelData = ModelDataManager.getBuildingModelData(buildingID, modelList);
                            if (modelData) {
                                if (modelData.modelID) {
                                    innerModelData.modelID = modelData.modelID;
                                }
                                if (modelData.modelFile) {
                                    innerModelData.modelFile = modelData.modelFile;
                                }
                            }
                        }
                    }

                    gltfModel.modelDatas.push(innerModelData);
                }
            }
        }

        return gltfModel;
    }

    static getCameraData(camera, isOrtho) {
        const modelData = {};        
        modelData.id = camera.id;

        if (camera.position != null && camera.position.length === 3) {
            modelData.cameraPositionX = camera.position[0];
            modelData.cameraPositionY = camera.position[1];
            modelData.cameraPositionZ = camera.position[2];
        }
        if (camera.quaternion != null && camera.quaternion.length === 4) {
            modelData.cameraQuaternionX = camera.quaternion[0];
            modelData.cameraQuaternionY = camera.quaternion[1];
            modelData.cameraQuaternionZ = camera.quaternion[2];
            modelData.cameraQuaternionW = camera.quaternion[3];
        }
        if (camera.rotation != null && camera.rotation.length === 3) {
            modelData.cameraRotationX = camera.rotation[0];
            modelData.cameraRotationY = camera.rotation[1];
            modelData.cameraRotationZ = camera.rotation[2];
        }

        if (!isOrtho) {
            modelData.cameraFar = camera.far;
            modelData.cameraFov = camera.fov;
            modelData.cameraNear = camera.near;

            if (camera.targetControl != null && camera.targetControl.length === 3) {
                modelData.orbitTargetX = camera.targetControl[0];
                modelData.orbitTargetY = camera.targetControl[1];
                modelData.orbitTargetZ = camera.targetControl[2];
            }
        }
        else {            
            if (camera.targetControl != null && camera.targetControl.length === 3) {
                modelData.targetX = camera.targetControl[0];
                modelData.targetY = camera.targetControl[1];
                modelData.targetZ = camera.targetControl[2];
            }
            modelData.zoom = camera.zoom;
        }

        return modelData
    }

    static makeBuildingGroupList(buildingGroupList) {
        const _buildingGroupDatas = [];
        const _buildingDatas = [];
        const _zoneDatas = [];
        const _equipzoneDatas = [];

        const _equipzoneIDs = [];

        try {
            let buildingGroupDatas = [];
            for (let i = 0; i < buildingGroupList.length; i++) {
                let buildingDatas = [];
                let zoneDatas = [];
                let equipzoneDatas = [];

                const buildingGroup = buildingGroupList[i];

                for (let j = 0; j < buildingGroup.buildingDatas.length; j++) {
                    const building = buildingGroup.buildingDatas[j];

                    for (let o = 0; o < buildingGroup.buildingDatas[j].zoneDatas.length; o++) {
                        const zone = buildingGroup.buildingDatas[j].zoneDatas[o];

                        for (var p = 0; p < zone.equipmentZoneDatas.length; p++) {
                            const equipzone = zone.equipmentZoneDatas[p];
                            const isMatch = _equipzoneIDs.find(p => p === equipzone.id);

                            if (!isMatch) {
                                const equipzoneData = {};
                                equipzoneData.id = equipzone.id;
                                equipzoneData.zoneName = equipzone.zoneName;
                                equipzoneData.boundary = equipzone.boundary;
                                equipzoneData.linkedZoneIDs = equipzone.linkedZoneIDs;
                                equipzoneData.type = equipzone.type;
                                equipzoneData.textCenter = equipzone.textCenter;
                                equipzoneData.broadcastText = equipzone.broadcastText;
                                equipzoneData.displayText = equipzone.displayText;
                                equipzoneData.siteID = equipzone.siteID;

                                //equipzoneData.linkedZoneDatas = equipzone.linkedZoneDatas

                                if (equipzoneData.id === undefined || equipzoneData.zoneName === undefined || equipzoneData.boundary === undefined || equipzoneData.type === undefined
                                    || equipzoneData.textCenter === undefined || equipzoneData.broadcastText === undefined || equipzoneData.displayText === undefined || equipzoneData.siteID === undefined) {
                                    //console.log(equipzoneData);
                                }

                                equipzoneDatas.push(equipzoneData);
                                _equipzoneDatas.push(equipzoneData);

                                _equipzoneIDs.push(equipzone.id);
                            }
                        }

                        const zoneData = {};
                        zoneData.id = zone.id;
                        zoneData.zoneName = zone.zoneName;
                        zoneData.buildingID = zone.buildingID;
                        zoneData.floorIndex = zone.floorIndex;
                        zoneData.addFloor = zone.addFloor;
                        zoneData.boundary = zone.boundary;
                        zoneData.textCenter = zone.textCenter;
                        zoneData.broadcastText = zone.broadcastText;
                        zoneData.displayText = zone.displayText;
                        zoneData.siteID = zone.siteID;

                        //zoneData.equipmentZoneDatas = equipzoneDatas;
                        //zoneData.sensors = zone.sensors;
                        //zoneData.datas = zone.datas;

                        if (zoneData.equipmentZoneDatas === undefined || zoneData.addFloor === undefined || zoneData.boundary === undefined || zoneData.broadcastText === undefined
                            || zoneData.buildingID === undefined || zoneData.datas === undefined || zoneData.displayText === undefined || zoneData.floorIndex === undefined ||
                            zoneData.id === undefined || zoneData.siteID === undefined || zoneData.textCenter === undefined || zoneData.zoneName === undefined) {
                            //console.log(zoneData);
                        }
                        zoneDatas.push(zoneData);
                        _zoneDatas.push(zoneData);
                    }

                    const buildingData = {};
                    buildingData.id = building.id;
                    buildingData.buildingCode = building.buildingCode;
                    buildingData.buildingName = building.buildingName;
                    buildingData.buildingGroupID = building.buildingGroupID;
                    buildingData.maxFloor = building.maxFloor;
                    buildingData.minFloor = building.minFloor;
                    buildingData.textCenter = building.textCenter;
                    buildingData.broadcastText = building.broadcastText;
                    buildingData.displayText = building.displayText;

                    //buildingData.zoneDatas = zoneDatas;

                    if (buildingData.zoneDatas === undefined || buildingData.broadcastText === undefined || buildingData.buildingCode === undefined || buildingData.buildingGroupID === undefined ||
                        buildingData.buildingName === undefined || buildingData.displayText === undefined || buildingData.id === undefined || buildingData.maxFloor === undefined ||
                        buildingData.minFloor === undefined || buildingData.textCenter === undefined) {
                        //console.log(buildingData);
                    }

                    buildingDatas.push(buildingData);
                    _buildingDatas.push(buildingData);
                }

                const buildingGroupData = {};
                buildingGroupData.id = buildingGroup.id;
                buildingGroupData.groupName = buildingGroup.groupName;
                buildingGroupData.parentID = buildingGroup.parentID;
                buildingGroupData.textCenter = buildingGroup.textCenter; //(buildingGroup.textCenter.x) ? buildingGroup.textCenter : null;
                buildingGroupData.displayText = buildingGroup.displayText;
                buildingGroupData.siteID = (buildingGroup.siteID) ? buildingGroup.siteID : -1;
                //buildingGroupData.parent = null;

                //buildingGroupData.buildingDatas = buildingDatas;
                buildingGroupData.visible = (buildingGroup.visible) ? buildingGroup.visible : false;
                buildingGroupDatas.push(buildingGroupData);
                _buildingGroupDatas.push(buildingGroupData);

                if (buildingGroupData.id === undefined || buildingGroupData.groupName === undefined || buildingGroupData.parentID === undefined ||
                    buildingGroupData.textCenter === undefined || buildingGroupData.displayText === undefined || buildingGroupData.siteID === undefined ||
                    buildingGroupData.parent === undefined || buildingGroupData.buildingDatas === undefined) {
                    //console.log(buildingGroupData);
                }
            }

            const result = {
                _buildingGroupDatas: _buildingGroupDatas,
                _buildingDatas: _buildingDatas,
                _zoneDatas: _zoneDatas,
                _equipzoneDatas: _equipzoneDatas,
            }

            return [true, result];
        } catch (e) {
            return [false, e.message];
        }
    }

    static makeOutdoorZoneList(outdoorZoneList) {
        const outdoorZones = [];
        try {
            if (outdoorZoneList) {
                for (const outdoorZoneID in outdoorZoneList) {
                    const zone = outdoorZoneList[outdoorZoneID];
                    const zoneData = {};

                    zoneData.zoneName = zone.name;
                    zoneData.id = zone.id;
                    //zoneData.sensors = zone.sensors;
                    zoneData.datas = zone.datas;

                    //zoneData.addFloor = zone.addFloor;
                    //zoneData.boundary = zone.boundary;
                    //zoneData.broadcastText = zone.broadcastText;
                    zoneData.displayText = zone.displayText;
                    //zoneData.equipmentZoneDatas = zone.equipmentZoneDatas;
                    //zoneData.floorIndex = zone.floorIndex;
                    //zoneData.textCenter = zone.textCenter;

                    outdoorZones.push(zoneData);
                }

                return [true, outdoorZones];
            }
        } catch (e) {
            return [false, e.message];
        }
    }
}