using System;
using System.Collections.Generic;
using System.Collections;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security;
using System.Globalization;
using System.Diagnostics;
using System.Security.Permissions;
using System.IO;


namespace SDMS
{
	public class ModuleManager
	{
		private static ModuleManager m_instance = null;
		public static ModuleManager Instance
		{
			get 
			{
				if (m_instance == null)
				{
					m_instance = new ModuleManager();					
				}
				return m_instance; 
			}
		}

		// 모듈이름, 폴더를 명시하면 로드됩니다.
		private ArrayList m_LoadModules = new ArrayList();

		// 폴더를 명시하면 dll을 검색하여 로드합니다.
		private ArrayList m_Add_Path = new ArrayList();

		private ModuleManager()
		{
			AddRelativePath(".");
			AddRelativePath("python");
			AddRelativePath("common");
			AddRelativePath("SOP");
            AddRelativePath("opencascade6.5.3");
			
			//AddModule("IronPython", "python");
			//AddModule("IronPython","python");
			//AddModule("IronPython.Modules", "python");
			//AddModule("Microsoft.Dynamic", "python");
			//AddModule("Microsoft.Scripting","python");
			//AddModule("Microsoft.Scripting.Metadata", "python");
		}
		
		public void AddAbsolutePath(string szPath)
		{
			m_Add_Path.Add(szPath);
		}

		public void AddRelativePath(string szPath)
		{
			string strPath = Application.ExecutablePath;
			string szParentPath = Path.GetDirectoryName(strPath);
			string szTempPath = szParentPath + "\\" + szPath;
			m_Add_Path.Add(szPath);
		}

		public void AddModule(string szModuleName, string szSubPath)
		{

		}

		public void RegisterModules()
		{
			 bool bAddedPath = AddPath();
			if (bAddedPath == true)
			{
				// Application.Restart();
			}
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolve);
		}

		public Assembly AssemblyResolve(object sender, ResolveEventArgs args)
		{
			string strPath = Application.ExecutablePath;
			string szPath = Path.GetDirectoryName(strPath);
			string name = args.Name.Substring(0, args.Name.IndexOf(','));

			if (m_LoadModules.Count > 0)
			{
				for (int i = 0; i < m_LoadModules.Count; i += 2)
				{
					if (name.Equals(m_LoadModules[i]))
					{
						Debug.WriteLine("File Load : " + szPath + "\\" + m_LoadModules[i + 1] + "\\" + name + ".dll");
						return Assembly.LoadFile(szPath + "\\" + m_LoadModules[i + 1] + "\\" + name + ".dll");
					}
				}
			}
			for (int i = 0; i < m_Add_Path.Count; i++)
			{
				string szFileName = szPath + "\\" + m_Add_Path[i] + "\\" + name + ".dll";
				if (File.Exists(szFileName))
				{
					Debug.WriteLine("File Load : " + szFileName);
					return Assembly.LoadFile(szFileName);
				}
			}
			return null;
		}

		private bool AddPath()
		{
			bool bAddedPath = false;
			string szPath = Environment.GetEnvironmentVariable("Path");

			for (int i = 0; i < m_Add_Path.Count; i++)
			{
				string szFileName = "..\\" + m_Add_Path[i] + ";";
				if (!szPath.Contains(szFileName))
				{
					if (szPath.Length == 0)
						szPath += szFileName;
					else
					{
						char c = szPath[szPath.Length - 1];
						if (c == ';')
						{
							szPath += szFileName;
						}
						else
						{
							szPath += ";";
							szPath += szFileName;
						}
					}
					bAddedPath = true;
				}
			}

			if (bAddedPath == true)
			{
				Environment.SetEnvironmentVariable("Path", szPath, EnvironmentVariableTarget.User);
			}
			return bAddedPath;
		}		
	}
}
