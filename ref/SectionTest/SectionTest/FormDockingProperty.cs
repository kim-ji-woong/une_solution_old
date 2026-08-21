using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XtremePropertyGrid;

namespace section
{
    public partial class FormDockingProperty : Form
    {
        private PropertyGridItem m_GridItemSizeRectWidth = null;
        private PropertyGridItem m_GridItemSizeRectHeight = null;
        private PropertyGridItem m_GridItemLocationRectX = null;
        private PropertyGridItem m_GridItemLocationRectY = null;
        private PropertyGridItem m_GridItemText = null;
        private PropertyGridItem m_GridItemBackColor = null;

        private SectionTree m_SectionCurrent = null;        
        private PageHome m_FormMain = null;

        public AxXtremePropertyGrid.AxPropertyGrid PropertyGrid
        {
            get { return axPropertyGrid1; }
            set { axPropertyGrid1 = value; }
        }
        public section.SectionTree SectionCurrent
        {
            get { return m_SectionCurrent; }
            set { m_SectionCurrent = value; }
        }
        
        public FormDockingProperty(PageHome main)
        {
            m_FormMain = main;
            InitializeComponent();

            PropertyGridItem itemCategory = PropertyGrid.AddCategory("Section Settings");

            m_GridItemText = itemCategory.AddChildItem(PropertyItemType.PropertyItemString, "Text");
            m_GridItemText.Id = ID.ID_TEXT_TEXT;

            PropertyGridItem itemSize = itemCategory.AddChildItem(PropertyItemType.PropertyItemString, "Size");
            itemSize.ReadOnly = true;

            m_GridItemSizeRectWidth = itemSize.AddChildItem(PropertyItemType.PropertyItemNumber, "Width");
            m_GridItemSizeRectWidth.Id = ID.ID_SIZE_WIDTH;
            m_GridItemSizeRectHeight = itemSize.AddChildItem(PropertyItemType.PropertyItemNumber, "Hieght");
            m_GridItemSizeRectHeight.Id = ID.ID_SIZE_HEIGHT;

            PropertyGridItem itemLocation = itemCategory.AddChildItem(PropertyItemType.PropertyItemString, "Location");
            itemLocation.ReadOnly = true;

            m_GridItemLocationRectX = itemLocation.AddChildItem(PropertyItemType.PropertyItemNumber, "X");
            m_GridItemLocationRectX.Id = ID.ID_LOCATION_X;
            m_GridItemLocationRectY = itemLocation.AddChildItem(PropertyItemType.PropertyItemNumber, "Y");
            m_GridItemLocationRectY.Id = ID.ID_LOCATION_Y;

            itemCategory.Expanded = true;
            itemSize.Expanded = true;
            itemLocation.Expanded = true;
        }

        private void FormDockingProperty_Load(object sender, EventArgs e)
        {
            
        }

        public void SetValue(SectionTree tree)
        {
            m_SectionCurrent = null;
            if (m_GridItemSizeRectWidth != null)
            {
                m_GridItemSizeRectWidth.Value = tree.Rect.Width;
                m_GridItemSizeRectHeight.Value = tree.Rect.Height;
                m_GridItemLocationRectX.Value = tree.Rect.X;
                m_GridItemLocationRectY.Value = tree.Rect.Y;
                m_GridItemText.Value = tree.textBox1.Text;
            }
            m_SectionCurrent = tree;
        }

       
        private void PropertyGrid_ValueChanged(object sender, AxXtremePropertyGrid._DPropertyGridEvents_ValueChangedEvent e)
        {
            if (m_SectionCurrent != null)
            {
                if (e.item.Id == ID.ID_TEXT_TEXT)
                {
                    m_SectionCurrent.textBox1.Text = e.item.Value.ToString();
                }
                else
                {
                    int x = m_SectionCurrent.Rect.Location.X;
                    int y = m_SectionCurrent.Rect.Location.Y;
                    int width = m_SectionCurrent.Rect.Width;
                    int height = m_SectionCurrent.Rect.Height;

                    if (e.item.Id == ID.ID_SIZE_WIDTH)
                    {
                        width = (int)e.item.Value;
                    }
                    if (e.item.Id == ID.ID_SIZE_HEIGHT)
                    {
                        height = (int)e.item.Value;
                    }
                    if (e.item.Id == ID.ID_LOCATION_X)
                    {
                        x = (int)e.item.Value;
                    }
                    if (e.item.Id == ID.ID_LOCATION_Y)
                    {
                        y = (int)e.item.Value;
                    }
                    m_SectionCurrent.SetLocation(x, y, width, height);

                }

                if (m_FormMain != null)
                    m_FormMain.Refresh();

            }     
        }
    }
}
