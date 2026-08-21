using System;
using System.Collections;
using System.IO;
using System.Security.AccessControl;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using Microsoft.Win32;
using UnE.Spatial;
using UnE.Sensor;
using DBUtility2;
using UnE.GUI;
using System.Collections.Generic;
using System.Drawing;
using SDMS.Help;

namespace SDMS
{
    public partial class FormSMSConfig : PopupFormBase
	{
        public enum MessageType { FACILITY_FAULT = 0, DETECT_FIRE, REPORT_FIRE, DETECT_SPILL, REPORT_SPILL, RESET_SPILL, DETECT_SECURITY, REPORT_SECURITY, RESET_SECURITY, DETECT_EARTHQUAKE, DETECT_TH, RESET_TH };
        
		private class XMLManager
		{
			private string m_strFilePath = "";

			public XMLManager()
			{
				m_strFilePath = FormMain.Instance.SimulationConfigFilePath;
			}

            public bool Write(bool useSMSOnDetectFire, bool activateTrainingMode, bool runSOPSimulatorOnReportFire, bool useSMSOnDetectSpill, bool useSMSOnReportSpill, bool useSMSOnResetSpill)
			{
				if (!File.Exists(m_strFilePath))
                    return Create(useSMSOnDetectFire, activateTrainingMode, runSOPSimulatorOnReportFire, useSMSOnDetectSpill, useSMSOnReportSpill, useSMSOnResetSpill);

				try
				{
					XmlDocument document = new XmlDocument();
					document.Load(m_strFilePath);
					XPathNavigator navigator = document.CreateNavigator();

					if (!navigator.MoveToChild("SimulationConfig", string.Empty))
						return false;

					if (!navigator.MoveToChild("SDMS", string.Empty))
					{
						XmlWriter writer = navigator.AppendChild();
						writer.WriteStartElement("SDMS");
						writer.WriteEndElement();
						writer.Close();

						if (!navigator.MoveToChild("SDMS", string.Empty))
							return false;
					}

					if (navigator.MoveToChild("MessageOptions", string.Empty))
					{
						navigator.DeleteSelf();
					}

					XmlWriter writer2 = navigator.AppendChild();
					WriteMessageOptions(writer2, useSMSOnDetectFire, activateTrainingMode, runSOPSimulatorOnReportFire, useSMSOnDetectSpill,  useSMSOnReportSpill,  useSMSOnResetSpill);
					writer2.Close();

					document.Save(m_strFilePath);
				}
				catch (Exception)
				{
					return false;
				}

				return true;
			}

            private bool Create(bool useSMSOnDetectFire, bool activateTrainingMode, bool runSOPSimulatorOnReportFire, bool useSMSOnDetectSpill, bool useSMSOnReportSpill, bool useSMSOnResetSpill)
			{
				XmlTextWriter writer = null;

				try
				{
					writer = new XmlTextWriter(m_strFilePath, Encoding.UTF8);

					writer.WriteStartElement("SimulationConfig");
					writer.WriteStartElement("SDMS");

					WriteMessageOptions(writer, useSMSOnDetectFire, activateTrainingMode, runSOPSimulatorOnReportFire, useSMSOnDetectSpill, useSMSOnReportSpill, useSMSOnResetSpill);

					writer.WriteEndElement();
					writer.WriteEndElement();
				}
				catch (Exception)
				{
					if (writer != null)
						writer.Close();

					return false;
				}

				writer.Close();
				return true;
			}

            private void WriteMessageOptions(XmlWriter writer, bool useSMSOnDetectFire, bool activateTrainingMode, bool runSOPSimulatorOnReportFire,
                bool useSMSOnDetectSpill, bool useSMSOnReportSpill, bool useSMSOnResetSpill)
			{
				writer.WriteStartElement("MessageOptions");

				writer.WriteStartElement("SMSOnDetectFire");
				writer.WriteString(useSMSOnDetectFire.ToString());
				writer.WriteEndElement();

				writer.WriteStartElement("ActivateTrainingMode");
				writer.WriteString(activateTrainingMode.ToString());
				writer.WriteEndElement();

				writer.WriteStartElement("RunSOPSimulatorOnReportFire");
				writer.WriteString(runSOPSimulatorOnReportFire.ToString());
				writer.WriteEndElement();

                writer.WriteStartElement("SMSOnDetectSpill");
                writer.WriteString(useSMSOnDetectSpill.ToString());
				writer.WriteEndElement();

                writer.WriteStartElement("SMSOnReportSpill");
                writer.WriteString(useSMSOnReportSpill.ToString());
				writer.WriteEndElement();

                writer.WriteStartElement("SMSOnResetSpill");
                writer.WriteString(useSMSOnResetSpill.ToString());
				writer.WriteEndElement();

				writer.WriteEndElement();
			}

