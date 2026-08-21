import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import home from '../components/css/home.module.css';
import $ from 'jquery';
import { Link } from "react-router-dom";

import 'owl.carousel/dist/assets/owl.carousel.css';
import 'owl.carousel/dist/assets/owl.theme.default.css';
import OwlCarousel from 'react-owl-carousel';


class Carousel extends Component {
    state = {
        responsive: {
            0: {
                items: 1,
            },
            450: {
                items: 2,
            },
            600: {
                items: 3,
            },
            1000: {
                items: 4,
            },
        },
    }

    componentDidMount(){

    }


    render() {
        return (
            <OwlCarousel className={'owl-theme'}
            loop={true}
            margin={10}
            nav={true}
            dots={false}
            autoplay={true}
            autoplayTimeout={2000}
            items={4}
            responsive={this.state.responsive} >

            <div className={'item'}>
                Item 1
            </div>
            <div className={'item'}>
                Item 2
            </div>
            <div className={'item'}>
                Item 3
            </div>
            <div className={'item'}>
                Item 4
            </div>
            <div className={'item'}>
                Item 5
            </div>
            </OwlCarousel>
       )
    }
}


export default Carousel;