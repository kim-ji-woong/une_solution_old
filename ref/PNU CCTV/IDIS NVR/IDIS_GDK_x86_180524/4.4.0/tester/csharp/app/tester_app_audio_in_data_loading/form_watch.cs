using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using GDK;

namespace GDK_tester
{
    public partial class form_watch : Form
    {
        public form_watch(string addr, string user_id, string pw, ushort port)
        {
            InitializeComponent();
            init_connective_watch();
            connect(addr, user_id, pw, port);
            BTN_SAVE.Enabled = false;
        }
        public void connect(string addr, string user_id, string pw, ushort port)
        {
            G2NETWORK_INFO ni = new G2NETWORK_INFO();
            ni._address = addr;
            ni._address_type = G2NETWORK_INFO.ADDRESS_TYPE.IPV4;
            ni._command_protocol_type = G2NETWORK_INFO.COMMAND_PROTOCOL_TYPE.IDIS;
            ni._media_record_protocol_type = G2NETWORK_INFO.MEDIA_STREAM_PROTOCOL_TYPE.IDIS;
            ni._media_search_protocol_type = G2NETWORK_INFO.MEDIA_STREAM_PROTOCOL_TYPE.IDIS;
            ni._media_stream_protocol_type = G2NETWORK_INFO.MEDIA_STREAM_PROTOCOL_TYPE.IDIS;
            ni.set_port(G2NETWORK_INFO.PORT_TYPE.WATCH_PORT, port);            
            ni._user_id = user_id;
            ni._password = pw;
            G2CONNECT_RES res;
            int channel = _adaptor.connect_ras(ref ni, out res);
            if (valid_channel(channel))
            {
                _channel = channel;
            }
        }
        private void BTN_FILE_PATH_Click(object sender, EventArgs e)
        {
            if (_buf == null) return;
            string file_name = "";
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Please specify a file path.";
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.Filter = "wav (*.wav)|*.wav|All files (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                file_name = saveFileDialog.FileName;
                this.LBL_PATH.Text = file_name;
                _buf.set_file_path(file_name);
            }
        }

        private void BTN_SAVE_Click(object sender, EventArgs e)
        {
            if (_buf == null || _buf._wg == null) return;
            if (LBL_PATH.Text == "")
            {
                MessageBox.Show("Set the File Path");
                return;
            }
            if (TXT_DURATION.Text == "")
            {
                MessageBox.Show("Set the Duration");
                return;
            }
            int duration = Convert.ToInt16(TXT_DURATION.Text);

            _buf._wg._is_data_saving = true;
            Thread.Sleep(duration * 1000);
            _buf._wg._is_data_saving = false;

            _buf.save_file();
        }

        private void form_watch_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_buf._wg != null)
            {
                _buf._wg._is_data_saving = false;
            }
            this.Hide();
            //Console.ReadKey(true);
            File.Delete(@"C:\temp.dat");
            _adaptor.disconnect(_channel);
            _adaptor.cleanup();
        }
    }
}