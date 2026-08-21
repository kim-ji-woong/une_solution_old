import React, { Component } from 'react';
/*import { useMediaQuery } from "@material-ui/core";*/
import { ConfirmDialog } from '../Root/confirmDialog';
import { Calendar } from './Calendar/calendar';
import styles from './css/vacation.module.css';
import calendarStyles from './css/calendar.module.css';
import { VacationController } from '../Root/services/vacationController';
import { VacationManager } from '../Root/services/vacationManager';

export class Request extends Component {
    static CloseDayMessage = "휴가 개시일로부터 3일 이내에 휴가요청을 할 경우에는 반드시 사유를 입력해야만 합니다.";

    refBtn = React.createRef();
    refConfirm = React.createRef();
    refCheckReqeustDescription = React.createRef();
    refTextReqeustDescription = React.createRef();

    state = {
        days: [],
        confirmMessages: null
    }

    getData(days, year, month, day) {
        const dayCount = days.length;
        const date = year * 10000 + month * 100 + day;

        for (let i = 0; i < dayCount; i++) {
            const data = days[i];

            if (data.date === date) {
                return data;
            }
        }

        return null;
    }

    setData(data, dayType, isActive) {
        const emptyDay = Calendar.getEmptyDay();
        const allDay = Calendar.getAllDay();
        const halfAM = Calendar.getHalfAM();
        const halfPM = Calendar.getHalfPM();

        if (isActive) {
            if (dayType === allDay) {
                data.dayType = allDay;
            }
            else if (dayType === Calendar.EmptyDay) {
                data.dayType = emptyDay;
            }
            else {
                data.dayType |= dayType;
            }
            /*else if (dayType === halfAM) {
                if (data.dayType === allDay || data.dayType === halfPM) {
                    data.dayType = allDay;
                }
                else {
                    data.dayType = halfAM;
                }
            }
            else if (dayType === halfPM) {
                if (data.dayType === allDay || data.dayType === halfAM) {
                    data.dayType = allDay;
                }
                else {
                    data.dayType = halfPM;
                }
            }
            else// if (dayType === Calendar.EmptyDay)
            {
                data.dayType = emptyDay;
            }*/
        }
        else {
            if (dayType === allDay) {
                data.dayType = emptyDay;
            }
            else if (dayType === Calendar.EmptyDay) {
                data.dayType = allDay;
            }
            else {
                data.dayType ^= dayType;
            }
            /*else if (dayType === halfAM) {
                if (data.dayType === allDay) {
                    data.dayType = halfPM;
                }
                else if (data.dayType === halfAM) {
                    data.dayType = emptyDay;
                }
            }
            else if (dayType === halfPM) {
                if (data.dayType === allDay) {
                    data.dayType = halfAM;
                }
                else if (data.dayType === halfPM) {
                    data.dayType = emptyDay;
                }
            }
            else// if (dayType === Calendar.EmptyDay)
            {
                data.dayType = allDay;
            }*/
        }
    }

    addData(days, year, month, day, dayType) {
        const date = year * 10000 + month * 100 + day;
        const data = {
            date: date,
            dayType: dayType
        }

        days.push(data);

        // 날짜순으로 정렬
        days.sort((data1, data2) => {
            return data1.date - data2.date;
        });
    }

    onClickDay = (year, month, day, dayType, isActive) => {
        const days = [ ...this.state.days ];
        const data = this.getData(days, year, month, day);

        if (data) {
            this.setData(data, dayType, isActive);
        }
        else {
            if (isActive) {
                this.addData(days, year, month, day, dayType);
            }
        }

        this.setState({ days: days, confirmMessages: null });
    }

    getDateString(year, data) {
        const _year = parseInt(data.date / 10000);
        const _month = parseInt((data.date % 10000) / 100);
        const _day = data.date % 100;

        let strDate = "";

        if (year === _year) {
            strDate = `${_month}월 ${_day}일`;
        }
        else {
            strDate = `${_year}년 ${_month}월 ${_day}일`;
        }

        strDate += this.getDayTypeString(data.dayType);
        /*const halfAM = Calendar.getHalfAM();
        const halfPM = Calendar.getHalfPM();

        if (data.dayType === halfAM) {
            strDate += "(오전)";
        }
        else if (data.dayType === halfPM) {
            strDate += "(오후)";
        }*/

        return strDate;
    }

