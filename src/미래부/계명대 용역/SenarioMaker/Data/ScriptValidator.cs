using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Windows.Forms;
using System.Text;

namespace UnE.SenarioMaker
{
    internal class ScriptValidator
    {
        private SenarioManager mSenarioManager = null;
        
        private string m_szScriptResult = "";
        public string ScriptResult
        {
            get
            {
                return m_szScriptResult;
            }
        }

        public ArrayList m_arScriptList = new ArrayList();

        private ArrayList m_arProcessSection = new ArrayList();

        private Dictionary<string, string> m_Funcion = new Dictionary<string, string>();

        internal ScriptValidator(SenarioManager sMgr)
        {
           
            mSenarioManager = sMgr;

            m_arScriptList.Clear();

            m_arProcessSection.Clear();

            // Variables => variables
            RegisterVariables();
            // ActionStep -> team Name == 'Main; -> Main Function
            RegisterSub();
            // ActionStep -> team Name == 'Sub' -> Sub Function

            
        }


        private void RegisterSub()
        {   
            ArrayList arList = mSenarioManager.ActionStepList;
            ActionStep mainActionStep = null;

            foreach (ActionStep actionStep in arList)
            {
                ArrayList arSections = actionStep.Panel.Sections;

                foreach (Sections.Section section in arSections)
                {
                    if (section.GetComponentType() == Sections.Section.ComponentType.ENDPOINT)
                    {
                        Sections.SectionDataEndPoint data = (Sections.SectionDataEndPoint)section.Data;
                        if( data != null && data.IsBegin == true)
                        {
                            string szMainName = actionStep.TeamName + actionStep.ID.ToString();
                            if (actionStep.TeamName == "Main")
                            {
                                szMainName = "Main";
                            }
                            else
                            {
                                szMainName = actionStep.StepName;
                            }
                            m_Funcion.Add(szMainName + "()", CheckString(data.ComponentID) + "()");
                            break;
                        }                       
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
                    RegisterActionStep(actionStep);
                }
            }
            RegisterActionStep(mainActionStep);
        }

        private void RegisterActionStep(ActionStep actionStep)
        {
            ArrayList arSections = actionStep.Panel.Sections;
            Sections.Section sectionStart = null;
            foreach (Sections.Section section in arSections)
            {
                if (section.GetComponentType() == Sections.Section.ComponentType.ENDPOINT)
                {
                    Sections.SectionDataEndPoint data = (Sections.SectionDataEndPoint)section.Data;
                    if (data != null && data.IsBegin == true)
                    {
                        sectionStart = section;
                        break;
                    }
                }
            } 
            AddScript(sectionStart);
        }     

        private void AddScript(Sections.Section section)
        {
            if (section == null)
                return;

            if (m_arProcessSection.Contains(section))
                return;

            string szScript = MakeScript(section);
            if( szScript != "")
                m_arScriptList.Add(szScript);
            
            m_arProcessSection.Add(section);

            ArrayList arList = GetNextSection(section);
            foreach (Sections.Section subSection in arList)
            {
                if (!m_arProcessSection.Contains(subSection))
                    AddScript(subSection);
            }
        }

        private ArrayList GetNextSection(Sections.Section section)
        {
            ArrayList arResult = new ArrayList();
            foreach (Sections.Arrow link in section.Arrows)
            {
                if( link.BeginLink == section)
                {
                    arResult.Add(link.EndLink);
                }                
            }
            return arResult;
        }

        private ArrayList RegisterGlobarVariables()
        {
            ArrayList arResult = new ArrayList();
            IEnumerable<UserVariable> list = mSenarioManager.UserVariables.VarList;
            foreach (UserVariable var in list)
            {
                string szScript = "global " + var.Name;
                arResult.Add(szScript);                
            }
            IEnumerable<Enums> list2 = mSenarioManager.EnumList.VarList;
            foreach (Enums var in list2)
            {
                string szScript = "global " + var.Name;
                arResult.Add(szScript);      

            }
            IEnumerable<Variable> list3 = mSenarioManager.SystemVariables.VarList;
            foreach (Variable var in list3)
            {
                string szScript = "global " + var.Name;
                arResult.Add(szScript); 
            }
            return arResult;
        }

        private string CheckString(string szInput)
        {
            string szResult = szInput.Trim();
            szResult = szResult.Replace("\n\r", "");
            szResult = szResult.Replace(" ", "_00_");
            szResult = szResult.Replace("\t", "_00_");
            return szResult;
        }
		public void RegisterVariables()
		{
			IEnumerable<UserVariable> list = mSenarioManager.UserVariables.VarList;
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
					szScript = "del " + var.Name;
					m_arScriptList.Add(szScript);
				}
			}
			IEnumerable<Enums> list2 = mSenarioManager.EnumList.VarList;
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
			IEnumerable<Variable> list3 = mSenarioManager.SystemVariables.VarList;
			foreach (Variable var in list3)
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
		}

		//private void RegisterVariables()
		//{
		//	IEnumerable<UserVariable> list = mSenarioManager.UserVariables.VarList;
		//	foreach (UserVariable var in list)
		//	{
		//		if (var.ToStringValue() != "")
		//		{
		//			string szScript = var.Name + "=" + var.ToStringValue();
		//			m_arScriptList.Add(szScript);
		//		}
		//	}
		//	IEnumerable<Enums> list2 = mSenarioManager.EnumList.VarList;
		//	foreach (Enums var in list2)
		//	{
		//		if (var.ToStringValue() != "")
		//		{
		//			string szScript = var.Name + "=" + var.ToStringValue();
		//			m_arScriptList.Add(szScript);
		//		}
		//	}
		//	IEnumerable<Variable> list3 = mSenarioManager.SystemVariables.VarList;
		//	foreach (Variable var in list3)
		//	{
		//		if(var.ToStringValue() != "")
		//		{
		//			string szScript = var.Name + "=" + var.ToStringValue();
		//			m_arScriptList.Add(szScript);
		//		}
               
