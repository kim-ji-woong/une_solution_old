import React, { Component } from 'react';
import styles from '../../../Common/css/style.module.css';
import commonStyles from '../../../Common/css/common.module.css';
import '../../css/componentProperty.css';
import '../../../Common/css/scroll.css';
import SectionDataAnnotation from '../../../Common/models/sections/sectionDataAnnotation';

class AnnotationProperty extends Component {
    static cssStyles = styles;

    constructor(props) {
        super(props);
        this.props = props;

        const sectionData = new SectionDataAnnotation();

        if (this.props.sectionData) {
            SectionDataAnnotation.copyTo(this.props.sectionData, sectionData);
        }

        this.state = {
            instance: this,
            sectionData: sectionData,
            prevProps: this.props
        }

        this.refTitle = React.createRef();
    }

    componentWillUnmount() {
        // 창이 닫히게 될 경우 편집한 내용을 저장한다.
        this.saveSectionData(false);
    }

    static getDerivedStateFromProps(props, state) {
        if (props === state.prevProps) {
            return state;
        }

        if (props.sectionData !== state.prevProps.sectionData && state.sectionData) {
            // 다른 Process를 선택하여 창이 바뀌게 될 경우 편집한 내용을 저장한다.
            state.instance.saveSectionData(false);
        }

        const sectionData = new SectionDataAnnotation();

        if (props.sectionData) {
            SectionDataAnnotation.copyTo(props.sectionData, sectionData);
        }

        state.instance.refTitle.current.value = state.instance.refTitle.current.text = sectionData.text;

        return {
            instance: state.instance,
            sectionData: sectionData,
            prevProps: props
        };
    }

    onChangeText = (event) => {
    }

    onClickApply(ok) {
        if (ok) {
            this.saveSectionData(true);
        }
        else {
            this.setState({
                sectionData: null
            });
        }
    }

    saveSectionData(shouldUpdate) {
        const sectionData = { ...this.state.sectionData };
        sectionData.text = this.refTitle.current.value;
        this.props.onApplyComponentProperty(sectionData, this.props.actionStep, shouldUpdate);
    }

    render() {
        return (
            <div className={AnnotationProperty.cssStyles.sprCont + " " + commonStyles.pt0}>
                <div className={AnnotationProperty.cssStyles.sprmExp}>
                    <div>
                        <h4>설명문 작성</h4>
                        <div className={"scroll-wrapper " + AnnotationProperty.cssStyles.sprmExTxt + " scrollbar-outer scroll-textarea"} id="pos_relative">
                            <div className="scroll-content" id="annotation_scrollContent">
                                <textarea ref={this.refTitle} name="" id="" cols="30" rows="10" className={AnnotationProperty.cssStyles.sprmExTxt + " scrollbar-outer"} defaultValue={this.state.sectionData?.text} onChange={this.onChangeText}></textarea>
                            </div>
                            <div className="scroll-element scroll-x">
                                <div className="scroll-element_outer">
                                    <div className="scroll-element_size">
                                    </div>
                                    <div className="scroll-element_track">
                                    </div>
                                    <div className="scroll-bar" id="width_100px">
                                    </div>
                                </div>
                            </div>
                            <div className="scroll-element scroll-y">
                                <div className="scroll-element_outer">
                                    <div className="scroll-element_size">
                                    </div>
                                    <div className="scroll-element_track">
                                    </div>
                                    <div className="scroll-bar" id="height_100px_top0">
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div className={AnnotationProperty.cssStyles.sprBot}>
                    <a className={AnnotationProperty.cssStyles.clickable} onClick={() => this.onClickApply(true)}>확인</a>
                    <a className={AnnotationProperty.cssStyles.clickable} onClick={() => this.onClickApply(false)}>취소</a>
                </div>
            </div>
        );
    }
    /*constructor(props) {
        super(props);
        this.props = props;

        this.state = {
            sectionData: this.props.sectionData
        }

        this.refText = React.createRef();
    }

    componentWillReceiveProps(nextProps) {
        if (this.state.sectionData !== nextProps.sectionData) {
            this.setState({ sectionData: nextProps.sectionData });
        }
    }

    onApplyComponentProperty = () => {
        const sectionData = { ...this.state.sectionData };
        sectionData.text = this.refText.current.value;
        this.props.onApplyComponentProperty(sectionData, this.props.actionStep);
    }

    onChangeText = (event) => {
        const sectionData = { ...this.state.sectionData };
        sectionData.text = this.refText.current.value;
        this.setState({ sectionData });
    }

    render() {
        return (
            <>
                <div className="componentProperties">
                    <span className="componentType">설명</span>
                    <div className="annotationProperty">
                        <label className="annotationLabel">내용</label>
                        <textarea ref={this.refText} className="componentText" value={this.state.sectionData.text} onChange={this.onChangeText}/>
                    </div>
                </div>
                <button className="btnApply" onClick={this.onApplyComponentProperty}>적용</button>
            </>
        );
    }*/
}

export default AnnotationProperty;