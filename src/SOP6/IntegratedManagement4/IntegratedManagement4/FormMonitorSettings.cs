using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility2;
using UnE.GUI;

namespace IntegratedManagement4
{
    public partial class FormMonitorSettings : Form
    {
        private Cursor m_customCursor = null;

        public FormMonitorSettings()
        {
            InitializeComponent();

            pnSDMS.Tag = "SDMS";
            pnSOP.Tag = "SOPSimulator";
            pnCCTV.Tag = "CCTV";
            pnMission.Tag = "MissionList";

            GetMonitors(); 
        }

        public void InitDataLoad()
        {
            foreach (Control ctrl in panel1.Controls)
            {
                Panel monitor = ctrl as Panel;
                monitor.BackgroundImage = null;
                monitor.Tag = null;

                ImageButton btn = monitor.Controls[0] as ImageButton;
                btn.Visible = false;
            }

            pnSDMS.Enabled = true;            
            pnSOP.Enabled = true;
            pnCCTV.Enabled = true;
            pnMission.Enabled = true;

            pnSDMS.BackgroundImage = imgSDMSEnable;
            pnSOP.BackgroundImage = imgSOPEnable;
            pnCCTV.BackgroundImage = imgCCTVEnable;
            pnMission.BackgroundImage = imgMissionEnable;

            string[] strValues = { "SOPSimulator", "SDMS", "MissionList", "CCTV" };
            string[] strRetVal = new string[5];
            strRetVal[0] = RegUtil.ReadRegValue("Monitor Info", strValues[0], FormMain.Instance.SiteID);
            strRetVal[1] = RegUtil.ReadRegValue("Monitor Info", strValues[1], FormMain.Instance.SiteID);
            strRetVal[2] = RegUtil.ReadRegValue("Monitor Info", strValues[2], FormMain.Instance.SiteID);
            strRetVal[3] = RegUtil.ReadRegValue("Monitor Info", strValues[3], FormMain.Instance.SiteID);
            
            int nMonitor = -1;
            for (int i = 0; i < 4; i++)
            {
                if (int.TryParse(strRetVal[i], out nMonitor))
                {
                    if (nMonitor >= 0 && nMonitor <= 4)
                    {
                        foreach (Control ctrl in panel1.Controls)
                        {
                            Panel monitor = ctrl as Panel;
                            string strMonitorIndex = monitor.Name.Replace("monitor_", "");
                            int nMonitorIndex = Convert.ToInt32(strMonitorIndex);
                            
                            if (nMonitor == nMonitorIndex)
                            {
                                if (pnSDMS.Tag.ToString() == strValues[i])
                                {
                                    monitor.Tag = pnSDMS;
                                    SetPanelImage(monitor, imgSDMSEnable);
                                    SetPanelImage(pnSDMS, imgSDMSDisable);
                                    pnSDMS.Enabled = false;
                                }
                                else if (pnSOP.Tag.ToString() == strValues[i])
                                {
                                    monitor.Tag = pnSOP;
                                    SetPanelImage(monitor, imgSOPEnable);
                                    SetPanelImage(pnSOP, imgSOPDisable);
                                    pnSOP.Enabled = false;
                                }
                                else if (pnCCTV.Tag.ToString() == strValues[i])
                                {
                                    monitor.Tag = pnCCTV;
                                    SetPanelImage(monitor, imgCCTVEnable);
                                    SetPanelImage(pnCCTV, imgCCTVDisable);
                                    pnCCTV.Enabled = false;
                                }
                                else if (pnMission.Tag.ToString() == strValues[i])
                                {
                                    monitor.Tag = pnMission;
                                    SetPanelImage(monitor, imgMissionEnable);
                                    SetPanelImage(pnMission, imgMissionDisable);
                                    pnMission.Enabled = false;
                                }

                                if (monitor.Controls.Count > 0 && monitor.Controls[0] != null && monitor.Controls[0] is ImageButton)
                                {
                                    ImageButton btnDelete = monitor.Controls[0] as ImageButton;
                                    btnDelete.Visible = true;
                                }

                                break;
                            }
                        }
                    }
                }
            }
        }