    getDayTypeString(dayType) {
        if ((dayType & Calendar.Quater1st) === Calendar.Quater1st) {
            if ((dayType & Calendar.Quater2nd) === Calendar.Quater2nd) {
                if ((dayType & Calendar.Quater3rd) === Calendar.Quater3rd) {
                    if ((dayType & Calendar.Quater4th) === Calendar.Quater4th) {
                        return "";
                    }
                    else {
                        return "(1Q,2Q,3Q)";
                    }
                }
                else {
                    if ((dayType & Calendar.Quater4th) === Calendar.Quater4th) {
                        return "(1Q,2Q,4Q)";
                    }
                    else {
                        return "(오전)";
                    }
                }
            }
            else {
                if ((dayType & Calendar.Quater3rd) === Calendar.Quater3rd) {
                    if ((dayType & Calendar.Quater4th) === Calendar.Quater4th) {
                        return "(1Q,3Q,4Q)";
                    }
                    else {
                        return "(1Q,3Q)";
                    }
                }
                else {
                    if ((dayType & Calendar.Quater4th) === Calendar.Quater4th) {
                        return "(1Q,4Q)";
                    }
                    else {
                        return "(1Q)";
                    }
                }
            }
        }
        else {
            if ((dayType & Calendar.Quater2nd) === Calendar.Quater2nd) {
                if ((dayType & Calendar.Quater3rd) === Calendar.Quater3rd) {
                    if ((dayType & Calendar.Quater4th) === Calendar.Quater4th) {
                        return "(2Q,3Q,4Q)";
                    }
                    else {
                        return "(2Q,3Q)";
                    }
                }
                else {
                    if ((dayType & Calendar.Quater4th) === Calendar.Quater4th) {
                        return "(2Q,4Q)";
                    }
                    else {
                        return "(2Q)";
                    }
                }
            }
            else {
                if ((dayType & Calendar.Quater3rd) === Calendar.Quater3rd) {
                    if ((dayType & Calendar.Quater4th) === Calendar.Quater4th) {
                        return "(오후)";
                    }
                    else {
                        return "(3Q)";
                    }
                }
                else {
                    if ((dayType & Calendar.Quater4th) === Calendar.Quater4th) {
                        return "(4Q)";
                    }
                    /*else {
                        return "";
                    }*/
                }
            }
        }

        return "";
    }

    getDayCount(dayType) {
        let count = 0.0;

        if ((dayType & Calendar.Quater1st) === Calendar.Quater1st) {
            count += 0.25;
        }

        if ((dayType & Calendar.Quater2nd) === Calendar.Quater2nd) {
            count += 0.25;
        }

        if ((dayType & Calendar.Quater3rd) === Calendar.Quater3rd) {
            count += 0.25;
        }

        if ((dayType & Calendar.Quater4th) === Calendar.Quater4th) {
            count += 0.25;
        }

        return count;
    }

    getDaysInfo(year) {
        let strDate = "";
        let count = 0.0;

        const days = [...this.state.days];
        const emptyDay = Calendar.getEmptyDay();

        for (let i = 0; i < days.length; i++) {
            const data = days[i];

            if (data.dayType === emptyDay) {
                continue;
            }
            else {
                if (strDate.length === 0) {
                    strDate = this.getDateString(year, data);
                }
                else {
                    strDate += ", " + this.getDateString(year, data);
                }

                count += this.getDayCount(data.dayType);
                /*if (data.dayType === Calendar.getAllDay()) {
                    count += 1.0;
                }
                else {
                    count += 0.5;
                }*/
            }
        }

        return [strDate, count];
    }

    onClickRequest() {
        // 승인요청 버튼을 사용할 수 없도록 한다.
        if (this.refBtn.current) {
            this.refBtn.current.setAttribute("disabled", true);
        }

        this.showRequest();
    }

    async showRequest() {
        if (this.refConfirm.current.classList.contains(styles.show) === false) {
            this.refConfirm.current.classList.add(styles.show);
        }

        const date = this.props.date ?? new Date();
        const dayCount = this.getDaysInfo(date.getFullYear())[1];

        const result = await VacationController.requestManager(this.props.loginUser, this.state.days);
        const warn = result.isOverRequest ? ConfirmDialog.getWarningTag() : "";

        const usingType = this.props.options?.usingType?.toLowerCase();
        const remainDays = VacationManager.getHistoryData(this.props.history, usingType)[3];
        //const [usedDays, reservationDays, waitingDays, remainDays, svDays] = VacationManager.getHistoryData(this.props.history);

        const isCloseDay = this.isCloseDay(this.state.days);

        if (isCloseDay === null) {
            return;
        }

        const messages = [];

        if (isCloseDay) {
            const requestDescription = this.refCheckReqeustDescription.current.checked ? this.refTextReqeustDescription.current.value.toString().trim() : null;

            if (requestDescription === null || requestDescription.length === 0) {
                messages.push(Request.CloseDayMessage);
                this.refConfirm.current.style.height = ConfirmDialog.getHeight(messages.length);
                this.setState({ confirmMessages: messages });
                return;
            }
        }

        messages.push(`${warn}남은 휴가 ${remainDays}일 가운데 총 ${dayCount}일의 휴가를 신청합니다`);

        if (result.isOverRequest && result.message && result.message.length > 0) {
            messages.push(`${warn}${result.message}`);
        }

        messages.push(`휴가승인은 아래의 담당자에게 요청됩니다. 이대로 진행할까요?`);
        messages.push(`승인 담당자 : ${this.getManagers(result.managers)}`);

        this.refConfirm.current.style.height = ConfirmDialog.getHeight(messages.length);
        this.setState({ days: this.state.days, confirmMessages: messages });
    }

