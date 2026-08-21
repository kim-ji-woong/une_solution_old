using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BIMViewer.DB;
using BIMViewer.Shapes;

namespace BIMViewer.PopupForms
{
    using BIM;

    public partial class PopupPOILayer : PopupFormBase
    {
        private string m_strProjectName = "";
        private string m_strLevelXMLID = "";

        private Project m_project = null;
        
        private _SqlConnection m_connection = null;
        private Dictionary<int, POIType> m_dicPOITypes = null;  // poi 전체 목록

        private List<Shapes.POI> m_POIs = null;   // 현재 level에서 사용중인 poi 목록

        private List<POITypeVisibleProperty> m_gridViewData1 = new List<POITypeVisibleProperty>();
        private List<POITypeVisibleProperty> m_gridViewData2 = new List<POITypeVisibleProperty>();
        private List<POITypeVisibleProperty> m_gridViewData3 = new List<POITypeVisibleProperty>();
        private List<POITypeVisibleProperty> m_gridViewData4 = new List<POITypeVisibleProperty>();
        private List<POITypeVisibleProperty> m_gridViewDataUser = new List<POITypeVisibleProperty>();

        private Dictionary<int, bool> m_dicPoiVisible = null;
        private List<Layer> m_layers = null;

        

        private IGDIOwner m_owner = null;
        public IGDIOwner GDIOwner
        {
            get { return m_owner; }
            set { m_owner = value; }
        }

        private bool m_isLoadFinish = false;

#if DB_USE
        public PopupPOILayer(_SqlConnection connection, Dictionary<int, POIType> poiTypes)
        {
            InitializeComponent();
            
            m_connection = connection;
            m_dicPOITypes = poiTypes;

            customGridViewUserEdit.AllowUserToAddRows = true;
            customGridViewUserEdit.CellBorderStyle = DataGridViewCellBorderStyle.Single;
        }
#elif XML_USE
        public PopupPOILayer(Project project, Dictionary<int, POIType> poiTypes)
        {
            InitializeComponent();
            
            m_project = project;
            m_dicPOITypes = poiTypes;

            customGridViewUserEdit.AllowUserToAddRows = true;
            customGridViewUserEdit.CellBorderStyle = DataGridViewCellBorderStyle.Single;
        }
#endif

        private void PopupPOILayer_Load(object sender, EventArgs e)
        {
            //LoadGridView();

            IsUserPOIType();
        }