			public void Read(out bool useSMSOnDetectFire, out bool useSMSOnReportFire, out bool activateTrainingMode, out bool runSOPSimulatorOnReportFire, out bool useSMSOnDetectSpill, out bool useSMSOnReportSpill, out bool useSMSOnResetSpill)
			{
				useSMSOnDetectFire = true;
                useSMSOnReportFire = true;
				activateTrainingMode = false;
				runSOPSimulatorOnReportFire = false;
                
                useSMSOnDetectSpill = true;
                useSMSOnReportSpill = true;
                useSMSOnResetSpill = false;

				if (!File.Exists(m_strFilePath))
					return;

				XmlTextReader reader = null;
				bool stop = false;

				try
				{
					reader = new XmlTextReader(m_strFilePath);

					while (reader.Read())
					{
						switch (reader.NodeType)
						{
							case XmlNodeType.Element:
                                if (string.Compare(reader.Name, "SimulationConfig", true) == 0)
                                    ReadConfig(reader, ref useSMSOnDetectFire, ref activateTrainingMode, ref runSOPSimulatorOnReportFire, ref useSMSOnDetectSpill, ref useSMSOnReportSpill, ref useSMSOnResetSpill);
								stop = true;
								break;
						}

						if (stop)
							break;
					}
				}
				catch (Exception)
				{
					if (reader != null)
						reader.Close();
					return;
				}

				reader.Close();
			}

			private void ReadConfig(XmlTextReader reader, ref bool useSMSOnDetectFire, ref bool activateTrainingMode, ref bool runSOPSimulatorOnReportFire, ref bool useSMSOnDetectSpill, ref bool useSMSOnReportSpill, ref bool useSMSOnResetSpill)
			{
				bool stop = false;

				try
				{
					while (reader.Read())
					{
						switch (reader.NodeType)
						{
							case XmlNodeType.Element:
								if (string.Compare(reader.Name, "SDMS", true) == 0)
								{
									ReadSDMS(reader, ref useSMSOnDetectFire, ref activateTrainingMode, ref runSOPSimulatorOnReportFire, ref useSMSOnDetectSpill, ref useSMSOnReportSpill, ref useSMSOnResetSpill);
									stop = true;
								}
								break;

							case XmlNodeType.EndElement:
								stop = true;
								break;
						}

						if (stop)
							break;
					}
				}
				catch (Exception)
				{
					return;
				}
			}

            private void ReadSDMS(XmlTextReader reader, ref bool useSMSOnDetectFire, ref bool activateTrainingMode, ref bool runSOPSimulatorOnReportFire, ref bool useSMSOnDetectSpill, ref bool useSMSOnReportSpill, ref bool useSMSOnResetSpill)
			{
				bool stop = false;

				try
				{
					while (reader.Read())
					{
						switch (reader.NodeType)
						{
							case XmlNodeType.Element:
								if (string.Compare(reader.Name, "MessageOptions", true) == 0)
								{
                                    ReadMessageOptions(reader, ref useSMSOnDetectFire, ref activateTrainingMode, ref runSOPSimulatorOnReportFire, ref useSMSOnDetectSpill, ref useSMSOnReportSpill, ref useSMSOnResetSpill);
									stop = true;
								}
								else
									PassElement(reader);
								break;

							case XmlNodeType.EndElement:
								stop = true;
								break;
						}

						if (stop)
							break;
					}
				}
				catch (Exception)
				{
				}
			}

			private void PassElement(XmlTextReader reader)
			{
				if (reader.IsEmptyElement)
					return;

				while (reader.Read())
				{
					switch (reader.NodeType)
					{
						case XmlNodeType.Element:
							PassElement(reader);
							break;

						case XmlNodeType.EndElement:
							return;
					}
				}
			}

