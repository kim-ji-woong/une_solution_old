package com.idis.gdk.util;

import java.util.HashMap;
import java.util.Map;

public class G2Enum {
    public static interface Converter<T, E extends Enum<E> & Converter<T, E>> {
        T to();
        E get(T id);
    }

    public static class ReverseMap<T, V extends Enum<V> & Converter<T, V>> {
        private Map<T, V> map = new HashMap<T, V>();

        public ReverseMap(Class<V> valueType, V def) {
            this._def = def;
            for (V v : valueType.getEnumConstants()) {
                map.put(v.to(), v);
            }
        }

        public V get(T id) {
            V res = map.get(id);
            return res == null ? _def : res;
        }

        private V _def;
    }
}
