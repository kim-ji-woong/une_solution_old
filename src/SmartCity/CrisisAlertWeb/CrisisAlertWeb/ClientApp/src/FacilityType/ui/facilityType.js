import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import { AccountController } from '../../Account/services/accountController';

import SessionString from '../../Common/js/sessionString';

class FacilityType extends Component {

    showFacilityType = (key) => {
        // 세션 값을 체크하여 총괄 관리자, 일반 관리자 구분
        const result = this.sessionLogin(key);

        alert(result)

        // 페이지 구분
    }

    async sessionLogin(key) {
        const result = await AccountController.sessionLogin(key);

        return result;
    }

    render() {
        let key = null;

        // 계정 체크, 총괄 관리자, 일반 관리자 구분하여 페이지 표시
        if (window.localStorage.getItem(SessionString.Key.account) === null && window.sessionStorage.getItem(SessionString.Key.account) === null) {
            // 로그인 정보가 없으면 로그인 페이지로 이동
            this.props.history.push('/');
        } else if (window.localStorage.getItem(SessionString.Key.account) !== null) {
            key = JSON.parse(window.localStorage.getItem(SessionString.Key.account));
        } else if (window.sessionStorage.getItem(SessionString.Key.account) !== null) {
            key = JSON.parse(window.sessionStorage.getItem(SessionString.Key.account));
        }

        this.showFacilityType(key);

        return (
            <>

            </>
        );
    }
}

export default withRouter(FacilityType);
