using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Threading;

namespace HwpEXEproject
{

    public partial class Form1 : Form
    {
       
        public Form1()
        {
      
            InitializeComponent();
            axHwpCtrl1.CreateControl();
        }
        
        public void CreateHWP()
        {
            //MessageBox.Show("ddd");
            Console.WriteLine(Application.StartupPath);
            string[] strArg = Environment.GetCommandLineArgs();
            if (strArg.Length < 3)
                return;

            string arg1 = strArg[1];
            string arg2 = strArg[2];

            Console.WriteLine(strArg[0]);
            Console.WriteLine(arg1);
            Console.WriteLine(arg2);
            Console.WriteLine(";;;");

            if (arg1 == "1")//1=화재 2=처리 3=대응
            {
                LoadFile(Application.StartupPath +"\\report\\화재 탐지 보고서.hwp");

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
                }

                
            }
            else if (arg1 == "2")
            {
                LoadFile(Application.StartupPath + "\\report\\처리 이력 보고서.hwp");

                SetBookMarkPosition("Begin Table");

                //현재 위치한 커서값을 받아온다.
                int nPosition = 0, para = 0, pos = 0;
                this.axHwpCtrl1.GetPos(ref nPosition, ref para, ref pos);

                //예제에있는 표의 줄 삭제
                for (int j = 0; j < 15; j++)
                {
                    TableDeleteRow(nPosition+11, 0, 0);
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
                            if (Arraycount % 11 == 0)
                            {
                                TableAppandRow(nPosition, 0, 0);
                            }
                        }

                        InsertText(nPosition+11, 0, 0, line2);
                        //Console.WriteLine(strArray[count]);

                        //표 배경색깔 변경
                        CellBlock(nPosition+11, 0, 0);
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
                }
            }

            else//대응
            {
                LoadFile(Application.StartupPath + "\\report\\대응 이력 보고서.hwp");

                SetBookMarkPosition("Begin Table");

                //현재 위치한 커서값을 받아온다.
                int nPosition = 0, para = 0, pos = 0;
                this.axHwpCtrl1.GetPos(ref nPosition, ref para, ref pos);

                //예제에있는 표의 줄 삭제
                for (int j = 0; j < 14; j++)
                {
                    TableDeleteRow(nPosition+6, 0, 0);
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

                        InsertText(nPosition+6, 0, 0, line2);
                        //Console.WriteLine(strArray[count]);

                        //표 배경색깔 변경
                        CellBlock(nPosition+6, 0, 0);
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
                }
            }
            Console.WriteLine("한글파일저장");
            if (arg2.Length > 0)
            {
               this.axHwpCtrl1.SaveAs(arg2);
            }

            Console.WriteLine("컨트롤삭제");
           
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

        private void Form1_Load(object sender, EventArgs e)
        {
			Application.Exit();
        }
    }
}
