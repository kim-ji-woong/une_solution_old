import { Calendar } from '../../Vacation/Calendar/calendar';

export class JsonManager {
    static makeAccountLogin(data) {
        const json = {
            "login":
            {
                "value": data
            },
            "logout": null,
            "register": null,
            "registerParam": null,
            "registerPassword": null,
            "currentUser": null
        }

        return JSON.stringify(json);
    }

    static makeAccountLogout(id) {
        const json = {
            "login": null,
            "logout":
            {
                "userID": id
            },
            "register": null,
            "registerParam": null,
            "registerPassword": null,
            "currentUser": null
        }

        return JSON.stringify(json);
    }

    static makeAutoLogin(beginCode) {
        const json = {
            "autoLogin":
            {
                "beginCode": beginCode
            }
        }

        return JSON.stringify(json);
    }

    static makeAccountRegist(name, email) {
        const json = {
            "login": null,
            "logout": null,
            "register":
            {
                "name": name,
                "email": email
            },
            "registerParam": null,
            "registerPassword": null,
            "currentUser": null
        }

        return JSON.stringify(json);
    }

    static makeAccountRegisterParam(param) {
        const json = {
            "login": null,
            "logout": null,
            "register": null,
            "registerParam":
            {
                "value": param
            },
            "registerPassword": null,
            "currentUser": null
        }

        return JSON.stringify(json);
    }

    static makeAccountSetPassword(data) {
        const json = {
            "login": null,
            "logout": null,
            "register": null,
            "registerParam": null,
            "registerPassword":
            {
                "value": data
            },
            "currentUser": null
        }

        return JSON.stringify(json);
    }

    static makeAccountCurrentUser() {
        const json = {
            "login": null,
            "logout": null,
            "register": null,
            "registerParam": null,
            "registerPassword": null,
            "currentUser": ""
        }

        return JSON.stringify(json);
    }

    static makeRequestManager(user, days) {
        const dayCount = days.length;
        let date = "";

        const emptyDay = Calendar.getEmptyDay();
        //const halfAM = Calendar.getHalfAM();
        //const halfPM = Calendar.getHalfPM();

        for (let i = 0; i < dayCount; i++) {
            const day = days[i];

            if (day.dayType === emptyDay)
                continue;

            if (date.length === 0)
                date = day.date.toString();
            else
                date += ";" + day.date.toString();

            if (day.dayType !== Calendar.AllDay) {
                date += ":" + parseInt(day.dayType);
            }

            /*if (day.dayType === halfAM || day.dayType === halfPM) {
                date += ":" + parseInt(day.dayType);
            }*/
        }

        const json = {
            "requestManager":
            {
                "userID": user.userID,
                "requestDays": date
            },
            "requestSpecialVacationManager": null,
            "requestHistory": null,
            "requestVacation": null,
            "requestSpecialVacation": null,
            "requestManagerData": null,
            "processRequest": null,
            "requestMemberHistory": null
        }

        return JSON.stringify(json);
    }

    static makeRequestSpecialVacationManager(user) {
        const json = {
            "requestManager": null,
            "requestSpecialVacationManager":
            {
                "userID": user.userID,
                "requestDays": null
            },
            "requestHistory": null,
            "requestVacation": null,
            "requestSpecialVacation": null,
            "requestManagerData": null,
            "processRequest": null,
            "requestMemberHistory": null
        }

        return JSON.stringify(json);
    }

    static makeRequestVacation(user, days, description) {
        const dayCount = days.length;
        let date = "";

        const emptyDay = Calendar.getEmptyDay();
        //const halfAM = Calendar.getHalfAM();
        //const halfPM = Calendar.getHalfPM();

        for (let i = 0; i < dayCount; i++) {
            const day = days[i];

            if (day.dayType === emptyDay)
                continue;

            if (date.length === 0)
                date = day.date.toString();
            else
                date += ";" + day.date.toString();

            if (day.dayType !== Calendar.AllDay) {
                date += ":" + parseInt(day.dayType);
            }

            /*if (day.dayType === halfAM || day.dayType === halfPM) {
                date += ":" + parseInt(day.dayType);
            }*/
        }

        const json = {
            "requestManager": null,
            "requestSpecialVacationManager": null,
            "requestHistory": null,
            "requestVacation":
            {
                "userID": user.userID,
                "requestDays": date,
                "description": description
            },
            "requestSpecialVacation": null,
            "requestManagerData": null,
            "processRequest": null,
            "requestMemberHistory": null
        }

        return JSON.stringify(json);
    }

