package com.idis.gdk.define;

import com.idis.gdk.G2GUID;
import com.idis.gdk.G2Object;
import com.idis.gdk.G2Stream;
import com.idis.gdk.util.G2Enum;

public final class G2EventInfo extends G2Object implements Cloneable {
    public enum TYPE_LEVEL1 implements G2Enum.Converter<Integer, TYPE_LEVEL1> {
        UNKNOWN(0),
        TIME(1 << 16),
        SYSTEM(2 << 16),
        DEVICE(3 << 16),
        USER(4 << 16),
        PREEVENT(5 << 16);
        
        static G2Enum.ReverseMap<Integer, TYPE_LEVEL1> map = new G2Enum.ReverseMap<Integer, TYPE_LEVEL1>(TYPE_LEVEL1.class, TYPE_LEVEL1.UNKNOWN);
        static public TYPE_LEVEL1 from(int id) { return map.get(id); }
        private final int value;
        private TYPE_LEVEL1(int value) { this.value = value; }
        public Integer to() { return this.value; }
        public TYPE_LEVEL1 get(Integer id) { return map.get(id); }
    }
  
    public enum TYPE_LEVEL2 implements G2Enum.Converter<Integer, TYPE_LEVEL2> {
        UNKNOWN(0),
        // L1_TIME
        CONTINUOUS(1000),
        PERIODIC(1001),
        ONE_TIME(1002),
        // L1_SYSTEM
        SERVICE_ADDED(2000),
        SERVICE_CONNECTED(2001),
        SERVICE_DISCONNECTED(2002),
        DEVICE_CONNECTED(2500),
        DEVICE_DISCONNECTED(2501),
        DEVICE_CONNECT_FAIL(2502),
        DEVICE_ADDED(2503),
        DEVICE_MODIFIED(2504),
        DEVICE_REMOVED(2505),
        NETWORK_CAMREA_CONNECTED(2506),
        NETWORK_CAMERA_DISCONNECTED(2507),
        // camera conditions
        CAMERA_EVENT_BEGIN(3099),
        CAMERA_MOTION_DETECTION_ON(3100),
        CAMERA_MOTION_DETECTION_OFF(3101),
        CAMERA_OBJECT_DETECTION_ON(3102),
        CAMERA_OBJECT_DETECTION_OFF(3103),
        CAMERA_VIDEO_LOSS_ON(3104),
        CAMERA_VIDEO_LOSS_OFF(3105),
        CAMERA_VIDEO_BLIND_ON(3106),
        CAMERA_VIDEO_BLIND_OFF(3107),
        CAMERA_IGNORED_MOTION_ON(3108),
        CAMERA_VIDEO_ANALYTICS_ON(3109),
        CAMERA_VIDEO_ANALYTICS_OFF(3110),
        CAMERA_TRIPZONE_ON(3111),
        CAMERA_TRIPZONE_OFF(3112),
        CAMERA_TAMPER_ON(3113),
        CAMERA_TAMPER_OFF(3114),
        CAMERA_IGNORED_TRIPZONE_ON(3115),
        CAMERA_IGNORED_VIDEO_ANALYTICS_ON(3116),
        CAMERA_FENCE_DETECTION(3117),            // deprecated
        CAMERA_LOITERING(3118),                  // deprecated
        CAMERA_ABANDONED_OBJECT_DETECTION(3119), // deprecated
        CAMERA_REMOVED_OBJECT_DETECTION(3120),   // deprecated
        CAMERA_TRAFFIC(3121),                    // deprecated
        CAMERA_DIRECTION_DETECTION(3122),        // deprecated
        CAMERA_FACE_DETECTION(3123),             // deprecated
        CAMERA_FACE_DETECTION_ON(3124),
        CAMERA_IGNORED_FACE_DETECTION_ON(3125),
        CAMERA_FACE_DETECTION_OFF(3126),
        CAMERA_INSTANT_RECORD_ON(3127),
        CAMERA_INSTANT_RECORD_OFF(3128),
        CAMERA_RECORD_BAD_ON(3129),
        CAMERA_RECORD_BAD_OFF(3130),
        CAMERA_FAN_ERROR_ON(3131),
        CAMERA_FAN_ERROR_OFF(3132),
        CAMERA_EVENT_END(3133),
        // alarm-in conditions
        ALARM_EVENT_BEGIN(3199),
        ALARMIN_ON(3200),
        ALARMIN_OFF(3201),
        NETWORK_ALARMIN_ON(3202),
        NETWORK_ALARMIN_OFF(3203),
        ALARMIN_BAD(3204),
        ALARM_RESET_IN(3205),
        USER_DEFINED_ALARMIN_ON(3206),
        USER_DEFINED_ALARMIN_OFF(3207),
        ALARM_EVENT_END(3208),
        // audio conditions
        AUDIO_EVENT_BEGIN(3299),
        AUDIO_ON(3300),
        AUDIO_OFF(3301),
        AUDIO_IGNORED_AUDIO_ON(3302),
        AUDIO_EVENT_END(3303),
        // text-in conditions
        TEXT_IN_EVENT_BEGIN(3399),
        TEXT_IN_ON(3400),
        TEXT_IN_OFF(3401),
        TEXT_IN_BAD_ON(3402),
        TEXT_IN_BAD_OFF(3403),
        TEXT_IN_EVENT_END(3404),
        USER_EVENT_BEGIN(3999),
        // L1_USER
        UNKNOWN_USER_TYPE(4000),
        USER_LOGIN(4001),
        USER_LOGOUT(4002),
        USER_AUTO_LOGOUT(4003),
        USER_AWAY(4004),
        USER_EVENT_END(4005),
        // L1_DEVICE (DVR)
        DVR_EVENT_BEGIN(4999),
        DVR_TANGO_ALMOST_FULL_ON(5000),
        DVR_TANGO_ALMOST_FULL_OFF(5001),
        DVR_PANIC_RECORD_ON(5002),
        DVR_PANIC_RECORD_OFF(5003),
        DVR_RECORDER_BAD_ON(5004),
        DVR_RECORDER_BAD_OFF(5005),
        DVR_FAN_ERROR_ON(5006),
        DVR_FAN_ERROR_OFF(5007),
        DVR_SYSTEM_BOOTUP(5008),
        DVR_SYSTEM_RESTART(5009),
        DVR_SYSTEM_SHUTDOWN(5010),
        DVR_DISK_FULL_ON(5011),
        DVR_DISK_FULL_OFF(5012),
        DVR_DISK_BAD_ON(5013),
        DVR_DISK_BAD_OFF(5014),
        DVR_DISK_TEMPERATURE_ON(5015),
        DVR_DISK_TEMPERATURE_OFF(5016),
        DVR_DISK_SMART_ON(5017),
        DVR_DISK_SMART_OFF(5018),
        DVR_DISK_ON(5019),
        DVR_DISK_OFF(5020),
        DVR_DISK_CONFIG_CHANGE(5021),
        DVR_COVER_OPEN(5022),
        DVR_COVER_CLOSE(5023),
        DVR_CAR_OVERSPEED_ON(5024),
        DVR_CAR_OVERSPEED_OFF(5025),
        DVR_CAR_SUDDEN_ACCLERATION(5026),
        DVR_CAR_SUDDEN_STOP(5027),
        DVR_CAR_STARTING_WITH_DOORS_OPEN(5028),
        DVR_SYSTEM_ALIVE(5029),
        DVR_GPS_RECEIVE_ERROR_ON(5030),
        DVR_GPS_RECEIVE_ERROR_OFF(5031),
        DVR_LOGIN_FAILED_SEVERAL_TIMES(5032),
        DVR_NO_DISK(5033),
        DVR_STOP_RECORD_ON(5034),
        DVR_EVENT_END(5035),
        // etc event
        SECOM_EVENT_BEGIN(6999),
        SECOM_FS_LOOP(7000),
        SECOM_FS_PANIC(7001),
        SECOM_FS_CARD(7002),
        SECOM_FS_MACHINE(7003),
        SECOM_EVENT_END(7004),
        SIPASS_EVENT_BEGIN(7499),
        SIPASS_RECORD_ON(7500),
        SIPASS_RECORD_OFF(7501),
        SIPASS_EVENT_END(7502);
        
