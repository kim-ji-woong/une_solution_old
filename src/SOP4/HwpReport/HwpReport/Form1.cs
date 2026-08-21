using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace HwpReport
{
    public partial class Form1 : Form
    {
        private class HWPOption
        {
            public string TagName
            {
                get;
                set;
            }

            public string ImageBookmark
            {
                get;
                set;
            }

            public string TableBookmark
            {
                get;
                set;
            }

            public int DeleteLineCount
            {
                get;
                set;
            }

            public int ColumnCount1
            {
                get;
                set;
            }

            public int ColumnCount2
            {
                get;
                set;
            }

            public string NewImageFilePath
            {
                get;
                set;
            }

            public int InitLineNumber
            {
                get;
                set;
            }

            public int InitPosition
            {
                get;
                set;
            }

            public int OtherPosition
            {
                get;
                set;
            }

            public int ImageWidth
            {
                get;
                set;
            }

            public int ImageHeight
            {
                get;
                set;
            }
        }
        private string m_strLogoName = string.Empty;
        private int m_nSiteID = 0;

        public Form1()
        {
            InitializeComponent();

            this.Size = new Size(100, 100);
            this.Location = new Point(-200, 0);

            //CreateHWP();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CreateHWP();
        }
        
        private void CreateHWP(string strFilePath, int nDeleteLineCount, int nColumnCount1, int nColumnCount2, string strNewImageFilePath, int nInitLineNumber, int nInitPosition, int nOtherPosition, int nImageLine = 14)
        {
            LoadFile(strFilePath);
             
            #region 머리말
            if (!string.IsNullOrEmpty(m_strLogoName))
            {
                HWPOption optionSensor = new HWPOption();
                optionSensor.ImageBookmark = "HeadArea";
                optionSensor.NewImageFilePath = Application.StartupPath + "\\" + m_strLogoName; 
                if (m_nSiteID == 100)
                {
                    optionSensor.ImageWidth = 25;
                    optionSensor.ImageHeight = 5;
                }
                else
                {
                    optionSensor.ImageWidth = 30;
                    optionSensor.ImageHeight = 5;
                }

                SetBookMarkPosition(optionSensor.ImageBookmark);

                FileInfo f = new FileInfo(optionSensor.NewImageFilePath);
                if (f.Exists)
                    axHwpCtrl1.InsertPicture(optionSensor.NewImageFilePath, 1, 1, 0, 0, 0, optionSensor.ImageWidth, optionSensor.ImageHeight);  
            }
            #endregion

            SetBookMarkPosition("Begin Table"); 

            //현재 위치한 커서값을 받아온다.
            int nPosition = 0, para = 0, pos = 0;
            this.axHwpCtrl1.GetPos(ref nPosition, ref para, ref pos);

            //예제에있는 표의 줄 삭제
            for (int j = 0; j < nDeleteLineCount; j++)
            {
                TableDeleteRow(nPosition + nColumnCount1, 0, 0);
            }

            TableAppandRow(nPosition, 0, 0);

            string line2;
            int Arraycount = 0;

            using (StreamReader sr = new StreamReader(Application.StartupPath + "\\report\\SaveData.txt", System.Text.Encoding.UTF8))
            {
                while ((line2 = sr.ReadLine()) != null)
                {
                    //데이터 수에 맞춰서 줄 늘림
                    if (Arraycount != 0)
                    {
                        if (Arraycount % nColumnCount2 == 0)
                        {
                            TableAppandRow(nPosition, 0, 0);
                        }
                    }

                    InsertText(nPosition + nColumnCount2, 0, 0, line2);
                    //Console.WriteLine(strArray[count]);

                    if (Arraycount < nColumnCount2)
                    {
                        //표 배경색깔 변경
                        CellBlock(nPosition + nColumnCount2, 0, 0);
                        ChangeTableProperty();

                        //글씨 굵기 없애기
                        HWPCONTROLLib.DHwpAction ac3 = (HWPCONTROLLib.DHwpAction)this.axHwpCtrl1.CreateAction("CharShape");
                        HWPCONTROLLib.DHwpParameterSet cs3 = (HWPCONTROLLib.DHwpParameterSet)ac3.CreateSet();
                        ac3.GetDefault(cs3);

                        cs3.SetItem("Bold", 0);
                        //cs3.SetItem("TextColor", 0xFF0000);

                        ac3.Execute(cs3);

                        axHwpCtrl1.Run("Cancel");
                    }

                    Arraycount++;
                    nPosition++;
                }
                sr.Close();
            }

            if (strNewImageFilePath != null)
            {
                //원래 있던 이미지 삭제
                axHwpCtrl1.SetPos(0, nImageLine, 0);
                axHwpCtrl1.Run("Select");
                axHwpCtrl1.Run("Select");
                axHwpCtrl1.Run("Delete");

                //새 이미지 삽입
                axHwpCtrl1.InsertPicture(strNewImageFilePath, 1, 1, 0, 0, 0, 150, 45);
            }
             
            //string strSaveFilePath = "C:\\Save.txt"; // 파일의 경로입니다.
            //StreamReader SRead = new StreamReader(strSaveFilePath, System.Text.Encoding.UTF8);
            //string strFileLine = SRead.ReadLine();

            //for (int i = 0; i < strArray.Length; i++)
            //{
            //    //Console.Write(strArray[i]); // 읽어온 문자열을 뿌립니다.
            //    Console.WriteLine(strArray[i]);
            //}

            if (m_nReportMode != (int)SDMS.Data.ReportMode.DisasterPrevention)
            {
                string line;
                int lineNum = nInitLineNumber;
                using (StreamReader sr = new StreamReader(Application.StartupPath + "\\report\\SaveDateTime.txt", System.Text.Encoding.UTF8))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (lineNum == nInitLineNumber)
                        {
                            DeleteText(0, lineNum, nInitPosition);
                            InsertText(0, lineNum, nInitPosition, line);
                        }
                        else
                        {
                            DeleteText(0, lineNum, nOtherPosition);
                            InsertText(0, lineNum, nOtherPosition, line);
                        }

                        lineNum++;
                    }
                    sr.Close();
                }

                SetBookMarkPosition("MemoBody");
                this.axHwpCtrl1.GetPos(ref nPosition, ref para, ref pos);
                string strMemo = "", strMargin = "   ";

                if (File.Exists(Application.StartupPath + "\\report\\SaveMemo.txt"))
                {
                    using (StreamReader sr = new StreamReader(Application.StartupPath + "\\report\\SaveMemo.txt", System.Text.Encoding.UTF8))
                    {

                        while (sr.EndOfStream == false)
                        {
                            line = sr.ReadLine().Trim();

                            if (strMemo.Length == 0)
                                strMemo = strMargin + line;
                            else
                                strMemo += "\r\n" + strMargin + line;
                        }
                        sr.Close();
                    }
                }

                if (strMemo.Length > 0)
                    InsertText(nPosition, para, pos, strMemo); 
            }
        }

        private bool CheckOption(string strLine, string strTagName, ref bool currentOption)
        {
            strLine = strLine.Trim();

            if (strLine.StartsWith("[") && strLine.EndsWith("]"))
            {
                string strTag = strLine.Substring(1, strLine.Length - 2);

                if (strTagName == strTag)
                {
                    currentOption = true;
                    return true;
                }
                else
                    currentOption = false;
            }

            return false;
        }

        private int GetImageHeight(string strImagePath)
        {
            Image img = Image.FromFile(strImagePath);

            if (img == null || img.Height <= 0 || img.Width <= 0)
                return 45;

            return 45 * img.Height / 290;
        }

        private void CreateHWP(string strFilePath, List<HWPOption> options)
        { 
            LoadFile(strFilePath);

            #region 머리말
            if (!string.IsNullOrEmpty(m_strLogoName))
            {
                HWPOption optionSensor = new HWPOption();
                optionSensor.ImageBookmark = "HeadArea";
                optionSensor.NewImageFilePath = Application.StartupPath + "\\" + m_strLogoName;                
                if (m_nSiteID == 100)
                {
                    optionSensor.ImageWidth = 25;
                    optionSensor.ImageHeight = 5;
                }
                else
                {
                    optionSensor.ImageWidth = 30;
                    optionSensor.ImageHeight = 5;
                }

                SetBookMarkPosition(optionSensor.ImageBookmark);

                FileInfo f = new FileInfo(optionSensor.NewImageFilePath);
                if (f.Exists)
                    axHwpCtrl1.InsertPicture(optionSensor.NewImageFilePath, 1, 1, 0, 0, 0, optionSensor.ImageWidth, optionSensor.ImageHeight);
            }
            #endregion 

            bool currentOption = false;

            foreach (HWPOption option in options)
            {
                SetBookMarkPosition(option.TableBookmark);

                //현재 위치한 커서값을 받아온다.
                int nPosition = 0, para = 0, pos = 0;
                this.axHwpCtrl1.GetPos(ref nPosition, ref para, ref pos);

                //예제에있는 표의 줄 삭제
                for (int j = 0; j < option.DeleteLineCount; j++)
                {
                    TableDeleteRow(nPosition + option.ColumnCount1, 0, 0);
                }

                TableAppandRow(nPosition, 0, 0);

                string line2;
                int Arraycount = 0;

                using (StreamReader sr = new StreamReader(Application.StartupPath + "\\report\\SaveData.txt", System.Text.Encoding.UTF8))
                {
                    while ((line2 = sr.ReadLine()) != null)
                    {
                        if (CheckOption(line2, option.TagName, ref currentOption))
                            continue;

                        if (!currentOption)
                            continue;

                        //데이터 수에 맞춰서 줄 늘림
                        if (Arraycount != 0)
                        {
                            if (Arraycount % option.ColumnCount2 == 0)
                            {
                                TableAppandRow(nPosition, 0, 0);
                            }
                        }

                        InsertText(nPosition + option.ColumnCount2, 0, 0, line2);
                        //Console.WriteLine(strArray[count]);

                        if (Arraycount < option.ColumnCount2)
                        {
                            //표 배경색깔 변경
                            CellBlock(nPosition + option.ColumnCount2, 0, 0);
                            ChangeTableProperty();

                            //글씨 굵기 없애기
                            HWPCONTROLLib.DHwpAction ac3 = (HWPCONTROLLib.DHwpAction)this.axHwpCtrl1.CreateAction("CharShape");
                            HWPCONTROLLib.DHwpParameterSet cs3 = (HWPCONTROLLib.DHwpParameterSet)ac3.CreateSet();
                            ac3.GetDefault(cs3);

                            cs3.SetItem("Bold", 0);
                            //cs3.SetItem("TextColor", 0xFF0000);

                            ac3.Execute(cs3);

                            axHwpCtrl1.Run("Cancel");
                        }

                        Arraycount++;
                        nPosition++;
                    }
                    sr.Close();
                }

                if (option.NewImageFilePath != null)
                {
                    SetBookMarkPosition(option.ImageBookmark);

                    //원래 있던 이미지 삭제
                    axHwpCtrl1.Run("Select");
                    axHwpCtrl1.Run("Select");
                    axHwpCtrl1.Run("Delete");

                    //새 이미지 삽입
                    axHwpCtrl1.InsertPicture(option.NewImageFilePath, 1, 1, 0, 0, 0, option.ImageWidth, option.ImageHeight);
                }

                if (option.InitLineNumber >= 0)
                {
                    string line;
                    int lineNum = option.InitLineNumber;
                    using (StreamReader sr = new StreamReader(Application.StartupPath + "\\report\\SaveDateTime.txt", System.Text.Encoding.UTF8))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (lineNum == option.InitLineNumber)
                            {
                                DeleteText(0, lineNum, option.InitPosition);
                                InsertText(0, lineNum, option.InitPosition, line);
                            }
                            else
                            {
                                DeleteText(0, lineNum, option.OtherPosition);
                                InsertText(0, lineNum, option.OtherPosition, line);
                            }

                            lineNum++;
                        }
                        sr.Close();
                    }
                }
            } 
        }

        private void CreateParetoFire(string strFilePath)
        {
            HWPOption optionSensor = new HWPOption();

            optionSensor.TagName = "ParetoSensor";
            optionSensor.ImageBookmark = "ParetoSensorImage";
            optionSensor.TableBookmark = "BeginTableSensor";
            optionSensor.DeleteLineCount = 2;
            optionSensor.ColumnCount1 = 7;
            optionSensor.ColumnCount2 = 7;
            optionSensor.NewImageFilePath = Application.StartupPath + "\\report\\ParetoSensor.bmp";
            optionSensor.InitLineNumber = 6;
            optionSensor.InitPosition = 8;
            optionSensor.OtherPosition = 8;
            optionSensor.ImageWidth = 240;
            optionSensor.ImageHeight = GetImageHeight(optionSensor.NewImageFilePath);//45;

            HWPOption optionEquipZone = new HWPOption();

            optionEquipZone.TagName = "ParetoEquipZone";
            optionEquipZone.ImageBookmark = "ParetoEquipZoneImage";
            optionEquipZone.TableBookmark = "BeginTableEquipZone";
            optionEquipZone.DeleteLineCount = 2;
            optionEquipZone.ColumnCount1 = 6;
            optionEquipZone.ColumnCount2 = 6;
            optionEquipZone.NewImageFilePath = Application.StartupPath + "\\report\\ParetoEquipZone.bmp";
            optionEquipZone.InitLineNumber = -1;
            optionEquipZone.InitPosition = -1;
            optionEquipZone.OtherPosition = -1;
            optionEquipZone.ImageWidth = 240;
            optionEquipZone.ImageHeight = GetImageHeight(optionEquipZone.NewImageFilePath);//45;
             
            List<HWPOption> options = new List<HWPOption>();
            options.Add(optionSensor);
            options.Add(optionEquipZone); 

            CreateHWP(strFilePath, options);
        }

        private void CreateParetoPSM(string strFilePath)
        {
            HWPOption optionSensor = new HWPOption();

            optionSensor.TagName = "ParetoSensor";
            optionSensor.ImageBookmark = "ParetoSensorImage";
            optionSensor.TableBookmark = "BeginTableSensor";
            optionSensor.DeleteLineCount = 2;
            optionSensor.ColumnCount1 = 7;
            optionSensor.ColumnCount2 = 7;
            optionSensor.NewImageFilePath = Application.StartupPath + "\\report\\ParetoSensor.bmp";
            optionSensor.InitLineNumber = 6;
            optionSensor.InitPosition = 8;
            optionSensor.OtherPosition = 8;
            optionSensor.ImageWidth = 240;
            optionSensor.ImageHeight = GetImageHeight(optionSensor.NewImageFilePath);//45;

            HWPOption optionTank = new HWPOption();

            optionTank.TagName = "ParetoTank";
            optionTank.ImageBookmark = "ParetoTankImage";
            optionTank.TableBookmark = "BeginTableTank";
            optionTank.DeleteLineCount = 2;
            optionTank.ColumnCount1 = 7;
            optionTank.ColumnCount2 = 7;
            optionTank.NewImageFilePath = Application.StartupPath + "\\report\\ParetoTank.bmp";
            optionTank.InitLineNumber = -1;
            optionTank.InitPosition = -1;
            optionTank.OtherPosition = -1;
            optionTank.ImageWidth = 240;
            optionTank.ImageHeight = GetImageHeight(optionTank.NewImageFilePath);//45;

            HWPOption optionEquipZone = new HWPOption();

            optionEquipZone.TagName = "ParetoEquipZone";
            optionEquipZone.ImageBookmark = "ParetoEquipZoneImage";
            optionEquipZone.TableBookmark = "BeginTableEquipZone";
            optionEquipZone.DeleteLineCount = 2;
            optionEquipZone.ColumnCount1 = 5;
            optionEquipZone.ColumnCount2 = 5;
            optionEquipZone.NewImageFilePath = Application.StartupPath + "\\report\\ParetoEquipZone.bmp";
            optionEquipZone.InitLineNumber = -1;
            optionEquipZone.InitPosition = -1;
            optionEquipZone.OtherPosition = -1;
            optionEquipZone.ImageWidth = 240;
            optionEquipZone.ImageHeight = GetImageHeight(optionEquipZone.NewImageFilePath);//45;

            HWPOption optionMaterial = new HWPOption();

            optionMaterial.TagName = "ParetoMaterial";
            optionMaterial.ImageBookmark = "ParetoMaterialImage";
            optionMaterial.TableBookmark = "BeginTableMaterial";
            optionMaterial.DeleteLineCount = 2;
            optionMaterial.ColumnCount1 = 4;
            optionMaterial.ColumnCount2 = 4;
            optionMaterial.NewImageFilePath = Application.StartupPath + "\\report\\ParetoMaterial.bmp";
            optionMaterial.InitLineNumber = -1;
            optionMaterial.InitPosition = -1;
            optionMaterial.OtherPosition = -1;
            optionMaterial.ImageWidth = 240;
            optionMaterial.ImageHeight = GetImageHeight(optionMaterial.NewImageFilePath);//45;
             
            List<HWPOption> options = new List<HWPOption>();
            options.Add(optionSensor);
            options.Add(optionTank);
            options.Add(optionEquipZone);
            options.Add(optionMaterial); 

            CreateHWP(strFilePath, options);
        }

        private void CreateParetoIntrusion(string strFilePath)
        {
            HWPOption optionSensor = new HWPOption();

            optionSensor.TagName = "ParetoSensor";
            optionSensor.ImageBookmark = "ParetoSensorImage";
            optionSensor.TableBookmark = "BeginTableSensor";
            optionSensor.DeleteLineCount = 2;
            optionSensor.ColumnCount1 = 7;
            optionSensor.ColumnCount2 = 7;
            optionSensor.NewImageFilePath = Application.StartupPath + "\\report\\ParetoSensor.bmp";
            optionSensor.InitLineNumber = 6;
            optionSensor.InitPosition = 8;
            optionSensor.OtherPosition = 8;
            optionSensor.ImageWidth = 240;
            optionSensor.ImageHeight = GetImageHeight(optionSensor.NewImageFilePath);//45;

            HWPOption optionEquipZone = new HWPOption();

            optionEquipZone.TagName = "ParetoEquipZone";
            optionEquipZone.ImageBookmark = "ParetoEquipZoneImage";
            optionEquipZone.TableBookmark = "BeginTableEquipZone";
            optionEquipZone.DeleteLineCount = 2;
            optionEquipZone.ColumnCount1 = 6;
            optionEquipZone.ColumnCount2 = 6;
            optionEquipZone.NewImageFilePath = Application.StartupPath + "\\report\\ParetoEquipZone.bmp";
            optionEquipZone.InitLineNumber = -1;
            optionEquipZone.InitPosition = -1;
            optionEquipZone.OtherPosition = -1;
            optionEquipZone.ImageWidth = 240;
            optionEquipZone.ImageHeight = GetImageHeight(optionEquipZone.NewImageFilePath);//45;
             
            List<HWPOption> options = new List<HWPOption>();
            options.Add(optionSensor);
            options.Add(optionEquipZone); 

            CreateHWP(strFilePath, options);
        }

        private void CreateDisasterPrevention(string strFilePath)
        {
            HWPOption option = new HWPOption();

            option.TagName = "DisasterPrevention";
            //optionSensor.ImageBookmark = "ParetoSensorImage";
            option.TableBookmark = "ContentTable";
            option.DeleteLineCount = 2;
            option.ColumnCount1 = 9;
            option.ColumnCount2 = 9;
            //optionSensor.NewImageFilePath = Application.StartupPath + "\\report\\ParetoSensor.bmp";
            option.InitLineNumber = 6;
            option.InitPosition = 8;
            option.OtherPosition = 8;
            //optionSensor.ImageWidth = 240;
            //optionSensor.ImageHeight = GetImageHeight(optionSensor.NewImageFilePath);//45;
             
            List<HWPOption> options = new List<HWPOption>();
            options.Add(option); 

            CreateHWP(strFilePath, options);
        }

        private int m_nReportMode = 0;
        public void CreateHWP()
        {
            //MessageBox.Show("ddd");
            Console.WriteLine(Application.StartupPath);
            string[] strArg = Environment.GetCommandLineArgs();
            if (strArg.Length < 3)
            {
                CloseForm();
                return;
            }

            string arg1 = strArg[1];
            string arg2 = strArg[2];
            string arg3 = (strArg.Length > 3) ? strArg[3] : string.Empty;
            int arg4 = (strArg.Length > 4) ? Convert.ToInt32(strArg[4]) : 0;

            this.m_strLogoName = arg3;
            this.m_nSiteID = arg4;

            Console.WriteLine(strArg[0]);
            Console.WriteLine(arg1);
            Console.WriteLine(arg2);
            Console.WriteLine(arg3);
            Console.WriteLine(arg4);
            Console.WriteLine(";;;");

            m_nReportMode = Convert.ToInt32(arg1);

            if (arg1 == ((int)SDMS.Data.ReportMode.DetectFireAnalyze).ToString())
            {
                CreateParetoFire(Application.StartupPath + "\\report\\화재 탐지분석 보고서.hwp");
            }
            else if (arg1 == ((int)SDMS.Data.ReportMode.DetectFire).ToString())
            {
                CreateHWP(Application.StartupPath + "\\report\\화재 탐지 보고서.hwp", 11, 8, 8, Application.StartupPath + "\\report\\Detect.bmp", 8, 8, 8);
                /*LoadFile(Application.StartupPath + "\\report\\화재 탐지 보고서.hwp");

                SetBookMarkPosition("Begin Table");

                //현재 위치한 커서값을 받아온다.
                int nPosition = 0, para = 0, pos = 0;
                this.axHwpCtrl1.GetPos(ref nPosition, ref para, ref pos);

                //예제에있는 표의 줄 삭제
                for (int j = 0; j < 15; j++)
                {
                    TableDeleteRow(nPosition + 7, 0, 0);
                }

                TableAppandRow(nPosition, 0, 0);

                string line2;
                int Arraycount = 0;

                using (StreamReader sr = new StreamReader(Application.StartupPath + "\\report\\SaveData.txt", System.Text.Encoding.UTF8))
                {
                    while ((line2 = sr.ReadLine()) != null)
                    {
                        //데이터 수에 맞춰서 줄 늘림
                        if (Arraycount != 0)
                        {
                            if (Arraycount % 7 == 0)
                            {
                                TableAppandRow(nPosition, 0, 0);
                            }
                        }

                        InsertText(nPosition + 7, 0, 0, line2);
                        //Console.WriteLine(strArray[count]);

                        if (Arraycount < 7)
                        {
                            //표 배경색깔 변경
                            CellBlock(nPosition + 7, 0, 0);
                            ChangeTableProperty();

                            //글씨 굵기 없애기
                            HWPCONTROLLib.DHwpAction ac3 = (HWPCONTROLLib.DHwpAction)this.axHwpCtrl1.CreateAction("CharShape");
                            HWPCONTROLLib.DHwpParameterSet cs3 = (HWPCONTROLLib.DHwpParameterSet)ac3.CreateSet();
                            ac3.GetDefault(cs3);

                            cs3.SetItem("Bold", 0);
                            //cs3.SetItem("TextColor", 0xFF0000);

                            ac3.Execute(cs3);

                            axHwpCtrl1.Run("Cancel");
                        }

                        Arraycount++;
                        nPosition++;
                    }
                    sr.Close();
                }

                //원래 있던 이미지 삭제
                axHwpCtrl1.SetPos(0, 14, 0);
                axHwpCtrl1.Run("Select");
                axHwpCtrl1.Run("Select");
                axHwpCtrl1.Run("Delete");

                //새 이미지 삽입
                axHwpCtrl1.InsertPicture(Application.StartupPath + "\\report\\Detect.bmp", 1, 1, 0, 0, 0, 150, 45);

                //string strSaveFilePath = "C:\\Save.txt"; // 파일의 경로입니다.
                //StreamReader SRead = new StreamReader(strSaveFilePath, System.Text.Encoding.UTF8);
                //string strFileLine = SRead.ReadLine();

                //for (int i = 0; i < strArray.Length; i++)
                //{
                //    //Console.Write(strArray[i]); // 읽어온 문자열을 뿌립니다.
                //    Console.WriteLine(strArray[i]);
                //}

                string line;
                int lineNum = 8;
                using (StreamReader sr = new StreamReader(Application.StartupPath + "\\report\\SaveDateTime.txt", System.Text.Encoding.UTF8))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        DeleteText(0, lineNum, 8);
                        InsertText(0, lineNum, 8, line);

                        lineNum++;
                    }
                    sr.Close();
                }*/
            }
            else if (arg1 == ((int)SDMS.Data.ReportMode.ProcessFire).ToString())
            {
                CreateHWP(Application.StartupPath + "\\report\\처리 이력 보고서.hwp", 15, 10, 10, Application.StartupPath + "\\report\\Malfunction.bmp", 7, 8, 8, 12);
                /*LoadFile(Application.StartupPath + "\\report\\처리 이력 보고서.hwp");

                SetBookMarkPosition("Begin Table");

                //현재 위치한 커서값을 받아온다.
                int nPosition = 0, para = 0, pos = 0;
                this.axHwpCtrl1.GetPos(ref nPosition, ref para, ref pos);

                //예제에있는 표의 줄 삭제
                for (int j = 0; j < 15; j++)
                {
                    TableDeleteRow(nPosition + 10, 0, 0);
                }

                TableAppandRow(nPosition, 0, 0);

                string line2;
                int Arraycount = 0;
                using (StreamReader sr = new StreamReader(Application.StartupPath + "\\report\\SaveData.txt", System.Text.Encoding.UTF8))
                {
                    while ((line2 = sr.ReadLine()) != null)
                    {
                        //데이터 수에 맞춰서 줄 늘림
                        if (Arraycount != 0)
                        {
                            if (Arraycount % 10 == 0)
                            {
                                TableAppandRow(nPosition, 0, 0);
                            }
                        }

                        InsertText(nPosition + 10, 0, 0, line2);
                        //Console.WriteLine(strArray[count]);

                        //표 배경색깔 변경
                        CellBlock(nPosition + 10, 0, 0);
                        ChangeTableProperty();

                        //글씨 굵기 없애기
                        HWPCONTROLLib.DHwpAction ac3 = (HWPCONTROLLib.DHwpAction)this.axHwpCtrl1.CreateAction("CharShape");
                        HWPCONTROLLib.DHwpParameterSet cs3 = (HWPCONTROLLib.DHwpParameterSet)ac3.CreateSet();
                        ac3.GetDefault(cs3);

                        cs3.SetItem("Bold", 0);
                        //cs3.SetItem("TextColor", 0xFF0000);

                        ac3.Execute(cs3);

                        axHwpCtrl1.Run("Cancel");

                        Arraycount++;
                        nPosition++;
                    }
                    sr.Close();
                }


                //원래 있던 이미지 삭제
                axHwpCtrl1.SetPos(0, 14, 0);
                axHwpCtrl1.Run("Select");
                axHwpCtrl1.Run("Select");
                axHwpCtrl1.Run("Delete");

                //새 이미지 삽입
                axHwpCtrl1.InsertPicture(Application.StartupPath + "\\report\\Malfunction.bmp", 1, 1, 0, 0, 0, 150, 45);



                string line;
                int lineNum = 9;
                using (StreamReader sr = new StreamReader(Application.StartupPath + "\\report\\SaveDateTime.txt", System.Text.Encoding.UTF8))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        DeleteText(0, lineNum, 8);
                        InsertText(0, lineNum, 8, line);

                        lineNum++;
                    }
                    sr.Close();
                }*/
            }
            else if (arg1 == ((int)SDMS.Data.ReportMode.ActionFire).ToString())//대응
            {
                CreateHWP(Application.StartupPath + "\\report\\대응 이력 보고서.hwp", 4, 4, 4, null, 6, 6, 8);
                /*LoadFile(Application.StartupPath + "\\report\\대응 이력 보고서.hwp");

                SetBookMarkPosition("Begin Table");

                //현재 위치한 커서값을 받아온다.
                int nPosition = 0, para = 0, pos = 0;
                this.axHwpCtrl1.GetPos(ref nPosition, ref para, ref pos);

                //예제에있는 표의 줄 삭제
                for (int j = 0; j < 14; j++)
                {
                    TableDeleteRow(nPosition + 6, 0, 0);
                }

                TableAppandRow(nPosition, 0, 0);


                string line2;
                int Arraycount = 0;
                using (StreamReader sr = new StreamReader(Application.StartupPath + "\\report\\SaveData.txt", System.Text.Encoding.UTF8))
                {
                    while ((line2 = sr.ReadLine()) != null)
                    {
                        //데이터 수에 맞춰서 줄 늘림
                        if (Arraycount != 0)
                        {
                            if (Arraycount % 6 == 0)
                            {
                                TableAppandRow(nPosition, 0, 0);
                            }
                        }

                        InsertText(nPosition + 6, 0, 0, line2);
                        //Console.WriteLine(strArray[count]);

                        //표 배경색깔 변경
                        CellBlock(nPosition + 6, 0, 0);
                        ChangeTableProperty();

                        //글씨 굵기 없애기
                        HWPCONTROLLib.DHwpAction ac3 = (HWPCONTROLLib.DHwpAction)this.axHwpCtrl1.CreateAction("CharShape");
                        HWPCONTROLLib.DHwpParameterSet cs3 = (HWPCONTROLLib.DHwpParameterSet)ac3.CreateSet();
                        ac3.GetDefault(cs3);

                        cs3.SetItem("Bold", 0);
                        //cs3.SetItem("TextColor", 0xFF0000);

                        ac3.Execute(cs3);

                        axHwpCtrl1.Run("Cancel");


                        Arraycount++;
                        nPosition++;
                    }
                    sr.Close();
                }


                string line;
                int lineNum = 7;
                using (StreamReader sr = new StreamReader(Application.StartupPath + "\\report\\SaveDateTime.txt", System.Text.Encoding.UTF8))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (lineNum == 7)
                        {
                            DeleteText(0, lineNum, 6);
                            InsertText(0, lineNum, 6, line);
                        }
                        else
                        {
                            DeleteText(0, lineNum, 8);
                            InsertText(0, lineNum, 8, line);
                        }
                        lineNum++;
                    }
                    sr.Close();
                }*/
            }
            else if (arg1 == ((int)SDMS.Data.ReportMode.DetectPSMAnalyze).ToString())
            {
                CreateParetoPSM(Application.StartupPath + "\\report\\누출 탐지분석 보고서.hwp");
            }
            else if (arg1 == ((int)SDMS.Data.ReportMode.DetectPSM).ToString()) // 누출 탐지
            {
                CreateHWP(Application.StartupPath + "\\report\\누출 탐지 보고서.hwp", 7, 8, 8, Application.StartupPath + "\\report\\Detect.bmp", 8, 8, 8);
                /*LoadFile(Application.StartupPath + "\\report\\누출 탐지 보고서.hwp");

                SetBookMarkPosition("Begin Table");

                //현재 위치한 커서값을 받아온다.
                int nPosition = 0, para = 0, pos = 0;
                this.axHwpCtrl1.GetPos(ref nPosition, ref para, ref pos);

                //예제에있는 표의 줄 삭제
                for (int j = 0; j < 15; j++)
                {
                    TableDeleteRow(nPosition + 7, 0, 0);
                }

                TableAppandRow(nPosition, 0, 0);

                string line2;
                int Arraycount = 0;

                using (StreamReader sr = new StreamReader(Application.StartupPath + "\\report\\SaveData.txt", System.Text.Encoding.UTF8))
                {
                    while ((line2 = sr.ReadLine()) != null)
                    {
                        //데이터 수에 맞춰서 줄 늘림
                        if (Arraycount != 0)
                        {
                            if (Arraycount % 6 == 0)
                            {
                                TableAppandRow(nPosition, 0, 0);
                            }
                        }

                        InsertText(nPosition + 6, 0, 0, line2);
                        //Console.WriteLine(strArray[count]);

                        //표 배경색깔 변경
                        CellBlock(nPosition + 6, 0, 0);
                        ChangeTableProperty();

                        //글씨 굵기 없애기
                        HWPCONTROLLib.DHwpAction ac3 = (HWPCONTROLLib.DHwpAction)this.axHwpCtrl1.CreateAction("CharShape");
                        HWPCONTROLLib.DHwpParameterSet cs3 = (HWPCONTROLLib.DHwpParameterSet)ac3.CreateSet();
                        ac3.GetDefault(cs3);

                        cs3.SetItem("Bold", 0);
                        //cs3.SetItem("TextColor", 0xFF0000);

                        ac3.Execute(cs3);

                        axHwpCtrl1.Run("Cancel");


                        Arraycount++;
                        nPosition++;
                    }
                    sr.Close();
                }

                //원래 있던 이미지 삭제
                axHwpCtrl1.SetPos(0, 14, 0);
                axHwpCtrl1.Run("Select");
                axHwpCtrl1.Run("Select");
                axHwpCtrl1.Run("Delete");

                //새 이미지 삽입
                axHwpCtrl1.InsertPicture(Application.StartupPath + "\\report\\Detect.bmp", 1, 1, 0, 0, 0, 150, 45);


                //string strSaveFilePath = "C:\\Save.txt"; // 파일의 경로입니다.
                //StreamReader SRead = new StreamReader(strSaveFilePath, System.Text.Encoding.UTF8);
                //string strFileLine = SRead.ReadLine();

                //for (int i = 0; i < strArray.Length; i++)
                //{
                //    //Console.Write(strArray[i]); // 읽어온 문자열을 뿌립니다.
                //    Console.WriteLine(strArray[i]);
                //}


                string line;
                int lineNum = 8;
                using (StreamReader sr = new StreamReader(Application.StartupPath + "\\report\\SaveDateTime.txt", System.Text.Encoding.UTF8))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        DeleteText(0, lineNum, 8);
                        InsertText(0, lineNum, 8, line);

                        lineNum++;
                    }
                    sr.Close();
                }*/
            }
            else if (arg1 == ((int)SDMS.Data.ReportMode.ProcessPSM).ToString()) // 누출 처리
            {
                CreateHWP(Application.StartupPath + "\\report\\누출 처리 이력 보고서.hwp", 15, 8, 8, Application.StartupPath + "\\report\\Malfunction.bmp", 7, 8, 8, 12);
                /*LoadFile(Application.StartupPath + "\\report\\누출 처리 이력 보고서.hwp");

                SetBookMarkPosition("Begin Table");

                //현재 위치한 커서값을 받아온다.
                int nPosition = 0, para = 0, pos = 0;
                this.axHwpCtrl1.GetPos(ref nPosition, ref para, ref pos);

                //예제에있는 표의 줄 삭제
                for (int j = 0; j < 15; j++)
                {
                    TableDeleteRow(nPosition + 8, 0, 0);
                }

                TableAppandRow(nPosition, 0, 0);

                string line2;
                int Arraycount = 0;
                using (StreamReader sr = new StreamReader(Application.StartupPath + "\\report\\SaveData.txt", System.Text.Encoding.UTF8))
                {
                    while ((line2 = sr.ReadLine()) != null)
                    {
                        //데이터 수에 맞춰서 줄 늘림
                        if (Arraycount != 0)
                        {
                            if (Arraycount % 8 == 0)
                            {
                                TableAppandRow(nPosition, 0, 0);
                            }
                        }

                        InsertText(nPosition + 8, 0, 0, line2);
                        //Console.WriteLine(strArray[count]);

                        //표 배경색깔 변경
                        CellBlock(nPosition + 8, 0, 0);
                        ChangeTableProperty();

                        //글씨 굵기 없애기
                        HWPCONTROLLib.DHwpAction ac3 = (HWPCONTROLLib.DHwpAction)this.axHwpCtrl1.CreateAction("CharShape");
                        HWPCONTROLLib.DHwpParameterSet cs3 = (HWPCONTROLLib.DHwpParameterSet)ac3.CreateSet();
                        ac3.GetDefault(cs3);

                        cs3.SetItem("Bold", 0);
                        //cs3.SetItem("TextColor", 0xFF0000);

                        ac3.Execute(cs3);

                        axHwpCtrl1.Run("Cancel");

                        Arraycount++;
                        nPosition++;
                    }
                    sr.Close();
                }


                //원래 있던 이미지 삭제
                axHwpCtrl1.SetPos(0, 14, 0);
                axHwpCtrl1.Run("Select");
                axHwpCtrl1.Run("Select");
                axHwpCtrl1.Run("Delete");

                //새 이미지 삽입
                axHwpCtrl1.InsertPicture(Application.StartupPath + "\\report\\Malfunction.bmp", 1, 1, 0, 0, 0, 150, 45);



                string line;
                int lineNum = 9;
                using (StreamReader sr = new StreamReader(Application.StartupPath + "\\report\\SaveDateTime.txt", System.Text.Encoding.UTF8))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        DeleteText(0, lineNum, 8);
                        InsertText(0, lineNum, 8, line);

                        lineNum++;
                    }
                    sr.Close();
                }*/
            }
            else if (arg1 == ((int)SDMS.Data.ReportMode.ActionPSM).ToString())//대응
            {
                CreateHWP(Application.StartupPath + "\\report\\누출 대응 이력 보고서.hwp", 4, 5, 5, null, 7, 8, 8);
                /*LoadFile(Application.StartupPath + "\\report\\누출 대응 이력 보고서.hwp");

                SetBookMarkPosition("Begin Table");

                //현재 위치한 커서값을 받아온다.
                int nPosition = 0, para = 0, pos = 0;
                this.axHwpCtrl1.GetPos(ref nPosition, ref para, ref pos);

                //예제에있는 표의 줄 삭제
                for (int j = 0; j < 14; j++)
                {
                    TableDeleteRow(nPosition + 8, 0, 0);
                }

                TableAppandRow(nPosition, 0, 0);


                string line2;
                int Arraycount = 0;
                using (StreamReader sr = new StreamReader(Application.StartupPath + "\\report\\SaveData.txt", System.Text.Encoding.UTF8))
                {
                    while ((line2 = sr.ReadLine()) != null)
                    {
                        //데이터 수에 맞춰서 줄 늘림
                        if (Arraycount != 0)
                        {
                            if (Arraycount % 7 == 0)
                            {
                                TableAppandRow(nPosition, 0, 0);
                            }
                        }

                        InsertText(nPosition + 7, 0, 0, line2);
                        //Console.WriteLine(strArray[count]);

                        //표 배경색깔 변경
                        CellBlock(nPosition + 7, 0, 0);
                        ChangeTableProperty();

                        //글씨 굵기 없애기
                        HWPCONTROLLib.DHwpAction ac3 = (HWPCONTROLLib.DHwpAction)this.axHwpCtrl1.CreateAction("CharShape");
                        HWPCONTROLLib.DHwpParameterSet cs3 = (HWPCONTROLLib.DHwpParameterSet)ac3.CreateSet();
                        ac3.GetDefault(cs3);

                        cs3.SetItem("Bold", 0);
                        //cs3.SetItem("TextColor", 0xFF0000);

                        ac3.Execute(cs3);

                        axHwpCtrl1.Run("Cancel");


                        Arraycount++;
                        nPosition++;
                    }
                    sr.Close();
                }


                string line;
                int lineNum = 7;
                using (StreamReader sr = new StreamReader(Application.StartupPath + "\\report\\SaveDateTime.txt", System.Text.Encoding.UTF8))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        //if (lineNum == 7)
                        //{
                        //    DeleteText(0, lineNum, 6);
                        //    InsertText(0, lineNum, 6, line);
                        //}
                        //else
                        //{
                            DeleteText(0, lineNum, 8);
                            InsertText(0, lineNum, 8, line);
                        //}
                        lineNum++;
                    }
                    sr.Close();
                }*/
            } 
            else if (arg1 == ((int)SDMS.Data.ReportMode.DetectIntrusionAnalyze).ToString())
            {
                CreateParetoIntrusion(Application.StartupPath + "\\report\\방범 탐지분석 보고서.hwp");
            }
            else if (arg1 == ((int)SDMS.Data.ReportMode.DetectIntrusion).ToString())
            {
                CreateHWP(Application.StartupPath + "\\report\\방범 탐지 보고서.hwp", 5, 8, 8, Application.StartupPath + "\\report\\Detect.bmp", 8, 8, 8);
                /*LoadFile(Application.StartupPath + "\\report\\화재 탐지 보고서.hwp");

                SetBookMarkPosition("Begin Table");

                //현재 위치한 커서값을 받아온다.
                int nPosition = 0, para = 0, pos = 0;
                this.axHwpCtrl1.GetPos(ref nPosition, ref para, ref pos);

                //예제에있는 표의 줄 삭제
                for (int j = 0; j < 15; j++)
                {
                    TableDeleteRow(nPosition + 7, 0, 0);
                }

                TableAppandRow(nPosition, 0, 0);

                string line2;
                int Arraycount = 0;

                using (StreamReader sr = new StreamReader(Application.StartupPath + "\\report\\SaveData.txt", System.Text.Encoding.UTF8))
                {
                    while ((line2 = sr.ReadLine()) != null)
                    {
                        //데이터 수에 맞춰서 줄 늘림
                        if (Arraycount != 0)
                        {
                            if (Arraycount % 7 == 0)
                            {
                                TableAppandRow(nPosition, 0, 0);
                            }
                        }

                        InsertText(nPosition + 7, 0, 0, line2);
                        //Console.WriteLine(strArray[count]);

                        if (Arraycount < 7)
                        {
                            //표 배경색깔 변경
                            CellBlock(nPosition + 7, 0, 0);
                            ChangeTableProperty();

                            //글씨 굵기 없애기
                            HWPCONTROLLib.DHwpAction ac3 = (HWPCONTROLLib.DHwpAction)this.axHwpCtrl1.CreateAction("CharShape");
                            HWPCONTROLLib.DHwpParameterSet cs3 = (HWPCONTROLLib.DHwpParameterSet)ac3.CreateSet();
                            ac3.GetDefault(cs3);

                            cs3.SetItem("Bold", 0);
                            //cs3.SetItem("TextColor", 0xFF0000);

                            ac3.Execute(cs3);

                            axHwpCtrl1.Run("Cancel");
                        }

                        Arraycount++;
                        nPosition++;
                    }
                    sr.Close();
                }

                //원래 있던 이미지 삭제
                axHwpCtrl1.SetPos(0, 14, 0);
                axHwpCtrl1.Run("Select");
                axHwpCtrl1.Run("Select");
                axHwpCtrl1.Run("Delete");

                //새 이미지 삽입
                axHwpCtrl1.InsertPicture(Application.StartupPath + "\\report\\Detect.bmp", 1, 1, 0, 0, 0, 150, 45);

                //string strSaveFilePath = "C:\\Save.txt"; // 파일의 경로입니다.
                //StreamReader SRead = new StreamReader(strSaveFilePath, System.Text.Encoding.UTF8);
                //string strFileLine = SRead.ReadLine();

                //for (int i = 0; i < strArray.Length; i++)
                //{
                //    //Console.Write(strArray[i]); // 읽어온 문자열을 뿌립니다.
                //    Console.WriteLine(strArray[i]);
                //}

                string line;
                int lineNum = 8;
                using (StreamReader sr = new StreamReader(Application.StartupPath + "\\report\\SaveDateTime.txt", System.Text.Encoding.UTF8))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        DeleteText(0, lineNum, 8);
                        InsertText(0, lineNum, 8, line);

                        lineNum++;
                    }
                    sr.Close();
                }*/
            }
            else if (arg1 == ((int)SDMS.Data.ReportMode.ProcessIntrusion).ToString())
            {
                CreateHWP(Application.StartupPath + "\\report\\방범 처리 이력 보고서.hwp", 15, 10, 10, Application.StartupPath + "\\report\\Malfunction.bmp", 7, 8, 8, 12);
                /*LoadFile(Application.StartupPath + "\\report\\처리 이력 보고서.hwp");

                SetBookMarkPosition("Begin Table");

                //현재 위치한 커서값을 받아온다.
                int nPosition = 0, para = 0, pos = 0;
                this.axHwpCtrl1.GetPos(ref nPosition, ref para, ref pos);

                //예제에있는 표의 줄 삭제
                for (int j = 0; j < 15; j++)
                {
                    TableDeleteRow(nPosition + 10, 0, 0);
                }

                TableAppandRow(nPosition, 0, 0);

                string line2;
                int Arraycount = 0;
                using (StreamReader sr = new StreamReader(Application.StartupPath + "\\report\\SaveData.txt", System.Text.Encoding.UTF8))
                {
                    while ((line2 = sr.ReadLine()) != null)
                    {
                        //데이터 수에 맞춰서 줄 늘림
                        if (Arraycount != 0)
                        {
                            if (Arraycount % 10 == 0)
                            {
                                TableAppandRow(nPosition, 0, 0);
                            }
                        }

                        InsertText(nPosition + 10, 0, 0, line2);
                        //Console.WriteLine(strArray[count]);

                        //표 배경색깔 변경
                        CellBlock(nPosition + 10, 0, 0);
                        ChangeTableProperty();

                        //글씨 굵기 없애기
                        HWPCONTROLLib.DHwpAction ac3 = (HWPCONTROLLib.DHwpAction)this.axHwpCtrl1.CreateAction("CharShape");
                        HWPCONTROLLib.DHwpParameterSet cs3 = (HWPCONTROLLib.DHwpParameterSet)ac3.CreateSet();
                        ac3.GetDefault(cs3);

                        cs3.SetItem("Bold", 0);
                        //cs3.SetItem("TextColor", 0xFF0000);

                        ac3.Execute(cs3);

                        axHwpCtrl1.Run("Cancel");

                        Arraycount++;
                        nPosition++;
                    }
                    sr.Close();
                }


                //원래 있던 이미지 삭제
                axHwpCtrl1.SetPos(0, 14, 0);
                axHwpCtrl1.Run("Select");
                axHwpCtrl1.Run("Select");
                axHwpCtrl1.Run("Delete");

                //새 이미지 삽입
                axHwpCtrl1.InsertPicture(Application.StartupPath + "\\report\\Malfunction.bmp", 1, 1, 0, 0, 0, 150, 45);



                string line;
                int lineNum = 9;
                using (StreamReader sr = new StreamReader(Application.StartupPath + "\\report\\SaveDateTime.txt", System.Text.Encoding.UTF8))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        DeleteText(0, lineNum, 8);
                        InsertText(0, lineNum, 8, line);

                        lineNum++;
                    }
                    sr.Close();
                }*/
            }
            else if (arg1 == ((int)SDMS.Data.ReportMode.ActionIntrusion).ToString())//대응
            {
                CreateHWP(Application.StartupPath + "\\report\\방범 대응 이력 보고서.hwp", 4, 4, 4, null, 6, 6, 8);
                /*LoadFile(Application.StartupPath + "\\report\\대응 이력 보고서.hwp");

                SetBookMarkPosition("Begin Table");

                //현재 위치한 커서값을 받아온다.
                int nPosition = 0, para = 0, pos = 0;
                this.axHwpCtrl1.GetPos(ref nPosition, ref para, ref pos);

                //예제에있는 표의 줄 삭제
                for (int j = 0; j < 14; j++)
                {
                    TableDeleteRow(nPosition + 6, 0, 0);
                }

                TableAppandRow(nPosition, 0, 0);


                string line2;
                int Arraycount = 0;
                using (StreamReader sr = new StreamReader(Application.StartupPath + "\\report\\SaveData.txt", System.Text.Encoding.UTF8))
                {
                    while ((line2 = sr.ReadLine()) != null)
                    {
                        //데이터 수에 맞춰서 줄 늘림
                        if (Arraycount != 0)
                        {
                            if (Arraycount % 6 == 0)
                            {
                                TableAppandRow(nPosition, 0, 0);
                            }
                        }

                        InsertText(nPosition + 6, 0, 0, line2);
                        //Console.WriteLine(strArray[count]);

                        //표 배경색깔 변경
                        CellBlock(nPosition + 6, 0, 0);
                        ChangeTableProperty();

                        //글씨 굵기 없애기
                        HWPCONTROLLib.DHwpAction ac3 = (HWPCONTROLLib.DHwpAction)this.axHwpCtrl1.CreateAction("CharShape");
                        HWPCONTROLLib.DHwpParameterSet cs3 = (HWPCONTROLLib.DHwpParameterSet)ac3.CreateSet();
                        ac3.GetDefault(cs3);

                        cs3.SetItem("Bold", 0);
                        //cs3.SetItem("TextColor", 0xFF0000);

                        ac3.Execute(cs3);

                        axHwpCtrl1.Run("Cancel");


                        Arraycount++;
                        nPosition++;
                    }
                    sr.Close();
                }


                string line;
                int lineNum = 7;
                using (StreamReader sr = new StreamReader(Application.StartupPath + "\\report\\SaveDateTime.txt", System.Text.Encoding.UTF8))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (lineNum == 7)
                        {
                            DeleteText(0, lineNum, 6);
                            InsertText(0, lineNum, 6, line);
                        }
                        else
                        {
                            DeleteText(0, lineNum, 8);
                            InsertText(0, lineNum, 8, line);
                        }
                        lineNum++;
                    }
                    sr.Close();
                }*/
            }
            else if (arg1 == ((int)SDMS.Data.ReportMode.DisasterPrevention).ToString())
            {
                //CreateDisasterPrevention(Application.StartupPath + "\\report\\방재장비.hwp");
                CreateHWP(Application.StartupPath + "\\report\\방재장비.hwp", 8, 10, 10, null, 0,0,0,0);
            }

            Console.WriteLine("한글파일저장");
            if (arg2.Length > 0)
            {
                this.axHwpCtrl1.SaveAs(arg2);
            }

            Console.WriteLine("컨트롤삭제");

            CloseForm();
        }

        //북마크 위치로 이동
        public void SetBookMarkPosition(string strMarkName)
        {
            HWPCONTROLLib.DHwpAction ac = (HWPCONTROLLib.DHwpAction)this.axHwpCtrl1.CreateAction("Bookmark");
            HWPCONTROLLib.DHwpParameterSet cs = (HWPCONTROLLib.DHwpParameterSet)ac.CreateSet();
            cs.SetItem("Name", strMarkName);
            cs.SetItem("Type", 0);
            cs.SetItem("Command", 1);
            ac.Execute(cs);
        }
        
        //한글문서 불러오기
        public void LoadFile(string strPath)
        {
            this.axHwpCtrl1.RegisterModule("FilePathCheckDLL", "FilePathCheckerModule");
            this.axHwpCtrl1.Open(strPath);
        }

        public void InsertText(int a, int b, int c, string str)
        {
            axHwpCtrl1.SetPos(a, b, c);
            HWPCONTROLLib.DHwpAction ac = (HWPCONTROLLib.DHwpAction)axHwpCtrl1.CreateAction("InsertText");
            HWPCONTROLLib.DHwpParameterSet cs = (HWPCONTROLLib.DHwpParameterSet)ac.CreateSet();
            cs.SetItem("Text", str);

            ac.Execute(cs);
        }

        public void DeleteText(int a, int b, int c)
        {
            axHwpCtrl1.SetPos(a, b, c);
            HWPCONTROLLib.DHwpAction ac = (HWPCONTROLLib.DHwpAction)axHwpCtrl1.CreateAction("DeleteLineEnd");
            HWPCONTROLLib.DHwpParameterSet cs = (HWPCONTROLLib.DHwpParameterSet)ac.CreateSet();

            ac.Execute(cs);
        }

        public void TableAppandRow(int a, int b, int c)
        {
            axHwpCtrl1.SetPos(a, b, c);

            HWPCONTROLLib.DHwpAction ac = (HWPCONTROLLib.DHwpAction)axHwpCtrl1.CreateAction("TableAppendRow");
            HWPCONTROLLib.DHwpParameterSet cs = (HWPCONTROLLib.DHwpParameterSet)ac.CreateSet();

            ac.Execute(cs);
        }

        public void TableDeleteRow(int a, int b, int c)
        {
            axHwpCtrl1.SetPos(a, b, c);
            HWPCONTROLLib.DHwpAction ac = (HWPCONTROLLib.DHwpAction)axHwpCtrl1.CreateAction("TableDeleteRow");
            HWPCONTROLLib.DHwpParameterSet cs = (HWPCONTROLLib.DHwpParameterSet)ac.CreateSet();

            ac.Execute(cs);
        }

        public void CellBlock(int a, int b, int c)
        {
            axHwpCtrl1.SetPos(a, b, c);
            HWPCONTROLLib.DHwpAction ac = (HWPCONTROLLib.DHwpAction)axHwpCtrl1.CreateAction("TableCellBlock");
            HWPCONTROLLib.DHwpParameterSet cs = (HWPCONTROLLib.DHwpParameterSet)ac.CreateSet();

            ac.Execute(cs);
        }

        public void BackSpace(int a, int b, int c)
        {
            axHwpCtrl1.SetPos(a, b, c);
            HWPCONTROLLib.DHwpAction ac = (HWPCONTROLLib.DHwpAction)axHwpCtrl1.CreateAction("DeleteBack");
            HWPCONTROLLib.DHwpParameterSet cs = (HWPCONTROLLib.DHwpParameterSet)ac.CreateSet();

            ac.Execute(cs);
        }

        private int ColorToBGR(Color color)
        {
            int bgr = (((int)color.B) << 16) | (((int)color.G) << 8) | ((int)color.R);
            return bgr;
        }

        public void ChangeTableProperty()
        {
            //HWPCONTROLLib.DHwpAction ac = (HWPCONTROLLib.DHwpAction)this.axHwpCtrl1.CreateAction("CellZoneFill");
            HWPCONTROLLib.DHwpAction ac = (HWPCONTROLLib.DHwpAction)axHwpCtrl1.CreateAction("CellBorderFill");
            HWPCONTROLLib.DHwpParameterSet cs = (HWPCONTROLLib.DHwpParameterSet)ac.CreateSet();

            ac.GetDefault(cs);

            HWPCONTROLLib.DHwpParameterSet subset = (HWPCONTROLLib.DHwpParameterSet)cs.Item("FillAttr");
            if (subset != null)
            {
                subset.SetItem("Type", 1);
                subset.SetItem("WinBrushFaceStyle", 6);
                subset.SetItem("WinBrushFaceColor", ColorToBGR(Color.White));

                //HWPCONTROLLib.DHwpParameterSet cs2 = (HWPCONTROLLib.DHwpParameterSet)cs.CreateItemSet("CellFill", "CellBorderFill");
                //cs.SetItem("Type", 1);
                //cs.SetItem("WinBrushFaceColor", Color.Red.ToArgb());
                //ac.Execute(cs2);
                ac.Execute(cs);
            }
        }

        private void CloseForm()
        {
            button1_Click(null, null);
            System.Threading.Thread t = new System.Threading.Thread(ExitMain);
            t.Start();
        }

        static void ExitMain()
        {
            System.Threading.Thread.Sleep(1000);
            //Application.Exit();
            System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess();

            if (process != null)
                process.Kill();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Controls.Remove(this.axHwpCtrl1);
            this.axHwpCtrl1.Dispose();
            this.axHwpCtrl1 = null;
        }
    }
}

