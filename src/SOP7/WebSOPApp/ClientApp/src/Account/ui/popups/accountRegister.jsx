import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import $ from 'jquery';

import uis from '../../../Common/css/ui.module.css';
import contents from '../../../Common/css/content.module.css';
import uneCommon from '../../../Common/css/uneCommon.module.css';
import accounts from '../../css/account.module.css';
import imgClose from '../../../Common/image/icon/popup_close_x.png';

import { AccountController } from '../../services/accountController';
import SessionString from '../../../Common/js/sessionString';

import ProjectResource from '../../../Root/resource/id';

class AccountRegister extends Component {

	constructor(props) {
        super(props);

        this.refName = React.createRef();
        this.refRegular = React.createRef();
        this.refPhoneNum1 = React.createRef();
        this.refPhoneNum2 = React.createRef();
        this.refPhoneNum3 = React.createRef();

		this.state = {
            displayAccountUser: null,       // 검색결과 유저 리스트
            selectAccountUser: [],        // 선택결과 유저 리스트
            loginUser: null
		}

        this.props = props;
        this.state.displayAccountUser = this.props.accountUsers;
	}

	componentDidUpdate() {
		//console.log('componentDidUpdate');
	}

	componentWillMount() {
        let user = ProjectResource.getUserInfo();
        if (user !== null && user !== undefined) {
            this.setState({ loginUser: user });
        }

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

    onClickCancle = () => {
        this.props.onClickCancle();
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


    onClickSearch = () => {
        const name = this.refName.current.value.toString().trim();
        const regular = this.refRegular.current.value.toString().trim();
        const phoneNum1 = this.refPhoneNum1.current.value.toString().trim();
        const phoneNum2 = this.refPhoneNum2.current.value.toString().trim();
        const phoneNum3 = this.refPhoneNum3.current.value.toString().trim();

        let accountUsers = this.props.accountUsers;
        let displayUsers = [];

        for (let i = 0; i < accountUsers.length; i++) {
            let user = accountUsers[i];

            if (name !== "" && user.memberName.indexOf(name) === -1)
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

    setSelectUserTable = () => {
        let selectUserTable = [];
        let selectUsers = this.state.selectAccountUser;

        if (selectUsers != null || selectUsers.length != 0) {
            for (let i = 0; i < selectUsers.length; i++) {
                let selectUser = selectUsers[i];

                let regularName = "";
                if (selectUser.regular !== null && selectUser.regular !== undefined) {
                    regularName = selectUser.regular.teamName;
                }

                let accountLevel = "";
                if (selectUser.accountLevel !== null && selectUser.accountLevel !== undefined) {
                    accountLevel = selectUser.accountLevel.levelName;
                }

                selectUserTable.push(
                    <ul>
                        <li className={contents.tableCheck}>
                            <label className={contents.checkboxCssEtc}>
                                <input type="checkbox" id={selectUser.id + "_selectCheck"} className="selectCheck" value={selectUser.id} />
                                <span className={contents.checkmarkEtc}></span>
                            </label>
                        </li>
                        <li>{selectUser.memberID}</li>
                        <li>{selectUser.memberName}</li>
                        <li>{regularName}</li>
                        <li>{accountLevel}</li>
                        <li>{selectUser.phoneNumber}</li>
                    </ul>
                );
            }
        }

        return selectUserTable;
    }

    onChangeCheck = (id) => {

        // 권한이 변경되면 체크
        $('#' + id + '_searchCheck').prop("checked", true);
    }

    setAccountUserTable = () => {
        let accountUserTable = [];
        let accountUsers = this.state.displayAccountUser;

        if (accountUsers != null) {
            for (let i = 0; i < accountUsers.length; i++) {
                let user = accountUsers[i];
                let regularName = "";
                if (user.regular !== null && user.regular !== undefined) {
                    regularName = user.regular.teamName;
                }

                let accountLevelID = "-1";
                if (user.accountLevel !== null && user.accountLevel !== undefined) {
                    accountLevelID = user.accountLevel.id.toString();
                }

                let accountLevelComboBax = [];
                accountLevelComboBax.push(<option key="-1" value="-1"> 선택</option >);

                let accountLevels = this.props.accountLevels;

                if (accountLevels != null) {
                    for (let i = 0; i < accountLevels.length; i++) {
                        let level = accountLevels[i];

                        accountLevelComboBax.push(<option key={level.id} value={level.id}>{level.levelName}</option>);
                    }
                }

                accountUserTable.push(
                    <ul key={user.id}>
                        <li className={contents.tableCheck}>
                            <label className={contents.checkboxCssEtc}>
                                <input type="checkbox" id={user.id + "_searchCheck"} className="searchCheck" value={user.id} />
                                <span className={contents.checkmarkEtc}></span>
                            </label>
                        </li>
                        <li>{user.memberID}</li>
                        <li>{user.memberName}</li>
                        <li>{regularName}</li>
                        <li>
                            <select id={user.id + "_searchLevel"} className={contents.blueSel + " " + contents.btnlmH} onChange={() => this.onChangeCheck(user.id)} defaultValue={accountLevelID}>
                                {accountLevelComboBax}
                            </select>
                        </li>
                        <li>{user.phoneNumber}</li>
                    </ul>
                );
            }
        }

        return accountUserTable;
    }

    onChangeSearchAllChk = (e) => {
        let target = e;
        let checked = target.checked;

        $('.searchCheck').prop("checked", checked);
    }

    onChangeSelectAllChk = (e) => {
        let target = e;
        let checked = target.checked;

        $('.selectCheck').prop("checked", checked);
    }

    onClickRemove = () => {
        let selectUsers = this.state.selectAccountUser;
        let removeUsers = [];
        let newSelectUsers = [];    

        if (selectUsers != null || selectUsers.length !== 0) {
            // 체크된 유저 리스트 만들기
            for (let i = 0; i < selectUsers.length; i++) {
                let user = selectUsers[i];
                let userID = user.id;

                if ($('#' + userID + '_selectCheck').is(":checked") == true) {
                    removeUsers.push(user);
                }
            }

            // 체크된 리스트가 없다면 리턴
            if (removeUsers.length === 0)
                return;

            for (let i = 0; i < selectUsers.length; i++) {
                let user = selectUsers[i];
                let chk = true;

                for (let j = 0; j < removeUsers.length; j++) {
                    let removeUser = removeUsers[j];

                    if (user.id === removeUser.id) {
                        chk = false;
                        break;
                    }
                }

                if (chk === true)
                    newSelectUsers.push(user);
            }

            this.setState({ selectAccountUser: newSelectUsers});
        }

    }

    onClickAdd = () => {
        let accountUsers = this.state.displayAccountUser;
        let selectUsers = [];

        if (accountUsers != null) {
            // 체크된 유저 리스트 만들기
            for (let i = 0; i < accountUsers.length; i++) {
                let user = accountUsers[i];
                let userID = user.id;

                if ($('#' + userID + '_searchCheck').is(":checked") == true) {
                    let levelID = $('#' + userID + "_searchLevel").val();

                    if (levelID !== "-1") {
                        // 부여된 권한 데이터 만들기
                        let accountLevels = this.props.accountLevels;
                        let accountLevel = null;

                        if (accountLevels != null) {
                            for (let i = 0; i < accountLevels.length; i++) {
                                let level = accountLevels[i];

                                if (level.id.toString() === levelID) {
                                    accountLevel = { id: level.id.toString(), levelName: level.levelName };
                                    break;
                                }
                            }
                        }

                        user.accountLevel = accountLevel;
                        selectUsers.push(user);
                    }
                } 
            }

            // 선택된 유저 리스트가 없다면 리턴
            if (selectUsers.length === 0)
                return;

            // 선택된 유저 리스트와 비교 후 없다면 추가
            let selectAccountUser = this.state.selectAccountUser;

            for (let i = 0; i < selectUsers.length; i++) {
                let selectUser = selectUsers[i];
                let chk = true;

                for (let j = 0; j < selectAccountUser.length; j++) {
                    let user = selectAccountUser[j];

                    // 이미 있다면 부여권한 변경
                    if (selectUser.id === user.id) {
                        chk = false;
                        selectAccountUser[j].accountLevel = selectUser.accountLevel;
                    }
                }

                // 없다면 추가
                if (chk === true) {
                    selectAccountUser.push(selectUser);
                }
            }

            this.setState({ selectAccountUser: selectAccountUser});
   
        }
    }

    onClickConfirm = () => {
        let selectUsers = this.state.selectAccountUser;

        if (selectUsers === null || selectUsers.length === 0)
            return;

        this.updateAccountUsers(selectUsers);
    }

    async updateAccountUsers(accountUsers) {
        if (accountUsers === null || accountUsers.length === 0) {
            this.props.onClickConfirm();
        }

        const [result, message] = await AccountController.updateAccountUser(accountUsers, this.state.loginUser.id);

        if (result === null) {
            // 에러 발생
            console.log(message);
        } else {
            // 업데이트 성공
            // 사용자 관리 페이지로 이동
            this.props.onClickConfirm();
        }
    }

    render() {
        let regularComboBax = this.setRegularComboBax();
        let accountUserTable = this.setAccountUserTable();
        let selectUserTable = this.setSelectUserTable();

		return (
			<>
                <div id={contents.popupConts} className="overScrollY">

                    <div className={contents.popupBox} style={{ padding: "25px 30px" }}>
                        <div className={contents.popupBoxTitle}>사용자 등록</div>
                        <div className={contents.popupBoxX}><a onClick={this.onClickClosePopup}><img src={imgClose} alt="닫기" /></a></div>

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
                                        <td><input type="text" ref={this.refName} className={contents.blueInput + " " + contents.w90p} placeholder="이름을 입력하세요." /></td>
                                        <td>&nbsp;</td>
                                        <td>&nbsp;</td>
                                        <td rowSpan="2"><a onClick={this.onClickSearch} className={contents.searchBlueBtn}>검색</a></td>
                                    </tr>
                                    <tr>
                                        <td>・ 부서</td>
                                        <td>
                                            <select ref={this.refRegular} className={contents.blueSel + " " + contents.w90p}>
                                                { regularComboBax }
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


                        <div className={contents.gap30}></div>

                        <div className={uis.floatL}>검색 결과</div>
                        <div className={uis.floatR} >
                            <a onClick={this.onClickAdd} className={contents.lightSkyBtn}>추가</a>
                        </div>

                        <div className={contents.gap10}></div>

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


                            <div className={accounts.registerTableScroll}>


                                <div className={contents.tablebody}>
                                    {accountUserTable}
                                </div>

                            </div>

                        </div>


                        <div className={contents.gap30}></div>

                        <div className={uis.floatL}>선택 결과</div>
                        <div className={uis.floatR}>
                            <a onClick={this.onClickRemove} className={contents.lightNaveBtn}>삭제</a>
                        </div>

                        <div className={contents.gap10}></div>

                        <div className={contents.boxTypeBlue}>

                            <div className={contents.tableHead}>
                                <ul>
                                    <li className={contents.tableCheck}>
                                        <label className={contents.checkboxCssEtc}>
                                            <input type="checkbox" onChange={(e) => this.onChangeSelectAllChk(e.target)} />
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


                            <div className={accounts.registerTableScroll}>


                                <div className={contents.tablebody}>
                                    {selectUserTable}
                                </div>

                            </div>


                        </div>


                        <div className={contents.gap15}></div>

                        <div className={uis.btnArea + " " + uis.alignC}>
                            <a onClick={this.onClickConfirm} className={contents.btnBlue}>확인</a>
                            <a onClick={this.onClickCancle} className={contents.btnNavy}>취소</a>
                        </div>


                    </div>



                </div>
			</>
        );
    }
}

export default withRouter(AccountRegister);