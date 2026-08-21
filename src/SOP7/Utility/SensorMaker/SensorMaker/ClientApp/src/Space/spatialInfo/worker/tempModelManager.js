import { ModelDataManager } from "../../services/modelDataManager";
import { SpaceDataManager } from "../../services/spaceDataManager";
import { SpaceBody } from "../../spaceBody";

export class TempModelManager {
    constructor(siteInfo) {
        this.siteInfo = siteInfo;
    }

    static makeTempModelFiles() {
        const tempModelFiles = {
            site: null,
            buildingGroups: {},
            buildings: {},
            zones: {}
        };

        return tempModelFiles;
    }

    static makeTempModelFilesFrom3dOptions(_3dOptions) {
        const tempModelFiles = TempModelManager.makeTempModelFiles();
        const outdoorModel = _3dOptions[ModelDataManager.OutdoorModelName];

        if (outdoorModel?.file && outdoorModel.file.length > 0) {
            TempModelManager._onSetModelFile(outdoorModel.file, SpaceBody.Type_Site, null, tempModelFiles);
        }

        const indoorModels = _3dOptions[ModelDataManager.IndoorModelName];

        if (indoorModels) {
            for (const buildingGroupName in indoorModels) {
                const buildingGroupModel = indoorModels[buildingGroupName];
                if (buildingGroupModel === null)
                    continue;

                if (buildingGroupModel.file && buildingGroupModel.file.length > 0) {
                    TempModelManager._onSetModelFile(buildingGroupModel.file, SpaceBody.Type_BuildingGroup, buildingGroupModel.buildingGroupID, tempModelFiles);
                }

                for (const buildingName in buildingGroupModel) {
                    const buildingModel = buildingGroupModel[buildingName];

                    if (SpaceDataManager.isBuildingModelData(buildingModel)) {
                        if (buildingModel.file && buildingModel.file.length > 0) {
                            TempModelManager._onSetModelFile(buildingModel.file, SpaceBody.Type_Building, buildingModel.buildingID, tempModelFiles);
                        }

                        if (buildingModel.floors) {
                            for (const zoneModel of buildingModel.floors) {
                                if (zoneModel.file && zoneModel.file.length > 0) {
                                    TempModelManager._onSetModelFile(zoneModel.file, SpaceBody.Type_Floor, zoneModel.zoneID, tempModelFiles);
                                }
                            }
                        }
                    }
                }
            }
        }

        return tempModelFiles;
    }

    onSetModelFile(fileName, type, id, update) {
        const tempModelFiles = { ...this.siteInfo.state.tempModelFiles };
        TempModelManager._onSetModelFile(fileName, type, id, tempModelFiles);

        if (update) {
            this.siteInfo.setState({ tempModelFiles });
        }

        return tempModelFiles;
    }

    static _onSetModelFile(fileName, type, id, tempModelFiles) {
        if (type === SpaceBody.Type_Site) {
            tempModelFiles.site = fileName;
        }
        else if (type === SpaceBody.Type_BuildingGroup) {
            if (fileName)
                tempModelFiles.buildingGroups[id] = fileName;
            else
                delete tempModelFiles.buildingGroups[id];
        }
        else if (type === SpaceBody.Type_Building) {
            if (fileName)
                tempModelFiles.buildings[id] = fileName;
            else
                delete tempModelFiles.buildings[id];
        }
        else if (type === SpaceBody.Type_Floor) {
            if (fileName)
                tempModelFiles.zones[id] = fileName;
            else
                delete tempModelFiles.zones[id];
        }
    }

    rollbackTempModelFiles(_3dOptions, tempModelFiles) {
        const outdoorModelFile = _3dOptions.outdoorModel?.file;

        if (outdoorModelFile) {
            tempModelFiles.site = outdoorModelFile;
        }
        else {
            tempModelFiles.site = outdoorModelFile;
        }

        tempModelFiles.buildingGroups = {};
        tempModelFiles.buildings = {};
        tempModelFiles.zones = {};

        const indoorModels = _3dOptions.indoorModels;

        if (indoorModels) {
            for (const buildingGroupName in indoorModels) {
                const buildingGroupModelData = indoorModels[buildingGroupName];

                if (buildingGroupModelData.file && buildingGroupModelData.file.length > 0) {
                    tempModelFiles.buildingGroups[buildingGroupModelData.buildingGroupID] = buildingGroupModelData.file;
                }

                for (const buildingName in buildingGroupModelData) {
                    const buildingModelData = buildingGroupModelData[buildingName];

                    if (SpaceDataManager.isBuildingModelData(buildingModelData)) {
                        if (buildingModelData.file && buildingModelData.file.length > 0) {
                            tempModelFiles.buildings[buildingModelData.buildingID] = buildingModelData.file;
                        }

                        const floors = buildingModelData.floors;

                        for (const zoneModelData of floors) {
                            if (zoneModelData.file !== null && zoneModelData.file !== undefined && zoneModelData.file.length > 0) {
                                tempModelFiles.zones[zoneModelData.zoneID] = zoneModelData.file;
                            }
                        }
                    }
                }
            }
        }

        this.siteInfo.setState({ tempModelFiles, updateModelingFilePath: true });
    }

    isChanged() {
        const tempModelFiles = { ...this.siteInfo.state.tempModelFiles };
        const _3dOptions = { ...this.siteInfo.props._3dOptions };

        if (TempModelManager.isChangedOutdoorModelFile(tempModelFiles, _3dOptions)) {
            return true;
        }

        if (TempModelManager.isChangedBuildingGroupModelFiles(tempModelFiles, _3dOptions)) {
            return true;
        }

        if (TempModelManager.isChangedBuildingModelFiles(tempModelFiles, _3dOptions)) {
            return true;
        }

        if (TempModelManager.isChangedZoneModelFiles(tempModelFiles, _3dOptions)) {
            return true;
        }

        return false;
    }

