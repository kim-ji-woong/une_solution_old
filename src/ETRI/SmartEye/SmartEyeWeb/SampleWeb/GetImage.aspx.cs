using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartEyeWeb.SampleWeb
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                labelResult.Text = "";
            }
        }

        protected void btnBeginDisaster_Click(object sender, EventArgs e)
        {
            string strStation = "U&E";
            string strLocation = "서울시 용산구";
            string strEtc = "";
            string time = "14:45:00";

            using (Service.SmartEyeService service = new Service.SmartEyeService())
            {
                int nDisasterID = service.BeginDisaster(strStation, strLocation, strEtc, time);
                System.Diagnostics.Trace.WriteLine("BeginDisaster : " + nDisasterID.ToString());
            }
        }

        private void MessageBox(string sMessage)
        {
            /*string msg = "<script language=\"javascript\">";
            msg += "alert('" + sMessage + "');";
            msg += "</script>";
            Response.Write(msg);*/
            ScriptManager.RegisterStartupScript(this, GetType(), "MessageBoxKey", "alert('" + sMessage + "');", true);
        }

        protected void btnGetDisasterImage_Click(object sender, EventArgs e)
        {
            ResultData prevResult = (ResultData)Application["Result"];
            
            using (Service.SmartEyeService service = new Service.SmartEyeService())
            {
                string[] results = service.GetDisasterImageData();

                if (results != null && results.Count() == 10)
                {
                    //             [0] : Image URL
                    //             [1] : Latitude
                    //             [2] : Longitude
                    //             [3] : 촬영 시각
                    //             [4] : Description
                    //             [5] : 관측소 이름
                    //             [6] : 재난발생 장소 이름
                    //             [7] : 기타
                    //             [8] : 재난발생 시각
                    //             [9] : Disaster ID
                    string strImageURL = results[0];
                    double latitude, longitude;
                    DateTime dtImage, dtDisaster;
                    int nDisasterID = 0;

                    if (!ResultData.GetGPSCoords(out latitude, out longitude, results[1], results[2]))
                    {
                        SetResultData(null, prevResult, "GetDisasterImageData GPS 오류");
                        return;
                    }

                    if (!DateTime.TryParse(results[3], out dtImage))
                    {
                        SetResultData(null, prevResult, "GetDisasterImageData 촬영시각 오류");
                        return;
                    }

                    List<string> values = new List<string>();
                    List<string> tags = ResultData.ParseTagDatas(results[4], values);

                    string strWatcher = results[5];
                    string strLocation = results[6];
                    string strEtc = results[7];

                    if (!DateTime.TryParse(results[8], out dtDisaster))
                    {
                        SetResultData(null, prevResult, "GetDisasterImageData 재난발생 시각 오류");
                        return;
                    }

                    if (int.TryParse(results[9], out nDisasterID))
                    {
                        string strResultText = "GetDisasterImageData 성공, Disaster ID : " + nDisasterID.ToString();
                        DisasterResult result = new DisasterResult(strImageURL, results[1], results[2], dtImage, results[4], strResultText, nDisasterID, strWatcher, strLocation, strEtc, dtDisaster);
                        SetResultData(result, prevResult, strResultText);
                    }
                    else
                        SetResultData(null, prevResult, "GetDisasterImageData Disaster ID 오류");
                }
                else
                {
                    results = service.GetImageData();

                    if (results != null && results.Count() == 5)
                    {
                        //             [0] : Image URL
                        //             [1] : Latitude
                        //             [2] : Longitude
                        //             [3] : 촬영 시각
                        //             [4] : Description
                        string strImageURL = results[0];
                        double latitude, longitude;
                        DateTime dtImage;

                        if (!ResultData.GetGPSCoords(out latitude, out longitude, results[1], results[2]))
                        {
                            SetResultData(null, prevResult, "GetImageData GPS 오류");
                            return;
                        }

                        if (!DateTime.TryParse(results[3], out dtImage))
                        {
                            SetResultData(null, prevResult, "GetImageData 촬영시각 오류");
                            return;
                        }

                        List<string> values = new List<string>();
                        List<string> tags = ResultData.ParseTagDatas(results[4], values);

                        string strResultText = "GetImageData 성공";

                        RealTimeResult result = new RealTimeResult(strImageURL, results[1], results[2], dtImage, results[4], strResultText);
                        SetResultData(result, prevResult, strResultText);
                    }
                    else
                        SetResultData(null, prevResult, "Image Data 없음");
                }
            }
        }

        private void SetResultData(ResultData dataCurrent, ResultData dataPrev, string strResultText)
        {
            if (dataPrev == null)
            {
                if (dataCurrent == null)
                    dataCurrent = new ResultData();

                SetResultData(dataCurrent, strResultText);
            }
            else if (dataCurrent == null)
            {
                dataCurrent = new ResultData();
                dataCurrent.ResultText = strResultText;

                if (!dataPrev.Equals(dataCurrent))
                    SetResultData(dataCurrent, strResultText);
            }
            else
            {
                dataCurrent.ResultText = strResultText;

                if (!dataPrev.Equals(dataCurrent))
                    SetResultData(dataCurrent, strResultText);
            }
        }

        private void SetResultData(ResultData data, string strResultText)
        {
            data.ResultText = strResultText;

            Application["Result"] = data;

            labelResult.Text = strResultText;
            Image1.ImageUrl = data.ImageURL;
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            btnGetDisasterImage_Click(null, null);
        }
    }
}