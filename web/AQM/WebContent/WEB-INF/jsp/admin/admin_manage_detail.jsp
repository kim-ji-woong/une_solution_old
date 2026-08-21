<%@ page language="java" contentType="text/html; charset=UTF-8"
	pageEncoding="UTF-8"%>
<%@ page import="javax.naming.*"%>
<%@ page import="java.io.*"%>
<%@ page import="java.util.*"%>
<%@ page import="sun.misc.*"%>
<%@page import="org.slf4j.Logger"%>
<%@page import="org.slf4j.LoggerFactory"%>
<%@page import="kr.co.unes.aqm.dto.site.Site"%>
<%@page import="kr.co.unes.aqm.dto.area.*"%>
<%@page import="kr.co.unes.aqm.dto.site.NodeLocation"%>
<%
	Logger logger = LoggerFactory.getLogger(getClass());
	logger.info("JSP Begin");

	Map<String, Object> model = (Map<String, Object>) request.getAttribute("model");
	Site target = (Site) model.get("targetLocation");	
	
	List<NodeLocation> nodeLocList = (List<NodeLocation>)model.get("linkNodes");

%>
<!DOCTYPE html>
<html lang=en>
<head>
<meta charset=utf-8>
<meta content="IE=edge" http-equiv=X-UA-Compatible>
<meta name="viewport"
	content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no">
<title>U&amp;E</title>
<style>
  #feedback { font-size: 1.4em; }
  #selectable .ui-selecting { background: #99D9EA; }
  #selectable .ui-selected { background: #99D9EA; color: white; }
  #selectable { list-style-type: none; margin: 0; padding: 0; width: 100%; }
  #selectable li { margin: 0px; padding: 0.2em; font-size: 1.2em; height: 18px; }
