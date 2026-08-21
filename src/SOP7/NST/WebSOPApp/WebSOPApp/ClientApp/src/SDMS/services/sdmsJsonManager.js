import * as Frontend from "../data/frontend";
import * as Common from '../../Common/data/common';

export class SdmsJsonManager {
    static makeRequestBuildingGroupList()/*: string*/ {
        const json = {
            "requestBuildingGroupList": true
        };

        return JSON.stringify(json);
    }

    static makeRequestGltfDataList() {
        const json = {
            "requestGltfDataList": true
        };

        return JSON.stringify(json);
    }

    static makeRequestSaveIndoorModelViewport(modelName/*: string*/, cameraData/*: Frontend.PerspectiveCameraData2*/)/*: string*/ {
        const json = {
            "requestSaveIndoorModelViewport":
            {
                "modelName": modelName,
                "cameraPosition": {
                    x: cameraData.pos.x,
                    y: cameraData.pos.y,
                    z: cameraData.pos.z
                },
                "cameraQuaternion": {
                    x: cameraData.quaternion.x,
                    y: cameraData.quaternion.y,
                    z: cameraData.quaternion.z,
                    w: cameraData.quaternion.w
                },
                "cameraRotation": {
                    x: cameraData.rotation.x,
                    y: cameraData.rotation.y,
                    z: cameraData.rotation.z
                },
                "orbitTarget": {
                    x: cameraData.orbitTarget.x,
                    y: cameraData.orbitTarget.y,
                    z: cameraData.orbitTarget.z
                },
            }
        };

        return JSON.stringify(json);
    }

    static makeRequestStreamServerURL()/*: string*/ {
        const json = {
            "requestStreamServerURL": true
        };

        return JSON.stringify(json);
    }

    // 옵션 요청
    static makeRequestGetOption(UserID/*: string*/, Category/*: string*/) {
        const json = {
            "requestOption":
            {
                "userID": UserID,
                "category": Category

            }
        }
        return JSON.stringify(json);
    }

    static makeRequestSaveOption(ID/*: number*/, UserID/*: number*/, Category/*: string*/, SubCategory/*: Common.NullableString*/, PropertyValue1/*: Common.NullableString*/, PropertyValue2/*: Common.NullableString*/, PropertyValue3/*: Common.NullableString*/, PropertyValue4/*: Common.NullableString*/)/*: string*/ {
        const json = {
            "requestSaveOption":
            {
                'saveOption': {
                    "id": ID,
                    "userID": UserID,
                    "category": Category,
                    "subCategory": SubCategory,
                    "propertyValue1": PropertyValue1,
                    "propertyValue2": PropertyValue2,
                    "propertyValue3": PropertyValue3,
                    "propertyValue4": PropertyValue4,
                }
            }
        }
        return JSON.stringify(json);
    }

    static makeRequestSensorList() {
        const json = {
            "requestSensorList":
            {
                "requestFireSensors": true,
                "requestPSMSensors": true,
                "requestEtcSensors": true,
                "requestCCTVs": true
            }
        };

        return JSON.stringify(json);
    }

    static makeRequestOuterDatas() {
        const json = {
            "requestOuterDatas": true
        };

        return JSON.stringify(json);
    }

    static makeRequestIndoorDatas(zoneID) {
        const json = {
            "requestIndoorDatas":
            {
                "zoneID": zoneID
            }
        };

        return JSON.stringify(json);
    }

    static makeRequestWeatherInfo() {
        const json = {
            "requestWeatherInfo": true
        };

        return JSON.stringify(json);
    }

    static makeRequestWeatherWeeklyInfo() {
        const json = {
            "requestWeatherWeeklyInfo": true
        };

        return JSON.stringify(json);
    }

    static makeRequestMalfunction(sensorType, sensorZoneID, accessedUserID, isMalfunction) {
        const json = {
            "requestMalfunction":
            {
                "sensorType": sensorType,
                "sensorZoneID": sensorZoneID,
                "accessedUserID": accessedUserID,
                "isMalfunction": isMalfunction
            }
        };

        return JSON.stringify(json);
    }

    static makeRequestClearManualReport(sensorType, sensorZoneID, sensorZoneHistoryID, accessedUserID) {
        const json = {
            "requestClearManualReport":
            {
                "sensorType": sensorType,
                "sensorZoneID": sensorZoneID,
                "sensorZoneHistoryID": sensorZoneHistoryID,
                "accessedUserID": accessedUserID
            }
        };

        return JSON.stringify(json);
    }

    static makeRequestMobileUserList() {
        const json = {
            "requestMobileUserList": true
        };

        return JSON.stringify(json);
    }

    static makeRequestRegulars() {
        const json = {
            "requestRegulars": true
        };

        return JSON.stringify(json);
    }

    static makeRequestRegularMembers() {
        const json = {
            "requestRegularMembers": true
        };

        return JSON.stringify(json);
    }
}