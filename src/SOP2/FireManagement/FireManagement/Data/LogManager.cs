using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Threading;

namespace FireManagement
{
    public class LogManager
    {
        // 설비 점검 시간
        protected class EquipmentCheckHistory
        {
            private static FireEquipmentHistory.EquipmentStatus m_defStatus = FireEquipmentHistory.EquipmentStatus.NORMAL;
            public static FireEquipmentHistory.EquipmentStatus DefaultStatus
            {
                get { return m_defStatus; }
            }

            private FireEquipment m_equip = null;
            private DateTime m_dtTime = new DateTime();
            private FireEquipmentHistory.EquipmentStatus m_status;

            public EquipmentCheckHistory()
            {
                m_status = m_defStatus;
            }

            public FireEquipment Equipment
            {
                get { return m_equip; }
                set { m_equip = value; }
            }

            public DateTime Time
            {
                get { return m_dtTime; }
                set { m_dtTime = value; }
            }

            public FireEquipmentHistory.EquipmentStatus Status
            {
                get { return m_status; }
                set { m_status = value; }
            }
        }

        private string m_strLogFolder = ".";
        private ArrayList m_arrEquipHistory = new ArrayList();

        // 몇 분 이내에 다시 읽혀진 설비는 기록하지 않는다.
        private int m_nIgnoreTime = 10;
        // 점검 번호
        private int m_nHistoryNum = 0;
        private int m_nFECheckCount = 0;
        private int m_nHDCheckCount = 0;
        private int m_nFACheckCount = 0;

        private StreamWriter m_writer = null;
        private string m_strLogFilePath = "";

        // Key : 설비별 점검 번호(설비의 고유값이 아니라 점검한 순서)
        private Dictionary<FireEquipment, int> m_dicEquipNo = new Dictionary<FireEquipment, int>();

        // 로그파일 확장자
        private string m_strExt = "txt";
        public string Ext
        {
            get { return m_strExt; }
        }

        // 로그파일 구분자
        private string m_strDelimeter = "\t";
        public string Delimeter
        {
            get { return m_strDelimeter; }
        }

        private static LogManager m_instance = null;
        public static LogManager Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new LogManager();

