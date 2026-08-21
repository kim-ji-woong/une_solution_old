package g1Weather.service.SearchWaterLevelOption;

import g1Weather.service.SearchWaterLevel;

public class SearchWaterLevelDay extends SearchWaterLevel {
	private String h00 = UnknownResult;
	private String h01 = UnknownResult;
	private String h02 = UnknownResult;
	private String h03 = UnknownResult;
	private String h04 = UnknownResult;
	private String h05 = UnknownResult;
	private String h06 = UnknownResult;
	private String h07 = UnknownResult;
	private String h08 = UnknownResult;
	private String h09 = UnknownResult;
	private String h10 = UnknownResult;
	private String h11 = UnknownResult;
	private String h12 = UnknownResult;
	private String h13 = UnknownResult;
	private String h14 = UnknownResult;
	private String h15 = UnknownResult;
	private String h16 = UnknownResult;
	private String h17 = UnknownResult;
	private String h18 = UnknownResult;
	private String h19 = UnknownResult;
	private String h20 = UnknownResult;
	private String h21 = UnknownResult;
	private String h22 = UnknownResult;
	private String h23 = UnknownResult;
	
	private String waterLevelDate = UnknownResult;
	//private String avg = UnknownResult;
	//private String daySum = UnknownResult;
	//private String max = UnknownResult;
	
	//private double totalHourValue = 0.0;
	//private int hourCount = 0;
	//private double maxValue = 0.0;
	
	public SearchWaterLevelDay()
	{
		for (int i=0;i<=23;i++)
		{
			itemValues.add(new WaterLevelData());
		}
		
		/*WaterLevelData today = new WaterLevelData();
		today.setSumData(true);
		itemValues.add(today);
		
		// 최대값 사용하지 않는다.
		WaterLevelData maxHour = new WaterLevelData();
		maxHour.setSumData(true);
		itemValues.add(maxHour);*/
	}
	
	// 평균값과 최대값을 계산한다.
	private void postProcess(double waterLevel)
	{
		//totalHourValue += waterLevel;
		//hourCount++;
		//this.avg = Double.toString(totalHourValue / hourCount);

		/*if (maxValue <= waterLevel)
		{
			maxValue = waterLevel;
			setMax(DoubleToString(maxValue));
		}*/
	}
	
	private void setItemValue(String value, int index)
	{
		WaterLevelData waterLevel = this.itemValues.get(index);
		waterLevel.setValue(value);
	}
	
	public String getH00()
	{
		return h00;
	}
	
	public void setH00(String waterLevelHour)
	{
		this.h00 = setHour(waterLevelHour, 0);
	}
	
	public String getH01()
	{
		return h01;
	}
	
	private String setHour(String waterLevelHour, int index)
	{
		SomeType<Double> waterLevel = new SomeType<Double>(0.0);
		String result = UnknownResult;
		
		if (tryParseDouble(waterLevelHour, waterLevel) == false)
			result = UnknownResult;
		else
		{
			if (waterLevel.getValue() < 0.0)
				result = UnknownResult;
			else
			{
				result = DoubleToString(waterLevel.getValue() * 0.1);
				postProcess(waterLevel.getValue() * 0.1);
			}
		}
		
		setItemValue(result, index);
		return result;
	}
	
	public void setH01(String waterLevelHour)
	{
		this.h01 = setHour(waterLevelHour, 1);
	}
	
	public String getH02()
	{
		return h02;
	}
	
	public void setH02(String waterLevelHour)
	{
		this.h02 = setHour(waterLevelHour, 2);
	}
	
	public String getH03()
	{
		return h03;
	}
	
	public void setH03(String waterLevelHour)
	{
		this.h03 = setHour(waterLevelHour, 3);
	}
	
	public String getH04()
	{
		return h04;
	}
	
	public void setH04(String waterLevelHour)
	{
		this.h04 = setHour(waterLevelHour, 4);
	}
	
	public String getH05()
	{
		return h05;
	}
	
	public void setH05(String waterLevelHour)
	{
		this.h05 = setHour(waterLevelHour, 5);
	}
	
	public String getH06()
	{
		return h06;
	}
	
