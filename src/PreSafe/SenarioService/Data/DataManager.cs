using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Collections;

namespace PreSafe
{
    public class DataManager
    {
        private string m_szSqlConnectionString = @"Data Source=DEVSERVER2;Initial Catalog=PreSafe;User ID=sa;Password=9449966Ab";
        private string m_szDBName = String.Empty;
        public String DBName { get { return m_szDBName; } }

        
        public DataManager()
        {
            using (SqlConnection conn = new SqlConnection(m_szSqlConnectionString))
            {
                conn.Open();

                try
                {
                    //SqlCommand cmd = new SqlCommand(

                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public bool AddData(
            string szDeviceID, int nDeviceType,
            bool bUseLocation, int nLocation,
            bool bUseHeartBeat, int nHeartBeat,
            bool bUseAcc, int nAcc,
            bool bUseAlcohol, int nAlcohol,
            bool bUseSound, int nSound,
            bool bUseImpact, int nImpact,
            string szDescription)
        {
            bool bReturn = false;

            using (SqlConnection conn = new SqlConnection(m_szSqlConnectionString))
            {
                conn.Open();

                try
                {
                    string sql = String.Format(@"INSERT INTO EBDeviceHistory (ID, DeviceID, DeviceType, CLocation, HeartBeat, Acceleration, Drinking, Sound, Impact, TimeStamp, Description)
                                                SELECT ISNULL(MAX(ID), 0) + 1, '{0}', {1}, {2}, {3}, {4}, {5}, {6}, {7}, GetDate(), {8} FROM EBDeviceHistory",
                        szDeviceID, nDeviceType,
                        (bUseLocation ? nLocation.ToString() : "NULL"),
                        (bUseHeartBeat ? nHeartBeat.ToString() : "NULL"),
                        (bUseAcc ? nAcc.ToString() : "NULL"),
                        (bUseAlcohol ? nAlcohol.ToString() : "NULL"),
                        (bUseSound ? nSound.ToString() : "NULL"),
                        (bUseImpact ? nImpact.ToString() : "NULL"),
                        String.Format("'{0}'", szDescription));

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        cmd.Dispose();

                        int nID = 0;
                        int nHistoryID = 0;

                        sql = String.Format(@"SELECT ISNULL(MAX(ID), 0) AS ID FROM EBDeviceData WHERE DeviceID = '{0}' AND DeviceType = {1}", szDeviceID, nDeviceType);
                        cmd = new SqlCommand(sql, conn);
                        SqlDataReader sr = cmd.ExecuteReader();
                        while (sr.Read())
                        {
                            nID = Convert.ToInt32(sr["ID"]);
                        }
                        sr.Close();
                        cmd.Dispose();

                        sql = String.Format(@"SELECT MAX(ID) AS ID FROM EBDeviceHistory WHERE DeviceID = '{0}' AND DeviceType = {1}", szDeviceID, nDeviceType);
                        cmd = new SqlCommand(sql, conn);
                        sr = cmd.ExecuteReader();
                        while (sr.Read())
                        {
                            nHistoryID = Convert.ToInt32(sr["ID"]);
                        }
                        sr.Close();
                        cmd.Dispose();

                        if (nID == 0)
                        {
                            sql = String.Format(@"INSERT INTO EBDeviceData (ID, DeviceID, DeviceType, CLocation, HeartBeat, Acceleration, Drinking, Sound, Impact, TimeStamp, Description, HistoryID)
                                                SELECT ISNULL(MAX(ID), 0) + 1, '{0}', {1}, {2}, {3}, {4}, {5}, {6}, {7}, GetDate(), {8}, {9} FROM EBDeviceData",
                            szDeviceID, nDeviceType,
                            (bUseLocation ? nLocation.ToString() : "NULL"),
                            (bUseHeartBeat ? nHeartBeat.ToString() : "NULL"),
                            (bUseAcc ? nAcc.ToString() : "NULL"),
                            (bUseAlcohol ? nAlcohol.ToString() : "NULL"),
                            (bUseSound ? nSound.ToString() : "NULL"),
                            (bUseImpact ? nImpact.ToString() : "NULL"),
                            String.Format("'{0}'", szDescription),
                            nHistoryID);
                        }
                        else
                        {
                            sql = String.Format(@"UPDATE EBDeviceData SET CLocation = {0},
                                                                                HeartBeat = {1},
                                                                                Acceleration = {2},
                                                                                Drinking = {3},
                                                                                Sound = {4},
                                                                                Impact = {5}, 
                                                                                TimeStamp = GETDATE(),
                                                                                Description = {6},
                                                                                HistoryID = {7}
                                                                    WHERE ID = {8}",
                            (bUseLocation ? nLocation.ToString() : "NULL"),
                            (bUseHeartBeat ? nHeartBeat.ToString() : "NULL"),
                            (bUseAcc ? nAcc.ToString() : "NULL"),
                            (bUseAlcohol ? nAlcohol.ToString() : "NULL"),
                            (bUseSound ? nSound.ToString() : "NULL"),
                            (bUseImpact ? nImpact.ToString() : "NULL"),
                            String.Format("'{0}'", szDescription),
                            nHistoryID,
                            nID);
                        }

                        cmd = new SqlCommand(sql, conn);

                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            bReturn = true;
                        }

                        cmd.Dispose();
                    }

                }
                finally
                {
                    conn.Close();
                }
            }

            return bReturn;
        }

        public object[] LoadData(string szDeviceID, int nDeviceType)
        {
            ArrayList arrResult = new ArrayList();

            using (SqlConnection conn = new SqlConnection(m_szSqlConnectionString))
            {
                conn.Open();

                try
                {
                    string sql = String.Format(@"SELECT ID, HistoryID, DeviceID, DeviceType, CLocation, HeartBeat, Acceleration, Drinking, Sound, Impact, TimeStamp, Description FROM EBDeviceData WHERE DeviceID = '{0}' AND DeviceType = {1}",
                        szDeviceID, nDeviceType);

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader sr = cmd.ExecuteReader();
                    while (sr.Read())
                    {
                        for(int i = 0 ; i < sr.FieldCount ; i++)
                        {
                            if (sr[i] == DBNull.Value)
                            {
                                arrResult.Add("NULL");
                            }
                            else
                            {
                                arrResult.Add(sr[i]);
                            }
                        }
                    }
                    sr.Close();
                    cmd.Dispose();

                }
                finally
                {
                    conn.Close();
                }
            }


            return arrResult.ToArray();
        }


    }
}