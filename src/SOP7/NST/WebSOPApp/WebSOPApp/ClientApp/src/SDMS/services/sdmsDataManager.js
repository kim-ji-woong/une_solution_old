import { SDMSController } from "./sdmsController";
import * as Common from "../../Common/data/common";
import * as Backend from "../data/backend";
import * as SdmsCommon from "../data/common";
import * as Frontend from "../data/frontend";

export class SDMSDataManager {
    static BoundingBoxTag = "-0";

    static async get3DOptions(buildingGroupList/*: Array<Backend.BuildingGroup> | null*/, outdoorZones/*: Array<Backend.Zone> | null*/, errorMessage/*: string*/)/*: Promise<Frontend._3DOptions>*/ {
        if (!buildingGroupList && errorMessage && errorMessage.length > 0) {
            alert(errorMessage);
        }
        else {
            const buildingGroups = [];
            //const buildingGroups = new Array<Frontend.BuildingGroup>();
            // Key : BuildingGroup Name
            // Value.Key : Building Name
            const buildings = {};
            //const buildings = new Map<string, Map<string, Frontend.Building>>();
            // Key : Zone ID
            const zones = {};
            //const zones = new Map<string, Frontend.Zone>();
            const buildingGroupCount = buildingGroupList.length;

            for (let i = 0; i < buildingGroupCount; i++) {
                const buildingGroup = buildingGroupList[i];
                buildings[buildingGroup.groupName] = SDMSDataManager.getBuildings(buildingGroup.buildingDatas, zones);

                if (!buildingGroup.textCenter) {
                    continue;
                }

                const bgData = [];

                bgData.push(buildingGroup.groupName);
                bgData.push(buildingGroup.displayText);
                bgData.push(buildingGroup.groupName + SDMSDataManager.BoundingBoxTag);
                bgData.push(buildingGroup.textCenter.x);
                bgData.push(buildingGroup.textCenter.y);
                bgData.push(buildingGroup.textCenter.z);
                bgData.push(buildingGroup.id);

                buildingGroups.push(bgData/* as Frontend.BuildingGroup*/);
            }

            if (buildingGroups.length === 0) {
                alert("buildingGroups is empty");
            }

            const [models, option, message] = await SDMSController.requestGltfModelList();

            if (!models && message && message.length > 0) {
                alert(message);
                return {}/* as Frontend._3DOptions*/;
            }
            else {
                const _3DOptions = this.make3DOptions(buildingGroups, outdoorZones, buildings, zones, option, models);
                return _3DOptions;
            }
        }

        return {}/* as Frontend._3DOptions*/;
    }

    static getBuildings(buildingDatas/*: Array<Backend.Building>*/, zones/*: Map<string, Frontend.Zone>*/)/*: Map<string, Frontend.Building>*/ {
        // key : Building Name
        const buildings = {};
        //const buildings = new Map<string, Frontend.Building>();
        const buildingCount = buildingDatas.length;

        for (let i = 0; i < buildingCount; i++) {
            const building = buildingDatas[i];
            const buildingData = [];

            buildingData.push(building.id);
            buildingData.push(building.displayText);
            buildingData.push(building.buildingName + SDMSDataManager.BoundingBoxTag);

            const textCenter = building.textCenter;

            if (textCenter) {
                buildingData.push(textCenter.x);
                buildingData.push(textCenter.y);
                buildingData.push(textCenter.z);
            }
            else {
                buildingData.push(null);
                buildingData.push(null);
                buildingData.push(null);
            }

            // Key : Zone ID
            const buildingZones = {};
            //const buildingZones = new Map<number, Frontend.Zone>();
            // Key : Zone ID
            const equipZoneDatas = {};
            //const equipZoneDatas = new Map<number, Array<Backend.EquipmentZone>>();
            const buildingZoneDatas = building.zoneDatas;

            SDMSDataManager.getZones(buildingZoneDatas, buildingZones, equipZoneDatas);
            buildingData.push(buildingZones);
            buildings[building.buildingName] = buildingData;

            for (const zoneID in buildingZones) {
                const zone = [...buildingZones[zoneID]]/* as Frontend.Zone*/;
                zone.sensors = {};
                zones[zoneID] = zone;

                zone.equipZones = {};
                //zone.equipZones = new Map<number, Frontend.EquipmentZone>();
                zone.datas = SDMSDataManager.getZoneDatas(parseInt(zoneID), buildingZoneDatas);

                const equipmentZoneDatas = equipZoneDatas[parseInt(zoneID)];

                if (equipmentZoneDatas) {
                    const equipZoneCount = equipmentZoneDatas.length;

                    for (let j = 0; j < equipZoneCount; j++) {
                        const equipmentZoneData = equipmentZoneDatas[j];
                        const equipZoneData = [];

                        equipZoneData.push(equipmentZoneData.id);
                        equipZoneData.push(equipmentZoneData.zoneName);
                        equipZoneData.push(equipmentZoneData.textCenter);

                        const equipZones = zone.equipZones;
                        equipZones[equipmentZoneData.id] = equipZoneData;
                    }
                }
            }
        }

        return buildings;
    }

