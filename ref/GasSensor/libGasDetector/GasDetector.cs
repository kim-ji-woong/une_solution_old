using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace GasDetector
{
    internal class GasDetector
    {
        public event AlarmNotifyDelegate OnNotifyAlarm;

        AlarmUnit[] m_Units = null;
        float[] m_UnitValues = null;

        private int m_nFunction = 3;
        public int Function
        {
            get { return m_nFunction; }
            set { m_nFunction = value; }
        }

        private int m_nStatusFunction = 1;


        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
        }

        private int m_nLastUnit = 0;
        private int m_nStartUnit = 0;

        private int m_nLastChannel = 0;

        public GasDetector(int nID, int nStartAddress, int EndAddress,  AlarmUnit[] aUnits)
        {
            m_nID = nID;
            m_nLastUnit = EndAddress;
            m_nStartUnit = nStartAddress;
            int nLength = aUnits.Length;
            m_UnitValues = new float[nLength];
            for (int i = 0; i < nLength; i++)
            {
                m_UnitValues[i] = -999.0f;
            }

            for(int i = 0 ; i < aUnits.Length ; i++)
            {
                if (m_nLastChannel < aUnits[i].Channel)
                    m_nLastChannel = aUnits[i].Channel;
                m_nStatusFunction = aUnits[i].Function;
            }
            m_Units = aUnits;
        }
                
        private bool m_bBuzStop = false;
        private bool m_bReset = false;

        private bool m_bAddedStop = false;
        private bool m_bAddedReset = false;

        private int m_nCmdFunc = 5;
        public void AddCommand(int nFunction, int nRegister, int nValue)
        {
            m_nCmdFunc = nFunction;
            if(nRegister == 0)
            {
                if( nValue == 1)
                    m_bReset = true;
                else
                    m_bReset = false;

                m_bAddedReset = true;
            }

            else if( nRegister == 1)
            {
                if( nValue == 1)
                    m_bBuzStop = true;
                else
                    m_bBuzStop = false;

                m_bAddedStop = true;
            }            
        }

        public void CheckValue(SerialManager sm, int nTimeOut)
        {
            if( sm.IsConnected == true)
            {
                byte[] data = MsgHelper.MakeData((byte)0, 0, 0, (byte)m_nLastUnit, (byte)m_nFunction, (byte)m_nID);
                sm.SendBytes(data); // value check

                Thread.Sleep(5);

                byte[] smReadBuffer = sm.ReciveBuffer;
                if (smReadBuffer == null)
                {
                    sm.ClearBuffer();
                    sm.SendBytes(data);
                    Thread.Sleep(5);
                    smReadBuffer = sm.ReciveBuffer;
                }

                if (smReadBuffer != null)
                {
                    SetDetectorValue(smReadBuffer);
                    sm.ClearBuffer();

                    byte[] data2 = MsgHelper.MakeData((byte)0, 0, 0, (byte)m_nLastChannel, (byte)1, (byte)m_nID);
                    sm.SendBytes(data2); // Unit Status'

                    Thread.Sleep(5);

                    byte[] smReadBuffer2 = sm.ReciveBuffer;
                    if (smReadBuffer2 == null)
                    {
                        sm.ClearBuffer();
                        sm.SendBytes(data2);
                        Thread.Sleep(5);
                        smReadBuffer2 = sm.ReciveBuffer;
                    }
                    if (smReadBuffer2 != null)
                        SetUnitStatus(smReadBuffer2);
                    sm.ClearBuffer();
                }
                else
                {
                    SetOff();
                }
            }            
        }

        internal void SendCommand(SerialManager sm, int nTimeOut)
        {
            byte[] data = null;
            if (m_bAddedStop == true)
            {
                if (m_bBuzStop == true)
                {
                    if (m_nCmdFunc < 10)
                        data = MsgHelper.MakeData(0, (byte)1, (byte)255, (byte)0, (byte)m_nCmdFunc, (byte)m_nID);
                    else
                        data = MsgHelper.MakeLongCmdData(0, 1, 0, 1, 1, (byte)255, 0, (byte)m_nCmdFunc, (byte)m_nID);
                    sm.SendBytes(data);
                }
                else
                {
                    if (m_nCmdFunc < 10)
                        data = MsgHelper.MakeData(0, (byte)1, (byte)0, (byte)0, (byte)m_nCmdFunc, (byte)m_nID);
                    else
                        data = MsgHelper.MakeLongCmdData(0, 1, 0, 1, 1, (byte)0, 0, (byte)m_nCmdFunc, (byte)m_nID);
                    sm.SendBytes(data);
                }
            }
            if (m_bAddedReset == true)
            {
                if (m_bReset == true)
                {
                    if (m_nCmdFunc < 10)
                        data = MsgHelper.MakeData(0, (byte)0, (byte)255, (byte)0, (byte)m_nCmdFunc, (byte)m_nID);
                    else
                        data = MsgHelper.MakeLongCmdData(0, 0, 0, 1, 1, (byte)255, 0, (byte)m_nCmdFunc, (byte)m_nID);
                    sm.SendBytes(data);
                }
                else
                {
                    if (m_nCmdFunc < 10)
                        data = MsgHelper.MakeData(0, (byte)0, (byte)0, (byte)0, (byte)m_nCmdFunc, (byte)m_nID);
                    else
                        data = MsgHelper.MakeLongCmdData(0, 0, 0, 1, 1, 0, 0, (byte)m_nCmdFunc, (byte)m_nID);
                    sm.SendBytes(data);
                }
                m_bBuzStop = false;
                m_bReset = false;
            }

            m_bAddedStop = false;
            m_bAddedReset = false;
        }

        public void UpdateValue(SerialManager sm, int nTimeOut)
        {
            // Check Notify

            if (m_Units != null)
            {
                for (int i = 0; i < m_Units.Length; i++ )
                {
                    AlarmUnit unit = m_Units[i];
                    int nChannel = unit.Channel;

                    for (int j = 0; j < nChannel; j++ )
                    {
                        int nStatus = 0;
                        if (unit.CheckFireNotify(j, out nStatus) == true)
                        {
                            if (OnNotifyAlarm != null)
                                OnNotifyAlarm(m_nID, i, m_UnitValues[i], j, nStatus);
                        }
                    }                   
                }
            } 

            Thread.Sleep(5);

          
        }

        public void SetOff()
        {
            if(m_UnitValues != null)
            {
                int nUnit = 0;
                for (int i = 0; i < m_UnitValues.Length; i++)
                {
                    m_UnitValues[i] = -999.0f;
                }
            }

            if (m_Units != null)
            {
                for (int i = 0; i < m_Units.Length; i++)
                {
                    AlarmUnit unit = m_Units[i];
                    unit.SetOff();
                }
            }
            
        }

        private void SetDetectorValue(byte[] bStatus)
        {
            // 01 03 04 00 1D 00 40 6B C5

            int nLength = bStatus[2];

            int nUnit = 1;
            for (int i = 0; i < nLength; i += 2)
            {
                byte[] temp = new byte[2];
                temp[1] = bStatus[i+3];
                temp[0] = bStatus[i+4];

                float value = BitConverter.ToInt16(temp, 0) / 100.0f;
                if (value < 0.0f)
                    value = 0.0f;

                m_UnitValues[nUnit++] = value;

                System.Diagnostics.Trace.WriteLine("Value : "  + value);
            }
        }

        public float GetValue(int nUnit)
        {
            if (m_UnitValues == null)
                return -999.0f;
            if (nUnit < 0 || m_UnitValues.Length <= nUnit)
                return -999.0f;

            return m_UnitValues[nUnit];
        }

        public void SetChannelNotify(int nUnit, int nCh, bool bValue)
        {
            if (m_Units == null)
                return;
            if (nUnit < 0 || m_Units.Length <= nUnit)
                return;

            AlarmUnit aUnit = m_Units[nUnit];
            aUnit.SetChannelNotify(nCh, bValue);
        }

        public bool GetChannelNotify(int nUnit, int nCh)
        {
            if (m_Units == null)
                return false;
            if (nUnit < 0 || m_Units.Length <= nUnit)
                return false;

            AlarmUnit aUnit = m_Units[nUnit];
            return aUnit.GetChannelNotify(nCh);
        }

        public int GetStatus(int nUnit, int nCh)
        {
            if (m_Units == null)
                return -1;
            if (nUnit < 0 || m_Units.Length <= nUnit)
                return -1;

            AlarmUnit aUnit = m_Units[nUnit];
            return aUnit.GetChannelStatus(nCh);
        }

        private void SetUnitStatus(byte[] bStatus)
        {
            // 01 01 03 80 00 00 3D A6
            int nLength = bStatus[2];

            if (m_Units == null || m_Units.Length == 0)
                return;

            nLength = Math.Min(m_Units.Length, nLength);

            for (int i = 0; i < nLength; i ++)
            {
                byte temp = bStatus[i + 3];
                byte value = (byte)(temp & 0x0F);

                System.Diagnostics.Trace.WriteLine("AlarmUnit : " + m_Units[i].UnitName);
                m_Units[i].SetStatus(value);
                
            }
        }

        //private byte[] MakeData(byte nBaseHi, byte nBaseLow, byte nHmiHi, byte nHmiLow, byte nFunc, byte nAddress)
        //{
        //    byte[] data = new byte[8];

        //    data[0] = nAddress;
        //    data[1] = nFunc;
        //    data[2] = nBaseHi;
        //    data[3] = nBaseLow;
        //    data[4] = nHmiHi;
        //    data[5] = nHmiLow;

        //    byte[] crc = new byte[2];
        //    GetCRC(data, ref crc);

        //    data[6] = crc[0];
        //    data[7] = crc[1];
        //    return data;
        //}

        //private void GetCRC(byte[] message, ref byte[] CRC)
        //{
        //    ushort CRCFull = 0xFFFF;
        //    byte CRCHigh = 0xFF, CRCLow = 0xFF;
        //    char CRCLSB;

        //    for (int i = 0; i < (message.Length) - 2; i++)
        //    {
        //        CRCFull = (ushort)(CRCFull ^ message[i]);

        //        for (int j = 0; j < 8; j++)
        //        {
        //            CRCLSB = (char)(CRCFull & 0x0001);
        //            CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);

        //            if (CRCLSB == 1)
        //                CRCFull = (ushort)(CRCFull ^ 0xA001);
        //        }
        //    }
        //    CRC[1] = CRCHigh = (byte)((CRCFull >> 8) & 0xFF);
        //    CRC[0] = CRCLow = (byte)(CRCFull & 0xFF);
        //}
    }

    internal class AlarmUnit
    {
        private ChannelInfo[] m_Channel = null;
        private bool[] m_bChecked = null;
        
        private int m_nChannel = 0;
        public int Channel
        {
            get { return m_nChannel; }
        }

        private int m_nFunction = 0;
        public int Function
        {
            get { return m_nFunction; }
            set { m_nFunction = value; }
        }

        private string m_szUnitName = "";
        public string UnitName
        {
            get { return m_szUnitName; }
            set { m_szUnitName = value; }
        }        

        public AlarmUnit(int nUnitID, int nAddress, int nLastAddress, ChannelInfo[] channels)
        {
            m_nChannel = nLastAddress;
            
            if (channels != null && channels.Length > 0)
            {
                //m_nChannel = channels.Length;
                m_Channel = channels;
            }           
        }

        public int GetChannelStatus(int nChannel)
        {
            if (m_Channel == null)
                return -1;
            if (nChannel < 0 || m_Channel.Length <= nChannel)
                return -1;

            ChannelInfo aUnit = m_Channel[nChannel];
            return aUnit.Status;
        }

        private byte m_nStatus = 0;
        public byte Status
        {
            get { return m_nStatus; }
        }

        public void SetStatus(byte nStatus)
        {
            if( m_Channel == null || m_Channel.Length == 0)
                return;
            
            m_nStatus = nStatus;

            byte nMask = 1;
            for(int i = 0 ; i < m_Channel.Length ; i++)
            {
                nMask = (byte)(1 << i);

                byte nResult = (byte)((nStatus & nMask) >> i);

                if (m_Channel[i].Status != nResult)// && m_Channel[i].First == false)
                {
                    
                    if( m_Channel[i].EnableNotify == true)
                    {
                        if (m_Channel[i].First == true && m_Channel[i].TargetValue == nResult)
                        {
                             m_Channel[i].FireNotify = true;
                        }

                        if (m_Channel[i].First == false)
                        {
                            m_Channel[i].FireNotify = true;
                        }
                    }
                }

                m_Channel[i].Status = nResult;
                m_Channel[i].First = false;
                System.Diagnostics.Trace.WriteLine(m_Channel[i].ChannelName + ":" + nResult);
                
            }
        }

        public void SetOff()
        {
            for (int i = 0; i < m_Channel.Length; i++)
            {
                m_Channel[i].FireNotify = false;
                m_Channel[i].First = true;
                m_Channel[i].Status = -1;  
            }
        }

        public void SetChannelNotify(int nChannel, bool bValue)
        {
            if (m_Channel == null || m_Channel.Length == 0)
                return;


            if (nChannel < 0 || m_Channel.Length <= nChannel)
                return;

            m_Channel[nChannel].EnableNotify = bValue;
        }

        public bool GetChannelNotify(int nChannel)
        {
            if (m_Channel == null || m_Channel.Length == 0)
                return false;

            if (nChannel < 0 || m_Channel.Length <= nChannel)
                return false;

            return m_Channel[nChannel].EnableNotify;
        }

        public bool CheckFireNotify(int nChannel, out int value)
        {
            value = 0;
            if( m_Channel == null || m_Channel.Length == 0)
                return false;

            
            if (nChannel < 0 || m_Channel.Length <= nChannel)
                return false;
            
            if(m_Channel[nChannel].FireNotify == true)
            {
                m_Channel[nChannel].FireNotify = false;
                value = m_Channel[nChannel].Status;
                return true;
            }
            return false;
        }
    }

    internal class ChannelInfo
    {
        private int m_nChannelID = 0;
        public int Channel
        {
            get { return m_nChannelID; }
            set { m_nChannelID = value; }
        }

        private string m_szChannelName = "";
        public string ChannelName
        {
            get { return m_szChannelName; }
            set { m_szChannelName = value; }
        }
        
        private int m_nTargetValue = 0;
        public int TargetValue
        {
            get { return m_nTargetValue; }
            set { m_nTargetValue = value; }
        }

        private int m_nStatus = 0;
        public int Status
        {
            get { return m_nStatus; }
            set { m_nStatus = value; }
        }

        private bool m_bEnableNotify = true;
        public bool EnableNotify
        {
            get { return m_bEnableNotify; }
            set { m_bEnableNotify = value; }
        }

        private bool m_bFireNotify = false;
        public bool FireNotify
        {
            get { return m_bFireNotify; }
            set { m_bFireNotify = value; }
        }

        private bool m_bFirst = true;
        public bool First
        {
            get { return m_bFirst; }
            set { m_bFirst = value; }
        }

    }
}
