package kr.co.unes.aqm.dao;

import org.apache.ibatis.annotations.Param;

import kr.co.unes.aqm.dto.QualityEvaluation;

public interface QualityEvaluationDAO {

	QualityEvaluation getQualityEvalution(@Param("type") int nSensorCode);
	
}
