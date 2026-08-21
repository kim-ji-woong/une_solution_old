import React, { Component } from 'react';
import { BrowserRouter as Route, Link } from 'react-router-dom';
import { withRouter } from 'react-router-dom';
import $ from 'jquery';

import styles from '../SDMS/css/sdms.module.css';
import title from './css/titleBar.module.css';
import drawerIc from './image/icon/drawer_ic.png';
import partlySunnyIc from './image/icon/partly_sunny_ic.png';
import settingIc from './image/icon/setting_ic.png';
import userIc from './image/icon/user_ic.png';
import nstLogo from '../Account/image/icon/enhancement_CI.png';
import imgClose from '../SDMS/image/common_Icon/popup_close.png';

import SDMSResource from "../SDMS/resource/id";
import ChangePassword from '../Account/ui/popups/changePassword';
import UserManagement from '../Account/ui/popups/userManagement';
import Setting from './popups/setting';

import ReactDom from 'react-dom';
import SDMSMainMenu from '../SDMS/data/sdmsMainMenu';

import WeatherInfo from './weatherInfo';
import { SDMSController } from '../SDMS/services/sdmsController';
import { AccountController } from '../Account/services/accountController';
import RootResource from './resource/id';
import AccountResource from '../Account/resource/id';
import AccountStore from '../Account/accountStore';

import ConfirmDialog from '../Common/ui/confirmDialog';

import SessionString from '../Common/js/sessionString';
import ProjectResource from './resource/id';

class TitleBar extends Component {
    static pathSDMS = '/sdms';

    static NavMenu = {
        StatusInfo: "현황정보",
        CCTV: "CCTV",
        WorkerMonitor: "작업자 모니터링",
        Warning: "경고/알람",
        Spread: "상황전파",
        History: "이력관리"
    };

    constructor(props) {
        super(props);

        this.state = {
            content: SDMSResource.ID.menu.statusInfo,
            isOpenPopup: false,
            visiblePopups: {
                changePassword: false,
                userManagement: false,
                setting: false,
            },
            confirmMessage: {
                visible: false,
                title: "",
                messages: [""],
                buttons: ["확인"],
                onClose: this.onCloseConfirmDialog,
                onClickButton: null
            },
            loading: true
        }

        this.openPopup = this.openPopup.bind(this);
        this.closePopup = this.closePopup.bind(this);

        this.props = props;

        this.refNavPanel = React.createRef();

        AccountStore.subscribe(function () {
            let data = AccountStore.getState();

            if (data.actionType === 'LOGIN_STATE') {
                this.checkLoginState(data);
            }
        }.bind(this));
    }

    openPopup() {
        this.setState({
            isOpenPopup:true,
        })
    }

    closePopup() {
        this.setState({
            isOpenPopup:false,
        })
    }


    //const StatusInfoo = ({ children }) => {
    //    const el = document.getElementById('statusInfo');
    //    return ReactDOM.createPortal(children, el);
    //};

    //export default StatusInfoo;

    componentDidMount() {
        // 상단 Util Button
        //$('.' + style.navWrap).on('click', 'button', function () {
        //    $(this).closest('.subMenu').toggleClass(title.isShow).siblings().removeClass(title.isShow);
        //});

        this.init();
    }

