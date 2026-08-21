package com.idis.gdk.define;

import java.awt.Rectangle;
import java.awt.Dimension;
import java.util.Vector;

import com.idis.gdk.G2Object;
import com.idis.gdk.G2Stream;
import com.idis.gdk.util.G2Enum;

public final class G2CodecInfo {
    public static class ContentAnalyticsFaceDetection extends G2Object implements Cloneable {
        public static class COORDINATE_TYPE extends G2Object implements Cloneable {
            public COORDINATE_TYPE() {}
            public COORDINATE_TYPE(COORDINATE_TYPE right) {
                this._left = right._left;
                this._top = right._top;
                this._right = right._right;
                this._bottom = right._bottom;
            }

            public COORDINATE_TYPE(Rectangle rect) {
                this._left = (short) rect.x;
                this._top = (short) rect.y;
                this._right = (short) (rect.x + rect.width);
                this._bottom = (short) (rect.y + rect.height);
            }

            public COORDINATE_TYPE clone() {
                return new COORDINATE_TYPE(this);
            }

            public void serialize(G2Stream s) {
                s.putInt16(_left);
                s.putInt16(_top);
                s.putInt16(_right);
                s.putInt16(_bottom);
            }

            public void deserialize(G2Stream s) {
                _left = s.getInt16();
                _top = s.getInt16();
                _right = s.getInt16();
                _bottom = s.getInt16();
            }

            short _left;
            short _top;
            short _right;
            short _bottom;
        }

        public ContentAnalyticsFaceDetection() {}
        @SuppressWarnings("unchecked")
        public ContentAnalyticsFaceDetection(ContentAnalyticsFaceDetection right) {
            this._resolution = (Dimension) right._resolution.clone();
            this._coords = (Vector<Rectangle>) right._coords.clone();
        }

        public ContentAnalyticsFaceDetection clone() {
            return new ContentAnalyticsFaceDetection(this);
        }

        public void serialize(G2Stream s) {
            COORDINATE_TYPE[] coords = new COORDINATE_TYPE[_coords.size()];
            int i = 0;
            for (Rectangle r : _coords) {
                coords[i] = new COORDINATE_TYPE(r);
            }

            s.putDimension(_resolution);
            s.putVector(coords);
        }

        public void deserialize(G2Stream s) {
            _resolution = s.getDimension();
            _coords.clear();

            COORDINATE_TYPE[] coords = s.getVectorObject(COORDINATE_TYPE.class);
            if (coords != null) {
                for (COORDINATE_TYPE v : coords) {
                    _coords.add(new Rectangle(v._left, v._top, v._right - v._left, v._bottom - v._top));
                }
            }
        }

        public Dimension _resolution;
        public Vector<Rectangle> _coords = new Vector<Rectangle>(2);
    }

    public static class ElevatorStatus extends G2Object implements Cloneable {
        public enum DOOR_STATUS implements G2Enum.Converter<Byte, DOOR_STATUS> {
            UNKNOWN((byte)0), CLOSING((byte)1), CLOSE((byte)2), OPENING((byte)3), OPEN((byte)4);

            static G2Enum.ReverseMap<Byte, DOOR_STATUS> map = new G2Enum.ReverseMap<Byte, DOOR_STATUS>(DOOR_STATUS.class, DOOR_STATUS.UNKNOWN);
            static public DOOR_STATUS from(Byte id) { return map.get(id); }
            private final byte value;
            private DOOR_STATUS(byte value) { this.value = value; }
            public Byte to() { return this.value; }
            public DOOR_STATUS get(Byte id) { return map.get(id); }
        }

        public enum DIRECTION implements G2Enum.Converter<Byte, DIRECTION> {
            UNKNOWN((byte)0), DOWN((byte)1), STOP((byte)2), UP((byte)3);

            static G2Enum.ReverseMap<Byte, DIRECTION> map = new G2Enum.ReverseMap<Byte, DIRECTION>(DIRECTION.class, DIRECTION.UNKNOWN);
            static public DIRECTION from(byte id) { return map.get(id); }
            private final byte value;
            private DIRECTION(byte value) { this.value = value; }
            public Byte to() { return this.value; }
            public DIRECTION get(Byte id) { return map.get(id); }
        }
        
        public enum MODE implements G2Enum.Converter<Byte, MODE> {
            UNKNOWN((byte)0), MANUAL((byte)1), AUTO((byte)2);

            static G2Enum.ReverseMap<Byte, MODE> map = new G2Enum.ReverseMap<Byte, MODE>(MODE.class, MODE.UNKNOWN);
            static public MODE from(byte id) { return map.get(id); }
            private final byte value;
            private MODE(byte value) { this.value = value; }
            public Byte to() { return this.value; }
            public MODE get(Byte id) { return map.get(id); }
        }

        public ElevatorStatus() {}
        public ElevatorStatus(ElevatorStatus right) {
            this._id = right._id;
            this._floor = right._floor;
            this._door_status = right._door_status;
            this._direction = right._direction;
            this._mode = right._mode;
            this._additional = right._additional;
        }

        public ElevatorStatus clone() {
            return new ElevatorStatus(this);
        }

        public void serialize(G2Stream s) {
            s.putInt16(_id);
            s.putFloat32(_floor);
            s.putInt8(_door_status.to());
            s.putInt8(_direction.to());
            s.putInt8(_mode.to());
            s.putString(_additional);
        }

        public void deserialize(G2Stream s) {
            _id = s.getInt16();
            _floor = s.getFloat32();
            _door_status = DOOR_STATUS.from(s.getInt8());
            _direction = DIRECTION.from(s.getInt8());
            _mode = MODE.from(s.getInt8());
            _additional = s.getString();
        }

        public short _id = 0;
        public float _floor = 0.0f;
        public DOOR_STATUS _door_status = DOOR_STATUS.UNKNOWN;
        public DIRECTION _direction = DIRECTION.UNKNOWN;
        public MODE _mode = MODE.UNKNOWN;
        public String _additional = "";
    }
}
