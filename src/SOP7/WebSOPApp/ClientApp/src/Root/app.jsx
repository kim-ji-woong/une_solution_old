import React from 'react';
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';
import MenuSB from './menuSB';

import LoginPageSB from '../Account/ui/loginPageSB';
import AccountFindPwd from '../Account/ui/popups/accountFindPwd';
import SpecialReport from '../Dashboard/ui/popups/specialReport';

import SwiperCore, { Autoplay, Navigation } from 'swiper';

import RootResource from './resource/id';

function App() {
    SwiperCore.use([Autoplay, Navigation])

    return (  
        <Router>
            <Switch>
                <Route exact path={RootResource.path.root} component={LoginPageSB} />
                <Route path={RootResource.path.sopManager} component={MenuSB} />
                <Route path={RootResource.path.teamEditor} component={MenuSB} />
                <Route path={RootResource.path.sopSimulator} component={MenuSB} />
                <Route path={RootResource.path.sdms} component={MenuSB} />
                <Route path={RootResource.path.dashboard} component={MenuSB} />
                <Route path={RootResource.path.findPassword} component={AccountFindPwd} />
                <Route path={RootResource.path.history} component={MenuSB} />
                <Route path={RootResource.path.specialReport} component={SpecialReport} />
            </Switch>
        </Router>

  );
}

export default App;
