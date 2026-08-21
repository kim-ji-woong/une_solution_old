import React, { Component } from 'react';
import styles from '../css/calendar.module.css';
import { CalendarColumn } from './calendarColumn';
import { CalendarDay } from './calendarDay';

export class Calendar extends Component {
    static WeekdayIndex = [0, 1, 2, 3, 4, 5, 6];
    static Weekday = ["일", "월", "화", "수", "목", "금", "토"];
    static EmptyDay = 0;
    static AllDay = 60;
    static HalfAM = 12;
    static HalfPM = 48;
    /*static AllDay = 1;
    static HalfAM = 2;
    static HalfPM = 3;*/
    static Quater1st = 4;
    static Quater2nd = 8;
    static Quater3rd = 16;
    static Quater4th = 32;

    static getWeekday(index) {
        return Calendar.Weekday[index];
    }

    static getEmptyDay() {
        return Calendar.EmptyDay;
    }

    static getAllDay() {
        return Calendar.AllDay;
    }

    static getHalfAM() {
        return Calendar.HalfAM;
    }

    static getHalfPM() {
        return Calendar.HalfPM;
    }

    static getQuater(index) {
        if (index === 1) {
            return Calendar.Quater1st;
        }
        else if (index === 2) {
            return Calendar.Quater2nd;
        }
        else if (index === 3) {
            return Calendar.Quater3rd;
        }
        else if (index === 4) {
            return Calendar.Quater4th;
        }

        return Calendar.AllDay;
    }

    static getFromToCalendar(loginUser) {
        const date = new Date();
        let year = date.getFullYear();
        let month = date.getMonth() + 1 + loginUser.reservationMonth;

        if (month > 12) {
            month -= 12;
            year++;
        }

        return {
            from: {
                year: loginUser.startYear,
                month: loginUser.startMonth
            },
            to: {
                year: year,
                month: month
            }
        };
    }

    lastDayofMonth(year, month) {
        return 32 - new Date(year, month - 1, 32).getDate();
    }

    getRowCount(year, month) {
        const monthDay = this.lastDayofMonth(year, month);
        const dateFirst = new Date(year, month - 1, 1);
        const firstWeekDay = dateFirst.getDay();
        let rowCount = 0;

        if (firstWeekDay === 0) {
            if (monthDay === 28) {
                rowCount = 4;
            }
            else {
                rowCount = 5;
            }
        }
        else if (firstWeekDay >= 1 && firstWeekDay <= 4) {
            rowCount = 5;
        }
        else if (firstWeekDay === 5) {
            if (monthDay <= 30) {
                rowCount = 5;
            }
            else {
                rowCount = 6;
            }
        }
        else// if (firstWeekDay === 6)
        {
            if (monthDay <= 29) {
                rowCount = 5;
            }
            else {
                rowCount = 6;
            }
        }

        return [rowCount, firstWeekDay, monthDay];
    }

    getLeftDateClassName(year, month) {
        const fromDate = this.props.fromTo.from.year * 100 + this.props.fromTo.from.month;
        const thisDate = year * 100 + month;

        if (thisDate <= fromDate) {
            return styles.btnDate + " " + styles.disabled;
        }

        return styles.btnDate;
    }

    getRightDateClassName(year, month) {
        const toDate = this.props.fromTo.to.year * 100 + this.props.fromTo.to.month;
        const thisDate = year * 100 + month;

        if (thisDate >= toDate) {
            return styles.btnDate + " " + styles.disabled;
        }

        return styles.btnDate;
    }

    makeFromToDate() {
        const date = new Date();

        // 오늘날짜 기준
        const fromTo = {
            from:
            {
                year: date.getFullYear(),
                month: date.getMonth() + 1,
                day: date.getDate()
            },
            to:
            {
                year: this.props.fromTo.to.year,
                month: this.props.fromTo.to.month,
                day: date.getDate()
            }
        }

        return fromTo;
    }

