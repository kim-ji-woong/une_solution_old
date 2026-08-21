package g1Weather.webService;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

/*
 * 기상특보
 */
public class SpecialNews implements Comparable<SpecialNews> {
	/*  Key : 지역코드
	* Value : 지역명
	*/
	private static HashMap<String, String> mapG1AreaCode = InitG1AreaCodes();
	
	/*  Key : 특보종류 코드
	* Value : 특보종류
	*/
	private static HashMap<Integer, String> mapWarningVar = InitWarningVar();
	
	/*  Key : 특보강도 코드
	* Value : 특보강도
	*/
	private static HashMap<Integer, String> mapWarningStress = InitWarningStress();
	
	/*  Key : 특보발표 코드
	* Value : 특보발표
	*/
	private static HashMap<Integer, String> mapCommand = InitCommand();
	
	/*  Key : 특보발표 코드
	* Value : 특보발표
	*/
	private static List<String> g1Cities = InitG1Cities();
	
	private static List<String> InitG1Cities()
	{
		List<String> cities = new ArrayList();
		
		cities.add("춘천");
		cities.add("원주");
		cities.add("강릉");
		cities.add("동해");
		cities.add("태백");
		cities.add("속초");
		cities.add("삼척");
		cities.add("홍천");
		cities.add("횡성");
		cities.add("영월");
		cities.add("평창");
		cities.add("정선");
		cities.add("철원");
		cities.add("화천");
		cities.add("양구");
		cities.add("인제");
		cities.add("고성");
		cities.add("양양");
		cities.add("강원");
		
		return cities;
	}

	private static HashMap<String, String> InitG1AreaCodes()
	{
		HashMap<String, String> map = new HashMap<String, String>();
		
		map.put("L1020100", "강릉시");
		map.put("L1020110", "강릉시평지");
		map.put("L1020120", "강릉시산간");
		map.put("L1020200", "동해시");
		map.put("L1020210", "동해시평지");
		map.put("L1020220", "동해시산간");
		map.put("L1020300", "태백시");
		map.put("L1020400", "삼척시");
		map.put("L1020410", "삼척시평지");
		map.put("L1020420", "삼척시산간");
		map.put("L1020500", "속초시");
		map.put("L1020510", "속초시평지");
		map.put("L1020520", "속초시산간");
		map.put("S1151200", "강원중부앞바다");
		map.put("S1151000", "동해중부앞바다");
		map.put("L1020600", "고성군");
		map.put("L1020610", "고성군평지");
		map.put("L1020620", "고성군산간");
		map.put("L1020700", "양양군");
		map.put("L1020710", "양양군평지");
		map.put("L1020720", "양양군산간");
		map.put("L1020800", "영월군");
		map.put("L1020900", "평창군");
		map.put("L1020910", "평창군평지");
		map.put("L1020920", "평창군산간");
		map.put("L1021000", "정선군");
		map.put("L1021010", "정선군평지");
		map.put("L1021020", "정선군산간");
		map.put("S1151300", "강원남부앞바다");
		map.put("S1152000", "동해중부먼바다");
		map.put("L1021100", "횡성군");
		map.put("L1021200", "원주시");
		map.put("L1021300", "철원군");
		map.put("L1021400", "화천군");
		map.put("L1021500", "홍천군");
		map.put("L1021510", "홍천군평지");
		map.put("L1021520", "홍천군산간");
		map.put("L1021600", "춘천시");
		map.put("L1021700", "양구군");
		map.put("L1021800", "인제군");
		map.put("L1021810", "인제군평지");
		map.put("L1021820", "인제군산간");
		map.put("S1151100", "강원북부앞바다");
		map.put("S1150000", "동해중부전해상");

		return map;
	}
	
	private static HashMap<Integer, String> InitWarningVar()
	{
		HashMap<Integer, String> map = new HashMap<Integer, String>();
		
		map.put(1,  "강풍");
		map.put(2,  "호우");
		map.put(3,  "한파");
		map.put(4,  "건조");
		map.put(5,  "해일");
		map.put(6,  "풍랑");
		map.put(7,  "태풍");
		map.put(8,  "대설");
		map.put(9,  "황사");
		
		return map;
	}
	