        private void LoadGridView()
        {
            customGridView1.Rows.Clear();
            customGridView2.Rows.Clear();
            customGridView3.Rows.Clear();
            customGridView4.Rows.Clear();
            customGridViewUser.Rows.Clear();
            customGridViewUserEdit.Rows.Clear();

            m_gridViewData1.Clear();
            m_gridViewData2.Clear();
            m_gridViewData3.Clear();
            m_gridViewData4.Clear();
            m_gridViewDataUser.Clear();

            if (m_isLoadFinish)
                return;

            try
            {
                // 현재 level에 작성된 poi만 표시
                Dictionary<int, POIType> dicPOITypes = new Dictionary<int, POIType>();

                if (m_POIs != null)
                {
                    foreach (POI poi in m_POIs)
                    {
                        POIType type = poi.PoiType;
                        dicPOITypes[type.ID] = type;

                        if (type.Parent != null)
                            GetParentPOI(dicPOITypes, type.Parent);
                    }
                }

                //foreach (KeyValuePair<int, POIType> item in FormMain.Instance.BimManager.POITypes)
                foreach (KeyValuePair<int, POIType> item in dicPOITypes)
                {
                    POIType poiType = item.Value;

                    bool isVisible = true;
                    if (m_dicPoiVisible != null && m_dicPoiVisible.ContainsKey(poiType.ID))
                        isVisible = m_dicPoiVisible[poiType.ID];

                    POITypeVisibleProperty poiTypeProperty = new POITypeVisibleProperty(poiType, isVisible);

                    if (!poiType.UserDefined)
                    {
                        int nDept = 0;
                        FindParentPOI(poiType, ref nDept);

                        if (nDept == 1)
                            m_gridViewData1.Add(poiTypeProperty);
                        else if (nDept == 2)
                            m_gridViewData2.Add(poiTypeProperty);
                        else if (nDept == 3)
                            m_gridViewData3.Add(poiTypeProperty);
                        else if (nDept == 4)
                            m_gridViewData4.Add(poiTypeProperty);
                    }
                    else
                    {
                        if (!poiType.IsGroup)
                            m_gridViewDataUser.Add(poiTypeProperty);
                    }
                }

                     

                bool isAllVisible = true;
                foreach (POITypeVisibleProperty item in m_gridViewData1)
                {
                    AddRow(customGridView1, item.POIType, item.POIVisible);
                    if (!item.POIVisible && isAllVisible)
                        isAllVisible = false;
                }

                if (isAllVisible)
                    cb1.Checked = isAllVisible;

                isAllVisible = true;
                foreach (POITypeVisibleProperty item in m_gridViewDataUser)
                {
                    AddRow(customGridViewUser, item.POIType, item.POIVisible);
                    AddRow(customGridViewUserEdit, item.POIType, item.POIVisible);
                    if (!item.POIVisible && isAllVisible)
                        isAllVisible = false;
                }

                if (isAllVisible)
                    cbUser.Checked = isAllVisible;

                customGridView1_SelectionChanged(null, null);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            //m_isLoadFinish = true;
        }

        private bool GetParentPOI(Dictionary<int, POIType> dicPOITypes, POIType poiType)
        {
            if (poiType == null)
                return true;

            dicPOITypes[poiType.ID] = poiType;

            if (poiType.Parent != null)
                GetParentPOI(dicPOITypes, poiType.Parent);

            return true;
        }

        private void LoadUserPOIType()
        {
            foreach (KeyValuePair<int, POIType> item in FormMain.Instance.BimManager.POITypes)
            {
                if (!item.Value.UserDefined)
                    continue;

                AddRow(customGridViewUser, item.Value, true);
                AddRow(customGridViewUserEdit, item.Value, true);
            }
        }

        /// <summary>
        /// 몇번째 단계의 POI인가?
        /// </summary>
        /// <param name="poiType"></param>
        /// <param name="dept"></param>
        /// <returns></returns>
        private POIType FindParentPOI(POIType poiType, ref int dept)
        {
            dept++;
            POIType returnPoiType = null;

            if (poiType.Parent == null)
            {
                returnPoiType = null;
            }
            else
            {
                POIType poiType2 = null;
                if (m_dicPOITypes.TryGetValue(poiType.Parent.ID, out poiType2))
                    returnPoiType = FindParentPOI(poiType2, ref dept);
                else
                    returnPoiType = null;
            }

            return returnPoiType;
        }

        private void AddRow(DataGridView gridView, POIType poiType, bool poiTypeVisible)
        {
            customGridView1.CellValueChanged -= customGridView_CellValueChanged;
            customGridView2.CellValueChanged -= customGridView_CellValueChanged;
            customGridView3.CellValueChanged -= customGridView_CellValueChanged;
            customGridView4.CellValueChanged -= customGridView_CellValueChanged;

            bool readOnly = true;

            if (gridView == customGridView2 || gridView == customGridView3 || gridView == customGridView4)
            {
                if (customGridView1.SelectedRows.Count > 0 && customGridView1.SelectedRows[0].Cells[1].Value != null && readOnly)
                    readOnly = (bool)customGridView1.SelectedRows[0].Cells[1].Value;
            }
            if (gridView == customGridView3 || gridView == customGridView4)
            {
                if (customGridView2.SelectedRows.Count > 0 && customGridView2.SelectedRows[0].Cells[1].Value != null && readOnly)
                    readOnly = (bool)customGridView2.SelectedRows[0].Cells[1].Value;
            }
            if (gridView == customGridView4)
            {
                if (customGridView3.SelectedRows.Count > 0 && customGridView3.SelectedRows[0].Cells[1].Value != null && readOnly)
                    readOnly = (bool)customGridView3.SelectedRows[0].Cells[1].Value;
            }
            
            int nRowIndex = gridView.Rows.Add();
            if (gridView == customGridViewUser)
            {
                gridView.Rows[nRowIndex].Cells[0].Value = poiType.ID;
                gridView.Rows[nRowIndex].Cells[1].Value = poiTypeVisible;
                gridView.Rows[nRowIndex].Cells[2].Value = poiType.Name;
                gridView.Rows[nRowIndex].Cells[3].Tag = poiType.Color;
                gridView.Rows[nRowIndex].ReadOnly = !readOnly;
            }
            else if (gridView == customGridViewUserEdit)
            {
                gridView.Rows[nRowIndex].Cells[0].Value = poiType.ID;
                gridView.Rows[nRowIndex].Cells[1].Value = poiType.Name;
                gridView.Rows[nRowIndex].Cells[2].Tag = poiType.Color;
                gridView.Rows[nRowIndex].ReadOnly = !readOnly; 
            }
            else
            {
                gridView.Rows[nRowIndex].Cells[0].Value = poiType.ID;
                gridView.Rows[nRowIndex].Cells[1].Value = poiTypeVisible;
                gridView.Rows[nRowIndex].Cells[2].Value = poiType.Name;
                gridView.Rows[nRowIndex].ReadOnly = !readOnly;
            }

            gridView.Rows[nRowIndex].Tag = poiType;            

            if (gridView == customGridView4)
            {
                System.Resources.ResourceManager rm = Properties.Resources.ResourceManager;
                Bitmap img = (Bitmap)rm.GetObject(poiType.Code); 
                if (img == null)
                    img = (Bitmap)rm.GetObject("empty");

                colIcon.ImageLayout = DataGridViewImageCellLayout.Zoom;
                
                gridView.Rows[nRowIndex].Cells[3].Value = img;
                gridView.Rows[nRowIndex].Cells[4].Tag = poiType.Color;
                gridView.Rows[nRowIndex].Cells[5].Value = "100";
            }

            customGridView1.CellValueChanged += customGridView_CellValueChanged;
            customGridView2.CellValueChanged += customGridView_CellValueChanged;
            customGridView3.CellValueChanged += customGridView_CellValueChanged;
            customGridView4.CellValueChanged += customGridView_CellValueChanged;
        }

        private _SqlDataReader ReadQuery(string strSQL, _SqlConnection connection, _SqlTransaction transaction)
        {
            try
            {
                _SqlCommand cmd = new _SqlCommand(strSQL, connection, transaction);
                return cmd.ExecuteReader();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return null;
        }
        
        private void customGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (customGridView1.SelectedCells == null || customGridView1.SelectedCells.Count == 0 || customGridView1.SelectedCells[0].Value == null)
                return;

            customGridView2.Rows.Clear();

            bool isAllVisible = true;
            int nID = (int)customGridView1.SelectedCells[0].Value;
            foreach (POITypeVisibleProperty item in m_gridViewData2)
            {
                if (item.POIType.Parent != null && item.POIType.Parent.ID == nID)
                {
                    AddRow(customGridView2, item.POIType, item.POIVisible);
                    if (!item.POIVisible && isAllVisible)
                        isAllVisible = false;
                }
            }
            
            cb2.Checked = isAllVisible;

            if (customGridView2.Rows.Count == 0)
            {
                customGridView3.Rows.Clear();
                customGridView4.Rows.Clear();
            }

            customGridView2_SelectionChanged(null, null);
        }

        private void customGridView2_SelectionChanged(object sender, EventArgs e)
        {
            if (customGridView2.SelectedCells == null || customGridView2.SelectedCells.Count == 0 || customGridView2.SelectedCells[0].Value == null)
                return;

            customGridView3.Rows.Clear();

            bool isAllVisible = true;
            int nID = (int)customGridView2.SelectedCells[0].Value;
            foreach (POITypeVisibleProperty item in m_gridViewData3)
            {
                if (item.POIType.Parent != null && item.POIType.Parent.ID == nID)
                {
                    AddRow(customGridView3, item.POIType, item.POIVisible);
                    if (!item.POIVisible && isAllVisible)
                        isAllVisible = false;
                }
            }
                        
            cb3.Checked = isAllVisible;

            if (customGridView3.Rows.Count == 0)
            {
                customGridView4.Rows.Clear();
            }

            customGridView3_SelectionChanged(null, null);
        }

        private void customGridView3_SelectionChanged(object sender, EventArgs e)
        {
            if (customGridView3.SelectedCells == null || customGridView3.SelectedCells.Count == 0 || customGridView3.SelectedCells[0].Value == null)
                return;

            customGridView4.Rows.Clear();

            bool isAllVisible = true;
            int nID = (int)customGridView3.SelectedCells[0].Value;
            foreach (POITypeVisibleProperty item in m_gridViewData4)
            {
                if (item.POIType.Parent != null && item.POIType.Parent.ID == nID)
                {
                    AddRow(customGridView4, item.POIType, item.POIVisible);
                    if (!item.POIVisible && isAllVisible)
                        isAllVisible = false;
                }
            }
            
            cb4.Checked = isAllVisible;

            //if (customGridView4.SelectedRows.Count > 0)
            //{
            //    DataGridViewRow row = customGridView4.SelectedRows[0];

            //    POIType poiType = row.Tag as POIType;
            //    if (poiType.Code.Length > 0)
            //    {
            //        string codeName = poiType.Code;

            //        System.Resources.ResourceManager rm = Properties.Resources.ResourceManager;

            //        // 선택된 행은 분홍색 이미지로 변경
            //        Bitmap img = (Bitmap)rm.GetObject(codeName + "_select");
            //        if (img == null)
            //        {
            //            img = (Bitmap)rm.GetObject(codeName);
            //            if (img == null)
            //                img = (Bitmap)rm.GetObject("empty");
            //        }

            //        row.Cells[3].Value = img;
            //    }
            //}
        }
        
        private void SetGridViewCheckBox(DataGridView gridView, POIType poiType, bool isVisible)
        {
            if (gridView == customGridView1)
                colCheck2.ReadOnly = !isVisible;
            if (gridView == customGridView1 || gridView == customGridView2)
                colCheck3.ReadOnly = !isVisible;
            if (gridView == customGridView1 || gridView == customGridView2 || gridView == customGridView3)
                colCheck4.ReadOnly = !isVisible;
        }
        
        private void cb_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb == null)
                return;

