import React, { Component } from 'react';
import styles from '../css/homeStyle.module.css';

export class DefaultContents extends Component {
    render() {
        return (
            <div className={styles.bodyTop}>
                <div className={styles.bodyTopImg}>
                    <span className={styles.bodyTopTitle}>Unique & Experience</span>
                    <span className={styles.bodyTopText}>
                        (주)유엔이는 공간정보 기반의 재난,보안,시설물(FMS),에너지설비(BEMS)등을
                        통합 연계하여 모니터링하고, 이벤트 발생시 SOP기반의 상황 대응 솔루션을 제공하는
                        K-PSIM(Physical Security Information Management)플랫폼을 개발하여
                        서비스하고 있습니다.
                    </span>
                </div>
            </div>
        );
    }
}