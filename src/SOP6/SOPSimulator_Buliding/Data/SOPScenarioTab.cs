using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.GUI;
using UnE.SOP.Workstate;
using System.Windows.Forms;
using System.Drawing;
using UnE.SOP.Sections;
using System.Collections.Concurrent;

namespace SOPMonitoringSystem.Data
{
    public class SOPScenarioTab
    {
        private List<SOPScenario> m_scenarios = new List<SOPScenario>();
        // Key : 상위 4바이트(Disaster ID), 하위 4바이트(ActionStepHistory ID)
        //       Key에 SOPScenario를 넣지 않는 이유는 SOPScenario가 생성되기 전에 데이터 생성을 해야할 경우가 있기 때문이다.
        private ConcurrentDictionary<long, SOPScenarioButton> m_dicButtonScenarios = new ConcurrentDictionary<long, SOPScenarioButton>();
        private SOPScenario m_selectedScenario = null;
        private bool m_isEnabled = false;
        private PageBackstageSOP m_owner = null;
        private Control m_parentCtrl = null;

        private static int m_nIndexCount = 1;

        public bool Enabled
        {
            get { return m_isEnabled; }
            set { m_isEnabled = value; }
        }

        public SOPScenario CurrentScenario
        {
            get { return m_selectedScenario; }
        }

        public SOPScenarioTab(PageBackstageSOP owner, Control parentCtrl)
        {
            m_owner = owner;
            m_parentCtrl = parentCtrl;
        }

        public int Add(SOPScenario scenario, SectionTabPage page)
        {
            int nIndex = m_scenarios.IndexOf(scenario);

            if (nIndex >= 0)
                return nIndex;

            m_scenarios.Add(scenario);
            nIndex = m_scenarios.IndexOf(scenario);

            AddScenarioButton(scenario, page);

            // 현재 선택되어진 시나리오와 같은 Disaster ID를 공유하면서 더 단계가 높은 시나리오가 추가되면
            // 해당 시나리오를 선택시킨다.
            /*if (m_selectedScenario != null && m_selectedScenario.DisasterID == scenario.DisasterID)
            {
                int nIndex1 = GetActionStepIndex(m_selectedScenario.ActionStepName);
                int nIndex2 = GetActionStepIndex(scenario.ActionStepName);

                if (nIndex2 > nIndex1)
                    Select(scenario);
            }*/

            return nIndex;
        }

        private void AddScenarioButton(SOPScenario scenario, SectionTabPage page)
        {
            SOPScenarioButton btn;
            long key = MakeKey(scenario);

            if (m_dicButtonScenarios.TryGetValue(key, out btn) == false)
            {
                btn = CreateButton(scenario, page);
                m_dicButtonScenarios[key] = btn;
            }
            else
            {
                int nActionStepIndex = GetActionStepIndex(page.Text);

                if (nActionStepIndex >= 0)
                {
                    if (btn.GetTabPage(nActionStepIndex) == null)
                        btn.SetTabPage(page, nActionStepIndex);

                    if (btn.GetScenario(nActionStepIndex) == null)
                        btn.SetScenario(scenario, nActionStepIndex);

                    // 같은 DisasterID를 공휴하는 다른 Button들에도 TabPage를 적용한다.
                    SetTabPageToOtherButtons(page, nActionStepIndex, scenario.DisasterID, scenario.ActionStepHistoryID);
                }
            }

            btn.Scenario = scenario;
            btn.Text = scenario.ToString();
            ArrangeButtons();
        }

        private long MakeKey(SOPScenario scenario)
        {
            if (scenario == null)
                return 0;

            long disasterID = (long)scenario.DisasterID;
            long actionStepHistoryID = (long)scenario.ActionStepHistoryID;
            long key = ((disasterID << 32) | actionStepHistoryID);

            return key;
        }

