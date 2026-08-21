import React, { Component } from 'react';
import TreeView from '../../utility/treeview';
import space from './../css/space.module.css';
import $ from 'jquery';

export class SensorTypeInfo extends Component {
    constructor(props) {
        super(props);
        this.state = {
            searchText: ''
        }

        this.refLayer = React.createRef();
        this.refScrollArea = React.createRef();
        this.refScrollbar = React.createRef();
        this.refTree = React.createRef();
    }


    componentDidMount() {

        $('.' + space.sensorTypeIcon2).mouseover(function () {
            $('.' + space.sensorUl).show();
            console.log('space.sensorUl');
        });

        $('.' + space.sensorItem).mouseover(function () {
            $('.' + space.sensorUlDetail).show();
            console.log('space.sensorUlDetail');
        });

        /* $('.' + space.sensorUlDetail).mouseleave(function () {
            $('.' + space.sensorUlDetail).hide();
            $('.' + space.sensorItem).hide();
        }); */
    }



    /* 
    const button = document.querySelector('.button');

    button.addEventListener('click', () => {
        const dropdown = document.querySelector('.dropdown');
        dropdown.style.display = 'block';
    });

    button.addEventListener('blur', () => {
        const dropdown = document.querySelector('.dropdown');
        dropdown.style.display = '';
    });
    */



    render() {
        return (
            <>
                <div className={space.sensorTypeBox}>
                   <div className={space.sensorTitleBox}>
                    <span className={space.sensorTypeTitle}>센서 유형 등록</span>
                        <span className={space.sensorTypeIcon2}>공간유형 선택</span>
                        <span className={space.sensorUl}>
                            <ul className={space.sensorItem}>
                                <li>화재</li>
                                <li>누출</li>
                                <li>보안</li>
                                <li>CCTV</li>
                                <li>기타</li>
                            </ul>
                        </span>
                        <span className={space.sensorUlDetail}>
                            <ul className={space.sensorDetailItem}>
                                <li>발신기</li>
                                <li>연기감지기</li>
                                <li>불꽃감지기</li>
                            </ul>
                        </span> 

                        {/* <div className={space.buttonContainer}>
                            <button className={space.button}>클릭</button>
                            <div className={space.dropdown}>
                                <div className={space.dropdownItem}>서울</div>
                                <div className={space.dropdownItem}>대전</div>
                                <div className={space.dropdownItem}>대구</div>
                                <div className={space.dropdownItem}>부산</div>
                            </div>
                        </div> */}

                  </div>
                  <div className={space.sensorContScrollbar}>
                  <div className={space.sensorContent}>
                    <div className={space.sensorLine}><span className={space.sensorName}>화재_발신기1</span><span className={space.sensorReIcon}></span><span className={space.sensorMinusIcon}></span></div>
                    <div className={space.sensorLine}><span className={space.sensorName}>누출_발신기2</span><span className={space.sensorReIcon}></span><span className={space.sensorMinusIcon}></span></div>
                    <div className={space.sensorLine}><span className={space.sensorName}>누출_감지기1</span><span className={space.sensorReIcon}></span><span className={space.sensorMinusIcon}></span></div>
                    <div className={space.sensorLine}><span className={space.sensorName}>화재_연기감지기</span><span className={space.sensorReIcon}></span><span className={space.sensorMinusIcon}></span></div>
                    <div className={space.sensorLine}><span className={space.sensorName}>화재_연기감지기</span><span className={space.sensorReIcon}></span><span className={space.sensorMinusIcon}></span></div>
                    <div className={space.sensorLine}><span className={space.sensorName}>화재_연기감지기</span><span className={space.sensorReIcon}></span><span className={space.sensorMinusIcon}></span></div>
                    <div className={space.sensorLine}><span className={space.sensorName}>화재_연기감지기</span><span className={space.sensorReIcon}></span><span className={space.sensorMinusIcon}></span></div>
                    <div className={space.sensorLine}><span className={space.sensorName}>화재_연기감지기</span><span className={space.sensorReIcon}></span><span className={space.sensorMinusIcon}></span></div>
                    <div className={space.sensorLine}><span className={space.sensorName}>화재_연기감지기</span><span className={space.sensorReIcon}></span><span className={space.sensorMinusIcon}></span></div>
                    <div className={space.sensorLine}><span className={space.sensorName}>화재_연기감지기</span><span className={space.sensorReIcon}></span><span className={space.sensorMinusIcon}></span></div>
                    <div className={space.sensorLine}><span className={space.sensorName}>화재_연기감지기</span><span className={space.sensorReIcon}></span><span className={space.sensorMinusIcon}></span></div>
                    <div className={space.sensorLine}><span className={space.sensorName}>화재_연기감지기</span><span className={space.sensorReIcon}></span><span className={space.sensorMinusIcon}></span></div>
                    <div className={space.sensorLine}><span className={space.sensorName}>화재_연기감지기</span><span className={space.sensorReIcon}></span><span className={space.sensorMinusIcon}></span></div>
                    <div className={space.sensorLine}><span className={space.sensorName}>화재_연기감지기</span><span className={space.sensorReIcon}></span><span className={space.sensorMinusIcon}></span></div>
                  </div>
                  </div>
                </div>

                <TreeView
                    treeViewID="teamTree"
                    teamTreeData={this.props.teamTreeData}
                    /*onTreeNodeChanged={this.onTreeNodeChanged}*/
                    isEditMode={this.props.isEditMode}
                    editNodeID={this.state.editNodeID}
                    editTeamInfo={this.editTeamInfo}
                    selectedTeam={this.props.selectedTeam}
                    addTeam={this.addTeam}
                    editTeam={this.editTeam}
                    removeTeam={this.removeTeam}
                />
            </>
            );
    }
}