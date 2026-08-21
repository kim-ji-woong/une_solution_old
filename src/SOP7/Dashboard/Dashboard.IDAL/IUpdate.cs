using Dashboard.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dashboard.IDAL
{
    public interface IUpdate
    {
        bool UpdateCurrentWorkPermit(CurrentWorkPermit currentWorkPermit, out string strErrorMessage);
        bool UpdateCurrentWorkPermit(Dictionary<CurrentWorkPermit.Fields, object> dicSets, Dictionary<CurrentWorkPermit.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
    }
}
