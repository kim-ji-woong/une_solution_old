import React, { Component } from 'react';
import { BrowserRouter as Route, Link } from 'react-router-dom';
import { withRouter } from 'react-router-dom';
import SessionString from '../Common/js/sessionString';
import { SDMSController } from '../SDMS/services/sdmsController';
import SopSimulatorController from '../SOPSimulator/services/sopSimulatorController';
import { AccountController } from '../Account/services/accountController';
import { TeamEditController } from '../TeamEditor/services/teamEditController';
import { SettingController } from '../Settings/services/settingController';
import { DashboardController } from '../Dashboard/services/dashboardController';
import LayoutSetting from '../Settings/ui/popups/layoutSetting';
import SettingsStore from '../Settings/settingsStore';
import Store from './store';

import uis from '../Common/css/ui.module.css';
import contents from '../Common/css/content.module.css';
import styles from '../Common/css/style.module.css';
import titleBarSBs from './css/titleBarSB.module.css';
import newStyles from '../Common/css/newStyle.module.css';
import uneCommon from '../Common/css/uneCommon.module.css';
import dashboard from '../Dashboard/css/dashboardNew.module.css';

import logo from '../Common/image/common/logo.png';
import Ulogout from '../Common/img/common/user_logout.png';
import Umanagement from '../Common/img/common/user_management.png';
import Upassword from '../Common/img/common/user_password.png';
import $ from 'jquery';
import SDMSMenuBtn from '../SDMS/ui/sdmsMenuBtn';
import SopSimulatorMenuBtn from '../SOPSimulator/ui/sopSimulatorMenuBtn';
import AccountManager from '../Account/ui/popups/accountManager';
import AccountRegister from '../Account/ui/popups/accountRegister';
import ConfirmDialog from '../Common/ui/confirmDialog';

import AccountResource from '../Account/resource/id';
import SettingResource from '../Settings/resource/id';
import RootResource from './resource/id';
import AccountStore from '../Account/accountStore';

import ProjectResource from './resource/id';
import SopController from '../SOPManager/services/sopController';

import AccountChangePwd from '../Account/ui/popups/accountChangePwd';

class TitleBarSB extends Component {
    static pathSDMS = '/sdms';
    static pathSOPSimulator = '/sop-simulator';
    static pathSopManager = '/sop-manager';
    static pathTeamEditor = '/team-editor';
    static pathDashboard = '/dashboard';
    static pathHistory = '/history';

    static keys = [];
    static shortcutKey = null;

	constructor(props) {
        super(props);

        this.state = {
            popupOpen: false,
            settingOnOff: false,
            opneChangePwd: false,

            confirmMessage: {
                visible: false,
                title: "",
                messages: [""],
                buttons: ["확인"],
                onClose: this.onCloseConfirmDialog,
                onClickButton: null
            },
            loading: true,
            reload: null,
        }

        this.props = props;

        SettingsStore.subscribe(function () {
            let data = SettingsStore.getState();

            if (data.actionType === 'SHORTCUT_KEY') {
                TitleBarSB.shortcutKey = data.shortcutKey;
            }
        }.bind(this));

        AccountStore.subscribe(function () {
            let data = AccountStore.getState();

            if (data.actionType === 'LOGIN_STATE') {
                this.checkLoginState(data);
            }
        }.bind(this));

        this.initSiteID();
    }

	componentDidUpdate() {
		//console.log('componentDidUpdate');
	}

	componentWillMount() {
		//console.log('componentWillMount');
	}

	componentWillUpdate(nextProps, nextState) {
		//console.log('componentWillUpdate');
	}

	componentDidMount() {
		//console.log('componentDidMount');

        // 센서 히스토리 감시 타이머 시작
        SDMSController.StartWatchTimer();
        SopSimulatorController.StartWatchTimer();
        SettingController.StartWatchTimer();


        // 로그인 세션 감시 타이머 
        AccountController.StartWatchTimer();

        // 다른 곳 클릭했을때 이벤트 발생
        $('#mainSB').click(function (e) {

            if ($('.rqQckBtn').hasClass('on') || $('.rqAppBtn').hasClass('on') || $('.rqUsrBtn').hasClass('on')) {
                let targetName = e.target.className;

                if (targetName === "") {
                    $('.rqQckBtn').next().hide();
                    $('.rqQckBtn').removeClass('on');
                    $('.rqAppBtn').next().hide();
                    $('.rqAppBtn').removeClass('on');
                    $('.rqUsrBtn').next().hide();
                    $('.rqUsrBtn').removeClass('on');
                }
            }
        });

        $('.rqBtn button').click(function () {
            if ($(this).is('.on')) {
                $(this).next().hide();
                $(this).removeClass('on');
            } else {
                $('.rqBtn > ul, .rqBtn > div').hide();
                $('.rqBtn button').removeClass('on');
                $(this).next().show();
                $(this).addClass('on');
            }
        });

        // 메뉴 선택 시 닫힘
        $('.' + newStyles.rqQck + ' ul li').click(function (e) {
            $('.rqQckBtn').next().hide();
            $('.rqQckBtn').removeClass('on');
        });
        $('.' + newStyles.rqApp + ' ul li').click(function (e) {
            $('.rqAppBtn').next().hide();
            $('.rqAppBtn').removeClass('on');
        });

        $(document).mouseup(function (e) {
            if ($('.rqBtn').has(e.target).length === 0) {
                $('.rqBtn > ul, .rqBtn > div').hide();
                $('.rqBtn button').removeClass('on');
            }
        });

        // 단축키 이벤트 리스너
        window.addEventListener("keydown", (e) => this.keysPressed(e, this), false);
        window.addEventListener("keyup", (e) => this.keysReleased(e, this), false);
    }

