import React, { Component } from 'react';
import { TMCascading } from '../Root/services/tmCascading';
import styles from './css/specialVacation.module.css';
import { VacationController } from '../Root/services/vacationController';
import { ConfirmDialog } from '../Root/confirmDialog';

export class RequestSpecialVacation extends Component {
    constructor(props) {
        super(props);

        this.props = props;

        this.refTextReason = React.createRef();
        this.refConfirm = React.createRef();
        this.refRequest = React.createRef();

        const membersHistory = this.copyMyTeamMembersHistory();
        const teamSelections = TMCascading.makeTeamSelections(membersHistory);
        //const teamSelections = TMCascading.makeTeamSelections(this.props.membersHistory);
        const teamDatas = this.initState(null, null, teamSelections, membersHistory)[0];

        if (teamDatas?.length > 0) {
            const teamData = teamDatas[0];

            if (teamData !== TMCascading.All) {
                this.initState(teamData.team.id, null, teamSelections, membersHistory);
            }
        }
    }

    copyMyTeamMembersHistory() {
        const membersHistory = { ...this.props.membersHistory };

        // 관리자로 로그인할 경우 모든 팀이 나오도록 한다.
        if (this.props.loginUser.isAdmin) {
            return membersHistory;
        }

        const rootTeam = this.getRootTeam(membersHistory.rootTeam);
        membersHistory.rootTeam = rootTeam;
        return membersHistory;
    }

    getRootTeam(team) {
        if (team === null) {
            return null;
        }

        if (team.id === this.props.loginUser.teamID) {
            return team;
        }

        for (let i = 0; i < team.childTeams.length; i++) {
            const _team = this.getRootTeam(team.childTeams[i]);

            if (_team !== null) {
                return _team;
            }
        }

        return null;
    }

    initState(selectedTeamID, selectedMemberID, teamSelections, membersHistory) {
        const [teamDatas, memberDatas, member] = this.getCurrentMemberHistory(selectedTeamID, selectedMemberID, teamSelections, membersHistory);

        let _member = member;
        let _memberID = null;

        if (_member === null) {
            const memberData = this.getFirstMemberData(memberDatas);
            _member = memberData;

            if (_member) {
                _memberID = _member.id;
            }
        }

        this.state = {
            membersHistory: membersHistory,
            teamSelections: teamSelections,
            teamDatas: teamDatas,
            memberDatas: memberDatas,
            selectedTeamID: selectedTeamID,
            selectedMemberID: _memberID,
            selectedMember: _member,
            addedMembers: [],
            days: 0,
            confirmMessages: null
        }

        return [teamDatas, memberDatas, member];
    }

    getFirstMemberData(members) {
        if (!members || members.length === 0) {
            return null;
        }

        const member = members[0];
        return member;
    }

    getCurrentMemberHistory(selectedTeamID, selectedMemberID, teamSelections, membersHistory) {
        const [teamDatas, members] = TMCascading.getTeamMembers(selectedTeamID, selectedMemberID, teamSelections, membersHistory, true);

        for (let i = 0; i < members.length; i++) {
            const member = members[i];

            if (member === TMCascading.All) {
                // 전체는 없애준다.
                members.splice(i, 1);
                break;
            }
        }

        const member = TMCascading.getMember(selectedMemberID, members);
        return [teamDatas, members, member];
    }

    onChangeTeam(event, teamDatas) {
        const selectedTeamID = parseInt(event.target.value);
        this.updateState(selectedTeamID, null);
    }

    updateState(selectedTeamID, selectedMemberID) {
        const teamSelections = [...this.state.teamSelections];
        const [teamDatas, memberDatas, member] = this.getCurrentMemberHistory(selectedTeamID, selectedMemberID, teamSelections, this.state.membersHistory);

        let _member = member;
        let _memberID = selectedMemberID;

        if (_member === null) {
            const memberData = this.getFirstMemberData(memberDatas);
            _member = memberData;

            if (_member) {
                _memberID = _member.id;
            }
        }

        this.setState({
            teamSelections: teamSelections,
            teamDatas: teamDatas,
            memberDatas: memberDatas,
            selectedTeamID: selectedTeamID,
            selectedMemberID: _memberID,
            selectedMember: _member,
            confirmMessages: null
        });
    }

    onChangeMember(event) {
        const memberID = parseInt(event.target.value);
        this.updateState(this.state.selectedTeamID, memberID);
    }

    onClickAdd() {
        if (this.state.selectedMember === null) {
            return;
        }

        if (this.findMember(this.state.selectedMember.id) === null) {
            const addedMembers = [...this.state.addedMembers];
            addedMembers.push(this.state.selectedMember);
            this.setState({ addedMembers: addedMembers });
        }
    }

    onClickDays(days) {
        if (this.props.options?.minSpecialVacationDays && this.props.options?.maxSpecialVacationDays) {
            let svDays = this.state.days + days;

            if (svDays < this.props.options.minSpecialVacationDays) {
                svDays = this.props.options.minSpecialVacationDays;
            }
            else if (svDays > this.props.options.maxSpecialVacationDays) {
                svDays = this.props.options.maxSpecialVacationDays;
            }

            this.setState({ days: svDays });
        }
        else {
            this.setState({ days: this.state.days + days });
        }
    }

