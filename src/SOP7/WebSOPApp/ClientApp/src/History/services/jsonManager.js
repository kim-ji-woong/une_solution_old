export default class JsonManager{    
    static makeRequestUserHistories(beginTime, endTime) {
        const json = {
            "requestUserHistories":
            {
                "BeginTime": beginTime,
                "EndTime": endTime
            }
        };

        return JSON.stringify(json);
    }

    static makeRequestGetMinMaxIndex(beginTime, endTime, facilityType, buildingGroupID, buildingID, zoneID) {
        const json = {
            "requestGetMinMaxIndex":
            {
                "BeginTime": beginTime,
                "EndTime": endTime,
                "FacilityType": facilityType,
                "BuildingGroupID": buildingGroupID,
                "BuildingID": buildingID,
                "ZoneID": zoneID
            }
        };

        return JSON.stringify(json);
    }

    static makeRequestSensorDetectHistories(beginTime, endTime, facilityType, buildingGroupID, buildingID, zoneID, lastSensorZoneHistoryID, rowCount, isDesc) {
        const json = {
            "requestSensorDetectHistories":
            {
                "BeginTime": beginTime,
                "EndTime": endTime,
                "FacilityType": facilityType,
                "BuildingGroupID": buildingGroupID,
                "BuildingID": buildingID,
                "ZoneID": zoneID,
                "LastSensorZoneHistoryID": lastSensorZoneHistoryID,
                "RowCount": rowCount,
                "IsDesc": isDesc
            }
        };

        return JSON.stringify(json);
    }

    static makeRequestSensorDetectAnalysis(beginTime, endTime, facilityType, buildingGroupID, buildingID, zoneID) {
        const json = {
            "requestSensorDetectAnalysis":
            {
                "BeginTime": beginTime,
                "EndTime": endTime,
                "FacilityType": facilityType,
                "BuildingGroupID": buildingGroupID,
                "BuildingID": buildingID,
                "ZoneID": zoneID
            }
        };

        return JSON.stringify(json);
    }

    static makeRequestSOPHistories(beginTime, endTime) {
        const json = {
            "requestSOPHistories":
            {
                "BeginTime": beginTime,
                "EndTime": endTime
            }
        };

        return JSON.stringify(json);
    }

    static makeRequestSOPComponentHistories(actionStepHistoryID) {
        const json = {
            "requestSOPComponentHistories":
            {
                "ActionStepHistoryID": actionStepHistoryID
            }
        };

        return JSON.stringify(json);
    }

    static makeRequestDisasterCategories() {
        const json = {
            "requestDisasterCategories": true
        };

        return JSON.stringify(json);
    }

    static makeRequestUpdateAlarmMemo(sensorZoneHistoryID, memo) {
        const json = {
            "RequestUpdateAlarmMemo":
            {
                "SensorZoneHistoryID": sensorZoneHistoryID,
                "Memo": memo
            }
        };

        return JSON.stringify(json);
    }
}