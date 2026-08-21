import React, { Component } from 'react';
import $ from 'jquery';

import AlarmComponet from './alarmComponet';
import ProjectResource from '../../Root/resource/id';

import dashboard from '../css/dashboardNew.module.css';
import dashboardImage from '../css/image/solbrain06.jpg';
import dashboardGCImage from '../css/image/GC_topview_04.png';

class BoardView extends Component {
    constructor(props) {
        super(props);

        this.props = props;
    }

    componentDidMount() {
        //const resizableBoardViewColumn = "." + dashboard.boardView + " " + " ." + dashboard.weeklyStatus;
        //const resizableBoardViewRow = "." + dashboard.boardView;
        //const boardView = this;

        //$(function () {
        //    $(resizableBoardViewRow).resizable({
        //        direction: 'bottom'
        //    });
        //});

        //$(function () {
        //    $(resizableBoardViewColumn).resizable({
        //        direction: 'left'
        //    });
        //});
    };

    displaySiteUI = () => {
        const siteID = ProjectResource.SiteID;
        let displaySiteUI = [];

        if (siteID === ProjectResource.Site.GCC) {
            {/* 녹십자 */ }
            displaySiteUI.push(
                <div className={dashboard.boardViewGC}>
                    <img src={dashboardGCImage} alt="녹십자 항공사진" className={dashboard.dashboardImageGC} />

                    <div className={dashboard.sensorBoxGC} id={dashboard.BoxGC1}>
                        <div className={dashboard.redCircle1}></div>
                        <div className={dashboard.redLine1}></div>
                        <AlarmComponet buildingGroupID={null} buildingID={14} type={this.props.type} changeType={this.props.changeType} todayAlarms={this.props.todayAlarms} selectSensors={this.props.selectSensors} buildingGroupList={this.props.buildingGroupList} />
                    </div>
                    <div className={dashboard.sensorBoxGC} id={dashboard.BoxGC2}>
                        <div className={dashboard.redCircle2}></div>
                        <div className={dashboard.redLine2}></div>
                        <AlarmComponet buildingGroupID={null} buildingID={3} type={this.props.type} changeType={this.props.changeType} todayAlarms={this.props.todayAlarms} selectSensors={this.props.selectSensors} buildingGroupList={this.props.buildingGroupList} />
                    </div>
                    <div className={dashboard.sensorBoxGC} id={dashboard.BoxGC3}>
                        <div className={dashboard.redCircle3}></div>
                        <div className={dashboard.redLine3}></div>
                        <AlarmComponet buildingGroupID={null} buildingID={4} type={this.props.type} changeType={this.props.changeType} todayAlarms={this.props.todayAlarms} selectSensors={this.props.selectSensors} buildingGroupList={this.props.buildingGroupList} />
                    </div>
                    <div className={dashboard.sensorBoxGC} id={dashboard.BoxGC4}>
                        <div className={dashboard.redCircle4}></div>
                        <div className={dashboard.redLine4}></div>
                        <AlarmComponet buildingGroupID={null} buildingID={11} type={this.props.type} changeType={this.props.changeType} todayAlarms={this.props.todayAlarms} selectSensors={this.props.selectSensors} buildingGroupList={this.props.buildingGroupList} />
                    </div>
                    <div className={dashboard.sensorBoxGC} id={dashboard.BoxGC5}>
                        <div className={dashboard.redCircle5}></div>
                        <div className={dashboard.redLine5}></div>
                        <AlarmComponet buildingGroupID={null} buildingID={6} type={this.props.type} changeType={this.props.changeType} todayAlarms={this.props.todayAlarms} selectSensors={this.props.selectSensors} buildingGroupList={this.props.buildingGroupList} />
                    </div>
                    <div className={dashboard.sensorBoxGC} id={dashboard.BoxGC6}>
                        <div className={dashboard.redCircle6}></div>
                        <div className={dashboard.redLine6}></div>
                        <AlarmComponet buildingGroupID={null} buildingID={7} type={this.props.type} changeType={this.props.changeType} todayAlarms={this.props.todayAlarms} selectSensors={this.props.selectSensors} buildingGroupList={this.props.buildingGroupList} />
                    </div>
                    <div className={dashboard.sensorBoxGC} id={dashboard.BoxGC7}>
                        <div className={dashboard.redCircle7}></div>
                        <div className={dashboard.redLine7}></div>
                        <AlarmComponet buildingGroupID={null} buildingID={2} type={this.props.type} changeType={this.props.changeType} todayAlarms={this.props.todayAlarms} selectSensors={this.props.selectSensors} buildingGroupList={this.props.buildingGroupList} />
                    </div>
                    <div className={dashboard.sensorBoxGC} id={dashboard.BoxGC8}>
                        <div className={dashboard.redCircle8}></div>
                        <div className={dashboard.redLine8}></div>
                        <AlarmComponet buildingGroupID={null} buildingID={1} type={this.props.type} changeType={this.props.changeType} todayAlarms={this.props.todayAlarms} selectSensors={this.props.selectSensors} buildingGroupList={this.props.buildingGroupList} />
                    </div>
                    <div className={dashboard.sensorBoxGC} id={dashboard.BoxGC9}>
                        <div className={dashboard.redCircle9}></div>
                        <div className={dashboard.redLine9}></div>
                        <AlarmComponet buildingGroupID={null} buildingID={5} type={this.props.type} changeType={this.props.changeType} todayAlarms={this.props.todayAlarms} selectSensors={this.props.selectSensors} buildingGroupList={this.props.buildingGroupList} />
                    </div>
                    <div className={dashboard.sensorBoxGC} id={dashboard.BoxGC10}>
                        <div className={dashboard.redCircle10}></div>
                        <div className={dashboard.redLine10}></div>
                        <AlarmComponet buildingGroupID={null} buildingID={15} type={this.props.type} changeType={this.props.changeType} todayAlarms={this.props.todayAlarms} selectSensors={this.props.selectSensors} buildingGroupList={this.props.buildingGroupList} />
                    </div>
                    <div className={dashboard.sensorBoxGC} id={dashboard.BoxGC11}>
                        <div className={dashboard.redCircle11}></div>
                        <div className={dashboard.redLine11}></div>
                        <AlarmComponet buildingGroupID={null} buildingID={12} type={this.props.type} changeType={this.props.changeType} todayAlarms={this.props.todayAlarms} selectSensors={this.props.selectSensors} buildingGroupList={this.props.buildingGroupList} />
                    </div>
                    <div className={dashboard.sensorBoxGC} id={dashboard.BoxGC12}>
                        <div className={dashboard.redCircle12}></div>
                        <div className={dashboard.redLine12}></div>
                        <AlarmComponet buildingGroupID={null} buildingID={20} type={this.props.type} changeType={this.props.changeType} todayAlarms={this.props.todayAlarms} selectSensors={this.props.selectSensors} buildingGroupList={this.props.buildingGroupList} />
                    </div>
                    <div className={dashboard.sensorBoxGC} id={dashboard.BoxGC13}>
                        <div className={dashboard.redCircle13}></div>
                        <div className={dashboard.redLine13}></div>
                        <AlarmComponet buildingGroupID={null} buildingID={8} type={this.props.type} changeType={this.props.changeType} todayAlarms={this.props.todayAlarms} selectSensors={this.props.selectSensors} buildingGroupList={this.props.buildingGroupList} />
                    </div>
                    <div className={dashboard.sensorBoxGC} id={dashboard.BoxGC14}>
                        <div className={dashboard.redCircle14}></div>
                        <div className={dashboard.redLine14}></div>
                        <AlarmComponet buildingGroupID={null} buildingID={10} type={this.props.type} changeType={this.props.changeType} todayAlarms={this.props.todayAlarms} selectSensors={this.props.selectSensors} buildingGroupList={this.props.buildingGroupList} />
                    </div>
                    <div className={dashboard.sensorBoxGC} id={dashboard.BoxGC15}>
                        <div className={dashboard.redCircle15}></div>
                        <div className={dashboard.redLine15}></div>
                        <AlarmComponet buildingGroupID={null} buildingID={16} type={this.props.type} changeType={this.props.changeType} todayAlarms={this.props.todayAlarms} selectSensors={this.props.selectSensors} buildingGroupList={this.props.buildingGroupList} />
                    </div>
                    <div className={dashboard.sensorBoxGC} id={dashboard.BoxGC16}>
                        <div className={dashboard.redCircle16}></div>
                        <div className={dashboard.redLine16}></div>
                        <AlarmComponet buildingGroupID={null} buildingID={17} type={this.props.type} changeType={this.props.changeType} todayAlarms={this.props.todayAlarms} selectSensors={this.props.selectSensors} buildingGroupList={this.props.buildingGroupList} />
                    </div>
                    <div className={dashboard.sensorBoxGC} id={dashboard.BoxGC17}>
                        <div className={dashboard.redCircle17}></div>
                        <div className={dashboard.redLine17}></div>
                        <AlarmComponet buildingGroupID={null} buildingID={13} type={this.props.type} changeType={this.props.changeType} todayAlarms={this.props.todayAlarms} selectSensors={this.props.selectSensors} buildingGroupList={this.props.buildingGroupList} />
                    </div>
                </div>
            );
        } else {
            {/* 솔브레인 */ }
            displaySiteUI.push(
                <div className={dashboard.boardView}>
                    <span className={dashboard.imageFilter} id={dashboard.def}></span>
                    <img src={dashboardImage} alt="솔브레인 항공사진" className={dashboard.dashboardImage} id={dashboard.ghi} />

                    <div className={dashboard.sensorBox}>
                        <div className={dashboard.redCircle1}></div>
                        <div className={dashboard.redLine1}></div>
                        <AlarmComponet buildingGroupID={1} buildingID={null} type={this.props.type} changeType={this.props.changeType} todayAlarms={this.props.todayAlarms} selectSensors={this.props.selectSensors} buildingGroupList={this.props.buildingGroupList} />
                    </div>
                    <div className={dashboard.sensorBox}>
                        <div className={dashboard.redCircle2}></div>
                        <div className={dashboard.redLine2}></div>
                        <AlarmComponet buildingGroupID={2} buildingID={null} type={this.props.type} changeType={this.props.changeType} todayAlarms={this.props.todayAlarms} selectSensors={this.props.selectSensors} buildingGroupList={this.props.buildingGroupList} />
                    </div>
                    <div className={dashboard.sensorBox}>
                        <div className={dashboard.redCircle3}></div>
                        <div className={dashboard.redLine3}></div>
                        <AlarmComponet buildingGroupID={3} buildingID={null} type={this.props.type} changeType={this.props.changeType} todayAlarms={this.props.todayAlarms} selectSensors={this.props.selectSensors} buildingGroupList={this.props.buildingGroupList} />
                    </div>
                    <div className={dashboard.sensorBox}>
                        <div className={dashboard.redCircle4}></div>
                        <div className={dashboard.redLine4}></div>
                        <AlarmComponet buildingGroupID={4} buildingID={null} type={this.props.type} changeType={this.props.changeType} todayAlarms={this.props.todayAlarms} selectSensors={this.props.selectSensors} buildingGroupList={this.props.buildingGroupList} />
                    </div>
                    <div className={dashboard.sensorBox}>
                        <div className={dashboard.redCircle5}></div>
                        <div className={dashboard.redLine5}></div>
                        <AlarmComponet buildingGroupID={5} buildingID={null} type={this.props.type} changeType={this.props.changeType} todayAlarms={this.props.todayAlarms} selectSensors={this.props.selectSensors} buildingGroupList={this.props.buildingGroupList} />
                    </div>
                    <div className={dashboard.sensorBox}>
                        <div className={dashboard.redCircle6}></div>
                        <div className={dashboard.redLine6}></div>
                        <AlarmComponet buildingGroupID={6} buildingID={null} type={this.props.type} changeType={this.props.changeType} todayAlarms={this.props.todayAlarms} selectSensors={this.props.selectSensors} buildingGroupList={this.props.buildingGroupList} />
                    </div>
                    <div className={dashboard.sensorBox}>
                        <div className={dashboard.redCircle7}></div>
                        <div className={dashboard.redLine7}></div>
                        <AlarmComponet buildingGroupID={7} buildingID={null} type={this.props.type} changeType={this.props.changeType} todayAlarms={this.props.todayAlarms} selectSensors={this.props.selectSensors} buildingGroupList={this.props.buildingGroupList} />
                    </div>
                    <div className={dashboard.sensorBox}>
                        <div className={dashboard.redCircle7_1}></div>
                        <div className={dashboard.redLine7_1}></div>
                        <AlarmComponet buildingGroupID={13} buildingID={null} type={this.props.type} changeType={this.props.changeType} todayAlarms={this.props.todayAlarms} selectSensors={this.props.selectSensors} buildingGroupList={this.props.buildingGroupList} />
                    </div>
                    <div className={dashboard.sensorBox}>
                        <div className={dashboard.redCircle8}></div>
                        <div className={dashboard.redLine8}></div>
                        <AlarmComponet buildingGroupID={10} buildingID={null} type={this.props.type} changeType={this.props.changeType} todayAlarms={this.props.todayAlarms} selectSensors={this.props.selectSensors} buildingGroupList={this.props.buildingGroupList} />
                    </div>
                    <div className={dashboard.sensorBox}>
                        <div className={dashboard.redCircle9}></div>
                        <div className={dashboard.redLine9}></div>
                        <AlarmComponet buildingGroupID={11} buildingID={null} type={this.props.type} changeType={this.props.changeType} todayAlarms={this.props.todayAlarms} selectSensors={this.props.selectSensors} buildingGroupList={this.props.buildingGroupList} />
                    </div>
                    <div className={dashboard.sensorBox}>
                        <div className={dashboard.redCircle10}></div>
                        <div className={dashboard.redLine10}></div>
                        <AlarmComponet buildingGroupID={12} buildingID={null} type={this.props.type} changeType={this.props.changeType} todayAlarms={this.props.todayAlarms} selectSensors={this.props.selectSensors} buildingGroupList={this.props.buildingGroupList} />
                    </div>
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
export default BoardView;