using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WebQueryTest
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			
			InitializeComponent();
		}
		DBUtility.WebDBManager webDb = new DBUtility.WebDBManager();
		private void button1_Click(object sender, EventArgs e)
		{
			
			
			webDb.WebServerURL = "http://127.0.0.1:8080/SOP";
			// GO chagen semi colon
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("DECLARE @teamID int;");
            sb.AppendFormat("SET @teamID = ( SELECT TeamID FROM Site WHERE ID = {0});", 1);
            sb.AppendLine("");
            sb.AppendLine("DECLARE @teamTable TABLE(TeamID int, TeamName nvarchar(50), pTeamID int);");
            sb.AppendLine("DECLARE @idTable TABLE(TeamID int);");
            sb.AppendLine("INSERT INTO @teamTable EXECUTE sp_TeamList2 @teamID;");
            sb.AppendLine("INSERT INTO @idTable SELECT TeamID FROM @teamTable;");
            sb.Append("SELECT c.id , j.LevelNo FROM CompanyMember as c, JobLevel as j ");
            sb.AppendFormat(" WHERE c.MemberID = '{0}' AND c.MemberName = '{1}' ", "11140114", "강근영");
            sb.AppendFormat(" AND c.LevelID = j.ID AND c.RegularTeamID in (SELECT TeamID FROM @idTable);");

          
            ArrayList arList = webDb.GetResultData(sb.ToString(), 0, "SOP3");
			if (arList != null)
			{
				this.label1.Text = "" + arList.Count;
			}
			else
			{
				this.label1.Text = "NULL";
			}

		}
	}
}
