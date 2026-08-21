using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility2;
using System.Collections;

namespace SOPManager.FormPanel
{
    public partial class FormSOPGenLevels : Form
    {
        private class LevelControlData
        {
            private bool m_isChecked = true;
            private int m_nID = 0;

            public bool IsChecked
            {
                get { return m_isChecked; }
                set { m_isChecked = value; }
            }

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }

            public LevelControlData()
            {
            }

            public LevelControlData(bool isChecked, int nID)
            {
                m_isChecked = isChecked;
                m_nID = nID;
            }
        }

        private Dictionary<int, bool> m_dicLevelUse = new Dictionary<int, bool>();
        private List<int> m_usingLevelIDs = null;
        
        public FormSOPGenLevels(ref List<int> usingLevelIDs)
        {
            InitializeComponent();

            SetItems(usingLevelIDs);

            if (usingLevelIDs == null)
                usingLevelIDs = new List<int>();

            m_usingLevelIDs = usingLevelIDs;
        }

        private void SetItems(List<int> usingLevelIDs)
        {
            string strSQL = "Select ID, LevelName from SOPGenLevel";
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            PictureBox pbPrev = null;
            Label labelPrev = null;
            int nVerticalSpace = 30;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strLevelName = WebDBManager.GetStringField(arrResult[i + 1]);

                if (id == null || strLevelName == null)
                    continue;

                bool isChecked = usingLevelIDs == null || usingLevelIDs.Contains(id.Data) ? true : false;

                if (pbPrev == null)
                {
                    pbFirstLevel.Visible = true;
                    pbFirstLevel.Tag = new LevelControlData(isChecked, id.Data);

                    labelFirstLevelName.Text = strLevelName;
                    labelFirstLevelName.Visible = true;
                    labelFirstLevelName.Tag = pbFirstLevel;

                    pbPrev = pbFirstLevel;
                    labelPrev = labelFirstLevelName;
                }
                else
                {
                    PictureBox pb = new PictureBox();
                    panelLevels.Controls.Add(pb);

                    pb.BackgroundImage = pbFirstLevel.BackgroundImage;
                    pb.BackgroundImageLayout = pbFirstLevel.BackgroundImageLayout;
                    pb.Location = new System.Drawing.Point(pbPrev.Location.X, pbPrev.Location.Y + nVerticalSpace);
                    pb.Name = "pbLevel" + i.ToString();
                    pb.Size = pbFirstLevel.Size;
                    pb.TabIndex = pbPrev.TabIndex + 1;
                    pb.TabStop = pbPrev.TabStop;
                    pb.Visible = true;
                    pb.Click += new System.EventHandler(this.pbLevel_Click);
                    pb.Tag = new LevelControlData(isChecked, id.Data);

                    Label label = new Label();
                    panelLevels.Controls.Add(label);

                    label.AutoSize = labelPrev.AutoSize;
                    label.Font = labelPrev.Font;
                    label.ForeColor = labelPrev.ForeColor;
                    label.Location = new System.Drawing.Point(labelPrev.Location.X, pb.Location.Y + labelPrev.Location.Y - pbPrev.Location.Y);
                    label.Name = strLevelName;
                    label.TabIndex = pb.TabIndex + 1;
                    label.Text = strLevelName;
                    label.Visible = true;
                    label.Click += new System.EventHandler(this.labelLevelName_Click);
                    label.Tag = pb;

                    pbPrev = pb;
                    labelPrev = label;
                }

                if (isChecked)
                    pbPrev.BackgroundImage = global::SOPManager.Properties.Resources.@__COMMON_ckb_enable;
                else
                    pbPrev.BackgroundImage = global::SOPManager.Properties.Resources.@__COMMON_ckb_disable;

                m_dicLevelUse[id.Data] = isChecked;
            }
        }

        private void pbLevel_Click(object sender, EventArgs e)
        {
            PictureBox pb = (PictureBox)sender;
            LevelControlData data = (LevelControlData)pb.Tag;

            if (data.IsChecked)
                pb.BackgroundImage = global::SOPManager.Properties.Resources.@__COMMON_ckb_disable;
            else
                pb.BackgroundImage = global::SOPManager.Properties.Resources.@__COMMON_ckb_enable;

            data.IsChecked = !data.IsChecked;

            m_dicLevelUse[data.ID] = data.IsChecked;
        }

        private void labelLevelName_Click(object sender, EventArgs e)
        {
            Label label = (Label)sender;
            PictureBox pb = (PictureBox)label.Tag;
            pbLevel_Click(pb, null);
        }

        private void FormSOPGenLevels_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_usingLevelIDs.Clear();

            foreach (KeyValuePair<int, bool> pair in m_dicLevelUse)
            {
                if (pair.Value)
                    m_usingLevelIDs.Add(pair.Key);
            }
        }
    }
}
