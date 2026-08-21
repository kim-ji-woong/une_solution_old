using DBUtility2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.Earthquake;
using UnE.GUI;
using UnE.SOP;

namespace SDMS_Building.PopupDialog.Config
{
    public partial class FormEarthquake : Form
    {
        private class OptionGroup
        {
            private TextBox m_textBoxMin = null;
            private TextBox m_textBoxMax = null;
            private Label m_labelMin = null;
            private Label m_labelMax = null;

            private double m_minIntens = 0.0;
            private double m_maxIntens = 0.0;

            private EarthquakeOption.IntensOption m_intensOption = EarthquakeOption.IntensOption.NONE;

            private FormEarthquake m_frmParent = null;

            private WebDBManager m_dbMgr = null;
            UnE.SOP.SOPManager m_sopMgr = null;
            
            public TextBox MinIntens
            {
                get { return m_textBoxMin; }
                set { m_textBoxMin = value; }
            }

            public TextBox MaxIntens
            {
                get { return m_textBoxMax; }
                set { m_textBoxMax = value; }
            }

            public Label MinIntensLabel
            {
                get { return m_labelMin; }
                set { m_labelMin = value; }
            }

            public Label MaxIntensLabel
            {
                get { return m_labelMax; }
                set { m_labelMax = value; }
            }

            public EarthquakeOption.IntensOption IntensOption
            {
                get { return m_intensOption; }
                set { m_intensOption = value; }
            }

            public OptionGroup(FormEarthquake frmParent, TextBox textBoxMin, TextBox textBoxMax)
            {
                m_dbMgr = FormMain.Instance.DBManager;
                m_sopMgr = new UnE.SOP.SOPManager(m_dbMgr);
                m_sopMgr.Load(true, true);

                m_textBoxMin = textBoxMin;
                m_textBoxMax = textBoxMax;
                m_frmParent = frmParent;

                CbSopLoad();
                
                SetIntensOption(m_frmParent.GetTypeText(), textBoxMin, textBoxMax);

                //m_btnSOP.Click += new System.EventHandler(btnSOP_Click);
                //m_pbPrev.Click += new System.EventHandler(GoPrev);
                //m_pbNext.Click += new System.EventHandler(GoNext);
            }

            private bool m_isValidate = false;
            public bool SetIntensOption(string strType, TextBox textBoxMin, TextBox textBoxMax)
            {
                // 0 : 이하 또는 이상, 1 : 미만 또는 초과
                int minOption = 0, maxOption = 0;
                bool isIntensity = true;

                m_isValidate = false;
                m_intensOption = EarthquakeOption.IntensOption.NONE;

                if (strType == "진도")
                    isIntensity = true;
                else if (strType == "규모")
                    isIntensity = false;
                else
                    return false;

                if (textBoxMin.Visible == true)
                {
                    if (GetMinMaxOption(true, ref minOption) == false)
                        return false;

                    if (textBoxMax != null)
                    {
                        if (textBoxMax.Visible == true)
                        {
                            if (GetMinMaxOption(false, ref maxOption) == false)
                                return false;

                            if (minOption == 0 && maxOption == 1)
                            {
                                if (isIntensity)
                                    m_intensOption = EarthquakeOption.IntensOption.I_MIN_GE_MAX_LT;
                                else
                                    m_intensOption = EarthquakeOption.IntensOption.M_MIN_GE_MAX_LT;
                            }
                            else if (minOption == 1 && maxOption == 0)
                            {
                                if (isIntensity)
                                    m_intensOption = EarthquakeOption.IntensOption.I_MIN_GT_MAX_LE;
                                else
                                    m_intensOption = EarthquakeOption.IntensOption.M_MIN_GT_MAX_LE;
                            }
                            else
                                return false;
                        }
                        else if (textBoxMax.Visible == false)
                        {
                            if (minOption == 0)
                            {
                                if (isIntensity)
                                    m_intensOption = EarthquakeOption.IntensOption.I_MIN_LE;
                                else
                                    m_intensOption = EarthquakeOption.IntensOption.M_MIN_LE;
                            }
                            else if (minOption == 1)
                            {
                                if (isIntensity)
                                    m_intensOption = EarthquakeOption.IntensOption.I_MIN_LT;
                                else
                                    m_intensOption = EarthquakeOption.IntensOption.M_MIN_LT;
                            }
                        }
                        else
                            return false; 
                    }
                }
                else if (textBoxMin.Visible == false)
                {
                    if (textBoxMax.Visible == true)
                    {
                        if (GetMinMaxOption(true, ref maxOption) == false)
                            return false;

                        if (maxOption == 0)
                        {
                            if (isIntensity)
                                m_intensOption = EarthquakeOption.IntensOption.I_MAX_GE;
                            else
                                m_intensOption = EarthquakeOption.IntensOption.M_MAX_GE;
                        }
                        else if (maxOption == 1)
                        {
                            if (isIntensity)
                                m_intensOption = EarthquakeOption.IntensOption.I_MAX_GT;
                            else
                                m_intensOption = EarthquakeOption.IntensOption.M_MAX_GT;
                        }
                    }
                    else
                        return false;
                }
                else
                    return false;

                m_isValidate = true;
                return true;
            }

