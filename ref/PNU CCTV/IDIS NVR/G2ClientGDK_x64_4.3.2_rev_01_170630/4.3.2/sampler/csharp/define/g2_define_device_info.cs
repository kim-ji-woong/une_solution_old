using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDK
{
    public struct G2DEVICE_INFO_CALLBACK
    {
        public enum TYPE
        {
            on_receive_product_info = 0,
            on_failed = 1,
            on_canceled = 2,

            CALLBACK_COUNT = 3
        }
    }
}
