import { createStore } from 'redux';

export default createStore(function (state, action) {
    if (state === undefined) {
        return { sensorAlarm: null }
    }
    else if (action.type === 'SENSOR_ALARM') { // 개수 제한한 알람 리스트
        return {
            sensorAlarm: action.sensorAlarm,
            sensorAllAlarm: action.sensorAllAlarm,
            sensorCount: state.sensorCount,
            sopHistory: state.sopHistory,
            weatherDatas: state.weatherDatas,
            newCCTVList: state.newCCTVList,
            //sdmsCommonSettings: state.sdmsCommonSettings,
            //sopCommonSettings: state.sopCommonSettings,
            actionType: action.type
        }
    }
    else if (action.type === 'SENSOR_COUNT') {
        return {
            sensorAlarm: state.sensorAlarm,
            sensorAllAlarm: state.sensorAllAlarm,
            sensorCount: action.sensorCount,
            sopHistory: state.sopHistory,
            weatherDatas: state.weatherDatas,
            newCCTVList: state.newCCTVList,
            //sdmsCommonSettings: state.sdmsCommonSettings,
            //sopCommonSettings: state.sopCommonSettings,
            actionType: action.type
        }
    }
    else if (action.type === 'SOP_HISTORY') {
        return {
            sensorAlarm: state.sensorAlarm,
            sensorAllAlarm: state.sensorAllAlarm,
            sensorCount: state.sensorCount,
            sopHistory: action.sopHistory,
            weatherDatas: state.weatherDatas,
            newCCTVList: state.newCCTVList,
            //sdmsCommonSettings: state.sdmsCommonSettings,
            //sopCommonSettings: state.sopCommonSettings,
            actionType: action.type
        }
    }
    else if (action.type === 'WEATHER_CURRENT') {
        return {
            sensorAlarm: state.sensorAlarm,
            sensorAllAlarm: state.sensorAllAlarm,
            sensorCount: state.sensorCount,
            sopHistory: state.sopHistory,
            weatherDatas: action.weatherDatas,
            newCCTVList: state.newCCTVList,
            //sdmsCommonSettings: state.sdmsCommonSettings,
            //sopCommonSettings: state.sopCommonSettings,
            actionType: action.type
        }
    }
    else if (action.type === 'NEW_CCTV_LIST') {
        return {
            sensorAlarm: state.sensorAlarm,
            sensorAllAlarm: state.sensorAllAlarm,
            sensorCount: state.sensorCount,
            sopHistory: state.sopHistory,
            weatherDatas: state.weatherDatas,
            newCCTVList: action.newCCTVList,
            //sdmsCommonSettings: state.sdmsCommonSettings,
            //sopCommonSettings: state.sopCommonSettings,
            actionType: action.type
        }
    }
    /*
    else if (action.type === 'SDMS_COMMON_SETTINGS') {
        return {
            sensorAlarm: state.sensorAlarm,
            sensorAllAlarm: state.sensorAllAlarm,
            sensorCount: state.sensorCount,
            sopHistory: state.sopHistory,
            weatherDatas: state.weatherDatas,
            newCCTVList: state.newCCTVList,
            sdmsCommonSettings: action.sdmsCommonSettings,
            sopCommonSettings: state.sopCommonSettings,
            actionType: action.type
        }
    }
    else if (action.type === 'SOP_COMMON_SETTINGS') {
        return {
            sensorAlarm: state.sensorAlarm,
            sensorAllAlarm: state.sensorAllAlarm,
            sensorCount: state.sensorCount,
            sopHistory: state.sopHistory,
            weatherDatas: state.weatherDatas,
            newCCTVList: state.newCCTVList,
            sdmsCommonSettings: state.sdmsCommonSettings,
            sopCommonSettings: action.sopCommonSettings,
            actionType: action.type
        }
    }
    */

    return state;
}, window.__REDUX_DEVTOOLS_EXTENSION__ && window.__REDUX_DEVTOOLS_EXTENSION__())