			private void ReadMessageOptions(XmlTextReader reader, ref bool useSMSOnDetectFire, ref bool activateTrainingMode, ref bool runSOPSimulatorOnReportFire,ref bool useSMSOnDetectSpill, ref bool useSMSOnReportSpill, ref bool useSMSOnResetSpill)
			{
				bool stop = false;
				string strData = "";

				if (reader.IsEmptyElement)
					return;

				try
				{
					while (reader.Read())
					{
						switch (reader.NodeType)
						{
							case XmlNodeType.Element:
								if (string.Compare(reader.Name, "SMSOnDetectFire", true) == 0)
								{
									if (!ReadText(reader, ref strData))
										stop = true;
									else
									{
										if (!bool.TryParse(strData, out useSMSOnDetectFire))
											stop = true;
									}
								}
                                else if (string.Compare(reader.Name, "SMSOnDetectSpill", true) == 0)
                                {
                                    if (!ReadText(reader, ref strData))
                                        stop = true;
                                    else
                                    {
                                        if (!bool.TryParse(strData, out useSMSOnDetectSpill))
                                            stop = true;
                                    }
                                }
                                else if (string.Compare(reader.Name, "SMSOnReportSpill", true) == 0)
                                {
                                    if (!ReadText(reader, ref strData))
                                        stop = true;
                                    else
                                    {
                                        if (!bool.TryParse(strData, out useSMSOnReportSpill))
                                            stop = true;
                                    }
                                }
                                else if (string.Compare(reader.Name, "SMSOnResetSpill", true) == 0)
                                {
                                    if (!ReadText(reader, ref strData))
                                        stop = true;
                                    else
                                    {
                                        if (!bool.TryParse(strData, out useSMSOnResetSpill))
                                            stop = true;
                                    }
                                }
								else if (string.Compare(reader.Name, "ActivateTrainingMode", true) == 0)
								{
									if (!ReadText(reader, ref strData))
										stop = true;
									else
									{
										if (!bool.TryParse(strData, out activateTrainingMode))
											stop = true;
									}
								}
								else if (string.Compare(reader.Name, "RunSOPSimulatorOnReportFire", true) == 0)
								{
									if (!ReadText(reader, ref strData))
										stop = true;
									else
									{
										if (!bool.TryParse(strData, out runSOPSimulatorOnReportFire))
											stop = true;
									}
								}
								break;

							case XmlNodeType.EndElement:
								stop = true;
								break;
						}

						if (stop)
							break;
					}
				}
				catch (Exception)
				{
				}
			}

			private bool ReadText(XmlTextReader reader, ref string strText, bool allowEmpty = false)
			{
				if (reader.IsEmptyElement)
				{
					strText = "";
					return allowEmpty;
				}

				if (!ReadElementText(reader, ref strText))
					strText = "";

				return true;
			}

			private bool ReadElementText(XmlTextReader reader, ref string strText)
			{
				bool stop = false, readText = false;

				while (reader.Read())
				{
					switch (reader.NodeType)
					{
						case XmlNodeType.Text:
							strText = reader.Value;
							readText = true;
							break;

						case XmlNodeType.EndElement:
							stop = true;
							break;
					}

					if (stop)
						break;
				}

				return readText;
			}
		}

        private static VariousData<bool> m_useSMSOnDetectFire = null;
        private static VariousData<bool> m_useSMSOnReportFire = null;
        private static VariousData<bool> m_activateTrainingMode = null;
        private static VariousData<bool> m_runSOPSimulatorOnReportFire = null;

        private static VariousData<bool> m_useSMSOnDetectSpill = null;
        private static VariousData<bool> m_useSMSOnReportSpill = null;
        private static VariousData<bool> m_useSMSOnResetSpill = null;
        
        public static bool UseSMSOnResetSpill
        {
            get
            {
                if (m_useSMSOnResetSpill == null)
                    ReadSimulationOptions();

                return m_useSMSOnResetSpill.Data;
            }
        }
        
        public static bool UseSMSOnDetectSpill
        {
            get
            {
                if (m_useSMSOnDetectSpill == null)
                    ReadSimulationOptions();

                return m_useSMSOnDetectSpill.Data;
            }
        }