            // nResult : 0(이하 또는 이상), 1(미만 또는 초과)
            private bool GetMinMaxOption(bool isMinimum, ref int nResult)
            {
                if (isMinimum)
                {
                    //if (strText == "이상")
                    //    nResult = 0;
                    //else if (strText == "초과")
                    //    nResult = 1;
                    //else
                    //    return false;
                    nResult = 0;
                }
                else
                {
                    //if (strText == "이하")
                    //    nResult = 0;
                    //else if (strText == "미만")
                    //    nResult = 1;
                    //else
                    //    return false;
                    nResult = 1;
                }

                return true;
            }

            private void CbSopLoad()
            {
                Dictionary<string, DisasterInfo> dicSOP = m_sopMgr.GetSOPDictionary(true, true);
                Dictionary<int, VersionInfo> dicVersion = m_sopMgr.GetVersionDictionary(true, true);

                TreeData treeRoot = new TreeData();

                foreach (KeyValuePair<string, DisasterInfo> pair in dicSOP)
                {
                    string strFullPath = pair.Key;

                    char m_chDelimeter = (char)6;

                    int nIndex1 = strFullPath.IndexOf(m_chDelimeter);
                    int nIndex2 = strFullPath.LastIndexOf(m_chDelimeter);
                    if (nIndex1 < 0 || nIndex2 < 0)
                        continue;

                    string strCategoryName = strFullPath.Substring(0, nIndex1);
                    string strSubCategoryName = strFullPath.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
                    string strDisasterName = strFullPath.Substring(nIndex2 + 1);
                    string strPath = null;

                    strPath = strCategoryName;

                    if (FindNode(strCategoryName, treeRoot) == null)
                    {
                        treeRoot.child.Add(new TreeData(strCategoryName, strPath));
                    }


                    TreeData subTree = null;
                    strPath = strCategoryName + "/" + strSubCategoryName;

                    foreach (TreeData node in treeRoot.child)
                    {
                        if (strCategoryName == node.strNodeName)
                            subTree = node;
                    }

                    if (FindNode(strSubCategoryName, subTree) == null)
                    {
                        subTree.child.Add(new TreeData(strSubCategoryName, strPath));
                    }


                    TreeData DisasterTree = null;
                    strPath = strCategoryName + "/" + strSubCategoryName + "/" + strDisasterName;

                    foreach (TreeData node in subTree.child)
                    {
                        if (strSubCategoryName == node.strNodeName)
                            DisasterTree = node;
                    }

                    if (FindNode(strDisasterName, DisasterTree) == null)
                    {
                        DisasterTree.child.Add(new TreeData(strDisasterName, strPath));
                    }

                    DisasterInfo disaster = pair.Value;
                    ArrayList arrActionSteps = disaster.ActionSteps;

                    TreeData ActionStepTree = null;
                    strPath = strCategoryName + "/" + strSubCategoryName + "/" + strDisasterName;

                    foreach (TreeData node in DisasterTree.child)
                    {
                        if (strDisasterName == node.strNodeName)
                            ActionStepTree = node;
                    }

                    if (arrActionSteps.Count > 0)
                    {
                        foreach (ActionStepInfo actionStep in arrActionSteps)
                        {
                            ActionStepTree.child.Add(new TreeData(actionStep.ActionStepName, strPath));
                        }
                    }
                }
            }