    keysPressed(e, target) {
        // store an entry for every key pressed
        TitleBarSB.keys[e.keyCode] = true;

        const commonKey = 18;   // Alt 키
        
        // 단축키 설정 가져오기
        let shortcutKey = TitleBarSB.shortcutKey;

        if (shortcutKey === null || shortcutKey === undefined) {
            return;
        }

        if (TitleBarSB.keys[commonKey] && TitleBarSB.keys[parseInt(shortcutKey.sdms)]) {
            // sdms 단축키
            let url = window.location.origin + RootResource.path.sdms;
            window.open(url, "_blank");
            target.hideShortCutKey();

            TitleBarSB.keys[commonKey] = false;
            TitleBarSB.keys[parseInt(shortcutKey.sdms)] = false;
            // prevent default browser behavior
            e.preventDefault();
        } else if (TitleBarSB.keys[commonKey] && TitleBarSB.keys[parseInt(shortcutKey.settings)]) {
            // settings 단축키
            console.log("settings 단축키");
            target.onClickSetting();

            TitleBarSB.keys[commonKey] = false;
            TitleBarSB.keys[parseInt(shortcutKey.settings)] = false;
            // prevent default browser behavior
            e.preventDefault();
        } else if (TitleBarSB.keys[commonKey] && TitleBarSB.keys[parseInt(shortcutKey.teamEdit)]) {
            // teamEdit 단축키
            console.log("teamEdit 단축키");
            let url = window.location.origin + RootResource.path.teamEditor;
            window.open(url, "_blank");
            target.hideShortCutKey();

            TitleBarSB.keys[commonKey] = false;
            TitleBarSB.keys[parseInt(shortcutKey.teamEdit)] = false;
            // prevent default browser behavior
            e.preventDefault();
        } else if (TitleBarSB.keys[commonKey] && TitleBarSB.keys[parseInt(shortcutKey.home)]) {
            // home 단축키
            console.log("home 단축키");
            target.onClickLogo();
            target.hideShortCutKey();

            TitleBarSB.keys[commonKey] = false;
            TitleBarSB.keys[parseInt(shortcutKey.home)] = false;
            // prevent default browser behavior
            e.preventDefault();
        } else if (TitleBarSB.keys[commonKey]) {
            // 단축키 도움말
            console.log("단축키 도움말 단축키");
            target.showShortCutKey();

            // prevent default browser behavior
            e.preventDefault();
        }
        else if (TitleBarSB.keys[commonKey] && TitleBarSB.keys[parseInt(shortcutKey.sop)]) {
            console.log("sop 단축키");
            let url = window.location.origin + RootResource.path.sopSimulator;
            window.open(url, "_blank");
            target.hideShortCutKey();

            TitleBarSB.keys[commonKey] = false;
            TitleBarSB.keys[parseInt(shortcutKey.sop)] = false;
            // prevent default browser behavior
            e.preventDefault();
        } else if (TitleBarSB.keys[commonKey] && TitleBarSB.keys[parseInt(shortcutKey.sopMgr)]) {
            // sopMgr 단축키
            let url = window.location.origin + RootResource.path.sopManager;
            window.open(url, "_blank");
            target.hideShortCutKey();

            TitleBarSB.keys[commonKey] = false;
            TitleBarSB.keys[parseInt(shortcutKey.sopMgr)] = false;
            // prevent default browser behavior
            e.preventDefault();
        } else if (TitleBarSB.keys[commonKey] && TitleBarSB.keys[parseInt(shortcutKey.dashboard)]) {
            // dashboard 단축키
            let url = window.location.origin + RootResource.path.dashboard;
            window.open(url, "_blank");
            target.hideShortCutKey();

            TitleBarSB.keys[commonKey] = false;
            TitleBarSB.keys[parseInt(shortcutKey.dashboard)] = false;
            // prevent default browser behavior
            e.preventDefault();
        } else if (TitleBarSB.keys[commonKey] && TitleBarSB.keys[parseInt(shortcutKey.history)]) {
            // history 단축키
            let url = window.location.origin + RootResource.path.history;
            window.open(url, "_blank");
            target.hideShortCutKey();

            TitleBarSB.keys[commonKey] = false;
            TitleBarSB.keys[parseInt(shortcutKey.history)] = false;
            // prevent default browser behavior
            e.preventDefault();
        }
        
    }

