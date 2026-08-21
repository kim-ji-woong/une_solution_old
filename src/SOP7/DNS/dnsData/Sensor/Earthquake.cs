using System;
using System.Collections.Generic;
using System.Text;

namespace dnsData.Sensor
{
    public class EarthquakeOption : IComparable
    {
        public enum IntensOption
        {
            NONE = -1,
            I_MIN_GE_MAX_LT = 0,    // (진도) 최소값 이상 ~ 최대값 미만
            I_MIN_GT_MAX_LE,        // (진도) 최소값 초과 ~ 최대값 이하
            I_MIN_LT,               // (진도) 최소값 미만
            I_MIN_LE,               // (진도) 최소값 이하
            I_MAX_GT,               // (진도) 최대값 초과
            I_MAX_GE,               // (진도) 최대값 이상
            M_MIN_GE_MAX_LT,        // (규모) 최소값 이상 ~ 최대값 미만
            M_MIN_GT_MAX_LE,        // (규모) 최소값 초과 ~ 최대값 이하
            M_MIN_LT,               // (규모) 최소값 미만
            M_MIN_LE,               // (규모) 최소값 이하
            M_MAX_GT,               // (규모) 최대값 초과
            M_MAX_GE                // (규모) 최대값 이상
        }

        private double m_dMin = 0.0;
        private double m_dMax = 0.0;
        private IntensOption m_opt = IntensOption.NONE;
        private bool m_useSMS = false;
        private string m_strSMS = "";
        private bool m_useBroadcast = false;
        private string m_strBroadcast = "";
        private bool m_runSOP = false;
        private string m_strLinkedSOP = "";

        // 이 값이 true이면 진도, false이면 규모
        public bool IsIntensity
        {
            get { return m_opt <= IntensOption.I_MAX_GE; }
        }

        // 최소값
        public double Minimum
        {
            get { return m_dMin; }
            set { m_dMin = value; }
        }

        // 최대값
        public double Maximum
        {
            get { return m_dMax; }
            set { m_dMax = value; }
        }

        // 최대, 최소값 옵션
        public IntensOption MinMaxOption
        {
            get { return m_opt; }
            set { m_opt = value; }
        }

        public bool BothMinMax
        {
            get { return m_opt == IntensOption.I_MIN_GE_MAX_LT || m_opt == IntensOption.I_MIN_GT_MAX_LE || m_opt == IntensOption.M_MIN_GE_MAX_LT || m_opt == IntensOption.M_MIN_GT_MAX_LE; }
        }

        public bool OnlyMin
        {
            get { return m_opt == IntensOption.I_MIN_LE || m_opt == IntensOption.I_MIN_LT || m_opt == IntensOption.M_MIN_LE || m_opt == IntensOption.M_MIN_LT; }
        }

        public bool OnlyMax
        {
            get { return m_opt == IntensOption.I_MAX_GE || m_opt == IntensOption.I_MAX_GT || m_opt == IntensOption.M_MAX_GE || m_opt == IntensOption.M_MAX_GT; }
        }

        // 문자메시지 사용 여부
        public bool UseSMS
        {
            get { return m_useSMS; }
            set { m_useSMS = value; }
        }

        // 문자메시지 발송 문구
        public string SMSMessage
        {
            get { return m_strSMS; }
            set { m_strSMS = value; }
        }

        // 방송 사용 여부
        public bool UseBroadcast
        {
            get { return m_useBroadcast; }
            set { m_useBroadcast = value; }
        }

        // 방송 문구
        public string BroadcastMessage
        {
            get { return m_strBroadcast; }
            set { m_strBroadcast = value; }
        }

        // 지진신호 발생시 SOP 자동실행 여부
        public bool RunSOP
        {
            get { return m_runSOP; }
            set { m_runSOP = value; }
        }

        // 지진신호 발생시 사용될 SOP
        public string LinkedSOP
        {
            get { return m_strLinkedSOP; }
            set { m_strLinkedSOP = value; }
        }

        public void SetMinMaxOption(int opt)
        {
            if (opt >= (int)IntensOption.I_MIN_GE_MAX_LT && opt <= (int)IntensOption.M_MAX_GE)
                m_opt = (IntensOption)opt;
            else
                m_opt = IntensOption.NONE;
        }

