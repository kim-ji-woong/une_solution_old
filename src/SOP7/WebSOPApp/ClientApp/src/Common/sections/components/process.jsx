import React from 'react';
import SectionComponent from './sectionComponent';
import SectionDataProcess from '../../models/sections/sectionDataProcess.js';
import sectionStyles from '../../css/section.module.css';
import SectionData from '../../models/sections/sectionData';
import SopManagerResource from '../../../SOPManager/resource/id';
import SectionGridCell from '../sectionGridCell';

class Process extends SectionComponent {
    constructor(props) {
        super(props);
        this.props = props;

        // 실행모드 경우 제외
        if (this.props.mode !== "exec") {
            this.props.onClickComponent(this.props.sectionData);
        }
    }

    getMarkAreaElement() {
        return (
            <div className={sectionStyles.sectionMarkArea + " " + sectionStyles.process}>
                {
                    this.props.sectionData.checked &&
                    <div className={sectionStyles.sectionMark + " " + sectionStyles.process + " " + sectionStyles.checkComponent}></div>
                }
            </div>
        );
    }

    getAutoElement() {
        if (this.props.sectionData.autoRun) {
            return <div className={sectionStyles.sectionMark + " " + sectionStyles.auto + " " + sectionStyles.process}>{SopManagerResource.ID.sectionMark.auto}</div>
        }

        return <></>
    }

    render() {
        const styleValue = this.getStyleValue("process");
        const arrowButtons = this.makeArrowButtons();
        let sectionID = "process";
        let statusClass = "";

        if (this.props.isSelected) {
            if (SectionGridCell.isEditMode(this.props.mode)) {
                sectionID = sectionStyles.selectedComponent;
            }
            else {
                sectionID = sectionStyles.currentComponent;
            }
        }

        if (this.props.status === SectionData.Status_Run) {
            statusClass = " " + sectionStyles.runComponent;
        } else if (this.props.status === SectionData.Status_Done) {
            statusClass = " " + sectionStyles.doneComponent;
        } else if (this.props.status === SectionData.Status_Normal) {
            statusClass = " " + sectionStyles.waitComponent;
        }

        return (
            <div className={sectionStyles.sectionProcess}>
                {
                    this.getMarkAreaElement()
                }
                <div id={sectionID} className={sectionStyles.sectionComponent + " " + sectionStyles.process + " " + sectionStyles.round + statusClass} style={styleValue} onClick={() => this.props.onClickComponent(this.getSectionData())}>
                    {
                        (this.props.mode === "exec") ? this.props.sectionData.sectionNumber + '.' : ''
                    }
                    {this.props.sectionData.text}
                    {arrowButtons}
                </div>
                {
                    this.getAutoElement()
                }
            </div>
        );
    }

    static makeSectionData() {
        return new SectionDataProcess();
    }
}

export default Process;