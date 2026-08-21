import React, { Component } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
//import { Link } from 'react-router-dom';
import { BrowserRouter as Route, Link } from 'react-router-dom';
import $ from 'jquery';

/* import './NavMenu.css'; */
import nav from '../components/css/navMenu.module.css';
import ProjectResource from '../components/resource/id';
import home from '../components/css/home.module.css';

export class NavMenu extends Component {
  static displayName = NavMenu.name;

  constructor (props) {
    super(props);

    this.toggleNavbar = this.toggleNavbar.bind(this);
    this.state = {
      collapsed: true
    };
  }

  toggleNavbar () {
    this.setState({
      collapsed: !this.state.collapsed
    });
    }

    componentDidMount() {
       $('#' + nav.menu1).mouseenter(function () {
            $('.' + nav.menu2).show();
            $('.' + home.overlay).show();
        });
        $('#' + nav.menu1).mouseleave(function () {
            $('.' + nav.menu2).hide();
            $('.' + home.overlay).hide();
        });

        $('.' + nav.sub).click(function () {
            $('.' + nav.menu2).hide();
        });
    }


  render () {
    return (
        <>
             <div id={nav.menu1}>
                <Link to="/"><span className={nav.menuLogo}></span></Link> {/* 메인으로 */}
                <ul className={nav.menu}>
                    <span className={nav.menuWidth}>
                        <li clasName={nav.menuBorder}><Link to="/aboutUnE">회사소개</Link></li>
                        <li clasName={nav.menuBorder}><Link to="/safetyManagement">사업분야</Link></li>
                        <li clasName={nav.menuBorder}><Link to="/video">회사소식</Link></li>
                        <li clasName={nav.menuBorder}><Link to="/">고객지원</Link></li>
                        <li clasName={nav.menuBorder}><Link to="/">인재채용</Link></li>
                    </span>
                </ul>
                <div className={nav.menu2}>
                    <div className={nav.menuContent}>
                        <ul className={nav.sub}>
                            <li><Link to="/aboutUnE">About U&E</Link></li>
                            <li><Link to="/">보유기술</Link></li>
                            <li><Link to="/video">보도자료</Link></li>
                            <li><Link to="/clientCompany">고객사</Link></li>
                            <li><Link to="/">채용정보</Link></li>
                        </ul>
                        <ul className={nav.sub}>
                            <li><Link to="/history">연혁</Link></li>
                            <li><Link to="/safetyManagement">안전관리</Link></li>
                            <li><Link to="/report">월간유엔이</Link></li>
                            <li><Link to="/inquiry">문의하기</Link></li>
                            <li><Link to="/">복리후생</Link></li>
                        </ul>
                        <ul className={nav.sub}>
                            <li><Link to="/PatentTab">특허 및 인증</Link></li>
                            <li><Link to="/digitalTwin">디지털 트윈</Link></li>
                        </ul>
                        <ul className={nav.sub}>
                            <li><Link to="/chart">조직도</Link></li>
                            <li><Link to="/">주요 실적</Link></li>
                        </ul>
                        <ul className={nav.sub}>
                            <li><Link to="/cI">C.I</Link></li>
                        </ul>
                        <ul className={nav.sub}>
                            <li><Link to="/directions">오시는 길</Link></li>
                        </ul>
                   </div>
                </div>
             </div>
        </>
    );
  }
}