	public void setH06(String waterLevelHour)
	{
		this.h06 = setHour(waterLevelHour, 6);
	}
	
	public String getH07()
	{
		return h07;
	}
	
	public void setH07(String waterLevelHour)
	{
		this.h07 = setHour(waterLevelHour, 7);
	}
	
	public String getH08()
	{
		return h08;
	}
	
	public void setH08(String waterLevelHour)
	{
		this.h08 = setHour(waterLevelHour, 8);
	}
	
	public String getH09()
	{
		return h09;
	}
	
	public void setH09(String waterLevelHour)
	{
		this.h09 = setHour(waterLevelHour, 9);
	}
	
	public String getH10()
	{
		return h10;
	}
	
	public void setH10(String waterLevelHour)
	{
		this.h10 = setHour(waterLevelHour, 10);
	}
	
	public String getH11()
	{
		return h11;
	}
	
	public void setH11(String waterLevelHour)
	{
		this.h11 = setHour(waterLevelHour, 11);
	}
	
	public String getH12()
	{
		return h12;
	}
	
	public void setH12(String waterLevelHour)
	{
		this.h12 = setHour(waterLevelHour, 12);
	}
	
	public String getH13()
	{
		return h13;
	}
	
	public void setH13(String waterLevelHour)
	{
		this.h13 = setHour(waterLevelHour, 13);
	}
	
	public String getH14()
	{
		return h14;
	}
	
	public void setH14(String waterLevelHour)
	{
		this.h14 = setHour(waterLevelHour, 14);
	}
	
	public String getH15()
	{
		return h15;
	}
	
	public void setH15(String waterLevelHour)
	{
		this.h15 = setHour(waterLevelHour, 15);
	}
	
	public String getH16()
	{
		return h16;
	}
	
	public void setH16(String waterLevelHour)
	{
		this.h16 = setHour(waterLevelHour, 16);
	}
	
	public String getH17()
	{
		return h17;
	}
	
	public void setH17(String waterLevelHour)
	{
		this.h17 = setHour(waterLevelHour, 17);
	}
	
	public String getH18()
	{
		return h18;
	}
	
	public void setH18(String waterLevelHour)
	{
		this.h18 = setHour(waterLevelHour, 18);
	}
	
	public String getH19()
	{
		return h19;
	}
	
	public void setH19(String waterLevelHour)
	{
		this.h19 = setHour(waterLevelHour, 19);
	}
	
	public String getH20()
	{
		return h20;
	}
	
	public void setH20(String waterLevelHour)
	{
		this.h20 = setHour(waterLevelHour, 20);
	}
	
	public String getH21()
	{
		return h21;
	}
	
	public void setH21(String waterLevelHour)
	{
		this.h21 = setHour(waterLevelHour, 21);
	}
	
	public String getH22()
	{
		return h22;
	}
	
	public void setH22(String waterLevelHour)
	{
		this.h22 = setHour(waterLevelHour, 22);
	}
	
	public String getH23()
	{
		return h23;
	}
	
	public void setH23(String waterLevelHour)
	{
		this.h23 = setHour(waterLevelHour, 23);
	}
	
	/*public String getDaySum()
	{
		return daySum;
	}
	
	public void setDaySum(String daySum)
	{
		SomeType<Double> num = new SomeType<Double>(0.0);
		
		// 소수점 자리수를 맞추기 위함
		if (tryParseDouble(daySum, num))
		{
			daySum = DoubleToString(num.getValue());
		}
		
		this.daySum = daySum;
		setItemValue(this.daySum, 23);
	}*/
	
	/*public String getAvg()
	{
		return this.avg;
	}*/
	
	/*public String getMax()
	{
		return this.max;
	}
	
	public void setMax(String max)
	{
		SomeType<Double> num = new SomeType<Double>(0.0);
		
		// 소수점 자리수를 맞추기 위함
		if (tryParseDouble(max, num))
		{
			max = DoubleToString(num.getValue());
		}
		
		this.max = max;
		//setItemValue(this.max, 24);
	}*/
	
	public String getWaterLevelDate()
	{
		return waterLevelDate;
	}
	
	public void setWaterLevelDate(String date)
	{
		this.waterLevelDate = date;
	}
}
