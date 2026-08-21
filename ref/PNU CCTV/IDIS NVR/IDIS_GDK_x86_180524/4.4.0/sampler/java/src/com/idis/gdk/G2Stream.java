package com.idis.gdk;

import java.awt.Dimension;
import java.awt.Rectangle;
import java.lang.reflect.Array;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.nio.charset.Charset;

import com.sun.jna.Pointer;

public class G2Stream {
    public static Charset _default_charset = Charset.forName("UTF-8");

    public static G2Stream from(ByteBuffer buf) {
        return new G2Stream(buf);
    }

    public static G2Stream from(ByteBuffer buf, ByteOrder order) {
        return new G2Stream(buf, order);
    }

    public static G2Stream from(Pointer ptr, int len) {
        return new G2Stream(ptr, len);
    }

    public static G2Stream from(Pointer ptr, int offset, int len, ByteOrder order) {
        return new G2Stream(ptr, offset, len, order);
    }

    public G2Stream(ByteBuffer buf) {
        _buf = buf;
        _buf.order(ByteOrder.LITTLE_ENDIAN);
    }

    public G2Stream(ByteBuffer buf, ByteOrder order) {
        _buf = buf;
        _buf.order(order);
    }

    public G2Stream(Pointer ptr, int len) {
        _buf = ptr.getByteBuffer(0, len);
        _buf.order(ByteOrder.LITTLE_ENDIAN);
    }

    public G2Stream(Pointer ptr, int offset, int len, ByteOrder order) {
        _buf = ptr.getByteBuffer(offset, len);
        _buf.order(order);
    }

    public void putObject(G2Object s) {
        if (s == null) {
            _buf.put((byte) 0);
        } else {
            _buf.put((byte) 1);
            s.serialize(this);
        }
    }

    public void putString(String s) {
        if (s == null) {
            _buf.putInt(0);
        } else {
            byte[] buf = s.getBytes(_default_charset);
            _buf.putInt(buf.length);
            _buf.put(buf);
        }
    }

    public void putInt64(long s) {
        _buf.putLong(s);
    }

    public void putInt32(int s) {
        _buf.putInt(s);
    }

    public void putInt16(short s) {
        _buf.putShort(s);
    }

    public void putInt8(byte s) {
        _buf.put(s);
    }

    public void putFloat32(float s) {
        _buf.putFloat(s);
    }

    public void putFloat64(double s) {
        _buf.putDouble(s);
    }

    public void putBool(boolean s) {
        _buf.put((byte) (s ? 1 : 0));
    }

    public void putVector(G2Object[] a) {
        if (a == null) {
            _buf.putInt(0);
        } else {
            _buf.putInt(a.length);
            for (int i = 0; i < a.length; ++i) {
                G2Object s = a[i];
                if (s == null) {
                    _buf.put((byte) 0);
                } else {
                    _buf.put((byte) 1);
                    s.serialize(this);
                }
            }
        }
    }

    public void putVector(String[] a) {
        if (a == null) {
            _buf.putInt(0);
        } else {
            _buf.putInt(a.length);
            for (int i = 0; i < a.length; ++i) {
                String s = a[i];
                if (s == null) {
                    _buf.putInt(0);
                } else {
                    byte[] buf = s.getBytes(_default_charset);
                    _buf.putInt(buf.length);
                    _buf.put(buf);
                }
            }
        }
    }

    public void putVector(int[] a) {
        if (a == null) {
            _buf.putInt(0);
        } else {
            _buf.putInt(a.length);
            for (int var : a) {
                _buf.putInt(var);
            }
        }
    }

    public void putVector(short[] a) {
        if (a == null) {
            _buf.putInt(0);
        } else {
            _buf.putInt(a.length);
            for (int i = 0; i < a.length; ++i) {
                _buf.putShort(a[i]);
            }
        }
    }

    public void putVector(byte[] a) {
        if (a == null) {
            _buf.putInt(0);
        } else {
            _buf.putInt(a.length);
            _buf.put(a);
        }
    }

    public void putDimension(Dimension s) {
        if (s == null) {
            _buf.put((byte) 0);
        } else {
            _buf.put((byte) 1);
            _buf.putInt(s.width);
            _buf.putInt(s.height);
        }
    }

    public void putRectangle(Rectangle s) {
        if (s == null) {
            _buf.put((byte) 0);
        } else {
            _buf.put((byte) 1);
            _buf.putInt(s.x);
            _buf.putInt(s.y);
            _buf.putInt(s.x + s.width);
            _buf.putInt(s.y + s.height);
        }
    }

