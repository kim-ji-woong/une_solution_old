using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services.Description;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SenarioWeb
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SenarioWeb.SenarioService.PreSafe service = new SenarioWeb.SenarioService.PreSafe();
            //string[] szResult = service.SenarioList();
            //int i = 0;
            //i++;

            string[] szResult = { "test1", "test2", "test3", "test4", "test5", "test6", "test7", "test8", "test9", "test10", "test11", "test12" };


            for (int i = 0; i < 8; i++)
            {
                if (i >= szResult.Length)
                    return;
                ListBox1.Items[i].Value = szResult[i];
            }
        }

        protected void CheckBox5_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void CheckTextBox()
        {
            if(txtUseSound.Text == "")
            {
                Response.Write("<script language='javascript'>alert('소리를 입력하세요.');</script>");
                return;
            }
            
        }

        protected void ImageButton2_Click(object sender, ImageClickEventArgs e)
        {
            CheckTextBox();
            SenarioWeb.SenarioService.PreSafe service = new SenarioWeb.SenarioService.PreSafe();

            bool bUseSound = chkUseSound.Checked;
            bool bUseHeartBeat = chkHeartBeat.Checked;
            bool bUseAlcole = chkUseAlcole.Checked;
            bool bUseVelocity = chkUseVelocity.Checked;
            bool bUseAcc = chkUseAcc.Checked;
            bool bUseLocation = chkUseLocation.Checked;
            bool bUseImpact = chkUseImpact.Checked;
            bool bImpact = false;
            if (ListUseImpact.SelectedIndex == 0)
            {
                bImpact = true;
            }
            else if(ListUseImpact.SelectedIndex == 1)
            {
                bImpact = false;
            }
            else
            {
                //아무것도 선택 안되어있을 때
            }

            float fSound = 0.0f;
            if(bUseSound == true)
                fSound = Convert.ToSingle(txtUseSound.Text);
            
            int nHeartBeat = 0;
            if(bUseHeartBeat == true)
                nHeartBeat = Convert.ToInt32(txtHeartBeat.Text);

            float fAlcole = 0.0f;
            if(bUseAlcole == true)
                fAlcole = Convert.ToSingle(txtUseAlcole.Text);

            float fVelocity = 0.0f;
            if(bUseVelocity == true)
                fVelocity = Convert.ToSingle(txtUseVelocity.Text);

            float fAccelate = 0.0f;
            if (bUseAcc == true)
                fAccelate = Convert.ToSingle(txtUseAcc.Text);
            

            string[] szResult = service.RunSenario("test1", bUseSound, fSound, bUseHeartBeat, nHeartBeat, bUseAlcole, fAlcole
                , bUseVelocity, fVelocity, bUseAcc, fAccelate, bUseLocation, ListUseLoaction.SelectedIndex + 1, bUseImpact, bImpact);
            Label1.Text = szResult[2] + "%";

            lblUseSound.Text = txtUseSound.Text;
            lblUseHeartBeat.Text = txtHeartBeat.Text;
            lblUseVelocity.Text = txtUseVelocity.Text;

            if (bImpact == true)
                lblUseImpact.Text = "충격있음";
            else
                lblUseImpact.Text = "충격없음";

            lblUseLocation.Text = ListUseLoaction.SelectedValue;

        }

        protected void chkUseImpact_CheckedChanged(object sender, EventArgs e)
        {

        }

        protected void chkUseSound_CheckedChanged(object sender, EventArgs e)
        {

        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblSenarioTitle.Text = ListBox1.Items[ListBox1.SelectedIndex].Value;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {

        }
    }
}