            public TreeData FindNode(string strValue, TreeData parentNodes = null)
            {
                List<TreeData> nodes = parentNodes.child;

                if (nodes == null)
                    return null;

                foreach (TreeData node in nodes)
                {
                    if (strValue == node.strNodeName)
                        return node;
                }

                return null;
            }

            public void Init()
            {
                m_textBoxMin.Text = "";
                if (m_textBoxMax != null)
                    m_textBoxMax.Text = "";
            }

            public void SetData(double min, double max)
            {
                m_minIntens = min;
                m_maxIntens = max;
            }

            public void SetData(double min)
            {
                m_minIntens = min;
            }

            public string UpdateQuery(int nID)
            {
                string strFormat = "Update OptionEarthquake set MinIntens = {0:F1}, MaxIntens = {1:F1}, IntensOption = {2}, UseSMS = {3},";
                strFormat += "SMSMessage = '{4}', UseBroadcast = {5}, BroadcastMessage = '{6}',";
                strFormat += "SiteID = {7} where ID = {8}";

                string strSQL = string.Format(strFormat, m_minIntens, m_maxIntens, (int)m_intensOption,
                    0, " ",
                    0, " ",
                    UnE.SOP.ProxySOP.Instance.SiteID, nID);

                return strSQL;
            }
                       
            public string InsertQuery(int nID)
            {
                string strFormat = "Insert into OptionEarthquake (ID, MinIntens,  MaxIntens, IntensOption, UseSMS, SMSMessage, ";
                strFormat += "UseBroadcast, BroadcastMessage, SiteID) values ({8}, {0:F1}, {1:F1}, {2}, ";
                strFormat += "{3}, '{4}', {5}, '{6}', {7})";

                string strSQL = string.Format(strFormat, m_minIntens, m_maxIntens, (int)m_intensOption,
                    0, " ",
                    0, " ",                    
                    UnE.SOP.ProxySOP.Instance.SiteID, nID);

                return strSQL;
            }
        }
        
        private Pen m_pen = new Pen(Color.FromArgb(0xe0, 0xe0, 0xe0));
        private Pen m_pen2 = new Pen(Color.FromArgb(0x25, 0x31, 0x50));
        private Brush m_brushGreen = new SolidBrush(Color.FromArgb(0x61, 0xdb, 0x5f));
        private Brush m_brushYellow = new SolidBrush(Color.FromArgb(0xef, 0xb5, 0x00));
        private Brush m_brushOrange = new SolidBrush(Color.FromArgb(0xf5, 0x82, 0x1f));
        private Brush m_brushRed = new SolidBrush(Color.FromArgb(0xef, 0x57, 0x57));
        private Brush m_brushForeColor = new SolidBrush(Color.FromArgb(0x33, 0x33, 0x3));
        private Font m_fontTitle = new System.Drawing.Font("나눔바른고딕", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
        private Font m_font2 = new System.Drawing.Font("나눔바른고딕", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));

        private WebDBManager m_dbMgr = null;
        UnE.SOP.SOPManager m_sopMgr = null;
        private List<OptionGroup> m_optionGroups = new List<OptionGroup>();
        private int m_nStepIndex = 0;

        public FormEarthquake()
        {
            InitializeComponent();

            this.DoubleBuffered = true;

            m_dbMgr = FormMain.Instance.DBManager;
            m_sopMgr = new UnE.SOP.SOPManager(m_dbMgr);
            m_sopMgr.Load(true, true);
        }
        
