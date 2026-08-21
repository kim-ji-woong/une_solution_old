using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOPMonitoringSystem
{
    public partial class FormPreviewContainer : Form
    {
        private PreviewComponentContainer mContainer = null;
        public FormPreviewContainer()
        {
            InitializeComponent();

            btnCollapse.BackgroundImage = GetImage(false);
            ToolTip tooltip = new ToolTip();
            tooltip.SetToolTip(btnCollapse, "전체 닫기");

            btnExpand.BackgroundImage = GetImage(true);
            ToolTip tooltip2 = new ToolTip();
            tooltip2.SetToolTip(btnExpand, "전체 열기");
        }
        
        public void AddPreviewContainer(PreviewComponentContainer container)
        {
            mContainer = container;
            container.AutoScroll = true;
            container.Dock = DockStyle.Fill;            
            this.Controls.Add(container);
            container.BringToFront();            
        }

        private void btnCollapse_Click(object sender, EventArgs e)
        {
            if (mContainer == null)
                return;

            mContainer.CollapseAll();
        }

        private void btnExpand_Click(object sender, EventArgs e)
        {
            if (mContainer == null)
                return;

            mContainer.ExpandAll();
        }

        public Image GetImage(bool isFlag)
        {
            Bitmap bmp = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.btn_arrow2);

            ImageList imgList = new ImageList();
            imgList.ImageSize = new Size(32, 32);
            imgList.Images.AddStrip(bmp);

            int nFlag = 0;
            if (!isFlag) nFlag = 1;

            Image img = imgList.Images[nFlag];

            return img;
        }
    }
}
