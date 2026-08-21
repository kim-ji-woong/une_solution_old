using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;


namespace RoadMan
{

	public interface IUndoRedoData
	{
		int Hash
		{
			get;
			set;
		}
	}

	public interface IUndoRedoManager
	{	

		void AddUndoRedoData(IUndoRedoData data);
		void RemoveUndoRedoData(IUndoRedoData data);
	}

	public class UndoRedoData : IDisposable, IUndoRedoData, INotifyPropertyChanged, INotifyPropertyChanging
	{
		private int m_nHashValue = -1;


		protected IUndoRedoManager mUndoRedoManager = null;
		
		public event PropertyChangedEventHandler PropertyChanged;
		public event PropertyChangingEventHandler PropertyChanging;
		
		public UndoRedoData()
		{
			try
			{
				mUndoRedoManager = UndoRedoObjectManager.Instance;
				if(mUndoRedoManager != null)
				{
					mUndoRedoManager.AddUndoRedoData(this); 
				}
			}
			catch(Exception)
			{

			}			
		}

		public virtual void Dispose()
		{
			if (mUndoRedoManager != null)
			{
				mUndoRedoManager.RemoveUndoRedoData(this);
			}
		}
		
		[XmlIgnore]
		public int HashXML
		{
			get { return m_nHashValue; }			
		}

		public int Hash
		{
			get
			{
				return GetHashCode();
			}
			set
			{
				m_nHashValue = value;
			}
		}

		public void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChangedEventArgs args =
					new PropertyChangedEventArgs(propertyName);
				PropertyChanged(this, args);
			}
		}

		public void OnPropertyChanging(string propertyName)
		{
			if (PropertyChanging != null)
			{
				PropertyChangingEventArgs args =
					new PropertyChangingEventArgs(propertyName);
				PropertyChanging(this, args);
			}
		}


		private PanelDXFViewer m_ParentPane = null;
		[XmlIgnore]
		public PanelDXFViewer ParentPane
		{
			get { return m_ParentPane; }
			set { m_ParentPane = value; }
		}


		private string m_szPanelName = "";
		public string PanelName
		{
			get { return m_szPanelName; }
			set { m_szPanelName = value; }
		}


	}
}
