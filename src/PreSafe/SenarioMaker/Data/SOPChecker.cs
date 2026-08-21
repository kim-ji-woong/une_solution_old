using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.Utility;

namespace UnE.SenarioMaker
{
    internal delegate void OnCheckFailed(SectionScriptFailEventArg e);
    internal class SectionScriptFailEventArg
    {
        public SectionScriptFailEventArg(Sections.PanelSection panel, Sections.Section section, string szScript, Exception ex)
        {
            mSection = section;
            mPanel = panel;
            m_szScript = szScript;
            m_Excpetion = ex;
 
        }
        private Sections.Section mSection = null;
        public Sections.Section Section
        {
            get { return mSection; }
        }

        private string m_szScript = "";
        public string Script
        {
            get { return m_szScript; }
        }
        private Exception m_Excpetion = null;
        public Exception Excpetion
        {
            get { return m_Excpetion; }
        }

        private Sections.PanelSection mPanel = null;
        public Sections.PanelSection Panel
        {
            get { return mPanel; }

        }

    }

    internal class SOPChecker : IDisposable
    {
		private bool m_bSaved = true;
        public event OnCheckFailed OnChecFailed;
        public void Dispose()
        {

        }

        public SOPChecker(bool bSaved)
        {
			m_bSaved = bSaved;
            //OnChecFailed += SectionExpressionCheckFail;
        }

        private void UnRegisterVariables()
        {
            IEnumerable<UserVariable> list = mManager.UserVariables.VarList;
            foreach (UserVariable var in list)
            {
                string szScript = "del " + var.Name;
                ScriptProxy.Instance.RunPythonScript(szScript);  
            }
            IEnumerable<Enums> list2 = mManager.EnumList.VarList;
            foreach (Enums var in list2)
            {
                string szScript = "del " + var.Name;
                ScriptProxy.Instance.RunPythonScript(szScript);  
            }
            IEnumerable<Variable> list3 = mManager.SystemVariables.VarList;
            foreach (Variable var in list3)
            {
                string szScript = "del " + var.Name;
                ScriptProxy.Instance.RunPythonScript(szScript);                
            }
        }

        private void RegisterVariables()
        {
            IEnumerable<UserVariable> list = mManager.UserVariables.VarList;
            foreach (UserVariable var in list)
            {
                if (var.Value != null)
                {
                    string szScript = var.Name + "=" + var.ToStringValue();
                    ScriptProxy.Instance.RunPythonScript(szScript);                    
                }
                else
                {
                    string szScript = var.Name + "=0";
                    ScriptProxy.Instance.RunPythonScript(szScript);   
                }
            }
            IEnumerable<Enums> list2 = mManager.EnumList.VarList;
            foreach (Enums var in list2)
            {
                if (var.Value != null)
                {
                    string szScript = var.Name + "=" + var.ToStringValue();
                    ScriptProxy.Instance.RunPythonScript(szScript);    
                }
                else
                {
                    string szScript = var.Name + "=0";
                    ScriptProxy.Instance.RunPythonScript(szScript);
                }
            }
            IEnumerable<Variable> list3 = mManager.SystemVariables.VarList;
            foreach (Variable var in list3)
            {
                if (var.Value != null)
                {
                    string szScript = var.Name + "=" + var.ToStringValue();
                    ScriptProxy.Instance.RunPythonScript(szScript);
                }
                else
                {
                    string szScript = var.Name + "=0";
                    ScriptProxy.Instance.RunPythonScript(szScript);
                }
            }
        }

        private SenarioManager mManager = null;
        private Dictionary<string, string> m_Funcion = new Dictionary<string, string>();

