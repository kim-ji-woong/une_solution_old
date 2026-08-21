using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PushServer
{
    public class PushLog
    {
        private static string m_fileName = @"C:\Work\UnE\src\KPX\PushServer\PushServer\bin\Debug\LOG_2017_11_07.log";
        private static StreamWriter m_writer = null;

        public static void Close()
        {
            try
            {
                if (m_writer != null)
                {
                    m_writer.Close();
                    m_writer = null;
                }
            }
            catch(Exception)
            {

            }
        }

        public static bool Write(string str)
        {
            try
            {
                DateTime dtNow = DateTime.Now;
                string strDay = string.Format("{0}_{1:00}_{2:00}", dtNow.Year, dtNow.Month, dtNow.Day);
                string path = System.Windows.Forms.Application.StartupPath + "\\LOG_" + strDay + ".log";
                if(m_fileName != path)
                {
                    if (m_fileName != "")
                    {
                        Close();
                        System.IO.File.Delete(m_fileName);
                    }

                    if (!Open(path))
                    {
                        return false;
                    }
                }

                string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00} : ", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);
                m_writer.Write(strTime);

                m_writer.WriteLine(str);
                m_writer.Flush();
            }
            catch (IOException)
            {
                return false;
            }

            return true;
        }

        private static bool Open(string strPath)
        {
            try
            {
                m_writer = new StreamWriter(strPath, false, Encoding.UTF8);
            }
            catch (IOException)
            {
                m_writer = null;
                return false;
            }
            m_fileName = strPath;

            return true;
        }
    }
}
