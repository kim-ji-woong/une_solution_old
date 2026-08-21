using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOPManager
{
	public class SopDocManager
	{
		private static SopDocManager m_Instance = null;
		internal static SopDocManager Instance
		{
			get 
			{
				if (m_Instance == null)
					m_Instance = new SopDocManager();
				return m_Instance;
			}
		}

		// return 값이 true : 평일 false : 휴일 및 야간
		private bool m_bWeekMode = true;
		public bool WeekMode
		{
			get { return m_bWeekMode; }
			set { m_bWeekMode = value; }
		}
		

		// Return 값 : true이면 등록모드, false이면 미등록모드
		private bool m_bRegular = true;
		public bool RegularMode
		{
			get { return m_bRegular; }
			set { m_bRegular = value; }
		}	


		private bool m_bDBLoad = false;
		public bool UseDB
		{
			get { return m_bDBLoad; }
			set 
			{
				m_bDBLoad = value;
				m_bXMLLoad = !value;
			}
		}

		private bool m_bXMLLoad = false;
		public bool UseXML
		{
			get { return m_bXMLLoad; }
			set
			{ 
				m_bXMLLoad = value;
				m_bDBLoad = !value;
			}
		}

		private bool m_bNewSOP = true;
		public bool IsNewSOP
		{
			get { return m_bNewSOP; }
			set { m_bNewSOP = value; }
		}


		private string m_szFilePath = "";
		public string FilePath
		{
			get { return m_szFilePath; }
			set { m_szFilePath = value; }
		}
		
		private string m_szSopName = "";
		public string SopName
		{
			get { return m_szSopName; }
			set { m_szSopName = value; }
		}

		private string m_szCategory = "";
		public string CategoryName
		{
			get { return m_szCategory; }
			set { m_szCategory = value; }
		}

		private string m_szSubCategory = "";
		public string SubCategoryName
		{
			get { return m_szSubCategory; }
			set { m_szSubCategory = value; }
		}

		private string m_szDisaster = "";
		public string DisasterName
		{
			get { return m_szDisaster; }
			set { m_szDisaster = value; }
		}

		private string m_szDescDisaster = "";
		public string DisasterDescription
		{
			get { return m_szDescDisaster; }
			set { m_szDescDisaster = value; }
		}


		public string GetLevelPath()
        {          
            //string strLevelName = FormMain.Instance.GetPageLevel().GetTabPageName();
            return m_szCategory + "/" + m_szCategory + "/" + m_szDisaster;
        }

		private SopDocManager()
		{

		}

		public void InitMode()
		{
			m_bXMLLoad = false;
			m_bDBLoad = false;
			m_bNewSOP = true;
			m_szFilePath = "";
			m_szSopName = "";

		}

		public void Save()
		{

		}

		public void SaveAs()
		{

		}

		public void Open()
		{
		}

		public void OpenDB()
		{

		}

		public void SaveDB()
		{

		}

		public void SaveAsDB()
		{

		}


		
	}
}
