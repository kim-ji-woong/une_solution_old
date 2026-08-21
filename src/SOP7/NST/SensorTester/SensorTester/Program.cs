using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoulbrainSensorTester
{
    static class Program
    {
        public static IFormMain MainForm = null;

        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainForm = new FormMain2();
            Application.Run((Form)MainForm);
        }
    }

    public interface IFormMain
    {

        void reloadGrid();
    }
}
