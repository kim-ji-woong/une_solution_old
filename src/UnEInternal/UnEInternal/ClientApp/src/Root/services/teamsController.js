import { JsonManager } from './jsonManager';

export class TeamsController {
    static async displayJobLevel() {
        try {
            const response = await fetch('Teams/DisplayJobLevel');
            const datas = await response.text();

            return datas;
        }
        catch (e) {
            console.log(e);
        }

        return null;
    }

    static async displayRegular() {
        try {
            const response = await fetch('Teams/DisplayRegular');
            const datas = await response.json();

            return datas;
        }
        catch (e) {
            console.log(e);
        }

        return null;
    }

    static async displayRegularMember(teamID) {
        try {
            const RegularTeam = {
                ID: teamID
            }

            const response = await fetch('Teams/DisplayRegularMember', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json;charset=utf-8'
                },
                body: JSON.stringify(RegularTeam)
            });
            const arrMembers = await response.text();

            return arrMembers;
        }
        catch (e) {
            console.log(e);
        }

        return null;
    }

    static async saveMember(members) {
        try {
            const jsonData = [];
            for (var i = 0; i < members.length; i++) {
                if (members[i].change === 1) {
                    const msg = await this.IsValid(members[i]);
                    if (msg.length > 0) {
                        alert(msg);
                        return false;
                    }
                    jsonData.push(members[i]);
                    members[i].change = 0;
                }
            }

            await fetch('Teams/Save', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ data: jsonData })
            });

            return true;
        }
        catch (e) {
            console.log(e);

            // 실패해서 저장하지 않았으므로 change 1로 다시 돌려놓는다
            for (var i = 0; i < members.length; i++) {
                members[i].change = 1;
            }
            return false;
        }
    }

    static async IsValid(memberInfo) {
        if (memberInfo.RegularTeam.Name === undefined || memberInfo.RegularTeam.Name === "") {
            return '팀 정보를 입력하세요.';
        }
        if (memberInfo.CompanyMember.Name === undefined || memberInfo.CompanyMember.Name === "") {
            return '직원 이름을 입력하세요.';
        }
        if (memberInfo.JobLevel.LevelName === undefined || memberInfo.JobLevel.LevelName === "") {
            return '직급을 입력하세요.';
        }
        if (memberInfo.CompanyMember.PhoneNumber === undefined || memberInfo.CompanyMember.PhoneNumber === "") {
            return '휴대전화번호를 입력하세요.';
        }
        else {
            var re = /^\d{3}\d{3,4}\d{4}$/;
            const phoneValid = re.test(memberInfo.CompanyMember.PhoneNumber);
            if (!phoneValid) {
                return '핸드폰 형식을 확인하세요. (01011112222)';
            }
        }
        if (memberInfo.StartDate === undefined || memberInfo.StartDate === "") {
            return '입사일을 입력하세요.';
        }
        else {
            var re = /[0-9]{4}-[0-9]{2}-[0-9]{2}/;
            const dateValid = re.test(memberInfo.StartDate);
            if (!dateValid) {
                return '날짜 형식을 확인하세요. (2021-01-01)';
            }
        }
        if (memberInfo.CompanyMember.UserID === undefined || memberInfo.CompanyMember.UserID === "") {
            return 'ID를 입력하세요.';
        }

        return '';
    }

    static async deleteMember(members) {
        try {
            await fetch('Teams/DeleteMember', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ data: members })
            });

            console.log('deletemember end');
        }
        catch (e) {
            console.log(e);
        }
    }

    static async saveTeam(id, name, parentTeamID) {
        try {
            await fetch('Teams/SaveTeam', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ ID: id, Name: name, ParentTeamID: parentTeamID })
            });
        }
        catch (e) {
            console.log(e);
        }
    }

    static async deleteTeam(team) {
        try {
            var deleteTeams = new Array();
            deleteTeams.push(team);
            await this.pushTeam(team, deleteTeams);            

            await fetch('Teams/DeleteTeam', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ data: deleteTeams })
            });
        }
        catch (e) {
            console.log(e);
        }
    }

    static async pushTeam(team, array) {
        if (team.children != undefined) {
            for (var i = 0; i < team.children.length; i++) {
                array.push(team.children[i]);
                this.pushTeam(team.children[i], array);
            }
        }
    }

    static async checkAdminLength(teamID) {
        try {
            const RegularTeamID = {
                ID: teamID
            }

            const response = await fetch('Teams/CheckAdminLength', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json;charset=utf-8'
                },
                body: JSON.stringify(RegularTeamID)
            });
            let length = await response.text();

            return length;
        }
        catch (e) {
            console.log(e);
        }

        return null;
    }
}