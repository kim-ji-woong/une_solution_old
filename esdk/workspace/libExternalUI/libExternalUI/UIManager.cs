using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libExternalUI
{
    public interface IUIManager
    {
        void ShowControl(object arg);
        void HideControl(object arg);
        void OnResize();
    }
}