		//	}
		//}
       
        private string MakeScript(Sections.Section section)
        {
            if ( (section.GetComponentType() != Sections.Section.ComponentType.ENDPOINT) &&
                 (section.GetComponentType() != Sections.Section.ComponentType.PROCESS) &&
                 (section.GetComponentType() != Sections.Section.ComponentType.DECISION))                 
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();

            sb.Append("def ");
            sb.Append(CheckString(section.Data.ComponentID));
            sb.Append("()");
            sb.AppendLine(":");

            if (section.GetComponentType() == Sections.Section.ComponentType.ENDPOINT)
            {
                Sections.SectionDataEndPoint data = (Sections.SectionDataEndPoint)section.Data;
                if( data != null && data.IsBegin == false)
                {
                    sb.Append("\t").AppendLine("zzz = 1");
                    return sb.ToString();
                }                
            }
                        
            ArrayList arGlobals = RegisterGlobarVariables();
            foreach (string global in arGlobals)
            {
                sb.Append("\t").AppendLine(global);
            }


            foreach (Sections.Arrow link in section.Arrows)
            {
                if (link.BeginLink != section)
                {
                    continue;
                }

                if (section.GetComponentType() == Sections.Section.ComponentType.DECISION)
                {
                    Sections.SectionDataDecision data = (Sections.SectionDataDecision)section.Data;
                    if (data == null)
                        continue;

					string szExpr = data.Expression.Replace("&&", "and");
					szExpr = szExpr.Replace("||", "or");

					sb.Append("\t").Append("tempBool = (").Append(szExpr).AppendLine(")");

                    if (link.Text == "Yes")
                    {
                        sb.AppendLine("\tif tempBool == True :");

                        Sections.Section sectionEnd = link.EndLink;
                        if (sectionEnd.GetComponentType() == Sections.Section.ComponentType.PROCESS ||
                            sectionEnd.GetComponentType() == Sections.Section.ComponentType.DECISION)
                        {
                            sb.Append("\t\t").Append(CheckString(sectionEnd.Data.ComponentID)).AppendLine("()");
                        }
                        else
                        {
                            sb.Append("\t\t").AppendLine("zzz = 1");
                        }
                    }
                    else if (link.Text == "No")
                    {
                        sb.AppendLine("\tif tempBool == False :");

                        Sections.Section sectionEnd = link.EndLink;
                        if (sectionEnd.GetComponentType() == Sections.Section.ComponentType.PROCESS ||
                            sectionEnd.GetComponentType() == Sections.Section.ComponentType.DECISION)
                        {
                            sb.Append("\t\t").Append(CheckString(sectionEnd.Data.ComponentID)).AppendLine("()");
                        }                        
                        else
                        {
                            sb.Append("\t\t").AppendLine("zzz = 1");
                        }
                    }
                    else
                    {
                        string szScript = data.Expression;
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
								{
									string szTemp = scripts[i].Replace("&&", "and");
									szTemp = szTemp.Replace("||", "or");
									sb.Append("\t").AppendLine(szTemp);
								}
                            }
                        }

                        Sections.Section sectionEnd = link.EndLink;
                        if (sectionEnd.GetComponentType() == Sections.Section.ComponentType.PROCESS ||
                            sectionEnd.GetComponentType() == Sections.Section.ComponentType.DECISION)
                        {
                            sb.Append("\t").Append(CheckString(sectionEnd.Data.ComponentID)).AppendLine("()");
                        }
                        else
                        {                           
                            sb.Append("\t").AppendLine("zzz = 1");                       
                        }
                    }
                }
                else
                { 

                    if( section.GetComponentType() == Sections.Section.ComponentType.PROCESS)
                    {
                        Sections.SectionDataProcess data = (Sections.SectionDataProcess)section.Data;
                        string szScript = data.Expression;
                        if (szScript != null && szScript != "")
                        {                           
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
									{
										string szTemp = scripts[i].Replace("&&", "and");
										szTemp = szTemp.Replace("||", "or");
										sb.Append("\t").AppendLine(szTemp);
									}                                        
                                }
                            }
                        }
                    }

                    Sections.Section sectionEnd = link.EndLink;
                    if (section.GetComponentType() == Sections.Section.ComponentType.PROCESS || 
                        section.GetComponentType() == Sections.Section.ComponentType.ENDPOINT)
                    {
                        sb.Append("\t").Append(CheckString(sectionEnd.Data.ComponentID)).AppendLine("()");
                    }
                }
            }

            sb.AppendLine("");

            return sb.ToString();
        }
        
        public string CheckScript()
        {
            StringBuilder sb = new StringBuilder();

            foreach(string szScript in m_arScriptList)
            {
                sb.AppendLine(szScript);
            }

            if (m_arScriptList.Count == 0)
                return "NOTHING";

            if( m_Funcion.Count == 0)
            {
                return "NOTHING";
            }
            
            string szMainCall = m_Funcion["Main()"];
            sb.AppendLine(szMainCall);

            try
            {
				ScriptProxy.Instance.RunPythonScript("CR=0");
                ScriptProxy.Instance.RunPythonScript(sb.ToString());
                //m_szScriptResult = ScriptProxy.Instance.Result;
            }
            catch(Exception e)
            {
                m_szScriptResult = e.Message;
                return "ERROR";
            }

            object obj = ScriptProxy.Instance.CallAction("CR");
            m_szScriptResult = obj.ToString();
           
            return sb.ToString();
        }
    }
}