            bool check = cb.Checked;

            if (cb == cbAll)
            {
                customGridView1.CellContentClick -= customGridView_CellContentClick;
                customGridView2.CellContentClick -= customGridView_CellContentClick;
                customGridView3.CellContentClick -= customGridView_CellContentClick;
                customGridView4.CellContentClick -= customGridView_CellContentClick;
                customGridView1.CellValueChanged -= customGridView_CellValueChanged;
                customGridView2.CellValueChanged -= customGridView_CellValueChanged;
                customGridView3.CellValueChanged -= customGridView_CellValueChanged;
                customGridView4.CellValueChanged -= customGridView_CellValueChanged;

                foreach (POITypeVisibleProperty item in m_gridViewData1)
                {
                    item.POIVisible = check;
                    item.POIReadOnly = !check;
                }
                foreach (POITypeVisibleProperty item in m_gridViewData2)
                {
                    item.POIVisible = check;
                    item.POIReadOnly = !check;
                }
                foreach (POITypeVisibleProperty item in m_gridViewData3)
                {
                    item.POIVisible = check;
                    item.POIReadOnly = !check;
                }
                foreach (POITypeVisibleProperty item in m_gridViewData4)
                {
                    item.POIVisible = check;
                    item.POIReadOnly = !check;
                }
                cb1.Checked = cb2.Checked = cb3.Checked = cb4.Checked = check;

                customGridView1.CellContentClick += customGridView_CellContentClick;
                customGridView2.CellContentClick += customGridView_CellContentClick;
                customGridView3.CellContentClick += customGridView_CellContentClick;
                customGridView4.CellContentClick += customGridView_CellContentClick;
                customGridView1.CellValueChanged += customGridView_CellValueChanged;
                customGridView2.CellValueChanged += customGridView_CellValueChanged;
                customGridView3.CellValueChanged += customGridView_CellValueChanged;
                customGridView4.CellValueChanged += customGridView_CellValueChanged;

                customGridView1_SelectionChanged(null, null);
            }
            else
            {
                DataGridView gridView = null;
                if (cb == cb1)
                    gridView = customGridView1;
                else if (cb == cb2)
                    gridView = customGridView2;
                else if (cb == cb3)
                    gridView = customGridView3;
                else if (cb == cb4)
                    gridView = customGridView4;
                else if (cb == cbUser)
                    gridView = customGridViewUser;

                if (gridView.ReadOnly)
                    return;

                foreach (DataGridViewRow row in gridView.Rows)
                {
                    if (!row.ReadOnly)
                        row.Cells[1].Value = check;
                }
            }
        }

