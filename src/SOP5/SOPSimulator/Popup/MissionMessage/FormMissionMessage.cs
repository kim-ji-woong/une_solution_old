using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using DBUtility;

namespace SOPMonitoringSystem.Popup.MissionMessage
{
    public partial class FormMissionMessage : Form
    {
        public enum MessageType { DisasterType, System, UserInput, Scenario, 주의, 청색, 적색, 긴급, None };

        public class LocationString
        {
            private string m_strLocation = "";

            public string Location
            {
                get { return m_strLocation; }
                set { m_strLocation = value; }
            }

            public LocationString()
            {
            }

            public LocationString(string strLocation)
            {
                m_strLocation = strLocation;
            }
        }

        public class StartTimeString
        {
            private string m_strStartTime = "";

            public string StartTime
            {
                get { return m_strStartTime; }
                set { m_strStartTime = value; }
            }

            public StartTimeString()
            {
            }

            public StartTimeString(string strStartTime)
            {
                m_strStartTime = strStartTime;
            }
        }

        public class ComboBoxData
        {
            private object m_data = null;
            private string m_strText = "";

            public string Text
            {
                get { return m_strText; }
                set { m_strText = value; }
            }

            public object Data
            {
                get { return m_data; }
                set { m_data = value; }
            }

            public ComboBoxData()
            {
            }

            public ComboBoxData(string strText, object data = null)
            {
                m_strText = strText;
                m_data = data;
            }

            public override string ToString()
            {
                return m_strText;
            }
        }

        public abstract class Disaster
        {
            protected string m_strLocation = "";
            // null이면 SOP 시작 시간
            protected VariousData<DateTime> m_dtStart = null;
            protected bool m_isReal = false;
            protected string m_strStepName = "";
            
            //protected Dictionary<MessageType, string> m_dicFullMessage = new Dictionary<MessageType, string>();
            protected Dictionary<MessageType, ArrayList> m_dicMessageString = new Dictionary<MessageType, ArrayList>();

            protected FormMissionMessage m_parent = null;

            public string Location
            {
                get { return m_strLocation; }
                set { m_strLocation = value; }
            }

            // null이면 SOP 시작 시간
            public VariousData<DateTime> StartTime
            {
                get { return m_dtStart; }
                set { m_dtStart = value; }
            }

            public bool RealMode
            {
                get { return m_isReal; }
                set { m_isReal = value; }
            }

            public FormMissionMessage Parent
            {
                get { return m_parent; }
                set { m_parent = value; }
            }

            /*public string GetFullMessage(MessageType type)
            {
                string strMessage = null;

                if (m_dicFullMessage.TryGetValue(type, out strMessage))
                    return strMessage;

                return null;
            }*/

            public void SetupScenarioMessage(string strMessage)
            {
                ArrayList strings = new ArrayList();
                strings.Add(strMessage);

                m_dicMessageString[MessageType.Scenario] = strings;
            }

            protected virtual void SetupUserInputMessage()
            {
                m_dicMessageString[MessageType.UserInput] = new ArrayList();
            }

            protected virtual string GetUserInputMessage()
            {
                ArrayList strings = null;

                if (!m_dicMessageString.TryGetValue(MessageType.UserInput, out strings))
                    return "";

                string strMessage = "";
                int nCount = strings.Count;

                for (int i=0;i<nCount;i++)
                {
                    if (strings[i] is string)
                    {
                        strMessage += strings[i].ToString();
                    }
                    else
                        return "";
                }

                return strMessage;
            }

            protected virtual string GetScenarioMessage(string strLocation, VariousData<DateTime> dtStart, bool isRealMode, string strPSMMaterialName, int nPSMDistance, string strAmountSnowfall)
            {
                ArrayList strings = null;

                if (!m_dicMessageString.TryGetValue(MessageType.Scenario, out strings))
                    return "";

                string strMsg = "";
                int nCount = strings.Count;

                for (int i = 0; i < nCount; i++ )
                {
                    if (strings[i] is string)
                        strMsg += strings[i].ToString();
                    else
                        return "";
                }

                // 아직 SOP가 실행되지 않았으면 원본 Text를 그대로 보여준다.
                if (m_parent != null && m_parent.RunMode == false)
                    return strMsg;

                DateTime dtTime = dtStart == null ? DateTime.Now : dtStart.Data;
                //return UnE.SOP.Utility.SOPSimulatorScript.Parse(strMsg, dtTime, strLocation, isRealMode ? 1 : 0);
                UnE.SOP.Utility.SOPSimulatorScript.DataParameter param = new UnE.SOP.Utility.SOPSimulatorScript.DataParameter(strMsg, dtTime, strLocation);
                param.RealMode = isRealMode ? 1 : 0;

                if (strPSMMaterialName != null && strPSMMaterialName.Length > 0)
                {
                    param.PSMMaterialType = strPSMMaterialName;
                    param.PSMDistance = nPSMDistance;
                }

                if (strAmountSnowfall != null && strAmountSnowfall.Length > 0)
                    param.AmountSnowfall = strAmountSnowfall;

                return UnE.SOP.Utility.SOPSimulatorScript.Parse(param);
            }

            public virtual void SetMessage(MessageType type, string strMsg)
            {
                ArrayList strings = null;

                if (!m_dicMessageString.TryGetValue(type, out strings))
                {
                    strings = new ArrayList();
                    m_dicMessageString[type] = strings;
                }

                if (strings.Count == 0)
                {
                    //strings.Clear();
                    strings.Add(strMsg);
                }
            }

            public abstract string GetMessage(string strLocation, VariousData<DateTime> dtStart, MessageType type, bool isRealMode, string strPSMMaterialName, int nPSMDistance, string strAmountSnowfall);
            public abstract void SetComboBox(ComboBox cbo);
        }

        public class Fire : Disaster
        {
            public Fire(string strStepName)
            {
                m_strStepName = strStepName;
                SetupMessage();
            }

            private void SetupMessage()
            {
                SetupDisasterTypeMessage();
                SetupSystemMessage();
                SetupUserInputMessage();
            }

            private void SetupDisasterTypeMessage()
            {
                LocationString location = new LocationString("[재난발생위치]");

                ArrayList strings = new ArrayList();

                strings.Add("본부 재난안전대책본부에서 ");
                strings.Add("금일 현재시각 ");
                strings.Add(location);
                strings.Add("에서 ()가 발생되어 ()로 확산되고 있습니다.\r\n 지금 즉시 케이블 TV 채널 2번으로 비상상황을 청취, 비상 체제로 임해 주시기 바라며, 필수 발전운전 근무자를 제외한 전 직원의 비상동원을 발령합니다.\r\n주변에 있는 방제장비를 지참하고 ");
                strings.Add(location);
                strings.Add("로 신속하게 출동하여 현장 통제반의 지시에 따라 임무를 수행하시기 바랍니다.");

                m_dicMessageString[MessageType.DisasterType] = strings;
            }

            private void SetupSystemMessage()
            {
                LocationString location = new LocationString("[재난발생위치]");
                StartTimeString startTime = new StartTimeString("[재난발생시간]");

                ArrayList strings = new ArrayList();

                strings.Add(startTime);
                strings.Add(" 화재 " + m_strStepName + " SOP 상황이 시작되었습니다. 발생 위치는 ");
                strings.Add(location);
                strings.Add("입니다.");
            }

            public override string GetMessage(string strLocation, VariousData<DateTime> dtStart, MessageType type, bool isRealMode, string strPSMMaterialName, int nPSMDistance, string strAmountSnowfall)
            {
                if (type == MessageType.DisasterType)
                    return GetDisasterTypeMessage(strLocation, dtStart, isRealMode);
                else if (type == MessageType.System)
                    return GetSystemMessage(strLocation, dtStart, isRealMode);
                else if (type == MessageType.UserInput)
                    return GetUserInputMessage();
                else if (type == MessageType.Scenario)
                    return GetScenarioMessage(strLocation, dtStart, isRealMode, strPSMMaterialName, nPSMDistance, strAmountSnowfall);

                return "";
            }

