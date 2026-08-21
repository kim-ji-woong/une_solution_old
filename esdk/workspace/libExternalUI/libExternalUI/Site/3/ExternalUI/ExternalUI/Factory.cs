using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using libExternalUI;

namespace libExternalUI
{
    public class Factory
    {
        public static IUIManager GetUIManager(Control parentCtrl)
        {
            //return new libExternalUI.Lib.UIManager(parentCtrl);
            
            if (parentCtrl == null)
                return null;

            foreach (Control ctrl in parentCtrl.Controls)
            {
                System.Diagnostics.Trace.WriteLine(ctrl.Name);
                if (ctrl.Name == "panelBottom")
                //if (ctrl.Name == "panelLeft2")
                {
                    foreach (Control ctrl2 in ctrl.Controls)
                    {
                        if (ctrl2.Name == "PageBackstageHome")
                        //if (ctrl2.Name == "panelLeftItem")
                        {
                            return new libExternalUI.Lib.UIManager(ctrl2);
                        }
                    }
                }
            }

            return null;
        }
    }
}
