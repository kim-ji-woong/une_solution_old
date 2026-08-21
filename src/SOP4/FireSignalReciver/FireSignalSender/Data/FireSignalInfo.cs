using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FireSignalSender
{

    public class FireSignalInfo
    {
        private DateTime m_Time;
        public DateTime Time
        {
            get { return m_Time; }
            set { m_Time = value; }
        }

        private int m_nReciverNo;
        public int ReciverNo
        {
            get { return m_nReciverNo; }
            set { m_nReciverNo = value; }
        }

        private string szFuncionCode = "";
        public string FuncionCode
        {
            get { return szFuncionCode; }
            set { szFuncionCode = value; }
        }


        private string szCode;
        public string Code
        {
            get { return szCode; }
            set
            {
                szCode = value;

                try
                {
                    if (szCode != null && szCode != "")
                    {
                        char[] arr = szCode.ToCharArray();
                        char c = arr[arr.Length - 1];
                        if (c == 'F')
                        {
                            m_bOff = true;
                        }
                        szFuncionCode = szCode.Substring(0, arr.Length - 1);
                    }
                }
                catch (Exception)
                {

                }

            }
        }

        private string szName;
        public string Name
        {
            get { return szName; }
            set { szName = value; }
        }

        private string szCircuit;
        public string Circuit
        {
            get { return szCircuit; }
            set
            {

                szCircuit = value;

            }
        }

        private bool m_bOff = false;
        public bool IsOff
        {
            get { return m_bOff; }
        }


        public static int CompareSignal(FireSignalInfo info1, FireSignalInfo info2)
        {
            if (info1.Time > info2.Time)
                return 1;
            else if (info1.Time < info2.Time)
                return -1;
            return 0;
        }
    }

    public class ReciverState
    {
        private int m_nID;

        private int state;


    }
}

