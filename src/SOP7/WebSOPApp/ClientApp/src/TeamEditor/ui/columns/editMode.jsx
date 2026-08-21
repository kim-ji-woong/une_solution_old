import React, { Component } from 'react';
import Commands from "../../services/commands";
import $ from 'jquery';
import uneStyles from '../../../Common/css/uneCommon.module.css';

class editMode extends Component {
    constructor(props) {
        super(props);
        this.state =
        {
            value: null,
            isEditMode: false
        };

        this.props = props;
        this.state.value = this.props.value;
        this.state.isEditMode = this.props.isEditMode;
    }

    componentDidMount() {
        this.hi();
        //this.focuss();
    }

    //정규식
    onBlurCheck = (e) => {
        let target = e;

        if (this.props.validationCheck === 1) // phoneNumber
        //if(this.props.member.PhoneNumber)
        {
            var re = /^\d{3}\d{3,4}\d{4}$/;

            const phoneValid = re.test(target.value);
            if (!phoneValid) {
                //alert('핸드폰 형식을 확인하세요.');
                this.setState({ value: '' });
                return;
            }
        }

        this.props.onChange(target.value); // 포커스 잃을 경우에만 부모에게 값을 전달
        return;
    }

    onChangeCheck = (e) => {

        let target = e;
        this.setState({ value: target.value });
        //this.props.onChange(target.value);        

        return;
    }


    handleKeyPress = (e) => {
        if (e.key === "Enter") {
            this.onBlurCheck(e.target);
        }
    }

    //하이픈
    hi() {

        $(document).ready(function () {
            $(function () {

                $('#' + 'mobileNo').keydown(function (event) {
                    var key = event.charCode || event.keyCode || 0;
                    var text = $(this);
                    if (key !== 8 && key !== 9) {
                        if (text.val().length === 3) {
                            text.val(text.val() + '-');
                        }
                        if (text.val().length === 8) {
                            text.val(text.val() + '-');
                        }
                    }
                    return (key == 8 || key == 9 || key == 46 || (key >= 48 && key <= 57) || (key >= 96 && key <= 105));
                })
            });

        });
    }

    //input 비활성화
    /*    textDis() {
            $(function () {
                $('#' + 'inputText').on('input', function () {
                    if ($('#' + 'inputText').val() == '') {
                        $('#' + 'testBtn').attr('disabled', true);
                    } else {
                        $('#' + 'testBtn').attr('disabled', false);
                    }
                });
            });
        }*/


    //enter치면 반응
    handleKeyPressTT = (e) => {
        if (e.key === 'Enter') {
            this.handleCreate();
        }
        return;
    }

    handleCreate() {
        //$('.' + uneStyles.handleKeyPressTT).focus();

        //$('.' + uneStyles.handleKeyPressTT).css('border', 'none');
    }

/*    focuss() {
        //$('.' + uneStyles.handleKeyPressTT).addClass('input');
        $('.' + uneStyles.handleKeyPressTT).focus();
    }*/


    render() {
        //console.log(this.state.value);
        return (
            <td>
                {
                    (this.state.isEditMode)
                        ?
                        <input type="text"
                            id={this.state.id}
                            onChange={(e) => this.onChangeCheck(e.target)}
                            onBlur={(e) => this.onBlurCheck(e.target)}
                            onKeyPress={this.handleKeyPress}
                            value={this.state.value || ''}
                            className={uneStyles.handleKeyPressTT}
                            onKeyPress={(e) => this.handleKeyPressTT(e)} //enter 이후 이벤트 발생 //이거 2개임
                            name="mobileNo"
                            id="mobileNo" //id 중복
                            maxLength="13"
                        />
                        :
                        <span>{this.state.value}</span>

                }
            </td>
        );
    }
}

export default editMode;