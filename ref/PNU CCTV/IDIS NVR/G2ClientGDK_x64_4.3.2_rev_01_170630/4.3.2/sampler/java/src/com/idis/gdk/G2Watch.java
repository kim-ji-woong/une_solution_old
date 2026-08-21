package com.idis.gdk;

import java.nio.ByteBuffer;

import com.sun.jna.Library;
import com.sun.jna.Native;
import com.sun.jna.Pointer;
import com.sun.jna.win32.StdCallLibrary;
import com.idis.gdk.define.*;
import com.idis.gdk.define.live.*;

public class G2Watch {
    public interface Listener {
        void onG2WatchConnected(long handle, int channel);
        void onG2WatchDisconnected(long handle, int channel, G2DisconnectReason reason);
        void onG2WatchReceiveEvent(long handle, int channel, G2EventInfo event);
        void onG2WatchReceiveNetworkAlarmResult(long handle, int channel, G2LiveNetworkAlarmResult res);
        void onG2WatchReceiveElevatorStatusInfoResponse(long handle, int channel, int seq_number);
    }

    public interface G2Library extends Library {
        void j2_watch_register_callback(long handle, int type, FN_LISTENER func);
        long j2_watch_initialize();
        void j2_watch_finalize(long handle);
        void j2_watch_startup(long handle, int connections, Pointer focus);
        void j2_watch_cleanup(long handle);
        int j2_watch_connect_ras(long handle, byte[] pni, int pni_len, byte[] poptions, int poptions_len, byte[] pres, int pres_len);
        int j2_watch_connect_ras_event(long handle, byte[] pni, int pni_len, byte[] poptions, int poptions_len, byte[] pres, int pres_len);
        void j2_watch_disconnect(long handle, int channel);
        int j2_watch_is_connecting(long handle, int channel);
        int j2_watch_is_connected(long handle, int channel);
        int j2_watch_is_disconnecting(long handle, int channel);
        int j2_watch_is_disconnected(long handle, int channel);
        int j2_watch_is_disconnectable(long handle, int channel);
        int j2_watch_send_network_alarm_info(long handle, int channel, byte[] pinfo, int pinfo_len);
        int j2_watch_send_elevator_status_info(long handle, int channel, byte[] pinfo, int pinfo_len);
        int j2_watch_is_support(long handle, int channel, int query);

        public interface FN_LISTENER extends StdCallLibrary.StdCallCallback {
            long invoke(long handle, long wparam, long lparam, Pointer uparam, int uparam_len);
        }
    }

    private enum LISTENER_ID {
        on_connected(0),
        on_disconnected(1),
        on_receive_event(3),
        on_receive_network_alarm_result(15),
        on_receive_elevator_status_info_response(24);

        private int value;
        private LISTENER_ID(int value) { this.value = value; }
        public int to() { return value; }
    }

    public G2Watch() {
        _handle = 0;
        _lib = null;
        _listener = null;
        _p2_on_connected = new p2_on_connected(this);
        _p2_on_disconnected = new p2_on_disconnected(this);
        _p2_on_receive_event = new p2_on_receive_event(this);
        _p2_on_receive_network_alarm_result = new p2_on_receive_network_alarm_result(this);
        _p2_on_receive_elevator_status_info_response = new p2_on_receive_elevator_status_info_response(this); 
    }

    public void startup(int connections) {
        cleanup();

        _lib = (G2Library) Native.loadLibrary(G2Main.get().moduleName(), G2Library.class);
        _handle = _lib.j2_watch_initialize();

        registerCallback(LISTENER_ID.on_connected, _p2_on_connected);
        registerCallback(LISTENER_ID.on_disconnected, _p2_on_disconnected);
        registerCallback(LISTENER_ID.on_receive_event, _p2_on_receive_event);
        registerCallback(LISTENER_ID.on_receive_network_alarm_result, _p2_on_receive_network_alarm_result);
        registerCallback(LISTENER_ID.on_receive_elevator_status_info_response, _p2_on_receive_elevator_status_info_response);

        lib().j2_watch_startup(_handle, connections, null);
    }

    public void cleanup() {
        if (_handle != 0) {
            long handle = _handle;
            _handle = 0;
            _lib.j2_watch_cleanup(handle);
            _lib.j2_watch_finalize(handle);
            _lib = null;
        }
    }

    public int connect(G2NetworkInfo ni, G2ConnectOptions options, G2ConnectResult res) {
        return impConnectRas(ni, options, res, false);
    }

    public int connectEvent(G2NetworkInfo ni, G2ConnectOptions options, G2ConnectResult res) {
        return impConnectRas(ni, options, res, true);
    }

    protected int impConnectRas(G2NetworkInfo ni, G2ConnectOptions options, G2ConnectResult res, boolean event) {
        G2Stream stream_ni = G2Stream.from(ByteBuffer.allocate(512));
        G2Stream stream_options = G2Stream.from(ByteBuffer.allocate(32));
        G2Stream stream_res = G2Stream.from(ByteBuffer.allocate(16));
        stream_ni.putObject(ni);
        stream_options.putObject(options);

        int channel = -1;
        if (event) {
            channel = lib().j2_watch_connect_ras_event(_handle, stream_ni.buf().array(), stream_ni.buf().position(),
                                                                stream_options.buf().array(), stream_options.buf().position(),
                                                                stream_res.buf().array(), stream_res.buf().capacity());
        } else {
            channel = lib().j2_watch_connect_ras(_handle, stream_ni.buf().array(), stream_ni.buf().position(),
                                                          stream_options.buf().array(), stream_options.buf().position(),
                                                          stream_res.buf().array(), stream_res.buf().capacity());
        }

        stream_res.getObject(res);
        return channel;
    }

