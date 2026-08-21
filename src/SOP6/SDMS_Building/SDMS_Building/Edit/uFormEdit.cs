using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.Spatial;
using System.Collections;
using static SDMS_Building.Edit.ImagePanel;
using DBUtility2;

namespace SDMS_Building.Edit
{
    public partial class uFormEdit : UserControl, IShapeOwner
    {
        private int FIRE_SENSOR = -1;
        private int PSM_SENSOR = -1;
        private int STRONG_WIND = -1;
        private int EARTHQUAKE_SENSOR = -1;
        private int FIREWALL_SENSOR = -1;
        private int DOOR_SENSOR = -1;
        private int BLACKOUT_SENSOR = -1;
        private int CCTV = -1;

        private UEWpfControl.WpfComboBox m_cbType = null;
        private UEWpfControl.WpfComboBox m_cbBuilding = null;
        private UEWpfControl.WpfComboBox m_cbFloor = null;
        private UEWpfControl.WpfComboBox m_cbShapes = null;

        WebDBManager m_dbMgr = null;

        public uFormEdit()
        {
            InitializeComponent();

            m_dbMgr = FormMain.Instance.DBManager;

            m_cbType = new UEWpfControl.WpfComboBox();
            eleType.Child = m_cbType;
            //m_cbType.customComboBox.SelectionChanged += EleTypeComboBox_SelectionChanged;
            m_cbType.SetSize(eleType.Width, eleType.Height);

            m_cbBuilding = new UEWpfControl.WpfComboBox();
            eleBuilding.Child = m_cbBuilding;
            m_cbBuilding.customComboBox.SelectionChanged += EleBuildingComboBox_SelectionChanged;
            m_cbBuilding.SetSize(eleBuilding.Width, eleBuilding.Height);

            m_cbFloor = new UEWpfControl.WpfComboBox();
            eleFloor.Child = m_cbFloor;
            m_cbFloor.customComboBox.SelectionChanged += EleFloorComboBox_SelectionChanged;
            m_cbFloor.SetSize(eleFloor.Width, eleFloor.Height);

            m_cbShapes = new UEWpfControl.WpfComboBox();
            eleShapes.Child = m_cbShapes;
            m_cbShapes.customComboBox.SelectionChanged += EleShapesComboBox_SelectionChanged;
            m_cbShapes.SetSize(eleShapes.Width, eleShapes.Height);

            panelImage.Owner = this;

            InitTypeComboBox();
            InitBuildingComboBox();
        }

        private void InitTypeComboBox()
        {
            int nItemIndex = 0;

            m_cbType.customComboBox.Items.Clear();
            m_cbType.customComboBox.Items.Add(Data.CommonString.POI_CCTV_Kor);
            CCTV = nItemIndex++;

            if (m_cbType.customComboBox.Items.Count > 0)
                m_cbType.customComboBox.SelectedIndex = 0;
        }

        private void InitBuildingComboBox()
        {
            m_cbBuilding.customComboBox.Items.Clear();
            m_cbBuilding.customComboBox.Items.Add("건물별");

            foreach (KeyValuePair<int, Building> item in UnE.Spatial.ZoneManager.Instance.DicBuildings)
            {
                m_cbBuilding.customComboBox.Items.Add(item.Value);
            }

            if (m_cbBuilding.customComboBox.Items.Count > 0)
                m_cbBuilding.customComboBox.SelectedIndex = 0;

        }

        private void EleBuildingComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            m_cbFloor.customComboBox.Items.Clear();
            m_cbFloor.customComboBox.Items.Add("층별");
            m_cbFloor.customComboBox.SelectedIndex = 0;

            object obj = m_cbBuilding.customComboBox.Items[m_cbBuilding.customComboBox.SelectedIndex];
            Type type = obj.GetType();

            // 층 콤보박스 채우기
            if (type == typeof(Building))
            {
                Building building = (Building)obj;
                ArrayList arrFloor = (ArrayList)building.FloorList.Clone();

                foreach (Zone floor in arrFloor)
                {
                    m_cbFloor.customComboBox.Items.Add(floor.Floor);
                }
            }
        }

        private void EleFloorComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Image img = null;
            panelImage.ClearShapes();
            m_cbShapes.customComboBox.Items.Clear();

            if (m_cbFloor.customComboBox.SelectedItem != null && m_cbFloor.customComboBox.SelectedIndex != 0)
            {
                Floor floor = (Floor)m_cbFloor.customComboBox.SelectedItem;
                
                if (floor.Zone.ID == 1)
                    img = global::SDMS_Building.Properties.Resources.Hotel_1F;
                else if (floor.Zone.ID == 2)
                    img = global::SDMS_Building.Properties.Resources.Hotel_2F;
                else if (floor.Zone.ID == 3)
                    img = global::SDMS_Building.Properties.Resources.Hotel_3F;
                else if (floor.Zone.ID == 4)
                    img = global::SDMS_Building.Properties.Resources.Hotel_4F;
                else if (floor.Zone.ID == 5)
                    img = global::SDMS_Building.Properties.Resources.Hotel_5F;
                else if (floor.Zone.ID == 6)
                    img = global::SDMS_Building.Properties.Resources.Hotel_6F;
                else if (floor.Zone.ID == 7)
                    img = global::SDMS_Building.Properties.Resources.Hotel_7F;
                else if (floor.Zone.ID == 8)
                    img = global::SDMS_Building.Properties.Resources.Hotel_8F;

                // 선택된 타입(CCTV 등등)에 따른 불러오는 리스트(DB테이블) 변경 
                if (m_cbType.customComboBox.SelectedIndex == CCTV)
                    LoadCCTVZoneList();
            }

