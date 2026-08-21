export class JsonManager {
    static makeRequestURL() {
        const json = {
            "requestURL": true
        }

        return JSON.stringify(json);
    }

    static makeAccountCurrentUser() {
        const json = {
            "currentUser": ""
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

    static makeRequestNewLoginKey(beginCode) {
        const json = {
            "requestNewLoginKey":
            {
                "beginCode": beginCode
            }
        }

        return JSON.stringify(json);
    }

    static makeAccountRegist(name, email) {
        const json = {
            "register":
            {
                "name": name,
                "email": email,
                "returnUrl": window.location.origin
            }
        }

        return JSON.stringify(json);
    }

    static makeAccountRegisterParam(param) {
        const json = {
            "registerParam":
            {
                "value": param
            }
        }

        return JSON.stringify(json);
    }

    static makeAccountSetPassword(data) {
        const json = {
            "registerPassword":
            {
                "value": data
            }
        }

        return JSON.stringify(json);
    }

    static makeRequestLinks() {
        const json = {
            "requestLinks": true
        }

        return JSON.stringify(json);
    }
}