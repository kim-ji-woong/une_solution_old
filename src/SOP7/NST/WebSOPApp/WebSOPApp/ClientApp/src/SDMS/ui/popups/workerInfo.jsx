import React, { Component } from 'react';
import { BrowserRouter as Route, Link } from 'react-router-dom';
import { withRouter } from 'react-router-dom';
import styles from '../../css/sdms.module.css';
//import imgClose from '../../image/common_Icon/close_x.png';
import imgClose from '../../image/common_Icon/popup_close.png';


class WorkerInfo extends Component {
    constructor(props) {
        super(props);

        this.props = props;
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

        this.initPopupState();
    }

    componentDidUpdate(prevProps, prevState) {
        // 팝업이 선택 됐을 때(Drag 될때) 맨 앞에 팝업 위치
        if (this.props.zIndex !== prevProps.zIndex) {
            this.state.popup.style.zIndex = this.props.zIndex;
        }
    }

    initPopupState() {
        var popup = document.getElementsByClassName(styles.viewDashboard + " " + styles.viewDashboardPerson)[0];

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

    getWorkerInfo = () => {
        const worker = this.props.selectWorker;

        let name = "-";
        let regular = "-";
        let jobLevel = "-";
        let zoneName = "-";

        let location = {};
        location.x = "-";
        location.y = "-";

        if (worker === null || worker === undefined)
            return [name, regular, jobLevel, location, zoneName];

        name = worker.name;
        regular = worker.teamName;
        jobLevel = worker.jobLevelName;

        location.x = worker.x.toString();
        location.y = worker.y.toString();

        if (this.props.buildingGroupList !== null && this.props.buildingGroupList !== undefined) {
            let buildingGroupList = this.props.buildingGroupList;
            let chk = false;

            for (const buildingGroupID in buildingGroupList) {
                const buildingGroup = buildingGroupList[buildingGroupID];
                //console.log(buildingGroup.displayText);

                for (const buildingID in buildingGroup.buildingDatas) {
                    const building = buildingGroup.buildingDatas[buildingID];
                    //zoneDatas = Array(12)[Object, Object, Object, …]

                    for (const zoneID in building.zoneDatas) {
                        const zone = building.zoneDatas[zoneID];

                        if (zone.id === worker.zoneID) {
                            zoneName = zone.displayText;
                            chk = true;
                            break;
                        }
                    }

                    if (chk === true)
                        break;
                }

                if (chk === true)
                    break;
            }
        }

        return [name, regular, jobLevel, location, zoneName];
    }

    render() {
        const [name, regular, jobLevel, location, zoneName] = this.getWorkerInfo();

        return (
            <>
                <div className={styles.viewDashboard + " " + styles.viewDashboardPerson}>
                    <div className={styles.colseX}><a onClick={this.onClose}><img src={imgClose} alt="닫기" /></a></div>
                    <div className={styles.viewTitle} onMouseDown={(e) => this.popupDragMousePress(e)}>직원정보(<span className={styles.greenDOTPerson}></span>)</div>

                    <div className={styles.viewDashboardPersonview}>
                        <ul className={styles.workerInfo}>・ 직원정보
                            <li>  이름: {name}</li>
                            <li>  소속: {regular}</li>
                            <li>  직급: {jobLevel}</li>
                        </ul>
                        <ul><span className={styles.fontWeight}>・ 모바일 연결 상태 :</span> WIFI</ul>
                        <ul><span className={styles.fontWeight}>・ 위치:</span> {zoneName}  X: {location.x}, Y: {location.y}</ul>
                        <ul className={styles.equipment}>・ 작업자 안전 상태</ul>
                        <ul className={styles.equipmentIcon}>
                            <li><i className={styles.safetyHelmetDis}></i>안전모</li>
                            <li><i className={styles.safetyTeamAct}></i><span>안전조끼</span></li>
                            <li><i className={styles.maskAct}></i><span>마스크</span></li>
                            <li><i className={styles.safetyShoesAct}></i><span>안전화</span></li>
                            <li><i className={styles.safetyBeltAct}></i><span>안전대</span></li>
                            <li><i className={styles.dangerSpaceAct}></i><span>위험구역</span></li>
                        </ul>
                        <button onClick={this.props.openSpreadInfo}>상황전파</button>
                    </div>
                </div>
            </>
        )

    }

} export default WorkerInfo;