using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Reflection;
using System.IO;
using System.Security;
using System.Security.AccessControl;
 
namespace TankModbusLib
{
    internal class ConfigFile
    {
        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder refval, int size, string filepath);

        [DllImport("kernel32.dll")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filepath);
        
        private string m_szFilePath = "";
        public ConfigFile(string szFilePath)
        {
            try
            {
                if (File.Exists(szFilePath))
                {
                    m_szFilePath = szFilePath;
                }
            }
            catch(Exception ex)
            { 
            }
        }        

        public string SetValue(string section, string key, string value)
        {
            StringBuilder temp = new StringBuilder(1024);
            string strPath = m_szFilePath;
            WritePrivateProfileString(section, key, value, strPath);
            return temp.ToString();
        }

        public string GetValue(string section, string key)
        {
            StringBuilder temp = new StringBuilder(1024);
            string strPath = m_szFilePath;
            int nResult = GetPrivateProfileString(section, key, "", temp, 1024, strPath);
            m_length = temp.Length;
            return temp.ToString();
        }
        
        private int m_length = 0;
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

    internal class RegUtil
    {
        internal static string ReadRegValue(string section, string key)
        {
            string szResult = "";
            try
            {
                string szKey = @"Software\UNE\" + section;
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

        internal static void WriteRegValue(string section, string key, string szValue)
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

                string szKey = @"Software\UNE\" + section;
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
