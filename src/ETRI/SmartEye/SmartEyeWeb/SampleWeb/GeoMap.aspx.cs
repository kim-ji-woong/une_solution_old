using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartEyeWeb.SampleWeb
{
    public partial class GeoMap : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                textBoxCoord.Text = "37.5675451, 126.9773356";
            }
        }

        protected void btnMove_Click(object sender, EventArgs e)
        {
            string strCoord = textBoxCoord.Text;
            string[] coords = strCoord.Split(',');

            if (coords.Length == 2)
            {
                double latitude, longitude;

                if (double.TryParse(coords[0], out latitude) && double.TryParse(coords[1], out longitude))
                {
                    SearchMap(latitude, longitude);
                }
            }
        }

        private void SearchMap(double latitude, double longitude)
        {
            string msg = "<script language=\"javascript\">";
            msg += "var oInitPoint = new nhn.api.map.LatLng(" + latitude.ToString() + ", " + longitude.ToString() + ");";
            msg += "oMap.setCenterAndLevel(oInitPoint, defaultLevel);";
            msg += "</script>";
            Response.Write(msg);
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            // DB 확인

            // 처리


            string strScript = "";
            strScript += "var oInitPoint = new nhn.api.map.LatLng(37.0, 126.8);";
            strScript += "oMap.setCenterAndLevel(oInitPoint, defaultLevel);";
            strScript += "oMap.clearOverlay();";
            strScript += "var oMarker = new nhn.api.map.Marker(oIcon, { });";
            strScript += "oMarker.setPoint(oInitPoint);";
            strScript += "oMap.addOverlay(oMarker);";

            ScriptManager.RegisterStartupScript(this, GetType(), "SetPosition", strScript, true);

           

        }
    }
}