using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnE.GUI;
using System.Collections;

namespace FireManagement.Docking
{
    public partial class FormPanel2 : Form, IRibbonButtonOwner, Ubists.IReaderOwner
    {
        // Button별 ID
        private FormEquipList m_dockEquipList = new FormEquipList();

        private Dictionary<Button, int> m_dicButtonIDs = new Dictionary<Button, int>();
        private Dictionary<int, Button> m_dicIDButtons = new Dictionary<int, Button>();
        private Dictionary<Button, bool> m_dicButtonChecked = new Dictionary<Button, bool>();

        private bool m_isConnectedRFIDReader = false;

        private FormAddEquip2 m_frmAddEquip = null;

        private bool m_btnShowEquipmentClicked = false;

        private DXFViewer.ShapeGroupOption m_optFE = null;
        private DXFViewer.ShapeGroupOption m_optFA = null;
        private DXFViewer.ShapeGroupOption m_optHD = null;

        private Form1 PopupGroupList = null;

        #region EquipZone Text Center 편집
        private static bool m_equipZoneTextEditMode = true;
        public static bool EquipZoneTextEditMode
        {
            get { return m_equipZoneTextEditMode; }
            set { m_equipZoneTextEditMode = value; }
        }

        private int m_nEditEquipZoneTextX = 0, m_nEditEquipZoneTextY = 0;
        private ArrayList m_arrNewText = new ArrayList();

        private FormEquipZoneText m_frmEquipZoneText = null;
        #endregion

        private DXFViewer.Text m_textSelected = null;
        private UnE.Geometry.Vertex2D m_vRClickOrigin = null;

        public FormAddEquip2 FrmAddEquip
        {
            get { return m_frmAddEquip; }
            set { m_frmAddEquip = value; }
        }
        private DockingEquipHistory m_frmEquipHistory = null;

        public DockingEquipHistory FrmEquipHistory
        {
            get { return m_frmEquipHistory; }
            set { m_frmEquipHistory = value; }
        }
        private FormCheckEquip3 m_frmCheckEquip = null;

        public RibbonButtonFireManagement BtnHome
        {
            get { return btnHome; }
            set { btnHome = value; }
        }
        public RibbonButtonFireManagement BtnFireExtinguisher
        {
            get { return btnFireExtinguisher; }
            set { btnFireExtinguisher = value; }
        }
        public RibbonButtonFireManagement BtnFirePlug
        {
            get { return btnFirePlug; }
            set { btnFirePlug = value; }
        }
        public RibbonButtonFireManagement BtnFireAlarm
        {
            get { return btnFireAlarm; }
            set { btnFireAlarm = value; }
        }
        
        public UnE.GUI.RibbonButton BtnEquipDel
        {
            get { return btnEquipDel; }
            set { btnEquipDel = value; }
        }
        public RibbonButtonFireManagement BtnGroup
        {
            get { return btnGroup; }
            set { btnGroup = value; }
        }

        public DXFViewer.ShapeGroupOption FEGroupOption
        {
            get { return m_optFE; }
        }

        public DXFViewer.ShapeGroupOption FAGroupOption
        {
            get { return m_optFA; }
        }

        public DXFViewer.ShapeGroupOption HDGroupOption
        {
            get { return m_optHD; }
        }

        public FireManagement.Mode Mode
        {
            get { return m_TabType; }
        }

        public System.Windows.Forms.Panel PanelRightBar
        {
            get { return panelRightBar; }
            set { panelRightBar = value; }
        }

        public FormPanel2()
        {
            InitializeComponent();
            m_frmAddEquip = new FormAddEquip2();

            FormMain2 frmMain = FormMain2.Instance;
            //m_frmEquipList = frmMain.ViewControl.LeftBar;
            m_frmEquipHistory = FormMain2.Instance.FrmEquipHistoryList;
            m_frmCheckEquip = FormMain2.Instance.EquipmentChecker;

            m_frmEquipZoneText = new FormEquipZoneText(dxfControl1);

            Init();
            btnEquipAdd.Visible = false;
            btnEquipDel.Visible = false;
        }

        private void FormPanel2_Load(object sender, EventArgs e)
        {
            InitButtons();
            InitPanel();
            ResizeControls();

            //btnShowEquipmentList.Location = new Point();
        }

        public DXFViewer.DXFControl DXFControl
        {
            get { return dxfControl1; }
        }

        public FormEquipList LeftBar
        {
            get { return m_dockEquipList; }
        }

        public void SetLabelText(string strText)
        {
            FormMain2.Instance.ZoneNameText.Text = strText;
            m_arrNewText.Clear();
        }

        private void InitRibbonButton(RibbonButton btn, int nID, Image imgNormal, Image imgChecked, Image imgDisabled, Image imgMouseOverBkgnd, Image imgCheckedBkgnd, Image imgDisabledBkgnd)
        {
            btn.NormalImage = imgNormal;
            btn.CheckedImage = imgChecked;
            btn.DisabledImage = imgDisabled;
            btn.MouseOverBkgndImage = imgMouseOverBkgnd;
            btn.CheckedBkgndImage = imgCheckedBkgnd;
            btn.DisabledBkgndImage = imgDisabledBkgnd;
            btn.Owner = this;

            SetButtonID(btn, nID);
        }

        public void SetButtonID(Button btn, int nID, string strTooltipText = "")
        {
            m_dicButtonIDs[btn] = nID;
            m_dicIDButtons[nID] = btn;
            m_dicButtonChecked[btn] = false;

            if (strTooltipText.Length > 0)
            {
                ToolTip tooltip = new ToolTip();
                tooltip.SetToolTip(btn, strTooltipText);
            }
        }

        public int GetButtonID(Button btn)
        {
            if (m_dicButtonIDs.ContainsKey(btn))
                return m_dicButtonIDs[btn];

            return -1;
        }

