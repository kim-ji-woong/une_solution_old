using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Security.AccessControl;
using Microsoft.Win32;

namespace HelpViewer
{
    public class RegUtil
    {
        public static bool CheckIEOption(string strExePath)
        {
            string strExeName = GetExeName(strExePath);

            RegistryKey keyParent = null;
            string[] keys = new string[6] { "SOFTWARE", "Microsoft", "Internet Explorer", "MAIN", "FeatureControl", "FEATURE_BROWSER_EMULATION" };

            string strKey = "";

            for (int i = 0; i < 6;i++ )
            {
                if (strKey.Length == 0)
                    strKey = keys[i];
                else
                    strKey += "\\" + keys[i];
            }

            RegistryKey directKey = GetKey(null, strKey, false);

            if (directKey == null)
            {
                // 이 값을 찾을수 없으면 Internet Explorer 설치에 뭔가 문제가 생겼음.
                return true;
            }

            // IE 옵션이 이미 설정되어 있음
            if (directKey.GetValue(strExeName) != null)
            {
                directKey.Close();
                return true;
            }

            directKey.Close();

            for (int i = 0; i < 6; i++)
            {
                RegistryKey key = GetKey(keyParent, keys[i], i == 5);

                if (key == null)
                {
                    System.Windows.Forms.MessageBox.Show("IE 옵션이 적용되어 있지 않습니다.\r\n관리자 권한으로 프로그램을 재시작하여 주세요.");
                    //key = MakeKey(null, keys[i]);

                    //if (key == null)
                    return false;
                }

                if (keyParent != null)
                    keyParent.Close();

                keyParent = key;
            }

            try
            {
                object value = keyParent.GetValue(strExeName);

                if (value == null)
                {
                    keyParent.SetValue(strExeName, 0x8888, RegistryValueKind.DWord);
                    System.Windows.Forms.MessageBox.Show("IE 옵션이 변경되었습니다.\r\n프로그램을 재시작하여 주세요.");
                    return false;
                }

                keyParent.Close();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                keyParent.Close();
                //return false;
            }

            return true;
        }

        private static string GetExeName(string strPath)
        {
            int nIndex = strPath.LastIndexOf('\\');

            if (nIndex < 0)
                return strPath;

            return strPath.Substring(nIndex + 1);
        }

        private static RegistryKey GetKey(RegistryKey keyParent, string key, bool writable)
        {
            try
            {
                if (keyParent == null)
                    keyParent = Registry.LocalMachine;

                if (writable)
                {
                    string szUserName = Environment.UserDomainName + "\\" + Environment.UserName;
                    RegistryAccessRule rule = new RegistryAccessRule(szUserName,
                        RegistryRights.FullControl,
                        AccessControlType.Allow);

                    RegistrySecurity rs = new RegistrySecurity();
                    rs.AddAccessRule(rule);

                    RegistryKey rkey = keyParent.OpenSubKey(key, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);
                    rkey.SetAccessControl(rs);
                    return rkey;
                }
                else
                {
                    RegistryKey rkey = keyParent.OpenSubKey(key);
                    return rkey;
                }
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }

            return null;
        }

        private static RegistryKey MakeKey(RegistryKey keyParent, string key)
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

                if (keyParent == null)
                    keyParent = Registry.LocalMachine;

                return keyParent.CreateSubKey(key, RegistryKeyPermissionCheck.ReadWriteSubTree, rs);
            }
            catch (Exception)
            {
            }

            return null;
        }
    }
}
