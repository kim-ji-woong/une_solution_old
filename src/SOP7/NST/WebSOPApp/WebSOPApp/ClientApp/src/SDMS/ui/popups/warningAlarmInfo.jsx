import React, { Component } from 'react';
import { BrowserRouter as Route, Link } from 'react-router-dom';
import { withRouter } from 'react-router-dom';
import styles from '../../css/sdms.module.css';
import imgPrev from '../../image/prev.png';
import imgBack from '../../image/back.png';
import StringUtil from '../../../Common/util/StringUtil';
//import icAlarmDanger from '../image/warning_Icon/dangerAlarm01-04.png';
import $ from 'jquery';


class WarningAlarmInfo extends Component {
    constructor(props) {
        super(props);

        this.state = {
            popupMinWidth: 320,
            popupMinHeight: 600,
            searchText: ''
        }

        this.props = props;

        this.onMalfunction = this.onMalfunction.bind(this);
        this.onSound = this.onSound.bind(this);
    }

    componentDidMount() {

        $(function () {
            $('.' + styles.btnMd1).mouseenter(function () {
                $('.' + styles.screenChange).show();
            })
            $('.' + styles.btnMd1).mouseleave(function () {
                $('.' + styles.screenChange).hide();
            })
        });

        $(function () {
            $('.' + styles.btnMd2).mouseenter(function () {
                $('.' + styles.alarmOff).show();
            });
            $('.' + styles.btnMd2).mouseleave(function () {
                $('.' + styles.alarmOff).hide();
            });
        });

        $(function () {
            $('.' + styles.btnMd3).mouseenter(function () {
                $('.' + styles.powerOff).show();
            });
            $('.' + styles.btnMd3).mouseleave(function () {
                $('.' + styles.powerOff).hide();
            });
        });


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
        /*const rect = this.refScrollArea.current.getBoundingClientRect();

        let scrollVisible = false;

        if (this.refTree.current) {
            const rectTree = this.refTree.current.getBoundingClientRect();

            if (rectTree.height > rect.height) {
                scrollVisible = true;
            }
        }

        SdmsScrollbar.setContentStyle(this.refScrollbar.current, rect.width, rect.height, scrollVisible);*/
    }