    setInitDayTypes(year, month) {
        const usingType = this.props.options?.usingType?.toLowerCase();

        if (usingType === "quater") {
            return this.setQuaterInitDayTypes(year, month);
        }

        return this.setHalfInitDayTypes(year, month);
    }

    setHalfInitDayTypes(year, month) {
        const none = CalendarDay.getNone();
        const waiting = CalendarDay.getWaitingDay();
        const reservation = CalendarDay.getReservation();
        const used = CalendarDay.getUsedDay();

        const dayTypes = {};

        for (let i = 1; i <= 31; i++) {
            dayTypes[i] = [none, none];
        }

        if (!this.props.vacations || !year || !month) {
            return dayTypes;
        }

        const thisDate = new Date();
        const today = thisDate.getFullYear() * 10000 + (thisDate.getMonth() + 1) * 100 + thisDate.getDate();
        const thisHour = thisDate.getHours();

        const vacationCount = this.props.vacations.length;

        for (let i = 0; i < vacationCount; i++) {
            const vacation = this.props.vacations[i];
            const dateCount = vacation.dates.length;

            for (let j = 0; j < dateCount; j++) {
                const date = vacation.dates[j];

                if (date.year !== year || date.month !== month) {
                    continue;
                }

                const dateNumber = date.year * 10000 + date.month * 100 + date.day;
                const [amType, pmType] = dayTypes[date.day];

                if (date.dateType === Calendar.HalfAM) {
                    if (vacation.isPermitted) {
                        if (dateNumber < today) {
                            dayTypes[date.day] = [used, pmType];
                        }
                        else if (dateNumber > today) {
                            dayTypes[date.day] = [reservation, pmType];
                        }
                        else {
                            dayTypes[date.day] = [used, pmType];
                        }
                    }
                    else {
                        dayTypes[date.day] = [waiting, pmType];
                    }
                }
                else if (date.dateType === Calendar.HalfPM) {
                    if (vacation.isPermitted) {
                        if (dateNumber < today) {
                            dayTypes[date.day] = [amType, used];
                        }
                        else if (dateNumber > today) {
                            dayTypes[date.day] = [amType, reservation];
                        }
                        else {
                            if (thisHour < 12) {
                                dayTypes[date.day] = [amType, reservation];
                            }
                            else {
                                dayTypes[date.day] = [amType, used];
                            }
                        }
                    }
                    else {
                        dayTypes[date.day] = [amType, waiting];
                    }
                }
                else {
                    if (vacation.isPermitted) {
                        if (dateNumber < today) {
                            dayTypes[date.day] = [used, used];
                        }
                        else if (dateNumber > today) {
                            dayTypes[date.day] = [reservation, reservation];
                        }
                        else {
                            if (thisHour < 12) {
                                dayTypes[date.day] = [used, reservation];
                            }
                            else {
                                dayTypes[date.day] = [used, used];
                            }
                        }
                    }
                    else {
                        dayTypes[date.day] = [waiting, waiting];
                    }
                }
            }
        }

        return dayTypes;
    }

