import React, { Component } from 'react';
import { AccountController } from '../Account/services/accountController';
import { ConfirmDialog } from '../Root/confirmDialog';
import styles from './css/member.module.css';

export class WaitingUsers extends Component {
    static NormalType = 1;
    static DeveloperType = 2;
    static AdminType = 3;

    constructor(props) {
        super(props);
        this.state =
        {
            prevProps: props,
            prevInstance: this,
            selectedCount: 0,
            userTypes: {},
            confirmMessages: null,
            response: null,
            processing: false
        };

        this.refTHead = React.createRef();
        this.refTBody = React.createRef();
        this.refBtnPermit = React.createRef();
        this.refBtnDeny = React.createRef();
        this.refConfirm = React.createRef();
    }

    componentDidMount() {
        this.updateUsers(this.props, true);
        /*if (this.props.users) {
            const userTypes = {};
            const userCount = this.props.users.length;

            for (let i = 0; i < userCount; i++) {
                const user = this.props.users[i];

                if (user.isDeveloper) {
                    userTypes[user.id] = WaitingUsers.DeveloperType;
                }
                else {
                    userTypes[user.id] = WaitingUsers.NormalType;
                }
            }

            this.setState({ userTypes, response: null, processing: false, selectedCount: 0 });
        }
        else {
            this.setState({ response: null, processing: false, selectedCount: 0 });
        }*/
    }

    static getDerivedStateFromProps(props, state) {
        if (props === state.prevProps) {
            return state;
        }

        if (WaitingUsers.isEmpty(state.userTypes)) {
            const userTypes = state.prevInstance.updateUsers(props, false);

            return {
                prevProps: props,
                prevInstance: state.prevInstance,
                selectedCount: 0,
                userTypes: userTypes,
                confirmMessages: null,
                response: null,
                processing: false
            };
        }

        return {
            prevProps: props,
            prevInstance: state.prevInstance,
            selectedCount: state.selectedCount,
            userTypes: state.userTypes,
            confirmMessages: state.confirmMessages,
            response: state.response,
            processing: state.processing
        };
    }

    static isEmpty(obj) {
        for (const key in obj) {
            return false;
        }

        return true;
    }

    updateUsers(props, update) {
        const userTypes = {};

        if (props.users) {
            const userCount = props.users.length;

            for (let i = 0; i < userCount; i++) {
                const user = props.users[i];

                if (user.isDeveloper) {
                    userTypes[user.id] = WaitingUsers.DeveloperType;
                }
                else {
                    userTypes[user.id] = WaitingUsers.NormalType;
                }
            }

            if (update) {
                this.setState({ userTypes, response: null, processing: false, selectedCount: 0 });
            }
        }
        else {
            if (update) {
                this.setState({ response: null, processing: false, selectedCount: 0 });
            }
        }

        return userTypes;
    }

    getUserType(id) {
        return this.state.userTypes[id];
    }

    getRowContent() {
        const rowContent = [];

        if (!this.props.users) {
            return rowContent;
        }

        const users = this.props.users;

        users.map(user => {
            rowContent.push(
                <tr key={"rowBody_" + user.id} data-userID={user.id}>
                    <td>
                        <input type="checkbox" onChange={(e) => this.onChangeCheckRow(e.target)} />
                    </td>
                    <td className={styles.thickTD}>{user.name}</td>
                    <td className={styles.thickTD}>{user.email}</td>
                    <td className={styles.thickTD}>{user.phoneNumber}</td>
                    <td className={styles.thickTD}>{WaitingUsers.getCreateTime(user.createTime)}</td>
                    <td>
                        <input type="radio" checked={this.getUserType(user.id) === WaitingUsers.NormalType} onChange={(e) => this.onChangeCheckUserType(e.target, WaitingUsers.NormalType)} />
                    </td>
                    <td>
                        <input type="radio" checked={this.getUserType(user.id) === WaitingUsers.DeveloperType} onChange={(e) => this.onChangeCheckUserType(e.target, WaitingUsers.DeveloperType)} />
                    </td>
                </tr>
            );
        });

        return rowContent;
    }

