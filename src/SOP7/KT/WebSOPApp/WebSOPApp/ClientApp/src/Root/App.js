import React, { Component } from 'react';
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';
import Menu from './menu';
import RootResource from './resource/id';
import LoginPage from '../Account/ui/loginPage';

export default class App extends Component {
  static displayName = App.name;

    render() {
        return (
          <Router>
                <Switch>
                    <Route exact path={RootResource.path.root} component={LoginPage} />
                    <Route path={RootResource.path.sopSimulator} component={Menu} />
                </Switch>
          </Router>
        );
  }
}