        static G2Enum.ReverseMap<Integer, TYPE_LEVEL2> map = new G2Enum.ReverseMap<Integer, TYPE_LEVEL2>(TYPE_LEVEL2.class, TYPE_LEVEL2.UNKNOWN);
        static public TYPE_LEVEL2 from(int id) { return map.get(id); }
        private final int value;
        private TYPE_LEVEL2(int value) { this.value = value; }
        public Integer to() { return this.value; }
        public TYPE_LEVEL2 get(Integer id) { return map.get(id); }
    }
    
    public static class CallbackSite extends G2Object implements Cloneable {
        public CallbackSite() {}
        public CallbackSite(CallbackSite right) {
            this._site = right._site;
            this._mac = right._mac.clone();
            this._is_name = right._is_name;
            this._case_insensitive = right._case_insensitive;
        }

        public CallbackSite clone() {
            return new CallbackSite(this);
        }

        public void serialize(G2Stream s) {
            s.putString(_site);
            s.putObject(_mac);
            s.putBool(_is_name);
            s.putBool(_case_insensitive);
        }

        public void deserialize(G2Stream s) {
            _site = s.getString();
            _mac = s.getObject(_mac, G2MacAddress.class);
            _is_name = s.getBool();
            _case_insensitive = s.getBool();
        }