        private long MakeKey(int nDisasterID, int nActionStepHistoryID)
        {
            long disasterID = (long)nDisasterID;
            long actionStepHistoryID = (long)nActionStepHistoryID;
            long key = ((disasterID << 32) | actionStepHistoryID);

            return key;
        }

        private SOPScenarioButton CreateButton(SOPScenario scenario, SectionTabPage page)
        {
            return CreateButton(scenario.DisasterID, scenario.ActionStepHistoryID, page, scenario.ToString(), scenario);
            /*string strText = scenario.ToString();

            SOPScenarioButton btn = new SOPScenarioButton();
            btn.BackColor = System.Drawing.Color.Transparent;
            btn.CheckButton = false;
            btn.CheckedImage = global::SOPMonitoringSystem.Properties.Resources.Scenario_Tab_Selected;
            btn.ClickedImage = global::SOPMonitoringSystem.Properties.Resources.Scenario_Tab_Selected;
            btn.MouseOverImage = global::SOPMonitoringSystem.Properties.Resources.Scenario_Tab_MouseOver;
            btn.NormalImage = global::SOPMonitoringSystem.Properties.Resources.Scenario_Tab_Normal;

            btn.CustomImageRect = new System.Drawing.Rectangle(0, 0, 160, 45);
            btn.ForeColor = System.Drawing.Color.Black;
            btn.ForeColorChecked = System.Drawing.Color.Black;
            btn.ForeColorCheckedMouseOver = System.Drawing.Color.Black;
            btn.ForeColorDisabled = System.Drawing.Color.Black;
            btn.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            btn.ForeColorsByTypeUse = true;
            btn.ID = -1;
            btn.InitButtonWidth = 160;
            btn.IsChecked = false;
            btn.Name = "btnScenarioTab" + (m_nIndexCount++).ToString();

            btn.Owner = null;
            btn.Font = new System.Drawing.Font("나눔바른고딕", 14.25F, System.Drawing.FontStyle.Bold);
            btn.Text = strText;
            btn.TextLocation = new System.Drawing.Point(0, 15);
            btn.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            btn.ToolTipText = strText;
            btn.UseCustomImageRect = true;
            btn.UseTextLocation = true;
            btn.UseVisualStyleBackColor = false;
            btn.Size = new System.Drawing.Size(160, 45);
            btn.Click += new System.EventHandler(this.OnClick);

            m_parentCtrl.Controls.Add(btn);
            ArrangeButtons();

            int nActionStepIndex = GetActionStepIndex(scenario.ActionStepName);

            if (nActionStepIndex >= 0)
            {
                btn.SetScenario(scenario, nActionStepIndex);
                btn.SetTabPage(page, nActionStepIndex);
                btn.SelectedIndex = nActionStepIndex;
            }
            
            btn.DisasterID = scenario.DisasterID;
            return btn;*/
        }

