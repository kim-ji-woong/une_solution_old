package com.idis.gdk.define;

import com.idis.gdk.G2Object;
import com.idis.gdk.G2Stream;
import com.idis.gdk.define.G2MacAddress;

public final class G2NetworkInfo extends G2Object implements Cloneable {
    public enum ADDRESS_TYPE {
        UNKNOWN(-1),
        IPV4(0),
        DVRNS(1),
        DNS(2),
        MDNS(3),
        DDNS(4),
        IPV6(5),
        WSDISCOVERY(6);

        private final int value;
        private ADDRESS_TYPE(int value) { this.value = value; }
        public boolean equal(int i) { return value == i; }
        public int to() { return value; }
        public static ADDRESS_TYPE from(int id) {
            ADDRESS_TYPE res = ADDRESS_TYPE.UNKNOWN;
            for (ADDRESS_TYPE var : ADDRESS_TYPE.values()) {
                if (var.equal(id)) {
                    res = var;
                    break;
                }
            }
            return res;
        }
    }

    public enum COMMAND_PROTOCOL_TYPE {
        UNKNOWN(-1),
        IDIS(0),
        ONVIF(1),
        HTTP_TCP(2),
        COMBINE_ONVIF_HTTP(3);

        private final int value;
        private COMMAND_PROTOCOL_TYPE(int value) { this.value = value; }
        public boolean equal(int i) { return value == i; }
        public int to() { return value; }
        public static COMMAND_PROTOCOL_TYPE from(int id) {
            COMMAND_PROTOCOL_TYPE res = COMMAND_PROTOCOL_TYPE.UNKNOWN;
            for (COMMAND_PROTOCOL_TYPE var : COMMAND_PROTOCOL_TYPE.values()) {
                if (var.equal(id)) {
                    res = var;
                    break;
                }
            }
            return res;
        }
    }

    public enum MEDIA_STREAM_PROTOCOL_TYPE {
        UNKNOWN(-1),
        IDIS(0),
        RTSP_RTP_UDP_UNICAST(10),
        RTSP_RTP_UDP_MULTICAST(11),
        RTSP_RTP_TCP(12),
        HTTP_RTP_UDP_UNICAST(20),
        HTTP_TCP(21),
        RTP_RTSP_HTTP_TCP(30);

        private final int value;
        private MEDIA_STREAM_PROTOCOL_TYPE(int value) { this.value = value; }
        public boolean equal(int i) { return value == i; }
        public int to() { return value; }
        public static MEDIA_STREAM_PROTOCOL_TYPE from(int id) {
            MEDIA_STREAM_PROTOCOL_TYPE res = MEDIA_STREAM_PROTOCOL_TYPE.UNKNOWN;
            for (MEDIA_STREAM_PROTOCOL_TYPE var : MEDIA_STREAM_PROTOCOL_TYPE
                    .values()) {
                if (var.equal(id)) {
                    res = var;
                    break;
                }
            }
            return res;
        }
    }

    public enum PORT_TYPE {
        MIN_PORT_INDEX(0),
        SERVICE(0),
        ADMIN(1),
        WATCH(2),
        SEARCH(3),
        RECORD(4),
        AUDIO(5),
        WEB(6),
        RTP(7),
        RTSP(8),
        EXTRA_SERVER(9),
        HTTPS(10),
        MAX(11);

        private final int value;
        private PORT_TYPE(int value) { this.value = value; }
        public boolean equal(int i) { return value == i; }
        public int to() { return value; }
        public static PORT_TYPE from(int id) {
            PORT_TYPE res = PORT_TYPE.MIN_PORT_INDEX;
            for (PORT_TYPE var : PORT_TYPE.values()) {
                if (var.equal(id)) {
                    res = var;
                    break;
                }
            }
            return res;
        }
    }

    public enum ONVIF_SERVICE_PATH_TYPE {
        DEVICE(0),
        MEDIA(1),
        PTZ(2),
        IMAGING(3),
        MAX(4);

