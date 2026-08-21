using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamEditor.Import
{
    public static class RegularReaderManager
    {
        public static IRegularReader MakeExcelReader(string[] tokens)
        {
            IRegularReader reader = FindColumnHeader(new Common.RegularExcelReader(), tokens);

            if (reader != null)
                return reader;

            reader = FindColumnHeader(new Site._201.Parc1RegularExcelReader(), tokens);

            if (reader != null)
                return reader;

            return null;
        }

        private static IRegularReader FindColumnHeader(IRegularReader reader, string[] tokens)
        {
            if (reader.FindColumnHeader(tokens))
                return reader;

            return null;
        }
    }
}
