using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingSMS
{
    public class HotelManager : SendManager
    {
        public HotelManager(Building building, int nFloorIndex)
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
                    AddFloor(1, floors);
                }
                else if (m_nFloorIndex <= 9)
                {
                    if (m_nFloorIndex % 2 == 0)
                    {
                        if (m_nFloorIndex < 8)
                        {
                            // 발화층
                            AddFloor(m_nFloorIndex, floors);
                            AddFloor(m_nFloorIndex + 1, floors);
                        }
                        else
                        {
                            // 발화층
                            AddFloor(m_nFloorIndex, floors);
                        }

                        // 직상층
                        if (m_nFloorIndex < 6)
                        {
                            AddFloor(m_nFloorIndex + 2, floors);
                            AddFloor(m_nFloorIndex + 3, floors);
                        }
                        else
                        {
                            if (m_nFloorIndex == 8)
                                AddFloor(m_nFloorIndex + 1, floors);
                            else
                                AddFloor(m_nFloorIndex + 2, floors);
                        }
                    }
                    else
                    {
                        if (m_nFloorIndex < 9)
                        {
                            // 발화층
                            AddFloor(m_nFloorIndex - 1, floors);
                            AddFloor(m_nFloorIndex, floors);
                        }
                        else
                        {
                            // 발화층
                            AddFloor(m_nFloorIndex, floors);
                        }

                        // 직상층
                        if (m_nFloorIndex < 7)
                        {
                            AddFloor(m_nFloorIndex + 1, floors);
                            AddFloor(m_nFloorIndex + 2, floors);
                        }
                        else
                        {
                            AddFloor(m_nFloorIndex + 1, floors);
                        }
                    }

                    if (m_dicSend[0] == false)
                        AddFloor(0, floors);

                    if (m_dicSend[1] == false)
                        AddFloor(1, floors);
                }
                else// if (m_nFloorIndex <= 32)
                {
                    AddFloor(m_nFloorIndex, floors);

                    if (m_nFloorIndex < m_building.MaxFloor)
                        AddFloor(m_nFloorIndex + 1, floors);

                    AddFloor(0, floors);
                    AddFloor(1, floors);
                }
            }
            else if (m_nTimes == 1)
            {
                if (m_nFloorIndex == -2 || m_nFloorIndex == -1)
                {
                    // 지하 전층에서 6층까지
                    for (int i=m_building.MinFloor;i<=5;i++)
                    {
                        if (m_dicSend[i] == false)
                            AddFloor(i, floors);
                    }
                }
                else if (m_nFloorIndex <= -3)
                {
                    // 지하 전층
                    for (int i = m_building.MinFloor; i < 0; i++)
                    {
                        if (m_dicSend[i] == false)
                            AddFloor(i, floors);
                    }
                }
                else if (m_nFloorIndex <= 9)
                {
                    int nTargetFloor = 0;

                    if (m_nFloorIndex <= 1)
                        nTargetFloor = 8;
                    else if (m_nFloorIndex <= 3)
                        nTargetFloor = 9;
                    else if (m_nFloorIndex <= 5)
                        nTargetFloor = 10;
                    else if (m_nFloorIndex <= 7)
                        nTargetFloor = 11;
                    else if (m_nFloorIndex == 8)
                        nTargetFloor = 12;
                    else// if (m_nFloorIndex == 9)
                        nTargetFloor = 13;

                    for (int i=m_nFloorIndex + 1;i<=nTargetFloor;i++)
                    {
                        if (m_dicSend[i] == false)
                            AddFloor(i, floors);
                    }
                    
                    /*for (int i=m_nFloorIndex;i<m_nFloorIndex+4;i++)
                    {
                        if (m_dicSend[i] == false)
                            AddFloor(i, floors);
                    }*/
                }
                else// if (m_nFloorIndex <= 32)
                {
                    int nCount = 0;

                    for (int i = m_nFloorIndex; i <= m_nFloorIndex + 4 && i <= m_building.MaxFloor; i++)
                    {
                        if (m_dicSend[i] == false)
                        {
                            AddFloor(i, floors);
                            nCount++;
                        }
                    }

                    if (nCount == 0)
                        GetNext2(floors, out isLast);
                }
            }
            else// if (m_nTimes == 2)
            {
                if (m_nFloorIndex == -2 || m_nFloorIndex == -1)
                {
                    if (m_dicSend[6] == false)
                    {
                        AddFloor(6, floors);
                        AddFloor(7, floors);
                        AddFloor(8, floors);
                        AddFloor(9, floors);
                    }

                    int nBeginIndex = 8;

                    for (int i=nBeginIndex + 1;i<=m_building.MaxFloor;i++)
                    {
                        if (m_dicSend[i] == false)
                        {
                            nBeginIndex = i;
                            break;
                        }
                    }

                    if (nBeginIndex == 8)
                    {
                        isLast = true;
                        return false;
                    }

                    for (int i=nBeginIndex;i<=m_building.MaxFloor && i < nBeginIndex + 4;i++)
                    {
                        AddFloor(i, floors);
                    }

                    if (m_dicSend[m_building.MaxFloor])
                        isLast = true;
                }
                else if (m_nFloorIndex <= -3)
                {
                    int nCount1 = 0;

                    for (int i=2;i<=9;i++)
                    {
                        if (m_dicSend[i] == false)
                        {
                            AddFloor(i, floors);
                            nCount1++;

                            if (nCount1 >= 4)
                                break;
                        }
                    }

                    int nCount2 = 0;

                    for (int i=10;i<=m_building.MaxFloor;i++)
                    {
                        if (m_dicSend[i] == false)
                        {
                            AddFloor(i, floors);
                            nCount2++;

                            if (nCount2 >= 4)
                                break;
                        }
                    }

                    if (m_dicSend[m_building.MaxFloor])
                        isLast = true;
                }
                else if (m_nFloorIndex <= 9)
                {
                    int nCount1 = 0;

                    for (int i=-1;i>=m_building.MinFloor;i--)
                    {
                        if (m_dicSend[i] == false)
                        {
                            AddFloor(i, floors);
                            nCount1++;

                            if (nCount1 >= 3)
                                break;
                        }
                    }

                    int nCount2 = 0;

                    for (int i=9;i>=2;i--)
                    {
                        if (m_dicSend[i] == false)
                        {
                            AddFloor(i, floors);
                            nCount2++;

                            if (nCount2 >= 2)
                                break;
                        }
                    }

                    int nCount3 = 0;

                    for (int i=10;i<=m_building.MaxFloor;i++)
                    {
                        if (m_dicSend[i] == false)
                        {
                            AddFloor(i, floors);
                            nCount3++;

                            if (nCount3 >= 4)
                                break;
                        }
                    }

                    bool notFinish = false;

                    for (int i=2;i<=m_building.MaxFloor;i++)
                    {
                        if (m_dicSend[i] == false)
                        {
                            notFinish = true;
                            break;
                        }
                    }

                    isLast = !notFinish;
                }
                else// if (m_nFloorIndex <= 32)
                {
                    GetNext2(floors, out isLast);
                }
            }

            m_nTimes++;
            return true;
        }

        private void GetNext2(List<int> floors, out bool isLast)
        {
            int nCount1 = 0;

            for (int i = -1; i >= m_building.MinFloor; i--)
            {
                if (m_dicSend[i] == false)
                {
                    AddFloor(i, floors);
                    nCount1++;
                }

                if (nCount1 >= 3)
                    break;
            }

            int nCount2 = 0;

            for (int i = 2; i <= 9; i++)
            {
                if (m_dicSend[i] == false)
                {
                    AddFloor(i, floors);
                    nCount2++;
                }

                if (nCount2 >= 3)
                    break;
            }

            int nCount3 = 0;

            for (int i = m_nFloorIndex + 1; i <= m_building.MaxFloor; i++)
            {
                if (m_dicSend[i] == false)
                {
                    AddFloor(i, floors);
                    nCount3++;
                }

                if (nCount3 >= 4)
                    break;
            }

            if (nCount3 >= 4 || m_dicSend[m_building.MaxFloor] == true)
            {
                int nCount4 = 0;

                for (int i = m_nFloorIndex - 1; i > 9; i--)
                {
                    if (m_dicSend[i] == false)
                    {
                        AddFloor(i, floors);
                        nCount4++;
                    }

                    if (nCount4 >= 4)
                        break;
                }
            }

            bool notFinish = false;

            for (int i = 2; i <= m_building.MaxFloor; i++)
            {
                if (m_dicSend[i] == false)
                {
                    notFinish = true;
                    break;
                }
            }

            isLast = !notFinish;
        }
    }
}