        public static bool UseSMSOnReportSpill
        {
            get
            {
                if (m_useSMSOnReportSpill == null)
                    ReadSimulationOptions();

                return m_useSMSOnReportSpill.Data;
            }
        }        

        public static bool UseSMSOnDetectFire
        {
            get
            {
                if (m_useSMSOnDetectFire == null)
                    ReadSimulationOptions();

                return m_useSMSOnDetectFire.Data;
            }
        }

        public static bool UseSMSOnReportFire
        {
            get
            {
                if (m_useSMSOnReportFire == null)
                    ReadSimulationOptions();

                return m_useSMSOnReportFire.Data;
            }
        }

        public static bool ActivateTrainingMode
        {
            get
            {
                if (m_activateTrainingMode == null)
                    ReadSimulationOptions();

                return m_activateTrainingMode.Data;
            }
        }

        public static bool RunSOPSimulatorOnReportFire
        {
            get
            {
                if (m_runSOPSimulatorOnReportFire == null)
                    ReadSimulationOptions();

                return m_runSOPSimulatorOnReportFire.Data;
            }
        }

		private XMLManager m_xmlManager = null;
        	
		private static void ReadSimulationOptions()
		{
			XMLManager mgr = new XMLManager();
            
            bool useSMSOnDetectSpill, useSMSOnReportSpill, useSMSOnResetSpill;

			bool useSMSOnDetectFire, useSMSOnReportFire, activateTrainingMode, runSOPSimulatorOnReportFire;
			mgr.Read(out useSMSOnDetectFire, out useSMSOnReportFire, out activateTrainingMode, out runSOPSimulatorOnReportFire, out useSMSOnDetectSpill, out useSMSOnReportSpill, out useSMSOnResetSpill);


            if (m_useSMSOnDetectFire == null)
			    m_useSMSOnDetectFire = new VariousData<bool>(useSMSOnDetectFire);

            if (m_useSMSOnReportFire == null)
                m_useSMSOnReportFire = new VariousData<bool>(useSMSOnReportFire);

            if (m_activateTrainingMode == null)
			    m_activateTrainingMode = new VariousData<bool>(activateTrainingMode);

            if (m_runSOPSimulatorOnReportFire == null)
                m_runSOPSimulatorOnReportFire = new VariousData<bool>(runSOPSimulatorOnReportFire);

            if (m_useSMSOnDetectSpill == null)
                m_useSMSOnDetectSpill = new VariousData<bool>(useSMSOnDetectSpill);

            if (m_useSMSOnReportSpill == null)
                m_useSMSOnReportSpill = new VariousData<bool>(useSMSOnReportSpill);

            if (m_useSMSOnResetSpill == null)
			    m_useSMSOnResetSpill = new VariousData<bool>(useSMSOnResetSpill);
		}
        
        private int m_nSiteID = 1;

        private ManualManager m_manualManager = null;

		public FormSMSConfig()
		{
            this.DoubleBuffered = true;

            m_nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;

			m_xmlManager = new XMLManager();
			InitializeComponent();

            this.UseFrmMove = false;
            InitCtrlSize(this);
            SetChildCtrlResize(this, this.Width, this.Height);
            SetChildCtrlLocation();

            m_manualManager = new ManualManager(this);
            SetManualID();
		}

		private void FormSMSConfig_Load(object sender, EventArgs e)
		{
			LoadDB();

			checkBoxSendSMSToDuty.Checked = m_bSendSmsNightDuty;

			m_useSMSOnDetectFire = new VariousData<bool>(checkBoxDetectFire.Checked);
            m_useSMSOnReportFire = new VariousData<bool>(checkBoxReportFire.Checked);
			m_activateTrainingMode = new VariousData<bool>(checkBoxActivateTrainingMode.Checked);
			m_runSOPSimulatorOnReportFire = new VariousData<bool>(ckbRunSimulator.Checked);
		}

