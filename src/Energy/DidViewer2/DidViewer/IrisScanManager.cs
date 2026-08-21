using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;

namespace DidViewer
{
    public class IrisScanManager
    {
        private Dictionary<string, Company> m_dicCompanies = new Dictionary<string, Company>();
        public Dictionary<string, Company> DicCompanies
        {
            get { return m_dicCompanies; }
        }

        public int StayCompanyCount
        {
            get
            {
                int showCompanyCount = 0;
                foreach (KeyValuePair<string, Company> item in m_dicCompanies)
                {
                    int cnt = item.Value.Workers.Where(p => p.InWork).Count();
                    if (cnt > 0)
                        showCompanyCount++;
                }

                return showCompanyCount;
            }
        }

        private Dictionary<string, Location> m_dicLocations = new Dictionary<string, Location>();
        private Dictionary<string, Department> m_dicDepartments = new Dictionary<string, Department>();
        private Dictionary<string, Worker> m_dicWorkers = new Dictionary<string, Worker>();
        public Dictionary<string, Worker> Workers
        {
            get { return m_dicWorkers; }
            set { m_dicWorkers = value; }
        }

        private int m_nHiCount = 0;
        public int HiCount
        {
            get { return m_nHiCount; }
        }
        private int m_nByeCount = 0;
        public int ByeCount
        {
            get { return m_nByeCount; }
        }
        private int m_nStayCount = 0;
        public int StayCount
        {
            get { return m_nStayCount; }
        }

        private WebDBManager m_dbMgr = null;

        public IrisScanManager(WebDBManager dbMgr)
        {
            m_dbMgr = dbMgr;

            // 처음 실행하면 00시부터
            DateTime dtNow = DateTime.Now;
            m_dtPrev = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, 0, 0, 0);
        }

        /// <summary>
        /// 지정된 시간에 초기화 시킨다.
        /// </summary>
        public void Init()
        {
            m_dicCompanies.Clear();
            m_dicLocations.Clear();
            m_dicDepartments.Clear();
            m_dicWorkers.Clear();

            DateTime temp = m_dtPrev.AddDays(1);
            m_dtPrev = new DateTime(temp.Year, temp.Month, temp.Day, 0, 0, 0);

            m_nHiCount = m_nByeCount = m_nStayCount = 0;
        }
        
        private int m_nLastLogIndex = 0;
        private DateTime m_dtPrev = new DateTime();

        public void DisplayUser()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT ID, employee_user_id, name, company, location, department, position, eventtime, door_name ");
            sb.Append("  FROM access_log ");
            sb.AppendFormat(" WHERE eventtime > '{0}' ", GetTimeString(m_dtPrev));
            sb.AppendFormat(" AND ID > " + m_nLastLogIndex);
            sb.Append(" ORDER BY ID ");
            
            ArrayList arrResult = m_dbMgr.GetResultData(sb.ToString());
            if (arrResult == null || arrResult.Count == 0)
                return;

            Dictionary<string, Worker> dicPrevWorkers = m_dicWorkers;

            Company company = null;
            Location location = null;
            Department department = null;