    // 휴가 개시일이 오늘로부터 3일 이내인지 확인
    isCloseDay(days) {
        let min = null;

        for (const day of days) {
            const date = day.date;

            if (min === null || min > date) {
                min = date;
            }
        }

        if (min === null)
            return null;

        const current = new Date();
        const today = current.getFullYear() * 10000 + (current.getMonth() + 1) * 100 + current.getDate();

        // 휴가 개시일이 오늘로부터 3일 이내인가?
        return min - today <= 3;
    }

    getManagers(managers) {
        const count = managers.length;
        let managerList = "";
        let noArrow = false;

        for (let i = 0; i < count; i++) {
            const manager = managers[i];
            const next = noArrow ? ", " : " -> ";

            if (managerList.length === 0)
                managerList = manager.name + " " + manager.level;
            else
                managerList += next + manager.name + " " + manager.level;

            noArrow = manager.isTopManager;
        }

        return managerList;
    }

    onClickConfirm = (result) => {
        const yes = ConfirmDialog.getResultYes();
        //const no = ConfirmDialog.getResultNo();

        if (result === yes) {
            this.doRequest();
        }
        else {
            console.log("No Click");

            // 승인요청 버튼을 다시 사용할 수 있도록 한다.
            if (this.refBtn.current) {
                this.refBtn.current.removeAttribute("disabled");
                this.removeActiveClearDays();
            }
        }

        if (this.refConfirm.current.classList.contains(styles.show)) {
            this.refConfirm.current.classList.remove(styles.show);
        }

        this.setState({ days: [], confirmMessages: null });
    }