        public String _site = "";
        public G2MacAddress _mac = new G2MacAddress();
        public boolean _is_name = false;
        public boolean _case_insensitive = false;
    }
    
    public static class G2EventRas extends G2Object implements Cloneable {
        public G2EventRas() {}
        public G2EventRas(G2EventRas right) {
            this._seq_number = right._seq_number;
            this._level = right._level;
            this._channel = right._channel;
            this._cameras = right._cameras;
            this._spot = right._spot.clone();
            this._label = right._label;
            this._site = right._site.clone();
            this._g2 = right._g2;
            this._g2event = right._g2event.clone();
        }

        public G2EventRas clone() {
            return new G2EventRas(this);
        }

        public void serialize(G2Stream s) {
            s.putInt32(_seq_number);
            s.putInt32(_level.to());
            s.putInt32(_channel);
            s.putInt32(_cameras);
            s.putObject(_spot);
            s.putString(_label);
            s.putObject(_site);
            s.putBool(_g2);
            s.putObject(_g2event);
        }

        public void deserialize(G2Stream s) {
            _seq_number = s.getInt32();
            _level = G2Event.LEVEL.from(s.getInt32());
            _channel = s.getInt32();
            _cameras = s.getInt32();
            _spot = s.getObject(_spot, G2Spot.class);
            _label = s.getString();
            _site = s.getObject(_site, CallbackSite.class);
            _g2 = s.getBool();
            _g2event = s.getObject(_g2event, G2Event.class);
        }
        
        public G2ChannelSet associatedChannels() {
            G2ChannelSet res;
            if (_g2) {
                res = new G2ChannelSet(_g2event._associated_channels);
            } else {
                res = G2ChannelSet.from(_cameras);
            }
            return res;
        }
        
        public int _seq_number = 0;
        public G2Event.LEVEL _level = G2Event.LEVEL.NORMAL;
        public int _channel = -1;
        public int _cameras = 0;
        public G2Spot _spot = G2Spot.INVALID.clone();
        public String _label = "";
        public CallbackSite _site;
        public boolean _g2 = false;
        public G2Event _g2event;
    }
    
    public G2EventInfo() {}
    public G2EventInfo(G2EventInfo right) {
        this._level1 = right._level1;
        this._level2 = right._level2;
        this._source = right._source.clone();
        this._target = right._target.clone();
        this._spot = right._spot.clone();
        this._local_time = right._local_time.clone();
        this._label = right._label;
        this._event_ras = right._event_ras;
        this._is_evt_onetime = right._is_evt_onetime;
        this._is_evt_dvr = right._is_evt_dvr;
        this._is_evt_dvr_system = right._is_evt_dvr_system;
        this._is_evt_monitoring = right._is_evt_monitoring;
    }
    
    public G2EventInfo clone() {
        return new G2EventInfo(this);
    }
    
    public void serialize(G2Stream s) {
        s.putInt32(_level1.to());
        s.putInt32(_level2.to());
        s.putObject(_source);
        s.putObject(_target);
        s.putObject(_spot);
        s.putObject(_local_time);
        s.putString(_label);
        s.putObject(_event_ras);
        s.putBool(_is_evt_onetime);
        s.putBool(_is_evt_dvr);
        s.putBool(_is_evt_dvr_system);
        s.putBool(_is_evt_monitoring);
    }
    
    public void deserialize(G2Stream s) {
        _level1 = TYPE_LEVEL1.from(s.getInt32());
        _level2 = TYPE_LEVEL2.from(s.getInt32());
        _source = s.getObject(_source, G2GUID.class);
        _target = s.getObject(_target, G2GUID.class);
        _spot = s.getObject(_spot, G2Spot.class);
        _local_time = s.getObject(_local_time, G2Time.class);
        _label = s.getString();
        _event_ras = s.getObject(_event_ras, G2EventRas.class);
        _is_evt_onetime = s.getBool();
        _is_evt_dvr = s.getBool();
        _is_evt_dvr_system = s.getBool();
        _is_evt_monitoring = s.getBool();
    }
    
    public TYPE_LEVEL1 _level1 = TYPE_LEVEL1.UNKNOWN;
    public TYPE_LEVEL2 _level2 = TYPE_LEVEL2.UNKNOWN;
    public G2GUID _source = G2GUID.INVALID.clone();
    public G2GUID _target = G2GUID.INVALID.clone();
    public G2Spot _spot = G2Spot.INVALID.clone();
    public G2Time _local_time = G2Time.INVALID.clone();
    public String _label = "";
    public G2EventRas _event_ras;
    public boolean _is_evt_onetime = false;
    public boolean _is_evt_dvr = false;
    public boolean _is_evt_dvr_system = false;
    public boolean _is_evt_monitoring = false;  // event made by monitoring service
}
