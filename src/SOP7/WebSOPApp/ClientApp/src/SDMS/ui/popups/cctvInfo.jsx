
import { ui } from 'jquery';
import React, { Component } from 'react';
import $ from 'jquery';
import content from '../../../Common/css/content.module.css';
import imgClose from '../../../Common/image/icon/close_x.png';
import imgReset from '../../../Common/image/icon/reset_ico.png';
import SDMS from '../sdms';
import SettingsStore from '../../../Settings/settingsStore';

import sdms from '../../css/sdms.module.css';
import Contents3D from '../3D/contents3D';

import SDMSResource from '../../resource/id';

class CCTVInfo extends Component {
    static Mode_Select_Sensor = 1;
    static Mode_Select_CCTV = 2;
    static Mode_Delete_CCTV = 3;

    constructor(props) {
        super(props);

        this.state = {
            cctvList: "",
            cctvWidth: 0,           // cctv 화면 사이즈(가로)
            cctvHeight: 0,          // cctv 화면 사이즈(세로)
            cctvCountMax: 4,        // cctv 최대 갯수
            streamServerURL: "",
            /* 비율 기준 (w:1903 * h:969) */
            //popupMinWidth: 18.91,     // 전체화면 대비 팝업 최소 너비 비율 (기본 너비px: 360)
            //popupMinHeight: 39.21,    // 전체화면 대비 팝업 최소 높이 비율 (기본 너비px: 380)
            fullScreenIndex: -1,
            popupMinWidth: 360,
            popupMinHeight: 380
        };

        this.props = props;

        if (this.props.cctvList !== null && this.props.cctvList !== "" && this.props.cctvList !== undefined)
            this.state.cctvList = this.props.cctvList;

        if (this.props.streamServerURL !== null && this.props.streamServerURL !== "" && this.props.streamServerURL !== undefined)
            this.state.streamServerURL = this.props.streamServerURL;

        this.initPopupState = this.initPopupState.bind(this);
        this.closeFullScreen = this.closeFullScreen.bind(this);

        this.refCCTV1Title = React.createRef();
        this.refCCTV2Title = React.createRef();
        this.refCCTV3Title = React.createRef();
        this.refCCTV4Title = React.createRef();

        SettingsStore.subscribe(function () {
            this.resetPopupState(SettingsStore.getState());
        }.bind(this));
    }

    componentDidMount() {
        // 팝업 마우스 드래그 이벤트 리스너
        this.popupDragMouseMove = (event) => {
            let mousePosition = {
                x: event.clientX,
                y: event.clientY
            }

            //움직여야할 좌표
            let moveX = mousePosition.x + this.state.dragOffsetX;
            let perMoveX = ((moveX / this.state.maxScreenWidth) * 100);

            let moveY = mousePosition.y + this.state.dragOffsetY;
            let perMoveY = ((moveY / this.state.maxScreenHeight) * 100);

            // 팝업 너비
            let width = this.state.popup.clientWidth;
            let left = this.state.popup.offsetLeft;

            // 팝업 높이
            let height = this.state.popup.clientHeight;
            let top = this.state.popup.offsetTop;

            let popupRightPos = width + left;   // 현재 위치에서 오른쪽 끝 절대 좌표
            let popupBottomPos = height + top;  // 현재 위치에서 아래쪽 끝 절대 좌표

            // 팝업이 화면밖으로 안나가도록 처리
            if (moveX > 0 && moveX + width < this.state.maxScreenWidth) {
                this.state.popup.style.left = perMoveX + '%';
            } else if (moveX + width > this.state.maxScreenWidth) {
                // 드래그 도중 이동할 마우스 포지션 지점부터 팝업 끝지점이 우측 화면 밖을 벗어나게 될 때
                if (popupRightPos < this.state.maxScreenWidth) {
                    // 팝업을 우측 변에 고정
                    let lim = ((this.state.maxScreenWidth - width ) / this.state.maxScreenWidth) * 100;
                    this.state.popup.style.left = lim + '%';
                }else if (this.state.preMousePosition.x > mousePosition.x) {
                    // 화면 오른쪽으로 팝업이 이미 벗어나 있을 때
                    this.state.popup.style.left = perMoveX + '%';
                }
            } else if (moveX <= 0) {
                // 드래그 도중 팝업 시작점이 좌측 화면 밖을 벗어나게 될 때
                if (left > 0) {
                    this.state.popup.style.left = '0%';
                } else if (this.state.preMousePosition.x < mousePosition.x) {
                    // 화면 왼쪽으로 팝업이 이미 벗어나 있을 때
                    this.state.popup.style.left = perMoveX + '%';
                }
            }

            if (moveY > 60 && moveY + height < this.state.maxScreenHeight) {
                this.state.popup.style.top = perMoveY + '%';
            } else if (moveY + height > this.state.maxScreenHeight) {
                // 드래그 도중 이동할 마우스 포지션 지점부터 팝업 하단 끝지점이 화면 밖을 벗어나게 될 때
                if (popupBottomPos < this.state.maxScreenHeight) {
                    // 팝업을 아랫 변에 고정
                    let lim = ((this.state.maxScreenHeight - height) / this.state.maxScreenHeight) * 100;
                    this.state.popup.style.top = lim + '%';
                } else if (this.state.preMousePosition.y > mousePosition.y) {
                    // 화면 아래쪽으로 팝업이 이미 벗어나 있을 때
                    this.state.popup.style.top = perMoveY + '%';
                }
            } else if (moveY <= 60) {
                // 드래그 도중 상단 끝지점이 화면 밖을 벗어나게 될 때
                if (top > 60) {
                    // 팝업을 윗 변에 고정
                    // 상단 툴바는 항상 높이 60 고정이기 때문에 현재 화면 사이즈에서 60px의 비율을 계산한다.
                    let lim = (60 / this.state.maxScreenHeight) * 100;
                    this.state.popup.style.top = lim + '%';
                }else if (this.state.preMousePosition.y < mousePosition.y) {
                    // 화면 위쪽으로 팝업이 이미 벗어나 있을 때
                    this.state.popup.style.top = perMoveY + '%';
                }
            }
        }

        //팝업 리사이즈 이벤트 리스너
        this.popupResizeMouseMove = (event) => {
            let sizeX = 0;
            //let perSizeX = 0.0;

            let sizeY = 0;
            //let perSizeY = 0.0;

            switch (this.state.resizeType) {
                // 수평
                case 'h-r': // 오른쪽 수평
                    sizeX = event.pageX - this.state.originalX;
                    //perSizeX = ((sizeX / this.state.maxScreenWidth) * 100);

                    if (event.pageX > 0 && event.pageX < this.state.maxScreenWidth && sizeX >= this.state.popupMinWidth) {
                        this.state.popup.style.width = sizeX + 'px';
                    }

                    //if (event.pageX > 0 && event.pageX < this.state.maxScreenWidth && perSizeX >= this.state.popupMinWidth) {
                    //    this.state.popup.style.width = perSizeX + '%';
                    //}
                    break;
                case 'h-l': //왼쪽 수평
                    sizeX = this.state.originalWidth - (event.pageX - this.state.originalMouseX);
                    //perSizeX = ((sizeX / this.state.maxScreenWidth) * 100);

                    if (event.pageX > 0 && event.pageX < this.state.maxScreenWidth && sizeX > this.state.popupMinWidth) {
                        this.state.popup.style.width = sizeX + 'px';

                        let pxLeft = (this.state.originalX + (event.pageX - this.state.originalMouseX));
                        this.state.popup.style.left = ((pxLeft / this.state.maxScreenWidth) * 100) + '%';
                    }

                    //if (event.pageX > 0 && event.pageX < this.state.maxScreenWidth && perSizeX > this.state.popupMinWidth) {
                    //    this.state.popup.style.width = perSizeX + '%';

                    //    let pxLeft = (this.state.originalX + (event.pageX - this.state.originalMouseX));
                    //    this.state.popup.style.left = ((pxLeft / this.state.maxScreenWidth) * 100) + '%';
                    //}
                    break;
                // 수직
                case 'v-b': // 바텀 수직
                    sizeY = event.pageY - this.state.originalY;
                    //perSizeY = (sizeY / this.state.maxScreenHeight) * 100;

                    if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && sizeY > this.state.popupMinHeight) {
                        this.state.popup.style.height = sizeY + 'px';
                        //그리드 사이즈를 부모(팝업) 사이즈 비율대로 조절
                        const grid = this.findElementByClassName(content.viewDashboardCCTVGrid);
                        //let grid = document.getElementsByClassName(content.viewDashboardCCTVGrid)[0];
                        if (grid) {
                            grid.style.height = (sizeY - 70) + 'px';
                        }
                    }

                    //if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && perSizeY > this.state.popupMinHeight) {
                    //    this.state.popup.style.height = perSizeY + '%';
                    //    // this.state.popup.style.height = sizeY + 'px';

                    //    //그리드 사이즈를 부모(팝업) 사이즈 비율대로 조절
                    //    let grid = document.getElementsByClassName(content.viewDashboardCCTVGrid)[0];
                    //    grid.style.height = (sizeY - 84) + 'px';
                    //}
                    break;
                case 'v-t': //탑 수직
                    sizeY = this.state.originalHeight - (event.pageY - this.state.originalMouseY);
                    //perSizeY = (sizeY / this.state.maxScreenHeight) * 100;

                    if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && sizeY > this.state.popupMinHeight) {
                        this.state.popup.style.height = sizeY + 'px'

                        let pxTop = this.state.originalY + (event.pageY - this.state.originalMouseY);
                        this.state.popup.style.top = ((pxTop / this.state.maxScreenHeight) * 100) + '%';

                        const grid = this.findElementByClassName(content.viewDashboardCCTVGrid);
                        //let grid = document.getElementsByClassName(content.viewDashboardCCTVGrid)[0];
                        if (grid) {
                            grid.style.height = (sizeY - 70) + 'px';
                        }
                    }

                    //if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && perSizeY > this.state.popupMinHeight) {
                    //    this.state.popup.style.height = perSizeY + '%';
                    //    let pxTop = this.state.originalY + (event.pageY - this.state.originalMouseY);
                    //    this.state.popup.style.top = ((pxTop / this.state.maxScreenHeight) * 100) + '%';

                    //    let grid = document.getElementsByClassName(content.viewDashboardCCTVGrid)[0];
                    //    grid.style.height = (sizeY - 84) + 'px';
                    //}
                    break;
                // 대각
                case 'd-rb': // 오른쪽 하단 대각
                    sizeX = event.pageX - this.state.originalX;
                    //perSizeX = (sizeX / this.state.maxScreenWidth) * 100;

                    sizeY = event.pageY - this.state.originalY;
                    //perSizeY = (sizeY / this.state.maxScreenHeight) * 100;

                    if (event.pageX > 0 && event.pageX < this.state.maxScreenWidth && sizeX > this.state.popupMinWidth) {
                        this.state.popup.style.width = sizeX + 'px';
                    }

                    if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && sizeY > this.state.popupMinHeight) {
                        this.state.popup.style.height = sizeY + 'px';

                        const grid = this.findElementByClassName(content.viewDashboardCCTVGrid);
                        //let grid = document.getElementsByClassName(content.viewDashboardCCTVGrid)[0];
                        if (grid) {
                            grid.style.height = (sizeY - 70) + 'px';
                        }
                    }

