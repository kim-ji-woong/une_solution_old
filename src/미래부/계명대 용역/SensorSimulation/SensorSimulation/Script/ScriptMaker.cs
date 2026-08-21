using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Text;

namespace SensorSimulation
{
    internal class ScriptMaker
    {
        private ScenarioManager mScenarioManager = null;
        
        private string m_szScriptResult = "";
        public string ScriptResult
        {
            get
            {
                return m_szScriptResult;
            }
        }

        internal ScriptMaker(ScenarioManager sMgr)
        {
           
            mScenarioManager = sMgr;

            m_arScriptList.Clear();

            m_arProcessSection.Clear();

            // Variables => variables
            RegisterVariables();
            // ActionStep -> team Name == 'Main; -> Main Function
            RegisterSub();
            // ActionStep -> team Name == 'Sub' -> Sub Function

            
        }
        private Dictionary<string, string> m_Funcion = new Dictionary<string, string>();

        public void RegisterSub()
        {
            

            ArrayList arList = mScenarioManager.ActionStepList;
            ActionStep mainActionStep = null;

            foreach (ActionStep actionStep in arList)
            {
                ArrayList arSections = actionStep.SectionList;
                foreach (ScriptSection section in arSections)
                {
                    if (section.Type == ScriptSection.ScriptType.End && section.BeginSection == true)
                    {
						if( actionStep.TeamName == "Main")
						{
							m_Funcion.Add("Main()", section.TargetComponent + "()");
						}
						else
							m_Funcion.Add(actionStep.StepName + "()", section.TargetComponent + "()");
                        break;
                    }
                }               
            }

            foreach(ActionStep actionStep in arList)
            {
                if( actionStep.TeamName == "Main")
                {
                    mainActionStep = actionStep;
                    
                }
                else
                {
                    RegisterSub(actionStep);
                }
            }
            RegisterMain(mainActionStep);
        }

        private void RegisterMain(ActionStep actionStep)
        {
            ArrayList arSections = actionStep.SectionList;
            ScriptSection sectionStart = null;
            foreach (ScriptSection section in arSections)
            {
                if (section.Type == ScriptSection.ScriptType.End && section.BeginSection == true)
                {
                    sectionStart = section;
                    break;
                }
            }

            AddScript(sectionStart);
        }

        public ArrayList m_arScriptList = new ArrayList();

        private void RegisterSub(ActionStep actionStep)
        {

            ArrayList arSections = actionStep.SectionList;
            ScriptSection sectionStart = null;
            foreach (ScriptSection section in arSections)
            {
                if (section.Type == ScriptSection.ScriptType.End && section.BeginSection == true)
                {
                    sectionStart = section;
                    break;
                }
            }

            AddScript(sectionStart);
        }

