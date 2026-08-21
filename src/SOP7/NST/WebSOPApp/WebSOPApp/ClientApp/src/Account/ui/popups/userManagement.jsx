import React, { Component } from 'react';
import { BrowserRouter as Route, Link } from 'react-router-dom';
import { withRouter } from 'react-router-dom';
import styles from '../../../SDMS/css/sdms.module.css';
import imgClose from '../../../SDMS/image/common_Icon/close_x.png';
import UserDelete from './userDelete';
import UserDetail from './userDetail';


class UserManagement extends Component {
    static Management = 0;
    static Register = 1;

    constructor(props) {
        super(props);

        this.state = {
            visiblePopup: {
                userDelete: false
            },
            menu: UserManagement.Management,
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

        this.initPopupState();
    }

    initPopupState() {
        var popup = document.getElementsByClassName(styles.loginPopup)[0];
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
        }
    }
    // 팝업 드래그 종료(mouse up)
    popupDragMouseUp = () => {
        console.log('popup drag false')
        document.removeEventListener('mousemove', this.popupDragMouseMove);
        document.removeEventListener('mouseup', this.popupDragMouseUp);
    }

    onClose = () => {
        let menu = this.state.menu;
        
        if (menu === UserManagement.Register) {
            this.setState({ menu: UserManagement.Management});
            return;
        } else {
            this.props.setVisiblePopup(this.props.popupType, false);
            return;
        }
    }

    onClickUserDelete = () => {
        let visiblePopup = this.state.visiblePopup;
        visiblePopup.userDelete = true;

        this.setState({ visiblePopup});
    }

    setVisiblePopup = (popup, visible) => {
        const visiblePopup = { ...this.state.visiblePopup };
        visiblePopup[popup] = visible;
        this.setState({ visiblePopup });
    }

    onClickRegister = () => {
        this.setState({ menu: UserManagement.Register})
    }

    onDoubleClickDetail = () => {
        let visiblePopup = this.state.visiblePopup;
        visiblePopup.userDetail = true;

        this.setState({ visiblePopup });
    }

