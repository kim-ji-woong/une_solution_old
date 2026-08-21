package com.idis.gdk.define;

import com.idis.gdk.G2Object;
import com.idis.gdk.G2Stream;

public final class G2MacAddress extends G2Object implements Cloneable {
    public static final int LEN = 6;

    public G2MacAddress() {}
    public G2MacAddress(G2MacAddress right) {
        System.arraycopy(right._mac, 0, this._mac, 0, LEN);
    }

    public G2MacAddress clone() {
        return new G2MacAddress(this);
    }

    public void serialize(G2Stream s) {
        s.putVector(_mac);
    }

    public void deserialize(G2Stream s) {
        _mac = s.getVectorInt8(_mac);
    }

    public byte[] _mac = new byte[LEN];
}
