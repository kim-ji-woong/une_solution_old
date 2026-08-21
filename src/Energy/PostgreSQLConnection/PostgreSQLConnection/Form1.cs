using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace PostgreSQLConnection
{
    public partial class Form1 : Form
    {
        private string m_strConnstring
        {
            get { return textBox1.Text; }
        }

        private NpgsqlConnection m_connection = null;
        private bool m_isConnected = false;

        private Timer m_timer = null;

        private int m_nLogIndex = 0;

        //private Dictionary<string, Company> m_dicCompanies = new Dictionary<string, Company>();
        //private Dictionary<string, Location> m_dicLocations = new Dictionary<string, Location>();
        //private Dictionary<string, Department> m_dicDepartments = new Dictionary<string, Department>();
        //private Dictionary<string, Worker> m_dicWorkers = new Dictionary<string, Worker>();

        private string m_strInDoorName = "입구";
        private string m_strOutDoorName = "출구";
        private DateTime m_dtPrev = new DateTime();

        public Form1()
        {
            InitializeComponent();

            //HOST=127.0.0.1;PORT=5432;USERNAME=postgres;PASSWORD=9449966Ab;DATABASE=DID
            //Server=192.168.1.11;Port=5432;User Id=postgres;Password=admin1234;Database=kdhc_db;

            m_timer = new Timer();
            m_timer.Interval = 1000;
            m_timer.Tick += M_timer_Tick;
        }

        private void M_timer_Tick(object sender, EventArgs e)
        {
            DisplayLog();
        }

        private void DisplayUser()
        {
            string strQuery = "SELECT id, employee_table_id, employee_user_id, name, company, location, department, position, telephone, hire_date, expiry_date FROM public.tbl_user_info";

            ArrayList arrResult = GetResultData(strQuery);
            if (arrResult == null || arrResult.Count == 0)
                return;

            WriteLog("DisplayUser()");
            DateTime dtNow = DateTime.Now;
            string strToday = string.Format("{0}-{1:00}-{2:00}", dtNow.Year, dtNow.Month, dtNow.Day);

            Dictionary<string, Company> dicPrevCompanies = m_dicCompanies;
            Dictionary<string, Location> dicPrevLocations = m_dicLocations;
            Dictionary<string, Department> dicPrevDepartments = m_dicDepartments;
            Dictionary<string, Worker> dicPrevWorkers = m_dicWorkers;

            Dictionary<string, Company> dicCompanies = new Dictionary<string, Company>();
            Dictionary<string, Location> dicLocations = new Dictionary<string, Location>();
            Dictionary<string, Department> dicDepartments = new Dictionary<string, Department>();
            Dictionary<string, Worker> dicWorkers = new Dictionary<string, Worker>();

            Company company = null;
            Location location = null;
            Department department = null;

            for (int i = 0; i < arrResult.Count; i+=11)
            {
                string id = arrResult[i].ToString();
                string employee_table_id = arrResult[i + 1].ToString();
                string employee_user_id = arrResult[i + 2].ToString();
                string name = arrResult[i + 3].ToString();
                string strCompany = arrResult[i + 4].ToString();
                string strLocation = arrResult[i + 5].ToString();
                string strDepartment = arrResult[i + 6].ToString();
                string position = arrResult[i + 7].ToString();
                string telephone = arrResult[i + 8].ToString();
                string hire_date = arrResult[i + 9].ToString();
                string expiry_date = arrResult[i + 10].ToString();

                string txt = string.Format("id : {0}, employee_table_id : {1}, employee_user_id : {2}, name : {3}, company : {4}, location : {5}, department : {6}, position : {7}, telephone : {8}, hire_date : {9}, expiry_date : {10}"
                    , id, employee_table_id, employee_user_id, name, strCompany, strLocation, strDepartment, position, telephone, hire_date, expiry_date);

                if (dicCompanies.TryGetValue(strCompany, out company) == false)
                {
                    company = new Company();
                    company.Name = strCompany;
                    dicCompanies[strCompany] = company;
                }

                if (dicLocations.TryGetValue(strLocation, out location) == false)
                {
                    location = new Location();
                    location.Name = strLocation;
                    dicLocations[strLocation] = location;
                }

                if (dicDepartments.TryGetValue(strDepartment, out department) == false)
                {
                    department = new Department();
                    department.Name = strDepartment;
                    dicDepartments[strDepartment] = department;
                }

                int nID;

                if (int.TryParse(id, out nID))
                {
                    Worker worker = new Worker();

                    worker.Company = company;
                    worker.Location = location;
                    worker.Department = department;
                    worker.PhoneNumber = telephone;
                    worker.Name = name;
                    worker.ID = nID;
                    worker.CompanyID = employee_user_id;

                    dicWorkers[employee_user_id] = worker;
                }

                WriteLog(txt);
            }

            Worker _worker;

            // 직원별 출입정보를 입력한다.
            foreach (KeyValuePair<string, Worker> pair in dicWorkers)
            {
                if (dicPrevWorkers.TryGetValue(pair.Key, out _worker))
                {
                    pair.Value.InWork = _worker.InWork;                  
                }
            }

            m_dicCompanies = dicCompanies;
            m_dicLocations = dicLocations;
            m_dicDepartments = dicDepartments;
            m_dicWorkers = dicWorkers;

            dicPrevWorkers.Clear();
            dicPrevDepartments.Clear();
            dicPrevLocations.Clear();
            dicPrevWorkers.Clear();
        }

        private void DisplayLog()
        {
            string strQuery = "SELECT id, doorac_table_id, ac_result, door_group, door_name, event_time, name, employee_user_id, company, location, department, position FROM public.tbl_access_log where id > " + m_nLogIndex;
            strQuery += " and event_time > '" + GetTimeString(m_dtPrev) + "' order by id, event_time";

            ArrayList arrResult = GetResultData(strQuery);
            if (arrResult == null || arrResult.Count == 0)
                return;

            WriteLog("DisplayLog()");
            Worker worker;

            for (int i = 0; i < arrResult.Count; i += 12)
            {
                string id = arrResult[i].ToString();
                string doorac_table_id = arrResult[i + 1].ToString();
                string ac_result = arrResult[i + 2].ToString();
                string door_group = arrResult[i + 3].ToString();
                string door_name = arrResult[i + 4].ToString();
                string event_time = arrResult[i + 5].ToString();
                string name = arrResult[i + 6].ToString();
                string employee_user_id = arrResult[i + 7].ToString();
                string company = arrResult[i + 8].ToString();
                string location = arrResult[i + 9].ToString();
                string department = arrResult[i + 10].ToString();
                string position = arrResult[i + 11].ToString();

                string txt = string.Format("id : {0}, doorac_table_id : {1}, ac_result : {2}, door_group : {3}, door_name : {4}, event_time : {5}, name : {6}, employee_user_id : {7}, company : {8}, location : {9}, department : {10}, position : {11}"
                    , id, doorac_table_id, ac_result, door_group, door_name, event_time, name, employee_user_id, company, location, department, position);

                WriteLog(txt);

                m_nLogIndex = Convert.ToInt32(id);

                if (m_dicWorkers.TryGetValue(employee_user_id, out worker))
                {
                    if (door_name == m_strInDoorName)
                        worker.InWork = true;
                    else if (door_name == m_strOutDoorName)
                        worker.InWork = false;
                }

                try
                {
                    DateTime time = Convert.ToDateTime(event_time);
                    m_dtPrev = time;
                }
                catch (Exception)
                {
                }
            }
        }

        private string GetTimeString(DateTime timeStamp)
        {
            string strTime = string.Format("{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", timeStamp.Year, timeStamp.Month, timeStamp.Day, timeStamp.Hour, timeStamp.Minute, timeStamp.Second);
            return strTime;
        }

        private void WriteLog(string txt)
        {
            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string path = Application.StartupPath + "\\log.txt";
            using (StreamWriter sw = new StreamWriter(path, true, Encoding.UTF8))
            {
                sw.WriteLine("[" + now + "] " + txt);
                textBox2.Text += "[" + now + "] " + txt + "\r\n";
            }
        }

        public bool Connect()
        {
            try
            {
                NpgsqlConnection connection = new NpgsqlConnection(m_strConnstring);
                connection.Open();

                if (connection.State == System.Data.ConnectionState.Open)
                {
                    m_connection = connection;
                    label1.Text = "DB 접속\r\n" + m_strConnstring;
                    label1.ForeColor = Color.Green;
                }
                else
                {
                    label1.Text = "DB 접속에 실패하였습니다.\r\n" + m_strConnstring;
                    label1.ForeColor = Color.Red;
                    return false;
                }
            }
            catch (Exception)
            {
                m_connection = null;
                label1.Text = "DB 접속에 실패하였습니다.\r\n" + m_strConnstring;
                label1.ForeColor = Color.Red;
                return false;
            }

            m_isConnected = true;
            return true;
        }

        public ArrayList GetResultData(string strSQL)
        {
            return RunQuery(strSQL);
        }

        private ArrayList RunQuery(string strSQL)
        {
            if (m_connection == null || m_connection.State != System.Data.ConnectionState.Open)
            {
                label1.Text = "DB와의 연결이 끊어졌습니다.";
                return null;
            }

            ArrayList results = null;

            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand(strSQL, m_connection);
                NpgsqlDataReader reader = cmd.ExecuteReader();

                ArrayList datas = new ArrayList();
                
                int nColumnCount = reader.FieldCount;

                while (reader.Read())
                {
                    for (int i = 0; i < nColumnCount; i++)
                    {
                        if (reader.IsDBNull(i))
                            AddNullData(datas);
                        else
                            AddData(datas, reader.GetValue(i));
                    }
                }

                reader.Close();
                return datas;
            }
            catch (Exception e)
            {
                label1.Text = e.Message;
                return null;
            }

            //return results;
        }

        private void AddNullData(ArrayList datas)
        {
            datas.Add("~");
        }

        private void AddData(ArrayList datas, object data)
        {
            datas.Add(data.ToString());
        }
        
        private void btnConn_Click(object sender, EventArgs e)
        {
            if (m_timer.Enabled)
                m_timer.Stop();

            bool bConn = Connect();
            if (!bConn)
            {
                MessageBox.Show("연결 실패");
                return;
            }

            DisplayUser();
            m_timer.Start();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            StreamWriter writer = new StreamWriter("MemberInfo.txt", false, Encoding.UTF8);

            // 루프 도중 값이 바뀔수 있으니 미리 List로 변환해서 값을 받아둔다.
            List<Company> companies = m_dicCompanies.Values.ToList();

            foreach (Company company in companies)
            {
                writer.WriteLine("[" + company.Name + "]");

                foreach (Worker worker in company.Workers)
                {
                    string strWorker = string.Format("작업장 : {0}, 공종 : {1}, 사번 : {2}, 이름 : {3}, 출근상태 : {4}",
                        worker.Location.Name, worker.Department.Name, worker.CompanyID, worker.Name, worker.InWork);
                    writer.WriteLine(strWorker);
                }
            }

            writer.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ArrayList arr = GetResultData("select coalesce(max(id) + 1,1) from public.tbl_access_log");
            int nID = Convert.ToInt32(arr[0]);

            string strNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("select employee_user_id from public.tbl_user_info where employee_user_id = '{0}'", textBox3.Text);
            arr = GetResultData(sb.ToString());
            
            if (arr.Count == 0)
            {
                arr = GetResultData("select coalesce(max(id) + 1,1) from public.tbl_user_info");
                int nID2 = Convert.ToInt32(arr[0]);

                sb = new StringBuilder();
                sb.Append("INSERT INTO public.tbl_user_info(");
                sb.Append("id, employee_table_id, employee_user_id, name, company, location, department, position, telephone, hire_date, expiry_date)");
                sb.AppendFormat("VALUES({0}, {1}, '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');"
                    , nID2, nID2, textBox3.Text, "이수현" + textBox3.Text, "유엔이" + textBox3.Text
                    , "소속" + textBox3.Text, "부서" + textBox3.Text, "직위" + textBox3.Text, "010-4111-1111", "2019-11-20", "3000-01-01");
                
                arr = GetResultData(sb.ToString()); 
            }

            sb = new StringBuilder();
            sb.Append("INSERT INTO public.tbl_access_log(id, doorac_table_id, ac_result, door_group, door_name, event_time,");
            sb.Append("name, employee_user_id, company, location, department, position)");
            sb.AppendFormat("VALUES({0}, {1}, {2}, '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');", nID, 1, 0, "테스트그룹1", "출근기", strNow
                , "이수현" + textBox3.Text, textBox3.Text, "유엔이" + textBox3.Text, "소속" + textBox3.Text, "부서" + textBox3.Text, "직위" + textBox3.Text);
            arr = GetResultData(sb.ToString());
    }

        private void button2_Click(object sender, EventArgs e)
        {
            ArrayList arr = GetResultData("select coalesce(max(id) + 1,1) from public.tbl_access_log");
            int nID = Convert.ToInt32(arr[0]);

            string strNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.tbl_access_log(id, doorac_table_id, ac_result, door_group, door_name, event_time,");
            sb.Append("name, employee_user_id, company, location, department, position)");
            sb.AppendFormat("VALUES({0}, {1}, {2}, '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');", nID, 1, 0, "테스트그룹1", "퇴근기", strNow
                , "이수현" + textBox3.Text, textBox3.Text, "유엔이" + textBox3.Text, "소속" + textBox3.Text, "부서" + textBox3.Text, "직위" + textBox3.Text);

            ArrayList arr2 = GetResultData(sb.ToString());
        }
    }
}
