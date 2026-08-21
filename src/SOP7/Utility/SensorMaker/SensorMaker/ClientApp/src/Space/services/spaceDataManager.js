import { SpaceBody } from "../spaceBody";
import { SpaceController } from "./spaceController";

export class SpaceDataManager {
    static BoundingBoxTag = "-0";

    static buildingGroupID = 0;
    static buildingID = 0;
    static zoneID = 0;
    static equipZoneID = 0;
    static outdoorZoneID = 19999;

    static fireSensorID = 0;
    static psmSensorID = 0; 
    static etcSensorID = 0;
    static cctvID = 0;

    static FireSensorType = "fire";
    static PSMSensorType = "psm";
    static EtcSensorType = "etc";
    static CCTVType = "cctv";

    static makeEquipZone(zone, equipZoneName) {        
        const eqiupZone = {
            boundary: {},
            broadcastText: equipZoneName,
            displayText: equipZoneName,
            id: ++SpaceDataManager.equipZoneID,
            linkedZoneDatas: [zone],
            linkedZoneIDs: [zone.id],
            //textCenter: {
            //    x: null,
            //    y: null,
            //    z: null
            //},
            textCenter: null,
            zoneName: equipZoneName,
            sensors: {},
            type: null
        };

        return eqiupZone;
    }

    static addEquipZoneToZones(equipZone, zone, _3dOptions) {
        let zoneData = _3dOptions.zones[zone.id];

        if (!zoneData) {
            zoneData = _3dOptions.outdoorZones[zone.id];
        }

        if (zoneData) {
            if (!zoneData.equipZones) {
                zoneData.equipZones = {};
            }

            zoneData.equipZones[equipZone.id] = [equipZone.id, equipZone.zoneName, equipZone.textCenter];
        }
    }

    static removeEquipZoneFromZones(equipZone, zone, _3dOptions) {
        const zoneData = _3dOptions.zones[zone.id];

        if (zoneData) {
            if (zoneData.equipZones) {
                delete zoneData.equipZones[equipZone.id];
            }
        }
    }

    static renameEquipZoneToZones(equipZone, zone, equipZoneName, _3dOptions) {
        let zoneData = _3dOptions.zones[zone.id];

        if (!zoneData) {
            zoneData = _3dOptions.outdoorZones[zone.id];
        }

        if (zoneData) {
            if (!zoneData.equipZones) {
                zoneData.equipZones = {};
            }

            zoneData.equipZones[equipZone.id] = [equipZone.id, equipZoneName, equipZone.textCenter];
        }
    }

    static removeEquipZoneFromZone(equipZone, zone) {
        const equipZoneCount = zone.equipmentZoneDatas.length;

        for (let i = 0; i < equipZoneCount; i++) {
            if (zone.equipmentZoneDatas[i].id === equipZone.id) {
                zone.equipmentZoneDatas.splice(i, 1);
                break;
            }
        }
    }

    static renameEquipZoneToZone(equipZone, zone, equipZoneName) {
        equipZone.zoneName = equipZone.broadcastText = equipZone.displayText = equipZoneName;
        const equipZoneCount = zone.equipmentZoneDatas.length;

        for (let i = 0; i < equipZoneCount; i++) {
            if (zone.equipmentZoneDatas[i].id === equipZone.id) {
                const equipZoneData = zone.equipmentZoneDatas[i];
                equipZoneData.zoneName = equipZoneData.broadcastText = equipZoneData.displayText = equipZoneName;
                break;
            }
        }
    }

    static addEquipZone(zone, equipZoneName, _3dOptions) {
        const equipZone = SpaceDataManager.makeEquipZone(zone, equipZoneName);
        zone.equipmentZoneDatas.push(equipZone);

        SpaceDataManager.addEquipZoneToZones(equipZone, zone, _3dOptions);
    }

    static removeEquipZone(equipZone, zone, _3dOptions) {
        SpaceDataManager.removeEquipZoneFromZone(equipZone, zone);
        SpaceDataManager.removeEquipZoneFromZones(equipZone, zone, _3dOptions);
    }

    static renameEquipZone(equipZone, zone, equipZoneName, _3dOptions) {
        SpaceDataManager.renameEquipZoneToZone(equipZone, zone, equipZoneName);
        SpaceDataManager.renameEquipZoneToZones(equipZone, zone, equipZoneName, _3dOptions);
    }

    static makeBuildingGroup(buildingGroupName, parentBuildingGroup) {
        const buildingGroup = {
            buildingDatas: [],
            displayText: buildingGroupName,
            groupName: buildingGroupName,
            id: ++SpaceDataManager.buildingGroupID,
            parent: parentBuildingGroup,
            parentID: parentBuildingGroup ? parentBuildingGroup.id : null,
            //textCenter: {
            //    x: null,
            //    y: null,
            //    z: null
            //},
            textCenter: null,
            visible: true
        };

        return buildingGroup;
    }

    static makeZone(zoneName, building) {
        let zone = null;

        if (building) {
            zone = {
                addFloor: null,
                boundary: null,
                broadcastText: zoneName,
                buildingID: building.id,
                datas: {},
                displayText: zoneName,
                equipmentZoneDatas: [],
                floorIndex: building.zoneDatas.length,
                id: ++SpaceDataManager.zoneID,
                textCenter: null,
                zoneName: zoneName
            };
        }
        else {
            zone = {
                id: ++SpaceDataManager.outdoorZoneID,
                datas: {},
                name: zoneName,
                displayText: zoneName
            }
        }

        return zone;
    }

    static getBuildingGroupFromBuilding(building, buildingGroupList) {
        let buildingGroup = null;

        for (let i = 0; i < buildingGroupList.length; i++) {
            if (buildingGroupList[i].id === building.buildingGroupID) {
                buildingGroup = buildingGroupList[i];
                break;
            }
        }

        return buildingGroup;
    }

    static getBuildingFromZone(zone, _3dOptions, buildingGroupList) {
        const buildingData = _3dOptions.buildingIDs[zone.buildingID];

        if (buildingData && buildingData.length >= 2) {
            const buildingGroupName = buildingData[1];
            const buildingGroupCount = buildingGroupList.length;

            for (let i = 0; i < buildingGroupCount; i++) {
                const buildingGroup = buildingGroupList[i];

                if (buildingGroup.groupName === buildingGroupName) {
                    const buildingCount = buildingGroup.buildingDatas.length;

                    for (let j = 0; j < buildingCount; j++) {
                        const building = buildingGroup.buildingDatas[j];

                        if (building.id === zone.buildingID) {
                            return building;
                        }
                    }
                }
            }
        }

        return null;
    }

    static addZoneToZones(zone, _3dOptions) {
        const zoneDataArray = [zone.floorIndex, zone.buildingID, zone.zoneName, zone.displayText, null, null, null];
        const zoneData = [...zoneDataArray];

        zoneData.datas = { ...zone.datas };
        zoneData.equipZones = {};
        zoneData.sensors = {};

        _3dOptions.zones[zone.id] = zoneData;
        return zoneDataArray;
    }

    static removeZoneFromZones(zone, _3dOptions) {
        delete _3dOptions.zones[zone.id];
    }

    static renameZoneToZones(zone, zoneName, _3dOptions) {
        const zoneData = _3dOptions.zones[zone.id];

        if (zoneData && zoneData.length >= 4) {
            zoneData[2] = zoneName;
            zoneData[3] = zoneName;
        }
    }

