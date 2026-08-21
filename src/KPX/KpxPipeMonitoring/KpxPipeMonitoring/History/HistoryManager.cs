using System;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using DBUtility;
using KpxPipeMonitoring;

namespace KpxPipeMonitoring
{
    public class HistoryManager : IHistoryManager
    {
        private class FileItem
        {
            private HistoryQueryType m_Type;
            internal HistoryQueryType QueryType
            {
                get { return m_Type; }
                set { m_Type = value; }
            }

            private string m_szFileName;
            public string FileName
            {
                get { return m_szFileName; }
                set { m_szFileName = value; }
            }
        }

        private MainForm mainForm;        
        public HistoryManager(MainForm form)
        {
            mainForm = form;
        }

        public List<CommonFunction.ChartField> ReadHistory(List<HistoryQuery> historyQueries)
        { 
            List<FileItem> pathList = new List<FileItem>();
            foreach(HistoryQuery query in historyQueries)
            {
                if (query.TargetID <= 0)
                    continue;
                
                string szPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                if(query.QueryType == HistoryQueryType.유량)
                {
                    szPath += "\\UNE\\KPX\\flow";
                }
                else if (query.QueryType == HistoryQueryType.압력 || query.QueryType == HistoryQueryType.작업중)
                {
                    szPath += "\\UNE\\KPX\\work";
                }

                string path = string.Format(@"{0}\{1}\{2}\{3}\{4}.dat", szPath, query.TargetID, query.Year, query.Month, query.Day);
                FileItem item = new FileItem();
                item.FileName = path;
                item.QueryType = query.QueryType;
                pathList.Add(item);
            }
            return ReadWorkFile(pathList);
        }

        private List<CommonFunction.ChartField> ReadWorkFile(List<FileItem> pathList)
        { 
            List<CommonFunction.ChartField> result = new List<CommonFunction.ChartField>();
            foreach (FileItem item in pathList)
            {
                string path = item.FileName;
                if (item.QueryType == HistoryQueryType.작업중 || item.QueryType == HistoryQueryType.압력)
                {
                    ReadPressureHistory(path, result);
                }      
                //else if(item.QueryType == HistoryQueryType.압력)
                //{
                //    ReadPressureHistory(path, result);
                //}
                else if(item.QueryType == HistoryQueryType.유량)
                {
                    ReadFlowHistory(path, result);
                }
            }
            return result;
        }

