import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import $ from 'jquery';
import cIntro from '../CompanyIntro/css/company.module.css';

/* import AOS from "aos";
import "aos/dist/aos.css"; */


class CI extends Component {
    static displayName = CI.name;

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
               <div className={cIntro.comCIBox}>
                <span className={cIntro.backColorBox}></span>
                <span className={cIntro.whiteBox8}></span>
                <div className={cIntro.comCITitle}>
                    <span>회사소개</span>
                    {/* <h4 data-aos="fade-up" data-aos-duration="1000">CI</h4> */}
                    <h4>CI</h4>
                    <span>전문 기술로 공간의 안전 가치를 높이는 (주)유엔이</span>
                </div>

                <div className={cIntro.comCIDown}>
                    <span>CI소개</span>
                    <span>U&E를 3D입체 형태로 표현한 심볼은 10년이상 유엔이가 단단하게 쌓아온</span>
                    <span>디지털트윈, 메타버스 기반의 공간정보 구축 관련 전문성을 상징합니다.</span>
                    <div className={cIntro.DownBox}>
                        <div className={cIntro.CIDownBox}>
                            <span>img</span>
                            <span>CI 다운로드</span>
                        </div>
                        <div className={cIntro.CIDownBox2}>
                            <span>img</span>
                            <span>CI 다운로드</span>
                        </div>
                    </div>
                </div>

                <div className={cIntro.comCIColor}>
                    <div className={cIntro.comCIColorFlexBox}>
                        <div className={cIntro.ciFlex1}>
                            <span>R-221 G-60 B-37</span>
                            <span>C-8 M-89 Y-89 K-0</span>
                        </div>
                        <div className={cIntro.ciFlex2}>
                            <span>R-0 G-0 B-0</span>
                            <span>C-0 M-0 Y-0 K-100</span>
                        </div>
                        <div className={cIntro.ciFlex3}>
                            <span>R-234 G-234 B-234</span>
                            <span>C-10 M-7 Y-7 K-0</span>
                        </div>
                        <div className={cIntro.ciFlex4}>
                            <span>R-171 G-171 B-171</span>
                            <span>C-38 M-30 Y-29 K-0</span>
                        </div>
                    </div>
                </div>

                <div className={cIntro.comCISBox}>
                <div className={cIntro.comCIBorderBox}>
                    <div className={cIntro.ciBFlex1}>
                        <div className={cIntro.cBox1}>
                            <span>FIRST</span>
                        </div>
                        <div className={cIntro.cBox2}>
                            <span>안전을 선도하는 기업</span>
                        </div>
                    </div>

                    <div className={cIntro.ciBFlex2}>
                        <div className={cIntro.cBox3}>
                            <span>SECOND</span>
                        </div>
                        <div className={cIntro.cBox4}>
                            <span>신속하고 정확한 대응</span>
                        </div>
                    </div>

                    <div className={cIntro.ciBFlex3}>
                        <div className={cIntro.cBox5}>
                            <span>THIRD</span>
                        </div>
                        <div className={cIntro.cBox6}>
                            <span>유엔이의 뜨거운 열정</span>
                        </div>
                    </div>
                  </div>
                </div>
              </div>
            </>
        );
    }
}

export default CI;