import { ui } from 'jquery';
import React, { Component } from 'react';
import content from '../../../Common/css/content.module.css';
import sdmsStyle from '../../css/sdms.module.css';
import imgClose from '../../../Common/image/icon/close_x.png';
import imgMinimap from '../../../Common/image/temp/mini_img1.png';
import SDMS from '../sdms';
import SettingsStore from '../../../Settings/settingsStore';
import * as THREE from "three/build/three.module.js";

class MiniMap extends Component {
    constructor(props) {
        super(props);
        this.state = {
            popupMinWidth: 350,
            popupMinHeight: 260,
        }

        this.initPopupState = this.initPopupState.bind(this);

        SettingsStore.subscribe(function () {
            this.resetPopupState(SettingsStore.getState());
        }.bind(this));

        if (SDMS.UseWalkingAvatar) {
            this.refPosX = React.createRef();
            this.refPosY = React.createRef();
            this.refPosZ = React.createRef();
            this.refScaleX = React.createRef();
            this.refScaleY = React.createRef();
            this.refScaleZ = React.createRef();
        }
    }

    componentDidMount() {
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

        this.props.setActiveDragPopup(this.props.popupType);
    }

    componentDidUpdate(prevProps, prevState) {
        // 팝업이 선택 됐을 때(Drag 될때) 맨 앞에 팝업 위치
        if (this.props.zIndex !== prevProps.zIndex) {
            this.state.popup.style.zIndex = this.props.zIndex;
            console.log('miniMapZIndex changed', this.state.popup.style.zIndex);
        }
    }

    initPopupState() {
        var popup = document.getElementsByClassName(content.viewDashboardBoxD + ' ' + content.viewDashboardMiniMap)[0];

        //DB에 값이 있을 경우에만
        if (typeof this.props.popupState !== 'undefined') {
            popup.style.left = this.props.popupState.x;
            popup.style.top = this.props.popupState.y;
            popup.style.width = this.props.popupState.width;
            popup.style.height = this.props.popupState.height;
        }

        this.setState({ popup: popup });
    }

    repositionPopup(popupState) {
        let data = popupState.miniMap;

        if (data === null || data === undefined)
            return;

        let popup = document.getElementsByClassName(content.viewDashboardBoxD + ' ' + content.viewDashboardMiniMap)[0];
        if (popup === null || popup === undefined)
            return;

        popup.style.left = data.x;
        popup.style.top = data.y;
        popup.style.width = data.width;
        popup.style.height = data.height;

        this.setState({ popup: popup });
    }