    keysReleased(e, target) {
        // mark keys that were released
        TitleBarSB.keys[e.keyCode] = false;

        if (e.keyCode === 18) {
            target.hideShortCutKey();
        }
    }

    async initSiteID() {
        let siteID = ProjectResource.SiteID;

        if (siteID === null || siteID === undefined) {
            // 사이트 ID 요청
            const [result, message] = await SDMSController.requestGetSiteID();

            if (result !== null && result !== undefined) {
                ProjectResource.SiteID = result;
            }

            this.setState({ reload: true });
        }

        // 솔브레인 작업현황 정보 읽어오기 관련 타이머
        if (ProjectResource.SiteID !== null && ProjectResource.SiteID !== undefined && ProjectResource.SiteID === ProjectResource.Site.Soulbrain) {
            DashboardController.StartWatchTimer();
        }
    }

    showShortCutKey = () => {
        // 단축키 도움말 표시 관련 클래스 제거로 단축키 도움말 표시
        $(".shortcutKey").removeClass(uis.hideKey);
    }

    hideShortCutKey = () => {
        // 단축키 도움말 표시 관련 클래스 추가로 단축키 도움말 숨김
        $(".shortcutKey").addClass(uis.hideKey);
    }

	onClickLogout = () => {
        // 계정 리덕스에 상태 업데이트
        AccountStore.dispatch({ type: 'LOGIN_STATE', loginState: AccountResource.loginState.logout, message: "로그아웃 하였습니다." });
    }

    checkLoginState = (data) => {
        if (data === null || data === undefined ||
            data.loginState === null || data.loginState === undefined)
            return;

        if (data.loginState === AccountResource.loginState.logout) {
            // 로그아웃 시
            // 로그인 페이지로 이동
            //window.location.href = RootResource.path.root;
            this.props.history.push(RootResource.path.root);
            // 세션 초기화
            //window.localStorage.removeItem(SessionString.Key.account);
            const siteID = ProjectResource.SiteID;

            if (siteID !== null && siteID !== undefined) {
                //window.localStorage.removeItem(SessionString.Key.account);
                window.localStorage.removeItem(SessionString.Key.account + "_" + siteID.toString());
            }
        } else if (data.loginState === AccountResource.loginState.disconnected) {
            // 네트워크 연결 끊김 시
            //alert(data.message);
            this.showConfirmDialog("오류", [data.message], ["확인"], this.onClickFalseConfirm, this.onClickFalseConfirm);
        } else if (data.loginState === AccountResource.loginState.false) {
            // 세션 조회 실패 시
            // 로그인 페이지로 이동
            //window.location.href = RootResource.path.root;

            //alert(data.message);
            this.showConfirmDialog("오류", [data.message], ["확인"], this.onClickFalseConfirm, this.onClickFalseConfirm);
            // 세션 초기화
            //window.localStorage.removeItem(SessionString.Key.account);
        } else if (data.loginState === AccountResource.loginState.login) {
            this.onCloseConfirmDialog();
        }
    }

    onClickFalseConfirm = () => {
        //window.location.href = RootResource.path.root;
        this.props.history.push(RootResource.path.root);

        // 세션 초기화
        //window.localStorage.removeItem(SessionString.Key.account);
        const siteID = ProjectResource.SiteID;

        if (siteID !== null && siteID !== undefined) {
            window.localStorage.removeItem(SessionString.Key.account + "_" + siteID.toString());
        }
    }

    showConfirmDialog = (title, messages, buttons, onClickButton, onClickClose) => {
        const confirmMessage = { ...this.state.confirmMessage };
        confirmMessage.visible = true;
        confirmMessage.title = title;
        confirmMessage.buttons = buttons;
        confirmMessage.onClickButton = onClickButton;

        if (onClickClose !== null && onClickClose !== undefined)
            confirmMessage.onClose = onClickClose;

        if (!messages) {
            confirmMessage.messages = [""];
        }
        else if (Array.isArray(messages)) {
            confirmMessage.messages = messages;
        }
        else {
            confirmMessage.messages = [messages];
        }

        this.setState({ confirmMessage });
    }

