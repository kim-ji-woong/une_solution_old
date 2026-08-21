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

        public bool GetRegistry()
        {
            const string HwpRoot = @"Applications\Hwp.exe";

            RegistryKey R = Registry.ClassesRoot.OpenSubKey(HwpRoot);

            if (R == null)
                return false;

            return true;
        }

    }
}
