package com.idis.gdk.define;

import java.util.Calendar;

import com.idis.gdk.G2Foundation;
import com.idis.gdk.G2Object;
import com.idis.gdk.G2Stream;

public final class G2Time extends G2Object implements Cloneable, Comparable<G2Time> {
    public static final G2Time INVALID = G2Time.from();

    public static G2Time current() {
        G2Time res = new G2Time(G2Foundation.get().timeGetCurrent());
        return res;
    }

    public static G2Time from() {
        G2Time res = new G2Time(G2Foundation.get().timeGetInvalid());
        return res;
    }

    public static G2Time from(int year, int month, int day, int hour, int minute, int second) {
        G2Time res = new G2Time(G2Foundation.get().timeFrom(year, month, day, hour, minute, second));
        return res;
    }

    public static G2Time from(Calendar time) {
        G2Time res = new G2Time(time);
        return res;
    }

    public static int compare(G2Time left, G2Time right) {
        return left.compareTo(right);
    }

    public G2Time() {
        this._time = INVALID._time;
    }
    public G2Time(G2Time right) {
        this._time = right._time;
    }

    public G2Time(int time) {
        this._time = time;
    }

    public G2Time(int year, int month, int day, int hour, int minute, int second) {
        this._time = G2Foundation.get().timeFrom(year, month, day, hour, minute, second);
    }
    
    public G2Time(Calendar time) {
        this._time = G2Foundation.get().timeFrom(time.get(Calendar.YEAR), time.get(Calendar.MONTH), time.get(Calendar.DAY_OF_MONTH),
                                                 time.get(Calendar.HOUR_OF_DAY), time.get(Calendar.MINUTE), time.get(Calendar.SECOND));
    }

    public G2Time clone() {
        return new G2Time(this);
    }
    
    public int hashCode() {
        return _time;
    }

    public int compareTo(G2Time right) {
        return G2Foundation.get().timeCompare(this._time, right._time);
    }

    public void serialize(G2Stream s) {
        s.putInt32(_time);
    }

    public void deserialize(G2Stream s) {
        _time = s.getInt32();
    }

    public int year() {
        return G2Foundation.get().timeGetYear(_time);
    }

    public int month() {
        return G2Foundation.get().timeGetMonth(_time);
    }

    public int day() {
        return G2Foundation.get().timeGetDay(_time);
    }

    public int hour() {
        return G2Foundation.get().timeGetHour(_time);
    }

    public int minute() {
        return G2Foundation.get().timeGetMinute(_time);
    }

    public int second() {
        return G2Foundation.get().timeGetSecond(_time);
    }

    public boolean valid() {
        return G2Foundation.get().timeIsValid(_time);
    }

    public Calendar to() {
        Calendar c = Calendar.getInstance();
        c.set(year(), month(), day(), hour(), minute(), second());
        return c;
    }
    
    public String toString() {
        return to().getTime().toString();
    }

    public int _time = 0;
}
