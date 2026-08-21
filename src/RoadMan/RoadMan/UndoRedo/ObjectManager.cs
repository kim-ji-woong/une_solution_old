using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;


namespace RoadMan
{
	public class UndoRedoObjectManager : IUndoRedoManager
	{
		private static UndoRedoObjectManager m_instance = null;
		public static UndoRedoObjectManager Instance
		{
			get 
			{
				if (m_instance == null)
					m_instance = new UndoRedoObjectManager();

				return m_instance; 
			}		
		}

		private Dictionary<int, ProcessSchedule> m_dicSchedules = new Dictionary<int, ProcessSchedule>();		
		public Dictionary<int, ProcessSchedule> DicSchedules
		{
			get { return m_dicSchedules; }
			set { m_dicSchedules = value; }
		}

		private Dictionary<int, ProcessResult> m_dicResults = new Dictionary<int, ProcessResult>();
		public Dictionary<int, ProcessResult> DicResults
		{
			get { return m_dicResults; }
			set { m_dicResults = value; }
		}


		private bool m_bFreeze = false;
		public bool Freeze
		{
			get { return m_bFreeze; }
			set { m_bFreeze = value; }
		}

		private ArrayList m_arDataList = new ArrayList();

		public void AddUndoRedoData(IUndoRedoData data)
		{
			//if (m_bFreeze == true)
			//	return;

			//if (m_bWatchObject == true)
			//{
			//	UndoRedoManager.Instance.SaveSnapshot("Add");
			//}

			//int nHash = data.GetHashCode();
			//if (typeof(ProcessSchedule).IsAssignableFrom(data.GetType()))
			//{
			//	if (!m_dicSchedules.ContainsKey(nHash))
			//	{
			//		m_dicSchedules.Add(nHash, (ProcessSchedule)data);
			//	}
			//}
			//else if (typeof(ProcessResult).IsAssignableFrom(data.GetType()))
			//{
			//	if (!m_dicResults.ContainsKey(nHash))
			//	{
			//		m_dicResults.Add(nHash, (ProcessResult)data);
			//	}
			//}

			//UndoRedoData datas = (UndoRedoData)data;
			//if (datas != null)
			//	m_arDataList.Add(datas);
		}

		public void RegisterObject(IUndoRedoData data)
		{
			//UndoRedoData datas = (UndoRedoData)data;
			//datas.PropertyChanged += OnChangeProperty;
			//datas.PropertyChanging += OnChangingProperty;
		}

		public void AddUndoRedoDataForRegister(IUndoRedoData data)
		{
			int nHash = data.GetHashCode();
			if (typeof(ProcessSchedule).IsAssignableFrom(data.GetType()))
			{
				if (!m_dicSchedules.ContainsKey(nHash))
				{
					m_dicSchedules.Add(nHash, (ProcessSchedule)data);
				}
			}
			else if (typeof(ProcessResult).IsAssignableFrom(data.GetType()))
			{
				if (!m_dicResults.ContainsKey(nHash))
				{
					m_dicResults.Add(nHash, (ProcessResult)data);
				}
			}
		}

		private ProcessSchedule m_TempSchedule = null;
		public ProcessSchedule TempSchedule
		{
			get
			{
				ProcessSchedule temp = m_TempSchedule;
				m_TempSchedule = null;
				return temp;
			}
			set
			{
				m_TempSchedule = value;
			}
		}


		private ProcessResult m_TempResult = null;
		public ProcessResult TempResult
		{
			get 
			{
				ProcessResult temp = m_TempResult;
				m_TempResult = null;
				return temp; 
			}
			set 
			{
				m_TempResult = value; 
			}
		}
			

		public void RemoveUndoRedoDataForRegister(IUndoRedoData data)
		{
			
			int nHash = data.GetHashCode();
			if (typeof(ProcessSchedule).IsAssignableFrom(data.GetType()))
			{
				if (m_dicSchedules.ContainsKey(nHash))
				{
					m_dicSchedules.Remove(nHash);
				}
			}
			else if (typeof(ProcessResult).IsAssignableFrom(data.GetType()))
			{
				if (m_dicResults.ContainsKey(nHash))
				{
					m_dicResults.Remove(nHash);
				}
			}
		}

		public void RemoveUndoRedoData(IUndoRedoData data)
		{
			//if (m_bFreeze == true)
			//	return;

			//if( m_bWatchObject == true)
			//{
			//	UndoRedoManager.Instance.SaveSnapshot("delete");
			//}
			//int nHash = data.GetHashCode();
			//if (typeof(ProcessSchedule).IsAssignableFrom(data.GetType()))
			//{
			//	if (m_dicSchedules.ContainsKey(nHash))
			//	{
			//		m_dicSchedules.Remove(nHash);
			//	}
			//}
			//else if (typeof(ProcessResult).IsAssignableFrom(data.GetType()))
			//{
			//	if (m_dicResults.ContainsKey(nHash))
			//	{
			//		m_dicResults.Remove(nHash);
			//	}
			//}

			//UndoRedoData datas = (UndoRedoData)data;
			//if (datas != null)
			//	m_arDataList.Remove(datas);

			//datas.PropertyChanged -= OnChangeProperty;
			//datas.PropertyChanging -= OnChangingProperty;
			
		}


		private bool m_bWatchObject = false;
		public void WatchObjects()
		{
			m_bWatchObject = true;
			//foreach(UndoRedoData data in m_arDataList)
			//{
			//	if (data != null && data is ProcessSchedule)
			//	{
			//		data.PropertyChanged += OnChangeProperty;
			//		data.PropertyChanging += OnChangingProperty;
			//	}				
			//}
		}

		public void OnChangingProperty(object sender, PropertyChangingEventArgs args)
		{
			if (m_bFreeze == true)
				return;

			UndoRedoManager.Instance.SaveSnapshot(args.PropertyName);
		}

		public void OnChangeProperty(object sender, PropertyChangedEventArgs args)
		{			
		}

	}
}
