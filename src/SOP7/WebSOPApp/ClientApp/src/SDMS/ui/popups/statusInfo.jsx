import { ui } from 'jquery';
import React, { Component } from 'react';
import content from '../../../Common/css/content.module.css';
import uis from '../../../Common/css/ui.module.css';
import imgClose from '../../../Common/image/icon/close_x.png';
import imgZoomIco from '../../../Common/image/icon/zoom_ico.png';
import SDMS from '../sdms';
import StatusInfoBuildingGroup from './statusInfoBuildingGroup';
import SDMSMainMenu from '../sdmsMainMenu';
import commonStyles from '../../../Common/css/style.module.css';
import sdmsStyles from '../../css/sdms.module.css';
import $ from 'jquery';
import { array } from '@amcharts/amcharts4/core';
import { SDMSController } from '../../services/sdmsController';
import SettingsStore from '../../../Settings/settingsStore';
import { Scrollbars } from 'react-custom-scrollbars-2';
import { SdmsScrollbar } from './SdmsScrollbar';
import Contents3D from '../3D/contents3D';
import { faCameraRetro } from '@fortawesome/free-solid-svg-icons';
    
class StatusInfo extends Component {
    constructor(props) {
        super(props);
        this.state = {
            popupMinWidth: 320,
            popupMinHeight: 500,
            searchText: ''
        }

        let updateScrollTop = false;
        let scrollTop = 0;

        this.initPopupState = this.initPopupState.bind(this);
        SettingsStore.subscribe(function () {
            this.resetPopupState(SettingsStore.getState());
        }.bind(this));

        this.refLayer = React.createRef();
        this.refScrollArea = React.createRef();
        this.refScrollbar = React.createRef();
        this.refTree = React.createRef();
    }

    componentDidMount() {        
        /*$(function () {
            $("." + content.tabcontent).hide();
            $("." + content.tabcontent + ':first').show();

            $('ul.' + content.tabs + ' li').click(function () {
                $('ul.' + content.tabs + ' li').removeClass(content.active).css("color", "#fff");
                //$(this).addClass("active").css({"color": "darkred","font-weight": "bolder"});
                $(this).addClass(content.active).css("color", "#fff");
                $("." + content.tabcontent).hide()
                var activeTab = $(this).attr("rel");
                $("#" + activeTab).fadeIn()
            });
        })*/

        // 팝업 마우스 드래그 이벤트 리스너
        this.popupDragMouseMove = (event) => {
            var mousePosition = {
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
                    let lim = ((this.state.maxScreenWidth - width) / this.state.maxScreenWidth) * 100;
                    this.state.popup.style.left = lim + '%';
                } else if (this.state.preMousePosition.x > mousePosition.x) {
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
                    //상단 툴바는 항상 높이 60 고정이기 때문에 현재 화면 사이즈에서 60px의 비율을 계산한다.
                    let lim = (60 / this.state.maxScreenHeight) * 100;
                    this.state.popup.style.top = lim + '%';
                } else if (this.state.preMousePosition.y < mousePosition.y) {
                    //화면 위쪽으로 팝업이 이미 벗어나 있을 때
                    this.state.popup.style.top = perMoveY + '%';
                }
            }
        }

        //팝업 리사이즈 이벤트 리스너
        this.popupResizeMouseMove = (event) => {
            let sizeX = 0;
            let sizeY = 0;

            switch (this.state.resizeType) {
                // 수평
                case 'h-r': // 오른쪽 수평
                    sizeX = event.pageX - this.state.originalX;

                    if (event.pageX > 0 && event.pageX < this.state.maxScreenWidth && sizeX >= this.state.popupMinWidth) {
                        this.state.popup.style.width = sizeX + 'px';
                    }
                    break;
                case 'h-l': //왼쪽 수평
                    sizeX = this.state.originalWidth - (event.pageX - this.state.originalMouseX);

                    if (event.pageX > 0 && event.pageX < this.state.maxScreenWidth && sizeX > this.state.popupMinWidth) {
                        this.state.popup.style.width = sizeX + 'px';

                        let pxLeft = (this.state.originalX + (event.pageX - this.state.originalMouseX));
                        this.state.popup.style.left = ((pxLeft / this.state.maxScreenWidth) * 100) + '%';
                    }
                    break;
                // 수직
                case 'v-b': // 바텀 수직
                    sizeY = event.pageY - this.state.originalY;

                    if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && sizeY > this.state.popupMinHeight) {
                        this.state.popup.style.height = sizeY + 'px';

                        // 팝업 내 스크롤이 적용된 태그 사이즈 재조정
                        this.setStatusInfoTabContent(sizeY);
                    }
                    break;
                case 'v-t': //탑 수직
                    sizeY = this.state.originalHeight - (event.pageY - this.state.originalMouseY);

                    if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && sizeY > this.state.popupMinHeight) {
                        this.state.popup.style.height = sizeY + 'px';

                        let pxTop = this.state.originalY + (event.pageY - this.state.originalMouseY);
                        this.state.popup.style.top = ((pxTop / this.state.maxScreenHeight) * 100) + '%';

                        // 팝업 내 스크롤이 적용된 태그 사이즈 재조정
                        this.setStatusInfoTabContent(sizeY);
                    }
                    break;
                // 대각
                case 'd-rb': // 오른쪽 하단 대각
                    sizeX = event.pageX - this.state.originalX;
                    sizeY = event.pageY - this.state.originalY;