        private void FormEarthquake_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            #region textbox 테두리
            // 관심 textbox 테두리
            Rectangle rect1 = new Rectangle(243, 100, 70, 50);
            Rectangle rect2 = new Rectangle(243, 160, 70, 50);

            // 주의 textbox 테두리
            Rectangle rect3 = new Rectangle(427, 100, 70, 50);
            Rectangle rect4 = new Rectangle(427, 160, 70, 50);

            // 경계 textbox 테두리
            Rectangle rect5 = new Rectangle(611, 100, 70, 50);
            Rectangle rect6 = new Rectangle(611, 160, 70, 50);

            // 심각 textbox 테두리
            Rectangle rect7 = new Rectangle(795, 100, 70, 50);

            g.DrawRectangle(m_pen, rect1);
            g.DrawRectangle(m_pen, rect2);
            g.DrawRectangle(m_pen, rect3);
            g.DrawRectangle(m_pen, rect4);
            g.DrawRectangle(m_pen, rect5);
            g.DrawRectangle(m_pen, rect6);
            g.DrawRectangle(m_pen, rect7);
            #endregion

            // 가로 선
            g.DrawLine(m_pen2, 203, 30, 940, 30);
            g.DrawLine(m_pen2, 20, 80, 940, 80);
            g.DrawLine(m_pen2, 20, 229, 940, 229);
            //g.DrawLine(m_pen2, 20, 349, 940, 349);

            // 세로 선
            g.DrawLine(m_pen2, 20, 80, 20, 228);
            g.DrawLine(m_pen2, 203, 30, 203, 228);
            g.DrawLine(m_pen2, 387, 30, 387, 228);
            g.DrawLine(m_pen2, 571, 30, 571, 228);
            g.DrawLine(m_pen2, 755, 30, 755, 228);
            g.DrawLine(m_pen2, 940, 30, 940, 228);
            
            g.FillRectangle(m_brushGreen, 204, 31, 183, 49);
            g.FillRectangle(m_brushYellow, 388, 31, 183, 49);
            g.FillRectangle(m_brushOrange, 572, 31, 183, 49);
            g.FillRectangle(m_brushRed, 756, 31, 184, 49);

            g.DrawString("관심", m_fontTitle, m_brushForeColor, 278, 45);
            g.DrawString("주의", m_fontTitle, m_brushForeColor, 462, 45);
            g.DrawString("경계", m_fontTitle, m_brushForeColor, 646, 45);
            g.DrawString("심각", m_fontTitle, m_brushForeColor, 829, 45);
            g.DrawString("진도", m_fontTitle, m_brushForeColor, 94, 145);

            g.DrawString("이상", m_font2, m_brushForeColor, 323, 118);
            g.DrawString("미만", m_font2, m_brushForeColor, 323, 178);
            g.DrawString("이상", m_font2, m_brushForeColor, 507, 118);
            g.DrawString("미만", m_font2, m_brushForeColor, 507, 178);
            g.DrawString("이상", m_font2, m_brushForeColor, 691, 118);
            g.DrawString("미만", m_font2, m_brushForeColor, 691, 178);
            g.DrawString("이상", m_font2, m_brushForeColor, 875, 118);            
        }

        private void FormEarthquake_Load(object sender, EventArgs e)
        {
            m_optionGroups.Add(new OptionGroup(this, tbStep1Min, tbStep1Max));
            m_optionGroups.Add(new OptionGroup(this, tbStep2Min, tbStep2Max));
            m_optionGroups.Add(new OptionGroup(this, tbStep3Min, tbStep3Max));
            m_optionGroups.Add(new OptionGroup(this, tbStep4Min, tbStep4Min));

            LoadOptions();
        }
        
