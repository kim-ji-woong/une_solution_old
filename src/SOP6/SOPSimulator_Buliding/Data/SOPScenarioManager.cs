using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;

//libSection
using Sections;
//libSOP
using UnE.SOP;
using UnE.SOP.Tree;
using UnE.SOP.History;
using UnE.SOP.Log;
using UnE.SOP.Data;
using UnE.SOP.Sections;
using UnE.SOP.Process;
using UnE.SOP.Workstate;
using DBUtility2;
using System.Collections.Concurrent;


namespace SOPMonitoringSystem
{
	public class SOPScenarioManager : ISOPPageContainer, ISOPScenarioManager
	{
        private class ActionStepHistoryLoadingData
        {
            public string strActionStepIDs = "";
            public ArrayList HistoryIDs = new ArrayList();
            public ArrayList ActionStepIDs = new ArrayList();
            public ArrayList BeginTimes = new ArrayList();
            public ArrayList DetectTimes = new ArrayList();
            public ArrayList Disasters = new ArrayList();
            public ArrayList ActionStepHistories = new ArrayList();
            public ArrayList SensorZoneHistories = new ArrayList();


            // Key : ActionStepID
            public Dictionary<int, DisasterInfo> ActionStepDisasters = new Dictionary<int,DisasterInfo>();

            // RealMode, Regular, Normal(0),
            // RealMode, Regular, Not Normal(1),
            // RealMode, Not Regular, Normal(2),
            // RealMode, Not Regular, Not Normal(3),
            // VirtualMode, Regular, Normal(4),
            // VirtualMode, Regular, Not Normal(5),
            // VirtualMode, Not Regular, Normal(6),
            // VirtualMode, Not Regular, Not Normal(7)
            public static int GetIndex(bool isRealMode, bool isRegular, bool isNormal)
            {
                if (isRealMode)
                {
                    if (isRegular)
                    {
                        if (isNormal)
                            return 0;
                        else
                            return 1;
                    }
                    else
                    {
                        if (isNormal)
                            return 2;
                        else
                            return 3;
                    }
                }
                else
                {
                    if (isRegular)
                    {
                        if (isNormal)
                            return 4;
                        else
                            return 5;
                    }
                    else
                    {
                        if (isNormal)
                            return 6;
                        else
                            return 7;
                    }
                }
            }

            // RealMode, Regular, Normal(0),
            // RealMode, Regular, Not Normal(1),
            // RealMode, Not Regular, Normal(2),
            // RealMode, Not Regular, Not Normal(3),
            // VirtualMode, Regular, Normal(4),
            // VirtualMode, Regular, Not Normal(5),
            // VirtualMode, Not Regular, Normal(6),
            // VirtualMode, Not Regular, Not Normal(7)
            public static bool GetIndexOption(int nIndex, out bool isRealMode, out bool isRegular, out bool isNormal)
            {
                isRealMode = isRegular = isNormal = true;

                if (nIndex == 0)
                {
                    isRealMode = isRegular = isNormal = true;
                }
                else if (nIndex == 1)
                {
                    isRealMode = isRegular = true;
                    isNormal = false;
                }
                else if (nIndex == 2)
                {
                    isRealMode = true;
                    isRegular = false;
                    isNormal = true;
                }
                else if (nIndex == 3)
                {
                    isRealMode = true;
                    isRegular = false;
                    isNormal = false;
                }
                else if (nIndex == 4)
                {
                    isRealMode = false;
                    isRegular = isNormal = true;
                }
                else if (nIndex == 5)
                {
                    isRealMode = false;
                    isRegular = true;
                    isNormal = false;
                }
                else if (nIndex == 6)
                {
                    isRealMode = false;
                    isRegular = false;
                    isNormal = true;
                }
                else if (nIndex == 7)
                {
                    isRealMode = false;
                    isRegular = false;
                    isNormal = false;
                }
                else
                    return false;

                return true;
            }
        }

        private class ActionStepHistoryEx
        {
            private bool m_isLoading = false;
            private Data_ActionStepHistory m_history = null;
            private DisasterInfo m_disaster = null;
            private int m_nAccessedUserID = -1;
            // SOPWebServer로부터 전달받는 새로운 ComponentHistory List
            // UI에서 처리된 이후에는 삭제되는 임시 데이터.
            private ConcurrentDictionary<int, Data_ComponentHistory> m_dicNewComponentHistory = new ConcurrentDictionary<int, Data_ComponentHistory>();

            public bool IsLoading
            {
                get { return m_isLoading; }
                set { m_isLoading = value; }
            }

            public Data_ActionStepHistory History
            {
                get { return m_history; }
                set { m_history = value; }
            }

            public DisasterInfo Disaster
            {
                get { return m_disaster; }
                set { m_disaster = value; }
            }

            // 제어권을 가진 User ID
            public int AccessedUserID
            {
                get { return m_nAccessedUserID; }
                set { m_nAccessedUserID = value; }
            }

            // SOPWebServer로부터 전달받는 새로운 ComponentHistory List
            // UI에서 처리된 이후에는 삭제되는 임시 데이터.
            public ConcurrentDictionary<int, Data_ComponentHistory> NewComponentHistories
            {
                get { return m_dicNewComponentHistory; }
            }

            public ActionStepHistoryEx(Data_ActionStepHistory history)
            {
                m_history = history;
            }
        }

		private BarLevelTree m_barTree = null;
		private BarPage m_barPage = null;
		
		//private bool m_isAllStop = false;

		private bool m_isLoadComponentHistory = false;

		public bool FinishLoadingComponentHistory
		{
			get { return m_isLoadComponentHistory; }
		}

		private ArrayList m_arrLoadHistory = new ArrayList();
		public ArrayList ArrLoadHistory
		{
			get { return m_arrLoadHistory; }
			set { m_arrLoadHistory = value; }
		}
		
		private static SOPScenarioManager m_instance = null;
		public static SOPScenarioManager Instance
		{
			get
			{
				if (m_instance == null)
					m_instance = new SOPScenarioManager();
				return m_instance; 
			}            
		}


		// 초기 로딩시 DB에 저장된 History를 불러와서 SOP Log 창에 생성된 Log의 개수
		private int m_nInitHistoryLogCount = 0;
        private int m_nLastComponentHistoryID = -1;

        private ArrayList m_arrHistory = new ArrayList();
		public ArrayList ArrHistory
		{
			get { return m_arrHistory; }
			set { m_arrHistory = value; }
		}

		private ArrayList m_arrScenario = new ArrayList();
		public ArrayList GetAllScenario()
		{
			return m_arrScenario;

		}
		public int ScenarioCount
		{
			get { return m_arrScenario.Count; }
		}

		private SOPScenario m_scCurrent = null;
		public SOPScenario CurrentScenario
		{
			get { return m_scCurrent; }
			set { m_scCurrent = value; }
		}

        public int LastComponentHistoryID
        {
            get { return m_nLastComponentHistoryID; }
        }

        private int m_nSiteID = 1;

        private ConcurrentDictionary<int, ActionStepHistoryEx> m_dicActionStepHistory = new ConcurrentDictionary<int, ActionStepHistoryEx>();

		public SOPScenarioManager()
		{
            m_nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;

			m_barPage = new BarPage();
		}

        public void RemoveActionStepHistoryByUser(int nActionStepID, bool isRealMode)
        {
            bool bFindScenario = false;
            SOPScenario deleteSOP = null;
            int nRowCount = m_arrScenario.Count;
            for (int i = 0; i < nRowCount; i++)
            {
                SOPScenario sop = (SOPScenario)m_arrScenario[i];
                if (sop.RealMode != isRealMode)
                    continue;
                if (sop.ActionStepID == nActionStepID)
                {
                    // 쓰레드에서 호출된다.
                    lock (m_arrLoadHistory)
                    {
                        // 원래의 ID를 제거한다.
                        if (m_arrLoadHistory.Contains(sop.ActionStepHistoryID))
                        {
                            m_arrLoadHistory.Remove(sop.ActionStepHistoryID);
                        }

                        bFindScenario = true;
                        deleteSOP = sop;
                    }
                    break;
                }
            }

            if (bFindScenario == true && deleteSOP != null)
            {
                //m_arrScenario.Remove(deleteSOP);
                FormSOP.Instance.GetPageHome().RemoveScenario(deleteSOP);
            }

            //TabPageManager.Instance.Remo//vePage(nActionStepID, isRealMode);
            DeleteScenario(nActionStepID, isRealMode);
        }

        public bool ExistSOPScenario(int nActionStepID, bool isRealMode)
        {
            bool bFindScenario = false;
            
            int nRowCount = m_arrScenario.Count;
            for (int i = 0; i < nRowCount; i++)
            {
                if (i >= m_arrScenario.Count)
                    break;

                SOPScenario sop = (SOPScenario)m_arrScenario[i];
                if (sop.RealMode != isRealMode)
                    continue;
                if (sop.ActionStepID == nActionStepID)
                { 
                    bFindScenario = true;
                    break;
                }
            }
            return bFindScenario;
        }

        public void RemoveActionStepHistory(int nActionStepID, bool isRealMode, bool bCloseTab = true)
        {
            bool bFindScenario = false;
             SOPScenario deleteSOP = null;
            int nRowCount = m_arrScenario.Count;
            for (int i = 0; i < nRowCount; i++)
            {
                SOPScenario sop = (SOPScenario)m_arrScenario[i];
                if (sop.RealMode != isRealMode)
                    continue;
                if (sop.ActionStepID == nActionStepID)
                {
                    // 쓰레드에서 호출된다.
                    lock (m_arrLoadHistory)
                    {
                        // 원래의 ID를 제거한다.
                        if (m_arrLoadHistory.Contains(sop.ActionStepHistoryID))
                        {
                            m_arrLoadHistory.Remove(sop.ActionStepHistoryID);
                        }

                        bFindScenario = true;
                        deleteSOP = sop;
                    }
                    break;
                }
            }

            if(bFindScenario == true && deleteSOP != null)
            {
                //m_arrScenario.Remove(deleteSOP);
                FormSOP.Instance.GetPageHome().RemoveScenario(deleteSOP, bCloseTab);
            }

            if (bCloseTab == true)
            {
                //TabPageManager.Instance.RemovePage(nActionStepID, isRealMode);
                DeleteScenario(nActionStepID, isRealMode);
            }
            else
                RemoveScenario(nActionStepID, isRealMode);

            //if (deleteSOP != null)
            //    FormSOP.Instance.RemoveSOPControl(deleteSOP.ActionStepHistoryID);
        }
        /// <summary>
        /// HistoryID는 HistoryManager에 의해 비동기적으로 받게 되므로 쓰레드에서 업데이트 받는다.
        /// 사용자가 시작한 시나리오의 HistoryID가 0또는 -1일수 있으므로 해당 HistoryID를 업데이트 해준다.
        /// 영향을 미치는 부분은 SOPSenario , SectionTabPage, m_arrLoadHistory 3부분이다.
        /// skkim 2015-09-15
        /// </summary>       
        public void SetActionStepHistoryID(int nActionStepID, bool isRealMode, int nActionStepHistoryID)
        {
            

            int nRowCount = m_arrScenario.Count;
            for (int i = 0; i < nRowCount; i++)
            {
                SOPScenario sop = (SOPScenario)m_arrScenario[i];
                if (sop.RealMode != isRealMode)
                    continue;
                if (sop.ActionStepID == nActionStepID)
                {
                    //if( sop.ActionStepHistoryID <= 0)
                    {
                        // 쓰레드에서 호출된다.
                        lock (m_arrLoadHistory)
                        {
                            // 원래의 ID를 제거한다.
                            if (m_arrLoadHistory.Contains(sop.ActionStepHistoryID))
                            {
                                m_arrLoadHistory.Remove(sop.ActionStepHistoryID);
                            }
                        }

                        // 새로운 ID를 지정
                        sop.ActionStepHistoryID = nActionStepHistoryID;

                        lock (m_arrLoadHistory)
                        {
                            // 새로운 HistoryID를 넣어준다.
                            if (!m_arrLoadHistory.Contains(sop.ActionStepHistoryID))
                            {
                                m_arrLoadHistory.Add(sop.ActionStepHistoryID);
                            }
                        }

                        // 쓰레드에서 호출된다.
                        FormSOP.Instance.Invoke((MethodInvoker)delegate
                        {
                            // SectionTabPage의 HistoryID를 변경한다.
                            SectionTabPage page = (SectionTabPage)TabPageManager.Instance.GetPage(nActionStepID, isRealMode);
                            if( page != null)
                            {
                                page.ActionStepHistoryID = nActionStepHistoryID;

                                if (page.SpecialWorker != null)
                                {
                                    page.SpecialWorker.Work(new PageBackstageSOP.SpecialWork(PageBackstageSOP.SpecialWork.SpecialWorkType.SAVE_USING_UserDefinedTeam, page));
                                    page.SpecialWorker = null;
                                }

                                //FormSOP.Instance.Invoke((MethodInvoker)delegate
                                //{
                                FormSOP.Instance.OnNewActionStepHistory(sop);
                                //});
                            }
                        });
                        
                    }
                }
            }
        }

		public void DeleteOptionChanged(object sender, DeleteOptionChangeEventArgs e)
		{
			int nTargetActionStep = -1;
			ArrayList activeList = new ArrayList();
			activeList.AddRange(TabPageManager.Instance.GetAliveList(true));
			activeList.AddRange(TabPageManager.Instance.GetAliveList(false));
			foreach (int id in activeList)
			{
				nTargetActionStep = id;
				if (nTargetActionStep == -1)
					continue;
				SectionTabPage page = (SectionTabPage)TabPageManager.Instance.GetPage(nTargetActionStep, true);
				if (page != null)
				{
					if (page.State == TabPageState.NOUSE)
					{
						foreach (SOPScenario sop in m_arrScenario)
						{
							int nActionStepID = sop.ActionStepID;
							if (nTargetActionStep == nActionStepID)
							{
								TabPageManager.Instance.RemovePage(nTargetActionStep, true);
								WorkFlowManager.Instance.Remove(nTargetActionStep, true);
							}							
						}
					}
				}

				page = (SectionTabPage)TabPageManager.Instance.GetPage(nTargetActionStep, false);
				if (page != null)
				{
					if (page.State == TabPageState.NOUSE)
					{
						foreach (SOPScenario sop in m_arrScenario)
						{
							int nActionStepID = sop.ActionStepID;
							if (nTargetActionStep == nActionStepID)
							{
								TabPageManager.Instance.RemovePage(nTargetActionStep, false);
								WorkFlowManager.Instance.Remove(nTargetActionStep, false);
							}
						}
					}
				}
			}
		}		

		//////////////////////////////////////////////////////////////////////////
		public BarLevelTree GetBarLevelTree()
		{
			return m_barTree;
		}

        public void CreateLevelTree()
        {
            m_barTree = new BarLevelTree();
            m_barTree.Location = new Point(-500, 0);
            m_barTree.Show();
            m_barTree.Visible = false;
        }
						
		public void DeleteRow(int nActionStepID)
		{
			foreach (SOPScenario sop in m_arrScenario)			
			{
				if (nActionStepID == sop.ActionStepID)
				{
					lock (m_arrLoadHistory)
					{
						int nActionStepHistoryID = sop.ActionStepHistoryID;
						m_arrLoadHistory.Remove(nActionStepHistoryID);						
					}
					break;
				}
			}            
		}

		public int FindRowIndex(int nActionStepID, bool isReal)
		{
			int nRowCount = m_arrScenario.Count;
			for (int i = 0; i < nRowCount; i++)
			{
				SOPScenario sop = (SOPScenario)m_arrScenario[i];
				if (sop.RealMode != isReal)
					continue;
				if (sop.ActionStepID == nActionStepID)
					return i;
			}
			return -1;
		}

		public void SelectRow(int nRowIndex)
		{
			int nRowCount = m_arrScenario.Count;
			if (nRowIndex >= nRowCount)
				return;
			CurrentScenario = (SOPScenario)m_arrScenario[nRowIndex];
			//dataGridScenario.Rows[nRowIndex].Selected = true;
		}

		public SOPScenario AddSOPScenario(string strPath, int nActionStepID, bool bReal, bool isNormal, int nActionStepHistoryID, int nSensorZoneHistoryID, SectionTabPage page)
		{
			bool bRegular = FormSOP.Instance.IsRegular;
			//bool bNormal = FormSOP.Instance.IsNormal;
            return AddSOPScenario(strPath, nActionStepID, bReal, bRegular, isNormal, nActionStepHistoryID, nSensorZoneHistoryID, page);
		}

