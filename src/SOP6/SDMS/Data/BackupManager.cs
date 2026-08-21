using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using DBUtility2;

namespace SDMS
{
	internal class BackupManager
	{
		private WebDBManager m_dbMgr = null;//new WebDBManager();

		private string m_szBackupDir = "";

		public string BackupDir
		{
			get { return m_szBackupDir; }
			set { m_szBackupDir = value; }
		}

        private string m_szRestoreDB = "SOP3_RESTORE";
        public string RestoreDBName
        {
            get { return m_szRestoreDB; }
            set { m_szRestoreDB = value; }
        }

		private static BackupManager m_Instance = null;
		public static BackupManager Instance
		{
			get
			{
				if (m_Instance == null)
					m_Instance = new BackupManager();
				return m_Instance;
			}
		}

		private BackupManager()
		{
            //m_dbMgr = new WebDBManager(UnE.SOP.ProxySOP.Instance.SiteID);
            m_dbMgr = FormMain.Instance.DBManager;

            m_szRestoreDB = m_dbMgr.DatabaseName + "_RESOTRE";


			m_szBackupDir = Application.StartupPath + "\\DBBackUP";

			try
			{
				if (!Directory.Exists(m_szBackupDir))
					Directory.CreateDirectory(m_szBackupDir);
			}
			catch (System.Exception)
			{
			}
		}

		private void RestoreDatabase(string szSQLFile)
		{
			string[] lines = System.IO.File.ReadAllLines(szSQLFile, System.Text.Encoding.UTF8);
			//using (StreamReader sr = new StreamReader(szSQLFile, System.Text.Encoding.UTF8))

			string szDBName = "master";
			StringBuilder sb = new StringBuilder();
			bool bProcMode = false;
			foreach (string line in lines)
			{
				string newline = line.Trim();

				if (newline.StartsWith("USE ") || newline.StartsWith("use "))
				{
					szDBName = newline.Substring(4, newline.Length - 4);
				}

				if (newline.ToUpper().StartsWith("CREATE PROCEDURE"))
				{
					bProcMode = true;
				}

				if (newline == "" || newline.StartsWith("--"))
					continue;

				bool bEnd = false;
				if (newline.ToLower().Equals("go"))
				{
                    newline = newline.ToLower().Replace("go", ";");
					bEnd = true;
				}

                //if (newline.Equals("GO"))
                //{
                //    newline = newline.Replace("GO", ";");
                //    bEnd = true;
                //}

                //if (newline.Equals("Go"))
                //{
                //    newline = newline.Replace("Go", ";");
                //    bEnd = true;
                //}

                //if (newline.Equals("gO"))
                //{
                //    newline = newline.Replace("gO", ";");
                //    bEnd = true;
                //}

				if (bProcMode == false && newline.EndsWith(";"))
				{
					bEnd = true;
				}

				sb.Append(newline);

				if (bEnd == true)
				{
					if (bProcMode == true)
						bProcMode = false;

					string szQuery = sb.ToString();
					sb.Clear();
					if (szQuery.StartsWith("USE ") || szQuery.StartsWith("use "))
					{
						continue;
					}

					if (szQuery.Equals(";"))
						continue;

					ArrayList arrResult = m_dbMgr.GetResultData(szQuery, szDBName);
					//Debug.WriteLine(szQuery);
					if (arrResult == null)
						break;
				}
				else
				{
					sb.Append(' ');
				}
			}
		}

		// strFolderPath가 존재하면 해당 폴더의 파일 및 Sub 폴더를 모두 지운다.
		// strFolderPath가 존재하지 않으면 생성한다.
		private void CreateFolder(string strFolderPath)
		{
            if (Directory.Exists(strFolderPath))
            {
                string[] arrFiles = Directory.GetFiles(strFolderPath);

                foreach (string strFile in arrFiles)
                {
                    File.Delete(strFile);
                }

                string[] arrFolders = Directory.GetDirectories(strFolderPath);

                foreach (string strFolder in arrFolders)
                {
                    DeleteFolder(strFolder);
                }
            }
            else
            {
                DirectoryInfo info = Directory.CreateDirectory(strFolderPath);
                
            }
		}

		public static void DeleteFolder(string strFolderPath)
		{
			string[] arrFiles = Directory.GetFiles(strFolderPath);

			foreach (string strFile in arrFiles)
			{
				File.Delete(strFile);
			}

			string[] arrFolders = Directory.GetDirectories(strFolderPath);

			foreach (string strFolder in arrFolders)
			{
				DeleteFolder(strFolder);
			}

			Directory.Delete(strFolderPath);
		}

		public bool CompressFile(string szTargetFile, ArrayList arAchive)
		{
			// make sure there are files to zip
			if (arAchive.Count < 1 || szTargetFile == string.Empty)
			{
				return false;
			}

			FileInfo fi = new FileInfo(szTargetFile);
			if (fi.Exists)
			{
				try
				{
					File.Delete(szTargetFile);
				}
				catch
				{
					return false;
				}
			}

			string strTempFolderPath = System.IO.Path.GetTempPath() + "\\SOPSimulatorZipTemp";
			CreateFolder(strTempFolderPath);

			foreach (string file in arAchive)
			{
				int nDotIndex = file.LastIndexOf('\\');
				string strFileName = nDotIndex >= 0 ? file.Substring(nDotIndex + 1) : file;
				File.Copy(file, strTempFolderPath + "\\" + strFileName);
            
			}
                      

			System.IO.Compression.ZipFile.CreateFromDirectory(strTempFolderPath, szTargetFile);
			DeleteFolder(strTempFolderPath);
			return true;
		}

		public bool ExtractToTrg(string strSrcFile, string strTrgPath)
		{
			try
			{

                if (Directory.Exists(strTrgPath))
				    DeleteFolder(strTrgPath);
                
                if (!Directory.Exists(strTrgPath))
					Directory.CreateDirectory(strTrgPath);

				System.IO.Compression.ZipFile.ExtractToDirectory(strSrcFile, strTrgPath);
	     	}
			catch (Exception e)
			{
				Debug.WriteLine(e);
				return false;
			}

			return true;
			//return Core.UZip.ExtractFile(strSrcFile, strTrgPath);
		}

