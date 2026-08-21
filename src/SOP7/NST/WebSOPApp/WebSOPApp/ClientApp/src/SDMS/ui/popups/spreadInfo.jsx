import React, { Component } from 'react';
import { BrowserRouter as Route, Link } from 'react-router-dom';
import { withRouter } from 'react-router-dom';
import styles from '../../css/sdms.module.css';
import imgClose from '../../image/common_Icon/popup_close.png';
import SetTargetMembers from './setTargetMembers';
import { SDMSController } from '../../services/sdmsController';


class SpreadInfo extends Component {
    constructor(props) {
        super(props);

        this.state = {
            visiblePopups: {
                setTargetMembers: false
            },
            regulars: [],
            regularMembers: [],
            selectMemberList: "",
            setTargetMembersState: {
                x: null,
                y: null,
            },
        }

        this.refTitle = React.createRef();
        this.refMessage = React.createRef();

        this.props = props;
        this.reloadTeamData();
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
        var popup = document.getElementsByClassName(styles.viewDashboardSpread)[0];

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

    setVisiblePopup = (popup, visible) => {
        const visiblePopups = { ...this.state.visiblePopups };
        visiblePopups[popup] = visible;
        this.setState({ visiblePopups });
    }

    // 드래그로 선택된 팝업과 나머지 팝업의 z-index를 조절한다. (선택된 팝업이 앞으로 나오도록)
    setActiveDragPopup = (popupType) => {
        this.props.setActiveDragPopup(this.props.popupType);
    }

    onClose = () => {
        this.props.setVisiblePopup(this.props.popupType, false);
    }

    onClickSetTargetMembers = () => {
        this.reloadTeamData();

        // TODO: 팝업창 뜨는 위치 잡는 계산
        // 정확한 계산이 필요함.
        let visiblePopups = this.state.visiblePopups;
        visiblePopups.setTargetMembers = true;

        let left = this.state.popup.style.left;
        const top = this.state.popup.style.top;

        left = left.replace("%", "");
        left = left * 1;
        left = left + 20;
        left = left + "%";

        let setTargetMembersState = this.state.setTargetMembersState;
        setTargetMembersState.x = left;
        setTargetMembersState.y = top;

        this.setState({ visiblePopups, setTargetMembersState});
    }

    async reloadTeamData() {
        let regulars = [];
        let regularMembers = [];
        let message = [];

        // 팀, 팀원 정보 불러오기
        const [regularsResult, regularsMessage] = await SDMSController.requestRegulars();

        if (regularsResult !== null && regularsResult !== undefined) {
            regulars = regularsResult;
        }

        const [regularMembersResult, regularsMembersMessage] = await SDMSController.requestRegularMembers();

        if (regularMembersResult !== null && regularMembersResult !== undefined) {
            regularMembers = regularMembersResult;
        }

        this.setState({ regulars: regulars, regularMembers: regularMembers});
    }

    saveMemberList = (selectMemberList) => {
        this.setState({ selectMemberList: selectMemberList });
    }

    displaySelectMemberList = () => {
        const selectMemberList = this.state.selectMemberList;
        const regularMembers = this.state.regularMembers;
        let displaySelectMemberList = "";
        let arrMemberList = [];

        if (selectMemberList === null || selectMemberList === undefined || selectMemberList === "" ||
            regularMembers === null || regularMembers === undefined)
            return displaySelectMemberList;

        arrMemberList = selectMemberList.split(",");
        let num = 1;

        for (let i = 0; i < arrMemberList.length; i++) {
            const memberID = arrMemberList[i];

            for (let j = 0; j < regularMembers.length; j++) {
                const regularMember = regularMembers[j];

                if (memberID === regularMember.id.toString()) {
                    if (displaySelectMemberList === "")
                        displaySelectMemberList = regularMember.memberName;
                    else
                        displaySelectMemberList = displaySelectMemberList + ", " + regularMember.memberName;

                    break;
                }
            }
        }

        return displaySelectMemberList;
    }

    onClickSend = () => {
        const title = this.refTitle.current.value;
        const message = this.refMessage.current.value;
        const selectMemberList = this.state.selectMemberList;
        let errMsg = "";

        if (title === null || title === undefined || title === "") {
            errMsg = "제목을 작성해주세요.";
        } else if (selectMemberList === null || selectMemberList === undefined || selectMemberList === "") {
            errMsg = "전파 대상자를 선택해주세요.";
        } else if (message === null || message === undefined || message === "") {
            errMsg = "상황전파 메세지를 작성해주세요.";
        }

        if (errMsg !== "") {
            this.props.popupMessage("상황전파", errMsg);
            return;
        }

        this.props.popupMessage("상황전파", "메시지 전송이 성공하였습니다.");
        this.props.setVisiblePopup(this.props.popupType, false);
    }

    render() {
        const targetMembersZIndex = this.props.zIndex > 0 ? this.props.zIndex + 1 : this.props.zIndex;
        const displaySelectMemberList = this.displaySelectMemberList();

        return (
            <div>
                {
                    <div className={styles.viewDashboardSpread}>
                        <div className={styles.viewTitle} onMouseDown={(e) => this.popupDragMousePress(e)}>상황전파</div>
                        <div className={styles.viewPopupBoxX}><a onClick={this.onClose}><img src={imgClose} alt="닫기" /></a></div>
                        <div className={styles.viewDashboardSpreadArea}>
                            <span>제목:</span><input ref={this.refTitle} type="text" className={styles.spreadInput} placeholder="상황전파 메세지의 제목을 입력하세요." />
                            <span>수신 대상자:</span><input type="text" value={displaySelectMemberList} className={styles.receiver} readOnly /><button className={styles.personSetting} onClick={this.onClickSetTargetMembers}>대상자 설정</button>
                            <span className={styles.content}>내용:</span>
                            <textarea ref={this.refMessage} type="text" className={styles.spreadMemo} cols="30" rows="3" placeholder="상황전파 메세지를 입력하세요." />
                            <div className={styles.spreadButtonArea}>
                                <button className={styles.spreadSend} onClick={this.onClickSend}>발송</button>
                                <button className={styles.spreadCancel} onClick={this.onClose}>취소</button>
                            </div>
                        </div>
                    </div>
                }

                {
                    this.state.visiblePopups.setTargetMembers &&
                    <SetTargetMembers
                        popupType="setTargetMembers"
                        setVisiblePopup={this.setVisiblePopup}
                        zIndex={targetMembersZIndex}
                        setActiveDragPopup={this.setActiveDragPopup}
                        regulars={this.state.regulars}
                        regularMembers={this.state.regularMembers}
                        selectMemberList={this.state.selectMemberList}
                        buildingGroupList={this.props.buildingGroupList}
                        workers={this.props.workers}
                        saveMemberList={this.saveMemberList}
                        popupState={this.state.setTargetMembersState}
                       
                    />
                }

         </div>
        )

    }


}export default SpreadInfo;