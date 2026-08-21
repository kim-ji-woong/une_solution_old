import { SpaceDataManager } from "../services/spaceDataManager";
import { SpaceController } from "../services/spaceController";

export class ExcelManager {
    static openSensorFile(event, sensorType, zoneID, equipZoneID, spaceBody) {
        const file = event.target.files[0];
        if (!file) {
            return;
        }

        if (sensorType === SpaceDataManager.FireSensorType) {
            ExcelManager.openFireSensorFile(file, zoneID, equipZoneID, spaceBody);
        }
        else if (sensorType === SpaceDataManager.PSMSensorType) {
            ExcelManager.openPSMSensorFile(file, zoneID, equipZoneID, spaceBody);
        }
        else if (sensorType === SpaceDataManager.EtcSensorType) {
            ExcelManager.openEtcSensorFile(file, zoneID, equipZoneID, spaceBody);
        }
        else if (sensorType === SpaceDataManager.CCTVType) {
            ExcelManager.openCCTVFile(file, zoneID, equipZoneID, spaceBody);
        }
    }

    static addSensors(sensors, sensorType, zoneID, equipZoneID, spaceBody) {
        const _3dOptions = { ...spaceBody.state._3dOptions };

        if (equipZoneID !== null && zoneID !== null) {
            const equipZone = SpaceDataManager.findEquipZone(equipZoneID, zoneID, _3dOptions);

            if (equipZone) {
                SpaceDataManager.addEquipZoneSensors(sensors, sensorType, equipZone);
            }
        }
        else if (zoneID !== null) {
            const zone = SpaceDataManager.findZone(zoneID, _3dOptions);

            if (zone) {
                SpaceDataManager.addZoneSensors(sensors, sensorType, zone);
            }
        }
        else {
            return;
        }

        spaceBody.setState({ _3dOptions });
    }

    static async openFireSensorFile(file, zoneID, equipZoneID, spaceBody) {
        const sensorType = SpaceDataManager.FireSensorType;
        const result = await SpaceController.requestUploadExcelFile(file, sensorType);

        if (result && result.length >= 3) {
            if (result[0]) {
                ExcelManager.addSensors(result[2], sensorType, zoneID, equipZoneID, spaceBody);
            }
            else {
                alert(result[1]);
            }
        }
    }

    static async openPSMSensorFile(file, zoneID, equipZoneID, spaceBody) {
        const sensorType = SpaceDataManager.PSMSensorType;
        const result = await SpaceController.requestUploadExcelFile(file, SpaceDataManager.PSMSensorType);

        if (result && result.length >= 3) {
            if (result[0]) {
                ExcelManager.addSensors(result[2], sensorType, zoneID, equipZoneID, spaceBody);
            }
            else {
                alert(result[1]);
            }
        }
    }

    static async openEtcSensorFile(file, zoneID, equipZoneID, spaceBody) {
        const sensorType = SpaceDataManager.EtcSensorType;
        const result = await SpaceController.requestUploadExcelFile(file, SpaceDataManager.EtcSensorType);

        if (result && result.length >= 3) {
            if (result[0]) {
                ExcelManager.addSensors(result[2], sensorType, zoneID, equipZoneID, spaceBody);
            }
            else {
                alert(result[1]);
            }
        }
    }

    static async openCCTVFile(file, zoneID, equipZoneID, spaceBody) {
        const sensorType = SpaceDataManager.CCTVType;
        const result = await SpaceController.requestUploadExcelFile(file, SpaceDataManager.CCTVType);

        if (result && result.length >= 3) {
            if (result[0]) {
                ExcelManager.addSensors(result[2], sensorType, zoneID, equipZoneID, spaceBody);
            }
            else {
                alert(result[1]);
            }
        }
    }

    static saveSensorFile(sensorType, zoneID, equipZoneID, _3dOptions) {
        const equipZone = SpaceDataManager.findEquipZone(equipZoneID, zoneID, _3dOptions);

        if (equipZone) {
            if (equipZone.sensors) {
                const sensors = equipZone.sensors[sensorType];

                if (sensors) {
                    SpaceController.requestDownloadSensorExcelFile(sensorType, sensors);
                }
            }
        }
        else {
            const zone = SpaceDataManager.findZone(zoneID, _3dOptions);

            if (zone) {
                if (zone.sensors) {
                    const sensors = zone.sensors[sensorType];

                    if (sensors) {
                        SpaceController.requestDownloadSensorExcelFile(sensorType, sensors);
                    }
                }
            }
        }
    }
}