    static makeRequestSpecialVacation(manager, userIDs, days, reason) {
        const json = {
            "requestManager": null,
            "requestSpecialVacationManager": null,
            "requestHistory": null,
            "requestVacation": null,
            "requestSpecialVacation":
            {
                "RequestManagerID": manager.userID,
                "Days": days,
                "UserIDs": userIDs,
                "Reason": reason
            },
            "requestManagerData": null,
            "processRequest": null,
            "requestMemberHistory": null
        }

        return JSON.stringify(json);
    }

    static makeRequestHistory(user, year, month, day) {
        const json = {
            "requestHistory":
            {
                "userID": user.userID,
                "year": year,
                "month": month,
                "day": day
            }
        }
        /*const json = {
            "requestManager": null,
            "requestSpecialVacationManager": null,
            "requestHistory":
            {
                "userID": user.userID,
                "year": year,
                "month": month,
                "day": day
            },
            "requestVacation": null,
            "requestSpecialVacation": null,
            "requestManagerData": null,
            "processRequest": null,
            "requestMemberHistory": null
        }*/

        return JSON.stringify(json);
    }

    static makeRequestManagerData(user, year) {
        const json = {
            "requestManager": null,
            "requestSpecialVacationManager": null,
            "requestHistory": null,
            "requestVacation": null,
            "requestSpecialVacation": null,
            "requestManagerData":
            {
                "managerUserID": user.userID,
                "year": year
            },
            "processRequest": null,
            "requestMemberHistory": null
        }

        return JSON.stringify(json);
    }

    static makeProcessRequest(requestID, permit, managerUserID, managerDescription, isNormal) {
        const json = {
            "requestManager": null,
            "requestSpecialVacationManager": null,
            "requestHistory": null,
            "requestVacation": null,
            "requestSpecialVacation": null,
            "requestManagerData": null,
            "processRequest":
            {
                "requestID": requestID,
                "isPermit": permit,
                "managerUserID": managerUserID,
                "managerDescription": managerDescription,
                "isNormal": isNormal
            },
            "requestMemberHistory": null
        }

        return JSON.stringify(json);
    }

    static makeRequestMemberHistory(managerUserID) {
        const json = {
            "requestManager": null,
            "requestSpecialVacationManager": null,
            "requestHistory": null,
            "requestVacation": null,
            "requestSpecialVacation": null,
            "requestManagerData": null,
            "processRequest": null,
            "requestMemberHistory":
            {
                "managerUserID": managerUserID
            }
        }

        return JSON.stringify(json);
    }

    // history의 MemberHistories List를 Dictionary 형태로 바꾼다.
    static setMemberHistory(history) {
        const memberHistories = {};
        const memberNextYearHistories = {};
        const memberLastYearHistories = {};
        const historyCount = history.memberIDs.length;

        for (let i = 0; i < historyCount; i++) {
            const memberID = history.memberIDs[i];
            const memberHistory = history.memberHistories[i];
            const memberNextYearHistory = history.memberHistoriesNextYear[i];
            const memberLastYearHistory = history.memberHistoriesLastYear[i];

            memberHistories[memberID] = memberHistory;
            memberNextYearHistories[memberID] = memberNextYearHistory;
            memberLastYearHistories[memberID] = memberLastYearHistory;
        }

        history.memberHistories = memberHistories;
        history.memberHistoriesNextYear = memberNextYearHistories;
        history.memberHistoriesLastYear = memberLastYearHistories;
    }

    static makeRequestVacationList(userID, year) {
        const json = {
            "requestVacationList":
            {
                "userID": userID,
                "year": year
            }
        }

        return JSON.stringify(json);
    }

    static makeRequestCancelVacations(requestIDs) {
        const json = {
            "requestCancelVacations":
            {
                "requestIDs": requestIDs
            }
        }

        return JSON.stringify(json);
    }

    static makeRequestHolidays(year, month) {
        const json = {
            "RequestHolidays":
            {
                "year": year,
                "month": month
            }
        }

        return JSON.stringify(json);
    }
}