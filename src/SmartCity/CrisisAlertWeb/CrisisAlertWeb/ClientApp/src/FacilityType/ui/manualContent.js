import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';

import Title from '../../Root/title';
import { FacilityTypeController } from '../services/facilityTypeController';
import Paginate from '../../Root/paginate';

import SessionString from '../../Common/js/sessionString';
import FacilityTypeResource from '../resource/id';

import styles from '../../Common/css/style.css';

class ManualContent extends Component {
    constructor(props) {
        super(props);

        this.state = {
            menu: FacilityTypeResource.ID.manualMenu.List,
        }

        this.props = props;
        this.initLoad();
    }

    showPage = () => {
        if (this.state.menu === FacilityTypeResource.ID.manualMenu.List) {

        } else if (this.state.menu === FacilityTypeResource.ID.manualMenu.Content) {

        }
}

    render() {
        let showPage = "";
        showPage = this.showPage();

        return (
            <>

  

       
                

            </>
        );
    }
}

export default ManualContent;
