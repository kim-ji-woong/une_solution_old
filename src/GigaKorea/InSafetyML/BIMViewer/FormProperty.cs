using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BIMViewer
{
    using Shapes;
    using BIM;
    using BIMViewer.PopupForms;

    public partial class FormProperty : PopupFormBase
    {
        private Shape m_shape = null;
        private bool m_systemInput = false;
        private IUIMaster m_master = null;
        public bool OpenSucFrm = true;
        public Shape SelectedShape
        {
            get { return m_shape; }
            set
            {
                if (m_shape != value)
                {
                    m_shape = value;
                    SetShape();
                }
            }
        }

        public FormProperty(IUIMaster master)
        {
            InitializeComponent();

            btnClose.Parent = this.PnTitle;
            btnClose.Location = new Point(342, 2);

            m_master = master;
        }

        private void SetShape()
        {
            if (m_shape == null)
            {
                labelShapeType.Text = "선택된 개체 없음";

                textBoxProperty.Visible = false;
                pnSpace.Visible = lblSpaceName.Visible = false;
                cboSafetyFire.Visible = false;
            }
            else
            {
                pnSpace.Visible = pnPOI.Visible = pnWall.Visible = pnUser.Visible = pnDoor.Visible = pnWindow.Visible = false;

                TableLayoutPanel pn = null;
                if (m_shape is Space)
                {
                    pn = pnSpace;
                    pnSpace.Visible = true;                    
                }
                else if (m_shape is POI)
                {
                    POI poi = m_shape as POI;
                    if (poi.PoiType.UserDefined) // 사용자 POI
                    {
                        pn = pnUser;
                        pnUser.Visible = true;
                    }
                    else
                    {
                        pn = pnPOI;
                        pnPOI.Visible = true;
                    }
                }
                else if (m_shape is Wall)
                {
                    pn = pnWall;
                    pnWall.Visible = true;
                }
                else if (m_shape is Door)
                {
                    pn = pnDoor;
                    pnDoor.Visible = true;
                }
                else if (m_shape is Window)
                {
                    pn = pnWindow;
                    pnWindow.Visible = true;
                }

                if (pn == null)
                {
                    OpenSucFrm = false;
                    return;
                }

                OpenSucFrm = true;
 
                pn.Location = new Point(15, 50);
                this.Size = new Size(382, pn.Location.Y + pn.Height + 20);
               
                SetPropertyValue();
            }
        }

        private void SetPropertyValue()
        {
            string strProperty = "";

            if (m_shape is Wall)
            {
                labelShapeType.Text = this.strTitle = "벽";

                Wall wall = (Wall)m_shape;
                lblWallType.Text = "벽";
                lblWallName.Text = wall.Component.ComponentName;
                lblWallMaterial.Text = wall.Component.TypeName;
                lblWallHeight.Text = "";
               

                strProperty = string.Format("타입 : {0}\r\n재질 : {1}", wall.Component.ComponentName, wall.Component.TypeName);
            }
            else if (m_shape is Door)
            {
                labelShapeType.Text = this.strTitle = "문";

                Door door = (Door)m_shape;
                lblDoor.Text = "문";
                lblDoorType.Text = door.GetDoorTypeName();
                lblDoorID.Text = door.XMLID;
                
                strProperty = string.Format("타입 : {0}", door.GetDoorTypeName());
            }
            else if (m_shape is Window)
            {
                labelShapeType.Text = this.strTitle = "창문";

                Window window = (Window)m_shape;
                lblWindow.Text = "창문";
                lblWindowID.Text = window.XMLID;
            }
            else if (m_shape is POI)
            {
                labelShapeType.Text = this.strTitle = "POI";
                lblPoiType1.Text = "";
                lblPoiType2.Text = "";
                lblPoiType3.Text = "";
                lblPoiType4.Text = "";

                lblPoiRx.Text = "";
                lblPoiLoop.Text = "";
                lblPoiAddress.Text = "";
                lblPoiChannel.Text = "";

                POI poi = (POI)m_shape;
                if (!poi.PoiType.UserDefined)
                {
                    int parentID = poi.PoiType.Parent.ID;
                    int index = 3;
                    //if (poi.PoiType.Code.Contains("LG"))
                    if (poi.PoiType.Code.Length == 2)
                    {
                        index = 0;
                        lblPoiType1.Text = poi.PoiType.Name;
                    }
                    //else if (poi.PoiType.Code.Contains("MD"))
                    else if (poi.PoiType.Code.Length == 3)
                    {
                        index = 1;
                        lblPoiType2.Text = poi.PoiType.Name;
                    }
                    //else if (poi.PoiType.Code.Contains("SM"))
                    else if (poi.PoiType.Code.Length == 4)
                    {
                        index = 2;
                        lblPoiType3.Text = poi.PoiType.Name;
                    }
                    //else if (poi.PoiType.Code.Contains("DT"))
                    else if (poi.PoiType.Code.Length == 5)
                    {
                        index = 3;
                        lblPoiType4.Text = poi.PoiType.Name;
                    }

                    lblPoiID.Text = poi.XMLID;
                    lblPoiHeight.Text = poi.Height.ToString();

                    foreach (Property property in poi.Properties)
                    {
                        if (property.Name == "rx")
                            lblPoiRx.Text = property.Value;

                        if (property.Name == "loop")
                            lblPoiLoop.Text = property.Value;

                        if (property.Name == "address")
                            lblPoiAddress.Text = property.Value;

                        if (property.Name == "channel")
                            lblPoiChannel.Text = property.Value;

                    }

                    while (index > 0)
                    {
                        POIType parentPOI = null;
                        foreach (KeyValuePair<int, POIType> item in FormMain.Instance.BimManager.POITypes)
                        {
                            if (item.Value.ID == parentID)
                            {
                                parentPOI = item.Value;
                                parentID = (item.Value.Parent == null) ? 0 : item.Value.Parent.ID;
                                break;
                            }
                        }

                        //if (index == 3)
                        if (parentPOI.Code.Length == 4)
                        {
                            lblPoiType3.Text = parentPOI.Name;
                            lblPoiType3.Tag = parentPOI;
                            index = 3;
                        }
                        else if (parentPOI.Code.Length == 3)
                        {
                            lblPoiType2.Text = parentPOI.Name;
                            lblPoiType2.Tag = parentPOI;
                            index = 2;
                        }
                        else if (parentPOI.Code.Length == 2)
                        {
                            lblPoiType1.Text = parentPOI.Name;
                            lblPoiType1.Tag = parentPOI;
                            index = 1;
                        }
                        index--;
                    } 
                }
                else
                {
                    lblUserPoiName.Text = poi.PoiType.Name;
                    lblUserPoiID.Text = poi.PoiType.ID.ToString();
                    lblUserPoiHeight.Text = poi.Height.ToString();
                }

                strProperty = string.Format("타입 : {0}", poi.PoiType.Name);
            }
            else if (m_shape is Space)
            {
                Space space = (Space)m_shape;
                lblSpaceName.Text = space.Name;
                lblSpaceID.Text = space.XMLID;
                lblSpaceType.Text = "공간";

                m_systemInput = true;
                cboSafetyFire.SelectedIndex = space.SafetyFire ? 1 : 0;
                m_systemInput = false;

                labelShapeType.Text = this.strTitle = "공간";
            }

            textBoxProperty.Text = strProperty;

            this.BringToFront();
        }

        private void cboSafetyFire_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_systemInput || m_master == null)
                return;

            if (m_shape != null && m_shape is Space)
            {
                if (m_master.SaveShapeProperty(m_shape))
                {
                    Space space = (Space)m_shape;
                    space.SafetyFire = cboSafetyFire.SelectedIndex == 1;
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
