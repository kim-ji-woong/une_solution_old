

import org.scidb.jdbc.IResultSetWrapper;

import java.io.IOException;
import java.sql.DriverManager;
import java.sql.Connection;
import java.sql.ResultSet;
import java.sql.ResultSetMetaData;
import java.sql.SQLException;
import java.sql.Statement;

class SciDBClient
{
  public static void main(String [] args) throws IOException
  {
    try
    {
      Class.forName("org.scidb.jdbc.Driver");
    }
    catch (ClassNotFoundException e)
    {
      System.out.println("Driver is not in the CLASSPATH -> " + e);
    }

    try
    {
      Connection conn = DriverManager.getConnection("jdbc:scidb://192.168.0.112/");
      Statement st = conn.createStatement();
      // create array A<a:string>[x=0:2,3,0, y=0:2,3,0];
      // select * into A from
      // array(A, '[["a","b","c"]["d","e","f"]["123","456","789"]]');
     
      //Statement st5 = conn.createStatement();
      //st5.execute("drop array A");
      //st5.close();
      
      //st.execute("create array A <a:string>[x=0:2,6,0, y=0:2,6,0]");
      //st.close();
      
      //Statement st2 = conn.createStatement();
      //st2.execute("insert INTO A '[[a,b,c],[d,e,f],[12,456,789]]'");
      //st2.execute("insert INTO A '[[(\"a\"),(\"b\"),(\"c\")],[(\"d\"),(\"e\"),(\"f\")],[(\"123\"),(\"456\"),(\"789\")]]'");
      //st2.close();
      
      
      /*
      ResultSet res = st.executeQuery("select * from array(<a:string>[x=0:2,3,0, y=0:2,3,0], '[[\"a\",\"b\",\"c\"][\"d\",\"e\",\"f\"][\"123\",\"456\",\"789\"]]')");
      ResultSetMetaData meta = res.getMetaData();

      System.out.println("Source array name: " + meta.getTableName(0));
      System.out.println(meta.getColumnCount() + " columns:");

      IResultSetWrapper resWrapper = res.unwrap(IResultSetWrapper.class);
      for (int i = 1; i <= meta.getColumnCount(); i++)
      {
        System.out.println(meta.getColumnName(i) + " - " + meta.getColumnTypeName(i)
           + " - is attribute:" + resWrapper.isColumnAttribute(i));
      }
      System.out.println("=====");

      System.out.println("x y a");
      System.out.println("-----");
      while(!res.isAfterLast())
      {
        System.out.println(res.getLong("x") + " " + res.getLong("y") + " "
           + res.getString("a"));
        res.next();
      }
      */
      Statement st3 = conn.createStatement();
      ResultSet res2 = st3.executeQuery("select * from A");
      ResultSetMetaData meta2 = res2.getMetaData();

      System.out.println("Source array name: " + meta2.getTableName(0));
      System.out.println(meta2.getColumnCount() + " columns:");

      IResultSetWrapper resWrapper2 = res2.unwrap(IResultSetWrapper.class);
      for (int i = 1; i <= meta2.getColumnCount(); i++)
      {
        System.out.println(meta2.getColumnName(i) + " - " + meta2.getColumnTypeName(i)
           + " - is attribute:" + resWrapper2.isColumnAttribute(i));
      }
      System.out.println("=====");

      System.out.println("x y a");
      System.out.println("-----");
      while(!res2.isAfterLast())
      {
        System.out.println(res2.getLong("x") + " " + res2.getLong("y") + " "
           + res2.getString("a"));
        res2.next();
      }
    }
    catch (SQLException e)
    {
      System.out.println(e);
    }
  System.exit(0);
  }
}