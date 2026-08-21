using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;

namespace RoadMan
{
    public class LayerData
    {
        private string m_strLayerName = "";
        private bool m_isVisible = true;
        private System.Drawing.Color m_color = new System.Drawing.Color();
        private DXFViewer.Layer m_linkedLayer = null;
        private int m_nAlpha = 255;
        private int m_nLayerIndex = 0;

		private bool m_bEnabled = true;
		public bool Enabled
		{
			get { return m_bEnabled; }
			set { m_bEnabled = value; }
		}

        public string LayerName
        {
            get { return m_strLayerName; }
            set { m_strLayerName = value; }
        }

        public bool Visible
        {
            get { return m_isVisible; }
            set { m_isVisible = value; }
        }

        // Alpha를 제외한 Color
        public System.Drawing.Color Color
        {
            get { return m_color; }
            set { m_color = value; }
        }

        public int Alpha
        {
            get { return m_nAlpha; }
            set { m_nAlpha = value; }
        }

        public int LayerIndex
        {
            get { return m_nLayerIndex; }
            set { m_nLayerIndex = value; }
        }

        public DXFViewer.Layer LinkedLayer
        {
            get { return m_linkedLayer; }
            set { m_linkedLayer = value; }
        }
    }

    public class ProcessSchedule : UndoRedoData
    {
		public ProcessSchedule() : base()
		{

		}

        private string m_strDescription = "";
        private string m_strScheduleName = "";
        private string m_strLength = "";
        private VariousData<int> m_nBeginYear = null;
        private VariousData<int> m_nEndYear = null;

		[XmlArray("SchedulePropertyArray")]
		[XmlArrayItem("ScheduleProperty")]
		public List<ScheduleProperty> m_listProperties = new List<ScheduleProperty>();

		
        public string ScheduleName
        {
            get { return m_strScheduleName; }
            set 
			{
				if( m_strScheduleName != value)
				{
					OnPropertyChanging("ScheduleName");
				}

				m_strScheduleName = value;
				OnPropertyChanged("ScheduleName");
			}
        }

