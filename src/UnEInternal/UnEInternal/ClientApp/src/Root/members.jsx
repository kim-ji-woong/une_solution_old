import React, { Component } from 'react';
import { TeamsController } from './services/teamsController';

class ColRegularMember extends Component {
    constructor(props) {
        super(props);
        this.state =
        {
            member: null,
            teamNameEdit: false,
            isTeamLeaderEdit: false,
            isAdminEdit: false,
            nameEdit: false,            
            jobLevelEdit: false,
            phoneNumEdit: false,
            startDateEdit: false,
            userIDEdit: false,
            jobLevels: null
        };

        this.props = props;
        this.state.jobLevels = this.props.jobLevels;
        this.state.member = this.props.member;        
    }

    onFristClick = (e) => {
        if (!this.props.isEditMode)
            return;

        const eID = e.target.id;
        if (eID === "Name") {
            if (this.state.nameEdit === false) {
                this.setState({ teamNameEdit: false, nameEdit: true, IsTeamLeader: false, isAdminEdit: false, jobLevelEdit: false, phoneNumEdit: false, startDateEdit: false, userIDEdit: false });
            }
        }
        else if (eID === "JobLevel") {
            if (this.state.jobLevelEdit === false) {
                this.setState({ teamNameEdit: false, nameEdit: false, IsTeamLeader: false, isAdminEdit: false, jobLevelEdit: true, phoneNumEdit: false, startDateEdit: false, userIDEdit: false });
            }
        }
        else if (eID === "PhoneNum") {
            if (this.state.phoneNumEdit === false) {
                this.setState({ teamNameEdit: false, nameEdit: false, IsTeamLeader: false, isAdminEdit: false, jobLevelEdit: false, phoneNumEdit: true, startDateEdit: false, userIDEdit: false });
            }
        }
        else if (eID === "StartDate") {
            if (this.state.member.StartDate === undefined || this.state.member.StartDate === "") {
                if (this.state.startDateEdit === false) {
                    this.setState({ teamNameEdit: false, nameEdit: false, IsTeamLeader: false, isAdminEdit: false, jobLevelEdit: false, phoneNumEdit: false, startDateEdit: true, userIDEdit: false });
                }
            }
            else {
                var re = /[0-9]{4}-[0-9]{2}-[0-9]{2}/;
                const dateValid = re.test(this.state.member.StartDate);

                if (!dateValid) {
                    if (this.state.startDateEdit === false) {
                        this.setState({ teamNameEdit: false, nameEdit: false, IsTeamLeader: false, isAdminEdit: false, jobLevelEdit: false, phoneNumEdit: false, startDateEdit: true, userIDEdit: false });
                    }
                }
            }
        }
        else if (eID === "UserID") {
            if (this.state.member.CompanyMember.UserID === undefined || this.state.member.CompanyMember.UserID === "") {
                if (this.state.userIDEdit === false) {
                    this.setState({ teamNameEdit: false, nameEdit: false, IsTeamLeader: false, isAdminEdit: false, jobLevelEdit: false, phoneNumEdit: false, startDateEdit: false, userIDEdit: true });
                }
            }
        }
    }

    onChangedIsTeamLeader(checked) {
        if (!this.props.isEditMode)
            return;

        if (checked !== this.state.member.CompanyMember.IsTeamLeader) {
            if (checked) {
                this.props.checkTeamLeader(this.state.member);
            }

            const newMember = this.state.member;
            newMember.CompanyMember.IsTeamLeader = checked;

            this.setState({ member: newMember });
            this.props.memberInfoChange(newMember, this.props.index);
        }
    }
    async onChangedIsAdmin(target, checked) {
        if (!this.props.isEditMode)
            return;

        if (checked !== this.state.member.CompanyMember.IsAdmin) {

            if (!checked) { //관리자가 최소 1명은 있어야 한다.
                var length1 = this.props.checkAdminLength(this.state.member.CompanyMember.ID);
                let length2 = await TeamsController.checkAdminLength(this.state.member.RegularTeam.ID);
                if (!length1 && Number(length2) === 0) {
                    alert('관리자는 최소 1명 이상이어야 합니다.');
                    return;
                }
            }

            const newMember = this.state.member;
            newMember.CompanyMember.IsAdmin = checked;

            this.setState({ member: newMember });
            this.props.memberInfoChange(newMember, this.props.index);
        }
    }

