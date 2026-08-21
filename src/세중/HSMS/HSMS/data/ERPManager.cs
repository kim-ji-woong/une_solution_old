using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections;

namespace HSMS
{
    public class ERPManager
    {
        public static ERPManager m_Instance = null;
        public static ERPManager Instance
        {
            get 
            {
                if (m_Instance == null)
                    m_Instance = new ERPManager();
                return m_Instance; 
            }          
        }

        private Dictionary<string, DataCompany> m_dicComapny = new Dictionary<string, DataCompany>();
        public Dictionary<string, DataCompany> DicComapny
        {
            get { return m_dicComapny; }
        }

        private Dictionary<string, DataDepartment> m_dicDepart = new Dictionary<string, DataDepartment>();
        public Dictionary<string, DataDepartment> DicTeams
        {
            get { return m_dicDepart; }
        }

        private Dictionary<string, DataWorker> m_dicCompanyWorkers = new Dictionary<string, DataWorker>();
        public Dictionary<string, DataWorker> DicCompanyWorkers
        {
            get { return m_dicCompanyWorkers; }
        }

        private Dictionary<string, DataCar> m_dicCompanyCars = new Dictionary<string, DataCar>();
        public Dictionary<string, DataCar> DicCompanyCars
        {
            get { return m_dicCompanyCars; }
        }

        private Dictionary<string, DataCarType> m_dicCarTypes = new Dictionary<string, DataCarType>();
        public Dictionary<string, DataCarType> DicCarTypes
        {
            get { return m_dicCarTypes; }
        }

        private Dictionary<string, DataCarStandard> m_dicCarStandards = new Dictionary<string, DataCarStandard>();
        public Dictionary<string, DataCarStandard> DicCarStandards
        {
            get { return m_dicCarStandards; }
        }

        private Dictionary<string, DataEquip> m_dicEquips = new Dictionary<string, DataEquip>();
        public Dictionary<string, DataEquip> DicEquips
        {
            get { return m_dicEquips; }
            set { m_dicEquips = value; }
        }

        private Dictionary<string, DataEquipName> m_dicEquipNames = new Dictionary<string, DataEquipName>();
        public Dictionary<string, DataEquipName> DicEquipName
        {
            get { return m_dicEquipNames; }
            set { m_dicEquipNames = value; }
        }
        
        private Dictionary<string, DataEquipStandard> m_dicEquipStandards = new Dictionary<string, DataEquipStandard>();
        public Dictionary<string, DataEquipStandard> DicEquipStandard
        {
            get { return m_dicEquipStandards; }
            set { m_dicEquipStandards = value; }
        }
        
        //직책코드, 직책
        private Dictionary<string, JobPosition> m_dicDutyCodeName = new Dictionary<string, JobPosition>();
        public Dictionary<string, JobPosition> DicDutyCodeName
        {
            get { return m_dicDutyCodeName; }
            set { m_dicDutyCodeName = value; }
        }


        //센서ID, 위험물
        private Dictionary<string, DataEquip> m_dicSensorEquip = new Dictionary<string, DataEquip>();
        public Dictionary<string, DataEquip> DicSensorEquip
        {
            get { return m_dicSensorEquip; }
            set { m_dicSensorEquip = value; }
        }

        //센서ID, 차량
        private Dictionary<string, DataCar> m_dicSensorCar = new Dictionary<string, DataCar>();
        public Dictionary<string, DataCar> DicSensorCar
        {
            get { return m_dicSensorCar; }
            set { m_dicSensorCar = value; }
        }

        private DBConn m_ConnectionHSMS = null;
        private DBConn m_Connectionhpublic00 = null;
        private DBConn m_Connectionhwinmm = null;
        protected ERPManager()
        {
            m_ConnectionHSMS = new DBConn("HSMS");
            m_Connectionhpublic00 = new DBConn("hpublic00");
            m_Connectionhwinmm = new DBConn("hwinmm");
                 
            ReadErpData();                
        }

        public void ReloadErpData()
        {
            m_bReadData = false;
            ReadErpData();
        }

        private bool m_bReadData = false;
        public bool ReadErpData()
        {
            if (m_bReadData == true)
                return true;

            //직책
            if (!ReadDuty())
                return false;

            //부서
            if (!ReadTeam())
                return false;

            if (!ReadCompany())
                return false;

            if (!ReadDepartment())
                return false;

            if (!ReadWorkers())
                return false;

            if (!ReadCarType())
                return false;

            if (!ReadCarStandard())
                return false;

            if (!ReadCars())
                return false;

            if (!ReadEquipName())
                return false;

            if (!ReadEquipStandard())
                return false;

            if (!ReadEquips())
                return false;

            m_bReadData = true;
            return true;
        }                

