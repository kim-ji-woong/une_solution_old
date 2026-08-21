using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTSP_ONVIF
{
    class RTSP_ONVIF_RESPONSE
    {
        public const int UNKNOWN = 21;
        public const int FAILED_GOTO_PRESET = 22;

        public const int PROFILE_NOT_FOUND = 100;
        public const int PRESET_NOT_FOUND = 110;
        public const int ONVIF_HTTP_NOT_CONNECTED = 120;
        public const int USER_NAME_EMPTY = 130;
        public const int USER_PASS_EMPTY = 131;
        public const int IPADDRESS_EMPTY = 132;
        public const int INSTANSIATE_SUCCESS = 200;
        public const int MAKE_SESSION_SUCCCESS = 210;
        public const int SET_TARGET_SUCCESS = 220;
        public const int MOVE_FINISHED = 230;
       
    }
}
