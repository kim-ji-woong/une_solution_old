using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace libExternalUI
{
    public class Factory
    {
        public static IUIManager GetUIManager(Control parentCtrl)
        {
            return new UIManager(parentCtrl);
        }
    }
}
