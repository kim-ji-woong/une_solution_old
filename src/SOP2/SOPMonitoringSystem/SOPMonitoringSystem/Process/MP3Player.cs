using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SOPMonitoringSystem.Process
{
    using System.Runtime.InteropServices;
    delegate long mciSendStringDelegate(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);

    public class MP3Player
    {
        private string _command;
        private bool isOpen;
        [DllImport("winmm.dll")]

        private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);

        public void Close(Form frm)
        {
            _command = "close MediaFile";
            //mciSendString(_command, null, 0, IntPtr.Zero);
            isOpen = false;

            mciSendStringDelegate mci = new mciSendStringDelegate(mciSendString);
            object[] obj = new object[] { _command, null, 0, IntPtr.Zero };

            if (!FormMain.Instance.CloseThread)
                frm.Invoke(mci, obj);
        }

        public bool Open(string sFileName, Form frm)
        {
            if (!System.IO.File.Exists(sFileName))
                return false;

            _command = "open \"" + sFileName + "\" type mpegvideo alias MediaFile";
            //mciSendString(_command, null, 0, IntPtr.Zero);
           isOpen = true;

           mciSendStringDelegate mci = new mciSendStringDelegate(mciSendString);
           object[] obj = new object[] { _command, null, 0, IntPtr.Zero };

           if (!FormMain.Instance.CloseThread)
                frm.Invoke(mci, obj);

           return true;
        }

        public void Play(bool loop, Form frm, IntPtr hwndNotify)
        {
            if (isOpen)
            {
                _command = "play MediaFile wait";
                if (loop)
                    _command += " REPEAT";
                //mciSendString(_command, null, 0, IntPtr.Zero);

                mciSendStringDelegate mci = new mciSendStringDelegate(mciSendString);
                object[] obj = new object[] { _command, null, 0, hwndNotify };

                if (!FormMain.Instance.CloseThread)
                    frm.Invoke(mci, obj);
            }
        }

        public static void AutoPlay(string strPath)
        {
            string cmd = "open \"" + strPath + "\" type mpegvideo alias MediaFile";
            mciSendString(cmd, null, 0, IntPtr.Zero);

            System.Threading.Thread.Sleep(300);

            cmd = "play MediaFile";
            mciSendString(cmd, null, 0, IntPtr.Zero);
        }
    }
}
