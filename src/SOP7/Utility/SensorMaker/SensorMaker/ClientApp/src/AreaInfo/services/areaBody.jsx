import React, { Component } from 'react';

import area from '../css/areaInfo.module.css';
import { BasicInfo } from './basicInfo';
import { AreaInfo } from './areaInfo';
import { ModelUpload } from './modelUpload';
import { ModelView } from './modelView';
import { SensorEdit } from './sensorEdit';
import { EquipCCTV } from './equipCCTV';
import { PoiEdit } from './poiEdit';


class AreaBody extends Component {
    constructor(props) {
        super(props);

        this.props = props;
    }

render(){
    return(
        <div className={area.areaBody}>?
        </div>
    );
   }
} 

export default AreaBody;
