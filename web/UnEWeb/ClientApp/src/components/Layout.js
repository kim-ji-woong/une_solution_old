import React, { Component } from 'react';
//import { Container } from 'reactstrap';
import { NavMenu } from './NavMenu';
import Footer from '../components/footer.jsx';
import home from '../components/css/home.module.css';

export class Layout extends Component {
  static displayName = Layout.name;

  render () {
      return (
       <>
          <div className={home.bodyArea}>
             <NavMenu />
             <div className={home.mainArea}>
                {this.props.children}
             </div>
          </div>
          <div id={home.footArea}>
           <Footer />
          </div>
       </>
    );
  }
}
