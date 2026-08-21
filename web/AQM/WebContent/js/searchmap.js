var markers = [];
var map;
function initMap() {
	var myLatLng = {
			lat : 37.558432,
			lng : 127.140612
	};
	var mapProp = {
			center : myLatLng,
			zoom : 13
	};
	map = new google.maps.Map(document.getElementById("map"), mapProp);
	var contentString;	
	// Add Maker 
};

function addMapMarker(map, icon, position, infoString, locationName){
	var infowindow = new google.maps.InfoWindow({content: infoString});
	var marker = new google.maps.Marker({
		position: position,
		icon: './images/icon_map_'+icon+'.png',
		map: map
	});
	marker.addListener('click', function() {
		//infowindow.open(map, marker);
		loadSensorAverageForLocation(infoString, locationName);
	});

	markers.push(marker);
}	

function loadSensorAverageForLocation(locationInfo, locationName){
	var params = 'LocationID='+ locationInfo + '&LocationName=' + locationName; 		
	$.ajax({
		type : "POST",
		url : "/AQM/Search/nodeAverage",
		data : params,
		contentType: "application/x-www-form-urlencoded; charset=UTF-8",
		dataType : "HTML",		
		success : function(data) {
			$("#popupDetail").html(data);
			$("#popupDetail").dialog('open');
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


function clearMarkers() {
	changeMapMaker(null);
}	

function showMarkers() {
	changeMapMaker(map);
}

function deleteMarkers() {
	clearMarkers();
	markers = [];
}

function moveMap(x, y)
{
	var myLatLng = {
			lat : x,
			lng : y
	};
	map.setCenter(myLatLng);
};

function changeMapMaker(map){
	for (var i = 0; i < markers.length; i++) {	    
		markers[i].setMap(map);
	}
}	

