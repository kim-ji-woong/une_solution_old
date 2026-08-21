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

using DBUtility;

namespace TrainingEvaluation
{
    public partial class FormMain : Form
    {
        private WebDBManager m_dbMan = null;
        private int m_nSiteID = 100;

        private int m_nCurrentStep = 0; // 0:초동단계, 1:대응단계, 2:매뉴얼, 3:평가
        private List<EvaStep> m_listEvaStep = new List<EvaStep>();
        private string[] m_arrEva = new string[] { "미평가", "A", "B", "C" };

        Dictionary<int, EvaluationNone> m_dicNone = new Dictionary<int, EvaluationNone>();
        Dictionary<int, EvaluationDone> m_dicDone = new Dictionary<int, EvaluationDone>();
        EvaluationNone m_selectNone = null;

        public FormMain()
        {
            InitializeComponent();

            ReadSiteID();
            m_dbMan = new WebDBManager(m_nSiteID);

            dtDueS.CustomFormat = dtDueE.CustomFormat = "yyyy-MM-dd";
            dtDueS.Format = dtDueE.Format = DateTimePickerFormat.Custom;

            SetResize();
            InitGridView();
            SetDue();               // 기간설정
            InitDisasterType();     // 유형 초기화
            SetEvaluation();        // 평가 및 미평가 항목 도출
            SetLabel();
            InitStepData();

            cbDue.DropDownStyle = ComboBoxStyle.DropDownList;
            cbDisasterType.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void ReadSiteID()
        {
            DBUtility.Utility util = new DBUtility.Utility();
            string szSiteID = util.getinivalue("Server Connection Info", "siteid");
            if (szSiteID == null || szSiteID == "")
            {
                MessageBox.Show("Site ID가 지정되지 않았습니다. ini파일을 확인하세요", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }
             
            int nSiteId = 1;
            if (int.TryParse(szSiteID, out nSiteId))
            {
                m_nSiteID = nSiteId;
            }
            else
            {
                MessageBox.Show("잘못된 Site ID입니다.. ini파일을 확인하세요", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }  
        }

        private void SetResize()
        {
            int width = panelMain.Size.Width;
            int height = this.Size.Height - 140;

            panelStep.Location = new Point(panelMain.Location.X, panelMain.Location.Y);
            panelStep.Size = new System.Drawing.Size(width, height);

            panelMain.Size = new System.Drawing.Size(width, height);
            gridNone.Location = new Point(0, 37);
            gridNone.Size = new System.Drawing.Size(width, height-37);
            gridDone.Location = new Point(0, 37);
            gridDone.Size = new System.Drawing.Size(width, height-37);
        }

        // gridView 초기화
        private void InitGridView()
        {
            // 미평가 훈련 목록
            gridNone.AllowUserToAddRows = false;
            gridNone.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            for (int i = 0; i < gridNone.ColumnCount; ++i)
            {
                gridNone.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                gridNone.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                if (i == 3)
                    gridNone.Columns[i].FillWeight = 400;
                else
                    gridNone.Columns[i].FillWeight = 100;
            }
            gridNone.CellClick += gridNone_CellClick;
            
            // 평가 훈련 목록
            gridDone.AllowUserToAddRows = false;
            gridDone.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            for (int i = 0; i < gridDone.ColumnCount; ++i)
            {
                gridDone.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                gridDone.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                if(i == 3)
                    gridDone.Columns[i].FillWeight = 400;
                else
                    gridDone.Columns[i].FillWeight = 100;
            }
            gridDone.CellClick += gridDone_CellClick;

            // 평가 단계
            DataGridViewComboBoxColumn combo = new DataGridViewComboBoxColumn();
            combo.Name = "gridStep_column4";
            combo.HeaderText = "평가";
            combo.Sorted = false;
            combo.ReadOnly = false;
            combo.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;
            combo.Items.AddRange(m_arrEva);
            gridStep.ReadOnly = false;
            gridStep.Columns.Add("gridStep_column1", "    항목");
            gridStep.Columns.Add("gridStep_column2", "    코드");
            gridStep.Columns.Add("gridStep_column3", "세부 항목");
            gridStep.Columns.Add(combo);
            gridStep.Columns[3].ReadOnly = false;
            
            gridStep.Columns[2].Width = 730; 
            gridStep.AllowUserToAddRows = false;

            gridStep.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            for (int i = 0; i < gridStep.ColumnCount; ++i)
            {
                if (i != 2)
                    gridStep.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                gridStep.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                if (i == 2)
                    gridStep.Columns[i].FillWeight = 700;
                else
                    gridStep.Columns[i].FillWeight = 100;
            }
            gridStep.CellValueChanged += gridStep_CellValueChanged;
            gridStep.CurrentCellDirtyStateChanged += new EventHandler(gridStep_CurrentCellDirtyStateChanged);
        }

        // 기간 설정
        private void SetDue()
        {
            dtDueE.Value = DateTime.Now;

            // 0 : 최근 1주일
            // 1 : 최근 1개월
            // 2 : 최근 3개월
            // 3 : 최근 6개월
            // 4 : 최근 1년

            if (cbDue.SelectedIndex == 0)
                dtDueS.Value = DateTime.Now.AddDays(-7);
            else if (cbDue.SelectedIndex == 1)
                dtDueS.Value = DateTime.Now.AddMonths(-1);
            else if (cbDue.SelectedIndex == 2)
                dtDueS.Value = DateTime.Now.AddMonths(-3);
            else if (cbDue.SelectedIndex == 3)
                dtDueS.Value = DateTime.Now.AddMonths(-6);
            else
                dtDueS.Value = DateTime.Now.AddYears(-1);
        }

        // 유형 초기화
        private void InitDisasterType()
        {
            ArrayList arrCategory = new ArrayList();
            string strSql = "SELECT ID, CategoryName FROM DisasterCategory WHERE SiteID = " + m_nSiteID.ToString() + " ORDER BY CASE WHEN CategoryName = '기타' THEN -1 ELSE 1 END DESC, CategoryName";
            ArrayList arrResult = m_dbMan.GetResultData(strSql, 0);
            if (arrResult != null)
            {
                for (int i = 0; i < arrResult.Count - 1; i += 2)
                {
                    Data_DisasterCategory dataNew = new Data_DisasterCategory();
                    dataNew.ID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                    dataNew.CategoryName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");

                    arrCategory.Add(dataNew);
                }
            }

            object[] arr = arrCategory.ToArray();
                        
            cbDisasterType.Items.AddRange(arrCategory.ToArray());
            cbDisasterType.SelectedIndex = 0;
        }

        // 평가 및 미평가 항목 도출
        private void SetEvaluation()
        {
            m_dicNone.Clear();
            m_dicDone.Clear();

            Data_DisasterCategory dataDisasterType = cbDisasterType.SelectedItem as Data_DisasterCategory;
            if (dataDisasterType == null) return;
            int disasterType = dataDisasterType.ID;
            //int disasterType = cbDisasterType.SelectedIndex + 1;

            string strS = string.Format("'{0}-{1}-{2} 00:00:00'", dtDueS.Value.Year, dtDueS.Value.Month, dtDueS.Value.Day);
            //string strS = string.Format("'{0}-{1}-{2} 00:00:00'", dtDueS.Value.Year-1, 4, 13);
            string strE = string.Format("'{0}-{1}-{2} 23:59:59'", dtDueE.Value.Year, dtDueE.Value.Month, dtDueE.Value.Day);

            string query = "SELECT ash.ID, ash.BeginTime, ash.Position, dc.CategoryName, v.VersionName, v.isNormal, ash.DisasterOption "
                + "FROM ActionStepHistory AS ash "
                + "INNER JOIN ActionStep AS acs ON ash.ActionStepID=acs.ID "
                + "INNER JOIN disaster AS d ON d.ID=acs.DisasterID "
                + "INNER JOIN subdisastercategory as sdc ON sdc.ID=d.SubDisasterID "
                + "INNER JOIN version AS v ON v.ID=d.VersionID "
                + "INNER JOIN disastercategory AS dc ON sdc.DisasterID=dc.ID "
                + string.Format("WHERE ash.EndTime IS NOT NULL AND ash.BeginTime >= {0} AND ash.BeginTime <= {1} AND dc.ID={2} AND ash.RealMode=0", 
                strS, strE, disasterType);
                //+ string.Format("WHERE ash.BeginTime >= {0} AND ash.BeginTime <= {1} AND dc.ID={2}", strS, strE, disasterType);
            ArrayList arrResult = m_dbMan.GetResultData(query, 0);
            if (arrResult != null && arrResult.Count > 0)
            {
                DateTime dtDefault = new DateTime();

                for (int i = 0; i < arrResult.Count - 1; i += 7)
                {
                    int id = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                    string dt = WebDBManager.GetDateTimeField(arrResult[i + 1], dtDefault).ToString("yyyy-MM-dd");
                    string pos = WebDBManager.GetStringField(arrResult[i + 2].ToString(), "");
                    string category = WebDBManager.GetStringField(arrResult[i + 3].ToString(), "");
                    string version = WebDBManager.GetStringField(arrResult[i + 4].ToString(), "");
                    string normal = WebDBManager.GetIntField(arrResult[i + 5].ToString(), 0) == 1 ? "평일" : "야간·휴일";
                    string opt = WebDBManager.GetStringField(arrResult[i + 6].ToString(), "");
                    EvaluationNone value = new EvaluationNone(id, dt, normal, category, pos, version, opt);
                    m_dicNone.Add(value.id, value);
                }

                // 평가항목 분리
                query = "select ash.ID, eh.ID, eh.Credit FROM evaluationhistory as eh INNER JOIN actionstephistory AS ash ON eh.ActionStepHistoryID=ash.ID";
                arrResult = m_dbMan.GetResultData(query, 0);
                if (arrResult != null && arrResult.Count > 0)
                {
                    for (int i = 0; i < arrResult.Count - 1; i += 3)
                    {
                        int aid = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                        int eid = WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0);
                        string credit = WebDBManager.GetStringField(arrResult[i + 2].ToString(), "");
                        EvaluationNone v;
                        if (m_dicNone.TryGetValue(aid, out v))
                        {
                            EvaluationDone done = new EvaluationDone(eid, v.date, v.mode, v.type, v.pos, credit);

                            m_dicDone.Add(eid, done);
                            m_dicNone.Remove(aid);
                        }
                    }
                }
            }

            SetNoneEvaluation();
            SetDoneEvaluation();
        }

        // 미평가 훈련 목록 설정
        private void SetNoneEvaluation()
        {
            gridNone.Rows.Clear();

            if (m_dicNone.Count == 0)
                return;

            int nCnt = 0;

            foreach (KeyValuePair<int, EvaluationNone> data in m_dicNone)
            {
                string[] rows = new string[7];
                rows[0] = data.Value.type;
                rows[1] = data.Value.date;
                rows[2] = data.Value.mode;
                rows[3] = data.Value.pos;
                rows[4] = data.Value.version;
                rows[5] = data.Value.option;
                rows[6] = "평가하기";

                gridNone.Rows.Add(rows);
                gridNone.Rows[nCnt].Tag = data.Value;
                ++nCnt;
            }
        }

        // 평가 훈련 목록 설정
        private void SetDoneEvaluation()
        {
            gridDone.Rows.Clear();

            if (m_dicDone.Count == 0)
                return;

            int nCnt = 0;

            foreach (KeyValuePair<int, EvaluationDone> data in m_dicDone)
            {
                string[] rows = new string[6];
                rows[0] = data.Value.type;
                rows[1] = data.Value.date;
                rows[2] = data.Value.mode;
                rows[3] = data.Value.pos;
                rows[4] = data.Value.credit;
                rows[5] = "확인";

                gridDone.Rows.Add(rows);
                gridDone.Rows[nCnt].Tag = data.Value;
                ++nCnt;
            }
        }

        // 조회 기간 설정
        private void SetLabel()
        {
            DateTime dtS = dtDueS.Value;
            DateTime dtE = dtDueE.Value;

            string str = string.Format("조회 기간 | {0}부터 {1}까지", dtS.ToString("yyyy년 MM월 dd일"), dtE.ToString("yyyy년 MM월 dd일"));
            labelDue.Text = str;
        }

        private void InitStepData()
        {
            string query = "SELECT ID, Code, Name FROM evaluationstep";
            ArrayList arrRes = m_dbMan.GetResultData(query, 0);
            if (arrRes != null && arrRes.Count > 0)
            {
                for (int i = 0; i < arrRes.Count; i += 3)
                {
                    EvaStep step = new EvaStep();

                    step.id = WebDBManager.GetIntField(arrRes[i].ToString(), 0);
                    step.code = WebDBManager.GetStringField(arrRes[i + 1].ToString(), "");
                    step.name = WebDBManager.GetStringField(arrRes[i + 2].ToString(), "");

                    query = string.Format("SELECT ID, Content FROM evaluationitem WHERE EvaluationStepID={0}", step.id);
                    ArrayList arrItem = m_dbMan.GetResultData(query, 0);
                    if (arrItem != null && arrItem.Count != 0)
                    {
                        for (int k = 0; k < arrItem.Count; k += 2)
                        {
                            EvaItem item = new EvaItem();
                            item.id = WebDBManager.GetIntField(arrItem[k].ToString(), 0);
                            item.name = WebDBManager.GetStringField(arrItem[k + 1].ToString(), "");

                            query = string.Format("SELECT ID, Content FROM evaluationsubitem WHERE EvaluationItemID={0}", item.id);
                            ArrayList arrSubItem = m_dbMan.GetResultData(query, 0);
                            if (arrSubItem != null && arrSubItem.Count != 0)
                            {
                                for (int m = 0; m < arrSubItem.Count; m+=2)
                                {
                                    EvaSubItem subItem = new EvaSubItem();
                                    subItem.id = WebDBManager.GetIntField(arrSubItem[m].ToString(), 0);
                                    subItem.content = WebDBManager.GetStringField(arrSubItem[m+1].ToString(), "");
                                    subItem.credit = 0;

                                    item.listSubItem.Add(subItem);
                                }
                            }

                            step.listItem.Add(item);
                        }
                    }

                    m_listEvaStep.Add(step);
                }
            }
        }

        // 단계별 평가 항목
        private void EvaluationStep(int nStep)
        {
            if (m_listEvaStep.Count == 0)
                return;
            
            labelStep.Text = m_listEvaStep[nStep].name;

            SetDataGridStep(nStep);
        }

        // 단계별 그리드 설정
        private void SetDataGridStep(int nStep)
        {
            if (m_listEvaStep.Count == 0)
                return;

            gridStep.Rows.Clear();

            int nCnt = 1;
            EvaStep step = m_listEvaStep[nStep];
            for (int i = 0; i < step.listItem.Count; ++i)
            {
                EvaItem item = step.listItem[i];

                for(int k=0; k<item.listSubItem.Count; ++k)
                {
                    EvaSubItem subItem = item.listSubItem[k];
                    string[] rows = new string[4];
                    rows[0] = item.name;
                    rows[1] = step.code + nCnt.ToString();
                    rows[2] = subItem.content;
                    rows[3] = m_arrEva[subItem.credit];

                    gridStep.Rows.Add(rows);
                    gridStep.Rows[nCnt-1].Tag = subItem.id;
                    ++nCnt;
                }
            }
        }

        private void Reset()
        {
            SetNoneEvaluation();
            SetDoneEvaluation();

            for (int i = 0; i < m_listEvaStep.Count; ++i)
            {
                EvaStep eStep = m_listEvaStep[i];
                for (int k = 0; k < eStep.listItem.Count; ++k)
                {
                    EvaItem item = eStep.listItem[k];
                    for (int m = 0; m < item.listSubItem.Count; ++m)
                    {
                        item.listSubItem[m].credit = 0;
                    }
                }
            }
        }

    #region panelTitle event
        bool m_bLeftMouseDown = false;
        Point m_ptMove;

        private void btnMin_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void btnMax_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
                btnMax.BackgroundImage = global::TrainingEvaluation.Properties.Resources.NormalWindow_Normal;
            }
            else if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
                btnMax.BackgroundImage = global::TrainingEvaluation.Properties.Resources.MaxWindow_Normal;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panelTitle_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = panelTitle.PointToScreen(new Point(e.X, e.Y));
            }
        }