    static addZoneToAllBuildings(zoneID, zoneData, building, _3dOptions) {
        const buildingData = _3dOptions.allBuildings[building.buildingName];

        if (buildingData && buildingData.length >= 8) {
            const zoneDatas = buildingData[7];
            zoneDatas[zoneID] = zoneData;
        }
    }

    static removeZoneFromAllBuildings(zone, building, _3dOptions) {
        const buildingData = _3dOptions.allBuildings[building.buildingName];

        if (buildingData && buildingData.length >= 8) {
            const zoneDatas = buildingData[7];
            delete zoneDatas[zone.id];
        }
    }

    static renameZoneToAllBuildings(zone, zoneName, building, _3dOptions) {
        const buildingData = _3dOptions.allBuildings[building.buildingName];

        if (buildingData && buildingData.length >= 8) {
            const zoneDatas = buildingData[7];
            const zoneData = zoneDatas[zone.id];

            if (zoneData && zoneData.length >= 4) {
                zoneData[2] = zoneName;
                zoneData[3] = zoneName;
            }
        }
    }

    static addZoneToBuildingIDs(zoneID, zoneData, building, _3dOptions) {
        const buildingData = _3dOptions.buildingIDs[building.id];

        if (buildingData && buildingData.length >= 8) {
            const zoneDatas = buildingData[7];
            zoneDatas[zoneID] = zoneData;
        }
    }

    static removeZoneFromBuildingIDs(zone, building, _3dOptions) {
        const buildingData = _3dOptions.buildingIDs[building.id];

        if (buildingData && buildingData.length >= 8) {
            const zoneDatas = buildingData[7];
            delete zoneDatas[zone.id];
        }
    }

    static renameZoneToBuildingIDs(zone, zoneName, building, _3dOptions) {
        const buildingData = _3dOptions.buildingIDs[building.id];

        if (buildingData && buildingData.length >= 8) {
            const zoneDatas = buildingData[7];
            const zoneData = zoneDatas[zone.id];

            if (zoneData && zoneData.length >= 4) {
                zoneData[2] = zoneName;
                zoneData[3] = zoneName;
            }
        }
    }

    static addZoneToBuildings(zoneID, zoneData, building, _3dOptions, buildingGroupList) {
        const buildingGroup = SpaceDataManager.getBuildingGroupFromBuilding(building, buildingGroupList);

        if (buildingGroup) {
            const buildingGroupData = _3dOptions.buildings[buildingGroup.groupName];

            if (buildingGroupData) {
                const buildingData = buildingGroupData[building.buildingName];

                if (buildingData && buildingData.length >= 7) {
                    const zoneDatas = buildingData[6];
                    zoneDatas[zoneID] = zoneData;
                }
            }
        }
    }

    static removeZoneFromBuildings(zone, building, _3dOptions, buildingGroupList) {
        const buildingGroup = SpaceDataManager.getBuildingGroupFromBuilding(building, buildingGroupList);

        if (buildingGroup) {
            const buildingGroupData = _3dOptions.buildings[buildingGroup.groupName];

            if (buildingGroupData) {
                const buildingData = buildingGroupData[building.buildingName];

                if (buildingData && buildingData.length >= 7) {
                    const zoneDatas = buildingData[6];
                    delete zoneDatas[zone.id];
                }
            }
        }
    }

    static renameZoneToBuildings(zone, zoneName, building, _3dOptions, buildingGroupList) {
        const buildingGroup = SpaceDataManager.getBuildingGroupFromBuilding(building, buildingGroupList);

        if (buildingGroup) {
            const buildingGroupData = _3dOptions.buildings[buildingGroup.groupName];

            if (buildingGroupData) {
                const buildingData = buildingGroupData[building.buildingName];

                if (buildingData && buildingData.length >= 7) {
                    const zoneDatas = buildingData[6];
                    const zoneData = zoneDatas[zone.id];

                    if (zoneData && zoneData.length >= 4) {
                        zoneData[2] = zoneName;
                        zoneData[3] = zoneName;
                    }
                }
            }
        }
    }

    static removeZoneFromBuilding(zone, building) {
        const zoneCount = building.zoneDatas.length;

        for (let i = 0; i < zoneCount; i++) {
            if (zone.id === building.zoneDatas[i].id) {
                building.zoneDatas.splice(i, 1);
                break;
            }
        }
    }

    static addZone(building, zoneName, _3dOptions, buildingGroupList) {
        const zone = SpaceDataManager.makeZone(zoneName, building);

        if (building) {
            building.zoneDatas.push(zone);

            const zoneData = SpaceDataManager.addZoneToZones(zone, _3dOptions);

            SpaceDataManager.addZoneToAllBuildings(zone.id, zoneData, building, _3dOptions);
            SpaceDataManager.addZoneToBuildingIDs(zone.id, zoneData, building, _3dOptions);
            SpaceDataManager.addZoneToBuildings(zone.id, zoneData, building, _3dOptions, buildingGroupList);
        }
        else {
            _3dOptions.outdoorZones[zone.id] = zone;
        }
    }

    static removeZone(zone, buildingGroupList, _3dOptions) {
        SpaceDataManager.removeZoneFromZones(zone, _3dOptions);

        const building = SpaceDataManager.getBuildingFromZone(zone, _3dOptions, buildingGroupList);

        if (building) {
            SpaceDataManager.removeZoneFromAllBuildings(zone, building, _3dOptions);
            SpaceDataManager.removeZoneFromBuildingIDs(zone, building, _3dOptions);
            SpaceDataManager.removeZoneFromBuildings(zone, building, _3dOptions, buildingGroupList);
            SpaceDataManager.removeZoneFromBuilding(zone, building);
        }
        else {
            delete _3dOptions.outdoorZones[zone.id];
        }
    }

    static renameZone(zone, zoneName, buildingGroupList, _3dOptions) {
        SpaceDataManager.renameZoneToZones(zone, zoneName, _3dOptions);
        zone.zoneName = zone.broadcastText = zone.displayText = zoneName;

        const building = SpaceDataManager.getBuildingFromZone(zone, _3dOptions, buildingGroupList);

        if (building) {
            SpaceDataManager.renameZoneToAllBuildings(zone, zoneName, building, _3dOptions);
            SpaceDataManager.renameZoneToBuildingIDs(zone, zoneName, building, _3dOptions);
            SpaceDataManager.renameZoneToBuildings(zone, zoneName, building, _3dOptions, buildingGroupList);
        }
    }

    static makeBuilding(buildingName, buildingGroup) {
        const building = [++SpaceDataManager.buildingID, buildingGroup.groupName, buildingName, buildingName + SpaceDataManager.BoundingBoxTag, null, null, null, {}];
        return building;
    }

    static addBuildingToAllBuildings(buildingName, buildingData, _3dOptions) {
        _3dOptions.allBuildings[buildingName] = buildingData;
    }

    static renameBuildingToAllBuildings(buildingName, building, _3dOptions) {
        const buildingData = _3dOptions.allBuildings[building.buildingName];

        if (buildingData && buildingData.length >= 4) {
            buildingData[2] = buildingName;
            buildingData[3] = buildingName + SpaceDataManager.BoundingBoxTag;
        }

        if (buildingName !== building.buildingName) {
            _3dOptions.allBuildings[buildingName] = buildingData;
            delete _3dOptions.allBuildings[building.buildingName];
        }
    }

    static removeBuildingFromAllBuildings(building, _3dOptions) {
        delete _3dOptions.allBuildings[building.buildingName];
    }

