package kr.co.unes.aqm.dto.sensor;

public class SensorCode 
{
	
	private int ID;	
	private String Name;	
	private int GroupID = -1;	
	private int LimitType;
	private float LimitNotice;	
	private float LimitAttention;	
	private float LimitWarning;	
	private float LimitValueLaw;
	private float LimitNoticeBegin;
	private float LimitNoticeEnd;
	private float LimitAttentionBegin;
	private float LimitAttentionEnd;
	private float LimitWarningBegin;
	private float LimitWarningEnd;
	private float LimitValueLawBegin;
	private float LimitValueLawEnd;
	private String Unit;
	private String Remark;

	public int getID() {
		return ID;
	}

	public void setID(int m_nID) {
		this.ID = m_nID;
	}

	public String getRemark() {
		return Remark;
	}

	public void setRemark(String remark) {
		Remark = remark;
	}

	public float getLimitValueLawEnd() {
		return LimitValueLawEnd;
	}

	public void setLimitValueLawEnd(float limitValueLawEnd) {
		LimitValueLawEnd = limitValueLawEnd;
	}

	public float getLimitValueLawBegin() {
		return LimitValueLawBegin;
	}

	public void setLimitValueLawBegin(float limitValueLawBegin) {
		LimitValueLawBegin = limitValueLawBegin;
	}

	public float getLimitWarningEnd() {
		return LimitWarningEnd;
	}

	public void setLimitWarningEnd(float limitWarningEnd) {
		LimitWarningEnd = limitWarningEnd;
	}

	public float getLimitWarningBegin() {
		return LimitWarningBegin;
	}

	public void setLimitWarningBegin(float limitWarningBegin) {
		LimitWarningBegin = limitWarningBegin;
	}

	public float getLimitAttentionEnd() {
		return LimitAttentionEnd;
	}

	public void setLimitAttentionEnd(float limitAttentionEnd) {
		LimitAttentionEnd = limitAttentionEnd;
	}

	public float getLimitAttentionBegin() {
		return LimitAttentionBegin;
	}

	public void setLimitAttentionBegin(float limitAttentionBegin) {
		LimitAttentionBegin = limitAttentionBegin;
	}

	public float getLimitNoticeEnd() {
		return LimitNoticeEnd;
	}

	public void setLimitNoticeEnd(float limitNoticeEnd) {
		LimitNoticeEnd = limitNoticeEnd;
	}

	public float getLimitNoticeBegin() {
		return LimitNoticeBegin;
	}

	public void setLimitNoticeBegin(float limitNoticeBegin) {
		LimitNoticeBegin = limitNoticeBegin;
	}

	public int getLimitType() {
		return LimitType;
	}

	public void setLimitType(int limitType) {
		LimitType = limitType;
	}

	public String getSensorUnit() {
		return Unit;
	}

	public void setSensorUnit(String szUnit) {
		this.Unit = szUnit;
	}

	public float getLimitValueLaw() {
		return LimitValueLaw;
	}

	public void setLimitValueLaw(float limitValueLaw) {
		LimitValueLaw = limitValueLaw;
	}

	public float getLimitWarning() {
		return LimitWarning;
	}

	public void setLimitWarning(float limitWarning) {
		LimitWarning = limitWarning;
	}

	public float getLimitAttention() {
		return LimitAttention;
	}

	public void setLimitAttention(float limitAttention) {
		LimitAttention = limitAttention;
	}

	public float getLimitNotice() {
		return LimitNotice;
	}

	public void setLimitNotice(float limitNotice) {
		LimitNotice = limitNotice;
	}

	public int getGroupID() {
		return GroupID;
	}

	public void setGroupID(int groupID) {
		GroupID = groupID;
	}

	public String getName() {
		return Name;
	}

	public void setName(String szName) {
		this.Name = szName;
	}	
	
}
