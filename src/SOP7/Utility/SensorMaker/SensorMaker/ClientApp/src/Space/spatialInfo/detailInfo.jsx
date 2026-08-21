import React, { Component } from 'react';
import styles from '../css/spatial.module.css';

export class DetailInfo extends Component {
    /* constructor(props) {
        super(props);
        this.state = {
            searchText: ''
        }

        this.refLayer = React.createRef();
        this.refScrollArea = React.createRef();
        this.refScrollbar = React.createRef();
        this.refTree = React.createRef();
    } */

    render() {
        return (
            <>
                <div className={styles.detailInfo}>
                    <span className={styles.detailTitle}>세부 정보</span>
                    <span className={styles.detailArea}>제어실 부속 회의실</span>

                    <div className={styles.detailBoxBorder}>
                        <div className={styles.detailBox}>
                            <div className={styles.detailCheck1}>
                                <span className={styles.checks}>
                                    <input type="checkbox" id="ex_chk" /><label htmlFor="ex_chk"><span className={styles.checksMargin}>화면 표시 이름</span></label>
                                </span>
                                <span className={styles.deSquareL}>통합관제실</span>
                            </div>
                            <div className={styles.detailCheck2}>
                                <span className={styles.checks}>
                                    <input type="checkbox" id="ex_chkk" /><label htmlFor="ex_chkk"><span className={styles.checksMargin}>방송용 이름</span></label>
                                </span>
                                <span className={styles.deSquareL}>통합관제실</span>
                            </div>
                        </div>

                        <div className={styles.detailBottom}>
                            <div className={styles.detailLeftBox}>
                            <div className={styles.detailCheck3}>
                                <span className={styles.checks}>
                                    <input type="checkbox" id="ex_chkkk" /><label htmlFor="ex_chkkk"></label>
                                </span>
                                <span className={styles.detailText}>Text Center</span>
                            </div>
                                <span className={styles.detailLeftFirst}>층 정보<span className={styles.detailLeftArea}><span className={styles.deSquareS}>1</span><span className={styles.detailFloor}>층</span></span></span>
                                <span className={styles.detailLeftSecond}>
                                    <span className={styles.checks}>
                                        <input type="checkbox" id="ex_chkkkk" /><label htmlFor="ex_chkkkk" className={styles.checkName}><span className={styles.checksMargin}>중층</span></label>
                                    </span>
                                    <span className={styles.deSquareSS}>2</span>
                                </span>
                            </div>
                            <div className={styles.detailRightBox}>
                                <span className={styles.detailRightText}>공간 좌표</span>
                                <span className={styles.detailXbox}><span className={styles.detailX}>X</span><span className={styles.deSquareM}></span></span>
                                <span className={styles.detailYbox}><span className={styles.detailY}>Y</span><span className={styles.deSquareM}></span></span>
                                <span className={styles.detailZbox}><span className={styles.detailZ}>Z</span><span className={styles.deSquareM}></span></span>
                            </div>
                        </div>
                    </div>
                </div>
            </>
        );
    }
}




