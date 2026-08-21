package g1Weather.service;

import org.apache.commons.lang3.builder.ToStringBuilder;

public class PageVO {
	protected class SomeType<T>
	{
		private T value;
		
		public SomeType()
		{
		}
		
		public SomeType(T value)
		{
			setValue(value);
		}
		
		public void setValue(T value)
		{
			this.value = value;
		}
		
		public T getValue()
		{
			return this.value;
		}
	}
	
	/** 현재페이지 */
	private int pageIndex = 1;

	/** 페이지갯수 */
	private int pageUnit = 10;

	/** 페이지사이즈 */
	private int pageSize = 10;

	/** firstIndex */
	private int firstIndex = 1;

	/** lastIndex */
	private int lastIndex = 1;

	/** recordCountPerPage */
	private int recordCountPerPage = 10;
	
	/* 검색 조건들 */
	private String cityName = "";
	// 시군 ID
	private int cityCode = 0;
	// 지역 ID
	private String townCode = "";
	private String dateCondition = "";
	private String beginDateCondition = "";
	private String endDateCondition = "";

	public int getFirstIndex() {
		return firstIndex;
	}

	public void setFirstIndex(int firstIndex) {
		this.firstIndex = firstIndex;
	}

	public int getLastIndex() {
		return lastIndex;
	}

	public void setLastIndex(int lastIndex) {
		this.lastIndex = lastIndex;
	}

	public int getRecordCountPerPage() {
		return recordCountPerPage;
	}

	public void setRecordCountPerPage(int recordCountPerPage) {
		this.recordCountPerPage = recordCountPerPage;
	}

	public int getPageIndex() {
		return pageIndex;
	}

	public void setPageIndex(int pageIndex) {
		this.pageIndex = pageIndex;
	}

	public int getPageUnit() {
		return pageUnit;
	}

	public void setPageUnit(int pageUnit) {
		this.pageUnit = pageUnit;
	}

	public int getPageSize() {
		return pageSize;
	}

	public void setPageSize(int pageSize) {
		this.pageSize = pageSize;
	}
	
	public String getCityName()
	{
		return cityName;
	}
	
	public void setCityName(String cityName)
	{
		this.cityName = cityName;
	}
	
	public int getCityCode()
	{
		return cityCode;
	}
	
	public void setCityCode(int cityCode)
	{
		this.cityCode = cityCode;
	}
	
	public String getTownCode()
	{
		return townCode;
	}
	
	public void setTownCode(String townCode)
	{
		this.townCode = townCode;
	}
	
	public String getDateCondition()
	{
		return dateCondition;
	}
	
	public void setDateCondition(String dateCondition)
	{
		this.dateCondition = dateCondition;
	}
	
	public String getBeginDateCondition()
	{
		return beginDateCondition;
	}
	
	public void setBeginDateCodition(String dateCondition)
	{
		this.beginDateCondition = dateCondition;
	}
	
	public String getEndDateCondition()
	{
		return endDateCondition;
	}
	
	public void setEndDateCondition(String dateCondition)
	{
		this.endDateCondition = dateCondition;
	}

	@Override
	public String toString() {
		return ToStringBuilder.reflectionToString(this);
	}
	
	protected boolean tryParseInt(String value, SomeType<Integer> out)
	{
		try
		{
			int num = Integer.parseInt(value);
			out.setValue(num);
			return true;
		}
		catch (NumberFormatException e)
		{
		}
		
		return false;
	}
	
	protected boolean tryParseDouble(String value, SomeType<Double> out)
	{
		try
		{
			double num = Double.parseDouble(value);
			out.setValue(num);
			return true;
		}
		catch (NumberFormatException e)
		{
		}
		
		return false;
	}
}
