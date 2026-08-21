package com.idis.gdk.define;

import com.idis.gdk.G2Object;
import com.idis.gdk.G2Stream;

public final class G2ConnectOptions extends G2Object implements Cloneable {
    public G2ConnectOptions() {}
    public G2ConnectOptions(G2ConnectOptions right) {
        this._connection_timeout = right._connection_timeout;
    }

    public G2ConnectOptions clone() {
        return new G2ConnectOptions(this);
    }

    public void serialize(G2Stream s) {
        s.putInt32(_connection_timeout);
    }

    public void deserialize(G2Stream s) {
        _connection_timeout = s.getInt32();
    }

    public int _connection_timeout = 0;
}
