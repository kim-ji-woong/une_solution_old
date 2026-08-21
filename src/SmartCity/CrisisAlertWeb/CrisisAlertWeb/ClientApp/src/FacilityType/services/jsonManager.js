export class JsonManager {
    static makeAlarmList(type) {
        const json = {
            "RequestAlarmList":
            {
                "facilityType": type,
            },
        }

        return JSON.stringify(json);
    }

    static makeManualList(type) {
        const json = {
            "RequestManualList":
            {
                "facilityType": type,
            },
        }

        return JSON.stringify(json);
    }

    static makeSensorInfo(id, type) {
        const json = {
            "RequestSensorInfo":
            {
                "id": id,
                "facilityType": type,
            },
        }

        return JSON.stringify(json);
    }

    static makeFirstSensor(type) {
        const json = {
            "RequestFirstSensor":
            {
                "facilityType": type,
            },
        }

        return JSON.stringify(json);
    }

    static makeFacilityTypeSensors(type) {
        const json = {
            "RequestFacilityTypeSensors":
            {
                "facilityType": type,
            },
        }

        return JSON.stringify(json);
    }
}