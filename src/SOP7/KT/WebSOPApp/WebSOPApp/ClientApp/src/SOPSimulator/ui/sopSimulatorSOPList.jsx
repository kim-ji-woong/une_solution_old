import React, { Component } from 'react';
import { BrowserRouter as Route, Link } from 'react-router-dom';
import $ from 'jquery';
import uis from '../../Common/css/ui.module.css';
import uneStyles from '../../Common/css/uneCommon.module.css';
import SopController from '../../SOPManager/services/sopController';

import apps from '../css/app.module.css';

class SopSimulatorSOPList extends Component {
    constructor(props) {
        super(props);
        this.props = props;

        this.state = {
            disasterCategories: [],
            disasterCategoriesEmergency: [],
            isNormal: true,
            isEmergency: false,
            selectedDisasterCategory: null, // 재난분야
            selectedSubDisasterCategory: null,      // 재난종류
            selectedDisaster: null,         // 재난상황
            selectedVersion: null           // 선택 SOP
        };
    }

    componentDidMount() {
        this.getDisasterCategories();
        $('html, body').css({ 'display': 'block', 'height': '100%', 'overflow': 'hidden', 'color': '#fff' });
        // 각 페이지 별로 클래스 초기화
        $('#subPage').removeClass('sop');

        /*$('.' + uis.list).on('click', function () {
            var isShow = $(this).attr('list');

            $('.' + uis.list).removeClass('isShow');
            $(this).addClass('isShow');
            $('.' + isShow).addClass('isShow');
        });*/


        // E-SOP 매뉴얼 Left List Toggle
        $('.' + uis.menualListWrap).on('click', 'dt', function () {
            $(this).closest('.' + uis.list).toggleClass(uis.isShow).siblings().removeClass(uis.isShow);
        });

        // 재난분야 버튼 이벤트
        $('.category dd ul').on('click', 'li', function () {
            //console.log("재난분야 버튼 테스트");

            // 재난분야 메뉴 닫기
            $('.category').removeClass(uis.isShow);
            // 재난종류 메뉴 열기
            $('.subCategory').addClass(uis.isShow);
        });

        // 재난종류 버튼 이벤트
        $('.subCategory dd ul').on('click', 'li', function () {
            //console.log("재난종류 버튼 테스트");
        });

        // 페이지 타이틀 
        $('#pageTitle').text("E-SOP");
    }

    async getDisasterCategories() {
        
        const [disasterCategories, message] = await SopController.disasterCategories(true);
        const [disasterCategoriesEmergency, message2] = await SopController.disasterCategories(false);

        this.setState({ disasterCategories, disasterCategoriesEmergency });
    }

    onSelectDisasterCategory = (disasterCategoryData) => {
        this.setState({ selectedDisasterCategory: disasterCategoryData, selectedSubDisasterCategory: null });
    }

    onSelectSubDisasterCategory = (subDisasterCategoryData) => {
        this.setState({ selectedSubDisasterCategory: subDisasterCategoryData });
    }

    onSelectVersion = (versionID) => {
        this.props.openDB(versionID, null);
    }

    onChangeNormal = () => {
        this.setState({ isNormal: !this.state.isNormal });
    }
    onChangeEmergency = () => {
        this.setState({ isEmergency: !this.state.isEmergency });
    }

