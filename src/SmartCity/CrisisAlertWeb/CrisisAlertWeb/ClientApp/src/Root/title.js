import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import $ from 'jquery';

import Menu from './menu';
import { AccountController } from '../Account/services/accountController';

import AccountResource from '../Account/resource/id';
import FacilityTypeResource from '../FacilityType/resource/id';
import SessionString from '../Common/js/sessionString';

import styles from '../Common/css/style.css'; 


class Title extends Component {
    constructor(props) {
        super(props);

        this.state = {
            userLevel: null,
            key: null,
        }

        this.props = props;

        this.checkSession();
    }

    componentDidMount() {
        $("#dis").css("display", "none") /* 로딩하자마자 사이드바 안보이게 */

        // 설정 버튼 표시 여부
        if (this.props.navigation === false) {
            $('.setting').hide();
            $('#hamburger-menu').hide();
        }

        //// 로고 표시 여부
        //if (this.props.logo === false) {
        //    $('#logo').hide();
        //}

        //// 타이틀 변경 여부
        //if (this.props.titleName !== null && this.props.titleName !== undefined) {
        //    $('#titleName').innerText = this.props.text;
        //}

        $("#menu-wrapper").click(function (event) {
            event.stopPropagation();
            $("#hamburger-menu").toggleClass("open");
            $("#menu-container .menu-list").toggleClass("active");

            let activeState = $("#menu-container .menu-list").hasClass("active");

            $("#menu-container .menu-list").animate({
                right: activeState ? "0px" : "100%"
            }, 5);

            $("body").toggleClass("overflow-hidden");
        });

        /* 닫기버튼 눌렀을 때 */
        $("#close").click(function () {
            if ($('#dis').css('display') == 'none') {
                $('#dis').show();
            } else {
                $('#dis').hide();
                $("#hamburger-menu").toggleClass("open");
            }
        });

        // 다시 열때
        $("#hamburger-menu").click(function () {
            $("#menu-container .menu-list").removeClass("active")
            $("#dis").css("display", "")
        });

        //스크롤방지
        $("body").bind('touchmove', function (e) {
            e.preventDefault()
        }); 
    }

    async checkSession() {
        let key = null;

        if (window.localStorage.getItem(SessionString.Key.account) === null && window.sessionStorage.getItem(SessionString.Key.account) === null) {
            // 로그인 정보가 없으면 로그인 페이지로 이동
            alert(AccountResource.ID.textLoginSessionError);
            this.props.history.push('/');
        } else if (window.localStorage.getItem(SessionString.Key.account) !== null) {
            key = JSON.parse(window.localStorage.getItem(SessionString.Key.account));
        } else if (window.sessionStorage.getItem(SessionString.Key.account) !== null) {
            key = JSON.parse(window.sessionStorage.getItem(SessionString.Key.account));
        }

        const result = await AccountController.sessionLogin(key);

        if (result === null) {
            alert(AccountResource.ID.textLoginSessionError);
            this.props.history.push('/');
        } else if (result.success === false) {
            alert(result.message);
            this.props.history.push('/');
        } else if (result.user.level !== null && result.user.level !== undefined) {
            this.state.userLevel = result.user.level;
            this.state.key = key;
            this.setState({ userLevel: result.user.level });
        } else {
            alert(AccountResource.ID.textLoginSessionError);
            this.props.history.push('/');
        }
    }

    onClickSetting = () => {
        this.props.history.push(Menu.pathSetting);
    }

    onClickLogout = () => {
        let key = this.state.key;

        if (key === null) {
            alert(AccountResource.ID.textLoginSessionError);
            this.props.history.push('/');
        }

        this.logout(key);
    }

    async logout(key) {
        
        // 키 값 전달하여 로그아웃
        const result = await AccountController.logout(key);

        if (result.success === true) {
            window.sessionStorage.removeItem(SessionString.Key.account);
            window.localStorage.removeItem(SessionString.Key.account);
            alert(result.message);
            this.props.history.push('/');
        }
        else {
            alert(result.message);
        }
    }

    onClickTypeMenu = () => {
        this.props.history.push(Menu.pathTypeMenu);
    }

    showMenu = () => {
        let menu = "";

        if (this.state.userLevel !== null && this.state.userLevel.id === AccountResource.ID.accountLevel.general) {
            // 총괄 관리자 메뉴
            menu =
                <li id="nav2" className="toggle accordion-toggle" onClick={this.onClickTypeMenu}>
                    <p>재난유형</p>
                    <span><img src="/resource/icon/arrow2.png"></img></span>
                </li>;
        } 

        return menu;
    }

    onClickTitle = () => {
        this.props.history.push(Menu.pathMain);
    }

    onClickLink = (path) => {
        this.props.history.push(path);
    }

    render() {
        let showType = "";
        let facilityType = parseInt(window.sessionStorage.getItem(SessionString.Key.facilityType));
        if (facilityType === FacilityTypeResource.ID.facilityType.fire) {
            showType = "화재";
        } else if (facilityType === FacilityTypeResource.ID.facilityType.flood) {
            showType = "홍수";
        } else if (facilityType === FacilityTypeResource.ID.facilityType.heat) {
            showType = "폭염";
        } else if (facilityType === FacilityTypeResource.ID.facilityType.collapse) {
            showType = "경사지 붕괴";
        }

        let menu = "";
        menu = this.showMenu();

        return (
                <div className="header_sub">
                    <embed id="logo" src="/resource/img/daeguCitySlogan.svg"></embed>
                <span>
                    <p id="titleName" onClick={this.onClickTitle}>위기경보수준 관리 시스템</p>
                    </span>
                    <div className="setting"><img src="/resource/icon/Wheel.svg" onClick={this.onClickSetting} /></div>
                    <div id="menu-container">
                        <div id="menu-wrapper">
                            <div id="hamburger-menu"><span></span><span></span><span></span></div>
                        </div>
                        <ul className="menu-list accordion" id="dis">
                            <li id="nav1" className="toggle accordion-toggle">
                            <a href="#">{showType}</a>
                                <p>위기경보수준 관리 시스템</p>
                                <span><img id="close" src="/resource/icon/iconfinder_x_5555-01.svg"></img></span>
                            </li>

                            {menu}

                        <li id="nav3" className="toggle accordion-toggle" onClick={() => this.onClickLink(Menu.pathMain)}>
                                <p>위기경보</p>
                                <span><img src="/resource/icon/arrow2.png"></img></span>
                            </li>
                        <li id="nav4" className="toggle accordion-toggle" onClick={() => this.onClickLink(Menu.pathAlarmList)}>
                                <p>알람이력</p>
                                <span><img src="/resource/icon/arrow2.png"></img></span>
                            </li>
                        <li id="nav5" className="toggle accordion-toggle" onClick={() => this.onClickLink(Menu.pathManualList)}>
                                <p>행동 메뉴얼</p>
                                <span><img src="/resource/icon/arrow2.png"></img></span>
                        </li>
                            <li id="nav6" className="toggle accordion-toggle" onClick={this.onClickLogout} >
                                <img src="/resource/icon/iconfinder_exit_2676937.png" /><a>로그아웃</a>
                            </li>
                        </ul>
                    </div>
                </div>
        );
    }
}

export default withRouter(Title);
