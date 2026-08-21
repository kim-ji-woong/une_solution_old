using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Pipes;

namespace Pipelib
{
    public delegate void OnDataRecive(PipeClientProvider provider, string cmd);
    
    public class PipeClientProvider : IDisposable
    {
        public void Dispose()
        {
            m_IoStream = null;
        }

        private System.IO.Stream m_IoStream = null;
        private UnicodeEncoding streamEncoding;
        private string m_szReadyString = "ReadyForCmd";

        public event OnDataRecive OnDataRecive;
        

        public PipeClientProvider(System.IO.Stream stream)
        {

            m_IoStream = stream;
            //m_IoStream.WriteTimeout = 1000;
           // m_IoStream.ReadTimeout = 2000;
            streamEncoding = new UnicodeEncoding();
        }

        private bool IsValidCmd(string szCmd)
        {
            return true;
        }

        public void ReadyCommand()
        {         
  
            WriteString(m_szReadyString);
            string szRead = ReadString();
            if (IsValidCmd(szRead))
            {
                PrcessCommand(szRead);
            }            
        }

        public void WriteCommand( string szCmd)
        {
            string szReady = ReadString();
            if(szReady == m_szReadyString)
            {
                WriteString(szCmd);
            }
        }

        public void PrcessCommand(string szCmd)
        {
            System.Diagnostics.Trace.WriteLine(szCmd);

            if (OnDataRecive != null)
                OnDataRecive(this, szCmd);
            
        }
        
        public string ReadString()
        {
            if (m_IoStream == null)
                return "";

            int len = 0;

            len = m_IoStream.ReadByte() * 256;
            if( len >= 0)
            {
                len += m_IoStream.ReadByte();
                byte[] inBuffer = new byte[len];
                m_IoStream.Read(inBuffer, 0, len);

                return streamEncoding.GetString(inBuffer);
            }
            return "";            
        }

        public int WriteString(string outString)
        {
            if (m_IoStream == null)
                return -1;

            byte[] outBuffer = streamEncoding.GetBytes(outString);
            int len = outBuffer.Length;
            if (len > UInt16.MaxValue)
            {
                len = (int)UInt16.MaxValue;
            }
            m_IoStream.WriteByte((byte)(len / 256));
            m_IoStream.WriteByte((byte)(len & 255));
            m_IoStream.Write(outBuffer, 0, len);
            m_IoStream.Flush();

            return outBuffer.Length + 2;
        }
    }
}