    public <T extends G2Object> T getObject(T s, Class<T> cls) {
        if (_buf.get() == 1) {
            if (s == null) {
                try {
                    s = cls.newInstance();
                    s.deserialize(this);
                } catch (Exception e) {}
            } else {
                s.deserialize(this);
            }
        } else {
            s = null;
        }
        return s;
    }

    public <T extends G2Object> boolean getObject(T s) {
        if (s == null)
            return false;
        if (_buf.get() == 1) {
            s.deserialize(this);
            return true;
        }
        return false;
    }

    public <T extends G2Object> T getObject(Class<T> cls) {
        T s = null;
        if (_buf.get() == 1) {
            try {
                s = cls.newInstance();
                s.deserialize(this);
            } catch (Exception e) {}
        }
        return s;
    }

    public String getString() {
        int len = _buf.getInt();
        if (len == 0) {
            return new String();
        }

        byte[] buf = new byte[len];
        _buf.get(buf);
        return new String(buf, _default_charset);
    }

    public long getInt64() {
        return _buf.getLong();
    }

    public int getInt32() {
        return _buf.getInt();
    }

    public short getInt16() {
        return _buf.getShort();
    }

    public byte getInt8() {
        return _buf.get();
    }

    public float getFloat32() {
        return _buf.getFloat();
    }

    public double getFloat64() {
        return _buf.getDouble();
    }

    public boolean getBool() {
        return _buf.get() != 0;
    }

    @SuppressWarnings("unchecked")
    public <T extends G2Object> T[] getVectorObject(T[] a, Class<T> cls) {
        int len = _buf.getInt();
        if (len > 0) {
            if (a == null || a.length != len) {
                a = (T[]) Array.newInstance(cls, len);
            }
            for (int i = 0; i < len; ++i) {
                T s = a[i];
                if (_buf.get() == 1) {
                    s.deserialize(this);
                }
            }
        }
        return a;
    }

    @SuppressWarnings("unchecked")
    public <T extends G2Object> T[] getVectorObject(Class<T> cls) {
        T[] a = null;
        int len = _buf.getInt();
        if (len > 0) {
            a = (T[]) Array.newInstance(cls, len);
            for (int i = 0; i < len; ++i) {
                T s = a[i];
                if (_buf.get() == 1) {
                    s.deserialize(this);
                }
            }
        }
        return a;
    }

    public String[] getVectorString() {
        String[] a = null;
        int len = _buf.getInt();
        if (len > 0) {
            a = new String[len];
            for (int i = 0; i < len; ++i) {
                if (_buf.getInt() > 0) {
                    byte[] buf = new byte[len];
                    _buf.get(buf);
                    a[i] = new String(buf, _default_charset);
                }
            }
        }
        return a;
    }

    public int[] get_vector_int32() {
        int[] a = null;
        int len = _buf.getInt();
        if (len > 0) {
            a = new int[len];
            for (int i = 0; i < len; ++i) {
                a[i] = _buf.getInt();
            }
        }
        return a;
    }

    public short[] getVectorInt16() {
        short[] a = null;
        int len = _buf.getInt();
        if (len > 0) {
            a = new short[len];
            for (int i = 0; i < len; ++i) {
                a[i] = _buf.getShort();
            }
        }
        return a;
    }

    public byte[] getVectorInt8() {
        byte[] a = null;
        int len = _buf.getInt();
        if (len > 0) {
            a = new byte[len];
            _buf.get(a);
        }
        return a;
    }

    public byte[] getVectorInt8(byte[] a) {
        int len = _buf.getInt();
        if (len > 0) {
            if (a == null || a.length != len) {
                a = new byte[len];
            }
            _buf.get(a);
        }
        return a;
    }

    public Dimension getDimension() {
        Dimension s = null;
        if (_buf.get() == 1) {
            s = new Dimension();
            s.width = _buf.getInt();
            s.height = _buf.getInt();
        }
        return s;
    }

    public Rectangle getRectangle() {
        Rectangle s = null;
        if (_buf.get() == 1) {
            s = new Rectangle();
            s.x = _buf.getInt();
            s.y = _buf.getInt();
            s.width = _buf.getInt() - s.x;
            s.height = _buf.getInt() - s.y;
        }
        return s;
    }

    public ByteBuffer buf() {
        return _buf;
    }

    private ByteBuffer _buf;
}
