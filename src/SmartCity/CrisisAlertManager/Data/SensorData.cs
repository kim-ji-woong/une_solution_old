using CrisisAlertManager.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisisAlertManager.Data
{
    class SensorData
    {
    }

    
}

public class FireSensor
{
    private int m_nID = -1;
    private string m_strSensorID = "";
    private string m_strState = CommonString.RiskLevel_Normal;
    private string m_strAddr = "";
    private DateTime m_dtOccurTime;
    private DateTime m_dtCloseTime;
    private bool m_bAfterFire = false;
    private DateTime m_dtAlarmPeriodStart;
    private DateTime m_dtAlarmPeriodEnd;
    private DateTime m_dtWeakStart;
    private DateTime m_dtWeakEnd;
    private int m_nInitReact = 0;
    private int m_nDemander = 0;
    private int m_nDeathToll = 0;
    private string m_strMessage = "";

    private int m_nUserModifity = 0;
    public int UserModifity
    {
        get { return m_nUserModifity; }
        set { m_nUserModifity = value; }
    }

    public int ID
    {
        get { return m_nID; }
        set { m_nID = value; }
    }

    public string SensorID
    {
        get { return m_strSensorID; }
        set { m_strSensorID = value; }
    }

    public string State
    {
        get { return m_strState; }
        set { m_strState = value; }
    }

    public string Addr
    {
        get { return m_strAddr; }
        set { m_strAddr = value; }
    }

    public DateTime OccurTime
    {
        get { return m_dtOccurTime; }
        set { m_dtOccurTime = value; }
    }

    public DateTime CloseTime
    {
        get { return m_dtCloseTime; }
        set { m_dtCloseTime = value; }
    }

    public bool AfterFire
    {
        get { return m_bAfterFire; }
        set { m_bAfterFire = value; }
    }

    public DateTime AlarmPeriodStart
    {
        get { return m_dtAlarmPeriodStart; }
        set { m_dtAlarmPeriodStart = value; }
    }

    public DateTime AlarmPeriodEnd
    {
        get { return m_dtAlarmPeriodEnd; }
        set { m_dtAlarmPeriodEnd = value; }
    }

    public DateTime WeakStart
    {
        get { return m_dtWeakStart; }
        set { m_dtWeakStart = value; }
    }

    public DateTime WeakEnd
    {
        get { return m_dtWeakEnd; }
        set { m_dtWeakEnd = value; }
    }

    public int InitReact
    {
        get { return m_nInitReact; }
        set { m_nInitReact = value; }
    }

    public int Demander
    {
        get { return m_nDemander; }
        set { m_nDemander = value; }
    }

    public int DeathToll
    {
        get { return m_nDeathToll; }
        set { m_nDeathToll = value; }
    }

    public string Message
    {
        get { return m_strMessage; }
        set { m_strMessage = value; }
    }
}

public class HeatSensor
{
    private int m_nID = -1;
    private string m_strSensorID = "";
    private string m_strState = CommonString.RiskLevel_Normal;
    private string m_strAddr = "";
    private DateTime m_dtOccurTime;
    private float m_fTemperature = 0;
    private float m_fHumidity = 0;
    private float m_fDirection = 0;
    private float m_fSpeed = 0;
    private DateTime m_dtMeasPeriodStart;
    private DateTime m_dtMeasPeriodEnd;
    private DateTime m_dtPreliminaryDate;
    private DateTime m_dtAdvisoryDate;
    private DateTime m_dtAlertDate;
    private int m_nDeathToll = 0;
    private string m_strMessage = "";

    private int m_nUserModifity = 0;
    public int UserModifity
    {
        get { return m_nUserModifity; }
        set { m_nUserModifity = value; }
    }

    public int ID
    {
        get { return m_nID; }
        set { m_nID = value; }
    }

    public string SensorID
    {
        get { return m_strSensorID; }
        set { m_strSensorID = value; }
    }

    public string State
    {
        get { return m_strState; }
        set { m_strState = value; }
    }

    public string Addr
    {
        get { return m_strAddr; }
        set { m_strAddr = value; }
    }

