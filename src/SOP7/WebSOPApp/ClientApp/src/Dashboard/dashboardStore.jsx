import { createStore } from 'redux';

export default createStore(function (state, action) {
    if (state === undefined) {
        return {
            currentWork: null,
        }
    }
    else if (action.type === 'CURRENT_WORK') {
        return {
            currentWork: action.currentWork,
            actionType: 'CURRENT_WORK',
        }
    }

    return state;
}, window.__REDUX_DEVTOOLS_EXTENSION__ && window.__REDUX_DEVTOOLS_EXTENSION__())
