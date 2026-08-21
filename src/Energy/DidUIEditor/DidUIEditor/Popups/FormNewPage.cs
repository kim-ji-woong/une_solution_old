using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DidUIEditor.Popups
{
    /// <summary>
    /// 페이지 추가
    /// </summary>
    public partial class FormNewPage : Form
    {
        public List<Page> ReturnPages = null;

        private List<Image> m_gridImgs = null;
        private List<Image> m_imgs = null;
        public FormNewPage()
        {
            InitializeComponent();            
        }

        private void FormNewPage_Load(object sender, EventArgs e)
        {
            m_gridImgs = new List<Image>();
            m_imgs = new List<Image>();

            if (FormMain.Instance.Mode == Mode.Normal)
            {
                lblDisasterType.Visible = cbDisasterType.Visible = false;

                m_gridImgs.Add(global::DidUIEditor.Properties.Resources._0_기본_grid);
                m_gridImgs.Add(global::DidUIEditor.Properties.Resources._1_근로자현황_grid);
                m_gridImgs.Add(global::DidUIEditor.Properties.Resources._2_안전조치사항_일반위험작업__grid);
                m_gridImgs.Add(global::DidUIEditor.Properties.Resources._3_안전조치사항_화재작업__grid);
                m_gridImgs.Add(global::DidUIEditor.Properties.Resources._4_안전조치사항_정전작업__grid);
                m_gridImgs.Add(global::DidUIEditor.Properties.Resources._5_안전조치사항_밀폐공간작업__grid);
                m_gridImgs.Add(global::DidUIEditor.Properties.Resources._6_안전조치사항_고소작업__grid);
                m_gridImgs.Add(global::DidUIEditor.Properties.Resources._7_안전조치사항_굴착_작업__grid);
                m_gridImgs.Add(global::DidUIEditor.Properties.Resources._9_안전조치사항_grid);
                m_gridImgs.Add(global::DidUIEditor.Properties.Resources._10_안전조치사항_grid);
                m_gridImgs.Add(global::DidUIEditor.Properties.Resources._11_안전조치사항_grid);
                m_gridImgs.Add(global::DidUIEditor.Properties.Resources._12_안전조치사항_grid);
                m_gridImgs.Add(global::DidUIEditor.Properties.Resources._13_안전조치사항_grid);
                m_gridImgs.Add(global::DidUIEditor.Properties.Resources._14_안전조치사항_grid);
                m_gridImgs.Add(global::DidUIEditor.Properties.Resources._15_안전조치사항_grid);
                m_gridImgs.Add(global::DidUIEditor.Properties.Resources._16_안전조치사항_grid);
                m_gridImgs.Add(global::DidUIEditor.Properties.Resources._17_안전조치사항_grid);
                m_gridImgs.Add(global::DidUIEditor.Properties.Resources._18_안전조치사항_grid);
                m_gridImgs.Add(global::DidUIEditor.Properties.Resources._19_안전조치사항_grid);
                m_gridImgs.Add(global::DidUIEditor.Properties.Resources._20_안전조치사항_grid);
                m_gridImgs.Add(global::DidUIEditor.Properties.Resources._21_안전조치사항_grid);
                m_gridImgs.Add(global::DidUIEditor.Properties.Resources._22_안전조치사항_grid);
                m_gridImgs.Add(global::DidUIEditor.Properties.Resources._23_안전조치사항_grid);
                m_gridImgs.Add(global::DidUIEditor.Properties.Resources._24_안전조치사항_grid);
                m_gridImgs.Add(global::DidUIEditor.Properties.Resources._25_안전조치사항_grid);
                m_gridImgs.Add(global::DidUIEditor.Properties.Resources._26_안전조치사항_grid);
                m_gridImgs.Add(global::DidUIEditor.Properties.Resources._27_안전조치사항_grid);
                m_gridImgs.Add(global::DidUIEditor.Properties.Resources._28_안전조치사항_grid);
                m_gridImgs.Add(global::DidUIEditor.Properties.Resources._29_안전조치사항_grid);
                m_gridImgs.Add(global::DidUIEditor.Properties.Resources._30_안전조치사항_grid);
                m_gridImgs.Add(global::DidUIEditor.Properties.Resources._31_안전조치사항_grid);

                m_imgs.Add(global::DidUIEditor.Properties.Resources._0_기본);
                m_imgs.Add(global::DidUIEditor.Properties.Resources._1_근로자현황);
                m_imgs.Add(global::DidUIEditor.Properties.Resources._2_안전조치사항_일반위험작업_);
                m_imgs.Add(global::DidUIEditor.Properties.Resources._3_안전조치사항_화재작업_);
                m_imgs.Add(global::DidUIEditor.Properties.Resources._4_안전조치사항_정전작업_);
                m_imgs.Add(global::DidUIEditor.Properties.Resources._5_안전조치사항_밀폐공간작업_);
                m_imgs.Add(global::DidUIEditor.Properties.Resources._6_안전조치사항_고소작업_);
                m_imgs.Add(global::DidUIEditor.Properties.Resources._7_안전조치사항_굴착_작업_);
                //m_imgs.Add(global::DidUIEditor.Properties.Resources._8_방재장비_배치도);
                m_imgs.Add(global::DidUIEditor.Properties.Resources._9_안전조치사항);
                m_imgs.Add(global::DidUIEditor.Properties.Resources._10_안전조치사항);
                m_imgs.Add(global::DidUIEditor.Properties.Resources._11_안전조치사항);
                m_imgs.Add(global::DidUIEditor.Properties.Resources._12_안전조치사항);
                m_imgs.Add(global::DidUIEditor.Properties.Resources._13_안전조치사항);
                m_imgs.Add(global::DidUIEditor.Properties.Resources._14_안전조치사항);
                m_imgs.Add(global::DidUIEditor.Properties.Resources._15_안전조치사항);
                m_imgs.Add(global::DidUIEditor.Properties.Resources._16_안전조치사항);
                m_imgs.Add(global::DidUIEditor.Properties.Resources._17_안전조치사항);
                m_imgs.Add(global::DidUIEditor.Properties.Resources._18_안전조치사항);
                m_imgs.Add(global::DidUIEditor.Properties.Resources._19_안전조치사항);
                m_imgs.Add(global::DidUIEditor.Properties.Resources._20_안전조치사항);
                m_imgs.Add(global::DidUIEditor.Properties.Resources._21_안전조치사항);
                m_imgs.Add(global::DidUIEditor.Properties.Resources._22_안전조치사항);
                m_imgs.Add(global::DidUIEditor.Properties.Resources._23_안전조치사항);
                m_imgs.Add(global::DidUIEditor.Properties.Resources._24_안전조치사항);
                m_imgs.Add(global::DidUIEditor.Properties.Resources._25_안전조치사항);
                m_imgs.Add(global::DidUIEditor.Properties.Resources._26_안전조치사항);
                m_imgs.Add(global::DidUIEditor.Properties.Resources._27_안전조치사항);
                m_imgs.Add(global::DidUIEditor.Properties.Resources._28_안전조치사항);
                m_imgs.Add(global::DidUIEditor.Properties.Resources._29_안전조치사항);
                m_imgs.Add(global::DidUIEditor.Properties.Resources._30_안전조치사항);
                m_imgs.Add(global::DidUIEditor.Properties.Resources._31_안전조치사항);
            }
            else
            {
                cbDisasterType.SelectedIndex = 0;

                m_gridImgs.Add(global::DidUIEditor.Properties.Resources._9_화재알람);
                m_gridImgs.Add(global::DidUIEditor.Properties.Resources._10_누출알람);
                m_gridImgs.Add(global::DidUIEditor.Properties.Resources._11_지진알람);
                m_gridImgs.Add(global::DidUIEditor.Properties.Resources._12_밀폐공간_알람);
            }

            Column1.ImageLayout = DataGridViewImageCellLayout.NotSet;
            dataGridView1.RowTemplate.Height = 200;
            //dataGridView1.Columns[0].Width = 300;
            foreach (Image img in m_gridImgs)
            {
                int nRowIndex = dataGridView1.Rows.Add();
                dataGridView1.Rows[nRowIndex].Cells[0].Value = img;
            }
        }
        
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ReturnPages = new List<Page>();
            List<DataGridViewRow> rows = new List<DataGridViewRow>();
            foreach (DataGridViewRow item in dataGridView1.SelectedRows)
            {
                rows.Add(item);
            }
            

            foreach (DataGridViewRow item in rows.OrderBy(p => p.Index))
            {
                Page page = new Page();
                //page.Name = txtPageName.Text;
                page.PageType = PageType.System;
                page.PageLocation = new Point(0, 0);
                page.PageSize = new Size(960, 540);

                page.BackgroundIMG = m_imgs[item.Index];// picSystemStyle1.Image;
                page.strBackgroundIMG = "systemstyle" + (item.Index) + ".png";

                if (FormMain.Instance.Mode == Mode.Emergency)
                {
                    if (cbDisasterType.SelectedItem.ToString() == "화재")
                        page.DisasterType = DisasterType.Fire;
                    else if (cbDisasterType.SelectedItem.ToString() == "누출")
                        page.DisasterType = DisasterType.PSM;
                }

                ReturnPages.Add(page);
            }
            
            this.DialogResult = DialogResult.Yes;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // 이미지 size 때문에 m_imgs의 이미지가 아닌 큰 이미지를 불러온다
            picSystemStyle1.Image = m_imgs[dataGridView1.CurrentRow.Index];
            /*
            switch (dataGridView1.CurrentRow.Index)
            {
                case 0: picSystemStyle1.Image = global::DidUIEditor.Properties.Resources._0_기본; break;
                case 1: picSystemStyle1.Image = global::DidUIEditor.Properties.Resources._1_근로자현황; break;
                case 2: picSystemStyle1.Image = global::DidUIEditor.Properties.Resources._2_안전조치사항_일반위험작업_; break;
                case 3: picSystemStyle1.Image = global::DidUIEditor.Properties.Resources._3_안전조치사항_화재작업_; break;
                case 4: picSystemStyle1.Image = global::DidUIEditor.Properties.Resources._4_안전조치사항_정전작업_; break;
                case 5: picSystemStyle1.Image = global::DidUIEditor.Properties.Resources._5_안전조치사항_밀폐공간작업_; break;
                case 6: picSystemStyle1.Image = global::DidUIEditor.Properties.Resources._6_안전조치사항_고소작업_; break;
                case 7: picSystemStyle1.Image = global::DidUIEditor.Properties.Resources._7_안전조치사항_굴착_작업_; break;
                case 8: picSystemStyle1.Image = global::DidUIEditor.Properties.Resources._8_방재장비_배치도; break;
                case 9: picSystemStyle1.Image = global::DidUIEditor.Properties.Resources._9_안전조치사항; break;
                case 10: picSystemStyle1.Image = global::DidUIEditor.Properties.Resources._10_안전조치사항; break;
                case 11: picSystemStyle1.Image = global::DidUIEditor.Properties.Resources._11_안전조치사항; break;
                case 12: picSystemStyle1.Image = global::DidUIEditor.Properties.Resources._12_안전조치사항; break;
                case 13: picSystemStyle1.Image = global::DidUIEditor.Properties.Resources._13_안전조치사항; break;
                case 14: picSystemStyle1.Image = global::DidUIEditor.Properties.Resources._14_안전조치사항; break;
                case 15: picSystemStyle1.Image = global::DidUIEditor.Properties.Resources._15_안전조치사항; break;
                case 16: picSystemStyle1.Image = global::DidUIEditor.Properties.Resources._16_안전조치사항; break;
                case 17: picSystemStyle1.Image = global::DidUIEditor.Properties.Resources._17_안전조치사항; break;
                case 18: picSystemStyle1.Image = global::DidUIEditor.Properties.Resources._18_안전조치사항; break;
                case 19: picSystemStyle1.Image = global::DidUIEditor.Properties.Resources._19_안전조치사항; break;
                case 20: picSystemStyle1.Image = global::DidUIEditor.Properties.Resources._20_안전조치사항; break;
                case 21: picSystemStyle1.Image = global::DidUIEditor.Properties.Resources._21_안전조치사항; break;
                case 22: picSystemStyle1.Image = global::DidUIEditor.Properties.Resources._22_안전조치사항; break;
                case 23: picSystemStyle1.Image = global::DidUIEditor.Properties.Resources._23_안전조치사항; break;
                case 24: picSystemStyle1.Image = global::DidUIEditor.Properties.Resources._24_안전조치사항; break;
                case 25: picSystemStyle1.Image = global::DidUIEditor.Properties.Resources._25_안전조치사항; break;
                case 26: picSystemStyle1.Image = global::DidUIEditor.Properties.Resources._26_안전조치사항; break;
                case 27: picSystemStyle1.Image = global::DidUIEditor.Properties.Resources._27_안전조치사항; break;
            }
            */
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }
    }
}
