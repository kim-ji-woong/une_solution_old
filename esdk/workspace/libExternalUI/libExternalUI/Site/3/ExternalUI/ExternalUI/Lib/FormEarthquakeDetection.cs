using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility2;
using UnE.Earthquake;

namespace libExternalUI.Lib
{
    public partial class FormEarthquakeDetection : Form
    {
        public FormEarthquakeDetection()
        {
            InitializeComponent();

            this.TopLevel = false;

            btnClose.ImageNormal = global::libExternalUI.Properties.Resources.close_Normal;
            btnClose.ImageClicked = global::libExternalUI.Properties.Resources.close_Click;
            btnClose.ImageMouseOver = global::libExternalUI.Properties.Resources.close_MouseOver;

            btnConfig.ImageNormal = global::libExternalUI.Properties.Resources.config_Normal;
            btnConfig.ImageClicked = global::libExternalUI.Properties.Resources.config_Click;
            btnConfig.ImageMouseOver = global::libExternalUI.Properties.Resources.config_MouseOver;
        }

        private void FormEarthquakeDetection_Load(object sender, EventArgs e)
        {
            
        }

        private int m_nPrevData = -1;
        public void DisplayEarthquake()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Select data From SensorZone Where ID = 223 ");

            ArrayList arrResult = UIManager.Instance.DBMgr.GetResultData(sb.ToString());
            if (arrResult == null || arrResult.Count == 0)
                return;

            int nData = DBUtility2.WebDBManager.GetIntField(arrResult[0].ToString(), -999);
            
            if (m_nPrevData != nData)
            {
                m_nPrevData = nData;
                if (nData == -999)
                {
                    plRank.BackColor = Color.FromArgb(0xbb, 0xbb, 0xbb);
                    lblConnError.Visible = true;
                    return;
                }
                else
                    lblConnError.Visible = false;

                List<EarthquakeOption> options = LoadOptions();
                EarthquakeOption option = EarthquakeOption.GetOption(nData, -1, options);
                if (option == null)
                    plRank.BackColor = Color.FromArgb(0xbb, 0xbb, 0xbb);
                else
                {
                    int nAlarmDepth = options.IndexOf(option) + 1;
                    
                    if (nAlarmDepth == 1)
                        plRank.BackColor = Color.FromArgb(0xff, 0xb8, 0x44);
                    else if (nAlarmDepth == 2)
                        plRank.BackColor = Color.FromArgb(0xdd, 0x92, 0x19);
                    else if (nAlarmDepth == 3)
                        plRank.BackColor = Color.FromArgb(0xe4, 0x47, 0x47);
                }

                lbData.Text = nData.ToString();
            }
        }
                
        private List<EarthquakeOption> LoadOptions()
        {
            string strSQL = "Select MinIntens, MaxIntens, IntensOption, UseSMS, SMSMessage, UseBroadcast, BroadcastMessage, RunSOP, LinkedSOP from OptionEarthquake";
            ArrayList arrResult = UIManager.Instance.DBMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            List<EarthquakeOption> options = new List<EarthquakeOption>();
            int nResultData = arrResult.Count;

            for (int i = 0; i < nResultData - 8; i += 9)
            {
                VariousData<float> min = WebDBManager.GetFloatField(arrResult[i].ToString());
                VariousData<float> max = WebDBManager.GetFloatField(arrResult[i + 1].ToString());
                VariousData<int> option = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<int> useSMS = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                string strSMS = WebDBManager.GetStringField(arrResult[i + 4]);
                VariousData<int> useBroadcast = WebDBManager.GetIntField(arrResult[i + 5].ToString());
                string strBroadcast = WebDBManager.GetStringField(arrResult[i + 6]);
                VariousData<int> runSOP = WebDBManager.GetIntField(arrResult[i + 7].ToString());
                string strLinkedSOP = WebDBManager.GetStringField(arrResult[i + 8]);

                if (min == null || max == null || option == null || useSMS == null || useBroadcast == null || runSOP == null)
                    continue;

                EarthquakeOption opt = new EarthquakeOption();
                opt.Minimum = min.Data;
                opt.Maximum = max.Data;
                opt.SetMinMaxOption(option.Data);
                opt.UseSMS = useSMS.Data == 1 ? true : false;
                opt.SMSMessage = strSMS == null ? "" : strSMS;
                opt.UseBroadcast = useBroadcast.Data == 1 ? true : false;
                opt.BroadcastMessage = strBroadcast == null ? "" : strBroadcast;
                opt.RunSOP = runSOP.Data == 1 ? true : false;
                opt.LinkedSOP = strLinkedSOP == null ? "" : strLinkedSOP;

                options.Add(opt);
            }

            options.Sort();
            return options;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            //SDMS.FormMain.TransferExternalForm((int)1);
            libExternalUI.Lib.UIManager.TransferExternalForm((int)1);
            this.Hide();
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            SDMS.PopupDialog.FormEarthquakeOption frm = new SDMS.PopupDialog.FormEarthquakeOption(UIManager.Instance.DBMgr);
            SDMS.PageBackstageHome.ShowTranslucentForm(frm, 200, 200, frm.Size.Width, frm.Size.Height, SDMS.ID.ID_MANAGE_EARTHQUAKE);
        }
    }
}