		public void RestoreData(string szResotreFileName)
		{
			FileInfo arFile = new FileInfo(szResotreFileName);
			if (arFile.Exists == true)
			{
				string szPath = arFile.Directory.ToString() + "\\ex";


				string szFileName = arFile.FullName.Replace(arFile.Extension, "_Main.sql");
                szFileName = szFileName.Replace("SDMS_Backup_", "");
                szFileName = szFileName.Replace(arFile.Directory.ToString(), szPath);

				if (File.Exists(szFileName))
				{
					try
					{
						File.Delete(szFileName);
					}
					catch (System.Exception)
					{
					}
				}

				if (ExtractToTrg(szResotreFileName, szPath))
				{
					if (File.Exists(szFileName))
					{
						// Resotre DB
						try
						{
							RestoreDatabase(szFileName);
						}
						catch (System.Exception)
						{
						}						
					}
				}

               if (Directory.Exists(szPath))
                  DeleteFolder(szPath);
                
			}
		}

        private string CreateTableSensorValues()
        {
            string szCreateSQL =
                "USE " + m_szRestoreDB + "\nGO\n" +
            "CREATE TABLE PSMSensorValues( " +
                                "ID int NULL, " +
                                "ValueTime datetime NULL," +
                                "SensorValue1 float NULL," +
                                "SensorValue2 float NULL," +
                                "SensorValue3 float NULL," +
                                "SensorValue4 float NULL," +
                                "SensorValue5 float NULL," +
                                "SensorValue6 float NULL," +
                                "SensorValue7 float NULL," +
                                "SensorValue8 float NULL," +
                                "SensorValue9 float NULL," +
                                "SensorValue10 float NULL," +
                                "SensorValue11 float NULL," +
                                "SensorValue12 float NULL," +
                                "SensorValue13 float NULL," +
                                "SensorValue14 float NULL," +
                                "SensorValue15 float NULL," +
                                "SensorValue16 float NULL," +
                                "SensorValue17 float NULL," +
                                "SensorValue18 float NULL," +
                                "SensorValue19 float NULL," +
                                "SensorValue20 float NULL," +
                                "SensorValue21 float NULL," +
                                "SensorValue22 float NULL," +
                                "SensorValue23 float NULL," +
                                "SensorValue24 float NULL," +
                                "SensorValue25 float NULL," +
                                "SensorValue26 float NULL," +
                                "SensorValue27 float NULL," +
                                "SensorValue28 float NULL," +
                                "SensorValue29 float NULL," +
                                "SensorValue30 float NULL," +
                                "SensorValue31 float NULL," +
                                "SensorValue32 float NULL," +
                                "SensorValue33 float NULL," +
                                "SensorValue34 float NULL," +
                                "SensorValue35 float NULL," +
                                "SensorValue36 float NULL," +
                                "SensorValue37 float NULL," +
                                "SensorValue38 float NULL," +
                                "SensorValue39 float NULL," +
                                "SensorValue40 float NULL," +
                                "SensorValue41 float NULL," +
                                "SensorValue42 float NULL," +
                                "SensorValue43 float NULL," +
                                "SensorValue44 float NULL," +
                                "SensorValue45 float NULL," +
                                "SensorValue46 float NULL," +
                                "SensorValue47 float NULL," +
                                "SensorValue48 float NULL," +
                                "SensorValue49 float NULL," +
                                "SensorValue50 float NULL" +
                                ");";

            return szCreateSQL;
        }

		private string CreateDatabase()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("use master");
			sb.AppendLine("go");
			sb.AppendLine("IF EXISTS (SELECT name FROM sys.databases WHERE name = N'"+ m_szRestoreDB + "')");
			sb.AppendLine("BEGIN");
            sb.AppendLine("alter database " + m_szRestoreDB + " set single_user with rollback immediate");
            sb.AppendLine("drop database " + m_szRestoreDB);
			sb.AppendLine("END");
			sb.AppendLine("go");
            sb.AppendLine("CREATE DATABASE " + m_szRestoreDB);
			sb.AppendLine("go");
            sb.AppendLine("ALTER DATABASE " + m_szRestoreDB + " SET COMPATIBILITY_LEVEL = 100");
			sb.AppendLine("GO");
            sb.AppendLine("ALTER DATABASE " + m_szRestoreDB + " SET ANSI_NULL_DEFAULT OFF");
			sb.AppendLine("GO");
            sb.AppendLine("ALTER DATABASE " + m_szRestoreDB + " SET ANSI_NULLS OFF ");
			sb.AppendLine("GO");
            sb.AppendLine("ALTER DATABASE " + m_szRestoreDB + " SET ANSI_PADDING OFF ");
			sb.AppendLine("GO");
            sb.AppendLine("ALTER DATABASE " + m_szRestoreDB + " SET ANSI_WARNINGS OFF ");
			sb.AppendLine("GO");
            sb.AppendLine("ALTER DATABASE " + m_szRestoreDB + " SET ARITHABORT OFF ");
			sb.AppendLine("GO");
            sb.AppendLine("ALTER DATABASE " + m_szRestoreDB + " SET AUTO_CLOSE ON ");
			sb.AppendLine("GO");
            sb.AppendLine("ALTER DATABASE " + m_szRestoreDB + " SET AUTO_CREATE_STATISTICS ON ");
			sb.AppendLine("GO");
            sb.AppendLine("ALTER DATABASE " + m_szRestoreDB + " SET AUTO_SHRINK OFF ");
			sb.AppendLine("GO");
            sb.AppendLine("ALTER DATABASE " + m_szRestoreDB + " SET AUTO_UPDATE_STATISTICS ON ");
			sb.AppendLine("GO");
            sb.AppendLine("ALTER DATABASE " + m_szRestoreDB + " SET CURSOR_CLOSE_ON_COMMIT OFF ");
			sb.AppendLine("GO");
            sb.AppendLine("ALTER DATABASE " + m_szRestoreDB + " SET CURSOR_DEFAULT  GLOBAL ");
			sb.AppendLine("GO");
            sb.AppendLine("ALTER DATABASE " + m_szRestoreDB + " SET CONCAT_NULL_YIELDS_NULL OFF ");
			sb.AppendLine("GO");
            sb.AppendLine("ALTER DATABASE " + m_szRestoreDB + " SET NUMERIC_ROUNDABORT OFF ");
			sb.AppendLine("GO");
            sb.AppendLine("ALTER DATABASE " + m_szRestoreDB + " SET QUOTED_IDENTIFIER OFF ");
			sb.AppendLine("GO");
            sb.AppendLine("ALTER DATABASE " + m_szRestoreDB + " SET RECURSIVE_TRIGGERS OFF ");
			sb.AppendLine("GO");
            sb.AppendLine("ALTER DATABASE " + m_szRestoreDB + " SET DISABLE_BROKER ");
			sb.AppendLine("GO");
            sb.AppendLine("ALTER DATABASE " + m_szRestoreDB + " SET AUTO_UPDATE_STATISTICS_ASYNC OFF ");
			sb.AppendLine("GO");
            sb.AppendLine("ALTER DATABASE " + m_szRestoreDB + " SET DATE_CORRELATION_OPTIMIZATION OFF");
			sb.AppendLine("GO");
            sb.AppendLine("ALTER DATABASE " + m_szRestoreDB + " SET TRUSTWORTHY OFF");
			sb.AppendLine("GO");
            sb.AppendLine("ALTER DATABASE " + m_szRestoreDB + " SET ALLOW_SNAPSHOT_ISOLATION ON");
			sb.AppendLine("GO");
            sb.AppendLine("ALTER DATABASE " + m_szRestoreDB + " SET PARAMETERIZATION SIMPLE");
			sb.AppendLine("GO");
            sb.AppendLine("ALTER DATABASE " + m_szRestoreDB + " SET READ_COMMITTED_SNAPSHOT ON");
			sb.AppendLine("GO");
            sb.AppendLine("ALTER DATABASE " + m_szRestoreDB + " SET HONOR_BROKER_PRIORITY OFF");
			sb.AppendLine("GO");
            sb.AppendLine("ALTER DATABASE " + m_szRestoreDB + " SET RECOVERY SIMPLE");
			sb.AppendLine("GO");
            sb.AppendLine("ALTER DATABASE " + m_szRestoreDB + " SET PAGE_VERIFY CHECKSUM");
			sb.AppendLine("GO");
            sb.AppendLine("ALTER DATABASE " + m_szRestoreDB + " SET DB_CHAINING OFF");
			sb.AppendLine("GO");
            sb.AppendLine("USE " + m_szRestoreDB);
			sb.AppendLine("GO");

