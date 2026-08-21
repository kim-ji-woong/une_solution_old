import { CommonJsonManager } from "./commonJsonManager";
import { SpaceDataManager } from "./spaceDataManager";

export class CommonController {
    static async requestOpenTempXML(loginData) {
        try {
            const jsonData = CommonJsonManager.makeRequestOpenTempXML(loginData);

            const res = await fetch('CommonMaker/CommonMaker/RequestData', {
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
                    SpaceDataManager.initIDsFromXMLData(result);
                    return result;
                }                
            }

            return null;
        }
        catch (e) {
            console.log(e);
        }
    }

    static async requestOpenXML(file) {
        try {
            const formData = new FormData();
            formData.append('files', file);

            const res = await fetch('CommonMaker/CommonMaker/RequestOpenXML', {
                method: 'post',               
                body: formData
            });

            if (res.ok) {
                const result = await res.json();

                if (result.success) {
                    SpaceDataManager.initIDsFromXMLData(result);
                    return result;
                }
                else {
                    return null;
                }
            }
        }
        catch (e) {
            console.log(e);
        }

        return [null, null, ""];
    }

    static async requestSaveXML(datas) {
        try {
            const jsonData = CommonJsonManager.makeRequestSaveXML(datas);

            const res = await fetch('CommonMaker/CommonMaker/RequestData', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                if (datas.bTempSave) {
                    return [true, ""];
                }
                else {
                    if (res.headers.get('content-type') === 'text/xml') {
                        await CommonController.downloadFile(res);
                        return [true, ""];
                    }
                    else {
                        const result = await res.json();
                        if (result.success) {
                            await CommonController.saveFile(res);
                            return result;
                        }
                        else {
                            return null;
                        }
                    }
                }
            }
        }
        catch (e) {
            console.log(e);
        }

        return [false, ""];
    }

    static async downloadFile(response) {
        const fileName = CommonController.getFileName(response);

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
}