using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility;

namespace UnE.SOP.Workstate
{
    public class WorkflowOption
    {
        protected const string USER_DEFINED_PARAMETER_TAG = "UserDefinedParam";

        protected bool m_useSmsMessage = true;
        protected VariousData<DateTime> m_dtDetect = null;
        protected bool m_useCurrentTime = true;
        protected bool m_useShelters = false;
        protected List<UnE.Spatial.Shelter> m_usingShelters = new List<UnE.Spatial.Shelter>();
        protected bool m_hasPosition = false;
        protected string m_strPosition = "";
        protected string m_strBroadcastPosition = "";
        protected HistoryDisasterPosition mLastPoistion = null;
        protected string m_strDisasterName = "";
        protected WorkFlow m_workFlow = null;
        // SOP에서 사용된 사용자 정의 변수에 대한 값들
        // double이나 int의 경우 string을 Parsing하면 되고, boolean의 경우 "1" 또는 "0"의 값을 갖는다.
        protected Dictionary<SOPParameter, string> m_userDefinedParameters = new Dictionary<SOPParameter, string>();

        private string m_strWorkFlowOptionType = "FIRE";
        public virtual string GetTypeString()
        {
            return m_strWorkFlowOptionType;
        }

        protected int m_nSensorZoneID = -1;
        public int SensorZoneID
        {
            get { return m_nSensorZoneID; }
            set { m_nSensorZoneID = value; }
        }

        protected int m_nSensorZoneHistoryID = -1;
        public int SensorZoneHistoryID
        {
            get { return m_nSensorZoneHistoryID; }
            set { m_nSensorZoneHistoryID = value; }
        }


        public bool UseCurrentTime
        {
            get { return m_useCurrentTime; }
            set { m_useCurrentTime = value; }
        }

        public bool UseSmsMessage
        {
            get { return m_useSmsMessage; }
            set { m_useSmsMessage = value; }
        }

        public VariousData<DateTime> DetectTime
        {
            get { return m_dtDetect; }
            set { m_dtDetect = value; }
        }

        public bool UseShelters
        {
            get { return m_useShelters; }
            set { m_useShelters = value; }
        }

        public List<UnE.Spatial.Shelter> UsingShelters
        {
            get { return m_usingShelters; }
            set { m_usingShelters = value; }
        }

        public bool HasPosition
        {
            get { return m_hasPosition; }
            set { m_hasPosition = value; }
        }

        public string PositionName
        {
            get { return m_strPosition; }
            set { m_strPosition = value; }
        }

        public string BroadcastPositionName
        {
            get { return m_strBroadcastPosition; }
            set { m_strBroadcastPosition = value; }
        }

        public HistoryDisasterPosition LastPosition
        {
            get { return mLastPoistion; }
            set { mLastPoistion = value; }//SetDisasterPosition(value); }
        }

        public string DisasterName
        {
            get { return m_strDisasterName; }
        }

        public WorkFlow WorkFlow
        {
            get { return m_workFlow; }
            set { m_workFlow = value; }
        }

        public Dictionary<SOPParameter, string> UserDefinedParameters
        {
            get { return m_userDefinedParameters; }
        }

        public virtual UnE.Spatial.Shelter.ShelterTypes ShelterType
        {
            // 화재 대피소가 모든 대피소의 기본
            get { return UnE.Spatial.Shelter.ShelterTypes.Fire; }
        }

        public virtual string GetDisasterTypeString()
        {
            string strDisasterType = "";

            foreach (KeyValuePair<SOPParameter, string> pair in m_userDefinedParameters)
            {
                string strData = string.Format("[{0}:{1}_{2}_{3}/{4}]",
                    m_strDisasterName,
                    USER_DEFINED_PARAMETER_TAG,
                    global::Sections.SectionDataDecision.GetVariableTypeName(pair.Key.Type, false),
                    pair.Key.VariableName,
                    pair.Value);

                if (strDisasterType.Length == 0)
                    strDisasterType = strData;
                else
                    strDisasterType += ";" + strData;
            }

            return strDisasterType;
        }

        /*protected virtual void SetDisasterPosition(HistoryDisasterPosition pos)
        {
            mLastPoistion = pos;
        }*/