    static addBuildingToBuildingIDs(buildingData, _3dOptions) {
        _3dOptions.buildingIDs[buildingData[0]] = buildingData;
    }

    static removeBuildingFromBuildingIDs(building, _3dOptions) {
        delete _3dOptions.buildingIDs[building.id];
    }

    static addBuildingToBuildings(buildingName, buildingData, buildingGroup, _3dOptions) {
        let buildings = _3dOptions.buildings[buildingGroup.groupName];

        if (!buildings) {
            buildings = {};
            _3dOptions.buildings[buildingGroup.groupName] = buildings;
        }

        buildings[buildingName] = [buildingData[0], buildingData[1], buildingData[3], buildingData[4], buildingData[5], buildingData[6], buildingData[7]];
    }

    static renameBuildingToBuildings(buildingName, building, buildingGroup, _3dOptions) {
        let buildings = _3dOptions.buildings[buildingGroup.groupName];

        if (!buildings) {
            buildings = {};
            _3dOptions.buildings[buildingGroup.groupName] = buildings;
        }

        const buildingData = buildings[building.buildingName];
        buildingData[1] = buildingName + SpaceDataManager.BoundingBoxTag;

        if (buildingName !== building.buildingName) {
            buildings[buildingName] = buildingData;
            delete buildings[building.buildingName];
        }
    }

    static removeBuildingFromBuildings(building, buildingGroup, _3dOptions) {
        let buildings = _3dOptions.buildings[buildingGroup.groupName];

        if (!buildings) {
            return;
        }

        delete buildings[building.buildingName];
    }

    static addBuildingToBuildingGroupList(buildingName, buildingData, buildingGroup, buildingGroupList) {
        const buildingGroupCount = buildingGroupList.length;

        for (let i = 0; i < buildingGroupCount; i++) {
            const bg = buildingGroupList[i];

            if (bg.id === buildingGroup.id) {
                const buildingData2 = {
                    broadcastText: buildingName,
                    buildingCode: buildingName,
                    buildingGroupID: buildingGroup.id,
                    buildingName: buildingName,
                    displayText: buildingName,
                    id: buildingData[0],
                    maxFloor: 0,
                    minFloor: 0,
                    //textCenter: {
                    //    x: null,
                    //    y: null,
                    //    z: null
                    //},
                    textCenter: null,
                    zoneDatas: []
                }

                bg.buildingDatas.push(buildingData2);
                break;
            }
        }
    }

    static renameBuildingToBuildingGroupList(buildingName, building, buildingGroup, buildingGroupList) {
        const buildingGroupCount = buildingGroupList.length;

        for (let i = 0; i < buildingGroupCount; i++) {
            const bg = buildingGroupList[i];

            if (bg.id === buildingGroup.id) {
                const buildingCount = bg.buildingDatas.length;

                for (let j = 0; j < buildingCount; j++) {
                    const buildingData = bg.buildingDatas[j];

                    if (buildingData.id === building.id) {
                        buildingData.broadcastText = buildingName;
                        buildingData.buildingCode = buildingName;
                        buildingData.buildingName = buildingName;
                        buildingData.displayText = buildingName;

                        break;
                    }
                }
                
                break;
            }
        }
    }

    static removeBuildingFromBuildingGroupList(building, buildingGroup, buildingGroupList) {
        const buildingGroupCount = buildingGroupList.length;

        for (let i = 0; i < buildingGroupCount; i++) {
            const bg = buildingGroupList[i];

            if (bg.id === buildingGroup.id) {
                const buildingCount = bg.buildingDatas.length;

                for (let j = 0; j < buildingCount; j++) {
                    const buildingData = bg.buildingDatas[j];

                    if (buildingData.id === building.id) {
                        bg.buildingDatas.splice(j, 1);
                        break;
                    }
                }

                break;
            }
        }
    }

    static addBuilding(buildingGroup, newName, _3dOptions, buildingGroupList) {
        const buildingData = SpaceDataManager.makeBuilding(newName, buildingGroup);

        SpaceDataManager.addBuildingToAllBuildings(newName, buildingData, _3dOptions);
        SpaceDataManager.addBuildingToBuildingIDs(buildingData, _3dOptions);
        SpaceDataManager.addBuildingToBuildings(newName, buildingData, buildingGroup, _3dOptions);
        SpaceDataManager.addBuildingToBuildingGroupList(newName, buildingData, buildingGroup, buildingGroupList);
    }

    static renameBuilding(building, newName, buildingGroupList, _3dOptions) {
        const buildingGroup = SpaceDataManager.getBuildingGroupFromBuilding(building, buildingGroupList);

        SpaceDataManager.renameBuildingToAllBuildings(newName, building, _3dOptions);
        SpaceDataManager.renameBuildingToBuildings(newName, building, buildingGroup, _3dOptions);
        SpaceDataManager.renameBuildingToBuildingGroupList(newName, building, buildingGroup, buildingGroupList);
    }

    static removeBuilding(building, buildingGroupList, _3dOptions) {
        SpaceDataManager.removeBuildingFromAllBuildings(building, _3dOptions);
        SpaceDataManager.removeBuildingFromBuildingIDs(building, _3dOptions);

        const buildingGroup = SpaceDataManager.getBuildingGroupFromBuilding(building, buildingGroupList);
        SpaceDataManager.removeBuildingFromBuildings(building, buildingGroup, _3dOptions);
        SpaceDataManager.removeBuildingFromBuildingGroupList(building, buildingGroup, buildingGroupList);
    }

    static removeBuildingGroupFromBuildingGroups(buildingGroup, _3dOptions) {
        const buildingGroupCount = _3dOptions.buildingGroups.length;

        for (let i = 0; i < buildingGroupCount; i++) {
            const buildingGroupData = _3dOptions.buildingGroups[i];

            if (buildingGroupData.length >= 7 && buildingGroupData[6] === buildingGroup.id) {
                _3dOptions.buildingGroups.splice(i, 1);
                break;
            }
        }
    }

    static renameBuildingGroupToBuildingGroups(buildingGroup, buildingGroupName, _3dOptions) {
        const buildingGroupCount = _3dOptions.buildingGroups.length;

        for (let i = 0; i < buildingGroupCount; i++) {
            const buildingGroupData = _3dOptions.buildingGroups[i];

            if (buildingGroupData.length >= 7 && buildingGroupData[6] === buildingGroup.id) {
                buildingGroupData[0] = buildingGroupName;
                buildingGroupData[1] = buildingGroupName;
                buildingGroupData[2] = buildingGroupName + SpaceDataManager.BoundingBoxTag;
                break;
            }
        }
    }

    static removeBuildingGroupFromAllBuildingsNBuildingIDs(buildingGroup, _3dOptions, buildingGroupList) {
        const bgCount = buildingGroupList.length;

        for (let i = 0; i < bgCount; i++) {
            const _buildingGroup = buildingGroupList[i];

            if (_buildingGroup.id === buildingGroup.id) {
                const buildingCount = _buildingGroup.buildingDatas.length;

                for (let j = 0; j < buildingCount; j++) {
                    const buildingData = _buildingGroup.buildingDatas[j];
                    delete _3dOptions.allBuildings[buildingData.buildingName];
                    delete _3dOptions.buildingIDs[buildingData.id];
                }

                buildingGroupList.splice(i, 1);
                break;
            }
        }
    }

