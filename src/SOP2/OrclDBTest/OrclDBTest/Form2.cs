using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Data.OleDb;
using System.IO;

namespace OrclDBTest
{
    public partial class Form2 : Form
    {
        private OleDbConnection m_Connection;
        private OleDbCommand m_Command;

        ArrayList m_arrInsa = new ArrayList();
        ArrayList m_arrJosjik = new ArrayList();

        public Form2()
        {
            InitializeComponent();
        }

        private void LoadInsa()
        {
            m_Connection = new OleDbConnection();

            m_Connection.ConnectionString = string.Format("Provider=OraOLEDB.Oracle;USER ID={0};PASSWORD={1};DATA SOURCE={2};OLEDB.NET=True;",
                                                             "insa_user",
                                                             "insa123",
                                                             "ORA8");

            m_Command = new OleDbCommand("SELECT * FROM select * from webadm.view_insa_tbl_sec", m_Connection);

            try
            {
                m_Connection.Open();

                OleDbDataReader reader = m_Command.ExecuteReader();
                while (reader.Read())
                {
                    table_Insa dataNew = new table_Insa();
                    dataNew.EMPNO = (string)reader.GetValue(0);
                    dataNew.DEPTNO = (string)reader.GetValue(1);
                    dataNew.LEVELNO = (string)reader.GetValue(2);
                    dataNew.CLASSNO = (string)reader.GetValue(3);
                    dataNew.NANNO = (string)reader.GetValue(4);
                    dataNew.PASSWD = (string)reader.GetValue(5);
                    dataNew.MAILNO = (string)reader.GetValue(6);
                    dataNew.NAME = (string)reader.GetValue(7);
                    dataNew.TELNO = (string)reader.GetValue(8);
                    dataNew.HOSTNAME = (string)reader.GetValue(9);
                    dataNew.NOTREMOVE = (int)reader.GetValue(10);
                    dataNew.EMP_ORDER = (int)reader.GetValue(11);
                    dataNew.DOWNLOAD = (int)reader.GetValue(12);
                    dataNew.LEVELDATE = (string)reader.GetValue(13);
                    dataNew.TITLE = (string)reader.GetValue(14);
                    dataNew.EXIST = (int)reader.GetValue(15);
                    dataNew.IPADDR = (string)reader.GetValue(16);
                    dataNew.M_COUNT = (string)reader.GetValue(17);
                    dataNew.MOBILE_PHN = (string)reader.GetValue(18);
                    dataNew.JANG_YN = (string)reader.GetValue(19);
                    dataNew.FIRST_ORG = (string)reader.GetValue(20);

                    m_arrInsa.Add(dataNew);
                }

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                m_Connection.Close();
            }
        }

        private void LoadJosjik()
        {
            m_Connection = new OleDbConnection();

            m_Connection.ConnectionString = string.Format("Provider=OraOLEDB.Oracle;USER ID={0};PASSWORD={1};DATA SOURCE={2};OLEDB.NET=True;",
                                                             "insa_user",
                                                             "insa123",
                                                             "ORA8");

            m_Command = new OleDbCommand("SELECT * FROM select * from webadm.view_jojik_tbl", m_Connection);

            try
            {
                m_Connection.Open();

                OleDbDataReader reader = m_Command.ExecuteReader();
                while (reader.Read())
                {
                    table_Josjik dataNew = new table_Josjik();
                    dataNew.DEPT_CODE = (string)reader.GetValue(0);
                    dataNew.UP_CODE = (string)reader.GetValue(1);
                    dataNew.DEPT_NAME = (string)reader.GetValue(2);
                    dataNew.DEPT_ORDER = (int)reader.GetValue(3);
                    dataNew.MINLEVELNO = (string)reader.GetValue(4);
                    dataNew.MANAGER = (string)reader.GetValue(5);
                    dataNew.HOSTNAME = (string)reader.GetValue(6);
                    dataNew.ALIAS_NAME = (string)reader.GetValue(7);
                    dataNew.END_YMD = (string)reader.GetValue(8);
                    
                    m_arrJosjik.Add(dataNew);
                }

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                m_Connection.Close();
            }
        }

        private void AddGrid_Insa()
        {
            foreach (table_Insa member in m_arrInsa)
            {
                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCell cell = null;

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.EMPNO;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.DEPTNO;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.LEVELNO;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.CLASSNO;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.NANNO;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.PASSWD;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.MAILNO;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.NAME;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.TELNO;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.HOSTNAME;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.NOTREMOVE;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.EMP_ORDER;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.DOWNLOAD;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.LEVELDATE;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.TITLE;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.EXIST;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.IPADDR;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.M_COUNT;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.MOBILE_PHN;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.JANG_YN;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.FIRST_ORG;
                gridRow.Cells.Add(cell);

            }
        }

