import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import $ from 'jquery';

import uis from '../../../Common/css/ui.module.css';
import contents from '../../../Common/css/content.module.css';
import uneCommon from '../../../Common/css/uneCommon.module.css';
import accounts from '../../css/account.module.css';

import imgClose from '../../../Common/image/icon/popup_close_x.png';

import { AccountController } from '../../services/accountController';
import { TeamEditController } from '../../../TeamEditor/services/teamEditController';

import AcoountResource from '../../resource/id';


class AccountManager extends Component {

	constructor(props) {
		super(props);

        this.refName = React.createRef();
        this.refLevel = React.createRef();
        this.refRegular = React.createRef();
        this.refPhoneNum1 = React.createRef();
        this.refPhoneNum2 = React.createRef();
        this.refPhoneNum3 = React.createRef();

		this.state = {
            displayAccountUser: null,
            removeAccountUsers: [],
            mode: AcoountResource.ID.popupMode.manager,
		}

        this.props = props;
        this.state.displayAccountUser = this.props.accountUsers;
    }

	componentDidUpdate(prevProps, prevState) {
        //console.log('componentDidUpdate');

        if (prevProps.accountUsers !== this.props.accountUsers) {
            this.onClickSearch();
        }
	}

	componentWillMount() {
		//console.log('componentWillMount');
	}

	componentWillUpdate(nextProps, nextState) {
		//console.log('componentWillUpdate');
	}

	componentDidMount() {
		//console.log('componentDidMount');
	}

    onClickClosePopup = () => {
        this.props.onClickClosePopup(false);
    }

    onClickRegister = () => {
        if (this.state.mode === AcoountResource.ID.popupMode.report) {
            // 삭제이력 모드일 경우 재등록 버튼 기능
            let accountUsers = this.state.displayAccountUser;
            let reRegisterUsers = [];
            //let newSelectUsers = [];

            if (accountUsers != null || accountUsers.length !== 0) {
                // 체크된 유저 리스트 만들기
                for (let i = 0; i < accountUsers.length; i++) {
                    let user = accountUsers[i];
                    let userID = user.id;

                    if ($('#' + userID + '_searchCheck').is(":checked") == true) {
                        reRegisterUsers.push(user);
                    }
                }
            }

            if (reRegisterUsers.length === 0)
                return;
            else {
                this.reRegisterAccountUsers(reRegisterUsers);
                return;
            }
                
        }

        this.props.onClickRegister();
    }

    async reRegisterAccountUsers(accountUsers) {
        if (accountUsers === null || accountUsers.length === 0)
            return;

        const [result, message] = await AccountController.reRegisterAccountUsers(accountUsers);

        if (result === null) {
            // 에러 발생
            console.log(message);
            return;
        } else {
            // 재등록 성공
            let removeAccountUsers = this.state.removeAccountUsers;
            let newAccountUsers = [];

            for (let i = 0; i < removeAccountUsers.length; i++) {
                let removeUser = removeAccountUsers[i];
                let chk = false;

                for (let j = 0; j < accountUsers.length; j++) {
                    let user = accountUsers[j];
                    

                    if (removeUser.id === user.id) {
                        chk = true;
                        break;
                    }
                }

                if (chk === false)
                    newAccountUsers.push(removeUser);
            }

            // 다시 불러오기
            this.props.onChangeReload();

            // 새로운 삭제인원으로 표시
            this.state.removeAccountUsers = newAccountUsers;
            this.onClickSearch();
        }
    }

    onChangeSearchAllChk = (e) => {
        let target = e;
        let checked = target.checked;

        $('.searchCheck').prop("checked", checked);
    }

    setRegularComboBax = () => {
        let regularComboBax = [];
        regularComboBax.push(<option key="-1" value="-1">전체</option>);

        let regulars = this.props.regulars;

        if (regulars != null) {
            for (let i = 0; i < regulars.length; i++) {
                let regular = regulars[i];

                regularComboBax.push(<option key={regular.id} value={regular.id}>{regular.teamName}</option>);
            }
        }

        return regularComboBax;
    }

    setAccountLevelsComboBax = () => {
        let accountLevelComboBax = [];
        accountLevelComboBax.push(<option key="-1" value="-1"> 전체</option >);

        let accountLevels = this.props.accountLevels;

        if (accountLevels != null) {
            for (let i = 0; i < accountLevels.length; i++) {
                let level = accountLevels[i];

                if (level === null || level === undefined)
                    continue;

                accountLevelComboBax.push(<option key={level.id} value={level.id}>{level.levelName}</option>);
            }
        }

        return accountLevelComboBax;
    }

