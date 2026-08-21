using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandAddressReader
{
    public class LandAddressData2 : LandAddressData
    {
        private DXFViewer.PolyLine m_boundary = null;

        public DXFViewer.PolyLine Boundary
        {
            get { return m_boundary; }
            set { m_boundary = value; }
        }

        public LandAddressData2(string strLandAddr)
        {
            string[] arrNames = strLandAddr.Split(' ');
            int nCount = arrNames.Count();

            if (nCount <= 2)
            {
                TownName = strLandAddr.Trim();
            }
            else
            {
                string strDong = arrNames[0].Trim();
                string strRi = arrNames[1].Trim();
                TownName = (strDong + " " + strRi);

                string strTail = "";

                for (int i = 2; i < nCount; i++)
                {
                    strTail += arrNames[i];
                }

                string[] arrAddrs = strTail.Split('-');
                int nAddrCount = arrAddrs.Count();

                if (nAddrCount == 1)
                {
                    MajorAddr = strTail;
                }
                else
                {
                    MajorAddr = arrAddrs[0].Trim();
                    int nIndex = strTail.IndexOf('-');
                    MinorAddr = strTail.Substring(nIndex + 1).Trim();
                }
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
            if (MinorAddr != null && MinorAddr != "")
                return TownName + " " + MajorAddr + "-" + MinorAddr;
            else
                return TownName + " " + MajorAddr;
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

            data.m_strTownName = this.m_strTownName;
            data.m_strDong = this.m_strDong;
            data.m_strRi = this.m_strRi;
            data.m_strMajorAddr = this.m_strMajorAddr;
            data.m_strMinorAddr = this.m_strMinorAddr;
            data.m_dTotalArea = this.m_dTotalArea;
            data.m_dStreetArea = this.m_dStreetArea;
            data.m_strOwnerType = this.m_strOwnerType;
            data.m_nPublicEstimation = this.m_nPublicEstimation;

            return data;
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
}
