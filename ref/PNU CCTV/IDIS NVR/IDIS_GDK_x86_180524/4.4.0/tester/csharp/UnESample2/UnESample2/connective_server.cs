using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDK;
using GDK_tester;

namespace UnESample2
{
    using G2HANDLE = System.Int32;

    public partial class FormMain : g2app_frame_relay_server_listener
    {
        private int m_nSearchControllerProcessID = -1;

        public void init_connective_server()
        {
            _server = new g2app_frame_relay_server();
            _server.set_listener(this);
            _option._connections = 1;
            _option._iocp = true;
            _option._port = 10000;
            _option._realloc = true;
            _option._send_queue_size = 64 * 1024;
            _option._threads = 1;
            _option._tick_out = Int32.MaxValue;
            _server.startup(ref _option);
        }

        public void on_g2app_frame_relay_server_connected(G2HANDLE handle, int channel)
        {
            int nIndex = m_strOptionFileName.LastIndexOf('.');

            if (nIndex >= 0)
            {
                string strResultFileName = m_strOptionFileName.Substring(0, nIndex) + ".result";
                m_nSearchControllerProcessID = GetSearchControllerProcessID(strResultFileName);
                System.IO.File.Delete(strResultFileName);

                System.Diagnostics.Trace.WriteLine("OnConnect : " + m_nSearchControllerProcessID.ToString());
            }
            else
                m_nSearchControllerProcessID = -1;
        }

        private int GetSearchControllerProcessID(string strPath)
        {
            try
            {
                // 파일이 생성될때까지 최대 1초간 기다린다.
                int nCount = 0;
                while (System.IO.File.Exists(strPath) == false)
                {
                    System.Threading.Thread.Sleep(10);

                    if (++nCount >= 100)
                        return -1;
                }

                System.IO.FileStream stream = new System.IO.FileStream(strPath, System.IO.FileMode.Open);
                System.IO.BinaryReader reader = new System.IO.BinaryReader(stream);

                int nID = reader.ReadInt32();

                reader.Close();
                stream.Close();
                return nID;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }

            return -1;
        }

        public void on_g2app_frame_relay_server_disconnected(G2HANDLE handle, int channel, G2DISCONNECT_REASON.TYPE reason)
        {
            _screen.buf_reset(channel);
            _screen.reset(channel, true);
            _screen.set_pane_status(channel, camera_pane.STATUS.NOT_CONNECTED, true);

            if (m_nSearchControllerProcessID >= 0)
            {
                try
                {
                    System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById(m_nSearchControllerProcessID);

                    if (process != null)
                        process.Kill();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                }

                System.Diagnostics.Trace.WriteLine("OnDisconnect : " + m_nSearchControllerProcessID.ToString());
                m_nSearchControllerProcessID = -1;
            }

            System.Diagnostics.Trace.WriteLine("OnDisconnect NULL");
        }

        public void on_g2app_frame_relay_server_receive_site_connected(G2HANDLE handle, int channel, ref G2STRING_64 site)
        {
            _screen.reset(channel, true);
            _screen.set_pane_mode(channel, camera_pane.MODE.PLAY);
            _screen.set_pane_status(channel, camera_pane.STATUS.ENABLE, true);
            _screen.buf_setup(channel, 1, frame_buf.TYPE.LIVE_SIMPLE);
        }
        public void on_g2app_frame_relay_server_receive_site_disconnected(G2HANDLE handle, int channel, ref G2STRING_64 site, int reason)
        {
            _screen.buf_reset(channel);
            _screen.reset(channel, true);
            _screen.set_pane_status(channel, camera_pane.STATUS.NOT_CONNECTED, true);
        }
        public void on_g2app_frame_relay_server_receive_site_product_info(G2HANDLE handle, int channel, ref G2STRING_64 site, ref G2_PRODUCT_INFO pi) { }
        public void on_g2app_frame_relay_server_receive_site_frame_data(G2HANDLE handle, int channel, ref G2STRING_64 site, ref G2FRAME frame)
        {
            _screen.put_frame(ref frame, channel, channel);
        }
    }
}
