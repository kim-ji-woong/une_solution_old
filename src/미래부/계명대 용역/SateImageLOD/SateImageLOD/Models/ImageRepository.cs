using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Sql;
using System.Data.SqlClient;

namespace SateImageLOD.Models
{
    public static class ImageRepository
    {
        private static SqlConnection m_connection = null;
        private static Dictionary<int, SateImage> m_dicImages = new Dictionary<int, SateImage>();

        /*public ImageRepository(string strDbPath)
        {
            string strConnection = "Data Source=(LocalDB)\\v11.0;AttachDbFilename=" + strDbPath + ";Integrated Security=True;Connect Timeout=30";
            SqlConnection connection = new SqlConnection(strConnection);
            connection.Open();

            string strSQL = "Select * from Sensor";
            List<string> sensorDatas = new List<string>();

            SqlCommand cmd = new SqlCommand(strSQL, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (reader.IsDBNull(i))
                        sensorDatas.Add("null");
                    else
                        sensorDatas.Add(reader.GetValue(i).ToString());
                }
            }

            reader.Close();
            connection.Close();
        }*/
       
        public static int LastImageID
        {
            get { return GetLastImageID(); }
        }

        private static int GetLastImageID()
        {
            if (m_connection == null)
                return -1;

            try
            {
                string strSQL = "Select max(ID) from SateImage";
                SqlCommand cmd = new SqlCommand(strSQL, m_connection);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    int nLastImageID = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                    reader.Close();
                    return nLastImageID;
                }

                reader.Close();
                return 0;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }

            return -1;
        }

        public static void AddImage(SateImage image)
        {
            //m_dicImages[image.ID] = image;
            if (m_connection == null)
                return;

            try
            {
                string strSQL = "Insert into SateImage (ID, ImageName, ImageHIndex, ImageVIndex, LOD, ParentImageID, RegionID, xTL, yTL, xBL, yBL, xBR, yBR, Time, ImageURL, LengthPerPixel, Description)";
                strSQL += string.Format(" values ({0}, '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, '{13}', '{14}', {15}, '{16}')",
                    image.ID, image.ImageName, image.HIndex, image.VIndex,
                    image.LODLevel, image.ParentImageID == 0 ? "NULL" : image.ParentImageID.ToString(),
                    image.RegionID == 0 ? "NULL" : image.RegionID.ToString(),
                    image.TLx, image.TLy,
                    image.BLx, image.BLy,
                    image.BRx, image.BRy,
                    image.Time, image.URL,
                    image.Scale, image.Description);

                SqlCommand cmd = new SqlCommand(strSQL, m_connection);
                cmd.ExecuteNonQuery();

                m_dicImages[image.ID] = image;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
        }

        public static void RemoveImage(SateImage image)
        {
            //m_dicImages.Remove(image.ID);
            if (m_connection == null)
                return;

            try
            {
                string strSQL = "Delete from SateImage where ID = " + image.ID.ToString();
                SqlCommand cmd = new SqlCommand(strSQL, m_connection);
                cmd.ExecuteNonQuery();

                m_dicImages.Remove(image.ID);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
        }

        public static SateImage GetImage(int nID)
        {
            /*SateImage image;

            if (m_dicImages.TryGetValue(nID, out image))
                return image;

            return null;*/
            SateImage image = null;

            if (m_dicImages.TryGetValue(nID, out image))
                return image;

            if (m_connection == null)
                return null;

            try
            {
                string strSQL = "Select ID, ImageName, ImageHIndex, ImageVIndex, LOD, ParentImageID, RegionID, xTL, yTL, xBL, yBL, xBR, yBR, Time, ImageURL, LengthPerPixel, Description from SateImage where ID = " + nID.ToString();
         
                SqlCommand cmd = new SqlCommand(strSQL, m_connection);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string strImageName = reader.IsDBNull(1) ? "" : reader.GetString(1);
                    int nHIndex = reader.GetInt32(2);
                    int nVIndex = reader.GetInt32(3);
                    int nLOD = reader.GetInt32(4);
                    int nParentImageID = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);
                    int nRegionID = reader.IsDBNull(6) ? 0 : reader.GetInt32(6);
                    float xTL = reader.GetFloat(7);
                    float yTL = reader.GetFloat(8);
                    float xBL = reader.GetFloat(9);
                    float yBL = reader.GetFloat(10);
                    float xBR = reader.GetFloat(11);
                    float yBR = reader.GetFloat(12);
                    string strTime = reader.GetString(13);
                    string strURL = reader.GetString(14);
                    float fScale = reader.GetFloat(15);
                    string strDescrption = reader.IsDBNull(16) ? "" : reader.GetString(16);

                    reader.Close();

                    image = new SateImage();

                    image.ID = nID;
                    image.ImageName = strImageName;
                    image.HIndex = nHIndex;
                    image.VIndex = nVIndex;
                    image.LODLevel = nLOD;

                    if (nParentImageID > 0)
                        image.SetParentImage(GetImage(nParentImageID));

                    image.RegionID = nRegionID;
                    image.TLx = xTL;
                    image.TLy = yTL;
                    image.BLx = xBL;
                    image.BLy = yBL;
                    image.BRx = xBR;
                    image.BRy = yBR;
                    image.Time = strTime;
                    image.URL = strURL;
                    image.Scale = fScale;
                    image.Description = strDescrption;

                    m_dicImages[image.ID] = image;
                }
                else
                    reader.Close();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }

            return image;
        }