                    //if (event.pageX > 0 && event.pageX < this.state.maxScreenWidth && perSizeX > this.state.popupMinWidth) {
                    //    this.state.popup.style.width = perSizeX + '%';
                    //}

                    //if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && perSizeY > this.state.popupMinHeight) {
                    //    this.state.popup.style.height = perSizeY + '%';

                    //    let grid = document.getElementsByClassName(content.viewDashboardCCTVGrid)[0];
                    //    grid.style.height = (sizeY - 84) + 'px';
                    //}
                    break;
                case 'd-rt': //오른쪽 상단 대각
                    sizeX = this.state.originalWidth + (event.pageX - this.state.originalMouseX);
                    //perSizeX = (sizeX / this.state.maxScreenWidth) * 100;

                    sizeY = this.state.originalHeight - (event.pageY - this.state.originalMouseY);
                    //perSizeY = (sizeY / this.state.maxScreenHeight) * 100;

                    if (event.pageX > 0 && event.pageX < this.state.maxScreenWidth && sizeX > this.state.popupMinWidth) {
                        this.state.popup.style.width = sizeX + 'px';
                    }

                    if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && sizeY > this.state.popupMinHeight) {
                        this.state.popup.style.height = sizeY + 'px';

                        let pxTop = this.state.originalY + (event.pageY - this.state.originalMouseY);
                        this.state.popup.style.top = ((pxTop / this.state.maxScreenHeight) * 100) + '%';

                        const grid = this.findElementByClassName(content.viewDashboardCCTVGrid);
                        //let grid = document.getElementsByClassName(content.viewDashboardCCTVGrid)[0];
                        grid.style.height = (sizeY - 70) + 'px';
                    }

                    //if (event.pageX > 0 && event.pageX < this.state.maxScreenWidth && perSizeX > this.state.popupMinWidth) {
                    //    this.state.popup.style.width = perSizeX + '%';
                    //    // this.state.popup.style.width = sizeX + 'px';
                    //}

                    //if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && perSizeY > this.state.popupMinHeight) {
                    //    this.state.popup.style.height = perSizeY + '%';
                    //    // this.state.popup.style.height = sizeY + 'px';
                    //    let pxTop = this.state.originalY + (event.pageY - this.state.originalMouseY);
                    //    this.state.popup.style.top = ((pxTop / this.state.maxScreenHeight) * 100) + '%';
                    //    // this.state.popup.style.top = this.state.originalY + (event.pageY - this.state.originalMouseY) + 'px';

                    //    let grid = document.getElementsByClassName(content.viewDashboardCCTVGrid)[0];
                    //    grid.style.height = (sizeY - 84) + 'px';
                    //}
                    break;
                case 'd-lb': //왼쪽 하단 대각
                    sizeX = this.state.originalWidth - (event.pageX - this.state.originalMouseX);
                    //perSizeX = (sizeX / this.state.maxScreenWidth) * 100;

                    sizeY = event.pageY - this.state.originalY;
                    //perSizeY = (sizeY / this.state.maxScreenHeight) * 100;

                    if (event.pageX > 0 && event.pageX < this.state.maxScreenWidth && sizeX > this.state.popupMinWidth) {
                        this.state.popup.style.width = sizeX + 'px';

                        let pxLeft = (this.state.originalX + (event.pageX - this.state.originalMouseX));
                        this.state.popup.style.left = ((pxLeft / this.state.maxScreenWidth) * 100) + '%';
                    }

                    if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && sizeY > this.state.popupMinHeight) {
                        this.state.popup.style.height = sizeY + 'px';

                        const grid = this.findElementByClassName(content.viewDashboardCCTVGrid);
                        //let grid = document.getElementsByClassName(content.viewDashboardCCTVGrid)[0];
                        grid.style.height = (sizeY - 70) + 'px';
                    }

                    //if (event.pageX > 0 && event.pageX < this.state.maxScreenWidth && perSizeX > this.state.popupMinWidth) {
                    //    this.state.popup.style.width = perSizeX + '%';
                    //    // this.state.popup.style.width = sizeX + 'px';

                    //    let leftPx = (this.state.originalX + (event.pageX - this.state.originalMouseX));
                    //    this.state.popup.style.left = ((leftPx / this.state.maxScreenWidth) * 100) + '%';
                    //    // this.state.popup.style.left = this.state.originalX + (event.pageX - this.state.originalMouseX) + 'px';
                    //}

                    //if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && perSizeY > this.state.popupMinHeight) {
                    //    this.state.popup.style.height = perSizeY + '%';
                    //    // this.state.popup.style.height = sizeY + 'px';

                    //    let grid = document.getElementsByClassName(content.viewDashboardCCTVGrid)[0];
                    //    grid.style.height = (sizeY - 84) + 'px';
                    //}
                    break;

                case 'd-lt': //왼쪽 상단 대각
                    sizeX = this.state.originalWidth - (event.pageX - this.state.originalMouseX);
                    //perSizeX = (sizeX / this.state.maxScreenWidth) * 100;

                    sizeY = this.state.originalHeight - (event.pageY - this.state.originalMouseY);
                    //perSizeY = (sizeY / this.state.maxScreenHeight) * 100;

                    if (event.pageX > 0 && event.pageX < this.state.maxScreenWidth && sizeX > this.state.popupMinWidth) {
                        this.state.popup.style.width = sizeX + 'px';

                        let pxLeft = (this.state.originalX + (event.pageX - this.state.originalMouseX));
                        this.state.popup.style.left = ((pxLeft / this.state.maxScreenWidth) * 100) + '%';
                    }

                    if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && sizeY > this.state.popupMinHeight) {
                        this.state.popup.style.height = sizeY + 'px';

                        let pxTop = this.state.originalY + (event.pageY - this.state.originalMouseY);
                        this.state.popup.style.top = ((pxTop / this.state.maxScreenHeight) * 100) + '%';

                        const grid = this.findElementByClassName(content.viewDashboardCCTVGrid);
                        //let grid = document.getElementsByClassName(content.viewDashboardCCTVGrid)[0];
                        grid.style.height = (sizeY - 70) + 'px';
                    }

                    //if (event.pageX > 0 && event.pageX < this.state.maxScreenWidth && perSizeX > this.state.popupMinWidth) {
                    //    this.state.popup.style.width = perSizeX + '%';
                    //    // this.state.popup.style.width = sizeX + 'px';
                    //    let leftPx = (this.state.originalX + (event.pageX - this.state.originalMouseX));
                    //    this.state.popup.style.left = ((leftPx / this.state.maxScreenWidth) * 100) + '%';
                    //    // this.state.popup.style.left = this.state.originalX + (event.pageX - this.state.originalMouseX) + 'px';
                    //}

                    //if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && perSizeY > this.state.popupMinHeight) {
                    //    this.state.popup.style.height = perSizeY + '%';
                    //    // this.state.popup.style.height = sizeY + 'px';
                    //    let topPx = this.state.originalY + (event.pageY - this.state.originalMouseY);
                    //    this.state.popup.style.top = ((topPx / this.state.maxScreenHeight) * 100) + '%';
                    //    // this.state.popup.style.top = this.state.originalY + (event.pageY - this.state.originalMouseY) + 'px';

                    //    let grid = document.getElementsByClassName(content.viewDashboardCCTVGrid)[0];
                    //    grid.style.height = (sizeY - 84) + 'px';
                    //}
                    break;
                default:
            }


            this.initSizeCCTV();
            this.showCCTVs();
            this.resizeFullScreenCCTV();
        }
        this.initPopupState();

        this.setCctvFullScreenInit();

        // 포커스 CCTV 팝업창
        this.initCCTVPopupFocus();

        this.props.setActiveDragPopup(this.props.popupType);

        // 모드에 따른 닫기 버튼 표시여부
        if (this.props.editMode === Contents3D.Edit_Mode_CCTVGroup) {
            $('#cctvInfoCloseBtn').hide();
        } else {
            $('#cctvInfoCloseBtn').show();
        }
    }

    initCCTVPopupFocus() {
        const alarmInfo = this.props.alarmInfo;
        const selectedAlarm = this.props.selectedAlarm;

        if (alarmInfo === null || alarmInfo === undefined ||
            selectedAlarm === null || selectedAlarm === undefined)
            return;

        // CCTV 팝업창이 포커스된 경우
        if (alarmInfo[1].sensorZoneHistoryID === selectedAlarm.sensorZoneHistoryID) {
            $(".cctvAlarm_" + alarmInfo[1].sensorZoneHistoryID).addClass(content.dslGrdAct);
        }
    }

    shouldComponentUpdate(nextProps, nextState) {
        if (this.isDifferntCCTVList(nextProps, nextState)) {
            return true;
        }

        if (this.isDifferntSize(nextProps, nextState)) {
            return true;
        }

        this.setCCTVClasses(nextProps);
        return false;
    }

    isDifferntSize(nextProps, nextState) {
        const currentStyle = this.state.popup?.style;
        const nextStyle = nextState.popup?.style;

        if (!currentStyle && !nextStyle) {
            return false;
        }
        else if (!currentStyle) {
            console.log("current style is : " + currentStyle);
            return true;
        }
        else if (!nextStyle) {
            console.log("next style is : " + nextStyle);
            return true;
        }

        if (currentStyle.width !== nextStyle.width) {
            console.log("current width : " + currentStyle.width + ", next width : " + nextStyle.width);
            return true;
        }

        if (currentStyle.height !== nextStyle.height) {
            console.log("current height : " + currentStyle.height + ", next height : " + nextStyle.height);
            return true;
        }

        return false;
    }

    isDifferntCCTVList(nextProps, nextState) {
        const cctvList1 = this.props.cctvList;
        const cctvList2 = nextProps.cctvList;

        if (cctvList1 !== cctvList2) {
            console.log("cctvList1 : " + cctvList1 + ", cctvList2 : " + cctvList2);
            return true;
        }

        if (this.state.fullScreenIndex !== nextState.fullScreenIndex) {
            console.log("current fullScreenIndex : " + this.state.fullScreenIndex + ", next fullScreenIndex : " + nextState.fullScreenIndex);
            return true;
        }

        return false;
    }

    componentDidUpdate(prevProps, prevState) {
        /*if (this.props.cctvList !== prevProps.cctvList)*/ {
            this.state.cctvList = this.props.cctvList;
            this.showCCTVs();
        }

        if (this.props.streamServerURL !== prevProps.streamServerURL) {
            this.state.streamServerURL = this.props.streamServerURL;
        }

        // 팝업이 선택 됐을 때(Drag 될때) 맨 앞에 팝업 위치
        if (this.props.zIndex !== prevProps.zIndex) {
            this.state.popup.style.zIndex = this.props.zIndex;
            console.log('cctvInfoZIndex changed', this.state.popup.style.zIndex)
        }
    }

    initPopupState() {
        const popup = this.findElementByClassName(content.viewDashboardBoxD + ' ' + content.viewDashboardCCTV);
        /*const popups = document.getElementsByClassName(content.viewDashboardBoxD + ' ' + content.viewDashboardCCTV);

        if (popups === null || popups === undefined)
            return;

        let popup = null;

        for (let i = 0; i < popups.length; i++) {
            if (popups[i].id === this.props.popupType) {
                popup = popups[i];
                break;
            }
        }*/

        if (!popup)
            return;

        //DB에 값이 있을 경우에만
        if (typeof this.props.popupState !== 'undefined') {
            popup.style.left = this.props.popupState.x;
            popup.style.top = this.props.popupState.y;
            popup.style.width = this.props.popupState.width;
            popup.style.height = this.props.popupState.height;

            const grid = this.findElementByClassName(content.viewDashboardCCTVGrid);
            //let grid = document.getElementsByClassName(content.viewDashboardCCTVGrid)[0];
            grid.style.height = (popup.offsetHeight - 70) + 'px';
        } else {
            // DB에 값이 따로 없을 경우
            let data = SDMSResource.popupResetLocation[this.props.popupType];

            popup.style.left = data.x;
            popup.style.top = data.y;
            popup.style.width = data.width;
            popup.style.height = data.height;

            const grid = this.findElementByClassName(content.viewDashboardCCTVGrid);

            grid.style.height = (popup.offsetHeight - 70) + 'px';
        }

        this.initSizeCCTV();
        this.showCCTVs();  
        this.setState({ popup: popup });
    }

    initSizeCCTV = () => {
        // cctv 화면 사이즈 체크
        //const frame = document.querySelector("#cctv1");
        const rootElement = document.querySelector("#" + this.props.popupType);

        if (!rootElement) {
            return;
        }

        const frames = [
            this.getChildElement(rootElement, "cctv1"),
            this.getChildElement(rootElement, "cctv2"),
            this.getChildElement(rootElement, "cctv3"),
            this.getChildElement(rootElement, "cctv4")
            /*document.querySelector("#cctv1"),
            document.querySelector("#cctv2"),
            document.querySelector("#cctv3"),
            document.querySelector("#cctv4")*/
        ];

        //그리드에 맞춰 iframe 사이즈를 재조정한다.
        const grid_col = this.findElementByClassName(content.col1row1);
        //const grid_col = document.getElementsByClassName(content.col1row1)[0];
        if (grid_col) {
            let width = grid_col.clientWidth;
            let height = grid_col.clientHeight;

            for (let frame of frames) {
                if (frame === null || frame === undefined)
                    continue;

                frame.style.width = width + "px";
                frame.style.height = height + "px";
            }

            this.state.cctvWidth = width;
            this.state.cctvHeight = height;

            this.setState({ cctvWidth: width, cctvHeight: height });

        }
    }

    repositionPopup(popupState) {
        //let data = popupState.cctvInfo;
        let data = popupState[this.props.popupType];

        if (data === null || data === undefined)
            return;
        
        const popup = this.findElementByClassName(content.viewDashboardBoxD + ' ' + content.viewDashboardCCTV);
        //let popup = document.getElementsByClassName(content.viewDashboardBoxD + ' ' + content.viewDashboardCCTV)[0];
        if (popup === null || popup === undefined)
            return;

        popup.style.left = data.x;
        popup.style.top = data.y;
        popup.style.width = data.width;
        popup.style.height = data.height;

        const grid = this.findElementByClassName(content.viewDashboardCCTVGrid);
        //let grid = document.getElementsByClassName(content.viewDashboardCCTVGrid)[0];
        grid.style.height = (popup.offsetHeight - 70) + 'px';

        this.initSizeCCTV();
        this.showCCTVs();
        this.setState({ popup: popup });
    }

    resetPopupState = (popupState) => {
        let data = popupState;

        if (data.actionType === 'RESET_POPUP') {
            this.repositionPopup(data.popupState);
        }
    }

    setEditModeCCTVList(ids, idCount) {
        const cctvList = [null, null, null, null];
        const [equipZoneID, equipZoneName] = this.getCurrentEquipZoneInfo();

        if (equipZoneID === 0 || equipZoneID) {
            let index = 0;

            for (let i = 0; i < idCount; i++) {
                const cctvID = ids[i].trim();

                if (cctvID.length > 0) {
                    cctvList[index++] = cctvID;
                }
            }

            this.props.editModeManager.setEquipZoneCCTVGroup(equipZoneID, cctvList[0], cctvList[1], cctvList[2], cctvList[3]);
        }
    }

    setCCTVClasses(props) {
        const input = !this.state.cctvList ? "" : this.state.cctvList;
        const cctvIDs = input.toString().trim();

        const ids = cctvIDs.split(',');
        let idCount = ids.length;

        if (idCount > this.state.cctvCountMax)
            idCount = this.state.cctvCountMax;

        let index = 1;

        for (let i = 0; i < idCount; i++) {
            const suuid = ids[i].trim();

            if (suuid.length === 0) {
                this.setTitleClassName(i, '');
                continue;
            }

            if (this.isSelectedCCTV(suuid, props)) {
                this.setTitleClassName(i, content.selected);
            }
            else {
                this.setTitleClassName(i, '');
            }

            index++;
        }

        for (let i = index; i <= this.state.cctvCountMax; i++) {
            this.setTitleClassName(i, '');
        }
    }

    showCCTVs = () => {
        const input = !this.state.cctvList ? "" : this.state.cctvList;
        const cctvIDs = input.toString().trim();
        
        const ids = cctvIDs.split(',');
        let idCount = ids.length;

        if (idCount > this.state.cctvCountMax)
            idCount = this.state.cctvCountMax;

        if (this.props.editMode === Contents3D.Edit_Mode_CCTVGroup) {
            this.setEditModeCCTVList(ids, idCount);
        }

        let index = 1;

        for (let i = 0; i < idCount; i++) {
            const suuid = ids[i].trim();

            if (suuid.length === 0) {
                this.setTitleClassName(i, '');
                continue;
            }

            if (this.isSelectedCCTV(suuid, this.props)) {
                this.setTitleClassName(i, content.selected);
            }
            else {
                this.setTitleClassName(i, '');
            }

            const id = '#cctv' + index.toString();
            this.connectStream(suuid, id);
            index++;
        }

        for (let i = index; i <= this.state.cctvCountMax; i++) {
            const id = '#cctv' + i.toString();
            this.closeStream(id);
            this.setTitleClassName(i, '');
        }
    }

    setTitleClassName(index, className) {
        let title = null;

        if (index === 0 && this.refCCTV1Title !== null && this.refCCTV1Title.current !== null) {
            title = this.refCCTV1Title;
        }
        else if (index === 1 && this.refCCTV2Title !== null && this.refCCTV2Title.current !== null) {
            title = this.refCCTV2Title;
        }
        else if (index === 2 && this.refCCTV3Title !== null && this.refCCTV3Title.current !== null) {
            title = this.refCCTV3Title;
        }
        else if (index === 3 && this.refCCTV4Title !== null && this.refCCTV4Title.current !== null) {
            title = this.refCCTV4Title;
        }
        else {
            return;
        }

        if (className === '') {
            title.current.removeAttribute('class');
        }
        else if (title.current.classList.contains(className) === false) {
            title.current.removeAttribute('class');
            title.current.classList.add(className);
        }
    }

    isSelectedCCTV(id, props) {
        if (id === null) {
            return false;
        }

        const _id = parseInt(id);

        if (_id !== null && _id !== undefined && isNaN(_id) === false) {
            return _id === props.selectedCCTVID;
        }

        return false;
    }

    connectStream = (suuid, id) => {
        const rootElement = document.querySelector("#" + this.props.popupType);

        if (!rootElement) {
            return;
        }

        const findID = id.startsWith('#') ? id.substring(1) : id;
        const frame = this.getChildElement(rootElement, findID);

        //const frame = document.querySelector(id);

        if (frame === null || frame === undefined)
            return;

        let width = this.state.cctvWidth;
        let height = this.state.cctvHeight;


        if (width === 0 || height === 0) {
            const frame = this.getChildElement(rootElement, "cctv1");
            //const frame = document.querySelector("#cctv1");

            if (frame === null || frame === undefined)
                return;

            width = frame.clientWidth;
            height = frame.clientHeight;
        }


        let param = "";
        if (width !== null && width !== undefined && height !== null && height !== undefined) {
            param = "?w=" + width + "&h=" + height;
        }
        else if (width !== null && width !== undefined) {
            param = "?w=" + width;
        }
        else if (height !== null && height !== undefined) {
            param = "?h=" + height;
        }

        //let url = CCTVInfo.streamServerURL + suuid + param;
        let url = this.state.streamServerURL + "/stream/player/" + suuid + param;
        frame.setAttribute("src", url);

        //전체화면 이벤트에 사용
        frame.parentNode.dataset.url = this.state.streamServerURL + "/stream/player/" + suuid;
        frame.previousElementSibling.dataset.url = this.state.streamServerURL + "/stream/player/" + suuid;


        let cctvs = this.props.cctvs;
        let cctvName;
        for (let i = 0; cctvs.length > i; i++) {
            if (cctvs[i].id == suuid) {
                cctvName = suuid + ". " + cctvs[i].name;
                break;
            }
        }

        frame.parentNode.dataset.cctvname = cctvName;
        frame.previousElementSibling.dataset.cctvname = cctvName;


        // CCTV 번호 및 이름 표시
        this.showCCTVInfo(suuid, id);

        const hidden = frame.classList.contains(sdms.hidden);

        if (hidden) {
            frame.classList.remove(sdms.hidden);
        }
    }

    getChildElement(parent, id) {
        if (parent.hasChildNodes()) {
            const childCount = parent.childNodes.length;

            for (let i = 0; i < childCount; i++) {
                const child = parent.childNodes[i];

                if (child.id === id) {
                    return child;
                }
                else {
                    const result = this.getChildElement(child, id);

                    if (result !== null)
                        return result;
                }
            }
        }

        return null;
    }

    findElementByClassName(className, parent) {
        if (!parent) {
            const rootElement = document.querySelector("#" + this.props.popupType);

            if (!rootElement) {
                return null;
            }

            parent = rootElement;

            if (parent.className === className)
                return parent;
        }

        if (parent.hasChildNodes()) {
            const childCount = parent.childNodes.length;

            for (let i = 0; i < childCount; i++) {
                const child = parent.childNodes[i];

                if (child.className === className) {
                    return child;
                }
                else {
                    const result = this.findElementByClassName(className, child);

                    if (result !== null)
                        return result;
                }
            }
        }

        return null;
    }

    showCCTVInfo = (suuid, id) => {
        if (this.props.ccvts === null || this.props.cctvs === undefined)
            return;

        let cctvs = this.props.cctvs;
        let titleID = id + "_name";
        
        let cctvName = "";

        for (let i = 0; cctvs.length > i; i++) {
            if (cctvs[i].id == suuid) {
                cctvName = suuid + ". " + cctvs[i].name;
                break;
            }
        }

        const rootElement = document.querySelector("#" + this.props.popupType);

        if (!rootElement) {
            return;
        }

        const findTitleID = titleID.startsWith('#') ? titleID.substring(1) : titleID;
        const title = this.getChildElement(rootElement, findTitleID);
        //let title = document.querySelector(titleID);

        if (title !== null) {
            title.innerHTML = cctvName;
        }
    }

    closeStream = (id) => {
        const rootElement = document.querySelector("#" + this.props.popupType);

        if (!rootElement) {
            return;
        }

        const findID = id.startsWith('#') ? id.substring(1) : id;
        const frame = this.getChildElement(rootElement, findID);
        //const frame = document.querySelector(id);

        if (frame === null)
            return;

        let titleID = id + "_name";
        const findTitleID = titleID.startsWith('#') ? titleID.substring(1) : titleID;

        const title = this.getChildElement(rootElement, findTitleID);
        //const title = document.querySelector(titleID);

        if (frame !== null && frame !== undefined) {
            const url = "";
            frame.setAttribute("src", url);

            title.innerHTML = "";

            const hidden = frame.classList.contains(sdms.hidden);

            if (hidden === false) {
                frame.classList.add(sdms.hidden);
            }
        }
    }

    onClickReset = () => {
        this.showCCTVs();
    }

    // 팝업 드래그 시작(팝업을 누르고 있을 때)
    popupDragMousePress(event) {
        if (event.button == 0) {
            //마우스 조작중에 브라우저의 크기를 조절할 수 없으므로
            // 이 시점에 도큐먼트 전체 크기를 호출한다.
            this.setState({
                maxScreenHeight: document.getElementsByTagName('body')[0].clientHeight,
                maxScreenWidth: document.getElementsByTagName('body')[0].clientWidth,
                dragOffsetX: this.state.popup.offsetLeft - event.clientX,
                dragOffsetY: this.state.popup.offsetTop - event.clientY,
                preMousePosition: {
                    x: event.clientX,
                    y: event.clientY
                }
            });
            
            document.addEventListener('mousemove', this.popupDragMouseMove);
            document.addEventListener('mouseup', this.popupDragMouseUp);

            // z-index 조정, 1 이 다른 팝업보다 앞에 배치됨
            this.props.setActiveDragPopup(this.props.popupType); /* 0524 */
        }
    }
    // 팝업 드래그 종료(mouse up)
    popupDragMouseUp = () => {
        console.log('popup drag false')
        document.removeEventListener('mousemove', this.popupDragMouseMove);
        document.removeEventListener('mouseup', this.popupDragMouseUp);
        // 팝업 정보 DB 작성
        this.setPopupState();
    }

    // 팝업 리사이징(누르고 있을 때)
    popupResizeMousePress(event, resizeType) {
        /* resizeType
         * h-r      오른쪽 수평
         * h-l      왼쪽 수평
         * v-b      바텀 수직
         * v-t      탑 수직
         * d-rt     우측 상단 대각
         * d-rb     우측 하단 대각
         * d-lt     좌축 상단 대각
         * d-lb     좌측 하단 대각
        */

        console.log('popupResizeMousePress');
        this.setState({
            maxScreenHeight: document.getElementsByTagName('body')[0].clientHeight,
            maxScreenWidth: document.getElementsByTagName('body')[0].clientWidth,
            resizeType: resizeType,
            originalMouseX: event.pageX,
            originalMouseY: event.pageY,
            originalWidth: parseFloat(getComputedStyle(this.state.popup, null).getPropertyValue('width').replace('px', '')),
            originalHeight: parseFloat(getComputedStyle(this.state.popup, null).getPropertyValue('height').replace('px', '')),
            originalX: this.state.popup.getBoundingClientRect().left,
            originalY: this.state.popup.getBoundingClientRect().top,
        });

        document.addEventListener('mousemove', this.popupResizeMouseMove);

        document.addEventListener('mouseup', this.popupResizeMouseUp);
        // z-index 조정, 1 이 다른 팝업보다 앞에 배치됨
        this.props.setActiveDragPopup(this.props.popupType);
    }

    popupResizeMouseUp = () => {
        console.log('popup resize false');
        document.removeEventListener('mousemove', this.popupResizeMouseMove);
        document.removeEventListener('mouseup', this.popupResizeMouseUp);

        this.setState({ resizeType: null });
        this.setPopupState();
    }

    setPopupState() {
        // 팝업 정보 DB 작성
        let perX = ((this.state.popup.offsetLeft / this.state.maxScreenWidth) * 100);
        let perY = ((this.state.popup.offsetTop) / this.state.maxScreenHeight * 100);
        let width = this.state.popup.offsetWidth;
        let height = this.state.popup.offsetHeight;

        //팝업 비활성화 될 때 컴포넌트가 사라져 계산식이 0으로 되는 현상이 발생함. 이때 DB 등록되는것을 방지
        if (perX > 0 && perY > 0 && width > 0 && height > 0) {
            let popupState = {
                id: typeof this.props.popupState !== 'undefined' ? this.props.popupState.id : -1,
                x: perX + '%',
                y: perY + '%',
                height: height + 'px',
                width: width + 'px'
            }
            this.props.setPopupState(this.props.popupType, popupState);
        }

        //let perX = ((this.state.popup.offsetLeft / this.state.maxScreenWidth) * 100);
        //let perY = ((this.state.popup.offsetTop) / this.state.maxScreenHeight * 100);
        //let perWidth = ((this.state.popup.offsetWidth) / this.state.maxScreenWidth * 100);
        //let perHeight = ((this.state.popup.offsetHeight / this.state.maxScreenHeight) * 100);

        //팝업 비활성화 될 때 컴포넌트가 사라져 계산식이 0으로 되는 현상이 발생함. 이때 DB 등록되는것을 방지
        //if (perX > 0 && perY > 0 && perWidth > 0 && perHeight > 0){
        //    let popupState = {
        //        id: typeof this.props.popupState !== 'undefined' ? this.props.popupState.id : -1,
        //        x: perX + '%',
        //        y: perY + '%',
        //        height: perHeight + '%',
        //        width: perWidth + '%'
        //    }
        //    this.props.setPopupState('cctvInfo', popupState);
        //}
    }

    resizeFullScreenCCTV() {
        const fullScreen = this.findElementByClassName(content.fullScreenCCTV);
        //let fullScreen = document.getElementsByClassName(content.fullScreenCCTV)[0];
        if (fullScreen) {
            const cctvConts = this.findElementByClassName(content.viewDashboardCCTVConts);
            //let cctvConts = document.getElementsByClassName(content.viewDashboardCCTVConts)[0];
            let width = cctvConts.offsetWidth - 18;
            let height = cctvConts.offsetHeight - 8;

            fullScreen.style.width = width + 'px';
            fullScreen.style.height = height + 'px';
            
            //닫기 이벤트용 div
            let eventTag = fullScreen.firstElementChild;
            eventTag.style.width = width + 'px';
            eventTag.style.height = height + 'px';

            let cctvName = fullScreen.childNodes[1].innerText;
            
            let frame = fullScreen.lastElementChild;

            let url = frame.src.split('?')[0];

            frame.src = url + "?w=" + width + "&h=" + height;

            this.props.setCctvFullScreenState({
                isFullScreen: true,
                cctvName: cctvName,
                url: url,
                w: width,
                h: height,
            });
        }
    }

    showFullScreenCCTV(index) {
        const rootElement = document.querySelector("#" + this.props.popupType);

        if (!rootElement) {
            return;
        }

        const gridParent = this.findElementByClassName(content.viewDashboardCCTVGrid, rootElement);
        //const gridParent = document.getElementsByClassName(content.viewDashboardCCTVGrid)[0];

        if (this.state.fullScreenIndex === index) {
            if (gridParent) {
                let width = (gridParent.clientWidth - 10) / 2;
                let height = (gridParent.clientHeight - 10) / 2;

                // 사이즈 조절하기 위한 url 재작성
                let id = "cctv" + index.toString();
                const frame = this.getChildElement(rootElement, id);
                //let id = "#cctv" + index.toString();
                //const frame = document.querySelector(id);

                if (frame !== null && frame !== undefined) {
                    let url = frame.parentNode.dataset.url + "?w=" + width + "px&h=" + height + "px";
                    frame.setAttribute("src", url);
                }

                this.setFrameSize(width, height);
                this.setState({ fullScreenIndex: -1, cctvWidth: width, cctvHeight: height });
            }
            else {
                this.setState({ fullScreenIndex: -1 });
            }
        }
        else {
            if (gridParent) {
                let width = gridParent.clientWidth;
                let height = gridParent.clientHeight;

                // 사이즈 조절하기 위한 url 재작성
                let id = "cctv" + index.toString();
                const frame = this.getChildElement(rootElement, id);
                //let id = "#cctv" + index.toString();
                //const frame = document.querySelector(id);

                if (frame !== null && frame !== undefined) {
                    let url = frame.parentNode.dataset.url + "?w=" + width + "px&h=" + height + "px";
                    frame.setAttribute("src", url);
                }

                this.setFrameSize(width, height);
                this.setState({ fullScreenIndex: index, cctvWidth: width, cctvHeight: height });
            }
            else {
                this.setState({ fullScreenIndex: index });
            }
        }
    }

    setFrameSize(width, height) {
        const rootElement = document.querySelector("#" + this.props.popupType);

        if (!rootElement) {
            return;
        }

        for (let i = 1; i <= 4; i++) {
            //그리드에 맞춰 iframe 사이즈를 재조정한다.
            const frame = this.getChildElement(rootElement, "cctv" + i);
            //const frame = document.querySelector("#cctv" + i);

            if (frame) {
                frame.style.width = width + "px";
                frame.style.height = height + "px";
            }
        }
    }

    /*showFullScreenCCTV(e) {
        console.log('showFullScreenCCTV');
        let url = e.target.dataset.url;
        if (url !== undefined) {
            let cctvConts = document.getElementsByClassName(content.viewDashboardCCTVConts)[0];

            let width = cctvConts.offsetWidth - 18;
            let height = cctvConts.offsetHeight - 8;

            //전체화면 프레임 div
            let div = document.createElement('div');

            div.style.width = width + "px";
            div.style.height = height + "px";
            div.classList.add(content.fullScreenCCTV);

            let cctvName = document.createElement('span');
            cctvName.innerText = e.target.dataset.cctvname;


            //닫기 이벤트용 div
            let eventTag = document.createElement('div');
            eventTag.style.width = width + 'px';
            eventTag.style.height = height + 'px';

            //전체화면 닫기 이벤트
            eventTag.addEventListener('dblclick', this.closeFullScreen, false);

            

            div.appendChild(eventTag);
            div.appendChild(cctvName);

            let frame = document.createElement('iframe');
            frame.src = url + "?w=" + width + "&h=" + height;
            div.appendChild(frame);

            let div2 = document.createElement('div');
            div2.appendChild(div);
            cctvConts.prepend(div2);


            let viewDashboardCCTVview = document.getElementsByClassName(content.viewDashboardCCTVview)[0];
            // display none으로 하면 전체화면을 띄우기 위한 크기 정보를 얻을 수 없으므로 화면상에서만 태그를 감춘다.
            viewDashboardCCTVview.style.visibility = 'hidden';


            //컴포넌트가 사라졌다 다시 마운트되도 전체화면 유지되도록 설정
            this.props.setCctvFullScreenState({
                isFullScreen: true,
                url: url,
                cctvName: e.target.dataset.cctvname,
                w: width,
                h: height,
            });
        }
    }*/

    onClickCCTV(index) {
        if (this.props.editMode === Contents3D.Edit_Mode_CCTVGroup) {
            if (this.props.editModeParam === CCTVInfo.Mode_Delete_CCTV) {
                this.removeCCTV(index);
            }

            console.log("onClickCCTV : " + index + ", " + this.state.cctvList);
        }
    }

    removeCCTV(index) {
        const cctvList = this.state.cctvList;

        if (!cctvList || cctvList.length === 0) {
            return;
        }

        const cctvIDs = cctvList.split(',');
        const cctvCount = cctvIDs.length;

        let newList = "";
        const newCCTVIDs = [null, null, null, null];
        let j = 0;

        for (let i = 0; i < cctvCount; i++) {
            if (i === index) {
                continue;
            }

            if (newList.length === 0) {
                newList = cctvIDs[i];
            }
            else {
                newList += "," + cctvIDs[i];
            }

            newCCTVIDs[j++] = cctvIDs[i];
        }

        this.props.setCCTVList(newList);

        const [equipZoneID, equipZoneName] = this.getCurrentEquipZoneInfo();

        if (equipZoneID !== null) {
            this.props.editModeManager.setEquipZoneCCTVGroup(equipZoneID, newCCTVIDs[0], newCCTVIDs[1], newCCTVIDs[2], newCCTVIDs[3]);
        }
    }

    // 전체화면 닫을 경우 실행
    // 전체화면 닫기 기능은 addEventListener로 구현되어 직접 props에 접근할 수 없으므로 아래 함수가 역할을 대행한다.
    closeFullScreen(e) {
        console.log('closeFullSize');
        const viewDashboardCCTVview = this.findElementByClassName(content.viewDashboardCCTVview);
        //let viewDashboardCCTVview = document.getElementsByClassName(content.viewDashboardCCTVview)[0];
        viewDashboardCCTVview.style.visibility = 'visible';

        const fullScreenCCTV = this.findElementByClassName(content.fullScreenCCTV);

        if (fullScreenCCTV)
            fullScreenCCTV.remove();
        //document.getElementsByClassName(content.fullScreenCCTV)[0].remove();

        this.props.setCctvFullScreenState({
            isFullScreen: false,
            url: null,
            cctvName: null,
            w: null,
            h: null,
        });

    }

    //컴포넌트가 마운트 될 때, 이전에 cctv 전체화면을 띄운경우 그대로 띄워준다.
    setCctvFullScreenInit() {
        let cctvFullScreenState = this.props.cctvFullScreenState;

        if (cctvFullScreenState.isFullScreen) {
            let url = cctvFullScreenState.url;

            let width = cctvFullScreenState.w;
            let height = cctvFullScreenState.h;

            const cctvConts = this.findElementByClassName(content.viewDashboardCCTVConts);
            //let cctvConts = document.getElementsByClassName(content.viewDashboardCCTVConts)[0];
            let cctvName = cctvFullScreenState.cctvName;

            let isUnmatchedSize = false;
            
            //사이즈 드래그 하는 도중 팝업이 사라질때 cctv 전체화면이 팝업 사이즈와 맞지 않는 문제 해결
            if (width != cctvConts.offsetWidth - 18) {
                width = cctvConts.offsetWidth - 18;
                isUnmatchedSize = true;
            }
            if (height != cctvConts.offsetHeight - 8) {
                height = cctvConts.offsetHeight - 8;
                isUnmatchedSize = true;
            }

            //사이즈가 맞지 않을 때 props도 갱신한다.
            if (isUnmatchedSize) {
                this.props.setCctvFullScreenState({
                    isFullScreen: true,
                    cctvName: cctvName,
                    url: url,
                    w: width,
                    h: height
                });
            }


            //전체화면 프레임 div
            let div = document.createElement('div');
            div.style.width = width + "px";
            div.style.height = height + "px";
            div.classList.add(content.fullScreenCCTV);

            let title = document.createElement('span');
            title.innerText = cctvName;

            //닫기 이벤트용 div
            let eventTag = document.createElement('div');
            eventTag.style.width = width + 'px';
            eventTag.style.height = height + 'px';

            //전체화면 닫기 이벤트
            eventTag.addEventListener('dblclick', function (e) {
                console.log('closeFullSize');
                const viewDashboardCCTVview = this.findElementByClassName(content.viewDashboardCCTVview);
                //let viewDashboardCCTVview = document.getElementsByClassName(content.viewDashboardCCTVview)[0];
                viewDashboardCCTVview.style.visibility = 'visible';
                div.remove();
            });

            div.appendChild(eventTag);
            div.appendChild(title);

            let frame = document.createElement('iframe');
            frame.src = url + "?w=" + width + "&h=" + height;
            div.appendChild(frame);

            cctvConts.prepend(div);

            const viewDashboardCCTVview = this.findElementByClassName(content.viewDashboardCCTVview);
            //let viewDashboardCCTVview = document.getElementsByClassName(content.viewDashboardCCTVview)[0];
            // display none으로 하면 전체화면을 띄우기 위한 크기 정보를 얻을 수 없으므로 화면상에서만 태그를 감춘다.
            viewDashboardCCTVview.style.visibility = 'hidden';
        }
    }

    getCurrentEquipZoneInfo() {
        if (this.props.editMode === Contents3D.Edit_Mode_CCTVGroup) {
            const editModeManager = this.props.editModeManager;

            if (editModeManager && editModeManager.cctvGroupDatas && editModeManager.cctvGroupDatas.length === 3) {
                const equipZoneID = editModeManager.cctvGroupDatas[0];
                const equipZoneName = editModeManager.cctvGroupDatas[1];

                if ((equipZoneID === 0 || equipZoneID) && equipZoneName && equipZoneName.length > 0) {
                    return [equipZoneID, equipZoneName];
                }
            }
        }

        return [null, null];
    }

    getTitle() {
        let title = "CCTV 영상정보";
        const [equipZoneID, equipZoneName] = this.getCurrentEquipZoneInfo();

        if (equipZoneName !== null) {
            title += " - " + equipZoneName;
        }
        else {
            if (this.props.alarmInfo) {
                if (this.props.alarmInfo.length >= 2) {
                    const alarmType = this.props.alarmInfo[0];
                    const selectedAlarm = this.props.alarmInfo[1];

                    title = "[" + alarmType + "] " + selectedAlarm.positionName + " - [" + selectedAlarm.dtTime.replace('T', ' ') + "]";
                    //title += " - [" + alarmType + "] " + selectedAlarm.positionName;
                }
            }
        }

        return title;
    }

    getFullScreenClassName(index) {
        if (index === this.state.fullScreenIndex) {
            return " " + content.full;
        }

        if (this.state.fullScreenIndex > 0) {
            return " " + content.hidden;
        }

        return "";
    }

    displayAlarmNumUI = () => {
        let menu = this.props.menu;
        let num = null;
        let displayAlarmNumUI = [];

        if (menu.indexOf(SDMSResource.ID.menu.alarmCCTV + "_") !== -1) {
            num = menu.replace(SDMSResource.ID.menu.alarmCCTV + "_", "");
        }

        if (num !== null) {
            displayAlarmNumUI.push(
                <span key={"cctvInfo_" + num} className={content.cctvAct}>{num}</span>
            );
        }

        return displayAlarmNumUI;
    }

    displayAlarmClass() {
        const alarm = this.props.alarmInfo;
        let displayAlarmClass = "cctvAlarmPopup";

        if (alarm !== null && alarm !== undefined)
            displayAlarmClass = displayAlarmClass + " cctvAlarm_" + alarm[1].sensorZoneHistoryID;

        return displayAlarmClass;
    }

    setVisibleCCTVPopup = () => {
        // 모드에 따른 닫기 기능
        if (this.props.editMode === Contents3D.Edit_Mode_None) {
            this.props.setVisiblePopups(this.props.menu, false);
        } else {
            this.props.setEditModeCCTV(!this.props.editModeCCTV);
        }
    }

    render() {
        const title = this.getTitle();

        // 이벤트 숫자
        // .TODO: 고도화 내용으로 인한 주석처리 
        //const displayAlarmNumUI = this.displayAlarmNumUI();
        //const displayAlarmClass = this.displayAlarmClass();
        const displayAlarmNumUI = [];
        const displayAlarmClass = "cctvAlarmPopup";

        return (
            <div id={this.props.popupType} className={content.viewDashboardBoxD + ' ' + content.viewDashboardCCTV}>
                <div className={content.popupSizingAreaRight} onMouseDown={(e) => this.popupResizeMousePress(e, 'h-r')} ></div>
                <div className={content.popupSizingAreaLeft} onMouseDown={(e) => this.popupResizeMousePress(e, 'h-l')}></div>
                <div className={content.popupSizingAreaBottom} onMouseDown={(e) => this.popupResizeMousePress(e, 'v-b')}></div>
                <div className={content.popupSizingAreaTop} onMouseDown={(e) => this.popupResizeMousePress(e, 'v-t')}></div>
                <div className={content.popupSizingAreaRightTopPoint} onMouseDown={(e) => this.popupResizeMousePress(e, 'd-rt')}></div>
                <div className={content.popupSizingAreaRightBottomPoint} onMouseDown={(e) => this.popupResizeMousePress(e, 'd-rb')}></div>
                <div className={content.popupSizingAreaLeftTopPoint} onMouseDown={(e) => this.popupResizeMousePress(e, 'd-lt')}></div>
                <div className={content.popupSizingAreaLeftBottomPoint} onMouseDown={(e) => this.popupResizeMousePress(e, 'd-lb')}></div>

                {/*<div className={content.resetBtn} onClick={this.onClickReset} ><a><img src={imgReset} alt="새로고침" /></a></div>*/}
                <div className={content.dslTop + " " + content.dslGrd + " " + displayAlarmClass}>
                {/*<div className={content.dslTop + " " + content.dslGrd}>*/}
                    { displayAlarmNumUI }  
                    <h5 className={content.dslTitle} onMouseDown={(e) => this.popupDragMousePress(e)}>
                        {title}
                    </h5>
                    <a id="cctvInfoCloseBtn" className={content.dslX} onClick={() => this.setVisibleCCTVPopup()}></a>
                </div>
                <div className={content.viewDashboardCCTVConts}>
                    { /*
                        <ul className={content.viewTab}>

                            <li><a className={content.viewTabOn}>CCTV</a></li>
                            //<li><a>이동형 장비</a></li>

                        </ul>
                    */ }
                    <div>
                        <div className={content.viewDashboardCCTVview}>
                            <div className={content.viewDashboardCCTVGrid}>
                                <div className={content.col1row1 + this.getFullScreenClassName(1)}>
                                    <span id="cctv1_span" onDoubleClick={(e) => this.showFullScreenCCTV(1)} onClick={() => this.onClickCCTV(0)}>
                                        <p ref={this.refCCTV1Title} id="cctv1_name"></p>
                                        <iframe id="cctv1" allowtransparency="yes" scrolling="no"></iframe>
                                    </span>
                                </div>
                                <div className={content.col2row1 + this.getFullScreenClassName(2)}>
                                    <span id="cctv2_span" onDoubleClick={(e) => this.showFullScreenCCTV(2)} onClick={() => this.onClickCCTV(1)}>
                                        <p ref={this.refCCTV2Title} id="cctv2_name"></p>
                                        <iframe id="cctv2" allowtransparency="yes" scrolling="no"></iframe>
                                    </span>
                                </div>
                                <div className={content.col1row2 + this.getFullScreenClassName(3)}>
                                    <span id="cctv3_span" onDoubleClick={(e) => this.showFullScreenCCTV(3)} onClick={() => this.onClickCCTV(2)}>
                                        <p ref={this.refCCTV3Title} id="cctv3_name"></p>
                                        <iframe id="cctv3" allowtransparency="yes" scrolling="no"></iframe>
                                    </span>
                                </div>
                                <div className={content.col2row2 + this.getFullScreenClassName(4)}>
                                    <span id="cctv4_span" onDoubleClick={(e) => this.showFullScreenCCTV(4)} onClick={() => this.onClickCCTV(3)}>
                                        <p ref={this.refCCTV4Title} id="cctv4_name"></p>
                                        <iframe id="cctv4" allowtransparency="yes" scrolling="no"></iframe>
                                    </span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}

export default CCTVInfo;