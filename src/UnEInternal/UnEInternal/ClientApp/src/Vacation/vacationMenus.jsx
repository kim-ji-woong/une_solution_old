import React, { Component } from 'react';
import styles from './css/vacation.module.css';
import { FaHome } from "react-icons/fa";
import $ from 'jquery';

import '../Root/css/NavMenu.css';
import { useEffect } from 'react'; 
import { Link } from 'react-router-dom';
import { Layout } from '../Root/layout';
//import { VacationBody } from './VacationBody.jsx';


export class VacationMenus extends Component {

    constructor(props) {
        super(props);

        this.props = props;

        this.state = {
            disUI: this.displayUI(),
        }
    }

    state = { width: 0, height: 0 };

    static displayName = VacationMenus.name;

    static MyVacations = "myVacations";
    static RequestHistory = "requestHistory";
    static Request = "request";
    static WaitResponse = "waitResponse";
    static ResponseHistory = "responseHistory";
    static MemberHistory = "memberHistory";
    static TeamHistory = "teamHistory";
    static RequestSpecialHistory = "requestSpecialHistory";
    static CancelHistory = "cancelHistory";


    resizeUI() {
        this.setState({ disUI: this.displayUI() });
    }

    componentDidMount() {
        window.addEventListener('resize', () => this.resizeUI());
    }

    onClickMenu = (menu) => {
        //this.setState({ selectedMenu: menu });
        this.props.onSelectMenu(menu);
    }

    getMenuItemClassName(isActive) {
        if (isActive) {
            return styles.menuItem + " " + styles.active;
        }

        return styles.menuItem;
    }

    componentDidMount() {
        $('.' + styles.menuicon).click(function () {
         $('.' + styles.menuItems).toggleClass("menuOn");
        });


        $('.hamburger').on('click', function () {
            $('#sidebar').addClass('active');
            $('.overlay').fadeIn();
            //$('#sidebar').hide();
        });

        $('.overlay').on('click', function () {
            $('#sidebar').removeClass('active');
            $('.overlay').fadeOut();
        });


        $('#sidebar').on('click', function () {
            $('#sidebar').removeClass('active');
            $('.overlay').fadeOut();
        });

    }


    processLogin = () => {
        if (this.props.loginUser) {
            this.props.onLogout(this.props.loginUser);
        }
        else {
            this.props.onLogin();
        }
    }

