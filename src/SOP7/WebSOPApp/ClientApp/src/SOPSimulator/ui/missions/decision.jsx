import React, { Component } from 'react';
import styles from '../../../Common/css/style.module.css';
import uis from '../../../Common/css/ui.module.css';
import uneStyles from '../../../Common/css/uneCommon.module.css';
import $ from 'jquery';
import SectionData from '../../../Common/models/sections/sectionData';

class Decision extends Component {
    constructor(props) {
        super(props);

        this.state = {
            sopData: null,
            returnValue: 'yes', // Default : Yes로 분기
            prevProps: props
        }

        this.props = props;

        this.runSection = this.runSection.bind(this);
    }

    runSection() {
        this.props.runSection(this.props.sectionData, this.state.returnValue);
    }

    onChangeValue = (e) => {
        if (e.target.value === this.state.returnValue)
            return;

        this.setState({ returnValue: e.target.value });
    }

    componentDidMount() {

        $('html, body').css({ 'display': 'block', 'height': '100%', 'overflow': 'hidden', 'color': '#fff' });

        // Selete Box UI
        $('.' + uis.seleteBox).on('click', '.' + uis.seletedTxt, function () {
            $(this).closest('.' + uis.seleteBox).toggleClass(uis.isShow);
        }).on('click', '.' + "value" , function () {
                var value = $(this).closest('li').data('val');
                $(this).closest('.' + uis.seleteBox).toggleClass(uis.isShow);
                $(this).closest('.' + uis.seleteBox).removeClass('.step01', '.step02', '.step03', '.step04').addClass(value);
                $(this).closest('.' + uis.seleteBox).find('.' + uis.seletedTxt).text($(this).text());
            });
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
        if (!this.props.sectionData.sectionNumber) {
            // sectionNumber가 null일 때
            btnNextClassName = uis.btnDisable;
        }
        else if (this.props.currentSection.componentType === 3 && this.props.currentSection.isBegin) {
            // 현재 SOP가 시작되지 않았을 때 Disable
            btnNextClassName = uis.btnDisable;
        }
        else if (this.props.sectionData.status === SectionData.Status_Done) {
            // 현재 임무가 완료된 상태라면 Disable
            btnNextClassName = uis.btnDisable;
        }

        var returnText = this.state.returnValue;

        return (
            <div className={uis.sectionBox + boxClassName} id={this.props.id}>
                <div className={uis.tit + " " + uis.clfix + " " + uis.hasFire + textClassName}>    
                    <strong>{this.props.sectionData.sectionNumber}.{this.props.sectionData.text}</strong>
                    {
                        /*
                        <div className={uis.seleteBox} onClick={this.onChangeValue}>
                            <button type="button" className={uis.seletedTxt}>{returnText}</button>
                            <ul>
                                <li data-val="yes"><button type="button" className="value">Yes</button></li>
                                <li data-val="no"><button type="button" className="value">No</button></li>
                            </ul>
                        </div>
                        */
                    }

                    <select className={uis.choiceBox} name="" id="" onChange={(e) => this.onChangeValue(e)} >
                        <option value="yes">yes</option>
                        <option value="no">no</option>
                    </select>
                    <div className={uis.btnArea + " " + uneStyles.btnArea}>
                        <a className={btnNextClassName} onClick={this.runSection}>다음</a>
                        {/*<a>전체완료</a>*/}
                    </div>
                </div>
            </div> 
        );
    }
}

export default Decision;