        public static List<SateImage> GetImageList(float xTL, float yTL, float xBL, float yBL, float xBR, float yBR)
        {
            if (m_connection == null)
                return null;

            try
            {
                string strWhere = string.Format(" where not (xTL >= {0} or xBR <= {1} or yTL <= {2} or yBR >= {3})",
                    xBR, xTL, yBR, yTL);
                string strSQL = "Select ID, ImageName, ImageHIndex, ImageVIndex, LOD, ParentImageID, RegionID, xTL, yTL, xBL, yBL, xBR, yBR, Time, ImageURL, LengthPerPixel, Description from SateImage " + strWhere;

                List<SateImage> images = new List<SateImage>();

                SqlCommand cmd = new SqlCommand(strSQL, m_connection);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int nID = reader.GetInt32(0);
                    string strImageName = reader.IsDBNull(1) ? "" : reader.GetString(1);
                    int nHIndex = reader.GetInt32(2);
                    int nVIndex = reader.GetInt32(3);
                    int nLOD = reader.GetInt32(4);
                    int nParentImageID = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);
                    int nRegionID = reader.IsDBNull(6) ? 0 : reader.GetInt32(6);
                    float _xTL = reader.GetFloat(7);
                    float _yTL = reader.GetFloat(8);
                    float _xBL = reader.GetFloat(9);
                    float _yBL = reader.GetFloat(10);
                    float _xBR = reader.GetFloat(11);
                    float _yBR = reader.GetFloat(12);
                    string strTime = reader.GetString(13);
                    string strURL = reader.GetString(14);
                    float fScale = reader.GetFloat(15);
                    string strDescrption = reader.IsDBNull(16) ? "" : reader.GetString(16);

                    SateImage image = new SateImage();

                    image.ID = nID;
                    image.ImageName = strImageName;
                    image.HIndex = nHIndex;
                    image.VIndex = nVIndex;
                    image.LODLevel = nLOD;

                    if (nParentImageID > 0)
                        image.SetParentImage(GetImage(nParentImageID));

                    image.RegionID = nRegionID;
                    image.TLx = _xTL;
                    image.TLy = _yTL;
                    image.BLx = _xBL;
                    image.BLy = _yBL;
                    image.BRx = _xBR;
                    image.BRy = _yBR;
                    image.Time = strTime;
                    image.URL = strURL;
                    image.Scale = fScale;
                    image.Description = strDescrption;

                    m_dicImages[image.ID] = image;
                    images.Add(image);
                }

