import CryptoJS from 'crypto-js';
import sha256 from 'crypto-js/sha256';
import { JsonManager } from './jsonManager';

import SessionString from '../../Common/js/sessionString';

export class AccountController {
    static async pwdFind(userID, pw) {
        if (pw === undefined || pw === null)
            return null;

        const key = await AccountController.getLoginKey();

        if (!key)
            return null;

        try {
            const pwHash = sha256(pw);
            const strEnc = AccountController.encrypt(userID + "|" + pwHash, key);
            const jsonData = JsonManager.makePWDFind(strEnc, key);

            const res = await fetch('/api/Account/', {
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
            } else {
                let result = new Object();
                result.success = false;
                result.message = "Account Controller 페이지를 찾을 수 없습니다. 네트워크를 확인해주세요.";

                return result;
            }
        }
        catch (e) {
            console.log(e);
        }

        return null;
    }

    static async checkCode(code) {
        if (code === undefined || code === null)
            return null;

        const key = await AccountController.getLoginKey();

        if (!key)
            return null;

        try {
            const codeHash = sha256(code);
            const strEnc = AccountController.encrypt(codeHash, key);
            const jsonData = JsonManager.makeCheckCode(strEnc, key);

            const res = await fetch('/api/Account/', {
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
            } else {
                let result = new Object();
                result.success = false;
                result.message = "Account Controller 페이지를 찾을 수 없습니다. 네트워크를 확인해주세요.";

                return result;
            }
        }
        catch (e) {
            console.log(e);
        }

        return null;
    }

    static async checkUserID(id) {
        if (id === undefined || id === null)
            return null;

        try {

            const jsonData = JsonManager.makeCheckUserID(id);

            const res = await fetch('/api/Account/', {
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
            } else {
                let result = new Object();
                result.success = false;
                result.message = "Account Controller 페이지를 찾을 수 없습니다. 네트워크를 확인해주세요.";

                return result;
            }
        }
        catch (e) {
            console.log(e);
        }

        return null;
    }

    static async logout(key) {
        if (key === undefined || key === null)
            return null;

        try {

            const jsonData = JsonManager.makeUserLogout(key);

            const res = await fetch('/api/Account/', {
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
            } else {
                let result = new Object();
                result.success = false;
                result.message = "Account Controller 페이지를 찾을 수 없습니다. 네트워크를 확인해주세요.";

                return result;
            }
        }
        catch (e) {
            console.log(e);
        }

        return null;
    }

    static async changePassword(id, pw, newPW) {
        const key = await AccountController.getLoginKey();

        if (!key)
            return null;

        try {
            const pwHash = sha256(pw);
            const newPWHash = sha256(newPW);

            const strEnc = AccountController.encrypt(id + "|" + pwHash + "|" + newPWHash, key);
            const jsonData = JsonManager.makeChangePassword(strEnc, key);

            const res = await fetch('/api/Account/', {
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
            } else {
                let result = new Object();
                result.success = false;
                result.message = "Account Controller 페이지를 찾을 수 없습니다. 네트워크를 확인해주세요.";

                return result;
            }
        }
        catch (e) {
            console.log(e);
        }

        return null;
    }


    static async sessionLogin(key) {
        if (key === undefined || key === null)
            return null;

        try {

            const jsonData = JsonManager.makeSessionLogin(key);

            const res = await fetch('/api/Account/', {
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
            } else {
                let result = new Object();
                result.success = false;
                result.message = "Account Controller 페이지를 찾을 수 없습니다. 네트워크를 확인해주세요.";

                return result;
            }
        }
        catch (e) {
            console.log(e);
        }

        return null;
    }

    static async login(id, pw, save) {
        const key = await AccountController.getLoginKey();

        if (!key)
            return null;

        try {
            const pwHash = sha256(pw);
            const strEnc = AccountController.encrypt(id + "|" + pwHash, key);
            const jsonData = JsonManager.makeUserLogin(strEnc, key);

            const res = await fetch('/api/Account/', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                const result = await res.json();

                if (result.success == true) {
                    // 로그인 성공
                    //세션 값 넣기
                    const key = result.key;

                    if (save === true) {
                        window.sessionStorage.removeItem(SessionString.Key.account);
                        window.localStorage.setItem(SessionString.Key.account, JSON.stringify(key));
                    }
                    else if (save === false) {
                        window.localStorage.removeItem(SessionString.Key.account);
                        window.sessionStorage.setItem(SessionString.Key.account, JSON.stringify(key));
                    }  
                }

                return result;
            } else {
                let result = new Object();
                result.success = false;
                result.message = "Account Controller 페이지를 찾을 수 없습니다. 네트워크를 확인해주세요.";

                return result;
            }
        }
        catch (e) {
            console.log(e);
        }

        return null;
    }

    static async getLoginKey() {
        const now = new Date();
        const ticks = now.getTime();

        let key = null;

        try {
            const res = await fetch('/api/Account/' + ticks);
            key = await res.text();
        }
        catch (e) {
            console.log(e);
        }

        return key;
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
}