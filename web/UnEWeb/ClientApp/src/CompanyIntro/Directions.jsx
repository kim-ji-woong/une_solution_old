import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import cIntro from '../CompanyIntro/css/company.module.css';
import $ from 'jquery';

/* import AOS from "aos";
import "aos/dist/aos.css"; */


class Directions extends Component {
    static displayName = Directions.name;

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
            <div className={cIntro.comDiBox}>
                <span className={cIntro.backColorBox}></span>
                <span className={cIntro.whiteBox9}></span>
                <div className={cIntro.comDiTitle}>
                    <span>회사소개</span>
                    {/* <h4 data-aos="fade-up" data-aos-duration="1000">오시는 길</h4> */}
                    <h4>오시는 길</h4>
                    <span>전문 기술로 공간의 안전 가치를 높이는 (주)유엔이</span>
                </div>

                <div className={cIntro.comDiArea}>
                    <span>연구소</span>
                    <span>[서울시 용산구 청파로 345 주연빌딩 1층]</span>
                    <span>지도 영역1</span>
                </div>
                <div className={cIntro.comDiArea2}>
                    <span>본사</span>
                    <span>[대구 달서구 달구벌대로 1053 계명대학교 첨단산업지원센터 108호]</span>
                    <span>지도 영역2</span>
                </div>
            </div>
            </>
        );
    }
}

export default Directions;