        public string Length
        {
            get { return m_strLength; }
            set { m_strLength = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set 
			{

				if (m_strDescription != value)
				{
                    ParseDescription(value);
					OnPropertyChanging("Description");
				}
				m_strDescription = value; 
				OnPropertyChanged("Description");
			}
        }
		
		public IList<ScheduleProperty> Properties
        {
            get { return m_listProperties; }
        }

        // 총연장(m)
        public int TotalLength
        {
            get
            {
                int nTotalLength = 0;

                foreach (ScheduleProperty prop in m_listProperties)
                {
                    if (prop.Length != null)
                        nTotalLength += prop.Length.Data;
                }

                return nTotalLength;
            }
        }

        // 총면적(m²)
        public double TotalArea
        {
            get
            {
                double dTotalArea = 0;

                foreach (ScheduleProperty prop in m_listProperties)
                {
                    if (prop.Area != null)
                        dTotalArea += prop.Area.Data;
                }

                return dTotalArea;
            }
        }

        public int Totali
        {
            get
            {
                if (Options.Instance.CompleteRatioByArea)
                    return (int)TotalArea;
                //else
                    return (int)TotalLength;
            }
        }

        public double Totald
        {
            get
            {
                if (Options.Instance.CompleteRatioByArea)
                    return (double)TotalArea;
                //else
                return (double)TotalLength;
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

            for (int i = 0; i < len; i++)
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

    public class ScheduleProperty : UndoRedoData, IComparable
    {
        private ProcessSchedule m_schedule = null;
        private string m_strStreetName = "";                      // 노선명
        //private VariousData<double> m_dImportance = null;   // 시급성
        private ImportanceData m_importance = null;
        private VariousData<double> m_dWidth = null;        // 도로폭(m)
        private VariousData<int> m_nLength = null;          // 도로의 총연장(m)
        //private string m_strLandAddr = "";                  // 토지지번
		
		[XmlArray("LandAddressDataArray")]
		[XmlArrayItem("LandAddressData")]
		public List<LandAddressData> m_arrLandAddr = new List<LandAddressData>();
        private VariousData<DateTime> m_dtFinal;            // 최종변경일자
        private bool m_isComplete = false;                  // 개설여부

		//[XmlArray("SchedulePropertySectorArray")]
		//[XmlArrayItem("SchedulePropertySector")]
        private List<SchedulePropertySector> m_sectors = new List<SchedulePropertySector>();

        private string m_strCategory = "";                  // 시설구분
        private string m_strSubCategory = "";               // 시설 세부
        private VariousData<DateTime> m_dtFirst;            // 최초결정일자
        private VariousData<double> m_dArea = null;         // 결정면적(m²)
        private VariousData<double> m_dRiceField = null;    // 지목(전) m²
        private VariousData<double> m_dField = null;        // 지목(답) m²
        private VariousData<double> m_dLand = null;         // 지목(대지) m²
        private VariousData<double> m_dETC = null;          // 지목(기타) m²
        private VariousData<long> m_nLandCost = null;        // 토지보상비(공시지가의 1.5배)(원)
        private VariousData<long> m_nObjectCost = null;      // 지장물보상비(원)
        private VariousData<long> m_nAroundCost = null;      // 개략공사비(원)

		[XmlIgnore]
        public ProcessSchedule Schedule
        {
            get { return m_schedule; }
            set { m_schedule = value; }
        }

        // 노선명
        public string StreetName
        {
            get { return m_strStreetName; }
            set { m_strStreetName = value; }
        }

        // 시급성
        public ImportanceData Importance
        {
            get { return m_importance; }
            set { m_importance = value; }
        }
        /*public VariousData<double> Importance
        {
            get { return m_dImportance; }
            set { m_dImportance = value; }
        }*/

        // 도로폭(m)
        public VariousData<double> Width
        {
            get { return m_dWidth; }
            set { m_dWidth = value; }
        }

        // 도로의 총연장(m)
        public VariousData<int> Length
        {
            get { return m_nLength; }
            set { m_nLength = value; }
        }

        // 토지지번
        public IList<LandAddressData> LandAddressDatas
        {
            get { return m_arrLandAddr; }
        }
        /*public string LandAddress
        {
            get { return m_strLandAddr; }
            set { m_strLandAddr = value; }
        }*/

        // 최종변경일자
        public VariousData<DateTime> FinalDate
        {
            get { return m_dtFinal; }
            set { m_dtFinal = value; }
        }

        // 사업비 총괄(원)
        public string TotalCost
        {
            get { return GetTotalCostString(m_nLandCost, m_nObjectCost, m_nAroundCost); }
        }

        // 개설여부
        public bool IsComplete
        {
            get { return m_isComplete; }
            set { m_isComplete = value; }
        }

        // 시설구분
        public string Category
        {
            get { return m_strCategory; }
            set { m_strCategory = value; }
        }

        // 시설 세부
        public string SubCategory
        {
            get { return m_strSubCategory; }
            set { m_strSubCategory = value; }
        }

        // 최초 결정일자
        public VariousData<DateTime> FirstDate
        {
            get { return m_dtFirst; }
            set { m_dtFirst = value; }
        }

        // 결정면적(m²)
        public VariousData<double> Area
        {
            get { return m_dArea; }
            set { m_dArea = value; }
        }

        // 지목(전) m²
        public VariousData<double> RiceField
        {
            get { return m_dRiceField; }
            set { m_dRiceField = value; }
        }

        // 지목(답) m²
        public VariousData<double> Field
        {
            get { return m_dField; }
            set { m_dField = value; }
        }

        // 지목(대지) m²
        public VariousData<double> Land
        {
            get { return m_dLand; }
            set { m_dLand = value; }
        }

        // 지목(기타) m²
        public VariousData<double> ETC
        {
            get { return m_dETC; }
            set { m_dETC = value; }
        }

        // 토지보상비(공시지가의 1.5배) - 원
        public VariousData<long> LandCost
        {
            get { return m_nLandCost; }
            set { m_nLandCost = value; }
        }

        // 지장물보상비 - 원
        public VariousData<long> ObjectCost
        {
            get { return m_nObjectCost; }
            set { m_nObjectCost = value; }
        }

        // 개략공사비 - 원
        public VariousData<long> AroundCost
        {
            get { return m_nAroundCost; }
            set { m_nAroundCost = value; }
        }

		
        public IList<SchedulePropertySector> Sectors
        {
            get { return m_sectors; }
        }

        public string GetFirstNLastLandAddressString()
        {
            return GetFirstNLastLandAddressString(m_arrLandAddr);
        }

        public string GetFirstLandAddressString()
        {
            return GetFirstLandAddressString(m_arrLandAddr);
        }

        public static string GetFirstNLastLandAddressString(List<LandAddressData> addrs)
        {
            string strFirst = GetFirstLandAddressString(addrs);
            string strLast = GetLastLandAddressString(addrs);

            if (strFirst.Length == 0 || strLast.Length == 0)
                return "";

            return strFirst + "(시점) ~ " + strLast + "(종점)";
        }

        public static string GetFirstLandAddressString(List<LandAddressData> addrs)
        {
            string strLandAddr = "";

            if (addrs.Count > 0)
            {
                LandAddressData addr = addrs[0];
                strLandAddr = addr.TownName + " " + addr.MajorAddr + "-" + addr.MinorAddr;
            }

            return strLandAddr;
        }

        public static string GetLastLandAddressString(List<LandAddressData> addrs)
        {
            string strLandAddr = "";

            if (addrs.Count > 0)
            {
                LandAddressData addr = addrs[addrs.Count - 1];
                strLandAddr = addr.TownName + " " + addr.MajorAddr + "-" + addr.MinorAddr;
            }

            return strLandAddr;
        }

        public static string GetTotalCostString(VariousData<long> nLandCost, VariousData<long> nObjectCost, VariousData<long> nAroundCost)
        {
            long nTotalCost = 0;

            if (nLandCost != null)
                nTotalCost += nLandCost.Data;

            if (nObjectCost != null)
                nTotalCost += nObjectCost.Data;

            if (nAroundCost != null)
                nTotalCost += nAroundCost.Data;

            if (nTotalCost == 0)
                return "";

            return string.Format("{0:###,###,###,###,###,###}원", nTotalCost);
            //return nTotalCost.ToString();
        }

        public long GetTotalCost()
        {
            long nTotalCost = 0;

            if (m_nLandCost != null)
                nTotalCost += m_nLandCost.Data;

            if (m_nObjectCost != null)
                nTotalCost += m_nObjectCost.Data;

            if (m_nAroundCost != null)
                nTotalCost += m_nAroundCost.Data;

            return nTotalCost;
        }

        public static string GetDateTimeString(DateTime dtTime)
        {
            return string.Format("{0}-{1}-{2}", dtTime.Year, dtTime.Month, dtTime.Day);
        }

        public static bool ReadDateTimeString(string strTime, out DateTime dtTime)
        {
            string[] arrDatas = strTime.Split('-');

            if (arrDatas.Count() == 3)
            {
                int nYear, nMonth, nDay;

                if (int.TryParse(arrDatas[0], out nYear))
                {
                    if (int.TryParse(arrDatas[1], out nMonth))
                    {
                        if (int.TryParse(arrDatas[2], out nDay))
                        {
                            dtTime = new DateTime(nYear, nMonth, nDay);
                            return true;
                        }
                    }
                }
            }

            dtTime = new DateTime();
            return false;
        }

        public List<LandAddressData> CloneLandAddressDataList()
        {
            List<LandAddressData> datas = new List<LandAddressData>();
            datas.AddRange(this.m_arrLandAddr);
            return datas;
        }

        private void CopyData<T>(ref VariousData<T> dataTrg, VariousData<T> dataSrc)
        {
            if (dataSrc == null)
                dataTrg = null;
            else
                dataTrg = new VariousData<T>(dataSrc.Data);
        }

        public ScheduleProperty Clone()
        {
            ScheduleProperty prop = new ScheduleProperty();

            prop.m_schedule = this.m_schedule;
            prop.m_strStreetName = this.m_strStreetName;
            prop.m_importance = this.m_importance == null ? null : this.m_importance.Clone();
            CopyData<double>(ref prop.m_dWidth, this.m_dWidth);
            CopyData<int>(ref prop.m_nLength, this.m_nLength);
            prop.m_arrLandAddr = CloneLandAddressDataList();
            CopyData<DateTime>(ref prop.m_dtFinal, this.m_dtFinal);
            CopyData<DateTime>(ref prop.m_dtFirst, this.m_dtFirst);
            prop.m_isComplete = this.m_isComplete;

            foreach (SchedulePropertySector sector in this.m_sectors)
            {
                prop.m_sectors.Add(sector.Clone());
            }

            prop.m_strCategory = this.m_strCategory;
            prop.m_strSubCategory = this.m_strSubCategory;
            CopyData<double>(ref prop.m_dArea, this.m_dArea);
            CopyData<double>(ref prop.m_dRiceField, this.m_dRiceField);
            CopyData<double>(ref prop.m_dField, this.m_dField);
            CopyData<double>(ref prop.m_dLand, this.m_dLand);
            CopyData<double>(ref prop.m_dETC, this.m_dETC);
            CopyData<long>(ref prop.m_nLandCost, this.m_nLandCost);
            CopyData<long>(ref prop.m_nObjectCost, this.m_nObjectCost);
            CopyData<long>(ref prop.m_nAroundCost, this.m_nAroundCost);

            return prop;
        }

        public void CopyFrom(ScheduleProperty prop)
        {
            this.m_schedule = prop.m_schedule;
            this.m_strStreetName = prop.m_strStreetName;
            this.m_importance = prop.m_importance == null ? null : prop.m_importance.Clone();
            CopyData<double>(ref this.m_dWidth, prop.m_dWidth);
            CopyData<int>(ref this.m_nLength, prop.m_nLength);
            this.m_arrLandAddr = prop.CloneLandAddressDataList();
            CopyData<DateTime>(ref this.m_dtFinal, prop.m_dtFinal);
            CopyData<DateTime>(ref this.m_dtFirst, prop.m_dtFirst);
            this.m_isComplete = prop.m_isComplete;

            this.m_sectors.Clear();

            foreach (SchedulePropertySector sector in prop.m_sectors)
            {
                this.m_sectors.Add(sector.Clone());
            }

            this.m_strCategory = prop.m_strCategory;
            this.m_strSubCategory = prop.m_strSubCategory;
            CopyData<double>(ref this.m_dArea, prop.m_dArea);
            CopyData<double>(ref this.m_dRiceField, prop.m_dRiceField);
            CopyData<double>(ref this.m_dField, prop.m_dField);
            CopyData<double>(ref this.m_dLand, prop.m_dLand);
            CopyData<double>(ref this.m_dETC, prop.m_dETC);
            CopyData<long>(ref this.m_nLandCost, prop.m_nLandCost);
            CopyData<long>(ref this.m_nObjectCost, prop.m_nObjectCost);
            CopyData<long>(ref this.m_nAroundCost, prop.m_nAroundCost);
        }

        public int CompareTo(object obj)
        {
            ScheduleProperty prop = (ScheduleProperty)obj;
            
            if (m_strStreetName.StartsWith("대로"))
            {
                if (prop.m_strStreetName.StartsWith("대로"))
                    return m_strStreetName.CompareTo(prop.m_strStreetName);
                else
                    return -1;
            }
            else if (m_strStreetName.StartsWith("중로"))
            {
                if (prop.m_strStreetName.StartsWith("대로"))
                    return 1;
                else if (prop.m_strStreetName.StartsWith("중로"))
                    return m_strStreetName.CompareTo(prop.m_strStreetName);
                else
                    return -1;
            }
            else if (m_strStreetName.StartsWith("소로"))
            {
                if (prop.m_strStreetName.StartsWith("대로"))
                    return 1;
                else if (prop.m_strStreetName.StartsWith("중로"))
                    return 1;
                else if (prop.m_strStreetName.StartsWith("소로"))
                    return m_strStreetName.CompareTo(prop.m_strStreetName);
                else
                    return -1;
            }
            else
            {
                if (prop.m_strStreetName.StartsWith("대로"))
                    return 1;
                else if (prop.m_strStreetName.StartsWith("중로"))
                    return 1;
                else if (prop.m_strStreetName.StartsWith("소로"))
                    return 1;
            }
            
            return m_strStreetName.CompareTo(prop.m_strStreetName);
        }
    }

    public class LandAddressData2 : LandAddressData
    {
        private List<DXFViewer.Hatch> m_hatchs = new List<DXFViewer.Hatch>();

        public List<DXFViewer.Hatch> Hatchs
        {
            get { return m_hatchs; }
        }

        public LandAddressData2()
        {
        }

        public LandAddressData2(string strLandAddr)
        {
            string[] arrNames = strLandAddr.Split(' ');
            int nCount = arrNames.Count();

            if (nCount <= 2)
            {
                this.TownName = strLandAddr.Trim();
            }
            else
            {
                string strDong = arrNames[0].Trim();
                string strRi = arrNames[1].Trim();
                this.TownName = (strDong + " " + strRi);

                int nIndex = strLandAddr.IndexOf(arrNames[2]);
                string strTail = strLandAddr.Substring(nIndex).Trim();

                string[] arrAddrs = strTail.Split('-');
                int nAddrCount = arrAddrs.Count();

                if (nAddrCount == 1)
                {
                    this.MajorAddr = strTail;
                }
                else
                {
                    this.MajorAddr = arrAddrs[0].Trim();
                    nIndex = strTail.IndexOf('-');
                    MinorAddr = strTail.Substring(nIndex + 1).Trim();
                }
            }
        }

        public LandAddressData2(LandAddressData data)
        {
            CopyFrom(data);
        }

        public override string ToString()
        {
            string strAddr = Ri == null || Ri.Length == 0 ? Dong : Dong + " " + Ri;

            if (MajorAddr == null || MajorAddr.Length == 0)
                return strAddr;
            else
                strAddr += " " + MajorAddr;

            if (MinorAddr == null || MinorAddr.Length == 0)
                return strAddr;
            else
                strAddr += "-" + MinorAddr;

            return strAddr;
        }

        public void MakeHatch(DXFViewer.PolyLine pLine)
        {
            if (pLine == null)
                return;

            int nVertexSize = pLine.GetVertexSize();

            if (nVertexSize > 0)
            {
                DXFViewer.Hatch hatch = new DXFViewer.Hatch();

                hatch.SetPointSize(nVertexSize);

                for (int i = 0; i < nVertexSize; i++)
                {
                    System.Drawing.PointF pt = pLine.GetVertex(i);
                    hatch.UpdatePoint(i, pt.X, pt.Y);
                }

                m_hatchs.Add(hatch);
            }
        }
    }

    public class LandAddressData : IComparable
    {
        private string m_strTownName = "";
        private string m_strDong = "";
        private string m_strRi = null;
        private string m_strMajorAddr = "";
        private string m_strMinorAddr = "";
        // 지적면적(m²) : 전체 토지 면적
        private VariousData<double> m_dTotalArea = null;
        // 편입면적(m²) : 전체 토지 가운데 도로에 편입될 면적
        private VariousData<double> m_dStreetArea = null;
        // 소유 구분
        private string m_strOwnerType = "";
        // 공시지가(원)
        private VariousData<long> m_nPublicEstimation = null;

        public string TownName
        {
            get { return m_strTownName; }
            set { ParseTownName(value); }
        }

        public string Dong
        {
            get { return m_strDong; }
        }

        public string Ri
        {
            get { return m_strRi; }
        }

        public string MajorAddr
        {
            get { return m_strMajorAddr; }
            set { ParseAddressNumber(value, ref m_strMajorAddr); }
        }

        public string MinorAddr
        {
            get { return m_strMinorAddr; }
            set { ParseAddressNumber(value, ref m_strMinorAddr); }
        }

		public override string ToString()
		{
            string strAddr = Ri == null || Ri.Length == 0 ? Dong : Dong + " " + Ri;

            if (MajorAddr == null || MajorAddr.Length == 0)
                return strAddr;
            else
                strAddr += " " + MajorAddr;

            if (MinorAddr == null || MinorAddr.Length == 0)
                return strAddr;
            else
                strAddr += "-" + MinorAddr;

            return strAddr;
			/*if (MinorAddr != null && MinorAddr != "")
				return TownName + " " + MajorAddr + "-" + MinorAddr;
			else
				return TownName + " " + MajorAddr;*/
		}

        // 지적면적(m²) : 전체 전체 면적
        public VariousData<double> TotalArea
        {
            get { return m_dTotalArea; }
            set { m_dTotalArea = value; }
        }

        // 편입면적(m²) : 전체 토지 가운데 도로에 편입될 면적
        public VariousData<double> StreetArea
        {
            get { return m_dStreetArea; }
            set { m_dStreetArea = value; }
        }

        // 소유 구분
        public string OwnerType
        {
            get { return m_strOwnerType; }
            set { m_strOwnerType = value; }
        }

        // 공시지가(원)
        public VariousData<long> PublicEstimation
        {
            get { return m_nPublicEstimation; }
            set { m_nPublicEstimation = value; }
        }

        public LandAddressData Clone()
        {
            LandAddressData data = new LandAddressData();
            data.CopyFrom(this);
            return data;
        }

        protected void CopyFrom(LandAddressData data)
        {
            this.m_strTownName = data.m_strTownName;
            this.m_strDong = data.m_strDong;
            this.m_strRi = data.m_strRi;
            this.m_strMajorAddr = data.m_strMajorAddr;
            this.m_strMinorAddr = data.m_strMinorAddr;
            this.m_dTotalArea = data.m_dTotalArea;
            this.m_dStreetArea = data.m_dStreetArea;
            this.m_strOwnerType = data.m_strOwnerType;
            this.m_nPublicEstimation = data.m_nPublicEstimation;
        }

        public int CompareTo(object obj)
        {
            LandAddressData data = (LandAddressData)obj;

            int nResult = m_strTownName.CompareTo(data.m_strTownName);

            if (nResult != 0)
                return nResult;

            nResult = m_strMajorAddr.CompareTo(data.m_strMajorAddr);

            if (nResult != 0)
                return nResult;

            return m_strMinorAddr.CompareTo(data.m_strMinorAddr);
        }

        private void ParseTownName(string strTownName)
        {
            m_strTownName = strTownName;
            m_strRi = null;

            string[] arrNames = strTownName.Split(' ');
            int nCount = arrNames.Count();

            if (nCount == 1)
            {
                m_strDong = arrNames[0].Trim();
            }
            else if (nCount == 2)
            {
                m_strDong = arrNames[0].Trim();
                m_strRi = arrNames[1].Trim();
            }
            else
            {
                m_strDong = arrNames[0].Trim();

                int nIndex = strTownName.IndexOf(m_strDong);
                m_strRi = strTownName.Substring(nIndex + strTownName.Length).Trim();

                if (m_strRi.Length == 0)
                    m_strRi = null;
            }
        }

        private void ParseAddressNumber(string strAddr, ref string strAddrNum)
        {
            string[] arrAddrs = strAddr.Split(' ');
            strAddrNum = "";

            foreach (string addr in arrAddrs)
            {
                strAddrNum += addr;
            }
        }
    }

    public class ImportanceData
    {
        private static double m_dPeopleRequestFactor = 0.25;
        private static double m_dNeedsFactor = 0.2;
        private static double m_dRightFactor = 0.1;
        private static double m_dNoDateFactor = 0.16;
        private static double m_dLandStatusFactor = 0.13;
        private static double m_dAroundFactor = 0.08;
        private static double m_dLevelFactor = 0.08;

        private int m_nPeopleRequest = 0;
        private int m_nNeeds = 0;
        private int m_nRight = 0;
        private int m_nNoDate = 0;
        private int m_nLandStatus = 0;
        private int m_nAround = 0;
        private int m_nLevel = 0;

        // 주민의 요구도 가중치
        public static double PeopleRequestFactor
        {
            get { return m_dPeopleRequestFactor; }
            set { m_dPeopleRequestFactor = value; }
        }

        // 사업의 필수성 가중치
        public static double NeedsFactor
        {
            get { return m_dNeedsFactor; }
            set { m_dNeedsFactor = value; }
        }

        // 정책의 부합성 가중치
        public static double RightFactor
        {
            get { return m_dRightFactor; }
            set { m_dRightFactor = value; }
        }

        // 미집행기간 가중치
        public static double NoDateFactor
        {
            get { return m_dNoDateFactor; }
            set { m_dNoDateFactor = value; }
        }

        // 토지현황 가중치
        public static double LandStatusFactor
        {
            get { return m_dLandStatusFactor; }
            set { m_dLandStatusFactor = value; }
        }

        // 주변여건 가중치
        public static double AroundFactor
        {
            get { return m_dAroundFactor; }
            set { m_dAroundFactor = value; }
        }

        // 시설의 등급 가중치
        public static double LevelFactor
        {
            get { return m_dLevelFactor; }
            set { m_dLevelFactor = value; }
        }

        // 주민의 요구도
        public int PeopleRequest
        {
            get { return m_nPeopleRequest; }
            set { m_nPeopleRequest = value; }
        }

        // 사업의 필수성
        public int Needs
        {
            get { return m_nNeeds; }
            set { m_nNeeds = value; }
        }

        // 정책의 부합성
        public int Right
        {
            get { return m_nRight; }
            set { m_nRight = value; }
        }

        // 미집행기간
        public int NoDate
        {
            get { return m_nNoDate; }
            set { m_nNoDate = value; }
        }

        // 토지현황
        public int LandStatus
        {
            get { return m_nLandStatus; }
            set { m_nLandStatus = value; }
        }

        // 주변여건
        public int Around
        {
            get { return m_nAround; }
            set { m_nAround = value; }
        }

        // 시설의 등급
        public int Level
        {
            get { return m_nLevel; }
            set { m_nLevel = value; }
        }
        
        // 시급성
        public double Importance
        {
            get { return (double)(m_dAroundFactor * m_nAround + m_dLandStatusFactor * m_nLandStatus + m_dLevelFactor * m_nLevel + m_dNeedsFactor * m_nNeeds + m_dNoDateFactor * m_nNoDate + m_dPeopleRequestFactor * m_nPeopleRequest + m_dRightFactor * m_nRight); }
        }

        public ImportanceData Clone()
        {
            ImportanceData data = new ImportanceData();

            data.m_nPeopleRequest = this.m_nPeopleRequest;
            data.m_nNeeds = this.m_nNeeds;
            data.m_nRight = this.m_nRight;
            data.m_nNoDate = this.m_nNoDate;
            data.m_nLandStatus = this.m_nLandStatus;
            data.m_nAround = this.m_nAround;
            data.m_nLevel = this.m_nLevel;

            return data;
        }

        public void CopyFrom(ImportanceData data)
        {
            this.m_nPeopleRequest = data.m_nPeopleRequest;
            this.m_nNeeds = data.m_nNeeds;
            this.m_nRight = data.m_nRight;
            this.m_nNoDate = data.m_nNoDate;
            this.m_nLandStatus = data.m_nLandStatus;
            this.m_nAround = data.m_nAround;
            this.m_nLevel = data.m_nLevel;
        }
    }

    public class SchedulePropertySector
    {
        private EditBoxHatch m_hatch = null;
        private DXFViewer.Shape m_shape = null;

        public EditBoxHatch Hatch
        {
            get { return m_hatch; }
            set { m_hatch = value; }
        }

        public DXFViewer.Shape Shape
        {
            get { return m_shape; }
            set { m_shape = value; }
        }

        public SchedulePropertySector()
        {
        }

        public SchedulePropertySector(EditBoxHatch hatch, DXFViewer.Shape shape)
        {
            m_hatch = hatch;
            m_shape = shape;
        }

        public SchedulePropertySector Clone()
        {
            SchedulePropertySector sector = new SchedulePropertySector();

            sector.m_hatch = this.m_hatch;
            sector.m_shape = this.m_shape;

            return sector;
        }
    }

    public class SchedulePropertySector_4_Read : SchedulePropertySector
    {
        private int m_nLayerIndex = -1;
        private int m_nShapeIndex = -1;

        public int LayerIndex
        {
            get { return m_nLayerIndex; }
            set { m_nLayerIndex = value; }
        }

        public int ShapeIndex
        {
            get { return m_nShapeIndex; }
            set { m_nShapeIndex = value; }
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

    public class ComboBoxText
    {
        public enum EndEditType { NONE = 0, CANCEL, ENTER };

        //private string m_strText = "";
        private EndEditType m_endType = EndEditType.NONE;
        private System.Windows.Forms.ComboBox m_comboBox = null;

        /*public string Text
        {
            get { return m_strText; }
            set { m_strText = value; }
        }*/

        public EndEditType EndType
        {
            get { return m_endType; }
            set { m_endType = value; }
        }

        public System.Windows.Forms.ComboBox Control
        {
            get { return m_comboBox; }
            set { m_comboBox = value; }
        }
    }

    public class ComboBoxText<T> : ComboBoxText
    {
        private VariousData<T> m_data = null;

        public VariousData<T> Data
        {
            get { return m_data; }
            set { m_data = value; }
        }
    }

    public class Options
    {
        private static Options m_instance = null;
        public static Options Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new Options();

                return m_instance;
            }
        }

        //백업 파일 개수(기본3개)
        private int nBackupCount = 3;
        public int BackupCount
        {
            get { return nBackupCount; }
            set { nBackupCount = value; }
        }

        private bool m_visibleBackgroundImage = true;
        public bool VisibleBackgroundImage
        {
            get { return m_visibleBackgroundImage; }
            set { m_visibleBackgroundImage = value; }
        }

        // 도로 선택시 해당객체 ZoomIn 여부
        private bool m_zoomOnSelectStreet = true;
        public bool ZoomOnSelectStreet
        {
            get { return m_zoomOnSelectStreet; }
            set { m_zoomOnSelectStreet = value; }
        }

        // 개설비율 옵션
        // true : 개설면적 / 총면적
        // false : 개설길이 / 총길이
        private bool m_completeRatioByArea = true;
        public bool CompleteRatioByArea
        {
            get { return m_completeRatioByArea; }
            set { m_completeRatioByArea = value; }
        }

        private System.Drawing.Color m_backgroundColor = System.Drawing.Color.Black;
        public System.Drawing.Color BackColor
        {
            get { return m_backgroundColor; }
            set { m_backgroundColor = value; }
        }

		private bool m_bPrintHeader = false;
		public bool PrintHeader
		{
			get { return m_bPrintHeader; }
			set { m_bPrintHeader = value; }
		}
		private bool m_bPrintDate = false;
		public bool PrintDate
		{
			get { return m_bPrintDate; }
			set { m_bPrintDate = value; }
		}
		private string m_szPrintHeaderText = "";
		public string PrintHeaderText
		{
			get { return m_szPrintHeaderText; }
			set { m_szPrintHeaderText = value; }
		}

    }

    public class DXFDatas
    {
        private List<LayerData> m_listLayerDatas = null;
        // 노선이름별 구간 Shape들
        // Value : Shape의 ID List
        private Dictionary<string, List<int>> m_dicStreetShapes = null;
        private Dictionary<string, StreetCenterLine> m_dicStreetCenterLines = null;

        public List<LayerData> LayerDatas
        {
            get { return m_listLayerDatas; }
        }

        // 노선이름별 구간 Shape들
        // Value : Shape의 ID List
        public Dictionary<string, List<int>> StreetShapes
        {
            get { return m_dicStreetShapes; }
        }

        public Dictionary<string, StreetCenterLine> StreetCenterLines
        {
            get { return m_dicStreetCenterLines; }
        }

        public DXFDatas()
        {
            m_listLayerDatas = new List<LayerData>();
            m_dicStreetShapes = new Dictionary<string, List<int>>();
            m_dicStreetCenterLines = new Dictionary<string, StreetCenterLine>();
        }

        public DXFDatas(List<LayerData> layerDatas, Dictionary<string, List<int>> streetShapes, Dictionary<string, StreetCenterLine> dicStreetCenterLines)
        {
            m_listLayerDatas = layerDatas;
            m_dicStreetShapes = streetShapes;
            m_dicStreetCenterLines = dicStreetCenterLines;
        }
    }

    public class StreetCenterLine
    {
        private string m_strStreetName = "";
        // Key : Target(Boundary) Shape의 ID
        // Value : 중심선 PolyLine
        private Dictionary<int, PolyLineEx> m_dicPolyLines = new Dictionary<int, PolyLineEx>();

        public string StreetName
        {
            get { return m_strStreetName; }
            set { m_strStreetName = value; }
        }

        public Dictionary<int, PolyLineEx> PolyLines
        {
            get { return m_dicPolyLines; }
            set { m_dicPolyLines = value; }
        }
    }

    public class StreetCenterLine2
    {
        private string m_strStreetName = "";
        // Key : Target(Boundary) Shape
        // Value : 중심선 PolyLine
        private Dictionary<DXFViewer.Shape, PolyLineEx> m_dicPolyLines = new Dictionary<DXFViewer.Shape, PolyLineEx>();
        private double m_dTotalLength = -1.0;

        public string StreetName
        {
            get { return m_strStreetName; }
            set { m_strStreetName = value; }
        }

        public Dictionary<DXFViewer.Shape, PolyLineEx> PolyLines
        {
            get { return m_dicPolyLines; }
            set { m_dicPolyLines = value; }
        }

        public double TotalLength
        {
            get
            {
                if (m_dTotalLength < 0.0)
                    CalcLength();

                return m_dTotalLength;
            }
        }

        private void CalcLength()
        {
            double dLen = 0.0;

            foreach (KeyValuePair<DXFViewer.Shape, PolyLineEx> pair in m_dicPolyLines)
            {
                dLen += pair.Value.LineLength;
            }

            m_dTotalLength = dLen;
        }
    }

    public class PolyLineEx : DXFViewer.PolyLine
    {
        private double m_dLineLength = 0.0;

        public double LineLength
        {
            get
            {
                if (m_dLineLength == 0.0)
                    CalcLength();

                return m_dLineLength;
            }
        }

        public UnE.Geometry.Polygon Polygon
        {
            get { return GetPolygon(); }
        }

        public new void SetVertex(System.Collections.ArrayList arrVertices)
        {
            base.SetVertex(arrVertices);
        }

        private void CalcLength()
        {
            int nVertexCount = GetVertexSize();
            m_dLineLength = 0.0;

            for (int i=1;i<nVertexCount;i++)
            {
                System.Drawing.PointF pt1 = GetVertex(i - 1);
                System.Drawing.PointF pt2 = GetVertex(i);
                m_dLineLength += System.Math.Sqrt((pt1.X - pt2.X) * (pt1.X - pt2.X) + (pt1.Y - pt2.Y) * (pt1.Y - pt2.Y));
            }
        }

        // dirFromBegin이 true이면 시작점으로부터 dLen만큼 떨어진 Vertex를 구한다.
        //                false이면 끝점으로부터 dLen만큼 떨어진 Vertex를 구한다.
        // dLen이 0보다 작거나 PolyLine 길이를 초과하면 null을 리턴한다.
        // nPrevIndex : 리턴값을 리턴하기 직전의 Vertex Index
        public UnE.Geometry.Vertex2D GetVertex(double dLen, bool dirFromBegin, out int nPrevIndex)
        {
            int nVertexCount = GetVertexSize();

            if (dirFromBegin)
                nPrevIndex = 0;
            else
                nPrevIndex = nVertexCount - 1;

            if (dLen == 0.0)
                return null;

            if (nVertexCount < 2)
                return null;

            double dSum = 0.0;
            UnE.Geometry.Vertex2D vPrev = null;

            if (dirFromBegin)
            {
                System.Drawing.PointF ptPrev = GetVertex(0);
                vPrev = new UnE.Geometry.Vertex2D(ptPrev.X, ptPrev.Y);

                for (int i=1;i<nVertexCount;i++)
                {
                    System.Drawing.PointF pt = GetVertex(i);
                    UnE.Geometry.Vertex2D vCurrent = new UnE.Geometry.Vertex2D(pt.X, pt.Y);

                    double len = vCurrent.GetDistance(vPrev);

                    if (dSum + len == dLen)
                        return vCurrent;
                    else if (dSum + len > dLen)
                        return UnE.Geometry.Math.GetLinearVertex(vPrev, vCurrent, dLen - dSum);
                    
                    dSum += len;
                    vPrev = vCurrent;
                    nPrevIndex = i;
                }
            }
            else
            {
                System.Drawing.PointF ptPrev = GetVertex(nVertexCount - 1);
                vPrev = new UnE.Geometry.Vertex2D(ptPrev.X, ptPrev.Y);

                for (int i = nVertexCount - 2; i >= 0; i--)
                {
                    System.Drawing.PointF pt = GetVertex(i);
                    UnE.Geometry.Vertex2D vCurrent = new UnE.Geometry.Vertex2D(pt.X, pt.Y);

                    double len = vCurrent.GetDistance(vPrev);

                    if (dSum + len == dLen)
                        return vCurrent;
                    else if (dSum + len > dLen)
                        return UnE.Geometry.Math.GetLinearVertex(vPrev, vCurrent, dLen - dSum);

                    dSum += len;
                    vPrev = vCurrent;
                    nPrevIndex = i;
                }
            }

            return null;
        }
    }

	public class ProcessResult : UndoRedoData
    {
		public ProcessResult() : base()
		{ }

        private ProcessSchedule m_schedule = null;

		[XmlArray("ResultPropertyArray")]
		[XmlArrayItem("ResultProperty")]
        public List<ResultProperty> m_properties = new List<ResultProperty>();
        private string m_strDescription = "";

		[XmlIgnore]
        public ProcessSchedule ProcessSchedule
        {
            get { return m_schedule; }
            
			set
			{
				m_schedule = value; 
			}
        }

		// Object Serialization에 사용되는 변수, ProcessSchedule의 HashCode
		private string m_szScheduleHash = "";
		public string ScheduleHash
		{
			get { return m_szScheduleHash; }
			set { m_szScheduleHash = value; }
		}


        public IList<ResultProperty> ResultProperties
        {
            get { return m_properties; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set 
			{
				
				if (m_strDescription != value)
				{
					OnPropertyChanging("ResultDescription");
				}

				m_strDescription = value;
				OnPropertyChanged("ResultDescription");
			}
        }

        // 길이는 int 값이지만 비율계산을 위한 값이므로 double로 리턴한다.
        public double TotalLength
        {
            get
            {
                double dTotalLength = 0.0;

                foreach (ResultProperty prop in m_properties)
                {
                    if (prop.ScheduleProperty == null || prop.ScheduleProperty.Length == null)
                        continue;

					double dPropLength = prop.TotalLength;
                    dTotalLength += dPropLength;
                }

                return dTotalLength;
            }
        }

        public double TotalArea
        {
            get
            {
                double dTotalArea = 0.0;

                foreach (ResultProperty prop in m_properties)
                {
                    if (prop.ScheduleProperty == null || prop.ScheduleProperty.Area == null)
                        continue;

                    double dPropArea = prop.TotalArea;
                    dTotalArea += dPropArea;
                }

                return dTotalArea;
            }
        }

        public double Total
        {
            get
            {
                if (Options.Instance.CompleteRatioByArea)
                    return TotalArea;
                //else
                return TotalLength;
            }
        }



		public ProcessResult(ProcessSchedule schedule, List<ResultProperty> properties, string strETC)
			: base()
        {
            m_schedule = schedule;
            m_strDescription = strETC;

            if (properties != null)
            {
                m_properties.AddRange(properties);
            }
        }
    }

    public class ResultProperty
    {


        private ScheduleProperty m_scheduleProperty = null;

		[XmlArray("ResultPropertyDataArray")]
		[XmlArrayItem("ResultPropertyData")]
        public List<ResultPropertyData> m_resultProperties = new List<ResultPropertyData>();

		[XmlIgnore]
        public ScheduleProperty ScheduleProperty
        {
            get { return m_scheduleProperty; }
            set 
			{
				m_scheduleProperty = value; 

				if( value != null)
				{
					m_szSchedulePropertyHash = value.GetHashCode().ToString();
				}
			
			}
        }

		[XmlIgnore]
        public List<ResultPropertyData> PropertyDatas
        {
            get { return m_resultProperties; }
        }

		private string m_szSchedulePropertyHash = "";
		public string SchedulePropertyHash
		{
			get { return m_szSchedulePropertyHash; }
			set { m_szSchedulePropertyHash = value; }
		}


        public void Sort()
        {
            m_resultProperties.Sort();
        }

        public ResultPropertyData FindPropertyData(VariousData<DateTime> dtBegin, VariousData<DateTime> dtEnd)
        {
            foreach (ResultPropertyData data in m_resultProperties)
            {
                if (data.BeginTime == dtBegin && data.EndTime == dtEnd)
                    return data;
            }

            return null;
        }

		public double TotalLength
		{
			get
			{
				double dPropLength = 0.0;

				foreach (ResultPropertyData data in this.PropertyDatas)
				{
					if (data.UnitLength != null)
						dPropLength += data.UnitLength.Data;
				}

				// 집행실적은 집행계획보다 클수 없다.
				if (this.ScheduleProperty.Length.Data < dPropLength)
					dPropLength = this.ScheduleProperty.Length.Data;

				return dPropLength;
			}
		}

		public double TotalArea
		{
			get
			{
				double dPropArea = 0.0;

				foreach (ResultPropertyData data in this.PropertyDatas)
				{
					if (data.UnitArea != null)
						dPropArea += data.UnitArea.Data;
				}

				// 집행실적은 집행계획보다 클수 없다.
				if (this.ScheduleProperty.Area.Data < dPropArea)
					dPropArea = this.ScheduleProperty.Area.Data;


				return dPropArea;
			}
		}

		public double Total
		{
			get
			{
				if (Options.Instance.CompleteRatioByArea)
					return TotalArea;
				//else
				return TotalLength;
			}
		}

        public long TotalCost
        {
            get
            {
                long nTotalCost = 0;

                foreach (ResultPropertyData data in this.PropertyDatas)
                {
                    if (data.ProjectCost != null)
                        nTotalCost += data.ProjectCost.Data;
                }

                return nTotalCost;
            }
        }

        public void CopyFrom(ResultProperty prop)
        {
            this.m_scheduleProperty = prop.ScheduleProperty;
            this.m_resultProperties.Clear();
            this.m_resultProperties.AddRange(prop.m_resultProperties);
        }
    }

    public class ResultPropertyData : IComparable
    {
        private string m_strProjectName = "";
        private VariousData<long> m_nProjectCost = null;
        private VariousData<DateTime> m_dtBegin = null;
        private VariousData<DateTime> m_dtEnd = null;
        private VariousData<int> m_nAccumulLength = null;
        private VariousData<int> m_nUnitLength = null;
        private VariousData<bool> m_isDirectionFromBegin = null;
        private VariousData<int> m_nAccumulArea = null;
        private VariousData<int> m_nUnitArea = null;

        // 사업명
        public string ProjectName
        {
            get { return m_strProjectName; }
            set { m_strProjectName = value; }
        }

        // 공사시작시간
        public VariousData<DateTime> BeginTime
        {
            get { return m_dtBegin; }
            set { m_dtBegin = value; }
        }

        // 공사종료시간
        public VariousData<DateTime> EndTime
        {
            get { return m_dtEnd; }
            set { m_dtEnd = value; }
        }

        // 사업비
        public VariousData<long> ProjectCost
        {
            get { return m_nProjectCost; }
            set { m_nProjectCost = value; }
        }

        // 사업량(누적길이, m)
        public VariousData<int> AccumulLength
        {
            get { return m_nAccumulLength; }
            set { m_nAccumulLength = value; }
        }

        // 사업량(단위공사 길이, m)
        public VariousData<int> UnitLength
        {
            get { return m_nUnitLength; }
            set { m_nUnitLength = value; }
        }

        // 사업량(길이)가 시작지점으로부터인가?
        public VariousData<bool> DirectionFromBegin
        {
            get { return m_isDirectionFromBegin; }
            set { m_isDirectionFromBegin = value; }
        }

        // 사업량(누적면적, m²)
        public VariousData<int> AccumulArea
        {
            get { return m_nAccumulArea; }
            set { m_nAccumulArea = value; }
        }

        // 사업량(단위공사 면적, m²)
        public VariousData<int> UnitArea
        {
            get { return m_nUnitArea; }
            set { m_nUnitArea = value; }
        }

        public int CompareTo(object obj)
        {
            ResultPropertyData data = (ResultPropertyData)obj;

            if (this.m_dtBegin == null && data.m_dtBegin == null)
            {
                if (this.m_dtEnd == null && data.m_dtEnd == null)
                    return 0;
                else if (this.m_dtEnd == null)
                    return -1;
                else if (data.m_dtEnd == null)
                    return 1;
                else if (this.m_dtEnd.Data > data.m_dtEnd.Data)
                    return 1;
                else if (this.m_dtEnd.Data < data.m_dtEnd.Data)
                    return -1;
                else
                    return 0;
            }
            else if (this.m_dtBegin == null)
                return -1;
            else if (data.m_dtBegin == null)
                return 1;

            if (this.m_dtBegin.Data > data.m_dtBegin.Data)
                return 1;
            else if (this.m_dtBegin.Data < data.m_dtBegin.Data)
                return -1;
            else
            {
                if (this.m_dtEnd == null)
                    return -1;
                else if (data.m_dtEnd == null)
                    return 1;

                if (this.m_dtEnd.Data > data.m_dtEnd.Data)
                    return 1;
                else if (this.m_dtEnd.Data < data.m_dtEnd.Data)
                    return -1;
            }

            return 0;
        }
    }

    public class MenuButton
    {
        private UnE.GUI.RibbonButton m_btn = null;
        private System.Windows.Forms.ToolStripMenuItem m_menu = null;

        public UnE.GUI.RibbonButton Button
        {
            get { return m_btn; }
            set { m_btn = value; }
        }

        public System.Windows.Forms.ToolStripMenuItem Menu
        {
            get { return m_menu; }
            set { m_menu = value; }
        }

        public bool Enabled
        {
            get { return m_btn.Enabled; }
            set { m_btn.Enabled = m_menu.Enabled = value; }
        }

        public bool Checked
        {
            get { return m_btn.IsChecked; }
            set
            {
                m_btn.IsChecked = m_menu.Checked = value;
                m_btn.Refresh();
            }
        }

        public bool Visible
        {
            get { return m_btn.Visible; }
            set { m_btn.Visible = m_menu.Visible = value; }
        }

        public System.Drawing.Point Location
        {
            get { return m_btn.Location; }
            set { m_btn.Location = value; }
        }

        public System.Drawing.Size Size
        {
            get { return m_btn.Size; }
            set { m_btn.Size = value; }
        }

        public MenuButton()
        {
        }

        public MenuButton(UnE.GUI.RibbonButton btn, System.Windows.Forms.ToolStripMenuItem menu, EventHandler clickHandler)
        {
            m_btn = btn;
            m_menu = menu;

            m_menu.Click += new System.EventHandler(clickHandler);
        }

        public override bool Equals(object obj)
        {
            return obj == m_btn || obj == m_menu;
        }

        public void Refresh()
        {
            m_btn.Refresh();
        }
    }

    public class AddrString : IComparable
    {
        private string m_str1 = null;
        private string m_str2 = null;

        public AddrString()
        {
        }

        public AddrString(string str)
        {
            Data = str;
        }

        public string Data
        {
            get { return m_str1; }
            set
            {
                m_str1 = value;
                int nLen = m_str1.Length;

                if (m_str1 == null)
                    m_str2 = null;
                else if (nLen == 0)
                    m_str2 = "";
                else
                {
                    char ch = m_str1.ElementAt(0);

                    if (ch >= '0' && ch <= '9')
                    {
                        m_str2 = "";

                        for (int i = 0; i < 4 - nLen; i++)
                        {
                            m_str2 += "0";
                        }

                        m_str2 += m_str1;
                    }
                    else
                    {
                        m_str2 = null;

                        for (int i = 0; i < nLen;i++ )
                        {
                            ch = m_str1.ElementAt(i);

                            if (i < '0' || i > '9')
                            {
                                string strHeader = m_str1.Substring(0, i + 1);
                                string strTail = "";

                                for (int j=0;j<5-(nLen - (i+1));j++)
                                {
                                    strTail += "0";
                                }

                                strTail += m_str1.Substring(i + 1);
                                m_str2 = strHeader + strTail;
                                break;
                            }
                        }

                        if (m_str2 == null)
                            m_str2 = m_str1;
                    }
                }
            }
        }

        public override string ToString()
        {
            return m_str1;
        }

        public int CompareTo(object obj)
        {
            AddrString addr = (AddrString)obj;
            return m_str2.CompareTo(addr.m_str2);
        }

        public static bool Contains(List<AddrString> arr, string str)
        {
            foreach (AddrString addrString in arr)
            {
                if (addrString.Data == str)
                    return true;
            }

            return false;
        }
    }

    public class WindowMessage
    {
        public static int WM_KEYDOWN { get { return 0x100; } }
        public static int WM_KEYUP { get { return 0x101; } }
        public static int WM_CHAR { get { return 0x102; } }
        public static int WM_SYSKEYDOWN { get { return 0x104; } }
        public static int WM_SYSKEYUP { get { return 0x105; } }
    }
}
