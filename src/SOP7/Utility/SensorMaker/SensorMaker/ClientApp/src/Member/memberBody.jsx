import React, { Component } from 'react';
import { AccountController } from '../Account/services/accountController';
import styles from './css/member.module.css';
import { WaitingUsers } from './waitingUsers';

export class MemberBody extends Component {
    // 계정생성후 승인을 기다리는 User List
    static Menu_WaitingUserList = 0;
    // 사용자 정보 편집
    static Menu_EditUser = 1;

    constructor(props) {
        super(props);
        this.state =
        {
            selectedMenu: MemberBody.Menu_WaitingUserList
        };
    }

    updateRequestUsers = (userTypes, permit, denyReason) => {
        this._updateRequestUsers(userTypes, permit, denyReason);
    }

    async _updateRequestUsers(userTypes, permit, denyReason) {
        const result = await AccountController.requestRegist(userTypes, permit, denyReason);

        if (result.success) {
            if (result.requestUsers) {
                this.props.loginData.requestUsers = result.requestUsers;
            }
            else
                this.props.loginData.requestUsers = null;
        }
        else {
            if (result.message.length > 0) {
                alert(result);
            }
        }

        this.setState({ selectedMenu: this.state.selectedMenu });
    }

    render() {
        if (this.props.loginData?.user?.isAdmin) {
            return this.renderDatas();
        }

        return (
            <div>사용권한이 없습니다.</div>
        );
    }

    renderDatas() {
        if (this.state.selectedMenu === MemberBody.Menu_WaitingUserList) {
            return (
                <WaitingUsers users={this.props.loginData.requestUsers} updateRequestUsers={this.updateRequestUsers}/>
            );
        }

        return <></>;
    }
}
