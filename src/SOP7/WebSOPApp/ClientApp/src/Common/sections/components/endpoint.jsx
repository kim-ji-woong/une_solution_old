import React from 'react';
import SectionComponent from './sectionComponent';
import SectionDataEndpoint from '../../models/sections/sectionDataEndpoint.js';
import sectionStyles from '../../css/section.module.css';
import SectionData from '../../models/sections/sectionData';

class Endpoint extends SectionComponent {
    constructor(props) {
        super(props);
        this.props = props;

        // 실행모드 경우 제외
        if (this.props.mode !== "exec") {
            this.props.onClickComponent(this.props.sectionData);
        }
    }

    render() {
        const styleValue = this.getStyleValue("endpoint");
        const arrowButtons = this.makeArrowButtons();
        let sectionID = "endpoint";
        let statusClass = "";

        if (this.props.isSelected) {
            sectionID = sectionStyles.selectedComponent;
        }

        if (this.props.status === SectionData.Status_Run) {
            statusClass = " " + sectionStyles.runComponent;
        } else if (this.props.status === SectionData.Status_Done) {
            statusClass = " " + sectionStyles.doneComponent;
        } else if (this.props.status === SectionData.Status_Normal) {
            statusClass = " " + sectionStyles.waitComponent;
        }

        return (
            <div id={sectionID} className={sectionStyles.sectionComponent + " " + sectionStyles.endpoint + statusClass} style={styleValue} onClick={() => this.props.onClickComponent(this.getSectionData())}>
                {
                    (this.props.mode === "exec") ? this.props.sectionData.sectionNumber + '.' : ''
                }
                {this.props.sectionData.text}
                {arrowButtons}
            </div>
        );
    }

    static makeSectionData() {
        return new SectionDataEndpoint();
    }
}

export default Endpoint;