package com.idis.gdk.define;

import com.idis.gdk.G2Foundation;
import com.idis.gdk.G2Object;
import com.idis.gdk.G2Stream;
import com.idis.gdk.define.G2Time;

public final class G2Spot extends G2Object implements Cloneable, Comparable<G2Spot> {
    public static final int INVALID_SEGMENT_ID = 0xFFFFFFFF;
    public static final int InvALID_SPOT_TICK = 0xFFFFFFFF;
    public static final G2Spot INVALID = G2Spot.from();

    public static G2Spot from() {
        return new G2Spot();
    }

    public static G2Spot from(int segment, G2Time time, int tick) {
        return new G2Spot(segment, time, tick);
    }

    public static int compare(G2Spot left, G2Spot right) {
        return left.compareTo(right);
    }

    public G2Spot() {}
    public G2Spot(G2Spot right) {
        this._segment = right._segment;
        this._tick = right._tick;
        this._time = right._time.clone();
    }

    public G2Spot(int segment, G2Time time, int tick) {
        this._segment = segment;
        this._time = time.clone();
        this._tick = tick;
    }

    public G2Spot clone() {
        return new G2Spot(this);
    }

    public int compareTo(G2Spot right) {
        return G2Foundation.get().spotCompare(this._segment, this._time._time, this._tick, right._segment, right._time._time, right._tick);
    }

    public void serialize(G2Stream s) {
        s.putInt32(_segment);
        s.putInt32(_tick);
        s.putObject(_time);
    }

    public void deserialize(G2Stream s) {
        _segment = s.getInt32();
        _tick = s.getInt32();
        _time = s.getObject(G2Time.class);
    }

    public boolean valid() {
        return G2Foundation.get().spotIsValid(_segment, _time._time, _tick);
    }
    
    public String toString() {
        return String.format("(%d) %s %d", _segment, _time.toString(), _tick);
    }

    public int _segment = INVALID_SEGMENT_ID;
    public int _tick = 0;
    public G2Time _time = G2Time.INVALID.clone();
}
