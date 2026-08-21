using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Dynamic;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using DBUtility2;


namespace SOPMonitoringSystem
{
    //public class SupervisorSOPClose : IDisposable
    //{
    //    private Thread m_monitoringThread = null;
    //    private bool m_bExitThread = false;

    //    private List<SOPCheckData> m_CheckData = new List<SOPCheckData>();
    //    private SortedList<int, SOPCheckData> m_CheckList = new SortedList<int, SOPCheckData>();
    //    private object m_LockObj = new object();


    //    private Dictionary<int, int> sameSensorGroupDic = new Dictionary<int, int>();        //sensorHistoryID(미수신), sensorHistoryID(수신)
            

    //    public void AddPythonFunction()
    //    {
    //        SDMS.ScriptProxy proxy = SDMS.ScriptProxy.Instance;
          
    //        proxy.UserObject.SupervisorSOPTouch = new Action<int>(TouchSOP);

    //        proxy.UserObject.SupervisorSOPObtainControlAuthority = new Action(ObtainControlAuthority);
    //        proxy.UserObject.SupervisorSOPLostControlAuthority = new Action(LostContorlAuthority);
            
    //        proxy.UserObject.SupervisorSOPAddSOP = new Action<int,int, int>(AddSOP);
    //        proxy.UserObject.SupervisorSOPSensorClose = new Action<int, int>(SensorClose);
    //        proxy.UserObject.SupervisorSOPRemoveSOP = new Action<int>(RemoveSOP);
    //    }

    //    public static void SupervisorSOPTouch(int nActionStepHistory)
    //    {
    //        if (m_instance != null)
    //            m_instance.TouchSOP(nActionStepHistory);
    //    }

    //    public static void SupervisorSOPObtainControlAuthority()
    //    {
    //        if (m_instance != null)
    //            m_instance.ObtainControlAuthority();
    //    }

    //    public static void SupervisorSOPLostControlAuthority()
    //    {
    //        if (m_instance != null)
    //            m_instance.LostContorlAuthority();
    //    }

    //    public static void SupervisorSOPAddSOP(int nActionStepHistoryID, int nSensorZoneID, int nSensorZoneHistoryID)
    //    {
    //        if (m_instance != null)
    //            m_instance.AddSOP(nActionStepHistoryID, nSensorZoneID, nSensorZoneHistoryID);
    //    }

    //    public static void SupervisorSOPSensorClose(int nSensorZoneID, int nSensorZoneHistoryID)
    //    {
    //        if (m_instance != null)
    //            m_instance.SensorClose(nSensorZoneID, nSensorZoneHistoryID);
    //    }
    //    //먼저 돌고 있는 sop에 같은 sensor group이 있을 때 해당 로직처리를 위한 dictionary
    //    //1개 이상의 sensorgroup이 있을 경우를 위한 처리. 
    //    public static void RegisterSameSensorGroupRunning(int sensorHistoryID, int activeSensorHistoryID)
    //    {
    //        if (m_instance != null)
    //            m_instance.sameSensorGroupDic.Add(sensorHistoryID, activeSensorHistoryID);
    //    }
        
    //    public static void SupervisorSOPRemoveSOP(int nActionStepHistoryID)
    //    {
    //        if (m_instance != null)
    //            m_instance.RemoveSOP(nActionStepHistoryID);
    //    }
    //    private WebDBManager m_dbMgr = null;
    //    public SupervisorSOPClose(WebDBManager dbMgr)
    //    {
    //        AddPythonFunction();

    //        m_dbMgr = dbMgr;

    //        m_instance = this;
    //    }

    //    private static SupervisorSOPClose m_instance = null;

    //    public void Dispose()
    //    {
    //        if (m_monitoringThread != null)
    //        {
    //            m_bExitThread = true;
    //            m_monitoringThread.Join();
    //        }
    //    }

    //    private void BeginCheckSOP()
    //    {
    //        //r//eturn;

    //        if( m_monitoringThread == null)
    //        {
    //            //LoadAllData();
                
    //            m_monitoringThread = new Thread(MonitorSOP);
    //            m_monitoringThread.Name = "ClsoeSOP Monitor";
    //            m_monitoringThread.Start();
    //        }
    //    }

    //    private void MonitorSOP()
    //    {
    //        List<SOPCheckData> deleteItems = new List<SOPCheckData>();
    //        List<SOPCheckData> checkItems = new List<SOPCheckData>();

