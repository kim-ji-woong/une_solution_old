import { ui } from 'jquery';
import React, { Component } from 'react';
import content from '../../../Common/css/content.module.css';
import dash from '../../../Common/css/dash.module.css';
import SDMS from '../sdms';
import SDMSResource from '../../resource/id';
import uneStyles from '../../../Common/css/uneCommon.module.css';
import imgCloudy from '../../../Common/img/weather/cloudy.png';
import imgCloudDay from '../../../Common/img/weather/cloud_day.png';
import imgCloudNight from '../../../Common/img/weather/cloud_night.png';
import imgHeavySnow from '../../../Common/img/weather/heavySnow.png';
import imgSnow from '../../../Common/img/weather/snow.png';
import imgSnowRain from '../../../Common/img/weather/snowRain.png';
import imgHeavyRain from '../../../Common/img/weather/heavyRain.png';
import imgRain from '../../../Common/img/weather/rain.png';
import imgSunnyDay from '../../../Common/img/weather/sunny_day.png';
import imgSunnyNight from '../../../Common/img/weather/sunny_night.png';
import imgThunder from '../../../Common/img/weather/thunder.png';
import imgDustStorm from '../../../Common/img/weather/dustStorm.png';
import SettingsStore from '../../../Settings/settingsStore';
import store from '../../../Root/store';

class WeatherInfo extends Component {
    static Unknown = 0;
    static Sunshine = 1;
    static Thunder = 2;
    static SnowRain = 3;
    static HeavySnow = 4;
    static Snow = 5;
    static HeavyRain = 6;
    static Rain = 7;
    static Cloudy = 8;
    static Cloud = 9;
    static DustStorm = 10;
    static FineDust = 11;

    constructor(props) {
        super(props);

        this.state = {
            popupMinWidth: 320, // 팝업 최소 너비
            popupMinHeight: 154, // 팝업 최소  높이
            //imgIndex: 0,
            //image: imgCloudy,
            //selectedSiteIndex: this.props.info.selectedIndex,
            selectedSiteIndex: 0,
            weatherInfo:
            {
                selectedIndex: 0,
                datas: store.getState().weatherDatas
            },
        }

        this.props = props;

        this.initPopupState = this.initPopupState.bind(this);

        store.subscribe(function () {
            const data = store.getState();

            if (data === null || data === undefined || data.actionType !== 'WEATHER_CURRENT')
                return;

            this.changeWeather(data);
        }.bind(this));

        SettingsStore.subscribe(function () {
            this.resetPopupState(SettingsStore.getState());
        }.bind(this));
    }