    setAccountUserTable = () => {
        let accountUserTable = [];
        let accountUserCount = 0;

        let accountUsers = this.state.displayAccountUser;

        if (accountUsers != null) {
            for (let i = 0; i < accountUsers.length; i++) {
                let user = accountUsers[i];
                let regularName = "";
                if (user.regular !== null && user.regular !== undefined) {
                    regularName = user.regular.teamName;
                }

                let accountLevelName = "";
                if (user.accountLevel !== null && user.accountLevel !== undefined) {
                    accountLevelName = user.accountLevel.levelName;
                }

                accountUserTable.push(
                    <ul key={user.id}>
                        <li className={contents.tableCheck}>
                            <label className={contents.checkboxCssEtc}>
                                <input type="checkbox" id={user.id + "_searchCheck"} className="searchCheck" />
                                <span className={contents.checkmarkEtc}></span>
                            </label>
                        </li>
                        <li>{user.memberID}</li>
                        <li>{user.memberName}</li>
                        <li>{regularName}</li>
                        <li>{accountLevelName}</li>
                        <li>{user.phoneNumber}</li>
                    </ul>
                );

                accountUserCount = accountUserCount + 1;
            }
        }

        return [accountUserCount, accountUserTable];
    }

    onClickSearch = () => {
        const name = this.refName.current.value.toString().trim();
        const level = this.refLevel.current.value.toString().trim();
        const regular = this.refRegular.current.value.toString().trim();
        const phoneNum1 = this.refPhoneNum1.current.value.toString().trim();
        const phoneNum2 = this.refPhoneNum2.current.value.toString().trim();
        const phoneNum3 = this.refPhoneNum3.current.value.toString().trim();

        let accountUsers = [];
        let displayUsers = [];

        if (this.state.mode === AcoountResource.ID.popupMode.report) {
            accountUsers = this.state.removeAccountUsers;
        } else {
            accountUsers = this.props.accountUsers;
        }

        for (let i = 0; i < accountUsers.length; i++) {
            let user = accountUsers[i];

            if (name !== "" && user.memberName.indexOf(name) === -1)
                continue;

            if (level !== "-1" && (user.accountLevel === null || (user.accountLevel !== null && user.accountLevel.id.toString() !== level)))
                continue;

            if (regular !== "-1" && (user.regular === null || (user.regular !== null && user.regular.id.toString() !== regular)))
                continue;


            if (phoneNum1 !== "-1" && user.phoneNumber.indexOf(phoneNum1) !== 0)
                continue;

            if (phoneNum2 !== "" && user.phoneNumber.indexOf(phoneNum2) === -1)
                continue;

            if (phoneNum3 !== "" && user.phoneNumber.indexOf(phoneNum3) === -1)
                continue;

            displayUsers.push(user);
        }

        this.setState({ displayAccountUser: displayUsers });
    }

    onClickReport = () => {
        // 삭제이력 버튼 감추기
        $('#btnReport').hide();

        // 삭제이력 모드 변경
        //this.setState({ mode: AcoountResource.ID.popupMode.report });
        this.state.mode = AcoountResource.ID.popupMode.report;

        // 삭제 인원으로 검색
        this.onClickSearch();
    }

    onClickRemove = () => {
        if (this.state.mode === AcoountResource.ID.popupMode.report) {
            // 삭제이력 모드일 경우 이전 버튼 기능

            // 삭제이력 버튼 표시
            $('#btnReport').show();

            // 다시 표시
            this.state.mode = AcoountResource.ID.popupMode.manager;
            this.onClickSearch();

            return;
        }

        let accountUsers = this.state.displayAccountUser;
        let removeUsers = [];
        let newSelectUsers = [];

        if (accountUsers != null || accountUsers.length !== 0) {
            // 체크된 유저 리스트 만들기
            for (let i = 0; i < accountUsers.length; i++) {
                let user = accountUsers[i];
                let userID = user.id;

                if (user.accountID === -1)
                    continue;

                if ($('#' + userID + '_searchCheck').is(":checked") == true) {
                    removeUsers.push(user);
                }
            }
        }

        if (removeUsers.length === 0)
            return;
        else 
            this.removeAccountUsers(removeUsers);
    }

    async removeAccountUsers(accountUsers) {
        if (accountUsers === null || accountUsers.length === 0)
            return;

        const [result, message] = await AccountController.removeAccountUsers(accountUsers);

        if (result === null) {
            // 에러 발생
            console.log(message);
            return;
        } else {
            // 삭제 성공
            // 다시 불러오기
            this.props.onChangeReload();
            this.setState({ removeAccountUsers: accountUsers });
        }
    }

