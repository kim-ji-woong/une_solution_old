 using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using DBUtility;
using UnE.Spatial;
using UnE.View.Content;

namespace SDMS
{
	internal class ModelManager
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

        private IFormContent m_ContentForm = null;

        public IFormContent TargetForm
		{
			get { return m_ContentForm; }
			set { m_ContentForm = value; }
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

		private ModelManager()
		{
		}

		public void Read3DModel(bool bAsynDownload = false)
		{
			WebDBManager dbMgr = FormMain.Instance.DBManager;

            int nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;

            string strSQL = "Select Name, URL, AccessedTime from BluePrint where SiteID = " + nSiteID.ToString();
			ArrayList arrTempResult = dbMgr.GetResultData(strSQL, 0);

			if (bAsynDownload == true)
			{
				System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Read3DModel));
				t.Start(arrTempResult);
			}
			else
			{
				Read3DModel(arrTempResult);
			}
		}

		private void Read3DModel(object param)
		{
			ArrayList arrResult = (ArrayList)param;
			WebDBManager dbMgr = FormMain.Instance.DBManager;

			if (arrResult == null || arrResult.Count == 0)
			{
				FormMain.Instance.Invoke((MethodInvoker)delegate
				{
					MessageBox.Show("3D Model 파일을 받아올 수 없습니다.\r\n네트웍 상태가 올바른지 확인해 주세요", "File Download Error");
				});
				Application.Exit();
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
				FormMain.Instance.Invoke((MethodInvoker)delegate
				{
					MessageBox.Show("3D 모델 파일을 받아올 수 없습니다.\r\n네트웍 상태가 올바른지 확인해 주세요", "File Download Error");
				});

				Application.Exit();
				return;
			}

			if (m_ContentForm != null)
				m_ContentForm.SetFilePath(System.IO.Path.GetTempPath(), m_strOutside3DModel, strNULL, m_dicInside3DModel);
		}

		private string Download3DModelFile(Dictionary<string, string> dic3DModelHistory, string strTag, string strShortTime, WebClient web, string strURL, Dictionary<string, string> dic3DModel, string strPath)
		{
			string tempPath = System.IO.Path.GetTempPath();
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

			if (File.Exists(localPath))
			{
				File.Delete(localPath);
			}

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