        private final int value;
        private ONVIF_SERVICE_PATH_TYPE(int value) { this.value = value; }
        public boolean equal(int i) { return value == i; }
        public int to() { return value; }
        public static ONVIF_SERVICE_PATH_TYPE from(int id) {
            ONVIF_SERVICE_PATH_TYPE res = ONVIF_SERVICE_PATH_TYPE.DEVICE;
            for (ONVIF_SERVICE_PATH_TYPE var : ONVIF_SERVICE_PATH_TYPE.values()) {
                if (var.equal(id)) {
                    res = var;
                    break;
                }
            }
            return res;
        }
    }

    public enum ONVIF_SERVICE_PATH_EXTRA_TYPE {
        EVENT_SERVICE(0),
        ANALYTICS(1),
        MAX(2);

        private final int value;
        private ONVIF_SERVICE_PATH_EXTRA_TYPE(int value) { this.value = value; }
        public boolean equal(int i) { return value == i; }
        public int to() { return value; }
        public static ONVIF_SERVICE_PATH_EXTRA_TYPE from(int id) {
            ONVIF_SERVICE_PATH_EXTRA_TYPE res = ONVIF_SERVICE_PATH_EXTRA_TYPE.EVENT_SERVICE;
            for (ONVIF_SERVICE_PATH_EXTRA_TYPE var : ONVIF_SERVICE_PATH_EXTRA_TYPE
                    .values()) {
                if (var.equal(id)) {
                    res = var;
                    break;
                }
            }
            return res;
        }
    }

    public enum MEDIA_CONNECT_TYPE {
        STREAM(0),
        RECORD(1),
        SEARCH(2),
        MONITORING(3),
        MAX(4);

        private final int value;
        private MEDIA_CONNECT_TYPE(int value) { this.value = value; }
        public boolean equal(int i) { return value == i; }
        public int to() { return value; }
        public static MEDIA_CONNECT_TYPE from(int id) {
            MEDIA_CONNECT_TYPE res = MEDIA_CONNECT_TYPE.STREAM;
            for (MEDIA_CONNECT_TYPE var : MEDIA_CONNECT_TYPE.values()) {
                if (var.equal(id)) {
                    res = var;
                    break;
                }
            }
            return res;
        }
    }

    public static class URI extends G2Object {
        public URI() {}
        public URI(URI right) {
            this._rtsp_tcp = right._rtsp_tcp;
            this._rtsp_http = right._rtsp_http;
            this._profile_token = right._profile_token;
        }

        public URI clone() {
            return new URI(this);
        }

        public void serialize(G2Stream s) {
            s.putString(_rtsp_tcp);
            s.putString(_rtsp_http);
            s.putString(_profile_token);
        }

        public void deserialize(G2Stream s) {
            _rtsp_tcp = s.getString();
            _rtsp_http = s.getString();
            _profile_token = s.getString();
        }

        public String _rtsp_tcp;
        public String _rtsp_http;
        public String _profile_token;
    }

    public G2NetworkInfo() {}
    public G2NetworkInfo(G2NetworkInfo right) {
        this._version = right._version.clone();
        this._address_type = right._address_type;
        this._command_protocol_type = right._command_protocol_type;
        this._media_stream_protocol_type = right._media_stream_protocol_type;
        this._media_record_protocol_type = right._media_record_protocol_type;
        this._media_search_protocol_type = right._media_search_protocol_type;
        this._address = right._address;
        this._address_resolved = right._address_resolved;
        this._user_id = right._user_id;
        this._password = right._password;
        this._extra_server_address = right._extra_server_address;
        this._mac = right._mac.clone();
        this._port = right._port.clone();
        this._profile_token = right._profile_token;
        this._rtsp_uri = right._rtsp_uri;
        this._rtsp_uri_http = right._rtsp_uri_http;
        System.arraycopy(right._onvif_service_path, 0, this._onvif_service_path, 0, ONVIF_SERVICE_PATH_TYPE.MAX.to());
        System.arraycopy(right._onvif_service_path_extra, 0, this._onvif_service_path_extra, 0, ONVIF_SERVICE_PATH_EXTRA_TYPE.MAX.to());
        this._uri = right._uri != null ? right._uri.clone() : null;
    }

