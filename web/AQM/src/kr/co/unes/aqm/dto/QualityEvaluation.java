package kr.co.unes.aqm.dto;

public class QualityEvaluation {

	private int ID;
	private String name;
	private int type;
	private float standardValue;
	private float goodValue;
	private float normalValue;
	private float attentionValue;
	private float badValue;
	private String descritpion;
	
	public int getID() {
		return ID;
	}
	public void setID(int iD) {
		ID = iD;
	}
	public String getName() {
		return name;
	}
	public void setName(String name) {
		this.name = name;
	}
	public int getType() {
		return type;
	}
	public void setType(int type) {
		this.type = type;
	}
	public float getStandardValue() {
		return standardValue;
	}
	public void setStandardValue(float standardValue) {
		this.standardValue = standardValue;
	}
	public float getGoodValue() {
		return goodValue;
	}
	public void setGoodValue(float goodValue) {
		this.goodValue = goodValue;
	}
	public float getNormalValue() {
		return normalValue;
	}
	public void setNormalValue(float normalValue) {
		this.normalValue = normalValue;
	}
	public float getAttentionValue() {
		return attentionValue;
	}
	public void setAttentionValue(float attentionValue) {
		this.attentionValue = attentionValue;
	}
	public float getBadValue() {
		return badValue;
	}
	public void setBadValue(float badValue) {
		this.badValue = badValue;
	}
	public String getDescritpion() {
		return descritpion;
	}
	public void setDescritpion(String descritpion) {
		this.descritpion = descritpion;
	}
	
	
	public int getPercentEvaluation(float value)
	{
		if( badValue == 0)
			return 0;
		
		float f = (value / badValue) * 100;
		int n = (int)f;
		
		if( n > 100)
			n = 100;
		return n;
	}
	public int getQualityEvalution(float value)
	{
		if( value < goodValue)
			return 1;		
		if( value < normalValue)
			return 2;		
		if( value < attentionValue)
			return 3;		
		return 4;
	}
	
}