    componentDidMount() {
        // 팝업 마우스 드래그 이벤트 리스너
        this.popupDragMouseMove = (event) => {
            var mousePosition = {
                x: event.clientX,
                y: event.clientY
            }

            //움직여야할 좌표
            let moveX = mousePosition.x + this.state.dragOffsetX;
            let perMoveX = ((moveX / this.state.maxScreenWidth) * 100);

            let moveY = mousePosition.y + this.state.dragOffsetY;
            let perMoveY = ((moveY / this.state.maxScreenHeight) * 100);

            // 팝업 너비
            let width = this.state.popup.clientWidth;
            let left = this.state.popup.offsetLeft;

            // 팝업 높이
            let height = this.state.popup.clientHeight;
            let top = this.state.popup.offsetTop;

            let popupRightPos = width + left;   // 현재 위치에서 오른쪽 끝 절대 좌표
            let popupBottomPos = height + top;  // 현재 위치에서 아래쪽 끝 절대 좌표

            // 팝업이 화면밖으로 안나가도록 처리
            if (moveX > 0 && moveX + width < this.state.maxScreenWidth) {
                this.state.popup.style.left = perMoveX + '%';
            } else if (moveX + width > this.state.maxScreenWidth) {
                // 드래그 도중 이동할 마우스 포지션 지점부터 팝업 끝지점이 우측 화면 밖을 벗어나게 될 때
                if (popupRightPos < this.state.maxScreenWidth) {
                    // 팝업을 우측 변에 고정
                    let lim = ((this.state.maxScreenWidth - width) / this.state.maxScreenWidth) * 100;
                    this.state.popup.style.left = lim + '%';
                } else if (this.state.preMousePosition.x > mousePosition.x) {
                    // 화면 오른쪽으로 팝업이 이미 벗어나 있을 때
                    this.state.popup.style.left = perMoveX + '%';
                }
            } else if (moveX <= 0) {
                // 드래그 도중 팝업 시작점이 좌측 화면 밖을 벗어나게 될 때
                if (left > 0) {
                    this.state.popup.style.left = '0%';
                } else if (this.state.preMousePosition.x < mousePosition.x) {
                    // 화면 왼쪽으로 팝업이 이미 벗어나 있을 때
                    this.state.popup.style.left = perMoveX + '%';
                }
            }

            if (moveY > 60 && moveY + height < this.state.maxScreenHeight) {
                this.state.popup.style.top = perMoveY + '%';
            } else if (moveY + height > this.state.maxScreenHeight) {
                // 드래그 도중 이동할 마우스 포지션 지점부터 팝업 하단 끝지점이 화면 밖을 벗어나게 될 때
                if (popupBottomPos < this.state.maxScreenHeight) {
                    // 팝업을 아랫 변에 고정
                    let lim = ((this.state.maxScreenHeight - height) / this.state.maxScreenHeight) * 100;
                    this.state.popup.style.top = lim + '%';
                } else if (this.state.preMousePosition.y > mousePosition.y) {
                    // 화면 아래쪽으로 팝업이 이미 벗어나 있을 때
                    this.state.popup.style.top = perMoveY + '%';
                }
            } else if (moveY <= 60) {
                // 드래그 도중 상단 끝지점이 화면 밖을 벗어나게 될 때
                if (top > 60) {
                    // 팝업을 윗 변에 고정
                    //상단 툴바는 항상 높이 60 고정이기 때문에 현재 화면 사이즈에서 60px의 비율을 계산한다.
                    let lim = (60 / this.state.maxScreenHeight) * 100;
                    this.state.popup.style.top = lim + '%';
                } else if (this.state.preMousePosition.y < mousePosition.y) {
                    //화면 위쪽으로 팝업이 이미 벗어나 있을 때
                    this.state.popup.style.top = perMoveY + '%';
                }
            }
        }

        //팝업 리사이즈 이벤트 리스너
        this.popupResizeMouseMove = (event) => {
            let sizeX = 0;
            let sizeY = 0;

            switch (this.state.resizeType) {
                // 수평
                case 'h-r': // 오른쪽 수평
                    sizeX = event.pageX - this.state.popup.getBoundingClientRect().left;

                    if (event.pageX > 0 && event.pageX < this.state.maxScreenWidth && sizeX > this.state.popupMinWidth) {
                        this.state.popup.style.width = sizeX + 'px';
                    }
                    break;
                case 'h-l': //왼쪽 수평
                    sizeX = this.state.originalWidth - (event.pageX - this.state.originalMouseX);

                    if (event.pageX > 0 && event.pageX < this.state.maxScreenWidth && sizeX > this.state.popupMinWidth) {
                        this.state.popup.style.width = sizeX + 'px';

                        let pxLeft = (this.state.originalX + (event.pageX - this.state.originalMouseX));
                        this.state.popup.style.left = ((pxLeft / this.state.maxScreenWidth) * 100) + '%';
                    }
                    break;
                // 수직
                case 'v-b': // 바텀 수직
                    sizeY = event.pageY - this.state.popup.getBoundingClientRect().top;

                    if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && sizeY > this.state.popupMinHeight) {
                        this.state.popup.style.height = sizeY + 'px';
                    }
                    break;
                case 'v-t': //탑 수직
                    sizeY = this.state.originalHeight - (event.pageY - this.state.originalMouseY);

                    if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && sizeY > this.state.popupMinHeight) {
                        this.state.popup.style.height = sizeY + 'px'

                        let pxTop = this.state.originalY + (event.pageY - this.state.originalMouseY);
                        this.state.popup.style.top = ((pxTop / this.state.maxScreenHeight) * 100) + '%';
                    }
                    break;
                // 대각
                case 'd-rb': // 오른쪽 하단 대각
                    sizeX = event.pageX - this.state.popup.getBoundingClientRect().left;
                    sizeY = event.pageY - this.state.popup.getBoundingClientRect().top;

                    if (event.pageX > 0 && event.pageX < this.state.maxScreenWidth && sizeX > this.state.popupMinWidth) {
                        this.state.popup.style.width = sizeX + 'px';
                    }

                    if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && sizeY > this.state.popupMinHeight) {
                        this.state.popup.style.height = sizeY + 'px';
                    }
                    break;
                case 'd-rt': //오른쪽 상단 대각
                    sizeX = this.state.originalWidth + (event.pageX - this.state.originalMouseX);
                    sizeY = this.state.originalHeight - (event.pageY - this.state.originalMouseY);

