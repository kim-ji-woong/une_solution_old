using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FireManagement
{
    public class EventManager
    {
        private static EventManager m_instance = new EventManager();

        public static EventManager Instance
        {
            get { return m_instance; }
        }

        private EventManager()
        {
        }

        public object ProcessEvent(int nEventID, object arg = null)
        {
            switch (nEventID)
            {
                case Event.NEW_DXF_OPENED:
                    OnNewDXFOpened();
                    //if (arg == null)
                        //FormMain2.Instance.PageOpen.HideBuildings();
                    //FormMain2.Instance.ChangeTab(ID.ID_TAB_FIREMANAGEMENT);
                    FormMain2.Instance.SelectFireManagerTab(1);
                    //Default로 Group은 꺼놓음
                    FormMain2.Instance.ViewControl.ShapeGroupLayerOn(false);
                    FormMain2.Instance.ViewControl.BtnGroup.IsChecked = false;
                    break;

                case Event.EQUIP_SELECTED:
                    PostEquipSelected((FireEquipment)arg);
                    break;

                case Event.PREV_OPEN_FMF:
                    PrevOpenFMF();
                    break;

                case Event.POST_OPEN_FMF:
                    PostOpenFMF((string)arg);
                    break;
            }

            return null;
        }

        private void PrevOpenFMF()
        {
            if (!FormMain2.Instance.IsPCMode)
            {
                IOManager ioMgr = FormMain2.Instance.IOManager;

                ioMgr.EquipmentHistory.Clear();
                ioMgr.ClearEquipments();
                ioMgr.ClearDBEquipments();
                ioMgr.AllZones.Clear();
                ioMgr.OutdoorZones.Clear();
                ioMgr.AllBuildings.Clear();
                ioMgr.AllBuildingGroups.Clear();
            }
        }

        private void PostOpenFMF(string strPath)
        {
            FormMain2 frmMain = FormMain2.Instance;

            if (!frmMain.IsPCMode)
            {
                frmMain.FormFileLoad.ReloadData();
                // 태블릿 자체 파일 생성
                string strTrgPath = System.Windows.Forms.Application.StartupPath + "\\" + frmMain.IOManager.TabletDataFile;
                if (strPath != strTrgPath)
                    System.IO.File.Copy(strPath, strTrgPath, true);
            }

            // 새로운 데이터 파일을 열었으므로 현재 열려있는 창은 닫는다.
            //frmMain.DXFControl.CloseDXF();
            frmMain.CurrentZone = null;
        }

        private void OnNewDXFOpened()
        {
            FormMain2 frmMain = FormMain2.Instance;
            FormEquipList leftBar = frmMain.ViewControl.LeftBar;

            DockingEquipHistory history = frmMain.ViewControl.FrmEquipHistory;

            if (frmMain.CurrentZone != null)
                frmMain.ViewControl.SetLabelText(frmMain.CurrentZone.ZoneName);
            //frmMain.IOManager.LoadZoneEquipments(frmMain.CurrentEquipments, frmMain.CurrentZone);
            frmMain.DXFManager.LoadZoneEquipments(frmMain.CurrentZone);
            leftBar.SetEquipments(frmMain.CurrentEquipments);
            history.SetEquipments(frmMain.CurrentEquipments);

            frmMain.SetEquipmentLayerOnOff();

            frmMain.ViewControl.SetGroupOption();
        }

        private void PostEquipSelected(FireEquipment equip)
        {
            if (equip == null)
                return;

            FormMain2 frmMain = FormMain2.Instance;

            if (frmMain.EquipmentChecker.IsWorking)
                frmMain.EquipmentChecker.SetEquipment(equip);
            else if (frmMain.EquipmentHistoryViewer == null || frmMain.EquipmentHistoryViewer.IsDisposed)
            {
                //시설 점검 이력이 아직 열리지 않았을 경우..?
                return;
            }
            else
            {
                if (frmMain.EquipmentHistoryViewer.IsWorking)
                    frmMain.EquipmentHistoryViewer.SetEquipment(equip);
            }

        }

        public void OnDisconnectRFIDReader(bool showMessage)
        {
            FormMain2 frmMain = FormMain2.Instance;

            try
            {
                if (frmMain.CurrentZone != null || frmMain.Handle != null)
                {
                    frmMain.Invoke((MethodInvoker)delegate
                    {
                        frmMain.ViewControl.SetLabelText(frmMain.CurrentZone.ZoneName + "(RFID Reader 연결 안됨)");

                        if (showMessage)
                            MessageBox.Show("RFID Reader 장비가 연결되어 있지 않습니다.\r\n장비가 꺼져있지 않은지, 연결 상태는 올바른지 확인하여 주십시오.");
                    });
                }
            }
            catch (Exception e)
            {
            }
            
        }

        public void OnConnectRFIDReader()
        {
            FormMain2 frmMain = FormMain2.Instance;

            if (frmMain.CurrentZone != null)
            {
                frmMain.Invoke((MethodInvoker)delegate
                {
                    frmMain.ViewControl.SetLabelText(frmMain.CurrentZone.ZoneName);
                });
            }
        }
    }
}
