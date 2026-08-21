import React, { Component } from 'react';
import { BrowserRouter as Route, Link } from 'react-router-dom';
import { withRouter } from 'react-router-dom';
import styles from '../../css/sdms.module.css';
//import imgClose from '../../image/common_Icon/close_x.png';
import imgClose from '../../image/common_Icon/popup_close.png';


class CCTVInfo extends Component {

    constructor(props) {
        super(props);

        this.state = {
            cctvList: "",
            cctvWidth: 0,           // cctv 화면 사이즈(가로)
            cctvHeight: 0,          // cctv 화면 사이즈(세로)
            cctvCountMax: 4,        // cctv 최대 갯수
            streamServerURL: "",
            fullScreenIndex: -1,
            popupMinWidth: 360,
            popupMinHeight: 350
        }

        this.props = props;
        this.refContents = React.createRef();
        this.refArea = React.createRef();

        if (this.props.streamServerURL !== null && this.props.streamServerURL !== "" && this.props.streamServerURL !== undefined)
            this.state.streamServerURL = this.props.streamServerURL;

        this.widthDiff = 0;
        this.heightDiff = 0;
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
                        this.refContents.current.style.width = sizeX - this.widthDiff + 'px';
                    }
                    break;
                case 'h-l': //왼쪽 수평
                    sizeX = this.state.originalWidth - (event.pageX - this.state.originalMouseX);

                    if (event.pageX > 0 && event.pageX < this.state.maxScreenWidth && sizeX >= this.state.popupMinWidth) {
                        this.state.popup.style.width = sizeX + 'px';
                        this.refContents.current.style.width = sizeX - this.widthDiff + 'px';

                        let pxLeft = (this.state.originalX + (event.pageX - this.state.originalMouseX));
                        this.state.popup.style.left = ((pxLeft / this.state.maxScreenWidth) * 100) + '%';
                    }
                    break;
                // 수직
                case 'v-b': // 바텀 수직
                    sizeY = event.pageY - this.state.originalY;
                    
