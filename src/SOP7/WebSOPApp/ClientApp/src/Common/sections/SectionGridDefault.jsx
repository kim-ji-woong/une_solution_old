import React, { Component } from 'react';
import SopManager from '../../SOPManager/ui/sopManager';
import styles from '../css/section.module.css';
import commonStyles from '../css/style.module.css';

class SectionGridDefault extends Component {
    onClickMenu(menu) {
        this.props.content(menu, null);
    }

	render() {
        return (
            <div className={styles.defaultGrid}>
                <div className={styles.defaultGridArea}>
                    <div className={styles.defaultButtonAreaV}>
                        <div className={styles.defaultButtonAreaH}>
                            <button className={styles.clickable} onClick={(e) => this.onClickMenu(SopManager.menu.newSOP)}>{SopManager.menu.newSOP}</button>
                            <button className={styles.clickable} onClick={(e) => this.onClickMenu(SopManager.menu.open)}>{SopManager.menu.open}</button>
                            <button className={styles.clickable} onClick={(e) => this.onClickMenu(SopManager.menu.openXML)}>{SopManager.menu.openXML}</button>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}

export default SectionGridDefault;