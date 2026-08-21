export class JsonManager {
    static makeAccountCurrentUser() {
        const json = {
            "currentUser": ""
        }

        return JSON.stringify(json);
    }

    static makeAccountRegist(name, email, phoneNumber, password) {
        const json = {
            "register":
            {
                "name": name,
                "email": email,
                "phoneNumber": phoneNumber,
                "password": password,
                "registNewUser": password !== null
            }
        }

        return JSON.stringify(json);
    }

    static makeAccountLogin(data) {
        const json = {
            "login":
            {
                "value": data
            }
        }

        return JSON.stringify(json);
    }

    static makeAccountLogout(id) {
        const json = {
            "logout":
            {
                "userID": id
            }
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

    static makeRequestRegist(userTypes, permit, denyReason) {
        const users = [];

        for (const userID in userTypes) {
            const user = userTypes[userID];
            users.push(user);
        }

        const json = {
            "requestRegist":
            {
                "permit": permit,
                "denyDescription": denyReason,
                "users": users
            }
        }

        return JSON.stringify(json);
    }
}