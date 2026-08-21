using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIMViewer.Data
{
    public class POIData
    {
        List<string> m_flowPOICodes = new List<string>();

        public static bool IsFloorPOI(string strPOICode)
        {
            if (strPOICode == "F1111" ||
                strPOICode == "F1112" ||
                strPOICode == "F1113" ||
                strPOICode == "F1114" ||
                strPOICode == "F1115" ||
                strPOICode == "F1116" ||
                strPOICode == "F1117" ||
                strPOICode == "F1118" ||
                strPOICode == "F1120" ||
                strPOICode == "F1130" ||
                strPOICode == "F1313" ||
                strPOICode == "F1314" ||
                strPOICode == "F1315" ||
                strPOICode == "F1316" ||
                strPOICode == "F1317" ||
                strPOICode == "F1318" ||
                strPOICode == "F1319" ||
                strPOICode == "F131A" ||
                strPOICode == "F131B" ||
                strPOICode == "F131C" ||
                strPOICode == "F131J" ||
                strPOICode == "F1321" ||
                strPOICode == "F1322" ||
                strPOICode == "F1323" ||
                strPOICode == "F1324" ||
                strPOICode == "F1463" ||
                strPOICode == "F3110" ||
                strPOICode == "F3120" ||
                strPOICode == "F3130" ||
                strPOICode == "F3140" ||
                strPOICode == "F3150" ||
                strPOICode == "F3160" ||
                strPOICode == "F3170" ||
                strPOICode == "F3180" ||
                strPOICode == "F3190" ||
                strPOICode == "F31A0" ||
                strPOICode == "F31B0" ||
                strPOICode == "F31C0" ||
                strPOICode == "F3210" ||
                strPOICode == "F3220" ||
                strPOICode == "F3230" ||
                strPOICode == "F3240" ||
                strPOICode == "F3250" ||
                strPOICode == "F3321" ||
                strPOICode == "F3322" ||
                strPOICode == "F3323" ||
                strPOICode == "F3330" ||
                strPOICode == "F3341" ||
                strPOICode == "F3342" ||
                strPOICode == "F3343" ||
                strPOICode == "F3344" ||
                strPOICode == "F51C0" ||
                strPOICode == "F51D0" ||
                strPOICode == "F5410" ||
                strPOICode == "F5420" ||
                strPOICode == "F5430" ||
                strPOICode == "F6510" ||
                strPOICode == "F6520" ||
                strPOICode == "F6610" ||
                strPOICode == "F6700")
                return true;

                return false;
        }
    }
}
