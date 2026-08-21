using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AxHWPCONTROLLib;

namespace HWPReportMaker
{
    public class HWPManager
    {
        private List<Project> m_listProject = null;
        private AxHwpCtrl m_hwpCtrl = null;
        private string m_emptySubCategoryName = "기타";

        public HWPManager(List<Project> projectList, AxHwpCtrl hwpCtrl)
        {
            m_listProject = projectList;
            m_hwpCtrl = hwpCtrl;
        }

        public bool Save(string strPath, bool noSave)
        {
            string strSampleFilePath = System.Windows.Forms.Application.StartupPath + "\\고령단계별집행계획수립_공고문_양식.hwp";

            if (!LoadFile(strSampleFilePath))
                return false;

            LoadDate();

            // Category 이름, Category
            Dictionary<string, CategoryData> dicAllCategories = ReadAllCategory();
            Dictionary<Project, List<CategoryData>> dicProjectCategories = ReadProjectCategory();

            // 단계별 집행계획 총괄표
            if (!LoadTotalTable(dicAllCategories, dicProjectCategories))
                return false;

            // 단계별 집행계획 조서
            if (!LoadLevelTable(dicProjectCategories))
                return false;

            MoveToBegin();

            if (!noSave)
                m_hwpCtrl.SaveAs(strPath);

            return true;
        }

        private void LoadDate()
        {
            SetBookMarkPosition("공고년도");
            InsertText(DateTime.Now.Year.ToString());

            SetBookMarkPosition("공고월");
            InsertText(DateTime.Now.Month.ToString());
        }

        // 한글문서 불러오기
        private bool LoadFile(string strPath)
        {
            m_hwpCtrl.RegisterModule("FilePathCheckDLL", "FilePathCheckerModule");
            return m_hwpCtrl.Open(strPath);
        }