    //        lock(m_LockObj)
    //        {
    //            m_CheckData.Clear();
    //            m_CheckList.Clear();
    //        }

    //        // Read All History
    //        LoadAllData();

    //        while(m_bExitThread == false)
    //        {
    //            deleteItems.Clear();
    //            checkItems.Clear();

    //            DateTime dtNow = DateTime.Now;

    //            lock (m_LockObj)
    //            {
    //                checkItems.AddRange(m_CheckData);
    //            }

    //            foreach (SOPCheckData data in checkItems)
    //            {
    //                if(data.CheckedSensorClose == true && data.CheckedTimeClose == true)
    //                {
    //                    deleteItems.Add(data);
    //                    continue;
    //                }

    //                if (m_bExitThread == true)
    //                    break;

    //                // Check Touch Time if CloseNoInput is on
    //                if(data.CloseNoInput == true)
    //                {
    //                    TimeSpan span = dtNow - data.TouchTime;
    //                    if( span.TotalSeconds > data.CloseNoInputTime)
    //                    {
    //                        data.CheckedTimeClose2 = true;
    //                        deleteItems.Add(data);
    //                    }
    //                }

    //                if (m_bExitThread == true)
    //                    break;
                    
    //                // Check Sensor Close Recived if CloseSensorClose is on
    //                if(data.CloseSensorClose == true)
    //                {
    //                    if (data.ReciveSensorClose == false)
    //                    {
    //                        if (CheckSensorClose(data.SensorZoneHistoryID))
    //                        {
    //                            data.ReciveSensorClose = true;
    //                            data.CheckTime = DateTime.Now;
    //                        }
    //                    }

    //                    if ( data.ReciveSensorClsseTime == true)
    //                    {
    //                        // check reicve span time 
    //                        if (data.ReciveSensorClose == true)
    //                        {
    //                            TimeSpan span = dtNow - data.CheckTime;
    //                            if( span.TotalSeconds > data.CloseSensorWaitTime)
    //                            {
    //                                data.CheckedSensorClose2 = true;
    //                                deleteItems.Add(data);
    //                            }                                
    //                        }                            
    //                    }
    //                    else
    //                    {
    //                        if(data.ReciveSensorClose == true)
    //                        {
    //                            data.CheckedSensorClose2 = true;
    //                            deleteItems.Add(data);
    //                        }
    //                    }
    //                }

    //                if (m_bExitThread == true)
    //                    break;
    //                List<int> deleteTemps = new List<int>();
    //                if (sameSensorGroupDic.Count > 0)
    //                {
    //                    foreach (KeyValuePair<int,int> pair in sameSensorGroupDic)
    //                    {
    //                        if (CheckSensorClose(pair.Key))      //같은 센서존에 있는 센서가 Close 되었는지 체크.(해당 센서는 history만 있고, SDMS,SOP에 수신 처리 되지 않음) {
    //                        {
    //                            if (CheckSensorClose(data.SensorZoneHistoryID) && (data.SensorZoneHistoryID == pair.Value))
    //                            {
    //                                //data.CheckedSensorClose = true;
    //                                //data.CheckedTimeClose = true;
    //                                deleteItems.Add(data);
    //                                deleteTemps.Add(pair.Key);
    //                            }
                                
    //                        }
    //                        //else if (CheckSensorClose(pair.Value))    //같은 센서 존에 있는 그룹에 활성화된 센서가 종료된 경우 해제하기 위함.
    //                        //{
    //                        //    deleteTemps.Add(pair.Key);
    //                        //}
    //                    }
    //                    foreach (int beDeleteID in deleteTemps)
    //                    {
    //                        sameSensorGroupDic.Remove(beDeleteID);
    //                    }
    //                }
    //                else
    //                {
    //                    /*if (CheckSensorClose(data.SensorZoneHistoryID))
    //                    {
    //                        //data.CheckedSensorClose = true;
    //                        //data.CheckedTimeClose = true;
    //                        deleteItems.Add(data);
    //                    }*/
    //                }

                   
    //            }

    //            if (m_bExitThread == true)
    //                break;

    //            // Remove close sop
    //            foreach(SOPCheckData data in deleteItems)
    //            {
    //                if (data.CheckedSensorClose == true && data.CheckedTimeClose == true)
    //                {
    //                    lock (m_LockObj)
    //                    {
    //                        m_CheckData.Remove(data);
    //                        m_CheckList.Remove(data.ActionStepHistoryID);
    //                    }
    //                }                    
    //                else
    //                    CloseSOP(data);
    //            }