            for (int i = 0; i < arrResult.Count; i += 9)
            {
                string strLogIndex = WebDBManager.GetStringField(arrResult[i]);
                string strUserID = WebDBManager.GetStringField(arrResult[i + 1]);
                string strName = WebDBManager.GetStringField(arrResult[i + 2]);
                string strCompany = WebDBManager.GetStringField(arrResult[i + 3]);
                string strLocation = WebDBManager.GetStringField(arrResult[i + 4]);
                string strDepartment = WebDBManager.GetStringField(arrResult[i + 5]);
                string strPosition = WebDBManager.GetStringField(arrResult[i + 6]);
                string strEventTime = WebDBManager.GetStringField(arrResult[i + 7]);                
                string strDoorName = WebDBManager.GetStringField(arrResult[i + 8]);

                m_nLastLogIndex = Convert.ToInt32(strLogIndex);

                if (m_dicCompanies.TryGetValue(strCompany, out company) == false)
                {
                    company = new Company();
                    company.Name = strCompany;
                    m_dicCompanies[strCompany] = company;
                }

                if (m_dicLocations.TryGetValue(strLocation, out location) == false)
                {
                    location = new Location();
                    location.Name = strLocation;
                    m_dicLocations[strLocation] = location;
                }

                if (m_dicDepartments.TryGetValue(strDepartment, out department) == false)
                {
                    department = new Department();
                    department.Name = strDepartment;
                    m_dicDepartments[strDepartment] = department;
                }

                bool isBye = false;
                int nID;

                bool err = false;

                if (int.TryParse(strUserID, out nID))
                {
                    long nEventTime = Convert.ToDateTime(strEventTime).ToFileTime();
                    if (strDoorName == "출근기")
                    {
                        if (m_dicWorkers.ContainsKey(strUserID))
                        {
                            if (m_dicWorkers[strUserID].HiTime < nEventTime) // 이미 출근 기록이 있음
                                err = true;
                        }
                        else
                        {
                            Worker worker = new Worker();
                            worker.Company = company;
                            worker.Location = location;
                            worker.Department = department;
                            worker.Name = strName;
                            worker.ID = nID;
                            worker.CompanyID = strUserID;

                            worker.InWork = true;
                            worker.HiTime = nEventTime;
                            m_dicWorkers[strUserID] = worker;
                        }
                    }
                    else if (strDoorName == "퇴근기")
                    {
                        if (m_dicWorkers.ContainsKey(strUserID))
                        {
                            m_dicWorkers[strUserID].InWork = false;
                            if (m_dicWorkers[strUserID].ByeTime > 0)
                            {
                                if (m_dicWorkers[strUserID].ByeTime < nEventTime)
                                    m_dicWorkers[strUserID].ByeTime = nEventTime;

                                err = true;
                            }
                            else
                            {
                                m_dicWorkers[strUserID].ByeTime = nEventTime;
                                isBye = true;
                            }
                        }
                        else // 출근기록이 없는데 퇴근을 찍으면 에러임
                        {
                            err = true;
                        }
                    }
                    
                    //long nEventTime = Convert.ToDateTime(strEventTime).ToFileTime();
                    
                    //if (m_dicWorkers.ContainsKey(strUserID))
                    //{
                    //    if (m_dicWorkers[strUserID].HiTime < nEventTime)
                    //    {
                    //        if (m_dicWorkers[strUserID].HiTime > 0 && m_dicWorkers[strUserID].ByeTime > 0)
                    //        {
                    //            // 출퇴근 기록이 이미 있는데 또 찍은경우 제외
                    //            err = true; 
                    //        }
                            
                    //        m_dicWorkers[strUserID].InWork = false;
                    //        m_dicWorkers[strUserID].ByeTime = nEventTime;
                    //        isBye = true;
                    //    }
                    //}
                    //else
                    //{
                    //    Worker worker = new Worker();
                    //    worker.Company = company;
                    //    worker.Location = location;
                    //    worker.Department = department;
                    //    worker.Name = strName;
                    //    worker.ID = nID;
                    //    worker.CompanyID = strUserID;

                    //    worker.InWork = true;
                    //    worker.HiTime = nEventTime;
                    //    m_dicWorkers[strUserID] = worker;
                    //}
                }
                
                // 출근 로그가 찍힌 데이터만 들어오므로 무조건 +
                if (!err)
                {
                    if (!isBye)
                        m_nHiCount++;
                    else
                        m_nByeCount++; 
                }

                m_nStayCount = m_nHiCount - m_nByeCount;
            }

            Worker _worker;

            // 직원별 출입정보를 입력한다.
            foreach (KeyValuePair<string, Worker> pair in m_dicWorkers)
            {
                if (dicPrevWorkers.TryGetValue(pair.Key, out _worker))
                {
                    pair.Value.InWork = _worker.InWork;
                }
            }

            // 새로운 로그가 있으므로 업데이트한다
            FormMain.Instance.IsUpdateIris = true;
        }
                
        private string GetTimeString(DateTime timeStamp)
        {
            string strTime = string.Format("{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", timeStamp.Year, timeStamp.Month, timeStamp.Day, timeStamp.Hour, timeStamp.Minute, timeStamp.Second);
            return strTime;
        }
    }
}