                    if (event.pageX > 0 && event.pageX < this.state.maxScreenWidth && sizeX > this.state.popupMinWidth) {
                        this.state.popup.style.width = sizeX + 'px';
                    }

                    if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && sizeY > this.state.popupMinHeight) {
                        this.state.popup.style.height = sizeY + 'px';

                        // 팝업 내 스크롤이 적용된 태그 사이즈 재조정
                        this.setStatusInfoTabContent(sizeY);
                    }
                    break;
                case 'd-rt': //오른쪽 상단 대각
                    sizeX = this.state.originalWidth + (event.pageX - this.state.originalMouseX);
                    sizeY = this.state.originalHeight - (event.pageY - this.state.originalMouseY);

                    if (event.pageX > 0 && event.pageX < this.state.maxScreenWidth && sizeX > this.state.popupMinWidth) {
                        this.state.popup.style.width = sizeX + 'px';
                    }

                    if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && sizeY > this.state.popupMinHeight) {
                        this.state.popup.style.height = sizeY + 'px';

                        let pxTop = this.state.originalY + (event.pageY - this.state.originalMouseY);
                        this.state.popup.style.top = ((pxTop / this.state.maxScreenHeight) * 100) + '%';

                        // 팝업 내 스크롤이 적용된 태그 사이즈 재조정
                        this.setStatusInfoTabContent(sizeY);
                    }
                    break;
                case 'd-lb': //왼쪽 하단 대각
                    sizeX = this.state.originalWidth - (event.pageX - this.state.originalMouseX);
                    sizeY = event.pageY - this.state.originalY;

                    if (event.pageX > 0 && event.pageX < this.state.maxScreenWidth && sizeX > this.state.popupMinWidth) {
                        this.state.popup.style.width = sizeX + 'px';

                        let pxLeft = (this.state.originalX + (event.pageX - this.state.originalMouseX));
                        this.state.popup.style.left = ((pxLeft / this.state.maxScreenWidth) * 100) + '%';
                    }

                    if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && sizeY > this.state.popupMinHeight) {
                        this.state.popup.style.height = sizeY + 'px';

                        // 팝업 내 스크롤이 적용된 태크 사이즈 재조정
                        this.setStatusInfoTabContent(sizeY);
                    }
                    break;
                case 'd-lt': //왼쪽 상단 대각
                    sizeX = this.state.originalWidth - (event.pageX - this.state.originalMouseX);
                    sizeY = this.state.originalHeight - (event.pageY - this.state.originalMouseY);

                    if (event.pageX > 0 && event.pageX < this.state.maxScreenWidth && sizeX > this.state.popupMinWidth) {
                        this.state.popup.style.width = sizeX + 'px';

                        let pxLeft = (this.state.originalX + (event.pageX - this.state.originalMouseX));
                        this.state.popup.style.left = ((pxLeft / this.state.maxScreenWidth) * 100) + '%';
                    }

                    if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && sizeY > this.state.popupMinHeight) {
                        this.state.popup.style.height = sizeY + 'px';

                        let pxTop = this.state.originalY + (event.pageY - this.state.originalMouseY);
                        this.state.popup.style.top = ((pxTop / this.state.maxScreenHeight) * 100) + '%';

                        // 팝업 내 스크롤이 적용된 태크 사이즈 재조정
                        this.setStatusInfoTabContent(sizeY);
                    }
                    break;
                default:
            }

        }

        this.initPopupState();
        this.setScrollbar();

        $('.' + sdmsStyles.scrollbar).scrollTop(0);

        this.props.setActiveDragPopup(this.props.popupType);
    }

    setScrollbar() {
        const treeArea = this.refScrollArea.current.getBoundingClientRect();

        let scrollVisible = false;

        if (this.refTree.current) {
            const rectTree = this.refTree.current.getBoundingClientRect();

            if (rectTree.height > treeArea.height) {
                scrollVisible = true;
            }
        }

        SdmsScrollbar.setContentStyle(this.refScrollbar.current, treeArea.width, treeArea.height, scrollVisible);

        const treeArea2 = this.refScrollArea.current.getBoundingClientRect();

        if (this.props.selectedInfo && this.props.selectedInfo.buildingGroup) {
            if (this.props.selectedInfo.sensorGroups) {

                const [sensorType, zoneID, sensorID] = this.props.selectedSensor;
                if (sensorType === SDMSMainMenu.Fire_Sensor && this.props.selectedInfo.fireSensors) {
                    const ele = document.getElementById('fireSensor_' + sensorID);
                    if (!ele) {
                        return;
                    }
                    const rect = ele.getBoundingClientRect();

                    const beginY = treeArea2.y;
                    const endY = treeArea2.y + treeArea2.height;
                    if (rect.top >= beginY && rect.bottom <= endY) {
                        // 범위내에 있음
                        return;
                    }

                    const temp = document.getElementById('fireSensor_' + sensorID);
                    temp.scrollIntoView({ "behavior": "smooth", "block": "center" });
                }
                else if (sensorType === SDMSMainMenu.PSM_Sensor && this.props.selectedInfo.psmSensors) {
                    const ele = document.getElementById('psmSensor_' + sensorID);
                    if (!ele) {
                        return;
                    }
                    const rect = ele.getBoundingClientRect();

                    const beginY = treeArea2.y;
                    const endY = treeArea2.y + treeArea2.height;
                    if (rect.top >= beginY && rect.bottom <= endY) {
                        // 범위내에 있음
                        return;
                    }

                    const temp = document.getElementById('psmSensor_' + sensorID);
                    temp.scrollIntoView({ "behavior": "smooth", "block": "center" });
                }
                else if (sensorType === SDMSMainMenu.Etc_Sensor && this.props.selectedInfo.etcSensors) {
                    const ele = document.getElementById('etcSensor_' + sensorID);
                    if (!ele) {
                        return;
                    }

                    const rect = ele.getBoundingClientRect();

                    const beginY = treeArea2.y;
                    const endY = treeArea2.y + treeArea2.height;
                    if (rect.top >= beginY && rect.bottom <= endY) {
                        // 범위내에 있음
                        return;
                    }

                    const temp = document.getElementById('etcSensor_' + sensorID);
                    temp.scrollIntoView({ "behavior": "smooth", "block": "center" });
                }
                else {
                    const zone = this.props.selectedInfo.zone;
                    if (!zone || !zone.id) {
                        return;
                    }

                    const isMove = this.checkRange('sensorGroups_' + zone.id, 'sensorGroupsArea_' + zone.id, treeArea2);
                    if (!isMove) {
                        return;
                    }

                    const temp = document.getElementById('zone_' + zone.id);
                    temp.scrollIntoView({ "behavior": "smooth" });
                }
            }
            else if (this.props.selectedInfo.cctvGroups)
            {
                const [sensorType, zoneID, sensorID] = this.props.selectedSensor;
                if (sensorType === SDMSMainMenu.CCTV_Type && this.props.selectedInfo.cctvSubGroups) {

                    const ele = document.getElementById('cctv_' + sensorID);
                    if (!ele) {
                        return;
                    }

                    const rect = ele.getBoundingClientRect();

                    const beginY = treeArea2.y;
                    const endY = treeArea2.y + treeArea2.height;
                    if (rect.top >= beginY && rect.bottom <= endY) {
                        // 범위내에 있음
                        return;
                    }

                    const temp = document.getElementById('cctv_' + sensorID);
                    temp.scrollIntoView({ "behavior": "smooth", "block": "center" });
                }
                else {
                    const zone = this.props.selectedInfo.zone;
                    if (!zone || !zone.id) {
                        return;
                    }

                    const isMove = this.checkRange('cctvGroups_' + zone.id, 'cctvGroupsArea_' + zone.id, treeArea2);
                    if (!isMove) {
                        return;
                    }

                    const temp = document.getElementById('zone_' + zone.id);
                    temp.scrollIntoView({ "behavior": "smooth" });
                }
            }
            else if (this.props.selectedInfo.facilityGroups) {
                const facility = this.props.selectedFacility;
                if (facility.facilityID >= 1 && this.props.selectedInfo.facilitySubGroups) {

                    const ele = document.getElementById('facilityInfo_' + facility.facilityID);
                    if (!ele) {
                        return;
                    }
                    const rect = ele.getBoundingClientRect();

                    const beginY = treeArea2.y;
                    const endY = treeArea2.y + treeArea2.height;
                    if (rect.top >= beginY && rect.bottom <= endY) {
                        // 범위내에 있음
                        return;
                    }

                    const temp = document.getElementById('facilityInfo_' + facility.facilityID);
                    temp.scrollIntoView({ "behavior": "smooth", "block": "center" });
                }
                else {
                    const zone = this.props.selectedInfo.zone;
                    if (!zone || !zone.id) {
                        return;
                    }

                    const isMove = this.checkRange('facilityGroups_' + zone.id, 'facilityGroupsArea_' + zone.id, treeArea2);
                    if (!isMove) {
                        return;
                    }

                    const temp = document.getElementById('zone_' + zone.id);
                    temp.scrollIntoView({ "behavior": "smooth" });
                }
            }
            else if (this.props.selectedInfo.zone) {
                const zone = this.props.selectedInfo.zone;
                if (!zone || !zone.id) {
                    return;
                }

                const isMove = this.checkRange('zone_' + zone.id, 'zoneArea_' + zone.id, treeArea2);
                if (!isMove) {
                    return;
                }

                const building = this.props.selectedInfo.building;
                const temp = document.getElementById('building_' + building.id);
                if (temp) {
                    temp.scrollIntoView({ "behavior": "smooth" });
                }
            }
            else if (this.props.selectedInfo.building) {
                const building = this.props.selectedInfo.building;
                if (!building || !building.id) {
                    return;
                }

                const isMove = this.checkRange('building_' + building.id, 'buildingArea_' + building.id, treeArea2);
                if (!isMove) {
                    return;
                }

                const temp = document.getElementById('building_' + building.id);
                temp.scrollIntoView({ "behavior": "smooth" });
            }
            else {
                const buildingGroup = this.props.selectedInfo.buildingGroup;
                if (!buildingGroup || !buildingGroup.id) {
                    return;
                }

                const temp = document.getElementById('buildingGroup_' + buildingGroup.id);
                temp.scrollIntoView({ "behavior": "smooth" });
            }
        }
    }

    // node가 보이는 범위내에 있는지 체크한다
    checkRange(titleID, areaID, targetRect) {
        const titleEle = document.getElementById(titleID);
        const areaEle = document.getElementById(areaID);
        if (!titleEle || !areaEle) {
            return false;
        }        

        const titleRect = titleEle.getBoundingClientRect();
        const areaRect = areaEle.getBoundingClientRect();

        const beginY = targetRect.y;
        const endY = targetRect.y + targetRect.height;
        if (titleRect.top >= beginY && areaRect.bottom <= endY) {
            // 범위내에 있음
            return false;
        }

        return true;
    }
    
    componentDidUpdate(prevProps, prevState) {



        // 팝업이 선택 됐을 때(Drag 될때) 맨 앞에 팝업 위치
        if (this.props.zIndex !== prevProps.zIndex) {
            this.state.popup.style.zIndex = this.props.zIndex;
            console.log('statusInfoZIndex changed', this.state.popup.style.zIndex)
        }

        this.setScrollbar();
    }

    setVisiblePoi(typeName, visible) {
        this.props.setVisiblePoi(typeName, visible);
    }
    
    initPopupState() {
        var popup = document.getElementsByClassName(content.viewDashboard + ' ' + content.viewDashboardBoxD)[0];

        //DB에 값이 있을 경우에만
        if (typeof this.props.popupState !== 'undefined') {
            popup.style.left = this.props.popupState.x;
            popup.style.top = this.props.popupState.y;
            popup.style.width = this.props.popupState.width;
            popup.style.height = this.props.popupState.height;
        }

        //팝업 내 스크롤 컨텐츠 사이즈 초기화
        /*var viewScroll = document.getElementsByClassName('statusInfoTabContent');
        var viewScrollSize = parseFloat(getComputedStyle(popup, null).getPropertyValue('height').replace('px', '')) - 160;
        // 340 - 500
        for (var i = 0; i < viewScroll.length; i++) {
            viewScroll[i].style.height = viewScrollSize + 'px';
        }*/

        this.setState({ popup: popup });
    }

    repositionPopup(popupState) {
        let data = popupState.statusInfo;

        if (data === null || data === undefined)
            return;

        let popup = document.getElementsByClassName(content.viewDashboard + ' ' + content.viewDashboardBoxD)[0];
        if (popup === null || popup === undefined)
            return;

        popup.style.left = data.x;
        popup.style.top = data.y;
        popup.style.width = data.width;
        popup.style.height = data.height;

        //팝업 내 스크롤 컨텐츠 사이즈 초기화
        /*let viewScroll = document.getElementsByClassName('statusInfoTabContent');
        let viewScrollSize = parseFloat(getComputedStyle(popup, null).getPropertyValue('height').replace('px', '')) - 160;
        // 340 - 500
        for (var i = 0; i < viewScroll.length; i++) {
            viewScroll[i].style.height = viewScrollSize + 'px';
        }*/

        this.setState({ popup: popup });
    }

    resetPopupState = (popupState) => {
        let data = popupState;

        if (data.actionType === 'RESET_POPUP') {
            this.repositionPopup(data.popupState);
        }
    }

    //사이즈 드래그 할 때마다 Scroll적용된 컨텐츠 사이즈 조절(CSS에서 조절이 어려워 스크립트로 대체)
    setStatusInfoTabContent(sizeY) {
        /*var viewScroll = document.getElementsByClassName('statusInfoTabContent');
        var viewScrollSize = sizeY - 160;
        // 340 - 500
        for (var i = 0; i < viewScroll.length; i++) {
            viewScroll[i].style.height = viewScrollSize + 'px';
        }*/
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
            this.props.setActiveDragPopup(this.props.popupType);
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
            originalY: this.state.popup.getBoundingClientRect().top
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
            var popupState = {
                // popupState값이 없다면 id값  -1 대입
                id: typeof this.props.popupState !== 'undefined' ? this.props.popupState.id : -1,
                x: perX + '%',
                y: perY + '%',
                height: height + 'px',
                width: width + 'px'
            }
            this.props.setPopupState(this.props.popupType, popupState);
        }
    }

    searchEnterKey = () => {
        if (window.event && window.event.keyCode === 13) {            
            this.search();
        }
    }

    search = () => {
        const text = document.getElementById('txtSearch').value;
        this.setState({ searchText: text });
    }

    getBuildingGroupUI() {
        let ui = [];
        let buildingGroupList = this.props.buildingGroupList;
        if (buildingGroupList === undefined || buildingGroupList === null || buildingGroupList.length === 0)
            return ui;

        if (this.state.searchText.length > 0) {
            this.setVisibleBuildingGroupList(buildingGroupList);
        }

        for (var i = 0; i < buildingGroupList.length; i++) {
            const buildingGroup = buildingGroupList[i];
            if (buildingGroup.visible === false && this.state.searchText.length > 0)
                continue;

            ui.push(<StatusInfoBuildingGroup
                id={'buildingGroup_' + buildingGroup.id}
                key={buildingGroup.id}
                buildingGroup={buildingGroup}
                zoneList={this.props.zoneList}
                buildingIDs={this.props.buildingIDs}
                indoorModels={this.props.indoorModels}
                sensorList={this.props.sensorList}
                moveToX={this.props.moveToX}
                onSelectSensor={this.props.onSelectSensor}
                selectedSensor={this.props.selectedSensor}
                selectedFacility={this.props.selectedFacility}
                getFacilityID={this.props.getFacilityID}
                sensorAlarms={this.props.sensorAlarms}
                searchText={this.state.searchText}
                facilityInfos={this.props.facilityInfos}
                isEditMode={false}
                multiSite={this.props.multiSite}
                selectedInfo={this.props.selectedInfo}
                onChangeBuildingGroup={this.onChangeBuildingGroup}
            />);
        }

        if (this.props.outdoorZones) {
            ui.push(<StatusInfoBuildingGroup
                id={'buildingGroup_outdoor'}
                key={"bg_outdoor"}
                isOutdoor={true}
                buildingGroup={this.props.outdoorZones}
                zoneList={this.props.zoneList}
                buildingIDs={this.props.buildingIDs}
                indoorModels={this.props.indoorModels}
                sensorList={this.props.sensorList}
                moveToX={this.props.moveToX}
                onSelectSensor={this.props.onSelectSensor}
                selectedSensor={this.props.selectedSensor}
                selectedFacility={this.props.selectedFacility}
                getFacilityID={this.props.getFacilityID}
                sensorAlarms={this.props.sensorAlarms}
                searchText={this.state.searchText}
                facilityInfos={this.props.facilityInfos}
                isEditMode={false}
                multiSite={this.props.multiSite}
                selectedInfo={this.props.selectedInfo}
                onChangeBuildingGroup={this.onChangeBuildingGroup}
            />);
        }

        return ui;
    }

    setVisibleBuildingGroupList(buildingGroupList) {        
        let count = buildingGroupList.length;
        for (let i = 0; i < count; i++) {
            const buildingGroup = buildingGroupList[i];
            if (buildingGroup.displayText.includes(this.state.searchText)) {
                buildingGroup.visible = true;
            }
            else {
                let visible = false;
                for (let i = 0; i < buildingGroup.buildingDatas.length; i++) {
                    const buildingVisible = this.setVisibleBuildingList(buildingGroup.buildingDatas[i]);
                    if (buildingVisible === true) {
                        visible = true;
                    }
                }

                if (visible) {
                    buildingGroup.visible = true;
                }
                else {
                    buildingGroup.visible = false;
                }
            }
        }
    }

    setVisibleBuildingList(buildingData) {
        let buildingVisible = false;

        let count = buildingData.zoneDatas.length;
        for (let i = 0; i < count; i++) {
            const zone = buildingData.zoneDatas[i];
            if (zone.displayText.includes(this.state.searchText)) {
                zone.visible = true;
                buildingVisible = true;
            }
            else {
                let visibleCount = 0;
                if (this.setVisibleSensors(zone.id, this.props.sensorList.fireSensors)) {
                    visibleCount++;
                }
                if (this.setVisibleSensors(zone.id, this.props.sensorList.etcSensors)) {
                    visibleCount++;
                }
                if (this.setVisibleSensors(zone.id, this.props.sensorList.cctvs)) {
                    visibleCount++;
                }
                if (this.setVisiblePsmSensors(zone.id, this.props.sensorList.psmSensors)) {
                    visibleCount++;
                }
                if (this.setVisibleFacilityInfos(zone.id, this.props.facilityInfos)) {
                    visibleCount++;
                }

                if (visibleCount > 0) {
                    zone.visible = true;
                }
                else {
                    zone.visible = false;
                }

                if (zone.visible) {
                    buildingVisible = true;
                }
            }
        }

        buildingData.visible = buildingVisible;

        return buildingData.visible;
    }

    setVisibleSensors(zoneID, sensors) {
        let visible2 = false;
        const sensorsCount = sensors.length;
        for (let j = 0; j < sensorsCount; j++) {
            const sensor = sensors[j];
            if (zoneID !== sensor.zoneID)
                continue;

            if (sensor.name.includes(this.state.searchText)) {
                sensor.visible = true;
                visible2 = true;
            }
            else {
                sensor.visible = false;
            }
        }

        return visible2;
    }

    setVisiblePsmSensors(zoneID, sensors) {
        let visible2 = false;
        const sensorsCount = sensors.length;
        for (let i = 0; i < sensorsCount; i++) {
            const sensor = sensors[i];
            if (!sensor.linkedZones)
                continue;

            for (var j = 0; j < sensor.linkedZones.length; j++) {
                if (sensor.linkedZones[j].id !== zoneID)
                    continue;

                if (sensor.name.includes(this.state.searchText)) {
                    sensor.visible = true;
                    visible2 = true;
                }
                else {
                    sensor.visible = false;
                }
                break;
            }
        }
        return visible2;
    }

    onChangeVisible(sensorType) {
        this.props.setVisiblePoi(sensorType, !this.props.visibleSensorTypes[sensorType]);
    }

    onClickLayer = (event) => {
        const on = content.on;

        if (event.target.classList.contains(on)) {
            event.target.classList.remove(on);
            this.refLayer.current.classList.remove(on);
        }
        else {
            event.target.classList.add(on);
            this.refLayer.current.classList.add(on);
        }
    }

    setVisibleFacilityInfos(zoneID, infos) {
        let visible2 = false;
        const infosCount = infos.length;
        for (let j = 0; j < infosCount; j++) {
            const info = infos[j];
            if (zoneID !== info.zoneID)
                continue;

            if (info.facilityName.includes(this.state.searchText)) {
                info.visible = true;
                visible2 = true;
            }
            else {
                info.visible = false;
            }
        }

        return visible2;
    }

    getCCTVNotify() {
        if (this.props.newCCTVList.length > 0) {
            return (
                <div className={content.dsToast}>
                    <p>
                        CCTV 위치지정이 필요합니다.
                            <br />
                        <a className={commonStyles.clickable} onClick={() => this.props.setEditModeItem(Contents3D.Edit_Mode_MovePOI, null)}>&lt;편집모드&gt;</a>
                            에서 수정하실 수 있습니다.
                        </p>
                </div>
            );
        }

        return <></>
    }

    onChangeBuildingGroup = (value, type) => {
        this.props.onChangeBuildingGroup(value, type);
    }

    render() {
        const buildingGroupUI = this.getBuildingGroupUI();

        let visibleFirePOI = this.props.visibleSensorTypes[SDMSMainMenu.Fire_Sensor] ? true : false;        
        let visibleCctvPOI = this.props.visibleSensorTypes[SDMSMainMenu.CCTV_Type] ? true : false;
        let visiblePSMPOI = this.props.visibleSensorTypes[SDMSMainMenu.PSM_Sensor] ? true : false;
        let visibleEtcPOI = this.props.visibleSensorTypes[SDMSMainMenu.Etc_Sensor] ? true : false;
        let visibleEquipZoneName = this.props.visibleSensorTypes[SDMSMainMenu.EquipZoneName] ? true : false;

        let visibleFireClassName = (visibleFirePOI) ? content.visibleFire : content.disableFire;
        let visibleCctvClassName = (visibleCctvPOI) ? content.visibleCCTV : content.disableCCTV;
        let visiblePSMClassName = (visiblePSMPOI) ? content.visiblePsm : content.disablePsm;
        let visibleEtcClassName = (visibleEtcPOI) ? content.visibleEtc : content.disableEtc;
        let visibleEquipZoneNameClassName = (visibleEquipZoneName) ? content.visibleEquip : content.disableEquip;

        return (
            <div id={this.props.popupType} className={content.viewDashboard + ' ' + content.viewDashboardBoxD}>
                <div className={content.popupSizingAreaRight} onMouseDown={(e) => this.popupResizeMousePress(e, 'h-r')} ></div>
                <div className={content.popupSizingAreaLeft} onMouseDown={(e) => this.popupResizeMousePress(e, 'h-l')}></div>
                <div className={content.popupSizingAreaBottom} onMouseDown={(e) => this.popupResizeMousePress(e, 'v-b')}></div>
                <div className={content.popupSizingAreaTop} onMouseDown={(e) => this.popupResizeMousePress(e, 'v-t')}></div>
                <div className={content.popupSizingAreaRightTopPoint} onMouseDown={(e) => this.popupResizeMousePress(e, 'd-rt')}></div>
                <div className={content.popupSizingAreaRightBottomPoint} onMouseDown={(e) => this.popupResizeMousePress(e, 'd-rb')}></div>
                <div className={content.popupSizingAreaLeftTopPoint} onMouseDown={(e) => this.popupResizeMousePress(e, 'd-lt')}></div>
                <div className={content.popupSizingAreaLeftBottomPoint} onMouseDown={(e) => this.popupResizeMousePress(e, 'd-lb')}></div>

                <div className={content.dslTop + " " + content.dslGrd}>
                    <h5 className={content.dslTitle} onMouseDown={(e) => this.popupDragMousePress(e)}>
                        현황정보
                    </h5>
                    <a className={content.dslX} onClick={() => this.props.setVisiblePopups(SDMS.menu.statusInfo, false)}></a>
                </div>

                <div className={content.dslCont}>
                    {
                        this.getCCTVNotify()
                    }
                    <div className={content.dsiSel}>
                        <div onClick={this.onClickLayer}>
                            <ul ref={this.refLayer}>
                                <li><label ref={this.refCheckEquipZone} className={visibleFireClassName} title="화재센서" ><input type="checkbox" checked={visibleFirePOI} onChange={() => this.onChangeVisible(SDMSMainMenu.Fire_Sensor)} /></label></li>
                                <li><label ref={this.refCheckEquipZone} className={visiblePSMClassName} title="누출센서" ><input type="checkbox" checked={visiblePSMPOI} onChange={() => this.onChangeVisible(SDMSMainMenu.PSM_Sensor)} /></label></li>
                                <li><label ref={this.refCheckEquipZone} className={visibleEtcClassName} title="IoT센서" ><input type="checkbox" checked={visibleEtcPOI} onChange={() => this.onChangeVisible(SDMSMainMenu.Etc_Sensor)} /></label></li>
                                <li><label ref={this.refCheckEquipZone} className={visibleCctvClassName} title="CCTV" ><input type="checkbox" checked={visibleCctvPOI} onChange={() => this.onChangeVisible(SDMSMainMenu.CCTV_Type)} /></label></li>
                                <li><label ref={this.refCheckEquipZone} className={visibleEquipZoneNameClassName} title="공간정보 명칭"  ><input type="checkbox" checked={visibleEquipZoneName} onChange={() => this.onChangeVisible(SDMSMainMenu.EquipZoneName)} /></label></li>
                            </ul> 
                           {/* <ul ref={this.refLayer}>
                            <li>
                                <label ref={this.refCheckEquipZone} className={commonStyles.clickable}>
                                    <input type="checkbox" className={sdmsStyles.labelInput} checked={visibleCctvPOI} onChange={() => this.onChangeVisible(SDMSMainMenu.CCTV_Type)} />
						            CCTV
					            </label>
                            </li>
                            <li>
                                <label ref={this.refCheckEquipZone} className={commonStyles.clickable}>
                                    <input type="checkbox" className={sdmsStyles.labelInput} checked={visibleFirePOI} onChange={() => this.onChangeVisible(SDMSMainMenu.Fire_Sensor)} />
						            화재센서
					            </label>
                            </li>
                            <li>
                                <label ref={this.refCheckEquipZone} className={commonStyles.clickable}>
                                    <input type="checkbox" className={sdmsStyles.labelInput} checked={visibleEtcPOI} onChange={() => this.onChangeVisible(SDMSMainMenu.Etc_Sensor)} />
						            IoT센서
					            </label>
                            </li>
                            <li>
                                <label ref={this.refCheckEquipZone} className={commonStyles.clickable}>
                                    <input type="checkbox" className={sdmsStyles.labelInput} checked={visiblePSMPOI} onChange={() => this.onChangeVisible(SDMSMainMenu.PSM_Sensor)} />
						            누출센서
					            </label>
                            </li>
                            <li>
                                <label ref={this.refCheckEquipZone} className={commonStyles.clickable}>
                                    <input type="checkbox" className={sdmsStyles.labelInput} checked={visibleEquipZoneName} onChange={() => this.onChangeVisible(SDMSMainMenu.EquipZoneName)} />
						            공간정보 명칭
					            </label>
                            </li>
                        </ul> */}
                        </div>
                    </div>
                    <div className={content.dsiSch}>
                            <input type="text" id="txtSearch" onKeyUp={this.searchEnterKey} />
                            <a onClick={this.search}>검색</a>
                    </div>
                    <div ref={this.refScrollArea} className={content.dsiScr + " " + sdmsStyles.scrollbar}>
                        {
                                <ul ref={this.refTree} className={content.dsiTree}>
                                    {buildingGroupUI}
                                </ul>
                        }
				    </div>
                </div>
                {
                    /*<div className={content.viewDashboardConts}>
                        <ul className={content.tabs}>
                            <li className={content.active} rel="tab1">건물별 분류</li>
                            <li rel="tab2">센서별 분류</li>
                        </ul>
                        <div className={content.tabcontainer}>
                            <div id="tab1" className={content.tabcontent} style={{ display:'block' }}>
                                <div className={uis.clfix + ' ' + content.posiRelative}>
                                    <input type="text" id="txtSearch" className={content.viewInput} onKeyUp={this.searchEnterKey} />
                                    <button type="button" className={content.viewSearch} onClick={this.search}> <img src={imgZoomIco} alt="검색" /></button>
                                </div>
                                <div className={content.viewScroll + ' statusInfoTabContent'}>
                                    <ul className={content.viewListDo}>
                                        {buildingGroupUI}
                                    </ul>
                                </div>
                            </div>
                            <div id="tab2" className={content.tabcontent}>
                                <div className={uis.clfix + ' ' + content.posiRelative}>
                                    <input type="text" className={content.viewInput} />
                                    <button type="button" className={content.viewSearch}><img src={imgZoomIco} alt="검색" />
                                    </button>
                                </div>
                                <div className={content.viewScroll + ' statusInfoTabContent'}>
                                    <ul className={content.viewListDo}>
                                        <li className={content.posiRelative}>
                                            <div className={content.switchBtn}>
                                                <label className={content.switch}>
                                                    <input type="checkbox" defaultChecked={visibleFirePOI} onChange={() => this.setVisiblePoi(SDMSMainMenu.Fire_Sensor, !visibleFirePOI)} />
                                                    <span className={content.slider + ' ' + content.round}></span>
                                                </label>
                                            </div>
                                            <div className={content.viewListHeadWrap}><span className={content.viewListHead}>화재센서</span></div>                                        
                                        </li>
                                        <li className={content.posiRelative}>
                                            <div className={content.switchBtn}>
                                                <label className={content.switch}>
                                                    <input type="checkbox" defaultChecked={visibleCctvPOI} onChange={() => this.setVisiblePoi(SDMSMainMenu.CCTV_Type, !visibleCctvPOI)} />
                                                    <span className={content.slider + ' ' + content.round}></span>
                                                </label>
                                            </div>
                                            <div className={content.viewListHeadWrap}><span className={content.viewListHead}>CCTV</span></div>
                                        </li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>*/
                }
            </div>                
        );
    }
}

export default StatusInfo;