        private void AddScript(ScriptSection section)
        {
            if (section == null)
                return;

            if (m_arProcessSection.Contains(section))
                return;

            string szScript = MakeScript(section);
            if( szScript != "")
                m_arScriptList.Add(szScript);
            
            m_arProcessSection.Add(section);

            ArrayList arList = section.GetNextSection();
            foreach (ScriptSection subSection in arList)
            {
                if (!m_arProcessSection.Contains(subSection))
                    AddScript(subSection);
            }
        }
        public ArrayList RegisterGlobarVariables()
        {
            ArrayList arResult = new ArrayList();
            IEnumerable<UserVariable> list = mScenarioManager.UserVariables.VarList;
            foreach (UserVariable var in list)
            {
				if (var.ToStringValue() != "")
				{
					string szScript = "global " + var.Name;
					arResult.Add(szScript);
				}
            }
            IEnumerable<Enums> list2 = mScenarioManager.EnumList.VarList;
            foreach (Enums var in list2)
            {
				if (var.ToStringValue() != "")
				{
					string szScript = "global " + var.Name;
					arResult.Add(szScript);
				}

            }
            IEnumerable<Variable> list3 = mScenarioManager.SystemVariables.VarList;
            foreach (Variable var in list3)
            {
				if (var.ToStringValue() != "")
				{

					string szScript = "global " + var.Name;
					arResult.Add(szScript);
				}
            }
            return arResult;
        }
        public void RegisterVariables()
        {
            IEnumerable<UserVariable> list = mScenarioManager.UserVariables.VarList;
            foreach (UserVariable var in list)
            {
                if (var.ToStringValue() != "")
                {
                    string szScript = var.Name + "=" + var.ToStringValue();
                    m_arScriptList.Add(szScript);
                }
				else
				{
					string szScript = var.Name + "=0";
					m_arScriptList.Add(szScript);
					szScript = "del "+var.Name;
					m_arScriptList.Add(szScript);
				}
            }
            IEnumerable<Enums> list2 = mScenarioManager.EnumList.VarList;
            foreach (Enums var in list2)
            {
                if (var.ToStringValue() != "")
                {
                    string szScript = var.Name + "=" + var.ToStringValue();
                    m_arScriptList.Add(szScript);
                }
				else
				{
					string szScript = var.Name + "=0";
					m_arScriptList.Add(szScript);
					szScript = "del " + var.Name;
					m_arScriptList.Add(szScript);
				}
            }
            IEnumerable<Variable> list3 = mScenarioManager.SystemVariables.VarList;
            foreach (Variable var in list3)
            {
                if(var.ToStringValue() != "")
                {
                    string szScript = var.Name + "=" + var.ToStringValue();
                    m_arScriptList.Add(szScript);
                }
				else
				{
					string szScript = var.Name + "=0";
					m_arScriptList.Add(szScript);
					szScript = "del " + var.Name;
					m_arScriptList.Add(szScript);
				}
               
            }
        }
        private  ArrayList m_arProcessSection = new ArrayList();
        public virtual string MakeScript(ScriptSection section)
        {
            if (section.Type == ScriptSection.ScriptType.None)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();

            sb.Append("def ");
            sb.Append(section.TargetComponent);
            sb.Append("()");
            sb.AppendLine(":");

            if(section.Type == ScriptSection.ScriptType.End && section.BeginSection == false)
            {
                sb.Append("\t").AppendLine("zzz = 1");
                return sb.ToString();
            }

            
            ArrayList arGlobals = RegisterGlobarVariables();
            foreach (string global in arGlobals)
            {
                sb.Append("\t").AppendLine(global);
            }
            

            foreach (ScriptSectionLink link in section.LinkList)
            {
                if (section.Type == ScriptSection.ScriptType.Decision)
                {                  
                    sb.Append("\t").Append("tempBool = (").Append(section.Script).AppendLine(")");

                    if (link.Type == ScriptSectionLink.LinkType.Yes)
                    {
                        sb.AppendLine("\tif tempBool == True :");

                        if (link.EndSection.Type != ScriptSection.ScriptType.End && link.EndSection.Type != ScriptSection.ScriptType.None)
                        {
                            sb.Append("\t\t").Append(link.EndSection.TargetComponent).AppendLine("()");
                        }
                        else
                        {
                            sb.Append("\t\t").AppendLine("zzz = 1");
                        }
                    }
                    else if (link.Type == ScriptSectionLink.LinkType.No)
                    {
                        sb.AppendLine("\tif tempBool == False :");

                        if (link.EndSection.Type != ScriptSection.ScriptType.End && link.EndSection.Type != ScriptSection.ScriptType.None)
                        {
                            sb.Append("\t\t").Append(link.EndSection.TargetComponent).AppendLine("()");
                        }                        
                        else
                        {
                            sb.Append("\t\t").AppendLine("zzz = 1");
                        }
                    }
                    else
                    {
                        string szScript = section.Script;
                        char[] sep = { '\r','\n' };
                        string[] scripts = szScript.Split(sep);

                        for (int i = 0; i < scripts.Length; i++)
                        {
                            if (scripts[i] != null && scripts[i] != "")
                            {
                                if (m_Funcion.ContainsKey(scripts[i]))
                                {
                                    sb.Append("\t").AppendLine(m_Funcion[scripts[i]]);
                                }
                                else
                                    sb.Append("\t").AppendLine(scripts[i]);
                            }
                        }
                        if (link.EndSection.Type != ScriptSection.ScriptType.End && link.EndSection.Type != ScriptSection.ScriptType.None)
                        {
                            sb.Append("\t").Append(link.EndSection.TargetComponent).AppendLine("()");
                        }
                        else
                        {                           
                            sb.Append("\t").AppendLine("zzz = 1");                       
                        }
                    }
                }
                else
                { 
                    if (section.Script != null && section.Script != "")
                    {    
                        string szScript = section.Script;
                        char[] sep = { '\r', '\n' };
                        string[] scripts = szScript.Split(sep);

                        for (int i = 0; i < scripts.Length; i++)
                        {
                            if (scripts[i] != null && scripts[i] != "")
                            {
                                if (m_Funcion.ContainsKey(scripts[i]))
                                {
                                    sb.Append("\t").AppendLine(m_Funcion[scripts[i]]);
                                }
                                else
                                    sb.Append("\t").AppendLine(scripts[i]);
                            }
                        }
                    }

                    if (link.EndSection.Type != ScriptSection.ScriptType.None)
                    {
                        sb.Append("\t").Append(link.EndSection.TargetComponent).AppendLine("()");
                    }

                }
            }

            sb.AppendLine("");

            return sb.ToString();
        }
        
        public string RunScript(string strVariableName, string strInitValue)
        {
            StringBuilder sb = new StringBuilder();

            foreach(string szScript in m_arScriptList)
            {
                sb.AppendLine(szScript);
            }


            string szMainCall = m_Funcion["Main()"];
            sb.AppendLine(szMainCall);

            try
            {
                ScriptProxy.Instance.RunPythonScript(strVariableName + "=" + strInitValue);
				//ScriptProxy.Instance.RunPythonScript("CR=0");
                ScriptProxy.Instance.RunPythonScript(sb.ToString());
                m_szScriptResult = ScriptProxy.Instance.Result;
            }
            catch(Exception e)
            {
                m_szScriptResult = e.Message;
                return "ERROR";
            }

            object obj = ScriptProxy.Instance.CallAction(strVariableName);
            m_szScriptResult = obj.ToString();
           
            return sb.ToString();
        }
    }
}