import React, { Component } from 'react';
import { Calendar } from './calendar';
import styles from '../css/teamCalendar.module.css';
import { CalendarDay } from './calendarDay';

export class TeamCalendarDay extends CalendarDay {
    static AndSoOn = "외...";

    setMembers(srcMembers, trgMembers, color, colors, show) {
        if (show) {
            const count = srcMembers.length;

            for (let i = 0; i < count; i++) {
                colors.push(color);
                trgMembers.push(srcMembers[i].name);
            }

            return count;
        }

        return 0;
    }

    set3Members(members, colors, waitCount, permitCount, usedCount) {
        const usingType = this.props.options?.usingType?.toLowerCase();
        const limitCount = usingType === "half" ? 2 : 1;

        const waitMember = waitCount >= limitCount ? members[0] + TeamCalendarDay.AndSoOn : members[0];
        const permitMember = permitCount >= limitCount ? members[waitCount] + TeamCalendarDay.AndSoOn : members[waitCount];
        const usedMember = usedCount >= limitCount ? members[waitCount + usedCount] + TeamCalendarDay.AndSoOn : members[waitCount + usedCount];

        members.splice(0, members.length);
        colors.splice(0, colors.length);

        members.push(waitMember)
        members.push(permitMember);
        members.push(usedMember);

        colors.push(styles.waiting);
        colors.push(styles.reservation);
        colors.push(styles.used);
    }

    set2Members(members, colors, type1Count, type2Count) {
        const usingType = this.props.options?.usingType?.toLowerCase();
        const limitCount = usingType === "half" ? 2 : 1;

        if (type1Count >= limitCount && type2Count >= limitCount) {
            const type1Member = members[0];
            const type1_1Member = type1Count >= limitCount + 1 ? members[1] + TeamCalendarDay.AndSoOn : members[1];
            const type2Member = members[type1Count] + TeamCalendarDay.AndSoOn;

            const type1Color = colors[0];
            const type1_1Color = colors[1];
            const type2Color = colors[type1Count];

            members.splice(0, members.length);
            colors.splice(0, colors.length);

            members.push(type1Member);
            members.push(type1_1Member);
            members.push(type2Member);

            colors.push(type1Color);
            colors.push(type1_1Color);
            colors.push(type2Color);
        }
        else if (type1Count >= limitCount) {
            const type1Member = members[0];
            const type1_1Member = type1Count >= limitCount + 1 ? members[1] + TeamCalendarDay.AndSoOn : members[1];
            const type2Member = members[type1Count];

            const type1Color = colors[0];
            const type1_1Color = colors[1];
            const type2Color = colors[type1Count];

            members.splice(0, members.length);
            colors.splice(0, colors.length);

            members.push(type1Member);
            members.push(type1_1Member);
            members.push(type2Member);

            colors.push(type1Color);
            colors.push(type1_1Color);
            colors.push(type2Color);
        }
        else if (type2Count >= limitCount) {
            const type1Member = members[0];
            const type2Member = members[type1Count];
            const type2_1Member = type2Count >= limitCount + 1 ? members[type1Count] + TeamCalendarDay.AndSoOn : members[type1Count];

            const type1Color = colors[0];
            const type2Color = colors[type1Count];
            const type2_1Color = colors[type1Count + 1];

            members.splice(0, members.length);
            colors.splice(0, colors.length);

            members.push(type1Member);
            members.push(type2Member);
            members.push(type2_1Member);

            colors.push(type1Color);
            colors.push(type2Color);
            colors.push(type2_1Color);
        }
        else {
            const type1Member = members[0];
            const type2Member = members[type1Count];

            const type1Color = colors[0];
            const type2Color = colors[type1Count];

            members.splice(0, members.length);
            colors.splice(0, colors.length);

            members.push(type1Member);
            members.push(type2Member);

            colors.push(type1Color);
            colors.push(type2Color);
        }
    }

    set1Members(members, colors, index) {
        members.splice(3, members.length - 3);
        colors.splice(3, colors.length - 3);
        members[index] = members[index] + TeamCalendarDay.AndSoOn;
    }

    getMembers(index) {
        const [waitingMembers, permitMembers, usedMembers] = this.props.dailyMembers;

        if (!waitingMembers || !permitMembers || !usedMembers) {
            return [[], []];
        }

        const members = [];
        const colors = [];

        const waitCount = this.setMembers(waitingMembers[index], members, styles.waiting, colors, this.props.showWait);
        const permitCount = this.setMembers(permitMembers[index], members, styles.reservation, colors, this.props.showReservation);
        const usedCount = this.setMembers(usedMembers[index], members, styles.used, colors, this.props.showUsed);

        const usingType = this.props.options?.usingType?.toLowerCase();
        const limitCount = usingType === "half" ? 3 : 2;

        if (waitCount + permitCount + usedCount > limitCount) {
            if (waitCount > 0) {
                if (permitCount > 0) {
                    if (usedCount > 0) {
                        this.set3Members(members, colors, waitCount, permitCount, usedCount);
                    }
                    else {
                        this.set2Members(members, colors, waitCount, permitCount);
                    }
                }
                else {
                    if (usedCount > 0) {
                        this.set2Members(members, colors, waitCount, usedCount);
                    }
                    else {
                        this.set1Members(members, colors, limitCount - 1);
                    }
                }
            }
            else {
                if (permitCount > 0) {
                    if (usedCount > 0) {
                        this.set2Members(members, colors, permitCount, usedCount);
                    }
                    else {
                        this.set1Members(members, colors, limitCount - 1);
                    }
                }
                else {
                    this.set1Members(members, colors, limitCount - 1);
                }
            }
        }

        return [members, colors];
    }

