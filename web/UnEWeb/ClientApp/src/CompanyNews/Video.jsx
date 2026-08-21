import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import cIntro from '../CompanyIntro/css/company.module.css';
import $ from 'jquery';

/* import AOS from "aos";
import "aos/dist/aos.css"; */

class Video extends Component {
    static displayName = Video.name;

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
            <div className={cIntro.comIntroBox2}>
                <span className={cIntro.whiteBox}></span>
                <div className={cIntro.comIntroTitle}>
                    <span>회사소개</span>
                    {/* <h4 data-aos="fade-up" data-aos-duration="1000">홍보영상</h4> */}
                    <h4>홍보영상</h4>
                    <span>전문 기술로 공간의 안전 가치를 높이는 (주)유엔이</span>
                </div>

                <div className={cIntro.comIntroImg}>
                    <span>경영이념</span>
                    <span>이미지 example</span>
                </div>
                <span className={cIntro.grayBox}></span>
                <div className={cIntro.comGreeting}>
                    <span className={cIntro.whiteBox2}></span>
                    <span>비전</span>
                    <div className={cIntro.ceoBox}>
                        <div className={cIntro.ceoTextBox}>
                            <span>안녕하세요. (주)유엔이 대표이사 여욱현입니다.</span>
                            <span>(주)유엔이 홈페이지를 방문해주신 고객 여러분들께 감사의 말씀을 드립니다.</span>
                            <span>추후 컨텐츠 추가 예정</span>
                        </div>
                    </div>
                </div>
            </div>
            </>
        );
    }
}

export default Video;