        protected bool GetDisasterTypeInfo(string strDisasterType, out string strDisasterName, out string strTagName, out string strTagValue)
        {
            strDisasterName = strTagName = strTagValue = "";

            int nIndex1 = strDisasterType.IndexOf('[');
            int nIndex2 = strDisasterType.LastIndexOf(']');

            if (nIndex1 < 0 || nIndex2 <= nIndex1)
                return false;

            int nIndex3 = strDisasterType.IndexOf(':', nIndex1 + 1);

            if (nIndex3 < 0)
                return false;

            int nIndex4 = strDisasterType.IndexOf('/', nIndex3 + 1);

            if (nIndex4 < 0)
                return false;

            strDisasterName = strDisasterType.Substring(nIndex1 + 1, nIndex3 - nIndex1 - 1).Trim();
            strTagName = strDisasterType.Substring(nIndex3 + 1, nIndex4 - nIndex3 - 1).Trim();
            strTagValue = strDisasterType.Substring(nIndex4 + 1, nIndex2 - nIndex4 - 1).Trim();
            return true;
        }

        public virtual void SetDisasterOptions(string strDisasterOptions)
        {
            string[] tokens = strDisasterOptions.Split(';');
            string strUserDefinedTag = "UserDefinedParam";

            foreach (string strToken in tokens)
            {
                int nIndex = strToken.IndexOf(strUserDefinedTag);

                if (nIndex >= 0)
                {
                    string[] parameters = strToken.Split('_');

                    if (parameters.Count() == 3)
                    {
                        string strParamType = parameters[1].Trim();
                        int nIndex2 = parameters[2].IndexOf('/');

                        if (nIndex2 > 0)
                        {
                            string strParamName = parameters[2].Substring(0, nIndex2).Trim();
                            string strParamValue = parameters[2].Substring(nIndex2 + 1).Trim();

                            if (strParamValue.EndsWith("]"))
                                strParamValue = strParamValue.Substring(0, strParamValue.Length - 1).Trim();

                            SOPParameter param = new SOPParameter();
                            param.Type = global::Sections.SectionDataDecision.ToVariableType(strParamType);
                            param.VariableName = strParamName;

                            m_userDefinedParameters[param] = strParamValue;
                        }
                    }
                }
            }
        }

        protected bool IsUserDefinedParameter(string strTagName)
        {
            return strTagName.StartsWith(USER_DEFINED_PARAMETER_TAG);
        }

        protected void SetUserDefinedParameters(string strTagName, string strTagValue)
        {
            int nIndex1 = strTagName.IndexOf('_');

            if (nIndex1 < 0)
                return;

            int nIndex2 = strTagName.IndexOf('_', nIndex1 + 1);

            if (nIndex2 < 0)
                return;

            string strVariableType = strTagName.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
            global::Sections.SectionDataDecision.VariableType type = global::Sections.SectionDataDecision.ToVariableType(strVariableType);

            if (type == global::Sections.SectionDataDecision.VariableType.UNKNOWN)
                return;

            if (nIndex2 == strTagName.Length - 1)
                return;

            string strVariableName = strTagName.Substring(nIndex2 + 1);

            SOPParameter param = new SOPParameter();
            param.VariableName = strVariableName;
            param.Type = type;

            m_userDefinedParameters[param] = strTagValue;
        }
    }

    public class WorkflowOptionEarthquake : WorkflowOption
    {
        public enum PowerMode { Unknown = 0, Magnitude, Intensity };

        private double m_dMagnitude = 0.0;
        private int m_nIntensity = 0;
        private PowerMode m_mode = PowerMode.Unknown;

        // 규모
        public double Magnitude
        {
            get { return m_dMagnitude; }
            set { m_dMagnitude = value; }
        }

        // 진도
        public int Intensity
        {
            get { return m_nIntensity; }
            set { m_nIntensity = value; }
        }

        public PowerMode Mode
        {
            get { return m_mode; }
            set { m_mode = value; }
        }

        public WorkflowOptionEarthquake()
        {
            m_strDisasterName = "지진";
        }

