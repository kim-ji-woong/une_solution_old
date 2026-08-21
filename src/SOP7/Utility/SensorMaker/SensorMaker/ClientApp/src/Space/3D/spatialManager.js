import * as THREE from "three/build/three.module.js";
import { Contents3D } from "./contents3D";

export class SpatialManager {
    static async clearIndoorModels(contents3D) {
        const internalModels = { ...contents3D.internalModels };
        contents3D.internalModels = {};

        const models = [];

        for (const contentsName in internalModels) {
            const model = internalModels[contentsName];

            if (model && model[0]) {
                contents3D.scene.remove(model[0]);
                models.push(model[0]);
            }
        }

        SpatialManager.clearModel(models);
    }

    static clearModel(models) {
        const data = {
            meshes: [],
            materials: [],
            textures: [],
            geometries: []
        }

        models.map(model => {
            SpatialManager.findMeshs(model, data);
        });

        data.meshes.forEach((obj) => {
            if (obj.parent !== null) {
                obj.parent.remove(obj);
            }
            if (obj.dispose) {
                obj.dispose();
            }
        });

        data.materials.forEach((mat) => {
            if (mat.dispose) {
                mat.dispose();
            }
        });

        data.textures.forEach((tex) => {
            tex.dispose();
        });

        data.geometries.forEach((geom) => {
            geom.dispose();
        });
    }

    static findMeshs(model, data) {
        if (model instanceof THREE.Mesh) {
            data.meshes.push(model);

            if (model.geometry instanceof THREE.BufferGeometry) {
                data.geometries.push(model.geometry);
            }

            if (model.material instanceof THREE.Material) {
                data.materials.push(model.material);

                if (model.material.map instanceof THREE.Texture) {
                    data.textures.push(model.material.map);
                }
            }
        }

        model.children.map(child => SpatialManager.findMeshs(child, data));
    }

    static showIndoor(zoneID, _3dOptions, contents3D) {
        SpatialManager.clearIndoorModels(contents3D);

        if (_3dOptions) {
            const floor = SpatialManager.getZoneFloor(zoneID, _3dOptions);

            if (floor?.file && floor?.camera) {
                const param = {
                    zoneID: zoneID
                };

                // 카메라 이동이 끝난후 나타나게 한다.
                //contents3D.poiManager.removeSensors(null);

                // contents3D.isInoodr()에서 true로 표시되도록 하기 위하여 임시 데이터를 사용한다.
                contents3D.currentModel = {
                    name: floor.file
                };

                if (contents3D.prevIndoorModel) {
                    contents3D.prevIndoorModel.visible = false;
                }

                contents3D.loadFile(floor.file, true, floor.camera, Contents3D.Mode_Indoor, _3dOptions, SpatialManager.postIndoorModelFile, [floor.file, contents3D]);
                setTimeout(() => contents3D.setMovingCamera(floor.camera, Contents3D.Mode_Indoor, param), 750);
            }
        }
    }

    static getZoneFloor(zoneID, _3dOptions) {
        const zone = _3dOptions.zones[zoneID];

        if (zone) {
            const buildingID = zone[1];
            const building = _3dOptions.buildingIDs[buildingID];

            if (building) {
                const buildingDisplayName = building[2];
                let model = _3dOptions.indoorModels[buildingDisplayName];

                if (model) {
                    for (const keyName in model) {
                        const data = model[keyName];

                        if (data.buildingID === buildingID) {
                            if (data.floors) {
                                const floorCount = data.floors.length;

                                for (let i = 0; i < floorCount; i++) {
                                    const floor = data.floors[i];

                                    if (floor.zoneID === zoneID) {
                                        return floor;
                                    }
                                }
                            }

                            return null;
                        }
                    }
                }
                else {
                    for (const modelName in _3dOptions.indoorModels) {
                        const _model = _3dOptions.indoorModels[modelName];

                        for (const keyName in _model) {
                            const data = _model[keyName];

                            if (data.buildingID === buildingID) {
                                if (data.floors) {
                                    const floorCount = data.floors.length;

                                    for (let i = 0; i < floorCount; i++) {
                                        const floor = data.floors[i];

                                        if (floor.zoneID === zoneID) {
                                            return floor;
                                        }
                                    }
                                }

                                return null;
                            }
                        }
                    }
                }
            }
        }

        return null;
    }
}
