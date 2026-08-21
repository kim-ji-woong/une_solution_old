using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Reflection;
using System.Security;
using System.Security.AccessControl;


namespace MessageServer
{
    public class RegUtil
    {
        public static string ReadRegValue(string section, string key)
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
            catch (System.Exception ex)
            {
                MessageService.Logger.Debug(ex.Message);
                MessageService.Logger.Debug(ex.StackTrace);
            }
            return szResult;
        }

        public static void WriteRegValue(string section, string key, string szValue)
        {
            try
            {
                string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                
                RegistrySecurity rs = new RegistrySecurity();
                rs.AddAccessRule(new RegistryAccessRule(userName,
                    RegistryRights.ReadKey | RegistryRights.Delete | RegistryRights.WriteKey,
                    InheritanceFlags.None,
                    PropagationFlags.None,
                    AccessControlType.Allow));

                rs.AddAccessRule(new RegistryAccessRule(userName,
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
            catch (System.Exception ex)
            {
                MessageService.Logger.Debug(ex.Message);
                MessageService.Logger.Debug(ex.StackTrace);
            }
        }
    }
}