        private void SetCheckBoxEnable()
        {
            if (customGridView1.SelectedRows.Count > 0)
            {
                bool isCheck = (bool)customGridView1.SelectedRows[0].Cells[1].Value;
                cb2.Enabled = isCheck;
            }
            if (customGridView2.SelectedRows.Count > 0)
            {
                bool isCheck = (bool)customGridView2.SelectedRows[0].Cells[1].Value;
                cb3.Enabled = isCheck;
            }
            if (customGridView3.SelectedRows.Count > 0)
            {
                bool isCheck = (bool)customGridView3.SelectedRows[0].Cells[1].Value;
                cb4.Enabled = isCheck;
            }
        }

        private void customGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView gridview = sender as DataGridView;
            
            if (e.ColumnIndex != 1)
                return;
            
            gridview.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void customGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView gridview = sender as DataGridView;
            
            if (e.ColumnIndex != 1)
                return;

            cb1.CheckedChanged -= cb_CheckedChanged;
            cb2.CheckedChanged -= cb_CheckedChanged;
            cb3.CheckedChanged -= cb_CheckedChanged;
            cb4.CheckedChanged -= cb_CheckedChanged;
            cbUser.CheckedChanged -= cb_CheckedChanged;

            int id = (int)gridview.Rows[e.RowIndex].Cells[0].Value;
            bool check = (bool)gridview.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

            if (gridview == customGridView1)
            {
                bool isAllCheck = true;
                foreach (DataGridViewRow item in customGridView1.Rows)
                {
                    if (!(bool)item.Cells[1].Value)
                        isAllCheck = false;
                }                
                cb1.Checked = isAllCheck;

                foreach (DataGridViewRow item in customGridView2.Rows)
                {
                    item.ReadOnly = !check;
                }
            }

            if (gridview == customGridView1 || gridview == customGridView2)
            {
                bool isAllCheck = true;
                foreach (DataGridViewRow item in customGridView2.Rows)
                {
                    if (!(bool)item.Cells[1].Value)
                        isAllCheck = false;
                }
                cb2.Checked = isAllCheck;

                foreach (DataGridViewRow item in customGridView3.Rows)
                {
                    item.ReadOnly = !check;
                }
            }

            if (gridview == customGridView1 || gridview == customGridView2 || gridview == customGridView3)
            {
                bool isAllCheck = true;
                foreach (DataGridViewRow item in customGridView3.Rows)
                {
                    if (!(bool)item.Cells[1].Value)
                        isAllCheck = false;
                }
                cb3.Checked = isAllCheck;

                foreach (DataGridViewRow item in customGridView4.Rows)
                {
                    item.ReadOnly = !check;
                }
            }

            if (gridview == customGridView4)
            {
                bool isAllCheck = true;
                foreach (DataGridViewRow item in customGridView4.Rows)
                {
                    if (!(bool)item.Cells[1].Value)
                        isAllCheck = false;
                }
                cb4.Checked = isAllCheck;                
            }

            if (gridview == customGridViewUser)
            {
                bool isAllCheck = true;
                foreach (DataGridViewRow item in customGridViewUser.Rows)
                {
                    if (!(bool)item.Cells[1].Value)
                        isAllCheck = false;
                }
                cbUser.Checked = isAllCheck;
            }
             
            List<POITypeVisibleProperty> property = null;
            if (gridview == customGridView1)
                property = m_gridViewData1;
            else if (gridview == customGridView2)
                property = m_gridViewData2;
            else if (gridview == customGridView3)
                property = m_gridViewData3;
            else if (gridview == customGridView4)
                property = m_gridViewData4;
            else if (gridview == customGridViewUser)
                property = m_gridViewDataUser;

            foreach (POITypeVisibleProperty item in property)
            {
                if (item.POIType.ID == id)
                {
                    item.POIVisible = check;
                    break;
                }
            }

            SetCheckBoxEnable(); 
            
            cb1.CheckedChanged += cb_CheckedChanged;
            cb2.CheckedChanged += cb_CheckedChanged;
            cb3.CheckedChanged += cb_CheckedChanged;
            cb4.CheckedChanged += cb_CheckedChanged;
            cbUser.CheckedChanged += cb_CheckedChanged;

