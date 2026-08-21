using System;
using System.Collections.Generic;

namespace GDK
{
#if _WIN64
    using G2WPARAM = System.UInt64;
    using G2LPARAM = System.Int64;
    using G2UPARAM = System.IntPtr;
    using G2RESULT = System.Int64;
#else
    using G2WPARAM = System.UInt32;
    using G2LPARAM = System.Int32;
    using G2UPARAM = System.IntPtr;
    using G2RESULT = System.Int32;
#endif

    public struct g2pair<T1, T2> : IEquatable<g2pair<T1, T2>>
    {
        public g2pair(T1 first, T2 second)
        {
            this.first = first;
            this.second = second;
        }

        public bool Equals(g2pair<T1, T2> other)
        {
            return ((first == null && other.first == null) || (first != null && first.Equals(other.first))) &&
                    ((second == null && other.second == null) || (second != null && second.Equals(other.second)));
        }
        public override bool Equals(object obj)
        {
            return (Equals((g2pair<T1, T2>)obj));
        }
        public override int GetHashCode()
        {
            uint seed = 0;
            if (first != null)
            {
                seed ^= (uint)first.GetHashCode() + 0x9e3779b9 + (seed << 6) + (seed >> 2);
            }
            if (second != null)
            {
                seed ^= (uint)second.GetHashCode() + 0x9e3779b9 + (seed << 6) + (seed >> 2);
            }
            return (int)seed;
        }

        public static bool operator ==(g2pair<T1, T2> left, g2pair<T1, T2> right) { return left.Equals(right); }
        public static bool operator !=(g2pair<T1, T2> left, g2pair<T1, T2> right) { return !(left == right); }

        public T1 first;
        public T2 second;
    }

    public struct G2PARAM_
    {
        public static ushort LOWORD(G2WPARAM param) { return (ushort)(param & 0xFFFF); }
        public static ushort HIWORD(G2WPARAM param) { return (ushort)((param >> 16) & 0xFFFF); }
    }
}
