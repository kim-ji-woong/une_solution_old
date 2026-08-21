using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartEyeWeb
{
    public partial class Watch : System.Web.UI.Page
    {
        private int m_nFunctionIndex = 0;
        private static string SEPARATOR_KEY = "!@#$%^&*()";

        private string GetTimeString()
        {
            DateTime dtNow = DateTime.Now;
            string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}_{6}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second, dtNow.Millisecond);
            return strTime;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["RealTimeResult"] = null;
                Session["DisasterResult"] = null;
                Session["ActionStep"] = null;

                //CheckInformation();
            }
            /*List<string> statusList = new List<string>();
            List<string> disasterList = new List<string>();

            for (int i=1;i<7;i++)
            {
                string strLine = "센서" + i.ToString() + "(가시광선) 정보: 화염 및 연기 감지 정보";
                statusList.Add(strLine);
                disasterList.Add(strLine);
            }

            SetText(statusList, "StatusInfo", repeaterStatus);
            SetText(disasterList, "DisasterInfo", repeaterDisaster);*/
        }

        private void SetText(string strData, string strContainerName, Repeater repeater)
        {
            List<string> textList = new List<string>();

            string[] datas = strData.Split(';');

            foreach (string data in datas)
            {
                int nIndex = data.IndexOf(':');

                if (nIndex >= 0)
                {
                    string strTag = data.Substring(0, nIndex).Trim();
                    string strValue = data.Substring(nIndex + 1).Trim();

                    textList.Add(strTag + " : " + strValue);
                }
                else
                    textList.Add(data.Trim());
                /*string[] tokens = data.Split(':');

                if (tokens.Count() == 2)
                {
                    string strTag = tokens[0].Trim();
                    string strValue = tokens[1].Trim();

                    textList.Add(strTag + " : " + strValue);
                }
                else
                    textList.Add(data.Trim());*/
            }

            SetText(textList, strContainerName, repeater);
        }

        private void SetText(List<string> textList, string strContainerName, Repeater repeater)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn(strContainerName));

            int nLinkIndex = -1;
            string strImageURL = "";
            int nTextCount = textList.Count;

            HasReactionImageLink(textList, ref nLinkIndex, ref strImageURL);
            dt.Columns.Add(new DataColumn("URLColumnName"));


            for (int i=0;i<nTextCount;i++)
            {
                string strText = textList[i];

                if (strText.Length == 0)
                    continue;

                if (i == nLinkIndex)
                {
                    DataRow row = dt.NewRow();
                    row[strContainerName] = "대응정보 바로가기";
                    row["URLColumnName"] = strImageURL;
                    dt.Rows.Add(row);
                }
                else
                {
                    DataRow row = dt.NewRow();
                    row[strContainerName] = strText;
                    dt.Rows.Add(row);
                }
            }

            dt.AcceptChanges();

            repeater.DataSource = dt;
            repeater.DataBind();
        }

        private bool HasReactionImageLink(List<string> textList, ref int nLinkIndex, ref string strLink)
        {
            int nLineCount = textList.Count;

            for (int i = 0; i < nLineCount;i++ )
            {
                string strText = textList[i];

                if (strText.Length == 0)
                    continue;

                int nIndex = strText.IndexOf(':');

                if (nIndex >= 0)
                {
                    string strTag = strText.Substring(0, nIndex).Trim();

                    if (strTag == "SmartEyeReactionImageLink")
                    {
                        strLink = strText.Substring(nIndex + 1).Trim();
                        nLinkIndex = i;
                        return true;
                    }
                }
            }

            return false;
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            /*nRefreshCount += 10;

            string strScript = "ProcessMove(" + nRefreshCount.ToString() + ");";

            ScriptManager.RegisterStartupScript(this, GetType(), "ProcessMove", strScript, true);*/

            /*if ((nRefreshCount / 10) % 2 == 0)
                SetRealTimeStatus();
            else
                SetFireStatus();*/

            CheckInformation();
        }

        private void CheckActionStep(Service.SmartEyeService service)
        {
            ActionData prevAction = (ActionData)Session["ActionStep"];

            string[] results = service.GetActionData(0);

            if (results != null && results.Count() == 3)
            {
                int nActionID, nDisasterID;

                if (int.TryParse(results[0], out nActionID) && int.TryParse(results[1], out nDisasterID))
                {
                    ActionData data = new ActionData(nActionID, nDisasterID, results[2]);
                    SetActionStep(data, prevAction);
                }
                else
                    SetActionStep(null, prevAction);
            }
            else
                SetActionStep(null, prevAction);
        }

        private void SetActionStep(ActionData actionCurrent, ActionData actionPrev)
        {
            if (actionPrev == null)
            {
                if (actionCurrent == null)
                    actionCurrent = new ActionData();

                SetActionStep(actionCurrent);
            }
            else if (actionCurrent == null)
            {
                actionCurrent = new ActionData();

                if (!actionCurrent.Equals(actionPrev))
                    SetActionStep(actionCurrent);
            }
            else
            {
                if (!actionPrev.Equals(actionCurrent))
                    SetActionStep(actionCurrent);
            }
        }

        // str1 : 수집 및 분석에 대한 Text
        // str2 : 예측, 경보, 대응에 대한 Text
        private void GetActionStepText(ActionData action, ref string str1, ref string str2)
        {
            if (action == null || action.Description == null || action.Description.Length == 0)
                return;

            int nActionData;
            string strCollect = null, strAnalys = null, strPredict = null, strWarning = null, strAction = null;

            char ch = (char)6;
            string strOldValue = SEPARATOR_KEY;
            string strNewValue = ch + "";

            string strDescription = action.Description.Replace(strOldValue, strNewValue);
            string[] tokens = strDescription.Split(ch);

            //string[] tokens = action.Description.Split(ch);

            foreach (string strLine in tokens)
            {
                int nIndex1 = strLine.IndexOf('[');
                int nIndex2 = strLine.IndexOf(']');

                if (nIndex1 < 0 || nIndex2 < 0 || nIndex2 <= nIndex1)
                    continue;

                string strNo = strLine.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);

                if (!int.TryParse(strNo, out nActionData))
                    continue;

                if (nActionData == (int)ActionData.ActionStep.수집)
                    strCollect = strLine.Substring(nIndex2 + 1);
                else if (nActionData == (int)ActionData.ActionStep.분석)
                    strAnalys = strLine.Substring(nIndex2 + 1);
                else if (nActionData == (int)ActionData.ActionStep.예측)
                    strPredict = strLine.Substring(nIndex2 + 1);
                else if (nActionData == (int)ActionData.ActionStep.경보)
                    strWarning = strLine.Substring(nIndex2 + 1);
                else if (nActionData == (int)ActionData.ActionStep.대응)
                    strAction = strLine.Substring(nIndex2 + 1);
            }

            AddActionStepText(strCollect, "[수집]", ref str1);
            AddActionStepText(strAnalys, "[분석]", ref str1);
            AddActionStepText(strPredict, "[예측]", ref str2);
            AddActionStepText(strWarning, "[경보]", ref str2);
            AddActionStepText(strAction, "[대응]", ref str2);
        }

        private void AddActionStepText(string strData, string strTag, ref string str)
        {
            if (strData == null)
                return;

            if (str.Length == 0)
                str = strTag + ":" + strData;
            else
                str += ";" + strTag + ":" + strData;
        }

        private void SetActionStep(ActionData action)
        {
            Session["ActionStep"] = action;

            string strLeft = "", strRight = "";
            GetActionStepText(action, ref strLeft, ref strRight);

            if (action != null)
            {
                //SetText(action.Description, "ActionInfo", repeaterAction);
                SetProcessBar(action.Current);
            }
            else
            {
                //SetText("", "ActionInfo", repeaterAction);
                SetProcessBar(ActionData.ActionStep.NONE);
            }

            SetText(strLeft, "StatusInfo", repeaterStatus);
            SetText(strRight, "ActionInfo", repeaterAction);
        }

        private void SetProcessBar(ActionData.ActionStep actionStep)
        {
            string[] imageTags = new string[] { "collect", "analys", "predict", "warning", "reaction" };

            int nIndex = (int)actionStep;

            if (actionStep >= ActionData.ActionStep.시각화)
                nIndex--;

            string strCurrentTag = actionStep == ActionData.ActionStep.NONE ? "" : imageTags[nIndex];
            string strScript = "ProcessMove(";

            if (actionStep == ActionData.ActionStep.수집)
                strScript += "10);";
            else if (actionStep == ActionData.ActionStep.분석)
                strScript += "266);";
            else if (actionStep == ActionData.ActionStep.예측)
                strScript += "533);";
            else if (actionStep == ActionData.ActionStep.경보)
                strScript += "800);";
            else if (actionStep == ActionData.ActionStep.대응)
                strScript += "1050);";
            else
                strScript += "10);";

            ScriptManager.RegisterStartupScript(this, GetType(), "ProcessMove" + (m_nFunctionIndex++).ToString(), strScript, true);

            foreach (string strTag in imageTags)
            {
                if (strTag == strCurrentTag)
                    strScript = "SetImage(\"" + strTag + "\", \"./Images/step_on.gif\");";
                else
                    strScript = "SetImage(\"" + strTag + "\", \"./Images/step_off.png\");";

                ScriptManager.RegisterStartupScript(this, GetType(), "SetImage" + (m_nFunctionIndex++).ToString(), strScript, true);
            }
        }

        protected void CheckInformation()
        {
            ResultData prevDisasterResult = (ResultData)Session["DisasterResult"];
            ResultData prevRealTimeResult = (ResultData)Session["RealTimeResult"];
            m_nFunctionIndex = 0;

            using (Service.SmartEyeService service = new Service.SmartEyeService())
            {
                CheckActionStep(service);

                string[] results = service.GetDisasterImageData();
                bool fireStatus = CheckDisasterResult(results, prevDisasterResult);

                results = service.GetImageData();
                CheckRealTimeResult(results, prevRealTimeResult, !fireStatus);

                //if (results != null && results.Count() == 10)
                //{
                //    //             [0] : Image URL
                //    //             [1] : Latitude
                //    //             [2] : Longitude
                //    //             [3] : 촬영 시각
                //    //             [4] : Description
                //    //             [5] : 관측소 이름
                //    //             [6] : 재난발생 장소 이름
                //    //             [7] : 기타
                //    //             [8] : 재난발생 시각
                //    //             [9] : Disaster ID
                //    string strImageURL = results[0];
                //    double latitude, longitude;
                //    DateTime dtImage, dtDisaster;
                //    int nDisasterID = 0;

                //    if (!ResultData.GetGPSCoords(out latitude, out longitude, results[1], results[2]))
                //    {
                //        SetDisasterResult(null, prevResult/*, "GetDisasterImageData GPS 오류"*/);
                //        return;
                //    }

                //    if (!DateTime.TryParse(results[3], out dtImage))
                //    {
                //        SetDisasterResult(null, prevResult/*, "GetDisasterImageData 촬영시각 오류"*/);
                //        return;
                //    }

                //    string strWatcher = results[5];
                //    string strLocation = results[6];
                //    string strEtc = results[7];

                //    if (!DateTime.TryParse(results[8], out dtDisaster))
                //    {
                //        SetDisasterResult(null, prevResult/*, "GetDisasterImageData 재난발생 시각 오류"*/);
                //        return;
                //    }

                //    if (int.TryParse(results[9], out nDisasterID))
                //    {
                //        //string strResultText = "GetDisasterImageData 성공, Disaster ID : " + nDisasterID.ToString();
                //        DisasterResult result = new DisasterResult(strImageURL, results[1], results[2], dtImage, results[4], "", nDisasterID, strWatcher, strLocation, strEtc, dtDisaster);
                //        SetDisasterResult(result, prevResult/*, strResultText*/);
                //    }
                //    else
                //        SetDisasterResult(null, prevResult/*, "GetDisasterImageData Disaster ID 오류"*/);

                //    return;
                //}
                //else
                //{
                //    results = service.GetImageData();

                //    /*if (results == null)
                //    {
                //        statusList.Add("results is null");
                //    }
                //    else
                //    {
                //        int nResultCount = results.Length;

                //        for (int i = 0; i < nResultCount; i++)
                //        {
                //            statusList.Add(i.ToString() + ", " + results[i]);
                //        }

                //        SetText(statusList, "StatusInfo", repeaterStatus);
                //    }*/

                //    if (results != null && results.Count() == 5)
                //    {
                //        //             [0] : Image URL
                //        //             [1] : Latitude
                //        //             [2] : Longitude
                //        //             [3] : 촬영 시각
                //        //             [4] : Description
                //        string strImageURL = results[0];
                //        double latitude, longitude;
                //        DateTime dtImage;

                //        if (!ResultData.GetGPSCoords(out latitude, out longitude, results[1], results[2]))
                //        {
                //            SetRealTimeResult(null, prevResult/*, "GetImageData GPS 오류"*/);
                //            return;
                //        }

                //        if (!DateTime.TryParse(results[3], out dtImage))
                //        {
                //            SetRealTimeResult(null, prevResult/*, "GetImageData 촬영시각 오류"*/);
                //            return;
                //        }

                //        List<string> values = new List<string>();
                //        List<string> tags = ResultData.ParseTagDatas(results[4], values);

                //        //string strResultText = "GetImageData 성공";

                //        RealTimeResult result = new RealTimeResult(strImageURL, results[1], results[2], dtImage, results[4], ""/*strResultText*/);
                //        SetRealTimeResult(result, prevResult/*, strResultText*/);
                //    }
                //    else
                //        SetRealTimeResult(null, prevResult/*, "Image Data 없음"*/);
                //}
            }

            //SetRealTimeStatus();
        }

        private void CheckRealTimeResult(string[] results, ResultData prevResult, bool realTimeStatus)
        {
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
                    SetRealTimeResult(null, prevResult/*, "GetImageData GPS 오류"*/);
                    return;
                }

                if (!DateTime.TryParse(results[3], out dtImage))
                {
                    SetRealTimeResult(null, prevResult/*, "GetImageData 촬영시각 오류"*/);
                    return;
                }

                List<string> values = new List<string>();
                List<string> tags = ResultData.ParseTagDatas(results[4], values);

                //string strResultText = "GetImageData 성공";

                RealTimeResult result = new RealTimeResult(strImageURL, results[1], results[2], dtImage, results[4], ""/*strResultText*/);
                SetRealTimeResult(result, prevResult/*, strResultText*/);
            }
            else
                SetRealTimeResult(null, prevResult/*, "Image Data 없음"*/);

            if (realTimeStatus)
                SetRealTimeStatus();
        }

        private bool CheckDisasterResult(string[] results, ResultData prevResult)
        {
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
                    SetDisasterResult(null, prevResult/*, "GetDisasterImageData GPS 오류"*/);
                    return true;
                }

                if (!DateTime.TryParse(results[3], out dtImage))
                {
                    SetDisasterResult(null, prevResult/*, "GetDisasterImageData 촬영시각 오류"*/);
                    return true;
                }

                string strWatcher = results[5];
                string strLocation = results[6];
                string strEtc = results[7];

                if (!DateTime.TryParse(results[8], out dtDisaster))
                {
                    SetDisasterResult(null, prevResult/*, "GetDisasterImageData 재난발생 시각 오류"*/);
                    return true;
                }

                if (int.TryParse(results[9], out nDisasterID))
                {
                    //string strResultText = "GetDisasterImageData 성공, Disaster ID : " + nDisasterID.ToString();
                    DisasterResult result = new DisasterResult(strImageURL, results[1], results[2], dtImage, results[4], "", nDisasterID, strWatcher, strLocation, strEtc, dtDisaster);
                    SetDisasterResult(result, prevResult/*, strResultText*/);
                }
                else
                    SetDisasterResult(null, prevResult/*, "GetDisasterImageData Disaster ID 오류"*/);

                return true;
            }
            else
                SetDisasterResult(null, prevResult);

            return false;
        }

        private void SetDisasterResult(DisasterResult dataCurrent, ResultData dataPrev)
        {
            if (dataPrev == null)
            {
                if (dataCurrent == null)
                    dataCurrent = new DisasterResult();

                SetResultData(dataCurrent);
            }
            else if (dataCurrent == null)
            {
                dataCurrent = new DisasterResult();

                if (!dataCurrent.Equals(dataPrev))
                {
                    SetResultData(dataCurrent);
                }
            }
            else
            {
                if (!dataPrev.Equals(dataCurrent))
                {
                    SetResultData(dataCurrent);
                }
            }

            Session["DisasterResult"] = dataCurrent;
            SetFireStatus();
        }

        private void SetRealTimeResult(RealTimeResult dataCurrent, ResultData dataPrev)
        {
            if (dataPrev == null)
            {
                if (dataCurrent == null)
                    dataCurrent = new RealTimeResult();

                SetResultData(dataCurrent);
            }
            else if (dataCurrent == null)
            {
                dataCurrent = new RealTimeResult();

                if (!dataCurrent.Equals(dataPrev))
                    SetResultData(dataCurrent);
            }
            else
            {
                if (!dataPrev.Equals(dataCurrent))
                    SetResultData(dataCurrent);
            }

            Session["RealTimeResult"] = dataCurrent;
        }

        private void MessageBox(string sMessage)
        {
            /*string msg = "<script language=\"javascript\">";
            msg += "alert('" + sMessage + "');";
            msg += "</script>";
            Response.Write(msg);*/
            ScriptManager.RegisterStartupScript(this, GetType(), "MessageBoxKey" + (m_nFunctionIndex++).ToString(), "alert('" + sMessage + "');", true);
        }

        private void SetTimerText(DateTime dtImage)
        {
            string strTime = string.Format("{0:00}:{1:00}:{2:00}", dtImage.Hour, dtImage.Minute, dtImage.Second);
            SetTimerText(strTime);
        }

        private void SetTimerText(string strTime)
        {
            string strScript = "SetText(\"droneImageTimer\", \"" + strTime + "\");";
            ScriptManager.RegisterStartupScript(this, GetType(), "SetText" + (m_nFunctionIndex++).ToString(), strScript, true);
        }

        private void SetResultData(ResultData data)
        {
            /*List<string> statusList = new List<string>();
            statusList.Add("Coord : " + data.Latitude + ", " + data.Longitude);
            SetText(statusList, "StatusInfo", repeaterStatus);*/

            Session["Result"] = data;
            
            if (data != null)
            {
                string strImage = data.ImageResult == ImageResult.REAL_TIME_IMAGE ? "droneImage" : "disasterImage";

                // [2016/02/15] 김지웅
                // 재난상황에 대한 Text 표시를 하지 않는다.
                /*if (data.ImageResult == ImageResult.DISASTER_IMAGE)
                    SetText(data.Description, "StatusInfo", repeaterStatus);*/

                if (data.ImageURL.Length > 0)
                {
                    string strScript2 = "SetImage(\"" + strImage + "\", \"" + data.ImageURL + "\");";
                    ScriptManager.RegisterStartupScript(this, GetType(), "SetImageResult" + (m_nFunctionIndex++).ToString(), strScript2, true);
                    //droneImage.ImageUrl = data.ImageURL;
                    System.Diagnostics.Trace.WriteLine(strImage + ", Image : " + data.ImageURL);

                    if (data.ImageResult == ImageResult.REAL_TIME_IMAGE)
                        SetTimerText(data.ImageTime);
                    /*else
                        SetTimerText("00:00:00");*/
                }
                else
                {
                    string strScript2 = "SetImage(\"" + strImage + "\", \"./Images/img01.jpg\");";
                    ScriptManager.RegisterStartupScript(this, GetType(), "SetImageResult" + (m_nFunctionIndex++).ToString(), strScript2, true);
                    //droneImage.ImageUrl = "./Images/img01.jpg";
                    System.Diagnostics.Trace.WriteLine(strImage + ", Image : null");

                    if (data.ImageResult == ImageResult.REAL_TIME_IMAGE)
                        SetTimerText("00:00:00");
                }

                if (data.ImageResult == ImageResult.DISASTER_IMAGE)
                {
                    string strLatitude = data.Latitude;
                    string strLongitude = data.Longitude;

                    if (strLatitude.Length == 0 || strLongitude.Length == 0)
                    {
                        strLatitude = "37.5675451";
                        strLongitude = "126.9773356";
                    }

                    string strScript = "";
                    strScript += "var oInitPoint = new nhn.api.map.LatLng(" + strLatitude + ", " + strLongitude + ");";
                    strScript += "oMap.setCenterAndLevel(oInitPoint, defaultLevel);";
                    strScript += "oMap.clearOverlay();";
                    strScript += "var oMarker = new nhn.api.map.Marker(oIcon, { });";
                    strScript += "oMarker.setPoint(oInitPoint);";
                    strScript += "oMap.addOverlay(oMarker);";

                    ScriptManager.RegisterStartupScript(this, GetType(), "SetPosition" + (m_nFunctionIndex++).ToString(), strScript, true);
                }
            }
            else
            {
                string strScript2 = "SetImage(\"droneImage\", \"./Images/img01.jpg\");";
                ScriptManager.RegisterStartupScript(this, GetType(), "SetImageResultDrone" + (m_nFunctionIndex++).ToString(), strScript2, true);

                string strScript3 = "SetImage(\"disasterImage\", \"./Images/img02.jpg\");";
                ScriptManager.RegisterStartupScript(this, GetType(), "SetImageResultDisaster" + (m_nFunctionIndex++).ToString(), strScript3, true);

                //droneImage.ImageUrl = "./Images/img01.jpg";
                System.Diagnostics.Trace.WriteLine("Image : null, data null");
            }
        }

        private void SetRealTimeStatus()
        {
            string strScript = "";/* "SetImage(\"droneImageTitle\", \"./Images/txt_situ.png\");";
            ScriptManager.RegisterStartupScript(this, GetType(), "SetImage", strScript, true);

            strScript = "SetBackgroundImage(\"leftBox\", \"url('../Images/icon_fire.png') no-repeat 8px top\");";
            ScriptManager.RegisterStartupScript(this, GetType(), "SetBackgroundImage", strScript, true);*/

            strScript = "SetImage(\"disasterImageTitle\", \"./Images/txt_jmov_off.png\");";
            ScriptManager.RegisterStartupScript(this, GetType(), "SetImage" + (m_nFunctionIndex++).ToString(), strScript, true);

            strScript = "SetBackgroundImage(\"rightImageBox\", \"url('../Images/icon_fire.png') no-repeat 8px top\");";
            ScriptManager.RegisterStartupScript(this, GetType(), "SetBackgroundImageRight" + (m_nFunctionIndex++).ToString(), strScript, true);

            strScript = "SetBackgroundImage(\"middleBox\", \"url('../Images/icon_spot.png') no-repeat 8px top\");";
            ScriptManager.RegisterStartupScript(this, GetType(), "SetBackgroundImage" + (m_nFunctionIndex++).ToString(), strScript, true);

            strScript = "SetImage(\"mapTitle\", \"./Images/txt_spot.png\");";
            ScriptManager.RegisterStartupScript(this, GetType(), "SetImage" + (m_nFunctionIndex++).ToString(), strScript, true);

            strScript = "SetBackgroundImage(\"rightBox\", \"url('../Images/icon_spot.png') no-repeat 8px top\");";
            ScriptManager.RegisterStartupScript(this, GetType(), "SetBackgroundImage" + (m_nFunctionIndex++).ToString(), strScript, true);

            strScript = "SetBorderColor(\"droneImage\", \"5px solid white\");";
            ScriptManager.RegisterStartupScript(this, GetType(), "SetBorderColor" + (m_nFunctionIndex++).ToString(), strScript, true);
            //droneImage.BorderColor = System.Drawing.Color.White;

            strScript = "SetBorderColor(\"map\", \"5px solid white\");";
            ScriptManager.RegisterStartupScript(this, GetType(), "SetBorderColor" + (m_nFunctionIndex++).ToString(), strScript, true);

            strScript = "SetBorderColor(\"disasterImage\", \"5px solid white\");";
            ScriptManager.RegisterStartupScript(this, GetType(), "SetBorderColor" + (m_nFunctionIndex++).ToString(), strScript, true);

            strScript = "SetClass(\"btnReset\", \"btn_reset2\");";
            ScriptManager.RegisterStartupScript(this, GetType(), "SetClass" + (m_nFunctionIndex++).ToString(), strScript, true);

            strScript = "SetFontColor(\"LeftBox\", \"black\");";
            ScriptManager.RegisterStartupScript(this, GetType(), "SetFontColor" + (m_nFunctionIndex++).ToString(), strScript, true);

            strScript = "SetFontColor(\"RightBox\", \"black\");";
            ScriptManager.RegisterStartupScript(this, GetType(), "SetFontColor" + (m_nFunctionIndex++).ToString(), strScript, true);
            /*btnInit.ImageUrl = "./Images/btn_off.png";
            btnInit.CssClass = "btn_resetDisabled";
            btnInit.Enabled = false;*/
        }

        private void SetFireStatus()
        {
            string strScript = "";/* "SetImage(\"droneImageTitle\", \"./Images/txt_situ_on.png\");";
            ScriptManager.RegisterStartupScript(this, GetType(), "SetImage3", strScript, true);

            strScript = "SetBackgroundImage(\"leftBox\", \"url('../Images/icon_fire_on.png') no-repeat 8px top\");";
            ScriptManager.RegisterStartupScript(this, GetType(), "SetBackgroundImage4", strScript, true);*/

            strScript = "SetImage(\"disasterImageTitle\", \"./Images/txt_jmov_on.png\");";
            ScriptManager.RegisterStartupScript(this, GetType(), "SetImage" + (m_nFunctionIndex++).ToString(), strScript, true);

            strScript = "SetBackgroundImage(\"rightImageBox\", \"url('../Images/icon_fire_on.png') no-repeat 8px top\");";
            ScriptManager.RegisterStartupScript(this, GetType(), "SetBackgroundImageRight" + (m_nFunctionIndex++).ToString(), strScript, true);

            strScript = "SetBackgroundImage(\"middleBox\", \"url('../Images/icon_spot_on.png') no-repeat 8px top\");";
            ScriptManager.RegisterStartupScript(this, GetType(), "SetBackgroundImage" + (m_nFunctionIndex++).ToString(), strScript, true);

            strScript = "SetImage(\"mapTitle\", \"./Images/txt_spot_on.png\");";
            ScriptManager.RegisterStartupScript(this, GetType(), "SetImage" + (m_nFunctionIndex++).ToString(), strScript, true);

            strScript = "SetBackgroundImage(\"rightBox\", \"url('../Images/icon_spot_on.png') no-repeat 8px top\");";
            ScriptManager.RegisterStartupScript(this, GetType(), "SetBackgroundImage" + (m_nFunctionIndex++).ToString(), strScript, true);

            strScript = "SetBorderColor(\"droneImage\", \"5px solid red\");";
            ScriptManager.RegisterStartupScript(this, GetType(), "SetBorderColor" + (m_nFunctionIndex++).ToString(), strScript, true);
            //droneImage.BorderColor = System.Drawing.Color.Red;

            strScript = "SetBorderColor(\"map\", \"5px solid red\");";
            ScriptManager.RegisterStartupScript(this, GetType(), "SetBorderColor" + (m_nFunctionIndex++).ToString(), strScript, true);

            strScript = "SetBorderColor(\"disasterImage\", \"5px solid red\");";
            ScriptManager.RegisterStartupScript(this, GetType(), "SetBorderColor" + (m_nFunctionIndex++).ToString(), strScript, true);

            strScript = "SetClass(\"btnReset\", \"btn_reset\");";
            ScriptManager.RegisterStartupScript(this, GetType(), "SetClass" + (m_nFunctionIndex++).ToString(), strScript, true);

            strScript = "SetFontColor(\"LeftBox\", \"red\");";
            ScriptManager.RegisterStartupScript(this, GetType(), "SetFontColor" + (m_nFunctionIndex++).ToString(), strScript, true);

            strScript = "SetFontColor(\"RightBox\", \"red\");";
            ScriptManager.RegisterStartupScript(this, GetType(), "SetFontColor" + (m_nFunctionIndex++).ToString(), strScript, true);
            /*btnInit.ImageUrl = "./Images/btn_on.png";
            btnInit.CssClass = "btn_resetEnabled";
            btnInit.Enabled = true;*/
        }

        /*protected void btnInit_Click(object sender, ImageClickEventArgs e)
        {
            using (Service.SmartEyeService service = new Service.SmartEyeService())
            {
                service.EndDisaster(0);
            }
        }*/
    }
}