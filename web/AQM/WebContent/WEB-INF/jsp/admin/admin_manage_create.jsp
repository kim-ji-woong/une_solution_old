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
	
	
	$('#formLocation').submit(function(e) {
		e.preventDefault(); //prevent the form from getting submitted       
		
	});
	$('#linkForm').submit(function(e) {
		e.preventDefault(); //prevent the form from getting submitted       
		
	});
	
	function saveContent() {
		var form = document.forms['formLocation'];
		setForm(form, "nodeAddedlist");
		form.submit();
	}
	
	function setForm(form, name)
	{
		var list = document.getElementById(name);
		var links = list.getElementsByTagName("li");
        for (i = 0; i < links.length; i++) {
            input = document.createElement('input');
            input.type = 'hidden';
            input.name = 'attach_link';
            input.value = links[i].value;
            form.appendChild(input);
        }
	}
		
	// Image map 업로드
	function uploadFile(){
		mockdata = null;
		var option = {
		type : 'POST',
		url : '${pageContext.request.contextPath}/Map/new',
		dataType : 'json',
		success : function(data) {
			done(data);
			}
		};
		$("#fileform").ajaxSubmit(option);
	}
	
	
	// Image map 업로드 결과 받기, url은 사용할 수 있다.
	// fileid를 기억해둔다.
	var uploadImage = false;
	function done(resultJson) {

		if(resultJson.Result > 0)
		{
			var _mockdata = {				
				'attachurl': resultJson.Upload.UploadInfo.attachurl,
				'filemime': resultJson.Upload.UploadInfo.filemime,
				'filename': resultJson.Upload.UploadInfo.filename,
				'filesize': resultJson.Upload.UploadInfo.filesize,
				'fileid' : resultJson.Upload.UploadInfo.fileid
			};
			dataAdd(_mockdata);
		}
		else
		{
			alert("파일 첨부 오류가 발생하였습니다.\n서버 관리자에게 문의바랍니다.");
			uploadImage = false;
		}
	}
	
	function dataAdd(data)
	{
		$("#MapID").val(data.fileid);	
	}
	
	
	
    function nodeItemAdd()
	{
    	var text = $("#NodeAddName").val();	
    	if( text == '')
    	{
    		alert('이름을 입력해야 합니다.');
    		return false;
    	}
    	if(selectedNode == false)
		{
    		alert('노드를 선택해야 합니다.');
    		return false;
		}
    	// add url
    	var url = '${pageContext.request.contextPath}/Admin/nodelink/new';
    	var linkID = $("#LinkID").val();
    	
    	// Link ID가 존재하는경우 수정
    	if( linkID != '' )
    		url = '${pageContext.request.contextPath}/Admin/nodelink/modify/' + linkID;
    	
    	var option = {
			type : 'POST',
			url : url,
			dataType : 'json',
			success : function(data) {
				saveNodeLink(data);
			}
		};    	
		$("#linkForm").ajaxSubmit(option);
	} 
    
    function saveNodeLink(jsonData)
    {
    	// nodelink id 받기
    	if(jsonData.Result > 0)
    	{
    		var linkID = jsonData.NodeLinks.NodeLink.LinkID;
			var siteID = jsonData.NodeLinks.NodeLink.SiteID;
			var nodeID = jsonData.NodeLinks.NodeLink.NodeID;			
			
    		var text = $("#NodeAddName").val();			
    		var list = document.getElementById("nodeAddedlist");
    		// find
    		var array = document.querySelectorAll('#nodeAddedlist>li');
    		for(i = 0; i < array.length; i++)
    		{
    			var item = $(array[i]).val();
    			if(item == linkID)
    			{
    				list.removeChild(array[i]);
    			}
    		}    		
    		var addField = document.createElement('li');
    		addField.setAttribute('id', linkID);
    		addField.setAttribute('name', linkID); // nodeid
    		addField.setAttribute('value',linkID); // 이름
    		addField.setAttribute('onclick', "showEditNode(this);return false;")
    		addField.innerHTML = '<a href="">' + text + '</a>';    		
    		list.appendChild(addField);
    	}
    	else
		{
			alert("파일 첨부 오류가 발생하였습니다.\n서버 관리자에게 문의바랍니다.");
			uploadImage = false;
		}
		$("#popup2").dialog('close');
    } 
    
	function preViewImg(input) {
				  
		if ( window.FileReader ) {
			 /*IE 9 이상에서는 FileReader  이용*/
			var reader = new FileReader();
		        reader.onload = function (e) {
		        	$("#prevImage").attr('src', e.target.result); 
		        };
		        reader.readAsDataURL(input.files[0]);
		        return input.files[0].name;  // 파일명 return
		} else {
			 /* IE8 전용 이미지 미리보기 */ 
			input.select();
			var src = document.selection.createRange().text;
			$("#prevImage").attr('src', src);  
			var n = src.substring(src.lastIndexOf('\\') + 1);
			return n; // 파일명 return
		}		        
    }

	var selectedNode = false;
	function unlinkNode(id) {
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
						else
						{
							htmlStr += '<li class="ui-widget-content" value="' + v.nodeID + '">'
							+ v.nodeName + '</li>';
						}	
						hasElement = true;
					});
					htmlStr += '</ol>';
					$("#nodesAvailable").html(htmlStr);			
					if( id > 0)
					{						
						selectNode(id);						
					}
						
					//$("#detailReviewNodeSelect").html(htmlStr);
					if(hasElement == false)
					{						
					}
					else
					{
						$("#selectable").selectable({
							 stop: function() {
						        $( ".ui-selected", this ).each(function() {
						        	var value = $(this).attr('value');						        	
						          selectNode(value);
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
	
	function selectNode(data)
	{
		$("#NodeID").val(data);		
		selectedNode = true;
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
		$("#fileform").ajaxSubmit(option);
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
			
			unlinkNode(nodeID);
		}
	}
	var popupMode = 1;
	function showEditNode(data)
	{
		popupMode = 1;
		
		selectNode('');
		selectedNode = false;
		
		var id = data.getAttribute("id");
		$("#LinkID").val(id);
	
				
		// get link info
		getLinkInfo(id);		
		var text = $(data).text();
		$("#NodeAddName").val(text);
	
		document.getElementById("removeBtn").style.visibility = "visible";
		
		showAddNode();	
	}
	
	function showNewNode()
	{		
		popupMode = 2;
		selectNode('');
		selectedNode = false;
		$("#LinkID").val('');	
		$("#NodeAddName").val('');
		resetFormElement($('#file'));
		$("#prevImage").attr('src', ''); 
		
		document.getElementById("removeBtn").style.visibility = "hidden";
		unlinkNode();
		showAddNode();
	}
	
	function nodeItemRemove()
	{
    	var linkID = $("#LinkID").val();
    	var list = document.getElementById("nodeAddedlist");
    	var array = Array.from(document.querySelectorAll('#nodeAddedlist>li'));
		for(i = 0; i < array.length; i++)
		{
			var item = $(array[i]).val();
			if(item == linkID)
			{
				list.removeChild(array[i]);
			}
		}   
	}
	 
	function deleteNode()
	{
		var linkID = $("#LinkID").val();
		var siteID = $("#SiteID").val();
		
		if( linkID != '' )
		{
    		url = '${pageContext.request.contextPath}/Admin/nodelink/delete/' + linkID;    	
	    	var option = {
				type : 'POST',
				url : url,
				dataType : 'json',
				success : function(data) {
					saveNodeLink(data);
				}
			};    
			$("#linkForm").ajaxSubmit(option);
		}		
		nodeItemRemove();
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
        	<input type="hidden" id="SiteID" name="SiteID"  value=""  /> 
        	<input type="hidden" id="MapID" name="MapID"  value=""  />
        	<input type="image" src="${pageContext.request.contextPath}/images/btn_save.png" onclick="file_add();return false;"/>
      	</form>
	    <div id="nodesAvailable" class="box_list"></div>
	    <div class="box_list2"><img id="prevImage" src="${pageContext.request.contextPath}/images/sample_box.png" width="260px"  height="260px" alt=""/></div>
	   	      
	    <div class="btn_submit">
			<a href="#" class="btn_add" onclick="nodeItemAdd();return false;" ><img src="${pageContext.request.contextPath}/images/btn_admin_save.png" /></a>
			<a id="removeBtn" href="#" class="btn_add" onclick="deleteNode();return false;"><img src="${pageContext.request.contextPath}/images/btn_info_remove.png" /></a>
		</div>
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
					<h2 class="title_admin">학교수정</h2>
					<%@include file="../inc/admin_menu.jsp"%>
				</div>
				<form id="formLocation" class="bbs_detail"
					action="${pageContext.request.contextPath}/Admin/manage/new"
					method="POST">
					<table>
						<thead>
							<tr>
								<th width="80px" class="num">학교이름</th>
								<td colspan="3"><input type="text" name="Name"
									placeholder="이름을 입력하세요" value=""></input></td>
							</tr>
							<tr>
								<th width="80px" class="num">주소</th>
								<td colspan="3"><input type="text" name="Address"
									placeholder="주소를 입력하세요" value="" /> <input type="button"
									name="showAddress" value="검색"></input></td>
							</tr>
							<tr>
								<th width="80px" class="num">상세주소</th>
								<td colspan="3"><input type="text" name="DetailAddress"
									placeholder="상세주소를 입력하세요" value="" /></td>
							</tr>
							<tr>
								<th width="73px" class="num">연락처</th>
								<td colspan="3"><input type="text" name="Phone" value=""
									placeholder="전화번호를 입력하세요" /></td>
							</tr>
							<tr>
								<th width="73px" class="num">구분</th>
								<td colspan="3"><input type="text" name="Description" value=""
									placeholder="설명을 입력하세요" /></td>

							</tr>
						</thead>
					</table>
					<div class="box_step2">
                        <h3>설치장소</h3>
                        <div class="step floor_list">
                            <div class="list">
                            	<ul id="nodeAddedlist">                            	
                              	</ul>
                           		<div class="action_buttun">
                                <a href="" class="btn_add" onclick="showNewNode();return false;"><img src="${pageContext.request.contextPath}/images/btn_add.png" alt="" /></a>
                            	</div>                       
                    		</div>
                    	</div>
                    </div>					
					<div class="btn_submit">
						<input type="image" onclick="saveContent();return false;"
							src="${pageContext.request.contextPath}/images/btn_admin_save.png" />
					</div>
				</form>					
				<form id="fileform" name="fileform" method="POST" enctype="multipart/form-data" action="/AQM/Map/new" class="hidden">
					<input type="file" name="file" id="file" />
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
<script type="text/javascript">
	function file_add() {
		$('#file').trigger('click');		
		return false;
	}
	
	$(document)
			.on(
					'click',
					'.floor_list .list ul li',
					function() {
						if ($(this).hasClass('active'))
							return false;

						$('.floor_list .list ul li').removeClass('active')
						$(this).addClass('active');
						$('#prevImage')
								.attr('src',
										'${pageContext.request.contextPath}/images/sample_map.png');
					});
</script>

<script type="text/javascript">
	$('#file').on('change', function() {
		ext = $(this).val().split('.').pop().toLowerCase();		
		if ($.inArray(ext, [ 'gif', 'png', 'jpg', 'jpeg' ]) == -1) {
			resetFormElement($(this)); //폼 초기화
			window.alert('이미지 파일이 아닙니다! (gif, png, jpg, jpeg 만 업로드 가능)');
		} else {
			uploadFile();
			preViewImg(this);
			
		}
		return false;
	});

	$('#remove_image').bind('click', function() {
		$('#prevImage').attr('src', '');
		resetFormElement($('#file')); //전달한 양식 초기화
		return false; //기본 이벤트 막음
	});

	function resetFormElement(e) {
		e.wrap('<form>').closest('form').get(0).reset();
		//리셋하려는 폼양식 요소를 폼(<form>) 으로 감싸고 (wrap()) , 
		//요소를 감싸고 있는 가장 가까운 폼( closest('form')) 에서 Dom요소를 반환받고 ( get(0) ),
		//DOM에서 제공하는 초기화 메서드 reset()을 호출
		e.unwrap(); //감싼 <form> 태그를 제거
	}
</script>
</html>