        private void LoadOptions()
        {
            List<EarthquakeOption> options = EarthquakeOption.LoadOptions(m_dbMgr);

            if (options == null)
                return;
            
            if (options.Count == 0)
            {
                initEarthquakeDBData();
                options = EarthquakeOption.LoadOptions(m_dbMgr);
            }

            options.Sort();
            SetDatas(options);
        }

 
        private void SetDatas(List<EarthquakeOption> options)
        {
            foreach (OptionGroup optionGroup in m_optionGroups)
            {
                optionGroup.Init();
            }

            if (options.Count == 0)
            {
                for (int i = 1; i < m_optionGroups.Count; i++)
                {
                    OptionGroup optionGroup = m_optionGroups[i];
                }

                return;
            }
            else
            {
                for (int i = 0; i < m_optionGroups.Count; i++)
                {
                    OptionGroup optionGroup = m_optionGroups[i];
                }
            }
            
            for (int i = m_nStepIndex; i < options.Count && i < m_nStepIndex + 4; i++)
            {
                EarthquakeOption opt = options[i];
                OptionGroup ui = m_optionGroups[i - m_nStepIndex];
                ui.IntensOption = opt.MinMaxOption;

                if (opt.BothMinMax)
                {
                    ui.MinIntens.Text = GetDataString(opt.Minimum, opt.IsIntensity);
                    if (ui.MaxIntens != null)
                        ui.MaxIntens.Text = GetDataString(opt.Maximum, opt.IsIntensity);
                }
                else if (opt.OnlyMin)
                {
                    ui.MinIntens.Text = GetDataString(opt.Minimum, opt.IsIntensity);
                }
                else if (opt.OnlyMax)
                {
                    if (ui.MaxIntens != null)
                        ui.MaxIntens.Text = GetDataString(opt.Maximum, opt.IsIntensity);
                }                
            }
        }


        private string GetDataString(double data, bool isIntensity)
        {
            if (isIntensity)
                return string.Format("{0}", (int)(data + 0.0001));

            return string.Format("{0:F1}", data);
        }

        
        public void initEarthquakeDBData()
        {
            string strFormat = "Insert into OptionEarthquake (ID, MinIntens,  MaxIntens, IntensOption, UseSMS, SMSMessage, ";
            strFormat += "UseBroadcast, BroadcastMessage, RunSOP, LinkedSOP, SiteID) values ({10}, {0:F1}, {1:F1}, {2}, ";
            strFormat += "{3}, '{4}', {5}, '{6}', {7}, '{8}', {9})";

            string strSQL = string.Format(strFormat, 1, 2, (int)0,
                0, " ",
                0, " ",
                0, " ",
                UnE.SOP.ProxySOP.Instance.SiteID, 1);
            m_dbMgr.GetResultData(strSQL);

            strSQL = string.Format(strFormat, 2.1, 3, (int)0,
                0, " ",
                0, " ",
                0, " ",
                UnE.SOP.ProxySOP.Instance.SiteID, 2);
            m_dbMgr.GetResultData(strSQL);

            strSQL = string.Format(strFormat, 3.1, 4, (int)0,
                0, " ",
                0, " ",
                0, " ",
                UnE.SOP.ProxySOP.Instance.SiteID, 3);
            m_dbMgr.GetResultData(strSQL);

            strSQL = string.Format(strFormat, 4.1, 4.1, (int)5,
                0, " ",
                0, " ",
                0, " ",
                UnE.SOP.ProxySOP.Instance.SiteID, 4);
            m_dbMgr.GetResultData(strSQL);
        }

