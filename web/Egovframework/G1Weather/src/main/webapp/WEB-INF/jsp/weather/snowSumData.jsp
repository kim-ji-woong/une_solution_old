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

<style>
	/* 페이지 번호 */
	.paging a
	{
	    color:#000000;
	}
</style>

<script type="text/javaScript" language="javascript" defer="defer">
        /* pagination 페이지 링크 function */
        /*function fn_egov_link_page(pageNo){
        	document.snowListForm.pageIndex.value = pageNo;
        	//document.listForm.method = "GET";
        	document.snowListForm.action = "<c:url value='/snowList.do'/>";
           	document.snowListForm.submit();
        }*/    
        
        function OnExportExcel()
	    {
        	window.parent.document.formWeatherFrame.action = "<c:url value='/downloadExcelSnowList.do'/>";
        	window.parent.document.formWeatherFrame.submit();
	    }
   
        function InitUpdateTime()
        {
        	originUpdateTime = ${AutoUpdateTime};
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
			<li class="txtPageTil">적설현황</li>
			<!--<li class="txtPageSub">| 적설 기상정보를 확인</li>-->		
		</ul>

		<!--소제목-->
		<ul class="subTitle">
			<li class="subTxt">
				<span>지점별 적설 현황</span>
				<span id="subTxtMini" class="subTxtMini">
				업데이트 남은 시간 : </span>			
			</li>
			
			<li class="rightFloat marginL10"><a href="" class="btnGreen"><input type="submit" value="엑셀저장" onclick="OnExportExcel();return false;"/></a>  </li> <!--20170222 에이앤디 class 수정 -->
			<li class="rightFloat"><a href="" class="btnGreen"><input type="submit" value="수동업데이트" onclick="updatePage();return false;"/></a>  </li> <!--20170222 버튼 추가 -->
		</ul>
		
		<!--리스트 : 헤더고정, 내용 스크롤로 구성..-->
		<div class="pageTbl">
			<!--테이블 헤더 부분; 내용부분은 스크롤 되게 처리-->
			<table class="tbl100pHead"  style="table-layout:fixed;">
				<colgroup>
					<col width="50" />
					<col width="250" />
					<col width="210" />
					<col width="140" />
					<col width="140" />
					<col width="140" />
					<col width="" />
				</colgroup>
				
				<thead class="tblHead">
					<tr>
						<th>  </th>
						<th> 지점명 </th>
						<th> 관측시각 </th>
						<th> 신적설(cm) </th>
						<th> 금일(cm) </th> 
						<th> 전일(cm) </th> 
						<th> 비고 </th> 
					</tr>
				</thead>				
			</table>
			
			<form:form commandName="searchVO" id="snowListForm" name="snowListForm" method="post" action="#LINK">
				<!--테이블 내용부분 -->
				<div class="tblBodyBox"> <!-- 10건 이상되면 자동 스크롤생김 -->
					<table class="tbl100p"  style="table-layout:fixed;">
					<colgroup>
						<col width="50" />
						<col width="250" />
						<col width="210" />
						<col width="140" />
						<col width="140" />
						<col width="140" />
						<col width="" />
					</colgroup>
					<tbody class="tblBodyBig">
						<c:forEach var="snow" items="${resultList}" varStatus="status">
		           			<tr>
		           				<td><c:out value="${(searchVO.pageIndex-1) * searchVO.pageSize + status.count}"/></td>
		           				<td><c:out value="${snow.locationName}"/>&nbsp;</td>
		           				<td><c:out value="${snow.timeStamp}"/>&nbsp;</td>
		           				<td><c:out value="${snow.snowNew}"/>&nbsp;</td>
		           				<td><c:out value="${snow.snowNow}"/>&nbsp;</td>
		           				<td><c:out value="${snow.snowYesterday}"/>&nbsp;</td>
		           				<td><c:out value="${snow.description}"/>&nbsp;</td>
		           			</tr>
		       			</c:forEach>
					</tbody>
					</table>
				</div>
				<!-- /List -->
	        	<!--<div class="paging">
	        		<ui:pagination paginationInfo = "${paginationInfo}" type="image" jsFunction="fn_egov_link_page" />
	        		<form:hidden path="pageIndex" />
	        	</div>-->
			</form:form>
		</div>
	</section>
</body>
</html>