        private void InitButtons()
        { 
            InitRibbonButton(btnHome, ID.ID_HOME_SCREEN, global::FireManagement.Properties.Resources.home_86_82, global::FireManagement.Properties.Resources.home_icon, null, global::FireManagement.Properties.Resources.mouse_over_home, global::FireManagement.Properties.Resources.click_FireManagement, global::FireManagement.Properties.Resources.home_disabled);
            InitRibbonButton(btnGroup, ID.ID_GROUP_VISIBLE, global::FireManagement.Properties.Resources.group_Nomal, global::FireManagement.Properties.Resources.click_groupimg, null, global::FireManagement.Properties.Resources.group_Nomal, global::FireManagement.Properties.Resources.click_FireManagement, null);
            InitRibbonButton(btnFireExtinguisher, ID.ID_FIREEXTINGUISHER_VISIBLE, global::FireManagement.Properties.Resources.Extinguisher_86_82, global::FireManagement.Properties.Resources.fireex_icon, null, global::FireManagement.Properties.Resources.mouseover_Extinguisher, global::FireManagement.Properties.Resources.click_FireManagement, global::FireManagement.Properties.Resources.Extinguisher_disabled);
            InitRibbonButton(btnFirePlug, ID.ID_FIREPLUG_VISIBLE, global::FireManagement.Properties.Resources.FirePlug_86_82, global::FireManagement.Properties.Resources.fireplug_Icon, null, global::FireManagement.Properties.Resources.mouseover_FirePlug, global::FireManagement.Properties.Resources.click_FireManagement, global::FireManagement.Properties.Resources.FirePlug_disabled);
            InitRibbonButton(btnFireAlarm, ID.ID_FIREALARM_VISIBLE, global::FireManagement.Properties.Resources.FireAlarm_86_82, global::FireManagement.Properties.Resources.transmitter__Icon, null, global::FireManagement.Properties.Resources.mouseover_Alarm, global::FireManagement.Properties.Resources.click_FireManagement, global::FireManagement.Properties.Resources.FireAlarm_disabled);
            //InitRibbonButton(btnShowEquipmentList, ID.ID_SHOWEQUIPMENTLIST, null, null, null, null, null, null);

            //ArrangeRibbonButtons();

            btnFireExtinguisher.IsChecked = true;
            btnFirePlug.IsChecked = true;
            btnFireAlarm.IsChecked = true;
            //btnGroup.IsChecked = true;
        }

        private void InitPanel()
        {
            //panelMain.Location = new Point(panelLeft.Right, panelTop.Location.Y + panelTop.Size.Height);
            //panelMain.Size = new Size(this.Size.Width, this.Size.Height - panelMain.Location.Y);

            m_dockEquipList.Location = new Point(0, 0);
            m_dockEquipList.Dock = DockStyle.Fill;
            m_dockEquipList.TopLevel = false;
            m_dockEquipList.Parent = this;
            panelRightBar.Controls.Add(m_dockEquipList);
            m_dockEquipList.Show();

            m_frmAddEquip.Location = new Point(0, 0);
            m_frmAddEquip.Dock = DockStyle.Fill;
            m_frmAddEquip.TopLevel = false;
            m_frmAddEquip.Parent = this;
            panelRightBar.Controls.Add(m_frmAddEquip);

            m_frmCheckEquip.Location = new Point(0, 0);
            m_frmCheckEquip.Dock = DockStyle.Fill;
            m_frmCheckEquip.TopLevel = false;
            m_frmCheckEquip.Parent = this;
            panelRightBar.Controls.Add(m_frmCheckEquip);
            m_frmCheckEquip.Show();

            m_frmEquipHistory.Location = new Point(0, 0);
            m_frmEquipHistory.Dock = DockStyle.Fill;
            m_frmEquipHistory.TopLevel = false;
            m_frmEquipHistory.Parent = this;
            panelRightBar.Controls.Add(m_frmEquipHistory);
            m_frmEquipHistory.Show();

            
            m_frmAddEquip.Visible = false;
            m_frmCheckEquip.Visible = false;
            m_frmEquipHistory.Visible = false;
        }

        public void OnRibbonButtonMouseDown(object sender, MouseEventArgs e)
        {

        }

        public void OnRibbonButtonMouseUp(object sender, MouseEventArgs e)
        {
            RibbonButton btn = (RibbonButton)sender;
            int nButtonID = GetButtonID(btn);

            switch (nButtonID)
            {
                case ID.ID_HOME_SCREEN :
                    btnCheck(btnHome);
                    if (DXFControl != null)
                        DXFControl.LoadHomeMatrix(true);
                    break;
                case ID.ID_FIREEXTINGUISHER_VISIBLE :
                    btnCheck(btnFireExtinguisher);
                    LeftBar.Rearrange(FireEquipment.EquipmentType.FE, btnFireExtinguisher.IsChecked, btnFirePlug.IsChecked, btnFireAlarm.IsChecked);
                    FormMain2.Instance.EquipmentLayerOn(FireEquipment.EquipmentType.FE, btnFireExtinguisher.IsChecked);
                    break;
                case ID.ID_FIREPLUG_VISIBLE :
                    btnCheck(btnFirePlug);
                    LeftBar.Rearrange(FireEquipment.EquipmentType.FE, btnFireExtinguisher.IsChecked, btnFirePlug.IsChecked, btnFireAlarm.IsChecked);
                    FormMain2.Instance.EquipmentLayerOn(FireEquipment.EquipmentType.HD, btnFirePlug.IsChecked);
                    break;
                case ID.ID_FIREALARM_VISIBLE :
                    btnCheck(btnFireAlarm);
                    //LeftBar.Rearrange(FireEquipment.EquipmentType.FE, btnFireExtinguisher.IsChecked, btnFirePlug.IsChecked, btnFireAlarm.IsChecked);
                    FormMain2.Instance.EquipmentLayerOn(FireEquipment.EquipmentType.FA, btnFireAlarm.IsChecked);
                    break;
                case ID.ID_GROUP_VISIBLE :
                    btnCheck(btnGroup);
                    //LeftBar.Rearrange(FireEquipment.EquipmentType.UNKNOWN, btnFireExtinguisher.IsChecked, btnFirePlug.IsChecked, btnFireAlarm.IsChecked);
                    ShapeGroupLayerOn(btnGroup.IsChecked);
                    //FormMain2.Instance.GroupShowHide(FireEquipment.EquipmentType.FE, btnGroup.IsChecked);
                    //FormMain2.Instance.GroupShowHide(FireEquipment.EquipmentType.HD, btnGroup.IsChecked);
                    //FormMain2.Instance.GroupShowHide(FireEquipment.EquipmentType.FA, btnGroup.IsChecked); 
                    
                    break;
            }
        }

        public void ShapeGroupLayerOn(bool visible)
        {
            DXFViewer.Layer layerFE = FormMain2.Instance.GetEquipmentLayer(FireEquipment.EquipmentType.FE);
            DXFViewer.Layer layerHD = FormMain2.Instance.GetEquipmentLayer(FireEquipment.EquipmentType.HD);
            DXFViewer.Layer layerFA = FormMain2.Instance.GetEquipmentLayer(FireEquipment.EquipmentType.FA);

            if (layerFE != null)
                layerFE.UseGroupItem = visible;
            
            if (layerHD != null)
                layerHD.UseGroupItem = visible;

            if (layerFA != null)
                layerFA.UseGroupItem = visible;

            dxfControl1.CalcShapeGroup();
            dxfControl1.Refresh();
        }


