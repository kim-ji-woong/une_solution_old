using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;

namespace UnE.SenarioMaker
{
	public class UndoRedoManager
	{
		protected static UndoRedoManager m_Instance = null;
		public static UndoRedoManager Instance
		{
			get 
			{
				if (m_Instance == null)
					m_Instance = new UndoRedoManager();
				return m_Instance; 
			}			
		}
		/// <summary>
		/// Undo 큐
		/// </summary>
		protected Deque m_UndoQue = new Deque();
		/// <summary>
		/// Redo 큐 
		/// </summary>
		protected Deque m_RedoQue = new Deque();

		/// <summary>
		/// 현재 상태
		/// </summary>
		protected MemoryStream m_Current = null;

		/// <summary>
		/// 최대 Undo Count
		/// </summary>
		protected int m_nMaxRedoCount = 50;
		public int MaxUndoCount
		{
			get { return m_nMaxRedoCount; }
			set 
			{
				m_nMaxRedoCount = value;
				SetQueueLength();
			}
		}

		public UndoRedoManager()
		{
            
		}

		/// <summary>
		/// Max undo size만큼 deque의 길이를 설정, size보다 큰 요소는 삭제
		/// </summary>
		private void SetQueueLength()
		{
			if (m_UndoQue.Count >= MaxUndoCount)
			{
				m_UndoQue.PopFront();
			}
			if (m_RedoQue.Count >= MaxUndoCount)
			{
				m_RedoQue.PopFront();
			}
		}
        private string m_szLastSnapShotName = "";
		private void LoadState(MemoryStream stream, bool bRedo)
		{
			using(XMLManager xmlMananger = new XMLManager())
            {
                xmlMananger.CheckContent = false;

                using(MemoryStream st = new MemoryStream())
                {
                    byte[] buffer = stream.GetBuffer();
                    st.Write(buffer, 0, buffer.Length);
                    st.Position = 0;

                    bool bResult = xmlMananger.Load(st);

                    if(bResult == true)
                    {
                        string szSnapShotName = xmlMananger.VersionName;
                        m_szLastSnapShotName = szSnapShotName;

                        if (bRedo == true)
                        {
                            szSnapShotName = "다시실행 - " + szSnapShotName;
                        }
                        else
                            szSnapShotName = "되돌리기 - " + szSnapShotName;

                        string szCategory = SenarioManager.Instance.Category;
                        string szSubCategory = SenarioManager.Instance.SubCategory;
                        string szDisasterType = SenarioManager.Instance.DisasterType;
                        ArrayList ar = SenarioManager.Instance.ActionStepList;

                        foreach (ActionStep actionStep in ar)
                        {
                            actionStep.StepName = FormMain.Instance.ContentForm.SenarioTitle;
                        }

                        FormMain.Instance.TreeForm.SetTreeView(szCategory, szSubCategory, szDisasterType, ar);
                        FormMain.Instance.SetStatusText(szSnapShotName);
                    }
                }
            }            
		}

		/// <summary>		
		/// Undo / Redo 큐 초기화
		/// </summary>
		public void Reset()
		{
			m_UndoQue.Clear();
			m_RedoQue.Clear();
			m_Current = null;
		}


		/// <summary>
		/// 현재 상태를 저장하여 Undo 큐에 추가, 상태 변화 전에 호출한다. 
		/// </summary>
		private bool SaveXML(System.IO.Stream stream, string strVersion, out string szError)
		{
			szError = "";
            
            using(XMLManager xmlMananger = new XMLManager())
            {
                xmlMananger.CheckContent = false;

                SenarioManager manager = SenarioManager.Instance;
                if (!xmlMananger.Save(manager, stream, strVersion))
                {
                    szError = xmlMananger.ErrorMessage;
                    return false;
                }
            }            
			return true;
		}

		/// <summary>
		/// 현재 상태를 저장하여 Undo 큐에 추가, 상태 변화 전에 호출한다. 
		/// </summary>
		public void SaveSnapshot(string szSnapShotName, bool bRedoClear = true)
		{
			string szErrMsg = "";		
			
			// 현재 상태를 undo 큐에 저장
			MemoryStream inMemoryCopy = new MemoryStream();

            if (SaveXML(inMemoryCopy, szSnapShotName, out szErrMsg))
			{
				m_Current = inMemoryCopy;
				m_UndoQue.PushBack(m_Current);

				SetQueueLength();

                m_szLastSnapShotName = szSnapShotName;

				if (bRedoClear == true)
					m_RedoQue.Clear();
			}
		}

        private void SaveRedoSnapshot(string szSnapShotName)
		{
			string szErrMsg = "";

			// 현재 상태를 undo 큐에 저장
			MemoryStream inMemoryCopy = new MemoryStream();

			if (SaveXML(inMemoryCopy, szSnapShotName,  out szErrMsg))
			{
				m_Current = inMemoryCopy;
				m_RedoQue.PushBack(m_Current);

				SetQueueLength();
			}
		}

		/// <summary>
		/// Undo
		/// </summary>
		/// <returns>Undo가 성공한경우 true, 실패한경우 false</returns>
		public bool Undo()
		{
			if (m_UndoQue.Count == 0)
				return false;

			SaveRedoSnapshot(m_szLastSnapShotName);

			// Undo큐에서 한개 가져온다. ( 마지막 현재 상태 )
			object obj = m_UndoQue.PopBack();

			if (obj != null)
			{
				m_Current = (MemoryStream)obj;
				LoadState(m_Current, false);
				return true;
			}
			return false;
		}

		public bool Redo()
		{
			if (m_RedoQue.Count == 0)
				return false;

            SaveSnapshot(m_szLastSnapShotName, false);

			// Redo큐에서 한개 가져온다. ( 마지막 현재 상태 )
			object obj = m_RedoQue.PopBack();			
			
			SetQueueLength();

			if (obj != null)
			{
				m_Current = (MemoryStream)obj;
				LoadState(m_Current, true);
				return true;
			}			

			return false;
		}


		public int UndoCount
		{
			get { return m_UndoQue.Count; }
		}

		public int RedoCount
		{
			get { return m_RedoQue.Count; }
		}


		public void Dump()
		{
			int i = 0;
			foreach (MemoryStream stream in m_UndoQue)
			{
				try
				{
					string szFileName = Path.GetDirectoryName(Application.ExecutablePath) + "\\logs\\" + string.Format("QueueDump{0}.txt", i++);
					if (!Directory.Exists(Path.GetDirectoryName(Application.ExecutablePath) + "\\logs\\"))
					{
						Directory.CreateDirectory(Path.GetDirectoryName(Application.ExecutablePath) + "\\logs\\");
					}

					if (File.Exists(szFileName))
					{
						File.Delete(szFileName);
					}

					FileStream file = new FileStream(szFileName, FileMode.CreateNew , FileAccess.Write);
					
					MemoryStream st = new MemoryStream();
					byte[] buffer = stream.GetBuffer();
					st.Write(buffer, 0, buffer.Length);
					st.Position = 0;
					st.WriteTo(file);
					st.Close();
					file.Close();
				}
				catch (System.Exception e)
				{
					Debug.WriteLine(e.StackTrace);
				}
			}
		}
	}
}
