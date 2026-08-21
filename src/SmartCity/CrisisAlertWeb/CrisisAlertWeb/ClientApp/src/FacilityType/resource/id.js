import ProjectResource from "../../Root/resource/id";

export default class FacilityTypeResource {
    static get ID() {
        return FacilityTypeResource.id[ProjectResource.targetLanguage];
    }

    static id = {
        "ko": {
            "textEnterError": "잘못된 접근입니다.",

            facilityType:
            {
                fire: 1,
                flood: 2,
                heat: 3,
                collapse: 4,
            },

            facilityTypeName:
            {
                fire: "화재",
                flood: "홍수",
                heat: "폭염",
                collapse: "경사지 붕괴",
            },

            riskLevel:
            {
                Normal: "Normal",
                Attention: "Attention",
                Caution: "Caution",
                Alert: "Alert",
                Serious: "Serious",
            },

            manualMenu:
            {
                List: 1,
                Content: 2,
            },
        },
        "en": {
        }
    }
}