        public override string GetDisasterTypeString()
        {
            string strDisasterType = "";

            if (m_mode == PowerMode.Intensity)
                strDisasterType = "[" + m_strDisasterName + ":" + m_mode.ToString() + "/" + m_nIntensity.ToString() + "]";
            else if (m_mode == PowerMode.Magnitude)
                strDisasterType = "[" + m_strDisasterName + ":" + m_mode.ToString() + "/" + m_dMagnitude.ToString() + "]";

            string strUserDefinedParams = base.GetDisasterTypeString();

            if (strDisasterType.Length == 0)
                strDisasterType = strUserDefinedParams;
            else if (strUserDefinedParams.Length > 0)
                strDisasterType += ";" + strUserDefinedParams;

            return strDisasterType;
        }

        public override void SetDisasterOptions(string strDisasterOptions)
        {
            base.SetDisasterOptions(strDisasterOptions);

            string[] tokens = strDisasterOptions.Split(';');

            int nTokenCount = tokens.Count();
            string strDisasterName, strTagName, strTagValue;

            for (int i=0;i<nTokenCount;i++)
            {
                if (GetDisasterTypeInfo(tokens[i], out strDisasterName, out strTagName, out strTagValue))
                {
                    if (IsUserDefinedParameter(strTagName))
                    {
                        SetUserDefinedParameters(strTagName, strTagValue);
                    }
                    else
                    {
                        PowerMode mode = ToPowerMode(strTagName);

                        if (mode == PowerMode.Intensity)
                        {
                            int nIntensity;

                            if (int.TryParse(strTagValue, out nIntensity))
                            {
                                m_mode = mode;
                                m_nIntensity = nIntensity;
                            }
                        }
                        else if (mode == PowerMode.Magnitude)
                        {
                            double dMagnitude;

                            if (double.TryParse(strTagValue, out dMagnitude))
                            {
                                m_mode = mode;
                                m_dMagnitude = dMagnitude;
                            }
                        }
                    }
                }
            }
        }

        public static PowerMode ToPowerMode(string strMode)
        {
            foreach (PowerMode mode in Enum.GetValues(typeof(PowerMode)))
            {
                if (string.Compare(strMode, mode.ToString(), true) == 0)
                    return mode;
            }

            return PowerMode.Unknown;
        }

        public override UnE.Spatial.Shelter.ShelterTypes ShelterType
        {
            get { return UnE.Spatial.Shelter.ShelterTypes.Earthquake; }
        }
                
        public override string GetTypeString()
        {
            return "Earthquake";
        }
    }

    public class WorkflowOptionSnowFall : WorkflowOption
    {
        private bool m_useAmountSnowFall = true;
        // 적설량(cm)
        private double m_dAmountSnowFall = 0.0;

        public bool UseAmountSnowFall
        {
            get { return m_useAmountSnowFall; }
            set { m_useAmountSnowFall = value; }
        }

        public double AmountSnowFall
        {
            get { return m_dAmountSnowFall; }
            set { m_dAmountSnowFall = value; }
        }

        public WorkflowOptionSnowFall()
        {
            m_strDisasterName = "폭설";
        }

        public override string GetDisasterTypeString()
        {
            string strDisasterType = "";

            if (m_useAmountSnowFall)
                strDisasterType = "[" + m_strDisasterName + ":적설량/" + m_dAmountSnowFall.ToString() + "]";

            string strUserDefinedParams = base.GetDisasterTypeString();

            if (strDisasterType.Length == 0)
                strDisasterType = strUserDefinedParams;
            else if (strUserDefinedParams.Length > 0)
                strDisasterType += ";" + strUserDefinedParams;

            return strDisasterType;
        }

        public override void SetDisasterOptions(string strDisasterOptions)
        {
            base.SetDisasterOptions(strDisasterOptions);

            string[] tokens = strDisasterOptions.Split(';');

            int nTokenCount = tokens.Count();
            string strDisasterName, strTagName, strTagValue;

            for (int i = 0; i < nTokenCount; i++)
            {
                if (GetDisasterTypeInfo(tokens[i], out strDisasterName, out strTagName, out strTagValue))
                {
                    if (IsUserDefinedParameter(strTagName))
                    {
                        SetUserDefinedParameters(strTagName, strTagValue);
                    }
                    else
                    {
                        if (strTagName == "적설량")
                        {
                            double dAmount;

                            if (double.TryParse(strTagValue, out dAmount))
                            {
                                m_useAmountSnowFall = true;
                                m_dAmountSnowFall = dAmount;
                            }
                        }
                    }
                }
            }
        }

