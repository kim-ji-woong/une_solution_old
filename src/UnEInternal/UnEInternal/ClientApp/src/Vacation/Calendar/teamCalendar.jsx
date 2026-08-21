import React from 'react';
import styles from '../css/teamCalendar.module.css';
import { CalendarColumn } from './calendarColumn';
import { Calendar } from './calendar';
import { CalendarDay } from './calendarDay';
import { TeamCalendarColumn } from './teamCalendarColumn';

export class TeamCalendar extends Calendar {
    setInitDayTypes(year, month) {
        const none = CalendarDay.getNone();

        const dayTypes = {};
        const usingType = this.props.options?.usingType.toLowerCase();

        if (usingType === "quater") {
            for (let i = 1; i <= 31; i++) {
                dayTypes[i] = [none, none, none, none];
            }
        }
        else {
            for (let i = 1; i <= 31; i++) {
                dayTypes[i] = [none, none];
            }
        }

        return dayTypes;
    }

    getEmptyMembers() {
        const members = {};
        const usingType = this.props.options?.usingType.toLowerCase();

        if (usingType === "quater") {
            for (let i = 1; i <= 31; i++) {
                members[i] = [[], [], [], []];
            }
        }
        else {
            for (let i = 1; i <= 31; i++) {
                members[i] = [[], []];
            }
        }

        return members;
    }

    setMember(members, date, member, dayType) {
        const usingType = this.props.options?.usingType?.toLowerCase();

        if (usingType === "quater") {
            if (dayType === null) {
                if ((date.dateType & Calendar.Quater1st) === Calendar.Quater1st) {
                    members[date.day][0].push(member);
                }
                if ((date.dateType & Calendar.Quater2nd) === Calendar.Quater2nd) {
                    members[date.day][1].push(member);
                }
                if ((date.dateType & Calendar.Quater3rd) === Calendar.Quater3rd) {
                    members[date.day][2].push(member);
                }
                if ((date.dateType & Calendar.Quater4th) === Calendar.Quater4th) {
                    members[date.day][3].push(member);
                }
            }
            else {
                if ((dayType & Calendar.Quater1st) === Calendar.Quater1st) {
                    members[date.day][0].push(member);
                }
                if ((dayType & Calendar.Quater2nd) === Calendar.Quater2nd) {
                    members[date.day][1].push(member);
                }
                if ((dayType & Calendar.Quater3rd) === Calendar.Quater3rd) {
                    members[date.day][2].push(member);
                }
                if ((dayType & Calendar.Quater4th) === Calendar.Quater4th) {
                    members[date.day][3].push(member);
                }
            }
        }
        else {
            const am = Calendar.getHalfAM();
            const pm = Calendar.getHalfPM();

            if (dayType === null) {
                if (date.dateType === am) {
                    members[date.day][0].push(member);
                }
                else if (date.dateType === pm) {
                    members[date.day][1].push(member);
                }
                else {
                    members[date.day][0].push(member);
                    members[date.day][1].push(member);
                }
            }
            else {
                if (dayType === am) {
                    members[date.day][0].push(member);
                }
                else if (dayType === pm) {
                    members[date.day][1].push(member);
                }
                else {
                    members[date.day][0].push(member);
                    members[date.day][1].push(member);
                }
            }
        }
    }

    getDailyMembers(year, month) {
        const waitingMembers = this.getEmptyMembers();
        const permitMembers = this.getEmptyMembers();
        const usedMembers = this.getEmptyMembers();

        const am = Calendar.getHalfAM();
        const pm = Calendar.getHalfPM();

        const today = new Date();
        const todayNumber = today.getFullYear() * 10000 + (today.getMonth() + 1) * 100 + today.getDate();
        const thisHour = today.getHours();

        const usingType = this.props.options?.usingType?.toLowerCase();
        const memberCount = this.props.memberDatas.length;

        for (let i = 0; i < memberCount; i++) {
            const member = this.props.memberDatas[i];
            let memberHistory = this.props.membersHistory.memberHistories[member.id];

            if (memberHistory) {
                if (memberHistory.year === year - 1) {
                    memberHistory = this.props.membersHistory.memberHistoriesNextYear[member.id];
                }
                else if (memberHistory.year === year + 1) {
                    memberHistory = this.props.membersHistory.memberHistoriesLastYear[member.id];
                }
            }

            if (!memberHistory) {
                continue;
            }

            const vacationCount = memberHistory.usedVacations.length;

            for (let j = 0; j < vacationCount; j++) {
                const vacation = memberHistory.usedVacations[j];
                const dateCount = vacation.dates.length;

                for (let k = 0; k < dateCount; k++) {
                    const date = vacation.dates[k];

                    if (date.year !== year || date.month !== month) {
                        continue;
                    }

                    const dateNumber = date.year * 10000 + date.month * 100 + date.day;

                    if (vacation.isPermitted) {
                        if (dateNumber < todayNumber) {
                            this.setMember(usedMembers, date, member, null);
                        }
                        else if (dateNumber === todayNumber) {
                            this.getTodayMembers(waitingMembers, permitMembers, usedMembers, date, member, usingType, thisHour);
                        }
                        else {
                            this.setMember(permitMembers, date, member, null);
                        }
                    }
                    else {
                        if (dateNumber < todayNumber) {
                            continue;
                        }
                        else if (dateNumber === todayNumber) {
                            this.getTodayMembers(waitingMembers, permitMembers, usedMembers, date, member, usingType, thisHour);
                        }
                        else {
                            this.setMember(waitingMembers, date, member, null);
                        }
                    }
                }
            }
        }

        return [waitingMembers, permitMembers, usedMembers];
    }

