import React, { Component } from 'react';
import styles from './css/response.module.css';
import { ConfirmDialog } from '../Root/confirmDialog';
import { VacationController } from '../Root/services/vacationController';

import Paginate from './paginate';

export class WaitResponse extends Component {
    refTBody = React.createRef();
    refCheckManagerDescription = React.createRef();
    refConfirmBox = React.createRef();
    refTextManagerDescription = React.createRef();

    constructor(props) {
        super(props);

        this.props = props;

        const allWaitingRequests = [];
        let allRequestCount = null;
        let managerRequest = null;

        if (this.props.managerRequest != null) {
            managerRequest = this.props.managerRequest;

            allRequestCount = managerRequest.waitingRequests.length + managerRequest.waitingRequestSpecialVacations.length;
            //this.state.allRequest = managerRequest.waitingRequests.length;
            //this.state.managerRequest = managerRequest;

            allWaitingRequests.push.apply(allWaitingRequests, managerRequest.waitingRequests);
            allWaitingRequests.push.apply(allWaitingRequests, managerRequest.waitingRequestSpecialVacations);
        }

        this.state = {
            confirmMessage: null,
            data: null,
            permit: null,

            managerRequest: null,
            allWaitingRequests: allWaitingRequests,
            description: null,
            prevProps: props,
            allRequest: allRequestCount,    // 전체 요청 갯수  
            page: 1,                        // 현재 페이지
            ongPage: 10                     // 한 페이지에 보여줄 요청의 수.
        }

        // 승인한 요청의 ID
        this.permitRequestID = null;
    }

    static getDerivedStateFromProps(props, state) {
        if (props === state.prevProps) {
            return state;
        }

        const allWaitingRequests = [];
        let allRequestCount = null;
        let managerRequest = null;

        if (props.managerRequest != null) {
            managerRequest = props.managerRequest;

            allRequestCount = managerRequest.waitingRequests.length + managerRequest.waitingRequestSpecialVacations.length;
            allWaitingRequests.push.apply(allWaitingRequests, managerRequest.waitingRequests);
            allWaitingRequests.push.apply(allWaitingRequests, managerRequest.waitingRequestSpecialVacations);
        }

        return {
            confirmMessage: null,
            data: null,
            permit: null,

            managerRequest: null,
            allWaitingRequests: allWaitingRequests,
            description: null,
            prevProps: props,
            allRequest: allRequestCount,    // 전체 요청 갯수  
            page: 1,                        // 현재 페이지
            ongPage: 10                     // 한 페이지에 보여줄 요청의 수.
        };
    }

    onClick = (event, wait, permit) => {
        if (event.target.classList.contains(styles.disabled)) {
            return;
        }

        this.setEnableButtons(false);

        if (wait.requestMember) {
            const user = wait.requestMember.name + " " + wait.requestMember.level;
            const decision = permit ? "승인" : "거절";
            const messages = [];

            messages.push(`요청자 : ${user}`);
            messages.push(`기간 : ${this.getPeriod(wait)}`);
            messages.push(`${decision} 하시겠습니까 ?`);

            this.setState({ confirmMessage: messages, data: wait, permit: permit });
        }
        else if (wait.requestManager) {
            const user = wait.requestManager.name + " " + wait.requestManager.level;
            const decision = permit ? "승인" : "거절";
            const messages = [];

            messages.push(`특별휴가 요청자 : ${user}`);
            messages.push(`기간 : ${this.getDays(wait)}`);
            messages.push(`대상자 : ${this.getTargetMembers(wait)}`);
            messages.push(`${decision} 하시겠습니까 ?`);

            this.setState({ confirmMessage: messages, data: wait, permit: permit });
        }
    }

    setEnableButtons(enabled) {
        const buttons = this.refTBody.current.getElementsByClassName(styles.btn);

        if (buttons) {
            for (let i = 0; i < buttons.length; i++) {
                const btn = buttons[i];

                if (enabled) {
                    if (btn.classList.contains(styles.disabled)) {
                        btn.classList.remove(styles.disabled);
                    }
                }
                else {
                    if (btn.classList.contains(styles.disabled) === false) {
                        btn.classList.add(styles.disabled);
                    }
                }
            }
        }
    }

    onClickCheckManagerDescription() {
        if (this.refCheckManagerDescription.current.checked) {
            if (this.refTextManagerDescription.current.classList.contains(styles.show) === false) {
                this.refTextManagerDescription.current.classList.add(styles.show);
            }
        }
        else {
            if (this.refTextManagerDescription.current.classList.contains(styles.show)) {
                this.refTextManagerDescription.current.classList.remove(styles.show);
            }
        }
    }

