package com.idis.gdk.define;

import com.idis.gdk.G2Foundation;
import com.idis.gdk.G2Object;
import com.idis.gdk.G2Stream;
import com.idis.gdk.define.G2ChannelSet;
import com.idis.gdk.define.G2Spot;
import com.idis.gdk.util.G2Enum;

public final class G2Event extends G2Object implements Cloneable {
    public enum LEVEL implements G2Enum.Converter<Integer, LEVEL> {
        NORMAL(0),
        EMERGENCY(1),
        ERROR(2);

        static G2Enum.ReverseMap<Integer, LEVEL> map = new G2Enum.ReverseMap<Integer, LEVEL>(LEVEL.class, LEVEL.NORMAL);
        static public LEVEL from(int id) { return map.get(id); }
        private final int value;
        private LEVEL(int value) { this.value = value; }
        public Integer to() { return this.value; }
        public LEVEL get(Integer id) { return map.get(id); }
    }

    public enum TYPE implements G2Enum.Converter<Integer, TYPE> {
        NONE(0),
        UNKNOWN(0x7FFFFFFF),
        ALARM_IN_ON(1),
        ALARM_IN_OFF(2),
        ALARM_IN_BAD_ON(3),
        ALARM_IN_BAD_OFF(4),
        USER_DEFINED_ALARM_ON(5),
        USER_DEFINED_ALARM_OFF(6),
        MOTION_ON(100),
        MOTION_OFF(101),
        OBJECT_TRACK_ON(102),
        OBJECT_TRACK_OFF(103),
        TRIPZONE_ON(104),
        TRIPZONE_OFF(105),
        VIDEO_ANALYTICS_ON(120),
        VIDEO_ANALYTICS_OFF(121),
        IGNORED_MOTION_ON(122),
        IGNORED_TRIPZONE_ON(123),
        VIDEO_INIT(200),
        VIDEO_LOSS_ON(201),
        VIDEO_LOSS_OFF(202),
        VIDEO_BLIND_ON(203),
        VIDEO_BLIND_OFF(204),
        TAMPER_ON(205),
        TAMPER_OFF(206),
        TEXT_IN_ON(300),
        TEXT_IN_OFF(301),
        TEXT_IN_DATA(302),
        TEXT_IN_BAD_ON(303),
        TEXT_IN_BAD_OFF(304),
        FACE_DETECTION_ON(400),
        FACE_DETECTION_OFF(401),
        IGNORED_FACE_DETECTION_ON(402),
        TANGO_FULL_ON(100000),
        TANGO_FULL_OFF(100001),
        TANGO_ALMOST_FULL_ON(100002),
        TANGO_ALMOST_FULL_OFF(100003),
        DISK_BAD(200000),
        DISK_TEMPERATURE_ON(200001),
        DISK_TEMPERATURE_OFF(200002),
        DISK_SMART_ON(200003),
        DISK_SMART_OFF(200004),
        DISK_CONFIG_CHANGE(200005),
        DISK_ON(200006),
        DISK_OFF(200007),
        NO_DISK(200008),
        SYSTEM_ALIVE(300000),
        PANIC_ON(300001),
        PANIC_OFF(300002),
        FAN_ERROR_ON(300003),
        FAN_ERROR_OFF(300004),
        SYSTEM_BOOT_UP(300005),
        SYSTEM_RESTART(300006),
        SYSTEM_SHUTDOWN(300007),
        COVER_OPEN(300008),
        COVER_CLOSE(300009),
        STOP_RECORD_ON(300010),
        LOGIN_FAILED_SEVERAL_TIMES(300011),
        USER_LOGIN(300012),
        USER_LOGOUT(300013),
        SETUP_CHANGED(300014),
        RECORDER_BAD_ON(400000),
        RECORDER_BAD_OFF(400001),
        NSTANT_RECORDING_ON(400002),
        INSTANT_RECORDING_OFF(400003),
        CAMERA_RECORD_BAD_ON(400004),
        CAMERA_RECORD_BAD_OFF(400005),
        AUDIO_ON(500000),
        AUDIO_OFF(500001),
        IGNORED_AUDIO_ON(500002),
        USER_DEFINED(600000),
        CAMERA_FAN_ERROR_ON(700000),
        CAMERA_FAN_ERROR_OFF(700001),
        NETWORK_ALARM_ON(1000000),
        NETWORK_ALARM_OFF(1000001),
        NETWORK_CAMERA_CONNECTED(1000010),
        NETWORK_CAMERA_DISCONNECTED(1000011),
        CHANNEL_RELATED_SEPARATOR(1100000000),
        CAR_OVERSPEED_ON(1100000000),
        CAR_OVERSPEED_OFF(1100000001),
        CAR_SUDDEN_ACCELERATION(1100000002),
        CAR_SUDDEN_STOP(1100000003),
        CAR_STARTING_WITH_DOORS_OPEN(1100000004),
        GPS_RECEIVE_ERROR_ON(1100000100),
        GPS_RECEIVE_ERROR_OFF(1100000101),
        GPS_DATA(1100000102),
        SIPASS_RECORD_ON(1200000000),
        SIPASS_RECORD_OFF(1200000001),
        SECOM_SMART_LOOP(1210000000),
        SECOM_SMART_PANIC(1210000001),
        SECOM_SMART_CARD(1210000002),
        SECOM_SMART_MACHINE(1210000003);

