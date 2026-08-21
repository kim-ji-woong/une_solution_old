import { SdmsJsonManager } from './sdmsJsonManager';
//import SessionString from '../../Common/js/sessionString';
import * as SdmsCommon from '../data/common';
import * as Backend from '../data/backend';
import * as Frontend from '../data/frontend';
import * as Common from '../../Common/data/common';
import store from '../../Root/store';

export class SDMSController {
    // 센서 히스토리 불러오기
    static async DisplayAlarm() {
        try {
            const response = await fetch('SDMS/SDMS/DisplayAlarm', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json;charset=utf-8'
                }
            });

            if (response.ok && response.status !== 204) {
                const data = await response.json();
                return data;
            }
        } catch (e) {
            console.log(e);
        }

        return null;
    }

    // ModelViewer로 동작하는가?
    static async isModelViewer() {
        try {
            const response = await fetch('SDMS/SDMS/IsModelViewer', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json;charset=utf-8'
                }
            });

            if (response.ok && response.status !== 204) {
                const data = await response.json();
                return data.isModelViewer;
            }
        } catch (e) {
            console.log(e);
        }

        return false;
    }

    static timerCheck = false;

    static async requestBuildingGroupList()/*: Promise<[Array<Backend.BuildingGroup> | null, Array<Backend.Zone> | null, string]>*/ {
        try {
            const jsonData = SdmsJsonManager.makeRequestBuildingGroupList();

            const res = await fetch('SDMS/SDMS/RequestData', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                const result = await res.json();

                if (result.success) {
                    return [result.buildingGroups, result.outdoorZones, ""];
                }
                else {
                    return [null, null, result.message];
                }
            }
        }
        catch (e) {
            console.log(e);
        }

        return [null, null, ""];
    }

    static async requestOuterDatas() {
        try {
            const jsonData = SdmsJsonManager.makeRequestOuterDatas();

            const res = await fetch('SDMS/SDMS/RequestData', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                const result = await res.json();

                if (result.success) {
                    /*const outdoorZoneCount = result.outdoorZones.length;

                    for (let i = 0; i < outdoorZoneCount; i++) {
                        const zone = result.outdoorZones[i];

                        if (zone.sensors?.cctvs) {
                            SDMSDataManager.checkCCTVTypes(zone.sensors.cctvs);
                        }
                    }*/

                    return [result.buildingGroups, result.outdoorZones, ""];
                }
                else {
                    return [null, null, result.message];
                }
            }
        }
        catch (e) {
            console.log(e);
        }

        return [null, null, ""];
    }

    static async requestIndoorDatas(zoneID) {
        try {
            const jsonData = SdmsJsonManager.makeRequestIndoorDatas(zoneID);

            const res = await fetch('SDMS/SDMS/RequestData', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                const result = await res.json();

                if (result.success) {
                    //SDMSDataManager.checkCCTVTypes(result.cctvs);
                    return [result, ""];
                }
                else {
                    return [null, result.message];
                }
            }
        }
        catch (e) {
            console.log(e);
        }

        return [null, "requestIndoorDatas 실패"];
    }

    static async requestGltfModelList()/*: Promise<[Array<SdmsCommon.GltfModel> | null, SdmsCommon.GltfOption | null, string]>*/ {
        try {
            const jsonData = SdmsJsonManager.makeRequestGltfDataList();

            const res = await fetch('SDMS/SDMS/RequestData', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                const result = await res.json();

                if (result.success) {
                    return [result.models, result.gltfOption, ""];
                }
                else {
                    return [null, null, result.message];
                }
            }
        }
        catch (e) {
            console.log(e);
        }

        return [null, null, "requestGltfModelList 실패"];
    }

    static async requestSaveIndoorModelViewport(modelName/*: string*/, cameraData/*: Frontend.PerspectiveCameraData2*/)/*: Backend.MessageResult | null*/ {
        try {
            const jsonData = SdmsJsonManager.makeRequestSaveIndoorModelViewport(modelName, cameraData);

            const res = await fetch('SDMS/SDMS/RequestData', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                const result = await res.json();
                return result/* as Backend.MessageResult*/;
            }

        } catch (e) {
            console.log(e);
        }

        return null;
    }

    static async getStreamServerURL()/*: Promise<Common.NullableString>*/ {
        try {
            const jsonData = SdmsJsonManager.makeRequestStreamServerURL();

            const res = await fetch('SDMS/SDMS/GetStreamServerURL', {
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
            }

        } catch (e) {
            console.log(e);
        }

        return null;
    }

    //옵션 획득(list)
    static async requestGetOption(UserID/*: string*/, Category/*: string*/)/*: Promise<[true, Array<Backend.AccountOption>] | [false, string]>*/ {
        try {
            const jsonData = SdmsJsonManager.makeRequestGetOption(UserID, Category);

            const res = await fetch('SOPManager/SOP/RequestData', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                const result = await res.json();
                if (result.success) {
                    return [true, result.options];
                } else {
                    return [false, result.message];
                }
            }
        } catch (e) {
            console.log(e);
        }

        return [false, 'requestGetOption 실패'];
    }

    //옵션 저장
    static async requestSaveOption(ID/*: number*/, UserID/*: number*/, Category/*: string*/, SubCategory/*: Common.NullableString*/, PropertyValue1/*: Common.NullableString*/, PropertyValue2/*: Common.NullableString*/, PropertyValue3/*: Common.NullableString*/, PropertyValue4/*: Common.NullableString*/)/*: Promise<[true, Array<Backend.AccountOption>] | [false, string]>*/ {
        try {
            const jsonData = SdmsJsonManager.makeRequestSaveOption(ID, UserID, Category, SubCategory, PropertyValue1, PropertyValue2, PropertyValue3, PropertyValue4);
            const res = await fetch('SOPManager/SOP/RequestData', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                const result = await res.json();
                //데이터가 성공적으로 삽입 되면 primary id를 반환 받는다.
                if (result.success) {
                    return [true, result.options]
                } else {
                    return [false, result.message];
                }
            }

        } catch (e) {
            console.log(e);
        }
        return [false, 'requestSaveOption 실패'];
    }

    static async requestSensorList() {
        try {
            const jsonData = SdmsJsonManager.makeRequestSensorList();

            const res = await fetch('SDMS/SDMS/RequestData', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });


            if (res.ok) {
                const result = await res.json();

                if (result.success) {
                    return [result, ""];
                }
                else {
                    return [null, result.message];
                }
            }
        }
        catch (e) {
            console.log(e);
        }

        return [null, "requestSensorList 실패"];
    }

    static StartWatchTimer() {
        // 타이머 실행 유무 판단
        if (SDMSController.timerCheck === true)
            return;

        // 타이머 실행 체크
        SDMSController.timerCheck = true;

        let timerAlarm = setTimeout(function tick() {
            SDMSController.WatchSensorAlarm();
            timerAlarm = setTimeout(tick, 1500);
        }, 1500);

        // 1분에 한번씩 실행
        SDMSController.WatchWeather();
        let timerWeather = setTimeout(function tick() {
            SDMSController.WatchWeather();
            timerWeather = setTimeout(tick, 60000);
        }, 60000);

        // 5초에 한번씩 실행
        SDMSController.WatchMobileUsers();
        let timerMobileUsers = setTimeout(function tick() {
            SDMSController.WatchMobileUsers();
            timerMobileUsers = setTimeout(tick, 5000);
        }, 5000);
    }

    static async WatchSensorAlarm() {
        // 센서 알람 히스토리 조회
        let result = await SDMSController.DisplayAlarm();
        result = result == null ? new Array() : result;
        if (result == null) {
            return new Array();
        }

        // 현재 센서 알람 히스토리 조회
        SDMSController.toCompareAlarm('SENSOR_ALARM', result);
    }

    static toCompareAlarm(type, result) {
        let currentAlarm = null;
        if (type === 'SENSOR_ALARM') {
            currentAlarm = store.getState().sensorAlarm;
        }
        else {
            return;
        }

        let receiveAlarm = result.allAlarmDatas;
        let temp = null; // 센서 비교에 사용

        currentAlarm = currentAlarm == null ? new Array() : currentAlarm;
        temp = currentAlarm.slice(); // 얕은 복사

        // 조회된 센서 알람와 표시되고 있는 센서 알람 비교 후 Redux에 저장
        if (receiveAlarm !== null && receiveAlarm !== undefined && receiveAlarm.length != currentAlarm.length) {
            // 알람 수가 같지 않을 때
            if (type === 'SENSOR_ALARM') {
                store.dispatch({ type: type, sensorAlarm: receiveAlarm });
            }
        } else if (receiveAlarm !== null && receiveAlarm !== undefined && receiveAlarm.length == currentAlarm.length && receiveAlarm.length != 0) {
            const receiveAlarmCount = receiveAlarm.length;
            for (let i = 0; i < receiveAlarmCount; i++) {
                for (let j = 0; j < temp.length; j++) {
                    // id 비교 같으면 삭제
                    if (receiveAlarm[i].dtTime == temp[j].dtTime &&
                        receiveAlarm[i].equipZoneID == temp[j].equipZoneID &&
                        receiveAlarm[i].sopStatus == temp[j].sopStatus &&
                        receiveAlarm[i].alarmDepth == temp[j].alarmDepth &&
                        receiveAlarm[i].isAlarm == temp[j].isAlarm) {
                        temp.splice(j, 1);
                        break;
                    }
                }
            }

            // currentAlarm 갯수가 남아있다면 >> 센서 알람이 동일하지 않음.
            if (temp.length != 0) {
                if (type === 'SENSOR_ALARM') {
                    store.dispatch({ type: type, sensorAlarm: receiveAlarm });
                }
            }
        }
    }

    static async WatchWeather() {
        let result = await SDMSController.requestWeatherInfo();
        result = result === null || result.success === false ? [] : result.datas;
        store.dispatch({ type: 'WEATHER_CURRENT', weatherDatas: result });
    }

    static async requestWeatherInfo() {
        try {
            const jsonData = SdmsJsonManager.makeRequestWeatherInfo();

            const res = await fetch('Weather/Weather/RequestData', {
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
            }

        } catch (e) {
            console.log(e);
        }

        return null;
    }

    static async requestWeatherWeeklyInfo() {
        try {
            const jsonData = SdmsJsonManager.makeRequestWeatherWeeklyInfo();

            const res = await fetch('Weather/Weather/RequestData', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                const result = await res.json();

                if (result.success) {
                    return [result.datas, ""];
                } else {
                    return [null, result.message];
                }
            }
        }
        catch (e) {
            console.log(e);
        }

        return [null, "requestWeatherWeeklyInfo 실패"];
    }

    static async requestMalfunction(sensorType, sensorZoneID, accessedUserID, isMalfunction) {
        try {
            const jsonData = SdmsJsonManager.makeRequestMalfunction(sensorType, sensorZoneID, accessedUserID, isMalfunction);

            const res = await fetch('SDMS/SDMS/RequestData', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });
        }
        catch (e) {
            console.log(e);
        }
    }

    static async requestClearManualReport(sensorType, sensorZoneID, sensorZoneHistoryID, accessedUserID) {
        try {
            const jsonData = SdmsJsonManager.makeRequestClearManualReport(sensorType, sensorZoneID, sensorZoneHistoryID, accessedUserID);

            const res = await fetch('SDMS/SDMS/RequestData', {
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
            }

        } catch (e) {
            console.log(e);
        }

        return null;
    }

    static async WatchMobileUsers() {
        let result = await SDMSController.requestMobileUserList();
        result = result === null || result.success === false ? [] : result.userList;
        store.dispatch({ type: 'MOBILE_USERS', mobileUsers: result });
    }

    static async requestMobileUserList() {
        try {
            const jsonData = SdmsJsonManager.makeRequestMobileUserList();

            const res = await fetch('Safety/Safety/RequestData', {
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
            }

        } catch (e) {
            console.log(e);
        }

        return null;
    }

    static async requestRegulars() {
        try {
            const jsonData = SdmsJsonManager.makeRequestRegulars();

            const res = await fetch('/TeamEditor/TeamEdit/RequestData', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                const result = await res.json();

                if (result.success) {
                    return [result.regulars, ""];
                }
                else {
                    return [null, result.message];
                }
            }
        }
        catch (e) {
            console.log(e);
        }

        return [null, "requestRegulars 실패"];
    }

    static async requestRegularMembers() {
        try {
            const jsonData = SdmsJsonManager.makeRequestRegularMembers();

            const res = await fetch('/TeamEditor/TeamEdit/RequestData', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                const result = await res.json();

                if (result.success) {
                    return [result.regularMembers, ""];
                }
                else {
                    return [null, result.message];
                }
            }
        }
        catch (e) {
            console.log(e);
        }

        return [null, "requestRegularMembers 실패"];
    }
}