    static isChangedZoneModelFiles(tempModelFiles, _3dOptions) {
        // tempModelFiles의 내용이 _3dOptions와 다른지 검사
        for (const zoneID in tempModelFiles.zones) {
            const zoneModelData = SpaceDataManager.getZoneModelData(_3dOptions, zoneID);

            if (zoneModelData && zoneModelData.file !== null && zoneModelData.file.length > 0) {
                if (tempModelFiles.zones[zoneID] !== zoneModelData.file) {
                    return true;
                }
            }
            else {
                if (TempModelManager.isNullModelFileName(tempModelFiles.zones[zoneID]) === false) {
                    return true;
                }
            }
        }

        const indoorModels = _3dOptions.indoorModels;

        // _3dOptions에 있는 것들중에 tempModelFiles에 없는 것들이 있는지 검사
        if (indoorModels) {
            for (const buildingGroupName in indoorModels) {
                const buildingGroupModelData = indoorModels[buildingGroupName];

                for (const buildingName in buildingGroupModelData) {
                    const buildingModelData = buildingGroupModelData[buildingName];

                    if (SpaceDataManager.isBuildingModelData(buildingModelData)) {
                        const floors = buildingModelData.floors;

                        for (const zoneModelData of floors) {
                            if (zoneModelData.file !== null && zoneModelData.file !== undefined && zoneModelData.file.length > 0) {
                                if (!tempModelFiles.zones[zoneModelData.zoneID]) {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    static isChangedBuildingModelFiles(tempModelFiles, _3dOptions) {
        const indoorModels = _3dOptions.indoorModels;

        // tempModelFiles의 내용이 _3dOptions와 다른지 검사
        for (const buildingID in tempModelFiles.buildings) {
            const buildingModelData = SpaceDataManager.getBuildingModelData(buildingID, indoorModels, _3dOptions);

            if (buildingModelData && buildingModelData.file !== null && buildingModelData.file.length > 0) {
                if (tempModelFiles.buildings[buildingID] !== buildingModelData.file) {
                    return true;
                }
            }
            else {
                if (TempModelManager.isNullModelFileName(tempModelFiles.buildings[buildingID]) === false) {
                    return true;
                }
            }
        }

        // _3dOptions에 있는 것들중에 tempModelFiles에 없는 것들이 있는지 검사
        if (indoorModels) {
            for (const buildingGroupName in indoorModels) {
                const buildingGroupModelData = indoorModels[buildingGroupName];

                for (const buildingName in buildingGroupModelData) {
                    const buildingModelData = buildingGroupModelData[buildingName];

                    if (SpaceDataManager.isBuildingModelData(buildingModelData)) {
                        if (buildingModelData.file !== null && buildingModelData.file !== undefined && buildingModelData.file.length > 0) {
                            if (!tempModelFiles.buildings[buildingModelData.buildingID]) {
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    static isChangedBuildingGroupModelFiles(tempModelFiles, _3dOptions) {
        const indoorModels = _3dOptions.indoorModels;

        // tempModelFiles의 내용이 _3dOptions와 다른지 검사
        for (const buildingGroupID in tempModelFiles.buildingGroups) {
            const buildingGroupModelData = indoorModels ? SpaceDataManager.getBuildingGroupModelData(buildingGroupID, indoorModels) : null;

            if (buildingGroupModelData && buildingGroupModelData.file !== null && buildingGroupModelData.file.length > 0) {
                if (tempModelFiles.buildingGroups[buildingGroupID] !== buildingGroupModelData.file) {
                    return true;
                }
            }
            else {
                if (TempModelManager.isNullModelFileName(tempModelFiles.buildingGroups[buildingGroupID]) === false) {
                    return true;
                }
            }
        }

        // _3dOptions에 있는 것들중에 tempModelFiles에 없는 것들이 있는지 검사
        if (indoorModels) {
            for (const buildingGroupName in indoorModels) {
                const buildingGroupModelData = indoorModels[buildingGroupName];

                if (buildingGroupModelData && buildingGroupModelData.file !== null && buildingGroupModelData.file !== undefined && buildingGroupModelData.file.length > 0) {
                    if (!tempModelFiles.buildingGroups[buildingGroupModelData.buildingGroupID]) {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    static isChangedOutdoorModelFile(tempModelFiles, _3dOptions) {
        const outdoorModelFile = _3dOptions.outdoorModel?.file;

        if (!tempModelFiles.site || tempModelFiles.site.length === 0) {
            if (outdoorModelFile && outdoorModelFile.length > 0) {
                return true;
            }
        }
        else {
            if (!outdoorModelFile || outdoorModelFile.length === 0) {
                return true;
            }

            if (outdoorModelFile !== tempModelFiles.site) {
                return true;
            }
        }

        return false;
    }

    static isNullModelFileName(modelFileName) {
        if (modelFileName === null || modelFileName === undefined) {
            return true;
        }

        if (modelFileName.length == 0) {
            return true;
        }

        return false;
    }

    initData(id, type) {
        const tempModelFiles = { ...this.siteInfo.state.tempModelFiles };

        if (type === SpaceBody.Type_Site) {
            tempModelFiles.site = null;
        }
        else if (type === SpaceBody.Type_BuildingGroup) {
            delete tempModelFiles.buildingGroups[id];
        }
        else if (type === SpaceBody.Type_Building) {
            delete tempModelFiles.buildings[id];
        }
        else if (type === SpaceBody.Type_Floor) {
            delete tempModelFiles.zones[id];
        }

        return tempModelFiles;
    }
}
