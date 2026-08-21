using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnE
{
    namespace SOP
    {
        namespace Workstate
        {
            public class SOPScenario
            {
                private int m_nSensorZoneHistoryID = -1;
                public int SensorZoneHistoryID
                {
                    get { return m_nSensorZoneHistoryID; }
                    set { m_nSensorZoneHistoryID = value; }
                }

                private bool m_bRealMode = false;
                public bool RealMode
                {
                    get { return m_bRealMode; }
                    set { m_bRealMode = value; }
                }

                private bool m_bRegular = false;
                public bool RegularMode
                {
                    get { return m_bRegular; }
                    set { m_bRegular = value; }
                }

                private bool m_bNormal = false;
                public bool NormalMode
                {
                    get { return m_bNormal; }
                    set { m_bNormal = value; }
                }

                private int m_nActionStepID = -1;
                public int ActionStepID
                {
                    get { return m_nActionStepID; }
                    set { m_nActionStepID = value; }
                }

                private int m_nActionStepHistoryID = -1;
                public int ActionStepHistoryID
                {
                    get { return m_nActionStepHistoryID; }
                    set { m_nActionStepHistoryID = value; }
                }

                private string m_szActionStepFullPath = "";
                public string ActionStepFullPath
                {
                    get { return m_szActionStepFullPath; }
                    set 
                    { 
                        m_szActionStepFullPath = value;
                        ParsePath();
                    }
                }


                private string m_szDelimeter = "/";
                public string Delimeter
                {
                    get { return m_szDelimeter; }
                    set { m_szDelimeter = value; }
                }

                private string m_szCategoryName = "";
                public string CategoryName
                {
                    get { return m_szCategoryName; }
                    set { m_szCategoryName = value; }
                }
                private string m_szSubCategoryName = "";
                public string SubCategoryName
                {
                    get { return m_szSubCategoryName; }
                    set { m_szSubCategoryName = value; }
                }

                private string m_szDisasterName = "";
                public string DisasterName
                {
                    get { return m_szDisasterName; }
                    set { m_szDisasterName = value; }
                }
                private string m_szActionStepName = "";
                public string ActionStepName
                {
                    get { return m_szActionStepName; }
                    set { m_szActionStepName = value; }
                }

                private bool m_displayActionStepName = false;
                public bool DisplayActionStepName
                {
                    get { return m_displayActionStepName; }
                    set { m_displayActionStepName = value; }
                }

                private void ParsePath()
                {
                    if (m_szActionStepFullPath == null || m_szActionStepFullPath == "")
                        return;

                    char cDeli = (char)0x06;
                    int idx = m_szActionStepFullPath.IndexOf((char)0x06);
                    if (idx == -1)
                    {
                        cDeli = '/';
                    }

                    string[] szTemps = m_szActionStepFullPath.Split(cDeli);
                    m_szCategoryName = szTemps[0];
                    m_szSubCategoryName = szTemps[1];
                    m_szDisasterName = szTemps[2];
                    m_szActionStepName = szTemps[3];
                    
                    int i = 0;
                    i++;
                }


                public override string ToString()
                {
                    if (m_bNormal)
                    {
                        if (m_displayActionStepName)
                            return m_szDisasterName + "(" + ActionStepName + ")";
                        else
                            return m_szDisasterName;
                    }

                    if (m_displayActionStepName)
                        return "[야간]" + m_szDisasterName + "(" + ActionStepName + ")";

                    return "[야간]" + m_szDisasterName;
                }
            }
        }
    }
}