    static getCreateTime(time) {
        const index = time.indexOf('T');

        if (index < 0) {
            return time;
        }

        const dayString = time.substring(0, index);
        const index1 = dayString.indexOf('-');
        const index2 = dayString.lastIndexOf('-');

        if (index1 < 0 || index2 <= index1) {
            return time;
        }

        const year = dayString.substring(0, index1);
        const month = dayString.substring(index1 + 1, index2);
        const day = dayString.substring(index2 + 1);

        const timeString = time.substring(index + 1);
        const index3 = timeString.indexOf(':');
        const index4 = timeString.lastIndexOf(':');

        if (index3 < 0 || index4 <= index3) {
            return time;
        }

        const hour = timeString.substring(0, index3);
        const minute = timeString.substring(index3 + 1, index4);

        return `${year}년 ${month}월 ${day}일 ${hour}시 ${minute}분`;
    }

    onChangeCheckUserType = (target, userType) => {
        const userID = target.parentElement?.parentElement?.dataset?.userid;

        if (userID) {
            const userTypes = { ...this.state.userTypes };
            const oldType = userTypes[userID];

            if (oldType !== userType) {
                userTypes[userID] = userType;
                this.setState({ userTypes });
            }
        }
    }

    onChangeCheckRow = (target) => {
        if (target.checked) {
            this.setState({ selectedCount: this.state.selectedCount + 1 });
        }
        else {
            if (this.state.selectedCount > 0) {
                this.setState({ selectedCount: this.state.selectedCount - 1 });
            }
        }
    }

    onChangeCheckHeader = (target) => {
        if (!this.refTBody.current) {
            return;
        }

        const children = this.refTBody.current.children;
        const childCount = children.length;
        const checked = target.checked;

        for (let i = 0; i < childCount; i++) {
            const child = children[i];

            if (child.tagName === "TR") {
                if (child.children.length > 0) {
                    const td = child.children[0];

                    if (td.tagName !== "TD") {
                        continue;
                    }

                    if (td.children.length > 0) {
                        const input = td.children[0];

                        if (input.tagName !== "INPUT") {
                            continue;
                        }

                        input.checked = checked;
                    }
                }
            }
        }

        if (checked) {
            this.setState({ selectedCount: childCount });
        }
        else {
            this.setState({ selectedCount: 0 });
        }
    }

    onClickResponse(permit) {
        this.showConfirmBox(permit);
    }

    async showConfirmBox(permit) {
        const selectedCount = this.state.selectedCount;

        if (selectedCount === 0) {
            return;
        }

        if (this.refConfirm.current.classList.contains(styles.show) === false) {
            this.refConfirm.current.classList.add(styles.show);
        }

        const messages = [];

        if (permit) {
            messages.push(`선택된 ${selectedCount}건의 요청을 승인하시겠습니까?`);
        }
        else {
            messages.push(`선택된 ${selectedCount}건의 요청을 거절하시겠습니까?`);
        }

        this.refConfirm.current.style.height = ConfirmDialog.getHeight(messages.length);
        this.setState({ confirmMessages: messages, response: permit });
    }

    onClickConfirm = (result) => {
        const yes = ConfirmDialog.getResultYes();

        if (result === yes) {
            this.processResponse(this.state.response);
        }
        else {
            this.clearCheckBox();
        }

        if (this.refConfirm.current.classList.contains(styles.show)) {
            this.refConfirm.current.classList.remove(styles.show);
        }

        this.setState({ confirmMessages: null });
    }

    processResponse(response) {
        if (response !== null) {
            if (!this.refTBody.current) {
                return;
            }

            const children = this.refTBody.current.children;
            const childCount = children.length;

            const userIDs = [];

            for (let i = 0; i < childCount; i++) {
                const child = children[i];

                if (child.tagName === "TR") {
                    if (child.children.length > 0) {
                        const td = child.children[0];

                        if (td.tagName !== "TD") {
                            continue;
                        }

                        if (td.children.length > 0) {
                            const input = td.children[0];

                            if (input.tagName !== "INPUT") {
                                continue;
                            }

                            if (input.checked) {
                                userIDs.push(parseInt(child.dataset.userid));
                            }
                        }
                    }
                }
            }

            if (userIDs.length > 0) {
                this.requestUpdateUserTypes(userIDs, response);
                this.clearCheckBox();
            }
        }
    }

