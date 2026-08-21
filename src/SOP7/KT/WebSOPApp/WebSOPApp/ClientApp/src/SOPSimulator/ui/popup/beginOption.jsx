import React, { Component } from 'react';
import $ from 'jquery';
import uis from '../../../Common/css/ui.module.css';
import uneStyles from '../../../Common/css/uneCommon.module.css';

class BeginOption extends Component {
    constructor(props) {
        super(props);
        this.state = {
            year: null,
            month: null,
            day: null,
            hour: null,
            min: null,
            sec: null
        }

        this.props = props;

    }

    componentDidMount() {
        //this.getDisasterCategories()
        $('html, body').css({ 'display': 'block', 'height': '100%', 'overflow': 'hidden' });
        $('.' + uis.sqpDown).css({ 'color': '#000000' });
         //각 페이지 별로 클래스 초기화
        $('#subPage').removeClass('sop');

        const now = new Date();

        const year = now.getFullYear();
        const month = now.getMonth();
        const day = now.getDate();
        const hour = now.getHours();
        const min = now.getMinutes();
        const sec = now.getSeconds();

        this.setState({ year: year, month: month, day: day, hour: hour, min: min, sec: sec });
    }

    onChangeTimeMode(isCurrent) {
        if (isCurrent) {

        }
        else {

        }
    }

    setDateTime() {           
        let yearTag = [];        
        for (var i = -1; i <= 1; i++) {

            if (this.state.year === this.state.year + i) {
                yearTag.push(<option key={'year_' + this.state.year + i} value={this.state.year + i} defaultValue>{this.state.year + i}</option>);
            }
            else {
                yearTag.push(<option key={'year_' + this.state.year + i} value={this.state.year + i}>{this.state.year + i}</option>);
            }
        }
        
        let monthTag = [];
        for (let i = 0; i <= 11; i++) {
            if (this.state.month === i) {
                monthTag.push(<option key={'month_' + i + 1} value={i} selected>{i + 1}</option>);
            }
            else {
                monthTag.push(<option key={'month_' + i + 1} value={i}>{i + 1}</option>);
            }
        }
                
        const lastDay = new Date(this.state.year, this.state.month, 0).getDate();
        let dayTag = [];
        for (let i = 1; i <= lastDay; i++) {
            if (this.state.day === i) {
                dayTag.push(<option key={'day_' + i} value={i} selected>{i}</option>);
            }
            else {
                dayTag.push(<option key={'day_' + i} value={i}>{i}</option>);
            }
        }

        let hourTag = [];
        for (let i = 0; i <= 23; i++) {
            if (this.state.hour === i) {
                hourTag.push(<option key={'hour_' + i} value={i} selected>{i}</option>);
            }
            else {
                hourTag.push(<option key={'hour_' + i} value={i}>{i}</option>);
            }
        }

        let minTag = [];
        for (let i = 0; i <= 59; i++) {
            if (this.state.min === i) {
                minTag.push(<option key={'min_' + i} value={i} selected>{i}</option>);
            }
            else {
                minTag.push(<option key={'min_' + i} value={i}>{i}</option>);
            }
        }

        return [yearTag, monthTag, dayTag, hourTag, minTag];
    }

    onChangeMonth(target) {
        console.log(target);
        this.setState({ month: Number(target.value) });
    }

    onClickBegin = () => {

        const position = document.getElementById('txtPosition').value;
        if (!position || position.length === 0) {
            alert('재난 발생 위치를 입력하세요');
        }
        else {
            const beginTime = new Date(this.state.year, this.state.month, this.state.day, this.state.hour, this.state.min, 0);
            this.props.beginSOP(beginTime, position);
        }
    }

    onClickClose = () => {
        this.props.changeContent('');
    }

    beginEnterKey = () => {
        if (window.event.keyCode === 13) {
            this.onClickBegin();
        }
    }

    render() {
        const [yearTag, monthTag, dayTag, hourTag, minTag] = this.setDateTime();
        return (
            <div id={uis.sopPop}>
                <div>
                    <div>
                        <div className={uis.sqPop}>
                            <div className={uis.sqpTop}>
                                <h4>시작 이벤트 옵션</h4>
                                <p>{this.props.title}</p>
                                <a onClick={() => this.onClickClose()}>닫기</a>
                            </div>
                            <div className={uis.sqpCont}>
                                <div className={uis.sqpUp}>
                                    {
                                        //<select name="" id="" className={uis.sqpSel}>
                                        //    <option value="">선택</option>
                                        //    <option value="">빌딩 1</option>
                                        //</select>
                                        <input type="text" id="txtPosition" className={uis.sqpSel} onKeyUp={this.beginEnterKey} />
                                    }
                                    {
                                        //<div className={uis.sqpChk}>
                                        //    <input type="checkbox" name="sqpChk" id="sqpChk" checked="" /><label for="sqpChk">상황 시작/종료 문자 메시지 사용</label>
                                        //</div>
                                    }
                                </div>
                                <div className={uis.sqpDown}>
                                    
                                        
                                    <ul className={uis.sqpRdo}>
                                        <li><input type="radio" name="sqpRdo" id="sqpRdo01" onChange={() => this.onChangeTimeMode(true)} checked={true} /><label htmlFor="sqpRdo01">현재 시간을 재난발생시간으로 설정</label></li>
                                        <li><input type="radio" name="sqpRdo" id="sqpRdo02" onChange={() => this.onChangeTimeMode(false)}/><label htmlFor="sqpRdo02">재난발생 시간 입력</label></li>
                                    </ul>
                                    <ul className={uis.sqpTime + " " + uneStyles.sqpTime}>
                                        <li>
                                            <select name="" id="">
                                                {yearTag}
                                            </select>
                                        </li>
                                        <li>년</li>
                                        <li>
                                            <select name="" id="" onChange={(e) => this.onChangeMonth(e.target)}>
                                                {monthTag}
                                            </select>
                                        </li>
                                        <li>월</li>
                                        <li>
                                            <select name="" id="">
                                                {dayTag}
                                            </select>
                                        </li>
                                        <li>일</li>
                                        <li>
                                            <select name="" id="">
                                                {hourTag}
                                            </select>
                                        </li>
                                        <li>:</li>
                                        <li>
                                            <select name="" id="">
                                                {minTag}
                                            </select>
                                        </li>
                                    </ul> 
                                    
                                    <ul className={uis.sqpBtn}>
                                        <li><a className={uis.bk} onClick={this.onClickBegin}>시작</a></li>
                                        <li><a className={uis.gry} onClick={this.onClickClose}>취소</a></li>
                                    </ul>
                                </div>
                            </div>{/*<!-- sqpCont -->*/}
                        </div>{/*<!-- sqPop -->*/}
                    </div>
                </div>
            </div>
          
        );
    }

}

export default BeginOption;