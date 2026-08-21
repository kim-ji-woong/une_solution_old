import { createStore } from 'redux';

export default createStore(function (state, action) {
    if (state === undefined) {
        return { weatherDatas: null }
    }
    else if (action.type === 'SENSOR_ALARM') {
        return {
            sensorAlarm: action.sensorAlarm,
            weatherDatas: state.weatherDatas,
            mobileUsers: state.mobileUsers,
            actionType: action.type
        }
    }
    else if (action.type === 'WEATHER_CURRENT') {
        return {
            sensorAlarm: state.sensorAlarm,
            weatherDatas: action.weatherDatas,
            mobileUsers: state.mobileUsers,
            actionType: action.type
        }
    }
    else if (action.type === 'MOBILE_USERS') {
        return {
            sensorAlarm: state.sensorAlarm,
            weatherDatas: state.weatherDatas,
            mobileUsers: action.mobileUsers,
            actionType: action.type
        }
    }

    return state;
}, window.__REDUX_DEVTOOLS_EXTENSION__ && window.__REDUX_DEVTOOLS_EXTENSION__())
