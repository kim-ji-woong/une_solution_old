using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegratedManagement2
{
    public class InternalMessage
    {
        public const byte RUN_SOP_SIMULATOR = 1;
        public const byte CHECK_SOP_SIMULATOR = 2;
        public const byte REPLY_CHECK_SOP_SIMULATOR = 3;
        public const byte REQUEST_PERMISSION_SOP_SIMULATOR = 4;
    }
}
