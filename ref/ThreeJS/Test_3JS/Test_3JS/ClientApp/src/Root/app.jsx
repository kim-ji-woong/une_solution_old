import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from '../components/Layout';
import { Home } from '../components/Home';
import { BodyManager } from '../3D/bodyManager';

import './css/custom.css';

function App() {
    return (
        <Layout>
            <Route exact path='/' component={Home} />
            <Route path='/3d' component={BodyManager} />
        </Layout>
    );
}

export default App;
