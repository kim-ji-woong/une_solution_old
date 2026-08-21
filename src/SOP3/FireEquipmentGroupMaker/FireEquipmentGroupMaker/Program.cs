using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBUtility;
using System.Collections;

namespace FireEquipmentGroupMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            FEGroupMaker maker = new FEGroupMaker();
        }
    }

    class FEGroupMaker
    {
        private WebDBManager m_dbMgr = new WebDBManager();
        // Key : Zone ID(상위 4바이트) & Equip Type(하위 4바이트)
        // Value : Zone 내에 포함된 Equip Type에 해당하는 설비 List
        private Dictionary<long, ArrayList> m_dicFireEquipments = new Dictionary<long, ArrayList>();

        public FEGroupMaker()
        {
            if (ReadFireEquipment())
            {
                System.Console.WriteLine("계산중입니다.");
                MakeFireEquipmentGroup();
                System.Console.WriteLine("DB 작업이 종료되었습니다.");
            }
            else
                System.Console.WriteLine("오류로 인하여 DB 작업이 취소되었습니다.");
        }

        private bool ReadFireEquipment()
        {
            string strSQL = "Select ID, EquipType, X, Y, Z, ZoneID from FireEquipment";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nEquipType = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                float x = WebDBManager.GetFloatField(arrResult[i + 2].ToString(), 0.0f);
                float y = WebDBManager.GetFloatField(arrResult[i + 3].ToString(), 0.0f);
                float z = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), 0.0f);
                int nZoneID = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);

                // 야외에 있는 소화설비는 고려하지 않는다.
                if (nZoneID < 0)
                    continue;

                long key = (((long)nZoneID) << 32) | (long)nEquipType;
                ArrayList arrEquipments = null;

                if (m_dicFireEquipments.ContainsKey(key))
                    arrEquipments = m_dicFireEquipments[key];
                else
                {
                    arrEquipments = new ArrayList();
                    m_dicFireEquipments[key] = arrEquipments;
                }

                FireEquipment equip = new FireEquipment();
                equip.ID = nID;
                equip.Type = nEquipType;
                equip.ZoneID = nZoneID;
                equip.X = x;
                equip.Y = y;
                equip.Z = z;

                arrEquipments.Add(equip);
            }

            return true;
        }

        private void MakeFireEquipmentGroup()
        {
            string strSQL = "Delete from FireEquipmentGroup";
            if (m_dbMgr.GetResultData(strSQL, 0) == null)
                return;

            CalcEquipments();

            int nID = 1;

            foreach (KeyValuePair<long, ArrayList> pair in m_dicFireEquipments)
            {
                ArrayList arrEquips = pair.Value;

                int nZoneID = (int)(pair.Key >> 32);
                int nEquipType = (int)(pair.Key & 0xffffffff);

                foreach (FireEquipment equip in arrEquips)
                {
                    strSQL = string.Format("Insert into FireEquipmentGroup (ID, linkedEquipID, X, Y, Z) values ({0}, {1}, NULL, NULL, NULL)",
                        nID++, equip.ID);

                    if (m_dbMgr.GetResultData(strSQL, 0) == null)
                        return;
                }
            }
        }

        private void CalcEquipments()
        {
            // 한계값 : 10미터
            float fLimit = 10.0f;

            foreach (KeyValuePair<long, ArrayList> pair in m_dicFireEquipments)
            {
                ArrayList arrEquips = pair.Value;
                ArrayList arrGroups = new ArrayList();

                int nEquipCount = arrEquips.Count;

                int nZoneID = (int)(pair.Key >> 32);
                int nEquipType = (int)(pair.Key & 0xffffffff);

                for (int i = 0; i < nEquipCount; i++)
                {
                    FireEquipment equip = (FireEquipment)arrEquips[i];
                    arrGroups.Add(equip);

                    ArrayList arrRemove = new ArrayList();
                    UnE.Geometry.Vertex2F v1 = new UnE.Geometry.Vertex2F(equip.X, equip.Y);

                    for (int j = i + 1; j < nEquipCount; j++)
                    {
                        FireEquipment equip2 = (FireEquipment)arrEquips[j];
                        UnE.Geometry.Vertex2F v2 = new UnE.Geometry.Vertex2F(equip2.X, equip2.Y);

                        // 두 설비의 위치가 fLimit 이하이면 equip2를 삭제 목록에 추가
                        if (v1.GetDistance(v2) <= fLimit)
                            arrRemove.Add(j);
                    }

                    // 삭제 목록의 설비들 삭제
                    int nRemoveCount = arrRemove.Count;

                    for (int j = nRemoveCount - 1; j >= 0; j--)
                    {
                        int nIndex = (int)arrRemove[j];
                        arrEquips.RemoveAt(nIndex);
                    }

                    nEquipCount -= nRemoveCount;
                    /////////////////////////////
                }

                pair.Value.Clear();

                foreach (FireEquipment equip in arrGroups)
                {
                    pair.Value.Add(equip);
                }
            }
        }
    }

    class FireEquipment
    {
        private int m_nID = -1;
        private int m_nZoneID = -1;
        private int m_nType = -1;

        private float x = 0.0f;
        private float y = 0.0f;
        private float z = 0.0f;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        public int Type
        {
            get { return m_nType; }
            set { m_nType = value; }
        }

        public float X
        {
            get { return x; }
            set { x = value; }
        }

        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        public float Z
        {
            get { return z; }
            set { z = value; }
        }
    }

    class FireEquipmentGroup
    {
        private int m_nID = -1;
        private int m_nEquipID = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int EquipID
        {
            get { return m_nEquipID; }
            set { m_nEquipID = value; }
        }
    }
}
