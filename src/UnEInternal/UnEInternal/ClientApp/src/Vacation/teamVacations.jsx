import React, { Component } from 'react';
import { TMCascading } from '../Root/services/tmCascading';
import styles from './css/teamVacation.module.css';
import { TeamCalendar } from './Calendar/teamCalendar';
import { Calendar } from './Calendar/calendar';

export class TeamVacations extends Component {
    constructor(props) {
        super(props);

        this.props = props;

        const teamSelections = TMCascading.makeTeamSelections(this.props.membersHistory);
        const teamDatas = this.initState(null, null, teamSelections)[0];

        if (teamDatas?.length > 0) {
            const teamData = teamDatas[0];

            if (teamData !== TMCascading.All) {
                this.initState(teamData.team.id, null, teamSelections);
            }
        }

        this.refCheckReservation = React.createRef();
        this.refCheckUsed = React.createRef();
        this.refCheckWait = React.createRef();
    }

    componentDidMount() {
        this.refCheckReservation.current.checked = this.state.showReservation;
        this.refCheckUsed.current.checked = this.state.showUsed;
        this.refCheckWait.current.checked = this.state.showWait;
    }

    initState(selectedTeamID, selectedMemberID, teamSelections) {
        const [teamDatas, memberDatas, member, memberHistory] = this.getCurrentMemberHistory(selectedTeamID, selectedMemberID, teamSelections);

        let _member = member;
        let _memberHistory = memberHistory;
        let _memberID = null;

        if (_member === null) {
            const [memberData, memberHistoryData] = this.getFirstMemberData(memberDatas);
            _member = memberData;
            _memberHistory = memberHistoryData;

            if (_member) {
                _memberID = _member.id;
            }
        }

        const date = new Date();

        this.state = {
            year: date.getFullYear(),
            month: date.getMonth() + 1,
            teamSelections: teamSelections,
            teamDatas: teamDatas,
            memberDatas: memberDatas,
            selectedTeamID: selectedTeamID,
            selectedMemberID: _memberID,
            selectedMember: _member,
            selectedMemberHistory: _memberHistory,
            showReservation: true,
            showUsed: true,
            showWait: true
        }

        return [teamDatas, memberDatas, member, memberHistory];
    }

    getFirstMemberData(members) {
        if (!members || members.length === 0) {
            return [null, null];
        }

        const member = members[0];
        const memberHistory = this.props.membersHistory.memberHistories[member.id];
        return [member, memberHistory];
    }

    getCurrentMemberHistory(selectedTeamID, selectedMemberID, teamSelections) {
        const [teamDatas, members] = TMCascading.getTeamMembers(selectedTeamID, selectedMemberID, teamSelections, this.props.membersHistory, true);

        for (let i = 0; i < members.length; i++) {
            const member = members[i];

            if (member === TMCascading.All) {
                // 전체는 없애준다.
                members.splice(i, 1);
                break;
            }
        }

        const member = TMCascading.getMember(selectedMemberID, members);
        const memberHistory = member === null ? null : this.props.membersHistory.memberHistories[member.id];
        return [teamDatas, members, member, memberHistory];
    }

    onChangeYear = (goNext) => {
        if (goNext) {
            this.setState({ year: this.state.year + 1, month: this.state.month });
        }
        else {
            this.setState({ year: this.state.year - 1, month: this.state.month });
        }
    }

    onChangeMonth = (goNext) => {
        let year = this.state.year;
        let month = 0;

        if (goNext) {
            month = this.state.month + 1;

            if (month > 12) {
                year++;
                month = 1;
            }
        }
        else {
            month = this.state.month - 1;

            if (month <= 0) {
                month = 12;
                year--;
            }
        }

        this.setState({ year: year, month: month });
    }

    onChangeTeam(event, teamDatas) {
        const selectedTeamID = parseInt(event.target.value);
        this.updateState(selectedTeamID, null);
    }

    updateState(selectedTeamID, selectedMemberID) {
        const teamSelections = [...this.state.teamSelections];
        const [teamDatas, memberDatas, member, memberHistory] = this.getCurrentMemberHistory(selectedTeamID, selectedMemberID, teamSelections);

        let _member = member;
        let _memberHistory = memberHistory;
        let _memberID = selectedMemberID;

        if (_member === null) {
            const [memberData, memberHistoryData] = this.getFirstMemberData(memberDatas);
            _member = memberData;
            _memberHistory = memberHistoryData;

            if (_member) {
                _memberID = _member.id;
            }
        }

        this.setState({
            year: this.state.year,
            month: this.state.month,
            teamSelections: teamSelections,
            teamDatas: teamDatas,
            memberDatas: memberDatas,
            selectedTeamID: selectedTeamID,
            selectedMemberID: _memberID,
            selectedMember: _member,
            selectedMemberHistory: _memberHistory,
            showReservation: this.refCheckReservation.current.checked,
            showUsed: this.refCheckUsed.current.checked,
            showWait: this.refCheckWait.current.checked
        });
    }

    onChangeMember(event) {
        const memberID = parseInt(event.target.value);
        this.updateState(this.state.selectedTeamID, memberID);
    }

    onClickCheckBox(checkBox) {
        if (this.refCheckReservation === checkBox) {
            this.setState({ showReservation: checkBox.current.checked});
        }
        else if (this.refCheckUsed === checkBox) {
            this.setState({ showUsed: checkBox.current.checked });
        }
        else if (this.refCheckWait === checkBox) {
            this.setState({ showWait: checkBox.current.checked });
        }
    }

