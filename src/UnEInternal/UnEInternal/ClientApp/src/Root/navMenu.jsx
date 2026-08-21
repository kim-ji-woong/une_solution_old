import React, { Component } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import './css/NavMenu.css';


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

    processLogin = () => {
        if (this.props.loginUser) {
            this.props.onLogout(this.props.loginUser);
        }
        else {
            this.props.onLogin();
        }
    }

    render() {
        const loginMenu = this.props.loginUser ? this.props.loginUser.userID + " 로그아웃" : "로그인";


        return (
            <>
          <header>
            <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" light>
              <Container>
                <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
                <NavbarBrand tag={Link} to="/">U&E Internal</NavbarBrand>
                <div className="loginBoxArea" onClick={this.processLogin}>{loginMenu}</div>
                <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
                    <ul className="navbar-nav flex-grow">
                    <NavItem>
                    <NavLink tag={Link} className="text-dark" to="/vacation">휴가관리</NavLink>
                    </NavItem>
                    {
                        this.props.loginUser?.isAdmin && (
                            <NavItem>
                                 <NavLink tag={Link} className="text-dark" to="/teams">조직관리</NavLink>
                            </NavItem>
                        )
                    }
                  </ul>
                </Collapse>
              </Container>
            </Navbar>
          </header>
          </>
        );
      }
}