        private bool LoadLevelTable(Dictionary<Project, List<CategoryData>> dicProjectCategories)
        {
            int nProjectIndex = 0;
            HWPPosition posCurrent = null;

            foreach (KeyValuePair<Project, List<CategoryData>> pair in dicProjectCategories)
            {
                // Key : SubCategory Name
                Dictionary<string, HWPPosition> dicSubCategoryPositions = new Dictionary<string, HWPPosition>();
                // Key : Street Name
                Dictionary<string, HWPPosition> dicStreetPositions = new Dictionary<string, HWPPosition>();
                // Key : SubCategoryName Tag
                Dictionary<string, SubCategoryProcess> dicSubCategories = new Dictionary<string, SubCategoryProcess>();
                SubCategoryProcess subProcess = null;
                List<Street> streetList = null;

                foreach (CategoryData data in pair.Value)
                {
                    foreach (SubCategoryData subData in data.SubCategoryDataList)
                    {
                        string strSubCategoryTag = RemoveNumber(subData.SubCategoryName);

                        if (!dicSubCategories.TryGetValue(strSubCategoryTag, out subProcess))
                        {
                            subProcess = new SubCategoryProcess();
                            subProcess.SubCategoryName = strSubCategoryTag;
                            dicSubCategories[strSubCategoryTag] = subProcess;
                        }

                        foreach (Street street in subData.StreetList)
                        {
                            if (!subProcess.StreetProcessList.TryGetValue(street.Process.ProcessName, out streetList))
                            {
                                streetList = new List<Street>();
                                subProcess.StreetProcessList[street.Process.ProcessName] = streetList;
                            }

                            streetList.Add(street);

                            if (!subProcess.StreetNames.Contains(street.StreetName))
                                subProcess.StreetNames.Add(street.StreetName);
                        }
                    }
                }

                LoadLevelName(pair.Key, nProjectIndex + 1);

                string strBookMark = "단계별_시설명_" + (++nProjectIndex).ToString();
                SetBookMarkPosition(strBookMark);

                HWPPosition posTotal = GetCurrentPosition();
                posCurrent = posTotal.Clone();

                int nProjectCompleteArea = 0, nProjectIncompleteArea = 0, nProjectStreetCount = 0;
                long nProjectCost = 0;
                Dictionary<int, int> dicProjectProcessArea = new Dictionary<int, int>();

                List<SubCategoryProcess> subCategories = ToList(dicSubCategories);

                foreach (SubCategoryProcess subCategoryProcess in subCategories)
                {
                    if (subCategoryProcess.StreetNames.Count == 0)
                        continue;

                    nProjectStreetCount += subCategoryProcess.StreetNames.Count;

                    AddTableRow(posCurrent.List, posCurrent.Parameter, posCurrent.Position);
                    posCurrent.List += 10;

                    int nSubCategoryCompleteArea = 0, nSubCategoryIncompleteArea = 0;
                    long nSubCategoryCost = 0;
                    Dictionary<int, int> dicSubCategoryProcessArea = new Dictionary<int, int>();

                    int nSubCategoryList = posCurrent.List;

                    int nSubCategoryStreetCount = LoadSubCategoryProcess(pair.Key, subCategoryProcess, posCurrent, dicSubCategoryProcessArea, ref nSubCategoryCompleteArea, ref nSubCategoryIncompleteArea, ref nSubCategoryCost);
                    /*foreach (string strStreetName in subCategoryProcess.StreetNames)
                    {
                        AddTableRow(posCurrent.List, posCurrent.Parameter, posCurrent.Position);
                        posCurrent.List += 10;

                        InsertText(posCurrent.List, 0, 0, strStreetName);

                        int nProcessIndex = 0;
                        VariousData<DateTime> firstDate = null;
                        VariousData<long> nScheduleCost = null;
                        long nResultCost = 0;
                        int nCompleteArea = 0;
                        VariousData<int> scheduleArea = null;

                        foreach (Process process in pair.Key.ProcessList)
                        {
                            bool processAreaText = false;

                            if (subCategoryProcess.StreetProcessList.TryGetValue(process.ProcessName, out streetList))
                            {
                                Street street = FindStreet(strStreetName, streetList);

                                if (street != null)
                                {
                                    if (street.ScheduleArea == null)
                                        InsertText(posCurrent.List + 6 + nProcessIndex, 0, 0, "-");
                                    else
                                        InsertText(posCurrent.List + 6 + nProcessIndex, 0, 0, ToString(street.ScheduleArea.Data));

                                    processAreaText = true;

                                    if (street.FirstDate != null && firstDate == null)
                                        firstDate = street.FirstDate;

                                    if (street.ScheduleCost != null && nScheduleCost == null)
                                        nScheduleCost = street.ScheduleCost;

                                    if (street.ResultCost != null)
                                        nResultCost += street.ResultCost.Data;

                                    if (street.ScheduleArea != null)
                                    {
                                        if (scheduleArea == null)
                                            scheduleArea = new VariousData<int>(street.ScheduleArea.Data);
                                        else
                                            scheduleArea.Data += street.ScheduleArea.Data;

                                        int nProcessArea;

                                        if (dicSubCategoryProcessArea.TryGetValue(nProcessIndex, out nProcessArea))
                                            dicSubCategoryProcessArea[nProcessIndex] = nProcessArea + street.ScheduleArea.Data;
                                        else
                                            dicSubCategoryProcessArea[nProcessIndex] = street.ScheduleArea.Data;
                                    }
                                }
                            }

                            if (!processAreaText && nProcessIndex < 3)
                                InsertText(posCurrent.List + 6 + nProcessIndex, 0, 0, "-");

                            nProcessIndex++;
                        }

                        if (scheduleArea == null)
                        {
                            InsertText(posCurrent.List + 1, 0, 0, "-");
                            InsertText(posCurrent.List + 2, 0, 0, "-");
                            InsertText(posCurrent.List + 3, 0, 0, "-");
                        }
                        else
                        {
                            InsertText(posCurrent.List + 1, 0, 0, ToString(scheduleArea.Data));
                            InsertText(posCurrent.List + 2, 0, 0, ToString(nCompleteArea));
                            InsertText(posCurrent.List + 3, 0, 0, ToString(scheduleArea.Data));
                        }

                        if (firstDate == null)
                            InsertText(posCurrent.List + 4, 0, 0, "-");
                        else
                            InsertText(posCurrent.List + 4, 0, 0, firstDate.Data.ToShortDateString());

                        if (nScheduleCost == null)
                            InsertText(posCurrent.List + 5, 0, 0, "-");
                        else
                            InsertText(posCurrent.List + 5, 0, 0, ToString((nScheduleCost.Data - nResultCost) / 1000000));

                        if (scheduleArea != null)
                        {
                            nSubCategoryIncompleteArea += (scheduleArea.Data - nCompleteArea);
                            nSubCategoryCompleteArea += nCompleteArea;
                        }

                        if (nScheduleCost != null)
                        {
                            nSubCategoryCost = nScheduleCost.Data - nResultCost;
                        }
                    }*/

                    string strSubCategoryName = subCategoryProcess.SubCategoryName + string.Format(" 소계\r\n({0}개소)", nSubCategoryStreetCount); //subCategoryProcess.StreetNames.Count);

                    InsertText(nSubCategoryList, 0, 0, strSubCategoryName);
                    InsertText(nSubCategoryList + 1, 0, 0, ToString(nSubCategoryIncompleteArea + nSubCategoryCompleteArea));
                    InsertText(nSubCategoryList + 2, 0, 0, ToString(nSubCategoryCompleteArea));
                    InsertText(nSubCategoryList + 3, 0, 0, ToString(nSubCategoryIncompleteArea));
                    InsertText(nSubCategoryList + 5, 0, 0, ToString(nSubCategoryCost / 1000000));

                    for (int i = 0; i < pair.Key.ProcessList.Count && i < 3; i++)
                    {
                        int nProcessArea;

                        if (dicSubCategoryProcessArea.TryGetValue(i, out nProcessArea))
                        {
                            InsertText(nSubCategoryList + 6 + i, 0, 0, ToString(nProcessArea));

                            int nProjectArea;

                            if (dicProjectProcessArea.TryGetValue(i, out nProjectArea))
                                dicProjectProcessArea[i] = nProjectArea + nProcessArea;
                            else
                                dicProjectProcessArea[i] = nProcessArea;
                        }
                        else
                            InsertText(nSubCategoryList + 6 + i, 0, 0, "-");
                    }

                    nProjectCompleteArea += nSubCategoryCompleteArea;
                    nProjectIncompleteArea += nSubCategoryIncompleteArea;
                    nProjectCost += nSubCategoryCost;
                }

                string strTotalName = string.Format("합계\r\n({0}개소)", nProjectStreetCount);

                InsertText(posTotal.List, 0, 0, strTotalName);
                InsertText(posTotal.List + 1, 0, 0, ToString(nProjectIncompleteArea + nProjectCompleteArea));
                InsertText(posTotal.List + 2, 0, 0, ToString(nProjectCompleteArea));
                InsertText(posTotal.List + 3, 0, 0, ToString(nProjectIncompleteArea));
                InsertText(posTotal.List + 5, 0, 0, ToString(nProjectCost / 1000000));

                for (int i = 0; i < pair.Key.ProcessList.Count && i < 3; i++)
                {
                    int nProcessArea;

                    if (dicProjectProcessArea.TryGetValue(i, out nProcessArea))
                        InsertText(posTotal.List + 6 + i, 0, 0, ToString(nProcessArea));
                    else
                        InsertText(posTotal.List + 6 + i, 0, 0, "-");
                }
            }

            DeleteWaste(nProjectIndex + 1);
            return true;
        }

