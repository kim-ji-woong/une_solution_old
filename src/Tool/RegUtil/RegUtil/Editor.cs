using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace RegUtil
{
    public enum HKeyType
    {
        CLASSES_ROOT = 0,
        CURRENT_USER,
        LOCAL_MACHINE,
        USERS,
        CURRENT_CONFIG
    };

    public class Editor
    {
        private static RegistryKey GetSubKey(RegistryKey hkey, string strSubPath, bool writable = false)
        {
            try
            {
                if (hkey == null)
                    return null;

                return hkey.OpenSubKey(strSubPath, writable);
            }
            catch (Exception)
            {
            }

            return null;
        }

        private static RegistryKey GetHKey(string strFullPath, out string strSubPath)
        {
            int nIndex = strFullPath.IndexOf('\\');

            if (nIndex < 0)
            {
                strSubPath = "";
                return null;
            }

            string strHKey = strFullPath.Substring(0, nIndex);
            strSubPath = strFullPath.Substring(nIndex + 1);

            RegistryKey hkey = null;

            if (string.Compare(strHKey, "HKEY_CLASSES_ROOT", true) == 0)
                hkey = Registry.ClassesRoot;
            else if (string.Compare(strHKey, "HKEY_CURRENT_USER", true) == 0)
                hkey = Registry.CurrentUser;
            else if (string.Compare(strHKey, "HKEY_LOCAL_MACHINE", true) == 0)
                hkey = Registry.LocalMachine;
            else if (string.Compare(strHKey, "HKEY_USERS", true) == 0)
                hkey = Registry.Users;
            else if (string.Compare(strHKey, "HKEY_CURRENT_CONFIG", true) == 0)
                hkey = Registry.CurrentConfig;

            return hkey;
        }

        private static bool IsValidPath(RegistryKey hkey, string strSubPath)
        {
            RegistryKey subKey = GetSubKey(hkey, strSubPath);

            if (subKey != null)
            {
                subKey.Close();
                return true;
            }
            else
            {
                int nIndex = strSubPath.LastIndexOf('\\');

                if (nIndex >= 0)
                {
                    string strSubKey = strSubPath.Substring(0, nIndex);
                    string strTargetValue = strSubPath.Substring(nIndex + 1);

                    subKey = GetSubKey(hkey, strSubKey);

                    if (subKey != null)
                    {
                        string[] arrValues = subKey.GetValueNames();
                        subKey.Close();

                        foreach (string strValue in arrValues)
                        {
                            if (strValue == strTargetValue)
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        public static bool IsValidPath(string strFullPath)
        {
            string strSubPath;
            RegistryKey hkey = GetHKey(strFullPath, out strSubPath);

            if (hkey == null)
                return false;

            return IsValidPath(hkey, strSubPath);
        }

        public static bool IsValidPath(HKeyType type, string strSubPath)
        {
            RegistryKey hkey = null;

            if (type == HKeyType.CLASSES_ROOT)
                hkey = Registry.ClassesRoot;
            else if (type == HKeyType.CURRENT_USER)
                hkey = Registry.CurrentUser;
            else if (type == HKeyType.LOCAL_MACHINE)
                hkey = Registry.LocalMachine;
            else if (type == HKeyType.USERS)
                hkey = Registry.Users;
            else if (type == HKeyType.CURRENT_CONFIG)
                hkey = Registry.CurrentConfig;
            else
                return false;

            return IsValidPath(hkey, strSubPath);
        }

        private static bool ReadValue(RegistryKey hkey, string strSubPath, out string strValue)
        {
            int nIndex = strSubPath.LastIndexOf('\\');

            if (nIndex < 0)
            {
                strValue = "";
                return false;
            }

            string strKey = strSubPath.Substring(nIndex + 1);
            strSubPath = strSubPath.Substring(0, nIndex);

            RegistryKey subKey = GetSubKey(hkey, strSubPath);

            if (subKey == null)
            {
                strValue = "";
                return false;
            }

            try
            {
                string strDefault = "aabbccdd~!@";

                strValue = (string)subKey.GetValue(strKey, strDefault);
                subKey.Close();

                if (strValue == strDefault)
                {
                    strValue = "";
                    return false;
                }
                else
                    return true;
            }
            catch (Exception)
            {
                strValue = "";
            }

            return false;
        }

        public static bool ReadValue(string strFullPath, out string strValue)
        {
            string strSubPath;
            RegistryKey hkey = GetHKey(strFullPath, out strSubPath);

            if (hkey == null)
            {
                strValue = "";
                return false;
            }

            return ReadValue(hkey, strSubPath, out strValue);
        }

        public static bool ReadValue(HKeyType type, string strSubPath, out string strValue)
        {
            RegistryKey hkey = null;

            if (type == HKeyType.CLASSES_ROOT)
                hkey = Registry.ClassesRoot;
            else if (type == HKeyType.CURRENT_USER)
                hkey = Registry.CurrentUser;
            else if (type == HKeyType.LOCAL_MACHINE)
                hkey = Registry.LocalMachine;
            else if (type == HKeyType.USERS)
                hkey = Registry.Users;
            else if (type == HKeyType.CURRENT_CONFIG)
                hkey = Registry.CurrentConfig;
            else
            {
                strValue = "";
                return false;
            }

            return ReadValue(hkey, strSubPath, out strValue);
        }

        private static bool DeletePath(RegistryKey hkey, string strSubPath)
        {
            int nIndex = strSubPath.LastIndexOf('\\');

            if (nIndex < 0)
                return false;

            string strSubKey = strSubPath.Substring(nIndex + 1);
            string strPath = strSubPath.Substring(0, nIndex);

            RegistryKey key = GetSubKey(hkey, strPath, true);

            if (key == null)
                return false;

            try
            {
                key.DeleteSubKeyTree(strSubKey);
            }
            catch (Exception)
            {
                try
                {
                    key.DeleteValue(strSubKey);
                }
                catch (Exception)
                {
                    key.Close();
                    return false;
                }
            }

            key.Close();
            return true;
        }

        public static bool DeletePath(string strFullPath)
        {
            string strSubPath;
            RegistryKey hkey = GetHKey(strFullPath, out strSubPath);

            if (hkey == null)
                return false;

            return DeletePath(hkey, strSubPath);
        }

        public static bool DeletePath(HKeyType type, string strSubPath)
        {
            RegistryKey hkey = null;

            if (type == HKeyType.CLASSES_ROOT)
                hkey = Registry.ClassesRoot;
            else if (type == HKeyType.CURRENT_USER)
                hkey = Registry.CurrentUser;
            else if (type == HKeyType.LOCAL_MACHINE)
                hkey = Registry.LocalMachine;
            else if (type == HKeyType.USERS)
                hkey = Registry.Users;
            else if (type == HKeyType.CURRENT_CONFIG)
                hkey = Registry.CurrentConfig;
            else
            {
                return false;
            }

            return DeletePath(hkey, strSubPath);
        }

        private static bool InsertKey(RegistryKey hkey, string strSubPath)
        {
            int nIndex = strSubPath.LastIndexOf('\\');

            if (nIndex < 0)
                return false;

            string strSubKey = strSubPath.Substring(nIndex + 1);
            string strPath = strSubPath.Substring(0, nIndex);

            RegistryKey key = GetSubKey(hkey, strPath, true);

            if (key == null)
                return false;

            try
            {
                key.CreateSubKey(strSubKey);
            }
            catch (Exception)
            {
                key.Close();
                return false;
            }

            key.Close();
            return true;
        }

        public static bool InsertKey(string strFullPath)
        {
            string strSubPath;
            RegistryKey hkey = GetHKey(strFullPath, out strSubPath);

            if (hkey == null)
                return false;

            return InsertKey(hkey, strSubPath);
        }

        public static bool InsertKey(HKeyType type, string strSubPath)
        {
            RegistryKey hkey = null;

            if (type == HKeyType.CLASSES_ROOT)
                hkey = Registry.ClassesRoot;
            else if (type == HKeyType.CURRENT_USER)
                hkey = Registry.CurrentUser;
            else if (type == HKeyType.LOCAL_MACHINE)
                hkey = Registry.LocalMachine;
            else if (type == HKeyType.USERS)
                hkey = Registry.Users;
            else if (type == HKeyType.CURRENT_CONFIG)
                hkey = Registry.CurrentConfig;
            else
            {
                return false;
            }

            return InsertKey(hkey, strSubPath);
        }

        private static bool SetValue(RegistryKey hkey, string strSubPath, string strValue)
        {
            int nIndex = strSubPath.LastIndexOf('\\');

            if (nIndex < 0)
                return false;

            string strSubKey = strSubPath.Substring(nIndex + 1);
            string strPath = strSubPath.Substring(0, nIndex);

            RegistryKey key = GetSubKey(hkey, strPath, true);

            if (key == null)
                return false;

            try
            {
                key.SetValue(strSubKey, strValue);
            }
            catch (Exception)
            {
                key.Close();
                return false;
            }

            key.Close();
            return true;
        }

        public static bool SetValue(string strFullPath, string strValue)
        {
            string strSubPath;
            RegistryKey hkey = GetHKey(strFullPath, out strSubPath);

            if (hkey == null)
                return false;

            return SetValue(hkey, strSubPath, strValue);
        }

        public static bool SetValue(HKeyType type, string strSubPath, string strValue)
        {
            RegistryKey hkey = null;

            if (type == HKeyType.CLASSES_ROOT)
                hkey = Registry.ClassesRoot;
            else if (type == HKeyType.CURRENT_USER)
                hkey = Registry.CurrentUser;
            else if (type == HKeyType.LOCAL_MACHINE)
                hkey = Registry.LocalMachine;
            else if (type == HKeyType.USERS)
                hkey = Registry.Users;
            else if (type == HKeyType.CURRENT_CONFIG)
                hkey = Registry.CurrentConfig;
            else
            {
                return false;
            }

            return SetValue(hkey, strSubPath, strValue);
        }
    }
}
