import React, { Component } from 'react';
import { BrowserRouter as Route, Link } from 'react-router-dom';
import { withRouter } from 'react-router-dom';
import styles from '../../../css/sdms.module.css';
import imgClose from '../../../image/common_Icon/popup_close.png';
import btnCalendarBk from '../../../image/history_Icon/dashboard_calendar.png';
import chartSample from '../../../image/history_Icon/chartSample.png';
import { ko } from 'date-fns/esm/locale';
import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';
import $ from 'jquery';
import SdmsResource from '../../../resource/id';
import HistoryController from '../../../services/historyController';

class SensorDetectHistory extends Component {
    
    constructor(props) {
        super(props);

        this.state = {
            content: SdmsResource.ID.menu.sensorDetectHistory,

            dataSource: null,

            // 현재 조회 데이터에 적용된 필터
            minID: 0,
            maxID: 0,
            currentLastID: 0,
            searchBeginDate: new Date(),
            searchEndDate: new Date(),
            searchFacilityType: -1,
            searchBuildingGroupID: -1,
            searchBuildingID: -1,
            searchZoneID: -1,

            searchZoneName: '-',

            selectedBuildingGroupID: -1,
            selectedBuildingID: -1,
            selectedZoneID: -1,
            dateType: 'today',
            beginDate: new Date(),
            endDate: new Date(),
            facilityType: -1,

            maxRowCount: 10,  // 한 페이지에 보여줄 data row 수
            maxPageCount: 5, // 한번에 보여줄 페이지 개수

            pageIndex: 1,    // 현재 페이지
            minPageIndex: 1, // 최소 페이지 Index
            maxPageIndex: 1, // 최대 페이지 Index
            IsKnowPageIndex: true, // 순서대로 조회할 경우 페이지 번호를 알 수 있지만 끝에서부터 조회하면 알 수 없다.

            havePrevPage: false, // 이전 데이터가 있나?
            haveAfterPage: false, // 이후 데이터가 있나?

            loadingIndicator: false, // 새로고침중인지 표시

            prevProps: null
        }

        this.props = props;

        this.currentPageMinID = -1;
        this.currentPageMaxID = -1;

        this.onClickSearch = this.onClickSearch.bind(this);
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

        $('.' + styles.hisPopupBoxX).click(function () {
            $('.' + styles.historyPopup).css('display','none');
        });

        this.initPopupState();
        this.onClickSearch();
    }

    componentDidUpdate(prevProps, prevState) {
        // 팝업이 선택 됐을 때(Drag 될때) 맨 앞에 팝업 위치
        if (this.props.zIndex !== prevProps.zIndex) {
            this.state.popup.style.zIndex = this.props.zIndex;
        }
    }

