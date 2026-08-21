export default class JsonManager{
    static makeRequestSettings(userID) {
        const json = {
            "requestSettings":
            {
                "userID": userID,
            }
        };

        return JSON.stringify(json);
    }

    static makeRequestSdmsCommonSettings() {
        const json = {
            "requestSdmsCommonSettings": true
        };

        return JSON.stringify(json);
    }

    static makeRequestSopCommonSettings() {
        const json = {
            "requestSopCommonSettings": true
        };

        return JSON.stringify(json);
    }

    static makeRequestAccountSettings(userID) {
        const json = {
            "requestAccountSettings":
            {
                "userID": userID,
            }
        };

        return JSON.stringify(json);
    }

    static makeSaveSettings(saveData) {
        const json = {
            "requestSaveSettings":
            {
                "userID": saveData.userID,
                "shortcutKey": saveData.shortcutKey,
                "idleTime": saveData.idleTime,
                "reAlarm": saveData.reAlarm,
                "useReceiveFire": saveData.useReceiveFire,
                "useReceivePSM": saveData.useReceivePSM,
                "useReceiveETC": saveData.useReceiveETC,
                "useReceiveSVMS": saveData.useReceiveSVMS,
                "eventInfoDisplayTerm": saveData.eventInfoDisplayTerm,
                "useScreenMove": saveData.useScreenMove,
                "exeCautionSOP": saveData.exeCautionSOP,
                "exeAlartSOP": saveData.exeAlartSOP,
                "exeSeriousSOP": saveData.exeSeriousSOP,
                "useTrainingMode": saveData.useTrainingMode,
                "useWaterMark": saveData.useWaterMark,
                "useHeadMessage": saveData.useHeadMessage,
                "useAutoMoveSOPScreen": saveData.useAutoMoveSOPScreen,
                "useBroadcast": saveData.useBroadcast,
                "useSMS": saveData.useSMS,
                "useEmail": saveData.useEmail,
                "useConfirm": saveData.useConfirm,
                "workingBeginHour": saveData.workingBeginHour,
                "workingEndHour": saveData.workingEndHour,
                "useResultSummary": saveData.useResultSummary,
                "dashboardBegin": saveData.dashboardBegin,
                "dashboardEnd": saveData.dashboardEnd,
                "fireSOPWaitEndTime": saveData.fireSOPWaitEndTime,
                "psmsopWaitEndTime": saveData.psmsopWaitEndTime,
                "etcsopWaitEndTime": saveData.etcsopWaitEndTime,
                "fireSOPRecoverEndTime": saveData.fireSOPRecoverEndTime,
                "psmsopRecoverEndTime": saveData.psmsopRecoverEndTime,
                "etcsopRecoverEndTime": saveData.etcsopRecoverEndTime,
                "moveDisplayAlarm": saveData.moveDisplayAlarm,
                "useAlarmBroadcast": saveData.useAlarmBroadcast,
                "usePoiFocus": saveData.usePoiFocus,
                "usePoiHighlight": saveData.usePoiHighlight,
                "turnStart": saveData.turnStart,
                "useAlarmTurn": saveData.useAlarmTurn,
            }
        };

        return JSON.stringify(json);
    }

    static makeSaveSetting(propertyName, propertyValue) {
        const json = {
            "requestSaveSetting":
            {
                "PropertyName": propertyName,
                "PropertyValue": propertyValue,
            }
        };

        return JSON.stringify(json);
    }

    static makeUpdateSdmsSettings(settings) {
        const json = {
            "requestUpdateSettings": {
                "properties": settings,
                // Options.OptionTarget.SDMS
                "optionTarget": 0
            }
        };

        return JSON.stringify(json);
    }

    static makeRequestResetPopup(userID, popupState) {
        const json = {
            "requestResetPopup":
            {
                "userID": userID,
                "popupState": popupState,
            }
        };

        return JSON.stringify(json);
    }

    static makeRequestDownloadBuilding() {
        const json = {
            "requestDownloadBuilding": true
        };

        return JSON.stringify(json);
    }

    static makeRequestDownloadBuildingGroup() {
        const json = {
            "requestDownloadBuildingGroup": true
        };

        return JSON.stringify(json);
    }

    static makeRequestDownloadFacility() {
        const json = {
            "requestDownloadFacility": true
        };

        return JSON.stringify(json);
    }

    static makeRequestDownloadRegularTeam() {
        const json = {
            "requestDownloadRegularTeam": true
        };

        return JSON.stringify(json);
    }

    static makeRequestGetSpreadMessage() {
        const json = {
            "requestGetSpreadMessage": true
        };

        return JSON.stringify(json);
    }

    static makeRequestSetSpreadMessage(addSpreadMessage, updateSpreadMessage, removeSpreadMessage) {
        const json = {
            "requestSetSpreadMessage":
            {
                "addSpreadMessage": addSpreadMessage,
                "updateSpreadMessage": updateSpreadMessage,
                "removeSpreadMessage": removeSpreadMessage,
            }
        };

        return JSON.stringify(json);
    }

    static makeRequestLinkedSOPs() {
        const json = {
            "requestLinkedSOPs": true
        };

        return JSON.stringify(json);
    }
    
    static makeRequestUpdateLinkedSOPs(addLinkedSOP, updateLinkedSOP, removeLinkedSOP) {
        const json = {
            "requestUpdateLinkedSOPs":
            {
                "addLinkedSOPs": addLinkedSOP,
                "updateLinkedSOPs": updateLinkedSOP,
                "removeLinkedSOPs": removeLinkedSOP,
            }
        };

        return JSON.stringify(json);
    }

    static makeRequestSetAccoutPopup(userID) {
        const json = {
            "requestSetAccoutPopup":
            {
                "userID": userID,
            }
        };

        return JSON.stringify(json);
    }

    static makeRequestResetAccoutPopup(userID) {
        const json = {
            "requestResetAccoutPopup":
            {
                "userID": userID,
            }
        };

        return JSON.stringify(json);
    }

    static makeRequestOnOffBroadcast(onOff, buildingID) {
        const json = {
            "requestOnOffBroadcast":
            {
                "onOff": onOff,
                "buildingID": buildingID,
            }
        };

        return JSON.stringify(json);
    }
}