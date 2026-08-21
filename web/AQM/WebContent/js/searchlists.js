
function ajaxSearchList(name)
{
	clearMapMaker();
	var params = jQuery(name).serialize(); 	
	$.ajax({
		type : "POST",
		url : "/AQM/Search/resultList",
		data : params,
		contentType: "application/x-www-form-urlencoded; charset=UTF-8",
		dataType : "HTML",
		success : function(data) {				
			if( data == '')
			{
				$("#popupSearch").dialog("close");
				$("#popupNoResult").dialog("open");
			}
			else
			{						
				$("#search_list").html(data);				
				$("#popupSearch").dialog("close");
				setPagination("#search_list");
			}
		},
		complete : function(data) {
			$("#popupSearch").dialog("close");
		},
		error : function(request, status, error) {
			$("#popupSearch").dialog("close");
			$("#popupNoResult").dialog("open");
		}
	});
}

function clearMapMaker() {
	deleteMarkers();
}

function changeClass(currentPage, newPage, nTotalPage){	
	if(currentPage == newPage)
		return;
	var nLiLoc = 3 + currentPage;
	if(currentPage == 0){      
		$('.pagination li:nth-child('+ nLiLoc +')').attr("class", "start");	 		
	}
	else if(currentPage == (nTotalPage - 1)){ 		
		$('.pagination li:nth-child('+nLiLoc+')').attr("class", "end");	 	
	}
	else {
		$('.pagination li:nth-child('+nLiLoc+')').attr("class", "");
	}	 	

	nLiLoc = 3 + newPage;
	if(newPage == 0){        	
		$('.pagination li:nth-child('+nLiLoc+')').attr("class", "start active");	 		
	}
	else if(newPage == (nTotalPage - 1)){
		$('.pagination li:nth-child('+nLiLoc+')').attr("class", "end active");	 	
	}
	else{
		$('.pagination li:nth-child('+nLiLoc+')').attr("class", "active");
	}
}

var currentPagination = 1;

function setPagination(name) {
	var objContent = $(name);	   
	var subPages = new Array();  
	var lastPage = 0;
	var nPageCount = 0;		

	init = function() {
		objContent.children().each(function(i){ 
			if(this.id == "tablelist")
			{
				subPages.push(this);
				nPageCount++;
			}
		});
		for(var k = 0 ; k < subPages.length ; k++)
		{
			$(subPages[k]).hide();
		}        
		showPage(lastPage);      
		showPagination(nPageCount);
	};

	showPage = function(page) {
		i = page; 
		if (subPages[i]) {                
			changeClass(lastPage, i, nPageCount);                   
			$(subPages[lastPage]).hide();
			lastPage = i;
			$(subPages[lastPage]).show();    
		}
	};

	showPagination = function(numPages) {
		var pagins = '';
		var lastPage = numPages -1;
		if( numPages > 0)
		{
			for (var i = 0; i < numPages; i++) {
				var nPage = i+1;
				if( i == 0){
					pagins += '<li class="start active"><a href="#" onclick="showPage(' + i + '); return false;">' +nPage + '</a></li>';
				}
				else
				{      
					if( i == lastPage){
						pagins += '<li class="end"><a href="#" onclick="showPage(' + i + '); return false;">' +nPage + '</a></li>';
					}  
					else {    
						pagins += '<li><a href="#" onclick="showPage(' + i + '); return false;">' + nPage + '</a></li>';
					}
				}
			}
			$('.pagination li:nth-child(2)').after(pagins);
		}	

	};

	init();

	$('.pagination #prev').click(function() {
		showPage(lastPage-1);
	});

	$('.pagination #next').click(function() {
		showPage(lastPage+1);
	});	

	$('.pagination #first').click(function() {
		showPage(0);
	});	

	$('.pagination #last').click(function() {
		showPage(nPageCount-1);
	}); 

}

