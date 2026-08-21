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
		var currentOption = "day";
		
        /* pagination 페이지 링크 function */
        function fn_egov_link_page(pageNo){
        	document.searchWaterLevelListForm.pageIndex.value = pageNo;
        	OnSearchWaterLevel(pageNo);
        }
        
        function OnExportExcel()
	    {
        	window.parent.document.formWeatherFrame.action = "<c:url value='/downloadExcelSearchWaterLevelList.do'/>";
        	window.parent.document.formWeatherFrame.submit();
	    }
        
        function OnSelectDateCondition(opt)
        {
        	if (opt != "day")
        		document.getElementById("liDay").className = " hidden";
        	
        	if (opt != "period")
        		document.getElementById("liPeriod").className = " hidden";
        	
        	if (opt == "day")
        	{
        		/*var today = new Date();
        		var month = today.getMonth() + 1;
        		var day = today.getDate();
        		document.getElementById("txtDay").value = today.getFullYear() + "-" + ((month < 10) ? "0" : "") + month + "-" + ((day < 10) ? "0" : "") + day;*/
        		
        		document.getElementById("liDay").className = "  ";
        	}
        	else if (opt == "period")
        	{
        		document.getElementById("liPeriod").className = "  ";
        	}
        	
        	initDate();
        	
        	currentOption = opt;
        }
        
        function OnSearchWaterLevel(pageNo)
        {
        	var dateOption = document.getElementById("cmbDateOption").value;
        	var param = dateOption;
        	
        	if (dateOption == "day")
        	{
        		var date = document.getElementById("txtDay").value;
        		param += ";" + date;
        	}
        	else if (dateOption == "period")
        	{
        		var firstDate = document.getElementById("txtFirstDate").value;
        		var lastDate = document.getElementById("txtLastDate").value;
        		var locationID = document.getElementById("cmbLocation").value;
        		param += ";" + firstDate + ";" + lastDate + ";" + locationID;
        	}
        	
        	if(pageNo == 1)
        	{
        		loadPage("수위조회", "/G1Weather/searchWaterLevelDataList.do?param=" + param);
        	}
        	else
       		{
        		var form = document.getElementById("searchWaterLevelListForm");
        		var url = "<c:url value='searchWaterLevelDataList.do?param=" + param + "'/>";
        		loadPageFormSubmit(url, form);        		
       		}
        	return false;
        	//document.searchWaterLevelListForm.action = "<c:url value='/searchWaterLevelList.do?param=" + param + "'/>";
        	//document.searchWaterLevelListForm.action = "<c:url value='/searchWaterLevelList.do'/>";
           	//document.searchWaterLevelListForm.submit();
        }
        
        function OnLoad()
        {
        	var param = "${searchWaterLevelParam}";
        	var arrOptions = param.split(";");
        	var isNullParam = 1;
        	
        	if (arrOptions.length == 0)
        		currentOption = "day";
        	else
        	{
        		if (arrOptions[0].length == 0)
        			currentOption = "day";
        		else
        		{
        			currentOption = arrOptions[0];
        			isNullParam = 0;
        		}
        	}
        	
        	if (isNullParam == 1)
        	{
        		// 전달된 값이 없으면 오늘 날짜를 입력한다.
        		var today = new Date();
        		var month = today.getMonth() + 1;
        		var day = today.getDate();
        		document.getElementById("txtDay").value = today.getFullYear() + "-" + ((month < 10) ? "0" : "") + month + "-" + ((day < 10) ? "0" : "") + day;

        		// 전달된 값이 없으면 1월 1일부터 시작한다.
        		//document.getElementById("txtFirstDate").value = today.getFullYear() + "-01-01";
        		// 전달된 값이 없으면 오늘까지로 한다.
        		document.getElementById("txtLastDate").value = document.getElementById("txtDay").value;
        		document.getElementById("txtFirstDate").value = document.getElementById("txtLastDate").value;
        	}
        	
        	document.getElementById("cmbDateOption").value = currentOption;
        	OnSelectDateCondition(currentOption);
        	
        	if (currentOption == "day")
        	{
        		if (arrOptions.length > 1)
        			document.getElementById("txtDay").value = arrOptions[1];
        		
        		if (document.getElementById("txtFirstDate").value.length == 0)
        		{
            		var today = new Date();
            		var month = today.getMonth() + 1;
            		var day = today.getDate();

            		// 초기화 되어있지 않으면 1월 1일부터 시작한다.
            		//document.getElementById("txtFirstDate").value = today.getFullYear() + "-01-01";
            		// 초기화 되어있지 않으면 오늘까지로 한다.
            		document.getElementById("txtLastDate").value = document.getElementById("txtDay").value;
            		document.getElementById("txtFirstDate").value = document.getElementById("txtLastDate").value;
        		}
        	}
        	else if (currentOption == "period")
        	{
        		document.getElementById("txtFirstDate").value = arrOptions[1];
        		document.getElementById("txtLastDate").value = arrOptions[2];
        		document.getElementById("cmbLocation").value = arrOptions[3];
        	}
        }
        
     	// DataTimePicker
        $( function() {	
        	 $( "#txtDay" )
			.datepicker({	
    			changeMonth: true, 
    			changeYear: true,
    			dayNames: ['월요일', '화요일', '수요일', '목요일', '금요일', '토요일', '일요일'],
    			dayNamesMin: ['월', '화', '수', '목', '금', '토', '일'], 
    			monthNamesShort: ['1','2','3','4','5','6','7','8','9','10','11','12'],
    			monthNames: ['1월','2월','3월','4월','5월','6월','7월','8월','9월','10월','11월','12월'],
    			dateFormat: "yy-mm-dd",
    			yearRange: '${FirstSearchYear}:c'
            });
        	 
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
     	
     	
        function initDate()
        {
        	var today = new Date();
    		var month = today.getMonth() + 1;
    		var day = today.getDate();
    		
    		var optionYear = today.getFullYear();
    		var optionMonth =  month;
    		
    		var dayNowFormat = today.getFullYear() + "-" + ((month < 10) ? "0" : "") + month + "-" + ((day < 10) ? "0" : "") + day;
    		var optionFromDate = dayNowFormat;
    		var optionToDate = dayNowFormat;
    		
    		var locationID = '${cityTowns[0].locationID}';
    				
    		document.getElementById("cmbLocation").value = locationID;
    		document.getElementById("txtDay").value = dayNowFormat;
    		document.getElementById("txtFirstDate").value = optionFromDate;
    		document.getElementById("txtLastDate").value = optionToDate;
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
			<li class="txtPageTil">수위조회</li>
			<!--<li class="txtPageSub">| 일일, 기간별 검색</li>-->
		</ul>

		<!-- 검색조건-->
		<ul class="searchBox">
			<li class="searchInput">
				
				<ul>
					<li class="searchTitle">조건선택 </li>
					
					<li>
						<select name="" id="cmbDateOption" onchange="OnSelectDateCondition(this.value)"  class="combo " style="width:120px;">
						<option value="day">일일</option>
						<option value="period">기간선택</option>    					
						</select>
					</li>
					
					<!--일일 검색시 -->
					<li id="liDay" class="  "> <!--보이게 하려면 hidden빼기-->
						<input id="txtDay" name="" class="txtInput" style="width:150px;" value="2017-02-01" readonly />
					</li>
					
					<!--기간 검색시-->
					<li id="liPeriod" class=" hidden" > <!--보이게 하려면 hidden빼기-->
						<select name="" id="cmbLocation" onchange=""  class="combo " style="width:250px;margin-right:10px;">
							<c:forEach var="town" items="${cityTowns}" varStatus="status">
								<option value="${town.locationID}">${town.locationName}</option>
							</c:forEach>
						</select>	
						
						<input id="txtFirstDate" name="" class="txtInput" style="width:150px;" value="" readonly />
						<span> ~  </span>
						<input id="txtLastDate" name="" class="txtInput" style="width:150px;" value="2017-02-02" readonly />
					</li>
				</ul>				
			</li>
			
			<li class="rightFloat">
				<a href="#" class="btnBlueSc" >
					<input type="submit" value="조 회" onclick="OnSearchWaterLevel(1);return false;"/>
				</a>
			</li>
		</ul>
		
		<!--소제목-->
		<ul class="subTitle">
			<li class="subTxt">
				<span>수위 조회 결과</span>
			</li>
			
			<li class="rightFloat"><a href="" class="btnGreen"><input type="submit" value="엑셀저장" onclick="OnExportExcel();return false;"/></a>  </li>
		</ul>
		
		<form:form commandName="searchVO" id="searchWaterLevelListForm" name="searchWaterLevelListForm" method="post" action="#LINK">
			<!--리스트 : 헤더, 내용 함께 스크롤 됨, 이유: 프로그램에서 다 그리는 테이블 이므로-->
			<!-- 조회쪽 리스트 보이형식 : 한페이지에 20개보임. 10개이상시 스크롤 처리, 20개 이상시 페이지 번호 처리 -->
			
			<!--금일 조회 리스트 예 -->
			<div class="tblBox "> 	
				<table class="tbl100p">				
					<thead class="tblHead">
						<tr>
							<c:forEach var="headerName" items="${searchHeader}" varStatus="status">
								<th>&nbsp;${headerName}</th>
							</c:forEach>
						</tr>
					</thead>
					<!--테이블 내용부분 -->
					<tbody class="tblBody">
						<c:forEach var="searchWaterLevel" items="${resultList}" varStatus="status">
							<c:choose>
								<c:when test="${searchWaterLevel.sumData == true}">
									<tr class="sumLine">
								</c:when>
								<c:otherwise>
									<tr>
								</c:otherwise>
							</c:choose>
								<td><c:out value="${searchWaterLevel.locationName}"/>&nbsp;</td>
								<c:forEach var="itemValue" items="${searchWaterLevel.itemValues}" varStatus="status">
									<c:choose>
										<c:when test="${itemValue.sumData == true}">
											<td class="sumTxt">
										</c:when>
										<c:otherwise>
											<td>
										</c:otherwise>
									</c:choose>
									<c:out value="${itemValue.value}"/>&nbsp;</td>
								</c:forEach>
							</tr>
						</c:forEach>
					</tbody>
				</table>
			</div>
			<!-- /List -->
        	<div class="paging">
        		<ul class="tblNum">
        			<ui:pagination paginationInfo = "${paginationInfo}" type="image" jsFunction="fn_egov_link_page" />
        		</ul>
        		<form:hidden path="pageIndex" />
        	</div>
		</form:form>
	</section>
</body>
</html>