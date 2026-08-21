import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import { Link } from 'react-router-dom';
import $ from 'jquery';
import cIntro from '../CompanyIntro/css/company.module.css';

/* import AOS from "aos";
import "aos/dist/aos.css"; */


class Vision extends Component {
    static displayName = Vision.name;

    constructor(props) {
        super(props);
    }

    componentDidMount() {
        /* $(document).ready(function () {
            AOS.init();
        }); */
    }

    render() {
        return (
            <>
                <div className={cIntro.comVisionBox}>
                <span className={cIntro.backColorBox}></span>
                <span className={cIntro.whiteBox3}></span>
                <div className={cIntro.comVisionTitle}>
                    <span>회사소개</span>
                    {/* <h4 data-aos="fade-up" data-aos-duration="1000">경영이념 / 비전</h4> */}
                    <span>전문 기술로 공간의 안전 가치를 높이는 (주)유엔이</span>
                </div>

                <div className={cIntro.comVisionImg}>
                    <span>경영이념</span>
                    <div className={cIntro.managementFlex}>
                        <div className={cIntro.mbox1}>
                            <span className={cIntro.mImg1}>img1</span>
                            <span className={cIntro.mboxContent}>
                                <span>-시대변화에 맞는 차별화된 기술혁신 추구</span>
                                <span>-인재 양성 및 시장 주도형 기술개발</span>
                                <span>-안전, 안심, 고객 만족도 제고</span>
                            </span>
                        </div>
                        <div className={cIntro.mbox2}>
                            <span className={cIntro.mImg2}>img2</span>
                            <span className={cIntro.mboxContent2}>
                                <span>-윤리 준수 경영</span>
                                <span>-고객과 직원간의 신뢰구축</span>
                                <span>-지속적 품질관리</span>
                            </span>
                        </div>
                        <div className={cIntro.mbox3}>
                            <span className={cIntro.mImg3}>img3</span>
                            <span className={cIntro.mboxContent3}>
                                <span>-안전분야의 새로운 가치창조</span>
                                <span>-지속적인 연구개발 투자</span>
                                <span>-인재 중심의 동반 성장관리</span>
                            </span>
                        </div>
                        <div className={cIntro.mbox4}>
                            <span className={cIntro.mImg4}>img4</span>
                            <span className={cIntro.mboxContent4}>
                                <span>-경쟁력 있는 선진형 기술력 확보</span>
                                <span>-정직, 신용, 기술 중심의 서비스 제공</span>
                                <span>-안전한 공간제공으로 삶의 질 향상</span>
                            </span>
                        </div>
                    </div>

                </div>
                <span className={cIntro.grayBox}></span>
                <span className={cIntro.whiteBox4}></span>
                <div className={cIntro.comVision}>
                    <span>비전</span>
                    <span>안전해서 안심할 수 있는 스마트 공간의 글로벌 리더</span>
                    <div className={cIntro.visionFlexBox}>
                        <div className={cIntro.vFlex1}>
                            <div className={cIntro.vBox1}>
                                <span>SPACE</span>
                            </div>
                            <div className={cIntro.vBox2}>
                                <span>안전해서 안심할 수 있는</span>
                                <span>디지털 기술적용 공간</span>
                            </div>
                        </div>

                        <div className={cIntro.vFlex2}>
                            <div className={cIntro.vBox3}>
                                <span>SMART</span>
                            </div>
                            <div className={cIntro.vBox4}>
                                <span>4차 산업혁명시대</span>
                                <span>스마트 기술의 핵심</span>
                                <span>디지털트윈 기술</span>
                            </div>
                        </div>

                        <div className={cIntro.vFlex3}>
                            <div className={cIntro.vBox5}>
                                <span>SAFETY</span>
                            </div>
                            <div className={cIntro.vBox6}>
                                <span>인간 중심의</span>
                                <span>안전제일 가치</span>
                             </div>
                        </div>

                        <div className={cIntro.vFlex4}>
                            <div className={cIntro.vBox7}>
                                <span>SECURITY</span>
                            </div>
                            <div className={cIntro.vBox8}>
                                <span>재난재해로부터</span>
                                <span>안심</span>
                            </div>
                        </div>
                    </div>
                </div>
             </div> 
            </>
        );
    }
}

export default Vision;