        private void btnCheck(RibbonButton btn)
        {
            if (btn == btnHome)
                return;

            if (btn.IsChecked == true)
            {
                btn.IsChecked = false;
            }
            else
            {
                btn.IsChecked = true;
            }
            btn.Refresh();
        }

        public void SetRFIDOwner()
        {
            if (FormMain2.Instance.IsPCMode)
                return;

            FormMain2.Instance.RFIDReader.Owner = this;
            m_isConnectedRFIDReader = FormMain2.Instance.RFIDReader.StartReading();
        }

        public void OnReadTag(string strTag)
        {
            if (Mode == Mode.EDIT)
            {
                m_frmAddEquip.OnReadTag(strTag);
            }
            else
            {
                FireEquipment equip = FormMain2.Instance.DXFManager.FindEquipment(strTag);

                if (equip == null || equip.LinkedShape == null)
                {
                    // 아직 열려있지 않은 도면의 Tag
                    equip = FormMain2.Instance.IOManager.FindEquipment(strTag);

                    if (equip != null)
                    {
                        FormMain2.Instance.FormFileLoad.LoadZone(equip.Zone, FormMain2.Instance.CurrentZone);
                    }
                }

                if (equip != null && equip.LinkedShape != null)
                {
                    LogManager.Instance.WriteCheckLog(equip);

                    // 이미 열려있는 도면의 Tag
                    m_dockEquipList.SelectShape(equip.LinkedShape);
                    dxfControl1.Refresh();
                }
            }
            /*if (m_dockEquipList.SelectedEquipment == null)
                return;

            if (m_dockEquipList.SelectedEquipment.RFIDTag == strTag)
                return;

            m_dockEquipList.SetRFID(m_dockEquipList.SelectedEquipment, strTag);*/
        }

        private DXFViewer.Shape m_tmpshape = null;
        private UnE.Geometry.Vertex2D m_tmpvPos = null;

        private Point DownPt = new Point();

        private void dxfControl1_MouseDown(object sender, MouseEventArgs e)
        {
            FormMain2 frmMain = FormMain2.Instance;

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (frmMain.NeedScreenInput())
                {
                    UnE.Geometry.Vertex2D vPos = dxfControl1.ScreenToGlobal(e.X, e.Y);
                    UnE.Geometry.Vertex2D vMove = dxfControl1.MovedVertex;

                    float fUnitFlag = frmMain.GetUnitFlag(DXFViewer.UnitOfLength.METER);
                    frmMain.ScreenInput((vPos.x - vMove.x) * fUnitFlag, (vPos.y - vMove.y) * fUnitFlag);
                }

                if (m_dockEquipList.IsOpened)
                {
                    UnE.Geometry.Vertex2D vPos = dxfControl1.ScreenToGlobal(e.X, e.Y);
                    DXFViewer.Shape shape = dxfControl1.PickObject(vPos.x, vPos.y);

                    //설비추가일경우...
                    if (Mode == Mode.EDIT)
                    {
                        //LongTab시작
                        timerLongTab.Interval = 1000;
                        timerLongTab.Start();
                        timerLongTab.Enabled = true;

                        
                    }

                    if (shape != null)
                    {

                       // FormMain2.Instance.GetEquipmentLayerType(shape.GetLayer());

                        if (Mode == Mode.EDIT)
                            dxfControl1.Panning = false;

                        {

                            //if (!FormMain2.Instance.IsPCMode)
                            //    dxfControl1.PanningMouseButton = System.Windows.Forms.MouseButtons.Right;
                            //설비이력일경우...
                            if (Mode == Mode.EQUIP_HISTORY)
                            {
                                //LongTab시작
                                timerLongTab.Interval = 1000;
                                timerLongTab.Start();
                                timerLongTab.Enabled = true;
                            }

                            DownPt.X = e.X;
                            DownPt.Y = e.Y;

                            m_tmpshape = shape;
                            m_tmpvPos = vPos;

                            m_dockEquipList.SelectShape(shape);
                            m_frmEquipHistory.SelectShape(shape);
                        }
                        dxfControl1.Refresh();

                    }
                    else
                    {
                        //m_dockEquipList.ClearSelection(true);
                        //m_frmEquipHistory.ClearSelection(true);
                        m_dockEquipList.ClearSelection(false);
                        m_frmEquipHistory.ClearSelection(false);

                        dxfControl1.Refresh();
                    }
                }
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                OnRButtonClick(e.X, e.Y);
            }
        }

        private bool GetDXFCoord(int x, int y, out double _x, out double _y)
        {
            _x = _y = 0.0;
            UnE.Geometry.Vertex2D vertex = dxfControl1.ScreenToGlobal(x, y);

            if (vertex != null)
            {
                UnE.Geometry.Vertex2D vMove = dxfControl1.MovedVertex;
                float fFlag = FormMain2.Instance.GetUnitFlag(DXFViewer.UnitOfLength.METER);
                _x = (vertex.x - vMove.x) * fFlag;
                _y = (vertex.y - vMove.y) * fFlag;

                return true;
            }

            return false;
        }

