using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sections;
using UnE.SOP;
using UnE.SOP.Process;
using UnE.SOP.Workstate;

namespace SOPManager.Process
{
    public class ProcessSectionFactory : UnE.SOP.Process.ProcessSectionFactory
    {
        private static ProcessSectionFactory m_Instance = new ProcessSectionFactory();
        public static SOPManager.Process.ProcessSectionFactory Instance
        {
            get { return m_Instance; }
            set { m_Instance = value; }
        }

        public ProcessSectionFactory()
        {
            ProcessSectionManager.Instance.Factory = this;
        }

        public override ProcessSectionIF CreateProcess(SectionState state)
        {
            if (state == null)
                return null;

            ProcessSectionIF process = null;

            /*if (state.Section.GetComponentType() == Section.ComponentType.INTERNAL)
            {
                process = new InternalNotifyProcess(state);
            }
            else if (state.Section.GetComponentType() == Section.ComponentType.EXTERNAL)
            {
                process = new ExternalNotifyProcess(state);
            }
            else if (state.Section.GetComponentType() == Section.ComponentType.TRANSMISSION)
            {
                process = new TransmissionNotifyProcess(state);
            }
            else */if (state.Section.GetComponentType() == Section.ComponentType.PROCESS)
            {
                process = new TaskNotifyProcess(state);
            }
            
            if (process == null)
                return null;

            //FormSOP.Instance.GetPageHome().PostCreateProcess(process, state);
            return process;
        }
    }
}
