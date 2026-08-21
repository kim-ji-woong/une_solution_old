#if _IDIS_NVR_
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GDK;

namespace UnE.Control.CCTVControl.IDIS_NVR
{
    public partial class IdisNvrSet
    {
        public void on_post_watch_receive_instant_recording_start(int channel, G2INSTANT_RECORDING_RESULT.TYPE result, G2INSTANT_RECORDING_CHANNEL_STATUS[] status)
        {
            string info = string.Format("instant recording start: {0}", result.ToString());
            _screen.message().disp(info, 5 * 1000, false);
        }

        public void on_post_watch_receive_instant_recording_stop(int channel, G2INSTANT_RECORDING_RESULT.TYPE result)
        {
            string info = string.Format("instant recording stop: {0}", result.ToString());
            _screen.message().disp(info, 5 * 1000, false);
        }

        public void on_post_watch_receive_instant_recording_status(int channel, G2INSTANT_RECORDING_RESULT.TYPE result, G2INSTANT_RECORDING_CHANNEL_STATUS[] status)
        {

        }
    }
}
#endif