import React, { Component } from 'react';
import { ConfirmDialog } from '../Root/confirmDialog';
import { VacationController } from '../Root/services/vacationController';
import styles from './css/response.module.css';
import vacationStyles from './css/vacation.module.css';

export class CancelVacation extends Component {
    constructor(props) {
        super(props);

        this.state = {
            vacations: [],
            selectedCount: 0,
            confirmMessages: null
        };

        this.refTHead = React.createRef();
        this.refTBody = React.createRef();
        this.refBtn = React.createRef();
        this.refConfirm = React.createRef();
    }

    componentDidMount() {
        this.getMyVacationList();
    }

    async getMyVacationList() {
        const userID = this.props.loginUser?.userID;

        if (userID) {
            const date = new Date();
            const year = date.getFullYear();
            const [success, message, vacations] = await VacationController.requestVacationList(userID, year);

            if (success)
                this.setState({ vacations });
            else if (message && message.length > 0)
                alert(message);
        }
    }

    static getManagerName(vacation) {
        if (!vacation.lastManager) {
            return "";
        }

        return vacation.lastManager.name + " " + vacation.lastManager.level;
    }

    static getConfirmTime(vacation) {
        if (!vacation.confirmTime) {
            return "-";
        }

        const dateNtime = vacation.confirmTime.split('T');

        if (dateNtime.length === 2) {
            const date = dateNtime[0].trim();
            const time = dateNtime[1].trim();

            const dateDatas = date.split('-');
            const timeDatas = time.split(':');

            if (dateDatas.length >= 3 && timeDatas.length >= 3) {
                return dateDatas[1].trim() + "월 " + dateDatas[2].trim() + "일 " + timeDatas[0].trim() + "시 " + timeDatas[1].trim() + "분";
            }
        }

        return "-";
    }

    static getStatusString(status) {
        if (status === 0) {
            return "결재 대기중";
        }
        else if (status === 1) {
            return "결재 완료";
        }
        else if (status === 2) {
            return "휴가 진행중";
        }

        return "";
    }

    getRowContent() {
        const rowContent = [];
        let index = 0;

        const vacations = [...this.state.vacations];

        vacations.map(vacation => {
            rowContent.push(
                <tr key={"rowBody_" + index++} data-requestID={vacation.requestID} className={styles.tableInterval} >
                    <td>
                        <input type="checkbox" onChange={(e) => this.onChangeCheckRow(e.target)}/>
                    </td>
                    <td className={styles.thickTD}>{CancelVacation.getManagerName(vacation)}</td>
                    <td className={styles.thickTD}>{CancelVacation.getConfirmTime(vacation)}</td>
                    <td className={styles.thickTD}>{vacation.daysDescription}</td>
                    <td className={styles.thickTD}>{CancelVacation.getStatusString(vacation.status)}</td>
                </tr>
            );
        });

        return rowContent;
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

    doCancel() {
        if (!this.refTBody.current) {
            return;
        }

        const children = this.refTBody.current.children;
        const childCount = children.length;

        const requestIDs = [];
        
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
                            requestIDs.push(parseInt(child.dataset.requestid));
                        }
                    }
                }
            }
        }

        if (requestIDs.length > 0) {
            this.requestCancel(requestIDs);
            this.clearCheckBox();
        }
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

    onClickCancel() {
        // 휴가취소 버튼을 사용할 수 없도록 한다.
        /*if (this.refBtn.current) {
            this.refBtn.current.setAttribute("disabled", true);
        }*/

        this.showConfirmBox();
    }

    async showConfirmBox() {
        const selectedCount = this.state.selectedCount;

        if (selectedCount === 0) {
            return;
        }

        if (this.refConfirm.current.classList.contains(styles.show) === false) {
            this.refConfirm.current.classList.add(styles.show);
        }

        const messages = [];
        messages.push(`선택된 ${selectedCount}건의 휴가를 취소하시겠습니까?`);
        messages.push(`취소된 휴가는 되돌릴 수 없습니다.`);
        messages.push(`이대로 진행할까요?`);

        this.refConfirm.current.style.height = ConfirmDialog.getHeight(messages.length);
        this.setState({ confirmMessages: messages });
    }

    onClickConfirm = (result) => {
        const yes = ConfirmDialog.getResultYes();
        
        if (result === yes) {
            this.doCancel();
        }
        else {
            console.log("No Click");
            this.clearCheckBox();

            // 휴가취소 버튼을 다시 사용할 수 있도록 한다.
            /*if (this.refBtn.current) {
                this.refBtn.current.removeAttribute("disabled");
            }*/
        }

        if (this.refConfirm.current.classList.contains(styles.show)) {
            this.refConfirm.current.classList.remove(styles.show);
        }

        this.setState({ confirmMessages: null });
    }

    async requestCancel(requestIDs) {
        const [success, message, history, historyNextYear] = await VacationController.requestCancelVacations(requestIDs);

        if (success) {
            if (history != null) {
                this.props.updateHistory(history, historyNextYear);
            }

            const vacations = this.removeVacations(requestIDs);
            this.setState({ vacations, selectedCount: 0 });

            alert("선택하신 휴가가 취소되었습니다.");
        }
        else {
            if (message && message.length > 0) {
                alert(message);
            }
        }
    }

    removeVacations(requestIDs) {
        const vacations = [...this.state.vacations];
        const vacationCount = vacations.length;

        for (let i = vacationCount - 1; i >= 0; i--) {
            const vacation = vacations[i];

            if (requestIDs.includes(vacation.requestID)) {
                vacations.splice(i, 1);
            }
        }

        return vacations;
    }

    render() {
        const rowContent = this.getRowContent();
        const btnClassName = this.state.selectedCount === 0 ? vacationStyles.btnRequest + " " + vacationStyles.disabled : vacationStyles.btnRequest;

        const confirmOption = ConfirmDialog.getYesNo();

        return (
            <div className={styles.responseArea}>
                <h4 className={styles.textCenter}>휴가 취소</h4>
                <div className={styles.titleBox}>
                    <div>
                        <div className={styles.textLeft}>휴가기간이 아직 끝나지 않았거나 결재 대기중인 목록입니다.</div>
                        <br />
                        <div className={styles.textLeft}>휴가취소는 본인만 가능합니다.</div>
                        <div className={styles.textLeft}>휴가 진행중인 경우 이미 사용한 휴가를 제외하고 취소됩니다.</div>
                        <div className={styles.textLeft}>당일의 휴가는 오전 9시 30분 이전에는 오전 반차, 낮 12시까지는 오후 반차를 취소할 수 있습니다.</div>
                    </div>
                </div>
                <table className={styles.stripedTable} aria-labelledby="tabelLabel">
                    <thead ref={this.refTHead}>
                        <tr>
                            <th>
                                <input type="checkbox" onChange={(e) => this.onChangeCheckHeader(e.target)} />
                            </th>
                            <th>승인자</th>
                            <th>승인일시</th>
                            <th>기간</th>
                            <th>상태</th>
                        </tr>
                    </thead>
                    <tbody ref={this.refTBody}>
                        {rowContent}
                    </tbody>
                </table>
                <button ref={this.refBtn} className={btnClassName} onClick={() => this.onClickCancel()} id={vacationStyles.btnCancel}>휴가취소</button>
                <div ref={this.refConfirm} className={styles.cancelBox}>
                    <ConfirmDialog messages={this.state.confirmMessages} option={confirmOption} onClickConfirm={this.onClickConfirm} />
                </div>
            </div>
        );
    }
}