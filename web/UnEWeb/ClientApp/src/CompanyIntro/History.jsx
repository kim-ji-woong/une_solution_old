import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import cIntro from '../CompanyIntro/css/company.module.css';
import $ from 'jquery';

/* import AOS from "aos";
import "aos/dist/aos.css"; */


class History extends Component {
    static displayName = History.name;

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
              <div className={cIntro.comHisBox}>
                <span className={cIntro.backColorBox}></span>
                <span className={cIntro.whiteBox5}></span>
                <div className={cIntro.comHisTitle}>
                    <span>회사소개</span>
                    {/* <h4 data-aos="fade-up" data-aos-duration="1000">연혁</h4> */}
                    <h4>연혁</h4>
                    <span>전문 기술로 공간의 안전 가치를 높이는 (주)유엔이</span>
                </div>

                <div className={cIntro.comHisGraph}>
                </div>
             </div>
            </>
        );
    }
}

export default History;