    async init() {
        const isModelViewer = await SDMSController.isModelViewer();
        ProjectResource.isModelViewer = isModelViewer;

        if (ProjectResource.isModelViewer === false) {
            // 센서 히스토리 감시 타이머 시작
            SDMSController.StartWatchTimer();

            // 로그인 세션 감시 타이머 
            //AccountController.StartWatchTimer();
        }

        $(function () {
            // 다른 곳 클릭했을때 이벤트 발생
            $('#main').click(function (e) {
                // 메뉴 버튼을 클릭 여부 확인 >> 아니라면 열려있는 메뉴는 닫힘
                const target = e.target;
                if ($(target).hasClass(title.tabBtn) || $(target).hasClass(title.arrowBtn) || $(target).hasClass(title.arrowBtn2) || $(target).hasClass("tabMenu"))
                    return;

                if ($('.' + title.navPanel).css('display') === "block") {
                    $('.' + title.navPanel).slideUp();
                } else if ($('.' + title.tabCamera).css('display') === "block") {
                    $('.' + title.tabCamera).slideUp();
                } else if ($('.' + title.tabWeather2).css('display') === "block") {
                    $('.' + title.tabWeather).slideUp();
                    $('.' + title.tabWeather2).slideUp();
                } else if ($('.' + title.tabWeather).css('display') === "block") {
                    $('.' + title.tabWeather).slideUp();
                } else if ($('.' + title.tabUser).css('display') === "block") {
                    $('.' + title.tabUser).slideUp();
                }
            });

            /*서랍*/
            $('.' + title.tabBtnDrawer).click(function () {
                $('.' + title.tabCamera).slideUp();
                $('.' + title.tabWeather).slideUp();
                $('.' + title.tabWeather2).slideUp();
                $('.' + title.tabUser).slideUp();

                $('.' + title.navPanel).slideToggle();
                //$('.' + title.navPanel).stop().slideUp(1000);
            });

            /*카메라*/
            $('.' + title.tabBtnCamera).click(function () {
                $('.' + title.navPanel).slideUp();
                $('.' + title.tabWeather).slideUp();
                $('.' + title.tabWeather2).slideUp();
                $('.' + title.tabUser).slideUp();

                $('.' + title.tabCamera).slideToggle();
            });

            /*날씨*/
            $('.' + title.tabBtnWeather).click(function () {
                $('.' + title.navPanel).slideUp();
                $('.' + title.tabCamera).slideUp();
                $('.' + title.tabWeather2).slideUp();
                $('.' + title.tabUser).slideUp();

                $('.' + title.tabWeather).slideToggle();    /* WeatherInfo 오픈 */
                $('.' + title.arrowBtn).show();
            });

            /*사용자*/
            $('.' + title.tabBtnUser).click(function () {
                $('.' + title.navPanel).slideUp();
                $('.' + title.tabCamera).slideUp();
                $('.' + title.tabWeather).slideUp();
                $('.' + title.tabWeather2).slideUp();

                $('.' + title.tabUser).slideToggle();
            });


        });

        this.setState({ loading: false });
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
            window.localStorage.removeItem(SessionString.Key.account);
        } else if (data.loginState === AccountResource.loginState.disconnected) {
            // 네트워크 연결 끊김 시
            //alert(data.message);
            this.showConfirmDialog("오류", data.message, ["확인"], this.onClickFalseConfirm, this.onClickFalseConfirm);
        } else if (data.loginState === AccountResource.loginState.false) {
            // 세션 조회 실패 시
            // 로그인 페이지로 이동
            //window.location.href = RootResource.path.root;

            //alert(data.message);

            this.showConfirmDialog("로그인", data.message, ["확인"], this.onClickFalseConfirm, this.onClickFalseConfirm);
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
        window.localStorage.removeItem(SessionString.Key.account);
    }

