// team, member
export class TMCascading {
    static All = "전체";

    static getTeamMembers(selectedTeamID, selectedMemberID, teamSelections, membersHistory) {
        const rootTeamDatas = TMCascading.getTeamDatas(membersHistory.rootTeam, selectedTeamID, teamSelections, 0, 0, membersHistory);

        const teamDatas = [/*TMCascading.All, */rootTeamDatas];
        const members = TMCascading.getMemberDatas(teamDatas, teamSelections);

        return [teamDatas, members];
    }

    static getTeamDatas(team, selectedTeamID, teamSelections, depth, teamIndex, membersHistory) {
        const teamDatas = {
            team: team,
            members: {},
            childTeams: []
        };

        let findTeam = false;

        if (team.id === selectedTeamID) {
            teamSelections[depth] = teamIndex;

            for (let i = depth + 1; i < teamSelections.length; i++) {
                teamSelections[i] = 0;
            }

            findTeam = true;
        }
        else if (selectedTeamID === (depth + 1) * (-1)) {
            for (let i = depth; i < teamSelections.length; i++) {
                teamSelections[i] = 0;
            }
        }

        const memberCount = team.members.length;

        if (memberCount > 1) {
            teamDatas.members[TMCascading.All] = "";
        }

        for (let i = 0; i < memberCount; i++) {
            const member = team.members[i];
            teamDatas.members[member.id] = membersHistory.memberHistories[member.id];
        }

        const childTeamCount = team.childTeams.length;

        if (selectedTeamID && childTeamCount > 1) {
            // 배열의 제일 앞에 넣는다.
            teamDatas.childTeams.unshift(TMCascading.All);
        }

        if (selectedTeamID) {
            for (let i = 0; i < childTeamCount; i++) {
                const childTeam = team.childTeams[i];

                const childTeamData = TMCascading.getTeamDatas(childTeam, findTeam ? null : selectedTeamID, teamSelections, depth + 1, i + 1, membersHistory);
                teamDatas.childTeams.push(childTeamData);
            }
        }

        return teamDatas;
    }

    static getMemberDatas(teamDatas, teamSelections) {
        let memberDatas = [];
        let prevSelectedTeam = null;
        const depthCount = teamSelections.length;

        for (let i = 0; i < depthCount; i++) {
            const index = teamSelections[i];
            const selectedTeam = teamDatas[index];

            if (selectedTeam === TMCascading.All) {
                if (prevSelectedTeam !== null) {
                    TMCascading.addTeamMembers(memberDatas, prevSelectedTeam.team);
                }

                const _memberDatas = TMCascading.getAllChildMembers(teamDatas);
                memberDatas.push.apply(memberDatas, _memberDatas);
                break;
            }
            else {
                teamDatas = selectedTeam.childTeams;
                prevSelectedTeam = selectedTeam;

                if (!teamDatas || teamDatas.length === 0) {
                    TMCascading.addTeamMembers(memberDatas, selectedTeam.team);
                    break;
                }
            }
        }

        if (memberDatas.length > 1) {
            memberDatas.unshift(TMCascading.All);
        }

        return memberDatas;
    }

    static addTeamMembers(members, team) {
        const memberCount = team.members.length;

        for (let i = 0; i < memberCount; i++) {
            const member = team.members[i];
            members.push(member);
        }
    }

    static getAllChildMembers(teamDatas) {
        const count = teamDatas.length;
        const members = [];

        for (let i = 0; i < count; i++) {
            const teamData = teamDatas[i];

            if (teamData === TMCascading.All) {
                continue;
            }
            else {
                const childTeamCount = teamData.team.childTeams.length;
                TMCascading.addTeamMembers(members, teamData.team);

                for (let j = 0; j < childTeamCount; j++) {
                    const childTeam = teamData.team.childTeams[j];
                    TMCascading.addTeamMembers(members, childTeam);
                    TMCascading.getChildTeamMembers(members, childTeam.childTeams);
                }
            }
        }

        return members;
    }

    static getChildTeamMembers(members, teams) {
        if (!teams) {
            return;
        }

        const teamCount = teams.length;

        for (let i = 0; i < teamCount; i++) {
            const team = teams[i];
            TMCascading.addTeamMembers(members, team);
            TMCascading.getChildTeamMembers(members, team.childTeams);
        }
    }

    static getMemberID(memberData) {
        if (memberData === TMCascading.All) {
            return -1;
        }

        return memberData.id;
    }

    static getMemberName(memberData) {
        if (memberData === TMCascading.All) {
            return memberData;
        }

        return memberData.name;
    }

    static getTeamName(teamData) {
        if (teamData === TMCascading.All) {
            return teamData;
        }

        return teamData.team.teamName;
    }

    static getTeamID(teamData, depth) {
        if (teamData === TMCascading.All) {
            return -1 * (depth + 1);
        }

        return teamData.team.id;
    }

    static makeTeamSelections(membersHistory) {
        const teamSelections = [];

        const childTeamCount = membersHistory.rootTeam.childTeams.length;
        let depth = 0;

        for (let i = 0; i < childTeamCount; i++) {
            const childTeam = membersHistory.rootTeam.childTeams[i];
            const childDepth = TMCascading.getTeamDepth(childTeam, 1);

            if (depth < childDepth) {
                depth = childDepth;
            }
        }

        for (let i = 0; i <= depth; i++) {
            teamSelections.push(0);
        }

        return teamSelections;
    }

    static getTeamDepth(team, depth) {
        if (!team.childTeams) {
            return depth - 1;
        }

        const childTeamCount = team.childTeams.length;
        let maxDepth = depth;

        for (let i = 0; i < childTeamCount; i++) {
            const childTeam = team.childTeams[i];
            let childDepth = TMCascading.getTeamDepth(childTeam, depth + 1);

            if (childDepth > maxDepth) {
                maxDepth = childDepth;
            }
        }

        return maxDepth;
    }

    static getMember(memberID, members) {
        if (!memberID || memberID < 0) {
            return null;
        }

        for (let i = 0; i < members.length; i++) {
            const member = members[i];

            if (member.id === memberID) {
                return member;
            }
        }

        return null;
    }
}