        private SOPScenarioButton CreateButton(int nDisasterID, int nActionStepHistoryID, SectionTabPage page, string strText, SOPScenario scenario)
        {
            //string strText = scenario.ToString();

            SOPScenarioButton btn = new SOPScenarioButton();
            btn.BackColor = System.Drawing.Color.Transparent;
            btn.CheckButton = false;
            btn.CheckedImage = global::SOPMonitoringSystem.Properties.Resources.Scenario_Tab_Selected;
            btn.ClickedImage = global::SOPMonitoringSystem.Properties.Resources.Scenario_Tab_Selected;
            btn.MouseOverImage = global::SOPMonitoringSystem.Properties.Resources.Scenario_Tab_MouseOver;
            btn.NormalImage = global::SOPMonitoringSystem.Properties.Resources.Scenario_Tab_Normal;

            btn.CustomImageRect = new System.Drawing.Rectangle(0, 0, 160, 45);
            btn.ForeColor = System.Drawing.Color.Black;
            btn.ForeColorChecked = System.Drawing.Color.Black;
            btn.ForeColorCheckedMouseOver = System.Drawing.Color.Black;
            btn.ForeColorDisabled = System.Drawing.Color.Black;
            btn.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            btn.ForeColorsByTypeUse = true;
            btn.ID = -1;
            btn.InitButtonWidth = 160;
            btn.IsChecked = false;
            btn.Name = "btnScenarioTab" + (m_nIndexCount++).ToString();

            btn.Owner = null;
            btn.Font = new System.Drawing.Font("나눔바른고딕", 14.25F, System.Drawing.FontStyle.Bold);
            btn.Text = strText;
            btn.TextLocation = new System.Drawing.Point(0, 15);
            btn.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            btn.ToolTipText = strText;
            btn.UseCustomImageRect = true;
            btn.UseTextLocation = true;
            btn.UseVisualStyleBackColor = false;
            btn.Size = new System.Drawing.Size(160, 45);
            btn.Click += new System.EventHandler(this.OnClick);

            m_parentCtrl.Controls.Add(btn);
            ArrangeButtons();

            string strActionStepName = scenario == null ? "" : scenario.ActionStepName;

            if (strActionStepName == "")
            {
                if (page != null)
                    strActionStepName = page.Text;
            }

            int nActionStepIndex = GetActionStepIndex(strActionStepName);

            if (nActionStepIndex >= 0)
            {
                btn.SetScenario(scenario, nActionStepIndex);
                btn.SetTabPage(page, nActionStepIndex);
                btn.SelectedIndex = nActionStepIndex;
            }

            btn.DisasterID = nDisasterID;
            btn.ActionStepHistoryID = nActionStepHistoryID;
            return btn;
        }

        public int GetActionStepIndex(string strActionStepName)
        {
            int nActionStepCount = UnE.SOP.Sections.SectionTabControl.StandardActionStepNames.Count();

            for (int i = 0; i < nActionStepCount; i++)
            {
                if (strActionStepName == UnE.SOP.Sections.SectionTabControl.StandardActionStepNames[i])
                {
                    return i;
                }
            }

            return -1;
        }

        private void OnClick(object sender, EventArgs e)
        {
            SOPScenarioButton btn = (SOPScenarioButton)sender;
            SOPScenario scenario = btn.GetScenario(btn.SelectedIndex);

            if (scenario != null)
                Select(scenario);
            else
            {
                ChangeSelection(btn.Scenario);
                PageBackstageSOP pageHome = FormSOP.Instance.GetPageHome();

                SectionTabPage page = btn.GetTabPage(btn.SelectedIndex);
                pageHome.TargetPage = page;

                if (page != null)
                    pageHome.ChangeTabPage(page, btn.DisasterID, btn.ActionStepHistoryID);
            }

            /*SOPScenario scenario;

            if (m_dicButtonScenarios.TryGetValue((SOPScenarioButton)sender, out scenario))
            {
                Select(scenario);
            }*/
        }

        public void Insert(int nIndex, SOPScenario scenario, SectionTabPage page)
        {
            if (m_scenarios.IndexOf(scenario) < 0)
            {
                m_scenarios.Insert(nIndex, scenario);
                AddScenarioButton(scenario, page);
            }
        }

        public SectionTabPage Remove(SOPScenario scenario)
        {
            m_scenarios.Remove(scenario);
            SectionTabPage removePage = RemoveButton(scenario);

            if (m_selectedScenario == scenario)
            {
                if (m_scenarios.Count > 0)
                    Select(m_scenarios[0]);
                else
                    m_selectedScenario = null;
            }

            return removePage;
        }

        public SectionTabPage RemoveAt(int nIndex)
        {
            if (nIndex >= 0 && m_scenarios.Count > nIndex)
            {
                SOPScenario scenario = m_scenarios[nIndex];
                m_scenarios.RemoveAt(nIndex);

                SectionTabPage removePage = RemoveButton(scenario);

                if (m_selectedScenario == scenario)
                {
                    if (m_scenarios.Count > 0)
                        Select(m_scenarios[0]);
                    else
                        m_selectedScenario = null;
                }

                return removePage;
            }

            return null;
        }