        private void AddGrid_Josjik()
        {
            foreach (table_Josjik member in m_arrJosjik)
            {
                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCell cell = null;

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.DEPT_CODE;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.UP_CODE;
                gridRow.Cells.Add(cell); 
                
                cell = new DataGridViewTextBoxCell();
                cell.Value = member.DEPT_NAME;
                gridRow.Cells.Add(cell);
                
                cell = new DataGridViewTextBoxCell();
                cell.Value = member.DEPT_ORDER;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.MINLEVELNO;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.MANAGER;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.HOSTNAME;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.ALIAS_NAME;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.END_YMD;
                gridRow.Cells.Add(cell);

                dataGridView1.Rows.Add(gridRow);
            }

        }

        public T GetField<T>(object dataSrc, T dataDefault)
        {
            T result;

            try
            {
                result = (T)dataSrc;
            }
            catch (Exception)
            {
                result = dataDefault;
            }

            return result;
        }

        private void btnInsa_Click(object sender, EventArgs e)
        {
            FileDialog dlg = new SaveFileDialog();
            dlg.FileName = "insa.txt";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = File.CreateText(dlg.FileName);

                string strInsa = "";
                foreach (table_Insa member in m_arrInsa)
                {
                    strInsa = member.EMPNO + "," +
                            member.DEPTNO + "," +
                            member.LEVELNO + "," +
                            member.CLASSNO + "," +
                            member.NANNO + "," +
                            member.PASSWD + "," +
                            member.MAILNO + "," +
                            member.NAME + "," +
                            member.TELNO + "," +
                            member.HOSTNAME + "," +
                            member.NOTREMOVE + "," +
                            member.EMP_ORDER + "," +
                            member.DOWNLOAD + "," +
                            member.LEVELDATE + "," +
                            member.TITLE + "," +
                            member.EXIST + "," +
                            member.IPADDR + "," +
                            member.M_COUNT + "," +
                            member.MOBILE_PHN + "," +
                            member.JANG_YN + "," +
                            member.FIRST_ORG + "\n";

                    sw.WriteLine(strInsa);
                    strInsa = "";
                }

                sw.Close();
            }
        }

