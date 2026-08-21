import DashboardJsonManager from './dashboardJsonManager';
import { SdmsJsonManager } from '../../SDMS/services/sdmsJsonManager';
import DashboardStore from '../dashboardStore';

export class DashboardController {
    static StartWatchTimer() {
        // 타이머 실행 유무 판단
        if (this.timerCheck == true)
            return;

        // 타이머 실행 체크
        this.timerCheck = true;

        // 1분에 한번씩 실행
        DashboardController.WatchWISH();
        let timerWatchWISH = setTimeout(function tick() {
            DashboardController.WatchWISH();
            timerWatchWISH = setTimeout(tick, 60000);
        }, 60000);
    }

    static async WatchWISH() {
        let [result, message] = await DashboardController.requestCurrentWorkPermit();

        if (result !== null && result !== undefined && result.length > 0) {
            let data = DashboardStore.getState().currentWork;

            if (data === null || data === undefined)
                DashboardStore.dispatch({ type: 'CURRENT_WORK', currentWork: result });
            else if (data.length > 0) {
                if (data[0].updateTime !== result[0].updateTime)
                    DashboardStore.dispatch({ type: 'CURRENT_WORK', currentWork: result });
            }
        }
    }

    static async requestCurrentWorkPermit() {
        try {
            const jsonData = DashboardJsonManager.makeRequestCurrentWorkPermit();

            const res = await fetch('Dashboard/Dashboard/RequestData', {
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
                    return [result.currentWorkPermits, ""];
                } else {
                    return [null, result.message];
                }
            }
        }
        catch (e) {
            console.log(e);
        }

        return [null, "requestCurrentWorkPermit 실패"];
    }

    static async requestUseSensor() {
        try {
            const jsonData = DashboardJsonManager.makeRequestUseSensor();

            const res = await fetch('Dashboard/Dashboard/RequestData', {
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
                    const sensorList = {};

                    sensorList.fireSensors = result.fireSensors;
                    sensorList.disabledFireSensors = result.disabledFireSensors;
                    sensorList.psmSensors = result.psmSensors;
                    sensorList.disabledPSMSensors = result.disabledPSMSensors;
                    sensorList.etcSensors = result.etcSensors;
                    sensorList.disabledEtcSensors = result.disabledEtcSensors;
                    sensorList.cctvs = result.cctVs;
                    sensorList.disabledCCTVs = result.disabledCCTVs;

                    return [sensorList, ""];
                } else {
                    return [null, result.message];
                }
            }
        }
        catch (e) {
            console.log(e);
        }

        return [null, "requestUseSensor 실패"];
    }

    static async requestWeeklyStatus() {
        try {
            const jsonData = DashboardJsonManager.makeRequestWeeklyStatus();

            const res = await fetch('Dashboard/Dashboard/RequestData', {
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

                    return [result.alarmInfos, ""];
                } else {
                    return [null, result.message];
                }
            }
        }
        catch (e) {
            console.log(e);
        }

        return [null, "requestWeeklyStatus 실패"];
    }

    static async requestWeatherWeeklyInfo() {
        try {
            const jsonData = DashboardJsonManager.makeRequestWeatherWeeklyInfo();

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


    static async requestGetSelectDay(userID) {
        try {
            const jsonData = DashboardJsonManager.makeRequestGetSelectDay(userID);

            const res = await fetch('Dashboard/Dashboard/RequestData', {
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

                    //return [result.alarmInfos, ""];
                } else {
                    return [null, result.message];
                }
            }
        }
        catch (e) {
            console.log(e);
        }

        return [null, "requestGetSelectDay 실패"];
    }
}