using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnE.SOP.Workstate;

namespace UnE.SOP
{
    public interface IDisasterContainer
    {
        HistoryDisasterPosition LastPos
        {
            get;
            set;
        }

        HistoryDisasterPosition GetLastDisasterPosition();

        void SetCheckPoistion(IWorkflowStartOption form, bool bCheck);

        void RemoveDisasterPos();

        void AddDisasterPos(string disastertype, float x, float y, float z);

        void HideAllShelter();

        // nType : ShelterPath의 Type
        //         CoreAPI의 UBaseView::ShowPath(int nType)의 인자로 사용된다.
        // nShelterType : UnE.Spatial.Shelter.ShelterTypes(화재, 누출, 지진...)
        //                재난종류별 대피소를 각각 지정할 수 있도록 한다.
        void ShowShelter(int nType, int nShelterType);

        void ShowCCTVForm(bool bShow);

        void OnCheckEnd(bool bResult);
    }
}
