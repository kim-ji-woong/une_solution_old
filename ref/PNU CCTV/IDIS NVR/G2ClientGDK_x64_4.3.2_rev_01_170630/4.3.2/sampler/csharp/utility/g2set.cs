using System;
using System.Collections.Generic;

namespace GDK
{
    public class g2set<T> : List<T> where T : IComparable<T>, IEquatable<T>
    {
        public g2set() { }
        public g2set(g2set<T> right)
        {
            this.AddRange(right);
        }
        public g2set(T val)
        {
            this.Add(val);
        }
        public g2set(T[] vals, int len)
        {
            if (len > 0)
            {
                foreach (T v in vals)
                {
                    insert(v);
                    if (--len == 0)
                    {
                        break;
                    }
                }
            }
        }

        public void insert(T val)
        {
            int i = lower_bound(val);
            if (i == this.Count ||
               (val.Equals(this[i]) != true))
            {
                this.Insert(i, val);
            }
        }
        public void insert(params T[] vals)
        {
            foreach (T v in vals)
            {
                insert(v);
            }
        }
        public void insert(g2set<T> right)
        {
            foreach (T v in right)
            {
                insert(v);
            }
        }
        public void assign(g2set<T> right)
        {
            this.Clear();
            this.AddRange(right);
        }
        public bool erase(T val)
        {
            int i = lower_bound(val);
            if (i != this.Count && val.Equals(this[i]))
            {
                this.RemoveAt(i);
                return true;
            }
            return false;
        }
        public void clear()
        {
            this.Clear();
        }
        public void erase(g2set<T> right)
        {
            foreach (T v in right)
            {
                erase(v);
            }
        }
        public bool empty()
        {
            return (this.Count == 0);
        }
        public bool contains(T val)
        {
            int i = lower_bound(val);
            return (i != this.Count) && val.Equals(this[i]);
        }
        public bool equal(g2set<T> right)
        {
            if (this.Count != right.Count) return false;

            int first1 = 0;
            int first2 = 0;
            int last1 = this.Count;

            while (first1 != last1)
            {
                if (this[first1].Equals(right[first2]) != true)
                {
                    return false;
                }
                ++first1;
                ++first2;
            }
            return true;
        }
        public int size()
        {
            return this.Count;
        }
        public int lower_bound(T val)
        {
            int first = 0;
            int i = 0;
            int step = 0;
            int count = this.Count;

            while (count > 0)
            {
                i = first; step = count / 2; i += step;
                if (this[i].CompareTo(val) < 0)
                {
                    first = ++i;
                    count -= step + 1;
                }
                else
                {
                    count = step;
                }
            }
            return first;
        }

        public static g2set<B> clone<B>(g2set<B> right) where B : ICloneable, IComparable<B>, IEquatable<B>
        {
            g2set<B> ret = new g2set<B>();
            for (int i = 0; i < right.Count; ++i)
            {
                ret.Add((B)right[i].Clone());
            }
            return ret;
        }
        public g2set<T> clone()
        {
            return new g2set<T>(this);
        }
        public T[] to_array()
        {
            return this.ToArray();
        }

        public static g2set<T> union(g2set<T> left, g2set<T> right)
        {
            g2set<T> ret = new g2set<T>();
            int first1 = 0, last1 = left.Count;
            int first2 = 0, last2 = right.Count;

            while (true)
            {
                if (first1 == last1)
                {
                    for (int i = first2; i < last2; ++i)
                    {
                        ret.insert(right[i]);
                    }
                    break;
                }
                if (first2 == last2)
                {
                    for (int i = first1; i < last1; ++i)
                    {
                        ret.insert(left[i]);
                    }
                    break;
                }

                int cmp = left[first1].CompareTo(right[first2]);
                if (cmp < 0)
                {
                    ret.insert(left[first1]);
                    ++first1;
                }
                else if (cmp > 0)
                {
                    ret.insert(right[first2]);
                    ++first2;
                }
                else
                {
                    ret.insert(left[first1]);
                    ++first1; ++first2;
                }
            }
            return ret;
        }
        public static g2set<T> difference(g2set<T> left, g2set<T> right)
        {
            g2set<T> ret = new g2set<T>();
            int first1 = 0, last1 = left.Count;
            int first2 = 0, last2 = right.Count;

            while (first1 != last1 &&
                   first2 != last2)
            {
                int cmp = left[first1].CompareTo(right[first2]);
                if (cmp < 0)
                {
                    ret.insert(left[first1]);
                    ++first1;
                }
                else if (cmp > 0)
                {
                    ++first2;
                }
                else
                {
                    ++first1; ++first2;
                }
            }

            for (int i = first1; i < last1; ++i)
            {
                ret.insert(left[i]);
            }
            return ret;
        }
        public static g2set<T> intersection(g2set<T> left, g2set<T> right)
        {
            g2set<T> ret = new g2set<T>();
            int first1 = 0, last1 = left.Count;
            int first2 = 0, last2 = right.Count;

            while (first1 != last1 &&
                   first2 != last2)
            {
                int cmp = left[first1].CompareTo(right[first2]);
                if (cmp < 0)
                {
                    ++first1;
                }
                else if (cmp > 0)
                {
                    ++first2;
                }
                else
                {
                    ret.insert(left[first1]);
                    ++first1; ++first2;
                }
            }
            return ret;
        }

        public static g2set<T> operator +(g2set<T> left, g2set<T> right)
        {
            return g2set<T>.union(left, right);
        }
        public static g2set<T> operator -(g2set<T> left, g2set<T> right)
        {
            return g2set<T>.difference(left, right);
        }
        public static g2set<T> operator &(g2set<T> left, g2set<T> right)
        {
            return g2set<T>.intersection(left, right);
        }
    }
}