        private int LoadSubCategoryProcess(Project project, SubCategoryProcess subCategoryProcess, HWPPosition posCurrent, Dictionary<int, int> dicSubCategoryProcessArea, ref int nSubCategoryCompleteArea, ref int nSubCategoryIncompleteArea, ref long nSubCategoryCost)
        {
            int nStreetCount = 0;

            foreach (string strStreetName in subCategoryProcess.StreetNames)
            {
                List<Street> streets = GetStreets(strStreetName, project.ProcessList);

                foreach (Street street in streets)
                {
                    nStreetCount++;

                    IncreaseCellHeight(4);
                    AddTableRow(posCurrent.List, posCurrent.Parameter, posCurrent.Position);
                    posCurrent.List += 10;

                    InsertText(posCurrent.List, 0, 0, strStreetName);

                    int nProcessIndex = 0;
                    VariousData<DateTime> firstDate = null;
                    VariousData<long> nScheduleCost = null;
                    long nResultCost = 0;
                    int nCompleteArea = 0;
                    VariousData<int> scheduleArea = null;

                    foreach (Process process in project.ProcessList)
                    {
                        bool processAreaText = false;

                        if (street.Process == process)
                        {
                            if (street != null)
                            {
                                if (street.ScheduleArea == null)
                                    InsertText(posCurrent.List + 6 + nProcessIndex, 0, 0, "-");
                                else
                                    InsertText(posCurrent.List + 6 + nProcessIndex, 0, 0, ToString(street.ScheduleArea.Data));

                                processAreaText = true;

                                if (street.FirstDate != null && firstDate == null)
                                    firstDate = street.FirstDate;

                                if (street.ScheduleCost != null && nScheduleCost == null)
                                    nScheduleCost = street.ScheduleCost;

                                if (street.ResultCost != null)
                                    nResultCost += street.ResultCost.Data;

                                if (street.CompleteArea != null)
                                    nCompleteArea += street.CompleteArea.Data;

                                if (street.ScheduleArea != null)
                                {
                                    if (scheduleArea == null)
                                        scheduleArea = new VariousData<int>(street.ScheduleArea.Data);
                                    else
                                        scheduleArea.Data += street.ScheduleArea.Data;

                                    int nProcessArea;

                                    if (dicSubCategoryProcessArea.TryGetValue(nProcessIndex, out nProcessArea))
                                        dicSubCategoryProcessArea[nProcessIndex] = nProcessArea + street.ScheduleArea.Data;
                                    else
                                        dicSubCategoryProcessArea[nProcessIndex] = street.ScheduleArea.Data;
                                }
                            }
                        }

                        if (!processAreaText && nProcessIndex < 3)
                            InsertText(posCurrent.List + 6 + nProcessIndex, 0, 0, "-");

                        nProcessIndex++;
                    }

                    if (scheduleArea == null)
                    {
                        InsertText(posCurrent.List + 1, 0, 0, "-");
                        InsertText(posCurrent.List + 2, 0, 0, "-");
                        InsertText(posCurrent.List + 3, 0, 0, "-");
                    }
                    else
                    {
                        InsertText(posCurrent.List + 1, 0, 0, ToString(scheduleArea.Data));
                        InsertText(posCurrent.List + 2, 0, 0, ToString(nCompleteArea));
                        InsertText(posCurrent.List + 3, 0, 0, ToString(scheduleArea.Data - nCompleteArea));
                    }

                    if (firstDate == null)
                        InsertText(posCurrent.List + 4, 0, 0, "-");
                    else
                        InsertText(posCurrent.List + 4, 0, 0, firstDate.Data.ToShortDateString());

                    if (nScheduleCost == null)
                        InsertText(posCurrent.List + 5, 0, 0, "-");
                    else
                        InsertText(posCurrent.List + 5, 0, 0, ToString((nScheduleCost.Data - nResultCost) / 1000000));

                    if (scheduleArea != null)
                    {
                        nSubCategoryIncompleteArea += (scheduleArea.Data - nCompleteArea);
                        nSubCategoryCompleteArea += nCompleteArea;
                    }

                    if (nScheduleCost != null)
                    {
                        nSubCategoryCost += nScheduleCost.Data - nResultCost;
                    }
                }
            }

            IncreaseCellHeight(4);

            return nStreetCount;
        }

