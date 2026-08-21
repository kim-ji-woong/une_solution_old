import React, { Component } from 'react';

import $ from 'jquery';
import styles from '../../../Common/css/style.module.css';
import uneStyles from '../../../Common/css/uneCommon.module.css';
import TeamEditorResource from '../../resource/id';

class ColComboBox extends Component {
    constructor(props) {
        super(props);
        this.state =
        {
            value: this.props.value,     // 선택된 값
            options: this.props.options, // 콤보박스 리스트
            isClickChk: false,           // 더블클릭 체크값
            columnName: this.props.columnName
        };

        this.props = props;

        if (this.state.value == null) {
            this.state.value = "";
        }
    }

    componentDidUpdate(prevProps, prevState) {
        if (prevProps.options !== this.props.options) {
            this.setState({ options: this.props.options });
        }
    }

    onChangeEditMode = (isEditMode) => {
        this.props.onChangeMemberEditMode(this.props.member, this.state.columnName, isEditMode);
    }

    onChangeCheck = (e) => {
        let val = Number(e.target.value);
        this.setState({ value: val });

        let isUpdate = true;
        if (this.props.value === val) {
            isUpdate = false;
        }

        if (this.state.columnName === TeamEditorResource.ID.colTextMode.jobLevel) {
            this.props.member.JobLevelID = val;
        }
        else if (this.state.columnName === TeamEditorResource.ID.colTextMode.jobPosition) {
            this.props.member.JobPositionID = val;
        }
        else if (this.state.columnName === TeamEditorResource.ID.colTextMode.role) {
            this.props.member.role = val;
        }
        else {
            return;
        }

        this.props.onChangeMember(this.props.member, isUpdate);
    }

    render() {
        var strName = null;
        if (this.props.options !== null) {
            for (var i = 0; i < this.props.options.length; i++) {
                if (this.props.options[i].value === this.props.value) {
                    strName = this.props.options[i].name;
                    break;
                }
            }
        }

        return (
            this.props.isEditMode && this.state.columnName === this.props.editColumnName ?
                <td>
                    <select onChange={(e) => this.onChangeCheck(e)} defaultValue={this.props.value} autoFocus className={uneStyles.selectCombo}>
                    {
                        this.props.options.map((level, index) =>
                        (
                            <option key={level.value} value={level.value}>{level.name}</option>
                        ))
                    }
                    </select>
                </td>
                :
                <td onMouseDown={() => this.onChangeEditMode(true)}>
                    <span className={styles.fixation}>{strName}</span>
                </td>
            );
    }
}

export default ColComboBox;