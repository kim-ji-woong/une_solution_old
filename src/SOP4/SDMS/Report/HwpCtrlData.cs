using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;

namespace SDMS.Report
{
    class HwpCtrlData
    {
        //보안모듈 등록
        private const string RegRoot = @"SoftWare\HNC\HwpCtrl\Modules";
        
		public void SetRegistry()
        {
            string FilePath = Application.StartupPath + @"\FilePathCheckerModule.dll";

            RegistryKey R = Registry.CurrentUser.OpenSubKey(RegRoot, true);
            if(R == null)
                R = Registry.CurrentUser.CreateSubKey(RegRoot);

			R.SetValue("FilePathCheckerModule", FilePath);

            R.Close();
        }

        public bool GetRegistry(ref string strHWPPath)
        {
            const string HwpRoot = @"Applications\Hwp.exe";

            RegistryKey R = Registry.ClassesRoot.OpenSubKey(HwpRoot);

            if (R == null)
                return false;

            if (strHWPPath != null)
                return true;

            strHWPPath = "";

            RegistryKey shell = R.OpenSubKey("shell");

            if (shell == null)
                return false;

            RegistryKey open = shell.OpenSubKey("open");

            if (open == null)
                return false;

            RegistryKey command = open.OpenSubKey("command");

            if (command == null)
                return false;

            string[] names = command.GetValueNames();

            if (names == null || names.Count() == 0)
                return false;

            object value = command.GetValue(names[0]);

            if (value == null)
                return false;

            string strValue = value.ToString();

            if (strValue[0] == '\"')
            {
                int nIndex = strValue.IndexOf('\"', 1);

                if (nIndex < 0)
                    return false;

                strHWPPath = strValue.Substring(1, nIndex - 1);
            }
            else
            {
                int nIndex = strValue.IndexOf(' ', 0);

                if (nIndex < 0)
                    return false;

                strHWPPath = strValue.Substring(0, nIndex);
            }

            /*int nIndex1 = strValue.IndexOf('\"');

            if (nIndex1 < 0)
                return true;

            int nIndex2 = strValue.IndexOf('\"', nIndex1 + 1);

            if (nIndex2 < 0)
                return true;

            string strPath = strValue.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
            strHWPPath = strPath;*/

            return true;
        }

    }
}
