import * as Common from '../../Common/data/common';
import * as SdmsCommon from './common';
//import * as THREE from "three/build/three.module.js";

// [0] : EquipZone ID
// [1] : EquipZone Name
// [2] : Text Center
export type EquipmentZone = [number, string, Common.Vector3 | null];

// [0] : floorIndex
// [1] : buildingID
// [2] : zoneName
// [3] : displayText
// [4] : textCenter X
// [5] : textCenter y
// [6] : textCenter z
export type Zone = [number, number, string, Common.NullableString, Common.NullableNumber, Common.NullableNumber, Common.NullableNumber] & {
    datas: SdmsCommon.ZoneData,
    // Key : EqupZone ID
    equipZones: Map<number, EquipmentZone>,
    sensors: object
};

// [0] : building ID
// [1] : building name
// [2] : building boundary model name
// [3] : text center x
// [4] : text center x
// [5] : text center x
// [6] : Zone array => Key : Zone ID
export type Building = [number, string, string, Common.NullableNumber, Common.NullableNumber, Common.NullableNumber, Map<number, Zone>];
//export type Building = Array<number | string | Map<number, Zone> | null>;

// [0] : building ID
// [1] : buildingGroup name
// [2] : building name
// [3] : building boundary model name
// [4] : text center x
// [5] : text center x
// [6] : text center x
// [7] : Zone array => Key : Zone ID
export type Building2 = [number, string, string, string, Common.NullableNumber, Common.NullableNumber, Common.NullableNumber, Map<number, Zone>];

// [0] : buildingGroup name
// [1] : display text
// [2] : buildingGroup boundary model name
// [3] : text center x
// [4] : text center x
// [5] : text center x
// [6] : buildingGroup ID
export type BuildingGroup = [string, Common.NullableString, Common.NullableString, Common.NullableNumber, Common.NullableNumber, Common.NullableNumber, number];

export type PerspectiveCameraData = {
    far: number,
    fov: number,
    near: number,
    position: Common.Vector3Array,
    quaternion: Common.Vector4Array,
    rotation: Common.Vector3Array,
    targetControl: Common.Vector3Array
};

export type OrthographicCameraData = {
    position: Common.Vector3Array,
    quaternion: Common.Vector4Array,
    rotation: Common.Vector3Array,
    targetControl: Common.Vector3Array,
    zoom: number
};

export type PerspectiveCameraData2 = {
    pos: {
        x: number,
        y: number,
        z: number
    },
    quaternion: {
        x: number,
        y: number,
        z: number,
        w: number
    },
    rotation: {
        x: number,
        y: number,
        z: number
    },
    orbitTarget: {
        x: number,
        y: number,
        z: number
    }
};

export type ZoneModel = {
    camera: PerspectiveCameraData | null,
    cameraOrtho: OrthographicCameraData | null,
    file: string,
    floorIndex: number,
    modelDisplayText: Common.NullableString,
    zoneID: number
};

export type BuildingModel = {
    buildingID: number,
    camera: PerspectiveCameraData | null,
    file: string,
    floors: Array<ZoneModel>,
    modelDisplayText: Common.NullableString
};

// Key : Building Name
export type IndoorModel = Map<string, BuildingModel> & {
    buildingGroupID: number,
    camera: PerspectiveCameraData,
    file: string,
    modelDisplayText: Common.NullableString
};

export type OutdoorZone = {
    datas: SdmsCommon.ZoneData | null,
    id: number,
    name: string,
    sensors: {}
};

export type _3DOptions = {
    // Key : Building Name
    allBuildings: Map<string, Building2>,
    backgroundImage: Common.NullableString,
    buildingGroups: Array<BuildingGroup>,
    // Key : Building ID
    buildingIDs: Map<string, Building2>,
    // Key : BuildingGroup Name
    // Value.Key : Building Name
    buildings: Map<string, Map<string, Building>>,
    // Key : BuildingGroup Name
    indoorModels: Map<string, IndoorModel>,
    modelBaseURL: string,
    outdoorModel: {
        camera: PerspectiveCameraData | null,
        cameraOrtho: OrthographicCameraData | null,
        file: string,
        modelDisplayText: Common.NullableString
    },
    // Key : Zone ID
    outdoorZones: Map<string, OutdoorZone>,
    textureBaseURL: Common.NullableString,
    // Key : Zone ID
    zones: Map<string, Zone>
};

// 대피로 안내를 위한 화살표
export type ExitArrowMovingData = [[object/*THREE.Mesh*/, object/*THREE.Vector3*/, object/*THREE.Vector3*/, number], [object/*THREE.Mesh*/, object/*THREE.Vector3*/, object/*THREE.Vector3*/, number]];
// 실내모델 정보
// [0] : 실내모델 노드
// [1] : 카메라 옵션
// [2] : 대피로 화살표 Model
// [3] : 대피로 화살표 움직임을 위한 데이터
export type InternalModel = [object/*THREE.Mesh*/, PerspectiveCameraData | null, object/*THREE.Mesh*/ | null, ExitArrowMovingData | null];

export type MovingCamera = {
    movingTime: number,
    elapsedTime: number,
    distancePosition: number,
    distanceQuaternion: Common.NullableNumber,
    distanceRotation: number,
    beginCameraPos: object/*THREE.Vector3*/,
    beginCameraQuaternion: object/*THREE.Quaternion*/,
    beginCameraRotation: object/*THREE.Vector3*/,
    targetCameraOptions: PerspectiveCameraData,
    fov: number,
    far: number,
    near: number,
    mode: number,
    param: any
}