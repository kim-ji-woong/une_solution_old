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
        function fn_egov_link_page(pageNo){
        	document.searchReportDataListForm.pageIndex.value = pageNo;
        	OnSearchReport(pageNo);
        	return false;
        }
        
        function OnExportExcel()
	    {
			var reportType = document.getElementById("cmbReportType").value;
        	
        	if (reportType != "empty")
        	{
	        	window.parent.document.formWeatherFrame.action = "<c:url value='/downloadExcelPrintReport.do'/>";
	        	window.parent.document.formWeatherFrame.submit();
        	}
	    }
        
        function OnPrintReportData()
        {
        	var reportType = document.getElementById("cmbReportType").value;
        	
        	if (reportType == "empty")
        		return false;
        	
        	document.getElementById("reportIsLoading").className = "boxLoading";
        	document.getElementById("searchTime").className = "hidden";
        	
        	
        	var param = reportType;
        	var pageNo = 1;
        	
       		var firstDate = document.getElementById("txtFirstDate").value;
       		var lastDate = document.getElementById("txtLastDate").value;
       		param += ";" + firstDate + ";" + lastDate;
        	
        	if(pageNo == 1)
        	{
        		loadPage("보고서출력", "/G1Weather/printReportDataList.do?param=" + param);
        	}
        	else
       		{
        		var form = document.getElementById("printReportDataListForm");
        		var url = "<c:url value='printReportDataList.do?param=" + param + "'/>";
        		loadPageFormSubmit(url, form);
       		}
        	
        	
        	
        	return false;
        }
        
        function OnLoad()
        {
        	var param = "${printReportDataParam}";
        	var arrOptions = new Array();
        	
        	if (param != null && param != "")
        		arrOptions = param.split(";");
        	
        	//var arrOptions = param.split(";"); 
        	var currentOption = "";
        	
        	if (arrOptions.length == 0)
        	{
        		//currentOption = "";
        		currentOption = "empty";
        	}
        	else
       		{
        		var cmbReportType = document.getElementById("cmbReportType");
        		currentOption = arrOptions[0];
        		if (cmbReportType.options[0].value == "empty")
        			cmbReportType.remove(0);
        	}
        	document.getElementById("cmbReportType").value = currentOption;
        	
        	if (currentOption == "empty")
        	{
        		document.getElementById("alertTable").className = "hidden";
        		document.getElementById("lblAlertNoResult").className = "tblbodyMemo";
        		
        		document.getElementById("lblNoResult").className = "tblbodyMemo";
        		document.getElementById("resultTable").className = "hidden";
        		document.getElementById("searchTime").innerText = "";
        	}
        	else
        	{			
				document.getElementById("lblAlertNoResult").className = "hidden";
				document.getElementById("alertTable").className = "tbl100p";
				
        		document.getElementById("lblNoResult").className = "hidden";
        		document.getElementById("resultTable").className = "tbl100p";
        		document.getElementById("searchTime").innerText = "조회시각 : " + "${searchingTime}";
        	}

        	// 기간선택 날짜가 초기화 되어있지 않을 경우...
       		if (document.getElementById("txtFirstDate").value.length == 0 && arrOptions.length < 3)
       		{
           		var today = new Date();
           		var month = today.getMonth() + 1;
           		var day = today.getDate();

           		// 초기화 되어있지 않으면 1월 1일부터 시작한다.
           		//document.getElementById("txtFirstDate").value = today.getFullYear() + "-01-01";
           		// 초기화 되어있지 않으면 오늘까지로 한다.
           		document.getElementById("txtLastDate").value = today.getFullYear() + "-" + ((month < 10) ? "0" : "") + month + "-" + ((day < 10) ? "0" : "") + day;
           		document.getElementById("txtFirstDate").value = document.getElementById("txtLastDate").value;
       		}
        	else
        	{
        		document.getElementById("txtFirstDate").value = arrOptions[1];
        		document.getElementById("txtLastDate").value = arrOptions[2];
        	}

        	document.getElementById("searchTime").className = " boxTimeTxt ";
       		document.getElementById("reportIsLoading").className = "hidden";
        }
        
     // DataTimePicker
        $( function() {	
        	var dateFormat = "yy-mm-dd",	
    		from = $( "#txtFirstDate" )	
            .datepicker({	
    			changeMonth: true, 
    			changeYear: true,
    			dayNames: ['월요일', '화요일', '수요일', '목요일', '금요일', '토요일', '일요일'],
    			dayNamesMin: ['월', '화', '수', '목', '금', '토', '일'], 
    			monthNamesShort: ['1','2','3','4','5','6','7','8','9','10','11','12'],
    			monthNames: ['1월','2월','3월','4월','5월','6월','7월','8월','9월','10월','11월','12월'],
    			dateFormat: "yy-mm-dd",
    			yearRange: '${FirstSearchYear}:c'
            })
            .on( "change", function() 
            {
            		to.datepicker( "option", "minDate", getDate( this ) );	
            }),
        	to = $( "#txtLastDate" ).datepicker({	
    	     	changeMonth: true,
    	     	changeYear: true,
    			dayNames: ['월요일', '화요일', '수요일', '목요일', '금요일', '토요일', '일요일'],
    			dayNamesMin: ['월', '화', '수', '목', '금', '토', '일'], 
    			monthNamesShort: ['1','2','3','4','5','6','7','8','9','10','11','12'],
    			monthNames: ['1월','2월','3월','4월','5월','6월','7월','8월','9월','10월','11월','12월'],
    			dateFormat: "yy-mm-dd",
    			yearRange: '${FirstSearchYear}:c'
    	    })	    
    		.on( "change", function() {	
    			from.datepicker( "option", "maxDate", getDate( this ) );	
    		});		      
    		function getDate( element ) {
    			var date;
    			try {
    				date = $.datepicker.parseDate( dateFormat, element.value );
    			} catch( error ) {
    				date = null;
    			}
    			return date;
    		}
      	});
     
        function OnChangeReportType(cmbReportType)
        {
        	var reportType = cmbReportType.value;
        	       	
        	
        	// empty는 임시 옵션이므로 제거한다.
        	if (reportType != "empty")
        	{
        		if (cmbReportType.options[0].value == "empty")
        			cmbReportType.remove(0);
        	}
        	
        	initDate();
        }
        
        function initDate()
        {
        	var today = new Date();
    		var month = today.getMonth() + 1;
    		var day = today.getDate();   		
    		
    		var dayNowFormat = today.getFullYear() + "-" + ((month < 10) ? "0" : "") + month + "-" + ((day < 10) ? "0" : "") + day;
    		var optionFromDate = dayNowFormat;
    		var optionToDate = dayNowFormat;
    	    				
    		document.getElementById("txtFirstDate").value = optionFromDate;
    		document.getElementById("txtLastDate").value = optionToDate;
        }
        
       
    </script>
