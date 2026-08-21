using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PreSafe
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            FormLoginMain frmLoginMain = new FormLoginMain();
            frmLoginMain.Size = new System.Drawing.Size(600, 329);
            frmLoginMain.StartPosition = FormStartPosition.CenterScreen;

            if (frmLoginMain.ShowDialog() == DialogResult.Cancel)
            {
                Application.Exit();
                return;
            }


            FormMain formMain = new FormMain();
            FormFrame frame = new FormFrame(formMain, 1);
            Application.Run(frame);
        }
    }
}
