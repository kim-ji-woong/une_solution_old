import * as Common from '../../Common/data/common';
import * as SdmsCommon from './common';

export type ZoneData = {
    id: number,
    addFloor: Common.NullableNumber,
    boundary: Common.NullableObject,
    broadcastText: Common.NullableString,
    buildingID: Common.NullableNumber,  // null이면 outdoor
    displayText: Common.NullableString,
    floorIndex: Common.NullableNumber, // null이면 outdoor
    textCenter: Common.Vector3 | null,
    zoneName: string,
    siteID: number
}

export type EquipmentZone = {
    boundary: Common.NullableObject,
    broadcastText: Common.NullableString,
    displayText: Common.NullableString,
    id: number,
    linkedZoneDatas: Array<ZoneData>,
    linkedZoneIDs: Array<number>,
    siteID: number,
    textCenter: Common.Vector3,
    type: Common.NullableNumber,
    zoneName: string
}

export type Zone = ZoneData & {
    datas: SdmsCommon.ZoneData,
    equipmentZoneDatas: Array<EquipmentZone>,
    sensors: object
}

export type Building = {
    id: number,
    broadcastText: Common.NullableString,
    buildingCode: string,
    buildingGroupID: number,
    buildingName: string,
    displayText: Common.NullableString,
    maxFloor: number,
    minFloor: number,
    textCenter: Common.Vector3 | null,
    zoneDatas: Array<Zone>
}

export type BuildingGroup = {
    id: number,
    groupName: string,
    displayText: Common.NullableString,
    textCenter: Common.Vector3 | null,
    parent: BuildingGroup | null,
    parentID: Common.NullableNumber,
    siteID: number,
    buildingDatas: Array<Building>
}

export type MessageResult = {
    success: boolean,
    message: string
}

export type AccountOption = {
    id: number,
    userID: number,
    category: string,
    subCategory: Common.NullableString,
    propertyValue1: Common.NullableString,
    propertyValue2: Common.NullableString,
    propertyValue3: Common.NullableString,
    propertyValue4: Common.NullableString
}