    onCloseConfirmDialog = () => {
        const confirmMessage = { ...this.state.confirmMessage };
        confirmMessage.visible = false;

        this.setState({ confirmMessage });
    }

    onClickLogo() {
        if (this.props.menuEvent && this.props.menuEvent.onClickLogo) {
            this.props.menuEvent.onClickLogo();
        }
    }

    static onClickTemp = () => {
        console.log("onClickTemp 실행");
        if (TitleBarSB.props.menuEvent && TitleBarSB.props.menuEvent.onClickLogo) {
            TitleBarSB.props.menuEvent.onClickLogo();
        }
    }

    onClickSetting = () => {
        this.setState({ settingOnOff: true });
    }

    getTargetMenus() {
        if (this.props.target === "sdms") {
            return <SDMSMenuBtn menuEvent={this.props.menuEvent} />
        } else if (this.props.target === "sop-simulator") {
            return <SopSimulatorMenuBtn menuEvent={this.props.menuEvent} />;
        } else {
            return (
                /*
                <div className={uis.fileWrap}>
                    <button type="button" className={uis.btnFile}><i className={uis.iconFile}></i></button>
                </div>
                */
                <div className={newStyles.rqQck + " rqBtn"}>
                    <button className="rqQckBtn">메뉴열기</button>
                </div>
            );
        }
    }

    getLinkMenu() {
        const path = window.location.pathname;
        let linkMenuUI = [];

        // 단축키 설정 가져오기
        let shortcutKey = TitleBarSB.shortcutKey;
        let sdms = "";
        let sop = "";
        let sopMgr = "";
        let dashboard = "";
        let history = "";
        let teamEdit = "";

        if (shortcutKey !== null && shortcutKey !== undefined) {
            if (shortcutKey.sdms !== null && shortcutKey.sdms !== undefined && shortcutKey.sdms !== "") {
                let key = String.fromCharCode(shortcutKey.sdms);
                sdms = <span className={"shortcutKey" + " " + uis.menuShortCut + " " + uis.hideKey}>Al+{key}</span>;
            }
            if (shortcutKey.sop !== null && shortcutKey.sop !== undefined && shortcutKey.sop !== "") {
                let key = String.fromCharCode(shortcutKey.sop);
                sop = <span className={"shortcutKey" + " " + uis.menuShortCut + " " + uis.hideKey}>Al+{key}</span>;
            }
            if (shortcutKey.sopMgr !== null && shortcutKey.sopMgr !== undefined && shortcutKey.sopMgr !== "") {
                let key = String.fromCharCode(shortcutKey.sopMgr);
                sopMgr = <span className={"shortcutKey" + " " + uis.menuShortCut + " " + uis.hideKey}>Al+{key}</span>;
            }
            if (shortcutKey.dashboard !== null && shortcutKey.dashboard !== undefined && shortcutKey.dashboard !== "") {
                let key = String.fromCharCode(shortcutKey.dashboard);
                dashboard = <span className={"shortcutKey" + " " + uis.menuShortCut + " " + uis.hideKey}>Al+{key}</span>;
            }
            if (shortcutKey.history !== null && shortcutKey.history !== undefined && shortcutKey.history !== "") {
                let key = String.fromCharCode(shortcutKey.history);
                history = <span className={"shortcutKey" + " " + uis.menuShortCut + " " + uis.hideKey}>Al+{key}</span>;
            }
            if (shortcutKey.teamEdit !== null && shortcutKey.teamEdit !== undefined && shortcutKey.teamEdit !== "") {
                let key = String.fromCharCode(shortcutKey.teamEdit);
                teamEdit = <span className={"shortcutKey" + " " + uis.menuShortCut + " " + uis.hideKey}>Al+{key}</span>;
            }
        }

        let linkSDMS = <Link to="/sdms" target="_blank">3D관제화면</Link>;
        let linkSOPSimulator = <Link to="/sop-simulator" target="_blank">SOP</Link>;
        let linkSOPManager = <Link to="/sop-manager" target="_blank">SOP 편집</Link>;
        let linkTeamEditor = <Link to="/team-editor" target="_blank">조직관리</Link>;
        let linkDashBoard = <Link to="/dashboard" target="_blank">대시보드</Link>;
        let linkCCTV = <a>CCTV</a>;
        let linkHistory = <Link to="/history" target="_blank">이력</Link>;

        if (path === RootResource.path.sdms) {
            linkSDMS = <a>3D관제화면</a>;
        } else if (path === RootResource.path.sopSimulator) {
            linkSOPSimulator = <a>SOP</a>;
        } else if (path === RootResource.path.sopManager) {
            linkSOPManager = <a>SOP 편집</a>;
        } else if (path === RootResource.path.teamEditor) {
            linkTeamEditor = <a>조직관리</a>;
        } else if (path === RootResource.path.dashboard) {
            linkDashBoard = <a>대시보드</a>;
        } else if (path === RootResource.path.history) {
            linkHistory = <a>이력</a>;
        }

        const userAuthor = ProjectResource.getUserAuthor();

        // 계정 권한 별 표시
        if (userAuthor === AccountResource.ID.accountLevel.admin) {
            linkMenuUI.push(<React.Fragment key={"linkAdmin"}>
                <li>{sdms}{linkSDMS}</li>
                <li>{teamEdit}{linkTeamEditor}</li>
                <li>{dashboard}{linkDashBoard}</li>
                <li>{sop}{linkSOPSimulator}</li>
                <li>{sopMgr}{linkSOPManager}</li>
                <li>{history}{linkHistory}</li>
            </React.Fragment>);
        } else {
            linkMenuUI.push(<React.Fragment key={"linkUser"}>
                <li>{sdms}{linkSDMS}</li>
                <li>{teamEdit}{linkTeamEditor}</li>
                <li>{dashboard}{linkDashBoard}</li>
                <li>{sop}{linkSOPSimulator}</li>
                <li>{history}{linkHistory}</li>
            </React.Fragment>);
        }
        
        return linkMenuUI;
        
    }

