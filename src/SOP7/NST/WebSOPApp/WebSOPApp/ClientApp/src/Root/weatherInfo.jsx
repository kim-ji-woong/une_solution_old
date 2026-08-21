import React, { Component } from 'react';
import { BrowserRouter as Route, Link } from 'react-router-dom';
import { withRouter } from 'react-router-dom';
import $ from 'jquery';
import store from './store';
import SDMSResource from '../SDMS/resource/id';
import { SDMSController } from '../SDMS/services/sdmsController';

import title from './css/titleBar.module.css';

class WeatherInfo extends Component {
    constructor(props) {
        super(props);

        this.state = {
            selectedSiteIndex: 0,
            weatherInfo:
            {
                selectedIndex: 0,
                datas: store.getState().weatherDatas
            },
            weeklyDatas: null,
        }

        this.props = props;

        store.subscribe(function () {
            const data = store.getState();

            if (data === null || data === undefined || data.actionType !== 'WEATHER_CURRENT')
                return;

            this.changeWeather(data);
            this.reloadWeeklyData();
        }.bind(this));
    }

    componentDidMount() {

        $('.' + title.arrowBtn).click(function () {
            $('.' + title.tabWeather2).slideToggle();
            $('.' + title.arrowBtn).hide();
        });
        $('.' + title.arrowBtn2).click(function () {
            $('.' + title.tabWeather2).slideUp();
            $('.' + title.arrowBtn).show();
        });
    }

    changeWeather(data) {
        const weatherDatas = data.weatherDatas;

        if (weatherDatas) {
            const weatherInfo =
            {
                selectedIndex: this.state.weatherInfo.selectedIndex,
                datas: weatherDatas,
            }

            this.setState({ weatherInfo });
        }
    }

    async reloadWeeklyData() {
        const weeklyDatas = this.state.weeklyDatas;
        const [result, message] = await SDMSController.requestWeatherWeeklyInfo();

        if (result === null || result === undefined)
            return;

        if (weeklyDatas === null || weeklyDatas === undefined) {
            this.setState({ weeklyDatas: result });
            return;
        } else {
            if (weeklyDatas.length === 0) {
                this.setState({ weeklyDatas: result });
                return;
            } else if (result.length !== 1) {
                return;
            }

            for (let i = 0; i < result.length; i++) {
                const weeklyData = weeklyDatas[i];
                const data = result[i];

                if (weeklyData.weekly === null || weeklyData.weekly === undefined ||
                    data.weekly === null || data.weekly === undefined)
                    return;

                if (weeklyData.weekly.oneDayLaterTemp !== data.weekly.oneDayLaterTemp ||
                    weeklyData.weekly.oneDayLaterState !== data.weekly.oneDayLaterState ||
                    weeklyData.weekly.twoDayLaterTemp !== data.weekly.twoDayLaterTemp ||
                    weeklyData.weekly.twoDayLaterState !== data.weekly.twoDayLaterState ||
                    weeklyData.weekly.threeDayLaterTemp !== data.weekly.threeDayLaterTemp ||
                    weeklyData.weekly.threeDayLaterState !== data.weekly.threeDayLaterState ||
                    weeklyData.weekly.fourDayLaterTemp !== data.weekly.fourDayLaterTemp ||
                    weeklyData.weekly.fourDayLaterState !== data.weekly.fourDayLaterState ||
                    weeklyData.weekly.fiveDayLaterTemp !== data.weekly.fiveDayLaterTemp ||
                    weeklyData.weekly.fiveDayLaterState !== data.weekly.fiveDayLaterState || 
                    weeklyData.weekly.sixDayLaterTemp !== data.weekly.sixDayLaterTemp ||
                    weeklyData.weekly.sixDayLaterState !== data.weekly.sixDayLaterState) {
                    this.setState({ weeklyDatas: result });
                    return;
                }
            }
        }
    }