    SetDisasterUI() {
        if (this.state.disasterCategories === null || this.state.disasterCategoriesEmergency === null)
            return;
        
        var disasterCategory = [];
        var subDisasterCategory = [];
        var disaster = [];

        disasterCategory.push(
            <li className={uis.isActive} key={'disasterCategory/'}>
                <a className={uis.btnList} onClick={() => this.onSelectDisasterCategory(null)}>
                    전체
                </a>
            </li>
        );

        subDisasterCategory.push(
            <li key={'subDisasterCategory/'}>
                <a className={uis.btnList} onClick={() => this.onSelectSubDisasterCategory(null)}>
                    전체
                </a>
            </li>
        );

        if (this.state.isNormal) {
            for (var i = 0; i < this.state.disasterCategories.length; i++) {
                const dc = this.state.disasterCategories[i];
                disasterCategory.push(
                    <li className={uis.isActive} key={'disasterCategory/' + dc.disasterCategory.id}>
                        <a className={uis.btnList} key={dc} onClick={() => this.onSelectDisasterCategory(dc.disasterCategory)}>
                            {dc.disasterCategory.categoryName}
                        </a>
                    </li>
                );

                for (var j = 0; j < dc.subDisasterCategories.length; j++) {
                    const sdc = dc.subDisasterCategories[j];
                    if (this.state.selectedDisasterCategory === null || this.state.selectedDisasterCategory.id === sdc.subDisasterCategory.disasterCategoryID) {

                        subDisasterCategory.push(
                            <li key={'subDisasterCategory/' + sdc.subDisasterCategory.id}>
                                <a className={uis.btnList} key={sdc} onClick={() => this.onSelectSubDisasterCategory(sdc.subDisasterCategory)}>
                                    {sdc.subDisasterCategory.subCategoryName}
                                </a>
                            </li>
                        );



                        for (var k = 0; k < sdc.disasterDatas.length; k++) {
                            const d = sdc.disasterDatas[k];
                            const allVersions = [];
                            for (var q = 0; q < d.disasterDatas.length; q++) {
                                if (this.state.selectedSubDisasterCategory === null || this.state.selectedSubDisasterCategory.id === d.disasterDatas[q].disaster.subDisasterCategoryID) {
                                    const isNormal = d.disasterDatas[q].version.isNormal;
                                    if ((isNormal && this.state.isNormal) || (!isNormal && this.state.isEmergency)) {
                                        allVersions.push(d.disasterDatas[q].version);
                                    }
                                }
                            }

                            let lastAccessVersion = (allVersions.length > 0) ? allVersions[0] : null;
                            for (var q = 0; q < allVersions.length; q++) {
                                if (allVersions.length - 1 >= q + 1) {
                                    if (lastAccessVersion < allVersions[q + 1].lastAccessTime) {
                                        lastAccessVersion = allVersions[q + 1];
                                    }
                                }
                            }

                            if (lastAccessVersion !== null) {
                                const lastAccessTime = lastAccessVersion.lastAccessTime.replace('T', ' '); //this.getMakeDateTime(lastAccessVersion.lastAccessTime);

                                disaster.push(
                                    <li key={'disaster/' + lastAccessVersion.id}>
                                        <a key={d} onClick={() => this.onSelectVersion(lastAccessVersion.id)}>
                                            <p>{dc.disasterCategory.categoryName}&nbsp;&gt;&nbsp;{sdc.subDisasterCategory.subCategoryName}&nbsp;&gt;&nbsp;{d.disasterName}</p>
                                            {
                                                (lastAccessVersion.isNormal)
                                                    ? <span className={uis.noti}>평일/주간</span>
                                                    : <span className={uis.noti + " " + uis.cRed}>휴일/야간</span>
                                            }
                                            <span className={uis.date}>{lastAccessTime}</span>
                                        </a>
                                    </li>
                                );
                            }
                        }
                    }
                }
            }
        }

        if (this.state.isEmergency) {
            for (var i = 0; i < this.state.disasterCategoriesEmergency.length; i++) {
                const dc = this.state.disasterCategoriesEmergency[i];
                
                for (var j = 0; j < dc.subDisasterCategories.length; j++) {
                    const sdc = dc.subDisasterCategories[j];
                    if (this.state.selectedDisasterCategory === null || this.state.selectedDisasterCategory.id === sdc.subDisasterCategory.disasterCategoryID) {

                        for (var k = 0; k < sdc.disasterDatas.length; k++) {
                            const d = sdc.disasterDatas[k];
                            const allVersions = [];
                            for (var q = 0; q < d.disasterDatas.length; q++) {
                                if (this.state.selectedSubDisasterCategory === null || this.state.selectedSubDisasterCategory.id === d.disasterDatas[q].disaster.subDisasterCategoryID) {
                                    const isNormal = d.disasterDatas[q].version.isNormal;
                                    if ((isNormal && this.state.isNormal) || (!isNormal && this.state.isEmergency)) {
                                        allVersions.push(d.disasterDatas[q].version);
                                    }
                                }
                            }

                            let lastAccessVersion = (allVersions.length > 0) ? allVersions[0] : null;
                            for (var q = 0; q < allVersions.length; q++) {
                                if (allVersions.length - 1 >= q + 1) {
                                    if (lastAccessVersion < allVersions[q + 1].lastAccessTime) {
                                        lastAccessVersion = allVersions[q + 1];
                                    }
                                }
                            }

                            if (lastAccessVersion !== null) {
                                const lastAccessTime = lastAccessVersion.lastAccessTime.replace('T', ' ');//this.getMakeDateTime(lastAccessVersion.lastAccessTime);
                                disaster.push(
                                    <li key={'disaster/' + lastAccessVersion.id}>
                                        <a key={d} onClick={() => this.onSelectVersion(lastAccessVersion.id)}>
                                            <p>{dc.disasterCategory.categoryName}&nbsp;&gt;&nbsp;{sdc.subDisasterCategory.subCategoryName}&nbsp;&gt;&nbsp;{d.disasterName}</p>
                                            {
                                                (lastAccessVersion.isNormal)
                                                    ? <span className={uis.noti}>평일/주간</span>
                                                    : <span className={uis.noti + " " + uis.cRed}>휴일/야간</span>
                                            }
                                            <span className={uis.date}>{lastAccessTime}</span>
                                        </a>
                                    </li>
                                );
                            }
                        }
                    }
                }
            }
        }
                
        return [disasterCategory, subDisasterCategory, disaster];
    }