    public G2NetworkInfo clone() {
        return new G2NetworkInfo(this);
    }

    public void serialize(G2Stream s) {
        s.putObject(_version);
        s.putInt32(_address_type.to());
        s.putInt32(_command_protocol_type.to());
        s.putInt32(_media_stream_protocol_type.to());
        s.putInt32(_media_record_protocol_type.to());
        s.putInt32(_media_search_protocol_type.to());
        s.putString(_address);
        s.putString(_address_resolved);
        s.putString(_user_id);
        s.putString(_password);
        s.putString(_extra_server_address);
        s.putObject(_mac);
        s.putVector(_port);
        s.putString(_profile_token);
        s.putString(_rtsp_uri);
        s.putString(_rtsp_uri_http);
        s.putVector(_onvif_service_path);
        s.putVector(_onvif_service_path_extra);
        s.putVector(_uri);
    }

    public void deserialize(G2Stream s) {
        _version = s.getObject(_version, G2VersionNetwork.class);
        _address_type = ADDRESS_TYPE.from(s.getInt32());
        _command_protocol_type = COMMAND_PROTOCOL_TYPE.from(s.getInt32());
        _media_stream_protocol_type = MEDIA_STREAM_PROTOCOL_TYPE.from(s.getInt32());
        _media_record_protocol_type = MEDIA_STREAM_PROTOCOL_TYPE.from(s.getInt32());
        _media_search_protocol_type = MEDIA_STREAM_PROTOCOL_TYPE.from(s.getInt32());
        _address = s.getString();
        _address_resolved = s.getString();
        _user_id = s.getString();
        _password = s.getString();
        _extra_server_address = s.getString();
        _mac = s.getObject(_mac, G2MacAddress.class);
        _port = s.getVectorInt16();
        _profile_token = s.getString();
        _rtsp_uri = s.getString();
        _rtsp_uri_http = s.getString();
        _onvif_service_path = s.getVectorString();
        _onvif_service_path_extra = s.getVectorString();
        _uri = s.getVectorObject(URI.class);
    }
    
    public void setPort(PORT_TYPE type, short port) {
        _port[type.to()] = port;
    }

    public short getPort(PORT_TYPE type) {
        return _port[type.to()];
    }

    public G2VersionNetwork _version = new G2VersionNetwork();
    public ADDRESS_TYPE _address_type = ADDRESS_TYPE.IPV4;
    public COMMAND_PROTOCOL_TYPE _command_protocol_type = COMMAND_PROTOCOL_TYPE.UNKNOWN;
    public MEDIA_STREAM_PROTOCOL_TYPE _media_stream_protocol_type = MEDIA_STREAM_PROTOCOL_TYPE.UNKNOWN;
    public MEDIA_STREAM_PROTOCOL_TYPE _media_record_protocol_type = MEDIA_STREAM_PROTOCOL_TYPE.UNKNOWN;
    public MEDIA_STREAM_PROTOCOL_TYPE _media_search_protocol_type = MEDIA_STREAM_PROTOCOL_TYPE.UNKNOWN;
    public String _address = new String();
    public String _address_resolved = new String();
    public String _user_id = new String();
    public String _password = new String();
    public String _extra_server_address = new String();
    public G2MacAddress _mac = new G2MacAddress();
    public short[] _port = new short[PORT_TYPE.MAX.to()];
    public String _profile_token = new String();
    public String _rtsp_uri = new String();
    public String _rtsp_uri_http = new String();
    public String[] _onvif_service_path = new String[ONVIF_SERVICE_PATH_TYPE.MAX.to()];
    public String[] _onvif_service_path_extra = new String[ONVIF_SERVICE_PATH_EXTRA_TYPE.MAX.to()];
    public URI[] _uri;
}