    getCurrentData() {
        const weatherDatas = this.state.weatherInfo.datas;
        //const dataCount = weatherDatas.length;
        const selectedIndex = this.state.selectedSiteIndex;

        if (weatherDatas === null || weatherDatas === undefined) {
            return ["", SDMSResource.getStateImage(0), '-', '-', '-', '-', '-'];
        }

        const data = weatherDatas[selectedIndex];

        if (!data?.current) {
            return ["", SDMSResource.getStateImage(0), '-', '-', '-', '-', '-'];
        }

        let updateTime = "";

        if (data.current.updateTime) {
            updateTime = data.current.updateTime.replace('T', ' ');

            // 현재 시각으로부터 30분 이전에 작성된 데이터는 무시한다.
            if (this.checkTime(updateTime, 30) === false) {
                return ["", SDMSResource.getStateImage(0), '-', '-', '-', '-', '-'];
            }
        }

        return [updateTime, SDMSResource.getStateImage(data.current.state), data.current.temperature, data.current.windSpeed, data.current.rain, data.current.humidity, SDMSResource.getStateString(data.current.state)];
    }

    // time이 현재시각으로부터 minutes 만큼 더 오래전인가?
    checkTime(strTime, minutes) {
        const now = new Date();
        const prevTime = new Date(now.getTime() - minutes * 60 * 1000);
        const strPrevTime = prevTime.getFullYear() + "-" + this.getDoubleString(prevTime.getMonth() + 1) + "-" + this.getDoubleString(prevTime.getDate()) + " " + this.getDoubleString(prevTime.getHours()) + ":" + this.getDoubleString(prevTime.getMinutes()) + ":" + this.getDoubleString(prevTime.getSeconds());

        if (strTime >= strPrevTime) {
            return true;
        }

        return false;
    }

    getDoubleString(num) {
        if (num < 10) {
            return "0" + num;
        }

        return num;
    }

    getWeatherWeeklyInfo = () => {
        let weeklyDatas = this.state.weeklyDatas;
        let selectedSiteIndex = this.state.selectedSiteIndex;

        // 요일 구하기
        let days = [];
        let dt = new Date();
        const arrDayStr = ['일', '월', '화', '수', '목', '금', '토'];

        days.push(arrDayStr[dt.getDay()]);
        dt.setDate(dt.getDate() + 1);
        days.push(arrDayStr[dt.getDay()]);
        dt.setDate(dt.getDate() + 1);
        days.push(arrDayStr[dt.getDay()]);
        dt.setDate(dt.getDate() + 1);
        days.push(arrDayStr[dt.getDay()]);
        dt.setDate(dt.getDate() + 1);
        days.push(arrDayStr[dt.getDay()]);
        dt.setDate(dt.getDate() + 1);
        days.push(arrDayStr[dt.getDay()]);
        dt.setDate(dt.getDate() + 1);
        days.push(arrDayStr[dt.getDay()]);

        // 기상 정보 구하기
        let weeklyInfo = [];
        if (weeklyDatas === null || weeklyDatas === undefined) {
            weeklyInfo = [
                { temp: "-", state: SDMSResource.getStateImage("-") },
                { temp: "-", state: SDMSResource.getStateImage("-") },
                { temp: "-", state: SDMSResource.getStateImage("-") },
                { temp: "-", state: SDMSResource.getStateImage("-") },
                { temp: "-", state: SDMSResource.getStateImage("-") },
                { temp: "-", state: SDMSResource.getStateImage("-") }
            ];
        } else {
            for (let i = 0; i < weeklyDatas.length; i++) {
                let weeklyData = weeklyDatas[i];

                //if (weeklyData.site.id === selectedSiteIndex) {
                    // 데이터 업데이트 시간 비교 >> 현재 시간보다 30분 전 데이터이라면 표시X
                    let today = new Date();
                    let date = new Date(weeklyData.weekly.updateTime);

                    let second = today.getTime() - date.getTime();
                    let minute = second / 1000 / 60;

                    if (minute > 30) {
                        weeklyInfo = [
                            { temp: "-", state: SDMSResource.getStateImage("-") },
                            { temp: "-", state: SDMSResource.getStateImage("-") },
                            { temp: "-", state: SDMSResource.getStateImage("-") },
                            { temp: "-", state: SDMSResource.getStateImage("-") },
                            { temp: "-", state: SDMSResource.getStateImage("-") },
                            { temp: "-", state: SDMSResource.getStateImage("-") }
                        ];
                    } else {
                        weeklyInfo.push({ temp: weeklyData.weekly.oneDayLaterTemp, state: SDMSResource.getStateImage(weeklyData.weekly.oneDayLaterState) });
                        weeklyInfo.push({ temp: weeklyData.weekly.twoDayLaterTemp, state: SDMSResource.getStateImage(weeklyData.weekly.twoDayLaterState) });
                        weeklyInfo.push({ temp: weeklyData.weekly.threeDayLaterTemp, state: SDMSResource.getStateImage(weeklyData.weekly.threeDayLaterState) });
                        weeklyInfo.push({ temp: weeklyData.weekly.fourDayLaterTemp, state: SDMSResource.getStateImage(weeklyData.weekly.fourDayLaterState) });
                        weeklyInfo.push({ temp: weeklyData.weekly.fiveDayLaterTemp, state: SDMSResource.getStateImage(weeklyData.weekly.fiveDayLaterState) });
                        weeklyInfo.push({ temp: weeklyData.weekly.sixDayLaterTemp, state: SDMSResource.getStateImage(weeklyData.weekly.sixDayLaterState) });
                    }

                    break;
                //}
            }
        }

        return [days, weeklyInfo];
    }

