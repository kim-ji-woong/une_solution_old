package g1Weather.webService;

import java.io.InputStream;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Date;
import java.util.HashMap;
import java.util.List;

import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.DocumentBuilder;

import org.apache.commons.io.IOUtils;
import org.w3c.dom.Document;
import org.w3c.dom.NodeList;
import org.w3c.dom.Node;
import org.w3c.dom.Element;

public class RadarImageURLParser
{
	public static String getCurrentURL(String serviceKey) throws Exception
	{
		try
		{
			DateFormat dateFormat = new SimpleDateFormat("yyyyMMdd");
			String today = dateFormat.format(new java.util.Date());
			
	        String radarURL = "http://newsky2.kma.go.kr/FileService/RadarVideoInfoService/RadarCompositionImage?data=CMB&time=";
	        radarURL += today + "&ServiceKey=" + serviceKey;
	        
	        java.net.URL url = new java.net.URL(radarURL);
			java.net.HttpURLConnection connection = (java.net.HttpURLConnection)url.openConnection();
			
	        java.io.InputStream is = connection.getInputStream();
	        java.util.Scanner scan = new java.util.Scanner(is);
	        
	        String result = "";
	        while (scan.hasNext())
	        {
	        	String str = scan.nextLine();
	        	result += str;
	        }
	        scan.close();
	        is.close();
	        
	        return parseXML(result);
		}
		catch (Exception e)
		{
		}
		
		return "";
	}
	
	private static String parseXML(String xml) throws Exception
	{
		InputStream stream = IOUtils.toInputStream(xml, "UTF-8");
		
		DocumentBuilderFactory dbFactory = DocumentBuilderFactory.newInstance();
		DocumentBuilder dBuilder = dbFactory.newDocumentBuilder();
		Document doc = dBuilder.parse(stream);
		
		NodeList nodeList = doc.getElementsByTagName("rdr-img-file");
		
		if (nodeList == null)
		{
			stream.close();
			return "";
		}
		
		int nodeCount = nodeList.getLength();
		
		if (nodeCount == 0)
			return "";
		
		for (int i=nodeCount-1;i>=0;i--)
		{
			Node node = nodeList.item(i);
			
			if (node.getNodeType() == Node.ELEMENT_NODE)
			{
				Node text = node.getFirstChild();
				
				if (text == null)
					continue;
				
				String url = text.getNodeValue();
				//String url = text.getTextContent();
				
				if (url != null && url.length() > 0)
				{
					stream.close();
					return url;
				}
			}
		}
		
		return "";
	}
}