            if (m_isLoadFinish)
                SavePOITypeVisible();
        }

        private void customGridView4_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            foreach (DataGridViewRow row in customGridView4.Rows)
            {
                POIType poiType = row.Tag as POIType;
                if (poiType.Code.Length > 0)
                {
                    string codeName = poiType.Code;

                    System.Resources.ResourceManager rm = Properties.Resources.ResourceManager;

                    // 선택된 행은 분홍색 이미지로 변경
                    //if (row.Index == e.RowIndex)
                    //{
                    //    Bitmap img = (Bitmap)rm.GetObject(codeName + "_select");
                    //    if (img == null)
                    //    {
                    //        img = (Bitmap)rm.GetObject(codeName);
                    //        if (img == null)
                    //            img = (Bitmap)rm.GetObject("empty");
                    //    }

                    //    row.Cells[3].Value = img;
                    //}
                    //else
                    {
                        Bitmap img = (Bitmap)rm.GetObject(codeName);
                        if (img == null)
                            img = (Bitmap)rm.GetObject("empty");

                        row.Cells[3].Value = img;
                    }
                }
            }

            if (e.ColumnIndex != 4)
                return;

            ColorDialog dialog = new ColorDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Color selectedColor = dialog.Color;

                if (customGridView4.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag != null)
                {
                    if (customGridView4.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag.ToString().Length > 0)
                    {
                        Color orgColor = (Color)customGridView4.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag;
                        if (orgColor == selectedColor)
                            return;
                    }
                }

                customGridView4.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag = selectedColor;
                customGridView4.Refresh();

                customGridView4_CellEndEdit(sender, e);
            }
        }

        private void customGridView4_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (customGridView4.Rows[e.RowIndex].Cells[4].Tag == null)
                return;

            if (customGridView4.Rows[e.RowIndex].Cells[4].Tag.ToString().Length == 0)
                return;

            int id = (int)customGridView4.Rows[e.RowIndex].Cells[0].Value;
            if (m_dicPOITypes.ContainsKey(id))
            {
                string code = m_dicPOITypes[id].Code;
                POIType poi = m_dicPOITypes[id];
                Color color = (Color)customGridView4.Rows[e.RowIndex].Cells[4].Tag;

                //SavePOITypeColor(code, color);
                SavePOITypeColor(poi, color);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {            
            this.Close();
        }         

        private void SavePOITypeVisible()
        {
            m_dicPoiVisible.Clear();            
            foreach (POITypeVisibleProperty item in m_gridViewData1)
            {
                m_dicPoiVisible.Add(item.POIType.ID, item.POIVisible);
            }
            foreach (POITypeVisibleProperty item in m_gridViewData2)
            {
                m_dicPoiVisible.Add(item.POIType.ID, item.POIVisible);
            }
            foreach (POITypeVisibleProperty item in m_gridViewData3)
            {
                m_dicPoiVisible.Add(item.POIType.ID, item.POIVisible);
            }
            foreach (POITypeVisibleProperty item in m_gridViewData4)
            {
                m_dicPoiVisible.Add(item.POIType.ID, item.POIVisible);
            }
            foreach (POITypeVisibleProperty item in m_gridViewDataUser)
            {
                m_dicPoiVisible.Add(item.POIType.ID, item.POIVisible);
            }

            if (m_strProjectName.Length == 0 || m_strLevelXMLID.Length == 0)
                return;

            string key = "_" + m_strLevelXMLID;
            FormMain.Instance.BimManager.DicPOIVisible[key] = m_dicPoiVisible;

            foreach (Layer layer in m_layers)
            {
                if (layer.LayerType == typeof(POI))
                {
                    foreach (Shape shape in layer.Shapes)
                    {
                        POI poi = shape as POI;
                        bool visible = true;
                        if (!poi.PoiType.UserDefined)
                        {
                            int parentID = 0;
                            visible = GetPOIVisible(m_gridViewData4, poi.PoiType.ID, ref parentID);
                            
                            // 상세분류부터 POI가 있지 않기에
                            if (parentID == 0)
                                parentID = poi.PoiType.ID;

                            if (visible)
                                visible = GetPOIVisible(m_gridViewData3, parentID, ref parentID);
                            if (visible)
                                visible = GetPOIVisible(m_gridViewData2, parentID, ref parentID);
                            if (visible)
                                visible = GetPOIVisible(m_gridViewData1, parentID, ref parentID);
                        }
                        else
                            visible = m_dicPoiVisible[poi.PoiType.ID];

                        poi.POIVisible = visible;
                    }
                }
            }

            if (m_owner != null)
                m_owner.RefreshView();
        }

        public bool GetPOIVisible(List<POITypeVisibleProperty> list, int poiTypeID, ref int parentPoiID)
        {
            foreach (POITypeVisibleProperty item in list)
            {
                if (item.POIType.ID == poiTypeID)
                {
                    if (item.POIType.Parent != null)
                        parentPoiID = item.POIType.Parent.ID;
                    else
                        parentPoiID = 0;
                    return item.POIVisible;
                }
            }

            return true;
        }

        private void SavePOITypeColor(POIType poiType, Color color)
        {
            FormMain.Instance.BimManager.DicPOIColor[poiType.Code] = color;

            if (m_layers != null)
            {
                foreach (Layer layer in m_layers)
                {
                    if (layer.LayerType == typeof(POI))
                    {
                        foreach (Shape shape in layer.Shapes)
                        {
                            POI poi = shape as POI;
                            if (poi.PoiType.Code == poiType.Code && poi.PoiType.Code == "F9999" && poi.PoiType.Name == poiType.Name)
                            {                                
                                poi.PoiType.Color = color;
                                poi.FillColor = color;
                            }
                            else if (poi.PoiType.Code == poiType.Code && poi.PoiType.Code != "F9999")
                            {
                                poi.PoiType.Color = color;
                                poi.FillColor = color;
                            }
                        }
                    }
                } 
            }

            if (m_owner != null)
                m_owner.RefreshView();
        }
        
        public void SetLayers(List<Layer> layers, string projectName, string levelName, string levelXMLID, List<Shapes.POI> pois)
        {
            m_isLoadFinish = false;
            if (projectName.Length == 0 || levelXMLID.Length == 0)
            {
                this.strTitle = "도면 선택 안됨, 사용자 POI 수정만 가능";
                return;
            }

            m_strProjectName = projectName;
            m_strLevelXMLID = levelXMLID;
            m_POIs = pois;

            string key = "_" + m_strLevelXMLID;
            if (FormMain.Instance.BimManager.DicPOIVisible.ContainsKey(key))
                m_dicPoiVisible = FormMain.Instance.BimManager.DicPOIVisible[key];
            else
                m_dicPoiVisible = new Dictionary<int, bool>();

            bool changed = false;

            if (m_layers != layers)
            {
                m_layers = layers;
                changed = true;
            }

            LoadGridView();

            if (changed)
                SetControls();

            this.strTitle = m_strProjectName;
            //BIM.Level level = FormMain.Instance.GetLevel(levelID);
            //if (level != null)
                this.strTitle += " - " + levelName;

            m_isLoadFinish = true;

            if (m_isLoadFinish)
                SavePOITypeVisible();
        }

        private void SetControls()
        {
            customGridView1.Rows.Clear();
            customGridView2.Rows.Clear();
            customGridView3.Rows.Clear();
            customGridView4.Rows.Clear();
            customGridViewUser.Rows.Clear();
            customGridViewUserEdit.Rows.Clear();

            foreach (POITypeVisibleProperty item in m_gridViewData1)
            {
                if (m_dicPoiVisible.ContainsKey(item.POIType.ID))
                    item.POIVisible = m_dicPoiVisible[item.POIType.ID];
                else
                    item.POIVisible = true;
            }

            foreach (POITypeVisibleProperty item in m_gridViewData2)
            {
                if (m_dicPoiVisible.ContainsKey(item.POIType.ID))
                    item.POIVisible = m_dicPoiVisible[item.POIType.ID];
                else
                    item.POIVisible = true;
            }

            foreach (POITypeVisibleProperty item in m_gridViewData3)
            {
                if (m_dicPoiVisible.ContainsKey(item.POIType.ID))
                    item.POIVisible = m_dicPoiVisible[item.POIType.ID];
                else
                    item.POIVisible = true;
            }

            foreach (POITypeVisibleProperty item in m_gridViewData4)
            {
                if (m_dicPoiVisible.ContainsKey(item.POIType.ID))
                    item.POIVisible = m_dicPoiVisible[item.POIType.ID];
                else
                    item.POIVisible = true;
            }
            
            bool isAllVisible = true;
            foreach (POITypeVisibleProperty item in m_gridViewData1)
            {
                AddRow(customGridView1, item.POIType, item.POIVisible);
                if (!item.POIVisible && isAllVisible)
                    isAllVisible = false;
            }

            if (isAllVisible)
                cb1.Checked = isAllVisible;

            foreach (POITypeVisibleProperty item in m_gridViewDataUser)
            {
                if (m_dicPoiVisible.ContainsKey(item.POIType.ID))
                    item.POIVisible = m_dicPoiVisible[item.POIType.ID];
                else
                    item.POIVisible = true;
            }

            foreach (POITypeVisibleProperty item in m_gridViewDataUser)
            {
                AddRow(customGridViewUser, item.POIType, item.POIVisible);
                AddRow(customGridViewUserEdit, item.POIType, item.POIVisible);
                if (!item.POIVisible && isAllVisible)
                    isAllVisible = false;
            }
            if (isAllVisible)
                cbUser.Checked = isAllVisible;

            customGridView1_SelectionChanged(null, null);
        }

        private void customGridViewUserEdit_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (customGridViewUserEdit.Rows[e.RowIndex].Cells[1].Value == null)
                return;

            if (customGridViewUserEdit.Rows[e.RowIndex].Cells[2].Tag == null)
                return;

            if (customGridViewUserEdit.Rows[e.RowIndex].Cells[2].Tag.ToString().Length == 0)
                return;

            int id = (int)customGridViewUserEdit.Rows[e.RowIndex].Cells[0].Value;
            string name = customGridViewUserEdit.Rows[e.RowIndex].Cells[1].Value.ToString();
            Color color = (Color)customGridViewUserEdit.Rows[e.RowIndex].Cells[2].Tag;

            UpdateUserPOIType(id, name, color, customGridViewUserEdit.Rows[e.RowIndex]);
        }

        private void customGridViewUserEdit_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 2)
                return;

            ColorDialog dialog = new ColorDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {                
                Color selectedColor = dialog.Color;

                if (customGridViewUserEdit.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag != null)
                {
                    if (customGridViewUserEdit.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag.ToString().Length > 0)
                    {
                        Color orgColor = (Color)customGridViewUserEdit.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag;
                        if (orgColor == selectedColor)
                            return;  
                    }
                }

                customGridViewUserEdit.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag = selectedColor;                
                customGridViewUserEdit.Refresh();

                customGridViewUserEdit_CellEndEdit(sender, e);
            }
        }

        private void customGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridView gridview = sender as DataGridView;
            if (gridview == customGridView4)
            {
                if (e.ColumnIndex != 4)
                    return;
            }
            else if (gridview == customGridViewUser)
            {
                if (e.ColumnIndex != 3)
                    return;
            }
            else if (gridview == customGridViewUserEdit)
            {
                if (e.ColumnIndex != 2)
                    return;
            }

            if (gridview.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag == null)
                return;
            if (gridview.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag.ToString().Length == 0)
                return;
            
            e.PaintBackground(e.CellBounds, false);

            Color color = (Color)gridview.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag;

            Brush br = new SolidBrush(color);
            Pen pen = new Pen(color);

            int x = e.CellBounds.X + (e.CellBounds.Width / 2) - (13 / 2);
            int y = e.CellBounds.Y + (e.CellBounds.Height / 2) - (13 / 2);
            Rectangle rect = new Rectangle(x, y, 13, 13);
            if (gridview == customGridView4)
            {
                e.Graphics.FillRectangle(br, rect);
            }
            else
            {
                e.Graphics.DrawEllipse(pen, rect);
                e.Graphics.FillPie(br, rect, 360, -360);
            }
            e.PaintContent(rect);
            e.Handled = true;
        }

        private int m_nUserGroupID = -1;
        /// <summary>
        /// 사용자 POIType 그룹 한 세트가 있는지 여부, 3단계의 그룹이 필요함.
        /// 있으면 소분류 그룹 ID를 Return, 없으면 한 세트를 만든다.
        /// </summary>
        /// <returns></returns>
        private void IsUserPOIType()
        {
            List<POIType> userPOITypeGroup = new List<POIType>();
            foreach (KeyValuePair<int, POIType> item in m_dicPOITypes)
            {
                //if (item.Value.UserDefined && item.Value.IsGroup)
                if (item.Value.Name == "사용자 POIType 그룹" && item.Value.IsGroup)
                {
                    userPOITypeGroup.Add(item.Value);
                }
            }

            //int totalCnt = 0;
            //foreach (POIType poiType in userPOITypeGroup)
            //{
            //    int nDept = 0;
            //    FindParentPOI(poiType, ref nDept);

            //    totalCnt += nDept;
            //}

            //if (totalCnt == 6) // 사용자 그룹 있음
            //{
            //    m_nUserGroupID = userPOITypeGroup[userPOITypeGroup.Count - 1].ID;
            //}
            if (userPOITypeGroup.Count > 0) // 사용자 그룹 있음
            {
                m_nUserGroupID = userPOITypeGroup[userPOITypeGroup.Count - 1].ID;
            }
            else // 없음
            {
#if DB_USE
                int nID = FormMain.Instance.GetMaxTableID("POIType", m_connection) + 1;
                if (nID <= 0)
                    return;

                List<POIType> newUserType = new List<POIType>();
                newUserType.Add(new POIType() { ID = nID, Name = "사용자 POIType 그룹", UserDefined = true });
                newUserType.Add(new POIType() { ID = ++nID, Name = "사용자 POIType 그룹", UserDefined = true, Parent = newUserType[0] });
                newUserType.Add(new POIType() { ID = ++nID, Name = "사용자 POIType 그룹", UserDefined = true, Parent = newUserType[1] });

                _SqlTransaction transaction = new _SqlTransaction(m_connection.DBType, m_connection);

                foreach (POIType item in newUserType)
                {
                    string strQuery = string.Format(
                        "INSERT INTO POIType (ID, IsGroup, ParentID, Name, IsUserDefined) VALUES ({0}, 1, {1}, '{2}', 1)"
                        , item.ID, (item.Parent == null) ? "NULL" : item.Parent.ID.ToString(), item.Name);

                    if (!FormMain.Instance.BimManager.ExecuteQuery(strQuery, m_connection, transaction))
                        transaction.Rollback(m_connection.DBType);
                }

                transaction.Commit(m_connection.DBType);
                m_nUserGroupID = newUserType[newUserType.Count - 1].ID;

                foreach (POIType item in newUserType)
                {
                    FormMain.Instance.BimManager.POITypes.Add(item.ID, item);
                }
#endif

#if XML_USE
                List<POIType> newUserType = new List<POIType>();

                string strXMLID = POIType.POITypeIDTag + System.Guid.NewGuid().ToString();
                int nID = strXMLID.GetHashCode();
                newUserType.Add(new POIType() { XMLID = strXMLID, ID = nID, Name = "사용자 POIType 그룹", UserDefined = true, IsGroup = true, Code = "F9" });

                strXMLID = POIType.POITypeIDTag + System.Guid.NewGuid().ToString();
                nID = strXMLID.GetHashCode();
                newUserType.Add(new POIType() { XMLID = strXMLID, ID = nID, Name = "사용자 POIType 그룹", UserDefined = true, Parent = newUserType[0], IsGroup = true, Code = "F99" });

                strXMLID = POIType.POITypeIDTag + System.Guid.NewGuid().ToString();
                nID = strXMLID.GetHashCode();
                newUserType.Add(new POIType() { XMLID = strXMLID, ID = nID, Name = "사용자 POIType 그룹", UserDefined = true, Parent = newUserType[1], IsGroup = true, Code = "F999" });

                m_nUserGroupID = newUserType[newUserType.Count - 1].ID;

                foreach (POIType item in newUserType)
                {
                    FormMain.Instance.BimManager.POITypes.Add(item.ID, item);
                }

                XMLManager mgr = new XMLManager();
                mgr.Save(m_project, m_dicPOITypes);
#endif
            }
        }