    onClickConfirm = (result) => {
        const no = ConfirmDialog.getResultNo();

        if (result === no) {
            this.setEnableButtons(true);
            this.setState({ confirmMessage: null, data: null, permit: null });
        }
        else {
            const managerDescription = this.getManagerDescription();
            this.permitRequestID = this.state.data.requestID;

            if (this.state.data.requestMember) {
                // 일반휴가
                this.processRequest(this.state.data.requestID, this.state.permit, managerDescription);
            }
            else if (this.state.data.requestManager) {
                // 특별휴가
                this.processSpecialVacationRequest(this.state.data.requestID, this.state.permit, managerDescription);
            }
        }
    }

    onMouseOver(wait) {
        if (wait.requestDescription && wait.requestDescription.length > 0) {
            this.setState({ description: wait.requestDescription });
        }
    }

    onMouseOut(wait) {
        if (wait.requestDescription && wait.requestDescription.length > 0) {
            this.setState({ description: null });
        }
    }

    getManagerDescription() {
        if (this.refCheckManagerDescription.current.checked) {
            const description = this.refTextManagerDescription.current.value.toString().trim();

            if (description.length === 0) {
                return null;
            }

            return description;
        }

        return null;
    }

    async processSpecialVacationRequest(requestID, permit, managerDescription) {
        await VacationController.processSpecialVacationRequest(requestID, permit, this.props.loginUser.userID, managerDescription);
        this.setEnableButtons(true);
        await this.setState({ confirmMessage: null, data: null, permit: null });
        this.props.removeRequest(requestID, false);
    }

    async processRequest(requestID, permit, managerDescription) {
        await VacationController.processRequest(requestID, permit, this.props.loginUser.userID, managerDescription);
        this.setEnableButtons(true);
        await this.setState({ confirmMessage: null, data: null, permit: null });
        this.props.removeRequest(requestID, true);
        /*const managerRequest = this.removeRequest(requestID);
        this.setEnableButtons(true);
        this.setState({ confirmMessage: null, data: null, permit: null, managerRequest: null });*/
    }

    getTime(wait) {
        return `${wait.requestMonth}월 ${wait.requestDay}일 ${wait.requestHour}시 ${wait.requestMinute}분`;
    }

    getFloatString(data) {
        const str = data.toFixed(1);

        if (str.endsWith(".0")) {
            return str.substring(0, str.length - 2);
        }

        return str;
    }

    getTargetMembers(wait) {
        let memberNames = "";

        if (wait.targetMembers) {
            const count = wait.targetMembers.length;

            for (let i = 0; i < count; i++) {
                const member = wait.targetMembers[i];

                if (i === 0) {
                    memberNames = member.name + " " + member.level;
                }
                else {
                    memberNames += ", " + member.name + " " + member.level;
                }
            }
        }

        return memberNames;
    }

    getDays(wait) {
        const days = wait.days.toFixed(1).toString();
        let strDays = "";

        if (days.endsWith(".0")) {
            strDays = days.substring(0, days.length - 2) + "일";
        }
        else {
            strDays = days + "일";
        }

        return strDays;
    }

    getPeriod(wait) {
        if (wait.requestManager) {
            // 특별휴가
            const days = wait.days.toFixed(1).toString();
            let strDays = "";

            if (days.endsWith(".0")) {
                strDays = days.substring(0, days.length - 2) + "일(특별휴가)";
            }
            else {
                strDays = days + "일(특별휴가)";
            }

            const targetMembers = this.getTargetMembers(wait);

            if (targetMembers.length > 0) {
                strDays += ", 대상자 : " + targetMembers;
            }

            return strDays;
        }

        if (wait.period.includes('~')) {
            return wait.period + "(" + this.getFloatString(wait.days) + "일)";
        }

        return wait.period;
    }

    pageChange = (pageNum) => {
        this.setState({ page: pageNum });
        return;
    }

    addRowContent(rowContent, min, max) {
        for (let i = min; i < max; i++) {
            const wait = this.state.allWaitingRequests[i];

            if (this.permitRequestID) {
                if (wait.requestID === this.permitRequestID) {
                    this.permitRequestID = null;
                    continue;
                }
            }

            if (wait) {
                if (!wait.requestManager) {
                    // 일반휴가 승인요청
                    rowContent.push(
                        <tr key={wait.requestTime} onMouseOver={() => this.onMouseOver(wait)} onMouseOut={() => this.onMouseOut(wait)}>
                            <td>{wait.requestMember.name + " " + wait.requestMember.level}</td>
                            <td>{this.getTime(wait)}</td>
                            <td className={styles.periodCell}>{this.getPeriod(wait)}</td>
                            <td className={styles.buttonCell}><button className={styles.btn} onClick={(event) => this.onClick(event, wait, true)}>승인</button></td>
                            <td className={styles.buttonCell}><button className={styles.btn} onClick={(event) => this.onClick(event, wait, false)}>거절</button></td>
                        </tr>
                    );
                }
                else {
                    // 특별휴가 승인요청
                    rowContent.push(
                        <tr key={"SV_" + wait.requestTime} onMouseOver={() => this.onMouseOver(wait)} onMouseOut={() => this.onMouseOut(wait)}>
                            <td>{wait.requestManager.name + " " + wait.requestManager.level}</td>
                            <td>{this.getTime(wait)}</td>
                            <td className={styles.periodCell}>{this.getPeriod(wait)}</td>
                            <td className={styles.buttonCell}><button className={styles.btn} onClick={(event) => this.onClick(event, wait, true)}>승인</button></td>
                            <td className={styles.buttonCell}><button className={styles.btn} onClick={(event) => this.onClick(event, wait, false)}>거절</button></td>
                        </tr>
                    );
                }
            }
            else {
                break;
            }
        }
    }

