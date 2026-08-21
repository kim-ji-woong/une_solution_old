using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using System.Drawing;

namespace RoadMan
{
	public class ObjectXMLManager : IDisposable
	{
		private string m_szVersion = "";

		public string VersionName
		{
			get { return m_szVersion; }
			set { m_szVersion = value; }
		}

		private string m_szErrorMessage = "";
		public string ErrorMessage
		{
			get { return m_szErrorMessage; }
			set { m_szErrorMessage = value; }
		}

		public void Dispose()
		{

		}
		private ProcessResult FindResult(string szHash)
		{
			if (!m_arResultData.ContainsKey(szHash))
				return null;
			return m_arResultData[szHash];

		}

		private ScheduleProperty FindProperty(ProcessSchedule sc, ResultProperty prop)
		{
			foreach (ScheduleProperty property in sc.Properties)
			{
				if( prop.SchedulePropertyHash == property.HashXML.ToString())
				{
					return property;
				}				
			}
			return null;

		}


		private bool LinkObject(ProcessSchedule sc, ProcessResult prop)
		{
			prop.ProcessSchedule = sc;

			foreach(ResultProperty property in prop.ResultProperties)
			{
				ScheduleProperty prop2 = FindProperty(sc, property);
				if( prop2 != null)
				{
					property.ScheduleProperty = prop2;
				}
			}
			return true;
		}



		private void UpdateData()
		{

			UndoRedoObjectManager.Instance.DicResults.Clear();
			UndoRedoObjectManager.Instance.DicSchedules.Clear();

			PanelDXFViewer pane = FormMain.Instance.CurrentPanel;

			pane.ProcessScheduleForm.ClearProcessSchedule();
			pane.ProcessResultForm.ClearProcessResult();
			
			foreach (ProcessSchedule schedule in m_arScheduleData)
			{
				if (schedule.PanelName == pane.GetHashCode().ToString())
				{
					string nHash = schedule.HashXML.ToString();
					ProcessResult result = FindResult(nHash);
					if (result != null)
					{

						LinkObject(schedule, result);

						pane.ProcessScheduleForm.AddRowWidthFullData(schedule, result);

						UndoRedoObjectManager.Instance.AddUndoRedoDataForRegister(schedule);
						UndoRedoObjectManager.Instance.AddUndoRedoDataForRegister(result);
					}

				}
			}		
		}

		public bool Load(System.IO.Stream stream)
		{
			XmlTextReader reader = InitReader(stream);
			return Load(reader);
		}

		private XmlTextReader InitReader(System.IO.Stream strem)
		{

			XmlTextReader reader = null;

			try
			{
				reader = new XmlTextReader(strem);

				while (reader.Read())
				{
					switch (reader.NodeType)
					{
						case XmlNodeType.Element:
							if (string.Compare(reader.Name, "RoadMan", true) != 0)
							{
								m_szErrorMessage = "RoadMan XML이 아닙니다.";
								reader.Close();
								return null;
							}

							return reader;

						
					}
				}
			}
			catch (Exception e)
			{
				m_szErrorMessage = e.Message;
				reader.Close();
				return null;
			}

			reader.Close();
			return reader;
		}
		

		private void SaveXML(XmlTextWriter writer, ProcessSchedule element)
		{
			Type[] types = { typeof(VariousData<DateTime>), typeof(VariousData<string>), typeof(VariousData<float>), typeof(VariousData<int>), typeof(ProcessResult), typeof(ScheduleProperty), typeof(List<ScheduleProperty>), typeof(ImportanceData), typeof(LandAddressData) };
			XmlSerializer sz = new XmlSerializer(element.GetType(), types);
			sz.Serialize(writer, element);			
		}

		private void SaveXML(XmlTextWriter writer, ProcessResult element)
		{			
			Type[] types = { typeof(UndoRedoData), typeof(VariousData<DateTime>), typeof(VariousData<string>), typeof(VariousData<float>), typeof(VariousData<int>), typeof(ProcessResult), typeof(ScheduleProperty), typeof(List<ScheduleProperty>), typeof(ImportanceData), typeof(LandAddressData) };
			XmlSerializer sz = new XmlSerializer(element.GetType(), types);
			sz.Serialize(writer, element);			
		}

		private string ObjectToXml(object output)
		{
			string objectAsXmlString;

			XmlSerializer xs = new XmlSerializer(output.GetType());	
			using (System.IO.StringWriter sw = new System.IO.StringWriter())
			{
				try
				{
					xs.Serialize(sw, output);
					objectAsXmlString = sw.ToString();
				}
				catch (Exception ex)
				{
					objectAsXmlString = ex.ToString();
				}
			}

			return objectAsXmlString;
		}

		private XmlTextWriter InitWriter(System.IO.Stream stream)
		{			
            XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);

            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument();

            return writer;
		}