    //            for(int i = 0; i < 20 ; i++)
    //            {
    //                if (m_bExitThread == true)
    //                    break;
    //                Thread.Sleep(100);
    //            }
    //        }
    //    }

    //    // 제어권 획득
    //    public void ObtainControlAuthority()
    //    {
    //        m_bExitThread = false;
          
    //        BeginCheckSOP();
    //    }

    //    // 제어권 잃음
    //    public void LostContorlAuthority()
    //    {
    //        m_bExitThread = true;            
    //        m_monitoringThread = null;

    //        //Dispose();
    //    }

    //    // 새로운 SOP 시작
    //    public void AddSOP(int nActionStepHistoryID, int nSensorZoneID, int nSensorZoneHistoryID)
    //    {
    //        // added by skkim 2018-01-08
    //        // 이미 종료된 센서신호가 들어오는 경우 해당 SensorZoneHistoryID를 -1로 지정한다.
    //        /*int nOrgSensorZoneHistoryID = nSensorZoneHistoryID;
    //        if (nSensorZoneHistoryID != -1)
    //        {
    //            if(CheckSensorClose(nSensorZoneHistoryID))
    //            {
    //                nSensorZoneHistoryID = -1;
    //                nSensorZoneID = -1;
    //            }                
    //        }*/

    //        if (nActionStepHistoryID < 0)
    //            throw new ArgumentException("ActionStepHistoryID 가 비정상적입니다.");

    //        UnE.SOP.Workstate.SOPScenario flow = SOPScenarioManager.Instance.GetSOPScenario(nActionStepHistoryID);
    //        if( flow == null)
    //            throw new ArgumentException("ActionStepHistoryID 가 비정상적입니다.");

    //        string szCategoryName = flow.CategoryName;
    //        if( szCategoryName == null || szCategoryName == "")
    //            throw new ArgumentException("ActionStepPath 가 비정상적입니다.");

    //        UnE.SOP.SOPCloseOption option = null;
    //        try
    //        {
    //            option = UnE.SOP.ProxySOP.Instance.OptionSOPAutoCloseSet[szCategoryName];
    //        }
    //        catch(Exception)
    //        { }

    //        if (option == null)
    //            throw new ArgumentException("자동종료 옵션 설정이 비정상적입니다.");

    //        SOPCheckData data = new SOPCheckData();
    //        data.CloseNoInput = option.UseCloseSOPWaitInputTime;
    //        data.CloseSensorClose = (option.UseCloseSOPSensorReset || option.UseCloseSOPSensorResetWaitTime);

    //        // 체크할 것이 없으므로 더할 필요가 없다
    //        if( data.CloseNoInput == false && data.CloseSensorClose == false)
    //        {
    //            return;
    //        }

            
            
    //        data.ReciveSensorClsseTime = option.UseCloseSOPSensorResetWaitTime;

    //        data.CloseNoInputTime = option.CloseSOPWaitInputTime * 60;
    //        data.CloseSensorWaitTime = option.CloseSOPSensorResetWaitTime * 60;

    //        DateTime dtNow = DateTime.Now;
    //        data.CheckTime = dtNow;
    //        data.TouchTime = dtNow;

    //        data.ReciveSensorClose = false;
    //        data.ActionStepHistoryID = nActionStepHistoryID;
    //        data.SensorZoneID = nSensorZoneID;
    //        data.SensorZoneHistoryID = nSensorZoneHistoryID;
                        
    //        if (!m_CheckList.ContainsKey(nActionStepHistoryID))
    //        {
    //            lock (m_LockObj)
    //            {

    //                m_CheckData.Add(data);
    //                m_CheckList.Add(nActionStepHistoryID, data);

    //                System.Diagnostics.Trace.WriteLine("Add SOP Check : " + nActionStepHistoryID + ", " + nSensorZoneID + ", " + nSensorZoneHistoryID);

    //            }

    //            try
    //            {
    //                SaveData(data);
    //            }
    //            catch (Exception ex)
    //            {
    //                System.Diagnostics.Trace.WriteLine(ex.Message);
    //                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
    //            }
    //        }
    //        else
    //        {
    //            System.Diagnostics.Trace.WriteLine("Duplicated SOP Check Data");
    //        }
    //    }

