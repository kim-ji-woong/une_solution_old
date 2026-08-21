package g1Weather.service.SearchSnowOption;

import g1Weather.service.SearchSnow;

public class SearchSnowPeriod extends SearchSnow {
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
	private String daySum = UnknownResult;
	private String snowDate = UnknownResult;
	
	public SearchSnowPeriod()
	{
		for (int i=0;i<=23;i++)
		{
			itemValues.add(new SnowData());
		}
		
		SnowData sum = new SnowData();
		sum.setSumData(true);
		itemValues.add(sum);
	}
	
	private void postProcess(double snow)
	{
	}
	
	private void setItemValue(String value, int index)
	{
		SnowData snow = this.itemValues.get(index);
		snow.setValue(value);
	}
	
	private String setHour(String snowHour, int index)
	{
		SomeType<Double> snow = new SomeType<Double>(0.0);
		String result = UnknownResult;
		
		if (tryParseDouble(snowHour, snow) == false)
			result = UnknownResult;
		else
		{
			if (snow.getValue() < 0.0)
				result = UnknownResult;
			else
			{
				result = DoubleToString(snow.getValue() * 0.1);
				postProcess(snow.getValue() * 0.1);
			}
		}
		
		setItemValue(result, index);
		return result;
	}
	
	public String getH00()
	{
		return h00;
	}
	
	public void setH00(String snowHour)
	{
		this.h00 = setHour(snowHour, 0);
	}
	
	public String getH01()
	{
		return h01;
	}
	
	public void setH01(String snowHour)
	{
		this.h01 = setHour(snowHour, 1);
		/*SomeType<Double> snow = new SomeType<Double>(0.0);
		
		if (tryParseDouble(snowHour, snow) == false)
			this.h01 = UnknownResult;
		else
		{
			if (snow.getValue() < 0.0)
				this.h01 = UnknownResult;
			else
			{
				this.h01 = Double.toString(snow.getValue());
				postProcess(snow.getValue());
			}
		}
		
		setItemValue(this.h01, 0);*/
	}
	
	public String getH02()
	{
		return h02;
	}
	
	public void setH02(String snowHour)
	{
		this.h02 = setHour(snowHour, 2);
	}
	
	public String getH03()
	{
		return h03;
	}
	
	public void setH03(String snowHour)
	{
		this.h03 = setHour(snowHour, 3);
	}
	
	public String getH04()
	{
		return h04;
	}
	
	public void setH04(String snowHour)
	{
		this.h04 = setHour(snowHour, 4);
	}
	
	public String getH05()
	{
		return h05;
	}
	
	public void setH05(String snowHour)
	{
		this.h05 = setHour(snowHour, 5);
	}
	
	public String getH06()
	{
		return h06;
	}
	
	public void setH06(String snowHour)
	{
		this.h06 = setHour(snowHour, 6);
	}
	
	public String getH07()
	{
		return h07;
	}
	
	public void setH07(String snowHour)
	{
		this.h07 = setHour(snowHour, 7);
	}
	
	public String getH08()
	{
		return h08;
	}
	
	public void setH08(String snowHour)
	{
		this.h08 = setHour(snowHour, 8);
	}
	
	public String getH09()
	{
		return h09;
	}
	
	public void setH09(String snowHour)
	{
		this.h09 = setHour(snowHour, 9);
	}
	
	public String getH10()
	{
		return h10;
	}
	
	public void setH10(String snowHour)
	{
		this.h10 = setHour(snowHour, 10);
	}
	
	public String getH11()
	{
		return h11;
	}
	
	public void setH11(String snowHour)
	{
		this.h11 = setHour(snowHour, 11);
	}
	
	public String getH12()
	{
		return h12;
	}
	
	public void setH12(String snowHour)
	{
		this.h12 = setHour(snowHour, 12);
	}
	
	public String getH13()
	{
		return h13;
	}
	
	public void setH13(String snowHour)
	{
		this.h13 = setHour(snowHour, 13);
	}
	
	public String getH14()
	{
		return h14;
	}
	
	public void setH14(String snowHour)
	{
		this.h14 = setHour(snowHour, 14);
	}
	
	public String getH15()
	{
		return h15;
	}
	
	public void setH15(String snowHour)
	{
		this.h15 = setHour(snowHour, 15);
	}
	
	public String getH16()
	{
		return h16;
	}
	
	public void setH16(String snowHour)
	{
		this.h16 = setHour(snowHour, 16);
	}
	
	public String getH17()
	{
		return h17;
	}
	
	public void setH17(String snowHour)
	{
		this.h17 = setHour(snowHour, 17);
	}
	
	public String getH18()
	{
		return h18;
	}
	
	public void setH18(String snowHour)
	{
		this.h18 = setHour(snowHour, 18);
	}
	
	public String getH19()
	{
		return h19;
	}
	
	public void setH19(String snowHour)
	{
		this.h19 = setHour(snowHour, 19);
	}
	
	public String getH20()
	{
		return h20;
	}
	
	public void setH20(String snowHour)
	{
		this.h20 = setHour(snowHour, 20);
	}
	
	public String getH21()
	{
		return h21;
	}
	
	public void setH21(String snowHour)
	{
		this.h21 = setHour(snowHour, 21);
	}
	
	public String getH22()
	{
		return h22;
	}
	
	public void setH22(String snowHour)
	{
		this.h22 = setHour(snowHour, 22);
	}
	
	public String getH23()
	{
		return h23;
	}
	
	public void setH23(String snowHour)
	{
		this.h23 = setHour(snowHour, 23);
	}
	
	public String getDaySum()
	{
		return daySum;
	}
	
	public void setDaySum(String daySum)
	{
		SomeType<Double> num = new SomeType<Double>(0.0);
		
		// 소수점 자리수를 맞추기 위함
		if (tryParseDouble(daySum, num))
		{
			daySum = DoubleToString(num.getValue() * 0.1);
		}
		
		this.daySum = daySum;
		setItemValue(this.daySum, 24);
	}
	
	public String getSnowDate()
	{
		return snowDate;
	}
	
	public void setSnowDate(String date)
	{
		this.snowDate = date;
	}
}
