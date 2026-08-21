<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8"%>
<%@ taglib prefix="c"      uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="form"   uri="http://www.springframework.org/tags/form" %>
<%@ taglib prefix="ui"     uri="http://egovframework.gov/ctl/ui"%>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags"%>
<%@ page import="java.util.*, java.text.*" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
<title>메인</title>
	<meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <!--<meta http-equiv="X-UA-Compatible" content="IE=10">-->

<!--모바일에서 그대로 보이기 -->
	<meta name="viewport" content="width=device-width" />
	<meta name="viewport" content="initial-scale=1.0; maximum-scale=1.0; minimum-scale=1.0; user-scalable=no;"/>

	<link rel="stylesheet" type="text/css" href="../css/default.css"> <!--개발시 경로 바뀌면 바꿔서 작성 -->

	<script type="text/javaScript" language="javascript" defer="defer">
	function loadRainPage(url)
    {
        $.ajax({
            type: "GET",
            url: url,
            data:"",
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            dataType: "HTML",
            success: function (data) {
                if (data != '') {
                  
                }
            },
            complete: function (data) {
            },
            error: function (request, status, error) {
                $("#webBody").html(error);
            }
        });
    }
	
	function InitUpdateTime()
    {
    	originUpdateTime = ${AutoUpdateTime}; 
    }
	
	function OnExportExcel()
    { 
       	window.parent.document.formWeatherFrame.action = "<c:url value='/downloadExcelMainPage.do'/>";
       	window.parent.document.formWeatherFrame.submit(); 
    }
	
	// 페이지가 로드되면 실행
    $(document).ready(function() {
    	InitUpdateTime();
    	currentMenuName = "${currentMenu.name}";
    	currentMenuHandler = "${currentMenu.linkedPage}";
    	startClock();
    });
	</script>

</head>

<body> 	
	<!-- 본문 내용 시작-->
	<section class="bodyCenter">
		<!-- 페이지 타이틀과 설명 -->
		<ul class="pageTitle">
			<li class="txtPageTil">메인화면</li>
			<li class="txtPageSub">| 레이더, 금일기상특보, 강수현황 확인</li>			
		</ul>				
		
		<form:form commandName="searchVO" method="post" action="#LINK">
		<ul class="printGroup">
			<!--왼쪽 부분 : 레이더, 특보리스트-->
			<li class="leftGroup">
				<ul>
					<li class="subTitle">
						<span class="subTxt">레이더</span>
					</li>
					
					<li>
						<div class="radarImg">   
							<img src=${radarImageURL} /> <!-- 초기 이미지 지정--> 
						</div> 
					</li>
					
					<li class="subTitle">
						<span class="subTxt">특보리스트</span>
					</li>
					
					<li>
						<div class="tblPrintAlertBox" style="height:285px;"> 	
							<table class="tbl100p">	
								<colgroup>
									<col width="100" /> 
									<col width="100" />
									<col width="" />
									<col width="" />
								</colgroup>
				
								<thead class="tblHead">
									<tr>						
									<th> 특보시각 </th>
									<th> 종류 </th>
									<th> 코드 </th>
									<th> 지역 </th>						
									</tr>
								</thead>

								<tbody class="tblBodyAlert">
										 <c:forEach var="news" items="${newsList}" varStatus="status">
											<tr>
												<c:choose>
													<c:when test="${news.emptyData == true}">
														<td colspan=4></td>
													</c:when>
													<c:otherwise>
														<td>${news.time}</td>
														<td>${news.newsType}</td>
														<td>${news.commandString}</td>
														<td>${news.areaName}</td>
													</c:otherwise>
												</c:choose> 
											</tr>
										</c:forEach>
									</tbody>
							</table>							
						</div>		
					</li>					
				</ul>			
			</li>
			
			<!--오른쪽 : 강우현황 -->
			<li class="rightGroup">
				<ul>	
					<li class="subTitle">
						<span class="subTxt">강우현황</span>
						<span class="rightFloat"><a href="" class="btnGreen"><input type="submit" value="보고서 출력" onclick="OnExportExcel();return false;" /></a></span>
					</li>					
					
					<li class="">
						<div class="tblMainResultBox"> 	
							
							<!--리스트 : 헤더고정, 내용 스크롤로 구성..-->
							<div class="pageTbl">
								<!--테이블 헤더 부분; 내용부분은 스크롤 되게 처리-->
								<table class="tbl100pHead"  style="table-layout:fixed;">
									<colgroup>					
									<col width="135" />
									<col width="200" />
									<col width="80" />
									<col width="80" />
									<col width="80" />
									<col width="80" />
									<col width="" />
									</colgroup>
				
									<thead class="tblHead" style="height:43px;">
										<tr>						
											<th> 지점명 </th>
											<th> 관측시간 </th>
											<th> 이동15분<br>(mm) </th>
											<th> 이동60분<br>(mm) </th> 
											<th> 금일<br>(mm) </th> 
											<th> 전일<br>(mm) </th> 
											<th> 비고 </th>  <!--20170222 에이앤디 글 추가 -->
										</tr>
									</thead>				
								</table>
		
								<!--테이블 내용부분 -->
								<div class="tblBodyBox"> <!-- 한번에 13개보임 -->
									<table class="tbl100p"  style="table-layout:fixed;">
										<colgroup>					
											<col width="135" />
											<col width="200" />
											<col width="80" />
											<col width="80" />
											<col width="80" />
											<col width="80" />
											<col width="" />
										</colgroup>
				<tbody class="tblBodyBig">
					<c:forEach var="rain" items="${rainResultList}" varStatus="status">
	           			<tr>
	           				<!--<td><c:out value="${status.count}"/></td>-->
	           				<!--<td><c:out value="${(searchVO.pageIndex-1) * searchVO.pageSize + status.count}"/></td>-->
	           				<td><c:out value="${rain.locationName}"/>&nbsp;</td>
	           				<td><c:out value="${rain.timeStamp}"/>&nbsp;</td>
	           				<td><c:out value="${rain.rain15M}"/>&nbsp;</td>
	           				<td><c:out value="${rain.rain60M}"/>&nbsp;</td>
	           				<td><c:out value="${rain.rainToday}"/>&nbsp;</td>
	           				<td><c:out value="${rain.rainYesterday}"/>&nbsp;</td>
	           				<td><c:out value="${rain.description}"/>&nbsp;</td>
	           			</tr>
	       			</c:forEach>
				</tbody> 
				</table>
			</div> 
		</div>											
						</div>	
					</li>							
				</ul>			
			</li> 
		</ul> 
		</form:form>
	</section>
	<!-- 본문 내용 끝--> 
</body>
</html>