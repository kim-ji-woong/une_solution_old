import React, { Component } from 'react';
import { BrowserRouter as Route, Link } from 'react-router-dom';
import { withRouter } from 'react-router-dom';
import styles from '../../../css/sdms.module.css';
import imgClose from '../../../image/common_Icon/popup_close.png';
import btnCalendarBk from '../../../image/history_Icon/dashboard_calendar.png';

import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';
import $ from 'jquery';
import SdmsResource from '../../../resource/id';

class SOPHistory extends Component {
    
    constructor(props) {
        super(props);

        this.state = {
            content: SdmsResource.ID.menu.sensorDetectHistory,
            buildingGroupList: null,
            prevProps: null
        }

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

        $('.' + styles.hisPopupBoxX).click(function () {
            $('.' + styles.historyPopup).css('display','none');
        });

        this.initPopupState();
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

    render() {
        
        return (
            <>
                <div id="popupConts" className={styles.historyPopup}>
                    <div className={styles.hisPopupBox}>
                        <div className={styles.hisPopupBoxTitle} onMouseDown={(e) => this.popupDragMousePress(e)}>이력관리</div>
                        <div className={styles.hisPopupBoxX}><a onClick={this.props.onClose}><img src={imgClose} alt="닫기" /></a></div>

                        <ul className={styles.hisTgTab}>
                            <li><a onClick={() => this.props.changeContent(SdmsResource.ID.menu.sensorDetectHistory)}>이벤트 발생이력</a></li>
                            <li><a onClick={() => this.props.changeContent(SdmsResource.ID.menu.sopHistory)} className={styles.on}>대응이력</a></li>
                            <li><a onClick={() => this.props.changeContent(SdmsResource.ID.menu.spreadHistory)}>상황전파이력</a></li>
                        </ul>
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
                                                    <select className={styles.hisBlueSell}>
                                                        <option style={{ border: "none" }}>건물명</option>
                                                    </select>
                                                </li>
                                                <li>
                                                    <select className={styles.hisBlueSell}>
                                                        <option>1동</option>
                                                        <option style={{ border: "none" }}>2동</option>
                                                        <option style={{ border: "none" }}>3동</option>
                                                    </select>
                                                </li>
                                                <li>
                                                    <select className={styles.hisBlueSell}>
                                                        <option style={{ border: "none" }}>1층</option>
                                                        <option style={{ border: "none" }}>2층</option>
                                                        <option style={{ border: "none" }}>3층</option>
                                                    </select>
                                                </li>
                                            </ul>
                                        </td>
                                        <td><a className={styles.searchBlueBtn}><span>검색</span></a></td>
                                    </tr>
                                    <tr>
                                        <td className={styles.tbBorderR}>조회기간</td>
                                        <td className={styles.tbBorderR}>
                                            <ul className={styles.hscsDate}>
                                                <li>
                                                    <div className={styles.datepicker}>
                                                        <DatePicker name="datepicker01" id="datepicker01"
                                                            dateFormat="yyyy-MM-dd" />
                                                        <img src={btnCalendarBk} alt="" className={styles.btnCalendarBk} />
                                                    </div>
                                                </li>
                                                <li>~</li>
                                                <li>
                                                    <div className={styles.datepicker}>
                                                        <DatePicker name="datepicker02" id="datepicker02"
                                                            dateFormat="yyyy-MM-dd" />
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

                        <div className={styles.gap20}></div>
                        <div className={styles.floatL + " " + styles.searchTxt}>검색결과 : 총 00명</div>
                        <div className={styles.floatR}>
                            <a className={styles.lightNaveBtn}>엑셀 다운로드</a>
                        </div>
                        <div className={styles.gap10}></div>
                        <div className={styles.boxTypeBlue2 + " " + styles.scrollbar}>
                            <table className={styles.tblB}>
                                <colgroup>
                                    <col style={{ width: "20px" }} />
                                    <col style={{ width: "90px" }} />
                                    <col style={{ width: "150px" }} />
                                    <col style={{ width: "160px" }} />
                                    <col style={{ width: "200px" }} />
                                    <col style={{ width: "250px" }} />
                                    <col style={{ width: "150px" }} />
                                    <col style={{ width: "150px" }} />
                                </colgroup>
                                <thead>
                                    <tr>
                                        <th>
                                            <label className={styles.checkboxCssEtc}>
                                                <input type="checkbox" />
                                                <span className={styles.checkmarkEtc}></span>
                                            </label>
                                        </th>
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
                                    <tr>
                                        <td>
                                            <label className={styles.checkboxCssEtc}>
                                                <input type="checkbox" />
                                                <span className={styles.checkmarkEtc}></span>
                                            </label>
                                        </td>
                                        <td>5</td>
                                        <td>21.01.01 14:20:11</td>
                                        <td>화재</td>
                                        <td>센서감지</td>
                                        <td>오작동</td>
                                        <td>관심</td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label className={styles.checkboxCssEtc}>
                                                <input type="checkbox" />
                                                <span className={styles.checkmarkEtc}></span>
                                            </label>
                                        </td>
                                        <td>4</td>
                                        <td>21.01.01 14:20:11</td>
                                        <td>화재</td>
                                        <td>센서감지</td>
                                        <td>오작동</td>
                                        <td>관심</td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label className={styles.checkboxCssEtc}>
                                                <input type="checkbox" />
                                                <span className={styles.checkmarkEtc}></span>
                                            </label>
                                        </td>
                                        <td>3</td>
                                        <td>21.01.01 14:20:11</td>
                                        <td>화재</td>
                                        <td>센서감지</td>
                                        <td>오작동</td>
                                        <td>관심</td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label className={styles.checkboxCssEtc}>
                                                <input type="checkbox" />
                                                <span className={styles.checkmarkEtc}></span>
                                            </label>
                                        </td>
                                        <td>2</td>
                                        <td>21.01.01 14:20:11</td>
                                        <td>화재</td>
                                        <td>센서감지</td>
                                        <td>오작동</td>
                                        <td>관심</td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label className={styles.checkboxCssEtc}>
                                                <input type="checkbox" />
                                                <span className={styles.checkmarkEtc}></span>
                                            </label>
                                        </td>
                                        <td>1</td>
                                        <td>21.01.01 14:20:11</td>
                                        <td>화재</td>
                                        <td>센서감지</td>
                                        <td>오작동</td>
                                        <td>관심</td>
                                        <td></td>
                                    </tr>
                                </tbody>
                            </table>
                        </div> 

                        <div className={styles.paging2}>
                            <a className={styles.btnArrFirst}></a>
                            <a className={styles.btnArrPrev}></a>
                            <a className={styles.select}>1</a>
                            <a>2</a>
                            <a>3</a>
                            <a>4</a>
                            <a>5</a>
                            <a>6</a>
                            <a>7</a>
                            <a>8</a>
                            <a>9</a>
                            <a>10</a>
                            <a className={styles.btnArrNext}></a>
                            <a className={styles.btnArrLast}></a>
                        </div>
                    </div>
                </div>
        </>
        )
    }

} export default SOPHistory;