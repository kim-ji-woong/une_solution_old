import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import styles from '../../Common/css/style.css';
import Title from '../../Root/title';
import Menu from '../../Root/menu';

class Setting extends Component {

    onClickPassword = () => {
        this.props.history.push(Menu.pathPassword);
    }

    render() {

        return (
            <div className="container_sub2">
                <Title />

                <div className="contents">
                    <h3>설정</h3>
                    <div className="content_box">
                        <div id="pw_change" onClick={this.onClickPassword}>
                            <embed src="/resource/icon/iconfinder_ic_lock_outline_48px_3669336.svg"></embed>
                            <p>비밀번호 변경</p>
                            <span><img src="/resource/icon/arrow2.png"></img></span>
                        </div>
                        {/*
                        <div id="pw_change" onclick="location.href='http://218.152.200.123:8887/general_passwordfind.html'">
                            <embed src="./css/icon/iconfinder_ic_search_48px_352091.svg"></embed>
                            <p>비밀번호 찾기</p>
                            <span><img src="./css/icon/arrow2.png"></img></span>
                        </div>
                        */}
                    </div>
                </div>
            </div>
        );
    }
}

export default withRouter(Setting);
