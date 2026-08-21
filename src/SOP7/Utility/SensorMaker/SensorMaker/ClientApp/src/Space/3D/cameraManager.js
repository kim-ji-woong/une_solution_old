import * as THREE from "three/build/three.module.js";
import Geometry from "../../utility/Geometry";
import { ModelDataManager } from "../services/modelDataManager";
import { SpaceBody } from "../spaceBody";
import { Contents3D } from "./contents3D";

export class CameraManager {
    static isEmptyCameraOptions(cameraOptions) {
        if (cameraOptions?.position && cameraOptions?.targetControl) {
            if (Geometry.getDistance3(cameraOptions.position[0], cameraOptions.position[1], cameraOptions.position[2], cameraOptions.targetControl[0], cameraOptions.targetControl[1], cameraOptions.targetControl[2]) <= Geometry.Tolerance) {
                return true;
            }

            return false;
        }

        return true;
    }

    static getBoundingBoxMinMax(model) {
        const boundingBox = new THREE.BoxHelper(model, 0xff0000);

        const posArray = boundingBox.geometry?.attributes?.position?.array;

        if (posArray) {
            const arrayCount = posArray.length;
            const min = new THREE.Vector3(null, null, null);
            const max = new THREE.Vector3(null, null, null);

            for (let i = 0; i < arrayCount; i += 3) {
                if (i === 0) {
                    min.x = max.x = posArray[i];
                    min.y = max.y = posArray[i + 1];
                    min.z = max.z = posArray[i + 2];
                }
                else {
                    if (min.x > posArray[i])
                        min.x = posArray[i];
                    if (min.y > posArray[i + 1])
                        min.y = posArray[i + 1];
                    if (min.z > posArray[i + 2])
                        min.z = posArray[i + 2];

                    if (max.x < posArray[i])
                        max.x = posArray[i];
                    if (max.y < posArray[i + 1])
                        max.y = posArray[i + 1];
                    if (max.z < posArray[i + 2])
                        max.z = posArray[i + 2];
                }
            }

            boundingBox.geometry.dispose();

            if (min.x === null)
                return [null, null];
            else
                return [min, max];
        }

        return [null, null];
    }

    static makeEmptyCameraOptions(orthoMode) {
        const cameraOptions = {
            position: [],
            quaternion: [],
            rotation: [],
            targetControl: []
        }

        if (orthoMode) {
            cameraOptions.zoom = 30;
        }

        return cameraOptions;
    }

    static getOutdoorModelCamera(_3dOptions, orthoMode) {
        const outdoorModel = _3dOptions[ModelDataManager.OutdoorModelName];

        if (outdoorModel) {
            let camera = orthoMode ? outdoorModel.cameraOrtho : outdoorModel.camera;

            if (CameraManager.isEmptyCameraOptions(camera)) {
                camera = null;
            }

            if (!camera) {
                return CameraManager.makeEmptyCameraOptions(orthoMode);
            }

            return camera;
        }

        return Contents3D.makeEmptyCameraOptions(orthoMode);
    }

