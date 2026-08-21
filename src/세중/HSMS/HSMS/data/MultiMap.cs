using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace HSMS
{
    public class MultiMap<U, V>
    {
        Dictionary<U, List<V>> _dictionary =
        new Dictionary<U, List<V>>();

        public void Add(U key, V value)
        {
            List<V> list;
            if (this._dictionary.TryGetValue(key, out list))
            {
                list.Add(value);
            }
            else
            {
                list = new List<V>();
                list.Add(value);
                this._dictionary[key] = list;
            }
        }

        public IEnumerable<U> Keys
        {
            get
            {
                return this._dictionary.Keys;
            }
        }

        public List<V> this[U key]
        {
            get
            {
                List<V> list;
                if (!this._dictionary.TryGetValue(key, out list))
                {
                    list = new List<V>();
                    this._dictionary[key] = list;
                }
                return list;
            }
        }

        public MultiMap<U, V> Clone()
        {
            MultiMap<U, V> _map = new MultiMap<U,V>();

            IEnumerable<U> keyList = Keys;
           
            foreach (U key in keyList)
            {
                List<V> list = this[key];

                foreach(V v in list)
                {
                    _map.Add(key, v);
                }                
            }
            return _map;
        }

        public bool Compare(MultiMap<U, V> other)
        {
            if (other == this)
                return true;

            IEnumerable<U> keyList = Keys;
            IEnumerable<U> keyList2 = other.Keys;

            
            if (keyList.Count() != keyList2.Count())
                return false;

            for (int i = 0; i < keyList.Count(); i++)
            {
                if (keyList.ElementAt(i).ToString() != keyList2.ElementAt(i).ToString())
                    return false;
            }

            foreach (U key in Keys)
            {
                List<V> list = this[key];
                List<V> _list = other[key];

                if (list.Count != _list.Count)
                    return false;

                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].ToString() != _list[i].ToString())
                        return false;
                }               
            }
        
            return false;
        }       
    }
}
