using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Windows.Forms;

namespace TeamEditor
{
    public class RegularMemberReader
    {
        // 사번, 성명, 소속명, 직위명, 직군명, 직급명, 휴대전화, 근무지전화
        private enum COLUMN_HEADER { MEMBER_ID = 1, MEMBER_NAME, TEAM_PATH, JOB_POSITION, JOB_GROUP, JOB_LEVEL, MOBILE_PHONE, OFFICE_PHONE };
        // 팀원, 팀장, 파트장, 휴직, 알수없음
        private enum JOB_POSITION { TEAM_MEMBER = 0, TEAM_LEADER, PART_LEADER, 처장, 본부장, LEAVE, UNKNOWN };

        private Tree<RegularTeam> m_teamTree = new Tree<RegularTeam>();
        // 팀별 직원들
        private Dictionary<RegularTeam, List<CompanyMember>> m_dicRegularMembers = new Dictionary<RegularTeam, List<CompanyMember>>();
        // 삭제될 이전 팀들의 목록
        private List<RegularTeam> m_removingOldTeams = null;
        // 삭제될 이전 직원들의 목록
        private List<CompanyMember> m_removingOldCompanyMembers = null;
        private List<CompanyMember> m_notRemovingOldCompanyMembers = null;
        
        public RegularMemberReader()
        {
        }

        public bool OpenFile(string strPath)
        {
            int nDotIndex = strPath.LastIndexOf('.');

            if (nDotIndex < 0)
                return false;

            string strExt = strPath.Substring(nDotIndex + 1).ToLower();
            bool opened = false;

            if (strExt == "txt")
                opened = OpenFile(strPath, '\t');
            else if (strExt == "csv")
                opened = OpenFile(strPath, ',');

            if (opened == false)
                return false;

            CompareRegularMembers();

            Command.CommandUpdateAllRegularMembers command = new Command.CommandUpdateAllRegularMembers();
            command.TeamTree = this.m_teamTree;
            command.RegularMembers = this.m_dicRegularMembers;
            command.RemovingOldTeams = this.m_removingOldTeams;
            command.RemovingOldCompanyMembers = this.m_removingOldCompanyMembers;
            command.CopyNotRemovingOldCompanyMembers(this.m_notRemovingOldCompanyMembers);

            FormMain.Instance.AddCommand(command);
            return opened;
        }

        // 1. 기존에 존재하는 팀 가운데 삭제할 목록을 얻어온다.
        // 2. 기존에 존재하는 팀 가운데 삭제되지 않을 팀으로부터 ID를 얻어와서 새로운 팀에 부여한다.
        // 3. 기존에 존재하는 직원 가운데 삭제할 목록을 얻어온다.
        // 4. 기존에 존재하는 직원 가운데 삭제되지 않을 직원으로부터 ID를 얻어와서 새로운 직원에게 부여한다.
        private void CompareRegularMembers()
        {
            m_removingOldTeams = new List<RegularTeam>();
            m_removingOldCompanyMembers = new List<CompanyMember>();
            m_notRemovingOldCompanyMembers = new List<CompanyMember>();

            TeamTreeView oldTeamTree = FormMain.Instance.RegularTeamTree;

            List<Tree<RegularTeam>.Node> newNodes = new List<Tree<RegularTeam>.Node>();
            newNodes.Add(m_teamTree.RootNode);

            CompareRegularMembers(newNodes, oldTeamTree.Nodes, m_removingOldTeams, m_removingOldCompanyMembers, m_notRemovingOldCompanyMembers);
        }

        private void CompareRegularMembers(List<Tree<RegularTeam>.Node> newNodes, TreeNodeCollection oldNodes, List<RegularTeam> removingOldTeams, List<CompanyMember> removingOldCompanyMembers, List<CompanyMember> notRemovingOldCompanyMembers)
        {
            foreach (TreeNode oldNode in oldNodes)
            {
                if (oldNode.Tag != null && (oldNode.Tag is RegularTeam))
                {
                    RegularTeam team = (RegularTeam)oldNode.Tag;

                    Tree<RegularTeam>.Node node = FindRegularTeamNode(team, newNodes);

                    if (node != null)
                    {
                        node.Data.TeamID = team.TeamID;
                        CompareRegularMembers(node.Children, oldNode.Nodes, removingOldTeams, removingOldCompanyMembers, notRemovingOldCompanyMembers);
                        CompareCompanyMembers(node.Data, team, removingOldCompanyMembers, notRemovingOldCompanyMembers);
                    }
                    else
                    {
                        removingOldTeams.Add(team);
                        GetRegularTeams(oldNode.Nodes, removingOldTeams);
                        GetCompanyMembers(team, oldNode.Nodes, removingOldCompanyMembers);
                    }
                }
                else
                {
                    GetRegularTeams(oldNode.Nodes, removingOldTeams);
                    GetCompanyMembers(null, oldNode.Nodes, removingOldCompanyMembers);
                }
            }
        }

