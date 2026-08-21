import React, { Component } from 'react';
/*import styles from '../../../Common/css/style.module.css';*/
import uis from '../../../Common/css/ui.module.css';
import uneStyles from '../../../Common/css/uneCommon.module.css';
import SectionData from '../../../Common/models/sections/sectionData';

class EndPoint extends Component {
    constructor(props) {
        super(props);

        this.state = {
            prevProps: props
        }

        this.props = props;

        this.runSection = this.runSection.bind(this);
    }

    async runSection() {        
        await this.props.runSection(this.props.sectionData);
    }

    render() {
        let boxClassName = "";
        let textClassName = " " + uis.textNormal;
        if (this.props.currentSection.id === this.props.sectionData.id && this.props.currentSection.componentType === this.props.sectionData.componentType) {
            boxClassName = " " + uis.sectionCurrent + " " + uneStyles.currentBox;
            textClassName = " " + uis.textCurrent;
        } else if (this.props.sectionData.status === SectionData.Status_Run) {
            boxClassName = " " + uis.sectionRun;
            textClassName = " " + uis.textRun;
        } else if (this.props.sectionData.status === SectionData.Status_Done) {
            boxClassName = " " + uis.sectionDone;
            textClassName = " " + uis.textDone;
        }

        // 다음 버튼 활성화
        var btnNextClassName = uis.btnAllCheck;
        // TODO: 테스트를 위한
        //if (!this.props.sectionData.isBegin &&
        //    this.props.currentSection.componentType === 3 && this.props.currentSection.isBegin) {
        if (!this.props.sectionData.isBegin &&
            this.props.currentSection.componentType === 3 && this.props.currentSection.isBegin) {
            // 현재 SOP가 시작되지 않았을 때 Disable
            btnNextClassName = uis.btnDisable;
        }
        else if (this.props.sectionData.status === SectionData.Status_Done) {
            // 현재 임무가 완료된 상태라면 Disable
            btnNextClassName = uis.btnDisable;
        }

        return (
                <div className={uis.sectionBox + boxClassName} id={this.props.id}>
                    <div className={uis.tit + " " + uis.clfix + " " + uis.sectionBoxStart + textClassName}>
                    <strong>{this.props.sectionData.sectionNumber}.{this.props.sectionData.text}</strong>
                    <div className={uis.btnArea + " " + uneStyles.btnArea}>
                       <a className={btnNextClassName} onClick={this.runSection} title={this.props.sectionData.text}>{this.props.sectionData.text}</a>
                    </div>
                    </div>
                </div>
        );
    }
}

export default EndPoint;