    onClickRequest() {
        // 승인요청 버튼을 사용할 수 없도록 한다.
        this.setEnableRequestButton(false);


        if (this.state.days > -0.1 && this.state.days < 0.1) {
            alert("특별휴가 일수를 지정해 주세요.");
        }
        else if (this.state.addedMembers.length === 0) {
            alert("특별휴가를 부여할 대상이 지정되지 않았습니다.");
        }
        else if (this.refTextReason.current.value.trim().length === 0) {
            alert("특별휴가를 부여하는 사유를 반드시 작성해야만 합니다.");
        }
        else {
            this.showRequest();
            return;
        }

        // 승인요청 버튼을 다시 사용할 수 있도록 한다.
        this.setEnableRequestButton(true);
    }

    async showRequest() {
        if (this.refConfirm.current.classList.contains(styles.show) === false) {
            this.refConfirm.current.classList.add(styles.show);
        }

        const result = await VacationController.requestSpecialVacationManager(this.props.loginUser, this.state.days);

        if (!result) {
            alert("특별휴가 신청을 수행할수 없습니다.\r\n시스템 관리자에게 문의하세요.");
        }
        else if (result.success === false) {
            alert(result.message);
        }
        else {
            const messages = [];
            messages.push(`${this.getDays()}일의 특별휴가를 신청합니다`);
            messages.push(`휴가승인은 아래의 담당자에게 요청됩니다. 이대로 진행할까요?`);
            messages.push(`승인 담당자 : ${this.getManagers(result.managers)}`);

            this.refConfirm.current.style.height = ConfirmDialog.getHeight(messages.length);
            this.setState({ confirmMessages: messages });
        }
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
            this.setEnableRequestButton(true);
        }

        if (this.refConfirm.current.classList.contains(styles.show)) {
            this.refConfirm.current.classList.remove(styles.show);
        }

        this.setState({ confirmMessages: null });
    }

    setEnableRequestButton(enable) {
        if (enable) {
            if (this.refRequest.current) {
                this.refRequest.current.removeAttribute("disabled");
            }
        }
        else {
            if (this.refRequest.current) {
                this.refRequest.current.setAttribute("disabled", true);
            }
        }
    }

    async doRequest() {
        const reason = this.refTextReason.current.value.trim();
        const svDays = this.state.days;

        if (svDays > -0.1 && svDays < 0.1) {
            alert("특별휴가 일수를 지정해 주세요.");

            // 승인요청 버튼을 다시 사용할 수 있도록 한다.
            this.setEnableRequestButton(true);
            return;
        }
        if (reason.length === 0) {
            alert("특별휴가를 부여하는 사유를 반드시 작성해야만 합니다.");

            // 승인요청 버튼을 다시 사용할 수 있도록 한다.
            this.setEnableRequestButton(true);
            return;
        }

        const result = await VacationController.requestSpecialVacation(this.props.loginUser, this.state.addedMembers, svDays, reason);

        if (!result) {
            alert("시스템 오류가 발생하였습니다.\r\n시스템 관리자에게 문의해 주세요.");
        }
        else {
            if (result.success) {
                this.props.addVacationHistory(null);
                alert(result.message);
            }
            else {
                alert(result.message);
            }
        }

        // 승인요청 버튼을 다시 사용할 수 있도록 한다.
        this.setEnableRequestButton(true);
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

    findMember(id) {
        for (let i = 0; i < this.state.addedMembers.length; i++) {
            const member = this.state.addedMembers[i];
            if (member.id === id) {
                return member;
            }
        }

        return null;
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

    getAddedMembers() {
        const count = this.state.addedMembers.length;
        let memberNames = "";

        for (let i = 0; i < count; i++) {
            const member = this.state.addedMembers[i];

            if (memberNames.length === 0) {
                memberNames = member.name;
            }
            else {
                memberNames += ", " + member.name;
            }
        }

        return memberNames;
    }

    getDays() {
        let strDays = this.state.days.toFixed(1).toString();

        if (strDays.endsWith(".0")) {
            strDays = strDays.substring(0, strDays.length - 2);
        }

        return strDays;
    }

    render() {
        const teamSelectionContents = this.getTeamSelectionContents();
        const memberSelectionContents = this.getMemberSelectionContents();

        const confirmOption = ConfirmDialog.getYesNo();

        return (
           <>
            <div className={styles.spacialArea}>
            <h4 className={styles.specialTitle}>특별휴가요청</h4>
            <div className={styles.columnContents}>
                <div className={styles.rowContents}>
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
                <button className={styles.btnAdd} onClick={() => this.onClickAdd()}>직원추가</button>
                <div className={styles.contentsArea}>
                    <div className={styles.leftArea}>
                        <span className={styles.targetMembers}>특별휴가 부여 대상자 : {this.getAddedMembers()}</span>
                        <div className={styles.dayArea}>
                            <span className={styles.targetDays}>특별휴가 일수 : </span>
                            <span className={styles.targetDays + " " + styles.count}>{this.getDays()}일</span>
                            <div className={styles.btnDay} onClick={() => this.onClickDays(-0.5)}>
                                <i className="fas fa-angle-left"></i>
                            </div>
                            <div className={styles.btnDay} onClick={() => this.onClickDays(0.5)}>
                                <i className="fas fa-angle-right"></i>
                            </div>
                        </div>
                    </div>
                </div>
                    <div className={styles.rightArea}>
                        <label>특별휴가를 부여하는 사유를 작성해주세요.</label>
                        <textarea ref={this.refTextReason} className={styles.reasonText}></textarea>
                    </div>
                <div ref={this.refConfirm} className={styles.confirmBox}>
                    <ConfirmDialog messages={this.state.confirmMessages} option={confirmOption} onClickConfirm={this.onClickConfirm} />
                </div>
                <button ref={this.refRequest} className={styles.btnRequest} onClick={() => this.onClickRequest()}>승인요청</button>
              </div>
         </div>
        </>
        ); 
    }
}