    getTodayMembers(waitingMembers, permitMembers, usedMembers, date, member, isPermitted, usingType, thisHour) {
        if (usingType === "quater") {
            if (isPermitted) {
                if (thisHour < 8) {
                    this.setMember(permitMembers, date, member, null);
                }
                else if (thisHour < 10) {
                    if ((date.dateType & Calendar.Quater1st) === Calendar.Quater1st) {
                        this.setMember(usedMembers, date, member, null);
                    }
                    else {
                        this.setMember(permitMembers, date, member, null);
                    }
                }
                else if (thisHour < 12) {
                    if ((date.dateType & Calendar.Quater1st) === Calendar.Quater1st || (date.dateType & Calendar.Quater2nd) === Calendar.Quater2nd) {
                        this.setMember(usedMembers, date, member, null);
                    }
                    else {
                        this.setMember(permitMembers, date, member, null);
                    }
                }
                else if (thisHour < 15) {
                    if ((date.dateType & Calendar.Quater1st) === Calendar.Quater1st || (date.dateType & Calendar.Quater2nd) === Calendar.Quater2nd || (date.dateType & Calendar.Quater3rd) === Calendar.Quater3rd) {
                        this.setMember(usedMembers, date, member, null);
                    }
                    else {
                        this.setMember(permitMembers, date, member, null);
                    }
                }
                else {
                    this.setMember(usedMembers, date, member, null);
                }
            }
            else {
                if (thisHour < 8) {
                    this.setMember(waitingMembers, date, member, null);
                }
                else if (thisHour < 10) {
                    if ((date.dateType & Calendar.Quater1st) !== Calendar.Quater1st) {
                        this.setMember(waitingMembers, date, member, null);
                    }
                }
                else if (thisHour < 12) {
                    if ((date.dateType & Calendar.Quater1st) !== Calendar.Quater1st && (date.dateType & Calendar.Quater2nd) !== Calendar.Quater2nd) {
                        this.setMember(waitingMembers, date, member, null);
                    }
                }
                else if (thisHour < 15) {
                    if ((date.dateType & Calendar.Quater1st) !== Calendar.Quater1st && (date.dateType & Calendar.Quater2nd) !== Calendar.Quater2nd && (date.dateType & Calendar.Quater3rd) !== Calendar.Quater3rd) {
                        this.setMember(waitingMembers, date, member, null);
                    }
                }
            }
        }
        else {
            if (isPermitted) {
                if (thisHour < 12) {
                    if (date.dateType === Calendar.HalfAM) {
                        this.setMember(usedMembers, date, member, null);
                    }
                    else if (date.dateType === Calendar.HalfPM) {
                        this.setMember(permitMembers, date, member, null);
                    }
                    else {
                        this.setMember(usedMembers, date, member, Calendar.HalfAM);
                        this.setMember(permitMembers, date, member, Calendar.HalfPM);
                    }
                }
                else {
                    this.setMember(usedMembers, date, member, null);
                }
            }
            else {
                if (thisHour < 12) {
                    if (date.dateType === Calendar.HalfAM) {
                    }
                    else if (date.dateType === Calendar.HalfPM) {
                        this.setMember(waitingMembers, date, member, null);
                    }
                    else {
                        this.setMember(waitingMembers, date, member, Calendar.HalfPM);
                    }
                }
            }
        }
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

        const fromTo = this.makeFromToDate();
        const dayTypes = this.setInitDayTypes(year, month);

        const dailyMembers = this.getDailyMembers(year, month);
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
                        <TeamCalendarColumn key={day} fromTo={fromTo} year={year} month={month} firstDay={day - firstWeekDay + 1} holidays={holidays} weekDay={day} monthDay={monthDay} rowCount={rowCount} dayTypes={dayTypes} options={this.props.options} onClickDay={this.props.onClickDay} dailyMembers={dailyMembers} showReservation={this.props.showReservation} showUsed={this.props.showUsed} showWait={this.props.showWait}/>
                    ))
                }
                </div>
            </div>
        );
    }
}