		private void LoadDB()
		{
			WebDBManager dbMgr = FormMain.Instance.DBManager;
            string strSQL = "Select id, MessageType, UseSMS from SDMSSMSConfig where SiteID = " + m_nSiteID.ToString();

			ArrayList arrResult = dbMgr.GetResultData(strSQL);
			if (arrResult == null)
				return;

			int nResultCount = arrResult.Count;

			for (int i = 0; i < nResultCount - 2; i += 3)
			{
				int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
				int nMessageType = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
				bool useSMS = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0) == 0 ? false : true;

				if (nMessageType == (int)MessageType.FACILITY_FAULT)
					checkBoxFacilityFault.Checked = useSMS;
				else if (nMessageType == (int)MessageType.DETECT_FIRE)
					checkBoxDetectFire.Checked = useSMS;
                else if (nMessageType == (int)MessageType.REPORT_FIRE)
                    checkBoxReportFire.Checked = useSMS;

                else if (nMessageType == (int)MessageType.DETECT_SPILL)
                    this.ckbDetectSpill.Checked = useSMS;
                else if (nMessageType == (int)MessageType.REPORT_SPILL)
                    this.ckbReportSpill.Checked = useSMS;
                else if (nMessageType == (int)MessageType.RESET_SPILL)
                    this.ckbReportReset.Checked = useSMS;

                else if (nMessageType == (int)MessageType.DETECT_SECURITY)
                    this.checkBoxDetectSecurity.Checked = useSMS;
                else if (nMessageType == (int)MessageType.REPORT_SECURITY)
                    this.checkBoxReportSecurity.Checked = useSMS;
                else if (nMessageType == (int)MessageType.RESET_SECURITY)
                    this.checkBoxResetSecurity.Checked = useSMS;

                else if (nMessageType == (int)MessageType.DETECT_TH)
                    this.ckbDetectSpill.Checked = useSMS;
                else if (nMessageType == (int)MessageType.RESET_TH)
                    this.ckbReportReset.Checked = useSMS;
            }

			bool isRealMode = true;

			string szSQL2 = "SELECT PropertyValue FROM OptionSDMS where PropertyName ='TranningMode' AND SiteID = " + m_nSiteID.ToString();;
			ArrayList arResult2 = dbMgr.GetResultData(szSQL2);
			if (arResult2 == null || arResult2.Count == 0)
			{
				isRealMode = true;
			}
			else
			{
				int nDur = WebDBManager.GetIntField(arResult2[0].ToString(), 0);
				if (nDur == 1)
					isRealMode = false;
				else
					isRealMode = true;
			}

			PreferenceManager.Instance.RealMode = isRealMode;
			checkBoxActivateTrainingMode.Checked = !isRealMode;
			ckbRunSimulator.Checked = ReadRunSimulator();

            // 171114 KYJ
            // SMS 전송시 앞머리 문구 추가
            string strHeaderMsg = "훈련상황";
            string query = "SELECT PropertyValue FROM OptionSDMS where PropertyName ='HeaderMsg' AND SiteID = " + m_nSiteID.ToString();
			ArrayList arrRes = dbMgr.GetResultData(query);
			if (arrRes == null || arrRes.Count == 0)
            {
                query = "SELECT MAX(ID) FROM OptionSDMS";
                arrRes = FormMain.Instance.DBManager.GetResultData(query);
                if (arrRes != null)
                {
                    int idx = WebDBManager.GetIntField(arrRes[0].ToString(), 0);
                    query = string.Format("INSERT INTO OptionSDMS(ID, PropertyName, PropertyValue, Description, SiteID) VALUES({2}, 'HeaderMsg', '{0}', '메시지 앞머리 문구', {1})", strHeaderMsg, m_nSiteID, idx + 1);
                    arrRes = FormMain.Instance.DBManager.GetResultData(query);
                }
            }
            else
            {
                strHeaderMsg = WebDBManager.GetStringField(arrRes[0].ToString(), "");
            }
            txt_msgHeader.Text = strHeaderMsg;
            txt_msgHeader.ReadOnly = !checkBoxActivateTrainingMode.Checked;
            //

