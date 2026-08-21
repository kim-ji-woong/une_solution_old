package com.idis.gdk.define;

import com.idis.gdk.G2Object;
import com.idis.gdk.G2Stream;

public final class G2VersionNetwork extends G2Object implements Cloneable {
    public enum TYPE {
        NETWORKINFO_1_0(0x00000100),
        NETWORKINFO_2_0(0x00000200),
        NETWORKINFO_2_1(0x00000201),
        NETWORKINFO_3_0(0x00000300),
        NETWORKINFO(0x00000300);

        private int value;
        private TYPE(int value) { this.value = value; }
        public boolean equal(int i) { return value == i; }
        public int to() { return value; }
        public static TYPE from(int id) {
            TYPE res = TYPE.NETWORKINFO;
            for (TYPE var : TYPE.values()) {
                if (var.equal(id)) {
                    res = var;
                    break;
                }
            }
            return res;
        }
    }

    public G2VersionNetwork() {}
    public G2VersionNetwork(G2VersionNetwork right) {
        this._version = right._version;
    }

    public G2VersionNetwork clone() {
        return new G2VersionNetwork(this);
    }

    public void serialize(G2Stream s) {
        s.putInt32(_version.to());
    }

    public void deserialize(G2Stream s) {
        _version = TYPE.from(s.getInt32());
    }

    public TYPE _version = TYPE.NETWORKINFO;
}
