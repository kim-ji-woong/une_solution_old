import React, { Component } from 'react';
import HistoryResource from "../resource/id";
import UserHistory from './userHistory';
import SensorDetectHistory from './SensorDetectHistory';
import SensorDetectAnalysis from './SensorDetectAnalysis';
import SOPHistory from './SOPHistory';
import SpreadHistory from './SpreadHistory';
import { SDMSController } from '../../SDMS/services/sdmsController';

class History extends Component {
    
    constructor(props) {
        super(props);

        this.state = {
            content: HistoryResource.ID.menu.sensorDetectHistory,
            buildingGroupList: null,

            linkSOP: null,/*{ beginTime: null, actionStepHistoryID: -1 },*/

            prevProps: null
        }

        this.props = props;
    }

    componentDidMount() {
        this.loadSpatialData();
    }

    async loadSpatialData() {
        const [buildingGroupList, outdoorZones, errorMessage] = await SDMSController.requestBuildingGroupList();
        console.log(buildingGroupList);
        console.log(outdoorZones);

        this.setState({ buildingGroupList });
    }

    changeContent = (content, param) => {
        if (content === HistoryResource.ID.menu.sopHistory && param && param.beginTime && param.actionStepHistoryID) {
            this.setState({ content, linkSOP: param });
        }
        else {
            this.setState({ content, linkSOP: null });
        }
    }

    getMenuUI() {
        let menu = [];
        if (this.state.content === HistoryResource.ID.menu.userHistory) {
            menu.push(<UserHistory key='history_UserHistory' changeContent={this.changeContent} />);
        }
        else if (this.state.content === HistoryResource.ID.menu.sensorDetectHistory) {
            menu.push(<SensorDetectHistory key='history_SensorDetectHistory' changeContent={this.changeContent} buildingGroupList={this.state.buildingGroupList} displayLinkSOP={this.displayLinkSOP}/>);
        }
        else if (this.state.content === HistoryResource.ID.menu.sensorDetectAnalysis) {
            menu.push(<SensorDetectAnalysis key='history_SensorDetectAnalysis' changeContent={this.changeContent} buildingGroupList={this.state.buildingGroupList}/>)
        }
        else if (this.state.content === HistoryResource.ID.menu.sopHistory) {
            menu.push(<SOPHistory key='history_SOPHistory' changeContent={this.changeContent} linkSOP={this.state.linkSOP}/>);
        }
        else if (this.state.content === HistoryResource.ID.menu.spreadHistory) {
            menu.push(<SpreadHistory key='history_SpreadHistory' changeContent={this.changeContent} />)
        }

        return menu;
    }

   render() {
        const menuUI = this.getMenuUI();
        return (
            <>
                {menuUI}
            </>
        );
    }
}

export default History;