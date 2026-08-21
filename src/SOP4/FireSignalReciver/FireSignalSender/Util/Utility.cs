using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Reflection;
using System.Security;
using System.Security.AccessControl;


namespace FireSignalSender
{
    public class Utility
    {
        int m_length;

        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder refval, int size, string filepath);

        [DllImport("kernel32.dll")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filepath);




        public string setinivalue(string section, string key, string value, string filepath)
        {
            StringBuilder temp = new StringBuilder(255);
            WritePrivateProfileString(section, key, value, filepath);
            return temp.ToString();
        }

        public string setinivalue(string section, string key, string value)
        {
            StringBuilder temp = new StringBuilder(255);
            string strPath = Application.StartupPath + "\\config.ini";
            WritePrivateProfileString(section, key, value, strPath);
            return temp.ToString();
        }

        public string getinivalue(string section, string key)
        {
            StringBuilder temp = new StringBuilder(255);
            string strPath = Application.StartupPath + "\\config.ini";
            GetPrivateProfileString(section, key, "", temp, 255, strPath);
            m_length = temp.Length;

            return temp.ToString();

        }

        public string getinivalue(string section, string key, string filepath)
        {
            StringBuilder temp = new StringBuilder(255);
            int nLen = GetPrivateProfileString(section, key, "", temp, 255, filepath);
            m_length = temp.Length;

            return temp.ToString();

        }

        public int Length
        {
            get { return m_length; }
            set { m_length = value; }
        }

        // 앞 뒤의 공백문자를 제거
        public static string TrimString(string str)
        {
            str = str.TrimStart(new char[] { ' ', '\t', '\r', '\n' });
            str = str.TrimEnd(new char[] { ' ', '\t', '\r', '\n' });
            return str;
        }

        public static string MakeDateTimeString(DateTime time)
        {
            return string.Format("{0} {1:00}:{2:00}:{3:00}", time.ToShortDateString(), time.Hour, time.Minute, time.Second);
        }

        public static void SetDoubleBuffer(Panel panel, bool bEnabled)
        {
            Type dgvType1 = panel.GetType();
            PropertyInfo pi1 = dgvType1.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi1.SetValue(panel, bEnabled, null);
        }

        public static void SetDoubleBuffer(DataGridView gvView, bool bEnabled)
        {
            Type dgvType1 = gvView.GetType();
            PropertyInfo pi1 = dgvType1.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi1.SetValue(gvView, bEnabled, null);
        }
    }

    public class RegUtil
    {
        public static string ReadRegValue(string section, string key, int nSiteID)
        {
            string szResult = "";
            try
            {
                string szKey = @"Software\UNE\Site\" + nSiteID.ToString() + "\\" + section;
                RegistryKey rkey = Registry.CurrentUser.OpenSubKey(szKey);
                if (rkey == null)
                {
                    return "";
                }
                else
                {
                    szResult = (string)rkey.GetValue(key, "");
                }
                if (rkey != null)
                    rkey.Close();
            }
            catch (System.Exception)
            {
            }
            return szResult;
        }

        public static void WriteRegValue(string section, string key, string szValue, int nSiteID)
        {
            try
            {
                string szUserName = Environment.UserDomainName + "\\" + Environment.UserName;

                RegistrySecurity rs = new RegistrySecurity();

                rs.AddAccessRule(new RegistryAccessRule(szUserName,
                    RegistryRights.ReadKey | RegistryRights.Delete | RegistryRights.WriteKey,
                    InheritanceFlags.None,
                    PropagationFlags.None,
                    AccessControlType.Allow));

                rs.AddAccessRule(new RegistryAccessRule(szUserName,
                    RegistryRights.ChangePermissions,
                    InheritanceFlags.None,
                    PropagationFlags.None,
                    AccessControlType.Deny));

                string szKey = @"Software\UNE\Site\" + nSiteID.ToString() + "\\" + section;
                RegistryKey rkey = Registry.CurrentUser.OpenSubKey(szKey, true);
                if (rkey == null)
                {
                    try
                    {
                        rkey = Registry.CurrentUser.CreateSubKey(szKey, RegistryKeyPermissionCheck.ReadWriteSubTree, rs);
                    }
                    catch (Exception)
                    {
                    }
                }

                if (rkey != null)
                {
                    rkey.SetValue(key, szValue);
                    rkey.Close();
                }
            }
            catch (System.Exception)
            {
            }
        }
    }

}
