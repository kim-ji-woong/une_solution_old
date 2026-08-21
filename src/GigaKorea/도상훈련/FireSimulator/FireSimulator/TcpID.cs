using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireSimulator
{
    public class TCP_ID
    {
        public const short ARE_YOU_THERE = 1;
        public const short I_AM_HERE = 2;
        public const short REPORT_FIRE = 3;
        public const short CLEAR_FIRE = 4;
        //public const short REPORT_EARTHQUAKE4 = 5;
        //public const short CLEAR_EARTHQUAKE4 = 6;
        public const short REPORT_OUTBREAK = 5;
        public const short CLEAR_OUTBREAK = 6;
        public const short REPORT_EARTHQUAKE5 = 7;
        public const short CLEAR_EARTHQUAKE5 = 8;
        public const short REPORT_SEQURITY = 9;
        public const short CLEAR_SEQURITY = 10;
        public const short REPORT_FINEDUST1 = 11;
        public const short CLEAR_FINEDUST1 = 12;
        public const short REPORT_FINEDUST2 = 13;
        public const short CLEAR_FINEDUST2 = 14;

        public const short POPUP_OUTBREAK = 101;
        public const short SIMULATOR_OPEN = 102;
    }
}
