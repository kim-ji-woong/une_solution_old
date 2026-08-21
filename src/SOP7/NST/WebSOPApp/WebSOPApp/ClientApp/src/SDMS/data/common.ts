import * as Common from "../../Common/data/common";

export type ZoneData = {
    fakeWallElevation: number | null,
    poiElevation: number | null,
    zoneID: number
}

export type GltfModelData = {
    buildingGroupID: number | null,
    buildingID: number | null,
    zoneID: number | null,
    cameraFar: number,
    cameraFov: number,
    cameraNear: number,
    cameraPosition: Common.Vector3,
    cameraPositionX: number,
    cameraPositionY: number,
    cameraPositionZ: number,
    cameraQuaternion: Common.Vector4,
    cameraQuaternionX: number,
    cameraQuaternionY: number,
    cameraQuaternionZ: number,
    cameraQuaternionW: number,
    cameraRotation: Common.Vector3,
    cameraRotaionX: number,
    cameraRotaionY: number,
    cameraRotaionZ: number,
    floorIndex: number | null,
    id: number,
    modelDisplayText: string | null,
    modelFile: string,
    modelID: number,
    orbitTarget: Common.Vector3,
    orbitTargetX: number,
    orbitTargetY: number,
    orbitTargetZ: number
}

export type GltfModelOrthoData = {
    zoneID: number | null,
    cameraPosition: Common.Vector3,
    cameraPositionX: number,
    cameraPositionY: number,
    cameraPositionZ: number,
    cameraQuaternion: Common.Vector4,
    cameraQuaternionX: number,
    cameraQuaternionY: number,
    cameraQuaternionZ: number,
    cameraQuaternionW: number,
    cameraRotation: Common.Vector3,
    cameraRotaionX: number,
    cameraRotaionY: number,
    cameraRotaionZ: number,
    id: number,
    modelFile: string,
    modelID: number,
    target: Common.Vector3,
    targetX: number,
    targetY: number,
    targetZ: number,
    zoom: number
}

export type GltfModel = {
    childModels: Array<GltfModel>,
    id: number,
    modelDatas: Array<GltfModelData>,
    modelName: string,
    modelOrthoDatas: Array<GltfModelOrthoData>,
    parentID: number | null
}

// Key : Option Name
// Value : Option Value
export type GltfOption = Map<string, string>;