            panelImage.SetImage(img);

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string strShapeName = textBoxShapeName.Text.Trim();

            if (strShapeName.Length == 0)
            {
                textBoxShapeName.Focus();
                MessageBox.Show("추가할 이름을 입력하세요.");
                return;
            }

            foreach (Shape _shape in m_cbShapes.customComboBox.Items)
            {
                if (_shape.Name == strShapeName)
                {
                    textBoxShapeName.Focus();
                    MessageBox.Show(strShapeName + "은 이미 사용중인 이름입니다.");
                    return;
                }
            }

            Shape shape = new Shape();
            shape.Name = strShapeName;

            int nIndex = m_cbShapes.customComboBox.Items.Add(shape);
            m_cbShapes.customComboBox.SelectedIndex = nIndex;

            panelImage.ClearShapes();

            foreach (Shape _shape in m_cbShapes.customComboBox.Items)
            {
                panelImage.AddShape(_shape);
            }

            panelImage.Refresh();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            Shape shape = (Shape)m_cbShapes.customComboBox.SelectedItem;

            if (shape != null)
            {
                panelImage.RemoveShape(shape);
                m_cbShapes.customComboBox.Items.Remove(shape);

                if (m_cbShapes.customComboBox.Items.Count == 0)
                    EleShapesComboBox_SelectionChanged(null, null);
                else
                {
                    m_cbShapes.customComboBox.SelectedIndex = m_cbShapes.customComboBox.Items.Count - 1;
                }

                panelImage.Refresh();
            }
        }

        private void EleShapesComboBox_SelectionChanged(object sender, EventArgs e)
        {
            if (m_cbShapes.customComboBox.SelectedIndex >= 0)
            {
                Shape shape = (Shape)m_cbShapes.customComboBox.SelectedItem;
                textBoxShapeName.Text = shape.Name;
                textBoxShapeX.Text = string.Format("{0:F1}", shape.Position.X);
                textBoxShapeY.Text = string.Format("{0:F1}", shape.Position.Y);
                textBoxURL.Text = shape.URL;

                if (shape.ID != -1)
                    textBoxID.Text = shape.ID.ToString();
            }
            else
            {
                textBoxShapeName.Text = "";
                textBoxShapeX.Text = "";
                textBoxShapeY.Text = "";
                textBoxURL.Text = "";
                textBoxID.Text = "";
            }
        }

        public void OnSelectShape(Shape shape)
        {
            if (shape == null)
                m_cbShapes.customComboBox.SelectedIndex = -1;
            else
            {
                foreach (Shape _shape in m_cbShapes.customComboBox.Items)
                {
                    if (shape == _shape)
                    {
                        if (m_cbShapes.customComboBox.SelectedItem == _shape)
                            return;
                        else
                        {
                            m_cbShapes.customComboBox.SelectedItem = shape;
                            return;
                        }
                    }
                }
            }
        }

        public void OnMoveShape(Shape shape, float x, float y)
        {
            textBoxShapeX.Text = string.Format("{0:F1}", x);
            textBoxShapeY.Text = string.Format("{0:F1}", y);
        }

        private void LoadCCTVZoneList()
        {
            Floor floor = (Floor)m_cbFloor.customComboBox.SelectedItem;

            string szText = "select ID, CameraName, X, Y, Z, URL from CCTV where ZoneID = {0}";
            string strSQL = string.Format(szText, floor.Zone.ID);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return;

            m_cbShapes.customComboBox.Items.Clear();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strName = WebDBManager.GetStringField(arrResult[i + 1], "");
                float nX = WebDBManager.GetFloatField(arrResult[i + 2].ToString(), -1);
                float nY = WebDBManager.GetFloatField(arrResult[i + 3].ToString(), -1);
                float nZ = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), -1);
                string strURL = WebDBManager.GetStringField(arrResult[i + 5], "");

                Shape shape = new Shape();
                shape.ID = nID;
                shape.Name = strName;
                shape.Position = new PointF(nX, nZ);
                shape.URL = strURL;

                //int nIndex = m_cbShapes.customComboBox.Items.Add(shape);
                m_cbShapes.customComboBox.Items.Add(shape);
                //m_cbShapes.customComboBox.SelectedIndex = nIndex;
            }

            panelImage.ClearShapes();

            foreach (Shape _shape in m_cbShapes.customComboBox.Items)
            {
                panelImage.AddShape(_shape);
            }

            panelImage.Refresh();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (m_cbShapes.customComboBox.Items.Count == 0)
                return;
        }

        private void btnApp_Click(object sender, EventArgs e)
        {
            Shape shape = (Shape)m_cbShapes.customComboBox.SelectedItem;

            if (shape == null)
                return;

            string strURL = textBoxURL.Text.Trim();

            if (strURL != null)
            {
                shape.URL = strURL;
            }
        }
    }
}
