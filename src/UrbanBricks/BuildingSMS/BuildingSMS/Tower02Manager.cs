using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingSMS
{
    // T-Tower
    public class Tower02Manager : SendManager
    {
        //private int m_nSecondTimes = 0;
        //private int m_nThirdTimes = 0;
        private const int MaxFloorIndex = 25;

        public Tower02Manager(Building building, int nFloorIndex)
            : base(building, nFloorIndex)
        {
        }

        public override bool GetNext(List<int> floors, int nReceiverIndex, out bool isLast)
        {
            isLast = false;

            if (m_nFloorIndex >= 27 || m_nFloorIndex < -8)
                return false;

            if (m_nTimes == 0)
            {
                SetFirst(floors);
                m_dicStairs[nReceiverIndex] = "1번 계단실";
            }
            else
            {
                if (SetSecond(floors))
                    isLast = true;

                if (m_nTimes % 2 == 1)
                    m_dicStairs[nReceiverIndex] = "1번 계단실";
                else
                    m_dicStairs[nReceiverIndex] = "2번 계단실";
            }

            m_nTimes++;
            return true;
        }

        /*private bool SetThird(List<int> floors)
        {
            if (m_nFloorIndex >= 0)
            {
                if (m_nThirdTimes == 0)
                {
                    AddFloor(-1, floors);
                    AddFloor(-2, floors);
                    m_nThirdTimes++;
                }
                else
                {
                    AddFloor(-3, floors);
                    AddFloor(-4, floors);
                    AddFloor(-5, floors);
                    AddFloor(-6, floors);
                    AddFloor(-7, floors);
                    AddFloor(-8, floors);
                    return true;
                }
            }
            else
            {
                int nFloorIndex1 = 3 * m_nThirdTimes;

                AddFloor(nFloorIndex1, floors);
                AddFloor(nFloorIndex1 + 1, floors);
                AddFloor(nFloorIndex1 + 2, floors);

                int nFloorIndex2 = 25 - 3 * m_nThirdTimes;

                AddFloor(nFloorIndex2, floors);
                AddFloor(nFloorIndex2 - 1, floors);
                AddFloor(nFloorIndex2 - 2, floors);

                if (CheckOverground(floors))
                    return true;
                else
                    m_nThirdTimes++;
            }

            return false;
        }*/

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

            return CheckOverground(floors);
            /*if (m_nFloorIndex >= 0)
            {
                int nFloorIndex1 = m_nFloorIndex - 3 * m_nSecondTimes;

                if (nFloorIndex1 >= 2)
                    AddFloor(nFloorIndex1 - 2, floors);

                if (nFloorIndex1 >= 3)
                    AddFloor(nFloorIndex1 - 3, floors);

                if (nFloorIndex1 >= 4)
                    AddFloor(nFloorIndex1 - 4, floors);

                int nFloorIndex2 = m_nFloorIndex + 3 * m_nSecondTimes;

                if (nFloorIndex2 <= 23)
                    AddFloor(nFloorIndex2 + 2, floors);

                if (nFloorIndex2 <= 22)
                    AddFloor(nFloorIndex2 + 3, floors);

                if (nFloorIndex2 <= 21)
                    AddFloor(nFloorIndex2 + 4, floors);

                int nFloorIndex3 = 25 - 3 * m_nSecondTimes;

                AddFloor(nFloorIndex3, floors);
                AddFloor(nFloorIndex3 - 1, floors);
                AddFloor(nFloorIndex3 - 2, floors);

                if (CheckOverground(floors))
                    return true;
                else
                    m_nSecondTimes++;
            }
            else
            {
                AddFloor(-3, floors);
                AddFloor(-4, floors);
                AddFloor(-5, floors);
                AddFloor(-6, floors);
                AddFloor(-7, floors);
                AddFloor(-8, floors);
                return true;
            }

            return false;*/
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

            AddFloor(0, floors);
            /*if (m_nFloorIndex >= 0)
            {
                if (m_nFloorIndex <= 24)
                    AddFloor(m_nFloorIndex + 1, floors);

                AddFloor(m_nFloorIndex, floors);

                if (m_nFloorIndex > 0)
                    AddFloor(m_nFloorIndex - 1, floors);

                AddFloor(0, floors);
            }
            else
            {
                AddFloor(-1, floors);
                AddFloor(-2, floors);
            }*/
        }

        private bool CheckOverground(List<int> floors)
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