</head>
<body>
	<!-- 본문 내용 시작-->
	<section class="bodyCenter">
		<!-- 페이지 타이틀과 설명 -->
		<ul class="pageTitle">
			<li class="txtPageTil">보고서출력</li>
			<!--<li class="txtPageSub">| 강수, 적설에 대한 보고서를 출력</li>	-->
		</ul>

		<!-- 검색조건-->
		<ul class="searchBox">
			<li class="searchInput">
				
				<ul>
					<li class="searchTitle">보고서 조건 </li>
					
					<li>
						<select name="cmbReportType" id="cmbReportType" onchange="OnChangeReportType(this);return false;"  class="combo " style="width:120px;">
						<option value="empty"></option>
						<option value="rain">강수</option>
						<option value="snow">적설</option>
						</select>
					</li>
					
					<li> 
						<input id="txtFirstDate" name="" class="txtInput" style="width:150px;" value="" readonly />
						<span> ~  </span>
						<input id="txtLastDate" name="" class="txtInput" style="width:150px;" value="2017-02-02" readonly />
					</li>
					
					<!--20170222 에이앤디 로딩이미지 추가 시작-->
					<li class="rightFloat"> <!--안보이게는 class="hidden 추가"-->					
						<div id="reportIsLoading" class="boxLoading" style="padding-top:3px;"> <!--20170223 style추가 , 여기 hidden해서 숨김표시-->
							<b>데이터를 불러오고 있습니다.</b> 
							<div class="loader"></div>
						</div>
						
						<!--20170223   데이터 불러온 후 불러온 시간 표시 추가-->
						<div id="searchTime" class=" boxTimeTxt " >
						</div>					
					</li>
					<!--20170222 에이앤디 로딩이미지 추가 끝-->
				</ul>				
			</li>
			
			<li class="rightFloat">
				<a href="#" class="btnBlueSc" >
					<input type="submit" value="조 회" onclick="OnPrintReportData();return false;"/>
				</a>
			</li>
		</ul>

		<form:form commandName="searchVO" id="printReportDataListForm" name="printReportDataListForm" method="post" action="#LINK">
			<ul class="printGroup">
				<!--왼쪽 부분 : 레이더, 특보리스트-->
				<li class="leftGroup">
					<ul>
						<li class="subTitle">
							<span class="subTxt">레이더</span>
						</li>
						
						<li>
							<div class="radarImg"> 
								<img src=${radarImageURL} /> <!--이미지 사이즈 박스에 맞춰 자동으로 늘어나게함-->
							</div> 
						</li>
						
						<li class="subTitle">
							<span class="subTxt">특보리스트</span>
						</li>
						
						<li>
							<div class="tblPrintAlertBox"> 	
								
								<b id="lblAlertNoResult" class="hidden">보고서 조회하면 특보 정보가 표시됩니다.</b>
								<table id="alertTable" class="tbl100p">	
									<colgroup>
										<col width="100" /> <!--20170222 에이앤디  사이즈 고정 -->
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
				
				<!--오른쪽: 조회 결과  20170607  수정 시작-->
			<li class="rightGroup">
				<ul>	
					<li class="subTitle">
						<span class="subTxt">조회결과</span>
						<span class="rightFloat"><a href="" class="btnGreen"><input type="submit" value="보고서 출력" onclick="OnExportExcel();return false;"/></a></span>
					</li>
					
					<!--강수 조회 예시 -->
					<li class="">
						<div class="tblPrintResultBox"> 	
							
							<!--20170223 초기값 추가--> 
							<b id="lblNoResult" class="tblbodyMemo">보고서 조건을 선택 후 조회버튼 누르세요.</b>
							
							<table id="resultTable" class="tbl100p">	
								<!--<colgroup>
									<col width="160" />
									<col width="" />
									<col width="" />
									<col width="" />
									<col width=" " />
								</colgroup>-->
								<thead class="tblHeadPrint">
									<tr>
									<c:forEach var="headerName" items="${reportDataHeader1}" varStatus="status">
										<th rowspan=2 > ${headerName}</th>
									</c:forEach>
									<c:forEach var="headerName" items="${reportDataHeaderUp}" varStatus="status">
										<th colspan=3> ${headerName}</th>
									</c:forEach>
									<c:forEach var="headerName" items="${reportDataHeader2}" varStatus="status">
										<th rowspan=2 > ${headerName}</th>
									</c:forEach>
									<!--<th rowspan=2 > ${reportDataHeader[0]}</th>
									<th colspan=3> ${reportUpDataHeader}</th>
									<th rowspan=2> 최대값 </th>	-->										
									</tr>
									
									<tr>
									<c:forEach var="headerName" items="${reportDataHeader3}" varStatus="status">
										<th> ${headerName}</th>
									</c:forEach>
									<!--<th> 어제날짜</th>
									<th> 오늘날짜 </th>
									<th> 계 </th>-->
									</tr>
									
								</thead>				
								
								<tbody class="tblBodyAlert">
									<c:forEach var="reportData" items="${resultList}" varStatus="status">
										<tr>
											<c:forEach var="item" items="${reportData.printDetailItems}" varStatus="itemStatus">
												<td>${item}</td>
											</c:forEach>
										</tr>
									</c:forEach>
									<tr>
										<c:forEach var="item" items="${resultAverageList}" varStatus="status">
											<td class="sumTx">${item}</td>
										</c:forEach>
									</tr>
									<!--<tr>  
										<td>김삿갓면</td>	
										<td>0</td>
										<td>0</td>	
										<td>0</td>	
										<td>0</td>	
									</tr>
									<tr>  
										<td>무릉도원면</td>	
										<td>0</td>
										<td>0</td>	
										<td>0</td>	
										<td>12.0202021-04-14</td>	
									</tr>
									<tr>  
										<td>상동(기상청)</td>	
										<td>0</td>
										<td>0</td>	
										<td>0</td>	
										<td>0</td>	
									</tr>
									<tr>  
										<td>상동읍</td>	
										<td>0</td>
										<td>0</td>	
										<td>0</td>	
										<td>0</td>	
									</tr>
									<tr>  
										<td>영월(기상청)</td>	
										<td>0</td>
										<td>0</td>	
										<td>0</td>	
										<td>0</td>	
									</tr>
									<tr>  
										<td>영월남면</td>	
										<td>0</td>
										<td>0</td>	
										<td>0</td>	
										<td>0</td>	
									</tr>
									<tr>  
										<td>영월북면</td>	
										<td>0</td>
										<td>0</td>	
										<td>0</td>	
										<td>0</td>	
									</tr>
									<tr>  
										<td>주천(기상청)</td>	
										<td>0</td>
										<td>0</td>	
										<td>0</td>	
										<td>0</td>	
									</tr>
									<tr>  
										<td>중동면</td>	
										<td>0</td>
										<td>0</td>	
										<td>0</td>	
										<td>0</td>	
									</tr>
									<tr>  
										<td>한반도면</td>	
										<td>0</td>
										<td>0</td>	
										<td>0</td>	
										<td>0</td>	
									</tr>
									
									<tr>  
										<td class="sumTx">평균</td>	
										<td class="sumTx">0</td>
										<td class="sumTx">0</td>	
										<td class="sumTx">0</td>	
										<td class="sumTx">0</td>	
									</tr>-->
									
								</tbody>
							</table>						
						</div>	
					</li>
					
					
							
				</ul>			
			</li>
			<!-- 20170607 수정 끝 -->
 
			</ul>
		</form:form>
	</section>
	<script>
	 // Page 로딩시 사용
    OnLoad();
	</script>
</body>
</html>