    static getIndoorModelCamera(contentsType, contentsID, _3dOptions, orthoMode) {
        const indoorModels = _3dOptions[ModelDataManager.IndoorModelName];

        if (indoorModels) {
            if (contentsType === SpaceBody.Type_BuildingGroup) {
                for (const buildingGroupName in indoorModels) {
                    const buildingGroup = indoorModels[buildingGroupName];

                    if (buildingGroup instanceof Object) {
                        if (buildingGroup.buildingGroupID === contentsID) {
                            let camera = orthoMode ? buildingGroup.cameraOrtho : buildingGroup.camera;

                            if (CameraManager.isEmptyCameraOptions(camera)) {
                                camera = null;
                            }

                            if (!camera) {
                                return CameraManager.makeEmptyCameraOptions(orthoMode);
                            }

                            return camera;
                        }
                    }
                }
            }
            else if (contentsType === SpaceBody.Type_Floor) {
                const zoneData = _3dOptions.zones[contentsID];

                if (zoneData) {
                    const buildingID = zoneData[1];
                    const buildingData = _3dOptions.buildingIDs[buildingID];

                    if (buildingData && buildingData.length >= 3) {
                        const buildingGroupName = buildingData[1];
                        const buildingName = buildingData[2];

                        const buildingGroup = indoorModels[buildingGroupName];

                        if (buildingGroup && buildingName) {
                            const building = buildingGroup[buildingName];

                            if (building?.floors) {
                                for (const floor of building.floors) {
                                    if (floor.zoneID === contentsID) {
                                        let camera = orthoMode ? floor.cameraOrtho : floor.camera;

                                        if (CameraManager.isEmptyCameraOptions(camera)) {
                                            camera = null;
                                        }

                                        if (!camera) {
                                            floor.camera = CameraManager.makeEmptyCameraOptions(orthoMode);
                                            return floor.camera;
                                        }

                                        return camera;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return Contents3D.makeEmptyCameraOptions(orthoMode);
    }

    static setIndoorModelCamera(cameraOptions, contentsType, contentsID, _3dOptions, orthoMode) {
        const indoorModels = _3dOptions[ModelDataManager.IndoorModelName];

        if (indoorModels) {
            if (contentsType === SpaceBody.Type_BuildingGroup) {
                for (const buildingGroupName in indoorModels) {
                    const buildingGroup = indoorModels[buildingGroupName];

                    if (buildingGroup instanceof Object) {
                        if (buildingGroup.buildingGroupID === contentsID) {
                            if (orthoMode) {
                                buildingGroup.cameraOrtho = cameraOptions;
                            }
                            else {
                                buildingGroup.camera = cameraOptions;
                            }

                            return;
                        }
                    }
                }
            }
            else if (contentsType === SpaceBody.Type_Floor) {
                const zoneData = _3dOptions.zones[contentsID];

                if (zoneData) {
                    const buildingID = zoneData[1];
                    const buildingData = _3dOptions.buildingIDs[buildingID];

                    if (buildingData && buildingData.length >= 3) {
                        const buildingGroupName = buildingData[1];
                        const buildingName = buildingData[2];

                        const buildingGroup = indoorModels[buildingGroupName];

                        if (buildingGroup && buildingName) {
                            const building = buildingGroup[buildingName];

                            if (building?.floors) {
                                for (const floor of building.floors) {
                                    if (floor.zoneID === contentsID) {
                                        if (orthoMode) {
                                            floor.cameraOrtho = cameraOptions;
                                        }
                                        else {
                                            floor.camera = cameraOptions;
                                        }

                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    static setControlCamera(camera, controls, min, max, cameraOptions, optionOnly) {
        if (min && max) {
            const theta = Math.PI * camera.fov / 180;
            const depth = max.z - min.z;
            const height = max.y - min.y;
            const length = max.x - min.x;
            const len = length > height ? length / 2 : height / 2;

            const distance = len / Math.tan(theta / 2);

            const vTarget = new THREE.Vector3((min.x + max.x) / 2, (min.y + max.y) / 2, (min.z + max.z) / 2);

            const originPos = optionOnly ? { x: camera.position.x, y: camera.position.y, z: camera.position.z } : null;
            const originTarget = optionOnly ? { x: controls.target.x, y: controls.target.y, z: controls.target.z } : null;

            camera.position.set(vTarget.x, vTarget.y, vTarget.z + depth / 2 + distance);

            controls.target.set(vTarget.x, vTarget.y, vTarget.z);
            camera.lookAt(vTarget);
            controls.update();

            CameraManager.setCameraOptions(camera, controls, cameraOptions);

            if (optionOnly) {
                camera.position.set(originPos.x, originPos.y, originPos.z);

                controls.target.set(originTarget.x, originTarget.y, originTarget.z);
                camera.lookAt(controls.target);
                controls.update();
            }

            const orthoCameraOptions = {
                position: [vTarget.x, vTarget.y + 30, vTarget.z],
                quaternion: [-0.7071062326431274, 0, 0, 0.7071062326431274],
                rotation: [-1.5707948207855225, 0, 0],
                targetControl: [vTarget.x, vTarget.y, vTarget.z],
                zoom: 30
            };

            return orthoCameraOptions;
        }

        return null;
    }

    static setControlCameraOrtho(camera, min, max) {
        if (min && max) {
            const vTarget = new THREE.Vector3((min.x + max.x) / 2, max.y + 100, (min.z + max.z) / 2);
            const vTarget2 = new THREE.Vector3((min.x + max.x) / 2, (min.y + max.y) / 2, (min.z + max.z) / 2);

            camera.position.set(vTarget.x, vTarget.y, vTarget.z);
            camera.lookAt(vTarget2);
        }
    }

    static setCameraOptions(camera, controls, cameraOptions) {
        if (cameraOptions) {
            cameraOptions.position[0] = camera.position.x;
            cameraOptions.position[1] = camera.position.y;
            cameraOptions.position[2] = camera.position.z;

            cameraOptions.rotation[0] = camera.rotation.x;
            cameraOptions.rotation[1] = camera.rotation.y;
            cameraOptions.rotation[2] = camera.rotation.z;

            cameraOptions.quaternion[0] = camera.quaternion.x;
            cameraOptions.quaternion[1] = camera.quaternion.y;
            cameraOptions.quaternion[2] = camera.quaternion.z;
            cameraOptions.quaternion[3] = camera.quaternion.w;

            cameraOptions.targetControl[0] = controls.target.x;
            cameraOptions.targetControl[1] = controls.target.y;
            cameraOptions.targetControl[2] = controls.target.z;
        }
    }

    static setCamera(camera, controls, cameraOptions) {
        if (cameraOptions) {
            camera.position.set(cameraOptions.position[0], cameraOptions.position[1], cameraOptions.position[2]);

            if (cameraOptions.quaternion) {
                camera.quaternion.set(cameraOptions.quaternion[0], cameraOptions.quaternion[1], cameraOptions.quaternion[2], cameraOptions.quaternion[3]);
            }

            camera.rotation.set(cameraOptions.rotation[0], cameraOptions.rotation[1], cameraOptions.rotation[2]);
            controls.target.set(cameraOptions.targetControl[0], cameraOptions.targetControl[1], cameraOptions.targetControl[2]);

            if (cameraOptions.near !== null && cameraOptions.near !== undefined) {
                camera.near = cameraOptions.near;
            }

            if (cameraOptions.far !== null && cameraOptions.far !== undefined) {
                camera.far = cameraOptions.far;
            }

            if (cameraOptions.fov !== null && cameraOptions.fov !== undefined) {
                camera.fov = cameraOptions.fov;
            }

            if (cameraOptions.zoom) {
                camera.zoom = cameraOptions.zoom;
            }
        }
    }
}