        private void btnJosjik_Click(object sender, EventArgs e)
        {
            FileDialog dlg = new SaveFileDialog();
            dlg.FileName = "josjik.txt";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = File.CreateText(dlg.FileName);

                string strJosjik = "";
                foreach (table_Josjik member in m_arrJosjik)
                {
                    strJosjik = member.DEPT_CODE + "," +
                                member.UP_CODE + "," +
                                member.DEPT_NAME + "," +
                                member.DEPT_ORDER + "," +
                                member.MINLEVELNO + "," +
                                member.MANAGER + "," +
                                member.HOSTNAME + "," +
                                member.ALIAS_NAME + "," +
                                member.END_YMD + "\n";

                    sw.WriteLine(strJosjik);
                    strJosjik = "";
                }

                sw.Close();
            }
        }

        private void test()
        {
            table_Josjik member = new table_Josjik();
            member.DEPT_CODE = "DEPT_CODE1";
            member.UP_CODE = "UP_CODE1";
            member.DEPT_NAME = "DEPT_NAME1";
            member.DEPT_ORDER = 1;
            member.MINLEVELNO = "MINLEVELNO1";
            member.MANAGER = "MANAGER1";
            member.HOSTNAME = "HOSTNAME1";
            member.ALIAS_NAME = "ALIAS_NAME1";
            member.END_YMD = "END_YMD1";

            m_arrJosjik.Add(member);

            member = new table_Josjik();
            member.DEPT_CODE = "DEPT_CODE2";
            member.UP_CODE = "UP_CODE2";
            member.DEPT_NAME = "DEPT_NAME2";
            member.DEPT_ORDER = 2;
            member.MINLEVELNO = "MINLEVELNO2";
            member.MANAGER = "MANAGER2";
            member.HOSTNAME = "HOSTNAME2";
            member.ALIAS_NAME = "ALIAS_NAME2";
            member.END_YMD = "END_YMD2";

            m_arrJosjik.Add(member);
        }
    }

    public class table_Insa
    {
        private string m_strEMPNO; // --사번
        private string m_strDEPTNO; // --부서코드
        private string m_strLEVELNO; // -- 직급
        private string m_strCLASSNO;
        private string m_strNANNO; // --주민번호
        private string m_strPASSWD; //  -비번
        private string m_strMAILNO; // --메일주소
        private string m_strNAME; // --이름
        private string m_strTELNO; // --전번
        private string m_strHOSTNAME;
        private int m_nNOTREMOVE;
        private int m_nEMP_ORDER;
        private int m_nDOWNLOAD;
        private string m_strLEVELDATE;
        private string m_strTITLE; // --직책
        private int m_nEXIST;
        private string m_strIPADDR;
        private string m_strM_COUNT;
        private string m_strMOBILE_PHN;
        private string m_strJANG_YN; // --부서장여부
        private string m_strFIRST_ORG;

        public string EMPNO
        {
            get { return m_strEMPNO; }
            set { m_strEMPNO = value; }
        }

        public string DEPTNO
        {
            get { return m_strDEPTNO; }
            set { m_strDEPTNO = value; }
        }

        public string LEVELNO
        {
            get { return m_strLEVELNO; }
            set { m_strLEVELNO = value; }
        }

        public string CLASSNO
        {
            get { return m_strCLASSNO; }
            set { m_strCLASSNO = value; }
        }

        public string NANNO
        {
            get { return m_strNANNO; }
            set { m_strNANNO = value; }
        }

        public string PASSWD
        {
            get { return m_strPASSWD; }
            set { m_strPASSWD = value; }
        }

        public string MAILNO
        {
            get { return m_strMAILNO; }
            set { m_strMAILNO = value; }
        }

        public string NAME
        {
            get { return m_strNAME; }
            set { m_strNAME = value; }
        }

        public string TELNO
        {
            get { return m_strTELNO; }
            set { m_strTELNO = value; }
        }

        public string HOSTNAME
        {
            get { return m_strHOSTNAME; }
            set { m_strHOSTNAME = value; }
        }

        public int NOTREMOVE
        {
            get { return m_nNOTREMOVE; }
            set { m_nNOTREMOVE = value; }
        }

        public int EMP_ORDER
        {
            get { return m_nEMP_ORDER; }
            set { m_nEMP_ORDER = value; }
        }

        public int DOWNLOAD
        {
            get { return m_nDOWNLOAD; }
            set { m_nDOWNLOAD = value; }
        }

        public string LEVELDATE
        {
            get { return m_strLEVELDATE; }
            set { m_strLEVELDATE = value; }
        }

        public string TITLE
        {
            get { return m_strTITLE; }
            set { m_strTITLE = value; }
        }

        public int EXIST
        {
            get { return m_nEXIST; }
            set { m_nEXIST = value; }
        }

        public string IPADDR
        {
            get { return m_strIPADDR; }
            set { m_strIPADDR = value; }
        }

        public string M_COUNT
        {
            get { return m_strM_COUNT; }
            set { m_strM_COUNT = value; }
        }

        public string MOBILE_PHN
        {
            get { return m_strMOBILE_PHN; }
            set { m_strMOBILE_PHN = value; }
        }

        public string JANG_YN
        {
            get { return m_strJANG_YN; }
            set { m_strJANG_YN = value; }
        }

        public string FIRST_ORG
        {
            get { return m_strFIRST_ORG; }
            set { m_strFIRST_ORG = value; }
        }
    }

    public class table_Josjik
    {
        private string m_strDEPT_CODE; // --부서코드
        private string m_strUP_CODE; // --상위코드
        private string m_strDEPT_NAME; // --부서명
        private int m_nDEPT_ORDER; // --부서직제
        private string m_strMINLEVELNO;
        private string m_strMANAGER; // --부서장
        private string m_strHOSTNAME;
        private string m_strALIAS_NAME;
        private string m_strEND_YMD; // --부서종료일
        
        public string DEPT_CODE
        {
            get { return m_strDEPT_CODE; }
            set { m_strDEPT_CODE = value; }
        }
        
        public string UP_CODE
        {
            get { return m_strUP_CODE; }
            set { m_strUP_CODE = value; }
        }
        
        public string DEPT_NAME
        {
            get { return m_strDEPT_NAME; }
            set { m_strDEPT_NAME = value; }
        }
        
        public int DEPT_ORDER
        {
            get { return m_nDEPT_ORDER; }
            set { m_nDEPT_ORDER = value; }
        }
        
        public string MINLEVELNO
        {
            get { return m_strMINLEVELNO; }
            set { m_strMINLEVELNO = value; }
        }
        
        public string MANAGER
        {
            get { return m_strMANAGER; }
            set { m_strMANAGER = value; }
        }
        
        public string HOSTNAME
        {
            get { return m_strHOSTNAME; }
            set { m_strHOSTNAME = value; }
        }
        
        public string ALIAS_NAME
        {
            get { return m_strALIAS_NAME; }
            set { m_strALIAS_NAME = value; }
        }
        
        public string END_YMD
        {
            get { return m_strEND_YMD; }
            set { m_strEND_YMD = value; }
        }
    }
}
