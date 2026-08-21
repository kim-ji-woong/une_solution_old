import ProjectResource from "../../Root/resource/id";

export default class SopManagerResource {
    static get ID() {
        return SopManagerResource.id[ProjectResource.targetLanguage];
    }

    static id = {
        "ko": {
            projectName: "SOP 편집",
            // 메인
            menu:
            {
                editSOP: "SOP 편집",
                home: "홈",
                newSOP: "새 SOP",
                open: "열기",
                save: "저장",
                saveAs: "다른 이름저장",
                delete: "삭제",
                openXML: "파일 열기",
                saveXML: "파일 저장",
            },
            cascadingMenu:
            {
                actionStep: "SOP 단계",
                addComponent: "컴포넌트 추가",
                specialCharacter: "특수문자 입력 형식",
                userDefined: "사용자 정의 인자"
            },
            editMenu:
            {
                undo: "뒤로가기",
                redo: "되돌리기",
                copy: "복사",
                cut: "잘라내기",
                paste: "붙여넣기",
                delete: "삭제"
            },
            actionStep:
            {
                _1st: "관심",
                _2nd: "주의",
                _3rd: "경계",
                _4th: "심각",
            },
            sopMode:
            {
                day: "주간",
                night: "야간",
                normal: "평일모드",
                abnormal: "야간 및 휴일모드"
            },
            component:
            {
                process: "프로세스",
                endpoint: "시작/끝",
                decision: "판단",
                annotation: "설명",
                internal: "상황전파"
            },
            specialCharacter:
            {
                selectType: "타입선택",
                disasterTime: "재난 발생 시각",
                tableDescription: "변수명, 타입, 설명으로 구성된 표",
                columnHeader:
                {
                    variable: "변수명",
                    type: "타입",
                    description: "설명"
                }
            },
            userDefinedVariable:
            {
                type:
                {
                    integer: "정수",
                    double: "실수",
                    boolean: "참/거짓",
                    string: "문자열"
                }
            },
            componentProperty:
            {
                title: "컴포넌트 속성"
            },
            messages:
            {
                loadingData: "데이터를 불러오고 있습니다.",
                inputDisasterType: "재난종류명을 입력하세요.",
                inputSOPName: "SOP 이름을 입력하세요.",
                unknownSOPName: "SOP 이름을 확인할 수 없습니다.",
                selectDisasterCategory: "재난분야를 선택하세요.",
                selectSubDisasterCategory: "재난종류를 선택하세요.",
                selectSOPName: "SOP를 선택하세요.",
                selectSOPVersion: "SOP 버전을 선택하세요.",
                inputSOPVersionName: "저장할 버전 이름을 입력하세요.",
                selectAddProgram: "추가할 프로그램을 선택하세요."
            },
            messageFormat:
            {
                checkNthParameters: "{0}번째 전달인자 값을 확인해주세요."
            },
            category:
            {
                disasterCategory: "재난분야",
                subDisasterCategory: "재난종류",
                disaster: "SOP 이름"
            },
            placeHolders:
            {
                newSubDisasterCategory: "새로운 재난종류",
                newSOP: "새로운 SOP"
            },
            common:
            {
                close: "닫기",
                confirm: "확인",
                notUse: "사용안함",
                edit: "편집",
                add: "추가",
                delete: "삭제",
                save: "저장",
                make: "만들기",
                cancel: "취소"
            },
            dataType:
            {
                integer: "정수",
                float: "실수",
                double: "실수",
                string: "텍스트",
                long: "정수",
                boolean: "참/거짓",
                short: "정수",
                byte: "정수"
            },
            sectionMark:
            {
                sms: "문자",
                broadcast: "방송",
                email: "메일",
                auto: "자동"
            }
        },
        "en": {
            projectName: "SOP Manager"
        }
    }

    static format(strFormat, arg1 = null, arg2 = null, arg3 = null, arg4 = null, arg5 = null, arg6 = null, arg7 = null, arg8 = null, arg9 = null, arg10 = null) {
        let value = strFormat;

        if (arg1 !== null) {
            value = value.replace("{0}", arg1.toString());
        }

        if (arg2 !== null) {
            value = value.replace("{1}", arg2.toString());
        }

        if (arg3 !== null) {
            value = value.replace("{2}", arg3.toString());
        }

        if (arg4 !== null) {
            value = value.replace("{3}", arg4.toString());
        }

        if (arg5 !== null) {
            value = value.replace("{4}", arg5.toString());
        }

        if (arg6 !== null) {
            value = value.replace("{5}", arg6.toString());
        }

        if (arg7 !== null) {
            value = value.replace("{6}", arg7.toString());
        }

        if (arg8 !== null) {
            value = value.replace("{7}", arg8.toString());
        }

        if (arg9 !== null) {
            value = value.replace("{8}", arg9.toString());
        }

        if (arg10 !== null) {
            value = value.replace("{9}", arg10.toString());
        }

        return value;
    }

    static disasterCategoryType = {
        fire: 0,
        natureDisaster: 1,
        explosion: 2,
        pollution: 3,
        security: 4,
        terror: 5,
        etc: 6
    }

    static getDisasterCategoryType(disasterCategoryName) {
        if (ProjectResource.targetLanguage === "ko") {
            if (disasterCategoryName.startsWith("자연재해")) {
                return SopManagerResource.disasterCategoryType.natureDisaster;
            }
            else if (disasterCategoryName.startsWith("화재")) {
                return SopManagerResource.disasterCategoryType.fire;
            }
            else if (disasterCategoryName.startsWith("폭발")) {
                return SopManagerResource.disasterCategoryType.explosion;
            }
            else if (disasterCategoryName.startsWith("누출") || disasterCategoryName.startsWith("유출") || disasterCategoryName.startsWith("오염") || disasterCategoryName.startsWith("유해물질")) {
                return SopManagerResource.disasterCategoryType.pollution;
            }
            else if (disasterCategoryName.startsWith("방범") || disasterCategoryName.startsWith("보안")) {
                return SopManagerResource.disasterCategoryType.security;
            }
            else if (disasterCategoryName.startsWith("테러")) {
                return SopManagerResource.disasterCategoryType.terror;
            }
        }

        return SopManagerResource.disasterCategoryType.etc;
    }
}