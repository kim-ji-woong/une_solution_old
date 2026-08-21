package com.idis.gdk.define;

import com.idis.gdk.G2Object;
import com.idis.gdk.G2Stream;
import com.idis.gdk.define.G2FenDefine;

public final class G2ConnectResult extends G2Object implements Cloneable {
    public enum ERROR {
        UNKNOWN(-1),
        NO(0),
        NETWORK_INFO_FIND(1),       // there is no device or no access right for device
        NETWORK_INFO_RESOLVE(2),    // FEN(or DVRNS), mDNS query fail
        ACTIVE_DIRECTORY_UNABLE_CLIENT(3),
        ADAPTOR_INCOMPATIABLE(4);

        private final int value;
        private ERROR(int value) { this.value = value; }
        public boolean equal(int i) { return value == i; }
        public int to() { return value; }
        public static ERROR from(int id) {
            ERROR res = ERROR.UNKNOWN;
            for (ERROR var : ERROR.values()) {
                if (var.equal(id)) {
                    res = var;
                    break;
                }
            }
            return res;
        }
    }

    public G2ConnectResult() {}
    public G2ConnectResult(G2ConnectResult right) {
        this._channel = right._channel;
        this._err = right._err;
        this._err_dvrns = right._err_dvrns;
    }

    public G2ConnectResult clone() {
        return new G2ConnectResult(this);
    }

    public void serialize(G2Stream s) {
        s.putInt32(_channel);
        s.putInt32(_err.to());
        s.putInt32(_err_dvrns.to());
    }

    public void deserialize(G2Stream s) {
        _channel = s.getInt32();
        _err = ERROR.from(s.getInt32());
        _err_dvrns = G2FenDefine.RESULT.from(s.getInt32());
    }

    public int _channel = -1;
    public ERROR _err = ERROR.UNKNOWN;
    public G2FenDefine.RESULT _err_dvrns = G2FenDefine.RESULT.NOT_ERROR;
}