    public void disconnect(int channel) {
        lib().j2_watch_disconnect(_handle, channel);
    }

    public boolean isConnecting(int channel) {
        return lib().j2_watch_is_connecting(_handle, channel) != 0;
    }

    public boolean isConnected(int channel) {
        return lib().j2_watch_is_connected(_handle, channel) != 0;
    }

    public boolean isDisconnecting(int channel) {
        return lib().j2_watch_is_disconnecting(_handle, channel) != 0;
    }

    public boolean isDisconnected(int channel) {
        return lib().j2_watch_is_disconnected(_handle, channel) != 0;
    }

    public boolean isDisconnectable(int channel) {
        return lib().j2_watch_is_disconnectable(_handle, channel) != 0;
    }

    public boolean sendNetworkAlarmInfo(int channel, G2LiveNetworkAlarmInfo info) {
        G2Stream s = G2Stream.from(ByteBuffer.allocate(512));
        s.putObject(info);
        return lib().j2_watch_send_network_alarm_info(_handle, channel, s.buf().array(), s.buf().position()) != 0;
    }
    
    public boolean sendElevatorStatusInfo(int channel, G2LiveElevatorStatusInfo info) {
        G2Stream s = G2Stream.from(ByteBuffer.allocate(256));
        s.putObject(info);
        return lib().j2_watch_send_elevator_status_info(_handle, channel, s.buf().array(), s.buf().position()) != 0;
    }

    public boolean isSupport(int channel, G2LiveDefine.SUPPORT query) {
        return lib().j2_watch_is_support(_handle, channel, query.to()) != 0;
    }

    private class p2_on_connected implements G2Library.FN_LISTENER {
        public G2Watch adaptor;
        public p2_on_connected(G2Watch a) { this.adaptor = a; }
        public long invoke(long handle, long wparam, long lparam, Pointer uparam, int uparam_len) {
            if (adaptor.isListener()) {
                adaptor.listener().onG2WatchConnected(handle, (int) wparam);
            }
            return 1;
        }
    }

    private class p2_on_disconnected implements G2Library.FN_LISTENER {
        public G2Watch adaptor;
        public p2_on_disconnected(G2Watch a) { this.adaptor = a; }
        public long invoke(long handle, long wparam, long lparam, Pointer uparam, int uparam_len) {
            if (adaptor.isListener()) {
                adaptor.listener().onG2WatchDisconnected(handle, (int) wparam, G2DisconnectReason.from((int) lparam));
            }
            return 1;
        }
    }

    private class p2_on_receive_event implements G2Library.FN_LISTENER {
        public G2Watch adaptor;
        public p2_on_receive_event(G2Watch a) { this.adaptor = a; }
        public long invoke(long handle, long wparam, long lparam, Pointer uparam, int uparam_len) {
            if (adaptor.isListener()) {
                G2Stream s = G2Stream.from(uparam, uparam_len);
                G2EventInfo event = s.getObject(G2EventInfo.class);
                adaptor.listener().onG2WatchReceiveEvent(handle, (int) wparam, event);
            }
            return 1;
        }
    }

    private class p2_on_receive_network_alarm_result implements G2Library.FN_LISTENER {
        public G2Watch adaptor;
        public p2_on_receive_network_alarm_result(G2Watch a) { this.adaptor = a; }
        public long invoke(long handle, long wparam, long lparam, Pointer uparam, int uparam_len) {
            if (adaptor.isListener()) {
                G2Stream s = G2Stream.from(uparam, uparam_len);
                G2LiveNetworkAlarmResult res = s.getObject(G2LiveNetworkAlarmResult.class);
                adaptor.listener().onG2WatchReceiveNetworkAlarmResult(handle, (int) wparam, res);
            }
            return 1;
        }
    }
 
    private class p2_on_receive_elevator_status_info_response implements G2Library.FN_LISTENER {
        public G2Watch adaptor;
        public p2_on_receive_elevator_status_info_response(G2Watch a) { this.adaptor = a; }
        public long invoke(long handle, long wparam, long lparam, Pointer uparam, int uparam_len) {
            if (adaptor.isListener()) {
                adaptor.listener().onG2WatchReceiveElevatorStatusInfoResponse(handle, (int) wparam, (int) lparam);
            }
            return 1;
        }
    }
 
    public void setListener(Listener listener) {
        _listener = listener;
    }

    public G2Library lib() {
        return _lib;
    }
    public Listener listener() {
        return _listener;
    }

    public boolean isListener() {
        return _listener != null;
    }

    public long handle() {
        return _handle;
    }

    protected long _handle = 0;
    protected G2Library _lib;
    protected Listener _listener;

    protected void registerCallback(LISTENER_ID id, G2Library.FN_LISTENER fn) { lib().j2_watch_register_callback(_handle, id.to(), fn); }
    protected p2_on_connected _p2_on_connected;
    protected p2_on_disconnected _p2_on_disconnected;
    protected p2_on_receive_event _p2_on_receive_event;
    protected p2_on_receive_network_alarm_result _p2_on_receive_network_alarm_result;
    protected p2_on_receive_elevator_status_info_response _p2_on_receive_elevator_status_info_response;
}
