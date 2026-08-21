using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace LibraryReader
{
    public class DBManagerMySQL : DBManager
    {
        #region Excel Index
        private const int GUBUN = 0;
        private const int LOCATION = 1;
        private const int OWNER = 2;
        private const int YEAR = 3;
        private const int NAME = 4;
        private const int BOOK_COUNT = 5;
        private const int AREA = 6;
        private const int ADDRESS = 7;
        private const int PHONE_NUMBER = 8;
        private const int INDEX_COUNT = 9;
        #endregion

        private MySqlConnection m_dbConnection = null;
        private Dictionary<string, Library> m_dicOldLibraries = new Dictionary<string, Library>();
        private List<Library> m_smallLibraries = new List<Library>();

        private Dictionary<string, Library> m_dicGubuns = new Dictionary<string, Library>();
        private Dictionary<string, Library> m_dicOwners = new Dictionary<string, Library>();

        public DBManagerMySQL()
        {
        }

        protected override void MakeConnection()
        {
            return;
            char[] arrID = new char[] { 'r', 'o', 'o', 't' };
            char[] arrPW = new char[] { 'l', 'i', 'b', '1', '!', '#', '%', '&', '(' };
            
            m_strServerID = new string(arrID);
            m_strServerPW = new string(arrPW);

            // DB 열기
            Loadini_ServerConnectionInfo();
            m_strConnection = GetStringConnection();
            m_dbConnection = new MySqlConnection(m_strConnection);

            m_isConnection = OpenConnection();
        }

        public void ReadDB(string strSQL, object transaction, out MySqlDataReader reader)
        {
            MySqlCommand cmd = new MySqlCommand(strSQL, m_dbConnection);
            reader = cmd.ExecuteReader();
        }

        public void Execute(string strSQL, object transaction = null)
        {
            MySqlCommand cmd = new MySqlCommand(strSQL, m_dbConnection);
            cmd.ExecuteNonQuery();
        }

        public override bool OpenConnection()
        {
            try
            {
                m_dbConnection.Open();
                return true;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                System.Windows.Forms.Application.Exit();
                return false;
            }
        }

        //Close connection
        public override bool CloseConnection()
        {
            try
            {
                m_isConnection = false;
                m_dbConnection.Close();
                return true;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                System.Windows.Forms.Application.Exit();
                return false;
            }
        }

        public bool UpdateLibraries(string strPath)
        {
            // EUC-KR : 51949
            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            StreamReader reader = new StreamReader(strPath, encEUC_KR);

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine();
                string[] arrDatas = strLine.Split('\t');

                int nCount = arrDatas.Count();
                Library lib = new Library();
                string[] arrAddr = new string[6] { "", "", "", "", "", "" };

                if (nCount > 9)
                    nCount = 9;

                for (int i=0;i<nCount;i++)
                {
                    string strData = arrDatas[i].Trim();

                    if (i == 0)
                        lib.ID = int.Parse(strData);
                    else if (i == 1)
                        lib.Name = strData;
                    else if (i == 2)
                        lib.Location = strData;
                    else
                        arrAddr[i - 3] = strData;
                }

                string strSQL = string.Format("update lib_list2 set location = '{0}', addr1 = '{1}', addr2 = '{2}', addr3 = '{3}', addr4 = '{4}', addr5 = '{5}', addr6 = '{6}' where idx = {7}",
                    lib.Location, arrAddr[0], arrAddr[1], arrAddr[2], arrAddr[3], arrAddr[4], arrAddr[5], lib.ID);

                try
                {
                    Execute(strSQL);
                }
                catch (Exception e)
                {
                    reader.Close();
                    System.Diagnostics.Trace.WriteLine(e.Message);
                    return false;
                }
            }

            reader.Close();
            return true;
        }

        public bool UpdateLibraries2()
        {
            string strSQL = "select idx, name, location, addr1, addr2, addr3, addr4, addr5, addr6 from lib_list2";

            // EUC-KR : 51949
            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            System.IO.StreamWriter writer = new StreamWriter("f:\\list2.txt", false, encEUC_KR);

            List<Library> lib2List = new List<Library>();

            MySqlDataReader reader;
            ReadDB(strSQL, null, out reader);

            while (reader.Read())
            {
                int nID = (int)reader[0];
                string strName = reader.IsDBNull(1) ? "" : reader[1].ToString().Trim();
                string strLocation = reader.IsDBNull(2) ? "" : reader[2].ToString().Trim();
                
                string strAddr1 = reader.IsDBNull(3) ? "" : reader[3].ToString().Trim();
                string strAddr2 = reader.IsDBNull(4) ? "" : reader[4].ToString().Trim();
                string strAddr3 = reader.IsDBNull(5) ? "" : reader[5].ToString().Trim();
                string strAddr4 = reader.IsDBNull(6) ? "" : reader[6].ToString().Trim();
                string strAddr5 = reader.IsDBNull(7) ? "" : reader[7].ToString().Trim();
                string strAddr6 = reader.IsDBNull(8) ? "" : reader[8].ToString().Trim();

                Library lib = new Library();

                lib.ID = nID;
                lib.Name = strName;
                lib.Location = strLocation;
                lib.Addr1 = strAddr1;
                lib.Addr2 = strAddr2;
                lib.Addr3 = strAddr3;
                lib.Addr4 = strAddr4 + " " + strAddr5 + " " + strAddr6;

                lib2List.Add(lib);
            }

            reader.Close();

            strSQL = "select idx, name, addr1, addr2, lng from lib_list";

            ReadDB(strSQL, null, out reader);

            while (reader.Read())
            {
                int nID = (int)reader[0];
                string strName = reader.IsDBNull(1) ? "" : reader[1].ToString().Trim();
                string strAddr1 = reader.IsDBNull(2) ? "" : reader[2].ToString().Trim();
                string strAddr2 = reader.IsDBNull(3) ? "" : reader[3].ToString().Trim();
                string strLng = reader.IsDBNull(4) ? "" : reader[4].ToString().Trim();

                Library lib = FindLibrary(nID, strName, strAddr1, strAddr2, lib2List);

                if (lib != null)
                    lib.Coord = strLng;
            }

            foreach (Library lib in lib2List)
            {
                writer.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}",
                    lib.ID, lib.Name, lib.Location, lib.Addr1, lib.Addr2, lib.Addr3, lib.Addr4, lib.Coord));
            }

            reader.Close();

            writer.Close();
            return true;
        }

        public void UpdateLibraries3(string strPath)
        {
            // EUC-KR : 51949
            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            StreamReader reader = new StreamReader(strPath, encEUC_KR);

            int nID;

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

                if (nDataCount < 8 || arrDatas[7].Length == 0)
                    continue;

                if (!int.TryParse(arrDatas[0], out nID))
                    continue;

                if (arrDatas[7].StartsWith("\""))
                    arrDatas[7] = arrDatas[7].Substring(1);

                if (arrDatas[7].EndsWith("\""))
                    arrDatas[7] = arrDatas[7].Substring(0, arrDatas[7].Length - 1);

                string strSQL = "Update lib_list2 set lng = '" + arrDatas[7] + "' where idx = " + nID.ToString();

                try
                {
                    Execute(strSQL);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                    return;
                }
            }

            reader.Close();
        }

        public bool UpdateLibraries5(string strPath)
        {
            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            StreamReader reader = new StreamReader(strPath, encEUC_KR);

            StreamWriter writer = new StreamWriter("F:/update.sql", false, Encoding.UTF8);
            writer.WriteLine("use libdb;");
            writer.WriteLine();

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine().Trim();
                string[] arrDatas = strLine.Split('\t');

                int nDataCount = arrDatas.Count();

                for (int i=0;i<nDataCount;i++)
                {
                    arrDatas[i] = arrDatas[i].Trim();

                    if (arrDatas[i].StartsWith("\""))
                        arrDatas[i] = arrDatas[i].Substring(1);

                    if (arrDatas[i].EndsWith("\""))
                        arrDatas[i] = arrDatas[i].Substring(0, arrDatas[i].Length - 1);
                }

                string strAddr1 = "", strAddr2 = "", strAddr3 = "", strAddr4 = "", strLng = "";

                if (nDataCount < 8 || arrDatas[7].Length == 0)
                {
                    if (nDataCount >= 4)
                        strAddr1 = arrDatas[3];

                    if (nDataCount >= 5)
                        strAddr2 = arrDatas[4];

                    if (nDataCount >= 6)
                        strAddr3 = arrDatas[5];

                    if (nDataCount >= 7)
                        strAddr4 = arrDatas[6];
                }
                else
                {
                    // 동 정보에 나머지 주소가 붙어있을 경우 Addr4로 옮긴다.
                    if (arrDatas[5].Length > 0)
                    {
                        string[] arrTokens = arrDatas[5].Split(' ');
                        int nTokenCount = arrTokens.Count();

                        if (nTokenCount > 1)
                        {
                            string strAdd = "";

                            for (int i = 1; i < nTokenCount; i++)
                            {
                                if (strAdd.Length == 0)
                                    strAdd = arrTokens[i];
                                else
                                    strAdd += " " + arrTokens[i];
                            }

                            if (strAdd.Length > 0)
                                arrDatas[6] = strAdd + " " + arrDatas[6];

                            arrDatas[5] = arrTokens[0];
                        }
                    }

                    strAddr1 = arrDatas[3];
                    strAddr2 = arrDatas[4];
                    strAddr3 = arrDatas[5];
                    strAddr4 = arrDatas[6];
                    strLng = arrDatas[7];
                }

                string strSQL = string.Format("Update lib_list2 set addr1 = '{0}', addr2 = '{1}', addr3 = '{2}', addr4 = '{3}', lng = '{4}' where idx = {5};",
                    strAddr1, strAddr2, strAddr3, strAddr4, strLng, arrDatas[0]);

                writer.WriteLine(strSQL);
                /*try
                {
                    Execute(strSQL);
                }
                catch (Exception e)
                {
                    reader.Close();
                    System.Diagnostics.Trace.WriteLine(e.Message);
                    return false;
                }*/
            }

            writer.Close();
            return true;
        }

        public bool UpdateLibraries4()
        {
            string strSQL = "select idx, name, location, addr1, addr2 from lib_list2";

            List<Library> lib2List = new List<Library>();

            MySqlDataReader reader;
            ReadDB(strSQL, null, out reader);

            while (reader.Read())
            {
                int nID = (int)reader[0];
                string strName = reader.IsDBNull(1) ? "" : reader[1].ToString().Trim();
                string strLocation = reader.IsDBNull(2) ? "" : reader[2].ToString().Trim();

                string strAddr1 = reader.IsDBNull(3) ? "" : reader[3].ToString().Trim();
                string strAddr2 = reader.IsDBNull(4) ? "" : reader[4].ToString().Trim();

                Library lib = new Library();

                lib.ID = nID;
                lib.Name = strName;
                lib.Location = strLocation;
                lib.Addr1 = strAddr1;
                lib.Addr2 = strAddr2;

                lib2List.Add(lib);
            }

            reader.Close();

            List<Library> txtLibraries = ReadTxtDatas();

            // 1. 데이터 만들기
            List<Library> newLibraries = new List<Library>();

            /*List<Library> findLibraries = new List<Library>();
            List<Library> lostLibraries = new List<Library>();

            foreach (Library txtLib in txtLibraries)
            {
                Library lib2 = FindLibrary(txtLib, lib2List);

                if (lib2 == null)
                    newLibraries.Add(txtLib);
                else
                {
                    findLibraries.Add(lib2);

                    strSQL = string.Format("Update lib_list2 set year = '{0}' where idx = {1}", txtLib.Year, lib2.ID);

                    try
                    {
                        Execute(strSQL);
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Trace.WriteLine(e.Message);
                    }
                }
            }

            foreach (Library lib in lib2List)
            {
                if (!findLibraries.Contains(lib))
                    lostLibraries.Add(lib);
            }

            WriteNewLibraries(lostLibraries, @"F:\Project\웹도서관\2014 추가발주(도서관 설치현황 뷰어)\유엔이_발송용\20150124\lost.txt");
            WriteNewLibraries(newLibraries, @"F:\Project\웹도서관\2014 추가발주(도서관 설치현황 뷰어)\유엔이_발송용\20150124\new.txt");*/

            // 2. 데이터 적용하기
            foreach (Library txtLib in txtLibraries)
            {
                Library lib2 = FindLibrary(txtLib, lib2List);

                if (lib2 != null)
                {
                    // DB Update
                    txtLib.ID = lib2.ID;
                    UpdateLibrary(txtLib);
                }
            }

            //RemoveLibraries(@"F:\Project\웹도서관\2014 추가발주(도서관 설치현황 뷰어)\유엔이_발송용\20150124\delete.txt");
            InsertNUpdateLibraries(@"F:\Project\웹도서관\2014 추가발주(도서관 설치현황 뷰어)\유엔이_발송용\20150124\insert.txt", txtLibraries);

            return true;
        }

        private void WriteNewLibraries(List<Library> libs, string strPath)
        {
            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            StreamWriter writer = new StreamWriter(strPath, false, encEUC_KR);

            foreach (Library lib in libs)
            {
                writer.Write(lib.ID);
                writer.Write("\t" + lib.Name);
                writer.Write("\t" + lib.Addr1);
                writer.Write("\t" + lib.Addr2);
                writer.Write("\t" + lib.Addr3);
                writer.WriteLine("\t" + lib.Addr4);
            }

            writer.Close();
        }

        private void RemoveLibraries(string strPath)
        {
            StreamReader reader = new StreamReader(strPath);
            string strIDs = "";

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine().Trim();

                if (strIDs.Length == 0)
                    strIDs = strLine;
                else
                    strIDs += ", " + strLine;
            }

            reader.Close();

            if (strIDs.Length == 0)
                return;

            string strSQL = "Delete from lib_list2 where idx in (" + strIDs + ")";

            try
            {
                Execute(strSQL);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
        }

        private void InsertNUpdateLibraries(string strPath, List<Library> libraries)
        {
            string strSQL = "Select max(idx) from lib_list2";

            MySqlDataReader reader2;
            ReadDB(strSQL, null, out reader2);

            int nID = 0;

            if (reader2.Read())
            {
                try
                {
                    nID = (int)reader2[0];
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                    return;
                }
            }

            reader2.Close();

            if (nID == 0)
                return;
            else
                nID++;

            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            StreamReader reader = new StreamReader(strPath, encEUC_KR);

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine().Trim();

                string[] arrDatas = strLine.Split('\t');

                if (arrDatas.Count() != 2)
                    continue;

                arrDatas[0] = arrDatas[0].Trim();
                arrDatas[1] = arrDatas[1].Trim();

                Library lib = FindLibrary(arrDatas[1], libraries);

                if (arrDatas[0] == "-1")
                {
                    if (lib == null)
                        continue;
                    else
                    {
                        SetGrade(lib);
                        InsertLibrary(lib, ref nID);
                    }
                }
                else
                {
                    if (lib == null)
                        continue;
                    else
                    {
                        lib.ID = int.Parse(arrDatas[0]);
                        UpdateLibrary(lib);
                    }
                }
            }

            reader.Close();
        }

        private void SetGrade(Library lib)
        {
            if (lib.Addr1.StartsWith("서울"))
            {
                if (lib.GubunType == Library.LibraryType.중앙)
                    lib.Grade = 1;
                else if (lib.GubunType == Library.LibraryType.거점)
                    lib.Grade = 2;
                else if (lib.GubunType == Library.LibraryType.분관)
                {
                    if (lib.Area >= 900)
                        lib.Grade = 3;
                    else if (lib.Area >= 600)
                        lib.Grade = 4;
                    else
                        lib.Grade = 5;
                }
            }
            else if (lib.Addr1.StartsWith("광주") || lib.Addr1.StartsWith("대구") || lib.Addr1.StartsWith("대전")
                || lib.Addr1.StartsWith("부산") || lib.Addr1.StartsWith("세종") || lib.Addr1.StartsWith("울산")
                || lib.Addr1.StartsWith("인천"))
            {
                if (lib.GubunType == Library.LibraryType.중앙)
                    lib.Grade = 6;
                else if (lib.GubunType == Library.LibraryType.거점)
                    lib.Grade = 7;
                else if (lib.GubunType == Library.LibraryType.분관)
                {
                    if (lib.Area >= 900)
                        lib.Grade = 8;
                    else if (lib.Area >= 600)
                        lib.Grade = 9;
                    else
                        lib.Grade = 10;
                }
            }
            else
            {
                if (lib.GubunType == Library.LibraryType.중앙)
                    lib.Grade = 11;
                else if (lib.GubunType == Library.LibraryType.거점)
                    lib.Grade = 12;
                else if (lib.GubunType == Library.LibraryType.분관)
                {
                    if (lib.Area >= 900)
                        lib.Grade = 13;
                    else if (lib.Area >= 600)
                        lib.Grade = 14;
                    else
                        lib.Grade = 15;
                }
            }
        }

        private Library FindLibrary(string strName, List<Library> libraries)
        {
            foreach (Library lib in libraries)
            {
                if (lib.Name == strName)
                    return lib;
            }

            return null;
        }

        private bool UpdateLibrary(Library lib)
        {
            string strSQL = "";

            if (lib.PhoneNumber.Length > 0)
            {
                strSQL = string.Format("Update lib_list2 set name = '{0}', year = '{1}', tel = '{2}', addr1 = '{3}', addr2 = '{4}', addr3 = '{5}', addr4 = '{6}', addr5 = '', addr6 = '', area_count = '{7}', user_count = '{8}' where idx = {9}",
                    lib.Name, lib.Year, lib.PhoneNumber, lib.Addr1, lib.Addr2, lib.Addr3, lib.Addr4,
                    lib.Area, lib.UserCount, lib.ID);
            }
            else
            {
                strSQL = string.Format("Update lib_list2 set name = '{0}', year = '{1}', addr1 = '{2}', addr2 = '{3}', addr3 = '{4}', addr4 = '{5}', addr5 = '', addr6 = '', area_count = '{6}', user_count = '{7}' where idx = {8}",
                    lib.Name, lib.Year, lib.Addr1, lib.Addr2, lib.Addr3, lib.Addr4,
                    lib.Area, lib.UserCount, lib.ID);
            }

            try
            {
                Execute(strSQL);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        private List<Library> ReadTxtDatas()
        {
            string strPublicPath = @"F:\Project\웹도서관\2014 추가발주(도서관 설치현황 뷰어)\유엔이_발송용\20150124\공공";
            string strSmallPath = @"F:\Project\웹도서관\2014 추가발주(도서관 설치현황 뷰어)\유엔이_발송용\20150124\작은";

            List<Library> libraries = new List<Library>();
            ReadPublicLibraries(strPublicPath, libraries);
            ReadSmallLibraries(strSmallPath, libraries);

            return libraries;
        }

        private void ReadSmallLibraries(string strPath, List<Library> libraries)
        {
            string[] arrFiles = Directory.GetFiles(strPath);
            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);

            int nDiv = -1, nLoc = -1, nOwner = -1, nYear = -1, nName = -1, nBookCount = -1;
            int nLibArea = -1, nPhone = -1, nAddr = -1;

            foreach (string strFile in arrFiles)
            {
                int nIndex2 = strFile.LastIndexOf('.');
                int nIndex1 = strFile.LastIndexOf('\\');

                if (nIndex1 < 0 || nIndex2 < 2)
                    continue;

                string strCityName = strFile.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);

                StreamReader reader = new StreamReader(strFile, encEUC_KR);
                bool isFirst = true;

                while (!reader.EndOfStream)
                {
                    string strLine = reader.ReadLine().Trim();

                    if (strLine.Length == 0)
                        break;

                    if (isFirst)
                    {
                        while (!strLine.EndsWith("주소"))
                            strLine += reader.ReadLine().Trim();
                    }

                    string[] arrDatas = strLine.Split('\t');

                    int nDataCount = arrDatas.Count();

                    for (int i = 0; i < nDataCount; i++)
                    {
                        arrDatas[i] = arrDatas[i].Trim();

                        if (arrDatas[i].StartsWith("\"") && arrDatas[i].EndsWith("\""))
                            arrDatas[i] = arrDatas[i].Substring(1, arrDatas[i].Length - 2);
                    }

                    if (isFirst)
                    {
                        isFirst = false;

                        for (int i = 0; i < nDataCount; i++)
                        {
                            if (arrDatas[i].StartsWith("구분"))
                                nDiv = i;
                            else if (arrDatas[i] == "지역")
                                nLoc = i;
                            else if (arrDatas[i].StartsWith("설립주체"))
                                nOwner = i;
                            else if (arrDatas[i].StartsWith("개관년도"))
                                nYear = i;
                            else if (arrDatas[i].StartsWith("도서관명"))
                                nName = i;
                            else if (arrDatas[i].StartsWith("도서자료"))
                                nBookCount = i;
                            else if (arrDatas[i].StartsWith("도서관 연면적"))
                                nLibArea = i;
                            else if (arrDatas[i].StartsWith("전화번호"))
                                nPhone = i;
                            else if (arrDatas[i].StartsWith("주소"))
                                nAddr = i;
                        }

                        if (nDiv < 0 || nLoc < 0 || nOwner < 0 || nYear < 0 || nName < 0 ||
                            nBookCount < 0 || nLibArea < 0 || nPhone < 0 || nAddr < 0)
                            break;
                    }
                    else
                    {
                        Library lib = new Library();

                        for (int i = 0; i < nDataCount; i++)
                        {
                            if (i == nDiv)
                                lib.GubunType = Library.LibraryType.작은;
                            else if (i == nLoc)
                                lib.Location = arrDatas[i];
                            else if (i == nOwner)
                                SetOwner(lib, arrDatas[i]);
                            else if (i == nYear)
                            {
                                int year;

                                if (!int.TryParse(arrDatas[i], out year))
                                    continue;

                                lib.Year = year;

                                if (nYear >= 2015)
                                    lib.OwnType = Library.OwnerType.건립중;
                            }
                            else if (i == nName)
                                lib.Name = arrDatas[i];
                            else if (i == nBookCount)
                                lib.UserCount = arrDatas[i];
                            else if (i == nLibArea)
                            {
                                double dArea;

                                if (double.TryParse(arrDatas[i], out dArea))
                                    lib.Area = (int)dArea;
                            }
                            else if (i == nPhone)
                                lib.PhoneNumber = arrDatas[i];
                            else if (i == nAddr)
                                SetAddress(lib, arrDatas[i], strCityName);
                        }

                        libraries.Add(lib);
                    }
                }

                reader.Close();
            }
        }

        private void ReadPublicLibraries(string strPath, List<Library> libraries)
        {
            string[] arrFiles = Directory.GetFiles(strPath);
            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);

            int nDiv = -1, nLoc = -1, nOwner = -1, nYear = -1, nName = -1, nBookCount = -1;
            int nLibArea = -1, nPhone = -1, nAddr = -1;

            foreach (string strFile in arrFiles)
            {
                int nIndex2 = strFile.LastIndexOf('.');
                int nIndex1 = strFile.LastIndexOf('\\');

                if (nIndex1 < 0 || nIndex2 < 2)
                    continue;

                string strCityName = strFile.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);

                StreamReader reader = new StreamReader(strFile, encEUC_KR);
                bool isFirst = true;

                while (!reader.EndOfStream)
                {
                    string strLine = reader.ReadLine().Trim();

                    if (strLine.Length == 0)
                        break;

                    if (isFirst)
                    {
                        while (!strLine.EndsWith("주소"))
                            strLine += reader.ReadLine().Trim();
                    }

                    string[] arrDatas = strLine.Split('\t');

                    int nDataCount = arrDatas.Count();

                    for (int i = 0; i < nDataCount; i++)
                    {
                        arrDatas[i] = arrDatas[i].Trim();

                        if (arrDatas[i].StartsWith("\"") && arrDatas[i].EndsWith("\""))
                            arrDatas[i] = arrDatas[i].Substring(1, arrDatas[i].Length - 2);
                    }

                    if (isFirst)
                    {
                        isFirst = false;

                        for (int i = 0; i < nDataCount; i++)
                        {
                            if (arrDatas[i].StartsWith("구분2"))
                                nDiv = i;
                            else if (arrDatas[i].StartsWith("지역"))
                                nLoc = i;
                            else if (arrDatas[i].StartsWith("설립주체"))
                                nOwner = i;
                            else if (arrDatas[i].StartsWith("개관년도"))
                                nYear = i;
                            else if (arrDatas[i].StartsWith("도서관명"))
                                nName = i;
                            else if (arrDatas[i].StartsWith("도서자료"))
                                nBookCount = i;
                            else if (arrDatas[i].StartsWith("도서관 연면적"))
                                nLibArea = i;
                            else if (arrDatas[i].StartsWith("전화번호"))
                                nPhone = i;
                            else if (arrDatas[i].StartsWith("주소"))
                                nAddr = i;
                        }

                        if (nDiv < 0 || nLoc < 0 || nOwner < 0 || nYear < 0 || nName < 0 ||
                            nBookCount < 0 || nLibArea < 0 || nPhone < 0 || nAddr < 0)
                            break;
                    }
                    else
                    {
                        Library lib = new Library();

                        for (int i = 0; i < nDataCount; i++)
                        {
                            if (i == nDiv)
                                SetGubun(lib, arrDatas[i]);
                            else if (i == nLoc)
                                lib.Location = arrDatas[i];
                            else if (i == nOwner)
                                SetOwner(lib, arrDatas[i]);
                            else if (i == nYear)
                            {
                                int year;

                                if (!int.TryParse(arrDatas[i], out year))
                                    continue;

                                lib.Year = year;

                                if (nYear >= 2015)
                                    lib.OwnType = Library.OwnerType.건립중;
                            }
                            else if (i == nName)
                                lib.Name = arrDatas[i];
                            else if (i == nBookCount)
                                lib.UserCount = arrDatas[i];
                            else if (i == nLibArea)
                            {
                                double dArea;

                                if (double.TryParse(arrDatas[i], out dArea))
                                    lib.Area = (int)dArea;
                            }
                            else if (i == nPhone)
                                lib.PhoneNumber = arrDatas[i];
                            else if (i == nAddr)
                                SetAddress(lib, arrDatas[i], strCityName);
                        }

                        libraries.Add(lib);
                    }
                }

                reader.Close();
            }
        }

        private void SetOwner(Library lib, string strOwner)
        {
            if (strOwner.StartsWith("지자체"))
                lib.OwnType = Library.OwnerType.지자체;
            else if (strOwner.StartsWith("교육청"))
                lib.OwnType = Library.OwnerType.교육청;
            else if (strOwner.StartsWith("사립"))
                lib.OwnType = Library.OwnerType.사립;
            else// if (strOwner.Length == 0)
                lib.OwnType = Library.OwnerType.지자체;
        }

        private void SetAddress(Library lib, string strAddr, string strCityName)
        {
            string[] arrAddr = strAddr.Split(' ');
            int nAddrCount = arrAddr.Count();

            for (int i = 0; i < nAddrCount; i++)
            {
                arrAddr[i] = arrAddr[i].Trim();
            }

            if (nAddrCount == 0)
                return;

            if (nAddrCount == 1)
                lib.Addr1 = arrAddr[0];
            else if (nAddrCount == 2)
            {
                lib.Addr1 = arrAddr[0];
                lib.Addr2 = arrAddr[1];
            }
            else if (nAddrCount == 3)
            {
                lib.Addr1 = arrAddr[0];
                lib.Addr2 = arrAddr[1];
                lib.Addr3 = arrAddr[2];
            }
            else
            {
                lib.Addr1 = arrAddr[0];
                lib.Addr2 = arrAddr[1];
                lib.Addr3 = arrAddr[2];

                for (int i = 3; i < nAddrCount; i++)
                {
                    lib.Addr3 += " " + arrAddr[i];
                }
            }

            if (lib.Addr1.StartsWith("서울"))
                lib.Addr1 = "서울특별시";
            else if (lib.Addr1.StartsWith("세종"))
                lib.Addr1 = "세종특별자치시";
            else if (lib.Addr1.StartsWith("인천"))
                lib.Addr1 = "인천광역시";
            else if (lib.Addr1.StartsWith("대전"))
                lib.Addr1 = "대전광역시";
            else if (lib.Addr1.StartsWith("대구"))
                lib.Addr1 = "대구광역시";
            else if (lib.Addr1.StartsWith("광주"))
                lib.Addr1 = "광주광역시";
            else if (lib.Addr1.StartsWith("부산"))
                lib.Addr1 = "부산광역시";
            else if (lib.Addr1.StartsWith("울산"))
                lib.Addr1 = "울산광역시";
            else if (lib.Addr1.StartsWith("경기"))
                lib.Addr1 = "경기도";
            else if (lib.Addr1.StartsWith("강원"))
                lib.Addr1 = "강원도";
            else if (lib.Addr1.StartsWith("충북") || lib.Addr1.StartsWith("충청북"))
                lib.Addr1 = "충청북도";
            else if (lib.Addr1.StartsWith("충남") || lib.Addr1.StartsWith("충청남"))
                lib.Addr1 = "충청남도";
            else if (lib.Addr1.StartsWith("전북") || lib.Addr1.StartsWith("전라북"))
                lib.Addr1 = "전라북도";
            else if (lib.Addr1.StartsWith("전남") || lib.Addr1.StartsWith("전라남"))
                lib.Addr1 = "전라남도";
            else if (lib.Addr1.StartsWith("경북") || lib.Addr1.StartsWith("경상북"))
                lib.Addr1 = "경상북도";
            else if (lib.Addr1.StartsWith("경남") || lib.Addr1.StartsWith("경상남"))
                lib.Addr1 = "경상남도";
            else if (lib.Addr1.StartsWith("제주"))
                lib.Addr1 = "제주특별자치도";
            else
            {
                ResetAddress(lib, strCityName);
            }
        }

        private void ResetAddress(Library lib, string strCityName)
        {
            string strAddr1 = "";

            if (strCityName.StartsWith("서울"))
                strAddr1 = "서울특별시";
            else if (strCityName.StartsWith("세종"))
                strAddr1 = "세종특별자치시";
            else if (strCityName.StartsWith("인천"))
                strAddr1 = "인천광역시";
            else if (strCityName.StartsWith("대전"))
                strAddr1 = "대전광역시";
            else if (strCityName.StartsWith("대구"))
                strAddr1 = "대구광역시";
            else if (strCityName.StartsWith("광주"))
                strAddr1 = "광주광역시";
            else if (strCityName.StartsWith("부산"))
                strAddr1 = "부산광역시";
            else if (strCityName.StartsWith("울산"))
                strAddr1 = "울산광역시";
            else if (strCityName.StartsWith("경기"))
                strAddr1 = "경기도";
            else if (strCityName.StartsWith("강원"))
                strAddr1 = "강원도";
            else if (strCityName.StartsWith("충북") || strCityName.StartsWith("충청북"))
                strAddr1 = "충청북도";
            else if (strCityName.StartsWith("충남") || strCityName.StartsWith("충청남"))
                strAddr1 = "충청남도";
            else if (strCityName.StartsWith("전북") || strCityName.StartsWith("전라북"))
                strAddr1 = "전라북도";
            else if (strCityName.StartsWith("전남") || strCityName.StartsWith("전라남"))
                strAddr1 = "전라남도";
            else if (strCityName.StartsWith("경북") || strCityName.StartsWith("경상북"))
                strAddr1 = "경상북도";
            else if (strCityName.StartsWith("경남") || strCityName.StartsWith("경상남"))
                strAddr1 = "경상남도";
            else if (strCityName.StartsWith("제주"))
                strAddr1 = "제주특별자치도";
            else
                return;

            if (lib.Addr4.Length > 0)
            {
                lib.Addr4 = lib.Addr3 + "\t" + lib.Addr4;
                lib.Addr3 = lib.Addr2;
                lib.Addr2 = lib.Addr1;
            }
            else if (lib.Addr3.Length > 0)
            {
                lib.Addr3 = lib.Addr2 + "\t" + lib.Addr3;
                lib.Addr2 = lib.Addr1;
            }
            else if (lib.Addr2.Length > 0)
            {
                lib.Addr2 = lib.Addr1 + "\t" + lib.Addr2;
            }

            lib.Addr1 = strAddr1;
        }

        private void SetGubun(Library lib, string strGubun)
        {
            if (strGubun.StartsWith("중"))
                lib.GubunType = Library.LibraryType.중앙;
            else if (strGubun.StartsWith("대"))
                lib.GubunType = Library.LibraryType.대표;
            else if (strGubun.StartsWith("거"))
                lib.GubunType = Library.LibraryType.거점;
            else if (strGubun.StartsWith("분") || strGubun.StartsWith("븐") || strGubun.StartsWith("붖"))
                lib.GubunType = Library.LibraryType.분관;
            else if (strGubun.StartsWith("교"))
                lib.GubunType = Library.LibraryType.중앙;
            else
                lib.GubunType = Library.LibraryType.UNKNOWN;
        }

        private Library FindLibrary(Library lib, List<Library> libraries)
        {
            foreach (Library lib2 in libraries)
            {
                //if (lib.Name == lib2.Name &&
                if (lib2.Name.Contains(lib.Name) &&
                    lib.Addr1 == lib2.Addr1 &&
                    lib.Addr1 == lib2.Addr1)
                    return lib2;
            }

            return null;
        }

        private Library FindLibrary(int nID, string strName, string strAddr1, string strAddr2, List<Library> libraries)
        {
            List<Library> finds = new List<Library>();

            foreach (Library lib in libraries)
            {
                if (lib.Name.Contains(strName) && lib.Addr1 == strAddr1 && lib.Addr2 == strAddr2)
                    finds.Add(lib);
            }

            int nCount = finds.Count;

            if (nCount == 0)
                return null;

            if (nCount == 1)
                return finds[0];

            FormSelectLibrary frm = new FormSelectLibrary(nID, strName, finds);

            if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return frm.SelectedLibrary;
            }

            return null;
        }

        private class GuLibrary : System.IComparable
        {
            public Library m_lib = null;

            public int CompareTo(object obj)
            {
                GuLibrary gu2 = (GuLibrary)obj;

                return this.m_lib.Year - gu2.m_lib.Year;
            }
        }

        private class CityLibrary : System.IComparable
        {
            public List<GuLibrary> libraries = new List<GuLibrary>();
            public string m_strGuName = "";

            public int CompareTo(object obj)
            {
                CityLibrary city2 = (CityLibrary)obj;

                return this.m_strGuName.CompareTo(city2.m_strGuName);
            }

            /*public int Compare(object obj1, object obj2)
            {
                CityLibrary city1 = (CityLibrary)obj1;
                CityLibrary city2 = (CityLibrary)obj2;

                return city1.m_strGuName.CompareTo(city2.m_strGuName);
            }*/
        }

        private CityLibrary FindCityLibrary(List<CityLibrary> libraries, string strGuName)
        {
            foreach (CityLibrary city in libraries)
            {
                if (city.m_strGuName == strGuName)
                    return city;
            }

            return null;
        }

        // Return : 주소, 좌표
        public static Dictionary<string, string> GetAddressCoord(string strAddr)
        {
            string resResult = string.Empty;

            string strDefURL = "http://openapi.map.naver.com/api/geocode.php";
            strDefURL += "?key=e0135c5eb69b7e373d4265d510a143ca";
            strDefURL += "&encoding=utf-8&coord=latlng&query=";

            string sourceUrl = strDefURL + strAddr;
            Dictionary<string, string> dicAddressCoords = new Dictionary<string, string>();

            System.Net.HttpWebRequest wReq = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(sourceUrl);

            wReq.Method = "GET";
            try
            {
                System.Net.HttpWebResponse wRes = (System.Net.HttpWebResponse)wReq.GetResponse();

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
                return dicAddressCoords;
            }

            int nBeginIndex = 0;

            while (true)
            {
                string strX = GetElement("x", resResult, ref nBeginIndex);
                if (strX.Length == 0)
                    break;

                string strY = GetElement("y", resResult, ref nBeginIndex);
                if (strY.Length == 0)
                    break;

                string strAddress = GetElement("address", resResult, ref nBeginIndex);
                if (strAddress.Length == 0)
                    break;

                dicAddressCoords[strAddress] = strX + "," + strY;
            }

            return dicAddressCoords;
        }

        private static string GetElement(string strTag, string strSrc, ref int nIndex)
        {
            string strTag1 = "<" + strTag + ">";
            string strTag2 = "</" + strTag + ">";

            int nIndex1 = strSrc.IndexOf(strTag1, nIndex);
            int nIndex2 = strSrc.IndexOf(strTag2, nIndex);

            if (nIndex1 < 0 || nIndex2 < 0 || nIndex2 <= nIndex1)
                return "";

            if (nIndex1 > nIndex2)
                nIndex = nIndex1 + 1;
            else
                nIndex = nIndex2 + 1;

            string strResult = strSrc.Substring(nIndex1 + strTag1.Length, nIndex2 - nIndex1 - strTag1.Length);
            return strResult;
        }

        // 도로명 주소를 지번주소로 변환한 정보 리스트를 얻어온다.
        private Dictionary<string, string> ReadOldTypeAddress(string strFilePath)
        {
            // Key : 도로명 주소
            // Value : 지번 주소
            Dictionary<string, string> dicOldTypeAddress = new Dictionary<string, string>();

            if (!File.Exists(strFilePath))
                return dicOldTypeAddress;

            // EUC-KR : 51949
            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            StreamReader reader = new StreamReader(strFilePath, encEUC_KR);

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                string[] arrDatas = strLine.Split('\t');

                if (arrDatas.Count() != 2)
                    continue;

                dicOldTypeAddress[arrDatas[0].Trim()] = arrDatas[1].Trim();
            }

            reader.Close();
            return dicOldTypeAddress;
        }

        public void UpdateExcelSheet(string strFilePath, string strSheetName, int nBeginIndex)
        {
            string strAddrCodePath = System.Windows.Forms.Application.StartupPath + "\\도로명주소변환.txt";
            // Key : 도로명 주소
            // Value : 지번 주소
            Dictionary<string, string> dicOldTypeAddress = ReadOldTypeAddress(strAddrCodePath);

            // Excel 프로세스 생성
            Excel.Application app = new Excel.Application();

            // 읽기전용 열기
            Excel.Workbook workBook = app.Workbooks.Open(strFilePath, 0, true, 5, Type.Missing, Type.Missing, false, Type.Missing, Type.Missing, true, false, Type.Missing, false, false, false);

            // sheets 생성
            Excel.Sheets sheets = workBook.Sheets;

            foreach (Excel.Worksheet sheet in sheets)
            {
                if (sheet.Name == strSheetName)
                    UpdateWorkSheet(sheet, nBeginIndex, dicOldTypeAddress);
            }

            CloseWorkBook(workBook);

            CloseExcel(app);

            // EUC-KR : 51949
            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            StreamWriter writer = new StreamWriter(strAddrCodePath, false, encEUC_KR);

            foreach (KeyValuePair<string, string> pair in dicOldTypeAddress)
            {
                writer.WriteLine(pair.Key + "\t" + pair.Value);
            }

            writer.Close();
        }

        private string GetCellString(int nRowIndex, int nColIndex, Excel.Worksheet workSheet)
        {
            object obj = workSheet.UsedRange.Cells[nRowIndex, nColIndex].Value;

            if (obj == null)
                return "";

            return obj.ToString();
        }

        // dicOldTypeAddress
        // Key : 도로명 주소
        // Value : 지번 주소
        private bool UpdateWorkSheet(Excel.Worksheet workSheet, int nBeginDataIndex, Dictionary<string, string> dicOldTypeAddress)
        {
            List<string> resultLines = new List<string>();
            List<int> popupIndeces = new List<int>();

            int nRowCount = workSheet.UsedRange.Rows.Count;

            FormMain.Instance.SetTotalCount(nRowCount - 1);

            // 첫번째 행은 컬럼 제목이니 nBeginDataIndex에 1을 더한다.
            int nBeginRowIndex = nBeginDataIndex + 1;
            
            for (int i = nBeginRowIndex; i <= nRowCount; i++)
            {
                bool stopProgress = false;

                string strGubun = GetCellString(i, 4, workSheet);
                string strOwner = GetCellString(i, 5, workSheet);
                string strYear = GetCellString(i, 6, workSheet);
                string strAddr1 = GetCellString(i, 11, workSheet);
                string strAddr2 = GetCellString(i, 12, workSheet);
                string strAddr3 = GetCellString(i, 13, workSheet);
                string strAddr4 = GetCellString(i, 14, workSheet);
                string strArea = GetCellString(i, 17, workSheet);

                if (strAddr1.Length == 0)
                {
                    FormMain.Instance.SetProgressCount(i - 1);
                    continue;
                }

                string strAddress = strAddr1.Trim() + " " + strAddr2.Trim() + " " + strAddr3.Trim() + " " + strAddr4.Trim();
                strAddress = strAddress.Trim();

                string strOldTypeAddress = "";

                if (!dicOldTypeAddress.TryGetValue(strAddress, out strOldTypeAddress))
                    strOldTypeAddress = strAddress;

                Dictionary<string, string> dicAddrCoords = GetAddressCoord(strOldTypeAddress);
                string strCoord = "";

                if (!dicAddrCoords.TryGetValue(strAddress, out strCoord))
                {
                    strCoord = "";
                    bool goBack = false;
                    int nHistoryCount = popupIndeces.Count;

                    FormMain.Instance.Invoke((System.Windows.Forms.MethodInvoker)delegate
                    {
                        FormSelectLibrary2 frm = new FormSelectLibrary2(strAddress, dicAddrCoords, dicOldTypeAddress);
                        frm.EnableGoBack = nHistoryCount > 0;
                        
                        if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            if (frm.GoBack)
                                goBack = true;
                            else
                            {
                                strCoord = frm.SelectedCoord;

                                if (frm.FinalAddress.Length > 0)
                                    dicOldTypeAddress[strAddress] = frm.FinalAddress;
                            }

                            stopProgress = frm.StopProgress;
                        }
                    });

                    if (goBack)
                    {
                        int nPrevPopupIndex = popupIndeces[nHistoryCount - 1];

                        resultLines.RemoveRange(nPrevPopupIndex - nBeginRowIndex, i - nPrevPopupIndex);
                        popupIndeces.RemoveAt(nHistoryCount - 1);
                        FormMain.Instance.SetProgressCount(nPrevPopupIndex - 2);

                        i = nPrevPopupIndex - 1;
                        continue;
                    }

                    popupIndeces.Add(i);
                }

                if (!stopProgress)
                {
                    string strLine = MakeLibraryLineString(strGubun, strOwner, strYear, strAddr1, strAddr2, strAddr3, strAddr4, strArea, strCoord, workSheet, i);
                    resultLines.Add(strLine);
                    //workSheet.UsedRange.Cells[i, 15].Value = strCoord;
                    //WriteLibrary(writer, strGubun, strOwner, strYear, strAddr1, strAddr2, strAddr3, strAddr4, strArea, strCoord, workSheet, i);
                    FormMain.Instance.SetProgressCount(i - 1);
                }
                else
                    break;
            }

            string strWorkSheetName = workSheet.Name;
            Marshal.ReleaseComObject(workSheet);

            StreamWriter writer = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\" + strWorkSheetName + ".txt");

            foreach (string strLine in resultLines)
            {
                writer.WriteLine(strLine);
            }

            writer.Close();
            
            return true;
        }

        private int GetRadiusCode(string strArea, string strAddr1, string strAddr2, string strGubun)
        {
            int nArea;

            if (!int.TryParse(strArea, out nArea))
                nArea = 0;

            int nRadiusCode = -1;

            if (strAddr1.StartsWith("서울"))
            {
                if (strGubun == "대표")
                    nRadiusCode = 0;
                else if (strGubun == "중앙")
                {
                    if (nArea >= 900)
                        nRadiusCode = 1;
                    else if (nArea >= 600)
                        nRadiusCode = 2;
                    else
                        nRadiusCode = 3;
                }
                else if (strGubun == "분관" || strGubun == "거점")
                {
                    if (nArea >= 900)
                        nRadiusCode = 4;
                    else if (nArea >= 600)
                        nRadiusCode = 5;
                    else
                        nRadiusCode = 6;
                }
                else if (strGubun == "작은")
                    nRadiusCode = 7;
            }
            else if ((strAddr1.StartsWith("부산") || strAddr1.StartsWith("울산") || strAddr1.StartsWith("대구") ||
                strAddr1.StartsWith("대전") || strAddr1.StartsWith("인천") || strAddr1.StartsWith("광주")) &&
                !strAddr2.EndsWith("군"))
            {
                if (strGubun == "대표")
                    nRadiusCode = 8;
                else if (strGubun == "중앙")
                {
                    if (nArea >= 900)
                        nRadiusCode = 9;
                    else if (nArea >= 600)
                        nRadiusCode = 10;
                    else
                        nRadiusCode = 11;
                }
                else if (strGubun == "분관" || strGubun == "거점")
                {
                    if (nArea >= 600)
                        nRadiusCode = 12;
                    else
                        nRadiusCode = 13;
                }
                else if (strGubun == "작은")
                    nRadiusCode = 14;
            }
            else if (strAddr1.StartsWith("세종") || strAddr2.EndsWith("시"))
            {
                if (strGubun == "대표")
                    nRadiusCode = 15;
                else if (strGubun == "중앙")
                {
                    if (nArea >= 900)
                        nRadiusCode = 16;
                    else if (nArea >= 600)
                        nRadiusCode = 17;
                    else
                        nRadiusCode = 18;
                }
                else if (strGubun == "분관" || strGubun == "거점")
                {
                    if (nArea >= 600)
                        nRadiusCode = 19;
                    else
                        nRadiusCode = 20;
                }
                else if (strGubun == "작은")
                    nRadiusCode = 21;
            }
            else
            {
                if (strGubun == "대표")
                    nRadiusCode = 22;
                else if (strGubun == "중앙")
                {
                    if (nArea >= 900)
                        nRadiusCode = 23;
                    else if (nArea >= 600)
                        nRadiusCode = 24;
                    else
                        nRadiusCode = 25;
                }
                else if (strGubun == "분관" || strGubun == "거점")
                {
                    nRadiusCode = 26;
                }
                else if (strGubun == "작은")
                    nRadiusCode = 27;
            }

            return nRadiusCode;
        }

        private string MakeLibraryLineString(string strGubun, string strOwner, string strYear, string strAddr1, string strAddr2, string strAddr3, string strAddr4, string strArea, string strCoord, Excel.Worksheet workSheet, int nRowIndex)
        {
            string strID = GetCellString(nRowIndex, 1, workSheet);
            string strName = GetCellString(nRowIndex, 2, workSheet);
            string strLocation = GetCellString(nRowIndex, 3, workSheet);
            string strHomepage = GetCellString(nRowIndex, 8, workSheet);
            string strPhone = GetCellString(nRowIndex, 9, workSheet);
            string strFax = GetCellString(nRowIndex, 10, workSheet);
            string strBookCount = GetCellString(nRowIndex, 18, workSheet);
            int nRadiusCode = GetRadiusCode(strArea, strAddr1, strAddr2, strGubun);

            if (nRadiusCode < 0)
                return "";

            string strLine = strID.Trim() + "\t";
            strLine += strName.Trim() + "\t";
            strLine += strLocation.Trim() + "\t";
            strLine += strGubun.Trim() + "\t";
            strLine += strOwner.Trim() + "\t";
            strLine += strYear.Trim() + "\t\t";
            strLine += strHomepage.Trim() + "\t";
            strLine += strPhone.Trim() + "\t";
            strLine += strFax.Trim() + "\t";
            strLine += strAddr1.Trim() + "\t";
            strLine += strAddr2.Trim() + "\t";
            strLine += strAddr3.Trim() + "\t";
            strLine += strAddr4.Trim() + "\t";
            strLine += strCoord.Trim() + "\t\t";
            strLine += strArea.Trim() + "\t";
            strLine += strBookCount.Trim() + "\t\t";
            strLine += nRadiusCode.ToString();

            return strLine;
        }

        private void WriteLibrary(StreamWriter writer, string strGubun, string strOwner, string strYear, string strAddr1, string strAddr2, string strAddr3, string strAddr4, string strArea, string strCoord, Excel.Worksheet workSheet, int nRowIndex)
        {
            string strLine = MakeLibraryLineString(strGubun, strOwner, strYear, strAddr1, strAddr2, strAddr3, strAddr4, strArea, strCoord, workSheet, nRowIndex);
            writer.WriteLine(strLine);
            /*string strID = GetCellString(nRowIndex, 1, workSheet);
            string strName = GetCellString(nRowIndex, 2, workSheet);
            string strLocation = GetCellString(nRowIndex, 3, workSheet);
            string strHomepage = GetCellString(nRowIndex, 8, workSheet);
            string strPhone = GetCellString(nRowIndex, 9, workSheet);
            string strFax = GetCellString(nRowIndex, 10, workSheet);
            string strBookCount = GetCellString(nRowIndex, 18, workSheet);
            int nRadiusCode = GetRadiusCode(strArea, strAddr1, strAddr2, strGubun);

            if (nRadiusCode < 0)
                return;

            writer.Write(strID.Trim() + "\t");
            writer.Write(strName.Trim() + "\t");
            writer.Write(strLocation.Trim() + "\t");
            writer.Write(strGubun.Trim() + "\t");
            writer.Write(strOwner.Trim() + "\t");
            writer.Write(strYear.Trim() + "\t\t");
            writer.Write(strHomepage.Trim() + "\t");
            writer.Write(strPhone.Trim() + "\t");
            writer.Write(strFax.Trim() + "\t");
            writer.Write(strAddr1.Trim() + "\t");
            writer.Write(strAddr2.Trim() + "\t");
            writer.Write(strAddr3.Trim() + "\t");
            writer.Write(strAddr4.Trim() + "\t");
            writer.Write(strCoord.Trim() + "\t\t");
            writer.Write(strArea.Trim() + "\t");
            writer.Write(strBookCount.Trim() + "\t\t");
            writer.WriteLine(nRadiusCode.ToString());*/

            writer.Flush();
        }

        // DB 로딩후 지역별 정렬시켜 txt에 출력
        public bool LoadLibraries2()
        {
            string strSQL = "select idx, name, location, gubun, useing, year, zipcode, homepage, tel, fax, addr1, addr2, addr3, addr4, lng, area_count, user_count, grade from lib_list2";
            //string strSQL = "select idx, name, location, gubun, year, homepage, fax from lib_list";
            // EUC-KR : 51949
            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            System.IO.StreamWriter writer = new StreamWriter("f:\\list2.txt", false, encEUC_KR);
            System.IO.StreamWriter writer2= new StreamWriter("f:\\list3.txt", false, encEUC_KR);

            MySqlDataReader reader;
            ReadDB(strSQL, null, out reader);

            Dictionary<string, List<CityLibrary>> dicCityLibraries = new Dictionary<string, List<CityLibrary>>();
            List<string> cities = new List<string>();

            while (reader.Read())
            {
                int nID = (int)reader[0];
                string strName = reader.IsDBNull(1) ? "" : reader[1].ToString().Trim();
                string strLocation = reader.IsDBNull(2) ? "" : reader[2].ToString().Trim();
                string strGubun = reader.IsDBNull(3) ? "" : reader[3].ToString().Trim();
                string strOwner = reader.IsDBNull(4) ? "" : reader[4].ToString().Trim();
                string strYear = reader.IsDBNull(5) ? "" : reader[5].ToString().Trim();
                string strHomepage = reader.IsDBNull(7) ? "" : reader[7].ToString().Trim();
                string strTel = reader.IsDBNull(8) ? "" : reader[8].ToString().Trim();
                string strFax = reader.IsDBNull(9) ? "" : reader[9].ToString().Trim();

                string strAddr1 = reader.IsDBNull(10) ? "" : reader[10].ToString().Trim();
                string strAddr2 = reader.IsDBNull(11) ? "" : reader[11].ToString().Trim();
                string strAddr3 = reader.IsDBNull(12) ? "" : reader[12].ToString().Trim();
                string strAddr4 = reader.IsDBNull(13) ? "" : reader[13].ToString().Trim();
                string strLng = reader.IsDBNull(14) ? "" : reader[14].ToString().Trim();
                string strAreaCount = reader.IsDBNull(15) ? "" : reader[15].ToString().Trim();
                string strBookCount = reader.IsDBNull(16) ? "" : reader[16].ToString().Trim();
                string strRadiusCode = reader.IsDBNull(17) ? "" : reader[17].ToString().Trim();

                Library lib = new Library();

                lib.ID = nID;
                lib.Name = strName;
                lib.Location = strLocation;
                lib.Gubun = strGubun;
                lib.Homepage = strHomepage;
                lib.FaxNumber = strFax;
                lib.Owner = strOwner;
                lib.Year = int.Parse(strYear);
                lib.PhoneNumber = strTel;
                lib.Addr1 = strAddr1;
                lib.Addr2 = strAddr2;
                lib.Addr3 = strAddr3;
                lib.Addr4 = strAddr4;
                lib.Coord = strLng;
                lib.UseCount = strAreaCount;
                lib.UserCount = strBookCount;
                lib.Grade = int.Parse(strRadiusCode);

                List<CityLibrary> libraries = null;

                if (!dicCityLibraries.TryGetValue(lib.Addr1, out libraries))
                {
                    libraries = new List<CityLibrary>();
                    dicCityLibraries[lib.Addr1] = libraries;
                    cities.Add(lib.Addr1);
                }

                CityLibrary city = FindCityLibrary(libraries, lib.Addr2);

                if (city == null)
                {
                    city = new CityLibrary();
                    city.m_strGuName = lib.Addr2;
                    libraries.Add(city);
                }

                GuLibrary gu = new GuLibrary();
                gu.m_lib = lib;
                city.libraries.Add(gu);
            }

            cities.Sort();

            foreach (string strCity in cities)
            {
                List<CityLibrary> libraries = null;

                if (!dicCityLibraries.TryGetValue(strCity, out libraries))
                    break;

                libraries.Sort();

                foreach (CityLibrary city in libraries)
                {
                    city.libraries.Sort();

                    foreach (GuLibrary gu in city.libraries)
                    {
                        Library lib = gu.m_lib;

                        if (lib.Gubun == "작은")
                            writer2.WriteLine(lib.ID.ToString() + "\t" + lib.Name + "\t" + lib.Location + "\t" + lib.Gubun + "\t" + lib.Owner + "\t" + lib.Year.ToString() + "\t\t" + lib.Homepage + "\t" + lib.PhoneNumber + "\t" + lib.FaxNumber + "\t" + lib.Addr1 + "\t" + lib.Addr2 + "\t" + lib.Addr3 + "\t" + lib.Addr4 + "\t" + lib.Coord + "\t\t" + lib.UseCount + "\t" + lib.UserCount + "\t\t" + lib.Grade.ToString());
                        else
                            writer.WriteLine(lib.ID.ToString() + "\t" + lib.Name + "\t" + lib.Location + "\t" + lib.Gubun + "\t" + lib.Owner + "\t" + lib.Year.ToString() + "\t\t" + lib.Homepage + "\t" + lib.PhoneNumber + "\t" + lib.FaxNumber + "\t" + lib.Addr1 + "\t" + lib.Addr2 + "\t" + lib.Addr3 + "\t" + lib.Addr4 + "\t" + lib.Coord + "\t\t" + lib.UseCount + "\t" + lib.UserCount + "\t\t" + lib.Grade.ToString());
                    }
                }
            }

            reader.Close();
            writer.Close();
            writer2.Close();
            return true;
        }

        public bool LoadLibraries()
        {
            string strSQL = "select idx, name, location, gubun, year, homepage, fax, addr1, addr2, addr3, addr4, addr5, addr6, lng from lib_list2";
            //string strSQL = "select idx, name, location, gubun, year, homepage, fax from lib_list";
            // EUC-KR : 51949
            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            System.IO.StreamWriter writer = new StreamWriter("f:\\list2.txt", false, encEUC_KR);

            MySqlDataReader reader;
            ReadDB(strSQL, null, out reader);

            while (reader.Read())
            {
                int nID = (int)reader[0];
                string strName = reader.IsDBNull(1) ? "" : reader[1].ToString().Trim();
                string strLocation = reader.IsDBNull(2) ? "" : reader[2].ToString().Trim();
                string strGubun = reader.IsDBNull(3) ? "" : reader[3].ToString().Trim();
                string strYear = reader.IsDBNull(4) ? "" : reader[4].ToString().Trim();
                string strHomepage = reader.IsDBNull(5) ? "" : reader[5].ToString().Trim();
                string strFax = reader.IsDBNull(6) ? "" : reader[6].ToString().Trim();

                string strAddr1 = reader.IsDBNull(7) ? "" : reader[7].ToString().Trim();
                string strAddr2 = reader.IsDBNull(8) ? "" : reader[8].ToString().Trim();
                string strAddr3 = reader.IsDBNull(9) ? "" : reader[9].ToString().Trim();
                string strAddr4 = reader.IsDBNull(10) ? "" : reader[10].ToString().Trim();
                string strAddr5 = reader.IsDBNull(11) ? "" : reader[11].ToString().Trim();
                string strAddr6 = reader.IsDBNull(12) ? "" : reader[12].ToString().Trim();
                string strLng = reader.IsDBNull(13) ? "" : reader[13].ToString().Trim();

                writer.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}", nID, strName, strLocation, strAddr1, strAddr2, strAddr3, strAddr4, strAddr5, strAddr6, strLng));

                Library lib = new Library();

                lib.ID = nID;
                lib.Name = strName;
                lib.Location = strLocation;
                lib.Gubun = strGubun;
                lib.Homepage = strHomepage;
                lib.FaxNumber = strFax;

                int nYear;
                if (int.TryParse(strYear, out nYear))
                    lib.Year = nYear;

                m_dicOldLibraries[strName] = lib;
            }

            reader.Close();
            writer.Close();
            return true;
        }

        public bool LoadSmallLibraries(string strFolder)
        {
            string[] arrFiles = Directory.GetFiles(strFolder);

            if (arrFiles == null)
                return false;

            // Excel 프로세스 생성
            Excel.Application app = new Excel.Application();
            
            foreach (string strPath in arrFiles)
            {
                int nIndex = strPath.LastIndexOf('\\');
                int nDotIndex = strPath.LastIndexOf('.');

                if (nIndex < 0 || nDotIndex < 0)
                    continue;

                string strExt = strPath.Substring(nDotIndex + 1);
                string strFileName = strPath.Substring(nIndex + 1);

                if (strFileName.StartsWith("~$"))
                    continue;

                if (strExt != "xlsx")
                    continue;

                // 읽기전용 열기
                Excel.Workbook workBook = app.Workbooks.Open(strPath, 0, true, 5, Type.Missing, Type.Missing, false, Type.Missing, Type.Missing, true, false, Type.Missing, false, false, false);

                // sheets 생성
                Excel.Sheets sheets = workBook.Sheets;
                System.Diagnostics.Trace.Write(strFileName + ", ");
                Excel.Worksheet sheet = (Excel.Worksheet)sheets[1];
                ReadSmallWorkSheet(sheet);

                CloseWorkBook(workBook);
            }

            Library lib;
            int nLibIndex = GetLastIndex("lib_list2") + 1;

            foreach (Library lib2 in m_smallLibraries)
            {
                if (lib2.GubunType == Library.LibraryType.UNKNOWN)
                    System.Diagnostics.Trace.WriteLine("Unknown Gubun : " + lib2.Name);
                else if (lib2.OwnType == Library.OwnerType.UNKNOWN)
                    System.Diagnostics.Trace.WriteLine("Unknown Owner : " + lib2.Name);

                if (m_dicOldLibraries.TryGetValue(lib2.Name, out lib))
                {
                    lib2.Homepage = lib.Homepage;
                    lib2.FaxNumber = lib.FaxNumber;
                }

                InsertLibrary(lib2, ref nLibIndex);
            }

            CloseExcel(app);
            return true;
        }

        private int GetLastIndex(string strTableName)
        {
            string strSQL = "select max(idx) from " + strTableName;

            MySqlDataReader reader;
            ReadDB(strSQL, null, out reader);

            int nIndex = 0;

            if (reader.Read())
            {
                nIndex = (int)reader[0];
            }

            reader.Close();
            return nIndex;
        }

        private bool ReadSmallWorkSheet(Excel.Worksheet workSheet)
        {
            int nRowCount = workSheet.UsedRange.Rows.Count;
            Library prevLib = null;
            bool begin = false;

            for (int i = 1; i <= nRowCount; i++)
            {
                object obj = workSheet.UsedRange.Cells[i, 1].Value;

                if (obj == null)
                    continue;

                string strValue = obj.ToString();

                if (strValue.Length == 0)
                    continue;

                if (begin)
                {
                    if (strValue == "연번" && prevLib != null)
                        m_smallLibraries.Remove(prevLib);
                    else
                    {
                        Library lib = ReadSmallLibrary(workSheet, i);

                        if (lib != null)
                            prevLib = lib;
                        else
                            prevLib = null;
                    }
                }
                else
                {
                    if (strValue == "연번")
                        begin = true;
                }
            }

            Marshal.ReleaseComObject(workSheet);
            return true;
        }

        private Library ReadSmallLibrary(Excel.Worksheet workSheet, int nRowIndex)
        {
            string strLocation = GetExcelString(workSheet, nRowIndex, 4).Trim();
            string strOwner = GetExcelString(workSheet, nRowIndex, 6).Trim();
            string strYear = GetExcelString(workSheet, nRowIndex, 8).Trim();
            string strName = GetExcelString(workSheet, nRowIndex, 9).Trim();
            string strArea = GetExcelString(workSheet, nRowIndex, 12).Trim();
            string strPhoneNumber = GetExcelString(workSheet, nRowIndex, 21).Trim();
            string strAddress = GetExcelString(workSheet, nRowIndex, 22).Trim();

            int nYear;

            if (!int.TryParse(strYear, out nYear))
                return null;

            double dArea = 0.0;
            double.TryParse(strArea, out dArea);

            Library lib = new Library();

            lib.Gubun = "작은";
            lib.Location = strLocation;
            lib.Owner = strOwner;
            lib.Year = nYear;
            lib.Name = strName;
            lib.PhoneNumber = strPhoneNumber;
            lib.Address = strAddress;
            lib.Area = (int)dArea;
            lib.GubunType = Library.LibraryType.작은;

            if (strOwner.StartsWith("지자체"))
                lib.OwnType = Library.OwnerType.지자체;
            else if (strOwner.StartsWith("교육청"))
                lib.OwnType = Library.OwnerType.교육청;
            else if (strOwner.StartsWith("사립"))
                lib.OwnType = Library.OwnerType.사립;
            else if (strOwner.Length == 0)
                lib.OwnType = Library.OwnerType.지자체;

            if (nYear >= 2015)
                lib.OwnType = Library.OwnerType.건립중;

            m_smallLibraries.Add(lib);
            return lib;
        }

        public bool LoadPublicLibraries2(string strFolder, Dictionary<string, List<Library>> dicCityLibraries)
        {
            string[] arrFiles = Directory.GetFiles(strFolder);

            if (arrFiles == null)
                return false;

            // Excel 프로세스 생성
            Excel.Application app = new Excel.Application();

            foreach (string strPath in arrFiles)
            {
                int nIndex = strPath.LastIndexOf('\\');
                int nDotIndex = strPath.LastIndexOf('.');

                if (nIndex < 0 || nDotIndex < 0)
                    continue;

                string strExt = strPath.Substring(nDotIndex + 1);
                string strFileName = strPath.Substring(nIndex + 1);

                if (strFileName.StartsWith("~$"))
                    continue;

                if (strExt != "xlsx")
                    continue;

                // 읽기전용 열기
                Excel.Workbook workBook = app.Workbooks.Open(strPath, 0, true, 5, Type.Missing, Type.Missing, false, Type.Missing, Type.Missing, true, false, Type.Missing, false, false, false);

                // sheets 생성
                Excel.Sheets sheets = workBook.Sheets;
                System.Diagnostics.Trace.Write(strFileName + ", ");
                Excel.Worksheet sheet = (Excel.Worksheet)sheets[1];
                ReadPublicWorkSheet2(sheet, strPath.Substring(nIndex + 1, nDotIndex - nIndex - 1), dicCityLibraries);

                CloseWorkBook(workBook);
            }

            CloseExcel(app);
            return true;
        }

        public bool LoadSmallLibraries2(string strFolder, Dictionary<string, List<Library>> dicCityLibraries)
        {
            string[] arrFiles = Directory.GetFiles(strFolder);

            if (arrFiles == null)
                return false;

            // Excel 프로세스 생성
            Excel.Application app = new Excel.Application();

            foreach (string strPath in arrFiles)
            {
                int nIndex = strPath.LastIndexOf('\\');
                int nDotIndex = strPath.LastIndexOf('.');

                if (nIndex < 0 || nDotIndex < 0)
                    continue;

                string strExt = strPath.Substring(nDotIndex + 1);
                string strFileName = strPath.Substring(nIndex + 1);

                if (strFileName.StartsWith("~$"))
                    continue;

                if (strExt != "xlsx")
                    continue;

                // 읽기전용 열기
                Excel.Workbook workBook = app.Workbooks.Open(strPath, 0, true, 5, Type.Missing, Type.Missing, false, Type.Missing, Type.Missing, true, false, Type.Missing, false, false, false);

                // sheets 생성
                Excel.Sheets sheets = workBook.Sheets;
                System.Diagnostics.Trace.Write(strFileName + ", ");
                Excel.Worksheet sheet = (Excel.Worksheet)sheets[1];
                ReadSmallWorkSheet2(sheet, strPath.Substring(nIndex + 1, nDotIndex - nIndex - 1), dicCityLibraries);

                CloseWorkBook(workBook);
            }

            CloseExcel(app);
            return true;
        }

        public bool LoadPublicLibraries(string strFolder)
        {
            string[] arrFiles = Directory.GetFiles(strFolder);

            if (arrFiles == null)
                return false;

            // Excel 프로세스 생성
            Excel.Application app = new Excel.Application();
            Dictionary<string, List<Library>> dicCityLibraries = new Dictionary<string, List<Library>>();

            foreach (string strPath in arrFiles)
            {
                int nIndex = strPath.LastIndexOf('\\');
                int nDotIndex = strPath.LastIndexOf('.');

                if (nIndex < 0 || nDotIndex < 0)
                    continue;

                string strExt = strPath.Substring(nDotIndex + 1);
                string strFileName = strPath.Substring(nIndex + 1);

                if (strFileName.StartsWith("~$"))
                    continue;

                if (strExt != "xlsx")
                    continue;

                // 읽기전용 열기
                Excel.Workbook workBook = app.Workbooks.Open(strPath, 0, true, 5, Type.Missing, Type.Missing, false, Type.Missing, Type.Missing, true, false, Type.Missing, false, false, false);

                // sheets 생성
                Excel.Sheets sheets = workBook.Sheets;
                System.Diagnostics.Trace.Write(strFileName + ", ");
                Excel.Worksheet sheet = (Excel.Worksheet)sheets[1];
                ReadPublicWorkSheet(sheet, strPath.Substring(nIndex + 1, nDotIndex - nIndex - 1), dicCityLibraries);

                CloseWorkBook(workBook);
            }

            SetGrade(dicCityLibraries);

            Library lib;
            int nLibIndex = 1;

            foreach (KeyValuePair<string, List<Library>> pair in dicCityLibraries)
            {
                foreach (Library lib2 in pair.Value)
                {
                    m_dicGubuns[lib2.Gubun] = lib2;
                    m_dicOwners[lib2.Owner] = lib2;

                    if (lib2.GubunType == Library.LibraryType.UNKNOWN)
                        System.Diagnostics.Trace.WriteLine("Unknown Gubun : " + lib2.Name);
                    else if (lib2.OwnType == Library.OwnerType.UNKNOWN)
                        System.Diagnostics.Trace.WriteLine("Unknown Owner : " + lib2.Name);

                    if (m_dicOldLibraries.TryGetValue(lib2.Name, out lib))
                    {
                        lib2.Homepage = lib.Homepage;
                        lib2.FaxNumber = lib.FaxNumber;
                    }

                    InsertLibrary(lib2, ref nLibIndex);
                }
            }

            CloseExcel(app);
            return true;
        }

        private void SetGrade(Dictionary<string, List<Library>> dicLibraries)
        {
            foreach (KeyValuePair<string, List<Library>> pair in dicLibraries)
            {
                if (pair.Key == "서울")
                {
                    foreach (Library lib in pair.Value)
                    {
                        if (lib.GubunType == Library.LibraryType.중앙)
                            lib.Grade = 1;
                        else if (lib.GubunType == Library.LibraryType.거점)
                            lib.Grade = 2;
                        else if (lib.GubunType == Library.LibraryType.분관)
                        {
                            if (lib.Area >= 900)
                                lib.Grade = 3;
                            else if (lib.Area >= 600)
                                lib.Grade = 4;
                            else
                                lib.Grade = 5;
                        }
                    }
                }
                else if (pair.Key == "광주" || pair.Key == "대구" || pair.Key == "대전"
                    || pair.Key == "부산" || pair.Key == "세종" || pair.Key == "울산"
                    || pair.Key == "인천")
                {
                    foreach (Library lib in pair.Value)
                    {
                        if (lib.GubunType == Library.LibraryType.중앙)
                            lib.Grade = 6;
                        else if (lib.GubunType == Library.LibraryType.거점)
                            lib.Grade = 7;
                        else if (lib.GubunType == Library.LibraryType.분관)
                        {
                            if (lib.Area >= 900)
                                lib.Grade = 8;
                            else if (lib.Area >= 600)
                                lib.Grade = 9;
                            else
                                lib.Grade = 10;
                        }
                    }
                }
                else
                {
                    foreach (Library lib in pair.Value)
                    {
                        if (lib.GubunType == Library.LibraryType.중앙)
                            lib.Grade = 11;
                        else if (lib.GubunType == Library.LibraryType.거점)
                            lib.Grade = 12;
                        else if (lib.GubunType == Library.LibraryType.분관)
                        {
                            if (lib.Area >= 900)
                                lib.Grade = 13;
                            else if (lib.Area >= 600)
                                lib.Grade = 14;
                            else
                                lib.Grade = 15;
                        }
                    }
                }
            }
        }

        private bool InsertLibrary(Library lib, ref int nLibIndex)
        {
            int nIndex = lib.Name.IndexOf('\r');

            if (nIndex >= 0)
                lib.Name = lib.Name.Remove(nIndex, 1);

            nIndex = lib.Name.IndexOf('\n');

            if (nIndex >= 0)
                lib.Name = lib.Name.Remove(nIndex, 1);

            string strFormat = "Insert into lib_list2 (idx, name, location, gubun, useing, year, zipcode, homepage, ";
            strFormat += "tel, fax, addr1, addr2, addr3, addr4, addr5, addr6, address1, address2, lng, tm, area_count, ";
            strFormat += "user_count, use_count, grade) values ({0}, '{1}', '{2}', '{3}', '{4}', '{5}', NULL, '{6}', ";
            strFormat += "'{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', NULL, NULL, NULL, NULL, ";
            strFormat += "'{15}', '{16}', NULL, {17})";

            string[] strAddr = new string[6] { "", "", "", "", "", "" };

            if (lib.Address.Length > 0)
            {
                string[] arrAddress = lib.Address.Split(' ');

                int nAddrCount = arrAddress.Count();

                for (int i = 0; i < nAddrCount; i++)
                {
                    if (i == 5 && i < nAddrCount - 1)
                    {
                        for (int j = i; j < nAddrCount; j++)
                        {
                            if (strAddr[i].Length == 0)
                                strAddr[i] = arrAddress[j];
                            else
                                strAddr[i] += " " + arrAddress[j];
                        }

                        break;
                    }
                    else
                        strAddr[i] = arrAddress[i];
                }
            }
            else
            {
                strAddr[0] = lib.Addr1;
                strAddr[1] = lib.Addr2;
                strAddr[2] = lib.Addr3;
                strAddr[3] = lib.Addr4;
            }

            string strSQL = string.Format(strFormat, nLibIndex, lib.Name, lib.Location, lib.GubunType.ToString(),
                lib.OwnType.ToString(), lib.Year.ToString(), lib.Homepage, lib.PhoneNumber, lib.FaxNumber,
                strAddr[0], strAddr[1], strAddr[2], strAddr[3], strAddr[4], strAddr[5],
                lib.Area.ToString(), lib.UserCount, lib.Grade);

            try
            {
                Execute(strSQL);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                return false;
            }

            nLibIndex++;
            return true;
        }

        public static void CloseWorkBook(Excel.Workbook workBook)
        {
            if (workBook != null)
            {
                Marshal.ReleaseComObject(workBook.Sheets);

                workBook.Close(false);
                Marshal.ReleaseComObject(workBook);
                workBook = null;
            }
        }

        public static void CloseExcel(Excel.Application app)
        {
            if (app != null)
            {
                Marshal.ReleaseComObject(app.Workbooks);
                app.Application.Quit();
                app.Quit();
                Marshal.ReleaseComObject(app);

                app = null;

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private int[] GetPublicIndeces(Excel.Worksheet workSheet, int nRowIndex)
        {
            int nColumnCount = workSheet.UsedRange.Columns.Count;
            int[] arrIndeces = new int[INDEX_COUNT];

            for (int i = 0; i < INDEX_COUNT;i++ )
            {
                arrIndeces[i] = -1;
            }

            for (int i = 1; i <= nColumnCount; i++)
            {
                object obj = workSheet.UsedRange.Cells[nRowIndex, i].Value;

                if (obj == null)
                    continue;

                string strType = obj.ToString().Trim();

                if (strType == "구분2")
                    arrIndeces[GUBUN] = i;
                else if (strType.StartsWith("지역"))
                    arrIndeces[LOCATION] = i;
                else if (strType.StartsWith("설립주체"))
                    arrIndeces[OWNER] = i;
                else if (strType.StartsWith("개관년도"))
                    arrIndeces[YEAR] = i;
                else if (strType.StartsWith("도서관명"))
                    arrIndeces[NAME] = i;
                else if (strType.StartsWith("도서자료"))
                    arrIndeces[BOOK_COUNT] = i;
                else if (strType.StartsWith("도서관 연면적") || strType.StartsWith("도서관연면적"))
                    arrIndeces[AREA] = i;
                else if (strType.StartsWith("주소"))
                    arrIndeces[ADDRESS] = i;
                else if (strType.StartsWith("전화번호"))
                    arrIndeces[PHONE_NUMBER] = i;
            }

            for (int i = 0; i < INDEX_COUNT; i++)
            {
                if (i == PHONE_NUMBER)
                    continue;

                if (arrIndeces[i] < 0)
                    return null;
            }

            return arrIndeces;
        }

        private int[] GetSmallIndeces(Excel.Worksheet workSheet, int nRowIndex)
        {
            int nColumnCount = workSheet.UsedRange.Columns.Count;
            int[] arrIndeces = new int[INDEX_COUNT];

            for (int i = 0; i < INDEX_COUNT; i++)
            {
                arrIndeces[i] = -1;
            }

            for (int i = 1; i <= nColumnCount; i++)
            {
                object obj = workSheet.UsedRange.Cells[nRowIndex, i].Value;

                if (obj == null)
                    continue;

                string strType = obj.ToString().Trim();

                if (strType.StartsWith("지역"))
                    arrIndeces[LOCATION] = i;
                else if (strType.StartsWith("설립주체"))
                    arrIndeces[OWNER] = i;
                else if (strType.StartsWith("개관년도"))
                    arrIndeces[YEAR] = i;
                else if (strType.StartsWith("도서관명"))
                    arrIndeces[NAME] = i;
                else if (strType.StartsWith("도서자료"))
                    arrIndeces[BOOK_COUNT] = i;
                else if (strType.StartsWith("주소"))
                    arrIndeces[ADDRESS] = i;
                else if (strType.StartsWith("전화번호"))
                    arrIndeces[PHONE_NUMBER] = i;
            }

            for (int i = 0; i < INDEX_COUNT; i++)
            {
                if (i == GUBUN || i == AREA)
                    continue;

                if (arrIndeces[i] < 0)
                    return null;
            }

            return arrIndeces;
        }

        private Library ReadLibrary2(Excel.Worksheet workSheet, int nRowIndex, int[] arrIndeces)
        {
            Library lib = new Library();

            for (int i=0;i<INDEX_COUNT;i++)
            {
                if (arrIndeces[i] < 0)
                    continue;

                object obj = workSheet.UsedRange.Cells[nRowIndex, arrIndeces[i]].Value;
                string strData = null;

                if (obj == null)
                {
                    if (i == ADDRESS)
                        return null;
                    else
                        strData = "";
                }
                else
                    strData = obj.ToString().Trim();

                if (i == GUBUN)
                {
                    if (strData.Length == 0)
                        return null;

                    lib.Gubun = strData;

                    if (strData.StartsWith("중"))
                        lib.GubunType = Library.LibraryType.중앙;
                    else if (strData.StartsWith("대"))
                        lib.GubunType = Library.LibraryType.대표;
                    else if (strData.StartsWith("거"))
                        lib.GubunType = Library.LibraryType.거점;
                    else if (strData.StartsWith("분") || strData.StartsWith("븐") || strData.StartsWith("붖"))
                        lib.GubunType = Library.LibraryType.분관;
                    else if (strData.StartsWith("교"))
                    {
                        lib.GubunType = Library.LibraryType.거점;
                        //lib.GubunType = Library.LibraryType.중앙;
                    }
                }
                else if (i == LOCATION)
                {
                    if (strData.StartsWith("지역"))
                        return null;

                    lib.Location = strData;
                }
                else if (i == OWNER)
                {
                    lib.Owner = strData;

                    if (strData.StartsWith("지자체"))
                        lib.OwnType = Library.OwnerType.지자체;
                    else if (strData.StartsWith("교육청"))
                        lib.OwnType = Library.OwnerType.교육청;
                    else if (strData.StartsWith("사립"))
                    {
                        lib.OwnType = Library.OwnerType.지자체;
                        //lib.OwnType = Library.OwnerType.사립;
                    }
                    else// if (strData.Length == 0)
                        lib.OwnType = Library.OwnerType.지자체;
                }
                else if (i == YEAR)
                {
                    int nYear;

                    if (!int.TryParse(strData, out nYear))
                        lib.Year = 9999;
                    else
                        lib.Year = nYear;
                }
                else if (i == NAME)
                    lib.Name = strData;
                else if (i == BOOK_COUNT)
                    lib.UserCount = strData;
                else if (i == AREA)
                {
                    double dArea = 0.0;
                    double.TryParse(strData, out dArea);
                    lib.Area = (int)dArea;
                }
                else if (i == ADDRESS)
                {
                    lib.Address = strData;

                    string[] arrTokens = strData.Split(' ');
                    lib.Addr1 = arrTokens[0];

                    if (arrTokens.Count() > 1)
                        lib.Addr2 = arrTokens[1];
                }
                else if (i == PHONE_NUMBER)
                    lib.PhoneNumber = strData;
            }

            return lib;
        }

        private bool ReadSmallWorkSheet2(Excel.Worksheet workSheet, string strCityName, Dictionary<string, List<Library>> dicCityLibraries)
        {
            int nRowCount = workSheet.UsedRange.Rows.Count;
            
            int[] arrIndeces = null;
            List<Library> libraries = new List<Library>();

            for (int i = 1; i <= nRowCount; i++)
            {
                if (arrIndeces == null)
                {
                    arrIndeces = GetSmallIndeces(workSheet, i);
                }
                else
                {
                    Library lib = ReadLibrary2(workSheet, i, arrIndeces);

                    if (lib != null)
                    {
                        lib.GubunType = Library.LibraryType.작은;
                        libraries.Add(lib);
                    }
                }
            }

            dicCityLibraries[strCityName] = libraries;
            Marshal.ReleaseComObject(workSheet);
            return true;
        }

        private bool ReadPublicWorkSheet2(Excel.Worksheet workSheet, string strCityName, Dictionary<string, List<Library>> dicCityLibraries)
        {
            int nRowCount = workSheet.UsedRange.Rows.Count;
            //Library prevLib = null;
            //bool begin = false;

            int[] arrIndeces = null;
            List<Library> libraries = new List<Library>();

            for (int i = 1; i <= nRowCount; i++)
            {
                if (arrIndeces == null)
                {
                    arrIndeces = GetPublicIndeces(workSheet, i);
                }
                else
                {
                    Library lib = ReadLibrary2(workSheet, i, arrIndeces);

                    if (lib != null)
                        libraries.Add(lib);
                }
            }

            dicCityLibraries[strCityName] = libraries;
            Marshal.ReleaseComObject(workSheet);
            return true;
        }

        private bool ReadPublicWorkSheet(Excel.Worksheet workSheet, string strCityName, Dictionary<string, List<Library>> dicCityLibraries)
        {
            //System.Diagnostics.Trace.WriteLine(workSheet.UsedRange.Rows.Count.ToString());
            int nRowCount = workSheet.UsedRange.Rows.Count;
            Library prevLib = null;
            bool begin = false;

            List<Library> libraries = new List<Library>();

            for (int i = 1; i <= nRowCount;i++ )
            {
                object obj = workSheet.UsedRange.Cells[i, 1].Value;

                if (obj == null)
                    continue;

                string strValue = obj.ToString();

                if (strValue.Length == 0)
                    continue;

                if (begin)
                {
                    if (strValue == "구분1" && prevLib != null)
                        libraries.Remove(prevLib);
                    else
                    {
                        Library lib = ReadPublicLibrary(workSheet, i);

                        if (lib != null)
                        {
                            prevLib = lib;
                            libraries.Add(lib);
                        }
                        else
                            prevLib = null;
                    }
                }
                else
                {
                    if (strValue == "구분1")
                        begin = true;
                }
            }

            dicCityLibraries[strCityName] = libraries;
            Marshal.ReleaseComObject(workSheet);
            return true;
        }

        private Library ReadPublicLibrary(Excel.Worksheet workSheet, int nRowIndex)
        {
            string strGubun = GetExcelString(workSheet, nRowIndex, 1).Trim();
            string strLocation = GetExcelString(workSheet, nRowIndex, 3).Trim();
            string strOwner = GetExcelString(workSheet, nRowIndex, 5).Trim();
            string strYear = GetExcelString(workSheet, nRowIndex, 7).Trim();
            string strName = GetExcelString(workSheet, nRowIndex, 8).Trim();
            string strArea = GetExcelString(workSheet, nRowIndex, 14).Trim();
            string strPhoneNumber = GetExcelString(workSheet, nRowIndex, 21).Trim();
            string strAddress = GetExcelString(workSheet, nRowIndex, 22).Trim();

            int nYear;

            if (!int.TryParse(strYear, out nYear))
                return null;

            if (strGubun.StartsWith("작"))
                return null;

            double dArea = 0.0;
            double.TryParse(strArea, out dArea);

            Library lib = new Library();

            lib.Gubun = strGubun;
            lib.Location = strLocation;
            lib.Owner = strOwner;
            lib.Year = nYear;
            lib.Name = strName;
            lib.PhoneNumber = strPhoneNumber;
            lib.Address = strAddress;
            lib.Area = (int)dArea;

            if (strGubun.StartsWith("중"))
                lib.GubunType = Library.LibraryType.중앙;
            else if (strGubun.StartsWith("대"))
                lib.GubunType = Library.LibraryType.대표;
            else if (strGubun.StartsWith("거"))
                lib.GubunType = Library.LibraryType.거점;
            else if (strGubun.StartsWith("분") || strGubun.StartsWith("븐") || strGubun.StartsWith("붖"))
                lib.GubunType = Library.LibraryType.분관;
            else if (strGubun.StartsWith("교"))
                lib.GubunType = Library.LibraryType.중앙;

            if (strOwner.StartsWith("지자체"))
                lib.OwnType = Library.OwnerType.지자체;
            else if (strOwner.StartsWith("교육청"))
                lib.OwnType = Library.OwnerType.교육청;
            else if (strOwner.StartsWith("사립"))
                lib.OwnType = Library.OwnerType.사립;
            else// if (strOwner.Length == 0)
                lib.OwnType = Library.OwnerType.지자체;

            if (nYear >= 2015)
                lib.OwnType = Library.OwnerType.건립중;

            return lib;
        }

        private string GetExcelString(Excel.Worksheet workSheet, int nRowIndex, int nColumnIndex)
        {
            object obj = workSheet.UsedRange.Cells[nRowIndex, nColumnIndex].Value;

            if (obj == null)
                return "";

            return obj.ToString();
        }

        public void CompareDatas2(string strPath)
        {
            Excel.Application app = new Excel.Application();

            // 읽기전용 열기
            Excel.Workbook workBook = app.Workbooks.Open(strPath, 0, true, 5, Type.Missing, Type.Missing, false, Type.Missing, Type.Missing, true, false, Type.Missing, false, false, false);

            // sheets 생성
            Excel.Sheets sheets = workBook.Sheets;
            Excel.Worksheet sheet = (Excel.Worksheet)sheets[2];
            ReadAddrWorkSheet2(sheet, 11);

            CloseWorkBook(workBook);

            CloseExcel(app);
        }

        public void CompareDatas(string strPath1, string strPath2)
        {
            Excel.Application app = new Excel.Application();

            Dictionary<int, string> dicAddr1 = new Dictionary<int, string>();
            Dictionary<int, string> dicAddr2 = new Dictionary<int, string>();

            // 읽기전용 열기
            Excel.Workbook workBook = app.Workbooks.Open(strPath1, 0, true, 5, Type.Missing, Type.Missing, false, Type.Missing, Type.Missing, true, false, Type.Missing, false, false, false);

            // sheets 생성
            Excel.Sheets sheets = workBook.Sheets;
            Excel.Worksheet sheet = (Excel.Worksheet)sheets[1];
            ReadAddrWorkSheet(sheet, dicAddr1, 4);

            CloseWorkBook(workBook);

            // 읽기전용 열기
            Excel.Workbook workBook2 = app.Workbooks.Open(strPath2, 0, true, 5, Type.Missing, Type.Missing, false, Type.Missing, Type.Missing, true, false, Type.Missing, false, false, false);

            // sheets 생성
            Excel.Sheets sheets2 = workBook2.Sheets;
            Excel.Worksheet sheet2 = (Excel.Worksheet)sheets2[2];
            ReadAddrWorkSheet(sheet2, dicAddr2, 11);
            
            CloseWorkBook(workBook2);

            CloseExcel(app);

            foreach (KeyValuePair<int, string> pair in dicAddr1)
            {
                if (!dicAddr2.ContainsKey(pair.Key))
                    continue;

                string addr1 = pair.Value.Trim();
                string addr2 = dicAddr2[pair.Key].Trim();

                string[] arrTokens1 = addr1.Split(' ');
                string[] arrTokens2 = addr2.Split(' ');

                string strAddr1 = GetLastAddrFactor(arrTokens1);
                string strAddr2 = GetLastAddrFactor(arrTokens2);

                if (strAddr1 != strAddr2)
                    continue;
            }
        }

        private string GetLastAddrFactor(string[] arrTokens)
        {
            int nTokenCount = arrTokens.Length;

            if (nTokenCount == 1)
                return arrTokens[0];

            if (arrTokens[nTokenCount - 1] == "번지")
                return arrTokens[nTokenCount - 2];

            if (arrTokens[nTokenCount - 1].EndsWith("번지"))
            {
                int len1 = arrTokens[nTokenCount - 1].Length;
                int len2 = "번지".Length;
                return arrTokens[nTokenCount - 1].Substring(0, len1 - len2);
            }

            return arrTokens[nTokenCount - 1];
        }

        private bool ReadAddrWorkSheet(Excel.Worksheet workSheet, Dictionary<int, string> dicAddr, int nAddrIndex)
        {
            int nRowCount = workSheet.UsedRange.Rows.Count;
            
            for (int i = 1; i <= nRowCount; i++)
            {
                object obj = workSheet.UsedRange.Cells[i, 1].Value;

                if (obj == null)
                    continue;

                string strID = obj.ToString();

                if (strID.Length == 0)
                    continue;

                int nID;
                if (!int.TryParse(strID, out nID))
                    continue;

                object addr1 = workSheet.UsedRange.Cells[i, nAddrIndex].Value;
                object addr2 = workSheet.UsedRange.Cells[i, nAddrIndex + 1].Value;
                object addr3 = workSheet.UsedRange.Cells[i, nAddrIndex + 2].Value;
                object addr4 = workSheet.UsedRange.Cells[i, nAddrIndex + 3].Value;

                string strAddr = addr1.ToString();

                if (addr2 != null && addr2.ToString().Length > 0)
                {
                    strAddr += " " + addr2.ToString();

                    if (addr3 != null && addr3.ToString().Length > 0)
                    {
                        strAddr += " " + addr3.ToString();

                        if (addr4 != null && addr4.ToString().Length > 0)
                        {
                            strAddr += " " + addr4.ToString();
                        }
                    }
                }

                dicAddr[nID] = strAddr;
            }

            return true;
        }

        private bool ReadAddrWorkSheet2(Excel.Worksheet workSheet, int nAddrIndex)
        {
            int nRowCount = workSheet.UsedRange.Rows.Count;

            for (int i = 1; i <= nRowCount; i++)
            {
                object obj = workSheet.UsedRange.Cells[i, 1].Value;

                if (obj == null)
                    continue;

                string strID = obj.ToString();

                if (strID.Length == 0)
                    continue;

                int nID;
                if (!int.TryParse(strID, out nID))
                    continue;

                object addr1 = workSheet.UsedRange.Cells[i, 3].Value;
                object addr2 = workSheet.UsedRange.Cells[i, nAddrIndex + 1].Value;
                object addr3 = workSheet.UsedRange.Cells[i, nAddrIndex + 2].Value;

                string strAddr1 = addr1 == null ? "" : addr1.ToString().Trim();
                string strAddr2 = addr2 == null ? "" : addr2.ToString().Trim();
                string strAddr3 = addr3 == null ? "" : addr3.ToString().Trim();

                if (strAddr1 != strAddr2)
                    continue;
            }

            return true;
        }
    }
}
