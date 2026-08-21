import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import Home from '../src/components/Home.js';

import AboutUnE from '../src/CompanyIntro/AboutUnE.jsx';
import Vision from '../src/CompanyIntro/Vision.jsx';
import History from '../src/CompanyIntro/History.jsx';
import PatentTab from '../src/CompanyIntro/PatentTab.jsx';
import Certified from '../src/CompanyIntro/Certified.jsx';
import Chart from '../src/CompanyIntro/Chart.jsx';
import CI from '../src/CompanyIntro/CI.jsx';
import Directions from '../src/CompanyIntro/Directions.jsx';

import SafetyManagement from '../src/BusinessIntro/SafetyManagement.jsx';
import DigitalTwin from '../src/BusinessIntro/DigitalTwin.jsx';
import Performance from '../src/BusinessIntro/Performance.jsx';

import Video from '../src/CompanyNews/Video.jsx';
import Report from '../src/CompanyNews/Report.jsx';

import Recruitment from '../src/CustomerSupport/Recruitment.jsx';
import ClientCompany from '../src/CustomerSupport/ClientCompany.jsx';
import Inquiry from '../src/CustomerSupport/Inquiry.jsx';


/* import { FetchData } from './components/FetchData';
import { Counter } from './components/Counter'; */

import './custom.css'

import ProjectResource from '../src/components/resource/id';
import { withRouter } from 'react-router-dom';


class App extends Component {
  static displayName = App.name;

    render() {

       //const path = this.props.match.path;

    return (
      <Layout>
        <Route exact path={ProjectResource.path.root} component={Home} />
        <Route path={ProjectResource.path.AboutUnE}  component={AboutUnE} />
        <Route path={ProjectResource.path.Vision} component={Vision} />
        <Route path={ProjectResource.path.History} component={History} />
        <Route path={ProjectResource.path.PatentTab} component={PatentTab} />
        <Route path={ProjectResource.path.Chart} component={Chart} />
        <Route path={ProjectResource.path.CI} component={CI} />
        <Route path={ProjectResource.path.Directions} component={Directions} />
        <Route path={ProjectResource.path.SafetyManagement} component={SafetyManagement} />
        <Route path={ProjectResource.path.DigitalTwin} component={DigitalTwin} />
        <Route path={ProjectResource.path.Performance} component={Performance} />
        <Route path={ProjectResource.path.Video} component={Video} />
        <Route path={ProjectResource.path.Report} component={Report} />
        <Route path={ProjectResource.path.Recruitment} component={Recruitment} />
        <Route path={ProjectResource.path.ClientCompany} component={ClientCompany} />
        <Route path={ProjectResource.path.Inquiry} component={Inquiry} /> 

      </Layout>
    );
  }
}

export default withRouter(App);