    showConfirmDialog = (title, messages, buttons, onClickButton, onClickClose) => {
        const confirmMessage = { ...this.state.confirmMessage };
        confirmMessage.visible = true;
        confirmMessage.title = title;
        confirmMessage.buttons = buttons;
        confirmMessage.onClickButton = onClickButton;

        if (onClickClose !== null)
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

    changeContent = (content) => {
        this.setState({ content });
    }


    /*getMenuUI() {
        let menu = [];
        if (this.state.content === SDMSResource.ID.menu.statusInfo) {
            menu.push(<StatusInfo changeContent={this.changeContent} />);
        }
        else if (this.state.content === SDMSResource.ID.menu.cctvInfo) {
            menu.push(<CCTVInfo changeContent={this.changeContent} />);
        }
        else if (this.state.content === SDMSResource.ID.menu.workerInfo) {
            menu.push(<WorkerInfo changeContent={this.changeContent} />)
        }
        else if (this.state.content === SDMSResource.ID.menu.warningAlarmInfo) {
            menu.push(<WarningAlarmInfo changeContent={this.changeContent} />);
        }
        else if (this.state.content === SDMSResource.ID.menu.spreadInfo) {
            menu.push(<SpreadInfo changeContent={this.changeContent} />);
        }
        return menu;
    }*/

    setVisiblePopup = (popup, visible) => {
        const visiblePopups = { ...this.state.visiblePopups };
        visiblePopups[popup] = visible;
        this.setState({ visiblePopups });
    }

    onClickNavPanel(menu) {
        if (menu === TitleBar.NavMenu.StatusInfo) {
            this.props.menuEvent.handler(SDMSMainMenu.Menu_ToggleStatusInfo, null);
        }
        else if (menu === TitleBar.NavMenu.CCTV) {
            this.props.menuEvent.handler(SDMSMainMenu.Menu_ToggleCCTV, null);
        }
        else if (menu === TitleBar.NavMenu.WorkerMonitor) {
            this.props.menuEvent.handler(SDMSMainMenu.Menu_ToggleWorkerMonitor, null);
        }
        else if (menu === TitleBar.NavMenu.Warning) {
            this.props.menuEvent.handler(SDMSMainMenu.Menu_ToggleWarning, null);
        }
        else if (menu === TitleBar.NavMenu.Spread) {
            this.props.menuEvent.handler(SDMSMainMenu.Menu_ToggleSpreadInfo, null);
        }
        else if (menu === TitleBar.NavMenu.History) {
            this.props.menuEvent.handler(SDMSMainMenu.Menu_ToggleHistory, null);
        }

        $(this.refNavPanel.current).slideToggle();
    }

    onClickLogo() {
        if (this.props.menuEvent && this.props.menuEvent.onClickLogo) {
            this.props.menuEvent.onClickLogo();
        }
    }

    onClickLogout() {
        // 계정 리덕스에 상태 업데이트
        AccountStore.dispatch({ type: 'LOGIN_STATE', loginState: AccountResource.loginState.logout, message: "로그아웃 하였습니다." });
    }

    getUserInfo() {
        let userInfo = "";
        let userName = "";
        let userLevel = "";

        if (window.localStorage.getItem(SessionString.Key.account) != null) {
            userInfo = JSON.parse(window.localStorage.getItem(SessionString.Key.account));
            userName = userInfo.nickName;
            userLevel = userInfo.level;
        }
        //else {
        //    // 로그인이 안되었다면 로그인 페이지로 이동
        //    this.props.history.push('/');
        //}

        return [userName, userLevel];
    }

    onClickViewMode(mode) {
        if (this.props.menuEvent?.handler) {
            this.props.menuEvent.handler(mode);
        }
    }

    onClickUserMgr = () => {
        this.setVisiblePopup("userManagement", true);
    }

    onClickUserPass = () => {
        this.setVisiblePopup("changePassword", true);
    }

    onClickSetting = () => {
        this.setVisiblePopup("setting", true);
    }

    render() {
        if (this.state.loading || ProjectResource.isModelViewer) {
            return <></>;
        }
        //const menuUI = this.getMenuUI();

        const [userName, userLevel] = this.getUserInfo();

        return (
            <>
                <div className={title.navWrap}>
                    <div className={title.navHeader}>
                        {/*<h1 className={title.navHeaderTitle}>KIST<span>NST</span></h1>*/}
                        <h1 className={title.navHeaderTitle}><img src={nstLogo} onClick={() => this.onClickLogo()} /></h1>
                        <ul className={title.tabLink}>
                            <li className={title.link}><i className={title.tabBtn + " " + title.tabBtnDrawer}></i></li>  {/*list 서랍*/}
                            <li className={title.link}><i className={title.tabBtn + " " + title.tabBtnCamera}></i></li> {/*카메라*/}
                            <li className={title.link}><i className={title.tabBtn + " " + title.tabBtnWeather}></i></li> {/*날씨*/}
                            <li className={title.link}><i className={title.tabBtn + " " + title.tabBtnUser}></i></li> {/*사용자 설정*/}
                            <li className={title.link}><i className={title.tabBtn + " " + title.tabBtnSet} onClick={this.onClickSetting}></i></li> {/*설정*/}
                        </ul>
                    </div>

                    <div ref={this.refNavPanel} className={title.navPanel}>
                        <div className={title.tabDrawer}>
                            <ul>
                                <li className="tabMenu" onClick={() => this.onClickNavPanel(TitleBar.NavMenu.StatusInfo)}><div className="tabMenu"><i className="tabMenu"></i>{TitleBar.NavMenu.StatusInfo}</div></li>
                                <li className="tabMenu" onClick={() => this.onClickNavPanel(TitleBar.NavMenu.CCTV)}><div className="tabMenu"><i className="tabMenu"></i>{TitleBar.NavMenu.CCTV}</div></li>
                                {/*<li className="tabMenu" onClick={() => this.onClickNavPanel(TitleBar.NavMenu.WorkerMonitor)}><div className="tabMenu"><i></i>{TitleBar.NavMenu.WorkerMonitor}</div></li>*/}
                                <li className="tabMenu" onClick={() => this.onClickNavPanel(TitleBar.NavMenu.Warning)}><div className="tabMenu"><i className="tabMenu"></i><Link to='' />{TitleBar.NavMenu.Warning}</div></li>
                                <li className="tabMenu" onClick={() => this.onClickNavPanel(TitleBar.NavMenu.Spread)}><div className="tabMenu"><i className="tabMenu"></i>{TitleBar.NavMenu.Spread}</div></li>
                                <li className="tabMenu" onClick={() => this.onClickNavPanel(TitleBar.NavMenu.History)}><div className="tabMenu"><i className="tabMenu"></i>{TitleBar.NavMenu.History}</div></li>
                            </ul>
                        </div>
                    </div>


                    {/*카메라뷰*/}
                    <figure className={title.tabCamera}>
                        <div className={title.tabCameraContent}>
                            <div className={title.basicView + " tabMenu"}><span onClick={() => this.onClickViewMode(SDMSMainMenu.Menu_ShowBasicViewMode)}>기본뷰</span></div>
                            {/*<div className={title.seeView}><span>투시도뷰</span></div>*/}
                            <div className={title.systemView + " tabMenu"}><span onClick={() => this.onClickViewMode(SDMSMainMenu.Menu_ShowPipeLineViewMode)}>계통도뷰</span></div>
                        </div>
                    </figure> 


                    {/*기상정보*/}
                    <WeatherInfo />



                    {/*사용자정보*/}
                    <figure className={title.panel + " " + title.tabUser}>
                        <div className={title.tabUserContainer + " " + title.user}>
                            <span className={title.tabUserImg}></span>
                            <ul className={title.tabUserText}>
                                <li className={title.name}>{userLevel}</li><span className={title.logoutIcon} onClick={this.onClickLogout}></span>
                                <li className={title.code}>(주)KIST 공사 관리부</li>
                                <li className={title.company}>사번 12504026</li>
                            </ul>
                        </div>
                        <div className={title.tabUserBtn}>
                            <div className={title.btnGroup}>
                                <button type="button" className={title.btn} onClick={this.onClickUserMgr}>사용자 관리</button>
                                <button type="button" className={title.btn} onClick={this.onClickUserPass}>비밀번호 변경</button>
                            </div>
                        </div>
                    </figure>

                </div> 

                {
                    this.state.visiblePopups.changePassword &&
                    <ChangePassword
                        popupType="changePassword"
                        setVisiblePopup={this.setVisiblePopup}/>
                }

                {
                    this.state.visiblePopups.userManagement &&
                    <UserManagement
                        popupType="userManagement"
                        setVisiblePopup={this.setVisiblePopup} />
                }

                {
                    this.state.visiblePopups.setting &&
                    <Setting
                        popupType="setting"
                        setVisiblePopup={this.setVisiblePopup} />
                }

                {
                    /* alert창 대신 사용 */
                    this.state.confirmMessage.visible &&
                    <ConfirmDialog title={this.state.confirmMessage.title} messages={this.state.confirmMessage.messages} buttons={this.state.confirmMessage.buttons} onClose={this.state.confirmMessage.onClose} onClickButton={this.state.confirmMessage.onClickButton} />
                }
            </>
        );

    }

} export default withRouter(TitleBar);