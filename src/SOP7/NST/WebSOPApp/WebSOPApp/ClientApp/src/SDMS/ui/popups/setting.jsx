import React, { Component } from 'react';
import { BrowserRouter as Route, Link } from 'react-router-dom';
import { withRouter } from 'react-router-dom';
import styles from '../../css/sdms.module.css';


class Setting extends Component {

    constructor(props) {
        super(props);

        this.props = props;
    }

    componentDidMount() {

    }


    render() {
        return (
           <>
				<div id={styles.stgPop}>
					<div>
						<div>
							<div className={styles.stgCont}>
									<div className={styles.stgTitle}>환경설정</div>
									<a href="" className={styles.popupBoxX}>닫기</a>
									<ul className={styles.stgTab}>
										<li><a href="#" className={styles.on}>3D제어 환경설정</a></li>
										<li><a href="#">재난상황 환경설정</a></li>
										<li><a href="#">초기 상황전파 환경설정</a></li>
								    </ul>


									{/*page1-3D제어 환경설정*/}
								  <div className={styles.stgList}>
										<div className={styles.stgName}>
											<h5>3D 회전 설정</h5>
										    <span className={styles.stgTltp} data-tooltip="백스페이스 기능을 비활성화 합니다."></span>
										   <div className={styles.dswitchBtn}>
												<label className={styles.dswitch}>
													<input type="checkbox" className={styles.dlabelInput} />
													<span className={styles.dslider + " " + styles.dround}></span>
												</label>
											</div>
											<div className={styles.stgBox}>
												<span>3D 회전속도</span>
												<select>
													<option>2초</option>
													<option>5초</option>
													<option>7초</option>
											     </select>
									        </div>
										</div>
										<div className={styles.stgName}>
											<h5>실행 단축키</h5>
											<span className={styles.stgTltp} data-tooltip="단위 시스템 불러오기 단축키 기능을 설정 합니다."></span>
											<ul className={styles.stgnKey}>
												<li><dl><dt>현황정보</dt><dd><span>Alt +</span><input type="text" name="" id="" value="M" /></dd></dl></li>
												<li><dl><dt>CCTV</dt><dd><span>Alt +</span><input type="text" name="" id="" value="C" /></dd></dl></li>
												<li><dl><dt>작업자 모니터링</dt><dd><span>Alt +</span><input type="text" name="" id="" value="D" /></dd></dl></li>
												<li><dl><dt>POI 뷰어 설정</dt><dd><span>Alt +</span><input type="text" name="" id="" value="O" /></dd></dl></li>
												<li><dl><dt>상황전파</dt><dd><span>Alt +</span><input type="text" name="" id="" value="S" /></dd></dl></li>
												<li><dl><dt>이력관리</dt><dd><span>Alt +</span><input type="text" name="" id="" value="Q" /></dd></dl></li>
											</ul>
										</div>
										<div className={styles.stgName}>
											<h5>3D 화면조작</h5>
											<span className={styles.stgTltp}></span>
											<ul>
												<li><span>왼쪽</span><input type="text" name="" id="" value="a" /></li>
												<li><span>오른쪽</span><input type="text" name="" id="" value="b" /></li>
												<li><span>위</span><input type="text" name="" id="" value="c" /></li>
												<li><span>아래</span><input type="text" name="" id="" value="d" /></li>
										    </ul>
										</div>
										<div className={styles.stgName}>
											<h5>3D 뷰 저장 및 이동</h5>
											<ul className={styles.stgnKey}>
												<li><dl><dt>뷰 저장</dt><dd><span>ctrl +</span><input type="text" name="" id="" value="" /></dd></dl></li>
												<li><dl><dt>뷰 이동(1)</dt><dd><span>ctrl +</span><input type="text" name="" id="" value="" /></dd></dl></li>
												<li><dl><dt>뷰 이동(2)</dt><dd><span>ctrl +</span><input type="text" name="" id="" value="" /></dd></dl></li>
												<li><dl><dt>뷰 이동(3)</dt><dd><span>ctrl +</span><input type="text" name="" id="" value="" /></dd></dl></li>
												<li><dl><dt>뷰 이동(4)</dt><dd><span>ctrl +</span><input type="text" name="" id="" value="" /></dd></dl></li>
												<li><dl><dt>뷰 이동(5)</dt><dd><span>ctrl +</span><input type="text" name="" id="" value="" /></dd></dl></li>
											</ul>
										</div>
									</div> 


									{/*page2-재난상황 환경설정*/}
								   {/* <div className={styles.stgList}>
										<div className={styles.stgName}>
											<h5>유형별 알람 설정</h5>
											<span className={styles.stgTltp}></span>
											<ul className={styles.stgnKey}>
												<li><dl><dt><span className={styles.stgTltp}></span>화재센서</dt><dd><input type="checkbox" name="" id="" />수신</dd></dl></li>
												<li><dl><dt><span className={styles.stgTltp}></span>저탄장 화재감지</dt><dd><input type="checkbox" name="" id="" />수신</dd></dl></li>
												<li><dl><dt><span className={styles.stgTltp}></span>감지센서</dt><dd><input type="checkbox" name="" id="" />수신</dd></dl></li>
												<li><dl><dt><span className={styles.stgTltp}></span>CCTV</dt><dd><input type="checkbox" name="" id="" />수신</dd></dl></li>
											</ul>
										</div>
										<div className={styles.stgName}>
											<h5>재난대응 프로세스 실행</h5>
											<span className={styles.stgTltp}></span>
											<ul className={styles.stgnKeyy}>
												<li><dl><dt>관심</dt><select className={styles.responseSelect}><option>감지 시 재난대응 프로세스 자동실행 안함</option></select></dl></li>
												<li><dl><dt>주의</dt><select className={styles.responseSelect}><option>감지 시 재난대응 프로세스 자동실행</option></select></dl></li>
												<li><dl><dt>경계</dt><select className={styles.responseSelect}><option>감지 시 재난대응 프로세스 자동실행</option></select></dl></li>
												<li><dl><dt>심각</dt><select className={styles.responseSelect}><option>감지 시 재난대응 프로세스 자동실행</option></select></dl></li>
											</ul>
										</div>
									</div> */}


									{/*page3-초기 상황전파 환경설정*/}
								   {/* <div className={styles.stgList3}>
										<div className={styles.stgName3}>
											<h5>초기 상황전파 환경설정</h5>
											<span className={styles.stgTltp3}></span>
										</div>
									    <div className={styles.setLeftBox}>
										    <select>
												<option>재난유형 선택</option>
												<option>재난유형1</option>
												<option>재난유형2</option>
										    </select>
										    <div className={styles.spreadPersonBtn}>전파대상자 지정</div>
									    </div>
									    <div className={styles.setRightBox}>
											<span>PUSH message</span>
											<span>기본문구가 표출되는 영역(수정 불가)</span>
											<span><textarea placeholder="전파 문구를 입력해주세요."></textarea></span>
											<span>수신자:안전관리팀 홍길동, 안전관리팀 콩순이</span>
										</div>
									</div> */}


									<ul className={styles.dspBtnn}>
										<li><a href="javascript:stgClose();">취소</a></li>
										<li><a href="#">저장</a></li>
									</ul>
							</div>
						</div>
					</div>
				</div>
           </>
        )

    }


} export default Setting;