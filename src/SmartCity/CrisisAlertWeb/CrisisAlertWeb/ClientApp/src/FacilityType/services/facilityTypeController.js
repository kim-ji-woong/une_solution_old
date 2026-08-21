import CryptoJS from 'crypto-js';
import sha256 from 'crypto-js/sha256';
import { JsonManager } from './jsonManager';
import store from '../../Root/store';

import SessionString from '../../Common/js/sessionString';

export class FacilityTypeController {
    static sensorTimer = null;

    static StartWatchTimer(id, type) {
        if (FacilityTypeController.sensorTimer !== null) {
            clearTimeout(FacilityTypeController.sensorTimer);
        }
            
        FacilityTypeController.sensorTimer = setTimeout(function tick() {
            FacilityTypeController.WatchSensorInfo(id, type);
            FacilityTypeController.sensorTimer = setTimeout(tick, 1500);
        }, 1500);
    }

    static async WatchSensorInfo(id, type) {
        let result = await FacilityTypeController.DisplaySensorInfo(id, type);

        if (result === null)
            return;

        let newSensorInfo = result.sensor;

        console.log("타입: " + type + ", 센서ID: " + newSensorInfo.id + ", 상태: " + newSensorInfo.state);
        let oldSensorInfo = store.getState().sensorInfo;

        if (oldSensorInfo === null || oldSensorInfo === undefined) {
            store.dispatch({ type: 'SENSOR_INFO', sensorInfo: newSensorInfo });
        } else if (oldSensorInfo.id !== newSensorInfo.id || oldSensorInfo.state !== newSensorInfo.state) {
            store.dispatch({ type: 'SENSOR_INFO', sensorInfo: newSensorInfo });
        }
    }

    static async DisplaySensorInfo(id, type) {
        if (id === null || id === undefined || type === null || type === undefined)
            return null;

        try {
            const jsonData = JsonManager.makeSensorInfo(id, type);

            const res = await fetch('/FacilityType/RequestData/', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok && res.status !== 204) {
                const result = await res.json();

                return result;
            } else {
                let result = new Object();
                result.success = false;
                result.message = "FacilityType Controller 페이지를 찾을 수 없습니다. 네트워크를 확인해주세요.";

                return result;
            }
        } catch (e) {
            console.log(e);
        }

        return null;
    }

    static async firstSensor(type) {
        if (type === undefined || type === null)
            return null;

        try {
            const jsonData = JsonManager.makeFirstSensor(type);

            const res = await fetch('/FacilityType/RequestData/', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                const result = await res.json();

                return result;
            } else {
                let result = new Object();
                result.success = false;
                result.message = "FacilityType Controller 페이지를 찾을 수 없습니다. 네트워크를 확인해주세요.";

                return result;
            }
        }
        catch (e) {
            console.log(e);
        }

        return null;
    }

    static async getFacilityTypeSensors(type) {
        if (type === undefined || type === null)
            return null;

        try {
            const jsonData = JsonManager.makeFacilityTypeSensors(type);

            const res = await fetch('/FacilityType/RequestData/', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                const result = await res.json();

                return result;
            } else {
                let result = new Object();
                result.success = false;
                result.message = "FacilityType Controller 페이지를 찾을 수 없습니다. 네트워크를 확인해주세요.";

                return result;
            }
        }
        catch (e) {
            console.log(e);
        }

        return null;
    }

    static async getAlarmList(type) {
        if (type === null || type === undefined)
            return null;

        try {
            const jsonData = JsonManager.makeAlarmList(type);

            const res = await fetch('/FacilityType/RequestData/', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                const result = await res.json();

                return result;
            } else {
                let result = new Object();
                result.success = false;
                result.message = "FacilityType Controller 페이지를 찾을 수 없습니다. 네트워크를 확인해주세요.";

                return result;
            }
        } catch (e) {
            console.log(e);
        }

        return null;
    }

    static async getManualList(type) {
        if (type === null || type === undefined)
            return null;

        try {
            const jsonData = JsonManager.makeManualList(type);

            const res = await fetch('/FacilityType/RequestData/', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                const result = await res.json();

                return result;
            } else {
                let result = new Object();
                result.success = false;
                result.message = "FacilityType Controller 페이지를 찾을 수 없습니다. 네트워크를 확인해주세요.";

                return result;
            }
        } catch (e) {
            console.log(e);
        }

        return null;
    }
}