        private SectionTabPage RemoveButton(SOPScenario scenario)
        {
            SOPScenarioButton btn = GetButton(scenario);

            if (btn != null)
            {
                SectionTabPage removePage = null;

                if (btn.Scenario != null)
                {
                    SectionTabPage[] tabPages = btn.GetTabPages();

                    foreach (SectionTabPage tabPage in tabPages)
                    {
                        if (tabPage != null && btn.Scenario.ActionStepID == tabPage.ActionStepID)
                        {
                            removePage = tabPage;
                            break;
                        }
                    }
                }

                long key = MakeKey(scenario);
                SOPScenarioButton temp;

                m_dicButtonScenarios.TryRemove(key, out temp);
                //m_dicButtonScenarios.Remove(btn);
                m_parentCtrl.Controls.Remove(btn);
                ArrangeButtons();

                return removePage;
            }

            return null;
        }

        private SOPScenarioButton GetButton(SOPScenario scenario)
        {
            long key = MakeKey(scenario);
            SOPScenarioButton btn;

            if (m_dicButtonScenarios.TryGetValue(key, out btn))
                return btn;
            /*foreach (KeyValuePair<long, SOPScenarioButton> pair in m_dicButtonScenarios)
            {
                if (pair.Value.IndexOf(scenario) >= 0)
                {
                    return pair.Value;
                }
            }*/

            return null;
        }

        private void ArrangeButtons()
        {
            int x = 30, space = 10;
            int y = -1, nButtonWidth = 0, nIndex = 0;

            int gapWidth = 10;

            SortButtons();

            foreach (Control ctrl in m_parentCtrl.Controls)
            {
                if (ctrl is SOPScenarioButton)
                {
                    SOPScenarioButton btn = (SOPScenarioButton)ctrl;

                    SizeF size = m_owner.GetMeasureString(btn.Text);
                    int width = (int)(size.Width + (gapWidth * 2));

                    if (width < 160)
                    {
                        width = 160;
                        btn.TextLocation = new Point(0, 15);
                    }
                    else
                        btn.TextLocation = new Point(10, 15);

                    btn.CustomImageRect = new System.Drawing.Rectangle(0, 0, width, 45);
                    btn.InitButtonWidth = width;
                    btn.Size = new Size(width, btn.Size.Height);

                    y = m_parentCtrl.Size.Height - btn.Size.Height;
                    btn.Location = new Point(x, y);
                    x = btn.Size.Width + btn.Location.X;
                }
            }
        }

        private void SortButtons()
        {
            int nPrevActionStepHistoryID = -1;
            bool needSort = false;
            List<SOPScenarioButton> buttons = new List<SOPScenarioButton>();

            foreach (Control ctrl in m_parentCtrl.Controls)
            {
                if (ctrl is SOPScenarioButton)
                {
                    SOPScenarioButton btn = (SOPScenarioButton)ctrl;

                    if (btn.ActionStepHistoryID < nPrevActionStepHistoryID)
                        needSort = true;

                    buttons.Add(btn);
                    nPrevActionStepHistoryID = btn.ActionStepHistoryID;
                }
            }

            if (needSort)
            {
                buttons.Sort();

                foreach (SOPScenarioButton btn in buttons)
                {
                    m_parentCtrl.Controls.Remove(btn);
                }

                foreach (SOPScenarioButton btn in buttons)
                {
                    m_parentCtrl.Controls.Add(btn);
                }
            }
        }

        public int IndexOf(SOPScenario scenario)
        {
            return m_scenarios.IndexOf(scenario);
        }

