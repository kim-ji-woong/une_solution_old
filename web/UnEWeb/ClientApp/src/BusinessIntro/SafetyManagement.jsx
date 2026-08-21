import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import BIntro from '../BusinessIntro/css/business.module.css';
import '../components/css/home.css';
import $ from 'jquery';

/* import AOS from "aos";
import "aos/dist/aos.css"; */

import Performance from '../BusinessIntro/Performance.jsx';
import DigitalTwin from '../BusinessIntro/DigitalTwin.jsx';


const menuList = {
    0: <Performance />,
    1: <DigitalTwin />,
};

class SafetyManagement extends Component {
    static displayName = SafetyManagement.name;

    constructor(props) {
        super(props);

        this.state = {
            menu: 0,
        };
    }

    /* componentDidMount() {
        $(document).ready(function () {
            AOS.init();
        });
    } */


    /* menuListFun() {
        const menuList = {
            0: <Performance />,
            1: <DigitalTwin />,
        }
    }; */


    changeMenu = (menuIndex) => {
        this.setState({ menu: menuIndex });
    }

    render() {
        return (
            <>
              <div className={BIntro.buIntroBox}>
                <span className={BIntro.backColorBox}></span>
                <span className={BIntro.whiteBoxB}></span>
                <div className={BIntro.buIntroTitle}>
                    <span>안전관리</span>
                    <h4 data-aos="fade-up" data-aos-duration="1000">안전관리</h4>
                    <span>전문 기술로 공간의 안전 가치를 높이는 (주)유엔이</span>
                </div>
                <div className={BIntro.buIntroBottom}>
                    <div className={BIntro.buIntroFlex1}>
                        <span>이미지 example</span>
                    </div>
                    <div className={BIntro.buIntroFlex2}>
                        <span>이미지 example</span>
                    </div>
                    <div className={BIntro.buIntroFlex3}>
                        <span>이미지 example</span>
                    </div>
                    <div className={BIntro.buIntroFlex4}>
                        <span>이미지 example</span>
                    </div>
                </div>
                {/* <div className="menuBar">
                    <ul className="tabs">
                        <li className={`${this.state.menu === 0 ? 'active' : ''}`} onClick={() => this.changeMenu(0)}>Performance</li>
                        <li className={`${this.state.menu === 1 ? 'active' : ''}`} onClick={() => this.changeMenu(1)}>DigitalTwin</li>
                    </ul>
                </div>
                <div className="contentArea">
                    {menuList[this.state.menu]}
                </div> */}
            </div>
            </>
        );
    }
}

export default SafetyManagement;