using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UnE.SOP.Workstate
{
    public delegate void EndCheckPosition(bool bResult);

    public interface IWorkflowStartOption
    {
       
        event EndCheckPosition OnCheckPositionEnd;
        HistoryDisasterPosition LastPosition
        {
            get;
            set;
        }
        string PositionName
        {
            get;
            set;
        }
        string DisasterName
        {
            get;
            set;
        }
        Form GetInvokeForm();
        bool IsHandleCreated();  
        void AddLastHistoryDisasterPoistion(HistoryDisasterPosition pos);
    }
}