        public SOPScenario Select(SOPScenario scenario, bool systemCall = false)
        {
            if (m_scenarios.Contains(scenario))
            {
                SOPScenario prevScenario = m_selectedScenario;
                bool changedScenario = m_selectedScenario != scenario;
                ChangeSelection(scenario);

                FormSOP.Instance.GetPageHome().TargetPage = GetTabPage(scenario);

                //m_selectedScenario = scenario;
                SelectScenario(scenario, prevScenario, changedScenario, systemCall);
                return scenario;
            }

            return null;
        }

        public SOPScenario Select(int nIndex, bool systemCall = false)
        {
            if (nIndex >= 0 && m_scenarios.Count > nIndex)
            {
                SOPScenario prevScenario = m_selectedScenario;
                bool changedScenario = m_selectedScenario == m_scenarios[nIndex];
                ChangeSelection(m_scenarios[nIndex]);

                //m_selectedScenario = m_scenarios[nIndex];
                SelectScenario(m_selectedScenario, prevScenario, changedScenario, systemCall);
                return m_selectedScenario;
            }

            return null;
        }

        private void SelectScenario(SOPScenario scenario, SOPScenario prevScenario, bool changedScenario, bool systemCall)
        {
            if (scenario == null)
                return;

            if (systemCall)
                return;

            bool bNeedChangeSOP = false;
            TabControl tab = FormSOP.Instance.GetPageHome().TabControls;
            if (tab != null)
            {
                int nActionStepID = scenario.ActionStepID;
                bool bReal = scenario.RealMode;

                SectionTabPage page = (SectionTabPage)tab.SelectedTab;
                if (page != null)
                {
                    if (page.ActionStepID != nActionStepID)
                    {
                        bNeedChangeSOP = true;
                    }
                    if (page.VirtualMode == bReal)
                    {
                        bNeedChangeSOP = true;
                    }
                }
            }

            FormSOP.Instance.SetRealModeStatus(scenario.RealMode);

            if (changedScenario == false && bNeedChangeSOP == false)
                return;

            m_owner.OnChangeSOPScenario(scenario, bNeedChangeSOP);
        }

        /*public SectionTabPage FindTabPage(SOPScenario scenario)
        {
            foreach (KeyValuePair<SOPScenarioButton, SOPScenario> pair in m_dicButtonScenarios)
            {

            }
        }*/

        public void SelectTabPage(SectionTabPage tabPage)
        {
            if (m_selectedScenario != null)
            {
                SOPScenarioButton btn;
                long key = MakeKey(m_selectedScenario);

                if (m_dicButtonScenarios.TryGetValue(key, out btn))
                {
                    int nIndex = btn.IndexOf(tabPage);

                    if (nIndex >= 0)
                    {
                        btn.SelectedIndex = nIndex;
                        return;
                    }
                }
            }

            List<SOPScenarioButton> scenarioButtons = m_dicButtonScenarios.Values.ToList();

            foreach (SOPScenarioButton btn in scenarioButtons)
            {
                int nIndex = btn.IndexOf(tabPage);

                if (nIndex >= 0)
                {
                    btn.SelectedIndex = nIndex;
                    return;
                }
            }
            /*foreach (KeyValuePair<int, SOPScenarioButton> pair in m_dicButtonScenarios)
            {
                int nIndex = pair.Value.IndexOf(tabPage);

                if (nIndex >= 0)
                {
                    pair.Value.SelectedIndex = nIndex;
                    return;
                }
            }*/
        }