                return m_instance;
            }
        }

        protected LogManager()
        {
            m_strLogFolder = System.Windows.Forms.Application.StartupPath + "\\Logs";

            if (!Directory.Exists(m_strLogFolder))
            {
                Directory.CreateDirectory(m_strLogFolder);
            }

            // 저장 기간이 지난 로그 삭제는 프로그램 실행시 한번만 실행한다.
            Thread connectionThread = new Thread(new ThreadStart(DeletingLogThread));
            connectionThread.Start();
        }

        // 1년이 경과한 로그는 삭제한다.
        private void DeletingLogThread()
        {
            DateTime dtNow = DateTime.Now;

            string[] arrFolders = Directory.GetDirectories(m_strLogFolder);

            int nYear, nMonth, nDay;

            foreach (string strFolder in arrFolders)
            {
                int nIndex = strFolder.LastIndexOf('\\');

                if (nIndex < 0)
                    continue;

                string strFolderTime = strFolder.Substring(nIndex + 1);

                int nIndex2 = strFolderTime.IndexOf('-');

                if (nIndex2 < 0)
                    continue;

                string strYear = strFolderTime.Substring(0, nIndex2);
                string strMonth = strFolderTime.Substring(nIndex2 + 1);

                if (!int.TryParse(strYear, out nYear))
                    continue;

                if (!int.TryParse(strMonth, out nMonth))
                    continue;

                int nDiffYear = dtNow.Year - nYear;

                if (nDiffYear > 1 || (nDiffYear == 1 && dtNow.Month > nMonth))
                    DeleteFolder(strFolder);
                else if (nDiffYear == 1 && dtNow.Month == nMonth)
                {
                    string[] arrFiles = Directory.GetFiles(strFolder);

                    foreach (string strFilePath in arrFiles)
                    {
                        if (!GetMonthDay(strFilePath, out nDay))
                            continue;

                        if (dtNow.Day > nDay)
                            File.Delete(strFilePath);
                    }
                }
            }
        }

        private bool GetMonthDay(string strFilePath, out int nDay)
        {
            nDay = -1;

            int nIndex1 = strFilePath.LastIndexOf('.');
            if (nIndex1 < 0)
                return false;

            string strExt = strFilePath.Substring(nIndex1 + 1);

            // 로그 파일만 삭제한다.
            if (string.Compare(strExt, m_strExt, true) != 0)
                return false;

            int nIndex2 = strFilePath.LastIndexOf('\\');
            if (nIndex2 < 0)
                return false;

            string strDate = strFilePath.Substring(nIndex2 + 1, nIndex1 - nIndex2 - 1);
            string strDay = strDate.Substring(strDate.Length - 2);

            if (!int.TryParse(strDay, out nDay))
                return false;

            return true;
        }

        private void DeleteFolder(string strFolderPath)
        {
            string[] arrFiles = Directory.GetFiles(strFolderPath);

            foreach (string strFilePath in arrFiles)
            {
                File.Delete(strFilePath);
            }

            Directory.Delete(strFolderPath);
        }

        public void WriteCheckLog(FireEquipment equip, FireEquipmentHistory equipHistory = null)
        {
            DateTime dtNow = DateTime.Now;
            WriteCheckLog(equip, dtNow, equipHistory);
        }

        private void WriteCheckLog(FireEquipment equip, DateTime dtNow, FireEquipmentHistory equipHistory)
        {
            if (equip == null)
                return;

            int nHistoryCount = m_arrEquipHistory.Count;
            bool logExist = false;

            for (int i = nHistoryCount - 1; i >= 0; i--)
            {
                EquipmentCheckHistory history = (EquipmentCheckHistory)m_arrEquipHistory[i];

                TimeSpan span = dtNow - history.Time;

                // m_nIgnoreTime이 경과한 로그는 지운다.
                if (span.TotalMinutes >= m_nIgnoreTime)
                {
                    for (int j = 0; j <= i; j++)
                    {
                        m_arrEquipHistory.RemoveAt(0);
                    }

                    break;
                }
                else if (!logExist)
                {
                    if (history.Equipment.ID == equip.ID)
                    {
                        if (equipHistory == null)
                            logExist = true;
                        else if (history.Status == equipHistory.Status)
                            logExist = true;
                    }
                }
            }

            if (!logExist)
            {
                string strFolder = m_strLogFolder + string.Format("\\{0}-{1:00}", dtNow.Year, dtNow.Month);

                if (!Directory.Exists(strFolder))
                {
                    Directory.CreateDirectory(strFolder);
                }

                string strFilePath = strFolder + string.Format("\\{0}{1:00}{2:00}.{3}", dtNow.Year, dtNow.Month, dtNow.Day, m_strExt);

                if (m_writer == null || m_strLogFilePath != strFilePath)
                {
                    m_strLogFilePath = strFilePath;

                    if (m_writer != null)
                        m_writer.Close();

                    m_arrEquipHistory.Clear();
                    m_dicEquipNo.Clear();

                    m_nHistoryNum = 0;
                    m_nFECheckCount = m_nHDCheckCount = m_nFACheckCount = 0;

                    string strColumns = string.Format("번호{0}점검시간{0}타입{0}관리번호{0}설비정보{0}설비상태{0}Zone이름{0}건물명{0}건물그룹", m_strDelimeter);

                    bool existFile = File.Exists(strFilePath);

                    if (existFile)
                        ReadLog(strFilePath, strColumns, dtNow);

                    m_writer = new StreamWriter(strFilePath, true, Encoding.UTF8);

                    if (!existFile)
                    {
                        FormMain2.Instance.IOManager.WriteEquipmentLog(m_writer, dtNow, m_strDelimeter);
                        m_writer.WriteLine(strColumns);
                        m_writer.Flush();
                    }

                    WriteCheckLog(equip, dtNow, equipHistory);
                    return;
                }

                EquipmentCheckHistory history = new EquipmentCheckHistory();
                history.Equipment = equip;
                history.Time = dtNow;
                history.Status = equipHistory == null ? GetLastEquipmentStatus(equip) : equipHistory.Status;

                WriteEquipmentLog(history);

                // history.Equipment에 대한 새로운 로그가 저장될 예정이므로
                // 해당 설비에 대한 이전 로그는 삭제한다.
                RemoveEquipHistory(history.Equipment);
                m_arrEquipHistory.Add(history);
            }
        }

        private void ReadLog(string strFilePath, string strColumns, DateTime dtNow)
        {
            StreamReader reader = new StreamReader(strFilePath, Encoding.UTF8);
            bool findData = false;

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine();

                if (strLine.Length == 0)
                    continue;

                if (!findData)
                {
                    if (strLine == strColumns)
                        findData = true;
                }
                else
                {
                    ReadLineData(strLine, dtNow);
                }
            }

            reader.Close();
        }

        private void ReadLineData(string strLine, DateTime dtNow)
        {
            //"1	22:11:10	소화기	248	2 ATO036 AVR Room FE AA1 4.5k 12	양호"
            int nBeginIndex = 0;

            int num = 0, hour = 0, min = 0, sec = 0;
            FireEquipment.EquipmentType type = FireEquipment.EquipmentType.UNKNOWN;
            string strEquipID = "";
            FireEquipmentHistory.EquipmentStatus status = FireEquipmentHistory.EquipmentStatus.ETC;

            for (int i = 0; i < 6; i++)
            {
                int nIndex = strLine.IndexOf(m_strDelimeter, nBeginIndex);

                if (nIndex < 0)
                    return;

                string strToken = strLine.Substring(nBeginIndex, nIndex - nBeginIndex);

                if (i == 0)
                {
                    if (!int.TryParse(strToken, out num))
                        return;
                }
                else if (i == 1)
                {
                    string[] arrData = strToken.Split(':');

                    if (arrData == null || arrData.Count() != 3)
                        return;

                    if (!int.TryParse(arrData[0], out hour))
                        return;

                    if (!int.TryParse(arrData[1], out min))
                        return;

                    if (!int.TryParse(arrData[2], out sec))
                        return;
                }
                else if (i == 2)
                {
                    type = FireEquipment.ToEquipmentType(strToken);
                }
                else if (i == 3)
                {
                    strEquipID = strToken;
                }
                else if (i == 5)
                {
                    status = FireEquipmentHistory.ToEquipmentStatus(strToken);
                }

                nBeginIndex = nIndex + 1;
            }

            FireEquipment equip = FormMain2.Instance.IOManager.FindEquipment(type, strEquipID);

            // 새로운 설비일때
            if (!m_dicEquipNo.ContainsKey(equip))
            {
                if (type == FireEquipment.EquipmentType.FE)
                    m_nFECheckCount++;
                else if (type == FireEquipment.EquipmentType.HD)
                    m_nHDCheckCount++;
                else if (type == FireEquipment.EquipmentType.FA)
                    m_nFACheckCount++;
            }

            m_dicEquipNo[equip] = num;

            if (m_nHistoryNum < num)
                m_nHistoryNum = num;

            DateTime dtTime = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, hour, min, sec);
            TimeSpan span = dtNow - dtTime;

            // m_nIgnoreTime이 경과하였는가?
            if (span.TotalMinutes >= m_nIgnoreTime)
                return;

            if (equip == null)
                return;

            EquipmentCheckHistory history = new EquipmentCheckHistory();

            history.Equipment = equip;
            history.Time = dtTime;
            history.Status = status;

            // history.Equipment에 대한 새로운 로그가 저장될 예정이므로
            // 해당 설비에 대한 이전 로그는 삭제한다.
            RemoveEquipHistory(history.Equipment);
            m_arrEquipHistory.Add(history);
        }

        private void RemoveEquipHistory(FireEquipment equip)
        {
            foreach (EquipmentCheckHistory history in m_arrEquipHistory)
            {
                if (history.Equipment == equip)
                {
                    m_arrEquipHistory.Remove(history);
                    return;
                }
            }
        }

        private FireEquipmentHistory.EquipmentStatus GetLastEquipmentStatus(FireEquipment equip)
        {
            ArrayList arrEquipHistories = FormMain2.Instance.IOManager.FindEquipmentHistoryList(equip.ID);

            if (arrEquipHistories != null)
            {
                int nCount = arrEquipHistories.Count;

                if (nCount > 0)
                {
                    FireEquipmentHistory equipHistory = (FireEquipmentHistory)arrEquipHistories[nCount - 1];
                    return equipHistory.Status;
                }
            }

            return EquipmentCheckHistory.DefaultStatus;
        }

        private void WriteEquipmentLog(EquipmentCheckHistory history)
        {
            string strStatus = FireEquipmentHistory.GetStatusText(history.Status);

            Zone zone = history.Equipment.Zone;

            string strBuildingName = "", strBuildingGroupName = "";

            if (zone.Building != null)
            {
                strBuildingName = zone.Building.BuildingName;
                strBuildingGroupName = zone.Building.BuildingGroup.BuildingGroupName;
            }

            // 점검 번호 설정
            // 점검 번호는 몇 개의 시설물을 점검하였는지에 대한 의미가 있으므로
            // 한 설비를 여러번 점검할 경우 점검 번호를 늘려나가지 않는다.
            int num = 0;

            if (m_dicEquipNo.ContainsKey(history.Equipment))
                num = m_dicEquipNo[history.Equipment];
            else
            {
                num = ++m_nHistoryNum;
                m_dicEquipNo[history.Equipment] = num;

                if (history.Equipment.Type == FireEquipment.EquipmentType.FE)
                    m_nFECheckCount++;
                else if (history.Equipment.Type == FireEquipment.EquipmentType.HD)
                    m_nHDCheckCount++;
                else if (history.Equipment.Type == FireEquipment.EquipmentType.FA)
                    m_nFACheckCount++;
            }

            // 오늘 하루 전체 점검 기록을 파일 첫머리에 기록한다.
            WriteTotalCheckLog(history.Time);

            string strLine = string.Format("{0}{11}{1:00}:{2:00}:{3:00}{11}{4}{11}{5}{11}{6}{11}{7}{11}{8}{11}{9}{11}{10}",
                num,
                history.Time.Hour, history.Time.Minute, history.Time.Second,
                FireEquipment.GetTypeName(history.Equipment.Type),
                history.Equipment.EquipID,
                history.Equipment.RFIDTagID,
                strStatus,
                zone.ZoneName,
                strBuildingName,
                strBuildingGroupName,
                m_strDelimeter);

            m_writer.WriteLine(strLine);
            m_writer.Flush();
        }

        private void WriteTotalCheckLog(DateTime dtToday)
        {
            m_writer.Close();

            StreamReader reader = new StreamReader(m_strLogFilePath, Encoding.UTF8);
            string strAll = "";

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine();

                if (strLine.IndexOf("현재 설비 전체 개수") >= 0)
                {
                    strAll = strLine + "\r\n" + reader.ReadToEnd();
                    break;
                }
            }

            reader.Close();

            m_writer = new StreamWriter(m_strLogFilePath, false, Encoding.UTF8);

            string str = string.Format("{0}년 {1}월 {2}일 점검한 설비 전체 개수{4}{3}",
                dtToday.Year, dtToday.Month, dtToday.Day, m_nHistoryNum, m_strDelimeter);
            m_writer.WriteLine(str);

            if (m_nFECheckCount > 0)
            {
                m_writer.WriteLine(string.Format("{0}년 {1}월 {2}일 점검한 소화기 개수{4}{3}",
                    dtToday.Year, dtToday.Month, dtToday.Day, m_nFECheckCount, m_strDelimeter));
            }

            if (m_nHDCheckCount > 0)
            {
                m_writer.WriteLine(string.Format("{0}년 {1}월 {2}일 점검한 소화전 개수{4}{3}",
                    dtToday.Year, dtToday.Month, dtToday.Day, m_nHDCheckCount, m_strDelimeter));
            }

            if (m_nFACheckCount > 0)
            {
                m_writer.WriteLine(string.Format("{0}년 {1}월 {2}일 점검한 발신기 개수{4}{3}",
                    dtToday.Year, dtToday.Month, dtToday.Day, m_nFACheckCount, m_strDelimeter));
            }

            m_writer.WriteLine();
            m_writer.WriteLine();
            m_writer.Write(strAll);
        }
    }
}
