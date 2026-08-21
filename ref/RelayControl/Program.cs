using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelayControl
{
    class Program
    {

        static void Main(string[] args)
        {
            if (ParseArguments(args))
            {
                SerialManager sm = new SerialManager();
                sm.CheckRelay();
                sm.RunRelay(nCommand);
            }
        }
        private static int nCommand = 2;
        static bool ParseArguments(string[] args)
        {
            if (args.Length < 1)
                return false;

            try
            {
                string szCommand = args[0];
                nCommand = int.Parse(szCommand);

            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
