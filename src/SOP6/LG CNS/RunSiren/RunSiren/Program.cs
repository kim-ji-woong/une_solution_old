using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RunSiren
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string strSirenFile = "66084^air-raid-siren-alert.mp3";
            libTTS.SoundUtils.PlaySound(strSirenFile, false);

            System.Threading.Thread.Sleep(10000);
        }
    }
}
