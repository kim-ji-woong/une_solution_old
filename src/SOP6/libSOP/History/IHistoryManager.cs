using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sections;

namespace UnE.SOP.History
{
    using Workstate;

    public interface IHistoryManager
    {
        HistorySectionData AddSectionHistory(Section section, SectionState sectionState, Workstate.State state, int nProcessDirections, bool showBoard, int nCheckedNotify1, int nCheckedNotify2, int nCheckedRun, int nCheckedComplete, Dictionary<int, List<HistorySectionData.DetailData>> detailDatas);
        HistorySectionDecisionData AddDecisionHistory(SectionDecision section, SectionState sectionState, Workstate.State state, int nProcessDirections, Section nextSection = null, bool showBoard = false);
        HistorySectionInternalData AddInternalHistory(SectionInternal section, SectionState sectionState, Workstate.State state, int nProcessDirections, int nCheckedNotify1, int nCheckedRun, int nCheckedComplete, bool usePopupMessage = false, bool useSMS = false, bool useBroadcast = false, bool showBoard = false);
        HistorySectionExternalData AddExternalHistory(SectionExternal section, Workstate.State state, int nProcessDirections, int nCheckedNotify1, int nCheckedNotify2, int nCheckedRun, int nCheckedComplete, bool useSMS = false, bool useFax = false, bool showBoard = false);
        HistorySectionTransmissionData AddTransmissionHistory(SectionTransmission section, Workstate.State state, int nProcessDirections, int nCheckedNotify1, int nCheckedNotify2, int nCheckedRun, int nCheckedComplete, bool usePopupMessage = false, bool useSMS = false, bool useBroadcast = false, bool useExSMS = false, bool useExFax = false, bool showBoard = false);
    }
}
