import SessionString from '../../Common/js/sessionString';
import { SdmsJsonManager } from '../../SDMS/services/sdmsJsonManager';

export default class ProjectResource {
    static targetLanguage = "ko";
    static siteID = null;

    // GS인증 버전 확인용
    static isGSMode = null;

    static get SiteID() {
        return ProjectResource.siteID;
    }

    static set SiteID(id) {
        ProjectResource.siteID = id;
    }

    static get ID() {
        return ProjectResource.id[ProjectResource.targetLanguage];
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
        findPassword: "/findPassword",
        specialReport: '/specialReport',
    }

    static getUserInfo() {
        const siteID = ProjectResource.SiteID;

        if (siteID === null || siteID === undefined ||
            window.localStorage.getItem(SessionString.Key.account + "_" + siteID.toString()) == null)
            return null;

        let userInfo = JSON.parse(window.localStorage.getItem(SessionString.Key.account + "_" + siteID.toString()));

        return userInfo;
    }

    static async initUserInfo() {
        let siteID = ProjectResource.SiteID;

        if (siteID === null || siteID === undefined) {
            siteID = await ProjectResource.loadSiteID();
        }

        return ProjectResource.getUserInfo();
    }

    static getUserAuthor() {
        const userInfo = ProjectResource.getUserInfo();
        let userAuthor = null;

        if (userInfo !== null && userInfo !== undefined)
            userAuthor = userInfo.level;

        return userAuthor;
    }

    static async initUserAuthor() {
        let siteID = ProjectResource.SiteID;

        if (siteID === null || siteID === undefined) {
            siteID = await ProjectResource.loadSiteID();
        }

        return ProjectResource.getUserAuthor();
    }

    static async loadSiteID() {
        let siteID = ProjectResource.SiteID;

        if (siteID === null || siteID === undefined) {
            // 사이트 ID 요청
            try {
                const jsonData = SdmsJsonManager.makeRequestGetSiteID();

                const res = await fetch('SDMS/SDMS/RequestData', {
                    method: 'post',
                    headers: {
                        'Accept': 'application/json',
                        'Content-Type': 'application/json'
                    },
                    body: jsonData
                });

                if (res.ok) {
                    const result = await res.json();

                    if (result.success === true) {
                        ProjectResource.SiteID = result.siteID;
                    } 
                }

            } catch (e) {
                console.log(e);
            }
        }

        return siteID;
    }

    static setLoginUser(user) {
        if (user === null || user === undefined)
            return;

        const siteID = ProjectResource.SiteID;
        if (siteID === null || siteID === undefined)
            return;

        window.localStorage.setItem(SessionString.Key.account + "_" + siteID.toString(), JSON.stringify(user));
    }

    static Site = {
        Soulbrain: 10,      // 솔브레인
        GCC: 12,            // 녹십자
    }
}