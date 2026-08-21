import React, { Component } from 'react';
import styles from '../../css/sdms.module.css';
import imgClose from '../../image/common_Icon/popup_close.png';

class SetTargetMembers extends Component {
    static ManualTarget = 0;
    static AutomaticTarget = 1;

    constructor(props) {
        super(props);

        this.state = {
            optionTarget: SetTargetMembers.ManualTarget,
            selectRegular: null,
            selectRegularMember: null,
            selectMemberList: [],
            selectRemoveMember: null,
            selectBuilding: null,
            selectZone: null,
        }

        this.props = props;
        this.state.selectMemberList = this.props.selectMemberList;
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
        var popup = document.getElementsByClassName(styles.spreadPersonBox)[0];

        if (typeof this.props.popupState !== 'undefined') {
            popup.style.left = this.props.popupState.x;
            popup.style.top = this.props.popupState.y;
            //popup.style.width = this.props.popupState.width;
            //popup.style.height = this.props.popupState.height;
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
    }

    onChangeTarget(option) {
        if (this.state.optionTarget === option) {
            return;
        }

        // 선택된 값 초기화
        this.setState({ optionTarget: option, selectRegular: null, selectRegularMember: null, selectBuilding: null, selectZone: null });
    }

    onClose = () => {
        this.props.setVisiblePopup(this.props.popupType, false);
    }

    displayRegulars = () => {
        const regulars = this.props.regulars;
        const selectRegular = this.state.selectRegular;
        let displayRegulars = [];


        if (regulars === null || regulars === undefined) {
            return displayRegulars;
        }

        for (let i = 0; i < regulars.length; i++) {
            let regular = regulars[i];
            let chkClass = "";

            if (selectRegular !== null && selectRegular !== undefined && selectRegular === regular.id)
                chkClass = styles.spreadCheck;

            displayRegulars.push(
                <tr key={"displayRegulars_" + regular.id} onClick={() => this.onClickRegular(regular.id)}>
                    <td>{i + 1}</td>
                    <td className={chkClass}>{regular.teamName}</td>
                </tr>);

        }

        return displayRegulars;
    }

    onClickRegular = (id) => {
        if (id === null || id === undefined)
            return;

        this.setState({ selectRegular: id, selectRegularMember: null });
    }

    displayRegularMembers = () => {
        const regularMembers = this.props.regularMembers;
        const selectRegular = this.state.selectRegular;
        const selectRegularMember = this.state.selectRegularMember;
        let displayRegularMembers = [];

        if (regularMembers === null || regularMembers === undefined || selectRegular === null || selectRegular === undefined) {
            return displayRegularMembers;
        }

        for (let i = 0; i < regularMembers.length; i++) {
            let regularMember = regularMembers[i];
            let chkClass = "";

            if (selectRegularMember !== null && selectRegularMember !== undefined && selectRegularMember === regularMember.id)
                chkClass = styles.spreadCheck;

            if (regularMember.regularID === selectRegular) {
                displayRegularMembers.push(
                    <tr key={"displayRegularMembers_" + regularMember.id} onClick={() => this.onClickRegularMember(regularMember.id)}>
                        <td>{i + 1}</td>
                        <td className={chkClass}>{regularMember.memberName}</td>
                    </tr>);
            }
        }

        return displayRegularMembers;
    }

    onClickRegularMember = (id) => {
        if (id === null || id === undefined)
            return;

        this.setState({ selectRegularMember: id });
    }

    displayBuilding = () => {
        const buildingGroupList = this.props.buildingGroupList;
        const selectBuilding = this.state.selectBuilding;
        let displayBuilding = [];
        let num = 1;

        if (buildingGroupList === null || buildingGroupList === undefined)
            return displayBuilding;

        for (let i = 0; i < buildingGroupList.length; i++) {
            const buildingGroup = buildingGroupList[i];

            for (let j = 0; j < buildingGroup.buildingDatas.length; j++) {
                const building = buildingGroup.buildingDatas[j];
                let chkClass = "";

                if (selectBuilding !== null && selectBuilding !== undefined && selectBuilding.id === building.id)
                    chkClass = styles.spreadCheck;

                displayBuilding.push(
                    <tr key={"displayBuilding_" + building.id} onClick={() => this.onClickBuilding(building)}>
                        <td>{num}</td>
                        <td className={chkClass}>{building.displayText}</td>
                    </tr>);

                num++;
            }
        }

        if (displayBuilding.length > 0) {
            // 외부영역 데이터 만들기
            let building = {
                id: 12,
                displayText: "외부영역",
                zoneDatas: [],
            }

            let zone = {
                id: 12,
                displayText: "외부영역",
            }

            building.zoneDatas.push(zone);

            let chkClass = "";

            if (selectBuilding !== null && selectBuilding !== undefined && selectBuilding.id === building.id)
                chkClass = styles.spreadCheck;

            displayBuilding.push(
                <tr key = { "displayBuilding_" + building.id } onClick={() => this.onClickBuilding(building)}>
                    <td>{num}</td>
                    <td className={chkClass}>{building.displayText}</td>
                </tr>);
        }

        return displayBuilding;
    }

    onClickBuilding = (building) => {
        if (building === null || building === undefined)
            return;

        this.setState({ selectBuilding: building, selectZone: null, selectRegularMember: null});
    }

    displayZone = () => {
        const selectBuilding = this.state.selectBuilding;
        const selectZone = this.state.selectZone;
        let displayZone = [];

        if (selectBuilding === null || selectBuilding === undefined)
            return displayZone;

        const zoneDatas = selectBuilding.zoneDatas;

        for (let i = 0; i < zoneDatas.length; i++) {
            const zone = zoneDatas[i];
            let chkClass = "";

            if (selectZone !== null && selectZone !== undefined && selectZone === zone.id)
                chkClass = styles.spreadCheck;

            displayZone.push(
                <tr key={"displayZone_" + zone.id} onClick={() => this.onClickZone(zone.id)}>
                    <td>{i + 1}</td>
                    <td className={chkClass}>{zone.displayText}</td>
                </tr>);
        }

        return displayZone;
    }

    onClickZone = (id) => {
        if (id === null || id === undefined)
            return;

        this.setState({ selectZone: id, selectRegularMember: null });
    }

    displayZoneMember = () => {
        const selectZone = this.state.selectZone;
        const selectRegularMember = this.state.selectRegularMember;
        const workers = this.props.workers;

        let displayZoneMember = [];

        if (selectZone === null || selectZone === undefined || workers === null || workers === undefined)
            return displayZoneMember;

        const workZones = workers.zones;

        for (const zoneID in workZones) {
            if (selectZone.toString() !== zoneID)
                continue;

            const zone = workZones[zoneID];
            let num = 1;

            for (const memberID in zone) {
                const member = zone[memberID];
                let chkClass = "";

                if (selectRegularMember !== null && selectRegularMember !== undefined && selectRegularMember === member.id) {
                    chkClass = styles.spreadCheck;
                }

                displayZoneMember.push(
                    <tr key={"displayZoneMember_" + member.id} onClick={() => this.onClickRegularMember(member.id)}>
                        <td>{num}</td>
                        <td className={chkClass}>{member.name}</td>
                    </tr>);

                num++;
            }
        }

        return displayZoneMember;
    }

    displayTarget = () => {
        const optionTarget = this.state.optionTarget;

        if (optionTarget === SetTargetMembers.ManualTarget) {

            const displayRegulars = this.displayRegulars();
            const displayRegularMembers = this.displayRegularMembers();

            return (
                <>
                    <div className={styles.spreadTb1}>
                        <span>부서 선택</span>
                        <div className={styles.tableParentCC}>
                            <table className={styles.tblCC + " " + styles.scrollbar}>
                                <colgroup>
                                    <col style={{ width: "50px" }} />
                                    <col style={{ width: "130px" }} />
                                </colgroup>
                                <thead>
                                    <tr>
                                        <th>No</th>
                                        <th>부서명</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {displayRegulars}
                                </tbody>
                            </table>
                        </div>
                    </div>
                    <div className={styles.spreadTb2}>
                        <span>팀원 선택</span>
                        <div className={styles.tableParentE}>
                            <table className={styles.tblE + " " + styles.scrollbar}>
                                <colgroup>
                                    <col style={{ width: "50px" }} />
                                    <col style={{ width: "130px" }} />
                                </colgroup>
                                <thead>
                                    <tr>
                                        <th>No</th>
                                        <th>이름</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {displayRegularMembers}
                                </tbody>
                            </table>
                        </div>
                    </div>
                </>
            );
        } else {
            const displayBuilding = this.displayBuilding();
            const displayZone = this.displayZone();
            const displayZoneMember = this.displayZoneMember();

            return (
                <>
                    <div className={styles.spreadTb4}>
                        <span>건물 선택</span>
                        <div className={styles.tableParentG}>
                            <table className={styles.tblG + " " + styles.scrollbar}>
                                <colgroup>
                                    <col style={{ width: "30px" }} />
                                    <col style={{ width: "100px" }} />
                                </colgroup>
                                <thead>
                                    <tr>
                                        <th>No</th>
                                        <th>건물명</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {displayBuilding}
                                </tbody>
                            </table>
                        </div>
                    </div>
                    <div className={styles.spreadTb5}>
                        <span>층 선택</span>
                        <div className={styles.tableParentH}>
                            <table className={styles.tblH + " " + styles.scrollbar}>
                                <colgroup>
                                    <col style={{ width: "30px" }} />
                                    <col style={{ width: "100px" }} />
                                </colgroup>
                                <thead>
                                    <tr>
                                        <th>No</th>
                                        <th>층</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {displayZone}
                                </tbody>
                            </table>
                        </div>
                    </div>
                    <div className={styles.spreadTb6}>
                        <span>팀원 선택</span>
                        <div className={styles.tableParentI}>
                            <table className={styles.tblI + " " + styles.scrollbar}>
                                <colgroup>
                                    <col style={{ width: "30px" }} />
                                    <col style={{ width: "100px" }} />
                                </colgroup>
                                <thead>
                                    <tr>
                                        <th>No</th>
                                        <th>이름</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {displayZoneMember}
                                </tbody>
                            </table>
                        </div>
                    </div>
                </>
            );
        }
    }

    onClickAdd = () => {
        const selectRegularMember = this.state.selectRegularMember;
        let selectMemberList = this.state.selectMemberList;
        let arrMemberList = [];
        let selectMemberListData = "";

        if (selectRegularMember === null || selectRegularMember === undefined)
            return;

        if (selectMemberList === null || selectMemberList === undefined)
            selectMemberList = [];

        if (selectMemberList.length > 0) {
            arrMemberList = selectMemberList.split(",");
            let chk = false;

            for (let i = 0; i < arrMemberList.length; i++) {
                let memberID = arrMemberList[i];

                if (memberID === selectRegularMember.toString()) {
                    chk = true;
                    break;
                }
            }

            if (chk === false) {
                arrMemberList.push(selectRegularMember.toString());

                for (let i = 0; i < arrMemberList.length; i++) {
                    const memberID = arrMemberList[i];

                    if (i === 0) {
                        selectMemberListData = memberID;
                    } else {
                        selectMemberListData = selectMemberListData + "," + memberID;
                    }
                }

                this.setState({ selectMemberList: selectMemberListData });
            }
        } else {
            selectMemberListData = selectRegularMember.toString();

            this.setState({ selectMemberList: selectMemberListData });
        }
    }

    displaySelectMemberList = () => {
        const selectMemberList = this.state.selectMemberList;
        const regularMembers = this.props.regularMembers;
        let displaySelectMemberList = [];
        let arrMemberList = [];

        if (selectMemberList === null || selectMemberList === undefined || selectMemberList.length === 0 ||
            regularMembers === null || regularMembers === undefined || regularMembers.length === 0)
            return displaySelectMemberList;

        arrMemberList = selectMemberList.split(",");
        let num = 1;

        for (let i = 0; i < arrMemberList.length; i++) {
            const memberID = arrMemberList[i];

            for (let j = 0; j < regularMembers.length; j++) {
                const regularMember = regularMembers[j];
                const selectRemoveMember = this.state.selectRemoveMember;
                let chkClass = "";

                if (memberID === regularMember.id.toString()) {

                    if (selectRemoveMember !== null && selectRemoveMember !== undefined && selectRemoveMember.toString() === memberID)
                        chkClass = styles.spreadCheck;

                    displaySelectMemberList.push(
                        <tr key={"displaySelectMemberList_" + regularMember.id} onClick={() => this.onClickRemoveMember(regularMember.id)}>
                            <td>{num}</td>
                            <td className={chkClass}>{regularMember.memberName}</td>
                        </tr>
                    );

                    num++;
                    break;
                }
            }

        }

        return displaySelectMemberList;
    }

    onClickRemoveMember = (id) => {
        if (id === null || id === undefined)
            return;

        this.setState({ selectRemoveMember: id});
    }

    onClickRemove = () => {
        const selectMemberList = this.state.selectMemberList;
        const id = this.state.selectRemoveMember;
        let arrMemberList = [];

        if (id === null || id === undefined ||
            selectMemberList === null || selectMemberList === undefined || selectMemberList.length === 0)
            return;

        let chk = false;
        arrMemberList = selectMemberList.split(",");

        for (let i = 0; i < arrMemberList.length; i++) {
            const memberID = arrMemberList[i];

            if (memberID === id.toString()) {
                chk = true;
                arrMemberList.splice(i, 1);
                break;
            }
        }

        if (chk === true) {
            let selectMemberListData = "";

            for (let i = 0; i < arrMemberList.length; i++) {
                const memberID = arrMemberList[i];
                
                if (i === 0) {
                    selectMemberListData = memberID;
                } else {
                    selectMemberListData = selectMemberListData + "," + memberID;
                }
            }

            this.setState({ selectMemberList: selectMemberListData });
        }
    }

    onSave = () => {
        const selectMemberList = this.state.selectMemberList;

        this.props.saveMemberList(selectMemberList);
        this.props.setVisiblePopup(this.props.popupType, false);
    }

    render() {
        const displaySelectMemberList = this.displaySelectMemberList();

        return (
            <div className={styles.spreadPersonBox}>
                <div className={styles.spreadPersonTitle} onMouseDown={(e) => this.popupDragMousePress(e)}>전파대상자 지정
                    <div className={styles.popupBoxDetailX}><a onClick={this.onClose}><img src={imgClose} alt="닫기" /></a></div>
                </div>
                <div className={styles.spreadPersonDetail}>


                    <div>
                        <span className={styles.spreadType}>전파대상자 유형</span>
                        <input type="radio" name="11" value="22" checked={this.state.optionTarget === SetTargetMembers.ManualTarget} onChange={() => this.onChangeTarget(SetTargetMembers.ManualTarget)} /><label>대상자 직접설정</label>
                        <input type="radio" name="11" value="22" checked={this.state.optionTarget === SetTargetMembers.AutomaticTarget} onChange={() => this.onChangeTarget(SetTargetMembers.AutomaticTarget)} /><label>위치기반 대상자 자동설정</label>
                    </div>

                    {/*전파대상자 지정-위치기반 대상자 자동설정*/}
                    <div className={styles.spreadTableBox2}>

                        {this.displayTarget()}


                        <div className={styles.sparrowBox2}>
                            <span className={styles.spplusArrow2} onClick={this.onClickAdd}></span>
                            <span className={styles.spminusArrow2} onClick={this.onClickRemove}></span>
                        </div>



                        <div className={styles.spreadTb7}>
                            <span>전파대상자</span>
                            <div className={styles.tableParentJ}>
                            <table className={styles.tblJ + " " + styles.scrollbar}>
                                <colgroup>
                                    <col style={{ width: "50px" }} />
                                    <col style={{ width: "90px" }} />
                                </colgroup>
                                <thead>
                                    <tr>
                                        <th>No</th>
                                        <th>이름</th>
                                    </tr>
                                </thead>
                                <tbody>
                                        {displaySelectMemberList}
                                </tbody>
                            </table>
                            </div>
                        </div>
                    </div>


                    <div className={styles.spreadConfirm}>
                        <span className={styles.Cancel} onClick={this.onClose}>취소</span>
                        <span className={styles.save} onClick={this.onSave}>저장</span>
                    </div>
                </div>
            </div>
        );

    }
} export default SetTargetMembers;