        public void AddSOPScenario(SOPScenario sopSC, SectionTabPage page)
        {
            lock (m_arrLoadHistory)
            {
                if (!m_arrLoadHistory.Contains(sopSC.ActionStepHistoryID))
                    m_arrLoadHistory.Add(sopSC.ActionStepHistoryID);
            }

            if (m_arrScenario.Count == 0)
            {
                m_arrScenario.Add(sopSC);
                CurrentScenario = sopSC;
                FormSOP.Instance.GetPageHome().AddScenario(sopSC, page);
            }
            else
            {
                List<SOPScenario> removeScenarios = new List<SOPScenario>();

                bool isSame = false;
                foreach (SOPScenario sop in m_arrScenario)
                {
                    if (sop.ActionStepID == sopSC.ActionStepID && sop.RealMode == sopSC.RealMode && sop.ActionStepHistoryID == sopSC.ActionStepHistoryID)
                    {
                        isSame = true;
                        CurrentScenario = sopSC;
                        break;
                    }

                    if (WorkFlowManager.Instance.Get(sop.ActionStepID, sop.RealMode) == null)
                    {
                        // 이미 종료된 시나리오
                        removeScenarios.Add(sop);
                    }
                }

                foreach (SOPScenario sop in removeScenarios)
                {
                    m_arrScenario.Remove(sop);
                }

                if (!isSame)
                {
                    m_arrScenario.Add(sopSC);
                    CurrentScenario = sopSC;
                    FormSOP.Instance.GetPageHome().AddScenario(sopSC, page);
                }
            }
        }
       
        public SOPScenario GetSOPScenario(int nActionStepHistory)
        {
            ArrayList temp = (ArrayList)m_arrScenario.Clone();
            foreach (SOPScenario sop in temp)
            {
                if (sop.ActionStepHistoryID == nActionStepHistory)
                {
                    return sop;                    
                }
            }
            return null;
        }
        

		public SOPScenario AddSOPScenario(string strPath, int nActionStepID, bool bReal, bool bRegular, bool bNormal, int nActionStepHistoryID, int nSensorZoneHistoryID, SectionTabPage page)
		{
			//strPath = strPath.Replace((char)0x06, '/');

			SOPScenario sopSC = new SOPScenario();
			sopSC.ActionStepFullPath = strPath;
			sopSC.ActionStepID = nActionStepID;
			sopSC.ActionStepHistoryID = nActionStepHistoryID;
			sopSC.RealMode = bReal;
			sopSC.NormalMode = bNormal;
			sopSC.RegularMode = bRegular;
            sopSC.SensorZoneHistoryID = nSensorZoneHistoryID;

            ActionStepInfo actionStep = FormSOP.Instance.SOPManager.GetActionStepInfo(nActionStepID);

            if (actionStep != null)
                sopSC.DisasterID = actionStep.DisasterID;

            AddSOPScenario(sopSC, page);
            return sopSC;
		}

		// Return 값 : 삭제된 행의 Index
		//             삭제되지 않을 경우 -1을 리턴
		public int DeleteGridRowScenario(string strPath)
		{
			int nDeletedIndex = -1;
			int nRowCount = m_arrScenario.Count;

			for (int i = 0; i < nRowCount; i++)
			{
				SOPScenario sop = (SOPScenario)m_arrScenario[i];
				if (strPath == sop.ActionStepFullPath)
				{
					
					int nActionStepHistoryID = sop.ActionStepHistoryID;
					lock (m_arrLoadHistory)
					{
						// 해당 ActionStepHistory를 삭제
						m_arrLoadHistory.Remove(nActionStepHistoryID);
					}
					// 해당 SOP시나리오를 삭제
					m_arrScenario.Remove(sop);
					
					// 삭제되는 index를 저장
					nDeletedIndex = i;

					// 선택된 시나리오가 없으면 마지막 시나리오 선택
					if (CurrentScenario == null)
					{                      
						if (m_arrScenario.Count > 0)
						{
							int nIndex = m_arrScenario.Count - 1;
							
							SelectRow(nIndex);
						}                        
					}

					// 선택된 시나리오를 현재 시나리오로 지정
					if (CurrentScenario == null)
					{
						SOPScenario sopCurrent = CurrentScenario;
						//FormSOP.Instance.WriteCurrentActionStepID(sopCurrent.ActionStepID, sopCurrent.RealMode);
					}                    
					break;
				}
			}
			return nDeletedIndex;
		}

		private bool IsVirtualMode(string name)
		{
			if( name == null)
				return false;

			if (name.IndexOf("훈련모드") != -1)
				return true;

			return false;
		}

		private void SetSelectedScenario(SOPScenario sop)
		{
			int nActionStepID = sop.ActionStepID;
			string strValue = sop.ActionStepFullPath;

			VersionInfo ainfo = FormSOP.Instance.SOPManager.GetActionStepVersionInfo(nActionStepID);
			ActionStepInfo info = FormSOP.Instance.SOPManager.GetActionStepInfo(nActionStepID);

			bool isRealMode = sop.RealMode;

			// MainForm의 상태를 변경한다.
			FormSOP.Instance.ChangeMode(ainfo, info, isRealMode);
			FormSOP.Instance.VirtualMode(!isRealMode);
            FormSOP.Instance.EnableOptions(false);

			BarLevelTree tree = GetBarLevelTree();
			if (tree != null)
			{
                if (tree.IsNormal != sop.NormalMode || tree.IsRegular != sop.RegularMode)
                    tree.Load(FormSOP.Instance.SOPManager, sop.RegularMode, sop.NormalMode);

				TreeNode node = tree.FindActionStepNode(nActionStepID);
				if (node != null)
				{
                    CurrentScenario = sop;

					TreeNode selectedNode = tree.GetSelectedNode();
					if (selectedNode != null)
						selectedNode.ForeColor = Color.Black;

					if (PageBackstageSOP.IsWorkingMode(info.ActionStepID, isRealMode))
					{
                        if (FormSOP.Instance.HasSOPControl(sop.ActionStepHistoryID))
						//if (FormSOP.Instance.HasControl == true)
						{
							// 현재 화면에 나타나고 있는 ActionStep을 기록한다.
							//FormSOP.Instance.WriteCurrentActionStepID(nActionStepID, isRealMode);
						}                 
					}

                    PanelSectionEx currentPanel = FormSOP.Instance.GetPageHome().GetCurrentPanel();

                    if (selectedNode != node || currentPanel == null || (currentPanel != null && currentPanel.ActionStepID != sop.ActionStepID))
                    {
                        tree.IgnoreLoadSOP = true;
                        tree.SelectNode(node, true);
                        tree.IgnoreLoadSOP = false;

                        if (NeedSelectSOP(selectedNode, node, sop.RealMode))
                        //if (selectedNode == null || selectedNode.Parent != node.Parent)
                            tree.SelectSop(node);

                        tree.IgnoreSelect = false;
                    }
				}
			}		
		}

        private bool NeedSelectSOP(TreeNode selectedNode, TreeNode node, bool isRealMode)
        {
            if (selectedNode == null || selectedNode.Parent != node.Parent)
                return true;

            if (node.Parent.Nodes.Count == 1)
                return false;

            foreach (TreeNode child in node.Parent.Nodes)
            {
                int nActionStepID = (int)child.Tag;

                if (FormSOP.Instance.GetPageHome().GetTabPage(nActionStepID, isRealMode) == null)
                    return true;
            }

            return false;
        }

		public void SetSelectedScenario()
		{
			int nRow = m_arrScenario.Count;
			if (nRow != 0)
			{
				int nSelectRow = nRow - 1;
				SOPScenario sop = (SOPScenario)m_arrScenario[nSelectRow];
				SetSelectedScenario(sop);
			}
		}

		
		/// <summary>
		/// 해당 ActionStepID와 모드에 맞는 시나리오를 현재 시나리오로 선택
		/// </summary>
		/// <param name="nActionStepID">ActionStepID</param>
		/// <param name="isRealMode">훈련모드/실제모드</param>
		public void SelectedScenario(int nActionStepID, bool isRealMode)
		{
            //if (FormSOP.Instance.HasControl == true)
			
			foreach (SOPScenario sop in m_arrScenario)
			{
				//row.Selected = false;
				if (nActionStepID == sop.ActionStepID && isRealMode == sop.RealMode)
				{
                    //if (CurrentScenario != sop)
                    {
                        CurrentScenario = sop;
                        // PanelSection 변경
                        SetSelectedScenario(sop);

                        // ComponentContents 변경
                        FormSOP.Instance.GetPageHome().SelectScenario(sop);

                        break;
                    }					
				}
			}
		}

        public void SelectedScenario(SOPScenario scenario, SectionTabPage tabPage)
        {
            foreach (SOPScenario sop in m_arrScenario)
            {
                if (scenario == sop)
                {
                    CurrentScenario = sop;
                    // PanelSection 변경
                    SetSelectedScenario(sop);

                    // ComponentContents 변경
                    FormSOP.Instance.GetPageHome().SelectScenario(sop);

                    break;
                }
            }
        }

		// isRegular : 등록된 버전인가?
		// isNormal : 평일 버전인가?
		// Return 값 : 현재 실행중인 SOP의 FullPath
		//             현재 실행중인 것이 없을 경우 빈 문자열을 리턴
		public SOPScenario GetCurrentSOPScenario()
		{
			return CurrentScenario;
		}

		public string GetCurrentDisasterName()
		{
			if (CurrentScenario == null)
				return null;
			return CurrentScenario.DisasterName;
		}
		
		public void DeleteScenario(int nActionStepID, bool bReal)
		{

			SectionTabPage page = (SectionTabPage)TabPageManager.Instance.GetPage(nActionStepID, bReal);
			
			RemoveTabPage(page);

			AfterRemoveTabPage(page); 

		}

        // DeleteScenario와의 차이점은 단지 m_arrScenario에서만 삭제하는 것이다.
        // TabPage는 그대로 놔둔다.
        public void RemoveScenario(int nActionStepID, bool isReal)
        {
            foreach (SOPScenario sop in m_arrScenario)
            {
                if (sop.ActionStepID == nActionStepID && sop.RealMode == isReal)
                {
                    m_arrScenario.Remove(sop);
                    break;
                }
            }
        }

        // DeleteScenario와의 차이점은 단지 m_arrScenario에서만 삭제하는 것이다.
        // TabPage는 그대로 놔둔다.
        public void RemoveScenario(int nActionStepHistoryID)
        {
            foreach (SOPScenario sop in m_arrScenario)
            {
                if (sop.ActionStepHistoryID == nActionStepHistoryID)
                {
                    m_arrScenario.Remove(sop);
                    break;
                }
            }
        }

        public void RemoveTabPage(SectionTabPage page, bool bRemoveOnly)
        {
            FormSOP.Instance.GetPageHome().RemoveTabPage(page);
        }

		public bool RemoveTabPage(SectionTabPage page)
		{
			if (page != null)
			{
				int nTargetActionStep = page.ActionStepID;
				bool isTargetRealMode = !page.VirtualMode;

				if (page.State == TabPageState.NOUSE)
				{

					foreach (SOPScenario row in m_arrScenario)
					{
						bool deleterow = false;						
						{
							int nActionStepID = row.ActionStepID;
							if (nTargetActionStep == nActionStepID && isTargetRealMode == row.RealMode)
							{
								deleterow = true;
							}
						}

						if (deleterow == true)
						{
							int nActionStepHistoryID = row.ActionStepHistoryID;
							lock (m_arrLoadHistory)
							{
								m_arrLoadHistory.Remove(nActionStepHistoryID);
							}

                            lock (m_arrScenario)
                            {
                                m_arrScenario.Remove(row);
                            }
                            if (CurrentScenario == row)
                            {
                                CurrentScenario = null;
                            }
                            

							BarLevelTree tree = GetBarLevelTree();
							if (tree != null)
							{
								tree.ResetSelect();
								tree.UnSelectedNode();
							}
							TabPageManager.Instance.RemovePage(nTargetActionStep, isTargetRealMode);
							WorkFlowManager.Instance.Remove(nTargetActionStep, isTargetRealMode);

							FormSOP.Instance.GetPageHome().ClearComponentContents(page, nTargetActionStep, isTargetRealMode);

							FormSOP.Instance.GetPageHome().RemoveTabPage(page);
							FormSOP.Instance.GetPageHome().PanelArray.Clear();

                            FormSOP.Instance.GetPageHome().RemoveScenario(row);
							
							return true;
						}
					}
				}
			}
			return false;
		}

		public void AfterRemoveTabPage(SectionTabPage removedPage)
		{
			if (m_arrScenario.Count == 0)
			{
                FormSOP.Instance.GetPageHome().TabControls.InitTabPages();
                FormSOP.Instance.GetPageHome().SelectTabPage(null);
                //FormSOP.Instance.GetPageHome().TabControls.SelectedTab = null;
                FormSOP.Instance.GetPageHome().panel.Visible = false;
				FormSOP.Instance.GetPageHome().SetBackgroundImage(false);
				FormSOP.Instance.WaitWorkflow();
				FormSOP.Instance.GetPageHome().ClearProcess();
                FormSOP.Instance.GetPageHome().ClearScenario();

				BarLevelTree tree = GetBarLevelTree();
				if (tree != null)
				{
					tree.ResetSelect();
					tree.UnSelectedNode();
				}

			}
			else
			{
                if (removedPage == null)
                    return;

                ActionStepInfo actionStep = FormSOP.Instance.SOPManager.GetActionStepInfo(removedPage.ActionStepID);

                if (actionStep != null)
                {
                    // 지워진 페이지를 공유하는 시나리오가 있으면 DB에서 새로 로딩하도록 한다.
                    foreach (SOPScenario scenario in m_arrScenario)
                    {
                        if (actionStep.DisasterID == scenario.DisasterID)
                        {
                            FormSOP.Instance.GetPageHome().LoadActionStep(actionStep);
                            break;
                        }
                    }
                }

				if (CurrentScenario == null)
				{
					CurrentScenario = (SOPScenario)m_arrScenario[0];
				}

                if (FormSOP.Instance.CheckClosedSOP(CurrentScenario.ActionStepHistoryID))
                {
                    FormSOP.Instance.StopWorkflow(DateTime.Now, true, CurrentScenario.ActionStepID, CurrentScenario.RealMode);
                    return;
                }

				SOPScenario workingRow = null;
				int nActionStepID = CurrentScenario.ActionStepID;
				bool isRealMode = CurrentScenario.RealMode;

				if (PageBackstageSOP.IsWorkingMode(nActionStepID, isRealMode))
				{
					workingRow = CurrentScenario;
				}                

				if( workingRow == null)
				{
					workingRow = CurrentScenario;
				}
				
				if( workingRow != null)
				{
					nActionStepID = workingRow.ActionStepID;
					isRealMode = workingRow.RealMode;

					VersionInfo ainfo = FormSOP.Instance.SOPManager.GetActionStepVersionInfo(nActionStepID);
					ActionStepInfo info = FormSOP.Instance.SOPManager.GetActionStepInfo(nActionStepID);

					FormSOP.Instance.ChangeMode(ainfo, info, isRealMode);
					FormSOP.Instance.VirtualMode(!isRealMode);
                    //FormSOP.Instance.EnableOptions(false);

					BarLevelTree tree = GetBarLevelTree();
					if (tree != null)
					{
                        TreeNode node = tree.FindActionStepNode(nActionStepID);                            
						if (node != null)
						{
							TreeNode selectedNode = tree.GetSelectedNode();
							if (selectedNode != null)
								selectedNode.ForeColor = Color.Black;

                            //if (node != selectedNode)
                            {
                                tree.IgnoreLoadSOP = true;
                                tree.SelectNode(node);
                                tree.IgnoreLoadSOP = false;
                                tree.SelectSop(node);
                                tree.IgnoreSelect = false;
                                node.ForeColor = Color.Red;
                            }

							//FormSOP.Instance.WriteCurrentActionStepID(nActionStepID, isRealMode);
                            FormSOP.Instance.GetPageHome().SelectScenario(workingRow);
						}
					}
				}
			}
		}

		private string GetActionStepPath(ArrayList arrActionSteps, int nActionStepID)
		{
			string strPath = "";

			foreach (ActionStepInfo actionStep in arrActionSteps)
			{
				if (actionStep.ActionStepID == nActionStepID)
				{
					strPath = actionStep.ActionStepName;

					if (actionStep.ParentStepID > 0)
					{
						string strParentPath = GetActionStepPath(arrActionSteps, actionStep.ParentStepID);
						if (strParentPath.Length > 0)
							strPath = strParentPath + '/' + strPath;
					}

					return strPath;
				}
			}
			return strPath;
		}
        