        static G2Enum.ReverseMap<Integer, TYPE> map = new G2Enum.ReverseMap<Integer, TYPE>(TYPE.class, TYPE.NONE);
        static public TYPE from(int id) { return map.get(id); }
        private final int value;
        private TYPE(int value) { this.value = value; }
        public Integer to() { return this.value; }
        public TYPE get(Integer id) { return map.get(id); }
    }

    public enum INFO_TYPE implements G2Enum.Converter<Integer, INFO_TYPE> {
        NONE(0),
        DISK(1),
        GPS(2),
        TEXT_IN(3),
        NETWORK_ALARM(4),
        USER_DEFINED(5);

        static G2Enum.ReverseMap<Integer, INFO_TYPE> map = new G2Enum.ReverseMap<Integer, INFO_TYPE>(INFO_TYPE.class, INFO_TYPE.NONE);
        static public INFO_TYPE from(int id) { return map.get(id); }
        private final int value;
        private INFO_TYPE(int value) { this.value = value; }
        public Integer to() { return this.value; }
        public INFO_TYPE get(Integer id) { return map.get(id); }
    }

    public static final class InfoDisk extends G2Object implements Cloneable {
        public enum TYPE implements G2Enum.Converter<Integer, TYPE> {
            UNKNOWN(0),
            IDE_HDD(1),
            SCSI_HDD(2),
            USB_HDD(3),
            IDE_CDRW(4),
            IDE_DVDRW(5),
            SW_RAID_DISK(6),
            FLASH_MEMORY(7),
            ESATA_HDD(8),
            ISCSI_HDD(9);

            static G2Enum.ReverseMap<Integer, TYPE> map = new G2Enum.ReverseMap<Integer, TYPE>(TYPE.class, TYPE.UNKNOWN);
            static public TYPE from(int id) { return map.get(id); }
            private final int value;
            private TYPE(int value) { this.value = value; }
            public Integer to() { return this.value; }
            public TYPE get(Integer id) { return map.get(id); }
        }
    
        public InfoDisk() {}
        public InfoDisk(InfoDisk right) {
            this._type = right._type;
            this._number = right._number;
            this._raid_index = right._raid_index;
        }
    
        public InfoDisk clone() {
            return new InfoDisk(this);
        }
    
        public void serialize(G2Stream s) {
            s.putInt32(_type.to());
            s.putInt32(_number);
            s.putInt32(_raid_index);
        }
    
        public void deserialize(G2Stream s) {
            _type = TYPE.from(s.getInt32());
            _number = s.getInt32();
            _raid_index = s.getInt32();
        }
    
        public TYPE _type = TYPE.UNKNOWN;
        public int _number = 0;
        public int _raid_index = 0;
    }
    
    public static final class InfoGPS extends G2Object implements Cloneable {
        public InfoGPS() {}
        public InfoGPS(InfoGPS right) {
            this._lat = right._lat;
            this._lon = right._lon;
            this._speed = right._speed;
            this._angle = right._angle;
            this._time = right._time.clone();
            this._data = right._data != null ? right._data.clone() : null;
        }

        public InfoGPS clone() {
            return new InfoGPS(this);
        }

        public void serialize(G2Stream s) {
            s.putFloat32(_lat);
            s.putFloat32(_lon);
            s.putFloat32(_speed);
            s.putFloat32(_angle);
            s.putObject(_time);
            s.putVector(_data);
        }

        public void deserialize(G2Stream s) {
            _lat = s.getFloat32();
            _lon = s.getFloat32();
            _speed = s.getFloat32();
            _angle = s.getFloat32();
            _time = s.getObject(G2Time.class);
            _data = s.getVectorInt8();
        }

        public float _lat = .0f;
        public float _lon = .0f;
        public float _speed = .0f;
        public float _angle = .0f;
        public G2Time _time = G2Time.INVALID.clone();
        public byte[] _data;
    }

    public static final class InfoTextIn extends G2Object implements Cloneable {
        public enum TRANSACTION implements G2Enum.Converter<Integer, TRANSACTION> {
            UNKNOWN(-1),
            BEGIN(0),
            CONTINUE(1),
            END(2),
            COMPLETE(3);
    
