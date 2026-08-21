using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Net;
using DBUtility2;

namespace UnE
{
	namespace SOP
	{
		/// <summary>
		/// 모델파일이 다운로드 된경우 호출되는 Interface의 함수
		/// 인터페이스를 구현한 클래스의 Object를 TargetForm에 등록하면 SetFilePath가 호출된다.
		/// </summary>
		public interface IModelDownload
		{
			void SetFilePath(string strFolderPath, string strOutsideFilePath, string strInsideFilePath, Dictionary<string, string> dicInside);
		}


		/// <summary>
		/// 서버의 BluePrint를 검사하여 해당 항목의 파일을 다운로드
		/// </summary>
		public class ModelManager
		{
			private static ModelManager m_Instance = null;
			public static ModelManager Instance
			{
				get
				{
					if (m_Instance == null)
						m_Instance = new ModelManager();
					return m_Instance;
				}
			}

			private bool m_bExtractOutside = false;
			public bool ExtractOutside
			{
				get { return m_bExtractOutside; }
				set { m_bExtractOutside = value; }
			}
			private bool m_bExtraceInside = false;
			public bool ExtractInside
			{
				get { return m_bExtraceInside; }
				set { m_bExtraceInside = value; }
			}

			private string m_szDownloadPath = "";
			public string DownloadPath
			{
				get { return m_szDownloadPath; }
				set { m_szDownloadPath = value; }
			}

			private IModelDownload m_ContentForm = null;
			public IModelDownload TargetForm
			{
				get { return m_ContentForm; }
				set { m_ContentForm = value; }
			}

			private WebDBManager m_dbMgr = null;
			public DBUtility2.WebDBManager Mgr
			{
				get { return m_dbMgr; }
				set { m_dbMgr = value; }
			}

			private Dictionary<string, string> m_dicInside3DModel = new Dictionary<string, string>();
			public Dictionary<string, string> Inside3DModel
			{
				get { return m_dicInside3DModel; }
			}

			private string m_strOutside3DModel = "";
			public string Outside3DModel
			{
				get { return m_strOutside3DModel; }
			}

			private string m_szLastErrorMsg = "";
			public string LastErrorMsg
			{
				get { return m_szLastErrorMsg; }
				set { m_szLastErrorMsg = value; }
			}

            private int m_SiteID = 1;
			private ModelManager()
			{
                m_SiteID = UnE.SOP.ProxySOP.Instance.SiteID;

				m_szDownloadPath = System.IO.Path.GetTempPath();
			}

            private string m_szLocalFilePath = "";
            public string LocalFilePath
            {
                get { return m_szLocalFilePath; }
                set { m_szLocalFilePath = value; }
            }


			public bool Read3DModel()
			{
                if( m_dbMgr == null)
                    m_dbMgr = ProxySOP.Instance.DBManager;

				if (m_dbMgr == null || TargetForm == null)
					return false;

				WebDBManager dbMgr = m_dbMgr;

                string strSQL = "Select Name, URL, AccessedTime from BluePrint where SiteID = " + m_SiteID.ToString();
				ArrayList arrTempResult = dbMgr.GetResultData(strSQL);

				System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Read3DModel));
				t.Start(arrTempResult);

				return true;
			}

			private void Read3DModel(object param)
			{
				ArrayList arrResult = (ArrayList)param;
				WebDBManager dbMgr = m_dbMgr;

				if (arrResult == null || arrResult.Count == 0)
				{
					m_szLastErrorMsg = "3D Model 파일을 받아올 수 없습니다.\r\n네트웍 상태가 올바른지 확인해 주세요";
					return;
				}

				WebClient web = new WebClient();
				string strNULL = null;

				Dictionary<string, string> dic3DModelHistory = new Dictionary<string, string>();
				Read3DModelHistory(ref dic3DModelHistory);

				DateTime dtDefault = new DateTime();

				for (int i = 0; i < arrResult.Count - 2; i += 3)
				{
					string strName = WebDBManager.GetStringField(arrResult[i].ToString(), "");
					string strURL = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");
					DateTime dtAccessed = WebDBManager.GetDateTimeField(arrResult[i + 2], dtDefault);

					string strShortTime = dtAccessed.ToShortDateString() + " " + dtAccessed.ToShortTimeString();

					if (!strURL.Contains("http:"))
						continue;

					if (strName == "All")
					{
						m_strOutside3DModel = Download3DModelFile(dic3DModelHistory, "Outside", strShortTime, web, strURL, null, m_strOutside3DModel);
					}
					else if (strName == "Inside")
					{
						strNULL = Download3DModelFile(dic3DModelHistory, "Inside", strShortTime, web, strURL, m_dicInside3DModel, strNULL);
					}
				}

				if (m_strOutside3DModel.Length == 0 || m_dicInside3DModel.Count == 0)
				{
					m_szLastErrorMsg = "3D 모델 파일을 받아올 수 없습니다.\r\n네트웍 상태가 올바른지 확인해 주세요";
					return;
				}

				if (m_ContentForm != null)
					m_ContentForm.SetFilePath(System.IO.Path.GetTempPath(), m_strOutside3DModel, strNULL, m_dicInside3DModel);
			}           

			private string Download3DModelFile(Dictionary<string, string> dic3DModelHistory, string strTag, string strShortTime, WebClient web, string strURL, Dictionary<string, string> dic3DModel, string strPath)
			{
				
				string tempPath = m_szDownloadPath;
				string localPath = tempPath + strTag + ".zip";

				if (dic3DModelHistory.ContainsKey(strTag) && dic3DModelHistory[strTag] == strShortTime)
				{
					if (System.IO.File.Exists(localPath))
					{
						if (dic3DModel == null)
							strPath = localPath;
						else
							dic3DModel[strTag] = localPath;

						return strPath;
					}
				}

				if (strTag == "Outside")
					m_bExtractOutside = true;
				else
					m_bExtraceInside = true;

				web.DownloadFile(strURL, localPath);

				if (dic3DModel == null)
					strPath = localPath;
				else
				{
					strPath = localPath;
					dic3DModel[strTag] = localPath;
				}

				System.IO.StreamWriter sw = new System.IO.StreamWriter(tempPath + strTag + ".log", false, Encoding.Default);
				sw.WriteLine(strShortTime);
				sw.Close();

				return strPath;
			}

			private void Read3DModelHistory(ref Dictionary<string, string> dic3DModelHistory)
			{
				string tempPath = System.IO.Path.GetTempPath();

				System.IO.StreamReader reader = null;

				try
				{
					reader = new System.IO.StreamReader(tempPath + "Outside.log", Encoding.Default);
					string strOutsideTime = reader.ReadLine();
					reader.Close();

					dic3DModelHistory["Outside"] = strOutsideTime;
				}
				catch (System.IO.FileNotFoundException)
				{
				}

				try
				{
					reader = new System.IO.StreamReader(tempPath + "Inside.log", Encoding.Default);
					string strInsideTime = reader.ReadLine();
					reader.Close();

					dic3DModelHistory["Inside"] = strInsideTime;
				}
				catch (System.IO.FileNotFoundException)
				{
				}
			}
		}

		
	}
}
