package com.idis.gdk.define;

public class G2FenDefine {
    public enum RESULT {
        NOT_ERROR(0),
        NETWORK_ERROR(1),
        NETWORK_INITIALIZE_ERROR(2),
        NETWORK_PROTOCOL_ERROR(3),
        INVALID_PACKET(4),
        INVALID_PACKET_VERSION(5),
        NETWORK_INITIALIZE_ERROR_HELPDESK(6),
        NETWORK_INITIALIZE_ERROR_NOTIFY(7),
        NETWORK_INITIALIZE_ERROR_MYIPADDRESS(8),
        QUERY_ERROR(200),
        NO_NAME_ERROR(1),
        NAME_TABLE_RETRIEVE_ERROR(2),
        NEW_DDNS_RESPONSE(3),
        REMOVE_FAIL(4);

        private final int value;
        private RESULT(int value) { this.value = value; }
        public boolean equal(int i) { return value == i; }
        public int to() { return value; }
        public static RESULT from(int id) {
            RESULT res = RESULT.NOT_ERROR;
            for (RESULT var : RESULT.values()) {
                if (var.equal(id)) {
                    res = var;
                    break;
                }
            }
            return res;
        }
    }
}
