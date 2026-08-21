import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import cIntro from '../CompanyIntro/css/company.module.css';
import $ from 'jquery';


class Patent extends Component {
    static displayName = Patent.name;

    constructor(props) {
        super(props);
    }

    render() {
        return (
            <>
                <div className={cIntro.PatentBox}>
                    <div className={cIntro.ParFirst}>
                        <span className={cIntro.ParFirstBox1}></span>
                        <span className={cIntro.ParFirstBox2}></span>
                        <span className={cIntro.ParFirstBox3}></span>
                        <span className={cIntro.ParFirstBox4}></span>
                    </div>
                    <div className={cIntro.ParSecond}>
                        <span className={cIntro.ParSecondBox1}></span>
                        <span className={cIntro.ParSecondBox2}></span>
                        <span className={cIntro.ParSecondBox3}></span>
                        <span className={cIntro.ParSecondBox4}></span>
                    </div>
                    <div className={cIntro.ParThird}>
                        <span className={cIntro.ParThirdBox1}></span>
                        <span className={cIntro.ParThirdBox2}></span>
                        <span className={cIntro.ParThirdBox3}></span>
                        <span className={cIntro.ParThirdBox4}></span>
                    </div>
                    <div className={cIntro.ParFourth}>
                        <span className={cIntro.ParFourthBox1}></span>
                        <span className={cIntro.ParFourthBox2}></span>
                        <span className={cIntro.ParFourthBox3}></span>
                        <span className={cIntro.ParFourthBox4}></span>
                    </div>
                </div>
            </>
        );
    }
}

export default Patent;