using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using GDK;

namespace GDK_tester
{
    public partial class form_search : g2fen_listener
    {
        public void init_connective_fen()
        {
            g2fen.get().set_listener(this);
        }

        public void on_g2fen_nat_type_discovered(G2NAT_INFO.TYPE type, ref G2NAT_INFO ni)
        {
            Debug.WriteLine(ni);
        }
    }
}