    //    // SOP 종료 처리
    //    private void CloseSOP(SOPCheckData data)
    //    {
    //        if (m_bProcessClose == true)
    //            return;
    //        if (data.Form != null && data.Form.IsDisposed == false && data.Form.Visible == true)
    //            return;
    //        //UnE.SOP.Workstate.SOPScenario sco = SOPScenarioManager.Instance.GetSOPScenario(data.ActionStepHistoryID);
    //        //if (sco == null)
    //        //{
    //        //    data.CheckedSensorClose = true;
    //        //    data.CheckedTimeClose = true;
    //        //    RemoveData(data);
    //        //    return;
    //        //}

            
    //        try
    //        {
    //            Thread t = new Thread(CloseThread);
    //            t.Start(data);
    //        }
    //        catch(Exception ex)
    //        {
    //            System.Diagnostics.Trace.WriteLine(ex.Message);
    //            System.Diagnostics.Trace.WriteLine(ex.StackTrace);
    //        }            
    //    }


    //    private bool m_bProcessClose = false;
    //    private void CloseThread(object param)
    //    {
    //        m_bProcessClose = true;
    //        SOPCheckData data = (SOPCheckData)param;

    //        bool bDelete = false;
    //        int nActionStepHistoryID = data.ActionStepHistoryID;
    //        // ActionStep종료 처리

    //        if (data.Form != null && data.Form.IsDisposed == false && data.Form.Visible == true)
    //        {
    //            m_bProcessClose = false;
    //            return;
    //        }

    //        if (data.Form != null && data.Form.IsDisposed == true)
    //        {
    //            data.Form = new Popup.SOPClose.PopupSOPClose();
    //            data.Form.TopMost = true;
    //        }

    //        Popup.SOPClose.PopupSOPClose form = data.Form;

    //        UnE.SOP.Workstate.SOPScenario sco = null;
    //        // SOP가 화면에 나타날때까지 최대 30초까지만 기다린다.
    //        int nLimit = 30;

    //        for (int i=0;i<nLimit && sco == null;i++)
    //        {
    //            sco = SOPScenarioManager.Instance.GetSOPScenario(nActionStepHistoryID);

    //            if (sco != null)
    //                form.SetSOPName(sco.ActionStepFullPath);
    //            else
    //            {
    //                Thread.Sleep(1000);
    //                continue;
    //            }

    //            //if( form.ShowDialog() == DialogResult.OK)
    //            {
    //                FormSOP.Instance.Invoke((MethodInvoker)delegate
    //                {
    //                    UnE.SOP.Sections.SectionTabPage page = FormSOP.Instance.GetPageHome().GetTabPage(nActionStepHistoryID);
    //                    if (page != null)
    //                    {
    //                        FormSOP.Instance.StopWorkflow(DateTime.Now, false, page.ActionStepID, !page.VirtualMode);
    //                    }
    //                });

    //                bDelete = true;
    //            }


    //            if ((data.CheckedSensorClose == true && data.CheckedTimeClose == true) || bDelete == true)
    //            {

    //                data.CheckedSensorClose = true;
    //                data.CheckedTimeClose = true;

    //                RemoveData(data);
    //            }
    //            else
    //            {
    //                data.CheckedSensorClose = data.CheckedSensorClose2;
    //                data.CheckedTimeClose = data.CheckedTimeClose2;
    //            }
    //        }

    //        m_bProcessClose = false;
    //    }

    //    // SOP Touch
    //    public void TouchSOP(int nActionStepHistory)
    //    {
    //        //return;

    //        lock(m_LockObj)
    //        {
    //            if( m_CheckList.ContainsKey(nActionStepHistory) == true)
    //            {
    //                m_CheckList[nActionStepHistory].TouchTime = DateTime.Now;
    //            }
    //        }
    //    }

    //    // 센서 종료 신호
    //    public void SensorClose(int nSensorZoneID, int nSensorZoneHistoryID)
    //    {
    //        //return;
    //        // 제어권이 없는경우 처리하지 않음
            

    //        if (nSensorZoneID < 0)
    //            return;
            
    //        List<SOPCheckData> checkItems = new List<SOPCheckData>();            
    //        lock (m_LockObj)
    //        {
    //            checkItems.AddRange(m_CheckData);
    //        }