                    if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && sizeY >= this.state.popupMinHeight) {
                        this.state.popup.style.height = sizeY + 'px';
                        this.refContents.current.style.height = sizeY - this.heightDiff + 'px';
                    }
                    break;
                case 'v-t': //탑 수직
                    sizeY = this.state.originalHeight - (event.pageY - this.state.originalMouseY);

                    if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && sizeY >= this.state.popupMinHeight) {
                        this.state.popup.style.height = sizeY + 'px';
                        this.refContents.current.style.height = sizeY - this.heightDiff + 'px';

                        let pxTop = this.state.originalY + (event.pageY - this.state.originalMouseY);
                        this.state.popup.style.top = ((pxTop / this.state.maxScreenHeight) * 100) + '%';
                    }
                    break;
                // 대각
                case 'd-rb': // 오른쪽 하단 대각
                    sizeX = event.pageX - this.state.originalX;
                    sizeY = event.pageY - this.state.originalY;

                    if (event.pageX > 0 && event.pageX < this.state.maxScreenWidth && sizeX >= this.state.popupMinWidth) {
                        this.state.popup.style.width = sizeX + 'px';
                        this.refContents.current.style.width = sizeX - this.widthDiff + 'px';
                    }

                    if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && sizeY >= this.state.popupMinHeight) {
                        this.state.popup.style.height = sizeY + 'px';
                        this.refContents.current.style.height = sizeY - this.heightDiff + 'px';
                    }
                    break;
                case 'd-rt': //오른쪽 상단 대각
                    sizeX = this.state.originalWidth + (event.pageX - this.state.originalMouseX);
                    sizeY = this.state.originalHeight - (event.pageY - this.state.originalMouseY);

                    if (event.pageX > 0 && event.pageX < this.state.maxScreenWidth && sizeX >= this.state.popupMinWidth) {
                        this.state.popup.style.width = sizeX + 'px';
                        this.refContents.current.style.width = sizeX - this.widthDiff + 'px';
                    }

                    if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && sizeY >= this.state.popupMinHeight) {
                        this.state.popup.style.height = sizeY + 'px';
                        this.refContents.current.style.height = sizeY - this.heightDiff + 'px';

                        let pxTop = this.state.originalY + (event.pageY - this.state.originalMouseY);
                        this.state.popup.style.top = ((pxTop / this.state.maxScreenHeight) * 100) + '%';
                    }
                    break;
                case 'd-lb': //왼쪽 하단 대각
                    sizeX = this.state.originalWidth - (event.pageX - this.state.originalMouseX);
                    sizeY = event.pageY - this.state.originalY;

                    if (event.pageX > 0 && event.pageX < this.state.maxScreenWidth && sizeX >= this.state.popupMinWidth) {
                        this.state.popup.style.width = sizeX + 'px';
                        this.refContents.current.style.width = sizeX - this.widthDiff + 'px';

                        let pxLeft = (this.state.originalX + (event.pageX - this.state.originalMouseX));
                        this.state.popup.style.left = ((pxLeft / this.state.maxScreenWidth) * 100) + '%';
                    }

                    if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && sizeY >= this.state.popupMinHeight) {
                        this.state.popup.style.height = sizeY + 'px';
                        this.refContents.current.style.height = sizeY - this.heightDiff + 'px';
                    }
                    break;
                case 'd-lt': //왼쪽 상단 대각
                    sizeX = this.state.originalWidth - (event.pageX - this.state.originalMouseX);
                    sizeY = this.state.originalHeight - (event.pageY - this.state.originalMouseY);

                    if (event.pageX > 0 && event.pageX < this.state.maxScreenWidth && sizeX >= this.state.popupMinWidth) {
                        this.state.popup.style.width = sizeX + 'px';
                        this.refContents.current.style.width = sizeX - this.widthDiff + 'px';

                        let pxLeft = (this.state.originalX + (event.pageX - this.state.originalMouseX));
                        this.state.popup.style.left = ((pxLeft / this.state.maxScreenWidth) * 100) + '%';
                    }

                    if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && sizeY >= this.state.popupMinHeight) {
                        this.state.popup.style.height = sizeY + 'px';
                        this.refContents.current.style.height = sizeY - this.heightDiff + 'px';

                        let pxTop = this.state.originalY + (event.pageY - this.state.originalMouseY);
                        this.state.popup.style.top = ((pxTop / this.state.maxScreenHeight) * 100) + '%';
                    }
                    break;
                default:
            }

            this.initSizeCCTV();
            this.showCCTVs();
        }

        this.initPopupState();
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
        const grid_col = this.findElementByClassName("cctv1_li");
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

    componentDidUpdate(prevProps, prevState) {
        this.state.cctvList = this.props.cctvList;
        this.showCCTVs();

        // 팝업이 선택 됐을 때(Drag 될때) 맨 앞에 팝업 위치
        if (this.props.zIndex !== prevProps.zIndex) {
            this.state.popup.style.zIndex = this.props.zIndex;
        }
    }

    showCCTVs = () => {
        const input = !this.state.cctvList ? "" : this.state.cctvList;
        const cctvIDs = input.toString().trim();

        const ids = cctvIDs.split(',');
        let idCount = ids.length;

        if (idCount > this.state.cctvCountMax)
            idCount = this.state.cctvCountMax;

        //if (this.props.editMode === Contents3D.Edit_Mode_CCTVGroup) {
        //    this.setEditModeCCTVList(ids, idCount);
        //}

        let index = 1;

        for (let i = 0; i < idCount; i++) {
            const suuid = ids[i].trim();

            if (suuid.length === 0) {
                continue;
            }

            const id = '#cctv' + index.toString();
            this.connectStream(suuid, id);
            index++;
        }

        for (let i = index; i <= this.state.cctvCountMax; i++) {
            const id = '#cctv' + i.toString();
            this.closeStream(id);
        }

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
        //frame.previousElementSibling.dataset.url = this.state.streamServerURL + "/stream/player/" + suuid;


        //let cctvs = this.props.cctvs;
        //let cctvName;
        //for (let i = 0; cctvs.length > i; i++) {
        //    if (cctvs[i].id == suuid) {
        //        cctvName = suuid + ". " + cctvs[i].name;
        //        break;
        //    }
        //}

        //frame.parentNode.dataset.cctvname = cctvName;
        //frame.previousElementSibling.dataset.cctvname = cctvName;


        // CCTV 번호 및 이름 표시
        this.showCCTVInfo(suuid, id);

        const hidden = frame.classList.contains(styles.hidden);

        if (hidden) {
            frame.classList.remove(styles.hidden);
        }
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

        //const title = this.getChildElement(rootElement, findTitleID);
        //const title = document.querySelector(titleID);

        if (frame !== null && frame !== undefined) {
            const url = "";
            frame.setAttribute("src", url);

            //title.innerHTML = "";

            const hidden = frame.classList.contains(styles.hidden);

            if (hidden === false) {
                frame.classList.add(styles.hidden);
            }
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

    initPopupState() {
        //this.refArea.current.style.height = '290px';
        var popup = document.getElementsByClassName(styles.viewDashboard + ' ' + styles.viewDashboardCCTV)[0];

        //DB에 값이 있을 경우에만
        if (typeof this.props.popupState !== 'undefined') {
            popup.style.left = this.props.popupState.x;
            popup.style.top = this.props.popupState.y;
            popup.style.width = this.props.popupState.width;
            popup.style.height = this.props.popupState.height;
        } else {
            popup.style.left = "78.5%";
            popup.style.top = "61%";
            popup.style.width = "360px";
            popup.style.height = "350px";
        }

        const popupRect = popup.getBoundingClientRect();
        const contentsRect = this.refContents.current.getBoundingClientRect();

        this.widthDiff = popupRect.width - contentsRect.width;
        this.heightDiff = popupRect.height - contentsRect.height;
        //this.heightDiff = 0;

        this.initSizeCCTV();
        this.showCCTVs();
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

    onClose = () => {
        this.props.setVisiblePopup(this.props.popupType, false);
    }

    showFullScreenCCTV(index) {
        const rootElement = document.querySelector("#" + this.props.popupType);

        if (!rootElement) {
            return;
        }

        const gridParent = this.findElementByClassName(styles.viewDashboardCCTVAreas, rootElement);
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

    getFullScreenClassName(index) {
        if (index === this.state.fullScreenIndex) {
            return " " + styles.full;
        }

        if (this.state.fullScreenIndex > 0) {
            return " " + styles.hidden;
        }

        return "";
    }

    render() {

        return (
            <>
                {/*CCTV 단일*/}
                <div id={this.props.popupType} className={styles.viewDashboard + " " + styles.viewDashboardCCTV}>
                    <div className={styles.popupSizingAreaRight} onMouseDown={(e) => this.popupResizeMousePress(e, 'h-r')} ></div>
                    <div className={styles.popupSizingAreaLeft} onMouseDown={(e) => this.popupResizeMousePress(e, 'h-l')}></div>
                    <div className={styles.popupSizingAreaBottom} onMouseDown={(e) => this.popupResizeMousePress(e, 'v-b')}></div>
                    <div className={styles.popupSizingAreaTop} onMouseDown={(e) => this.popupResizeMousePress(e, 'v-t')}></div>
                    <div className={styles.popupSizingAreaRightTopPoint} onMouseDown={(e) => this.popupResizeMousePress(e, 'd-rt')}></div>
                    <div className={styles.popupSizingAreaRightBottomPoint} onMouseDown={(e) => this.popupResizeMousePress(e, 'd-rb')}></div>
                    <div className={styles.popupSizingAreaLeftTopPoint} onMouseDown={(e) => this.popupResizeMousePress(e, 'd-lt')}></div>
                    <div className={styles.popupSizingAreaLeftBottomPoint} onMouseDown={(e) => this.popupResizeMousePress(e, 'd-lb')}></div>

                    <div className={styles.colseX}><a onClick={this.onClose}><img src={imgClose} alt="닫기" /></a></div>
                    <div className={styles.viewTitle} onMouseDown={(e) => this.popupDragMousePress(e)}>CCTV(<span className={styles.greenDOTCCTV}></span>)</div>



                    {/*<div ref={this.refContents} className={styles.viewDashboardCCTVConts}>*/}
                    {/*    <div ref={this.refArea} className={styles.viewDashboardCCTVArea}>CCTV 표출영역</div>*/}
                    {/*</div>*/}
                    <div ref={this.refContents} className={styles.viewDashboardCCTVsConts}>
                        {/*<div ref={this.refArea} className={styles.viewDashboardCCTVAreas}>*/}
                        {/*    <ul>*/}
                        {/*        <li className={"cctv1_li"}><span onDoubleClick={(e) => this.showFullScreenCCTV(1)}><p id="cctv1_name"></p><iframe id="cctv1" allowtransparency="yes" scrolling="no"></iframe></span></li>*/}
                        {/*        <li className={"cctv2_li"}><span onDoubleClick={(e) => this.showFullScreenCCTV(2)}><p id="cctv2_name"></p><iframe id="cctv2" allowtransparency="yes" scrolling="no"></iframe></span></li>*/}
                        {/*        <li className={"cctv3_li"}><span onDoubleClick={(e) => this.showFullScreenCCTV(3)}><p id="cctv3_name"></p><iframe id="cctv3" allowtransparency="yes" scrolling="no"></iframe></span></li>*/}
                        {/*        <li className={"cctv4_li"}><span onDoubleClick={(e) => this.showFullScreenCCTV(4)}><p id="cctv4_name"></p><iframe id="cctv4" allowtransparency="yes" scrolling="no"></iframe></span></li>*/}
                        {/*    </ul>*/}
                        {/*</div>*/}

                        <div ref={this.refArea} className={styles.viewDashboardCCTVGrid}>
                            <div className={styles.col1row1 + this.getFullScreenClassName(1)}>
                                <span id="cctv1_span" onDoubleClick={(e) => this.showFullScreenCCTV(1)}>
                                    <p id="cctv1_name"></p>
                                    <iframe id="cctv1" allowtransparency="yes" scrolling="no"></iframe>
                                </span>
                            </div>
                            <div className={styles.col2row1 + this.getFullScreenClassName(2)}>
                                <span id="cctv2_span" onDoubleClick={(e) => this.showFullScreenCCTV(2)}>
                                    <p id="cctv2_name"></p>
                                    <iframe id="cctv2" allowtransparency="yes" scrolling="no"></iframe>
                                </span>
                            </div>
                            <div className={styles.col1row2 + this.getFullScreenClassName(3)}>
                                <span id="cctv3_span" onDoubleClick={(e) => this.showFullScreenCCTV(3)}>
                                    <p id="cctv3_name"></p>
                                    <iframe id="cctv3" allowtransparency="yes" scrolling="no"></iframe>
                                </span>
                            </div>
                            <div className={styles.col2row2 + this.getFullScreenClassName(4)}>
                                <span id="cctv4_span" onDoubleClick={(e) => this.showFullScreenCCTV(4)}>
                                    <p id="cctv4_name"></p>
                                    <iframe id="cctv4" allowtransparency="yes" scrolling="no"></iframe>
                                </span>
                            </div>
                        </div>
                    </div>



                </div>

                {/*CCTV 다중*/}
                {/*<div className={styles.viewDashboardBoxD + " " + styles.viewDashboardCCTVs}>
                    <div className={styles.colseX}><a onClick={this.onClose}><img src={imgClose} alt="닫기" /></a></div>
                    <div className={styles.viewTitlee}>CCTV(1<span className={styles.greenDOTCCTV}></span>/2<span className={styles.greenDOTCCTV2}></span>)</div>

                   <div className={styles.viewDashboardCCTVsConts}>
                        <div className={styles.viewDashboardCCTVAreas}>
                            <ul>
                            <li><span>CCTV OFF</span><img src={imgClose} alt="닫기" /></li>
                            <li><span>CCTV 표출영역</span><img src={imgClose} alt="닫기" /></li>
                            <li><span>CCTV 표출영역</span><img src={imgClose} alt="닫기" /></li>
                            <li><span>CCTV 표출영역</span><img src={imgClose} alt="닫기" /></li>
                            </ul>
                        </div>
                    </div>
                </div>*/}

                {/*열화상 CCTV*/}
                {/* <div className={styles.viewDashboardBoxD + " " + styles.thermalBurnsCCTV}>
                    <div className={styles.colseX}><a href="#"><img src={imgClose} alt="닫기" /></a></div>
                    <div className={styles.thermalTitle}>열화상 CCTV(<span className={styles.greenDOTCCTV}></span>)</div>
                    <div className={styles.thermalBurnsCCTVConts}>
                        <div className={styles.thermalBurnsCCTVContsArea}>열화상 CCTV 표출영역</div>
                    </div>
                </div>*/}
        </>    
            
            
       )

    }







} export default CCTVInfo;