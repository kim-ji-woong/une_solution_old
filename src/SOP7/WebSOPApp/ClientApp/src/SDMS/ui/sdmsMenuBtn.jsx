import React, { Component } from 'react';
//import styles from '../css/sdms.module.css';
import styles from '../../Common/css/ui.module.css';
import styles2 from '../../Common/css/style.module.css';
import newStyles from '../../Common/css/newStyle.module.css';
import SDMSMainMenu from './sdmsMainMenu';
import SDMSResource from '../resource/id';

import $ from 'jquery';

class SDMSMenuBtn extends Component {
    static keys = [];

    constructor(props) {
        super(props);

        this.props = props;
        this.refMain = React.createRef();
    }

    componentDidMount() {
        $('#mainSB').click(function (e) {
            // 메뉴 창이 열렸을 경우 닫기 
            if ($(".subMenu." + styles.fileWrap).hasClass(styles.isShow)) {
                if (e.target.className !== styles.btnFile && e.target.className !== styles.iconFile) {
                    $(".subMenu." + styles.fileWrap).removeClass(styles.isShow);
                }
            }
        });

        // 단축키 이벤트 리스너
        window.addEventListener("keydown", (e) => this.keysPressed(e, this), false);
        window.addEventListener("keyup", this.keysReleased, false);
    }

    keysPressed(e, target) {
        // store an entry for every key pressed
        SDMSMenuBtn.keys[e.keyCode] = true;
        const commonKey = 17;   // ctrl 키
        const shiftKey = 16;

        if (SDMSMenuBtn.keys[commonKey] && SDMSMenuBtn.keys[SDMSResource.quickBtn.statusInfo]) {
            // statusInfo 퀵버튼
            target.onClickMenu(SDMSResource.ID.menu.statusInfo);

            SDMSMenuBtn.keys[SDMSResource.quickBtn.statusInfo] = false;
            // prevent default browser behavior
            e.preventDefault();
        } else if (SDMSMenuBtn.keys[commonKey] && SDMSMenuBtn.keys[SDMSResource.quickBtn.cctv]) {
            // cctv 퀵버튼
            target.onClickMenu(SDMSResource.ID.menu.allCCTV);

            SDMSMenuBtn.keys[SDMSResource.quickBtn.cctv] = false;
            // prevent default browser behavior
            e.preventDefault();
        } else if (SDMSMenuBtn.keys[commonKey] && SDMSMenuBtn.keys[SDMSResource.quickBtn.dashboard]) {
            // dashboard 퀵버튼
            target.onClickMenu(SDMSResource.ID.menu.dashboard);

            SDMSMenuBtn.keys[SDMSResource.quickBtn.dashboard] = false;
            // prevent default browser behavior
            e.preventDefault();
        } else if (SDMSMenuBtn.keys[commonKey] && SDMSMenuBtn.keys[SDMSResource.quickBtn.eventInfo]) {
            // eventInfo 퀵버튼
            target.onClickMenu(SDMSResource.ID.menu.eventInfo);

            SDMSMenuBtn.keys[SDMSResource.quickBtn.eventInfo] = false;
            // prevent default browser behavior
            e.preventDefault();
        } else if (SDMSMenuBtn.keys[commonKey] && SDMSMenuBtn.keys[SDMSResource.quickBtn.miniMap]) {
            // miniMap 퀵버튼
            target.onClickMenu(SDMSResource.ID.menu.miniMap);

            SDMSMenuBtn.keys[SDMSResource.quickBtn.miniMap] = false;
            // prevent default browser behavior
            e.preventDefault();
        } else if (SDMSMenuBtn.keys[commonKey] && SDMSMenuBtn.keys[SDMSResource.quickBtn.editMode]) {
            // editMode 퀵버튼
            target.onClickMenu(SDMSResource.ID.menu.editMode);

            SDMSMenuBtn.keys[SDMSResource.quickBtn.editMode] = false;
            // prevent default browser behavior
            e.preventDefault();
        } else if (SDMSMenuBtn.keys[commonKey] && SDMSMenuBtn.keys[SDMSResource.quickBtn.manualReport]) {
            // manualReport 퀵버튼
            target.onClickMenu(SDMSResource.ID.menu.manualReport);

            SDMSMenuBtn.keys[SDMSResource.quickBtn.manualReport] = false;
            // prevent default browser behavior
            e.preventDefault();
        } else if (SDMSMenuBtn.keys[commonKey] && SDMSMenuBtn.keys[SDMSResource.quickBtn.weatherInfo]) {
            // weatherInfo 퀵버튼
            target.onClickMenu(SDMSResource.ID.menu.weatherInfo);

            SDMSMenuBtn.keys[SDMSResource.quickBtn.weatherInfo] = false;
            // prevent default browser behavior
            e.preventDefault();
        }
        // .TODO: 고도화 내용으로 인한 주석처리 
        /*
        else if (SDMSMenuBtn.keys[shiftKey] && SDMSMenuBtn.keys[SDMSResource.quickBtn.cctvAlarm1]) {
            // 알람 CCTV 뷰어창1 퀵버튼
            target.onClickMenu(SDMSResource.ID.menu.alarmCCTV1);

            SDMSMenuBtn.keys[SDMSResource.quickBtn.cctvAlarm1] = false;
            // prevent default browser behavior
            e.preventDefault();
        } else if (SDMSMenuBtn.keys[shiftKey] && SDMSMenuBtn.keys[SDMSResource.quickBtn.cctvAlarm2]) {
            // 알람 CCTV 뷰어창2 퀵버튼
            target.onClickMenu(SDMSResource.ID.menu.alarmCCTV2);

            SDMSMenuBtn.keys[SDMSResource.quickBtn.cctvAlarm2] = false;
            // prevent default browser behavior
            e.preventDefault();
        } else if (SDMSMenuBtn.keys[shiftKey] && SDMSMenuBtn.keys[SDMSResource.quickBtn.cctvAlarm3]) {
            // 알람 CCTV 뷰어창3 퀵버튼
            target.onClickMenu(SDMSResource.ID.menu.alarmCCTV3);

            SDMSMenuBtn.keys[SDMSResource.quickBtn.cctvAlarm3] = false;
            // prevent default browser behavior
            e.preventDefault();
        }
        */
        
    }

