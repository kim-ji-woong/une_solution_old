using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DXFUtility
{
    public class FileName : IComparable
    {
        private string m_strFilePath = "";
        private float m_fFloorIndex = 0.0f;

        public FileName(string strFilePath)
        {
            FilePath = strFilePath;
        }

        public int CompareTo(object obj)
        {
            FileName file = (FileName)obj;

            if (this.m_fFloorIndex > file.m_fFloorIndex)
                return 1;
            else if (this.m_fFloorIndex < file.m_fFloorIndex)
                return -1;
            //else
            return 0;
        }

        public float FloorIndex
        {
            get { return m_fFloorIndex; }
        }

        public string FilePath
        {
            get { return m_strFilePath; }
            set
            {
                m_strFilePath = value;

                int nPathLen = m_strFilePath.Length;
                int nDotIndex = -1, nIndex = -1;

                for (int i = nPathLen - 1; i >= 0; i--)
                {
                    char ch = m_strFilePath.ElementAt(i);

                    if (ch == '.')
                    {
                        if (nDotIndex < 0)
                        {
                            nDotIndex = i;
                        }
                    }
                    else if (ch == '\\')
                        break;
                    else if (ch == '_')
                    {
                        nIndex = i + 1;
                        break;
                    }
                }

                if (nIndex >= 0 && nDotIndex >= 0)
                {
                    string strNum = m_strFilePath.Substring(nIndex, nDotIndex - nIndex);
                    bool underGround = strNum.StartsWith("B");

                    if (underGround)
                        strNum = strNum.Substring(1);

                    float fAddFloor = 0.0f;

                    if (strNum.EndsWith("M"))
                    {
                        strNum = strNum.Substring(0, strNum.Length - 1);
                        // "M"은 무시한다.
                        //fAddFloor = 0.5f;
                    }

                    try
                    {
                        m_fFloorIndex = float.Parse(strNum) + fAddFloor;

                        if (underGround)
                            m_fFloorIndex = -m_fFloorIndex;
                        else
                            m_fFloorIndex = m_fFloorIndex - 1;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
    }

    public class Building
    {
        private int m_nID = -1;
        private string m_strBuildingName = "";
        // 0은 1층, 1은 2층, 지하는 음수
        private int m_nMinFloorIndex = 0;
        private int m_nMaxFloorIndex = 0;

        private string m_strBuildingID = "";
        private string m_strBuildingCode = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string BuildingName
        {
            get { return m_strBuildingName; }
            set { m_strBuildingName = value; }
        }

        public int MinFloorIndex
        {
            get { return m_nMinFloorIndex; }
            set { m_nMinFloorIndex = value; }
        }

        public int MaxFloorIndex
        {
            get { return m_nMaxFloorIndex; }
            set { m_nMaxFloorIndex = value; }
        }

        public string BuildingID
        {
            get { return m_strBuildingID; }
            set { m_strBuildingID = value; }
        }

        public string BuildingCode
        {
            get { return m_strBuildingCode; }
            set { m_strBuildingCode = value; }
        }
    }

    public class Zone
    {
        private int m_nID = -1;
        private string m_strZoneName = "";
        private int m_nBuildingID = -1;
        private int m_nFloorIndex = -1;
        private string m_strBoundaryVertices = "";
        private UnE.Geometry.Vertex2D m_vTextCenter = new UnE.Geometry.Vertex2D();

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string ZoneName
        {
            get { return m_strZoneName; }
            set { m_strZoneName = value; }
        }

        public int BuildingID
        {
            get { return m_nBuildingID; }
            set { m_nBuildingID = value; }
        }

        public int FloorIndex
        {
            get { return m_nFloorIndex; }
            set { m_nFloorIndex = value; }
        }

        public string BoundaryVertices
        {
            get { return m_strBoundaryVertices; }
            set { m_strBoundaryVertices = value; }
        }

        public UnE.Geometry.Vertex2D TextCenter
        {
            get { return m_vTextCenter; }
        }
    }

    public class FireEquipmentDBData : IComparable
    {
        private string m_strEquipID = "''";
        private string m_strRFIDTag = "NULL";
        private string m_strRFIDTagID = "NULL";
        private int m_nEquipType = -1;
        private string m_strEquipSubType = "NULL";
        private int m_nZoneID = -1;
        private string m_strCreateDate = "NULL";
        private string m_strDuration = "NULL";
        private string m_strDescription = "NULL";

        int IComparable.CompareTo(object obj)
        {
            FireEquipmentDBData data = (FireEquipmentDBData)obj;
            return string.Compare(m_strEquipID, data.m_strEquipID);
        }

        public string EquipID
        {
            get { return m_strEquipID; }
            set { m_strEquipID = value; }
        }

        public string RFIDTag
        {
            get { return m_strRFIDTag; }
            set { m_strRFIDTag = value; }
        }

        public string RFIDTagID
        {
            get { return m_strRFIDTagID; }
            set { m_strRFIDTagID = value; }
        }

        public int EquipType
        {
            get { return m_nEquipType; }
            set { m_nEquipType = value; }
        }

        public string EquipSubType
        {
            get { return m_strEquipSubType; }
            set { m_strEquipSubType = value; }
        }

        public int ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        public string CreateDate
        {
            get { return m_strCreateDate; }
            set { m_strCreateDate = value; }
        }

        public string Duration
        {
            get { return m_strDuration; }
            set { m_strDuration = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }
    }
}
