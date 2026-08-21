using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTSSimulator
{
    public class Broadcaster
    {
        public static void Run(string strServerName, string strPort, bool isSiren, int nRepeat, string strMessage, string strResultFilePath)
        {
            try
            {
                using (libTTS.Broadcast br = new libTTS.Broadcast(strServerName, strPort))
                {
                    br.AddSpeech(strMessage, nRepeat, isSiren);
                }

                if (strResultFilePath.Length > 0)
                {
                    System.IO.StreamWriter writer = new System.IO.StreamWriter(strResultFilePath);
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                if (strResultFilePath.Length > 0)
                {
                    System.IO.StreamWriter writer = new System.IO.StreamWriter(strResultFilePath);
                    writer.Write(ex.Message);
                    writer.Close();
                }
            }
        }
    }
}
