using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControlTeamEditor
{
    public interface IMainForm
    {
        bool CloseApplication
        {
            get;
        }

        void RefreshCell(DataGridViewCell cell);
    }
}
