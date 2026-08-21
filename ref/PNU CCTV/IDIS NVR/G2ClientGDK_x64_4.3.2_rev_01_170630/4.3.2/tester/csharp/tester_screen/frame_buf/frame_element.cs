using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using GDK;

namespace GDK_tester
{
    public class frame_element
    {
        public enum LOADED
        {
            INVALID = -1,
            END_OF = -2
        }

        public frame_element()
        {
            frame = new G2FRAME();
            channel = -1;
            pane = -1;
            sleep = -1;
            discard = false;
            valid = false;
        }
        public frame_element(ref G2FRAME frame, int channel, int pane)
        {
            set(ref frame, channel, pane);
        }

        void set(ref G2FRAME frame, int channel, int pane)
        {
            this.frame = frame;
            this.channel = channel;
            this.pane = pane;
            this.valid = true;
            this.face_detection.from(ref frame._archived._content_analytics_face_detection);
            this.si_elevator_status.from(ref frame._archived._si_elevator_status);

            if (pane >= 0)
            {
                this.data = new byte[frame.data_size];
                Marshal.Copy(frame._data, this.data, 0, (int)frame.data_size);
            }
        }

        public int channelext { get { return frame.channel; } }
        public int stream_id { get { return frame._index._stream_id; } }
        public G2FRAME.TYPE type { get { return frame.type; } }
        public G2SPOT spot { get { return frame.spot; } }
        public G2FRAME frame;
        public G2CODEC_INFO_CONTENT_ANALYTICS_FACE_DETECTION face_detection;
        public G2CODEC_INFO_SI_ELEVATOR_STATUS si_elevator_status;
        public int channel;
        public int pane;
        public int sleep;
        public bool discard;
        public bool valid;
        public byte[] data;
    }
}
