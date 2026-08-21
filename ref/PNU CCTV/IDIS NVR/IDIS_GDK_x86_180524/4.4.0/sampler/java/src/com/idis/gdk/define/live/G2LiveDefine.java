package com.idis.gdk.define.live;

public class G2LiveDefine
{
    public enum SUPPORT {
        UNDEFINED(-1),
        DRAW_MOTION(1),
        STATUS_IDR(3),
        IMAGE_CONFIG(4),
        MULTI_STREAM(5),
        AUDIO_STREAM_IN_WATCH_PORT(6),
        BEEP_CONTROL(8),
        HYBRID_STREAMING_VIDEO(9),
        HYBRID_STREAMING_AUDIO_IN(10),
        HYBRID_STREAMING_AUDIO_OUT(11),
        PTZ_PRESET_G2(12),
        NETWORK_ALARM_G2(516),
        SI_ELEVATOR_STATUS_INFO(1000);

        private int value;
        private SUPPORT(int value) { this.value = value; }
        public boolean equal(int i) { return value == i; }
        public int to() { return value; }
        public static SUPPORT from(int id)
        {
            SUPPORT res = SUPPORT.UNDEFINED;
            for (SUPPORT var : SUPPORT.values()) {
                if (var.equal(id)) {
                    res = var;
                    break;
                }
            }
            return res;
        }
    }
}
