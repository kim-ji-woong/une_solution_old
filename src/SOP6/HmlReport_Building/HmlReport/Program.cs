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

            int reportType = 0; // 보고서 종류
            if (!int.TryParse(args[0], out reportType))
                return;

            int facilityType = -1; // 재난 종류
            if (!int.TryParse(args[1], out facilityType))
                return;

            string filePath = args[2].ToString(); // file path
            string logoFileName = args[3].ToString(); // Logo file name

            int siteID = 0; // Site Id
            if (!int.TryParse(args[4], out siteID))
                return;

            CreateHml hml = new CreateHml(reportType, facilityType, filePath, logoFileName, siteID);
        }
    }
}