    static getZoneDatas(zoneID/*: number*/, zoneDatas/*: Array<Backend.Zone>*/)/*: SdmsCommon.ZoneData*/ {
        const dataCount = zoneDatas.length;

        for (let i = 0; i < dataCount; i++) {
            const zoneData = zoneDatas[i];

            if (zoneData.id === zoneID) {
                return zoneData.datas;
            }
        }

        return {
            fakeWallElevation: null,
            poiElevation: null,
            zoneID: -1
        };
    }

    static getZones(zoneDatas/*: Array<Backend.Zone>*/, zones/*: Map<number, Frontend.Zone>*/, equipZoneDatas/*: Map<number, Array<Backend.EquipmentZone>>*/)/*: void*/ {
        zoneDatas.sort((zone1, zone2) => {
            const floor1 = SDMSDataManager.getZoneFloor(zone1);
            const floor2 = SDMSDataManager.getZoneFloor(zone2);
            return floor1 - floor2;
        });

        const zoneCount = zoneDatas.length;

        for (let i = 0; i < zoneCount; i++) {
            const zone = zoneDatas[i];

            if (zone.textCenter === null) {
                zones[zone.id] = [SDMSDataManager.getZoneFloor(zone), zone.buildingID, zone.zoneName, zone.displayText, null, null, null];
            }
            else {
                const textCenter = zone.textCenter;
                zones[zone.id] = [SDMSDataManager.getZoneFloor(zone), zone.buildingID, zone.zoneName, zone.displayText, textCenter.x, textCenter.y, textCenter.z];
            }

            equipZoneDatas[zone.id] = zone.equipmentZoneDatas;
        }
    }

    static getZoneFloor(zone/*: object*/)/*: number*/ {
        if (zone["addFloor"] === null)
            return zone["floorIndex"];

        return zone["floorIndex"] + zone["addFloor"];
    }

