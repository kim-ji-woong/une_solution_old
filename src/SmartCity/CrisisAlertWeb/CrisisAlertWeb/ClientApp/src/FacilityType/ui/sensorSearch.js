import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import $ from 'jquery';

import Menu from '../../Root/menu';
import { FacilityTypeController } from '../services/facilityTypeController';

import SessionString from '../../Common/js/sessionString';

class SensorSearch extends Component {
    static pathSensorSearch = '/sensorSearch';

    constructor(props) {
        super(props);

        this.refSearch = React.createRef();

        this.state = {
            sensorList: null,
            sensorNum: null,
            showList: null,
        }

        this.initLoad();
    }

    componentDidMount() {
        //console.log("componentDidMount");
    }

    componentDidUpdate() {
        //console.log('componentDidUpdate');

        $('.radioList').on('click', function () {
            $('.radioList').prop('checked', false);
            this.checked = true;
        });
    }

    // 재난 분류에 따른 센서 불러오기 및 표시하기
    initLoad = () => {
        let facilityType = parseInt(window.sessionStorage.getItem(SessionString.Key.facilityType));

        this.getFacilityTypeSensors(facilityType);
    }

    async getFacilityTypeSensors(type) {
        const result = await FacilityTypeController.getFacilityTypeSensors(type);
        
        if (result !== null && result.success !== false) {
            let sensorList = result.facilityTypeSensors;
            let sensorNum = sensorList.length;

            this.setState({ sensorList: sensorList, showList: sensorList, sensorNum: sensorNum });
        }
    }

    onClickCancle = () => {
        this.props.history.push(Menu.pathMain);
    }

    onClickConfirmm = () => {
        let facilityType = parseInt(window.sessionStorage.getItem(SessionString.Key.facilityType));
        let id = $('.radioList:checked').val();

        if (id === null || id === undefined) {
            alert("센서를 선택하세요.");
            return;
        }

        this.getSensorInfo(id, facilityType);
    }

    async getSensorInfo(id, type) {
        const result = await FacilityTypeController.DisplaySensorInfo(id, type);

        if (result.success === true && result.sensor !== null) {
            // 세션 스토리지에 선택된 센서 저장
            window.sessionStorage.setItem(SessionString.Key.selectSensor, JSON.stringify(result.sensor));
        }

        this.props.history.push(Menu.pathMain);
    }

    onClickSearch = () => {
        const search = this.refSearch.current.value.toString().trim();
        let sensorList = this.state.sensorList;
        let sensorNum = 0;
        let showList = [];

        if (search !== null && search !== "") {
            for (let i = 0; i < sensorList.length; i++) {
                let sensor = sensorList[i];
                let addr = sensor.addr;

                if (addr.indexOf(search) !== -1) {
                    showList.push(sensor);
                }
            }

            sensorNum = showList.length;
        } else {
            showList = sensorList;
            sensorNum = showList.length;
        }

        this.setState({ showList: showList, sensorNum: sensorNum });
    }

    showSensorList = () => {
        let selectSensor = JSON.parse(window.sessionStorage.getItem(SessionString.Key.selectSensor));

        let sensorList = this.state.showList;
        let showSensorList = [];

        if (sensorList === null)
            return "";

        for (let i = 0; i < sensorList.length; i++) {
            let sensor = sensorList[i];
            let checked = (selectSensor.id === sensor.id);

            //showSensorList.push(<label key={sensor.id} className='radioTemp'><input type='radio' className='radioList' value={sensor.id} {/*defaultChecked={checked}*/} /> {sensor.addr} ({sensor.sensorID}) </label>);
            showSensorList.push(<label key={sensor.id} className='radioTemp'><input type='radio' className='radioList' value={sensor.id} /> {sensor.addr} ({sensor.sensorID}) </label>);
        }

        

        return showSensorList;
    }

    render() {
        let sensorList = [];

        sensorList = this.showSensorList();


        return (
            <div className="area auto">
                <div className="container_sub4">
                    <div className="header_sub">
                        <span>
                            <p id="behav_title">센서 검색</p>
                        </span>
                        <span><img id="close" src="/resource/icon/close.png" onClick={this.onClickCancle}></img></span>
                    </div>
                    <div className="contents">
                        <div className="add_title">
                            <p>주소</p>
                            <input ref={this.refSearch} type="text" name="address" className="text_add" placeholder="주소를 입력하세요." />
                            <div className="add_btn" onClick={this.onClickSearch}>검색</div>
                        </div>
                        <div className="space">
                            <p>센서 검색 결과입니다.</p>
                            <p id="result">검색결과 : {this.state.sensorNum} 건</p>
 
                            <div className="space_1"> 
                                {sensorList}
                            </div>
                        </div>
                        <div className="confirmm" onClick={this.onClickConfirmm}>
                            <p>확인</p>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}

export default withRouter(SensorSearch);