                    if (event.pageX > 0 && event.pageX < this.state.maxScreenWidth && sizeX > this.state.popupMinWidth) {
                        this.state.popup.style.width = sizeX + 'px';
                    }

                    if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && sizeY > this.state.popupMinHeight) {
                        this.state.popup.style.height = sizeY + 'px';

                        let pxTop = this.state.originalY + (event.pageY - this.state.originalMouseY);
                        this.state.popup.style.top = ((pxTop / this.state.maxScreenHeight) * 100) + '%';
                    }
                    break;

                case 'd-lb': //왼쪽 하단 대각
                    sizeY = event.pageY - this.state.popup.getBoundingClientRect().top;
                    sizeX = this.state.originalWidth - (event.pageX - this.state.originalMouseX);

                    if (event.pageX > 0 && event.pageX < this.state.maxScreenWidth && sizeX > this.state.popupMinWidth) {
                        this.state.popup.style.width = sizeX + 'px';

                        let pxLeft = (this.state.originalX + (event.pageX - this.state.originalMouseX));
                        this.state.popup.style.left = ((pxLeft / this.state.maxScreenWidth) * 100) + '%';
                    }

                    if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && sizeY > this.state.popupMinHeight) {
                        this.state.popup.style.height = sizeY + 'px';
                    }
                    break;

                case 'd-lt': //왼쪽 상단 대각
                    sizeX = this.state.originalWidth - (event.pageX - this.state.originalMouseX);
                    sizeY = this.state.originalHeight - (event.pageY - this.state.originalMouseY);

                    if (event.pageX > 0 && event.pageX < this.state.maxScreenWidth && sizeX > this.state.popupMinWidth) {
                        this.state.popup.style.width = sizeX + 'px';

                        let pxLeft = (this.state.originalX + (event.pageX - this.state.originalMouseX));
                        this.state.popup.style.left = ((pxLeft / this.state.maxScreenWidth) * 100) + '%';
                    }
                    if (event.pageY > 60 && event.pageY < this.state.maxScreenHeight && sizeY > this.state.popupMinHeight) {
                        this.state.popup.style.height = sizeY + 'px';

                        let pxTop = this.state.originalY + (event.pageY - this.state.originalMouseY);
                        this.state.popup.style.top = ((pxTop / this.state.maxScreenHeight) * 100) + '%';
                    }
                    break;
                default:
            }

        }
        this.initPopupState();

        this.props.setActiveDragPopup(this.props.popupType);
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

    initPopupState() {
        var popup = document.getElementsByClassName(content.viewDashboardBoxD + ' ' + content.viewDashboardWeather)[0];

        //DB에 값이 있을 경우에만
        if (typeof this.props.popupState !== 'undefined') {
            popup.style.left = this.props.popupState.x;
            popup.style.top = this.props.popupState.y;
            popup.style.width = this.props.popupState.width;
            popup.style.height = this.props.popupState.height;
        }

        this.setState({ popup: popup });
    }

    repositionPopup(popupState) {
        let data = popupState.weatherInfo;

        if (data === null || data === undefined)
            return;

        let popup = document.getElementsByClassName(content.viewDashboardBoxD + ' ' + content.viewDashboardWeather)[0];
        if (popup === null || popup === undefined)
            return;

        popup.style.left = data.x;
        popup.style.top = data.y;
        popup.style.width = data.width;
        popup.style.height = data.height;

        this.setState({ popup: popup });
    }

    resetPopupState = (popupState) => {
        let data = popupState;

        if (data.actionType === 'RESET_POPUP') {
            this.repositionPopup(data.popupState);
        }
    }

    // 팝업 드래그 시작(팝업을 누르고 있을 때)
    popupDragMousePress(event) {
        if (event.button == 0) {
            //마우스 조작중에 브라우저의 크기를 조절할 수 없으므로
            // 이 시점에 도큐먼트 전체 크기를 호출한다.
            this.setState({
                maxScreenHeight: document.getElementsByTagName('body')[0].clientHeight,
                maxScreenWidth: document.getElementsByTagName('body')[0].clientWidth,
                dragOffsetX: this.state.popup.offsetLeft - event.clientX,
                dragOffsetY: this.state.popup.offsetTop - event.clientY,
                preMousePosition: {
                    x: event.clientX,
                    y: event.clientY
                }
            });

            document.addEventListener('mousemove', this.popupDragMouseMove);
            document.addEventListener('mouseup', this.popupDragMouseUp);

            // z-index 조정, 1 이 다른 팝업보다 앞에 배치됨
            this.props.setActiveDragPopup(this.props.popupType);
        }
    }
    // 팝업 드래그 종료(mouse up)
    popupDragMouseUp = () => {
        console.log('popup drag false')
        document.removeEventListener('mousemove', this.popupDragMouseMove);
        document.removeEventListener('mouseup', this.popupDragMouseUp);
        // 팝업 정보 DB 작성
        this.setPopupState();
    }

    componentDidUpdate(prevProps, prevState) {
        // 팝업이 선택 됐을 때(Drag 될때) 맨 앞에 팝업 위치
        if (this.props.zIndex !== prevProps.zIndex) {
            var popup = document.getElementsByClassName(content.viewDashboardBoxD + ' ' + content.viewDashboardWeather)[0];
            popup.style.zIndex = this.props.zIndex;
            console.log('weatherInfoZIndex changed', popup.style.zIndex);
        }
    }

    // 팝업 리사이징(누르고 있을 때)
    popupResizeMousePress(event, resizeType) {
        /* resizeType
         * h-r      오른쪽 수평
         * h-l      왼쪽 수평
         * v-b      바텀 수직
         * v-t      탑 수직
         * d-rt     우측 상단 대각
         * d-rb     우측 하단 대각
         * d-lt     좌축 상단 대각
         * d-lb     좌측 하단 대각
        */

        console.log('popupResizeMousePress');
        this.setState({
            maxScreenHeight: document.getElementsByTagName('body')[0].clientHeight,
            maxScreenWidth: document.getElementsByTagName('body')[0].clientWidth,
            resizeType: resizeType,
            originalMouseX: event.pageX,
            originalMouseY: event.pageY,
            originalWidth: parseFloat(getComputedStyle(this.state.popup, null).getPropertyValue('width').replace('px', '')),
            originalHeight: parseFloat(getComputedStyle(this.state.popup, null).getPropertyValue('height').replace('px', '')),
            originalX: this.state.popup.getBoundingClientRect().left,
            originalY: this.state.popup.getBoundingClientRect().top
        });

        document.addEventListener('mousemove', this.popupResizeMouseMove);

        document.addEventListener('mouseup', this.popupResizeMouseUp);
        // z-index 조정, 1 이 다른 팝업보다 앞에 배치됨
        this.props.setActiveDragPopup(this.props.popupType);

    }

    popupResizeMouseUp = () => {
        console.log('popup resize false');
        document.removeEventListener('mousemove', this.popupResizeMouseMove);
        document.removeEventListener('mouseup', this.popupResizeMouseUp);
        this.setState({ resizeType: null });
        this.setPopupState();
    }

    setPopupState() {
        // 팝업 정보 DB 작성
        let perX = ((this.state.popup.offsetLeft / this.state.maxScreenWidth) * 100);
        let perY = ((this.state.popup.offsetTop) / this.state.maxScreenHeight * 100);
        let width = this.state.popup.offsetWidth;
        let height = this.state.popup.offsetHeight;

        //팝업 비활성화 될 때 컴포넌트가 사라져 계산식이 0으로 되는 현상이 발생함. 이때 DB 등록되는것을 방지
        if (perX > 0 && perY > 0 && width > 0 && height > 0) {
            let popupState = {
                id: typeof this.props.popupState !== 'undefined' ? this.props.popupState.id : -1,
                x: perX + '%',
                y: perY + '%',
                height: height + 'px',
                width: width + 'px'
            }
            this.props.setPopupState(this.props.popupType, popupState);
        }
    }

    /*onClickWeatherImage = (event) => {
        let index = this.state.imgIndex + 1;

        if (index > 8) {
            index = 0;
        }

        let image = "";

        if (index === 0) {
            image = imgCloudy;
        }
        else if (index === 1) {
            image = imgCloud;
        }
        else if (index === 2) {
            image = imgHeavySnow;
        }
        else if (index === 3) {
            image = imgHeavySnow2;
        }
        else if (index === 4) {
            image = imgSnow;
        }
        else if (index === 5) {
            image = imgSnowRain;
        }
        else if (index === 6) {
            image = imgSunshine;
        }
        else if (index === 7) {
            image = imgThunder;
        }
        else if (index === 8) {
            image = imgTyphoon;
        }

        this.setState({ imgIndex: index, image });
    }*/

    onSelectSite = (event) => {
        const index = parseInt(event.target.value);

        if (index !== null && index !== undefined) {
            //this.props.info.selectedIndex = index;
            let weatherInfo = this.state.weatherInfo;
            weatherInfo.selectedIndex = index;

            this.setState({ selectedSiteIndex: index, weatherInfo: weatherInfo });
        }
    }

    getSiteOption(data, index, isSelected) {
        return <button key={"weatherSite_" + index} className={isSelected ? dash.weatherSiteButton + " " + dash.active : dash.weatherSiteButton} value={index} onClick={this.onSelectSite}>
            {data.site?.name === '보정' ? '용인' : data.site?.name}
        </button>
    }

    getSpecialReportURL(weatherDatas, dataCount, selectedIndex) {
        let isActive = dash.down;
        let url = null;

        if (dataCount > 0 && dataCount >= selectedIndex + 1) {
            //url = weatherDatas[0]?.specialReport?.url;
            url = weatherDatas[selectedIndex]?.specialReport?.imageUrl;

            if (url !== null) {
                isActive = dash.active + " " + dash.down;
            }
        }

        return [isActive, url];
    }

    getSelectElement(updateTime) {
        //if (updateTime.length > 0 && this.props.info) {//weatherInfo
        if (updateTime.length > 0 && this.state.weatherInfo) {
            //const weatherDatas = [...this.props.info.datas];
            const weatherDatas = this.state.weatherInfo.datas;
            const dataCount = weatherDatas.length;
            const selectedIndex = this.state.selectedSiteIndex;
            const [isActive, specialReportURL] = this.getSpecialReportURL(weatherDatas, dataCount, selectedIndex);

            if (selectedIndex < dataCount) {
                return (
                    <>
                        {
                            weatherDatas.map((data, index) => this.getSiteOption(data, index, index === selectedIndex))
                        }
                        <span className={dash.weatherButton + " " + isActive} onClick={() => WeatherInfo.onClickSpecialReport(specialReportURL)}>기상특보</span>
                    </>
                );
            }
        }

        return <></>
    }

    static onClickSpecialReport = (url) => {
        if (url === null || url === undefined)
            return;

        let specialReportURL = window.location.origin + "/specialReport?path=" + url;

        var child = window.open(specialReportURL, "기상특보", "width=800, height=700, location=no, toolbar=no, menubar=no, scrollbars=no, status=no");
        child.scrollTo(0, 40);
    }

    // ComboBox 사용하는 버전
    //getSiteOption(data, index/*, isSelected*/) {
    //    return <option key={"weatherSite_" + index} value={index}>{data.site?.name}</option>
    //}
    
    // ComboBox 사용하는 버전
    //getSelectElement(updateTime) {
    //    if (updateTime.length > 0 && this.props.info) {
    //        const weatherDatas = [...this.props.info.datas];
    //        const dataCount = weatherDatas.length;
    //        const selectedIndex = this.state.selectedSiteIndex;

    //        if (selectedIndex < dataCount) {
    //            return (
    //                <select className={content.dslSel} defaultValue={selectedIndex} onChange={this.onSelectSite}>
    //                {
    //                    weatherDatas.map((data, index) => this.getSiteOption(data, index/*, index === selectedIndex*/))
    //                }
    //                </select>
    //            );
    //        }
    //    }

    //    return <></>
    //}

    getStateImage(state) {
        if (state === WeatherInfo.Sunshine) {
            if (this.isDayLight()) {
                return imgSunnyDay;
            }
            else {
                return imgSunnyNight;
            }
            //return imgSunshine;
        }
        else if (state === WeatherInfo.Thunder) {
            return imgThunder;
        }
        else if (state === WeatherInfo.SnowRain) {
            return imgSnowRain;
        }
        else if (state === WeatherInfo.HeavySnow) {
            return imgHeavySnow;
        }
        else if (state === WeatherInfo.Snow) {
            return imgSnow;
        }
        else if (state === WeatherInfo.HeavyRain) {
            return imgHeavyRain;
        }
        else if (state === WeatherInfo.Rain) {
            return imgRain;
        }
        else if (state === WeatherInfo.Cloudy) {
            return imgCloudy;
        }
        else if (state === WeatherInfo.DustStorm) {
            return imgDustStorm;
        }

        if (this.isDayLight()) {
            return imgCloudDay;
        }

        return imgCloudNight;
    }

    isDayLight() {
        const now = new Date();
        const hour = now.getHours();

        if (hour < 6 || hour >= 19) {
            return false;
        }

        return true;
    }

    getDoubleString(num) {
        if (num < 10) {
            return "0" + num;
        }

        return num;
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

    getCurrentData() {
        //const weatherDatas = [...this.props.info.datas];
        const weatherDatas = this.state.weatherInfo.datas;
        const dataCount = weatherDatas.length;
        const selectedIndex = this.state.selectedSiteIndex;

        if (selectedIndex >= dataCount) {
            return ["", SDMSResource.getStateImage(0), '-', '-', '-', '-'];
        }

        const data = weatherDatas[selectedIndex];

        if (!data.current) {
            return ["", SDMSResource.getStateImage(0), '-', '-', '-', '-'];
        }

        let updateTime = "";

        if (data.current.updateTime) {
            updateTime = data.current.updateTime.replace('T', ' ');

            // 현재 시각으로부터 30분 이전에 작성된 데이터는 무시한다.
            if (this.checkTime(updateTime, 30) === false) {
                return ["", SDMSResource.getStateImage(0), '-', '-', '-', '-'];
            }
        }

        return [updateTime, SDMSResource.getStateImage(data.current.state), data.current.temperature, data.current.windSpeed, data.current.rain, data.current.humidity];
    }

    render() {
        const [updateTime, imgIcon, temp, windSpeed, rain, humidity] = this.getCurrentData();

        return (
            <div id={this.props.popupType} className={content.viewDashboardBoxD + ' ' + content.viewDashboardWeather}>
                <div className={content.popupSizingAreaRight} onMouseDown={(e) => this.popupResizeMousePress(e, 'h-r')} ></div>
                <div className={content.popupSizingAreaLeft} onMouseDown={(e) => this.popupResizeMousePress(e, 'h-l')}></div>
                <div className={content.popupSizingAreaBottom} onMouseDown={(e) => this.popupResizeMousePress(e, 'v-b')}></div>
                <div className={content.popupSizingAreaTop} onMouseDown={(e) => this.popupResizeMousePress(e, 'v-t')}></div>
                <div className={content.popupSizingAreaRightTopPoint} onMouseDown={(e) => this.popupResizeMousePress(e, 'd-rt')}></div>
                <div className={content.popupSizingAreaRightBottomPoint} onMouseDown={(e) => this.popupResizeMousePress(e, 'd-rb')}></div>
                <div className={content.popupSizingAreaLeftTopPoint} onMouseDown={(e) => this.popupResizeMousePress(e, 'd-lt')}></div>
                <div className={content.popupSizingAreaLeftBottomPoint} onMouseDown={(e) => this.popupResizeMousePress(e, 'd-lb')}></div>

                <div className={content.dslTop + " " + content.dslGrd}>
                    <h5 className={content.dslTitle} onMouseDown={(e) => this.popupDragMousePress(e)}>
                        기상정보
                        <span onMouseDown={(e) => this.popupDragMousePress(e)}>{updateTime}</span>
                    </h5>
                    <a className={content.dslX} onClick={() => this.props.setVisiblePopups(SDMS.menu.weatherInfo, false)}></a>
                </div>
                <div className={content.dslCont}>
                    <div>
                        {
                            this.getSelectElement(updateTime)
                        }
                        <dl className={content.dslInfo}>
                            <dt>
                                <img className={content.weatherImage} src={imgIcon} />
                                <h5><span>{temp}</span><em>℃</em></h5>
					        </dt>
                            <dd>
                                <p>바람  :  {windSpeed} m/s</p>
                                <p>강수량  : {rain} mm</p>
                                <p>습도  : {humidity} %</p>
                            </dd>
				        </dl>
			        </div>
                </div>
            </div>
        );
    }
}

export default WeatherInfo;