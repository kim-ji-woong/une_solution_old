using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SOPDisasterSystem
{
    public interface ISOPInfo
    {
        Sections.WorkFlow GetCurrentWorkflow();
    }
}
