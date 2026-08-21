using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace HSMS
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

        public ModelManager()
        {            
            m_DBConnection = new DBConn("HSMS");
            LoadModel();
        }
        
        

        public void LoadModel()
        { 
            if (m_DBConnection == null)
                return;

            int nSiteID = FormMain.Instance.SiteID;

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