    setButtonText = () => {
        let btnRegisterText = AcoountResource.ID.textRegister;
        let btnRemoveText = AcoountResource.ID.textRemove;

        if (this.state.mode === AcoountResource.ID.popupMode.report) {
            btnRegisterText = AcoountResource.ID.textReRegister;
            btnRemoveText = AcoountResource.ID.textBefore;
        }

        return [btnRegisterText, btnRemoveText];
    }

    render() {
        let regularComboBax = this.setRegularComboBax();
        let accountLevelComboBax = this.setAccountLevelsComboBax();
        let [accountUserCount, accountUserTable] = this.setAccountUserTable();
        let [btnRegisterText, btnRemoveText] = this.setButtonText();

		return (
			<>
                <div id={contents.popupConts} className={contents.loginPopup}>

                    <div className={contents.popupBox}>
                        <div className={contents.popupBoxTitle}>{this.state.mode}</div>
                        <div className={contents.popupBoxX}><a className={uneCommon.pointCursor} onClick={this.onClickClosePopup}><img src={imgClose} alt="닫기" /></a></div>

                        <div className={contents.boxTypeBlue}>
                            <table className={contents.tblNone}>
                                <caption>게시판입니다.</caption>
                                <colgroup>
                                    <col style={{ width: "90px" }} />
                                    <col style={{ width: "*" }} />
                                    <col style={{ width: "90px" }} />
                                    <col style={{ width: "*" }} />
                                    <col style={{ width: "110px" }} />
                                </colgroup>
                                <tbody>
                                    <tr>
                                        <td>・ 이름</td>
                                        <td><input ref={this.refName} type="text" className={contents.blueInput + " " + contents.w90p} placeholder="이름을 입력하세요." /></td>
                                        <td>・ 권한</td>
                                        <td>
                                            <select ref={this.refLevel} className={contents.blueSel + " " + contents.w100p}>
                                                {accountLevelComboBax}
                                            </select>
                                        </td>
                                        <td rowSpan="2"><a onClick={this.onClickSearch} className={contents.searchBlueBtn}>검색</a></td>
                                    </tr>
                                    <tr>
                                        <td>・ 부서</td>
                                        <td>
                                            <select ref={this.refRegular} className={contents.blueSel + " " + contents.w90p}>
                                                {regularComboBax}
                                            </select>
                                        </td>
                                        <td>・ 연락처</td>
                                        <td>
                                            <ul className={contents.tel3col}>
                                                <li>
                                                    <select ref={this.refPhoneNum1} className={contents.blueSel}>
                                                        <option value="-1">선택</option>
                                                        <option value="010">010</option>
                                                        <option value="011">011</option>
                                                        <option value="016">016</option>
                                                        <option value="017">017</option>
                                                        <option value="018">018</option>
                                                        <option value="019">019</option>
                                                    </select><span>-</span>
                                                </li>
                                                <li>
                                                    <input type="text" ref={this.refPhoneNum2} className={contents.blueInput + " " + contents.w100p} />
                                                </li>
                                                <li>
                                                    <input type="text" ref={this.refPhoneNum3} className={contents.blueInput + " " + contents.w100p} />
                                                </li>
                                            </ul>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>

                        </div>


                        <div className={contents.gap20}></div>

                        <div className={uis.floatL}>검색결과 : 총 {accountUserCount}명</div>
                        <div className={uis.floatR}>
                            <a onClick={this.onClickReport} id="btnReport" className={contents.darkNaveBtn}>{AcoountResource.ID.popupMode.report}</a>
                            <a onClick={this.onClickRemove} className={contents.lightNaveBtn}>{btnRemoveText}</a>
                            <a onClick={this.onClickRegister} className={contents.lightBlueBtn}>{btnRegisterText}</a>
                        </div>

                        <div className={contents.gap20}></div>

                        <div className={contents.boxTypeBlue}>

                            <div className={contents.tableHead}>
                                <ul>
                                    <li className={contents.tableCheck}>
                                        <label className={contents.checkboxCssEtc}>
                                            <input type="checkbox" onChange={(e) => this.onChangeSearchAllChk(e.target)} />
                                            <span className={contents.checkmarkEtc}></span>
                                        </label>
                                    </li>
                                    <li>ID</li>
                                    <li>이름</li>
                                    <li>부서</li>
                                    <li>부여권한</li>
                                    <li>연락처</li>
                                </ul>
                            </div>

                            <div className={accounts.tableScroll}>


                                <div className={contents.tablebody}>

                                    {accountUserTable}
                               
                                </div>

                            </div>

            
                            
                

                        </div>

                    </div>


                </div>
			</>
        );
    }
}

export default withRouter(AccountManager);