    //        foreach(SOPCheckData data in checkItems)
    //        { 
    //            if( data.SensorZoneID == nSensorZoneID)
    //            {
    //                data.ReciveSensorClose = true;
    //                data.CheckTime = DateTime.Now;

    //                System.Diagnostics.Trace.WriteLine("Check SensorClose  Sensor: " + data.SensorZoneID);
    //            }
    //            else if(data.SensorZoneHistoryID == nSensorZoneHistoryID)
    //            {
    //                data.ReciveSensorClose = true;
    //                data.CheckTime = DateTime.Now;

    //                System.Diagnostics.Trace.WriteLine("Check SensorClose History : " + data.ActionStepHistoryID);
    //            }
    //        }
    //    }

    //    // 외부에서 사용자가 종료하는 ActionStepHistory
    //    public void RemoveSOP(int nActionStepHistoryID)
    //    {
    //        if (nActionStepHistoryID < 0)
    //            return;

    //        SOPCheckData data = null;
    //        lock (m_LockObj)
    //        {
    //            if (m_CheckList.ContainsKey(nActionStepHistoryID) == true)
    //            {
    //                data = m_CheckList[nActionStepHistoryID];
    //                m_CheckList.Remove(nActionStepHistoryID);

    //                if( m_CheckData.Contains(data))
    //                    m_CheckData.Remove(data);

    //                System.Diagnostics.Trace.WriteLine("Close SOP Data  : " + data.ActionStepHistoryID);
    //            }
    //        }
    //        RemoveData(data);
    //    }

    //    private void RemoveData(SOPCheckData data)
    //    {
    //        if (data == null)
    //            return;

    //        string szSQL = "DELETE FROM ActionStepAutoClose WHERE ActionStepHistoryID=" + data.ActionStepHistoryID;
    //        m_dbMgr.GetResultData(szSQL);

    //        System.Diagnostics.Trace.WriteLine("Remove Check Data  : " + data.ActionStepHistoryID );
    //    }

    //    private string GetBeginStatusString()
    //    {
    //        string strBeginStatusString = "(";

    //        List<libSensorProcess.ReactionType> types = new List<libSensorProcess.ReactionType>();
    //        types.Add(libSensorProcess.ReactionType.BEGIN_STATUS);
    //        //types.Add(libSensorProcess.ReactionType.BEGIN_PSM_STATUS);
    //        types.Add(libSensorProcess.ReactionType.CHANGE_ALARM_DEPTH);
    //        //types.Add(libSensorProcess.ReactionType.CHANGE_PSM_ALARM_DEPTH);
    //        types.Add(libSensorProcess.ReactionType.NOTIFY_SIGNAL);
    //        /*types.Add(libSensorProcess.ReactionType.NOTIFY_SECURITY);
    //        types.Add(libSensorProcess.ReactionType.BEGIN_S1SVMS_STATUS);
    //        types.Add(libSensorProcess.ReactionType.BEGIN_S1ACCESS_STATUS);
    //        types.Add(libSensorProcess.ReactionType.BEGIN_SECOM_STATUS);*/

    //        foreach (libSensorProcess.ReactionType type in types)
    //        {
    //            if (strBeginStatusString.Length == 1)
    //                strBeginStatusString += ((int)type).ToString();
    //            else
    //                strBeginStatusString += "," + ((int)type).ToString();
    //        }

    //        strBeginStatusString += ")";
    //        return strBeginStatusString;
    //    }

    //    private string GetEndStatusString()
    //    {
    //        string strEndStatusString = "(";

    //        List<libSensorProcess.ReactionType> types = new List<libSensorProcess.ReactionType>();
    //        types.Add(libSensorProcess.ReactionType.MALFUNCTION);
    //        types.Add(libSensorProcess.ReactionType.USER_RESET);
    //        types.Add(libSensorProcess.ReactionType.IGNORE_SIGNAL);
    //        types.Add(libSensorProcess.ReactionType.IGNORE_SOP);
    //        types.Add(libSensorProcess.ReactionType.END_STATUS);
    //        types.Add(libSensorProcess.ReactionType.TIME_OUT);
    //        /*types.Add(libSensorProcess.ReactionType.END_PSM_STATUS);
    //        types.Add(libSensorProcess.ReactionType.IGNORE_S1SVMS_STATUS);
    //        types.Add(libSensorProcess.ReactionType.END_S1SVMS_STATUS);
    //        types.Add(libSensorProcess.ReactionType.IGNORE_S1ACCESS_STATUS);
    //        types.Add(libSensorProcess.ReactionType.END_S1ACCESS_STATUS);
    //        types.Add(libSensorProcess.ReactionType.IGNORE_SECOM_STATUS);
    //        types.Add(libSensorProcess.ReactionType.END_SECOM_STATUS);*/

