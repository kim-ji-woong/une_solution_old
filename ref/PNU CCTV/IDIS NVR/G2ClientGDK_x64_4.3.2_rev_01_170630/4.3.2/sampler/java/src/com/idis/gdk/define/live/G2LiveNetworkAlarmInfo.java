package com.idis.gdk.define.live;
import com.idis.gdk.G2Object;
import com.idis.gdk.G2Stream;
import com.idis.gdk.define.G2Event;
import com.idis.gdk.define.G2Time;

public class G2LiveNetworkAlarmInfo extends G2Object implements Cloneable
{
    public G2LiveNetworkAlarmInfo() {}
    public G2LiveNetworkAlarmInfo(G2LiveNetworkAlarmInfo right)
    {
        System.arraycopy(right._version, 0, this._version, 0, 2);
        this._seq_number = right._seq_number;
        this._id = right._id;
        this._level = right._level;
        this._event = right._event;
        this._on = right._on;
        this._data = right._data;
        this._time = right._time.clone();
        this._msec = right._msec;
    }
    public G2LiveNetworkAlarmInfo clone()
    {
        return new G2LiveNetworkAlarmInfo(this);
    }
    public void serialize(G2Stream s)
    {
    	s.putInt32(_seq_number);
        s.putVector(_version);
        s.putInt32(_id);
        s.putInt32(_level.to());
        s.putInt32(_event);
        s.putBool(_on);
        s.putString(_data);
        s.putObject(_time);
        s.putInt64(_msec);
    }
    public void deserialize(G2Stream s)
    {
    	_seq_number = s.getInt32();
        _version = s.getVectorInt8(_version);
        _id = s.getInt32();
        _level = G2Event.LEVEL.from(s.getInt32());
        _event = s.getInt32();
        _on = s.getBool();
        _data = s.getString();
        _time = s.getObject(_time, G2Time.class);
        _msec = s.getInt64();
    }

    public int _seq_number = 0;
    public byte[] _version = new byte[2];
    public int _id = 0;     // channel of G2EVENT, device number or ID
    public G2Event.LEVEL _level = G2Event.LEVEL.NORMAL;
    public int _event = 0;  // user defined type
    public boolean _on = true;
    public String _data = new String();
    public G2Time _time = G2Time.INVALID.clone();
    public long _msec = 0;
}