        private void GetCompanyMembers(RegularTeam team, TreeNodeCollection nodes, List<CompanyMember> removingOldCompanyMembers)
        {
            if (team != null)
            {
                List<CompanyMember> members = DataManager.GetRegularMembers(team);

                if (members != null)
                {
                    foreach (CompanyMember member in members)
                    {
                        removingOldCompanyMembers.Add(member);
                    }
                }
            }

            foreach (TreeNode node in nodes)
            {
                if (node.Tag != null && (node.Tag is RegularTeam))
                    GetCompanyMembers((RegularTeam)node.Tag, node.Nodes, removingOldCompanyMembers);
                else
                    GetCompanyMembers(null, node.Nodes, removingOldCompanyMembers);
            }
        }

        private void CompareCompanyMembers(RegularTeam newTeam, RegularTeam oldTeam, List<CompanyMember> removingOldCompanyMembers, List<CompanyMember> notRemovingOldCompanyMembers)
        {
            List<CompanyMember> newMembers = null;
            List<CompanyMember> oldMembers = DataManager.GetRegularMembers(oldTeam);

            if (oldMembers != null && m_dicRegularMembers.TryGetValue(newTeam, out newMembers))
            {
                foreach (CompanyMember oldMember in oldMembers)
                {
                    CompanyMember member = FindCompanyMember(oldMember.Name, oldMember.MemberID, newMembers);

                    if (member != null)
                    {
                        member.ID = oldMember.ID;
                        notRemovingOldCompanyMembers.Add(oldMember);
                    }
                    else
                        removingOldCompanyMembers.Add(oldMember);
                }
            }
            else
            {
                if (oldMembers != null)
                {
                    foreach (CompanyMember oldMember in oldMembers)
                    {
                        removingOldCompanyMembers.Add(oldMember);
                    }
                }
            }
        }

        private CompanyMember FindCompanyMember(string strMemberName, string strMemberID, List<CompanyMember> members)
        {
            foreach (CompanyMember member in members)
            {
                if (member.Name == strMemberName && member.MemberID == strMemberID)
                    return member;
            }

            return null;
        }

        private Tree<RegularTeam>.Node FindRegularTeamNode(RegularTeam team, List<Tree<RegularTeam>.Node> nodes)
        {
            foreach (Tree<RegularTeam>.Node node in nodes)
            {
                if (node.Data != null && node.Data.TeamName == team.TeamName)
                    return node;
            }

            return null;
        }

        private void GetRegularTeams(TreeNodeCollection nodes, List<RegularTeam> teams)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag != null && (node.Tag is RegularTeam))
                {
                    RegularTeam team = (RegularTeam)node.Tag;
                    teams.Add(team);
                }

