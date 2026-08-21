using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Collections;

namespace SDMS
{
    public class SeletCaseData
    {
        public SeletCaseData(SDMS.SensorZone sensor, int nSensorHistoryID)
		{			
			m_Sensor = sensor;
            SensorHistoryID = nSensorHistoryID;
		}

		public SeletCaseData(BaseViewEx view, SDMS.SensorZone sensor, int nSensorHistoryID)
		{
			m_Sensor = sensor;
            SensorHistoryID = nSensorHistoryID;
            ViewOwner = view;
		}

        private SDMS.POI m_POI = null;
        public SDMS.POI POI
        {
            get { return m_POI; }
            set { m_POI = value; }
        }

        private SensorZone m_Sensor = null;
        public SDMS.SensorZone Sensor
        {
            get { return m_Sensor; }
            set { m_Sensor = value; }
        }
        private int m_nSensorHistoryID = -1;
        public int SensorHistoryID
        {
            get { return m_nSensorHistoryID; }
            set { m_nSensorHistoryID = value; }
        }
       
        private BaseViewEx m_viewOwner = null;
        public SDMS.BaseViewEx ViewOwner
        {
            get { return m_viewOwner; }
            set { m_viewOwner = value; }
        }
    }

	public partial class DlgSelectCase : Form, IPOIPopup
	{
        private static int m_nProcessingSensorHistoryID = -1;
        public static int ProcessingSensorHistoryID
        {
            get { return m_nProcessingSensorHistoryID; }
            set { m_nProcessingSensorHistoryID = value; }
        }

        private static DlgSelectCase m_Instance = new DlgSelectCase();
        public static DlgSelectCase Instance
        {
            get 
            {

                return m_Instance; }
            set { m_Instance = value; }
        }

		public void ViewCCTV()
		{
			FormMain.Instance.PageHome.Invoke((MethodInvoker)delegate
			{
                Zone zone = mCurrentData.Sensor.POI.Zone;
				FormMain.Instance.PageHome.ShowBigCCTV(zone);				
			});
		}

        private int GetSensorHistoryID(int nSensorID)
        {
            if (ProcessManager.Instance.CurrentDetectProcess.ContainsKey(nSensorID))
            {
                ProcessIF process = ProcessManager.Instance.CurrentDetectProcess[nSensorID];
                return process.SensorHistoryID;
            }
            return -1;
        }

		public bool ReportFire()
		{
            FormMain.Instance.SendFireDetectMessageToSOPSimulator();	

            SensorZone m_Sensor = mCurrentData.Sensor;
			int nSensorID = m_Sensor.ID;
			int ZoneID = m_Sensor.EquipZoneID;

            int nHistoryID = GetSensorHistoryID(nSensorID);

			int nSOPGenUserID = FormMain.Instance.SOPGenUserID;
			return NetworkManager.Instance.SendMessage(1, TCP_ID.FIRE_DETECT_REPORT, nHistoryID, ZoneID, nSensorID, nSOPGenUserID);
		}

		public void ReportFireTranning()
		{
			//int nSensorID = m_Sensor.ID;
			//int ZoneID = m_Sensor.EquipZoneID;

			//int nHistoryID = GetSensorHistoryID(nSensorID);
			//NetworkManager.Instance.SendMessage(TCP_ID.FIRE_DETECT_TRAINNING, nHistoryID, ZoneID, nSensorID);

			//FormMain.Instance.SendFireDetectMessageToSOPSimulator();
		} 
		
		public bool ReportAbnormal()
		{
            SensorZone m_Sensor = mCurrentData.Sensor;
			int nSensorID = m_Sensor.ID;
            ProcessIF process = ProcessManager.Instance.GetProcess(nSensorID);
            if (process != null)
            {    
				int nSOPGenUserID = FormMain.Instance.SOPGenUserID;
				// 서버로 오작동 신고를 수행한다.
				return NetworkManager.Instance.SendMessage(1, TCP_ID.MALFUNCTION_REPORT, process.SensorHistoryID, nSensorID, nSOPGenUserID);
            }
            return false;
		}

