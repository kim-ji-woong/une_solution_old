using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartEyeWeb.SampleWeb
{
    public partial class SendImage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private void MessageBox(string sMessage)
        {
            /*string msg = "<script language=\"javascript\">";
            msg += "alert('" + sMessage + "');";
            msg += "</script>";
            Response.Write(msg);*/
            ScriptManager.RegisterStartupScript(this, GetType(), "MessageBoxKey", "alert('" + sMessage + "');", true); 
        }

        protected void btnSendDisasterImage_Click(object sender, EventArgs e)
        {
            string strImageURL = txtImageURL.Text;
            string strLatitude = txtLatitude.Text;
            string strLongitude = txtLongitude.Text;
            string strTime = txtTime.Text;

            double latitude = 0.0, longitude = 0.0;

            double.TryParse(strLatitude, out latitude);
            double.TryParse(strLongitude, out longitude);

            using (Service.SmartEyeService service = new Service.SmartEyeService())
            {
                int result = service.SendDisasterImageData(0, strImageURL, latitude, longitude, strTime, textBoxDisasterDescription.Text);
                MessageBox("Result : " + result.ToString());
            }
        }

        protected void btnSendImage_Click(object sender, EventArgs e)
        {
            string strImageURL = textBoxSendImageURL.Text;
            string strLatitude = textBoxSendImageLatitude.Text;
            string strLongitude = textBoxSendImageLongitude.Text;
            string strTime = textBoxSendImageTime.Text;
            string strDescription = textBoxSendImageDescription.Text;

            double latitude = 0.0, longitude = 0.0;

            double.TryParse(strLatitude, out latitude);
            double.TryParse(strLongitude, out longitude);

            using (Service.SmartEyeService service = new Service.SmartEyeService())
            {
                bool result = service.SendImageData(strImageURL, latitude, longitude, strTime, strTime, strDescription);
                MessageBox("Result : " + result.ToString());
            }
        }

        protected void btnEndDisaster_Click(object sender, EventArgs e)
        {
            /*string strDisasterID = textBoxDisasterID.Text;
            int nDisasterID;

            if (int.TryParse(strDisasterID, out nDisasterID) && nDisasterID > 0)
            {
                using (Service.SmartEyeService service = new Service.SmartEyeService())
                {
                    bool result = service.EndDisaster(nDisasterID);
                    MessageBox("Result : " + result.ToString());
                }
            }
            else
                MessageBox("DisasterID가 올바르지 않습니다.");*/
            using (Service.SmartEyeService service = new Service.SmartEyeService())
            {
                bool result = service.EndDisaster(0);
                MessageBox("Result : " + result.ToString());
            }
        }

        protected void btnSendActionData_Click(object sender, EventArgs e)
        {
            using (Service.SmartEyeService service = new Service.SmartEyeService())
            {
                int nActionID = GetCurrentActionStepID();
                string strDescription = textBoxActionDescription.Text;

                bool result = service.SendActionData(0, nActionID, strDescription);

                MessageBox("Result : " + result.ToString());
            }
        }

        private int GetCurrentActionStepID()
        {
            /*if (radioCollect.Checked)
                return 0;
            else if (radioAnalys.Checked)
                return 1;
            else if (radioPredict.Checked)
                return 2;
            else if (radioWarning.Checked)
                return 4;
            else if (radioReaction.Checked)
                return 5;*/

            return radioActionStep.SelectedIndex >= 3 ? radioActionStep.SelectedIndex + 1 : radioActionStep.SelectedIndex;
        }

        protected void radioActionStep_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (radioActionStep.SelectedIndex == 0)
                textBoxActionDescription.Text = "단계:수집중";
            else if (radioActionStep.SelectedIndex == 1)
                textBoxActionDescription.Text = "단계:분석중";
            else if (radioActionStep.SelectedIndex == 2)
                textBoxActionDescription.Text = "단계:예측중";
            else if (radioActionStep.SelectedIndex == 3)
                textBoxActionDescription.Text = "단계:경보중";
            else if (radioActionStep.SelectedIndex == 4)
                textBoxActionDescription.Text = "단계:대응중";
        }
    }
}