using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.Util;

namespace EmbeddingUnity
{
    public partial class FormMain : Form
    {
        private Panel4Unity unityOutPanel = new Panel4Unity();
        private Panel4Unity unityInPanel = new Panel4Unity();

        private Panel4Unity mCurrentPane = null;

        public FormMain()
        {
            InitializeComponent();

            unityOutPanel.Dock = DockStyle.Fill;
            splitContainer1.Panel1.Controls.Add(unityOutPanel);

            unityInPanel.NamedPipeName = "TestPipeIn";
            unityInPanel.UnityExePath = @"C:\UNE\bin\common12\UnitySamInside.exe";
            unityInPanel.UnityWndName = "UnitySamInside";
            unityInPanel.Dock = DockStyle.Fill;
            splitContainer1.Panel2.Controls.Add(unityInPanel);


            mCurrentPane = unityInPanel;
            

            //splitContainer1.MouseDown += splitContainer1_MouseDown;
            //splitContainer1.MouseUp += splitContainer1_MouseUp;
            //splitContainer1.MouseMove += splitContainer1_MouseMove;

        }


        private void FormMain_Load(object sender, EventArgs e)
        {
            unityInPanel.PopupMenu = contextMenuStrip2;
            unityInPanel.BeginUnity(OnReady);
            unityOutPanel.PopupMenu = contextMenuStrip1;
            unityOutPanel.BeginUnity(OnReady);

            LoadData();

            LoadCombobox();

            InitComboBox();
        }

        public void LoadData()
        {
            SDMS.ZoneManager.Instance.LoadBuildingData();
            SDMS.ZoneManager.Instance.LoadZones();
            SDMS.ZoneManager.Instance.LoadEquipmentZone();
            SDMS.ZoneManager.Instance.Load3DText();
        }

        private void InitComboBox()
        {
            List<SDMS.Building> arBuildings = new List<SDMS.Building>(SDMS.ZoneManager.Instance.DicBuildings.Values);

            foreach (SDMS.Building building in arBuildings)
            {
                comboBox3.Items.Add(building);
            }
            comboBox3.SelectedItem = comboBox3.Items[0];
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            object item = comboBox3.SelectedItem;
            if( item != null)
            {                
                SDMS.Building building = (SDMS.Building)item;
                comboBox4.Tag = building;
                mSelectBuilding = building;
                comboBox4.Items.Clear();
                foreach (SDMS.Zone floor in building.FloorList)
                {                    
                    comboBox4.Items.Add(floor);
                }                
            }
        }

        private SDMS.Zone mSelectZone = null;
        private SDMS.Building mSelectBuilding = null;

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            object item = comboBox4.SelectedItem;
            if (item != null)
            {
                mSelectZone = (SDMS.Zone)item;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if( mSelectZone != null)
            {
                string szIndoorName = null;            

                SDMS.Floor floor = mSelectZone.Floor;
                string szCode = mSelectBuilding.BuildingID;
                if (szCode == "")
                    return;

                // 층은 층인덱스 + 1임
                float nFloor = floor.FloorIndex + 1;
                // 지하층은 인덱스와 같은 값임
                float nBaseFloor = floor.FloorIndex; 

                // 숫자로 시작하는 BuildingCode는 z를 붙여준다.
                if(Char.IsDigit(szCode[0]))
                {
                    szCode = "z" + szCode;
                }
                                    
                // floor가 0보다 작으면 지하층
                if (floor.FloorIndex < 0.0f)
                    szIndoorName = string.Format("{0}_B{1:f1}", szCode, -nBaseFloor);
                else
                    szIndoorName = string.Format("{0}_{1:f1}", szCode, nFloor);

                // .0 으로 끝나는 부분은 삭제한다.
                if (szIndoorName.EndsWith(".0"))
                    szIndoorName = szIndoorName.Substring(0, szIndoorName.Length - 2);
 
                // .2, .5 와 같이 끝나는 경우 M을 붙인다.
                if (szIndoorName[szIndoorName.Length - 2] == '.')
                {
                    szIndoorName += "M";
                } 
               

                unityInPanel.OpenModel(szIndoorName);
            }           
        }

        



