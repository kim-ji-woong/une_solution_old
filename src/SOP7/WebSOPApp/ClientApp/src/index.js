//import 'bootstrap/dist/css/bootstrap.css';
import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter } from 'react-router-dom';
import App from './Root/app';
import registerServiceWorker from './registerServiceWorker';
//import './Root/css/index.css';

//import './Common/css/default.css';
//import './Common/css/slick.css';
//import './Common/css/animate.min.css';
//import './Common/css/common.css';
//import './Common/css/style.css';

//import './Common/css/scroll.css';
//import './Common/css/section.css';
//import './Common/css/treeview.css';

//import './Common/js/jquery-2.2.1.min.js';
//import './Common/js/placeholders.min.js';
//import './Common/js/slick.min.js';
//import './Common/js/common.js';
//import './Common/js/treeview.js';

const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');
const rootElement = document.getElementById('root');

ReactDOM.render(
    <BrowserRouter basename={baseUrl}>
        <App />
    </BrowserRouter>,
    rootElement);

registerServiceWorker();

