using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace HSMS
{
    public enum ChangeDataType
    {
        WORKER = 1,
        CAR,
        EQUIPNAME,
        EQUIPSTANDARD,
        EQUIP,
        CARTYPE ,
        CARSTANDARD,
        DEPRT,
        COMPANY ,
        ZONELEVEL,
        SENSORDETECT,
        SMSCONFIG,
        ALARM_IGNORE_OPTIONS,
        MANAGER,
        IGNORE_SENSORS_TO_WORKER,
        ALARM_DISTANCE,
        CHANGE_ZONE_GROUP,
        EDIT_EQUIPMENT
    }

    public interface IChangedData
    {
        ChangeDataType GetChangedDataType();
    }

    public enum JoinUserResult
    {
        SUCCESS = 0,
        ALREADY_EXIST,
        INVALID_PASSWORD,
        INVALID_USER_LEVEL,
        DB_IS_DISCONNECTED,
        UNKNOWN_JOIN_OPTION,
        UNKNOWN_ERROR,
        TYPE_COUNT
    }

    public enum LoginUserResult
    {
        SUCCESS = 0,
        INVALID_ID,
        INVALID_PW,
        NEED_MORE_DATA,
        NOT_PERMIT_PC,
        DB_IS_DISCONNECTED,
        DUPLICATE_LOGIN,
        UNKNOWN_ERROR,
        TYPE_COUNT
    }

    public enum DeleteUserResult
    {
        SUCCESS = 0,
        INVALID_ID,
        INVALID_PW,
        DB_IS_DISCONNECTED,
        NEED_MORE_DATA,
        UNKNOWN_ERROR,
        TYPE_COUNT
    }

    public enum ChangePasswordResult
    {
        SUCCESS = 0,
        INVALID_ID,
        INVALID_PW,
        DB_IS_DISCONNECTED,
        NEED_MORE_DATA,
        INVALID_CERT_CODE,
        UNKNOWN_ERROR,
        TYPE_COUNT
    }

    public class LinkField
    {
        private int nFieldType = 1;
        public int FieldType
        {
            get { return nFieldType; }
            set { nFieldType = value; }
        }

        private string szFieldValue = "";
        public string FieldValue
        {
            get { return szFieldValue; }
            set 
            { 
                szFieldValue = value;
                ParseField();
            }
        }
        
        private string m_szDBName = "";
        public string DBName
        {
            get { return m_szDBName; }
        }

        private string m_szTableName = "";
        public string TableName
        {
            get { return m_szTableName; }
        }

        private string m_szSchemaName = "dbo";
        public string SchemaName
        {
            get { return m_szSchemaName; }
        }
        private string m_szFieldName = "";
        public string FieldName
        {
            get { return m_szFieldName; }
            set { m_szFieldName = value; }
        }
        private void ParseField()
        {
            if( szFieldValue == null || szFieldValue == "")
                return;

            string[] strTables = szFieldValue.Split('.');
            m_szDBName = strTables[0];
            m_szTableName = strTables[1];
            m_szFieldName = strTables[2];
        }       
    }

    public class DataCompany : IChangedData
    {
        //사업부코드
        private string m_szCompanyID = "";
        public string CompanyID
        {
            get { return m_szCompanyID; }
            set { m_szCompanyID = value; }
        }
        //사업부이름
        private string m_szCompanyName = "";
        public string CompanyName
        {
            get { return m_szCompanyName; }
            set { m_szCompanyName = value; }
        }

        //부서 데이터들
        private ArrayList m_arDepartment = new ArrayList();
        public ArrayList Departments
        {
            get { return m_arDepartment; }
            set { m_arDepartment = value; }
        }

        public override string ToString()
        {
            return m_szCompanyName;
        }

        public ChangeDataType GetChangedDataType()
        {
            return ChangeDataType.COMPANY;
        }
    }

    public class DataDepartment : IChangedData
    {
        //부서코드
        private string m_szCode = "";
        public string Code
        {
            get { return m_szCode; }
            set { m_szCode = value; }
        }
        //부서이름
        private string m_szName = "";
        public string Name
        {
            get { return m_szName; }
            set { m_szName = value; }
        }
        //작업자데이터들
        private ArrayList m_arWorkers = new ArrayList();
        public ArrayList Workers
        {
            get { return m_arWorkers; }
            set { m_arWorkers = value; }
        }

        public override string ToString()
        {
            return m_szName;
        }

        public ChangeDataType GetChangedDataType()
        {
            return ChangeDataType.DEPRT;
        }
    }

    public class Manager : IChangedData
    {
        public ChangeDataType GetChangedDataType()
        {
            return ChangeDataType.WORKER;
        }

        public int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }
        public string m_szMemberID = "";
        public string MemberID
        {
            get { return m_szMemberID; }
            set { m_szMemberID = value; }
        }
        public DataWorker m_Worker = null;
        public HSMS.DataWorker Worker
        {
            get { return m_Worker; }
            set { m_Worker = value; }
        }
        public int m_nSiteID = -1;
        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }
    }
    
    //작업자 데이터
    public class DataWorker : ISensorDetectIgnoreChnaged, IChangedData
    {
        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        //사원번호
        private string m_szID =  "";
	    public string MemberID
	    {
		    get { return m_szID; }
		    set { m_szID = value; }
	    }

        //사원 이름
        private string m_szName = "";
	    public string Name
	    {
		    get { return m_szName; }
		    set { m_szName = value; }
	    }

        //사업부 코드
        private string m_szCompanyCode = "";
        public string CompanyCode
        {
            get { return m_szCompanyCode; }
            set { m_szCompanyCode = value; }
        }

        //부서코드
        private string m_szTeamCode = "";
        public string TeamCode
        {
            get { return m_szTeamCode; }
            set { m_szTeamCode = value; }
        }

        private string m_szTeamName = "";
        public string TeamName
        {
            get { return m_szTeamName; }
            set { m_szTeamName = value; }
        }

        //직책코드
        private string m_szJobPositionCode = "";
        public string JobPositionCode
        {
            get { return m_szJobPositionCode; }
            set { m_szJobPositionCode = value; }
        }

        //직책명
        private string m_szJobPositionName = "";
        public string JobPositionName
        {
            get { return m_szJobPositionName; }
            set { m_szJobPositionName = value; }
        }


        //작업자 센서ID
        private string m_szSensor = "";
        public string Sensor
        {
            get { return m_szSensor; }
            set { m_szSensor = value; }
        }

        //전화번호
        private string m_szOfficePhoneNumber = "";
        public string OfficePhoneNumber
        {
            get { return m_szOfficePhoneNumber; }
            set { m_szOfficePhoneNumber = value; }
        }

        //핸드폰번호
        private string m_szMobilePhoneNumber = "";
        public string MobilePhoneNumber
        {
            get { return m_szMobilePhoneNumber; }
            set { m_szMobilePhoneNumber = value; }
        }

        //출입등급
        private int m_EnterLevel = -1;
        public int EnterLevel
        {
            get { return m_EnterLevel; }
            set { m_EnterLevel = value; }
        }

        //DB에 저장되어 있는 출입등급
        private int m_DBEnterLevel = -1;
        public int DBEnterLevel
        {
            get { return m_DBEnterLevel; }
            set { m_DBEnterLevel = value; }
        }


        //SiteID
        private int m_SiteID = -1;
        public int SiteID
        {
            get { return m_SiteID; }
            set { m_SiteID = value; }
        }

        //사업부 데이터
        private DataCompany m_company = null;
	    public DataCompany Company
	    {
		    get { return m_company; }
		    set { m_company = value; }
	    }

        //부서 데이터
        private DataDepartment m_Team = null;
	    public DataDepartment Team
	    {
		    get { return m_Team; }
		    set { m_Team = value; }
	    }

        private JobPosition m_JobPosition = null;
        public JobPosition JobPosition
        {
            get { return m_JobPosition; }
            set { m_JobPosition = value; }
        }

        public override string ToString()
        {
            return m_szName;
        }

        private SensorWorker m_sensorWorker = null;
        public SensorWorker SensorWorker
        {
            get { return m_sensorWorker; }
            set { m_sensorWorker = value; }
        }

        private bool m_bSensorDetect = true;
        public bool SensorDetect
        {
            get { return m_bSensorDetect; }
            set { m_bSensorDetect = value; }
        }
        private bool m_bSensorDetectDB = true;
        public bool DBSensorDetect
        {
            get { return m_bSensorDetectDB; }
            set { m_bSensorDetectDB = value; }
        }
        public ChangeDataType GetChangedDataType()
        {
            return ChangeDataType.WORKER;
        }
    }

    //차량 데이터
    public class DataCar : ISensorDetectIgnoreChnaged, IChangedData
    {
        // HSMS CAR ID
        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        //장비코드
        private string m_szID = "";
        public string Code
        {
            get { return m_szID; }
            set { m_szID = value; }
        }

        //장비이름
        private string m_szName = "";
        public string Name
        {
            get { return m_szName; }
            set { m_szName = value; }
        }

        //부서 코드
        private string m_szTeamCode = "";
        public string TeamCode
        {
            get { return m_szTeamCode; }
            set { m_szTeamCode = value; }
        }

        //제작회사
        private string m_szMakerCompany = "";
        public string MakerCompany
        {
            get { return m_szMakerCompany; }
            set { m_szMakerCompany = value; }
        }
        //길이(mm)
        private int m_szLength = -1;
        public int Length
        {
            get { return m_szLength; }
            set { m_szLength = value; }
        }
        //너비(mm)
        private int m_szWidth = -1;
        public int Width
        {
            get { return m_szWidth; }
            set { m_szWidth = value; }
        }
        //높이(mm)
        private int m_szHeight = -1;
        public int Height
        {
            get { return m_szHeight; }
            set { m_szHeight = value; }
        }
        //차량규격
        private string m_szStandard = "";
        public string Standard
        {
            get { return m_szStandard; }
            set { m_szStandard = value; }
        }

        //차종
        private string m_szType = "";
        public string Type
        {
            get { return m_szType; }
            set { m_szType = value; }
        }

        //차량번호
        private string m_szNumber = "";
        public string Number
        {
            get { return m_szNumber; }
            set { m_szNumber = value; }
        }

        //차량센서ID
        private string m_szSensor = "";
        public string Sensor
        {
            get { return m_szSensor; }
            set { m_szSensor = value; }
        }

        //사용용도
        private string m_szUse = "";
        public string Use
        {
            get { return m_szUse; }
            set { m_szUse = value; }
        }

        //운전자이름
        private string m_szDriverName = "";
        public string DriverName
        {
            get { return m_szDriverName; }
            set { m_szDriverName = value; }
        }

        //차량규격
        private DataCarType m_CarType = null;
        public DataCarType CarType
        {
            get { return m_CarType; }
            set { m_CarType = value; }
        }

        //차종
        private DataCarStandard m_CarStandard = null;
        public DataCarStandard CarStandard
        {
            get { return m_CarStandard; }
            set { m_CarStandard = value; }
        }

        //SiteID
        private int m_SiteID = -1;
        public int SiteID
        {
            get { return m_SiteID; }
            set { m_SiteID = value; }
        }

        public override string ToString()
        {
            return m_szName;
        }

        private SensorVehicle m_sensorVehicle = null;
        public SensorVehicle SensorVehicle
        {
            get { return m_sensorVehicle; }
            set { m_sensorVehicle = value; }
        }

        private bool m_bSensorDetect = true;
        public bool SensorDetect
        {
            get { return m_bSensorDetect; }
            set { m_bSensorDetect = value; }
        }

        private bool m_bSensorDetectDB = true;
        public bool DBSensorDetect
        {
            get { return m_bSensorDetectDB; }
            set { m_bSensorDetectDB = value; }
        }

        public ChangeDataType GetChangedDataType()
        {
            return ChangeDataType.CAR;
        }
    }

    //차량규격
    public class DataCarStandard : IChangedData
    {
        //차종코드
        private string m_szCarID = "";
        public string CarID
        {
            get { return m_szCarID; }
            set { m_szCarID = value; }
        }

        //차종이름
        private string m_szName = "";
        public string Name
        {
            get { return m_szName; }
            set { m_szName = value; }
        }

        //차량 데이터들
        private ArrayList m_arCars = new ArrayList();
        public ArrayList Cars
        {
            get { return m_arCars; }
            set { m_arCars = value; }
        }

        public override string ToString()
        {
            return m_szName;
        }
        public ChangeDataType GetChangedDataType()
        {
            return ChangeDataType.CARSTANDARD;
        }
    }


    //차종
    public class DataCarType : IChangedData
    {
        //private string m_szCarID = "";
        //public string CarID
        //{
        //    get { return m_szCarID; }
        //    set { m_szCarID = value; }
        //}

        //차종코드
        private string m_szCode = "";
        public string Code
        {
            get { return m_szCode; }
            set { m_szCode = value; }
        }

        //차종이름
        private string m_szName = "";
        public string Name
        {
            get { return m_szName; }
            set { m_szName = value; }
        }

        //차량규격 데이터들..
        private ArrayList m_arCarStandards = new ArrayList();
        public ArrayList CarStandards
        {
            get { return m_arCarStandards; }
            set { m_arCarStandards = value; }
        }

        public override string ToString()
        {
            return m_szName;
        }
        public ChangeDataType GetChangedDataType()
        {
            return ChangeDataType.CARTYPE;
        }
    }

    public class GasSensor
    {
        private static string m_strIconPath = "";
        private float x = 0.0f;
        private float y = 0.0f;
        private float z = 0.0f;
        private string m_strSensorName = "";
        private string m_strDescription = "";
        private int m_nID = -1;
        private string m_strSensorID = "";
        private SensorVehicle m_vehicle = null;

        public static string IconPath
        {
            get { return m_strIconPath; }
            set { m_strIconPath = value; }
        }

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string SensorName
        {
            get { return m_strSensorName; }
            set { m_strSensorName = value; }
        }

        public float X
        {
            get { return x; }
            set { x = value; }
        }

        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        public float Z
        {
            get { return z; }
            set { z = value; }
        }

        public SensorVehicle SensorVehicle
        {
            get { return m_vehicle; }
            set { m_vehicle = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public string SensorID
        {
            get { return m_strSensorID; }
            set { m_strSensorID = value; }
        }

        public GasSensor()
        {
        }

        public GasSensor(int nID, string strSensorName, float x, float y, float z, string strDesc)
        {
            m_nID = nID;
            m_strSensorName = strSensorName;
            this.x = x;
            this.y = y;
            this.z = z;
            m_strDescription = strDesc;
        }
    }

    public class PrimitiveData<T>
    {
        private T m_data;

        public T Data
        {
            get { return m_data; }
            set { m_data = value; }
        }

        public PrimitiveData()
        {
        }

        public PrimitiveData(T data)
        {
            m_data = data;
        }
    }

    public class _3DEquipment
    {
        // Crane 센서의 Y축 이동 범위
        private PrimitiveData<double> m_minMovedY = null;
        private PrimitiveData<double> m_maxMovedY = null;
        // Crane 센서의 X축 이동 범위
        private PrimitiveData<double> m_minMovedX = null;
        private PrimitiveData<double> m_maxMovedX = null;

        private DataEquip m_equip = null;
        public DataEquip Equipment
        {
            get { return m_equip; }
            set { m_equip = value; }
        }

        public PrimitiveData<double> MinMovedY
        {
            get { return m_minMovedY; }
            set { m_minMovedY = value; }
        }

        public PrimitiveData<double> MaxMovedY
        {
            get { return m_maxMovedY; }
            set { m_maxMovedY = value; }
        }

        public PrimitiveData<double> MinMovedX
        {
            get { return m_minMovedX; }
            set { m_minMovedX = value; }
        }

        public PrimitiveData<double> MaxMovedX
        {
            get { return m_maxMovedX; }
            set { m_maxMovedX = value; }
        }

        public virtual void SetPosition(float x, float y, float z) {}
        public virtual void Select() {}
        public virtual void Unselect() {}
        public virtual object GetLinkedObject() { return null; }
        public virtual bool GetDistance(UnE.Geometry.Vertex2D vPos, UnE.Geometry.Vertex2D vEquipPos, out double distance)
        {
            distance = 0.0;
            return false;
        }

        // 설비는 일반적으로 움직일 수 있는 범위가 제한적이기 때문에, Sensor 오류로 인하여
        // vMoved만큼 움직였다고 계산된 값이 올바르지 않을수 있다.
        // 이 값이 설비의 이동 범위를 벗어날 경우 보정해준다.
        public virtual UnE.Geometry.Vertex2D GetFixedMoved(UnE.Geometry.Vertex2D vMoved)
        {
            UnE.Geometry.Vertex2D vFixed = null;

            if (m_minMovedX != null && m_maxMovedX != null)
            {
                if (vMoved.x < m_minMovedX.Data)
                {
                    vFixed = new UnE.Geometry.Vertex2D(vMoved.x, vMoved.y);
                    vFixed.x = m_minMovedX.Data;
                }

                if (vMoved.x > m_maxMovedX.Data)
                {
                    vFixed = new UnE.Geometry.Vertex2D(vMoved.x, vMoved.y);
                    vFixed.x = m_maxMovedX.Data;
                }
            }

            if (m_minMovedY != null && m_maxMovedY != null)
            {
                if (vMoved.y < m_minMovedY.Data)
                {
                    if (vFixed == null)
                        vFixed = new UnE.Geometry.Vertex2D(vMoved.x, vMoved.y);
                    vFixed.y = m_minMovedY.Data;
                }

                if (vMoved.y > m_maxMovedY.Data)
                {
                    if (vFixed == null)
                        vFixed = new UnE.Geometry.Vertex2D(vMoved.x, vMoved.y);
                    vFixed.y = m_maxMovedY.Data;
                }
            }

            return vFixed == null ? vMoved : vFixed;
        }
    }

    public partial class Crane3D : _3DEquipment
    {
        public Crane3D()
        {
            MinMovedY = new PrimitiveData<double>(-9.0);
            MaxMovedY = new PrimitiveData<double>(9.0);
        }

        public override bool GetDistance(UnE.Geometry.Vertex2D vPos, UnE.Geometry.Vertex2D vEquipPos, out double distance)
        {
            // 갈고리의 영역을 1m, 센서가 그 중앙에 위치한다고 가정한다.
            distance = vPos.GetDistance(vEquipPos) - 1.0;

            if (distance < 0.0)
                distance = 0.0;

            return true;
        }
    }

    public partial class MovingEquip3D : _3DEquipment
    {
        public MovingEquip3D()
        {
        }
    }

    public class EquipmentGroup
    {
        private string m_strGroupName = "";
        private ArrayList m_arrEquipments = new ArrayList();
        private static EquipmentGroup m_defEquipGroup = null;

        public string GroupName
        {
            get { return m_strGroupName; }
            set { m_strGroupName = value; }
        }

        public static EquipmentGroup DefaultEquipmentGroup
        {
            get
            {
                if (m_defEquipGroup == null)
                    m_defEquipGroup = new EquipmentGroup("Default");

                return m_defEquipGroup;
            }
        }

        public EquipmentGroup()
        {
        }

        public EquipmentGroup(string strGroupName)
        {
            m_strGroupName = strGroupName;
        }

        public int GetEquipmentCount()
        {
            return m_arrEquipments.Count;
        }

        public DataEquip GetEquipment(int nIndex)
        {
            if (nIndex < 0 || nIndex >= GetEquipmentCount())
                return null;

            return (DataEquip)m_arrEquipments[nIndex];
        }

        public void AddEquipment(DataEquip equip)
        {
            if (equip != null)
                m_arrEquipments.Add(equip);
        }

        public void RemoveEquipment(DataEquip equip)
        {
            m_arrEquipments.Remove(equip);
        }

        public DataEquip RemoveZone(int nIndex)
        {
            if (nIndex < 0 || nIndex >= GetEquipmentCount())
                return null;

            DataEquip zone = (DataEquip)m_arrEquipments[nIndex];
            m_arrEquipments.RemoveAt(nIndex);
            return zone;
        }

        public bool Contains(DataEquip zone)
        {
            return m_arrEquipments.Contains(zone);
        }

        public override string ToString()
        {
            if (this == m_defEquipGroup)
                return "기본 설비";

            return m_strGroupName;
        }
    }

    //설비
    public class DataEquip : ISensorDetectIgnoreChnaged, IChangedData
    {
        // HSMS Equip ID
        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        //설비코드
        private string m_szID = "";
        public string Code
        {
            get { return m_szID; }
            set { m_szID = value; }
        }

        //설비이름
        private string m_szName = "";
        public string Name
        {
            get { return m_szName; }
            set { m_szName = value; }
        }

        //설비 규격
        private string m_szStandard = "";
        public string Standard
        {
            get { return m_szStandard; }
            set { m_szStandard = value; }
        }

        //제조번호
        private string m_szNumber = "";
        public string Number
        {
            get { return m_szNumber; }
            set { m_szNumber = value; }
        }

        //제작회사
        private string m_szMaker = "";
        public string Maker
        {
            get { return m_szMaker; }
            set { m_szMaker = value; }
        }
        //센서
        private string m_szSensor = "";
        public string Sensor
        {
            get { return m_szSensor; }
            set { m_szSensor = value; }
        }

        //운전자이름
        private string m_szDriverName = "";
        public string DriverName
        {
            get { return m_szDriverName; }
            set { m_szDriverName = value; }
        }

        //모델명
        private string m_szTypeName = "";
        public string TypeName
        {
            get { return m_szTypeName; }
            set { m_szTypeName = value; }
        }

        //설비명
        private DataEquipName m_EquipName = null;
        public DataEquipName EquipName
        {
            get { return m_EquipName; }
            set { m_EquipName = value; }
        }

        //설비규격
        private DataEquipStandard m_EquipStandard = null;
        public DataEquipStandard EquipStandard
        {
            get { return m_EquipStandard; }
            set { m_EquipStandard = value; }
        }

        //SiteID
        private int m_SiteID = -1;
        public int SiteID
        {
            get { return m_SiteID; }
            set { m_SiteID = value; }
        }

        private UnE.Geometry.Polygon m_boundary = null;
        public UnE.Geometry.Polygon Boundary
        {
            get { return m_boundary; }
            set { m_boundary = value; }
        }

        // 설비가 원래 위치에서 얼만큼 이동하였는가?
        private UnE.Geometry.Vertex2D m_vMoved = new UnE.Geometry.Vertex2D();
        public UnE.Geometry.Vertex2D Moved
        {
            get { return m_vMoved; }
            set { m_vMoved = value; }
        }

        // m_boundary 위에서 센서는 설비의 어디에 위치해 있는가?
        // m_boundary의 (x 최소값, y 최소값)을 원점으로 두고 계산한 상대좌표
        private UnE.Geometry.Vertex2D m_vSensorPosition = new UnE.Geometry.Vertex2D();
        public UnE.Geometry.Vertex2D SensorPosition
        {
            get { return m_vSensorPosition; }
            set { m_vSensorPosition = value; }
        }

        // 움직이는 설비의 방향 벡터
        // 단위 방향벡터 방향으로 SetLocation(0)과 SetLocation(1)의 거리 차이만큼 이동한 값
        private UnE.Geometry.Vertex2D m_vSensorDirVector = new UnE.Geometry.Vertex2D();
        public UnE.Geometry.Vertex2D SensorDirVector
        {
            get { return m_vSensorDirVector; }
            set { m_vSensorDirVector = value; }
        }

        // 움직이는 설비가 최대 이동 지점까지 움직였을 경우 센서의 위치.
        // SensorPosition은 설비가 최초 지점에 있을 경우 센서의 위치이다.
        private UnE.Geometry.Vertex2D m_vSensorFinishPosition = new UnE.Geometry.Vertex2D();
        public UnE.Geometry.Vertex2D SensorFinishPosition
        {
            get { return m_vSensorFinishPosition; }
            set { m_vSensorFinishPosition = value; }
        }

        // 도면 위에서 설비의 원래 위치
        // 실제 설비의 좌표는 m_boundary에 m_vOriginPosition을 더한 값이다.
        private UnE.Geometry.Vertex2D m_vOriginPosition = new UnE.Geometry.Vertex2D();
        public UnE.Geometry.Vertex2D OriginPosition
        {
            get { return m_vOriginPosition; }
            set { m_vOriginPosition = value; }
        }
        
        public override string ToString()
        {
            return m_szName;
        }

        private bool m_bSensorDetect = true;
        public bool SensorDetect
        {
            get { return m_bSensorDetect; }
            set { m_bSensorDetect = value; }
        }
        private bool m_bSensorDetectDB = true;
        public bool DBSensorDetect
        {
            get { return m_bSensorDetectDB; }
            set { m_bSensorDetectDB = value; }
        }

        private _3DEquipment m_3dEquip = null;
        public _3DEquipment Linked3DEquipment
        {
            get { return m_3dEquip; }
            set
            {
                m_3dEquip = value;

                if (m_3dEquip != null)
                    m_3dEquip.Equipment = this;
            }
        }

        private EquipmentGroup m_equipGroup = null;
        public EquipmentGroup EquipmentGroup
        {
            get { return m_equipGroup; }
            set { SetEquipmentGroup(value); }
        }

        public DataEquip()
        {
        }

        public DataEquip(EquipmentGroup group)
        {
            SetEquipmentGroup(group);
        }

        private void SetEquipmentGroup(EquipmentGroup group)
        {
            if (m_equipGroup != group)
            {
                if (group == null)
                {
                    m_equipGroup.RemoveEquipment(this);
                }
                else if (m_equipGroup != null)
                {
                    m_equipGroup.RemoveEquipment(this);
                }

                m_equipGroup = group;

                if (m_equipGroup != null)
                {
                    if (!m_equipGroup.Contains(this))
                        m_equipGroup.AddEquipment(this);
                }
            }
        }

        // 설비가 처음 위치로부터 (xMove, yMove)만큼 움직였을 경우 SetLocation(distance)에 입력시킬
        // 거리를 구한다.
        public double GetMovingDistance(double xMove, double yMove)
        {
            double dMaxDistance = SensorPosition.GetDistance(SensorFinishPosition);

            UnE.Geometry.Vertex2D vOrigin = new UnE.Geometry.Vertex2D(0, 0);
            UnE.Geometry.Vertex2D vertex = new UnE.Geometry.Vertex2D(xMove, yMove);
            UnE.Geometry.Vertex2D vTemp = UnE.Geometry.Math.GetNearestVertex(vertex, vOrigin, SensorDirVector, true);

            double distance = vOrigin.GetDistance(vTemp);

            if (distance > dMaxDistance)
                distance = dMaxDistance;

            UnE.Geometry.Vertex2D vTempMirror = new UnE.Geometry.Vertex2D(-vTemp.x, -vTemp.y);

            double len1 = SensorDirVector.GetDistance(vTemp);
            double len2 = SensorDirVector.GetDistance(vTempMirror);

            return len1 < len2 ? distance : -distance;
        }

        // 설비가 처음 위치로부터 (xMove, yMove)만큼 움직였을 경우 SetLocation(distance)에 입력시킬
        // 거리를 구한다.
        /*public double GetMovingDistance(double xMove, double yMove)
        {
            if (SensorPosition.GetDistance(SensorFinishPosition) <= UnE.Geometry.Math.HALF_TOLERANCE())
                return 0.0;
                
            UnE.Geometry.Vertex2D vertex = new UnE.Geometry.Vertex2D(xMove, yMove);
            UnE.Geometry.Vertex2D vDirVector = SensorFinishPosition - SensorPosition;
            UnE.Geometry.Vertex2D vOrigin = new UnE.Geometry.Vertex2D(0, 0);

            UnE.Geometry.Vertex2D vTarget = UnE.Geometry.Math.GetNearestVertex(vertex, vOrigin, vDirVector, true);

            double dLen1 = vOrigin.GetDistance(vTarget);
            double dLen2 = vDirVector.GetDistance(vTarget);
            double dResult = 0.0;
            double min = 0.0, max = SensorPosition.GetDistance(SensorFinishPosition);
            
            UnE.Geometry.Line2D vLine = new UnE.Geometry.Line2D(vOrigin, vDirVector, UnE.Geometry.Line2D.LineType.SEGMENT);
            
            if (vLine.IsInclude(vTarget))
                dResult = dLen1;
            else
            {
                if (dLen1 < dLen2)
                    dResult = -dLen1;
                else
                    dResult = dLen1;

                if (dResult > max)
                    dResult = max;
                if (dResult < min)
                    dResult = min;
            }

            return dResult;
        }*/

        public ChangeDataType GetChangedDataType()
        {
            return ChangeDataType.EQUIP;
        }

        public double GetDistance(UnE.Geometry.Vertex2D vPos)
        {
            if (m_3dEquip != null)
            {
                double distance;
                UnE.Geometry.Vertex2D vMoved = m_3dEquip.GetFixedMoved(Moved);

                if (m_3dEquip.GetDistance(vPos, SensorPosition + OriginPosition + Moved, out distance))
                    return distance;
            }

            // 설비의 움직임만큼 작업자 좌표도 위치 이동시킨다
            //vPos = vPos - OriginPosition - Moved;
            //return m_boundary.GetDistance(vPos);
            double x = vPos.x - OriginPosition.x - Moved.x;
            double y = -(vPos.y - OriginPosition.y - Moved.y);
            return m_boundary.GetDistance(new UnE.Geometry.Vertex2D(x, y));
        }

        public double GetDistance(UnE.Geometry.Vertex2D vPos, UnE.Geometry.Vertex2D vSensorPos)
        {
            UnE.Geometry.Vertex2D vMoved = vSensorPos - OriginPosition;

            // 설비의 움직임만큼 작업자 좌표도 위치 이동시킨다
            double x = vPos.x - OriginPosition.x - vMoved.x;
            double y = -(vPos.y - OriginPosition.y - vMoved.y);
            return m_boundary.GetDistance(new UnE.Geometry.Vertex2D(x, y));
        }

        public _3DEquipment SetLiked3DEquipmentFromName()
        {
            string strEquipName = Name.ToLower();

            if (strEquipName.Contains("movingequip") || strEquipName.Contains("moving_equip"))
                Linked3DEquipment = new MovingEquip3D();
            else if (strEquipName.Contains("crane") || strEquipName.Contains("크레인"))
                Linked3DEquipment = new Crane3D();

            return Linked3DEquipment;
        }
    }
    
    //설비명
    public class DataEquipName : IChangedData
    {
        //설비코드
        private string m_szID = "";
        public string ID
        {
            get { return m_szID; }
            set { m_szID = value; }
        }

        //설비이름
        private string m_szName = "";
        public string Name
        {
            get { return m_szName; }
            set { m_szName = value; }
        }

        //설비규격 데이터들..
        private ArrayList m_arEquipStandards = new ArrayList();
        public ArrayList EquipStandards
        {
            get { return m_arEquipStandards; }
            set { m_arEquipStandards = value; }
        }

        public override string ToString()
        {
            return m_szName;
        }

        public ChangeDataType GetChangedDataType()
        {
            return ChangeDataType.EQUIPNAME;
        }
    }

    //설비규격
    public class DataEquipStandard : IChangedData
    {
        //설비코드
        private string m_szID = "";
        public string ID
        {
            get { return m_szID; }
            set { m_szID = value; }
        }

        //규격
        private string m_szName = "";
        public string Name
        {
            get { return m_szName; }
            set { m_szName = value; }
        }

        //설비 데이터들..
        private ArrayList m_arEquips = new ArrayList();
        public ArrayList Equips
        {
            get { return m_arEquips; }
            set { m_arEquips = value; }
        }

        public override string ToString()
        {
            return m_szName;
        }
        public ChangeDataType GetChangedDataType()
        {
            return ChangeDataType.EQUIPSTANDARD;
        }
    }

    public class EquipmentRawData
    {
        // HSMS ID
        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        //설비코드
        private string m_szID = "";
        public string Code
        {
            get { return m_szID; }
            set { m_szID = value; }
        }

        //설비이름
        private string m_szName = "";
        public string Name
        {
            get { return m_szName; }
            set { m_szName = value; }
        }

        //Boundary
        private string m_szBoundary = "";
        public string Boundary
        {
            get { return m_szBoundary; }
            set { m_szBoundary = value; }
        }

        //SiteID
        private int m_szSiteID = -1;
        public int SiteID
        {
            get { return m_szSiteID; }
            set { m_szSiteID = value; }
        }

        //TextCenter
        private string m_szTextCenter = "";
        public string TextCenter
        {
            get { return m_szTextCenter; }
            set { m_szTextCenter = value; }
        }

        private string m_szSensorPos = "";
        public string SensorPos
        {
            get { return m_szSensorPos; }
            set { m_szSensorPos = value; }
        }
        private string m_szSensorDirVector = "";
        public string SensorDirVector
        {
            get { return m_szSensorDirVector; }
            set { m_szSensorDirVector = value; }
        }
        private string m_szSensorFinishPos = "";
        public string SensorFinishPos
        {
            get { return m_szSensorFinishPos; }
            set { m_szSensorFinishPos = value; }
        }
    }

    public class JobPosition
    {
        private string m_zsCode = "";
        public string Code
        {
            get { return m_zsCode; }
            set { m_zsCode = value; }
        }

        private string m_szName = "";
        public string Name
        {
            get { return m_szName; }
            set { m_szName = value; }
        }
    }

    public class EventSensorData : IComparable
    {
        private DateTime m_eventTime = new DateTime();
        private double x = 0.0, y = 0.0;
        private string m_strSensorID = "";

        public DateTime EventTime
        {
            get { return m_eventTime; }
            set { m_eventTime = value; }
        }

        public double X
        {
            get { return x; }
            set { x = value; }
        }

        public double Y
        {
            get { return y; }
            set { y = value; }
        }

        public string SensorID
        {
            get { return m_strSensorID; }
            set { m_strSensorID = value; }
        }

        public int CompareTo(object obj)
        {
            EventSensorData data = (EventSensorData)obj;

            if (this.m_eventTime > data.m_eventTime)
                return 1;
            else if (this.m_eventTime < data.m_eventTime)
                return -1;
            //else
            return 0;
        }

        public EventSensorData()
        {
        }

        public EventSensorData(string strSensorID, DateTime dtEvent, double x, double y)
        {
            m_strSensorID = strSensorID;
            m_eventTime = dtEvent;
            this.x = x;
            this.y = y;
        }
    }

    public class ZoneGroup : object
    {
        private string m_strGroupName = "";
        private ArrayList m_arrZones = new ArrayList();
        private static ZoneGroup m_defZoneGroup = null;

        public string GroupName
        {
            get { return m_strGroupName; }
            set { m_strGroupName = value; }
        }

        public static ZoneGroup DefaultZoneGroup
        {
            get
            {
                if (m_defZoneGroup == null)
                    m_defZoneGroup = new ZoneGroup("Default");

                return m_defZoneGroup;
            }
        }

        public ZoneGroup()
        {
        }

        public ZoneGroup(string strGroupName)
        {
            m_strGroupName = strGroupName;
        }

        public int GetZoneCount()
        {
            return m_arrZones.Count;
        }

        public DataZone GetZone(int nIndex)
        {
            if (nIndex < 0 || nIndex >= GetZoneCount())
                return null;

            return (DataZone)m_arrZones[nIndex];
        }

        public void AddZone(DataZone zone)
        {
            if (zone != null)
                m_arrZones.Add(zone);
        }

        public void RemoveZone(DataZone zone)
        {
            m_arrZones.Remove(zone);
        }

        public DataZone RemoveZone(int nIndex)
        {
            if (nIndex < 0 || nIndex >= GetZoneCount())
                return null;

            DataZone zone = (DataZone)m_arrZones[nIndex];
            m_arrZones.RemoveAt(nIndex);
            return zone;
        }

        public bool Contains(DataZone zone)
        {
            return m_arrZones.Contains(zone);
        }

        public override string ToString()
        {
            if (this == m_defZoneGroup)
                return "기본 영역";

            return m_strGroupName;
        }
    }

    public class DataZone
    {
        private int m_nID = -1;
        // 출입이 허가된 출입등급 리스트(0보다 큰 정수)
        // m_arrPermitLevels가 비어 있으면 누구나 출입할 수 있다.
        // m_arrPermitLevels에 0이 있으면 아무도 출입할 수 없다.
        private ArrayList m_arrPermitLevels = new ArrayList();
        private string m_strZoneName = "";
        // 이 값이 null이면 Boundary의 무게 중심을 TextCenter로 사용한다.
        private UnE.Geometry.Vertex2D m_vTextCenter = null;
        private UnE.Geometry.Polygon m_boundary = null;
        private ZoneGroup m_zoneGroup = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string ZoneName
        {
            get { return m_strZoneName; }
            set { m_strZoneName = value; }
        }

        public UnE.Geometry.Vertex2D TextCenter
        {
            get
            {
                if (m_vTextCenter != null)
                    return m_vTextCenter;

                if (m_boundary != null)
                    m_vTextCenter = m_boundary.CalcWeightCenter();

                return m_vTextCenter;
            }
            set { m_vTextCenter = value; }
        }

        public UnE.Geometry.Polygon Boundary
        {
            get { return m_boundary; }
            set { m_boundary = value; }
        }

        public ZoneGroup ZoneGroup
        {
            get { return m_zoneGroup; }
            set { SetZoneGroup(value); }
        }

        public DataZone()
        {
        }

        public DataZone(ZoneGroup group)
        {
            SetZoneGroup(group);
        }

        private void SetZoneGroup(ZoneGroup group)
        {
            if (m_zoneGroup != group)
            {
                if (group == null)
                {
                    m_zoneGroup.RemoveZone(this);
                }
                else if (m_zoneGroup != null)
                {
                    m_zoneGroup.RemoveZone(this);
                }

                m_zoneGroup = group;

                if (m_zoneGroup != null)
                {
                    if (!m_zoneGroup.Contains(this))
                        m_zoneGroup.AddZone(this);
                }
            }
        }

        public void AddPermitLevel(int nPermitLevel)
        {
            if (!m_arrPermitLevels.Contains(nPermitLevel))
                m_arrPermitLevels.Add(nPermitLevel);
        }

        public bool FindPermitLevel(int nPermitLevel)
        {
            return m_arrPermitLevels.Contains(nPermitLevel);
        }

        public int GetPermitLevelCount()
        {
            return m_arrPermitLevels.Count;
        }

        // nIndex가 범위를 벗어나면 0을 리턴한다.
        public int GetPermitLevel(int nIndex)
        {
            if (nIndex < 0 || nIndex >= GetPermitLevelCount())
                return 0;

            return (int)m_arrPermitLevels[nIndex];
        }

        public void RemovePermitLevel(int nPermitLevel)
        {
            m_arrPermitLevels.Remove(nPermitLevel);
        }

        public void RemovePermitLevelAt(int nIndex)
        {
            if (nIndex < 0 || nIndex >= GetPermitLevelCount())
                return;

            m_arrPermitLevels.RemoveAt(nIndex);
        }

        public void RemoveAllPermitLevels()
        {
            m_arrPermitLevels.Clear();
        }

        public override string ToString()
        {
            return m_strZoneName;
        }

        public object Clone()
        {
            DataZone zone = new DataZone();
            zone.m_arrPermitLevels = (ArrayList)m_arrPermitLevels.Clone();
            zone.m_boundary = m_boundary;
            zone.m_nID = m_nID;
            zone.m_strZoneName = m_strZoneName;
            zone.m_vTextCenter = m_vTextCenter;
            zone.m_zoneGroup = m_zoneGroup;
            return zone;
        }
    }

    public interface ISensorDetectIgnoreChnaged
    {
        bool SensorDetect
        {
            get;
            set;
        }      
        bool DBSensorDetect
        {
            get;
            set;
        }
    }

    public class DetectIgnoreWorker
    {
        private int m_nWorkerID = -1;
        public int WorkerID
        {
            get { return m_nWorkerID; }
            set { m_nWorkerID = value; }
        }
        private int m_nIgnoreObjectID = -1;
        public int IgnoreObjectID
        {
            get { return m_nIgnoreObjectID; }
            set { m_nIgnoreObjectID = value; }
        }
        private int m_nIgnoreObjectType = -1;
        public int IgnoreObjectType
        {
            get { return m_nIgnoreObjectType; }
            set { m_nIgnoreObjectType = value; }
        }
        private int m_nSiteID = -1;
        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }
        private DataWorker m_Worker = null;
        public DataWorker Worker
        {
            get { return m_Worker; }
            set { m_Worker = value; }
        }
        
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("DetectIgnoreWorker_");
            sb.Append(m_nWorkerID);
            sb.Append('_');
            sb.Append(m_nIgnoreObjectType);
            sb.Append('_');
            sb.Append(m_nIgnoreObjectID);
            return sb.ToString();
        }
    }

    public class APData
    {
        private static string m_strIconPath = "";

        private int m_nID = -1;
        private string m_strAPName = "";
        private float x = 0.0f;
        private float y = 0.0f;
        private float z = 0.0f;
        private int m_nTextPOIID = -1;
        private string m_strDescription = "";

        public static string IconPath
        {
            get { return m_strIconPath; }
            set { m_strIconPath = value; }
        }

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string APName
        {
            get { return m_strAPName; }
            set { m_strAPName = value; }
        }

        public float X
        {
            get { return x; }
            set { x = value; }
        }

        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        public float Z
        {
            get { return z; }
            set { z = value; }
        }

        public int TextPOIID
        {
            get { return m_nTextPOIID; }
            set { m_nTextPOIID = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public APData()
        {
        }

        public APData(int nID, string strAPName, float x, float y, float z, string strDesc)
        {
            m_nID = nID;
            m_strAPName = strAPName;
            this.x = x;
            this.y = y;
            this.z = z;
            m_strDescription = strDesc;
        }
    }
}