		private TabPage FindTabPage(string szName)
		{
			int nTabs = FormMain.Instance.GetTabPageCount();
			for (int i = 0; i < nTabs; i++)
			{
				TabPage page = FormMain.Instance.GetTabPage(i);
				if (page.Text == szName)
					return page;
			}
			return null;
		}
			
            

		public bool Save(UndoRedoObjectManager manager, Stream st, string szVersionName)
		{
			XmlTextWriter writer = InitWriter(st);

			manager.Freeze = true;

			bool bResult = false;
			writer.WriteStartElement("RoadMan");
			writer.WriteStartElement("Version");
			writer.WriteString(szVersionName);
			writer.WriteEndElement();
			
			try
			{
				List<ProcessSchedule> arList = manager.DicSchedules.Values.ToList<ProcessSchedule>();
				foreach (ProcessSchedule element in arList)
				{
					writer.WriteStartElement("Element");
					SaveXML(writer, element);
					writer.WriteEndElement();
				}

				List<ProcessResult> arList2 = manager.DicResults.Values.ToList<ProcessResult>();
				foreach (ProcessResult element in arList2)
				{
					writer.WriteStartElement("Element");
					SaveXML(writer, element);
					writer.WriteEndElement();
				}

				int nTabs = FormMain.Instance.GetTabPageCount();
				for (int i = 0; i < nTabs; i++ )
				{
					TabPage page = FormMain.Instance.GetTabPage(i);
					PanelDXFViewer pane = (PanelDXFViewer)page.Tag;

					UnE.Overlay.OverlayPanel panel = pane.OverlayPanel;
					if (panel != null)
					{
						writer.WriteStartElement("Overlay");
						writer.WriteAttributeString("name", page.Text);

						writer.WriteStartAttribute("visible");
						writer.WriteString(panel.VisibleOverlay.ToString());
						writer.WriteEndAttribute();

						writer.WriteStartElement("TextColor");
						writer.WriteString(string.Format("{0}", panel.TextColor.ToArgb()));
						writer.WriteEndElement();

						writer.WriteStartElement("LineColor");
						writer.WriteString(string.Format("{0}", panel.LineColor.ToArgb()));
						writer.WriteEndElement();

						writer.WriteStartElement("OvElements");
						ArrayList arOvList = panel.EntityList;
						foreach (UnE.Overlay.OverlayElement element in arOvList)
						{
							writer.WriteStartElement("OvElement");
							element.SaveXML(writer);
							writer.WriteEndElement();
						}
						writer.WriteEndElement();
						writer.WriteEndElement();
					}
				}			
				bResult = true;
			}
			catch(Exception ex)
			{
				System.Diagnostics.Trace.WriteLine(ex.Message);
				System.Diagnostics.Trace.WriteLine(ex.StackTrace);

			}
			
			writer.WriteEndElement();
			writer.WriteEndDocument();
			writer.Close();

			manager.Freeze = false;

			return bResult;
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

		private bool ReadInt(XmlTextReader reader, ref int nData, string strMessage1, string strMessage2)
		{
			string strText = "";

			if (!ReadElementText(reader, ref strText))
			{
				m_szErrorMessage = string.Format("Line Number {0}, {1} 비어있습니다.", reader.LineNumber, strMessage1);
				return false;
			}

			if (!int.TryParse(strText, out nData))
			{
				m_szErrorMessage = string.Format("Line Number {0}, {1} 정수 형태의 숫자이어야만 합니다.", reader.LineNumber, strMessage2);
				return false;
			}

			return true;
		}

		private bool ReadOverlay(XmlTextReader reader, System.Windows.Forms.TabPage page)
		{
			if (page == null)
				return false;
			bool stop = false;

			PanelDXFViewer panel = (PanelDXFViewer)page.Tag;
			UnE.Overlay.OverlayPanel ovPanel = panel.OverlayPanel;
			try
			{
				while (reader.MoveToNextAttribute())
				{
					if (string.Compare(reader.Name, "visible", true) == 0)
					{
						bool visible;

						if (!bool.TryParse(reader.Value.ToString(), out visible))
							return false;
						else
							ovPanel.VisibleOverlay = visible;
					}
				}

				while (reader.Read())
				{
					switch (reader.NodeType)
					{
						case XmlNodeType.Element:
							if (string.Compare(reader.Name, "LineColor", true) == 0)
							{
								string szMsg1 = "LineColor";
								string szMsg2 = "LineColor";
								int nColor = 0;
								if (!ReadInt(reader, ref nColor, szMsg1, szMsg2))
									return false;

								ovPanel.LineColor = Color.FromArgb(nColor);
							}
							else if (string.Compare(reader.Name, "TextColor", true) == 0)
							{
								string szMsg1 = "TextColor";
								string szMsg2 = "TextColor";
								int nColor = 0;
								if (!ReadInt(reader, ref nColor, szMsg1, szMsg2))
									return false;

								ovPanel.TextColor = Color.FromArgb(nColor);
							}
							else if (string.Compare(reader.Name, "OvElements", true) == 0)
							{
								ArrayList arElements = ReadOverlayElements(reader);

								ovPanel.EntityList.Clear();
								if (arElements != null)
								{
									ovPanel.EntityList.AddRange(arElements);
									ovPanel.Invalidate();
								}
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
			catch (Exception e)
			{
				m_szErrorMessage = e.Message;
				return false;
			}
			return true;
		}

		private ArrayList ReadOverlayElements(XmlTextReader reader)
		{
			if (reader.IsEmptyElement)
				return null;

			ArrayList arResult = new ArrayList();
			bool stop = false;
			try
			{
				while (reader.Read())
				{
					switch (reader.NodeType)
					{
						case XmlNodeType.Element:
							if (string.Compare(reader.Name, "OvElement", true) == 0)
							{
								string szText = reader.ReadInnerXml();
								if (szText == "")
									continue;

								object obj = UnE.Overlay.OverlayFactory.Deserialize(szText);
								if (obj != null)
								{
									if (obj is UnE.Overlay.OverlayElement)
									{
										UnE.Overlay.OverlayElement element = (UnE.Overlay.OverlayElement)obj;
										element.OnPostXMLRead();
									}

									arResult.Add(obj);
								}
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
			catch (Exception e)
			{
				m_szErrorMessage = e.Message;
			}

			return arResult;
		}


		private ArrayList m_arScheduleData = new ArrayList();
		private Dictionary<string, ProcessResult> m_arResultData = new Dictionary<string, ProcessResult>();


		private bool ReadElement(XmlTextReader reader)
		{
			string szText = reader.ReadInnerXml();
			if (szText == "")
				return true;

			bool result = false;			
			try
			{
				
				object obj = ProcessObjectFactory.Deserialize(szText);
				if (obj != null)
				{
					if (obj is ProcessSchedule)
					{
						m_arScheduleData.Add(obj);
					}
					else if(obj is ProcessResult)
					{
						ProcessResult pr = (ProcessResult)obj;
						
						m_arResultData.Add(pr.ScheduleHash, pr);
					}	
				

				}
				result = true;
			}
			catch(Exception e)
			{
				m_szErrorMessage = e.Message;				
			}
			return result;
		}


		public bool Load(XmlTextReader reader)
		{
			if (reader == null)
				return false;

			bool stop = false;
			m_arResultData.Clear();
			m_arScheduleData.Clear();
			try
			{
				while (reader.Read())
				{
					switch (reader.NodeType)
					{
						case XmlNodeType.Element:
							if (string.Compare(reader.Name, "Element", true) == 0)
							{
								if (!ReadElement(reader))
									return false;
							}
							else if (string.Compare(reader.Name, "Version", true) == 0)
							{
								m_szVersion = reader.ReadString();
							}
							else if (string.Compare(reader.Name, "Overlay", true) == 0)
							{
								string szName = reader.GetAttribute("name");
								TabPage page = FindTabPage(szName);
								if (page != null)
								{
									if (!ReadOverlay(reader, page))
										return false;

									PanelDXFViewer viewer = (PanelDXFViewer)page.Tag;
									if( viewer != null)
									{
										viewer.DXFControl.Invalidate();
									}
								}
								
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
			catch (Exception e)
			{
				m_szErrorMessage = e.Message;
				reader.Close();
				return false;
			}

			reader.Close();

			UpdateData();

			return true;
		}

		private void PassElement(XmlTextReader reader)
		{
			if (reader.IsEmptyElement)
				return;

			while (reader.Read())
			{
				switch (reader.NodeType)
				{
					case XmlNodeType.EndElement:
						return;
				}
			}
		}
	}

	public class ProcessObjectFactory
	{

		public static object Deserialize(string srXML)
		{
			try
			{
				string szText = srXML.Substring(2);
				szText = szText.Trim();
				object result = null;

				Type type = typeof(object);
				if (szText.IndexOf("ProcessSchedule") != -1)
				{
					type = typeof(ProcessSchedule);
				}
				else if (szText.IndexOf("ProcessResult") != -1)
				{
					type = typeof(ProcessResult);
				}

				Type[] types = { typeof(UndoRedoData), typeof(VariousData<DateTime>), typeof(VariousData<string>), typeof(VariousData<float>), typeof(VariousData<int>), typeof(ProcessResult), typeof(ScheduleProperty), typeof(List<ScheduleProperty>), typeof(ImportanceData), typeof(LandAddressData) };

				XmlReader reader = XmlReader.Create(new StringReader(szText));
				XmlSerializer sz = new XmlSerializer(type, types);
				result = sz.Deserialize(reader);
				return result;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Trace.WriteLine(ex.StackTrace);
			}
			return null;

		}
	}
}
