using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HmlReport
{
    class Program
    {
        static void Main(string[] args)
        { 
            if (args.Length != 5)
                return;

            int disasterTypeID = 0; // 보고서 종류
            if (!int.TryParse(args[0], out disasterTypeID))
                return;

            string disasterType = args[1].ToString(); // 재난 종류
            if (disasterType.Length == 0)
                return;

            string filePath = args[2].ToString(); // file path
            string logoFileName = args[3].ToString(); // Logo file name

            int siteID = 0; // Site Id
            if (!int.TryParse(args[4], out siteID))
                return;

            CreateHml hml = new CreateHml(disasterTypeID, disasterType, filePath, logoFileName, siteID);
        }
    }
}
