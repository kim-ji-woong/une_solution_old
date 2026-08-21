using CrisisAlertManager.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisisAlertManager.Data
{
    class GroupData
    {
    }
}

public class DataTeam
{
    private int m_nID = -1;
    private string m_strTeamName = "";
    private DataTeam m_teamParent = null;
    private ArrayList m_arrChildTeams = new ArrayList();

    public int ID
    {
        get { return m_nID; }
        set { m_nID = value; }
    }

    public string TeamName
    {
        get { return m_strTeamName; }
        set { m_strTeamName = value; }
    }

    public DataTeam ParentTeam
    {
        get { return m_teamParent; }
        set
        {
            if (m_teamParent != null)
                m_teamParent.RemoveChild(this);

            m_teamParent = value;

            if (m_teamParent != null)
                m_teamParent.AddChild(this);
        }
    }

    public ArrayList ChildTeams
    {
        get { return m_arrChildTeams; }
    }

    protected void RemoveChild(DataTeam team)
    {
        if (team != null)
            m_arrChildTeams.Remove(team);
    }

    protected void AddChild(DataTeam team)
    {
        if (!m_arrChildTeams.Contains(team))
            m_arrChildTeams.Add(team);
    }
}

public class DataCompanyMember
{
    private int m_nID = -1;
    private string m_strMemberName = "";
    private DataTeam m_team = null;
    private JobLevel m_jobLevel = null;
    private string m_strPhoneNumber = "";
    private string m_strFacilityTypes = "";

    public int ID
    {
        get { return m_nID; }
        set { m_nID = value; }
    }

    public string MemberName
    {
        get { return m_strMemberName; }
        set { m_strMemberName = value; }
    }

    public DataTeam Team
    {
        get { return m_team; }
        set { m_team = value; }
    }

    public JobLevel Level
    {
        get { return m_jobLevel; }
        set { m_jobLevel = value; }
    }

    public string PhoneNumber
    {
        get { return m_strPhoneNumber; }
        set { m_strPhoneNumber = value; }
    }

    public string FacilityTypes
    {
        get { return m_strFacilityTypes; }
        set { m_strFacilityTypes = value; }
    }
}

public class JobLevel
{
    private int m_nID = -1;
    private string m_strLevelName = "";
    private int m_nLevelNo = -1;

    public int ID
    {
        get { return m_nID; }
        set { m_nID = value; }
    }

    public string LevelName
    {
        get { return m_strLevelName; }
        set { m_strLevelName = value; }
    }

    public int LevelNo
    {
        get { return m_nLevelNo; }
        set { m_nLevelNo = value; }
    }

}


public class FacilityManager
{
    private int m_nID = -1;
    private DataCompanyMember m_companyMember = null;
    private FacilityType m_facilityType = FacilityType.NONE;
    private int m_nSensorID = -1;
    private string m_strDescription = "";
    private string m_strDepartment = "";
    private string m_strName = "";
    private string m_strPhoneNumber = "";

    public int ID
    {
        get { return m_nID; }
        set { m_nID = value; }
    }

    public DataCompanyMember CompanyMember
    {
        get { return m_companyMember; }
        set { m_companyMember = value; }
    }

    public FacilityType FacilityType
    {
        get { return m_facilityType; }
        set { m_facilityType = value; }
    }

    public int SensorID
    {
        get { return m_nSensorID; }
        set { m_nSensorID = value; }
    }

    public string Description
    {
        get { return m_strDescription; }
        set { m_strDescription = value; }
    }

    public string Department
    {
        get { return m_strDepartment; }
        set { m_strDepartment = value; }
    }

    public string Name
    {
        get { return m_strName; }
        set { m_strName = value; }
    }

    public string PhoneNumber
    {
        get { return m_strPhoneNumber; }
        set { m_strPhoneNumber = value; }
    }
}


public class FacilityMessage
{
    private int m_nID = -1;
    private FacilityType m_facilityType = FacilityType.NONE;
    private int m_nSensorID = -1;
    private MessageType m_messageType = MessageType.MESSAGE;
    private string m_strMessage = "";

    public int ID
    {
        get { return m_nID; }
        set { m_nID = value; }
    }

    public FacilityType FacilityType
    {
        get { return m_facilityType; }
        set { m_facilityType = value; }
    }

    public int SensorID
    {
        get { return m_nSensorID; }
        set { m_nSensorID = value; }
    }

    public MessageType MessageType
    {
        get { return m_messageType; }
        set { m_messageType = value; }
    }

    public string Message
    {
        get { return m_strMessage; }
        set { m_strMessage = value; }
    }
}
