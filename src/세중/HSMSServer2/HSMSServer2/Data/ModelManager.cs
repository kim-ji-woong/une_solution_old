using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using HSMS;

namespace HSMSServer2
{
    public class ModelManager
    {
        private static ModelManager m_instance = null;
		public static ModelManager Instance
		{
			get 
			{
				if (m_instance == null)
				{
					m_instance = new ModelManager();					
				}
				return m_instance; 
			}
		}

        private DBConn m_DBConnection = null;
        public DBConn DBManager
        {
            get { return m_DBConnection; }
        }

        private bool m_bUseDB = false;
        public bool UseDB
        {
            get { return m_bUseDB; }
        }

        private float m_fWidth = 300.0f;
        public float Width
        {
            get { return m_fWidth; }
            set { m_fWidth = value; }
        }

        private float m_fHeight = 180.0f;
        public float Height
        {
            get { return m_fHeight; }
            set { m_fHeight = value; }
        }

        private float m_fElevation = 10.0f;
        public float Elevation
        {
            get { return m_fElevation; }
            set { m_fElevation = value; }
        }

        private float m_MinX = 0.0f;
        public float MinX
        {
            get { return m_MinX; }
        }

        private float m_MinY = 0.0f;
        public float MinY
        {
            get { return m_MinY; }
        }

        private float m_MinZ = 0.0f;
        public float MinZ
        {
            get { return m_MinZ; }
            set { m_MinZ = value; }
        }

        private float m_MaxX = 300.0f;
        public float MaxX
        {
            get { return m_MaxX; }
        }

        private float m_MaxY = 180.0f;
        public float MaxY
        {
            get { return m_MaxY; }
            set { m_MaxY = value; }
        }

        private float m_MaxZ = 10.0f;
        public float MaxZ
        {
            get { return m_MaxZ; }
            set { m_MaxZ = value; }
        }

        private float m_OriginX = 0.0f;
        public float OriginX
        {
            get { return m_OriginX; }
        }
        private float m_OriginY = 0.0f;
        public float OriginY
        {
            get { return m_OriginY; }
        }
        private float m_OriginZ = 0.0f;
        public float OriginZ
        {
            get { return m_OriginZ; }
        }

        
        public ModelManager()
        {            
            m_DBConnection = new DBConn("HSMS");
            LoadModel();
        }
               

        public void LoadModel()
        { 
            if (m_DBConnection == null)
                return;

            int nSiteID = NetworkServer.Instance.SiteID;

            SqlConnection connect = m_DBConnection.Connect();

            string szSQL = string.Format("SELECT PName, PValue, PValueType FROM Map WHERE SiteID = {0}", nSiteID);

            try
            {
                SqlDataReader rd = m_DBConnection.ExecuteReader(szSQL, connect);
                while (rd.Read())
                {                    
                    string szPName = rd[0].ToString().TrimEnd();
                    string szPValue = rd[1].ToString().TrimEnd();
                    string szPValueType = rd[2].ToString().TrimEnd();

                    if( szPName.ToLower() == "usedb")
                    {
                        if(!Boolean.TryParse(szPValue, out m_bUseDB))
                        {
                            m_bUseDB = false;
                        }
                    }

                    if (szPName.ToLower() == "mapwidth")
                    {
                        if (!float.TryParse(szPValue, out m_fWidth))
                        {
                            m_fWidth = 300.0f;
                        }
                    }

                    if (szPName.ToLower() == "mapheight")
                    {
                        if (!float.TryParse(szPValue, out m_fHeight))
                        {
                            m_fHeight = 300.0f;
                        }
                    }

                    if (szPName.ToLower() == "mapelevation")
                    {
                        if (!float.TryParse(szPValue, out m_fElevation))
                        {
                            m_fElevation = 10.0f;
                        }
                    }

                    if( szPName.ToLower() == "minx")
                    {
                        if(!float.TryParse(szPValue, out m_MinX))
                        {
                            m_MinX = 0.0f;
                        }
                    }

                    if (szPName.ToLower() == "miny")
                    {
                        if (!float.TryParse(szPValue, out m_MinY))
                        {
                            m_MinY = 0.0f;
                        }
                    }

                    if (szPName.ToLower() == "minz")
                    {
                        if (!float.TryParse(szPValue, out m_MinZ))
                        {
                            m_MinZ = 0.0f;
                        }
                    }

                    if (szPName.ToLower() == "maxx")
                    {
                        if (!float.TryParse(szPValue, out m_MaxX))
                        {
                            m_MaxX = 0.0f;
                        }
                    }

                    if (szPName.ToLower() == "maxy")
                    {
                        if (!float.TryParse(szPValue, out m_MaxY))
                        {
                            m_MaxY = 0.0f;
                        }
                    }

                    if (szPName.ToLower() == "maxz")
                    {
                        if (!float.TryParse(szPValue, out m_MaxZ))
                        {
                            m_MaxZ = 0.0f;
                        }
                    }

                    if (szPName.ToLower() == "originx")
                    {
                        if (!float.TryParse(szPValue, out m_OriginX))
                        {
                            m_OriginX = 0.0f;
                        }
                    }

                    if (szPName.ToLower() == "originy")
                    {
                        if (!float.TryParse(szPValue, out m_OriginY))
                        {
                            m_OriginY = 0.0f;
                        }
                    }

                    if (szPName.ToLower() == "originz")
                    {
                        if (!float.TryParse(szPValue, out m_OriginZ))
                        {
                            m_OriginZ = 0.0f;
                        }
                    }
                }
                rd.Close();
                connect.Close();
            }
            catch (Exception)
            {
            }
        }
    }        
}
