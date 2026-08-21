import React, { Component } from 'react';
import { NavMenu } from './navMenu';
import './css/navMenu.css';

export class Layout extends Component {
  static displayName = Layout.name;

  render () {
    return (
      <div className="mainBox">
        <NavMenu loginUser={this.props.loginUser} onLogin={this.props.onLogin} options={this.props.options} onLogout={this.props.onLogout} currentMenu={this.props.currentMenu} onSelectMenu={this.props.onSelectMenu}/>
        {this.props.children}
      </div>
    );
  }
}