        private List<Street> GetStreets(string strStreetName, List<Process> processList)
        {
            List<Street> streets = new List<Street>();

            foreach (Process process in processList)
            {
                foreach (Street street in process.StreetList)
                {
                    if (street.StreetName == strStreetName)
                        streets.Add(street);
                }
            }

            return streets;
        }

        private void DeleteWaste(int nProjectIndex)
        {
            string strBookMarkName = string.Format("단계별_이름_{0}", nProjectIndex);
            SetBookMarkPosition(strBookMarkName);
            MoveLineUp();

            if (nProjectIndex > 1)
                MoveLineUp();

            DeleteToEnd();
        }

        private void LoadLevelName(Project project, int nProjectIndex)
        {
            int nProcessCount = project.ProcessList.Count;
            if (nProcessCount == 0)
                return;

            string strLevelName = "", strBookMarkName = "";
            Process firstProcess = project.ProcessList[0];
            Process lastProcess = project.ProcessList[nProcessCount - 1];

            for (int i = 0; i < nProcessCount && i < 3;i++ )
            {
                Process process = project.ProcessList[i];

                if (firstProcess.BeginYear == null)
                {
                    strLevelName = process.Description;
                }
                else
                {
                    if (process == lastProcess && process != firstProcess)
                        strLevelName = string.Format("장기미집행\r\n({0})", process.Description);
                    else if (process.BeginYear != null && process.EndYear != null)
                        strLevelName = string.Format("{0}~{1}년차\r({2})", process.BeginYear.Data - firstProcess.BeginYear.Data + 1, process.EndYear.Data - firstProcess.BeginYear.Data + 1, process.Description);
                    else
                        strLevelName = process.Description;
                }

                strBookMarkName = string.Format("단계별_{0}_집행계획_위제목_{1}", nProjectIndex, i + 1);
                SetBookMarkPosition(strBookMarkName);
                InsertText(process.ProcessName);

                strBookMarkName = string.Format("단계별_{0}_집행계획_아래제목_{1}", nProjectIndex, i + 1);
                SetBookMarkPosition(strBookMarkName);
                InsertText(strLevelName);
            }

            strBookMarkName = string.Format("단계별_이름_{0}", nProjectIndex);
            SetBookMarkPosition(strBookMarkName);
            InsertText(project.RegionName);
        }

        private List<SubCategoryProcess> ToList(Dictionary<string, SubCategoryProcess> dicSubCategories)
        {
            SubCategoryProcess bigProcess = null;
            SubCategoryProcess middleProcess = null;
            SubCategoryProcess smallProcess = null;
            List<SubCategoryProcess> subCategories = new List<SubCategoryProcess>();

            foreach (KeyValuePair<string, SubCategoryProcess> pair in dicSubCategories)
            {
                if (pair.Key == "대로")
                    bigProcess = pair.Value;
                else if (pair.Key == "중로")
                    middleProcess = pair.Value;
                else if (pair.Key == "소로")
                    smallProcess = pair.Value;
                else
                    subCategories.Add(pair.Value);

                pair.Value.StreetNames.Sort();
            }

            subCategories.Sort();

            if (smallProcess != null)
                subCategories.Insert(0, smallProcess);

            if (middleProcess != null)
                subCategories.Insert(0, middleProcess);

            if (bigProcess != null)
                subCategories.Insert(0, bigProcess);

            return subCategories;
        }

        private Street FindStreet(string strStreetName, List<Street> streetList)
        {
            foreach (Street street in streetList)
            {
                if (street.StreetName == strStreetName)
                    return street;
            }

            return null;
        }

        private HWPPosition GetCurrentPosition()
        {
            int List = 0, para = 0, pos = 0;
            m_hwpCtrl.GetPos(ref List, ref para, ref pos);
            return new HWPPosition(List, para, pos);
        }

        private bool LoadTotalTable(Dictionary<string, CategoryData> dicAllCategories, Dictionary<Project, List<CategoryData>> dicProjectCategories)
        {
            string strFirst = "교통시설";
            CategoryData firstData = null;

            SetBookMarkPosition("총괄표_구분_카테고리");
            int nCategoryIndex = 1, nSubCategoryIndex = 1;
            int nCategoryCapacity = 16;

            // Key : CategoryName + "_" + SubCategoryName
            Dictionary<string, HWPPosition> dicSubCategoryPositions = new Dictionary<string, HWPPosition>();
            // Key : CategoryName
            Dictionary<string, HWPPosition> dicCategoryPositions = new Dictionary<string, HWPPosition>();

            if (dicAllCategories.TryGetValue(strFirst, out firstData))
            {
                SetTotalCategoryName(firstData, ref nCategoryIndex, ref nSubCategoryIndex, nCategoryCapacity, dicCategoryPositions, dicSubCategoryPositions);
            }

            foreach (KeyValuePair<string, CategoryData> pair in dicAllCategories)
            {
                if (pair.Value == firstData)
                    continue;

                SetTotalCategoryName(pair.Value, ref nCategoryIndex, ref nSubCategoryIndex, nCategoryCapacity, dicCategoryPositions, dicSubCategoryPositions);
            }

            //현재 위치한 커서값을 받아온다.
            int List = 0, para = 0, pos = 0;
            m_hwpCtrl.GetPos(ref List, ref para, ref pos);

            for (int i = nCategoryIndex; i <= nCategoryCapacity;i++ )
            {
                DeleteRow(List, para, pos);
                DeleteRow(List, para, pos);
                DeleteRow(List, para, pos);
            }

            for (int i = 0; i < 3;i++ )
            {
                SetProjectLevel(i, dicAllCategories, dicProjectCategories, dicCategoryPositions, dicSubCategoryPositions);
            }

            SetTotalSum(dicCategoryPositions, dicSubCategoryPositions);

            return true;
        }

