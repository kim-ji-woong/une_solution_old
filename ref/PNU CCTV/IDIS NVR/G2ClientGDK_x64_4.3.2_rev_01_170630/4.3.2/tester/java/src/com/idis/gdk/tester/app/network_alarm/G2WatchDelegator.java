package com.idis.gdk.tester.app.network_alarm;

import com.idis.gdk.G2Foundation;
import com.idis.gdk.G2Watch;
import com.idis.gdk.define.G2DisconnectReason;
import com.idis.gdk.define.G2Event;
import com.idis.gdk.define.G2EventInfo;
import com.idis.gdk.define.G2NetworkInfo;
import com.idis.gdk.define.live.G2LiveDefine;
import com.idis.gdk.define.live.G2LiveNetworkAlarmInfo;
import com.idis.gdk.define.live.G2LiveNetworkAlarmResult;

class G2WatchDelegator implements G2Watch.Listener {
    public class Options {
        public G2NetworkInfo ni;
        public G2LiveNetworkAlarmInfo alarm;
        public long timeout = 10;
        public int retry = 0;
    }

    public G2WatchDelegator(G2Watch adaptor) {
        this._adaptor = adaptor;
        this._res._result = G2LiveNetworkAlarmResult.TYPE.FAIL_UNKNOWN;
    }

    public void onG2WatchConnected(long handle, int channel) {
        if (_adaptor.isSupport(channel, G2LiveDefine.SUPPORT.NETWORK_ALARM_G2)) {
            _adaptor.sendNetworkAlarmInfo(channel, _options.alarm);
        } else {
            _adaptor.disconnect(channel);
            _continue = false;
            System.out.println("adaptor::network alarm not supported");
        }
    }
    
    public void onG2WatchReceiveEvent(long handle, int channel, G2EventInfo event) {

    }

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
        
        return (_res._result == G2LiveNetworkAlarmResult.TYPE.RESULT_OK ||
                _res._result == G2LiveNetworkAlarmResult.TYPE.FAIL_NO_OPERATION);
    }
    
    public void print() {
        if (_res != null) {
            print(_res);
        }
    }

    public static void print(G2LiveNetworkAlarmResult res) {
        System.out.format("\t%s\n", res.getClass().getName());
        System.out.format("\tseq: %d\n", res._seq_number);
        System.out.format("\tresult: %s\n", res._result.toString());

        if (res._event != null) {
            System.out.format("\tevent: %s\n", res._event.toStringType());
            System.out.format("\tlevel: %s\n", res._event._level.toString());
            System.out.format("\tdata: %s\n", res._event._data);

            if (res._event._info instanceof G2Event.InfoNetworkAlarm) {
                G2Event.InfoNetworkAlarm info = (G2Event.InfoNetworkAlarm) res._event._info;
                System.out.format("\tversion: %d.%d\n", info._version[1], info._version[0]);
                System.out.format("\tuser_defined: %d\n", info._event);
                System.out.format("\ttime: %s\n", info._time.to().getTime().toString());
                System.out.format("\tmsec: %d\n", info._msec);
            }
        }
    }

    private G2Watch _adaptor;
    private G2LiveNetworkAlarmResult _res = new G2LiveNetworkAlarmResult();
    private Options _options = new Options();
    private boolean _continue = true;
    private boolean _condSignaled = false;
    private Object _cond = new Object();
}
