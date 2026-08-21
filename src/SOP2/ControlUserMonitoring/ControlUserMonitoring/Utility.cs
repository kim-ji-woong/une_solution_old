using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Reflection;
using System.IO;

namespace ControlMonitoring
{
    class Utility
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

        public string getinivalue(string section, string key)
        {
            StringBuilder temp = new StringBuilder(255);

            string szPath = Assembly.GetEntryAssembly().Location;
            string szFullPath = Directory.GetParent(szPath).FullName;

            string strPath = szFullPath + "\\config.ini";
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
    }
}
