using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingSMS
{
    public class RetailManager : SendManager
    {
        public RetailManager(Building building, int nFloorIndex)
            : base(building, nFloorIndex)
        {
        }

        public override bool GetNext(List<int> floors, int nReceiverIndex, out bool isLast)
        {
            isLast = false;

            if (m_nFloorIndex >= 6 || m_nFloorIndex < 0)
                return false;

            if (m_nTimes == 0)
            {
                SetFirst(floors, out isLast);
            }
            else if (m_nTimes == 1)
            {
                SetSecond(floors, out isLast);
            }
            else
            {
                SetThird(floors);
                isLast = true;
            }

            m_nTimes++;
            return true;
        }

        private void SetThird(List<int> floors)
        {
            AddFloor(0, floors);
            AddFloor(1, floors);
            AddFloor(2, floors);

            AddFloor(5, floors);
            AddFloor(4, floors);
            AddFloor(3, floors);
        }

        private void SetSecond(List<int> floors, out bool isLast)
        {
            if (m_nFloorIndex >= 2)
                AddFloor(m_nFloorIndex - 2, floors);

            if (m_nFloorIndex >= 3)
                AddFloor(m_nFloorIndex - 3, floors);

            if (m_nFloorIndex >= 4)
                AddFloor(m_nFloorIndex - 4, floors);

            if (m_nFloorIndex <= 3)
                AddFloor(m_nFloorIndex + 2, floors);

            if (m_nFloorIndex <= 2)
                AddFloor(m_nFloorIndex + 3, floors);

            if (m_nFloorIndex <= 1)
                AddFloor(m_nFloorIndex + 4, floors);

            AddFloor(5, floors);
            AddFloor(4, floors);
            AddFloor(3, floors);

            isLast = CheckAll(floors);
        }

        private void SetFirst(List<int> floors, out bool isLast)
        {
            if (m_nFloorIndex >= 0)
            {
                if (m_nFloorIndex < 5)
                    AddFloor(m_nFloorIndex + 1, floors);

                if (m_nFloorIndex <= 5)
                    AddFloor(m_nFloorIndex, floors);

                if (m_nFloorIndex > 0)
                    AddFloor(m_nFloorIndex - 1, floors);

                AddFloor(0, floors);
                isLast = CheckAll(floors);
            }
            else
            {
                AddFloor(0, floors);
                AddFloor(1, floors);
                AddFloor(2, floors);

                AddFloor(5, floors);
                AddFloor(4, floors);
                AddFloor(3, floors);
                isLast = true;
            }
        }

        private bool CheckAll(List<int> floors)
        {
            for (int i = 0; i <= 5; i++)
            {
                if (floors.Contains(i) == false)
                    return false;
            }

            return true;
        }

        public override string Parsing(string strMessage, int nReceiverIndex)
        {
            return ParseStair(strMessage, nReceiverIndex);
        }
    }
}