        public int CompareTo(object obj)
        {
            EarthquakeOption option = (EarthquakeOption)obj;

            if (this.IsIntensity != option.IsIntensity)
                return -1;

            if (this.MinMaxOption == IntensOption.I_MIN_GE_MAX_LT || this.MinMaxOption == IntensOption.I_MIN_GT_MAX_LE ||
                this.MinMaxOption == IntensOption.M_MIN_GE_MAX_LT || this.MinMaxOption == IntensOption.M_MIN_GT_MAX_LE)
            {
                if (option.MinMaxOption == IntensOption.I_MIN_GE_MAX_LT || option.MinMaxOption == IntensOption.I_MIN_GT_MAX_LE ||
                    option.MinMaxOption == IntensOption.M_MIN_GE_MAX_LT || option.MinMaxOption == IntensOption.M_MIN_GT_MAX_LE)
                {
                    if (this.m_dMin > option.m_dMin)
                        return 1;
                    else if (this.m_dMin < option.m_dMin)
                        return -1;
                    else
                        return 0;
                }
                else if (option.MinMaxOption == IntensOption.I_MIN_LE || option.MinMaxOption == IntensOption.I_MIN_LT ||
                    option.MinMaxOption == IntensOption.M_MIN_LE || option.MinMaxOption == IntensOption.M_MIN_LT)
                    return 1;
                else if (option.MinMaxOption == IntensOption.I_MAX_GE || option.MinMaxOption == IntensOption.I_MAX_GT ||
                    option.MinMaxOption == IntensOption.M_MAX_GE || option.MinMaxOption == IntensOption.M_MAX_GT)
                    return -1;
            }
            else if (this.MinMaxOption == IntensOption.I_MIN_LE || this.MinMaxOption == IntensOption.I_MIN_LT ||
                    this.MinMaxOption == IntensOption.M_MIN_LE || this.MinMaxOption == IntensOption.M_MIN_LT)
            {
                if (option.MinMaxOption == IntensOption.I_MIN_GE_MAX_LT || option.MinMaxOption == IntensOption.I_MIN_GT_MAX_LE ||
                    option.MinMaxOption == IntensOption.M_MIN_GE_MAX_LT || option.MinMaxOption == IntensOption.M_MIN_GT_MAX_LE)
                    return -1;
                else if (option.MinMaxOption == IntensOption.I_MIN_LE || option.MinMaxOption == IntensOption.I_MIN_LT ||
                    option.MinMaxOption == IntensOption.M_MIN_LE || option.MinMaxOption == IntensOption.M_MIN_LT)
                {
                    if (this.m_dMin > option.m_dMin)
                        return 1;
                    else if (this.m_dMin < option.m_dMin)
                        return -1;
                    else
                        return 0;
                }
                else if (option.MinMaxOption == IntensOption.I_MAX_GE || option.MinMaxOption == IntensOption.I_MAX_GT ||
                    option.MinMaxOption == IntensOption.M_MAX_GE || option.MinMaxOption == IntensOption.M_MAX_GT)
                    return -1;
            }
            else if (this.MinMaxOption == IntensOption.I_MAX_GE || option.MinMaxOption == IntensOption.I_MAX_GT ||
                    option.MinMaxOption == IntensOption.M_MAX_GE || option.MinMaxOption == IntensOption.M_MAX_GT)
            {
                if (option.MinMaxOption == IntensOption.I_MIN_GE_MAX_LT || option.MinMaxOption == IntensOption.I_MIN_GT_MAX_LE ||
                    option.MinMaxOption == IntensOption.M_MIN_GE_MAX_LT || option.MinMaxOption == IntensOption.M_MIN_GT_MAX_LE)
                    return 1;
                else if (option.MinMaxOption == IntensOption.I_MIN_LE || option.MinMaxOption == IntensOption.I_MIN_LT ||
                    option.MinMaxOption == IntensOption.M_MIN_LE || option.MinMaxOption == IntensOption.M_MIN_LT)
                    return 1;
                else if (option.MinMaxOption == IntensOption.I_MAX_GE || option.MinMaxOption == IntensOption.I_MAX_GT ||
                    option.MinMaxOption == IntensOption.M_MAX_GE || option.MinMaxOption == IntensOption.M_MAX_GT)
                {
                    if (this.m_dMin > option.m_dMin)
                        return 1;
                    else if (this.m_dMin < option.m_dMin)
                        return -1;
                    else
                        return 0;
                }
            }
            //else
            return -1;
        }