            private string GetSystemMessage(string strLocation, VariousData<DateTime> dtStart, bool isRealMode)
            {
                ArrayList strings = null;

                if (!m_dicMessageString.TryGetValue(MessageType.System, out strings))
                    return "";

                int nCount = strings.Count;
                string strMessage = "";

                for (int i = 0; i < nCount; i++)
                {
                    if (i == 0 && strings[i] is StartTimeString)
                    {
                        if (isRealMode)
                            strMessage = "실제 상황입니다. ";
                        else
                            strMessage = "모의훈련 상황입니다. ";
                    }
                        
                    if (strings[i] is string)
                        strMessage += strings[i].ToString();
                    else if (strings[i] is LocationString)
                        strMessage += strLocation;
                    else if (strings[i] is StartTimeString)
                    {
                        if (dtStart == null)
                            strMessage += "금일 현재시각";
                        else if (dtStart.Data.Year == DateTime.Now.Year && dtStart.Data.Month == DateTime.Now.Month && dtStart.Data.Day == DateTime.Now.Day)
                            strMessage += "금일 " + dtStart.Data.Hour.ToString() + "시 " + dtStart.Data.Minute.ToString() + "분";
                        else
                            strMessage += dtStart.Data.Month.ToString() + "월 " + dtStart.Data.Day.ToString() + "일 " + dtStart.Data.Hour.ToString() + "시 " + dtStart.Data.Minute.ToString() + "분";
                    }
                    else
                        return "";
                }

                //m_dicFullMessage[MessageType.System] = strMessage;
                return strMessage;
            }

            private string GetDisasterTypeMessage(string strLocation, VariousData<DateTime> dtStart, bool isRealMode)
            {
                ArrayList strings = null;

                if (!m_dicMessageString.TryGetValue(MessageType.DisasterType, out strings))
                    return "";

                int nCount = strings.Count;
                string strMessage = "";

                for (int i=0;i<nCount;i++)
                {
                    if (i == 0 && strings[i] is string)
                    {
                        strMessage = strings[i].ToString();

                        if (isRealMode)
                            strMessage += "실제 비상상황을 알려드립니다.\r\n\r\n";
                        else
                            strMessage += "훈련 비상상황을 알려드립니다.\r\n\r\n";
                    }
                    else
                    {
                        if (strings[i] is string)
                            strMessage += strings[i].ToString();
                        else if (strings[i] is LocationString)
                            strMessage += strLocation;
                        else if (strings[i] is StartTimeString)
                        {
                            if (dtStart == null)
                                strMessage += "금일 현재시각";
                            else if (dtStart.Data.Year == DateTime.Now.Year && dtStart.Data.Month == DateTime.Now.Month && dtStart.Data.Day == DateTime.Now.Day)
                                strMessage += "금일 " + dtStart.Data.Hour.ToString() + "시 " + dtStart.Data.Minute.ToString() + "분";
                            else
                                strMessage += dtStart.Data.Month.ToString() + "월 " + dtStart.Data.Day.ToString() + "일 " + dtStart.Data.Hour.ToString() + "시 " + dtStart.Data.Minute.ToString() + "분";
                        }
                        else
                            return "";
                    }
                }

                //m_dicFullMessage[MessageType.DisasterType] = strMessage;
                return strMessage;
            }

            public override void SetComboBox(ComboBox cbo)
            {
                cbo.Items.Clear();

                cbo.Items.Add(new ComboBoxData("화재, 유출사고(비상상황)", MessageType.DisasterType));
                cbo.Items.Add(new ComboBoxData("기타(시스템 지정)", MessageType.System));
                cbo.Items.Add(new ComboBoxData("사용자 입력", MessageType.UserInput));
                cbo.Items.Add(new ComboBoxData("시나리오", MessageType.Scenario));

                cbo.SelectedIndex = 3;
            }
        }

        public class Typhoon : Disaster
        {
            public Typhoon(string strStepName)
            {
                m_strStepName = strStepName;
                SetupMessage();
            }

            private void SetupMessage()
            {
                Setup주의Message();
                Setup청색Message();
                Setup적색Message();
                Setup긴급Message();
                SetupSystemMessage();
                SetupUserInputMessage();
            }

            private void Setup주의Message()
            {
                ArrayList strings = new ArrayList();

                strings.Add("본부 재난안전대책본부에서 ");
                strings.Add("현재 폭풍 주의보가 발효 되어  호우로 인한 피해가 예상 되오니, 전 직원은 다음 사항을 확인하여 피해가 없도록 철저히 대비하여 주시기 바랍니다.\r\n");
                strings.Add("1. 소화기 위치를 잘 숙지하시기 바랍니다.\r\n");
                strings.Add("2. 출입문 한 곳을 제외한 모든 출입문과 창문을 완전히 닫아 주시기 바랍니다.\r\n");
                strings.Add("3. 해안, 위험구현 가까이 주차한 차량은 피하가 없도록 안전한 장소로 이동해 주시기 바랍니다.\r\n");
                strings.Add("4. 중점관리 시설물에 대한 점검을 철저히 시행해 주시기 바랍니다.\r\n");
                strings.Add("5. 각 부서에서 관리하는 비상전원 설비, 회사장, 방파제, 부두 시설물, 석탄 취급 설비, 취수구, 파워블록 외벽, 오폐수처리설비, 배수펌프 작동 상태를 다시 한번 점검하시고, \r\n");
                strings.Add("6. 야적 자재의 유실 방지를 위한 보강조치를 확인하시기 바랍니다.");

                m_dicMessageString[MessageType.주의] = strings;
            }

            private void Setup청색Message()
            {
                ArrayList strings = new ArrayList();

                strings.Add("본부 재난안전대책본부에서 ");
                strings.Add("금일 현재시각 폭풍으로 인한 기상경보 발효로  청색 비상을 발령합니다.\r\n");
                strings.Add("전 직원은 부서별 비상대응 책무를 수행하시기 바랍니다.");

                m_dicMessageString[MessageType.청색] = strings;
            }

            private void Setup적색Message()
            {
                ArrayList strings = new ArrayList();

                strings.Add("본부 재난안전대책본부에서 ");
                strings.Add("금일 현재시각 태풍으로 인한 기상경보 발효로  적색 비상을 발령합니다.\r\n");
                strings.Add("전 직원은 부서별 비상대응 책무를 수행하시기 바랍니다.");

                m_dicMessageString[MessageType.적색] = strings;
            }

            private void Setup긴급Message()
            {
                ArrayList strings = new ArrayList();

                strings.Add("본부 재난안전대책본부에서 ");
                strings.Add("금일 현재시각 태풍 경보가 발효 되었습니다. \r\n");
                strings.Add("전 직원은 건물 내에서 대기하며 비상상황에 대응해 주시기 바라며, 해안설비, 저탄장, 부두시설, 취수로 등의 강풍, 해일 위험 지역 접근을 금지합니다.\r\n");
                strings.Add("사무실에 근무하고 계신 분은 지금 즉시 현장에서 신속히 대피할 수 있도록 긴급 연락해 주시기 바랍니다.");

                m_dicMessageString[MessageType.긴급] = strings;
            }

            private void SetupSystemMessage()
            {
                StartTimeString startTime = new StartTimeString("[재난발생시간]");

                ArrayList strings = new ArrayList();

                strings.Add(startTime);
                strings.Add(" 태풍 " + m_strStepName + " SOP 상황이 시작되었습니다.");
            }

            public override string GetMessage(string strLocation, VariousData<DateTime> dtStart, MessageType type, bool isRealMode, string strPSMMaterialName, int nPSMDistance, string strAmountSnowfall)
            {
                if (type == MessageType.주의 || type == MessageType.청색 || type == MessageType.적색 || type == MessageType.긴급)
                    return GetDisasterTypeMessage(dtStart, type, isRealMode);
                else if (type == MessageType.System)
                    return GetSystemMessage(dtStart, isRealMode);
                else if (type == MessageType.UserInput)
                    return GetUserInputMessage();
                else if (type == MessageType.Scenario)
                    return GetScenarioMessage(strLocation, dtStart, isRealMode, strPSMMaterialName, nPSMDistance, strAmountSnowfall);

                return "";
            }

            private string GetSystemMessage(VariousData<DateTime> dtStart, bool isRealMode)
            {
                ArrayList strings = null;

                if (!m_dicMessageString.TryGetValue(MessageType.System, out strings))
                    return "";

                int nCount = strings.Count;
                string strMessage = "";

                for (int i = 0; i < nCount; i++)
                {
                    if (i == 0 && strings[i] is StartTimeString)
                    {
                        if (isRealMode)
                            strMessage = "실제 상황입니다. ";
                        else
                            strMessage = "모의훈련 상황입니다. ";
                    }
                        
                    if (strings[i] is string)
                        strMessage += strings[i].ToString();
                    else if (strings[i] is StartTimeString)
                    {
                        if (dtStart == null)
                            strMessage += "금일 현재시각";
                        else if (dtStart.Data.Year == DateTime.Now.Year && dtStart.Data.Month == DateTime.Now.Month && dtStart.Data.Day == DateTime.Now.Day)
                            strMessage += "금일 " + dtStart.Data.Hour.ToString() + "시 " + dtStart.Data.Minute.ToString() + "분";
                        else
                            strMessage += dtStart.Data.Month.ToString() + "월 " + dtStart.Data.Day.ToString() + "일 " + dtStart.Data.Hour.ToString() + "시 " + dtStart.Data.Minute.ToString() + "분";
                    }
                    else
                        return "";
                }

                //m_dicFullMessage[MessageType.System] = strMessage;
                return strMessage;
            }