#if DB_USE
        private void UpdateUserPOIType(int nID, string strName, Color color, DataGridViewRow row)
        {
            m_connection.Open();

            string strColor = color.R + "," + color.G + "," + color.B;
            string strQuery = "";
            bool isNew = false;
            if (nID == 0)
            {
                nID = FormMain.Instance.GetMaxTableID("POIType", m_connection) + 1;
                if (nID <= 0)
                    return;

                strQuery = string.Format(
                    "INSERT INTO POIType (ID, IsGroup, ParentID, Name, Code, IsUserDefined) VALUES({0}, 0, {2}, '{1}', '', 1)"
                    , nID, strName, m_nUserGroupID);

                isNew = true;
            }
            else
            {
                strQuery = string.Format("UPDATE POIType SET Name='{0}' WHERE ID={1}", strName, nID);
            }
            
            if (FormMain.Instance.BimManager.ExecuteQuery(strQuery, m_connection, null))
            {
                m_connection.Close();

                POIType poiType = null;
                
                if (isNew)
                {
                    poiType = new POIType();
                    poiType.ID = nID;
                    poiType.Name = strName;
                    poiType.ParentID = 0;
                    poiType.UserDefined = true;
                    poiType.Color = color;
                    poiType.POIVisible = true;
                    FormMain.Instance.BimManager.POITypes.Add(nID, poiType);

                    row.Cells[0].Value = nID;

                    AddRow(customGridViewUser, poiType, true);
                }
                else
                {
                    if (FormMain.Instance.BimManager.POITypes.ContainsKey(nID))
                    {
                        poiType = FormMain.Instance.BimManager.POITypes[nID];
                        poiType.Name = strName;
                        poiType.Color = color;
                    }

                    foreach (DataGridViewRow item in customGridViewUser.Rows)
                    {
                        if ((int)item.Cells[0].Value == nID)
                        {
                            item.Cells[2].Value = strName;
                            item.Cells[3].Tag = color;
                            customGridViewUser.Refresh();
                            break;
                        }
                    }
                }

                SavePOITypeColor(nID, color);

                FormMain.Instance.SetPOIComboList(poiType, isNew);
            }
        }