        private void button1_Click(object sender, EventArgs e)
        {
            m_nSavedSplitDistance = splitContainer1.SplitterDistance;
            splitContainer1.Panel1Collapsed = true;
            splitContainer1.Panel2Collapsed = false;
        }

        int m_nSavedSplitDistance = 400;
        private void button3_Click(object sender, EventArgs e)
        {
            m_nSavedSplitDistance = splitContainer1.SplitterDistance;
            splitContainer1.Panel1Collapsed = false;
            splitContainer1.Panel2Collapsed = true;
        }


        private void button4_Click(object sender, EventArgs e)
        {
            splitContainer1.SplitterDistance = m_nSavedSplitDistance;
            splitContainer1.Panel1Collapsed = false;
            splitContainer1.Panel2Collapsed = false;
        }


        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            unityOutPanel.StopUnity();
            unityInPanel.StopUnity();
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
        }


        private void rbPick_CheckedChanged(object sender, EventArgs e)
        {
            if(rbPick.Checked == true)
            {
                unityOutPanel.SetPickMode(true);
                unityInPanel.SetPickMode(true);
            }
            else
            {
                unityOutPanel.SetPickMode(false);
                unityInPanel.SetPickMode(false);
            }
        }

        private void rbPan_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void rbOrbit_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void rbNone_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void OnAddedTextPOI(int nID, float x, float y, float z)
        {
            MessageBox.Show("Add Text : " + nID);
        }

        private void OnAddedIconPOI(int nID, float x, float y, float z)
        {
            MessageBox.Show("Add Icon : " + nID);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked == true)
            {
                object item =  cmbIcons.SelectedItem;
                if( item != null)
                {
                    unityOutPanel.IconName = item.ToString();
                    unityInPanel.IconName = item.ToString();
                }
                unityOutPanel.SetIconPickAdd(true, OnAddedIconPOI);
                unityInPanel.SetIconPickAdd(true, OnAddedIconPOI);
            }
            else
            {
                unityOutPanel.SetIconPickAdd(false, null);
                unityInPanel.SetIconPickAdd(false, null);
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                unityOutPanel.PoiText = tbTextPOI.Text;
                unityOutPanel.SetTextPickAdd(true, OnAddedTextPOI);

                unityInPanel.PoiText = tbTextPOI.Text;
                unityInPanel.SetTextPickAdd(true, OnAddedTextPOI);
            }
            else
            {
                unityOutPanel.SetIconPickAdd(false, null);
                unityInPanel.SetIconPickAdd(false, null);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            mCurrentPane.SendCommand(textBox1.Text);
        }

        private void cmbIcons_SelectedIndexChanged(object sender, EventArgs e)
        {
            object item = cmbIcons.SelectedItem;
            if (item != null)
            {
                unityOutPanel.IconName = item.ToString();
                unityInPanel.IconName = item.ToString();    
            }  
        }

        public void OnReady()
        {
            //for (int i = 0; i < m_AliasNames.Length; i += 2)
            //{
            //    string szMeshName = m_AliasNames[i];
            //    string szAliasName = m_AliasNames[i + 1];

            //    unityPanel.AddAliasName(szMeshName, szAliasName);
            //}
            //unityPanel.UpdateAliasNames();
        }


        private void LoadCombobox()
        {
            for (int i = 0; i < m_AliasNames.Length; i += 2)
            {
                string szMeshName = m_AliasNames[i];
                string szAliasName = m_AliasNames[i + 1];

                comboBox1.Items.Add(szAliasName);                
            }            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            object obj = comboBox1.SelectedItem;
            if( obj != null)
            {
                string szAliasName = (string)obj;
                string szMeshName = FindMeshName(szAliasName);
                if( szMeshName != "")
                {
                    ///unityOutPanel.Focus();
                    unityOutPanel.SetZoomObject(szMeshName);
                    unityOutPanel.Focus();
                   // splitContainer1.Refresh();

                }
            }
        }

