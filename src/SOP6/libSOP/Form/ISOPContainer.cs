using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnE.SOP
{
    public interface ISOPContainer
    {
        void CreateSOPContainer(int nID, string szName, bool isSimulationMode, bool onlySDMS, int nTargetMonitor);

        void LinkDisasterSystem(IDisasterContainer form);
        void SelectComponent(int nActionStepID, bool isRealMode, global::Sections.Section section);
        global::Sections.SectionCommander LoadSectionCommander(int nTeamType, int nMemberID, string strDisplayText);

        void BeginHistory();
        void EndHistory();

        DisasterInfo ReloadDisaster(int nActionStepID);
    }
}
