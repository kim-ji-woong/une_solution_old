export class CommonJsonManager {
    static makeRequestSaveXML(datas) {
        const json = {
            "requestSaveXML": {
                "bTempSave": datas.bTempSave,
                "userID": datas.loginData.user.id,
                "userName": datas.loginData.user.name,
                "siteName": datas.siteName,
                "sensorTypes": datas.sensorTypes,
                
                "models": datas.models,

                "testBuildingGroupData": datas.buildingGroupList._buildingGroupDatas,
                "testBuildingData": datas.buildingGroupList._buildingDatas,
                "testZoneData": datas.buildingGroupList._zoneDatas,
                "TestEquipmentZoneData": datas.buildingGroupList._equipzoneDatas,
                
                "outdoorZones": datas.outdoorZones,
                "gltfOption": datas.gltfOption,
                "fireSensors": datas.fireSensors,
                "psmSensors": datas.psmSensors,
                "etcSensors": datas.etcSensors,
                "cctvs": datas.cctvSensors,
            }
        };

        return JSON.stringify(json);
    }

    static makeRequestOpenTempXML(loginData) {
        const json = {
            "requestOpenTempXML": {
                "userID": loginData.user.id,
                "userName": loginData.user.name
            }
        };

        return JSON.stringify(json);
    }
}