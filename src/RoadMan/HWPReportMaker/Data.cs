using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HWPReportMaker
{
    public class Project
    {
        private string m_strRegionName = "";
        private List<Process> m_listProcess = new List<Process>();

        public string RegionName
        {
            get { return m_strRegionName; }
            set { m_strRegionName = value; }
        }

        public List<Process> ProcessList
        {
            get { return m_listProcess; }
        }
    }

    public class Process
    {
        private string m_strProcessName = "";
        private string m_strDescription = "";
        private VariousData<int> m_nBeginYear = null;
        private VariousData<int> m_nEndYear = null;
        private List<Street> m_listStreet = new List<Street>();

        public string ProcessName
        {
            get { return m_strProcessName; }
            set { m_strProcessName = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set
            {
                ParseDescription(value);
                m_strDescription = value;
            }
        }

        public VariousData<int> BeginYear
        {
            get { return m_nBeginYear; }
            set { m_nBeginYear = value; }
        }

        public VariousData<int> EndYear
        {
            get { return m_nEndYear; }
            set { m_nEndYear = value; }
        }

        public List<Street> StreetList
        {
            get { return m_listStreet; }
        }

        private void ParseDescription(string strDescription)
        {
            strDescription = strDescription.Trim();

            char delimeter = ' ';

            if (strDescription.Contains(' '))
                delimeter = ' ';
            else if (strDescription.Contains('~'))
                delimeter = '~';
            else if (strDescription.Contains('-'))
                delimeter = '-';

            string[] strTokens = strDescription.Split(delimeter);

            int nYear;
            int nMinYear = -1, nMaxYear = -1;

            foreach (string strToken in strTokens)
            {
                if (StringToInt(strToken, out nYear))
                {
                    if (nYear > 0)
                    {
                        if (nMinYear < 0 || nMinYear > nYear)
                            nMinYear = nYear;

                        if (nMaxYear < nYear)
                            nMaxYear = nYear;
                    }
                }
            }

            int nTokenCount = strTokens.Count();

            if (nTokenCount == 0)
                return;

            string strFirst = strTokens[0];
            string strLast = strTokens[nTokenCount - 1];

            if (strFirst.StartsWith("~") || strFirst.StartsWith("-") || strLast.Contains("이전"))
            {
                if (nMaxYear > 0)
                    m_nEndYear = new VariousData<int>(nMaxYear);
                else
                    m_nEndYear = null;

                m_nBeginYear = null;
            }
            else if (strLast.EndsWith("~") || strLast.EndsWith("-") || strLast.Contains("이후"))
            {
                if (nMinYear > 0)
                    m_nBeginYear = new VariousData<int>(nMinYear);
                else
                    m_nBeginYear = null;

                m_nEndYear = null;
            }
            else
            {
                if (nMinYear > 0)
                    m_nBeginYear = new VariousData<int>(nMinYear);
                else
                    m_nBeginYear = null;

                if (nMaxYear > 0)
                    m_nEndYear = new VariousData<int>(nMaxYear);
                else
                    m_nEndYear = null;
            }
        }

        private bool StringToInt(string str, out int num)
        {
            int len = str.Length;
            num = 0;

            bool begin = false;

            for (int i=0;i<len;i++)
            {
                char ch = str.ElementAt(i);

                if (!begin)
                {
                    if (ch >= '0' && ch <= '9')
                    {
                        begin = true;
                    }
                }

                if (begin)
                {
                    if (ch >= '0' && ch <= '9')
                    {
                        num = num * 10 + ch - '0';
                    }
                    else
                        break;
                }
            }

            return begin;
        }
    }

    public class Street
    {
        private string m_strStreetName = "";
        private string m_strCategoryName = "";
        private string m_strSubCategoryName = "";
        private VariousData<int> m_nScheduleArea = null;
        private VariousData<int> m_nCompleteArea = null;
        private VariousData<long> m_nScheduleCost = null;
        private VariousData<long> m_nResultCost = null;
        private VariousData<DateTime> m_dtFirst = null;
        private VariousData<DateTime> m_dtFinal = null;
        private Process m_process = null;

        public string StreetName
        {
            get { return m_strStreetName; }
            set { m_strStreetName = value; }
        }

        public string CategoryName
        {
            get { return m_strCategoryName; }
            set { m_strCategoryName = value; }
        }

        public string SubCategoryName
        {
            get { return m_strSubCategoryName; }
            set { m_strSubCategoryName = value; }
        }

        public VariousData<int> ScheduleArea
        {
            get { return m_nScheduleArea; }
            set { m_nScheduleArea = value; }
        }

        public VariousData<int> CompleteArea
        {
            get { return m_nCompleteArea; }
            set { m_nCompleteArea = value; }
        }

        public VariousData<long> ScheduleCost
        {
            get { return m_nScheduleCost; }
            set { m_nScheduleCost = value; }
        }

        public VariousData<long> ResultCost
        {
            get { return m_nResultCost; }
            set { m_nResultCost = value; }
        }

        public VariousData<DateTime> FirstDate
        {
            get { return m_dtFirst; }
            set { m_dtFirst = value; }
        }

        public VariousData<DateTime> FinalDate
        {
            get { return m_dtFinal; }
            set { m_dtFinal = value; }
        }

        public Process Process
        {
            get { return m_process; }
            set { m_process = value; }
        }
    }

    // struct와 같이 null이 허용되지 않는 데이터를 위한 Wrapper 클래스
    public class VariousData<DataType>
    {
        private DataType data;

        public DataType Data
        {
            get { return data; }
            set { data = value; }
        }

        public VariousData()
        {
        }

        public VariousData(DataType data)
        {
            this.data = data;
        }
    }

    public class CategoryData
    {
        private List<SubCategoryData> m_listSubCategoryData = new List<SubCategoryData>();
        private string m_strCategoryName = "";

        public string CategoryName
        {
            get { return m_strCategoryName; }
            set { m_strCategoryName = value; }
        }

        public List<SubCategoryData> SubCategoryDataList
        {
            get { return m_listSubCategoryData; }
        }

        public SubCategoryData GetSubCategory(string strSubCategoryName)
        {
            foreach (SubCategoryData data in m_listSubCategoryData)
            {
                if (data.SubCategoryName == strSubCategoryName)
                    return data;
            }

            return null;
        }

        public void Sort()
        {
            SubCategoryData subBig = GetSubCategory("대로");
            SubCategoryData subMiddle = GetSubCategory("중로");
            SubCategoryData subSmall = GetSubCategory("소로");

            if (subSmall != null)
                m_listSubCategoryData.Remove(subSmall);
            
            if (subMiddle != null)
                m_listSubCategoryData.Remove(subMiddle);
            
            if (subBig != null)
                m_listSubCategoryData.Remove(subBig);
            
            m_listSubCategoryData.Sort();

            if (subSmall != null)
                m_listSubCategoryData.Insert(0, subSmall);

            if (subMiddle != null)
                m_listSubCategoryData.Insert(0, subMiddle);

            if (subBig != null)
                m_listSubCategoryData.Insert(0, subBig);
        }
    }

    public class SubCategoryData : IComparable
    {
        private string m_strSubCategoryName = "";
        private List<Street> m_listStreet = new List<Street>();
        
        public string SubCategoryName
        {
            get { return m_strSubCategoryName; }
            set { m_strSubCategoryName = value; }
        }

        public List<Street> StreetList
        {
            get { return m_listStreet; }
        }

        public int CompareTo(object obj)
        {
            SubCategoryData data = (SubCategoryData)obj;
            return this.m_strSubCategoryName.CompareTo(data.m_strSubCategoryName);
        }
    }

    public class HWPPosition
    {
        private int m_nList = 0;
        private int m_nParameter = 0;
        private int m_nPosition = 0;
        private object m_tag = null;

        public int List
        {
            get { return m_nList; }
            set { m_nList = value; }
        }

        public int Parameter
        {
            get { return m_nParameter; }
            set { m_nParameter = value; }
        }

        public int Position
        {
            get { return m_nPosition; }
            set { m_nPosition = value; }
        }

        public object Tag
        {
            get { return m_tag; }
            set { m_tag = value; }
        }

        public HWPPosition()
        {
        }

        public HWPPosition(int nList, int nParameter, int nPosition)
        {
            m_nList = nList;
            m_nParameter = nParameter;
            m_nPosition = nPosition;
        }

        public HWPPosition Clone()
        {
            return new HWPPosition(this.List, this.Parameter, this.Position);
        }
    }

    public class AreaData
    {
        private VariousData<int> m_nScheduleArea = null;
        private VariousData<int> m_nCompleteArea = null;
        private VariousData<int> m_nCount = null;

        public VariousData<int> ScheduleArea
        {
            get { return m_nScheduleArea; }
            set { m_nScheduleArea = value; }
        }

        public VariousData<int> CompleteArea
        {
            get { return m_nCompleteArea; }
            set { m_nCompleteArea = value; }
        }

        public VariousData<int> Count
        {
            get { return m_nCount; }
            set { m_nCount = value; }
        }
    }

    public class SubCategoryProcess : IComparable
    {
        private string m_strSubCategoryName = "";
        // Key : Process Name
        private Dictionary<string, List<Street>> m_dicStreetProcess = new Dictionary<string,List<Street>>();
        private List<string> m_listStreetNames = new List<string>();

        public string SubCategoryName
        {
            get { return m_strSubCategoryName; }
            set { m_strSubCategoryName = value; }
        }

        public Dictionary<string, List<Street>> StreetProcessList
        {
            get { return m_dicStreetProcess; }
        }

        public List<string> StreetNames
        {
            get { return m_listStreetNames; }
        }

        public int CompareTo(object obj)
        {
            SubCategoryProcess process = (SubCategoryProcess)obj;
            return this.m_strSubCategoryName.CompareTo(process.m_strSubCategoryName);
        }
    }
}
