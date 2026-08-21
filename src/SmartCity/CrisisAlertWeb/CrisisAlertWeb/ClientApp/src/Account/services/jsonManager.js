export class JsonManager {
    static makeUserLogin(data, key) {
        const json = {
            "RequestLogin":
            {
                "value": data,
                "key": key,
            },
        }

        return JSON.stringify(json);
    }

    static makeSessionLogin(key) {
        const json = {
            "RequestSessionLogin":
            {
                "key": key,
            },
        }

        return JSON.stringify(json);
    }

    static makeChangePassword(data, key) {
        const json = {
            "RequestChangePassword":
            {
                "value": data,
                "key": key,
            },
        }

        return JSON.stringify(json);
    }

    static makeUserLogout(key) {
        const json = {
            "RequestLogout":
            {
                "key": key,
            },
        }

        return JSON.stringify(json);
    }

    static makeCheckUserID(id) {
        const json = {
            "RequestCheckUserID":
            {
                "userID": id,
            },
        }

        return JSON.stringify(json);
    }

    static makeCheckCode(data, key) {
        const json = {
            "RequestCheckCode":
            {
                "value": data,
                "key": key,
            },
        }

        return JSON.stringify(json);
    }

    static makePWDFind(data, key) {
        const json = {
            "RequestPWDFind":
            {
                "value": data,
                "key": key,
            },
        }

        return JSON.stringify(json);
    }
}