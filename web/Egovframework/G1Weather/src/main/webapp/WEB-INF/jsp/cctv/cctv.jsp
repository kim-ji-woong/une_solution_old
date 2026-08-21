<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8"%>
<%@ taglib prefix="c"      uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="form"   uri="http://www.springframework.org/tags/form" %>
<%@ taglib prefix="ui"     uri="http://egovframework.gov/ctl/ui"%>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags"%>
<%@ page import="java.util.*, java.text.*" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
<title><spring:message code="rrf.title" /></title>
<link type="text/css" rel="stylesheet" href="<c:url value='/css/default.css'/>"/>

<script type="text/javaScript" language="javascript" defer="defer">

        function OnLoad()
        {
        	var param = "${CCTVOption}";
        	var arrOptions = new Array();
        	
        	if (param != null && param != "")
        		arrOptions = param.split(";");
        	
			var cctvOption = "";
        	
        	if (arrOptions.length == 0)
        	{
        		cctvOption = "0";
        	}
        	else
        		cctvOption = arrOptions[0];
        	
        	if (cctvOption == "0")
        	{
        		document.getElementById("cctvFrame").className = "";
        		document.getElementById("cctvNoImg").className = "hidden";
        	}
        	else if (cctvOption == "1")
        	{
        		document.getElementById("cctvFrame").className = "hidden";
        		document.getElementById("cctvNoImg").className = "";
        	}
        	else if (cctvOption == "2")
        	{
        		document.getElementById("cctvFrame").className = "";
        		document.getElementById("cctvNoImg").className = "hidden";
        		
        		if (arrOptions.length >= 2)
        		{
        			var url = arrOptions[1];
        			document.getElementById("cctvFrame").src = url;
        		}
        	}
        }
        
        // Page 로딩시 사용
        OnLoad();
    </script>
</head>
<body>
	<!-- 본문 내용 시작-->
	<section class="bodyCenter">
		<!-- 페이지 타이틀과 설명 -->
		<ul class="pageTitle">
			<li class="txtPageTil">CCTV영상</li>
			<!--<li class="txtPageSub">| 실시간 영상 확인</li>-->
		</ul>

		<!--소제목-->
		<ul class="subTitle">
			<li class="subTxt">
				<span>CCTV</span>	
				<!--<span class="subTxtMini">확인시각 2017년 01월 12일  9:30:30</span>-->				
			</li>
			
		</ul>
		
		<!--CCTV 영상 표시 부 여기는 샘플임 -->
		<div class="cctvBox">
			<!--<video id="cctvFrame" src="" autoplay="autoplay" class="hidden">
			</video>-->
			<!--cctv불러오는 아이프레임 예제 -->
			<iframe id="cctvFrame" src="https://www.w3.org" class="cctvFrame"></iframe>  
			
			<img id="cctvNoImg" src="./images/common/noCCTVImage.png"  style="vertical-align: middle;" align="middle" class="hidden"></img> 
		</div>
	</section>
</body>
</html>