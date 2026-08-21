import React from 'react';
import SectionComponent from './sectionComponent';
import SectionDataInternal from '../../models/sections/sectionDataInternal.js';
import sectionStyles from '../../css/section.module.css';
import SectionData from '../../models/sections/sectionData';
import SopManagerResource from '../../../SOPManager/resource/id';
import SectionGridCell from '../sectionGridCell';

class Internal extends SectionComponent {
    constructor(props) {
        super(props);
        this.props = props;

        // 실행모드 경우 제외
        if (this.props.mode !== "exec") {
            this.props.onClickComponent(this.props.sectionData);
        }
    }

    getTranstypeElement() {
        return (
            <div className={sectionStyles.sectionMarkArea}>
                {
                    this.props.sectionData.isSMS &&
                    <div className={sectionStyles.sectionMark + " " + sectionStyles.sms + " " + sectionStyles.internal}>{SopManagerResource.ID.sectionMark.sms}</div>
                }
                {
                    this.props.sectionData.isBroadcast &&
                    <div className={sectionStyles.sectionMark + " " + sectionStyles.broad + " " + sectionStyles.internal}>{SopManagerResource.ID.sectionMark.broadcast}</div>
                }
                {
                    this.props.sectionData.isEmail &&
                    <div className={sectionStyles.sectionMark + " " + sectionStyles.email + " " + sectionStyles.internal}>{SopManagerResource.ID.sectionMark.email}</div>
                }
                {
                    this.props.sectionData.checked &&
                    <div className={sectionStyles.sectionMark + " " + sectionStyles.internal + " " + sectionStyles.checkComponent}></div>
                }
            </div>
        );
        
        /*if (this.props.sectionData.isSMS) {
            return <div className={sectionStyles.sectionMark + " " + sectionStyles.sms + " " + sectionStyles.internal}>{SopManagerResource.ID.sectionMark.sms}</div>
        }
        else if (this.props.sectionData.isBroadcast) {
            return <div className={sectionStyles.sectionMark + " " + sectionStyles.broad + " " + sectionStyles.internal}>{SopManagerResource.ID.sectionMark.broadcast}</div>
        }
        else if (this.props.sectionData.isEmail) {
            return <div className={sectionStyles.sectionMark + " " + sectionStyles.email + " " + sectionStyles.internal}>{SopManagerResource.ID.sectionMark.email}</div>
        }

        return <></>*/
    }

    getAutoElement() {
        if (this.props.sectionData.autoRun) {
            return <div className={sectionStyles.sectionMark + " " + sectionStyles.auto + " " + sectionStyles.internal}>{SopManagerResource.ID.sectionMark.auto}</div>
        }

        return <></>
    }

    render() {
        const styleValue = this.getStyleValue("internal");
        const arrowButtons = this.makeArrowButtons();
        let sectionClassName = sectionStyles.internalOuter;
        let sectionClassName2 = sectionStyles.sectionComponent + " " + sectionStyles.internal;
        let statusClass = "";
        let statusBorder = "";

        if (this.props.isSelected) {
            if (SectionGridCell.isEditMode(this.props.mode)) {
                sectionClassName += " " + sectionStyles.selected;
                sectionClassName2 += " " + sectionStyles.selected;
            }
            else {
                sectionClassName += " " + sectionStyles.current;
                sectionClassName2 += " " + sectionStyles.current;
            }
        }

        if (this.props.status === SectionData.Status_Run) {
            statusClass = " " + sectionStyles.runComponent;

            if (!this.props.isSelected) {
                statusBorder = " " + sectionStyles.runBorder;
            }
            //statusBorder = " " + sectionStyles.runBorder;
        } else if (this.props.status === SectionData.Status_Done) {
            statusClass = " " + sectionStyles.doneComponent;

            if (!this.props.isSelected) {
                statusBorder = " " + sectionStyles.doneBorder;
            }
            //statusBorder = " " + sectionStyles.doneBorder;
        } else if (this.props.status === SectionData.Status_Normal) {
            statusClass = " " + sectionStyles.waitComponent;

            if (!this.props.isSelected) {
                statusBorder = " " + sectionStyles.waitBorder;
            }
        }

        return (
            <div className={sectionStyles.sectionInternal}>
                {
                    this.getTranstypeElement()
                }
                <div className={sectionStyles.internalArrowBox}>
                    <div className={sectionClassName + statusBorder}>
                        <div className={sectionClassName2 + statusClass} style={styleValue} onClick={() => this.props.onClickComponent(this.getSectionData())}>
                            {this.props.sectionData.text}
                        </div>
                    </div>
                    {arrowButtons}
                </div>
                {
                    this.getAutoElement()
                }
            </div>
        );
    }

    static makeSectionData() {
        return new SectionDataInternal();
    }
}

export default Internal;