        #region 모니터 정보 Load
        private void GetMonitors()
        {
            // 좌표 순서대로 정렬
            Screen[] screens = Screen.AllScreens.OrderBy(p => p.Bounds.Location.Y).OrderBy(p => p.Bounds.Location.X).ToArray();

            //int space = 10;
            int monitorIndex = 1;

            int minX = screens.Min(p => p.Bounds.X) / 20;
            int minY = screens.Min(p => p.Bounds.Y) / 20;

            Size parentSize = new Size();

            for (int i = 0; i < screens.Length; i++)
            {
                Size size = new Size(screens[i].Bounds.Size.Width / 20, screens[i].Bounds.Size.Height / 20);
                Point point = new Point(screens[i].Bounds.Location.X / 20, screens[i].Bounds.Location.Y / 20);

                // 마이너스 좌표가 있을경우 마이너스만큼 더해줘야함
                if (minX < 0)
                    point.X = point.X + Math.Abs(minX);
                if (minY < 0)
                    point.Y = point.Y + Math.Abs(minY);

                Panel monitorPanel = new Panel();
                monitorPanel.Name = "monitor_" + monitorIndex;
                monitorPanel.AllowDrop = true;
                monitorIndex++;
                monitorPanel.Size = size;
                monitorPanel.BackColor = Color.LightGray;
                monitorPanel.BackgroundImageLayout = ImageLayout.Zoom;

                monitorPanel.Location = point;
                monitorPanel.AllowDrop = true;

                // 삭제 버튼
                ImageButton btnDelete = new ImageButton();
                btnDelete.Parent = monitorPanel;
                btnDelete.Size = new Size(15, 15);
                btnDelete.Location = new Point(monitorPanel.Width - 18, 2);
                btnDelete.Click += BtnDelete_Click;
                btnDelete.SizeMode = PictureBoxSizeMode.Zoom;
                btnDelete.ImageNormal = global::IntegratedManagement4.Properties.Resources.Close_40_40_Default;                
                btnDelete.ImageClicked = global::IntegratedManagement4.Properties.Resources.Close_40_40_Click;
                btnDelete.ImageMouseOver = global::IntegratedManagement4.Properties.Resources.Close_40_40_Click;
                btnDelete.BackColor = Color.Transparent;
                btnDelete.Visible = false;
                monitorPanel.Controls.Add(btnDelete);

                monitorPanel.DragDrop += monitorPanel_DragDrop;
                monitorPanel.DragEnter += monitorPanel_DragEnter;
                monitorPanel.DragLeave += MonitorPanel_DragLeave;
                monitorPanel.Paint += monitorPanel_Paint;
                monitorPanel.MouseDown += monitorPanel_MouseDown;
                monitorPanel.GiveFeedback += MonitorPanel_GiveFeedback;
                panel1.Controls.Add(monitorPanel);

                parentSize = new Size(Math.Max(parentSize.Width, point.X + size.Width), Math.Max(parentSize.Height, point.Y + size.Height));
            }

            panel1.Size = parentSize;
            panel1.Parent = groupBox1;            
            panel1.Location = new Point(groupBox1.Width / 2 - panel1.Width / 2, groupBox1.Height / 2 - panel1.Height / 2);
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            ImageButton btnDelete = sender as ImageButton;
            Panel monitor = btnDelete.Parent as Panel;
            
            if (monitor != null)
            {
                Panel targetPanel = monitor.Tag as Panel;

                if (targetPanel != null)
                {
                    if (targetPanel == pnSDMS)
                        SetPanelImage(targetPanel, imgSDMSEnable);
                    else if (targetPanel == pnSOP)
                        SetPanelImage(targetPanel, imgSOPEnable);
                    else if (targetPanel == pnCCTV)
                        SetPanelImage(targetPanel, imgCCTVEnable);
                    else if (targetPanel == pnMission)
                        SetPanelImage(targetPanel, imgMissionEnable);

                    targetPanel.Enabled = true; 
                }

                monitor.BackgroundImage = null;
                monitor.Tag = null;
            }

            btnDelete.Visible = false;
        }

        private void MonitorPanel_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (e.Effect == DragDropEffects.Copy)
            {
                if (m_customCursor != null)
                {
                    e.UseDefaultCursors = false;
                    Cursor = m_customCursor;
                }
            }
        }

        private void monitorPanel_MouseDown(object sender, MouseEventArgs e)
        {
            Panel panel = sender as Panel;
            if (panel == null || panel.BackgroundImage == null)
                return;

            Bitmap bmp = ChangeOpacity(panel.BackgroundImage, 0.5f, pnDelete.Size); // new Bitmap(panel.BackgroundImage, pnDelete.Size);
            panel.DrawToBitmap(bmp, new Rectangle(panel.Location, pnDelete.Size));
            m_customCursor = new Cursor(bmp.GetHicon());

            this.Refresh();

            panel.DoDragDrop(sender, DragDropEffects.Copy);
        }

