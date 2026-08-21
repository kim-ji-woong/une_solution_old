<#ftl encoding="utf-8">
<!DOCTYPE html>
<html lang=ko>

<head>
	<meta content="blendTrans(Duration=0.0)" http-equiv="Page-Enter" />
	<meta content="blendTrans(Duration=0.0)" http-equiv="Page-Exit" />
    <meta charset=utf-8>
    <meta content="IE=edge" http-equiv=X-UA-Compatible>
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no">
    <title>U&amp;E</title>
    <#include "inc/include.ftl">
	<style type="text/css">	
	.ui-slider .ui-slider-handle{
	    width:30px; 
	    height:30px;
	    background:url(${Context.contextPath}/images/icon_progress_dot.png) no-repeat; overflow: hidden; 
	    position:absolute;
	    top: -3px;
	    border-style:none;
	}
	.ui-tabs { 
    	padding: 0px; 
    	background: none; 
    	border-width: 0px 0px 0px 0px; 
	 	border-radius: 0px;
	 	border-style: none;
	}
	.ui-tabs .ui-tabs-nav { 
    	padding-left: 0px; 
	    background: transparent; 
	    border-width: 0px 0px 1px 0px; 
	    border-radius: 0px;
	} 	
	.ui-tabs .ui-tabs-tab { 
    	padding-left: 0px; 
	    border-width: 1px 1px 1px 1px; 	  
	    border-radius: 0px;
	}
	.ui-tabs .ui-tabs-panel {
		border-width: 0px 0px 1px 0px;
 		border-radius: 0px;
	}
	</style>
    <script>    
    var globalSiteID = '${SiteID}';
	
	$(function () {
	    $('#popup').css('left','85%');
	    $('#popup').css('top','32%');
	});	

    $( function() {
        var dialog;
        dialog = $( "#popup" ).dialog({
          autoOpen: false,
          height: 400,
          width: 350,
          modal: true,
          buttons: {
            Cancel: function() {
              dialog.dialog( "close" );
            }
          },
          close: function() {
          }
        });
      });
       
	var checkTimer;
	var preSliderValue;
	var curSliderValue;
	var prevTimerID = -1;
	
    $( function() {
	    $( "#progress" ).slider({
	      range: "min",
	      value: 24,
	      min: 1,
	      max: 24,
	      step : 1,
	      slide: function( event, ui ) {	 
	      		if(currentNodeID != -1){
	      			var value = ui.value;
	      			curSliderValue = value;
		      		//if( prevTimerID != -1)
		      		//	clearTimeout(prevTimerID);
		   		   	//prevTimerID = setTimeout(function(){
		   		  		showSensorTimeSeries(value);       		  	
		   		  	//}, 500);
	      		}
	      }	   
    	});
    	$( "#progress" ).slider("disable");      
  	});
  	
  	function showAlert()
  	{
  		alert("측정위치를 먼저 선택하세요");	      			
  	}
  	
  	function showSensorTimeSeries(page)
  	{  	
  		showPage(page);
  		prevTimerID = -1;
  	}
    
	$( function() {	
    	var dateFormat = "yy-mm-dd",	
		from = $( "#from" )	
        .datepicker({	
			changeMonth: true, 
			dayNames: ['월요일', '화요일', '수요일', '목요일', '금요일', '토요일', '일요일'],
			dayNamesMin: ['월', '화', '수', '목', '금', '토', '일'], 
			monthNamesShort: ['1','2','3','4','5','6','7','8','9','10','11','12'],
			monthNames: ['1월','2월','3월','4월','5월','6월','7월','8월','9월','10월','11월','12월'],
			dateFormat: "yy-mm-dd"
        })
        .on( "change", function() 
        {
        		to.datepicker( "option", "minDate", getDate( this ) );	
        }),
    	to = $( "#to" ).datepicker({	
	     	changeMonth: true, 
			dayNames: ['월요일', '화요일', '수요일', '목요일', '금요일', '토요일', '일요일'],
			dayNamesMin: ['월', '화', '수', '목', '금', '토', '일'], 
			monthNamesShort: ['1','2','3','4','5','6','7','8','9','10','11','12'],
			monthNames: ['1월','2월','3월','4월','5월','6월','7월','8월','9월','10월','11월','12월'],
			dateFormat: "yy-mm-dd"
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
  	
  	
  	var firstNode = -1;
  	var hasElement = false;
  	$(document).ready(
		function() {
			var url = '${Context.contextPath}/Site/nodes/' + globalSiteID;
			$.ajax({
				type : "GET",
				url : url,
				dataType : "JSON",
				success : function(data) {				
					if( data.Result > 0) {
						$("#nodes").html('');	
						$("#detailReviewNodeSelect").html('');
						var htmlStr = '';
						
						$.each(data.Site.values, function(k, v) {
							htmlStr += '<option id="'+ v.ImageKey + '" value="' + v.nodeID + '">'
									+ v.nodeName + '</option>';
								
							if(firstNode < 0)
								firstNode = v.nodeID;
									
							hasElement = true;
							
							
						});
						$("#nodes").html(htmlStr);						
						$("#detailReviewNodeSelect").html(htmlStr);
						
						
						if(hasElement == true)
						{
							selectOption(firstNode);
						}
					}					
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
		}
	);
	
	function selectOption(data)
	{
		$( "#nodes" ).val(data).selectmenu("refresh").trigger("selectmenuchange");
	}
	
	var currentNodeID = -1;
	
	function reviewNodeSelect(event, data){
		
		currentNodeID = $( "#nodes" ).val();
		var imgKey = $("#nodes").find(":selected").attr("id");
		var text = $("#nodes").find(":selected").text();
		$("#nodeSelectResult").html('');
		var content = '<div class="alert">'+text+'</div>';
		$("#nodeSelectResult").html(content);
				
		getSensorRealtime(currentNodeID);
		getSensorTimeSeries(currentNodeID);
		
		$( "#progress" ).slider("enable");
		
		// set Image
		$("#MapImage").attr("src", imgKey);
		$("#MapImage").attr("width", 337);
		$("#MapImage").attr("height", 268);
	}
	
	function detailReviewChangeNode(event, data)
	{
	}
	
	$(function() {
		
		$("#tabs").tabs();
		
		$("#nodes").selectmenu({
			change :reviewNodeSelect,
			select :reviewNodeSelect 
		});
		
		$("#detailReviewNodeSelect").selectmenu({
			change :detailReviewChangeNode,
			select :detailReviewChangeNode 
		});	
		if(hasElement == true)
		{
			selectOption(firstNode);
		}
		
		$( "#nodes" ).on( "selectmenuselect", function( event, ui ) {
			reviewNodeSelect(event, ui);
		});
		
		$( "#nodes" ).on( "selectmenuchange", function( event, ui ) {
			reviewNodeSelect(event, ui);
		});
	});	
	
	function clearSensorData()
	{
	}
	
	function getSensorRealtime(nodeID)
	{
		//clearSensorData();		
		var params = $("#formNodeSelect").serialize(); 	
		$.ajax({
			type : "POST",
			url : "${Context.contextPath}/Review/realtime/" + nodeID,
			data : params,
			contentType: "application/x-www-form-urlencoded; charset=UTF-8",
			dataType : "HTML",
			success : function(data) {				
				if( data == '')
				{
					//$("#popupSearch").dialog("close");
					//$("#popupNoResult").dialog("open");
				}
				else
				{		
				   	$("#realtime").html('');			
					$("#realtime").html(data);				
					//$("#popupSearch").dialog("close");					
				}
			},
			complete : function(data) {
				//$("#popupSearch").dialog("close");
			},
			error : function(request, status, error) {
				//$("#popupSearch").dialog("close");
				//$("#popupNoResult").dialog("open");
			}
		});
	}
	
	function getSensorTimeSeries(nodeID)
	{
		clearSensorData();		
		var params = $("#formNodeSelect").serialize(); 	
		$.ajax({
			type : "POST",
			url : "${Context.contextPath}/Review/node/" + nodeID,
			data : params,
			contentType: "application/x-www-form-urlencoded; charset=UTF-8",
			dataType : "HTML",
			success : function(data) {				
				if( data == '')
				{
					//$("#popupSearch").dialog("close");
					//$("#popupNoResult").dialog("open");
				}
				else
				{		
				   	$("#timeseriesData").html('');			
					$("#timeseriesData").html(data);				
					//$("#popupSearch").dialog("close");
					setPagination("#timeseriesData");
				}
			},
			complete : function(data) {
				//$("#popupSearch").dialog("close");
			},
			error : function(request, status, error) {
				//$("#popupSearch").dialog("close");
				//$("#popupNoResult").dialog("open");
			}
		});
	}
	
	function setPagination(name) {
		var objContent = $(name);	   
		var subPages = new Array();  
		var lastPage = 0;
		var nPageCount = 0;		
	
		init = function() {
			objContent.children().each(function(i){ 
				if(this.id == "SensorTimeData")
				{
					subPages.push(this);
					nPageCount++;
				}
			});
			for(var k = 0 ; k < subPages.length ; k++)
			{
				$(subPages[k]).hide();
			}        
			showPage(24);			
		};
	
		showPage = function(page) {
			i = page - 1; 
			if (subPages[i]) {   
				$(subPages[lastPage]).hide();
				lastPage = i;
				$(subPages[lastPage]).show();    
			}
		};	
		init();
	}
	</script>
</head>
<!--[if (lte ie 9) ]>      <body class="ie9">          <![endif]-->
<!--[if (gt IE 9) ]> <body >       <![endif]-->
    <div id="wrap" oncontextmenu="return false">
        <#include "inc/logo.ftl">
        <div class="popup" id="popup" >
            <a href="#" class="btn_popup_close" onclick="$('#popup').close();"><img src="${Context.contextPath}/images/btn_popup_close.png" width="13" height="12" alt="닫기" /></a>
            <div class="download">
                <div class="item"><img src="${Context.contextPath}/images/sample_box.png"/></div>
                <div class="item"><img src="${Context.contextPath}/images/sample_box.png"/></div>
                <div class="item"><img src="${Context.contextPath}/images/sample_box.png"/></div>
                <div class="item"><img src="${Context.contextPath}/images/sample_box.png"/></div>
                <div class="clear"></div>
                <div class="btn_download"><a href=""><img src="${Context.contextPath}/images/btn_download.png" width="72" height="74" /></a></div>
            </div>
        </div>
        <div class="container">
            <#include "inc/gnb.ftl">
            <!-- gnb -->
            <div class="content">
                <div class="breadcrumbs no-image">
                   홈 > <strong>검색</strong>
                </div>
                <div class="title_detail">${SiteName}</div>
                
                
                <div class="node_info">
                        <form id="formTab" 
                        	action="${Context.contextPath}/Review"
                        	method="POST">
                        	
                            <input id="tabname" name="tabname" type="hidden" value=""/>
                            <ul class="tab_menu">
                            <li onclick="tabMove(this);return false;" class="active"><a href="">AA</a></li> 
                            <li onclick="tabMove(this);return false;" class=""><a href="">BB</a></li> 
                            </ul>
                            </form>
                          
                           
                <div class="search">
                    <div class="search_school school_info">
                        <h2 class="title">측정위치선택</h2>
                        <div class="box">
                            <form id="formNodeSelect" action="#" method="post">
                            	<input id="SiteID" name="SiteID" value="${SiteID}" type="hidden" />
                            	<input id="SiteName" name="SiteName" value="${SiteName}" type="hidden" />
                            	<select name="nodes" id="nodes"></select>
                            	<!-- <input type="image" src="${Context.contextPath}/images/btn_select.png" alt="검색" width="54" height="47" /> -->
                            </form>

                          
                            <div class="list" id="nodeSelectResult">                            
                                <div>서울특별시 용산구 서계동 주연빌딩 8층</div>
                                <div>유엔이 사무실</div>
                            </div>
                            
                            <div id="NodeImage" class="school_image">
                                <img id="MapImage" src="${Context.contextPath}/images/sample_school_image.png" width="337" height="268" />
                            </div>

                        </div><!-- box -->
                    </div> <!-- search_school , school_info-->
                </div><!-- search -->
                <div class="search_information">
                	<div id="tabs">
                		<ul>
					    	<li><a href="#realtime"><span>실시간</span></a></li>
						    <li><a href="#timeseries"><span>시계열</span></a></li>						   
					  	</ul>
					  	<div id="realtime">
					  		<div class="pollutant">
		                        <h3>오염물질 측정 정보 </h3>
		                        <div class="caption"><small>유지기준</small>
		                            <span class="normal">보통</span>
		                            <span class="warning">주의</span>
		                            <span class="danger">나쁨</span>
		                        </div>
		                        <div class="item">
		                            <h4>라돈</h4>
		                            <div class="value">0 PCI/L</div>
		                            <div class="progress">
		                                <div class="progress_on progress_normal" style="width:0%"></div>
		                            </div>
		                        </div>
		                        <div class="item">
		                            <h4>이산화질소</h4>
		                            <div class="value">0 PPM</div>
		                            <div class="progress">
		                                <div class="progress_on progress_danger" style="width:0%"></div>
		                            </div>
		                        </div>
		                        <div class="item">
		                            <h4>홈알데하이드</h4>
		                            <div class="value">0 PPM</div>
		                            <div class="progress">
		                                <div class="progress_on progress_warning" style="width:0%"></div>
		                            </div>
		                        </div>
		                        <div class="item">
		                            <h4>일산화탄소</h4>
		                            <div class="value">0 PPM</div>
		                            <div class="progress">
		                                <div class="progress_on progress_normal" style="width:0%"></div>
		                            </div>
		                        </div>
		                        <div class="item">
		                            <h4>이산화탄소</h4>
		                            <div class="value">0 PPM</div>
		                            <div class="progress">
		                                <div class="progress_on progress_normal" style="width:0%"></div>
		                            </div>
		                        </div>
		                        <div class="item">
		                            <h4>미세먼지</h4>
		                            <div class="value">0 PPM</div>
		                            <div class="progress">
		                                <div class="progress_on progress_warning" style="width:0%"></div>
		                            </div>
		                        </div>
		                    </div><!-- pollutant -->
					  	</div>
					  	<div id="timeseries">
					  		<div class="time_series">
		                        <h3>시계열 정보</h3>
		                        <div class="progress" id="progress" height="20">
		                            <!--<div class="progress_dot" style="left:30%;"><img src="${Context.contextPath}/images/icon_progress_dot.png" width="26" height="30" /></div>!-->
		                        </div>
		                    </div><!-- time_series -->
		                    <div id="timeseriesData">
		                    <div class="pollutant">
		                        <h3>오염물질 측정 정보 </h3>
		                        <div class="caption"><small>유지기준</small>
		                            <span class="normal">보통</span>
		                            <span class="warning">주의</span>
		                            <span class="danger">나쁨</span>
		                        </div>
		                        <div class="item">
		                            <h4>라돈</h4>
		                            <div class="value">4PCI/L</div>
		                            <div class="progress">
		                                <div class="progress_on progress_normal" style="width:50%"></div>
		                            </div>
		                        </div>
		                        <div class="item">
		                            <h4>이산화질소</h4>
		                            <div class="value">(0.05PPM)*0.01</div>
		                            <div class="progress">
		                                <div class="progress_on progress_danger" style="width:10%"></div>
		                            </div>
		                        </div>
		                        <div class="item">
		                            <h4>홈알데하이드</h4>
		                            <div class="value">(0.05PPM)*0.01</div>
		                            <div class="progress">
		                                <div class="progress_on progress_warning" style="width:10%"></div>
		                            </div>
		                        </div>
		                        <div class="item">
		                            <h4>일산화탄소</h4>
		                            <div class="value">(0.05PPM)*0.01</div>
		                            <div class="progress">
		                                <div class="progress_on progress_normal" style="width:60%"></div>
		                            </div>
		                        </div>
		                        <div class="item">
		                            <h4>이산화탄소</h4>
		                            <div class="value">(0.05PPM)*0.01</div>
		                            <div class="progress">
		                                <div class="progress_on progress_normal" style="width:60%"></div>
		                            </div>
		                        </div>
		                        <div class="item">
		                            <h4>미세먼지</h4>
		                            <div class="value">(0.05PPM)*0.01</div>
		                            <div class="progress">
		                                <div class="progress_on progress_warning" style="width:60%"></div>
		                            </div>
		                        </div>
		                    </div><!-- pollutant -->
		                    </div>
					  	</div>
                	</div>
                	
                    
                    <div class="search_detail">
                        <h3>실내공기질 상세측정정보</h3>
                        <form>
                             <table>
                                <tbody>
                                    <tr>
                                        <th>기간선택</th>
                                        <td>
                                            <input type="text" id="from" name="from">
                                            <img src="${Context.contextPath}/images/icon_calendar.png" />
                                            ~
                                            <input type="text"  id="to" name="to">
                                            <img src="${Context.contextPath}/images/icon_calendar.png" />
                                            <select>
                                                <option>최근 일주일</option>
                                            </select>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>위치선택</th>
                                        <td><select id="detailReviewNodeSelect" ></select></td>
                                    </tr>
                                </tbody>
                            </table>
                            <div class="btn_search_detail">
                                <input type="image" src="${Context.contextPath}/images/btn_result.png" />
                            </div>
                        </form>
                    </div>

                </div><!-- search_information -->
  </div>
                <#include "inc/footer.ftl">
               
            </div><!-- content -->           
        </div><!-- container -->
    </div><!-- wrap -->
<!-- container-flud -->
</body>
</html>