        private void panelTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_bLeftMouseDown == true)
                {
                    //Point ptScreen = new Point(e.X, e.Y);                     
                    Point ptCur = this.Location;

                    Point pt = panelTitle.PointToScreen(new Point(e.X, e.Y));
                    int dx = pt.X - m_ptMove.X;
                    int dy = pt.Y - m_ptMove.Y;
                    if (!(dx == 0 && dy == 0))
                    {
                        if (this.WindowState == FormWindowState.Maximized)
                        {
                            //PanelTop Click한 지점 %
                            double maxPerX = Math.Round((double)pt.X * 100 / this.Size.Width);
                            double maxPerY = Math.Round((double)pt.Y * 100 / this.Size.Height);
                            if (maxPerX > 100) maxPerX = maxPerX - 100; 

                            this.WindowState = FormWindowState.Normal;
                            //Size 변경된 후 maxPerX(%)에 알맞은 Mouse Cursor지점
                            int normalPerX = Convert.ToInt32(this.Size.Width * maxPerX / 100);
                            int normalPerY = Convert.ToInt32(this.Size.Height * maxPerY / 100);

                            this.Location = new Point(pt.X - normalPerX, pt.Y - normalPerY);
                            btnMax.BackgroundImage = global::TrainingEvaluation.Properties.Resources.MaxWindow_Normal;
                        }
                        else 
                            this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);

                        m_ptMove.X += dx;
                        m_ptMove.Y += dy;
                    }
                }
            }
        }

        private void panelTitle_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

        private void panelTitle_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
                btnMax.BackgroundImage = global::TrainingEvaluation.Properties.Resources.NormalWindow_Normal;
            }
            else if (this.WindowState == FormWindowState.Maximized)
            {
                Size sizeCur = this.Size;
                this.WindowState = FormWindowState.Normal;
                btnMax.BackgroundImage = global::TrainingEvaluation.Properties.Resources.MaxWindow_Normal;
                Size sizeNormal = this.Size;

                double hRate = (double)sizeNormal.Height / (double)sizeCur.Height;
                this.Size = new Size((int)(sizeCur.Width * hRate), sizeNormal.Height);
            }
        }
    #endregion

    #region panelTop event
        private void dtDueS_ValueChanged(object sender, EventArgs e)
        {
            //btnDueOk_Click(null, null);
        }

        private void dtDueE_ValueChanged(object sender, EventArgs e)
        {
            //btnDueOk_Click(null, null);
        }

        private void cbDue_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDue();
        }

        private void cbDisasterType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //btnDueOk_Click(null, null);
        }

        private void btnDueOk_Click(object sender, EventArgs e)
        {
            SetLabel();
            SetEvaluation();
        }
    #endregion

    #region panelMain event
        private void btnNone_Click(object sender, EventArgs e)
        {
            //btnNone.BackgroundImage = global::TrainingEvaluation.Properties.Resources.Tab_Pressed;
            //btnDone.BackgroundImage = global::TrainingEvaluation.Properties.Resources.Tab_Normal;
            btnNone.BackColor = Color.Maroon;
            btnDone.BackColor = Color.Transparent;

            panelStep.Visible = false;
            panelMain.Visible = true;

            gridDone.Visible = false;
            gridNone.Visible = true;
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            //btnNone.BackgroundImage = global::TrainingEvaluation.Properties.Resources.Tab_Normal;
            //btnDone.BackgroundImage = global::TrainingEvaluation.Properties.Resources.Tab_Pressed;
            btnNone.BackColor = Color.Transparent;
            btnDone.BackColor = Color.Maroon;

            panelStep.Visible = false;
            panelMain.Visible = true;

            gridNone.Visible = false;
            gridDone.Visible = true;
        }
    #endregion

    #region grid event
        private void gridNone_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 6)
            {
                m_selectNone = (EvaluationNone)gridNone.Rows[e.RowIndex].Tag;
                if (m_selectNone == null)
                    return;

                panelMain.Visible = false;
                panelStep.Visible = true;

                m_nCurrentStep = 0;
                EvaluationStep(m_nCurrentStep);
            }
        }

        private void gridDone_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 5)
            {
                EvaluationDone done = (EvaluationDone)gridDone.Rows[e.RowIndex].Tag;
                if (done == null)
                    return;

                FormComment form = new FormComment(m_dbMan, done.id);
                form.ShowDialog();
            }
        }

        private void gridStep_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (gridStep.IsCurrentCellDirty)
            {
                // This fires the cell value changed handler below
                gridStep.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
 
        private void gridStep_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewComboBoxCell combo = (DataGridViewComboBoxCell)gridStep.Rows[e.RowIndex].Cells[3];
            if (combo != null && combo.Value != null)
            {
                int id = (int)gridStep.Rows[e.RowIndex].Tag;
                int nSel = combo.Items.IndexOf(combo.Value);

                EvaStep step = m_listEvaStep[m_nCurrentStep];
                for (int i = 0; i < step.listItem.Count; ++i)
                {
                    EvaItem item = step.listItem[i];
                    for (int k = 0; k < item.listSubItem.Count; ++k)
                    {
                        EvaSubItem subItem = item.listSubItem[k];
                        if (subItem.id == id)
                        {
                            subItem.credit = nSel;
                            break;
                        }
                    }
                }
            }
        }
    #endregion

    #region panelStep event
        private void btnPrev_Click(object sender, EventArgs e)
        {
            if(m_nCurrentStep == 0)
            {
                panelMain.Visible = true;
                panelStep.Visible = false;
            }
            else
            {
                --m_nCurrentStep;
                EvaluationStep(m_nCurrentStep);
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (m_selectNone == null)
                return;

            EvaStep step = m_listEvaStep[m_nCurrentStep];
            for(int i=0; i<step.listItem.Count; ++i)
            {
                EvaItem item = step.listItem[i];
                for(int k=0; k<item.listSubItem.Count; ++k)
                {
                    if(item.listSubItem[k].credit == 0)
                    {
                        MessageBox.Show("평가되지 않은 항목이 있습니다.", "평가 확인");
                        return;
                    }
                }
            }

            if(m_nCurrentStep == m_listEvaStep.Count-1)
            {
                int nCredit = 0;
                int nCnt = 0;
                int creditCnt = m_arrEva.Length;

                for (int i = 0; i < m_listEvaStep.Count; ++i)
                {
                    EvaStep eStep = m_listEvaStep[i];
                    for (int k = 0; k < eStep.listItem.Count; ++k)
                    {
                        EvaItem item = eStep.listItem[k];
                        for (int m = 0; m < item.listSubItem.Count; ++m)
                        {
                            nCredit += creditCnt - item.listSubItem[m].credit;
                            ++nCnt;
                        }
                    }
                }
                double dTotalCredit = (double)nCredit / (double)nCnt;
                nCredit = (int)Math.Round(dTotalCredit);
                string strCredit = m_arrEva[creditCnt-nCredit];

                // 기존 데이터 있는지 확인
                string query = string.Format("SELECT COUNT(*) FROM evaluationhistory WHERE ActionStepHistoryID={0}", m_selectNone.id);
                ArrayList arrRes = m_dbMan.GetResultData(query, 0);
                if (arrRes != null && arrRes.Count != 0)
                {
                    int cnt = WebDBManager.GetIntField(arrRes[0].ToString(), 0);

                    // 기존 데이터 없으면 새로 입력
                    if (cnt == 0)
                    {
                        query = "SELECT MAX(ID) FROM evaluationhistory";
                        arrRes = m_dbMan.GetResultData(query, 0);
                        if (arrRes != null && arrRes.Count != 0)
                        {
                            int id = WebDBManager.GetIntField(arrRes[0].ToString(), 0);
                            ++id;

                            query = string.Format("INSERT INTO evaluationhistory (ID, Credit, CreateTime, ActionStepHistoryID) values({0}, '{1}', '{2}', {3})",
                                id, strCredit, m_selectNone.date, m_selectNone.id);

                            if (m_dbMan.GetResultData(query, 0) != null)
                            {
                                EvaluationDone done = new EvaluationDone(id, m_selectNone.date, m_selectNone.mode, m_selectNone.type, m_selectNone.pos, strCredit);
                                m_dicDone.Add(id, done);
                                m_dicNone.Remove(m_selectNone.id);
                                btnDone_Click(null, null);

                                Reset();

                                FormComment form = new FormComment(m_dbMan, done.id);
                                form.ShowDialog();
                            }
                        }
                    }
                    else // 기존 데이터 존재하면 UPDATE
                    {
                        query = string.Format("UPDATE evaluationhistory SET Credit={0} WHERE ActionStepHistoryID={1}",
                                strCredit, m_selectNone.id);

                        if (m_dbMan.GetResultData(query, 0) != null)
                        {
                            //EvaluationDone done = new EvaluationDone(id, m_selectNone.date, m_selectNone.mode, m_selectNone.type, m_selectNone.pos, strCredit);
                            //m_dicDone.Add(id, done);
                            //m_dicNone.Remove(m_selectNone.id);
                        }
                    }
                }
            }
            else
            {
                ++m_nCurrentStep;
                EvaluationStep(m_nCurrentStep);
            }
        }
    #endregion
    }

    #region 사용자 정의 클래스
    public class Data_DisasterCategory
    {
        private int m_nID;
        private string m_strCategoryName;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string CategoryName
        {
            get { return m_strCategoryName; }
            set { m_strCategoryName = value; }
        }

        public override string ToString()
        {
            return m_strCategoryName;
        }
    }

    public class Evaluation
    {
        protected int m_nID;
        protected string m_strDate;          // 훈련일
        protected string m_strMode;          // 1:평일, 0:야간,휴일
        protected string m_strDisasterType;  // 재난 종류
        protected string m_strPos;           // 위치

        public int id { get { return m_nID; } }
        public string date { get { return m_strDate; } }
        public string mode { get { return m_strMode; } }
        public string type { get { return m_strDisasterType; } }
        public string pos { get { return m_strPos; } }
    }

    public class EvaluationNone : Evaluation
    {
        string m_strVersion;    // version
        string m_strOption;     // 비고

        public EvaluationNone(int id, string dt, string mode, string type, string pos, string ver, string opt)
        {
            m_nID = id;
            m_strDate = dt;
            m_strMode = mode;
            m_strDisasterType = type;
            m_strPos = pos;
            m_strVersion = ver;
            m_strOption = opt;
            if (m_strOption == "null")
                m_strOption = "";
        }

        public string version { get { return m_strVersion; } }
        public string option { get { return m_strOption; } }
    }

    public class EvaluationDone : Evaluation
    {
        string m_strCredit; // 등급

        public EvaluationDone(int id, string dt, string mode, string type, string pos, string credit)
        {
            m_nID = id;
            m_strDate = dt;
            m_strMode = mode;
            m_strDisasterType = type;
            m_strPos = pos;
            m_strCredit = credit;
        }

        public string credit { get { return m_strCredit; } }
    }

    public class EvaStep
    {
        public int id;
        public string code;
        public string name;
        public List<EvaItem> listItem = new List<EvaItem>();
    }

    public class EvaItem
    {
        public int id;
        public string name;
        public List<EvaSubItem> listSubItem = new List<EvaSubItem>();
    }

    public class EvaSubItem
    {
        public int id;
        public string content;
        public int credit;
    }
    #endregion
}