            static G2Enum.ReverseMap<Integer, TRANSACTION> map = new G2Enum.ReverseMap<Integer, TRANSACTION>(TRANSACTION.class, TRANSACTION.UNKNOWN);
            static public TRANSACTION from(int id) { return map.get(id); }
            private final int value;
            private TRANSACTION(int value) { this.value = value; }
            public Integer to() { return this.value; }
            public TRANSACTION get(Integer id) { return map.get(id); }
        }
    
        public InfoTextIn() {}
        public InfoTextIn(InfoTextIn right) {
            this._transaction = right._transaction;
            this._system_id = right._system_id;
            this._bad = right._bad;
            this._data = right._data != null ? right._data.clone() : null;
        }
    
        public InfoTextIn clone() {
            return new InfoTextIn(this);
        }
    
        public void serialize(G2Stream s) {
            s.putInt32(_transaction.to());
            s.putInt32(_system_id);
            s.putBool(_bad);
            s.putVector(_data);
        }
    
        public void deserialize(G2Stream s) {
            _transaction = TRANSACTION.from(s.getInt32());
            _system_id = s.getInt32();
            _bad = s.getBool();
            _data = s.getVectorInt8(_data);
        }
    
        public TRANSACTION _transaction = TRANSACTION.BEGIN;
        public int _system_id = 0;
        public boolean _bad = false;
        public byte[] _data;
    }

    public static final class InfoNetworkAlarm extends G2Object implements Cloneable {
        public InfoNetworkAlarm() {}
        public InfoNetworkAlarm(InfoNetworkAlarm right) {
            this._version = right._version.clone();
            this._event = right._event;
            this._time = right._time.clone();
            this._msec = right._msec;
        }
    
        public InfoNetworkAlarm clone() {
            return new InfoNetworkAlarm(this);
        }
    
        public void serialize(G2Stream s) {
            s.putVector(_version);
            s.putInt32(_event);
            s.putObject(_time);
            s.putInt64(_msec);
        }
    
        public void deserialize(G2Stream s) {
            _version = s.getVectorInt8(_version);
            _event = s.getInt32();
            _time = s.getObject(G2Time.class);
            _msec = s.getInt64();
        }
    
        public byte[] _version = new byte[2];
        public int _event = 0;  // user defined type
        public G2Time _time = G2Time.INVALID.clone();
        public long _msec = 0;
    }
    
    public static final class InfoUserDefined extends G2Object implements Cloneable {
        public InfoUserDefined() {}
        public InfoUserDefined(InfoUserDefined right) {
            this._event = right._event;
        }
    
        public InfoUserDefined clone() {
            return new InfoUserDefined(this);
        }
    
        public void serialize(G2Stream s) {
            s.putInt16(_event);
        }
    
        public void deserialize(G2Stream s) {
            _event = s.getInt16();
        }
    
        public short _event = 0;    // user defined type
    }

    public G2Event() {}
    public G2Event(G2Event right) {
        this._type = right._type;
        this._level = right._level;
        this._channel = right._channel;
        this._spot = right._spot.clone();
        this._data = right._data;
        this._associated_channels = right._associated_channels.clone();
        this._info_type = right._info_type;
        this._info = right._info != null ? right._info.clone() : null;
    }

    public G2Event clone() {
        return new G2Event(this);
    }

    public void serialize(G2Stream s) {
        s.putInt32(_type.to());
        s.putInt32(_level.to());
        s.putInt32(_channel);
        s.putObject(_spot);
        s.putString(_data);
        s.putObject(_associated_channels);
        s.putInt32(_info_type.to());
        s.putObject(_info);
    }

    public void deserialize(G2Stream s) {
        _type = TYPE.from(s.getInt32());
        _level = LEVEL.from(s.getInt32());
        _channel = s.getInt32();
        _spot = s.getObject(_spot, G2Spot.class);
        _data = s.getString();
        _associated_channels = s.getObject(_associated_channels, G2ChannelSet.class);
        _info_type = INFO_TYPE.from(s.getInt32());

        if (_info_type == INFO_TYPE.DISK) {
            _info = s.getObject(InfoDisk.class);
        } else if (_info_type == INFO_TYPE.GPS) {
            _info = s.getObject(InfoGPS.class);
        } else if (_info_type == INFO_TYPE.TEXT_IN) {
            _info = s.getObject(InfoTextIn.class);
        } else if (_info_type == INFO_TYPE.NETWORK_ALARM) {
            _info = s.getObject(InfoNetworkAlarm.class);
        } else if (_info_type == INFO_TYPE.USER_DEFINED) {
            _info = s.getObject(InfoUserDefined.class);
        }
    }

    public String toStringType() {
        return G2Foundation.get().stringGetEventTypeG2(_type);
    }

    public TYPE _type = TYPE.NONE;
    public LEVEL _level = LEVEL.NORMAL;
    public int _channel = 0;
    public G2Spot _spot = G2Spot.INVALID.clone();
    public String _data = new String();
    public G2ChannelSet _associated_channels;
    public INFO_TYPE _info_type = INFO_TYPE.NONE;
    public G2Object _info;
}
