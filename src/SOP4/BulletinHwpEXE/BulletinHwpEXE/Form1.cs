using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace BulletinHwpEXE
{
    public partial class Form1 : Form
    {
        private string m_strLogoName = string.Empty;
        private int m_nSiteID = 0;
        public Form1()
        {
            InitializeComponent();

            this.Size = new Size(100, 100);
            this.Location = new Point(-200, 0);

            CreateHWP();
        }

        public void CreateHWP()
        {
            string[] strArg = Environment.GetCommandLineArgs();
            if (strArg.Length < 2)
                return;

            string arg1 = strArg[1];
            string arg2 = (strArg.Length > 2) ? strArg[2] : string.Empty;
            int arg3 = (strArg.Length > 3) ? Convert.ToInt32(strArg[3]) : 0;

            m_strLogoName = arg2;
            m_nSiteID = arg3;

            LoadFile(Application.StartupPath + "\\상황판 한글파일.hwp");

            #region 머리말
            if (!string.IsNullOrEmpty(m_strLogoName))
            { 
                SetBookMarkPosition("HeadArea");
                FileInfo f = new FileInfo(Application.StartupPath + "\\" + m_strLogoName);
                if (f.Exists)
                    axHwpCtrl1.InsertPicture(Application.StartupPath + "\\" + m_strLogoName, 1, 1, 0, 0, 0, (m_nSiteID == 100) ? 25 : 30, (m_nSiteID == 100) ? 5 : 5);
            }
            #endregion 

            SetBookMarkPosition("Begin Table2");

            //현재 위치한 커서값을 받아온다.
            int nPosition = 0, para = 0, pos = 0;
            this.axHwpCtrl1.GetPos(ref nPosition, ref para, ref pos);

            //예제에있는 표의 줄 삭제
            for (int j = 0; j < 11; j++)
            {
                TableDeleteRow(nPosition + 6, 0, 0);
            }

            string line2;
            int Arraycount = 0;
            using (StreamReader sr = new StreamReader(Application.StartupPath + "\\BulletHwpDetailData.txt", System.Text.Encoding.UTF8))
            {
                while ((line2 = sr.ReadLine()) != null)
                {
                    //line2.Replace(((char)0x03).ToString(), ((char)0x00).ToString());
                    line2 = line2.Replace(((char)0x02).ToString(), "\r");
                    line2 = line2.Replace(((char)0x03).ToString(), "\n");
                    if (line2 == "-----문단구분-----")
                        continue;

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

            SetBookMarkPosition("Begin Table");

            //현재 위치한 커서값을 받아온다.
            this.axHwpCtrl1.GetPos(ref nPosition, ref para, ref pos);

            //예제에있는 표의 줄 삭제
            for (int j = 0; j < 11; j++)
            {
                TableDeleteRow(nPosition + 6, 0, 0);
            }

            //TableAppandRow(nPosition, 0, 0);

            line2 = "";
            Arraycount = 0;
            using (StreamReader sr = new StreamReader(Application.StartupPath + "\\BulletHwpAllData.txt", System.Text.Encoding.UTF8))
            {
                while ((line2 = sr.ReadLine()) != null)
                {
                    //line2.Replace(((char)0x03).ToString(), ((char)0x00).ToString());
                    line2 = line2.Replace(((char)0x02).ToString(), "\r");
                    line2 = line2.Replace( ((char)0x03).ToString(), "\n");
                    if (line2 == "-----문단구분-----")
                        continue;

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
            int nCount = 0;

            int[] arrSpaceNum = {10, 10, 13, 13, 12, 10};

            using (StreamReader sr = new StreamReader(Application.StartupPath + "\\BulletHwpData.txt", System.Text.Encoding.UTF8))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    DeleteText(0, lineNum, arrSpaceNum[nCount]);
                    InsertText(0, lineNum, arrSpaceNum[nCount], line);
 
                    lineNum++;
                    nCount++;

                    if (nCount >= arrSpaceNum.Count())
                        break;
                }
                sr.Close();
            }

            Console.WriteLine("한글파일저장");
            if (arg1.Length > 0)
            {
                this.axHwpCtrl1.SaveAs(arg1);

                WriteResultFile(1);
            }

            Console.WriteLine("컨트롤삭제");
        }

        private void WriteResultFile(int nResult)
        {
            StreamWriter writer = new StreamWriter(Application.StartupPath + "\\BulletinResult.txt");
            writer.Write(nResult);
            writer.Close();
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

        private void button1_Click(object sender, EventArgs e)
        {
            this.Controls.Remove(this.axHwpCtrl1);
            this.axHwpCtrl1.Dispose();
            this.axHwpCtrl1 = null;
        }
    }
}

