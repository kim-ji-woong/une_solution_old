<#ftl encoding="utf-8">
<!DOCTYPE html>
<html lang=ko>

<head>
	<meta charset=utf-8>
	<meta content="IE=edge" http-equiv=X-UA-Compatible>
	<meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1,minimum-scale=1.0, user-scalable=no">
    <title>U&amp;E</title>
	<#include "inc/include.ftl">
	
	<script>
	var makers = [];
	var map;
	var contentString;
	
	window.initMap = function(){
		var myLatLng = {lat: 36.223562, lng: 127.792266};
		var mapProp = {
		    center:myLatLng,
		    zoom:7
		};
		map=new google.maps.Map(document.getElementById("map"), mapProp);
		loadMarkers();
	}
	
 	function loadMarkers(){
 		$.ajax({
			type : "GET",
			url : "${Context.contextPath}/Marker/Average",
			dataType : "HTML",
			success : function(data) {
				$("#marker").html(data);
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
 	
 	function loadLocationSensorAverage(location){
 	    
 		var params = 'LocationName=' + location; 		
 		$.ajax({
			type : "POST",
			url : "${Context.contextPath}/Average",
			data : params,
			contentType: "application/x-www-form-urlencoded; charset=UTF-8",
			dataType : "HTML",
			success : function(data) {
				$("#popupDetail").html(data);
				$("#popupDetail").dialog('open');
				$("span.ui-dialog-title").text(location);
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
	
	function addMapMarker(map,icon,position,infoString){
	  var infowindow = new google.maps.InfoWindow({content: infoString});
	  var marker = new google.maps.Marker({
	      position: position,
	      icon: '${Context.contextPath}/images/icon_map_'+icon+'.png',
	      map: map,
	      maxWidth: 300      
	  });
	  marker.addListener('click', function() {
	      //infowindow.open(map, marker);
	      loadLocationSensorAverage(infoString);
	  });
	
	  makers.push({'icon':icon,'maker':marker});
	}
	
	function changeMapMaker(key,map){
	  for (var i = 0; i < makers.length; i++) {
	      if(makers[i]['icon'] == key)
	        makers[i]['maker'].setMap(map);
	  }
	}	
			
	$(function() {
			var dialog;		
			dialog = $("#popupDetail").dialog({
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
	
	function closeNoResult(){
		$('#popupNoResult').dialog('close');
	}

	$(document).ready();
    </script>
    <script async defer src="https://maps.googleapis.com/maps/api/js?key=AIzaSyC0Zjs9pcdTAO3UCBJ1PqhHoYhwFZgRXZY&callback=initMap"></script>
</head>
<!--[if (lte ie 9) ]>      <body class="ie9">          <![endif]-->
<!--[if (gt IE 9) ]> <body ><![endif]-->
    <div id="wrap">
    	<div id="popupDetail" class="popup" oncontextmenu="return false" ondragstart="return false">
		</div> 
        <#include "inc/logo.ftl">
        <div class="container">
            <#include "inc/gnb.ftl">
            <!-- gnb -->
            <div class="content">
                <div class="breadcrumbs">
                   <strong>홈</strong>
                </div>
                <div class="bbs">
                    <div class="box marb">
                        <h2 class="title">공지사항</h2>
                        <div class="list">

                        </div>
                    </div>
                    <div class="box">
                        <h2 class="title">안내사항</h2>
                        <div class="list"></div>
                    </div>
                </div><!-- bbs -->
				<div class="map">
                    <div id="map"></div>
                    <div id="marker"></div>
                </div><!-- map -->                
                <#include "inc/footer.ftl">
            </div><!-- content -->           
        </div> <!-- container -->
    </div><!-- wrap -->    
</body>
</html>
