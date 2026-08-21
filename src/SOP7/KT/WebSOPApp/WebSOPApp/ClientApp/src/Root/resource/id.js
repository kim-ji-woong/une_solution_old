export default class ProjectResource {
    static targetLanguage = "ko";
    static siteID = null;
    static sopParams = {};

    static get SiteID() {
        return ProjectResource.siteID;
    }

    static set SiteID(id) {
        ProjectResource.siteID = id;
    }

    static get ID() {
        return ProjectResource.id[ProjectResource.targetLanguage];
    }

    static setSOPParams(actionStepHistoryID, accessMode, accessToken, serviceType, siteID) {
        ProjectResource.sopParams[actionStepHistoryID.toString()] = {
            "accessMode": accessMode,
            "accessToken": accessToken,
            "serviceType": serviceType,
            "siteID": siteID
        };
    }

    static getSOPParams(actionStepHistoryID) {
        const params = ProjectResource.sopParams[actionStepHistoryID.toString()];

        if (!params) {
            return null;
        }

        return params;
    }

    static id = {
        "ko": {
            title: {
                sdms: "sdms",
                sopSimulator: 'SOP',
                teamEditor: '조직관리',
                sopManager: 'SOP 편집',
                dashboard: '대시보드',
                history: "이력관리",
            },
        }
    }

    static path = {
        root: "/",
        sopSimulator: "/sop-simulator",
        sdms: "/sdms",
        teamEditor: "/team-editor",
        sopManager: "/sop-manager",
        dashboard: "/dashboard",
        history: "/history",
        setPassword: "/setPassword",
        specialReport: '/specialReport',
    }
}