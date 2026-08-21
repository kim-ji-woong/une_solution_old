import JsonManager from "./jsonManager";

export default class SopController {
    /*static async openDB(disasterID = null) {
        if (disasterID) {
            let actionStepDatas = null;

            await fetch('SOPManager/SOP/Open?disasterID=' + disasterID)
                .then(function (response) {
                    return response.json();
                })
                .then((json) => actionStepDatas = json);

            if (actionStepDatas) {
                SopController.setArrowSections(actionStepDatas);
            }

            return actionStepDatas;
        }
        else {
            let disasterCategories = null;

            await fetch('SOPManager/SOP/DisasterCategories')
                .then(function (response) {
                    return response.json();
                })
                .then((json) => disasterCategories = json);

            return disasterCategories;
        }
    }*/

    // 화살표와 Section 정보를 연결시켜준다.
    static setArrowSections(actionStepDatas) {
        const actionStepCount = actionStepDatas.length;

        for (let i = 0; i < actionStepCount; i++) {
            const actionStepData = actionStepDatas[i];

            if (actionStepData?.actionStep) {
                if (actionStepData.stepMemberDatas) {
                    const stepMemberCount = actionStepData.stepMemberDatas.length;

                    for (let j = 0; j < stepMemberCount; j++) {
                        const stepMemberData = actionStepData.stepMemberDatas[j];

                        if (stepMemberData) {
                            SopController.setArrowSections(stepMemberData.arrows, stepMemberData.sections);
                        }
                    }
                }
            }
        }
    }

    static setArrowSections(arrows, sections) {
        if (arrows && sections) {
            const arrowCount = arrows.length;

            for (let i = 0; i < arrowCount; i++) {
                const arrow = arrows[i];

                const [beginSectionType, beginSectionID] = SopController.getSectionInfo(arrow.beginComponentID);
                const [endSectionType, endSectionID] = SopController.getSectionInfo(arrow.endComponentID);


            }
        }
    }

    static findSection(componentID, sections) {
        const [sectionType, sectionID] = SopController.getSectionInfo(componentID);
        const sectionCount = sections.length;

        for (let i = 0; i < sectionCount; i++) {
            const section = sections[i];

            console.log("...");
        }
    }

    static getSectionInfo(componentID) {
        const sectionType = (componentID >> 24);
        const sectionID = (componentID & 0xffffff);
        return [sectionType, sectionID];
    }

    /*static async saveDB(sopData) {
        const data = JsonManager.fromSOPData(sopData);
        const jsonData = JSON.stringify(data, JsonManager.replacer);

        const response = await fetch('SOPManager/SOP/Save', {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: jsonData
        });
    }*/