            private string GetDisasterTypeMessage(VariousData<DateTime> dtStart, MessageType type, bool isRealMode)
            {
                ArrayList strings = null;

                if (!m_dicMessageString.TryGetValue(type, out strings))
                    return "";

                int nCount = strings.Count;
                string strMessage = "";

                for (int i=0;i<nCount;i++)
                {
                    if (i == 0 && strings[i] is string)
                    {
                        strMessage = strings[i].ToString();

                        if (isRealMode)
                            strMessage += "실제 비상상황을 알려드립니다.\r\n\r\n";
                        else
                            strMessage += "훈련 비상상황을 알려드립니다.\r\n\r\n";
                    }
                    else
                    {
                        if (strings[i] is string)
                            strMessage += strings[i].ToString();
                        else if (strings[i] is StartTimeString)
                        {
                            if (dtStart == null)
                                strMessage += "금일 현재시각";
                            else if (dtStart.Data.Year == DateTime.Now.Year && dtStart.Data.Month == DateTime.Now.Month && dtStart.Data.Day == DateTime.Now.Day)
                                strMessage += "금일 " + dtStart.Data.Hour.ToString() + "시 " + dtStart.Data.Minute.ToString() + "분";
                            else
                                strMessage += dtStart.Data.Month.ToString() + "월 " + dtStart.Data.Day.ToString() + "일 " + dtStart.Data.Hour.ToString() + "시 " + dtStart.Data.Minute.ToString() + "분";
                        }
                        else
                            return "";
                    }
                }

                //m_dicFullMessage[type] = strMessage;
                return strMessage;
            }

            public override void SetComboBox(ComboBox cbo)
            {
                cbo.Items.Clear();

                cbo.Items.Add(new ComboBoxData("집중호우 및 태풍(주의예보 단계)", MessageType.주의));
                cbo.Items.Add(new ComboBoxData("집중호우 및 태풍(경보예보(청색)단계)", MessageType.청색));
                cbo.Items.Add(new ComboBoxData("집중호우 및 태풍(경보예보(적색)단계)", MessageType.적색));
                cbo.Items.Add(new ComboBoxData("집중호우 및 태풍(긴급대피)", MessageType.긴급));
                cbo.Items.Add(new ComboBoxData("기타(시스템 지정)", MessageType.System));
                cbo.Items.Add(new ComboBoxData("사용자 입력", MessageType.UserInput));
                cbo.Items.Add(new ComboBoxData("시나리오", MessageType.Scenario));

                cbo.SelectedIndex = 6;
            }
        }

        public class Earthquake : Disaster
        {
            public Earthquake(string strStepName)
            {
                m_strStepName = strStepName;
                SetupMessage();
            }

            private void SetupMessage()
            {
                SetupDisasterTypeMessage();
                SetupSystemMessage();
                SetupUserInputMessage();
            }

            private void SetupDisasterTypeMessage()
            {
                LocationString location = new LocationString("[재난발생위치]");

                ArrayList strings = new ArrayList();

                strings.Add("본부 재난안전대책본부에서 ");
                strings.Add("현재 지진발생이 감지되어 알려 드리오니 전 직원은 휴대폰, 손전등, 마실 물, 소화기, 구급약품, 휴대용 라디오, 필기구 등을 \r\n");
                strings.Add("지참하시어 해안설비에서 먼 공터 또는 높은 지대나 튼튼한 건물 옥상으로 대피해 주시기 바랍니다.\r\n\r\n");
                strings.Add("미처 대피하지 못한 직원께서는 책상이나 탁자 아래, 발전설비 내부, 내력벽이 있는 건물 공간으로 긴급 대피하시고, \r\n");
                strings.Add("부두, 방파제, 취․배수로, 고압가스 또는 위험물질 저장소, 매달린 물체 아래, 거울, 문, 액자, 발코니 근처는 피해주시기 바랍니다.");

                m_dicMessageString[MessageType.DisasterType] = strings;
            }

            private void SetupSystemMessage()
            {
                StartTimeString startTime = new StartTimeString("[재난발생시간]");

                ArrayList strings = new ArrayList();

                strings.Add(startTime);
                strings.Add(" 지진 " + m_strStepName + " SOP 상황이 시작되었습니다.");
            }

            public override string GetMessage(string strLocation, VariousData<DateTime> dtStart, MessageType type, bool isRealMode, string strPSMMaterialName, int nPSMDistance, string strAmountSnowfall)
            {
                if (type == MessageType.DisasterType)
                    return GetDisasterTypeMessage(strLocation, dtStart, isRealMode);
                else if (type == MessageType.System)
                    return GetSystemMessage(strLocation, dtStart, isRealMode);
                else if (type == MessageType.UserInput)
                    return GetUserInputMessage();
                else if (type == MessageType.Scenario)
                    return GetScenarioMessage(strLocation, dtStart, isRealMode, strPSMMaterialName, nPSMDistance, strAmountSnowfall);

                return "";
            }

            private string GetSystemMessage(string strLocation, VariousData<DateTime> dtStart, bool isRealMode)
            {
                ArrayList strings = null;

                if (!m_dicMessageString.TryGetValue(MessageType.System, out strings))
                    return "";

                int nCount = strings.Count;
                string strMessage = "";

                for (int i = 0; i < nCount; i++)
                {
                    if (i == 0 && strings[i] is StartTimeString)
                    {
                        if (isRealMode)
                            strMessage = "실제 상황입니다. ";
                        else
                            strMessage = "모의훈련 상황입니다. ";
                    }

                    if (strings[i] is string)
                        strMessage += strings[i].ToString();
                    else if (strings[i] is LocationString)
                        strMessage += strLocation;
                    else if (strings[i] is StartTimeString)
                    {
                        if (dtStart == null)
                            strMessage += "금일 현재시각";
                        else if (dtStart.Data.Year == DateTime.Now.Year && dtStart.Data.Month == DateTime.Now.Month && dtStart.Data.Day == DateTime.Now.Day)
                            strMessage += "금일 " + dtStart.Data.Hour.ToString() + "시 " + dtStart.Data.Minute.ToString() + "분";
                        else
                            strMessage += dtStart.Data.Month.ToString() + "월 " + dtStart.Data.Day.ToString() + "일 " + dtStart.Data.Hour.ToString() + "시 " + dtStart.Data.Minute.ToString() + "분";
                    }
                    else
                        return "";
                }

                //m_dicFullMessage[MessageType.System] = strMessage;
                return strMessage;
            }

            private string GetDisasterTypeMessage(string strLocation, VariousData<DateTime> dtStart, bool isRealMode)
            {
                ArrayList strings = null;

                if (!m_dicMessageString.TryGetValue(MessageType.DisasterType, out strings))
                    return "";

                int nCount = strings.Count;
                string strMessage = "";

                for (int i = 0; i < nCount; i++)
                {
                    if (i == 0 && strings[i] is string)
                    {
                        strMessage = strings[i].ToString();

                        if (isRealMode)
                            strMessage += "실제 비상상황을 알려드립니다.\r\n\r\n";
                        else
                            strMessage += "훈련 비상상황을 알려드립니다.\r\n\r\n";
                    }
                    else
                    {
                        if (strings[i] is string)
                            strMessage += strings[i].ToString();
                        else if (strings[i] is LocationString)
                            strMessage += strLocation;
                        else if (strings[i] is StartTimeString)
                        {
                            if (dtStart == null)
                                strMessage += "금일 현재시각";
                            else if (dtStart.Data.Year == DateTime.Now.Year && dtStart.Data.Month == DateTime.Now.Month && dtStart.Data.Day == DateTime.Now.Day)
                                strMessage += "금일 " + dtStart.Data.Hour.ToString() + "시 " + dtStart.Data.Minute.ToString() + "분";
                            else
                                strMessage += dtStart.Data.Month.ToString() + "월 " + dtStart.Data.Day.ToString() + "일 " + dtStart.Data.Hour.ToString() + "시 " + dtStart.Data.Minute.ToString() + "분";
                        }
                        else
                            return "";
                    }
                }

                //m_dicFullMessage[MessageType.DisasterType] = strMessage;
                return strMessage;
            }