    static renameBuildingGroupToAllBuildingsNBuildingIDs(buildingGroup, buildingGroupName, _3dOptions, buildingGroupList) {
        const bgCount = buildingGroupList.length;
        const buildingIDs = {};

        for (let i = 0; i < bgCount; i++) {
            const _buildingGroup = buildingGroupList[i];

            if (_buildingGroup.id === buildingGroup.id) {
                const buildingCount = _buildingGroup.buildingDatas.length;

                for (let j = 0; j < buildingCount; j++) {
                    const buildingData = _buildingGroup.buildingDatas[j];
                    buildingIDs[buildingData.id] = buildingData.id;
                }

                break;
            }
        }

        for (const buildingName in _3dOptions.allBuildings) {
            const buildingData = _3dOptions.allBuildings[buildingName];

            if (buildingData.length >= 2) {
                const buildingID = buildingIDs[buildingData[0]];

                if (buildingID !== null && buildingID !== undefined) {
                    buildingData[1] = buildingGroupName;
                }
            }
        }

        for (const buildingID in buildingIDs) {
            const buildingData = _3dOptions.buildingIDs[buildingID];

            if (buildingData.length >= 2) {
                const buildingID = buildingIDs[buildingData[0]];

                if (buildingID !== null && buildingID !== undefined) {
                    buildingData[1] = buildingGroupName;
                }
            }
        }
    }

    static removeBuildingGroupFromBuildings(buildingGroup, _3dOptions) {
        delete _3dOptions.buildings[buildingGroup.groupName];
    }

    static renameBuildingGroupToBuildings(buildingGroup, buildingGroupName, _3dOptions) {
        const buildings = _3dOptions.buildings[buildingGroup.groupName];

        if (buildings) {
            for (const buildingName in buildings) {
                const buildingData = buildings[buildingName];

                if (buildingData.length >= 2) {
                    buildingData[1] = buildingGroupName;
                }
            }

            if (buildingGroup.groupName !== buildingGroupName) {
                delete _3dOptions.buildings[buildingGroup.groupName];
                _3dOptions.buildings[buildingGroupName] = buildings;
            }
        }
    }

    static removeBuildingGroupFromZones(buildingGroup, _3dOptions) {
        const buildingCount = buildingGroup.buildingDatas.length;

        for (let i = 0; i < buildingCount; i++) {
            const buildingData = buildingGroup.buildingDatas[i];
            const zoneCount = buildingData.zoneDatas.length;

            for (let j = 0; j < zoneCount; j++) {
                const zone = buildingData.zoneDatas[j];
                delete _3dOptions.zones[zone.id];
            }
        }
    }

    static addBuildingGroup(buildingGroup, _3dOptions) {
        const buildings = {};
        const zones = {};
        const bgData = SpaceDataManager.getBuildingGroupData(buildings, buildingGroup, zones);

        _3dOptions.buildingGroups.push(bgData);
        SpaceDataManager.add3DOptions(_3dOptions, null, buildings, zones);
    }

    static renameBuildingGroup(buildingGroup, newName, buildingGroupList, _3dOptions) {
        SpaceDataManager.renameBuildingGroupToBuildingGroups(buildingGroup, newName, _3dOptions);
        SpaceDataManager.renameBuildingGroupToAllBuildingsNBuildingIDs(buildingGroup, newName, _3dOptions, buildingGroupList);
        SpaceDataManager.renameBuildingGroupToBuildings(buildingGroup, newName, _3dOptions);

        buildingGroup.groupName = buildingGroup.displayText = newName;
    }

    static getHiddenBuildingGroup(buildingGroupList) {
        const buildingGroupCount = buildingGroupList.length;

        for (let i = 0; i < buildingGroupCount; i++) {
            const buildingGroup = buildingGroupList[i];

            if (buildingGroup.visible === false) {
                return buildingGroup;
            }
        }

        return null;
    }

    static clearBuildingGroup(buildingGroupList, _3dOptions) {
        const buildingGroupCount = buildingGroupList.length;

        for (let i = buildingGroupCount - 1; i >= 0; i--) {
            const buildingGroup = buildingGroupList[i];

            if (buildingGroup.visible) {
                SpaceDataManager.removeBuildingGroup(buildingGroup, buildingGroupList, _3dOptions);
            }
        }
    }

    static removeBuildingGroup(buildingGroup, buildingGroupList, _3dOptions) {
        SpaceDataManager.removeBuildingGroupFromZones(buildingGroup, _3dOptions);
        SpaceDataManager.removeBuildingGroupFromBuildingGroups(buildingGroup, _3dOptions);
        SpaceDataManager.removeBuildingGroupFromAllBuildingsNBuildingIDs(buildingGroup, _3dOptions, buildingGroupList);
        SpaceDataManager.removeBuildingGroupFromBuildings(buildingGroup, _3dOptions);
    }

    static add3DOptions(_3dOptions, outdoorZones, buildings, zones) {
        const allBuildings = _3dOptions.allBuildings;
        const buildingIDs = _3dOptions.buildingIDs;

        if (buildings) {
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
        }

        const outdoorZoneCount = outdoorZones ? outdoorZones.length : 0;

        for (let i = 0; i < outdoorZoneCount; i++) {
            const zone = outdoorZones[i];
            const zoneData = {};

            zoneData.name = zone.zoneName;
            zoneData.id = zone.id;
            zoneData.sensors = {};
            zoneData.datas = zone.datas;

            _3dOptions.outdoorZones[zone.id.toString()] = zoneData;
        }

        for (const buildingGroupName in buildings) {
            _3dOptions.buildings[buildingGroupName] = buildings[buildingGroupName];
        }

        for (const zoneID in zones) {
            _3dOptions.zones[zoneID] = zones[zoneID];
        }
    }

    static async get3DOptions(buildingGroupList, outdoorZones, gltfModels, gltfOption) {
        if (!buildingGroupList) {
            return [];
        }
        else {
            // 건물그룹없이 존재하는 건물들을 위한 감춰진 빌딩그룹
            if (buildingGroupList.length === 0) {
                const hiddenBuildingGroup = SpaceDataManager.makeBuildingGroup("hidden", null);
                hiddenBuildingGroup.visible = false;
                buildingGroupList.push(hiddenBuildingGroup);
            }

            const buildingGroups = [];
            const buildings = {};
            const zones = {};
            const buildingGroupCount = buildingGroupList ? buildingGroupList.length : 0;

            for (let i = 0; i < buildingGroupCount; i++) {
                const buildingGroup = buildingGroupList[i];
                const bgData = SpaceDataManager.getBuildingGroupData(buildings, buildingGroup, zones);

                if (bgData === null) {
                    continue;
                }
                /*buildings[buildingGroup.groupName] = SpaceDataManager.getBuildings(buildingGroup.buildingDatas, zones);

                if (!buildingGroup.textCenter) {
                    continue;
                }

                const bgData = [];

                bgData.push(buildingGroup.groupName);
                bgData.push(buildingGroup.displayText);
                bgData.push(buildingGroup.groupName + SpaceDataManager.BoundingBoxTag);
                bgData.push(buildingGroup.textCenter.x);
                bgData.push(buildingGroup.textCenter.y);
                bgData.push(buildingGroup.textCenter.z);
                bgData.push(buildingGroup.id);*/

                buildingGroups.push(bgData);
            }

            if (!gltfModels) {
                const [models, option, message] = await SpaceController.requestGltfModelList();

                if (!models && message && message.length > 0) {
                    alert(message);
                    return {};
                }
                else {
                    gltfModels = models;
                }
            }

            if (gltfModels) {
                const _3DOptions = this.make3DOptions(buildingGroups, outdoorZones, buildings, zones, gltfOption, gltfModels);

                if (_3DOptions)
                    return _3DOptions;
            }
        }

        return {};
    }

