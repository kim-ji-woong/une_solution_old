import CryptoJS from 'crypto-js';
import sha256 from 'crypto-js/sha256';
import { JsonManager } from './jsonManager';

import SessionString from '../../Common/js/sessionString';
import RootResource from '../../Root/resource/id';
import AccountStore from '../accountStore';
import AccountResource from '../resource/id';
import * as Common from '../../Common/data/common';
import * as AccountData from './accountData';

export class AccountController {
    static timerCheck = false;
    static loading3DChk = false;

    static StartWatchTimer() {
        // 타이머 실행 유무 판단
        if (AccountController.timerCheck == true)
            return;

        // 타이머 실행 체크
        AccountController.timerCheck = true;

        let timerLogin = setTimeout(function tick() {
            AccountController.WatchLoginCheck();
            timerLogin = setTimeout(tick, 5000);
        }, 5000);
    }

    static async WatchLoginCheck() {
        //console.log("WatchLoginCheck()");
        const user = JSON.parse(window.localStorage.getItem(SessionString.Key.account));

        if (user === null || user.sessionKey === null || user.sessionKey === undefined) {
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
                let message = "서버와 연결이 끊어졌습니다.<br/>연결 중입니다.<br/>(확인 버튼 클릭 시 로그인 화면으로 이동합니다.)";

                return [AccountResource.loginState.disconnected, message];
            }

        }

        // 요청 중에 페이지 이동 시 응답을 받지 못하는 경우가 발생할 수 있음. 
        return [AccountResource.loginState.login, "checkLoginSession 실패하였습니다."];
    }

    static async login(id, pw) {
        const key = await AccountController.getLoginKey();

        if (!key)
            return null;

        try {
            const pwHash = sha256(pw);
            const strEnc = AccountController.encrypt(id + "|" + pwHash, key);
            const jsonData = JsonManager.makeUserLogin(strEnc, key);

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
                    //세션 값 넣기
                    const user = result.user;
                    window.localStorage.setItem(SessionString.Key.account, JSON.stringify(user));
                }

                return result;
            } else {
                const result = new AccountData.LoginResult();
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