using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;


/// <summary>
/// DBConn의 요약 설명입니다.
/// </summary>
public class DBConn 
{
    private OleDbConnection conn = null;
    public DBConn()
    {
        string szDBPath = Application.StartupPath + "\\" + "Database1.accdb";
        conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + szDBPath);
    }
    
    // 2.데이터베이스 열기
    public void Open()
    {
        conn.Open();
    }
    // 3.데이터베이스 닫기
    public void Close()
    {
        conn.Close();
    }
    // 4.SQL 문을 실행합니다.
    public void ExecuteSQL(string sql)
    {
        OleDbCommand cmd = new OleDbCommand(sql, conn);
        cmd.ExecuteNonQuery();
    }
    // 5.SQL 문을 실행하고, SqlDataReader 객체를 리턴합니다.
    public OleDbDataReader ExecuteReader(string sql)
    {
        OleDbCommand cmd = new OleDbCommand(sql, conn);
        return cmd.ExecuteReader();
    }
    // 6.SQL 문을 실행하고, DataSet 객체를 리턴합니다.
    public DataSet GetDataSet(string sql)
    {
        OleDbDataAdapter adapter = new OleDbDataAdapter();
        adapter.SelectCommand = new OleDbCommand(sql, conn);

        DataSet ds = new DataSet();
        adapter.Fill(ds);

        return ds;
    }
}