        private Font m_font = new System.Drawing.Font(Program.prgFont, 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
        private Font m_font2 = new System.Drawing.Font(Program.prgFont, 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
        private Pen m_pen = new Pen(Color.Black);

        private void monitorPanel_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            
            string panelIndex = panel.Name.Replace("monitor_", "");

            Graphics g = e.Graphics;
            g.DrawString(panelIndex, m_font, Brushes.Black, 0, 0);
            g.DrawRectangle(m_pen, new Rectangle(0, 0, panel.Width - 1, panel.Height - 1));
        }

        private void monitorPanel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Panel)))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void MonitorPanel_DragLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;   
        }

        private void monitorPanel_DragDrop(object sender, DragEventArgs e)
        {
            Panel panel = (Panel)e.Data.GetData(typeof(Panel));
            Image img = panel.BackgroundImage;

            Panel monitor = sender as Panel;
            if (monitor == null)
                return;

            if (panel == monitor)
            {
                this.Cursor = Cursors.Default;
                return;
            }

            if (monitor.Tag != null)
            {
                // 이미 설정되어있던 화면
                Panel alreadyPanel = monitor.Tag as Panel;
                if (alreadyPanel != null)
                {
                    if (alreadyPanel == pnSDMS)
                        SetPanelImage(alreadyPanel, imgSDMSEnable);
                    else if (alreadyPanel == pnSOP)
                        SetPanelImage(alreadyPanel, imgSOPEnable);
                    else if (alreadyPanel == pnCCTV)
                        SetPanelImage(alreadyPanel, imgCCTVEnable);
                    else if (alreadyPanel == pnMission)
                        SetPanelImage(alreadyPanel, imgMissionEnable);

                    alreadyPanel.Enabled = true;
                }
            }
                        
            SetPanelImage(monitor, img);
            this.Cursor = Cursors.Default;

            // 모니터패널에서 모니터패널로 설정한 경우
            if (panel.Name.Contains("monitor_"))
            {
                if (panel != null)
                {
                    monitor.Tag = panel.Tag; // 현재 입력된 panel을 tag에 넣는다.
                    panel.Tag = null;
                    panel.BackgroundImage = null;
                    if (panel.Controls.Count > 0 && panel.Controls[0] != null && panel.Controls[0] is ImageButton)
                    {
                        ImageButton btnDelete = panel.Controls[0] as ImageButton;
                        btnDelete.Visible = false;
                    }
                } 
            }
            else
            {
                if (panel != null)
                {
                    monitor.Tag = panel; // 현재 입력된 panel을 tag에 넣는다.

                    if (panel == pnSDMS)
                        SetPanelImage(panel, imgSDMSDisable);
                    else if (panel == pnSOP)
                        SetPanelImage(panel, imgSOPDisable);
                    else if (panel == pnCCTV)
                        SetPanelImage(panel, imgCCTVDisable);
                    else if (panel == pnMission)
                        SetPanelImage(panel, imgMissionDisable);

                    panel.Enabled = false;
                }
            }

            if (monitor.Controls.Count > 0 && monitor.Controls[0] != null && monitor.Controls[0] is ImageButton)
            {
                ImageButton btnDelete = monitor.Controls[0] as ImageButton;
                btnDelete.Visible = true;
            }
        }
        #endregion

        #region 초기 버튼 설정
        private Image imgSDMSEnable = global::IntegratedManagement4.Properties.Resources.Monitor_SDMS_Enable;
        private Image imgSDMSDisable = global::IntegratedManagement4.Properties.Resources.Monitor_SDMS_Disable;
        private Image imgSOPEnable = global::IntegratedManagement4.Properties.Resources.Monitor_SOP_Enable;
        private Image imgSOPDisable = global::IntegratedManagement4.Properties.Resources.Monitor_SOP_Disable;
        private Image imgCCTVEnable = global::IntegratedManagement4.Properties.Resources.Monitor_CCTV_Enable;
        private Image imgCCTVDisable = global::IntegratedManagement4.Properties.Resources.Monitor_CCTV_Disable;
        private Image imgMissionEnable = global::IntegratedManagement4.Properties.Resources.Monitor_Mission_Enable;
        private Image imgMissionDisable = global::IntegratedManagement4.Properties.Resources.Monitor_Mission_Disable;

        private Image imgDeleteEnable = global::IntegratedManagement4.Properties.Resources.Monitor_Delete_Enable;
        private Image imgDeleteDisable = global::IntegratedManagement4.Properties.Resources.Monitor_Delete_Disable;

        private void SetButton()
        {

        } 
        #endregion
        
        #region 화면 패널
        private void panel_MouseDown(object sender, MouseEventArgs e)
        {
            Panel panel = sender as Panel;
            if (panel == null || panel.BackgroundImage == null)
                return;

            Bitmap bmp = new Bitmap(panel.BackgroundImage, panel.Size);
            panel.DrawToBitmap(bmp, new Rectangle(panel.Location, bmp.Size));
            m_customCursor = new Cursor(bmp.GetHicon());

            this.Refresh();

            panel.DoDragDrop(sender, DragDropEffects.Copy);
        }

        private void panel_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (e.Effect == DragDropEffects.Copy)
            {
                if (m_customCursor != null)
                {
                    e.UseDefaultCursors = false;
                    Cursor = m_customCursor;
                }
            }
        }
        #endregion

        #region 휴지통 이벤트
        private void pnDelete_DragDrop(object sender, DragEventArgs e)
        {
            Panel monitor = (Panel)e.Data.GetData(typeof(Panel));
            this.Cursor = Cursors.Default;

            if (monitor != null)
            {
                pnDelete.BackgroundImage = global::IntegratedManagement4.Properties.Resources.Monitor_Delete_Disable;
                pnDelete.BackColor = Color.Transparent;

                Panel targetPanel = monitor.Tag as Panel;
                if (targetPanel == pnSDMS)
                    SetPanelImage(targetPanel, imgSDMSEnable);
                else if (targetPanel == pnSOP)
                    SetPanelImage(targetPanel, imgSOPEnable);
                else if (targetPanel == pnCCTV)
                    SetPanelImage(targetPanel, imgCCTVEnable);
                else if (targetPanel == pnMission)
                    SetPanelImage(targetPanel, imgMissionEnable);

                targetPanel.Enabled = true;

                monitor.BackgroundImage = null;
                monitor.Tag = null;
            }

            if (monitor.Controls.Count > 0 && monitor.Controls[0] != null && monitor.Controls[0] is ImageButton)
            {
                ImageButton btnDelete = monitor.Controls[0] as ImageButton;
                btnDelete.Visible = false;
            }
        }

        private void pnDelete_DragEnter(object sender, DragEventArgs e)
        {
            Panel monitorPanel = e.Data.GetData(typeof(Panel)) as Panel;
            if (monitorPanel == pnSDMS || monitorPanel == pnSOP || monitorPanel == pnCCTV || monitorPanel == pnMission)
                return;

            if (e.Data.GetDataPresent(typeof(Panel)))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void pnDelete_DragOver(object sender, DragEventArgs e)
        {
            pnDelete.BackgroundImage = imgDeleteEnable;
            pnDelete.BackColor = Color.FromArgb(0xf7, 0xa9, 0x2b);
        }
        private void pnDelete_DragLeave(object sender, EventArgs e)
        {
            pnDelete.BackgroundImage = imgDeleteDisable;
            pnDelete.BackColor = Color.Transparent;

            this.Cursor = Cursors.Default;
        }
        #endregion

        private void SetPanelImage(Panel pn, Image img)
        {
            pn.BackgroundImage = img;
        }

        #region 버튼 이벤트
        private void rbClose_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void rbSave_Click(object sender, EventArgs e)
        {
            SaveData();
            FormMain.Instance.SetupForm.InitDataLoad();
            this.Visible = false;
        }
        public void SaveData()
        {
            string[] strValues = { "SOPSimulator", "SDMS", "MissionList", "CCTV" };
            
            ArrayList arList = new ArrayList();
            foreach (string item in strValues)
            {
                arList.Add(item);
            }

            foreach (Control ctrl in panel1.Controls)
            {
                Panel monitorPanel = ctrl as Panel;
                if (monitorPanel == null || monitorPanel.Tag == null)
                    continue;

                Panel panel = monitorPanel.Tag as Panel;
                if (panel.Tag == null)
                {
                    continue;
                }
                else
                {
                    string monitorIndex = monitorPanel.Name.Replace("monitor_", "");
                    RegUtil.WriteRegValue("Monitor Info", panel.Tag.ToString(), monitorIndex, FormMain.Instance.SiteID);

                    arList.Remove(panel.Tag.ToString());
                }
            } 

            for (int i = 0; i < arList.Count; i++)
            {
                //if (!arList.Contains(strValues[i]))
                {
                    RegUtil.WriteRegValue("Monitor Info", arList[i].ToString(), "-1", FormMain.Instance.SiteID);
                }
            }
        }
        #endregion

        /// <summary>
        /// 해당 이미지의 투명도를 변경한다.
        /// </summary>
        /// <param name="img"></param>
        /// <param name="opacityvalue"></param>
        /// <returns></returns>
        public Bitmap ChangeOpacity(Image img, float opacityvalue, Size imgSize)
        {
            Bitmap bmp = new Bitmap(imgSize.Width, imgSize.Height);
            Graphics graphics = Graphics.FromImage(bmp);
            ColorMatrix colormatrix = new ColorMatrix();
            colormatrix.Matrix33 = opacityvalue;
            ImageAttributes imgAttribute = new ImageAttributes();
            imgAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            graphics.DrawImage(img, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttribute);
            graphics.Dispose();

            return bmp;
        } 
    }
}
