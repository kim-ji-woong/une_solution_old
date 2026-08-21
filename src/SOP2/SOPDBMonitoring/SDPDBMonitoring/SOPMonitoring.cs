using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace SOPDBMonitoring
{
    public partial class SOPMonitoring : Form
    {
        private delegate void myDelegate();
        public WebDBManager m_dbMgr = null;
        int N_DBCount = 0, B_DBCount = 0, F_DBCount = 0; // N_DBCount 디비에 들어잇는 메시지의 갯수 / B_DBCount 처음 프로그램을 실행시켰을때 전까지 읽었던 메세지의 갯수 / F_DBCount 현재까지 읽은 메세지의 갯수

        //public delegate void LabelWriteDelegate(string msg); // 라벨에 찍기 위해 선언한 delegate

        private Thread DBCheck = null;
        private bool _isStop = false; // 쓰레드를 중지하고자 할때 true
        private bool _visible = false;

        public SOPMonitoring()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            notifyIcon1.Visible = true;
            m_dbMgr = new WebDBManager(this);
            Set_Table();

            _visible = true;
            _isStop = false;
            this.StartDB();
        }

        public void Set_Table()
        {
            F_DBCount = B_DBCount = N_DBCount = FileRead();

            GridView.Columns.Clear();

            GridView.ColumnCount = 2;
            GridView.Columns[0].Name = "Message";
            GridView.Columns[1].Name = "Time";

            ReadDB_Table(B_DBCount); 
        }
        public void ReadDB_Table(int Count) // DB 테이블 읽어옴
        {
            try
            {
                string strSQL = "SELECT Message, Time FROM InternalMessage where ID > " + Count;
                ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

                for (int i = 0; i < arrResult.Count - 1; i = i + 2)
                {
                    string[] addRows = { "       " + m_dbMgr.GetStringField(arrResult[i].ToString(), ""), "       " + m_dbMgr.GetStringField(arrResult[i + 1].ToString(), "") };
                    GridView.Rows.Add(addRows);

                    F_DBCount = N_DBCount;
                }
            }
            catch
            {

            }
        }

        public int ReadDB_Count(int count) // DB 테이블 읽어옴
        {
            string strSQL = "SELECT ID FROM InternalMessage where ID >= " + count;
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            for (int i = 0; i < arrResult.Count; i ++)
                count = int.Parse(arrResult[i].ToString());

            return count;
        }

        protected void DBconnection() // 0은 카운트, 1은 검사
        {
            // 지금까지 읽은 디비의 갯수

            //SearchDB("쓰레드 실행중 /  F_DBCount : " + F_DBCount + "  / N_DBCount : " + N_DBCount + "  / B_DBCount : " + B_DBCount);

            N_DBCount = ReadDB_Count(B_DBCount);

            if (_visible == true)
            {
                _visible = false;
                //this.Visible = false;
                this.Invoke(new myDelegate(falseVisible));
            }
            if (F_DBCount < N_DBCount)
            {
                GridView.Rows.Clear();
                ReadDB_Table(FileRead());

                F_DBCount = N_DBCount;
                this.Invoke(new myDelegate(truebisible));
                //this.Visible = true;
                //this.Activate();

            }
            Thread.Sleep(500);
        }
        public void falseVisible()
        {
            this.Visible = false;
        }
        public void truebisible()
        {
            this.Visible = true;
            this.Activate();
        }
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e) // 트레이 아이콘 더블클릭
        {
            Set_Table();
            this.Activate();

            this.Visible = true;
            this.ShowInTaskbar = true; // 현재 프로그램을 테스크 바에 표시하게 한다.
            this.WindowState = FormWindowState.Normal; // 폼을 윈도 상태를 normal
        }

        /*public void SearchDB(string msg)
        {
            LabelWriteDelegate deleLabel = new LabelWriteDelegate(AppendLabel);
            this.Invoke(deleLabel, new object[] { msg });
        }*/

        public void AppendLabel(string msg)
        {
            //GridSetting();

            try
            {
                label1.Text = msg;
            }
            catch (Exception ex)
            {
                label1.Text = ex.ToString();
            }
        }

        public void StartDB()
        {
            DBCheck = new Thread(new ThreadStart(WorkerThreadMethod));
            DBCheck.IsBackground = false;
            DBCheck.Start();
            Thread.Sleep(300);
        }

        public void StopDB()
        {
            try
            {
                if (DBCheck.IsAlive)
                {
                    _isStop = true;
                    DBCheck.Join(1000);
                    DBCheck.Abort();

                    DBCheck = null;
                }
            }
            catch (Exception ex)
            {
            }

            Application.ExitThread();
            Environment.Exit(0);
        }

        public void WorkerThreadMethod() // 메인 쓰레드
        {
            while (!_isStop)
            {
                Thread.Sleep(500);

                DBconnection(); // 1 = 검사
            }
        }

        public void FileWrite(int _num) // 현재까지 읽은 디비 갯수 입력
        {
            StreamWriter WriteFile = new StreamWriter(@"SOPMessage.txt");
            WriteFile.Write(_num);
            WriteFile.Close();
            WriteFile.Dispose();
        }
        public int FileRead() // 프로그램 종료 전까지 읽은 디비 갯수 읽어오기
        {
            StreamReader ReadFile = new StreamReader(@"SOPMessage.txt",System.Text.Encoding.Default);
            int Read_num = int.Parse(ReadFile.ReadToEnd());
            ReadFile.Close();
            ReadFile.Dispose();

            return Read_num;
        }

        public void btn_OK(object sender, EventArgs e)
        {
            FileWrite(F_DBCount);

            this.Visible = false;
            notifyIcon1.Visible = true;
            this.notifyIcon1.Text = "SOP DB Monitoring";
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.StopDB();
        }
    }
    class Data_SOPGenUser
    {
        private string g_Message;
        private string g_Time;

        public string Message
        {
            get { return g_Message; }
            set { g_Message = value; }
        }

        public string Time
        {
            get { return g_Time; }
            set { g_Time = value; }
        }
    }
}
