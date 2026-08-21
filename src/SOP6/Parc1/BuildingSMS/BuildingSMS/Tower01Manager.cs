using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingSMS
{
    public class Tower01Manager : SendManager
    {
        public Tower01Manager(Building building, int nFloorIndex)
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

                    if (m_building.MaxFloor >= m_nFloorIndex + 1)
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
                    AddFloor(5, floors);
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
                    AddFloor(2, floors);
                    AddFloor(3, floors);
                    AddFloor(4, floors);
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
                    if (m_dicSend[6] == false)
                    {
                        AddFloor(6, floors);
                        AddFloor(7, floors);
                        AddFloor(8, floors);
                        AddFloor(9, floors);
                        AddFloor(10, floors);
                    }
                    else
                    {
                        int nCount1 = 0;

                        for (int i = -3; i >= m_building.MinFloor; i--)
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

                        for (int i=11;i<=40;i++)
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

                        for (int i = 41; i <= m_building.MaxFloor; i++)
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

                        for (int i=11;i<=m_building.MaxFloor;i++)
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
                else if (m_nFloorIndex <= -3)
                {
                    int nCount1 = 0;

                    for (int i = 1; i <= 10; i++)
                    {
                        if (m_dicSend[i] == false)
                        {
                            AddFloor(i, floors);
                            nCount1++;

                            if (nCount1 >= 5)
                                break;
                        }
                    }

                    int nCount2 = 0;

                    for (int i = 11; i <= 40; i++)
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

                    for (int i = 41; i <= m_building.MaxFloor; i++)
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

                    for (int i = 11; i <= m_building.MaxFloor; i++)
                    {
                        if (m_dicSend[i] == false)
                        {
                            finish = false;
                            break;
                        }
                    }

                    isLast = finish;
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

                    for (int i = 1; i <= 10; i++)
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

                    for (int i = 11; i <= 40; i++)
                    {
                        if (m_dicSend[i] == false)
                        {
                            AddFloor(i, floors);
                            nCount3++;
                        }

                        if (nCount3 >= 5)
                            break;
                    }

                    int nCount4 = 0;

                    for (int i = 41; i <= m_building.MaxFloor; i++)
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

                    for (int i = 11; i <= m_building.MaxFloor; i++)
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
    }
}
