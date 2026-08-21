using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Data.SqlClient;

namespace WindowsService
{
    public partial class MyNewService : ServiceBase
    {
        public MyNewService()
        {
            InitializeComponent();

            if (!System.Diagnostics.EventLog.SourceExists("MySource")) 
			{         
				System.Diagnostics.EventLog.CreateEventSource("MySource","MyNewLog");
			}
			eventLog1.Source = "MySource";
			eventLog1.Log = "MyNewLog";  
        }

        Program p;

        private void ThreadTest()
        {
            Thread th = new Thread(new ThreadStart(WorkerThreadMethod));
            th.Start();
        }

        public void DBconnection()
        {
            FileStream FS;
            StreamWriter SW;
            FS = new FileStream("C:\\TEST.TXT", FileMode.Append, FileAccess.Write);
            SW = new StreamWriter(FS);

            SqlConnection con = new SqlConnection();
            con.ConnectionString = "Server=192.168.0.207;database=SOP2;uid=sa;pwd=9449966Ab";
            con.Open();
            SqlDataAdapter adapt = new SqlDataAdapter();
            adapt.SelectCommand = new SqlCommand("select Message from InternalMessage", con);

            DataSet ds = new DataSet();
            adapt.Fill(ds);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++ )
                SW.WriteLine(ds.Tables[0].Rows[i][0].ToString());

            SW.WriteLine(ds.Tables[0].Rows[0][0].ToString());
            SW.WriteLine(ds.Tables[0].Rows[1][0].ToString());
            SW.WriteLine(ds.Tables[0].Rows[2][0].ToString());
            
            /*try
            {
                SqlCommand sqlcmd = new SqlCommand();
                //명령 전달을 위해 사용할 커맨드 생성
                sqlcmd.CommandText = "select Message from InternalMessage where ID = '1'";

                sqlcmd.Connection = con;
                sqlcmd.CommandTimeout = 20;
                sqlcmd.CommandType = CommandType.Text;

                con.Open();
                String affect = sqlcmd.ExecuteNonQuery().ToString;

                SW.WriteLine(affect);
                con.Close();
            }
            catch (Exception ex)
            {
                SW.WriteLine("Error");
            }*/

            SW.Close();
            FS.Close();
        }

        public void WorkerThreadMethod() // 메인 쓰레드
        {
            FileStream FS;
            StreamWriter SW;

            DBconnection();

            for (int i = 0; i < 5; i++)
            {
                Thread.Sleep(3000);
                FS = new FileStream("C:\\TEST.TXT", FileMode.Append, FileAccess.Write);
                SW = new StreamWriter(FS);
                SW.WriteLine(DateTime.Now.ToString() + " 서비스 프로그램이 실행중입니다.");
                
                //if (Convert.ToInt32(DateTime.Now.Hour) == 10)
                //    SW.WriteLine("aaaaaaaaaaaaaaaaaaaaa");

                SW.Close();
                FS.Close();
            }
        }

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("In OnStart");
            MyNewService mns = new MyNewService();
            
            p = new Program();

            mns.ThreadTest();
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("In onStop.");
            p.Dispose();
        }

        public class Program : IDisposable
        {
            public Program()
            {
                FileStream FS = new FileStream("C:\\TEST.TXT", FileMode.Append, FileAccess.Write);
                StreamWriter SW = new StreamWriter(FS);
                SW.WriteLine(DateTime.Now.ToString() + " 서비스 프로그램이 실행되었습니다.");
                SW.Close();
                FS.Close();
            }
            public void Dispose()
            {
                FileStream FS = new FileStream("C:\\TEST.TXT", FileMode.Append, FileAccess.Write);
                StreamWriter SW = new StreamWriter(FS);
                SW.WriteLine(DateTime.Now.ToString() + " 서비스 프로그램이 종료되었습니다.");
                SW.Close();
                FS.Close();
            }
        }
    }
}
