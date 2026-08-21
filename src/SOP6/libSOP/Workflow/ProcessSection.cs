using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using UnE.SOP.Workstate;


namespace UnE.SOP.Process
{

    /// <summary>
    /// Section의 실행상태 정보를 갖는 SectionState 의 현재 진행 상태
    /// </summary>
    public enum ProcessSectionState
    {
        STANDBY = 1,
        RUNNING = 2,
        ABORT = 3,
        END = 4,
        ERROR = 5
    }

    /// <summary>
    /// ProcessSection의 Progress 시작, 종료 이벤트의 인자
    /// 상속받아서 필요한 내용을 넣도록한다.
    /// </summary>
    public class ProcessSectionEventArgs
    {
        public ProcessSectionEventArgs()
        {
        }
    }

     

    /// <summary>
    /// ProcessSection의 Progress 시작 전에 호출되는 이벤트
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void PreProcessEvent(object sender, ProcessSectionEventArgs e);

    /// <summary>
    /// ProcessSection의 Progress 종료 이전에 호출되는 이벤트
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void PostProcessEvent(object sender, ProcessSectionEventArgs e);

    /// <summary>
    /// ProcessSection의 기본 함수
    /// </summary>
    public abstract class ProcessSectionIF
    {
        public virtual event PreProcessEvent OnPreProcess;
        public virtual event PostProcessEvent OnPostProcess;
        protected ProcessSectionState nState = ProcessSectionState.STANDBY;
        public ProcessSectionState State
        {
            get { return nState; }
            set { nState = value; }
        }
        protected Thread mThread = null;
        public System.Threading.Thread Thread
        {
            get { return mThread; }
            set { mThread = value; }
        }
        public virtual void Progress() { }

        public virtual void Dispose() { }

        public virtual void StartBrodcast()
        {
        }

        public virtual void SendSMSMessage()
        {
        }

        protected void SendMessage(ArrayList callList, string szCaller, string szMessage)
        {
            if (callList == null || callList.Count == 0)
                return;
            string message = szMessage;

            //if (FormMain.Instance.UseEzSMS == false)
            {
                SMS.SMSManager.Instance.SendSMS(callList, szCaller, szMessage);
            }
            /*else
            {
                try
                {
                    ArrayList list = new ArrayList();
                    foreach (string callNumber in callList)
                    {
                        if (callNumber == null)
                            continue;

                        if (callNumber == "" || callNumber == "0" || callNumber == "0000000")
                            continue;

                        list.Add(callNumber);
                    }
                    //list.Clear();
                    // list.Add("01043632290");

                    //if (list.Count > 0)
                    //{
                    //    ezSMSComponent.ISMS m_sms = new ezSMSComponent.SMS();
                    //    m_sms.ServiceCode = "020026C9FCC7C39E41A88C2CF52D00D7BAA6";
                    //    ezSMSComponent.LoginInfo login = m_sms.Login("121.254.175.25", 4545, "unes", "unes0101");
                    //    ezSMSComponent.Receivers receiver = m_sms.CreateReceivers();

                    //    foreach (string callNumber in list)
                    //    {
                    //        receiver.AddDirect(callNumber, message, ezSMSComponent.EZSMS_SENDMODE.EZSMS_DIRECT, FlexTimer.Now);
                    //    }

                    //    ezSMSComponent.SendResults results = m_sms.SendSMS(szCaller, receiver);
                    //    foreach (ezSMSComponent.SendResult result in results)
                    //    {
                    //        if (result.Result != ezSMSComponent.EZSMS_RESULT.EZSMS_SUCCEEDED)
                    //        {
                    //        }
                    //    }
                    //}
                }
                catch (Exception)
                {
                }
            }*/
        }

        // strMsg에 {time}, {location}과 같은 특수 문자열이 존재하면
        // 해당 내용을 실제 시간과 장소로 바꾸어준다.
        public string ParseSpecialMessage(string strMsg, DateTime dtTime, string strPlace, bool isRealMode, bool isNormalMode, string strAlarmMessage, HistoryDisasterPosition position)
        {
            UnE.SOP.Utility.SOPSimulatorScript.DataParameter param = new UnE.SOP.Utility.SOPSimulatorScript.DataParameter(strMsg, dtTime, strPlace, strAlarmMessage);

            param.RealMode = isRealMode ? 1 : 0;
            param.NormalMode = isNormalMode ? 1 : 0;

            if (position != null)
            {
                if (position.UsePSM)
                {
                    param.PSMMaterialType = position.PSMMaterial;
                    param.PSMDistance = position.PSMDistance;
                }
            }

            return UnE.SOP.Utility.SOPSimulatorScript.Parse(param);
            //return UnE.SOP.Utility.SOPSimulatorScript.Parse(strMsg, dtTime, strPlace, isRealMode ? 1 : 0, isNormalMode ? 1 : 0);
        }
    }

    public abstract class ProcessSectionFactory
    {
        public abstract ProcessSectionIF CreateProcess(SectionState state);
    }
}