    render() {
        const [updateTime, imgIcon, temp, windSpeed, rain, humidity, state] = this.getCurrentData();
        const [days, weeklyInfo] = this.getWeatherWeeklyInfo();

        return (
            <>
                <figure className={title.tabWeather}>
                    <div className={title.tabWeatherContent}>
                        <img src={imgIcon} className={title.tabWeatherContentImg} />
                        {/*<div className={title.tabWeatherContentImg}></div>*/}
                        <span className={title.tabWeatherContentText}>{state}</span>
                    </div>
                    <div className={title.tabWeatherText}>
                        <div className={title.temperature}>기온: {temp}℃</div>
                        <div className={title.humidity}>습도: {humidity}%</div>
                        <div className={title.windSpeed}>풍속: {windSpeed}m/s</div>
                    </div>
                    <button type="button" className={title.arrowBtn}></button>
                </figure>
                <div className={title.tabWeather2 + " " + title.hide}>
                    <ul>
                        <li>
                            <p>{days[0]}</p>
                            <p><img src={imgIcon} className={title.weatherDaysMon} /></p>
                        </li>
                        <li>
                            <p>{days[1]}</p>
                            <p><img src={weeklyInfo[0].state} className={title.weatherDaysMon} /></p>
                        </li>
                        <li>
                            <p>{days[2]}</p>
                            <p><img src={weeklyInfo[1].state} className={title.weatherDaysMon} /></p>
                        </li>
                        <li>
                            <p>{days[3]}</p>
                            <p><img src={weeklyInfo[2].state} className={title.weatherDaysMon} /></p>
                        </li>
                        <li>
                            <p>{days[4]}</p>
                            <p><img src={weeklyInfo[3].state} className={title.weatherDaysMon} /></p>
                        </li>
                        <li>
                            <p>{days[5]}</p>
                            <p><img src={weeklyInfo[4].state} className={title.weatherDaysMon} /></p>
                        </li>
                        <li>
                            <p>{days[6]}</p>
                            <p><img src={weeklyInfo[5].state} className={title.weatherDaysMon} /></p>
                        </li>
                    </ul>
                    <button type="button" className={title.arrowBtn2}></button>
                </div>
            </>
        );
    }
} export default WeatherInfo;