    static getBuildingGroupData(buildings, buildingGroup, zones) {
        buildings[buildingGroup.groupName] = SpaceDataManager.getBuildings(buildingGroup.buildingDatas, zones);

        const bgData = [];

        bgData.push(buildingGroup.groupName);
        bgData.push(buildingGroup.displayText);
        bgData.push(buildingGroup.groupName + SpaceDataManager.BoundingBoxTag);

        if (buildingGroup.textCenter) {
            bgData.push(buildingGroup.textCenter.x);
            bgData.push(buildingGroup.textCenter.y);
            bgData.push(buildingGroup.textCenter.z);
        }
        else {
            bgData.push(null);
            bgData.push(null);
            bgData.push(null);
        }

        bgData.push(buildingGroup.id);

        return bgData;
    }

    static getBuildings(buildingDatas, zones) {
        const buildings = {};
        const buildingCount = buildingDatas.length;

        for (let i = 0; i < buildingCount; i++) {
            const building = buildingDatas[i];
            const buildingData = [];

            buildingData.push(building.id);
            buildingData.push(building.displayText);
            buildingData.push(building.buildingName + SpaceDataManager.BoundingBoxTag);

            if (building.textCenter) {
                buildingData.push(building.textCenter.x);
                buildingData.push(building.textCenter.y);
                buildingData.push(building.textCenter.z);
            }
            else {
                buildingData.push(null);
                buildingData.push(null);
                buildingData.push(null);
            }

            const buildingZones = {};
            const equipZoneDatas = {};
            SpaceDataManager.getZones(building.zoneDatas, buildingZones, equipZoneDatas);
            buildingData.push(buildingZones);
            buildings[building.buildingName] = buildingData;

            for (const zoneID in buildingZones) {
                const zone = [...buildingZones[zoneID]];
                zone.sensors = {};
                zones[zoneID] = zone;
                zone.equipZones = {};
                zone.datas = SpaceDataManager.getZoneDatas(parseInt(zoneID), building.zoneDatas);

                const equipmentZoneDatas = equipZoneDatas[parseInt(zoneID)];

                if (equipmentZoneDatas) {
                    const equipZoneCount = equipmentZoneDatas.length;

                    for (let j = 0; j < equipZoneCount; j++) {
                        const equipmentZoneData = equipmentZoneDatas[j];
                        const equipZoneData = [];

                        equipZoneData.push(equipmentZoneData.id);
                        equipZoneData.push(equipmentZoneData.zoneName);
                        equipZoneData.push(equipmentZoneData.textCenter);

                        zone.equipZones[equipmentZoneData.id] = equipZoneData;
                    }
                }
            }
        }

        return buildings;
    }

    static getZoneDatas(zoneID, zoneDatas) {
        const dataCount = zoneDatas.length;

        for (let i = 0; i < dataCount; i++) {
            const zoneData = zoneDatas[i];

            if (zoneData.id === zoneID) {
                return zoneData.datas;
            }
        }

        return {};
    }

    static getZones(zoneDatas, zones, equipZoneDatas) {
        zoneDatas.sort((zone1, zone2) => {
            const floor1 = SpaceDataManager.getZoneFloor(zone1);
            const floor2 = SpaceDataManager.getZoneFloor(zone2);
            return floor1 - floor2;
        });

        const zoneCount = zoneDatas.length;

        for (let i = 0; i < zoneCount; i++) {
            const zone = zoneDatas[i];

            if (zone.textCenter === null) {
                zones[zone.id] = [SpaceDataManager.getZoneFloor(zone), zone.buildingID, zone.zoneName, zone.displayText, null, null, null];
            }
            else {
                zones[zone.id] = [SpaceDataManager.getZoneFloor(zone), zone.buildingID, zone.zoneName, zone.displayText, zone.textCenter.x, zone.textCenter.y, zone.textCenter.z];
            }

            equipZoneDatas[zone.id] = zone.equipmentZoneDatas;
        }
    }

    static getZoneFloor(zone) {
        if (zone.addFloor === null)
            return zone.floorIndex;

        return zone.floorIndex + zone.addFloor;
    }

    static make3DOptions(buildingGroups, outdoorZones, buildings, zones, option, models) {
        const _3DOptions = {
            siteName: null
        };
        const modelCount = models.length;

        for (let i = 0; i < modelCount; i++) {
            const model = models[i];
            const json = SpaceDataManager.addModel(model);

            _3DOptions[model.modelName] = json;
            _3DOptions[model.modelName].id = model.id;
            /*if (SpaceDataManager.isEmpty(json) === false) {
                _3DOptions[model.modelName] = json;
            }*/
        }

        const allBuildings = {};
        const buildingIDs = {};

        if (buildings) {
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
        }

        const _outdoorZones = {};
        const outdoorZoneCount = outdoorZones ? outdoorZones.length : 0;

        for (let i = 0; i < outdoorZoneCount; i++) {
            const zone = outdoorZones[i];
            const zoneData = {};

            zoneData.name = zone.zoneName;
            zoneData.id = zone.id;
            zoneData.sensors = (!zone.sensors) ? {} : zone.sensors;
            zoneData.datas = zone.datas;

            zoneData.addFloor = zone.addFloor;
            zoneData.boundary = zone.boundary;
            zoneData.broadcastText = zone.broadcastText;
            zoneData.displayText = zone.displayText;
            zoneData.equipmentZoneDatas = zone.equipmentZoneDatas;
            zoneData.floorIndex = zone.floorIndex;
            zoneData.textCenter = zone.textCenter;

            _outdoorZones[zone.id.toString()] = zoneData;
        }

        if (SpaceDataManager.isEmpty(_outdoorZones)) {
            const outdoorZone = SpaceDataManager.makeZone("외부영역", null);
            _outdoorZones[outdoorZone.id] = outdoorZone;
        }

        _3DOptions.buildingGroups = buildingGroups;
        _3DOptions.buildings = buildings;
        _3DOptions.allBuildings = allBuildings;
        _3DOptions.buildingIDs = buildingIDs;
        _3DOptions.zones = zones;
        _3DOptions.outdoorZones = _outdoorZones;
        
        if (option) {
            _3DOptions.indoorModelOnMemory = option.indoorModelOnMemory;
            _3DOptions.modelBaseURL = option._3DModelBaseURL;
            _3DOptions.textureBaseURL = option._3DTextureBaseURL;
            _3DOptions.backgroundImage = option._3DBackgroundImage;
        }

        return _3DOptions;
    }

    static getSensor(sensorType, zoneID, sensorID, _3dOptions) {
        let zoneData = _3dOptions.zones[zoneID];

        if (!zoneData) {
            zoneData = _3dOptions.outdoorZones[zoneID];

            if (!zoneData)
                return null;
        }

        if (!zoneData.sensors)
            return null;

        const sensors = zoneData.sensors[sensorType];

        if (sensors) {
            const sensorCount = sensors.length;

            for (let i = 0; i < sensorCount; i++) {
                const sensor = sensors[i];

                if (sensor.id === sensorID)
                    return sensor;
            }
        }

        return null;
    }

