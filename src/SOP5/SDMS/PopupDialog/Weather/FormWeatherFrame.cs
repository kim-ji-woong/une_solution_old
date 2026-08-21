using System;
using System.Windows.Forms;
using UnE.GUI;

namespace SDMS.WeatherDisplay
{
    public class FormWeatherFrame : UnE.GUI.FormNoFrameSizable
    {
        public FormWeatherFrame(Form frm)
            : base(frm)
        {
            this.Load += new EventHandler(FormFrame_Load);
            this.FormClosing += new FormClosingEventHandler(FormFrame_FormClosing);
        }

        private void FormFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!FormMain.Instance.CloseApplication)
            {
                e.Cancel = true;
                //this.Hide();

                ImageButton btnWeather = FormMain.Instance.GetButton(ID.ID_VIEW_WEATHER_INFO);

                if (btnWeather != null)
                    FormMain.Instance.PageHome.OnClickToolBarButton(btnWeather);
                else
                    this.Hide();
            }
            else if (m_frmMain != null)
            {
                m_frmMain.Visible = false;
                m_frmMain.Close();
            }
        }

        private void FormFrame_Load(object sender, EventArgs e)
        {
            this.ShowMaxButton = false;
            this.ShowMinButton = false;
            //this.ShowCloseButton = false;
            //this.Text = "";

            this.m_frmMain.Visible = true;

            this.TitleBarHeight = 30;
            this.Icon = FormMain.Instance.Icon;

            this.Size = new System.Drawing.Size(this.Size.Width, this.Size.Height + this.TitleBarHeight);
            this.ShowCloseButton = true;

            this.CloseButtonImage = global::SDMS.Properties.Resources.close_button;
            this.SystemButtonSize = this.CloseButtonImage.Size;

            this.Sizable = false;

            this.ResizeFrame();

            this.Text = "기후 정보";
        }

        public void UpdateData(DBUtility.WebDBManager dbMgr, int nSiteID)
        {
            if (m_frmMain != null)
            {
                FormWeatherDisplay frm = (FormWeatherDisplay)m_frmMain;
                frm.UpdateData(dbMgr, nSiteID);
            }
        }
    }
}
