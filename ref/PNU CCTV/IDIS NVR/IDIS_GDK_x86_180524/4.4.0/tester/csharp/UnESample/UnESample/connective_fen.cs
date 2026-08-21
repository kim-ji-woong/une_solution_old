using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDK;

namespace UnESample
{
    public partial class FormMain : g2fen_listener
    {
        private void init_connective_fen()
        {
            g2fen.get().set_listener(this);
        }

        public void on_g2fen_nat_type_discovered(G2NAT_INFO.TYPE type, ref G2NAT_INFO ni)
        {
            //System.Diagnostics.Trace.WriteLine(ni);
        }
    }
}