        public void AddTabPage(SectionTabPage tabPage, int nDisasterID, int nActionStepHistoryID)
        {
            if (nDisasterID <= 0)
                return;

            if (_AddTabPage(tabPage, nDisasterID, nActionStepHistoryID))
                return;

            if (nActionStepHistoryID > 0)
            {
                bool isNormal, isRegular;
                UnE.SOP.DisasterInfo disaster = FormSOP.Instance.SOPManager.GetDisaster(nDisasterID, out isNormal, out isRegular);

                if (disaster != null)
                {
                    SOPScenarioButton btn;
                    long key = MakeKey(nDisasterID, nActionStepHistoryID);

                    if (m_dicButtonScenarios.TryGetValue(key, out btn) == false)
                    {
                        btn = null;
                        SOPScenarioButton _btn = null;
                        List<long> keys = m_dicButtonScenarios.Keys.ToList();

                        foreach (long _key in keys)
                        {
                            if (m_dicButtonScenarios.TryGetValue(_key, out _btn) == false)
                                continue;

                            if (_btn.Scenario == null)
                                continue;

                            if (_btn.Scenario.DisasterID == nDisasterID && _btn.Scenario.ActionStepHistoryID == nActionStepHistoryID)
                            {
                                _btn.ActionStepHistoryID = nActionStepHistoryID;
                                btn = _btn;

                                m_dicButtonScenarios.TryRemove(_key, out _btn);
                                m_dicButtonScenarios[key] = btn;
                                break;
                            }
                        }

                        if (btn == null)
                        {
                            btn = CreateButton(nDisasterID, nActionStepHistoryID, tabPage, disaster.DisasterName, null);
                            m_dicButtonScenarios[key] = btn;
                        }
                    }

                    _AddTabPage(tabPage, nDisasterID, nActionStepHistoryID);
                }
            }
        }

        private bool _AddTabPage(SectionTabPage tabPage, int nDisasterID, int nActionStepHistoryID)
        {
            if (nActionStepHistoryID > 0)
            {
                SOPScenarioButton btn;
                long key = MakeKey(nDisasterID, nActionStepHistoryID);

                if (m_dicButtonScenarios.TryGetValue(key, out btn))
                {
                    int nActionStepIndex = GetActionStepIndex(tabPage.Text);

                    if (nActionStepIndex >= 0)
                    {
                        btn.SetTabPage(tabPage, nActionStepIndex);

                        // 같은 DisasterID를 공휴하는 다른 Button들에도 TabPage를 적용한다.
                        SetTabPageToOtherButtons(tabPage, nActionStepIndex, nDisasterID, nActionStepHistoryID);
                    }

                    return true;
                }
            }
            else
            {
                int nActionStepIndex = GetActionStepIndex(tabPage.Text);

                if (nActionStepIndex >= 0)
                {
                    // 같은 DisasterID를 공휴하는 다른 Button들에도 TabPage를 적용한다.
                    return SetTabPageToOtherButtons(tabPage, nActionStepIndex, nDisasterID, nActionStepHistoryID);
                }
            }
            
            return false;
        }

        // 같은 DisasterID를 공휴하는 다른 Button들에도 TabPage를 적용한다.
        private bool SetTabPageToOtherButtons(SectionTabPage tabPage, int nActionStepIndex, int nDisasterID, int nActionStepHistoryID)
        {
            bool added = false;
            List<SOPScenarioButton> scenarioButtons = m_dicButtonScenarios.Values.ToList();

            foreach (SOPScenarioButton btn in scenarioButtons)
            {
                if (btn.DisasterID == nDisasterID && btn.ActionStepHistoryID != nActionStepHistoryID)
                {
                    btn.SetTabPage(tabPage, nActionStepIndex);
                    added = true;
                }
            }

            return added;
        }

        public SectionTabPage GetTabPage(int nDisasterID, int nActionStepID)
        {
            List<SOPScenarioButton> scenarioButtons = m_dicButtonScenarios.Values.ToList();

            foreach (SOPScenarioButton btn in scenarioButtons)
            {
                if (btn.DisasterID == nDisasterID)
                {
                    return btn.FindTabPage(nActionStepID);
                }
            }
            /*SOPScenarioButton btn;

            if (m_dicButtonScenarios.TryGetValue(nDisasterID, out btn))
            {
                return btn.FindTabPage(nActionStepID);
            }*/

            return null;
        }

