import React, { Component } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import './css/navMenu.css';

export class NavMenu extends Component {
    static Menu_None = 0;
    static Menu_Login = 1;
    static Menu_SpaceInfo = 2;

    static SubMenu_None = 0;
    static SubMenu_NewRegist = 1;

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

    processLogin = () => {
        if (this.props.loginData?.user) {
            this.props.onLogout(this.props.loginData.user);
        }
        else {
            this.props.onLogin();
        }
    }

    render() {
        const loginMenu = this.props.loginData?.user ? this.props.loginData.user.name + " 로그아웃" : "로그인";

        return (
          <header>
            {/* <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" light id="navBottom"> */}
                <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow" >
             {/* <Container> */}
                <NavbarBrand tag={Link} to="/">센서와 공간정보 편집을 위한 도구</NavbarBrand>
                <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
                <div className="loginBoxArea" onClick={this.processLogin}>{loginMenu}</div>
                {/*( <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar> */}
                <Collapse isOpen={!this.state.collapsed} navbar>
                {/* <ul className="navbar-nav flex-grow"> */}
                  <ul className="navList">
                    <NavItem className="navListItem">
                        <NavLink className="navListLo"><span className="navHomeIcon"></span><span className="navHomeText">HOME</span></NavLink>
                        <NavLink className="navListLo"><span className="navLockIcon"></span><span className="navLockText">LOCK</span></NavLink>
                        <NavLink className="navListLo"><span className="navAreaIcon" ></span><span className="navAreaText" tag={Link} to="/space">AREA INFO</span></NavLink>
                    </NavItem>
                    {
                        this.props.loginData?.user?.isAdmin && (
                            <NavItem>
                                <NavLink tag={Link} className="text-dark" to="/member">사용자관리</NavLink>
                            </NavItem>
                        )
                    }
                  </ul>
                </Collapse>
            {/*  </Container> */}
            </Navbar>
          </header>
        );
      }
}