			/*string szSQL3 = "SELECT PropertyValue FROM OptionSDMS where PropertyName='BroadcastOn'";
			ArrayList arResult3 = dbMgr.GetResultData(szSQL3);
		   if (arResult3 == null || arResult3.Count == 0)
		   {
			   m_bEnableBroadcast = false;
		   }
		   else
		   {
			   int nTemp = WebDBManager.GetIntField(arResult3[0].ToString(), -1);

			   if (nTemp == 1)
				   m_bEnableBroadcast = true;
			   else
				   m_bEnableBroadcast = false;
		   }

		   string szSQL4 = "SELECT PropertyValue FROM OptionSDMS where PropertyName='SendSMSonNightDuty'";
		   ArrayList arResult4 = dbMgr.GetResultData(szSQL4);
		   if (arResult4 == null || arResult4.Count == 0)
		   {
			   m_bSendSmsNightDuty = false;
		   }
		   else
		   {
			   int nTemp = WebDBManager.GetIntField(arResult4[0].ToString(), -1);

			   if (nTemp == 1)
				   m_bSendSmsNightDuty = true;
			   else
				   m_bSendSmsNightDuty = false;
		   }*/
		}

		private bool m_bSendSmsNightDuty = false;

		private void SaveDB()
		{
			bool useFacilityFault = checkBoxFacilityFault.Checked;
			bool useDetectFire = checkBoxDetectFire.Checked;
            bool useReportFire = checkBoxReportFire.Checked;

            if (useDetectFire== false)
            {
                useFacilityFault = false;
            }

			Save(useFacilityFault, MessageType.FACILITY_FAULT);
			Save(useDetectFire, MessageType.DETECT_FIRE);
            Save(useReportFire, MessageType.REPORT_FIRE);

            if (UnE.SOP.ProxySOP.Instance.UsePSM)
            {
                bool useResetSpiil = ckbReportReset.Checked;
                bool useDetectSpill = ckbDetectSpill.Checked;
                bool useReportSpill = ckbReportSpill.Checked;

                if (useDetectSpill == false)
                {
                    useResetSpiil = false;
                }

                Save(useResetSpiil, MessageType.RESET_SPILL);
                Save(useDetectSpill, MessageType.DETECT_SPILL);
                Save(useReportSpill, MessageType.REPORT_SPILL);
            }
            else if (FormMain.Instance.UseTH)
            {
                bool useResetTH = ckbReportReset.Checked;
                bool useDetectTH = ckbDetectSpill.Checked;

                if (useDetectTH == false)
                {
                    useResetTH = false;
                }

                Save(useResetTH, MessageType.RESET_TH);
                Save(useDetectTH, MessageType.DETECT_TH);
            }

            bool useResetSecurity = checkBoxResetSecurity.Checked;
            bool useDetectSecurity = checkBoxDetectSecurity.Checked;
            bool useReportSecurity = checkBoxReportSecurity.Checked;

            if (useDetectSecurity == false)
            {
                useResetSecurity = false;
            }

            Save(useResetSecurity, MessageType.RESET_SECURITY);
            Save(useDetectSecurity, MessageType.DETECT_SECURITY);
            Save(useReportSecurity, MessageType.REPORT_SECURITY);


			string szSQL1 = string.Format("UPDATE OptionSDMS SET PropertyValue={0} WHERE PropertyName='TranningMode' and SiteID = {1}", checkBoxActivateTrainingMode.Checked ? 1 : 0, m_nSiteID);
			FormMain.Instance.DBManager.GetResultData(szSQL1);

            // 171114 KYJ
            // SMS 전송시 앞머리 문구 추가
            string strMsgHeader = txt_msgHeader.Text.Trim();
            //if (strMsgHeader != "")
            {
                szSQL1 = string.Format("UPDATE OptionSDMS SET PropertyValue='{0}' WHERE PropertyName='HeaderMsg' and SiteID = {1}", strMsgHeader, m_nSiteID);
                FormMain.Instance.DBManager.GetResultData(szSQL1);
            }

			/*string szSQL2 = string.Format("UPDATE OptionSDMS SET PropertyValue={0} WHERE PropertyName='BroadcastOn'", m_bEnableBroadcast == true ? 1 : 0);
			FormMain.Instance.DBManager.GetResultData(szSQL2);

			string szSQL3 = string.Format("UPDATE OptionSDMS SET PropertyValue={0} WHERE PropertyName='SendSMSonNightDuty'", m_bSendSmsNightDuty == true ? 1 : 0);
			FormMain.Instance.DBManager.GetResultData(szSQL3);*/

			WriteRunSimulator(ckbRunSimulator.Checked);

			m_useSMSOnDetectFire = new VariousData<bool>(checkBoxDetectFire.Checked);
            m_useSMSOnReportFire = new VariousData<bool>(checkBoxReportFire.Checked);
			m_activateTrainingMode = new VariousData<bool>(checkBoxActivateTrainingMode.Checked);
			m_runSOPSimulatorOnReportFire = new VariousData<bool>(ckbRunSimulator.Checked);

            m_useSMSOnDetectSpill = new VariousData<bool>(ckbDetectSpill.Checked);
            m_useSMSOnReportSpill = new VariousData<bool>(ckbReportSpill.Checked);
            m_useSMSOnResetSpill = new VariousData<bool>(ckbReportReset.Checked);

		}