            public override void SetComboBox(ComboBox cbo)
            {
                cbo.Items.Clear();

                cbo.Items.Add(new ComboBoxData("지진 및 지진해일(긴급대피)", MessageType.DisasterType));
                cbo.Items.Add(new ComboBoxData("기타(시스템 지정)", MessageType.System));
                cbo.Items.Add(new ComboBoxData("사용자 입력", MessageType.UserInput));
                cbo.Items.Add(new ComboBoxData("시나리오", MessageType.Scenario));

                cbo.SelectedIndex = 3;
            }
        }

        public class HeavySnow : Disaster
        {
            public HeavySnow(string strStepName)
            {
                m_strStepName = strStepName;
                SetupMessage();
            }

            private void SetupMessage()
            {
                SetupDisasterTypeMessage();
                SetupSystemMessage();
                SetupUserInputMessage();
            }

            private void SetupDisasterTypeMessage()
            {
                StartTimeString startTime = new StartTimeString("[재난발생시간]");

                ArrayList strings = new ArrayList();

                strings.Add("본부 재난안전대책본부에서 ");
                strings.Add(startTime);
                strings.Add(" 우리본부 인근지역에 대설 경보가 발효되었습니다.\r\n");
                strings.Add("전 직원께서는 폭설예보에 대비한 조치사항을 숙지하시어 분담업무를 수행하시기 바라며, \r\n");
                strings.Add("출퇴근길에는 가능한 회사에서 제공한 대형버스를 탑승해 주시고, \r\n");
                strings.Add("불가피하게 자가 승용차를 운행할 경우에는 반드시 월동 장비를 채비하시기 바라며, \r\n");
                strings.Add("결빙이 우려되는 길을 피하여 우회하여 주시기 바랍니다.");

                m_dicMessageString[MessageType.DisasterType] = strings;
            }

            private void SetupSystemMessage()
            {
                StartTimeString startTime = new StartTimeString("[재난발생시간]");

                ArrayList strings = new ArrayList();

                strings.Add(startTime);
                strings.Add(" 폭설 " + m_strStepName + " SOP 상황이 시작되었습니다.");
            }

            public override string GetMessage(string strLocation, VariousData<DateTime> dtStart, MessageType type, bool isRealMode, string strPSMMaterialName, int nPSMDistance, string strAmountSnowfall)
            {
                if (type == MessageType.DisasterType)
                    return GetDisasterTypeMessage(strLocation, dtStart, isRealMode);
                else if (type == MessageType.System)
                    return GetSystemMessage(strLocation, dtStart, isRealMode);
                else if (type == MessageType.UserInput)
                    return GetUserInputMessage();
                else if (type == MessageType.Scenario)
                    return GetScenarioMessage(strLocation, dtStart, isRealMode, strPSMMaterialName, nPSMDistance, strAmountSnowfall);

                return "";
            }

            private string GetSystemMessage(string strLocation, VariousData<DateTime> dtStart, bool isRealMode)
            {
                ArrayList strings = null;

                if (!m_dicMessageString.TryGetValue(MessageType.System, out strings))
                    return "";

                int nCount = strings.Count;
                string strMessage = "";

                for (int i = 0; i < nCount; i++)
                {
                    if (i == 0 && strings[i] is StartTimeString)
                    {
                        if (isRealMode)
                            strMessage = "실제 상황입니다. ";
                        else
                            strMessage = "모의훈련 상황입니다. ";
                    }

                    if (strings[i] is string)
                        strMessage += strings[i].ToString();
                    else if (strings[i] is LocationString)
                        strMessage += strLocation;
                    else if (strings[i] is StartTimeString)
                    {
                        if (dtStart == null)
                            strMessage += "금일 현재시각";
                        else if (dtStart.Data.Year == DateTime.Now.Year && dtStart.Data.Month == DateTime.Now.Month && dtStart.Data.Day == DateTime.Now.Day)
                            strMessage += "금일 " + dtStart.Data.Hour.ToString() + "시 " + dtStart.Data.Minute.ToString() + "분";
                        else
                            strMessage += dtStart.Data.Month.ToString() + "월 " + dtStart.Data.Day.ToString() + "일 " + dtStart.Data.Hour.ToString() + "시 " + dtStart.Data.Minute.ToString() + "분";
                    }
                    else
                        return "";
                }

                //m_dicFullMessage[MessageType.System] = strMessage;
                return strMessage;
            }

            private string GetDisasterTypeMessage(string strLocation, VariousData<DateTime> dtStart, bool isRealMode)
            {
                ArrayList strings = null;

                if (!m_dicMessageString.TryGetValue(MessageType.DisasterType, out strings))
                    return "";

                int nCount = strings.Count;
                string strMessage = "";

                for (int i = 0; i < nCount; i++)
                {
                    if (i == 0 && strings[i] is string)
                    {
                        strMessage = strings[i].ToString();

                        if (isRealMode)
                            strMessage += "실제 비상상황을 알려드립니다.\r\n\r\n";
                        else
                            strMessage += "훈련 비상상황을 알려드립니다.\r\n\r\n";
                    }
                    else
                    {
                        if (strings[i] is string)
                            strMessage += strings[i].ToString();
                        else if (strings[i] is LocationString)
                            strMessage += strLocation;
                        else if (strings[i] is StartTimeString)
                        {
                            if (dtStart == null)
                                strMessage += "금일 현재시각";
                            else if (dtStart.Data.Year == DateTime.Now.Year && dtStart.Data.Month == DateTime.Now.Month && dtStart.Data.Day == DateTime.Now.Day)
                                strMessage += "금일 " + dtStart.Data.Hour.ToString() + "시 " + dtStart.Data.Minute.ToString() + "분";
                            else
                                strMessage += dtStart.Data.Month.ToString() + "월 " + dtStart.Data.Day.ToString() + "일 " + dtStart.Data.Hour.ToString() + "시 " + dtStart.Data.Minute.ToString() + "분";
                        }
                        else
                            return "";
                    }
                }

                //m_dicFullMessage[MessageType.DisasterType] = strMessage;
                return strMessage;
            }

            public override void SetComboBox(ComboBox cbo)
            {
                cbo.Items.Clear();

                cbo.Items.Add(new ComboBoxData("폭설(긴급안내)", MessageType.DisasterType));
                cbo.Items.Add(new ComboBoxData("기타(시스템 지정)", MessageType.System));
                cbo.Items.Add(new ComboBoxData("사용자 입력", MessageType.UserInput));
                cbo.Items.Add(new ComboBoxData("시나리오", MessageType.Scenario));

                cbo.SelectedIndex = 3;
            }
        }

        public class General : Disaster
        {
            public General(string strStepName)
            {
                m_strStepName = strStepName;
                SetupMessage();
            }

            private void SetupMessage()
            {
                SetupUserInputMessage();
            }

            public override string GetMessage(string strLocation, VariousData<DateTime> dtStart, MessageType type, bool isRealMode, string strPSMMaterialName, int nPSMDistance, string strAmountSnowfall)
            {
                if (type == MessageType.UserInput)
                    return GetUserInputMessage();
                else if (type == MessageType.Scenario)
                    return GetScenarioMessage(strLocation, dtStart, isRealMode, strPSMMaterialName, nPSMDistance, strAmountSnowfall);

                return "";
            }

            public override void SetComboBox(ComboBox cbo)
            {
                cbo.Items.Clear();

                cbo.Items.Add(new ComboBoxData("사용자 입력", MessageType.UserInput));
                cbo.Items.Add(new ComboBoxData("시나리오", MessageType.Scenario));

                cbo.SelectedIndex = 1;
            }
        }

        private Sections.Section m_section = null;
        private Disaster m_disaster = null;
        private string m_strLocation = "";
        private string m_strBroadcastLocationName = "";
        private VariousData<DateTime> m_dtStart = null;
        private bool m_isRealMode = false;
        private bool m_runMode = false;
        private bool m_useBroadcast = true;
        private bool m_useSMS = false;

        private string m_strPSMMaterialName = "";
        // 유해화학물질 누출시 대피거리(미터)
        private int m_nPSMDistance = 0;
        private string m_strAmountSnowfall = "";

        private Sections.SectionCommander m_commander = null;
        private string m_strCommanderName = "", m_strCommanderName2 = "";
        private string m_strCommanderPhoneNumber = "";
        private ArrayList m_arrReceiverPhoneNumbers = null;
        private List<Sections.SOPTeam> m_receiverTeams = null;
        private bool m_onlyTeamLeaderReceiver = true;
        private string m_strReceiverNames = "";
        private VariousData<DateTime> m_dtExecute = null;
        private VariousData<DateTime> m_dtComplete = null;
        private VariousData<DateTime> m_dtUncomplete = null;
        // CheckBoxComplete을 Mouse Click을 통하여 해제시켰는가?
        private bool m_uncheckedComplete = false;

