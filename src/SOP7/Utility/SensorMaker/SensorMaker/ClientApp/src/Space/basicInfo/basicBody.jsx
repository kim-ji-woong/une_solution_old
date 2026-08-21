import React, { Component } from 'react';
/* import { LogoInfo } from './logoInfo'; */
import { SensorTypeInfo } from './sensorTypeInfo';
import space from './../css/space.module.css';

export class BasicBody extends Component {
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

    render() {
       {/*  console.log('basic'); */}
        return (
            <>
               <span className={space.logoTitle}>기본정보 등록</span>
               <div className={space.basicInfoBox}>
               <SensorTypeInfo />
               <div className={space.logoArea}>
                <div className={space.logoBox}>
                  <div className={space.logoTitleBox}>
                     <input type="text" placeholder="입력하세요" className={space.logoInput} />
                     {/* <span className={space.logoInput}>입력하세요</span> */}
                     <span className={space.logoTitleIcon}></span>
                  </div>
                  <div className={space.logoContent}>
                      <span className={space.logoSelect}> {/* 전체영역 클릭해서 업로드( 버튼 X ) */}
                        <span className={space.logoAdd}></span>
                        <span className={space.uploadText}>Click files to Upload</span>
                      </span>
                        <p className={space.logoText1}>preview</p>
                        <p className={space.logoText2}>- 800*600 해상도 권장</p>
                        <p className={space.logoText3}>- png, jpeg 파일만 업로드 가능</p>
                  </div>
                  <span className={space.registration}>등록</span>
                </div>
                </div>
               </div>
            </>            
            );
    }
}