    displayUI = () => {
        let displayUI = [];
        let widthSize = window.outerWidth;


        //$(window).resize(function () {

        const loginMenu = this.props.loginUser ? this.props.loginUser.userID + " 로그아웃" : "로그인";

        const mvClass = this.getMenuItemClassName(this.props.selectedMenu === VacationMenus.MyVacations);
        const rhClass = this.getMenuItemClassName(this.props.selectedMenu === VacationMenus.RequestHistory);
        const rClass = this.getMenuItemClassName(this.props.selectedMenu === VacationMenus.Request);
        const chClass = this.getMenuItemClassName(this.props.selectedMenu === VacationMenus.CancelHistory);
        const wrClass = this.getMenuItemClassName(this.props.selectedMenu === VacationMenus.WaitResponse);
        const rehClass = this.getMenuItemClassName(this.props.selectedMenu === VacationMenus.ResponseHistory);
        const mhClass = this.getMenuItemClassName(this.props.selectedMenu === VacationMenus.MemberHistory);
        const thClass = this.getMenuItemClassName(this.props.selectedMenu === VacationMenus.TeamHistory);
        const rshClass = this.getMenuItemClassName(this.props.selectedMenu === VacationMenus.RequestSpecialHistory);


        
        //$(window).resize(function () {
            if (widthSize < 768) { //모바일
                displayUI.push(
                    <>
                        <div id="wrap">
                            <nav id="sidebar">
                                <span className={styles.manageTitle}><Link to="/vacation">휴가관리</Link></span>
                                <span className={styles.teamManageTitle}>
                                    {
                                        this.props.loginUser?.isAdmin && (
                                            <span>
                                                <Link to="/teams">조직관리</Link>
                                            </span>
                                        )

                                    }
                                </span>
                                <span className={styles.sideTitle}>휴가관리</span>

                                <li className={mvClass} id={styles.test1} onClick={() => this.onClickMenu(VacationMenus.MyVacations)}>휴가현황</li>
                                {

                                }
                                <li className={rClass} id={styles.test2} onClick={() => this.onClickMenu(VacationMenus.Request)}>휴가요청</li>
                                <li className={chClass} id={styles.test3} onClick={() => this.onClickMenu(VacationMenus.CancelHistory)}>휴가취소</li>
                                {
                                    this.props.loginUser?.isTeamLeader && this.props.managerRequest && (
                                        <li className={wrClass} id={styles.test4} onClick={() => this.onClickMenu(VacationMenus.WaitResponse)}>결재대기 리스트</li>
                                    )
                                }
                                {
                                    this.props.loginUser?.isTeamLeader && this.props.managerRequest && (
                                        <li className={rehClass} id={styles.test5} onClick={() => this.onClickMenu(VacationMenus.ResponseHistory)}>결재처리 현황</li>
                                    )
                                }
                                {
                                    (
                                        <li className={mhClass} id={styles.test6} onClick={() => this.onClickMenu(VacationMenus.MemberHistory)}>직원휴가조회</li>
                                    )
                                }
                                {
                                    (
                                        <li className={thClass} id={styles.test7} onClick={() => this.onClickMenu(VacationMenus.TeamHistory)}>팀별휴가조회</li>
                                    )
                                }
                                {
                                    (this.props.loginUser?.isTeamLeader || this.props.loginUser?.isAdmin || this.props.loginUser?.isTopManager) && (
                                        <li className={rshClass} id={styles.test8} onClick={() => this.onClickMenu(VacationMenus.RequestSpecialHistory)}>특별휴가요청</li>
                                    )
                                }
                            </nav>

                            <header className="hamburgerHeader" onClick={this.processLogin}><span className="hamburgerlogin">{loginMenu}</span></header>
                            <div className="hamburger">
                                <input type="checkbox" />
                                <div className="hamburgerlines">
                                    <span className="lines line1">
                                    </span>
                                    <span className="lines line2">
                                    </span>
                                    <span className="lines line3">
                                    </span>
                                </div>
                            </div>
                            <div className="overlay"></div>
                        </div>
                    </>
                );

            } else if (640 <= widthSize && widthSize <= 959) { //가로 모바일
                displayUI.push(
                    <>
                        <div id="wrap">
                            <nav id="sidebar">
                                <span className={styles.manageTitle}><Link to="/vacation">휴가관리</Link></span>
                                <span className={styles.teamManageTitle}>
                                    {
                                        this.props.loginUser?.isAdmin && (
                                            <span>
                                                <Link to="/teams">조직관리</Link>
                                            </span>
                                        )

                                    }
                                </span>
                                <span className={styles.sideTitle}>휴가관리</span>

                                <li className={mvClass} id={styles.test1} onClick={() => this.onClickMenu(VacationMenus.MyVacations)}>휴가현황</li>
                                {

                                }
                                <li className={rClass} id={styles.test2} onClick={() => this.onClickMenu(VacationMenus.Request)}>휴가요청</li>
                                <li className={chClass} id={styles.test3} onClick={() => this.onClickMenu(VacationMenus.CancelHistory)}>휴가취소</li>
                                {
                                    this.props.loginUser?.isTeamLeader && this.props.managerRequest && (
                                        <li className={wrClass} id={styles.test4} onClick={() => this.onClickMenu(VacationMenus.WaitResponse)}>결재대기 리스트</li>
                                    )
                                }
                                {
                                    this.props.loginUser?.isTeamLeader && this.props.managerRequest && (
                                        <li className={rehClass} id={styles.test5} onClick={() => this.onClickMenu(VacationMenus.ResponseHistory)}>결재처리 현황</li>
                                    )
                                }
                                {
                                    (
                                        <li className={mhClass} id={styles.test6} onClick={() => this.onClickMenu(VacationMenus.MemberHistory)}>직원휴가조회</li>
                                    )
                                }
                                {
                                    (
                                        <li className={thClass} id={styles.test7} onClick={() => this.onClickMenu(VacationMenus.TeamHistory)}>팀별휴가조회</li>
                                    )
                                }
                                {
                                    (this.props.loginUser?.isTeamLeader || this.props.loginUser?.isAdmin || this.props.loginUser?.isTopManager) && (
                                        <li className={rshClass} id={styles.test8} onClick={() => this.onClickMenu(VacationMenus.RequestSpecialHistory)}>특별휴가요청</li>
                                    )
                                }
                            </nav>

                            <header className="hamburgerHeader" onClick={this.processLogin}><span className="hamburgerlogin">{loginMenu}</span></header>
                            <div className="hamburger">
                                <input type="checkbox" />
                                <div className="hamburgerlines">
                                    <span className="lines line1">
                                    </span>
                                    <span className="lines line2">
                                    </span>
                                    <span className="lines line3">
                                    </span>
                                </div>
                            </div>
                            <div className="overlay"></div>
                        </div>
                    </>
                );

            } else if (768 <= widthSize && widthSize <= 1024) { //태블릿
                displayUI.push(
                    <>
                        <div className={styles.navBarMenu}>
                            <div className={styles.menuicon}><FaHome size="40" /><span></span></div>
                            <div className={styles.menuItems}>
                                <div className={mvClass} id={styles.test1} onClick={() => this.onClickMenu(VacationMenus.MyVacations)}>휴가현황</div>
                                {

                                }
                                <div className={rClass} id={styles.test2} onClick={() => this.onClickMenu(VacationMenus.Request)}>휴가요청</div>
                                <div className={chClass} id={styles.test3} onClick={() => this.onClickMenu(VacationMenus.CancelHistory)}>휴가취소</div>
                                {
                                    this.props.loginUser?.isTeamLeader && this.props.managerRequest && (
                                        <div className={wrClass} id={styles.test4} onClick={() => this.onClickMenu(VacationMenus.WaitResponse)}>결재대기 리스트</div>
                                    )
                                }
                                {
                                    this.props.loginUser?.isTeamLeader && this.props.managerRequest && (
                                        <div className={rehClass} id={styles.test5} onClick={() => this.onClickMenu(VacationMenus.ResponseHistory)}>결재처리 현황</div>
                                    )
                                }
                                {
                                    (
                                        <div className={mhClass} id={styles.test6} onClick={() => this.onClickMenu(VacationMenus.MemberHistory)}>직원 휴가조회</div>
                                    )
                                }
                                {
                                    (
                                        <div className={thClass} id={styles.test7} onClick={() => this.onClickMenu(VacationMenus.TeamHistory)}>팀별 휴가조회</div>
                                    )
                                }
                                {
                                    (this.props.loginUser?.isTeamLeader || this.props.loginUser?.isAdmin || this.props.loginUser?.isTopManager) && (
                                        <div className={rshClass} id={styles.test8} onClick={() => this.onClickMenu(VacationMenus.RequestSpecialHistory)}>특별휴가요청</div>
                                    )
                                }
                            </div>
                        </div>
                    </>
                );

            } else if (960 <= widthSize && widthSize <= 1280) { //가로 태블릿
                displayUI.push(
                    <>
                        <div className={styles.navBarMenu}>
                            <div className={styles.menuicon}><FaHome size="40" /><span></span></div>
                            <div className={styles.menuItems}>
                                <div className={mvClass} id={styles.test1} onClick={() => this.onClickMenu(VacationMenus.MyVacations)}>휴가현황</div>
                                {

                                }
                                <div className={rClass} id={styles.test2} onClick={() => this.onClickMenu(VacationMenus.Request)}>휴가요청</div>
                                <div className={chClass} id={styles.test3} onClick={() => this.onClickMenu(VacationMenus.CancelHistory)}>휴가취소</div>
                                {
                                    this.props.loginUser?.isTeamLeader && this.props.managerRequest && (
                                        <div className={wrClass} id={styles.test4} onClick={() => this.onClickMenu(VacationMenus.WaitResponse)}>결재대기 리스트</div>
                                    )
                                }
                                {
                                    this.props.loginUser?.isTeamLeader && this.props.managerRequest && (
                                        <div className={rehClass} id={styles.test5} onClick={() => this.onClickMenu(VacationMenus.ResponseHistory)}>결재처리 현황</div>
                                    )
                                }
                                {
                                    (
                                        <div className={mhClass} id={styles.test6} onClick={() => this.onClickMenu(VacationMenus.MemberHistory)}>직원 휴가조회</div>
                                    )
                                }
                                {
                                    (
                                        <div className={thClass} id={styles.test7} onClick={() => this.onClickMenu(VacationMenus.TeamHistory)}>팀별 휴가조회</div>
                                    )
                                }
                                {
                                    (this.props.loginUser?.isTeamLeader || this.props.loginUser?.isAdmin || this.props.loginUser?.isTopManager) && (
                                        <div className={rshClass} id={styles.test8} onClick={() => this.onClickMenu(VacationMenus.RequestSpecialHistory)}>특별휴가요청</div>
                                    )
                                }
                            </div>
                        </div>
                    </>
                );

            } else if (widthSize >= 1025) {
                displayUI.push(
                    <>
                        <div className={styles.navBarMenu}>
                            <div className={styles.menuicon}><FaHome size="40" /><span></span></div>
                            <div className={styles.menuItems}>
                                <div className={mvClass} id={styles.test1} onClick={() => this.onClickMenu(VacationMenus.MyVacations)}>휴가현황</div>
                                {

                                }
                                <div className={rClass} id={styles.test2} onClick={() => this.onClickMenu(VacationMenus.Request)}>휴가요청</div>
                                <div className={chClass} id={styles.test3} onClick={() => this.onClickMenu(VacationMenus.CancelHistory)}>휴가취소</div>
                                {
                                    this.props.loginUser?.isTeamLeader && this.props.managerRequest && (
                                        <div className={wrClass} id={styles.test4} onClick={() => this.onClickMenu(VacationMenus.WaitResponse)}>결재대기 리스트</div>
                                    )
                                }
                                {
                                    this.props.loginUser?.isTeamLeader && this.props.managerRequest && (
                                        <div className={rehClass} id={styles.test5} onClick={() => this.onClickMenu(VacationMenus.ResponseHistory)}>결재처리 현황</div>
                                    )
                                }
                                {
                                    (
                                        <div className={mhClass} id={styles.test6} onClick={() => this.onClickMenu(VacationMenus.MemberHistory)}>직원 휴가조회</div>
                                    )
                                }
                                {
                                    (
                                        <div className={thClass} id={styles.test7} onClick={() => this.onClickMenu(VacationMenus.TeamHistory)}>팀별 휴가조회</div>
                                    )
                                }
                                {
                                    (this.props.loginUser?.isTeamLeader || this.props.loginUser?.isAdmin || this.props.loginUser?.isTopManager) && (
                                        <div className={rshClass} id={styles.test8} onClick={() => this.onClickMenu(VacationMenus.RequestSpecialHistory)}>특별휴가요청</div>
                                    )
                                }
                            </div>
                        </div>
                    </>
                );
            }
        
        return displayUI;
    }

    
    render() {
        setTimeout(() => { this.resizeUI() }, 500);
        let displayUI = this.state.disUI;
        //let displayUI = this.displayUI();

        return (
            <>
                {displayUI}
            </>

        );
     }
}