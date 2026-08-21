<%@ page contentType="text/html; charset=utf-8" pageEncoding="utf-8"%>
<%@ page import ="egovframework.weather.service.LoginVO" %>
<%@ taglib prefix="c"         uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="form"      uri="http://www.springframework.org/tags/form" %>
<%@ taglib prefix="validator" uri="http://www.springmodules.org/tags/commons-validator" %>
<%@ taglib prefix="spring"    uri="http://www.springframework.org/tags"%>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
<title>강원 기상 웹</title>
<style>
        #topBar
        {
            background-color:#1F497D;
            height:50px;
        }
        #topBarWrap
        {
            width:1280px;
            margin-left:auto;
            margin-right:auto;
        }
        #title
        {
            display:inline;
            float:left;
            width:auto;
            margin:-9px 5px;
            color:white;
        }
        #topGreeting
        {
            display:inline;
            float:right;
            width:auto;
            text-align:right;
            margin:-9px;
            color:white;
        }
        #menuBar
        {
            background-color:#DBEEF4;
            height:50px;
        }
        #menuBarWrap
        {
            width:1280px;
            margin-left:auto;
            margin-right:auto;
        }
        #menuLeft
        {
            display:inline;
            float:left;
            width:200px;
            margin:6px;
        }
        #menuMiddle
        {
            display:inline;
            float:left;
            width:800px;
            text-align:center;
            margin:10px;
            margin-left:auto;
            margin-right:auto;
        }
        #menuRight
        {
            display:inline;
            float:right;
            width:200px;
            text-align:right;
            margin:10px;
            margin-right:-10px;
        }
        #webBody
        {
            width:1280px;
            margin-left:auto;
            margin-right:auto;
            border:solid;
            height:700px;
            overflow-y:auto;
        }
    </style>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js"></script>
    <script type="text/javascript">
		 // 페이지가 로드되면 실행
	    $(document).ready(function() {
	    	<%
	    	String id = (String)session.getAttribute("userID");// + ", " + session.getId();
	    	%>
	    	var userID = '<%=id%>';
	    	//var userID = '@Request.RequestContext.HttpContext.Session["userID"]';
	    	//alert("userID : " + userID);
	    	
	    	if (userID == null || userID == "")
	    	{
	    		OnLogout();
	    	}
	    	else
	    	{
	        	$("#webBody").load("realTimeRainFallList.do");
	    	}
	    });
	    
        function OnCurrent()
        {
        	alert("OnCurrent");
            $.ajax({
                type: "GET",
                url: "realTimeRainFallList.do",
                data:"",
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                dataType: "HTML",
                success: function (data) {
                    if (data != '') {
                        $("#webBody").html(data);
                    }
                },
                complete: function (data) {
                },
                error: function (request, status, error) {
                    $("#webBody").html(error);
                }
            });
	    };
	    
	    function OnExportExcel()
	    {
	    	document.formWeatherFrame.action = "<c:url value='/downloadExcelRRF.do'/>";
	       	document.formWeatherFrame.submit();
	    }
	    
	    function OnLogout()
	    {
	    	document.formWeatherFrame.action="<c:url value='/actionLogout.do'/>";
			document.formWeatherFrame.submit();
	    }
	</script>
</head>
<body>
	<form:form id="formWeatherFrame" name="formWeatherFrame" method="post" action="#LINK">
    <div id="topBar">
        <div id="topBarWrap">
            <div id="title"><h2>기상웹 시스템</h2></div>
            <div id="topGreeting"><h2>관리자님 안녕하세요</h2></div>
        </div>
    </div>
    <div id="menuBar">
        <div id="menuBarWrap">
            <div id="menuLeft"><img src="<c:url value='/images/layout/LogoButton.png' />" /></div>
            <div id="menuMiddle">
                <img src="<c:url value='/images/layout/ButtonCurrent.png' />" onclick="OnCurrent()"/>
                <img src="<c:url value='/images/layout/ButtonRainFall.png'/>" onclick="OnExportExcel()"/>
                <img src="<c:url value='/images/layout/ButtonSnowFall.png' />" />
                <img src="<c:url value='/images/layout/ButtonWaterLevel.png' />" />
                <img src="<c:url value='/images/layout/ButtonSpecialReport.png' />" />
                <img src="<c:url value='/images/layout/ButtonManagement.png' />" />
            </div>
            <%
	            LoginVO loginVO = (LoginVO)request.getSession().getAttribute("LoginVO");
            	System.out.println("loginVO : " + loginVO);
	    		
	    	    if(loginVO == null || loginVO.getStatus() != LoginVO.LoginStatus.SUCCESS)
	    	    {
            %>
            <div id="menuRight"><img src="<c:url value='/images/layout/ButtonLogIn.png' />" /></div>
            <%
	    	    } else {
            %>
            <div id="menuRight">
            	<a href="<c:url value='/actionLogout.do'/>">
            		<img src="<c:url value='/images/layout/ButtonLogOut.png' />" />
            	</a>
            </div>
            <%
	    	    }
            %>
        </div>
    </div>
    <div id="webBody"></div>
    </form:form>
</body>
</html>