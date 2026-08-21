using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;

namespace SDMS
{
	public partial class FormManager_Simulation : PopupFormBase
	{
		private class SMSManager
		{
			private string m_strManagerName = "";
			private string m_strPhoneNumber = "";

			public string ManagerName
			{
				get { return m_strManagerName; }
				set { m_strManagerName = value; }
			}

			public string PhoneNumber
			{
				get { return m_strPhoneNumber; }
				set { m_strPhoneNumber = value; }
			}

			public SMSManager()
			{
			}

			public SMSManager(string strManagerName, string strPhoneNumber)
			{
				m_strManagerName = strManagerName;
				m_strPhoneNumber = strPhoneNumber;
			}
		}

		private class XMLManager
		{
			private string m_strFilePath = "";

			public XMLManager()
			{
				m_strFilePath = FormMain.Instance.SimulationConfigFilePath;
			}

			public bool Write(Dictionary<string, string> dicManagerPhoneNumbers)
			{
				if (!File.Exists(m_strFilePath))
					return Create(dicManagerPhoneNumbers);

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

					if (!navigator.MoveToChild("SMSManagers", string.Empty))
					{
						XmlWriter writer = navigator.AppendChild();
						writer.WriteStartElement("SMSManagers");
						writer.WriteEndElement();
						writer.Close();

						if (!navigator.MoveToChild("SMSManagers", string.Empty))
							return false;
					}

					navigator.DeleteSelf();

					XmlWriter writer2 = navigator.AppendChild();
					WriteSMSManagers(writer2, dicManagerPhoneNumbers);
					writer2.Close();

					document.Save(m_strFilePath);
				}
				catch (Exception)
				{
					return false;
				}

				return true;
			}

