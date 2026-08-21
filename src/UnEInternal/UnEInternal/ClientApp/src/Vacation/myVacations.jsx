import React, { Component } from 'react';
import { VacationManager } from '../Root/services/vacationManager';
import { Calendar } from './Calendar/calendar';
import styles from './css/vacation.module.css';

export class MyVacations extends Component {

    onChangeMonth = (goNext) => {
        this.props.onChangeMonth(goNext);
    }
    
    render() {
        if (!this.props.history) {
            return (
                <div className={styles.reportArea}>
                    <h2>로그인한 사용자의 휴가이력 정보를 얻어올 수 없습니다. 관리자에게 문의하세요.</h2>
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
                    <h2>로그인한 사용자의 휴가이력 정보를 얻어올 수 없습니다. 관리자에게 문의하세요.</h2>
                </div>
            );
        }

        const usingType = this.props.options?.usingType?.toLowerCase();

        //const date = new Date();
        const fromTo = Calendar.getFromToCalendar(this.props.loginUser);
        const [usedDays, reservationDays, waitingDays, remainDays, svDays] = VacationManager.getHistoryData(history, usingType);

        return (
            <>
            {/* <div className={styles.divv}> */ }
                <h4 className={styles.vacationTitle}>휴가현황</h4>
                <div className={styles.reportArea}>
                    <ul className={styles.reportLeft}>
                        <li><span className={styles.reportborder}>{`기준일 : ${history.year}년 ${history.month}월 ${history.day}일`}</span></li>
                        <li><span className={styles.reportborder}>{`부여된 휴가일수 : ${VacationManager.floatString(history.totalDays)}일`}</span></li>
                        <li className={styles.used}>{`사용한 휴가일수 : ${VacationManager.floatString(usedDays)}일`}</li>
                        <li><span className={styles.reportborder}>{`남은 휴가일수 : ${VacationManager.floatString(remainDays)}일`}</span></li>
                    </ul>
                    <ul className={styles.reportRight}>
                        <li><span className={styles.reportborder}>{`부여된 특별휴가 : ${VacationManager.floatString(svDays)}일`}</span></li>
                        <li className={styles.reservation}>{`승인된 휴가 : ${VacationManager.floatString(reservationDays)}일`}</li>
                        <li className={styles.waiting}>{`승인 대기중인 휴가 : ${VacationManager.floatString(waitingDays)}일`}</li>
                    </ul>
                </div>
                <Calendar fromTo={fromTo} year={this.props.year} month={this.props.month} holidays={this.props.holidays} vacations={history.usedVacations} options={this.props.options} onChangeYear={this.props.onChangeYear} onChangeMonth={this.onChangeMonth} />
            {/* </div> */}
            </>
        );
    }
}