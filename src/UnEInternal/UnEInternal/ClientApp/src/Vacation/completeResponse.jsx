import React, { Component } from 'react';
import styles from './css/response.module.css';
//import { ConfirmDialog } from '../Root/confirmDialog';
//import { VacationController } from '../Root/services/vacationController';
import Paginate from './paginate';
import { TMCascading } from '../Root/services/tmCascading';

export class CompleteResponse extends Component {
    static Permit = 0;
    static Deny = 1;
    static Processing = 2;
    static Timeout = 3;
    static Cancel = 4;
    static None = 5;

    constructor(props) {
        super(props);

        this.props = props;

        const date = new Date();
        const year = date.getFullYear();
        const teamSelections = TMCascading.makeTeamSelections(this.props.membersHistory);
        const teamDatas = this.initState(year, null, null, teamSelections)[2];
        /*const [managerRequest, years, teamDatas, memberDatas] = this.getCurrentManagerRequests(year, null, null, teamSelections);

        this.state = {
            currentYear: year,
            years: years,
            teamSelections: teamSelections,
            teamDatas: teamDatas,
            memberDatas: memberDatas,
            selectedTeamID: null,
            selectedMemberID: null,
            managerRequest: managerRequest,
            allRequest: managerRequest.length,  // 전체 요청 갯수  
            page: 1,                            // 현재 페이지
            ongPage: 10                         // 한 페이지에 보여줄 요청의 수.
        }*/

        if (teamDatas?.length > 0) {
            const teamData = teamDatas[0];

            if (teamData !== TMCascading.All) {
                this.initState(year, teamData.team.id, null, teamSelections);
            }
        }
    }

    initState(year, selectedTeamID, selectedMemberID, teamSelections) {
        const [managerRequest, years, teamDatas, memberDatas] = this.getCurrentManagerRequests(year, selectedTeamID, selectedMemberID, teamSelections);

        this.state = {
            currentYear: year,
            years: years,
            teamSelections: teamSelections,
            teamDatas: teamDatas,
            memberDatas: memberDatas,
            selectedTeamID: selectedTeamID,
            selectedMemberID: selectedMemberID,
            managerRequest: managerRequest,
            allRequest: managerRequest.length,  // 전체 요청 갯수  
            page: 1,                            // 현재 페이지
            ongPage: 10                         // 한 페이지에 보여줄 요청의 수.
        };

        return [managerRequest, years, teamDatas, memberDatas];
    }

    getCurrentManagerRequests(year, selectedTeamID, selectedMemberID, teamSelections) {
        const requests = [];
        const years = [];
        const count = this.props.managerRequest?.completedRequests?.length;

        const [teamDatas, members] = TMCascading.getTeamMembers(selectedTeamID, selectedMemberID, teamSelections, this.props.membersHistory, false);

        if (!count || count === 0) {
            years.push(year);
            return [requests, years, teamDatas, members];
        }

        let firstYear = 0;
        let lastYear = 0;

        for (let i = 0; i < count; i++) {
            const request = this.props.managerRequest.completedRequests[i];

            if (request.responseYear === year) {
                if (((!selectedMemberID || selectedMemberID < 0) && TMCascading.getMember(request.requestMember.id, members) !== null) ||
                    selectedMemberID === request.requestMember.id) {
                    requests.push(request);
                }
            }

            if (i === 0) {
                firstYear = request.responseYear;
                lastYear = request.responseYear;
            }
            else {
                if (request.responseYear < firstYear) {
                    firstYear = request.responseYear;
                }

                if (request.responseYear > lastYear) {
                    lastYear = request.responseYear;
                }
            }
        }

        for (let i = firstYear; i <= lastYear; i++) {
            years.push(i);
        }

        return [requests.reverse(), years, teamDatas, members];
    }

    getRequestTime(request) {
        return `${request.requestMonth}월 ${request.requestDay}일 ${request.requestHour}시 ${request.requestMinute}분`;
    }

    getResponseTime(request) {
        return `${request.responseMonth}월 ${request.responseDay}일 ${request.responseHour}시 ${request.responseMinute}분`;
    }

    getFloatString(data) {
        const str = data.toFixed(1);

        if (str.endsWith(".0")) {
            return str.substring(0, str.length - 2);
        }

        return str;
    }

    getPeriod(request) {
        if (request.period.includes('~')) {
            return request.period + "(" + this.getFloatString(request.days) + "일)";
        }

        return request.period;
    }

    getResponseResult(request) {
        if (request.response === CompleteResponse.Permit) {
            return "승인";
        }
        else if (request.response === CompleteResponse.Deny) {
            return "거부";
        }
        else if (request.response === CompleteResponse.Processing) {
            return "처리중";
        }
        else if (request.response === CompleteResponse.Timeout) {
            return "입력안함";
        }
        else if (request.response === CompleteResponse.Cancel) {
            return "승인후 취소";
        }

        return "";
    }

    pageChange = (pageNum) => {
        this.setState({ page: pageNum });
        return;
    }

