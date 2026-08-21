package com.idis.gdk;

import java.nio.ByteBuffer;

import com.sun.jna.Native;
import com.sun.jna.win32.StdCallLibrary;
import com.idis.gdk.define.G2DisconnectReason;
import com.idis.gdk.define.G2Event;
import com.idis.gdk.define.G2FenDefine;

public class G2Foundation {
    public interface G2Library extends StdCallLibrary {
        int j2_time_from_elements(int year, int month, int day, int hour, int minute, int second);
        int j2_time_compare(int left, int right);
        int j2_time_get_invalid();
        int j2_time_get_current();
        int j2_time_get_year(int time);
        int j2_time_get_month(int time);
        int j2_time_get_day(int time);
        int j2_time_get_hour(int time);
        int j2_time_get_minute(int time);
        int j2_time_get_second(int time);
        int j2_time_is_valid(int time);
        int j2_spot_compare(int left_segment, int left_time, int left_tick, int right_segment, int right_time, int right_tick);
        int j2_spot_is_valid(int segment, int time, int tick);
        void j2_get_string_disconnect_reason(int reason, byte[] string, int string_len);
        void j2_get_string_dvrns_error(int error, byte[] string, int string_len);
        void j2_get_string_event_type_g2(int type, byte[] string, int string_len);
    }
    
    public void startup()
    {
        if (_lib == null) {
            _lib = (G2Library) Native.loadLibrary(G2Main.get().moduleName(), G2Library.class);
        }
    }

    public void cleanup()
    {
        _lib = null;
    }

    public int timeFrom(int year, int month, int day, int hour, int minute, int second) {
        return lib().j2_time_from_elements(year, month, day, hour, minute, second);
    }

    public int timeCompare(int left, int right) {
        return lib().j2_time_compare(left, right);
    }

    public int timeGetInvalid() {
        return lib().j2_time_get_invalid();
    }

    public int timeGetCurrent() {
        return lib().j2_time_get_current();
    }

    public int timeGetYear(int time) {
        return lib().j2_time_get_year(time);
    }

    public int timeGetMonth(int time) {
        return lib().j2_time_get_month(time);
    }

    public int timeGetDay(int time) {
        return lib().j2_time_get_day(time);
    }

    public int timeGetHour(int time) {
        return lib().j2_time_get_hour(time);
    }

    public int timeGetMinute(int time) {
        return lib().j2_time_get_minute(time);
    }

    public int timeGetSecond(int time) {
        return lib().j2_time_get_second(time);
    }

    public boolean timeIsValid(int time) {
        return lib().j2_time_is_valid(time) != 0;
    }

    public int spotCompare(int left_segment, int left_time, int left_tick, int right_segment, int right_time, int right_tick) {
        return lib().j2_spot_compare(left_segment, left_time, left_tick, right_segment, right_time, right_tick);
    }

    public boolean spotIsValid(int segment, int time, int tick) {
        return lib().j2_spot_is_valid(segment, time, tick) != 0;
    }

    public String stringGetDisconnectReason(G2DisconnectReason reason) {
        G2Stream s = G2Stream.from(ByteBuffer.allocate(256));
        lib().j2_get_string_disconnect_reason(reason.to(), s.buf().array(), s.buf().capacity());
        return s.getString();
    }
    
    public String stringGetDvrnsError(G2FenDefine.RESULT res)
    {
        G2Stream s = G2Stream.from(ByteBuffer.allocate(256));
        lib().j2_get_string_dvrns_error(res.to(), s.buf().array(), s.buf().capacity());
        return s.getString();
    }

    public String stringGetEventTypeG2(G2Event.TYPE type) {
        byte[] buf = new byte[256];
        lib().j2_get_string_event_type_g2(type.to(), buf, buf.length);
        return G2Stream.from(ByteBuffer.wrap(buf)).getString();
    }

    public G2Library lib() {
        return _lib;
    }
    
    protected G2Foundation() {}
    protected G2Library _lib;
    protected static G2Foundation s_singleton = new G2Foundation();
    public static G2Foundation get() {
        return s_singleton;
    }
}