    keysReleased(e) {
        // mark keys that were released
        SDMSMenuBtn.keys[e.keyCode] = false;
    }

    onClickMenu(menu) {
        const parent = this.refMain.current.parentNode;

        if (parent) {
            parent.classList.remove(styles.isShow);
        }

        if (this.props.menuEvent.handler) {
            this.props.menuEvent.handler(menu, this.getParameter(menu));
        }
    }

    getParameter(menu) {
        return "";

        
    }

    render() {
        return (
            <div className={newStyles.rqQck + " rqBtn"}>
                <button className="rqQckBtn">메뉴열기</button>
                <ul ref={this.refMain}>
                    <li><a onClick={() => this.onClickMenu(SDMSResource.ID.menu.statusInfo)}>현황정보</a></li>
                    <li><a onClick={() => this.onClickMenu(SDMSResource.ID.menu.allCCTV)}>CCTV</a></li>
                    <li><a onClick={() => this.onClickMenu(SDMSResource.ID.menu.dashboard)}>대시보드</a></li>
                    <li><a onClick={() => this.onClickMenu(SDMSResource.ID.menu.eventInfo)}>이벤트 정보</a></li>
                    <li><a onClick={() => this.onClickMenu(SDMSResource.ID.menu.miniMap)}>미니맵</a></li>
                    <li><a onClick={() => this.onClickMenu(SDMSResource.ID.menu.editMode)}>편집모드</a></li>
                    <li><a onClick={() => this.onClickMenu(SDMSResource.ID.menu.manualReport)}>수동신고</a></li>
                    <li><a onClick={() => this.onClickMenu(SDMSResource.ID.menu.weatherInfo)}>기상정보</a></li>
                </ul>
            </div>
        );
    }
}

export default SDMSMenuBtn;