import ProjectResource from "../../Root/resource/id";

export default class AccountResource {
    static get ID() {
        return AccountResource.id[ProjectResource.targetLanguage];
    }

    static id = {
        "ko": {
            "textLoginError": "가입하지 않은 아이디 이거나 잘못된 비밀번호 입니다.",
            "textLoginIDError": "아이디를 입력하세요.",
            "textLoginPwdError": "비밀번호를 입력하세요.",
            "textLoginSessionError": "다른 곳에서 로그인하여 로그아웃이 되었습니다.",
            "textNewPwdError": "새로운 비밀번호를 입력하세요.",
            "textNewPwd2Error": "새로운 비밀번호를 한번 더 입력하세요.",
            "textNewPwd3Error": "새로운 비밀번호가 서로 맞질 않습니다. 확인해주세요.",
            "textNewPwd4Error": "기존 비밀번호와 새로운 비밀번호가 같습니다. 다른 비밀번호를 설정해주세요.",
            "textFindCodeError": "인증코드를 입력하세요.",
            "textEnterError": "잘못된 접근입니다.",

            accountLevel:
            {
                general: 1,
                manager: 2,
            },
        },
        "en": {
        }
    }
}