    public DateTime OccurTime
    {
        get { return m_dtOccurTime; }
        set { m_dtOccurTime = value; }
    }

    public float Temperature
    {
        get { return m_fTemperature; }
        set { m_fTemperature = value; }
    }

    public float Humidity
    {
        get { return m_fHumidity; }
        set { m_fHumidity = value; }
    }

    public float Direction
    {
        get { return m_fDirection; }
        set { m_fDirection = value; }
    }

    public float Speed
    {
        get { return m_fSpeed; }
        set { m_fSpeed = value; }
    }

    public DateTime MeasPeriodStart
    {
        get { return m_dtMeasPeriodStart; }
        set { m_dtMeasPeriodStart = value; }
    }

    public DateTime MeasPeriodEnd
    {
        get { return m_dtMeasPeriodEnd; }
        set { m_dtMeasPeriodEnd = value; }
    }

    public DateTime PreliminaryDate
    {
        get { return m_dtPreliminaryDate; }
        set { m_dtPreliminaryDate = value; }
    }

    public DateTime AdvisoryDate
    {
        get { return m_dtAdvisoryDate; }
        set { m_dtAdvisoryDate = value; }
    }

    public DateTime AlertDate
    {
        get { return m_dtAlertDate; }
        set { m_dtAlertDate = value; }
    }

    public int DeathToll
    {
        get { return m_nDeathToll; }
        set { m_nDeathToll = value; }
    }

    public string Message
    {
        get { return m_strMessage; }
        set { m_strMessage = value; }
    }
}

public class FloodSensor
{
    private int m_nID = -1;
    private string m_strSensorID = "";
    private string m_strState = CommonString.RiskLevel_Normal;
    private string m_strAddr = "";
    private DateTime m_dtMeasureTime;
    private float m_fDepth = 0;
    private float m_fFlow = 0;
    private string m_strMessage = "";

    private int m_nUserModifity = 0;
    public int UserModifity
    {
        get { return m_nUserModifity; }
        set { m_nUserModifity = value; }
    }

    public int ID
    {
        get { return m_nID; }
        set { m_nID = value; }
    }

    public string SensorID
    {
        get { return m_strSensorID; }
        set { m_strSensorID = value; }
    }

    public string State
    {
        get { return m_strState; }
        set { m_strState = value; }
    }

    public string Addr
    {
        get { return m_strAddr; }
        set { m_strAddr = value; }
    }

    public DateTime MeasureTime
    {
        get { return m_dtMeasureTime; }
        set { m_dtMeasureTime = value; }
    }

    public float Depth
    {
        get { return m_fDepth; }
        set { m_fDepth = value; }
    }

    public float Flow
    {
        get { return m_fFlow; }
        set { m_fFlow = value; }
    }

    public string Message
    {
        get { return m_strMessage; }
        set { m_strMessage = value; }
    }
}

public class CollapseSensor
{
    private int m_nID = -1;
    private string m_strSensorID = "";
    private string m_strState = CommonString.RiskLevel_Normal;
    private string m_strAddr = "";
    private DateTime m_dtMeasureTime;
    private string m_strMessage = "";

    private int m_nUserModifity = 0;
    public int UserModifity
    {
        get { return m_nUserModifity; }
        set { m_nUserModifity = value; }
    }

    public int ID
    {
        get { return m_nID; }
        set { m_nID = value; }
    }

    public string SensorID
    {
        get { return m_strSensorID; }
        set { m_strSensorID = value; }
    }

    public string State
    {
        get { return m_strState; }
        set { m_strState = value; }
    }

    public string Addr
    {
        get { return m_strAddr; }
        set { m_strAddr = value; }
    }

    public DateTime MeasureTime
    {
        get { return m_dtMeasureTime; }
        set { m_dtMeasureTime = value; }
    }

    public string Message
    {
        get { return m_strMessage; }
        set { m_strMessage = value; }
    }
}

public class DataReport
{
    private int m_nID = -1;
    private FacilityType m_facilityType;
    private int m_nSensorID;
    private DateTime m_dtOccurTime;
    private string m_strDataName = "";
    private string m_strOriginData = "";
    private string m_strNewData = "";


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