		private void BtnReportFire_Click(object sender, EventArgs e)
		{
            if (MessageBox.Show("전직원에게 화재발생을 전파합니다.\r\n화재발생으로 신고하시겠습니까?", "확인", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                return;

			if (FireDetectProcess.SoundPlayer.SoundLocation != null)
				FireDetectProcess.SoundPlayer.Stop();

            if (ReportFire())
            {
                FireDetectProcess.SoundPlayer.Stop();

                this.Visible = false;

                //ViewCCTV();

                DialogResult = DialogResult.Yes;
                HideDialog();

                ConfirmDialogManager.Instance.RemoveDialog(mCurrentData, true);

                FormMain.Instance.PageHome.Invoke((MethodInvoker)delegate
                {
                    SensorZone m_Sensor = mCurrentData.Sensor;
                    FormMain.Instance.PageHome.ShowBigCCTV(m_Sensor.POI.Zone, false);

                });
            }
            else
            {
                MessageBox.Show("서버가 연결되지 않았습니다. 잠시 후에 시도하세요", "확인", MessageBoxButtons.OK);
            }
			
		}

        private void BtnSoundOnOff_Click(object sender, EventArgs e)
        {
            SensorZone m_Sensor = mCurrentData.Sensor;
            if (m_Sensor.SoundOn)
            {
                btnSound.Text = "소리 꺼짐";
                FireDetectProcess.SoundPlayer.Stop();
            }
            else
            {
                btnSound.Text = "소리 켜짐";
                FireDetectProcess.PlaySound();
            }

            m_Sensor.SoundOn = !m_Sensor.SoundOn;
        }

		private void BtnReportMalfunction_Click(object sender, EventArgs e)
		{
            if (MessageBox.Show("화재 탐지결과를 오작동으로 신고하시겠습니까?", "확인", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                return;

			if (FireDetectProcess.SoundPlayer.SoundLocation != null)
				FireDetectProcess.SoundPlayer.Stop();

            if (ReportAbnormal())
            {
                FireDetectProcess.SoundPlayer.Stop();

                DialogResult = DialogResult.Cancel;
                HideDialog();

                try
                {
                    ConfirmDialogManager.Instance.RemoveDialog(mCurrentData, false);
                }
                catch (System.Exception)
                {

                }
            }
            else
            {
                MessageBox.Show("서버가 연결되지 않았습니다. 잠시 후에 시도하세요", "확인", MessageBoxButtons.OK);
            }			
		}

		private void BtnViewCCTV_Click(object sender, EventArgs e)
		{
			ViewCCTV();
			DialogResult = DialogResult.No;
            HideDialog();

			ConfirmDialogManager.Instance.RemoveDialog(mCurrentData, true);
		}

		static private int m_nTargetSpaceX = 30;
        static private int m_nTargetSpaceY = 50;

       

        private bool m_bVisible = false;

		

        private bool m_bLayerVisible = true;
        public bool LayerVisible
        {
            get { return m_bLayerVisible; }
			set
			{				
			}               
        }

		private void SetRealMode()
		{
			bool bRealMode = true;
			if (bRealMode)
			{
				button1.Visible = false;
				//Size = new Size(373, 94);
                Size = new Size(487, 94);
			}
			else
			{
				button1.Visible = true;
				//Size = new Size(530, 94);
                Size = new Size(642, 94);
			}

		}

        private SeletCaseData mCurrentData = null;
        public SeletCaseData CurrentData
        {
            get 
            {
                return mCurrentData; 
            }
            set 
            {
                mCurrentData = value; 

                if(mCurrentData.ViewOwner == null)
                    InitData(mCurrentData.Sensor, mCurrentData.SensorHistoryID);
                else
		            InitData(mCurrentData.ViewOwner, mCurrentData.Sensor, mCurrentData.SensorHistoryID);                
            }
        }

        public DlgSelectCase()
        {
            InitializeComponent();
        }

        public System.Windows.Forms.Timer CheckTimer
        {
            get { return timer1; }
        }

		private void InitData(SDMS.SensorZone sensor, int nSensorHistoryID)
		{
			SensorZone m_Sensor = mCurrentData.Sensor;
			m_Sensor = sensor;
            mCurrentData.SensorHistoryID = nSensorHistoryID;

			SetRealMode();
            InitSound();

			timer1.Start();
		}

		private void InitData(BaseViewEx view, SDMS.SensorZone sensor, int nSensorHistoryID)
		{
            SensorZone m_Sensor = mCurrentData.Sensor;
			m_Sensor = sensor;
            mCurrentData.SensorHistoryID = nSensorHistoryID;
            mCurrentData.ViewOwner = view;

			SetRealMode();
            InitSound();

			timer1.Start();
		}

        private void InitSound()
        {
            SensorZone m_Sensor = mCurrentData.Sensor;
            if (m_Sensor == null)
                return;

            if (m_Sensor.SoundOn)
            {
                btnSound.Text = "소리 켜짐";
                FireDetectProcess.PlaySound();
            }
            else
            {
                btnSound.Text = "소리 꺼짐";
                FireDetectProcess.SoundPlayer.Stop();
            }
        }

		// xTarget, yTarget : Target POI의 좌표
		public void Show(int xTarget, int yTarget)
		{
			int x = xTarget + m_nTargetSpaceX;
			int y = yTarget - m_nTargetSpaceY;

			this.Location = new Point(x, y);
			m_bVisible = true;

			this.Show();
		}

		// Panning이나 Orbit같은 동작을 위하여 잠시동안 임시로 꺼두는 것인가?
		private bool IsTemporaryHidden()
		{
            SensorZone m_Sensor = mCurrentData.Sensor;
            if (mCurrentData.ViewOwner == null)
				return false;

            if (mCurrentData.POI == null)
				return false;

            return mCurrentData.ViewOwner.IsTemporaryHiddenPOI(mCurrentData.POI);
		}

		public void Hide(bool absolutely)
		{
			base.Hide();
			m_bVisible = false;
		}

		public void MoveTarget(int xTarget, int yTarget)
		{
			int x = xTarget + m_nTargetSpaceX;
			int y = yTarget - m_nTargetSpaceY;

			this.Location = new Point(x, y);
		}

		public bool IsVisible()
		{
			if (m_bLayerVisible == true && m_bVisible == true)
				return true;
			return Visible;
		}

		public void HideDialog()
		{
            if (mCurrentData != null)
            {
                SensorZone m_Sensor = mCurrentData.Sensor;
                //m_Sensor.SoundOn = true;
            }
            

            timer1.Stop();
			m_bLayerVisible = false;
			m_bVisible = false;
			Visible = false;
            //mCurrentData = null;
			//base.Close();
		}

        public new DialogResult ShowDialog(IWin32Window owner)
        {
            
            return base.ShowDialog(owner);
        }

        // 상황 해제되었는지 감시
        private void timer1_Tick(object sender, EventArgs e)
        {
            SensorZone m_Sensor = mCurrentData.Sensor;
            if (m_Sensor == null)
            {
                HideDialog();
                ConfirmDialogManager.Instance.RemoveDialog(mCurrentData, true);
				
            }
            else
            {
                ProcessIF process = ProcessManager.Instance.GetProcess(m_Sensor.ID);

                if (process == null)
                {
                    HideDialog();
                    ConfirmDialogManager.Instance.RemoveDialog(mCurrentData, false);
                }
                else if (m_nProcessingSensorHistoryID > 0 && mCurrentData.SensorHistoryID == m_nProcessingSensorHistoryID)
                {
                    HideDialog();
					ConfirmDialogManager.Instance.RemoveDialog(mCurrentData, false);
                }
            }
        }

		private void DlgSelectCase_Load(object sender, EventArgs e)
		{
            if (FormMain.Instance.ShowEquipZoneCCTV)
            {
                // 이미 큰 CCTV 화면을 보고 있는 경우는 DlgSelectCase 창을 감춘다.
                BtnViewCCTV_Click(null, null);
            }
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (FireDetectProcess.SoundPlayer.SoundLocation != null)
				FireDetectProcess.SoundPlayer.Stop();
			


			ReportFireTranning();

			DialogResult = DialogResult.Yes;
            HideDialog();

			ConfirmDialogManager.Instance.RemoveDialog(mCurrentData);

			FormMain.Instance.PageHome.Invoke((MethodInvoker)delegate
			{
                SensorZone m_Sensor = mCurrentData.Sensor;
				FormMain.Instance.PageHome.ShowBigCCTV(m_Sensor.POI.Zone, false);

			});		
		}

	}

	public class ConfirmDialogManager
	{
		private static ConfirmDialogManager m_instance = null;
		public static ConfirmDialogManager Instance
		{
			get 
			{
				if (m_instance == null)
					m_instance = new ConfirmDialogManager();
				return m_instance; 
			}
			set { m_instance = value; }
		}

		private ArrayList m_arForm = new ArrayList();

		private ConfirmDialogManager()
		{
		}


        public SeletCaseData FindForm(int nSensorHistoryID)
		{
            foreach (SeletCaseData form in m_arForm)
			{
				if (form.SensorHistoryID == nSensorHistoryID)
				{
                    return (SeletCaseData)form;
				}
			}
			return null;
		}

        public SeletCaseData ShowDialog(int nSensorHistoryID, int nSensorID)
		{
			if (m_arForm.Count == 0)
				return null;

            SeletCaseData form = FindForm(nSensorHistoryID);

			if (DlgSelectCase.Instance.CurrentData == form && 
                DlgSelectCase.Instance.Visible == true)
				return null;
            else
            {
                DlgSelectCase.Instance.HideDialog();
            }

            try
            {
                CloseAllDialog();

                FormMain.Instance.Invoke((MethodInvoker)delegate
                {
                    DlgSelectCase.Instance.CurrentData = form;
                    DlgSelectCase.Instance.StartPosition = FormStartPosition.Manual;
                    DlgSelectCase.Instance.Location = PageBackstageHome.Instance.SelectCaseFirestPosition();
                    DlgSelectCase.Instance.TopMost = true;
                    DlgSelectCase.Instance.Show(FormMain.Instance);
                });
            }
            catch (System.Exception)
            {            	
            }
			return form;
		}

        public SeletCaseData ShowDialogNext()
		{			
			if(m_arForm.Count == 0)
				return null;

            SeletCaseData form = (SeletCaseData)m_arForm[0];

            if (DlgSelectCase.Instance.CurrentData == form &&
                DlgSelectCase.Instance.Visible == true)
                return null;

            try
            {
                CloseAllDialog();

                FormMain.Instance.Invoke((MethodInvoker)delegate
                {
                    DlgSelectCase.Instance.CurrentData = form;
                    DlgSelectCase.Instance.StartPosition = FormStartPosition.Manual;
                    DlgSelectCase.Instance.Location = PageBackstageHome.Instance.SelectCaseFirestPosition();
                    DlgSelectCase.Instance.TopMost = true;
                    DlgSelectCase.Instance.Show(FormMain.Instance);
                });
            }
            catch (System.Exception)
            {            	
            }		
		
			return form;
		}

        public void AddDialog(SeletCaseData form)
		{
            if (form == null)
                return;

            SeletCaseData form2 = FindForm(form.SensorHistoryID);
            if( form2 == null)
                m_arForm.Add(form);
		}

        public void AddDialogFirst(SeletCaseData form)
		{
            SeletCaseData form2 = FindForm(form.SensorHistoryID);
            if (form2 == null)
                m_arForm.Insert(0, form);
            else
            {
                m_arForm.Remove(form2);
                m_arForm.Insert(0, form);
            }
		}

        public void RemoveDialog(int nHistoryID, int nSensorID)
        {
            try
            {
                SeletCaseData form = null;
                foreach (SeletCaseData dForm in m_arForm)
                {
                    if( dForm.SensorHistoryID == nHistoryID && dForm.Sensor.ID == nSensorID)
                    {
                        form = dForm;
                    }

                }
                if (form != null)
                {
                    RemoveDialog(form, true);
                }
            }
            catch (System.Exception)
            {
            }
        }

		public void RemoveDialog(SeletCaseData data, bool bRemoveOnly = true)
		{
            SeletCaseData data2 = FindForm(data.SensorHistoryID);
            m_arForm.Remove(data2);

            try
			{
			    if (bRemoveOnly == false)
			    {		
				    FormMain.Instance.Invoke((MethodInvoker)delegate
				    {					
					    if (!FormMain.Instance.ShowEquipZoneCCTV)
					    {
                            SeletCaseData form2 = (SeletCaseData)(ShowDialogNext());
						    if (form2 != null)
						    {
							    int nID = form2.SensorHistoryID;
							    int nSensorID = form2.Sensor.ID;
							    FormMain.Instance.SelectFireDetectProcess(nID, nSensorID);
						    }	
					    }
				    });
			    }
			    //if( ! form.IsDisposed)
				//    form.Dispose();
            }
            catch (System.Exception)
            {
            }
		}
		
		public void CloseAllDialog()
		{
            DlgSelectCase.Instance.HideDialog();
		}
	}
}
