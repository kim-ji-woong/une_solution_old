using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows.Forms;
using System.Drawing;

namespace SoilMan.Data
{
    public class ExcelReport
    {
        private static int GrayCell = ColorTranslator.ToOle(Color.FromArgb(217, 217, 217));
        private static int BlueCell = ColorTranslator.ToOle(Color.FromArgb(183, 222, 232));
        private static int GreenCell = ColorTranslator.ToOle(Color.FromArgb(196, 215, 155));
        private static int LightGreenCell = ColorTranslator.ToOle(Color.FromArgb(235, 241, 222));
        private static int OrangeCell = ColorTranslator.ToOle(Color.FromArgb(252, 213, 180));


        private static long m_nYear = 30;
        public static bool Export(string strPath, IWin32Window owner, DataGridView gridArea, DataGridView gridPublicCost, DataGridView gridCondition, DataGridView gridValueCost, DataGridView gridCapacity, DataGridView gridValue, DataGridView gridTotalValue,DataGridView gridEconomicValue)
        {

           
            FormMain.Instance.DXFControl_BeginRead("Export Excel", "EXL", 17);

            try
            {
                Excel.Application xlApp;
                Excel.Workbook xlWorkBook;
                Excel.Worksheet xlWorkSheet;
                object misValue = System.Reflection.Missing.Value;

                xlApp = new Excel.Application();
                xlApp.DisplayAlerts = false;
                xlWorkBook = xlApp.Workbooks.Add(misValue);

                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                xlWorkSheet.Name = "경제적가치 평가결과";

                if (gridCondition != null)
                {
                    string szTemp1 = gridCondition.Rows[4].Cells[1].Value.ToString();
                    long.TryParse(szTemp1, out m_nYear);
                }

                if (!AddResult(xlWorkSheet, gridCapacity, gridValue, gridTotalValue, gridEconomicValue))
                    return Result(xlApp, xlWorkBook, xlWorkSheet, false);

                releaseObject(xlWorkSheet);

                xlWorkSheet = xlWorkBook.Worksheets.Add();
                xlWorkSheet.Name = "입력(대상별)";

                if (!AddUserInput(xlWorkSheet, gridArea, gridPublicCost, gridCondition, gridValueCost))
                    return Result(xlApp, xlWorkBook, xlWorkSheet, false);

                releaseObject(xlWorkSheet);

                xlWorkSheet = xlWorkBook.Worksheets.Add();
                xlWorkSheet.Name = "입력(시스템)";

                if (!AddSystemConst(xlWorkSheet))
                    return Result(xlApp, xlWorkBook, xlWorkSheet, false);

                releaseObject(xlWorkSheet);

                xlWorkBook.SaveAs(strPath, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                xlWorkBook.Close(true, misValue, misValue);
                releaseObject(xlWorkBook);

                xlApp.Workbooks.Close();
                releaseObject(xlApp.Workbooks);
                xlApp.Quit();
                releaseObject(xlApp);
            }
            catch (Exception ex)
            {
                MessageBox.Show(owner, "Excel문서로 출력중 오류가 발생하였습니다.\r\n오류내용 : " + ex.Message, "Excel 내보내기", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            FormMain.Instance.DXFControl_EndRead("Export Excel", "EXL");
            
            return true;
        }

        private static bool AddResult(Excel.Worksheet xlWorkSheet, DataGridView gridCapacity, DataGridView gridValue, DataGridView gridTotalValue, DataGridView gridEconomicValue)
        {
            char order = '1';
            int nRowIndex = 2, nColumnIndex = 1;
            int nRowIndex2 = 2, nColumnIndex2 = nColumnIndex + 8;
            FormMain.Instance.DXFControl_ReadEntity("Export Excel", 0);
            nRowIndex = AddAnnualCapacity(gridCapacity, xlWorkSheet, nRowIndex, nColumnIndex, ref order);
            FormMain.Instance.DXFControl_ReadEntity("Export Excel", 1);
            nRowIndex = AddAnnualValue(gridValue, xlWorkSheet, nRowIndex + 1, nColumnIndex, ref order);
            FormMain.Instance.DXFControl_ReadEntity("Export Excel", 2);
            nRowIndex = AddTotalValue(gridTotalValue, xlWorkSheet, nRowIndex + 1, nColumnIndex, ref order);

            FormMain.Instance.DXFControl_ReadEntity("Export Excel", 3);
            nRowIndex2 = AddEconomicValueSummary(gridEconomicValue, xlWorkSheet, nRowIndex2, nColumnIndex2, ref order);
            
            FormMain.Instance.DXFControl_ReadEntity("Export Excel", 3);
            nRowIndex2 = AddEconomicValue(gridEconomicValue, xlWorkSheet, nRowIndex2+1, nColumnIndex2, ref order);
            
            return true;
        }

        private static bool AddUserInput(Excel.Worksheet xlWorkSheet, DataGridView gridArea, DataGridView gridPublicCost, DataGridView gridCondition, DataGridView gridValueCost)
        {
            char order = '1';
            int nRowIndex = 2, nColumnIndex = 1;
            FormMain.Instance.DXFControl_ReadEntity("Export Excel", 4);
            nRowIndex = AddArea(gridArea, xlWorkSheet, nRowIndex, nColumnIndex, ref order);
            FormMain.Instance.DXFControl_ReadEntity("Export Excel", 5);
            nRowIndex = AddCondition(gridCondition, xlWorkSheet, nRowIndex + 1, nColumnIndex, ref order);
            FormMain.Instance.DXFControl_ReadEntity("Export Excel", 6);
            nRowIndex = AddPublicCost(gridPublicCost, xlWorkSheet, nRowIndex + 1, nColumnIndex, ref order);
            FormMain.Instance.DXFControl_ReadEntity("Export Excel", 7);
            nRowIndex = AddValueCost(gridValueCost, xlWorkSheet, nRowIndex + 1, nColumnIndex, ref order);
                      
            return true;
        }

        private static bool AddSystemConst(Excel.Worksheet xlWorkSheet)
        {
            char order = 'A';
            int nRowIndex = 2, nColumnIndex = 1;
            int nRowIndex2 = nRowIndex, nColumnIndex2 = nColumnIndex + 8;
            int nRowIndex3 = nRowIndex, nColumnIndex3 = nColumnIndex2 + 8;

            FormMain.Instance.DXFControl_ReadEntity("Export Excel", 8);
            nRowIndex = Add계량지표(FormMain.Instance.Get계량지표Grid(), xlWorkSheet, nRowIndex, nColumnIndex, ref order);
            FormMain.Instance.DXFControl_ReadEntity("Export Excel", 9);
            nRowIndex = Add화폐화지표(FormMain.Instance.Get화폐화지표Grid(), xlWorkSheet, nRowIndex + 1, nColumnIndex, ref order);
            FormMain.Instance.DXFControl_ReadEntity("Export Excel", 10);
            nRowIndex2 = Add기능회복율(FormMain.Instance.Get기능회복율Grid(), xlWorkSheet, nRowIndex2, nColumnIndex2, ref order);
            FormMain.Instance.DXFControl_ReadEntity("Export Excel", 11);
            nRowIndex2 = Add기능회복기간(FormMain.Instance.Get기능회복기간Grid(), xlWorkSheet, nRowIndex2 + 1, nColumnIndex2, ref order);
            FormMain.Instance.DXFControl_ReadEntity("Export Excel", 12);
            nRowIndex2 = Add단가(FormMain.Instance.Get토양정화기술Grid(), xlWorkSheet, nRowIndex2 + 1, nColumnIndex2, ref order);
            FormMain.Instance.DXFControl_ReadEntity("Export Excel", 13);
            nRowIndex3 = Add지불의사액(FormMain.Instance.Get지불의사액Grid(), xlWorkSheet, nRowIndex3, nColumnIndex3, ref order);
            FormMain.Instance.DXFControl_ReadEntity("Export Excel", 14);

            nRowIndex3 += 3;
            nRowIndex3 = Add가구수면적(FormMain.Instance.Get지역별가구수면적Grid(), xlWorkSheet, nRowIndex3+1, nColumnIndex3, ref order);
            FormMain.Instance.DXFControl_ReadEntity("Export Excel", 14);
            nRowIndex3 = Add비사용가중치(FormMain.Instance.Get비사용가치Grid(), xlWorkSheet, nRowIndex3 + 1, nColumnIndex3, ref order);
            FormMain.Instance.DXFControl_ReadEntity("Export Excel", 16);
            nRowIndex3 = Add스티그마(FormMain.Instance.Get스티그마Grid(), xlWorkSheet, nRowIndex3 + 1, nColumnIndex3, ref order);
            FormMain.Instance.DXFControl_ReadEntity("Export Excel", 17);
            return true;
        }

        private static int AddEconomicValueSummary(DataGridView grid, Excel.Worksheet xlWorkSheet, int nRowIndex, int nColumnIndex, ref char order)
        {
            if (grid == null)
                return nRowIndex;

            xlWorkSheet.Cells[nRowIndex, nColumnIndex] = order++ + "단계: 토양정화기술의 경제적가치 [억원]";
            SetTitle(xlWorkSheet.Cells[nRowIndex++, nColumnIndex]);
            
            xlWorkSheet.Cells[nRowIndex, nColumnIndex+ 4] = "순편익 NPV";

            xlWorkSheet.Cells[nRowIndex+1, nColumnIndex] = "구분";
            xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + 1] = "비용";
            xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + 2] = "편익";
            xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + 3] = "순편익";
            xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + 4] = "입력할인율";
            xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + 5] = "할인율:2%";
            xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + 6] = "할인율:4%";
            xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + 7] = "할인율:6%";
            
            for (int i = 0; i < 8; i++)
            {
                if( i > 3)
                {
                    SetCell(xlWorkSheet.Cells[nRowIndex, nColumnIndex + i]);
                    xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Interior.Color = GrayCell;
                    xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                }
                xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + i].Interior.Color = GrayCell;
                xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + i].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + i].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + i].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + i].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            }

            xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex, nColumnIndex+4], xlWorkSheet.Cells[nRowIndex, nColumnIndex+7]].Merge();

            nRowIndex++;
            nRowIndex++;

            int nRowCount = grid.Rows.Count;
            int nColumnCount = 8;

            nRowCount =  5;
            //nRowCount = grid.Rows.Count / 10 + 1;

            long nAfterYear = 13 + m_nYear;

            string [,] values = {
                {"정화직후","=J13+K13", "=SUM(L13:N13)", "=K5-J5","=P13","=R13","=T13","=V13"},
                {"10년후","=J13+K13", "=SUM(L13:N23)", "=K6-J6","=P23","=R23","=T23","=V23"},
                {"20년후","=J13+K13", "=SUM(L13:N33)", "=K7-J7","=P33","=R33","=T33","=V33"},
                {"30년후","=J13+K13", "=SUM(L13:N43)", "=K8-J8","=P43","=R43","=T43","=V43"},
                {"{0}년후(입력값)","=J13+K13", "=SUM(L13:N{0})", "=K9-J9","=P{0}","=R{0}","=T{0}","=V{0}"}
            };



            for (int i = 0; i < nRowCount; i++)
            {
                for (int j = 0; j < nColumnCount; j++)
                {
                    if (i == 4)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Font.Bold = true;
                        if( j == 0)
                        {
                            xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j] = string.Format(values[i, j], m_nYear);
                        }
                        else
                        {
                            xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j] = string.Format(values[i, j], nAfterYear);
                        }
                    }
                    else
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j] = values[i, j];
                        
                    }
                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    if (j == 0)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = GrayCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    }
                    else
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = GreenCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                    }
                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].EntireColumn.AutoFit();
                }
            }
            return nRowCount + nRowIndex;
        }

        // Return 값 : 토양정화기술의 경제적가치 작성한 바로 다음 Row Index
        private static int AddEconomicValue(DataGridView grid, Excel.Worksheet xlWorkSheet, int nRowIndex, int nColumnIndex, ref char order)
        {
            if (grid == null)
                return nRowIndex;

            //xlWorkSheet.Cells[nRowIndex, nColumnIndex] = order++ + "단계: 토양정화기술의 경제적가치 [억원]";
            //SetTitle(xlWorkSheet.Cells[nRowIndex++, nColumnIndex]);

            xlWorkSheet.Cells[nRowIndex, nColumnIndex] = "년수";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 1] = "비용";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 3] = "직접사용가치";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 5] = "비사용가치";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 6] = "순편익(할인율:입력값)";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 8] = "순편익(할인율:2%)";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 10] = "순편익(할인율:4%)";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 12] = "순편익(할인율:6%)";

            xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + 1] = "정화비용";
            xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + 2] = "기타비용";
            xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + 3] = "직접사용가치";
            xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + 4] = "간접사용가치";
            xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + 6] = "PV";
            xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + 7] = "NPV";
            xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + 8] = "PV";
            xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + 9] = "NPV";
            xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + 10] = "PV";
            xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + 11] = "NPV";
            xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + 12] = "PV";
            xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + 13] = "NPV";     

            for (int i = 0; i < 14; i++)
            {
                SetCell(xlWorkSheet.Cells[nRowIndex, nColumnIndex + i]);
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Interior.Color = GrayCell;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                xlWorkSheet.Cells[nRowIndex+1, nColumnIndex + i].Interior.Color = GrayCell;
                xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + i].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + i].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + i].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + i].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            }

            xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex, nColumnIndex], xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex]].Merge();
            xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex, nColumnIndex+1], xlWorkSheet.Cells[nRowIndex, nColumnIndex +2]].Merge();
            xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex, nColumnIndex + 3], xlWorkSheet.Cells[nRowIndex, nColumnIndex + 4]].Merge();
            xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex, nColumnIndex +5], xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + 5]].Merge();

            xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex, nColumnIndex + 6], xlWorkSheet.Cells[nRowIndex, nColumnIndex + 7]].Merge();
            xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex, nColumnIndex + 8], xlWorkSheet.Cells[nRowIndex, nColumnIndex + 9]].Merge();
            xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex, nColumnIndex + 10], xlWorkSheet.Cells[nRowIndex, nColumnIndex + 11]].Merge();
            xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex, nColumnIndex + 12], xlWorkSheet.Cells[nRowIndex, nColumnIndex + 13]].Merge();

            
            nRowIndex++;
            nRowIndex++;

            int nRowCount = grid.Rows.Count;
            int nColumnCount = grid.Columns.Count;

            // 처음 두 행은 Column 대신이므로 포함시키지 않는다.
            int nBeginRowIndex = 2;

            for (int i = nBeginRowIndex; i < nRowCount; i++)
            {
                for (int j = 0; j < nColumnCount; j++)
                {
                    DataGridViewCell cell = grid.Rows[i].Cells[j];

                    if (cell != null && cell.Value != null)
                    {
                        xlWorkSheet.Cells[nRowIndex + i - nBeginRowIndex, nColumnIndex + j] = cell.Value.ToString();
                        xlWorkSheet.Cells[nRowIndex + i - nBeginRowIndex, nColumnIndex + j].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    }

                    if (j == 0)
                    {
                        xlWorkSheet.Cells[nRowIndex + i - nBeginRowIndex, nColumnIndex + j].Interior.Color = GrayCell;
                        xlWorkSheet.Cells[nRowIndex + i - nBeginRowIndex, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                        xlWorkSheet.Cells[nRowIndex + i - nBeginRowIndex, nColumnIndex + j].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;    
                    }
                    else
                    {
                        xlWorkSheet.Cells[nRowIndex + i - nBeginRowIndex, nColumnIndex + j].Interior.Color = GreenCell;
                        xlWorkSheet.Cells[nRowIndex + i - nBeginRowIndex, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                        xlWorkSheet.Cells[nRowIndex + i - nBeginRowIndex, nColumnIndex + j].HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                    }
                    xlWorkSheet.Cells[nRowIndex + i - nBeginRowIndex, nColumnIndex + j].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    xlWorkSheet.Cells[nRowIndex + i - nBeginRowIndex, nColumnIndex + j].EntireColumn.AutoFit();
                }
            }
            return nRowCount + nRowIndex - nBeginRowIndex;
        }

        // Return 값 : 총 경제적가치 작성한 바로 다음 Row Index
        private static int AddTotalValue(DataGridView grid, Excel.Worksheet xlWorkSheet, int nRowIndex, int nColumnIndex, ref char order)
        {
            if (grid == null)
                return nRowIndex;

            xlWorkSheet.Cells[nRowIndex, nColumnIndex] = order++ + "단계: 총경제적가치 [억원]";
            SetTitle(xlWorkSheet.Cells[nRowIndex++, nColumnIndex]);

            xlWorkSheet.Cells[nRowIndex, nColumnIndex] = "기 능";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 1] = "일반토양";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 2] = "밭토양";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 3] = "논토양";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 4] = "임야토양";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 5] = "계";

            for (int i = 0; i < 6; i++)
            {
                SetCell(xlWorkSheet.Cells[nRowIndex, nColumnIndex + i]);
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Interior.Color = GrayCell;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            }
            nRowIndex++;

            int nRowCount = grid.Rows.Count;
            int nColumnCount = grid.Columns.Count;

            for (int i = 0; i < nRowCount; i++)
            {
                for (int j = 0; j < nColumnCount; j++)
                {
                    DataGridViewCell cell = grid.Rows[i].Cells[j];

                    if (cell != null && cell.Value != null)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j] = cell.Value.ToString();
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    }

                    if (j < 1)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = GrayCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }
                    else if (i <= (int)SoilFunctionType.수질정화)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = GreenCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }
                    else if (i <= (int)SoilFunctionType.원료공급기능)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = LightGreenCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }
                    else if (i <= (int)SoilFunctionType.생태학적가치)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = OrangeCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }
                    else
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = GreenCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }

                    if (j == 0)
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;                   
                    else
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].EntireColumn.AutoFit();
                }
            }
            xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex + (int)SoilFunctionType.식물생산기능, nColumnIndex + nColumnCount - 1], xlWorkSheet.Cells[nRowIndex + (int)SoilFunctionType.원료공급기능, nColumnIndex + nColumnCount - 1]].Merge();
          
            return nRowCount + nRowIndex;
        }

        // Return 값 : 연간 경제적가치 작성한 바로 다음 Row Index
        private static int AddAnnualValue(DataGridView grid, Excel.Worksheet xlWorkSheet, int nRowIndex, int nColumnIndex, ref char order)
        {
            if (grid == null)
                return nRowIndex;

            xlWorkSheet.Cells[nRowIndex, nColumnIndex] = order++ + "단계: 연간 경제적가치 [억원/년]";
            SetTitle(xlWorkSheet.Cells[nRowIndex++, nColumnIndex]);

            xlWorkSheet.Cells[nRowIndex, nColumnIndex] = "기 능";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 1] = "일반토양";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 2] = "밭토양";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 3] = "논토양";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 4] = "임야토양";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 5] = "계";

            for (int i = 0; i < 6; i++)
            {
                SetCell(xlWorkSheet.Cells[nRowIndex, nColumnIndex + i]);
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Interior.Color = GrayCell;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            }

            //xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex, nColumnIndex], xlWorkSheet.Cells[nRowIndex, nColumnIndex + 1]].Merge();

            nRowIndex++;

            int nRowCount = grid.Rows.Count;
            int nColumnCount = grid.Columns.Count;

            for (int i = 0; i < nRowCount; i++)
            {
                for (int j = 0; j < nColumnCount; j++)
                {
                    DataGridViewCell cell = grid.Rows[i].Cells[j];

                    if (cell != null && cell.Value != null)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j] = cell.Value.ToString();
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    }

                    if (j < 1)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = GrayCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }
                    else if (i <= (int)SoilFunctionType.수질정화)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = GreenCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }
                    else if (i <= (int)SoilFunctionType.원료공급기능)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = LightGreenCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }
                    else if (i <= (int)SoilFunctionType.생태학적가치)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = OrangeCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }
                    else
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = GreenCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }

                    if (j == 0)
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                    //else if (j == 1)
                    //    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    else
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].EntireColumn.AutoFit();
                }
            }

            xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex + (int)SoilFunctionType.식물생산기능, nColumnIndex + nColumnCount - 1], xlWorkSheet.Cells[nRowIndex + (int)SoilFunctionType.원료공급기능, nColumnIndex + nColumnCount - 1]].Merge();
            //xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex + nRowCount - 1, nColumnIndex], xlWorkSheet.Cells[nRowIndex + nRowCount - 1, nColumnIndex + 1]].Merge();

            return nRowCount + nRowIndex;
        }

        // Return 값 : 연간 기능용량 작성한 바로 다음 Row Index
        private static int AddAnnualCapacity(DataGridView grid, Excel.Worksheet xlWorkSheet, int nRowIndex, int nColumnIndex, ref char order)
        {
            if (grid == null)
                return nRowIndex;

            xlWorkSheet.Cells[nRowIndex, nColumnIndex] = order++ + "단계: 연간 기능용량 [ton/년]";
            SetTitle(xlWorkSheet.Cells[nRowIndex++, nColumnIndex]);

            xlWorkSheet.Cells[nRowIndex, nColumnIndex] = "기 능";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 1] = "일반토양";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 2] = "밭토양";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 3] = "논토양";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 4] = "임야토양";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 5] = "계";

            for (int i = 0; i < 6; i++)
            {
                SetCell(xlWorkSheet.Cells[nRowIndex, nColumnIndex + i]);
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Interior.Color = GrayCell;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            }

            //xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex, nColumnIndex], xlWorkSheet.Cells[nRowIndex, nColumnIndex + 1]].Merge();

            nRowIndex++;

            int nRowCount = grid.Rows.Count;
            int nColumnCount = grid.Columns.Count;

            for (int i = 0; i < nRowCount; i++)
            {
                for (int j = 0; j < nColumnCount; j++)
                {
                    DataGridViewCell cell = grid.Rows[i].Cells[j];

                    if (cell != null && cell.Value != null)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j] = cell.Value.ToString();
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    }

                    if (j < 1)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = GrayCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }
                    else if (i <= (int)SoilFunctionType.생태학적가치 && i >= (int)SoilFunctionType.식물생산기능)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = LightGreenCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }
                    else
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = GreenCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }

                    if (j == 0)
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                    //else if (j == 1)
                    //    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    else
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].EntireColumn.AutoFit();
                }
            }

            //xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex + nRowCount - 1, nColumnIndex], xlWorkSheet.Cells[nRowIndex + nRowCount - 1, nColumnIndex + 1]].Merge();
            return nRowCount + nRowIndex;
        }

        // Return 값 : 비사용가치 입력 작성한 바로 다음 Row Index
        private static int AddValueCost(DataGridView grid, Excel.Worksheet xlWorkSheet, int nRowIndex, int nColumnIndex, ref char order)
        {
            if (grid == null)
                return nRowIndex;

            xlWorkSheet.Cells[nRowIndex, nColumnIndex] = order++ + ") 비사용가치입력";
            SetTitle(xlWorkSheet.Cells[nRowIndex++, nColumnIndex]);

            xlWorkSheet.Cells[nRowIndex, nColumnIndex] = "구 분";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 1] = "경제적가치(억원/년)";

            for (int i = 0; i <= 2; i++)
            {
                SetCell(xlWorkSheet.Cells[nRowIndex, nColumnIndex + i]);
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Interior.Color = GrayCell;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            }

            xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex, nColumnIndex + 1], xlWorkSheet.Cells[nRowIndex, nColumnIndex + 2]].Merge();

            nRowIndex++;

            int nRowCount = grid.Rows.Count;
            int nColumnCount = grid.Columns.Count;

            for (int i = 0; i < nRowCount; i++)
            {
                for (int j = 0; j < nColumnCount; j++)
                {
                    DataGridViewCell cell = grid.Rows[i].Cells[j];

                    if (cell != null && cell.Value != null)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j] = cell.Value.ToString();
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    }

                    if (j == 0 || (j == 1 && i == 2) || (j == 1 && i == 3) || (j == 1 && i == 4))
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = GrayCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }
                    else
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = OrangeCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }

                    if (j == 0)
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                    else
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].EntireColumn.AutoFit();
                }               
            }

            xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex , nColumnIndex + 1], xlWorkSheet.Cells[nRowIndex , nColumnIndex + 2]].Merge();
            xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + 1], xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + 2]].Merge();
            xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex + 5, nColumnIndex + 1], xlWorkSheet.Cells[nRowIndex + 5, nColumnIndex + 2]].Merge();
            xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex + 6, nColumnIndex + 1], xlWorkSheet.Cells[nRowIndex + 6, nColumnIndex + 2]].Merge();
            xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex + 7, nColumnIndex + 1], xlWorkSheet.Cells[nRowIndex + 7, nColumnIndex + 2]].Merge();
            xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex + 8, nColumnIndex + 1], xlWorkSheet.Cells[nRowIndex + 8, nColumnIndex + 2]].Merge();
            xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex + 9, nColumnIndex + 1], xlWorkSheet.Cells[nRowIndex + 9, nColumnIndex + 2]].Merge();
           
            return nRowCount + nRowIndex;
        }

        // Return 값 : 대상지 공시지가 작성한 바로 다음 Row Index
        private static int AddPublicCost(DataGridView grid, Excel.Worksheet xlWorkSheet, int nRowIndex, int nColumnIndex, ref char order)
        {
            if (grid == null)
                return nRowIndex;

            xlWorkSheet.Cells[nRowIndex, nColumnIndex] = order++ + ") 대상지 공시지가(직접사용가치)";
            SetTitle(xlWorkSheet.Cells[nRowIndex++, nColumnIndex]);

            xlWorkSheet.Cells[nRowIndex, nColumnIndex] = "구분(지목)";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 2] = "공시지가";

           
            for (int i = 0; i <= 2; i++)
            {
                SetCell(xlWorkSheet.Cells[nRowIndex, nColumnIndex + i]);
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Interior.Color = GrayCell;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            }

            xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex, nColumnIndex + 1], xlWorkSheet.Cells[nRowIndex, nColumnIndex + 2]].Merge();

            nRowIndex++;

            int nRowCount = grid.Rows.Count;
            int nColumnCount = grid.Columns.Count;

            for (int i = 0; i < nRowCount; i++)
            {
                
                for (int j = 0; j < nColumnCount + 1; j++)
                {
                    DataGridViewCell cell = null;
                    if (j < nColumnCount)
                    {
                        cell = grid.Rows[i].Cells[j];
                    }   
                    if (cell != null && cell.Value != null)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j] = cell.Value.ToString();
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    }

                    if (j == 0 || i == nRowCount - 1)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = GrayCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }
                    else
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = OrangeCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }

                    if (j == 0)
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    else
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].EntireColumn.AutoFit();
                }
            }

            for (int i = 0; i < nRowCount; i++)
            {
                xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + 1], xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + 2]].Merge();
            }

            return nRowCount + nRowIndex;
        }

        // Return 값 : 분석조건 입력 작성한 바로 다음 Row Index
        private static int AddCondition(DataGridView grid, Excel.Worksheet xlWorkSheet, int nRowIndex, int nColumnIndex, ref char order)
        {
            if (grid == null)
                return nRowIndex;

            xlWorkSheet.Cells[nRowIndex, nColumnIndex] = order++ + ") 분석조건 입력";
            SetTitle(xlWorkSheet.Cells[nRowIndex++, nColumnIndex]);            
            
            int nRowCount = grid.Rows.Count;
            int nColumnCount = grid.Columns.Count;

            for (int i = 0; i < nRowCount; i++)
            {
                for (int j = 0; j < nColumnCount + 1; j++)
                {
                    DataGridViewCell cell = null;
                    if( j < nColumnCount)
                    {
                        cell = grid.Rows[i].Cells[j];
                    }   
                    if (cell != null && cell.Value != null)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j] = cell.Value.ToString();
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    }

                    if (j == 0)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = GrayCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }
                    else
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = OrangeCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }

                    if (j == 0)
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                    else
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].EntireColumn.AutoFit();
                }
                if( i == 1)
                {
                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + 1].Interior.Color = GrayCell;
                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + 2].Interior.Color = GrayCell;
                }
               
            }

            for (int i = 0; i < nRowCount; i++)
            {
                xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + 1], xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + 2]].Merge();
            }
            return nRowCount + nRowIndex;
        }

        // Return 값 : 대상지 면적 입력 작성한 바로 다음 Row Index
        private static int AddArea(DataGridView grid, Excel.Worksheet xlWorkSheet, int nRowIndex, int nColumnIndex, ref char order)
        {
            if (grid == null)
                return nRowIndex;

            xlWorkSheet.Cells[nRowIndex, nColumnIndex] = order++ + ") 대상지 면적 입력";
            SetTitle(xlWorkSheet.Cells[nRowIndex++, nColumnIndex]);

            xlWorkSheet.Cells[nRowIndex, nColumnIndex] = "구 분";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 1] = "면 적";

            for (int i = 0; i <= 2; i++)
            {
                SetCell(xlWorkSheet.Cells[nRowIndex, nColumnIndex + i]);
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Interior.Color = GrayCell;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            }

            xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex, nColumnIndex + 1], xlWorkSheet.Cells[nRowIndex, nColumnIndex + 2]].Merge();

            nRowIndex++;

            int nRowCount = grid.Rows.Count;
            int nColumnCount = grid.Columns.Count;

            for (int i = 0; i < nRowCount; i++)
            {
                for (int j = 0; j < nColumnCount; j++)
                {
                    DataGridViewCell cell = grid.Rows[i].Cells[j];

                    if (cell != null && cell.Value != null)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j] = cell.Value.ToString();
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    }

                    if (j == 0 || j == 2 || i == nRowCount - 1)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = GrayCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }
                    else
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = OrangeCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }

                    if (j == 0)
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    else
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].EntireColumn.AutoFit();
                }
            }

            return nRowCount + nRowIndex;
        }


        private static int Add지불의사액(DataGridView grid, Excel.Worksheet xlWorkSheet, int nRowIndex, int nColumnIndex, ref char order)
        {
            if (grid == null)
                return nRowIndex;

            xlWorkSheet.Cells[nRowIndex, nColumnIndex] = order++ + ") 지불의사액 [원/가구/월]";
            SetTitle(xlWorkSheet.Cells[nRowIndex++, nColumnIndex]);

            xlWorkSheet.Cells[nRowIndex, nColumnIndex] = "WTP [원/월]";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 1] = "대수선형로짓트";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 3] = "Weibull";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 5] = "지불거부율(%)";
                        
            xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + 1] = "중앙치";            
            xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + 2] = "WTP(절단)";
            xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + 3] = "중앙치";
            xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + 4] = "WTP(절단)";

            for (int i = 0; i < 6; i++)
            {
                SetCell(xlWorkSheet.Cells[nRowIndex, nColumnIndex + i]);
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Interior.Color = GrayCell;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                SetCell(xlWorkSheet.Cells[nRowIndex+1, nColumnIndex + i]);
                xlWorkSheet.Cells[nRowIndex+1, nColumnIndex + i].Interior.Color = GrayCell;
                xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + i].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + i].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + i].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + i].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            }

            xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex, nColumnIndex + 1], xlWorkSheet.Cells[nRowIndex, nColumnIndex + 2]].Merge();
            xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex, nColumnIndex + 3], xlWorkSheet.Cells[nRowIndex, nColumnIndex + 4]].Merge();
            
            xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex, nColumnIndex], xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex]].Merge();
            xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex, nColumnIndex + 5], xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + 5]].Merge();

            nRowIndex++;
            nRowIndex++;

            int nRowCount = grid.Rows.Count;
            int nColumnCount = grid.Columns.Count;

            for (int i = 0; i < nRowCount; i++)
            {
                for (int j = 0; j < nColumnCount - 1; j++)
                {
                    DataGridViewCell cell = grid.Rows[i].Cells[j + 1];

                    if (cell != null && cell.Value != null)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j] = cell.Value.ToString();
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    }

                    if (j < 1)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = GrayCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }
                    else if (cell != null && cell.Value != null && cell.Value.ToString().Trim().Length > 0)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = BlueCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }

                    if (j >= 1)
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].EntireColumn.AutoFit();
                }

                //xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex + i, nColumnIndex], xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + 1]].Merge();
            }

            return nRowCount + nRowIndex;
        }
        private static int Add가구수면적(DataGridView grid, Excel.Worksheet xlWorkSheet, int nRowIndex, int nColumnIndex, ref char order)
        {
            if (grid == null)
                return nRowIndex;

            xlWorkSheet.Cells[nRowIndex, nColumnIndex] = order++ + ") 지역별 가구수 및 면적";
            SetTitle(xlWorkSheet.Cells[nRowIndex++, nColumnIndex]);

            xlWorkSheet.Cells[nRowIndex, nColumnIndex] = "구  분";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 1] = "가구수";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 3] = "면적";

            xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + 1] = "가구수";
            xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + 2] = "%";
            xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + 3] = "㎢";
            xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + 4] = "%";
            for (int i = 0; i < 5; i++)
            {
                SetCell(xlWorkSheet.Cells[nRowIndex, nColumnIndex + i]);
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Interior.Color = GrayCell;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                SetCell(xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + i]);
                xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + i].Interior.Color = GrayCell;
                xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + i].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + i].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + i].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex + i].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            }

            xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex, nColumnIndex + 1], xlWorkSheet.Cells[nRowIndex, nColumnIndex + 2]].Merge();
            xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex, nColumnIndex + 3], xlWorkSheet.Cells[nRowIndex, nColumnIndex + 4]].Merge();

            xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex, nColumnIndex], xlWorkSheet.Cells[nRowIndex + 1, nColumnIndex]].Merge();
   
            nRowIndex++;
            nRowIndex++;

            int nRowCount = grid.Rows.Count;
            int nColumnCount = grid.Columns.Count;

            for (int i = 0; i < nRowCount; i++)
            {
                for (int j = 0; j < nColumnCount - 1; j++)
                {
                    DataGridViewCell cell = grid.Rows[i].Cells[j + 1];

                    if (cell != null && cell.Value != null)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j] = cell.Value.ToString();
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    }

                    if (j < 1)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = GrayCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }
                    else if (cell != null && cell.Value != null && cell.Value.ToString().Trim().Length > 0)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = BlueCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }

                    if (j >= 1)
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].EntireColumn.AutoFit();
                }

                //xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex + i, nColumnIndex], xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + 1]].Merge();
            }

            return nRowCount + nRowIndex;
        }

        private static int Add비사용가중치(DataGridView grid, Excel.Worksheet xlWorkSheet, int nRowIndex, int nColumnIndex, ref char order)
        {
            if (grid == null)
                return nRowIndex;

            xlWorkSheet.Cells[nRowIndex, nColumnIndex] = order++ + ") 비사용가치 가중치";
            SetTitle(xlWorkSheet.Cells[nRowIndex++, nColumnIndex]);

            xlWorkSheet.Cells[nRowIndex, nColumnIndex] = "가중치";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 1] = "유산가치";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 2] = "존재가치";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 3] = "선택가치";
 
            for (int i = 0; i < 4; i++)
            {
                SetCell(xlWorkSheet.Cells[nRowIndex, nColumnIndex + i]);
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Interior.Color = GrayCell;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            }

            //xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex, nColumnIndex], xlWorkSheet.Cells[nRowIndex, nColumnIndex + 1]].Merge();

            nRowIndex++;

            int nRowCount = grid.Rows.Count;
            int nColumnCount = grid.Columns.Count;

            for (int i = 0; i < nRowCount; i++)
            {
                for (int j = 0; j < nColumnCount - 1; j++)
                {
                    DataGridViewCell cell = grid.Rows[i].Cells[j + 1];

                    if (cell != null && cell.Value != null)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j] = cell.Value.ToString();
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    }

                    if (j < 1)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = GrayCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }
                    else if (cell != null && cell.Value != null && cell.Value.ToString().Trim().Length > 0)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = BlueCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }

                    if (j >= 1)
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].EntireColumn.AutoFit();
                }

                //xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex + i, nColumnIndex], xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + 1]].Merge();
            }

            return nRowCount + nRowIndex;
        }

        private static int Add스티그마(DataGridView grid, Excel.Worksheet xlWorkSheet, int nRowIndex, int nColumnIndex, ref char order)
        {
            if (grid == null)
                return nRowIndex;

            xlWorkSheet.Cells[nRowIndex, nColumnIndex] = order++ + ") 스티그마 및 회복기간";
            SetTitle(xlWorkSheet.Cells[nRowIndex++, nColumnIndex]);

            xlWorkSheet.Cells[nRowIndex, nColumnIndex] = "스티그마 [%]";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 1] = "회복기간 [년]";

            for (int i = 0; i < 2; i++)
            {
                SetCell(xlWorkSheet.Cells[nRowIndex, nColumnIndex + i]);
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Interior.Color = GrayCell;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            }
            nRowIndex++;

            int nRowCount = grid.Rows.Count;
            int nColumnCount = grid.Columns.Count;

            for (int i = 0; i < nRowCount; i++)
            {
                for (int j = 0; j < nColumnCount - 1; j++)
                {
                    DataGridViewCell cell = grid.Rows[i].Cells[j + 1];

                    if (cell != null && cell.Value != null)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j] = cell.Value.ToString();
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    }

                    if (cell != null && cell.Value != null && cell.Value.ToString().Trim().Length > 0)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = BlueCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }
                   
                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].EntireColumn.AutoFit();
                }
            }
            return nRowCount + nRowIndex;
        }      

        // Return 값 : 토양정화기술 단가 작성한 바로 다음 Row Index
        private static int Add단가(DataGridView grid, Excel.Worksheet xlWorkSheet, int nRowIndex, int nColumnIndex, ref char order)
        {
            if (grid == null)
                return nRowIndex;

            xlWorkSheet.Cells[nRowIndex, nColumnIndex] = order++ + ") 토양정화기술 단가 [억원/ha]";
            SetTitle(xlWorkSheet.Cells[nRowIndex++, nColumnIndex]);

            xlWorkSheet.Cells[nRowIndex, nColumnIndex] = "구 분";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 1] = "생물통풍";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 2] = "토양경작";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 3] = "증기추출";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 4] = "토양세척";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 5] = "화학산화";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 6] = "열탈착";

            for (int i = 0; i < 7; i++)
            {
                SetCell(xlWorkSheet.Cells[nRowIndex, nColumnIndex + i]);
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Interior.Color = GrayCell;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            }

            //xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex, nColumnIndex], xlWorkSheet.Cells[nRowIndex, nColumnIndex + 1]].Merge();

            nRowIndex++;

            int nRowCount = grid.Rows.Count;
            int nColumnCount = grid.Columns.Count;

            for (int i = 0; i < nRowCount; i++)
            {
                for (int j = 0; j < nColumnCount - 1; j++)
                {
                    DataGridViewCell cell = grid.Rows[i].Cells[j + 1];

                    if (cell != null && cell.Value != null)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j] = cell.Value.ToString();
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    }

                    if (j < 1)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = GrayCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }
                    else if (cell != null && cell.Value != null && cell.Value.ToString().Trim().Length > 0)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = BlueCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }

                    if (j >= 1)
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].EntireColumn.AutoFit();
                }

                //xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex + i, nColumnIndex], xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + 1]].Merge();
            }

            return nRowCount + nRowIndex;
        }

        // Return 값 : 기능회복기간 작성한 바로 다음 Row Index
        private static int Add기능회복기간(DataGridView grid, Excel.Worksheet xlWorkSheet, int nRowIndex, int nColumnIndex, ref char order)
        {
            if (grid == null)
                return nRowIndex;

            xlWorkSheet.Cells[nRowIndex, nColumnIndex] = order++ + ") 토양정화기술별 기능회복기간 [년]";
            SetTitle(xlWorkSheet.Cells[nRowIndex++, nColumnIndex]);

            xlWorkSheet.Cells[nRowIndex, nColumnIndex] = "기 능";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 1] = "생물통풍";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 2] = "토양경작";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 3] = "증기추출";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 4] = "토양세척";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 5] = "화학산화";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 6] = "열탈착";

            for (int i = 0; i < 7; i++)
            {
                SetCell(xlWorkSheet.Cells[nRowIndex, nColumnIndex + i]);
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Interior.Color = GrayCell;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            }

            //xlWorkSheet.Range[xlWorkSheet.Cells[nRowIndex, nColumnIndex], xlWorkSheet.Cells[nRowIndex, nColumnIndex + 1]].Merge();

            nRowIndex++;

            int nRowCount = grid.Rows.Count;
            int nColumnCount = grid.Columns.Count;

            for (int i = 0; i < nRowCount; i++)
            {
                for (int j = 0; j < nColumnCount - 1; j++)
                {
                    DataGridViewCell cell = grid.Rows[i].Cells[j + 1];

                    if (cell != null && cell.Value != null)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j] = cell.Value.ToString();
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    }

                    if (j < 1)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = GrayCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }
                    else if (cell != null && cell.Value != null && cell.Value.ToString().Trim().Length > 0)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = BlueCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }

                    if (j >= 1)
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].EntireColumn.AutoFit();
                }
            }

            return nRowCount + nRowIndex;
        }

        // Return 값 : 기능회복율 작성한 바로 다음 Row Index
        private static int Add기능회복율(DataGridView grid, Excel.Worksheet xlWorkSheet, int nRowIndex, int nColumnIndex, ref char order)
        {
            if (grid == null)
                return nRowIndex;

            xlWorkSheet.Cells[nRowIndex, nColumnIndex] = order++ + ") 토양정화기술별 기능회복율 [-]";
            SetTitle(xlWorkSheet.Cells[nRowIndex++, nColumnIndex]);

            xlWorkSheet.Cells[nRowIndex, nColumnIndex] = "기 능";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 1] = "생물통풍";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 2] = "토양경작";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 3] = "증기추출";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 4] = "토양세척";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 5] = "화학산화";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 6] = "열탈착";

            for (int i = 0; i < 7; i++)
            {
                SetCell(xlWorkSheet.Cells[nRowIndex, nColumnIndex + i]);
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Interior.Color = GrayCell;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            }
            nRowIndex++;

            int nRowCount = grid.Rows.Count;
            int nColumnCount = grid.Columns.Count;

            for (int i = 0; i < nRowCount; i++)
            {
                for (int j = 0; j < nColumnCount - 1; j++)
                {
                    DataGridViewCell cell = grid.Rows[i].Cells[j + 1];

                    if (cell != null && cell.Value != null)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j] = cell.Value.ToString();
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    }

                    if (j < 1)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = GrayCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }
                    else if (cell != null && cell.Value != null && cell.Value.ToString().Trim().Length > 0)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = BlueCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }

                    if (j >= 1)
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    
                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].EntireColumn.AutoFit();
                }
            }

            return nRowCount + nRowIndex;
        }

        // Return 값 : 화폐화지표를 작성한 바로 다음 Row Index
        private static int Add화폐화지표(DataGridView grid, Excel.Worksheet xlWorkSheet, int nRowIndex, int nColumnIndex, ref char order)
        {
            if (grid == null)
                return nRowIndex;

            xlWorkSheet.Cells[nRowIndex, nColumnIndex] = order++ + ") 기능별 화폐화지표 [원/ton]";
            SetTitle(xlWorkSheet.Cells[nRowIndex++, nColumnIndex]);

            xlWorkSheet.Cells[nRowIndex, nColumnIndex] = "기 능";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 1] = "화폐화지표";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 2] = "값";

            for (int i=0; i < 3;i++)
            {
                SetCell(xlWorkSheet.Cells[nRowIndex, nColumnIndex + i]);
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Interior.Color = GrayCell;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Interior.Pattern = Excel.XlPattern.xlPatternSolid;	
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            }
            nRowIndex++;

            int nRowCount = grid.Rows.Count;
            int nColumnCount = grid.Columns.Count;

            for (int i = 0; i < nRowCount; i++)
            {
                for (int j = 0; j < nColumnCount - 1; j++)
                {
                    DataGridViewCell cell = grid.Rows[i].Cells[j + 1];

                    if (cell != null && cell.Value != null)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j] = cell.Value.ToString();
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    }

                    if (j <= 1)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = GrayCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }
                    else if (cell.Value != null && cell.Value.ToString().Trim().Length > 0)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = BlueCell;
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Pattern = Excel.XlPattern.xlPatternSolid;
                    }
                                        
                    if (j == 2)
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                    else
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].EntireColumn.AutoFit();
                }
            }
            return nRowCount + nRowIndex;
        }

        // Return 값 : 계량지표를 작성한 바로 다음 Row Index
        private static int Add계량지표(DataGridView grid, Excel.Worksheet xlWorkSheet, int nRowIndex, int nColumnIndex, ref char order)
        {
            if (grid == null)
                return nRowIndex;

            xlWorkSheet.Cells[nRowIndex, nColumnIndex] = order++ + ") 기능별 계량지표 [ton/ha]";
            SetTitle(xlWorkSheet.Cells[nRowIndex++, nColumnIndex]);

            xlWorkSheet.Cells[nRowIndex, nColumnIndex] = "기 능";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 1] = "계량지표";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 2] = "일반토양";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 3] = "밭토양";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 4] = "논토양";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 5] = "임야토양";
            xlWorkSheet.Cells[nRowIndex, nColumnIndex + 6] = "계";

            for (int i = 0; i < 7; i++)
            {
                SetCell(xlWorkSheet.Cells[nRowIndex, nColumnIndex + i]);
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Interior.Color = GrayCell;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                xlWorkSheet.Cells[nRowIndex, nColumnIndex + i].EntireColumn.AutoFit();
            }
            nRowIndex++;

            int nRowCount = grid.Rows.Count;
            int nColumnCount = grid.Columns.Count;

            for (int i=0;i<nRowCount;i++)
            {
                for (int j=0;j<nColumnCount - 1;j++)
                {
                    DataGridViewCell cell = grid.Rows[i].Cells[j + 1];

                    if (cell != null && cell.Value != null)
                    {
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j] = cell.Value.ToString();
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    }

                    if (j <= 1)
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = GrayCell;
                    else if (cell.Value != null && cell.Value.ToString().Trim().Length > 0)
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Interior.Color = BlueCell;
                    
                    if (j >= 2)
                        xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    xlWorkSheet.Cells[nRowIndex + i, nColumnIndex + j].EntireColumn.AutoFit();
                }
            }

            return nRowCount + nRowIndex;
        }

        private static void SetTitle(Excel.Range cell)
        {
            cell.Font.Bold = true;
            cell.Font.Name = "맑은 고딕";
            cell.Font.Size = 12;
        }

        private static void SetCell(Excel.Range cell)
        {
            cell.Font.Bold = false;
            cell.Font.Name = "맑은 고딕";
            cell.Font.Size = 11;
        }

        private static bool Result(Excel.Application xlApp, Excel.Workbook xlWorkBook, Excel.Worksheet xlWorkSheet, bool isSuccess)
        {
            releaseObject(xlWorkSheet);
            xlWorkBook.Close(true);
            releaseObject(xlWorkBook);

            xlApp.Workbooks.Close();
            releaseObject(xlApp.Workbooks);
            xlApp.Quit();
            releaseObject(xlApp);

            return isSuccess;
        }

        private static void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();

            }
        }
    }
}