    setQuaterInitDayTypes(year, month) {
        const none = CalendarDay.getNone();
        const waiting = CalendarDay.getWaitingDay();
        const reservation = CalendarDay.getReservation();
        const used = CalendarDay.getUsedDay();

        const dayTypes = {};

        for (let i = 1; i <= 31; i++) {
            dayTypes[i] = [none, none, none, none];
        }

        if (!this.props.vacations || !year || !month) {
            return dayTypes;
        }

        const thisDate = new Date();
        const today = thisDate.getFullYear() * 10000 + (thisDate.getMonth() + 1) * 100 + thisDate.getDate();
        const thisHour = thisDate.getHours();

        const vacationCount = this.props.vacations.length;

        for (let i = 0; i < vacationCount; i++) {
            const vacation = this.props.vacations[i];
            const dateCount = vacation.dates.length;

            for (let j = 0; j < dateCount; j++) {
                const date = vacation.dates[j];

                if (date.year !== year || date.month !== month) {
                    continue;
                }

                const dateNumber = date.year * 10000 + date.month * 100 + date.day;
                const [q1Type, q2Type, q3Type, q4Type] = dayTypes[date.day];

                let q1 = q1Type;
                let q2 = q2Type;
                let q3 = q3Type;
                let q4 = q4Type;

                if ((date.dateType & Calendar.Quater1st) === Calendar.Quater1st) {
                    if (vacation.isPermitted) {
                        if (dateNumber < today) {
                            q1 = used;
                        }
                        else if (dateNumber > today) {
                            q1 = reservation;
                        }
                        else if (thisHour < 8) {
                            q1 = reservation;
                        }
                        else {
                            q1 = used;
                        }
                    }
                    else {
                        q1 = waiting;
                    }
                }

                if ((date.dateType & Calendar.Quater2nd) === Calendar.Quater2nd) {
                    if (vacation.isPermitted) {
                        if (dateNumber < today) {
                            q2 = used;
                        }
                        else if (dateNumber > today) {
                            q2 = reservation;
                        }
                        else if (thisHour < 10) {
                            q2 = reservation;
                        }
                        else {
                            q2 = used;
                        }
                    }
                    else {
                        q2 = waiting;
                    }
                }

                if ((date.dateType & Calendar.Quater3rd) === Calendar.Quater3rd) {
                    if (vacation.isPermitted) {
                        if (dateNumber < today) {
                            q3 = used;
                        }
                        else if (dateNumber > today) {
                            q3 = reservation;
                        }
                        else if (thisHour < 12) {
                            q3 = reservation;
                        }
                        else {
                            q3 = used;
                        }
                    }
                    else {
                        q3 = waiting;
                    }
                }

                if ((date.dateType & Calendar.Quater4th) === Calendar.Quater4th) {
                    if (vacation.isPermitted) {
                        if (dateNumber < today) {
                            q4 = used;
                        }
                        else if (dateNumber > today) {
                            q4 = reservation;
                        }
                        else if (thisHour < 15) {
                            q4 = reservation;
                        }
                        else {
                            q4 = used;
                        }
                    }
                    else {
                        q4 = waiting;
                    }
                }

                dayTypes[date.day] = [q1, q2, q3, q4];
            }
        }

        return dayTypes;
    }

    render() {
        const year = this.props.year;
        const month = this.props.month;
        const [rowCount, firstWeekDay, monthDay] = this.getRowCount(year, month);

        let calendarAreaClassName = styles.calendarArea;

        if (rowCount === 4) {
            calendarAreaClassName += " " + styles._4;
        }
        else if (rowCount === 6) {
            calendarAreaClassName += " " + styles._6;
        }

        //const date = `${year}년 ${month}월`;
        const editable = this.props.editable ? this.props.editable === "true" : false;

        const fromTo = this.makeFromToDate();
        const dayTypes = this.setInitDayTypes(year, month);
        const holidays = this.props.holidays === null ? [] : this.props.holidays;

        return (
            <div className={calendarAreaClassName}>
                <div className={styles.date}>
                    <div className={styles.dateLeft}>
                        <div className={this.getLeftDateClassName(year, month)} onClick={() => this.props.onChangeMonth(false)}>
                            <i className="fas fa-chevron-left"></i>
                        </div>
                    </div>
                    <div className={styles.dateCenter}>
                        <span>{`${year}년`}</span>
                        <span>{` ${month}월`}</span>
                    </div>
                    <div className={styles.dateRight}>
                        <div className={this.getRightDateClassName(year, month)} onClick={() => this.props.onChangeMonth(true)}>
                            <i className="fas fa-chevron-right"></i>
                        </div>
                    </div>
                </div>
                <div className={styles.dateItems}>
                {
                    Calendar.WeekdayIndex.map(day => (
                        <CalendarColumn key={day} fromTo={fromTo} year={year} month={month} firstDay={day - firstWeekDay + 1} holidays={holidays} weekDay={day} monthDay={monthDay} rowCount={rowCount} editable={editable} dayTypes={dayTypes} options={this.props.options} onClickDay={this.props.onClickDay}/>
                    ))
                }
                </div>
            </div>
        );
    }
}