                reader.Close();
                return images;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }

            return null;
        }

        public static List<SateImage> GetImageList(string strImageName)
        {
            if (m_connection == null)
                return null;

            try
            {
                string strSQL = "Select ID, ImageName, ImageHIndex, ImageVIndex, LOD, ParentImageID, RegionID, xTL, yTL, xBL, yBL, xBR, yBR, Time, ImageURL, LengthPerPixel, Description from SateImage where ImageName = '" + strImageName + "'";

                List<SateImage> images = new List<SateImage>();

                SqlCommand cmd = new SqlCommand(strSQL, m_connection);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int nID = reader.GetInt32(0);
                    int nHIndex = reader.GetInt32(2);
                    int nVIndex = reader.GetInt32(3);
                    int nLOD = reader.GetInt32(4);
                    int nParentImageID = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);
                    int nRegionID = reader.IsDBNull(6) ? 0 : reader.GetInt32(6);
                    float _xTL = reader.GetFloat(7);
                    float _yTL = reader.GetFloat(8);
                    float _xBL = reader.GetFloat(9);
                    float _yBL = reader.GetFloat(10);
                    float _xBR = reader.GetFloat(11);
                    float _yBR = reader.GetFloat(12);
                    string strTime = reader.GetString(13);
                    string strURL = reader.GetString(14);
                    float fScale = reader.GetFloat(15);
                    string strDescrption = reader.IsDBNull(16) ? "" : reader.GetString(16);

                    SateImage image = new SateImage();

                    image.ID = nID;
                    image.ImageName = strImageName;
                    image.HIndex = nHIndex;
                    image.VIndex = nVIndex;
                    image.LODLevel = nLOD;

                    if (nParentImageID > 0)
                        image.SetParentImage(GetImage(nParentImageID));

                    image.RegionID = nRegionID;
                    image.TLx = _xTL;
                    image.TLy = _yTL;
                    image.BLx = _xBL;
                    image.BLy = _yBL;
                    image.BRx = _xBR;
                    image.BRy = _yBR;
                    image.Time = strTime;
                    image.URL = strURL;
                    image.Scale = fScale;
                    image.Description = strDescrption;

                    m_dicImages[image.ID] = image;
                    images.Add(image);
                }

                reader.Close();
                return images;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }

            return null;
        }

        public static int GetLastImageLODDepth(string strImageName)
        {
            if (m_connection == null)
                return 0;

            try
            {
                string strSQL = string.Format("select max(LOD) from SateImage where ImageName = '{0}' and Time = (select max(Time) from SateImage where ImageName = '{0}')",
                    strImageName);

                SqlCommand cmd = new SqlCommand(strSQL, m_connection);
                SqlDataReader reader = cmd.ExecuteReader();

                int maxLODDepth = 0;

                if (reader.Read())
                {
                    maxLODDepth = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                }

                reader.Close();
                return maxLODDepth;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }

            return 0;
        }

        public static int GetImageLODDepth(string strImageName, string strTime)
        {
            if (m_connection == null)
                return 0;

            try
            {
                string strSQL = string.Format("select max(LOD) from SateImage where ImageName = '{0}' and Time = '{1}'",
                    strImageName, strTime);

                SqlCommand cmd = new SqlCommand(strSQL, m_connection);
                SqlDataReader reader = cmd.ExecuteReader();

                int maxLODDepth = 0;

                if (reader.Read())
                {
                    maxLODDepth = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                }

                reader.Close();
                return maxLODDepth;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }

            return 0;
        }

        public static List<SateImage> GetLastImageList(string strImageName, int nLODIndex)
        {
            if (m_connection == null)
                return null;

            try
            {
                string strSQL = "Select ID, ImageName, ImageHIndex, ImageVIndex, LOD, ParentImageID, RegionID, xTL, yTL, xBL, yBL, xBR, yBR, Time, ImageURL, LengthPerPixel, Description from SateImage ";
                strSQL += string.Format("where ImageName = '{0}' and LOD = {1} and Time = (select max(Time) from SateImage where ImageName = '{0}')",
                    strImageName, nLODIndex);

                List<SateImage> images = new List<SateImage>();

                SqlCommand cmd = new SqlCommand(strSQL, m_connection);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int nID = reader.GetInt32(0);
                    int nHIndex = reader.GetInt32(2);
                    int nVIndex = reader.GetInt32(3);
                    int nParentImageID = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);
                    int nRegionID = reader.IsDBNull(6) ? 0 : reader.GetInt32(6);
                    float _xTL = reader.GetFloat(7);
                    float _yTL = reader.GetFloat(8);
                    float _xBL = reader.GetFloat(9);
                    float _yBL = reader.GetFloat(10);
                    float _xBR = reader.GetFloat(11);
                    float _yBR = reader.GetFloat(12);
                    string strTime = reader.GetString(13);
                    string strURL = reader.GetString(14);
                    float fScale = reader.GetFloat(15);
                    string strDescrption = reader.IsDBNull(16) ? "" : reader.GetString(16);

                    SateImage image = new SateImage();

                    image.ID = nID;
                    image.ImageName = strImageName;
                    image.HIndex = nHIndex;
                    image.VIndex = nVIndex;
                    image.LODLevel = nLODIndex;

                    if (nParentImageID > 0)
                        image.SetParentImage(GetImage(nParentImageID));

                    image.RegionID = nRegionID;
                    image.TLx = _xTL;
                    image.TLy = _yTL;
                    image.BLx = _xBL;
                    image.BLy = _yBL;
                    image.BRx = _xBR;
                    image.BRy = _yBR;
                    image.Time = strTime;
                    image.URL = strURL;
                    image.Scale = fScale;
                    image.Description = strDescrption;

                    m_dicImages[image.ID] = image;
                    images.Add(image);
                }

                reader.Close();
                return images;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }

            return null;
        }

        public static List<SateImage> GetImageList(string strImageName, int nLODIndex, string strTime)
        {
            if (m_connection == null)
                return null;

            try
            {
                string strSQL = "Select ID, ImageName, ImageHIndex, ImageVIndex, LOD, ParentImageID, RegionID, xTL, yTL, xBL, yBL, xBR, yBR, Time, ImageURL, LengthPerPixel, Description from SateImage ";
                strSQL += string.Format("where ImageName = '{0}' and LOD = {1} and Time = '{2}'",
                    strImageName, nLODIndex, strTime);

                List<SateImage> images = new List<SateImage>();

                SqlCommand cmd = new SqlCommand(strSQL, m_connection);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int nID = reader.GetInt32(0);
                    int nHIndex = reader.GetInt32(2);
                    int nVIndex = reader.GetInt32(3);
                    int nParentImageID = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);
                    int nRegionID = reader.IsDBNull(6) ? 0 : reader.GetInt32(6);
                    float _xTL = reader.GetFloat(7);
                    float _yTL = reader.GetFloat(8);
                    float _xBL = reader.GetFloat(9);
                    float _yBL = reader.GetFloat(10);
                    float _xBR = reader.GetFloat(11);
                    float _yBR = reader.GetFloat(12);
                    string strURL = reader.GetString(14);
                    float fScale = reader.GetFloat(15);
                    string strDescrption = reader.IsDBNull(16) ? "" : reader.GetString(16);

                    SateImage image = new SateImage();

                    image.ID = nID;
                    image.ImageName = strImageName;
                    image.HIndex = nHIndex;
                    image.VIndex = nVIndex;
                    image.LODLevel = nLODIndex;

                    if (nParentImageID > 0)
                        image.SetParentImage(GetImage(nParentImageID));

                    image.RegionID = nRegionID;
                    image.TLx = _xTL;
                    image.TLy = _yTL;
                    image.BLx = _xBL;
                    image.BLy = _yBL;
                    image.BRx = _xBR;
                    image.BRy = _yBR;
                    image.Time = strTime;
                    image.URL = strURL;
                    image.Scale = fScale;
                    image.Description = strDescrption;

                    m_dicImages[image.ID] = image;
                    images.Add(image);
                }

                reader.Close();
                return images;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }

            return null;
        }

        public static void Clear()
        {
            m_dicImages.Clear();

            if (m_connection == null)
                return;

            try
            {
                string strSQL = "Delete from SateImage";
                SqlCommand cmd = new SqlCommand(strSQL, m_connection);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
        }

        public static bool ConnectDB(string strDbPath)
        {
            if (m_connection != null)
                return true;

            try
            {
                string strConnection = "Data Source=(LocalDB)\\v11.0;AttachDbFilename=" + strDbPath + ";Integrated Security=True;Connect Timeout=30";
                m_connection = new SqlConnection(strConnection);
                m_connection.Open();

                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                m_connection = null;
            }

            return false;
        }

        public static void CloseDB()
        {
            if (m_connection != null)
            {
                m_connection.Close();
                m_connection = null;
            }
        }
    }
}