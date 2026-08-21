import React, { Component } from 'react';
import { Container } from 'reactstrap';
import { NavMenu } from './navMenu';
import common from './css/common.module.css';

export class Layout extends Component {
  
  render () {
    return (
      <div>
        <NavMenu loginData={this.props.loginData} menu={this.props.menu} onLogin={this.props.onLogin} options={this.props.options} onLogout={this.props.onLogout}/>
        {/* <Container id={common.containerSize}> */}
          <div>
           {this.props.children}
          </div>
         {/*  </Container> */}
      </div>
    );
  }
}
