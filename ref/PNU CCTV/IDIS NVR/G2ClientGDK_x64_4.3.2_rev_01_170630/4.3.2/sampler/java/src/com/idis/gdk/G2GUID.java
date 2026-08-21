package com.idis.gdk;

import java.nio.ByteBuffer;
import com.sun.jna.Native;
import com.sun.jna.win32.StdCallLibrary;

public final class G2GUID extends G2Object implements Cloneable, Comparable<G2GUID> {
    public static final G2GUID INVALID = G2GUID.from();

    public static class GUID extends G2Object implements Cloneable {
        public GUID() {}
        public GUID(GUID right) {
            this._data1 = right._data1;
            this._data2 = right._data2;
            this._data3 = right._data3;
            this._data4 = right._data4.clone();
        }

        public GUID clone() {
            return new GUID(this);
        }

        public void serialize(G2Stream s) {
            s.putInt32(_data1);
            s.putInt16(_data2);
            s.putInt16(_data3);
            s.putVector(_data4);
        }

        public void deserialize(G2Stream s) {
            _data1 = s.getInt32();
            _data2 = s.getInt16();
            _data3 = s.getInt16();
            _data4 = s.getVectorInt8(_data4);
        }
        
        public String toString() {
            byte[] data4 = _data4;
            if (data4 == null ||
                data4.length != 8) {
                data4 = INVALID._GUID._data4;
            }
            return String.format("%08x-%04x-%04x-%02x%02x%02x%02x-%02x%02x%02x%02x", _data1, _data2, _data3,
                                 data4[0], data4[1], data4[2], data4[3], data4[4], data4[5], data4[6], data4[7]);
        }

        public int _data1 = 0;
        public short _data2 = 0;
        public short _data3 = 0;
        public byte[] _data4 = new byte[8];
    }

    public static G2GUID from() {
        byte[] buf = new byte[32];
        lib().j2_guid_make_invalid(buf, buf.length);
        return G2Stream.from(ByteBuffer.wrap(buf)).getObject(G2GUID.class);
    }

    public static int compare(G2GUID left, G2GUID right) {
        return left.compareTo(right);
    }

    public G2GUID() {}
    public G2GUID(G2GUID right) {
        this._GUID = right._GUID.clone();
        this._type = right._type;
        this._hash = right._hash;
    }

    public G2GUID clone() {
        return new G2GUID(this);
    }

    public int hashCode() {
        if (_hash == 0) {
            G2Stream stream = serialize();
            _hash = lib().j2_guid_get_hash_value(stream.buf().array(), stream.buf().position());
        }
        return _hash;
    }

    public int compareTo(G2GUID right) {
        G2Stream l = serialize();
        G2Stream r = right.serialize();
        return lib().j2_guid_compare(l.buf().array(), l.buf().position(), r.buf().array(), r.buf().position());
    }

    public G2Stream serialize() {
        G2Stream res = G2Stream.from(ByteBuffer.allocate(32));
        serialize(res);
        return res;
    }

    public void serialize(G2Stream s) {
        s.putObject(_GUID);
        s.putInt32(_type);
        s.putInt32(_hash);
    }

    public void deserialize(G2Stream s) {
        _GUID = s.getObject(_GUID, GUID.class);
        _type = s.getInt32();
        _hash = s.getInt32();
    }

    public boolean valid() {
        G2Stream stream = serialize();
        return lib().j2_guid_is_valid(stream.buf().array(), stream.buf().position()) != 0;
    }
    
    public String toString() {
        return String.format("%08x-%s", _type, _GUID.toString());
    }

    public static G2GUIDLib.G2Library lib() {
        return G2GUIDLib.get().lib();
    }

    private GUID _GUID = new GUID();
    private int _type = 0;
    private int _hash = 0;

    public static class G2GUIDLib {
        public interface G2Library extends StdCallLibrary {
            void j2_guid_make_invalid(byte[] guid, int guid_len);
            int j2_guid_get_hash_value(byte[] guid, int guid_len);
            int j2_guid_compare(byte[] l_guid, int l_guid_len, byte[] r_guid, int r_guid_len);
            int j2_guid_is_valid(byte[] guid, int guid_len);
        }

        public void startup() {
            if (_lib == null) {
                _lib = (G2Library) Native.loadLibrary(G2Main.get().moduleName(), G2Library.class);
            }
        }
        
        public void cleanup() {
            _lib = null;
        }
 
        public G2Library lib() {
            return _lib;
        }

        protected G2GUIDLib() {}
        protected G2Library _lib;
        protected static G2GUIDLib s_singleton = new G2GUIDLib();
        public static G2GUIDLib get() {
            return s_singleton;
        }
    }
}