    //        foreach (libSensorProcess.ReactionType type in types)
    //        {
    //            if (strEndStatusString.Length == 1)
    //                strEndStatusString += ((int)type).ToString();
    //            else
    //                strEndStatusString += "," + ((int)type).ToString();
    //        }

    //        strEndStatusString += ")";
    //        return strEndStatusString;
    //    }

    //    public bool CheckSensorClose(int nSensorHistoryID)
    //    {
    //        if (nSensorHistoryID <= 0)
    //            return false;

    //        string strEndStatus = GetEndStatusString();

    //        string strSQL = "Select ID from SensorReactionHistory where ReactionType in " + strEndStatus + " and SensorHistoryID = " + nSensorHistoryID.ToString();
    //        ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

    //        if (arrResult != null && arrResult.Count > 0)
    //            return true;

    //        /*DateTime dtTarget = DateTime.Now.AddDays(-1);
    //        string szTimeString = WebDBManager.MakeDateTimeString(dtTarget);

    //        string strBeginStatus = GetBeginStatusString();
    //        string strEndStatus = GetEndStatusString();

    //        string szSQL = "SELECT sz.Data FROM SensorReactionHistory as srh ";
    //        szSQL += " INNER JOIN  SensorZoneHistory as szh on srh.SensorHistoryID = szh.ID ";
    //        szSQL += " INNER JOIN  SensorZone as sz on sz.ID = szh.SensorID ";
    //        szSQL += " WHERE SensorHistoryID in (  SELECT srh2.SensorHistoryID FROM SensorReactionHistory as srh2 WHERE srh2.ReactionType in " + strBeginStatus + ") ";
    //        szSQL += " AND SensorHistoryID not in (  SELECT srh3.SensorHistoryID FROM SensorReactionHistory as srh3 WHERE srh3.ReactionType in " + strEndStatus + ") ";
    //        szSQL += " AND szh.SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();
    //        szSQL += " AND srh.SensorHistoryID = " + nSensorHistoryID.ToString() + " AND (srh.ReactionType != 10 AND srh.ReactionType != 11) ";
    //        szSQL += " AND srh.Time > '" + szTimeString + "' ";
    //        szSQL += " AND ( sz.Data = 1 OR sz.Data = 21 OR sz.Data = 22 OR sz.Data = 23) ";

    //        ArrayList arData = m_dbMgr.GetResultData(szSQL);
    //        if (arData == null || arData.Count == 0)
    //            return true;*/

    //        return false;
    //    }

    //    private int GetMaxID(string strTableName, WebDBManager dbMgr)
    //    {
    //        string strSQL = "select max(ID) from " + strTableName;
    //        ArrayList arrResult = dbMgr.GetResultData(strSQL);

    //        if (arrResult == null || arrResult.Count == 0)
    //            return 0;

    //        return WebDBManager.GetIntField(arrResult[0].ToString(), 0);
    //    }

    //    private bool ExistCheckDataInDB(SOPCheckData data)
    //    {
    //        if( data == null)
    //            return true;
            
    //        string szTmp = "SELECT ID FROM ActionStepAutoClose WHERE ActionStepHistoryID = {0}";
    //        string szSQL = string.Format(szTmp, data.ActionStepHistoryID);

    //        ArrayList arResult = m_dbMgr.GetResultData(szSQL);
    //        if (arResult == null || arResult.Count == 0)
    //            return false;
    //        return true;
    //    }

