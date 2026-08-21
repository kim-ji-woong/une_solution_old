import { SpaceBody } from '../spaceBody';
import { SpaceDataManager } from './spaceDataManager';
import { SpaceJsonManager } from './spaceJsonManager';

export class SpaceController {
    static async requestBuildingGroupList() {
        try {
            return [[], {}, ""];
            /*const jsonData = SpaceJsonManager.makeRequestBuildingGroupList();

            const res = await fetch('Space/Space/RequestData', {
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
            }*/
        }
        catch (e) {
            console.log(e);
        }

        return [null, null, ""];
    }

    static async requestGltfModelList() {
        try {
            return [[], {}, ""];
            /*const jsonData = SpaceJsonManager.makeRequestGltfDataList(-1);

            const res = await fetch('Space/Space/RequestData', {
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
            }*/
        }
        catch (e) {
            console.log(e);
        }

        return [null, null, "requestGltfModelList 실패"];
    }

    static async requestUploadExcelFile(file, sensorType) {
        try {
            const formData = new FormData();
            formData.append('files', file);

            let url = "";

            if (sensorType === SpaceDataManager.FireSensorType)
                url = 'Space/Space/UploadFireSensorFile';
            else if (sensorType === SpaceDataManager.PSMSensorType)
                url = 'Space/Space/UploadPSMSensorFile';
            else if (sensorType === SpaceDataManager.EtcSensorType)
                url = 'Space/Space/UploadEtcSensorFile';
            else if (sensorType === SpaceDataManager.CCTVType)
                url = 'Space/Space/UploadCCTVFile';
            else
                return [false, "알수없는 형식의 파일입니다.", null];

            const res = await fetch(url, {
                method: 'post',
                body: formData
            });

            if (res.ok) {
                const result = await res.json();
                return [result.success, result.message, SpaceController.getSensors(result, sensorType)];
            }
            else {
                const errorMessage = `fail : ${res.status} error, ${res.url}`;
                return [false, errorMessage, null];
            }
        }
        catch (e) {
            return [false, e.message, null];
        }

        return [false, "Excel 파일 업로드 실패", null];
    }

    static getSensors(result, sensorType) {
        if (sensorType === SpaceDataManager.FireSensorType)
            return result.fireSensors;
        else if (sensorType === SpaceDataManager.PSMSensorType)
            return result.psmSensors;
        else if (sensorType === SpaceDataManager.EtcSensorType)
            return result.etcSensors;
        else if (sensorType === SpaceDataManager.CCTVType)
            return result.cctvs;

        return null;
    }

    static async requestDownloadSensorExcelFile(sensorType, sensors) {
        try {
            const jsonData = SpaceJsonManager.makeRequestDownloadSensorExcelFile(sensorType, sensors);

            const res = await fetch('Space/Space/RequestData', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                if (res.headers.get('content-type') === 'application/vnd.ms-excel') {
                    await SpaceController.downloadFile(res);
                    return [true, ""];
                }
                else {
                    const result = await res.json();

                    if (result.success) {
                        return [result.success, ""];
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
        const fileName = SpaceController.getFileName(response);

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

    static async requestUploadTempModelFile(file, loginData, modelType) {
        try {
            if (!loginData || !loginData.success) {
                return [false, "로그인 되지 않았습니다."];
            }

            const [checkFileResult, message] = SpaceController.checkModelFile(file);

            if (checkFileResult === false) {
                return [false, message];
            }

            const dummyFile = new File([], "userID_" + loginData.user.id);

            const formData = new FormData();
            formData.append('files', file);
            formData.append('files', dummyFile);

            let url = "";

            //if (modelType === SpaceBody.Type_Site)
                url = 'Space/Space/UploadTempModelFile';
            /*else
                return [false, "알수없는 형식의 파일입니다."];*/

            const res = await fetch(url, {
                method: 'post',
                body: formData
            });

            if (res.ok) {
                const result = await res.json();
                return [result.success, result.message];
            }
            else {
                const errorMessage = `fail : ${res.status} error, ${res.url}`;
                return [false, errorMessage];
            }
        }
        catch (e) {
            return [false, e.message];
        }

        return [false, "Model 파일 업로드 실패"];
    }

    static checkModelFile(file) {
        const index = file.name.lastIndexOf('.');

        if (index < 0) {
            return [false, `${file.name}은 업로드 가능한 모델 파일이 아닙니다.`];
        }

        const ext = file.name.substring(index + 1).trim().toLowerCase();

        if (ext !== "glb") {
            return [false, `${file.name}은 업로드 가능한 모델 파일이 아닙니다.`];
        }

        return [true, ""];
    }

    static async requestUploadModelFile(loginData, _3dOptions) {
        if (!loginData || !loginData.success) {
            return [false, "로그인 되지 않았습니다."];
        }

        try {
            const jsonData = SpaceJsonManager.makeRequestUploadModelFile(loginData, _3dOptions, false, false, true);

            const res = await fetch('Space/Space/RequestData', {
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
            else {
                const errorMessage = `fail : ${res.status} error, ${res.url}`;
                return [false, errorMessage];
            }
        }
        catch (e) {
            return [false, e.message];
        }

        return [false, "Model 파일 업로드 실패"];
    }

    static async requestClearTempModelFiles(loginData) {
        if (!loginData || !loginData.success) {
            return [false, "로그인 되지 않았습니다."];
        }

        try {
            const jsonData = SpaceJsonManager.makeRequestUploadModelFile(loginData, null, true, false, false);

            const res = await fetch('Space/Space/RequestData', {
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
            else {
                const errorMessage = `fail : ${res.status} error, ${res.url}`;
                return [false, errorMessage];
            }
        }
        catch (e) {
            return [false, e.message];
        }

        return [false, "임시 Model 파일 초기화 실패"];
    }

    static async requestRemoveTempFile(loginData, fileName) {
        if (!loginData || !loginData.success) {
            return [false, "로그인 되지 않았습니다."];
        }

        try {
            const jsonData = SpaceJsonManager.makeRequestRemoveTempFile(loginData, fileName);

            const res = await fetch('Space/Space/RequestData', {
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
            else {
                const errorMessage = `fail : ${res.status} error, ${res.url}`;
                return [false, errorMessage];
            }
        }
        catch (e) {
            return [false, e.message];
        }

        return [false, "임시 Model 파일 초기화 실패"];
    }
}