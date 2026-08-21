<%@ page contentType="text/html; charset=utf-8" pageEncoding="utf-8"%>
<%@ taglib prefix="c"         uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="form"      uri="http://www.springframework.org/tags/form" %>
<%@ taglib prefix="validator" uri="http://www.springmodules.org/tags/commons-validator" %>
<%@ taglib prefix="spring"    uri="http://www.springframework.org/tags"%>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
<title>${Title}</title>
	<!--모바일에서 그대로 보이기 -->
	<meta name="viewport" content="width=device-width" />
	<meta name="viewport" content="initial-scale=1.0, maximum-scale=1.0, minimum-scale=1.0, user-scalable=no"/>

    <!--  JQuery UI 사용하기 -->
	<link rel="stylesheet" href="<c:url value='/js/jquery-ui-1.12.1/jquery-ui.css'/>"/>
	<script src="<c:url value='/js/jquery-1.12.4.js'/>"></script>
	<script src="<c:url value='/js/jquery-ui-1.12.1/jquery-ui.js'/>"></script>

	<link type="text/css" rel="stylesheet" href="<c:url value='/css/default.css'/>"/>
	
    <!--  <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js"></script> -->
    <script type="text/javascript">
		 // 페이지가 로드되면 실행
	    $(document).ready(function() {
	    	if ("${currentMenu}" != null)
	    	{
	    		var linkedPage = "${currentMenu.linkedPage}";
	    		if (linkedPage.length > 0)
	    			$("#webBody").load(linkedPage);
	    	}
	    });
		 
		function OnClickedMenu(menuName, eventHandler)
		{			
			if (eventHandler.length > 0)
			{
				loadPage(menuName, eventHandler);
			}
		};
		
		var selectedMenu = '';
		function menuSelect(menu)
		{		
			selectedMenu = menu;
			var tt =  document.getElementById("MainMenu");
			
			var array = tt.getElementsByTagName("li");
			//var array = document.getElementById("MainMenu").querySelectorAll('li');
			//var array = Array.from(temp);
    		for(i = 0; i < array.length; i++)
    		{
    			$(array[i]).removeClass("navSelected");  
    			$(array[i]).addClass("navNormal");
    		}     		
    		$('#'+menu).removeClass("navNormal");	
			$('#'+menu).addClass("navSelected");
		}
		
		function loadPage(menu, url)
        {
            $.ajax({
                type: "GET",
                url: url,
                data:"",
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                dataType: "HTML",
                success: function (data) {
                    if (data != '') {
                   	 	$("#webBody").html('');
                        $("#webBody").html(data);
                        menuSelect(menu);
                    }
                },
                complete: function (data) {
                },
                error: function (request, status, error) {
                    $("#webBody").html(error);
                }
            });
	    }
		
		function loadPageFormSubmit(url, form)
		{
			var params = '';
			if( form != null)
				params = jQuery(form).serialize();

			$.ajax({
				type : "POST",
				url : url,
				data : params,
				cache : false,
				contentType: "application/x-www-form-urlencoded; charset=UTF-8",
				dataType : "HTML",
				success : function(data) {				
					if (data != '') {
                   	 	$("#webBody").html('');
                        $("#webBody").html(data);
                        menuSelect(selectedMenu);
                    }
				},
				complete : function(data) {					
				},
				error : function(request, status, error) {
					$("#webBody").html(error);
				}
			});
		}
      
        function getTimeStamp() { // 24시간제
      	  var d = new Date();

      	  var s =
      	    leadingZeros(d.getFullYear(), 4) + '년 ' +
      	    leadingZeros(d.getMonth() + 1, 2) + '월 ' +
      	    leadingZeros(d.getDate(), 2) + '일 ' +

      	    leadingZeros(d.getHours(), 2) + ':' +
      	    leadingZeros(d.getMinutes(), 2) + ':' +
      	    leadingZeros(d.getSeconds(), 2);

      	  return s;
      	}

      	function leadingZeros(n, digits) {
      	  var zero = '';
      	  n = n.toString();

      	  if (n.length < digits) {
      	    for (i = 0; i < digits - n.length; i++)
      	      zero += '0';
      	  }
      	  return zero + n;
      	}
      	
      	var prevTime = new Date();
      	var originUpdateTime = -1;
      	var isStartClock = false;
      	var currentMenuName = '';
      	var currentMenuHandler = '';
      	
		function startTime() {
			//if (updateTime == "")
			//	updateTime = "${AutoUpdateTime}";
			
			updateTime = getUpdateTime();
			
			if (updateTime <= 0)
			{
				var str = updateTime + "초";
				//var str = getTimeStamp();
				var spanClcok = document.getElementById('subTxtMini');
				if (spanClcok != null)
					$(spanClcok).text("업데이트 남은 시간 : " + str);
				
				updatePage();
			}
			else
			{
				var str = updateTime + "초";
				//var str = getTimeStamp();
				var spanClcok = document.getElementById('subTxtMini');
				if (spanClcok != null)
				{
					$(spanClcok).text("업데이트 남은 시간 : " + str);
					var t = setTimeout(startTime, 500);
					isStartClock = true;
				}
				else
					isStartClock = false;				
				
				/*if (updateTime != -1)
				{
					updateTime = updateTime - 1;
				}*/
			}
		}
		
		function getUpdateTime()
		{
			var currentTime = new Date();
			// milli seconds
			var elapsed = currentTime.getTime() - prevTime.getTime();
			// Number를 int로 변환
			var second = (elapsed / 1000) | 0;
			return originUpdateTime - second;
		}
		
		function updatePage()
		{
			OnClickedMenu(currentMenuName, currentMenuHandler);
			/*isStartClock = false;
			
			if ("${currentMenu}" != null)
	    	{
	    		var linkedPage = "${currentMenu.linkedPage}";
	    		if (linkedPage.length > 0)
	    			$("#webBody").load(linkedPage);
	    	}*/
			
			//updateTime = originUpdateTime;
		}
		
		function startClock()
		{
			prevTime = new Date();
			
			//if(isStartClock == false)
			{
				startTime();
			}
		}
		
		function goMain() {
			loadPage("메인화면", "mainPageList.do");
		}
		
		startClock();
	</script>
</head>
<body>
	<form:form id="formWeatherFrame" name="formWeatherFrame" method="post" action="#LINK">
    <!--상단 프로그램명, 사용자명 시작-->
	<div class="topBg" style="background-color:#004898;">
		<ul>
			<li class="rightFloat">${AppName}</li>
		</ul>
	</div>
	<!--메뉴시작-->
	<nav>
		<ul id="MainMenu">
			<li class="navLogo">
				<img src="<c:url value='/images/layout/logoSample.png' />" onclick="goMain();" style="cursor: pointer"/>				
			</li>
			<c:forEach var="menu" items="${menus}">
				<c:if test="${menu.visible == true}">
					<c:if test="${menu.firstMenu == false}">
						<li class="navDot">·</li>
					</c:if>
					<c:choose>
						<c:when test="${menu.selected == true}">
							<li id="${menu.name}" class="navSelected">
						</c:when>
						<c:otherwise>
							<li id="${menu.name}" class="navNormal">
						</c:otherwise>
					</c:choose>
					<a href="#${menu.name}" onclick="OnClickedMenu('${menu.name}', '${menu.linkedPage}')">${menu.name}</a></li>
				</c:if>
			</c:forEach>
		</ul>
	</nav>
    <div id="webBody"></div>
    <footer>
	Copyright ⓒ 2017 강원도 All rights Reserved
	</footer>
    </form:form>
</body>
</html>