		private void Save(bool useSMS, MessageType type)
		{
			WebDBManager dbMgr = FormMain.Instance.DBManager;
			
            string szSQL = string.Format("select ID from SDMSSMSConfig  where MessageType = {0} and SiteID = {1}", (int)type, m_nSiteID);
            ArrayList arTemp = dbMgr.GetResultData(szSQL);
            if (arTemp == null || arTemp.Count == 0)
			{
				ArrayList arrResult = dbMgr.GetResultData("Select max(id) from SDMSSMSConfig");
				if (arrResult == null)
					return;

				int nID = arrResult.Count == 0 ? 1 : WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;
				string strSQL = string.Format("Insert into SDMSSMSConfig (ID, MessageType, UseSMS, Description, SiteID) values ({0}, {1}, {2}, NULL, {3})",
					nID, (int)type, useSMS ? 1 : 0, m_nSiteID);

				dbMgr.GetResultData(strSQL);
			}
            else
            {
                string strSQL = string.Format("Update SDMSSMSConfig set UseSMS = {0} where MessageType = {1} and SiteID = {2}", useSMS ? 1 : 0, (int)type, m_nSiteID);
                dbMgr.GetResultData(strSQL);
            }

			PreferenceManager.Instance.RealMode = !checkBoxActivateTrainingMode.Checked;
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			SaveDB();

			DialogResult = System.Windows.Forms.DialogResult.OK;

			Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
			Close();
		}

		private void labelDetect_Click(object sender, EventArgs e)
		{
			checkBoxDetectFire.Checked = !checkBoxDetectFire.Checked;
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
            txt_msgHeader.ReadOnly = !checkBoxActivateTrainingMode.Checked;
		}

		private void checkBox3_CheckedChanged(object sender, EventArgs e)
		{
			if (checkBoxSendSMSToDuty.Checked)
			{
				m_bSendSmsNightDuty = true;
			}
			else
			{
				m_bSendSmsNightDuty = false;
			}
		}

		private void ckbRunSimulator_CheckedChanged(object sender, EventArgs e)
		{
		}

		public static bool ReadRunSimulator()
		{
			bool bResult = false;
			try
			{
				RegistryKey rkey = Registry.CurrentUser.OpenSubKey(@"SDMS\Simulator");
				if (rkey == null)
				{
					return true;
				}
				else
				{
					int nRun = (int)rkey.GetValue("Run", 1);
					bResult = (nRun == 1 ? true : false);
				}
				if (rkey != null)
					rkey.Close();
			}
			catch (System.Exception)
			{
			}
			return bResult;
		}

		public static void WriteRunSimulator(bool bCheck)
		{
			try
			{
				string szUserName = Environment.UserDomainName + "\\" + Environment.UserName;

				RegistrySecurity rs = new RegistrySecurity();

				rs.AddAccessRule(new RegistryAccessRule(szUserName,
					RegistryRights.ReadKey | RegistryRights.Delete | RegistryRights.WriteKey,
					InheritanceFlags.None,
					PropagationFlags.None,
					AccessControlType.Allow));

				rs.AddAccessRule(new RegistryAccessRule(szUserName,
					RegistryRights.ChangePermissions,
					InheritanceFlags.None,
					PropagationFlags.None,
					AccessControlType.Deny));

				RegistryKey rkey = Registry.CurrentUser.OpenSubKey(@"SDMS\Simulator", true);
				if (rkey == null)
				{
					try
					{
						rkey = Registry.CurrentUser.CreateSubKey(@"SDMS\Simulator", RegistryKeyPermissionCheck.ReadWriteSubTree, rs);
					}
					catch (Exception)
					{
					}
				}

				if (rkey != null)
				{
					int nValue = (bCheck == true ? 1 : 0);
					rkey.SetValue("Run", nValue);
					rkey.Close();
				}
			}
			catch (System.Exception)
			{
			}
		}