    static addModel(model) {
        const data = {};
        
        const childModelCount = model.childModels.length;

        for (let i = 0; i < childModelCount; i++) {
            const childModel = model.childModels[i];
            const json = SpaceDataManager.addModel(childModel);

            data[childModel.modelName] = json;
            /*if (this.isEmpty(json) === false) {
                data[childModel.modelName] = json;
            }*/
        }

        const modelDataCount = model.modelDatas.length;
        const floors = [];

        for (let i = 0; i < modelDataCount; i++) {
            const modelData = model.modelDatas[i];
            const modelOrthoData = SpaceDataManager.getOrthoDataModel(modelData.modelFile, model.modelOrthoDatas);

            if (modelData.floorIndex !== null && modelData.floorIndex !== undefined) {
                const floor = {};

                floor.file = modelData.modelFile;
                floor.camera = SpaceDataManager.getCameraData(modelData);
                floor.modelDisplayText = modelData.modelDisplayText;
                floor.floorIndex = modelData.floorIndex;

                if (modelOrthoData) {
                    floor.cameraOrtho = SpaceDataManager.getCameraOrthoData(modelOrthoData);
                }

                if (modelData.buildingGroupID) {
                    floor.buildingGroupID = modelData.buildingGroupID;
                }

                if (modelData.buildingID) {
                    floor.buildingID = modelData.buildingID;
                }

                if (modelData.zoneID) {
                    floor.zoneID = modelData.zoneID;
                }

                floors.push(floor);
            }
            else {
                data.file = modelData.modelFile;
                data.camera = SpaceDataManager.getCameraData(modelData);
                data.modelDisplayText = modelData.modelDisplayText;
                
                if (modelOrthoData) {
                    data.cameraOrtho = SpaceDataManager.getCameraOrthoData(modelOrthoData);
                }

                if (modelData.buildingGroupID) {
                    data.buildingGroupID = modelData.buildingGroupID;
                }

                if (modelData.buildingID) {
                    data.buildingID = modelData.buildingID;
                }

                if (modelData.zoneID) {
                    data.zoneID = modelData.zoneID;
                }
            }
        }

        if (floors.length > 0) {
            data.floors = floors;
        }

        return data;
    }