    static make3DOptions(buildingGroups/*: Array<Frontend.BuildingGroup>*/, outdoorZones/*: Array<Backend.Zone> | null*/, buildings/*: Map<string, Map<string, Frontend.Building>>*/, zones/*: Map<string, Frontend.Zone>*/, option/*: SdmsCommon.GltfOption*/, models/*: Array<SdmsCommon.GltfModel>*/)/*: Frontend._3DOptions*/ {
        const _3DOptions = {}/* as Frontend._3DOptions*/;
        const modelCount = models.length;

        for (let i = 0; i < modelCount; i++) {
            const model = models[i];
            const json = SDMSDataManager.addModel(model);

            _3DOptions[model.modelName] = json;
            /*if (SDMSDataManager.isEmpty(json) === false) {
                _3DOptions[model.modelName] = json;
            }*/
        }

        const allBuildings = {};
        //const allBuildings = new Map<string, Frontend.Building2>();
        const buildingIDs = {};
        //const buildingIDs = new Map<string, Frontend.Building2>();

        for (const buildingGroupName in buildings) {
            const buildingGroup = buildings[buildingGroupName];

            for (const buildingName in buildingGroup) {
                const building = [...buildingGroup[buildingName]];

                // BuildingGroupName 추가
                building.unshift(buildingGroupName);
                building.unshift(building[1]);
                building.splice(2, 1);

                allBuildings[buildingName] = building;
                buildingIDs[building[0].toString()] = building;
            }
        }

        const _outdoorZones = {};
        //const _outdoorZones = new Map<string, Frontend.OutdoorZone>();
        const outdoorZoneCount = outdoorZones.length;

        for (let i = 0; i < outdoorZoneCount; i++) {
            const zone = outdoorZones[i];
            const zoneID = zone.id;

            const zoneData = {
                name: zone.zoneName,
                id: zoneID,
                sensors: {},
                datas: zone.datas
            };

            _outdoorZones[zoneID.toString()] = zoneData;
        }

        _3DOptions.buildingGroups = buildingGroups;
        _3DOptions.buildings = buildings;
        _3DOptions.allBuildings = allBuildings;
        _3DOptions.buildingIDs = buildingIDs;
        _3DOptions.zones = zones;
        _3DOptions.outdoorZones = _outdoorZones;
        _3DOptions.modelBaseURL = option["_3DModelBaseURL"];
        _3DOptions.textureBaseURL = option["_3DTextureBaseURL"];
        _3DOptions.backgroundImage = option["_3DBackgroundImage"];

        return _3DOptions;
    }

    static addModel(model/*: SdmsCommon.GltfModel*/)/*: object*/ {
        const data = {};

        const childModels = model.childModels;
        const childModelCount = childModels.length;

        for (let i = 0; i < childModelCount; i++) {
            const childModel = childModels[i];
            const json = SDMSDataManager.addModel(childModel);

            data[childModel.modelName] = json;
            /*if (this.isEmpty(json) === false) {
                data[childModel.modelName] = json;
            }*/
        }

        const modelDatas = model.modelDatas;
        const modelDataCount = modelDatas.length;
        const floors = [];

        for (let i = 0; i < modelDataCount; i++) {
            const modelData = modelDatas[i];
            const modelOrthoData = SDMSDataManager.getOrthoDataModel(modelData.modelFile, model.modelOrthoDatas);

            const floorIndex = modelData.floorIndex;

            if (floorIndex !== null && floorIndex !== undefined) {
                const floor = {
                    file: modelData.modelFile,
                    camera: SDMSDataManager.getCameraData(modelData),
                    modelDisplayText: modelData.modelDisplayText,
                    floorIndex: floorIndex,
                    cameraOrtho: {},
                    buildingGroupID: null,
                    buildingID: null,
                    zoneID: null
                };

                if (modelOrthoData) {
                    floor.cameraOrtho = SDMSDataManager.getCameraOrthoData(modelOrthoData);
                }

                const buildingGroupID = modelData.buildingGroupID;

                if (buildingGroupID) {
                    floor.buildingGroupID = buildingGroupID;
                }

                const buildingID = modelData.buildingID;

                if (buildingID) {
                    floor.buildingID = buildingID;
                }

                const zoneID = modelData.zoneID;

                if (zoneID) {
                    floor.zoneID = zoneID;
                }

                floors.push(floor);
            }
            else {
                data["file"] = modelData.modelFile;
                data["camera"] = SDMSDataManager.getCameraData(modelData);
                data["modelDisplayText"] = modelData.modelDisplayText;

                if (modelOrthoData) {
                    data["cameraOrtho"] = SDMSDataManager.getCameraOrthoData(modelOrthoData);
                }

                const buildingGroupID = modelData.buildingGroupID;

                if (buildingGroupID) {
                    data["buildingGroupID"] = buildingGroupID;
                }

                const buildingID = modelData.buildingID;

                if (buildingID) {
                    data["buildingID"] = buildingID;
                }

                const zoneID = modelData.zoneID;

                if (zoneID) {
                    data["zoneID"] = zoneID;
                }
            }
        }

        if (floors.length > 0) {
            data["floors"] = floors;
        }

        return data;
    }

