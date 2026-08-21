import CryptoJS from 'crypto-js';
import sha256 from 'crypto-js/sha256';
import { JsonManager } from './jsonManager';

import SessionString from '../../Common/js/sessionString';
import RootResource from '../../Root/resource/id';
import AccountStore from '../accountStore';
import AccountResource from '../resource/id';

import ProjectResource from '../../Root/resource/id';

export class AccountController {
    static logoutMsgChk = false;
    static loading3DChk = false;

    static StartWatchTimer() {
        // 타이머 실행 유무 판단
        if (this.timerCheck == true)
            return;

        // 타이머 실행 체크
        this.timerCheck = true;

        let timerLogin = setTimeout(function tick() {
            AccountController.WatchLoginCheck();
            timerLogin = setTimeout(tick, 5000);
        }, 5000);
    }

    static async WatchLoginCheck() {
        const user = await ProjectResource.initUserInfo();

        //const siteID = ProjectResource.SiteID;

        //if (siteID !== null && siteID !== undefined) {
        if (user !== null && user !== undefined) {
            //const user = JSON.parse(window.localStorage.getItem(SessionString.Key.account + "_" + siteID.toString()));

            //if (user === null || user.sessionKey === null || user.sessionKey === undefined) {
            if (user.sessionKey === null || user.sessionKey === undefined) {
                let path = window.location.pathname;
                if (path !== RootResource.path.root && path !== RootResource.path.setPassword) {
                    AccountStore.dispatch({ type: 'LOGIN_STATE', loginState: AccountResource.loginState.false, message: "로그아웃 되었습니다." });
                }

                return;
            }

            const userID = user.id;
            const sessionKey = user.sessionKey;

            const [result, message] = await AccountController.checkLoginSession(userID, sessionKey);

            if (result === AccountResource.loginState.login) {
                // 세션이 유효
                //console.log(message);
                // 계정 리덕스에 상태 업데이트
                AccountStore.dispatch({ type: 'LOGIN_STATE', loginState: result, message: message });
            } else {
                // 세션 값이 일치하지 않음

                // 계정 리덕스에 상태 업데이트
                AccountStore.dispatch({ type: 'LOGIN_STATE', loginState: result, message: message });
            }
        } else {
            let path = window.location.pathname;
            if (path !== RootResource.path.root && path !== RootResource.path.setPassword) {
                AccountStore.dispatch({ type: 'LOGIN_STATE', loginState: AccountResource.loginState.false, message: "로그아웃 되었습니다." });
            }
        }
    }

    static async checkLoginSession(userID, sessionKey) {
        try {
            const jsonData = JsonManager.makeCheckLoginSession(userID, sessionKey);

            const res = await fetch('/Account/Account/RequestData', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });

            if (res.ok) {
                const result = await res.json();
                AccountController.loading3DChk = false;

                if (result.success) {
                    return [AccountResource.loginState.login, result.message];
                }
                else {
                    return [AccountResource.loginState.false, result.message];
                }
            }
        }
        catch (e) {
            console.log(e);

            if (AccountController.loading3DChk === false) {
                let message = "서버와 연결이 끊어졌습니다.";

                return [AccountResource.loginState.disconnected, message];
            }
                
        }

