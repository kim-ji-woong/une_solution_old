using DBUtility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TeamEditor
{
    public partial class PageBackstageOption : Form
    {
        private WebDBManager m_dbMgr = null;//new WebDBManager("SOP4");

        private static PageBackstageOption m_instance = null;

        public static PageBackstageOption Instance { get { return m_instance; } }

        public Color ColorTreeBack
        {
            get { return btnTreeBack.BackColor; }
        }
        public Color ColorTreeFont
        {
            get { return btnTreeFont.BackColor; }
        }
        public Color ColorGridBack
        {
            get { return btnGridBack.BackColor; }
        }
        public Color ColorGridFont
        {
            get { return btnGridFont.BackColor; }
        }

        private Color m_colorDefaultTreeBack;
        private Color m_colorDefaultTreeFont;
        private Color m_colorDefaultGridBack;
        private Color m_colorDefaultGridFont;


        public PageBackstageOption()
        {
            InitializeComponent();

            m_dbMgr = new TeamEditor.WebDBManagerEx(FormMain.Instance.SiteID);
            m_instance = this;


            btnGridBack.BackColor = GetControlBackColor("GridBackColor");
            btnGridFont.BackColor = GetControlBackColor("GridFontColor");
            btnTreeBack.BackColor = GetControlBackColor("TreeBackColor");
            btnTreeFont.BackColor = GetControlBackColor("TreeFontColor");

            btnGridBack.Click += btnColor_Click;
            btnGridFont.Click += btnColor_Click;
            btnTreeBack.Click += btnColor_Click;
            btnTreeFont.Click += btnColor_Click;
        }


        private void Save()
        {
            SaveControlBackColor();
        }

        public void SetDefaultColor(Color colorTreeBack, Color colorTreeFont, Color colorGridBack, Color colorGridFont)
        {
            m_colorDefaultTreeBack = colorTreeBack;
            m_colorDefaultTreeFont = colorTreeFont;
            m_colorDefaultGridBack = colorGridBack;
            m_colorDefaultGridFont = colorGridFont;

            if (String.IsNullOrWhiteSpace(m_dbMgr.LoadIni("GridBackColor", "Team Editor Info")))
                btnGridBack.BackColor = m_colorDefaultGridBack;
            if (String.IsNullOrWhiteSpace(m_dbMgr.LoadIni("GridFontColor", "Team Editor Info")))
                btnGridFont.BackColor = m_colorDefaultGridFont;
            if (String.IsNullOrWhiteSpace(m_dbMgr.LoadIni("TreeBackColor", "Team Editor Info")))
                btnTreeBack.BackColor = m_colorDefaultTreeBack;
            if (String.IsNullOrWhiteSpace(m_dbMgr.LoadIni("TreeFontColor", "Team Editor Info")))
                btnTreeFont.BackColor = m_colorDefaultTreeFont;

        }

        public bool HasColorInfo()
        {
            if (String.IsNullOrWhiteSpace(m_dbMgr.LoadIni("GridBackColor", "Team Editor Info")))
                return false;
            if (String.IsNullOrWhiteSpace(m_dbMgr.LoadIni("GridFontColor", "Team Editor Info")))
                return false;
            if (String.IsNullOrWhiteSpace(m_dbMgr.LoadIni("TreeBackColor", "Team Editor Info")))
                return false;
            if (String.IsNullOrWhiteSpace(m_dbMgr.LoadIni("TreeFontColor", "Team Editor Info")))
                return false;

            return true;
        }


        private Color GetControlBackColor(string strTargetName)
        {
            Color colorReturn;

            string strColorArgb = m_dbMgr.LoadIni(strTargetName, "Team Editor Info");

            if (String.IsNullOrWhiteSpace(strColorArgb))
            {
                switch (strTargetName)
                {
                    case "GridBackColor":
                        colorReturn = m_colorDefaultGridBack;
                        break;
                    case "GridFontColor":
                        colorReturn = m_colorDefaultGridFont;
                        break;
                    case "TreeBackColor":
                        colorReturn = m_colorDefaultTreeBack;
                        break;
                    case "TreeFontColor":
                        colorReturn = m_colorDefaultTreeFont;
                        break;
                    default :
                        colorReturn = Color.White;
                        break;
                }
            }
            else
            {
                colorReturn = Color.FromArgb(Convert.ToInt32(strColorArgb));
            }

            return colorReturn;
        }

        private void SaveControlBackColor()
        {
            m_dbMgr.SaveIni("TreeBackColor", btnTreeBack.BackColor.ToArgb().ToString(), "Team Editor Info");
            m_dbMgr.SaveIni("TreeFontColor", btnTreeFont.BackColor.ToArgb().ToString(), "Team Editor Info");
            m_dbMgr.SaveIni("GridBackColor", btnGridBack.BackColor.ToArgb().ToString(), "Team Editor Info");
            m_dbMgr.SaveIni("GridFontColor", btnGridFont.BackColor.ToArgb().ToString(), "Team Editor Info");
        }


        private void btnColor_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            colorDialog.AllowFullOpen = true;
            colorDialog.FullOpen = true;

            int[] nUserColors = new int[]
            {
                ColorTranslator.ToOle(btnGridBack.BackColor),
                ColorTranslator.ToOle(btnGridFont.BackColor),
                ColorTranslator.ToOle(btnTreeBack.BackColor),
                ColorTranslator.ToOle(btnTreeFont.BackColor)
            };

            colorDialog.CustomColors = nUserColors;

            colorDialog.Color = btn.BackColor;

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                btn.BackColor = colorDialog.Color;
            }

        }

        private void btnTreeInit_Click(object sender, EventArgs e)
        {
            btnTreeBack.BackColor = m_colorDefaultTreeBack;
            btnTreeFont.BackColor = m_colorDefaultTreeFont;
        }

        private void btnGridInit_Click(object sender, EventArgs e)
        {
            btnGridBack.BackColor = m_colorDefaultGridBack;
            btnGridFont.BackColor = m_colorDefaultGridFont;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.Save();
            FormMain.Instance.InitControlColor();
        }

    }
}