    initPopupState() {
        var popup = document.getElementsByClassName(styles.popupContainer + " " + styles.popupHasTitle)[0];

        //DB에 값이 있을 경우에만
        if (typeof this.props.popupState !== 'undefined') {
            popup.style.left = this.props.popupState.x;
            popup.style.top = this.props.popupState.y;
            popup.style.width = this.props.popupState.width;
            popup.style.height = this.props.popupState.height;
        } else {
            popup.style.left = "76.5%";
            popup.style.top = "10%";
            popup.style.width = "400px";
            popup.style.height = "640px";
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
        if (/*perX > 0 && */perY > 0 && width > 0 && height > 0) {
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

    onSelectedAlarm(alarm) {
        this.props.onSelectedAlarm(alarm);
    }
    onMoveSelectedAlarm() {
        this.props.onMoveSelectedAlarm();
    }
    async onMalfunction() {
        const alarm = this.props.selectedAlarm;
        if (alarm.isAlarm) {
            this.props.onMalfunction(alarm);
        }
    }
    onSound() {
        if (this.props.selectedAlarm.sound === undefined) {
            this.props.selectedAlarm.sound = true;
        }

        this.props.selectedAlarm.sound = !this.props.selectedAlarm.sound;

        if (this.props.onSound) {
            this.props.onSound(this.props.selectedAlarm.sound);
        }
    }

    setGridUI() {
        if (!this.props.sensorAlarms || this.props.sensorAlarms === null || this.props.sensorAlarms.length === 0)
            return null;

        var grid = [];


        const alarms = this.props.sensorAlarms;
        for (let i = 0; i < alarms.length; i++) {
            const alarm = alarms[i];
            const dt = new Date(alarm.dtTime);
            const yyyy = dt.getFullYear();
            const mm = dt.getMonth() + 1;
            const dd = dt.getDate();
            const ss = dt.getSeconds();
            const ymd = String(yyyy).substring(2) + '.' + StringUtil.getDoubleString(mm) + '.' + StringUtil.getDoubleString(dd);
            const hms = StringUtil.getDoubleString(dt.getHours()) + ':' + StringUtil.getDoubleString(dt.getMinutes()) + ':' + StringUtil.getDoubleString(ss);
            const ymdHms = ymd + ' ' + hms;

            let sopStatusText = '미대응';
            let statusClassName = '';
            if (alarm.sopStatus === 1) {
                sopStatusText = '대응중';
            }
            else if (alarm.sopStatus === 2) {
                sopStatusText = '상황종료';
            }

            let alarmDepth = '주의'
            if (alarm.alarmDepth === 1) {
                alarmDepth = '관심';
            }
            else if (alarm.alarmDepth === 3) {
                alarmDepth = '경계';
            }
            else if (alarm.alarmDepth === 4) {
                alarmDepth = '심각';
            }

            if (alarm.isAlarm === false) {
                sopStatusText = '알람종료';
            }
            else {
                statusClassName = styles.red;
            }

            let rowIndex = i + 1;
            let dataViewName = "detailWarning" + rowIndex;

            grid.push(
                <tr data-view={dataViewName} key={i} onClick={() => this.onSelectedAlarm(alarm)} onDoubleClick={() => this.onMoveSelectedAlarm()}>
                    <td>{rowIndex}</td>
                    <td>{ymdHms}</td>
                    <td>{alarm.facilityTypeString}</td>
                    <td>{alarmDepth}</td>
                    <td><span className={statusClassName}>{sopStatusText}</span></td>
                </tr>
            );
        }

        return grid;
    }

    render() {
        const gridUI = this.setGridUI();

        let selectedAlarmDate = '';
        let selectedIsAlarm = '';
        let selectedAlarmMessage = '';

        if (this.props.selectedAlarm) {
            selectedAlarmDate = this.props.selectedAlarm.strDateTime.substring(0, 10);
            selectedIsAlarm = (!this.props.selectedAlarm.isAlarm) ? '[종료 알람]' : '';
            selectedAlarmMessage = this.props.selectedAlarm.message;
        }

        return (
         <>
                {/* <aside className={styles.popupWrap + " " + styles.popupSm + " " + styles.themeDark + " " + "is-Open"}> */}
                    <section className={styles.popupContainer + " " + styles.popupHasTitle}>
                        <div className={styles.popupSizingAreaRight} onMouseDown={(e) => this.popupResizeMousePress(e, 'h-r')} ></div>
                        <div className={styles.popupSizingAreaLeft} onMouseDown={(e) => this.popupResizeMousePress(e, 'h-l')}></div>
                        <div className={styles.popupSizingAreaBottom} onMouseDown={(e) => this.popupResizeMousePress(e, 'v-b')}></div>
                        <div className={styles.popupSizingAreaTop} onMouseDown={(e) => this.popupResizeMousePress(e, 'v-t')}></div>
                        <div className={styles.popupSizingAreaRightTopPoint} onMouseDown={(e) => this.popupResizeMousePress(e, 'd-rt')}></div>
                        <div className={styles.popupSizingAreaRightBottomPoint} onMouseDown={(e) => this.popupResizeMousePress(e, 'd-rb')}></div>
                        <div className={styles.popupSizingAreaLeftTopPoint} onMouseDown={(e) => this.popupResizeMousePress(e, 'd-lt')}></div>
                        <div className={styles.popupSizingAreaLeftBottomPoint} onMouseDown={(e) => this.popupResizeMousePress(e, 'd-lb')}></div>

                        <header className={styles.popupHeader}>
                            <h3 className={styles.popupHeaderTitle} onMouseDown={(e) => this.popupDragMousePress(e)}>경고/알람</h3>
                            <button type="button" className={styles.btnPopupClose} onClick={this.onClose}>팝업닫기</button>
                        </header>
                        <div className={styles.popupBody + "scroll-none"}>
                            <header className={styles.listHeader}>
                                <h3 className={styles.listHeaderTitle}>경고/알람 리스트</h3>
                        </header>
                        <table className={styles.table}>
                            <caption>경고/알람</caption>
                            <colgroup>
                                <col style={{ width: '11%' }} />
                                <col style={{ width: '36%' }} />
                                <col style={{ width: '19%' }} />
                                <col style={{ width: '21%' }} />
                                <col style={{ width: '17%' }} />
                            </colgroup>
                            <thead style={{ color: '#e7e7e7'}}>
                                <tr>
                                    <th>No.</th>
                                    <th>발행일시</th>
                                    <th>유형</th>
                                    <th>단계</th>
                                    <th>상태</th>
                                </tr>
                            </thead>
                        </table>
                        {/*<div className={styles.scrollContents}> */}
                            <table className={styles.table2 + " " + styles.scrollbar}>
                                <colgroup style={{ width: '100%' }}>
                                    <col style={{ width: '11%' }} />
                                    <col style={{ width: '36%' }} />
                                    <col style={{ width: '19%' }} />
                                    <col style={{ width: '21%' }} />
                                    <col style={{ width: '23%' }} />
                                </colgroup>
                                <tbody>
                                    {gridUI}
                                </tbody>
                            </table>
                        {/*</div>*/}
                        <article className={styles.listViewWrap}>
                            <div className={styles.listGroup} id="detailWarning1" style={{ display: 'block' }}>
                                <header className={styles.listHeader}>
                                <h3 className={styles.listHeaderTitle}>세부 현황{selectedIsAlarm}</h3>
                                </header>
                                    <i className={styles.icImg + " " + styles.icSuccess}></i>
                                    <ul className={styles.listView}>
                                        <li>
                                            <p className={styles.label}>{selectedAlarmDate}</p>
                                            <p className={styles.data}>{selectedAlarmMessage}</p>
                                        </li>
                                    </ul>
                            <div className={styles.iconArea}><span className={styles.icAlarmDanger}></span></div>
                           </div>
                       </article>
                        <div className={styles.popupFooter}>
                            <div className={styles.btnGroup + " " + styles.btnFull + " " + styles.btnMargin}>
                                <div className={styles.btn + " " + styles.btnMd1} onClick={() => this.onMoveSelectedAlarm()}><span className={styles.screenChange}>화면전환</span></div>
                                {
                                    (this.props.alarmSound)
                                        ? <div className={styles.btn + " " + styles.btnMd2} onClick={this.onSound}><span className={styles.alarmOff}>소리 OFF</span></div>
                                        : <div className={styles.btn + " " + styles.btnMd2} onClick={this.onSound}><span className={styles.alarmOff}>소리 ON</span></div>
                                }
                                {                                    
                                    (this.props.selectedAlarm && this.props.selectedAlarm.isAlarm) 
                                        ? <div className={styles.btn + " " + styles.btnMd3} onClick={this.onMalfunction}><span className={styles.powerOff}>알람 OFF</span></div>
                                        : <div className={styles.btn + " " + styles.btnMd3}></div>
                                }
                            </div>
                        </div>
                       </div>
                </section>
            {/*</aside> */}
         </>
        )
    }



} export default WarningAlarmInfo;