using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingSMS
{
    public class HotelManager : SendManager
    {
        //private int m_nSecondTimes = 0;
        //private int m_nThirdTimes = 0;
        private const int MaxFloorIndex = 26;

        public HotelManager(Building building, int nFloorIndex)
            : base(building, nFloorIndex)
        {
        }

        public override bool GetNext(List<int> floors, int nReceiverIndex, out bool isLast)
        {
            isLast = false;

            if (m_nFloorIndex > MaxFloorIndex || m_nFloorIndex < -8)
                return false;

            if (m_nTimes == 0)
            {
                SetFirst(floors);
                m_dicStairs[nReceiverIndex] = "10번 계단실";
            }
            else
            {
                if (SetSecond(floors))
                    isLast = true;

                if (m_nTimes % 2 == 1)
                    m_dicStairs[nReceiverIndex] = "10번 계단실";
                else
                    m_dicStairs[nReceiverIndex] = "11번 계단실";
            }

            m_nTimes++;
            return true;
        }

        private void SetFirst(List<int> floors)
        {
            int upFloor = m_nFloorIndex + 1;
            int current = m_nFloorIndex;
            int downFloor = m_nFloorIndex - 1;

            if (upFloor >= 0 && upFloor <= MaxFloorIndex)
                AddFloor(upFloor, floors);

            if (current >= 0)
                AddFloor(current, floors);

            if (downFloor >= 0)
                AddFloor(downFloor, floors);

            AddFloor(5, floors);
            AddFloor(0, floors);
        }

        private bool SetSecond(List<int> floors)
        {
            int number = (m_nTimes - 1) * 3 + 2 + m_nFloorIndex;

            int minusN2 = number * (-1);
            int minusN3 = minusN2 - 1;
            int minusN4 = minusN2 - 2;
            int plusN2 = number;
            int plusN3 = number + 1;
            int plusN4 = number + 2;

            int topFloor = MaxFloorIndex - (m_nTimes - 1) * 3;
            int top_1 = topFloor - 1;
            int top_2 = topFloor - 2;

            if (minusN2 >= 0)
                AddFloor(minusN2, floors);

            if (minusN3 >= 0)
                AddFloor(minusN3, floors);

            if (minusN4 >= 0)
                AddFloor(minusN4, floors);

            if (plusN2 >= 0 && plusN2 <= MaxFloorIndex)
                AddFloor(plusN2, floors);

            if (plusN3 >= 0 && plusN3 <= MaxFloorIndex)
                AddFloor(plusN3, floors);

            if (plusN4 >= 0 && plusN4 <= MaxFloorIndex)
                AddFloor(plusN4, floors);

            if (topFloor >= 0)
                AddFloor(topFloor, floors);

            if (top_1 >= 0)
                AddFloor(top_1, floors);

            if (top_2 >= 0)
                AddFloor(top_2, floors);

            return CheckLast(floors);
            /*if (m_nFloorIndex < 0)
            {
                AddFloor(-3, floors);
                AddFloor(-4, floors);
                AddFloor(-5, floors);
                AddFloor(-6, floors);
                AddFloor(-7, floors);
                AddFloor(-8, floors);
                return true;
            }

            int init1 = m_nFloorIndex - 2 - 3 * m_nSecondTimes;

            for (int i=init1;i>=0 && i>=init1-2;i--)
            {
                AddFloor(i, floors);
            }

            int init2 = m_nFloorIndex + 2 + 3 * m_nSecondTimes;

            for (int i=init2;i<=m_building.MaxFloor && i<=init2+2;i++)
            {
                AddFloor(i, floors);
            }

            int init3 = m_building.MaxFloor - 3 * m_nSecondTimes;

            for (int i=init3;i>=0 && i>=init3-2;i--)
            {
                AddFloor(i, floors);
            }

            if (CheckSecond(floors) == false)
            {
                m_nSecondTimes++;
                return false;
            }

            return true;*/
        }

        /*private bool SetThird(List<int> floors)
        {
            if (m_nFloorIndex < 0)
            {
                int init1 = 3 * m_nThirdTimes;

                for (int i=init1;i<=m_building.MaxFloor && i<=init1+2;i++)
                {
                    AddFloor(i, floors);
                }

                int init2 = m_building.MaxFloor - 3 * m_nThirdTimes;

                for (int i=init2;i>=0 && i>=init2-2;i--)
                {
                    AddFloor(i, floors);
                }
            }
            else
            {
                if (m_nThirdTimes == 0)
                {
                    AddFloor(-1, floors);
                    AddFloor(-2, floors);
                    m_nThirdTimes++;
                    return false;
                }
                else
                {
                    for (int i=-3;i>=-8;i--)
                    {
                        AddFloor(i, floors);
                    }

                    return true;
                }
            }

            if (CheckThird(floors) == false)
            {
                m_nThirdTimes++;
                return false;
            }

            return true;
        }*/

        /*private bool CheckSecond(List<int> floors)
        {
            if (m_nFloorIndex < 0)
                return true;

            for (int i=0;i<=m_building.MaxFloor;i++)
            {
                if (floors.Contains(i) == false)
                    return false;
            }

            return true;
        }

        private bool CheckThird(List<int> floors)
        {
            for (int i = m_building.MinFloor; i <= m_building.MaxFloor; i++)
            {
                if (floors.Contains(i) == false)
                    return false;
            }

            return true;
        }*/

        private bool CheckLast(List<int> floors)
        {
            for (int i = 0; i <= MaxFloorIndex; i++)
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