        private string FindMeshName(string szName)
        {
            for (int i = 0; i < m_AliasNames.Length; i += 2)
            {
                string szMeshName = m_AliasNames[i];
                string szAliasName = m_AliasNames[i + 1];

                if (szName == szAliasName)
                {
                    return szMeshName;
                }
                
            }
            return "";
        }
        
        private string [] m_AliasNames = 
        {
            "z1","터빈건물(1, 2호기)",
            "z1-1","주제어 건물(1, 2호기)",
            "z2","보일러(1호기)",
            "z2-1","보일러(2호기)",
            "z3","수처리실",
            "z4","공작 정비실",
            "z5","비상 펌프실",
            "z6","#1, 2 KPS 정비작업장",
            "z7","#1~4 운탄제어건물",
            "z8","J/H-A, SAMPLE HOUSE",
            "z9","Crusher House",
            "z10","Screen Hoper",
            "z11","J/H-C",
            "z12","J/H-B",
            "z13","J/H-E",
            "z14","J/H-F",
            "z15","J/H-G",
            "z16","J/H-H",
            "z16-1","Tripper Room",
            "z17","한전산업 기계작업장",
            "z18","보조보일러",
            "z19","경영지원처",
            "z20","차고",
            "z21","#1,2 자재창고",
            "z22","중기정비고",
            "z23","#1,2 유치창고",
            "z24","#1,2 가스공급실",
            "z25","온실",
            "z26","정문경비실",
            "z28","방카 A",
            "z28-1","방카 A-1",
            "z28-2","방카 A-2",
            "z31","석탄하역창고",
            "z33","발전운영처",
            "z36","민방공 대피실",
            "z37","테니스장 샤워실",
            "z38","회처리 재순환실",
            "z39","1,2호기 화공약품창고",
            "z41","1,2호기 KPS 전기자재창고",
            "z43","연료설비창고",
            "z45","망루(5만톤 부두)",
            "z45-1","망루 (신부두+구부두접속부)",
            "z46","망루(산악1)",
            "z46-1","망루(산악2)",
            "z46-2","망루 (1회사장입구)",
            "z46-3","망루(방수로입구)",
            "z46-4","망루(종바위)",
            "z46-5","망루(방파제입구)",
            "z47","KPS 기계사무동(1~4호기)",
            "z48","해안분초소",
            "A-1","터빈건물(3,4호기)",
            "A-1-1","주제어 건물(3,4호기)",
            "A-2","보일러(3호기)",
            "A-3","보일러(4호기)",
            "A-4","수처리 건물",
            "A-5","전기 집진기 및 회처리 제어실",
            "A-6","염소 주입실",
            "A-7","폐수 처리실",
            "A-8","유지 창고",
            "A-9","약품 창고/1,2호기 해수전해실",
            "A-10","#3,4 분말 저장용기실",
            "B-56","종합창고(A동)",
            "B-56-1","종합창고(B동)",
            "A-11","발전기술처",
            "A-12","시료 조제실",
            "A-13","망루",
            "B-1","터빈건물(5, 6호기)",
            "B-2","보일러(5호기)",
            "B-2-1","보일러(6호기)",
            "B-3","주제어건물(5,6호기)",
            "B-4","#1~4운탄 J/H-A-1",
            "B-5","#1~4운탄 J/H-D",
            "B-6","염소주입실",
            "B-9","용신건설 작업사무실(#1~4)",
            "B-10","철편분리기 건물",
            "B-11","비회정제실 주제어 건물",
            "B-13","폐기물 보관 창고",
            "B-16","#5,6 운탄제어건물",
            "B-17","#5,6 KPS 공작실 건물",
            "B-18","용신건설 사무동",
            "B-19","보온재 저장창고",
            "B-20","가스 창고",
            "B-21","화공약품 창고",
            "B-22","#3,4 용접작업장",
            "B-23","석탄 취급실(TTT)",
            "B-23-1","석탄 취급실(TR)",
            "B-24","석탄 취급실(TT05)",
            "B-25","석탄 취급실 (B/B)",
            "B-26","석탄 취급실 (C/B)",
            "B-27","석탄 취급실 (TT04)",
            "B-28","석탄 취급실 (TT3C)",
            "B-29","석탄 취급실 (TT3B)",
            "B-30","석탄 취급실 (TT3A)",
            "B-31","석탄 취급실 (TT06)",
            "B-31-1","석탄 취급실 (TT07)",
            "B-32","석탄 취급실 (TT01)",
            "B-33","석탄 취급실 (TT2A)",
            "B-34","석탄 취급실 (TT2B)",
            "B-35","5,6호기 소방펌프실",
            "B-35-1","1,4호기 소방펌프실",
            "B-36","#5,6 주제어 CO2저장용기실",
            "B-37","#5,6 미분기 CO2저장용기실",
            "B-38","가스실-1(5호기)",
            "B-39","가스실-1(6호기)",
            "B-40","세방기업 사무실",
            "B-41","#1~4 KPS 기계 외주사무실",
            "B-42","#1,2 분말저장용기실",
            "B-46","신고성 변전소",
            "B-47","구내 식당",
            "B-48","식당 가스창고",
            "B-49","1,2호기 전기집진기 제어건물",
            "B-53","폐기물 창고",
            "B-54","1MWp 태양광 전기실",
            "B-55","KPS 본관 사무동",
            "F-1","탈황설비 전기 및 전자기기건물",
            "F-2","탈황설비 흡수탑재순환펌프건물(#1,2)",
            "F-3","탈황설비 흡수탑재순환펌프건물(#3,4)",
            "F-4","탈황설비 석회석슬러리제조건물",
            "F-5","탈황설비 석고탈수 및 저장건물",
            "F-6","탈황설비 용수 및 폐수처리건물",
            "F-7","석회석 취급실",
            "F-8","석회석 취급실",
            "F-9","석고 취급실",
            "F-10","석고 취급실",
            "F-11","석고 취급실",
            "F-12","석고 취급실",
            "F-13","석탄 취급실(TT03A)",
            "F-14","석탄 취급실(TT21)",
            "F-15","석탄 취급실(TT22)",
            "FS-1","#1 ABS",
            "FS-2","#2 ABS",
            "FS-3","#3 ABS",
            "FS-4","#4 ABS",
            "FS-5","탈황 재순환 탱크",
            "FS-6","#1,2 ARP Room",
            "FS-7","#3,4 ARP Room",
            "FS-8","탈황 RAW Wator Tank",
            "FS-9","탈황 Filtrate Tank",
            "FS-10","탈황 G/S Tank",
            "FS-11","탈황 Lime Stone Slurry Storage Tank",
            "FS-12","탈황 Lime Stone Storage Silo",
            "G-1","회정제 작업장",
            "H-1","화목정",
            "z57","고압전동기 작업장",
            "z103","암모니아 저장소",
            "z104","공공용시설(전기실, 제어실, 전시실)",
            "z105","탈황·탈질 자재창고",
            "z106","소수력 수차동",
            "z107","소수력 전기실",
            "z108","#1석탄 비상 방지 설비동",
            "z109","#2석탄 비상 방지 설비동",
            "z110","자재창고 태양광발전소",
            "z111","1회사장 태양광발전소",
            "z112","L.O Storage Tank",
            "z113","L.O Daily Tank",
            "z114","#1,2 L.O Burner Pump",
            "z115","#3,4 L.O Burner Pump",
            "z116","1,2호기 수소충진실",
            "z117","1,2호기 CO2 저장실",
            "z118","3호기 수소충진실",
            "z119","4호기 수소충진실",
            "z120","5호기 L.O Storage Tank Dirty/Clean",
            "z121","6호기 L.O Storage Tank Dirty/Clean",
            "z122","지하전력구",
            "TR-1","TR-1",
            "TR-2","TR-2",
            "TR-3","TR-3",
            "TR-4","TR-4",
            "TR-5","TR-5",
            "TR-6","TR-6",
            "z123","공기압축실",
            "z1-E-P","1호기 E-P",
            "z2-E-P","2호기 E-P",
            "z3-E-P","3호기 E-P",
            "z4-E-P","4호기 E-P",
            "z5-E-P","5호기 E-P",
            "z6-E-P","6호기 E-P",
            "C-06","C-06",
            "C-7","C-7",
            "CSU-01A","CSU-01A",
            "CSU-01B","CSU-01B",
            "CU-01A","CU-01A",
            "CU-01B","CU-01B",
            "CV-02","CV-02",
            "CV-03","CV-03",
            "CV-06-1","CV-06-1",
            "CV-06-2","CV-06-2",
            "CV-07","CV-07",
            "CV-08","CV-08",
            "CV-09","CV-09",
            "CV-11","CV-11",
            "CV-12","CV-12",
            "CV-15","CV-15",
            "CV-21","CV-21",
            "CV-22","CV-22",
            "MD_Room","MD Room",
            "RE-01","RE-01",
            "SR-01A","SR-01A",
            "SR-01B","SR-01B"
         };