</style>
<%@include file="../inc/include.jsp"%>
<script src="${pageContext.request.contextPath}/js/jquery.form.min.js"></script>
<script src="${pageContext.request.contextPath}/js/searchnode.js?ver=4"></script>
<script>  
	function deleteLocation(url) {
	   	if( confirm("등록된 학교와 연결된 측정소가 삭제됩니다.\n삭제하시겠습니까?"))
	   	{
	   		$.ajax({
				type : "POST",
				url : url,
				contentType: "application/x-www-form-urlencoded; charset=UTF-8",
				dataType : "HTML",
				success : function(data) {				
					window.location.replace('${pageContext.request.contextPath}/Admin/manage');
				},
				complete : function(data) {				
				},
				error : function(request, status, error) {				
				}
			})
	   	}
	   	else
	   	{
	   	}	
	}   
	
	function unlinkNode() {
		//AQM/Site/node/available
		var url = '${pageContext.request.contextPath}/Site/node/available';
		$.ajax({
			type : "GET",
			url : url,
			dataType : "JSON",
			success : function(data) {				
				if( data.Result > 0) {
					$("#nodesAvailable").html('');	
					//$("#detailReviewNodeSelect").html('');
					var htmlStr = '';
					var hasElement = false;
					htmlStr = '<ol id="selectable">';
					$.each(data.Site.values, function(k, v) {						
						htmlStr += '<li class="ui-widget-content" value="' + v.nodeID + '">'
								+ v.nodeName + '</li>';
								
						hasElement = true;
					});
					htmlStr += '</ol>';
					$("#nodesAvailable").html(htmlStr);						
					//$("#detailReviewNodeSelect").html(htmlStr);
					if(hasElement == false)
					{						
					}
					else
					{
						$("#selectable").selectable();	
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
	
	function getLinkInfo(id)
	{
		var url =  '${pageContext.request.contextPath}/Admin/nodelink/detail/' + id;
		mockdata = null;
		var option = {
		type : 'GET',
		url : url,
		dataType : 'json',
		success : function(data) {
				readLinkInfo(data);
			}
		};
		$("#linkForm").ajaxSubmit(option);
	}
	
	function readLinkInfo(jsonData)
	{
		if(jsonData.Result > 0)
		{
			var linkID = jsonData.NodeLinks.NodeLink.LinkID;
			var siteID = jsonData.NodeLinks.NodeLink.SiteID;
			var nodeID = jsonData.NodeLinks.NodeLink.NodeID;
			var mapID = jsonData.NodeLinks.NodeLink.MapID;
			var mapURL = jsonData.NodeLinks.NodeLink.MapURL;	
						
			$("#LinkID").val(linkID);
			$("#SiteID").val(siteID);
			$("#MapID").val(mapID);
			$("#NodeID").val(nodeID);
			
			if( mapURL != '')
			{
				$("#prevImage").attr('src', mapURL); 
			}
			
			getLinkNodeInfo(nodeID);
		}
	}
	
	function getLinkNodeInfo(id) {
		//AQM/Site/node/available
		
		var url = '${pageContext.request.contextPath}/Site/node/available';
		if( id > 0)
			url = url + '/' + id;
		$.ajax({
			type : "GET",
			url : url,
			dataType : "JSON",
			success : function(data) {				
				if( data.Result > 0) {
					$("#nodesAvailable").html('');	
					//$("#detailReviewNodeSelect").html('');
					var htmlStr = '';
					var hasElement = false;
					htmlStr = '<ol id="selectable">';
					$.each(data.Site.values, function(k, v) {	
						if( id == v.nodeID) 
						{
							htmlStr += '<li class="ui-widget-content ui-selected" value="' + v.nodeID + '">'
							+ v.nodeName + '</li>';
						}							
						hasElement = true;
					});
					htmlStr += '</ol>';
					$("#nodesAvailable").html(htmlStr);
					if(hasElement == true)
					{
						$("#selectable").selectable({
							 stop: function() {
						        $( ".ui-selected", this ).each(function() {
						        	var value = $(this).attr('value');						        	
						          //selectNode(value);
						        });
						      }
						});	
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
	
	var popupMode = 1;
	function showNode(data)
	{
		popupMode = 1;		
	
		var id = data.getAttribute("id");
		$("#LinkID").val(id);
	
				
		// get link info
		getLinkInfo(id);		
		var text = $(data).text();
		$("#NodeAddName").val(text);
		showAddNode();
	}


</script>
</head>
<!--[if (lte ie 9) ]>      <body class="ie9">          <![endif]-->
<!--[if (gt IE 9) ]> <body >       <![endif]-->
<div class="popup" style="display:none" id="popup2" oncontextmenu="return false" ondragstart="return false" onselectstart="return false">	
    <div class="save">
    	<form id="linkForm" action="${pageContext.request.contextPath}/Admin/nodelink/new" method="POST" class="bbs_search_form2">
             이름 
        	<input type="text" id="NodeAddName" name="NodeAddName" value="" />
        	<input type="hidden" id="LinkID" name="LinkID"  value=""  /> 
        	<input type="hidden" id="NodeID" name="NodeID"  value=""  /> 
        	<input type="hidden" id="SiteID" name="SiteID"  value="<%= target.getID() %>"  /> 
        	<input type="hidden" id="MapID" name="MapID"  value=""  />
        	<input type="image" src="${pageContext.request.contextPath}/images/btn_save.png" onclick="file_add();return false;"/>
      	</form>
	    <div id="nodesAvailable" class="box_list"></div>
	    <div class="box_list2"><img id="prevImage" src="${pageContext.request.contextPath}/images/sample_box.png" width="260px"  height="260px" alt=""/></div>

	</div>
</div>
<div id="wrap">
	<%@include file="../inc/logo.jsp"%>
	<div class="container">
		<%@include file="../inc/gnb.jsp"%>
		<!-- gnb -->
		<div class="content">
			<div class="breadcrumbs breadcrumbs_no_margin">
				홈 > <strong>학교관리</strong>
			</div>
			<div class="well">
				<div class="admin_sub_menu">
					<h2 class="title_admin">학교정보</h2>
					<%@include file="../inc/admin_menu.jsp"%>
				</div>
				  <form class="bbs_detail" action="" method="">
					<table>
						<thead>
							<tr>
								<th width="80px" class="num">학교이름</th>
								<td colspan="3"><%= target.getName()%></td>
							</tr>
							<tr>
								<th width="80px" class="num">주소</th>
								<td colspan="3"><%= target.getAddress()%></td>
							</tr>
							<tr>
								<th width="80px" class="num">상세주소</th>
								<td colspan="3"><%= target.getDetailAddress()%></td>
							</tr>
							<tr>
								<th width="73px" class="num">연락처</th>
								<td colspan="3"><%= target.getPhone()%></td>
							</tr>
							<tr>
								<th width="73px" class="num">구분</th>
								<td colspan="3"><%= target.getDescription()%></td>

							</tr>
							<tr>
								<th width="73px" class="num">위치</th>
								<td colspan="3">(<%= target.getLocationX()%>,<%= target.getLocationY()%>)</td>

							</tr>
						</thead>
					</table>
					<div class="box_step2">
                        <h3>설치장소</h3>
                        <div class="step floor_list">
                            <div class="list">
                            	<ul>
	                                <%
                            	   int i = 0; 
                            	   for(NodeLocation node : nodeLocList ) {
	                            	   if( i == 0 )
	                            	   {
                            	%>
	                                <li id="<%= node.getID()%>" class="active" onclick="showNode(this);return false;" value="<%= node.getID()%>"><a href=""><%= node.getName() %></a></li>
	                            <% 
	                            		}
                            	   	else
                            	   	{
                            	%>
                            		<li id="<%= node.getID()%>"  value="<%= node.getID() %>" onclick="showNode(this);return false;"><a href=""><%= node.getName() %></a></li>
                         		<%
                            	   	}
                            	   }
                            	%>
                              	</ul>
                              	<!-- 
                           		<div class="action_buttun">
                                <a href="" class="btn_add" onclick="showAddNode();return false;"><img src="${pageContext.request.contextPath}/images/btn_add.png" alt="" /></a>
                            	</div>
                            	-->                       
                    		</div>
                    	</div>
                    </div>
					<div class="btn_submit">
                    	<% if(target != null){ %>    
                  	 	<a href="${pageContext.request.contextPath}/Admin/manage/modify/<%= target.getID() %>"><img src="${pageContext.request.contextPath}/images/icon_bbs_modify.png" alt="수정" width="97" height="49"/></a>
                        <a href="#" onclick="deleteLocation('${pageContext.request.contextPath}/Admin/manage/delete/<%= target.getID() %>');return false;"><img src="${pageContext.request.contextPath}/images/icon_bbs_delete.png" alt="삭제" width="97" height="49"/></a>
                        <a href="${pageContext.request.contextPath}/Admin/manage"><img src="${pageContext.request.contextPath}/images/icon_bbs_list.png" alt="목록" width="97" height="49" /></a>
                      	<% } else { %>
                      	<a href="${pageContext.request.contextPath}/Admin/manage"><img src="${pageContext.request.contextPath}/images/icon_bbs_modify.png" alt="수정" width="97" height="49"/></a>
                        <a href="${pageContext.request.contextPath}/Admin/manage"><img src="${pageContext.request.contextPath}/images/icon_bbs_delete.png" alt="삭제" width="97" height="49"/></a>
                        <a href="${pageContext.request.contextPath}/Admin/manage"><img src="${pageContext.request.contextPath}/images/icon_bbs_list.png" alt="목록" width="97" height="49" /></a>
                      	<% } %>                    
                      </div>		
				</form>
			</div>
			<!-- well -->
			<%@include file="../inc/footer.jsp"%>
			<!-- footer -->
		</div>
		<!-- content -->
	</div>
	<!-- container -->
</div>
<!-- wrap -->
</body>
</html>
