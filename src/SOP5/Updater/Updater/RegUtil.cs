using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Reflection;
using System.Security;
using System.Security.AccessControl;
using System.Windows.Forms;

namespace Updater
{
    //public class RegUtil
    //{
    //    public static string ReadRegValue(string section, string key)
    //    {
    //        string szResult = "";
    //        try
    //        {
    //            string szKey = @"Software\UNE\" + section;
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

    //    public static void WriteRegValue(string section, string key, string szValue)
    //    {
    //        try
    //        {
    //            string szUserName = Environment.UserDomainName + "\\" + Environment.UserName;

    //            RegistrySecurity rs = new RegistrySecurity();

    //            rs.AddAccessRule(new RegistryAccessRule(szUserName,
    //                RegistryRights.ReadKey | RegistryRights.Delete | RegistryRights.WriteKey,
    //                InheritanceFlags.None,
    //                PropagationFlags.None,
    //                AccessControlType.Allow));

    //            rs.AddAccessRule(new RegistryAccessRule(szUserName,
    //                RegistryRights.ChangePermissions,
    //                InheritanceFlags.None,
    //                PropagationFlags.None,
    //                AccessControlType.Deny));

    //            string szKey = @"Software\UNE\" + section;
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


        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder refval, int size, string filepath);
        
        public static string ReadINI(string section, string key)
        {
            StringBuilder temp = new StringBuilder(255);
            string strPath = Application.StartupPath + "\\config.ini";
            GetPrivateProfileString(section, key, "", temp, 255, strPath);
            return temp.ToString();
        }
    }

    

}