			private bool Create(Dictionary<string, string> dicManagerPhoneNumbers)
			{
				XmlTextWriter writer = null;

				try
				{
					writer = new XmlTextWriter(m_strFilePath, Encoding.UTF8);

					writer.WriteStartElement("SimulationConfig");
					writer.WriteStartElement("SDMS");

					WriteSMSManagers(writer, dicManagerPhoneNumbers);

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

			private void WriteSMSManagers(XmlWriter writer, Dictionary<string, string> dicManagerPhoneNumbers)
			{
				writer.WriteStartElement("SMSManagers");

				foreach (KeyValuePair<string, string> pair in dicManagerPhoneNumbers)
				{
					writer.WriteStartElement("SMSManager");

					writer.WriteStartElement("ManagerName");
					writer.WriteString(pair.Key);
					writer.WriteEndElement();

					writer.WriteStartElement("PhoneNumber");
					writer.WriteString(pair.Value);
					writer.WriteEndElement();

					writer.WriteEndElement();
				}

				writer.WriteEndElement();
			}

			public List<SMSManager> Read()
			{
				if (!File.Exists(m_strFilePath))
					return null;

				XmlTextReader reader = null;
				List<SMSManager> managers = null;
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
									managers = ReadConfig(reader);
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
					return null;
				}

				reader.Close();
				return managers;
			}

			private List<SMSManager> ReadConfig(XmlTextReader reader)
			{
				bool stop = false;
				List<SMSManager> managers = null;

				try
				{
					while (reader.Read())
					{
						switch (reader.NodeType)
						{
							case XmlNodeType.Element:
								if (string.Compare(reader.Name, "SDMS", true) == 0)
								{
									managers = ReadSDMS(reader);
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
					return null;
				}

				return managers;
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

			private List<SMSManager> ReadSDMS(XmlTextReader reader)
			{
				bool stop = false;
				List<SMSManager> managers = null;

				try
				{
					while (reader.Read())
					{
						switch (reader.NodeType)
						{
							case XmlNodeType.Element:
								if (string.Compare(reader.Name, "SMSManagers", true) == 0)
								{
									managers = ReadSMSManagers(reader);
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
					return null;
				}

				return managers;
			}

			private List<SMSManager> ReadSMSManagers(XmlTextReader reader)
			{
				bool stop = false;
				List<SMSManager> managers = new List<SMSManager>();

				if (reader.IsEmptyElement)
					return managers;

				try
				{
					while (reader.Read())
					{
						switch (reader.NodeType)
						{
							case XmlNodeType.Element:
								if (string.Compare(reader.Name, "SMSManager", true) == 0)
								{
									SMSManager manager = ReadSMSManager(reader);

									if (manager == null)
									{
										stop = true;
										managers = null;
									}
									else
										managers.Add(manager);
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
					return null;
				}

				return managers;
			}

			private SMSManager ReadSMSManager(XmlTextReader reader)
			{
				bool stop = false;
				string strManagerName = "", strPhoneNumber = "";

				try
				{
					while (reader.Read())
					{
						switch (reader.NodeType)
						{
							case XmlNodeType.Element:
								if (string.Compare(reader.Name, "ManagerName", true) == 0)
								{
									if (!ReadText(reader, ref strManagerName))
										stop = true;
								}
								else if (string.Compare(reader.Name, "PhoneNumber", true) == 0)
								{
									if (!ReadText(reader, ref strPhoneNumber))
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
					return null;
				}

				if (strManagerName.Length == 0 || strPhoneNumber.Length == 0)
					return null;

				return new SMSManager(strManagerName, strPhoneNumber);
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

		private string m_strConfigPath = "";
		private XMLManager m_xmlManager = null;

		// Key : 이름
		// Value : 전화번호('-'이나 띄워쓰기 없음)
		private static Dictionary<string, string> m_dicManagerPhoneNumbers = null;//new Dictionary<string, string>();

		public static Dictionary<string, string> ManagerPhoneNumbers
		{
			get
			{
				if (m_dicManagerPhoneNumbers == null)
				{
					FormManager_Simulation frm = new FormManager_Simulation();
					frm.Dispose();
				}

				return m_dicManagerPhoneNumbers;
			}
		}

		public FormManager_Simulation()
		{
			m_xmlManager = new XMLManager();

			ReadConfig();
			InitializeComponent();

			foreach (DataGridViewColumn column in gridManager.Columns)
			{
				column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
			}

            InitCtrlSize(this);
            SetChildCtrlResize(this, this.Width, this.Height);
            FormMain.Instance.CustomizeGridView(gridManager);
		}

		private void ReadConfig()
		{
			List<SMSManager> managers = m_xmlManager.Read();

			if (m_dicManagerPhoneNumbers == null)
				m_dicManagerPhoneNumbers = new Dictionary<string, string>();

			if (managers == null)
				return;
			else
				m_dicManagerPhoneNumbers.Clear();

			foreach (SMSManager manager in managers)
			{
				m_dicManagerPhoneNumbers[manager.ManagerName] = manager.PhoneNumber;
			}
		}

		private void WriteConfig()
		{
			m_xmlManager.Write(m_dicManagerPhoneNumbers);
		}

		private void FormManager_Simulation_Load(object sender, EventArgs e)
		{
			int nColumnIndex = 1;

			foreach (KeyValuePair<string, string> pair in m_dicManagerPhoneNumbers)
			{
				DataGridViewRow row = new DataGridViewRow();

				DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
				cell.Value = nColumnIndex++;
				row.Cells.Add(cell);
				cell.ReadOnly = true;

				cell = new DataGridViewTextBoxCell();
				cell.Value = pair.Key;
				row.Cells.Add(cell);

				cell = new DataGridViewTextBoxCell();
				cell.Value = pair.Value;
				row.Cells.Add(cell);

				gridManager.Rows.Add(row);
			}

            btnEdit_Click(null, null);
		}

        private bool m_isEdit = false;
        private Image imgCheckBoxUnChecked = global::SDMS.Properties.Resources.CheckBox_Default;
        private Image imgCheckBoxChecked = global::SDMS.Properties.Resources.CheckBox_Click;

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (sender != null && e != null) 
                m_isEdit = !m_isEdit;  

            if (m_isEdit)
            {
                gridManager.AllowUserToDeleteRows = gridManager.AllowUserToAddRows = true;
                gridManager.ReadOnly = false;
                colNo.ReadOnly = true;

                btnEdit.ImageNormal = imgCheckBoxChecked;
                btnEdit.ImageMouseOver = imgCheckBoxChecked;
            }
            else
            {
                gridManager.AllowUserToDeleteRows = gridManager.AllowUserToAddRows = false;
                gridManager.ReadOnly = true;

                btnEdit.ImageNormal = imgCheckBoxUnChecked;
                btnEdit.ImageMouseOver = imgCheckBoxUnChecked;
            }
        }

        private void labelMiddle_Click(object sender, EventArgs e)
        {
            btnEdit_Click(btnEdit, null);
        } 

		private string GetPhoneNumber(string str)
		{
			string strPhoneNumber = "";
			int nLen = str.Length;

			for (int i = 0; i < nLen; i++)
			{
				char ch = str.ElementAt(i);

				if (ch >= '0' && ch <= '9')
					strPhoneNumber += ch;
			}

			return strPhoneNumber;
		}

		public void FormManager_Simulation_FormClosing(object sender, FormClosingEventArgs e)
		{
			m_dicManagerPhoneNumbers.Clear();

			foreach (DataGridViewRow row in gridManager.Rows)
			{
				if (row.IsNewRow)
					continue;

				string strManagerName = row.Cells[1].EditedFormattedValue.ToString();
				string strPhoneNumber = row.Cells[2].EditedFormattedValue.ToString();
				strManagerName = strManagerName.Trim();

				if (strManagerName.Length == 0)
					continue;

				strPhoneNumber = GetPhoneNumber(strPhoneNumber);

				if (strPhoneNumber.Length == 0)
					continue;

				//string strManagerName = row.Cells[1].Value.ToString();
				//string strPhoneNumber = GetPhoneNumber(row.Cells[2].Value.ToString());

				m_dicManagerPhoneNumbers[strManagerName] = strPhoneNumber;
			}

			WriteConfig();
		}

		private void gridManager_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
		{
			int nRowCount = gridManager.Rows.Count;
			if (nRowCount <= 1)
				return;

			DataGridViewRow row = gridManager.Rows[nRowCount - 2];
			row.Cells[0].Value = nRowCount - 1;
			row.Cells[0].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
		}

		private void gridManager_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
			{
				if (gridManager.CurrentRow == null)
					return;

				if (gridManager.CurrentRow.IsNewRow)
					return;

				gridManager.Rows.Remove(gridManager.CurrentRow);
			}
		}

		private void gridManager_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
		{
			int nRowCount = gridManager.Rows.Count;

			for (int i = e.RowIndex; i < nRowCount; i++)
			{
				DataGridViewRow row = gridManager.Rows[i];

				if (!row.IsNewRow)
					row.Cells[0].Value = i + 1;
			}
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}  
	}
}