    initPopupState() {
        var popup = document.getElementsByClassName(styles.historyPopup)[0];

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

    // 드래그로 선택된 팝업과 나머지 팝업의 z-index를 조절한다. (선택된 팝업이 앞으로 나오도록)
    setActiveDragPopup = (popupType) => {
        this.props.setActiveDragPopup(this.props.popupType);
    }

    changeContent = (content) => {
        this.setState({ content });
    }

    onChangeBegin = (date) => {
        this.setState({ beginDate: date });
        $("input:radio[name='stgDate']").prop('checked', false);

        let year = date.getFullYear();
        let month = date.getMonth() + 1;
        let day = date.getDate();

        let korFormat = year + "-" + month + "-" + day;

        this.setState({ dateType: 'select' });
    }

    onChangeBuildingGroup = (target) => {
        this.setState({ selectedBuildingGroupID: Number(target.value), selectedBuildingID: -1, selectedZoneID: -1 });
    }
    onChangeBuilding = (target) => {
        this.setState({ selectedBuildingID: Number(target.value), selectedZoneID: -1 });
    }
    onChangeZone = (target) => {
        this.setState({ selectedZoneID: Number(target.value) });
    }

    onChangeEnd = (date) => {
        this.setState({ endDate: date });
        $("input:radio[name='stgDate']").prop('checked', false);

        let year = date.getFullYear();
        let month = date.getMonth() + 1;
        let day = date.getDate();

        let korFormat = year + "-" + month + "-" + day;

        this.setState({ dateType: 'select' });
    }

    onClickFacilityType = (facilityType) => {
        this.setState({ facilityType });
    }

    getMakeDateTime(dateTime) {
        let year = dateTime.getFullYear();
        let month = 1 + dateTime.getMonth();
        month = month >= 10 ? month : '0' + month;  //month 두자리로 저장
        let day = dateTime.getDate();                   //d
        day = day >= 10 ? day : '0' + day;

        let strDate = year + '-' + month + '-' + day;
        return strDate;
    }

    getMakeTime(dateTime) {
        let hour = dateTime.getHours();
        hour = hour >= 10 ? hour : '0' + hour;
        let min = dateTime.getMinutes();
        min = min >= 10 ? min : '0' + min;
        let sec = dateTime.getSeconds();
        sec = sec >= 10 ? sec : '0' + sec;

        let strDate = hour + ':' + min + ':' + sec;
        return strDate;
    }

    async getMinMaxIndex(beginDate, endDate, facilityType, buildingGroupID, buildingID, zoneID) {
        const [minID, maxID] = await HistoryController.GetMinMaxIndex(beginDate, endDate, facilityType, buildingGroupID, buildingID, zoneID);

        return [minID, maxID];
    }

    async onClickSearch() {
        $("body").css("cursor", "wait");

        const beginDate = this.getMakeDateTime(this.state.beginDate) + ' 00:00:00';
        const endDate = this.getMakeDateTime(this.state.endDate) + ' 23:59:59';

        if (beginDate > endDate) {
            $("body").css("cursor", "default");
            alert('조회 기간을 다시 선택하세요');
            return;
        }

        await this.setState({ loadingIndicator: true })

        const buildingGroupID = this.state.selectedBuildingGroupID;
        const buildingID = this.state.selectedBuildingID;
        const zoneID = this.state.selectedZoneID;
        const facilityType = this.state.facilityType;

        const [minID, maxID] = await this.getMinMaxIndex(beginDate, endDate, facilityType, buildingGroupID, buildingID, zoneID);

        console.log('maxID' + maxID);

        const [dataSource, currentLastID] = await HistoryController.DisplaySensorDetectHistories(
            beginDate, endDate, facilityType, buildingGroupID, buildingID, zoneID,
            -1, this.state.maxRowCount * this.state.maxPageCount, true);

        let havePrevPage = false;
        let haveAfterPage = false;
        if (minID < currentLastID) {
            // 뒤에 데이터 더 있음
            haveAfterPage = true;
        }

        const datacount = dataSource.length;
        const value1 = parseInt(datacount / this.state.maxRowCount);
        const value2 = datacount % this.state.maxRowCount; // 나머지가 있는 경우 페이지 하나를 추가한다.
        let maxPageIndex = value1 + ((value2 > 0) ? 1 : 0);

        let searchZoneName = '-';
        if (this.state.selectedBuildingGroupID === -1) {
            searchZoneName = '전체'
        }
        else {
            const buildingGroupLength = this.props.buildingGroupList.length;
            for (let i = 0; i < buildingGroupLength; i++) {
                const buildingGroup = this.props.buildingGroupList[i];

                if (this.state.selectedBuildingGroupID === buildingGroup.id) {
                    searchZoneName = buildingGroup.displayText;

                    if (this.state.selectedBuildingID === -1) {
                        break;
                    }

                    const buildingLength = buildingGroup.buildingDatas.length;
                    for (let j = 0; j < buildingLength; j++) {
                        const building = buildingGroup.buildingDatas[j];
                        if (this.state.selectedBuildingID === building.id) {
                            searchZoneName += ' ' + building.displayText;
                            if (this.state.selectedZoneID === -1) {
                                break;
                            }

                            const zoneLength = building.zoneDatas.length;
                            for (var k = 0; k < zoneLength; k++) {
                                const zone = building.zoneDatas[k];
                                if (this.state.selectedZoneID === zone.id) {
                                    searchZoneName += ' ' + zone.displayText;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        $("body").css("cursor", "default");

        this.setState({
            dataSource, maxPageIndex, minPageIndex: 1, pageIndex: 1, minID, maxID, IsKnowPageIndex: true, loadingIndicator: false,
            searchBeginDate: beginDate, searchEndDate: endDate, searchFacilityType: facilityType,
            searchBuildingGroupID: buildingGroupID, searchBuildingID: buildingID, searchZoneID: zoneID, searchZoneName,
            currentLastID,
            havePrevPage, haveAfterPage
        });
    }

    getSpatailUI() {
        let buildingGroupUI = [];
        let buildingUI = [];
        let zoneUI = [];

        // <option style={{ border: "none" }}>3층</option>

        buildingGroupUI.push(<option key={'buildingGroupOption_-1'} value="-1">전체</option>);
        buildingUI.push(<option key={'buildingOption_-1'} value="-1">전체</option>);
        zoneUI.push(<option key={'zoneOption_-1'} value="-1">전체</option>);

        if (!this.props.buildingGroupList) {
            return [buildingGroupUI, buildingUI, zoneUI];
        }

        const buildingGroupLength = this.props.buildingGroupList.length;
        for (let i = 0; i < buildingGroupLength; i++) {
            const buildingGroup = this.props.buildingGroupList[i];

            if (this.state.selectedBuildingGroupID === buildingGroup.id) {
                buildingGroupUI.push(<option key={'buildingGroupOption_' + buildingGroup.id} value={buildingGroup.id} selected>{buildingGroup.displayText}</option>);

                const buildingLength = buildingGroup.buildingDatas.length;
                for (let j = 0; j < buildingLength; j++) {
                    const building = buildingGroup.buildingDatas[j];
                    if (this.state.selectedBuildingID === building.id) {
                        buildingUI.push(<option key={'buildingOption_' + building.id} value={building.id} selected>{building.displayText}</option>);

                        const zoneLength = building.zoneDatas.length;
                        for (var k = 0; k < zoneLength; k++) {
                            const zone = building.zoneDatas[k];
                            if (this.state.selectedZoneID === zone.id) {
                                zoneUI.push(<option key={'zoneOption_' + zone.id} value={zone.id} selected>{zone.displayText}</option>);
                            }
                            else {
                                zoneUI.push(<option key={'zoneOption_' + zone.id} value={zone.id}>{zone.displayText}</option>);
                            }
                        }
                    }
                    else {
                        buildingUI.push(<option key={'buildingOption_' + building.id} value={building.id}>{building.displayText}</option>);
                    }
                }
            }
            else {
                buildingGroupUI.push(<option key={'buildingGroupOption_' + buildingGroup.id} value={buildingGroup.id}>{buildingGroup.displayText}</option>);
            }
        }

        return [buildingGroupUI, buildingUI, zoneUI];
    }

    getGridData() {
        let ui = [];
        if (!this.state.dataSource) {
            return [ui, false];
        }

        const dataSource = this.state.dataSource;
        const datacount = dataSource.length;

        // 데이터를 읽을 시작할 배열값
        let beginIndex = (this.state.pageIndex - this.state.minPageIndex) * this.state.maxRowCount;//0;

        for (let i = beginIndex; i < beginIndex + this.state.maxRowCount; i++) {
            if (datacount < i + 1) {
                break;
            }

            if (i == beginIndex) {
                this.currentPageMaxID = dataSource[i].sensorZoneHistoryID;
            }
            if (i == beginIndex + this.state.maxRowCount - 1) {
                this.currentPageMinID = dataSource[i].sensorZoneHistoryID;
            }
            ui.push(<tr key={'dataSource_' + (i)}>                
                <td>{/*(i + 1)*/dataSource[i].sensorZoneHistoryID}</td>
                <td>{dataSource[i].time}</td>
                <td>{dataSource[i].type}</td>                
                <td>{dataSource[i].detectType}</td>
                <td>{dataSource[i].detectInfo}</td>
                <td>{dataSource[i].alarmLevel}</td>
                <td>-</td>
            </tr>);
        }

        return ui;
    }

    // 하단 페이지 index 만들기
    getPageIndexUI() {
        let ui = [];
        if (!this.state.dataSource) {
            return ui;
        }

        if (this.state.IsKnowPageIndex) {
            for (let i = this.state.minPageIndex; i <= this.state.maxPageIndex; i++) {
                if (i === this.state.pageIndex) {
                    ui.push(<a key={'pageIndex_' + (i)} className={styles.select}>{i}</a>);
                }
                else {
                    ui.push(<a key={'pageIndex_' + (i)} onClick={() => this.setPageIndex(i)}>{i}</a>);
                }
            }
        }
        else {
            ui.push(<a key={'pageIndex_'} className={styles.select}>...</a>);
        }

        return ui;
    }

    render() {
        const [buildingGroupUI, buildingUI, zoneUI] = this.getSpatailUI();
        const pageIndexUI = this.getPageIndexUI();
        const gridUI = this.getGridData();
        return (
            <>
                <div id="popupConts" className={styles.historyPopup}>
                    <div className={styles.hisPopupBox}>
                        <div className={styles.hisPopupBoxTitle} onMouseDown={(e) => this.popupDragMousePress(e)}>이력관리</div>
                        <div className={styles.hisPopupBoxX}><a onClick={this.props.onClose}><img src={imgClose} alt="닫기" /></a></div>

                        <ul className={styles.hisTgTab}>
                            <li><a onClick={() => this.props.changeContent(SdmsResource.ID.menu.sensorDetectHistory)} className={styles.on}>이벤트 발생이력</a></li>
                            <li><a onClick={() => this.props.changeContent(SdmsResource.ID.menu.sopHistory)}>대응이력</a></li>
                            <li><a onClick={() => this.props.changeContent(SdmsResource.ID.menu.spreadHistory)}>상황전파이력</a></li>
                        </ul>

                        {/*공용 header*/}
                        {/*이벤트 발생이력*/}
                        <div className={styles.hisBoxTypeBlue}>
                            <table className={styles.hisTblNone}>
                                <colgroup>
                                    <col style={{ width: "100px" }} />
                                    <col style={{ width: "300px" }} />
                                    <col style={{ width: "100px" }} />
                                    <col style={{ width: "300px" }} />
                                    <col style={{ width: "100px" }} />
                                </colgroup>
                                <tbody>
                                    <tr className={styles.tbBorderB}>
                                        <td className={styles.hisTableHeight}>이벤트 타입</td>
                                        <td className={styles.tbBorderR}>
                                            <input type="radio" name="11" value="-1" defaultChecked /><label>전체</label>
                                            <input type="radio" name="11" value="22" /><label>화재센서</label>
                                            <input type="radio" name="11" value="22" /><label>CCTV</label>
                                            <input type="radio" name="11" value="22" /><label>저탄장화재감지기</label>
                                        </td>
                                        <td className={styles.tbBorderR}>발생위치</td>
                                        <td className={styles.tbBorderR}>
                                            <ul className={styles.hisTel3col}>
                                                <li>
                                                    <select onChange={(e) => this.onChangeBuildingGroup(e.target)} className={styles.hisBlueSell}>
                                                        {buildingGroupUI}
                                                    </select>
                                                </li>
                                                <li>
                                                    <select onChange={(e) => this.onChangeBuilding(e.target)} className={styles.hisBlueSell}>
                                                        {buildingUI}
                                                    </select>
                                                </li>
                                                <li>
                                                    <select onChange={(e) => this.onChangeZone(e.target)} className={styles.hisBlueSell}>
                                                        {zoneUI}
                                                    </select>
                                                </li>
                                            </ul>
                                        </td>
                                        <td><a onClick={this.onClickSearch} className={styles.searchBlueBtn}><span>검색</span></a></td>
                                    </tr>
                                    <tr>
                                        <td className={styles.tbBorderR}>조회기간</td>
                                        <td className={styles.tbBorderR}>
                                            <ul className={styles.hscsDate}>
                                                <li>
                                                    <div className={styles.datepicker}>
                                                        <DatePicker name="datepicker01" id="datepicker01"
                                                            dateFormat="yyyy-MM-dd"
                                                            maxDate={new Date()}
                                                            selected={this.state.beginDate}
                                                            onChange={date => this.onChangeBegin(date)} />
                                                        <img src={btnCalendarBk} alt="" className={styles.btnCalendarBk} />
                                                    </div>
                                                </li>
                                                <li>~</li>
                                                <li>
                                                    <div className={styles.datepicker}>
                                                        <DatePicker name="datepicker02" id="datepicker02"
                                                            dateFormat="yyyy-MM-dd"
                                                            locale={ko}
                                                            maxDate={new Date()}
                                                            selected={this.state.endDate}
                                                            onChange={date => this.onChangeEnd(date)} />
                                                        <img src={btnCalendarBk} alt="" className={styles.btnCalendarBk} />
                                                    </div>
                                                </li>
                                            </ul>
                                        </td>
                                        <td className={styles.tbBorderR}></td>
                                        <td></td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>

                        <div className={styles.historyGraph}>
                            <img src={chartSample} className={styles.chartSample} />
                        </div>

                        <div className={styles.gap20}></div>
                        {
                            /*
                            <div className={styles.floatL + " " + styles.searchTxt}>검색결과 : 총 00건</div>
                            */
                        }
                        <div className={styles.floatR}>
                            <a className={styles.lightNaveBtn}>엑셀 다운로드</a>
                        </div>
                        <div className={styles.gap10}></div>
                        <div className={styles.boxTypeBlue2 + " " + styles.scrollbar}>
                            <table className={styles.tblB}>
                                <colgroup>
                                    <col style={{ width: "90px" }} />
                                    <col style={{ width: "170px" }} />
                                    <col style={{ width: "160px" }} />
                                    <col style={{ width: "200px" }} />
                                    <col style={{ width: "250px" }} />
                                    <col style={{ width: "150px" }} />
                                    <col style={{ width: "150px" }} />
                                </colgroup>
                                <thead>
                                    <tr>
                                        <th>No</th>
                                        <th>일시</th>
                                        <th>이벤트 타입</th>
                                        <th>감지유형</th>
                                        <th>감지정보</th>
                                        <th>위기경보단계</th>
                                        <th>비고</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {gridUI}
                                </tbody>
                            </table>
                        </div> 

                        {
                            (this.state.dataSource && this.state.dataSource.length > 0) ?
                                <div className={styles.paging}>
                                    {
                                        (this.state.havePrevPage) ?
                                            <>
                                                <a className={styles.btnArrFirst} onClick={() => this.onClickPrevSearch(true)}></a>
                                                <a className={styles.btnArrPrev} onClick={() => this.onClickPrevSearch(false)}></a>
                                            </>
                                            :
                                            <>
                                                <a className={styles.btnArrFirstDisabled}></a>
                                                <a className={styles.btnArrPrevDisabled}></a>
                                            </>
                                    }
                                    {pageIndexUI}
                                    {
                                        (this.state.haveAfterPage) ?
                                            <>
                                                <a className={styles.btnArrNext} onClick={() => this.onClickAfterSearch(false)}></a>
                                                <a className={styles.btnArrLast} onClick={() => this.onClickAfterSearch(true)}></a>
                                            </>
                                            :
                                            <>
                                                <a className={styles.btnArrNextDisabled}></a>
                                                <a className={styles.btnArrLastDisabled}></a>
                                            </>
                                    }
                                </div>
                                : <> </>
                        }
                    </div>
                </div>
        </>
        )
    }

} export default SensorDetectHistory;