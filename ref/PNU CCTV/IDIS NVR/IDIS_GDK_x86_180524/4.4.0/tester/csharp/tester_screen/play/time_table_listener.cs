using System;
using System.Collections.Generic;
using System.Text;
using GDK;

namespace GDK_tester
{
    public interface time_table_listener
    {
        void on_time_table_move_to_spot(G2SPOT spot);
        G2SEARCH_PLAYBACK.COMMAND on_time_table_get_current_command();
    }
}
