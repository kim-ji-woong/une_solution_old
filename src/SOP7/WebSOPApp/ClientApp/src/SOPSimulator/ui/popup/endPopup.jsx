import React, { Component } from 'react';
import $ from 'jquery';
import uis from '../../../Common/css/ui.module.css';
import uneStyles from '../../../Common/css/uneCommon.module.css';

class EndPopup extends Component {
    constructor(props) {
        super(props);
        this.state = {

        }

        this.props = props;
    }

    componentDidMount() {

        this.timerHandle = setTimeout(() => {
            this.onClose();
        }, 10000);
    }

    onClose = () => {
        if (this.timerHandle) {
            clearTimeout(this.timerHandle);
        }
        const history = this.props.sopRunData.sopData.currentActionStep._ActionStepHistory;
        this.props.closeSOP(null, history.id, history.endTime, this.props.loginUser.id);
        this.props.changeContent('');
    }

    onClickClose = () => {
        this.onClose();
    }

    componentWillUnmount = () => {
        if (this.timerHandle) {
            clearTimeout(this.timerHandle);
        }
    }

    render() {
        const sopRunData = this.props.sopRunData;
        return (
            <div className={uneStyles.endBox}>
                <div className={uneStyles.endBoxTop}>
                    <p>SOP 결과요약</p>
                    <a onClick={() => this.onClickClose()}>닫기</a>
                </div>
                <div className={uneStyles.endBoxCont}>
                    <dl>
                        <dt>1.SOP 유형</dt>
                        <dd>-{sopRunData.sopData.disaster.disasterName}</dd>
                        <dt>2.재난 위치</dt>
                        <dd>-{sopRunData.position}</dd>
                        <dt>3.발생시간</dt>
                        <dd>-{sopRunData.sopData.currentActionStep._ActionStepHistory.beginTime}</dd>
                        <dt>4.SOP 시작시간</dt>
                        <dd>-{sopRunData.sopData.currentActionStep._ActionStepHistory.beginTime}</dd>
                        <dt>5.SOP 종료시간</dt>
                        <dd>-{sopRunData.sopData.currentActionStep._ActionStepHistory.endTime}</dd>
                        <dt>6.단계</dt>
                        <dd>-{sopRunData.sopData.currentActionStep.stepName}</dd>
                    </dl>
                </div>
                <div className={uneStyles.endBoxBtn}>
                    <a className={uneStyles.endBoxClose} onClick={() => this.onClickClose()}>닫기</a>
                    {/*<a className={uneStyles.endBoxDetail}>상세보기</a>*/}
                </div>
            </div>
        );
    }
}

export default EndPopup;