        public SectionTabPage GetTabPage(SOPScenario scenario)
        {
            SOPScenarioButton btn;
            long key = MakeKey(scenario);

            if (m_dicButtonScenarios.TryGetValue(key, out btn))
            {
                return btn.FindTabPage(scenario.ActionStepID, scenario.RealMode);
            }

            return null;
        }

        public SectionTabPage GetTabPage(int nActionStepHistoryID)
        {
            SOPScenarioButton btn;
            List<long> keys = m_dicButtonScenarios.Keys.ToList();

            foreach (long _key in keys)
            {
                if (m_dicButtonScenarios.TryGetValue(_key, out btn) == false)
                    continue;

                if (btn.ActionStepHistoryID == nActionStepHistoryID)
                {
                    return btn.FindTabPageFromHistory(nActionStepHistoryID);
                }
                else if (btn.Scenario != null && btn.Scenario.ActionStepHistoryID == nActionStepHistoryID)
                {
                    long key = MakeKey(btn.Scenario);
                    SOPScenarioButton temp;

                    m_dicButtonScenarios.TryRemove(_key, out temp);
                    m_dicButtonScenarios[key] = btn;

                    return btn.FindTabPageFromHistory(nActionStepHistoryID);
                }
            }

            return null;
        }

        public SectionTabPage GetTabPage(int nActionStepID, bool realMode)
        {
            List<SOPScenarioButton> scenarioButtons = m_dicButtonScenarios.Values.ToList();

            foreach (SOPScenarioButton btn in scenarioButtons)
            {
                SectionTabPage page = btn.FindTabPage(nActionStepID, realMode);

                if (page != null)
                    return page;
            }
            
            return null;
        }

        private void ChangeSelection(SOPScenario scenario)
        {
            if (m_selectedScenario != scenario)
            {
                if (m_selectedScenario != null)
                    CheckButton(m_selectedScenario, false);

                m_selectedScenario = scenario;

                if (m_selectedScenario != null)
                    CheckButton(m_selectedScenario, true);
            }
        }

        private void CheckButton(SOPScenario scenario, bool isChecked)
        {
            SOPScenarioButton btn = GetButton(scenario);

            if (btn != null)
            {
                btn.IsChecked = isChecked;
                btn.Refresh();
            }
        }

        public int GetScenarioCount()
        {
            return m_scenarios.Count;
        }

        public SOPScenario GetScenario(int nIndex)
        {
            if (nIndex >= 0 && m_scenarios.Count > nIndex)
            {
                return m_scenarios[nIndex];
            }

            return null;
        }

        public SOPScenario Find(int nActionStepID, bool isRealMode)
        {
            foreach (SOPScenario scenario in m_scenarios)
            {
                if (scenario.ActionStepID == nActionStepID && scenario.RealMode == isRealMode)
                    return scenario;
            }

            return null;
        }

        public void UpdateText(int nIndex)
        {

        }


        public void Clear()
        {
            m_scenarios.Clear();
            m_selectedScenario = null;
        }

        public SectionTabPage[] GetTabPages(int nDisasterID, int nActionStepHistoryID)
        {
            SOPScenarioButton btn;
            long key = MakeKey(nDisasterID, nActionStepHistoryID);

            if (m_dicButtonScenarios.TryGetValue(key, out btn))
            {
                return btn.GetTabPages();
            }

            return null;
        }
    }

    public class SOPScenarioButton : RibbonButton, IComparable
    {
        private const int ActionStepCount = 4;

        private SectionTabPage[] m_tabPages = new SectionTabPage[] { null, null, null, null };
        private SOPScenario[] m_scenarios = new SOPScenario[] { null, null, null, null };
        private WorkFlow[] m_works = new WorkFlow[] { null, null, null, null };

