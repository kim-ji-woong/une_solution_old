import React, { Component } from 'react';
import { BrowserRouter as Route, Link } from 'react-router-dom';
import { withRouter } from 'react-router-dom';
import styles from '../../css/sdms.module.css';
/*import root from '../../Root/css/root.module.css';*/

//import imgClose from '../../image/common_Icon/close_x.png';
import imgClose from '../../image/common_Icon/popup_close.png';
import imgZoomIco from '../../image/status_Icon/aside_ico0402_on.png';
import lightRedICO from '../../image/status_Icon/gray_light_ico.png';
import lightGrayICO from '../../image/status_Icon/red_light_ico.png';
import $ from 'jquery';

import { Scrollbars } from 'react-custom-scrollbars-2';
import { SdmsScrollbar } from './SdmsScrollbar';
import SDMSMainMenu from '../../data/sdmsMainMenu';
import StatusInfoBuildingGroup from './statusInfoBuildingGroup';
import ProjectResource from '../../../Root/resource/id';

class StatusInfo extends Component {

    constructor(props) {
        super(props);

        this.state = {
            popupMinWidth: 320,
            popupMinHeight: 500,
            searchText: ''
        }

        this.props = props;

        this.refScrollArea = React.createRef();
        this.refScrollbar = React.createRef();
        this.refBuildingGroup = React.createRef();
    }