        private bool m_disabled = false;
        private bool m_systemCall = false;

        private UnE.SOP.Workstate.WorkflowOption m_option = null;

        public bool UsePSM
        {
            get { return m_strPSMMaterialName == null || m_strPSMMaterialName.Length == 0 ? false : true; }
        }

        public string PSMMaterial
        {
            get { return m_strPSMMaterialName; }
            set { m_strPSMMaterialName = value; }
        }

        // 유해화학물질 누출시 대피거리(미터)
        public int PSMDistance
        {
            get { return m_nPSMDistance; }
            set { m_nPSMDistance = value; }
        }

        public bool UseAmountSnowfall
        {
            get { return m_strAmountSnowfall == null || m_strAmountSnowfall.Length == 0 ? false : true; }
        }

        public string AmountSnowfall
        {
            get { return m_strAmountSnowfall; }
            set { m_strAmountSnowfall = value; }
        }

        public bool RealMode
        {
            get { return m_isRealMode; }
            set { m_isRealMode = value; }
        }

        public Sections.Section Section
        {
            get { return m_section; }
            set { SetSection(value); }
        }

        public UnE.SOP.Workstate.WorkflowOption Option
        {
            get { return m_option; }
            set { m_option = value; }
        }

        public bool RunMode
        {
            get { return m_runMode; }
            set
            {
                m_runMode = value;

                // Run 모드 변경전에 로그를 통하여 미리 상태가 바뀐 경우는 m_runMode에 따라 강제로 상태를 바꾸면 안된다.
                VariousData<bool> execute = m_dtExecute == null ? null : new VariousData<bool>(btnExecute.Enabled);

                btnExecute.Enabled = checkBoxComplete.Enabled = m_runMode;

                if (execute != null)
                    btnExecute.Enabled = execute.Data;

                // RunMode가 바뀌면 즉시 시나리오 메시지를 바꿔준다.
                if (cboDisaster.SelectedIndex >= 0 && m_disaster != null)
                {
                    ComboBoxData data = (ComboBoxData)cboDisaster.Items[cboDisaster.SelectedIndex];

                    if ((MessageType)data.Data == MessageType.Scenario)
                    {
                        string strLocation = m_strLocation;

                        if (m_useBroadcast)
                            strLocation = m_strBroadcastLocationName;

                        string strMessage = m_disaster.GetMessage(strLocation, m_dtStart, (MessageType)data.Data, m_isRealMode, m_strPSMMaterialName, m_nPSMDistance, m_strAmountSnowfall);
                        strMessage = ChangeText(strMessage);
                        textBoxMessage.Text = strMessage;
                        SetSectionMessage(textBoxMessage.Text);
                    }

                    if (m_commander != null && m_commander.Team == null)
                    {
                        ComponentContents contents = (ComponentContents)this.Parent.Parent;
                        UnE.SOP.Workstate.SectionState state = contents.GetSectionState();
                        ChangeCommanderName(state);
                        //textBoxSender.Text = m_strCommanderName2;
                    }
                }
            }
        }

        private string ChangeText(string strMessage)
        {
            if (m_option == null)
                return strMessage;

            if (m_option is UnE.SOP.Workstate.WorkflowOptionEarthquake)
            {
                strMessage = ComponentContents.ChangeEarthquakeString(strMessage, (UnE.SOP.Workstate.WorkflowOptionEarthquake)m_option);
            }
            else if (m_option is UnE.SOP.Workstate.WorkflowOptionPSM)
            {
                strMessage = ComponentContents.ChangePSMString(strMessage, (UnE.SOP.Workstate.WorkflowOptionPSM)m_option);
            }
            else if (m_option is UnE.SOP.Workstate.WorkflowOptionSnowFall)
            {
                strMessage = ComponentContents.ChangeClimateString(strMessage, (UnE.SOP.Workstate.WorkflowOptionSnowFall)m_option);
            }
            else if (m_option != null)
            {
                strMessage = ComponentContents.ChangeCommonString(strMessage, m_option);
            }

            return strMessage;
        }

        public bool UseBroadcast
        {
            get { return m_useBroadcast; }
            set { m_useBroadcast = value; }
        }

        public VariousData<DateTime> ExecuteTime
        {
            get { return m_dtExecute; }
            set { m_dtExecute = value; }
        }

        public VariousData<DateTime> CompleteTime
        {
            get { return m_dtComplete; }
            set { m_dtComplete = value; }
        }

        public VariousData<DateTime> UncompleteTime
        {
            get { return m_dtUncomplete; }
            set { m_dtUncomplete = value; }
        }

        // CheckBoxComplete을 Mouse Click을 통하여 해제시켰는가?
        public bool UncheckedComplete
        {
            get { return m_uncheckedComplete; }
        }

        public bool IsComplete
        {
            get { return checkBoxComplete.Checked; }
        }

        public bool Disabled
        {
            get { return m_disabled; }
            set
            {
                m_disabled = value;

                textBoxMessage.ReadOnly = m_disabled;
                cboBroadcastCount.Disabled = m_disabled;
                //cboBroadcastCount.Enabled = !m_disabled;
                //checkBoxSiren.Enabled = checkBoxComplete.Enabled = !m_disabled;
                checkBoxSiren.AutoCheck = checkBoxComplete.AutoCheck = !m_disabled;
            }
        }

        public FormMissionMessage(Sections.Section section)
        {
            InitializeComponent();
            Section = section;

            this.TopLevel = false;
            cboBroadcastCount.SelectedIndex = 0;

            Init();
        }

        public void Init()
        {
            btnExecute.Enabled = checkBoxComplete.Enabled = false;
        }

        private void SetSection(Sections.Section section)
        {
            m_section = section;

            if (m_section == null)
            {
                SetDisaster(null);
                return;
            }

            Sections.PanelSection panel = section.GetParent();

            if (panel == null)
                return;

            UnE.SOP.Sections.SectionTabPage page = (UnE.SOP.Sections.SectionTabPage)panel.Parent;
            m_isRealMode = !page.VirtualMode;

            string szFullPath = FormSOP.Instance.GetActionStepPath(page.ActionStepID);
            char[] seperators = { '/', '\\', (char)0x06 };
            string[] arPath = szFullPath.Split(seperators);
            if (arPath.Length < 2)
                return;

            string strStepName = "";

            if (arPath.Length >= 4)
                strStepName = arPath[3];

            Disaster disaster = null;

            bool bCreate = false;
            if (arPath[0] == "자연재해")
            {
                if (arPath[1] == "태풍")
                {
                    disaster = new Typhoon(strStepName);
                    bCreate = true;
                }
                else if (arPath[1] == "지진")
                {
                    disaster = new Earthquake(strStepName);
                    bCreate = true;
                }
                else if (arPath[1] == "폭설")
                {
                    disaster = new HeavySnow(strStepName);
                    bCreate = true;
                }
            }
            else if (arPath[0] == "태풍")
            {
                disaster = new Typhoon(strStepName);
                bCreate = true;
            }
            else if (arPath[0] == "화재" || arPath[0] == "유출사고")
            {
                disaster = new Fire(strStepName);
                bCreate = true;
            }

            if (bCreate == false)
            {
                disaster = new General(strStepName);
            }

            if (disaster != null)
                disaster.Parent = this;

            SetScenarioMessage(section, disaster);
            SetDisaster(disaster);
        }


        // 현재 금무조가 아닌 상황실 근무조의 MemberID/TeamType 을 쌍으로 가지는 리스트
        private SortedList<int, int> mOffDutyMembers = new SortedList<int, int>();
        private void GetAllOffDutyMembers()
        {
            mOffDutyMembers.Clear();
            
            // 현재 근무조가 아닌조에 지정된 모든 멤버를 가져온다.
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT ctm.MemberType, ctm.MemberID FROM ControlTeamMembers as ctm ");
            sb.Append("INNER JOIN ControlWorkingTeam AS cwt ON ctm.RoomID = cwt.RoomID ");
            sb.Append("WHERE MemberID is not NULL and cwt.TeamID <> ctm.TeamID");

            string szSQL = sb.ToString();
            ArrayList arrResult = FormSOP.Instance.DBManager.GetResultData(szSQL, 0);
            if (arrResult == null)
                return;

            int nCount = arrResult.Count;
            if (nCount == 0)
                return;

            for (int i = 0; i < nCount - 1; i += 2)
            {
                int nTeamType = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nMemberID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);

                // 정규팀인 경우만 검사할 필요가 있다
                if( nTeamType == (int)ControlTeamEditor.DataControlTeamMember.ControlMemberType.RegularMember)
                {
                    if (!mOffDutyMembers.ContainsKey(nMemberID))
                    {
                        mOffDutyMembers.Add(nMemberID, nTeamType);
                    }
                }                
            }
        }

