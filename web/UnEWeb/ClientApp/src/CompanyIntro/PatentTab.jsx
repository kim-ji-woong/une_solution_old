import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import cIntro from '../CompanyIntro/css/company.module.css';
import $ from 'jquery';

/* import AOS from "aos";
import "aos/dist/aos.css"; */

import Patent from '../CompanyIntro/Patent.jsx';
import Certified from '../CompanyIntro/Certified.jsx';

const menuList = {
    0: <Patent />,
    1: <Certified/>,
};


class PatentTab extends Component {
    static displayName = PatentTab.name;

    constructor(props) {
        super(props);

        this.state = {
            menu: 0,
        };
    }

    componentDidMount() {
       /* $(document).ready(function () {
            AOS.init();
        }); */
    }

    changeMenu = (menuIndex) => {
        this.setState({ menu: menuIndex });
    }


    render() {
        return (
            <>
                <div className={cIntro.comCerBox}>
                    <span className={cIntro.backColorBox}></span>
                    <span className={cIntro.whiteBox6}></span>
                    <div className={cIntro.comCerTitle}>
                        <span>회사소개</span>
                        {/* <h4 data-aos="fade-up" data-aos-duration="1000">특허 / 인증 / 수상</h4> */}
                        <h4>특허 / 인증 / 수상</h4>
                        <span>전문 기술로 공간의 안전 가치를 높이는 (주)유엔이</span>
                    </div>

                    <div className={cIntro.comCerBox2}>
                        <div className={cIntro.menuBarr}>
                            <ul className={cIntro.tabss}>
                                <li className={`${this.state.menu === 0 ? 'active' : ''}`} onClick={() => this.changeMenu(0)}>특허</li>
                                <li className={`${this.state.menu === 1 ? 'active' : ''}`} onClick={() => this.changeMenu(1)}>인증 / 수상</li>
                            </ul>
                        </div>
                        <div className={cIntro.tabArea}>
                            {menuList[this.state.menu]}
                        </div>
                    </div>
                </div>
            </>
        );
    }
}

export default PatentTab;