    public DateTime OccurTime
    {
        get { return m_dtOccurTime; }
        set { m_dtOccurTime = value; }
    }

    public string DataName
    {
        get { return m_strDataName; }
        set { m_strDataName = value; }
    }

    public string OriginData
    {
        get { return m_strOriginData; }
        set { m_strOriginData = value; }
    }

    public string NewData
    {
        get { return m_strNewData; }
        set { m_strNewData = value; }
    }

}

public class AlertReport
{
    private int m_nID = -1;
    private FacilityType m_facilityType;
    private int m_nSensorID;
    private DateTime m_dtOccurTime;
    private string m_strDataName = "";
    private string m_strOriginData = "";
    private string m_strNewData = "";


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

    public DateTime OccurTime
    {
        get { return m_dtOccurTime; }
        set { m_dtOccurTime = value; }
    }

    public string DataName
    {
        get { return m_strDataName; }
        set { m_strDataName = value; }
    }

    public string OriginData
    {
        get { return m_strOriginData; }
        set { m_strOriginData = value; }
    }

    public string NewData
    {
        get { return m_strNewData; }
        set { m_strNewData = value; }
    }

}

public class SMSReport
{
    private int m_nID = -1;
    private FacilityType m_facilityType;
    private int m_nSensorID;
    private DateTime m_dtOccurTime;
    private string m_strMessage = "";
    private string m_strManagers = "";

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

    public DateTime OccurTime
    {
        get { return m_dtOccurTime; }
        set { m_dtOccurTime = value; }
    }

    public string Message
    {
        get { return m_strMessage; }
        set { m_strMessage = value; }
    }

    public string Managers
    {
        get { return m_strManagers; }
        set { m_strManagers = value; }
    }


}

public class HeatData
{
    private int m_nID = -1;
    private string m_strSensorID = "";
    private string m_strAddr = "";
    private DateTime m_dtOccurTime;
    private float m_fTemperature = 0;
    private float m_fHumidity = 0;
    private float m_fDirection = 0;
    private float m_fSpeed = 0;
    private DateTime m_dtCreateTime;


    public int ID
    {
        get { return m_nID; }
        set { m_nID = value; }
    }

    public string SensorID
    {
        get { return m_strSensorID; }
        set { m_strSensorID = value; }
    }

    public string Addr
    {
        get { return m_strAddr; }
        set { m_strAddr = value; }
    }

    public DateTime OccurTime
    {
        get { return m_dtOccurTime; }
        set { m_dtOccurTime = value; }
    }

    public float Temperature
    {
        get { return m_fTemperature; }
        set { m_fTemperature = value; }
    }

    public float Humidity
    {
        get { return m_fHumidity; }
        set { m_fHumidity = value; }
    }

    public float Direction
    {
        get { return m_fDirection; }
        set { m_fDirection = value; }
    }

    public float Speed
    {
        get { return m_fSpeed; }
        set { m_fSpeed = value; }
    }

    public DateTime CreateTime
    {
        get { return m_dtCreateTime; }
        set { m_dtCreateTime = value; }
    }

}


public class AlarmData
{
    private int m_nID = -1;
    private FacilityType m_facilityType = FacilityType.NONE;
    private int m_nSersorID = -1;
    private string m_strRiskLevel = CommonString.RiskLevel_Normal_Kor;
    private string m_strAddress = "";
    private bool m_bCheck = false;
    private DateTime m_dtCreateTime;

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

    public int SersorID
    {
        get { return m_nSersorID; }
        set { m_nSersorID = value; }
    }

    public string RiskLevel
    {
        get { return m_strRiskLevel; }
        set { m_strRiskLevel = value; }
    }

    public string Address
    {
        get { return m_strAddress; }
        set { m_strAddress = value; }
    }

    public bool Check
    {
        get { return m_bCheck; }
        set { m_bCheck = value; }
    }

    public DateTime CreateTime
    {
        get { return m_dtCreateTime; }
        set { m_dtCreateTime = value; }
    }
}