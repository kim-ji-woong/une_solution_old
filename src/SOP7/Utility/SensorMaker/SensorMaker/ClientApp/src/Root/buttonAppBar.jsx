import React, { Component } from 'react';
/* import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap'; */
import { Link } from 'react-router-dom';

import { AppBar, Box, Toolbar, Typography, Button, IconButton } from '@material-ui/core';
/* import MenuIcon from '@mui/icons-material/Menu'; */

import nav from './css/navMenu.module.css';



/* export class NavMenu extends Component {
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
                 <div className={nav.navBox}>
                  <div className={nav.navTitle} tag={Link} to="/">Sensor Maker</div>
                    <div className="loginBoxArea" onClick={this.processLogin}>{loginMenu}</div>
                    <div isOpen={!this.state.collapsed} >
                        <ul>
                            <span className={nav.navItem}>
                                <span tag={Link} to="/vacation">공간정보</span>
                            </span>
                            {
                                this.props.loginUser?.isAdmin && (
                                    <span className={nav.navItem}>
                                        <span tag={Link} to="/teams">사용자관리</span>
                                    </span>
                                )
                            }
                        </ul>
                    </div>
                </div>
          </>
        );
      }
} */



export default function ButtonAppBar() {

    const useState = React.useState;

    return (
        <Box sx={{ flexGrow: 1 }}>
            <AppBar position="static">
                <Toolbar>
                    <IconButton
                        size="large"
                        edge="start"
                        color="inherit"
                        aria-label="menu"
                        sx={{ mr: 2 }}
                    >
                    </IconButton>
                    <Typography variant="h6" component="div" sx={{ flexGrow: 1 }} tag={Link} to="/">SDMS Basic Tool</Typography>
                    <Button color="inherit" onClick={this.processLogin}>{loginMenu}</Button>
                    <Button color="inherit" tag={Link} to="/vacation">공간정보</Button>
                    {
                        this.props.loginUser?.isAdmin && (
                           <Button color="inherit" tag={Link} to="/teams">사용자관리</Button>
                        )
                    }
                </Toolbar>
            </AppBar>
        </Box>
    );
}