#endif

#if XML_USE
        private void UpdateUserPOIType(int nID, string strName, Color color, DataGridViewRow row)
        {
            bool isNew = false;
            string strXMLID = "";

            if (nID == 0)
            {
                strXMLID = POIType.POITypeIDTag + System.Guid.NewGuid().ToString();
                nID = strXMLID.GetHashCode();
                isNew = true;
            }

            POIType poiType = null;

            if (isNew)
            {
                poiType = new POIType();
                poiType.ID = nID;
                poiType.XMLID = strXMLID;
                poiType.Name = strName;
                poiType.ParentID = m_nUserGroupID;
                poiType.Parent = m_dicPOITypes[m_nUserGroupID];
                poiType.UserDefined = true;
                poiType.Color = color;
                poiType.Code = "F9999";
                //poiType.POIVisible = true;
                FormMain.Instance.BimManager.POITypes.Add(nID, poiType);

                row.Cells[0].Value = nID;

                AddRow(customGridViewUser, poiType, true);
            }
            else
            {
                if (FormMain.Instance.BimManager.POITypes.ContainsKey(nID))
                {
                    poiType = FormMain.Instance.BimManager.POITypes[nID];
                    poiType.Name = strName;
                    poiType.Color = color;
                }

                foreach (DataGridViewRow item in customGridViewUser.Rows)
                {
                    if ((int)item.Cells[0].Value == nID)
                    {
                        item.Cells[2].Value = strName;
                        item.Cells[3].Tag = color;
                        customGridViewUser.Refresh();
                        break;
                    }
                }
            }

            //SavePOITypeColor(poiType.Code, color);
            SavePOITypeColor(poiType, color);

            // TODO: 사용자 POI 문제 부분 >> 확인 필요
            FormMain.Instance.SetPOIComboList(poiType, isNew);

            XMLManager mgr = new XMLManager();
            mgr.Save(m_project, m_dicPOITypes);
        }
