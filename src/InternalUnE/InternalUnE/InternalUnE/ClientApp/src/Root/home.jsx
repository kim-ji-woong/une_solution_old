import React, { Component } from 'react';
import { DefaultContents } from './homeContents/defaultContents';
import { AccountController } from './services/accountController';
import styles from './css/homeStyle.module.css';
import { Register } from '../Account/register';
import { Login } from '../Account/login';
import { NavMenu } from './navMenu';
import { TeamTreeContents } from './homeContents/teamTreeContents';
//import $ from 'jquery';
//import { useEffect, useState } from 'react';

export class Home extends Component {
    static Mode_Detault = 0;

    static ItemMaxCount = 5;

    constructor(props) {
        super(props);

        this.state = {
            newRegist: null,
            contentsMode: Home.Mode_Detault,
            beginIndex: 0
        }

        this.itemCount = 0;
    }


    onLogin = (id, pw) => {
        this.doLogin(id, pw);
    }

    async doLogin(id, pw) {
        const result = await AccountController.login(id, pw);

        if (result?.success) {
            this.props.onLogin(result);
        }
        else {
            if (result.message && result.message.length > 0) {
                alert(result.message);
            }
            else {
                alert("로그인에 실패하였습니다.");
            }

            this.setState({ contentsMode: Home.Mode_Detault });
        }
    }

    onRegist = (name, email) => {
        if (name && email) {
            this.doRegist(name, email);
        }
        else {
            this.setState({ newRegist: { success: null, message: null }});
        }
    }

    async doRegist(name, email) {
        const result = await AccountController.regist(name, email);

        if (result) {
            this.setState({ newRegist: { success: result.success, message: result.message } });
        }
        else {
            this.setState({ newRegist: { success: false, message: "비밀번호가 등록되지 않았습니다." } });
        }
    }

    getContents() {
        if (this.props.currentMenu === NavMenu.Menu_Basic) {
            if (this.state.contentsMode === Home.Mode_Detault) {
                return <DefaultContents />;
            }
        }
        else if (this.props.currentMenu === NavMenu.Menu_TeamTree) {
            if (this.state.contentsMode === Home.Mode_Detault) {
                return <TeamTreeContents />;
            }
        }

        return <></>;
    }

    /*static getHexCode(str) {
        let result = "";

        for (let i = 0; i < str.length; i++) {
            const hex = str.charCodeAt(i).toString(16);

            if (hex.length === 1) {
                result += "0" + hex;
            }
            else {
                result += hex;
            }
        }

        return result;
    }*/

    async onClickLink(url) {
        const loginKey = this.props.loginUser?.loginKey;

        if (loginKey) {
            const beginCode = await this.getNewLoginKey(loginKey);

            if (beginCode) {
                url += '?bc=' + beginCode;
            }
            else
                return;
            /*const beginCode = Home.getHexCode(this.props.loginUser.userID) + "x" + loginKey;
            url += '?bc=' + beginCode;
            window.open(url, '_black').focus();*/
        }
        
        const _window = window.open(url, '_blank');

        if (_window)
            _window.focus();
    }

    async getNewLoginKey(beginCode) {
        const result = await AccountController.requestNewLoginKey(beginCode);

        if (result?.success) {
            const loginUser = this.props.loginUser;

            if (loginUser) {
                loginUser.loginKey = result.loginKey;
                return result.loginKey;
            }
        }

        return null;
    }

    getItemContents() {
        const items = [];

        items.push(
            <div className={styles.menuTab1}>
                <span className={styles.tab1Icon} onClick={() => this.onClickLink('http://www.internalune.com:8088')}></span>
                <span className={styles.tab1Text}>휴가관리 사이트</span>
            </div>
        );

        items.push(
            <div className={styles.menuTab2}>
                <span className={styles.tab2Icon} onClick={() => this.onClickLink('http://www.internalune.com:8090')}></span>
                <span className={styles.tab2Text}></span>
            </div>
        );

        items.push(
            <div className={styles.menuTab3}>
                <span className={styles.tab3Icon} onClick={() => this.onClickLink('http://www.internalune.com:44333')}></span>
                <span className={styles.tab3Text}></span>
            </div>
        );

        /*items.push(
            <div className={styles.menuTab4}>
                <span className={styles.tab4Icon} onClick={() => this.onClickLink('http://www.internalune.com:8089')}></span>
                <span className={styles.tab4Text}>SensorMaker</span>
            </div>
        );*/

        items.push(
            <div className={styles.menuTab4}>
                <span className={styles.tab5Icon} onClick={() => this.onClickLink('https://gigasafe.kt.com/mnc_5g/login/getLoginView')}></span>
                <span className={styles.tab5Text}>KT GiGA 2020</span>
            </div>
        );

        items.push(
            <div className={styles.menuTab5}>
                <span className={styles.tab5Icon} onClick={() => this.onClickLink('https://gigasafe.kt.com/mnc/login/getLoginView')}></span>
                <span className={styles.tab5Text}>KT GiGA 2021</span>
            </div>
        );

        items.push(
            <div className={styles.menuTab6}>
                <span className={styles.tab6Icon} onClick={() => this.onClickLink('http://221.147.100.165:8088')}></span> 
                <span className={styles.tab6Text}>청량리 수자인</span>
            </div>
        );

        items.push(
            <div className={styles.menuTab6}>
                <span className={styles.tab7Icon} onClick={() => this.onClickLink('http://15.165.91.226')}></span>
                <span className={styles.tab6Text}>Udesign Viewer</span>
            </div>
        );

        items.push(
            <div className={styles.menuTab6}>
                <span className={styles.tab8Icon} onClick={() => this.onClickLink('https://gigasafe.kt.com/mnc/#/login')}></span>
                <span className={styles.tab6Text}>KT city</span>
            </div>
        );

        items.push(
            <div className={styles.menuTab6}>
                <span className={styles.tab9Icon} onClick={() => this.onClickLink('http://221.147.100.165:8080')}></span>
                <span className={styles.tab9Text}>원익</span>
            </div>
        );

        items.push(
            <div className={styles.menuTab6}>
                <span className={styles.tab10Icon} onClick={() => this.onClickLink('http://221.147.100.161:8091')}></span>
                <span className={styles.tab10Text}>경기도청</span>
            </div>
        );


        const beginIndex = this.state.beginIndex;
        const itemCount = items.length;
        const itemContents = [];

        this.itemCount = itemCount;

        for (let i = beginIndex; i < beginIndex + Home.ItemMaxCount && i < itemCount; i++) {
            itemContents.push(items[i]);
        }

        return (
            <div className={styles.menuFloat}>
                {itemContents}
            </div>
            );
    }


