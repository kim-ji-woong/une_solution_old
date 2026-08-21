import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import $ from 'jquery';

import Title from '../../Root/title';
import { FacilityTypeController } from '../services/facilityTypeController';
import Paginate from '../../Root/paginate';

import SessionString from '../../Common/js/sessionString';
import FacilityTypeResource from '../resource/id';

import styles from '../../Common/css/style.css';
import { extend } from 'jquery';

class Manual extends Component {
    constructor(props) {
        super(props);

        this.state = {
            menu: FacilityTypeResource.ID.manualMenu.List,
            showManual: null,
        }

        this.props = props;
    }
    
    onChangeMenu = (manual) => {
        if (manual === null || manual === undefined)
            return;

        this.setState({ menu: FacilityTypeResource.ID.manualMenu.Content, showManual: manual });
    }

    onConfirm = () => {
        this.setState({ menu: FacilityTypeResource.ID.manualMenu.List });
    }

    showPage = () => {
        if (this.state.menu === FacilityTypeResource.ID.manualMenu.List) {
            return <ManualList onChangeMenu={this.onChangeMenu} />;
        } else if (this.state.menu === FacilityTypeResource.ID.manualMenu.Content) {
            return <ManualContent showManual={this.state.showManual} onConfirm={this.onConfirm} />;
        }
    }

    render() {
        let showPage = "";
        showPage = this.showPage();

        return (
            <>


                {showPage}



            </>
        );
    }
}

export default Manual;

class ManualList extends Component {
    constructor(props) {
        super(props);

        this.state = {
            manualList: null,
            allRequest: null,                   // 전체 요청 갯수  
            page: 1,                            // 현재 페이지
            ongPage: 10,                        // 한 페이지에 보여줄 요청의 수.
            riskLevel: FacilityTypeResource.ID.riskLevel.Attention,
            showList: null,
        }

        this.props = props;
        this.initLoad();
    }

    componentDidMount() {

    }

    initLoad = () => {
        let facilityType = parseInt(window.sessionStorage.getItem(SessionString.Key.facilityType));

        this.getManualList(facilityType);
    }

    async getManualList(type) {
        const result = await FacilityTypeController.getManualList(type);

        if (result.success === true && result.manuals !== null) {
            let manuals = result.manuals;
            let allRequest = 0;

            if (manuals !== null && manuals.length > 0)
                allRequest = manuals.length;

            manuals = JSON.stringify(manuals);

            //this.setState({ manualList: manuals, allRequest: allRequest });
            this.state.manualList = manuals;
            this.setManual(this.state.riskLevel);
        }
    }

    pageChange = (pageNum) => {
        this.setState({ page: pageNum });
        return;
    }

    onClickManual = (manual) => {
        this.props.onChangeMenu(manual);
    }

    // 행동 메뉴얼 단계 변경
    onClickTab = (level, e) => {
        let target = e.target;

        // UI 변경
        $('ul.tabs li').removeClass('current');
        $(target).addClass('current');

        this.setManual(level);
    }

    setManual(level) {
        if (this.state.manualList === null)
            return;

        let manualList = JSON.parse(this.state.manualList);
        if (manualList.length === 0)
            return;

        let showList = [];

        for (let i = 0; i < manualList.length; i++) {
            let manual = manualList[i];
            let manualLevel = manual.manualType;

            if (manualLevel === level)
                showList.push(manual);
        }

        let allRequest = 0;

        if (showList.length !== 0) {
            allRequest = showList.length;
            showList = JSON.stringify(showList);
        }
            

        this.setState({ showList: showList, allRequest: allRequest, page: 1, riskLevel: level });
    }

    showAlarmList() {
        let showList = [];
        let manualList = this.state.showList;

        if (manualList === null || manualList === undefined || manualList.length === 0)
            return showList;

        let list = JSON.parse(manualList);

        let min = (this.state.page - 1) * this.state.ongPage;
        let max = min + this.state.ongPage;
        if (max > this.state.allRequest) {
            max = this.state.allRequest;
        }

        for (let i = min; i < max; i++) {
            let manual = list[i];
            let title = manual.manualTitle;
            let level = manual.manualType;
            let content = manual.manualContent;
            let members = manual.members;

            let showMembers = "";
            for (let j = 0; j < members.length; j++) {
                let member = members[j];

                if (showMembers == "")
                    showMembers += member.memberName;
                else 
                    showMembers += ", " + member.memberName;
            }

            showList.push(
                <tr onClick={() => this.onClickManual(manual)}>
                    <td>{i + 1}</td>
                    <td>{title}</td>
                    <td>{showMembers}</td>
                    <td>{content}</td>
                </tr>
            );
        }

        return showList;
    }