#endif

#if DB_USE
        private void DeleteUserPOIType(int nID)
        {
            DialogResult digResult = MessageBox.Show("모든 도면에서 해당 POI가 삭제됩니다.\r삭제할까요?", "", MessageBoxButtons.YesNo);
            if (digResult != DialogResult.Yes)
                return;

            m_connection.Open();            

            _SqlTransaction transaction = new _SqlTransaction(m_connection.DBType, m_connection);

            string strQuery = string.Format("DELETE FROM POI WHERE TypeID={0}", nID);

            if (FormMain.Instance.BimManager.ExecuteQuery(strQuery, m_connection, transaction))
            {
                strQuery = string.Format("DELETE FROM POIType WHERE ID={0}", nID);

                if (FormMain.Instance.BimManager.ExecuteQuery(strQuery, m_connection, transaction))
                {
                    if (FormMain.Instance.BimManager.POITypes.ContainsKey(nID))
                    {
                        FormMain.Instance.BimManager.POITypes.Remove(nID);
                    }

                    DataGridViewRow deleteRow = null;
                    POIType deletePOIType = null;
                    foreach (DataGridViewRow item in customGridViewUser.Rows)
                    {
                        int rowID = (int)item.Cells[0].Value;
                        if (rowID == nID)
                        {
                            deletePOIType = item.Tag as POIType;
                            deleteRow = item;
                            break;
                        }
                    }
                    if (deleteRow != null)
                        customGridViewUser.Rows.Remove(deleteRow);

                    if (deletePOIType != null)
                        FormMain.Instance.SetPOIComboList(deletePOIType, false, true);
                    
                    FormMain.Instance.DeleteUserPOI(nID);

                    transaction.Commit(m_connection.DBType);                    
                }
                else
                    transaction.Rollback(m_connection.DBType);
            }
            else
                transaction.Rollback(m_connection.DBType);

            m_connection.Close();
        }
#endif

#if XML_USE
        private void DeleteUserPOIType(int nID)
        {
            DialogResult digResult = MessageBox.Show("모든 도면에서 해당 POI가 삭제됩니다.\r삭제할까요?", "", MessageBoxButtons.YesNo);
            if (digResult != DialogResult.Yes)
                return;

            if (FormMain.Instance.BimManager.POITypes.ContainsKey(nID))
            {
                FormMain.Instance.BimManager.POITypes.Remove(nID);
            }

            DataGridViewRow deleteRow = null;
            POIType deletePOIType = null;
            foreach (DataGridViewRow item in customGridViewUser.Rows)
            {
                int rowID = (int)item.Cells[0].Value;
                if (rowID == nID)
                {
                    deletePOIType = item.Tag as POIType;
                    deleteRow = item;
                    break;
                }
            }
            if (deleteRow != null)
                customGridViewUser.Rows.Remove(deleteRow);

            if (deletePOIType != null)
                FormMain.Instance.SetPOIComboList(deletePOIType, false, true);

            FormMain.Instance.DeleteUserPOI(nID);

            XMLManager mgr = new XMLManager();
            mgr.Save(m_project, m_dicPOITypes);
        }
#endif

        private void customGridViewUserEdit_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            customGridViewUserEdit.Rows[e.RowIndex].Cells[0].Value = 0;
            customGridViewUserEdit.Rows[e.RowIndex].Cells[2].Tag = Color.Yellow;
        }

        private void customGridViewUserEdit_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            int nID = (int)e.Row.Cells[0].Value;
            if (nID > 0)
                DeleteUserPOIType(nID);
        }
    }

    public class POITypeVisibleProperty
    {
        public POIType POIType { get; set; }
        public bool POIVisible { get; set; } // 로컬에 저장
        public bool POIReadOnly { get; set; } // GridView ReadOnly 로컬 저장안함
        
        public POITypeVisibleProperty(POIType poiType, bool visible)
        {
            this.POIType = poiType;
            this.POIVisible = visible;
        }
    }
}
