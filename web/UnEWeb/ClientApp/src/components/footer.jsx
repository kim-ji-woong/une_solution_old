import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import home from '../components/css/home.module.css';


class Footer extends Component {
    static displayName = Footer.name;

    componentDidMount() {

    }

    render() {
        return (
            <>
                <div className={home.footBox}>
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
                </div> 
           </>
        )
    }
}

export default Footer;