	private static HashMap<Integer, String> InitWarningStress()
	{
		HashMap<Integer, String> map = new HashMap<Integer, String>();
		
		map.put(0,  "주의보");
		map.put(1,  "경보");
		
		return map;
	}
	
	private static HashMap<Integer, String> InitCommand()
	{
		HashMap<Integer, String> map = new HashMap<Integer, String>();
		
		map.put(1,  "발표");
		map.put(2,  "해제");
		map.put(3,  "연장");
		map.put(4,  "대치에 의한 해제");
		map.put(5,  "대치에 의한 발표");
		map.put(6,  "정정");
		
		return map;
	}
	
	private String areaCode = "";
	private String areaName = "";
	private String startTime = "";
	private String endTime = "";
	private int warnVar = -1;
	private int warnStress = -1;
	private int command = -1;
	
	private String time = "";
	private String newsType = "";
	private String commandString = "";
	private boolean emptyData = false;
	
	public String getAreaCode()
	{
		return this.areaCode;
	}
	
	public void setAreaCode(String areaCode)
	{
		this.areaCode = areaCode;
	}
	
	public String getAreaName()
	{
		return this.areaName;
	}
	
	public void setAreaName(String areaName)
	{
		this.areaName = areaName;
	}
	
	public String getStartTime()
	{
		return this.startTime;
	}
	
	public void setStartTime(String startTime)
	{
		this.startTime = startTime;
		
		if (startTime.length() > 0)
			setTime(startTime);
	}
	
	public String getEndTime()
	{
		return this.endTime;
	}
	
	public void setEndTime(String endTime)
	{
		this.endTime = endTime;
		
		if (endTime.length() > 0)
			setTime(endTime);
	}
	
	private void setTime(String value)
	{
		String year = "", month = "", day = "";
		String hour = "", min = "", sec = "";
		int len = value.length();
		
		if (len >= 8)
		{
			year = value.substring(0, 4);
			month = value.substring(4, 6);
			day = value.substring(6, 8);
		}
		
		if (len >= 12)
		{
			hour = value.substring(8, 10);
			min = value.substring(10, 12);
		}
		
		if (len >= 14)
			sec = value.substring(12, 14);
		
		if (sec.length() > 0)
			time = year + "-" + month + "-" + day + " " + hour + ":" + min + ":" + sec;
		else if (min.length() > 0)
			time = year + "-" + month + "-" + day + " " + hour + ":" + min;
		else if (day.length() > 0)
			time = year + "-" + month + "-" + day;
		else
			time = value;
	}
	
	public int getWarnVar()
	{
		return this.warnVar;
	}
	
	public void setWarnVar(int warnVar)
	{
		this.warnVar = warnVar;
		makeNewsType();
	}
	
	public int getWarnStress()
	{
		return this.warnStress;
	}
	
	public void setWarnStress(int warnStress)
	{
		this.warnStress = warnStress;
		makeNewsType();
	}
	
	public int getCommand()
	{
		return this.command;
	}
	
	public void setCommand(int command)
	{
		this.command = command;
		commandString = mapCommand.get(this.command);
		
		if (commandString == null)
			commandString = "";
	}
	
	public boolean isG1Area()
	{
		for (String cityName : g1Cities)
		{
			if (this.areaName.startsWith(cityName))
				return true;
		}
		
		return false;
		/*String areaName = mapG1AreaCode.get(this.areaCode);
		return areaName != null;*/
	}
	
	public String getTime()
	{
		return time;
	}
	
	public String getCommandString()
	{
		return commandString;
	}
	
	public String getNewsType()
	{
		return newsType;
	}
	
	private void makeNewsType()
	{
		String type = mapWarningVar.get(this.warnVar);
		String stress = mapWarningStress.get(this.warnStress);
		
		if (type != null && stress != null)
			newsType = type + stress;
	}
	
	public boolean isToday(String today)
	{
		if (time.length() >= 10)
		{
			String date = time.substring(0, 10);
			
			if (date.equals(today))
				return true;
		}
		
		return false;
	}
	
	public void setEmptyData(boolean isEmpty)
	{
		this.emptyData = isEmpty;
	}
	
	public boolean getEmptyData()
	{
		return this.emptyData;
	}
	
	public int compareTo(SpecialNews news)
	{
		return this.time.compareTo(news.time);
	}
}
