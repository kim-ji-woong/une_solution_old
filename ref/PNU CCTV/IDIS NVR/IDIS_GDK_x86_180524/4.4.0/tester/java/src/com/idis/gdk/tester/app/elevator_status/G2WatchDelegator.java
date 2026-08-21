package com.idis.gdk.tester.app.elevator_status;

import com.idis.gdk.G2Foundation;
import com.idis.gdk.G2Watch;
import com.idis.gdk.define.G2DisconnectReason;
import com.idis.gdk.define.G2EventInfo;
import com.idis.gdk.define.G2NetworkInfo;
import com.idis.gdk.define.live.G2LiveDefine;
import com.idis.gdk.define.live.G2LiveElevatorStatusInfo;
import com.idis.gdk.define.live.G2LiveNetworkAlarmResult;

class G2WatchDelegator implements G2Watch.Listener {
    public class Options {
        public G2NetworkInfo ni;
        public G2LiveElevatorStatusInfo status;
        public long timeout = 10;
        public int retry = 0;
    }

    public G2WatchDelegator(G2Watch adaptor) {
        this._adaptor = adaptor;
        this._seq_number = -1;
    }

    public void onG2WatchConnected(long handle, int channel) {
        if (_adaptor.isSupport(channel, G2LiveDefine.SUPPORT.SI_ELEVATOR_STATUS_INFO)) {
            _adaptor.sendElevatorStatusInfo(channel, _options.status);
        } else {
            _adaptor.disconnect(channel);
            _continue = false;
            System.out.println("adaptor::elevator status info not supported");
        }
    }

    public void onG2WatchReceiveEvent(long handle, int channel, G2EventInfo event) {}

    public void onG2WatchDisconnected(long handle, int channel, G2DisconnectReason reason) {
        synchronized (_cond) {
            if (reason == G2DisconnectReason.INVALID_VERSION ||
                reason == G2DisconnectReason.LOGIN_FAIL ||
                reason == G2DisconnectReason.NO_AUTHORITY ||
                reason == G2DisconnectReason.NOT_SUPPORT_PRODUCT) {
                _continue = false;
            }

            if (reason != G2DisconnectReason.LOGOUT) {
                String s = G2Foundation.get().stringGetDisconnectReason(reason);
                System.out.format("disconnected::reason::(%d) %s\n", reason.to(), s);
            }

            _condSignaled = true;
            _cond.notifyAll();
        }
    }

    public void onG2WatchReceiveNetworkAlarmResult(long handle, int channel, G2LiveNetworkAlarmResult res) {}

    public void onG2WatchReceiveElevatorStatusInfoResponse(long handle, int channel, int seq_number) {
        if (_options.status._seq_number == seq_number) {
            _seq_number = seq_number;
            _adaptor.disconnect(channel);
        }
    }

    public Options options() {
        return _options;
    }

    public boolean isContinue() {
        return _continue;
    }

    public boolean operation() {
        int channel = _adaptor.connectEvent(options().ni, null, null);
        if (channel >= 0) {
            synchronized (_cond) {
                if (_condSignaled) {
                    _condSignaled = false;
                } else {
                    try {
                        _cond.wait(options().timeout);
                        _condSignaled = false;
                    } catch (InterruptedException e) {}
                }
            }

            if (_adaptor.isDisconnectable(channel)) {
                _adaptor.disconnect(channel);
            }

            for (; _adaptor.isDisconnecting(channel);) {
                try {
                    Thread.sleep(1);
                } catch (InterruptedException e) {}
            }
        }

        return (_seq_number == _options.status._seq_number);
    }

    public void print() {
        System.out.format("\t%s\n", _options.status.getClass().getName());
        System.out.format("\tseq: %d, received(%d)\n", _options.status._seq_number, _seq_number);
    }

    private G2Watch _adaptor;
    private int _seq_number;
    private Options _options = new Options();
    private boolean _continue = true;
    private boolean _condSignaled = false;
    private Object _cond = new Object();
}