        private void ReadFlowHistory(string path, List<CommonFunction.ChartField> result)
        {
            if (!File.Exists(path))
                return;

            //TankID 구하기
            string[] pathIndex = path.Split('\\');
            if (pathIndex.Length == 0)
                return;
            int tankId = 0;
            int.TryParse(pathIndex[pathIndex.Length - 4], out tankId);

            try
            {
                // 파일 read, write 충돌로 인한 더미파일 체크
                string[] names = path.Split('.');
                if (!File.Exists(names[0] + "_dummy.dat"))
                {
                    //// 기존
                    //using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                    //{
                    //    int pos = 0;
                    //    int length = (int)reader.BaseStream.Length;
                    //    while (pos < length)
                    //    {
                    //        long time = reader.ReadInt64();
                    //        DateTime dt = new DateTime(time);

                    //        float flow = reader.ReadSingle();
                    //        float temp = reader.ReadSingle();
                    //        float level = reader.ReadSingle();
                    //        int pipeId = reader.ReadInt32();
                    //        float pressure = reader.ReadSingle();

                    //        pos += sizeof(long) + (sizeof(float) * 4) + sizeof(int);

                    //        result.Add(new CommonFunction.ChartField(pipeId, tankId, dt, pressure, flow));
                    //    }
                    //} 

                    // 개선
                    //var bytes = File.ReadAllBytes(path);
                    //var rowSize = sizeof(long) + (sizeof(float) * 4) + sizeof(int);
                    //for (var offset = 0; offset < bytes.Length; offset += rowSize)
                    //{
                    //    long time = BitConverter.ToInt64(bytes, offset + 0);
                    //    DateTime dt = new DateTime(time);

                    //    float flow = BitConverter.ToSingle(bytes, offset + 8);
                    //    float temp = BitConverter.ToSingle(bytes, offset + 12);
                    //    float level = BitConverter.ToSingle(bytes, offset + 16);
                    //    int pipeId = BitConverter.ToInt32(bytes, offset + 20);
                    //    float pressure = BitConverter.ToSingle(bytes, offset + 24);
                    //    if (pressure == -999 || pressure == -9999) pressure = 0;

                    //    result.Add(new CommonFunction.ChartField(pipeId, tankId, dt, pressure, flow));
                    //}

                    //bytes = null; 
                    
                    ReadFileManager.ReadHistory rh = new ReadFileManager.ReadHistory();
                    List<ReadFileManager.ChartField> list = rh.ReadFlow(path, tankId);
                     
                    if (list != null)
                    {
                        foreach (ReadFileManager.ChartField item in list)
                        {
                            result.Add(new CommonFunction.ChartField(item.nPipeID(), item.nTankID(), (DateTime)item.dtTimeStamp(), item.dPressure(), item.dFlow()));
                        }  
                    }
                }
            }
            catch (Exception ex)
            {
                MainForm.Instance.SetSystemLog("[ERROR] ReadFlowHistory(string path, List<CommonFunction.ChartField> result) / " + ex.Message);
            }
        } 
        private void ReadPressureHistory(string path, List<CommonFunction.ChartField> result)
        { 
            if (!File.Exists(path))
                return;

            //PipeID 구하기
            string[] pathIndex = path.Split('\\');
            if (pathIndex.Length == 0)
                return;
            int pipeId = 0;
            int.TryParse(pathIndex[pathIndex.Length - 4], out pipeId);
             
            try
            {
                // 파일 read, write 충돌로 인한 더미파일 체크
                string[] names = path.Split('.');
                if (!File.Exists(names[0] + "_dummy.dat"))
                { 
                    // 기존
                    //using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                    //{
                    //    int pos = 0;
                    //    int length = (int)reader.BaseStream.Length;

                    //    while (pos < length)
                    //    {
                    //        long time = reader.ReadInt64();
                    //        DateTime dt = new DateTime(time);

                    //        float pressure = reader.ReadSingle();
                    //        float flow = reader.ReadSingle();
                    //        if (flow == -999 || flow == -9999) flow = 0;
                    //        int tankId = reader.ReadInt32();
                    //        pos += sizeof(long) + (sizeof(float) * 2) + sizeof(int); 
                    //        result.Add(new CommonFunction.ChartField(pipeId, tankId, dt, pressure, flow));
                    //    }
                    //}  

                    // 속도 개선
                    //var bytes = File.ReadAllBytes(path);
                    //var rowSize = sizeof(long) + (sizeof(float) * 2) + sizeof(int);
                    //for (var offset = 0; offset < bytes.Length; offset += rowSize)
                    //{
                    //    long time = BitConverter.ToInt64(bytes, offset + 0);
                    //    DateTime dt = new DateTime(time);

                    //    float pressure = BitConverter.ToSingle(bytes, offset + 8);
                    //    float flow = BitConverter.ToSingle(bytes, offset + 12);
                    //    if (flow == -999 || flow == -9999) flow = 0;
                    //    int tankId = BitConverter.ToInt32(bytes, offset + 16);

                    //    result.Add(new CommonFunction.ChartField(pipeId, tankId, dt, pressure, flow));
                    //}
                    //GC.Collect();
                     
                    //C++/cli 
                    ReadFileManager.ReadHistory rh = new ReadFileManager.ReadHistory();
                    List<ReadFileManager.ChartField> list = rh.ReadPressure(path, pipeId); 

                    if (list != null)
                    {
                        foreach (ReadFileManager.ChartField item in list)
                        {
                            result.Add(new CommonFunction.ChartField(pipeId, item.nTankID(), (DateTime)item.dtTimeStamp(), item.dPressure(), item.dFlow()));
                        } 
                    } 
                }
            }
            catch (Exception ex)
            {
                MainForm.Instance.SetSystemLog("[ERROR] ReadWorkHistory(string path, List<CommonFunction.ChartField> result) / " + ex.Message);
            }
        }
    }
}
