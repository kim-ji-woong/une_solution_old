using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamEditor.Import
{
    public interface IRegularReader
    {
        bool UpdateAll
        {
            get;
        }

        List<Team> NewTemporaryNormalTeams
        {
            get;
        }

        List<Team> NewTemporaryEmergencyTeams
        {
            get;
        }

        List<Team> RemovingOldTemporaryNormalTeams
        {
            get;
        }

        List<Team> RemovingOldTemporaryEmergencyTeams
        {
            get;
        }

        // 추가될 주간 및 평일 자위소방대원 목록
        Dictionary<Team, List<TemporaryMember>> NewTemporaryNormalMembers
        {
            get;
        }

        // 추가될 야간 및 휴일 자위소방대원 목록
        Dictionary<Team, List<TemporaryMember>> NewTemporaryEmergencyMembers
        {
            get;
        }

        // 삭제될 주간 및 평일 자위소방대원 목록
        Dictionary<Team, List<TemporaryMember>> RemovingOldTemporaryNormalMembers
        {
            get;
        }

        // 삭제될 야간 및 휴일 자위소방대원 목록
        Dictionary<Team, List<TemporaryMember>> RemovingOldTemporaryEmergencyMembers
        {
            get;
        }

        bool FindColumnHeader(string[] tokens);
        bool ReadRegularMember(string[] tokens, Dictionary<RegularTeam, List<CompanyMember>> dicRegularMembers);
    }
}
