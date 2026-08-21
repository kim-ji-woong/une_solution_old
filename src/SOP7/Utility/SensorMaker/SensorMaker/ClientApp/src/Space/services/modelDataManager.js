import { SpaceDataManager } from "./spaceDataManager";

export class ModelDataManager {
    static OutdoorModelName = "outdoorModel";
    static IndoorModelName = "indoorModels";

    static modelID = 0;
    static modelDataID = 0;
    
    static makeNewModelList() {
        const modelList = {
            models: {},
            modelDatas: {},
            //modelOrthoDatas: {}
        }

        return modelList;
    }

    static makeNewModel(modelName, parentID = null) {
        const model = {
            id: ++ModelDataManager.modelID,
            parentID: parentID,
            modelName: modelName,
            children: {}
        }

        return model;
    }

    static setOutdoorModel(modelList) {
        modelList.models[ModelDataManager.OutdoorModelName] = ModelDataManager.makeNewModel(ModelDataManager.OutdoorModelName);
        return modelList;
    }

    static getOutdoorModel(modelList) {
        return modelList.models[ModelDataManager.OutdoorModelName];
    }

    static getModelName(model, parentName) {
        return parentName ? parentName + "_" + model.modelName : model.modelName;
    }

    static modelTreeToList(model, modelList, rootValue, defaultValue, parentName = null) {
        parentName = ModelDataManager.getModelName(model, parentName);
        modelList[parentName] = rootValue;
        
        for (const childName in model.children) {
            const child = model.children[childName];
            ModelDataManager.modelTreeToList(child, modelList, defaultValue, defaultValue, parentName);
        }
    }

    static removeUnusedModels(model, oldModelList, parentModel = null, parentModelName = null) {
        const modelName = ModelDataManager.getModelName(model, parentModelName);
        const use = oldModelList[modelName];

        if (!use && parentModel) {
            delete parentModel.children[model.modelName];
            return;
        }

        for (const childName in model.children) {
            const child = model.children[childName];
            ModelDataManager.removeUnusedModels(child, oldModelList, model, modelName);
        }
    }

    static setIndoorModels(modelList, buildingGroupList) {
        let indoorModels = modelList.models[ModelDataManager.IndoorModelName];

        if (!indoorModels) {
            indoorModels = ModelDataManager.makeNewModel(ModelDataManager.IndoorModelName);
            modelList.models[ModelDataManager.IndoorModelName] = indoorModels;
        }

        const oldModelList = {};
        ModelDataManager.modelTreeToList(indoorModels, oldModelList, true, false);

        let parentModelName = indoorModels.modelName;

        for (const buildingGroup of buildingGroupList) {
            let buildingGroupModel = buildingGroup.visible ? ModelDataManager.findIndoorModel(buildingGroup.groupName, indoorModels.id, indoorModels) : indoorModels;

            if (!buildingGroupModel) {
                buildingGroupModel = ModelDataManager.makeNewModel(buildingGroup.groupName, indoorModels.id);
                indoorModels.children[buildingGroup.groupName] = buildingGroupModel;
            }

            if (buildingGroupModel) {
                let parentModelName2 = buildingGroup.visible ? ModelDataManager.getModelName(buildingGroupModel, parentModelName) : parentModelName;
                oldModelList[parentModelName2] = true;

                for (const building of buildingGroup.buildingDatas) {
                    let buildingModel = ModelDataManager.findIndoorModel(building.buildingName, buildingGroupModel.id, buildingGroupModel);

                    if (!buildingModel) {
                        buildingModel = ModelDataManager.makeNewModel(building.buildingName, buildingGroupModel.id);
                        buildingGroupModel.children[building.buildingName] = buildingModel;
                    }

                    if (buildingModel) {
                        let parentModelName3 = ModelDataManager.getModelName(buildingModel, parentModelName2);
                        oldModelList[parentModelName3] = true;

                        for (const zone of building.zoneDatas) {
                            let zoneModel = ModelDataManager.findIndoorModel(zone.zoneName, buildingModel.id, buildingModel);

                            if (!zoneModel) {
                                zoneModel = ModelDataManager.makeNewModel(zone.zoneName, buildingModel.id);
                                buildingModel.children[zone.zoneName] = zoneModel;
                            }

                            if (zoneModel) {
                                oldModelList[ModelDataManager.getModelName(zoneModel, parentModelName3)] = true;
                            }
                        }
                    }
                }
            }
        }

        // 사용하지 않는 model들은 제거한다.
        ModelDataManager.removeUnusedModels(indoorModels, oldModelList);
        return modelList;
    }

