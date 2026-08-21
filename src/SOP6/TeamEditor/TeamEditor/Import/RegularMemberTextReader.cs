using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace TeamEditor.Import
{
    public class RegularMemberTextReader : RegularMemberReader
    {
        private bool m_updateAll = true;

        public bool UpdateAll
        {
            get { return m_updateAll; }
        }

        public bool OpenTextFile(string strPath, char delimeter)
        {
            if (File.Exists(strPath) == false)
                return false;

            try
            {
                Encoding encoding = GetEncoding(strPath);
                StreamReader reader = new StreamReader(strPath, encoding);

                // Key : Column Index
                // Value : 해당 Column에 대한 실제 File 내 Column Index
                //Dictionary<COLUMN_HEADER, int> dicIndices = null;
                //Dictionary<JOB_POSITION, int> dicJobPositionID = ReadJobPositions();
                // 사번 중복체크용
                //Dictionary<string, string> dicMemberID = new Dictionary<string, string>();
                // 휴대전화번호 중복체크용
                //Dictionary<string, string> dicPhoneNumber = new Dictionary<string, string>();

                Import.IRegularReader regularReader = null;
                //int nIndexCount = Enum.GetValues(typeof(COLUMN_HEADER)).Length;

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

                    if (regularReader == null)
                        regularReader = Import.RegularReaderManager.MakeExcelReader(tokens);
                    else
                        regularReader.ReadRegularMember(tokens, m_dicRegularMembers);
                }

                reader.Close();

                if (regularReader != null)
                {
                    m_updateAll = regularReader.UpdateAll;
                    m_newTemporaryNormalTeams = regularReader.NewTemporaryNormalTeams;
                    m_newTemporaryEmergencyTeams = regularReader.NewTemporaryEmergencyTeams;
                    m_removingOldTemporaryNormalTeams = regularReader.RemovingOldTemporaryNormalTeams;
                    m_removingOldTemporaryEmergencyTeams = regularReader.RemovingOldTemporaryEmergencyTeams;
                    m_dicNewTemporaryNormalMembers = regularReader.NewTemporaryNormalMembers;
                    m_dicNewTemporaryEmergencyMembers = regularReader.NewTemporaryEmergencyMembers;
                    m_dicRemovingOldTemporaryNormalMembers = regularReader.RemovingOldTemporaryNormalMembers;
                    m_dicRemovingOldTemporaryEmergencyMembers = regularReader.RemovingOldTemporaryEmergencyMembers;

                    if (regularReader is RegularMemberReader)
                    {
                        m_teamTree = ((RegularMemberReader)regularReader).TeamTree;
                    }
                }
                else
                    return false;

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return false;
        }
    }
}
