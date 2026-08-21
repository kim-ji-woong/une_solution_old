package com.idis.gdk.define;

import java.util.SortedSet;
import java.util.TreeSet;
import com.idis.gdk.G2Object;
import com.idis.gdk.G2Stream;

public final class G2ChannelSet extends G2Object implements Cloneable {
    public static G2ChannelSet from(int chs) {
        G2ChannelSet res = new G2ChannelSet();
        res.fromInt32(chs);
        return res;
    }

    public static G2ChannelSet from(int[] chs) {
        G2ChannelSet res = new G2ChannelSet();
        res.fromArray(chs);
        return res;
    }

    public G2ChannelSet() {}
    public G2ChannelSet(G2ChannelSet right) {
        if (right._channels != null && right._channels.isEmpty() != true) {
            this._channels.addAll(right._channels);
        }
    }

    public G2ChannelSet clone() {
        return new G2ChannelSet(this);
    }

    public void serialize(G2Stream s) {
        int[] chs = toArray();
        s.putVector(chs);
    }

    public void deserialize(G2Stream s) {
        fromArray(s.get_vector_int32());
    }

    public boolean insert(int ch) {
        return _channels.add(ch);
    }

    public boolean erase(int ch) {
        return _channels.remove(ch);
    }

    public boolean contains(int ch) {
        return _channels.contains(ch);
    }

    public int[] toArray() {
        int[] res = new int[_channels.size()];
        int i = 0;
        for (Integer var : _channels) {
            res[i++] = var;
        }
        return res;
    }

    public void fromInt32(int chs) {
        for (int i = 0; i < 32; ++i) {
            if ((chs & (1 << i)) != 0) {
                _channels.add(i);
            }
        }
    }

    public void fromArray(final int[] chs) {
        _channels.clear();
        for (int var : chs) {
            _channels.add(var);
        }
    }

    public SortedSet<Integer> _channels = new TreeSet<Integer>();
}
