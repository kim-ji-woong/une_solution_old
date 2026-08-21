<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8"%>
<%@ taglib prefix="c"      uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="form"   uri="http://www.springframework.org/tags/form" %>
<%@ taglib prefix="ui"     uri="http://egovframework.gov/ctl/ui"%>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags"%>
<%@ page import="java.util.*, java.text.*" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head>
<title>에러페이지</title>
	<meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <!--<meta http-equiv="X-UA-Compatible" content="IE=10">-->


<!--모바일에서 그대로 보이기 -->
	<meta name="viewport" content="width=device-width" />
	<meta name="viewport" content="initial-scale=1.0, maximum-scale=1.0, minimum-scale=1.0, user-scalable=no"/>

	<link rel="stylesheet" type="text/css" href="<c:url value='/css/default.css'/>"> <!--개발시 경로 바뀌면 바꿔서 작성 -->


	
</head>
<body style="background-color:#f4f4f4;">
	<div class="errorPage">
		<div class="errorPageTitle">
		기상웹시스템
		</div>	
		<ul class="errorPageCenter">
			<li><img src="<c:url value='/images/common/iconError.png'/>"/></li>
			<li style="vertical-align:top;">
				<div class="tx1"> 세션이 종료 되었습니다.</div>  <!-- 주요문제 글-->
				<div class="tx2">뒤로가기 또는 브라우저를 다시 시작하세요.</div> <!-- 해결법 글-->
			
			</li>
		</ul>
		<hr class="errorPageBar" />
		<div class="errorpageTextarea"> 
			<textarea id="errorTextBox" rows="3" style="resize:none; readonly="readonly"></textarea>
		</div>
		<!---버튼 영역, 사용안하면 지우세요 -->
		<div>
			<a href="" class="btnGreen" ><input type="submit" style="font-size:13pt;"  value="뒤로가기" /> </a> 
		</div>	
	</div>
</body>
<script>
	function setErrorText()
	{
		var errMsg = '';
		
		<c:if test="${errorText != null}">
		
		errMsg = 'SyntaxError : ' + '${errorText}';
		</c:if>
		<c:if test="${exceptMsg != null}">
		errMsg = 'SyntaxError : ' + '${exceptMsg.message}';
		</c:if>
		var text = document.getElementById("errorTextBox");
		if( text!= null)
			text.innerHTML = errMsg;
	}

	setErrorText();
	</script>
</html>

