<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8"%>
<%@ taglib prefix="c"      uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="form"   uri="http://www.springframework.org/tags/form" %>
<%@ taglib prefix="ui"     uri="http://egovframework.gov/ctl/ui"%>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
<title><spring:message code="rrf.title" /></title>
<link type="text/css" rel="stylesheet" href="<c:url value='/css/g1Weather.css'/>"/>
<script type="text/javaScript" language="javascript" defer="defer">
        /* pagination 페이지 링크 function */
        function fn_egov_link_page(pageNo){
        	document.listForm.pageIndex.value = pageNo;
        	//document.listForm.method = "GET";
        	document.listForm.action = "<c:url value='/main.do'/>";
           	document.listForm.submit();
        	//$("#webBody").load("realTimeRainFallList.do");
        }
    </script>
</head>
<body style="text-align:center; margin:0 auto; display:inline; padding-top:100px;">
    <form:form commandName="searchVO" id="listForm" name="listForm" method="post">
        <div id="content_pop">
        	<!-- List -->
        	<div id="table">
        		<table width="100%" border="0" cellpadding="0" cellspacing="0" summary=", 시군, 지점번호, 지점명, 관측시각, 강수, 15M, 60M, 금일, 전일, 기온, 풍향1M, 풍속1M, 습도, 비고">
        			<colgroup>
        				<col width="40"/>
        				<col width="100"/>
        				<col width="100"/>
        				<col width="200"/>
        				<col width="240"/>
        				<col width="60"/>
        				<col width="60"/>
        				<col width="60"/>
        				<col width="60"/>
        				<col width="60"/>
        				<col width="60"/>
        				<col width="120"/>
        				<col width="120"/>
        				<col width="60"/>
        				<col width="120"/>
        			</colgroup>
        			<tr>
        				<th align="center"></th>
        				<th align="center"><spring:message code="rrf.table.distname" /></th>
        				<th align="center"><spring:message code="rrf.table.kmacode" /></th>
        				<th align="center"><spring:message code="rrf.table.name" /></th>
        				<th align="center"><spring:message code="rrf.table.eventtime" /></th>
        				<th align="center"><spring:message code="rrf.table.rainflag" /></th>
        				<th align="center"><spring:message code="rrf.table.move15m" /></th>
        				<th align="center"><spring:message code="rrf.table.move60m" /></th>
        				<th align="center"><spring:message code="rrf.table.today" /></th>
        				<th align="center"><spring:message code="rrf.table.yesterday" /></th>
        				<th align="center"><spring:message code="rrf.table.temp" /></th>
        				<th align="center"><spring:message code="rrf.table.winddir" /></th>
        				<th align="center"><spring:message code="rrf.table.windspeed" /></th>
        				<th align="center"><spring:message code="rrf.table.humidity" /></th>
        				<th align="center"><spring:message code="rrf.table.note" /></th>
        			</tr>
        			<c:forEach var="result" items="${resultList}" varStatus="status">
            			<tr>
            				<td align="center" class="listtd"><c:out value="${(searchVO.pageIndex-1) * searchVO.pageSize + status.count}"/></td>
            				<!--td align="center" class="listtd"><c:out value="${paginationInfo.totalRecordCount+1 - ((searchVO.pageIndex-1) * searchVO.pageSize + status.count)}"/></td-->
            				<td align="center" class="listtd"><c:out value="${result.cityName}"/>&nbsp;</td>
            				<td align="center" class="listtd"><c:out value="${result.locationNumber}"/>&nbsp;</td>
            				<td align="center" class="listtd"><c:out value="${result.locationName}"/>&nbsp;</td>
            				<td align="center" class="listtd"><c:out value="${result.timeStamp}"/>&nbsp;</td>
            				<td align="center" class="listtd"><c:out value="${result.raining}"/>&nbsp;</td>
            				<td align="center" class="listtd"><c:out value="${result.rain15M}"/>&nbsp;</td>
            				<td align="center" class="listtd"><c:out value="${result.rain60M}"/>&nbsp;</td>
            				<td align="center" class="listtd"><c:out value="${result.rainToday}"/>&nbsp;</td>
            				<td align="center" class="listtd"><c:out value="${result.rainYesterday}"/>&nbsp;</td>
            				<td align="center" class="listtd"><c:out value="${result.temperature}"/>&nbsp;</td>
            				<td align="center" class="listtd"><c:out value="${result.windDirection1M}"/>&nbsp;</td>
            				<td align="center" class="listtd"><c:out value="${result.windSpeed1M}"/>&nbsp;</td>
            				<td align="center" class="listtd"><c:out value="${result.humidity}"/>&nbsp;</td>
            				<td align="center" class="listtd"><c:out value="${result.description}"/>&nbsp;</td>
            			</tr>
        			</c:forEach>
        		</table>
        	</div>
        	<!-- /List -->
        	<div id="paging">
        		<ui:pagination paginationInfo = "${paginationInfo}" type="image" jsFunction="fn_egov_link_page" />
        		<form:hidden path="pageIndex" />
        	</div>
        </div>
    </form:form>
</body>
</html>