    displayMenu = () => {
        let menu = this.state.menu;
        let displayMenu = [];

        if (menu === UserManagement.Management) {
            displayMenu.push(
                <>
                    <div className={styles.gap20}></div>
                    <div className={styles.floatL + " " + styles.searchTxt}>검색결과 : 총 00명</div>
                    <div className={styles.floatR}>
                        <a className={styles.lightNaveBtn} onClick={this.onClickUserDelete}>삭제</a>
                        <a className={styles.lightBlueBtn} onClick={this.onClickRegister}>등록</a>
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
                                    <th>이름</th>
                                    <th>부서</th>
                                    <th>권한</th>
                                    <th>연락처</th>
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
                                    <td>1</td>
                                    <td onDoubleClick={this.onDoubleClickDetail}>가나다</td>
                                    <td>방재팀</td>
                                    <td>총괄관리자</td>
                                    <td>010-1234-5678</td>
                                    <td>비고</td>
                                </tr>
                                <tr>
                                    <td>
                                        <label className={styles.checkboxCssEtc}>
                                            <input type="checkbox" />
                                            <span className={styles.checkmarkEtc}></span>
                                        </label>
                                    </td>
                                    <td>2</td>
                                    <td onDoubleClick={this.onDoubleClickDetail}>가나다</td>
                                    <td>방재팀</td>
                                    <td>총괄관리자</td>
                                    <td>010-1234-5678</td>
                                    <td>비고</td>
                                </tr>
                                <tr>
                                    <td>
                                        <label className={styles.checkboxCssEtc}>
                                            <input type="checkbox" />
                                            <span className={styles.checkmarkEtc}></span>
                                        </label>
                                    </td>
                                    <td>3</td>
                                    <td onDoubleClick={this.onDoubleClickDetail}>가나다</td>
                                    <td>방재팀</td>
                                    <td>총괄관리자</td>
                                    <td>010-1234-5678</td>
                                    <td>비고</td>
                                </tr>
                                <tr>
                                    <td>
                                        <label className={styles.checkboxCssEtc}>
                                            <input type="checkbox" />
                                            <span className={styles.checkmarkEtc}></span>
                                        </label>
                                    </td>
                                    <td onDoubleClick={this.onDoubleClickDetail}>4</td>
                                    <td>가나다</td>
                                    <td>방재팀</td>
                                    <td>총괄관리자</td>
                                    <td>010-1234-5678</td>
                                    <td>비고</td>
                                </tr>
                                <tr>
                                    <td>
                                        <label className={styles.checkboxCssEtc}>
                                            <input type="checkbox" />
                                            <span className={styles.checkmarkEtc}></span>
                                        </label>
                                    </td>
                                    <td>5</td>
                                    <td>가나다</td>
                                    <td>방재팀</td>
                                    <td>총괄관리자</td>
                                    <td>010-1234-5678</td>
                                    <td>비고</td>
                                </tr>
                                <tr>
                                    <td>
                                        <label className={styles.checkboxCssEtc}>
                                            <input type="checkbox" />
                                            <span className={styles.checkmarkEtc}></span>
                                        </label>
                                    </td>
                                    <td>6</td>
                                    <td>가나다</td>
                                    <td>방재팀</td>
                                    <td>총괄관리자</td>
                                    <td>010-1234-5678</td>
                                    <td>비고</td>
                                </tr>
                                <tr>
                                    <td>
                                        <label className={styles.checkboxCssEtc}>
                                            <input type="checkbox" />
                                            <span className={styles.checkmarkEtc}></span>
                                        </label>
                                    </td>
                                    <td>7</td>
                                    <td>가나다</td>
                                    <td>방재팀</td>
                                    <td>총괄관리자</td>
                                    <td>010-1234-5678</td>
                                    <td>비고</td>
                                </tr>
                                <tr>
                                    <td>
                                        <label className={styles.checkboxCssEtc}>
                                            <input type="checkbox" />
                                            <span className={styles.checkmarkEtc}></span>
                                        </label>
                                    </td>
                                    <td>8</td>
                                    <td>가나다</td>
                                    <td>방재팀</td>
                                    <td>총괄관리자</td>
                                    <td>010-1234-5678</td>
                                    <td>비고</td>
                                </tr>
                                <tr>
                                    <td>
                                        <label className={styles.checkboxCssEtc}>
                                            <input type="checkbox" />
                                            <span className={styles.checkmarkEtc}></span>
                                        </label>
                                    </td>
                                    <td>9</td>
                                    <td>가나다</td>
                                    <td>방재팀</td>
                                    <td>총괄관리자</td>
                                    <td>010-1234-5678</td>
                                    <td>비고</td>
                                </tr>
                                <tr>
                                    <td>
                                        <label className={styles.checkboxCssEtc}>
                                            <input type="checkbox" />
                                            <span className={styles.checkmarkEtc}></span>
                                        </label>
                                    </td>
                                    <td>10</td>
                                    <td>가나다</td>
                                    <td>방재팀</td>
                                    <td>총괄관리자</td>
                                    <td>010-1234-5678</td>
                                    <td>비고</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </>
            );
        } else {
            displayMenu.push(
                <>
                    <div className={styles.gap20}></div>
                    <div className={styles.floatL + " " + styles.searchText}>검색결과 : 총 00명</div>
                    <div className={styles.floatL + " " + styles.searchTxtt}>수정결과 : 총 00명</div>
                    <div className={styles.gap10}></div>
                    <div className={styles.userBox + " " + styles.scrollbar}>
                        <table className={styles.tblC}>
                            <colgroup>
                                <col style={{ width: "20px" }} />
                                <col style={{ width: "60px" }} />
                                <col style={{ width: "80px" }} />
                                <col style={{ width: "80px" }} />
                                <col style={{ width: "110px" }} />
                                <col style={{ width: "110px" }} />
                            </colgroup>
                            <thead>
                                <tr>
                                    <th>
                                        <label className={styles.checkboxCssEtc}>
                                            <input type="checkbox" className={styles.userCheckBox} />
                                            <span className={styles.checkmarkEtc}></span>
                                        </label>
                                    </th>
                                    <th>No</th>
                                    <th>이름</th>
                                    <th>부서</th>
                                    <th>권한</th>
                                    <th>연락처</th>
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
                                    <td>1</td>
                                    <td>가나다</td>
                                    <td>방재팀</td>
                                    <td>
                                        <select className={styles.blueSel + " " + styles.w100p}>
                                            <option>미등록</option>
                                            <option>총괄관리자</option>
                                            <option>일반관리자</option>
                                            <option>사용자</option>
                                        </select>
                                    </td>
                                    <td>010-1234-5678</td>
                                </tr>
                                <tr>
                                    <td>
                                        <label className={styles.checkboxCssEtc}>
                                            <input type="checkbox" />
                                            <span className={styles.checkmarkEtc}></span>
                                        </label>
                                    </td>
                                    <td>2</td>
                                    <td>가나다</td>
                                    <td>방재팀</td>
                                    <td>
                                        <select className={styles.blueSel}>
                                            <option>미등록</option>
                                            <option>총괄관리자</option>
                                            <option>일반관리자</option>
                                            <option>사용자</option>
                                        </select>
                                    </td>
                                    <td>010-1234-5678</td>
                                </tr>
                                <tr>
                                    <td>
                                        <label className={styles.checkboxCssEtc}>
                                            <input type="checkbox" />
                                            <span className={styles.checkmarkEtc}></span>
                                        </label>
                                    </td>
                                    <td>3</td>
                                    <td>가나다</td>
                                    <td>방재팀</td>
                                    <td>
                                        <select className={styles.blueSel}>
                                            <option>미등록</option>
                                            <option>총괄관리자</option>
                                            <option>일반관리자</option>
                                            <option>사용자</option>
                                        </select>
                                    </td>
                                    <td>010-1234-5678</td>
                                </tr>
                                <tr>
                                    <td>
                                        <label className={styles.checkboxCssEtc}>
                                            <input type="checkbox" />
                                            <span className={styles.checkmarkEtc}></span>
                                        </label>
                                    </td>
                                    <td>4</td>
                                    <td>가나다</td>
                                    <td>방재팀</td>
                                    <td>
                                        <select className={styles.blueSel}>
                                            <option>미등록</option>
                                            <option>총괄관리자</option>
                                            <option>일반관리자</option>
                                            <option>사용자</option>
                                        </select>
                                    </td>
                                    <td>010-1234-5678</td>
                                </tr>
                                <tr>
                                    <td>
                                        <label className={styles.checkboxCssEtc}>
                                            <input type="checkbox" />
                                            <span className={styles.checkmarkEtc}></span>
                                        </label>
                                    </td>
                                    <td>4</td>
                                    <td>가나다</td>
                                    <td>방재팀</td>
                                    <td>
                                        <select className={styles.blueSel}>
                                            <option>미등록</option>
                                            <option>총괄관리자</option>
                                            <option>일반관리자</option>
                                            <option>사용자</option>
                                        </select>
                                    </td>
                                    <td>010-1234-5678</td>
                                </tr>
                                <tr>
                                    <td>
                                        <label className={styles.checkboxCssEtc}>
                                            <input type="checkbox" />
                                            <span className={styles.checkmarkEtc}></span>
                                        </label>
                                    </td>
                                    <td>4</td>
                                    <td>가나다</td>
                                    <td>방재팀</td>
                                    <td>
                                        <select className={styles.blueSel}>
                                            <option>미등록</option>
                                            <option>총괄관리자</option>
                                            <option>일반관리자</option>
                                            <option>사용자</option>
                                        </select>
                                    </td>
                                    <td>010-1234-5678</td>
                                </tr>
                                <tr>
                                    <td>
                                        <label className={styles.checkboxCssEtc}>
                                            <input type="checkbox" />
                                            <span className={styles.checkmarkEtc}></span>
                                        </label>
                                    </td>
                                    <td>4</td>
                                    <td>가나다</td>
                                    <td>방재팀</td>
                                    <td>
                                        <select className={styles.blueSel}>
                                            <option>미등록</option>
                                            <option>총괄관리자</option>
                                            <option>일반관리자</option>
                                            <option>사용자</option>
                                        </select>
                                    </td>
                                    <td>010-1234-5678</td>
                                </tr>
                                <tr>
                                    <td>
                                        <label className={styles.checkboxCssEtc}>
                                            <input type="checkbox" />
                                            <span className={styles.checkmarkEtc}></span>
                                        </label>
                                    </td>
                                    <td>4</td>
                                    <td>가나다</td>
                                    <td>방재팀</td>
                                    <td>
                                        <select className={styles.blueSel}>
                                            <option>미등록</option>
                                            <option>총괄관리자</option>
                                            <option>일반관리자</option>
                                            <option>사용자</option>
                                        </select>
                                    </td>
                                    <td>010-1234-5678</td>
                                </tr>
                                <tr>
                                    <td>
                                        <label className={styles.checkboxCssEtc}>
                                            <input type="checkbox" />
                                            <span className={styles.checkmarkEtc}></span>
                                        </label>
                                    </td>
                                    <td>4</td>
                                    <td>가나다</td>
                                    <td>방재팀</td>
                                    <td>
                                        <select className={styles.blueSel}>
                                            <option>미등록</option>
                                            <option>총괄관리자</option>
                                            <option>일반관리자</option>
                                            <option>사용자</option>
                                        </select>
                                    </td>
                                    <td>010-1234-5678</td>
                                </tr>
                                <tr>
                                    <td>
                                        <label className={styles.checkboxCssEtc}>
                                            <input type="checkbox" />
                                            <span className={styles.checkmarkEtc}></span>
                                        </label>
                                    </td>
                                    <td>4</td>
                                    <td>가나다</td>
                                    <td>방재팀</td>
                                    <td>
                                        <select className={styles.blueSel}>
                                            <option>미등록</option>
                                            <option>총괄관리자</option>
                                            <option>일반관리자</option>
                                            <option>사용자</option>
                                        </select>
                                    </td>
                                    <td>010-1234-5678</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <div className={styles.arrowBox}>
                        <span className={styles.plusArrow}></span>
                        <span className={styles.minusArrow}></span>
                    </div>
                    <div className={styles.userBox2 + " " + styles.scrollbar}>
                        <table className={styles.tblD}>
                            <colgroup>
                                <col style={{ width: "20px" }} />
                                <col style={{ width: "60px" }} />
                                <col style={{ width: "80px" }} />
                                <col style={{ width: "80px" }} />
                                <col style={{ width: "110px" }} />
                                <col style={{ width: "110px" }} />
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
                                    <th>이름</th>
                                    <th>부서</th>
                                    <th>권한</th>
                                    <th>연락처</th>
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
                                    <td>1</td>
                                    <td>가나다</td>
                                    <td>방재팀</td>
                                    <td>총괄관리자</td>
                                    <td>010-1234-5678</td>
                                </tr>
                                <tr>
                                    <td>
                                        <label className={styles.checkboxCssEtc}>
                                            <input type="checkbox" />
                                            <span className={styles.checkmarkEtc}></span>
                                        </label>
                                    </td>
                                    <td>2</td>
                                    <td>가나다</td>
                                    <td>방재팀</td>
                                    <td>총괄관리자</td>
                                    <td>010-1234-5678</td>
                                </tr>
                                <tr>
                                    <td>
                                        <label className={styles.checkboxCssEtc}>
                                            <input type="checkbox" />
                                            <span className={styles.checkmarkEtc}></span>
                                        </label>
                                    </td>
                                    <td>3</td>
                                    <td>가나다</td>
                                    <td>방재팀</td>
                                    <td>총괄관리자</td>
                                    <td>010-1234-5678</td>
                                </tr>
                                <tr>
                                    <td>
                                        <label className={styles.checkboxCssEtc}>
                                            <input type="checkbox" />
                                            <span className={styles.checkmarkEtc}></span>
                                        </label>
                                    </td>
                                    <td>4</td>
                                    <td>가나다</td>
                                    <td>방재팀</td>
                                    <td>총괄관리자</td>
                                    <td>010-1234-5678</td>
                                </tr>
                                <tr>
                                    <td>
                                        <label className={styles.checkboxCssEtc}>
                                            <input type="checkbox" />
                                            <span className={styles.checkmarkEtc}></span>
                                        </label>
                                    </td>
                                    <td>5</td>
                                    <td>가나다</td>
                                    <td>방재팀</td>
                                    <td>총괄관리자</td>
                                    <td>010-1234-5678</td>
                                </tr>
                                <tr>
                                    <td>
                                        <label className={styles.checkboxCssEtc}>
                                            <input type="checkbox" />
                                            <span className={styles.checkmarkEtc}></span>
                                        </label>
                                    </td>
                                    <td>6</td>
                                    <td>가나다</td>
                                    <td>방재팀</td>
                                    <td>총괄관리자</td>
                                    <td>010-1234-5678</td>
                                </tr>
                                <tr>
                                    <td>
                                        <label className={styles.checkboxCssEtc}>
                                            <input type="checkbox" />
                                            <span className={styles.checkmarkEtc}></span>
                                        </label>
                                    </td>
                                    <td>7</td>
                                    <td>가나다</td>
                                    <td>방재팀</td>
                                    <td>총괄관리자</td>
                                    <td>010-1234-5678</td>
                                </tr>
                                <tr>
                                    <td>
                                        <label className={styles.checkboxCssEtc}>
                                            <input type="checkbox" />
                                            <span className={styles.checkmarkEtc}></span>
                                        </label>
                                    </td>
                                    <td>8</td>
                                    <td>가나다</td>
                                    <td>방재팀</td>
                                    <td>총괄관리자</td>
                                    <td>010-1234-5678</td>
                                </tr>
                                <tr>
                                    <td>
                                        <label className={styles.checkboxCssEtc}>
                                            <input type="checkbox" />
                                            <span className={styles.checkmarkEtc}></span>
                                        </label>
                                    </td>
                                    <td>9</td>
                                    <td>가나다</td>
                                    <td>방재팀</td>
                                    <td>총괄관리자</td>
                                    <td>010-1234-5678</td>
                                </tr>
                                <tr>
                                    <td>
                                        <label className={styles.checkboxCssEtc}>
                                            <input type="checkbox" />
                                            <span className={styles.checkmarkEtc}></span>
                                        </label>
                                    </td>
                                    <td>10</td>
                                    <td>가나다</td>
                                    <td>방재팀</td>
                                    <td>총괄관리자</td>
                                    <td>010-1234-5678</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </>);
        }

        return displayMenu;
    }