    static getIndoorModel(modelList) {
        return modelList.models[ModelDataManager.IndoorModelName];
    }

    static findIndoorModel(modelName, parentID, parentModel, findCascade = false) {
        if (parentModel) {
            for (const childName in parentModel.children) {
                const model = parentModel.children[childName];

                if (model.parentID === parentID && model.modelName === modelName) {
                    return model;
                }

                if (findCascade) {
                    const findModel = ModelDataManager.findIndoorModel(modelName, parentID, model, findCascade);

                    if (findModel) {
                        return findModel;
                    }
                }
            }
        }

        return null;
    }

    static makeModelData(fileName, modelID, floorIndex, buildingGroupID, buildingID, zoneID) {
        if (!modelID) {
            return null;
        }

        const modelData = {
            id: ++ModelDataManager.modelDataID,
            modelID: modelID,
            modelFile: fileName,
            floorIndex: floorIndex,
            buildingGroupID: buildingGroupID,
            buildingID: buildingID,
            zoneID: zoneID
        };

        return modelData;
    }

    static getOrthoModelDatas(modelList) {
        const modelDatas = modelList.modelDatas;

        if (!modelDatas) {
            return {};
        }

        let id = 0;
        const orthoModelDatas = {};

        for (const modelDataID in modelDatas) {
            const modelData = modelDatas[modelDataID];

            if (modelData.zoneID) {
                id++;

                orthoModelDatas[id] = {
                    id: id,
                    modelID: modelData.modelID,
                    modelFile: modelData.modelFile,
                    zoneID: modelData.zoneID
                };
            }
        }

        return orthoModelDatas;
    }

    static setModelDatas(modelFiles, modelList, buildingGroupList) {
        const outdoorModel = ModelDataManager.getOutdoorModel(modelList);

        if (!outdoorModel) {
            return modelList;
        }

        const modelDatas = {};
        modelList.modelDatas = modelDatas;
        const siteModelFileName = modelFiles.site;

        if (siteModelFileName && siteModelFileName.length > 0) {
            const siteModelData = ModelDataManager.makeModelData(siteModelFileName, outdoorModel.id, null, null, null, null);

            if (siteModelData) {
                modelDatas[siteModelData.id] = siteModelData;
            }
        }

        const indoorModel = ModelDataManager.getIndoorModel(modelList);

        if (!indoorModel) {
            return modelList;
        }

        const [buildingGroupModels, buildingModels, zoneModels, zones] = ModelDataManager.getBuildingGroupModels(indoorModel, buildingGroupList);

        for (const buildingGroupID in modelFiles.buildingGroups) {
            const fileName = modelFiles.buildingGroups[buildingGroupID];

            if (fileName && fileName.length > 0) {
                const buildingGroupModel = buildingGroupModels[buildingGroupID];

                if (buildingGroupModel) {
                    const modelData = ModelDataManager.makeModelData(fileName, buildingGroupModel.id, null, parseInt(buildingGroupID), null, null);
                    modelDatas[modelData.id] = modelData;
                }
            }
        }

        for (const buildingID in modelFiles.buildings) {
            const fileName = modelFiles.buildings[buildingID];

            if (fileName && fileName.length > 0) {
                const buildingModel = buildingModels[buildingID];

                if (buildingModel) {
                    const modelData = ModelDataManager.makeModelData(fileName, buildingModel.id, null, null, parseInt(buildingID), null);
                    modelDatas[modelData.id] = modelData;
                }
            }
        }

        for (const zoneID in modelFiles.zones) {
            const fileName = modelFiles.zones[zoneID];
            let zone = zones[zoneID];

            if (!zone) {
                continue;
            }

            if (fileName && fileName.length > 0) {
                const zoneModel = zoneModels[zoneID];

                if (zoneModel) {
                    const modelData = ModelDataManager.makeModelData(fileName, zoneModel.id, zone.floorIndex, null, null, parseInt(zoneID));
                    modelDatas[modelData.id] = modelData;
                }
            }
        }

        return modelList;
    }

