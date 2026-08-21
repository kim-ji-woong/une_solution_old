using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication10
{
    public interface ICommand
    {
        object Execute(object context);

        object DoExecute(object context);
    }
}