        public bool CheckExpression(SenarioManager manager, bool bSaved = true)
        {
            bool bResult = false;
            mManager = manager;


            RegisterVariables();

            ArrayList arList = manager.ActionStepList;

			string szTitle = "저장오류";
			string szMessage = "확인후 저장하십시오.";
			if (bSaved == false)
			{
				szTitle = "검증 오류";
				szMessage = "확인후 다시 검증하십시오.";
			}

            foreach (ActionStep actionStep in arList)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("def {0}():", actionStep.StepName);
                sb.AppendLine("");
                sb.AppendLine("\tzzz=1");
                try
                {
                    ScriptProxy.Instance.RunPythonScript(sb.ToString());
                }
                catch(Exception)
                {
                    UnRegisterVariables();
					UMessageBox.Show("함수이름에 오류가 있습니다.\r\n" + szMessage, szTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                
            }


            foreach (ActionStep step in arList)
            {
                Sections.PanelSectionEx panel = step.Panel;
                if (panel == null)
                    continue;
                
                string szStepName = panel.StepName;                
                foreach (Sections.Section section in panel.Sections)
                {                    
                    if (!CheckExpr(section, panel))
                        return false;                                    
                }
            }

            UnRegisterVariables();
            bResult = true;
            return bResult;
        }

        private bool RunScript(Sections.PanelSection panel, Sections.Section section, string szScript)
        {
            if (szScript == "")
            {
                return true;
            }

            try
            {
                ScriptProxy.Instance.RunPythonScript(szScript);
                return true;
            }
            catch (Exception ex)
            {
                if (OnChecFailed != null)
                {
                    SectionScriptFailEventArg args = new SectionScriptFailEventArg(panel, section, szScript, ex);
                    OnChecFailed(args);
                }
            }
            return false;
        }

        private bool CheckExpr(Sections.Section section, Sections.PanelSection panel)
        {
            Sections.Section.ComponentType type = section.GetComponentType();
            if (type == Sections.Section.ComponentType.PROCESS)
            {
                Sections.SectionDataProcess data = (Sections.SectionDataProcess)section.Data;

				string szTemp = data.Expression.Replace("&&", "and");
				szTemp = szTemp.Replace("||", "or");
				szTemp = szTemp.Trim();
				return RunScript(panel, section, szTemp);                              
            }
            else if(type == Sections.Section.ComponentType.DECISION)
            {
                Sections.SectionDataDecision data = (Sections.SectionDataDecision)section.Data;
				string szTemp = data.Expression.Replace("&&", "and");
				szTemp = szTemp.Replace("||", "or");
				szTemp = szTemp.Trim();
				return RunScript(panel, section, szTemp); 
            }            
            return true;           
        }

        public bool CheckSOP(  SenarioManager manager , bool mbSaved = true )
        {
            bool bResult = false;

            ArrayList arList = manager.ActionStepList;
            foreach (ActionStep step in arList)
            {
                Sections.PanelSectionEx panel = step.Panel;
                if (panel == null)
                    continue;

                int nStart = 0, nEnd = 0;
                string szStepName = panel.StepName;
                string szHeader = string.Format("[{0}] - ", szStepName);
                foreach (Sections.Section section in panel.Sections)
                {
                    Sections.Section.ComponentType type = section.GetComponentType();

                    if (type == Sections.Section.ComponentType.PROCESS)
                    {
                        if (!CheckProcess((Sections.SectionProcess)section))
                            return false;
                    }
                    else if (type == Sections.Section.ComponentType.ENDPOINT)
                    {
                        PrepareCheckEndPoint((Sections.SectionEndPoint)section, ref nStart, ref nEnd);
                    }
                }
                
                bResult = true;

				string szTitle = "저장오류";
				string szMessage = "확인후 저장하십시오.";
				if (mbSaved == false)
				{
					szTitle = "검증 오류";
					szMessage = "확인후 다시 검증하십시오.";
				}


                if (nStart == 0)
                {
					UMessageBox.Show(szHeader + "[시작] 태그가 없습니다.\r\n" + szMessage, szTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    bResult = false;
                }
                else if (nStart > 1)
                {
					UMessageBox.Show(szHeader + string.Format("[시작] 태그가 {0}개 존재합니다.\r\n[시작] 태그는 반드시 하나만 존재하여야 합니다.\r\n" + szMessage, nStart), szTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    bResult = false;
                }
                if (nEnd == 0)
                {
					UMessageBox.Show(szHeader + "[종료] 태그가 없습니다.\r\n" + szMessage, szTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    bResult = false;
                }
                if (bResult == false)
                    return false;
            }

            if (!CheckExpression(manager, mbSaved))
            {
              
                return false;
            }

            return bResult;
        }

        private void SectionExpressionCheckFail(SectionScriptFailEventArg e)
        {

        }

        private void PrepareCheckEndPoint(Sections.SectionEndPoint section, ref int nStart, ref int nEnd)
        {
            Sections.SectionDataEndPoint data = (Sections.SectionDataEndPoint)section.Data;

            if (data.IsBegin)
                nStart++;
            else
                nEnd++;
        }

        private bool CheckProcess(Sections.SectionProcess section)
        {
            Sections.SectionDataProcess data = (Sections.SectionDataProcess)section.Data;

            //if (data.TeamList.Count < 0)
            //{
                //MessageBox.Show("임무를 수행할 대상이 지정되지 않은 [프로세스] 태그가 존재합니다.\r\n확인후 저장하십시오.");
               // ZoomNSelectSection(section);
               // return false;
           // }

            return true;
        }

    }

    
}