    static getBuildingGroupModels(indoorModel, buildingGroupList) {
        const buildingGroupModels = {};
        const buildingModels = {};
        const zoneModels = {};
        const zones = {};

        for (const buildingGroup of buildingGroupList) {
            const buildingGroupModel = buildingGroup.visible ? ModelDataManager.findIndoorModel(buildingGroup.groupName, indoorModel.id, indoorModel) : indoorModel;

            if (buildingGroupModel) {
                if (buildingGroup.visible) {
                    buildingGroupModels[buildingGroup.id] = buildingGroupModel;
                }

                for (const building of buildingGroup.buildingDatas) {
                    const buildingModel = ModelDataManager.findIndoorModel(building.buildingName, buildingGroupModel.id, buildingGroupModel);

                    if (buildingModel) {
                        buildingModels[building.id] = buildingModel;

                        for (const zone of building.zoneDatas) {
                            const zoneModel = ModelDataManager.findIndoorModel(zone.zoneName, buildingModel.id, buildingModel);

                            if (zoneModel) {
                                zoneModels[zone.id] = zoneModel;
                            }

                            zones[zone.id] = zone;
                        }
                    }
                }
            }
        }

        return [buildingGroupModels, buildingModels, zoneModels, zones];
    }

    static getBuildingGroupModel(buildingGroupID, modelList, buildingGroupList) {
        const indoorModels = ModelDataManager.getIndoorModel(modelList);

        if (!indoorModels) {
            return null;
        }

        for (const buildingGroup of buildingGroupList) {
            if (buildingGroup.id.toString() === buildingGroupID.toString()) {
                return ModelDataManager.findIndoorModel(buildingGroup.groupName, indoorModels.id, indoorModels);
            }
        }

        return null;
    }

    static getBuildingModel(buildingID, modelList, buildingGroupList) {
        let buildingGroup = null, building = null;

        for (const bg of buildingGroupList) {
            for (const b of bg.buildingDatas) {
                if (b.id.toString() === buildingID.toString()) {
                    building = b;
                    break;
                }
            }

            if (building) {
                buildingGroup = bg;
                break;
            }
        }

        if (!buildingGroup) {
            return null;
        }

        const buildingGroupModel = ModelDataManager.getBuildingGroupModel(buildingGroup.id, modelList, buildingGroupList);

        if (buildingGroupModel) {
            return ModelDataManager.findIndoorModel(building.buildingName, buildingGroupModel.id, buildingGroupModel);
        }

        return null;
    }

    static getZoneModel(zoneID, modelList, buildingGroupList) {
        let buildingGroup = null, building = null, zone = null;

        for (const bg of buildingGroupList) {
            for (const b of bg.buildingDatas) {
                for (const z of b.zoneDatas) {
                    if (z.id.toString() === zoneID.toString()) {
                        zone = z;
                        break;
                    }
                }

                if (zone) {
                    building = b;
                    break;
                }
            }

            if (building) {
                buildingGroup = bg;
                break;
            }
        }

        if (!buildingGroup) {
            return null;
        }

        const buildingGroupModel = ModelDataManager.getBuildingGroupModel(buildingGroup.id, modelList, buildingGroupList);

        if (buildingGroupModel) {
            const buildingModel = ModelDataManager.findIndoorModel(building.buildingName, buildingGroupModel.id, buildingGroupModel);

            if (buildingModel) {
                return ModelDataManager.findIndoorModel(zone.zoneName, buildingModel.id, buildingModel);
            }
        }

        return null;
    }

    static getBuildingGroupModelData(buildingGroupID, modelList) {
        for (const modelDataID in modelList.modelDatas) {
            const modelData = modelList.modelDatas[modelDataID];

            if (modelData.buildingGroupID?.toString() === buildingGroupID.toString()) {
                return modelData;
            }
        }

        return null;
    }

    static getBuildingModelData(buildingID, modelList) {
        for (const modelDataID in modelList.modelDatas) {
            const modelData = modelList.modelDatas[modelDataID];

            if (modelData.buildingID?.toString() === buildingID.toString()) {
                return modelData;
            }
        }

        return null;
    }

    static getZoneModelData(zoneID, modelList) {
        for (const modelDataID in modelList.modelDatas) {
            const modelData = modelList.modelDatas[modelDataID];

            if (modelData.zoneID?.toString() === zoneID.toString()) {
                return modelData;
            }
        }

        return null;
    }
}