    onChangeYear = (event) => {
        const year = parseInt(event.target.value);
        const teamSelections = [...this.state.teamSelections];
        const selectedTeamID = this.state.selectedTeamID;
        const selectedMemberID = this.state.selectedMemberID;
        const [managerRequest, years, teamDatas, memberDatas] = this.getCurrentManagerRequests(year, selectedTeamID, selectedMemberID, teamSelections);

        this.setState({
            currentYear: year,
            years: years,
            teamSelections: teamSelections,
            teamDatas: teamDatas,
            memberDatas: memberDatas,
            selectedTeamID: selectedTeamID,
            selectedMemberID: selectedMemberID,
            managerRequest: managerRequest,
            allRequest: managerRequest.length
        });
    }

    onChangeTeam(event, teamDatas) {
        const selectedTeamID = parseInt(event.target.value);
        const teamCount = teamDatas.length;

        for (let i = 1; i < teamCount; i++) {
            const teamData = teamDatas[i];

            if (teamData.team.id === selectedTeamID) {
                const year = this.state.currentYear;
                const teamSelections = [...this.state.teamSelections];
                const [managerRequest, years, teamDatas, memberDatas] = this.getCurrentManagerRequests(year, selectedTeamID, null, teamSelections);

                this.setState({
                    currentYear: year,
                    years: years,
                    teamSelections: teamSelections,
                    teamDatas: teamDatas,
                    memberDatas: memberDatas,
                    selectedTeamID: selectedTeamID,
                    selectedMemberID: null,
                    managerRequest: managerRequest,
                    allRequest: managerRequest.length
                });

                return;
            }
        }

        const _year = this.state.currentYear;
        const _teamSelections = [...this.state.teamSelections];
        const [_managerRequest, _years, _teamDatas, _memberDatas] = this.getCurrentManagerRequests(_year, selectedTeamID, null, _teamSelections);

        this.setState({
            currentYear: _year,
            years: _years,
            teamSelections: _teamSelections,
            teamDatas: _teamDatas,
            memberDatas: _memberDatas,
            selectedTeamID: selectedTeamID,
            selectedMemberID: null,
            managerRequest: _managerRequest,
            allRequest: _managerRequest.length
        });
    }

    onChangeMember(event) {
        const memberID = parseInt(event.target.value);
        const year = this.state.currentYear;
        const selectedTeamID = this.state.selectedTeamID;
        const teamSelections = [...this.state.teamSelections];
        const [managerRequest, years, teamDatas, memberDatas] = this.getCurrentManagerRequests(year, selectedTeamID, memberID, teamSelections);

        this.setState({
            currentYear: year,
            years: years,
            teamSelections: teamSelections,
            teamDatas: teamDatas,
            memberDatas: memberDatas,
            selectedTeamID: selectedTeamID,
            selectedMemberID: memberID,
            managerRequest: managerRequest,
            allRequest: managerRequest.length
        });
    }

    getRowContent() {
        const rowContent = [];

        let min = (this.state.page - 1) * this.state.ongPage;
        let max = min + this.state.ongPage;
        if (max > this.state.allRequest) {
            max = this.state.allRequest;
        }

        for (let i = min; i < max; i++) {
            const request = this.state.managerRequest[i];

            if (request) {
                rowContent.push(
                    <tr key={request.requestTime + "_" + i}>
                        <td className={styles.thickTD}>{request.requestMember.name + " " + request.requestMember.level}</td>
                        <td className={styles.thickTD}>{this.getRequestTime(request)}</td>
                        <td className={styles.thickTD}>{this.getPeriod(request)}</td>
                        <td className={styles.thickTD}>{this.getResponseTime(request)}</td>
                        <td className={styles.thickTD}>{this.getResponseResult(request)}</td>
                    </tr>
                );
            }
        }

        return rowContent;
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
        const rowContent = this.getRowContent();
        const teamSelectionContents = this.getTeamSelectionContents();
        const memberSelectionContents = this.getMemberSelectionContents();
        
        return (
            <div className={styles.responseAreaCom}>
                <div className={styles.titleBox}>
                    <h4 className={styles.completeTitle}>결재 이력</h4>
                    <div className={styles.optionArea}>
                        <span className={styles.textLeft}>결재가 완료된 목록들입니다.</span>
                        <div className={styles.yearBox}>
                            <select className={styles.yearCombobox} value={this.state.currentYear} onChange={this.onChangeYear}>
                            {
                                this.state.years.map(year => (
                                    <option key={year} value={year}>{year}</option>
                                ))
                            }
                            </select>
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
                        </div>
                    </div>
                </div>
                {this.props.managerRequest && (
                    <table className={styles.stripedTableResult} aria-labelledby="tabelLabel">
                        <thead>
                            <tr>
                                <th>요청자</th>
                                <th>요청일시</th>
                                <th>기간</th>
                                <th>처리일시</th>
                                <th>처리결과</th>
                            </tr>
                        </thead>
                        <tbody ref={this.refTBody}>
                            {rowContent}
                        </tbody>
                    </table>
                )}

                <Paginate page={this.state.page} allRequest={this.state.allRequest} onChange={this.pageChange} />
            </div>
        );
    }
}