        private void addTextPOIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Point pt = contextMenuStrip1.Bounds.Location;
            pt = unityOutPanel.PointToClient(pt);
            mCurrentPane.AddTextPOI(tbTextPOI.Text, pt.X, pt.Y);
        }

        private void addIconPOIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            object item = cmbIcons.SelectedItem;
            if (item != null)
            {
                Point pt = contextMenuStrip1.Bounds.Location;
                pt = unityOutPanel.PointToClient(pt);
                mCurrentPane.AddIconPOI(item.ToString(), pt.X, pt.Y);
            }
        }

        private void selectToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void clearSelectToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ColorDialog color = new ColorDialog();
            color.Color = pictureBox1.BackColor;
            if(color.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pictureBox1.BackColor = color.Color;
                mCurrentPane.SetTextColor(color.Color);
            }

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            ColorDialog color = new ColorDialog();
            color.Color = pictureBox2.BackColor;
            if (color.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pictureBox2.BackColor = color.Color;
                mCurrentPane.SetAliasTextColor(color.Color);
            }
        }

        private void btnViewHome_Click(object sender, EventArgs e)
        {
            mCurrentPane.SetFrontView();
        }

        private void btnViewFront_Click(object sender, EventArgs e)
        {
            mCurrentPane.SetRearView();
        }

        private void btnViewTop_Click(object sender, EventArgs e)
        {
            mCurrentPane.SetTopView();
        }

        private void btnViewLeft_Click(object sender, EventArgs e)
        {
            mCurrentPane.SetLeftView();
        }

        private void btnViewRight_Click(object sender, EventArgs e)
        {
            mCurrentPane.SetRightView();
        }

        private void btnSaveHome_Click(object sender, EventArgs e)
        {
            object obj = comboBox2.SelectedItem;
            if( obj != null)
            {
                string szName = (string)obj;
                mCurrentPane.SaveHomeView(szName);
            }
        }

        private void btnLoadHome_Click(object sender, EventArgs e)
        {
            object obj = comboBox2.SelectedItem;
            if (obj != null)
            {
                string szName = (string)obj;
                mCurrentPane.LoadHomeView(szName);
            }
        }

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Point pt = contextMenuStrip2.Bounds.Location;
            pt = unityInPanel.PointToClient(pt);
            unityInPanel.AddTextPOI(tbTextPOI.Text, pt.X, pt.Y);
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer1_Panel1_Enter(object sender, EventArgs e)
        {
            mCurrentPane = unityOutPanel;
            System.Diagnostics.Trace.WriteLine("Set Active Outside");
        }

        private void splitContainer1_Panel1_Leave(object sender, EventArgs e)
        {
        }

        private void splitContainer1_Panel2_Enter(object sender, EventArgs e)
        {
            mCurrentPane = unityInPanel;

            System.Diagnostics.Trace.WriteLine("Set Active Inside");
        }

        private void splitContainer1_Panel2_Leave(object sender, EventArgs e)
        {           
        }

       
       
        

        //private void splitContainer1_SizeChanged(object sender, EventArgs e)
        //{
        //    unityOutPanel.Update3D();
        //    unityInPanel.Update3D();
        //}

        //private void splitContainer1_SplitterMoving(object sender, SplitterCancelEventArgs e)
        //{
        //    this.Refresh();
        //    unityOutPanel.Update3D();
        //    unityInPanel.Update3D();
        //}


        ////assign this to the SplitContainer's MouseDown event
        //private void splitContainer1_MouseDown(object sender, MouseEventArgs e)
        //{
        //    // This disables the normal move behavior
        //    ((SplitContainer)sender).IsSplitterFixed = true;
        //}

        ////assign this to the SplitContainer's MouseUp event
        //private void splitContainer1_MouseUp(object sender, MouseEventArgs e)
        //{
        //    // This allows the splitter to be moved normally again
        //    ((SplitContainer)sender).IsSplitterFixed = false;
        //}

        ////assign this to the SplitContainer's MouseMove event
        //private void splitContainer1_MouseMove(object sender, MouseEventArgs e)
        //{
        //    // Check to make sure the splitter won't be updated by the
        //    // normal move behavior also
        //    if (((SplitContainer)sender).IsSplitterFixed)
        //    {
        //        // Make sure that the button used to move the splitter
        //        // is the left mouse button
        //        if (e.Button.Equals(MouseButtons.Left))
        //        {
        //            // Checks to see if the splitter is aligned Vertically
        //            if (((SplitContainer)sender).Orientation.Equals(Orientation.Vertical))
        //            {
        //                // Only move the splitter if the mouse is within
        //                // the appropriate bounds
        //                if (e.X > 0 && e.X < ((SplitContainer)sender).Width)
        //                {
        //                    // Move the splitter & force a visual refresh
        //                    ((SplitContainer)sender).SplitterDistance = e.X;
        //                    ((SplitContainer)sender).Refresh();
        //                }
        //            }
        //            // If it isn't aligned vertically then it must be
        //            // horizontal
        //            else
        //            {
        //                // Only move the splitter if the mouse is within
        //                // the appropriate bounds
        //                if (e.Y > 0 && e.Y < ((SplitContainer)sender).Height)
        //                {
        //                    // Move the splitter & force a visual refresh
        //                    ((SplitContainer)sender).SplitterDistance = e.Y;
        //                    ((SplitContainer)sender).Refresh();
        //                }
        //            }
        //        }
        //        // If a button other than left is pressed or no button
        //        // at all
        //        else
        //        {
        //            // This allows the splitter to be moved normally again
        //            ((SplitContainer)sender).IsSplitterFixed = false;
        //        }
        //    }
        //}

        //private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        //{

        //}

        
           
    }
}

 namespace System.Windows.Forms
 { 
    public class MySplitContainer : SplitContainer
    {
        protected override void OnSizeChanged(EventArgs e)
        {
            if (this.Handle != null)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    Panel1.Refresh();
                    Panel2.Refresh();
                    base.OnSizeChanged(e);
                });
            }
        }
    }
}
