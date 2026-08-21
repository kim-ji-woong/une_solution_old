using System;
using System.Collections.Generic;
using System.Text;
using GDK;

namespace GDK_tester
{
    public class perf_counter
    {
        public perf_counter()
        {
            begin = end = 0;
        }

        public void do_begin()
        {
            begin = Environment.TickCount;
        }
        public void do_end()
        {
            end = Environment.TickCount;
        }
        public int result() { return (end - begin); }
        public bool empty() { return (begin == 0 && end == 0); }
        public void reset() { begin = end = 0; }
        public void reset_begin() { begin = 0; }
        public void reset_end() { end = 0; }
        public void equal_begin() { end = begin; }
        public void equal_end() { begin = end; }

        public int begin;
        public int end;
    }
}