    static getOrthoDataModel(modelFileName, modelOrthoDatas) {
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

    static isEmpty(json) {
        for (const key in json) {
            return false;
        }

        return true;
    }

    static getCameraData(modelData) {
        const data = {};

        data.id = modelData.id;
        data.position = SpaceDataManager.getVector3(modelData.cameraPosition);
        data.quaternion = SpaceDataManager.getVector3(modelData.cameraQuaternion);
        data.quaternion.push(modelData.cameraQuaternion.w);
        data.rotation = SpaceDataManager.getVector3(modelData.cameraRotation);
        data.targetControl = SpaceDataManager.getVector3(modelData.orbitTarget);
        data.fov = modelData.cameraFov;
        data.near = modelData.cameraNear;
        data.far = modelData.cameraFar;

        return data;
    }

    static getCameraOrthoData(modelOrthoData) {
        const data = {};

        data.id = modelOrthoData.id;
        data.position = SpaceDataManager.getVector3(modelOrthoData.cameraPosition);
        data.quaternion = SpaceDataManager.getVector3(modelOrthoData.cameraQuaternion);
        data.quaternion.push(modelOrthoData.cameraQuaternion.w);
        data.rotation = SpaceDataManager.getVector3(modelOrthoData.cameraRotation);
        data.targetControl = SpaceDataManager.getVector3(modelOrthoData.target);
        data.zoom = modelOrthoData.zoom;

        return data;
    }

    static getVector3(vector) {
        const data = [];

        data.push(vector.x);
        data.push(vector.y);
        data.push(vector.z);

        return data;
    }

    static getZoneModelData(_3dOptions, zoneID) {
        if (!_3dOptions.indoorModels) {
            return null;
        }

        const zone = _3dOptions.zones[zoneID.toString()];

        if (!zone) {
            return null;
        }

        const buildingID = zone[1];
        const building = _3dOptions.buildingIDs[buildingID.toString()];

        if (!building) {
            return null;
        }

        const buildingGroupName = building[1];
        //const buildingName = building[2];

        const buildingGroup = _3dOptions.indoorModels[buildingGroupName];

        if (!buildingGroup) {
            return null;
        }

        const buildingData = SpaceDataManager.getBuildingDataFromID(building[0], buildingGroup);
        //const buildingData = buildingGroup[buildingName];

        if (!buildingData || !buildingData.floors) {
            return null;
        }

        const floorCount = buildingData.floors.length;

        for (let i = 0; i < floorCount; i++) {
            const floor = buildingData.floors[i];

            if (floor.zoneID.toString() === zoneID.toString()) {
                return floor;
            }
        }

        return null;
    }

    static getBuildingDataFromID(buildingID, buildingGroup) {
        for (const buildingName in buildingGroup) {
            const buildingData = buildingGroup[buildingName];

            if (buildingData) {
                if (Array.isArray(buildingData)) {
                    for (const building of buildingData) {
                        if (building.id.toString() === buildingID.toString()) {
                            return building;
                        }
                    }
                }
                else if (SpaceDataManager.isBuildingModelData(buildingData)) {
                    if (buildingData.buildingID.toString() === buildingID.toString()) {
                        return buildingData;
                    }
                }
            }
        }
        
        return null;
    }

    static addZoneSensors(sensors, sensorType, zone) {
        if (!zone.sensors) {
            zone.sensors = {};
        }

        zone.sensors[sensorType] = sensors;
    }

    static addEquipZoneSensors(sensors, sensorType, equipZone) {
        if (!equipZone.sensors) {
            equipZone.sensors = {};
        }

        equipZone.sensors[sensorType] = sensors;
    }

    static findZone(zoneID, _3dOptions) {
        const zone = _3dOptions.zones[zoneID];

        if (zone) {
            return zone;
        }

        return _3dOptions.outdoorZones[zoneID];
    }

    static findEquipZone(equipZoneID, zoneID, _3dOptions) {
        const zone = SpaceDataManager.findZone(zoneID, _3dOptions);

        if (zone) {
            if (zone.quipZones) {
                return zone.equipZones[equipZoneID];
            }
        }

        return null;
    }

    static setModelFiles(modelFiles, _3dOptions, buildingGroupList) {
        const siteModelFileName = modelFiles.site;

        if (siteModelFileName && siteModelFileName.length > 0) {
            _3dOptions.outdoorModel = SpaceDataManager.makeModelData(siteModelFileName);
        }

        const indoorModels = {};
        const buildingGroupNames = {};
        const buildingGroups = modelFiles.buildingGroups;

        _3dOptions.indoorModels = indoorModels;

        if (buildingGroups) {
            for (const buildingGroupID in buildingGroups) {
                const fileName = buildingGroups[buildingGroupID];
                const buildingGroup = SpaceDataManager.getBuildingGroup(buildingGroupID, buildingGroupList);

                if (buildingGroup) {
                    indoorModels[buildingGroup.groupName] = SpaceDataManager.makeBuildingGroupModelData(fileName, buildingGroup);
                    buildingGroupNames[buildingGroup.groupName] = buildingGroup;
                }
            }
        }

        const zoneModels = {};
        const buildings = modelFiles.buildings;

        if (buildings) {
            for (const buildingID in buildings) {
                const fileName = buildings[buildingID];
                const buildingData = _3dOptions.buildingIDs[buildingID];

                if (buildingData && buildingData.length >= 2) {
                    const buildingGroupName = buildingData[1];
                    let buildingGroup = buildingGroupNames[buildingGroupName];

                    if (!buildingGroup) {
                        buildingGroup = SpaceDataManager.getBuildingGroupFromName(buildingGroupName, buildingGroupList);

                        if (buildingGroup) {
                            indoorModels[buildingGroupName] = SpaceDataManager.makeBuildingGroupModelData("", buildingGroup);
                            buildingGroupNames[buildingGroupName] = buildingGroup;
                        }
                    }

                    if (buildingGroup) {
                        const buildingData = SpaceDataManager.getBuildingDataFromID(buildingID, buildingGroup);

                        if (buildingData) {
                            const buildingGroupModel = indoorModels[buildingGroup.groupName];

                            if (buildingGroupModel) {
                                const buildingModelData = SpaceDataManager.makeBuildingModelData(fileName, buildingData);
                                buildingGroupModel[buildingData.buildingName] = buildingModelData;

                                for (const floor of buildingModelData.floors) {
                                    zoneModels[floor.zoneID] = floor;
                                }
                            }
                        }
                    }
                }
            }
        }

        const zones = modelFiles.zones;

        if (zones) {
            for (const zoneID in zones) {
                const fileName = zones[zoneID];
                let zoneModel = zoneModels[zoneID];

                if (!zoneModel) {
                    const zoneData = SpaceDataManager.findZone(zoneID, _3dOptions);

                    if (zoneData && zoneData.length >= 2) {
                        const buildingID = zoneData[1];
                        const buildingData = _3dOptions.buildingIDs[buildingID];

                        if (buildingData && buildingData.length >= 2) {
                            const buildingGroupName = buildingData[1];
                            let buildingGroup = buildingGroupNames[buildingGroupName];

                            if (!buildingGroup) {
                                buildingGroup = SpaceDataManager.getBuildingGroupFromName(buildingGroupName, buildingGroupList);

                                if (buildingGroup) {
                                    indoorModels[buildingGroupName] = SpaceDataManager.makeBuildingGroupModelData("", buildingGroup);
                                    buildingGroupNames[buildingGroupName] = buildingGroup;

                                    const buildingGroupModel = indoorModels[buildingGroupName];

                                    if (buildingGroupModel) {
                                        const _buildingData = SpaceDataManager.getBuildingDataFromID(buildingID, buildingGroup);

                                        if (_buildingData) {
                                            const buildingModelData = SpaceDataManager.makeBuildingModelData("", _buildingData);
                                            buildingGroupModel[_buildingData.buildingName] = buildingModelData;

                                            for (const floor of buildingModelData.floors) {
                                                zoneModels[floor.zoneID] = floor;

                                                if (floor.zoneID.toString() === zoneID.toString()) {
                                                    zoneModel = floor;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (zoneModel) {
                    zoneModel.file = fileName;
                }
            }
        }
    }

    static makeModelData(fileName) {
        const modelData = {
            camera: SpaceDataManager.makeDefaultCamera(),
            cameraOrtho: null,
            file: fileName,
            modelDisplayText: null
        };

        return modelData;
    }

    static makeBuildingGroupModelData(fileName, buildingGroup) {
        const modelData = {
            buildingGroupID: buildingGroup.id,
            camera: SpaceDataManager.makeDefaultCamera(),
            file: fileName,
            modelDisplayText: buildingGroup.displayText
        };

        return modelData;
    }

    static makeBuildingModelData(fileName, buildingData) {
        const modelData = {
            buildingID: buildingData.id,
            camera: SpaceDataManager.makeDefaultCamera(),
            file: fileName,
            floors: SpaceDataManager.zoneDatasToFloors(buildingData.zoneDatas),
            modelDisplayText: buildingData.displayText
        };

        return modelData;
    }

    static makeDefaultCamera() {
        return {
            far: 5000,
            fov: 60,
            near: 0.1,
            position: [0, 0, 0],
            quaternion: [0, 0, 0, 0],
            rotation: [0, 0, 0],
            targetControl: [0, 0, 0]
        };
    }

    static zoneDatasToFloors(zoneDatas) {
        const floors = [];

        for (const zoneData of zoneDatas) {
            const floorData = SpaceDataManager.makeModelData(null);
            floorData.floorIndex = zoneData.floorIndex;
            floorData.zoneID = zoneData.id;
            floorData.modelDisplayText = zoneData.displayText;

            floors.push(floorData);
        }

        return floors;
    }

    static getBuildingGroup(id, buildingGroupList) {
        for (const buildingGroup of buildingGroupList) {
            if (buildingGroup.id.toString() === id.toString()) {
                return buildingGroup;
            }
        }

        return null;
    }

    static getBuildingGroupFromName(buildingGroupName, buildingGroupList) {
        for (const buildingGroup of buildingGroupList) {
            if (buildingGroup.groupName === buildingGroupName) {
                return buildingGroup;
            }
        }

        return null;
    }

    static getModelFileNames(_3dOptions) {
        const fileNames = [];

        if (!_3dOptions) {
            return fileNames;
        }

        const outdoorModelFile = _3dOptions.outdoorModel?.file;

        if (outdoorModelFile)
            fileNames.push(outdoorModelFile);

        const indoorModels = _3dOptions.indoorModels;

        if (indoorModels) {
            for (const buildingGroupName in indoorModels) {
                const buildingGroupModelData = indoorModels[buildingGroupName];

                if (buildingGroupModelData && buildingGroupModelData.file)
                    fileNames.push(buildingGroupModelData.file);

                if (buildingGroupModelData) {
                    for (const buildingName in buildingGroupModelData) {
                        const buildingModelData = buildingGroupModelData[buildingName];

                        if (SpaceDataManager.isBuildingModelData(buildingModelData)) {
                            if (buildingModelData.file)
                                fileNames.push(buildingModelData.file);

                            const floors = buildingModelData.floors;

                            if (floors) {
                                for (const zoneModelData of floors) {
                                    if (zoneModelData.file)
                                        fileNames.push(zoneModelData.file);
                                }
                            }
                        }
                    }
                }
            }
        }

        return fileNames;
    }

    static getBuildingGroupModelData(buildingGroupID, indoorModels) {
        for (const buildingGroupName in indoorModels) {
            const buildingGroupModelData = indoorModels[buildingGroupName];

            if (buildingGroupModelData.buildingGroupID !== undefined && buildingGroupModelData.buildingGroupID !== null && buildingGroupModelData.buildingGroupID.toString() === buildingGroupID.toString()) {
                return buildingGroupModelData;
            }
        }

        return null;
    }

    static getBuildingModelData(buildingID, indoorModels, _3dOptions) {
        if (!indoorModels) {
            return null;
        }

        const buildingData = _3dOptions.buildingIDs[buildingID];

        if (buildingData && buildingData.length >= 2) {
            const buildingGroupName = buildingData[1];
            const buildingGroupModelData = indoorModels[buildingGroupName];

            if (buildingGroupModelData) {
                for (const buildingName in buildingGroupModelData) {
                    const buildingModelData = buildingGroupModelData[buildingName];

                    if (SpaceDataManager.isBuildingModelData(buildingModelData)) {
                        if (buildingModelData.buildingID.toString() === buildingID.toString()) {
                            return buildingModelData;
                        }
                    }
                }
            }
        }

        return null;
    }

    static isBuildingGroupModelData(data) {
        if (data && (data instanceof Object)) {
            if (data.buildingGroupID !== null && data.buildingGroupID !== undefined) {
                return true
            }
        }

        return false;
    }

    static isBuildingModelData(data) {
        if (data && (data instanceof Object)) {
            if (data.buildingID !== null && data.buildingID !== undefined) {
                return true
            }
        }

        return false;
    }

    static initIDsFromXMLData(data) {
        let maxBuildingGroupID = 1;
        let maxBuildingID = 1;
        let maxZoneID = 1;
        let maxEquipZoneID = 1;

        for (const buildingGroup of data.buildingGroups) {
            if (buildingGroup.id > maxBuildingGroupID) {
                maxBuildingGroupID = buildingGroup.id;
            }

            const buildingDatas = buildingGroup.buildingDatas;

            if (buildingDatas) {
                for (const buildingData of buildingDatas) {
                    if (buildingData.id > maxBuildingID) {
                        maxBuildingID = buildingData.id;
                    }

                    const zoneDatas = buildingData.zoneDatas;

                    if (zoneDatas) {
                        for (const zoneData of zoneDatas) {
                            if (zoneData.id > maxZoneID) {
                                maxZoneID = zoneData.id;
                            }

                            const equipZoneDatas = zoneData.equipmentZoneDatas;

                            if (equipZoneDatas) {
                                for (const equipZoneData of equipZoneDatas) {
                                    if (equipZoneData.id > maxEquipZoneID) {
                                        maxEquipZoneID = equipZoneData.id;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        SpaceDataManager.buildingGroupID = maxBuildingGroupID;
        SpaceDataManager.buildingID = maxBuildingID;
        SpaceDataManager.zoneID = maxZoneID;
        SpaceDataManager.equipZoneID = maxEquipZoneID;

        let maxFireSensorID = 1;
        let maxPsmSensorID = 1;
        let maxEtcSensorID = 1;
        let maxCctvID = 1;

        const fireSensors = data.fireSensors;
        const psmSensors = data.psmSensors;
        const etcSensors = data.etcSensors;
        const cctvs = data.cctvs;

        if (fireSensors) {
            const sensorCount = fireSensors.length;
            for (let i = 0; i < sensorCount; i++) {
                const sensor = fireSensors[i];
                if (sensor.id > maxFireSensorID) {
                    maxFireSensorID = sensor.id;
                }
            }
        }

        if (psmSensors) {
            const sensorCount = psmSensors.length;
            for (let i = 0; i < sensorCount; i++) {
                const sensor = psmSensors[i];
                if (sensor.id > maxPsmSensorID) {
                    maxPsmSensorID = sensor.id;
                }
            }
        }

        if (etcSensors) {
            const sensorCount = etcSensors.length;
            for (let i = 0; i < sensorCount; i++) {
                const sensor = etcSensors[i];
                if (sensor.id > maxEtcSensorID) {
                    maxEtcSensorID = sensor.id;
                }
            }
        }

        if (cctvs) {
            const sensorCount = cctvs.length;
            for (let i = 0; i < sensorCount; i++) {
                const sensor = cctvs[i];
                if (sensor.id > maxCctvID) {
                    maxCctvID = sensor.id;
                }
            }
        }

        SpaceDataManager.fireSensorID = maxFireSensorID;
        SpaceDataManager.psmSensorID = maxPsmSensorID;
        SpaceDataManager.etcSensorID = maxEtcSensorID;
        SpaceDataManager.cctvID = maxCctvID;
    }

    static setVisibleBuildingGroupList(buildingGroupList, sensorList, searchText) {
        let count = buildingGroupList.length;
        for (let i = 0; i < count; i++) {
            const buildingGroup = buildingGroupList[i];
            if (buildingGroup.displayText.includes(searchText)) {
                buildingGroup.visibleTreeView = true;
            }
            else {
                let visibleTreeView = false;
                for (let i = 0; i < buildingGroup.buildingDatas.length; i++) {
                    const buildingVisible = SpaceDataManager.setVisibleBuildingList(buildingGroup.buildingDatas[i], sensorList, searchText);
                    if (buildingVisible === true) {                        
                        visibleTreeView = true;
                    }
                }

                if (visibleTreeView) {
                    buildingGroup.visibleTreeView = true;
                }
                else {
                    buildingGroup.visibleTreeView = false;
                }
            }
        }
    }

    static setVisibleBuildingList(buildingData, sensorList, searchText) {
        let buildingVisible = false;

        let count = buildingData.zoneDatas.length;
        for (let i = 0; i < count; i++) {
            const zone = buildingData.zoneDatas[i];
            if (zone.displayText.includes(searchText)) {
                zone.visibleTreeView = true;
                buildingVisible = true;
            }
            else {
                const zoneVisible = SpaceDataManager.setVisibleZoneList(zone.equipmentZoneDatas, sensorList, searchText);
                if (zoneVisible === true) {
                    zone.visibleTreeView = true;
                    buildingVisible = true;
                }
                else {
                    zone.visibleTreeView = false;
                }
            }
        }

        buildingData.visibleTreeView = buildingVisible;

        return buildingData.visibleTreeView;
    }

    static setVisibleZoneList(equipZoneDatas, sensorList, searchText) {
        let zoneVisible = false;

        let count = equipZoneDatas.length;
        for (let i = 0; i < count; i++) {
            const equipZone = equipZoneDatas[i];
            if (equipZone.displayText.includes(searchText)) {
                equipZone.visibleTreeView = true;
                zoneVisible = true;
            }
            else {
                let visibleCount = 0;
                if (SpaceDataManager.setVisibleSensors(equipZone.id, sensorList[SpaceDataManager.FireSensorType], searchText)) {
                    visibleCount++;
                }
                if (SpaceDataManager.setVisibleSensors(equipZone.id, sensorList[SpaceDataManager.EtcSensorType], searchText)) {
                    visibleCount++;
                }
                if (SpaceDataManager.setVisibleSensors(equipZone.id, sensorList[SpaceDataManager.CCTVType], searchText)) {
                    visibleCount++;
                }

                if (SpaceDataManager.setVisibleSensors(equipZone.id, sensorList[SpaceDataManager.PSMSensorType], searchText)) {
                //if (SpaceDataManager.setVisiblePsmSensors(equipZone.id, sensorList[SpaceDataManager.PSMSensorType], searchText)) {
                    visibleCount++;
                }

                if (visibleCount > 0) {
                    equipZone.visibleTreeView = true;
                    zoneVisible = true;
                }
                else {
                    equipZone.visibleTreeView = false;
                }
            }
        }

        return zoneVisible;
    }

    static setVisibleSensors(equipZoneID, sensors, searchText) {
        let visible2 = false;
        const sensorsCount = sensors.length;
        for (let j = 0; j < sensorsCount; j++) {
            const sensor = sensors[j];
            if (equipZoneID !== sensor.equipZoneID)
                continue;

            if (sensor.name.includes(searchText)) {
                sensor.visibleTreeView = true;
                visible2 = true;
            }
            else {
                sensor.visibleTreeView = false;
            }
        }

        return visible2;
    }

    static setVisiblePsmSensors(zoneID, sensors, searchText) {
        let visible2 = false;
        const sensorsCount = sensors.length;
        for (let i = 0; i < sensorsCount; i++) {
            const sensor = sensors[i];
            if (!sensor.linkedZones)
                continue;

            for (var j = 0; j < sensor.linkedZones.length; j++) {
                if (sensor.linkedZones[j].id !== zoneID)
                    continue;

                if (sensor.name.includes(searchText)) {
                    sensor.visibleTreeView = true;
                    visible2 = true;
                }
                else {
                    sensor.visibleTreeView = false;
                }
                break;
            }
        }
        return visible2;
    }
}