    render() {
        let showList = [];

        showList = this.showAlarmList();


        return (
            <div className="container_sub2">

                <Title />



                <div className="contents">
                    <h3>행동 메뉴얼</h3>
                    <div className="content_box">

                        <ul className="tabs">
                            <li className="tab-link current" onClick={(e) => this.onClickTab(FacilityTypeResource.ID.riskLevel.Attention, e)} data-tab="tab-1">관심</li>
                            <li className="tab-link" onClick={(e) => this.onClickTab(FacilityTypeResource.ID.riskLevel.Caution, e)} data-tab="tab-2">경계</li>
                            <li className="tab-link" onClick={(e) => this.onClickTab(FacilityTypeResource.ID.riskLevel.Alert, e)} data-tab="tab-3">주의</li>
                            <li className="tab-link" onClick={(e) => this.onClickTab(FacilityTypeResource.ID.riskLevel.Serious, e)} data-tab="tab-3">심각</li>
                        </ul>

                        <div id="tab-1" className="tab-content current">

                            <table id="behav_tb" frame="void">
                                <colgroup>
                                    <col width="1%" />
                                    <col width="12%" />
                                    <col width="8%" />
                                    <col width="25%" />
                                </colgroup>
                                <thead>
                                    <tr>
                                        <th>NO</th>
                                        <th>제목</th>
                                        <th>수신자</th>
                                        <th>내용</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {showList}
                                </tbody>
                            </table>

                        </div>
                    </div>
                </div>

                <Paginate page={this.state.page} allRequest={this.state.allRequest} onChange={this.pageChange} />
                

            </div >
        );
    }
}

class ManualContent extends Component {
    constructor(props) {
        super(props);

        this.state = {

        }

        this.props = props;

    }

    onClickConfirm = () => {
        this.props.onConfirm();
    }

    render() {
        let showType = "";
        let facilityType = parseInt(window.sessionStorage.getItem(SessionString.Key.facilityType));
        if (facilityType === FacilityTypeResource.ID.facilityType.fire) {
            showType = "화재";
        } else if (facilityType === FacilityTypeResource.ID.facilityType.flood) {
            showType = "홍수";
        } else if (facilityType === FacilityTypeResource.ID.facilityType.heat) {
            showType = "폭염";
        } else if (facilityType === FacilityTypeResource.ID.facilityType.collapse) {
            showType = "경사지 붕괴";
        }

        let title = "";
        let showMembers = [];
        let contents = "";


        if (this.props.showManual !== null && this.props.showManual !== undefined) {
            let manual = this.props.showManual;

            title = manual.manualTitle;
            contents = manual.manualContent;

            let members = manual.members;

            for (let j = 0; j < members.length; j++) {
                let member = members[j];

                if (showMembers.length === 0) {
                    showMembers.push(
                        <>
                            <p style={{ textIndent: "15px", lineHeight: "20px" }}> -{member.regularTeam.teamName} {member.memberName}</p>
                        </>
                    );
                } else {
                    showMembers.push(
                        <>
                            <br />
                            <p style={{ textIndent: "15px", lineHeight: "20px" }}> -{member.regularTeam.teamName} {member.memberName}</p>
                        </>
                    );
                }
            }
        }

        return (
            <div class="container_sub3">
                <div class="header_sub">
                    <span><p id="behav_title">행동 메뉴얼</p></span>
                    <span onClick={this.onClickConfirm}><img src="/resource/icon/iconfinder_x_6666-01-01.svg" ></img></span>
                </div>
                <div>
                    <div class="text">
                        <div class="headline">
                            <img src="/resource/icon/alarm.png" />
                            <p>{showType} 대응 행동 메뉴얼</p>
                        </div>
                        <p>1){title}</p>
                        <br/>
                        <p>2)&nbsp; 수신자</p>
                        {showMembers}
                        <br />
                        <p>3)&nbsp; 내용</p>
                        <p style={{ textIndent: "15px", lineHeight: "20px" }}>{contents}</p>
                    </div>
                    <div class="confirm" onClick={this.onClickConfirm} >
                        <p>확인</p>
                    </div>
                </div>
            </div>
        );
    }
}


