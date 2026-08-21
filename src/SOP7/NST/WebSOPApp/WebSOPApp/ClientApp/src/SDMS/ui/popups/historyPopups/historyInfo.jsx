import React, { Component } from 'react';
import { BrowserRouter as Route, Link } from 'react-router-dom';
import { withRouter } from 'react-router-dom';
import styles from '../../../css/sdms.module.css';
import imgClose from '../../../image/common_Icon/popup_close.png';
import btnCalendarBk from '../../../image/history_Icon/dashboard_calendar.png';

import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';
import $ from 'jquery';
import SdmsResource from '../../../resource/id';
import { SDMSController } from '../../../services/sdmsController';

import SensorDetectHistory from './SensorDetectHistory';
import SOPHistory from './SOPHistory';
import SpreadHistory from './SpreadHistory';

class HistoryInfo extends Component {
    
    constructor(props) {
        super(props);

        this.state = {
            content: SdmsResource.ID.menu.sensorDetectHistory,
            prevProps: null
        }

        this.props = props;
    }


    componentDidMount() {
        $('.' + styles.hisPopupBoxX).click(function () {
            $('.' + styles.historyPopup).css('display', 'none');
        })
    }

    changeContent = (content) => {
        this.setState({ content });
    }

    onClose = () => {
        this.props.setVisiblePopup(this.props.popupType, false);
    }

    getMenuUI() {
        let menu = [];
        if (this.state.content === SdmsResource.ID.menu.sensorDetectHistory) {
            menu.push(
                <SensorDetectHistory key='history_SensorDetectHistory'
                    changeContent={this.changeContent}
                    onClose={this.onClose}
                    buildingGroupList={this.props.buildingGroupList}
                    popupType={this.props.popupType}
                    setVisiblePopup={this.props.setVisiblePopup}
                    setActiveDragPopup={this.props.setActiveDragPopup}
                    zIndex={this.props.zIndex}
                    popupState={this.props.popupState}
                    setPopupState={this.props.setPopupState}
                />
            );
        }
        else if (this.state.content === SdmsResource.ID.menu.sopHistory) {
            menu.push(
                <SOPHistory key='history_SOPHistory'
                    changeContent={this.changeContent}
                    onClose={this.onClose}
                    popupType={this.props.popupType}
                    setVisiblePopup={this.props.setVisiblePopup}
                    setActiveDragPopup={this.props.setActiveDragPopup}
                    zIndex={this.props.zIndex}
                    popupState={this.props.popupState}
                    setPopupState={this.props.setPopupState}
                />
            );
        }
        else if (this.state.content === SdmsResource.ID.menu.spreadHistory) {
            menu.push(
                <SpreadHistory key='history_SpreadHistory'
                    changeContent={this.changeContent}
                    onClose={this.onClose}
                    popupType={this.props.popupType}
                    setVisiblePopup={this.props.setVisiblePopup}
                    setActiveDragPopup={this.props.setActiveDragPopup}
                    zIndex={this.props.zIndex}
                    popupState={this.props.popupState}
                    setPopupState={this.props.setPopupState}
                />
            );
        }

        return menu;
    }

    render() {
        const menuUI = this.getMenuUI();

        return (
            <>
                {menuUI}
            </>
        )
    }

} export default HistoryInfo;