        private void dxfControl1_MouseMove(object sender, MouseEventArgs e)
        {
            FormMain2 frmMain = FormMain2.Instance;

            if (dxfControl1.IsOpened)
            {
                /*UnE.Geometry.Vertex2D vertex = dxfControl1.ScreenToGlobal(e.X, e.Y);

                if (vertex != null)
                {
                    UnE.Geometry.Vertex2D vMove = dxfControl1.MovedVertex;
                    float fFlag = FormMain2.Instance.GetUnitFlag(DXFViewer.UnitOfLength.METER);
                    FormMain2.Instance.StatusText = string.Format("{0}, {1}, 단위(m)", (vertex.x - vMove.x) * fFlag, (vertex.y - vMove.y) * fFlag);
                }*/
                double x, y;

                if (GetDXFCoord(e.X, e.Y, out x, out y))
                {
                    FormMain2.Instance.StatusText = string.Format("{0}, {1}, 단위(m)", x, y);
                }
            }
            else
                FormMain2.Instance.StatusText = "";



            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (frmMain.NeedScreenInput())
                {
                    UnE.Geometry.Vertex2D vPos = dxfControl1.ScreenToGlobal(e.X, e.Y);
                    UnE.Geometry.Vertex2D vMove = dxfControl1.MovedVertex;

                    float fUnitFlag = frmMain.GetUnitFlag(DXFViewer.UnitOfLength.METER);
                    frmMain.ScreenInput((vPos.x - vMove.x) * fUnitFlag, (vPos.y - vMove.y) * fUnitFlag);


                }
                else if (m_dockEquipList.IsOpened)
                {
                    UnE.Geometry.Vertex2D vPos = dxfControl1.ScreenToGlobal(e.X, e.Y);

                    if (e.X != DownPt.X || e.Y != DownPt.Y)
                    {
                        if (timerLongTab.Enabled == true)
                        {
                            timerLongTab.Stop();
                            timerLongTab.Enabled = false;
                            nTabCount = 0;

                            System.Diagnostics.Debug.WriteLine("movestop");
                        }
                    }


                    if (Mode == Mode.EDIT)
                    {
                        if (m_dockEquipList.SelectedEquipment != null)
                        {
                            m_dockEquipList.SelectedEquipment.Move(vPos);

                            //m_tmpshape = null;

                            //m_tmpvPos = null;
                            //frmMain.Refresh();
                                
                            dxfControl1.Refresh();
                        }
                    }
                    else
                    {
                        //m_dockEquipList.ClearSelection(true);
                        //m_frmEquipHistory.ClearSelection(true);
                        m_dockEquipList.ClearSelection(false);
                        m_frmEquipHistory.ClearSelection(false);
                    }
                    
                }
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                OnRButtonMove(e.X, e.Y);
            }
        }

        private void dxfControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (timerLongTab.Enabled == true)
            {
                //타이머종료
                timerLongTab.Stop();
                timerLongTab.Enabled = false;
                nTabCount = 0;
            }


            FormMain2 frmMain = FormMain2.Instance;

            if (frmMain.NeedScreenInput())
            {
                UnE.Geometry.Vertex2D vPos = dxfControl1.ScreenToGlobal(e.X, e.Y);
                UnE.Geometry.Vertex2D vMove = dxfControl1.MovedVertex;

               // float fUnitFlag = frmMain.GetUnitFlag(DXFViewer.UnitOfLength.METER);
               // frmMain.ScreenInput((vPos.x - vMove.x) * fUnitFlag, (vPos.y - vMove.y) * fUnitFlag);
            }
            else if (m_dockEquipList.IsOpened)
            {
                UnE.Geometry.Vertex2D vPos = dxfControl1.ScreenToGlobal(e.X, e.Y);
                DXFViewer.Shape shape = dxfControl1.PickObject(vPos.x, vPos.y);


                if (shape != null)
                {
                    //Form1 form = new Form1();
                    //form.Show();


                    if (FormMain2.Instance.GetEquipmentLayerType(shape.GetLayer()) == FireEquipment.EquipmentType.UNKNOWN)
                    {
                        DXFViewer.ShapeGroup shapeGroup = (DXFViewer.ShapeGroup)shape;

                        if (PopupGroupList == null || PopupGroupList.IsDisposed)
                        {
                            PopupGroupList = new Form1(shapeGroup);
                        }
                        else
                        {
                            PopupGroupList.Close();
                            PopupGroupList = new Form1(shapeGroup);
                        }

                        PopupGroupList.StartPosition = FormStartPosition.Manual;
                        Point pt = PointToScreen(new Point(e.X - 50, e.Y - PopupGroupList.Size.Height));

                        PopupGroupList.Location = new Point(pt.X, pt.Y);

                        PopupGroupList.Show();
                    }

                    m_tmpshape = shape;
                    m_dockEquipList.SelectShape(shape);
                    m_frmEquipHistory.SelectShape(shape);

                    dxfControl1.Refresh();

                    if (Mode == Mode.EDIT)
                    {
                        if (m_dockEquipList.SelectedEquipment != null)
                        {
                            //m_dockEquipList.SelectedEquipment.Move(vPos);

                            Rectangle rect = FormMain2.Instance.ViewControl.BtnEquipDel.Bounds;

                            if (rect.Contains(new Point(e.X, e.Y+30)))
                            {
                                FormEquipDeletePopup frmEquipDelete = new FormEquipDeletePopup(m_tmpshape, m_tmpvPos);

                                frmEquipDelete.StartPosition = FormStartPosition.Manual;
                                Point pt = PointToScreen(new Point(FormMain2.Instance.PanelMain.Width/2 - frmEquipDelete.Size.Width/2 , btnShowEquipmentList.Location.Y -btnShowEquipmentList.Size.Height ));

                                frmEquipDelete.Location = new Point(pt.X, pt.Y);
                                frmEquipDelete.ShowDialog();
                            }

                            //m_tmpshape = null;

                            //m_tmpvPos = null;
                            dxfControl1.Refresh();
                        }
                    }
                }

                else
                {
                    if (PopupGroupList == null || PopupGroupList.IsDisposed)
                    {
                    }
                    else
                    {
                        PopupGroupList.Close();
                    }

                }
            }
        }

        #region EquipZone Text Center 편집
        private void OnRButtonClick(int x, int y)
        {
            if (!FormMain2.Instance.IsPCMode || !m_equipZoneTextEditMode)
                return;

            UnE.Geometry.Vertex2D vPos = dxfControl1.ScreenToGlobal(x, y);

            DXFViewer.Layer layerSensor = DXFManager.GetLayer(EquipmentZone.EquipZoneType.SENSOR_TYPE, dxfControl1);
            DXFViewer.Layer layerFA = DXFManager.GetLayer(EquipmentZone.EquipZoneType.FA_TYPE, dxfControl1);

            DXFViewer.Text textSelected = PickText(layerSensor, vPos);

            if (textSelected == null)
                textSelected = PickText(layerFA, vPos);

            m_textSelected = textSelected;
        }

        private DXFViewer.Text PickText(DXFViewer.Layer layer, UnE.Geometry.Vertex2D vPos)
        {
            if (layer == null)
                return null;

            double dy = 350, dx = 3500;

            foreach (DXFViewer.Shape shape in layer.Shapes)
            {
                if (shape.GetShapeType() != DXFViewer.Shape.ShapeType.TEXT)
                    continue;

                DXFViewer.Text text = (DXFViewer.Text)shape;

                double y = System.Math.Abs(text.Position.y - vPos.y);
                double x = System.Math.Abs(text.Position.x - vPos.x);

                if (x <= dx && y <= dy)
                {
                    System.Diagnostics.Trace.WriteLine(text.Title + " is Selected");
                    m_vRClickOrigin = vPos;

                    if (m_arrNewText.Contains(text))
                    {
                        m_frmEquipZoneText.SetText(text);
                        m_frmEquipZoneText.Show();
                    }

                    return text;
                }
            }

            return null;
        }

        private void OnRButtonMove(int x, int y)
        {
            if (!FormMain2.Instance.IsPCMode || !m_equipZoneTextEditMode)
                return;

            if (m_textSelected == null)
                return;

            UnE.Geometry.Vertex2D vPos = dxfControl1.ScreenToGlobal(x, y);
            m_textSelected.Move(vPos.x - m_vRClickOrigin.x, vPos.y - m_vRClickOrigin.y);

            m_vRClickOrigin = vPos;

            dxfControl1.Refresh();
        }

        private void OnRButtonDoubleClick(int x, int y)
        {
            if (!FormMain2.Instance.IsPCMode || !m_equipZoneTextEditMode)
                return;

            if (!dxfControl1.IsOpened)
                return;

            m_nEditEquipZoneTextX = x;
            m_nEditEquipZoneTextY = y;
            contextMenuStripEditEquipZoneText.Show(dxfControl1, x, y);
        }

        private void toolStripMenuItemSaveDB_Click(object sender, EventArgs e)
        {
            DXFViewer.Layer layerSensor = DXFManager.GetLayer(EquipmentZone.EquipZoneType.SENSOR_TYPE, dxfControl1);
            DXFViewer.Layer layerFA = DXFManager.GetLayer(EquipmentZone.EquipZoneType.FA_TYPE, dxfControl1);

            SaveToDBNewEquipZone();

            if (!SaveToDBEquipZoneText(layerSensor))
                return;
            if (!SaveToDBEquipZoneText(layerFA))
                return;

            SaveToDBRemovedEquipZone(layerSensor, layerFA);
        }

        private void SaveToDBNewEquipZone()
        {
            if (m_arrNewText.Count == 0)
                return;

            WebDBManager dbMgr = FormMain2.Instance.DBManager;
            string strSQL = "select max(id) from EquipmentZone";

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            IOManager ioMgr = FormMain2.Instance.IOManager;

            int nID = arrResult.Count == 0 ? 1 : WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;

            foreach (DXFViewer.Text text in m_arrNewText)
            {
                if (text.Tag == null)
                    continue;

                EquipmentZone equipZone = (EquipmentZone)text.Tag;

                Zone zone = (Zone)equipZone.LinkedZoneList[0];

                if (zone == null)
                    continue;

                string strBoundary = MakePolygonBoundary(equipZone.Polygon);
                string strTextCenter = MakeEquipZoneTextCenter(equipZone);

                strSQL = "Insert into EquipmentZone (ID, ZoneName, Boundary, LinkedZoneIDList, Type, BroadcastName, TextCenter, Description) values ";
                strSQL += string.Format("({0}, '{1}', '{2}', '{3}', {4}, '{1}', '{5}', NULL)", 
                    nID, equipZone.ZoneName, strBoundary, zone.ID, (int)equipZone.ZoneType, strTextCenter);

                if (dbMgr.GetResultData(strSQL, 0) == null)
                    return;

                equipZone.ID = nID++;

                ioMgr.AddEquipmentZone(equipZone, zone);
            }

            m_arrNewText.Clear();
        }

        private string MakePolygonBoundary(UnE.Geometry.Polygon polygon)
        {
            // 좌표 변경
            UnE.Geometry.Vertex2D vMove = FormMain2.Instance.DXFControl.MovedVertex;
            float fUnitFlag = FormMain2.Instance.GetUnitFlag(DXFViewer.UnitOfLength.METER);

            int nVertexCount = polygon.GetVertexCount();
            string strBoundary = "";

            for (int i=0;i<nVertexCount;i++)
            {
                UnE.Geometry.Vertex2D vertex = polygon.GetVertex(i);

                vertex = new UnE.Geometry.Vertex2D((vertex.x - vMove.x) * fUnitFlag, (vertex.y - vMove.y) * fUnitFlag);

                if (strBoundary.Length == 0)
                    strBoundary = string.Format("{0:f3}, {1:f3}", vertex.x, vertex.y);
                else
                    strBoundary += string.Format(", {0:f3}, {1:f3}", vertex.x, vertex.y);
            }

            return strBoundary;
        }

        private void toolStripMenuItemCreateText_Click(object sender, EventArgs e)
        {
            DXFViewer.Layer layerSensor = DXFManager.GetLayer(EquipmentZone.EquipZoneType.SENSOR_TYPE, dxfControl1);
            DXFViewer.Text text = MakeText(layerSensor, m_nEditEquipZoneTextX, m_nEditEquipZoneTextY);

            if (text == null)
                return;

            m_frmEquipZoneText.SetText(text);
            m_frmEquipZoneText.Show();
            dxfControl1.Refresh();

            m_arrNewText.Add(text);
            m_textSelected = text;
        }

        private DXFViewer.Text MakeText(DXFViewer.Layer layer, int x, int y)
        {
            UnE.Geometry.Vertex2D vPos = dxfControl1.ScreenToGlobal(x, y);

            EquipmentZone equipZone = new EquipmentZone();

            equipZone.BroadcastName = "새 Text";
            equipZone.ID = -1;
            equipZone.LinkedZoneList.Add(FormMain2.Instance.CurrentZone);

            equipZone.ZoneName = equipZone.BroadcastName;
            equipZone.ZoneTextCenter[FormMain2.Instance.CurrentZone] = vPos;
            equipZone.ZoneType = EquipmentZone.EquipZoneType.SENSOR_TYPE;

            equipZone.Polygon = new UnE.Geometry.Polygon();
            equipZone.Polygon.AddVertex(vPos);
            equipZone.Polygon.AddVertex(vPos);
            equipZone.Polygon.AddVertex(vPos);

            DXFViewer.Text textTrg = new DXFViewer.Text();

            textTrg.HorizontalAlignment = StringAlignment.Center;
            textTrg.VerticalAlignment = StringAlignment.Center;
            textTrg.Title = "새 Text";
            textTrg.SetPosition(vPos);
            textTrg.Font = new Font(textTrg.Font.FontFamily, 640.0f);
            textTrg.Tag = equipZone;

            layer.Add(textTrg);
            return textTrg;
        }

        private bool SaveToDBEquipZoneText(DXFViewer.Layer layer)
        {
            Zone zoneCurrent = FormMain2.Instance.CurrentZone;

            if (zoneCurrent == null)
                return false;

            UnE.Geometry.Vertex2D vMove = FormMain2.Instance.DXFControl.MovedVertex;
            float fUnitFlag = FormMain2.Instance.GetUnitFlag(DXFViewer.UnitOfLength.METER);

            foreach (DXFViewer.Shape shape in layer.Shapes)
            {
                if (shape.GetShapeType() != DXFViewer.Shape.ShapeType.TEXT)
                    continue;

                DXFViewer.Text text = (DXFViewer.Text)shape;

                if (text.Tag == null)
                    continue;

                UnE.Geometry.Vertex2D vPos = text.Position;
                vPos.SetVertex((vPos.x - vMove.x) * fUnitFlag, (vPos.y - vMove.y) * fUnitFlag);

                EquipmentZone equipZone = (EquipmentZone)text.Tag;
                equipZone.ZoneTextCenter[zoneCurrent] = vPos;

                if (!WriteDB(equipZone))
                    return false;
            }

            return true;
        }

        private void SaveToDBRemovedEquipZone(DXFViewer.Layer layer1, DXFViewer.Layer layer2)
        {
            Zone zoneCurrent = FormMain2.Instance.CurrentZone;

            if (zoneCurrent == null)
                return;

            ArrayList arrEquipZones = FormMain2.Instance.IOManager.GetEquipmentZoneList(zoneCurrent);

            foreach (EquipmentZone equipZone in arrEquipZones)
            {
                if (ContainsEquipZone(equipZone, layer1))
                    continue;

                if (ContainsEquipZone(equipZone, layer2))
                    continue;

                equipZone.ZoneTextCenter.Remove(zoneCurrent);

                if (!equipZone.NotShowingZone.Contains(zoneCurrent))
                    equipZone.NotShowingZone.Add(zoneCurrent);

                if (!WriteDB(equipZone))
                    return;
            }
        }

        private bool WriteDB(EquipmentZone equipZone)
        {
            WebDBManager dbMgr = FormMain2.Instance.DBManager;

            string strTextCenter = MakeEquipZoneTextCenter(equipZone);

            string strSQL = string.Format("Update EquipmentZone set ZoneName = '{0}', TextCenter = '{1}' where ID = {2}",
                equipZone.ZoneName, strTextCenter, equipZone.ID);

            bool isSuccess = dbMgr.GetResultData(strSQL, 0) != null;

            if (!isSuccess)
            {
                MessageBox.Show("DB 저장에 실패하였습니다.");
            }

            return isSuccess;
        }

        private bool ContainsEquipZone(EquipmentZone equipZone, DXFViewer.Layer layer)
        {
            foreach (DXFViewer.Shape shape in layer.Shapes)
            {
                if (shape.GetShapeType() != DXFViewer.Shape.ShapeType.TEXT)
                    continue;

                EquipmentZone equipZone2 = (EquipmentZone)shape.Tag;

                if (equipZone == equipZone2)
                    return true;
            }

            return false;
        }

        private void dxfControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!FormMain2.Instance.IsPCMode || !m_equipZoneTextEditMode)
                return;

            if (e.KeyCode == Keys.Delete)
            {
                if (m_textSelected != null)
                {
                    m_textSelected.GetLayer().Remove(m_textSelected);
                    dxfControl1.Refresh();
                }
            }
        }

        private string MakeEquipZoneTextCenter(EquipmentZone equipZone)
        {
            string strZoneTextCenter = "";

            foreach (KeyValuePair<Zone, UnE.Geometry.Vertex2D> pair in equipZone.ZoneTextCenter)
            {
                string strText = string.Format("{0}({1:f3},{2:f3})", pair.Key.ID, pair.Value.x, pair.Value.y);

                if (strZoneTextCenter.Length == 0)
                    strZoneTextCenter = strText;
                else
                    strZoneTextCenter += ", " + strText;
            }

            foreach (Zone zone in equipZone.NotShowingZone)
            {
                string strText = string.Format("{0}(null)", zone.ID);

                if (strZoneTextCenter.Length == 0)
                    strZoneTextCenter = strText;
                else
                    strZoneTextCenter += ", " + strText;
            }

            return strZoneTextCenter;
        }
        #endregion

        private void Init()
        {
            //labelZoneName.Text = "";

            //CreatePane();

            panelRightBar.Visible = false;

            if (!FormMain2.Instance.IsPCMode)
                dxfControl1.PanningMouseButton = System.Windows.Forms.MouseButtons.Left;

            dxfControl1.UseGroupItem = true;

            Image imgFE = global::FireManagement.Properties.Resources.visible_FireExtinguisher_52_52;
            Image imgFA = global::FireManagement.Properties.Resources.visible_FirePlug_52_52;
            Image imgHD = global::FireManagement.Properties.Resources.visible_HD_52_52;

            m_optFE = new DXFViewer.ShapeGroupOption(imgFE, 52, 52);
            m_optFA = new DXFViewer.ShapeGroupOption(imgFA, 52, 52);
            m_optHD = new DXFViewer.ShapeGroupOption(imgHD, 52, 52);
        }

        public void SetGroupOption()
        {
            DXFViewer.Layer layerFE = FormMain2.Instance.GetEquipmentLayer(FireEquipment.EquipmentType.FE);
            DXFViewer.Layer layerFA = FormMain2.Instance.GetEquipmentLayer(FireEquipment.EquipmentType.FA);
            DXFViewer.Layer layerHD = FormMain2.Instance.GetEquipmentLayer(FireEquipment.EquipmentType.HD);

            SetGroupOption(layerFE, m_optFE);
            SetGroupOption(layerFA, m_optFA);
            SetGroupOption(layerHD, m_optHD);

            dxfControl1.CalcShapeGroup();
        }

        private void SetGroupOption(DXFViewer.Layer layer, DXFViewer.ShapeGroupOption opt)
        {
            if (layer == null)
                return;

            layer.UseGroupItem = true;
            layer.ShapeGroupOption = opt;
        }

        public void ResizeControls()
        {
            int Widthdxf = FormMain2.Instance.Size.Width;
            int Heightdxf = FormMain2.Instance.Size.Height - FormMain2.Instance.PanelTop.Size.Height;

            dxfControl1.Location = new Point(0, FormMain2.Instance.ZoneNameText.Size.Height);
            dxfControl1.Size = new Size(Widthdxf, Heightdxf);


            if (m_btnShowEquipmentClicked == false)
            {
                btnShowEquipmentList.Location = new Point(Widthdxf - btnShowEquipmentList.Size.Width, Heightdxf / 2 - btnShowEquipmentList.Size.Height);
                
            }
            else
            {
                btnShowEquipmentList.Location = new Point(this.Width - (panelRightBar.Width + btnShowEquipmentList.Size.Width), Heightdxf / 2 - btnShowEquipmentList.Size.Height);
            }

            panelRightBar.Location = new Point(Widthdxf - panelRightBar.Size.Width, 0);
            btnEquipAdd.Location = new Point(FormMain2.Instance.PanelTop.Width / 2 - btnEquipAdd.Width, this.Height - btnEquipAdd.Height - 50);
            btnEquipDel.Location = new Point(btnEquipAdd.Location.X + 90, btnEquipAdd.Location.Y);
        }

        public void ChangedTab()
        {
            FormMain2 frmMain = FormMain2.Instance;

            int Widthdxf = FormMain2.Instance.Size.Width;
            int Heightdxf = FormMain2.Instance.Size.Height - FormMain2.Instance.PanelTop.Size.Height;

            btnShowEquipmentList.Location = new Point(Widthdxf - btnShowEquipmentList.Size.Width, Heightdxf / 2 - btnShowEquipmentList.Size.Height);
            btnShowEquipmentList.BackgroundImage = global::FireManagement.Properties.Resources.Btn_close_Panel_;
            panelRightBar.Visible = false;

            //파일탭
            if (frmMain.TypePictureBoxTab == 0)
            {
                frmMain.PictureBoxHistory.Visible = false;
                frmMain.PictureBoxCheckEquip.Visible = false;
                frmMain.PictureBoxNormalMode.Visible = false;
                frmMain.PictureBoxEditMode.Visible = false;
            }
            //소방설비탭
            else if (frmMain.TypePictureBoxTab == 1)
            {
                frmMain.PictureBoxNormalMode.Visible = true;
                frmMain.PictureBoxEditMode.Visible = true;
                frmMain.PictureBoxHistory.Visible = false;
                frmMain.PictureBoxCheckEquip.Visible = false;
                ButtonClose();
            }
                //관리탭
            else if (frmMain.TypePictureBoxTab == 2)
            {
                frmMain.PictureBoxNormalMode.Visible = false;
                frmMain.PictureBoxEditMode.Visible = false;
                frmMain.PictureBoxHistory.Visible = true;
                frmMain.PictureBoxCheckEquip.Visible = true;
                ButtonClose();
            }
            btnEquipAdd.Visible = false;
            btnEquipDel.Visible = false;
        }


        private void btnShowEquipmentList_Click(object sender, EventArgs e)
        {
            FormMain2 frmMain = FormMain2.Instance;
            //btnShowEquipmentList.Visible = false;

            if (panelRightBar.Visible == false)
            {
                ButtonOpen();
            }
            else
            {
                ButtonClose();


            }
        }

        public void ButtonClose()
        {
            int Widthdxf = FormMain2.Instance.Size.Width;
            int Heightdxf = FormMain2.Instance.Size.Height - FormMain2.Instance.PanelTop.Size.Height;

            m_btnShowEquipmentClicked = false;

            panelRightBar.Visible = false;
            btnShowEquipmentList.Location = new Point(Widthdxf - btnShowEquipmentList.Size.Width, Heightdxf / 2 - btnShowEquipmentList.Size.Height);
            btnShowEquipmentList.BackgroundImage = global::FireManagement.Properties.Resources.Btn_close_Panel_;

            btnShowEquipmentList.Visible = true;

            //설비추가모드로 선택됐을 때
            if (Mode == Mode.EDIT)
            {
                if (m_frmAddEquip.IsWorking == true)
                {
                    m_frmAddEquip.Hide();
                    m_dockEquipList.Visible = true;
                }
            }
            //설비점검으로 선택됐을 때
            else if (Mode == Mode.CHECK_EQUIP)
            {
                m_frmCheckEquip.Hide();
            }
        }

        public void ButtonOpen()
        {
            if (m_btnShowEquipmentClicked == false)
            {
                int Widthdxf = FormMain2.Instance.Size.Width;
                int Heightdxf = FormMain2.Instance.Size.Height - FormMain2.Instance.PanelTop.Size.Height;
                btnShowEquipmentList.Location = new Point(this.Width - (panelRightBar.Width + btnShowEquipmentList.Size.Width), Heightdxf / 2 - btnShowEquipmentList.Size.Height);
                btnShowEquipmentList.BackgroundImage = global::FireManagement.Properties.Resources.Btn_Open_Panel;
            }


            m_btnShowEquipmentClicked = true;

            panelRightBar.Visible = true;


            //설비추가모드일때 & +버튼을 눌렀을 때를 동시에 만족해야함
            if (Mode == Mode.EDIT && btnShowEquipmentList.Visible == false)
            {
                if (m_frmAddEquip == null || m_frmAddEquip.IsDisposed)
                {
                    m_frmAddEquip = new FormAddEquip2();

                    m_frmAddEquip.Location = new Point(0, 0);
                    m_frmAddEquip.Dock = DockStyle.Fill;
                    m_frmAddEquip.TopLevel = false;
                    m_frmAddEquip.Parent = this;
                    panelRightBar.Controls.Add(m_frmAddEquip);
                }

                m_frmAddEquip.Show();
            }
            else if (Mode == Mode.CHECK_EQUIP)
            {
                if (m_frmCheckEquip == null || m_frmCheckEquip.IsDisposed)
                {
                    m_frmCheckEquip = new FormCheckEquip3();

                    m_frmCheckEquip.Location = new Point(0, 0);
                    m_frmCheckEquip.Dock = DockStyle.Fill;
                    m_frmCheckEquip.TopLevel = false;
                    m_frmCheckEquip.Parent = this;
                    panelRightBar.Controls.Add(m_frmCheckEquip);
                }

                m_frmCheckEquip.Show();
            }
        }

        private Mode m_TabType = Mode.GENERAL;
        public void ChangeDocking(Mode nType)
        {
            if (m_frmAddEquip.IsWorking)
                m_frmAddEquip.Hide();
            if (m_frmCheckEquip.IsWorking)
                m_frmCheckEquip.Hide();

            m_TabType = nType;
            //nType = 0 - 일반모드 / 1 - 편집모드 / 2 - 이력 / 3 - 설비점검
            if (nType == Mode.GENERAL)
            {
                m_dockEquipList.ChangeEditMode(false);

                btnEquipAdd.Visible = false;
                btnEquipDel.Visible = false;
                btnShowEquipmentList.Visible = true;

                m_dockEquipList.Visible = true;
                m_frmAddEquip.Visible = false;
                m_frmCheckEquip.Visible = false;
                m_frmEquipHistory.Visible = false;

                if (m_dockEquipList.SelectedShape != null)
                    m_dockEquipList.SelectShape(m_dockEquipList.SelectedShape);

                m_dockEquipList.ReSizeControl();
                //ButtonOpen();
                //ButtonClose();
            }
            else if (nType == Mode.EDIT)
            {
                m_dockEquipList.ChangeEditMode();

                //ButtonClose();
                btnEquipAdd.Visible = true;
                btnEquipDel.Visible = true;
                btnShowEquipmentList.Visible = true;
                //btnShowEquipmentList.Visible = false;

                m_dockEquipList.Visible = true;
                m_frmAddEquip.Visible = false;
                m_frmCheckEquip.Visible = false;
                m_frmEquipHistory.Visible = false;
            }
            else if (nType == Mode.EQUIP_HISTORY)
            {
                btnEquipAdd.Visible = false;
                btnEquipDel.Visible = false;
                btnShowEquipmentList.Visible = true;

                m_dockEquipList.Visible = false;
                m_frmAddEquip.Visible = false;
                m_frmEquipHistory.Visible = true;
                m_frmCheckEquip.Visible = false;

                if (m_dockEquipList.SelectedShape != null)
                    m_frmEquipHistory.SelectShape(m_dockEquipList.SelectedShape);

                m_frmEquipHistory.ReSizeControl();

                //ButtonOpen();
                //ButtonClose();

                //if (FormMain2.Instance.EquipmentHistoryViewer.IsDisposed)
                //    FormMain2.Instance.EquipmentHistoryViewer = new FormEquipHistory();
                //FormMain2.Instance.EquipmentHistoryViewer.Show();
            }
            else if (nType == Mode.CHECK_EQUIP)
            {
                if (panelRightBar.Visible == true && !m_frmCheckEquip.IsDisposed)
                    m_frmCheckEquip.Show();

                btnEquipAdd.Visible = false;
                btnEquipDel.Visible = false;
                btnShowEquipmentList.Visible = true;

                m_dockEquipList.Visible = false;
                m_frmAddEquip.Visible = false;
                m_frmEquipHistory.Visible = false;
                m_frmCheckEquip.Visible = true;

                if (m_dockEquipList.SelectedEquipment != null)
                    m_frmCheckEquip.SetEquipment(m_dockEquipList.SelectedEquipment);

                ButtonOpen();
            }
        }

        private void btnEquipAdd_Click(object sender, EventArgs e)
        {
            btnShowEquipmentList.Visible = false;

            m_dockEquipList.Visible = false;
            m_frmAddEquip.Visible = true;
            m_frmCheckEquip.Visible = false;
            m_frmEquipHistory.Visible = false;

            panelRightBar.Visible = true;

            ButtonOpen();
        }

        private void btnEquipDel_MouseUp(object sender, MouseEventArgs e)
        {
            Point pt2 = ((Control)sender).PointToScreen(new Point(e.X, e.Y));
            Point pt = dxfControl1.PointToClient(pt2);
            MouseEventArgs eventArg = new MouseEventArgs(e.Button, e.Clicks, pt.X, pt.Y, e.Delta);
            dxfControl1_MouseUp(sender, eventArg);
        }

        private static int nTabCount = 0;

        private void timerLongTab_Tick(object sender, EventArgs e)
        {
            nTabCount++;
            System.Diagnostics.Debug.WriteLine(nTabCount);

            //1초동안 화면을 길게 누를 경우..
            if (nTabCount > 0)
            {
                nTabCount = 0;
                timerLongTab.Stop();
                timerLongTab.Enabled = false;
                System.Diagnostics.Debug.WriteLine("endTimer");

                //설비추가일 때
                if (Mode == Mode.EDIT)
                {
                    btnEquipAdd_Click(sender, e);


                }
                //설비이력일때
                else if (Mode == Mode.EQUIP_HISTORY)
                {
                    if (FormMain2.Instance.EquipmentHistoryViewer == null || FormMain2.Instance.EquipmentHistoryViewer.IsDisposed)
                    {
                        FormMain2.Instance.EquipmentHistoryViewer = new FormEquipHistory();

                        FormMain2.Instance.EquipmentHistoryViewer.StartPosition = FormStartPosition.Manual;
                        Point pt = FormMain2.Instance.ViewControl.PointToScreen(new Point(FormMain2.Instance.ViewControl.PanelRightBar.Location.X-5
                            , FormMain2.Instance.ViewControl.PanelRightBar.Location.Y));

                        FormMain2.Instance.EquipmentHistoryViewer.Location = new Point(pt.X - FormMain2.Instance.EquipmentHistoryViewer.Size.Width, pt.Y);
                    }

                    FormMain2.Instance.EquipmentHistoryViewer.Show(LeftBar.SelectedEquipment);
                }
            }
        }

        private void dxfControl1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            FormMain2 frmMain = FormMain2.Instance;

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (frmMain.NeedScreenInput())
                {
                    UnE.Geometry.Vertex2D vPos = dxfControl1.ScreenToGlobal(e.X, e.Y);
                    UnE.Geometry.Vertex2D vMove = dxfControl1.MovedVertex;

                    float fUnitFlag = frmMain.GetUnitFlag(DXFViewer.UnitOfLength.METER);
                    frmMain.ScreenInput((vPos.x - vMove.x) * fUnitFlag, (vPos.y - vMove.y) * fUnitFlag);
                }

                if (m_dockEquipList.IsOpened)
                {
                    UnE.Geometry.Vertex2D vPos = dxfControl1.ScreenToGlobal(e.X, e.Y);
                    DXFViewer.Shape shape = dxfControl1.PickObject(vPos.x, vPos.y);

                    if (shape != null)
                    {
                        //설비이력일경우...
                        if (Mode == Mode.EQUIP_HISTORY)
                        {
                            if (FormMain2.Instance.EquipmentHistoryViewer == null || FormMain2.Instance.EquipmentHistoryViewer.IsDisposed)
                            {
                                FormMain2.Instance.EquipmentHistoryViewer = new FormEquipHistory();

                                FormMain2.Instance.EquipmentHistoryViewer.StartPosition = FormStartPosition.Manual;
                                Point pt = FormMain2.Instance.ViewControl.PointToScreen(new Point(FormMain2.Instance.ViewControl.PanelRightBar.Location.X - 5
                                    , FormMain2.Instance.ViewControl.PanelRightBar.Location.Y));

                                FormMain2.Instance.EquipmentHistoryViewer.Location = new Point(pt.X - FormMain2.Instance.EquipmentHistoryViewer.Size.Width, pt.Y);
                            }

                            FormMain2.Instance.EquipmentHistoryViewer.Show(LeftBar.SelectedEquipment);
                        }

                        m_dockEquipList.SelectShape(shape);
                        m_frmEquipHistory.SelectShape(shape);

                        dxfControl1.Refresh();
                    }
                    else
                    {
                        //m_dockEquipList.ClearSelection(true);
                        //m_frmEquipHistory.ClearSelection(true);
                        m_dockEquipList.ClearSelection(false);
                        m_frmEquipHistory.ClearSelection(false);

                        dxfControl1.Refresh();
                    }
                }
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                OnRButtonDoubleClick(e.X, e.Y);
            }
        }

        //private void CreatePane()
        //{
        //    Pane paneLayer = axDockingPane.CreatePane(0, 280, 190, DockingDirection.DockLeftOf, null);
        //    paneLayer.Title = "Layer";
        //    paneLayer.Options = PaneOptions.PaneNoCloseable;

        //    m_dockLeft = new DockingLeftBar();
        //    m_arrDocking[0] = m_dockLeft;

        //    axDockingPane.VisualTheme = VisualTheme.ThemeVisualStudio2010;
        //}
    }
}
