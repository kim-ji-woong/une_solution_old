var selectedDepth1Value = '';
var selectedDepth2Value = '';
var selectedDepth3Value = '';
var selectedDepth4Value = '';

function ajxComplete(data1) {
	//alert(data);
}

function ajaxError(request, status, error) {
	alert("code:" + request.status + "\n" + "message:"
			+ request.responseText + "\n" + "error:" + error);
}

function ajax4Error(request, status, error) {
}

function clearSelected(name, func) {		
	$(name).val("");
	$(name).selectmenu('destroy').selectmenu({
		style : 'dropdown'
	});
	$(name).selectmenu({
		change :func,
		select :func
	});
}

function deptp1SelectChanged(event, data) {
	selectedDepth2Value = '';
	selectedDepth3Value = '';
	selectedDepth4Value = '';
	selectedDepth1Value = data.item.value;

	clearSelected("#depth2", deptp2SelectChanged);
	clearSelected("#depth3", deptp3SelectChanged);
	clearSelected("#depth4", deptp4SelectChanged);

	$.ajax({
		type : "GET",
		url : "/AQM/Area/depth2s/" + selectedDepth1Value,
		dataType : "JSON",
		success : addDepth2,
		complete : ajxComplete,
		error : ajaxError
	});
}

function deptp2SelectChanged(event, data) {
	selectedDepth3Value = '';
	selectedDepth4Value = '';
	selectedDepth2Value = data.item.value;
	clearSelected("#depth3", deptp3SelectChanged);
	clearSelected("#depth4", deptp4SelectChanged);
	$.ajax({
		type : "GET",
		url : "/AQM/Area/depth3s/" + selectedDepth1Value + '/'
		+ selectedDepth2Value,
		dataType : "JSON",
		success : addDepth3,
		complete : ajxComplete,
		error : ajaxError
	});
}

function deptp3SelectChanged(event, data) {		

	selectedDepth4Value = '';
	selectedDepth3Value = data.item.value;
	clearSelected("#depth4", deptp4SelectChanged);
	$.ajax({
		type : "GET",
		url : "/AQM/Area/depth4s/" + selectedDepth1Value
		+ '/' + selectedDepth2Value + '/'
		+ selectedDepth3Value,
		dataType : "JSON",
		success : addDepth4,
		complete : ajxComplete,
		error : ajax4Error
	});
}

function deptp4SelectChanged(event, data) {	
	selectedDepth4Value = data.item.value;
}	

function addDepth2(data1) {
	var htmlStr = '';
	$.each(data1.Area.depth2s, function(k, v) {
		htmlStr += '<option value="' + v.depth2 + '">' + v.depth2
		+ '</option>';
	});		
	$("#depth2").html(htmlStr);	
}

function addDepth3(data1) {
	$("#depth3").html('');
	var htmlStr = '';
	$.each(data1.Area.depth3s, function(k, v) {
		htmlStr += '<option value="' + v.depth3 + '">' + v.depth3
		+ '</option>';
	});
	$("#depth3").html(htmlStr);		
}

function addDepth4(data1) {
	var htmlStr = '';
	$("#depth4").attr("disable", true);
	$.each(data1.Area.depth4s, function(k, v) {

		if (v.depth4 != "") {
			htmlStr += '<option value="' + v.depth4 + '">' + v.depth4
			+ '</option>';
		}
	});
	$("#depth4").html(htmlStr);
}	

$(function() {
	$("#depth1").selectmenu({
		change :deptp1SelectChanged,
		select :deptp1SelectChanged 
	});
	$("#depth2").selectmenu({
		change :deptp2SelectChanged,
		select :deptp2SelectChanged 
	});
	$("#depth3").selectmenu({
		change :deptp3SelectChanged,
		select :deptp3SelectChanged 
	});
	$("#depth4").selectmenu({
		change : deptp4SelectChanged,
		select :deptp4SelectChanged 
	});
});