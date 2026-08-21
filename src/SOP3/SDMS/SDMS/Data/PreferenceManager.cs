using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDMS
{
	public class PreferenceManager
	{
		private static PreferenceManager m_instance = null;
		public static PreferenceManager Instance
		{
			get
			{
				if (m_instance == null)
				{
					m_instance = new PreferenceManager();
				}
				return m_instance;
			}
		}


		private PreferenceManager()
		{
		}

		private bool m_bRealMode = false;
		public bool RealMode
		{
			get { return m_bRealMode; }
			
			set { m_bRealMode = value; }
		}
	}
}
