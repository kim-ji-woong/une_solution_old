import CryptoJS from 'crypto-js';
import sha256 from 'crypto-js/sha256';
import { JsonManager } from './jsonManager';

export class AccountController {
    static async currentUser() {
        try {
            const jsonData = JsonManager.makeAccountCurrentUser();

            const res = await fetch('Account/Account/RequestData', {
            //const res = await fetch('/api/Account', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                const result = await res.json();
                if (result.success) {
                    return result.user;
                }
            }
        }
        catch (e) {
            console.log(e);
        }

        return null;
    }

    static encrypt(str, KEY) {
        const IV = KEY.substring(0, 16);
        const key = CryptoJS.enc.Utf8.parse(KEY);
        const iv = CryptoJS.enc.Utf8.parse(IV);

        const srcs = CryptoJS.enc.Utf8.parse(str);
        const encrypted = CryptoJS.AES.encrypt(srcs, key, {
            iv: iv,
            mode: CryptoJS.mode.CBC,
            padding: CryptoJS.pad.Pkcs7
        });

        return encrypted.ciphertext.toString();
    }

    static async regist(name, email, phoneNumber, password) {
        try {
            const enc = await AccountController.getEncryptedPassword(email, password);
            const jsonData = JsonManager.makeAccountRegist(name, email, phoneNumber, enc);

            const res = await fetch('Account/Account/RequestData', {
            //const res = await fetch('/api/Account', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                const result = await res.json();
                return result;
            }
        }
        catch (e) {
            console.log(e);
        }

        return null;
    }

    static async getEncryptedPassword(id, password) {
        const key = await AccountController.getLoginKey();

        if (!key) {
            return null;
        }

        const pwHash = sha256(password);
        return AccountController.encrypt(id + "|" + pwHash, key);
    }

    static async getLoginKey() {
        const now = new Date();
        const ticks = now.getTime();

        let key = null;

        try {
            const res = await fetch('Account/Account/Get?num=' + ticks);
            //const res = await fetch('/api/Account/' + ticks);
            key = await res.text();
        }
        catch (e) {
            console.log(e);
        }

        return key;
    }

    static async login(id, pw) {
        const enc = await AccountController.getEncryptedPassword(id, pw);
        
        if (!enc)
            return { success: false, message: "서버와 접속이 끊어졌거나 로그인 과정을 진행할 수 없습니다.", user: null };

        try {
            const jsonData = JsonManager.makeAccountLogin(enc);

            const res = await fetch('Account/Account/RequestData', {
            //const res = await fetch('/api/Account', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                const result = await res.json();
                return result;
            }
        }
        catch (e) {
            console.log(e);
            return { success: false, message: e.message, user: null};
        }

        return { success: false, message: "", user: null};
    }

    static async logout(id) {
        try {
            const jsonData = JsonManager.makeAccountLogout(id);

            const res = await fetch('Account/Account/RequestData', {
            //const res = await fetch('/api/Account', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                const result = await res.json();
                return result;
            }
        }
        catch (e) {
            console.log(e);
        }

        return null;
    }

    static async autoLogin(beginCode) {
        try {
            const jsonData = JsonManager.makeAutoLogin(beginCode);

            const res = await fetch('Account/Account/RequestData', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                const result = await res.json();
                return result;
            }
        }
        catch (e) {
            console.log(e);
        }

        return null;
    }

    static async requestRegist(userTypes, permit, denyReason) {
        try {
            const jsonData = JsonManager.makeRequestRegist(userTypes, permit, denyReason);

            const res = await fetch('Account/Account/RequestData', {
            //const res = await fetch('/api/Account', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                const result = await res.json();
                return result;
            }
        }
        catch (e) {
            console.log(e);
            return { success: false, message: e.message, requestUsers: null };
        }

        return { success: false, message: "", requestUsers: null };
    }
}