        private void SetTotalSum(Dictionary<string, HWPPosition> dicCategoryPositions, Dictionary<string, HWPPosition> dicSubCategoryPositions)
        {
            foreach (KeyValuePair<string, HWPPosition> pair in dicSubCategoryPositions)
            {
                if (pair.Value.Tag == null)
                    InsertText(pair.Value.List + 1, pair.Value.Parameter, pair.Value.Position, "-");
                else
                {
                    VariousData<int> nArea = (VariousData<int>)pair.Value.Tag;
                    InsertText(pair.Value.List + 1, pair.Value.Parameter, pair.Value.Position, ToString(nArea.Data));
                }
            }

            int nTotalArea = -1;

            foreach (KeyValuePair<string, HWPPosition> pair in dicCategoryPositions)
            {
                if (pair.Value.Tag == null)
                    InsertText(pair.Value.List + 1, pair.Value.Parameter, pair.Value.Position, "-");
                else
                {
                    AreaData areaData = (AreaData)pair.Value.Tag;

                    if (areaData.ScheduleArea == null)
                        InsertText(pair.Value.List + 1, pair.Value.Parameter, pair.Value.Position, "-");
                    else
                    {
                        InsertText(pair.Value.List + 1, pair.Value.Parameter, pair.Value.Position, ToString(areaData.ScheduleArea.Data));

                        if (nTotalArea < 0)
                            nTotalArea = areaData.ScheduleArea.Data;
                        else
                            nTotalArea += areaData.ScheduleArea.Data;
                    }
                }
            }

            SetBookMarkPosition("총괄표_미집행_합계");

            if (nTotalArea < 0)
                InsertText("-");
            else
                InsertText(ToString(nTotalArea));
        }