        /// <summary>
        /// HSMS 의 입력된 Prefiex의 field링크를 읽어서 sql 쿼리로 만들어주는 함수
        /// </summary>
        /// <param name="szPrefix">ItemName prefiex</param>
        /// <returns>sql query</returns>
        private string ReadFieldLink(string szPrefix)
        {
            SqlConnection connect = m_ConnectionHSMS.Connect();
            string szSQL = string.Format("SELECT ID, ItemName, ItemValue, ItemType,Description FROM HSMS.dbo.FieldLink where ItemName like '{0}%'", szPrefix);

            ArrayList arLinkFields = new ArrayList();
            Dictionary<string, string> dicTableName = new Dictionary<string, string>();
            Dictionary<string, string> dicAliasName = new Dictionary<string, string>();

            SqlDataReader rd = m_ConnectionHSMS.ExecuteReader(szSQL, connect);
            while (rd.Read())
            {
                int nID = (int)rd[0];
                string strName = rd[1].ToString().TrimEnd();
                string strVAlue = rd[2].ToString().TrimEnd();
                int nType = Convert.ToInt32(rd[3].ToString().TrimEnd());
                string strDesc = rd[4].ToString().TrimEnd();

                LinkField field = new LinkField();
                field.FieldType = nType;
                field.FieldValue = strVAlue;

                arLinkFields.Add(field);

                dicTableName[field.TableName] = field.DBName;

            }

            char c = 'b';
            foreach (KeyValuePair<string, string> pair in dicTableName)
            {
                dicAliasName[pair.Key] = (c).ToString();
                c = (char)(c + 1);
            }

            string szSchema = "dbo";
            bool m_bFirst = true;
            StringBuilder sb = new StringBuilder();
            sb.Append("select ");



            foreach (LinkField field in arLinkFields)
            {
                if (m_bFirst == true)
                {
                    m_bFirst = false;
                }
                else
                {
                    sb.Append(", ");
                }
                string szAlias = dicAliasName[field.TableName];
                sb.Append(szAlias);
                sb.Append(".");
                sb.Append(field.FieldName);
            }

            sb.Append(" from ");
            m_bFirst = true;
            foreach (KeyValuePair<string, string> pair in dicTableName)
            {
                if (m_bFirst == true)
                {
                    m_bFirst = false;
                }
                else
                {
                    sb.Append(", ");
                }
                string szAlias = dicAliasName[pair.Key];
                string szTable = pair.Key;
                string szDB = pair.Value;
                sb.Append(szDB);
                sb.Append(".");
                sb.Append(szSchema);
                sb.Append(".");
                sb.Append(szTable);
                sb.Append(" as ");
                sb.Append(szAlias);
            }

            rd.Close();
            connect.Close();

            return sb.ToString();
        } 

        private bool ReadDepartment()
        {
            SqlConnection connect = m_Connectionhpublic00.Connect();
            if (m_Connectionhpublic00 == null)
                return false;

            m_dicDepart.Clear();

            string szSQL = ReadFieldLink("Department");
            SqlDataReader rd = m_Connectionhpublic00.ExecuteReader(szSQL,connect);
            while (rd.Read())
            {
                string strCode = rd[0].ToString().TrimEnd();
                string strName = rd[1].ToString().TrimEnd();

                DataDepartment depart = new DataDepartment();
                depart.Code = strCode;
                depart.Name = strName;
                
                m_dicDepart[strCode] = depart;                
            }
            rd.Close();
            connect.Close();

            return true;
        }


        private bool ReadCompany()
        {
            SqlConnection connect = m_Connectionhpublic00.Connect();
            if (m_Connectionhpublic00 == null)
                return false;

            m_dicComapny.Clear();

            string szSQL = ReadFieldLink("Company");

            SqlDataReader rd = m_Connectionhpublic00.ExecuteReader(szSQL,connect);
            while (rd.Read())
            {
                string strCompanyID = rd[1].ToString().TrimEnd();
                string strCompanyName = rd[0].ToString().TrimEnd();               

                DataCompany company = new DataCompany();
                company.CompanyID = strCompanyID;
                company.CompanyName = strCompanyName;               

                m_dicComapny[strCompanyID] = company;
            }

            rd.Close();
            connect.Close();
            return true;
        }

