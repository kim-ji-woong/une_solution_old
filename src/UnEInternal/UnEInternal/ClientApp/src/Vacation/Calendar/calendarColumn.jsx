import React, { Component } from 'react';
import { Calendar } from './calendar';
import { CalendarDay } from './calendarDay';
import styles from '../css/calendar.module.css';

export class CalendarColumn extends Component {
    getDays() {
        const days = [];

        for (let i = 0; i < this.props.rowCount; i++) {
            if (i === 0) {
                days.push(this.props.firstDay);
            }
            else {
                days.push(i * 7 + this.props.firstDay);
            }
        }

        return days;
    }

    getDayType(day) {
        if (day <= 0 || day > 31) {
            const usingType = this.props.options?.usingType.toLowerCase();

            if (usingType === "quater") {
                return [0, 0, 0, 0];
            }
            else {
                return [0, 0];
            }
        }

        const dayType = this.props.dayTypes[day];
        return dayType;
        //return this.props.dayTypes[day];
    }

    isHoliday(day) {
        // 토,일요일은 제외
        if (this.props.weekDay === 0 || this.props.weekDay === 6) {
            return true;
        }

        const date = this.props.year * 10000 + this.props.month * 100 + day;

        for (const holiday of this.props.holidays) {
            if (date === holiday) {
                return true;
            }
        }

        return false;
    }

    isEditable(day) {
        if (!this.props.editable) {
            return false;
        }

        if (this.isHoliday(day) === false) {
            return true;
        }

        return false;
    }

    render() {
        let columnClassName = styles.column;

        if (this.props.rowCount === 4) {
            columnClassName += " " + styles._4;
        }
        else if (this.props.rowCount === 6) {
            columnClassName += " " + styles._6;
        }

        const days = this.getDays();

        // 토,일요일은 제외
        //const holiday = this.props.weekDay === 0 || this.props.weekDay === 6;
        //let editable = this.props.editable && holiday === false;

        return (
            <div className={columnClassName}>
                <div className={styles.weekday}>{Calendar.getWeekday(this.props.weekDay)}</div>
                {
                    days.map(day => (
                        <CalendarDay key={this.props.year * 10000 + this.props.month * 100 + day} fromTo={this.props.fromTo} year={this.props.year} month={this.props.month} day={day} monthDay={this.props.monthDay} holiday={this.isHoliday(day)} editable={this.isEditable(day)} dayType={this.getDayType(day)} options={this.props.options} onClickDay={this.props.onClickDay} />
                        ))
                }
            </div>
        );
    }
}