import React, { Component } from 'react';
import styles from '../css/spatial.module.css';

export class UploadDetailInfo extends Component {
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
                    <div className={styles.detailBox2}>
                      <span className={styles.detailText2}>파일명 : A_bui_004.glb</span>
                      <span className={styles.detailText2}>용량 : 2mb</span>
                      <span className={styles.detailText2}>수정일 : 2022.01.11. pm 12:10</span>
                      <span className={styles.detailText2}>업로드 : 2022.01.13. pm 05:10</span>
                    </div>
                </div>
            </>
        );
    }
}