        //사업부 - > 작업자
        private bool ReadWorkers()
        {
            SqlConnection connect = m_Connectionhpublic00.Connect();
            if (m_Connectionhpublic00 == null)
                return false;

            m_dicCompanyWorkers.Clear();

            string strSQL = ReadFieldLink("Worker");
            SqlDataReader rd = m_Connectionhpublic00.ExecuteReader(strSQL,connect);
            while (rd.Read())
            {
                string strWorkerCompanyID = rd[0].ToString().TrimEnd();
                string strWorkerName = rd[1].ToString().TrimEnd();
                string strCompanyCode = rd[2].ToString().TrimEnd();
                string strTeamCode = rd[3].ToString().TrimEnd();
                string strJobPositionCode = rd[4].ToString().TrimEnd();
                string strSensorID = rd[5].ToString().TrimEnd();
                string strOfficePhoneNumber = rd[6].ToString().TrimEnd();
                string strMobilePhoneNumber = rd[7].ToString().TrimEnd();

                DataWorker worker = new DataWorker();                
                worker.MemberID = strWorkerCompanyID;
                worker.Name  = strWorkerName;
                worker.CompanyCode = strCompanyCode;
                worker.TeamCode = strTeamCode;

                DataCompany company = null;
                if (m_dicComapny.ContainsKey(strCompanyCode))
                {
                    company = m_dicComapny[strCompanyCode];
                    worker.Company = company;
                }
                DataDepartment team = null;
                if (m_dicDepart.ContainsKey(strTeamCode))
                {
                    team = m_dicDepart[strTeamCode];
                    worker.Team = team; 
                }               

                worker.JobPositionCode = strJobPositionCode;
                worker.Sensor = strSensorID.TrimEnd(new char[] { ' ', '\t', '\r', '\n' });
                worker.OfficePhoneNumber = strOfficePhoneNumber;
                worker.MobilePhoneNumber = strMobilePhoneNumber;

                if (m_dicDutyCodeName.ContainsKey(strJobPositionCode))
                {
                    JobPosition jobPosition = m_dicDutyCodeName[strJobPositionCode];
                    worker.JobPosition = jobPosition;
                }

                if (m_dicDepart.ContainsKey(strTeamCode))
                {
                    DataDepartment department = m_dicDepart[strTeamCode];
                    department.Name = department.Name;
                }

                if (company != null && team != null)
                {
                    if (!company.Departments.Contains(team))
                        company.Departments.Add(team);
                }
                
                if( team != null)
                {
                    team.Workers.Add(worker);
                }

                m_dicCompanyWorkers[strWorkerCompanyID] = worker;
            }

            rd.Close();
            connect.Close();
            return true; 
        }

        //입력문자열이 숫자인지 문자인지 판단(숫자면true, 문자열이나 공백이면 false를반환)
        private bool CheckisNum(string value)
        {
            string check = value.Trim();
            bool returnVal = false;
            int dotCount = 0;
            for (int i = 0; i < check.Length; i++)
            {
                if (System.Char.IsNumber(check, i))
                    returnVal = true;
                else
                {
                    if (check.Substring(i, 1) == ".")
                    {
                        returnVal = true;
                        dotCount++;
                    }
                    else
                    {
                        returnVal = false;
                        break;
                    } 
                }
            }
            if (dotCount > 1)
            {
                returnVal = false;
            }
            return returnVal;
        }

