using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SectionContents.Utility
{
    public class MissionItemExternal : Sections.MissionItem
    {
        private string m_strExternalExeFileName = "";
        private List<string> m_arguments = new List<string>();
        private List<string> m_originalArguments = new List<string>();
        private string m_strDescription = "";

        private static string m_strExternalFolderPath = ".";

        private int m_nActionStepHistoryID = -1;
        private UnE.SOP.Sections.SectionTabPage m_sectionTabPage = null;

        public UnE.SOP.Sections.SectionTabPage TabPage
        {
            get { return m_sectionTabPage; }
            set { m_sectionTabPage = value; }
        }

        public string ExternalExeFilePath
        {
            get
            {
                string strFolder = m_strExternalFolderPath;

                if (strFolder == null || strFolder.Length == 0)
                    strFolder = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\";
                else if (strFolder.EndsWith("\\") == false)
                    strFolder += "\\";

                return strFolder + m_strExternalExeFileName;
            }
        }

        public static string ExternalFolderPath
        {
            get { return m_strExternalFolderPath; }
            set { m_strExternalFolderPath = value; }
        }

        public List<string> Arguments
        {
            get { return m_arguments; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public List<string> OriginalArguments
        {
            get { return m_originalArguments; }
        }

        public int ActionStepHistoryID
        {
            get { return m_nActionStepHistoryID; }
            set { m_nActionStepHistoryID = value; }
        }

        public MissionItemExternal(string str)
        {
            //if (EXTERNAL_EXE_FOLDER == null)
            //{
            //    // Local 옵션
            //    DBUtility.Utility util = new DBUtility.Utility();
            //    string strFolderPath = util.getinivalue("ExternalRun", "Folder");

            //    if (strFolderPath != null && strFolderPath.Length > 0)
            //        EXTERNAL_EXE_FOLDER = strFolderPath;
            //    else
            //        EXTERNAL_EXE_FOLDER = "";

            //    // DB를 통한 공통 옵션
            //    /*string strSQL = "Select PropertyValue from OptionSOPSimulator where PropertyName = 'ExternalExecFolder' and SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();
            //    ArrayList arrResult = FormSOP.Instance.DBManager.GetResultData(strSQL);

            //    if (arrResult == null || arrResult.Count == 0)
            //        EXTERNAL_EXE_FOLDER = "";
            //    else
            //        EXTERNAL_EXE_FOLDER = WebDBManager.GetStringField(arrResult[0], "");*/
            //}

            int nIndex1 = str.IndexOf('(');
            int nIndex2 = str.LastIndexOf(')');

            if (nIndex1 >= 0 && nIndex2 > nIndex1)
            {
                string strData = str.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
                int nIndex = strData.IndexOf(',');

                string strExe = nIndex >= 0 ? strData.Substring(0, nIndex).Trim() : strData.Trim();

                //string strExe = strData.Substring(0, nIndex).Trim();

                if (strExe.ToLower().EndsWith(".exe") == false)
                    strExe += ".exe";

                strExe = strExe.Replace("/", "\\");

                /*bool beginSlash = strExe.StartsWith("\\");
                bool endSlash = EXTERNAL_EXE_FOLDER.EndsWith("\\");

                if (EXTERNAL_EXE_FOLDER.Length == 0)
                {
                    if (beginSlash)
                        strExe = "." + strExe;
                }
                else
                {
                    if (endSlash)
                    {
                        if (beginSlash)
                            strExe = EXTERNAL_EXE_FOLDER + "." + strExe;
                        else
                            strExe = EXTERNAL_EXE_FOLDER + strExe;
                    }
                    else
                    {
                        if (beginSlash)
                            strExe = EXTERNAL_EXE_FOLDER + strExe;
                        else
                            strExe = EXTERNAL_EXE_FOLDER + "\\" + strExe;
                    }
                }*/

                m_strExternalExeFileName = strExe;

                if (nIndex >= 0)
                {
                    bool quotationBlock = false;
                    int nLength = strData.Length;
                    int nBeginIndex = nIndex + 1;

                    for (int i = nIndex + 1; i < nLength; i++)
                    {
                        char ch = strData[i];

                        if (ch == '\"')
                        {
                            quotationBlock = !quotationBlock;
                        }
                        else if (ch == ',')
                        {
                            if (!quotationBlock)
                            {
                                AddArgument(strData, ref nBeginIndex, i);
                            }
                        }
                    }

                    if (nBeginIndex < nLength - 1)
                    {
                        AddArgument(strData, ref nBeginIndex, nLength);
                    }
                }
            }
        }

        private void AddArgument(string strData, ref int nBeginIndex, int nEndIndex)
        {
            string strArg = strData.Substring(nBeginIndex, nEndIndex - nBeginIndex).Trim();

            if (strArg.StartsWith("\"") && strArg.EndsWith("\""))
            {
                if (strArg.Length >= 2)
                    strArg = strArg.Substring(1, strArg.Length - 2);
                else
                    strArg = "";
            }

            if (strArg.Length > 0)
            {
                m_arguments.Add(strArg);
                m_originalArguments.Add(strArg);
            }

            nBeginIndex = nEndIndex + 1;
        }

        public static bool IsExternalMissionText(string strMissionText)
        {
            string strLower = strMissionText.ToLower().Trim();

            int nIndex = strLower.IndexOf('(');

            if (nIndex < 0)
                return false;

            if (strLower.EndsWith(")") == false)
                return false;

            string strTag = strLower.Substring(0, nIndex).Trim();

            if (strTag == "#exec")
                return true;

            return false;
        }

        // 외부 실행파일에 대한 주석인가?
        public static bool IsExternalMissionDescriptionText(string strMissionText, out string strDescription)
        {
            string strTag = "설명";
            strMissionText = strMissionText.Trim();

            if (strMissionText.StartsWith(strTag))
            {
                strMissionText = strMissionText.Substring(strTag.Length).Trim();

                if (strMissionText.StartsWith(":"))
                {
                    strDescription = strMissionText.Substring(1).Trim();
                    return true;
                }
            }

            strDescription = null;
            return false;
        }
    }

    public class MissionData
    {
        public enum ProcessType { None = 0, SendSMS, CheckedComplete, UncheckedComplete };

        private ProcessType m_processType = ProcessType.None;
        private DateTime m_timeStamp;
        private int m_nMissionIndex = -1;

        public DateTime TimeStamp
        {
            get { return m_timeStamp; }
        }

        public int MissionIndex
        {
            get { return m_nMissionIndex; }
        }

        public MissionData(ProcessType type, DateTime time, int nMissionIndex)
        {
            m_processType = type;
            m_timeStamp = time;
            m_nMissionIndex = nMissionIndex;
        }

        public ProcessType GetProcessType()
        {
            return m_processType;
        }
    }
}