    getTeamMemberSelections(teamDatas, depth, selectedValue) {
        return (
            <select key={`teamSelection_${depth}`} className={styles.teamCombobox} value={selectedValue} onChange={(event) => this.onChangeTeam(event, teamDatas)}>
                {
                    teamDatas.map(teamData => (
                        <option key={`team_${depth}_${TMCascading.getTeamName(teamData)}`} value={TMCascading.getTeamID(teamData, depth)}>{TMCascading.getTeamName(teamData)}</option>
                    ))
                }
            </select>
        );
    }

    getTeamSelectionContents() {
        let teamDatas = this.state.teamDatas;
        const teamSelectionContents = [];
        const depthCount = this.state.teamSelections.length;

        for (let i = 0; i < depthCount; i++) {
            const index = this.state.teamSelections[i];
            const selectedTeam = teamDatas[index];

            if (!selectedTeam) {
                break;
            }

            const selectedValue = TMCascading.getTeamID(selectedTeam, i);

            teamSelectionContents.push(this.getTeamMemberSelections(teamDatas, i, selectedValue));

            if (selectedTeam === TMCascading.All) {
                break;
            }
            else {
                teamDatas = selectedTeam.childTeams;
            }
        }

        return teamSelectionContents;
    }

    /*getMemberSelectionContents() {
        if (!this.state.memberDatas || this.state.memberDatas.length === 0) {
            return <></>
        }

        const selectedMemberID = !this.state.selectedMemberID ? -1 : this.state.selectedMemberID;

        return (
            <select className={styles.memberCombobox} value={selectedMemberID} onChange={(event) => this.onChangeMember(event)}>
                {
                    this.state.memberDatas.map(member => (
                        <option key={TMCascading.getMemberID(member)} value={TMCascading.getMemberID(member)}>{TMCascading.getMemberName(member)}</option>
                    ))
                }
            </select>
        );
    }*/
    getFromTo() {
        if (!this.state.memberDatas || this.state.memberDatas.length === 0) {
            return null;
        }

        const date = new Date();

        const memberCount = this.state.memberDatas.length;
        const fromTo = {
            from: {
                year: date.getFullYear(),
                month: date.getMonth() + 1
            },
            to: {
                year: date.getFullYear(),
                month: date.getMonth() + 1
            }
        };

        for (let i = 0; i < memberCount; i++) {
            const member = this.state.memberDatas[i];
            const _fromTo = Calendar.getFromToCalendar(member);

            if (_fromTo === null) {
                continue;
            }

            if (fromTo.from.year === null) {
                fromTo.from = _fromTo.from;
                fromTo.to = _fromTo.to;
            }
            else {
                const date1 = fromTo.from.year * 100 + fromTo.from.month;
                const date2 = _fromTo.from.year * 100 + _fromTo.from.month;

                if (date1 > date2) {
                    fromTo.from.year = _fromTo.from.year;
                    fromTo.from.month = _fromTo.from.month;
                }

                const date3 = fromTo.to.year * 100 + fromTo.to.month;
                const date4 = _fromTo.to.year * 100 + _fromTo.to.month;

                if (date3 < date4) {
                    fromTo.to.year = _fromTo.to.year;
                    fromTo.to.month = _fromTo.to.month;
                }
            }
        }

        return fromTo;
    }

    render() {
        const teamSelectionContents = this.getTeamSelectionContents();
        //const memberSelectionContents = this.getMemberSelectionContents();

        const fromTo = this.getFromTo();

        return (
            <>
            <div className={styles.teamArea}> 
                <h4 className={styles.teamTitle}>팀별 휴가조회</h4>
             <div className={styles.columnContents}>
                <div className={styles.teamMemberBox}>
                    {
                        teamSelectionContents.map(contents => (
                            contents
                        ))
                    }
                </div>
                <div className={styles.checkBoxArea}>
                    <div className={styles.checkBox}>
                        <input ref={this.refCheckReservation} name="showReservation" type="checkbox" value="true" onClick={() => this.onClickCheckBox(this.refCheckReservation)} />
                        <span className={styles.checkBoxText + " " + styles.reservation}>&nbsp;승인된 휴가</span>
                    </div>
                    <div className={styles.checkBox}>
                        <input ref={this.refCheckUsed} name="showUsed" type="checkbox" value="true" onClick={() => this.onClickCheckBox(this.refCheckUsed)} />
                        <span className={styles.checkBoxText + " " + styles.used}>&nbsp;사용한 휴가</span>
                    </div>
                    <div className={styles.checkBox}>
                        <input ref={this.refCheckWait} name="showWait" type="checkbox" value="true" onClick={() => this.onClickCheckBox(this.refCheckWait)} />
                        <span className={styles.checkBoxText + " " + styles.wait}>&nbsp;승인 대기중인 휴가</span>
                    </div>
                </div>
                <div>
                {
                    this.state.memberDatas && this.state.memberDatas.length > 0 && (
                        <TeamCalendar fromTo={fromTo} year={this.state.year} month={this.state.month} holidays={this.props.holidays} onChangeYear={this.onChangeYear} onChangeMonth={this.onChangeMonth} memberDatas={this.state.memberDatas} membersHistory={this.props.membersHistory} options={this.props.options} showReservation={this.state.showReservation} showUsed={this.state.showUsed} showWait={this.state.showWait} />
                )}
                </div>
             </div>
            </div>
            </>
        );
    }
}