    onClickAccountMgr = () => {
        this.setState({ popupOpen: true });
    }

    onClickClosePopup = (value) => {
        this.setState({ popupOpen: value });
        //this.state.popupOpen = value;
        //this.props.close(value);
    }

    settingOff = () => {
        this.setState({ settingOnOff: false });
    }

    getTitleNameUI() {
        const path = window.location.pathname;

        if (path === RootResource.path.sdms) {
            return (<></>);
        } else if (path === RootResource.path.teamEditor) {
            return (
                <div id={newStyles.hsTop}>
                    <h2 className={newStyles.hstTitle}>{RootResource.ID.title.teamEditor}</h2>
                </div>);
        } 
        else if (path === RootResource.path.sopSimulator) {
            return (
                <div id={newStyles.hsTop}>
                    <h2 className={newStyles.hstTitle}>{RootResource.ID.title.sopSimulator}</h2>
                </div>);
        } else if (path === RootResource.path.sopManager) {
            return (
                <div id={newStyles.hsTop}>
                    <h2 className={newStyles.hstTitle}>{RootResource.ID.title.sopManager}</h2>
                </div>);
        } else if (path === RootResource.path.dashboard) {
            return (
                <div id={newStyles.hsTop} className={dashboard.hsTop}>
                    <h2 className={newStyles.hstTitle}>{RootResource.ID.title.dashboard}</h2>
                </div>);
        } else if (path === RootResource.path.history) {
            return (
                <div id={newStyles.hsTop}>
                    <h2 className={newStyles.hstTitle}>{RootResource.ID.title.history}</h2>
                </div>);
        }
        
    }

    getUserInfo() {
        let userName = "-";
        let userLevel = "-";

        let userInfo = ProjectResource.getUserInfo();
        if (userInfo !== null && userInfo !== undefined) {
            userName = userInfo.nickName;
            userLevel = userInfo.level;
        }

        if (userLevel === AccountResource.ID.accountLevel.admin) {
            return (
                <div className={newStyles.rqUsr + " rqBtn"}>
                    <button className="rqUsrBtn"><span className="rqUsrSpan">{userName}</span></button>
                    <div>
                        <em className={uneCommon.adminProfile}>프로필사진</em>
                        <span>{userLevel}</span>
                        <p>{userName}</p>
                        <ul>
                            <li><a onClick={this.onClickAccountMgr} title="사용자 관리"><img src={Umanagement} alt="사용자관리" className={newStyles.Amanagement} title="사용자 관리" /></a></li>
                            <li><a onClick={this.onClickChangePwd} title="비밀번호 변경"><img src={Upassword} alt="비밀번호 변경" className={newStyles.Apassword} title="비밀번호 변경" /></a></li>
                            <li><a onClick={this.onClickLogout} title="로그아웃"><img src={Ulogout} alt="로그아웃" className={newStyles.Alogout} title="로그아웃" /></a></li>
                        </ul>
                    </div>
                </div>
            );
        } else {
            return (
                <div className={newStyles.rqUsr + " rqBtn " + newStyles.user}>
                    <button className="rqUsrBtn"><span className="rqUsrSpan">{userName}</span></button>
                    <div>
                        <em className={uneCommon.userProfile}>프로필사진</em>
                        <span>{userLevel}</span>
                        <p>{userName}</p>
                        <ul>
                            <li className={newStyles.rqli} title="비밀번호 변경"><a onClick={this.onClickChangePwd}><img src={Upassword} alt="비밀번호 변경" className={newStyles.Upassword} title="비밀번호 변경" /></a></li>
                            <li className={newStyles.rqli} title="로그아웃"><a onClick={this.onClickLogout}><img src={Ulogout} alt="로그아웃" className={newStyles.Ulogout} title="로그아웃" /></a></li>
                        </ul>
                    </div>
                </div>
            );
        }
        
    }

