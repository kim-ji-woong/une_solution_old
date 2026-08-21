package egovframework.weather.service;

import java.io.Serializable;

import org.apache.commons.lang3.builder.ToStringBuilder;

public class WeatherDefaultVO implements Serializable {

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
	
	/**
	 *  serialVersion UID
	 */
	private static final long serialVersionUID = -858838578081269360L;

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
