import React, { Component } from 'react';
import styles from '../../Common/css/style.css';
import Title from '../../Root/title';
import Menu from '../../Root/menu';

import FacilityTypeResource from '../resource/id';
import SessionString from '../../Common/js/sessionString';

class TypeMenu extends Component {

    onClickFire = () => {
        this.selectFacility(FacilityTypeResource.ID.facilityType.fire);
    }

    onClickFlood = () => {
        this.selectFacility(FacilityTypeResource.ID.facilityType.flood);
    }

    onClickHeat = () => {
        this.selectFacility(FacilityTypeResource.ID.facilityType.heat);
    }

    onClickCollapse = () => {
        this.selectFacility(FacilityTypeResource.ID.facilityType.collapse);
    }

    selectFacility = (type) => {
        window.sessionStorage.removeItem(SessionString.Key.selectSensor);
        window.sessionStorage.setItem(SessionString.Key.facilityType, type);
        this.props.history.push(Menu.pathMain);
    }

    render() {

        return (
            <div className="container_sub">

                <Title navigation={false} />

                <div className="contents">
                    <div className="disaster_title">
                        <h2>재난 유형 선택</h2>
                        <p>위기 경보수준 관리시스템 이용을 위해<br/>재난 유형을 선택해주세요.</p>
                    </div>
                    <div className="disaster_box">
                        <div className="fire_disaster" onClick={this.onClickFire} >
                            <embed src="/resource/icon/fire-03-03.svg" ></embed>
                            <p id="box_title">'화재' 재난 유형 선택</p>
                            <p id="sub_title">화재 위기경보수준 관리 정보를 불러옵니다.</p>
                        </div>
                        <div className="flood_disaster" onClick={this.onClickFlood}>
                            <embed src="/resource/icon/flood-03.svg" ></embed>
                            <p id="box_title">'홍수' 재난 유형 선택</p>
                            <p id="sub_title">홍수 위기경보수준 관리 정보를 불러옵니다.</p>
                        </div>
                        <div className="warming_disaster" onClick={this.onClickHeat}>
                            <embed src="/resource/icon/warming-03.svg" ></embed>
                            <p id="box_title">'폭염' 재난 유형 선택</p>
                            <p id="sub_title">폭염 위기경보수준 관리 정보를 불러옵니다.</p>
                        </div>
                        <div className="landslide_disaster" onClick={this.onClickCollapse}>
                            <embed src="/resource/icon/landslide-03.svg" ></embed>
                            <p id="box_title">'경사지 붕괴' 재난 유형 선택</p>
                            <p id="sub_title">경사지 붕괴 위기정보수준 관리 정보를 불러옵니다.</p>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}

export default TypeMenu;