    getLogo() {
        // 단축키 설정 가져오기
        let shortcutKey = TitleBarSB.shortcutKey;
        let home = <></>;

        if (shortcutKey !== null && shortcutKey !== undefined) {
            if (shortcutKey.home !== null && shortcutKey.home !== undefined && shortcutKey.home !== "") {
                let key = String.fromCharCode(shortcutKey.home);
                home = <span className={"shortcutKey" + " " + uis.logoShortCut + " " + uis.hideKey}>Al+{key}</span>;
            }
        }

        if (ProjectResource.SiteID === ProjectResource.Site.Soulbrain) {
            return <h1 className={newStyles.rqLogo} onClick={() => this.onClickLogo()}>{home}</h1>
        }
        else if (ProjectResource.SiteID === ProjectResource.Site.GCC) {
            return <h1 className={newStyles.rqLogoGC} onClick={() => this.onClickLogo()}>{home}</h1>
        }

        return <></>
    }

    onClickChangePwd = () => {
        this.setState({ opneChangePwd: true});
    }

    onClickCloseChangePwd = () => {
        this.setState({ opneChangePwd: false });
    }

    getSettingKey = () => {
        let shortcutKey = TitleBarSB.shortcutKey;
        let settings = <></>;

        if (shortcutKey !== null && shortcutKey !== undefined) {
            if (shortcutKey.settings !== null && shortcutKey.settings !== undefined && shortcutKey.settings !== "") {
                let key = String.fromCharCode(shortcutKey.settings);
                settings = <span className={"shortcutKey" + " " + uis.setShortCut + " " + uis.hideKey}>Al+{key}</span>;
            }
        }

        return settings;
    }

    render() {
        return (
            <>                
                <div id={newStyles.rqMenu}>
                    {
                        this.getLogo()
                    }
                    {
                        this.getTargetMenus()
                    }
                    <div className={newStyles.rqApp + " rqBtn"}>
                        <button className="rqAppBtn">메뉴열기</button>
                        <ul>
                            { this.getLinkMenu() }
                        </ul>
                    </div>
                    { this.getUserInfo() }
                    <a onClick={this.onClickSetting} className={newStyles.rqStng}>{this.getSettingKey()}</a>
                </div>
                
                { this.getTitleNameUI() }

                <DisplayPopup open={this.state.popupOpen} mode={AccountResource.ID.popupMode.manager} onClickClosePopup={this.onClickClosePopup} />

                
                {
                    /* 환경설정 팝업 */
                    /*<DisplaySetting settingOnOff={this.state.settingOnOff} settingOff={this.settingOff} />*/
                    this.state.settingOnOff &&
                    <LayoutSetting
                        settingOff={this.settingOff}
                        settingOnOff={this.state.settingOnOff}

                    />
                }

                {
                    /* 비밀번호 변경 팝업 */
                    this.state.opneChangePwd &&
                    <AccountChangePwd onClickCloseChangePwd={this.onClickCloseChangePwd} />
                }

                {
                    /* alert창 대신 사용 */
                    this.state.confirmMessage.visible &&
                    <ConfirmDialog title={this.state.confirmMessage.title} messages={this.state.confirmMessage.messages} buttons={this.state.confirmMessage.buttons} onClose={this.state.confirmMessage.onClose} onClickButton={this.state.confirmMessage.onClickButton} />
                }
            </>
		);
	}
}

export default withRouter(TitleBarSB);


class DisplayPopup extends Component {

    constructor(props) {
        super(props);

        this.state = {
            open: false,
            popupMode: null,

            regulars: null,             // 부서
            accountLevels: null,        // 권한
            accountUsers: null,         // 유저 리스트
        }

        this.props = props;
        this.init();
    }

    async init() {
        const accountLevels = await AccountController.getAccountLevels();
        const regulars = await TeamEditController.GetRegular();
        let accountUsers = await AccountController.getAccountUsers();

        this.setState({ regulars: regulars, accountLevels: accountLevels, accountUsers: accountUsers });
    }

