<#ftl encoding="utf-8">
<!DOCTYPE html>
<html lang=en>

<head>
    <meta charset=utf-8>
    <meta content="IE=edge" http-equiv=X-UA-Compatible>
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no">
    <title>U&amp;E</title>
    <#include "../inc/include.ftl">
    <script>
    function bbsSearch(){   	
		ajaxSearchBBSList("#searchForm");
		//$("#searchForm#input#text").innerHTML('');
		
	}
    function ajaxSearchBBSList(name)
	{
		var params = jQuery(name).serialize(); 	
		$.ajax({
			type : "POST",
			url : "/AQM/Admin/searchlist",
			data : params,
			contentType: "application/x-www-form-urlencoded; charset=UTF-8",
			dataType : "HTML",
			success : function(data) {				
					if( data == '')
					{
					}
					else
					{			
						$("#bbs_list").html(data);				
						setPagination("#bbs_list");
					}
				},
				complete : function(data) {				
				},
				error : function(request, status, error) {				
				}
		});
	}
	

    $(document).ready(
    	function() {
			$.ajax({
				type : "GET",
				url : "/AQM/Admin/postlist",
				contentType: "application/x-www-form-urlencoded; charset=UTF-8",
				dataType : "HTML",
				success : function(data) {				
					if( data == '')
					{
					}
					else
					{			
						$("#bbs_list").html(data);				
						setPagination("#bbs_list");
					}
				},
				complete : function(data) {				
				},
				error : function(request, status, error) {				
				}
			})
		}
	);  
	
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
    </script>
</head>
<!--[if (lte ie 9) ]>      <body class="ie9">          <![endif]-->
<!--[if (gt IE 9) ]> <body >       <![endif]-->
<div id="wrap">
    <#include "../inc/logo.ftl"> 
    <div class="container">
        <#include "../inc/gnb.ftl">
        <!-- gnb -->
        <div class="content">
            <div class="breadcrumbs breadcrumbs_no_margin">
                홈 > <strong>관리자</strong>
            </div>
            <div class="well">
                <div class="admin_sub_menu">
                    <h2 class="title_admin">안내사항</h2>
                    <#include "../inc/admin_menu.ftl">
                </div>
                <form id="searchForm" action="#" method="POST" class="bbs_search_form">
                    <span class="select">
                        <select id="type" name="type">
                          <option value="">전체</option>
                          <option value="">제목</option>
                          <option value="">내용</option>
                          <option value="">제목 + 내용</option>
                        </select>
                      </span>
                    <input type="text" id="text" name="text" value="" onfocus="this.value=''" />
                    <input type="image" onclick="bbsSearch();return false;" src="${Context.contextPath}/images/btn_search_text.png" />                    
                </form>
                <div id="bbs" class="bbs_list">
                    <div id="bbs_list" class="bbs_list">
	                    <div id="tablelist">
							<table>
								 <thead>
						            <tr>
						                <th width="150px" class="num">번호</th>
						                <th>제목</th>
						                <th width="150px">작성일</th>
						                <th width="150px">조회</th>
						            </tr>
						        </thead>
								<tbody>	
								</tbody>
							</table>
						</div> 
						<div class="pagination"></div>
                    </div>
                    <div class="write_button">
                        <a href="${Context.contextPath}/Admin/post/new"><img src="${Context.contextPath}/images/btn_write.png" /></a>
                    </div>
                </div>
            </div>
            <!-- well -->
            <#include "../inc/footer.ftl">
        </div>
        <!-- content -->
    </div>
    <!-- container -->
</div>
<!-- wrap -->
</body>

</html>