    onBlurTest(e) {
        console.log(e.target.id);
        console.log(e.target.ivalue);

        const eID = e.target.id;
        const eValue = e.target.value;

        const newMember = this.state.member;

        if (eID === "Name") {
            if (eValue !== this.state.member.CompanyMember.Name) {                
                newMember.CompanyMember.Name = eValue;
            }
        }
        else if (eID === "JobLevel") {
            if (eValue !== this.state.member.JobLevel.LevelName) {
                newMember.JobLevel.LevelName = eValue;
            }
        }
        else if (eID === "PhoneNum") {
            if (eValue !== this.state.member.CompanyMember.PhoneNumber) {
                newMember.CompanyMember.PhoneNumber = eValue;
            }
        }
        else if (eID === "StartDate") {
            if (eValue !== this.state.member.StartDate) {
                newMember.StartDate = eValue;
            }
        }
        else if (eID === "UserID") {
            if (eValue !== this.state.member.UserID) {                
                newMember.CompanyMember.UserID = eValue;
            }
        }
        else {
            return;
        }

        this.setState({ member: newMember, teamNameEdit: false, nameEdit: false, IsTeamLeader: false, isAdminEdit: false, jobLevelEdit: false, phoneNumEdit: false, startDateEdit: false, userIDEdit: false });
        this.props.memberInfoChange(newMember, this.props.index);
    }

    onChangeJobLevel(e) {
        const eText = e.target.selectedOptions[0].text;
        if (eText !== this.state.member.JobLevel.LevelName) {
            const newMember = this.state.member;
            newMember.JobLevel.ID = Number(e.target.value);
            newMember.JobLevel.LevelName = eText;            

            this.setState({ member: newMember, teamNameEdit: false, nameEdit: false, IsTeamLeader: false, isAdminEdit: false, jobLevelEdit: false, phoneNumEdit: false, startDateEdit: false, userIDEdit: false });
            this.props.memberInfoChange(newMember, this.props.index);
        }
    }

    handleKeyPress = (e) => {
        if (e.key === "Enter") {
            this.onBlurTest(e);
        }
    }

    render() {
        return (
            <>
                <td>{this.props.member.RegularTeam.Name}</td>
                {/*직원명*/
                    (this.state.nameEdit)
                        ? <td>
                            <input type="text" id="Name"
                                defaultValue={this.props.member.CompanyMember.Name}
                                onBlur={(e) => this.onBlurTest(e)}
                                onKeyPress={this.handleKeyPress}
                                autoFocus />
                          </td>
                        : <td id="Name" onClick={(e) => this.onFristClick(e)}>{this.props.member.CompanyMember.Name}</td>
                }
                {/*직급*/
                    //(this.state.jobLevelEdit)
                    //    ? <td><input type="text" id="JobLevel" defaultValue={this.props.member.JobLevel.LevelName} onBlur={(e) => this.onBlurTest(e)}/></td>
                    //    : <td id="JobLevel" onClick={(e) => this.onFristClick(e)}>{this.props.member.JobLevel.LevelName}</td>

                    (this.state.jobLevelEdit)
                        ? <td>
                            <select name="level" onChange={(e) => this.onChangeJobLevel(e)} autoFocus>
                            {                                    
                                this.props.jobLevels.map((level, index) =>
                                (
                                        (this.props.member.JobLevel.LevelName === level.levelName)
                                        ? <option key={level.id} value={level.id} selected>{level.levelName}</option> 
                                        : <option key={level.id} value={level.id}>{level.levelName}</option>
                                ))
                            }
                            </select>
                          </td>
                        : <td id="JobLevel" onClick={(e) => this.onFristClick(e)}>{this.props.member.JobLevel.LevelName}</td>
                }
                <td><input type="checkbox" checked={this.props.member.CompanyMember.IsTeamLeader} onChange={(e) => this.onChangedIsTeamLeader(e.target.checked)} /></td>
                <td><input type="checkbox" checked={this.props.member.CompanyMember.IsAdmin} onChange={(e) => this.onChangedIsAdmin(e.target, e.target.checked)} /></td>
                {/*핸드폰*/
                    (this.state.phoneNumEdit)
                        ? <td><input type="text" id="PhoneNum" defaultValue={this.props.member.CompanyMember.PhoneNumber} onBlur={(e) => this.onBlurTest(e)} onKeyPress={this.handleKeyPress} autoFocus/></td>
                        : <td id="PhoneNum" onClick={(e) => this.onFristClick(e)}>{this.props.member.CompanyMember.PhoneNumber}</td>
                }
                {/*입사일*/
                    (this.state.startDateEdit)
                        ? <td><input type="text" id="StartDate" defaultValue={this.props.member.StartDate} onBlur={(e) => this.onBlurTest(e)} onKeyPress={this.handleKeyPress} autoFocus /></td>
                        : <td id="StartDate" onClick={(e) => this.onFristClick(e)}>{this.props.member.StartDate}</td>
                }
                {/*로그인ID*/
                    (this.state.userIDEdit)
                        ? <td><input type="text" id="UserID" defaultValue={this.props.member.CompanyMember.UserID} onBlur={(e) => this.onBlurTest(e)} onKeyPress={this.handleKeyPress} autoFocus /></td>
                        : <td id="UserID" onClick={(e) => this.onFristClick(e)}>{this.props.member.CompanyMember.UserID}</td>
                }
            </>
        );
    }
}

export default ColRegularMember;