        private void checkBoxFacilityFault_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxDetectFire_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxDetectFire.Checked == false)
            {
                //checkBoxFacilityFault.Enabled = false;
                checkBoxFacilityFault.Checked = false;
            }
            //else
            //{
            //    checkBoxFacilityFault.Enabled = true;
            //}
        }

        private void checkBoxReportFire_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxReportFire.Checked == false)
            {
                //checkBoxFacilityFault.Enabled = false;
                checkBoxFacilityFault.Checked = false;
            }
            //else
            //{
            //    checkBoxFacilityFault.Enabled = true;
            //}
        }

        private void ckbReportReset_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void ckbReportSpill_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbReportSpill.Checked == false)
            {
                if (ckbDetectSpill.Checked == false)
                {
                    //ckbReportReset.Enabled = false;
                    ckbReportReset.Checked = false;
                }

            }
            //else
            //{
            //    ckbReportReset.Enabled = true;
            //}
        }

        private void ckbDetectSpill_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbDetectSpill.Checked == false)
            {
                if (ckbReportSpill.Checked == false)
                {
                    //ckbReportReset.Enabled = false;
                    ckbReportReset.Checked = false;
                }
            }
            //else
            //{
            //    ckbReportReset.Enabled = true;
            //}
        }
         
        public void SetChildCtrlLocation()
        {
            if (UnE.SOP.ProxySOP.Instance.UsePSM == false)
            {
                labelPsm.Enabled = false;
                ckbDetectSpill.Enabled = false;
                ckbReportSpill.Enabled = false;
                ckbReportReset.Enabled = false;

                if (UnE.SOP.ProxySOP.Instance.UseIntrusion)
                {
                    labelPsm.Visible = false;
                    ckbDetectSpill.Visible = false;
                    ckbReportSpill.Visible = false;
                    ckbReportReset.Visible = false;
                }
            }

            if (UnE.SOP.ProxySOP.Instance.UseIntrusion)
            {
                labelSecurity.Visible = true;
                checkBoxDetectSecurity.Visible = true;
                checkBoxReportSecurity.Visible = true;
                checkBoxResetSecurity.Visible = true;

                labelSecurity.Location = labelPsm.Location;
                checkBoxDetectSecurity.Location = ckbDetectSpill.Location;
                checkBoxReportSecurity.Location = ckbReportSpill.Location;
                checkBoxResetSecurity.Location = ckbReportReset.Location;
            }

            if (UnE.SOP.ProxySOP.Instance.UsePSM == false && FormMain.Instance.UseTH)
            {
                labelPsm.Text = "온도/습도 신호";
                ckbDetectSpill.Text = "신호탐지시 담당자에게 문자 메시지 발송";
                ckbReportSpill.Text = "신호전파시 담당자에게 문자 메시지 발송";

                labelPsm.Enabled = true;
                labelPsm.Visible = true;
                ckbDetectSpill.Enabled = true;
                ckbDetectSpill.Visible = true;
                ckbReportReset.Enabled = ckbReportReset.Visible = true;
            }
        }

        private void SetManualID()
        {
            m_manualManager.Handle = this.Handle;

            m_manualManager.Clear();

            m_manualManager.SetID(this, "SDMS_Manage_SMS");
            m_manualManager.SetID(labelFire, "SMS_Option_FireSensor");
            m_manualManager.SetID(labelMode, "SMS_Option_SOPLink");
            m_manualManager.SetID(labelPsm, "SMS_Option_PSMSensor");
            if (UnE.SOP.ProxySOP.Instance.UseIntrusion)
                m_manualManager.SetID(labelSecurity, "SMS_Option_SecuritySensor");
            m_manualManager.ProcessEvent();
        } 
	}
}