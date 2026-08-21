export class JsonManager {
    static makeUserLogin(data: string, key: string): string {
        const json = {
            "login":
            {
                "value": data,
                "key": key,
            }
        }

        return JSON.stringify(json);
    }

    static makeUpdateAccountUser(accountUser: string, accessedUserID: string): string {
        const json = {
            "updateAccountUsers":
            {
                "AccountUsers": accountUser,
                "AccessedUserID": accessedUserID
            }
        }

        return JSON.stringify(json);
    }

    static makeRemoveAccountUsers(accountUser: string): string {
        const json = {
            "removeAccountUsers": accountUser
        }

        return JSON.stringify(json);
    }

    static makeReRegisterAccountUsers(accountUser: string): string {
        const json = {
            "reRegisterAccountUsers": accountUser
        }

        return JSON.stringify(json);
    }


    static makeGetAccountLevels(): string {
        const json = {
            "getAccountLevels": true
        }

        return JSON.stringify(json);
    }

    static makeGetAccountUsers(): string {
        const json = {
            "getAccountUsers": true
        }

        return JSON.stringify(json);
    }

    static makeChangePassword(name: string, email: string): string {
        const json = {
            "changePassword":
            {
                "name": name,
                "email": email,
            },
        }

        return JSON.stringify(json);
    }

    static makeCheckParamsCode(code: string): string {
        const json = {
            "checkParamsCode":
            {
                "code": code,
            },
        }

        return JSON.stringify(json);
    }

    static makeSetPassword(data: string, key: string): string {
        const json = {
            "setPassword":
            {
                "value": data,
                "key": key,
            },
        }

        return JSON.stringify(json);
    }

    static makeCheckLoginSession(userID: string, sessionKey: string): string {
        const json = {
            "checkLoginSession":
            {
                "userID": userID,
                "sessionKey": sessionKey,
            },
        }

        return JSON.stringify(json);
    }
}