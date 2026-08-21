import React, { Component } from 'react';
import styles from '../../../Common/css/style.module.css';
import commonStyles from '../../../Common/css/common.module.css';
import '../../css/componentProperty.css';
import '../../../Common/css/scroll.css';

class ArrowProperty extends Component {
    static cssStyles = styles;

    constructor(props) {
        super(props);
        this.props = props;

        let arrowData = null;

        if (this.props.arrowData) {
            arrowData = this.props.arrowData.clone();
        }

        this.state = {
            instance: this,
            arrowData: arrowData,
            prevProps: this.props
        }

        this.refText = React.createRef();
    }

    componentWillUnmount() {
        // 창이 닫히게 될 경우 편집한 내용을 저장한다.
        this.saveData(false);
    }

    static getDerivedStateFromProps(props, state) {
        if (props === state.prevProps) {
            return state;
        }

        if (props.arrowData !== state.prevProps.arrowData && state.arrowData) {
            // 다른 Process를 선택하여 창이 바뀌게 될 경우 편집한 내용을 저장한다.
            state.instance.saveData(false);
        }

        let arrowData = null;

        if (props.arrowData) {
            arrowData = props.arrowData.clone();
        }

        state.instance.refText.current.value = state.instance.refText.current.text = arrowData.text;

        return {
            instance: state.instance,
            arrowData: arrowData,
            prevProps: props
        };
    }

    onChangeText = (event) => {
    }

    onClickApply(ok) {
        if (ok) {
            this.saveData(true);
        }
        else {
            this.setState({
                arrowData: null
            });
        }
    }

    saveData(shouldUpdate) {
        const arrowData = { ...this.state.arrowData };
        arrowData.text = this.refText.current.value;
        this.props.onApplyComponentProperty(arrowData, this.props.actionStep, shouldUpdate);
    }

    render() {
        return (
            <div className={ArrowProperty.cssStyles.sprCont + " " + commonStyles.pt0}>
                <div className={ArrowProperty.cssStyles.sprmExp}>
                    <div>
                        <h4>화살표 작성</h4>
                        <div className={"scroll-wrapper " + ArrowProperty.cssStyles.sprmExTxt + " scrollbar-outer scroll-textarea"} id="pos_relative">
                            <div className="scroll-content" id="annotation_scrollContent">
                                <textarea ref={this.refText} name="" id="" cols="30" rows="10" className={ArrowProperty.cssStyles.sprmExTxt + " scrollbar-outer"} defaultValue={this.state.arrowData?.text} onChange={this.onChangeText}></textarea>
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
                <div className={ArrowProperty.cssStyles.sprBot}>
                    <a className={ArrowProperty.cssStyles.clickable} onClick={() => this.onClickApply(true)}>확인</a>
                    <a className={ArrowProperty.cssStyles.clickable} onClick={() => this.onClickApply(false)}>취소</a>
                </div>
            </div>
        );
    }
}

export default ArrowProperty;