        private int LoadActionStepHistoryOption(WebDBManager dbMgr, int nActionStepHistoryID)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Select ash.StartOption from ActionStepHistory as ash ");
            sb.Append(" INNER JOIN ActionStep as step on step.ID = ash.ActionStepID and ash.EndTime is null and CancelTime is null");
            sb.Append(" INNER JOIN Disaster as dis on step.DisasterID = dis.ID ");
            sb.Append(" INNER JOIN SubDisasterCategory as sdc on dis.SubDisasterID = sdc.ID ");
            sb.AppendFormat(" INNER JOIN DisasterCategory as dc on dc.ID = sdc.DisasterID AND dc.SiteID = {0} ", m_nSiteID);
            sb.AppendFormat(" WHERE ash.ID = {0}", nActionStepHistoryID);

            string strSQL = sb.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            int nStartOption = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            return nStartOption;
        }

        // 실행중인 ActionStepHistory 목록을 얻어온다.
        // Key : 양수이면 실제모드의 ActionStepID
        //       음수이면 훈련모드의 ActionStepID
        /*private bool LoadActionStepHistory(WebDBManager dbMgr, Dictionary<int, Data_ActionStepHistory> dicActionStepHistories)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append("Select ash.ID, ash.ActionStepID, ash.RealMode, ash.BeginTime, ash.DetectTime, ash.SelectedComponentID, ash.SelectedComponentType, ash.StartOption, ash.Description, ash.DisasterOption, ash.SensorZoneHistoryID from ActionStepHistory as ash ");
            sb.Append(" INNER JOIN ActionStep as step on step.ID = ash.ActionStepID and ash.EndTime is null and CancelTime is null");
            sb.Append(" INNER JOIN Disaster as dis on step.DisasterID = dis.ID ");
            sb.Append(" INNER JOIN SubDisasterCategory as sdc on dis.SubDisasterID = sdc.ID ");
            sb.AppendFormat(" INNER JOIN DisasterCategory as dc on dc.ID = sdc.DisasterID AND dc.SiteID = {0} ", m_nSiteID);
            sb.Append(" ORDER BY ash.ID DESC");

            string strSQL = sb.ToString();

			//string strSQL = "select id, ActionStepID, RealMode, BeginTime, DetectTime from ActionStepHistory where EndTime is null and CancelTime is null";
			ArrayList arrResult = dbMgr.GetResultData(strSQL);

			if (arrResult == null)
				return false;

			int nResultCount = arrResult.Count;
			DateTime dtDefault = new DateTime();

			for (int i = 0; i < nResultCount - 10; i += 11)
			{
				int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
				int nActionStepID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
				bool isRealMode = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0) == 0 ? false : true;
				DateTime dtBegin = WebDBManager.GetDateTimeField(arrResult[i + 3], dtDefault);
				DateTime dtDetect = WebDBManager.GetDateTimeField(arrResult[i + 4], dtDefault);
                int nSelectedComponentID = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                int nSelectedComponentType = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                int nStartOption = WebDBManager.GetIntField(arrResult[i + 7].ToString(), -1);
                string strDescription = WebDBManager.GetStringField(arrResult[i + 8]);
                string strDisasterOption = WebDBManager.GetStringField(arrResult[i + 9]);
                int nSensorZoneHistoryID = WebDBManager.GetIntField(arrResult[i + 10].ToString(), -1);

				if (nID < 0 || nActionStepID < 0)
					continue;

				if (!isRealMode)
					nActionStepID = -nActionStepID;

                HistoryDisasterNoPosition info = new HistoryDisasterNoPosition();
                info.HistoryActionStepID = nID;
                info.AmountSnowfall = HistoryManager.ParseAmountSnowfall(strDescription);

                if (strDisasterOption != null)
                    info.DisasterOptions = strDisasterOption;

				if (dicActionStepHistories.ContainsKey(nActionStepID))
				{
					Data_ActionStepHistory history = dicActionStepHistories[nActionStepID];

					if (history.BeginTime < dtBegin)
					{
						history.ID = nID;
						history.ActionStepID = System.Math.Abs(nActionStepID);
						history.BeginTime = dtBegin;
						history.DetectTime = dtDetect;
						history.RealMode = isRealMode;
                        history.SelectedSectionID = nSelectedComponentID;
                        history.SelectedSectionType = nSelectedComponentType;
                        history.StartOption = nStartOption;
                        // add by skkim 2018-01-02. Relation SensorZoneHistory
                        history.SensorZoneHistoryID = nSensorZoneHistoryID;
					}

                    history.HistoryDisasterNoPositionInfo = info;
				}
				else
				{
					Data_ActionStepHistory history = new Data_ActionStepHistory();

					history.ID = nID;
					history.ActionStepID = System.Math.Abs(nActionStepID);
					history.BeginTime = dtBegin;
					history.DetectTime = dtDetect;
					history.RealMode = isRealMode;
                    history.SelectedSectionID = nSelectedComponentID;
                    history.SelectedSectionType = nSelectedComponentType;
                    history.StartOption = nStartOption;
                    history.HistoryDisasterNoPositionInfo = info;
                    // add by skkim 2018-01-02. Relation SensorZoneHistory
                    history.SensorZoneHistoryID = nSensorZoneHistoryID;

					dicActionStepHistories[nActionStepID] = history;
				}
			}
			return true;
		}*/
        
        // Key : ActionStepHistory ID
        // Value : Disaster ID
        private Dictionary<int, int> GetActionStepHistoryInfo(WebDBManager dbMgr, Dictionary<int, Data_ActionStepHistory> dicActionStepHistories)
        {
            Dictionary<int, int> dicActionStepHistoryInfos = new Dictionary<int, int>();

            string strActionStepHistoryIDs = "";

            foreach (KeyValuePair<int, Data_ActionStepHistory> pair in dicActionStepHistories)
            {
                if (strActionStepHistoryIDs.Length == 0)
                    strActionStepHistoryIDs = pair.Value.ID.ToString();
                else
                    strActionStepHistoryIDs += ", " + pair.Value.ID.ToString();
            }

            if (strActionStepHistoryIDs.Length == 0)
                return dicActionStepHistoryInfos;

            string strSQL = "select d.ID, ash.ID from Disaster as d, ActionStep as _as, ActionStepHistory as ash where _as.DisasterID = d.ID and ash.ActionStepID = _as.ID and ash.ID in (" + strActionStepHistoryIDs + ")";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                VariousData<int> disasterID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> actionStepHistoryID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                
                if (disasterID == null || actionStepHistoryID == null)
                    continue;

                dicActionStepHistoryInfos[actionStepHistoryID.Data] = disasterID.Data;
            }

