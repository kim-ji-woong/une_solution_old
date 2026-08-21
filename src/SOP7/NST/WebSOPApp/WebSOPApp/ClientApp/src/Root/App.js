import React, { Component } from 'react';
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';
import Menu from './menu';
import LoginPage from '../Account/ui/loginPage';

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
        <Router>
            <Switch>
                <Route exact path='/' component={LoginPage} />
                <Route path={Menu.pathSDMS} component={Menu} />
            </Switch>
        </Router>
    );
  }
}