        private int m_nDisasterID = -1;
        private int m_nActionStepHistoyID = -1;
        private int m_nSelectedIndex = -1;
        // Button의 대표 시나리오
        // SOPScenarioButton은 하나의 Workflow에서 시작된다.
        // 즉, ActionStepHistory가 생성된 SOP라는 의미가 된다.
        // 다른 단계의 SOP가 실행중일 수도 있는데, 그 경우에는 해당 단계의 SOP는 다른 SOPScenarioButton을 갖게 된다.
        private SOPScenario m_scenario = null;

        public int SelectedIndex
        {
            get { return m_nSelectedIndex; }
            set { m_nSelectedIndex = value; }
        }

        public int DisasterID
        {
            get { return m_nDisasterID; }
            set { m_nDisasterID = value; }
        }

        public int ActionStepHistoryID
        {
            get { return m_nActionStepHistoyID; }
            set { m_nActionStepHistoyID = value; }
        }

        public SOPScenario Scenario
        {
            get { return m_scenario; }
            set
            {
                m_scenario = value;

                if (m_scenario == null)
                {
                    m_nDisasterID = m_nActionStepHistoyID = -1;
                }
                else
                {
                    m_nDisasterID = m_scenario.DisasterID;
                    m_nActionStepHistoyID = m_scenario.ActionStepHistoryID;
                }
            }
        }

        public void SetTabPage(SectionTabPage tabPage, int nIndex)
        {
            m_tabPages[nIndex] = tabPage;
        }

        public SectionTabPage GetTabPage(int nIndex)
        {
            if (nIndex < 0 || nIndex >= ActionStepCount)
                return null;

            return m_tabPages[nIndex];
        }

        public SectionTabPage FindTabPage(int nActionStepID)
        {
            foreach (SectionTabPage page in m_tabPages)
            {
                if (page != null && page.ActionStepID == nActionStepID)
                    return page;
            }

            return null;
        }

        public SectionTabPage FindTabPage(int nActionStepID, bool realMode)
        {
            foreach (SectionTabPage page in m_tabPages)
            {
                if (page != null && page.ActionStepID == nActionStepID && page.VirtualMode == !realMode)
                    return page;
            }

            return null;
        }

        public SectionTabPage FindTabPageFromHistory(int nActionStepHistoryID)
        {
            foreach (SectionTabPage page in m_tabPages)
            {
                if (page != null && page.ActionStepHistoryID == nActionStepHistoryID)
                    return page;
            }

            return null;
        }

        public SectionTabPage[] GetTabPages()
        {
            return m_tabPages;
        }

        public void SetScenario(SOPScenario scenario, int nIndex)
        {
            m_scenarios[nIndex] = scenario;
        }

        public SOPScenario GetScenario(int nIndex)
        {
            if (nIndex < 0 || nIndex >= ActionStepCount)
                return null;

            return m_scenarios[nIndex];
        }

        public void SetWorkFlow(WorkFlow work, int nIndex)
        {
            m_works[nIndex] = work;
        }

        public WorkFlow GetWorkFlow(int nIndex)
        {
            if (nIndex < 0 || nIndex >= ActionStepCount)
                return null;

            return m_works[nIndex];
        }

        public int IndexOf(SectionTabPage tabPage)
        {
            for (int i=0;i<ActionStepCount;i++)
            {
                if (m_tabPages[i] != null && m_tabPages[i] == tabPage)
                    return i;
            }

            return -1;
        }

        public int IndexOf(SOPScenario scenario)
        {
            for (int i = 0; i < ActionStepCount; i++)
            {
                if (m_scenarios[i] != null && m_scenarios[i] == scenario)
                    return i;
            }

            return -1;
        }

        public int CompareTo(object obj)
        {
            SOPScenarioButton btn = (SOPScenarioButton)obj;

            if (this.ActionStepHistoryID > btn.ActionStepHistoryID)
                return 1;
            else if (this.ActionStepHistoryID < btn.ActionStepHistoryID)
                return -1;
            //else
            return 0;
        }
    }
}
