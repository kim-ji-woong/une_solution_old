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

        public override bool GetNext(List<int> floors, out bool isLast)
        {
            isLast = false;

            if (m_nTimes == 0)
            {
                if (m_nFloorIndex == -2 || m_nFloorIndex == -1)
                {
                    AddFloor(m_nFloorIndex, floors);
                    AddFloor(m_nFloorIndex + 1, floors);

                    if (m_nFloorIndex == -1)
                    {
                        AddFloor(1, floors);
                    }
                    else
                    {
                        AddFloor(0, floors);
                        AddFloor(1, floors);
                    }
                }
                else if (m_nFloorIndex <= -3)
                {
                    AddFloor(m_nFloorIndex, floors);
                    AddFloor(m_nFloorIndex + 1, floors);
                    AddFloor(0, floors);
                }
                else// if (m_nFloorIndex >= 0)
                {
                    AddFloor(m_nFloorIndex, floors);
                    AddFloor(m_nFloorIndex + 1, floors);

                    if (m_nFloorIndex == 5)
                        AddFloor(7, floors);

                    if (m_dicSend[0] == false)
                        AddFloor(0, floors);
                }
            }
            else if (m_nTimes == 1)
            {
                if (m_nFloorIndex == -2 || m_nFloorIndex == -1)
                {
                    AddFloor(2, floors);
                    AddFloor(3, floors);
                    AddFloor(4, floors);
                }
                else if (m_nFloorIndex <= -3)
                {
                    // 지하 전층
                    for (int i = m_building.MinFloor; i < 0; i++)
                    {
                        if (m_dicSend[i] == false)
                            AddFloor(i, floors);
                    }

                    AddFloor(1, floors);
                }
                else// if (m_nFloorIndex >= 0)
                {
                    if (m_nFloorIndex <= 4)
                    {
                        for (int i = m_nFloorIndex + 2; i <= m_building.MaxFloor && i <= m_nFloorIndex + 4; i++)
                        {
                            AddFloor(i, floors);
                        }
                    }
                    else
                    {
                        GetNext(floors);
                    }
                }
            }
            else// if (m_nTimes == 2)
            {
                if (m_nFloorIndex == -2 || m_nFloorIndex == -1)
                {
                    if (m_dicSend[5] == false)
                    {
                        AddFloor(5, floors);
                        AddFloor(6, floors);
                        AddFloor(7, floors);
                    }
                    else
                    {
                        int nBeginIndex = m_nFloorIndex - 1;

                        for (int i = nBeginIndex; i >= m_building.MinFloor; i--)
                        {
                            if (m_dicSend[i] == false)
                            {
                                nBeginIndex = i;
                                break;
                            }
                        }

                        for (int i = nBeginIndex; i >= m_building.MinFloor && i >= nBeginIndex - 2; i--)
                        {
                            AddFloor(i, floors);
                        }

                        if (m_dicSend[m_building.MinFloor])
                            isLast = true;
                    }
                }
                else if (m_nFloorIndex <= -3)
                {
                    int nCount = 0;

                    for (int i = 2; i <= m_building.MaxFloor; i++)
                    {
                        if (m_dicSend[i] == false)
                        {
                            AddFloor(i, floors);
                            nCount++;

                            if (nCount >= 3)
                                break;
                        }
                    }

                    if (m_dicSend[m_building.MaxFloor])
                        isLast = true;
                }
                else// if (m_nFloorIndex >= 0)
                {
                    isLast = GetNext(floors);
                }
            }

            m_nTimes++;
            return true;
        }

        // Return 값 : isLast
        private bool GetNext(List<int> floors)
        {
            bool send = false;

            for (int i = m_nFloorIndex + 2; i <= m_building.MaxFloor; i++)
            {
                if (m_dicSend[i] == false)
                {
                    AddFloor(i, floors);
                    send = true;
                }
            }

            if (send)
                return false;

            int nCount1 = 0;

            for (int i=1;i<=m_nFloorIndex-1;i++)
            {
                if (m_dicSend[i] == false)
                {
                    AddFloor(i, floors);
                    nCount1++;

                    if (nCount1 >= 3)
                        return false;
                }
            }

            if (nCount1 == 0)
            {
                int nCount2 = 0;

                for (int i=-1;i>=m_building.MinFloor;i--)
                {
                    if (m_dicSend[i] == false)
                    {
                        AddFloor(i, floors);
                        nCount2++;

                        if (nCount2 >= 4)
                            break;
                    }
                }

                if (m_dicSend[m_building.MinFloor])
                    return true;
            }

            return false;
        }
    }
}