			return sb.ToString();
		}

		public bool BackupData()
		{
			//SOP3에있는 테이블을 불러옴.
			string strSQL = "select TABLE_NAME from information_schema.tables where TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME <> 'sysdiagrams' order by TABLE_NAME";
			ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
			if (arrResult == null)
				return false;
            
			string strNowDate = DateTime.Now.ToString(("yyyyMMdd_HHmmss"));
			string szFileName = m_szBackupDir + "\\" + strNowDate + "_Main.sql";
            string szFileName2 = m_szBackupDir + "\\" + strNowDate + "_SensorValues1.sql";
            string szFileName3 = m_szBackupDir + "\\" + strNowDate + "_SensorValues2.sql";
            string szFileName4 = m_szBackupDir + "\\" + strNowDate + "_SensorValues3.sql";
            string szFileName5 = m_szBackupDir + "\\" + strNowDate + "_SensorValues4.sql";
            string szFileName6 = m_szBackupDir + "\\" + strNowDate + "_SensorValues5.sql";
            string szFileName7 = m_szBackupDir + "\\" + strNowDate + "_SensorValues6.sql";
            string szZipName = m_szBackupDir + "\\" + "SDMS_Backup_" + strNowDate + ".zip";
			try
			{
				using (System.IO.StreamWriter file = new System.IO.StreamWriter(szFileName))
				{
					string szCreate = CreateDatabase();
					file.WriteLine(szCreate);

					//Create Table
					file.WriteLine("--======================CREATE TABLE============================");
					CreateTable(file, arrResult);

					//Create View
					file.WriteLine("--======================CREATE VIEW============================");
					CreateView(file);

					//프로시저
					file.WriteLine("--======================CREATE PROCEDURE============================");
					ProcedureSQL(file, arrResult);

					//Insert Table
					file.WriteLine("--======================INSERT TABLE============================");
					InsertTable(file, arrResult);

					//Default Value
					file.WriteLine("--======================DEFAULT VALUE============================");
					Default_Value(file, arrResult);

					//Foreign Key
					file.WriteLine("--======================FOREIGN KEY============================");
					Foreign_Key(file, arrResult);

					//addextendedproperty
					file.WriteLine("--======================추가 설명============================");
					Addextendedproperty(file);


                    if( UnE.SOP.ProxySOP.Instance.UsePSM == true)
                    {
                        file.WriteLine("--======================Restore Sensor Value============================");
                        string szCreate2 = CreateTableSensorValues();
                        file.WriteLine(szCreate2);
                        file.WriteLine("GO");
                        AddRestoreSensorValues(file);
                    }
                    

					file.Close();
				}

                if (UnE.SOP.ProxySOP.Instance.UsePSM == true)
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(szFileName2))
                    {
                        file.WriteLine("--======================CREATE TABLE============================");
                        string szCreate = CreateTableSensorValues();
                        file.WriteLine(szCreate);

                        using (System.IO.StreamWriter file2 = new System.IO.StreamWriter(szFileName3))
                        {
                            //Default Value
                            file.WriteLine("--======================DEFAULT VALUE============================");
                            file2.WriteLine("--======================DEFAULT VALUE============================");
                            DefaultSensorValue(file, file2, "PSMSensorValues");
                            file2.Close();
                        }
                        file.Close();
                    }

                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(szFileName4))
                    {
                        file.WriteLine("--======================INSERT ZERO VALUE============================");
                        using (System.IO.StreamWriter file2 = new System.IO.StreamWriter(szFileName5))
                        {
                            file2.WriteLine("--======================INSERT ZERO VALUE============================");
                            InsertSensorValueTable(file, file2, "PSMSensorValues");
                            file2.Close();
                        }
                        file.Close();
                    }                          

                }
                
				ArrayList ar = new ArrayList();
				ar.Add(szFileName);

                if( UnE.SOP.ProxySOP.Instance.UsePSM == true)
                {
                    ar.Add(szFileName2);
                    ar.Add(szFileName3);
                    ar.Add(szFileName4);
                    ar.Add(szFileName5);
                }
               
               
                if (!CompressFile(szZipName, ar))
				{
                    if (File.Exists(szZipName))
                        File.Delete(szZipName);
				}	
			}
			catch (System.Exception ex)
			{
                System.Diagnostics.Trace.WriteLine(ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
			}


            try
            {
                if (File.Exists(szFileName))
                {
                    File.Delete(szFileName);
                }
                if (File.Exists(szFileName2))
                {
                    File.Delete(szFileName2);
                }
                if (File.Exists(szFileName3))
                {
                    File.Delete(szFileName3);
                }
                if (File.Exists(szFileName4))
                {
                    File.Delete(szFileName4);
                }
                if (File.Exists(szFileName5))
                {
                    File.Delete(szFileName5);
                }

            }
            catch(Exception)
            {

            }
			return true;
		}

        private void AddRestoreSensorValues(StreamWriter file)
        {
            string szSQL = "IF EXISTS (SELECT name FROM sys.databases WHERE name = N'TempSensorValues')\n" +
                            " BEGIN\n" +
                            "   insert into SOP3_RESTORE.dbo.PSMSensorValues select * from TempSensorValues.dbo.PSMSensorValues\n"+
                            " END";

            file.WriteLine(szSQL);
            file.WriteLine("GO");
         
            // Backup sensor value to tempdb
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("use master");
            sb.AppendLine("go");
            sb.AppendLine("IF EXISTS (SELECT name FROM sys.databases WHERE name = N'TempSensorValues')");
            sb.AppendLine("BEGIN");
            sb.AppendLine("alter database TempSensorValues set single_user with rollback immediate");
            sb.AppendLine("drop database TempSensorValues");
            sb.AppendLine("END");
            sb.AppendLine("GO");
            sb.AppendLine("CREATE DATABASE TempSensorValues");
            sb.AppendLine("go");
            sb.AppendLine("select * into TempSensorValues.dbo.PSMSensorValues from SOP_1.dbo.PSMSensorValues");
            sb.AppendLine("go");

            m_dbMgr.GetResultData(sb.ToString());           

        }

		private void ProcedureSQL(StreamWriter file, ArrayList arrResult)
		{
			string strProcedure = "select sys.syscomments.text from sys.syscomments, sys.procedures where syscomments.id=sys.procedures.object_id";

			ArrayList arrProcedure = m_dbMgr.GetResultData(strProcedure);
			for (int i = 0; i < arrProcedure.Count; i++)
			{
				if (arrProcedure[i].ToString() == "")
					continue;

				if (arrProcedure[i].ToString().ToUpper().StartsWith("CREATE PROCEDURE"))
				{
					file.WriteLine("GO");
					file.WriteLine("");
				}
				file.WriteLine(arrProcedure[i]);
			}
			file.WriteLine("GO");
		}

		private void CreateTable(StreamWriter file, ArrayList arrResult)
		{
			ArrayList arrResult5 = new ArrayList();
			ArrayList arrResult6 = new ArrayList();

			for (int i = 0; i < arrResult.Count; i++)
			{
				string strTableName = WebDBManager.GetStringField(arrResult[i], "");
                if (strTableName == "PSMSensorValues")
                    continue;
				string strSQL5 = "SELECT COLUMN_NAME, data_type, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '" + strTableName + "'";
				arrResult5 = m_dbMgr.GetResultData(strSQL5);
				if (arrResult5 == null)
					return;

				string strSQL6 = "";
				strSQL6 = "select t.name as TABLE_NAME, k.name as CONSTRAINT_NAME, c.name as COLUMN_NAME, "
						+ "ix.allow_page_locks , ix.allow_row_locks, ix.is_padded, ix.ignore_dup_key, st.no_recompute from sys.key_constraints as k "
						+ "join sys.tables as t on t.object_id = k.parent_object_id "
						+ "join sys.schemas as s on s.schema_id = t.schema_id "
						+ "join sys.index_columns as ic on ic.object_id = t.object_id and ic.index_id = k.unique_index_id "
						+ "join sys.columns as c on c.object_id = t.object_id and c.column_id = ic.column_id "
						+ "join sys.indexes as ix on ix.name = k.name "
						+ "join sys.stats as st on st.object_id = ix.object_id And st.stats_id = (select max(stats_id) from sys.stats where object_id = ix.object_id) "
						+ "where t.name = '" + strTableName + "'";

				arrResult6 = m_dbMgr.GetResultData(strSQL6);
				if (arrResult6 == null)
					return;

				//Create Table
				string strSQLCreate = "";
				strSQLCreate = "CREATE TABLE " + strTableName + "(";
				file.WriteLine(strSQLCreate);

				string strIsNull = "";
				string strColumnName = "";
				string strDataType = "";
				string strCharLength = "";

				for (int n = 0; n < arrResult5.Count - 3; n += 4)
				{
					strColumnName = WebDBManager.GetStringField(arrResult5[n], "");
					strDataType = WebDBManager.GetStringField(arrResult5[n + 1], "");
					strCharLength = WebDBManager.GetStringField(arrResult5[n + 2], "");
					strIsNull = WebDBManager.GetStringField(arrResult5[n + 3], "");

					if (strCharLength == "null" || strDataType == "text")
						strCharLength = "";
					else
						strCharLength = "(" + strCharLength + ")";

					if (strIsNull == "NO")
						strIsNull = "NOT NULL";
					else
						strIsNull = "NULL";

					//마지막 for문일때
					if ((arrResult5.Count - 4) == n)
					{
						//PK가 있는 테이블이면
						if (arrResult6.Count > 0)
							strIsNull += ",";
					}
					else
					{
						//마지막이 아니면 무조건 , 붙임
						strIsNull += ",";
					}
					string strIdentityID = "";
					//ID 자동증가라면
					bool isIdentity = IsAutoID(file, strTableName, ref strIdentityID);

					if (isIdentity == true && strColumnName == strIdentityID)
						strSQLCreate = "    " + strColumnName + " " + strDataType + strCharLength + " IDENTITY(1,1) " + strIsNull;
					else
						strSQLCreate = "    " + strColumnName + " " + strDataType + strCharLength + " " + strIsNull;

					file.WriteLine(strSQLCreate);
				}

				//pk가 없는 테이블이면
				if (arrResult6.Count == 0)
				{
					strSQLCreate = ") ON [PRIMARY]";
					file.WriteLine(strSQLCreate);

					//3칸줄넘기기
					for (int count = 0; count < 3; count++)
					{
						file.WriteLine("");
					}
				}
				else
				{
					string strIsPadded = "";
					string strNoRecompute = "";
					string strIgnoreDupKey = "";
					string strAllowRowLock = "";
					string strAllowPageLock = "";
					for (int n = 0; n < arrResult6.Count - 7; n += 8)
					{
						//string strTable = WebDBManager.GetStringField(arrResult5[n], "");
						string strConstraintName = WebDBManager.GetStringField(arrResult6[n + 1], "");
						string strKeyName = WebDBManager.GetStringField(arrResult6[n + 2], "");
						int nAllowPageLock = WebDBManager.GetIntField(arrResult6[n + 3].ToString(), 0);
						int nAllowRowLock = WebDBManager.GetIntField(arrResult6[n + 4].ToString(), 0);
						int nIsPadded = WebDBManager.GetIntField(arrResult6[n + 5].ToString(), 0);
						int nIgnoreDupKey = WebDBManager.GetIntField(arrResult6[n + 6].ToString(), 0);
						int nNoRecompute = WebDBManager.GetIntField(arrResult6[n + 7].ToString(), 0);

						//PAD_INDEX
						if (nIsPadded == 0)
							strIsPadded = "OFF";
						else
							strIsPadded = "ON";

						//STATISTICS_NORECOMPUTE
						if (nNoRecompute == 0)
							strNoRecompute = "OFF";
						else
							strNoRecompute = "ON";

						//IGNORE_DUP_KEY
						if (nIgnoreDupKey == 0)
							strIgnoreDupKey = "OFF";
						else
							strIgnoreDupKey = "ON";

						//ALLOW_ROW_LOCKS
						if (nAllowRowLock == 0)
							strAllowRowLock = "OFF";
						else
							strAllowRowLock = "ON";

						//ALLOW_PAGE_LOCKS
						if (nAllowPageLock == 0)
							strAllowPageLock = "OFF";
						else
							strAllowPageLock = "ON";

                        if(n == 0)
                        {
                            strSQLCreate = "CONSTRAINT " + strConstraintName + " PRIMARY KEY CLUSTERED";
                            file.WriteLine(strSQLCreate);
                            strSQLCreate = "(";
                            file.WriteLine(strSQLCreate);
                        }	

                        if( n == 0)
                        {
                            strSQLCreate = "    " + strKeyName + " ASC";
                            file.WriteLine(strSQLCreate);
                        }
                        else
                        {
                            strSQLCreate = "    ," + strKeyName + " ASC";
                            file.WriteLine(strSQLCreate);
                        }
					}

                    strSQLCreate = ")WITH (PAD_INDEX = " + strIsPadded + ", STATISTICS_NORECOMPUTE = " + strNoRecompute + ", IGNORE_DUP_KEY = " + strIgnoreDupKey + ", "
                        + "ALLOW_ROW_LOCKS = " + strAllowRowLock + ", ALLOW_PAGE_LOCKS = " + strAllowPageLock + ") ON [PRIMARY]";
                    file.WriteLine(strSQLCreate);
                    strSQLCreate = ") ON [PRIMARY]";
                    file.WriteLine(strSQLCreate);

                    //3칸줄넘기기
                    for (int count = 0; count < 3; count++)
                    {
                        file.WriteLine("");
                    }
                    file.WriteLine("GO");
				}
			}
		}

		private bool IsAutoID(StreamWriter file, string strTableName, ref string strIdentityID)
		{
			string strSQL = "select ic.name, tb.name from sys.identity_columns as ic ";
			strSQL += "join sys.tables as tb on ic.object_id = tb.object_id where tb.name = '" + strTableName + "'";

			ArrayList arrResult = new ArrayList();
			arrResult = m_dbMgr.GetResultData(strSQL);
			if (arrResult == null)
				return false;
			if (arrResult.Count == 0)
				return false;

			strIdentityID = WebDBManager.GetStringField(arrResult[0], "");
			return true;
		}

		private void CreateView(StreamWriter file)
		{
			file.WriteLine("GO");
			string strSQL = "select VIEW_DEFINITION from INFORMATION_SCHEMA.VIEWS";
			ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

			bool bFirst = true;
			string szPrevLine = "";
			for (int i = 0; i < arrResult.Count; i++)
			{
				string strViewDefinition = WebDBManager.GetStringField(arrResult[i], "");
				strViewDefinition = strViewDefinition.Trim();

				if (strViewDefinition == "")
					continue;
				if (strViewDefinition.StartsWith("CREATE VIEW"))
				{
					if (bFirst == true)
					{
						bFirst = false;
					}
					else
					{
						szPrevLine += "\r\nGO\r\n";
					}
				}
				//if (szPrevLine == "" )
				//    continue;
				file.WriteLine(szPrevLine);
				szPrevLine = strViewDefinition;
			}
			file.WriteLine(szPrevLine + "\r\nGO\r\n");
		}

		private void InsertTable(StreamWriter file, ArrayList arrResult)
		{
			ArrayList arrResult2 = new ArrayList();
			ArrayList arrResult3 = new ArrayList();
			ArrayList arrResult4 = new ArrayList();

			for (int i = 0; i < arrResult.Count; i++)
			{
				string strTableName = WebDBManager.GetStringField(arrResult[i], "");

				if (strTableName == "Broadcast" || strTableName == "PSMSensorValues")
					continue;

				string strSQL2 = "select * from " + strTableName + "";
				arrResult2 = m_dbMgr.GetResultData(strSQL2);
				if (arrResult2 == null)
					return;

				string strSQL3 = "SELECT DATA_TYPE from INFORMATION_SCHEMA.COLUMNS A WHERE A.TABLE_NAME = '" + strTableName + "'";
				arrResult3 = m_dbMgr.GetResultData(strSQL3);
				if (arrResult3 == null)
					return;

				string strSQL4 = "SELECT COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS A WHERE A.TABLE_NAME = '" + strTableName + "'";
				arrResult4 = m_dbMgr.GetResultData(strSQL4);
				if (arrResult4 == null)
					return;

				//file.WriteLine(strSQLCreate);

				string strSQLInsert = "";
				string stridentityID = "";
				bool isIdentity = IsAutoID(file, strTableName, ref stridentityID);

				//if (isIdentity == true)
				//{
				//    strSQLInsert = "SET IDENTITY_INSERT " + strTableName + " ON;";
				//    file.WriteLine(strSQLInsert);
				//    file.WriteLine("");
				//}
				int nIDIdx = -1;
                int nCount = 0;
				for (int j = 0; j < arrResult2.Count - (arrResult3.Count - 1); ) //+= arrRetsult2.Count)
				{
					//칼럼명

					strSQLInsert = "INSERT INTO " + strTableName + "(";
					for (int m = 0; m < arrResult4.Count; m++)
					{
						if (stridentityID.Equals(arrResult4[m].ToString()))
						{
							nIDIdx = m;
							continue;
						}
						if (m != arrResult4.Count - 1)
						{
							strSQLInsert += arrResult4[m] + ",";
						}
						else
						{
							strSQLInsert += arrResult4[m];
						}
					}
					strSQLInsert += ") ";
					strSQLInsert += "VALUES(";
					for (int k = 0; k < arrResult3.Count; k++)
					{
						if (nIDIdx != k)
						{
							if (arrResult2[j].ToString() == "null")
							{
								arrResult2[j] = "NULL";
							}
							else
							{
								if (arrResult3[k].ToString() == "int")
								{
									arrResult2[j] = arrResult2[j];
								}
								else if (arrResult3[k].ToString() == "nvarchar" || arrResult3[k].ToString() == "char" || arrResult3[k].ToString() == "varchar")
								{
                                    string szValue = arrResult2[j].ToString().Trim().Replace('\'', '`');
                                    arrResult2[j] = "N'" + szValue + "'";
								}
								else if (arrResult3[k].ToString() == "text" || arrResult3[k].ToString() == "nchar")
								{
                                    string szValue = arrResult2[j].ToString().Trim().Replace('\'', '`');
                                    arrResult2[j] = "N'" + szValue + "'";
								}
								else if (arrResult3[k].ToString() == "datetime")
									arrResult2[j] = "CAST('" + arrResult2[j] + "' AS DateTime)";
							}

							//마지막 데이터가 아니면 뒤에 ,를 찍어줌
							if (k != arrResult3.Count - 1)
								arrResult2[j] = arrResult2[j] + ",";

							strSQLInsert += arrResult2[j];
						}
						j++;                        
					}
					strSQLInsert += ")";
					file.WriteLine(strSQLInsert + ";");

                    nCount++;

                    if (nCount == 50)
                    {
                        nCount = 0;
                        file.WriteLine("GO");
                    }
				}
				file.WriteLine("GO");
				file.WriteLine("");
			}
		}

		private void Default_Value(StreamWriter file, ArrayList arrResult)
		{
			for (int i = 0; i < arrResult.Count; i++)
			{
				string strTableName = WebDBManager.GetStringField(arrResult[i], "");
                if (strTableName == "PSMSensorValues")
                    continue;
				string strDefaultValue = "";

				string strSQL = "select tb.name,dc.name, dc.definition, cs.name from sys.tables as tb ";
				strSQL += "join  sys.default_constraints as dc on tb.object_id = dc.parent_object_id ";
				strSQL += "join sys.columns as cs on cs.object_id = dc.parent_object_id And cs.column_id = dc.parent_column_id ";
				strSQL += "where tb.name = '" + strTableName + "'";

				ArrayList arrResult2 = new ArrayList();
				arrResult2 = m_dbMgr.GetResultData(strSQL);
				if (arrResult2 == null)
					return;

				for (int j = 0; j < arrResult2.Count - 3; j += 4)
				{
					string strDefValueName = WebDBManager.GetStringField(arrResult2[j + 1], "");
					string strDefinition = WebDBManager.GetStringField(arrResult2[j + 2], "");
					string strColumn = WebDBManager.GetStringField(arrResult2[j + 3], "");

					strDefaultValue = "ALTER TABLE " + strTableName + " ADD CONSTRAINT " + strDefValueName + " DEFAULT "
						+ strDefinition + " FOR " + strColumn;
					file.WriteLine(strDefaultValue);
					strDefaultValue = "GO";
					file.WriteLine(strDefaultValue);
				}
			}
		}

		private void Foreign_Key(StreamWriter file, ArrayList arrResult)
		{
			for (int i = 0; i < arrResult.Count; i++)
			{
				string strTableName = WebDBManager.GetStringField(arrResult[i], "");
                if (strTableName == "PSMSensorValues")
                    continue;
				//PK Constraint
				string strPKConstraint = "";

				string strSQL7 = "select fk.name, ts.name, fk.type_desc, cs.name , ts2.name as FKname, cs2.name  from  sys.foreign_keys as fk ";
				strSQL7 += "join sys.foreign_key_columns as fkc on fkc.constraint_object_id= fk.object_id ";
				strSQL7 += "join sys.tables as ts on ts.object_id = fkc.parent_object_id ";
				strSQL7 += "join sys.columns as cs on ts.object_id = cs.object_id and fkc.parent_column_id = cs.column_id ";
				strSQL7 += "join sys.tables as ts2 on ts2.object_id = fkc.referenced_object_id ";
				strSQL7 += "join sys.columns as cs2 on ts2.object_id = cs2.object_id and fkc.referenced_column_id = cs2.column_id ";
				strSQL7 += "where ts.name = '" + strTableName + "'";

				ArrayList arrResult7 = new ArrayList();
				arrResult7 = m_dbMgr.GetResultData(strSQL7);
				if (arrResult7 == null)
					return;

				for (int j = 0; j < arrResult7.Count - 5; j += 6)
				{
					string strFKName = WebDBManager.GetStringField(arrResult7[j], "");
					string strForeignKey = WebDBManager.GetStringField(arrResult7[j + 3], "");
					string strFKTableName = WebDBManager.GetStringField(arrResult7[j + 4], "");
					string strFKID = WebDBManager.GetStringField(arrResult7[j + 5], "");

					strPKConstraint = "ALTER TABLE " + strTableName + " WITH CHECK ADD  CONSTRAINT " + strFKName + " FOREIGN KEY(" + strForeignKey + ")";
					file.WriteLine(strPKConstraint);
					strPKConstraint = "REFERENCES " + strFKTableName + " (" + strFKID + ")";
					file.WriteLine(strPKConstraint);
					strPKConstraint = "GO";
					file.WriteLine(strPKConstraint);
					strPKConstraint = "ALTER TABLE " + strTableName + " CHECK CONSTRAINT " + strFKName;
					file.WriteLine(strPKConstraint);
					file.WriteLine("GO");
				}
			}
		}

		//특수문자 포함여부
		private bool CheckingSpecialText(string txt)
		{
			string str = @"[']";
			System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(str);
			return rex.IsMatch(txt);
		}

		private void Addextendedproperty(StreamWriter file)
		{
			//열에 있는 주석
			string strSQL = "select ep.name, stb.TABLE_SCHEMA, stb.TABLE_TYPE, tb.name, cs.name from sys.extended_properties as ep ";
			//string strSQL = "select ep.name, ep.value, stb.TABLE_SCHEMA, stb.TABLE_TYPE, tb.name, cs.name from sys.extended_properties as ep ";
			strSQL += "join sys.tables as tb on ep.major_id = tb.object_id ";
			strSQL += "join INFORMATION_SCHEMA.TABLES stb on stb.TABLE_NAME = tb.name ";
			strSQL += "join sys.columns as cs on ep.minor_id = cs.column_id And tb.object_id = cs.object_id where tb.name <> 'sysdiagrams' order by tb.name";

			ArrayList arrResult = new ArrayList();
			arrResult = m_dbMgr.GetResultData(strSQL);
			if (arrResult == null)
				return;

			string strAddextend = "";

			for (int i = 0; i < arrResult.Count - 4; i += 5)
			{
				string strName = WebDBManager.GetStringField(arrResult[i], "");
				//string strValue = WebDBManager.GetStringField(arrResult[i + 1], "");
				string strTableSchema = WebDBManager.GetStringField(arrResult[i + 1], "");
				string strTableType = WebDBManager.GetStringField(arrResult[i + 2], "");
				string strTableName = WebDBManager.GetStringField(arrResult[i + 3], "");
				string strColumnName = WebDBManager.GetStringField(arrResult[i + 4], "");
                if (strTableName == "PSMSensorValues")
                    continue;
				if (strTableType == "BASE TABLE")
					strTableType = "TABLE";

                string szSubSql = string.Format("SELECT CAST(ep.value AS nvarchar(255)) FROM sys.extended_properties AS ep " +
                                  " JOIN sys.tables AS tb ON ep.major_id = tb.object_id " +
                                  " JOIN INFORMATION_SCHEMA.TABLES stb ON stb.TABLE_NAME = tb.name " +
                                  " JOIN sys.columns AS cs ON ep.minor_id = cs.column_id And tb.object_id = cs.object_id " +
                                  " WHERE tb.name <> 'sysdiagrams' AND cs.Name = '{0}' AND tb.name = '{1}'", strColumnName, strTableName);

                ArrayList arrResult10 = m_dbMgr.GetResultData(szSubSql);
                if (arrResult10 == null)
                    continue;

                string strValue = "";
                for (int j = 0; j < arrResult10.Count; j++)
                {
                    strValue += WebDBManager.GetStringField(arrResult10[j], "");
                }

				bool test = CheckingSpecialText(strValue);
				if (test == true)
				{
					strValue = strValue.Replace("'", "''");
				}

				strName = "N'" + strName + "'";
				strValue = "N'" + strValue + "'";
				strTableSchema = "N'" + strTableSchema + "'";
				strTableType = "N'" + strTableType + "'";
				strTableName = "N'" + strTableName + "'";
				strColumnName = "N'" + strColumnName + "'";

				strAddextend = "EXEC sys.sp_addextendedproperty @name= " + strName + ", @value = " + strValue + ", @level0type= N'SCHEMA', ";
				strAddextend += "@level0name = " + strTableSchema + ", @level1type = " + strTableType + ", @level1name= " + strTableName;
				strAddextend += ", @level2type= N'COLUMN' , @level2name = " + strColumnName;

				file.WriteLine(strAddextend);
				file.WriteLine("GO");
			}

			//테이블 자체에 주석
			string strSQL2 = "select ep.name, CAST(ep.value as nvarchar(255)),stb.TABLE_SCHEMA,  stb.TABLE_TYPE, tb.name from sys.extended_properties as ep ";
			strSQL2 += "join sys.tables as tb on ep.major_id = tb.object_id ";
			strSQL2 += "join INFORMATION_SCHEMA.TABLES stb on stb.TABLE_NAME  = tb.name where ep.minor_id = 0 And tb.name <> 'sysdiagrams'";

			ArrayList arrResult2 = m_dbMgr.GetResultData(strSQL2);
			if (arrResult2 == null)
				return;

			for (int j = 0; j < arrResult2.Count - 4; j += 5)
			{
				string strName = WebDBManager.GetStringField(arrResult2[j], "");
				string strValue = WebDBManager.GetStringField(arrResult2[j + 1], "");
				string strTableSchema = WebDBManager.GetStringField(arrResult2[j + 2], "");
				string strTableType = WebDBManager.GetStringField(arrResult2[j + 3], "");
				string strTableName = WebDBManager.GetStringField(arrResult2[j + 4], "");
                if (strTableName == "PSMSensorValues")
                    continue;
				if (strTableType == "BASE TABLE")
					strTableType = "TABLE";

				strName = "N'" + strName + "'";
				strValue = "N'" + strValue + "'";
				strTableSchema = "N'" + strTableSchema + "'";
				strTableType = "N'" + strTableType + "'";
				strTableName = "N'" + strTableName + "'";

				strAddextend = "EXEC sys.sp_addextendedproperty @name= " + strName + ", @value = " + strValue + ", @level0type= N'SCHEMA', ";
				strAddextend += "@level0name = " + strTableSchema + ", @level1type = " + strTableType + ", @level1name= " + strTableName;

				file.WriteLine(strAddextend);
				file.WriteLine("GO");
			}

			//View
			string strSQL3 = "select ep.name, stb.TABLE_SCHEMA,stb.TABLE_TYPE, stb.TABLE_NAME   from sys.views as vw ";
			strSQL3 += "join sys.extended_properties as ep on vw.object_id = ep.major_id ";
			strSQL3 += "join INFORMATION_SCHEMA.TABLES as stb on vw.name = stb.TABLE_NAME ";

			ArrayList arrResult3 = m_dbMgr.GetResultData(strSQL3);
			if (arrResult3 == null)
				return;

			for (int j = 0; j < arrResult3.Count - 3; j += 4)
			{
				string strName = WebDBManager.GetStringField(arrResult3[j], "");
				//string strValue = WebDBManager.GetStringField(arrResult3[j + 1], "");
				string strTableSchema = WebDBManager.GetStringField(arrResult3[j + 1], "");
				string strTableType = WebDBManager.GetStringField(arrResult3[j + 2], "");
				string strTableName = WebDBManager.GetStringField(arrResult3[j + 3], "");
                if (strTableName == "PSMSensorValues")
                    continue;

				//Value값만 따로 한줄씩 읽음
				string strSQL4 = "select CAST(ep.value as nvarchar(4000)) from sys.views as vw ";
				strSQL4 += "join sys.extended_properties as ep on vw.object_id = ep.major_id where ep.name = '" + strName + "' And vw.name = '" + strTableName + "'";
				ArrayList arrResult4 = m_dbMgr.GetResultData(strSQL4);
				if (arrResult4 == null)
					return;

				strName = "N'" + strName + "'";
				//strValue = "N'" + strValue + "'";
				strTableSchema = "N'" + strTableSchema + "'";
				strTableType = "N'" + strTableType + "'";
				strTableName = "N'" + strTableName + "'";

				//, CAST(ep.value as nvarchar(255))

				strAddextend = "EXEC sys.sp_addextendedproperty @name= " + strName + ",@value = ";
				file.WriteLine(strAddextend);

				//Value부분
				for (int k = 0; k < arrResult4.Count; k++)
				{
					string strValue = WebDBManager.GetStringField(arrResult4[k], "");

					//Count일경우는 int로 처리
					if (strName == "N'MS_DiagramPaneCount'")
					{
						file.WriteLine(strValue);
					}
					else
					{
						if (k == 0)
							strValue = "N'" + strValue;
						else if (k == arrResult4.Count - 1)
							strValue = strValue + "'";
						file.WriteLine(strValue);
					}
				}

				strAddextend = ", @level0type= N'SCHEMA', @level0name = " + strTableSchema + ", @level1type = " + strTableType + ", @level1name= " + strTableName;
				file.WriteLine(strAddextend);
				file.WriteLine("GO");
			}
		}

        private void DefaultSensorValue(StreamWriter file, StreamWriter file2, string szTableName)
        {
            file.WriteLine("USE " + m_szRestoreDB);
            file.WriteLine("GO");

            string szSQL = "SELECT ID, ValueTime FROM PSMSensorValues";
            ArrayList arrResult2 = new ArrayList();
            arrResult2 = m_dbMgr.GetResultData(szSQL);

            string strSQLInsert = "";

            int nTargetCount = arrResult2.Count / 4;
            int nCount = 0;
            for (int j = 0; j < arrResult2.Count; j += 2)
			{
				strSQLInsert = "INSERT INTO " + szTableName + "( ID, ValueTime ) VALUES (";
                strSQLInsert += arrResult2[j] + ",";
                strSQLInsert += "CAST('" + arrResult2[j + 1] + "' AS DateTime)";
				strSQLInsert += ")";

                if (nTargetCount > nCount)
				    file.WriteLine(strSQLInsert + ";");
                else
                    file2.WriteLine(strSQLInsert + ";");

                nCount++;
			}			
			file.WriteLine("GO");
            file2.WriteLine("GO");

            string szUpdateZero = "UPDATE PSMSensorValues " +
                    "SET SensorValue1 = 0  ,SensorValue2 = 0  ,SensorValue3 = 0  ,SensorValue4 = 0 "+
                      ",SensorValue5 = 0  ,SensorValue6 = 0  ,SensorValue7 = 0  ,SensorValue8 = 0  "+
                      ",SensorValue9 = 0  ,SensorValue10 = 0 ,SensorValue11 = 0 ,SensorValue12 = 0 "+
                      ",SensorValue13 = 0 ,SensorValue14 = 0 ,SensorValue15 = 0 ,SensorValue16 = 0 "+
                      ",SensorValue17 = 0 ,SensorValue18 = 0 ,SensorValue19 = 0 ,SensorValue20 = 0 "+
                      ",SensorValue21 = 0 ,SensorValue22 = 0 ,SensorValue23 = 0 ,SensorValue24 = 0 "+
                      ",SensorValue25 = 0 ,SensorValue26 = 0 ,SensorValue27 = 0 ,SensorValue28 = 0 "+
                      ",SensorValue29 = 0 ,SensorValue30 = 0 ,SensorValue31 = 0 ,SensorValue32 = 0 "+
                      ",SensorValue33 = 0 ,SensorValue34 = 0 ,SensorValue35 = 0 ,SensorValue36 = 0 "+
                      ",SensorValue37 = 0 ,SensorValue38 = 0 ,SensorValue39 = 0 ,SensorValue40 = 0 "+
                      ",SensorValue41 = 0 ,SensorValue42 = 0";

            file2.WriteLine(szUpdateZero);
            file2.WriteLine("GO");           
        }

        private void InsertSensorValueTable(StreamWriter file, StreamWriter file2, string szTableName)
        {
            for (int i = 1; i <= 50; i++)
            {
                string strSQL = string.Format("Select ID, SensorValue{0} from PSMSensorValues where SensorValue{0} <> 0", i);
                ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
                if (arrResult == null)
                    break;

                int nResultCount = arrResult.Count;
                int nTargetCount = nResultCount / 4;
                int nCount = 0;
                for (int j = 0; j < nResultCount - 1; j += 2)
                {
                    VariousData<int> id = WebDBManager.GetIntField(arrResult[j].ToString());
                    VariousData<float> value = WebDBManager.GetFloatField(arrResult[j + 1].ToString());

                    if (id == null || value == null)
                        continue;


                    string szInserValue = string.Format("UPDATE PSMSensorValues SET SensorValue{0} = {1} WHERE ID = {2}", i, value.Data, id.Data);


                    if (nTargetCount > nCount)
                        file.WriteLine(szInserValue);
                    else
                        file2.WriteLine(szInserValue);

                    nCount++;
                }
            }
            file.WriteLine("GO");
        } 
	}

}