    requestUpdateUserTypes(userIDs, response) {
        const userCount = userIDs.length;
        const userTypes = {};

        for (let i = 0; i < userCount; i++) {
            const userType = this.getUserType(userIDs[i]);
            const userTypeData = {
                id: userIDs[i],
                isNormalUser: false, 
                isDeveloper: false,
                isAdmin: false
            }

            if (userType) {
                if (userType === WaitingUsers.NormalType) {
                    userTypeData.isNormalUser = true;
                }
                else if (userType === WaitingUsers.DeveloperType) {
                    userTypeData.isDeveloper = true;
                }
                else if (userType === WaitingUsers.AdminType) {
                    userTypeData.isAdmin = true;
                }
            }

            userTypes[userIDs[i]] = userTypeData;
        }

        this.setState({ processing: true });
        this.props.updateRequestUsers(userTypes, response, null);
    }

    clearCheckBox() {
        const tbody = this.refTBody.current;
        const thead = this.refTHead.current;

        if (!tbody || !thead) {
            return;
        }

        if (thead.children.length === 0) {
            return;
        }

        const headTR = thead.children[0];

        if (headTR.tagName === "TR") {
            const trChildCount = headTR.children.length;
            let checked = false;

            for (let i = 0; i < trChildCount; i++) {
                const th = headTR.children[i];

                if (th.tagName === "TH") {
                    const thChildCount = th.children.length;

                    for (let j = 0; j < thChildCount; j++) {
                        const input = th.children[j];

                        if (input.tagName === "INPUT") {
                            input.checked = false;
                            checked = true;
                            break;
                        }
                    }
                }

                if (checked) {
                    break;
                }
            }
        }

        const childCount = tbody.children.length;

        for (let i = 0; i < childCount; i++) {
            const tr = tbody.children[i];

            if (tr.tagName === "TR") {
                const trChildCount = tr.children.length;
                let checked = false;

                for (let j = 0; j < trChildCount; j++) {
                    const td = tr.children[j];

                    if (td.tagName === "TD") {
                        const tdChildCount = td.children.length;

                        for (let k = 0; k < tdChildCount; k++) {
                            const input = td.children[k];

                            if (input.tagName === "INPUT") {
                                input.checked = false;
                                checked = true;
                                break;
                            }
                        }
                    }

                    if (checked) {
                        break;
                    }
                }
            }
        }
    }

    render() {
        const rowContent = this.getRowContent();
        const btnClassName = this.state.selectedCount === 0 || this.state.processing ? styles.btnRequest + " " + styles.disabled : styles.btnRequest;

        const confirmOption = ConfirmDialog.getYesNo();

        return (
            <div className={styles.bodyArea}>
                <div className={styles.titleBox}>
                    <h4 className={styles.textTitle}>계정승인 대기자 목록</h4>
                    <hr></hr>
                    <div>
                        <div className={styles.textLeft}>계정생성후 승인을 기다리는 사용자들입니다.</div>
                        <div className={styles.textLeft}>승인 또는 거부할 수 있으며, 승인시 계정등급을 결정할 수 있습니다.</div>
                    </div>
                </div>
                <table className={styles.stripedTable} aria-labelledby="tabelLabel">
                    <thead ref={this.refTHead}>
                        <tr>
                            <th>
                                <input type="checkbox" onChange={(e) => this.onChangeCheckHeader(e.target)} />
                            </th>
                            <th>이름</th>
                            <th>메일주소</th>
                            <th>전화번호</th>
                            <th>요청시간</th>
                            <th>일반사용자</th>
                            <th>개발자</th>
                        </tr>
                    </thead>
                    <tbody ref={this.refTBody}>
                        {rowContent}
                    </tbody>
                </table>
                <div className={styles.buttonArea}>
                    <button ref={this.refBtnDeny} className={btnClassName} onClick={() => this.onClickResponse(false)}>거절</button>
                    <button ref={this.refBtnPermit} className={btnClassName} onClick={() => this.onClickResponse(true)}>승인</button>
                </div>
                <div ref={this.refConfirm} className={styles.confirmBox}>
                    <ConfirmDialog messages={this.state.confirmMessages} option={confirmOption} onClickConfirm={this.onClickConfirm} />
                </div>
            </div>
        );
    }
}