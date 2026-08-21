import { createStore } from 'redux';

export default createStore(function (state, action) {
    if (state === undefined) {
        return { sensorInfo: null }
    }
    else if (action.type === 'SENSOR_INFO') {
        return { ...state, sensorInfo: action.sensorInfo }
    }

    return state;
}, window.__REDUX_DEVTOOLS_EXTENSION__ && window.__REDUX_DEVTOOLS_EXTENSION__())