        private void SetProjectLevel(int nLevelIndex, Dictionary<string, CategoryData> dicAllCategories, Dictionary<Project, List<CategoryData>> dicProjectCategories, Dictionary<string, HWPPosition> dicCategoryPositions, Dictionary<string, HWPPosition> dicSubCategoryPositions)
        {
            List<Process> processList = new List<Process>();
            Dictionary<string, AreaData> dicCategoryAreaData = new Dictionary<string, AreaData>();

            foreach (KeyValuePair<Project, List<CategoryData>> pair in dicProjectCategories)
            {
                if (pair.Key.ProcessList.Count > nLevelIndex)
                {
                    Process process = pair.Key.ProcessList[nLevelIndex];
                    processList.Add(process);
                }
            }

            if (processList.Count == 0)
                return;

            Process firstProcess = processList[0];
            string strLevelName = "";

            if (firstProcess.Description.Length > 0)
                strLevelName = firstProcess.ProcessName + "\r\n(" + firstProcess.Description + ")";
            else
                strLevelName = firstProcess.ProcessName;

            string strBookMarkName = "총괄표_단계_" + (nLevelIndex + 1).ToString();
            SetBookMarkPosition(strBookMarkName);

            InsertText(strLevelName);

            VariousData<int> nScheduleArea, nCompleteArea, nCount;
            
            foreach (KeyValuePair<string, HWPPosition> pair in dicSubCategoryPositions)
            {
                int nIndex = pair.Key.IndexOf('_');

                if (nIndex < 0)
                    continue;

                string strCategoryName = pair.Key.Substring(0, nIndex);
                string strSubCategoryName = pair.Key.Substring(nIndex + 1);
                GetAreaNCount(processList, strCategoryName, strSubCategoryName, out nScheduleArea, out nCompleteArea, out nCount);

                AreaData areaData = null;

                if (!dicCategoryAreaData.TryGetValue(strCategoryName, out areaData))
                {
                    areaData = new AreaData();
                    dicCategoryAreaData[strCategoryName] = areaData;
                }

                if (nScheduleArea == null)
                    InsertText(pair.Value.List + 2 + nLevelIndex * 2, pair.Value.Parameter, pair.Value.Position, "-");
                else
                {
                    InsertText(pair.Value.List + 2 + nLevelIndex * 2, pair.Value.Parameter, pair.Value.Position, ToString(nScheduleArea.Data));

                    if (pair.Value.Tag == null)
                        pair.Value.Tag = nScheduleArea;
                    else
                        ((VariousData<int>)pair.Value.Tag).Data += nScheduleArea.Data;

                    if (areaData.ScheduleArea == null)
                        areaData.ScheduleArea = new VariousData<int>(nScheduleArea.Data);
                    else
                        ((VariousData<int>)areaData.ScheduleArea).Data += nScheduleArea.Data;
                }

                if (nCount == null)
                    InsertText(pair.Value.List + 3 + nLevelIndex * 2, pair.Value.Parameter, pair.Value.Position, "-");
                else
                {
                    InsertText(pair.Value.List + 3 + nLevelIndex * 2, pair.Value.Parameter, pair.Value.Position, ToString(nCount.Data));

                    if (areaData.Count == null)
                        areaData.Count = new VariousData<int>(nCount.Data);
                    else
                        ((VariousData<int>)areaData.Count).Data += nCount.Data;
                }
            }

            int nTotalArea = -1, nTotalCount = -1;

            foreach (KeyValuePair<string, HWPPosition> pair in dicCategoryPositions)
            {
                AreaData areaData = null;

                if (!dicCategoryAreaData.TryGetValue(pair.Key, out areaData))
                    continue;

                if (areaData.ScheduleArea == null && areaData.CompleteArea == null)
                    InsertText(pair.Value.List + 2 + nLevelIndex * 2, pair.Value.Parameter, pair.Value.Position, "-");
                else
                {
                    int nInCompleteArea = 0;

                    if (areaData.ScheduleArea != null)
                        nInCompleteArea += areaData.ScheduleArea.Data;

                    if (areaData.CompleteArea != null)
                        nInCompleteArea -= areaData.CompleteArea.Data;

                    InsertText(pair.Value.List + 2 + nLevelIndex * 2, pair.Value.Parameter, pair.Value.Position, ToString(nInCompleteArea));

                    if (nTotalArea < 0)
                        nTotalArea = nInCompleteArea;
                    else
                        nTotalArea += nInCompleteArea;
                }

                if (areaData.Count == null)
                    InsertText(pair.Value.List + 3 + nLevelIndex * 2, pair.Value.Parameter, pair.Value.Position, "-");
                else
                {
                    InsertText(pair.Value.List + 3 + nLevelIndex * 2, pair.Value.Parameter, pair.Value.Position, ToString(areaData.Count.Data));

                    if (nTotalCount < 0)
                        nTotalCount = areaData.Count.Data;
                    else
                        nTotalCount += areaData.Count.Data;
                }

                if (pair.Value.Tag == null)
                    pair.Value.Tag = areaData;
                else
                {
                    AreaData data2 = (AreaData)pair.Value.Tag;

                    if (data2.ScheduleArea == null)
                        data2.ScheduleArea = areaData.ScheduleArea;
                    else
                    {
                        if (areaData.ScheduleArea != null)
                            data2.ScheduleArea.Data += areaData.ScheduleArea.Data;
                    }

                    if (data2.Count == null)
                        data2.Count = areaData.Count;
                    else
                    {
                        if (areaData.Count != null)
                            data2.Count.Data += areaData.Count.Data;
                    }
                }
            }

            SetBookMarkPosition("총괄표_미집행_합계");

            //현재 위치한 커서값을 받아온다.
            int nList = 0, para = 0, pos = 0;
            m_hwpCtrl.GetPos(ref nList, ref para, ref pos);

            if (nTotalArea < 0)
                InsertText(nList + 1 + nLevelIndex * 2, 0, 0, "-");
            else
                InsertText(nList + 1 + nLevelIndex * 2, 0, 0, ToString(nTotalArea));

            if (nTotalCount < 0)
                InsertText(nList + 2 + nLevelIndex * 2, 0, 0, "-");
            else
                InsertText(nList + 2 + nLevelIndex * 2, 0, 0, ToString(nTotalCount));
        }

        private string ToString(int nData)
        {
            if (nData == 0)
                return "0";

            return string.Format("{0:###,###,###,###,###,###}", nData);
        }

        private string ToString(long nData)
        {
            if (nData == 0)
                return "0";

            return string.Format("{0:###,###,###,###,###,###}", nData);
        }

        private void GetAreaNCount(List<Process> processList, string strCategoryName, string strSubCategoryName, out VariousData<int> nScheduleArea, out VariousData<int> nCompleteArea, out VariousData<int> nCount)
        {
            nScheduleArea = nCompleteArea = nCount = null;

            foreach (Process process in processList)
            {
                foreach (Street street in process.StreetList)
                {
                    if (street.CategoryName == strCategoryName && street.SubCategoryName.Contains(strSubCategoryName))
                    {
                        if (nCount == null)
                            nCount = new VariousData<int>(0);

                        nCount.Data++;

                        if (street.ScheduleArea != null)
                        {
                            if (nScheduleArea == null)
                                nScheduleArea = new VariousData<int>(0);

                            nScheduleArea.Data += street.ScheduleArea.Data;
                        }

                        if (street.CompleteArea != null)
                        {
                            if (nCompleteArea == null)
                                nCompleteArea = new VariousData<int>(0);

                            nCompleteArea.Data += street.CompleteArea.Data;
                        }
                    }
                }
            }
        }

