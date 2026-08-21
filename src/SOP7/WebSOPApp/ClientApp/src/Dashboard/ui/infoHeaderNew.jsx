import { ui } from 'jquery';
import React, { Component } from 'react';
import $ from 'jquery';
import store from '../../Root/store';

import dashboard from '../css/dashboardNew.module.css';

import DashboardResource from '../resource/id';
import ProjectResource from '../../Root/resource/id';

class InfoHeader extends Component {
    static arrDayStr = ['일', '월', '화', '수', '목', '금', '토'];

    constructor(props) {
        super(props);

        this.state = {
            date: null,
            time: null,
            selectDay: [],
        }

        this.props = props;
        this.initDate();

        store.subscribe(function () {
            let data = store.getState();

            if (data.actionType === "WEATHER_CURRENT") {
                // 1분마다 date 갱신
                this.reloadDate();
            }
        }.bind(this));
    }

    setSensorCount() {
        if (this.props.selectSensors === null || this.props.selectSensors === undefined) {
            return ["-", "-", "-", "-", "-", "-", "-", "-", "-", "-"];
        }

        const selectSensors = this.props.selectSensors;

        let fireSensorCount = selectSensors.fireSensors.length;
        let disabledFireSensorCount = selectSensors.disabledFireSensors.length;
        let psmSensorCount = selectSensors.psmSensors.length;
        let disabledPSMSensorCount = selectSensors.disabledPSMSensors.length;
        let etcSensorCount = selectSensors.etcSensors.length;
        let disabledEtcSensorCount = selectSensors.disabledEtcSensors.length;
        let cctvCount = selectSensors.cctvs.length;
        let disabledCCTVCount = selectSensors.disabledCCTVs.length;

        let enabledFireSensorCount = fireSensorCount - disabledFireSensorCount;
        let enabledPSMSensorCount = psmSensorCount - disabledPSMSensorCount;
        let enabledEtcSensorCount = etcSensorCount - disabledEtcSensorCount;
        let enabledCCTVCount = cctvCount - disabledCCTVCount;

        let iotSensorCount = psmSensorCount + etcSensorCount;
        let enabledIotSensorCount = enabledPSMSensorCount + enabledEtcSensorCount;

        //fireSensorCount = fireSensorCount - disabledFireSensorCount;
        //psmSensorCount = psmSensorCount - disabledPSMSensorCount;
        //etcSensorCount = etcSensorCount - disabledEtcSensorCount;
        //cctvCount = cctvCount - disabledCCTVCount;

        //return [fireSensorCount, enabledFireSensorCount, iotSensorCount, enabledIotSensorCount, cctvCount, enabledCCTVCount];
        return [fireSensorCount, enabledFireSensorCount, psmSensorCount, enabledPSMSensorCount, etcSensorCount, enabledEtcSensorCount, cctvCount, enabledCCTVCount];
    }

    getModeTabUI = (siteID) => {
        const mode = this.props.mode;
        let modeTabUI = [];
        let mainClass = dashboard.synthesis;
        let subClass = dashboard.details;
        let mainSpan = [];
        let subSpan = [];

        if (mode === DashboardResource.mode.main) {
            mainClass = dashboard.synthesis;
            mainSpan.push(<span key={"mainSpan"} className={dashboard.underLine}></span>);
        } else if (mode === DashboardResource.mode.sub) {
            subClass = dashboard.details;
            subSpan.push(<span key={"subSpan"} className={dashboard.underLine}></span>);
        }

        if (siteID === ProjectResource.Site.GCC) {
            // 녹십자
            modeTabUI.push(
                <>
                    <div key={"mainDiv"} className={mainClass} onClick={() => this.props.changeMode(DashboardResource.mode.main)} >종합 현황{mainSpan}</div>
                </>
            );
        } else {
            // 솔브레인
            modeTabUI.push(
                <>
                    <div key={"mainDiv"} className={mainClass} onClick={() => this.props.changeMode(DashboardResource.mode.main)} >종합 현황{mainSpan}</div>
                    <div key={"subDiv"} className={subClass} onClick={() => this.props.changeMode(DashboardResource.mode.sub)} >상세 현황{subSpan}</div>
                </>
            );
        }

        return modeTabUI;
    }

    getDate = () => {
        let dt = new Date();
        const arrDayStr = InfoHeader.arrDayStr;

        const year = dt.getFullYear();

        let month = dt.getMonth() + 1;
        if (month < 10)
            month = "0" + month;

        let day = dt.getDate();
        if (day < 10)
            day = "0" + day;

        const date = year + "." + month + "." + day;

        const dayString = arrDayStr[dt.getDay()];

        let unit = "am";
        let hours = dt.getHours();
        if (hours < 10)
            hours = "0" + hours;
        else if (hours > 12) {
            unit = "pm";
            hours = hours - 12;

            if (hours < 10)
                hours = "0" + hours;
        }

        let minutes = dt.getMinutes();
        if (minutes < 10)
            minutes = "0" + minutes;

        const time = dayString + " " + hours + ":" + minutes + " " + unit;

        return [date, time];
    }


