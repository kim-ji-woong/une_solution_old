using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ExcelWorker
{
    using Reader;
    using Writer;

    public enum DataMode { None = 0, FacilityInfo = 1, BuildingData, BuildingGroupData, RegularMember };

    static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        static void Main(string[] args)
        {
            bool readFile;
            string strFilePath;
            DataMode mode;

            if (ReadFileOptions(args, out readFile, out mode, out strFilePath))
            {
                if (readFile)
                {
                    ExcelReader reader = ExcelReader.MakeInstance(mode, strFilePath);

                    if (reader != null)
                        reader.Run();
                }
                else
                {
                    ExcelWriter writer = ExcelWriter.MakeInstance(mode, strFilePath);

                    if (writer != null)
                        writer.Run();
                }
            }
            /*ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1()
            };
            ServiceBase.Run(ServicesToRun);*/
        }

        // [0] : Read Option => 1이면 read, 0이면 write
        // [1] : Data Mode
        // [2] : File Path
        private static bool ReadFileOptions(string[] args, out bool readFile, out DataMode mode, out string strFilePath)
        {
            readFile = true;
            strFilePath = null;
            mode = DataMode.None;

            if (args == null || args.Count() < 3)
                return false;

            string strReadOpt = args[0].Trim().ToLower();

            if (strReadOpt == "1" || strReadOpt == "true")
                readFile = true;
            else if (strReadOpt == "0" || strReadOpt == "false")
                readFile = false;
            else
                return false;

            string strMode = args[1].Trim();
            
            if (ToDataMode(strMode, out mode) == false)
                return false;

            strFilePath = args[2].Trim();
            return strFilePath.Length > 0;
        }

        private static bool ToDataMode(string strMode, out DataMode mode)
        {
            mode = DataMode.None;

            int nMode;

            if (int.TryParse(strMode, out nMode) == false)
                return false;

            foreach (DataMode _mode in Enum.GetValues(typeof(DataMode)))
            {
                if (nMode == (int)_mode)
                {
                    mode = _mode;
                    return true;
                }
            }

            return false;
        }
    }
}