        private void SetTotalCategoryName(CategoryData data, ref int nCategoryIndex, ref int nSubCategoryIndex, int nCategoryCapacity, Dictionary<string, HWPPosition> dicCategoryPositions, Dictionary<string, HWPPosition> dicSubCategoryPositions)
        {
            if (nCategoryIndex > nCategoryCapacity)
                return;

            //현재 위치한 커서값을 받아온다.
            int nList = 0, para = 0, pos = 0;
            m_hwpCtrl.GetPos(ref nList, ref para, ref pos);

            InsertText(nList, 0, 0, data.CategoryName);
            dicCategoryPositions[data.CategoryName] = new HWPPosition(nList, 0, 0);
            nCategoryIndex++;

            int nBeginIndex = nSubCategoryIndex;
            int nBeginPos = nList;

            foreach (SubCategoryData subData in data.SubCategoryDataList)
            {
                if (nBeginIndex == nSubCategoryIndex)
                {
                    nList += 9;
                    nBeginPos = nList;
                }
                else
                {
                    //IncreaseCellHeight(3);
                    AddTableRow(nList, 0, 0);
                    nList += 9;
                }

                m_hwpCtrl.SetPos(nList, para, pos);
                InsertText(nList, 0, 0, string.Format("{0:00}.", nSubCategoryIndex++) + subData.SubCategoryName);
                dicSubCategoryPositions[data.CategoryName + "_" + subData.SubCategoryName] = new HWPPosition(nList, 0, 0);
            }

            for (int i = 0; i < data.SubCategoryDataList.Count; i++)
            {
                m_hwpCtrl.SetPos(nBeginPos, para, pos);
                IncreaseCellHeight(3);
                nBeginPos += 9;
            }

            DeleteRow(nList + 9, para, pos);

            m_hwpCtrl.SetPos(nList + 8, para, pos);
        }

        private void InsertText(string str)
        {
            //현재 위치한 커서값을 받아온다.
            int List = 0, para = 0, pos = 0;
            m_hwpCtrl.GetPos(ref List, ref para, ref pos);

            m_hwpCtrl.SetPos(List, para, pos);
            HWPCONTROLLib.DHwpAction ac = (HWPCONTROLLib.DHwpAction)m_hwpCtrl.CreateAction("InsertText");
            HWPCONTROLLib.DHwpParameterSet cs = (HWPCONTROLLib.DHwpParameterSet)ac.CreateSet();
            cs.SetItem("Text", str);

            ac.Execute(cs);
        }

        // 현재위치로부터 문서끝까지 삭제
        private void DeleteToEnd()
        {
            HWPCONTROLLib.DHwpAction ac = (HWPCONTROLLib.DHwpAction)m_hwpCtrl.CreateAction("MoveSelDocEnd");
            HWPCONTROLLib.DHwpParameterSet cs = (HWPCONTROLLib.DHwpParameterSet)ac.CreateSet();
            ac.Execute(cs);

            ac = (HWPCONTROLLib.DHwpAction)m_hwpCtrl.CreateAction("Delete");
            cs = (HWPCONTROLLib.DHwpParameterSet)ac.CreateSet();
            ac.Execute(cs);
        }

        private void InsertText(int a, int b, int c, string str)
        {
            m_hwpCtrl.SetPos(a, b, c);
            HWPCONTROLLib.DHwpAction ac = (HWPCONTROLLib.DHwpAction)m_hwpCtrl.CreateAction("InsertText");
            HWPCONTROLLib.DHwpParameterSet cs = (HWPCONTROLLib.DHwpParameterSet)ac.CreateSet();
            cs.SetItem("Text", str);

            ac.Execute(cs);
        }

        private void IncreaseCellHeight(int nTimes)
        {
            for (int i = 0; i < nTimes; i++)
            {
                HWPCONTROLLib.DHwpAction ac = (HWPCONTROLLib.DHwpAction)m_hwpCtrl.CreateAction("TableResizeExDown");
                HWPCONTROLLib.DHwpParameterSet cs = (HWPCONTROLLib.DHwpParameterSet)ac.CreateSet();

                ac.Execute(cs);
            }
        }

        private void AddTableRow(int a, int b, int c)
        {
            m_hwpCtrl.SetPos(a, b, c);

            HWPCONTROLLib.DHwpAction ac = (HWPCONTROLLib.DHwpAction)m_hwpCtrl.CreateAction("TableAppendRow");
            HWPCONTROLLib.DHwpParameterSet cs = (HWPCONTROLLib.DHwpParameterSet)ac.CreateSet();

            ac.Execute(cs);
        }

        private Dictionary<Project, List<CategoryData>> ReadProjectCategory()
        {
            Dictionary<Project, List<CategoryData>> dicProjectCategories = new Dictionary<Project, List<CategoryData>>();

            foreach (Project prj in m_listProject)
            {
                List<CategoryData> categoryDatas = ReadProjectCategory(prj);

                if (categoryDatas == null)
                    return null;
                else
                    dicProjectCategories[prj] = categoryDatas;
            }

            return dicProjectCategories;
        }

        private List<CategoryData> ReadProjectCategory(Project prj)
        {
            List<CategoryData> datas = new List<CategoryData>();

            foreach (Process process in prj.ProcessList)
            {
                foreach (Street street in process.StreetList)
                {
                    if (street.CategoryName.Length == 0)
                        continue;

                    CategoryData data = FindCategoryData(street.CategoryName, datas);

                    if (data == null)
                    {
                        data = new CategoryData();
                        data.CategoryName = street.CategoryName;
                        datas.Add(data);
                    }

                    string strSubCategoryName = street.SubCategoryName.Length == 0 ? m_emptySubCategoryName : street.SubCategoryName;
                    SubCategoryData subData = data.GetSubCategory(strSubCategoryName);

                    if (subData == null)
                    {
                        subData = new SubCategoryData();
                        subData.SubCategoryName = strSubCategoryName;
                        data.SubCategoryDataList.Add(subData);
                    }

                    subData.StreetList.Add(street);
                }
            }

            return datas;
        }

