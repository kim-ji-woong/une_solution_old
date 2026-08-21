import React, { Component } from 'react';
import Commands from "../../services/commands";
import $ from 'jquery';
import styles from '../../../Common/css/style.module.css';
import teamEditors from '../../css/teamEditor.module.css';
import TeamEditorResource from '../../resource/id';

class ColText extends Component {
    constructor(props) {
        super(props);
        this.state =
        {
            value: this.props.value,
            columnName: this.props.columnName
        };

        this.props = props;
    }

    onChangeEditMode = (isEditMode) => {
        this.props.onChangeMemberEditMode(this.props.member, this.state.columnName, isEditMode);
    }

    //정규식
    onBlurCheck = (e) => {
        let target = e;

        let isUpdate = true;
        if (this.props.value === target.value) {
            isUpdate = false;
        }

        if (this.state.columnName === TeamEditorResource.ID.colTextMode.phoneNumber) {
            let patternPhone = /01[016789]-[^0][0-9]{2,3}-[0-9]{3,4}/;

            const phoneValid = patternPhone.test(target.value);
            if (!phoneValid && target.value != "") {
                //alert(target.value + "휴대전화번호 형식이 맞지 않습니다.");
                this.props.showConfirmDialog("에러", [target.value + " 휴대전화번호 형식이 맞지 않습니다."], null, null);
                this.setState({ value: "" });
                this.props.member.PhoneNumber = '';
                return;
            }

            // 중복 검사
            if (target.value !== '') {
                if (this.props.checkPhoneNumber(target.value, this.props.member.ID))
                    return;
            }

            this.props.member.PhoneNumber = target.value;
        }
        else if (this.state.columnName === TeamEditorResource.ID.colTextMode.officePhoneNumber) {
            let patternOffice = /02|0[3-9]{1}[0-9]{1}-[0-9]{3,4}-[0-9]{4}/;

            const officeValid = patternOffice.test(target.value);
            if (!officeValid && target.value != "") {
                //alert(target.value + "전화번호 형식이 맞지 않습니다.");
                this.props.showConfirmDialog("에러", [target.value + " 전화번호 형식이 맞지 않습니다."], null, null);
                this.setState({ value: "" });
                this.props.member.OfficePhoneNumber = '';
                return;
            }

            this.props.member.OfficePhoneNumber = target.value;

        }
        else if (this.state.columnName === TeamEditorResource.ID.colTextMode.email) {
            let patternEmail = /^([0-9a-zA-Z_\.-]+)@([0-9a-zA-Z_-]+)(\.[0-9a-zA-Z_-]+){1,2}$/;

            const emailValid = patternEmail.test(target.value);
            if (target.value != "" && !emailValid) {
                //alert(target.value + "이메일 형식이 맞지 않습니다.");
                this.props.showConfirmDialog("에러", [target.value + " 이메일 형식이 맞지 않습니다."], null, null);
                this.setState({ value: "" });
                this.props.member.Email = '';
                return;
            }

            this.props.member.Email = target.value;
        }
        else if (this.state.columnName === TeamEditorResource.ID.colTextMode.memberID) {

            // 중복 검사
            if (target.value !== '') {
                if (this.props.checkMemberID(target.value))
                    return;
            }

            this.props.member.MemberID = target.value;
        }
        else if (this.state.columnName === TeamEditorResource.ID.colTextMode.memberName) {
            this.props.member.MemberName = target.value;
        }
        else if (this.state.columnName === TeamEditorResource.ID.colTextMode.role) {
            this.props.member.role = target.value;
        }
        else if (this.state.columnName === TeamEditorResource.ID.colTextMode.displaySOPName) {
            this.props.member.displaySOPName = target.value;
        }
        else {
            return;
        }

        this.props.onChangeMember(this.props.member, isUpdate);
    }

    onChangeCheck = (e) => {
        let target = e;
        if (this.state.value === target.value) {
            return;
        }

        if (this.state.columnName === TeamEditorResource.ID.colTextMode.phoneNumber ||
            this.state.columnName === TeamEditorResource.ID.colTextMode.officePhoneNumber) {
            // 휴대전화일 경우 숫자 및 자릿수 제한
            const regex = /^[0-9\b -]{0,13}$/;
            if (regex.test(target.value)) {
                let value = target.value;
                let inputValue = value.replace(/-/g, '');

                if (inputValue.length === 4) {
                    // 지역번호 02 경우
                    if (this.state.columnName === TeamEditorResource.ID.colTextMode.officePhoneNumber &&
                        inputValue.indexOf('02') === 0) {
                        inputValue = inputValue.replace(/(\d{2})(\d{2})/, '$1-$2');
                    } else {
                        inputValue = inputValue.replace(/(\d{3})(\d{1})/, '$1-$2');
                    }
                } else if (inputValue.length === 8) {
                    if (this.state.columnName === TeamEditorResource.ID.colTextMode.officePhoneNumber &&
                        inputValue.indexOf('02') === 0) {
                        // 지역번호 02 경우
                        inputValue = inputValue.replace(/(\d{2})(\d{3})(\d{3})/, '$1-$2-$3');
                    } else 
                        inputValue = inputValue.replace(/(\d{3})(\d{4})(\d{1})/, '$1-$2-$3');
                } else if (inputValue.length === 10) {
                    if (this.state.columnName === TeamEditorResource.ID.colTextMode.officePhoneNumber &&
                        inputValue.indexOf('02') === 0) {
                        // 지역번호 02 경우
                        inputValue = inputValue.replace(/(\d{2})(\d{4})(\d{4})/, '$1-$2-$3');
                    } else
                        inputValue = inputValue.replace(/(\d{3})(\d{3})(\d{4})/, '$1-$2-$3');
                } else if (inputValue.length === 11 &&
                    !(this.state.columnName === TeamEditorResource.ID.colTextMode.officePhoneNumber && inputValue.indexOf('02') === 0)) {
                    inputValue = inputValue.replace(/(\d{3})(\d{4})(\d{4})/, '$1-$2-$3');
                } else {
                    inputValue = value;
                }

                this.setState({ value: inputValue });
            }
        }
        else {
            this.setState({ value: target.value });
        } 

       return;
    }

    handleKeyPress = (e) => {
        if (e.key === "Enter") {
            //this.onBlurCheck(e.target);
            e.target.blur();
        }
        else if (e.key === 'Escape') {
            this.setState({ value: this.props.value });
            this.onChangeEditMode(false);
        }
    }

    render() {
        let colID = this.state.columnName + this.props.member.ID;
        let value = this.state.value;
        let placeholder = '';
        if (this.state.columnName === TeamEditorResource.ID.colTextMode.memberName ||
            this.state.columnName === TeamEditorResource.ID.colTextMode.displaySOPName) {
            if (value === '새 인원') {
                value = '';
                placeholder = '새 인원';
            }
        }

        return (
            this.props.isEditMode && this.state.columnName === this.props.editColumnName ?
                <td>  
                    <input 
                        type="text"
                        id={'input_' + colID}
                        //autoFocus={true}
                        onChange={(e) => this.onChangeCheck(e.target)}
                        onBlur={(e) => this.onBlurCheck(e.target)}
                        onKeyPress={this.handleKeyPress}
                        value={value || ''}
                        placeholder={placeholder}                        
                    />
                </td >
                :
                <td id={'td_' + colID} onMouseDown={() => this.onChangeEditMode(true)}>
                    <span className={styles.fixation + " " + teamEditors.colTextSpan}>{this.state.value}</span>
                </td>
            );
    }
}

export default ColText;