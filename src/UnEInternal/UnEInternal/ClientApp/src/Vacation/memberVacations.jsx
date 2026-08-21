import React, { Component } from 'react';
import { TMCascading } from '../Root/services/tmCascading';
//import { Calendar } from './Calendar/calendar';
//import { VacationManager } from '../Root/services/vacationManager';
import styles from './css/memberVacation.module.css';
import { MyVacations } from './myVacations';

export class MemberVacations extends Component {
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
        /*const [teamDatas, memberDatas, member, memberHistory] = this.getCurrentMemberHistory(null, null, teamSelections);

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
            selectedTeamID: null,
            selectedMemberID: _memberID,
            selectedMember: _member,
            selectedMemberHistory: _memberHistory
        }*/
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
            selectedMemberHistory: _memberHistory
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
            selectedMemberHistory: _memberHistory
        });
    }

    onChangeMember(event) {
        const memberID = parseInt(event.target.value);
        this.updateState(this.state.selectedTeamID, memberID);
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

    getMemberSelectionContents() {
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
    }

    render() {
        const teamSelectionContents = this.getTeamSelectionContents();
        const memberSelectionContents = this.getMemberSelectionContents();

        return (
            <div className={styles.columnContents}>
                <div className={styles.teamMemberBox}>
                    {
                        teamSelectionContents.map(contents => (
                            contents
                        ))
                    }
                    {
                        memberSelectionContents
                    }
                </div>
                <div>
                {
                    this.state.selectedMember && this.state.selectedMemberHistory && (
                            <MyVacations loginUser={this.state.selectedMember} holidays={this.props.holidays} history={this.state.selectedMemberHistory} year={this.state.year} month={this.state.month} options={this.props.options} onChangeYear={this.onChangeYear} onChangeMonth={this.onChangeMonth} getNextYearHistory={this.props.getNextYearHistory} getLastYearHistory={this.props.getLastYearHistory} />
                )}
                </div>
            </div>
        );
    }
}