        private void AddAllCompanyMemberPhoneNumbers()
        {
            List<UnE.SOP.Data_CompanyMember> allMembers = FormSOP.Instance.SOPManager.GetAllRegularCompanyMemberList();

            if (allMembers != null)
            {
                // 비번 리스트를 갱신한다.
                // skkim 2015.08.24
                GetAllOffDutyMembers();

                foreach (UnE.SOP.Data_CompanyMember member in allMembers)
                {
                    // 비번목록에 있는경우 수신자에서 제외한다.
                    // skkim 2015.08.24
                    int nMemberID = member.ID;
                    if (mOffDutyMembers.ContainsKey(nMemberID))
                        continue;

                    if (member.PhoneNumber.Length > 0)
                        m_arrReceiverPhoneNumbers.Add(member.PhoneNumber);
                }
            }
        }

        private void SetScenarioMessage(Sections.Section section, Disaster disaster)
        {
            if (disaster == null || section == null)
                return;

            Sections.Section.ComponentType type = section.GetComponentType();

            if (type == Sections.Section.ComponentType.INTERNAL)
            {
                Sections.SectionDataInternal data = (Sections.SectionDataInternal)section.Data;
                disaster.SetupScenarioMessage(data.BroadcastMessage);

                cboBroadcastCount.CanVisible = data.UseBroadcast;

                if (data.UseBroadcast == false)
                {
                    labelMessageType.Visible = cboDisaster.Visible = labelBroadcastCount.Visible = checkBoxSiren.Visible = false;
                    cboBroadcastCount.HideControl();
                }
                else
                {
                    checkBoxSiren.Visible = true;
                    cboBroadcastCount.ShowControl();
                }
                
                // 이전에 방송옵션과 문자옵션을 동일하게 적용가능한 시나리오가 이미 존재하여
                // 방송타입의 컴포넌트에게는 문자타입을 적용하지 않도록 함.
                //if (data.UseMobileApp == false)
                if (data.UseBroadcast == true)
                {
                    labelSender.Visible = textBoxSender.Visible = labelReceiver.Visible = textBoxReceiver.Visible = false;
                }
                else
                {
                    m_commander = ComponentContents.GetCommanderInfo(section, out m_strCommanderName, out m_strCommanderName2, out m_strCommanderPhoneNumber);

                    ArrayList arrTeamList;
                    m_arrReceiverPhoneNumbers = ComponentContents.GetReceiverInfo(section, out m_strReceiverNames, out arrTeamList, out m_onlyTeamLeaderReceiver);

                    if (arrTeamList == null)
                        m_receiverTeams = null;
                    else
                    {
                        if (m_receiverTeams == null)
                            m_receiverTeams = new List<Sections.SOPTeam>();
                        else
                            m_receiverTeams.Clear();

                        foreach (Sections.SOPTeam team in arrTeamList)
                        {
                            m_receiverTeams.Add(team);
                        }
                    }

                    textBoxSender.Text = m_strCommanderName;

                    /*Sections.SOPTeam teamRoot = IOManager.LoadRegularRootTeam(FormSOP.Instance.DBManager);

                    // 최상위팀이 수신팀에 있을 경우, 전직원의 전화번호를 넣는다.
                    if (ContainsTeam(arrTeamList, teamRoot))
                    {
                        if (m_arrReceiverPhoneNumbers != null)
                            m_arrReceiverPhoneNumbers.Clear();
                    }*/

                    /*if (m_arrReceiverPhoneNumbers == null || m_arrReceiverPhoneNumbers.Count == 0)
                    {
                        if (m_arrReceiverPhoneNumbers == null)
                            m_arrReceiverPhoneNumbers = new ArrayList();

                        Sections.SOPTeam teamRoot = IOManager.LoadRegularRootTeam(FormSOP.Instance.DBManager);

                        if (teamRoot != null)
                        {
                            AddAllCompanyMemberPhoneNumbers();
                            //m_arrReceiverPhoneNumbers.Add(team);
                            textBoxReceiver.Text = teamRoot.TeamName;
                        }
                        else
                            textBoxReceiver.Text = "";

                        m_strReceiverNames = textBoxReceiver.Text;
                    }
                    else*/
                        textBoxReceiver.Text = m_strReceiverNames;
                }

                m_useBroadcast = data.UseBroadcast;
                m_useSMS = (data.UseBroadcast == true ? false : data.UseMobileApp);
            }
        }

        private bool ContainsTeam(ArrayList arrTeams, Sections.SOPTeam team)
        {
            if (team == null)
                return false;

            foreach (Sections.SOPTeam _team in arrTeams)
            {
                if (_team.TeamID == team.TeamID && _team.TeamType == team.TeamType)
                    return true;
            }

            return false;
        }

        public Disaster GetDisaster()
        {
            return m_disaster;
        }

        public void SetDisaster(Disaster disaster)
        {
            m_disaster = disaster;

            if (m_disaster == null)
                cboDisaster.Items.Clear();
            else
                m_disaster.SetComboBox(cboDisaster);
        }

        public string GetLocation()
        {
            return m_strLocation;
        }

        public void SetLocation(string strLocation, string strBroadcastLocationName)
        {
            m_strLocation = strLocation;
            m_strBroadcastLocationName = strBroadcastLocationName;
        }

        public VariousData<DateTime> GetStartTime()
        {
            return m_dtStart;
        }


        public void SetStartTime(VariousData<DateTime> dtStart)
        {
            m_dtStart = dtStart;
        }

