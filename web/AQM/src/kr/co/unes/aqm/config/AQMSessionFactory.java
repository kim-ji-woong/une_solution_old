package kr.co.unes.aqm.config;

import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.Reader;

import org.apache.ibatis.io.Resources;
import org.apache.ibatis.session.SqlSessionFactoryBuilder;
import org.apache.ibatis.session.SqlSessionFactory;

public class AQMSessionFactory {

	private static SqlSessionFactory sqlSessionFactory; 
	
	public static void initSesseionFactory()
	{ 		
		try 
		{ 
			String resource = "kr/co/unes/aqm/config/mybatis-config.xml";
			Reader reader = Resources.getResourceAsReader(resource); 
			if (sqlSessionFactory == null)
			{ 
				sqlSessionFactory = new SqlSessionFactoryBuilder().build(reader); 
			} 
		}
		catch (FileNotFoundException fileNotFoundException) 
		{
			fileNotFoundException.printStackTrace(); 
		}
		catch (IOException iOException)
		{
			iOException.printStackTrace(); 
		}
	}
	
	public static SqlSessionFactory getSqlSessionFactory() {
		return sqlSessionFactory; 
	}
}
