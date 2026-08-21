using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using HmlReport.Columns;
using UnE.Sensor;

namespace HmlReport
{
    public enum ReportType { NONE = -1, ParetoSensor = 0, ParetoEquipZone = 10, Detect = 1, NotOperation = 2, Action = 4, Bulletin = 5, BulletinDetail = 6}
    public class CreateHml
    {
        private ReportType m_reportType = ReportType.NONE;
        private Dictionary<ReportType, ArrayList> m_dicData = new Dictionary<ReportType, ArrayList>();

        private string m_strSavePath = "";
        private string m_strPath = "";
        //private string m_strType = "화재";
        private IFacility.FacilityType m_facilityType = IFacility.FacilityType.FIRE_SENSOR;
        private string m_strTitle = "";
        private string m_strDate = "";
        private string m_strTarget = "";
        private string m_strTarget2 = ""; // 대응이력 사용
        private string m_strMemo = ""; // 대응이력 사용
        private string m_strLogo = "";
        private int m_nSiteID = 1;

        private int m_nDataMaxCount = 10000; // 한 문서에 작성할 데이터 수
        private int m_nDataMaxCount2 = 10000; // 한 문서에 작성할 데이터 수 탐지분석만 사용
        private int m_nDataMaxCount3 = 10000; // 한 문서에 작성할 데이터 수 누출탐지분석만 사용
        private int m_nDataMaxCount4 = 10000; // 한 문서에 작성할 데이터 수 누출탐지분석만 사용
        private int m_nDataIndex = 0;
        private bool m_nDataMaxIndex = false;

        private string m_strSopName = ""; // SOP명
        private string m_strProcManager = ""; // 진행총괄
        private string m_strLocation = ""; // 상황발생 위치
        private string m_strTimeRequired = ""; // 소요시간
        private string m_strEndState = ""; // 최종상태

        private WebDBManager m_dbMgr = null;

        public CreateHml(int reportTypeID, int facilityType, string filePath, string logoFileName, int siteID)
        {
            this.m_reportType = (ReportType)reportTypeID;
            this.m_facilityType = IFacility.ToFacilityType(facilityType);
            this.m_strSavePath = filePath.Replace("\\\\", "\\");
            this.m_strPath = Directory.GetCurrentDirectory();// @"D:\test\방범탐지이력";            
            this.m_strLogo = logoFileName;
            this.m_nSiteID = siteID;

            //if (m_reportType != ReportType.Bulletin)
            {
                m_dbMgr = new WebDBManager(siteID);
                bool suc = LoadDefineColumn();
                if (!suc)
                {
                    Console.WriteLine("컬럼 정의 안됨");
                    Environment.Exit(1);
                }
            }

            if (m_reportType == ReportType.Bulletin)
            {
                GetInfoBulletin();
                GetDataBulletin();

                Writer();
                WriteResultFile(1);
            }
            else
            {
                GetInfo();
                GetData();
                
                if (m_reportType == ReportType.ParetoSensor)
                {
                    if (m_dicData.ContainsKey(ReportType.ParetoEquipZone))
                    {
                        if (m_nDataMaxCount2 > m_dicData[ReportType.ParetoEquipZone].Count / m_dicDefineColumns[ReportType.ParetoEquipZone].Count)
                            m_nDataMaxCount2 = m_dicData[ReportType.ParetoEquipZone].Count / m_dicDefineColumns[ReportType.ParetoEquipZone].Count; 
                    }
                }

                int hmlCount = 0;
                if (!m_dicData.ContainsKey(m_reportType))
                {
                    hmlCount = 0;
                    m_dicData.Add(m_reportType, new ArrayList());
                }
                else
                {
                    int val = m_dicData[m_reportType].Count / m_dicDefineColumns[m_reportType].Count;
                    hmlCount = (val / m_nDataMaxCount) + ((val % m_nDataMaxCount > 0) ? 1 : 0);

                    if (m_nDataMaxCount > val)
                        m_nDataMaxCount = val;
                }

                if (hmlCount > 1)
                    m_nDataMaxIndex = true;
                if (hmlCount == 0) // 데이터 없을때
                    hmlCount = 1; 
                for (int i = 0; i < hmlCount; i++)
                {
                    m_nDataIndex = i;
                    Writer();
                } 
            }
        }

        private void WriteResultFile(int nResult)
        {
            StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + "\\report\\BulletinResult.txt");
            writer.Write(nResult);
            writer.Close();
        }

        private void GetInfo()
        {
            string strType = IFacility.GetFacilityTypeString(m_facilityType);
            if (m_facilityType == IFacility.FacilityType.PSM_SENSOR && m_nSiteID == 201)
                strType = "가스";

            if (m_reportType == ReportType.ParetoSensor)
                m_strTitle = strType + " 탐지 분석 보고서";
            else if (m_reportType == ReportType.Detect)
                m_strTitle = strType + " 탐지 이력 보고서";
            else if (m_reportType == ReportType.NotOperation)
                m_strTitle = strType + " 처리 이력 보고서";
            else if (m_reportType == ReportType.Action)
                m_strTitle = strType + " 대응 이력 보고서";

            using (StreamReader sr = new StreamReader(m_strPath + @"\report\SaveDateTime.txt"))
            {
                int nIndex = 0;
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (m_reportType == ReportType.Action)
                    {
                        if (m_facilityType == IFacility.FacilityType.PSM_SENSOR)
                        {
                            if (nIndex == 0)
                                m_strDate = line;
                            else if (nIndex == 1)
                                m_strTarget = line;
                            else if (nIndex == 2)
                                m_strTarget2 = line;
                        }
                        else
                        {
                            if (nIndex == 1)
                                m_strDate = line;
                            else if (nIndex == 2)
                                m_strTarget = line;
                            else if (nIndex == 3)
                                m_strTarget2 = line;
                        }
                    }
                    else
                    {
                        if (nIndex == 0)
                            m_strDate = line;
                        else if (nIndex == 1)
                            m_strTarget = line;
                    }

                    nIndex++;
                }
            }