    static getOrthoDataModel(modelFileName/*: string*/, modelOrthoDatas/*: Array<SdmsCommon.GltfModelOrthoData> | null*/)/*: SdmsCommon.GltfModelOrthoData | null*/ {
        if (!modelOrthoDatas) {
            return null;
        }

        const modelCount = modelOrthoDatas.length;

        for (let i = 0; i < modelCount; i++) {
            const modelOrtho = modelOrthoDatas[i];

            if (modelOrtho.modelFile === modelFileName) {
                return modelOrtho;
            }
        }

        return null;
    }

    static isEmpty(json/*: object*/)/*: boolean*/ {
        for (const key in json) {
            return false;
        }

        return true;
    }

    static getCameraData(modelData/*: SdmsCommon.GltfModelData*/)/*: Frontend.PerspectiveCameraData*/ {
        const data = {
            position: SDMSDataManager.getVector3(modelData.cameraPosition),
            quaternion: SDMSDataManager.getVector4(modelData.cameraQuaternion),
            rotation: SDMSDataManager.getVector3(modelData.cameraRotation),
            targetControl: SDMSDataManager.getVector3(modelData.orbitTarget),
            fov: modelData.cameraFov,
            near: modelData.cameraNear,
            far: modelData.cameraFar
        };

        return data;
    }

    static getCameraOrthoData(modelOrthoData/*: SdmsCommon.GltfModelOrthoData*/)/*: Frontend.OrthographicCameraData*/ {
        const data = {
            position: SDMSDataManager.getVector3(modelOrthoData.cameraPosition),
            quaternion: SDMSDataManager.getVector4(modelOrthoData.cameraQuaternion),
            rotation: SDMSDataManager.getVector3(modelOrthoData.cameraRotation),
            targetControl: SDMSDataManager.getVector3(modelOrthoData.target),
            zoom: modelOrthoData.zoom
        };

        return data;
    }

    static getVector3(vector/*: Common.Vector3*/)/*: Common.Vector3Array*/ {
        const data = [vector.x, vector.y, vector.z];
        return data/* as Common.Vector3Array*/;
    }

    static getVector4(vector/*: Common.Vector4*/)/*: Common.Vector4Array*/ {
        const data = [vector.x, vector.y, vector.z, vector.w];
        return data /*as Common.Vector4Array*/;
    }

    static getZoneModelData(_3dOptions/*: Frontend._3DOptions*/, zoneID/*: number*/)/*: [string, string | null | undefined, Frontend.PerspectiveCameraData] | null*/ {
        const zone = _3dOptions.zones[zoneID.toString()]/* as Frontend.Zone*/;

        if (!zone) {
            return null;
        }

        const buildingID = zone[1];
        const building = _3dOptions.buildingIDs[buildingID.toString()]/* as Frontend.Building2*/;

        if (!building) {
            return null;
        }

        const buildingGroupName = building[1];
        const buildingName = building[2];

        const buildingGroup = _3dOptions.indoorModels[buildingGroupName] /*as Frontend.IndoorModel*/;

        if (!buildingGroup) {
            return null;
        }

        const buildingData = buildingGroup[buildingName] /*as Frontend.BuildingModel*/;

        if (!buildingData || !buildingData.floors) {
            return null;
        }

        const floorCount = buildingData.floors.length;

        for (let i = 0; i < floorCount; i++) {
            const floor = buildingData.floors[i];

            if (floor.zoneID === zoneID) {
                return floor.file && floor.camera ? [floor.file, floor.modelDisplayText, floor.camera] : null;
            }
        }

        return null;
    }
}