    render() {
        let displayMenu = this.displayMenu();

        return (
            <div>
                
                <div id="popupConts" className={styles.loginPopup}>
                    <div className={styles.popupBox}>
                        <div className={styles.popupBoxTitle} onMouseDown={(e) => this.popupDragMousePress(e)}>사용자 관리</div>
                        <div className={styles.popupBoxX}><a onClick={this.onClose}><img src={imgClose} alt="닫기" /></a></div>

                        {/*공용 header*/}
                        <div className={styles.boxTypeBlue}>
                            <table className={styles.tblNone}>
                                <colgroup>
                                    <col style={{ width: "100px" }} />
                                    <col style={{ width: "300px" }} />
                                    <col style={{ width: "100px" }} />
                                    <col style={{ width: "300px" }} />
                                    <col style={{ width: "100px" }} />
                                </colgroup>
                                <tbody>
                                    <tr>
                                        <td className={styles.tableHeight}>이름</td>
                                        <td><input type="text" className={styles.blueInput + " " + styles.w90p} placeholder="이름을 입력하세요." /></td>
                                    <td>연락처</td>
                                    <td>
                                        <ul className={styles.tel3col}>
                                            <li>
                                                <select className={styles.blueSellSelect}>
                                                   <option style={{ border : "none" }}>선택</option>
                                                </select><span>-</span>
                                            </li>
                                            <li>
                                                <input type="text" className={styles.blueInput + " " + styles.w100p} />
                                            </li>
                                            <li>
                                                <input type="text" className={styles.blueInput + " " + styles.w100p} />
                                            </li>
                                        </ul>
                                    </td>
                                        <td rowspan="2"><a href="#" className={styles.searchBlueBtn2}><span>검색</span></a></td>
                                    </tr>
                                <tr>
                                    <td>부서</td>
                                    <td>
                                        <select className={styles.blueSell + " " + styles.w90p}>
                                           <option>전체</option>
                                        </select>
                                    </td>
                                    <td>권한</td>
                                    <td>
                                        <select className={styles.blueSell + " " + styles.w100p}>
                                                <option>전체</option>
                                                <option>미등록</option>
                                                <option>총괄관리자</option>
                                                <option>일반관리자</option>
                                                <option>사용자</option>
                                        </select>
                                    </td>
                                </tr>
                                </tbody>
                            </table>
                        </div>




                        {displayMenu}



                    <div className={styles.paging4}>
                        <a href="#" className={styles.btnArrFirst}></a>
                        <a href="#" className={styles.btnArrPrev}></a>
                            <a className={styles.select} href="#">1</a>
                            <a href="#">2</a>
                            <a href="#">3</a>
                            <a href="#">4</a>
                            <a href="#">5</a>
                            <a href="#">6</a>
                            <a href="#">7</a>
                            <a href="#">8</a>
                            <a href="#">9</a>
                            <a href="#">10</a>
                        <a href="#" className={styles.btnArrNext}></a>
                        <a href="#" className={styles.btnArrLast}></a>
                    </div>
                </div>
             </div>
                

                {
                    this.state.visiblePopup.userDelete &&
                    <UserDelete
                        popupType="userDelete"
                        setVisiblePopup={this.setVisiblePopup}
                    />
                }

                {
                    this.state.visiblePopup.userDetail &&
                    <UserDetail
                        popupType="userDetail"
                        setVisiblePopup={this.setVisiblePopup}
                    />
                }
        </div>
       )
    }

} export default UserManagement;