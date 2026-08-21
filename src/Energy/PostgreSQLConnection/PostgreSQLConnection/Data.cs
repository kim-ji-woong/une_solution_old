using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace PostgreSQLConnection
{
    //public class Company
    //{
    //    private string m_strName = "";
    //    private ConcurrentDictionary<Worker, Worker> m_workers = new ConcurrentDictionary<Worker, Worker>();
    //    //private Dictionary<Worker, Worker> m_workers = new Dictionary<Worker, Worker>();
    //    public string Name
    //    {
    //        get { return m_strName; }
    //        set { m_strName = value; }
    //    }

    //    public List<Worker> Workers
    //    {
    //        get { return m_workers.Values.ToList(); }
    //    }

    //    public void AddWorker(Worker worker)
    //    {
    //        m_workers[worker] = worker;
    //    }

    //    public void RemoveWorker(Worker worker)
    //    {
    //        Worker temp;
    //        m_workers.TryRemove(worker, out temp);
    //    }
    //}

    //public class Location
    //{
    //    private string m_strName = "";
    //    // 등록된 전체 인원
    //    private ConcurrentDictionary<Worker, Worker> m_totalWorkers = new ConcurrentDictionary<Worker, Worker>();
    //    // 현재 작업중인 인원
    //    private ConcurrentDictionary<Worker, Worker> m_workingWorkers = new ConcurrentDictionary<Worker, Worker>();

    //    public string Name
    //    {
    //        get { return m_strName; }
    //        set { m_strName = value; }
    //    }

    //    public List<Worker> TotalWorkers
    //    {
    //        get { return m_totalWorkers.Values.ToList(); }
    //    }

    //    public List<Worker> WorkingWorkers
    //    {
    //        get { return m_workingWorkers.Values.ToList(); }
    //    }

    //    public void AddWorker(Worker worker)
    //    {
    //        m_totalWorkers[worker] = worker;
    //    }

    //    public void RemoveWorker(Worker worker)
    //    {
    //        Worker temp;

    //        // totalWorker에서 제외하면 workingWorker에서도 무조건 삭제해야 한다.
    //        m_totalWorkers.TryRemove(worker, out temp);
    //        m_workingWorkers.TryRemove(worker, out temp);
    //    }

    //    public void AddWorkerInWork(Worker worker)
    //    {
    //        m_totalWorkers[worker] = worker;
    //        m_workingWorkers[worker] = worker;
    //    }

    //    public void RemoveWorkerInWork(Worker worker)
    //    {
    //        Worker temp;
    //        m_workingWorkers.TryRemove(worker, out temp);
    //    }
    //}

    //// 공종
    //public class Department
    //{
    //    private string m_strName = "";

    //    public string Name
    //    {
    //        get { return m_strName; }
    //        set { m_strName = value; }
    //    }
    //}

    //public class Worker
    //{
    //    // DB ID
    //    private int m_nID = 0;
    //    // 사번
    //    private string m_strCompanyID = "";
    //    private string m_strName = "";
    //    private Location m_location = null;
    //    private Department m_department = null;
    //    private Company m_company = null;
    //    // 작업장에서 작업중인가?
    //    private bool m_inWork = false;

    //    private int m_nHeader = 0, m_nBody = 0, m_nTail = 0;
    //    private int m_nBodyLen = 0;

    //    public int ID
    //    {
    //        get { return m_nID; }
    //        set { m_nID = value; }
    //    }

    //    public string CompanyID
    //    {
    //        get { return m_strCompanyID; }
    //        set { m_strCompanyID = value; }
    //    }

    //    public string Name
    //    {
    //        get { return m_strName; }
    //        set { m_strName = value; }
    //    }

    //    public string PhoneNumber
    //    {
    //        get
    //        {
    //            if (m_nBodyLen == 3)
    //                return string.Format("01{0}{1:000}{2:0000}", m_nHeader, m_nBody, m_nTail);
    //            else if (m_nBodyLen == 4)
    //                return string.Format("01{0}{1:0000}{2:0000}", m_nHeader, m_nBody, m_nTail);

    //            return "";
    //        }
    //        set
    //        {
    //            if (!String.IsNullOrWhiteSpace(value))
    //            {
    //                SetPhoneNumber(value);
    //            }
    //        }
    //    }

    //    public Location Location
    //    {
    //        get { return m_location; }
    //        set
    //        {
    //            if (m_location != value)
    //            {
    //                if (m_location != null)
    //                    m_location.RemoveWorker(this);

    //                m_location = value;

    //                if (m_location != null)
    //                    m_location.AddWorker(this);
    //            }
    //        }
    //    }

    //    public Department Department
    //    {
    //        get { return m_department; }
    //        set { m_department = value; }
    //    }

    //    public Company Company
    //    {
    //        get { return m_company; }
    //        set
    //        {
    //            if (m_company != value)
    //            {
    //                if (m_company != null)
    //                    m_company.RemoveWorker(this);

    //                m_company = value;

    //                if (m_company != null)
    //                    m_company.AddWorker(this);
    //            }
    //        }
    //    }

    //    public bool InWork
    //    {
    //        get { return m_inWork; }
    //        set
    //        {
    //            if (m_inWork != value)
    //            {
    //                m_inWork = value;

    //                if (m_location != null)
    //                {
    //                    if (m_inWork)
    //                        m_location.AddWorkerInWork(this);
    //                    else
    //                        m_location.RemoveWorkerInWork(this);
    //                }
    //            }
    //        }
    //    }

    //    private void SetPhoneNumber(string strPhoneNumber)
    //    {
    //        string[] arrTokens = strPhoneNumber.Trim().Split('-');
    //        int nTokenCount = arrTokens.Count();

    //        m_nHeader = m_nBody = m_nTail = m_nBodyLen = 0;

    //        if (nTokenCount == 3)
    //            SetPhoneNumber2(arrTokens[0].Trim(), arrTokens[1].Trim(), arrTokens[2].Trim());
    //        else if (nTokenCount == 2)
    //            SetPhoneNumber2(arrTokens[0].Trim() + arrTokens[1].Trim());
    //        else if (nTokenCount == 1)
    //            SetPhoneNumber2(strPhoneNumber.Trim());
    //    }

    //    private bool SetPhoneNumber2(string strHead, string strBody, string strTail)
    //    {
    //        if (!strHead.StartsWith("01") || strHead.Length != 3)
    //            return false;

    //        char chHead = strHead.ElementAt(2);

    //        if (chHead < '0' || chHead > '9')
    //            return false;

    //        int nBody = 0, nTail = 0;
    //        int nBodyLen = strBody.Length;
    //        int nTailLen = strTail.Length;

    //        if (nBodyLen < 3 || nBodyLen > 4 || nTailLen != 4)
    //            return false;

    //        if (!int.TryParse(strBody, out nBody))
    //            return false;

    //        if (!int.TryParse(strTail, out nTail))
    //            return false;

    //        m_nHeader = chHead - '0';
    //        m_nBody = nBody;
    //        m_nTail = nTail;
    //        m_nBodyLen = nBodyLen;

    //        return true;
    //    }

    //    private bool SetPhoneNumber2(string strPhoneNumber)
    //    {
    //        int len = strPhoneNumber.Length;

    //        bool readNum = false;
    //        int nIndex1 = -1, nIndex2 = -1;

    //        for (int i = 0; i < len; i++)
    //        {
    //            char ch = strPhoneNumber.ElementAt(i);

    //            if (ch >= '0' && ch <= '9')
    //            {
    //                readNum = true;
    //            }
    //            else if (ch == ' ' || ch == '\t')
    //            {
    //                if (readNum)
    //                {
    //                    readNum = false;

    //                    if (nIndex1 < 0)
    //                        nIndex1 = i;
    //                    else
    //                    {
    //                        nIndex2 = i;
    //                        break;
    //                    }
    //                }
    //            }
    //        }

    //        if (nIndex1 >= 0 && nIndex2 > nIndex1)
    //        {
    //            string str1 = strPhoneNumber.Substring(0, nIndex1).Trim();
    //            string str2 = strPhoneNumber.Substring(nIndex1, nIndex2 - nIndex1 - 1).Trim();
    //            string str3 = strPhoneNumber.Substring(nIndex2).Trim();

    //            return SetPhoneNumber2(str1, str2, str3);
    //        }
    //        else if (nIndex1 >= 0)
    //        {
    //            string str1 = strPhoneNumber.Substring(0, nIndex1).Trim();
    //            string str2 = strPhoneNumber.Substring(nIndex1).Trim();

    //            int len1 = str1.Length;
    //            int len2 = str2.Length;

    //            if (len1 == 3 && (len2 == 7 || len2 == 8))
    //            {
    //                return SetPhoneNumber2(str1, str2.Substring(0, len2 - 4), str2.Substring(len2 - 4));
    //            }
    //            else if ((len1 == 6 || len1 == 7) || len2 == 4)
    //            {
    //                return SetPhoneNumber2(str1.Substring(0, 3), str1.Substring(3), str2);
    //            }
    //        }
    //        else
    //        {
    //            if (len == 10 || len == 11)
    //            {
    //                string str1 = strPhoneNumber.Substring(0, 3);
    //                string str2 = strPhoneNumber.Substring(3, len - 7);
    //                string str3 = strPhoneNumber.Substring(len - 4);

    //                return SetPhoneNumber2(str1, str2, str3);
    //            }
    //        }

    //        return false;
    //    }
    //}
}
