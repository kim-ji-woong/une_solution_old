using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace HSMS
{
    public partial class FormDetect : Form
    {
        public enum EditMode { EDIT_ON_WORKERS = 0, EDIT_ITEMS };

        private EditMode m_editMode = EditMode.EDIT_ON_WORKERS;
        private Dictionary<Object, bool> m_dicSensorDetect = new Dictionary<object, bool>();
        private Dictionary<DataWorker, Dictionary<Object, bool>> m_dicWorkerSensorDetect = new Dictionary<DataWorker, Dictionary<object, bool>>();

        // 센서 신호를 무시할 객체들만 ArrayList에 담겨있다.
        private Dictionary<DataWorker, ArrayList> m_dicOriginalWorkerIgnores = new Dictionary<DataWorker, ArrayList>();

        private DataWorker CurrentWorker
        {
            get
            {
                if (cmbWorkers.SelectedIndex == -1)
                    return null;

                return (DataWorker)cmbWorkers.Items[cmbWorkers.SelectedIndex];
            }
        }

        private DataCar CurrentCar
        {
            get
            {
                if (cmbVehicles.SelectedIndex == -1)
                    return null;

                return (DataCar)cmbVehicles.Items[cmbVehicles.SelectedIndex];
            }
        }

        private DataEquip CurrentEquip
        {
            get
            {
                if (cmbEquips.SelectedIndex == -1)
                    return null;

                return (DataEquip)cmbEquips.Items[cmbEquips.SelectedIndex];
            }
        }
                
        public FormDetect()
        {
            InitializeComponent();
        }
        
        private void LoadIgnoreDB()
        {
            DataManager dataMgr = FormMain.Instance.DataMgr;

            int nCars = dataMgr.GetCarCount();
            for (int i = 0; i < nCars; i++)
            {
                DataCar car = dataMgr.GetCar(i);
                cmbVehicles.Items.Add(car);
            }

            int nEquip = dataMgr.GetEquipCount();
            for (int i = 0; i < nEquip; i++)
            {
                DataEquip equip = dataMgr.GetEquip(i);
                if(equip.Sensor == null || equip.Sensor == "")
                    continue;
                
                cmbEquips.Items.Add(equip);
            }

            int nWorkers = dataMgr.GetWorkerCount();
            for (int i = 0; i < nWorkers; i++)
            {
                DataWorker worker = dataMgr.GetWorker(i);
                cmbWorkers.Items.Add(worker);
            }

            ArrayList arrIgnores = dataMgr.GetSensorIgnoreDatas();
            if (arrIgnores != null)
            {
                foreach (DetectIgnoreWorker data in arrIgnores)
                {
                    if (data.SiteID != FormMain.Instance.SiteID)
                        continue;

                    ArrayList arrWorkerIgnores = null;

                    if (m_dicOriginalWorkerIgnores.ContainsKey(data.Worker))
                        arrWorkerIgnores = m_dicOriginalWorkerIgnores[data.Worker];
                    else
                    {
                        arrWorkerIgnores = new ArrayList();
                        m_dicOriginalWorkerIgnores[data.Worker] = arrWorkerIgnores;
                    }

                    if (data.IgnoreObjectType == 1)
                    {
                        DataCar car = dataMgr.GetCarFromID(data.IgnoreObjectID);

                        if (car != null)
                            arrWorkerIgnores.Add(car);
                    }
                    else if (data.IgnoreObjectType == 2)
                    {
                        DataEquip equip = dataMgr.GetEquipFromID(data.IgnoreObjectID);

                        if (equip != null)
                            arrWorkerIgnores.Add(equip);
                    }
                }
            }
        }
        
        private void btnForWorkers_Click(object sender, EventArgs e)
        {
            if (m_editMode == EditMode.EDIT_ON_WORKERS)
                return;

            btnForWorkers.BackColor = Color.FromKnownColor(KnownColor.Control);
            btnForSensors.BackColor = Color.White;

            panelSpace1.Visible = true;
            panelSpace2.Visible = false;

            cmbUseWorker.Visible = false;
            m_editMode = EditMode.EDIT_ON_WORKERS;

            cmbWorkers.SelectedIndex = -1;
            cmbVehicles.SelectedIndex = -1;
            cmbEquips.SelectedIndex = -1;
        }

        private void btnForSensors_Click(object sender, EventArgs e)
        {
            if (m_editMode == EditMode.EDIT_ITEMS)
                return;

            btnForSensors.BackColor = Color.FromKnownColor(KnownColor.Control);
            btnForWorkers.BackColor = Color.White;

            panelSpace1.Visible = false;
            panelSpace2.Visible = true;

            cmbUseWorker.Visible = true;
            m_editMode = EditMode.EDIT_ITEMS;

            cmbWorkers.SelectedIndex = -1;
            cmbVehicles.SelectedIndex = -1;
            cmbEquips.SelectedIndex = -1;
        }

        private void FormDetect_Load(object sender, EventArgs e)
        {
            LoadIgnoreDB();

            // 작업자별 설정이 기본값
            btnForWorkers.BackColor = Color.FromKnownColor(KnownColor.Control);
            btnForSensors.BackColor = Color.White;

            panelSpace1.Visible = true;
            panelSpace2.Visible = false;

            cmbUseWorker.Visible = false;
            m_editMode = EditMode.EDIT_ON_WORKERS;

            // 센서 사용함이 기본값
            cmbUseWorker.SelectedIndex = 0;
            cmbUseVehicle.SelectedIndex = 0;
            cmbUseEquip.SelectedIndex = 0;
        }

        private void cmbUseWorker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_editMode == EditMode.EDIT_ON_WORKERS)
                return;

            if (cmbWorkers.SelectedIndex == -1)
            {
                return;
            }

            DataWorker currentWorker = CurrentWorker;

            if (currentWorker == null)
                return;

            int nSelected = cmbUseWorker.SelectedIndex;

            if (m_editMode == EditMode.EDIT_ITEMS)
            {
                if ((nSelected == 0 && currentWorker.SensorDetect == true) ||
                    (nSelected == 1 && currentWorker.SensorDetect == false))
                {
                    m_dicSensorDetect.Remove(currentWorker);
                    return;
                }

                m_dicSensorDetect[currentWorker] = nSelected == 0;
            }
        }

        private void cmbUseVehicle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbVehicles.SelectedIndex == -1)
            {
                return;
            }

            DataCar currentCar = CurrentCar;

            if (currentCar == null)
                return;

            int nSelected = cmbUseVehicle.SelectedIndex;

            if (m_editMode == EditMode.EDIT_ITEMS)
            {
                if ((nSelected == 0 && currentCar.SensorDetect == true) ||
                    (nSelected == 1 && currentCar.SensorDetect == false))
                {
                    m_dicSensorDetect.Remove(currentCar);
                    return;
                }

                m_dicSensorDetect[currentCar] = nSelected == 0;
            }
            else
            {
                DataWorker currentWorker = CurrentWorker;

                if (currentWorker == null)
                    return;

                DetectIgnoreWorker ignore = FormMain.Instance.DataMgr.FindIgnoreWorker(currentWorker.ID, currentCar.ID, 1, currentWorker.SiteID);
                Dictionary<Object, bool> dicSensorDetect = null;

                if (m_dicWorkerSensorDetect.ContainsKey(currentWorker))
                    dicSensorDetect = m_dicWorkerSensorDetect[currentWorker];
                else
                {
                    dicSensorDetect = new Dictionary<object, bool>();
                    m_dicWorkerSensorDetect[currentWorker] = dicSensorDetect;
                }
                
                bool detectSensor = nSelected == 0;

                if ((ignore == null && !detectSensor) ||
                    (ignore != null && detectSensor))
                    dicSensorDetect[currentCar] = detectSensor;
            }
        }        

        private void cmbUseEquip_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbEquips.SelectedIndex == -1)
            {
                return;
            }

            DataEquip currentEquip = CurrentEquip;

            if (currentEquip == null)
                return;

            int nSelected = cmbUseEquip.SelectedIndex;

            if (m_editMode == EditMode.EDIT_ITEMS)
            {
                if ((nSelected == 0 && currentEquip.SensorDetect == true) ||
                    (nSelected == 1 && currentEquip.SensorDetect == false))
                {
                    m_dicSensorDetect.Remove(currentEquip);
                    return;
                }

                m_dicSensorDetect[currentEquip] = nSelected == 0;
            }
            else
            {
                DataWorker currentWorker = CurrentWorker;

                if (currentWorker == null)
                    return;

                DetectIgnoreWorker ignore = FormMain.Instance.DataMgr.FindIgnoreWorker(currentWorker.ID, currentEquip.ID, 2, currentWorker.SiteID);
                Dictionary<Object, bool> dicSensorDetect = null;

                if (m_dicWorkerSensorDetect.ContainsKey(currentWorker))
                    dicSensorDetect = m_dicWorkerSensorDetect[currentWorker];
                else
                {
                    dicSensorDetect = new Dictionary<object, bool>();
                    m_dicWorkerSensorDetect[currentWorker] = dicSensorDetect;
                }

                bool detectSensor = nSelected == 0;

                if ((ignore == null && !detectSensor) ||
                    (ignore != null && detectSensor))
                    dicSensorDetect[currentEquip] = detectSensor;
            }
        }
        
        private void cmbWorkers_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmbWorker = (ComboBox)sender;

            if (cmbWorker.SelectedIndex < 0)
                return;

            DataWorker worker = (DataWorker)cmbWorker.Items[cmbWorker.SelectedIndex];

            if (m_editMode == EditMode.EDIT_ITEMS)
            {
                if (m_dicSensorDetect.ContainsKey(worker))
                {
                    bool sensorDetect = m_dicSensorDetect[worker];
                    cmbUseWorker.SelectedIndex = sensorDetect ? 0 : 1;
                }
                else
                {
                    if (worker.SensorDetect)
                    {
                        cmbUseWorker.SelectedIndex = 0;
                    }
                    else
                    {
                        cmbUseWorker.SelectedIndex = 1;
                    }
                }
            }
            else
            {
                cmbEquips.SelectedIndex = -1;
                cmbVehicles.SelectedIndex = -1;

                cmbUseEquip.SelectedIndex = 0;
                cmbUseVehicle.SelectedIndex = 0;
            }
        }
        
        private void cmbVehicles_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataCar car = CurrentCar;

            if (car == null)
                return;

            if (m_editMode == EditMode.EDIT_ITEMS)
            {
                if (m_dicSensorDetect.ContainsKey(car))
                {
                    bool sensorDetect = m_dicSensorDetect[car];
                    cmbUseVehicle.SelectedIndex = sensorDetect ? 0 : 1;
                }
                else
                {
                    if (car.SensorDetect)
                    {
                        cmbUseVehicle.SelectedIndex = 0;
                    }
                    else
                    {
                        cmbUseVehicle.SelectedIndex = 1;
                    }
                }
            }
            else
            {
                cmbUseVehicle.SelectedIndex = FindIgnoreSensorOnWorker(CurrentWorker, car) ? 1 : 0;
            }
        }

        // 작업자별 무시할 센서 여부 확인
        // Return 값 : true이면 센서 사용안함
        //             false이면 센서 사용
        private bool FindIgnoreSensorOnWorker(DataWorker worker, Object obj)
        {
            if (worker == null)
                return false;

            if (m_dicWorkerSensorDetect.ContainsKey(worker))
            {
                Dictionary<Object, bool> dicSensorDetect = m_dicWorkerSensorDetect[worker];

                if (dicSensorDetect.ContainsKey(obj))
                {
                    bool sensorDetect = dicSensorDetect[obj];
                    return !sensorDetect;
                }
            }

            if (m_dicOriginalWorkerIgnores.ContainsKey(worker))
            {
                ArrayList arrIgnores = m_dicOriginalWorkerIgnores[worker];

                if (arrIgnores.Contains(obj))
                    return true;
            }

            return false;
        }

        //1(Car), 2(Equipment), 3(Zone)
        private void cmbEquips_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataEquip equip = CurrentEquip;

            if (equip == null)
                return;

            if (m_editMode == EditMode.EDIT_ITEMS)
            {
                if (m_dicSensorDetect.ContainsKey(equip))
                {
                    bool sensorDetect = m_dicSensorDetect[equip];
                    cmbUseEquip.SelectedIndex = sensorDetect ? 0 : 1;
                }
                else
                {
                    if (equip.SensorDetect)
                    {
                        cmbUseEquip.SelectedIndex = 0;
                    }
                    else
                    {
                        cmbUseEquip.SelectedIndex = 1;
                    }
                }
            }
            else
            {
                cmbUseEquip.SelectedIndex = FindIgnoreSensorOnWorker(CurrentWorker, equip) ? 1 : 0;
            }
        }

        private void FormDetect_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            SaveChangedData();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SaveChangedData();
        }

        private void SaveChangedData()
        {
            bool isChanged = false;
            EditIgnoreDetect editData = new EditIgnoreDetect();

            foreach (KeyValuePair<object, bool> pair in m_dicSensorDetect)
            {
                isChanged = true;
                editData.AddUpdateData(pair.Key);
            }

            Type typeVehicle = typeof(DataCar);
            Type typeEquip = typeof(DataEquip);
            Type typeZone = typeof(DataZone);

            foreach (KeyValuePair<DataWorker, Dictionary<object, bool>> pair in m_dicWorkerSensorDetect)
            {
                foreach (KeyValuePair<object, bool> objPair in pair.Value)
                {
                    DetectIgnoreWorker ignore = new DetectIgnoreWorker();

                    ignore.WorkerID = pair.Key.ID;
                    ignore.SiteID = pair.Key.SiteID;
                    ignore.Worker = pair.Key;

                    object obj = objPair.Key;

                    if (obj.GetType() == typeVehicle)
                    {
                        DataCar car = (DataCar)objPair.Key;
                        ignore.IgnoreObjectID = car.ID;
                        ignore.IgnoreObjectType = 1;
                    }
                    else if (obj.GetType() == typeEquip)
                    {
                        DataEquip equip = (DataEquip)objPair.Key;
                        ignore.IgnoreObjectID = equip.ID;
                        ignore.IgnoreObjectType = 2;
                    }
                    else if (obj.GetType() == typeZone)
                    {
                        DataZone zone = (DataZone)objPair.Key;
                        ignore.IgnoreObjectID = zone.ID;
                        ignore.IgnoreObjectType = 3;
                    }
                    else
                        continue;

                    isChanged = true;

                    if (objPair.Value)
                        editData.AddDeleteIgnore(ignore);
                    else
                        editData.AddIgnore(ignore);
                }
            }

            if (isChanged)
            {
                ArrayList arrIgnoreSensorsToWorker = new ArrayList();
                ArrayList arrWorker = new ArrayList();
                ArrayList arrCar = new ArrayList();
                ArrayList arrEquip = new ArrayList();

                editData.Datas = arrIgnoreSensorsToWorker;
                editData.WorkerDatas = arrWorker;
                editData.CarDatas = arrCar;
                editData.EquipDatas = arrEquip;

                if (editData.Update(null))
                {
                    NetworkManager netMgr = FormMain.Instance.NetMgr;

                    netMgr.SendDBDataList(ChangeDataType.IGNORE_SENSORS_TO_WORKER, arrIgnoreSensorsToWorker);
                    netMgr.SendDBDataList(ChangeDataType.WORKER, arrWorker);
                    netMgr.SendDBDataList(ChangeDataType.CAR, arrCar);
                    netMgr.SendDBDataList(ChangeDataType.EQUIP, arrEquip);
                }
            }
        }
    }
    //public partial class FormDetect : Form
    //{
    //    private int m_nEditMode = 0;

    //    private bool m_bSaveDB = false;

    //    private ArrayList m_arUsingObject = new ArrayList();
    //    private ArrayList m_arUsingIgnoreData = null;

    //    private DataCar mCurrentCar = null;

    //    private DataWorker mCurrentWorker = null;

    //    private DataEquip mCurrentEquip = null;

    //    private MultiMap<int, DetectIgnoreWorker> m_dicIgnores = new MultiMap<int, DetectIgnoreWorker>();

    //    private MultiMap<int, DetectIgnoreWorker> m_dicOrgData = null;
    //    public FormDetect()
    //    {
    //        InitializeComponent();
    //    }
        
    //    private void LoadIgnoreDB()
    //    {
    //        DataManager dataMgr = FormMain.Instance.DataMgr;
    //        int nCars = dataMgr.GetCarCount();
    //        for (int i = 0; i < nCars; i++)
    //        {
    //            cmbVehicles.Items.Add(dataMgr.GetCar(i));
    //            m_arUsingObject.Add(dataMgr.GetCar(i));
    //        }

    //        int nEquip = dataMgr.GetEquipCount();
    //        for (int i = 0; i < nEquip; i++)
    //        {
    //            DataEquip equip = dataMgr.GetEquip(i);
    //            if(equip.Sensor == null || equip.Sensor == "")
    //                continue;
    //            cmbEquips.Items.Add(equip);
    //            m_arUsingObject.Add(equip);
    //        }

    //        int nWorkers = dataMgr.GetWorkerCount();
    //        for (int i = 0; i < nWorkers; i++)
    //        {
    //            cmbWorkers.Items.Add(dataMgr.GetWorker(i));
    //            m_arUsingObject.Add(dataMgr.GetWorker(i));
    //        }

    //        m_arUsingIgnoreData = dataMgr.GetSensorIgnoreDatas();
    //        if (m_arUsingIgnoreData != null)
    //        {
    //            foreach (DetectIgnoreWorker data in m_arUsingIgnoreData)
    //            {
    //                m_dicIgnores.Add(data.WorkerID, data);
    //            }
    //        }
    //        m_dicOrgData = m_dicIgnores.Clone();
    //    }
        
    //    private void btnForWorkers_Click(object sender, EventArgs e)
    //    {
    //        btnForWorkers.BackColor = Color.FromKnownColor(KnownColor.Control);
    //        btnForSensors.BackColor = Color.White;

    //        panelSpace1.Visible = true;
    //        panelSpace2.Visible = false;

    //        cmbUseWorker.Visible = false;
    //        m_nEditMode = 0;

    //        cmbWorkers.SelectedIndex = -1;
    //        cmbVehicles.SelectedIndex = -1;
    //        cmbEquips.SelectedIndex = -1;
    //    }

    //    private void btnForSensors_Click(object sender, EventArgs e)
    //    {
    //        btnForSensors.BackColor = Color.FromKnownColor(KnownColor.Control);
    //        btnForWorkers.BackColor = Color.White;

    //        panelSpace1.Visible = false;
    //        panelSpace2.Visible = true;

    //        cmbUseWorker.Visible = true;
    //        m_nEditMode = 1;

    //        cmbWorkers.SelectedIndex = -1;
    //        cmbVehicles.SelectedIndex = -1;
    //        cmbEquips.SelectedIndex = -1;
    //    }

    //    private void FormDetect_Load(object sender, EventArgs e)
    //    {
    //        LoadIgnoreDB();

    //        // 작업자별 설정이 기본값
    //        btnForWorkers.BackColor = Color.FromKnownColor(KnownColor.Control);
    //        btnForSensors.BackColor = Color.White;

    //        panelSpace1.Visible = true;
    //        panelSpace2.Visible = false;

    //        cmbUseWorker.Visible = false;
    //        m_nEditMode = 0;

    //        // 센서 사용함이 기본값
    //        cmbUseWorker.SelectedIndex = 0;
    //        cmbUseVehicle.SelectedIndex = 0;
    //        cmbUseEquip.SelectedIndex = 0;
    //    }

    //    private void cmbUseWorker_SelectedIndexChanged(object sender, EventArgs e)
    //    {
    //        if (m_nEditMode == 0)
    //            return;

    //        if (cmbWorkers.SelectedIndex == -1)
    //        {
    //            return;
    //        }

    //        if (mCurrentWorker == null)
    //            return;

    //        int nSelected = cmbUseWorker.SelectedIndex;

    //        if (m_nEditMode == 1)
    //        {
    //            if (nSelected == 0 && mCurrentWorker.SensorDetect == true)
    //                return;

    //            if (nSelected == 1 && mCurrentWorker.SensorDetect == false)
    //                return;

    //            // add to  chanage
    //            mCurrentWorker.SensorDetect = nSelected == 0 ? true : false;
    //        }

    //    }

    //    private void cmbUseVehicle_SelectedIndexChanged(object sender, EventArgs e)
    //    {
    //        if (cmbVehicles.SelectedIndex == -1)
    //        {
    //            return;
    //        }          
    //        if (mCurrentCar == null)
    //            return;

    //        int nSelected = cmbUseVehicle.SelectedIndex;

    //        if (m_nEditMode == 1)
    //        {
    //            if (nSelected == 0 && mCurrentCar.SensorDetect == true)
    //                return;

    //            if (nSelected == 1 && mCurrentCar.SensorDetect == false)
    //                return;

    //            // add to  chanage
    //            mCurrentCar.SensorDetect = nSelected == 0 ? true : false;
    //        }
    //        else
    //        {
    //            bool bFindData = false;
    //            if (mCurrentIgonreList != null)
    //            {
    //                foreach (DetectIgnoreWorker data in mCurrentIgonreList)
    //                {
    //                    if (data.IgnoreObjectType == 1 && data.IgnoreObjectID == mCurrentCar.ID)
    //                    {
    //                        bFindData = true;
    //                        break;
    //                    }
    //                }
    //            }
    //            if (bFindData == false)
    //            {
    //                if (nSelected == 0)
    //                    return;
    //                else
    //                {
    //                    // ADD Ignore Data
    //                    int nWorkerID = mCurrentWorker.ID;
    //                    DetectIgnoreWorker data = new DetectIgnoreWorker();
    //                    data.Worker = mCurrentWorker;
    //                    data.WorkerID = nWorkerID;
    //                    data.IgnoreObjectID = mCurrentCar.ID;
    //                    data.IgnoreObjectType = 1;
    //                    data.SiteID = FormMain.Instance.SiteID;

    //                    m_dicIgnores.Add(nWorkerID, data);                        
    //                }
    //            }
    //            else
    //            {
    //                if (nSelected == 1)
    //                {
    //                    return;
    //                }
    //                else
    //                {
    //                    // Remove Ignore Data
    //                    ArrayList arDelete = new ArrayList();                        
    //                    foreach (DetectIgnoreWorker data in mCurrentIgonreList)
    //                    {
    //                        if( data.IgnoreObjectType == 1 && data.IgnoreObjectID == mCurrentCar.ID)
    //                        {
    //                            arDelete.Add(data);
    //                        }
    //                    }                    
    //                    foreach (DetectIgnoreWorker data in arDelete)
    //                    {
    //                        mCurrentIgonreList.Remove(data);
    //                    }                        
    //                }

    //            }
    //        }
    //    }        

    //    private void cmbUseEquip_SelectedIndexChanged(object sender, EventArgs e)
    //    {
    //        if (cmbEquips.SelectedIndex == -1)
    //        {
    //            return;
    //        }

    //        if (mCurrentEquip == null)
    //            return;

    //        int nSelected = cmbUseEquip.SelectedIndex;

    //        if (m_nEditMode == 1)
    //        {
    //            if (nSelected == 0 && mCurrentEquip.SensorDetect == true)
    //                return;

    //            if (nSelected == 1 && mCurrentEquip.SensorDetect == false)
    //                return;

    //            // add to  chanage
    //            mCurrentEquip.SensorDetect = nSelected == 0 ? true : false;
    //        }
    //        else
    //        {
    //            if (mCurrentWorker == null)
    //                return;

    //            bool bFindData = false;
    //            if (mCurrentIgonreList != null)
    //            {
    //                foreach (DetectIgnoreWorker data in mCurrentIgonreList)
    //                {
    //                    if (data.IgnoreObjectType == 2 && data.IgnoreObjectID == mCurrentEquip.ID)
    //                    {
                            
    //                        bFindData = true;
    //                        break;
    //                    }
    //                }
    //            }
    //            if (bFindData == false)
    //            {
    //                if (nSelected == 0)
    //                    return;
    //                else
    //                {
    //                    // ADD Ignore Data
    //                    int nWorkerID = mCurrentWorker.ID;
    //                    DetectIgnoreWorker data = new DetectIgnoreWorker();
    //                    data.Worker = mCurrentWorker;
    //                    data.WorkerID = nWorkerID;
    //                    data.IgnoreObjectID = mCurrentEquip.ID;
    //                    data.IgnoreObjectType = 2;
    //                    data.SiteID = FormMain.Instance.SiteID;

    //                    m_dicIgnores.Add(nWorkerID, data);
    //                }
    //            }
    //            else
    //            {
    //                if (nSelected == 1)
    //                {
    //                    return;
    //                }
    //                else
    //                {
    //                    // Remove Ignore Data
    //                    ArrayList arDelete = new ArrayList();
    //                    foreach (DetectIgnoreWorker data in mCurrentIgonreList)
    //                    {
    //                        if (data.IgnoreObjectType == 2 && data.IgnoreObjectID == mCurrentEquip.ID)
    //                        {
    //                            arDelete.Add(data);
    //                        }
    //                    }
    //                    foreach (DetectIgnoreWorker data in arDelete)
    //                    {
    //                        mCurrentIgonreList.Remove(data);
    //                    }
    //                }
    //            }
    //        }
    //    }

        
    //    private void cmbWorkers_SelectedIndexChanged(object sender, EventArgs e)
    //    {
    //        ComboBox cmb = (ComboBox)sender;
    //        if (cmb.SelectedItem == null)
    //        {
    //            mCurrentWorker = null;
    //            return;
    //        }

    //        DataWorker worker = (DataWorker)cmb.SelectedItem;
    //        mCurrentWorker = worker;

    //        if (m_nEditMode == 1)
    //        {                
    //            if (worker.SensorDetect)
    //            {
    //                cmbUseWorker.SelectedIndex = 0;
    //            }
    //            else
    //            {
    //                cmbUseWorker.SelectedIndex = 1;
    //            }
    //        }
    //        else
    //        {
    //            cmbEquips.SelectedIndex = -1;
    //            cmbVehicles.SelectedIndex = -1;

    //            cmbUseEquip.SelectedIndex = 0;
    //            cmbUseVehicle.SelectedIndex = 0;

    //            List<DetectIgnoreWorker> list = m_dicIgnores[worker.ID];
    //            if (list == null || list.Count == 0)
    //                return;

    //            mCurrentIgonreList = list;
    //        }
    //    }

    //    private List<DetectIgnoreWorker> mCurrentIgonreList = null;

        
    //    private void cmbVehicles_SelectedIndexChanged(object sender, EventArgs e)
    //    {
    //        ComboBox cmb = (ComboBox)sender;
    //        if (cmb.SelectedItem == null)
    //        {
    //            mCurrentCar = null;
    //            return;
    //        }
            
    //        DataCar car = (DataCar)cmb.SelectedItem;
    //        mCurrentCar = car;
    //        if (m_nEditMode == 1)
    //        {                
    //            if (car.SensorDetect)
    //            {
    //                cmbUseVehicle.SelectedIndex = 0;
    //            }
    //            else
    //            {
    //                cmbUseVehicle.SelectedIndex = 1;
    //            }
    //        }
    //        else
    //        {
    //            cmbUseVehicle.SelectedIndex = FindIgnoreSensorOnWorker(1, mCurrentCar.ID) ? 1 : 0;
    //            /*bool bFindData = false;
    //            if (mCurrentIgonreList != null)
    //            {
                    
    //                foreach (DetectIgnoreWorker data in mCurrentIgonreList)
    //                {
    //                    if (data.IgnoreObjectType == 1 && data.IgnoreObjectID == mCurrentCar.ID)
    //                    {
    //                        cmbUseVehicle.SelectedIndex = 1;
    //                        bFindData = true;
    //                        break;
    //                    }
    //                }
    //            }
    //            if (bFindData == false)
    //                cmbUseVehicle.SelectedIndex = 0;*/
                
    //        }
    //    }

    //    // 작업자별 무시할 센서 여부 확인
    //    // Return 값 : true이면 센서 사용안함
    //    //             false이면 센서 사용
    //    private bool FindIgnoreSensorOnWorker(int nObjectType, int nObjectID)
    //    {
    //        if (cmbWorkers.SelectedIndex < 0)
    //            return false;

    //        DataWorker worker = (DataWorker)cmbWorkers.Items[cmbWorkers.SelectedIndex];

    //        List<DetectIgnoreWorker> list = m_dicIgnores[worker.ID];

    //        foreach (DetectIgnoreWorker ignore in list)
    //        {
    //            if (ignore.IgnoreObjectType == nObjectType && ignore.IgnoreObjectID == nObjectID)
    //            {
    //                return true;
    //            }
    //        }

    //        if (mCurrentIgonreList != null)
    //        {
    //            foreach (DetectIgnoreWorker data in mCurrentIgonreList)
    //            {
    //                if (data.IgnoreObjectType == nObjectType && data.IgnoreObjectID == nObjectID)
    //                {
    //                    return true;
    //                }
    //            }
    //        }

    //        return false;
    //    }

    //    //1(Car), 2(Equipment), 3(Zone)
    //    private void cmbEquips_SelectedIndexChanged(object sender, EventArgs e)
    //    {
    //        ComboBox cmb = (ComboBox)sender;
    //        if (cmb.SelectedItem == null)
    //        {
    //            mCurrentEquip = null;
    //            return;
    //        }
            
    //        DataEquip equip = (DataEquip)cmb.SelectedItem;
    //        mCurrentEquip = equip;
    //        if (m_nEditMode == 1)            {
              
    //            if (equip.SensorDetect)
    //            {
    //                cmbUseEquip.SelectedIndex = 0;
    //            }
    //            else
    //            {
    //                cmbUseEquip.SelectedIndex = 1;
    //            }
    //        }
    //        else
    //        {
    //            cmbUseEquip.SelectedIndex = FindIgnoreSensorOnWorker(2, mCurrentEquip.ID) ? 1 : 0;
    //            /*bool bFindData = false;
    //            if (mCurrentIgonreList != null)
    //            {
                   
    //                foreach (DetectIgnoreWorker data in mCurrentIgonreList)
    //                {
    //                    if (data.IgnoreObjectType == 2 && data.IgnoreObjectID == mCurrentEquip.ID)
    //                    {
    //                        cmbUseEquip.SelectedIndex = 1;
    //                        bFindData = true;
    //                        break;
    //                    }
    //                }
    //            }
    //            if (bFindData == false)
    //                cmbUseEquip.SelectedIndex = 0;*/
    //        }
    //    }

    //    private void FormDetect_FormClosing(object sender, FormClosingEventArgs e)
    //    {
    //        if (m_bSaveDB == true)
    //        {
                
    //        }
    //        else
    //        {
    //            foreach (ISensorDetectIgnoreChnaged obj in m_arUsingObject)
    //            {
    //                if (obj.DBSensorDetect != obj.SensorDetect)
    //                {
    //                    obj.SensorDetect = obj.DBSensorDetect;
    //                }
    //            }
    //        }
    //    }

    //    private bool m_bUpdateData = false;
    //    private bool m_bUpdageIgnoreData = false;
    //    private ArrayList m_arUpdateDetect = new ArrayList();
    //    private bool IsChnagedData()
    //    {
    //        m_arUpdateDetect.Clear();
    //        foreach (ISensorDetectIgnoreChnaged obj in m_arUsingObject)
    //        {
    //            if (obj.DBSensorDetect != obj.SensorDetect)
    //            {
    //                m_bUpdateData = true;
    //                m_arUpdateDetect.Add(obj);
    //            }
    //        }
    //        if (m_arUpdateDetect.Count == 0)
    //            m_bUpdateData = false;

    //        if (!m_dicIgnores.Compare(m_dicOrgData))
    //        {
    //            m_bUpdageIgnoreData = true;
    //            return true;
    //        }
    //        else
    //        {
    //            m_bUpdageIgnoreData = false;
    //        }
    //        return false;
    //    }
        

    //    private void btnCancel_Click(object sender, EventArgs e)
    //    {
    //        m_bSaveDB = true;
    //        this.DialogResult = DialogResult.Cancel;
    //        this.Close();
    //    }
        
    //    private void button1_Click(object sender, EventArgs e)
    //    {
    //        m_bSaveDB = true;
            
    //        if (IsChnagedData() == true)
    //            SaveChangedData();

    //        this.DialogResult = DialogResult.OK;
    //        this.Close();
    //    }

    //    private void btnOK_Click(object sender, EventArgs e)
    //    {
    //        m_bSaveDB = false;
            
    //        if( IsChnagedData() == true)
    //            SaveChangedData();
    //    }

    //    private void SaveChangedData()
    //    {          
    //        ArrayList arDiffData1 = new ArrayList();
    //        ArrayList arDiffData2 = new ArrayList();
    //        ArrayList arDiffData3 = new ArrayList();
    //        ArrayList arDiffData4 = new ArrayList();
            
    //        IEnumerable<int> keyList2 = m_dicIgnores.Keys;
    //        foreach (int key in keyList2)
    //        {
    //            List<DetectIgnoreWorker> list = m_dicIgnores[key];
                
    //            for (int i = 0; i < list.Count; i++)
    //            {
    //                arDiffData1.Add(list[i]);
    //            }
    //        }

    //        IEnumerable<int> keyList = m_dicOrgData.Keys;
    //        foreach (int key in keyList)
    //        {
    //            List<DetectIgnoreWorker> list = m_dicOrgData[key];
    //            for (int i = 0; i < list.Count; i++)
    //            {
    //                arDiffData2.Add(list[i]);
    //            }
    //        }

    //        foreach (DetectIgnoreWorker data in arDiffData2)
    //        {
    //            bool bFindData = false;
    //            foreach (DetectIgnoreWorker data2 in arDiffData1)
    //            {
    //                if (data.ToString() == data2.ToString())
    //                {
    //                    bFindData = true;
    //                    //arDiffData3.Add(data);
    //                    break;
    //                }
    //            }
    //            if (bFindData == false)
    //            {
    //                arDiffData4.Add(data);
    //            }
    //        }

    //        foreach (DetectIgnoreWorker data in arDiffData1)
    //        {
    //            bool bFindData = false;
    //            foreach (DetectIgnoreWorker data2 in arDiffData2)
    //            {
    //                if (data.ToString() == data2.ToString())
    //                {
    //                    bFindData = true;
    //                    //arDiffData3.Add(data);
    //                    break;
    //                }
    //            }
    //            if (bFindData == false)
    //            {
    //                arDiffData3.Add(data);
    //            }
    //        }

    //        if (arDiffData4.Count > 0 || arDiffData3.Count > 0 || m_arUpdateDetect.Count > 0)
    //        {
    //            EditIgnoreDetect editData = new EditIgnoreDetect();

    //            foreach (DetectIgnoreWorker data in arDiffData4)
    //            {
    //                editData.AddDeleteIgnore(data);
    //            }

    //            foreach (DetectIgnoreWorker data in arDiffData3)
    //            {
    //                editData.AddIgnore(data);
    //            }

    //            foreach (object data in m_arUpdateDetect)
    //            {
    //                editData.AddUpdateData(data);
    //            }

    //            ArrayList arrIgnoreSensorsToWorker = new ArrayList();
    //            ArrayList arrWorker = new ArrayList();
    //            ArrayList arrCar = new ArrayList();
    //            ArrayList arrEquip = new ArrayList();

    //            editData.Datas = arrIgnoreSensorsToWorker;
    //            editData.WorkerDatas = arrWorker;
    //            editData.CarDatas = arrCar;
    //            editData.EquipDatas = arrEquip;

    //            if (editData.Update(null))
    //            {
    //                SendDBDataList(ChangeDataType.IGNORE_SENSORS_TO_WORKER, arrIgnoreSensorsToWorker);
    //                SendDBDataList(ChangeDataType.WORKER, arrWorker);
    //                SendDBDataList(ChangeDataType.CAR, arrCar);
    //                SendDBDataList(ChangeDataType.EQUIP, arrEquip);
    //            }
    //        }
    //    }

    //    private void SendDBDataList(ChangeDataType type, ArrayList arrDatas)
    //    {
    //        if (arrDatas.Count == 0)
    //            return;

    //        arrDatas.Insert(0, (int)type);
    //        byte[] bytes = ClientProvider.MakeBytes(TCP_ID.CHANGE_DB_DATA_LIST, arrDatas);

    //        NetworkManager netMgr = FormMain.Instance.NetMgr;
    //        netMgr.Send(bytes, netMgr.ClientProvider);
    //    }
    //}
}