            return dicActionStepHistoryInfos;
        }

        private bool LoadHistory(WebDBManager dbMgr, Dictionary<int, Data_ActionStepHistory> dicActionStepHistories)
        {
            Dictionary<int, int> dicActionStepHistoryInfos = GetActionStepHistoryInfo(dbMgr, dicActionStepHistories);

            if (dicActionStepHistories == null)
                return false;

            if (dicActionStepHistories.Count == 0)
                return true;

            bool isNormal, isRegular;
            ActionStepHistoryLoadingData[] arrDatas = new ActionStepHistoryLoadingData[8]
            {
                new ActionStepHistoryLoadingData(), new ActionStepHistoryLoadingData(), new ActionStepHistoryLoadingData(), new ActionStepHistoryLoadingData(),
                new ActionStepHistoryLoadingData(), new ActionStepHistoryLoadingData(), new ActionStepHistoryLoadingData(), new ActionStepHistoryLoadingData()
            };

            foreach (KeyValuePair<int, Data_ActionStepHistory> pair in dicActionStepHistories)
            {
                int nDisasterID;

                if (!dicActionStepHistoryInfos.TryGetValue(pair.Value.ID, out nDisasterID))
                    continue;

                DisasterInfo disaster = FormSOP.Instance.SOPManager.GetDisaster(nDisasterID, out isNormal, out isRegular);

                if (disaster == null)
                    continue;

                Data_ActionStepHistory actionStepHistory = pair.Value;
                bool isRealMode = actionStepHistory.RealMode;
                int nSensorZoneHistoryID = actionStepHistory.SensorZoneHistoryID;

                int nIndex = ActionStepHistoryLoadingData.GetIndex(isRealMode, isRegular, isNormal);
                arrDatas[nIndex].ActionStepHistories.Add(actionStepHistory);
                arrDatas[nIndex].ActionStepDisasters[actionStepHistory.ActionStepID] = disaster;
                arrDatas[nIndex].SensorZoneHistories.Add(nSensorZoneHistoryID);

                ArrayList arrResult = new ArrayList();

                arrResult.Add(actionStepHistory.ID);
                arrResult.Add(actionStepHistory.ActionStepID);
                arrResult.Add(actionStepHistory.BeginTime);
                arrResult.Add(actionStepHistory.SensorZoneHistoryID);
                
                m_arrHistory.Add(arrResult);

                foreach (ActionStepInfo ainfo in disaster.ActionSteps)
                {
                    if (ainfo.ActionStepID != actionStepHistory.ActionStepID)
                        continue;

                    if (arrDatas[nIndex].strActionStepIDs.Length == 0)
                        arrDatas[nIndex].strActionStepIDs = ainfo.ActionStepID.ToString();
                    else
                        arrDatas[nIndex].strActionStepIDs += ", " + ainfo.ActionStepID.ToString();

                    arrDatas[nIndex].ActionStepIDs.Add(ainfo.ActionStepID);

                    arrDatas[nIndex].Disasters.Add(disaster);
                   

                    if (ainfo.ActionStepID == actionStepHistory.ActionStepID)
                    {
                        arrDatas[nIndex].HistoryIDs.Add(actionStepHistory.ID);
                        arrDatas[nIndex].BeginTimes.Add(actionStepHistory.BeginTime);
                        arrDatas[nIndex].DetectTimes.Add(actionStepHistory.DetectTime);
                    }
                    else
                    {
                        arrDatas[nIndex].HistoryIDs.Add(0);
                        arrDatas[nIndex].BeginTimes.Add(new DateTime());
                        arrDatas[nIndex].DetectTimes.Add(new DateTime());
                    }
                }

                HistoryManager2.Instance.AddHistoryDisasterPosition(actionStepHistory.ID, actionStepHistory.ActionStepID, isRealMode);
                HistoryManager2.Instance.AddHistoryDisasterNoPosition(actionStepHistory.ActionStepID, isRealMode, actionStepHistory.HistoryDisasterNoPositionInfo);
            }

            Dictionary<DisasterInfo, string> dicDisasterFullPath = FormSOP.Instance.SOPManager.GetFullPathDictionary();

            for (int j = 0; j < 8; j++)
            {
                ActionStepHistoryLoadingData data = arrDatas[j];
                bool isRealMode;

                if (!ActionStepHistoryLoadingData.GetIndexOption(j, out isRealMode, out isRegular, out isNormal))
                    continue;

                if (!_LoadActionStepPanel(dbMgr, data.ActionStepHistories, data.strActionStepIDs, data.HistoryIDs, data.ActionStepIDs, data.BeginTimes, data.DetectTimes, data.Disasters, data.SensorZoneHistories, isRealMode, data.ActionStepDisasters, dicDisasterFullPath, isRegular, isNormal))
                    return false;

                int nActionStepCount =data.ActionStepIDs.Count;

                for (int i = 0; i < nActionStepCount; i++)
                {
                    FormSOP.Instance.SOPManager.SetActionStepHistoryID((int)data.ActionStepIDs[i], isRealMode, (int)data.HistoryIDs[i]);
                }
            }

            return true;
        }

       

		// 기존의 LoadHistory(...)가 너무 많은 SubQuery로 인하여 DB 부하가 많은 관계로 SubQuery를 사용하지 않고 
		// dicActionStepHistories를 사용하는 버전으로 변경
		private bool LoadHistory(WebDBManager dbMgr, Dictionary<string, DisasterInfo> dicData, Dictionary<int, Data_ActionStepHistory> dicActionStepHistories, bool isRealMode, bool isRegular, bool isNormal)
		{
			// ActionStep ID, Disaster
			Dictionary<int, DisasterInfo> dicDisaster = new Dictionary<int, DisasterInfo>();
			// Disaster, Disaster Full Path
			Dictionary<DisasterInfo, string> dicDisasterFullPath = new Dictionary<DisasterInfo, string>();
			ArrayList arrActionStepHistories = new ArrayList();
			ArrayList arrResult = new ArrayList();

			foreach (KeyValuePair<string, DisasterInfo> pair in dicData)
			{
				DisasterInfo disaster = pair.Value;
				dicDisasterFullPath[disaster] = pair.Key;

				foreach (ActionStepInfo actionStep in disaster.ActionSteps)
				{
					dicDisaster[actionStep.ActionStepID] = disaster;

					int nKey = isRealMode ? actionStep.ActionStepID : -actionStep.ActionStepID;

					if (dicActionStepHistories.ContainsKey(nKey))
					{
						Data_ActionStepHistory history = dicActionStepHistories[nKey];
						arrActionStepHistories.Add(history);

						arrResult.Add(history.ID);
						arrResult.Add(history.ActionStepID);
						arrResult.Add(history.BeginTime);
					}
				}
			}

			if (arrActionStepHistories.Count == 0)
				return true;

			m_arrHistory.Add(arrResult);

			string strActionStepIDs = "";
			ArrayList arrHistoryID = new ArrayList();
			ArrayList arrActionStepID = new ArrayList();
			ArrayList arrBeginTime = new ArrayList();
			ArrayList arrDetectTime = new ArrayList();
			ArrayList arrDisaster = new ArrayList();
            ArrayList arrSensorZoneHistories = new ArrayList();

			foreach (Data_ActionStepHistory history in arrActionStepHistories)
			{
				if (!dicDisaster.ContainsKey(history.ActionStepID))
					continue;

				DisasterInfo disaster = dicDisaster[history.ActionStepID];

				if (!dicDisasterFullPath.ContainsKey(disaster))
					continue;			

                foreach(ActionStepInfo ainfo in disaster.ActionSteps)
                {
                    if (strActionStepIDs.Length == 0)
                        strActionStepIDs = ainfo.ActionStepID.ToString();
                    else
                        strActionStepIDs += ", " + ainfo.ActionStepID.ToString();

                    arrActionStepID.Add(ainfo.ActionStepID);

                    arrDisaster.Add(disaster);
                   
                    if (ainfo.ActionStepID == history.ActionStepID)
                    {
                        arrHistoryID.Add(history.ID);
                        arrBeginTime.Add(history.BeginTime);
                        arrDetectTime.Add(history.DetectTime);
                        arrSensorZoneHistories.Add(history.SensorZoneHistoryID);

                    }
                    else
                    {
                        arrHistoryID.Add(0);
                        arrBeginTime.Add(new DateTime());
                        arrDetectTime.Add(new DateTime());
                        arrSensorZoneHistories.Add(-1);
                    }
                }

				HistoryManager2.Instance.AddHistoryDisasterPosition(history.ID, history.ActionStepID, isRealMode);
                HistoryManager2.Instance.AddHistoryDisasterNoPosition(history.ActionStepID, isRealMode, history.HistoryDisasterNoPositionInfo);
			}

            if (!_LoadActionStepPanel(dbMgr, arrActionStepHistories, strActionStepIDs, arrHistoryID, arrActionStepID, arrBeginTime, arrDetectTime, arrDisaster, arrSensorZoneHistories,isRealMode, dicDisaster, dicDisasterFullPath, isRegular, isNormal))
				return false;

			int nActionStepCount = arrActionStepID.Count;

			for (int i = 0; i < nActionStepCount; i++)
			{
				FormSOP.Instance.SOPManager.SetActionStepHistoryID((int)arrActionStepID[i], isRealMode, (int)arrHistoryID[i]);
			}

			return true;
		}

		public ArrayList GetRunActionStepHistory()
		{
			ArrayList arrHistory = ArrHistory;
			foreach (ArrayList arr in arrHistory)
			{
				if (arr.Count > 0)
					return arr;
			}
			return null;
		}

		private ArrayList FindStepMemberList(int nActionStepID, Dictionary<ActionStepInfo, ArrayList> dicStepMember)
		{
			foreach (KeyValuePair<ActionStepInfo, ArrayList> pair in dicStepMember)
			{
				if (pair.Key.ActionStepID == nActionStepID)
					return pair.Value;
			}

			return null;
		}

		// dicStepMember : ActionStepInfo, StepMemberData List
		private bool LoadActionSteps(WebDBManager dbMgr, string strSQL, ArrayList arrActionStepID, ArrayList arrDisaster, Dictionary<ActionStepInfo, ArrayList> dicStepMember, bool isNormal)
		{
			ArrayList arrResult = dbMgr.GetResultData(strSQL);
			if (arrResult == null) return false;

			int nResultCount = arrResult.Count;

			int nPrevActionStepID = -2;
			int nIndex = -1;

			ArrayList arrStepMember = null;

			for (int i = 0; i < nResultCount - 4; i += 5)
			{
				int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
				int nTeamID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
				string strTeamName = WebDBManager.GetStringField(arrResult[i + 2].ToString(), "");
                int nLevelNo = -1;
				//int nLevelNo = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
				int nTeamType = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
				int nActionStepID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);

                List<TemporaryMember> members = FormSOP.Instance.SOPManager.GetTemporaryMembers(nTeamID, isNormal);

                if (members != null)
                {
                    foreach (TemporaryMember member in members)
                    {
                        if (member._MemberType == TemporaryMember.MemberType.JobLevel)
                        {
                            nLevelNo = member.MemberID;
                            break;
                        }
                    }
                }

				if (nActionStepID != nPrevActionStepID)
				{
					arrStepMember = FindStepMemberList(nActionStepID, dicStepMember);

					if (arrStepMember == null)
					{
						nIndex = arrActionStepID.IndexOf(nActionStepID);
						if (nIndex < 0)
							continue;

						DisasterInfo disaster = (DisasterInfo)arrDisaster[nIndex];
						ActionStepInfo actionStep = disaster.FindActionStep(nActionStepID);
						if (actionStep == null)
							continue;

						arrStepMember = new ArrayList();
						dicStepMember[actionStep] = arrStepMember;
					}

					nPrevActionStepID = nActionStepID;
				}

				StepMemberData data = new StepMemberData(strTeamName, nTeamID, nTeamType, nID, nLevelNo);
				arrStepMember.Add(data);
			}
			return true;
		}

        // dicStepMember : ActionStepInfo, StepMemberData List
        private bool LoadActionSteps(WebDBManager dbMgr, string strSQL, ArrayList arrActionStepID, ArrayList arrDisaster, Dictionary<ActionStepInfo, ArrayList> dicStepMember)
        {
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            int nResultCount = arrResult.Count;

            int nPrevActionStepID = -2;
            int nIndex = -1;

            ArrayList arrStepMember = null;

            for (int i = 0; i < nResultCount - 4; i += 5)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nTeamID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                string strTeamName = WebDBManager.GetStringField(arrResult[i + 2].ToString(), "");
                int nTeamType = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nActionStepID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);

                if (nActionStepID != nPrevActionStepID)
                {
                    arrStepMember = FindStepMemberList(nActionStepID, dicStepMember);

                    if (arrStepMember == null)
                    {
                        nIndex = arrActionStepID.IndexOf(nActionStepID);
                        if (nIndex < 0)
                            continue;

                        DisasterInfo disaster = (DisasterInfo)arrDisaster[nIndex];
                        ActionStepInfo actionStep = disaster.FindActionStep(nActionStepID);
                        if (actionStep == null)
                            continue;

                        arrStepMember = new ArrayList();
                        dicStepMember[actionStep] = arrStepMember;
                    }

                    nPrevActionStepID = nActionStepID;
                }

                StepMemberData data = new StepMemberData(strTeamName, nTeamID, nTeamType, nID, -1);
                arrStepMember.Add(data);
            }
            return true;
        }

        public bool LoadActionStepPanel(DBUtility2.WebDBManager dbMgr, string strActionstepIDs, ArrayList arrActionStepHistoryID, ArrayList arrActionStepID, ArrayList arrActionStepBeginTime, ArrayList arrActionStepDetectTime, ArrayList arrDisaster, ArrayList arrSensorZoneHistories, bool isRealMode, Dictionary<int, DisasterInfo> dicDisaster, Dictionary<DisasterInfo, string> dicDisasterFullPath, bool isRegular, bool isNormal)
        {
            return _LoadActionStepPanel((SOPMonitoringSystem.WebDBManager)dbMgr, null, strActionstepIDs, arrActionStepHistoryID, arrActionStepID, arrActionStepBeginTime, arrActionStepDetectTime, arrDisaster, arrSensorZoneHistories, isRealMode, dicDisaster, dicDisasterFullPath, isRegular, isNormal);
        }
        
        private bool _LoadActionStepPanel(WebDBManager dbMgrex, ArrayList arrActionStepHistories, string strActionstepIDs, ArrayList arrActionStepHistoryID, ArrayList arrActionStepID, ArrayList arrActionStepBeginTime, ArrayList arrActionStepDetectTime, ArrayList arrDisaster, ArrayList arrSensorZoneHistories, bool isRealMode, Dictionary<int, DisasterInfo> dicDisaster, Dictionary<DisasterInfo, string> dicDisasterFullPath, bool isRegular, bool isNormal)
        {
            if (strActionstepIDs.Length == 0)
                return true;

            SOPMonitoringSystem.WebDBManager dbMgr = (SOPMonitoringSystem.WebDBManager)dbMgrex;
            // ActionStepInfo, StepMemberData List
            Dictionary<ActionStepInfo, ArrayList> dicStepMember = new Dictionary<ActionStepInfo, ArrayList>();

            string strSQL = string.Format("select sm.ID, sm.TeamID, tt.TeamName, sm.TeamType, sm.ActionStepID from StepMember as sm, TemporaryNormalTeam as tt where sm.TeamType = 0 and sm.TeamID = tt.ID and sm.ActionStepID in ({0}) order by ActionStepID",
                strActionstepIDs);
            if (!LoadActionSteps(dbMgr, strSQL, arrActionStepID, arrDisaster, dicStepMember, true))
                return false;

            strSQL = string.Format("select sm.ID, sm.TeamID, tt.TeamName, sm.TeamType, sm.ActionStepID from StepMember as sm, TemporaryEmergencyTeam as tt where sm.TeamType = 1 and sm.TeamID = tt.ID and sm.ActionStepID in ({0}) order by ActionStepID",
                strActionstepIDs);
            if (!LoadActionSteps(dbMgr, strSQL, arrActionStepID, arrDisaster, dicStepMember, false))
                return false;

            strSQL = string.Format("select sm.ID, sm.TeamID, tt.TeamName, sm.TeamType, sm.ActionStepID from StepMember as sm, ExternalTeam as tt where sm.TeamType = 2 and sm.TeamID = tt.ID and sm.ActionStepID in ({0}) order by ActionStepID",
                strActionstepIDs);
            if (!LoadActionSteps(dbMgr, strSQL, arrActionStepID, arrDisaster, dicStepMember))
                return false;

            strSQL = string.Format("select sm.ID, sm.TeamID, tt.TeamName, sm.TeamType, sm.ActionStepID from StepMember as sm, UserDefinedTeam as tt where sm.TeamType = 3 and sm.TeamID = tt.ID and sm.ActionStepID in ({0}) order by ActionStepID",
                strActionstepIDs);
            if (!LoadActionSteps(dbMgr, strSQL, arrActionStepID, arrDisaster, dicStepMember))
                return false;

            strSQL = string.Format("select sm.ID, sm.TeamID, tt.TeamName, sm.TeamType, sm.ActionStepID from StepMember as sm, RegularTeam as tt where sm.TeamType = 4 and sm.TeamID = tt.ID and sm.ActionStepID in ({0}) order by ActionStepID",
                strActionstepIDs);
            if (!LoadActionSteps(dbMgr, strSQL, arrActionStepID, arrDisaster, dicStepMember))
                return false;

            PageBackstageSOP pageHome = FormSOP.Instance.GetPageHome();
            IOManager ioMgr = new IOManager();

            // TeamID, Team Name
            Dictionary<int, string> dicNormal = null;
            Dictionary<int, string> dicEmergency = null;
            Dictionary<int, string> dicUserDefined = null;
            Dictionary<int, Sections.ExternalTeamData> dicExternal = IOManager.ReadExternalTeamList(dbMgr);
            Dictionary<int, string> dicRegular = null;
            Dictionary<int, string> dicControlRoom = null;

            string szPrevDisasterName = "";
            foreach (KeyValuePair<ActionStepInfo, ArrayList> pair in dicStepMember)
            {
                ActionStepInfo actionStep = pair.Key;
                ArrayList arrStepMember = pair.Value;

                // 새로운 Disaster인경우 모든 탭을 초기화한다.
                string szDisasterName = GetDisasterName(actionStep.ActionStepID);
                if (szDisasterName != szPrevDisasterName)
                {
                    /*ArrayList arrTabPages = pageHome.GetTabPage();
                    int nOldTabPageCount = arrTabPages == null ? 0 : arrTabPages.Count;
                    // 기존 탭이 남아 있게 되는데, 불러오기 후 해당 탭들을 삭제한다.
                    for (int i = 0; i < nOldTabPageCount; i++)
                    {
                        SectionTabPage oldTabPage = (SectionTabPage)arrTabPages[0];
                        pageHome.RemoveTabPage(oldTabPage);
                        arrTabPages.RemoveAt(0);
                        oldTabPage.LinkedZoneID = -1;
                        oldTabPage.LinkedZoneName = "";
                        TabPageManager.Instance.RemovePage(oldTabPage, isRealMode);
                    }*/

                }
                szPrevDisasterName = szDisasterName;
                Sections.SectionData.ClearIDList();

                bool addTabPage = pageHome.CurrentDisasterID < 0 || pageHome.CurrentDisasterID == actionStep.DisasterID;

                int nActionStepHistoryID = 0;
                int nIndex = arrActionStepID.IndexOf(actionStep.ActionStepID);
                if (arrActionStepHistoryID.Count <= nIndex)
                {
                    nIndex = -1;
                    nActionStepHistoryID = 0;
                }
                else
                {
                    nActionStepHistoryID = (int)arrActionStepHistoryID[nIndex];
                }

                SectionTabPage tabPage = (SectionTabPage)pageHome.AddTabPage(actionStep.ToData_ActionStep(), nActionStepHistoryID, isRealMode, addTabPage);

                int nSensorZoneHistoryID = 0;
                if (arrSensorZoneHistories.Count <= nIndex)
                {
                    nSensorZoneHistoryID = -1;
                }
                else
                {
                    nSensorZoneHistoryID = (int)arrSensorZoneHistories[nIndex];
                }

                bool bUseSMS = false;
                int nStartOption = LoadActionStepHistoryOption(dbMgr, nActionStepHistoryID);
                bUseSMS = ((nStartOption & 1) == 1 ? true : false);


                if (nIndex >= 0 && nActionStepHistoryID > 0)
                {
                    // ActionStep 시작 정보를 Log 창에 표시
                    HistoryActionStepData data = HistoryManager2.Instance.AddActionStepHistory(nActionStepHistoryID, actionStep.ActionStepID, isRealMode, WorkFlowState.RUN, (DateTime)arrActionStepBeginTime[nIndex], true, bUseSMS);
                    m_nInitHistoryLogCount++;
                }

                if (tabPage != null && nIndex >= 0)
                {
                    tabPage.ActionStepHistoryID = nActionStepHistoryID;
                }

                
                
                if (tabPage == null)
                    continue;

                tabPage.SensorZoneHistoryID = nSensorZoneHistoryID;

                WorkFlow work = WorkFlowManager.Instance.Get(tabPage.ActionStepID, !tabPage.VirtualMode);
                if (work != null)
                {
                    work.State = WorkFlowState.RUN;
                }

                if (pageHome.GetCurrentTabPage() == null)
                    pageHome.SelectTabPage(tabPage);

                //pageHome.TabControls.SelectedTab = tabPage;
                TabPageManager.Instance.AddPage(tabPage, isRealMode);
                if (tabPage.CreateNew == true)
                {
                    tabPage.LinkedZoneID = -1;
                    tabPage.LinkedZoneName = "";
                    tabPage.SensorZoneHistoryID = -1;

                    ArrayList arrPanels = pageHome.AddPane(arrStepMember, actionStep.ActionStepID, tabPage);

                    string strAdd = "";

                    if (!isNormal)
                        strAdd = "(야간)";

                    foreach (PanelSectionEx pane in arrPanels)
                    {
                        pane.AddPanelTitle(szDisasterName + strAdd);
                        pane.ShowSectionButton(FormSOP.Instance.ShowSectionBtn);
                    }

                    if (!ioMgr.LoadNewPanelComponent(dbMgr, arrPanels, arrStepMember, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular, ref dicControlRoom))
                        return false;

                    string strPosition = null, strBroadcastPositionName = null;
                    VariousData<DateTime> dtDetect = null;

                    if (nIndex >= 0 && nActionStepHistoryID > 0)
                    {
                        dtDetect = new VariousData<DateTime>((DateTime)arrActionStepDetectTime[nIndex]);
                    }

                    HistoryDisasterPosition pos = HistoryManager2.Instance.FindHistoryDisasterPosition(tabPage.ActionStepID, !tabPage.VirtualMode);

                    if (pos != null)
                    {
                        strPosition = pos.PoistionName;
                        strBroadcastPositionName = pos.BroadcastName;
                    }

                    string strAmountSnowfall = null;
                    HistoryDisasterNoPosition noPos = HistoryManager2.Instance.FindHistoryDisasterNoPosition(tabPage.ActionStepID, !tabPage.VirtualMode);

                    if (noPos != null)
                    {
                        if (noPos.UseAmountSnowfall)
                        {
                            if (work != null && work.Option != null && work.Option is UnE.SOP.Workstate.WorkflowOptionSnowFall)
                            {
                                ((UnE.SOP.Workstate.WorkflowOptionSnowFall)work.Option).UseAmountSnowFall = noPos.UseAmountSnowfall;
                                double dAmountSnowfall;

                                if (double.TryParse(noPos.AmountSnowfall.Trim(), out dAmountSnowfall))
                                    ((UnE.SOP.Workstate.WorkflowOptionSnowFall)work.Option).AmountSnowFall = dAmountSnowfall;
                                //work.AmountSnowfall = noPos.AmountSnowfall;
                            }

                            strAmountSnowfall = noPos.AmountSnowfall;
                        }
                    }

                    pageHome.CreateComponentContentsSet(tabPage);
                    //pageHome.StartComponentContents(tabPage.ActionStepID, !tabPage.VirtualMode, dtDetect, strPosition);

                    if (nIndex >= 0 && arrActionStepHistories != null && nActionStepHistoryID > 0)
                    {
                        Data_ActionStepHistory find = null;
                        foreach (Data_ActionStepHistory history in arrActionStepHistories)
                        {
                            if (history.ID == nActionStepHistoryID)
                            {
                                find = history;
                            }
                        }

                        if (find != null)
                            SelectActionStepComponent((Data_ActionStepHistory)find, tabPage, pageHome);
                    }

                    /*if (nIndex >= 0 && nActionStepHistoryID > 0)
                    {
                        if (!LoadComponentHistory(dbMgr, arrPanels, arrActionStepHistoryID, arrActionStepID, arrActionStepBeginTime, arrActionStepDetectTime, arrDisaster, arrSensorZoneHistories, isRealMode, dicDisaster, dicDisasterFullPath, isRegular, isNormal))
                        {
                            return false;
                        }
                    }*/

                    if (nIndex >= 0 && nActionStepHistoryID > 0)
                    {
                        // 이미 실행되고 있는 SOP의 재난위치와 시각을 표시한다.
                        WorkFlow workflow = FormSOP.Instance.CurrentWork;

                        WorkflowOption option = workflow == null ? null : workflow.Option;

                        if (pos == null)
                        {
                            pageHome.StartComponentContents(tabPage.ActionStepID, !tabPage.VirtualMode, option, true);
                            //pageHome.StartComponentContents(tabPage.ActionStepID, !tabPage.VirtualMode, dtDetect, strPosition, strBroadcastPositionName, true, null, null, strAmountSnowfall);
                        }
                        else
                        {
                            pageHome.StartComponentContents(tabPage.ActionStepID, !tabPage.VirtualMode, option, true);
                            //pageHome.StartComponentContents(tabPage.ActionStepID, !tabPage.VirtualMode, dtDetect, strPosition, strBroadcastPositionName, true, pos.PSMMaterial, new VariousData<int>(pos.PSMDistance), strAmountSnowfall);
                        }

                        // 이미 실행되고 있는 SOP의 재난위치와 시각을 표시한다.
                        //WorkFlow workflow = FormSOP.Instance.CurrentWork;
                        if (workflow != null)
                        {
                            workflow.BeginEndEventSendSMS = bUseSMS;

                            List<PanelSection> panels = tabPage.GetPanelSections();
                            foreach (PanelSectionEx pane in panels)
                            {
                                // 이미 실행되고 있는 SOP에 대해서는 시작버튼을 비활성화 한다.
                                pane.HideAllSectionButtons();

                                string szName = UnE.SOP.ProxySOP.Instance.SiteName;
                                if (workflow.Option.HasPosition == true && strPosition != null && strPosition != "")
                                {
                                    szName = strPosition;
                                }

                                pane.SetWorkflowOption(workflow.Option);
                                /*if (pos != null && pos.UsePSM)
                                    pane.SetInfoText(szName, dtDetect.Data.ToString(), pos.PSMMaterial);
                                else
                                    pane.SetInfoText(szName, dtDetect.Data.ToString());*/
                            }

                            FormSOP.Instance.SetWorkflowState(workflow.State);
                        }
                    }
                }
                tabPage.CreateNew = false;
                tabPage.VirtualMode = !isRealMode;
            }

            ArrayList allTabPages = pageHome.GetTabPage();
            SectionTabPage _tabPage = null;

            for (int i = 0; i < allTabPages.Count; i++)
            {
                SectionTabPage page = (SectionTabPage)allTabPages[i];
                if (page.ActionStepHistoryID > 0)
                {
                    _tabPage = page;
                    //pageHome.SelectTabPage(page);
                }
            }

            if (_tabPage != null)
                pageHome.SelectTabPage(_tabPage);

            return true;
        }

        //private bool _LoadActionStepPanel(DBUtility.WebDBManager dbMgrex, ArrayList arrActionStepHistories, string strActionstepIDs, ArrayList arrActionStepHistoryID, ArrayList arrActionStepID, ArrayList arrActionStepBeginTime, ArrayList arrActionStepDetectTime, ArrayList arrDisaster, bool isRealMode, Dictionary<int, DisasterInfo> dicDisaster, Dictionary<DisasterInfo, string> dicDisasterFullPath, bool isRegular, bool isNormal)
		//{
		//	
		//}

        private void SelectActionStepComponent(Data_ActionStepHistory history, SectionTabPage page, PageBackstageSOP pageHome)
        {
            if (history.SelectedSectionID < 0 || history.SelectedSectionType < 0)
                return;

            foreach (Control ctrl in page.Controls)
            {
                if (ctrl is PanelSection)
                {
                    PanelSection panel = (PanelSection)ctrl;

                    foreach (Section section in panel.Sections)
                    {
                        if (section.Data.ID == history.SelectedSectionID && (int)section.GetComponentType() == history.SelectedSectionType)
                        {
                            ISectionContents contents = pageHome.GetComponentContents(section);

                            if (contents != null)
                            {
                                // 동기화 문제로 인하여 1초뒤 실행되도록 한다.
                                System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(SelectComponentContentsThread));
                                t.Start(contents);
                            }
                            break;
                        }
                    }

                    break;
                }
            }
        }

        private void SelectComponentContentsThread(object param)
        {
            System.Threading.Thread.Sleep(1000);
            ISectionContents contents = (ISectionContents)param;

            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                FormSOP.Instance.FocusSection(contents.Section);
                FormSOP.Instance.GetPageHome().SelectComponentContents(contents, true);
            });
        }

		private Sections.Section FindSection(string strComponentID, ArrayList arrSections)
		{
			foreach (Sections.Section section in arrSections)
			{
				if (section.Data.ComponentID == strComponentID)
					return section;
			}
			return null;
		}

		private void SetSectionProcessButtonStatus(ProcessButtonManager mgr, State state, int nProcessDirections, ProcessDirectionHistory processDirection)
		{
			Sections.Arrow.ArrowPosition arrowPosition = Sections.Arrow.ArrowPosition.NONE;

			if (processDirection == ProcessDirectionHistory.TOP)
				arrowPosition = Sections.Arrow.ArrowPosition.TOP;
			else if (processDirection == ProcessDirectionHistory.BOTTOM)
				arrowPosition = Sections.Arrow.ArrowPosition.BOTTOM;
			else if (processDirection == ProcessDirectionHistory.LEFT)
				arrowPosition = Sections.Arrow.ArrowPosition.LEFT;
			else if (processDirection == ProcessDirectionHistory.RIGHT)
				arrowPosition = Sections.Arrow.ArrowPosition.RIGHT;

			ProcessButton btn = mgr.FindButton(arrowPosition);
			if (btn == null)
				return;

			ProcessButton.ButtonStatus btnStatus = ProcessButton.ButtonStatus.UNKNOWN;

			if (state == State.DONE)
				btnStatus = ProcessButton.ButtonStatus.DONE;
			else if (state == State.INPUT || state == State.NORMAL)
				btnStatus = ProcessButton.ButtonStatus.WAIT;
			else if (state == State.RUN)
				btnStatus = ProcessButton.ButtonStatus.WAIT;
			else if (state == State.SKIP)
				btnStatus = ProcessButton.ButtonStatus.CANCEL;

			if ((nProcessDirections & (int)processDirection) == (int)processDirection)
				btn.Status = btnStatus;
			else
				btn.Status = ProcessButton.ButtonStatus.WAIT;
		}

		private void SetSectionProcessButtons(Sections.Section section, State state, int nProcessDirections, WorkFlow workFlow)
		{
            if (section.GetSectionPainter(0) == null)
				return;

            ProcessButtonManager mgr = (ProcessButtonManager)section.GetSectionPainter(0);

			if (state != State.DONE)
			{
				//Sections.SectionState sectionState = FormMain.Instance.CurrentWork.FindState(section);
				SectionState sectionState = workFlow.FindState(section);
				mgr.SetAllButtonsStatus(ProcessButton.ButtonStatus.WAIT, null, sectionState);
				return;
			}

			SetSectionProcessButtonStatus(mgr, state, nProcessDirections, ProcessDirectionHistory.TOP);
			SetSectionProcessButtonStatus(mgr, state, nProcessDirections, ProcessDirectionHistory.BOTTOM);
			SetSectionProcessButtonStatus(mgr, state, nProcessDirections, ProcessDirectionHistory.LEFT);
			SetSectionProcessButtonStatus(mgr, state, nProcessDirections, ProcessDirectionHistory.RIGHT);

			// Decision의 경우 특정 버튼이 완료 상태면 나머지 버튼들은 모두 사용안함 상태로 만든다.
			if (section.GetComponentType() == Sections.Section.ComponentType.DECISION)
			{
				ProcessButton btn = GetCompletedProcessButton(mgr);

				if (btn != null)
					mgr.SetAllButtonsStatus(ProcessButton.ButtonStatus.NOT_USE, btn);
			}
		}

		private ProcessButton GetCompletedProcessButton(ProcessButtonManager mgr)
		{
			ProcessButton btn = mgr.FindButton(Sections.Arrow.ArrowPosition.TOP);
			
			if (btn != null && btn.Status == ProcessButton.ButtonStatus.DONE)
				return btn;

			btn = mgr.FindButton(Sections.Arrow.ArrowPosition.BOTTOM);
			
			if (btn != null && btn.Status == ProcessButton.ButtonStatus.DONE)
				return btn;

			btn = mgr.FindButton(Sections.Arrow.ArrowPosition.RIGHT);

			if (btn != null && btn.Status == ProcessButton.ButtonStatus.DONE)
				return btn;

			btn = mgr.FindButton(Sections.Arrow.ArrowPosition.LEFT);

			if (btn != null && btn.Status == ProcessButton.ButtonStatus.DONE)
				return btn;

			return null;
		}

        private void AddSOPSectionLog(int nActionStepID, int nActionStepHistoryID, Sections.Section section, int nComponentHistoryID, bool isRealMode, int nStatus, int nProcessDirections, ArrayList arrSections, string strTask, DateTime time, string strDescription, bool showBoard, int nCheckedNotify1, int nCheckedNotify2, int nCheckedRun, int nCheckedComplete, int nAccessedUserID, Dictionary<int, List<HistorySectionData.DetailData>> dicDetailDatas, WorkFlow workFlow)
		{
			State state;

			if (nStatus == 1)
				state = State.NORMAL;
			else if (nStatus == 2)
				state = State.RUN;
			else if (nStatus == 3)
				state = State.DONE;
			else if (nStatus == 5)
				state = State.SKIP;
			else
			{
                ApplyDetailDatas(nActionStepHistoryID, nComponentHistoryID, section, dicDetailDatas);

				// 입력대기는 SOP Log 창에 표시하지 않는다.
				return;
			}

			//HistoryManager2.Instance.SetLastComponentHistory(nActionStepID, nComponentHistoryID);

			SetSectionProcessButtons(section, state, nProcessDirections, workFlow);
			Sections.Section.ComponentType type = section.GetComponentType();

			SectionState sectionState = WorkFlowManager.Instance.Find(section, isRealMode);
            if (sectionState == null)
                return;

            if (sectionState != null && FormSOP.Instance.HasSOPControl(nActionStepHistoryID))
                FormSOP.Instance.GetPageHome().SetSectionDetailDatas(sectionState.DetailDatas, section, nComponentHistoryID);

			if (type == Sections.Section.ComponentType.ENDPOINT || type == Sections.Section.ComponentType.PROCESS ||
				type == Sections.Section.ComponentType.TRANSSOP || type == Sections.Section.ComponentType.LINK)
			{
                HistoryManager2.Instance.AddSectionHistory(section, sectionState, nComponentHistoryID, state, nProcessDirections, true, time, showBoard, nCheckedNotify1, nCheckedNotify2, nCheckedRun, nCheckedComplete, sectionState.DetailDatas);
				m_nInitHistoryLogCount++;
			}
			else if (type == Sections.Section.ComponentType.DECISION)
			{
				Sections.Section nextSection = strDescription.Length == 0 ? null : FindSection(strDescription, arrSections);
				HistoryManager2.Instance.AddDecisionHistory((Sections.SectionDecision)section, sectionState, state, nProcessDirections, nextSection, true, time, showBoard);
				m_nInitHistoryLogCount++;
			}
			else if (type == Sections.Section.ComponentType.INTERNAL)
			{
				Sections.SectionDataInternal data = (Sections.SectionDataInternal)section.Data;
				bool usePopupMessage = data.UsePopupMessage;
				bool useSMS = data.UseMobileApp;
				bool useBroadcast = data.UseBroadcast;

				HistoryManager2.Instance.AddInternalHistory((Sections.SectionInternal)section, sectionState, state, nProcessDirections, nCheckedRun, nCheckedComplete, usePopupMessage, useSMS, useBroadcast, true, time, showBoard, nCheckedNotify1);
				m_nInitHistoryLogCount++;
			}
			else if (type == Sections.Section.ComponentType.EXTERNAL)
			{
				Sections.SectionDataExternal data = (Sections.SectionDataExternal)section.Data;
				bool useSMS = data.UseSMS;
				bool useFax = data.UseFax;
				HistoryManager2.Instance.AddExternalHistory((Sections.SectionExternal)section, state, nProcessDirections, useSMS, useFax, true, time, showBoard, nCheckedNotify1, nCheckedNotify2, nCheckedRun, nCheckedComplete);
				m_nInitHistoryLogCount++;
			}
			else if (type == Sections.Section.ComponentType.TRANSMISSION)
			{
				Sections.SectionDataTransmission data = (Sections.SectionDataTransmission)section.Data;
				bool usePopupMessage = data.DataInternal.UsePopupMessage;
				bool useSMS = data.DataInternal.UseMobileApp;
				bool useBroadcast = data.DataInternal.UseBroadcast;
				bool useExSMS = data.DataExternal.UseSMS;
				bool useExFax = data.DataExternal.UseFax;
				HistoryManager2.Instance.AddTransmissionHistory((Sections.SectionTransmission)section, state, nProcessDirections, usePopupMessage, useSMS, useBroadcast, useExSMS, useExFax, true, time, showBoard, nCheckedNotify1, nCheckedNotify2, nCheckedRun, nCheckedComplete);
				m_nInitHistoryLogCount++;
			}

            //sectionState.State = state;
            sectionState.Time = new VariousData<DateTime>(time);
            sectionState.AccessedUserID = nAccessedUserID;
            ApplyDetailDatas(nActionStepHistoryID, nComponentHistoryID, section, dicDetailDatas);
		}

        private void ApplyDetailDatas(int nActionStepHistoryID, int nComponentHistoryID, Section section, Dictionary<int, List<HistorySectionData.DetailData>> dicDetailDatas)
        {
            //if (!FormSOP.Instance.HasSOPControl(nActionStepHistoryID) || !FormSOP.Instance.Initialization)
            //if (!FormSOP.Instance.HasControl || !FormSOP.Instance.Initialization)
            {
                List<HistorySectionData.DetailData> detailDatas;

                if (dicDetailDatas.TryGetValue(nComponentHistoryID, out detailDatas))
                {
                    ISectionContents contents = FormSOP.Instance.GetPageHome().GetComponentContents(section);

                    if (contents != null)
                    {
                        PanelSectionEx panel = (PanelSectionEx)section.GetParent();
                        SectionTabPage page = (SectionTabPage)panel.Parent;
                        SectionState state = WorkFlowManager.Instance.Find(section, !page.VirtualMode);

                        AddDetailDatas(state, detailDatas, nComponentHistoryID);
                        contents.SetDetailDatas(nComponentHistoryID, detailDatas);

                        if (detailDatas.Count > 0)
                        {
                            System.Diagnostics.Trace.WriteLine("contents.SetDetailDatas : " + nComponentHistoryID.ToString() + ", " + detailDatas.Count);
                        }
                    }
                }
            }
        }

        // dicDetailDatas의 내용을 state에 추가한다.
        private void AddDetailDatas(SectionState state, List<HistorySectionData.DetailData> detailDatas, int nComponentHistoryID)
        {
            if (state == null)
                return;

            List<HistorySectionData.DetailData> details;

            if (state.DetailDatas.TryGetValue(nComponentHistoryID, out details))
            {
                foreach (HistorySectionData.DetailData detail in detailDatas)
                {
                    if (!details.Contains(detail))
                    {
                        details.Add(detail);
                    }
                }
            }
            else
            {
                state.DetailDatas[nComponentHistoryID] = detailDatas;
            }
        }

        public void AddSOPSectionLog(int nActionStepID, int nActionStepHistoryID, ArrayList arrComponentHistoryID, ArrayList arrSections, ArrayList arrStatus, ArrayList arrProcessDirections, ArrayList arrTask, ArrayList arrTime, ArrayList arrDescription, ArrayList arrShowBoard, ArrayList arrCheckedNotify1, ArrayList arrCheckedNotify2, ArrayList arrCheckedRun, ArrayList arrCheckedComplete, ArrayList arrAccessedUserID, bool isRealMode, Dictionary<int, List<HistorySectionData.DetailData>> dicDetailDatas, WorkFlow workFlow)
		{
			int nSectionCount = arrSections.Count;

			for (int i=0;i<nSectionCount;i++)
			{
				Sections.Section section = (Sections.Section)arrSections[i];

				int nComponentHistoryID = (int)arrComponentHistoryID[i];
				int nStatus = (int)arrStatus[i];
				int nProcessDirections = (int)arrProcessDirections[i];
				string strDescription = (string)arrDescription[i];
				string strTask = (string)arrTask[i];
				DateTime time = (DateTime)arrTime[i];
				bool showBoard = (bool)arrShowBoard[i];
				int nCheckedNotify1 = (int)arrCheckedNotify1[i];
				int nCheckedNotify2 = (int)arrCheckedNotify2[i];
                int nCheckedRun = (int)arrCheckedRun[i];
                int nCheckedComplete = (int)arrCheckedComplete[i];
                int nAccessedUserID = (int)arrAccessedUserID[i];

                AddSOPSectionLog(nActionStepID, nActionStepHistoryID, section, nComponentHistoryID, isRealMode, nStatus, nProcessDirections, arrSections, strTask, time, strDescription, showBoard, nCheckedNotify1, nCheckedNotify2, nCheckedRun, nCheckedComplete, nAccessedUserID, dicDetailDatas, workFlow);
			}
		}

		private string GetActionStepPath(int nActionStepID, ArrayList arrActionSteps)
		{
			foreach (ActionStepInfo actionStep in arrActionSteps)
			{
				if (actionStep.ActionStepID == nActionStepID)
				{
					if (actionStep.ParentStepID >= 0)
						return GetActionStepPath(actionStep.ParentStepID, arrActionSteps) + "/" + actionStep.ActionStepName;
					else
						return actionStep.ActionStepName;
				}
			}

			return "";
		}

		private string GetActionStepFullPath(int nActionStepID, Dictionary<int, DisasterInfo> dicDisaster, Dictionary<DisasterInfo, string> dicDisasterFullPath)
		{
			if (dicDisaster.ContainsKey(nActionStepID))
			{
				DisasterInfo disaster = dicDisaster[nActionStepID];

				if (dicDisasterFullPath.ContainsKey(disaster))
				{
					string strDisasterFullPath = dicDisasterFullPath[disaster];
					return strDisasterFullPath + "/" + GetActionStepPath(nActionStepID, disaster.ActionSteps);
				}
			}

			return "";
		}

		//private bool LoadComponentHistory(WebDBManager dbMgr, ArrayList arrPanels, ArrayList arrActionStepHistoryID, ArrayList arrActionStepID, ArrayList arrActionStepBeginTime, ArrayList arrActionStepDetectTime, ArrayList arrDisaster,ArrayList arrSensorZoneHistories, bool isRealMode, Dictionary<int, DisasterInfo> dicDisaster, Dictionary<DisasterInfo, string> dicDisasterFullPath, bool isRegular, bool isNormal)
		//{
		//	if (arrPanels.Count == 0)
		//		return true;

		//	Sections.PanelSectionEx panel = (Sections.PanelSectionEx)arrPanels[0];
		//	int nIndex = arrActionStepID.IndexOf(panel.ActionStepID);

		//	if (nIndex < 0)
		//		return false;

		//	int nActionStepHistoryID = (int)arrActionStepHistoryID[nIndex];
		//	DateTime dtBegin = (DateTime)arrActionStepBeginTime[nIndex];
		//	DisasterInfo disaster = (DisasterInfo)arrDisaster[nIndex];

  //          string strSQL = string.Format("select ID, ComponentID, ComponentType, Time, Status, Task, CompleteCount, CheckedNotify1, CheckedNotify2, CheckedRun, CheckedComplete, Description, ShowBoard, AccessedUserID from ComponentHistory where ActionStepHistoryID = {0} order by ID",
		//		nActionStepHistoryID);

		//	ArrayList arrResult = dbMgr.GetResultData(strSQL);
		//	if (arrResult == null) return false;

		//	int nResultCount = arrResult.Count;
		//	DateTime dtDefault = new DateTime();

		//	// Section, Section Status
		//	Dictionary<Sections.Section, int> dicSectionStatus = new Dictionary<Sections.Section, int>();

		//	ArrayList arrSections4Log = new ArrayList();
		//	ArrayList arrSectionStatus4Log = new ArrayList();
		//	ArrayList arrSectionProcessDirections4Log = new ArrayList();
		//	ArrayList arrDescription = new ArrayList();
		//	ArrayList arrTask = new ArrayList();
		//	ArrayList arrTime = new ArrayList();
		//	// 상황판에 보여줄 것인가?
		//	ArrayList arrShowBoard = new ArrayList();
		//	ArrayList arrComponentHistoryID = new ArrayList();
		//	ArrayList arrCheckedNotify1 = new ArrayList();
		//	ArrayList arrCheckedNotify2 = new ArrayList();
  //          ArrayList arrCheckedRun = new ArrayList();
  //          ArrayList arrCheckedComplete = new ArrayList();
  //          ArrayList arrAccessedUserID = new ArrayList();

		//	ArrayList arrAllSections = GetAllPanelSections(arrPanels);

  //          string strComponentHistoryIDs = "";
  //          Section selectedSection = null;

		//	for (int i = 0; i < nResultCount - 13; i += 14)
		//	{
		//		int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
		//		int nComponentID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
		//		int nComponentType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
		//		DateTime time = WebDBManager.GetDateTimeField(arrResult[i + 3], dtDefault);
		//		int nStatus = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
		//		string strTask = WebDBManager.GetStringField(arrResult[i + 5].ToString(), "");
		//		int nCompleteCount = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
		//		int nCheckedNotify1 = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0);
		//		int nCheckedNotify2 = WebDBManager.GetIntField(arrResult[i + 8].ToString(), 0);
  //              int nCheckedRun = WebDBManager.GetIntField(arrResult[i + 9].ToString(), 0);
  //              int nCheckedComplete = WebDBManager.GetIntField(arrResult[i + 10].ToString(), 0);
		//		string strDescription = WebDBManager.GetStringField(arrResult[i + 11].ToString(), "");
		//		bool showBoard = WebDBManager.GetIntField(arrResult[i + 12].ToString(), 0) == 0 ? false : true;
  //              int nAccessedUserID = WebDBManager.GetIntField(arrResult[i + 13].ToString(), -1);

		//		Sections.Section section = FindSection(nComponentID, nComponentType, arrAllSections);
		//		if (section == null)
		//			continue;

		//		section.CompleteCount = nCompleteCount;

		//		dicSectionStatus[section] = nStatus;

		//		int nDirections = nStatus >> 16;
		//		nStatus = nStatus & 0x0000ffff;

  //              if (nStatus == (int)UnE.SOP.Workstate.State.RUN)
  //                  selectedSection = section;

		//		// SOP Log창 기록을 위한 List
		//		arrComponentHistoryID.Add(nID);
		//		arrSections4Log.Add(section);
		//		arrSectionStatus4Log.Add(nStatus);
		//		arrSectionProcessDirections4Log.Add(nDirections);
		//		arrDescription.Add(strDescription);
		//		arrTask.Add(strTask);
		//		arrTime.Add(time);
		//		arrShowBoard.Add(showBoard);
		//		arrCheckedNotify1.Add(nCheckedNotify1);
		//		arrCheckedNotify2.Add(nCheckedNotify2);
  //              arrCheckedRun.Add(nCheckedRun);
  //              arrCheckedComplete.Add(nCheckedComplete);
  //              arrAccessedUserID.Add(nAccessedUserID);

  //              if (strComponentHistoryIDs.Length == 0)
  //                  strComponentHistoryIDs = nID.ToString();
  //              else
  //                  strComponentHistoryIDs += ", " + nID.ToString();

  //              if (m_nLastComponentHistoryID < nID)
  //                  m_nLastComponentHistoryID = nID;
  //          }


		//	if (WorkFlowManager.Instance.Exist(panel.ActionStepID, isRealMode))
		//		WorkFlowManager.Instance.Remove(panel.ActionStepID, isRealMode);

		//	WorkFlow workFlow = WorkFlowManager.Instance.Add(panel.ActionStepID, arrAllSections, isRealMode);

  //          if (workFlow == null)
  //              return false;

  //          if (panel.Parent is SectionTabPage)
  //          {
  //              SectionTabPage page = (SectionTabPage)panel.Parent;
  //              workFlow.SetSectionContents(page.SectionContents);
  //              workFlow.InitSectionContents();
  //          }

  //          workFlow.SelectSection(selectedSection);

  //          workFlow.WorkFlowEvent -= FormSOP.Instance.OnWorkflowChanged;
		//	workFlow.WorkFlowEvent += FormSOP.Instance.OnWorkflowChanged;

  //          if (workFlow.Option == null)
  //          {
  //              string strCategoryName, strSubCategoryName;
                
  //              if (FormSOP.Instance.SOPManager.GetDisasterFullPath(disaster, out strCategoryName, out strSubCategoryName))
  //                  workFlow.Option = SOPMonitoringSystem.Process.WorkFlowStartNotifyProcess.MakeWorkflowOption(strCategoryName, strSubCategoryName);
  //          }

  //          if (workFlow.Option == null)
  //              workFlow.Option = new UnE.SOP.Workstate.WorkflowOption();

		//	int nArrIndex = arrActionStepID.IndexOf(panel.ActionStepID);

		//	if (nArrIndex >= 0)
		//	{
		//		DateTime dtDetect = (DateTime)arrActionStepDetectTime[nArrIndex];
		//		workFlow.Option.DetectTime = new VariousData<DateTime>(dtDetect);
		//	}

  //          int nSensorZoneHistoryID = -1;
  //          if( nArrIndex >= 0)
  //          {
  //              nSensorZoneHistoryID = (int)arrSensorZoneHistories[nArrIndex];
  //              workFlow.Option.SensorZoneHistoryID = nSensorZoneHistoryID;                
  //          }

  //          // Key : ComponentHistory ID
  //          Dictionary<int, List<HistorySectionData.DetailData>> dicDetailDatas = UnE.SOP.History.HistoryManager.LoadComponentHistoryDetailDatas(dbMgr, strComponentHistoryIDs, arrComponentHistoryID);

		//	AddSOPSectionLog(panel.ActionStepID, nActionStepHistoryID, arrComponentHistoryID, arrSections4Log, arrSectionStatus4Log, arrSectionProcessDirections4Log, arrTask, arrTime, arrDescription, arrShowBoard, arrCheckedNotify1, arrCheckedNotify2, arrCheckedRun, arrCheckedComplete, arrAccessedUserID, isRealMode, dicDetailDatas, workFlow);

		//	BarLevelTree tree = GetBarLevelTree();
		//	TreeNode node = tree.FindActionStepNode(panel.ActionStepID);
		//	string szPath = node == null ? GetActionStepFullPath(panel.ActionStepID, dicDisaster, dicDisasterFullPath) : node.FullPath;
		//	bool bHasPos = true;
		//	if (szPath.IndexOf("자연재해") != -1 || szPath.IndexOf("태풍") != -1)
		//	{
		//		bHasPos = false;
		//	}
		//	string sopName = szPath.Substring(szPath.IndexOf("\\") + 1);


           

		//	workFlow.Option.HasPosition = bHasPos;

  //          HistoryDisasterNoPosition info = HistoryManager.Instance.FindHistoryDisasterNoPosition(panel.ActionStepID, isRealMode);

  //          if (info != null)
  //              workFlow.Option.SetDisasterOptions(info.DisasterOptions);

		//	if (bHasPos == true)
		//	{
		//		HistoryDisasterPosition pos = HistoryManager.Instance.FindHistoryDisasterPosition(panel.ActionStepID, isRealMode);
		//		if (pos != null)
		//		{
		//			workFlow.Option.LastPosition = pos;
		//			workFlow.Option.PositionName = pos.PoistionName;
		//		}
		//	}
  //          else
  //          {
  //              if (info != null && workFlow.Option is UnE.SOP.Workstate.WorkflowOptionSnowFall)
  //              {
  //                  UnE.SOP.Workstate.WorkflowOptionSnowFall option = (UnE.SOP.Workstate.WorkflowOptionSnowFall)workFlow.Option;
  //                  option.UseAmountSnowFall = info.UseAmountSnowfall;

  //                  if (info.UseAmountSnowfall)
  //                  {
  //                      double dAmountSnowFall;

  //                      if (double.TryParse(info.AmountSnowfall.Trim(), out dAmountSnowFall))
  //                          option.AmountSnowFall = dAmountSnowFall;
  //                  }
  //              }
  //          }

		//	workFlow.SOPName = sopName;
		//	workFlow.State = WorkFlowState.RUN;
		//	FormSOP.Instance.SetCurrentWorkflow(workFlow);

		//	int nSectionCount = arrAllSections.Count;

  //          Dictionary<SectionState, Sections.Section> dicDup = new Dictionary<SectionState, Section>();

		//	for (int i = 0; i < nSectionCount; i++)
		//	{
		//		Sections.Section section = (Sections.Section)arrAllSections[i];

		//		// add by skkim : 2013-01-07 링크 노드 상태 세팅 제외
		//		if (section.GetComponentType() == Sections.Section.ComponentType.LINK || 
  //                   section.GetComponentType() == Section.ComponentType.ANNOTATION ||
  //                  section.GetComponentType() == Section.ComponentType.GROUP)
                
		//			continue;

		//		int nStatus = dicSectionStatus.ContainsKey(section) ? dicSectionStatus[section] : 1/*대기상태*/;
		//		int nDirection = nStatus >> 16;

		//		nStatus = nStatus & 0x0000ffff;

		//		// changed by skkim : 2013-01-07 링크노드 상태 세팅 제외
		//		//Sections.SectionState state = workFlow.FindState(section, true);
		//		SectionState state = workFlow.FindState(section, false);
  //              if (state == null)
  //                  continue;

		//		if (nStatus == 2)
		//		{
		//			//state.InProgress();
		//			state.CopyState(State.RUN, WorkFlowManager.Instance.InProgressColor);
		//			ArrayList arList = workFlow.FindNext(state);
		//			foreach (SectionState next in arList)
		//			{
		//				if (next != null)
		//				{
  //                          if (dicDup.ContainsKey(next) || next == state)
  //                          {
  //                              continue;
  //                          }

  //                          next.CopyState(State.NORMAL, WorkFlowManager.Instance.NoramlColor);
  //                          //next.CopyState(State.INPUT, WorkFlowManager.Instance.InputWaitColor);
  //                          //next.InputWait();
  //                      }
		//			}

  //                  if (!dicDup.ContainsKey(state))
  //                  {
  //                      dicDup.Add(state, section);
  //                  }
		//		}
		//		else if (nStatus == 3)
		//		{
		//			state.CopyState(State.DONE, WorkFlowManager.Instance.CompleteColor);
		//			state.ProcessDirections = nDirection;
  //                  if (!dicDup.ContainsKey(state))
  //                  {
  //                      dicDup.Add(state, section);
  //                  }
		//		}
		//		else if (nStatus == 4)
		//		{
		//			state.CopyState(State.INPUT, WorkFlowManager.Instance.InputWaitColor);
  //                  if (!dicDup.ContainsKey(state))
  //                  {
  //                      dicDup.Add(state, section);
  //                  }
		//		}
		//		else if (nStatus == 5)
		//		{
		//			state.CopyState(State.SKIP, WorkFlowManager.Instance.SkipColor);
  //                  if (!dicDup.ContainsKey(state))
  //                  {
  //                      dicDup.Add(state, section);
  //                  }
		//		}

  //              //if (state != null)
  //              //    state.DetailDatas.Clear();
		//	}

  //          if (dicDisasterFullPath.ContainsKey(disaster))
  //          {
  //              string strDisasterPath = dicDisasterFullPath[disaster];
  //              string strActionStepPath = GetActionStepPath(disaster.ActionSteps, panel.ActionStepID);

  //              if (strActionStepPath.Length == 0)
  //                  return false;
  //              strDisasterPath = strDisasterPath.Replace((char)0x06, '/');

  //              AddSOPScenario(strDisasterPath + '/' + strActionStepPath, panel.ActionStepID, isRealMode, isRegular, isNormal, nActionStepHistoryID, nSensorZoneHistoryID);
  //          }

  //          return true;
		//}

        

		public ArrayList GetAllPanels(int nActionStepID)
		{
			return FormSOP.Instance.GetPageHome().GetAllPanels(nActionStepID);
		}

		public ArrayList GetTabPage()
		{
			return FormSOP.Instance.GetPageHome().GetTabPage();
		}

		public ArrayList GetAllPanelSections(ArrayList arrPanels)
		{
			ArrayList arrSections = new ArrayList();

			foreach (Sections.PanelSectionEx panel in arrPanels)
			{
				foreach (Sections.Section section in panel.Sections)
				{
					arrSections.Add(section);
				}
			}
			return arrSections;
		}

        public ArrayList GetAllPanelSections(List<PanelSection> arrPanels)
        {
            ArrayList arrSections = new ArrayList();

            foreach (Sections.PanelSection panel in arrPanels)
            {
                foreach (Sections.Section section in panel.Sections)
                {
                    arrSections.Add(section);
                }
            }
            return arrSections;
        }

        public Sections.Section FindSection(int nComponentID, int nComponentType, ArrayList arrSections)
		{
			foreach (Sections.Section section in arrSections)
			{
				if ((int)section.GetComponentType() == nComponentType)
				{
					if (section.Data.ID == nComponentID)
						return section;
				}
			}
            return null;
        }

        // 기존에 실행되고 있던 SOP를 불러온다.
        public bool LoadHistory(WebDBManager dbMgr, UnE.SOP.SOPManager sopMgr)
		{
			/*Dictionary<string, DisasterInfo> dicRegularNormal = sopMgr.GetSOPDictionary(true, true);
			Dictionary<string, DisasterInfo> dicRegularAbnormal = sopMgr.GetSOPDictionary(true, false);
			Dictionary<string, DisasterInfo> dicNonregularNormal = sopMgr.GetSOPDictionary(false, true);
			Dictionary<string, DisasterInfo> dicNonregularAbnormal = sopMgr.GetSOPDictionary(false, false);*/

			// Key : 양수이면 실제모드의 ActionStepID
			//       음수이면 훈련모드의 ActionStepID
			/*Dictionary<int, Data_ActionStepHistory> dicActionStepHistories = new Dictionary<int,Data_ActionStepHistory>();

			if (!LoadActionStepHistory(dbMgr, dicActionStepHistories))
				return false;

            foreach (SOPScenario scenario in m_arrScenario)
            {
                foreach (KeyValuePair<int, Data_ActionStepHistory> pair in dicActionStepHistories)
                {
                    if (scenario.ActionStepHistoryID == pair.Value.ID)
                    {
                        dicActionStepHistories.Remove(pair.Key);
                        break;
                    }
                }
            }

            if (!LoadHistory(dbMgr, dicActionStepHistories))
            {
                m_isLoadComponentHistory = true;
                return false;
            }

            foreach (KeyValuePair<int, Data_ActionStepHistory> pair in dicActionStepHistories)
            {
                int nActionStepID = pair.Value.ActionStepID;

                int nHistoryResultCount = m_arrHistory.Count;
                bool processed = false;

                for (int i = 0; i < nHistoryResultCount; i ++ )
                {
                    ArrayList arrResult = (ArrayList)m_arrHistory[i];

                    if (arrResult.Count >= 2)
                    {
                        if (arrResult[1] is int)
                        {
                            int nID = (int)arrResult[1];

                            if (nActionStepID == nID)
                            {
                                processed = true;
                                break;
                            }
                        }
                    }
                }

                if (!processed)
                {
                    FormSOP.Instance.ReloadDisaster(nActionStepID);
                }
            }

			// 초기 로딩시 DB로부터 읽어들인 History는 HistoryManager의 Thread 및 SOPLog의 Timer를 통하여 최종 전달된다.
			// 따라서, 데이터가 최종적으로 전달된 후에 Log 보기 옵션을 [개별 보기]로 바꾼다.
			FormSOP.Instance.GetPageHome().GetDockSOPLog().ReservationComboBoxChange(false);

			// DB로부터 현재 실행중인 SOP를 불러왔으면 그 가운데 하나를 선택한다.
			SelectCurrentSOP();*/

			m_isLoadComponentHistory = true;
			return true;
		}

		/*private void SelectCurrentSOP()
		{
			int nRowCount = m_arrScenario.Count;
			if (nRowCount == 0)
				return;

			if (nRowCount == 1)
				CurrentScenario = (SOPScenario)m_arrScenario[0];
			else
			{
				//string strSQL = "Select ActionStepID, RealMode from CurrentActionStep where id = 1";

                string szText = "SELECT ActionStepID, RealMode FROM CurrentActionStep WHERE id = (SELECT min(id) FROM CurrentActionStep WHERE SiteID = {0})";
                string strSQL = string.Format(szText, m_nSiteID);

				ArrayList arrResult = FormSOP.Instance.DBManager.GetResultData(strSQL);

				if (arrResult == null || arrResult.Count < 2)
				{
					CurrentScenario = (SOPScenario)m_arrScenario[0];
				}
				else
				{
					int nActionStepID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
					bool isRealMode = WebDBManager.GetIntField(arrResult[1].ToString(), 0) == 0 ? false : true;

					for (int i = 0; i < nRowCount; i++)
					{
						SOPScenario sop = (SOPScenario)m_arrScenario[i];
						if (sop.RealMode == isRealMode && sop.ActionStepID == nActionStepID)
						{
							CurrentScenario = (SOPScenario)m_arrScenario[i];
							return;
						}
					}
					CurrentScenario = (SOPScenario)m_arrScenario[0];
				}
			}
		}*/
		
        public void ClearScenario()
        {
            m_arrScenario.Clear();
        }


        public string GetDisasterName(int nActionStepID)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT dis.DisasterName FROM ActionStep AS step ");
            sb.Append(" INNER JOIN Disaster AS dis ON dis.ID = step.DisasterID ");
            sb.AppendFormat(" WHERE step.ID = {0}", nActionStepID);

            string szSQL = sb.ToString();

            ArrayList arrResult = FormSOP.Instance.DBManager.GetResultData(szSQL);
            if (arrResult == null || arrResult.Count == 0)
                return "";

            string strDisName = WebDBManager.GetStringField(arrResult[0], "");
            return strDisName;
        }


        public void NewActionStepHistory(int nActionStepHistoryID)
        {
                       
        }

        public Data_ActionStepHistory GetActionStepHistory(int nActionStepHistoryID)
        {
            ActionStepHistoryEx actionStepHistory;

            if (m_dicActionStepHistory.TryGetValue(nActionStepHistoryID, out actionStepHistory) == false)
                return null;

            return actionStepHistory.History;
        }

        public void AddActionStepHistory(Data_ActionStepHistory actionStepHistory)
        {
            var historyEx = new ActionStepHistoryEx(actionStepHistory);
            m_dicActionStepHistory[actionStepHistory.ID] = historyEx;
        }

        public List<Data_ActionStepHistory> GetNewActionStepHistory(Dictionary<int, Data_ActionStepHistory> dicOldActionStepHistory)
        {
            List<Data_ActionStepHistory> histories = new List<Data_ActionStepHistory>();
            List<int> actionStepHistoryIDs = m_dicActionStepHistory.Keys.ToList();

            ActionStepHistoryEx actionStepHistory;

            foreach (int nActionStepHistoryID in actionStepHistoryIDs)
            {
                if (dicOldActionStepHistory.ContainsKey(nActionStepHistoryID))
                    continue;

                if (m_dicActionStepHistory.TryGetValue(nActionStepHistoryID, out actionStepHistory))
                {
                    if (actionStepHistory.IsLoading == false && actionStepHistory.History.EndTime == null && actionStepHistory.History.CancelTime == null)
                    {
                        actionStepHistory.IsLoading = true;
                        histories.Add(actionStepHistory.History);
                    }
                }
            }

            return histories;
        }

        public List<Data_ActionStepHistory> GetRunningActionStepHistories()
        {
            var histories = new List<Data_ActionStepHistory>();
            List<int> actionStepHistoryIDs = m_dicActionStepHistory.Keys.ToList();

            ActionStepHistoryEx actionStepHistory;

            foreach (int nActionStepHistoryID in actionStepHistoryIDs)
            {
                if (m_dicActionStepHistory.TryGetValue(nActionStepHistoryID, out actionStepHistory))
                {
                    if (actionStepHistory.IsLoading && actionStepHistory.History.EndTime == null && actionStepHistory.History.CancelTime == null)
                        histories.Add(actionStepHistory.History);
                }
            }

            return histories;
        }

        public void LoadActionStepHistory(Data_ActionStepHistory actionStepHistory)
        {
            ActionStepHistoryEx historyEx = null;

            if (m_dicActionStepHistory.TryGetValue(actionStepHistory.ID, out historyEx) == false)
                return;

            if (!UnE.SOP.ProxySOP.Instance.SOPDataContainer.LoadDisasterActionStep(actionStepHistory.ActionStepID))
                return;

            ActionStepInfo actionStep = UnE.SOP.ProxySOP.Instance.SOPDataContainer.GetActionStepInfo(actionStepHistory.ActionStepID);

            if (actionStep == null)
                return;

            Dictionary<DisasterInfo, string> dicDisasterFullPath = new Dictionary<DisasterInfo, string>();
            bool isNormal, isRegular;
            DisasterInfo disaster = UnE.SOP.ProxySOP.Instance.SOPDataContainer.GetDisaster(actionStep.DisasterID, out isNormal, out isRegular);

            if (disaster == null)
                return;

            string strFullPath = UnE.SOP.ProxySOP.Instance.SOPDataContainer.GetDisasterFullPath(disaster);

            if (strFullPath == null || strFullPath.Length == 0)
                return;

            historyEx.Disaster = disaster;
            dicDisasterFullPath[disaster] = strFullPath;

            string strActionStepIDs = actionStepHistory.ActionStepID.ToString();

            ArrayList arrActionStepHistoryIDs = new ArrayList();
            ArrayList arrActionStepIDs = new ArrayList();
            ArrayList arrBeginTimes = new ArrayList();
            ArrayList arrDetectTimes = new ArrayList();
            ArrayList arrDisasters = new ArrayList();
            ArrayList arrSensorZoneHistoryIDs = new ArrayList();

            arrActionStepHistoryIDs.Add(actionStepHistory.ID);
            arrActionStepIDs.Add(actionStepHistory.ActionStepID);
            arrBeginTimes.Add(actionStepHistory.BeginTime);
            arrDetectTimes.Add(actionStepHistory.DetectTime);
            arrDisasters.Add(disaster);
            arrSensorZoneHistoryIDs.Add(actionStepHistory.SensorZoneHistoryID);

            HistoryManager2.Instance.AddHistoryDisasterPosition(actionStepHistory.ID, actionStepHistory.ActionStepID, actionStepHistory.RealMode);
            HistoryManager2.Instance.AddHistoryDisasterNoPosition(actionStepHistory.ActionStepID, actionStepHistory.RealMode, new UnE.SOP.HistoryDisasterNoPosition());

            DBUtility2.WebDBManager dbMgr = ProxySOP.Instance.DBManager;

            // ActionStep ID, Disaster
            Dictionary<int, DisasterInfo> dicDisaster = new Dictionary<int, DisasterInfo>();

            IWorkflowContainer workMan = ProxySOP.Instance.WorkflowContainer;
            bool isSuccess = LoadActionStepPanel(dbMgr, strActionStepIDs, arrActionStepHistoryIDs, arrActionStepIDs, arrBeginTimes, arrDetectTimes, arrDisasters, arrSensorZoneHistoryIDs, actionStepHistory.RealMode, dicDisaster, dicDisasterFullPath, isRegular, isNormal);

            if (isSuccess)
            {
                ISOPDataContainer sopData = ProxySOP.Instance.SOPDataContainer;
                sopData.SetActionStepHistoryID(actionStepHistory.ActionStepID, actionStepHistory.RealMode, actionStepHistory.ID);
            }
        }

        public bool LoadComponentHistory(int nActionStepHistoryID, int nActionStepID, bool isRealMode, List<Data_ComponentHistory> componentHistories)
        //public bool LoadComponentHistory(WebDBManager dbMgr, ArrayList arrPanels, ArrayList arrActionStepHistoryID, ArrayList arrActionStepID, ArrayList arrActionStepBeginTime, ArrayList arrActionStepDetectTime, ArrayList arrDisaster, ArrayList arrSensorZoneHistories, bool isRealMode, Dictionary<int, DisasterInfo> dicDisaster, Dictionary<DisasterInfo, string> dicDisasterFullPath, bool isRegular, bool isNormal)
        {
            ActionStepHistoryEx historyEx;

            if (m_dicActionStepHistory.TryGetValue(nActionStepHistoryID, out historyEx) == false)
                return false;

            // Loading되지 않은 ActionStepHistory는 아직 탭 생성이 시도되지 않은 상태다.
            if (historyEx == null || historyEx.IsLoading == false)
                return false;

            Data_ActionStepHistory actionStepHistory = historyEx.History;

            SectionTabPage page = FormSOP.Instance.GetPageHome().GetTabPage(nActionStepHistoryID);

            if (page == null)
            {
                LoadActionStepHistory(actionStepHistory);
                page = FormSOP.Instance.GetPageHome().GetTabPage(nActionStepHistoryID);

                if (page == null)
                    return false;
            }

            List<PanelSection> panels = page.GetPanelSections();

            if (panels == null || panels.Count == 0)
                return false;

            Sections.PanelSectionEx panel = (Sections.PanelSectionEx)panels[0];

            DateTime dtBegin = actionStepHistory.BeginTime;
            DisasterInfo disaster = historyEx.Disaster;

            if (disaster == null)
                return false;

            // Section, Section Status
            Dictionary<Sections.Section, int> dicSectionStatus = new Dictionary<Sections.Section, int>();

            ArrayList arrSections4Log = new ArrayList();
            ArrayList arrSectionStatus4Log = new ArrayList();
            ArrayList arrSectionProcessDirections4Log = new ArrayList();
            ArrayList arrDescription = new ArrayList();
            ArrayList arrTask = new ArrayList();
            ArrayList arrTime = new ArrayList();
            // 상황판에 보여줄 것인가?
            ArrayList arrShowBoard = new ArrayList();
            ArrayList arrComponentHistoryID = new ArrayList();
            ArrayList arrCheckedNotify1 = new ArrayList();
            ArrayList arrCheckedNotify2 = new ArrayList();
            ArrayList arrCheckedRun = new ArrayList();
            ArrayList arrCheckedComplete = new ArrayList();
            ArrayList arrAccessedUserID = new ArrayList();

            ArrayList arrAllSections = GetAllPanelSections(panels);

            string strComponentHistoryIDs = "";
            Section selectedSection = null;

            foreach (Data_ComponentHistory componentHistory in componentHistories)
            {
                Sections.Section section = FindSection(componentHistory.ComponentID, componentHistory.ComponentType, arrAllSections);

                if (actionStepHistory.MaxComponentHistoryIDInClient >= componentHistory.ID)
                {
                    if (section != null)
                    {
                        int nStatus = componentHistory.Status;
                        nStatus = nStatus & 0x0000ffff;

                        if (nStatus == (int)UnE.SOP.Workstate.State.RUN)
                            selectedSection = section;
                    }

                    // 이미 읽은 데이터
                    continue;
                }
                else
                    actionStepHistory.MaxComponentHistoryIDInClient = componentHistory.ID;

                if (strComponentHistoryIDs.Length == 0)
                    strComponentHistoryIDs = componentHistory.ID.ToString();
                else
                    strComponentHistoryIDs += ", " + componentHistory.ID.ToString();

                if (section != null)
                {
                    section.CompleteCount = componentHistory.CompleteCount == null ? -1 : componentHistory.CompleteCount.Data;

                    int nStatus = componentHistory.Status;
                    dicSectionStatus[section] = nStatus;

                    int nDirections = nStatus >> 16;
                    nStatus = nStatus & 0x0000ffff;

                    if (nStatus == (int)UnE.SOP.Workstate.State.RUN)
                        selectedSection = section;

                    bool showBoard = false;

                    if (componentHistory.ShowBoard != null)
                        showBoard = componentHistory.ShowBoard.Data == 1;

                    // SOP Log창 기록을 위한 List
                    arrComponentHistoryID.Add(componentHistory.ID);
                    arrSections4Log.Add(section);
                    arrSectionStatus4Log.Add(nStatus);
                    arrSectionProcessDirections4Log.Add(nDirections);
                    arrDescription.Add("");
                    arrTask.Add(componentHistory.Task == null ? "" : componentHistory.Task);
                    arrTime.Add(componentHistory.TimeStamp);
                    arrShowBoard.Add(showBoard);
                    arrCheckedNotify1.Add(componentHistory.CheckedNotify1 == null ? 0 : componentHistory.CheckedNotify1.Data);
                    arrCheckedNotify2.Add(componentHistory.CheckedNotify2 == null ? 0 : componentHistory.CheckedNotify2.Data);
                    arrCheckedRun.Add(componentHistory.CheckedRun == null ? 0 : componentHistory.CheckedRun.Data);
                    arrCheckedComplete.Add(componentHistory.CheckedComplete == null ? 0 : componentHistory.CheckedComplete.Data);
                    arrAccessedUserID.Add(componentHistory.AccessedUserID);
                }
            }

            WorkFlow workFlow = WorkFlowManager.Instance.Get(nActionStepHistoryID);

            if (workFlow == null)
            {
                workFlow = WorkFlowManager.Instance.Add(nActionStepID, arrAllSections, isRealMode);

                if (workFlow == null)
                    return false;

                if (workFlow.Option == null)
                {
                    workFlow.Option = MakeWorkFlowOption(actionStepHistory, disaster);
                    SetOptionText(workFlow.Option, panel);
                }

                workFlow.ActionStepHistoryID = nActionStepHistoryID;
                SOPScenario scenario = SOPScenarioManager.Instance.GetSOPScenario(nActionStepHistoryID);

                if (scenario == null)
                {
                    BarLevelTree tree = SOPScenarioManager.Instance.GetBarLevelTree();
                    TreeNode node = tree.FindActionStepNode(nActionStepID);

                    string strPath = "";

                    if (node != null)
                    {
                        strPath = node.FullPath.Replace('\\', '/');
                    }
                    else
                        strPath = ReadActionStepFullPath(nActionStepID);

                    if (strPath == null)
                        return false;

                    VersionInfo version = FormSOP.Instance.SOPManager.GetActionStepVersionInfo(nActionStepID);
                    bool isRegular = true, isNormal = true;

                    if (version == null)
                    {
                        ReadVersionInfo(disaster.DisasterID, ref isRegular, ref isNormal);
                    }
                    else
                    {
                        isRegular = version.IsRegular;
                        isNormal = version.IsNormal;
                    }

                    scenario = SOPScenarioManager.Instance.AddSOPScenario(strPath, nActionStepID, isRealMode, isRegular, isNormal, nActionStepHistoryID, actionStepHistory.SensorZoneHistoryID, page);
                }
            }

            if (panel.Parent is SectionTabPage)
            {
                workFlow.SetSectionContents(page.SectionContents);
                workFlow.InitSectionContents();
            }

            workFlow.SelectSection(selectedSection);

            workFlow.WorkFlowEvent -= FormSOP.Instance.OnWorkflowChanged;
            workFlow.WorkFlowEvent += FormSOP.Instance.OnWorkflowChanged;

            /*if (workFlow.Option == null)
            {
                string strCategoryName, strSubCategoryName;

                if (FormSOP.Instance.SOPManager.GetDisasterFullPath(disaster, out strCategoryName, out strSubCategoryName))
                    workFlow.Option = SOPMonitoringSystem.Process.WorkFlowStartNotifyProcess.MakeWorkflowOption(strCategoryName, strSubCategoryName);
            }

            if (workFlow.Option == null)
                workFlow.Option = new UnE.SOP.Workstate.WorkflowOption();

            int nArrIndex = arrActionStepID.IndexOf(panel.ActionStepID);

            if (nArrIndex >= 0)
            {
                DateTime dtDetect = (DateTime)arrActionStepDetectTime[nArrIndex];
                workFlow.Option.DetectTime = new VariousData<DateTime>(dtDetect);
            }

            int nSensorZoneHistoryID = -1;
            if (nArrIndex >= 0)
            {
                nSensorZoneHistoryID = (int)arrSensorZoneHistories[nArrIndex];
                workFlow.Option.SensorZoneHistoryID = nSensorZoneHistoryID;
            }*/

            // Key : ComponentHistory ID
            Dictionary<int, List<HistorySectionData.DetailData>> dicDetailDatas = GetComponentHistoryDetails(componentHistories);

            AddSOPSectionLog(panel.ActionStepID, nActionStepHistoryID, arrComponentHistoryID, arrSections4Log, arrSectionStatus4Log, arrSectionProcessDirections4Log, arrTask, arrTime, arrDescription, arrShowBoard, arrCheckedNotify1, arrCheckedNotify2, arrCheckedRun, arrCheckedComplete, arrAccessedUserID, isRealMode, dicDetailDatas, workFlow);

            // ActionStep ID, Disaster
            /*Dictionary<int, DisasterInfo> dicDisaster = new Dictionary<int, DisasterInfo>();
            dicDisaster[nActionStepID] = disaster;

            Dictionary<DisasterInfo, string> dicDisasterFullPath = FormSOP.Instance.SOPManager.GetFullPathDictionary();

            BarLevelTree tree = GetBarLevelTree();
            TreeNode node = tree.FindActionStepNode(panel.ActionStepID);
            string szPath = node == null ? GetActionStepFullPath(panel.ActionStepID, dicDisaster, dicDisasterFullPath) : node.FullPath;
            bool bHasPos = true;
            if (szPath.IndexOf("자연재해") != -1 || szPath.IndexOf("태풍") != -1)
            {
                bHasPos = false;
            }

            string sopName = szPath.Substring(szPath.IndexOf("\\") + 1);
            workFlow.Option.HasPosition = bHasPos;

            HistoryDisasterNoPosition info = HistoryManager.Instance.FindHistoryDisasterNoPosition(panel.ActionStepID, isRealMode);

            if (info != null)
                workFlow.Option.SetDisasterOptions(info.DisasterOptions);

            if (bHasPos == true)
            {
                HistoryDisasterPosition pos = HistoryManager.Instance.FindHistoryDisasterPosition(panel.ActionStepID, isRealMode);
                if (pos != null)
                {
                    workFlow.Option.LastPosition = pos;
                    workFlow.Option.PositionName = pos.PoistionName;
                }
            }
            else
            {
                if (info != null && workFlow.Option is UnE.SOP.Workstate.WorkflowOptionSnowFall)
                {
                    UnE.SOP.Workstate.WorkflowOptionSnowFall option = (UnE.SOP.Workstate.WorkflowOptionSnowFall)workFlow.Option;
                    option.UseAmountSnowFall = info.UseAmountSnowfall;

                    if (info.UseAmountSnowfall)
                    {
                        double dAmountSnowFall;

                        if (double.TryParse(info.AmountSnowfall.Trim(), out dAmountSnowFall))
                            option.AmountSnowFall = dAmountSnowFall;
                    }
                }
            }

            workFlow.SOPName = sopName;
            workFlow.State = WorkFlowState.RUN;
            FormSOP.Instance.SetCurrentWorkflow(workFlow);*/

            int nSectionCount = arrAllSections.Count;

            Dictionary<SectionState, Sections.Section> dicDup = new Dictionary<SectionState, Section>();

            for (int i = 0; i < nSectionCount; i++)
            {
                Sections.Section section = (Sections.Section)arrAllSections[i];

                // add by skkim : 2013-01-07 링크 노드 상태 세팅 제외
                if (section.GetComponentType() == Sections.Section.ComponentType.LINK ||
                           section.GetComponentType() == Section.ComponentType.ANNOTATION ||
                          section.GetComponentType() == Section.ComponentType.GROUP)

                    continue;

                int nStatus = dicSectionStatus.ContainsKey(section) ? dicSectionStatus[section] : 1/*대기상태*/;
                int nDirection = nStatus >> 16;

                nStatus = nStatus & 0x0000ffff;

                // changed by skkim : 2013-01-07 링크노드 상태 세팅 제외
                //Sections.SectionState state = workFlow.FindState(section, true);
                SectionState state = workFlow.FindState(section, false);
                if (state == null)
                    continue;

                if (nStatus == 2)
                {
                    //state.InProgress();
                    state.CopyState(State.RUN, WorkFlowManager.Instance.InProgressColor);
                    ArrayList arList = workFlow.FindNext(state);
                    foreach (SectionState next in arList)
                    {
                        if (next != null)
                        {
                            if (dicDup.ContainsKey(next) || next == state)
                            {
                                continue;
                            }

                            next.CopyState(State.NORMAL, WorkFlowManager.Instance.NoramlColor);
                            //next.CopyState(State.INPUT, WorkFlowManager.Instance.InputWaitColor);
                            //next.InputWait();
                        }
                    }

                    if (!dicDup.ContainsKey(state))
                    {
                        dicDup.Add(state, section);
                    }
                }
                else if (nStatus == 3)
                {
                    state.CopyState(State.DONE, WorkFlowManager.Instance.CompleteColor);
                    state.ProcessDirections = nDirection;
                    if (!dicDup.ContainsKey(state))
                    {
                        dicDup.Add(state, section);
                    }
                }
                else if (nStatus == 4)
                {
                    state.CopyState(State.INPUT, WorkFlowManager.Instance.InputWaitColor);
                    if (!dicDup.ContainsKey(state))
                    {
                        dicDup.Add(state, section);
                    }
                }
                else if (nStatus == 5)
                {
                    state.CopyState(State.SKIP, WorkFlowManager.Instance.SkipColor);
                    if (!dicDup.ContainsKey(state))
                    {
                        dicDup.Add(state, section);
                    }
                }

                //if (state != null)
                //    state.DetailDatas.Clear();
            }

            /*if (dicDisasterFullPath.ContainsKey(disaster))
            {
                string strDisasterPath = dicDisasterFullPath[disaster];
                string strActionStepPath = GetActionStepPath(disaster.ActionSteps, panel.ActionStepID);

                if (strActionStepPath.Length == 0)
                    return false;
                strDisasterPath = strDisasterPath.Replace((char)0x06, '/');

                AddSOPScenario(strDisasterPath + '/' + strActionStepPath, panel.ActionStepID, isRealMode, isRegular, isNormal, nActionStepHistoryID, nSensorZoneHistoryID);
            }*/

            return true;
        }

        private void SetOptionText(WorkflowOption option, PanelSectionEx panel)
        {
            if (option == null)
                return;

            panel.SetWorkflowOption(option);
            /*PSMMaterial material = null;

            if (option is WorkflowOptionPSM)
            {
                WorkflowOptionPSM optionPSM = (WorkflowOptionPSM)option;
                material = optionPSM.PSMMaterial;
            }

            if (material != null)
                panel.SetInfoText(option.PositionName, option.DetectTime.Data.ToString(), material.MaterialName);
            else
                panel.SetInfoText(option.PositionName, option.DetectTime.Data.ToString());*/
        }

        private bool ReadVersionInfo(int nDisasterID, ref bool isRegular, ref bool isNormal)
        {
            string strSQL = "Select v.isRegular, v.isRegular from Disaster as d, Version as v where d.VersionID = v.ID and d.ID = " + nDisasterID.ToString();
            ArrayList arrResult = FormSOP.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count < 2)
                return false;

            VariousData<int> regular = WebDBManager.GetIntField(arrResult[0].ToString());
            VariousData<int> normal = WebDBManager.GetIntField(arrResult[1].ToString());

            if (regular == null || normal == null)
                return false;

            isRegular = regular.Data == 1;
            isNormal = normal.Data == 1;
            return true;
        }

        private WorkflowOption MakeWorkFlowOption(Data_ActionStepHistory actionStepHistory, DisasterInfo disaster)
        {
            string strCategoryName, strSubCategoryName;

            if (FormSOP.Instance.SOPManager.GetDisasterFullPath(disaster, out strCategoryName, out strSubCategoryName))
            {
                WorkflowOption option = SOPMonitoringSystem.Process.WorkFlowStartNotifyProcess.MakeWorkflowOption(strCategoryName, strSubCategoryName);

                if (option != null)
                {
                    if (actionStepHistory.Position != null)
                    {
                        option.HasPosition = true;
                        option.PositionName = actionStepHistory.Position;
                    }

                    option.SensorZoneHistoryID = actionStepHistory.SensorZoneHistoryID;
                    option.DetectTime = new VariousData<DateTime>(actionStepHistory.DetectTime);
                    option.SetDisasterOptions(actionStepHistory.HistoryDisasterNoPositionInfo.DisasterOptions);
                    return option;
                }
            }

            return null;
        }

        private string ReadActionStepFullPath(int nActionStepID)
        {
            string strSQL = "Select dc.CategoryName, sdc.SubCategoryName, d.DisasterName, _as.StepName ";
            strSQL += "from ActionStep as _as, Disaster as d, SubDisasterCategory as sdc, DisasterCategory as dc ";
            strSQL += "where _as.DisasterID = d.ID and d.SubDisasterID = sdc.ID and sdc.DisasterID = dc.ID and _as.ID = " + nActionStepID.ToString();

            ArrayList arrResult = FormSOP.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;

            if (nResultCount < 4)
                return null;

            string strDisasterCategoryName = WebDBManager.GetStringField(arrResult[0]);
            string strSubDisasterCategoryName = WebDBManager.GetStringField(arrResult[1]);
            string strDisasterName = WebDBManager.GetStringField(arrResult[2]);
            string strActionStepName = WebDBManager.GetStringField(arrResult[3]);

            if (strDisasterCategoryName == null || strSubDisasterCategoryName == null || strDisasterName == null || strActionStepName == null)
                return null;

            return strDisasterCategoryName + "/" + strSubDisasterCategoryName + "/" + strDisasterName + "/" + strActionStepName;
        }

        // Key : ComponentHistory ID
        private Dictionary<int, List<HistorySectionData.DetailData>> GetComponentHistoryDetails(List<Data_ComponentHistory> componentHistories)
        {
            Dictionary<int, List<HistorySectionData.DetailData>> dicDetailDatas = new Dictionary<int, List<HistorySectionData.DetailData>>();

            foreach (Data_ComponentHistory componentHistory in componentHistories)
            {
                List<HistorySectionData.DetailData> details = new List<HistorySectionData.DetailData>();
                dicDetailDatas[componentHistory.ID] = details;

                foreach (Data_ComponentHistoryDetail detailData in componentHistory.DetailDatas)
                {
                    HistorySectionData.DetailData detail = new HistorySectionData.DetailData();

                    detail.ComponentHistoryID = componentHistory.ID;
                    detail.DataIndex = new VariousData<int>(detailData.DataIndex);
                    detail.Datai = detailData.Datai;
                    detail.Dataf = detailData.Dataf;
                    detail.Datas = detailData.Datas;
                    detail.Time = detailData.TimeStamp;

                    details.Add(detail);
                }
            }

            return dicDetailDatas;
        }

        public void AddNewComponentHistories(int nActionStepHistoryID, List<Data_ComponentHistory> componentHistories)
        {
            ActionStepHistoryEx historyEx;

            if (m_dicActionStepHistory.TryGetValue(nActionStepHistoryID, out historyEx))
            {
                foreach (Data_ComponentHistory componentHistory in componentHistories)
                {
                    if (historyEx.NewComponentHistories.ContainsKey(componentHistory.ID))
                        continue;

                    historyEx.NewComponentHistories[componentHistory.ID] = componentHistory;
                }
            }
        }

        // 새로운 ComponentHistory를 리턴한다.
        // 리턴한 데이터는 모두 지운다.
        public List<Data_ComponentHistory> PopNewComponentHistory(int nActionStepHistoryID)
        {
            List<Data_ComponentHistory> componentHistories = new List<Data_ComponentHistory>();
            ActionStepHistoryEx historyEx;

            if (m_dicActionStepHistory.TryGetValue(nActionStepHistoryID, out historyEx))
            {
                Data_ComponentHistory history;
                List<int> componentHistoryIDs = historyEx.NewComponentHistories.Keys.ToList();

                foreach (int nComponentHistoryID in componentHistoryIDs)
                {
                    if (historyEx.NewComponentHistories.TryGetValue(nComponentHistoryID, out history))
                        componentHistories.Add(history);
                }

                foreach (int nComponentHistoryID in componentHistoryIDs)
                {
                    historyEx.NewComponentHistories.TryRemove(nComponentHistoryID, out history);
                }

                componentHistories.Sort();
            }

            return componentHistories;
        }

        public void SetSOPControl(int nActionStepHistoryID, int nSOPGenUserID)
        {
            ActionStepHistoryEx historyEx;

            if (m_dicActionStepHistory.TryGetValue(nActionStepHistoryID, out historyEx))
            {
                historyEx.AccessedUserID = nSOPGenUserID;
            }
        }

        public int GetSOPControlUserID(int nActionStepHistoryID)
        {
            ActionStepHistoryEx historyEx;

            if (m_dicActionStepHistory.TryGetValue(nActionStepHistoryID, out historyEx))
            {
                return historyEx.AccessedUserID;
            }

            return -1;
        }

        // 실행중인 모든 ActionStepHistory의 제어권 정보를 초기화한다.
        public void ClearSOPControls()
        {
            ActionStepHistoryEx historyEx;
            List<int> actionStepHistoryIDs = m_dicActionStepHistory.Keys.ToList();

            foreach (int nActionStepHistoryID in actionStepHistoryIDs)
            {
                if (m_dicActionStepHistory.TryGetValue(nActionStepHistoryID, out historyEx))
                {
                    historyEx.AccessedUserID = -1;
                }
            }
        }

        public void GetControlActionStepIDList(int nSOPGenUserID, List<int> controlActionStepHistoryIDs)
        {
            ActionStepHistoryEx historyEx;
            List<int> actionStepHistoryIDs = m_dicActionStepHistory.Keys.ToList();

            foreach (int nActionStepHistoryID in actionStepHistoryIDs)
            {
                if (m_dicActionStepHistory.TryGetValue(nActionStepHistoryID, out historyEx))
                {
                    if (historyEx.AccessedUserID == nSOPGenUserID)
                        controlActionStepHistoryIDs.Add(nActionStepHistoryID);
                }
            }
        }

        public void FinishActionStepHistory(int nActionStepHistoryID, DateTime dtEnd)
        {
            ActionStepHistoryEx historyEx;

            if(m_dicActionStepHistory.TryGetValue(nActionStepHistoryID, out historyEx))
            {
                historyEx.History.EndTime = new VariousData<DateTime>(dtEnd);
            }
        }

        public void SelectSOPComponent(int nActionStepHistoryID, string strComponentType, int nComponentID)
        {
            ActionStepHistoryEx historyEx;
            strComponentType = strComponentType.ToLower();

            if (m_dicActionStepHistory.TryGetValue(nActionStepHistoryID, out historyEx))
            {
                SectionTabPage tabPage = FormSOP.Instance.GetPageHome().GetTabPage(historyEx.History.ID);

                if (tabPage != null)
                {
                    List<PanelSection> panels = tabPage.GetPanelSections();

                    foreach (PanelSection panel in panels)
                    {
                        foreach (Section section in panel.Sections)
                        {
                            if (section.GetComponentType().ToString().ToLower() == strComponentType && section.Data.ID == nComponentID)
                            {
                                FormSOP.Instance.Invoke((MethodInvoker)delegate
                                {
                                    ((Sections.PanelSectionEx)panel).FocusSection(section);
                                    panel.SelectSection(section);
                                    return;
                                });
                            }
                        }
                    }
                }
            }
        }
    }
}
