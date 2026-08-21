using UnityEngine;
using System.Collections;

public static class DataManager
{
    private static int m_nID = 1;
    public static int CurrentID
    {
        get
        {
            return m_nID;
        }
    }

    public static int CreateID()
    {
        m_nID++;

        return CurrentID;
    }

}