    //    private void SaveData(SOPCheckData data)
    //    {
    //        if(!ExistCheckDataInDB(data))
    //        {
    //            int nMaxID = GetMaxID("ActionStepAutoClose", m_dbMgr) + 1;
    //            string szTemp = "INSERT INTO ActionStepAutoClose (ID, ActionStepHistoryID,ActionStepID, UseCloseNoInput, " +
    //                           " UseCloseSensorReset, UseCloseSensorResetWaitTime,InputWaitTime, SensorResetWaitTime, " +
    //                           " BeginTime, SensorZoneID, SensorZoneHistoryID ) " +
    //                           " VALUES  ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, '{8}', {9}, {10}) ";
    //            string szSQL = string.Format(szTemp, nMaxID, data.ActionStepHistoryID, "NULL",
    //                         (data.CloseNoInput ? 1 : 0), //3 
    //                         (data.CloseSensorClose ? 1 : 0), // 4
    //                         (data.ReciveSensorClsseTime ? 1 : 0), // 5
    //                         data.CloseNoInputTime,//6
    //                         data.CloseSensorWaitTime,//7
    //                         WebDBManager.MakeDateTimeString(data.CheckTime), //8
    //                         data.SensorZoneID < 0 ? "NULL" : data.SensorZoneID.ToString(),
    //                         data.SensorZoneHistoryID < 0 ? "NULL" : data.SensorZoneHistoryID.ToString());//9,10

    //            m_dbMgr.GetResultData(szSQL);

    //            System.Diagnostics.Trace.WriteLine("Save DB Check Data  : " + data.ActionStepHistoryID);
    //        }            
    //    }

    //    private void LoadAllData()
    //    {
    //        string szSQL = "SELECT asac.ID, asac.ActionStepHistoryID, asac.ActionStepID, asac.UseCloseNoInput, asac.UseCloseSensorReset, asac.UseCloseSensorResetWaitTime, " +
    //                        " asac.InputWaitTime, asac.SensorResetWaitTime, asac.BeginTime, asac.SensorZoneID, asac.SensorZoneHistoryID, asac.Description " +
    //                        " FROM ActionStepAutoClose as asac, ActionStepHistory as ash " +
    //                        " where asac.ActionStepHistoryID = ash.ID and ash.EndTime is NULL and ash.CancelTime is NULL and asac.SensorZoneHistoryID is not null";

    //        ArrayList arResult = m_dbMgr.GetResultData(szSQL);
    //        if (arResult == null || arResult.Count == 0)
    //            return;

    //        for(int i  = 0 ; i < arResult.Count; i+= 12)
    //        {
    //            int nID = WebDBManager.GetIntField(arResult[i].ToString(), -1);
    //            int nActionStepHistoryID = WebDBManager.GetIntField(arResult[i+1].ToString(), -1);
    //            int nActionStepID = WebDBManager.GetIntField(arResult[i+2].ToString(), -1);
    //            int nUseCloseNoInput = WebDBManager.GetIntField(arResult[i+3].ToString(), -1);
    //            int nUseCloseSensorReset = WebDBManager.GetIntField(arResult[i+4].ToString(), -1);
    //            int nUseCloseSensorResetWaitTime = WebDBManager.GetIntField(arResult[i+5].ToString(), -1);
    //            int nInputWaitTime = WebDBManager.GetIntField(arResult[i+6].ToString(), -1);
    //            int nSensorResetWaitTime = WebDBManager.GetIntField(arResult[i+7].ToString(), -1);
    //            DateTime dtBeginTime = WebDBManager.GetDateTimeField(arResult[i+8].ToString(), DateTime.Now);
    //            int nSensorZoneID = WebDBManager.GetIntField(arResult[i+9].ToString(), -1);

    //            int nSensorZoneHistoryID = WebDBManager.GetIntField(arResult[i+10].ToString(), -1);
    //            string szDescription = WebDBManager.GetStringField(arResult[i+11].ToString());

    //            if (nSensorZoneID < 0 || nSensorZoneHistoryID < 0)
    //                continue;

    //            SOPCheckData data = new SOPCheckData();
    //            data.CloseNoInput = nUseCloseNoInput == 1 ? true : false;
    //            data.CloseSensorClose = nUseCloseSensorReset == 1 ? true : false;
    //            data.ReciveSensorClsseTime = nUseCloseSensorResetWaitTime == 1 ? true : false;

    //            data.CloseNoInputTime = nInputWaitTime;
    //            data.CloseSensorWaitTime = nSensorResetWaitTime;

    //            DateTime dtNow = DateTime.Now;
    //            data.CheckTime = dtBeginTime;
    //            data.TouchTime = dtNow;
    //            data.ReciveSensorClose = false;
    //            data.ActionStepHistoryID = nActionStepHistoryID;
    //            data.SensorZoneID = nSensorZoneID;
    //            data.SensorZoneHistoryID = nSensorZoneHistoryID;