    getMakeDateTime = (dateTime) => {
        let year = dateTime.getFullYear();
        let month = 1 + dateTime.getMonth();
        month = month >= 10 ? month : '0' + month;  //month 두자리로 저장
        let day = dateTime.getDate();                   //d
        day = day >= 10 ? day : '0' + day;

        let hour = dateTime.getHours();
        hour = hour >= 10 ? hour : '0' + hour;
        let min = dateTime.getMinutes();
        min = min >= 10 ? min : '0' + min;
        let sec = dateTime.getSeconds();
        sec = sec >= 10 ? sec : '0' + sec;

        let strDate = year + '-' + month + '-' + day + ' ' + hour + ':' + min + ':' + sec;
        return strDate;
    }

    render() {
        const [disasterCategory, subDisasterCategory, disaster] = this.SetDisasterUI();

        var categoryName = (this.state.selectedDisasterCategory === null) ? '전체' : this.state.selectedDisasterCategory.categoryName; 
        var subCategoryName = (this.state.selectedSubDisasterCategory === null) ? '전체' : this.state.selectedSubDisasterCategory.subCategoryName;

        return (
                <section className={uis.appContainerWrapp + " " + uis.clfix}>
                    <div className={uis.appContainer}>
                        <section className={uis.subSection + " " + uis.menualListWrap}>
                            <dl>
                                <div className={uis.list + ' category'}>
                                    <dt>재난분야<em>{categoryName}</em></dt>
                                    <dd>
                                        <ul className={uis.bullet}>
                                            {disasterCategory}
                                        </ul>
                                    </dd>
                                </div>
                                <div className={uis.list + ' subCategory'}>
                                    <dt>재난종류<em>{subCategoryName}</em></dt>
                                    <dd>
                                        <ul className={uis.bullet}>
                                            {subDisasterCategory}
                                        </ul>
                                    </dd>
                                </div>
                            </dl>
                        </section>
                        <section className={uis.subSection + " " + uis.boardListWrap}>
                            <div className={uis.tit + " " + uis.clfix}>
                                <strong>SOP 임무 목록</strong>
                                <div className={uis.filterArea + " " + uis.clfix}>
                                <div className={uis.checkBox}>
                                        <input type="checkbox" id="filter01" checked={this.state.isNormal} onChange={this.onChangeNormal}/>
                                        <label>평일/주간</label>
                                    </div>
                                    <div className={uis.checkBox + " " + uis.cRed}>
                                        <input type="checkbox" id="filter02" checked={this.state.isEmergency} onChange={this.onChangeEmergency}/>
                                        <label>휴일/야간</label>
                                    </div>
                                </div>
                            </div>
                            <div className={uis.innerSection + " " + uis.scrollbar}>
                            {/*<div className={uneStyles.innerSectionnnn + " " + uis.scrollbar}>*/}
                                <ol className={uis.numList + " " + uis.list}>
                                    {disaster}                                    
                                </ol>
                            </div>
                        </section>
                    </div>
                </section>
        );
    }
}
export default SopSimulatorSOPList;