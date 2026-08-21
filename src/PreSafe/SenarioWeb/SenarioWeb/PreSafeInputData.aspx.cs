using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SenarioWeb
{
    public partial class PreSafeInputData : System.Web.UI.Page
    {
        private int m_nLocation = 0;
        private int m_nHeartBeat = 0;
        private int m_nAcc = 0;
        private int m_nAlcohol = 0;
        private int m_nSound = 0;
        private int m_nImpact = 0;

        private bool m_bUseLocation = false;
        private bool m_bUseHeartRate = false;
        private bool m_bUseAcc = false;
        private bool m_bUseAlcohol = false;
        private bool m_bUseScream = false;
        private bool m_bUseImpact = false;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack == false)
            {
            }
        }


        protected void btnSave_Click(object sender, EventArgs e)
        {
            using (SenarioWeb.SenarioService.PreSafe service = new SenarioWeb.SenarioService.PreSafe())
            {
                int.TryParse(txtHeartRate.Text, out m_nHeartBeat);

                m_nLocation = cmbLocation.SelectedIndex;
                m_nAcc = cmbAcc.SelectedIndex;
                m_nAlcohol = cmbAlcohol.SelectedIndex;
                m_nSound = cmbScream.SelectedIndex;
                m_nImpact = cmbImpact.SelectedIndex;

                bool bResult = service.SaveSenarioData(
                    "UNES", 1,
                    m_bUseLocation, m_nLocation,
                    m_bUseHeartRate, m_nHeartBeat,
                    m_bUseAcc, m_nAcc,
                    m_bUseAlcohol, m_nAlcohol,
                    m_bUseScream, m_nSound,
                    m_bUseImpact, m_nImpact,
                    "");
            }

            Response.Redirect(@"./PreSafe.aspx");
        }


        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"./PreSafe.aspx");
        }

        #region Check Box Event

        protected void chkLocation_CheckedChanged(object sender, EventArgs e)
        {
            m_bUseLocation = chkLocation.Checked;
        }

        protected void chkHeartRate_CheckedChanged(object sender, EventArgs e)
        {
            m_bUseHeartRate = chkHeartRate.Checked;

        }

        protected void chkAcc_CheckedChanged(object sender, EventArgs e)
        {
            m_bUseAcc = chkAcc.Checked;
        }

        protected void chkAlcohol_CheckedChanged(object sender, EventArgs e)
        {
            m_bUseAlcohol = chkAlcohol.Checked;
        }

        protected void chkScream_CheckedChanged(object sender, EventArgs e)
        {
            m_bUseScream = chkScream.Checked;
        }

        protected void chkImpact_CheckedChanged(object sender, EventArgs e)
        {
            m_bUseImpact = chkImpact.Checked;
        }

        #endregion Check Box Event


    }
}