        //차량
        private bool ReadCars()
        {
            bool istest = CheckisNum("");
            SqlConnection Connect = m_Connectionhwinmm.Connect();
            if (m_Connectionhwinmm == null)
                return false;
            
            string strSQL = ReadFieldLink("Car");

            m_dicCompanyCars.Clear();

            SqlDataReader rd = m_Connectionhwinmm.ExecuteReader(strSQL,Connect);
            while (rd.Read())
            {
                DataCar car = new DataCar();
   
                string strCarCode = rd[0].ToString().TrimEnd();
                string strCarName = rd[1].ToString().TrimEnd();
                string strCarTeamCode = rd[2].ToString().TrimEnd();
                string strCarMaker = rd[3].ToString().TrimEnd();

                if (CheckisNum(rd[4].ToString().TrimEnd()))
                {
                    car.Length = Convert.ToInt32(rd[4].ToString().TrimEnd());
                }
                else
                    continue;


                if (CheckisNum(rd[5].ToString().TrimEnd()))
                {
                    car.Width = Convert.ToInt32(rd[5].ToString().TrimEnd());
                }
                else
                    continue;

                string strtest = rd[6].ToString();
                if (CheckisNum(rd[6].ToString().TrimEnd()))
                {
                    car.Height = Convert.ToInt32(rd[6].ToString().TrimEnd());
                }
                else
                    continue;   

                string strCarStandard = rd[7].ToString().TrimEnd();
                string strCarType = rd[8].ToString().TrimEnd();
                string strCarNumber = rd[9].ToString().TrimEnd();
                string strCarSensor = rd[10].ToString().TrimEnd();
                string strCarUse = rd[11].ToString().TrimEnd();
                string strCarDriveName = rd[12].ToString().TrimEnd();
                
                car.Code = strCarCode;
                car.Name = strCarName;
                car.TeamCode = strCarTeamCode;
                car.MakerCompany = strCarMaker;
                car.Standard = strCarStandard;
                car.Type = strCarType;
                car.Number = strCarNumber;
                car.Sensor = strCarSensor;
                car.Use = strCarUse;
                car.DriverName = strCarDriveName;
                
                DataCarType cartype = null;
                if (m_dicCarTypes.ContainsKey(strCarType))
                {
                    cartype = m_dicCarTypes[strCarType];
                    car.CarType = cartype;
                }
                else
                {
                    // CAR Type 이 삭제됨
                    // 해당 CarType이 포함된 데이터 삭제 추가
                }
                
                DataCarStandard carStandard = null;
                if (m_dicCarStandards.ContainsKey(strCarCode))
                {
                    carStandard = m_dicCarStandards[strCarCode];
                    car.CarStandard = carStandard;
                }
                else
                {
                    // Car Code 가 삭제됨
                    // 삭제될 Car 데이터 추가
                }

                if (carStandard != null && cartype != null)
                {
                    if (!cartype.CarStandards.Contains(carStandard))
                        cartype.CarStandards.Add(carStandard);
                }

                if (carStandard != null)
                {
                    carStandard.Cars.Add(car);
                }

                m_dicCompanyCars[strCarCode] = car;

                m_dicSensorCar[strCarSensor] = car;
            }

            rd.Close();
            Connect.Close();
            return true;
        }

        //차종
        private bool ReadCarType()
        {
            if (m_Connectionhwinmm == null)
                return false;

            SqlConnection connect = m_Connectionhwinmm.Connect();

            m_dicCarTypes.Clear();

            string szSQL = ReadFieldLink("TypeCar");

            SqlDataReader rd = m_Connectionhwinmm.ExecuteReader(szSQL,connect);
            while (rd.Read())
            {
                string strTypeCode = rd[0].ToString().TrimEnd();
                string strTypeName = rd[1].ToString().TrimEnd();
                strTypeCode = strTypeCode.TrimEnd();

                DataCarType carType = new DataCarType();
                carType.Code = strTypeCode;
                carType.Name = strTypeName;

                m_dicCarTypes[strTypeCode] = carType;
            }
            rd.Close();
            connect.Close();
            return true;
        }

        //차량규격
        private bool ReadCarStandard()
        {
            if (m_Connectionhwinmm == null)
                return false;

            SqlConnection connect = m_Connectionhwinmm.Connect();

            m_dicCarStandards.Clear();

            string szSQL = ReadFieldLink("StandardCar");

            SqlDataReader rd = m_Connectionhwinmm.ExecuteReader(szSQL,connect);
            while (rd.Read())
            {
                string strCarID = rd[0].ToString().TrimEnd();
                string strCarStandardName = rd[1].ToString().TrimEnd();

                DataCarStandard carStandard = new DataCarStandard();
                carStandard.CarID = strCarID;
                carStandard.Name = strCarStandardName;

                m_dicCarStandards[strCarID] = carStandard;
            }
            rd.Close();
            connect.Close();
            return true;
        }
        
