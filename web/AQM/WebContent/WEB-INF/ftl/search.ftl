<#ftl encoding="utf-8">
<!DOCTYPE html>
<html lang=ko>

<head>
<meta charset=utf-8>
<meta content="IE=edge" http-equiv=X-UA-Compatible>
<meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no">
<title>U&amp;E</title>
<#include "inc/include.ftl">
<style>
fieldset {
	border: 0;
	margin-top: 30px;
	margin-left: 60px;
}

label {
	display: block;
	color: #FEFEFE;
	margin: 20px 0 0 0;
}
</style>
<script src="${Context.contextPath}/js/searchmap.js?ver=6"></script>
<script src="${Context.contextPath}/js/searchlists.js?ver=6"></script>
<script src="${Context.contextPath}/js/address.js?ver=6"></script>
<script>
	$(document).ready(
			function() {
				$.ajax({
					type : "GET",
					url : "${Context.contextPath}/Area/depth1s",
					dataType : "JSON",
					success : function(data) {

						//alert(data.Area.depth1s);
						var htmlStr = '';
						$.each(data.Area.depth1s, function(k, v) {

							htmlStr += '<option value="' + v.depth1 + '">'
									+ v.depth1 + '</option>';
						});
						$("#depth1").html(htmlStr);						
						
					},
					complete : function(data) {
						//alert(data);
					},
					error : function(request, status, error) {
						alert("code:" + request.status + "\n" + "message:"
								+ request.responseText + "\n" + "error:"
								+ error);
					}
				});
						
				$('#NameforSearch').keypress(function(event) {
	        		if (event.keyCode == 13) {
	           			event.preventDefault();
	        		}
	        	});
			    $('#NameforSearch').keyup(function() {
			        var txt = $('#NameforSearch').val();
			        //$('#NameforSearch').val(txt.replace(/[\n\r]+/g, " "));
			    });
			});
		
	$(function() {
		var dialog;		
		dialog = $("#popupSearch").dialog({
			autoOpen : false,
			height : 140,
			width : 310,
			modal : true,
			resizable: false,			
			dialogClass : 'no-close popup',
			open: function(){
				$('.ui-widget-overlay').bind('click', function() {
                	$('#popupSearch').dialog('close');
            	});
			},			
			buttons : {				
			},
			close : function() {
			}
		});
		
		var dialog_noresult;
		dialog_noresult = $("#popupNoResult").dialog({
			autoOpen : false,
			height : 150,
			width : 320,
			modal : true,
			resizable: false,
			dialogClass : 'no-close popup',
			open: function(){
				$('.ui-widget-overlay').bind('click', closeNoResult);
			},			
			close : function() {
			}
		});
		
		var dialogDetail;
		dialogDetail = $("#popupDetail").dialog({
			autoOpen : false,
			height : 288,
			width : 323,
			modal : true,
			resizable: false,			
			dialogClass : 'no-close popup',
			open: function(){
				$('.ui-widget-overlay').bind('click', function() {
                	$('#popupDetail').dialog('close');
            	});
			},			
			buttons : {				
			},
			close : function() {
			}
		});	
	});
	
	function closeSearch(){
		$('#popupSearch').dialog('close');
	}
	
	function closeNoResult(){
		$('#popupNoResult').dialog('close');
	}
	
	// 탐색함수 연결
	//$(function(){
	//	$("#addressSearchImg").click(addressSearch);
	//	$("#nameSearchImg").click(nameSearch);
	//});
	
	// 이름으로 검색
	function nameSearch(){
		$("#popupSearch").dialog("open");
		ajaxSearchList("#formNameSearch");
	}
	
	// 지역으로 검색
	function addressSearch(){
					
		$("#selecteddepth1").val(selectedDepth1Value);
		$("#selecteddepth2").val(selectedDepth2Value);
		$("#selecteddepth3").val(selectedDepth3Value);
		$("#selecteddepth4").val(selectedDepth4Value);		
									
	 	$("#popupSearch").dialog("open");
		ajaxSearchList("#formAddressSearch");
	}	
	
	function viewDetail(id, name){
		var form = document.getElementById("ReviewName");
		var locName = document.getElementById("SiteName");
		var locID = document.getElementById("SiteID");
		locName.value = name;
		locID.value = id;
		form.submit();
	}
</script>
<script async defer
	src="https://maps.googleapis.com/maps/api/js?key=AIzaSyC0Zjs9pcdTAO3UCBJ1PqhHoYhwFZgRXZY&callback=initMap"></script>