    render() {
        const rowContent = [];

        let min = (this.state.page - 1) * this.state.ongPage;
        let max = min + this.state.ongPage;
        if (max > this.state.allRequest) {
            max = this.state.allRequest;
        }

        this.addRowContent(rowContent, min, max);
        /*for (let i = min; i < max; i++) {
            const wait = this.state.managerRequest.waitingRequests[i];

            if (wait) {
                //this.state.managerRequest.waitingRequests.map(wait =>
                //    <tr key={wait.requestTime}>
                //        <td>{wait.requestMember.name + " " + wait.requestMember.level}</td>
                //        <td>{this.getTime(wait)}</td>
                //        <td>{this.getPeriod(wait)}</td>
                //        <td className={styles.buttonCell}><button className={styles.btn} onClick={(event) => this.onClick(event, wait, true)}>승인</button></td>
                //        <td className={styles.buttonCell}><button className={styles.btn} onClick={(event) => this.onClick(event, wait, false)}>거절</button></td>
                //    </tr>
                //)
                rowContent.push(
                    <tr key={wait.requestTime} onMouseOver={() => this.onMouseOver(wait)} onMouseOut={() => this.onMouseOut(wait)}>
                        <td>{wait.requestMember.name + " " + wait.requestMember.level}</td>
                        <td>{this.getTime(wait)}</td>
                        <td>{this.getPeriod(wait)}</td>
                        <td className={styles.buttonCell}><button className={styles.btn} onClick={(event) => this.onClick(event, wait, true)}>승인</button></td>
                        <td className={styles.buttonCell}><button className={styles.btn} onClick={(event) => this.onClick(event, wait, false)}>거절</button></td>
                    </tr>
                );
            }
            else {
                break;
            }
        }*/

        const tooltipAdd = this.state.description && this.state.description.length > 0 ? "" : " " + styles.hide;

        return (
            <div className={styles.contentsArea}>
                <div className={styles.responseArea}>
                    <div className={styles.titleBox}>
                        <h4 className={styles.waitTitle}>결재 대기</h4>
                        <span className={styles.textLeft}>결재를 기다리고 있는 목록들입니다.</span>
                        <span className={styles.textLeft}>결재가 올라온지 일주일이 지날때까지 처리되지 않은 결재는 자동으로 취소됩니다.</span>
                    </div>
                    {
                        this.state.confirmMessage && (
                            <div className={styles.confirmBoxArea}>
                                <div className={styles.confirmLeftArea}>
                                    <div ref={this.refConfirmBox} className={styles.confirmBox}>
                                        <ConfirmDialog messages={this.state.confirmMessage} option={ConfirmDialog.getYesNo()} buttonPosition={ConfirmDialog.Center} onClickConfirm={this.onClickConfirm} />
                                    </div>
                                </div>
                                <div className={styles.confirmRightArea}>
                                    <input ref={this.refCheckManagerDescription} className={styles.checkManagerDescription} id="checkManagerDescription" name="checkManagerDescription" type="checkbox" value="true" onClick={() => this.onClickCheckManagerDescription()} />
                                    <input name="checkManagerDescription" type="hidden" value="false" />
                                    <label htmlFor="checkManagerDescription" className={styles.labelManagerDescription}>&nbsp;의견쓰기</label>
                                    <textarea ref={this.refTextManagerDescription} className={styles.managerDescription}></textarea>
                                </div>
                            </div>
                         )
                    }
                    {this.props.managerRequest && (
                        <table className={styles.stripedTable} aria-labelledby="tabelLabel">
                            <thead>
                                <tr>
                                    <th>요청자</th>
                                    <th>요청일시</th>
                                    <th>기간</th>
                                    <th>승인</th>
                                    <th>거절</th>
                                </tr>
                            </thead>
                            <tbody ref={this.refTBody}>
                                {rowContent}
                            </tbody>
                        </table>
                    )}

                    <Paginate page={this.state.page} allRequest={this.state.allRequest} onChange={this.pageChange} />
                </div>
                {
                    this.state.description && 
                    <div className={styles.tooltipArea}>
                        <div className={styles.tooltipTextBox + tooltipAdd}>
                            <p className={styles.tooltipText}>{this.state.description}</p>
                        </div>
                    </div>
                }
            </div>
        );
    }
}