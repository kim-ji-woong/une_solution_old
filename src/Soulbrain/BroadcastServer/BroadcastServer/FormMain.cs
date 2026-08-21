using System;
using System.Configuration;
using System.Windows.Forms;

namespace BroadcastServer
{
    using Network;

    public partial class FormMain : Form, IServiceOwner
    {
        private BroadcastManager m_manager = null;
        private ServiceTemp m_service = null;

        public FormMain()
        {
            InitializeComponent();

            int? port = GetServerPort();

            if (port != null)
            {
#if !UseServiceTemp
                m_manager = new BroadcastManager(this, (int)port);
#endif
            }

#if UseServiceTemp
            m_service = new ServiceTemp(this);
#endif
        }

        private int? GetServerPort()
        {
            string strPort = ConfigurationManager.AppSettings.Get("port");

            if (strPort == null || strPort.Length == 0)
                return null;

            int nPort;
            if (int.TryParse(strPort.Trim(), out nPort) == false)
                return null;

            return nPort;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            cboCommandType.SelectedIndex = 0;
            cboMaterialType.SelectedIndex = 0;
            cboAlarmLevel.SelectedIndex = 0;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (m_manager != null)
            {
                BroadcastManager.CommandType cmd = (BroadcastManager.CommandType)(cboCommandType.SelectedIndex + 1);
                //BroadcastManager.MaterialType material = (BroadcastManager.MaterialType)(cboMaterialType.SelectedIndex + 1);
                BroadcastManager.AlarmLevel alarmLevel;

                if (cboAlarmLevel.SelectedIndex == 0)
                    alarmLevel = BroadcastManager.AlarmLevel.ClearAlarm;
                else
                    alarmLevel = (BroadcastManager.AlarmLevel)(cboAlarmLevel.SelectedIndex + 1);

                //if (alarmLevel != BroadcastManager.AlarmLevel.ClearAlarm)
                //{
                //    // 모든 타입에 대하여 알람해제를 시킨다.
                //    /*foreach (BroadcastManager.MaterialType materialType in Enum.GetValues(typeof(BroadcastManager.MaterialType)))
                //    {
                //        m_manager.SendMessage(cmd, materialType, BroadcastManager.AlarmLevel.ClearAlarm);
                //        System.Threading.Thread.Sleep(500);
                //    }*/

                //    /*m_manager.SendMessage(cmd, material, BroadcastManager.AlarmLevel.ClearAlarm);
                //    System.Threading.Thread.Sleep(500);*/
                //    m_manager.SendMessage(cmd, material, alarmLevel);
                //}
                //else
                {
                    if (cmd == BroadcastManager.CommandType.CMD_PSM)
                    {
                        BroadcastManager.MaterialType material = (BroadcastManager.MaterialType)(cboMaterialType.SelectedIndex + 1);
                        m_manager.SendMessage(cmd, (int)material, alarmLevel);
                    }
                    else
                    {
                        BroadcastManager.FireType fire = (BroadcastManager.FireType)(cboMaterialType.SelectedIndex + 1);
                        m_manager.SendMessage(cmd, (int)fire, alarmLevel);
                    }
                }
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //m_manager.Close();
        }

        private void cboCommandType_SelectedIndexChanged(object sender, EventArgs e)
        {
            BroadcastManager.CommandType cmd = (BroadcastManager.CommandType)(cboCommandType.SelectedIndex + 1);

            if (cmd == BroadcastManager.CommandType.CMD_PSM)
            {
                cboMaterialType.Items.Clear();

                cboMaterialType.Items.Add("불산");
                cboMaterialType.Items.Add("염산");
                cboMaterialType.Items.Add("Co");
                cboMaterialType.Items.Add("Co2");
                cboMaterialType.Items.Add("Tvoc");
                cboMaterialType.Items.Add("O2");
            }
            else
            {
                cboMaterialType.Items.Clear();

                cboMaterialType.Items.Add("화재");
            }

            cboMaterialType.SelectedIndex = 0;
        }

        public void OnAccept(string strConnectionInfo)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.labelConnection.Text = "클라이언트 접속중 : " + strConnectionInfo;
            });

            Logger.Instance.Write("클라이언트 접속(" + strConnectionInfo + ")");
        }

        public void OnDropConnection(string strConnectionInfo)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.labelConnection.Text = "접속된 클라이언트 없음";
            });

            Logger.Instance.Write("클라이언트 접속종료(" + strConnectionInfo + ")");
        }
    }
}
