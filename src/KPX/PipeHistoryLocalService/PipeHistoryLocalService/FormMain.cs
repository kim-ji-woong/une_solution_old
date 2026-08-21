using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace PipeHistoryLocalService
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
#if !SERVICE
            LogFileManager.Instance.Start();
            //ReadFile(@"C:\Users\UNESUHYUN\Desktop\13.dat");
            //ReadFileFlow(@"C:\Users\UNESUHYUN\Desktop\tank\403\1.dat");
            //ReadFileFlow(@"C:\Users\UNESUHYUN\Desktop\tank\403\30.dat");
            //ReadFileFlow(@"13.dat");
#endif
        }

        // File 확인
        public void ReadFile(string path)
        {
            try
            {
                string[] paths = path.Split('.');
                string strOutput = paths[0] + ".txt";
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(strOutput))
                {
                    // 파일공유 옵션 추가
                    using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                    {
                        int pos = 0;
                        int length = (int)reader.BaseStream.Length;
                        while (pos < length)
                        {
                            long time = reader.ReadInt64();
                            DateTime dt = new DateTime(time);

                            float pressure = reader.ReadSingle();
                            float flow = reader.ReadSingle();
                            float tankID = reader.ReadInt32();
                            //if (flow != 0)
                            //{
                            //    Console.Write(dt);
                            //    Console.WriteLine(" : " + flow);
                            //}

                            string str = string.Format("[{0}-{1:D2}-{2:D2} {3:D2}:{4:D2}:{5:D2}] {6}, {7}, {8}",
                                dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, pressure, flow, tankID);
                            file.WriteLine(str);
                        }
                    }
                }
            }
            catch (Exception)
            {
                System.Diagnostics.Trace.WriteLine("File Read Error");
            }
        }

        public void ReadFileFlow(string path)
        {
            try
            {
                string[] paths = path.Split('.');
                string strOutput = paths[0] + ".txt";
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(strOutput))
                {
                    // 파일공유 옵션 추가
                    using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                    {
                        int pos = 0;
                        int length = (int)reader.BaseStream.Length;
                        while (pos < length)
                        {
                            long time = reader.ReadInt64();
                            DateTime dt = new DateTime(time);

                            float flow = reader.ReadSingle();
                            float temp = reader.ReadSingle();
                            float level = reader.ReadSingle();
                            int pipeID = reader.ReadInt32();
                            float press = reader.ReadSingle();

                            string str = string.Format("[{0}-{1:D2}-{2:D2} {3:D2}:{4:D2}:{5:D2}] {6}, {7}, {8}, {9}, {10}",
                                dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, flow, temp, level, pipeID, press);
                            file.WriteLine(str);
                        }
                    }
                }
            }
            catch (Exception)
            {
                System.Diagnostics.Trace.WriteLine("File Read Error");
            }
        }
    }
}