        // 요청 중에 페이지 이동 시 응답을 받지 못하는 경우가 발생할 수 있음. 
        return [AccountResource.loginState.login, "checkLoginSession 실패하였습니다."];
    }

    static async getAccountLevels() {
        try {
            const jsonData = JsonManager.makeGetAccountLevels();

            const res = await fetch('/Account/Account/RequestData', {
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
                    //return [result, ""];
                    return result.accountLevels;
                }
                else {
                    return [null, result.message];
                }
            }


        }
        catch (e) {
            console.log(e);
        }

        return [null, "getAccountLevels 실패"];
    }

    static async getAccountUsers() {
        try {
            const jsonData = JsonManager.makeGetAccountUsers();

            const res = await fetch('/Account/Account/RequestData', {
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
                    //return [result, ""];
                    return result.accountUsers;
                }
                else {
                    return [null, result.message];
                }
            }


        }
        catch (e) {
            console.log(e);
        }

        return [null, "getAccountUsers 실패"];
    }

    static async changePassword(name, data, mode) {
        const key = await AccountController.getLoginKey();

        if (!key)
            return [null, "changePassword 실패"];

        try {
            const pw = Math.random().toString(36).slice(2);
            const pwHash = sha256(pw).toString();

            const strEnc = AccountController.encrypt(pw + "|" + pwHash, key);

            const jsonData = JsonManager.makeChangePassword(name, data, strEnc, key, mode);

            const res = await fetch('/Account/Account/RequestData', {
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
                    return [result, result.message];
                }
                else {
                    return [null, result.message];
                }
            }
        }
        catch (e) {
            console.log(e);
        }

        return [null, "changePassword 실패"];
    }

    static async checkParamsCode(code) {
        try {
            const jsonData = JsonManager.makeCheckParamsCode(code);

            const res = await fetch('/Account/Account/RequestData', {
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
                    return [result, result.message];
                }
                else {
                    return [null, result.message];
                }
            }
        }
        catch (e) {
            console.log(e);
        }

        return [null, "changePassword 실패"];
    }

    static async setPassword(id, pwd, newPwd) {
        const key = await AccountController.getLoginKey();

        if (!key)
            return [null, "setPassword 실패"];

        try {
            const pwdHash = sha256(pwd);
            const newPwdHash = sha256(newPwd);

            const strEnc = AccountController.encrypt(id + "|" + pwdHash + "|" + newPwdHash, key);
            const jsonData = JsonManager.makeSetPassword(strEnc, key);

            const res = await fetch('/Account/Account/RequestData', {
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
                    return [result, result.message];
                }
                else {
                    return [null, result.message];
                }
            }
        }
        catch (e) {
            console.log(e);
        }

        return [null, "setPassword 실패"];
    }

    static async reRegisterAccountUsers(accountUsers) {
        if (accountUsers === null || accountUsers === undefined || accountUsers.length === 0)
            return [null, "reRegisterAccountUsers 실패"];

        try {
            const jsonData = JsonManager.makeReRegisterAccountUsers(accountUsers);

            const res = await fetch('/Account/Account/RequestData', {
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
                    return [result, ""];
                }
                else {
                    return [null, result.message];
                }
            }
        }
        catch (e) {
            console.log(e);
        }

        return [null, "reRegisterAccountUsers 실패"];
    }

    static async removeAccountUsers(accountUsers) {
        if (accountUsers === null || accountUsers === undefined || accountUsers.length === 0)
            return [null, "removeAccountUsers 실패"];

        try {
            const jsonData = JsonManager.makeRemoveAccountUsers(accountUsers);

            const res = await fetch('/Account/Account/RequestData', {
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
                    return [result, ""];
                }
                else {
                    return [null, result.message];
                }
            }
        }
        catch (e) {
            console.log(e);
        }

        return [null, "removeAccountUsers 실패"];
    }

    static async updateAccountUser(accountUsers, accessedUserID) {
        if (accountUsers === null || accountUsers === undefined || accountUsers.length === 0)
            return [null, "updateAccountUsers 실패"];

        for (let i = 0; i < accountUsers.length; i++) {
            let accountUser = accountUsers[i];
            let num = "";

            if (accountUser.accountID !== -1) 
                continue;

            // 계정이 없다면 임시 비밀번호 저장 후 전달
            if (accountUser.phoneNumber === null || accountUser.phoneNumber === undefined) {
                num = "1234";
            } else {
                num = accountUser.phoneNumber;

                let index = num.indexOf('-');
                num = num.substring(index + 1);
                num = num.replace("-", "");
            }

            // 임시로 저장 후 전달
            num = sha256(num);
            accountUser.userID = num.toString();
        }

        try {
            const jsonData = JsonManager.makeUpdateAccountUser(accountUsers, accessedUserID);

            const res = await fetch('/Account/Account/RequestData', {
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
                    return [result, ""];
                }
                else {
                    return [null, result.message];
                }
            }
        }
        catch (e) {
            console.log(e);
        }

        return [null, "updateAccountUsers 실패"];
    }

    static async login(id, pw, isFullVersion) {
        const key = await AccountController.getLoginKey();

        if (!key)
            return null;

        try {
            const pwHash = sha256(pw);
            const strEnc = AccountController.encrypt(id + "|" + pwHash, key);
            const jsonData = JsonManager.makeUserLogin(strEnc, key, isFullVersion);

            const res = await fetch('/Account/Account/RequestData', {
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
                    //AccountController.setLoginUser(result.user);
                    /*const siteID = ProjectResource.SiteID;

                    //세션 값 넣기
                    const user = result.user;
                    window.localStorage.setItem(SessionString.Key.account + "_" + siteID.toString(), JSON.stringify(user));*/
                        
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

    /*
    static setLoginUser(user) {
        const siteID = ProjectResource.SiteID;
        window.localStorage.setItem(SessionString.Key.account + "_" + siteID.toString(), JSON.stringify(user));
    }
    */

    static async autoLogin(beginCode) {
        const key = await AccountController.getLoginKey();

        if (!key)
            return null;

        try {
            const jsonData = JsonManager.makeAutoLogin(beginCode, key);

            const res = await fetch('/Account/Account/RequestData', {
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

    static async getLoginKey() {
        const now = new Date();
        const ticks = now.getTime();

        let key = null;

        try {
            const res = await fetch('/Account/Account/GetLoginKey?num=' + ticks);
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