        public override UnE.Spatial.Shelter.ShelterTypes ShelterType
        {
            // 폭설은 따로 대피소가 없음.
            get { return UnE.Spatial.Shelter.ShelterTypes.None; }
        }

        public override string GetTypeString()
        {
            return "SnowFall";
        }
    }

    public class WorkflowOptionPSM : WorkflowOption
    {
        private PSMMaterial m_material = null;
        // 유해화학물질 누출시 대피거리(미터)
        private int m_nPSMDistance = 0;

        public PSMMaterial PSMMaterial
        {
            get { return m_material; }
            set { m_material = value; }
        }

        public int PSMDistance
        {
            get { return m_nPSMDistance; }
            set { m_nPSMDistance = value; }
        }

        public override UnE.Spatial.Shelter.ShelterTypes ShelterType
        {
            get { return UnE.Spatial.Shelter.ShelterTypes.PSM; }
        }

        public WorkflowOptionPSM()
        {
            HasPosition = true;
            m_strDisasterName = "유출사고";
        }

        public override string GetDisasterTypeString()
        {
            string strDisasterType = "";

            if (m_material != null)
                strDisasterType = "[" + m_strDisasterName + ":" + m_material.MaterialName + "/" + m_nPSMDistance.ToString() + "]";

            string strUserDefinedParams = base.GetDisasterTypeString();

            if (strDisasterType.Length == 0)
                strDisasterType = strUserDefinedParams;
            else if (strUserDefinedParams.Length > 0)
                strDisasterType += ";" + strUserDefinedParams;

            return strDisasterType;
        }

        public void SetMaterial(string szMaterialName)
        {

        }

        public override void SetDisasterOptions(string strDisasterOptions)
        {
            base.SetDisasterOptions(strDisasterOptions);

            string[] tokens = strDisasterOptions.Split(';');

            int nTokenCount = tokens.Count();
            string strDisasterName, strTagName, strTagValue;

            for (int i = 0; i < nTokenCount; i++)
            {
                if (GetDisasterTypeInfo(tokens[i], out strDisasterName, out strTagName, out strTagValue))
                {
                    if (IsUserDefinedParameter(strTagName))
                    {
                        SetUserDefinedParameters(strTagName, strTagValue);
                    }
                    else
                    {
                        PSMMaterial material = LoadPSMMaterial(strTagName);

                        if (material != null)
                        {
                            int nDistance;

                            if (int.TryParse(strTagValue, out nDistance))
                            {
                                m_material = material;
                                m_nPSMDistance = nDistance;
                            }
                        }
                    }
                }
            }

          
        }

        private PSMMaterial LoadPSMMaterial(string strMaterialName)
        {
            string strSQL = "Select ID, UOM, PageNo, EvacInitDistance, EvacDayDistance, EvacNightDistance from PSMMaterial where MaterialName = '" + strMaterialName + "'";
            System.Collections.ArrayList arrResult = UnE.SOP.ProxySOP.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;

            if (nResultCount >= 6)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());
                string strUOM = WebDBManager.GetStringField(arrResult[1]);
                VariousData<int> pageNo = WebDBManager.GetIntField(arrResult[2].ToString());
                VariousData<int> initDistance = WebDBManager.GetIntField(arrResult[3].ToString());
                VariousData<int> dayDistance = WebDBManager.GetIntField(arrResult[4].ToString());
                VariousData<int> nightDistance = WebDBManager.GetIntField(arrResult[5].ToString());

                if (id != null && strUOM != null)
                {
                    PSMMaterial material = new PSMMaterial();

                    material.MaterialID = id.Data;
                    material.MaterialName = strMaterialName;

                    if (initDistance != null)
                        material.InitDistance = initDistance.Data;

                    if (dayDistance != null)
                        material.DayDistance = dayDistance.Data;

                    if (nightDistance != null)
                        material.NightDistance = nightDistance.Data;

                    return material;
                }
            }

            return null;
        }
    }
}