    getMember(index, members) {
        if (index < members.length) {
            return members[index];
        }

        return "";
    }

    getColor(index, colors) {
        if (index < colors.length) {
            return " " + colors[index];
        }

        return "";
    }

    render() {
        const day = this.props.day <= 0 || this.props.day > this.props.monthDay ? "" : this.props.day.toString();
        const editable = false;

        let className = this.getClassName();

        const usingType = this.props.options?.usingType.toLowerCase();

        if (usingType === "quater") {
            const [q1Type, q2Type, q3Type, q4Type] = this.props.dayType;
            const q1Name = this.getAddName(q1Type, editable);
            const q2Name = this.getAddName(q2Type, editable);
            const q3Name = this.getAddName(q3Type, editable);
            const q4Name = this.getAddName(q4Type, editable);
            const q1Editable = this.getHalfEditable(q1Type, editable);
            const q2Editable = this.getHalfEditable(q2Type, editable);
            const q3Editable = this.getHalfEditable(q3Type, editable);
            const q4Editable = this.getHalfEditable(q4Type, editable);

            const [q1Members, q1Colors] = this.getMembers(0);
            const [q2Members, q2Colors] = this.getMembers(1);
            const [q3Members, q3Colors] = this.getMembers(2);
            const [q4Members, q4Colors] = this.getMembers(3);

            return (
                <div className={className}>
                    <span className={styles.dayText}>{day}</span>
                    <div className={styles.quaterDays}>
                        <div ref={this.ref1stQuarter} className={styles.quaterDay + q1Name} onClick={() => this.onHalfDayClick(this.ref1stQuarter, q1Editable)}>
                            <span className={styles.memberName + this.getColor(0, q1Colors)}>{this.getMember(0, q1Members)}</span>
                            <span className={styles.memberName + this.getColor(1, q1Colors)}>{this.getMember(1, q1Members)}</span>
                        </div>
                        <div ref={this.ref3rdQuarter} className={styles.quaterDay + q3Name} onClick={() => this.onHalfDayClick(this.ref3rdQuarter, q3Editable)}>
                            <span className={styles.memberName + this.getColor(0, q3Colors)}>{this.getMember(0, q3Members)}</span>
                            <span className={styles.memberName + this.getColor(1, q3Colors)}>{this.getMember(1, q3Members)}</span>
                        </div>
                        <div ref={this.ref2ndQuarter} className={styles.quaterDay + q2Name} onClick={() => this.onHalfDayClick(this.ref2ndQuarter, q2Editable)}>
                            <span className={styles.memberName + this.getColor(0, q2Colors)}>{this.getMember(0, q2Members)}</span>
                            <span className={styles.memberName + this.getColor(1, q2Colors)}>{this.getMember(1, q2Members)}</span>
                        </div>
                        <div ref={this.ref4thQuarter} className={styles.quaterDay + q4Name} onClick={() => this.onHalfDayClick(this.ref4thQuarter, q4Editable)}>
                            <span className={styles.memberName + this.getColor(0, q4Colors)}>{this.getMember(0, q4Members)}</span>
                            <span className={styles.memberName + this.getColor(1, q4Colors)}>{this.getMember(1, q4Members)}</span>
                        </div>
                    </div>
                </div>
            );
        }
        else {
            const [amType, pmType] = this.props.dayType;
            const halfAMName = this.getAddName(amType, editable);
            const halfPMName = this.getAddName(pmType, editable);
            const leftEditable = this.getHalfEditable(amType, editable);
            const rightEditable = this.getHalfEditable(pmType, editable);

            const [leftMembers, leftColors] = this.getMembers(0);
            const [rightMembers, rightColors] = this.getMembers(1);

            return (
                <div className={className}>
                    <span className={styles.dayText}>{day}</span>
                    <div className={styles.halfDays}>
                        <div ref={this.refLeft} className={styles.halfDay + halfAMName} onClick={() => this.onHalfDayClick(this.refLeft, leftEditable)}>
                            <span className={styles.memberName + this.getColor(0, leftColors)}>{this.getMember(0, leftMembers)}</span>
                            <span className={styles.memberName + this.getColor(1, leftColors)}>{this.getMember(1, leftMembers)}</span>
                            <span className={styles.memberName + this.getColor(2, leftColors)}>{this.getMember(2, leftMembers)}</span>
                        </div>
                        <div ref={this.refRight} className={styles.halfDay + halfPMName} onClick={() => this.onHalfDayClick(this.refRight, rightEditable)}>
                            <span className={styles.memberName + this.getColor(0, rightColors)}>{this.getMember(0, rightMembers)}</span>
                            <span className={styles.memberName + this.getColor(1, rightColors)}>{this.getMember(1, rightMembers)}</span>
                            <span className={styles.memberName + this.getColor(2, rightColors)}>{this.getMember(2, rightMembers)}</span>
                        </div>
                    </div>
                </div>
            );
        }
    }
}