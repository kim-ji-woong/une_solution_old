import store from '../../Root/store';

export default class SopSimulatorController {
    static async DisplaySopRun() {
        try {
            const response = await fetch('SOPSimulator/SOPSimulator/DisplaySopRun', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json;charset=utf-8'
                }
            });

            if (response.ok && response.status !== 204) {
                const datas = await response.json();
                return datas;
            }
        } catch (e) {
            console.log(e);
        }
    }

    static async requestSiteID() {
        try {
            const response = await fetch('SOPSimulator/SOPSimulator/GetSiteID', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json;charset=utf-8'
                }
            });

            if (response.ok && response.status !== 204) {
                const datas = await response.json();
                return [datas.success, datas.message, datas.siteID];
            }
        } catch (e) {
            console.log(e);
        }

        return [false, "SiteID를 읽을수 없습니다.", -1];
    }

    static async WatchSopRun() {
        let sopHistory = await SopSimulatorController.DisplaySopRun();

        if (!sopHistory) {
            return;
        }
        
        // 현재
        //const storeState = store.getState();
        //if (storeState.actionType !== 'SOP_HISTORY') {
        //    return;
        //}
        let value = store.getState().sopHistory;
        let lastHistory = (!value) ? null : value.sopRunDatas;

        // 변경되었거나
        // 변경은 안되었지만 클라이언트가 처음 켜진 경우라서 data가 없을 때
        if (sopHistory.changed ||
            (value && sopHistory.nChanged !== value.nChanged) ||
            (!sopHistory.changed && lastHistory === null)) {
            store.dispatch({
                type: 'SOP_HISTORY',
                sopHistory: sopHistory
            });
        }
    }

    static StartWatchTimer() {
        // 타이머 실행 유무 판단
        if (this.timerCheck == true)
            return;

        // 타이머 실행 체크
        this.timerCheck = true;

        let timerId = setTimeout(function tick() {
            SopSimulatorController.WatchSopRun();
            timerId = setTimeout(tick, 500);
        }, 500);
    }

    static async excuteSOP(beginTime, actionStepID, position, userID, sensorZoneHistoryID) {
        try {
            const response = await fetch('SOPSimulator/SOPSimulator/ExcuteSOP', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json;charset=utf-8'
                },
                body: JSON.stringify({
                    BeginTime: beginTime,
                    ActionStepID: actionStepID,
                    Position: position,
                    LastAccessedUserID: userID,
                    SensorZoneHistoryID: sensorZoneHistoryID
                })
            });
            const actionStepHistoryID = await response.json();            
            return actionStepHistoryID; // result : ActionStepHistory ID
        } catch (e) {
            console.log(e);
        }
    }

    static async closeSOP(actionStepHistoryID, endTime, accessedUserID) {
        try {
            const response = await fetch('SOPSimulator/SOPSimulator/CloseSOPByUser', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json;charset=utf-8'
                },
                body: JSON.stringify({ ActionStepHistoryID: actionStepHistoryID, EndTime:endTime, LastAccessedUserID: accessedUserID })
            });
        } catch (e) {
            console.log(e);
        }
    }

    static async progressSOP(actionStepHistoryID, componentID, componentType, accessedUserID, status, text) {
        try {
            const response = await fetch('SOPSimulator/SOPSimulator/ProgressSOP', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json;charset=utf-8'
                },
                body: JSON.stringify({
                    ActionStepHistoryID: actionStepHistoryID,
                    ComponentID: componentID,
                    ComponentType: componentType,
                    AccessedUserID: accessedUserID,
                    Status: status,
                    Text: text
                })
            });

            const history = await response.json();
            return history; // result : ComponentHistory
        } catch (e) {
            console.log(e);
        }
    }

    static async runSection(sopKey, actionStepID, actionStepHistoryID, componentID, componentType, accessedUserID, text, decisionValue, isSkip) {
        try {
            if (!decisionValue)
                decisionValue = '';

            const response = await fetch('SOPSimulator/SOPSimulator/RunSection', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json;charset=utf-8'
                },
                body: JSON.stringify({
                    SopKey: sopKey,
                    ActionStepID: actionStepID,
                    ActionStepHistoryID: actionStepHistoryID,
                    ComponentID: componentID,
                    ComponentType: componentType,
                    AccessedUserID: accessedUserID,
                    DecisionValue: decisionValue,
                    Text: text,
                    Skip:isSkip
                })
            });

            const history = await response.json();
            return history; // result : ComponentHistory
        } catch (e) {
            console.log(e);
        }
    }

    static async monitorComponentHistory() {
        try {
            const response = await fetch('SOPSimulator/SOPSimulator/MonitorComponentHistory');
            const datas = await response.json();
            
            return datas;
        } catch (e) {
            console.log(e);
        }
    }

    static async progressMission(sopKey, actionStepHistoryID, componentType, componentID, dataIndex, componentStatus, userID, checked) {
        try {
            const response = await fetch('SOPSimulator/SOPSimulator/ProgressMission', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json;charset=utf-8'
                },
                body: JSON.stringify({
                    SopKey: sopKey,
                    ActionStepHistoryID: actionStepHistoryID,
                    ComponentType: componentType,
                    ComponentID: componentID,
                    DataIndex: dataIndex,
                    ComponentStatus: componentStatus,
                    AccessedUserID: userID,
                    Checked: checked
                })
            });

            const detail = await response.json();
            return detail; // result : ComponentHistoryDetail
        } catch (e) {
            console.log(e);
        }
    }

    static async progressSpread(sopKey, actionStepHistoryID, componentType, componentID, dataIndex, componentStatus, userID, isSMS, isEmail, isBroadcast, isSiren) {
        try {
            const response = await fetch('SOPSimulator/SOPSimulator/ProgressSpread', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json;charset=utf-8'
                },
                body: JSON.stringify({
                    SopKey: sopKey,
                    ActionStepHistoryID: actionStepHistoryID,
                    ComponentType: componentType,
                    ComponentID: componentID,
                    DataIndex: dataIndex,
                    ComponentStatus: componentStatus,
                    AccessedUserID: userID,
                    IsSMS: isSMS,                    
                    IsEmail: isEmail,
                    IsBroadcast: isBroadcast,
                    IsSiren: isSiren
                })
            });

            const detail = await response.json();
            return detail; // result : 
        } catch (e) {
            console.log(e);
        }
    }

    static async requestSensorName(sensorZoneHistoryID) {
        try {
            const response = await fetch('SOPSimulator/SOPSimulator/RequestSensorName', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json;charset=utf-8'
                },
                body: JSON.stringify({
                    SensorZoneHistoryID: sensorZoneHistoryID
                })
            });

            const value = await response.json();
            return value.sensorName; // result : Disaster ID
        } catch (e) {
            console.log(e);
        }
    }

    static async setSopParameter(params) {
        try {
            if (!params) {
                return;
            }

            const paramString = params.startsWith('?') ? params.substring(1).trim() : params.trim();
            const parameters = paramString.split('&');
            const paramCount = parameters.length;

            const targetName = "sopParameter";

            const data = {
                targetName: {
                }
            };

            const target = data[targetName];

            for (let i = 0; i < paramCount; i++) {
                const parameter = parameters[i].split('=');

                if (parameter.length !== 2) {
                    continue;
                }

                const paramName = parameter[0].toLowerCase().trim();
                const paramValue = parameter[1].trim();

                if (paramName === 'accessmode') {
                    target['accessMode'] = paramValue;
                }
                else if (paramName === 'accesstoken') {
                    target['accessToken'] = paramValue;
                }
                else if (paramName === 'servicetype') {
                    target['serviceType'] = paramValue;
                }
                else if (paramName === 'location') {
                    target['location'] = paramValue;
                }
            }

            const response = await fetch('SOP/Param/RequestData', {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data)
            });
        }
        catch (e) {
            console.log(e);
        }
    }

    static async runSOP(beginCode) {
        try {
            const data = {
                "runSOP": {
                    "beginCode": beginCode
                }
            };

            const res = await fetch('SOP/Param/RequestData', {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data)
            });

            if (res !== null && res.ok) {
                const response = await res.json();
                return [response.success, response.message, response.actionStepHistoryID, response.accessMode, response.accessToken, response.serviceType, response.siteID];
            }
        }
        catch (e) {
            console.log(e);
        }

        return [false, "SOP를 실행할 수 없습니다.", null, null, null, null, null];
    }
}