    getLeftButtonClassName() {
        if (this.itemCount <= Home.ItemMaxCount) {
            return styles.btnSlide + " " + styles.hidden;
        }

        if (this.state.beginIndex === 0) {
            return styles.btnSlide + " " + styles.disabled;
        }

        return styles.btnSlide;
    }

    getRightButtonClassName() {
        if (this.itemCount <= Home.ItemMaxCount) {
            return styles.btnSlide + " " + styles.hidden;
        }

        if (this.state.beginIndex + Home.ItemMaxCount >= this.itemCount) {
            return styles.btnSlide + " " + styles.disabled;
        }

        return styles.btnSlide;
    }

    onClickSlide(next) {
        if (next) {
            if (this.state.beginIndex + Home.ItemMaxCount < this.itemCount)
                this.setState({ beginIndex: this.state.beginIndex + 1 });
        }
        else {
            if (this.state.beginIndex > 0)
                this.setState({ beginIndex: this.state.beginIndex - 1 });
        }
    }

    render() {
        let login = null;
        let regist = null;

        const user = this.props.loginUser;

        if (!user) {
            if (this.props.currentMenu === NavMenu.Menu_Login && this.state.newRegist) {
                regist = <Register onRegist={this.onRegist} response={this.state.newRegist} />;
            }
            else if (this.props.currentMenu === NavMenu.Menu_Login) {
                login = <Login onLogin={this.onLogin} onRegist={this.onRegist} />;
            }
        }

        const itemContents = this.getItemContents();

        return (
            <div>
                {/*user*/}
                {login}
                {regist}
                <div className={styles.bodyArea}>
                    {
                        this.getContents()
                    }
                    <div className={styles.bodyBottom}>
                        <div className={styles.bodyBottomLeft}>
                            <div className={this.getLeftButtonClassName()} onClick={() => this.onClickSlide(false)}>
                                <i className="fas fa-chevron-left"></i>
                            </div>
                        </div>
                        <div className={styles.menuTabArea}>

                            {itemContents}
                            {
                            /*<div className={styles.menuFloat1}>
                                <div className={styles.menuTab1}>
                                    <span className={styles.tab1Icon} onClick={() => this.onClickLink('http://www.internalune.com:8088')}></span>
                                    <span className={styles.tab1Text}>휴가관리 사이트</span>
                                </div>
                                <div className={styles.menuTab2}>
                                    <span className={styles.tab2Icon} onClick={() => this.onClickLink('http://www.internalune.com:8090')}></span>
                                    <span className = {styles.tab2Text}></span>
                                </div>
                            </div>
                            <div className={styles.menuFloat2}>
                                <div className={styles.menuTab3}>
                                    <span className={styles.tab3Icon} onClick={() => this.onClickLink('http://www.internalune.com:44333')}></span>
                                    <span className = {styles.tab3Text}></span>
                                </div>
                                <div className={styles.menuTab4}>
                                    <span className={styles.tab1Icon} onClick={() => this.onClickLink('http://www.internalune.com:8089')}></span>
                                    <span className={styles.tab1Text}>SensorMaker</span>
                                </div>
                                <div className={styles.menuTab4}>
                                    <span className={styles.tab1Icon} onClick={() => this.onClickLink('https://gigasafe.kt.com/mnc_5g/login/getLoginView')}></span>
                                    <span className={styles.tab1Text}>KT GiGA 2020</span>
                                </div>
                            </div>
                            <div className={styles.menuFloat3}>
                                <div className={styles.menuTab5}>
                                    <span className={styles.tab1Icon} onClick={() => this.onClickLink('https://gigasafe.kt.com/mnc/login/getLoginView')}></span>
                                    <span className={styles.tab1Text}>KT GiGA 2021</span>
                                </div>
                            </div>*/
                            }
                        </div>
                        <div className={styles.bodyBottomRight}>
                            <div className={this.getRightButtonClassName()} onClick={() => this.onClickSlide(true)}>
                                <i className="fas fa-chevron-right"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}
