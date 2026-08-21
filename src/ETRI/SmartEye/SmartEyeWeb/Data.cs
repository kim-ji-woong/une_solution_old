using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartEyeWeb
{
    public enum ImageResult { NO_IMAGE = 0, DISASTER_IMAGE, REAL_TIME_IMAGE };

    public class ResultData
    {
        private ImageResult m_result = ImageResult.NO_IMAGE;
        private string m_strImageURL = "";
        private string m_strLatitude = "", m_strLongitude = "";
        private DateTime m_dtImageTime = new DateTime();
        private string m_strDescription = "";
        private string m_strResultText = "";

        public ImageResult ImageResult
        {
            get { return m_result; }
            set { m_result = value; }
        }

        public string ImageURL
        {
            get { return m_strImageURL; }
            set { m_strImageURL = value; }
        }

        public string Latitude
        {
            get { return m_strLatitude; }
            set { m_strLatitude = value; }
        }

        public string Longitude
        {
            get { return m_strLongitude; }
            set { m_strLongitude = value; }
        }

        public DateTime ImageTime
        {
            get { return m_dtImageTime; }
            set { m_dtImageTime = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public string ResultText
        {
            get { return m_strResultText; }
            set { m_strResultText = value; }
        }

        public static bool GetGPSCoords(out double latitude, out double longitude, string strLatitude, string strLongitude)
        {
            latitude = longitude = 0.0;

            if (double.TryParse(strLatitude, out latitude) && double.TryParse(strLongitude, out longitude))
                return true;

            return false;
        }

        public static List<string> ParseTagDatas(string str, List<string> values)
        {
            List<string> tags = new List<string>();

            string[] datas = str.Split(';');

            foreach (string data in datas)
            {
                string[] tokens = data.Split(':');

                if (tokens.Count() != 2)
                    continue;

                string strTag = tokens[0].Trim();
                string strValue = tokens[1].Trim();

                tags.Add(strTag);
                values.Add(strValue);
            }

            return tags;
        }
    }

    public class DisasterResult : ResultData
    {
        private int m_nDisasterID = 0;
        private string m_strWatcherName = "";
        private string m_strLocationName = "";
        private string m_strEtc = "";
        private DateTime m_dtDisasterTime = new DateTime();

        public int DisasterID
        {
            get { return m_nDisasterID; }
            set { m_nDisasterID = value; }
        }

        public string WatcherName
        {
            get { return m_strWatcherName; }
            set { m_strWatcherName = value; }
        }

        public string LocationName
        {
            get { return m_strLocationName; }
            set { m_strLocationName = value; }
        }

        public string Etc
        {
            get { return m_strEtc; }
            set { m_strEtc = value; }
        }

        public DateTime DisasterTime
        {
            get { return m_dtDisasterTime; }
            set { m_dtDisasterTime = value; }
        }

        public DisasterResult()
        {
            ImageResult = SmartEyeWeb.ImageResult.DISASTER_IMAGE;
        }

        public DisasterResult(string strImageURL, string strLatitude, string strLongitude, DateTime dtImageTime, string strDescription, string strResultText, int nDisasterID, string strWatcherName, string strLocationName, string strEtc, DateTime dtDisasterTime)
        {
            ImageResult = SmartEyeWeb.ImageResult.DISASTER_IMAGE;

            ImageURL = strImageURL;
            Latitude = strLatitude;
            Longitude = strLongitude;
            ImageTime = dtImageTime;
            Description = strDescription;
            ResultText = strResultText;

            m_nDisasterID = nDisasterID;
            m_strWatcherName = strWatcherName;
            m_strLocationName = strLocationName;
            m_strEtc = strEtc;
            m_dtDisasterTime = dtDisasterTime;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is DisasterResult)
            {
                DisasterResult result = (DisasterResult)obj;

                if (result.ImageURL != this.ImageURL)
                    return false;

                if (result.Latitude != this.Latitude)
                    return false;

                if (result.Longitude != this.Longitude)
                    return false;

                if (result.ImageTime != this.ImageTime)
                    return false;

                if (result.Description != this.Description)
                    return false;

                if (result.ResultText != this.ResultText)
                    return false;

                if (result.DisasterID != this.DisasterID)
                    return false;

                if (result.WatcherName != this.WatcherName)
                    return false;

                if (result.LocationName != this.LocationName)
                    return false;

                if (result.Etc != this.Etc)
                    return false;

                if (result.DisasterTime != this.DisasterTime)
                    return false;

                return true;
            }

            return false;
        }
    }

    public class RealTimeResult : ResultData
    {
        public RealTimeResult()
        {
            ImageResult = SmartEyeWeb.ImageResult.REAL_TIME_IMAGE;
        }

        public RealTimeResult(string strImageURL, string strLatitude, string strLongitude, DateTime dtImageTime, string strDescription, string strResultText)
        {
            ImageResult = SmartEyeWeb.ImageResult.REAL_TIME_IMAGE;

            ImageURL = strImageURL;
            Latitude = strLatitude;
            Longitude = strLongitude;
            ImageTime = dtImageTime;
            Description = strDescription;
            ResultText = strResultText;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is RealTimeResult)
            {
                RealTimeResult result = (RealTimeResult)obj;

                if (result.ImageURL != this.ImageURL)
                    return false;

                if (result.Latitude != this.Latitude)
                    return false;

                if (result.Longitude != this.Longitude)
                    return false;

                if (result.ImageTime != this.ImageTime)
                    return false;

                if (result.Description != this.Description)
                    return false;

                if (result.ResultText != this.ResultText)
                    return false;

                return true;
            }

            return false;
        }
    }

    public class ActionData
    {
        public enum ActionStep { NONE = -1, 수집 = 0, 분석, 예측, 시각화, 경보,  대응 };

        private ActionStep m_actionStep = ActionStep.NONE;
        private int m_nDisasterID = 0;
        private string m_strDescription = "";

        public ActionStep Current
        {
            get { return m_actionStep; }
            set { m_actionStep = value; }
        }

        public int DisasterID
        {
            get { return m_nDisasterID; }
            set { m_nDisasterID = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public ActionData()
        {
        }

        public ActionData(ActionStep actionStep, int nDisasterID, string strDescription)
        {
            SetData(actionStep, nDisasterID, strDescription);
        }

        public ActionData(int nActionStep, int nDisasterID, string strDescription)
        {
            SetData(ToActionStep(nActionStep), nDisasterID, strDescription);
        }

        public void SetData(ActionStep actionStep, int nDisasterID, string strDescription)
        {
            m_actionStep = actionStep;
            m_nDisasterID = nDisasterID;
            m_strDescription = strDescription;
        }

        public static ActionStep ToActionStep(int nActionStep)
        {
            if (nActionStep < 0 || nActionStep > (int)ActionStep.대응)
                return ActionStep.NONE;

            return (ActionStep)nActionStep;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is ActionData)
            {
                ActionData action = (ActionData)obj;

                if (this.Current != action.Current)
                    return false;

                if (this.DisasterID != action.DisasterID)
                    return false;

                if (this.Description != action.Description)
                    return false;

                return true;
            }

            return false;
        }
    }
}