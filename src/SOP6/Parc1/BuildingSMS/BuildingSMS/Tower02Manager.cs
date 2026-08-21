using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingSMS
{
    public class Tower02Manager : SendManager
    {
        public Tower02Manager(Building building, int nFloorIndex)
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

                    if (m_dicSend[0] == false)
                        AddFloor(0, floors);
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

                    if (m_nFloorIndex < m_building.MaxFloor)
                        AddFloor(m_nFloorIndex + 1, floors);

                    if (m_dicSend[0] == false)
                        AddFloor(0, floors);
                }
            }
            else if (m_nTimes == 1)
            {
                if (m_nFloorIndex == -2 || m_nFloorIndex == -1)
                {
                    if (m_nFloorIndex == -1)
                        AddFloor(-2, floors);

                    AddFloor(1, floors);
                    AddFloor(2, floors);
                    AddFloor(3, floors);
                    AddFloor(4, floors);
                }
                else if (m_nFloorIndex <= -3)
                {
                    GetNext(floors);
                }
                else// if (m_nFloorIndex >= 0)
                {
                    for (int i = m_nFloorIndex + 2; i <= m_nFloorIndex + 4; i++)
                    {
                        AddFloor(i, floors);
                    }
                }
            }
            else// if (m_nTimes == 2)
            {
                if (m_nFloorIndex == -2 || m_nFloorIndex == -1)
                {
                    int nCount1 = 0;

                    for (int i = m_nFloorIndex-1; i >= m_building.MinFloor; i--)
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

                    for (int i = 5; i <= 25; i++)
                    {
                        if (m_dicSend[i] == false)
                        {
                            AddFloor(i, floors);
                            nCount2++;

                            if (nCount2 >= 5)
                                break;
                        }
                    }

                    int nCount3 = 0;

                    for (int i = 26; i <= m_building.MaxFloor; i++)
                    {
                        if (m_dicSend[i] == false)
                        {
                            AddFloor(i, floors);
                            nCount3++;

                            if (nCount3 >= 5)
                                break;
                        }
                    }

                    bool finish = true;

                    for (int i = 5; i <= m_building.MaxFloor; i++)
                    {
                        if (m_dicSend[i] == false)
                        {
                            finish = false;
                            break;
                        }
                    }

                    isLast = finish;
                }
                else if (m_nFloorIndex <= -3)
                {
                    isLast = GetNext(floors);
                }
                else// if (m_nFloorIndex >= 0)
                {
                    int nCount1 = 0;

                    for (int i = -1; i >= m_building.MinFloor; i--)
                    {
                        if (m_dicSend[i] == false)
                        {
                            AddFloor(i, floors);
                            nCount1++;
                        }

                        if (nCount1 >= 4)
                            break;
                    }

                    int nCount2 = 0;

                    for (int i = 1; i <= 25; i++)
                    {
                        if (m_dicSend[i] == false)
                        {
                            AddFloor(i, floors);
                            nCount2++;
                        }

                        if (nCount2 >= 5)
                            break;
                    }

                    int nCount3 = 0;

                    for (int i = 26; i <= m_building.MaxFloor; i++)
                    {
                        if (m_dicSend[i] == false)
                        {
                            AddFloor(i, floors);
                            nCount3++;
                        }

                        if (nCount3 >= 5)
                            break;
                    }

                    bool finish = true;

                    for (int i = 1; i <= m_building.MaxFloor; i++)
                    {
                        if (m_dicSend[i] == false)
                        {
                            finish = false;
                            break;
                        }
                    }

                    isLast = finish;
                }
            }

            m_nTimes++;
            return true;
        }

        // Return 값 : isLast
        private bool GetNext(List<int> floors)
        {
            int nCount1 = 0;

            for (int i=m_nFloorIndex+2;i<=0;i++)
            {
                if (m_dicSend[i] == false)
                {
                    AddFloor(i, floors);
                    nCount1++;

                    if (nCount1 >= 3)
                        break;
                }
            }

            if (nCount1 == 0)
            {
                int nCount2 = 0;

                for (int i=m_nFloorIndex-1;i>=m_building.MinFloor;i--)
                {
                    if (m_dicSend[i] == false)
                    {
                        AddFloor(i, floors);
                        nCount2++;

                        if (nCount2 >= 3)
                            break;
                    }
                }

                if (nCount2 > 0)
                    return false;
            }

            int nCount5 = 0;

            for (int i=1;i<=4;i++)
            {
                if (m_dicSend[i] == false)
                {
                    AddFloor(i, floors);
                    nCount5++;
                }
            }

            if (nCount1 == 0 && nCount5 == 0)
            {
                int nCount3 = 0;

                for (int i = 5; i <= 25; i++)
                {
                    if (m_dicSend[i] == false)
                    {
                        AddFloor(i, floors);
                        nCount3++;

                        if (nCount3 >= 5)
                            break;
                    }
                }

                int nCount4 = 0;

                for (int i = 26; i <= m_building.MaxFloor; i++)
                {
                    if (m_dicSend[i] == false)
                    {
                        AddFloor(i, floors);
                        nCount4++;

                        if (nCount4 >= 5)
                            break;
                    }
                }

                bool finish = true;

                for (int i = 5; i <= m_building.MaxFloor; i++)
                {
                    if (m_dicSend[i] == false)
                    {
                        finish = false;
                        break;
                    }
                }

                return finish;
            }

            return false;
        }
    }
}