        private void comboDisaster_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_disaster == null)
            {
                textBoxMessage.Text = "";
                return;
            }

            if (cboDisaster.SelectedIndex < 0)
            {
                textBoxMessage.Text = "";
                return;
            }

            ComboBoxData data = (ComboBoxData)cboDisaster.Items[cboDisaster.SelectedIndex];

            string strMessage = m_disaster.GetMessage(m_strLocation, m_dtStart, (MessageType)data.Data, m_isRealMode, m_strPSMMaterialName, m_nPSMDistance, m_strAmountSnowfall);
            textBoxMessage.Text = strMessage;
        }

        private void textBoxMessage_TextChanged(object sender, EventArgs e)
        {
            if (m_disaster != null && cboDisaster.SelectedIndex >= 0)
            {
                ComboBoxData data = (ComboBoxData)cboDisaster.Items[cboDisaster.SelectedIndex];
                m_disaster.SetMessage((MessageType)data.Data, textBoxMessage.Text);

                SetSectionMessage(textBoxMessage.Text);

                if (m_runMode)
                {
                    if (FormSOP.Instance.HasControl == false)
                        return;

                    string szMissionText = textBoxMessage.Text;
                    string szToTarget = "";

                    if ((m_section.Data as Sections.SectionDataInternal).UseMobileApp == true)
                    {
                        szToTarget = textBoxReceiver.Text;
                    }

                    PopupMissionText form = PopupMissionText.Instance;
                    form.SetOnlyText(szMissionText, szToTarget, "", "", "", this.Section);

                }
            }
        }

        private void SetSectionMessage(string strMessage)
        {
            if (m_section == null)
                return;

            Sections.Section.ComponentType type = m_section.GetComponentType();

            if (type == Sections.Section.ComponentType.INTERNAL)
            {
                Sections.SectionDataInternal data = (Sections.SectionDataInternal)m_section.Data;

                data.BroadcastMessage = textBoxMessage.Text;
                data.UseSiren = checkBoxSiren.Checked;
                data.RepeatCount = cboBroadcastCount.SelectedIndex + 1;
            }
        }

        private void btnShowSpecialMessageOption_Click(object sender, EventArgs e)
        {
            FormSOP.Instance.ShowSpecialMessageHelp();
        }

        private void checkBoxSiren_CheckedChanged(object sender, EventArgs e)
        {
            if (m_section == null)
                return;

            Sections.Section.ComponentType type = m_section.GetComponentType();

            if (type == Sections.Section.ComponentType.INTERNAL)
            {
                Sections.SectionDataInternal data = (Sections.SectionDataInternal)m_section.Data;

                data.UseSiren = checkBoxSiren.Checked;
            }
        }

        private void cboBroadcast_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_section == null)
                return;

            Sections.Section.ComponentType type = m_section.GetComponentType();

            if (type == Sections.Section.ComponentType.INTERNAL)
            {
                Sections.SectionDataInternal sectionData = (Sections.SectionDataInternal)m_section.Data;

                sectionData.RepeatCount = cboBroadcastCount.SelectedIndex + 1;
            }
        }

        public static string GetDefaultSMSCaller()
        {
            string strSQL = "Select PropertyValue from OptionSDMS where PropertyName = 'SMSCaller' and SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();
            ArrayList arrResult = FormSOP.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return "";

            string strPhoneNumber = DBUtility.WebDBManager.GetStringField(arrResult[0]);
            return strPhoneNumber == null ? "" : strPhoneNumber;
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            if (this.Parent == null || this.Parent.Parent == null || (this.Parent.Parent is ComponentContents) == false || m_section == null)
                return;

            if (this.Disabled)
                return;

            ComponentContents contents = (ComponentContents)this.Parent.Parent;

            if (m_useBroadcast)
            {
                if (cboBroadcastCount.SelectedIndex >= 0)
                {
                    if (m_systemCall == true || MessageBox.Show("방송을 실행하시겠습니까?", "방송", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        AfterRunExecute();
                        contents.RunBroadcast(textBoxMessage.Text, cboBroadcastCount.SelectedIndex + 1, checkBoxSiren.Checked);
                    }
                }
            }

            if (m_useSMS)
            {
                // 발신자 전화번호를 알수없을 경우 Default 전화번호를 사용한다.
                if (m_strCommanderPhoneNumber.Length == 0)
                {
                    m_strCommanderPhoneNumber = GetDefaultSMSCaller();
                    //m_strCommanderPhoneNumber = WebDBManager.SMSCaller;
                }

                if (m_strCommanderPhoneNumber.Length > 0)
                {
                    ArrayList arrTeamList;
                    //bool onlyTeamLeader;
                    m_arrReceiverPhoneNumbers = ComponentContents.GetReceiverInfo(m_section, out m_strReceiverNames, out arrTeamList, out m_onlyTeamLeaderReceiver);

                    if (m_arrReceiverPhoneNumbers != null && m_arrReceiverPhoneNumbers.Count > 0)
                    {
                        bool bSendSMS = true;
                        if (UnE.SOP.ProxySOP.Instance.ConfirmSendSMS == true)
                        {
                            if (UnE.SOP.ProxySOP.Instance.ConfirmSMSAll == false)
                            {
                                MessageBoxEx msgBox = new MessageBoxEx();
                                msgBox.Text = "문자발송";
                                msgBox.ShowDialog();
                                if (msgBox.DialogResult != System.Windows.Forms.DialogResult.No)
                                {
                                    if (msgBox.DialogResult == System.Windows.Forms.DialogResult.Ignore)
                                    {
                                        UnE.SOP.ProxySOP.Instance.ConfirmSMSAll = true;
                                    }
                                }
                                else
                                {
                                    bSendSMS = false;
                                }
                            }
                        }
                        if(bSendSMS == true)
                        {
                            AfterRunExecute();

                            ArrayList arrCallList = m_arrReceiverPhoneNumbers.Clone() as ArrayList;
                            arrCallList = ControlTeamEditor.VaildMemberPhoneNumber.IsVaildPhoneNumber(arrCallList, FormSOP.Instance.DBManager);

                            UnE.SOP.SMS.SMSManager.Instance.SendSMS(arrCallList, m_strCommanderPhoneNumber, textBoxMessage.Text);
                            SetSMSDBHistory(textBoxMessage.Text, FormSOP.Instance.DBManager);
                            contents.SendLogState(m_section);
                        }
                    }
                    else
                    {
                        // 전직원
                        ArrayList arrPhoneNumbers = FormSOP.Instance.GetPageHome().GetDockPersonnel().GetMemberPhoneNumber();

                        if (FormSOP.Instance.SmsExternalCompanyMemberOn)
                        {
                            // 협력업체 직원들의 전화번호 추가
                            FormSOP.Instance.AddExternalCompanyMemberPhoneNumbers(arrPhoneNumbers);
                        }


                        bool bSendSMS = true;

                        if(UnE.SOP.ProxySOP.Instance.ConfirmSendSMS == true)
                        {
                            if (UnE.SOP.ProxySOP.Instance.ConfirmSMSAll == false)
                            {
                                MessageBoxEx msgBox = new MessageBoxEx();
                                msgBox.Text = "문자발송";
                                msgBox.ShowDialog();
                                if (msgBox.DialogResult != System.Windows.Forms.DialogResult.No)
                                {
                                    if (msgBox.DialogResult == System.Windows.Forms.DialogResult.Ignore)
                                    {
                                        UnE.SOP.ProxySOP.Instance.ConfirmSMSAll = true;
                                    }
                                }
                                else
                                {
                                    bSendSMS = false;
                                }
                            }
                        }                       
                        
                        if( bSendSMS == true)
                        {
                            AfterRunExecute();

                            ArrayList arrCallList = arrPhoneNumbers.Clone() as ArrayList;
                            arrCallList = ControlTeamEditor.VaildMemberPhoneNumber.IsVaildPhoneNumber(arrCallList, FormSOP.Instance.DBManager);

                            UnE.SOP.SMS.SMSManager.Instance.SendSMS(arrCallList, m_strCommanderPhoneNumber, textBoxMessage.Text);
                            SetSMSDBHistory(textBoxMessage.Text, FormSOP.Instance.DBManager);
                            contents.SendLogState(m_section);
                        }
                     
                    }
                }
            }
        }

        private static void SetSMSDBHistory(string strMsg, WebDBManager dbMgr)
        {
            string strSQL = "Select ID from OptionSOPSimulator where PropertyName = 'LastSMSMessage' and SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            DateTime dtNow = DateTime.Now;
            int nID = 0;
            string strTime = string.Format("{0}-{1}-{2} {3}:{4}:{5}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);
            string strBody = strTime + "," + strMsg;

            if (arrResult == null || arrResult.Count == 0)
            {
                strSQL = "Select max(ID) from OptionSOPSimulator";
                arrResult = dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null || arrResult.Count == 0)
                    nID = 1;
                else
                {
                    nID = WebDBManager.GetIntField(arrResult[0].ToString(), -1) + 1;

                    if (nID < 0)
                        nID = 1;
                }

                strSQL = "Insert into OptionSOPSimulator (ID, PropertyName, PropertyValue, Description, SiteID) values (";
                strSQL += string.Format("{0}, 'LastSMSMessage', '{1}', '마지막으로 발송된 문자메시지', {2})", nID, strBody, UnE.SOP.ProxySOP.Instance.SiteID);
                dbMgr.GetResultData(strSQL, 0);
            }
            else
            {
                nID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);

                if (nID > 0)
                {
                    strSQL = string.Format("Update OptionSOPSimulator set PropertyValue = '{0}' where ID = {1}", strBody, nID);
                    dbMgr.GetResultData(strSQL, 0);
                }
            }
        }

        public void Run(bool noSMS)
        {
            m_systemCall = true;

            if (noSMS)
            {
                bool originOption = m_useSMS;
                bool originOptionBroadcast = m_useBroadcast;
                m_useSMS = false;
                m_useBroadcast = false;

                btnExecute_Click(null, null);

                m_useBroadcast = originOptionBroadcast;
                m_useSMS = originOption;
            }
            else
                btnExecute_Click(null, null);

            m_systemCall = false;

            checkBoxComplete.Checked = true;
        }

        /// <summary>
        /// 컴포넌트 컨텐츠로 사용될 때 리셋
        /// </summary>
        public void ResetForComponentContent()
        {
            m_dtExecute = null;
            m_dtComplete = null;
            m_dtUncomplete = null;

            btnExecute.Enabled = true;
            cboBroadcastCount.Enabled = true;
            checkBoxSiren.Enabled = true;

            cboBroadcastCount.SelectedIndex = 0;
            checkBoxSiren.Checked = true;
            checkBoxComplete.Checked = false;
        }

        private void AfterRunExecute()
        {
            btnExecute.Enabled = false;
            cboBroadcastCount.Enabled = false;
            checkBoxSiren.Enabled = false;

            m_dtExecute = new VariousData<DateTime>(DateTime.Now);
        }

        private void checkBoxComplete_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxComplete.Checked)
            {
                m_uncheckedComplete = false;
                m_dtComplete = new VariousData<DateTime>(DateTime.Now);
            }
            else
            {
                m_uncheckedComplete = true;
                m_dtUncomplete = new VariousData<DateTime>(DateTime.Now);
                btnExecute.Enabled = true;
                cboBroadcastCount.Enabled = true;
                checkBoxSiren.Enabled = true;
            }

            if (m_section != null && m_systemCall == false)
            {
                ComponentContents contents = (ComponentContents)this.Parent.Parent;
                contents.SendLogState(m_section);
            }

            // btnExecute.Enabled는 실행버튼이 눌려졌는지 여부를 판단하는 기준이 되므로, Log를 기록한 이후에 상태를 바꾼다.
            if (checkBoxComplete.Checked)
            {
                btnExecute.Enabled = false;

                // 완료 버튼이 눌려졌으므로 [다음] 버튼을 누른것과 같은 효과를 내도록 한다.
                if (FormSOP.Instance.HasControl)
                {
                    ComponentContents contents = (ComponentContents)this.Parent.Parent;
                    contents.ClickNextButtonIfItisEnabled();
                }
            }
        }
        
        public void SetBroadcastOptions(VariousData<int> nBroadcastCount, VariousData<bool> useSiren, VariousData<bool> runExecute, VariousData<DateTime> dtExecute, VariousData<bool> checkedComplete, VariousData<DateTime> dtComplete, string strMsg)
        {
            if (nBroadcastCount != null && nBroadcastCount.Data >= 0 && nBroadcastCount.Data < cboBroadcastCount.Items.Count)
                cboBroadcastCount.SelectedIndex = nBroadcastCount.Data - 1;

            if (useSiren != null)
                checkBoxSiren.Checked = useSiren.Data;

            if (runExecute != null)
            {
                btnExecute.Enabled = !runExecute.Data;

                if (runExecute.Data)
                    m_dtExecute = dtExecute;
            }

            if (checkedComplete != null)
            {
                m_systemCall = true;
                checkBoxComplete.Checked = checkedComplete.Data;
                m_systemCall = false;

                if (checkedComplete.Data)
                    m_dtComplete = dtComplete;
            }

            if (strMsg != null)
                textBoxMessage.Text = strMsg;

        }

        public void GetBroadcastOptions(out int nBroadcastCount, out bool useSiren, out bool runExecute, out bool checkedComplete, out string strMsg)
        {
            nBroadcastCount = cboBroadcastCount.SelectedIndex + 1;
            useSiren = checkBoxSiren.Checked;
            runExecute = !btnExecute.Enabled;
            checkedComplete = checkBoxComplete.Checked;
            strMsg = textBoxMessage.Text;
        }

        public void SetSMSOptions(Sections.SectionCommander commander, string strDisplayText, List<Sections.SOPTeam> receivers, VariousData<bool> onlyTeamLeader, string strMsg, VariousData<bool> runExecute, VariousData<DateTime> dtExecute, VariousData<bool> checkedComplete, VariousData<DateTime> dtComplete)
        {
            if (runExecute != null)
            {
                m_commander = commander;
                m_receiverTeams = receivers;

                if (onlyTeamLeader != null)
                    m_onlyTeamLeaderReceiver = onlyTeamLeader.Data;

                if (strDisplayText != null && (m_commander == null || strDisplayText.Length > 0))
                {
                    textBoxSender.Text = strDisplayText;

                    if (m_commander != null)
                        m_commander.DisplayText = strDisplayText;
                }
                else if (m_commander != null)
                    textBoxSender.Text = m_commander.DisplayText;

                if (m_receiverTeams == null)
                    textBoxReceiver.Text = "";
                else
                {
                    string strReceivers = "";

                    foreach (Sections.SOPTeam team in receivers)
                    {
                        string strTeamName = team.IncludeChildTeams ? team.TeamName + "(+)" : team.TeamName;

                        if (strReceivers.Length == 0)
                            strReceivers = strTeamName;
                        else
                            strReceivers += ", " + strTeamName;
                    }

                    textBoxReceiver.Text = strReceivers;
                }

                btnExecute.Enabled = !runExecute.Data;

                if (runExecute.Data)
                    m_dtExecute = dtExecute;
            }

            if (checkedComplete != null)
            {
                m_systemCall = true;
                checkBoxComplete.Checked = checkedComplete.Data;
                m_systemCall = false;

                if (checkedComplete.Data)
                    m_dtComplete = dtComplete;
            }

            if (strMsg != null)
                textBoxMessage.Text = strMsg;

        }

        public bool GetSMSOptions(out string strCommanderText, out string strReceiverText, out bool runExecute, out bool checkedComplete, out string strMsg)
        {
            strCommanderText = strReceiverText = strMsg = "";
            runExecute = checkedComplete = false;

            //if (m_section == null)
            //    return false;

            //Sections.SectionDataInternal data = (Sections.SectionDataInternal)m_section.Data;

            if (m_commander == null)
                strCommanderText = "";
            else
            {
                int nTeamType = -1;

                if (m_commander.Team != null)
                {
                    nTeamType = (int)m_commander.Team.TeamType;

                    if (m_commander.IsTeamMember)
                    {
                        if (m_commander.Team.TeamType == Sections.SOPTeam.SOPTeamType.Normal || m_commander.Team.TeamType == Sections.SOPTeam.SOPTeamType.Holiday ||
                            m_commander.Team.TeamType == Sections.SOPTeam.SOPTeamType.External)
                            nTeamType += 5;
                        else if (m_commander.Team.TeamType == Sections.SOPTeam.SOPTeamType.Regular)
                            nTeamType += 4;
                        else
                            return false;
                    }

                    strCommanderText = string.Format("{0}({1})", m_commander.Team.TeamID, nTeamType);
                }
                else
                    strCommanderText = "-1";
            }

            if (textBoxSender.Text.Length > 0)
                strCommanderText += ", " + textBoxSender.Text;

            if (m_receiverTeams != null)
            {
                foreach (Sections.SOPTeam team in m_receiverTeams)
                {
                    int nTeamID = team.TeamID;

                    if (team.IncludeChildTeams == false)
                    {
                        if (team.TeamType == Sections.SOPTeam.SOPTeamType.Normal || team.TeamType == Sections.SOPTeam.SOPTeamType.Holiday ||
                            team.TeamType == Sections.SOPTeam.SOPTeamType.External || team.TeamType == Sections.SOPTeam.SOPTeamType.Regular)
                            nTeamID = -nTeamID;
                    }

                    if (strReceiverText.Length == 0)
                        strReceiverText = string.Format("{0}({1})", nTeamID, (int)team.TeamType);
                    else
                        strReceiverText += string.Format(", {0}({1})", nTeamID, (int)team.TeamType);
                }
            }

            if (m_onlyTeamLeaderReceiver)
                strReceiverText += ", 1";
            else
                strReceiverText += ", 0";

            /*foreach (Sections.SOPTeam team in data.TeamList)
            {
                if (strReceiverText.Length == 0)
                    strReceiverText = string.Format("{0}({1})", team.TeamID, (int)team.TeamType);
                else
                    strReceiverText += string.Format(", {0}({1})", team.TeamID, (int)team.TeamType);
            }*/

            runExecute = !btnExecute.Enabled;
            checkedComplete = checkBoxComplete.Checked;
            strMsg = textBoxMessage.Text;

            return true;
        }

        public string ChangeCommanderName(UnE.SOP.Workstate.SectionState sectionState)
        {
            if (m_useBroadcast)
                return string.Empty;

            if (m_commander == null || m_commander.Team != null)
                return string.Empty;

            if (sectionState == null)
                return string.Empty;

            string strCommanderName, strUserName = null;
            ComponentContents contents = (ComponentContents)this.Parent.Parent;

            if (/*sectionState.Time != null || */sectionState.DetailDatas.Count > 0)
            {
                if (contents.GetAccessedUserName(out strCommanderName, sectionState))
                    strUserName = strCommanderName;
            }

            if (strUserName == null)
            {
                strUserName = contents.GetCurrentAccessedUserName(sectionState.Time == null ? DateTime.Now : sectionState.Time.Data);

                if (strUserName == null)
                    strUserName = m_strCommanderName2;
            }

            textBoxSender.Text = strUserName;
            return strUserName;
        }

        private void textBoxMessage_Click(object sender, EventArgs e)
        {
            PopupMissionDetail();
        }

        private void PopupMissionDetail()
        {
            // 임무 상세창 팝업
            // added by mwkim 2015-10-06 임무 상세창 팝업
            if (m_runMode)
            {
                if (FormSOP.Instance.HasControl == false)
                    return;

                Sections.SectionDataInternal data = (Sections.SectionDataInternal)m_section.Data;

                string szMissionText = textBoxMessage.Text;
                string szToTarget = "";

                if (data.UseMobileApp == true)
                {
                    szToTarget = textBoxReceiver.Text;
                }

                PopupMissionText form = PopupMissionText.Instance;
                form.SetText(szMissionText, szToTarget, "", "", "", this.Section);

                Activate();
                textBoxMessage.Focus();
            }
        }

        public void ChangeVisiblityToPerformer(bool isVisible)
        {
            labelSender.Visible = textBoxSender.Visible = isVisible;
        }
    }
}
