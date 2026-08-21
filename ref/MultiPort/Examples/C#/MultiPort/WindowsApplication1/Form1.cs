using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using log4net;

namespace WindowsApplication1
{ 
    public partial class Form1 : Form
    {
        private static log4net.ILog logger = null;

        [DllImport("IPSerial.dll")]
        public static extern int nsio_init();
        [DllImport("IPSerial.dll")]
        public static extern int nsio_end();
        [DllImport("IPSerial.dll")]
        public static extern int nsio_open(string server_ip, int port_index, int timeouts);
        [DllImport("IPSerial.dll")]
        public static extern int nsio_close(int port_id);
        [DllImport("IPSerial.dll")]
        public static extern int nsio_ioctl(int port_id, int baud, int mode);
        [DllImport("IPSerial.dll")]
        public static extern int nsio_flowctrl(int port_id, int mode);
        [DllImport("IPSerial.dll")]
        public static extern int nsio_write(int port_id, string buf, int len);
        [DllImport("IPSerial.dll")]
        public static extern int nsio_read(int port_id, byte[] buf, int len);
        [DllImport("IPSerial.dll")]
        public static extern int nsio_data_status(int port_id);
        int PID1, PID2;
        int NSIO_OK = 0;
        
        public Form1()
        {
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            InitializeComponent();
            PID1 = -1;
            PID2 = -1;
            nsio_init();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            byte[] buf1 = new byte[16];
            byte[] buf2 = new byte[16];
            string tmp;
            int ret;            
            
            ret = nsio_read(PID2, buf2, 16);
            if (ret > 0)
            {
                if (buf2[ret - 1] == 0x03)
                {
                    //02 50 4F 4C 03 00 00 00 00 00 00 00 00 00 00 00 - poll
                    StringBuilder sb = new StringBuilder();                   
                    sb.Append((char)0x06);
                    string sz = sb.ToString();
                    logger.Debug("[" + ip2.Text + "][SEDN BIN : " + sz);
                    Debug.WriteLine("SEDN BIN : " + sz);
                    nsio_write(PID2, sz, sz.Length);
                    tp2.Text = tp2.Text + sz + "\r\n";
                }

                tmp = "";
                for (int j = 0; j < ret; j++)// (byte b in buf2)
                {
                    byte b = buf2[j];
                    if (tmp.Length == 0)
                        tmp = string.Format("{0:X2}", (int)b);
                    else
                        tmp += string.Format(" {0:X2}", (int)b);
                }
                string tmp2 = System.Text.Encoding.ASCII.GetString(buf2);
                logger.Debug("[" + ip2.Text + "][RECIVED TXT] : " + tmp2);
                logger.Debug("[" + ip2.Text + "][RECIVED TXT] : " + tmp);
                Debug.WriteLine("RECIVED TXT : " + tmp2);
                rp2.Text = rp2.Text + tmp2 + "\r\n";
            }
        }

        private void b_exit_Click(object sender, EventArgs e)
        {
            if (PID1 >= 0)
            {
                nsio_close(PID1);
            }
            if (PID2 >= 0)
            {
                nsio_close(PID2);
            }
            nsio_end();
            Close();
        }

        private void b_close_Click(object sender, EventArgs e)
        {
            int ret=0;
            if (nsio_close(PID1) != NSIO_OK)
            {
                ret = 1;
                MessageBox.Show("Close port One fail!");
            }
            else
            {
                PID1 = -1;
            }
            if (nsio_close(PID2) != NSIO_OK)
            {
                ret = 1;
                MessageBox.Show("Close port Two fail!");
            }
            else
            {
                PID2 = -1;
            }
            if (ret != 1)
                MessageBox.Show("Close port OK!!");
            ip1.Enabled = true;
            ip2.Enabled = true;
            port1.Enabled = true;
            port2.Enabled = true;
            tp1.Text = "";
            tp2.Text = "";
            rp1.Text = "";
            rp2.Text = "";
            tp1.Enabled = false;
            tp2.Enabled = false;
            rp1.Enabled = false;
            rp2.Enabled = false;
            b_close.Enabled = false;
            b_open.Enabled = true;
            timer1.Enabled = false;
        }

        private void b_open_Click(object sender, EventArgs e)
        {
            int num;
            num = Convert.ToInt32(port1.Text);
           // PID1 = nsio_open(ip1.Text, num, 3000);
           // if (PID1 < NSIO_OK)
           // {
           //     MessageBox.Show("Open port One fail!");
           //     return;
           // }
            num = Convert.ToInt32(port2.Text);
            PID2 = nsio_open(ip2.Text, num, 3000);
            if (PID2 < NSIO_OK)
            {
               // nsio_close(PID1);
                MessageBox.Show("Open port Two fail!");
                return;
            }
            MessageBox.Show("Open port OK!");
            ip1.Enabled = false;
            ip2.Enabled = false;
            port1.Enabled = false;
            port2.Enabled = false;
            tp1.Enabled = true;
            tp2.Enabled = true;
            rp1.Enabled = true;
            rp2.Enabled = true;
            b_close.Enabled = true;
            b_open.Enabled = false;
            timer1.Enabled = true;

            int ret;
            // ret = nsio_ioctl(PID1, B9600, 0x03);         // baudrate 38400, N81
            //if(ret < NSIO_OK)
           // {
           //     MessageBox.Show("Open port One IO control settings fail!");
           // }
          //  ret = nsio_flowctrl(PID1, 0x03);          // HW flow control
          //  if (ret < NSIO_OK)
           // {
          //      MessageBox.Show("Open port One flow control settings fail!");
           // }

            ret = nsio_ioctl(PID2, 12, 0x03);         // baudrate 38400, N81
            if (ret < NSIO_OK)
            {
                MessageBox.Show("Open port Two IO control settings fail!");
            }
            ret = nsio_flowctrl(PID2, 1 | 0);          // HW flow control
            if (ret < NSIO_OK)
            {
                MessageBox.Show("Open port Two flow control settings fail!");
            }

            int nDI= nsio_data_status(PID2);
            Debug.WriteLine(nDI);
        }

        private void tp1_KeyPress(object sender, KeyPressEventArgs e)
        {

            //int ret;
            //string tmp = "";

            //tmp = e.KeyChar.ToString();
            //ret = nsio_write(PID1, tmp, tmp.Length);
            //if(ret < NSIO_OK)
            //{
            //    MessageBox.Show("Write error may connection is closed!");
            //}

        }

        private void tp2_KeyPress(object sender, KeyPressEventArgs e)
        {
            //int ret;
            //string tmp = "";
            //tmp = e.KeyChar.ToString();
            //ret = nsio_write(PID1, tmp, tmp.Length);
            //if (ret < NSIO_OK)
            //{
            //    MessageBox.Show("Write error may connection is closed!");
            //}
        }


    }
}