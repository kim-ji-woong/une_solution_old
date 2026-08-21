import React, { Component, useDebugValue } from 'react';
import { BrowserRouter as Route, Link } from 'react-router-dom';
import { withRouter } from 'react-router-dom';
import styles from '../../SDMS/css/sdms.module.css';
import imgClose from '../../SDMS/image/common_Icon/popup_close.png';
import title from '../css/titleBar.module.css';
import SetSpreadMembers from './setSpreadMembers';

import RootResource from '../resource/id';


class Setting extends Component {

    constructor(props) {
		super(props);


		this.state = {
			tabMenu: RootResource.settingTab.monitoring,
			visiblePopups: {
				setSpreadMembers: false
			},
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
		var popup = document.getElementsByClassName(styles.stgCont)[0];
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
		this.props.setVisiblePopup(this.props.popupType, false);
	}

	setVisiblePopup = (popup, visible) => {
		const visiblePopups = { ...this.state.visiblePopups };
		visiblePopups[popup] = visible;
		this.setState({ visiblePopups });
	}

	displayTabMenu() {
		const tabMenu = this.state.tabMenu;

		let monitoringClass = "";
		let disasterClass = "";
		let spreadClass = "";

		if (tabMenu === RootResource.settingTab.monitoring)
			monitoringClass = styles.on;
		else if (tabMenu === RootResource.settingTab.disaster)
			disasterClass = styles.on;
		else if (tabMenu === RootResource.settingTab.spread)
			spreadClass = styles.on;

		return (
			<ul className={styles.stgTab}>
				<li><a className={monitoringClass} onClick={() => this.onClickTab(RootResource.settingTab.monitoring)}>3D제어 환경설정</a></li>
				<li><a className={disasterClass} onClick={() => this.onClickTab(RootResource.settingTab.disaster)}>재난상황 환경설정</a></li>
				<li><a className={spreadClass} onClick={() => this.onClickTab(RootResource.settingTab.spread)}>초기 상황전파 환경설정</a></li>
			</ul>
		);
    }

	onClickTab = (menu) => {
		const tabMenu = this.state.tabMenu;

		if (menu === null || menu === undefined || tabMenu === menu)
			return;

		this.setState({ tabMenu: menu});
    }
	

	displayMenu() {
		const tabMenu = this.state.tabMenu;

		if (tabMenu === RootResource.settingTab.monitoring) {
			return (<div className={styles.stgList}>
				<div className={styles.stgName}>
					<h5>3D 회전 설정</h5>
					<span className={styles.stgTltp} data-tooltip="3D 회전 기능 활성화와 회전 속도 설정이 가능합니다."></span>
					<div className={styles.dswitchBtn}>
						<label className={styles.dswitch}>
							<input type="checkbox" className={styles.dlabelInput} />
							<span className={styles.dslider + " " + styles.dround}></span>
						</label>
					</div>
					<div className={styles.stgBox}>
						<span>3D 회전속도</span>
						<select className={styles.blueSellSet1}>
							<option>2초</option>
							<option>5초</option>
							<option>7초</option>
						</select>
					</div>
				</div>
				<div className={styles.stgName}>
					<h5>실행 단축키</h5>
					<span className={styles.stgTltp} data-tooltip="시스템 단축키 설정이 가능합니다."></span>
					<ul className={styles.stgnKey}>
						<li><dl><dt>현황정보</dt><dd><span>Alt +</span><input type="text" name="" id="" /></dd></dl></li>
						<li><dl><dt>CCTV</dt><dd><span>Alt +</span><input type="text" name="" id="" /></dd></dl></li>
						<li><dl><dt>작업자 모니터링</dt><dd><span>Alt +</span><input type="text" name="" id="" /></dd></dl></li>
						<li><dl><dt>POI 뷰어 설정</dt><dd><span>Alt +</span><input type="text" name="" id="" /></dd></dl></li>
						<li><dl><dt>상황전파</dt><dd><span>Alt +</span><input type="text" name="" id="" /></dd></dl></li>
						<li><dl><dt>이력관리</dt><dd><span>Alt +</span><input type="text" name="" id="" /></dd></dl></li>
					</ul>
				</div>
				<div className={styles.stgName}>
					<h5>3D 화면조작</h5>
					<span className={styles.stgTltp} data-tooltip="시스템 단축키 설정이 가능합니다."></span>
					<ul>
						<li><span>왼쪽</span><input type="text" name="" id=""  /></li>
						<li><span>오른쪽</span><input type="text" name="" id="" /></li>
						<li><span>위</span><input type="text" name="" id="" /></li>
						<li><span>아래</span><input type="text" name="" id="" /></li>
					</ul>
				</div>
				<div className={styles.stgName}>
					<h5>3D 뷰 저장 및 이동</h5>
					<ul className={styles.stgnKey}>
						<li><dl><dt>뷰 저장</dt><dd><span>ctrl +</span><input type="text" name="" id="" /></dd></dl></li>
						<li><dl><dt>뷰 이동(1)</dt><dd><span>ctrl +</span><input type="text" name="" id="" /></dd></dl></li>
						<li><dl><dt>뷰 이동(2)</dt><dd><span>ctrl +</span><input type="text" name="" id="" /></dd></dl></li>
						<li><dl><dt>뷰 이동(3)</dt><dd><span>ctrl +</span><input type="text" name="" id="" /></dd></dl></li>
						<li><dl><dt>뷰 이동(4)</dt><dd><span>ctrl +</span><input type="text" name="" id="" /></dd></dl></li>
						<li><dl><dt>뷰 이동(5)</dt><dd><span>ctrl +</span><input type="text" name="" id="" /></dd></dl></li>
					</ul>
				</div>
			</div> );
		} else if (tabMenu === RootResource.settingTab.disaster) {





			return (<div className={styles.stgList}>
				<div className={styles.stgName}>
					<h5>유형별 알람 설정</h5>
					<span className={styles.stgTltp} data-tooltip="유형별 알람 수신여부 설정이 가능합니다."></span>
					<ul className={styles.stgnKey}>
						<li><dl><dt> &#183; 화재센서</dt><dd><input type="checkbox" name="" id="" />수신</dd></dl></li>
						<li><dl><dt> &#183; 저탄장 화재감지</dt><dd><input type="checkbox" name="" id="" />수신</dd></dl></li>
						<li><dl><dt> &#183; 감지센서</dt><dd><input type="checkbox" name="" id="" />수신</dd></dl></li>
						<li><dl><dt> &#183; CCTV</dt><dd><input type="checkbox" name="" id="" />수신</dd></dl></li>
					</ul>
				</div>
				<div className={styles.stgName}>
					<h5>재난대응 프로세스 실행</h5>
					<span className={styles.stgTltp} data-tooltip="알람 단계별 재난대응 프로세스 실행 설정이 가능합니다."></span>
					<ul className={styles.stgnKeyy}>
						<li><dl><dt>관심</dt><select className={styles.responseSelect1}><option>감지 시 재난대응 프로세스 자동실행 안함</option></select></dl></li>
						<li><dl><dt>주의</dt><select className={styles.responseSelect1}><option>감지 시 재난대응 프로세스 자동실행</option></select></dl></li>
						<li><dl><dt>경계</dt><select className={styles.responseSelect1}><option>감지 시 재난대응 프로세스 자동실행</option></select></dl></li>
						<li><dl><dt>심각</dt><select className={styles.responseSelect1}><option>감지 시 재난대응 프로세스 자동실행</option></select></dl></li>
					</ul>
				</div>
			</div>);
		} else {




			return (<div className={styles.stgList3}>
				<div className={styles.stgName3}>
					<h5>초기 상황전파 환경설정</h5>
					<span className={styles.stgTltp} data-tooltip="재난 상황 발생 시 발송되는 상황전파 메시지 수신 대상자를 설정하는 기능입니다."></span>
				</div>
				<div className={styles.setLeftBox}>
					<select>
						<option>재난유형 선택</option>
						<option>재난유형1</option>
						<option>재난유형2</option>
					</select>
					<div className={styles.spreadPersonBtn} onClick={this.onClickSetSpreadMembers}>전파대상자 지정</div>
				</div>
				<div className={styles.setRightBox}>
					<span>PUSH message</span>
					<span>기본문구가 표출되는 영역(수정 불가)</span>
					<span><textarea placeholder="전파 문구를 입력해주세요."></textarea></span>
					<span>수신자:안전관리팀 홍길동, 안전관리팀 콩순이</span>
				</div>
			</div>);
        }
    }

	onClickSetSpreadMembers = () => {
		let visiblePopups = this.state.visiblePopups;
		visiblePopups.setSpreadMembers = true;

		this.setState({ visiblePopups });
	}
	

	render() {


        return (
           <>
				<div id={styles.stgPop}>
					<div>
						<div>
							<div className={styles.stgCont} onMouseDown={(e) => this.popupDragMousePress(e)}>
								<div className={styles.stgTitle} >환경설정</div>
								<a className={styles.popupBoxX} onClick={this.onClose}><img src={imgClose} alt="닫기" /></a>
								{this.displayTabMenu()}


								{this.displayMenu()}


									<ul className={styles.dspBtnn}>
									<li><a onClick={this.onClose}>취소</a></li>
									<li><a onClick={this.onClose}>저장</a></li>
									</ul>
							</div>
						</div>
					</div>
				{
					this.state.visiblePopups.setSpreadMembers &&
					<SetSpreadMembers
						popupType="setSpreadMembers"
						setVisiblePopup={this.setVisiblePopup}
					/>
				}

				</div>
           </>
        )

    }


} export default Setting;