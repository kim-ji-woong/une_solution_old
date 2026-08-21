using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryReader
{
    public class Library
    {
        public enum LibraryType { 중앙 = 0, 대표, 거점, 분관, 작은, UNKNOWN };
        public enum OwnerType { 지자체 = 0, 교육청, 사립, 건립중, UNKNOWN };

        private int m_nID = -1;
        private string m_strName = "";
        private string m_strLocation = "";
        private string m_strGubun = "";
        private int m_nYear = -1;
        private string m_strOwner = "";
        private LibraryType m_type = LibraryType.UNKNOWN;
        private OwnerType m_ownerType = OwnerType.UNKNOWN;
        private string m_strHomepage = "";
        private string m_strFax = "";
        private string m_strPhoneNumber = "";
        private string m_strAddress = "";
        private int m_nArea = 0;    // m²
        private int m_nGrade = 0;
        private string m_strAddr1 = "";
        private string m_strAddr2 = "";
        private string m_strAddr3 = "";
        private string m_strAddr4 = "";
        private string m_strCoord = "";
        // 도서자료(권)
        private string m_strUserCount = "";
        private string m_strUseCount = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public string Location
        {
            get { return m_strLocation; }
            set { m_strLocation = value; }
        }

        public string Gubun
        {
            get { return m_strGubun; }
            set { m_strGubun = value; }
        }

        public int Year
        {
            get { return m_nYear; }
            set { m_nYear = value; }
        }

        public string Owner
        {
            get { return m_strOwner; }
            set { m_strOwner = value; }
        }

        public LibraryType GubunType
        {
            get { return m_type; }
            set { m_type = value; }
        }

        public OwnerType OwnType
        {
            get { return m_ownerType; }
            set { m_ownerType = value; }
        }

        public string Homepage
        {
            get { return m_strHomepage; }
            set { m_strHomepage = value; }
        }

        public string PhoneNumber
        {
            get { return m_strPhoneNumber; }
            set { m_strPhoneNumber = value; }
        }

        public string FaxNumber
        {
            get { return m_strFax; }
            set { m_strFax = value; }
        }

        public string Address
        {
            get { return m_strAddress; }
            set { m_strAddress = value; }
        }

        // m²
        public int Area
        {
            get { return m_nArea; }
            set { m_nArea = value; }
        }

        public int Grade
        {
            get { return m_nGrade; }
            set { m_nGrade = value; }
        }

        public string Addr1
        {
            get { return m_strAddr1; }
            set { m_strAddr1 = value; }
        }

        public string Addr2
        {
            get { return m_strAddr2; }
            set { m_strAddr2 = value; }
        }

        public string Addr3
        {
            get { return m_strAddr3; }
            set { m_strAddr3 = value; }
        }

        public string Addr4
        {
            get { return m_strAddr4; }
            set { m_strAddr4 = value; }
        }

        public string Coord
        {
            get { return m_strCoord; }
            set { m_strCoord = value; }
        }

        // 도서자료(권)
        public string UserCount
        {
            get { return m_strUserCount; }
            set { m_strUserCount = value; }
        }

        public string UseCount
        {
            get { return m_strUseCount; }
            set { m_strUseCount = value; }
        }
    }
}
