package g1Weather.webService;

import java.io.InputStream;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
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

public class SpecialNewsXMLParser {
	public static List<SpecialNews> parse(String xml) throws Exception
	{
		InputStream stream = IOUtils.toInputStream(xml, "UTF-8");
		
		DocumentBuilderFactory dbFactory = DocumentBuilderFactory.newInstance();
		DocumentBuilder dBuilder = dbFactory.newDocumentBuilder();
		Document doc = dBuilder.parse(stream);
		
		NodeList nodeList = doc.getElementsByTagName("item");
		
		if (nodeList == null)
		{
			stream.close();
			return null;
		}
		
		DateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd");
		String today = dateFormat.format(new Date());
		
		// Key : 특보 Type String
		// Value : 같은 특보를 공유하는 지역 목록
		HashMap<String, SpecialNews> mapNews = new HashMap<String, SpecialNews>();
		
		int nodeCount = nodeList.getLength();
		
		for (int i=0;i<nodeCount;i++)
		{
			Node node = nodeList.item(i);
			
			if (node.getNodeType() == Node.ELEMENT_NODE)
			{
				SpecialNews news = readElement((Element)node);
				
				// 오늘의 특보만 게시한다.
				if (news.isToday(today) == false)
					continue;
				
				// 강원도 이외의 지역은 무시한다.
				if (news.isG1Area() == false)
					continue;
				
				String key = makeNewsKey(news);
				
				SpecialNews value = mapNews.get(key);
				
				if (value == null)
				{
					value = new SpecialNews();
					
					value.setCommand(news.getCommand());
					value.setStartTime(news.getStartTime());
					value.setEndTime(news.getEndTime());
					value.setWarnStress(news.getWarnStress());
					value.setWarnVar(news.getWarnVar());
					
					mapNews.put(key,  value);
				}
				
				if (value.getAreaName().length() == 0)
					value.setAreaName(news.getAreaName());
				else
					value.setAreaName(value.getAreaName() + ", " + news.getAreaName());
			}
		}
		
		stream.close();
		
		// 시간순으로 정렬한다.
		List<SpecialNews> newsList = new ArrayList(mapNews.values());
		Collections.sort(newsList);
		
		return newsList;
	}
	
	private static String makeNewsKey(SpecialNews news)
	{
		String key = "";
		int warnType = news.getWarnVar();
		int warnStress = news.getWarnStress();
		int command = news.getCommand();
		
		if (warnType < 0 || warnStress < 0 || command < 0)
			return key;
		
		if (news.getStartTime().length() > 0)
			key = "S;" + news.getStartTime();
		else if (news.getEndTime().length() > 0)
			key = "E;" + news.getEndTime();
		else
			return key;
		
		key += ";" + Integer.toString(warnType) +";" + Integer.toString(warnStress) + ";" + Integer.toString(command);
		return key;
	}
	
	private static SpecialNews readElement(Element element)
	{
		NodeList nodeList = element.getChildNodes();
		
		if (nodeList == null)
			return null;
		
		SpecialNews news = new SpecialNews();
		int nodeCount = nodeList.getLength();
		
		for (int i=0;i<nodeCount;i++)
		{
			Node node = nodeList.item(i);
			
			if (node.getNodeType() == Node.ELEMENT_NODE)
			{
				Node text = node.getFirstChild();
				
				if (text == null)
					continue;
				
				String nodeName = node.getNodeName();
				String nodeValue = text.getNodeValue();
				//String nodeValue = text.getTextContent();
				
				if (nodeName == "areaCode")
					news.setAreaCode(nodeValue);
				else if (nodeName == "areaName")
					news.setAreaName(nodeValue);
				else if (nodeName == "command")
				{
					int cmd = toInt(nodeValue);
					
					if (cmd >= 0)
						news.setCommand(cmd);
				}
				else if (nodeName == "startTime")
					news.setStartTime(nodeValue);
				else if (nodeName == "endTime")
					news.setEndTime(nodeValue);
				else if (nodeName == "warnStress")
				{
					int stress = toInt(nodeValue);
					
					if (stress >= 0)
						news.setWarnStress(stress);
				}
				else if (nodeName == "warnVar")
				{
					int type = toInt(nodeValue);
					
					if (type >= 0)
						news.setWarnVar(type);
				}
			}
		}
		
		return news;
	}
	
	private static int toInt(String value)
	{
		try
		{
			int num = Integer.parseInt(value);
			return num;
		}
		catch (Exception e)
		{
		}
		
		return -1;
	}
}