                GetRegularTeams(node.Nodes, teams);
            }
        }

        private bool OpenFile(string strPath, char delimeter)
        {
            if (File.Exists(strPath) == false)
                return false;

            try
            {
                Encoding encoding = GetEncoding(strPath);
                StreamReader reader = new StreamReader(strPath, encoding);

                // Key : Column Index
                // Value : 해당 Column에 대한 실제 File 내 Column Index
                Dictionary<COLUMN_HEADER, int> dicIndices = null;
                Dictionary<JOB_POSITION, int> dicJobPositionID = ReadJobPositions();

                int nIndexCount = Enum.GetValues(typeof(COLUMN_HEADER)).Length;

                while (!reader.EndOfStream)
                {
                    string strLine = reader.ReadLine().Trim();

                    if (strLine.Length == 0)
                        continue;

                    bool isEmpty = true;
                    string[] tokens = strLine.Split(delimeter);

                    foreach (string strToken in tokens)
                    {
                        if (strToken.Length > 0)
                        {
                            isEmpty = false;
                            break;
                        }
                    }

                    if (isEmpty)
                        continue;

                    if (dicIndices == null)
                        dicIndices = FindColumnHeader(tokens, nIndexCount);
                    else
                        ReadRegularMember(tokens, dicIndices, nIndexCount, dicJobPositionID);
                }

                reader.Close();
                return dicIndices != null;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return false;
        }

        private bool ReadRegularMember(string[] tokens, Dictionary<COLUMN_HEADER, int> dicIndices, int nIndexCount, Dictionary<JOB_POSITION, int> dicJobPositionID)
        {
            int nTokenCount = tokens.Count();
            Dictionary<COLUMN_HEADER, string> dicValues = new Dictionary<COLUMN_HEADER,string>();

            foreach (KeyValuePair<COLUMN_HEADER, int> pair in dicIndices)
            {
                if (pair.Value >= nTokenCount)
                    continue;

                dicValues[pair.Key] = tokens[pair.Value];
            }

            string strMemberID = null, strName = null, strTeamPath = null, strJobLevel = null;
            string strJobPosition = null, strJobGroup = null, strMobilePhoneNumber = null, strOfficePhoneNumber = null;

            // 사번은 필수
            if (dicValues.TryGetValue(COLUMN_HEADER.MEMBER_ID, out strMemberID) == false)
                return false;
            // 성명은 필수
            if (dicValues.TryGetValue(COLUMN_HEADER.MEMBER_NAME, out strName) == false)
                return false;
            // 소속명은 필수
            if (dicValues.TryGetValue(COLUMN_HEADER.TEAM_PATH, out strTeamPath) == false)
                return false;
            // 직급명은 필수
            if (dicValues.TryGetValue(COLUMN_HEADER.JOB_LEVEL, out strJobLevel) == false)
                return false;

            dicValues.TryGetValue(COLUMN_HEADER.JOB_POSITION, out strJobPosition);
            dicValues.TryGetValue(COLUMN_HEADER.JOB_GROUP, out strJobGroup);
            dicValues.TryGetValue(COLUMN_HEADER.MOBILE_PHONE, out strMobilePhoneNumber);
            dicValues.TryGetValue(COLUMN_HEADER.OFFICE_PHONE, out strOfficePhoneNumber);

            RegularTeam team = GetRegularTeam(dicValues[COLUMN_HEADER.TEAM_PATH]);

            if (team != null)
            {
                CompanyMember.JobPositionSubInfo subPosition = null;
                CompanyMember.JobLevelSubInfo subLevel = null;
                CompanyMember member = new CompanyMember();

                member.MemberID = strMemberID;
                member.Name = strName;
                member.PositionID = GetJobPositionID(strTeamPath, strJobPosition, dicJobPositionID, ref subPosition);
                member.SubJobPosition = subPosition;
                member.PhoneNumber = GetMobilePhoneNumber(strMobilePhoneNumber);
                member.OfficePhoneNumber = strOfficePhoneNumber;
                member.LevelID = GetJobLevel(strJobLevel, ref subLevel);
                member.SubJobLevel = subLevel;
                member.GroupPosition = GetJobGroup(strJobGroup);
                
                List<CompanyMember> members = null;

                if (!m_dicRegularMembers.TryGetValue(team, out members))
                {
                    members = new List<CompanyMember>();
                    m_dicRegularMembers[team] = members;
                }

                members.Add(member);
                return true;
            }

            return false;
        }

        private RegularTeam GetRegularTeam(string strTeamPath)
        {
            string[] teams = strTeamPath.Split('/');
            int nTeamCount = teams.Count();

            Tree<RegularTeam>.Node node = null;

            for (int i=0;i<nTeamCount;i++)
            {
                string strTeamName = teams[i].Trim();
                node = GetRegularTeam(strTeamName, node);

                if (node == null)
                    return null;
            }

            return node.Data;
        }

        private Tree<RegularTeam>.Node GetRegularTeam(string strTeamName, Tree<RegularTeam>.Node nodeParent)
        {
            if (nodeParent == null)
            {
                if (m_teamTree.RootNode.Data == null)
                {
                    RegularTeam team = new RegularTeam();
                    team.TeamName = strTeamName;

                    m_teamTree.RootNode.Data = team;
                    nodeParent = m_teamTree.RootNode;
                }
                else if (m_teamTree.RootNode.Data.TeamName == strTeamName)
                {
                    nodeParent = m_teamTree.RootNode;
                }
                
                return nodeParent;
            }

            foreach (Tree<RegularTeam>.Node node in nodeParent.Children)
            {
                if (node.Data != null && node.Data.TeamName == strTeamName)
                    return node;
            }

            RegularTeam _team = new RegularTeam();
            _team.TeamName = strTeamName;
            _team.ParentTeam = nodeParent.Data;

            return nodeParent.AddChild(_team);
        }

        private CompanyMember.JobGroupPosition GetJobGroup(string strJobGroup)
        {
            if (strJobGroup == null || strJobGroup.Length == 0)
                return null;

            strJobGroup = strJobGroup.Trim();
            CompanyMember.JobGroupPosition group = CompanyMember.JobGroupPosition.GetJobGroupPosition(strJobGroup);

            if (group == null)
            {
                group = new CompanyMember.JobGroupPosition();
                group.Name = strJobGroup;
            }

            return group;
        }

        private int GetJobLevel(string strJobLevel, ref CompanyMember.JobLevelSubInfo subJob)
        {
            if (strJobLevel == null || strJobLevel.Length == 0)
                return 0;

            int nLevelID = 0;
            int nIndex = strJobLevel.IndexOf("직급");

            if (nIndex > 0)
            {
                string strLevel = strJobLevel.Substring(0, nIndex).Trim();
                int.TryParse(strLevel, out nLevelID);

                int nIndex1 = strJobLevel.IndexOf('(');
                int nIndex2 = strJobLevel.IndexOf(')');

                if (nIndex1 > 0 && nIndex2 > nIndex1)
                {
                    string strSubLevel = strJobLevel.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
                    subJob = CompanyMember.JobLevelSubInfo.GetJobSubLevel(strSubLevel);

                    if (subJob == null)
                    {
                        subJob = new CompanyMember.JobLevelSubInfo();
                        subJob.Name = strSubLevel;
                    }
                }
            }

            return nLevelID;
        }

        private string GetMobilePhoneNumber(string strPhoneNumber)
        {
            if (strPhoneNumber == null)
                return null;

            string str = "";
            int nLength = strPhoneNumber.Length;

            for (int i=0;i<nLength;i++)
            {
                char ch = strPhoneNumber.ElementAt(i);

                if (ch >= '0' && ch <= '9')
                    str += ch;
            }

            // Excel의 셀서식(표시형식) 속성 때문에 010이 10으로 표시될 경우 0을 앞에 붙여준다.
            if (str.Length > 0 && str.ElementAt(0) == '1')
                str = "0" + str;

            return str;
        }

        private int GetJobPositionID(string strTeamPath, string strJobPosition, Dictionary<JOB_POSITION, int> dicJobPositionID, ref CompanyMember.JobPositionSubInfo subJob)
        {
            int nPositionID = -1;

            if (strJobPosition != null)
            {
                strJobPosition = strJobPosition.Trim();

                if (strJobPosition.Length > 0)
                {
                    subJob = CompanyMember.JobPositionSubInfo.GetSubPosition(strJobPosition);

                    if (subJob == null)
                    {
                        subJob = new CompanyMember.JobPositionSubInfo();
                        subJob.Name = strJobPosition;
                    }
                }

                if (strJobPosition.EndsWith("파트장"))
                    dicJobPositionID.TryGetValue(JOB_POSITION.PART_LEADER, out nPositionID);
                else if (strJobPosition.EndsWith("처장"))
                    dicJobPositionID.TryGetValue(JOB_POSITION.처장, out nPositionID);
                else if (strJobPosition.EndsWith("본부장"))
                    dicJobPositionID.TryGetValue(JOB_POSITION.본부장, out nPositionID);
                else if (strJobPosition.Length == 0)
                    dicJobPositionID.TryGetValue(JOB_POSITION.UNKNOWN, out nPositionID);
                else if (strTeamPath.Length > 0)
                {
                    string strLast = strTeamPath.Substring(strTeamPath.Length - 1);
                    string strTeamLeader = strLast + "장";

                    if (strJobPosition.EndsWith(strTeamLeader))
                        dicJobPositionID.TryGetValue(JOB_POSITION.TEAM_LEADER, out nPositionID);
                    else
                        dicJobPositionID.TryGetValue(JOB_POSITION.TEAM_MEMBER, out nPositionID);
                }
            }

            if (nPositionID < 0)
                dicJobPositionID.TryGetValue(JOB_POSITION.UNKNOWN, out nPositionID);

            if (nPositionID < 0)
                nPositionID = 0;

            return nPositionID;
        }

        private Dictionary<JOB_POSITION, int> ReadJobPositions()
        {
            Dictionary<JOB_POSITION, int> dicJobPositionID = new Dictionary<JOB_POSITION, int>();

            foreach (KeyValuePair<int, string> pair in DataManager.JobPositions)
            {
                string strPositionName = pair.Value;

                if (strPositionName == "팀원")
                    dicJobPositionID[JOB_POSITION.TEAM_MEMBER] = pair.Key;
                else if (strPositionName == "팀장")
                    dicJobPositionID[JOB_POSITION.TEAM_LEADER] = pair.Key;
                else if (strPositionName == "파트장")
                    dicJobPositionID[JOB_POSITION.PART_LEADER] = pair.Key;
                else if (strPositionName == "처장")
                    dicJobPositionID[JOB_POSITION.처장] = pair.Key;
                else if (strPositionName == "본부장")
                    dicJobPositionID[JOB_POSITION.본부장] = pair.Key;
                else if (strPositionName == "휴직")
                    dicJobPositionID[JOB_POSITION.LEAVE] = pair.Key;
                else if (strPositionName == "알 수 없음" || strPositionName == "알수 없음" || strPositionName == "알 수없음" || strPositionName == "알수없음")
                    dicJobPositionID[JOB_POSITION.UNKNOWN] = pair.Key;
            }

            return dicJobPositionID;
            /*string strSQL = "Select ID, PositionName from JobPosition";
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            Dictionary<JOB_POSITION, int> dicJobPositionID = new Dictionary<JOB_POSITION,int>();
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                DBUtility.VariousData<int> id = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString());
                string strPositionName = DBUtility.WebDBManager.GetStringField(arrResult[i + 1].ToString());

                if (id == null || strPositionName == null)
                    continue;

                if (strPositionName == "팀원")
                    dicJobPositionID[JOB_POSITION.TEAM_MEMBER] = id.Data;
                else if (strPositionName == "팀장")
                    dicJobPositionID[JOB_POSITION.TEAM_LEADER] = id.Data;
                else if (strPositionName == "파트장")
                    dicJobPositionID[JOB_POSITION.PART_LEADER] = id.Data;
                else if (strPositionName == "처장")
                    dicJobPositionID[JOB_POSITION.처장] = id.Data;
                else if (strPositionName == "본부장")
                    dicJobPositionID[JOB_POSITION.본부장] = id.Data;
                else if (strPositionName == "휴직")
                    dicJobPositionID[JOB_POSITION.LEAVE] = id.Data;
                else if (strPositionName == "알 수 없음" || strPositionName == "알수 없음" || strPositionName == "알 수없음" || strPositionName == "알수없음")
                    dicJobPositionID[JOB_POSITION.UNKNOWN] = id.Data;
            }

            return dicJobPositionID;*/
        }

        // Key : Column Index
        // Value : 해당 Column에 대한 실제 File 내 Column Index
        private Dictionary<COLUMN_HEADER, int> FindColumnHeader(string[] tokens, int nIndexCount)
        {
            Dictionary<COLUMN_HEADER, int> dicIndeces = new Dictionary<COLUMN_HEADER,int>();
            int nTokenCount = tokens.Count();

            for (int i=0;i<nTokenCount;i++)
            {
                string strToken = tokens[i].Trim();

                if (strToken == "사번")
                    dicIndeces[COLUMN_HEADER.MEMBER_ID] = i;
                else if (strToken == "성명" || strToken == "이름")
                    dicIndeces[COLUMN_HEADER.MEMBER_NAME] = i;
                else if (strToken == "소속명" || strToken == "소속")
                    dicIndeces[COLUMN_HEADER.TEAM_PATH] = i;
                else if (strToken == "직위명" || strToken == "직위")
                    dicIndeces[COLUMN_HEADER.JOB_POSITION] = i;
                else if (strToken == "직군명" || strToken == "직군")
                    dicIndeces[COLUMN_HEADER.JOB_GROUP] = i;
                else if (strToken == "직급명" || strToken == "직급")
                    dicIndeces[COLUMN_HEADER.JOB_LEVEL] = i;
                else if (strToken == "휴대전화")
                    dicIndeces[COLUMN_HEADER.MOBILE_PHONE] = i;
                else if (strToken == "근무지전화")
                    dicIndeces[COLUMN_HEADER.OFFICE_PHONE] = i;
            }

            if (dicIndeces.Count < nIndexCount)
                return null;

            return dicIndeces;
        }

        private Encoding GetEncoding(string strPath)
        {
            FileStream stream = new FileStream(strPath, FileMode.Open);
            long nFileSize = stream.Seek(0, SeekOrigin.End);

            Encoding euckr = Encoding.GetEncoding(51949);

            if (nFileSize < 3)
            {
                stream.Close();
                return euckr;
            }

            byte[] bytes = new byte[nFileSize];
            stream.Read(bytes, 0, (int)nFileSize);
            stream.Close();

            // BOM 정의
	        byte[] btBOM_UnicodeBE = new byte[]{0xFE, 0xFF};
	        byte[] btBOM_UnicodeLE = new byte[]{0xFF, 0xFE};
	        byte[] btBOM_UTF8 = new byte[]{0xEF, 0xBB, 0xBF};

            if (CompareBytes(bytes, btBOM_UnicodeBE, 2))
                return Encoding.BigEndianUnicode;
            else if (CompareBytes(bytes, btBOM_UnicodeLE, 2))
                return Encoding.Unicode;
            else if (CompareBytes(bytes, btBOM_UTF8, 3))
                return new UTF8Encoding(true);

            return AnalyzeFormatUtf8(bytes) ? new UTF8Encoding(false) : euckr;
        }

        // Return 값 : true이면 BOM이 없는 UTF-8
        //             false이면 ANSI
        private bool AnalyzeFormatUtf8(byte[] bytes)
        {
            int nStringLength = bytes.Count();

            bool bFind = false;

            for (int i = 0; i < nStringLength;i++ )
            {
                byte b = bytes[i];
                if ((b & 0x80) == 0x80)
                {
                    bFind = true;

                    // 상위 비트가 110이고 다음 문자의 상위 비트가 10이면 UTF8맞음
                    // p가 문서 끝을 넘거나 중간에 하나라도 규칙에 맞지 않으면 UTF8이 아님
                    if ((b & 0xe0) == 0xc0)
                    {
                        if (++i >= nStringLength)
                            return false;

                        b = bytes[i];

                        if ((b & 0xc0) != 0x80)
                            return false;
                        continue;
                    }

                    // 상위 비트가 1110일 때는 다음 두 문자의 상위 비트가 10이어야 한다.
                    if ((b & 0xf0) == 0xe0)
                    {
                        if (++i >= nStringLength)
                            return false;

                        b = bytes[i];

                        if ((b & 0xc0) != 0x80)
                            return false;

                        if (++i >= nStringLength)
                            return false;

                        b = bytes[i];

                        if ((b & 0xc0) != 0x80)
                            return false;

                        continue;
                    }

                    // 상위 비트가 11110일 때는 다음 세 문자의 상위 비트가 10이어야 한다.
                    if ((b & 0xf8) == 0xf0)
                    {
                        if (++i >= nStringLength)
                            return false;

                        b = bytes[i];

                        if ((b & 0xc0) != 0x80)
                            return false;

                        if (++i >= nStringLength)
                            return false;

                        b = bytes[i];

                        if ((b & 0xc0) != 0x80)
                            return false;

                        if (++i >= nStringLength)
                            return false;

                        b = bytes[i];

                        if ((b & 0xc0) != 0x80)
                            return false;

                        continue;
                    }

                    // 0x80을 넘었는데 상위 비트가 110, 1110, 11110 중 하나가 아니면
                    // UTF-8 문서가 아니다.
                    return false;
                }
            }

            // 0x80 넘는 값이 하나도 없으면 ANSI로 취급한다.
            if (bFind == false)
            {
                return false;
            }

            // 0x80을 넘는 모든 값이 UTF-8의 조건을 만족하면 UTF-8문서이다.
            return true;
        }

        private bool CompareBytes(byte[] bytes1, byte[] bytes2, int nCount)
        {
            if (bytes1 == null && bytes2 == null)
                return true;
            else if (bytes1 == null || bytes2 == null)
                return false;
            else if (bytes1.Count() < nCount || bytes2.Count() < nCount)
                return false;

            for (int i=0;i<nCount;i++)
            {
                if (bytes1[i] != bytes2[i])
                    return false;
            }

            return true;
        }
    }
}