        private CategoryData FindCategoryData(string strCategoryName, List<CategoryData> datas)
        {
            foreach (CategoryData data in datas)
            {
                if (data.CategoryName == strCategoryName)
                    return data;
            }

            return null;
        }

        private Dictionary<string, CategoryData> ReadAllCategory()
        {
            Dictionary<string, CategoryData> dicAllCategories = new Dictionary<string, CategoryData>();

            foreach (Project prj in m_listProject)
            {
                ReadCategory(prj, dicAllCategories);
            }

            foreach (KeyValuePair<string, CategoryData> pair in dicAllCategories)
            {
                pair.Value.Sort();
            }

            return dicAllCategories;
        }

        private void ReadCategory(Project prj, Dictionary<string, CategoryData> dicCategories)
        {
            CategoryData data;

            foreach (Process process in prj.ProcessList)
            {
                foreach (Street street in process.StreetList)
                {
                    if (street.CategoryName.Length == 0)
                        continue;

                    if (street.CategoryName == "공간시설" && street.SubCategoryName == "소로")
                    {
                        System.Diagnostics.Trace.WriteLine(street.ToString());
                    }

                    if (!dicCategories.TryGetValue(street.CategoryName, out data))
                    {
                        data = new CategoryData();
                        data.CategoryName = street.CategoryName;
                        dicCategories[street.CategoryName] = data;
                    }

                    string strSubCategoryName = street.SubCategoryName.Length == 0 ? m_emptySubCategoryName : street.SubCategoryName;
                    // 주차장1, 주차장2등을 주차장으로 만든다.
                    strSubCategoryName = RemoveNumber(strSubCategoryName);

                    SubCategoryData subData = FindSubCategory(data, strSubCategoryName, false);
                    //SubCategoryData subData = data.GetSubCategory(strSubCategoryName);

                    if (subData == null)
                    {
                        subData = new SubCategoryData();
                        subData.SubCategoryName = strSubCategoryName;
                        data.SubCategoryDataList.Add(subData);
                    }
                }
            }
        }

        private string RemoveNumber(string str)
        {
            int len = str.Length;

            for (int i=0;i<len;i++)
            {
                char ch = str.ElementAt(i);

                if (ch >= '0' && ch <= '9')
                {
                    if (i == 0)
                        return "";
                    else
                        return str.Substring(0, i);
                }
            }

            return str;
        }

        private SubCategoryData FindSubCategory(CategoryData data, string strSubCategoryName, bool exactlySame)
        {
            foreach (SubCategoryData subData in data.SubCategoryDataList)
            {
                if (exactlySame)
                {
                    if (subData.SubCategoryName == strSubCategoryName)
                        return subData;
                }
                else
                {
                    if (subData.SubCategoryName.Contains(strSubCategoryName))
                        return subData;
                }
            }

            return null;
        }

        // 책갈피 위치로 이동
        private void SetBookMarkPosition(string strMarkName)
        {
            HWPCONTROLLib.DHwpAction ac = (HWPCONTROLLib.DHwpAction)m_hwpCtrl.CreateAction("Bookmark");
            HWPCONTROLLib.DHwpParameterSet cs = (HWPCONTROLLib.DHwpParameterSet)ac.CreateSet();
            cs.SetItem("Name", strMarkName);
            cs.SetItem("Type", 0);
            cs.SetItem("Command", 1);
            ac.Execute(cs);
        }

        private void DeleteRow(int a, int b, int c)
        {
            m_hwpCtrl.SetPos(a, b, c);
            HWPCONTROLLib.DHwpAction ac = (HWPCONTROLLib.DHwpAction)m_hwpCtrl.CreateAction("TableDeleteRow");
            HWPCONTROLLib.DHwpParameterSet cs = (HWPCONTROLLib.DHwpParameterSet)ac.CreateSet();

            ac.Execute(cs);
        }

        private void MoveLineUp()
        {
            HWPCONTROLLib.DHwpAction ac = (HWPCONTROLLib.DHwpAction)m_hwpCtrl.CreateAction("MoveLineUp");
            HWPCONTROLLib.DHwpParameterSet cs = (HWPCONTROLLib.DHwpParameterSet)ac.CreateSet();

            ac.Execute(cs);
        }

        private void MoveLineDown()
        {
            HWPCONTROLLib.DHwpAction ac = (HWPCONTROLLib.DHwpAction)m_hwpCtrl.CreateAction("MoveLineUp");
            HWPCONTROLLib.DHwpParameterSet cs = (HWPCONTROLLib.DHwpParameterSet)ac.CreateSet();

            ac.Execute(cs);
        }

        private void MoveToBegin()
        {
            HWPCONTROLLib.DHwpAction ac = (HWPCONTROLLib.DHwpAction)m_hwpCtrl.CreateAction("MoveDocBegin");
            HWPCONTROLLib.DHwpParameterSet cs = (HWPCONTROLLib.DHwpParameterSet)ac.CreateSet();

            ac.Execute(cs);
        }
    }
}
