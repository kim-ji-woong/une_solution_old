package com.idis.gdk.define.live;

import com.idis.gdk.G2Object;
import com.idis.gdk.G2Stream;
import com.idis.gdk.define.G2Event;

public class G2LiveNetworkAlarmResult extends G2Object implements Cloneable {
    public enum TYPE {
        RESULT_OK(0),
        FAIL_UNKNOWN(1),
        FAIL_NO_OPERATION(2),
        FAIL_DEVICE_INACTIVE(3);

        private int value;
        private TYPE(int value) { this.value = value; }
        public boolean equal(int i) { return value == i; }
        public int to() { return value; }
        public static TYPE from(int id) {
            TYPE res = TYPE.FAIL_UNKNOWN;
            for (TYPE var : TYPE.values()) {
                if (var.equal(id)) {
                    res = var;
                    break;
                }
            }
            return res;
        }
    }

    public G2LiveNetworkAlarmResult() {}
    public G2LiveNetworkAlarmResult(G2LiveNetworkAlarmResult right) {
    	this._seq_number = right._seq_number;
        this._result = right._result;
        this._event = right._event != null ? right._event.clone() : null;
    }

    public G2LiveNetworkAlarmResult clone() {
        return new G2LiveNetworkAlarmResult(this);
    }

    public void serialize(G2Stream s) {
    	s.putInt32(_seq_number);
        s.putInt32(_result.to());
        s.putObject(_event);
    }

    public void deserialize(G2Stream s) {
    	_seq_number = s.getInt32();
        _result = TYPE.from(s.getInt32());
        _event = s.getObject(_event, G2Event.class);
    }

    public int _seq_number = 0;
    public TYPE _result = TYPE.RESULT_OK;
    public G2Event _event;
}
