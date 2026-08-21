import React from 'react';
import SectionComponent from './sectionComponent';
import SectionDataAnnotation from '../../models/sections/sectionDataAnnotation.js';
import sectionStyles from '../../css/section.module.css';
import SectionData from '../../models/sections/sectionData';

class Annotation extends SectionComponent {
    constructor(props) {
        super(props);
        this.props = props;

        // 실행모드 경우 제외
        if (this.props.mode !== "exec") {
            this.props.onClickComponent(this.props.sectionData);
        }
    }

    render() {
        const styleValue = this.getStyleValue("annotation");
        const arrowButtons = this.makeArrowButtons();
        let sectionClassName = sectionStyles.sectionComponent + " " + sectionStyles.annotation;
        let sectionID = "annotation";
        let statusFillClass = " ";
        let statusBorderClass = " ";

        if (this.props.isSelected) {
            if (this.props.mode !== "exec") {
                sectionClassName += " " + sectionStyles.selected;
                sectionID = sectionStyles.selectedComponent;
                statusFillClass += " " + sectionStyles.selected;
            }
        }

        // SOP 실행모드에서는 무조건 대기상태여야만 한다.
        if (this.props.mode === "exec") {
            statusFillClass = " " + sectionStyles.waitAnnoFill;
            statusBorderClass += " " + sectionStyles.waitAnnoBorder;
        }

        return (
            <div className={sectionStyles.annotationArrowBox}>
                <div id={sectionID} className={sectionClassName + statusBorderClass}>
                    <div className={sectionStyles.inner + statusFillClass} style={styleValue} onClick={() => this.props.onClickComponent(this.getSectionData())}>
                        {this.props.sectionData.text}
                    </div>
                    <div id={sectionID} className={sectionStyles.edge}></div>
                </div>
                {arrowButtons}
            </div>
        );
    }

    static makeSectionData() {
        return new SectionDataAnnotation();
    }
}

export default Annotation;