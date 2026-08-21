using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SOPSimulator.IDAL
{
    public interface ISelect
    {

        ArrayList JoinHistoryComponentActionStep(int actionStepHistoryID, out string strErrorMessage);
    }
}
