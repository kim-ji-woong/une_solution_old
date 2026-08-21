using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Reflection;
using System.Security;
using System.Security.AccessControl;

namespace dnsDBUtil
{
    public class Utility
    {
        int m_length;

        [System.Runtime.InteropServices.DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileString", CharSet = CharSet.Ansi, ExactSpelling = false)]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder refval, int size, string filepath);

        [System.Runtime.InteropServices.DllImport("kernel32.dll", EntryPoint = "WritePrivateProfileString", CharSet = CharSet.Ansi, ExactSpelling = false)]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filepath);


        private string szFileName = "config.ini";
        private string szAssemPath = "";
        private string configFilePath = "";

        private void MakePath()
        {
            try
            {
                szAssemPath = System.Reflection.Assembly.GetEntryAssembly().Location;
                string szFullPath = System.IO.Directory.GetParent(szAssemPath).FullName;
                configFilePath = szFullPath + "\\" + szFileName;
            }
            catch (Exception)
            { }

        }

        public Utility()
        {
            MakePath();
        }

        public Utility(string iniFileName_PathExclude)
        {
            szFileName = iniFileName_PathExclude;
            MakePath();
        }

        public string setinivalue(string section, string key, string value, string filepath)
        {
            StringBuilder temp = new StringBuilder(255);
            WritePrivateProfileString(section, key, value, filepath);
            return temp.ToString();
        }

        public string setinivalue(string section, string key, string value)
        {
            StringBuilder temp = new StringBuilder(255);
            WritePrivateProfileString(section, key, value, configFilePath);
            return temp.ToString();
        }

        public string getinivalue(string section, string key)
        {
            StringBuilder temp = new StringBuilder(255);
            GetPrivateProfileString(section, key, "", temp, 255, configFilePath);
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
    }

    //public class RegUtil
    //{
    //    public static string ReadRegValue(string section, string key, int nSiteID)
    //    {
    //        string szResult = "";
    //        try
    //        {
    //            string szKey = @"Software\UNE\Site\" + nSiteID.ToString() + "\\" + section;
    //            RegistryKey rkey = Registry.CurrentUser.OpenSubKey(szKey);
    //            if (rkey == null)
    //            {
    //                return "";
    //            }
    //            else
    //            {
    //                szResult = (string)rkey.GetValue(key, "");
    //            }
    //            if (rkey != null)
    //                rkey.Close();
    //        }
    //        catch (System.Exception)
    //        {
    //        }
    //        return szResult;
    //    }
    //
    //    public static void WriteRegValue(string section, string key, string szValue, int nSiteID)
    //    {
    //        try
    //        {
    //            string szUserName = Environment.UserDomainName + "\\" + Environment.UserName;
    //
    //            RegistrySecurity rs = new RegistrySecurity();
    //
    //            rs.AddAccessRule(new RegistryAccessRule(szUserName,
    //                RegistryRights.ReadKey | RegistryRights.Delete | RegistryRights.WriteKey,
    //                InheritanceFlags.None,
    //                PropagationFlags.None,
    //                AccessControlType.Allow));
    //
    //            rs.AddAccessRule(new RegistryAccessRule(szUserName,
    //                RegistryRights.ChangePermissions,
    //                InheritanceFlags.None,
    //                PropagationFlags.None,
    //                AccessControlType.Deny));
    //
    //            string szKey = @"Software\UNE\Site\" + nSiteID.ToString() + "\\" + section;
    //            RegistryKey rkey = Registry.CurrentUser.OpenSubKey(szKey, true);
    //            if (rkey == null)
    //            {
    //                try
    //                {
    //                    rkey = Registry.CurrentUser.CreateSubKey(szKey, RegistryKeyPermissionCheck.ReadWriteSubTree, rs);
    //                }
    //                catch (Exception)
    //                {
    //                }
    //            }
    //
    //            if (rkey != null)
    //            {
    //                rkey.SetValue(key, szValue);
    //                rkey.Close();
    //            }
    //        }
    //        catch (System.Exception)
    //        {
    //        }
    //    }
    //}

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
