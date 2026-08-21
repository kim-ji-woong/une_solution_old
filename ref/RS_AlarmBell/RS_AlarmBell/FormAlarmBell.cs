using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;
using System.Text;
using System.IO;

namespace RS_AlarmBell
{
    public partial class FormAlarmBell : Form
    {

        private event SerialDataReceivedEventHandler handler;
      
        public FormAlarmBell()
        {
            InitializeComponent();
            
            cmbPort.BeginUpdate();
            foreach (string comport in SerialPort.GetPortNames())
            {
                cmbPort.Items.Add(comport);
            }
            cmbPort.EndUpdate();
           
            SP.PortName = "COM1";
            SP.BaudRate = (int)9600;
            SP.DataBits = (int)8;
            SP.Parity = Parity.None;
            SP.StopBits = StopBits.One;
            SP.Handshake = Handshake.None;
            SP.RtsEnable = true;
           
        }

        private void SP_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort sp = (SerialPort)sender;
                lock (sp)
                {
                    if (sp.IsOpen && sp.BytesToRead > 0)
                    {
                        //usb converter로 연결해서인지 데이터 버퍼 수신이 이상함.
                        // string data = SP.ReadExisting();
                        int readable = sp.BytesToRead;
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        while (readable <= 19)
                        {
                            readable = sp.BytesToRead;
                            if (sw.ElapsedMilliseconds == 2000)
                            {
                                break;
                            }
                        }
                        Debug.WriteLine("Buffer Read Size = " + readable);
                        //if (readable < 19) throw new IOException("Data 수신 이상! - " + readable);

                        byte[] buffer = new byte[readable];
                        //There is no accurate method for checking how many bytes are read 
                        //unless you check the return from the Read method 

                        int bytesRead = sp.Read(buffer, 0, buffer.Length);


                        if (buffer[0] == 0x02)          //stx
                        {
                            ////For the example assume the data we are received is ASCII data. 
                            string keycode = Encoding.ASCII.GetString(buffer, 1, 2);

                            if (keycode.Equals("14"))        //1개의 벨 스위치일때. 현재 비상벨 스위치 타입
                            {
                                string displayID = Encoding.ASCII.GetString(buffer, 11, 3).Trim();
                                int displayNum = int.Parse(displayID);

                                String bellChipID = Encoding.ASCII.GetString(buffer, 4, 6);
                                Debug.WriteLine(bellChipID);
                                switch (displayNum)
                                {
                                    case 1:             //1번 비상벨, 문자???
                                        //if (bellChipID.Equals("5270C6"))      //등록한 비상벨의 chip ID 비교.
                                        //{

                                        //}
                                        break;

                                    case 2:             //2번 비상벨,316BFD

                                        break;

                                    case 3:             //3번 비상벨, C76576
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new IOException("Data 수신 이상! Not Open");
                    }
                }
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);  
                Thread.Sleep(1000);
            }
            finally
            {
                            
                if (handler != null)
                    this.SP.DataReceived -= handler;
                SP.Close();
                Thread.Sleep(500);    
                ConnectComPort();
            }
            
        }
        private void ConnectComPort()
        {
            handler = new System.IO.Ports.SerialDataReceivedEventHandler(this.SP_DataReceived);
            this.SP.DataReceived += handler;
            SP.Open();
            if (SP.IsOpen)
            {
                rbText.Text = string.Format("{0}{1}", rbText.Text, "\r\n[Success] Port Open!!");
                rbText.Text = "[" + SP.PortName.ToString() + "] Port Open Connect!!";
                lbStatus.Text = "Connect!!";
                btnOpen.Visible = false;
                btnPortClose.Visible = true;
            }
            else
            {
                rbText.Text = string.Format("{0}{1}", rbText.Text, "\r\n[Fail] Port Open!!");
                rbText.Text = "[" + SP.PortName.ToString() + "] Port Open Failed!";
                lbStatus.Text = "[Fail] Port Open!";
                lbStatus.ForeColor = Color.Red;
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            ConnectComPort();
        }

        private void cmbPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            SP.PortName = cmbPort.SelectedItem.ToString();
        }

        private void cmbBRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbBRate.SelectedIndex)
            {
                case 0 :
                    SP.BaudRate = (int)9600;
                    break;
                case 1:
                    SP.BaudRate = (int)14400;
                    break;
                case 2:
                    SP.BaudRate = (int)19200;
                    break;
                case 3:
                    SP.BaudRate = (int)38400;
                    break;
                case 4:
                    SP.BaudRate = (int)57600;
                    break;
                case 5:
                    SP.BaudRate = (int)115200;
                    break;
                default:
                    SP.BaudRate = (int)19200;
                    break;
            }
        }

        private void cmbDataBits_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbDataBits.SelectedIndex)
            {
                case 0:
                    SP.DataBits = 8;
                    break;
                case 1:
                    SP.DataBits = 7;
                    break;
                default :
                    SP.DataBits = 8;
                    break;
            }
        }

        private void cmbParity_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbParity.SelectedIndex)
            {
                case 0:
                    SP.Parity = Parity.Even;
                    break;
                case 1:
                    SP.Parity = Parity.Mark;
                    break;
                case 2:
                    SP.Parity = Parity.None;
                    break;
                case 3:
                    SP.Parity = Parity.Odd;
                    break;
                case 4:
                    SP.Parity = Parity.Space;
                    break;
                default:
                    SP.Parity = Parity.None;
                    break;
            }
        }

        private void cmbStopBits_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbStopBits.SelectedIndex)
            {
                case 0:
                    //SP.StopBits = StopBits.None;
                    MessageBox.Show("이 값은 지원되지 않습니다");
                    break;
                case 1:
                    SP.StopBits = StopBits.One;
                    break;
                case 2:
                    SP.StopBits = StopBits.OnePointFive;
                    break;
                case 3:
                    SP.StopBits = StopBits.Two;
                    break;
                default:
                    SP.StopBits = StopBits.One;
                    break;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
        }

        private void btnPortClose_Click(object sender, EventArgs e)
        {
            SP.Close();
            rbText.Text += "\r\n" + "[" + SP.PortName.ToString() + "] Port Close!!";
            lbStatus.Text = "Not Connect!!";
            btnOpen.Visible = true;
            btnPortClose.Visible = false;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (SP.IsOpen)
            {
                SP.Close();
            }
        }
    }
}