    setSelectDay(index) {
        this.props.changeDay(index);
    }

    initDate = () => {
        const [date, time] = this.getDate();
            
        this.state.date = date;
        this.state.time = time;
    }

    reloadDate = () => {
        const [date, time] = this.getDate();
        const currentTime = this.state.time;
        const currentDate = this.state.date;

        if (currentTime === null || currentTime === undefined || currentTime !== time) {

            if (currentDate !== date) {
                this.props.reloadDate();
                this.setState({ date: date, time: time });
            } else {
                this.setState({ time: time });
            }
            
        } else {
            return;
        }
    }

    getWatchDate = () => {
        let date = this.state.date;
        let time = this.state.time;

        if (date === null || date === undefined ||
            time === null || time === undefined)
            return ["-", "-"];

        return [date, time];
    }

    getSelectDayUI() {
        let ui = [];
        const selectDay = this.props.selectDay;

        for (let i = 0; i < selectDay.length; i++) {
            if (selectDay[i].checked === true) {
                ui.push(
                    <>
                        <input key={"hsmUsr0_input_" + i} type="checkbox" id={"hsmUsr0" + i} defaultChecked onChange={(e) => this.setSelectDay(i)} />
                        <label key={"hsmUsr0_label_" + i} htmlFor={"hsmUsr0" + i}>{selectDay[i].displayText}</label> &nbsp;
                    </>
                );
            } else {
                ui.push(
                    <>
                        <input key={"hsmUsr0_input_" + i} type="checkbox" id={"hsmUsr0" + i}  onChange={(e) => this.setSelectDay(i)} />
                        <label key={"hsmUsr0_label_" + i} htmlFor={"hsmUsr0" + i}>{selectDay[i].displayText}</label> &nbsp;
                    </>
                );
            }

            
        }

        return ui;
    }

    displaySiteUI = () => {
        const siteID = ProjectResource.SiteID;
        let displaySiteUI = [];

        const [fireSensor, enabledFireSensorCount, psmSensor, enabledPSMSensorCount, etcSensor, enabledEtcSensorCount, CCTV, enabledCCTVCount] = this.setSensorCount();
        const modeTabUI = this.getModeTabUI(siteID);

        const [date, time] = this.getWatchDate();
        const selectDayUI = this.getSelectDayUI();

        if (siteID === ProjectResource.Site.GCC) {
            // 녹십자
            displaySiteUI.push(
                <header className={dashboard.infoHeader}>
                    <div className={dashboard.infoHeaderWrap}>
                        {modeTabUI}
                        <span className={dashboard.clockIcon}></span><div className={dashboard.titleClock}>{date}<span>{time}</span></div>
                        <span className={dashboard.selectDay + " " + dashboard.dashCheck}>
                            {selectDayUI}
                        </span>
                        <div className={dashboard.infoHeaderTxtArea}>
                            <div className={dashboard.infoHeaderTxt}>화재<span className={dashboard.greenText}>{enabledFireSensorCount}</span> / {fireSensor}</div>
                            <div className={dashboard.infoHeaderTxt}>누출<span className={dashboard.greenText}>{enabledPSMSensorCount}</span> / {psmSensor}</div>
                            <div className={dashboard.infoHeaderTxt} style={{ border: 'none'}} >CCTV<span className={dashboard.greenText}>{enabledCCTVCount}</span> / {CCTV} </div>
                        </div>
                    </div>
                </header>
            );
        } else {
            // 솔브레인
            displaySiteUI.push(
                <header className={dashboard.infoHeader}>
                    <div className={dashboard.infoHeaderWrap}>
                        {modeTabUI}
                        <span className={dashboard.clockIcon}></span><div className={dashboard.titleClock}>{date}<span>{time}</span></div>
                        <span className={dashboard.selectDay + " " + dashboard.dashCheck}>
                            {selectDayUI}
                        </span>
                        <div className={dashboard.infoHeaderTxtArea}>
                            <div className={dashboard.infoHeaderTxt}>화재<span className={dashboard.greenText}>{enabledFireSensorCount}</span> / {fireSensor}</div>
                            <div className={dashboard.infoHeaderTxt}>누출<span className={dashboard.greenText}>{enabledPSMSensorCount}</span> / {psmSensor}</div>
                            <div className={dashboard.infoHeaderTxt}>ETC<span className={dashboard.greenText}>{enabledEtcSensorCount}</span> / {etcSensor}</div>
                            <div className={dashboard.infoHeaderTxt}>CCTV<span className={dashboard.greenText}>{enabledCCTVCount}</span> / {CCTV} </div>
                        </div>
                    </div>
                </header>
            );
        }

        return displaySiteUI;
    }

    render() {

        const displaySiteUI = this.displaySiteUI();

        return (
            <>
                {   /* Site별 UI */
                    displaySiteUI
                }
            </>
        );
    }
}
export default InfoHeader;