    componentDidUpdate(prevProps, prevState) {
        if (prevProps.open !== this.props.open) {
            // 외부에서 열고 닫았을때
            this.setState({ open: this.props.open, popupMode: this.props.mode });
        }
    }

    componentWillUpdate(nextProps, nextState) {
        //console.log('componentWillUpdate');

        if (nextProps.open !== this.props.open && nextProps.open === true) {
            this.reload();
        }
    }

    onClickClosePopup = (value) => {
        this.setState({ open: value });
        this.props.onClickClosePopup(value);

        return;
    }

    onClickCancle = () => {
        this.setState({ popupMode: AccountResource.ID.popupMode.manager});
    }

    onClickRegister = () => {
        this.setState({ popupMode: AccountResource.ID.popupMode.register});
    }

    onClickConfirm = () => {
        this.setState({ popupMode: AccountResource.ID.popupMode.manager });

        this.reload();
    }

    onChangeReload = () => {
        this.reload();
    }

    async reload() {
        const accountUsers = await AccountController.getAccountUsers();
        const regulars = await TeamEditController.GetRegular();

        this.setState({ regulars: regulars, accountUsers: accountUsers });
    }

    render() {
        if (this.state.open === true && this.state.popupMode === AccountResource.ID.popupMode.manager) {
            return <AccountManager onClickClosePopup={this.onClickClosePopup} onClickRegister={this.onClickRegister} onChangeReload={this.onChangeReload} regulars={this.state.regulars} accountLevels={this.state.accountLevels} accountUsers={this.state.accountUsers}/>;
        } else if (this.state.open === true && this.state.popupMode === AccountResource.ID.popupMode.register) {
            return <AccountRegister onClickClosePopup={this.onClickClosePopup} onClickCancle={this.onClickCancle} onClickConfirm={this.onClickConfirm} regulars={this.state.regulars} accountLevels={this.state.accountLevels} accountUsers={this.state.accountUsers} />;
        } else {
            return null;
        }
    }
}
/*
class DisplaySetting extends Component {
    constructor(props) {
        super(props);

        this.state = {
            settings: null,
            oldData: null,              // 백업 데이터 >> 취소 시에 데이터 원래대로 복구
            buildingGroupList: null,
            spreadMessages: null,
            teamTreeDatas: null,
            teams: null,
            members: null,
            disasterCategories: null,   // 재난 종류
            linkedSOPs: null,           // 재난 종류 및 빌딩, 층에 따른 SOP 연결 정보
            
        }

        this.props = props;

        this.init();
    }

    componentWillUpdate(nextProps, nextState) {
        //console.log('componentWillUpdate');

        if (this.props.settingOnOff !== nextProps.settingOnOff) {
            if (nextProps.settingOnOff === true) {
                // 창이 다시 열릴 경우
                console.log('componentWillUpdate DisplaySetting on');
                this.reloadSetting();
            }
        }
    }

    componentDidUpdate(prevProps, prevState) {
        //console.log('componentDidUpdate');
    }

    settingOff = (mode) => {
        if (mode === SettingResource.closeMode.confirm) {
            // 저장
            this.props.settingOff();

            this.backupData();
        } else if (mode === SettingResource.closeMode.cancle) {
            // 취소
            this.props.settingOff();

            // 데이터 복귀
            let oldData = this.state.oldData;

            if (oldData !== null && oldData !== undefined)
                this.setState({ settings: oldData });

            this.backupData();
        } else if (mode === SettingResource.closeMode.afterReload) {
            // 창이 닫힌 후 저장
            this.reloadSetting();
        }
    }

    async backupData() {
        let userInfo = ProjectResource.getUserInfo();
        if (userInfo === null || userInfo === undefined)
            return;

        // 초기 상황전파 메시지 가져오기
        let spreadMessages = [];
        const [spreadResult, spreadMessage] = await SettingController.requestGetSpreadMessage();
        if (spreadResult !== null && spreadResult !== undefined)
            spreadMessages = spreadResult;

        // SOP Link 정보 가져오기
        let linkedSOPs = [];
        const [linkedSOPData, linkedSOPMessage] = await SettingController.requestLinkedSOPs();
        if (linkedSOPData !== null && linkedSOPData !== undefined)
            linkedSOPs = linkedSOPData;

        // 백업용 데이터 설정 데이터 다시 불러오기 
        const [backup, temp] = await SettingController.requestSettings(userInfo.id);

        if (backup !== null && backup !== undefined)
            this.setState({ oldData: backup, spreadMessages: spreadMessages, linkedSOPs: linkedSOPs })
    }

    async reloadSetting() {
        let userInfo = ProjectResource.getUserInfo();
        if (userInfo === null || userInfo === undefined)
            return;

        // 초기 상황전파 메시지 가져오기
        let spreadMessages = [];
        const [spreadResult, spreadMessage] = await SettingController.requestGetSpreadMessage();
        if (spreadResult !== null && spreadResult !== undefined)
            spreadMessages = spreadResult;

        // SOP Link 정보 가져오기
        let linkedSOPs = [];
        const [linkedSOPData, linkedSOPMessage] = await SettingController.requestLinkedSOPs();
        if (linkedSOPData !== null && linkedSOPData !== undefined)
            linkedSOPs = linkedSOPData;

        // 설정 불러오기 
        const [result, message] = await SettingController.requestSettings(userInfo.id);
        if (result === null || result === undefined)
            return;

        this.setState({ settings: result, oldData: result, spreadMessages: spreadMessages, linkedSOPs: linkedSOPs });
    }

    async init() {
        let userInfo = await ProjectResource.initUserInfo();
        if (userInfo === null || userInfo === undefined)
            return;

        // 설정 불러오기 
        const [result, message] = await SettingController.requestSettings(userInfo.id);
        if (result === null || result === undefined)
            return;

        this.setState({ settings: result });

        // 단축키 적용, sdms 회전 대기시간 적용
        let shortcutKey = result.shortcutKey;
        let idleTime = result.idleTime;
        let moveDisplayAlarm = result.moveDisplayAlarm;
        let turnStart = result.turnStart;
        let useAlarmTurn = result.useAlarmTurn;
        SettingsStore.dispatch({ type: 'SETTINGS', idleTime, moveDisplayAlarm, turnStart, useAlarmTurn });

        // 건물 정보 가져오기
        const [buildingGroupListData, outdoorZones, errorMessage] = await SDMSController.requestBuildingGroupList();
        let buildingGroupList = [];

        if (buildingGroupListData !== null && buildingGroupListData !== undefined)
            buildingGroupList = buildingGroupListData;

        // 초기 상황전파 메시지 가져오기
        let spreadMessages = [];
        const [spreadResult, spreadMessage] = await SettingController.requestGetSpreadMessage();
        if (spreadResult !== null && spreadResult !== undefined)
            spreadMessages = spreadResult;

        // SOP Link 정보 가져오기
        let linkedSOPs = [];
        const [linkedSOPData, linkedSOPMessage] = await SettingController.requestLinkedSOPs();
        if (linkedSOPData !== null && linkedSOPData !== undefined)
            linkedSOPs = linkedSOPData;

        // 정규조직 팀, 멤버 가져오기
        const teamTreeDatas = await TeamEditController.DisplayRegular();
        const teams = await TeamEditController.GetRegular();
        const members = await TeamEditController.DisplayRegularMember();

        // SOP 재난 정보 가져오기
        const [disasterCategories, disasterCategoriesMessage] = await SopController.disasterCategories(true);

        this.setState({ buildingGroupList: buildingGroupList, spreadMessages: spreadMessages, teamTreeDatas: teamTreeDatas, teams: teams, members: members, disasterCategories: disasterCategories, linkedSOPs: linkedSOPs });


        // 백업용 데이터 설정 데이터 불러오기 
        this.backupData();
    }

    async reloadSiteID() {
        let siteID = ProjectResource.SiteID;

        if (siteID === null || siteID === undefined) {
            // 사이트 ID 요청
            const [result, message] = await SDMSController.requestGetSiteID();

            if (result !== null && result !== undefined) {
                siteID = result;
            }
        }

        return siteID;
    }

    setSdmsCommonSettings(settings) {
        const store = Store.getState();

        if (store?.sdmsCommonSettings && settings) {
            settings.useAlarmBroadcast = store.sdmsCommonSettings.UseAlarmBroadcast;
        }
    }

    render() {
        this.setSdmsCommonSettings(this.state.settings);
      
        if (this.props.settingOnOff === true) {

            return (
                <>
                    <LayoutSetting
                        settingOff={this.settingOff}
                        settings={this.state.settings}
                        buildingGroupList={this.state.buildingGroupList}
                        spreadMessages={this.state.spreadMessages}
                        teamTreeDatas={this.state.teamTreeDatas}
                        teams={this.state.teams}
                        members={this.state.members}
                        showConfirmDialog={this.showConfirmDialog}
                        settingOnOff={this.props.settingOnOff}
                        reloadSetting={this.reloadSetting}
                        disasterCategories={this.state.disasterCategories}
                        linkedSOPs={this.state.linkedSOPs}
                    />
                    
                </>);
        } else {
            return null;
        }
        
    }

}
*/