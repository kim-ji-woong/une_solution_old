import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import home from '../components/css/home.module.css';
import '../components/css/home.css';
import $ from 'jquery';
import { Link } from "react-router-dom";
//import Carousel from '../components/carousel.jsx';

/* carousel */
import "slick-carousel/slick/slick.css";
import "slick-carousel/slick/slick-theme.css";
import Slider from "react-slick";

import "aos/dist/aos.css";
import AOS from "aos"; 

/* fullPage test */

import { FullPage, Slide } from 'react-full-page';



class Home extends Component {
    static displayName = Home.name;

    componentDidMount() {
        //aos 초기화
        $(document).ready(function () {
            AOS.init();
        });


       $(function () {
            $(window).scroll(function () {
                //if ($(this).scrollTop() > 200) {
                if ($(this).scrollTop() > 200) {
                    $('#toTop').fadeIn();
                    $('#toTop').css('right', $('.' + home.homeArea).offset().right);
                } else {
                    $('#toTop').fadeOut();
                }
            });

            $('#toTop').click(function () {
                $('html, body').animate({
                    scrollTop: 0
                }, 400);
                return false;
            });
        });
    }


    render() {
        const settings = {
            dots: true,
            infinite: true,
            speed: 500,
            slidesToShow: 1,
            slidesToScroll: 1
        };
       
        return (
            <>
                <div className={home.overlay}></div>
                <div className={home.homeArea}>
                    <a id="toTop" href="#"></a>
                    <FullPage controls controlsProps={{ className: "slideNavigation" }}>
                        <Slide id={home.section1}>
                            <div>
                                <span className={home.slideTitle}>Unique & Experience</span>
                                <span className={home.slideTitle2}>디지털 트윈과 재난 안전에 대한 전문 기술로 공간의 안전 가치를 높이는 IT기업</span>

                                <Slider {...settings}>
                                    <div>
                                        <div className={home.mainImg1}></div>
                                    </div>
                                    <div>
                                        <div className={home.mainImg2}></div>
                                    </div>
                                    <div>
                                        <div className={home.mainImg3}></div>
                                    </div>
                                    <div>
                                        <div className={home.mainImg4}></div>
                                    </div>
                                </Slider>
                        </div>
                    </Slide>
                    <Slide id={home.section2}>
                        <div className={home.uTechnology}>
                            <span className={home.skillETitle}>Our Technology</span>
                            <span className={home.skillTitle}>기술 소개</span>
                            <div className={home.skillBox}>
                                <div className={home.skill1}>
                                    <div className={home.iconArea1}>
                                        <span></span>
                                        <span>Spatial information construction</span>
                                    </div>
                                    <div className={home.textBox1}>
                                        <span>공간정보구축</span>
                                        <span>디지털 트윈에 대한 설명을</span>
                                        <span>입력하여 주십시오 디지털 트윈에</span>
                                        <span>대한 설명을 입력하여 주십시오</span>
                                        <span>디지털 트윈에 대한 설명을</span>
                                        <span>입력하여 주십시오</span>
                                    </div>
                                    <span className={home.moreView1}>MORE VIEW</span>
                                </div>
                                <div className={home.skill2}>
                                    <div className={home.iconArea2}>
                                        <span></span>
                                        <span>E-Standard Operating Procedure</span>
                                    </div>
                                    <div className={home.textBox2}>
                                        <span>E-SOP</span>
                                        <span>디지털 트윈에 대한 설명을</span>
                                        <span>입력하여 주십시오 디지털 트윈에</span>
                                        <span>대한 설명을 입력하여 주십시오</span>
                                        <span>디지털 트윈에 대한 설명을</span>
                                        <span>입력하여 주십시오</span>
                                    </div>
                                    <span className={home.moreView2}>MORE VIEW</span>
                                </div>
                                    <div className={home.skill3}>
                                    <div className={home.iconArea3}>
                                        <span></span>
                                        <span>Data visualization</span>
                                    </div>
                                    <div className={home.textBox3}>
                                        <span>데이터 시각화</span>
                                        <span>디지털 트윈에 대한 설명을</span>
                                        <span>입력하여 주십시오 디지털 트윈에</span>
                                        <span>대한 설명을 입력하여 주십시오</span>
                                        <span>디지털 트윈에 대한 설명을</span>
                                        <span>입력하여 주십시오</span>
                                    </div>
                                    <span className={home.moreView3}>MORE VIEW</span>
                                </div>
                                    <div className={home.skill4}>
                                    <div className={home.iconArea4}>
                                    <span></span>
                                    <span>System linkage</span>
                                    </div>
                                    <div className={home.textBox4}>
                                        <span>시스템 연계</span>
                                        <span>디지털 트윈에 대한 설명을</span>
                                        <span>입력하여 주십시오 디지털 트윈에</span>
                                        <span>대한 설명을 입력하여 주십시오</span>
                                        <span>디지털 트윈에 대한 설명을</span>
                                        <span>입력하여 주십시오</span>
                                    </div>
                                    <span className={home.moreView4}>MORE VIEW</span>
                                </div>
                            </div>
                        </div>
                    </Slide>
                    <Slide id={home.section3}>
                        <div className={home.uIntro}>
                            <span className={home.introETitle}>Our Business</span>
                            <span className={home.introTitle}>사업 소개</span>
                            <div className={home.introBox}>
                                <span className={home.safetyBox}>
                                    <span><Link to="/counter">안전관리</Link></span>
                                </span>
                                <span className={home.digitalBox}>
                                    <span><Link to="/counter">디지털 트윈</Link></span>
                                </span>
                            </div>
                        </div>
                    </Slide>
                    <Slide id={home.section4}>
                        <div className={home.uNews}>
                            <span className={home.uNewsETitle}>Company News</span>
                            <span className={home.uNewsTitle}>회사 소식</span>
                                <div className={home.uNewsBtn}>
                                    <div className={home.uNewsTab}>
                                        <span>보도자료</span>
                                        <span>월간 유엔이</span>
                                    </div>
                                    <div className={home.arrowTab}>
                                        <span className={home.arrowLeft}></span>
                                        <span className={home.arrowRight}></span>
                                    </div>
                                </div>
                                <div className={home.uNewsArea}>
                                <span className={home.uNewsBox1}>
                                    <span className={home.uNewImg1}></span>
                                    <span className={home.uNewText1}>
                                       <span>대한산업안전협회, (주)유엔이,</span>
                                       <span>(주)티랩스와 디지털 트윈 기술..</span>
                                       <span>대한산업안전협회(이하 협회, 회장 박종선)</span>
                                       <span>와 (주)유엔이(대표이사 여욱현), (주)티랩스</span>
                                       <span>(대표이사 도락주)가 메타버스와 디지털 트</span>
                                       <span>윈 기술을 접목한 첨단 안전 시스템 개발. 보</span>
                                       <span>급을 위해 손을 잡았다. 협회와 (주)유엔이,...</span>
                                    </span>
                                </span>
                                <span className={home.uNewsBox2}>
                                    <span className={home.uNewImg2}></span>
                                    <span className={home.uNewText2}>
                                        <span>제주 테크노파크, 화재 예방 재</span>
                                        <span>난 안전 포럼 개최</span>
                                        <span>최근 대형 화재와 붕괴 사고로 재난 안전에</span>
                                        <span>대한 경각심이 고조되는 가운데, 제주에서</span>
                                        <span>효과적인 재난 안전 인프라 구축을 위한 포</span>
                                        <span>럼이 열린다. 제주 테크노파크는 27일 오후</span>
                                        <span>2시 제주 벤처마루 10층 대강당에서 '화재...</span>
                                    </span>
                                </span>
                                <span className={home.uNewsBox3}>
                                    <span className={home.uNewImg3}></span>
                                    <span className={home.uNewText3}>
                                        <span>유엔이, 스마트 재난 관리 시스</span>
                                        <span>템 부문 ICT 대상</span>
                                        <span>여욱현 유엔이 대표가 머니투데이 주최의</span>
                                        <span>'2021 제 7회 대한민국 리딩기업대상'에서</span>
                                        <span>'스마트 재난 관리 시스템' 부문 ICT 대상을</span>
                                        <span>수상하고 기념 사진을 찍고 있다. 유엔이는</span>
                                        <span>공간 정보와 재난 안전 분야에서 전문 기...</span>
                                    </span>
                                </span>
                                <span className={home.uNewsBox4}>
                                    <span className={home.uNewImg4}></span>
                                    <span className={home.uNewText4}>
                                        <span>대한산업안전협회, (주)유엔이,</span>
                                        <span>(주)티랩스와 디지털 트윈 기술..</span>
                                        <span>대한산업안전협회(이하 협회, 회장 박종선)</span>
                                        <span>와 (주)유엔이(대표이사 여욱현), (주)티랩스</span>
                                        <span>(대표이사 도락주)가 메타버스와 디지털 트</span>
                                        <span>윈 기술을 접목한 첨단 안전 시스템 개발. 보</span>
                                        <span>급을 위해 손을 잡았다. 협회와 (주)유엔이,...</span>
                                    </span>
                                </span>
                            </div>
                        </div>

                          {/* <div class="uStoryy">
                                <div class="uStoryBox"> 
                                <div class="uStoryTitle">유엔이 이야기</div>
                                <div class="slide_wrap">

                                    <div class="slide_show">
                                        <div class="slide_img">
                                            <div class="slide"><img src="https://tistory4.daumcdn.net/tistory/2980705/skin/images/hover01.jpg" /></div>
                                            <div class="slide"><img src="https://tistory4.daumcdn.net/tistory/2980705/skin/images/hover02.jpg" /></div>
                                            <div class="slide"><img src="https://tistory4.daumcdn.net/tistory/2980705/skin/images/hover03.jpg" /></div>
                                            <div class="slide"><img src="https://tistory4.daumcdn.net/tistory/2980705/skin/images/hover04.jpg" /></div>
                                            <div class="slide"><img src="https://tistory4.daumcdn.net/tistory/2980705/skin/images/hover05.jpg" /></div>
                                        </div>
                                    </div>

                                    <div class="slide_btn">
                                        <a href="#" class="prev"><i class="fas fa-angle-left"></i></a>
                                        <a href="#" class="next"><i class="fas fa-angle-right"></i></a>
                                    </div>

                                </div>
                                </div>
                            </div> */}

                        {/*   <div class="image-gallery">
                            <a class="button prev"></a>
                            <a class="button next"></a>
                                <div class="thumbs">
                                    <ul>
                                        <li>
                                            <a><img className={home.img1} /></a>
                                        </li>
                                        <li>
                                           <a><img className={home.img2} /></a>
                                        </li>
                                        <li>
                                           <a><img className={home.img3} /></a>
                                        </li>
                                        <li>
                                            <a><img className={home.img4} /></a>
                                        </li>
                                        <li>
                                            <a><img className={home.img5} /></a>
                                        </li>
                                        <li>
                                            <a><img className={home.img6} /></a>
                                        </li>
                                    </ul>
                                </div>
                           </div> */}

                        {/* <Carousel>
                            <img src='https://via.placeholder.com/150' alt='imagem' title='imagem' />
                            <img src='https://via.placeholder.com/150' alt='imagem' title='imagem' />
                            <img src='https://via.placeholder.com/150' alt='imagem' title='imagem' />
                            <img src='https://via.placeholder.com/150' alt='imagem' title='imagem' />
                            <img src='https://via.placeholder.com/150' alt='imagem' title='imagem' />
                        </Carousel> */}

                    </Slide>
                </FullPage>
                </div>


                {/* <div className={home.imgScale}>
                    <img src="이미지 스케일 테스트" />
                </div> */}

                {/* <div className={home.tab}>
                    <ul className={home.tabnav}>
                        <li><a href="#tab01">탭1</a></li>
                        <li><a href="#tab02">탭2</a></li>
                        <li><a href="#tab03">탭3</a></li>
                        <li><a href="#tab04">탭4</a></li>
                    </ul>
                    <div className={home.tabcontent}>
                        <div id="tab01">tab1 content</div>
                        <div id="tab02">tab2 content</div>
                        <div id="tab03">tab3 content</div>
                        <div id="tab04">tab4 content</div>
                    </div>
                </div> */}

                {/* <div className={home.footBox}>
                    <div className={home.footTitleBox}>
                        <div className={home.footTitle}>
                            <span>오시는길</span>
                            <span>1:1 이메일 문의</span>
                        </div>
                        <span className={home.companyDown}>회사소개서 다운로드
                            <span className={home.companyImg}></span>
                        </span>
                    </div>
                    <div className={home.footContents}>
                       <div className={home.footConTop}>
                          <span>(주)유엔이</span>
                          <span>Tel:02-714-4133</span>
                          <span>Fax:02-714-4134</span>
                       </div>
                       <div className={home.footConBottom}>
                            <span>서울지사:서울용산구청파로345주연빌딩1층</span>
                            <span>본사:대구 달서구 달구벌대로 1053 계명대학교 첨단산업지원센터 108호</span>
                       </div>
                    </div>
                    <div className={home.footContents2}>
                        <span className={home.footLogo}></span>
                        <span className={home.footText}>CopyrightⓒU&E All rights reserved.</span>
                    <div className={home.footIconArea}>
                        <span className={home.footYouTube}></span>
                        <span className={home.footinstagram}></span>
                        <span className={home.footFacebook}></span>
                    </div>
                    </div>
                </div> */}
            </>
        );
    } 
}

export default Home;
