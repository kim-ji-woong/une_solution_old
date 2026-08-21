import { SpaceDataManager } from "./spaceDataManager";

export class SpaceJsonManager {
    static makeRequestBuildingGroupList() {
        const json = {
            "requestBuildingGroupList": true
        };

        return JSON.stringify(json);
    }

    static makeRequestGltfDataList(userID) {
        const json = {
            "requestGltfDataList": {
                "userID": userID,
            }
        };

        return JSON.stringify(json);
    }

    static makeRequestDownloadSensorExcelFile(sensorType, sensors) {
        const targetName = "requestSensorExcelFile";
        const json = {};

        json[targetName] = {};

        if (sensorType === SpaceDataManager.FireSensorType) {
            json[targetName]["fireSensors"] = sensors;
        }
        else if (sensorType === SpaceDataManager.PSMSensorType) {
            json[targetName]["psmSensors"] = sensors;
        }
        else if (sensorType === SpaceDataManager.EtcSensorType) {
            json[targetName]["etcSensors"] = sensors;
        }
        else if (sensorType === SpaceDataManager.CCTVType) {
            json[targetName]["cctvs"] = sensors;
        }

        return JSON.stringify(json);
    }

    static makeRequestUploadModelFile(loginData, _3dOptions, cancelTempFiles, appendFiles, removeNCopy) {
        const json = {
            "requestUploadModelFile": {
                "userID": loginData.user.id,
                "userName": loginData.user.name,
                "cancelTempFiles": cancelTempFiles,
                "appendFiles": appendFiles,
                "removeNCopy": removeNCopy,
                "fileNames": SpaceDataManager.getModelFileNames(_3dOptions)
            }
        };

        return JSON.stringify(json);
    }

    static makeRequestRemoveTempFile(loginData, fileName) {
        const json = {
            "requestRemoveTempFile": {
                "userID": loginData.user.id,
                "userName": loginData.user.name,
                "fileName": fileName
            }
        };

        return JSON.stringify(json);
    }
}