package com.idis.gdk.define.live;

import com.idis.gdk.G2Object;
import com.idis.gdk.G2Stream;
import com.idis.gdk.define.G2CodecInfo;

public class G2LiveElevatorStatusInfo extends G2Object implements Cloneable {
    public G2LiveElevatorStatusInfo() {}
    public G2LiveElevatorStatusInfo(G2LiveElevatorStatusInfo right) {
        this._seq_number = right._seq_number;
        this._status = right._status.clone();
    }

    public G2LiveElevatorStatusInfo clone() {
        return new G2LiveElevatorStatusInfo(this);
    }

    public void serialize(G2Stream s) {
        s.putInt32(_seq_number);
        s.putObject(_status);
    }

    public void deserialize(G2Stream s) {
        _seq_number = s.getInt32();
        _status = s.getObject(_status, G2CodecInfo.ElevatorStatus.class);
    }

    public int _seq_number = 0;
    public G2CodecInfo.ElevatorStatus _status = new G2CodecInfo.ElevatorStatus();
}
