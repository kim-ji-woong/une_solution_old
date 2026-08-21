import React, { Component } from 'react';
import $ from 'jquery';

import OperationBox from './operationBox';
import BoardView from './boardView';
import WeatherBox from './weatherBox';
import WeeklyStatus from './weeklyStatus';
import AlarmInfomation from './alarmInfomation';
import DashboardResource from '../resource/id';
import ProjectResource from '../../Root/resource/id';

import dashboard from '../css/dashboardNew.module.css';

class Mainboard extends Component {
    constructor(props) {
        super(props);

        this.state = {
            type: DashboardResource.displayInfoType.FIRE,
        }

        this.props = props;
    }

    changeType = (type) => {
        console.log(type);

        this.setState({ type: type});
    }

    displaySiteUI = () => {
        const siteID = ProjectResource.SiteID;
        let displaySiteUI = [];

        if (siteID === ProjectResource.Site.GCC) {
            {/* 녹십자 레이아웃 */ }
            displaySiteUI.push(
                <div className={dashboard.leftFlexAreaGC}>

                    <BoardView
                        type={this.state.type}
                        changeType={this.changeType}
                        todayAlarms={this.props.todayAlarms}
                        selectSensors={this.props.selectSensors}
                        buildingGroupList={this.props.buildingGroupList}
                    />

                    <WeatherBox />

                    <WeeklyStatus
                        todayAlarms={this.props.todayAlarms}
                        type={this.state.type}
                        changeType={this.changeType}
                        weeklyAlarms={this.props.weeklyAlarms}
                    />

                    <AlarmInfomation
                        selectSensors={this.props.selectSensors}
                        type={this.state.type}
                        changeType={this.changeType}
                        changeMode={this.props.changeMode}
                        selectWeeklyAlarms={this.props.selectWeeklyAlarms}
                        materials={this.props.materials}
                    />

                </div>
            );
        } else {
            {/* 솔브레인 레이아웃 */ }
            displaySiteUI.push(
                <div className={dashboard.leftFlexArea}>

                    <OperationBox currentWork={this.props.currentWork} />

                    <WeatherBox />

                    <BoardView
                        type={this.state.type}
                        changeType={this.changeType}
                        todayAlarms={this.props.todayAlarms}
                        selectSensors={this.props.selectSensors}
                        buildingGroupList={this.props.buildingGroupList}
                    />
                    <WeeklyStatus
                        todayAlarms={this.props.todayAlarms}
                        type={this.state.type}
                        changeType={this.changeType}
                        weeklyAlarms={this.props.weeklyAlarms}
                    />

                    <AlarmInfomation
                        selectSensors={this.props.selectSensors}
                        type={this.state.type}
                        changeType={this.changeType}
                        changeMode={this.props.changeMode}
                        selectWeeklyAlarms={this.props.selectWeeklyAlarms}
                        materials={this.props.materials}
                    />
                </div>
            );
        }

        return displaySiteUI;
    }

    render() {

        const displaySiteUI = this.displaySiteUI();

        return (
            <>
                {   /* 사이트별 UI */
                    displaySiteUI
                }
            </>
        );
    }
}
export default Mainboard;