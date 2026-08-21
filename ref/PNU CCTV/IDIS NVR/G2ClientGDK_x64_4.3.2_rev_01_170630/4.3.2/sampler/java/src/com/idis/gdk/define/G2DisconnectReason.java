package com.idis.gdk.define;

import com.idis.gdk.util.G2Enum;

public enum G2DisconnectReason implements G2Enum.Converter<Integer, G2DisconnectReason> {
    UNKNOWN(0),                             // unknown case
    LOGOUT(1),                              // normally logout (base->post)
    FULL_CHANNEL(2),                        // deny connection because all of server channels are used(base<-post)
    INVALID_VERSION(3),                     // invalid product version (base->post)
    LOGIN_FAIL(4),                          // invalid user or passwd (base<-post)
    ADMIN_CLOSE(5),                         // admin close the current connection forcibly (base<-post)
    ADMIN_TIMEOUT(6),                       // timeout (base<-post)
    SYS_SHUTDOWN(7),                        // post system shutdown (base<-post)
    NO_CHANNEL(8),                          // can't connect - all of my network channels are used
    NO_SERVER(10),                          // can't connect - no server module (sock. err=10061)
    NET_DOWN(11),                           // network is down (sock. err=10050)
    NET_UNREACHABLE(12),                    // network is unreachable (sock. err=10051)
    CONN_TIMEOUT(13),                       // connection time out (sock. err=10060)
    CONN_RESET(14),                         // connection reset by peer (sock. err=10054)
    HOST_DOWN(15),                          // host is down (sock. err=10064)
    HOST_UNREACHABLE(16),                   // no route th host (sock. err=10065)
    CONN_ABORTED(17),                       // connection aborted (sock. err=10053)
    CONN_CANCEL(20),                        // connection has been canceled by user.
    NET_NORESPONSE(21),                     // the peer host does not respond.
    NET_NOISY(22),                          // network is too noisy.
    SEND_OVERFLOW(23),                      // sending queue overflow.
    NO_AUTHORITY(25),                       // You have no authority for search.
    PORT_USED(26),                          // the port is already in use.
    SSL_CONNECTION_FAILED(27),              // SSL connection failed.
    NET_TIMEOUT(28),                        // network is timed out
    HOST_TIMEOUT(29),                       // host is timed out
    NOT_SUPPORT_RTP_TCP(30),                // host cannot support RTP over TCP
    SOCKET_ERROR_OCCURRED(31),              // socket error occurred
    FEN_RENDEZ_CONN_FAILED(32),             // rendezvous service is not available
    FEN_RENDEZ_NO_ELEMENT(33),              // rendezvous Element Not Found
    FEN_RELAY_CONN_FAILED(34),              // relay Connection Failed
    FEN_RELAY_NOT_AVAILABLE(35),            // relay Service is not available
    FEN_DIRECT_CONN_DOWN(36),               // Fen Direct connection is closed
    FEN_UDT_CONN_DOWN(37),                  // Fen UDT connection is closed
    FEN_RELAY_CONN_DOWN(38),                // Fen Relay Connection is closed
    INVALID_RECEIVE_PACKET_BUFFER(1001),    // invalid receive packet buffer
    INVALID_SEND_PACKET_BUFFER(1002),       // invalid send packet buffer
    ALIVE_CHECK_TICKOUT(1003),              // alive check tick out
    RTSP_START_FAILED(2001),                // RTSP session start failed
    RTSP_STOP_FAILED(2002),                 // RTSP session stop failed
    RTSP_IMAGE_NOT_RECEIVED(2003),          // image is not received by rtsp
    RTSP_TEARDOWN_DISCONNECT(2004),         // disconnected when you requests teardown
    RTSP_TUNNELING_DISCONNECT(2005),        // request disconnect of http tunneling channel
    RTSP_SESSION_ALREADY_FINISHED(2006),    // RTSP session is already finished
    RTSP_ALIVE_CHECK_ERROR(2007),           // RTSP alive check error occurred
    RTSP_OVER_ALIVE_CHECK_INTERVAL(2008),   // RTSP over alive check interval
    MISMATCH_ADAPTOR(10000),
    MISMATCH_PORT_UNITY(10001),
    NOT_SUPPORT_PRODUCT(10002);

    static G2Enum.ReverseMap<Integer, G2DisconnectReason> map = new G2Enum.ReverseMap<Integer, G2DisconnectReason>(G2DisconnectReason.class, G2DisconnectReason.UNKNOWN);
    public static G2DisconnectReason from(int id) { return map.get(id); }
    private final int value;
    private G2DisconnectReason(int value) { this.value = value; }
    public Integer to() { return this.value; }
    public G2DisconnectReason get(Integer id) { return map.get(id); }
}
