import React from 'react';
import SectionComponent from './sectionComponent';
import SectionDataDecision from '../../models/sections/sectionDataDecision.js';
import sectionStyles from '../../css/section.module.css';
import SectionData from '../../models/sections/sectionData';

class Decision extends SectionComponent {
    constructor(props) {
        super(props);
        this.props = props;

        // 실행모드 경우 제외
        if (this.props.mode !== "exec") {
            this.props.onClickComponent(this.props.sectionData);
        }
    }

    render() {
        const styleValue = this.getStyleValue("decision");
        const arrowButtons = this.makeArrowButtons();
        let sectionClassName = sectionStyles.decisionOuter;
        let statusClass = "";
        //let statusBorder = "";

        if (this.props.isSelected) {
            sectionClassName += " " + sectionStyles.selected;
            statusClass = " " + sectionStyles.selected;
        }

        if (this.props.status === SectionData.Status_Run) {
            statusClass += " " + sectionStyles.runComponent;
            //statusBorder = " " + sectionStyles.runBorder;
            sectionClassName += " " + sectionStyles.runBorder;
        } else if (this.props.status === SectionData.Status_Done) {
            statusClass += " " + sectionStyles.doneComponent;
            //statusBorder = " " + sectionStyles.doneBorder;
            sectionClassName += " " + sectionStyles.doneBorder;
        } else if (this.props.status === SectionData.Status_Normal) {
            statusClass = " " + sectionStyles.waitComponent;

            if (!this.props.isSelected) {
                sectionClassName += " " + sectionStyles.waitBorder;
            }
        }

        return (
            <div className={sectionStyles.decisionArrowBox}>
                <div className={sectionClassName}>
                    <div className={sectionStyles.sectionComponent + " " + sectionStyles.decision + statusClass} style={styleValue} onClick={() => this.props.onClickComponent(this.getSectionData())}>
                        {
                            (this.props.mode === "exec") ? this.props.sectionData.sectionNumber + '.' : ''
                        }
                        {this.props.sectionData.text}
                    </div>
                </div>
                {arrowButtons}
            </div>
        );
    }

    static makeSectionData() {
        return new SectionDataDecision();
    }
}

export default Decision;