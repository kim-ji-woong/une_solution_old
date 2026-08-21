using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libExternalUI;
using System.Windows.Forms;

namespace Sample.SampleLib
{
    public class SampleFactory
    {
        public static IUIManager GetUIManager(Control parentCtrl)
        {
            return new SampleUIManager(parentCtrl);
        }
    }
}