    removeActiveClearDays() {
        const div = this.refBtn.current.parentElement?.parentElement?.parentElement;

        if (div) {
            if (div.tagName === "DIV") {
                const childCount = div.children.length;
                let calendarArea = null;

                for (let i = 0; i < childCount; i++) {
                    const child = div.children[i];
                    
                    if (child.classList.contains(calendarStyles.calendarArea)) {
                        calendarArea = child;
                        break;
                    }
                }

                if (calendarArea) {
                    const count = calendarArea.children.length;
                    let calendarBody = null;

                    for (let i = 0; i < count; i++) {
                        const item = calendarArea.children[i];
                        
                        if (item.classList.contains(calendarStyles.dateItems)) {
                            calendarBody = item;
                            break;
                        }
                    }

                    if (calendarBody) {
                        const columnCount = calendarBody.children.length;

                        for (let i = 0; i < columnCount; i++) {
                            const column = calendarBody.children[i];
                            const itemCount = column.children.length;

                            for (let j = 0; j < itemCount; j++) {
                                const item = column.children[j];

                                if (item.classList.contains(calendarStyles.weekday)) {
                                    continue;
                                }

                                const dayItemCount = item.children.length;

                                for (let k = 0; k < dayItemCount; k++) {
                                    const halfDays = item.children[k];

                                    if (halfDays.classList.contains(calendarStyles.halfDays)) {
                                        const halfDayItemCount = halfDays.children.length;

                                        for (let l = 0; l < halfDayItemCount; l++) {
                                            const halfDay = halfDays.children[l];

                                            if (halfDay.classList.contains(calendarStyles.active)) {
                                                halfDay.classList.remove(calendarStyles.active);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    async doRequest() {
        const requestDescription = this.refCheckReqeustDescription.current.checked ? this.refTextReqeustDescription.current.value.toString().trim() : null;
        const result = await VacationController.requestVacation(this.props.loginUser, this.state.days, requestDescription);

        if (!result) {
            alert("시스템 오류가 발생하였습니다.\r\n시스템 관리자에게 문의해 주세요.");
        }
        else {
            if (result.success) {
                this.props.addVacationHistory(result.vacationDetail);
                alert(result.message);
            }
            else {
                alert(result.message);
            }
        }

        // 승인요청 버튼을 다시 사용할 수 있도록 한다.
        if (this.refBtn.current) {
            this.refBtn.current.removeAttribute("disabled");
        }
    }

    onClickCheckReqeustDescription() {
        if (this.refCheckReqeustDescription.current.checked) {
            if (this.refTextReqeustDescription.current.classList.contains(styles.show) === false) {
                this.refTextReqeustDescription.current.classList.add(styles.show);
            }
        }
        else {
            if (this.refTextReqeustDescription.current.classList.contains(styles.show)) {
                this.refTextReqeustDescription.current.classList.remove(styles.show);
            }
        }

        this.setState({ days: this.state.days, confirmMessages: this.state.confirmMessages });
    }

    /*getTextAreaHeight() {
        const textAreaHeight = ConfirmDialog.getHeight(3);
        return parseInt(textAreaHeight.substring(0, textAreaHeight.length - 2));
    }*/

    getConfirmDialogYesNo() {
        const messages = this.state.confirmMessages;

        if (messages !== null && messages.length === 1) {
            if (messages[0] === Request.CloseDayMessage) {
                return ConfirmDialog.OK;
            }
        }

        return ConfirmDialog.getYesNo();
    }

    render() {
        if (!this.props.history) {
            return (
                <div className={styles.reportArea}>
                    <h2>로그인한 사용자의 휴가요청 페이지를 얻어올 수 없습니다. 관리자에게 문의하세요.</h2>
                </div>
            );
        }

        let history = null;

        if (this.props.history.year === this.props.year) {
            history = this.props.history;
        }
        else if (this.props.history.year === this.props.year - 1) {
            history = this.props.getNextYearHistory(this.props.loginUser);
        }
        else if (this.props.history.year === this.props.year + 1) {
            history = this.props.getLastYearHistory(this.props.loginUser);
        }

        if (!history) {
            return (
                <div className={styles.reportArea}>
                    <h2>로그인한 사용자의 휴가요청 페이지를를 얻어올 수 없습니다. 관리자에게 문의하세요.</h2>
                </div>
            );
        }

        //const date = this.props.date ?? new Date();
        //const [strDate, dayCount] = this.getDaysInfo(date.getFullYear());
        const [strDate, dayCount] = this.getDaysInfo(this.props.year);
        const btnClassName = dayCount === 0 ? styles.btnRequest + " " + styles.disabled : styles.btnRequest;

        const confirmOption = this.getConfirmDialogYesNo();
        //const confirmOption = ConfirmDialog.getYesNo();

        //const textAreaHeight = this.getTextAreaHeight();
        const fromTo = Calendar.getFromToCalendar(this.props.loginUser);

/*        const confirmBox = useMediaQuery({
            query: "(min-width:1024px) and (max-width:1920px)"
        });*/

        return (
            <div>
                <h4 className={styles.requestTitle}>휴가요청</h4>
                <div className={styles.requestArea2}>
                    <span className={styles.totalnumber}>{`총 휴가일수 : ${dayCount}일`}</span>
                    <br />
                    <br />
                    <span className={styles.countdate}>{strDate}</span>
                </div>
                <Calendar fromTo={fromTo} year={this.props.year} month={this.props.month} holidays={this.props.holidays} vacations={history.usedVacations} editable="true" options={this.props.options} onClickDay={this.onClickDay} onChangeYear={this.props.onChangeYear} onChangeMonth={this.props.onChangeMonth} />
                <div className={styles.btnArea}>
                    <div className={styles.btnLeftArea}>
                        <button ref={this.refBtn} className={btnClassName} disabled={dayCount === 0} onClick={() => this.onClickRequest()}>승인요청</button>
                        <div ref={this.refConfirm} className={styles.confirmBox}>
                            <ConfirmDialog messages={this.state.confirmMessages} option={confirmOption} onClickConfirm={this.onClickConfirm} />
                        </div>
                    </div>
                    <div className={styles.btnRightArea}>
                        <input ref={this.refCheckReqeustDescription} className={styles.checkRequestDescription} id="checkRequestDescription" name="checkRequestDescription" type="checkbox" value="true" onClick={() => this.onClickCheckReqeustDescription()}/>
                        <input name="checkRequestDescription" type="hidden" value="false" />
                        <label htmlFor="checkRequestDescription">&nbsp;요청의견 쓰기</label>
                        <textarea ref={this.refTextReqeustDescription} className={styles.requestDescription}></textarea>
                    </div>
                </div>
            </div>
        );
    }
}