import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import cIntro from '../CompanyIntro/css/company.module.css';
import $ from 'jquery';
import { Link } from "react-router-dom";

/* import ReactDOM from "react-dom";
import "fullpage.js/vendors/scrolloverflow"; // Optional. When using scrollOverflow:true
import ReactFullpage from "@fullpage/react-fullpage"; */

/* import AOS from "aos";
import "aos/dist/aos.css"; */


class AboutUnE extends Component {
    static displayName = AboutUnE.name;

    constructor(props) {
        super(props);
    }

    componentDidMount() {
        /* $(document).ready(function () {
             AOS.init();
         }); */


      /* var speed = 500;

        function scrolling(obj) {
            if (!obj) {	
                $('html, body').animate({ scrollTop: 0 }, speed);
            } else {
                var posTop = $(obj).offset().top - 80;	
                $('html, body').animate({ scrollTop: posTop }, speed)	
            }
        };

        $('#' + cIntro.navigation + ' ul li').click(function () {	
            var direction = $(this).attr('Link');	
            scrolling(direction);	
            return false;
        }); */
    }


    render() {
        return (
            <>
                <div className={cIntro.cMenu}>
                <span>회사소개</span>
                <div className={cIntro.dropdown}>
                    <button className={cIntro.dropbtn}>About U&E</button>
                    <div className={cIntro.dropdownContent}>
                        <a href="#">About U&E</a>
                        <a href="#">연혁</a>
                        <a href="#">특허 및 인증</a>
                        <a href="#">조직도</a>
                        <a href="#">C.I</a>
                        <a href="#">오시는길</a>
                    </div>
                </div>
                </div>

                <div className={cIntro.comIntroBox}>
                <span className={cIntro.backColorBox}></span>
                <span className={cIntro.whiteBox}></span>
                <div className={cIntro.comIntroTitle}>
                    <span>회사소개</span>
                    {/* <h4 data-aos="fade-up" data-aos-duration="1000">About U&E</h4> */}
                    <h4>About U&E</h4>
                    <span>전문 기술로 공간의 안전 가치를 높이는 (주)유엔이</span>
                </div>

                <div className={cIntro.comIntroImg}>
                    <span>유엔이</span>
                    <span>[미래를 창조하는 공간 기반의 디지털 트윈기술, 재난안전 전문 기업]</span>
                    <span>이미지 example</span>
                </div>
                <span className={cIntro.grayBox}></span>
                <span className={cIntro.whiteBox2}></span>
                <div className={cIntro.comGreeting}>
                    <span>CEO 인사말</span>
                  <div className={cIntro.ceoBox}>
                    <span>사진 example</span>
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

export default AboutUnE;