        //설비
        private bool ReadEquips()
        {
            SqlConnection Connect = m_Connectionhwinmm.Connect();
            if (m_Connectionhwinmm == null)
                return false;

            m_dicEquips.Clear();

            string strSQL = ReadFieldLink("Equip");
            SqlDataReader rd = m_Connectionhwinmm.ExecuteReader(strSQL, Connect);
            while (rd.Read())
            {
                string strEquipCode = rd[0].ToString().TrimEnd();
                string strEquipName = rd[1].ToString().TrimEnd();
                string strEquipStandard = rd[2].ToString().TrimEnd();
                string strEquipNumber = rd[3].ToString().TrimEnd();
                string strEquipMaker = rd[4].ToString().TrimEnd();
                string strEquipSesnsor = rd[5].ToString().TrimEnd();
                string strEquipDriverName = rd[6].ToString().TrimEnd();
                string strEquipType = rd[7].ToString().TrimEnd();

                DataEquip equip = new DataEquip();

                equip.Code = strEquipCode;
                equip.Name = strEquipName;
                equip.Standard = strEquipStandard;
                equip.Number = strEquipNumber;
                equip.Maker = strEquipMaker;
                equip.Sensor = strEquipSesnsor;
                equip.DriverName = strEquipDriverName;
                equip.TypeName = strEquipType;
                equip.EquipmentGroup = EquipmentGroup.DefaultEquipmentGroup;

                DataEquipName equipName = null;
                if (m_dicEquipNames.ContainsKey(strEquipCode))
                {
                    equipName = m_dicEquipNames[strEquipCode];
                    equip.EquipName = equipName;
                }

                DataEquipStandard equipStandard = null;
                if (m_dicEquipStandards.ContainsKey(strEquipCode))
                {
                    equipStandard = m_dicEquipStandards[strEquipCode];
                    equip.EquipStandard = equipStandard;
                }

                if (equipName != null && equipStandard != null)
                {
                    if (!equipName.EquipStandards.Contains(equipStandard))
                        equipName.EquipStandards.Add(equipStandard);
                }

                if (equipStandard != null)
                {
                    equipStandard.Equips.Add(equip);
                }

                m_dicEquips[strEquipCode] = equip;

                m_dicSensorEquip[strEquipSesnsor] = equip;
            }

            rd.Close();
            Connect.Close();
            return true;
        }

        //설비명
        private bool ReadEquipName()
        {
            if (m_Connectionhwinmm == null)
                return false;

            SqlConnection connect = m_Connectionhwinmm.Connect();

            m_dicCarStandards.Clear();

            string szSQL = ReadFieldLink("Equip");

            SqlDataReader rd = m_Connectionhwinmm.ExecuteReader(szSQL, connect);
            while (rd.Read())
            {
                string strEquipID = rd[0].ToString().TrimEnd();
                string strEquipName = rd[1].ToString().TrimEnd();

                DataEquipName equipName = new DataEquipName();
                equipName.ID = strEquipID;
                equipName.Name = strEquipName;

                m_dicEquipNames[strEquipID] = equipName;

                //m_dicEquipName.Add(equipName);
            }
            rd.Close();
            connect.Close();
            return true;
        }

        //설비규격
        private bool ReadEquipStandard()
        {
            if (m_Connectionhwinmm == null)
                return false;

            SqlConnection connect = m_Connectionhwinmm.Connect();

            m_dicCarStandards.Clear();

            string szSQL = ReadFieldLink("Equip");

            SqlDataReader rd = m_Connectionhwinmm.ExecuteReader(szSQL, connect);
            while (rd.Read())
            {
                string strEquipID = rd[0].ToString().TrimEnd();
                string strEquipName = rd[2].ToString().TrimEnd();

                DataEquipStandard equipStandard = new DataEquipStandard();
                equipStandard.ID = strEquipID;
                equipStandard.Name = strEquipName;

                m_dicEquipStandards[strEquipID] = equipStandard;

                //m_arrEquipStandards.Add(equipStandard);
            }
            rd.Close();
            connect.Close();
            return true;
        }

        //직책
        private bool ReadDuty()
        {
            if (m_Connectionhwinmm == null)
                return false;

            SqlConnection connect = m_Connectionhwinmm.Connect();

            m_dicDutyCodeName.Clear();

            string szSQL = ReadFieldLink("Duty");

            SqlDataReader rd = m_Connectionhwinmm.ExecuteReader(szSQL, connect);
            while (rd.Read())
            {
                string strDutyCode = rd[0].ToString().TrimEnd();
                string strDutyName = rd[1].ToString().TrimEnd();
                //strTypeCode = strTypeCode.TrimEnd();

                JobPosition jobPosition = new JobPosition();
                jobPosition.Code = strDutyCode;
                jobPosition.Name = strDutyName;

                m_dicDutyCodeName[strDutyCode] = jobPosition;
            }
            rd.Close();
            connect.Close();
            return true;
        }

        //부서
        private bool ReadTeam()
        {
            if (m_Connectionhwinmm == null)
                return false;

            SqlConnection connect = m_Connectionhwinmm.Connect();

            m_dicDepart.Clear();

            string szSQL = ReadFieldLink("Team");

            SqlDataReader rd = m_Connectionhwinmm.ExecuteReader(szSQL, connect);
            while (rd.Read())
            {
                string strTeamCode = rd[0].ToString().TrimEnd();
                string strTeamName = rd[1].ToString().TrimEnd();
                //strTypeCode = strTypeCode.TrimEnd();

                DataDepartment department = new DataDepartment();
                department.Code = strTeamCode;
                department.Name = strTeamName;

                m_dicDepart[strTeamCode] = department;
            }
            rd.Close();
            connect.Close();
            return true;
        }
    }
}