    componentDidMount() {
        $(function () {
            $('.' + styles.arrowDown).on("click", function () {
                $('.' + styles.viewSet + ' ul').slideToggle();
                
            })
        })


        /*$('.viewListHead').click(function () {
            //1뎁스 밑에 있는 하위메뉴 초기화 
            $('.viewListDo .viewListConts').removeClass("on");

            $('.viewList2Depth').removeClass("on");
            $('.viewList3Depth').removeClass("on");
            $('.viewList4Depth').removeClass("on");
            $('.viewList5Depth').removeClass("on");


            $(this).parent().next().addClass("on");

        })
        //두번째 제 1공장~4공장 밑에 1동 ~ 4동 클릭
        $(".viewList1Depth").click(function () {
            $('.viewList2Depth').removeClass("on");
            $('.viewList3Depth').removeClass("on");
            $('.viewList4Depth').removeClass("on");
            $('.viewList5Depth').removeClass("on");
            $(this).next().addClass("on");
        })


        //세번쨰 1층 ~ 2층 클릭
        $(".viewList2DepthHead").click(function () {
            $('.viewList3Depth').removeClass("on");
            $('.viewList4Depth').removeClass("on");
            $('.viewList5Depth').removeClass("on");
            $(this).next().addClass("on");
        })



        // 3뎁스
        $(".viewList3DepthHead").click(function () {
            $('.viewList4Depth').removeClass("on");
            $('.viewList5Depth').removeClass("on");
            $(this).next().addClass("on");
        })


        // 4뎁스 viewList_4Depth 
        $(".viewList4Depth > li").click(function () {
            $('.viewList5Depth').removeClass("on");
            $(this).find(".viewList5Depth").addClass("on");
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
                    }
                    break;
                case 'v-t': //탑 수직
                    sizeY = this.state.originalHeight - (event.pageY - this.state.originalMouseY);

                    if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && sizeY > this.state.popupMinHeight) {
                        this.state.popup.style.height = sizeY + 'px';

                        let pxTop = this.state.originalY + (event.pageY - this.state.originalMouseY);
                        this.state.popup.style.top = ((pxTop / this.state.maxScreenHeight) * 100) + '%';
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
                    }
                    break;
                default:
            }

        }

        this.initPopupState();
        this.setScrollbar();
    }

    componentDidUpdate(prevProps, prevState) {
        // 팝업이 선택 됐을 때(Drag 될때) 맨 앞에 팝업 위치
        if (this.props.zIndex !== prevProps.zIndex) {
            this.state.popup.style.zIndex = this.props.zIndex;
        }

        this.setScrollbar();
    }

    setScrollbar() {
        const rect = this.refScrollArea.current.getBoundingClientRect();

        let scrollVisible = false;

        if (this.refBuildingGroup.current) {
            const rectBuildingGroup = this.refBuildingGroup.current.getBoundingClientRect();

            if (rectBuildingGroup.height > rect.height) {
                scrollVisible = true;
            }
        }

        SdmsScrollbar.setContentStyle(this.refScrollbar.current, rect.width, rect.height, scrollVisible);
    }

    initPopupState() {
        var popup = document.getElementsByClassName(styles.viewDashboard + ' ' + styles.viewDashboardBuildInfo)[0];

        //DB에 값이 있을 경우에만
        if (typeof this.props.popupState !== 'undefined') {
            popup.style.left = this.props.popupState.x;
            popup.style.top = this.props.popupState.y;
            popup.style.width = this.props.popupState.width;
            popup.style.height = this.props.popupState.height;
        }

        this.setState({ popup: popup });
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

    onChangeVisible(sensorType) {
        this.props.setVisiblePoi(sensorType, !this.props.visibleSensorTypes[sensorType]);
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

    onClose = () => {
        this.props.setVisiblePopup(this.props.popupType, false);
    }

    onChangeBuildingGroup = (value, type) => {
        this.props.onChangeBuildingGroup(value, type);
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
                key={buildingGroup.id}
                buildingGroup={buildingGroup}
                zoneList={this.props.zoneList}
                buildingIDs={this.props.buildingIDs}
                indoorModels={this.props.indoorModels}
                sensorList={this.props.sensorList}
                moveToX={this.props.moveToX}
                onSelectSensor={this.props.onSelectSensor}
                selectedSensor={this.props.selectedSensor}
                sensorAlarms={this.props.sensorAlarms}
                searchText={this.state.searchText}
                facilityInfos={this.props.facilityInfos}
                isEditMode={false}
                selectedInfo={this.props.selectedInfo}
                onChangeBuildingGroup={this.onChangeBuildingGroup}
            />);
        }

        if (this.props.outdoorZones) {
            ui.push(<StatusInfoBuildingGroup
                key={"bg_outdoor"}
                buildingGroup={this.props.outdoorZones}
                zoneList={this.props.zoneList}
                buildingIDs={this.props.buildingIDs}
                indoorModels={this.props.indoorModels}
                sensorList={this.props.sensorList}
                moveToX={this.props.moveToX}
                onSelectSensor={this.props.onSelectSensor}
                selectedSensor={this.props.selectedSensor}
                sensorAlarms={this.props.sensorAlarms}
                searchText={this.state.searchText}
                facilityInfos={this.props.facilityInfos}
                isEditMode={false}
                selectedInfo={this.props.selectedInfo}
                onChangeBuildingGroup={this.onChangeBuildingGroup}
            />);
        }

        return ui;
    }

    render() {
        let buildingGroupUI = this.getBuildingGroupUI();

        const visibleDetectSensor = this.props.visibleSensorTypes[SDMSMainMenu.Detect_Sensor] ? true : false;
        const visibleO2 = this.props.visibleSensorTypes[SDMSMainMenu.O2_Sensor] ? true : false;
        const visibleH2 = this.props.visibleSensorTypes[SDMSMainMenu.H2_Sensor] ? true : false;
        const visibleCo = this.props.visibleSensorTypes[SDMSMainMenu.CO_Sensor] ? true : false;
        const visibleCh4 = this.props.visibleSensorTypes[SDMSMainMenu.CH4_Sensor] ? true : false;
        const visibleCCTV = this.props.visibleSensorTypes[SDMSMainMenu.CCTV_Type] ? true : false;
        const visibleWorker = this.props.visibleSensorTypes[SDMSMainMenu.Worker_Type] ? true : false;

        return (
            <>
                <div className={styles.viewDashboard + " " + styles.viewDashboardBuildInfo}>
                    <div className={styles.popupSizingAreaRight} onMouseDown={(e) => this.popupResizeMousePress(e, 'h-r')} ></div>
                    <div className={styles.popupSizingAreaLeft} onMouseDown={(e) => this.popupResizeMousePress(e, 'h-l')}></div>
                    <div className={styles.popupSizingAreaBottom} onMouseDown={(e) => this.popupResizeMousePress(e, 'v-b')}></div>
                    <div className={styles.popupSizingAreaTop} onMouseDown={(e) => this.popupResizeMousePress(e, 'v-t')}></div>
                    <div className={styles.popupSizingAreaRightTopPoint} onMouseDown={(e) => this.popupResizeMousePress(e, 'd-rt')}></div>
                    <div className={styles.popupSizingAreaRightBottomPoint} onMouseDown={(e) => this.popupResizeMousePress(e, 'd-rb')}></div>
                    <div className={styles.popupSizingAreaLeftTopPoint} onMouseDown={(e) => this.popupResizeMousePress(e, 'd-lt')}></div>
                    <div className={styles.popupSizingAreaLeftBottomPoint} onMouseDown={(e) => this.popupResizeMousePress(e, 'd-lb')}></div>

                    {/*<div className={nst.dslTop + " " + nst.dslGrd}>*/}
                    <div className={styles.dslGrd}>
                        <h5 className={styles.dslTitle} onMouseDown={(e) => this.popupDragMousePress(e)}>현황정보</h5>
                        <div className={styles.colseX}><a onClick={this.onClose}><img src={imgClose} alt="닫기" /></a></div>
                        <div className={styles.dslCont}>
                            <span className={styles.statusInfo}>건물 현황정보</span>
                            {
                                ProjectResource.isModelViewer &&
                                <span className={styles.goLink} onClick={() => this.props.initOutdoorViewport()}><a>외부공간</a></span>
                            }
                            {
                                !ProjectResource.isModelViewer &&
                                <>
                                    <div className={styles.viewSet}>
                                        <div>POI 뷰어 설정<span className={styles.arrowDown}></span></div>
                                        <ul ref={this.refLayer}>
                                            <li>
                                                <span>감지센서</span>
                                                <div className={styles.switchBtn}>
                                                    <label className={styles.switch}>
                                                        <input type="checkbox" className={styles.labelInput} checked={visibleDetectSensor} onChange={() => this.onChangeVisible(SDMSMainMenu.Detect_Sensor)} />
                                                        <span className={styles.slider + " " + styles.round}></span>
                                                    </label>
                                                </div>
                                            </li>
                                            <li>
                                                <span>O2</span>
                                                <div className={styles.switchBtn}>
                                                    <label className={styles.switch}>
                                                        <input type="checkbox" className={styles.labelInput} checked={visibleO2} onChange={() => this.onChangeVisible(SDMSMainMenu.O2_Sensor)}/>
                                                        <span className={styles.slider + " " + styles.round}></span>
                                                    </label>
                                                </div>
                                            </li>
                                            <li>
                                                <span>H2</span>
                                                <div className={styles.switchBtn}>
                                                    <label className={styles.switch}>
                                                        <input type="checkbox" className={styles.labelInput} checked={visibleH2} onChange={() => this.onChangeVisible(SDMSMainMenu.H2_Sensor)}/>
                                                        <span className={styles.slider + " " + styles.round}></span>
                                                    </label>
                                                </div>
                                            </li>
                                            <li>
                                                <span>CO</span>
                                                <div className={styles.switchBtn}>
                                                    <label className={styles.switch}>
                                                        <input type="checkbox" className={styles.labelInput} checked={visibleCo} onChange={() => this.onChangeVisible(SDMSMainMenu.CO_Sensor)} />
                                                        <span className={styles.slider + " " + styles.round}></span>
                                                    </label>
                                                </div>
                                            </li>
                                            <li>
                                                <span>CH4</span>
                                                <div className={styles.switchBtn}>
                                                    <label className={styles.switch}>
                                                        <input type="checkbox" className={styles.labelInput} checked={visibleCh4} onChange={() => this.onChangeVisible(SDMSMainMenu.CH4_Sensor)} />
                                                        <span className={styles.slider + " " + styles.round}></span>
                                                    </label>
                                                </div>
                                            </li>
                                            <li>
                                                <span>CCTV</span>
                                                <div className={styles.switchBtn}>
                                                    <label className={styles.switch}>
                                                        <input type="checkbox" className={styles.labelInput} checked={visibleCCTV} onChange={() => this.onChangeVisible(SDMSMainMenu.CCTV_Type)}/>
                                                        <span className={styles.slider + " " + styles.round}></span>
                                                    </label>
                                                </div>
                                            </li>
                                            <li>
                                                <span>작업자</span>
                                                <div className={styles.switchBtn}>
                                                    <label className={styles.switch}>
                                                        <input type="checkbox" className={styles.labelInput} checked={visibleWorker} onChange={() => this.onChangeVisible(SDMSMainMenu.Worker_Type)} />
                                                        <span className={styles.slider + " " + styles.round}></span>
                                                    </label>
                                                </div>
                                            </li>
                                        </ul>
                                    </div>
                                    <div className={styles.dslSch}>
                                        <input type="text" placeholder="검색어를 입력해주세요." />
                                        <img src={imgZoomIco} alt="검색" />
                                    </div>
                                </>
                            }
                            <div ref={this.refScrollArea} className={styles.dsiScr}>
                                <Scrollbars ref={this.refScrollbar}>
                                    <div ref={this.refBuildingGroup} className={styles.dslScr}>
                                        <ul className={styles.viewListDo}>
                                            {buildingGroupUI}
                                        </ul>
                                    </div> {/*dslScr*/}
                                </Scrollbars>
                            </div>
                        </div> {/*dslCont*/}
                    </div> {/*dslGrd*/}
                </div> {/*viewDashboardBuildInfo*/}
            </>
        );
    }
} export default StatusInfo;