        /*public static List<EarthquakeOption> LoadOptions(WebDBManager dbMgr)
        {
            string strSQL = "Select MinIntens, MaxIntens, IntensOption, UseSMS, SMSMessage, UseBroadcast, BroadcastMessage from OptionEarthquake";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            List<EarthquakeOption> options = new List<EarthquakeOption>();
            int nResultData = arrResult.Count;

            for (int i = 0; i < nResultData - 6; i += 7)
            {
                VariousData<float> min = WebDBManager.GetFloatField(arrResult[i].ToString());
                VariousData<float> max = WebDBManager.GetFloatField(arrResult[i + 1].ToString());
                VariousData<int> option = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<int> useSMS = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                string strSMS = WebDBManager.GetStringField(arrResult[i + 4]);
                VariousData<int> useBroadcast = WebDBManager.GetIntField(arrResult[i + 5].ToString());
                string strBroadcast = WebDBManager.GetStringField(arrResult[i + 6]);

                if (min == null || max == null || option == null || useSMS == null || useBroadcast == null)
                    continue;

                EarthquakeOption opt = new EarthquakeOption();
                opt.Minimum = min.Data;
                opt.Maximum = max.Data;
                opt.SetMinMaxOption(option.Data);
                opt.UseSMS = useSMS.Data == 1 ? true : false;
                opt.SMSMessage = strSMS == null ? "" : strSMS;
                opt.UseBroadcast = useBroadcast.Data == 1 ? true : false;
                opt.BroadcastMessage = strBroadcast == null ? "" : strBroadcast;

                options.Add(opt);
            }

            options.Sort();
            return options;
        }*/

        // options 가운데 nIntensity 또는 fMagnitude를 만족하는 option을 찾아낸다.
        // nIntensity : 진도
        // fMagnitude : 규모
        public static EarthquakeOption GetOption(int nIntensity, float fMagnitude, List<EarthquakeOption> options)
        {
            if (options == null)
                return null;

            foreach (EarthquakeOption option in options)
            {
                if (option.m_opt == IntensOption.NONE)
                    continue;

                if (nIntensity > 0 && option.IsIntensity)
                {
                    if (option.MinMaxOption == IntensOption.I_MIN_LT)
                    {
                        if (nIntensity < option.m_dMin)
                            return option;
                    }
                    else if (option.MinMaxOption == IntensOption.I_MIN_LE)
                    {
                        if (nIntensity <= option.m_dMin)
                            return option;
                    }
                    else if (option.MinMaxOption == IntensOption.I_MIN_GE_MAX_LT)
                    {
                        if (nIntensity >= option.m_dMin && nIntensity < option.m_dMax)
                            return option;
                    }
                    else if (option.MinMaxOption == IntensOption.I_MIN_GT_MAX_LE)
                    {
                        if (nIntensity > option.m_dMin && nIntensity <= option.m_dMax)
                            return option;
                    }
                    else if (option.MinMaxOption == IntensOption.I_MAX_GT)
                    {
                        if (nIntensity > option.m_dMax)
                            return option;
                    }
                    else if (option.MinMaxOption == IntensOption.I_MAX_GE)
                    {
                        if (nIntensity >= option.m_dMax)
                            return option;
                    }
                }
                else if (fMagnitude > 0.0f && !option.IsIntensity)
                {
                    if (option.MinMaxOption == IntensOption.M_MIN_LT)
                    {
                        if (fMagnitude < option.m_dMin)
                            return option;
                    }
                    else if (option.MinMaxOption == IntensOption.M_MIN_LE)
                    {
                        if (fMagnitude <= option.m_dMin)
                            return option;
                    }
                    else if (option.MinMaxOption == IntensOption.M_MIN_GE_MAX_LT)
                    {
                        if (fMagnitude >= option.m_dMin && fMagnitude < option.m_dMax)
                            return option;
                    }
                    else if (option.MinMaxOption == IntensOption.M_MIN_GT_MAX_LE)
                    {
                        if (fMagnitude > option.m_dMin && fMagnitude <= option.m_dMax)
                            return option;
                    }
                    else if (option.MinMaxOption == IntensOption.M_MAX_GT)
                    {
                        if (fMagnitude > option.m_dMax)
                            return option;
                    }
                    else if (option.MinMaxOption == IntensOption.M_MAX_GE)
                    {
                        if (fMagnitude >= option.m_dMax)
                            return option;
                    }
                }
            }

            return null;
        }
    }
}
