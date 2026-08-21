using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVSingleViewer.Command
{
    public abstract class Command
    {
        public abstract void RollBack();
        public abstract void Do();
    }
}
