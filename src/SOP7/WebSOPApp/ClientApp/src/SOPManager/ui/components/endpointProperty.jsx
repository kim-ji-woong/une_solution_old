import React, { Component } from 'react';
import '../../css/componentProperty.css';
import styles from '../../../Common/css/style.module.css';
import bodyStyles from '../../css/body.module.css';
import '../../css/componentProperty.css';
import '../../../Common/css/scroll.css';
import commonStyles from '../../../Common/css/common.module.css';
import SectionDataEndpoint from '../../../Common/models/sections/sectionDataEndpoint';

class EndpointProperty extends Component {
    static cssStyles = styles;

    constructor(props) {
        super(props);
        this.props = props;

        const sectionData = new SectionDataEndpoint();

        if (this.props.sectionData) {
            SectionDataEndpoint.copyTo(this.props.sectionData, sectionData);
        }

        this.state = {
            instance: this,
            sectionData: sectionData,
            prevProps: this.props
        };

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

        const sectionData = new SectionDataEndpoint();

        if (props.sectionData) {
            SectionDataEndpoint.copyTo(props.sectionData, sectionData);
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

    onChange(isBegin) {
        const sectionData = { ...this.state.sectionData };
        sectionData.isBegin = isBegin;
        this.setState({sectionData: sectionData});
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
            <div className={EndpointProperty.cssStyles.sprCont + " " + commonStyles.pt0}>
                <div className={EndpointProperty.cssStyles.sprmExp + " " + EndpointProperty.cssStyles.stend}>
                    <div>
                        <h4>시작/끝 작성</h4>
                        <div className={"scroll-wrapper " + EndpointProperty.cssStyles.sprmExTxt + " scrollbar-outer scroll-textarea"} id="pos_relative">
                            <div className="scroll-content" id="endpoint_scrollContent">
                                <textarea ref={this.refTitle} name="" id="" cols="30" rows="10" className={EndpointProperty.cssStyles.sprmExTxt + " scrollbar-outer"} defaultValue={this.state.sectionData?.text} onChange={this.onChangeText}></textarea>
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
                                    <div className="scroll-bar" id="height_100">
                                    </div>
                                </div>
                            </div>
                        </div>
                        <ul className={EndpointProperty.cssStyles.sprmStend}>
                            <li>
                                <label className={EndpointProperty.cssStyles.clickable}>
                                    <input type="radio" name="sprmStend" className={bodyStyles.labelInput} id={EndpointProperty.cssStyles.sprmStend01} checked={this.state.sectionData?.isBegin} onChange={() => this.onChange(true)} />
                                    시작
                                </label>
                            </li>
                            <li>
                                <label className={EndpointProperty.cssStyles.clickable}>
                                    <input type="radio" name="sprmStend" className={bodyStyles.labelInput} id={EndpointProperty.cssStyles.sprmStend02} checked={!this.state.sectionData?.isBegin} onChange={() => this.onChange(false)} />
                                    종료
                                </label>
                            </li>
							</ul>
						</div>
					</div>
                <div className={EndpointProperty.cssStyles.sprBot}>
                    <a className={EndpointProperty.cssStyles.clickable} onClick={() => this.onClickApply(true)}>확인</a>
                    <a className={EndpointProperty.cssStyles.clickable} onClick={() => this.onClickApply(false)}>취소</a>
                </div>
				</div>
            );
    }
    /*constructor(props)
    {
        super(props);
        this.props = props;

        this.state = {
            sectionData: this.props.sectionData
        }

        this.refBegin = React.createRef();
        this.refEnd = React.createRef();
        this.refText = React.createRef();
    }

    componentWillReceiveProps(nextProps) {
        if (this.state.sectionData !== nextProps.sectionData) {
            this.setState({ sectionData: nextProps.sectionData });
        }
    }

    onApplyComponentProperty = () => {
        const sectionData = { ...this.state.sectionData };
        sectionData.isBegin = this.refBegin.current.checked;
        sectionData.text = this.refText.current.value;

        this.props.onApplyComponentProperty(sectionData, this.props.actionStep);
    }

    onChange(isBegin) {
        const sectionData = { ...this.state.sectionData };
        sectionData.isBegin = isBegin;
        this.setState({ sectionData });
    }

    onChangeText = (event) => {
        const sectionData = { ...this.state.sectionData };
        sectionData.text = event.target.value;
        this.setState({ sectionData });
    }

    render() {
        return (
            <>
                <div className="componentProperties">
                    <span className="componentType">시작/끝</span>
                    <div className="endpointProperty">
                        <div>
                            <input ref={this.refBegin} type="radio" name="begin" onChange={() => this.onChange(true)} checked={this.state.sectionData.isBegin === true} />
                            <label htmlFor="begin">시작</label>
                        </div>

                        <div className="optionItem">
                            <input ref={this.refEnd} type="radio" name="end" onChange={() => this.onChange(false)} checked={this.state.sectionData.isBegin === false} />
                            <label htmlFor="end">종료</label>
                        </div>
                    </div>
                    <textarea ref={this.refText} className="componentText" value={this.state.sectionData.text} onChange={this.onChangeText} />
                </div>
                <button className="btnApply" onClick={this.onApplyComponentProperty}>적용</button>
            </>
        );
    }*/
}

export default EndpointProperty;