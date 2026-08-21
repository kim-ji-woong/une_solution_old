import React, { Component } from 'react';
import { useState, useEffect } from "react";
import $ from 'jquery';

/* aos */
/* import React, { useEffect } from 'react';
import AOS from "aos";
import "aos/dist/aos.css"; */


class AOS extends Component {

    componentDidMount() {

        AOS.init();
    };

    render() {
        const boxStyle = {
            width: '40%',
            height: '200px',
            fontSize: '30px',
            lineHeight: '200px',
            textAlign: 'center'
        };

        /* useEffect(() => {
              AOS.init({
                  duration : 1000
              });
          }); */

        return (
            <>
                <div>
                    <p data-aos="fade-up" data-aos-duration="600">AOS 테스트1</p>
                </div>
                <div style={boxStyle}>
                    <p data-aos="fade-down" data-aos-duration="1000">AOS 테스트2</p>
                </div>
            </>
        );
    }
}
export default AOS;