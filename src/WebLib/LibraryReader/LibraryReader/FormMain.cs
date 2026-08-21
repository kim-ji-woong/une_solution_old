using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace LibraryReader
{
    public partial class FormMain : Form
    {
        // 세션 유지용 쿠키
        private CookieContainer cookieContainer = new CookieContainer();
        private DBManagerMySQL m_dbMgr = new DBManagerMySQL();

        private int m_nTotalCount = 0;
        private int m_nProgressCount = 0;
        private string m_strPrevIndex = "";

        private static FormMain m_instance = null;

        public static FormMain Instance
        {
            get { return m_instance; }
        }

        public FormMain()
        {
            m_instance = this;
            InitializeComponent();

            m_strPrevIndex = textBoxFromIndex.Text;
        }

        private void ReadLibrary(string strPath, Dictionary<string, List<Library>> dicNameLibraries)
        {
            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            StreamReader reader = new StreamReader(strPath, encEUC_KR);

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                string[] arrTokens = strLine.Split('\t');

                if (arrTokens.Count() != 4)
                    break;

                int nID;

                if (!int.TryParse(arrTokens[0].Trim(), out nID))
                    break;

                string strName = arrTokens[1].Trim();
                string strAddr1 = arrTokens[2].Trim();
                string strAddr2 = arrTokens[3].Trim();

                Library lib = new Library();

                lib.ID = nID;
                lib.Name = strName;
                lib.Addr1 = strAddr1;
                lib.Addr2 = strAddr2;

                List<Library> libraries = null;

                if (!dicNameLibraries.TryGetValue(strName, out libraries))
                {
                    libraries = new List<Library>();
                    dicNameLibraries[strName] = libraries;
                }

                libraries.Add(lib);
            }

            reader.Close();
        }

        private Library FindLibrary(Library lib, Dictionary<string, List<Library>> dicNameLibraries, StreamWriter writerUnknown)
        {
            List<Library> libraries = null;
            
            if (!dicNameLibraries.TryGetValue(lib.Name, out libraries) || libraries.Count == 0)
            {
                writerUnknown.WriteLine(lib.Name);
                return null;
            }

            if (libraries.Count == 1)
                return libraries[0];

            List<Library> sameLibraries = new List<Library>();

            foreach (Library library in libraries)
            {
                if (library.Addr1.StartsWith(lib.Addr1) && library.Addr2 == lib.Addr2)
                    sameLibraries.Add(library);
            }

            if (sameLibraries.Count == 1)
                return sameLibraries[0];

            FormSelectLibrary frm = null;

            if (sameLibraries.Count > 0)
                frm = new FormSelectLibrary(0, lib.Name + " " + lib.Addr1 + " " + lib.Addr2, sameLibraries);
            else
                frm = new FormSelectLibrary(0, lib.Name + " " + lib.Addr1 + " " + lib.Addr2, libraries);

            if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return frm.SelectedLibrary;
            }

            return null;
        }

        private void CompareLibrary(Dictionary<string, List<Library>> dicCityLibraries, Dictionary<string, List<Library>> dicNameLibraries, string strOutFilePath)
        {
            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            StreamWriter writer = new StreamWriter(strOutFilePath, false, encEUC_KR);
            StreamWriter writerUnknown = new StreamWriter("F:/unknown.txt", false, encEUC_KR);

            int nCount = 0;

            foreach (KeyValuePair<string, List<Library>> pair in dicCityLibraries)
            {
                foreach (Library library in pair.Value)
                {
                    Library lib = FindLibrary(library, dicNameLibraries, writerUnknown);
                    nCount++;

                    if (lib == null)
                        continue;

                    writer.WriteLine(lib.ID.ToString() + "\t" + library.Name + "\t" + library.GubunType.ToString() + "\t" + library.OwnType.ToString() + "\t" + library.Year.ToString() + "\t" + library.UserCount + "\t" + library.Area.ToString());
                }
            }

            writer.Close();
            writerUnknown.Close();
        }

        private Dictionary<int, string> CityFromID()
        {
            Dictionary<int, string> dicCities = new Dictionary<int,string>();

            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            StreamReader reader = new StreamReader("F:/aaa.txt", encEUC_KR);

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                string[] arrTokens = strLine.Split('\t');

                int nID;

                if (!int.TryParse(arrTokens[0].Trim(), out nID))
                    break;

                dicCities[nID] = arrTokens[1].Trim();
            }

            reader.Close();
            return dicCities;
        }

        private void Test2()
        {
            Dictionary<int, string> dicCities = CityFromID();

            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            StreamReader reader = new StreamReader("F:/aaa.txt", encEUC_KR);
            StreamWriter writer = new StreamWriter("F:/ccc.txt", false, encEUC_KR);

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                string[] arrTokens = strLine.Split('\t');
                int nTokenCount = arrTokens.Count();

                if (nTokenCount != 7)
                    System.Diagnostics.Trace.WriteLine(strLine);

                int nID;

                if (!int.TryParse(arrTokens[0].Trim(), out nID))
                    break;

                string strName = arrTokens[1].Trim();
                string strGubun = arrTokens[2].Trim();
                string strOwner = arrTokens[3].Trim();

                int nYear, nUserCount = 0, nArea;

                if (!int.TryParse(arrTokens[4].Trim(), out nYear))
                    break;

                int.TryParse(arrTokens[5].Trim(), out nUserCount);

                if (!int.TryParse(arrTokens[6].Trim(), out nArea))
                    break;

                string strCity = dicCities[nID];
                int nGrade = GetGrade(nArea, strGubun, strCity);

                string strSQL = string.Format("Update libdb.lib_list2 set name = '{0}', gubun = '{1}', useing = '{2}', year = '{3}', user_count = '{4}', area_count = '{5}', grade = {6} where idx = {7};",
                    strName, strGubun, strOwner, nYear, nUserCount == 0 ? "" : nUserCount.ToString(), nArea, nGrade, nID);

                writer.WriteLine(strSQL);
            }

            reader.Close();
            writer.Close();
        }

        private int GetGrade(int nArea, string strGubunType, string strCity)
        {
            if (strGubunType == "작은")
                return 0;

            if (strCity.StartsWith("서울"))
            {
                if (strGubunType == "중앙")
                    return 1;
                else if (strGubunType == "거점")
                    return 2;
                else if (strGubunType == "분관")
                {
                    if (nArea >= 900)
                        return 3;
                    else if (nArea >= 600)
                        return 4;
                    else
                        return 5;
                }
                else if (strGubunType == "대표")
                    return 0;
            }
            else if (strCity.StartsWith("광주") || strCity.StartsWith("대구") || strCity.StartsWith("대전")
                || strCity.StartsWith("부산") || strCity.StartsWith("세종") || strCity.StartsWith("울산")
                || strCity.StartsWith("인천"))
            {
                if (strGubunType == "중앙")
                    return 6;
                else if (strGubunType == "거점")
                    return 7;
                else if (strGubunType == "분관")
                {
                    if (nArea >= 900)
                        return 8;
                    else if (nArea >= 600)
                        return 9;
                    else
                        return 10;
                }
                else if (strGubunType == "대표")
                    return 0;
            }
            else
            {
                if (strGubunType == "중앙")
                    return 11;
                else if (strGubunType == "거점")
                    return 12;
                else if (strGubunType == "분관")
                {
                    if (nArea >= 900)
                        return 13;
                    else if (nArea >= 600)
                        return 14;
                    else
                        return 15;
                }
                else if (strGubunType == "대표")
                    return 0;
            }

            return -1;
        }

        private void WriteOriginData(Dictionary<string, List<Library>> dicCityLibraries)
        {
            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            StreamWriter writer = new StreamWriter("F:/OriginLib.txt", false, encEUC_KR);

            foreach (KeyValuePair<string, List<Library>> pair in dicCityLibraries)
            {
                foreach (Library lib in pair.Value)
                {
                    string strLocation = lib.Location.Length == 0 ? "@@" : lib.Location;
                    string strUserCount = lib.UserCount.Length == 0 ? "@@" : lib.UserCount;
                    string strPhoneNumber = lib.PhoneNumber.Length == 0 ? "@@" : lib.PhoneNumber;

                    writer.WriteLine(lib.GubunType.ToString() + "\t" + strLocation + "\t" + lib.OwnType.ToString() + "\t" + lib.Year.ToString() + "\t" + lib.Name + "\t" + strPhoneNumber + "\t" + strUserCount + "\t" + lib.Area.ToString());
                }
            }

            writer.Close();
        }

        private Dictionary<string, List<Library>> ReadOriginData(string strPath)
        {
            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            StreamReader reader = new StreamReader(strPath, encEUC_KR);

            Dictionary<string, List<Library>> dicCityLibraries = new Dictionary<string, List<Library>>();
            List<Library> libraries = new List<Library>();
            dicCityLibraries["전국"] = libraries;

            StreamWriter writer = new StreamWriter("F:/미정.txt", false, encEUC_KR);

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                string[] arrTokens = strLine.Split('\t');

                if (arrTokens.Count() != 8)
                    System.Diagnostics.Trace.WriteLine(strLine);
                else
                {
                    string strGubun = arrTokens[0].Trim();
                    string strLocation = arrTokens[1].Trim();
                    string strOwner = arrTokens[2].Trim();
                    string strYear = arrTokens[3].Trim();
                    string strName = arrTokens[4].Trim();
                    string strPhoneNumber = arrTokens[5].Trim();
                    string strUserCount = arrTokens[6].Trim();
                    string strArea = arrTokens[7].Trim();

                    Library lib = new Library();

                    if (strGubun.StartsWith("중앙"))
                        lib.GubunType = Library.LibraryType.중앙;
                    else if (strGubun.StartsWith("대표"))
                        lib.GubunType = Library.LibraryType.대표;
                    else if (strGubun.StartsWith("거점"))
                        lib.GubunType = Library.LibraryType.거점;
                    else if (strGubun.StartsWith("분관"))
                        lib.GubunType = Library.LibraryType.분관;
                    else if (strGubun.StartsWith("작은"))
                        lib.GubunType = Library.LibraryType.작은;
                    else
                        continue;

                    lib.Location = strLocation;

                    if (strOwner.StartsWith("지자체"))
                        lib.OwnType = Library.OwnerType.지자체;
                    else if (strOwner.StartsWith("교육청"))
                        lib.OwnType = Library.OwnerType.교육청;
                    else if (strOwner.Length == 0)
                        lib.OwnType = Library.OwnerType.지자체;
                    else
                        continue;

                    if (strYear.Length == 0 || strYear == "@@")
                        lib.Year = 9999;
                    else
                    {
                        int nYear;
                        if (!int.TryParse(strYear, out nYear))
                            continue;

                        lib.Year = nYear;
                    }

                    if (strName.Length == 0 || strName == "미정" || strName == "@@")
                    {
                        writer.WriteLine(strLine);
                        continue;
                    }
                    else
                        lib.Name = strName;

                    if (strPhoneNumber.Length > 0 && strPhoneNumber != "@@")
                        lib.PhoneNumber = strPhoneNumber;

                    if (strUserCount.Length > 0 && strUserCount != "@@")
                        lib.UserCount = strUserCount;

                    if (strArea.Length > 0 && strArea != "@@")
                    {
                        int nArea;

                        if (!int.TryParse(strArea, out nArea))
                            continue;
                        else
                            lib.Area = nArea;
                    }

                    libraries.Add(lib);
                }
            }

            writer.Close();
            return dicCityLibraries;
        }

        private void Test3()
        {
            StreamReader reader = new StreamReader("F:/aa.txt");

            List<string> indeces = new List<string>();
            int nLineCount = 0;

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine().Trim();
                nLineCount++;

                if (strLine.Length == 0)
                    continue;

                if (indeces.Contains(strLine))
                    continue;
                else
                    indeces.Add(strLine);
            }

            reader.Close();
        }

        private void Test4()
        {
            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            StreamReader reader = new StreamReader("F:/1.txt", encEUC_KR);
            StreamWriter writer = new StreamWriter("F:/2.txt", false, encEUC_KR);

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                string strCoord = GetAddressCoord(strLine);
                writer.WriteLine(strCoord);
            }

            reader.Close();
            writer.Close();
        }

        private void RemoveZero(ref string strNumber)
        {
            int len = strNumber.Length;

            for (int i=len-1;i>=0;i--)
            {
                char ch = strNumber.ElementAt(i);

                if (ch != '0')
                {
                    strNumber = strNumber.Substring(0, i + 1);
                    return;
                }
            }
        }

        private void Test5()
        {
            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            StreamReader reader = new StreamReader("F:/1.txt", encEUC_KR);
            StreamWriter writer = new StreamWriter("F:/2.txt", false, encEUC_KR);

            Dictionary<string, List<string>> coordIDs = new Dictionary<string, List<string>>();

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                string[] arrTokens = strLine.Split('\t');
                string[] arrCoord = arrTokens[1].Split(',');

                string strX = arrCoord[0].Trim();
                string strY = arrCoord[1].Trim();

                RemoveZero(ref strX);
                RemoveZero(ref strY);

                string strCoord = strX + "," + strY;

                List<string> IDs = null;

                if (!coordIDs.TryGetValue(strCoord, out IDs))
                {
                    IDs = new List<string>();
                    coordIDs[strCoord] = IDs;
                }

                IDs.Add(arrTokens[0].Trim());
            }

            foreach (KeyValuePair<string, List<string>> pair in coordIDs)
            {
                if (pair.Value.Count > 1)
                {
                    bool isFirst = true;

                    foreach (string strID in pair.Value)
                    {
                        if (isFirst)
                        {
                            writer.Write(strID);
                            isFirst = false;
                        }
                        else
                            writer.Write(", " + strID);
                    }

                    writer.WriteLine();
                }
            }

            reader.Close();
            writer.Close();
        }

        private void Test6()
        {
            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            StreamReader reader = new StreamReader("F:/1.txt", encEUC_KR);

            Dictionary<string, Library> libraries = new Dictionary<string, Library>();

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                string[] arrTokens = strLine.Split('\t');
                int nTokenCount = arrTokens.Count();

                string strID = arrTokens[0].Trim();
                string strName = arrTokens[1].Trim();

                Library lib = new Library();
                lib.Name = strName;

                if (nTokenCount >= 3)
                    lib.Addr1 = arrTokens[2].Trim();

                if (nTokenCount >= 4)
                    lib.Addr2 = arrTokens[3].Trim();

                if (nTokenCount >= 5)
                    lib.Addr3 = arrTokens[4].Trim();

                if (nTokenCount >= 6)
                    lib.Addr4 = arrTokens[5].Trim();

                libraries[strID] = lib;
            }

            reader.Close();

            reader = new StreamReader("F:/2.txt", encEUC_KR);
            StreamWriter writer = new StreamWriter("F:/3.txt", false, encEUC_KR);

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                string[] arrIDs = strLine.Split(',');

                foreach (string strID in arrIDs)
                {
                    string _strID = strID.Trim();

                    if (!libraries.ContainsKey(_strID))
                        continue;

                    Library lib = libraries[_strID];
                    writer.WriteLine(_strID + "\t" + lib.Name + "\t" + lib.Addr1 + "\t" + lib.Addr2 + "\t" + lib.Addr3 + "\t" + lib.Addr4);
                }

                writer.WriteLine();
            }

            reader.Close();
            writer.Close();
        }

        private void Test7()
        {
            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            StreamReader reader = new StreamReader("F:/3.txt", encEUC_KR);
            StreamWriter writer = new StreamWriter("F:/4.txt", false, encEUC_KR);

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                {
                    writer.WriteLine();
                    continue;
                }

                string[] arrTokens = strLine.Split('\t');
                int nTokenCount = arrTokens.Count();

                if (nTokenCount > 6)
                    nTokenCount = 6;

                string strAddr = "";

                for (int i=2;i<nTokenCount;i++)
                {
                    if (strAddr.Length == 0)
                        strAddr = arrTokens[i].Trim();
                    else
                        strAddr = strAddr + " " + arrTokens[i].Trim();
                }

                string strCoord = GetAddressCoord(strAddr);
                writer.WriteLine(strCoord);
            }

            reader.Close();
            writer.Close();
        }

        private class LibInfo
        {
            public int ID = -1;
            public int Area = 0;
            public int BookCount = 0;
            public string Addr1 = "";
            public string Addr2 = "";
            public string Addr3 = "";
            public string Addr4 = "";
            public int AreaCode = -1;
            public string Gubun = "";

            // 새로 구한 값과 이전값이 다를 경우 비교용으로 사용
            public int Area2 = 0;
            public int BookCount2 = 0;
        }

        private string RemoveSticky(string strData)
        {
            if (strData.StartsWith("\""))
            {
                if (strData.Length == 1)
                    return "";
                else
                    strData = strData.Substring(1);
            }

            if (strData.EndsWith("\""))
            {
                if (strData.Length == 1)
                    return "";
                else
                    strData = strData.Substring(0, strData.Length - 1);
            }

            return strData.Trim();
        }

        private List<string> ParseString(string strLine, char delimeter)
        {
            if (strLine.StartsWith("\""))
                strLine = strLine.Substring(1);

            if (strLine.EndsWith("\""))
                strLine = strLine.Substring(0, strLine.Length - 1);

            strLine = strLine.Trim();

            List<string> tokens = new List<string>();
            int nBeginIndex = 0;

            while (true)
            {
                int nIndex = strLine.IndexOf(delimeter, nBeginIndex);

                if (nIndex < 0)
                {
                    if (strLine.Length > nBeginIndex)
                    {
                        string strToken = strLine.Substring(nBeginIndex).Trim();
                        tokens.Add(RemoveSticky(strToken));
                    }

                    break;
                }

                if (nBeginIndex == nIndex)
                {
                    tokens.Add("");
                }
                else if (nBeginIndex < nIndex)
                {
                    string strToken = strLine.Substring(nBeginIndex, nIndex - nBeginIndex).Trim();
                    tokens.Add(RemoveSticky(strToken));
                }
                else
                    break;

                nBeginIndex = nIndex + 1;
            }

            return tokens;
        }

        private Dictionary<int, LibInfo> ReadAreaList()
        {
            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            StreamReader reader = new StreamReader("F:/libArea.txt", encEUC_KR);

            Dictionary<int, LibInfo> infoList = new Dictionary<int, LibInfo>();

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                {
                    continue;
                }

                List<string> arrTokens = ParseString(strLine, '\t');
                int nTokenCount = arrTokens.Count();

                if (nTokenCount > 3)
                    nTokenCount = 3;

                LibInfo info = new LibInfo();

                for (int i=0;i<nTokenCount;i++)
                {
                    string strData = arrTokens[i].Trim();

                    if (i == 0)
                    {
                        int nID;

                        if (!int.TryParse(strData, out nID))
                            continue;
                        else
                            info.ID = nID;
                    }
                    else if (i == 1)
                    {
                        if (strData.Length == 0 || strData == "-" || strData == "?")
                            continue;

                        double dData;

                        if (!double.TryParse(strData, out dData))
                            continue;
                        else
                            info.Area = (int)dData;
                    }
                    else if (i == 2)
                    {
                        if (strData.Length == 0 || strData == "-" || strData == "?")
                            continue;

                        double dData;

                        if (!double.TryParse(strData, out dData))
                            continue;
                        else
                            info.BookCount = (int)dData;
                    }
                }

                if (info.ID > 0)
                    infoList[info.ID] = info;
            }

            reader.Close();
            return infoList;
        }

        private List<LibInfo> ReadLibraryList()
        {
            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            StreamReader reader = new StreamReader("F:/libList.txt", encEUC_KR);

            List<LibInfo> libraries = new List<LibInfo>();

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                {
                    continue;
                }

                List<string> arrTokens = ParseString(strLine, '\t');
                int nTokenCount = arrTokens.Count();

                if (nTokenCount > 10)
                    nTokenCount = 10;

                LibInfo info = new LibInfo();

                for (int i = 0; i < nTokenCount; i++)
                {
                    string strData = arrTokens[i].Trim();

                    if (i == 0)
                    {
                        int nID;

                        if (!int.TryParse(strData, out nID))
                            continue;
                        else
                            info.ID = nID;
                    }
                    else if (i == 1)
                    {
                        info.Gubun = strData;
                    }
                    else if (i == 2)
                    {
                        info.Addr1 = strData;
                    }
                    else if (i == 3)
                    {
                        info.Addr2 = strData;
                    }
                    else if (i == 4)
                    {
                        info.Addr3 = strData;
                    }
                    else if (i == 5)
                    {
                        info.Addr4 = strData;
                    }
                    else if (i == 8)
                    {
                        if (strData.Length == 0 || strData == "-" || strData == "?")
                            continue;

                        double dData;

                        if (!double.TryParse(strData, out dData))
                            continue;
                        else
                            info.Area = (int)dData;
                    }
                    else if (i == 9)
                    {
                        if (strData.Length == 0 || strData == "-" || strData == "?")
                            continue;

                        double dData;

                        if (!double.TryParse(strData, out dData))
                            continue;
                        else
                            info.BookCount = (int)dData;
                    }
                }

                libraries.Add(info);
            }

            reader.Close();
            return libraries;
        }

        private void CompareLibraryList(List<LibInfo> libraries, Dictionary<int, LibInfo> infoList)
        {
            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            StreamWriter writer = new StreamWriter("F:/result.txt", false, encEUC_KR);

            LibInfo value;

            foreach (LibInfo lib in libraries)
            {
                if (infoList.TryGetValue(lib.ID, out value))
                {
                    lib.Area2 = value.Area;
                    lib.BookCount2 = value.BookCount2;
                }
                else
                {
                    lib.Area2 = lib.Area;
                    lib.BookCount2 = lib.BookCount;
                }
            }

            foreach (LibInfo lib in libraries)
            {
                string strLine = string.Format("{0}\t{1}\t{2}\t{3}\t{4}",
                    lib.ID, lib.Area, lib.BookCount, lib.Area2, lib.BookCount2);

                if (lib.Area != lib.Area2 || lib.BookCount != lib.BookCount2)
                    strLine += "\t&&&&";
                else
                    strLine += "\t";

                int nRadiusCode = -1;

                if (lib.Addr1.StartsWith("서울"))
                {
                    if (lib.Gubun == "대표")
                        nRadiusCode = 0;
                    else if (lib.Gubun == "중앙")
                    {
                        if (lib.Area2 >= 900)
                            nRadiusCode = 1;
                        else if (lib.Area2 >= 600)
                            nRadiusCode = 2;
                        else
                            nRadiusCode = 3;
                    }
                    else if (lib.Gubun == "분관" || lib.Gubun == "거점")
                    {
                        if (lib.Area2 >= 900)
                            nRadiusCode = 4;
                        else if (lib.Area2 >= 600)
                            nRadiusCode = 5;
                        else
                            nRadiusCode = 6;
                    }
                    else if (lib.Gubun == "작은")
                        nRadiusCode = 7;
                }
                else if ((lib.Addr1.StartsWith("부산") || lib.Addr1.StartsWith("울산") || lib.Addr1.StartsWith("대구") ||
                    lib.Addr1.StartsWith("대전") || lib.Addr1.StartsWith("인천") || lib.Addr1.StartsWith("광주")) &&
                    !lib.Addr2.EndsWith("군"))
                {
                    if (lib.Gubun == "대표")
                        nRadiusCode = 8;
                    else if (lib.Gubun == "중앙")
                    {
                        if (lib.Area2 >= 900)
                            nRadiusCode = 9;
                        else if (lib.Area2 >= 600)
                            nRadiusCode = 10;
                        else
                            nRadiusCode = 11;
                    }
                    else if (lib.Gubun == "분관" || lib.Gubun == "거점")
                    {
                        if (lib.Area2 >= 600)
                            nRadiusCode = 12;
                        else
                            nRadiusCode = 13;
                    }
                    else if (lib.Gubun == "작은")
                        nRadiusCode = 14;
                }
                else if (lib.Addr1.StartsWith("세종") || lib.Addr2.EndsWith("시"))
                {
                    if (lib.Gubun == "대표")
                        nRadiusCode = 15;
                    else if (lib.Gubun == "중앙")
                    {
                        if (lib.Area2 >= 900)
                            nRadiusCode = 16;
                        else if (lib.Area2 >= 600)
                            nRadiusCode = 17;
                        else
                            nRadiusCode = 18;
                    }
                    else if (lib.Gubun == "분관" || lib.Gubun == "거점")
                    {
                        if (lib.Area2 >= 600)
                            nRadiusCode = 19;
                        else
                            nRadiusCode = 20;
                    }
                    else if (lib.Gubun == "작은")
                        nRadiusCode = 21;
                }
                else
                {
                    if (lib.Gubun == "대표")
                        nRadiusCode = 22;
                    else if (lib.Gubun == "중앙")
                    {
                        if (lib.Area2 >= 900)
                            nRadiusCode = 23;
                        else if (lib.Area2 >= 600)
                            nRadiusCode = 24;
                        else
                            nRadiusCode = 25;
                    }
                    else if (lib.Gubun == "분관" || lib.Gubun == "거점")
                    {
                        nRadiusCode = 26;
                    }
                    else if (lib.Gubun == "작은")
                        nRadiusCode = 27;
                }

                if (nRadiusCode < 0)
                    continue;
                else
                    strLine += "\t" + nRadiusCode.ToString();

                writer.WriteLine(strLine);
            }

            writer.Close();
        }

        private void Test8()
        {
            Dictionary<int, LibInfo> infoList = ReadAreaList();
            List<LibInfo> libraries = ReadLibraryList();
            CompareLibraryList(libraries, infoList);
        }

        private void Test9()
        {
            StreamReader reader = new StreamReader("F:/1.txt", Encoding.ASCII);
            StreamWriter writer = new StreamWriter("F:/2.txt", false, Encoding.ASCII);

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    writer.WriteLine();
                else
                {
                    string[] arrTokens = strLine.Split('\t');
                    string str1 = arrTokens[0].Trim();
                    string str2 = arrTokens[1].Trim();

                    if (str1 == "0")
                        writer.Write("\t");
                    else
                        writer.Write(str1 + "\t");

                    if (str2 == "0")
                        writer.WriteLine();
                    else
                        writer.WriteLine(str2);
                }
            }

            writer.Close();
            reader.Close();
        }

        public class Do
        {
            public string m_strName = "";
            public List<City> m_cities = new List<City>();

            public City FindCity(string strCityName)
            {
                foreach (City city in m_cities)
                {
                    if (city.m_strName == strCityName)
                        return city;
                }

                return null;
            }
        }

        public class City
        {
            public string m_strName = "";
            public List<Gu> m_gues = new List<Gu>();

            public Gu FindGu(string strGuName)
            {
                foreach (Gu gu in m_gues)
                {
                    if (gu.m_strName == strGuName)
                        return gu;
                }

                return null;
            }
        }

        public class Gu
        {
            public string m_strName = "";
            public List<Dong> m_dongs = new List<Dong>();

            public Dong FindDong(string strDongName)
            {
                foreach (Dong dong in m_dongs)
                {
                    if (dong.m_strDongName == strDongName)
                        return dong;
                }

                return null;
            }
        }

        public class Dong
        {
            public string m_strDongName = "";
        }

        private Do FindDo(string strDo, List<Do> dos)
        {
            foreach (Do _do in dos)
            {
                if (_do.m_strName == strDo)
                    return _do;
            }

            return null;
        }

        // lib_area 설정
        private void MakeAddressDepth()
        {
            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            StreamReader reader = new StreamReader("F:/aaa.txt", encEUC_KR);

            List<Do> dos = new List<Do>();

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine().Trim();
                string[] arrAddr = strLine.Split('\t');

                int nCount = arrAddr.Count();

                if (nCount <= 2 || strLine.Length == 0)
                    continue;

                Do _do = null;
                City city = null;
                Gu gu = null;
                Dong dong = null;

                for (int i = 0; i < nCount; i++)
                {
                    string strData = arrAddr[i].Trim();

                    if (strData.Length == 0)
                        break;

                    if (i == 0)
                    {
                        _do = FindDo(strData, dos);

                        if (_do == null)
                        {
                            _do = new Do();
                            _do.m_strName = strData;
                            dos.Add(_do);
                        }
                    }
                    else if (i == 1)
                    {
                        if (_do == null)
                            break;

                        city = _do.FindCity(strData);

                        if (city == null)
                        {
                            city = new City();
                            city.m_strName = strData;
                            _do.m_cities.Add(city);
                        }
                    }
                    else if (i == 2)
                    {
                        if (city == null)
                            break;

                        bool lastData = false;
                        string strGuName = "";

                        if (strData.EndsWith("동"))
                        {
                            strGuName = strData;
                            lastData = true;
                        }
                        else if (strData.EndsWith("구") || strData.EndsWith("읍") || strData.EndsWith("면") || strData.EndsWith("리"))
                            strGuName = strData;
                        else
                            break;

                        gu = city.FindGu(strGuName);

                        if (gu == null)
                        {
                            gu = new Gu();
                            gu.m_strName = strGuName;
                            city.m_gues.Add(gu);
                        }

                        if (lastData)
                            break;
                    }
                    else if (i == 3)
                    {
                        string[] arrTokens = strData.Split(' ');

                        if (arrTokens.Count() == 0)
                            break;

                        string strDongName = arrTokens[0].Trim();

                        if (!strDongName.EndsWith("동"))
                            break;

                        if (gu == null)
                            break;

                        if (gu.FindDong(strDongName) == null)
                        {
                            dong = new Dong();
                            dong.m_strDongName = strDongName;
                            gu.m_dongs.Add(dong);
                        }
                    }
                }
            }

            reader.Close();

            StreamWriter writer = new StreamWriter("F:/bbb.txt", false, encEUC_KR);
            int nIndex = 1;

            foreach (Do _do in dos)
            {
                foreach (City city in _do.m_cities)
                {
                    if (city.m_gues.Count == 0)
                    {
                        writer.WriteLine(string.Format("insert into libdb.lib_area (idx, depth1, depth2, depth3, depth4) values ({0}, '{1}', '{2}', '', '');",
                            nIndex++, _do.m_strName, city.m_strName));
                        //writer.WriteLine(_do.m_strName + "\t" + city.m_strName);
                    }
                    else
                    {
                        foreach (Gu gu in city.m_gues)
                        {
                            if (gu.m_dongs.Count == 0)
                            {
                                writer.WriteLine(string.Format("insert into libdb.lib_area (idx, depth1, depth2, depth3, depth4) values ({0}, '{1}', '{2}', '{3}', '');",
                                    nIndex++, _do.m_strName, city.m_strName, gu.m_strName));
                                //writer.WriteLine(_do.m_strName + "\t" + city.m_strName + "\t" + gu.m_strName);
                            }
                            else
                            {
                                foreach (Dong dong in gu.m_dongs)
                                {
                                    writer.WriteLine(string.Format("insert into libdb.lib_area (idx, depth1, depth2, depth3, depth4) values ({0}, '{1}', '{2}', '{3}', '{4}');",
                                        nIndex++, _do.m_strName, city.m_strName, gu.m_strName, dong.m_strDongName));
                                    //writer.WriteLine(_do.m_strName + "\t" + city.m_strName + "\t" + gu.m_strName + "\t" + dong.m_strDongName);
                                }
                            }
                        }
                    }
                }
            }

            writer.Close();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            //m_dbMgr.UpdateExcelSheet(@"F:\Project\웹도서관\2014 추가발주(도서관 설치현황 뷰어)\DB자료3.xlsx", "공공도서관");
            // DB 로딩후 지역별 정렬시켜 txt에 출력
            //m_dbMgr.LoadLibraries2();
            //string strCoord = GetAddressCoord("서울시 마포구 도화동 357-8");
            //Test9();
            //Test8();
            //Test7();
            //Test6();
            //Test5();
            //Test4();
            //Test3();
            //Test2();
            /*string strFolder = @"F:\Project\웹도서관\2014 추가발주(도서관 설치현황 뷰어)\유엔이_발송용\20150124\0124_도서관 주소\공공";
            string strFolder2 = @"F:\Project\웹도서관\2014 추가발주(도서관 설치현황 뷰어)\유엔이_발송용\20150124\0124_도서관 주소\작은";
            string strOutFilePath = "F:/aaa.txt";

            // 도시이름, 도서관들
            Dictionary<string, List<Library>> dicCityLibraries = ReadOriginData("F:/OriginLib.txt");*/
            /*Dictionary<string, List<Library>> dicCityLibraries = new Dictionary<string, List<Library>>();
            bool result1 = m_dbMgr.LoadPublicLibraries2(strFolder, dicCityLibraries);
            bool result2 = m_dbMgr.LoadSmallLibraries2(strFolder2, dicCityLibraries);
            WriteOriginData(dicCityLibraries);*/

            // 도서관이름, 도서관들
            /*Dictionary<string, List<Library>> dicNameLibraries = new Dictionary<string, List<Library>>();
            
            ReadLibrary("F:/bbb.txt", dicNameLibraries);

            CompareLibrary(dicCityLibraries, dicNameLibraries, strOutFilePath);

            //string strCoord = GetAddressCoord("경기도 남양주시 호평동 669");
            string strPath1 = "F:/list2.xlsx";
            string strPath2 = @"F:\Project\웹도서관\2014 추가발주(도서관 설치현황 뷰어)\개발 Source\행정구역 영역표시 안됨_20150128\successResult.xlsx";
            m_dbMgr.CompareDatas2(strPath2);*/
            //m_dbMgr.CompareDatas(strPath1, strPath2);
            //Test("F:/test.txt");
            //m_dbMgr.UpdateLibraries5(@"F:\Project\웹도서관\2014 추가발주(도서관 설치현황 뷰어)\미확인주소.txt");
            //m_dbMgr.UpdateLibraries4();
            //m_dbMgr.UpdateLibraries3("F:/list2.txt");
            //ReadAddressCoord("F:/list2.txt");
            //m_dbMgr.UpdateLibraries("F:/list2.txt");
            //m_dbMgr.UpdateLibraries2();
            //m_dbMgr.LoadLibraries();
            //m_dbMgr.LoadPublicLibraries(@"F:\Project\웹도서관\2014 추가발주(도서관 설치현황 뷰어)\유엔이_발송용\공공");
            //m_dbMgr.LoadSmallLibraries(@"F:\Project\웹도서관\2014 추가발주(도서관 설치현황 뷰어)\유엔이_발송용\작은");
        }

        private void Test(string strPath)
        {
            StreamReader reader = new StreamReader(strPath);

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                string[] arrCoord = strLine.Split(',');

                if (arrCoord.Count() != 2)
                {
                    System.Diagnostics.Trace.WriteLine(strLine);
                    continue;
                }

                double x, y;

                if (!double.TryParse(arrCoord[0], out x) || !double.TryParse(arrCoord[1], out y))
                {
                    System.Diagnostics.Trace.WriteLine(strLine);
                }
            }
        }

        private void ReadAddressCoord(string strPath)
        {
            // EUC-KR : 51949
            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            StreamReader reader = new StreamReader(strPath, encEUC_KR);

            StreamWriter writer = new StreamWriter("F:/successResult.txt", false, encEUC_KR);
            StreamWriter writer2 = new StreamWriter("F:/failResult.txt", false, encEUC_KR);

            string strAddr = "";
            int nPrevCount = 0, nSuccessCount = 0, nFailCount = 0;

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine();
                string[] arrDatas = strLine.Split('\t');

                int nDataCount = arrDatas.Count();

                for (int i = 0; i < nDataCount;i++ )
                {
                    arrDatas[i] = arrDatas[i].Trim();
                }

                if (nDataCount > 8)
                    nDataCount = 8;

                if (nDataCount == 8 && arrDatas[7].Length > 0)
                {
                    writer.WriteLine(strLine);
                    nPrevCount++;
                    continue;
                }

                if (nDataCount < 7 || arrDatas[6].Length == 0)
                    goto FAIL;

                strAddr = "";

                for (int i=3;i<7;i++)
                {
                    if (arrDatas[i].Length == 0)
                        goto FAIL;

                    if (strAddr.Length == 0)
                        strAddr = arrDatas[i];
                    else
                        strAddr += " " + arrDatas[i];
                }

                string strCoord = GetAddressCoord(strAddr);

                if (strCoord.Length == 0)
                    goto FAIL;

                for (int i = 0; i < nDataCount && i < 7;i++ )
                {
                    if (i == 0)
                        writer.Write(arrDatas[i]);
                    else
                        writer.Write("\t" + arrDatas[i]);
                }

                for (int i = nDataCount - 1; i < 7;i++)
                {
                    writer.Write("\t");
                }

                writer.WriteLine("\t" + strCoord);
                writer.Flush();
                System.Diagnostics.Trace.WriteLine(arrDatas[0]);
                nSuccessCount++;
                continue;

            FAIL:
                writer2.WriteLine(strLine);
                System.Diagnostics.Trace.WriteLine(arrDatas[0]);
                nFailCount++;
                continue;
            }

            writer.Close();
            writer2.Close();

            System.Diagnostics.Trace.WriteLine("기존 좌표 개수 : " + nPrevCount.ToString());
            System.Diagnostics.Trace.WriteLine("새로 추가된 좌표 개수 : " + nSuccessCount.ToString());
            System.Diagnostics.Trace.WriteLine("실패한 좌표 개수 : " + nFailCount.ToString());
        }

        private string GetAddressCoord(string strAddr)
        {
            string resResult = string.Empty;

            string strDefURL = "http://openapi.map.naver.com/api/geocode.php";
            strDefURL += "?key=e0135c5eb69b7e373d4265d510a143ca";
            strDefURL += "&encoding=utf-8&coord=latlng&query=";

            string sourceUrl = strDefURL + strAddr;

            HttpWebRequest wReq = (HttpWebRequest)WebRequest.Create(sourceUrl);

            wReq.Method = "GET";
            try
            {
                HttpWebResponse wRes = (HttpWebResponse)wReq.GetResponse();

                // http 내용 추출
                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, Encoding.UTF8);

                resResult = readerPost.ReadToEnd();

                readerPost.Close();
                respPostStream.Close();
            }
            catch (System.Net.WebException e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                return "";
            }

            string strX = GetElement("x", resResult);
            string strY = GetElement("y", resResult);

            if (strX.Length == 0 || strY.Length == 0)
                return "";

            return strX + "," + strY;
        }

        private string GetElement(string strTag, string strSrc)
        {
            string strTag1 = "<" + strTag + ">";
            string strTag2 = "</" + strTag + ">";

            int nIndex1 = strSrc.IndexOf(strTag1);
            int nIndex2 = strSrc.IndexOf(strTag2);

            if (nIndex1 < 0 || nIndex2 < 0 || nIndex2 <= nIndex1)
                return "";

            string strResult = strSrc.Substring(nIndex1 + strTag1.Length, nIndex2 - nIndex1 - strTag1.Length);
            return strResult;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "Excel Files|*.xlsx|All FIles|*.*";
            dlg.FilterIndex = 0;
            dlg.Title = "도서관 DB 파일 열기";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBoxFilePath.Text = dlg.FileName;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (textBoxFilePath.Text.Length == 0)
                MessageBox.Show("DB 파일의 경로를 입력하세요.");
            else if (!File.Exists(textBoxFilePath.Text))
                MessageBox.Show("존재하지 않는 경로입니다.\r\n" + textBoxFilePath.Text);
            else
            {
                if (textBoxSheetName.Text.Length == 0)
                    MessageBox.Show("엑셀 쉬트 이름을 입력하세요.");
                else if (!CheckSheetName(textBoxFilePath.Text, textBoxSheetName.Text))
                    MessageBox.Show("존재하지 않는 쉬트 이름입니다.\r\n" + textBoxSheetName.Text);
                else
                {
                    int nBeginIndex = 1;

                    if (radioFromManual.Checked)
                    {
                        if (textBoxFromIndex.Text.Length == 0)
                        {
                            MessageBox.Show("좌표 변환을 시작할 Index 번호를 입력하세요.");
                            return;
                        }

                        if (!int.TryParse(textBoxFromIndex.Text, out nBeginIndex) || nBeginIndex <= 0)
                        {
                            MessageBox.Show("좌표변환을 시작할 Index는 0보다 큰 정수만 입력가능합니다.");
                            return;
                        }
                    }

                    System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(FindCoordThread));
                    t.Start(nBeginIndex);
                }
            }
        }

        private bool CheckSheetName(string strExcelFile, string strSheetName)
        {
            // Excel 프로세스 생성
            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();

            // 읽기전용 열기
            Microsoft.Office.Interop.Excel.Workbook workBook = app.Workbooks.Open(strExcelFile, 0, true, 5, Type.Missing, Type.Missing, false, Type.Missing, Type.Missing, true, false, Type.Missing, false, false, false);

            // sheets 생성
            Microsoft.Office.Interop.Excel.Sheets sheets = workBook.Sheets;
            bool find = false;

            foreach (Microsoft.Office.Interop.Excel.Worksheet sheet in sheets)
            {
                if (sheet.Name == strSheetName)
                {
                    find = true;
                    break;
                }
            }

            DBManagerMySQL.CloseWorkBook(workBook);
            DBManagerMySQL.CloseExcel(app);
            return find;
        }

        private void FindCoordThread(object arg)
        {
            int nBeginIndex = (int)arg;
            m_dbMgr.UpdateExcelSheet(textBoxFilePath.Text, textBoxSheetName.Text, nBeginIndex);
        }

        public void SetTotalCount(int nTotalCount)
        {
            m_nTotalCount = nTotalCount;

            this.Invoke((MethodInvoker)delegate
            {
                if (m_nTotalCount == 0)
                    labelProgress.Text = string.Format("진행률(0%) : {0} / {1}", m_nProgressCount, m_nTotalCount);
                else
                    labelProgress.Text = string.Format("진행률({0}%) : {1} / {2}", m_nProgressCount * 100 / m_nTotalCount, m_nProgressCount, m_nTotalCount);

                progressBar1.Minimum = 0;
                progressBar1.Maximum = m_nTotalCount;
                progressBar1.Step = 1;
                progressBar1.Value = 0;

                progressBar1.Visible = labelProgress.Visible = true;
            });
        }

        public void SetProgressCount(int nProgressCount)
        {
            m_nProgressCount = nProgressCount;

            this.Invoke((MethodInvoker)delegate
            {
                if (m_nTotalCount == 0)
                    labelProgress.Text = string.Format("진행률(0%) : {0} / {1}", m_nProgressCount, m_nTotalCount);
                else
                    labelProgress.Text = string.Format("진행률({0}%) : {1} / {2}", m_nProgressCount * 100 / m_nTotalCount, m_nProgressCount, m_nTotalCount);

                progressBar1.Value = nProgressCount;
            });
        }

        private void textBoxFromIndex_TextChanged(object sender, EventArgs e)
        {
            if (textBoxFromIndex.Text.Length > 0)
            {
                int nIndex = 0;

                if (!int.TryParse(textBoxFromIndex.Text, out nIndex) || nIndex <= 0)
                {
                    MessageBox.Show("좌표변환을 시작할 Index는 0보다 큰 정수만 입력가능합니다.");
                    textBoxFromIndex.Text = m_strPrevIndex;
                    return;
                }
            }

            m_strPrevIndex = textBoxFromIndex.Text;
        }

        private void radioFromManual_CheckedChanged(object sender, EventArgs e)
        {
            if (radioFromManual.Checked)
                textBoxFromIndex.Enabled = true;
        }

        private void radioFromBegin_CheckedChanged(object sender, EventArgs e)
        {
            if (radioFromBegin.Checked)
                textBoxFromIndex.Enabled = false;
        }
    }
}
