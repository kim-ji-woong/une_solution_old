package g1Weather.service.SearchRainOption;

import g1Weather.service.SearchRain;

public class SearchRainPeriod extends SearchRain {
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
	private String rainDate = UnknownResult;
	
	public SearchRainPeriod()
	{
		for (int i=0;i<=23;i++)
		{
			itemValues.add(new RainData());
		}
		
		RainData sum = new RainData();
		sum.setSumData(true);
		itemValues.add(sum);
	}
	
	private void postProcess(double rain)
	{
	}
	
	private void setItemValue(String value, int index)
	{
		RainData rain = this.itemValues.get(index);
		rain.setValue(value);
	}
	
	private String setHour(String rainHour, int index)
	{
		SomeType<Double> rain = new SomeType<Double>(0.0);
		String result = UnknownResult;
		
		if (tryParseDouble(rainHour, rain) == false)
			result = UnknownResult;
		else
		{
			if (rain.getValue() < 0.0)
				result = UnknownResult;
			else
			{
				result = DoubleToString(rain.getValue() * 0.1);
				postProcess(rain.getValue() * 0.1);
			}
		}
		
		setItemValue(result, index);
		return result;
	}
	
	public String getH00()
	{
		return h00;
	}
	
	public void setH00(String rainHour)
	{
		this.h00 = setHour(rainHour, 0);
		/*SomeType<Double> rain = new SomeType<Double>(0.0);
		
		if (tryParseDouble(rainHour, rain) == false)
			this.h00 = UnknownResult;
		else
		{
			if (rain.getValue() < 0.0)
				this.h00 = UnknownResult;
			else
			{
				this.h00 = Double.toString(rain.getValue());
				postProcess(rain.getValue());
			}
		}
		
		setItemValue(this.h00, 0);*/
	}
	
	public String getH01()
	{
		return h01;
	}
	
	public void setH01(String rainHour)
	{
		this.h01 = setHour(rainHour, 1);
	}
	
	public String getH02()
	{
		return h02;
	}
	
	public void setH02(String rainHour)
	{
		this.h02 = setHour(rainHour, 2);
	}
	
	public String getH03()
	{
		return h03;
	}
	
	public void setH03(String rainHour)
	{
		this.h03 = setHour(rainHour, 3);
	}
	
	public String getH04()
	{
		return h04;
	}
	
	public void setH04(String rainHour)
	{
		this.h04 = setHour(rainHour, 4);
	}
	
	public String getH05()
	{
		return h05;
	}
	
	public void setH05(String rainHour)
	{
		this.h05 = setHour(rainHour, 5);
	}
	
	public String getH06()
	{
		return h06;
	}
	
	public void setH06(String rainHour)
	{
		this.h06 = setHour(rainHour, 6);
	}
	
	public String getH07()
	{
		return h07;
	}
	
	public void setH07(String rainHour)
	{
		this.h07 = setHour(rainHour, 7);
	}
	
	public String getH08()
	{
		return h08;
	}
	
	public void setH08(String rainHour)
	{
		this.h08 = setHour(rainHour, 8);
	}
	
	public String getH09()
	{
		return h09;
	}
	
	public void setH09(String rainHour)
	{
		this.h09 = setHour(rainHour, 9);
	}
	
	public String getH10()
	{
		return h10;
	}
	
	public void setH10(String rainHour)
	{
		this.h10 = setHour(rainHour, 10);
	}
	
	public String getH11()
	{
		return h11;
	}
	
	public void setH11(String rainHour)
	{
		this.h11 = setHour(rainHour, 11);
	}
	
	public String getH12()
	{
		return h12;
	}
	
	public void setH12(String rainHour)
	{
		this.h12 = setHour(rainHour, 12);
	}
	
	public String getH13()
	{
		return h13;
	}
	
	public void setH13(String rainHour)
	{
		this.h13 = setHour(rainHour, 13);
	}
	
	public String getH14()
	{
		return h14;
	}
	
	public void setH14(String rainHour)
	{
		this.h14 = setHour(rainHour, 14);
	}
	
	public String getH15()
	{
		return h15;
	}
	
	public void setH15(String rainHour)
	{
		this.h15 = setHour(rainHour, 15);
	}
	
	public String getH16()
	{
		return h16;
	}
	
	public void setH16(String rainHour)
	{
		this.h16 = setHour(rainHour, 16);
	}
	
	public String getH17()
	{
		return h17;
	}
	
	public void setH17(String rainHour)
	{
		this.h17 = setHour(rainHour, 17);
	}
	
	public String getH18()
	{
		return h18;
	}
	
	public void setH18(String rainHour)
	{
		this.h18 = setHour(rainHour, 18);
	}
	
	public String getH19()
	{
		return h19;
	}
	
	public void setH19(String rainHour)
	{
		this.h19 = setHour(rainHour, 19);
	}
	
	public String getH20()
	{
		return h20;
	}
	
	public void setH20(String rainHour)
	{
		this.h20 = setHour(rainHour, 20);
	}
	
	public String getH21()
	{
		return h21;
	}
	
	public void setH21(String rainHour)
	{
		this.h21 = setHour(rainHour, 21);
	}
	
	public String getH22()
	{
		return h22;
	}
	
	public void setH22(String rainHour)
	{
		this.h22 = setHour(rainHour, 22);
	}
	
	public String getH23()
	{
		return h23;
	}
	
	public void setH23(String rainHour)
	{
		this.h23 = setHour(rainHour, 23);
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
	
	public String getRainDate()
	{
		return rainDate;
	}
	
	public void setRainDate(String date)
	{
		this.rainDate = date;
	}
}
