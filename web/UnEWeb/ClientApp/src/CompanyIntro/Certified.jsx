import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import cIntro from '../CompanyIntro/css/company.module.css';
import $ from 'jquery';



class Certified extends Component {
    static displayName = Certified.name;

    constructor(props) {
        super(props);
    }

    render() {
        return (
            <>
                <div className={cIntro.CertifiedBox}>
                    <span>인증</span>
                    <div className={cIntro.CertifiedFirst}>
                        <span className={cIntro.CertifiedFirstBox1}></span>
                        <span className={cIntro.CertifiedFirstBox2}></span>
                        <span className={cIntro.CertifiedFirstBox3}></span>
                        <span className={cIntro.CertifiedFirstBox4}></span>
                    </div>
                    <div className={cIntro.CertifiedSecond}>
                        <span className={cIntro.CertifiedSecondBox1}></span>
                        <span className={cIntro.CertifiedSecondBox2}></span>
                        <span className={cIntro.CertifiedSecondBox3}></span>
                        <span className={cIntro.CertifiedSecondBox4}></span>
                    </div>
                </div>
                <div className={cIntro.CertifiedBox2}>
                    <span>인증</span>
                    <div className={cIntro.CertifiedThird}>
                        <span className={cIntro.CertifiedThirdBox1}></span>
                        <span className={cIntro.CertifiedThirdBox2}></span>
                        <span className={cIntro.CertifiedThirdBox3}></span>
                        <span className={cIntro.CertifiedThirdBox4}></span>
                    </div>
                </div>
            </>
        );
    }
}

export default Certified;