        public void Save()
        {
            if (CheckValid() == false)
                return;

            string strSQL = "Select ID from OptionEarthquake where SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nIndex = 0;
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount; i++)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());

                if (id == null)
                    continue;

                if (m_optionGroups.Count > nIndex)
                {
                    OptionGroup option = m_optionGroups[nIndex];

                    strSQL = option.UpdateQuery(id.Data);
                    if (strSQL.Length > 0)
                    {
                        if (m_dbMgr.GetResultData(strSQL) != null)
                            nIndex++;
                    }
                }
            }
        }

        private bool CheckValid()
        {
            if (m_optionGroups.Count > 0)
            {
                OptionGroup lastOption = m_optionGroups[m_optionGroups.Count - 1];

                foreach (OptionGroup optionGroup in m_optionGroups)
                {
                    double min, max;
                    bool maxResult;
                    bool minResult = GetDouble(optionGroup.MinIntens, out min);
                    if (optionGroup.MaxIntens != null)
                    {
                        maxResult = GetDouble(optionGroup.MaxIntens, out max);

                        if (maxResult == false || max <= 0.0)
                        {
                            optionGroup.MaxIntens.Focus();
                            MessageBox.Show("진도값은 0보다 큰 숫자이어야만 합니다.");
                            return false;
                        }
                    }
                    else
                    {
                        max = 0;
                    }

                    if (optionGroup != lastOption && (minResult == false || min <= 0.0))
                    {
                        optionGroup.MinIntens.Focus();
                        MessageBox.Show("진도값은 0보다 큰 숫자이어야만 합니다.");
                        return false;
                    }
                    
                    if (optionGroup.MaxIntens != null)
                    {
                        optionGroup.SetData(min, max);
                    }
                    else
                    {
                        optionGroup.SetData(min);
                    }
                    
                }
            }

            return true;
        }

        private bool GetDouble(TextBox text, out double data)
        {
            string str = text.Text.Trim();

            if (double.TryParse(str, out data) == false)
                return false;

            return true;
        }

        private bool CheckValidSOP(UEWpfControl.WpfComboBox text, out string strSOP)
        {
            strSOP = "NULL";
            string strPath = ""; //.ToString().Trim();
            TreeData data = text.customComboBox.SelectedItem as TreeData;            
            if (data == null)
                return true;

            strPath = data.strFullPath;

            if (strPath.Length == 0)
                return true;

            // SOP가 ''로 감싸여 있을경우 이를 제거한다.
            if (strPath.StartsWith("'"))
                strPath = strPath.Substring(1);

            if (strPath.EndsWith("'"))
                strPath = strPath.Substring(0, strPath.Length - 1);

            string[] tokens = strPath.Split('/');

            if (tokens.Count() < 3)
            {
                text.Focus();
                MessageBox.Show("'SOP는 카테고리/하부카테고리/SOP이름'의 형식으로 표기되어야 합니다.");
                return false;
            }

            string strSQL = "Select ID from DisasterCategory where SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString() + " and CategoryName = '" + tokens[0].Trim() + "'";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                MessageBox.Show("DB에 접속할 수 없습니다.");
                return false;
            }

            if (arrResult.Count == 0)
            {
                text.Focus();
                MessageBox.Show("'" + tokens[0].Trim() + "'는 존재하지 않는 카테고리입니다.");
                return false;
            }

            int nDisasterCategoryID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);

            strSQL = "Select ID from SubDisasterCategory where DisasterID = " + nDisasterCategoryID.ToString() + " and SubCategoryName = '" + tokens[1].Trim() + "'";
            arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                MessageBox.Show("DB에 접속할 수 없습니다.");
                return false;
            }

            if (arrResult.Count == 0)
            {
                text.Focus();
                MessageBox.Show("'" + tokens[0] + "/" + tokens[0] + "'는 유효하지 않은 경로입니다.");
                return false;
            }

            int nSubCategoryID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);

            strSQL = "Select ID from Disaster where SubDisasterID = " + nSubCategoryID.ToString() + " and DisasterName = '" + tokens[2].Trim() + "'";
            arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                MessageBox.Show("DB에 접속할 수 없습니다.");
                return false;
            }

            if (arrResult.Count == 0)
            {
                text.Focus();
                MessageBox.Show("'" + strPath + "'는 유효하지 않은 경로입니다.");
                return false;
            }

            strSOP = "'" + tokens[0].Trim() + "/" + tokens[1].Trim() + "/" + tokens[2].Trim() + "'";
            return true;
        }

        public string GetTypeText()
        {
            return "진도";
        }
    }

  
    public class TreeData
    {
        public string strNodeName { get; set; }
        public string strFullPath { get; set; }
        public List<TreeData> child = new List<TreeData>();

        public TreeData()
        {
        }

        public TreeData(string nodeName, string path)
        {
            strNodeName = nodeName;
            strFullPath = path;
        }
    }
 
}