    static async disasterCategories(isNormal) {
        try {
            const jsonData = JsonManager.makeRequestDisasterCategories(isNormal);

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
                    return [result.disasterCategoryDatas, ""];
                }
                else {
                    return [null, result.message];
                }
            }
        }
        catch (e) {
            console.log(e);
        }

        return [null, ""];
    }

    static async requestDefaultStepMemberData(actionStep) {
        try {
            const jsonData = JsonManager.makeRequestStepMemberData();

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
                    if (actionStep.stepMemberDatas) {
                        actionStep.stepMemberDatas.push(result.stepMemberData);
                    }
                    else {
                        actionStep.stepMemberDatas = [result.stepMemberData];
                    }

                    return [result.stepMemberData, ""];
                }
                else {
                    return [null, result.message];
                }
            }
        }
        catch (e) {
            console.log(e);
        }

        return [null, ""];
    }

    static async requestDefaultActionStepDatas() {
        try {
            const jsonData = JsonManager.makeRequestActionStepDatas();

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
                    return [result.actionStepDatas, ""];
                }
                else {
                    return [null, result.message];
                }
            }
        }
        catch (e) {
            console.log(e);
        }

        return [null, ""];
    }

    // 특정 Disaster에 대한 버전 리스트를 얻어온다.
    static async requestDisasterVersions(sopData, isNormal) {
        try {
            const jsonData = JsonManager.makeRequestDisasterVersions(sopData, isNormal);

            if (jsonData === null) {
                return [null, "SOP 정보가 존재하지 않습니다."];
            }

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

        return [null, ""];
    }

    static async requestOpenXML(file) {
        try {
            const formData = new FormData();
            formData.append('textFile', file);

            const res = await fetch('SOPManager/SOP/OpenXML', {
                method: 'post',
                body: formData
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

        return [null, ""];
    }

    static async requestSaveXML(userID, sopData) {
        try {
            const jsonData = JsonManager.makeRequestSaveXML(userID, sopData);

            if (jsonData === null) {
                return [null, "올바르지 않은 SOP 데이터입니다."];
            }

            const res = await fetch('SOPManager/SOP/RequestData', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                if (res.headers.get('content-type') === 'text/xml') {
                    await SopController.downloadFile(res);
                    return [sopData, ""];
                }
                else {
                    const result = await res.json();

                    if (result.success) {
                        return [result, ""];
                    }
                    else {
                        return [null, result.message];
                    }
                }
            }
        }
        catch (e) {
            console.log(e);
        }

        return [null, ""];
    }

    static async downloadFile(response) {
        const fileName = SopController.getFileName(response);

        if (fileName.length === 0) {
            return;
        }

        const blob = await response.blob();
        const newBlob = new Blob([blob]);

        const blobUrl = window.URL.createObjectURL(newBlob);

        const link = document.createElement('a');
        link.href = blobUrl;
        link.setAttribute('download', fileName);
        document.body.appendChild(link);
        link.click();
        link.parentNode.removeChild(link);

        window.URL.revokeObjectURL(blob);
    }

    static getFileName(response) {
        const result = response.headers.get('content-disposition');
        const tokens = result.split(';');

        const tokenCount = tokens.length;

        for (let i = 0; i < tokenCount; i++) {
            const token = tokens[i].trim();
            const index = token.indexOf('=');

            if (index > 0) {
                const key = token.substring(0, index).trim();
                const value = token.substring(index + 1).trim();

                if (key === 'filename*') {
                    const index2 = value.indexOf("''");

                    if (index2 >= 0) {
                        const uri = value.substring(index2 + 2).trim();
                        return decodeURI(uri);
                    }
                }
            }
        }

        return "";
    }

    static async requestSaveDB(userID, sopData) {
        try {
            const jsonData = JsonManager.makeRequestSaveDB(userID, sopData);

            if (jsonData === null) {
                return [null, "올바르지 않은 SOP 데이터입니다."];
            }

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

        return [null, ""];
    }

    static async requestOpenDB(versionID) {
        try {
            const jsonData = JsonManager.makeRequestOpenDB(versionID);

            if (jsonData === null) {
                return [null, "올바르지 않은 SOP 데이터입니다."];
            }

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

        return [null, ""];
    }

    static async requestDeleteDB(versionIDs) {
        try {
            const jsonData = JsonManager.makeRequestDeleteDB(versionIDs);

            if (jsonData === null) {
                return [null, "올바르지 않은 데이터 형식입니다."];
            }

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
                return [result.success, result.message];
            }
        }
        catch (e) {
            console.log(e);
            return [false, e.message];
        }

        return [false, ""];
    }

    static async requestExternalPrograms() {
        try {
            const jsonData = JsonManager.makeRequestExternalPrograms();

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

        return [null, ""];
    }

    static addStepMember(disasterCategories, stepMemberData) {
        for (let i = 0; i < disasterCategories.length; i++) {
            const dc = disasterCategories[i];

            for (let j = 0; j < dc.subDisasterCategories.length; j++) {
                const sdc = dc.subDisasterCategories[j];

                for (const key in sdc.disasters) {
                    const disasterDatas = sdc.disasters[key];

                    if (disasterDatas.length > 0) {
                        const disasterData = disasterDatas[0];

                        if (disasterData.actionSteps) {
                            for (let k = 0; k < disasterData.actionSteps.length; k++) {
                                const actionStepData = disasterData.actionSteps[k];

                                if (actionStepData === null) {
                                    continue;
                                }

                                if (actionStepData.stepMemberDatas) {
                                    actionStepData.stepMemberDatas.push({ ...stepMemberData });
                                }
                                else {
                                    actionStepData.stepMemberDatas = [{ ...stepMemberData }];
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    static removeVersion(disasterCategories) {
        for (let i = 0; i < disasterCategories.length; i++) {
            const dc = disasterCategories[i];

            for (let j = 0; j < dc.subDisasterCategories.length; j++) {
                const sdc = dc.subDisasterCategories[j];

                for (const key in sdc.disasters) {
                    const disasterDatas = sdc.disasters[key];

                    // 새로운 SOP를 생성하는 것이니 Disaster 정보는 하나만 남기고 지운다.
                    disasterDatas.splice(1);

                    /*if (disasterDatas.length > 0) {
                        const disasterData = disasterDatas[0];
    
                        disasterData.disaster.versionID = -1;
                        disasterData.version = this.initVersion(disasterData.version);
                    }*/
                }
            }
        }
    }

    /**
     * @param : SpecialMessageParameter
     */
    static async requestParseSpecialMessage(param) {
        try {
            const jsonData = JsonManager.makeRequestParseSpecialMessage(param);

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
                    return [result.parseMessage, null];
                }
                else {
                    return [null, result.message];
                }
            }
        }
        catch (e) {
            console.log(e);
        }

        return [null, ""];
    }

    static async requestSpecialMessageList() {
        try {
            const jsonData = JsonManager.makeRequestSpecialMessageList();

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
                    return [result.specialMessages, null];
                }
                else {
                    return [null, result.message];
                }
            }
        }
        catch (e) {
            console.log(e);
        }

        return [null, ""];
    }
}