    resetPopupState = (popupState) => {
        let data = popupState;

        if (data.actionType === 'RESET_POPUP') {
            this.repositionPopup(data.popupState);
        }
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
                id: typeof this.props.popupState !== 'undefined' ? this.props.popupState.id : -1,
                x: perX + '%',
                y: perY + '%',
                height: height + 'px',
                width: width + 'px'
            }
            this.props.setPopupState(this.props.popupType, popupState);
        }
    }

    getImageURL() {
        if (this.props.currentView.buildingID === null) {
            return '/resource/image/minimap/outdoor.png';
        }
        else if (this.props.currentView.buildingID >= 100) {
            return '/resource/image/minimap/outdoor_' + this.props.currentView.buildingID + '.png';
        }
        else if (this.props.currentView.buildingID >= 10) {
            return '/resource/image/minimap/outdoor_0' + this.props.currentView.buildingID + '.png';
        }

        return '/resource/image/minimap/outdoor_00' + this.props.currentView.buildingID + '.png';
    }

    getImageTitle() {
        if (this.props.currentView.zoneName.length === 0) {
            return <></>;
        }

        return <p>{this.props.currentView.zoneName}</p>;
    }

    getWalkerInfo() {
        if (!this.props.walker || !this.props.walker.model) {
            return [0, 0, 0, 0, 0, 0, true];
        }

        let globalCamera = true;

        if (this.props.walker.contents3D) {
            if (this.props.walker.contents3D.camera === this.props.walker.camera) {
                globalCamera = false;
            }
        }

        const model = this.props.walker.model;
        return [model.position.x, model.position.y, model.position.z, model.scale.x, model.scale.y, model.scale.z, globalCamera];
    }

    onChangeText = (event) => {
    }

    onClickChangePosition = (event) => {
        if (!this.props.walker || !this.props.walker.model) {
            return;
        }

        const x = Number(this.refPosX.current.value.trim());
        const y = Number(this.refPosY.current.value.trim());
        const z = Number(this.refPosZ.current.value.trim());

        if (isNaN(x) || isNaN(y) || isNaN(z)) {
            return;
        }

        this.props.walker.model.position.set(x, y, z);
        this.props.walker.moveCamera(new THREE.Vector3(x, y, z));
        this.props.walker.model.position.set(x, y, z);
    }

    onClickChangeScale = (event) => {
        if (!this.props.walker || !this.props.walker.model) {
            return;
        }

        const x = Number(this.refScaleX.current.value.trim());
        const y = Number(this.refScaleY.current.value.trim());
        const z = Number(this.refScaleZ.current.value.trim());

        if (isNaN(x) || isNaN(y) || isNaN(z)) {
            return;
        }

        this.props.walker.model.scale.set(x, y, z);
        this.props.walker.moveCamera(new THREE.Vector3(this.props.walker.model.position.x, this.props.walker.model.position.y, this.props.walker.model.position.z));
    }

    changeCamera(globalCamera) {
        if (this.props.walker) {
            if (globalCamera) {
                this.props.walker.setGlobalCamera();
            }
            else {
                this.props.walker.setAvatarCamera();
            }

            this.setState({ popupMinWidth: this.state.popupMinWidth });
        }
    }

    setCameraDistance(faraway) {
        if (this.props.walker) {
            this.props.walker.farFromModel(faraway);
        }
    }

    setCameraElevation(upper) {
        if (this.props.walker) {
            this.props.walker.goUpCamera(upper);
        }
    }

    setCameraAngle(mode) {
        if (this.props.walker) {
            if (mode === 1) {
                this.props.walker.rotateVerticalCamera(true);
            }
            else if (mode === 2) {
                this.props.walker.rotateVerticalCamera(false);
            }
            else if (mode === 3) {
                this.props.walker.rotateHorizontalCamera(true);
            }
            else if (mode === 4) {
                this.props.walker.rotateHorizontalCamera(false);
            }
        }
    }

    setCameraToRight(right) {
        if (this.props.walker) {
            this.props.walker.cameraToRight(right);
        }
    }

    getAvatarElements() {
        const [posX, posY, posZ, scaleX, scaleY, scaleZ, globalCamera] = this.getWalkerInfo();

        return (
            <div id={this.props.popupType} className={content.viewDashboardBoxD + ' ' + content.viewDashboardMiniMap}>
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
                        미니맵
                    </h5>
                    <a className={content.dslX} onClick={() => this.props.setVisiblePopups(SDMS.menu.miniMap, false)}></a>
                </div>

                <div className={sdmsStyle.miniMapConts}>
                    <div>
                        <h5 className={sdmsStyle.sensorName}>Model 위치</h5>
                        <div className={sdmsStyle.horzArea}>
                            <input ref={this.refPosX} className={sdmsStyle.inputText + " " + sdmsStyle.width100} type="text" placeholder="X" defaultValue={posX} onChange={this.onChangeText} />
                            <input ref={this.refPosY} className={sdmsStyle.inputText + " " + sdmsStyle.width100} type="text" placeholder="Y" defaultValue={posY} onChange={this.onChangeText} />
                            <input ref={this.refPosZ} className={sdmsStyle.inputText + " " + sdmsStyle.width100} type="text" placeholder="Z" defaultValue={posZ} onChange={this.onChangeText} />
                            <button className={sdmsStyle.menuBtnModel} onClick={this.onClickChangePosition}>이동</button>
                        </div>

                        <br />

                        <h5 className={sdmsStyle.sensorName}>Model Scale</h5>
                        <div className={sdmsStyle.horzArea}>
                            <input ref={this.refScaleX} className={sdmsStyle.inputText + " " + sdmsStyle.width100} type="text" placeholder="X" defaultValue={scaleX} onChange={this.onChangeText} />
                            <input ref={this.refScaleY} className={sdmsStyle.inputText + " " + sdmsStyle.width100} type="text" placeholder="Y" defaultValue={scaleY} onChange={this.onChangeText} />
                            <input ref={this.refScaleZ} className={sdmsStyle.inputText + " " + sdmsStyle.width100} type="text" placeholder="Z" defaultValue={scaleZ} onChange={this.onChangeText} />
                            <button className={sdmsStyle.menuBtnModel} onClick={this.onClickChangeScale}>변경</button>
                        </div>

                        <br />

                        <h5 className={sdmsStyle.sensorName + " " + sdmsStyle.marginBottom10}>카메라 설정</h5>
                        <div className={sdmsStyle.horzArea}>
                            <label className={sdmsStyle.clickable + " " + sdmsStyle.whiteText}>
                                <input type="checkbox" className={sdmsStyle.labelInput} checked={globalCamera} onChange={() => this.changeCamera(true)} />
								전역 카메라
							</label>

                            <label className={sdmsStyle.clickable + " " + sdmsStyle.whiteText}>
                                <input type="checkbox" className={sdmsStyle.labelInput} checked={!globalCamera} onChange={() => this.changeCamera(false)} />
								1인칭 카메라
							</label>
                        </div>

                        <br />

                        <h5 className={sdmsStyle.sensorName + " " + sdmsStyle.marginBottom10}>카메라 거리</h5>
                        <div className={sdmsStyle.horzArea}>
                            <button className={sdmsStyle.menuBtnModel} onClick={() => this.setCameraDistance(true)}>멀리</button>
                            <button className={sdmsStyle.menuBtnModel} onClick={() => this.setCameraDistance(false)}>가까이</button>
                            <button className={sdmsStyle.menuBtnModel} onClick={() => this.setCameraElevation(true)}>위로</button>
                            <button className={sdmsStyle.menuBtnModel} onClick={() => this.setCameraElevation(false)}>아래로</button>
                            <button className={sdmsStyle.menuBtnModel} onClick={() => this.setCameraToRight(true)}>오른쪽</button>
                            <button className={sdmsStyle.menuBtnModel} onClick={() => this.setCameraToRight(false)}>왼쪽</button>
                        </div>

                        <br />

                        <h5 className={sdmsStyle.sensorName + " " + sdmsStyle.marginBottom10}>카메라 각도</h5>
                        <div className={sdmsStyle.horzArea}>
                            <button className={sdmsStyle.menuBtnModel} onClick={() => this.setCameraAngle(1)}>위로</button>
                            <button className={sdmsStyle.menuBtnModel} onClick={() => this.setCameraAngle(2)}>아래로</button>
                            <button className={sdmsStyle.menuBtnModel} onClick={() => this.setCameraAngle(3)}>오른쪽</button>
                            <button className={sdmsStyle.menuBtnModel} onClick={() => this.setCameraAngle(4)}>왼쪽</button>
                        </div>
                    </div>
                </div>
            </div>
        );
    }

    render() {
        const url = this.getImageURL();//'/resource/image/minimap/outdoor.png';

        if (SDMS.UseWalkingAvatar) {
            return this.getAvatarElements();
        }

        return (
            <div id={this.props.popupType} className={content.viewDashboardBoxD + ' ' + content.viewDashboardMiniMap}>
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
                        미니맵
                    </h5>
                    <a className={content.dslX} onClick={() => this.props.setVisiblePopups(SDMS.menu.miniMap, false)}></a>
                </div>

                <div className={sdmsStyle.miniMapConts}>
                    {
                        this.getImageTitle()
                    }
                    {
                        <img sync="true" importance="high" src={url} alt="미니맵" />
                        //<img src={imgMinimap} alt="미니맵" />
                    }
                </div>
            </div>
        );
    }
}

export default MiniMap;