export default class ProjectResource {
    static targetLanguage = "ko";
    static _isModelViewer = false;

    static get ID() {
        return ProjectResource.id[ProjectResource.targetLanguage];
    }

    static get isModelViewer() {
        return ProjectResource._isModelViewer;
    }

    static set isModelViewer(value) {
        ProjectResource._isModelViewer = value;
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

    static settingTab = {
        monitoring: "monitoring",
        disaster: "disaster",
        spread: "spread",
    }
}