    //            if (!m_CheckList.ContainsKey(nActionStepHistoryID))
    //            {
    //                lock (m_LockObj)
    //                {
    //                    m_CheckData.Add(data);
    //                    m_CheckList.Add(nActionStepHistoryID, data);
    //                }
    //            }                
    //        }
    //    }
    //}

    //public class SOPCheckData
    //{
    //    private int m_nSensorZoneID;
    //    public int SensorZoneID
    //    {
    //        get { return m_nSensorZoneID; }
    //        set { m_nSensorZoneID = value; }
    //    }

    //    private int m_nActionStepHistoryID;
    //    public int ActionStepHistoryID
    //    {
    //        get { return m_nActionStepHistoryID; }
    //        set { m_nActionStepHistoryID = value; }
    //    }

    //    private DateTime m_bTouchTime;
    //    public DateTime TouchTime
    //    {
    //        get { return m_bTouchTime; }
    //        set { m_bTouchTime = value; }
    //    }

    //    private DateTime m_bCheckTime;
    //    public DateTime CheckTime
    //    {
    //        get { return m_bCheckTime; }
    //        set { m_bCheckTime = value; }
    //    }

    //    // 입력대기로 인한 자동종료 사용할 것인가?
    //    private bool m_bCloseNoInput;
    //    public bool CloseNoInput
    //    {
    //        get { return m_bCloseNoInput; }
    //        set { m_bCloseNoInput = value; }
    //    }

    //    // 입력대기로 인한 자동종료 대기시간(초)
    //    private int m_bCloseNoInputTime;
    //    public int CloseNoInputTime
    //    {
    //        get { return m_bCloseNoInputTime; }
    //        set { m_bCloseNoInputTime = value; }
    //    }

    //    // 센서신호 복구시 종료할 것인가?
    //    // ReciveSensorClsseTime가 true이면 CloseSensorWaitTime시간 이후 종료
    //    // ReciveSensorClsseTime가 false이면 즉시 종료
    //    private bool m_bCloseSensorClose;
    //    public bool CloseSensorClose
    //    {
    //        get { return m_bCloseSensorClose; }
    //        set { m_bCloseSensorClose = value; }
    //    }

    //    // 센서신호 복구시 일정시간 이후 종료할 때 지연시간(초)
    //    private int m_nCloseSensorWaitTime;
    //    public int CloseSensorWaitTime
    //    {
    //        get { return m_nCloseSensorWaitTime; }
    //        set { m_nCloseSensorWaitTime = value; }
    //    }

    //    private bool m_bReciveSensorClose;
    //    public bool ReciveSensorClose
    //    {
    //        get { return m_bReciveSensorClose; }
    //        set { m_bReciveSensorClose = value; }
    //    }

    //    // 센서신호 복구시 일정시간 이후 종료할 것인가?
    //    // 이 값이 true이면 CloseSensorClose도 true이어야 한다.
    //    private bool m_bReciveSensorClsseTime;       
    //    public bool ReciveSensorClsseTime
    //    {
    //        get { return m_bReciveSensorClsseTime; }
    //        set { m_bReciveSensorClsseTime = value; }
    //    }


    //    private bool m_bCheckedSensorClose = false;
    //    public bool CheckedSensorClose
    //    {
    //        get { return m_bCheckedSensorClose; }
    //        set { m_bCheckedSensorClose = value; }
    //    }

    //    private bool m_bCheckedTimeClose = false;
    //    public bool CheckedTimeClose
    //    {
    //        get { return m_bCheckedTimeClose; }
    //        set { m_bCheckedTimeClose = value; }
    //    }

    //    private bool m_bCheckedSensorClose2 = false;
    //    public bool CheckedSensorClose2
    //    {
    //        get { return m_bCheckedSensorClose2; }
    //        set { m_bCheckedSensorClose2 = value; }
    //    }

    //    private bool m_bCheckedTimeClose2 = false;
    //    public bool CheckedTimeClose2
    //    {
    //        get { return m_bCheckedTimeClose2; }
    //        set { m_bCheckedTimeClose2 = value; }
    //    }


    //    private Popup.SOPClose.PopupSOPClose form = new Popup.SOPClose.PopupSOPClose();
    //    public Popup.SOPClose.PopupSOPClose Form
    //    {
    //        get { return form; }
    //        set { form = value; }
    //    }

    //    public int SensorZoneHistoryID { get; set; }
    //}
}