<!--[if (lte ie 9) ]>      <body class="ie9">          <![endif]-->
<!--[if (gt IE 9) ]> <body >       <![endif]-->	
	<div id="wrap" oncontextmenu="return false" ondragstart="return false" onselectstart="return false">	
	<#include "inc/logo.ftl">
	<div id="popupSearch" class="popup" oncontextmenu="return false" ondragstart="return false" onselectstart="return false">
	  <!-- <a href="#" class="btn_popup_close" onclick="$('#popupSearch').close();"><img
			src="${Context.contextPath}/images/btn_popup_close.png" width="13" height="12" alt="닫기" /></a> -->
		<div class="message empty">	
			<p>검색중입니다. 잠시만 기다려주세요</p>
			<a href="#" onclick="closeSearch();"><img src="${Context.contextPath}/images/btn_ok.png" alt="확인" width="122" height="26" /></a>
		</div>
	</div>
	
	<div id="popupNoResult" class="popup" oncontextmenu="return false" 
			ondragstart="return false" onselectstart="return false">
		<div class="message empty">	
			<p>검색결과가 없습니다.</p>
			<a href="#" onclick="closeNoResult();"><img
				src="${Context.contextPath}/images/btn_ok.png" alt="확인" width="122" height="26" /></a>
		</div>
	</div>	
	<div id="popupDetail" class="popup" oncontextmenu="return false" ondragstart="return false" onselectstart="return false">
	</div>
	
	<div class="container">
		<#include "inc/gnb.ftl">
        <!-- gnb -->
		<div class="content">
			<div class="breadcrumbs">
				<a href="Home">홈</a> > <strong>검색</strong>
			</div>
			<div class="search">
				<div class="search_school">
					<h2 class="title">학교 명 검색</h2>
					<form id="formNameSearch" class="box" method="POST" action="${Context.contextPath}/Search">						
						<input type="text" id="NameforSearch" name="Name" placeholder="학교명을 입력하세요" onfocus="this.value=''" /><a href="#" onclick="nameSearch();return false;"><image class="image" id="nameSearchImg" src="${Context.contextPath}/images/btn_search.png" alt="검색" width="54" height="47" /></a>
						<input type='hidden' name="SearchType" value="1">
					</form>
				</div>
				<!-- search_school -->
				<div class="searc_location">
					<h2 class="title">지역 명 검색</h2>
					<div class="list" id="AreaList">
						<form id="formAddressSearch" action="#" method="post">
							<fieldset>	
								<input type="hidden" name="selecteddepth1" id="selecteddepth1" value=""/>								
								<input type="hidden" name="selecteddepth2" id="selecteddepth2" value=""/>	
								<input type="hidden" name="selecteddepth3" id="selecteddepth3" value=""/>	
								<input type="hidden" name="selecteddepth4" id="selecteddepth4" value=""/>
							 	<select name="depth1" id="depth1"></select>
							 	<label for="depth1" color='black'>시 / 도</label>
						 		<select name="depth2" id="depth2"></select>
						 		<label for="depth2">시 / 군 / 구</label>								
								<select name="depth3" id="depth3" ></select>
								<label for="depth3">구 / 읍 / 면 / 동</label>								
								<select name="depth4" id="depth4"></select>
								<label for="depth4">동 / 리</label>
								<input type=hidden name = "SearchType" value="2">
								<a href="#" onclick="addressSearch();return false;"><image id="addressSearchImg" type="image" src="${Context.contextPath}/images/btn_search.png" alt="검색" width="54" height="50" /></a>
								
							</fieldset>
						</form>
					</div>
				</div>
				<!-- searc_location -->
			</div>
			<!-- search -->
			<div class="search_map">
				<div id="map"></div>
				<form id="ReviewName" action="${Context.contextPath}/Review" method="POST">
    			<input id="SiteName" name="SiteName" type="hidden" />
    			<input id="SiteID" name="SiteID" type="hidden" />
				<div id="search_list" class="search_list">					
					<#include "common/search_list.ftl">					
				</div>				
				</form>
				<!-- search_list -->
			</div>
			<!-- search_map -->
			<#include "inc/footer.ftl">
			<!-- footer -->
		</div>
		<!-- content -->
	</div>
	<!-- container -->
</div>
<!-- wrap -->
</body>
</html>