            if (m_reportType == ReportType.Action)
            {
                using (StreamReader sr = new StreamReader(m_strPath + @"\report\SaveMemo.txt"))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        m_strMemo += line;
                    }
                }
            }
        }

        private void GetInfoBulletin()
        {
            m_strTitle = "SOP 상황판";

            using (StreamReader sr = new StreamReader(m_strPath + @"\report\BulletHwpData.txt"))
            {
                int index = 0;
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (index == 0)
                        m_strSopName = line;
                    if (index == 1)
                        m_strProcManager = line;
                    if (index == 2)
                        m_strLocation = line;
                    if (index == 3)
                        m_strDate = line;
                    if (index == 4)
                        m_strTimeRequired = line;
                    if (index == 5)
                        m_strEndState = line;

                    index++;
                }
            }
        }

        private void GetData()
        {
            using (StreamReader sr = new StreamReader(m_strPath + @"\report\SaveData.txt"))
            {
                string line;
                ReportType curKey = m_reportType;

                if (this.m_reportType == ReportType.ParetoSensor) // 탐지분석은 센서별, 위치별 두가지 형식을 제공한다.
                {                     
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.StartsWith("[") && line.EndsWith("]"))
                        {
                            string temp = line.Replace("[", "").Replace("]", "");
                            curKey = (ReportType)Enum.Parse(typeof(ReportType), temp);
                            if (!m_dicData.ContainsKey(curKey))
                                m_dicData.Add(curKey, new ArrayList());
                            continue;
                        }

                        m_dicData[curKey].Add(line);
                    }
                }
                else
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!m_dicData.ContainsKey(curKey))
                            m_dicData.Add(curKey, new ArrayList());

                        m_dicData[curKey].Add(line);
                    }
                }
            }
        }

        private void GetDataBulletin()
        {
            using (StreamReader sr = new StreamReader(m_strPath + @"\report\BulletHwpAllData.txt"))
            {
                string line;
                ReportType curKey = m_reportType;

                List<Bulletin> list = new List<Bulletin>();
                Bulletin detect = new Bulletin();

                while ((line = sr.ReadLine()) != null)
                {
                    if (line == "-----문단구분-----")
                        continue;

                    if (!m_dicData.ContainsKey(curKey))
                        m_dicData.Add(curKey, new ArrayList());

                    m_dicData[curKey].Add(line);
                }
            }

            using (StreamReader sr = new StreamReader(m_strPath + @"\report\BulletHwpDetailData.txt"))
            {
                string line;
                ReportType curKey = ReportType.BulletinDetail;

                List<Bulletin> list = new List<Bulletin>();
                Bulletin detect = new Bulletin();

                while ((line = sr.ReadLine()) != null)
                {
                    if (line == "-----문단구분-----")
                        continue;

                    if (!m_dicData.ContainsKey(curKey))
                        m_dicData.Add(curKey, new ArrayList());

                    m_dicData[curKey].Add(line);
                }
            }
        }

        public void Writer()
        {
            string title = m_strSavePath;
            if (m_reportType != ReportType.Bulletin)
            {
                if (m_nDataMaxIndex)
                    title = m_strSavePath + "_" + (m_nDataIndex + 1);
            }

            int nIndex = m_strSavePath.LastIndexOf("\\");
            string path = m_strSavePath.Substring(0, nIndex);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            using (StreamWriter sw = new StreamWriter(title + ".hml", false))
            {
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\" ?>");
                sw.WriteLine("<HWPML Style=\"embed\" SubVersion=\"8.0.1.0\" Version=\"2.8\">");

                int secCnt = 1;
                if (m_reportType == ReportType.Detect)
                    secCnt = 2;
                sw.WriteLine("<HEAD SecCnt=\"" + secCnt + "\">");
                MakeDocSummary(sw, m_strTitle, "Seungcheol", m_strDate);
                MakeDocSetting(sw);
                MakeMappingTable(sw);
                MakeCompatibleDocument(sw);                
                sw.WriteLine("</HEAD>");
                
                MakeCommonBody(sw);
                /*TAIL*/
                sw.WriteLine("<TAIL>");
                /*BINDATA*/
                sw.WriteLine("<BINDATASTORAGE>");
                ReadPNGFile(sw, 1, m_strPath + @"\report\" + m_strLogo);
                if (m_reportType == ReportType.ParetoSensor)
                {
                    if (m_facilityType == IFacility.FacilityType.PSM_SENSOR)
                    {
                        ReadBMPFile(sw, 2, m_strPath + @"\report\ParetoSensor.bmp");
                        ReadBMPFile(sw, 3, m_strPath + @"\report\ParetoEquipZone.bmp");
                        ReadBMPFile(sw, 4, m_strPath + @"\report\ParetoMaterial.bmp");
                        ReadBMPFile(sw, 5, m_strPath + @"\report\ParetoTank.bmp");
                    }
                    else
                    {
                        ReadBMPFile(sw, 2, m_strPath + @"\report\ParetoSensor.bmp");
                        if (m_nSiteID != 200)
                            ReadBMPFile(sw, 3, m_strPath + @"\report\ParetoEquipZone.bmp");
                    }
                }
                else if (m_reportType == ReportType.Detect)
                    ReadBMPFile(sw, 2, m_strPath + @"\report\Detect.bmp");
                else if (m_reportType == ReportType.NotOperation)
                {
                    ReadBMPFile(sw, 2, m_strPath + @"\report\Malfunction.bmp");
                    if (m_facilityType == IFacility.FacilityType.FIRE_SENSOR)
                        ReadPNGFile(sw, 3, m_strPath + @"\report\NotOperationLegend_Fire.png");
                    else
                        ReadPNGFile(sw, 3, m_strPath + @"\report\NotOperationLegend.png");
                }
                sw.WriteLine("</BINDATASTORAGE>");
                sw.WriteLine("<SCRIPTCODE Type=\"JScript\" Version=\"1.0\">");
                sw.WriteLine("<SCRIPTHEADER>var Documents = XHwpDocuments; var Document = Documents.Active_XHwpDocument;</SCRIPTHEADER>");
                sw.WriteLine("<SCRIPTSOURCE>function OnDocument_New() { //todo : }");
                sw.WriteLine("</SCRIPTSOURCE>");
                sw.WriteLine("</SCRIPTCODE>");
                sw.WriteLine("</TAIL>");
                sw.WriteLine("</HWPML>");
            }
        }

        private void MakeTagP(StreamWriter sw, int paraShape, int charShape, string text, bool columnBreak, bool pageBreak, int instId = -1)
        {
            string strLine = "";

            if (!columnBreak && !pageBreak)
                strLine = "<P ParaShape=\"" + paraShape + "\" Style=\"0\"";
            else
                strLine = "<P ParaShape=\"" + paraShape + "\" Style=\"0\" ColumnBreak=\"" + columnBreak.ToString().ToLower() + "\" PageBreak=\"" + pageBreak.ToString().ToLower() + "\"";

            if (instId == -1)
                strLine += ">";
            else
                strLine += "InstId = \"" + instId + "\">";

            if (text.Length > 0)
                strLine += "<TEXT CharShape=\"" + charShape + "\"><CHAR>" + text + "</CHAR></TEXT>";
            else
                strLine += "<TEXT CharShape=\"" + charShape + "\"/>";

            strLine += "</P>";

            sw.WriteLine(strLine);
        }
        private void MakeTagP(StreamWriter sw, int paraShape, ArrayList arr)
        {
            sw.WriteLine("<P ParaShape=\"" + paraShape + "\" Style=\"0\">");
            for (int i = 0; i < arr.Count; i += 2)
            {
                sw.WriteLine("<TEXT CharShape=\"" + arr[i] + "\"><CHAR>" + arr[i + 1] + "</CHAR></TEXT>");
            }
            sw.WriteLine("</P>");
        }
        private void MakeTable(StreamWriter sw, ReportType reportType = ReportType.NONE)
        {
            // 한 페이지에 13개행으로 고정
            int fixedRowCount = 13;
            if (reportType == ReportType.ParetoSensor || reportType == ReportType.ParetoEquipZone)
                fixedRowCount = 13;
            else if (reportType == ReportType.Detect)
                fixedRowCount = 12;
            else if (reportType == ReportType.Bulletin)
                fixedRowCount = 4;
            else if (reportType == ReportType.BulletinDetail)
                fixedRowCount = 10;
            else if (reportType == ReportType.NotOperation)
                fixedRowCount = 22;

            int nTableWidth = 67149;
            if (reportType == ReportType.ParetoSensor || reportType == ReportType.ParetoEquipZone)
                nTableWidth = 67149;
            else if (reportType == ReportType.Detect)
                nTableWidth = 66915;
            else if (reportType == ReportType.NotOperation)
                nTableWidth = 41515;
            else if (reportType == ReportType.Action)
                nTableWidth = 42143;

            int tableHeaderCellBorderFill = 4;
            int headerCellBorderFill = 4;
            int dataCellBorderFill = 4;
            if (reportType == ReportType.ParetoSensor || reportType == ReportType.ParetoEquipZone)
            {
                tableHeaderCellBorderFill = 4;
                headerCellBorderFill = 3;
                dataCellBorderFill = 5;
            }
            else if (reportType == ReportType.Detect)
            {
                tableHeaderCellBorderFill = 4;
                headerCellBorderFill = 3;
                dataCellBorderFill = 4;
            }
            else if (reportType == ReportType.NotOperation || reportType == ReportType.Action || reportType == ReportType.Bulletin || reportType == ReportType.BulletinDetail)
            {
                tableHeaderCellBorderFill = 2;
                headerCellBorderFill = 4;
                dataCellBorderFill = 5;
            }

            List<TableColumns> defineColumns = new List<TableColumns>();
            if (m_dicDefineColumns.ContainsKey(reportType))
                defineColumns = m_dicDefineColumns[reportType];

            if (reportType == ReportType.BulletinDetail)
            {
                defineColumns = m_dicDefineColumns[ReportType.Bulletin];
            }

            sw.WriteLine("<P ParaShape=\"12\" Style=\"0\">");
            sw.WriteLine("<TEXT CharShape=\"5\">");

            int curTableIndex = 0;
            int curRowIndex = 0;
            int dataMinIndex = m_nDataIndex * m_nDataMaxCount;
            int dataMaxIndex = (m_nDataIndex * m_nDataMaxCount) + m_nDataMaxCount;

            if (dataMaxIndex > m_dicData[reportType].Count / defineColumns.Count)
                dataMaxIndex = m_dicData[reportType].Count / defineColumns.Count;

            for (int i = dataMinIndex; i < dataMaxIndex; i++)
            {
                int rowCount = fixedRowCount;
                if (curTableIndex > 0) // 첫번째 표는 제목이 있기때문에 Row가 한줄 적다.
                {
                    if (reportType == ReportType.Bulletin)
                        rowCount = 17;
                    else
                        rowCount = fixedRowCount + 1;
                }

                if (rowCount > dataMaxIndex - dataMinIndex)
                    rowCount = dataMaxIndex - dataMinIndex + 1;
                
                if (curRowIndex == 0)
                {
                    int curTableRowCount = 1; // Field Row 1 미리 추가
                    if (curTableIndex == 0)
                        curTableRowCount = rowCount;
                    else
                    {
                        if (dataMaxIndex - i < rowCount)
                            curTableRowCount += (dataMaxIndex) - i;
                        else
                            curTableRowCount = rowCount;
                    }
                                
                    MakeTableHeader(sw, tableHeaderCellBorderFill, defineColumns.Count, curTableRowCount, nTableWidth, 8094, 4, 0, 0);

                    sw.WriteLine("<ROW>");
                    int nColAddr = 0;
                                                                
                    foreach (TableColumns item in defineColumns)
                    {
                        int colWidth = nTableWidth * item.ColumnWidthRatio / 100;

                        MakeCell(sw, headerCellBorderFill, nColAddr, 1, 0, 1, item.ColumnName, colWidth, 10, false);
                        nColAddr++;
                    }
                    sw.WriteLine("</ROW>");
                }

                curRowIndex++;

                // 읽은 데이터 Row 추가                
                if (m_dicData.ContainsKey(reportType))
                {
                    ArrayList data = m_dicData[reportType];
                    int dataIndex = i * defineColumns.Count;
                    sw.WriteLine("<ROW>");
                    int curColAddr = 0;
                    for (int j = 0; j < defineColumns.Count; j++)
                    {
                        int colWidth = nTableWidth * defineColumns[j].ColumnWidthRatio / 100;
                        MakeCell(sw, dataCellBorderFill, curColAddr, 1, curRowIndex, 1, data[dataIndex++].ToString(), colWidth, 9, true);
                        curColAddr++;
                    }
                    sw.WriteLine("</ROW>");
                }

                // 한 페이지가 꽉 찼거나 마지막 데이터일 때
                if (curRowIndex == rowCount - 1 || dataMaxIndex - 1 == i)
                {
                    sw.WriteLine("</TABLE>");

                    curTableIndex++;
                    curRowIndex = 0;
                }
            }

            sw.WriteLine("</TEXT>");
            sw.WriteLine("</P>");
        }

        private void MakeTableHeader(StreamWriter sw, int borderFill, int colCount, int rowCount, int width, int height, int zorder, int horzOffset, int vertOffset)
        {
            //sw.WriteLine("<P ParaShape=\"12\" Style=\"0\">");
            //sw.WriteLine("<TEXT CharShape=\"5\">");
            sw.WriteLine("<TABLE BorderFill=\"2\" CellSpacing=\"0\" ColCount=\"" + colCount + "\" PageBreak=\"Table\" RepeatHeader=\"true\" RowCount=\"" + rowCount + "\">");
            sw.WriteLine("<SHAPEOBJECT InstId=\"1335333524\" Lock=\"false\" NumberingType=\"Table\" TextWrap=\"TopAndBottom\" ZOrder=\"" + zorder + "\">");
            sw.WriteLine("<SIZE Height=\"" + height + "\" HeightRelTo=\"Absolute\" Protect=\"false\" Width=\"" + width + "\" WidthRelTo=\"Absolute\"/>");
            sw.WriteLine("<POSITION AffectLSpacing=\"false\" AllowOverlap=\"false\" FlowWithText=\"true\" HoldAnchorAndSO=\"false\" HorzAlign=\"Left\" HorzOffset=\"" + horzOffset + "\" HorzRelTo=\"Para\" TreatAsChar=\"false\" VertAlign=\"Top\" VertOffset=\"" + vertOffset + "\" VertRelTo=\"Para\"/>");
            sw.WriteLine("<OUTSIDEMARGIN Bottom=\"141\" Left=\"141\" Right=\"141\" Top=\"141\"/>");
            sw.WriteLine("</SHAPEOBJECT>");
            sw.WriteLine("<INSIDEMARGIN Bottom=\"283\" Left=\"283\" Right=\"283\" Top=\"283\"/>");
        }

        private void MakeCell(StreamWriter sw, int borderFill, int colAddr, int colSpan, int rowAddr, int rowSpan, string colName, int colWidth, int charShape, bool isHeader)
        {
            if (colName.Contains("&"))
                colName = colName.Replace("&", "&amp;");

            sw.WriteLine("<CELL BorderFill=\"" + borderFill + "\" ColAddr=\"" + colAddr + "\" ColSpan=\"" + colSpan + "\" Dirty=\"false\" Editable=\"false\" HasMargin=\"false\" Header=\"" + isHeader.ToString().ToLower() + "\" Height=\"2698\" Protect=\"false\" RowAddr=\"" + rowAddr + "\" RowSpan=\"" + rowSpan + "\" Width=\"" + colWidth + "\">");
            sw.WriteLine("<PARALIST LineWrap=\"Break\" LinkListID=\"0\" LinkListIDNext=\"0\" TextDirection=\"0\" VertAlign=\"Center\">");
            MakeTagP(sw, 13, charShape, colName, false, false);
            sw.WriteLine("</PARALIST>");
            sw.WriteLine("</CELL>");
        }
        private void MakeDocSummary(StreamWriter sw, string title, string author, string date)
        {
            sw.WriteLine("<DOCSUMMARY>");
			sw.WriteLine("<TITLE>" + title + "</TITLE>");
            sw.WriteLine("<AUTHOR>" + author + "</AUTHOR>");
            sw.WriteLine("<DATE>" + date + "</DATE>");
            sw.WriteLine("</DOCSUMMARY>");
        }
        private void MakeDocSetting(StreamWriter sw)
        {
            sw.WriteLine("<DOCSETTING>");
			sw.WriteLine("<BEGINNUMBER Endnote=\"1\" Equation=\"1\" Footnote=\"1\" Page=\"1\" Picture=\"1\" Table=\"1\"/>");
            if (m_reportType == ReportType.ParetoSensor)
                sw.WriteLine("<CARETPOS List=\"0\" Para=\"15\" Pos=\"16\"/>");
            else if (m_reportType == ReportType.Detect)
                sw.WriteLine("<CARETPOS List=\"0\" Para=\"15\" Pos=\"8\"/>");
            else if (m_reportType == ReportType.NotOperation)
                sw.WriteLine("<CARETPOS List=\"0\" Para=\"2\" Pos=\"16\"/>");
            else if (m_reportType == ReportType.Action)
                sw.WriteLine("<CARETPOS List=\"0\" Para=\"9\" Pos=\"31\"/>");
            else if (m_reportType == ReportType.Bulletin)
                sw.WriteLine("<CARETPOS List=\"0\" Para=\"12\" Pos=\"20\"/>");

            sw.WriteLine("</DOCSETTING>");
        }

        #region MappingTable
        private void MakeMappingTable(StreamWriter sw)
        {
            sw.WriteLine("<MAPPINGTABLE>");
            MakeBinDataList(sw);
            MakeFacenameList(sw);
            MakeBorderFillList(sw);
            MakeCharShapeList(sw);
            MakeTabdefList(sw);
            MakeNumberingList(sw);
            MakeParaShapeList(sw);
            MakeStyleList(sw);
            sw.WriteLine("</MAPPINGTABLE>");
        }
        private void MakeBinDataList(StreamWriter sw)
        {            
            List<string> binList = new List<string>();
            binList.Add("png"); // Logo

            if (m_reportType == ReportType.ParetoSensor)
            {
                binList.Add("bmp"); // 차트1
                binList.Add("bmp"); // 차트2
                if (m_facilityType == IFacility.FacilityType.PSM_SENSOR)
                {
                    binList.Add("bmp"); // 차트3
                    binList.Add("bmp"); // 차트4
                }
            }
            else if (m_reportType == ReportType.Detect)
                binList.Add("bmp"); // 차트1
            else if (m_reportType == ReportType.NotOperation)
            {
                binList.Add("bmp"); // 차트1
                binList.Add("png"); // 범례
            }

            sw.WriteLine("<BINDATALIST Count=\"" + binList.Count + "\">");
            for (int i = 1; i <= binList.Count; i++)
            {
                sw.WriteLine("<BINITEM BinData=\"" + i + "\" Format=\"" + binList[i - 1] + "\" Type=\"Embedding\"/>");
            }
			sw.WriteLine("</BINDATALIST>");
        }

        private void MakeFacenameList(StreamWriter sw)
        {
            List<string> langList = new List<string>();
            langList.AddRange(new string[] { "Hangul", "Latin", "Hanja", "Japanese", "Other", "Symbol", "User" });

            List<string> fontList = new List<string>();
            fontList.AddRange(new string[] { "맑은 고딕", "바탕", "함초롬돋움", "함초롬바탕" });

            sw.WriteLine("<FACENAMELIST>");
            for (int i = 0; i < langList.Count; i++)
            {
                sw.WriteLine("<FONTFACE Count=\"" + fontList.Count + "\" Lang=\"" + langList[i] + "\">");
                for (int j = 0; j < fontList.Count; j++)
                {
                    sw.WriteLine("<FONT Id=\"" + j + "\" Name=\"" + fontList[j] + "\" Type=\"ttf\">");
                    if (fontList[j] == "맑은 고딕")
                        sw.WriteLine("<TYPEINFO ArmStyle=\"0\" Contrast=\"2\" FamilyType=\"2\" Letterform=\"2\" Midline=\"0\" Proportion=\"3\" StrokeVariation=\"0\" Weight=\"5\" XHeight=\"4\"/>");
                    else if (fontList[j] == "바탕")
                        sw.WriteLine("<TYPEINFO ArmStyle=\"1\" Contrast=\"0\" FamilyType=\"2\" Letterform=\"1\" Midline=\"1\" Proportion=\"0\" StrokeVariation=\"1\" Weight=\"6\" XHeight=\"1\"/>");
                    else if (fontList[j] == "함초롬돋움")
                        sw.WriteLine("<TYPEINFO ArmStyle=\"1\" Contrast=\"0\" FamilyType=\"2\" Letterform=\"1\" Midline=\"1\" Proportion=\"4\" StrokeVariation=\"1\" Weight=\"5\" XHeight=\"1\"/>");
                    else if (fontList[j] == "함초롬바탕")
                        sw.WriteLine("<TYPEINFO ArmStyle=\"1\" Contrast=\"0\" FamilyType=\"2\" Letterform=\"1\" Midline=\"1\" Proportion=\"4\" StrokeVariation=\"1\" Weight=\"5\" XHeight=\"1\"/>");
                    sw.WriteLine("</FONT>");
                }
                sw.WriteLine("</FONTFACE>");
            }
            sw.WriteLine("</FACENAMELIST>");
        }

        private void MakeBorderFillList(StreamWriter sw)
        {
            int count = 5;
            if (m_reportType == ReportType.Detect)
                count = 4;

            sw.WriteLine("<BORDERFILLLIST Count=\"" + count + "\">"); //BorderFillList count
            for (int i = 1; i <= count; i++)
            {
                string type = "None";
                string width = "0.1";
                string breakCellSeparateLine = "0";
                string diagonalWidth = "0.1";
                uint faceColor = 0;
                uint hatchColor = 0;

                if (m_reportType == ReportType.NotOperation)
                {
                    if (i == 2) { type = "Solid"; width = "0.12"; }
                    else if (i == 3) { width = "0.1"; faceColor = 4294967295; hatchColor = 4278190080; }
                    else if (i == 4) { type = "Solid"; width = "0.25"; breakCellSeparateLine = "1"; faceColor = 16116447; hatchColor = 0; }
                    else if (i == 5) { type = "Solid"; width = "0.25"; faceColor = 16777215; hatchColor = 0; }
                }
                else if (m_reportType == ReportType.Action)
                {
                    if (i == 2) { type = "Solid"; width = "0.12"; }
                    else if (i == 3) { type = "None"; width = "0.1"; faceColor = 4294967295; hatchColor = 4278190080; }
                    else if (i == 4) { type = "Solid"; width = "0.25"; faceColor = 16116447; hatchColor = 0; }
                    else if (i == 5) { type = "Solid"; width = "0.25"; faceColor = 16777215; hatchColor = 0; }
                }
                else if (m_reportType == ReportType.Detect)
                {
                    if (i == 2) { faceColor = 4294967295; hatchColor = 4278190080; }
                    else if (i == 3) { type = "Solid"; width = "0.25"; faceColor = 16116447; hatchColor = 0; }
                    else if (i == 4) { type = "Solid"; width = "0.25"; faceColor = 16777215; hatchColor = 0; }
                }
                else if (m_reportType == ReportType.Bulletin)
                {
                    if (i == 2) { type = "Solid"; width = "0.12"; }
                    else if (i == 3) { type = "None"; width = "0.1"; faceColor = 4294967295; hatchColor = 4278190080; }
                    else if (i == 4) { type = "Solid"; width = "0.25"; faceColor = 16116447; hatchColor = 0; }
                    else if (i == 5) { type = "Solid"; width = "0.25"; }
                }
                else
                {
                    if (i == 2) { faceColor = 4294967295; hatchColor = 4278190080; }
                    else if (i == 3) { type = "Solid"; width = "0.25"; faceColor = 16116447; hatchColor = 0; }
                    else if (i == 4) { type = "Solid"; width = "1.5"; breakCellSeparateLine = "1"; faceColor = 16777215; hatchColor = 7882806; diagonalWidth = "2.0"; }
                    else if (i == 5) { type = "Solid"; width = "0.25"; faceColor = 16777215; hatchColor = 0; } 
                }

                sw.WriteLine("<BORDERFILL BackSlash=\"0\" BreakCellSeparateLine=\"" + breakCellSeparateLine + "\" CenterLine=\"0\" CounterBackSlash=\"0\" CounterSlash=\"0\" CrookedSlash=\"0\" Id=\"" + i + "\" Shadow=\"false\" Slash=\"0\" ThreeD=\"false\">");
                sw.WriteLine("<LEFTBORDER Type=\"" + type + "\" Width=\"" + width + "mm\"/>");
                sw.WriteLine("<RIGHTBORDER Type=\"" + type + "\" Width=\"" + width + "mm\"/>");
                sw.WriteLine("<TOPBORDER Type=\"" + type + "\" Width=\"" + width + "mm\"/>");
                sw.WriteLine("<BOTTOMBORDER Type=\"" + type + "\" Width=\"" + width + "mm\"/>");
                sw.WriteLine("<DIAGONAL Type=\"Solid\" Width=\"" + diagonalWidth + "mm\"/>");

                if (faceColor > 0 || hatchColor > 0)
                {
                    sw.WriteLine("<FILLBRUSH>");
                    sw.WriteLine("<WINDOWBRUSH Alpha=\"0\" FaceColor=\"" + faceColor + "\" HatchColor=\"" + hatchColor + "\"/>");
                    sw.WriteLine("</FILLBRUSH>");
                }

                sw.WriteLine("</BORDERFILL>");
            }
            sw.WriteLine("</BORDERFILLLIST>");
        }

        private void MakeCharShapeList(StreamWriter sw)
        {
            int count = 15;
            if (m_reportType == ReportType.Detect)
                count = 14;
            else if (m_reportType == ReportType.NotOperation)
                count = 18;
            else if (m_reportType == ReportType.Action || m_reportType == ReportType.Bulletin)
                count = 12;

            sw.WriteLine("<CHARSHAPELIST Count=\"" + count + "\">");
            for (int i = 0; i < count; i++)
            {
                int id = i;
                int height = 1000;
                int fontid = 2;
                uint textColor = 0;
                bool isBold = false;
                int charspacing = 0;
                int borderFillId = 2;
                if (m_reportType == ReportType.NotOperation)
                {
                    borderFillId = 3;
                    if (id == 0) { height = 1000; fontid = 2; }
                    else if (id == 1) { height = 1000; fontid = 4; }
                    else if (id == 2) { height = 900; fontid = 3; }
                    else if (id == 3) { height = 900; fontid = 4; }
                    else if (id == 4) { height = 900; fontid = 3; charspacing = -5; }
                    else if (id == 5) { height = 1100; fontid = 1; }
                    else if (id == 6) { height = 1100; fontid = 0; isBold = true; }
                    else if (id == 7) { height = 2000; fontid = 0; isBold = true; }
                    else if (id == 8) { height = 1000; fontid = 0; isBold = true; }
                    else if (id == 9) { height = 900; fontid = 0; }
                    else if (id == 10) { height = 900; fontid = 0; isBold = true; }
                    else if (id == 11) { height = 1200; fontid = 0; isBold = true; }
                    else if (id == 12) { height = 900; fontid = 0; isBold = true; textColor = 6118749; }
                    else if (id == 13) { height = 900; fontid = 0; textColor = 7895160; }
                    else if (id == 14) { height = 1100; fontid = 2; }
                    else if (id == 15) { height = 1100; fontid = 0; }
                    else if (id == 16) { height = 1000; fontid = 0; isBold = true; textColor = 3487029; }
                    else if (id == 17) { height = 1000; fontid = 0; }
                }
                else if (m_reportType == ReportType.Action)
                {
                    borderFillId = 3;
                    if (id == 0) { height = 1000; fontid = 2; }
                    else if (id == 1) { height = 1000; fontid = 3; }
                    else if (id == 2) { height = 900; fontid = 2; }
                    else if (id == 3) { height = 900; fontid = 3; }
                    else if (id == 4) { height = 900; fontid = 2; charspacing = -5; }
                    else if (id == 5) { height = 1100; fontid = 1; }
                    else if (id == 6) { height = 1100; fontid = 0; isBold = true; }
                    else if (id == 7) { height = 2000; fontid = 0; isBold = true; }
                    else if (id == 8) { height = 1000; fontid = 0; isBold = true; }
                    else if (id == 9) { height = 1000; fontid = 0; }
                    else if (id == 10) { height = 1100; fontid = 0; isBold = true; textColor = 3487029; }
                    else if (id == 11) { height = 1000; fontid = 0; textColor = 6118749; }
                }
                else if (m_reportType == ReportType.Bulletin)
                {
                    borderFillId = 3;
                    if (id == 0) { height = 1000; fontid = 2; }
                    else if (id == 1) { height = 1000; fontid = 3; }
                    else if (id == 2) { height = 900; fontid = 2; }
                    else if (id == 3) { height = 900; fontid = 3; }
                    else if (id == 4) { height = 900; fontid = 2; charspacing = -5; }
                    else if (id == 5) { height = 1000; fontid = 1; }
                    else if (id == 6) { height = 1100; fontid = 1; }
                    else if (id == 7) { height = 1100; fontid = 0; isBold = true; }
                    else if (id == 8) { height = 2000; fontid = 0; isBold = true; }
                    else if (id == 9) { height = 1000; fontid = 0; isBold = true; }
                    else if (id == 10) { height = 1000; fontid = 0; }
                    else if (id == 11) { height = 1100; fontid = 0; isBold = true; textColor = 3487029; }
                }
                else if (m_reportType == ReportType.Detect)
                {
                    if (id == 0) { height = 1000; fontid = 2; }
                    else if (id == 1) { height = 1000; fontid = 3; }
                    else if (id == 2) { height = 900; fontid = 2; }
                    else if (id == 3) { height = 900; fontid = 3; }
                    else if (id == 4) { height = 900; fontid = 2; charspacing = -5; }
                    else if (id == 5) { height = 1100; fontid = 1; }
                    else if (id == 6) { height = 1100; fontid = 1; isBold = true; }
                    else if (id == 7) { height = 1100; fontid = 0; isBold = true; }
                    else if (id == 8) { height = 2000; fontid = 0; isBold = true; }
                    else if (id == 9) { height = 900; fontid = 0; }
                    else if (id == 10) { height = 900; fontid = 0; isBold = true; }
                    else if (id == 11) { height = 1200; fontid = 0; isBold = true; }
                    else if (id == 12) { height = 900; fontid = 0; isBold = true; textColor = 6118749; }
                    else if (id == 13) { height = 1000; fontid = 0; textColor = 6118749; }
                }
                else
                {
                    if (id == 0) { height = 1000; fontid = 2; }
                    else if (id == 1) { height = 1000; fontid = 3; }
                    else if (id == 2) { height = 900; fontid = 2; }
                    else if (id == 3) { height = 900; fontid = 3; }
                    else if (id == 4) { height = 900; fontid = 2; charspacing = -5; }
                    else if (id == 5) { height = 1100; fontid = 1; }
                    else if (id == 6) { height = 1100; fontid = 1; isBold = true; }
                    else if (id == 7) { height = 1100; fontid = 0; isBold = true; }
                    else if (id == 8) { height = 2000; fontid = 0; isBold = true; }
                    else if (id == 9) { height = 900; fontid = 0; }
                    else if (id == 10) { height = 900; fontid = 0; isBold = true; }
                    else if (id == 11) { height = 1200; fontid = 0; isBold = true; }
                    else if (id == 12) { height = 900; fontid = 0; isBold = true; textColor = 6118749; }
                    else if (id == 13) { height = 1000; fontid = 0; textColor = 7895160; }
                    else if (id == 14) { height = 1000; fontid = 0; isBold = true; textColor = 6118749; }
                }


                sw.WriteLine("<CHARSHAPE BorderFillId=\"" + borderFillId + "\" Height=\"" + height + "\" Id=\"" + id + "\" ShadeColor=\"4294967295\" SymMark=\"0\" TextColor=\"" + textColor + "\" UseFontSpace=\"false\" UseKerning=\"false\">");
                sw.WriteLine(string.Format("<FONTID Hangul=\"{0}\" Hanja=\"{0}\" Japanese=\"{0}\" Latin=\"{0}\" Other=\"{0}\" Symbol=\"{0}\" User=\"{0}\"/>", fontid));
                sw.WriteLine("<RATIO Hangul=\"100\" Hanja=\"100\" Japanese=\"100\" Latin=\"100\" Other=\"100\" Symbol=\"100\" User=\"100\"/>");
                sw.WriteLine(string.Format("<CHARSPACING Hangul=\"{0}\" Hanja=\"{0}\" Japanese=\"{0}\" Latin=\"{0}\" Other=\"{0}\" Symbol=\"{0}\" User=\"{0}\"/>", charspacing));
                sw.WriteLine("<RELSIZE Hangul=\"100\" Hanja=\"100\" Japanese=\"100\" Latin=\"100\" Other=\"100\" Symbol=\"100\" User=\"100\"/>");
                sw.WriteLine("<CHAROFFSET Hangul=\"0\" Hanja=\"0\" Japanese=\"0\" Latin=\"0\" Other=\"0\" Symbol=\"0\" User=\"0\"/>");
                if (isBold)
                    sw.WriteLine("<BOLD/>");
                sw.WriteLine("</CHARSHAPE>");
            }
            sw.WriteLine("</CHARSHAPELIST>");
        }

        private void MakeTabdefList(StreamWriter sw)
        {
            sw.WriteLine("<TABDEFLIST Count=\"1\">");
            sw.WriteLine("<TABDEF AutoTabLeft=\"true\" AutoTabRight=\"false\" Id=\"0\"/>");
            sw.WriteLine("</TABDEFLIST>");
        }

        private void MakeNumberingList(StreamWriter sw)
        {
            if (m_reportType == ReportType.Detect)
            {
                sw.WriteLine("<NUMBERINGLIST Count=\"1\">");
                sw.WriteLine("<NUMBERING Id=\"1\" Start=\"0\">");
                sw.WriteLine("<PARAHEAD Alignment=\"Left\" AutoIndent=\"true\" Level=\"1\" NumFormat=\"Digit\" Start=\"1\" TextOffset=\"50\" TextOffsetType=\"percent\" UseInstWidth=\"true\" WidthAdjust=\"0\">^1.</PARAHEAD>");
                sw.WriteLine("<PARAHEAD Alignment=\"Left\" AutoIndent=\"true\" Level=\"2\" NumFormat=\"HangulSyllable\" Start=\"1\" TextOffset=\"50\" TextOffsetType=\"percent\" UseInstWidth=\"true\" WidthAdjust=\"0\">^2.</PARAHEAD>");
                sw.WriteLine("<PARAHEAD Alignment=\"Left\" AutoIndent=\"true\" Level=\"3\" NumFormat=\"Digit\" Start=\"1\" TextOffset=\"50\" TextOffsetType=\"percent\" UseInstWidth=\"true\"  WidthAdjust=\"0\">^3)</PARAHEAD>");
                sw.WriteLine("<PARAHEAD Alignment=\"Left\" AutoIndent=\"true\" Level=\"4\" NumFormat=\"Digit\" Start=\"1\" TextOffset=\"50\" TextOffsetType=\"percent\" UseInstWidth=\"false\" WidthAdjust=\"0\"/>");
                sw.WriteLine("<PARAHEAD Alignment=\"Left\" AutoIndent=\"true\" Level=\"5\" NumFormat=\"Digit\" Start=\"1\" TextOffset=\"50\" TextOffsetType=\"percent\" UseInstWidth=\"false\" WidthAdjust=\"0\"/>");
                sw.WriteLine("<PARAHEAD Alignment=\"Left\" AutoIndent=\"true\" Level=\"6\" NumFormat=\"Digit\" Start=\"1\" TextOffset=\"50\" TextOffsetType=\"percent\" UseInstWidth=\"false\" WidthAdjust=\"0\"/>");
                sw.WriteLine("<PARAHEAD Alignment=\"Left\" AutoIndent=\"true\" Level=\"7\" NumFormat=\"Digit\" Start=\"1\" TextOffset=\"50\" TextOffsetType=\"percent\" UseInstWidth=\"false\" WidthAdjust=\"0\"/>");
                sw.WriteLine("</NUMBERING>");
                sw.WriteLine("</NUMBERINGLIST>");
            }
            else
            {
                sw.WriteLine("<NUMBERINGLIST Count=\"1\">");
                sw.WriteLine("<NUMBERING Id=\"1\" Start=\"0\">");
                sw.WriteLine("<PARAHEAD Alignment=\"Left\" AutoIndent=\"true\" Level=\"1\" NumFormat=\"Digit\" Start=\"1\" TextOffset=\"50\" TextOffsetType=\"percent\" UseInstWidth=\"true\" WidthAdjust=\"0\">^1.</PARAHEAD>");
                sw.WriteLine("<PARAHEAD Alignment=\"Left\" AutoIndent=\"true\" Level=\"2\" NumFormat=\"HangulSyllable\" Start=\"1\" TextOffset=\"50\" TextOffsetType=\"percent\" UseInstWidth=\"true\" WidthAdjust=\"0\">^2.</PARAHEAD>");
                sw.WriteLine("<PARAHEAD Alignment=\"Left\" AutoIndent=\"true\" Level=\"3\" NumFormat=\"Digit\" Start=\"1\" TextOffset=\"50\" TextOffsetType=\"percent\" UseInstWidth=\"true\" WidthAdjust=\"0\">^3)</PARAHEAD>");
                sw.WriteLine("<PARAHEAD Alignment=\"Left\" AutoIndent=\"true\" Level=\"4\" NumFormat=\"HangulSyllable\" Start=\"1\" TextOffset=\"50\" TextOffsetType=\"percent\" UseInstWidth=\"true\" WidthAdjust=\"0\">^4)</PARAHEAD>");
                sw.WriteLine("<PARAHEAD Alignment=\"Left\" AutoIndent=\"true\" Level=\"5\" NumFormat=\"Digit\" Start=\"1\" TextOffset=\"50\" TextOffsetType=\"percent\" UseInstWidth=\"true\" WidthAdjust=\"0\">(^5)</PARAHEAD>");
                sw.WriteLine("<PARAHEAD Alignment=\"Left\" AutoIndent=\"true\" Level=\"6\" NumFormat=\"HangulSyllable\" Start=\"1\" TextOffset=\"50\" TextOffsetType=\"percent\" UseInstWidth=\"true\" WidthAdjust=\"0\">(^6)</PARAHEAD>");
                sw.WriteLine("<PARAHEAD Alignment=\"Left\" AutoIndent=\"true\" Level=\"7\" NumFormat=\"CircledDigit\" Start=\"1\" TextOffset=\"50\" TextOffsetType=\"percent\" UseInstWidth=\"true\" WidthAdjust=\"0\">^7</PARAHEAD>");
                sw.WriteLine("</NUMBERING>");
                sw.WriteLine("</NUMBERINGLIST>");
            }
        }

        private void MakeParaShapeList(StreamWriter sw)
        {
            int count = 16;
            if (m_reportType == ReportType.Bulletin)
                count = 17;

            sw.WriteLine("<PARASHAPELIST Count=\"" + count + "\">");
            for (int i = 0; i < count; i++)
            {
                int id = i;
                string align = "Justify";
                int indent = 0;
                int left = 0;
                int lineSpacing = 160;
                bool breakNonLatinWord = true;
                string headingType = "None";
                int condense = 0;
                int level = 0;
                bool snapToGrid = false;
                int borderFill = 2;
                
                if (m_reportType == ReportType.Bulletin)
                {
                    borderFill = 3;
                    if (i == 0) { indent = -2620; lineSpacing = 130; }
                    else if (i == 1) { }
                    else if (i == 2) { left = 2000; headingType = "Outline"; condense = 20; }
                    else if (i == 3) { left = 4000; headingType = "Outline"; condense = 20; level = 1; }
                    else if (i == 4) { left = 6000; headingType = "Outline"; condense = 20; level = 2; }
                    else if (i == 5) { left = 8000; headingType = "Outline"; condense = 20; level = 3; }
                    else if (i == 6) { left = 10000; headingType = "Outline"; condense = 20; level = 4; }
                    else if (i == 7) { left = 12000; headingType = "Outline"; condense = 20; level = 5; }
                    else if (i == 8) { left = 14000; headingType = "Outline"; condense = 20; level = 6; }
                    else if (i == 9) { left = 3000; }
                    else if (i == 10) { lineSpacing = 150; breakNonLatinWord = false; }
                    else if (i == 11) { lineSpacing = 130; snapToGrid = true; }
                    else if (i == 12) { snapToGrid = true; }
                    else if (i == 13) { align = "Center"; breakNonLatinWord = false; snapToGrid = true; }
                    else if (i == 14) { align = "Right"; lineSpacing = 150; breakNonLatinWord = false; }
                    else if (i == 15) { lineSpacing = 180; snapToGrid = true; }
                    else if (i == 16) { align = "Left"; indent = 1000; breakNonLatinWord = false; snapToGrid = true; }
                }
                else
                {
                    if (m_reportType == ReportType.NotOperation || m_reportType == ReportType.Action)
                        borderFill = 3;

                    if (i == 0) { indent = -2620; lineSpacing = 130; }
                    else if (i == 1) { }
                    else if (i == 2) { left = 2000; headingType = "Outline"; condense = 20; }
                    else if (i == 3) { left = 4000; headingType = "Outline"; condense = 20; level = 1; }
                    else if (i == 4) { left = 6000; headingType = "Outline"; condense = 20; level = 2; }
                    else if (i == 5) { left = 8000; headingType = "Outline"; condense = 20; level = 3; }
                    else if (i == 6) { left = 10000; headingType = "Outline"; condense = 20; level = 4; }
                    else if (i == 7) { left = 12000; headingType = "Outline"; condense = 20; level = 5; }
                    else if (i == 8) { left = 14000; headingType = "Outline"; condense = 20; level = 6; }
                    else if (i == 9) { left = 3000; }
                    else if (i == 10) { lineSpacing = 150; breakNonLatinWord = false; }
                    else if (i == 11) { lineSpacing = 130; snapToGrid = true; }
                    else if (i == 12) { snapToGrid = true; }
                    else if (i == 13) { align = "Center"; breakNonLatinWord = false; snapToGrid = true; }
                    else if (i == 14) { align = "Right"; lineSpacing = 150; breakNonLatinWord = false; }
                    else if (i == 15) { lineSpacing = 200; snapToGrid = true; }
                }
                

                sw.WriteLine("<PARASHAPE Align=\"" + align + "\" AutoSpaceEAsianEng=\"false\" AutoSpaceEAsianNum=\"false\" BreakLatinWord=\"KeepWord\" BreakNonLatinWord=\"" + breakNonLatinWord.ToString().ToLower() + "\" Condense=\"" + condense + "\" FontLineHeight=\"false\" HeadingType=\"" + headingType + "\" Id=\"" + id + "\" KeepLines=\"false\" KeepWithNext=\"false\" Level=\"" + level + "\" LineWrap=\"Break\" PageBreakBefore=\"false\" SnapToGrid=\"" + snapToGrid.ToString().ToLower() + "\" TabDef=\"0\" VerAlign=\"Baseline\" WidowOrphan=\"false\">");
                sw.WriteLine("<PARAMARGIN Indent=\"" + indent + "\" Left=\"" + left + "\" LineSpacing=\"" + lineSpacing + "\" LineSpacingType=\"Percent\" Next=\"0\" Prev=\"0\" Right=\"0\"/>");
                sw.WriteLine("<PARABORDER BorderFill=\"" + borderFill + "\" Connect=\"false\" IgnoreMargin=\"false\"/>");
                sw.WriteLine("</PARASHAPE>");
            }
            sw.WriteLine("</PARASHAPELIST>");
        }

        private void MakeStyleList(StreamWriter sw)
        {
            sw.WriteLine("<STYLELIST Count=\"14\">");
            for (int i = 0; i < 14; i++)
            {
                int id = i;
                int charShape = 1;
                string engName = "Normal";
                string name = "바탕글";
                int paraShape = 12;

                if (id == 1) { engName = "Body"; name = "본문"; paraShape = 9; }
                else if (id == 2) { engName = "Outline 1"; name = "개요 1"; paraShape = 2; }
                else if (id == 3) { engName = "Outline 2"; name = "개요 2"; paraShape = 3; }
                else if (id == 4) { engName = "Outline 3"; name = "개요 3"; paraShape = 4; }
                else if (id == 5) { engName = "Outline 4"; name = "개요 4"; paraShape = 5; }
                else if (id == 6) { engName = "Outline 5"; name = "개요 5"; paraShape = 6; }
                else if (id == 7) { engName = "Outline 6"; name = "개요 6"; paraShape = 7; }
                else if (id == 8) { engName = "Outline 7"; name = "개요 7"; paraShape = 8; }

                else if (id == 9) { engName = "Page Number"; name = "쪽 번호"; paraShape = 1; charShape = 0; }
                else if (id == 10) { engName = "Header"; name = "머리말"; paraShape = 10; charShape = 2; }
                else if (id == 11) { engName = "Footnote"; name = "각주"; paraShape = 0; charShape = 3; }
                else if (id == 12) { engName = "Endnote"; name = "미주"; paraShape = 0; charShape = 3; }
                else if (id == 13) { engName = "Memo"; name = "메모"; paraShape = 11; charShape = 4; }

                sw.WriteLine("<STYLE CharShape=\"" + charShape + "\" EngName=\"" + engName + "\" Id=\"" + id + "\" LangId=\"1042\" LockForm=\"0\" Name=\"" + name + "\" NextStyle=\"" + id + "\" ParaShape=\"" + paraShape + "\" Type=\"Para\"/>");
            }
            sw.WriteLine("</STYLELIST>");
        }
        #endregion

        private void MakeCompatibleDocument(StreamWriter sw)
        {
            sw.WriteLine("<COMPATIBLEDOCUMENT TargetProgram=\"None\">");
            sw.WriteLine("<LAYOUTCOMPATIBILITY AdjustBaselineInFixedLinespacing=\"false\" AdjustBaselineOfObjectToBottom=\"false\" AdjustLineheightToFont=\"false\" AdjustMarginFromAdjustLineheight=\"false\" AdjustParaBorderOffsetWithBorder=\"false\" AdjustParaBorderfillToSpacing=\"false\" AdjustVertPosOfLine=\"false\" ApplyAtLeastToPercent100Pct=\"false\" ApplyCharSpacingToCharGrid=\"false\" ApplyExtendHeaderFooterEachSection=\"false\" ApplyFontWeightToBold=\"false\" ApplyFontspaceToLatin=\"false\" ApplyMinColumnWidthTo1mm=\"false\" ApplyNextspacingOfLastPara=\"false\" ApplyParaBorderToOutside=\"false\" ApplyPrevspacingBeneathObject=\"false\" ApplyTabPosBasedOnSegment=\"false\" BaseCharUnitOfIndentOnFirstChar=\"false\" BaseCharUnitOnEAsian=\"false\" BaseLinespacingOnLinegrid=\"false\" BreakTabOverLine=\"false\" ConnectParaBorderfillOfEqualBorder=\"false\" DoNotAdjustEmptyAnchorLine=\"false\" DoNotAdjustWordInJustify=\"false\" DoNotAlignLastForbidden=\"false\" DoNotAlignLastPeriod=\"false\" DoNotAlignWhitespaceOnRight=\"false\" DoNotApplyAutoSpaceEAsianEng=\"false\" DoNotApplyAutoSpaceEAsianNum=\"false\" DoNotApplyColSeparatorAtNoGap=\"false\" DoNotApplyExtensionCharCompose=\"false\" DoNotApplyGridInHeaderFooter=\"false\" DoNotApplyHeaderFooterAtNoSpace=\"false\" DoNotApplyImageEffect=\"false\" DoNotApplyLinegridAtNoLinespacing=\"false\" DoNotApplyShapeComment=\"false\" DoNotApplyStrikeoutWithUnderline=\"false\" DoNotApplyVertOffsetOfForward=\"false\" DoNotApplyWhiteSpaceHeight=\"false\" DoNotFormattingAtBeneathAnchor=\"false\" DoNotHoldAnchorOfTable=\"false\" ExtendLineheightToOffset=\"false\" ExtendLineheightToParaBorderOffset=\"false\" ExtendVertLimitToPageMargins=\"false\" FixedUnderlineWidth=\"false\" OverlapBothAllowOverlap=\"false\" TreatQuotationAsLatin=\"false\" UseInnerUnderline=\"false\" UseLowercaseStrikeout=\"false\"/>");
            sw.WriteLine("</COMPATIBLEDOCUMENT>");
        }         

        private void MakeCommonBody(StreamWriter sw)
        {
            sw.WriteLine("<BODY>");
            sw.WriteLine("<SECTION Id=\"0\">");
            
            sw.WriteLine("<P ColumnBreak=\"false\" PageBreak=\"false\" ParaShape=\"13\" Style=\"0\">");
            if (m_reportType == ReportType.NotOperation || m_reportType == ReportType.Action)
                sw.WriteLine("<TEXT CharShape=\"7\">");
            else
                sw.WriteLine("<TEXT CharShape=\"8\">");

            int landscape = 0; // 용지 방향
            if (m_reportType == ReportType.ParetoSensor || m_reportType == ReportType.Bulletin)
                landscape = 1;

            MakeSecDff(sw, landscape, 1);
            sw.WriteLine("<PAGENUM FormatType=\"Digit\" Pos=\"BottomCenter\" SideChar=\"-\"/>");

            /*HEADER*/
            sw.WriteLine("<HEADER ApplyPageType=\"Both\" SeriesNum=\"0\">");
            sw.WriteLine("<PARALIST HasNumRef=\"false\" HasTextRef=\"false\" LineWrap=\"Break\" LinkListID=\"0\" LinkListIDNext=\"0\" TextDirection=\"0\" TextHeight=\"4252\" TextWidth=\"67180\" VertAlign=\"Top\">");
            sw.WriteLine("<P ParaShape=\"14\" Style=\"10\">");
            sw.WriteLine("<TEXT CharShape=\"2\">");            

            int zOrder = 0; // 틀의 zorder 값. 첫 비트가 서 있으면 글 뒤.
            if (m_reportType == ReportType.ParetoSensor)
                zOrder = 6;
            else if (m_reportType == ReportType.Detect)
                zOrder = 4;
            else if (m_reportType == ReportType.NotOperation)
                zOrder = 5;
            else if (m_reportType == ReportType.Action || m_reportType == ReportType.Bulletin)
                zOrder = 3;

            int logoHeight = 1473;
            int logoWidth = 8748;
            int logoX = 16380;
            int logoY = 2760;
            int centerX = 4374;
            int centerY = 736;
            string transMatrixE3 = "-3815.00000";
            string transMatrixE6 = "-643.00000";
            string scaMatrixE1 = "0.53407";
            string scaMatrixE3 = "3815.00000";
            string scaMatrixE5 = "0.53370";
            string scaMatrixE6 = "643.00000";
            int xPos = -3815;
            int yPos = -643;

            if (m_nSiteID == 3)
            {
                logoHeight = 1498;
                logoWidth = 10200;
                logoX = 18000;//14280;
                logoY = 2760;
                xPos = -3687;
                yPos = -541;
                centerX = 5100;
                centerY = 749;
                transMatrixE3 = "-3687.00000";
                transMatrixE6 = "-541.00000";
                scaMatrixE1 = "0.58020";
                scaMatrixE3 = "3687.00000";
                scaMatrixE5 = "0.58062";
                scaMatrixE6 = "541.00000";
            }
            else if (m_nSiteID == 100)
            {
                logoHeight = 1564;
                logoWidth = 8092;
                logoX = 14280;
                logoY = 2760;
                xPos = -3094;
                yPos = -598;
                centerX = 4046;
                centerY = 782;
                transMatrixE3 = "-3094.00000";
                transMatrixE6 = "-598.00000";
                scaMatrixE1 = "0.56667";
                scaMatrixE3 = "3094.00000";
                scaMatrixE5 = "0.56667";
                scaMatrixE6 = "598.00000";                
            }
            else if (m_nSiteID == 101)
            {
                logoHeight = 1500;
                logoWidth = 6020;
                logoX = 16380;
                logoY = 4080;
                xPos = -5178;
                yPos = -1290;
                centerX = 3010;
                centerY = 750;
                transMatrixE3 = "-5178.00000";
                transMatrixE6 = "-1290.00000";
                scaMatrixE1 = "0.36752";
                scaMatrixE3 = "5178.00000";
                scaMatrixE5 = "0.36765";
                scaMatrixE6 = "1290.00000";
            }
            else if (m_nSiteID == 201)
            {
                logoHeight = 1148;
                logoWidth = 2768;
                logoX = 16380;
                logoY = 2760;
                centerX = 4374;
                centerY = 736;
                transMatrixE3 = "-3815.00000";
                transMatrixE6 = "-643.00000";
                scaMatrixE1 = "0.53407";
                scaMatrixE3 = "3815.00000";
                scaMatrixE5 = "0.53370";
                scaMatrixE6 = "643.00000";
                xPos = -3815;
                yPos = -643;
            }

            /*LOGO*/
            sw.WriteLine("<PICTURE Reverse=\"false\">");
            sw.WriteLine("<SHAPEOBJECT InstId=\"2059310173\" Lock=\"false\" NumberingType=\"Figure\" ZOrder=\"" + zOrder + "\">");
            sw.WriteLine("<SIZE Height=\"" + logoHeight + "\" HeightRelTo=\"Absolute\" Protect=\"false\" Width=\"" + logoWidth + "\" WidthRelTo=\"Absolute\"/>");
            sw.WriteLine("<POSITION AffectLSpacing=\"false\" AllowOverlap=\"false\" FlowWithText=\"true\" HoldAnchorAndSO=\"false\" HorzAlign=\"Left\" HorzOffset=\"0\" HorzRelTo=\"Column\" TreatAsChar=\"true\" VertAlign=\"Top\" VertOffset=\"0\" VertRelTo=\"Para\"/>");
            sw.WriteLine("<OUTSIDEMARGIN Bottom=\"0\" Left=\"0\" Right=\"0\" Top=\"0\"/>");
            sw.WriteLine("<SHAPECOMMENT>그림입니다. 원본 그림의 이름: " + m_strLogo + " 원본 그림의 크기: 가로 218pixel, 세로 37pixel</SHAPECOMMENT>");
            sw.WriteLine("</SHAPEOBJECT>");
            sw.WriteLine("<SHAPECOMPONENT CurHeight=\"" + logoHeight + "\" CurWidth=\"" + logoWidth + "\" GroupLevel=\"0\" HorzFlip=\"false\" InstID=\"985568350\" OriHeight=\"" + logoY + "\" OriWidth=\"" + logoX + "\" VertFlip=\"false\" XPos=\"" + xPos + "\" YPos=\"" + yPos + "\">");
            sw.WriteLine("<ROTATIONINFO Angle=\"0\" CenterX=\"" + centerX + "\" CenterY=\"" + centerY + "\"/>");
            sw.WriteLine("<RENDERINGINFO>");
            sw.WriteLine("<TRANSMATRIX E1=\"1.00000\" E2=\"0.00000\" E3=\"" + transMatrixE3 + "\" E4=\"0.00000\" E5=\"1.00000\" E6=\"" + transMatrixE6 + "\"/>");
            sw.WriteLine("<SCAMATRIX E1=\"" + scaMatrixE1 + "\" E2=\"0.00000\" E3=\"" + scaMatrixE3 + "\" E4=\"0.00000\" E5=\"" + scaMatrixE5 + "\" E6=\"" + scaMatrixE6 + "\"/>");
            sw.WriteLine("<ROTMATRIX E1=\"1.00000\" E2=\"0.00000\" E3=\"0.00000\" E4=\"0.00000\" E5=\"1.00000\" E6=\"0.00000\"/>");
            sw.WriteLine("</RENDERINGINFO>");
            sw.WriteLine("</SHAPECOMPONENT>");
            sw.WriteLine("<IMAGERECT X0=\"0\" X1=\"" + logoX + "\" X2=\"" + logoX + "\" X3=\"0\" Y0=\"0\" Y1=\"0\" Y2=\"" + logoY + "\" Y3=\"" + logoY + "\"/>");
            sw.WriteLine("<IMAGECLIP Bottom=\"" + logoY + "\" Left=\"0\" Right=\"" + logoX + "\" Top=\"0\"/>");
            sw.WriteLine("<INSIDEMARGIN Bottom=\"0\" Left=\"0\" Right=\"0\" Top=\"0\"/>");
            sw.WriteLine("<IMAGE Alpha=\"0\" BinItem=\"1\" Bright=\"0\" Contrast=\"0\" Effect=\"RealPic\"/>");
            sw.WriteLine("<EFFECTS/>");
            sw.WriteLine("</PICTURE>");

            sw.WriteLine("<CHAR/>");
            sw.WriteLine("</TEXT>");
            sw.WriteLine("</P>");
            sw.WriteLine("</PARALIST>");
            sw.WriteLine("</HEADER>");

            /*LINE*/
            int lineWidth = 0;
            int lineHeight = 0;
            int lineZOrder = 0;
            int lineCenterX = 0;
            int lineCenterY = 0;
            if (m_reportType == ReportType.ParetoSensor)
            {
                lineWidth = 67056;
                lineHeight = 28;
                lineZOrder = 5;
                lineCenterX = 33528;
                lineCenterY = 14;
            }
            else if (m_reportType == ReportType.Detect || m_reportType == ReportType.NotOperation || m_reportType == ReportType.Action)
            {
                lineWidth = 42680;
                lineHeight = 1;
                if (m_reportType == ReportType.Action)
                    lineZOrder = 1;
                else
                    lineZOrder = 3;
                lineCenterX = 21340;
                lineCenterY = 0;
            }
            else if (m_reportType == ReportType.Bulletin)
            {
                lineWidth = 66882;
                lineHeight = 1;
                lineCenterX = 33441;
                lineCenterY = 0;
                lineZOrder = 3;
            }

            sw.WriteLine("<LINE EndX=\"100\" EndY=\"100\" IsReverseHV=\"false\" StartX=\"0\" StartY=\"0\">");
            sw.WriteLine("<SHAPEOBJECT InstId=\"1335378543\" Lock=\"false\" NumberingType=\"Figure\" TextWrap=\"InFrontOfText\" ZOrder=\"" + lineZOrder + "\">");
            sw.WriteLine("<SIZE Height=\"" + lineHeight + "\" HeightRelTo=\"Absolute\" Protect=\"false\" Width=\"" + lineWidth + "\" WidthRelTo=\"Absolute\"/>");
            sw.WriteLine("<POSITION AffectLSpacing=\"false\" AllowOverlap=\"true\" FlowWithText=\"false\" HoldAnchorAndSO=\"false\" HorzAlign=\"Left\" HorzOffset=\"8802\" HorzRelTo=\"Paper\" TreatAsChar=\"false\" VertAlign=\"Top\" VertOffset=\"7322\" VertRelTo=\"Paper\"/>");
            sw.WriteLine("<OUTSIDEMARGIN Bottom=\"0\" Left=\"0\" Right=\"0\" Top=\"0\"/>");
            sw.WriteLine("</SHAPEOBJECT>");
            sw.WriteLine("<DRAWINGOBJECT>");
            sw.WriteLine("<SHAPECOMPONENT CurHeight=\"" + lineHeight + "\" CurWidth=\"" + lineWidth + "\" GroupLevel=\"0\" HorzFlip=\"false\" InstID=\"261636720\" OriHeight=\"100\" OriWidth=\"100\" VertFlip=\"false\" XPos=\"0\" YPos=\"28\">");
            sw.WriteLine("<ROTATIONINFO Angle=\"0\" CenterX=\"" + lineCenterX + "\" CenterY=\"" + lineCenterY + "\"/>");
            sw.WriteLine("<RENDERINGINFO>");
            sw.WriteLine("<TRANSMATRIX E1=\"1.00000\" E2=\"0.00000\" E3=\"0.00000\" E4=\"0.00000\" E5=\"1.00000\" E6=\"28.00000\"/>");
            sw.WriteLine("<SCAMATRIX E1=\"670.56000\" E2=\"0.00000\" E3=\"0.00000\" E4=\"0.00000\" E5=\"-0.28000\" E6=\"0.00000\"/>");
            sw.WriteLine("<ROTMATRIX E1=\"1.00000\" E2=\"0.00000\" E3=\"0.00000\" E4=\"0.00000\" E5=\"1.00000\" E6=\"0.00000\"/>");
            sw.WriteLine("</RENDERINGINFO>");
            sw.WriteLine("</SHAPECOMPONENT>");
            sw.WriteLine("<LINESHAPE Alpha=\"0\" EndCap=\"Flat\" HeadSize=\"MediumMedium\" HeadStyle=\"Normal\" Style=\"Solid\" TailSize=\"MediumMedium\" TailStyle=\"Normal\" Width=\"283\"/>");
            sw.WriteLine("<SHADOW Alpha=\"0\" Color=\"11711154\" OffsetX=\"0\" OffsetY=\"0\" Type=\"0\"/>");
            sw.WriteLine("</DRAWINGOBJECT>");
            sw.WriteLine("</LINE>");

            sw.WriteLine("<CHAR>" + m_strTitle + "</CHAR>");
            sw.WriteLine("</TEXT>");
            sw.WriteLine("</P>");

            string strType = IFacility.GetFacilityTypeString(m_facilityType);
            if (m_facilityType == IFacility.FacilityType.PSM_SENSOR && m_nSiteID == 201)
                strType = "가스";
            if (m_reportType == ReportType.ParetoSensor)
            {
                MakeTagP(sw, 13, 14, strType + " 탐지횟수가 높은 센서 및 위치를 보여줍니다. ", false, false);
                MakeTagP(sw, 13, 14, "발생빈도가 높은 순으로 왼쪽에서 오른쪽으로 나타납니다.", false, false);
            }
            else if (m_reportType == ReportType.Detect)
            {
                MakeTagP(sw, 13, 13, "각 센서들이 탐지한 " + strType + "빈도를 표시합니다. ", false, false);
                MakeTagP(sw, 13, 13, "센서 오류 및 특정 상황에 의한 오작동을 포함한 빈도입니다.", false, false);
            }
            else if (m_reportType == ReportType.NotOperation)
            {
                MakeTagP(sw, 13, 16, "", false, false);
                MakeTagP(sw, 13, 16, "설비 영역별로 탐지된 " + strType + "정보들에 대한 처리 이력을 표시합니다.", false, false);
            }
            else if (m_reportType == ReportType.Action)
            {
                MakeTagP(sw, 13, 10, "", false, false);
                MakeTagP(sw, 13, 10, "담당자의 대응이력을 표시합니다.", false, false);
            }
            else if (m_reportType == ReportType.Bulletin)
            {
                //MakeTagP(sw, 13, 11, "", false, false);
                MakeTagP(sw, 13, 11, "현재 실행중인 SOP의 진행 상황을 보여줍니다.", false, false);
            }

            int line2ParaShape = 12;
            int line2CharShape = 6;
            if (m_reportType == ReportType.NotOperation)
            {
                line2ParaShape = 13;
                line2CharShape = 14;
            }
            else if (m_reportType == ReportType.Action)
            {
                line2ParaShape = 13;
                line2CharShape = 10;
                lineZOrder = 2;
                lineWidth = 42236;
                lineCenterX = 21118;
            }
            else if (m_reportType == ReportType.Bulletin)
            {
                line2ParaShape = 13;
                line2CharShape = 11;
                lineWidth = 66772;
                lineHeight = 30;
                lineCenterX = 33386;
                lineCenterY = 15;
            }

            sw.WriteLine("<P ParaShape=\"" + line2ParaShape + "\" Style=\"0\">");
            sw.WriteLine("<TEXT CharShape=\"" + line2CharShape + "\">");
            sw.WriteLine("<LINE EndX=\"100\" EndY=\"100\" IsReverseHV=\"false\" StartX=\"0\" StartY=\"0\">");
            sw.WriteLine("<SHAPEOBJECT InstId=\"1335378547\" Lock=\"false\" NumberingType=\"Figure\" TextWrap=\"InFrontOfText\" ZOrder=\"" + lineZOrder + "\">");
            sw.WriteLine("<SIZE Height=\"" + lineHeight + "\" HeightRelTo=\"Absolute\" Protect=\"false\" Width=\"" + lineWidth + "\" WidthRelTo=\"Absolute\"/>");
            sw.WriteLine("<POSITION AffectLSpacing=\"false\" AllowOverlap=\"true\" FlowWithText=\"false\" HoldAnchorAndSO=\"false\" HorzAlign=\"Left\" HorzOffset=\"8898\" HorzRelTo=\"Paper\" TreatAsChar=\"false\" VertAlign=\"Top\" VertOffset=\"17350\" VertRelTo=\"Paper\"/>");
            sw.WriteLine("<OUTSIDEMARGIN Bottom=\"0\" Left=\"0\" Right=\"0\" Top=\"0\"/>");
            sw.WriteLine("</SHAPEOBJECT>");
            sw.WriteLine("<DRAWINGOBJECT>");
            sw.WriteLine("<SHAPECOMPONENT CurHeight=\"" + lineHeight + "\" CurWidth=\"" + lineWidth + "\" GroupLevel=\"0\" HorzFlip=\"false\" InstID=\"261636724\" OriHeight=\"100\" OriWidth=\"100\" VertFlip=\"false\" XPos=\"0\" YPos=\"0\">");
            sw.WriteLine("<ROTATIONINFO Angle=\"0\" CenterX=\"" + lineCenterX + "\" CenterY=\"" + lineCenterY + "\"/>");
            sw.WriteLine("<RENDERINGINFO>");
            sw.WriteLine("<TRANSMATRIX E1=\"1.00000\" E2=\"0.00000\" E3=\"0.00000\" E4=\"0.00000\" E5=\"1.00000\" E6=\"0.00000\"/>");
            sw.WriteLine("<SCAMATRIX E1=\"669.60000\" E2=\"0.00000\" E3=\"0.00000\" E4=\"0.00000\" E5=\"0.28000\" E6=\"0.00000\"/>");
            sw.WriteLine("<ROTMATRIX E1=\"1.00000\" E2=\"0.00000\" E3=\"0.00000\" E4=\"0.00000\" E5=\"1.00000\" E6=\"0.00000\"/>");
            sw.WriteLine("</RENDERINGINFO>");
            sw.WriteLine("</SHAPECOMPONENT>");
            sw.WriteLine("<LINESHAPE Alpha=\"0\" EndCap=\"Flat\" HeadSize=\"MediumMedium\" HeadStyle=\"Normal\" Style=\"Solid\" TailSize=\"MediumMedium\" TailStyle=\"Normal\" Width=\"42\"/>");
            sw.WriteLine("<SHADOW Alpha=\"0\" Color=\"11711154\" OffsetX=\"0\" OffsetY=\"0\" Type=\"0\"/>");
            sw.WriteLine("</DRAWINGOBJECT>");
            sw.WriteLine("</LINE>");
            sw.WriteLine("<CHAR/>");
            sw.WriteLine("</TEXT>");
            sw.WriteLine("</P>");

            if (m_reportType == ReportType.Action)
            {
                MakeTagP(sw, 13, 10, "", false, false);
                MakeTagP(sw, 15, 6, "", false, false);
                MakeTagP(sw, 15, 6, "1. 조회", false, false);

                ArrayList arrList = new ArrayList();
                arrList.Add(8);
                arrList.Add("발생 시간 | ");
                arrList.Add(11);
                arrList.Add(m_strDate);
                MakeTagP(sw, 12, arrList);

                if (m_facilityType != IFacility.FacilityType.Earthquake)
                {
                    arrList = new ArrayList();
                    arrList.Add(8);
                    arrList.Add("조회 범위 | ");
                    arrList.Add(11);
                    arrList.Add(m_strTarget);
                    MakeTagP(sw, 12, arrList);

                    arrList = new ArrayList();
                    arrList.Add(8);
                    arrList.Add("발생 장소 | ");
                    arrList.Add(11);
                    arrList.Add(m_strTarget2);
                    MakeTagP(sw, 12, arrList); 
                }
            }
            else if (m_reportType == ReportType.Bulletin)
            {
                MakeTagP(sw, 13, 11, "", false, false);
                MakeTagP(sw, 15, 7, "", false, false);
                MakeTagP(sw, 15, 7, "", false, false);

                ArrayList arrList = new ArrayList();
                arrList.Add(9);
                arrList.Add("1. SOP명 | ");
                arrList.Add(10);
                arrList.Add(m_strSopName);
                MakeTagP(sw, 15, arrList);

                arrList = new ArrayList();
                arrList.Add(9);
                arrList.Add("2. 진행총괄 | ");
                arrList.Add(10);
                arrList.Add(m_strProcManager);
                MakeTagP(sw, 15, arrList);

                arrList = new ArrayList();
                arrList.Add(9);
                arrList.Add("3. 상황발생 위치 | ");
                arrList.Add(10);
                arrList.Add(m_strLocation);
                MakeTagP(sw, 15, arrList);

                arrList = new ArrayList();
                arrList.Add(9);
                arrList.Add("4. 상황발생 시간 | ");
                arrList.Add(10);
                arrList.Add(m_strDate);
                MakeTagP(sw, 15, arrList);

                arrList = new ArrayList();
                arrList.Add(9);
                arrList.Add("5. 총 소요시간 | ");
                arrList.Add(10);
                arrList.Add(m_strTimeRequired);
                MakeTagP(sw, 15, arrList);

                arrList = new ArrayList();
                arrList.Add(9);
                arrList.Add("6. 최종상태 | ");
                arrList.Add(10);
                arrList.Add(m_strEndState);
                MakeTagP(sw, 15, arrList);
            }
            else
            {
                MakeTagP(sw, 15, 11, "", false, false);
                MakeTagP(sw, 15, 11, "1. 조회 기간 및 범위", false, false);

                ArrayList arrList = new ArrayList();
                arrList.Add(12);
                arrList.Add("조회 기간");
                arrList.Add(9);
                arrList.Add(" | " + m_strDate);
                MakeTagP(sw, 12, arrList);

                if (m_facilityType != IFacility.FacilityType.Earthquake)
                {
                    arrList = new ArrayList();
                    arrList.Add(12);
                    arrList.Add("조회 범위");
                    arrList.Add(9);
                    arrList.Add(" | " + m_strTarget);
                    MakeTagP(sw, 12, arrList); 
                }
            }

            MakeTagP(sw, 12, 7, "", false, false);
            if (m_reportType == ReportType.ParetoSensor)
                MakeTagP(sw, 12, 7, "2. Pareto Chart(센서별)", false, false);
            else if (m_reportType == ReportType.Detect)
                MakeTagP(sw, 12, 7, "2. " + strType + " 탐지 빈도", false, false);
            else if (m_reportType == ReportType.NotOperation)
                MakeTagP(sw, 12, 6, "2. 오작동 처리                        ", false, false);
            else if (m_reportType == ReportType.Action)
            {
                MakeTagP(sw, 12, 6, "2. 대응 이력", false, false);
                MakeTagP(sw, 12, 6, "", false, false);
            }
            else if (m_reportType == ReportType.Bulletin)
            {
                MakeTagP(sw, 12, 7, "7. 상황판 이력", false, false);
            }

            if (m_reportType == ReportType.ParetoSensor)
                MakeImageTag(sw, 12, 7, 0, "ParetoSensorImage", "ParetoSensor.bmp", 7, 2, 240, 67, 34016, 9565, 34380, 131580, 0.95056);
            else if (m_reportType == ReportType.Detect)
            {
                MakeImageTag(sw, 12, 7, 0, "", "Detect.bmp", 2, 2, 150, 37, 21260, 6378, 30660, 129300);
            }
            else if (m_reportType == ReportType.NotOperation)
            {
                MakeImageTag(sw, 12, 6, 0, "", "NotOperationPageLegend2.png", 2, 3, 77, 4, 10961, 525, 1140, 21840, "                                     ");
                MakeImageTag(sw, 12, 5, 0, "", "Malfunction.bmp", 6, 2, 150, 37, 21260, 6378, 27600, 127800);
            }

            if (m_reportType == ReportType.ParetoSensor)
            {
                MakeTagP(sw, 12, 7, "3. " + strType + " 탐지 센서 리스트", false, true);
                MakeTagP(sw, 12, 7, "", false, false);

                /*Grid*/
                MakeTable(sw, ReportType.ParetoSensor);

                if (m_facilityType == IFacility.FacilityType.PSM_SENSOR)
                {
                    //MakeTagP(sw, 12, 7, "4. Pareto Chart(탱크별)", false, true);
                    //MakeImageTag(sw, 12, 7, 0, "ParetoTank", "ParetoTank.bmp", 7, 5, 240, 67, 34016, 9565, 34380, 131580, 0.95056);

                    //MakeTagP(sw, 12, 7, "5. " + strType + " 탐지 탱크 리스트", false, true);
                    //MakeTagP(sw, 12, 7, "", false, false);
                    //MakeTable(sw, "ParetoTank");

                    MakeTagP(sw, 12, 7, "6. Pareto Chart(위치별)", false, true);
                    MakeTagP(sw, 12, 7, "", false, false);
                    MakeImageTag(sw, 12, 7, 0, "ParetoEquipZone", "ParetoEquipZone.bmp", 7, 3, 240, 67, 34016, 9565, 34380, 131580, 0.95056);

                    MakeTagP(sw, 12, 7, "7. 누출 탐지 위치 리스트", false, true);
                    MakeTagP(sw, 12, 7, "", false, false);
                    MakeTable(sw, ReportType.ParetoEquipZone);

                    //MakeTagP(sw, 12, 7, "8. Pareto Chart(물질별)", false, true);
                    //MakeTagP(sw, 12, 7, "", false, false);
                    //MakeImageTag(sw, 12, 7, 0, "ParetoMaterial", "ParetoMaterial.bmp", 7, 4, 240, 67, 34016, 9565, 34380, 131580, 0.95056);

                    //MakeTagP(sw, 12, 7, "9. 누출 탐지 물질 리스트", false, true);
                    //MakeTagP(sw, 12, 7, "", false, false);
                    //MakeTable(sw, "ParetoMaterial");
                }
                else
                {
                    if (m_nSiteID != 200)
                    {
                        MakeTagP(sw, 12, 7, "4. Pareto Chart(위치별)", false, true);
                        MakeImageTag(sw, 12, 7, 0, "ParetoEquipZoneImage", "ParetoEquipZone.bmp", 7, 2, 240, 67, 34016, 9565, 34380, 131580, 0.95056);

                        MakeTagP(sw, 12, 7, "5. " + strType + " 탐지 위치 리스트", false, true);
                        MakeTagP(sw, 12, 7, "", false, false);
                        MakeTable(sw, ReportType.ParetoEquipZone); 
                    }
                }

                sw.WriteLine("</SECTION>");
            }
            else if (m_reportType == ReportType.Detect)
            {
                sw.WriteLine("</SECTION>");
                sw.WriteLine("<SECTION Id=\"1\">");
                sw.WriteLine("<P ColumnBreak=\"false\" PageBreak=\"false\" ParaShape=\"12\" Style=\"0\">");
                sw.WriteLine("<TEXT CharShape=\"7\">");
                MakeSecDff(sw, 1, 0);

                int dataMinIndex = (m_nDataIndex * m_nDataMaxCount);
                if (dataMinIndex < 0)
                    dataMinIndex = 0;
                int dataMaxIndex = (m_nDataIndex * m_nDataMaxCount) + m_nDataMaxCount -1;
                if (dataMaxIndex >= m_dicData[this.m_reportType].Count)
                    dataMaxIndex = m_dicData[this.m_reportType].Count -1;

                //string beginDate = "";
                //string endDate = "";
                // 날짜
                //if (m_facilityType == IFacility.FacilityType.PSM_SENSOR)
                //{
                //    DetectPSM detect = null;
                //    if (m_dicData[this.m_reportType.ToString()].Count > 0)
                //    {
                //        detect = m_dicData[this.m_reportType.ToString()][dataMinIndex] as DetectPSM;
                //        beginDate = detect.Date;

                //        detect = m_dicData[this.m_reportType.ToString()][dataMaxIndex] as DetectPSM;
                //        endDate = detect.Date;
                //    }
                //}
                //else if (m_facilityType == IFacility.FacilityType.Earthquake)
                //{
                //    DetectEarthquake detect = null;
                //    if (m_dicData[this.m_reportType.ToString()].Count > 0)
                //    {
                //        detect = m_dicData[this.m_reportType.ToString()][dataMinIndex] as DetectEarthquake;
                //        beginDate = detect.Date;

                //        detect = m_dicData[this.m_reportType.ToString()][dataMaxIndex] as DetectEarthquake;
                //        endDate = detect.Date;
                //    }
                //}
                //else if (m_facilityType == IFacility.FacilityType.TEMPERATURE_HUMIDITY)
                //{
                //    DetectTH detect = null;
                //    if (m_dicData[this.m_reportType.ToString()].Count > 0)
                //    {
                //        detect = m_dicData[this.m_reportType.ToString()][dataMinIndex] as DetectTH;
                //        beginDate = detect.Date;

                //        detect = m_dicData[this.m_reportType.ToString()][dataMaxIndex] as DetectTH;
                //        endDate = detect.Date;
                //    }
                //}
                //else
                //{
                //    Detect detect = null;
                //    if (m_dicData[this.m_reportType.ToString()].Count > 0)
                //    {
                //        detect = m_dicData[this.m_reportType.ToString()][dataMinIndex] as Detect;
                //        beginDate = detect.Date;

                //        detect = m_dicData[this.m_reportType.ToString()][dataMaxIndex] as Detect;
                //        endDate = detect.Date;
                //    }
                //}
                
                sw.WriteLine("<CHAR>3. " + strType + " 탐지 센서구역 리스트 </CHAR>");
                sw.WriteLine("</TEXT>");
                sw.WriteLine("</P>");

                //if (m_strType != "누출")
                //    MakeTagP(sw, 12, 7, "", false, false);
                MakeTable(sw, ReportType.Detect);
                sw.WriteLine("</SECTION>");
            }
            else if (m_reportType == ReportType.NotOperation)
            {
                MakeTagP(sw, 12, 6, "3. 오작동 발생 센서구역 리스트", false, true);
                MakeTable(sw, ReportType.NotOperation);
                sw.WriteLine("</SECTION>");
            }
            else if (m_reportType == ReportType.Action)
            {
                MakeTable(sw, ReportType.Action);
                MakeTagP(sw, 12, 5, "", false, false);
                MakeTagP(sw, 12, 5, "", false, false);
                MakeTagP(sw, 12, 6, "3. 메모", false, false);
                MakeTagP(sw, 12, 9, m_strMemo, false, false);
                sw.WriteLine("</SECTION>");
            }
            else if (m_reportType == ReportType.Bulletin)
            {
                MakeTable(sw, ReportType.Bulletin);
                MakeTagP(sw, 12, 6, "", false, false);
                MakeTagP(sw, 12, 7, "8. 상황판 세부이력", false, true);
                MakeTable(sw, ReportType.BulletinDetail);
                MakeTagP(sw, 12, 6, "", false, false);
                MakeTagP(sw, 12, 6, "", false, false);
                MakeTagP(sw, 12, 5, "", false, false);
                sw.WriteLine("</SECTION>");
            }

            sw.WriteLine("</BODY>");
        }

        private void MakeSecDff(StreamWriter sw, int landscape, int outlineShape)
        {
            /*SECDEF*/
            sw.WriteLine("<SECDEF CharGrid=\"0\" FirstBorder=\"false\" FirstFill=\"false\" LineGrid=\"0\" OutlineShape=\"" + outlineShape + "\" SpaceColumns=\"1134\" TabStop=\"8000\" TextDirection=\"0\" TextVerticalWidthHead=\"0\">");
            sw.WriteLine("<STARTNUMBER Equation=\"0\" Figure=\"0\" Page=\"0\" PageStartsOn=\"Both\" Table=\"0\"/>");
            sw.WriteLine("<HIDE Border=\"false\" EmptyLine=\"false\" Fill=\"false\" Footer=\"false\" Header=\"false\" MasterPage=\"false\" PageNumPos=\"false\"/>");            
            sw.WriteLine("<PAGEDEF GutterType=\"LeftOnly\" Height=\"84188\" Landscape=\"" + landscape + "\" Width=\"59528\">");
            sw.WriteLine("<PAGEMARGIN Bottom=\"4252\" Footer=\"4252\" Gutter=\"0\" Header=\"4252\" Left=\"8504\" Right=\"8504\" Top=\"5668\"/>");
            sw.WriteLine("</PAGEDEF>");
            sw.WriteLine("<FOOTNOTESHAPE>");
            sw.WriteLine("<AUTONUMFORMAT SuffixChar=\")\" Superscript=\"false\" Type=\"Digit\"/>");
            sw.WriteLine("<NOTELINE Length=\"5cm\" Type=\"Solid\" Width=\"0.12mm\"/>");
            sw.WriteLine("<NOTESPACING AboveLine=\"850\" BelowLine=\"567\" BetweenNotes=\"283\"/>");
            sw.WriteLine("<NOTENUMBERING NewNumber=\"1\" Type=\"Continuous\"/>");
            sw.WriteLine("<NOTEPLACEMENT BeneathText=\"false\" Place=\"EachColumn\"/>");
            sw.WriteLine("</FOOTNOTESHAPE>");
            sw.WriteLine("<ENDNOTESHAPE>");
            sw.WriteLine("<AUTONUMFORMAT SuffixChar=\")\" Superscript=\"false\" Type=\"Digit\"/>");
            sw.WriteLine("<NOTELINE Length=\"14692344\" Type=\"Solid\" Width=\"0.12mm\"/>");
            sw.WriteLine("<NOTESPACING AboveLine=\"850\" BelowLine=\"567\" BetweenNotes=\"0\"/>");
            sw.WriteLine("<NOTENUMBERING NewNumber=\"1\" Type=\"Continuous\"/>");
            sw.WriteLine("<NOTEPLACEMENT BeneathText=\"false\" Place=\"EndOfDocument\"/>");
            sw.WriteLine("</ENDNOTESHAPE>");
            sw.WriteLine("<PAGEBORDERFILL BorferFill=\"1\" FillArea=\"Paper\" FooterInside=\"false\" HeaderInside=\"false\" TextBorder=\"true\" Type=\"Both\">");
            sw.WriteLine("<PAGEOFFSET Bottom=\"1417\" Left=\"1417\" Right=\"1417\" Top=\"1417\"/>");
            sw.WriteLine("</PAGEBORDERFILL>");
            sw.WriteLine("<PAGEBORDERFILL BorferFill=\"1\" FillArea=\"Paper\" FooterInside=\"false\" HeaderInside=\"false\" TextBorder=\"true\" Type=\"Even\">");
            sw.WriteLine("<PAGEOFFSET Bottom=\"1417\" Left=\"1417\" Right=\"1417\" Top=\"1417\"/>");
            sw.WriteLine("</PAGEBORDERFILL>");
            sw.WriteLine("<PAGEBORDERFILL BorferFill=\"1\" FillArea=\"Paper\" FooterInside=\"false\" HeaderInside=\"false\" TextBorder=\"true\" Type=\"Odd\">");
            sw.WriteLine("<PAGEOFFSET Bottom=\"1417\" Left=\"1417\" Right=\"1417\" Top=\"1417\"/>");
            sw.WriteLine("</PAGEBORDERFILL>");
            sw.WriteLine("</SECDEF>");
            sw.WriteLine("<COLDEF Count=\"1\" Layout=\"Left\" SameGap=\"0\" SameSize=\"true\" Type=\"Newspaper\"/>");
        }
        private void MakeImageTag(StreamWriter sw, int paraShape, int charShape, int style, string bookmarkName,  string imageName, int zorder, int imageID, int widthmm, int heightmm, int centerX, int centerY, int bottom, int right, string text = "")
        {
            int nWidthmm = widthmm;
            int nHeightmm = heightmm;
            double inch = 0.0393701;
            int nHwpUnit = 7200; // 1/7200인치로 표현된 한글 내부 단위
            
            double chartWidth = Math.Round((nWidthmm * inch) * nHwpUnit);
            double chartHegith = Math.Round((nHeightmm * inch) * nHwpUnit);

            sw.WriteLine("<P ParaShape=\"" + paraShape + "\" Style=\"0\">");
            sw.WriteLine("<TEXT CharShape=\"" + charShape + "\">");
            if (text.Length > 0)
                sw.WriteLine("<CHAR>" + text + "</CHAR>");
            if (bookmarkName.Length > 0)
                sw.WriteLine("<BOOKMARK Name=\"" + bookmarkName + "\"/>");
            sw.WriteLine("<PICTURE Reverse=\"false\">");
            sw.WriteLine("<SHAPEOBJECT InstId=\"2045758955\" Lock=\"false\" NumberingType=\"Figure\" ZOrder=\"" + zorder + "\">");
            sw.WriteLine("<SIZE Height=\"" + chartHegith + "\" HeightRelTo=\"Absolute\" Protect=\"false\" Width=\"" + chartWidth + "\" WidthRelTo=\"Absolute\"/>");
            sw.WriteLine("<POSITION AffectLSpacing=\"false\" AllowOverlap=\"false\" FlowWithText=\"true\" HoldAnchorAndSO=\"false\" HorzAlign=\"Left\" HorzOffset=\"0\" HorzRelTo=\"Column\" TreatAsChar=\"true\" VertAlign=\"Top\" VertOffset=\"0\" VertRelTo=\"Para\"/>");
            sw.WriteLine("<OUTSIDEMARGIN Bottom=\"0\" Left=\"0\" Right=\"0\" Top=\"0\"/>");
            sw.WriteLine("<SHAPECOMMENT>그림입니다. 원본 그림의 이름: " + imageName + " 원본 그림의 크기: 가로 1683pixel, 세로 290pixel</SHAPECOMMENT>");
            sw.WriteLine("</SHAPEOBJECT>");
            sw.WriteLine("<SHAPECOMPONENT GroupLevel=\"0\" HorzFlip=\"false\" InstID=\"972017132\" OriHeight=\"" + chartHegith + "\" OriWidth=\"" + chartWidth + "\" VertFlip=\"false\" XPos=\"0\" YPos=\"0\">");
            sw.WriteLine("<ROTATIONINFO Angle=\"0\" CenterX=\"" + centerX + "\" CenterY=\"" + centerY + "\"/>");
            sw.WriteLine("<RENDERINGINFO>");
            sw.WriteLine("<TRANSMATRIX E1=\"1.00000\" E2=\"0.00000\" E3=\"0.00000\" E4=\"0.00000\" E5=\"1.00000\" E6=\"0.00000\"/>");
            sw.WriteLine("<SCAMATRIX E1=\"1.00000\" E2=\"0.00000\" E3=\"0.00000\" E4=\"0.00000\" E5=\"1.00000\" E6=\"0.00000\"/>");
            sw.WriteLine("<ROTMATRIX E1=\"1.00000\" E2=\"0.00000\" E3=\"0.00000\" E4=\"0.00000\" E5=\"1.00000\" E6=\"0.00000\"/>");
            sw.WriteLine("</RENDERINGINFO>");
            sw.WriteLine("</SHAPECOMPONENT>");
            sw.WriteLine("<IMAGERECT X0=\"0\" X1=\"" + chartWidth + "\" X2=\"" + chartWidth + "\" X3=\"0\" Y0=\"0\" Y1=\"0\" Y2=\"" + chartHegith + "\" Y3=\"" + chartHegith + "\"/>");
            sw.WriteLine("<IMAGECLIP Bottom=\"" + bottom + "\" Left=\"0\" Right=\"" + right + "\" Top=\"0\"/>");
            sw.WriteLine("<INSIDEMARGIN Bottom=\"0\" Left=\"0\" Right=\"0\" Top=\"0\"/>");
            sw.WriteLine("<IMAGE Alpha=\"0\" BinItem=\"" + imageID + "\" Bright=\"0\" Contrast=\"0\" Effect=\"RealPic\"/>");
            sw.WriteLine("<EFFECTS/>");
            sw.WriteLine("</PICTURE>");
            sw.WriteLine("<CHAR/>");
            sw.WriteLine("</TEXT>");
            sw.WriteLine("</P>");
        }

        private void MakeImageTag(StreamWriter sw, int paraShape, int charShape, int style, string bookmarkName, string imageName, int zorder, int imageID, int widthmm, int heightmm, int centerX, int centerY, int bottom, int right, double scaMatrix, string text = "")
        {
            int nWidthmm = widthmm;
            int nHeightmm = heightmm;
            double inch = 0.0393701;
            int nHwpUnit = 7200; // 1/7200인치로 표현된 한글 내부 단위

            double chartWidth = Math.Round((nWidthmm * inch) * nHwpUnit);
            double chartHegith = Math.Round((nHeightmm * inch) * nHwpUnit);

            sw.WriteLine("<P ParaShape=\"" + paraShape + "\" Style=\"0\">");
            sw.WriteLine("<TEXT CharShape=\"" + charShape + "\">");
            if (text.Length > 0)
                sw.WriteLine("<CHAR>" + text + "</CHAR>");
            if (bookmarkName.Length > 0)
                sw.WriteLine("<BOOKMARK Name=\"" + bookmarkName + "\"/>");
            sw.WriteLine("<PICTURE Reverse=\"false\">");
            sw.WriteLine("<SHAPEOBJECT InstId=\"2045758955\" Lock=\"false\" NumberingType=\"Figure\" ZOrder=\"" + zorder + "\">");
            sw.WriteLine("<SIZE Height=\"" + chartHegith + "\" HeightRelTo=\"Absolute\" Protect=\"false\" Width=\"" + chartWidth + "\" WidthRelTo=\"Absolute\"/>");
            sw.WriteLine("<POSITION AffectLSpacing=\"false\" AllowOverlap=\"false\" FlowWithText=\"true\" HoldAnchorAndSO=\"false\" HorzAlign=\"Left\" HorzOffset=\"0\" HorzRelTo=\"Column\" TreatAsChar=\"true\" VertAlign=\"Top\" VertOffset=\"0\" VertRelTo=\"Para\"/>");
            sw.WriteLine("<OUTSIDEMARGIN Bottom=\"0\" Left=\"0\" Right=\"0\" Top=\"0\"/>");
            sw.WriteLine("<SHAPECOMMENT>그림입니다. 원본 그림의 이름: " + imageName + " 원본 그림의 크기: 가로 1683pixel, 세로 290pixel</SHAPECOMMENT>");
            sw.WriteLine("</SHAPEOBJECT>");
            sw.WriteLine("<SHAPECOMPONENT GroupLevel=\"0\" HorzFlip=\"false\" InstID=\"972017132\" OriHeight=\"" + chartHegith + "\" OriWidth=\"" + chartWidth + "\" VertFlip=\"false\" XPos=\"0\" YPos=\"0\">");
            sw.WriteLine("<ROTATIONINFO Angle=\"0\" CenterX=\"" + centerX + "\" CenterY=\"" + centerY + "\"/>");
            sw.WriteLine("<RENDERINGINFO>");
            sw.WriteLine("<TRANSMATRIX E1=\"1.00000\" E2=\"0.00000\" E3=\"0.00000\" E4=\"0.00000\" E5=\"1.00000\" E6=\"0.00000\"/>");
            sw.WriteLine("<SCAMATRIX E1=\"1.00000\" E2=\"0.00000\" E3=\"0.00000\" E4=\"0.00000\" E5=\"" + scaMatrix + "\" E6=\"0.00000\"/>");
            sw.WriteLine("<ROTMATRIX E1=\"1.00000\" E2=\"0.00000\" E3=\"0.00000\" E4=\"0.00000\" E5=\"1.00000\" E6=\"0.00000\"/>");
            sw.WriteLine("</RENDERINGINFO>");
            sw.WriteLine("</SHAPECOMPONENT>");
            sw.WriteLine("<IMAGERECT X0=\"0\" X1=\"" + chartWidth + "\" X2=\"" + chartWidth + "\" X3=\"0\" Y0=\"0\" Y1=\"0\" Y2=\"" + chartHegith + "\" Y3=\"" + chartHegith + "\"/>");
            sw.WriteLine("<IMAGECLIP Bottom=\"" + bottom + "\" Left=\"0\" Right=\"" + right + "\" Top=\"0\"/>");
            sw.WriteLine("<INSIDEMARGIN Bottom=\"0\" Left=\"0\" Right=\"0\" Top=\"0\"/>");
            sw.WriteLine("<IMAGE Alpha=\"0\" BinItem=\"" + imageID + "\" Bright=\"0\" Contrast=\"0\" Effect=\"RealPic\"/>");
            sw.WriteLine("<EFFECTS/>");
            sw.WriteLine("</PICTURE>");
            sw.WriteLine("<CHAR/>");
            sw.WriteLine("</TEXT>");
            sw.WriteLine("</P>");
        }

        private void ReadPNGFile(StreamWriter sw, int nID, string strFilePath)
        {
            if (!File.Exists(strFilePath))
                return;

            FileStream fs = new FileStream(strFilePath, FileMode.Open);
            BinaryReader reader = new BinaryReader(fs);

            byte[] bytes = reader.ReadBytes((int)fs.Length);
            string strBase64 = System.Convert.ToBase64String(bytes);
            reader.Close();
            
            sw.WriteLine("<BINDATA Encoding=\"Base64\" Id=\"" + nID.ToString() + "\">");
            sw.WriteLine(strBase64);
            sw.WriteLine("</BINDATA>");
        }
        private void ReadBMPFile(StreamWriter sw, int nID, string strFilePath)
        {
            FileStream fs = new FileStream(strFilePath, FileMode.Open);
            BinaryReader reader = new BinaryReader(fs);

            System.Diagnostics.Trace.WriteLine("File 크기 : " + fs.Length.ToString());
            byte[] bytes = reader.ReadBytes((int)fs.Length);
            string strBase64 = System.Convert.ToBase64String(bytes);
            System.Diagnostics.Trace.WriteLine("Base64 크기 : " + strBase64.Length.ToString());
            reader.Close();
            
            sw.WriteLine("<BINDATA Compress=\"false\" Encoding=\"Base64\" Id=\"" + nID.ToString() + "\">");
            sw.Write(strBase64);
            sw.WriteLine("</BINDATA>");
        }
                
        private Dictionary<ReportType, List<TableColumns>> m_dicDefineColumns = new Dictionary<ReportType, List<TableColumns>>();
        private bool LoadDefineColumn()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT ReportType, HeaderText, ColumnWidthRatio ");
            sb.Append("  FROM DefineReportColumnsByFacilityType ");
            sb.AppendFormat(" WHERE FacilityTypeID = {0}", (int)m_facilityType);
            if (m_reportType == ReportType.ParetoSensor)
            {
                // 탐지분석은 센서별, 위치별 두가지 형식을 제공한다.
                sb.AppendFormat("   AND ReportType IN ({0}, {1})", (int)m_reportType, (int)ReportType.ParetoEquipZone);
            }
            else
                sb.AppendFormat("   AND ReportType = {0}", (int)m_reportType);
            sb.Append(" ORDER BY ColumnIndex ");

            ArrayList arrResult = m_dbMgr.GetResultData(sb.ToString());
            if (arrResult == null || arrResult.Count == 0)
                return false;

            for (int i = 0; i < arrResult.Count; i+=3)
            {
                int nReportType = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strHeaderText = WebDBManager.GetStringField(arrResult[i + 1]);
                int nColumnWidthRatio = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);

                TableColumns col = new TableColumns();
                col.ColumnName = strHeaderText;
                col.ColumnWidthRatio = nColumnWidthRatio;

                if (!m_dicDefineColumns.ContainsKey((ReportType)nReportType))
                    m_dicDefineColumns.Add((ReportType)nReportType, new List<TableColumns>());

                m_dicDefineColumns[(ReportType)nReportType].Add(col);
            }

            return true;
        }
    }
}
