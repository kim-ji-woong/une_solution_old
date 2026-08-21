using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDMS;
using TcpLib2;
using System.Collections;
using System.Threading;
using SOP;
using DBUtility;

namespace SDMSServer
{
    public class ClientDataEarthquakeSensorServer : ClientData
    {
        private int m_nSiteID = 1;

        public ClientDataEarthquakeSensorServer(ServiceProvider provider)
        {
            m_nSiteID = NetworkServer.Instance.SiteID;

            m_provider = provider;
            ClientType = TCP_CLIENT.EARTHQUAKE_SENSOR_SERVER;
        }

        // OnAccept() 이후 WhoIAm을 받은 뒤 처리해야 할 로직
        protected override bool ProcessFirstConnection(ConnectionState state)
        {
            return true;
        }

        // bytes는 length byte가 제거되었음
        protected override bool OnReceive(ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
        {
            if (nHeader == TCP_ID.EARTHQUAKE_SENSOR_DETECT)
            {
                int nDataCount = arrDatas.Count;

                if (nDataCount < 6)
                    return false;

                if (arrDatas[0] is int && arrDatas[1] is float && arrDatas[2] is int && arrDatas[3] is int && arrDatas[4] is string && arrDatas[5] is long)
                {
                    int nSensorID = (int)arrDatas[0];
                    float fMagnitude = (float)arrDatas[1];
                    int nIntensity = (int)arrDatas[2];
                    int nAlarmLevel = (int)arrDatas[3];
                    string strPosition = (string)arrDatas[4];
                    DBUtility.VariousData<DateTime> time = (long)arrDatas[5] == 0 ? null : new DBUtility.VariousData<DateTime>(DateTime.FromBinary((long)arrDatas[5]));

                    List<UnE.Earthquake.EarthquakeOption> options = UnE.Earthquake.EarthquakeOption.LoadOptions(NetworkServer.Instance.DBManager);

                    if (options == null)
                        return true;

                    UnE.Earthquake.EarthquakeOption option = UnE.Earthquake.EarthquakeOption.GetOption(nIntensity, fMagnitude, options);

                    if (option == null)
                        return true;

                    string strShelterName = "";

                    if (option.UseSMS)
                        SendSMS(option.SMSMessage, nIntensity, fMagnitude, strPosition, ref strShelterName);

                    if (option.UseBroadcast)
                        RunBroadcast(option.BroadcastMessage, nIntensity, fMagnitude, strPosition, strShelterName);

                    m_provider.SendData(bytes, false, TCP_CLIENT.SDMS_CLIENT);
                    //m_provider.SendData(bytes, false, ClientType.SOP_SIMULATOR);

                    if (option.LinkedSOP.Length > 0)
                    {
                        ArrayList datas = new ArrayList();

                        datas.AddRange(arrDatas);
                        datas.Add(option.RunSOP);
                        datas.Add(option.LinkedSOP);

                        byte[] bytesData = TcpHelper.MakeBytes((short)TCP_ID.EARTHQUAKE_SENSOR_DETECT, datas);
                        m_provider.SendData(bytesData, false, TCP_CLIENT.SOP_SIMULATOR);
                    }
                }
            }
            else if (nHeader == TCP_ID.SDMS_COMMAND)
            {
                m_provider.SendData(bytes, false, TCP_CLIENT.SDMS_CLIENT);
            }
            else if (nHeader == TCP_ID.COLLAPSE_BUILDING_DETECT)
            {
                m_provider.SendData(bytes, false, TCP_CLIENT.SDMS_CLIENT);
            }
            
            return true;
        }

        // strPosition : 진앙지
        private void RunBroadcast(string strMessage, int nIntensity, float fMagnitude, string strPosition, string strShelterName)
        {
            if (strMessage == null)
                return;

            strMessage = strMessage.Trim();

            if (strMessage.Length == 0)
                return;

            if (strShelterName.Length == 0)
            {
                UnE.Spatial.ZoneManager.Instance.LoadShelters();
                Dictionary<int, UnE.Spatial.Shelter> dicShelters = UnE.Spatial.ZoneManager.Instance.GetShelters(UnE.Spatial.Shelter.ShelterTypes.Earthquake);

                strShelterName = "대피소";

                if (dicShelters != null)
                {
                    foreach (KeyValuePair<int, UnE.Spatial.Shelter> pair in dicShelters)
                    {
                        strShelterName = pair.Value.ShelterName;
                        break;
                    }
                }
            }

            ReplaceString("{INTENS}", nIntensity.ToString(), ref strMessage);
            ReplaceString("{MAGNIT}", string.Format("{0:F1}", fMagnitude), ref strMessage);
            ReplaceString("{SHELTER}", strShelterName, ref strMessage);

            BroadcastManager.Instance.AddSpeech(strMessage, 1, true, BroadcastManager.SituationType.ALWAYS);
        }

        // strPosition : 진앙지
        private void SendSMS(string strMessage, int nIntensity, float fMagnitude, string strPosition, ref string strShelterName)
        {
            if (strMessage == null)
                return;

            strMessage = strMessage.Trim();

            if (strMessage.Length == 0)
                return;

            UnE.Spatial.ZoneManager.Instance.LoadShelters();
            Dictionary<int, UnE.Spatial.Shelter> dicShelters = UnE.Spatial.ZoneManager.Instance.GetShelters(UnE.Spatial.Shelter.ShelterTypes.Earthquake);

            strShelterName = "대피소";

            if (dicShelters != null)
            {
                foreach (KeyValuePair<int, UnE.Spatial.Shelter> pair in dicShelters)
                {
                    strShelterName = pair.Value.ShelterName;
                    break;
                }
            }

            ReplaceString("{INTENS}", nIntensity.ToString(), ref strMessage);
            ReplaceString("{MAGNIT}", string.Format("{0:F1}", fMagnitude), ref strMessage);
            ReplaceString("{SHELTER}", strShelterName, ref strMessage);

            m_provider.SendSMSToAllCompanyMember(strMessage);
        }

        // string.Replace()는 대소문자를 엄격히 구별하여 사용하여야 한다.
        // 대소문자 구별없이 같은 기능을 수행한다.
        private void ReplaceString(string strSrc, string strTrg, ref string strMessage)
        {
            int nSrcLen = strSrc.Length;
            strSrc = strSrc.ToLower();

            string strLow = strMessage.ToLower();

            int nIndex = 0;

            do
            {
                nIndex = strLow.IndexOf(strSrc, nIndex);

                if (nIndex >= 0)
                {
                    strLow = strLow.Substring(0, nIndex) + strTrg + strLow.Substring(nIndex + nSrcLen);
                    strMessage = strMessage.Substring(0, nIndex) + strTrg + strMessage.Substring(nIndex + nSrcLen);
                }
            }
            while (nIndex >= 0);
        }
    }
}
